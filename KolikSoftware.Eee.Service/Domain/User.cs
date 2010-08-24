using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace KolikSoftware.Eee.Service.Domain
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public UserState State { get; set; }
        [DataMember]
        public int Color { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string Client { get; set; }
        [DataMember]
        public DateTime ConnectedSince { get; set; }

        string userImagePath;

        public string ImagePath
        {
            get
            {
                if (this.userImagePath == null)
                    SetUserImagePath();

                return this.userImagePath;
            }
        }

        void SetUserImagePath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userDataDir = Path.Combine(appDataDir, @"EeeClient\Users");
            this.userImagePath = Path.Combine(userDataDir, this.Login + ".png");
        }
    }
}
