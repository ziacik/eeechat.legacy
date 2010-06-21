using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KolikSoftware.Eee.Service.Domain
{
    public class UploadInfo
    {
        public string FilePath { get; set; }
        public string Comment { get; set; }
        public Room Room { get; set; }
        public User User { get; set; }

        public string FileName { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public byte[] Data { get; set; }
    }
}
