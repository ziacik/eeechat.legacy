namespace KolikSoftware.Eee.Client.Reporting
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.errorPicture = new System.Windows.Forms.PictureBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.connectionProblemsPicture = new System.Windows.Forms.PictureBox();
            this.reportButton = new System.Windows.Forms.Button();
            this.disconnectedPicture = new System.Windows.Forms.PictureBox();
            this.downloadProblemPicture = new System.Windows.Forms.PictureBox();
            this.uploadProblemPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectionProblemsPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.disconnectedPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.downloadProblemPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uploadProblemPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // errorPicture
            // 
            this.errorPicture.Image = global::KolikSoftware.Eee.Client.Properties.Resources.ErrorBig;
            this.errorPicture.Location = new System.Drawing.Point(13, 13);
            this.errorPicture.Name = "errorPicture";
            this.errorPicture.Size = new System.Drawing.Size(32, 32);
            this.errorPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.errorPicture.TabIndex = 0;
            this.errorPicture.TabStop = false;
            // 
            // errorLabel
            // 
            this.errorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.errorLabel.Location = new System.Drawing.Point(52, 13);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(270, 62);
            this.errorLabel.TabIndex = 1;
            this.errorLabel.Text = "Error";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(246, 82);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(84, 82);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 3;
            this.copyButton.Text = "&Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Visible = false;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // connectionProblemsPicture
            // 
            this.connectionProblemsPicture.Image = global::KolikSoftware.Eee.Client.Properties.Resources.ConnectionProblemsBig;
            this.connectionProblemsPicture.Location = new System.Drawing.Point(14, 13);
            this.connectionProblemsPicture.Name = "connectionProblemsPicture";
            this.connectionProblemsPicture.Size = new System.Drawing.Size(32, 32);
            this.connectionProblemsPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.connectionProblemsPicture.TabIndex = 0;
            this.connectionProblemsPicture.TabStop = false;
            this.connectionProblemsPicture.Visible = false;
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(165, 82);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(75, 23);
            this.reportButton.TabIndex = 3;
            this.reportButton.Text = "&Report";
            this.reportButton.UseVisualStyleBackColor = true;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // disconnectedPicture
            // 
            this.disconnectedPicture.Image = global::KolikSoftware.Eee.Client.Properties.Resources.ConnectBig;
            this.disconnectedPicture.Location = new System.Drawing.Point(14, 13);
            this.disconnectedPicture.Name = "disconnectedPicture";
            this.disconnectedPicture.Size = new System.Drawing.Size(32, 32);
            this.disconnectedPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.disconnectedPicture.TabIndex = 0;
            this.disconnectedPicture.TabStop = false;
            this.disconnectedPicture.Visible = false;
            // 
            // downloadProblemPicture
            // 
            this.downloadProblemPicture.Image = global::KolikSoftware.Eee.Client.Properties.Resources.DownloadBig;
            this.downloadProblemPicture.Location = new System.Drawing.Point(14, 13);
            this.downloadProblemPicture.Name = "downloadProblemPicture";
            this.downloadProblemPicture.Size = new System.Drawing.Size(32, 32);
            this.downloadProblemPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.downloadProblemPicture.TabIndex = 4;
            this.downloadProblemPicture.TabStop = false;
            this.downloadProblemPicture.Visible = false;
            // 
            // uploadProblemPicture
            // 
            this.uploadProblemPicture.Image = global::KolikSoftware.Eee.Client.Properties.Resources.UploadBig;
            this.uploadProblemPicture.Location = new System.Drawing.Point(14, 13);
            this.uploadProblemPicture.Name = "uploadProblemPicture";
            this.uploadProblemPicture.Size = new System.Drawing.Size(32, 32);
            this.uploadProblemPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.uploadProblemPicture.TabIndex = 5;
            this.uploadProblemPicture.TabStop = false;
            this.uploadProblemPicture.Visible = false;
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 117);
            this.ControlBox = false;
            this.Controls.Add(this.uploadProblemPicture);
            this.Controls.Add(this.downloadProblemPicture);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.disconnectedPicture);
            this.Controls.Add(this.connectionProblemsPicture);
            this.Controls.Add(this.errorPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unhandled Error";
            ((System.ComponentModel.ISupportInitialize)(this.errorPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectionProblemsPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.disconnectedPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.downloadProblemPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uploadProblemPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox errorPicture;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.PictureBox connectionProblemsPicture;
        private System.Windows.Forms.Button reportButton;
        private System.Windows.Forms.PictureBox disconnectedPicture;
        private System.Windows.Forms.PictureBox downloadProblemPicture;
        private System.Windows.Forms.PictureBox uploadProblemPicture;
    }
}