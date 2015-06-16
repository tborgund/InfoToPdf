namespace InfoToPdf
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonEmail = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInnstillinger = new System.Windows.Forms.TabPage();
            this.checkBoxSettingsWarnExit = new System.Windows.Forms.CheckBox();
            this.checkBoxSettingsAddBarcode = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxSettingsPdfStyle = new System.Windows.Forms.ComboBox();
            this.checkBoxSettingsWarnMissingOrderno = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSettingsFromAddress = new System.Windows.Forms.TextBox();
            this.textBoxSettingsSmtpPort = new System.Windows.Forms.TextBox();
            this.checkBoxSettingsSmtpUseSsl = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSettingsSmtpHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSettingsShopName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelStatsCountDocuments = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonSettingsReset = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonSettingsClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabInnstillinger.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonEmail);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.buttonSettings);
            this.panel1.Controls.Add(this.buttonUpdate);
            this.panel1.Controls.Add(this.buttonConvert);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(819, 52);
            this.panel1.TabIndex = 0;
            // 
            // buttonEmail
            // 
            this.buttonEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEmail.Location = new System.Drawing.Point(564, 12);
            this.buttonEmail.Name = "buttonEmail";
            this.buttonEmail.Size = new System.Drawing.Size(109, 30);
            this.buttonEmail.TabIndex = 4;
            this.buttonEmail.Text = "Send E-post";
            this.buttonEmail.UseVisualStyleBackColor = true;
            this.buttonEmail.Click += new System.EventHandler(this.buttonEmail_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(449, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 30);
            this.button2.TabIndex = 5;
            this.button2.Text = "Hurtigutfyll";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSettings.Location = new System.Drawing.Point(115, 12);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(97, 30);
            this.buttonSettings.TabIndex = 3;
            this.buttonSettings.Text = "Innstillinger";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUpdate.Location = new System.Drawing.Point(12, 12);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(97, 30);
            this.buttonUpdate.TabIndex = 2;
            this.buttonUpdate.Text = "Ny";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConvert.Location = new System.Drawing.Point(679, 12);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(128, 30);
            this.buttonConvert.TabIndex = 6;
            this.buttonConvert.Text = "Åpne PDF";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(817, 607);
            this.webBrowser.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panelSettings);
            this.panel2.Controls.Add(this.webBrowser);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 52);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(819, 609);
            this.panel2.TabIndex = 2;
            // 
            // panelSettings
            // 
            this.panelSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSettings.Controls.Add(this.tabControl1);
            this.panelSettings.Controls.Add(this.panel3);
            this.panelSettings.Location = new System.Drawing.Point(80, 29);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Padding = new System.Windows.Forms.Padding(10);
            this.panelSettings.Size = new System.Drawing.Size(648, 390);
            this.panelSettings.TabIndex = 2;
            this.panelSettings.Visible = false;
            this.panelSettings.VisibleChanged += new System.EventHandler(this.panelSettings_VisibleChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabInnstillinger);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(10, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(626, 326);
            this.tabControl1.TabIndex = 0;
            // 
            // tabInnstillinger
            // 
            this.tabInnstillinger.Controls.Add(this.checkBoxSettingsWarnExit);
            this.tabInnstillinger.Controls.Add(this.checkBoxSettingsAddBarcode);
            this.tabInnstillinger.Controls.Add(this.label7);
            this.tabInnstillinger.Controls.Add(this.comboBoxSettingsPdfStyle);
            this.tabInnstillinger.Controls.Add(this.checkBoxSettingsWarnMissingOrderno);
            this.tabInnstillinger.Controls.Add(this.label4);
            this.tabInnstillinger.Controls.Add(this.textBoxSettingsFromAddress);
            this.tabInnstillinger.Controls.Add(this.textBoxSettingsSmtpPort);
            this.tabInnstillinger.Controls.Add(this.checkBoxSettingsSmtpUseSsl);
            this.tabInnstillinger.Controls.Add(this.label3);
            this.tabInnstillinger.Controls.Add(this.textBoxSettingsSmtpHost);
            this.tabInnstillinger.Controls.Add(this.label2);
            this.tabInnstillinger.Controls.Add(this.textBoxSettingsShopName);
            this.tabInnstillinger.Controls.Add(this.label1);
            this.tabInnstillinger.Location = new System.Drawing.Point(4, 27);
            this.tabInnstillinger.Name = "tabInnstillinger";
            this.tabInnstillinger.Padding = new System.Windows.Forms.Padding(3);
            this.tabInnstillinger.Size = new System.Drawing.Size(618, 295);
            this.tabInnstillinger.TabIndex = 0;
            this.tabInnstillinger.Text = "Innstillinger";
            this.tabInnstillinger.UseVisualStyleBackColor = true;
            // 
            // checkBoxSettingsWarnExit
            // 
            this.checkBoxSettingsWarnExit.AutoSize = true;
            this.checkBoxSettingsWarnExit.Location = new System.Drawing.Point(180, 243);
            this.checkBoxSettingsWarnExit.Name = "checkBoxSettingsWarnExit";
            this.checkBoxSettingsWarnExit.Size = new System.Drawing.Size(136, 22);
            this.checkBoxSettingsWarnExit.TabIndex = 26;
            this.checkBoxSettingsWarnExit.Text = "Advar før sletting";
            this.checkBoxSettingsWarnExit.UseVisualStyleBackColor = true;
            this.checkBoxSettingsWarnExit.CheckedChanged += new System.EventHandler(this.checkBoxSettingsWarnExit_CheckedChanged);
            // 
            // checkBoxSettingsAddBarcode
            // 
            this.checkBoxSettingsAddBarcode.AutoSize = true;
            this.checkBoxSettingsAddBarcode.Location = new System.Drawing.Point(180, 183);
            this.checkBoxSettingsAddBarcode.Name = "checkBoxSettingsAddBarcode";
            this.checkBoxSettingsAddBarcode.Size = new System.Drawing.Size(143, 22);
            this.checkBoxSettingsAddBarcode.TabIndex = 25;
            this.checkBoxSettingsAddBarcode.Text = "Legg til strekkode";
            this.checkBoxSettingsAddBarcode.UseVisualStyleBackColor = true;
            this.checkBoxSettingsAddBarcode.CheckedChanged += new System.EventHandler(this.checkBoxSettingsWarnMissingBarcode_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 18);
            this.label7.TabIndex = 24;
            this.label7.Text = "Utseende:";
            // 
            // comboBoxSettingsPdfStyle
            // 
            this.comboBoxSettingsPdfStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSettingsPdfStyle.FormattingEnabled = true;
            this.comboBoxSettingsPdfStyle.Items.AddRange(new object[] {
            "Svart logo",
            "Enkel"});
            this.comboBoxSettingsPdfStyle.Location = new System.Drawing.Point(180, 27);
            this.comboBoxSettingsPdfStyle.Name = "comboBoxSettingsPdfStyle";
            this.comboBoxSettingsPdfStyle.Size = new System.Drawing.Size(121, 26);
            this.comboBoxSettingsPdfStyle.TabIndex = 23;
            this.comboBoxSettingsPdfStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxSettingsPdfStyle_SelectedIndexChanged);
            // 
            // checkBoxSettingsWarnMissingOrderno
            // 
            this.checkBoxSettingsWarnMissingOrderno.AutoSize = true;
            this.checkBoxSettingsWarnMissingOrderno.Location = new System.Drawing.Point(180, 213);
            this.checkBoxSettingsWarnMissingOrderno.Name = "checkBoxSettingsWarnMissingOrderno";
            this.checkBoxSettingsWarnMissingOrderno.Size = new System.Drawing.Size(264, 22);
            this.checkBoxSettingsWarnMissingOrderno.TabIndex = 22;
            this.checkBoxSettingsWarnMissingOrderno.Text = "Varsle om manglende ordrenummer";
            this.checkBoxSettingsWarnMissingOrderno.UseVisualStyleBackColor = true;
            this.checkBoxSettingsWarnMissingOrderno.CheckedChanged += new System.EventHandler(this.checkBoxSettingsWarnMissingOrderno_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(37, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "Fra adresse:";
            // 
            // textBoxSettingsFromAddress
            // 
            this.textBoxSettingsFromAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSettingsFromAddress.Location = new System.Drawing.Point(180, 149);
            this.textBoxSettingsFromAddress.Name = "textBoxSettingsFromAddress";
            this.textBoxSettingsFromAddress.Size = new System.Drawing.Size(271, 24);
            this.textBoxSettingsFromAddress.TabIndex = 11;
            this.textBoxSettingsFromAddress.TextChanged += new System.EventHandler(this.textBoxSettingsFromAddress_TextChanged);
            // 
            // textBoxSettingsSmtpPort
            // 
            this.textBoxSettingsSmtpPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSettingsSmtpPort.Location = new System.Drawing.Point(180, 119);
            this.textBoxSettingsSmtpPort.Name = "textBoxSettingsSmtpPort";
            this.textBoxSettingsSmtpPort.Size = new System.Drawing.Size(73, 24);
            this.textBoxSettingsSmtpPort.TabIndex = 9;
            this.textBoxSettingsSmtpPort.TextChanged += new System.EventHandler(this.textBoxSettingsSmtpPort_TextChanged);
            // 
            // checkBoxSettingsSmtpUseSsl
            // 
            this.checkBoxSettingsSmtpUseSsl.AutoSize = true;
            this.checkBoxSettingsSmtpUseSsl.Location = new System.Drawing.Point(290, 121);
            this.checkBoxSettingsSmtpUseSsl.Name = "checkBoxSettingsSmtpUseSsl";
            this.checkBoxSettingsSmtpUseSsl.Size = new System.Drawing.Size(55, 22);
            this.checkBoxSettingsSmtpUseSsl.TabIndex = 10;
            this.checkBoxSettingsSmtpUseSsl.Text = "SSL";
            this.checkBoxSettingsSmtpUseSsl.UseVisualStyleBackColor = true;
            this.checkBoxSettingsSmtpUseSsl.CheckedChanged += new System.EventHandler(this.checkBoxSettingsSmtpUseSsl_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(37, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // textBoxSettingsSmtpHost
            // 
            this.textBoxSettingsSmtpHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSettingsSmtpHost.Location = new System.Drawing.Point(180, 89);
            this.textBoxSettingsSmtpHost.Name = "textBoxSettingsSmtpHost";
            this.textBoxSettingsSmtpHost.Size = new System.Drawing.Size(271, 24);
            this.textBoxSettingsSmtpHost.TabIndex = 8;
            this.textBoxSettingsSmtpHost.TextChanged += new System.EventHandler(this.textBoxSettingsSmtpHost_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(37, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "SMTP host:";
            // 
            // textBoxSettingsShopName
            // 
            this.textBoxSettingsShopName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSettingsShopName.Location = new System.Drawing.Point(180, 59);
            this.textBoxSettingsShopName.Name = "textBoxSettingsShopName";
            this.textBoxSettingsShopName.Size = new System.Drawing.Size(271, 24);
            this.textBoxSettingsShopName.TabIndex = 7;
            this.textBoxSettingsShopName.TextChanged += new System.EventHandler(this.textBoxSettingsShopName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Butikknavn:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelStatsCountDocuments);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.buttonSettingsReset);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.buttonSettingsClose);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(10, 336);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(626, 42);
            this.panel3.TabIndex = 1;
            // 
            // labelStatsCountDocuments
            // 
            this.labelStatsCountDocuments.AutoSize = true;
            this.labelStatsCountDocuments.Location = new System.Drawing.Point(385, 20);
            this.labelStatsCountDocuments.Name = "labelStatsCountDocuments";
            this.labelStatsCountDocuments.Size = new System.Drawing.Size(10, 13);
            this.labelStatsCountDocuments.TabIndex = 21;
            this.labelStatsCountDocuments.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(285, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Dokumenter laget:";
            // 
            // buttonSettingsReset
            // 
            this.buttonSettingsReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSettingsReset.Location = new System.Drawing.Point(3, 13);
            this.buttonSettingsReset.Name = "buttonSettingsReset";
            this.buttonSettingsReset.Size = new System.Drawing.Size(105, 26);
            this.buttonSettingsReset.TabIndex = 13;
            this.buttonSettingsReset.Text = "Tilbakestill";
            this.buttonSettingsReset.UseVisualStyleBackColor = true;
            this.buttonSettingsReset.Click += new System.EventHandler(this.buttonSettingsReset_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(140, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 26);
            this.button1.TabIndex = 19;
            this.button1.Text = "Om..";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // buttonSettingsClose
            // 
            this.buttonSettingsClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettingsClose.Location = new System.Drawing.Point(546, 13);
            this.buttonSettingsClose.Name = "buttonSettingsClose";
            this.buttonSettingsClose.Size = new System.Drawing.Size(76, 26);
            this.buttonSettingsClose.TabIndex = 12;
            this.buttonSettingsClose.Text = "OK";
            this.buttonSettingsClose.UseVisualStyleBackColor = true;
            this.buttonSettingsClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 661);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(835, 588);
            this.Name = "MainForm";
            this.Text = "Konto Informasjon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabInnstillinger.ResumeLayout(false);
            this.tabInnstillinger.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInnstillinger;
        private System.Windows.Forms.Button buttonSettingsClose;
        private System.Windows.Forms.TextBox textBoxSettingsShopName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSettingsFromAddress;
        private System.Windows.Forms.TextBox textBoxSettingsSmtpPort;
        private System.Windows.Forms.CheckBox checkBoxSettingsSmtpUseSsl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSettingsSmtpHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSettingsReset;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxSettingsWarnMissingOrderno;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxSettingsPdfStyle;
        private System.Windows.Forms.CheckBox checkBoxSettingsAddBarcode;
        private System.Windows.Forms.CheckBox checkBoxSettingsWarnExit;
        private System.Windows.Forms.Label labelStatsCountDocuments;
        private System.Windows.Forms.Label label5;
    }
}

