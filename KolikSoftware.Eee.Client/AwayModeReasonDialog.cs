using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client
{
    public partial class AwayModeReasonDialog : Form
    {
        public AwayModeReasonDialog()
        {
            InitializeComponent();
        }

        public string Reason
        {
            get
            {
                return this.reasonText.Text;
            }
        }
    }
}