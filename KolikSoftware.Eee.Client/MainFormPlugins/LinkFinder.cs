using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Web;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class LinkFinder : IMainFormPlugin
    {
        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            InitDomains();
        }


        public void FindLinksInPost(Post post)
        {
            post.Text = ConvertHyperLinks(post.Text);
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
            @"([?/][\w!~#-;?@&=/.+]*)?" + // Path and Params
            @")(?=($|[^\w]))",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        static readonly Regex MailUrls = new Regex(@"(?<mail>\w*([.]\w*)?@\w*[.]\w*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        static readonly Regex YouTubeRegex = new Regex(@"youtube\.com/watch\?v=([^&]+)", RegexOptions.Compiled);

        string ConvertHyperLinks(string text)
        {
            if (text.IndexOf('.') >= 0)
            {
                text = LinkFinder.HttpUrls.Replace(text, EvaluateHyperlinkMatch);
                text = LinkFinder.MailUrls.Replace(text, @"<a href=""mailto:${mail}"">${mail}</a>");
            }
            return text;
        }

        static string UrlPicPath = "file://" + Path.Combine(Application.StartupPath, @"Resources\Url.gif");
        static string MailPicPath = "file://" + Path.Combine(Application.StartupPath, @"Resources\Mail.gif");
        static string MediaPicPath = "file://" + Path.Combine(Application.StartupPath, @"Resources\Media.gif");
        static string PlayerPath = "file://" + Path.Combine(Application.StartupPath, @"Templates\MusicPlayer.swf");

        string EvaluateHyperlinkMatch(Match match)
        {
            string server = match.Groups["server"].Value;
            string domain = server.Substring(server.LastIndexOf('.') + 1);
            if (this.domains.Contains(domain) == false)
                return match.Value;

            string link = match.Groups["link"].Value;
            if (link.IndexOf("://") < 0)
                link = "http://" + link;

            string extraEnd = "";

            if (link.EndsWith("!"))
            {
                link = link.Substring(0, link.Length - 1);
                extraEnd = "!";
            }

            string ext = link.Substring(link.Length - 4, 4).ToLower();

            if (ext == ".mp3" || ext == ".wma" || ext == ".m4a")
            {
                string title = Path.GetFileName(link);

                string template =
                @"
                    <object type=""application/x-shockwave-flash"" data=""{0}/MusicPlayer.swf"" width=""300"" height=""20"">
                        <param name=""movie"" value=""{0}/MusicPlayer.swf"" />
                        <param name=""bgcolor"" value=""#ffffff"" />
                        <param name=""FlashVars"" value=""mp3={1}&amp;width=300&amp;showstop=1&amp;showinfo=1&amp;showvolume=1"" />
                    </object>
                ";

//                string template = @"
//                    <embed type=""application/x-shockwave-flash"" src=""Templates/MusicPlayer.swf"" width=""200"" height=""20"">
//                         <param name=""movie"" value=""Templates/MusicPlayer.swf"" />
//                         <param name=""FlashVars"" value=""{0}"" />
//                    </embed>";

                return string.Format(template, this.Form.Service.ServiceUrl, HttpUtility.UrlEncode(link)) + extraEnd;
                //return string.Format(@"<a id=""Media.{0}"" href=""{1}"">{2}</a>", Guid.NewGuid(), link, title) + extraEnd;
            }
            else
            {
                Match youTubeMatch = YouTubeRegex.Match(link);

                if (youTubeMatch.Success)
                {
                    string videoId = youTubeMatch.Groups[1].Value;
                    return string.Format(@"<embed class=""FitSize"" src=""http://www.youtube.com/v/{0}"" type=""application/x-shockwave-flash"" allowscriptaccess=""always"" allowfullscreen=""true""></embed>", videoId);
                }
                else
                {
                    return string.Format(@"<a id=""{0}"" href=""{1}"">{2}</a>", Guid.NewGuid(), link, server) + extraEnd;
                }
            }
        }

        #region Domains
        HashSet<string> domains = new HashSet<string>();

        void InitDomains()
        {
            this.domains.Add("ac");
            this.domains.Add("ad");
            this.domains.Add("ae");
            this.domains.Add("af");
            this.domains.Add("ag");
            this.domains.Add("ai");
            this.domains.Add("al");
            this.domains.Add("am");
            this.domains.Add("an");
            this.domains.Add("ao");
            this.domains.Add("aq");
            this.domains.Add("ar");
            this.domains.Add("as");
            this.domains.Add("at");
            this.domains.Add("au");
            this.domains.Add("aw");
            this.domains.Add("az");
            this.domains.Add("ba");
            this.domains.Add("bb");
            this.domains.Add("bd");
            this.domains.Add("be");
            this.domains.Add("bf");
            this.domains.Add("bg");
            this.domains.Add("bh");
            this.domains.Add("bi");
            this.domains.Add("bj");
            this.domains.Add("bm");
            this.domains.Add("bn");
            this.domains.Add("bo");
            this.domains.Add("br");
            this.domains.Add("bs");
            this.domains.Add("bt");
            this.domains.Add("bv");
            this.domains.Add("bw");
            this.domains.Add("by");
            this.domains.Add("bz");
            this.domains.Add("ca");
            this.domains.Add("cc");
            this.domains.Add("cd");
            this.domains.Add("cf");
            this.domains.Add("cg");
            this.domains.Add("ch");
            this.domains.Add("ci");
            this.domains.Add("ck");
            this.domains.Add("cl");
            this.domains.Add("cm");
            this.domains.Add("cn");
            this.domains.Add("co");
            this.domains.Add("com");
            this.domains.Add("cr");
            this.domains.Add("cs");
            this.domains.Add("cu");
            this.domains.Add("cv");
            this.domains.Add("cx");
            this.domains.Add("cy");
            this.domains.Add("cz");
            this.domains.Add("de");
            this.domains.Add("dj");
            this.domains.Add("dk");
            this.domains.Add("dm");
            this.domains.Add("do");
            this.domains.Add("dz");
            this.domains.Add("ec");
            this.domains.Add("edu");
            this.domains.Add("ee");
            this.domains.Add("eg");
            this.domains.Add("eh");
            this.domains.Add("er");
            this.domains.Add("es");
            this.domains.Add("et");
            this.domains.Add("eu");
            this.domains.Add("fi");
            this.domains.Add("fj");
            this.domains.Add("fk");
            this.domains.Add("fm");
            this.domains.Add("fo");
            this.domains.Add("fr");
            this.domains.Add("ga");
            this.domains.Add("gd");
            this.domains.Add("ge");
            this.domains.Add("gf");
            this.domains.Add("gg");
            this.domains.Add("gh");
            this.domains.Add("gi");
            this.domains.Add("gl");
            this.domains.Add("gm");
            this.domains.Add("gn");
            this.domains.Add("gov");
            this.domains.Add("gp");
            this.domains.Add("gq");
            this.domains.Add("gr");
            this.domains.Add("gs");
            this.domains.Add("gt");
            this.domains.Add("gu");
            this.domains.Add("gw");
            this.domains.Add("gy");
            this.domains.Add("hk");
            this.domains.Add("hm");
            this.domains.Add("hn");
            this.domains.Add("hr");
            this.domains.Add("ht");
            this.domains.Add("hu");
            this.domains.Add("id");
            this.domains.Add("ie");
            this.domains.Add("il");
            this.domains.Add("im");
            this.domains.Add("in");
            this.domains.Add("int");
            this.domains.Add("io");
            this.domains.Add("iq");
            this.domains.Add("ir");
            this.domains.Add("is");
            this.domains.Add("it");
            this.domains.Add("je");
            this.domains.Add("jm");
            this.domains.Add("jo");
            this.domains.Add("jp");
            this.domains.Add("ke");
            this.domains.Add("kg");
            this.domains.Add("kh");
            this.domains.Add("ki");
            this.domains.Add("km");
            this.domains.Add("kn");
            this.domains.Add("kp");
            this.domains.Add("kr");
            this.domains.Add("kw");
            this.domains.Add("ky");
            this.domains.Add("kz");
            this.domains.Add("la");
            this.domains.Add("lb");
            this.domains.Add("lc");
            this.domains.Add("li");
            this.domains.Add("lk");
            this.domains.Add("lr");
            this.domains.Add("ls");
            this.domains.Add("lt");
            this.domains.Add("lu");
            this.domains.Add("lv");
            this.domains.Add("ly");
            this.domains.Add("ma");
            this.domains.Add("mc");
            this.domains.Add("md");
            this.domains.Add("mg");
            this.domains.Add("mh");
            this.domains.Add("mil");
            this.domains.Add("mk");
            this.domains.Add("ml");
            this.domains.Add("mm");
            this.domains.Add("mn");
            this.domains.Add("mo");
            this.domains.Add("mp");
            this.domains.Add("mq");
            this.domains.Add("mr");
            this.domains.Add("ms");
            this.domains.Add("mt");
            this.domains.Add("mu");
            this.domains.Add("mv");
            this.domains.Add("mw");
            this.domains.Add("mx");
            this.domains.Add("my");
            this.domains.Add("mz");
            this.domains.Add("na");
            this.domains.Add("nc");
            this.domains.Add("ne");
            this.domains.Add("net");
            this.domains.Add("nf");
            this.domains.Add("ng");
            this.domains.Add("ni");
            this.domains.Add("nl");
            this.domains.Add("no");
            this.domains.Add("np");
            this.domains.Add("nr");
            this.domains.Add("nt");
            this.domains.Add("nu");
            this.domains.Add("nz");
            this.domains.Add("om");
            this.domains.Add("org");
            this.domains.Add("pa");
            this.domains.Add("pe");
            this.domains.Add("pf");
            this.domains.Add("pg");
            this.domains.Add("ph");
            this.domains.Add("pk");
            this.domains.Add("pl");
            this.domains.Add("pm");
            this.domains.Add("pn");
            this.domains.Add("pr");
            this.domains.Add("ps");
            this.domains.Add("pt");
            this.domains.Add("pw");
            this.domains.Add("py");
            this.domains.Add("qa");
            this.domains.Add("re");
            this.domains.Add("ro");
            this.domains.Add("ru");
            this.domains.Add("rw");
            this.domains.Add("sa");
            this.domains.Add("sb");
            this.domains.Add("sc");
            this.domains.Add("sd");
            this.domains.Add("se");
            this.domains.Add("sg");
            this.domains.Add("sh");
            this.domains.Add("si");
            this.domains.Add("sj");
            this.domains.Add("sk");
            this.domains.Add("sl");
            this.domains.Add("sm");
            this.domains.Add("sn");
            this.domains.Add("so");
            this.domains.Add("sr");
            this.domains.Add("sv");
            this.domains.Add("st");
            this.domains.Add("sy");
            this.domains.Add("sz");
            this.domains.Add("tc");
            this.domains.Add("td");
            this.domains.Add("tf");
            this.domains.Add("tg");
            this.domains.Add("th");
            this.domains.Add("tj");
            this.domains.Add("tk");
            this.domains.Add("tm");
            this.domains.Add("tn");
            this.domains.Add("to");
            this.domains.Add("tp");
            this.domains.Add("tr");
            this.domains.Add("tt");
            this.domains.Add("tv");
            this.domains.Add("tw");
            this.domains.Add("tz");
            this.domains.Add("ua");
            this.domains.Add("ug");
            this.domains.Add("uk");
            this.domains.Add("um");
            this.domains.Add("us");
            this.domains.Add("uy");
            this.domains.Add("uz");
            this.domains.Add("va");
            this.domains.Add("vc");
            this.domains.Add("ve");
            this.domains.Add("vg");
            this.domains.Add("vi");
            this.domains.Add("vn");
            this.domains.Add("vu");
            this.domains.Add("wf");
            this.domains.Add("ws");
            this.domains.Add("ye");
            this.domains.Add("yt");
            this.domains.Add("yu");
            this.domains.Add("za");
            this.domains.Add("zm");
            this.domains.Add("zw");
            this.domains.Add("aero");
            this.domains.Add("biz");
            this.domains.Add("coop");
            this.domains.Add("info");
            this.domains.Add("museum");
            this.domains.Add("name");
            this.domains.Add("pro");
            this.domains.Add("travel");
        }
        #endregion
    }
}
