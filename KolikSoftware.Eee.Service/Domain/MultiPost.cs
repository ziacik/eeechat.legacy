using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service.Domain
{
    public class MultiPost : Post
    {
        public List<Post> Posts { get; private set; }

        public MultiPost()
        {
            this.Posts = new List<Post>();
        }
    }
}
