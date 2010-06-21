using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybound.Gecko;
using System.Windows.Forms;
using KolikSoftware.Eee.Service.Domain;

namespace KolikSoftware.Eee.Client.MainFormPlugins
{
    public class UploadPlugin : IMainFormPlugin
    {
        public MainForm Form { get; set; }

        public void Init(MainForm mainForm)
        {
            this.Form = mainForm;
            this.Form.GetPlugin<BrowserPlugin>().Browser.Navigating += new GeckoNavigatingEventHandler(Browser_Navigating);
            this.Form.UsersToolStrip.AllowDrop = true;
            this.Form.UsersToolStrip.DragEnter += new DragEventHandler(UsersToolStrip_DragEnter);
            this.Form.UsersToolStrip.DragDrop += new DragEventHandler(UsersToolStrip_DragDrop);
        }

        void UsersToolStrip_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void UsersToolStrip_DragDrop(object sender, DragEventArgs e)
        {
            string info = UploadInfoBox.Ask();
        }

        void Browser_Navigating(object sender, GeckoNavigatingEventArgs e)
        {
            if (e.Uri.IsLoopback && e.Uri.IsFile && System.IO.Path.GetFileName(e.Uri.LocalPath) != "ChatTemplate.html")
            {
                e.Cancel = true;

                string info = UploadInfoBox.Ask();

                if (info != null)
                {
                    Room room = this.Form.GetPlugin<RoomStatePlugin>().SelectedRoom;
                    User user = this.Form.GetPlugin<UserStatePlugin>().SelectedUser;

                    UploadInfo uploadInfo = new UploadInfo();
                    uploadInfo.Comment = info;
                    uploadInfo.Room = room;
                    uploadInfo.User = user;
                    uploadInfo.FilePath = e.Uri.LocalPath;

                    this.Form.Service.UploadFile(uploadInfo);
                    //UpdateUploadStatus(null);
                }            
            }
        }

        void Browser_DragDrop(object sender, DragEventArgs e)
        {
            string info = UploadInfoBox.Ask();

            if (info != null)
            {
                Room room = this.Form.GetPlugin<RoomStatePlugin>().SelectedRoom;
                User user = this.Form.GetPlugin<UserStatePlugin>().SelectedUser;

                UploadInfo uploadInfo = new UploadInfo();
                uploadInfo.Comment = info;
                uploadInfo.Room = room;
                uploadInfo.User = user;
                uploadInfo.FilePath = (string)e.Data.GetData(typeof(string));

                this.Form.Service.UploadFile(uploadInfo);
                //UpdateUploadStatus(null);
            }            
        }
    }
}
