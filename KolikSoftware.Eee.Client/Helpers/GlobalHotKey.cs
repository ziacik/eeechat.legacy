using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace KolikSoftware.Eee.Client.Helpers
{

    public partial class GlobalHotKey : Component
    {
        HotkeyWindow hotkeyWindow = null;
        Keys hotKey = Keys.None;

        #region Events
        public class PressedEventArgs : EventArgs
        {
            public static readonly new PressedEventArgs Empty = new PressedEventArgs();
        }

        public event EventHandler<PressedEventArgs> Pressed;

        protected virtual void OnPressed(PressedEventArgs e)
        {
            EventHandler<PressedEventArgs> handler = Pressed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ErrorEventArgs : EventArgs
        {
            public ErrorEventArgs(Exception ex)
                : base()
            {
                this.error = ex;
            }

            private Exception error;

            public Exception Error
            {
                get
                {
                    return this.error;
                }
                set
                {
                    this.error = value;
                }
            }
        }

        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void OnError(ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = Error;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        public GlobalHotKey()
        {
            InitializeComponent();
        }

        #region Win Api
        [DllImport("user32", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);
        [DllImport("user32", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);
        [DllImport("kernel32", SetLastError = true)]
        private static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32", SetLastError = true)]
        private static extern short GlobalDeleteAtom(short nAtom);

        private const int MOD_ALT = 1;
        private const int MOD_CONTROL = 2;
        private const int MOD_SHIFT = 4;
        private const int MOD_WIN = 8;

        short hotkeyID;

        void RegisterGlobalHotKey(Keys hotkey)
        {
            try
            {
                int modifiers = 0;

                if ((hotkey & Keys.Alt) == Keys.Alt)
                    modifiers += 1;

                if ((hotkey & Keys.Shift) == Keys.Shift)
                    modifiers += 4;

                if ((hotkey & Keys.Control) == Keys.Control)
                    modifiers += 2;

                if (((hotkey & Keys.LWin) == Keys.LWin) || ((hotkey & Keys.RWin) == Keys.RWin))
                    modifiers += 8;

                hotkey = hotkey & Keys.KeyCode;

                string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + this.Site.Name;

                this.hotkeyID = GlobalAddAtom(atomName);

                if (this.hotkeyID == 0)
                    throw new Exception("Unable to generate unique hotkey ID.", new Win32Exception(Marshal.GetLastWin32Error()));

                this.hotkeyWindow = new HotkeyWindow(this.hotkeyID);
                this.hotkeyWindow.Pressed += new EventHandler<PressedEventArgs>(hotkeyWindow_Pressed);

                if (RegisterHotKey(this.hotkeyWindow.Handle, this.hotkeyID, modifiers, (int)hotkey) == 0)
                    return; //TODO: throw new Exception("Unable to register hotkey.", new Win32Exception(Marshal.GetLastWin32Error()));
            }
            catch (Exception e)
            {
                UnregisterGlobalHotKey();
                OnError(new ErrorEventArgs(e));
            }
        }

        void hotkeyWindow_Pressed(object sender, GlobalHotKey.PressedEventArgs e)
        {
            OnPressed(new PressedEventArgs());
        }

        void UnregisterGlobalHotKey()
        {
            if (this.hotkeyID != 0)
            {
                UnregisterHotKey(this.hotkeyWindow.Handle, this.hotkeyID);
                GlobalDeleteAtom(this.hotkeyID);
                this.hotkeyID = 0;
                this.hotkeyWindow.Dispose();
                this.hotkeyWindow = null;
            }
        }
        #endregion

        public GlobalHotKey(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        [DefaultValue(Shortcut.None)]
        public Keys HotKey
        {
            get
            {
                return this.hotKey;
            }
            set
            {
                try
                {
                    if (this.hotKey != value)
                    {
                        if (this.DesignMode == true)
                        {
                            this.hotKey = value;
                            return;
                        }
                        else
                        {
                            if (this.hotkeyID != 0)
                                UnregisterGlobalHotKey();

                            this.hotKey = value;

                            if (value != Keys.None)
                                RegisterGlobalHotKey(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError(new ErrorEventArgs(ex));
                }
            }
        }

        public class HotkeyWindow : System.Windows.Forms.NativeWindow, IDisposable
        {
            public HotkeyWindow(short hotkeyID)
                : base()
            {
                this.hotkeyID = hotkeyID;
                CreateParams parms = new CreateParams();
                this.CreateHandle(parms);
            }

            short hotkeyID = 0;

            public event EventHandler<PressedEventArgs> Pressed;

            protected virtual void OnPressed(PressedEventArgs e)
            {
                EventHandler<PressedEventArgs> handler = Pressed;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x312 && m.WParam == (IntPtr)hotkeyID)
                    OnPressed(new PressedEventArgs());
                else
                    base.WndProc(ref m);
            }

            #region IDisposable Members
            public void Dispose()
            {
                if (this.Handle != (IntPtr)0)
                {
                    this.DestroyHandle();
                }
            }
            #endregion
        }
    }
}
