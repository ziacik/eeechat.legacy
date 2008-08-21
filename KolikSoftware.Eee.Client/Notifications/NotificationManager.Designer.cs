namespace KolikSoftware.Eee.Client.Notifications
{
    partial class NotificationManager
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
            this.blinkingTimer = new System.Windows.Forms.Timer(this.components);
            this.notificationTimer = new System.Windows.Forms.Timer(this.components);
            // 
            // blinkingTimer
            // 
            this.blinkingTimer.Interval = 500;
            this.blinkingTimer.Tick += new System.EventHandler(this.blinkingTimer_Tick);
            // 
            // notificationTimer
            // 
            this.notificationTimer.Interval = 4000;
            this.notificationTimer.Tick += new System.EventHandler(this.notificationTimer_Tick);

        }

        #endregion

        private System.Windows.Forms.Timer blinkingTimer;
        private System.Windows.Forms.Timer notificationTimer;

    }
}
