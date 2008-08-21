namespace KolikSoftware.Controls
{
    partial class Suggestion
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.suggestionLabel = new System.Windows.Forms.LinkLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // suggestionLabel
            // 
            this.suggestionLabel.ActiveLinkColor = System.Drawing.Color.Black;
            this.suggestionLabel.AutoSize = true;
            this.suggestionLabel.DisabledLinkColor = System.Drawing.Color.Black;
            this.suggestionLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.suggestionLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.suggestionLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.suggestionLabel.LinkColor = System.Drawing.Color.Black;
            this.suggestionLabel.Location = new System.Drawing.Point(1, 3);
            this.suggestionLabel.Name = "suggestionLabel";
            this.suggestionLabel.Size = new System.Drawing.Size(60, 13);
            this.suggestionLabel.TabIndex = 1;
            this.suggestionLabel.Text = "Suggestion";
            this.suggestionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.suggestionLabel.VisitedLinkColor = System.Drawing.Color.Black;
            this.suggestionLabel.Resize += new System.EventHandler(this.suggestionLabel_Resize);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(70, 55);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.ShortcutsEnabled = false;
            this.richTextBox1.ShowSelectionMargin = true;
            this.richTextBox1.Size = new System.Drawing.Size(275, 151);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // Suggestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Ivory;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.suggestionLabel);
            this.Name = "Suggestion";
            this.Size = new System.Drawing.Size(410, 255);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel suggestionLabel;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}
