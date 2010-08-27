using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Eee.Client.Notifications;
using KolikSoftware.Eee.Client.Helpers;
using KolikSoftware.Eee.Client.PluginCore;
using KolikSoftware.Eee.Service;

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
            SetupServiceList();
            Properties.Settings.Default.Save();
            LoadAdvancedSettings();
        }

        private void SetupServiceList()
        {
            this.serviceList.Items.Clear();

            foreach (var service in PluginHelper.Services)
            {
                var serviceName = service.GetType().Name;
                var item = new ListViewItem(serviceName);
                item.Tag = service;
                this.serviceList.Items.Add(item);
            }
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

            foreach (ListViewItem item in this.serviceList.Items)
            {
                string serviceName = item.Text;
                var service = (IEeeService)item.Tag;
                item.Checked = service.Enabled;
            }
        }

        void SaveAdvancedSettings()
        {
            if (!this.serviceList.Items[0].Checked)
                this.serviceList.Items[0].Checked = true;

            foreach (ListViewItem item in this.serviceList.Items)
            {
                var serviceName = item.Text;
                var property = Properties.Settings.Default.PropertyValues["Service." + serviceName];
                if (property == null)
                {
                    var prop = new System.Configuration.SettingsProperty("Service." + serviceName);
                    property = new System.Configuration.SettingsPropertyValue(prop);
                    Properties.Settings.Default.PropertyValues.Add(property);
                }
                property.PropertyValue = item.Checked;
                var service = (IEeeService)item.Tag;

                if (service.Enabled && !item.Checked)
                    DisableService(service);
                else if (!service.Enabled && item.Checked)
                    EnableService(service);
            }

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
        }

        void DisableService(IEeeService service)
        {
            service.Disconnect();
            service.Enabled = false;
        }

        void EnableService(IEeeService service)
        {            
            service.Connect(null, null);
            service.Enabled = true;
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