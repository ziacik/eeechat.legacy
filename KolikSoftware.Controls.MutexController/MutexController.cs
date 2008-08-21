using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace KolikSoftware.Controls.MutexController
{
    public partial class MutexController : Component, ISupportInitialize
    {
        public enum MutexControllerBehaviour
        {
            Indicate,
            Wait,
            Terminate
        }

        public MutexController()
        {
            InitializeComponent();
        }

        public MutexController(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        string mutexName = "DefaultMutexName";

        [DefaultValue("DefaultMutexName")]
        [Description("Unique name used to identify the mutex globally across the system.")]
        public string MutexName
        {
            get
            {
                return this.mutexName;
            }
            set
            {
                this.mutexName = value;
            }
        }

        int waitTimeout = -1;

        [DefaultValue(-1)]
        [Description("Time in millisecond before the controller timeouts if Wait behaviour applied.")]
        public int WaitTimeout
        {
            get
            {
                return this.waitTimeout;
            }
            set
            {
                this.waitTimeout = value;
            }
        }

        MutexControllerBehaviour behaviour = MutexControllerBehaviour.Indicate;

        [DefaultValue(KolikSoftware.Controls.MutexController.MutexController.MutexControllerBehaviour.Indicate)]
        [Description("Behaviour of the controller when the mutex is hit.")]
        public MutexControllerBehaviour Behaviour
        {
            get
            {
                return this.behaviour;
            }
            set
            {
                this.behaviour = value;
            }
        }

        bool isConflict = false;

        [Browsable(false)]
        public bool IsConflict
        {
            get
            {
                return this.isConflict;
            }
        }

        #region ISupportInitialize Members
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            Apply();
        }
        #endregion

        Mutex mutex = null;

        void Apply()
        {
            if (this.mutex != null)
                Release();

            this.mutex = new Mutex(false, this.MutexName);

            int timeout;

            if (this.behaviour == MutexControllerBehaviour.Wait)
                timeout = this.waitTimeout;
            else
                timeout = 0;

            if (this.mutex.WaitOne(timeout, false))
                this.isConflict = false;
            else
                this.isConflict = true;

            if (this.isConflict)
            {
                this.mutex.Close();
                this.mutex = null;
            }

            //TODO: if (this.behaviour == MutexControllerBehaviour.Terminate && this.isConflict)
                //System.Environment.Exit(-1);
        }

        void Release()
        {
            if (this.mutex != null)
            {
                this.mutex.ReleaseMutex();
                this.mutex.Close();
                this.mutex = null;
            }
        }
    }
}
