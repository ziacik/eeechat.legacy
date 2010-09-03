using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KolikSoftware.Eee.Client;
using KolikSoftware.Eee.Client.MainFormPlugins;
using KolikSoftware.Eee.Service.Domain;
using Skybound.Gecko;

namespace KolikSoftware.Eee.GeckoBrowser
{
    public class GeckoBrowserPlugin : IBrowserPlugin
    {
        public MainForm Form { get; set; }
        public GeckoWebBrowser Browser { get; set; }
        public string MessageTemplate { get; set; }
        public string DialogTemplate { get; set; }
        public IList<Post> AllPosts { get; private set; }
        public Dictionary<Post, GeckoElement> ElementsByPost { get; set; }

        public void Init(MainForm mainForm)
        {
            SetupProxy();

            this.AllPosts = new List<Post>();
            this.ElementsByPost = new Dictionary<Post, GeckoElement>();

            this.IsFirstRun = true;

            this.Form = mainForm;
            this.Browser = new GeckoWebBrowser();
            this.Browser.Dock = DockStyle.Fill;
            this.Form.BrowserContainer.Controls.Add(this.Browser);
            this.Browser.DocumentCompleted += new EventHandler(Browser_DocumentCompleted);
            this.Browser.Navigating += new GeckoNavigatingEventHandler(Browser_Navigating);

            this.MessageTemplate = File.ReadAllText(Path.Combine(Application.StartupPath, @"Templates/MessageTemplate.html"));
            this.DialogTemplate = File.ReadAllText(Path.Combine(Application.StartupPath, @"Templates/DialogTemplate.html"));
        }

        void Browser_Navigating(object sender, GeckoNavigatingEventArgs e)
        {
            if (!e.Uri.IsLoopback & e.Uri.AbsolutePath != "blank")
            {
                e.Cancel = true;
                Process.Start(e.Uri.ToString()); //TODO: vulnerability? also, catch exceptions
            }
        }

        public bool CanScroll()
        {
            if (this.ElementsByPost.Count == 0)
                return true;

            GeckoElement lastElement = this.ElementsByPost[this.AllPosts[this.AllPosts.Count - 1]];
            bool isLastInView = lastElement.OffsetTop < (this.Browser.Window.ScrollY + this.Browser.ClientSize.Height);

            return isLastInView;
        }

        public void ScrollDown()
        {
            if (this.AllPosts.Count > 0)
            {
                GeckoElement lastElement = this.ElementsByPost[this.AllPosts[this.AllPosts.Count - 1]];
                lastElement.ScrollIntoView(false);
            }
        }

        void Browser_DocumentCompleted(object sender, EventArgs e)
        {
            if (this.IsRefresh)
            {
                this.IsRefresh = false;

                this.MessageTemplate = File.ReadAllText(Path.Combine(Application.StartupPath, @"Templates/MessageTemplate.html"));

                List<Post> allPosts = new List<Post>();
                allPosts.AddRange(this.AllPosts);
                this.AllPosts.Clear();

                foreach (Post post in allPosts)
                {
                    AddMessage(post, true, true);
                }

                ScrollDown();
            }
            else if (this.IsFirstRun)
            {
                this.IsFirstRun = false;
                string templatePath = Path.Combine(Application.StartupPath, @"Templates\ChatTemplate.html");
                this.Browser.Navigate("file:///" + templatePath.Replace('\\', '/'));
            }
        }

        public bool IsRefresh { get; set; }
        public bool IsFirstRun { get; set; }

        public void Reload()
        {
            this.IsRefresh = true;
            this.Browser.Reload();

        }
        public bool HadRef { get; set; }

        static readonly Regex TextRecipientRegex = new Regex(@"^(\w+):", RegexOptions.Compiled | RegexOptions.Singleline);

        public void AddMessage(Post post, bool initial, bool noResolve)
        {
            //TODO:
            if (!initial && post.From.Login == this.Form.Service.CurrentUser.Login && post.GlobalId == "TEST")
                return;

            Post postToAdd = post;

            if (!noResolve)
            {
                this.Form.GetPlugin<LinkFinder>().FindLinksInPost(post);
                this.Form.GetPlugin<SmileFinder>().FindSmilesInPost(post);
            }

            string conversationRecipient = null;
            string conversationSender = post.From.Login;

            if (post.To != null)
            {
                conversationRecipient = post.To.Login;
            }
            else
            {
                Match match = TextRecipientRegex.Match(post.Text);

                if (match.Success)
                    conversationRecipient = match.Groups[1].Value;
            }

            if (conversationRecipient != null)
            {
                /*User recipient = post.To;

                if (recipient == null)
                    recipient = this.Form.GetPlugin<UserStatePlugin>().GetUser(conversationRecipient);

                if (recipient == null)
                    recipient = new User { Login = conversationRecipient };*/

                Conversation conversation = new Conversation
                {
                    From = post.From,
                    GlobalId = post.GlobalId,
                    Id = post.Id,
                    Room = post.Room,
                    Sent = post.Sent,
                    Text = post.Text,
                    To = post.To
                };

                conversation.Participants.Add(post.From.Login);
                conversation.Participants.Add(conversationRecipient);
                conversation.Posts.Add(post);

                postToAdd = conversation;
            }

            int postIndex = this.AllPosts.Count - 1;
            DateTime timeThreshold = postToAdd.Sent.AddMinutes(-5);

            Conversation referenceConversation = null;

            if (postToAdd is Conversation)
            {
                while (postIndex >= 0)
                {
                    Post referencePost = this.AllPosts[postIndex];
                    postIndex--;

                    if (referencePost.Sent < timeThreshold)
                        break;

                    Conversation conversation = referencePost as Conversation;

                    if (conversation == null)
                        continue;

                    bool isReferencePrivate = conversation.To != null;
                    bool isPostPrivate = postToAdd.To != null;

                    if (isReferencePrivate == isPostPrivate && conversation.Room.Name == postToAdd.Room.Name)
                    {
                        if (conversation.Participants.Contains(conversationRecipient) && conversation.Participants.Contains(conversationSender))
                        {
                            if (conversation.Posts.Count < 8) //TODO: make configurable
                                referenceConversation = conversation;
                        }
                    }

                    break;
                }
            }

            if (referenceConversation != null)
            {
                this.HadRef = true;

                GeckoElement referenceElement = this.ElementsByPost[referenceConversation];
                referenceElement.Parent.RemoveChild(referenceElement);

                Conversation conversation = (Conversation)postToAdd;
                referenceConversation.Posts.AddRange(conversation.Posts);

                referenceConversation.Participants.Add(conversation.From.Login);
                referenceConversation.Sent = conversation.Sent;
                referenceConversation.Text += Environment.NewLine + "-" + Environment.NewLine + "<b>" + conversation.From.Login + "</b>: " + conversation.Text;

                postToAdd = referenceConversation;
            }
            else if (!(postToAdd is Conversation))
            {
                Post appendToPost = null;

                if (this.AllPosts.Count > 0)
                {
                    appendToPost = this.AllPosts[this.AllPosts.Count - 1];

                    string appendToPostRecipient = appendToPost.To != null ? appendToPost.To.Login : null;
                    string postRecipient = postToAdd.ToLogin;

                    if (appendToPost.From.Login != postToAdd.From.Login
                        || appendToPostRecipient != postRecipient
                        || appendToPost.Room.Name != postToAdd.Room.Name
                        || appendToPost is Conversation)
                        appendToPost = null;
                }

                if (appendToPost != null)
                {
                    GeckoElement referenceElement = this.ElementsByPost[appendToPost];
                    referenceElement.Parent.RemoveChild(referenceElement);

                    appendToPost.Text = appendToPost.Text + Environment.NewLine + "-" + Environment.NewLine + postToAdd.Text;
                    postToAdd = appendToPost;
                }
            }

            GeckoElement messageDiv = this.Browser.Document.CreateElement("div");

            if (postToAdd.To != null)
                messageDiv.ClassName = "Message Private Unseen";
            else
                messageDiv.ClassName = "Message Public Unseen";

            string html = PostToHtml(postToAdd);

            messageDiv.InnerHtml = html;

            this.Browser.Document.Body.AppendChild(messageDiv);

            //TODO: this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(messageDiv, postToAdd);

            if (postToAdd == post)
            {
                this.AllPosts.Add(post);
            }
            else
            {
                this.AllPosts.Remove(postToAdd);
                this.AllPosts.Add(postToAdd);
            }

            /// Must use original so that we are able to use SetPostSent.
            this.ElementsByPost[post] = messageDiv;
            this.ElementsByPost[postToAdd] = messageDiv;
        }

        public void SetPostUnseen(Post post)
        {
            GeckoElement element;

            if (this.ElementsByPost.TryGetValue(post, out element))
            {
                foreach (GeckoElement node in element.GetElementsByTagName("div"))
                {
                    node.ClassName = "Unseen";
                }
            }
        }

        public void SetPostSeen(Post post)
        {
            GeckoElement element;

            if (this.ElementsByPost.TryGetValue(post, out element))
            {
                foreach (GeckoElement node in element.GetElementsByTagName("div"))
                {
                    if (node.ClassName == "Unseen")
                        node.ClassName = "";
                }
            }
        }

        public void SetPostPending(Post post)
        {
            GeckoElement element;

            if (this.ElementsByPost.TryGetValue(post, out element))
            {
                foreach (GeckoElement node in element.GetElementsByTagName("div"))
                {
                    if (node.ClassName == "Status")
                        node.ClassName = "Pending";
                }
            }
        }

        public void SetPostSent(Post post)
        {
            GeckoElement element;

            if (this.ElementsByPost.TryGetValue(post, out element))
            {
                foreach (GeckoElement node in element.GetElementsByTagName("div"))
                {
                    if (node.ClassName == "Pending")
                        node.ClassName = "Sent";
                }
            }
        }

        public void UpdatePost(Post post)
        {
            GeckoElement postDiv = this.ElementsByPost[post];
            string html = PostToHtml(post);
            postDiv.InnerHtml = html;

            //TODO: to treba inac, lebo toto urobi viacnasobny resolve pri appende.
            //TODO: this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(postDiv, post);
        }

        string ConversationToHtml(Conversation conversation)
        {
            string participant1 = conversation.Participants.ElementAt<string>(0);
            string participant2 = conversation.Participants.ElementAtOrDefault<string>(1);
            if (participant2 == null)
                participant2 = participant1;
            User user1 = this.Form.GetPlugin<UserStatePlugin>().GetUser(participant1);
            User user2 = this.Form.GetPlugin<UserStatePlugin>().GetUser(participant2);
            int color1 = user1 != null ? user1.Color : 0;
            int color2 = user2 != null ? user2.Color : 0;
            string avatarUrl1 = user1 != null ? "file:///" + user1.ImagePath : "";
            string avatarUrl2 = user2 != null ? "file:///" + user2.ImagePath : "";

            //TODO: should be done some other way. This is for users that are not connected right now.
            if (user1 != null && !File.Exists(user1.ImagePath))
                avatarUrl1 = "http://www.eeechat.net/Avatars/" + user1.Login;

            if (user2 != null && !File.Exists(user2.ImagePath))
                avatarUrl2 = "http://www.eeechat.net/Avatars/" + user2.Login;

            StringBuilder builder = new StringBuilder(this.DialogTemplate);
            builder.Replace("[AvatarUrl1]", avatarUrl1);
            builder.Replace("[AvatarUrl2]", avatarUrl2);
            builder.Replace("[UserName1]", participant1);
            builder.Replace("[UserName2]", participant2);
            builder.Replace("[Time]", conversation.Sent.ToShortTimeString());
            builder.Replace("[Text]", conversation.Text.Replace("\n", "<br />"));
            builder.Replace("[UserColorHex1]", "#" + color1.ToString("x6"));
            builder.Replace("[UserColorHex2]", "#" + color2.ToString("x6"));

            string roomDelimiter;
            string room;

            if (conversation.Room.Name != "Pokec")
            {
                roomDelimiter = "in";
                room = conversation.Room.Name;
            }
            else
            {
                roomDelimiter = "";
                room = "";
            }

            builder.Replace("[RoomInfoDelimiter]", roomDelimiter);
            builder.Replace("[Room]", room);

            string html = builder.ToString();
            return html;
        }

        string PostToHtml(Post post)
        {
            if (post is Conversation)
            {
                return ConversationToHtml((Conversation)post);
            }
            else
            {
                StringBuilder builder = new StringBuilder(this.MessageTemplate);

                var avatarUrl = "file:///" + post.From.ImagePath;

                //TODO: should be done some other way. This is for users that are not connected right now.
                if (!File.Exists(post.From.ImagePath))
                    avatarUrl = "http://www.eeechat.net/Avatars/" + post.From.Login;

                builder.Replace("[AvatarUrl]", avatarUrl);
                builder.Replace("[UserName]", post.From.Login);
                builder.Replace("[Time]", post.Sent.ToShortTimeString());
                builder.Replace("[Text]", post.Text.Replace("\n", "<br />"));
                builder.Replace("[UserColorHex]", "#" + post.From.Color.ToString("x6"));

                string roomDelimiter;
                string room;

                if (post.Room.Name != "Pokec")
                {
                    roomDelimiter = "in";
                    room = post.Room.Name;
                }
                else
                {
                    roomDelimiter = "";
                    room = "";
                }

                builder.Replace("[RoomInfoDelimiter]", roomDelimiter);
                builder.Replace("[Room]", room);

                string html = builder.ToString();
                return html;
            }
        }

        void SetupProxy()
        {
            GeckoPreferences.User["network.proxy.type"] = 5;
            /*            if (!string.IsNullOrEmpty(this.Form.Service.ProxySettings.Server))
                        {
                            string[] server = this.Form.Service.ProxySettings.Server.Split(':');
                            GeckoPreferences.User["network.proxy.http"] = server[0];
                            GeckoPreferences.User["network.proxy.http_port"] = int.Parse(server[1]);


                        }*/
        }

        void Browser_VisibleChanged(object sender, EventArgs e)
        {
        }
    }
}
