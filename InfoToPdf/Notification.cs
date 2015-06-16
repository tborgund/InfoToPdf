using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InfoToPdf
{
    public partial class Notification : Form
    {
        public Notification()
        {
            InitializeComponent();
        }

        public void Init(string txt, string title = "Info", MessageBoxButtons msgButton = MessageBoxButtons.OK, MessageBoxIcon msgIcon = MessageBoxIcon.Error, MessageBoxDefaultButton msgDefaultButton = MessageBoxDefaultButton.Button1, bool extraHeight = false)
        {
            if (msgButton == MessageBoxButtons.OKCancel)
            {
                buttonOK.Visible = true;
                buttonCancel.Visible = true;

                if (msgDefaultButton == MessageBoxDefaultButton.Button1)
                    this.AcceptButton = buttonOK;
                if (msgDefaultButton == MessageBoxDefaultButton.Button2)
                    this.AcceptButton = buttonCancel;
            }
            else if (msgButton == MessageBoxButtons.YesNo)
            {
                buttonYes.Visible = true;
                buttonNo.Visible = true;

                if (msgDefaultButton == MessageBoxDefaultButton.Button1)
                    this.AcceptButton = buttonYes;
                if (msgDefaultButton == MessageBoxDefaultButton.Button2)
                    this.AcceptButton = buttonNo;
            }
            else
            {
                buttonOK.Visible = true;
                this.AcceptButton = buttonOK;
            }

            if (msgIcon == MessageBoxIcon.Information)
                pictureBox1.Image = System.Drawing.SystemIcons.Information.ToBitmap();
            else if (msgIcon == MessageBoxIcon.Question)
                pictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
            else if (msgIcon == MessageBoxIcon.Error)
                pictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
            else if (msgIcon == MessageBoxIcon.Warning)
                pictureBox1.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
            else if (msgIcon == MessageBoxIcon.Exclamation)
                pictureBox1.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();

            this.labelTxt.Text = txt;
            this.Text = title;

            if (extraHeight)
                this.Height = 250;
            else
                this.Height = 180;
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            // no larger than screen size
            this.MaximumSize = new System.Drawing.Size((int)System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.X, (int)System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Y);

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
    }
}
