using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InfoToPdf
{
    public partial class PassordGenerator : Form
    {
        MainForm main;
        public string password = "";
        bool simpleMode = false;
        public PassordGenerator(MainForm form, bool simple = false)
        {
            this.main = form;
            this.simpleMode = simple;
            InitializeComponent();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string pass = main.CreatePassword((int)numericPasswordLength.Value, textBoxCharacters.Text);
            if (pass != null)
            {
                textBoxPassword.Text = pass;
                this.password = pass;
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!textBoxPassword.Text.Equals(""))
                System.Windows.Forms.Clipboard.SetText(textBoxPassword.Text);
        }

        private void textBoxCharacters_TextChanged(object sender, EventArgs e)
        {
            if (!textBoxCharacters.Text.Equals("abcdefghijklmnopqrstuvwxyz"))
                textBoxCharacters.ForeColor = Color.Black;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            numericPasswordLength.Value = 8M;
            textBoxCharacters.Text = "abcdefghijklmnopqrstuvwxyz";
            textBoxPassword.Text = "";
            buttonGenerate.Focus();
        }

        private void PassordGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.DialogResult == System.Windows.Forms.DialogResult.Yes
                || this.DialogResult == System.Windows.Forms.DialogResult.OK) && textBoxPassword.Text.Length < 8)
            {
                e.Cancel = true;

                Flash(textBoxPassword, 150, Color.OrangeRed, 3);
                buttonGenerate.Select();
            }
        }

        public void Flash(TextBox textBox, int interval, Color color, int flashes)
        {
            new Thread(() => FlashInternal(textBox, interval, color, flashes)).Start();
        }

        private delegate void UpdateTextboxDelegate(TextBox textBox, Color originalColor);
        public void UpdateTextbox(TextBox textBox, Color color)
        {
            if (textBox.InvokeRequired)
            {
                this.Invoke(new UpdateTextboxDelegate(UpdateTextbox), new object[] { textBox, color });
            }
            textBox.BackColor = color;
        }

        private void FlashInternal(TextBox textBox, int interval, Color flashColor, int flashes)
        {
            Color original = textBox.BackColor;
            for (int i = 0; i < flashes; i++)
            {

                UpdateTextbox(textBox, flashColor);
                Thread.Sleep(interval / 2);
                UpdateTextbox(textBox, original);
                Thread.Sleep(interval / 2);
            }
        }

        private void PassordGenerator_Shown(object sender, EventArgs e)
        {
            if (simpleMode)
            {
                buttonAuto.Visible = false;
                buttonOk.Visible = true;
            }
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPassword.Text.Equals(""))
                buttonCopy.Enabled = false;
            else
                buttonCopy.Enabled = true;
        }
    }
}
