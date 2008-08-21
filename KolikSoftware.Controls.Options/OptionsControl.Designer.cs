using System.ComponentModel;
using System.ComponentModel.Design;
namespace KolikSoftware.Controls.Options
{
    [Designer(typeof(OptionsControlDesigner))]
    partial class OptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsControl));
            this.categoryToolStrip = new System.Windows.Forms.ToolStrip();
            this.separator = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.separator)).BeginInit();
            this.SuspendLayout();
            // 
            // categoryToolStrip
            // 
            this.categoryToolStrip.AllowMerge = false;
            this.categoryToolStrip.BackColor = System.Drawing.Color.White;
            this.categoryToolStrip.CanOverflow = false;
            this.categoryToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.categoryToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.categoryToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.categoryToolStrip, "categoryToolStrip");
            this.categoryToolStrip.Name = "categoryToolStrip";
            this.categoryToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.categoryToolStrip.ShowItemToolTips = false;
            this.categoryToolStrip.Stretch = true;
            // 
            // separator
            // 
            this.separator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.separator, "separator");
            this.separator.Name = "separator";
            this.separator.TabStop = false;
            // 
            // OptionsControl
            // 
            this.AllowDrop = true;
            this.Controls.Add(this.separator);
            this.Controls.Add(this.categoryToolStrip);
            this.Name = "OptionsControl";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.separator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolStrip categoryToolStrip;
        private System.Windows.Forms.PictureBox separator;


    }
}
