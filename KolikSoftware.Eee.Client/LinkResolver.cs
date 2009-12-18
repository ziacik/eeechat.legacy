using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Net;
using KolikSoftware.Eee.Service;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace KolikSoftware.Eee.Client
{
    public partial class LinkResolver : Component
    {
        public event EventHandler<LinkResolverEventArgs> LinkResolved;

        protected virtual void OnLinkResolved(LinkResolverEventArgs e)
        {
            EventHandler<LinkResolverEventArgs> handler = this.LinkResolved;
            if (handler != null) handler(this, e);
        }

        List<LinkResolverEventArgs> Links { get; set; }
        public ProxySettings ProxySettings { get; set; }

        public LinkResolver()
        {
            InitializeComponent();
            this.Links = new List<LinkResolverEventArgs>();
        }

        public LinkResolver(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.Links = new List<LinkResolverEventArgs>();
        }

        public void AddLink(string id, string href)
        {
            this.Links.Add(new LinkResolverEventArgs() { Id = id, Href = href });
            CheckStartProcessing();
        }

        void CheckStartProcessing()
        {
            if (!this.linkResolverWorker.IsBusy && this.Links.Count > 0)
            {
                this.linkResolverWorker.RunWorkerAsync(this.Links[0]);
                this.Links.RemoveAt(0);
            }
        }

        static readonly Regex TitleRegex = new Regex("<title>(.*)</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static readonly Regex CharSetRegex = new Regex(@"charset=(.*?)['""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        void linkResolverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LinkResolverEventArgs info = (LinkResolverEventArgs)e.Argument;

                HttpWebRequest request = RequestFactory.Instance.CreateRequest(new Uri(info.Href), this.ProxySettings);

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

        static string ConvertEncoding(string inputString, Encoding sourceEncoding, Encoding destinationEncoding)
        {
            byte[] sourceBytes = sourceEncoding.GetBytes(inputString);
            byte[] destinationBytes = Encoding.Convert(sourceEncoding, destinationEncoding, sourceBytes);
            return destinationEncoding.GetString(destinationBytes);
        }

        void ExtractTitle(DoWorkEventArgs e, LinkResolverEventArgs info, HttpWebResponse response)
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
                        info.Title = match.Groups[1].Value.Trim().Replace("&nbsp", " ");
                        e.Result = info;
                    }
                }
            }
        }

        void linkResolverWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                LinkResolverEventArgs info = (LinkResolverEventArgs)e.Result;
                OnLinkResolved(info);
            }

            CheckStartProcessing();
        }
    }

    public class LinkResolverEventArgs : EventArgs
    {
        public string Id { get; internal set; }
        public string Href { get; internal set; }
        public string Title { get; internal set; }
        public string ImageUrl { get; internal set; }
    }
}
