using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class SmileFinder : IMainFormPlugin
    {
        public MainForm Form { get; set; }
        public readonly string SmiliesPath = Path.Combine(Application.StartupPath, @"Smilies");

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            ConstructSmilieStructures();
        }

        Regex SmilieRegex { get; set; }
        Dictionary<string, string> SmilieToFileDict { get; set; }
        public Dictionary<string, string> SmilieFiles { get; private set; }

        public void FindSmilesInPost(Post post)
        {
            post.Text = ConvertSmiles(post.Text);
        }

        string ConvertSmiles(string text)
        {
            return this.SmilieRegex.Replace(text, EvaluateSmilieMatch);
        }

        void ConstructSmilieStructures()
        {
            string[] smilies = new string[]
            {
                ":)",  "smile.gif",
                ":-)", "smile.gif",
                ":(", "sad.gif",
                ":-(", "sad.gif",
                ":-P", "bleh.gif",
                ":P",  "bleh.gif",
                ":-p", "bleh.gif",
                ":p",  "bleh.gif",
                ":~/", "bad.gif",
                ":beee:", "beee.gif",
                @"\_/", "beer.gif",
                ";)", "wink.gif",
                ";-)", "wink.gif",
                ":o", "shocking.gif",
                ":-o", "shocking.gif",
                ":O", "shocking.gif",
                ":-O", "shocking.gif",
                ":|", "rolleyes.gif",
                ":-|", "rolleyes.gif",
                ":beer:", "beer.gif",
                ":beta:", "beta.gif",
                ":blow:", "blow.gif",
                ":bash:", "bash.gif",
                ":blush:", "blush.gif",
                ":bye:", "bye.gif",
                ":((", "cry.gif",
                ":-((", "cry.gif",
                ":clap:", "clap.gif",
                ":crazy:", "crazy.gif",
                ":dance:", "dance.gif",
                ":disgust:", "disgust.gif",
                ":dontknow:", "dontknow.gif",
                ":drunk:", "drunk.gif",
                ":eat:", "eat.gif",
                ":excl:", "exclamation.gif",
                ":firefox:", "firefox.gif",
                ":mad:", "mad.gif",
                ":haha:", "haha.gif",
                ":help:", "help.gif",
                ":lama:", "lama.gif",
                ":lol:", "lol.gif",
                ":))", "lol.gif",
                ":-))", "lol.gif",
                ":nono:", "nono.gif",
                ":notme:", "notme.gif",
                ":nyam:", "nyam.gif",
                ":ok:", "ok.gif",
                ":play:", "play.gif",
                ":puke:", "puke.gif",
                ":question:", "question.gif",
                ":rolleyes:", "rolleyes.gif",
                ":rtfm:", "rtfm.gif",
                ":secret:", "secret.gif",
                ":shocking:", "shocking.gif",
                ":shutup:", "shutup.gif",
                ":sorry:", "sorry.gif",
                ":cool:", "cool.gif",
                ":starwars:", "starwars.gif",
                ":sun:", "sun.gif",
                ":thinking:", "thinking.gif",
                ":threat:", "threat.gif",
                ":thumbdown:", "thumbdown.gif",
                ":thumbup:", "thumbup.gif",
                ":wallbash:", "wallbash.gif",
                ":wink:", "wink.gif",
                ":yo:", "yo.gif"
            };

            this.SmilieToFileDict = new Dictionary<string, string>();
            this.SmilieFiles = new Dictionary<string, string>();

            SortedList<string, string> sortedSmilies = new SortedList<string, string>();

            int index = 0;
            while (index < smilies.Length)
            {
                this.SmilieToFileDict.Add(smilies[index], smilies[index + 1]);
                sortedSmilies.Add(smilies[index], smilies[index]);

                if (this.SmilieFiles.ContainsKey(smilies[index + 1]) == false)
                    this.SmilieFiles.Add(smilies[index + 1], smilies[index]);

                index += 2;
            }

            StringBuilder smilieRegexString = new StringBuilder();

            smilieRegexString.Append(@"(?<![^\s~?,.'""()\n\r])(");

            for (int i = sortedSmilies.Count - 1; i >= 0; i--)
            {
                smilieRegexString.Append('(');
                string smilie = sortedSmilies.Values[i];
                foreach (char smilieChar in smilie)
                {
                    smilieRegexString.Append('[');
                    smilieRegexString.Append(smilieChar);
                    if (smilieChar == '\\')
                        smilieRegexString.Append(smilieChar);
                    smilieRegexString.Append(']');
                }
                smilieRegexString.Append(')');
                if (i > 0)
                    smilieRegexString.Append('|');
            }

            smilieRegexString.Append(@")(?![^\s~?,.'""()\n\r])");

            this.SmilieRegex = new Regex(smilieRegexString.ToString(), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        string EvaluateSmilieMatch(Match match)
        {
            return @"<img border=""0"" src=""file://" + Path.Combine(this.SmiliesPath, this.SmilieToFileDict[match.Value]) + @""" />";
        }
    }
}
