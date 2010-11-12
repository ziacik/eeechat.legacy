using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EeeClientLight
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.browser.Navigate(@"file:///C:/Users/fziacik/Documents/Visual%20Studio%202010/Projects/WorkspacesMy/KolikSoftware.EeeChat/Webeee/webeee.html");
        }
    }
}
