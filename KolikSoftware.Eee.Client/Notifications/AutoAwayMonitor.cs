using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Runtime.InteropServices;

namespace KolikSoftware.Eee.Client.Notifications
{
    public partial class AutoAwayMonitor : Component
    {
        public AutoAwayMonitor()
        {
            InitializeComponent();
            this.autoAwayTimer.Elapsed += new ElapsedEventHandler(autoAwayTimer_Elapsed);
            this.autoAwayTimer.Start();
        }

        public AutoAwayMonitor(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.autoAwayTimer.Elapsed += new ElapsedEventHandler(autoAwayTimer_Elapsed);
            this.autoAwayTimer.Start();
        }

        bool enabled = true;

        [DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        #region Auto Away
        #region Event
        public class AutoAwayEventArgs : EventArgs
        {
            public AutoAwayEventArgs(bool away)
            {
                this.away = away;
            }

            bool away;

            /// <summary>
            /// True if we got to away mode, false if we woke up.
            /// </summary>
            public bool Away
            {
                get
                {
                    return this.away;
                }
            }
        }

        public event EventHandler<AutoAwayEventArgs> AutoAway;

        protected virtual void OnAutoAway(AutoAwayEventArgs e)
        {
            EventHandler<AutoAwayEventArgs> handler = AutoAway;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Win API & Private Members
        Timer autoAwayTimer = new Timer(15000);

        [StructLayout(LayoutKind.Sequential)]
        struct Win32LastInputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern bool GetLastInputInfo(out Win32LastInputInfo plii);

        Win32LastInputInfo lastInputBuffer = new Win32LastInputInfo();
        uint lastTicks = 0;
        DateTime lastActivity = DateTime.Now;
        bool isAutoAway = false;
        #endregion

        void autoAwayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.lastInputBuffer.cbSize = 8;

            if (GetLastInputInfo(out this.lastInputBuffer))
            {
                if (this.lastInputBuffer.dwTime != this.lastTicks)
                {
                    this.lastTicks = this.lastInputBuffer.dwTime;
                    this.lastActivity = DateTime.Now;
                }
            }
            else
            {
                this.lastActivity = DateTime.Now;
            }

            if (this.enabled)
            {
                if (this.isAutoAway)
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes < Properties.Settings.Default.AutoAwayDelay)
                    {
                        OnAutoAway(new AutoAwayEventArgs(false));
                        this.isAutoAway = false;
                    }
                }
                else
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes >= Properties.Settings.Default.AutoAwayDelay)
                    {
                        this.isAutoAway = true;
                        OnAutoAway(new AutoAwayEventArgs(true));
                    }
                }
            }
        }
        #endregion
    }
}
