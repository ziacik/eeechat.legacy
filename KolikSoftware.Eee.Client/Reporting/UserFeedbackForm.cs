using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client.Reporting
{
    public partial class UserFeedbackForm : Form
    {
        BackgroundServiceController serviceController;

        public UserFeedbackForm(BackgroundServiceController serviceController)
        {
            InitializeComponent();
            this.serviceController = serviceController;
            this.feedbackTypeCombo.SelectedIndex = 0;
            if (this.serviceController != null && this.serviceController.CurrentUser != null)
                this.nameText.Text = this.serviceController.CurrentUser.Login;
        }

        public void ReportError(Exception ex)
        {
            this.feedbackTypeCombo.SelectedIndex = 3;

            string environment = "An error has occured in " + Global.Instance.ApplicationAndVersion + ", at " + DateTime.Now.ToString() + "." + Environment.NewLine;
            this.bugDescriptionText.Text = environment + Environment.NewLine + ex.ToString();

            ShowDialog();
        }

        Panel activePanel = null;

        void ActivatePanel(Panel panel)
        {
            if (this.activePanel != null)
                this.activePanel.Visible = false;

            panel.Visible = true;
            this.activePanel = panel;
        }

        void Send()
        {
            if (CheckRequiredFields() == false)
                return;

            string feedbackType = "";
            string description = "";

            switch (this.feedbackTypeCombo.SelectedIndex)
            {
                case 0:
                    feedbackType = "Comment";
                    description = this.commentText.Text;
                    break;
                case 1:
                    feedbackType = "Suggestion";
                    description = this.suggestionText.Text;
                    break;
                case 2:
                    feedbackType = "Question";
                    description = this.questionText.Text;
                    break;
                case 3:
                    feedbackType = "Bug Report";
                    description = "Description:\n" + this.bugDescriptionText.Text + "\n\nSteps:\n" + this.stepsToReproduceText.Text;
                    break;
            }

            this.serviceController.SendFeedback(this.nameText.Text, this.emailText.Text, feedbackType, description);

            MessageBox.Show(this, "Thank you for submitting your feedback.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }

        bool CheckRequiredFields()
        {
            /// If Question, e-mail must be filled.
            if (this.feedbackTypeCombo.SelectedIndex == 2 && this.emailText.Text.Trim().Length == 0)
            {
                MessageBox.Show(this, "We can only able to respond to your question if you provide an e-mail address for us.", Application.ProductName);
                return false;
            }

            string requiredField = null;

            if (this.feedbackTypeCombo.SelectedIndex == 0 && this.commentText.Text.Trim().Length == 0)
                requiredField = "Comment";
            else if (this.feedbackTypeCombo.SelectedIndex == 1 && this.suggestionText.Text.Trim().Length == 0)
                requiredField = "Suggestion";
            else if (this.feedbackTypeCombo.SelectedIndex == 2 && this.questionText.Text.Trim().Length == 0)
                requiredField = "Question";
            else if (this.feedbackTypeCombo.SelectedIndex == 3 && this.bugDescriptionText.Text.Trim().Length == 0)
                requiredField = "Bug Description";

            if (requiredField != null)
            {
                MessageBox.Show(this, "Please, fill required field: " + requiredField + ".", Application.ProductName);
                return false;
            }

            return true;
        }

        #region Event Handlers
        void feedbackTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.feedbackTypeCombo.SelectedIndex)
            {
                case 0:
                    ActivatePanel(this.commentPanel);
                    break;
                case 1:
                    ActivatePanel(this.suggestionPanel);
                    break;
                case 2:
                    ActivatePanel(this.questionPanel);
                    break;
                case 3:
                    ActivatePanel(this.bugReportPanel);
                    break;
            }
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        void sendButton_Click(object sender, EventArgs e)
        {
            Send();
        }
        #endregion
    }
}