using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KolikSoftware.Eee.Service.Domain;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class CommandPostPlugin : IMainFormPlugin
    {
        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
        }

        public bool ProcessCommandPost(Post post)
        {
            if (post.Text.StartsWith("[STATE"))
            {
                ProcessStateCommand(post);
                return true;
            }

            return false;
        }

        void ProcessStateCommand(Post post)
        {
            this.Form.GetPlugin<UserStatePlugin>().SetUserFromCommand(post);
        }
    }
}
