namespace KolikSoftware.Eee.Client.Reporting
{
    partial class UserFeedbackForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserFeedbackForm));
            this.captionPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.feedbackTypeCombo = new System.Windows.Forms.ComboBox();
            this.emailText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.commentPanel = new System.Windows.Forms.Panel();
            this.commentText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nameText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.suggestionPanel = new System.Windows.Forms.Panel();
            this.suggestionText = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.questionPanel = new System.Windows.Forms.Panel();
            this.questionText = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.bugReportPanel = new System.Windows.Forms.Panel();
            this.stepsToReproduceText = new System.Windows.Forms.TextBox();
            this.stepsToReproduceLabel = new System.Windows.Forms.Label();
            this.bugDescriptionText = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.captionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.commentPanel.SuspendLayout();
            this.suggestionPanel.SuspendLayout();
            this.questionPanel.SuspendLayout();
            this.bugReportPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionPanel
            // 
            this.captionPanel.BackColor = System.Drawing.SystemColors.Window;
            this.captionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captionPanel.Controls.Add(this.label3);
            this.captionPanel.Controls.Add(this.label2);
            this.captionPanel.Controls.Add(this.label1);
            this.captionPanel.Controls.Add(this.pictureBox1);
            this.captionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.captionPanel.Location = new System.Drawing.Point(0, 0);
            this.captionPanel.Name = "captionPanel";
            this.captionPanel.Size = new System.Drawing.Size(496, 118);
            this.captionPanel.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(52, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(413, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Thank you for your feedback.";
            this.label3.UseMnemonic = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(52, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(431, 56);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.UseMnemonic = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(52, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "User Feedback";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::KolikSoftware.Eee.Client.Properties.Resources.FeedbackBig;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(34, 34);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Feedback Type";
            // 
            // feedbackTypeCombo
            // 
            this.feedbackTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.feedbackTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.feedbackTypeCombo.FormattingEnabled = true;
            this.feedbackTypeCombo.Items.AddRange(new object[] {
            "Comment",
            "Suggestion",
            "Question",
            "Bug Report"});
            this.feedbackTypeCombo.Location = new System.Drawing.Point(101, 124);
            this.feedbackTypeCombo.Name = "feedbackTypeCombo";
            this.feedbackTypeCombo.Size = new System.Drawing.Size(383, 21);
            this.feedbackTypeCombo.TabIndex = 2;
            this.feedbackTypeCombo.SelectedIndexChanged += new System.EventHandler(this.feedbackTypeCombo_SelectedIndexChanged);
            // 
            // emailText
            // 
            this.emailText.Location = new System.Drawing.Point(101, 177);
            this.emailText.Name = "emailText";
            this.emailText.Size = new System.Drawing.Size(200, 20);
            this.emailText.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Your E-mail";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(97, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(386, 53);
            this.label6.TabIndex = 1;
            this.label6.Text = "By providing an e-mail address, we are able to inform you about the current state" +
                " of development or answer your question. Your e-mail address will not be provide" +
                "d to other persons or companies.";
            // 
            // commentPanel
            // 
            this.commentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commentPanel.Controls.Add(this.commentText);
            this.commentPanel.Controls.Add(this.label7);
            this.commentPanel.Location = new System.Drawing.Point(13, 246);
            this.commentPanel.Name = "commentPanel";
            this.commentPanel.Size = new System.Drawing.Size(471, 275);
            this.commentPanel.TabIndex = 4;
            this.commentPanel.Visible = false;
            // 
            // commentText
            // 
            this.commentText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commentText.Location = new System.Drawing.Point(87, 3);
            this.commentText.Multiline = true;
            this.commentText.Name = "commentText";
            this.commentText.Size = new System.Drawing.Size(383, 269);
            this.commentText.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(-3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 55);
            this.label7.TabIndex = 1;
            this.label7.Text = "Your Comment";
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(101, 151);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(200, 20);
            this.nameText.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(10, 154);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "Your Name";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(408, 526);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(327, 526);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // suggestionPanel
            // 
            this.suggestionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suggestionPanel.Controls.Add(this.suggestionText);
            this.suggestionPanel.Controls.Add(this.label9);
            this.suggestionPanel.Location = new System.Drawing.Point(13, 246);
            this.suggestionPanel.Name = "suggestionPanel";
            this.suggestionPanel.Size = new System.Drawing.Size(471, 268);
            this.suggestionPanel.TabIndex = 6;
            this.suggestionPanel.Visible = false;
            // 
            // suggestionText
            // 
            this.suggestionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suggestionText.Location = new System.Drawing.Point(87, 3);
            this.suggestionText.Multiline = true;
            this.suggestionText.Name = "suggestionText";
            this.suggestionText.Size = new System.Drawing.Size(383, 269);
            this.suggestionText.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(-3, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 55);
            this.label9.TabIndex = 1;
            this.label9.Text = "Your Suggestion";
            // 
            // questionPanel
            // 
            this.questionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.questionPanel.Controls.Add(this.questionText);
            this.questionPanel.Controls.Add(this.label10);
            this.questionPanel.Location = new System.Drawing.Point(13, 246);
            this.questionPanel.Name = "questionPanel";
            this.questionPanel.Size = new System.Drawing.Size(471, 275);
            this.questionPanel.TabIndex = 7;
            this.questionPanel.Visible = false;
            // 
            // questionText
            // 
            this.questionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.questionText.Location = new System.Drawing.Point(87, 3);
            this.questionText.Multiline = true;
            this.questionText.Name = "questionText";
            this.questionText.Size = new System.Drawing.Size(383, 269);
            this.questionText.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(-3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 55);
            this.label10.TabIndex = 1;
            this.label10.Text = "Your Question";
            // 
            // bugReportPanel
            // 
            this.bugReportPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bugReportPanel.Controls.Add(this.stepsToReproduceText);
            this.bugReportPanel.Controls.Add(this.stepsToReproduceLabel);
            this.bugReportPanel.Controls.Add(this.bugDescriptionText);
            this.bugReportPanel.Controls.Add(this.label11);
            this.bugReportPanel.Location = new System.Drawing.Point(13, 246);
            this.bugReportPanel.Name = "bugReportPanel";
            this.bugReportPanel.Size = new System.Drawing.Size(471, 275);
            this.bugReportPanel.TabIndex = 8;
            this.bugReportPanel.Visible = false;
            // 
            // stepsToReproduceText
            // 
            this.stepsToReproduceText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stepsToReproduceText.Location = new System.Drawing.Point(87, 145);
            this.stepsToReproduceText.Multiline = true;
            this.stepsToReproduceText.Name = "stepsToReproduceText";
            this.stepsToReproduceText.Size = new System.Drawing.Size(383, 127);
            this.stepsToReproduceText.TabIndex = 0;
            // 
            // stepsToReproduceLabel
            // 
            this.stepsToReproduceLabel.Location = new System.Drawing.Point(-3, 145);
            this.stepsToReproduceLabel.Name = "stepsToReproduceLabel";
            this.stepsToReproduceLabel.Size = new System.Drawing.Size(79, 55);
            this.stepsToReproduceLabel.TabIndex = 1;
            this.stepsToReproduceLabel.Text = "Steps to reproduce";
            // 
            // bugDescriptionText
            // 
            this.bugDescriptionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bugDescriptionText.Location = new System.Drawing.Point(87, 3);
            this.bugDescriptionText.Multiline = true;
            this.bugDescriptionText.Name = "bugDescriptionText";
            this.bugDescriptionText.Size = new System.Drawing.Size(383, 131);
            this.bugDescriptionText.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(-3, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 55);
            this.label11.TabIndex = 1;
            this.label11.Text = "Bug Description";
            // 
            // UserFeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 554);
            this.Controls.Add(this.commentPanel);
            this.Controls.Add(this.bugReportPanel);
            this.Controls.Add(this.questionPanel);
            this.Controls.Add(this.suggestionPanel);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.emailText);
            this.Controls.Add(this.feedbackTypeCombo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.captionPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserFeedbackForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Feedback";
            this.captionPanel.ResumeLayout(false);
            this.captionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.commentPanel.ResumeLayout(false);
            this.commentPanel.PerformLayout();
            this.suggestionPanel.ResumeLayout(false);
            this.suggestionPanel.PerformLayout();
            this.questionPanel.ResumeLayout(false);
            this.questionPanel.PerformLayout();
            this.bugReportPanel.ResumeLayout(false);
            this.bugReportPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel captionPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox feedbackTypeCombo;
        private System.Windows.Forms.TextBox emailText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel commentPanel;
        private System.Windows.Forms.TextBox commentText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Panel suggestionPanel;
        private System.Windows.Forms.TextBox suggestionText;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel questionPanel;
        private System.Windows.Forms.TextBox questionText;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel bugReportPanel;
        private System.Windows.Forms.TextBox stepsToReproduceText;
        private System.Windows.Forms.Label stepsToReproduceLabel;
        private System.Windows.Forms.TextBox bugDescriptionText;
        private System.Windows.Forms.Label label11;
    }
}