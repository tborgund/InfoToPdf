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
    public partial class Error : Form
    {
        public Error(string tittel, Exception ex, string detaljer = "")
        {
            InitializeComponent();
            this.Text = "KGSA (" + MainForm.version + ") - Kritisk feil";

            labelErrorTitle.Text = tittel;
            if (detaljer != "")
                textBoxErrorMessage.Text = detaljer + Environment.NewLine;
            textBoxErrorMessage.Text += "Unntak beskjed: " + ex.Message;
            textBoxErrorMessage.Text += Environment.NewLine + "Unntak: " + ex.ToString();
            textBoxErrorMessage.Text += Environment.NewLine + "App versjon: " + MainForm.version;
            textBoxErrorMessage.Text += Environment.NewLine + "OS versjon: " + Environment.OSVersion.Version.ToString();
            textBoxErrorMessage.Text += Environment.NewLine + "Tid og Dato: " + DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
