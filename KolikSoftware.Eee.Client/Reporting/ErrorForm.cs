using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client.Reporting
{
    public partial class ErrorForm : Form
    {
        Exception error = null;
        Mode mode = Mode.Error;
        BackgroundServiceController serviceController;

        public enum Mode
        {
            Error,
            ConnectionProblem,
            Disconnected,
            DownloadProblem,
            UploadProblem
        }

        public ErrorForm(Exception error, Mode mode, BackgroundServiceController serviceController)
        {
            this.error = error;
            this.mode = mode;
            this.serviceController = serviceController;

            InitializeComponent();

            if (this.mode == Mode.ConnectionProblem)
            {
                this.errorPicture.Visible = false;
                //this.reportButton.Visible = false;
                this.connectionProblemsPicture.Visible = true;
                this.Text = "Connection Problems";
                this.errorLabel.Text = error.Message;
            }
            else if (this.mode == Mode.Disconnected)
            {
                this.errorPicture.Visible = false;
                this.reportButton.Visible = false;
                this.disconnectedPicture.Visible = true;
                this.Text = "Disconnected";
                this.errorLabel.Text = "You have been disconnected by another Eee client.";
            }
            else if (this.mode == Mode.DownloadProblem)
            {
                this.errorPicture.Visible = false;
                this.reportButton.Visible = false;
                this.downloadProblemPicture.Visible = true;
                this.Text = "Download Failed";
                this.errorLabel.Text = "Download Failed (" + ExtractErrorMessage(error) + "). Try restarting the download.";
            }
            else if (this.mode == Mode.UploadProblem)
            {
                this.errorPicture.Visible = false;
                this.reportButton.Visible = false;
                this.uploadProblemPicture.Visible = true;
                this.Text = "Upload Failed";
                this.errorLabel.Text = "Upload Failed (" + ExtractErrorMessage(error) + "). Try restarting the upload.";
            }
            else
            {
                this.errorPicture.Visible = true;
                this.connectionProblemsPicture.Visible = false;
                this.errorLabel.Text = error.ToString();
            }
        }

        string ExtractErrorMessage(Exception error)
        {
            while (error.InnerException != null)
            {
                error = error.InnerException;
            }

            return error.Message;
        }

        public static void ShowError(Exception ex, BackgroundServiceController serviceController)
        {
            using (ErrorForm errorForm = new ErrorForm(ex, Mode.Error, serviceController))
            {
                errorForm.ShowDialog();
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.error.ToString());
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static void ShowConnectionProblem(Exception ex, BackgroundServiceController serviceController)
        {
            using (ErrorForm errorForm = new ErrorForm(ex, Mode.ConnectionProblem, serviceController))
            {
                errorForm.ShowDialog();
            }
        }

        public static void ShowDisconnectedInfo(BackgroundServiceController serviceController)
        {
            using (ErrorForm errorForm = new ErrorForm(null, Mode.Disconnected, serviceController))
            {
                errorForm.ShowDialog();
            }
        }

        public static void ShowDownloadProblem(Exception exception, BackgroundServiceController serviceController)
        {
            using (ErrorForm errorForm = new ErrorForm(exception, Mode.DownloadProblem, serviceController))
            {
                errorForm.ShowDialog();
            }
        }

        public static void ShowUploadProblem(Exception exception, BackgroundServiceController serviceController)
        {
            using (ErrorForm errorForm = new ErrorForm(exception, Mode.UploadProblem, serviceController))
            {
                errorForm.ShowDialog();
            }
        }

        void reportButton_Click(object sender, EventArgs e)
        {
            Close();

            using (UserFeedbackForm userFeedbackForm = new UserFeedbackForm(this.serviceController))
            {
                userFeedbackForm.ReportError(this.error);
            }
        }
    }
}