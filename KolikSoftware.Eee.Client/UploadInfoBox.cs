using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KolikSoftware.Eee.Client
{
    public partial class UploadInfoBox : Form
    {
        public UploadInfoBox()
        {
            InitializeComponent();
        }

        public string Answer
        {
            get
            {
                return this.answerText.Text;
            }
        }

        public static string Ask(string title, string question)
        {
            using (UploadInfoBox inputBox = new UploadInfoBox())
            {
                inputBox.Text = title;
                inputBox.questionLabel.Text = question;

                if (inputBox.ShowDialog() == DialogResult.OK)
                    return inputBox.answerText.Text;
                else
                    return null;
            }
        }

        public static string Ask()
        {
            using (UploadInfoBox inputBox = new UploadInfoBox())
            {
                if (inputBox.ShowDialog() == DialogResult.OK)
                    return inputBox.answerText.Text;
                else
                    return null;
            }
        }
    }
}