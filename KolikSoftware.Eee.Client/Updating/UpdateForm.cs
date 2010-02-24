using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KolikSoftware.Eee.Service;
using System.IO;

namespace KolikSoftware.Eee.Client.Updating
{
    public partial class UpdateForm : Form
    {
        //EeeDataSet.UpdateDataTable updates;

        //public UpdateForm(EeeDataSet.UpdateDataTable updates)
        //{
        //    this.updates = updates;

        //    InitializeComponent();

        //    FillUpdatesInfo();
        //}

        //void FillUpdatesInfo()
        //{
        //    if (this.updates == null)
        //        throw new ApplicationException("No updates available.");

        //    Font boldFont = new Font("Arial", 9, FontStyle.Bold);
        //    Font normalFont = new Font("Arial", 8, FontStyle.Regular);

        //    foreach (EeeDataSet.UpdateRow updateRow in this.updates)
        //    {
        //        this.updatesText.SelectionFont = boldFont;
        //        this.updatesText.SelectionColor = Color.SteelBlue;
        //        this.updatesText.AppendText(updateRow.Name);
        //        this.updatesText.AppendText(Environment.NewLine);

        //        this.updatesText.SelectionFont = normalFont;
        //        this.updatesText.SelectionColor = Color.Black;
        //        this.updatesText.AppendText(updateRow.Description);
        //        this.updatesText.AppendText(Environment.NewLine);
        //        this.updatesText.AppendText(Environment.NewLine);
        //    }
        //}

        //bool install = false;

        //public bool Install
        //{
        //    get
        //    {
        //        return this.install;
        //    }
        //}
        
        //void closeButton_Click(object sender, EventArgs e)
        //{
        //    this.install = false;
        //    Close();
        //}

        //void downloadButton_Click(object sender, EventArgs e)
        //{
        //    this.install = true;
        //    Close();
        //}
   }
}