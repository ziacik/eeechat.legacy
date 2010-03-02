using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class UserStatePlugin : IMainFormPlugin
    { 
        Dictionary<string, User> UsersByName { get; set; }
        Dictionary<ToolStripButton, User> UsersByButton { get; set; }
        Dictionary<string, ToolStripButton> ButtonsByUserName { get; set; }

        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;

            this.UsersByButton = new Dictionary<ToolStripButton, User>();
            this.UsersByName = new Dictionary<string, User>();
            this.ButtonsByUserName = new Dictionary<string, ToolStripButton>();
        }

        public void SetUsers(IList<User> users)
        {
            this.UsersByName.Clear();

            foreach (User user in users)
            {
                this.UsersByName.Add(user.Login, user);
            }

            SynchronizeUsers();
        }

        public void SetUser(User user)
        {
            User existingUser;

            if (this.UsersByName.TryGetValue(user.Login, out existingUser))
            {
                existingUser.Client = user.Client;
                existingUser.Comment = user.Comment;
                existingUser.State = user.State;
            }
            else
            {
                this.UsersByName[user.Login] = user;
            }
            
            SynchronizeUsers();
        }

        public void SetUsersToPosts(IList<Post> posts)
        {
            foreach (Post post in posts)
            {
                User from;

                if (this.UsersByName.TryGetValue(post.From.Login, out from))
                    post.From = from;
            }
        }

        public void SynchronizeUsers()
        {
            foreach (User user in this.UsersByName.Values)
            {
                ToolStripButton userButton;                
                this.ButtonsByUserName.TryGetValue(user.Login, out userButton);                

                if (user.State == UserState.Disconnected)
                {
                    if (userButton != null)
                        RemoveUserButton(user);
                }
                else
                {
                    if (userButton == null)
                        AddUserButton(user);
                    else
                        UpdateUserButton(user);
                }
            }

            UpdateUserIcons();
        }

        void UpdateUserIcons()
        {
            int index = 0;

            foreach (ToolStripItem item in this.Form.UsersToolStrip.Items)
            {
                if (index >= 10) return;
                item.ImageIndex = index++;
            }
        }

        void UpdateUserButton(User user)
        {
            ToolStripButton button = this.ButtonsByUserName[user.Login];

            if (user.State == UserState.Away)
                button.ForeColor = Color.Red;
            else
                button.ForeColor = Color.Black;
        }

        void AddUserButton(User user)
        {
            ToolStripButton button = new ToolStripButton(user.Login);
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.CheckOnClick = true;
            button.CheckedChanged += new EventHandler(button_CheckedChanged);
            button.MouseUp += new MouseEventHandler(button_MouseUp);

            if (user.Login == this.Form.Service.CurrentUser.Login)
                this.Form.UsersToolStrip.Items.Insert(0, button);
            else
                this.Form.UsersToolStrip.Items.Add(button);

            //TODO: this.notificationManager.AddNotification("Connected", 0, user.Login, MessageType.Connection);

            button.MouseHover += new EventHandler(button_MouseHover);
            button.MouseLeave += new EventHandler(button_MouseLeave);

            this.UsersByButton[button] = user;
            this.ButtonsByUserName[user.Login] = button;

            UpdateUserButton(user);
        }

        void button_MouseLeave(object sender, EventArgs e)
        {
        }

        void button_MouseHover(object sender, EventArgs e)
        {
        }

        void button_MouseUp(object sender, MouseEventArgs e)
        {
        }

        void button_CheckedChanged(object sender, EventArgs e)
        {
        }

        void RemoveUserButton(User user)
        {
            using (ToolStripButton button = this.ButtonsByUserName[user.Login])
            {
                this.Form.UsersToolStrip.Items.Remove(button);
                this.ButtonsByUserName.Remove(user.Login);
                this.UsersByButton.Remove(button);
            }
            //TODO: notification
            //this.notificationManager.AddNotification("Disconnected", 0, user.Login, MessageType.Connection);
        }

        //TODO: To be removed
        public void SetUserFromCommand(Post post)
        {
            string userCommand = post.Text;

            Debug.WriteLine(post.Sent + ": " + post.Text);
            
            int paramsLen = userCommand.Length - "STATE".Length - 3;
            string[] paramList = userCommand.Substring("STATE".Length + 2, paramsLen).Split(';');

            post.From.State = (UserState)int.Parse(paramList[1]);
            post.From.Comment = paramList[2];

            if (paramList.Length > 3)
                post.From.Client = paramList[3];

            SetUser(post.From);
        }

        public User GetUser(string userName)
        {
            User user = null;

            if (!string.IsNullOrEmpty(userName))
                this.UsersByName.TryGetValue(userName, out user);

            return user;       
        }
    }
}
