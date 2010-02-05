using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace KolikSoftware.Eee.Client
{
    public partial class MainForm
    {
        #region Controls

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ImageList userImages;
        private System.Windows.Forms.ImageList roomImages;
        private System.Windows.Forms.Timer activationCheckerTimer;
        private System.Windows.Forms.Splitter splitter1;
        private MenuStrip mainMenu;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem connectMenuItem;
        private ToolStripMenuItem accountMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem toolsMenu;
        private ToolStripMenuItem replyMenuItem;
        private ToolStripMenuItem awayModeMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem optionsMenuItem;
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem helpMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem aboutMenuItem;
        private ToolStripContainer toolbarContainer;
        private ToolStrip actionsToolStrip;
        private ToolStripButton replyToolItem;
        private ToolStripButton awayModeToolItem;
        private StatusStrip statusStrip;
        private SplitContainer splitContainer1;
        private Panel mainBottomPanel;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            
            this.notificationManager.NormalIcon = this.Icon;
            this.notificationManager.AwayIcon = Properties.Resources.AwayModeIco;
            this.notificationManager.MessageIcon = Properties.Resources.NotificationIco;

            /// Settings Bindings, must be added manually due to ms bug.
            this.notificationManager.DataBindings.Add(new Binding("IconNotificationType", Properties.Settings.Default, "IconNotificationType", false, DataSourceUpdateMode.OnPropertyChanged));
            this.notificationManager.DataBindings.Add(new Binding("AllowNotifications", Properties.Settings.Default, "ShowNotifications", false, DataSourceUpdateMode.OnPropertyChanged));
            this.notificationManager.DataBindings.Add(new Binding("ConnectionBlinking", Properties.Settings.Default, "BlinkConnected", false, DataSourceUpdateMode.OnPropertyChanged));
            this.notificationManager.DataBindings.Add(new Binding("ConnectionNotifications", Properties.Settings.Default, "NotifyConnected", false, DataSourceUpdateMode.OnPropertyChanged));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (this.rooms != null)
                    this.rooms.Dispose();

                if (this.smilieStreams != null)
                {
                    foreach (MemoryStream smilieStream in this.smilieStreams)
                    {
                        smilieStream.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.roomImages = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.notificationsNotifyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.awayModeNotifyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.helpNotifyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitNotifyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userImages = new System.Windows.Forms.ImageList(this.components);
            this.activationCheckerTimer = new System.Windows.Forms.Timer(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.connectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.showMediaBarTooltem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.replyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.followMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.awayModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.feedbackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarContainer = new System.Windows.Forms.ToolStripContainer();
            this.roomsToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolbarCustomizeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleStretchItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.messagesToSendStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.errorLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectionProblemsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.downloadingLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.updatesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.uploadingLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.disconnectedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chatBrowser = new System.Windows.Forms.WebBrowser();
            this.browserMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadMediaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.externalUsersToolStrip = new System.Windows.Forms.ToolStrip();
            this.mediaToolStrip = new System.Windows.Forms.ToolStrip();
            this.closeMediaBarToolItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.playToolItem2 = new System.Windows.Forms.ToolStripButton();
            this.pauseToolItem2 = new System.Windows.Forms.ToolStripButton();
            this.stopToolItem2 = new System.Windows.Forms.ToolStripButton();
            this.backToolItem = new System.Windows.Forms.ToolStripButton();
            this.forwardToolItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.volumeUpToolItem = new System.Windows.Forms.ToolStripButton();
            this.volumeDownToolItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.mediaLabel = new System.Windows.Forms.ToolStripLabel();
            this.mainBottomPanel = new System.Windows.Forms.Panel();
            this.text = new System.Windows.Forms.RichTextBox();
            this.actionsToolStrip = new System.Windows.Forms.ToolStrip();
            this.replyToolItem = new System.Windows.Forms.ToolStripButton();
            this.followToolItem = new System.Windows.Forms.ToolStripButton();
            this.awayModeToolItem = new System.Windows.Forms.ToolStripButton();
            this.uploadToolItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.dateLabel = new System.Windows.Forms.ToolStripLabel();
            this.historyDayLeftToolItem = new System.Windows.Forms.ToolStripButton();
            this.historyDayRightToolItem = new System.Windows.Forms.ToolStripButton();
            this.historyMonthLeftToolItem = new System.Windows.Forms.ToolStripButton();
            this.historyMonthRightToolItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.searchLabel = new System.Windows.Forms.ToolStripLabel();
            this.searchBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.playToolItem = new System.Windows.Forms.ToolStripButton();
            this.pauseToolItem = new System.Windows.Forms.ToolStripButton();
            this.stopToolItem = new System.Windows.Forms.ToolStripButton();
            this.usersToolStrip = new System.Windows.Forms.ToolStrip();
            this.messageCheckerTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.userMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ignoreUserItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopIgnoringUserItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.ignoreAllUsersButThisItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopIgnoringAllUsersItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roomMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ignoreRoomItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopIgnoringRoomItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.ignoreAllRoomsButThisItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopIgnoringAllRoomsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userInfoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.eeeServiceController = new KolikSoftware.Eee.Client.BackgroundServiceController(this.components);
            this.activatingHotkey = new KolikSoftware.Eee.Client.Helpers.GlobalHotKey(this.components);
            this.notificationManager = new KolikSoftware.Eee.Client.Notifications.NotificationManager(this.components);
            this.historyManager = new KolikSoftware.Eee.Client.History.HistoryManager(this.components);
            this.loginManager = new KolikSoftware.Eee.Client.LoginProcess.LoginManager(this.components);
            this.autoAwayMonitor = new KolikSoftware.Eee.Client.Notifications.AutoAwayMonitor(this.components);
            this.updateManager = new KolikSoftware.Eee.Client.Updating.UpdateManager(this.components);
            this.mediaPlayer = new KolikSoftware.Eee.Client.Media.MediaPlayer(this.components);
            this.linkResolver = new KolikSoftware.Eee.Client.LinkResolver(this.components);
            this.notifyMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.toolbarContainer.BottomToolStripPanel.SuspendLayout();
            this.toolbarContainer.ContentPanel.SuspendLayout();
            this.toolbarContainer.TopToolStripPanel.SuspendLayout();
            this.toolbarContainer.SuspendLayout();
            this.toolbarCustomizeMenu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.browserMenu.SuspendLayout();
            this.mediaToolStrip.SuspendLayout();
            this.mainBottomPanel.SuspendLayout();
            this.actionsToolStrip.SuspendLayout();
            this.userMenu.SuspendLayout();
            this.roomMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // roomImages
            // 
            this.roomImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("roomImages.ImageStream")));
            this.roomImages.TransparentColor = System.Drawing.Color.Transparent;
            this.roomImages.Images.SetKeyName(0, "CtrlF1.png");
            this.roomImages.Images.SetKeyName(1, "CtrlF2.png");
            this.roomImages.Images.SetKeyName(2, "CtrlF3.png");
            this.roomImages.Images.SetKeyName(3, "CtrlF4.png");
            this.roomImages.Images.SetKeyName(4, "CtrlF5.png");
            this.roomImages.Images.SetKeyName(5, "CtrlF6.png");
            this.roomImages.Images.SetKeyName(6, "CtrlF7.png");
            this.roomImages.Images.SetKeyName(7, "CtrlF8.png");
            this.roomImages.Images.SetKeyName(8, "CtrlF9.png");
            this.roomImages.Images.SetKeyName(9, "CtrlF10.png");
            this.roomImages.Images.SetKeyName(10, "CtrlF11.png");
            this.roomImages.Images.SetKeyName(11, "CtrlF12.png");
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.ContextMenuStrip = this.notifyMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "(EeeClient)";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseMove);
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDown);
            // 
            // notifyMenu
            // 
            this.notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notificationsNotifyItem,
            this.awayModeNotifyItem,
            this.toolStripSeparator5,
            this.helpNotifyItem,
            this.toolStripSeparator6,
            this.exitNotifyItem});
            this.notifyMenu.Name = "notifyMenu";
            this.notifyMenu.Size = new System.Drawing.Size(167, 104);
            // 
            // notificationsNotifyItem
            // 
            this.notificationsNotifyItem.Checked = true;
            this.notificationsNotifyItem.CheckOnClick = true;
            this.notificationsNotifyItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.notificationsNotifyItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Notification;
            this.notificationsNotifyItem.Name = "notificationsNotifyItem";
            this.notificationsNotifyItem.Size = new System.Drawing.Size(166, 22);
            this.notificationsNotifyItem.Text = "&Notifications";
            this.notificationsNotifyItem.CheckedChanged += new System.EventHandler(this.notificationsNotifyItem_CheckedChanged);
            // 
            // awayModeNotifyItem
            // 
            this.awayModeNotifyItem.CheckOnClick = true;
            this.awayModeNotifyItem.Image = ((System.Drawing.Image)(resources.GetObject("awayModeNotifyItem.Image")));
            this.awayModeNotifyItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.awayModeNotifyItem.Name = "awayModeNotifyItem";
            this.awayModeNotifyItem.Size = new System.Drawing.Size(166, 22);
            this.awayModeNotifyItem.Text = "&Away Mode";
            this.awayModeNotifyItem.CheckedChanged += new System.EventHandler(this.awayModeItem_CheckedChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // helpNotifyItem
            // 
            this.helpNotifyItem.Image = ((System.Drawing.Image)(resources.GetObject("helpNotifyItem.Image")));
            this.helpNotifyItem.Name = "helpNotifyItem";
            this.helpNotifyItem.Size = new System.Drawing.Size(166, 22);
            this.helpNotifyItem.Text = "&Eee Client Online";
            this.helpNotifyItem.Click += new System.EventHandler(this.helpNotifyItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(163, 6);
            // 
            // exitNotifyItem
            // 
            this.exitNotifyItem.Name = "exitNotifyItem";
            this.exitNotifyItem.Size = new System.Drawing.Size(166, 22);
            this.exitNotifyItem.Text = "E&xit";
            this.exitNotifyItem.Click += new System.EventHandler(this.exitNotifyItem_Click);
            // 
            // userImages
            // 
            this.userImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("userImages.ImageStream")));
            this.userImages.TransparentColor = System.Drawing.Color.Transparent;
            this.userImages.Images.SetKeyName(0, "AltF1.png");
            this.userImages.Images.SetKeyName(1, "AltF2.png");
            this.userImages.Images.SetKeyName(2, "AltF3.png");
            this.userImages.Images.SetKeyName(3, "AltF4.png");
            this.userImages.Images.SetKeyName(4, "AltF5.png");
            this.userImages.Images.SetKeyName(5, "AltF6.png");
            this.userImages.Images.SetKeyName(6, "AltF7.png");
            this.userImages.Images.SetKeyName(7, "AltF8.png");
            this.userImages.Images.SetKeyName(8, "AltF9.png");
            this.userImages.Images.SetKeyName(9, "AltF10.png");
            this.userImages.Images.SetKeyName(10, "AltF11.png");
            this.userImages.Images.SetKeyName(11, "AltF12.png");
            // 
            // activationCheckerTimer
            // 
            this.activationCheckerTimer.Enabled = true;
            this.activationCheckerTimer.Interval = 500;
            this.activationCheckerTimer.Tick += new System.EventHandler(this.activationCheckerTimer_Tick);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 598);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(592, 3);
            this.splitter1.TabIndex = 51;
            this.splitter1.TabStop = false;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.viewMenu,
            this.toolsMenu,
            this.helpMenu});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(592, 24);
            this.mainMenu.TabIndex = 0;
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectMenuItem,
            this.accountMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(35, 20);
            this.fileMenu.Text = "&File";
            // 
            // connectMenuItem
            // 
            this.connectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("connectMenuItem.Image")));
            this.connectMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.connectMenuItem.Name = "connectMenuItem";
            this.connectMenuItem.Size = new System.Drawing.Size(161, 22);
            this.connectMenuItem.Text = "&Connect...";
            this.connectMenuItem.Click += new System.EventHandler(this.connectItem_Click);
            // 
            // accountMenuItem
            // 
            this.accountMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.NewAccount;
            this.accountMenuItem.Name = "accountMenuItem";
            this.accountMenuItem.Size = new System.Drawing.Size(161, 22);
            this.accountMenuItem.Text = "&User Account...";
            this.accountMenuItem.Click += new System.EventHandler(this.accountItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(161, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMediaBarTooltem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(41, 20);
            this.viewMenu.Text = "&View";
            // 
            // showMediaBarTooltem
            // 
            this.showMediaBarTooltem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Media;
            this.showMediaBarTooltem.Name = "showMediaBarTooltem";
            this.showMediaBarTooltem.Size = new System.Drawing.Size(132, 22);
            this.showMediaBarTooltem.Text = "&Media Bar";
            this.showMediaBarTooltem.Click += new System.EventHandler(this.showMediaBarTooltem_Click);
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replyMenuItem,
            this.followMenuItem,
            this.toolStripSeparator3,
            this.awayModeMenuItem,
            this.notificationsMenuItem,
            this.downloadsMenuItem,
            this.toolStripSeparator2,
            this.optionsMenuItem});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(44, 20);
            this.toolsMenu.Text = "&Tools";
            // 
            // replyMenuItem
            // 
            this.replyMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("replyMenuItem.Image")));
            this.replyMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.replyMenuItem.Name = "replyMenuItem";
            this.replyMenuItem.Size = new System.Drawing.Size(144, 22);
            this.replyMenuItem.Text = "&Reply";
            this.replyMenuItem.Click += new System.EventHandler(this.replyMenuItem_Click);
            // 
            // followMenuItem
            // 
            this.followMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Follow;
            this.followMenuItem.Name = "followMenuItem";
            this.followMenuItem.Size = new System.Drawing.Size(144, 22);
            this.followMenuItem.Text = "&Follow";
            this.followMenuItem.Click += new System.EventHandler(this.followMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // awayModeMenuItem
            // 
            this.awayModeMenuItem.CheckOnClick = true;
            this.awayModeMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("awayModeMenuItem.Image")));
            this.awayModeMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.awayModeMenuItem.Name = "awayModeMenuItem";
            this.awayModeMenuItem.Size = new System.Drawing.Size(144, 22);
            this.awayModeMenuItem.Text = "&Away Mode";
            this.awayModeMenuItem.CheckedChanged += new System.EventHandler(this.awayModeItem_CheckedChanged);
            // 
            // notificationsMenuItem
            // 
            this.notificationsMenuItem.Checked = true;
            this.notificationsMenuItem.CheckOnClick = true;
            this.notificationsMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.notificationsMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Notification;
            this.notificationsMenuItem.Name = "notificationsMenuItem";
            this.notificationsMenuItem.Size = new System.Drawing.Size(144, 22);
            this.notificationsMenuItem.Text = "&Notifications";
            this.notificationsMenuItem.CheckedChanged += new System.EventHandler(this.notificationsMenuItem_CheckedChanged);
            // 
            // downloadsMenuItem
            // 
            this.downloadsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderMenuItem,
            this.removeAllMenuItem});
            this.downloadsMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Download;
            this.downloadsMenuItem.Name = "downloadsMenuItem";
            this.downloadsMenuItem.Size = new System.Drawing.Size(144, 22);
            this.downloadsMenuItem.Text = "&Downloads";
            // 
            // openFolderMenuItem
            // 
            this.openFolderMenuItem.Name = "openFolderMenuItem";
            this.openFolderMenuItem.Size = new System.Drawing.Size(144, 22);
            this.openFolderMenuItem.Text = "&Open Folder";
            this.openFolderMenuItem.Click += new System.EventHandler(this.openFolderMenuItem_Click);
            // 
            // removeAllMenuItem
            // 
            this.removeAllMenuItem.Name = "removeAllMenuItem";
            this.removeAllMenuItem.Size = new System.Drawing.Size(144, 22);
            this.removeAllMenuItem.Text = "&Remove All";
            this.removeAllMenuItem.Click += new System.EventHandler(this.removeAllMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(141, 6);
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsMenuItem.Image")));
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(144, 22);
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpMenuItem,
            this.toolStripSeparator4,
            this.feedbackMenuItem,
            this.toolStripSeparator11,
            this.aboutMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(40, 20);
            this.helpMenu.Text = "&Help";
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpMenuItem.Image")));
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(177, 22);
            this.helpMenuItem.Text = "&Eee Client Online";
            this.helpMenuItem.Click += new System.EventHandler(this.helpItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            // 
            // feedbackMenuItem
            // 
            this.feedbackMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Feedback;
            this.feedbackMenuItem.Name = "feedbackMenuItem";
            this.feedbackMenuItem.Size = new System.Drawing.Size(177, 22);
            this.feedbackMenuItem.Text = "&Feedback...";
            this.feedbackMenuItem.Click += new System.EventHandler(this.feedbackItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(174, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(177, 22);
            this.aboutMenuItem.Text = "&About Eee Client...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // toolbarContainer
            // 
            // 
            // toolbarContainer.BottomToolStripPanel
            // 
            this.toolbarContainer.BottomToolStripPanel.Controls.Add(this.roomsToolStrip);
            this.toolbarContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolbarContainer.ContentPanel
            // 
            this.toolbarContainer.ContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolbarContainer.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolbarContainer.ContentPanel.Size = new System.Drawing.Size(592, 477);
            this.toolbarContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarContainer.Location = new System.Drawing.Point(0, 24);
            this.toolbarContainer.Name = "toolbarContainer";
            this.toolbarContainer.Size = new System.Drawing.Size(592, 574);
            this.toolbarContainer.TabIndex = 2;
            this.toolbarContainer.Text = "toolStripContainer1";
            // 
            // toolbarContainer.TopToolStripPanel
            // 
            this.toolbarContainer.TopToolStripPanel.Controls.Add(this.actionsToolStrip);
            this.toolbarContainer.TopToolStripPanel.Controls.Add(this.usersToolStrip);
            // 
            // roomsToolStrip
            // 
            this.roomsToolStrip.ContextMenuStrip = this.toolbarCustomizeMenu;
            this.roomsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.roomsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.roomsToolStrip.Name = "roomsToolStrip";
            this.roomsToolStrip.ShowItemToolTips = false;
            this.roomsToolStrip.Size = new System.Drawing.Size(592, 25);
            this.roomsToolStrip.Stretch = true;
            this.roomsToolStrip.TabIndex = 0;
            this.roomsToolStrip.Resize += new System.EventHandler(this.roomsToolStrip_Resize);
            // 
            // toolbarCustomizeMenu
            // 
            this.toolbarCustomizeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleStretchItem});
            this.toolbarCustomizeMenu.Name = "toolbarCustomizeMenu";
            this.toolbarCustomizeMenu.Size = new System.Drawing.Size(156, 26);
            // 
            // toggleStretchItem
            // 
            this.toggleStretchItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.StretchToggle;
            this.toggleStretchItem.Name = "toggleStretchItem";
            this.toggleStretchItem.Size = new System.Drawing.Size(155, 22);
            this.toggleStretchItem.Text = "Toggle Stretch";
            this.toggleStretchItem.Click += new System.EventHandler(this.toggleStretchItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.messagesToSendStatus,
            this.errorLabel,
            this.connectionProblemsLabel,
            this.downloadingLabel,
            this.updatesLabel,
            this.uploadingLabel,
            this.disconnectedLabel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 25);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(592, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(60, 17);
            this.statusLabel.Text = "(EeeClient)";
            // 
            // messagesToSendStatus
            // 
            this.messagesToSendStatus.Name = "messagesToSendStatus";
            this.messagesToSendStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // errorLabel
            // 
            this.errorLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.errorLabel.Image = ((System.Drawing.Image)(resources.GetObject("errorLabel.Image")));
            this.errorLabel.IsLink = true;
            this.errorLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.errorLabel.LinkColor = System.Drawing.Color.Red;
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(47, 17);
            this.errorLabel.Text = "Error";
            this.errorLabel.ToolTipText = "Click to show details";
            this.errorLabel.Visible = false;
            this.errorLabel.VisitedLinkColor = System.Drawing.Color.Red;
            this.errorLabel.Click += new System.EventHandler(this.errorLabel_Click);
            // 
            // connectionProblemsLabel
            // 
            this.connectionProblemsLabel.ActiveLinkColor = System.Drawing.Color.NavajoWhite;
            this.connectionProblemsLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.connectionProblemsLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.ConnectionProblems;
            this.connectionProblemsLabel.IsLink = true;
            this.connectionProblemsLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.connectionProblemsLabel.LinkColor = System.Drawing.Color.Goldenrod;
            this.connectionProblemsLabel.Name = "connectionProblemsLabel";
            this.connectionProblemsLabel.Size = new System.Drawing.Size(123, 17);
            this.connectionProblemsLabel.Text = "Connection Problems";
            this.connectionProblemsLabel.ToolTipText = "Click to show details";
            this.connectionProblemsLabel.Visible = false;
            this.connectionProblemsLabel.VisitedLinkColor = System.Drawing.Color.Goldenrod;
            this.connectionProblemsLabel.Click += new System.EventHandler(this.connectionProblemsLabel_Click);
            // 
            // downloadingLabel
            // 
            this.downloadingLabel.ActiveLinkColor = System.Drawing.Color.DarkSlateGray;
            this.downloadingLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.downloadingLabel.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.downloadingLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Download;
            this.downloadingLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.downloadingLabel.LinkColor = System.Drawing.Color.DarkSlateGray;
            this.downloadingLabel.Name = "downloadingLabel";
            this.downloadingLabel.Size = new System.Drawing.Size(106, 17);
            this.downloadingLabel.Text = "Downloading files";
            this.downloadingLabel.Visible = false;
            this.downloadingLabel.Click += new System.EventHandler(this.downloadingLabel_Click);
            // 
            // updatesLabel
            // 
            this.updatesLabel.ActiveLinkColor = System.Drawing.Color.SteelBlue;
            this.updatesLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.updatesLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Updates;
            this.updatesLabel.IsLink = true;
            this.updatesLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.updatesLabel.LinkColor = System.Drawing.Color.SteelBlue;
            this.updatesLabel.Name = "updatesLabel";
            this.updatesLabel.Size = new System.Drawing.Size(109, 17);
            this.updatesLabel.Text = "Updates Available";
            this.updatesLabel.ToolTipText = "Click to show details";
            this.updatesLabel.Visible = false;
            this.updatesLabel.VisitedLinkColor = System.Drawing.Color.SteelBlue;
            this.updatesLabel.Click += new System.EventHandler(this.updatesAvailableLabel_Click);
            // 
            // uploadingLabel
            // 
            this.uploadingLabel.ActiveLinkColor = System.Drawing.Color.SteelBlue;
            this.uploadingLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.uploadingLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.uploadingLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Upload;
            this.uploadingLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.uploadingLabel.LinkColor = System.Drawing.Color.RoyalBlue;
            this.uploadingLabel.Name = "uploadingLabel";
            this.uploadingLabel.Size = new System.Drawing.Size(92, 17);
            this.uploadingLabel.Text = "Uploading files";
            this.uploadingLabel.Visible = false;
            this.uploadingLabel.VisitedLinkColor = System.Drawing.Color.SteelBlue;
            this.uploadingLabel.Click += new System.EventHandler(this.uploadingLabel_Click);
            // 
            // disconnectedLabel
            // 
            this.disconnectedLabel.ActiveLinkColor = System.Drawing.Color.Linen;
            this.disconnectedLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.disconnectedLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Connect;
            this.disconnectedLabel.IsLink = true;
            this.disconnectedLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.disconnectedLabel.LinkColor = System.Drawing.Color.DimGray;
            this.disconnectedLabel.Name = "disconnectedLabel";
            this.disconnectedLabel.Size = new System.Drawing.Size(87, 17);
            this.disconnectedLabel.Text = "Disconnected";
            this.disconnectedLabel.ToolTipText = "Click to show details";
            this.disconnectedLabel.Visible = false;
            this.disconnectedLabel.VisitedLinkColor = System.Drawing.Color.Purple;
            this.disconnectedLabel.Click += new System.EventHandler(this.disconnectedLabel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = ThemeBackColor;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chatBrowser);
            this.splitContainer1.Panel1.Controls.Add(this.externalUsersToolStrip);
            this.splitContainer1.Panel1.Controls.Add(this.mediaToolStrip);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = ThemeBackColor;
            this.splitContainer1.Panel2.Controls.Add(this.mainBottomPanel);
            this.splitContainer1.Panel2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Size = new System.Drawing.Size(590, 475);
            this.splitContainer1.SplitterDistance = 399;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // chatBrowser
            // 
            this.chatBrowser.AllowWebBrowserDrop = false;
            this.chatBrowser.ContextMenuStrip = this.browserMenu;
            this.chatBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatBrowser.IsWebBrowserContextMenuEnabled = false;
            this.chatBrowser.Location = new System.Drawing.Point(0, 0);
            this.chatBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.chatBrowser.Name = "chatBrowser";
            this.chatBrowser.Size = new System.Drawing.Size(590, 399);
            this.chatBrowser.TabIndex = 8;
            this.chatBrowser.Url = new System.Uri("about:blank", System.UriKind.Absolute);
            this.chatBrowser.WebBrowserShortcutsEnabled = false;
            this.chatBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.chatBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chatBrowser_PreviewKeyDown);
            this.chatBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.chatBrowser_DocumentCompleted);
            // 
            // browserMenu
            // 
            this.browserMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyMenuItem,
            this.toolStripSeparator17,
            this.uploadMediaMenuItem});
            this.browserMenu.Name = "browserMenu";
            this.browserMenu.Size = new System.Drawing.Size(162, 54);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Copy;
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyMenuItem.Size = new System.Drawing.Size(161, 22);
            this.copyMenuItem.Text = "&Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(158, 6);
            // 
            // uploadMediaMenuItem
            // 
            this.uploadMediaMenuItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Upload;
            this.uploadMediaMenuItem.Name = "uploadMediaMenuItem";
            this.uploadMediaMenuItem.Size = new System.Drawing.Size(161, 22);
            this.uploadMediaMenuItem.Text = "&Upload Media...";
            this.uploadMediaMenuItem.Click += new System.EventHandler(this.uploadMediaMenuItem_Click);
            // 
            // externalUsersToolStrip
            // 
            this.externalUsersToolStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.externalUsersToolStrip.Location = new System.Drawing.Point(0, 0);
            this.externalUsersToolStrip.Name = "externalUsersToolStrip";
            this.externalUsersToolStrip.Size = new System.Drawing.Size(32, 399);
            this.externalUsersToolStrip.TabIndex = 7;
            this.externalUsersToolStrip.Text = "toolStrip1";
            this.externalUsersToolStrip.Visible = false;
            // 
            // mediaToolStrip
            // 
            this.mediaToolStrip.Dock = System.Windows.Forms.DockStyle.Right;
            this.mediaToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mediaToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeMediaBarToolItem,
            this.toolStripSeparator15,
            this.playToolItem2,
            this.pauseToolItem2,
            this.stopToolItem2,
            this.backToolItem,
            this.forwardToolItem,
            this.toolStripSeparator16,
            this.volumeUpToolItem,
            this.volumeDownToolItem,
            this.toolStripSeparator13,
            this.mediaLabel});
            this.mediaToolStrip.Location = new System.Drawing.Point(558, 0);
            this.mediaToolStrip.Name = "mediaToolStrip";
            this.mediaToolStrip.Size = new System.Drawing.Size(32, 399);
            this.mediaToolStrip.TabIndex = 5;
            this.mediaToolStrip.Text = "Media Bar";
            this.mediaToolStrip.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            this.mediaToolStrip.Visible = false;
            this.mediaToolStrip.Resize += new System.EventHandler(this.mediaToolStrip_Resize);
            // 
            // closeMediaBarToolItem
            // 
            this.closeMediaBarToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.closeMediaBarToolItem.Image = ((System.Drawing.Image)(resources.GetObject("closeMediaBarToolItem.Image")));
            this.closeMediaBarToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeMediaBarToolItem.Name = "closeMediaBarToolItem";
            this.closeMediaBarToolItem.Size = new System.Drawing.Size(21, 37);
            this.closeMediaBarToolItem.Text = "Close";
            this.closeMediaBarToolItem.Click += new System.EventHandler(this.closeMediaBarToolItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(21, 6);
            this.toolStripSeparator15.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // playToolItem2
            // 
            this.playToolItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playToolItem2.Enabled = false;
            this.playToolItem2.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Play;
            this.playToolItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playToolItem2.Name = "playToolItem2";
            this.playToolItem2.Size = new System.Drawing.Size(21, 20);
            this.playToolItem2.Text = "Play";
            this.playToolItem2.Click += new System.EventHandler(this.playToolItem_Click);
            // 
            // pauseToolItem2
            // 
            this.pauseToolItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseToolItem2.Enabled = false;
            this.pauseToolItem2.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Pause;
            this.pauseToolItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseToolItem2.Name = "pauseToolItem2";
            this.pauseToolItem2.Size = new System.Drawing.Size(21, 20);
            this.pauseToolItem2.Text = "Pause";
            this.pauseToolItem2.Click += new System.EventHandler(this.pauseToolItem_Click);
            // 
            // stopToolItem2
            // 
            this.stopToolItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopToolItem2.Enabled = false;
            this.stopToolItem2.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Stop;
            this.stopToolItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopToolItem2.Name = "stopToolItem2";
            this.stopToolItem2.Size = new System.Drawing.Size(21, 20);
            this.stopToolItem2.Text = "Stop";
            this.stopToolItem2.Click += new System.EventHandler(this.stopToolItem_Click);
            // 
            // backToolItem
            // 
            this.backToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backToolItem.Enabled = false;
            this.backToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Back;
            this.backToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backToolItem.Name = "backToolItem";
            this.backToolItem.Size = new System.Drawing.Size(21, 20);
            this.backToolItem.Text = "Previous";
            this.backToolItem.Click += new System.EventHandler(this.backToolItem_Click);
            // 
            // forwardToolItem
            // 
            this.forwardToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardToolItem.Enabled = false;
            this.forwardToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Forward;
            this.forwardToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardToolItem.Name = "forwardToolItem";
            this.forwardToolItem.Size = new System.Drawing.Size(21, 20);
            this.forwardToolItem.Text = "Next";
            this.forwardToolItem.Click += new System.EventHandler(this.forwardToolItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(21, 6);
            this.toolStripSeparator16.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // volumeUpToolItem
            // 
            this.volumeUpToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.volumeUpToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.VolumeUp;
            this.volumeUpToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.volumeUpToolItem.Name = "volumeUpToolItem";
            this.volumeUpToolItem.Size = new System.Drawing.Size(21, 20);
            this.volumeUpToolItem.Text = "Volume Up";
            this.volumeUpToolItem.Click += new System.EventHandler(this.volumeUpToolItem_Click);
            // 
            // volumeDownToolItem
            // 
            this.volumeDownToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.volumeDownToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.VolumeDown;
            this.volumeDownToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.volumeDownToolItem.Name = "volumeDownToolItem";
            this.volumeDownToolItem.Size = new System.Drawing.Size(21, 20);
            this.volumeDownToolItem.Text = "Volume Down";
            this.volumeDownToolItem.Click += new System.EventHandler(this.volumeDownToolItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(21, 6);
            this.toolStripSeparator13.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // mediaLabel
            // 
            this.mediaLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mediaLabel.Name = "mediaLabel";
            this.mediaLabel.Size = new System.Drawing.Size(21, 51);
            this.mediaLabel.Text = "No Media";
            this.mediaLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mainBottomPanel
            // 
            this.mainBottomPanel.Controls.Add(this.text);
            this.mainBottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainBottomPanel.Location = new System.Drawing.Point(4, 4);
            this.mainBottomPanel.Name = "mainBottomPanel";
            this.mainBottomPanel.Size = new System.Drawing.Size(582, 66);
            this.mainBottomPanel.TabIndex = 60;
            // 
            // text
            // 
            this.text.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text.Location = new System.Drawing.Point(0, 0);
            this.text.Multiline = true;
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(582, 66);
            this.text.TabIndex = 0;
            this.text.TextChanged += new System.EventHandler(this.text_TextChanged);
            this.text.DragDrop += new System.Windows.Forms.DragEventHandler(this.text_DragDrop);
            this.text.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
            this.text.DragEnter += new System.Windows.Forms.DragEventHandler(this.text_DragEnter);
            // 
            // actionsToolStrip
            // 
            this.actionsToolStrip.ContextMenuStrip = this.toolbarCustomizeMenu;
            this.actionsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.actionsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replyToolItem,
            this.followToolItem,
            this.awayModeToolItem,
            this.uploadToolItem,
            this.toolStripSeparator10,
            this.dateLabel,
            this.historyDayLeftToolItem,
            this.historyDayRightToolItem,
            this.historyMonthLeftToolItem,
            this.historyMonthRightToolItem,
            this.toolStripSeparator14,
            this.searchLabel,
            this.searchBox,
            this.toolStripSeparator12,
            this.playToolItem,
            this.pauseToolItem,
            this.stopToolItem});
            this.actionsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.actionsToolStrip.Name = "actionsToolStrip";
            this.actionsToolStrip.Size = new System.Drawing.Size(592, 25);
            this.actionsToolStrip.Stretch = true;
            this.actionsToolStrip.TabIndex = 0;
            // 
            // replyToolItem
            // 
            this.replyToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replyToolItem.Image = ((System.Drawing.Image)(resources.GetObject("replyToolItem.Image")));
            this.replyToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replyToolItem.MergeIndex = 20;
            this.replyToolItem.Name = "replyToolItem";
            this.replyToolItem.Size = new System.Drawing.Size(23, 22);
            this.replyToolItem.Text = "Reply to last user";
            this.replyToolItem.Click += new System.EventHandler(this.replyToolItem_Click);
            // 
            // followToolItem
            // 
            this.followToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.followToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Follow;
            this.followToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.followToolItem.MergeIndex = 30;
            this.followToolItem.Name = "followToolItem";
            this.followToolItem.Size = new System.Drawing.Size(23, 22);
            this.followToolItem.Text = "Follow last user";
            this.followToolItem.Click += new System.EventHandler(this.followToolItem_Click);
            // 
            // awayModeToolItem
            // 
            this.awayModeToolItem.CheckOnClick = true;
            this.awayModeToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.awayModeToolItem.Image = ((System.Drawing.Image)(resources.GetObject("awayModeToolItem.Image")));
            this.awayModeToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.awayModeToolItem.MergeIndex = 40;
            this.awayModeToolItem.Name = "awayModeToolItem";
            this.awayModeToolItem.Size = new System.Drawing.Size(23, 22);
            this.awayModeToolItem.Text = "Away Mode";
            this.awayModeToolItem.CheckedChanged += new System.EventHandler(this.awayModeItem_CheckedChanged);
            // 
            // uploadToolItem
            // 
            this.uploadToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Upload;
            this.uploadToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uploadToolItem.MergeIndex = 50;
            this.uploadToolItem.Name = "uploadToolItem";
            this.uploadToolItem.Size = new System.Drawing.Size(23, 22);
            this.uploadToolItem.Text = "Upload Media";
            this.uploadToolItem.Click += new System.EventHandler(this.uploadToolItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.MergeIndex = 55;
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = false;
            this.dateLabel.Image = ((System.Drawing.Image)(resources.GetObject("dateLabel.Image")));
            this.dateLabel.MergeIndex = 60;
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(80, 22);
            this.dateLabel.Text = "Today";
            // 
            // historyDayLeftToolItem
            // 
            this.historyDayLeftToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.historyDayLeftToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.HistoryDayLeft;
            this.historyDayLeftToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyDayLeftToolItem.MergeIndex = 70;
            this.historyDayLeftToolItem.Name = "historyDayLeftToolItem";
            this.historyDayLeftToolItem.Size = new System.Drawing.Size(23, 22);
            this.historyDayLeftToolItem.Text = "One Day Back";
            this.historyDayLeftToolItem.Click += new System.EventHandler(this.historyDayLeftToolItem_Click);
            // 
            // historyDayRightToolItem
            // 
            this.historyDayRightToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.historyDayRightToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.HistoryDayRight;
            this.historyDayRightToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyDayRightToolItem.MergeIndex = 80;
            this.historyDayRightToolItem.Name = "historyDayRightToolItem";
            this.historyDayRightToolItem.Size = new System.Drawing.Size(23, 22);
            this.historyDayRightToolItem.Text = "One Day Forward";
            this.historyDayRightToolItem.Click += new System.EventHandler(this.historyDayRightToolItem_Click);
            // 
            // historyMonthLeftToolItem
            // 
            this.historyMonthLeftToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.historyMonthLeftToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.HistoryMonthLeft;
            this.historyMonthLeftToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyMonthLeftToolItem.MergeIndex = 90;
            this.historyMonthLeftToolItem.Name = "historyMonthLeftToolItem";
            this.historyMonthLeftToolItem.Size = new System.Drawing.Size(23, 22);
            this.historyMonthLeftToolItem.Text = "One Month Back";
            this.historyMonthLeftToolItem.Click += new System.EventHandler(this.historyMonthLeftToolItem_Click);
            // 
            // historyMonthRightToolItem
            // 
            this.historyMonthRightToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.historyMonthRightToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.HistoryMonthRight;
            this.historyMonthRightToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyMonthRightToolItem.MergeIndex = 100;
            this.historyMonthRightToolItem.Name = "historyMonthRightToolItem";
            this.historyMonthRightToolItem.Size = new System.Drawing.Size(23, 22);
            this.historyMonthRightToolItem.Text = "One Month Forward";
            this.historyMonthRightToolItem.Click += new System.EventHandler(this.historyMonthRightToolItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // searchLabel
            // 
            this.searchLabel.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Search;
            this.searchLabel.MergeIndex = 110;
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(56, 22);
            this.searchLabel.Text = "Search";
            // 
            // searchBox
            // 
            this.searchBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.searchBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.searchBox.MergeIndex = 120;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(121, 25);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyDown);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // playToolItem
            // 
            this.playToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playToolItem.Enabled = false;
            this.playToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Play;
            this.playToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playToolItem.MergeIndex = 130;
            this.playToolItem.Name = "playToolItem";
            this.playToolItem.Size = new System.Drawing.Size(23, 22);
            this.playToolItem.Text = "Play";
            this.playToolItem.ToolTipText = "Play";
            this.playToolItem.Click += new System.EventHandler(this.playToolItem_Click);
            // 
            // pauseToolItem
            // 
            this.pauseToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseToolItem.Enabled = false;
            this.pauseToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Pause;
            this.pauseToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseToolItem.MergeIndex = 140;
            this.pauseToolItem.Name = "pauseToolItem";
            this.pauseToolItem.Size = new System.Drawing.Size(23, 22);
            this.pauseToolItem.Text = "Pause";
            this.pauseToolItem.Click += new System.EventHandler(this.pauseToolItem_Click);
            // 
            // stopToolItem
            // 
            this.stopToolItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopToolItem.Enabled = false;
            this.stopToolItem.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Stop;
            this.stopToolItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopToolItem.MergeIndex = 150;
            this.stopToolItem.Name = "stopToolItem";
            this.stopToolItem.Size = new System.Drawing.Size(23, 22);
            this.stopToolItem.Text = "Stop";
            this.stopToolItem.Click += new System.EventHandler(this.stopToolItem_Click);
            // 
            // usersToolStrip
            // 
            this.usersToolStrip.ContextMenuStrip = this.toolbarCustomizeMenu;
            this.usersToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.usersToolStrip.Location = new System.Drawing.Point(0, 25);
            this.usersToolStrip.Name = "usersToolStrip";
            this.usersToolStrip.ShowItemToolTips = false;
            this.usersToolStrip.Size = new System.Drawing.Size(592, 25);
            this.usersToolStrip.Stretch = true;
            this.usersToolStrip.TabIndex = 2;
            this.usersToolStrip.Resize += new System.EventHandler(this.usersToolStrip_Resize);
            // 
            // messageCheckerTimer
            // 
            this.messageCheckerTimer.Interval = 30000;
            this.messageCheckerTimer.Tick += new System.EventHandler(this.messageCheckerTimer_Tick);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // userMenu
            // 
            this.userMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreUserItem,
            this.stopIgnoringUserItem,
            this.toolStripSeparator8,
            this.ignoreAllUsersButThisItem,
            this.stopIgnoringAllUsersItem});
            this.userMenu.Name = "userMenu";
            this.userMenu.Size = new System.Drawing.Size(176, 98);
            // 
            // ignoreUserItem
            // 
            this.ignoreUserItem.Image = ((System.Drawing.Image)(resources.GetObject("ignoreUserItem.Image")));
            this.ignoreUserItem.Name = "ignoreUserItem";
            this.ignoreUserItem.Size = new System.Drawing.Size(175, 22);
            this.ignoreUserItem.Text = "&Ignore User";
            this.ignoreUserItem.Click += new System.EventHandler(this.ignoreUserItem_Click);
            // 
            // stopIgnoringUserItem
            // 
            this.stopIgnoringUserItem.Image = ((System.Drawing.Image)(resources.GetObject("stopIgnoringUserItem.Image")));
            this.stopIgnoringUserItem.Name = "stopIgnoringUserItem";
            this.stopIgnoringUserItem.Size = new System.Drawing.Size(175, 22);
            this.stopIgnoringUserItem.Text = "&Stop Ignoring User";
            this.stopIgnoringUserItem.Visible = false;
            this.stopIgnoringUserItem.Click += new System.EventHandler(this.stopIgnoringUserItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(172, 6);
            // 
            // ignoreAllUsersButThisItem
            // 
            this.ignoreAllUsersButThisItem.Name = "ignoreAllUsersButThisItem";
            this.ignoreAllUsersButThisItem.Size = new System.Drawing.Size(175, 22);
            this.ignoreAllUsersButThisItem.Text = "Ignore &All But This";
            this.ignoreAllUsersButThisItem.Click += new System.EventHandler(this.ignoreAllUsersButThisItem_Click);
            // 
            // stopIgnoringAllUsersItem
            // 
            this.stopIgnoringAllUsersItem.Name = "stopIgnoringAllUsersItem";
            this.stopIgnoringAllUsersItem.Size = new System.Drawing.Size(175, 22);
            this.stopIgnoringAllUsersItem.Text = "Stop Ignoring  &All";
            this.stopIgnoringAllUsersItem.Click += new System.EventHandler(this.stopIgnoringAllUsersItem_Click);
            // 
            // roomMenu
            // 
            this.roomMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreRoomItem,
            this.stopIgnoringRoomItem,
            this.toolStripSeparator9,
            this.ignoreAllRoomsButThisItem,
            this.stopIgnoringAllRoomsItem});
            this.roomMenu.Name = "roomMenu";
            this.roomMenu.Size = new System.Drawing.Size(181, 98);
            // 
            // ignoreRoomItem
            // 
            this.ignoreRoomItem.Image = ((System.Drawing.Image)(resources.GetObject("ignoreRoomItem.Image")));
            this.ignoreRoomItem.Name = "ignoreRoomItem";
            this.ignoreRoomItem.Size = new System.Drawing.Size(180, 22);
            this.ignoreRoomItem.Text = "&Ignore Room";
            this.ignoreRoomItem.Click += new System.EventHandler(this.ignoreRoomItem_Click);
            // 
            // stopIgnoringRoomItem
            // 
            this.stopIgnoringRoomItem.Image = ((System.Drawing.Image)(resources.GetObject("stopIgnoringRoomItem.Image")));
            this.stopIgnoringRoomItem.Name = "stopIgnoringRoomItem";
            this.stopIgnoringRoomItem.Size = new System.Drawing.Size(180, 22);
            this.stopIgnoringRoomItem.Text = "&Stop Ignoring Room";
            this.stopIgnoringRoomItem.Visible = false;
            this.stopIgnoringRoomItem.Click += new System.EventHandler(this.stopIgnoringRoomItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(177, 6);
            // 
            // ignoreAllRoomsButThisItem
            // 
            this.ignoreAllRoomsButThisItem.Name = "ignoreAllRoomsButThisItem";
            this.ignoreAllRoomsButThisItem.Size = new System.Drawing.Size(180, 22);
            this.ignoreAllRoomsButThisItem.Text = "Ignore &All But This";
            this.ignoreAllRoomsButThisItem.Click += new System.EventHandler(this.ignoreAllRoomsButThisItem_Click);
            // 
            // stopIgnoringAllRoomsItem
            // 
            this.stopIgnoringAllRoomsItem.Name = "stopIgnoringAllRoomsItem";
            this.stopIgnoringAllRoomsItem.Size = new System.Drawing.Size(180, 22);
            this.stopIgnoringAllRoomsItem.Text = "Stop Ignoring &All";
            this.stopIgnoringAllRoomsItem.Click += new System.EventHandler(this.stopIgnoringAllRoomsItem_Click);
            // 
            // eeeServiceController
            // 
            this.eeeServiceController.Service = null;
            this.eeeServiceController.UploadFailed += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.UploadFailedEventArgs>(this.eeeServiceController_UploadFailed);
            this.eeeServiceController.ExternalUserStateChanged += new System.EventHandler<KolikSoftware.Eee.Processor.CommandProcessor.ExternalUserStateChangedEventArgs>(this.eeeServiceController_ExternalUserStateChanged);
            this.eeeServiceController.GetMessagesFinished += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.GetMessagesFinishedEventArgs>(this.eeeServiceController_GetMessagesFinished);
            this.eeeServiceController.GetRoomsFinished += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.GetRoomsFinishedEventArgs>(this.eeeServiceController_GetRoomsFinished);
            this.eeeServiceController.UserStateChanged += new System.EventHandler<KolikSoftware.Eee.Processor.CommandProcessor.UserStateChangedEventArgs>(this.eeeServiceController_UserStateChanged);
            this.eeeServiceController.SucessfulRequest += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.SucessfulRequestEventArgs>(this.eeeServiceController_SucessfulRequest);
            this.eeeServiceController.DownloadFailed += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.DownloadFailedEventArgs>(this.eeeServiceController_DownloadFailed);
            this.eeeServiceController.UploadFinished += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.UploadFinishedEventArgs>(this.eeeServiceController_UploadFinished);
            this.eeeServiceController.GetUsersFinished += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.GetUsersFinishedEventArgs>(this.eeeServiceController_GetUsersFinished);
            this.eeeServiceController.ErrorOccured += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.ErrorOccuredEventArgs>(this.eeeServiceController_ErrorOccured);
            this.eeeServiceController.DownloadFinished += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.DownloadFinishedEventArgs>(this.eeeServiceController_DownloadFinished);
            this.eeeServiceController.UpdatesAvailable += new System.EventHandler<KolikSoftware.Eee.Client.BackgroundServiceController.UpdatesAvailableEventArgs>(this.eeeServiceController_UpdatesAvailable);
            // 
            // activatingHotkey
            // 
            this.activatingHotkey.HotKey = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemtilde)));
            this.activatingHotkey.Pressed += new System.EventHandler<KolikSoftware.Eee.Client.Helpers.GlobalHotKey.PressedEventArgs>(this.activatingHotkey_Pressed);
            this.activatingHotkey.Error += new System.EventHandler<KolikSoftware.Eee.Client.Helpers.GlobalHotKey.ErrorEventArgs>(this.activatingHotkey_Error);
            // 
            // notificationManager
            // 
            this.notificationManager.NotifyIcon = this.notifyIcon;
            this.notificationManager.Activate += new System.EventHandler<KolikSoftware.Eee.Client.Notifications.NotificationManager.ActivateEventArgs>(this.notificationManager_Activate);
            // 
            // historyManager
            // 
            this.historyManager.ServiceController = this.eeeServiceController;
            // 
            // loginManager
            // 
            this.loginManager.ServiceController = this.eeeServiceController;
            this.loginManager.Disconnected += new System.EventHandler<KolikSoftware.Eee.Client.LoginProcess.LoginManager.DisconnectedEventArgs>(this.loginManager_Disconnected);
            this.loginManager.BeforeLogin += new System.EventHandler<KolikSoftware.Eee.Client.LoginProcess.LoginManager.BeforeLoginEventArgs>(this.loginManager_BeforeLogin);
            this.loginManager.InvalidPassword += new System.EventHandler<KolikSoftware.Eee.Client.LoginProcess.LoginManager.InvalidPasswordEventArgs>(this.loginManager_InvalidPassword);
            this.loginManager.AfterLogin += new System.EventHandler<KolikSoftware.Eee.Client.LoginProcess.LoginManager.AfterLoginEventArgs>(this.loginManager_AfterLogin);
            this.loginManager.Connected += new System.EventHandler<KolikSoftware.Eee.Client.LoginProcess.LoginManager.ConnectedEventArgs>(this.loginManager_Connected);
            // 
            // autoAwayMonitor
            // 
            this.autoAwayMonitor.AutoAway += new System.EventHandler<KolikSoftware.Eee.Client.Notifications.AutoAwayMonitor.AutoAwayEventArgs>(this.autoAwayMonitor_AutoAway);
            // 
            // updateManager
            // 
            this.updateManager.ServiceController = this.eeeServiceController;
            this.updateManager.InstallFailed += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.InstallFailedEventArgs>(this.updateManager_InstallFailed);
            this.updateManager.DownloadAllFinished += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.DownloadAllFinishedEventArgs>(this.updateManager_DownloadAllFinished);
            this.updateManager.DownloadFailed += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.DownloadFailedEventArgs>(this.updateManager_DownloadFailed);
            this.updateManager.DownloadStarted += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.DownloadStartedEventArgs>(this.updateManager_DownloadStarted);
            this.updateManager.InstallStarted += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.InstallStartedEventArgs>(this.updateManager_InstallStarted);
            this.updateManager.InstallAllFinished += new System.EventHandler<KolikSoftware.Eee.Client.Updating.UpdateManager.InstallAllFinishedEventArgs>(this.updateManager_InstallAllFinished);
            // 
            // mediaPlayer
            // 
            this.mediaPlayer.ModeChanged += new System.EventHandler<KolikSoftware.Eee.Client.Media.MediaPlayer.ModeChangedEventArgs>(this.mediaPlayer_ModeChanged);
            this.mediaPlayer.MediaChanged += new System.EventHandler<KolikSoftware.Eee.Client.Media.MediaPlayer.MediaChangedEventArgs>(this.mediaPlayer_MediaChanged);
            // 
            // linkResolver
            // 
            this.linkResolver.ProxySettings = null;
            this.linkResolver.LinkResolved += new System.EventHandler<KolikSoftware.Eee.Client.LinkResolverEventArgs>(this.linkResolver_LinkResolved);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(592, 601);
            this.Controls.Add(this.toolbarContainer);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.splitter1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "(EeeClient)";
            this.notifyMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.toolbarContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolbarContainer.BottomToolStripPanel.PerformLayout();
            this.toolbarContainer.ContentPanel.ResumeLayout(false);
            this.toolbarContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolbarContainer.TopToolStripPanel.PerformLayout();
            this.toolbarContainer.ResumeLayout(false);
            this.toolbarContainer.PerformLayout();
            this.toolbarCustomizeMenu.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.browserMenu.ResumeLayout(false);
            this.mediaToolStrip.ResumeLayout(false);
            this.mediaToolStrip.PerformLayout();
            this.mainBottomPanel.ResumeLayout(false);
            this.mainBottomPanel.PerformLayout();
            this.actionsToolStrip.ResumeLayout(false);
            this.actionsToolStrip.PerformLayout();
            this.userMenu.ResumeLayout(false);
            this.roomMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        static readonly MainForm MainFormInstance = new MainForm();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            Application.Run(MainFormInstance);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MainFormInstance.ShowError(new ApplicationException("Unhandled Application Error", e.Exception));
        }

        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel messagesToSendStatus;
        private ToolStripMenuItem notificationsMenuItem;
        private ContextMenuStrip notifyMenu;
        private ToolStripMenuItem notificationsNotifyItem;
        private ToolStripMenuItem awayModeNotifyItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem exitNotifyItem;
        private ToolStripMenuItem helpNotifyItem;
        private ToolStripSeparator toolStripSeparator6;
        private BackgroundServiceController eeeServiceController;
        private ToolStrip usersToolStrip;
        private ToolStrip roomsToolStrip;
        private Timer messageCheckerTimer;
        private KolikSoftware.Eee.Client.Helpers.GlobalHotKey activatingHotkey;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripStatusLabel errorLabel;
        private KolikSoftware.Eee.Client.Notifications.NotificationManager notificationManager;
        private KolikSoftware.Eee.Client.History.HistoryManager historyManager;
        private KolikSoftware.Eee.Client.LoginProcess.LoginManager loginManager;
        private ToolStripStatusLabel connectionProblemsLabel;
        private ContextMenuStrip userMenu;
        private ToolStripMenuItem ignoreUserItem;
        private ContextMenuStrip roomMenu;
        private ToolStripMenuItem ignoreRoomItem;
        private ToolStripMenuItem stopIgnoringRoomItem;
        private ToolStripMenuItem stopIgnoringUserItem;
        private KolikSoftware.Eee.Client.Notifications.AutoAwayMonitor autoAwayMonitor;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem ignoreAllUsersButThisItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem ignoreAllRoomsButThisItem;
        private ToolStripMenuItem stopIgnoringAllRoomsItem;
        private ToolStripMenuItem stopIgnoringAllUsersItem;
        private ToolStripMenuItem feedbackMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolTip userInfoToolTip;
        private ToolStripStatusLabel updatesLabel;
        private KolikSoftware.Eee.Client.Updating.UpdateManager updateManager;
        private ToolStripButton uploadToolItem;
        private ToolStripStatusLabel uploadingLabel;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripLabel dateLabel;
        private ToolStripButton historyDayLeftToolItem;
        private ToolStripButton historyDayRightToolItem;
        private ToolStripButton historyMonthLeftToolItem;
        private ToolStripButton historyMonthRightToolItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem followMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton followToolItem;
        private ToolStripStatusLabel disconnectedLabel;
        private ContextMenuStrip toolbarCustomizeMenu;
        private ToolStripMenuItem toggleStretchItem;
        private ToolStripComboBox searchBox;
        private ToolStripLabel searchLabel;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripButton playToolItem;
        private KolikSoftware.Eee.Client.Media.MediaPlayer mediaPlayer;
        private ToolStripStatusLabel downloadingLabel;
        private ToolStripButton pauseToolItem;
        private ToolStripButton stopToolItem;
        private ToolStripMenuItem downloadsMenuItem;
        private ToolStripMenuItem removeAllMenuItem;
        private ToolStripMenuItem openFolderMenuItem;
        private ToolStrip mediaToolStrip;
        private ToolStripButton playToolItem2;
        private ToolStripButton pauseToolItem2;
        private ToolStripButton stopToolItem2;
        private ToolStripLabel mediaLabel;
        private ToolStripButton backToolItem;
        private ToolStripButton forwardToolItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripButton closeMediaBarToolItem;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem viewMenu;
        private ToolStripMenuItem showMediaBarTooltem;
        private ToolStripButton volumeUpToolItem;
        private ToolStripButton volumeDownToolItem;
        private ToolStripSeparator toolStripSeparator16;
        private ContextMenuStrip browserMenu;
        private ToolStripMenuItem copyMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem uploadMediaMenuItem;
        private ToolStrip externalUsersToolStrip;
        private WebBrowser chatBrowser;
        private RichTextBox text;
        private LinkResolver linkResolver;


    }
}
