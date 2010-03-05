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
            ((GeckoElement)this.Browser.Document.Body.LastChild).ScrollIntoView(false);
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
                    AddMessage(post);
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

        public void Test()
        {
            this.IsRefresh = true;
            this.Browser.Reload();

        }

        public bool HadRef { get; set; }

        public void AddMessage(Post post)
        {
            //TODO:
            if (post.From.Login == this.Form.Service.CurrentUser.Login && post.GlobalId == "TEST")
                return;

            this.Form.GetPlugin<LinkFinder>().FindLinksInPost(post);
            this.Form.GetPlugin<SmileFinder>().FindSmilesInPost(post);

            int postIndex = this.AllPosts.Count - 1;
            DateTime timeThreshold = post.Sent.AddMinutes(-5);

            Post referencePost = null;

            while (postIndex >= 0)
            {
                referencePost = this.AllPosts[postIndex];

                if (referencePost.Sent < timeThreshold)
                {
                    referencePost = null;
                    break;
                }

                if (referencePost is MultiPost)
                {
                    MultiPost multiPost = (MultiPost)referencePost;

                    if (post.Private == multiPost.Private && post.Room.Name == multiPost.Room.Name)
                    {
                        if (post.Text.StartsWith(multiPost.Posts[0].From.Login + ":")
                            || post.Text.StartsWith(multiPost.Posts[1].From.Login + ":"))
                        {
                            if (multiPost.Posts.Count == 8) //TODO: Max configurable
                                referencePost = null;

                            break;
                        }
                    }
                }
                else
                {
                    if (post.Text.StartsWith(referencePost.From.Login + ":")
                        && referencePost.Text.StartsWith(post.From.Login + ":")
                        && post.From.Login != referencePost.From.Login
                        && post.Private == referencePost.Private
                        && post.Room.Name == referencePost.Room.Name)
                        break;
                }

                postIndex--;
            }

            if (referencePost != null)
            {
                this.HadRef = true;

                GeckoElement referenceElement = this.ElementsByPost[referencePost];
                referenceElement.Parent.RemoveChild(referenceElement);
                this.ElementsByPost.Remove(referencePost);
                this.AllPosts.Remove(referencePost);

                MultiPost multiPost = referencePost as MultiPost;

                if (multiPost == null)
                {
                    multiPost = new MultiPost();
                    multiPost.Posts.Add(referencePost);
                    multiPost.From = referencePost.From;
                    multiPost.Room = referencePost.Room;
                    multiPost.Text = "<b>" + referencePost.From.Login + "</b>: " + referencePost.Text;
                    multiPost.Private = referencePost.Private;
                }

                multiPost.Posts.Add(post);
                multiPost.Sent = post.Sent;
                multiPost.Text = multiPost.Text + Environment.NewLine + "-" + Environment.NewLine + "<b>" + post.From.Login + "</b>: " + post.Text;

                GeckoElement messageDiv = this.Browser.Document.CreateElement("div");

                if (multiPost.Private)
                    messageDiv.ClassName = "Message Private";
                else
                    messageDiv.ClassName = "Message Public";

                string html = MultiPostToHtml(multiPost);

                messageDiv.InnerHtml = html;

                this.Browser.Document.Body.AppendChild(messageDiv);

                this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(messageDiv, post);

                this.AllPosts.Add(multiPost);
                this.ElementsByPost[multiPost] = messageDiv;
                this.ElementsByPost[referencePost] = messageDiv;
                this.ElementsByPost[post] = messageDiv;

                return;
            }

            Post appendToPost = null;

            if (this.AllPosts.Count > 0)
            {
                appendToPost = this.AllPosts[this.AllPosts.Count - 1];

                if (appendToPost.From.Login != post.From.Login
                    || appendToPost.Private != post.Private
                    || appendToPost.Room.Name != post.Room.Name
                    || appendToPost is MultiPost)
                    appendToPost = null;
            }

            if (appendToPost != null)
            {
                GeckoElement messageDiv = (GeckoElement)this.Browser.Document.Body.LastChild;

                post.Text = appendToPost.Text + Environment.NewLine + "-" + Environment.NewLine + post.Text;

                string html = PostToHtml(post);

                messageDiv.InnerHtml = html;

                //TODO: to treba inac, lebo toto urobi viacnasobny resolve pri appende.
                this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(messageDiv, post);

                this.AllPosts[this.AllPosts.Count - 1] = post;
                this.ElementsByPost[post] = messageDiv;
            }
            else
            {
                GeckoElement messageDiv = this.Browser.Document.CreateElement("div");

                if (post.Private)
                    messageDiv.ClassName = "Message Private";
                else
                    messageDiv.ClassName = "Message Public";

                string html = PostToHtml(post);

                messageDiv.InnerHtml = html;

                this.Browser.Document.Body.AppendChild(messageDiv);

                this.Form.GetPlugin<LinkResolver>().ResolveLinksIn(messageDiv, post);

                this.AllPosts.Add(post);
                this.ElementsByPost[post] = messageDiv;
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

        string MultiPostToHtml(MultiPost multiPost)
        {
            Post post1 = multiPost.Posts[0];
            Post post2 = multiPost.Posts[1];

            StringBuilder builder = new StringBuilder(this.DialogTemplate);
            builder.Replace("[AvatarUrl1]", "http://www.eeechat.net/Avatars/" + post1.From.Login);
            builder.Replace("[AvatarUrl2]", "http://www.eeechat.net/Avatars/" + post2.From.Login);
            builder.Replace("[UserName1]", post1.From.Login);
            builder.Replace("[UserName2]", post2.From.Login);
            builder.Replace("[Time]", multiPost.Sent.ToShortTimeString());
            builder.Replace("[Text]", multiPost.Text.Replace("\n", "<br />"));
            builder.Replace("[UserColorHex1]", "#" + post1.From.Color.ToString("x6"));
            builder.Replace("[UserColorHex2]", "#" + post2.From.Color.ToString("x6"));

            string roomDelimiter;
            string room;

            if (multiPost.Room.Name != "Pokec")
            {
                roomDelimiter = "in";
                room = multiPost.Room.Name;
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
