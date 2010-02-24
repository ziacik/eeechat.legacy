namespace KolikSoftware.Eee.Client.Updating
{
    partial class UpdateManager
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
            this.installWorker = new System.ComponentModel.BackgroundWorker();
            // 
            // installWorker
            // 
            this.installWorker.WorkerReportsProgress = true;
            //this.installWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.installWorker_DoWork);
            //this.installWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.installWorker_RunWorkerCompleted);
            //this.installWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.installWorker_ProgressChanged);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker installWorker;

    }
}
