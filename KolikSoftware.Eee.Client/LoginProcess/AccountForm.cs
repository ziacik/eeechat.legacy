using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Text.RegularExpressions;

namespace KolikSoftware.Eee.Client.LoginProcess
{
    public partial class AccountForm : Form
    {
        BackgroundServiceController serviceController;

        public AccountForm(BackgroundServiceController serviceController)
        {
            InitializeComponent();

            this.serviceController = serviceController;
            
            this.serviceController.Registered += serviceController_Registered;
            this.serviceController.RegisterFailed += serviceController_RegisterFailed;
            this.serviceController.ErrorOccured += serviceController_ErrorOccured;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (this.cancelButton.Enabled == false)
                e.Cancel = true;
        }

        void serviceController_ErrorOccured(object sender, BackgroundServiceController.ErrorOccuredEventArgs e)
        {
            MessageBox.Show(this, "Error creating new account.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.cancelButton.Enabled = true;
            Close();
        }

        void serviceController_RegisterFailed(object sender, BackgroundServiceController.RegisterFailedEventArgs e)
        {
            SetEnabled(true);
            MessageBox.Show(this, "Creating new account failed. The login is probably already taken. Please, try another one.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void SetEnabled(bool enabled)
        {
            this.applyButton.Enabled = enabled;
            this.cancelButton.Enabled = enabled;
        }

        void serviceController_Registered(object sender, BackgroundServiceController.RegisteredEventArgs e)
        {
            SetEnabled(true);
            MessageBox.Show(this, "Account has been created. Please, click File / Connect to log in.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        void applyButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SetEnabled(false);

                Color color = (Color)this.colorLabel.ForeColor;

                SecureString securePassword = new SecureString();

                foreach (char passwordChar in this.passwordText.Text)
                {
                    securePassword.AppendChar(passwordChar);
                }

                this.serviceController.RegisterUser(this.loginText.Text, securePassword, (int)(((uint)color.ToArgb()) & 16777215));
            }
        }

        static readonly Regex LoginRegex = new Regex("^[a-zA-Z0-9]{3,12}$", RegexOptions.Compiled);
        static readonly Regex PasswordRegex = new Regex("^[a-zA-Z0-9_]{5,20}$", RegexOptions.Compiled);

        bool ValidateInput()
        {
            if (string.IsNullOrEmpty(this.loginText.Text))
            {
                MessageBox.Show(this, "Please enter login name.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(this.passwordText.Text))
            {
                MessageBox.Show(this, "Please enter password.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(this.password2Text.Text))
            {
                MessageBox.Show(this, "Please enter pasword confirmation.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (this.passwordText.Text != this.password2Text.Text)
            {
                MessageBox.Show(this, "Password confirmation does not match original password.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (LoginRegex.IsMatch(this.loginText.Text) == false)
            {
                MessageBox.Show(this, "Login name may only contain these characters: a-z, A-Z and numbers and must be at least 3 and at most 12 characters long.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (PasswordRegex.IsMatch(this.passwordText.Text) == false)
            {
                MessageBox.Show(this, "Password may only contain these characters: a-z, A-Z, underscore and numbers and must be at least 5 and at most 20 characters long.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        void colorLabel_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = (Color)this.colorLabel.ForeColor;
                colorDialog.AllowFullOpen = true;
                colorDialog.AnyColor = true;
                colorDialog.FullOpen = true;
                colorDialog.SolidColorOnly = true;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    this.colorLabel.ForeColor = colorDialog.Color;
                }
            }
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}