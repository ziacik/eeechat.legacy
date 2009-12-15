using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace KolikSoftware.Eee.Client.Notifications
{
    public partial class AutoAwayMonitor : Component
    {
        public AutoAwayMonitor()
        {
            InitializeComponent();
            InitMonitor();
        }

        public AutoAwayMonitor(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitMonitor();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                UnsubscribeSystemEvents();
                this.components.Dispose();
            }
            
            base.Dispose(disposing);
        }

        void InitMonitor()
        {
            this.Enabled = true;

            SubscribeSystemEvents();

            this.autoAwayTimer.Elapsed += new ElapsedEventHandler(autoAwayTimer_Elapsed);
            this.autoAwayTimer.Start();
        }

        void SubscribeSystemEvents()
        {
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        void UnsubscribeSystemEvents()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (this.Enabled && !this.IsAutoAway)
                OnAutoAway(new AutoAwayEventArgs(true, "Locked", false));
        }

        void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            OnAutoAway(new AutoAwayEventArgs(false, null, true));
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {            
            if (e.Mode == PowerModes.Suspend)
            {
                OnAutoAway(new AutoAwayEventArgs(false, null, true));
            }
        }

        [DefaultValue(true)]
        public bool Enabled { get; set; }

        [DefaultValue(false)]
        public bool IsAutoAway { get; set; }

        #region Auto Away
        #region Event
        public class AutoAwayEventArgs : EventArgs
        {
            public AutoAwayEventArgs(bool away, string awayComment, bool logout)
            {
                this.Away = away;
                this.AwayComment = awayComment;
                this.Logout = logout;
            }

            public bool Away { get; private set; }
            public bool Logout { get; private set; }
            public string AwayComment { get; private set; }
        }

        public event EventHandler<AutoAwayEventArgs> AutoAway;

        protected virtual void OnAutoAway(AutoAwayEventArgs e)
        {
            this.IsAutoAway = e.Away;

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

            if (this.Enabled)
            {
                if (this.IsAutoAway)
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes < Properties.Settings.Default.AutoAwayDelay)
                    {
                        OnAutoAway(new AutoAwayEventArgs(false, null, false));
                    }
                }
                else
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes >= Properties.Settings.Default.AutoAwayDelay)
                    {
                        OnAutoAway(new AutoAwayEventArgs(true, "Idle", false));
                    }
                }
            }
        }
        #endregion
    }
}
