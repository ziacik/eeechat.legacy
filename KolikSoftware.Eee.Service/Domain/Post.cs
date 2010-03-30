using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class Post
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string GlobalId { get; set; }
        [DataMember]
        public User From { get; set; }
        [DataMember]
        public User To { get; set; }
        [DataMember]
        public Room Room { get; set; }
        [DataMember]
        public DateTime Sent { get; set; }
        [DataMember]
        public string Text { get; set; }

        public string ToLogin
        {
            get
            {
                return this.To != null ? this.To.Login : null;
            }
        }
    }
}
