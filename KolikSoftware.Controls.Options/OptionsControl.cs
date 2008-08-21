using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.ComponentModel.Design;

namespace KolikSoftware.Controls.Options
{
    public partial class OptionsControl : UserControl
    {
        public OptionsControl()
        {
            InitializeComponent();

            this.pages.Added += new EventHandler<OptionsPageList.AddedEventArgs>(pages_Added);
            this.pages.Removed += new EventHandler<OptionsPageList.RemovedEventArgs>(pages_Removed);
            this.pages.Cleared += new EventHandler<OptionsPageList.ClearedEventArgs>(pages_Cleared);
            this.pages.RangeRemoved += new EventHandler<OptionsPageList.RangeRemovedEventArgs>(pages_RangeRemoved);
        }

        int currentSelectedTabIndex = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetControlsVisibility();
        }

        void SetControlsVisibility()
        {
            foreach (Control control in this.Controls)
            {
                if (control != this.categoryToolStrip && control != this.separator)
                {
                    if (this.currentSelectedTabIndex.Equals(control.Tag))
                        control.Visible = true;
                    else
                        control.Visible = false;
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (this.DesignMode)
            {
                if (e.Control != this.categoryToolStrip)
                {
                    if (e.Control.Tag == null)
                        e.Control.Tag = this.currentSelectedTabIndex;
                }
            }
        }

        void pages_RangeRemoved(object sender, OptionsPageList.RangeRemovedEventArgs e)
        {
            this.categoryToolStrip.SuspendLayout();
            this.SuspendLayout();

            for (int i = e.Index + e.Count - 1; i >= e.Index; i++)
            {
                this.categoryToolStrip.Items.RemoveAt(i);
            }

            this.categoryToolStrip.ResumeLayout(false);
            this.categoryToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        void pages_Cleared(object sender, OptionsPageList.ClearedEventArgs e)
        {
            this.categoryToolStrip.SuspendLayout();
            this.SuspendLayout();

            this.categoryToolStrip.Items.Clear();

            this.categoryToolStrip.ResumeLayout(false);
            this.categoryToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        void pages_Added(object sender, OptionsPageList.AddedEventArgs e)
        {
            this.categoryToolStrip.SuspendLayout();
            this.SuspendLayout();

            ToolStripButton newButton = new ToolStripButton();

            newButton.AutoSize = false;
            newButton.BackColor = System.Drawing.Color.Transparent;
            newButton.Image = e.Page.Image;
            newButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            newButton.Margin = new System.Windows.Forms.Padding(1, 1, 1, 2);
            newButton.Size = new System.Drawing.Size(60, 49);
            newButton.Text = e.Page.Caption;
            newButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            newButton.Checked = this.categoryToolStrip.Items.Count == 0;
            newButton.Click += new EventHandler(newButton_Click);
            newButton.Tag = e.Page;

            this.categoryToolStrip.Items.Add(newButton);

            this.categoryToolStrip.ResumeLayout(false);
            this.categoryToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        void pages_Removed(object sender, OptionsPageList.RemovedEventArgs e)
        {
            ToolStripButton buttonToRemove = null;

            foreach (ToolStripButton button in this.categoryToolStrip.Items)
            {
                if (button.Tag == e.Page)
                {
                    buttonToRemove = button;
                    break;
                }
            }

            if (buttonToRemove != null)
            {
                this.categoryToolStrip.SuspendLayout();
                this.SuspendLayout();

                this.categoryToolStrip.Items.Remove(buttonToRemove);

                this.categoryToolStrip.ResumeLayout(false);
                this.categoryToolStrip.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        void newButton_Click(object sender, EventArgs e)
        {
            foreach (ToolStripButton button in this.categoryToolStrip.Items)
            {
                button.Checked = false;
            }

            ToolStripButton selectedButton = (sender as ToolStripButton);
            selectedButton.Checked = true;

            this.currentSelectedTabIndex = this.categoryToolStrip.Items.IndexOf(selectedButton);
            SetControlsVisibility();
        }

        OptionsPageList pages = new OptionsPageList();

        [
        Category("Data"),
        Description("List of pages to show."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true)
        ]
        public OptionsPageList Pages
        {
            get
            {
                return this.pages;
            }
        }
    }
}