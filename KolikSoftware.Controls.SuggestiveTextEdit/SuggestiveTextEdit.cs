using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace KolikSoftware.Controls
{
    public class SuggestiveTextEdit : RichTextBox
    {
        string currentSuggestion = null;

        public void Suggest(string suggestion)
        {
            if (this.SelectionStart != 0 || this.SelectionLength != this.Text.Length)
                return;

            if (this.currentSuggestion != null)
                RemoveSuggestion();

            this.SelectionColor = Color.LightSlateGray;
            this.SelectedText = suggestion + ":";
            this.currentSuggestion = suggestion;
        }

        public void RemoveSuggestion()
        {
            this.SelectedText = "";
            this.currentSuggestion = null;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (this.currentSuggestion != null)
            {
                if (this.SelectionLength == 0)
                {
                    RemoveSuggestion();
                }
                else if (e.KeyChar == ' ')
                {
                    this.SelectionStart = this.Text.Length;
                }
                else if (e.KeyChar.ToString().Equals(this.currentSuggestion[this.SelectionStart].ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    this.SelectionStart++;
                    this.SelectionLength = this.Text.Length - this.SelectionStart;
                    e.Handled = true;
                }
                else
                {
                    RemoveSuggestion();
                }
            }

            base.OnKeyPress(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.ShortcutsEnabled = false;
            this.ShowSelectionMargin = true;
            this.ResumeLayout(false);
        }
    }
}
