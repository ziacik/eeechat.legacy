namespace KolikSoftware.Eee.Client
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Blink icon when user is connected or disconnected");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Notify about user connecting or disconnecting");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Notify about private message from ignored user");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Notify about private message even when notifications are disabled");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Stop notify icon on mouse over");
            KolikSoftware.Controls.Options.OptionsPage optionsPage1 = new KolikSoftware.Controls.Options.OptionsPage();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            KolikSoftware.Controls.Options.OptionsPage optionsPage2 = new KolikSoftware.Controls.Options.OptionsPage();
            KolikSoftware.Controls.Options.OptionsPage optionsPage3 = new KolikSoftware.Controls.Options.OptionsPage();
            KolikSoftware.Controls.Options.OptionsPage optionsPage4 = new KolikSoftware.Controls.Options.OptionsPage();
            KolikSoftware.Controls.Options.OptionsPage optionsPage5 = new KolikSoftware.Controls.Options.OptionsPage();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.advancedList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.options = new KolikSoftware.Controls.Options.OptionsControl();
            this.serviceGroup = new System.Windows.Forms.GroupBox();
            this.addressLabel = new System.Windows.Forms.Label();
            this.serviceAddressText = new System.Windows.Forms.TextBox();
            this.notificationsGroupBox = new System.Windows.Forms.GroupBox();
            this.notifyIconActionCombo = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.showNotificationCheck = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.jabberPasswordText = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.jabberIdText = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mediaBarVisibilityCombo = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.proxyGroup = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.proxyPassword = new System.Windows.Forms.TextBox();
            this.proxyUser = new System.Windows.Forms.TextBox();
            this.proxyDomain = new System.Windows.Forms.TextBox();
            this.proxyServer = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.templateCombo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.clientGroup = new System.Windows.Forms.GroupBox();
            this.useProfessionalRenderingCheck = new System.Windows.Forms.CheckBox();
            this.showInTaskBarCheck = new System.Windows.Forms.CheckBox();
            this.messagesGroup = new System.Windows.Forms.GroupBox();
            this.enterSendsBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.refreshRateText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.awayModeGroup = new System.Windows.Forms.GroupBox();
            this.autoAwayDelayText = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.bottomPanel.SuspendLayout();
            this.options.SuspendLayout();
            this.serviceGroup.SuspendLayout();
            this.notificationsGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.proxyGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.clientGroup.SuspendLayout();
            this.messagesGroup.SuspendLayout();
            this.awayModeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 281);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(395, 45);
            this.bottomPanel.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(227, 9);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(308, 10);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // advancedList
            // 
            this.advancedList.CheckBoxes = true;
            this.advancedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.advancedList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem1.StateImageIndex = 0;
            listViewItem1.Tag = "BlinkConnected";
            listViewItem2.StateImageIndex = 0;
            listViewItem2.Tag = "NotifyConnected";
            listViewItem3.StateImageIndex = 0;
            listViewItem3.Tag = "NotifyAboutIgnoredPersonalMessages";
            listViewItem4.StateImageIndex = 0;
            listViewItem4.Tag = "AlwaysNotifyAboutPersonalMessage";
            listViewItem5.StateImageIndex = 0;
            listViewItem5.Tag = "StopIconOnMouseOver";
            this.advancedList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.advancedList.Location = new System.Drawing.Point(12, 67);
            this.advancedList.MultiSelect = false;
            this.advancedList.Name = "advancedList";
            this.advancedList.ShowItemToolTips = true;
            this.advancedList.Size = new System.Drawing.Size(371, 190);
            this.advancedList.TabIndex = 12;
            this.advancedList.Tag = 3;
            this.advancedList.UseCompatibleStateImageBehavior = false;
            this.advancedList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 350;
            // 
            // options
            // 
            this.options.AllowDrop = true;
            this.options.Controls.Add(this.serviceGroup);
            this.options.Controls.Add(this.notificationsGroupBox);
            this.options.Controls.Add(this.groupBox3);
            this.options.Controls.Add(this.groupBox2);
            this.options.Controls.Add(this.proxyGroup);
            this.options.Controls.Add(this.groupBox1);
            this.options.Controls.Add(this.advancedList);
            this.options.Controls.Add(this.clientGroup);
            this.options.Controls.Add(this.messagesGroup);
            this.options.Controls.Add(this.awayModeGroup);
            this.options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.options.Location = new System.Drawing.Point(0, 0);
            this.options.Name = "options";
            optionsPage1.Caption = "General";
            optionsPage1.Image = ((System.Drawing.Image)(resources.GetObject("optionsPage1.Image")));
            optionsPage2.Caption = "Connection";
            optionsPage2.Image = global::KolikSoftware.Eee.Client.Properties.Resources.OptionsConnection;
            optionsPage3.Caption = "Layout";
            optionsPage3.Image = global::KolikSoftware.Eee.Client.Properties.Resources.OptionsLayout;
            optionsPage4.Caption = "Advanced";
            optionsPage4.Image = global::KolikSoftware.Eee.Client.Properties.Resources.OptionsAdvanced;
            optionsPage5.Caption = "Jabber";
            optionsPage5.Image = global::KolikSoftware.Eee.Client.Properties.Resources.Jabber;
            this.options.Pages.Add(optionsPage1);
            this.options.Pages.Add(optionsPage2);
            this.options.Pages.Add(optionsPage3);
            this.options.Pages.Add(optionsPage4);
            this.options.Pages.Add(optionsPage5);
            this.options.Size = new System.Drawing.Size(395, 281);
            this.options.TabIndex = 0;
            // 
            // serviceGroup
            // 
            this.serviceGroup.Controls.Add(this.addressLabel);
            this.serviceGroup.Controls.Add(this.serviceAddressText);
            this.serviceGroup.Location = new System.Drawing.Point(12, 204);
            this.serviceGroup.Name = "serviceGroup";
            this.serviceGroup.Size = new System.Drawing.Size(370, 59);
            this.serviceGroup.TabIndex = 15;
            this.serviceGroup.TabStop = false;
            this.serviceGroup.Tag = 1;
            this.serviceGroup.Text = "Service";
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(10, 20);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(45, 13);
            this.addressLabel.TabIndex = 0;
            this.addressLabel.Text = "&Address";
            // 
            // serviceAddressText
            // 
            this.serviceAddressText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ServiceUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.serviceAddressText.Location = new System.Drawing.Point(80, 19);
            this.serviceAddressText.Name = "serviceAddressText";
            this.serviceAddressText.Size = new System.Drawing.Size(284, 20);
            this.serviceAddressText.TabIndex = 0;
            this.serviceAddressText.Text = global::KolikSoftware.Eee.Client.Properties.Settings.Default.NewServiceUrl;
            // 
            // notificationsGroupBox
            // 
            this.notificationsGroupBox.Controls.Add(this.notifyIconActionCombo);
            this.notificationsGroupBox.Controls.Add(this.label7);
            this.notificationsGroupBox.Controls.Add(this.showNotificationCheck);
            this.notificationsGroupBox.Location = new System.Drawing.Point(13, 66);
            this.notificationsGroupBox.Name = "notificationsGroupBox";
            this.notificationsGroupBox.Size = new System.Drawing.Size(370, 72);
            this.notificationsGroupBox.TabIndex = 0;
            this.notificationsGroupBox.TabStop = false;
            this.notificationsGroupBox.Tag = 0;
            this.notificationsGroupBox.Text = "Notifications";
            // 
            // notifyIconActionCombo
            // 
            this.notifyIconActionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.notifyIconActionCombo.FormattingEnabled = true;
            this.notifyIconActionCombo.Items.AddRange(new object[] {
            "Blink icon",
            "Change icon",
            "Don\'t change icon"});
            this.notifyIconActionCombo.Location = new System.Drawing.Point(151, 19);
            this.notifyIconActionCombo.Name = "notifyIconActionCombo";
            this.notifyIconActionCombo.Size = new System.Drawing.Size(213, 21);
            this.notifyIconActionCombo.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "When new message arrives";
            // 
            // showNotificationCheck
            // 
            this.showNotificationCheck.AutoSize = true;
            this.showNotificationCheck.Checked = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ShowNotifications;
            this.showNotificationCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showNotificationCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ShowNotifications", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.showNotificationCheck.Location = new System.Drawing.Point(151, 46);
            this.showNotificationCheck.Name = "showNotificationCheck";
            this.showNotificationCheck.Size = new System.Drawing.Size(142, 17);
            this.showNotificationCheck.TabIndex = 0;
            this.showNotificationCheck.Text = "&Show bubble notification";
            this.showNotificationCheck.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.jabberPasswordText);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.jabberIdText);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(12, 66);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(371, 82);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Tag = 4;
            this.groupBox3.Text = "Credentials";
            // 
            // jabberPasswordText
            // 
            this.jabberPasswordText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jabberPasswordText.Location = new System.Drawing.Point(67, 45);
            this.jabberPasswordText.Name = "jabberPasswordText";
            this.jabberPasswordText.Size = new System.Drawing.Size(298, 20);
            this.jabberPasswordText.TabIndex = 3;
            this.jabberPasswordText.UseSystemPasswordChar = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Password";
            // 
            // jabberIdText
            // 
            this.jabberIdText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jabberIdText.Location = new System.Drawing.Point(67, 19);
            this.jabberIdText.Name = "jabberIdText";
            this.jabberIdText.Size = new System.Drawing.Size(298, 20);
            this.jabberIdText.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Jabber ID";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mediaBarVisibilityCombo);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Location = new System.Drawing.Point(12, 214);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(371, 61);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Tag = 2;
            this.groupBox2.Text = "Media";
            // 
            // mediaBarVisibilityCombo
            // 
            this.mediaBarVisibilityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mediaBarVisibilityCombo.FormattingEnabled = true;
            this.mediaBarVisibilityCombo.Items.AddRange(new object[] {
            "Never",
            "On Media",
            "Always"});
            this.mediaBarVisibilityCombo.Location = new System.Drawing.Point(10, 32);
            this.mediaBarVisibilityCombo.Name = "mediaBarVisibilityCombo";
            this.mediaBarVisibilityCombo.Size = new System.Drawing.Size(208, 21);
            this.mediaBarVisibilityCombo.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Show Media bar";
            // 
            // proxyGroup
            // 
            this.proxyGroup.Controls.Add(this.label4);
            this.proxyGroup.Controls.Add(this.label3);
            this.proxyGroup.Controls.Add(this.label2);
            this.proxyGroup.Controls.Add(this.label1);
            this.proxyGroup.Controls.Add(this.proxyPassword);
            this.proxyGroup.Controls.Add(this.proxyUser);
            this.proxyGroup.Controls.Add(this.proxyDomain);
            this.proxyGroup.Controls.Add(this.proxyServer);
            this.proxyGroup.Location = new System.Drawing.Point(12, 67);
            this.proxyGroup.Name = "proxyGroup";
            this.proxyGroup.Size = new System.Drawing.Size(370, 130);
            this.proxyGroup.TabIndex = 5;
            this.proxyGroup.TabStop = false;
            this.proxyGroup.Tag = 1;
            this.proxyGroup.Text = "Proxy";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "&Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "&User";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Domain";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Server";
            // 
            // proxyPassword
            // 
            this.proxyPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ProxyPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyPassword.Location = new System.Drawing.Point(80, 100);
            this.proxyPassword.Name = "proxyPassword";
            this.proxyPassword.Size = new System.Drawing.Size(284, 20);
            this.proxyPassword.TabIndex = 7;
            this.proxyPassword.Text = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ProxyPassword;
            this.proxyPassword.UseSystemPasswordChar = true;
            // 
            // proxyUser
            // 
            this.proxyUser.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ProxyUser", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyUser.Location = new System.Drawing.Point(80, 74);
            this.proxyUser.Name = "proxyUser";
            this.proxyUser.Size = new System.Drawing.Size(284, 20);
            this.proxyUser.TabIndex = 5;
            this.proxyUser.Text = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ProxyUser;
            // 
            // proxyDomain
            // 
            this.proxyDomain.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ProxyDomain", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyDomain.Location = new System.Drawing.Point(80, 48);
            this.proxyDomain.Name = "proxyDomain";
            this.proxyDomain.Size = new System.Drawing.Size(284, 20);
            this.proxyDomain.TabIndex = 3;
            this.proxyDomain.Text = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ProxyDomain;
            // 
            // proxyServer
            // 
            this.proxyServer.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ProxyServer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyServer.Location = new System.Drawing.Point(80, 22);
            this.proxyServer.Name = "proxyServer";
            this.proxyServer.Size = new System.Drawing.Size(284, 20);
            this.proxyServer.TabIndex = 0;
            this.proxyServer.Text = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ProxyServer;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.templateCombo);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(12, 136);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 72);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = 2;
            this.groupBox1.Text = "Messages";
            // 
            // templateCombo
            // 
            this.templateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.templateCombo.FormattingEnabled = true;
            this.templateCombo.Items.AddRange(new object[] {
            "Classic",
            "Colorful",
            "Elegant",
            "Posters",
            "Condensed Posters",
            "Funny Posters",
            "Full Posters"});
            this.templateCombo.Location = new System.Drawing.Point(10, 36);
            this.templateCombo.Name = "templateCombo";
            this.templateCombo.Size = new System.Drawing.Size(208, 21);
            this.templateCombo.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(186, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Template for messages (needs restart)";
            // 
            // clientGroup
            // 
            this.clientGroup.Controls.Add(this.useProfessionalRenderingCheck);
            this.clientGroup.Controls.Add(this.showInTaskBarCheck);
            this.clientGroup.Location = new System.Drawing.Point(12, 59);
            this.clientGroup.Name = "clientGroup";
            this.clientGroup.Size = new System.Drawing.Size(371, 71);
            this.clientGroup.TabIndex = 13;
            this.clientGroup.TabStop = false;
            this.clientGroup.Tag = 2;
            this.clientGroup.Text = "Client";
            // 
            // useProfessionalRenderingCheck
            // 
            this.useProfessionalRenderingCheck.AutoSize = true;
            this.useProfessionalRenderingCheck.Checked = global::KolikSoftware.Eee.Client.Properties.Settings.Default.UseProfessionalAppearance;
            this.useProfessionalRenderingCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useProfessionalRenderingCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "UseProfessionalAppearance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.useProfessionalRenderingCheck.Location = new System.Drawing.Point(7, 43);
            this.useProfessionalRenderingCheck.Name = "useProfessionalRenderingCheck";
            this.useProfessionalRenderingCheck.Size = new System.Drawing.Size(164, 17);
            this.useProfessionalRenderingCheck.TabIndex = 1;
            this.useProfessionalRenderingCheck.Text = "Use &professional appearance";
            this.useProfessionalRenderingCheck.UseVisualStyleBackColor = true;
            // 
            // showInTaskBarCheck
            // 
            this.showInTaskBarCheck.AutoSize = true;
            this.showInTaskBarCheck.Checked = global::KolikSoftware.Eee.Client.Properties.Settings.Default.ShowInTaskbar;
            this.showInTaskBarCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "ShowInTaskbar", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.showInTaskBarCheck.Location = new System.Drawing.Point(6, 19);
            this.showInTaskBarCheck.Name = "showInTaskBarCheck";
            this.showInTaskBarCheck.Size = new System.Drawing.Size(134, 17);
            this.showInTaskBarCheck.TabIndex = 0;
            this.showInTaskBarCheck.Text = "&Show client in Taskbar";
            this.showInTaskBarCheck.UseVisualStyleBackColor = true;
            // 
            // messagesGroup
            // 
            this.messagesGroup.Controls.Add(this.enterSendsBox);
            this.messagesGroup.Controls.Add(this.label6);
            this.messagesGroup.Controls.Add(this.refreshRateText);
            this.messagesGroup.Controls.Add(this.label5);
            this.messagesGroup.Location = new System.Drawing.Point(13, 144);
            this.messagesGroup.Name = "messagesGroup";
            this.messagesGroup.Size = new System.Drawing.Size(370, 72);
            this.messagesGroup.TabIndex = 7;
            this.messagesGroup.TabStop = false;
            this.messagesGroup.Tag = 0;
            this.messagesGroup.Text = "Messages";
            // 
            // enterSendsBox
            // 
            this.enterSendsBox.AutoSize = true;
            this.enterSendsBox.Checked = global::KolikSoftware.Eee.Client.Properties.Settings.Default.EnterSends;
            this.enterSendsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enterSendsBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::KolikSoftware.Eee.Client.Properties.Settings.Default, "EnterSends", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.enterSendsBox.Location = new System.Drawing.Point(10, 43);
            this.enterSendsBox.Name = "enterSendsBox";
            this.enterSendsBox.Size = new System.Drawing.Size(132, 17);
            this.enterSendsBox.TabIndex = 3;
            this.enterSendsBox.Text = "&Enter sends messages";
            this.enterSendsBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(252, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "seconds.";
            // 
            // refreshRateText
            // 
            this.refreshRateText.Location = new System.Drawing.Point(146, 17);
            this.refreshRateText.MaxLength = 5;
            this.refreshRateText.Name = "refreshRateText";
            this.refreshRateText.Size = new System.Drawing.Size(100, 20);
            this.refreshRateText.TabIndex = 1;
            this.refreshRateText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Check for messages every";
            // 
            // awayModeGroup
            // 
            this.awayModeGroup.Controls.Add(this.autoAwayDelayText);
            this.awayModeGroup.Controls.Add(this.label9);
            this.awayModeGroup.Controls.Add(this.label8);
            this.awayModeGroup.Location = new System.Drawing.Point(12, 222);
            this.awayModeGroup.Name = "awayModeGroup";
            this.awayModeGroup.Size = new System.Drawing.Size(370, 53);
            this.awayModeGroup.TabIndex = 8;
            this.awayModeGroup.TabStop = false;
            this.awayModeGroup.Tag = 0;
            this.awayModeGroup.Text = "Away Mode";
            // 
            // autoAwayDelayText
            // 
            this.autoAwayDelayText.Location = new System.Drawing.Point(41, 19);
            this.autoAwayDelayText.MaxLength = 2;
            this.autoAwayDelayText.Name = "autoAwayDelayText";
            this.autoAwayDelayText.Size = new System.Drawing.Size(49, 20);
            this.autoAwayDelayText.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Wait";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(96, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(231, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "min before entering Auto Away mode when idle.";
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(395, 326);
            this.Controls.Add(this.options);
            this.Controls.Add(this.bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.bottomPanel.ResumeLayout(false);
            this.options.ResumeLayout(false);
            this.options.PerformLayout();
            this.serviceGroup.ResumeLayout(false);
            this.serviceGroup.PerformLayout();
            this.notificationsGroupBox.ResumeLayout(false);
            this.notificationsGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.proxyGroup.ResumeLayout(false);
            this.proxyGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.clientGroup.ResumeLayout(false);
            this.clientGroup.PerformLayout();
            this.messagesGroup.ResumeLayout(false);
            this.messagesGroup.PerformLayout();
            this.awayModeGroup.ResumeLayout(false);
            this.awayModeGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private KolikSoftware.Controls.Options.OptionsControl options;
        private System.Windows.Forms.GroupBox notificationsGroupBox;
        private System.Windows.Forms.GroupBox proxyGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox proxyServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox proxyPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox proxyUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox proxyDomain;
        private System.Windows.Forms.GroupBox messagesGroup;
        private System.Windows.Forms.TextBox refreshRateText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox notifyIconActionCombo;
        private System.Windows.Forms.CheckBox showNotificationCheck;
        private System.Windows.Forms.GroupBox awayModeGroup;
        private System.Windows.Forms.CheckBox showInTaskBarCheck;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox autoAwayDelayText;
        private System.Windows.Forms.ListView advancedList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox clientGroup;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox useProfessionalRenderingCheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox templateCombo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox serviceGroup;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox serviceAddressText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox mediaBarVisibilityCombo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox enterSendsBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox jabberIdText;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox jabberPasswordText;
        private System.Windows.Forms.Label label13;



    }
}