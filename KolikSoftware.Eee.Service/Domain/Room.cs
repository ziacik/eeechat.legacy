﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class Room
    {
        [DataMember]
        public string Name { get; set; }
    }
}
