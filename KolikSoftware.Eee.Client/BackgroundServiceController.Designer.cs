namespace KolikSoftware.Eee.Client
{
    partial class BackgroundServiceController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sendingWorker = new System.ComponentModel.BackgroundWorker();
            this.receivingWorker = new System.ComponentModel.BackgroundWorker();
            this.invocationTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadingWorker = new System.ComponentModel.BackgroundWorker();
            this.uploadingWorker = new System.ComponentModel.BackgroundWorker();
            this.externalServicesWorker = new System.ComponentModel.BackgroundWorker();
            this.externalServicesWorker2 = new System.ComponentModel.BackgroundWorker();
            // 
            // sendingWorker
            // 
            this.sendingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.invocationWorker_DoWork);
            this.sendingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.invocationWorker_RunWorkerCompleted);
            // 
            // receivingWorker
            // 
            this.receivingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.invocationWorker_DoWork);
            this.receivingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.invocationWorker_RunWorkerCompleted);
            // 
            // invocationTimer
            // 
            this.invocationTimer.Enabled = true;
            this.invocationTimer.Interval = 1000;
            this.invocationTimer.Tick += new System.EventHandler(this.invocationTimer_Tick);
            // 
            // downloadingWorker
            // 
            this.downloadingWorker.WorkerReportsProgress = true;
            this.downloadingWorker.WorkerSupportsCancellation = true;
            this.downloadingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.invocationWorker_DoWork);
            this.downloadingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.invocationWorker_RunWorkerCompleted);
            // 
            // uploadingWorker
            // 
            this.uploadingWorker.WorkerReportsProgress = true;
            this.uploadingWorker.WorkerSupportsCancellation = true;
            this.uploadingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.invocationWorker_DoWork);
            this.uploadingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.invocationWorker_RunWorkerCompleted);
            // 
            // externalServicesWorker
            // 
            this.externalServicesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.externalServicesWorker_DoWork);
            this.externalServicesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.externalServicesWorker_RunWorkerCompleted);
            // 
            // externalServicesWorker2
            // 
            this.externalServicesWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.externalServicesWorker2_DoWork);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker sendingWorker;
        private System.ComponentModel.BackgroundWorker receivingWorker;
        private System.Windows.Forms.Timer invocationTimer;
        private System.ComponentModel.BackgroundWorker downloadingWorker;
        private System.ComponentModel.BackgroundWorker uploadingWorker;
        private System.ComponentModel.BackgroundWorker externalServicesWorker;
        private System.ComponentModel.BackgroundWorker externalServicesWorker2;
    }
}
