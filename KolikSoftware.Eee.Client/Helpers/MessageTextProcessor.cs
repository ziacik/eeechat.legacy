using System;
using System.Collections.Generic;
using System.Text;
using KolikSoftware.Eee.Service;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Reflection;
using KolikSoftware.Eee.Client.Media;

namespace KolikSoftware.Eee.Client.Helpers
{
    public class MessageTextProcessor
    {
        static readonly MessageTextProcessor instance = new MessageTextProcessor();

        XslCompiledTransform transformation = new XslCompiledTransform();
        Dictionary<string, object> domains = new Dictionary<string, object>();

        private MessageTextProcessor()
        {
            LoadTemplate();
            SetupDomains();
        }

        public static MessageTextProcessor Instance
        {
            get
            {
                return MessageTextProcessor.instance;
            }
        }

        public void LoadTemplate()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(GetTemplate());

            this.transformation.Load(document);
        }

        public XmlDocument ProcessMessages(EeeDataSet.MessageDataTable messages, List<string> links)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                messages.WriteXml(stream);

                stream.Flush();
                stream.Position = 0;

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("eee", messages.Namespace);

                XmlDocument document = new XmlDocument();
                document.Load(stream);

                XmlNodeList list = document.SelectNodes("/eee:EeeDataSet/eee:Message/eee:Message", nsmgr);

                string conversion;

                foreach (XmlNode node in list)
                {
                    conversion = ConvertHyperLinks(node.InnerXml);
                    conversion = Smilies.Instance.ConvertSmilies(conversion);
                    conversion = conversion.Replace("\n", "<br xmlns=\"\"/>\n");
                    node.InnerXml = conversion;

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.LocalName == "a")
                        {
                            string href = childNode.Attributes["href"].Value;

                            if (href.Contains("#MEDIA"))
                            {
                                href = href.Substring(0, href.Length - "#MEDIA".Length);
                                if (!links.Contains(href))
                                    links.Add(href);
                            }
                        }
                    }
                }

                using (MemoryStream outputStream = new MemoryStream())
                {
                    this.transformation.Transform(document, null, outputStream);

                    outputStream.Position = 0;

                    StreamReader r = new StreamReader(outputStream);
                    string x = r.ReadToEnd();

                    outputStream.Position = 0;

                    XmlDocument outputDocument = new XmlDocument();
                    outputDocument.Load(outputStream);

                    return outputDocument;
                }
            }
        }

        static readonly Regex HttpUrls = new Regex(
            @"(?<=(^|[^\w@]))(?<link>" +
            @"(\w+[:]//)?" + // Protocol
            @"(?<server>" +
            @"((" +
            @"([\w-]+[.]){1,}" + // Sub Domains, at least one
            @"[a-zA-Z]{2,4}" + // Top Domain
            @")|(" +
            @"[0-9]+[.][0-9]+[.][0-9]+[.][0-9]+" + // IP Address
            @"))" +
            @")" +
            @"([:][0-9]*)?" + // Port Number
            @"([?/][\w~#-;?@&=/.+]*)?" + // Path and Params
            @")(?=($|[^\w]))",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        static readonly Regex MailUrls = new Regex(@"(?<mail>\w*([.]\w*)?@\w*[.]\w*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        string ConvertHyperLinks(string text)
        {
            if (text.IndexOf('.') >= 0)
            {
                text = MessageTextProcessor.HttpUrls.Replace(text, EvaluateHyperlinkMatch);
                text = MessageTextProcessor.MailUrls.Replace(text, @"<a style=""text-decoration:none;"" xmlns="""" href=""mailto:${mail}"">[<img border=""0"" src=""" + MessageTextProcessor.MailPicPath + @""" /> ${mail}]</a>");
            }
            return text;
        }

        static string UrlPicPath = "file://" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Url.gif");
        static string MailPicPath = "file://" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Mail.gif");
        static string MediaPicPath = "file://" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Media.gif");

        string EvaluateHyperlinkMatch(Match match)
        {
            string server = match.Groups["server"].Value;
            string domain = server.Substring(server.LastIndexOf('.') + 1);
            if (this.domains.ContainsKey(domain) == false)
                return match.Value;

            string link = match.Groups["link"].Value;
            if (link.IndexOf("://") < 0)
                link = "http://" + link;

            string ext = link.Substring(link.Length - 4, 4).ToLower();

            if (ext == ".mp3" || ext == ".wma" || ext == ".m4a")
            {
                string title = Path.GetFileName(link);

                return @"<a style=""text-decoration:none"" xmlns="""" href=""" + link + @"#MEDIA"">[<img border=""0"" src=""" + MessageTextProcessor.MediaPicPath + @""" /> " + title + "]</a>";
            }
            else
            {
                return @"<a id=""" + Guid.NewGuid().ToString() + @""" style=""text-decoration:none"" xmlns="""" href=""" + link + @""">[<img border=""0"" src=""" + MessageTextProcessor.UrlPicPath + @""" /> " + server + "]</a>";
            }
        }

        string GetTemplate()
        {
            string templateName = "MessageTemplate" + Properties.Settings.Default.MessageTemplateNo;

            if (templateName == null || templateName.Trim().Length == 0)
                throw new InvalidOperationException("Message template is not set in configuration.");

            string template = Properties.Resources.ResourceManager.GetString(templateName);

            if (template == null || template.Trim().Length == 0)
                throw new InvalidOperationException("Message template does not exist in resources or it is empty.");

            return template;
        }

        #region Domains
        void SetupDomains()
        {
            this.domains.Add("ac", null);
            this.domains.Add("ad", null);
            this.domains.Add("ae", null);
            this.domains.Add("af", null);
            this.domains.Add("ag", null);
            this.domains.Add("ai", null);
            this.domains.Add("al", null);
            this.domains.Add("am", null);
            this.domains.Add("an", null);
            this.domains.Add("ao", null);
            this.domains.Add("aq", null);
            this.domains.Add("ar", null);
            this.domains.Add("as", null);
            this.domains.Add("at", null);
            this.domains.Add("au", null);
            this.domains.Add("aw", null);
            this.domains.Add("az", null);
            this.domains.Add("ba", null);
            this.domains.Add("bb", null);
            this.domains.Add("bd", null);
            this.domains.Add("be", null);
            this.domains.Add("bf", null);
            this.domains.Add("bg", null);
            this.domains.Add("bh", null);
            this.domains.Add("bi", null);
            this.domains.Add("bj", null);
            this.domains.Add("bm", null);
            this.domains.Add("bn", null);
            this.domains.Add("bo", null);
            this.domains.Add("br", null);
            this.domains.Add("bs", null);
            this.domains.Add("bt", null);
            this.domains.Add("bv", null);
            this.domains.Add("bw", null);
            this.domains.Add("by", null);
            this.domains.Add("bz", null);
            this.domains.Add("ca", null);
            this.domains.Add("cc", null);
            this.domains.Add("cd", null);
            this.domains.Add("cf", null);
            this.domains.Add("cg", null);
            this.domains.Add("ch", null);
            this.domains.Add("ci", null);
            this.domains.Add("ck", null);
            this.domains.Add("cl", null);
            this.domains.Add("cm", null);
            this.domains.Add("cn", null);
            this.domains.Add("co", null);
            this.domains.Add("com", null);
            this.domains.Add("cr", null);
            this.domains.Add("cs", null);
            this.domains.Add("cu", null);
            this.domains.Add("cv", null);
            this.domains.Add("cx", null);
            this.domains.Add("cy", null);
            this.domains.Add("cz", null);
            this.domains.Add("de", null);
            this.domains.Add("dj", null);
            this.domains.Add("dk", null);
            this.domains.Add("dm", null);
            this.domains.Add("do", null);
            this.domains.Add("dz", null);
            this.domains.Add("ec", null);
            this.domains.Add("edu", null);
            this.domains.Add("ee", null);
            this.domains.Add("eg", null);
            this.domains.Add("eh", null);
            this.domains.Add("er", null);
            this.domains.Add("es", null);
            this.domains.Add("et", null);
            this.domains.Add("eu", null);
            this.domains.Add("fi", null);
            this.domains.Add("fj", null);
            this.domains.Add("fk", null);
            this.domains.Add("fm", null);
            this.domains.Add("fo", null);
            this.domains.Add("fr", null);
            this.domains.Add("ga", null);
            this.domains.Add("gd", null);
            this.domains.Add("ge", null);
            this.domains.Add("gf", null);
            this.domains.Add("gg", null);
            this.domains.Add("gh", null);
            this.domains.Add("gi", null);
            this.domains.Add("gl", null);
            this.domains.Add("gm", null);
            this.domains.Add("gn", null);
            this.domains.Add("gov", null);
            this.domains.Add("gp", null);
            this.domains.Add("gq", null);
            this.domains.Add("gr", null);
            this.domains.Add("gs", null);
            this.domains.Add("gt", null);
            this.domains.Add("gu", null);
            this.domains.Add("gw", null);
            this.domains.Add("gy", null);
            this.domains.Add("hk", null);
            this.domains.Add("hm", null);
            this.domains.Add("hn", null);
            this.domains.Add("hr", null);
            this.domains.Add("ht", null);
            this.domains.Add("hu", null);
            this.domains.Add("id", null);
            this.domains.Add("ie", null);
            this.domains.Add("il", null);
            this.domains.Add("im", null);
            this.domains.Add("in", null);
            this.domains.Add("int", null);
            this.domains.Add("io", null);
            this.domains.Add("iq", null);
            this.domains.Add("ir", null);
            this.domains.Add("is", null);
            this.domains.Add("it", null);
            this.domains.Add("je", null);
            this.domains.Add("jm", null);
            this.domains.Add("jo", null);
            this.domains.Add("jp", null);
            this.domains.Add("ke", null);
            this.domains.Add("kg", null);
            this.domains.Add("kh", null);
            this.domains.Add("ki", null);
            this.domains.Add("km", null);
            this.domains.Add("kn", null);
            this.domains.Add("kp", null);
            this.domains.Add("kr", null);
            this.domains.Add("kw", null);
            this.domains.Add("ky", null);
            this.domains.Add("kz", null);
            this.domains.Add("la", null);
            this.domains.Add("lb", null);
            this.domains.Add("lc", null);
            this.domains.Add("li", null);
            this.domains.Add("lk", null);
            this.domains.Add("lr", null);
            this.domains.Add("ls", null);
            this.domains.Add("lt", null);
            this.domains.Add("lu", null);
            this.domains.Add("lv", null);
            this.domains.Add("ly", null);
            this.domains.Add("ma", null);
            this.domains.Add("mc", null);
            this.domains.Add("md", null);
            this.domains.Add("mg", null);
            this.domains.Add("mh", null);
            this.domains.Add("mil", null);
            this.domains.Add("mk", null);
            this.domains.Add("ml", null);
            this.domains.Add("mm", null);
            this.domains.Add("mn", null);
            this.domains.Add("mo", null);
            this.domains.Add("mp", null);
            this.domains.Add("mq", null);
            this.domains.Add("mr", null);
            this.domains.Add("ms", null);
            this.domains.Add("mt", null);
            this.domains.Add("mu", null);
            this.domains.Add("mv", null);
            this.domains.Add("mw", null);
            this.domains.Add("mx", null);
            this.domains.Add("my", null);
            this.domains.Add("mz", null);
            this.domains.Add("na", null);
            this.domains.Add("nc", null);
            this.domains.Add("ne", null);
            this.domains.Add("net", null);
            this.domains.Add("nf", null);
            this.domains.Add("ng", null);
            this.domains.Add("ni", null);
            this.domains.Add("nl", null);
            this.domains.Add("no", null);
            this.domains.Add("np", null);
            this.domains.Add("nr", null);
            this.domains.Add("nt", null);
            this.domains.Add("nu", null);
            this.domains.Add("nz", null);
            this.domains.Add("om", null);
            this.domains.Add("org", null);
            this.domains.Add("pa", null);
            this.domains.Add("pe", null);
            this.domains.Add("pf", null);
            this.domains.Add("pg", null);
            this.domains.Add("ph", null);
            this.domains.Add("pk", null);
            this.domains.Add("pl", null);
            this.domains.Add("pm", null);
            this.domains.Add("pn", null);
            this.domains.Add("pr", null);
            this.domains.Add("ps", null);
            this.domains.Add("pt", null);
            this.domains.Add("pw", null);
            this.domains.Add("py", null);
            this.domains.Add("qa", null);
            this.domains.Add("re", null);
            this.domains.Add("ro", null);
            this.domains.Add("ru", null);
            this.domains.Add("rw", null);
            this.domains.Add("sa", null);
            this.domains.Add("sb", null);
            this.domains.Add("sc", null);
            this.domains.Add("sd", null);
            this.domains.Add("se", null);
            this.domains.Add("sg", null);
            this.domains.Add("sh", null);
            this.domains.Add("si", null);
            this.domains.Add("sj", null);
            this.domains.Add("sk", null);
            this.domains.Add("sl", null);
            this.domains.Add("sm", null);
            this.domains.Add("sn", null);
            this.domains.Add("so", null);
            this.domains.Add("sr", null);
            this.domains.Add("sv", null);
            this.domains.Add("st", null);
            this.domains.Add("sy", null);
            this.domains.Add("sz", null);
            this.domains.Add("tc", null);
            this.domains.Add("td", null);
            this.domains.Add("tf", null);
            this.domains.Add("tg", null);
            this.domains.Add("th", null);
            this.domains.Add("tj", null);
            this.domains.Add("tk", null);
            this.domains.Add("tm", null);
            this.domains.Add("tn", null);
            this.domains.Add("to", null);
            this.domains.Add("tp", null);
            this.domains.Add("tr", null);
            this.domains.Add("tt", null);
            this.domains.Add("tv", null);
            this.domains.Add("tw", null);
            this.domains.Add("tz", null);
            this.domains.Add("ua", null);
            this.domains.Add("ug", null);
            this.domains.Add("uk", null);
            this.domains.Add("um", null);
            this.domains.Add("us", null);
            this.domains.Add("uy", null);
            this.domains.Add("uz", null);
            this.domains.Add("va", null);
            this.domains.Add("vc", null);
            this.domains.Add("ve", null);
            this.domains.Add("vg", null);
            this.domains.Add("vi", null);
            this.domains.Add("vn", null);
            this.domains.Add("vu", null);
            this.domains.Add("wf", null);
            this.domains.Add("ws", null);
            this.domains.Add("ye", null);
            this.domains.Add("yt", null);
            this.domains.Add("yu", null);
            this.domains.Add("za", null);
            this.domains.Add("zm", null);
            this.domains.Add("zw", null);
            this.domains.Add("aero", null);
            this.domains.Add("biz", null);
            this.domains.Add("coop", null);
            this.domains.Add("info", null);
            this.domains.Add("museum", null);
            this.domains.Add("name", null);
            this.domains.Add("pro", null);
            this.domains.Add("travel", null);
        }
        #endregion
    }
}
