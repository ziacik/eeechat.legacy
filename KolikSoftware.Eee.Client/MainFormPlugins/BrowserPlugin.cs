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
            ((GeckoElement)this.Browser.Document.Body.LastChild).ScrollIntoView(true);
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

        public void AddMessage(Post post)
        {
            //TODO:
            if (post.From.Login == this.Form.Service.CurrentUser.Login && post.GlobalId == "TEST")
                return;

            this.Form.GetPlugin<LinkFinder>().FindLinksInPost(post);
            this.Form.GetPlugin<SmileFinder>().FindSmilesInPost(post);

            Post appendToPost = null;

            if (this.AllPosts.Count > 0)
            {
                appendToPost = this.AllPosts[this.AllPosts.Count - 1];

                if (appendToPost.From.Login != post.From.Login 
                    || appendToPost.Private != post.Private
                    || appendToPost.Room.Name != post.Room.Name)
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

        private string PostToHtml(Post post)
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
            GeckoPreferences.User["network.proxy.type"] = 1;
            GeckoPreferences.User["network.proxy.http"] = "127.0.0.1";
            GeckoPreferences.User["network.proxy.http_port"] = 3128;
        }

        void Browser_VisibleChanged(object sender, EventArgs e)
        {
        }
    }   
 }
