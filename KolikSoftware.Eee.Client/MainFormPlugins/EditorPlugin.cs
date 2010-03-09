using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Eee.Service.Domain;
using System.Drawing;
using System.Text.RegularExpressions;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class EditorPlugin : IMainFormPlugin
    {
        public MainForm Form { get; set; }
        public int LastPostCount { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            this.Form.Editor.KeyDown += new KeyEventHandler(Editor_KeyDown);
            this.Form.Editor.TextChanged += new EventHandler(Editor_TextChanged);
            this.Form.GetPlugin<UserStatePlugin>().SelectedUserChanged += new EventHandler<EventArgs>(EditorPlugin_SelectedUserChanged);
            
            this.ReplyList = new List<Post>();
            this.CurrentReplyIndex = -1;

            this.Form.ReplyUsersMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(ReplyUsersMenuStrip_ItemClicked);
        }

        void EditorPlugin_SelectedUserChanged(object sender, EventArgs e)
        {
            SetColorByUserInText();
        }

        void SetColorByUserInText()
        {
            Match match = ReplyCheckRegexNoEnd.Match(this.Form.Editor.Text);
            Color targetColor = Color.White;

            if (match.Success)
            {
                if (this.Form.Editor.Text[0] != '/' && this.Form.GetPlugin<UserStatePlugin>().SelectedUser != null)
                {
                    SetColorBySelectedUser();
                }
                else
                {
                    string userName = match.Groups[1].Value;
                    UserStatePlugin userStatePlugin = this.Form.GetPlugin<UserStatePlugin>();

                    User user = userStatePlugin.GetUser(userName);

                    if (user != null)
                    {
                        targetColor = Color.FromArgb(user.Color);
                        targetColor = Color.FromArgb(255, targetColor);
                    }

                    if (this.Form.Editor.BackColor != targetColor)
                    {
                        this.Form.Editor.BackColor = targetColor;
                        this.Form.Editor.ForeColor = (targetColor == Color.White) ? Color.Black : Color.White;
                    }
                }
            }
            else
            {
                SetColorBySelectedUser();
            }
        }

        void SetColorBySelectedUser()
        {
            Color targetColor = Color.White;
            User selectedUser = this.Form.GetPlugin<UserStatePlugin>().SelectedUser;

            if (selectedUser != null)
            {
                if (selectedUser != null)
                {
                    targetColor = Color.FromArgb(selectedUser.Color);
                    targetColor = Color.FromArgb(255, targetColor);
                }
            }

            if (this.Form.Editor.BackColor != targetColor)
            {
                this.Form.Editor.BackColor = targetColor;
                this.Form.Editor.ForeColor = (targetColor == Color.White) ? Color.Black : Color.White;
                this.Form.Editor.Refresh();
            }
        }
  
        void ReplyUsersMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.Form.Editor.Text = e.ClickedItem.Text + ": ";
            this.Form.Editor.SelectionStart = this.Form.Editor.Text.Length;
            ContextMenuStrip strip = (ContextMenuStrip)sender;
            this.CurrentReplyIndex = strip.Items.IndexOf(e.ClickedItem);
        }

        void Editor_TextChanged(object sender, EventArgs e)
        {
            SetColorByUserInText();
        }

        //TODO: naming
        static readonly Regex ReplyCheckRegex = new Regex(@"^/?[a-zA-Z]+:\s*$", RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly Regex ReplyCheckRegexNoEnd = new Regex(@"^/([a-zA-Z]+):", RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly Regex ReplaceReplyUser = new Regex(@"^([/]?\w+[:]\s*)?(?<text>.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;

            bool control;

            if (Properties.Settings.Default.EnterSends)
                control = !e.Control && !e.Shift;
            else
                control = e.Control;

            string text = this.Form.Editor.Text;
            bool empty = string.IsNullOrEmpty(text);

            bool noShifts = !e.Control && !e.Alt && !e.Shift;

            if ((control && e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.S))
            {
                string textToSend = this.Form.Editor.Text.Trim();
                this.Form.Editor.Text = "";

#if DEBUG
                if (textToSend == "reload")
                {
                    this.Form.GetPlugin<BrowserPlugin>().Reload();
                    return;
                }
#endif

                if (textToSend != "")
                {
                    string recipientName = GetRecipient(ref textToSend);

                    if (textToSend != null && textToSend.Length > 0)
                    {
                        User recipient = this.Form.GetPlugin<UserStatePlugin>().GetUser(recipientName);
                        if (recipient == null)
                            recipient = this.Form.GetPlugin<UserStatePlugin>().SelectedUser;

                        Room room = this.Form.GetPlugin<RoomStatePlugin>().SelectedRoom;

                        this.Form.Service.SendMessage(room, recipient, textToSend);
                        this.CurrentReplyIndex = -1;
                    }
                }
            }
            else if (noShifts && e.KeyCode == Keys.Space)
            {
                CheckMacro();
                e.SuppressKeyPress = false;
                e.Handled = false;
            }
            else if (noShifts && e.KeyCode == Keys.Up && (empty || ReplyCheckRegex.IsMatch(text)))
            {
                ReplyUp();
            }
            else if (noShifts && e.KeyCode == Keys.Down && (empty || ReplyCheckRegex.IsMatch(text)))
            {
                ReplyDown();
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                //Follow();
            }
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)
            {
                int index = (int)e.KeyCode - (int)Keys.F1;

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
                }
            }
            else
            {
                e.SuppressKeyPress = false;
                e.Handled = false;
            }

            //if (e.Control == false && e.Alt == false && (e.KeyCode & Keys.KeyCode) != Keys.None)
                //this.replyUserIndex = 0;
            
        }

        void SelectUserNo(int index)
        {
            if (this.Form.UsersToolStrip.Items.Count > index)
            {
                ToolStripButton button = this.Form.UsersToolStrip.Items[index] as ToolStripButton;
                button.Checked = !button.Checked;
            }
        }

        void SelectRoomNo(int index)
        {
            if (this.Form.RoomsToolStrip.Items.Count > index)
            {
                ToolStripButton button = this.Form.RoomsToolStrip.Items[index] as ToolStripButton;
                button.Checked = !button.Checked;
            }
        }

        void AddUserToText(int index)
        {
            if (this.Form.UsersToolStrip.Items.Count > index)
            {
                string userName = this.Form.UsersToolStrip.Items[index].Text;

                string text = this.Form.Editor.Text;

                bool hasSlash = text.StartsWith("/");

                if (hasSlash)
                    text = text.Substring(1);

                text = ReplaceReplyUser.Replace(text, userName+ ": ${text}");

                if (hasSlash)
                    text = "/" + text;

                this.Form.Editor.Text = text;
                this.Form.Editor.SelectionStart = text.Length;
            }            
        }

        int CurrentReplyIndex { get; set; }

        void ReplyDown()
        {
            CheckReplyList();

            this.CurrentReplyIndex++;

            if (this.CurrentReplyIndex >= this.ReplyList.Count)
                this.CurrentReplyIndex = 0;

            this.Form.ReplyUsersMenuStrip.Show(this.Form.Editor, new System.Drawing.Point(0, 0), ToolStripDropDownDirection.AboveRight);
            this.Form.ReplyUsersMenuStrip.Items[this.CurrentReplyIndex].Select();
        }

        void ReplyUp()
        {
            CheckReplyList();

            this.CurrentReplyIndex--;

            if (this.CurrentReplyIndex < 0)
                this.CurrentReplyIndex = this.ReplyList.Count - 1;

            this.Form.ReplyUsersMenuStrip.Show(this.Form.Editor, new Point(0, 0), ToolStripDropDownDirection.AboveRight);
            this.Form.ReplyUsersMenuStrip.Items[this.CurrentReplyIndex].Select();
        }

        List<Post> ReplyList { get; set; }

        void CheckReplyList()
        {
            int currentPostCount = this.Form.GetPlugin<BrowserPlugin>().AllPosts.Count;

            if (currentPostCount > this.LastPostCount)
            {
                this.LastPostCount = currentPostCount;
                BuildReplyList();
            }
        }

        void BuildReplyList()
        {
            this.Form.ReplyUsersMenuStrip.Items.Clear();

            HashSet<string> nameSet = new HashSet<string>();
            this.ReplyList.Clear();

            BrowserPlugin browserPlugin = this.Form.GetPlugin<BrowserPlugin>();
            IList<Post> allPosts = browserPlugin.AllPosts;
            string currentLogin = this.Form.Service.CurrentUser.Login;

            for (int i = allPosts.Count - 1; i >= 0; i--)
            {
                Post replyPost = allPosts[i];

                if (replyPost.From.Login != currentLogin)
                {
                    string id = (replyPost.Private ? "/" : "") + replyPost.From.Login;

                    if (!nameSet.Contains(id))
                    {
                        this.ReplyList.Insert(0, replyPost);
                        nameSet.Add(id);
                    }
                }
            }

            foreach (Post post in this.ReplyList)
            {
                this.Form.ReplyUsersMenuStrip.Items.Add((post.Private ? "/" : "") + post.From.Login);
            }
        }

        void CheckMacro()
        {
            if (this.Form.Editor.Text == "r")
                Reply();
        }

        void Reply()
        {
            BrowserPlugin browserPlugin = this.Form.GetPlugin<BrowserPlugin>();
            IList<Post> allPosts = browserPlugin.AllPosts;
            string currentLogin = this.Form.Service.CurrentUser.Login;

            for (int i = allPosts.Count - 1; i >= 0; i--)
            {
                Post replyPost = allPosts[i];
                
                if (replyPost.From.Login != currentLogin)
                {
                    if (replyPost.Private)
                        this.Form.Editor.Text = "/" + replyPost.From.Login + ":";
                    else
                        this.Form.Editor.Text = replyPost.From.Login + ":";

                    this.Form.Editor.SelectionStart = this.Form.Editor.Text.Length;

                    break;
                }
            }
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
