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
    public partial class Quick : Form
    {
        MainForm main;
        public Quick(MainForm form)
        {
            this.main = form;
            InitializeComponent();
        }

        private void textBoxFornavn_Leave(object sender, EventArgs e)
        {
            if (textBoxFornavn.Text.Length > 2)
                textBoxFornavn.Text = textBoxFornavn.Text.Substring(0, 1).ToUpper() + textBoxFornavn.Text.Substring(1, textBoxFornavn.Text.Length - 1);
        }

        private void textBoxEtternavn_Leave(object sender, EventArgs e)
        {
            if (textBoxEtternavn.Text.Length > 2)
                textBoxEtternavn.Text = textBoxEtternavn.Text.Substring(0, 1).ToUpper() + textBoxEtternavn.Text.Substring(1, textBoxEtternavn.Text.Length - 1);
        }

        private void buttonPassordGen_Click(object sender, EventArgs e)
        {
            OpenPassGen();
        }

        public void OpenPassGen()
        {
            var form = new PassordGenerator(main, true);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string _pass = form.textBoxPassword.Text;

                if (_pass.Equals(""))
                    return;

                textBoxPassord.Text = _pass;
            }
        }

        private void Quick_Shown(object sender, EventArgs e)
        {
            textBoxFornavn.Select();
        }
    }
}
