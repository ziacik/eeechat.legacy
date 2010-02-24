using System;
using System.Collections.Generic;
using System.Text;

namespace KolikSoftware.Eee.Service.Domain
{
    public class User
    {
        public string Login { get; set; }
        public UserState State { get; set; }
        public int Color { get; set; }
        public string Comment { get; set; }
        public string Client { get; set; }
        public DateTime ConnectedSince { get; set; }
    }
}
