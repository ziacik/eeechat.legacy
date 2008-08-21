namespace KolikSoftware.Eee.Client.Media
{
    partial class MediaPlayer
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
            if (disposing && this.currentMode != Mode.None)
            {
                CloseFile();
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
            this.mediaPlayerNotifyHelper = new KolikSoftware.Eee.Client.Media.MediaPlayerNotifyHelper();
            // 
            // mediaPlayerNotifyHelper
            // 
            this.mediaPlayerNotifyHelper.Location = new System.Drawing.Point(0, 0);
            this.mediaPlayerNotifyHelper.Name = "mediaPlayerNotifyHelper";
            this.mediaPlayerNotifyHelper.Size = new System.Drawing.Size(205, 87);
            this.mediaPlayerNotifyHelper.TabIndex = 0;
            this.mediaPlayerNotifyHelper.MediaNotified += new System.EventHandler<KolikSoftware.Eee.Client.Media.MediaPlayerNotifyHelper.MediaNotifiedEventArgs>(this.mediaPlayerNotifyHelper_MediaNotified);

        }

        #endregion

        private MediaPlayerNotifyHelper mediaPlayerNotifyHelper;

    }
}
