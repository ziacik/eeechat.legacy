using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Runtime.InteropServices;

namespace KolikSoftware.Controls.Options
{
    public partial class AwayMonitor : Component
    {
        public AwayMonitor()
        {
            InitializeComponent();

            this.awayTimer.Elapsed += new ElapsedEventHandler(awayTimer_Elapsed);
            this.awayTimer.Start();
        }

        public AwayMonitor(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            this.awayTimer.Elapsed += new ElapsedEventHandler(awayTimer_Elapsed);
            this.awayTimer.Start();
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

        int delay = 15;

        /// <summary>
        /// Determines time, in minutes, that pass before the Away event is called when the user is inactive.
        /// </summary>
        [DefaultValue(15)]
        public int Delay
        {
            get
            {
                return this.delay;
            }
            set
            {
                this.delay = value;
            }
        }

        #region Away
        #region Event
        public class AwayEventArgs : EventArgs
        {
            public AwayEventArgs(bool away)
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

        public event EventHandler<AwayEventArgs> Away;

        protected virtual void OnAway(AwayEventArgs e)
        {
            EventHandler<AwayEventArgs> handler = Away;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region Win API & Private Members
        Timer awayTimer = new Timer(15000);

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
        bool isAway = false;
        #endregion

        void awayTimer_Elapsed(object sender, ElapsedEventArgs e)
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
                if (this.isAway)
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes < this.delay)
                    {
                        OnAway(new AwayEventArgs(false));
                        this.isAway = false;
                    }
                }
                else
                {
                    if ((DateTime.Now - this.lastActivity).TotalMinutes >= this.delay)
                    {
                        this.isAway = true;
                        OnAway(new AwayEventArgs(true));
                    }
                }
            }
        }
        #endregion
    }
}
