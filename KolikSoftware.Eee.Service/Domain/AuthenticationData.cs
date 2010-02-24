using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class AuthenticationData
    {
        [DataMember]
        public string Salt { get; set; }
    }
}
