#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using KolikSoftware.Eee.Client.Helpers;
using KolikSoftware.Eee.Client.LoginProcess;
using KolikSoftware.Eee.Client.Media;
using KolikSoftware.Eee.Client.Notifications;
using KolikSoftware.Eee.Client.Reporting;
using KolikSoftware.Eee.Client.Updating;
using KolikSoftware.Eee.Processor;
using KolikSoftware.Eee.Service;
#endregion

namespace KolikSoftware.Eee.Client
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        #region Private Members
        enum FormClosingMode
        {
            None,
            Exit,
            Shutdown
        }

        struct InvisibleMessageProperties
        {
            public int MessageId;
            public int UserId;
            public int RoomId;
        }

        FormClosingMode closingMode = FormClosingMode.None;

        bool canShutdown = false;
        bool layoutLoaded = false;
        bool firstGet = true;

        DateTime currentDay = DateTime.Today;
        DateTime startingDay = DateTime.Today;

        Dictionary<int, InvisibleMessageProperties> invisibleUserMessages = new Dictionary<int, InvisibleMessageProperties>();
        Dictionary<int, InvisibleMessageProperties> invisibleRoomMessages = new Dictionary<int, InvisibleMessageProperties>();
        #endregion

        void AddInvisibleUserMessage(int messageId, int userId)
        {
            InvisibleMessageProperties properties = new InvisibleMessageProperties();
            properties.MessageId = messageId;
            properties.UserId = userId;
            this.invisibleUserMessages[messageId] = properties;
        }

        void AddInvisibleRoomMessage(int messageId, int roomId)
        {
            InvisibleMessageProperties properties = new InvisibleMessageProperties();
            properties.MessageId = messageId;
            properties.RoomId = roomId;
            this.invisibleRoomMessages[messageId] = properties;
        }

        void RemoveInvisibleMessage(int messageId)
        {
            if (this.invisibleUserMessages.ContainsKey(messageId))
                this.invisibleUserMessages.Remove(messageId);

            if (this.invisibleRoomMessages.ContainsKey(messageId))
                this.invisibleRoomMessages.Remove(messageId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.normalFont = new Font(this.searchLabel.Font, FontStyle.Regular);
            this.boldFont = new Font(this.searchLabel.Font, FontStyle.Bold);

            this.Text = Application.ProductName;
            this.statusLabel.Text = Application.ProductName;
            this.notifyIcon.Text = Application.ProductName;

            this.eeeServiceController.Service = Global.Instance.CreateService();

            if (Properties.Settings.Default.UpgradeSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
                Properties.Settings.Default.Save();

                ResetSettingsForThisVersion();
            }

            this.messageCheckerTimer.Interval = Properties.Settings.Default.RefreshRate * 1000;

            this.usersToolStrip.ImageList = this.userImages;
            this.roomsToolStrip.ImageList = this.roomImages;

            LoadLayout();

            this.loginManager.Login(this);
        }

        void ResetSettingsForThisVersion()
        {
            Properties.Settings.Default.MessageTemplateNo = 4;
            Properties.Settings.Default.EnterSends = true;
            Properties.Settings.Default.Save();
        }

        void LoadLayout()
        {
            Application.EnableVisualStyles();

            if (Properties.Settings.Default.ResetLayout == false)
            {
                ToolstripSettingsSerializer.Deserialize(this.toolbarContainer, Properties.Settings.Default.ToolStripSettings);
            }
            else
            {
                Properties.Settings.Default.ResetLayout = false;
            }

            LoadSmilieToolbar();

            RefreshUnbindableProperties(false);

            CheckUpdateState();
            this.layoutLoaded = true;
        }

        List<MemoryStream> smilieStreams;

        void LoadSmilieToolbar()
        {
            bool first = true;

            this.smilieStreams = new List<MemoryStream>();

            foreach (KeyValuePair<string, string> smilieFile in Smilies.Instance.SmilieFiles)
            {
                ToolStripButton smilieButton = new ToolStripButton();
                smilieButton.Size = new Size(23, 22);
                if (first)
                {
                    smilieButton.Image = Properties.Resources.Smilie;
                    smilieButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                }
                else
                {
                    MemoryStream smilieStream = new MemoryStream();
                    smilieStreams.Add(smilieStream);

                    using (Stream fileStream = File.OpenRead(Path.Combine(Smilies.SmiliesPath, smilieFile.Key)))
                    {
                        int count;
                        byte[] buffer = new byte[32000];

                        while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            smilieStream.Write(buffer, 0, count);
                        }
                    }

                    smilieButton.Image = new Bitmap(smilieStream);
                    smilieButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    smilieButton.TextImageRelation = TextImageRelation.ImageAboveText;
                    smilieButton.ImageScaling = ToolStripItemImageScaling.None;
                }
                smilieButton.Text = Path.GetFileNameWithoutExtension(smilieFile.Key);
                smilieButton.ToolTipText = smilieButton.Text;
                smilieButton.Tag = smilieFile.Value;
                if (first == false)
                    smilieButton.Overflow = ToolStripItemOverflow.Always;
                else
                    first = false;
                smilieButton.Click += new EventHandler(smilieButton_Click);
                smilieButton.Alignment = ToolStripItemAlignment.Right;
                this.actionsToolStrip.Items.Add(smilieButton);
            }
        }

        Regex smilieSpaceRegex = new Regex(@"[\s~?,.'""()]", RegexOptions.Compiled | RegexOptions.Singleline);

        void smilieButton_Click(object sender, EventArgs e)
        {
            string smilie = (sender as ToolStripButton).Tag as string;

            if (this.text.SelectionStart > 0 && this.smilieSpaceRegex.IsMatch(this.text.Text[this.text.SelectionStart - 1].ToString()) == false)
                smilie = " " + smilie;
            if (this.text.SelectionStart + this.text.SelectionLength < this.text.Text.Length - 1 && this.smilieSpaceRegex.IsMatch(this.text.Text[this.text.SelectionStart + this.text.SelectionLength].ToString()) == false)
                smilie += " ";
            this.text.SelectedText = smilie;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.ShowInTaskbar == false && this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }

            if (this.WindowState != FormWindowState.Minimized)
                this.lastState = this.WindowState;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                MinimizeMe();
            }
        }

        void Copy()
        {
            if (this.chatBrowser.Document != null)
                this.chatBrowser.Document.ExecCommand("Copy", false, null);
        }

        void ClearBrowserContent(WebBrowser webBrowser)
        {
            this.contentElement = this.historyElement = null;
            webBrowser.DocumentText = "<html><head></head><body><div id='Content' style='" + VisibleMasterStyle + "'></div><div id='History' style='" + InvisibleMasterStyle + "'></div></body></html>";
            WaitForBrowser();
        }

        void WaitForBrowser()
        {
            while (this.contentElement == null)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }

        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsolutePath != "blank")
            {
                e.Cancel = true;

                if (e.Url.AbsoluteUri.EndsWith("#MEDIA"))
                {
                    PlayLink(e.Url.AbsoluteUri.Substring(0, e.Url.AbsoluteUri.Length - 6));
                }
                else
                {
                    BackgroundWorker browserStarter = new BackgroundWorker();
                    browserStarter.DoWork += new DoWorkEventHandler(browserStarter_DoWork);
                    browserStarter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(browserStarter_RunWorkerCompleted);
                    browserStarter.RunWorkerAsync(e.Url.AbsoluteUri);
                }
            }
        }

        string linkToPlay = null;

        void PlayLink(string link)
        {
            string destinationDir = this.eeeServiceController.GetPathToSave("Downloads");

            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            string fileName = link;
            int lastSlashIdx = fileName.LastIndexOf('/');
            fileName = fileName.Substring(lastSlashIdx + 1);
            string filePath = Path.Combine(destinationDir, fileName);

            if (File.Exists(filePath))
            {
                PlayFile(link, filePath);
            }
            else
            {
                this.linkToPlay = link;
                this.eeeServiceController.DownloadFile(link, destinationDir);
                UpdateDownloadStatus(null);
            }
        }

        void PlayFile(string link, string filePath)
        {
            this.mediaPlayer.SetFile(link, filePath);
            this.mediaPlayer.SetVolume(Properties.Settings.Default.MediaVolume);
            this.volumeDownToolItem.ToolTipText = string.Format("Volume Down ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            this.volumeUpToolItem.ToolTipText = string.Format("Volume Up ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            this.playToolItem.ToolTipText = "Play " + this.mediaPlayer.TrackInfo;
            this.mediaPlayer.Play();
        }

        void browserStarter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                ShowError(e.Error);
            ((BackgroundWorker)sender).Dispose();
        }

        void browserStarter_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Argument.ToString());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.ClosingMode == FormClosingMode.None)
            {
                if (e.CloseReason == CloseReason.WindowsShutDown)
                {
                    Exit(FormClosingMode.Shutdown);
                }
                else if (this.active)
                {
                    e.Cancel = true;
                    MinimizeMe();
                }
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            BecomeActive();
            this.text.Focus();

            this.notificationManager.Enabled = false;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            BecomeInactive();

            if (this.loginManager.IsConnected)
                this.notificationManager.Enabled = true;
        }

        void Logout()
        {
            this.eeeServiceController.DisconnectJabber();
            this.eeeServiceController.DisconnectUser(this.ClosingMode != FormClosingMode.None);
        }

        #region Menu Methods
        void About()
        {
            using (AboutBox about = new AboutBox())
            {
                about.ShowDialog();
            }
        }

        void Exit(FormClosingMode closingMode)
        {
            SaveLayout();

            this.ClosingMode = closingMode;

            if (this.loginManager.IsConnected)
            {
                Logout();

                if (closingMode == FormClosingMode.Shutdown)
                {
                    while (this.canShutdown == false)
                    {
                        Application.DoEvents();
                        Thread.Sleep(200);
                    }
                }
            }
            else if (closingMode == FormClosingMode.Exit)
            {
                Close();
            }
            else
            {
                FinishShutdown();
            }
        }

        void SaveLayout()
        {
            this.usersToolStrip.MinimumSize = new Size(0, 0);
            this.usersToolStrip.Items.Clear();
            this.usersToolStrip.PerformLayout();
            this.roomsToolStrip.MinimumSize = new Size(0, 0);
            this.roomsToolStrip.Items.Clear();
            this.roomsToolStrip.PerformLayout();

            Properties.Settings.Default.ToolStripSettings = ToolstripSettingsSerializer.Serialize(this.toolbarContainer);

            Properties.Settings.Default.Save();
        }

        void Help()
        {
            System.Diagnostics.Process.Start(Properties.Resources.HomePageUrl);
        }
        #endregion

        #region Processing Methods
        void SetAwayMode(bool away, string comment)
        {
            if (this.loginManager.IsConnected)
            {
                if (away)
                    this.eeeServiceController.SetAwayMode(comment);
                else
                    this.eeeServiceController.ResetAwayMode();

                this.eeeServiceController.GetMessages(new TimeSpan(0, 0, 2));
            }
        }

        #region User
        List<int> ignoredUsers = new List<int>();

        void AddUser(EeeDataSet.UserRow user)
        {
            ToolStripButton button = new ToolStripButton(user.Login);
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.CheckOnClick = true;
            button.Tag = user;
            button.CheckedChanged += new EventHandler(userButton_CheckedChanged);
            button.MouseUp += new MouseEventHandler(userButton_MouseUp);

            if (user.UserID == this.eeeServiceController.CurrentUser.UserID)
                this.usersToolStrip.Items.Insert(0, button);
            else
                this.usersToolStrip.Items.Add(button);

            this.notificationManager.AddNotification("Connected", 0, user.Login, MessageType.Connection);

            button.MouseHover += new EventHandler(userButton_MouseHover);
            button.MouseLeave += new EventHandler(userButton_MouseLeave);
            //PlaySound(SettingsManager.ConnectSound);
        }

        void userButton_MouseLeave(object sender, EventArgs e)
        {
            this.userInfoToolTip.Hide(this.usersToolStrip);
        }

        void userButton_MouseHover(object sender, EventArgs e)
        {
            SetUserInfo((ToolStripButton)sender);
        }

        void userButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolStripButton button = sender as ToolStripButton;
                Point newPoint = new Point(e.Location.X + button.Bounds.X, e.Location.Y + button.Bounds.Y);

                bool currentlyIgnored = button.ForeColor == Color.Gray;

                this.ignoreUserItem.Visible = !currentlyIgnored;
                this.stopIgnoringUserItem.Visible = currentlyIgnored;

                this.stopIgnoringAllUsersItem.Visible = this.ignoredUsers.Count > 0;

                this.userMenu.Tag = button;
                this.userMenu.Show(this.usersToolStrip.PointToScreen(newPoint));
            }
        }

        void userButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.freezeEvents) return;
            this.freezeEvents = true;

            ToolStripButton button = sender as ToolStripButton;

            if (button.Checked && ((EeeDataSet.UserRow)button.Tag).UserID != this.eeeServiceController.CurrentUser.UserID)
            {
                UncheckUserButtons(button);
                UncheckExternalButtons(null);
            }

            RearrangeMessages();

            this.freezeEvents = false;
        }

        void RemoveUser(EeeDataSet.UserRow user)
        {
            ToolStripButton button = GetUserButton(user);
            if (button != null)
            {
                this.usersToolStrip.Items.Remove(button);
                this.notificationManager.AddNotification("Disconnected", 0, user.Login, MessageType.Connection);
                //PlaySound(SettingsManager.ConnectSound);
            }
        }

        void SetUserInfo(ToolStripButton button)
        {
            if (button != null)
            {
                EeeDataSet.UserRow user = GetUser(button);

                string state = "Active";

                if (user.State == (int)UserState.Away)
                {
                    if (user.IsAwayModeCommentNull() == false && user.AwayModeComment.Trim().Length > 0)
                        state = user.AwayModeComment;
                    else
                        state = "Away";
                }

                string since;

                if (user.IsLoginTimeNull())
                    since = "N/A";
                else
                    since = user.LoginTime.ToString();

                string info = string.Format("Client: {2}\nState: {0}\nConnected since: {1}", state, since, user.Client);

                this.userInfoToolTip.ToolTipTitle = user.Login;
                this.userInfoToolTip.RemoveAll();
                this.userInfoToolTip.Show(info, this.usersToolStrip, button.Bounds.X + button.Bounds.Width - 10, button.Bounds.Y + button.Bounds.Height / 2);
            }
        }

        void SetUserAway(EeeDataSet.UserRow user)
        {
            ToolStripButton button = GetUserButton(user);

            if (button != null)
            {
                //PlaySound(SettingsManager.ConnectSound);

                EeeDataSet.UserRow originalUser = GetUser(button);
                originalUser.State = user.State;
                if (user.IsAwayModeCommentNull() || user.AwayModeComment.Length == 0)
                    originalUser.AwayModeComment = "";
                else
                    originalUser.AwayModeComment = user.AwayModeComment;

                SetUserColor(button);
            }

            if (user.UserID == this.eeeServiceController.CurrentUser.UserID)
            {
                this.notificationManager.AwayMode = true;
                bool freezeEventsBackup = this.freezeEvents;
                this.freezeEvents = true;
                this.awayModeToolItem.Checked = true;
                this.awayModeNotifyItem.Checked = true;
                this.awayModeMenuItem.Checked = true;
                this.freezeEvents = freezeEventsBackup;
            }
        }

        void SetUserNotAway(EeeDataSet.UserRow user)
        {
            ToolStripButton button = GetUserButton(user);
            if (button != null)
            {
                //PlaySound(SettingsManager.ConnectSound);

                EeeDataSet.UserRow originalUser = GetUser(button);
                originalUser.State = user.State;
                originalUser.AwayModeComment = user.AwayModeComment;

                SetUserColor(button);
            }

            if (user.UserID == this.eeeServiceController.CurrentUser.UserID)
            {
                this.notificationManager.AwayMode = false;
                bool freezeEventsBackup = this.freezeEvents;
                this.freezeEvents = true;
                this.awayModeToolItem.Checked = false;
                this.awayModeNotifyItem.Checked = false;
                this.awayModeMenuItem.Checked = false;
                this.freezeEvents = freezeEventsBackup;
            }
        }

        ToolStripButton GetUserButton(EeeDataSet.UserRow user)
        {
            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                //TODO: if (((EeeDataSet.UserRow)button.Tag).UserID == user.UserID)
                if (item.Tag != null)
                {
                    if (((EeeDataSet.UserRow)item.Tag).Login.ToLower() == user.Login.ToLower())
                        return (ToolStripButton)item;
                }
            }

            return null;
        }

        void SetUserIcons()
        {
            int index = 0;

            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                if (index >= 10) return;
                if (item.Tag != null)
                    item.ImageIndex = index++;
            }
        }

        void SynchronizeUsers(EeeDataSet.UserDataTable users)
        {
            foreach (EeeDataSet.UserRow user in users)
            {
                UserState state = (UserState)user.State;

                ToolStripButton userButton = GetUserButton(user);
                
                if (userButton != null)
                    userButton.Tag = user;

                if (state == UserState.Disconnected)
                {
                    RemoveUser(user);
                }
                else if (state == UserState.Connected)
                {
                    if (userButton == null)
                        AddUser(user);
                    else
                        SetUserNotAway(user);
                }
                else if (state == UserState.Away)
                {
                    if (userButton == null)
                        AddUser(user);

                    SetUserAway(user);
                }
            }

            SetUserIcons();
        }

        bool IsUserIgnored(int userId)
        {
            return this.ignoredUsers.Contains(userId);
        }
        #endregion

        #region Rooms
        EeeDataSet.RoomDataTable rooms = null;

        List<int> ignoredRooms = new List<int>();

        void SynchronizeRooms(EeeDataSet.RoomDataTable rooms)
        {
            if (this.rooms != null)
                this.rooms.Dispose();

            this.rooms = rooms;

            int index = 0;

            /// Adding only supported, for now.
            foreach (EeeDataSet.RoomRow room in rooms)
            {
                ToolStripButton item = new ToolStripButton(room.Name);
                if (index < 10)
                    item.ImageIndex = index;
                index++;
                this.roomsToolStrip.Items.Add(item);
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Tag = room;
                item.ImageAlign = ContentAlignment.MiddleLeft;
                item.TextAlign = ContentAlignment.MiddleLeft;
                item.CheckOnClick = true;
                item.CheckedChanged += new EventHandler(roomButton_CheckedChanged);
                item.MouseUp += new MouseEventHandler(roomButton_MouseUp);
            }
        }

        void roomButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolStripButton button = sender as ToolStripButton;
                Point newPoint = new Point(e.Location.X + button.Bounds.X, e.Location.Y + button.Bounds.Y);

                bool currentlyIgnored = button.ForeColor == Color.Gray;

                this.ignoreRoomItem.Visible = !currentlyIgnored;
                this.stopIgnoringRoomItem.Visible = currentlyIgnored;

                this.stopIgnoringAllRoomsItem.Visible = this.ignoredRooms.Count > 0;

                this.roomMenu.Tag = button;
                this.roomMenu.Show(this.roomsToolStrip.PointToScreen(newPoint));
            }
        }

        void roomButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.freezeEvents) return;
            this.freezeEvents = true;

            ToolStripButton button = sender as ToolStripButton;
            if (button.Checked)
            {
                foreach (ToolStripItem roomItem in this.roomsToolStrip.Items)
                {
                    if (roomItem is ToolStripButton)
                    {
                        ToolStripButton roomButton = roomItem as ToolStripButton;

                        if (roomButton != button)
                            roomButton.Checked = false;
                    }
                }
            }

            RearrangeMessages();

            this.freezeEvents = false;
        }

        HtmlElement contentElement;

        HtmlElement ContentElement
        {
            get
            {
                WaitForBrowser();
                return this.contentElement;
            }
        }

        HtmlElement historyElement;

        HtmlElement HistoryElement
        {
            get
            {
                WaitForBrowser();
                return this.historyElement;
            }
        }

        List<string> contentLinks = new List<string>();

        public List<string> ContentLinks
        {
            get
            {
                return this.contentLinks;
            }
        }

        int SelectedRoomId
        {
            get
            {
                foreach (ToolStripItem item in this.roomsToolStrip.Items)
                {
                    if (item is ToolStripButton && item.Tag != null)
                    {
                        ToolStripButton button = item as ToolStripButton;
                        if (button.Checked)
                            return ((EeeDataSet.RoomRow)button.Tag).RoomID;
                    }
                }

                return 0;
            }
        }

        int SelectedUserId
        {
            get
            {
                foreach (ToolStripItem item in this.usersToolStrip.Items)
                {
                    if (item is ToolStripButton && item.Tag != null)
                    {
                        ToolStripButton button = (ToolStripButton)item;
                        if (button.Checked && ((EeeDataSet.UserRow)button.Tag).UserID != this.eeeServiceController.CurrentUser.UserID)
                            return ((EeeDataSet.UserRow)button.Tag).UserID;
                    }
                }

                return 0;
            }
        }

        string SelectedUserLogin
        {
            get
            {
                foreach (ToolStripItem item in this.usersToolStrip.Items)
                {
                    if (item is ToolStripButton && item.Tag != null)
                    {
                        ToolStripButton button = (ToolStripButton)item;
                        if (button.Checked && ((EeeDataSet.UserRow)button.Tag).UserID != this.eeeServiceController.CurrentUser.UserID)
                            return ((EeeDataSet.UserRow)button.Tag).Login;
                    }
                }

                return null;
            }
        }

        bool MeSelected
        {
            get
            {
                foreach (ToolStripItem item in this.usersToolStrip.Items)
                {
                    if (item is ToolStripButton && item.Tag != null)
                    {
                        ToolStripButton button = (ToolStripButton)item;
                        if (((EeeDataSet.UserRow)button.Tag).UserID == this.eeeServiceController.CurrentUser.UserID)
                            return button.Checked;
                    }
                }

                return false;
            }
        }

        bool IsRoomIgnored(int roomId)
        {
            return this.ignoredRooms.Contains(roomId);
        }
        #endregion
        #endregion

        #region Properties
        bool ShowNotifications
        {
            get
            {
                return this.notificationsMenuItem.Checked;
            }
            set
            {
                bool freezeEventsBackup = this.freezeEvents;
                this.freezeEvents = true;

                Properties.Settings.Default.ShowNotifications = value;
                Properties.Settings.Default.Save();

                this.notificationsMenuItem.Checked = value;
                this.notificationsNotifyItem.Checked = value;

                this.freezeEvents = freezeEventsBackup;
            }
        }

        FormClosingMode ClosingMode
        {
            get
            {
                return this.closingMode;
            }
            set
            {
                this.closingMode = value;
            }
        }

        bool LayoutLoaded
        {
            get
            {
                return this.layoutLoaded;
            }
        }
        #endregion

        #region Events
        private bool freezeEvents = false;

        #region NotifyIcon & Activation
        private bool lastActive;
        private bool beforeLastActive;
        private bool active = false;
        private TimeSpan activityTime = new TimeSpan(0);
        private DateTime activationTime = DateTime.Now;
        private bool doActivate = false;

        FormWindowState lastState = FormWindowState.Normal;

        private void ActivateMe()
        {
            this.Visible = true;
            this.WindowState = this.lastState;
            this.Activate();

            if (this.eeeServiceController != null && this.eeeServiceController.Service != null)
                this.statusLabel.Text = string.Format("Rq: {0} Bytes: {1}", this.eeeServiceController.Service.RequestsMade, this.eeeServiceController.Service.BytesReceived);
        }

        private void MinimizeMe()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void activateHotKey_HotkeyPressed(object sender, System.EventArgs e)
        {
            this.doActivate = true;
        }

        private void BecomeActive()
        {
            this.active = true;
            this.activationTime = DateTime.Now;
        }

        private void BecomeInactive()
        {
            this.active = false;
            this.activityTime += DateTime.Now - this.activationTime;
        }

        private void activationCheckerTimer_Tick(object sender, System.EventArgs e)
        {
            this.beforeLastActive = this.lastActive;
            this.lastActive = this.active;

            if (this.doActivate)
            {
                this.doActivate = false;
                ActivateMe();
            }
        }

        private void notifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.beforeLastActive)
                {
                    MinimizeMe();
                }
                else
                {
                    ActivateMe();
                }
            }
        }
        #endregion

        #region Menu
        private void notificationsNotifyItem_CheckedChanged(object sender, EventArgs e)
        {
            ShowNotifications = this.notificationsNotifyItem.Checked;
        }

        private void notificationsMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ShowNotifications = this.notificationsMenuItem.Checked;
        }

        private void awayModeItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.freezeEvents) return;

            this.freezeEvents = true;

            bool away;

            if (sender is ToolStripMenuItem)
                away = (sender as ToolStripMenuItem).Checked;
            else if (sender is ToolStripButton)
                away = (sender as ToolStripButton).Checked;
            else
                throw new NotImplementedException();

            this.awayModeToolItem.Checked = false;
            this.awayModeMenuItem.Checked = false;

            this.autoAwayMonitor.Enabled = !away;

            if (away)
            {
                using (AwayModeReasonDialog dialog = new AwayModeReasonDialog())
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        SetAwayMode(true, dialog.Reason);
                    else
                        away = false;
                }
            }
            else
            {
                SetAwayMode(false, "");
            }

            this.freezeEvents = false;
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            About();
        }

        private void exitNotifyItem_Click(object sender, EventArgs e)
        {
            Exit(FormClosingMode.Exit);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Exit(FormClosingMode.Exit);
        }

        private void helpNotifyItem_Click(object sender, EventArgs e)
        {
            Help();
        }
        #endregion
        #endregion

        bool resizing = false;

        void roomsToolStrip_Resize(object sender, EventArgs e)
        {
            if (this.LayoutLoaded == false || this.resizing)
                return;

            this.resizing = true;

            if (this.usersToolStrip.Parent == this.roomsToolStrip.Parent && this.usersToolStrip.Orientation == Orientation.Vertical && this.roomsToolStrip.Orientation == Orientation.Vertical)
                AdjustToolbarWidths();

            this.resizing = false;
        }

        void usersToolStrip_Resize(object sender, EventArgs e)
        {
            if (this.LayoutLoaded == false || this.resizing)
                return;

            this.resizing = true;

            if (this.usersToolStrip.Parent == this.roomsToolStrip.Parent && this.usersToolStrip.Orientation == Orientation.Vertical && this.roomsToolStrip.Orientation == Orientation.Vertical)
                AdjustToolbarWidths();

            this.resizing = false;
        }

        Size zeroSize = new Size(0, 0);

        void AdjustToolbarWidths()
        {
            Size usersToolStripSize = this.usersToolStrip.GetPreferredSize(this.zeroSize);
            Size roomsToolStripSize = this.roomsToolStrip.GetPreferredSize(this.zeroSize);

            int maxWidth = usersToolStripSize.Width;
            if (roomsToolStripSize.Width > maxWidth)
                maxWidth = roomsToolStripSize.Width;

            this.usersToolStrip.Width = maxWidth;
            this.roomsToolStrip.Width = maxWidth;
        }

        bool AwayMode
        {
            get
            {
                if (this.eeeServiceController.CurrentUser == null)
                    return false;
                else
                    return (UserState)this.eeeServiceController.CurrentUser.State == UserState.Away;
            }
        }

        int updateCheckCounter = 3;

        void messageCheckerTimer_Tick(object sender, EventArgs e)
        {
            if (this.loginManager.IsConnected && this.AwayMode == false)
            {
                this.eeeServiceController.GetMessages();

                this.updateCheckCounter--;
                if (this.updateCheckCounter == 0)
                {
                    this.eeeServiceController.GetUpdates();
                    this.updateCheckCounter = 10;
                }
            }

            if (this.loginManager.IsConnected)
            {
                this.eeeServiceController.RenewJabber();
            }
        }

        void eeeServiceController_GetMessagesFinished(object sender, BackgroundServiceController.GetMessagesFinishedEventArgs e)
        {
            this.historyManager.Save(e.Messages);

            if (this.firstGet)
                AddMessages((EeeDataSet.MessageDataTable)this.historyManager.Current, false);
            else
                AddMessages(e.Messages, false);

            this.firstGet = false;
        }

        void AddMessages(EeeDataSet.MessageDataTable messages, bool history)
        {
            if (messages.Count > 0)
            {
                bool willScroll = !history && CanScroll();

                EeeDataSet.RoomRow mainRoom = this.rooms[0];

                /// Add notifications & set attributes.
                foreach (EeeDataSet.MessageRow message in messages)
                {
                    if (message.RoomID == 0)
                        message.RoomID = mainRoom.RoomID;

                    if (message.ToUserID != 0)
                        message.RoomID = mainRoom.RoomID;

                    if (message.ExternalFrom != "")
                        message.ExternalFrom = JidToName(message.ExternalFrom);

                    EeeDataSet.RoomRow roomRow = this.rooms.FindByRoomID(message.RoomID);

                    if (roomRow != null && roomRow != mainRoom)
                    {
                        message.Room = roomRow.Name;
                    }

                    SetMessageVisibility(message, history);

                    /// The notification is added when the message is not from me, and the room is not ignored, and the user is not ignored.
                    /// If the NotifyAboutIgnoredPersonalMessages is set, also show the notification if the room or user is ignored, but it is for me personally.
                    bool fromMe = message.FromUserID == this.eeeServiceController.CurrentUser.UserID;
                    bool roomIgnored = IsRoomIgnored(message.RoomID);
                    bool userIgnored = IsUserIgnored(message.FromUserID);
                    bool ignored = roomIgnored || userIgnored;

                    bool forMe = message.IsToUserIDNull() == false && message.ToUserID == this.eeeServiceController.CurrentUser.UserID;
                    bool showForMe = forMe && Properties.Settings.Default.NotifyAboutIgnoredPersonalMessages;

                    if (history == false && fromMe == false && (ignored == false || showForMe))
                    {
                        MessageType messageType = (message.ToUserID != 0) ? MessageType.Private : MessageType.Public;
                        this.notificationManager.AddNotification(message.Login, message.Color, message.Message, messageType);

                        AddReplyUser(message.Login, forMe, message.RoomID); /// Message privately sent to me.
                    }
                }

                List<string> links = new List<string>();

                XmlDocument transformedXml = MessageTextProcessor.Instance.ProcessMessages(messages, links);

                if (history == this.InHistory)
                    this.mediaPlayer.AddLinks(links);

                if (!history)
                {
                    foreach (string link in links)
                    {
                        if (!this.contentLinks.Contains(link))
                            this.contentLinks.Add(link);
                    }
                }

                foreach (XmlNode messageNode in transformedXml.SelectNodes("/root/message"))
                {
                    string messageIdStr = messageNode.Attributes["messageId"].Value;

                    if (messageIdStr == null)
                        throw new ApplicationException("Wrong template: messageId not specified.");

                    int messageId;

                    if (!int.TryParse(messageIdStr, out messageId))
                        throw new ApplicationException("Wrong template: messageId format no good.");

                    EeeDataSet.MessageRow messageRow = messages.FindByMessageID(messageId);

                    if (messageRow == null)
                        throw new ApplicationException("Wrong template: message with specified id not found.");

                    HtmlElement messageElement = this.chatBrowser.Document.CreateElement("div");

                    messageElement.SetAttribute("messageId", messageRow.MessageID.ToString());
                    messageElement.SetAttribute("roomId", messageRow.RoomID.ToString());
                    messageElement.SetAttribute("userId", messageRow.FromUserID.ToString());

                    if (messageRow.InitiallyVisible == false)
                        messageElement.Style = "display: none;";

                    messageElement.InnerHtml = messageNode.InnerXml;

                    HtmlElement element;

                    if (history)
                        element = this.HistoryElement;
                    else
                        element = this.ContentElement;

                    element.AppendChild(messageElement);
                }

                if (willScroll)
                    ScrollDown();

                UpdateInvisibleMessageIndicators();
            }
        }

        string JidToName(string externalFrom)
        {
            if (this.jidToNameMap.ContainsKey(externalFrom))
                return this.jidToNameMap[externalFrom];
            else
                return externalFrom;
        }

        struct ReplyUser
        {
            public string User;
            public bool Private;
            public int RoomId;
        }

        void AddReplyUser(string user, bool isPrivate, int roomId)
        {
            if (this.replyUserHistory.Count > 10)
                this.replyUserHistory.RemoveAt(10);

            ReplyUser replyUser = new ReplyUser();
            replyUser.User = user;
            replyUser.Private = isPrivate;
            replyUser.RoomId = roomId;

            this.replyUserHistory.Insert(0, replyUser);
            this.replyUserIndex = 0;
        }

        void UpdateInvisibleMessageIndicators()
        {
            Dictionary<int, int> counter;

            counter = new Dictionary<int, int>();

            foreach (InvisibleMessageProperties property in this.invisibleUserMessages.Values)
            {
                int count = 0;
                if (counter.ContainsKey(property.UserId))
                    count = counter[property.UserId];
                counter[property.UserId] = count + 1;
            }

            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                if (item.Tag != null)
                {
                    EeeDataSet.UserRow row = (EeeDataSet.UserRow)item.Tag;
                    if (counter.ContainsKey(row.UserID))
                    {
                        item.Text = row.Login + " (" + counter[row.UserID] + ")";
                        SetButtonBoldness(item, true);
                    }
                    else
                    {
                        item.Text = row.Login;
                        SetButtonBoldness(item, false);
                    }
                }
            }

            counter = new Dictionary<int, int>();

            foreach (InvisibleMessageProperties property in this.invisibleRoomMessages.Values)
            {
                int count = 0;
                if (counter.ContainsKey(property.RoomId))
                    count = counter[property.RoomId];
                counter[property.RoomId] = count + 1;
            }

            foreach (ToolStripItem item in this.roomsToolStrip.Items)
            {
                if (item.Tag != null)
                {
                    EeeDataSet.RoomRow row = (EeeDataSet.RoomRow)item.Tag;
                    if (counter.ContainsKey(row.RoomID))
                    {
                        item.Text = row.Name + " (" + counter[row.RoomID] + ")";
                        SetButtonBoldness(item, true);
                    }
                    else
                    {
                        item.Text = row.Name;
                        SetButtonBoldness(item, false);
                    }
                }
            }
        }

        Font normalFont;
        Font boldFont;

        void SetButtonBoldness(ToolStripItem item, bool bold)
        {
            if (item.Font.Bold != bold)
            {
                if (bold)
                    item.Font = this.boldFont;
                else
                    item.Font = this.normalFont;
            }
        }

        void eeeServiceController_UserStateChanged(object sender, CommandProcessor.UserStateChangedEventArgs e)
        {
            //TODO: subject to change
            //this.eeeServiceController.GetUsers();
            EeeDataSet.UserDataTable userDataTable = new EeeDataSet.UserDataTable();
            userDataTable.AddUserRow(e.UserID, e.UserLogin, (int)e.UserState, 0, null, e.Comment, DateTime.Now, e.Client);
            SynchronizeUsers(userDataTable);
        }

        void text_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            bool control;

            if (Properties.Settings.Default.EnterSends)
                control = !e.Control && !e.Shift;
            else
                control = e.Control;

            if ((control && e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.S))
            {
                if (!this.InHistory)
                {
                    string textToSend = this.text.Text.Trim();
                    this.text.Text = "";

                    if (textToSend != "")
                    {
                        string externalNick = null;
                        string externalUser = ExtenalUser(ref externalNick);

                        if (externalUser == null)
                        {
                            string recipient = GetRecipient(ref textToSend);

                            if (textToSend != null && textToSend.Length > 0)
                            {
                                if (this.loginManager.IsConnected)
                                {
                                    this.eeeServiceController.AddMessage(this.SelectedRoomId, recipient, textToSend);
                                    //this.eeeServiceController.GetMessages(new TimeSpan(0, 0, 2));
                                }
                            }
                        }
                        else
                        {
                            this.eeeServiceController.SendJabber(externalUser, textToSend, externalNick);
                        }
                    }
                }
            }
            else if (e.Control && e.KeyCode == Keys.R)
            {
                Reply();
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                Follow();
            }
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)
            {
                int index = (int)e.KeyCode - (int)Keys.F1;

                switch (e.Modifiers)
                {
                    case Keys.None:
                        AddUserToText(index);
                        break;
                    case Keys.Control:
                        SelectRoomNo(index);
                        break;
                    case Keys.Alt:
                        SelectUserNo(index);
                        break;
                }
            }
            else
            {
                e.SuppressKeyPress = false;
                e.Handled = false;
            }

            if (e.Control == false && e.Alt == false && (e.KeyCode & Keys.KeyCode) != Keys.None)
                this.replyUserIndex = 0;
        }

        string ExtenalUser(ref string externalNick)
        {
            foreach (ToolStripItem item in this.externalUsersToolStrip.Items)
            {
                if (((ToolStripButton)item).Checked)
                {
                    externalNick = item.Text;
                    return (string)item.Tag;
                }
            }

            return null;
        }

        void AddUserToText(int index)
        {
            if (this.usersToolStrip.Items.Count > index)
            {
                EeeDataSet.UserRow user = (EeeDataSet.UserRow)this.usersToolStrip.Items[index].Tag;

                string text = this.text.Text;

                bool hasSlash = text.StartsWith("/");

                if (hasSlash)
                    text = text.Substring(1);

                text = this.replaceReplyUser.Replace(text, user.Login + ": ${text}");

                if (hasSlash)
                    text = "/" + text;

                this.text.Text = text;
                this.text.SelectionStart = text.Length;
            }
        }

        void SelectUserNo(int index)
        {
            if (this.usersToolStrip.Items.Count > index)
            {
                ToolStripButton button = this.usersToolStrip.Items[index] as ToolStripButton;
                button.Checked = !button.Checked;
            }
        }

        void SelectRoomNo(int index)
        {
            if (this.roomsToolStrip.Items.Count > index)
            {
                ToolStripButton button = this.roomsToolStrip.Items[index] as ToolStripButton;
                button.Checked = !button.Checked;
            }
        }

        List<ReplyUser> replyUserHistory = new List<ReplyUser>();
        int replyUserIndex = 0;

        Regex replaceReplyUser = new Regex(@"^([/]?\w+[:]\s*)?(?<text>.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        void RotateReplyUser(bool skipSameUsers)
        {
            if (this.replyUserHistory.Count > 0)
            {
                string currentUser = this.replyUserHistory[this.replyUserIndex].User;

                this.replyUserIndex++;

                if (this.replyUserHistory.Count <= this.replyUserIndex)
                    this.replyUserIndex = 0;

                if (skipSameUsers)
                {
                    int wall = this.replyUserIndex;

                    while (this.replyUserHistory[this.replyUserIndex].User == currentUser)
                    {
                        this.replyUserIndex++;

                        if (this.replyUserHistory.Count <= this.replyUserIndex)
                            this.replyUserIndex = 0;

                        if (this.replyUserIndex == wall)
                            break;
                    }
                }
            }
        }

        void Reply()
        {
            if (this.replyUserHistory.Count > this.replyUserIndex)
            {
                ReplyUser replyUser = this.replyUserHistory[this.replyUserIndex];

                if (replyUser.Private)
                    this.text.Text = this.replaceReplyUser.Replace(this.text.Text, "/" + this.replyUserHistory[this.replyUserIndex].User + ": ${text}");
                else
                    this.text.Text = this.replaceReplyUser.Replace(this.text.Text, this.replyUserHistory[this.replyUserIndex].User + ": ${text}");

                this.text.SelectionStart = this.text.Text.Length;

                RotateReplyUser(true);
            }
        }

        void Follow()
        {
            if (this.replyUserHistory.Count > this.replyUserIndex)
            {
                ReplyUser replyUser = this.replyUserHistory[this.replyUserIndex];

                if (replyUser.Private)
                    this.text.Text = this.replaceReplyUser.Replace(this.text.Text, "/" + this.replyUserHistory[this.replyUserIndex].User + ": ${text}");
                else
                    this.text.Text = this.replaceReplyUser.Replace(this.text.Text, this.replyUserHistory[this.replyUserIndex].User + ": ${text}");

                this.text.SelectionStart = this.text.Text.Length;

                if (replyUser.RoomId == 0 || replyUser.RoomId == 1)
                {
                    if (this.SelectedRoomId != 0)
                        SelectRoomNo(this.SelectedRoomId - 1);
                }
                else
                {
                    if (this.SelectedRoomId != replyUser.RoomId)
                        SelectRoomNo(replyUser.RoomId - 1);
                }

                RotateReplyUser(false);
            }
        }

        string GetRecipient(ref string messageToSend)
        {
            if (messageToSend == null || messageToSend.Length == 0)
                return null;

            if (messageToSend == "/resetlayout")
            {
                Properties.Settings.Default.ResetLayout = true;
                MessageBox.Show(this, "Please restart the application to apply.", "Reset Layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                messageToSend = null;
                return null;
            }
            else if (messageToSend[0] == '/' && messageToSend.IndexOf(":") > 0)
            {
                int idx = messageToSend.IndexOf(":");
                string recipient = messageToSend.Substring(1, idx - 1);
                messageToSend = messageToSend.Substring(1);

                AddReplyUser(recipient, true, this.SelectedRoomId);

                return recipient;
            }
            else if (this.SelectedUserId != 0)
            {
                if (messageToSend.StartsWith(this.SelectedUserLogin + ":") == false)
                    messageToSend = this.SelectedUserLogin + ": " + messageToSend;

                AddReplyUser(this.SelectedUserLogin, true, this.SelectedRoomId);

                return this.SelectedUserLogin;
            }
            else
            {
                if (MessageVisibilityHelper.Instance.IsAddressedMessage(messageToSend))
                    AddReplyUser(messageToSend.Substring(0, messageToSend.IndexOf(':')), false, this.SelectedRoomId);

                return null;
            }
        }

        string GetRecipient(string messageToSend)
        {
            if (messageToSend == null || messageToSend.Length == 0)
                return null;

            if (messageToSend[0] == '/' && messageToSend.IndexOf(":") > 0)
            {
                int idx = messageToSend.IndexOf(":");
                string recipient = messageToSend.Substring(1, idx - 1);
                return recipient;
            }
            else if (this.SelectedUserId != 0)
            {
                return this.SelectedUserLogin;
            }
            else
            {
                return null;
            }
        }

        void eeeServiceController_ErrorOccured(object sender, BackgroundServiceController.ErrorOccuredEventArgs e)
        {
            ShowError(e.Error);
        }

        bool CanScroll()
        {
            if (this.InHistory || this.chatBrowser.Document == null)
                return false;

            int scrollPosition = this.chatBrowser.Document.Body.ScrollTop;
            int scrollMax = this.chatBrowser.Document.Body.ScrollRectangle.Height - this.ContentElement.OffsetRectangle.Y - this.chatBrowser.Document.Body.ClientRectangle.Height;
            return scrollMax - scrollPosition < 20;
        }

        void ScrollDown()
        {
            HtmlElement scrollElement = this.ContentElement;

            if (scrollElement != null)
                scrollElement.ScrollIntoView(false);
        }

        void SetMessagesVisibility()
        {
            HtmlElement element;

            if (this.InHistory)
                element = this.HistoryElement;
            else
                element = this.ContentElement;

            if (element != null)
            {
                foreach (HtmlElement divElement in element.Children)
                {
                    SetMessageVisibility(divElement, this.SelectedRoomId, this.SelectedUserId, this.SelectedUserLogin, this.MeSelected);
                }
            }
        }

        void SetMessageVisibility(HtmlElement divElement, int selectedRoomId, int selectedUserId, string selectedUserLogin, bool meSelected)
        {
            string messageIdInString = divElement.GetAttribute("messageId");

            if (messageIdInString != null && messageIdInString.Length > 0)
            {
                string userId = divElement.GetAttribute("userId");
                string roomId = divElement.GetAttribute("roomId");

                bool showUser = MessageVisibilityHelper.Instance.UserVisible(userId, divElement.InnerText, selectedUserId, selectedUserLogin, meSelected, this.ignoredUsers, this.eeeServiceController.CurrentUser.UserID, this.eeeServiceController.CurrentUser.Login);
                bool showRoom = MessageVisibilityHelper.Instance.RoomVisible(roomId, selectedRoomId, this.ignoredRooms);

                if (showUser && showRoom)
                {
                    divElement.Style = "";
                    if (!this.InHistory)
                        RemoveInvisibleMessage(int.Parse(messageIdInString));
                }
                else
                {
                    divElement.Style = "display: none;";
                }
            }
        }

        void SetMessageVisibility(EeeDataSet.MessageRow message, bool historicalMessage)
        {
            string userId = message.FromUserID.ToString();
            string roomId = message.RoomID.ToString();

            //TODO: Remove when new protocol is in practice.

            /// Set the color to user so that reply operation can use it.
            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                if (item.Tag != null)
                {
                    if (((EeeDataSet.UserRow)item.Tag).UserID.ToString() == userId)
                    {
                        ((EeeDataSet.UserRow)item.Tag).Color = message.Color;
                        break;
                    }
                }
            }

            bool inHistory = this.InHistory;
            bool showUser = MessageVisibilityHelper.Instance.UserVisible(userId, message.Message, this.SelectedUserId, this.SelectedUserLogin, this.MeSelected, this.ignoredUsers, this.eeeServiceController.CurrentUser.UserID, this.eeeServiceController.CurrentUser.Login);
            bool showRoom = MessageVisibilityHelper.Instance.RoomVisible(roomId, this.SelectedRoomId, this.ignoredRooms);

            if (!historicalMessage)
            {
                /// Test if we need to add this to invisible user messages counter.
                if (showUser == false || inHistory)
                {
                    int fromUserId = message.FromUserID;
                    string myLogin = this.eeeServiceController.CurrentUser.Login;
                    int myId = this.eeeServiceController.CurrentUser.UserID;

                    /// If the message is not from me.
                    if (fromUserId != myId)
                    {
                        /// Add the invisible message to "from user" list.
                        AddInvisibleUserMessage(message.MessageID, fromUserId);

                        /// If the message is for me, add the invisible message to my list.
                        if (message.ToUserID == myId || message.Message.ToLower().StartsWith(myLogin + ":"))
                            AddInvisibleUserMessage(message.MessageID, myId);
                    }
                }

                if (showRoom == false || inHistory)
                    AddInvisibleRoomMessage(message.MessageID, message.RoomID);
            }

            message.InitiallyVisible = showUser && showRoom;
        }

        void removeRoomButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedRoomId == 0)
                return;

            this.roomsToolStrip.Items[this.roomsToolStrip.Items.Count - 1].Visible = false;
        }

        void activatingHotkey_Pressed(object sender, GlobalHotKey.PressedEventArgs e)
        {
            ActivateMe();
        }

        void activatingHotkey_Error(object sender, GlobalHotKey.ErrorEventArgs e)
        {
            ShowError(new ApplicationException("Cannot register global hotkey.", e.Error));
        }

        void errorLabel_Click(object sender, EventArgs e)
        {
            ErrorForm.ShowError((Exception)this.errorLabel.Tag, this.eeeServiceController);
            this.errorLabel.Visible = false;
        }

        void ShowError(Exception error)
        {
            if (IsConnectionProblem(error))
            {
                this.connectionProblemsLabel.Tag = error;
                this.connectionProblemsLabel.Visible = true;
            }
            else if (IsDisconnectedException(error))
            {
                this.disconnectedLabel.Visible = true;
            }
            else
            {
                this.errorLabel.Tag = error;
                this.errorLabel.Visible = true;
            }
        }

        bool IsConnectionProblem(Exception error)
        {
            while (error != null)
            {
                if (error is System.Net.WebException || error is System.Net.Sockets.SocketException)
                    return true;
                error = error.InnerException;
            }

            return false;
        }

        bool IsDisconnectedException(Exception error)
        {
            while (error != null)
            {
                if (error is DisconnectedException)
                    return true;
                error = error.InnerException;
            }

            return false;
        }

        void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (OptionsForm optionsForm = new OptionsForm())
            {
                optionsForm.ShowDialog();
                RefreshUnbindableProperties(optionsForm.TemplateChanged);
            }
        }

        void RefreshUnbindableProperties(bool templateChanged)
        {
            this.eeeServiceController.Service.ServiceUrl = Properties.Settings.Default.ServiceUrl;

            this.ShowNotifications = Properties.Settings.Default.ShowNotifications;
            this.ShowInTaskbar = Properties.Settings.Default.ShowInTaskbar;
            if (Properties.Settings.Default.UseProfessionalAppearance)
            {
                if (ToolStripManager.RenderMode != ToolStripManagerRenderMode.Professional)
                    ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
            }
            else
            {
                if (ToolStripManager.RenderMode != ToolStripManagerRenderMode.System)
                    ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
            }
            if (templateChanged)
                ChangeTemplate();
        }

        void ChangeTemplate()
        {
            ClearBrowserContent(this.chatBrowser);
            MessageTextProcessor.Instance.LoadTemplate();
            AddMessages((EeeDataSet.MessageDataTable)this.historyManager.Current, false);
        }

        void notificationManager_Activate(object sender, NotificationManager.ActivateEventArgs e)
        {
            ActivateMe();
        }

        void text_TextChanged(object sender, EventArgs e)
        {
            /*Point where = this.text.GetPositionFromCharIndex(this.text.Text.Length - 1);
            where.X = where.X + 20;
            this.label1.Location = this.PointToClient(this.text.PointToScreen(where));*/

            string userToSend = GetRecipient(this.text.Text);
            string currentUserToSend = this.text.Tag as string;

            if (userToSend != null)
                userToSend = userToSend.ToLower();

            if (userToSend != currentUserToSend)
            {
                this.text.Tag = userToSend;

                foreach (ToolStripItem item in this.usersToolStrip.Items)
                {
                    if (item.Tag != null)
                    {
                        EeeDataSet.UserRow userRow = (EeeDataSet.UserRow)item.Tag;
                        if (userRow.Login.ToLower() == userToSend)
                        {
                            Color color = Color.FromArgb(userRow.Color | (255 << 24));
                            this.text.ForeColor = color;
                            return;
                        }
                    }
                }

                this.text.ForeColor = Color.Black;
            }
        }

        void connectItem_Click(object sender, EventArgs e)
        {
            this.loginManager.Login(this);
        }

        void helpItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://eee.php5.sk");
        }

        void accountItem_Click(object sender, EventArgs e)
        {
            using (AccountForm accountForm = new AccountForm(this.eeeServiceController))
            {
                accountForm.ShowDialog();
            }
        }

        void loginManager_InvalidPassword(object sender, KolikSoftware.Eee.Client.LoginProcess.LoginManager.InvalidPasswordEventArgs e)
        {
            MessageBox.Show(this, "The user name or password is invalid.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void loginManager_Connected(object sender, KolikSoftware.Eee.Client.LoginProcess.LoginManager.ConnectedEventArgs e)
        {
            this.Text = Application.ProductName + " - " + this.eeeServiceController.CurrentUser.Login;
            this.statusLabel.Text = this.Text;
            this.notifyIcon.Text = this.Text;

            ClearBrowserContent(this.chatBrowser);

            this.firstGet = true;

            this.eeeServiceController.GetMessages();
            this.messageCheckerTimer.Enabled = true;
            this.eeeServiceController.ConnectJabber();//TODO:
        }

        void loginManager_Disconnected(object sender, KolikSoftware.Eee.Client.LoginProcess.LoginManager.DisconnectedEventArgs e)
        {
            this.messageCheckerTimer.Enabled = false;

            ClearBrowserContent(this.chatBrowser);

            GoToDay(DateTime.Today);

            this.usersToolStrip.Items.Clear();
            this.roomsToolStrip.Items.Clear();

            this.Text = Application.ProductName + " - (disconnected)";
            this.statusLabel.Text = this.Text;
            this.notifyIcon.Text = this.Text;

            if (this.ClosingMode == FormClosingMode.Exit)
                Close();
            else if (this.ClosingMode == FormClosingMode.Shutdown)
                FinishShutdown();
        }

        void FinishShutdown()
        {
            this.canShutdown = true;
        }

        void RearrangeMessages()
        {
            SetMessagesVisibility();
            ScrollDown();
            UpdateInvisibleMessageIndicators();
        }

        private void eeeServiceController_GetRoomsFinished(object sender, BackgroundServiceController.GetRoomsFinishedEventArgs e)
        {
            SynchronizeRooms(e.Rooms);
        }

        private void eeeServiceController_GetUsersFinished(object sender, BackgroundServiceController.GetUsersFinishedEventArgs e)
        {
            SynchronizeUsers(e.Users);
        }

        private void loginManager_BeforeLogin(object sender, KolikSoftware.Eee.Client.LoginProcess.LoginManager.BeforeLoginEventArgs e)
        {
            this.connectMenuItem.Enabled = false;
        }

        private void loginManager_AfterLogin(object sender, KolikSoftware.Eee.Client.LoginProcess.LoginManager.AfterLoginEventArgs e)
        {
            this.connectMenuItem.Enabled = true;
        }

        private void connectionProblemsLabel_Click(object sender, EventArgs e)
        {
            ErrorForm.ShowConnectionProblem((Exception)this.connectionProblemsLabel.Tag, this.eeeServiceController);
        }

        private void eeeServiceController_SucessfulRequest(object sender, BackgroundServiceController.SucessfulRequestEventArgs e)
        {
            this.connectionProblemsLabel.Visible = false;
            this.disconnectedLabel.Visible = false;
        }

        int GetID(ToolStripItem item)
        {
            /// This may be a context menu item. In that case, the owner's menu is tagged with reference to item with which the context menu is associated.
            if (item.Owner.Tag != null)
                item = item.Owner.Tag as ToolStripItem;

            if (item.Tag is EeeDataSet.RoomRow)
                return ((EeeDataSet.RoomRow)item.Tag).RoomID;
            else if (item.Tag is EeeDataSet.UserRow)
                return ((EeeDataSet.UserRow)item.Tag).UserID;
            else
                return 0;
        }

        int GetID(object obj)
        {
            return GetID(obj as ToolStripItem);
        }

        ToolStripItem GetItem(ToolStripItem item)
        {
            if (item.Owner.Tag == null)
                return item;
            else
                return (ToolStripItem)item.Owner.Tag;
        }

        ToolStripItem GetItem(object obj)
        {
            return GetItem(obj as ToolStripItem);
        }

        EeeDataSet.UserRow GetUser(object obj)
        {
            return GetItem(obj).Tag as EeeDataSet.UserRow;
        }

        EeeDataSet.RoomRow GetRoom(object obj)
        {
            return GetItem(obj).Tag as EeeDataSet.RoomRow;
        }

        void IgnoreRoom(object item)
        {
            ToolStripItem roomItem = GetItem(item);
            int roomId = GetID(roomItem);

            if (this.ignoredRooms.Contains(roomId) == false)
                this.ignoredRooms.Add(roomId);

            SetRoomColor(roomItem);
        }

        void StopIgnoringRoom(object item)
        {
            ToolStripItem roomItem = GetItem(item);
            int roomId = GetID(roomItem);

            if (this.ignoredRooms.Contains(roomId))
                this.ignoredRooms.Remove(roomId);

            SetRoomColor(roomItem);
        }

        void IgnoreUser(object item)
        {
            ToolStripItem userItem = GetItem(item);
            int userId = GetID(userItem);

            if (this.ignoredUsers.Contains(userId) == false)
                this.ignoredUsers.Add(userId);

            SetUserColor(userItem);
        }

        void StopIgnoringUser(object item)
        {
            ToolStripItem userItem = GetItem(item);
            int userId = GetID(userItem);

            if (this.ignoredUsers.Contains(userId))
                this.ignoredUsers.Remove(userId);

            SetUserColor(userItem);
        }

        private void SetUserColor(ToolStripItem item)
        {
            EeeDataSet.UserRow user = GetUser(item);

            UserState userState = (UserState)user.State;

            ToolStripButton button = item as ToolStripButton;

            if (IsUserIgnored(user.UserID))
                button.ForeColor = Color.Gray;
            else if (userState == UserState.Away)
                button.ForeColor = Color.Red;
            else
                button.ForeColor = Color.Black;
        }

        private void SetRoomColor(ToolStripItem item)
        {
            EeeDataSet.RoomRow room = GetRoom(item);

            ToolStripButton button = item as ToolStripButton;

            if (IsRoomIgnored(room.RoomID))
                button.ForeColor = Color.Gray;
            else
                button.ForeColor = Color.Black;
        }

        private void ignoreRoomItem_Click(object sender, EventArgs e)
        {
            IgnoreRoom(sender);
            RearrangeMessages();
        }

        private void stopIgnoringRoomItem_Click(object sender, EventArgs e)
        {
            StopIgnoringRoom(sender);
            RearrangeMessages();
        }

        private void ignoreUserItem_Click(object sender, EventArgs e)
        {
            IgnoreUser(sender);
            RearrangeMessages();
        }

        private void stopIgnoringUserItem_Click(object sender, EventArgs e)
        {
            StopIgnoringUser(sender);
            RearrangeMessages();
        }

        private void autoAwayMonitor_AutoAway(object sender, AutoAwayMonitor.AutoAwayEventArgs e)
        {
            if (this.loginManager.IsConnected)
                SetAwayMode(e.Away, "Idle");
        }

        private void ignoreAllUsersButThisItem_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = GetItem(sender);

            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                if (item == clickedItem)
                    StopIgnoringUser(item);
                else
                    IgnoreUser(item);
            }
            RearrangeMessages();
        }

        private void ignoreAllRoomsButThisItem_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = GetItem(sender);

            foreach (ToolStripItem item in this.roomsToolStrip.Items)
            {
                if (item == clickedItem)
                    StopIgnoringRoom(item);
                else
                    IgnoreRoom(item);
            }
            RearrangeMessages();
        }

        private void stopIgnoringAllRoomsItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem item in this.roomsToolStrip.Items)
            {
                StopIgnoringRoom(item);
            }
            RearrangeMessages();
        }

        private void stopIgnoringAllUsersItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                StopIgnoringUser(item);
            }
            RearrangeMessages();
        }

        void notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.notificationManager.IsBlinking && Properties.Settings.Default.StopIconOnMouseOver)
                this.notificationManager.StopBlinking();
        }

        void feedbackItem_Click(object sender, EventArgs e)
        {
            using (UserFeedbackForm userFeedbackForm = new UserFeedbackForm(this.eeeServiceController))
            {
                userFeedbackForm.ShowDialog(this);
            }
        }

        void eeeServiceController_UpdatesAvailable(object sender, BackgroundServiceController.UpdatesAvailableEventArgs e)
        {
            if (!this.updateManager.IsBusy)
                CheckUpdateState();
        }

        void updatesAvailableLabel_Click(object sender, EventArgs e)
        {
            if (this.updatesLabel.Tag == null)
            {
                if (!this.updateManager.IsBusy)
                    this.updateManager.Perform();
            }
            else
            {
                if (this.updateManager.State == UpdateManager.UpdatesState.Downloadable)
                    ErrorForm.ShowDownloadProblem((Exception)this.updatesLabel.Tag, this.eeeServiceController);
                else
                    ErrorForm.ShowError((Exception)this.updatesLabel.Tag, this.eeeServiceController);
            }
        }

        struct UploadInfo
        {
            public string Info;
            public int RoomId;
            public string User;
        }

        void uploadToolItem_Click(object sender, EventArgs e)
        {
            UploadMedia();
        }

        void UploadMedia()
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "All Files (*.*)|*.*";
                openDialog.CheckFileExists = true;
                openDialog.CheckPathExists = true;
                openDialog.Multiselect = false;
                openDialog.Title = "Open File";

                if (openDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UploadMedia(openDialog.FileName);
                }
            }
        }

        void UploadMedia(string file)
        {
            string info = UploadInfoBox.Ask();

            if (info != null)
            {
                UploadInfo uploadInfo = new UploadInfo();
                uploadInfo.Info = info;
                uploadInfo.RoomId = this.SelectedRoomId;
                uploadInfo.User = this.SelectedUserLogin;
                if (uploadInfo.User == null && this.MeSelected)
                    uploadInfo.User = this.eeeServiceController.CurrentUser.Login;

                this.eeeServiceController.UploadFile(file, uploadInfo);
                UpdateUploadStatus(null);
            }
        }

        void UpdateUploadStatus(Exception error)
        {
            this.uploadingLabel.Tag = null;
            this.uploadingLabel.IsLink = false;

            int count = this.eeeServiceController.UploadInvocationsCount;

            if (count == 0)
            {
                if (error != null)
                {
                    this.uploadingLabel.Text = "Last upload failed";
                    this.uploadingLabel.Tag = error;
                    this.uploadingLabel.IsLink = true;
                }
                else
                {
                    this.uploadingLabel.Visible = false;
                }
            }
            else
            {
                if (error != null)
                {
                    this.uploadingLabel.Text = string.Format("Uploading ({0} remaining, prev. failed)", count);
                    this.uploadingLabel.Tag = error;
                    this.uploadingLabel.IsLink = false;
                }
                else
                {
                    this.uploadingLabel.Text = string.Format("Uploading ({0} remaining)", count);
                }
                this.uploadingLabel.Visible = true;
            }
        }

        void UpdateDownloadStatus(Exception error)
        {
            this.downloadingLabel.Tag = null;
            this.downloadingLabel.IsLink = false;

            int count = this.eeeServiceController.DownloadInvocationsCount;

            if (count == 0)
            {
                if (error != null)
                {
                    this.downloadingLabel.Text = "Last download failed";
                    this.downloadingLabel.Tag = error;
                    this.downloadingLabel.IsLink = true;
                }
                else
                {
                    this.downloadingLabel.Visible = false;
                }
            }
            else
            {
                if (error != null)
                {
                    this.downloadingLabel.Text = string.Format("Downloading ({0} remaining, prev. failed)", count);
                    this.downloadingLabel.Tag = error;
                    this.downloadingLabel.IsLink = true;
                }
                else
                {
                    this.downloadingLabel.Text = string.Format("Downloading ({0} remaining)", count);
                }
                this.downloadingLabel.Visible = true;
            }
        }

        void eeeServiceController_UploadFinished(object sender, BackgroundServiceController.UploadFinishedEventArgs e)
        {
            UploadInfo uploadInfo = (UploadInfo)e.Parameter;

            this.eeeServiceController.AddMessage(uploadInfo.RoomId, uploadInfo.User, e.Link + "\n" + uploadInfo.Info);
            this.eeeServiceController.GetMessages(new TimeSpan(0, 0, 2));

            UpdateUploadStatus(null);
        }

        void eeeServiceController_UploadFailed(object sender, BackgroundServiceController.UploadFailedEventArgs e)
        {
            this.notificationManager.AddNotification("File Upload", 0, "Upload failed (" + e.Link + ")", MessageType.System);
            UpdateUploadStatus(e.Error);
        }

        void SetCurrentDayLabel()
        {
            if (this.currentDay == this.startingDay)
                this.dateLabel.Text = "Today";
            else
                this.dateLabel.Text = this.currentDay.ToShortDateString();
        }

        const string VisibleMasterStyle = "a:visited {color: blue}; padding-right: 10px; word-wrap:break-word;";
        const string InvisibleMasterStyle = "display:none; a:visited {color: blue}; padding-right: 10px; word-wrap:break-word;";

        void GoToDay(DateTime date)
        {
            this.mediaPlayer.Stop();
            this.mediaPlayer.ClearLinks();

            Unhighlight();
            this.searchIndex = -1;

            DateTime currentDayBackup = this.currentDay;
            this.currentDay = date;

            if (Properties.Settings.Default.MediaBarVisible == MediaBarVisibility.Always)
            {
                this.mediaToolStrip.Visible = true;
                this.mediaLabel.Text = this.mediaLabel.ToolTipText = "No Media";
            }
            else if (Properties.Settings.Default.MediaBarVisible == MediaBarVisibility.Never)
            {
                this.mediaToolStrip.Visible = false;
            }

            if (date == this.startingDay)
            {
                this.HistoryElement.Style = InvisibleMasterStyle;
                this.HistoryElement.InnerHtml = "";
                this.ContentElement.Style = VisibleMasterStyle;
                this.text.Enabled = true;
                this.mediaPlayer.SetLinks(this.ContentLinks);
            }
            else if (this.loginManager.IsConnected)
            {
                this.HistoryElement.InnerHtml = "";

                if (currentDayBackup == this.startingDay)
                {
                    this.ContentElement.Style = InvisibleMasterStyle;
                    this.HistoryElement.Style = VisibleMasterStyle;
                }

                LoadHistory(date);
                this.text.Enabled = false;
            }

            SetCurrentDayLabel();

            if (this.loginManager.IsConnected)
                RearrangeMessages();
        }

        void LoadHistory(DateTime date)
        {
            EeeDataSet.MessageDataTable messages = this.historyManager.Load(date);

            if (messages != null)
            {
                AddMessages(messages, true);
                messages.DataSet.Dispose();
            }
        }

        bool InHistory
        {
            get
            {
                return this.currentDay != this.startingDay;
            }
        }

        void historyDayLeftToolItem_Click(object sender, EventArgs e)
        {
            GoToDay(this.currentDay.AddDays(-1));
        }

        void historyDayRightToolItem_Click(object sender, EventArgs e)
        {
            GoToDay(this.currentDay.AddDays(1));
        }

        void historyMonthLeftToolItem_Click(object sender, EventArgs e)
        {
            GoToDay(this.currentDay.AddMonths(-1));
        }

        void historyMonthRightToolItem_Click(object sender, EventArgs e)
        {
            GoToDay(this.currentDay.AddMonths(1));
        }

        void replyMenuItem_Click(object sender, EventArgs e)
        {
            Reply();
        }

        void followMenuItem_Click(object sender, EventArgs e)
        {
            Follow();
        }

        void replyToolItem_Click(object sender, EventArgs e)
        {
            Reply();
        }

        void followToolItem_Click(object sender, EventArgs e)
        {
            Follow();
        }

        void CheckUpdateState()
        {
            UpdateManager.UpdatesState state = this.updateManager.State;

            switch (state)
            {
                case UpdateManager.UpdatesState.None:
                    SetUpdateState(null);
                    break;
                case UpdateManager.UpdatesState.Downloadable:
                    SetUpdateState("Updates Available");
                    break;
                case UpdateManager.UpdatesState.Installable:
                    SetUpdateState("Install Updates");
                    break;
            }
        }

        void SetUpdateState(string state)
        {
            if (state == null)
            {
                this.updatesLabel.Visible = false;
            }
            else
            {
                this.updatesLabel.Text = state;
                this.updatesLabel.Visible = true;
            }

            this.updatesLabel.Tag = null;
        }

        void SetUpdateState(string state, Exception error)
        {
            this.updatesLabel.Text = state;
            this.updatesLabel.Tag = error;
        }

        void updateManager_DownloadAllFinished(object sender, UpdateManager.DownloadAllFinishedEventArgs e)
        {
            this.notificationManager.AddNotification(Application.ProductName, 0, "Updates have been downloaded successfully. Please click Install Updates to install.", MessageType.System);
            CheckUpdateState();
        }

        void updateManager_DownloadFailed(object sender, UpdateManager.DownloadFailedEventArgs e)
        {
            SetUpdateState("Update Download Failed", e.Error);
            this.notificationManager.AddNotification("Updates Installation", 0, "One or more updates could not be downloaded.", MessageType.System);
        }

        void updateManager_DownloadStarted(object sender, UpdateManager.DownloadStartedEventArgs e)
        {
            SetUpdateState(string.Format("Downloading update {0} of {1}", e.UpdateNo, e.UpdatesTotal));
        }

        void updateManager_InstallStarted(object sender, UpdateManager.InstallStartedEventArgs e)
        {
            SetUpdateState(string.Format("Installing update {0} of {1}", e.UpdateNo, e.UpdatesTotal));
        }

        void updateManager_InstallAllFinished(object sender, UpdateManager.InstallAllFinishedEventArgs e)
        {
            this.notificationManager.AddNotification(Application.ProductName, 0, "Updates have been installed successfully. Please restart application to apply. Click this balloon to restart.", MessageType.System, NotificationClickAction.Restart);
            CheckUpdateState();
        }

        void updateManager_InstallFailed(object sender, UpdateManager.InstallFailedEventArgs e)
        {
            SetUpdateState("Install Failed", e.Error);
        }

        void disconnectedLabel_Click(object sender, EventArgs e)
        {
            ErrorForm.ShowDisconnectedInfo(this.eeeServiceController);
        }

        void toggleStretchItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContextMenuStrip menu = (ContextMenuStrip)item.Owner;
            ToolStrip strip = (ToolStrip)menu.SourceControl;
            strip.Stretch = !strip.Stretch;
            strip.PerformLayout();
        }

        void chatBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
            {
                Copy();
            }
        }

        void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Search();
        }

        int searchIndex = -1;
        HtmlElement hightLightedElement = null;

        void Search()
        {
            string text = this.searchBox.Text.Trim();

            if (text.Length != 0)
            {
                SearchNext(text);

                if (!this.searchBox.Items.Contains(text))
                    this.searchBox.Items.Add(text);
            }
        }

        void SearchNext(string text)
        {
            Unhighlight();

            EeeDataSet.MessageDataTable messages;

            if (this.InHistory)
                messages = this.historyManager.History;
            else
                messages = this.historyManager.Current;

            int count = messages.Count;

            if (this.searchIndex >= count)
                this.searchIndex = -1;

            EeeDataSet.MessageRow messageMatch = null;
            int foundIndex = -1;

            for (int i = this.searchIndex + 1; i < count; i++)
            {
                if (IsMatch(text, messages[i].Message))
                {
                    foundIndex = i;
                    messageMatch = messages[i];
                    break;
                }
            }

            if (foundIndex < 0)
            {
                for (int i = 0; i <= this.searchIndex; i++)
                {
                    if (IsMatch(text, messages[i].Message))
                    {
                        foundIndex = i;
                        messageMatch = messages[i];
                        break;
                    }
                }
            }

            this.searchIndex = foundIndex;

            if (messageMatch != null)
            {
                HtmlElement mainElement;

                if (this.InHistory)
                    mainElement = this.HistoryElement;
                else
                    mainElement = this.ContentElement;

                HtmlElementCollection messageElements = mainElement.Children;

                string messageIdToGet = messageMatch.MessageID.ToString();
                int messageIndex = foundIndex;

                while (messageIndex < messageElements.Count)
                {
                    string messageId = messageElements[messageIndex].GetAttribute("messageId");

                    if (messageId == messageIdToGet)
                    {
                        HtmlElement elementFound = messageElements[messageIndex];
                        elementFound.ScrollIntoView(true);
                        Highlight(elementFound, text);
                        break;
                    }

                    messageIndex++;
                }
            }
        }

        static Regex HtmlInnerTextRegex = new Regex("(?<Prefix>[<].*?[>])?(?<Text>[^<>]*)(?<Postfix>[<].*?[>])?", RegexOptions.Compiled | RegexOptions.Singleline);

        static string HighlightPrefix = "<span style='text-decoration: underline'>";
        static string HightlightPostfix = "</span>";

        void Highlight(HtmlElement element, string text)
        {
            if (this.hightLightedElement != null)
                Unhighlight();

            StringBuilder builder = new StringBuilder();

            string pattern = Regex.Escape(text);
            Regex replacingRegex = new Regex("(?<Match>" + pattern + ")", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            string replacement = HighlightPrefix + "${Match}" + HightlightPostfix;

            foreach (Match match in HtmlInnerTextRegex.Matches(element.InnerHtml))
            {
                builder.Append(match.Groups["Prefix"].Value);
                builder.Append(replacingRegex.Replace(match.Groups["Text"].Value, replacement));
                builder.Append(match.Groups["Postfix"].Value);
            }

            element.InnerHtml = builder.ToString();
            this.hightLightedElement = element;
        }

        void Unhighlight()
        {
            if (this.hightLightedElement != null)
            {
                foreach (HtmlElement element in this.hightLightedElement.GetElementsByTagName("span"))
                {
                    element.OuterHtml = element.InnerHtml;
                }

                this.hightLightedElement = null;
            }
        }

        bool IsMatch(string text, string message)
        {
            return message.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        void playToolItem_Click(object sender, EventArgs e)
        {
            if (this.mediaPlayer.CurrentMode == MediaPlayer.Mode.None || this.mediaPlayer.CurrentMode == MediaPlayer.Mode.Stopped)
                PlayLink(this.mediaPlayer.CurrentLink);
            else
                this.mediaPlayer.Play();
        }

        void pauseToolItem_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.Pause();
        }

        void stopToolItem_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.Stop();
        }

        void eeeServiceController_DownloadFinished(object sender, BackgroundServiceController.DownloadFinishedEventArgs e)
        {
            if (e.Link == this.linkToPlay)
                PlayFile(e.Link, e.FilePath);

            UpdateDownloadStatus(null);
        }

        void eeeServiceController_DownloadFailed(object sender, BackgroundServiceController.DownloadFailedEventArgs e)
        {
            this.notificationManager.AddNotification("File Download", 0, "Download failed (" + e.Link + ")", MessageType.System);
            UpdateDownloadStatus(e.Error);
        }

        void downloadingLabel_Click(object sender, EventArgs e)
        {
            if (this.downloadingLabel.Tag != null)
                ErrorForm.ShowDownloadProblem((Exception)this.downloadingLabel.Tag, this.eeeServiceController);
        }

        void uploadingLabel_Click(object sender, EventArgs e)
        {
            if (this.uploadingLabel.Tag != null)
                ErrorForm.ShowUploadProblem((Exception)this.uploadingLabel.Tag, this.eeeServiceController);
        }

        void mediaPlayer_ModeChanged(object sender, KolikSoftware.Eee.Client.Media.MediaPlayer.ModeChangedEventArgs e)
        {
            SetupMediaButtons();
        }

        void SetupMediaButtons()
        {
            switch (this.mediaPlayer.CurrentMode)
            {
                case MediaPlayer.Mode.None:
                    this.playToolItem.Enabled = this.mediaPlayer.HasMedia;
                    this.pauseToolItem.Enabled = false;
                    this.stopToolItem.Enabled = false;
                    this.playToolItem2.Enabled = this.mediaPlayer.HasMedia;
                    this.pauseToolItem2.Enabled = false;
                    this.stopToolItem2.Enabled = false;
                    break;
                case MediaPlayer.Mode.Playing:
                    this.playToolItem.Enabled = false;
                    this.pauseToolItem.Enabled = true;
                    this.stopToolItem.Enabled = true;
                    this.playToolItem2.Enabled = false;
                    this.pauseToolItem2.Enabled = true;
                    this.stopToolItem2.Enabled = true;
                    break;
                case MediaPlayer.Mode.Paused:
                    this.playToolItem.Enabled = true;
                    this.pauseToolItem.Enabled = false;
                    this.stopToolItem.Enabled = true;
                    this.playToolItem2.Enabled = true;
                    this.pauseToolItem2.Enabled = false;
                    this.stopToolItem2.Enabled = true;
                    break;
                case MediaPlayer.Mode.Stopped:
                    this.playToolItem.Enabled = true;
                    this.pauseToolItem.Enabled = false;
                    this.stopToolItem.Enabled = false;
                    this.playToolItem2.Enabled = true;
                    this.pauseToolItem2.Enabled = false;
                    this.stopToolItem2.Enabled = false;
                    break;
            }

            this.backToolItem.Enabled = this.mediaPlayer.HasMedia && !this.mediaPlayer.IsFirst;
            this.forwardToolItem.Enabled = this.mediaPlayer.HasMedia && !this.mediaPlayer.IsLast;
        }

        void chatBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.contentElement = this.chatBrowser.Document.GetElementById("Content");
            this.historyElement = this.chatBrowser.Document.GetElementById("History");
        }

        void removeAllMenuItem_Click(object sender, EventArgs e)
        {
            if (this.loginManager.IsConnected)
            {
                if (MessageBox.Show(this, "Do you want to remove all files in your Eee Client Downloads folder?", "Remove Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.mediaPlayer.CloseFile();
                    string destinationDir = this.eeeServiceController.GetPathToSave("Downloads");

                    try
                    {
                        if (Directory.Exists(destinationDir))
                        {
                            foreach (string file in Directory.GetFiles(destinationDir))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        void openFolderMenuItem_Click(object sender, EventArgs e)
        {
            if (this.loginManager.IsConnected)
            {
                BackgroundWorker browserStarter = new BackgroundWorker();
                browserStarter.DoWork += new DoWorkEventHandler(browserStarter_DoWork);
                browserStarter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(browserStarter_RunWorkerCompleted);
                browserStarter.RunWorkerAsync(this.eeeServiceController.GetPathToSave("Downloads"));
            }
        }

        void backToolItem_Click(object sender, EventArgs e)
        {
            bool playing = this.mediaPlayer.CurrentMode == MediaPlayer.Mode.Playing;

            this.mediaPlayer.Stop();

            string link = this.mediaPlayer.PrevLink();

            if (playing)
                PlayLink(link);
        }

        void forwardToolItem_Click(object sender, EventArgs e)
        {
            bool playing = this.mediaPlayer.CurrentMode == MediaPlayer.Mode.Playing;

            this.mediaPlayer.Stop();

            string link = this.mediaPlayer.NextLink();

            if (playing)
                PlayLink(link);
        }

        void closeMediaBarToolItem_Click(object sender, EventArgs e)
        {
            this.mediaToolStrip.Visible = false;
        }

        void mediaPlayer_MediaChanged(object sender, MediaPlayer.MediaChangedEventArgs e)
        {
            bool hasMedia = this.mediaPlayer.HasMedia;

            if (Properties.Settings.Default.MediaBarVisible == MediaBarVisibility.OnMedia && hasMedia != this.mediaToolStrip.Visible)
                this.mediaToolStrip.Visible = hasMedia;

            SetupMediaButtons();

            if (hasMedia && this.mediaLabel.Text != this.mediaPlayer.TrackInfo)
            {
                this.mediaLabel.TextDirection = ToolStripTextDirection.Vertical90;
                this.mediaLabel.Text = "";
                this.mediaLabel.ToolTipText = this.mediaPlayer.TrackInfo;
                AdjustMediaLabelSize();
            }
        }

        int mediaButtonsHeight = 0;
        Graphics currentMediaLabelGraphics;
        StringFormat mediaMeasureStringFormat;

        void PrepareMediaBar()
        {
            foreach (ToolStripItem item in this.mediaToolStrip.Items)
            {
                if (item != this.mediaLabel)
                    this.mediaButtonsHeight += item.Height + item.Padding.Top + item.Padding.Bottom;
            }
        }

        void mediaToolStrip_Resize(object sender, EventArgs e)
        {
            AdjustMediaLabelSize();
        }

        void AdjustMediaLabelSize()
        {
            if (this.currentMediaLabelGraphics == null && this.mediaToolStrip != null && this.mediaToolStrip.Handle != null)
                this.currentMediaLabelGraphics = Graphics.FromHwnd(this.mediaToolStrip.Handle);

            if (this.currentMediaLabelGraphics != null)
            {
                if (this.mediaButtonsHeight == 0)
                    PrepareMediaBar();

                int maxHeight = this.mediaToolStrip.Height - this.mediaButtonsHeight - this.mediaToolStrip.Padding.Top - this.mediaToolStrip.Padding.Bottom - 35;

                string text = this.mediaLabel.ToolTipText;

                if (text != null)
                {
                    int charsFitted;
                    int linesFilled;

                    if (this.mediaMeasureStringFormat == null)
                    {
                        this.mediaMeasureStringFormat = new StringFormat();
                        this.mediaMeasureStringFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.LineLimit;
                        this.mediaMeasureStringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    }

                    this.currentMediaLabelGraphics.MeasureString(text, this.mediaLabel.Font, new SizeF(this.mediaLabel.Width, maxHeight), this.mediaMeasureStringFormat, out charsFitted, out linesFilled);

                    if (charsFitted > 3 && charsFitted < text.Length)
                    {
                        text = text.Substring(0, charsFitted) + "...";
                    }

                    this.mediaLabel.Text = text;
                }
            }
        }

        void showMediaBarTooltem_Click(object sender, EventArgs e)
        {
            if (!this.mediaToolStrip.Visible)
            {
                this.mediaToolStrip.Visible = true;
                this.mediaLabel.Text = this.mediaPlayer.TrackInfo;
            }
        }

        void volumeUpToolItem_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.VolumeUp();
            this.volumeDownToolItem.ToolTipText = string.Format("Volume Down ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            this.volumeUpToolItem.ToolTipText = string.Format("Volume Up ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            Properties.Settings.Default.MediaVolume = this.mediaPlayer.CurrentVolume;
            Properties.Settings.Default.Save();
        }

        void volumeDownToolItem_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.VolumeDown();
            this.volumeDownToolItem.ToolTipText = string.Format("Volume Down ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            this.volumeUpToolItem.ToolTipText = string.Format("Volume Up ({0} %)", this.mediaPlayer.CurrentVolume / 10);
            Properties.Settings.Default.MediaVolume = this.mediaPlayer.CurrentVolume;
            Properties.Settings.Default.Save();
        }

        void copyMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        void text_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length != 1)
            {
                MessageBox.Show(this, "Only one file can be uploaded at once.", "Upload Media", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                UploadMedia(files[0]);
            }
        }

        void uploadMediaMenuItem_Click(object sender, EventArgs e)
        {
            UploadMedia();
        }

        void text_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        Dictionary<string, ToolStripButton> externalUserButtons = new Dictionary<string, ToolStripButton>();

        void eeeServiceController_ExternalUserStateChanged(object sender, CommandProcessor.ExternalUserStateChangedEventArgs e)
        {
            if (e.UserName == "~")
            {
                this.externalUserButtons.Clear();
                this.externalUsersToolStrip.Items.Clear();
            }
            else
            {
                ToolStripButton userButton = null;

                if (this.externalUserButtons.ContainsKey(e.UserId))
                    userButton = this.externalUserButtons[e.UserId];

                if (userButton != null)
                    userButton.Tag = e.UserId;

                if (e.UserState == UserState.Disconnected && userButton != null)
                {
                    RemoveExternalUser(e.UserId);
                }
                else if (e.UserState == UserState.Connected)
                {
                    if (userButton == null)
                        AddExternalUser(e);
                    else
                        SetExternalUserNotAway(e);
                }
                else if (e.UserState == UserState.Away)
                {
                    if (userButton == null)
                        AddExternalUser(e);

                    SetExternalUserAway(e);
                }
            }
        }

        void SetExternalUserAway(CommandProcessor.ExternalUserStateChangedEventArgs e)
        {
            ToolStripButton userButton = this.externalUserButtons[e.UserId];
            userButton.ForeColor = Color.Red;
        }

        void SetExternalUserNotAway(CommandProcessor.ExternalUserStateChangedEventArgs e)
        {
            ToolStripButton userButton = this.externalUserButtons[e.UserId];
            userButton.ForeColor = Color.Black;
        }

        Dictionary<string, string> jidToNameMap = new Dictionary<string, string>();

        void AddExternalUser(CommandProcessor.ExternalUserStateChangedEventArgs e)
        {
            string userName = e.UserName;

            int idx = userName.IndexOf('@');
            if (idx > 0)
                userName = userName.Substring(0, idx);

            ToolStripButton button = new ToolStripButton(userName);

            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.CheckOnClick = true;
            button.Tag = e.UserId;

            this.jidToNameMap[e.UserId] = userName;

            button.Image = Properties.Resources.ExternalUser;
            button.CheckedChanged += new EventHandler(externalUserButton_CheckedChanged);
            //button.MouseUp += new MouseEventHandler(userButton_MouseUp);

            this.externalUsersToolStrip.Items.Add(button);
            this.externalUserButtons.Add(e.UserId, button);

            this.notificationManager.AddNotification("Connected", 0, e.UserName, MessageType.Connection);

            //button.MouseHover += new EventHandler(userButton_MouseHover);
            //button.MouseLeave += new EventHandler(userButton_MouseLeave);
        }

        void externalUserButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.freezeEvents) return;
            this.freezeEvents = true;

            ToolStripButton button = sender as ToolStripButton;

            if (button.Checked)
            {
                UncheckUserButtons(null);
                UncheckExternalButtons(button);
            }

            RearrangeMessages();

            this.freezeEvents = false;
        }

        void UncheckUserButtons(ToolStripButton exceptFor)
        {
            foreach (ToolStripItem item in this.usersToolStrip.Items)
            {
                if (item.Tag != null)
                {
                    if (item != exceptFor && ((EeeDataSet.UserRow)item.Tag).UserID != this.eeeServiceController.CurrentUser.UserID)
                        ((ToolStripButton)item).Checked = false;
                }
            }
        }

        void UncheckExternalButtons(ToolStripButton exceptFor)
        {
            foreach (ToolStripItem item in this.externalUsersToolStrip.Items)
            {
                if (item != exceptFor)
                    ((ToolStripButton)item).Checked = false;
            }
        }

        void RemoveExternalUser(string userId)
        {
            ToolStripButton userButton = this.externalUserButtons[userId];
            this.externalUsersToolStrip.Items.Remove(userButton);
            this.externalUserButtons.Remove(userId);
            this.notificationManager.AddNotification("Disconnected", 0, userButton.Text, MessageType.Connection);
        }

    }
}
