using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Eee.Client.Notifications;
using KolikSoftware.Eee.Client.Helpers;

namespace KolikSoftware.Eee.Client
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        bool templateChanged = false;

        public bool TemplateChanged
        {
            get
            {
                return this.templateChanged;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Properties.Settings.Default.Save();
            LoadAdvancedSettings();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveAdvancedSettings();
            Properties.Settings.Default.Save();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
        }

        void LoadAdvancedSettings()
        {
            this.jabberIdText.Text = Properties.Settings.Default.JabberID;
            this.jabberPasswordText.Text = Properties.Settings.Default.JabberPassword;
            this.refreshRateText.Text = Properties.Settings.Default.RefreshRate.ToString();
            this.notifyIconActionCombo.SelectedIndex = (int)Properties.Settings.Default.IconNotificationType;
            this.mediaBarVisibilityCombo.SelectedIndex = (int)Properties.Settings.Default.MediaBarVisible;
            this.autoAwayDelayText.Text = Properties.Settings.Default.AutoAwayDelay.ToString();
            this.templateCombo.SelectedIndex = Properties.Settings.Default.MessageTemplateNo - 1;

            foreach (ListViewItem item in this.advancedList.Items)
            {
                string settingName = item.Tag as string;
                item.Checked = (bool)Properties.Settings.Default.PropertyValues[settingName].PropertyValue;
            }
        }

        void SaveAdvancedSettings()
        {
            foreach (ListViewItem item in this.advancedList.Items)
            {
                string settingName = item.Tag as string;
                Properties.Settings.Default.PropertyValues[settingName].PropertyValue = item.Checked;
            }

            Properties.Settings.Default.RefreshRate = ParseInt(Properties.Settings.Default.RefreshRate, this.refreshRateText.Text);
            Properties.Settings.Default.AutoAwayDelay = ParseInt(Properties.Settings.Default.AutoAwayDelay, this.autoAwayDelayText.Text);

            Properties.Settings.Default.IconNotificationType = (IconNotificationType)this.notifyIconActionCombo.SelectedIndex;
            Properties.Settings.Default.MediaBarVisible = (MediaBarVisibility)this.mediaBarVisibilityCombo.SelectedIndex;

            if (Properties.Settings.Default.MessageTemplateNo != this.templateCombo.SelectedIndex + 1)
            {
                this.templateChanged = true;
                Properties.Settings.Default.MessageTemplateNo = this.templateCombo.SelectedIndex + 1;
            }

            Properties.Settings.Default.JabberID = this.jabberIdText.Text;

            if (Properties.Settings.Default.JabberPassword != this.jabberPasswordText.Text)
                Properties.Settings.Default.JabberPassword = Security.Encrypt(this.jabberPasswordText.Text, true);
        }

        int ParseInt(int setting, string value)
        {
            try
            {
                setting = int.Parse(value);
            }
            catch
            {
            }

            return setting;
        }
    }
}