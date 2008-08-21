using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Shell32;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client.Media
{
    public partial class MediaPlayer : Component
    {
        public MediaPlayer()
        {
            InitializeComponent();
        }

        public MediaPlayer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region Win32
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        #endregion

        public string TrackInfo
        {
            get
            {
                if (this.linkIndex == -1)
                {
                    return "No Media";
                }
                else 
                {
                    string currentLink = this.links[this.linkIndex];

                    if (this.linksToTrackInfos.ContainsKey(currentLink))
                        return this.linksToTrackInfos[currentLink];
                    else
                        return Path.GetFileName(currentLink);
                }
            }
        }

        int currentVolume = 500;

        public int CurrentVolume
        {
            get
            {
                return this.currentVolume;
            }
        }

        public void VolumeDown()
        {
            if (this.currentVolume >= 100)
            {
                this.currentVolume -= 100;
                SetVolume();
            }
        }

        public void VolumeUp()
        {
            if (this.currentVolume <= 900)
            {
                this.currentVolume += 100;
                SetVolume();
            }
        }

        public void SetVolume(int volume)
        {
            this.currentVolume = volume;

            if (this.currentMode == Mode.Playing || this.currentMode == Mode.Paused)
                SetVolume();
        }

        void SetVolume()
        {
            mciSendString(@"setaudio MediaFile volume to " + this.currentVolume, null, 0, IntPtr.Zero);
            Application.DoEvents();
        }

        List<string> links = new List<string>();
        Dictionary<string, string> linksToTrackInfos = new Dictionary<string, string>();

        int linkIndex = -1;

        public void AddLink(string link)
        {
            if (!this.links.Contains(link))
            {
                this.links.Add(link);
                if (this.linkIndex == -1)
                    this.linkIndex = 0;
                OnMediaChanged(MediaChangedEventArgs.Empty);
            }
        }

        public void ClearLinks()
        {
            CloseFile();
            this.links.Clear();
            this.linkIndex = -1;
            OnMediaChanged(MediaChangedEventArgs.Empty);
        }

        public void AddLinks(List<string> links)
        {
            if (links.Count > 0)
            {
                foreach (string link in links)
                {
                    if (!this.links.Contains(link))
                        this.links.Add(link);
                }

                if (this.linkIndex == -1)
                    this.linkIndex = 0;
                OnMediaChanged(MediaChangedEventArgs.Empty);
            }
        }

        public void SetLinks(List<string> links)
        {
            CloseFile();

            this.links.Clear();
            this.links.AddRange(links);

            if (this.links.Count > 0)
                this.linkIndex = 0;
            else
                this.linkIndex = -1;

            OnMediaChanged(MediaChangedEventArgs.Empty);
        }

        public bool HasMedia
        {
            get
            {
                return this.links.Count > 0;
            }
        }

        public bool IsFirst
        {
            get
            {
                return this.linkIndex == 0;
            }
        }

        public bool IsLast
        {
            get
            {
                return this.linkIndex == this.links.Count - 1;
            }
        }

        public string CurrentLink
        {
            get
            {
                if (this.linkIndex >= 0)
                    return this.links[this.linkIndex];
                else
                    return "#";
            }
        }

        public void SetFile(string link, string filePath)
        {
            if (this.currentMode != Mode.None)
                CloseFile();

            GetTrackInfo(link, filePath);

            mciSendString(@"open """ + filePath + @""" type mpegvideo alias MediaFile wait", null, 0, IntPtr.Zero);
            Application.DoEvents();

            SetMode(Mode.Stopped);

            SetLink(link);
        }

        void SetLink(string link)
        {
            this.linkIndex = this.links.IndexOf(link);
            OnMediaChanged(MediaChangedEventArgs.Empty);
        }

        void GetTrackInfo(string link, string filePath)
        {
            Shell shell = new Shell32.ShellClass();
            Folder folder = shell.NameSpace(Path.GetDirectoryName(filePath));
            FolderItem item = folder.ParseName(Path.GetFileName(filePath));

            if (item != null)
            {
                string artist = folder.GetDetailsOf(item, 9);
                string title = folder.GetDetailsOf(item, 10);

                bool hasUnknown = false;

                if (artist == null || artist.Length == 0)
                {
                    hasUnknown = true;
                    artist = "Unknown Artist";
                }

                if (title == null || title.Length == 0)
                {
                    hasUnknown = true;
                    title = "Unknown Title";
                }

                string detail = "";

                if (hasUnknown)
                    detail = " (" + Path.GetFileName(filePath) + ")";

                this.linksToTrackInfos[link] = artist + " - " + title + detail;
            }
            else
            {
                this.linksToTrackInfos[link] = Path.GetFileName(filePath);
            }
        }

        public void CloseFile()
        {
            if (this.currentMode != Mode.None)
            {
                mciSendString(@"close MediaFile", null, 0, IntPtr.Zero);
                Application.DoEvents();

                SetMode(Mode.None);
            }
        }

        public enum Mode
        {
            None,
            Playing,
            Paused,
            Stopped
        }

        Mode currentMode = Mode.None;

        public Mode CurrentMode
        {
            get
            {
                return this.currentMode;
            }
        }

        public void Play()
        {
            if (this.currentMode != Mode.None)
            {
                if (this.currentMode == Mode.Paused)
                {
                    Resume();
                }
                else
                {
                    mciSendString(@"seek MediaFile to start wait", null, 0, IntPtr.Zero);
                    Application.DoEvents();

                    mciSendString(@"play MediaFile notify", null, 0, this.mediaPlayerNotifyHelper.Handle);
                    Application.DoEvents();

                    SetVolume();

                    SetMode(Mode.Playing);
                }
            }
        }

        public void Pause()
        {
            if (this.currentMode != Mode.None)
            {
                if (this.currentMode == Mode.Paused)
                {
                    Resume();
                }
                else if (this.currentMode == Mode.Playing)
                {
                    mciSendString(@"pause MediaFile", null, 0, IntPtr.Zero);
                    Application.DoEvents();

                    SetMode(Mode.Paused);
                }
            }
        }

        public void Stop()
        {
            if (this.currentMode != Mode.None && this.currentMode != Mode.Stopped)
            {
                mciSendString(@"stop MediaFile", null, 0, IntPtr.Zero);
                Application.DoEvents();

                SetMode(Mode.Stopped);
            }
        }

        public void Resume()
        {
            if (this.currentMode == Mode.Paused)
            {
                mciSendString(@"resume MediaFile", null, 0, IntPtr.Zero);
                Application.DoEvents();

                SetMode(Mode.Playing);
            }
        }

        public class ModeChangedEventArgs : EventArgs
        {
            public static readonly new ModeChangedEventArgs Empty = new ModeChangedEventArgs();
        }

        public event EventHandler<ModeChangedEventArgs> ModeChanged;

        protected virtual void OnModeChanged(ModeChangedEventArgs e)
        {
            EventHandler<ModeChangedEventArgs> handler = ModeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class MediaChangedEventArgs : EventArgs
        {
            public static readonly new MediaChangedEventArgs Empty = new MediaChangedEventArgs();
        }

        public event EventHandler<MediaChangedEventArgs> MediaChanged;

        protected virtual void OnMediaChanged(MediaChangedEventArgs e)
        {
            EventHandler<MediaChangedEventArgs> handler = MediaChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        void mediaPlayerNotifyHelper_MediaNotified(object sender, MediaPlayerNotifyHelper.MediaNotifiedEventArgs e)
        {
            SetMode(Mode.Stopped);
        }

        void SetMode(Mode mode)
        {
            this.currentMode = mode;
            OnModeChanged(ModeChangedEventArgs.Empty);
        }

        public string PrevLink()
        {
            if (this.linkIndex > 0)
                this.linkIndex--;

            OnMediaChanged(MediaChangedEventArgs.Empty);
            return this.links[this.linkIndex];
        }

        public string NextLink()
        {
            if (this.linkIndex < this.links.Count - 1)
                this.linkIndex++;

            OnMediaChanged(MediaChangedEventArgs.Empty);
            return this.links[this.linkIndex];
        }
    }
}
