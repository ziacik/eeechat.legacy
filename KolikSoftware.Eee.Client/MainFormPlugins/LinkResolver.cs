using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using KolikSoftware.Eee.Service.Core;
using KolikSoftware.Eee.Service.Domain;
using Skybound.Gecko;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class LinkResolver : IMainFormPlugin
    {
        public MainForm Form { get; set; }
        BackgroundWorker LinkResolverWorker { get; set; }
        List<LinkResolverInfo> Links { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            this.Links = new List<LinkResolverInfo>();
            this.LinkResolverWorker = new BackgroundWorker();
            this.LinkResolverWorker.DoWork += new DoWorkEventHandler(LinkResolverWorker_DoWork);
            this.LinkResolverWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LinkResolverWorker_RunWorkerCompleted);
        }

        static readonly Regex TitleRegex = new Regex("<title>(.*?)</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static readonly Regex CharSetRegex = new Regex(@"charset=(.*?)['""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void ResolveLinksIn(GeckoElement element, Post relatedPost)
        {
            foreach (GeckoElement hyperLinkElement in element.GetElementsByTagName("a"))
            {
                //TODO: nejako inac by to trebalo
                if (!string.IsNullOrEmpty(hyperLinkElement.GetAttribute("Id")))
                    this.Links.Add(new LinkResolverInfo() { Element = hyperLinkElement, RelatedPost = relatedPost, Href = hyperLinkElement.GetAttribute("href") });
            }

            CheckStartProcessing();
        }

        void CheckStartProcessing()
        {
            if (!this.LinkResolverWorker.IsBusy && this.Links.Count > 0)
            {
                this.LinkResolverWorker.RunWorkerAsync(this.Links[0]);
                this.Links.RemoveAt(0);
            }
        }

        class LinkResolverInfo
        {
            public GeckoElement Element { get; internal set; }
            public Post RelatedPost { get; set; }
            public string Href { get; internal set; }
            public string Title { get; internal set; }
            public string ImageUrl { get; internal set; }
        }

        void LinkResolverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LinkResolverInfo info = (LinkResolverInfo)e.Argument;
                HttpWebRequest request = RequestFactory.Instance.CreateRequest(new Uri(info.Href), this.Form.Service.ProxySettings);
                
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.ContentType.StartsWith("image/"))
                    {
                        info.ImageUrl = info.Href;
                        e.Result = info;
                    }
                    else if (response.ContentType.StartsWith("text/html"))
                    {
                        ExtractTitle(e, info, response);
                    }
                }
            }
            catch
            {
                /// Do nothing.
            }
        }

        void LinkResolverWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                LinkResolverInfo info = (LinkResolverInfo)e.Result;
                
                if (!string.IsNullOrEmpty(info.Title))
                {
                    info.Element.TextContent = info.Title;
                }
                else if (!string.IsNullOrEmpty(info.ImageUrl))
                {
                    info.Element.InnerHtml = "<img src=\"" + info.ImageUrl + "\" />";
                }

                //info.RelatedPost.Text = 
            }

            CheckStartProcessing();
        }

        string ConvertEncoding(string inputString, Encoding sourceEncoding, Encoding destinationEncoding)
        {
            byte[] sourceBytes = sourceEncoding.GetBytes(inputString);
            byte[] destinationBytes = Encoding.Convert(sourceEncoding, destinationEncoding, sourceBytes);
            return destinationEncoding.GetString(destinationBytes);
        }

        void ExtractTitle(DoWorkEventArgs e, LinkResolverInfo info, HttpWebResponse response)
        {
            Encoding encoding = Encoding.Default;

            if (!string.IsNullOrEmpty(response.CharacterSet))
                encoding = Encoding.GetEncoding(response.CharacterSet);
            else if (!string.IsNullOrEmpty(response.ContentEncoding))
                encoding = Encoding.GetEncoding(response.ContentEncoding);

            using (Stream responseStream = response.GetResponseStream())
            {
                using (BinaryReader reader = new BinaryReader(responseStream))
                {
                    byte[] buffer = reader.ReadBytes(50000);
                    string text = encoding.GetString(buffer);

                    Match match;

                    match = CharSetRegex.Match(text);

                    if (match.Success)
                    {
                        string destinationCharSet = match.Groups[1].Value;
                        encoding = Encoding.GetEncoding(destinationCharSet);
                        text = encoding.GetString(buffer);
                    }

                    match = TitleRegex.Match(text);

                    if (match != null && match.Success)
                    {
                        info.Title = HttpUtility.HtmlDecode(match.Groups[1].Value.Trim());
                        e.Result = info;
                    }
                }
            }
        }
    }
}
