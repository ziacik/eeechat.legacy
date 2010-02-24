using System;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Eee.Service.Domain
{
    public class Message
    {
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public User From { get; set; }
        public bool Private { get; set; }
        public Room Room { get; set; }
        public DateTime Sent { get; set; }
        public string Text { get; set; }
    }
}
