using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybound.Gecko;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using KolikSoftware.Eee.Service.Domain;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class BrowserPlugin : IMainFormPlugin
    {
        public MainForm Form { get; set; }
        public GeckoWebBrowser Browser { get; set; }
        public string MessageTemplate { get; set; }
        public string DialogTemplate { get; set; }
        public List<Post> AllPosts { get; set; }
        public Dictionary<Post, GeckoElement> ElementsByPost { get; set; }

        public void Init(MainForm mainForm)
        {
            SetupProxy();

            this.AllPosts = new List<Post>();
            this.ElementsByPost = new Dictionary<Post, GeckoElement>();

            this.IsFirstRun = true;

            this.Form = mainForm;
            this.Browser = this.Form.Browser;            
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

#if DEBUG
        public void Reload()
        {
            this.IsRefresh = true;
            this.Browser.Reload();

        }
#endif

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

                            break;
                        }
                    }
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
                messageDiv.ClassName = "Message Private";
            else
                messageDiv.ClassName = "Message Public";

            string html = PostToHtml(postToAdd);

            messageDiv.InnerHtml = html;

            this.Browser.Document.Body.AppendChild(messageDiv);

            this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(messageDiv, postToAdd);

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
            this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(postDiv, post);
        }

        string ConversationToHtml(Conversation conversation)
        {
            string participant1 = conversation.Participants.ElementAt<string>(0);
            string participant2 = conversation.Participants.ElementAt<string>(1);
            User user1 = this.Form.GetPlugin<UserStatePlugin>().GetUser(participant1);
            User user2 = this.Form.GetPlugin<UserStatePlugin>().GetUser(participant2);
            int color1 = user1 != null ? user1.Color : 0;
            int color2 = user2 != null ? user2.Color : 0;

            StringBuilder builder = new StringBuilder(this.DialogTemplate);
            builder.Replace("[AvatarUrl1]", "http://www.eeechat.net/Avatars/" + participant1);
            builder.Replace("[AvatarUrl2]", "http://www.eeechat.net/Avatars/" + participant2);
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
                builder.Replace("[AvatarUrl]", "http://www.eeechat.net/Avatars/" + post.From.Login);
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
