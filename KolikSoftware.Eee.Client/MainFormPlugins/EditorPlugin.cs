using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Eee.Service.Domain;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class EditorPlugin : IMainFormPlugin
    {
        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            this.Form.Editor.KeyDown += new KeyEventHandler(Editor_KeyDown);
            this.Form.Editor.TextChanged += new EventHandler(Editor_TextChanged);
        }

        void Editor_TextChanged(object sender, EventArgs e)
        {
            
        }

        void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            bool control;

            if (Properties.Settings.Default.EnterSends)
                control = !e.Control && !e.Shift;
            else
                control = e.Control;

            if ((control && e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.S))
            {
                string textToSend = this.Form.Editor.Text.Trim();
                this.Form.Editor.Text = "";

                if (textToSend != "")
                {
                    string recipientName = GetRecipient(ref textToSend);

                    if (textToSend != null && textToSend.Length > 0)
                    {
                        //TODO:
                        Room room = new Room();
                        room.Name = "Pokec";

                        User recipient = this.Form.GetPlugin<UserStatePlugin>().GetUser(recipientName);

                        this.Form.Service.SendMessage(room, recipient, textToSend);
                    }
                }
            }
            else if (e.Control && e.KeyCode == Keys.R)
            {
                //Reply();
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                //Follow();
            }
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)
            {
               /* int index = (int)e.KeyCode - (int)Keys.F1;

                switch (e.Modifiers)
                {
                    case Keys.None:
                        AddUserToText(index);
                        break;
                    case Keys.Control:
                        SelectRoomNo(index);
                        break;
                    case Keys.Alt:
                        SelectUserNo(index);
                        break;
                }*/
            }
            else
            {
                e.SuppressKeyPress = false;
                e.Handled = false;
            }

            //if (e.Control == false && e.Alt == false && (e.KeyCode & Keys.KeyCode) != Keys.None)
                //this.replyUserIndex = 0;
            
        }

        string GetRecipient(ref string messageToSend)
        {
            if (messageToSend == null || messageToSend.Length == 0)
                return null;

            if (messageToSend == "/resetlayout")
            {
                Properties.Settings.Default.ResetLayout = true;
                MessageBox.Show(this.Form, "Please restart the application to apply.", "Reset Layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                messageToSend = null;
                return null;
            }
            else if (messageToSend[0] == '/' && messageToSend.IndexOf(":") > 0)
            {
                int idx = messageToSend.IndexOf(":");
                string recipient = messageToSend.Substring(1, idx - 1);
                messageToSend = messageToSend.Substring(1);

                //AddReplyUser(recipient, true, this.SelectedRoomId);

                return recipient;
            }
            //else if (this.SelectedUserId != 0)
            //{
            //    if (messageToSend.StartsWith(this.SelectedUserLogin + ":") == false)
            //        messageToSend = this.SelectedUserLogin + ": " + messageToSend;

            //    AddReplyUser(this.SelectedUserLogin, true, this.SelectedRoomId);

            //    return this.SelectedUserLogin;
            //}
            else
            {
                //if (MessageVisibilityHelper.Instance.IsAddressedMessage(messageToSend))
                //    AddReplyUser(messageToSend.Substring(0, messageToSend.IndexOf(':')), false, this.SelectedRoomId);

                return null;
            }
        }

        string GetRecipient(string messageToSend)
        {
            if (!string.IsNullOrEmpty(messageToSend) && messageToSend[0] == '/' && messageToSend.IndexOf(":") > 0)
            {
                int idx = messageToSend.IndexOf(":");
                string recipient = messageToSend.Substring(1, idx - 1);
                return recipient;
            }
            //else if (this.SelectedUserId != 0)
            //{
            //    return this.SelectedUserLogin;
            //}
            else
            {
                return null;
            }
        }

    }
}
