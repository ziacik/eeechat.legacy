using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service.Domain
{
    public class Conversation : Post
    {
        public HashSet<string> Participants { get; set; }
        public List<Post> Posts { get; private set; }

        public Conversation()
        {
            this.Posts = new List<Post>();
            this.Participants = new HashSet<string>();
        }
    }
}
