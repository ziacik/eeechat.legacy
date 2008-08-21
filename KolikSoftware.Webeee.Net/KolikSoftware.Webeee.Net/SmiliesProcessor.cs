using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace KolikSoftware.Eee.Client.Helpers
{
    class SmiliesProcessor
    {
        #region Construction and Members
        static readonly SmiliesProcessor instance = new SmiliesProcessor();

        public static SmiliesProcessor Instance
        {
            get
            {
                return SmiliesProcessor.instance;
            }
        }

        Regex smilieRegex;
        Dictionary<string, string> smilieToFileDict;

        private SmiliesProcessor()
        {
            ConstructSmilieStructures();
        }

        Dictionary<string, string> smilieFiles = new Dictionary<string, string>();

        public Dictionary<string, string> SmilieFiles
        {
            get
            {
                return this.smilieFiles;
            }
        }
        #endregion

        #region Public Interface
        public string ConvertSmilies(string messageText)
        {
            return this.smilieRegex.Replace(messageText, EvaluateSmilieMatch);
        }
        #endregion

        #region Private Methods
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

            this.smilieToFileDict = new Dictionary<string, string>();

            SortedList<string, string> sortedSmilies = new SortedList<string, string>();

            int index = 0;
            while (index < smilies.Length)
            {
                this.smilieToFileDict.Add(smilies[index], smilies[index + 1]);
                sortedSmilies.Add(smilies[index], smilies[index]);

                if (this.smilieFiles.ContainsKey(smilies[index + 1]) == false)
                    this.smilieFiles.Add(smilies[index + 1], smilies[index]);

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

            this.smilieRegex = new Regex(smilieRegexString.ToString(), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        string EvaluateSmilieMatch(Match match)
        {
            return @"<img border=""0"" src=""Images/Smile/" + this.smilieToFileDict[match.Value] + @""" />";
        }
        #endregion
    }
}
