using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client.Media
{
    public partial class MediaPlayerNotifyHelper : UserControl
    {
        const int MM_MCINOTIFY = 0x3B9;

        public MediaPlayerNotifyHelper()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == MM_MCINOTIFY)
                OnMediaNotified(MediaNotifiedEventArgs.Empty);
        }

        public class MediaNotifiedEventArgs : EventArgs
        {
            public static readonly new MediaNotifiedEventArgs Empty = new MediaNotifiedEventArgs();
        }

        public event EventHandler<MediaNotifiedEventArgs> MediaNotified;

        protected virtual void OnMediaNotified(MediaNotifiedEventArgs e)
        {
            EventHandler<MediaNotifiedEventArgs> handler = MediaNotified;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
