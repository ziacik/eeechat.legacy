using System;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Eee.Client
{
    class Class1
    {

        #region Windows API
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32LastInputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern bool GetLastInputInfo(out Win32LastInputInfo plii);

        private const int SB_VERT = 1;
        private const int EM_SETSCROLLPOS = 0x0400 + 222;

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, POINT lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetScrollRange(System.IntPtr hWnd, int nBar, out int minPos, out int maxPos);

        public enum SND
        {
            SND_SYNC = 0x0000,/* play synchronously (default) */
            SND_ASYNC = 0x0001, /* play asynchronously */
            SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004, /* pszSound points to a memory file */
            SND_LOOP = 0x0008, /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000,/* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a pre d ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004, /* name is resource name or atom */
            SND_PURGE = 0x0040,  /* purge non-static events for task */
            SND_APPLICATION = 0x0080 /* look for application specific association */
        }

        [DllImport("winmm.dll", EntryPoint = "PlaySound", CharSet = CharSet.Auto)]
        private static extern int PlaySound(string fileName, int module, int flags);

        #endregion

        private bool menuExit = false;

        private Notificator notificator = null;
        private CommandProcessor commandProcessor = null;

        private EeeDataSet.RoomDataTable rooms = null;

        private int balloonDelay = 4;




        protected void AfkUser(string user, string comment)
        {
            ListViewItem userItem = FindUserItem(user);

            if (userItem != null)
            {
                userItem.ForeColor = Color.Red;
                userItem.ToolTipText = comment;
                PlaySound(SettingsManager.AfkSound);
            }
        }

        protected void UnafkUser(string user)
        {
            ListViewItem userItem = FindUserItem(user);

            if (userItem != null && userItem.ForeColor == Color.Red)
            {
                userItem.ForeColor = Color.Black;
                userItem.ToolTipText = null;
                PlaySound(SettingsManager.UnafkSound);
            }
        }

        protected void ClearStateUser(string user)
        {
            UnafkUser(user);
        }

        protected void LoadRooms()
        {
            try
            {
                this.rooms = eee.GetRooms();

                this.roomList.BeginUpdate();

                try
                {
                    this.roomList.Items.Clear();

                    foreach (EeeDataSet.RoomRow room in rooms)
                    {
                        ListViewItem roomItem = this.roomList.Items.Add(room.Name);
                        roomItem.Tag = room.RoomID;
                    }

                    this.roomsLoaded = true;
                }
                finally
                {
                    this.roomList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                this.notificator.ReportError(this, "Can't load rooms.", ex);
            }
        }

        private bool firstUserSynchronization = true;

        private void SynchronizeUsers()
        {
            if (this.receiver == null || !this.receiver.HasUserSyncs())
                return;

            EeeDataSet.UserRow user = this.receiver.GetUserSync();

            while (user != null)
            {
                if (user.State == 0)
                {
                    LogoutUser(user.Login);
                }
                else if (user.State > 0)
                {
                    LoginUser(user.Login);

                    if (user.State == 2)
                        AfkUser(user.Login, null);
                }

                user = this.receiver.GetUserSync();
            }

            SetUserIcons();

            this.firstUserSynchronization = false;
        }

        private void SetUserIcons()
        {
            foreach (ListViewItem item in usersList.Items)
            {
                if (item.Index < 12)
                    item.ImageIndex = item.Index;
                else
                    item.ImageIndex = -1;
            }
        }

        private void ScrollDown()
        {
            this.webBrowser1.Document.Window.ScrollTo(0, 32767);
        }

        private void AddMessages()
        {
            if (this.receiver == null || !this.receiver.HasMessages())
                return;

            bool doScroll = false;
            bool wasAtLeastOnePrivate = false;
            bool wasAtLeastOneNotFromMe = false;

            EeeDataSet.MessageRow message = this.receiver.GetMessage();

            while (message != null)
            {
                bool added = AddMessage(message);
                doScroll = doScroll || added;

                if (added)
                {
                    wasAtLeastOnePrivate = wasAtLeastOnePrivate || message.ToUserID == myUserID;
                    wasAtLeastOneNotFromMe = wasAtLeastOneNotFromMe || message.FromUserID != myUserID;
                }

                message = this.receiver.GetMessage();
            }

            if (doScroll && this.lockViewMenuItem.Checked == false && this.lockViewToolItem.Checked == false)
            {
                ScrollDown();
            }

            if (wasAtLeastOneNotFromMe)
            {
                if (wasAtLeastOnePrivate)
                    PlaySound(SettingsManager.NewPrivateMessageSound);
                else
                    PlaySound(SettingsManager.NewMessageSound);
            }
        }

        private Font textTitleFont = new Font("Arial", 9, FontStyle.Bold);
        private Font textFontRegular = new Font("Arial", 9, FontStyle.Regular);
        private Font textFontItalic = new Font("Arial", 9, FontStyle.Italic);

        private bool AddMessage(EeeDataSet.MessageRow message)
        {
            bool continueProcessing;

            commandProcessor.ProcessMessage(message, out continueProcessing);

            bool doScroll = false;

            if (continueProcessing)
            {
                if (CanShowInRoom(message))
                {
                    string time = message.Time.ToString("d.M. HH:mm");

                    StringBuilder builder = new StringBuilder();

                    builder.Append("<b><font color='gray'>&lt;<font color='#");
                    builder.Append(message.Color.ToString("x"));
                    builder.Append("'>");
                    builder.Append(message.Login);
                    builder.Append("</font> ");
                    builder.Append(time);

                    if (message.IsRoomNull() == false && message.Room != null && message.Room.Trim() != "")
                    {
                        builder.Append(" ");
                        builder.Append(message.Room.ToUpper());
                    }

                    builder.Append("&gt;</font></b><br/><font color='#");
                    builder.Append(message.Color.ToString("x"));
                    builder.Append("'>");

                    if (message.IsToUserIDNull() == false && message.ToUserID != 0)
                    {
                        builder.Append("<i>");
                    }

                    string msgText = message.Message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br/>");

                    do
                    {
                        int smilieNo;
                        string text;

                        GetNearestSmilieAndText(ref msgText, out smilieNo, out text);

                        if (text != "")
                        {
                            builder.Append(TransformAddresses(text));
                        }

                        if (smilieNo >= 0)
                        {
                            builder.Append("<img src='file://");
                            string pathToSmilie = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Smilies\" + smilieNo + ".gif");
                            builder.Append(pathToSmilie);
                            builder.Append("' />");
                        }
                    }
                    while (msgText != "");

                    if (message.IsToUserIDNull() == false && message.ToUserID != 0)
                    {
                        builder.Append("</i>");
                    }

                    builder.Append("</font><br /><br />");

                    string messageHtml = builder.ToString();

                    HtmlElement element = this.webBrowser1.Document.CreateElement("div");
                    element.InnerHtml = messageHtml;
                    this.webBrowser1.Document.GetElementById("Content").AppendChild(element);

                    WebBrowser userBrowser = GetUserBrowser(message);

                    if (userBrowser != null)
                    {
                        HtmlElement element2 = userBrowser.Document.CreateElement("div");
                        element2.InnerHtml = messageHtml;
                        HtmlElement elementx = userBrowser.Document.GetElementById("Content");
                        if (elementx != null)
                            elementx.AppendChild(element2);
                    }

                    doScroll = true;

                    if (message.FromUserID != myUserID)
                    {
                        AddNotification(message.Login + " " + time, message.Message, message.Color, message);

                        this.Text = "Eee - o O o - o O o - o O o - o O o - ";
                        notificationTimer.Start();
                    }

                    if (message.FromUserID != myUserID && !message.IsToUserIDNull() && message.ToUserID != 0)
                        this.lastToFrom = message.Login;

                    AddToHistory(message);
                }
            }

            return doScroll;
        }

        private string linkImage = null;
        private string mailImage = null;

        Regex webAddressRegex = new Regex(@"(?<prefix>([\s():;,><'""=+]|^)+)(?<hyperlink>(?<protocol>([-a-zA-Z]+://)?)(?<server>([\w-]*[a-zA-Z][\w-]*)?(\.?[\w-]*[a-zA-Z][\w-]*)+\.\w{2,4}(:[0-9]+)?)(/[\w\.\?+&%#;\-=]*)*)(?<postfix>([^\w/]|$)+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        Regex mailAddressRegex = new Regex(@"(?<prefix>(\W|^)+)(mailto:)?(?<mail>[\w\.]+@\w+\.\w+(\.\w+)*)(?<postfix>(\W|$)+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private string TransformAddresses(string message)
        {
            if (message.IndexOf('.') < 0)
                return message;

            if (linkImage == null)
                this.linkImage = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Smilies\link.png"); //TODO: Smilies->Resources

            if (mailImage == null)
                this.mailImage = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Smilies\mail.png"); //TODO: Smilies->Resources

            MatchEvaluator evaluator = new MatchEvaluator(HyperlinkEvaluator);

            message = this.webAddressRegex.Replace(message, evaluator);
            message = this.mailAddressRegex.Replace(message, @"${prefix}<a href='mailto:${mail}'><img border='0' src='file://" + mailImage + "' /></a> <a href='mailto:${mail}'>${mail}</a>${postfix}");

            message = message.Replace("[LINKIMAGE]", linkImage);
            message = message.Replace("[MAILIMAGE]", mailImage);

            return message;
        }

        private string HyperlinkEvaluator(Match m)
        {
            string noProtocolReplacement = @"${prefix}<a href='${hyperlink}'><img border='0' src='file://" + linkImage + "' /></a> <a href='${hyperlink}'>Link at ${protocol}${server}</a>${postfix}";
            string httpProtocolReplacement = @"${prefix}<a href='http://${hyperlink}'><img border='0' src='file://" + linkImage + "' /></a> <a href='http://${hyperlink}'>Link at http://${server}</a>${postfix}";

            string replaced;

            if (m.Groups["protocol"].Value.Trim() == "")
                replaced = m.Result(httpProtocolReplacement);
            else
                replaced = m.Result(noProtocolReplacement);

            return replaced;
        }

        private WebBrowser GetUserBrowser(EeeDataSet.MessageRow message)
        {
            int index = message.Message.IndexOf(':');
            if (index < 0) return null;

            string user = message.Message.Substring(0, index).ToLower();

            ListViewItem userItem = FindUserItem(user);
            if (userItem == null) return null;

            string key = null;

            if (user == this.myLogin.ToLower())
                key = message.Login.ToLower();
            else if (message.Login.ToLower() == this.myLogin.ToLower())
                key = user;

            if (key == null)
                return null;

            if (this.tabControl1.TabPages.ContainsKey(key))
                return this.tabControl1.TabPages[key].Controls[0] as WebBrowser;

            this.tabControl1.TabPages.Add(key, key);

            WebBrowser newWebBrowser = new WebBrowser();
            this.tabControl1.TabPages[key].Controls.Add(newWebBrowser);
            newWebBrowser.Dock = DockStyle.Fill;

            InitializeBrowser(newWebBrowser);

            return newWebBrowser;
        }

        private void InitializeBrowser(WebBrowser webBrowser)
        {
            webBrowser.DocumentText = "<html><body><div id='Content' style='font-family: Arial; font-size: 12px;'></div></body></html>";
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
        }

        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = true;

            string url = e.Url.ToString();

            if (url.StartsWith("http://") == false && url.StartsWith("https://") == false && url.StartsWith("mailto:") == false)
                System.Diagnostics.Process.Start("iexplore", e.Url.ToString());
            else
                System.Diagnostics.Process.Start(e.Url.ToString());
        }

        protected void GetNearestSmilieAndText(ref string msgText, out int smilieNo, out string text)
        {
            int min = int.MaxValue;

            smilieNo = -1;

            int smilieIdx = -1;

            for (int i = smilies.Length - 1; i >= 0; i--) /// From more complex to less
            {
                int idx = msgText.IndexOf(this.smilies[i]);

                if (idx >= 0)
                {
                    if (idx < min)
                    {
                        if (smilieImages[smilieIDs[i]] != null)
                        {
                            smilieNo = smilieIDs[i];
                            smilieIdx = i;
                            min = idx;
                        }
                    }
                }
            }

            if (min == int.MaxValue)
            {
                text = msgText;
                msgText = "";
            }
            else
            {
                text = msgText.Substring(0, min);
                msgText = msgText.Substring(min + this.smilies[smilieIdx].Length);

                int removeFirst = 0;

                if (msgText.Length > 0)
                {
                    while (removeFirst < msgText.Length && (msgText[removeFirst] == ')' || msgText[removeFirst] == '(' || msgText[removeFirst] == '>' || msgText[removeFirst] == '<' || msgText[removeFirst] == '|'))
                    {
                        removeFirst++;
                    }

                    if (removeFirst > 0)
                        msgText = msgText.Substring(removeFirst);
                }
            }
        }

        protected bool CanShowInRoom(EeeDataSet.MessageRow message)
        {
            if (message.ToUserID != 0)
                return true;
            if (message.IsRoomIDNull())
                return true;
            if (this.rooms == null)
                return true;

            EeeDataSet.RoomRow room;

            if (message.RoomID == 0)
                room = this.rooms[0];
            else
                room = this.rooms.FindByRoomID(message.RoomID);

            if (room == null)
            {
                SaveRooms(); /// So that active room selection is saved.
                LoadRooms();

                room = this.rooms.FindByRoomID(message.RoomID);
            }

            if (room == null)
                message.Room = "Neznáma miestnos";
            else if (room != this.rooms[0])
                message.Room = room.Name;

            if (room == null)
                return true;
            else
                return InRoom(room.Name);
        }

        private void StopNotification()
        {
            notificationTimer.Stop();
            NotifyIcon(false);

            SetFormTitle();
        }

        private Font titleFont = new Font("Arial Black", (float)9, FontStyle.Regular);
        private Font messageFontNormal = new Font("Arial", (float)8.25, FontStyle.Bold);
        private Font messageFontItalic = new Font("Arial", (float)8.25, FontStyle.Italic);
        private StringFormat messageFormat = new StringFormat();

        /*private bool Authenticate(string loginName, string password)
        {
            EeeDataSet.LoginUserRow loginUser = eee.GetUser(loginName);

            if (loginUser != null) 
            {
                string passwordHash = Security.CreatePasswordHash(password, loginUser.Salt);

                if (eee.ChangeState(loginUser.UserID, passwordHash, 1, "")) /// LOGIN
                {
                    myUserID = loginUser.UserID;
                    myPasswordHash = passwordHash;

                    return true;
                }
            }

            return false;
        }

        private bool AutoAuthenticate(string loginName, string passwordHash)
        {
            EeeDataSet.LoginUserRow loginUser = eee.GetUser(loginName);

            if (loginUser != null) 
            {
                if (eee.ChangeState(loginUser.UserID, passwordHash, 1, "")) /// LOGIN
                {
                    myUserID = loginUser.UserID;
                    myPasswordHash = passwordHash;

                    return true;
                }
            }

            return false;
        }
        */
        private void ReportError(Exception ex, bool showMessage)
        {
            if (this.receiver != null)
                this.receiver.ResetLastRefresh();

            SetStatus("CHYBA: " + ex.Message);
            statusBarCounter = 2;

            if (showMessage)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void messages_LinkClicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private bool handleKey = false;

        private void text_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                int userToAdd = -1;

                if (e.KeyCode == Keys.F1)
                    userToAdd = 0;
                else if (e.KeyCode == Keys.F2)
                    userToAdd = 1;
                else if (e.KeyCode == Keys.F3)
                    userToAdd = 2;
                else if (e.KeyCode == Keys.F4)
                    userToAdd = 3;
                else if (e.KeyCode == Keys.F5)
                    userToAdd = 4;
                else if (e.KeyCode == Keys.F6)
                    userToAdd = 5;
                else if (e.KeyCode == Keys.F7)
                    userToAdd = 6;
                else if (e.KeyCode == Keys.F8)
                    userToAdd = 7;
                else if (e.KeyCode == Keys.F9)
                    userToAdd = 8;
                else if (e.KeyCode == Keys.F10)
                    userToAdd = 9;
                else if (e.KeyCode == Keys.F11)
                    userToAdd = 10;
                else if (e.KeyCode == Keys.F12)
                    userToAdd = 11;

                if (userToAdd >= 0)
                {
                    if (usersList.Items.Count > userToAdd)
                        AddUserToText(usersList.Items[userToAdd].Text);
                }
            }
            else if (e.Modifiers == Keys.Control)
            {
                int roomToSelect = -1;

                if (e.KeyCode == Keys.F1)
                    roomToSelect = 0;
                else if (e.KeyCode == Keys.F2)
                    roomToSelect = 1;
                else if (e.KeyCode == Keys.F3)
                    roomToSelect = 2;
                else if (e.KeyCode == Keys.F4)
                    roomToSelect = 3;
                else if (e.KeyCode == Keys.F5)
                    roomToSelect = 4;
                else if (e.KeyCode == Keys.F6)
                    roomToSelect = 5;
                else if (e.KeyCode == Keys.F7)
                    roomToSelect = 6;
                else if (e.KeyCode == Keys.F8)
                    roomToSelect = 7;
                else if (e.KeyCode == Keys.F9)
                    roomToSelect = 8;
                else if (e.KeyCode == Keys.F10)
                    roomToSelect = 9;
                else if (e.KeyCode == Keys.F11)
                    roomToSelect = 10;
                else if (e.KeyCode == Keys.F12)
                    roomToSelect = 11;

                if (roomToSelect >= 0)
                {
                    if (roomList.Items.Count > roomToSelect)
                        roomList.Items[roomToSelect].Selected = true;
                }
            }

            if (e.Control && e.KeyCode == Keys.R)
            {
                Reply();
            }

            if ((e.Control && e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.S))
            {
                if (e.KeyCode != Keys.S) //TODO:
                    this.handleKey = true;

                string send = this.text.Text.Trim();
                this.text.Text = "";

                if (send != "")
                {
                    string toUser = null;
                    bool goAfk = false;

                    send = AdjustIfTo(send, ref toUser);
                    send = AdjustIfAfk(send, ref goAfk);

                    if (!goAfk)
                    {
                        if (this.awayModeToolItem.Checked)
                            SendUNAFK();

                        SendMessage(send, CurrentRoomID, toUser);
                    }
                    else
                    {
                        SendAFK(send);
                    }
                }
            }
        }

        private int GetRoom()
        {
            if (roomList.SelectedItems.Count == 0)
                roomList.Items[0].Selected = true;

            return (int)roomList.SelectedItems[0].Tag;
        }

        private string AdjustIfTo(string send, ref string toUser)
        {
            if (send[0] == '/' && send.IndexOf(":") > 0)
            {
                int idx = send.IndexOf(":");

                toUser = send.Substring(1, idx - 1);

                return send.Substring(1);

            }
            else
            {
                return send;
            }
        }

        private string AdjustIfAfk(string send, ref bool goAfk)
        {
            if (send.ToUpper() == "/AFK") send = "/AFK ";

            if (send.ToUpper().IndexOf("/AFK ") == 0)
            {
                goAfk = true;
                return send.Substring(5);
            }
            else
            {
                return send;
            }
        }

        private void text_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (this.handleKey)
            {
                e.Handled = true;
                this.handleKey = false;
            }
        }

        private void SendMessage(string text, int roomID, string toUser)
        {
            if (myUserID == 0) return;

            if (this.sender != null)
            {
                this.sender.Send(text, roomID, toUser);
                SetStatusMessages();
            }
        }

        private bool lastIconNotification = false;

        private void NotifyIcon(bool activeSecond)
        {
            if (activeSecond)
            {
                notifyIcon.Icon = Properties.Resources.Notify;
            }
            else
            {
                if (this.awayModeToolItem.Checked)
                    notifyIcon.Icon = Properties.Resources.Afk;
                else
                    notifyIcon.Icon = this.Icon;
            }

            if (this.Text.Length > 0)
                this.Text = this.Text.Substring(1) + this.Text[0];
        }

        private void SetFormTitle()
        {
            string user = "(disconnected)";

            if (this.myLogin != null)
                user = this.myLogin;

            this.Text = "Eee - " + user;
        }

        private string lastToFrom;

        private void Reply()
        {
            if (this.lastToFrom != null)
                this.text.SelectedText = "/" + this.lastToFrom + ": ";
        }

        /// <returns>True, ak mám zapnutú túto room.</returns>
        private bool InRoom(string room)
        {
            if (room == null)
                return roomList.Items[0].Checked;

            room = room.Trim();

            foreach (ListViewItem item in roomList.Items)
            {
                if (item.Text.Trim() == room)
                {
                    return item.Checked;
                }
            }

            return false;
        }

        private bool CheckVersion(string text, bool showMessageIfCurrentVersion)
        {
            /*
			Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			string myVersion = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString();

			try 
			{
				string currentVersion = eee.Version().Trim();

				if (myVersion.CompareTo(currentVersion) < 0 ) 
				{
					string addText = "";

					if (text != null && text.Trim() != "")
						addText = "\n\n" + text;

					AddNotification("Info", "Vznikla nová verzia Eee klienta!", 0, null);

					if (MessageBox.Show("Vznikla nová verzia Eee klienta! Program bude automaticky aktualizovaný." + addText, "Nová verzia", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK) 
					{
                        Exit();
			
						string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"tools\eeeclientupdater.exe");

						System.Diagnostics.Process.Start(path, "EeeClient" + currentVersion.Replace(".", "_") + ".zip");

						return true;
					}
				} 
				else if (showMessageIfCurrentVersion)
				{
					MessageBox.Show(this, "Aktualizácia nie je potrebná, toto je aktuálna verzia klienta.", "Aktualizácia", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

			}
			catch (Exception ex)
			{
				this.notificator.ReportError(this, "Could not check version.", ex);
			}
            */
            return false;
        }

        private void SetStatus(string text)
        {
            if (text == null)
            {
                text = GetVersion();
            }

            this.statusLabel.Text = text;
        }

        private void SetStatusMessages()
        {
            if (this.sender != null)
            {
                int toSend = this.sender.ToSend;

                string sprav;
                if (toSend == 1)
                    sprav = "správa";
                else if (toSend >= 2 && toSend <= 4)
                    sprav = "správy";
                else
                    sprav = "správ";

                this.messagesToSendStatus.Text = toSend.ToString() + " " + sprav + " na odoslanie.";
            }
        }

        private void usersList_Click(object sender, System.EventArgs e)
        {
            if (usersList.SelectedItems.Count == 1)
                AddUserToText(usersList.SelectedItems[0].Text);
        }

        private void AddUserToText(string user)
        {
            text.SelectedText = user + ": ";
        }

        private void MainForm_Resize(object sender, System.EventArgs e)
        {
            if (this.ShowInTaskbar == false && this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }

            if (this.WindowState != FormWindowState.Minimized)
                this.lastState = this.WindowState;
        }

        private void MainForm_Closed(object sender, System.EventArgs e)
        {
        }

        private string GetVersion()
        {
            /*Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string myVersion = version.Major.ToString() + "." + version.Minor.ToString();*/

            return "Eee Client 2006";
        }


        private void roomList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (roomList.SelectedItems.Count == 0 || roomList.SelectedItems[0].Index == 0)
            {
                text.BackColor = Color.WhiteSmoke;
                roomInfoPanel.Visible = false;
            }
            else
            {
                text.BackColor = Color.SeaShell;
                roomInfoLabel.Text = roomList.SelectedItems[0].Text;
                roomInfoPanel.Visible = true;
            }
        }

        private void LoadSettings()
        {
            this.ShowInTaskbar = SettingsManager.ShowInTaskbar;

            /*if (this.ShowInTaskbar)
                this.FormBorderStyle = FormBorderStyle.Sizable;
            else
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;*/

            this.balloonDelay = SettingsManager.Delay;

            ShowBalloons = SettingsManager.ShowBubblesDefault;

            for (int i = 0; i < roomList.Items.Count; i++)
            {
                roomList.Items[i].Checked = SettingsManager.Room(i);
            }

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (SettingsManager.AutoStart)
            {
                registryKey.SetValue("EeeClient", Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue("EeeClient", false);
            }
        }

        private string CurrentRoom
        {
            get
            {
                if (roomList.Items.Count == 0)
                    return "";

                if (roomList.SelectedItems.Count == 0)
                    return roomList.Items[0].Text;

                return roomList.SelectedItems[0].Text;
            }
        }

        private string CurrentUser
        {
            get
            {
                if (this.usersList.Items.Count == 0)
                    return "";

                if (this.usersList.SelectedItems.Count == 0)
                    return "";

                return this.usersList.SelectedItems[0].Text;
            }
        }

        private int CurrentRoomID
        {
            get
            {
                if (roomList.SelectedItems.Count == 0)
                    return 0;

                return (int)roomList.SelectedItems[0].Tag;
            }
        }

        private void CheckCurrentRoom()
        {
            SendMessage("[CHECKROOM " + CurrentRoom + "]", 0, "");
        }

        private void SaveRooms()
        {
            for (int i = 0; i < roomList.Items.Count; i++)
            {
                SettingsManager.SetRoom(i, roomList.Items[i].Checked);
            }
        }

        private void SaveSettings()
        {
        }

        private void LoadLayout()
        {
            this.Location = SettingsManager.Location;
            this.Size = SettingsManager.Size;
            //this.rightSidePanel.Width = SettingsManager.RightSidePanelWidth;
            this.usersList.Height = SettingsManager.UsersListHeight;
            this.mainBottomPanel.Height = SettingsManager.BottomPanelHeight;
            this.WindowState = SettingsManager.WindowState;
        }

        private void SaveLayout()
        {
            SettingsManager.WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                SettingsManager.Location = this.Location;
                SettingsManager.Size = this.Size;
                //SettingsManager.RightSidePanelWidth = this.rightSidePanel.Width;
                SettingsManager.UsersListHeight = this.usersList.Height;
                SettingsManager.BottomPanelHeight = this.mainBottomPanel.Height;
            }

        }


        protected void SendAFK(string comment)
        {
            /*
			try 
			{
                this.awayModeToolItem.Checked = true;
                this.notifyIcon.Icon = Properties.Resources.Afk;

				if (this.sender != null)
					this.sender.ChangeState(2, comment);

				if (this.receiver != null)
					this.receiver.CheckNow();
			}
			catch (Exception ex)
			{
				this.notificator.ReportError(this, "Could not set AFK mode.", ex);
			}*/
        }

        protected void SendUNAFK()
        {
            /*
			try 
			{
                this.awayModeToolItem.Checked = false;
				this.notifyIcon.Icon = this.Icon;

				if (this.sender != null)
					this.sender.ChangeState(1, "");

				if (this.receiver != null) 
				{
					this.receiver.UnPause();
					this.receiver.CheckNow();
				}
			}
			catch (Exception ex)
			{
				this.notificator.ReportError(this, "Could not set AFK mode.", ex);
			}*/
        }

        private void sender_Sent(object sender, EventArgs e)
        {
            if (this.receiver != null)
                if (SettingsManager.RefreshAfterSend)
                    this.receiver.CheckNow();

            SetStatusMessages();
        }

        private void checkMessagesTimer_Tick(object sender, System.EventArgs e)
        {
            SynchronizeUsers();
            AddMessages();
            CheckAutoAfk();
        }

        private byte statusBarCounter = 0;

        private void statusBarTimer_Tick(object sender, System.EventArgs e)
        {
            if (statusBarCounter == 1)
                SetStatus(null);
            if (statusBarCounter > 0)
                statusBarCounter--;
        }

        private void balloonTimer_Tick(object sender, System.EventArgs e)
        {
            lock (notifications.SyncRoot)
            {
                if (notifications.Count > 0)
                    if (balloon == null || !balloon.Visible)
                        Notify();
            }
        }

        private void notificationTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.active)
            {
                RemoveAllNotifications();
                StopNotification();
                HideBalloon();
            }
            else
            {
                this.lastIconNotification = this.lastIconNotification == false;
                NotifyIcon(this.lastIconNotification);
                notificationTimer.Start();
            }
        }

        private void notificator_Error(object sender, KolikSoftware.Eee.Service.ErrorEventArgs e)
        {
            SetStatus(e.Message);
            statusBarCounter = 2;
        }

        private void commandProcessor_CheckVersion(object sender, CommandEventArgs e)
        {
            CheckVersion(e.Param1, false);
        }

        private void commandProcessor_Identify(object sender, CommandEventArgs e)
        {
            try
            {
                string winName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                string myHostName = System.Net.Dns.GetHostName();
                /*System.Net.IPHostEntry ipE = System.Net.Dns.GetHostByName (myHostName);
                System.Net.IPAddress [] IpA = ipE.AddressList;

                string ips = "";

                for (int i = 0; i < IpA.Length; i++)
                {
                    ips += "IP Address " + i.ToString() + IpA[i].ToString ();
                }*/

                eee.AddReport("IDENTIFY", "User '" + myLogin + "': Logon: " + winName + " " /*+ ips*/);
            }
            catch (Exception ex)
            {
                this.notificator.ReportError(this, "Could not Identify.", ex);
            }
        }

        private void commandProcessor_ReportVer(object sender, CommandEventArgs e)
        {
            string returnTo = e.Param1;

            SendMessage("[REPORTVER_REPLY " + e.Param1 + ";VERZIA KLIENTA (" + myLogin + "): " + GetVersion() + "]", 0, returnTo);
        }

        private void commandProcessor_State(object sender, CommandEventArgs e)
        {
            if (e.Param2 == "0") /// LOGOUT
            {
                LogoutUser(e.Param1);
                SetUserIcons();
            }
            else if (e.Param2 == "1") /// LOGIN or UNAFK
            {
                LoginUser(e.Param1);
                SetUserIcons();

                if (e.Param1 == myLogin)
                    if (this.receiver != null)
                        this.receiver.UnPause();
            }
            else if (e.Param2 == "2") ///AFK
            {
                AfkUser(e.Param1, e.Param3);

                if (e.Param1 == myLogin)
                    if (this.receiver != null)
                        this.receiver.Pause();
            }
        }

        private void autoLogoffTimer_Tick(object sender, System.EventArgs e)
        {
            /*
			if (this.eee != null)
			{
				try 
				{
					this.eee.AutoLogoff();
				}
				catch
				{
				}
			}*/
        }



        private void PlaySound(string path)
        {
            if (!SettingsManager.SoundsActive)
                return;

            if (path == null || path.Trim() == "")
                return;

            PlaySound(path, 0, (int)(SND.SND_FILENAME | SND.SND_NOWAIT | SND.SND_ASYNC));
        }

        protected void AddToHistory(EeeDataSet.MessageRow message)
        {
            if (!SettingsManager.HistoryActive)
                return;

            string historyDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "History");

            if (!Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);

            string historyFile = Path.Combine(historyDir, this.myLogin + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".log");

            using (FileStream stream = new FileStream(historyFile, FileMode.Append))
            {
                XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

                writer.WriteStartElement("Message");
                writer.WriteElementString("Message", message.Message);
                writer.WriteElementString("Color", message.Color.ToString());
                writer.WriteElementString("FromUserID", message.FromUserID.ToString());
                writer.WriteElementString("Login", message.Login);
                if (!message.IsRoomNull())
                    writer.WriteElementString("Room", message.Room);
                if (!message.IsRoomIDNull())
                    writer.WriteElementString("RoomID", message.RoomID.ToString());
                writer.WriteElementString("Time", XmlConvert.ToString(message.Time, XmlDateTimeSerializationMode.Utc));
                writer.WriteElementString("ToUserID", message.ToUserID.ToString());
                writer.WriteEndElement();

                writer.Close();
            }
        }

        private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                MinimizeMe();
            }
        }

        private void switchBalloonHotKey_HotkeyPressed(object sender, System.EventArgs e)
        {
            ShowBalloons = !ShowBalloons;
        }

        private bool ShowBalloons
        {
            get
            {
                return this.balloonsMenuItem.Checked;
            }
            set
            {
                this.freezeEvents = true;

                this.balloonsMenuItem.Checked = value;
                this.balloonsNotifyItem.Checked = value;

                if (value == false)
                    HideBalloon();

                this.freezeEvents = false;
            }
        }

        protected const int SmilieCount = 11;
        protected Image[] smilieImages = new Image[SmilieCount];
        protected string[] smilies = new string[] { ":)", ":-)", ";)", ";-)", ":(", ":-(", ":o", ":-o", ":'", ":'(", ":'-(", ":<", ":-<", ":((", ":-((", ">:-<", ":-|", ":-P", ":P", ":->", ":-))", ":))", ":~/", ":-||", @"\_/" };
        protected int[] smilieIDs = new int[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 4, 4, 4, 4, 5, 9, 6, 6, 7, 7, 7, 8, 5, 10 };

        protected void LoadSmilies()
        {
            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Smilies");

            for (int i = 0; i < SmilieCount; i++)
            {
                try
                {
                    smilieImages[i] = new Bitmap(Path.Combine(path, i.ToString() + ".gif"));
                }
                catch (Exception ex)
                {
                    Notificator notificator = new Notificator();
                    notificator.ReportError(null, "Nemožno naèíta obrázok " + i.ToString() + ".gif z adresára Smilies.", ex);
                }
            }
        }

    
        private int messagesHeight = 0;


        private bool freezeEvents = false;

        private void awayModeToolItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.freezeEvents) return;

            this.freezeEvents = true;

            this.awayModeMenuItem.Checked = this.awayModeToolItem.Checked;
            this.awayModeNotifyItem.Checked = this.awayModeToolItem.Checked;

            AwayModeChanged();

            this.freezeEvents = false;
        }

        private void lockViewMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.lockViewToolItem.Checked != this.lockViewMenuItem.Checked)
                this.lockViewToolItem.Checked = this.lockViewMenuItem.Checked;
        }

        private void lockViewToolItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.lockViewMenuItem.Checked != this.lockViewToolItem.Checked)
                this.lockViewMenuItem.Checked = this.lockViewToolItem.Checked;
        }



        private void Register()
        {
            using (Register register = new Register())
            {
                register.ShowDialog();
            }
        }

        private void Settings()
        {
            SaveSettings();

            using (Options options = new Options())
            {
                if (options.Run())
                {
                    LoadSettings();
                    if (this.receiver != null)
                        this.receiver.Refresh = SettingsManager.Refresh;
                }
            }
        }

        private void connectMenuItem_Click(object sender, EventArgs e)
        {
            Login(false);
        }

        private void registerMenuItem_Click(object sender, EventArgs e)
        {
            Register();
        }

        private void replyMenuItem_Click(object sender, EventArgs e)
        {
            Reply();
        }

        private void replyToolItem_Click(object sender, EventArgs e)
        {
            Reply();
        }

        private void roomStatusMenuItem_Click(object sender, EventArgs e)
        {
            CheckCurrentRoom();
        }

        private void roomStatusToolItem_Click(object sender, EventArgs e)
        {
            CheckCurrentRoom();
        }

        private void historyMenuItem_Click(object sender, EventArgs e)
        {
            History();
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            Settings();
        }

        private void historyToolItem_Click(object sender, EventArgs e)
        {
            History();
        }

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            Help();
        }

        private void helpToolItem_Click(object sender, EventArgs e)
        {
            Help();
        }

        private void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            CheckVersion(null, true);
        }

        private void UsersClientStatus()
        {
            string currentUser = CurrentUser;

            if (currentUser != "")
                SendMessage("[REPORTVER " + currentUser + "]", 0, "");
        }

        private void eeeServiceController_Connected(object sender, BackgroundServiceController.ConnectedEventArgs e)
        {
            this.Text = "Eee - " + this.eeeServiceController.CurrentUser.Login;
        }

        private void eeeServiceController_LoginFailed(object sender, BackgroundServiceController.LoginFailedEventArgs e)
        {
            MessageBox.Show(this, "Login / password combination is wrong.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Login(false);
        }




        /*private void messages_Resize(object sender, System.EventArgs e)
        {
            if (this.messages.Height != 0)
                this.messagesHeight = this.messages.Height;
        }*/

    }
}



