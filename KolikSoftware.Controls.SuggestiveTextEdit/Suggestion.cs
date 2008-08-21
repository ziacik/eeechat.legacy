using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Controls
{
    public partial class Suggestion : UserControl
    {
        public Suggestion()
        {
            InitializeComponent();
        }

        void suggestionLabel_Resize(object sender, EventArgs e)
        {
            this.Width = this.suggestionLabel.Width + 2;
        }

        public string Suggest
        {
            get
            {
                return this.suggestionLabel.Text;
            }
            set
            {
                this.suggestionLabel.Text = value;
            }            
        }
    }
}
