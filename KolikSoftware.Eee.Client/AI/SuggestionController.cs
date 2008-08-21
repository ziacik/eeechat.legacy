using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Controls;

namespace KolikSoftware.Eee.Client.AI
{
    public class SuggestionController
    {
        Form form;
        SuggestiveTextEdit textEdit;

        bool wasDeactivated = true;
        bool isActive = false;

        public SuggestionController(Form form, SuggestiveTextEdit textEdit)
        {
            this.form = form;
            this.textEdit = textEdit;

            this.form.Activated += new EventHandler(form_Activated);
            this.form.Deactivate += new EventHandler(form_Deactivate);
        }

        void form_Deactivate(object sender, EventArgs e)
        {
            this.wasDeactivated = true;
            this.isActive = false;
        }

        void form_Activated(object sender, EventArgs e)
        {
            this.isActive = true;
            //if (this.textEdit.Text.Length == 0)
                Suggest();
        }

        void Suggest()
        {
            if (this.currentRoomSuggestion != null)
                this.textEdit.Suggest(this.currentRoomSuggestion);
        }

        string currentRoomSuggestion = null;

        public void Room(string name)
        {
            if (this.currentRoomSuggestion == null)
            {
                this.currentRoomSuggestion = name;
            }
            else
            {
                if (this.wasDeactivated)
                {
                    this.currentRoomSuggestion = name;
                    this.wasDeactivated = true;
                }
            }
        }
    }
}
