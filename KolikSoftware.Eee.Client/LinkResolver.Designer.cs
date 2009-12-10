namespace KolikSoftware.Eee.Client
{
    partial class LinkResolver
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
            this.linkResolverWorker = new System.ComponentModel.BackgroundWorker();
            // 
            // linkResolverWorker
            // 
            this.linkResolverWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.linkResolverWorker_DoWork);
            this.linkResolverWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.linkResolverWorker_RunWorkerCompleted);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker linkResolverWorker;
    }
}
