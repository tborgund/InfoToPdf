using InfoToPdf.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using BarcodeLib;
using System.Drawing.Imaging;

namespace InfoToPdf
{
    public partial class MainForm : Form
    {
        public static string version = "0.4";
        public static string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf";
        public static string appTemp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Temp";
        public static string settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Settings.xml";
        AppSettings appConfig = new AppSettings();
        public static string filePDFwkhtmltopdf = settingsPath + @"\wkhtmltopdf.exe";
        BackgroundWorker bwPdf = new BackgroundWorker();
        public StringBuilder content = new StringBuilder();
        public static string fileHtml = appTemp + @"\kontoinfo_temp.html";
        public static string emailCustomer = "";
        public WebBrowserExtract wbe;
        private Notification notify = new Notification();

        public MainForm()
        {
            InitializeComponent();
            StartupCheck();
            LoadSettings();
            LoadPage();
            webBrowser.Document.MouseDown += new HtmlElementEventHandler(CloseSettings);
        }

        private void LoadPage()
        {
            File.WriteAllText(fileHtml, Resources.kontoinfo);
            Go(fileHtml);
        }

        private void Go(string url)
        {
            webBrowser.Navigate(url);
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (GenerateHtml())
            {
                buttonConvert.Enabled = false;
                buttonConvert.Text = "Lager PDF..";

                bwPdf.DoWork += new DoWorkEventHandler(bwPdf_DoWork);
                bwPdf.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwPdf_Completed);
                bwPdf.RunWorkerAsync();
            }
        }

        private void bwPdf_DoWork(object sender, DoWorkEventArgs e)
        {
            var pdf = new Pdf(appConfig);
            e.Result = pdf.CreatePDF(content, bwPdf, wbe.orderno, FindFirstEmailAddress());
        }

        private void bwPdf_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var error = new Error("Feil under generering av PDF", e.Error.InnerException);
                error.ShowDialog();
            }
            else if (e.Cancelled)
            {
                Alert("Avbrutt!");
            }
            else
            {
                try
                {
                    string result = (string)e.Result;
                    buttonConvert.Text = "Åpner PDF..";
                    System.Diagnostics.Process.Start(result);
                }
                catch (FileNotFoundException ex)
                {
                    var error = new Error("Fant ikke PDF", ex);
                    error.ShowDialog();
                }
            }
            buttonConvert.Text = "Åpne PDF";
            buttonConvert.Enabled = true;
        }

        /// <summary>
        /// Load settings from settings.xml
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(settingsFile))
                return;

            XmlSerializer mySerializer = new XmlSerializer(typeof(AppSettings));
            using (StreamReader myXmlReader = new StreamReader(settingsFile))
            {
                try
                {
                    appConfig = (AppSettings)mySerializer.Deserialize(myXmlReader);

                    FillSettings();
                }
                catch(Exception ex)
                {
                    var error = new Error("Kan ikke laste innstillinger", ex);
                    error.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Populate settings controls from current settings
        /// </summary>
        private void FillSettings()
        {
            // General settngs
            if (appConfig.pdfStyle >= 0 && appConfig.pdfStyle <= 1)
                comboBoxSettingsPdfStyle.SelectedIndex = appConfig.pdfStyle;
            else
                comboBoxSettingsPdfStyle.SelectedIndex = 0;
            textBoxSettingsShopName.Text = appConfig.shopName;
            // E-mail settings
            textBoxSettingsSmtpHost.Text = appConfig.emailSmtpHost;
            textBoxSettingsSmtpPort.Text = appConfig.emailSmtpPort.ToString();
            textBoxSettingsFromAddress.Text = appConfig.emailFromAddress;
            // PDF settings
            radioButtonSettingsOrientH.Checked = appConfig.pdfLandscape;
            radioButtonSettingsOrientV.Checked = !appConfig.pdfLandscape;
            numericSettingsPdfZoom.Value = appConfig.pdfZoom;
            checkBoxSettingsAddBarcode.Checked = appConfig.pdfAddBarcode;
            
            // Other settings
            checkBoxSettingsWarnMissingOrderno.Checked = appConfig.warnMissingOrderno;
        }

        /// <summary>
        /// Save current settings to settings.xml
        /// </summary>
        public void SaveSettings()
        {
            var serializerObj = new XmlSerializer(typeof(AppSettings));
            TextWriter writeFileStream = new StreamWriter(settingsFile);
            using (writeFileStream)
            {
                serializerObj.Serialize(writeFileStream, appConfig);
            }
        }

        /// <summary>
        /// Check for missing directories and modules
        /// </summary>
        public static void StartupCheck()
        {
            try
            {
                if (!Directory.Exists(settingsPath))
                {
                    Directory.CreateDirectory(settingsPath);
                }
                if (!Directory.Exists(appTemp))
                {
                    Directory.CreateDirectory(appTemp);
                }
                if (!File.Exists(filePDFwkhtmltopdf))
                {
                    File.WriteAllBytes(filePDFwkhtmltopdf, Resources.wkhtmltopdf);
                }

            }
            catch (Exception ex)
            {
                var error = new Error("Feil ved opprettelse av moduler.", ex);
                error.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (BrowserContainsData())
                if (Alert("Dokument inneholder data!\nEr du sikker på at du vil avslutte?", "Avslutt", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

            Cleanup();

            if (this.WindowState == FormWindowState.Normal)
            {
                appConfig.WindowSizeX = this.Size.Width;
                appConfig.WindowSizeY = this.Size.Height;
            }
            else
            {
                appConfig.WindowSizeX = this.RestoreBounds.Size.Width;
                appConfig.WindowSizeY = this.RestoreBounds.Size.Height;
            }

            if (this.WindowState == FormWindowState.Maximized)
                appConfig.WindowMax = true;
            else
                appConfig.WindowMax = false;

            SaveSettings();
        }

        private void Cleanup()
        {
            try
            {
                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(appTemp);

                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            catch
            {
                // ignore
            }

        }

        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            if (BrowserContainsData())
                if (Alert("Er du sikker på at du vil slette gjeldene data?", "Nytt dokument", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;

            object y = webBrowser.Document.InvokeScript("resetForm");
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {

            GenerateHtml();

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            webBrowser.Refresh();
        }

        private void AddField(StringBuilder doc, string label, string field, bool bold = false)
        {
            doc.Append("<div id=\"table\">");
            doc.Append("<div id=\"left\">");
            doc.Append(label);
            doc.Append("</div>");
            doc.Append("<div id=\"right\" class=\"field\">");
            if (bold)
                doc.Append("<b>"  + field + "</b>");
            else
                doc.Append(field);
            doc.Append("</div>");
            doc.Append("</div>");
        }

        public DialogResult Alert(string txt, string title = "Info", MessageBoxButtons msgButton = MessageBoxButtons.OK, MessageBoxIcon msgIcon = MessageBoxIcon.Error, MessageBoxDefaultButton msgDefaultButton = MessageBoxDefaultButton.Button1)
        {
            Notification n = new Notification();
            n.Init(txt, title, msgButton, msgIcon, msgDefaultButton);
            return n.ShowDialog(this);
        }

        private bool BrowserContainsData()
        {
            wbe = new WebBrowserExtract();
            wbe.Extract(webBrowser);

            if (!wbe.jotta && !wbe.mcafee && !wbe.fsecure && !wbe.microsoft && !wbe.office && !wbe.gmail && !wbe.apple &&
                !wbe.email && !wbe.dropbox && !wbe.samsung && !wbe.pin && !wbe.comment)
            {
                return false;
            }

            return true;
        }


        private bool GenerateHtml()
        {
            wbe = new WebBrowserExtract();
            wbe.Extract(webBrowser);

            if (!wbe.jotta && !wbe.mcafee && !wbe.fsecure && !wbe.microsoft && !wbe.office && !wbe.gmail && !wbe.apple &&
                !wbe.email && !wbe.dropbox && !wbe.samsung && !wbe.pin && !wbe.comment)
            {
                Alert("Ingen bokser valgt!", "Mangler valg - Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (wbe.orderno.Length == 0 && appConfig.warnMissingOrderno)
            {
                if (Alert("Ordrenummer er ikke utfylt. Fortsette?", "Mangler valg - Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    return false;
            }

            StringBuilder doc = new StringBuilder();
            int height = 0;

            if (appConfig.pdfStyle <= 0)
                doc.Append(Resources.htmlStart);
            else
                doc.Append(Resources.htmlStartPlain);

            doc.Append("<div id=\"container\">");
            if (wbe.orderno.Length > 0)
            {
                if (wbe.orderno.Length == 7 && appConfig.pdfAddBarcode)
                {
                    string order = "0" + wbe.orderno;
                    using (Barcode barcode = new Barcode(order))
                    {
                        barcode.EncodedType = TYPE.EAN8;
                        barcode.Height = 75;
                        barcode.Width = 300;
                        barcode.Encode();
                        barcode.SaveImage(MainForm.appTemp + @"\barcodeKgsa.png", SaveTypes.PNG);
                    }
                    if (File.Exists(MainForm.appTemp + @"\barcodeKgsa.png"))
                        doc.Append("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"><img src='barcodeKgsa.png' style='width:190px;height:20px;vertical-align:middle;'> Ordre-id: " + wbe.orderno + "</span>");
                    else
                        doc.Append("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
                }
                else
                    doc.Append("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
            }
            doc.Append("<h1>Konto informasjon</h1>");
            doc.Append("<p>");
            doc.Append("Kjære kunde!<br />");
            doc.Append("Takk for at du har benyttet deg av våre teknikere til å få satt opp ditt produkt. Her har du en oversikt over brukernavn og passord knyttet til dine kontoer.<br />");
            doc.Append("<b>Ta godt vare på denne informasjonen!</b>");
            doc.Append("</p>");


            if (wbe.jotta)
            {
                height += 176 + 25;
                doc.Append("<div class=\"jotta service\">");
                doc.Append("<p><h2>Elkjøp Cloud konto</h2>");
                if (wbe.jottaUnlimited)
                    doc.Append("Elkjøp Cloud med ubegrenset lagring er installert og klargjort.</p>");
                else
                    doc.Append("Elkjøp Cloud med 15 GB gratis lagring er installert og klargjort.</p>");

                AddField(doc, "Brukernavn:", wbe.jottaUser, true);
                AddField(doc, "Passord:", wbe.jottaPass, true);
                
                doc.Append("<p>For administrasjon og endring av passord, gå til<br />");
    			doc.Append("<a href=\"https://sikkerlagring.elkjop.no/login\">sikkerlagring.elkjop.no/login</a></p>");

                doc.Append("</div>"); // end
            }

            if (wbe.mcafee)
            {
                height += 200 + 25;
                doc.Append("<div class=\"mcafee service\">");
                doc.Append("<p><h2>McAfee konto</h2>");
                doc.Append("Din sikkerhetsprogramvare McAfee Live Safe er installert og klargjort.<br /></p>");

                AddField(doc, "Brukernavn / E-post:", wbe.mcafeeUser, true);
                AddField(doc, "Passord:", wbe.mcafeePass, true);
                if (wbe.mcafeeLicense.Length > 0)
                    AddField(doc, "Lisens:", wbe.mcafeeLicense, true);

                doc.Append("<p>For administrasjon og endring av passord, gå til<br />");
                doc.Append("<a href=\"http://home.mcafee.com/Default.aspx?culture=NB-NO\" target=\"_blank\">home.mcafee.com</a></p>");

                doc.Append("</div>"); // end
            }

            if (wbe.fsecure)
            {
                height += 88 + 25;
                doc.Append("<div class=\"fsecure service\">");
                doc.Append("<p><h2>F-Secure Internet Security</h2>");
                doc.Append("Din sikkerhetsprogramvare F-Secure Internet Security er installert og klargjort.<br /></p>");

                AddField(doc, "Lisens:", wbe.fsecureKey, true);

                doc.Append("</div>"); // end
            }

            if (wbe.microsoft)
            {
                height += 160 + 25;
                doc.Append("<div class=\"microsoft service\">");
                doc.Append("<p><h2>Microsoft konto</h2>");
                doc.Append("Vi har opprettet en Microsoft konto i forbindelse med klargjøringen<br/> av ditt produkt.<br /></p>");

                AddField(doc, "E-post adresse:", wbe.microsoftUser, true);
                AddField(doc, "Passord:", wbe.microsoftPass, true);
                AddField(doc, "Fødselsdato:", wbe.microsoftDay + " " + wbe.microsoftMonth + " " + wbe.microsoftYear, true);

                doc.Append("</div>"); // end
            }

            if (wbe.office)
            {
                height += 88 + 25;
                doc.Append("<div class=\"office service\">");
                doc.Append("<p><h2>Office Lisens</h2>");

                AddField(doc, "Lisens:", wbe.officeKey, true);

                doc.Append("</div>"); // end
            }

            if (wbe.gmail)
            {
                height += 184 + 25;
                doc.Append("<div class=\"gmail service\">");
                doc.Append("<p><h2>Google Konto</h2>");
                doc.Append("Vi har opprettet en Google konto i forbindelse med klargjøringen av ditt produkt.<br /></p>");

                AddField(doc, "E-post adresse:", wbe.gmailUser, true);
                AddField(doc, "Passord:", wbe.gmailPass, true);
                if (wbe.gmailAnswer.Length > 0)
                {
                    AddField(doc, "Sikkerhetsspørsmål:", wbe.gmailQuestion, true);
                    AddField(doc, "Hemmelig svar:", wbe.gmailAnswer, true);
                }
                if (wbe.gmailDay != "Dag")
                    AddField(doc, "Fødselsdato:", wbe.gmailDay + " " + wbe.gmailMonth + " " + wbe.gmailYear, true);

                doc.Append("</div>"); // end
            }

            if (wbe.apple)
            {
                height += 237 + 25;
                doc.Append("<div class=\"apple service\">");
                doc.Append("<p><h2>Apple-ID</h2>");
                doc.Append("Vi har opprettet en Apple-ID i forbindelse med klargjøringen av ditt produkt.<br /></p>");

                AddField(doc, "Brukernavn / E-post adresse:", wbe.appleUser, true);
                AddField(doc, "Passord:", wbe.applePass, true);

                doc.Append("<div id=\"table\">");
                doc.Append("<div id=\"left\">");
                doc.Append("Sikkerhetsspørsmål:");
                doc.Append("</div>");
                doc.Append("<div id=\"right\"  style=\"font-size:12px;line-height: 135%;\">");
                doc.Append("Spørsmål 1: " + wbe.appleQuestionOne + "<br />");
                doc.Append("Svar: <b>" + wbe.appleAnswerOne + "</b><br />");
                doc.Append("Spørsmål 2: " + wbe.appleQuestionTwo + "<br />");
                doc.Append("Svar: <b>" + wbe.appleAnswerTwo + "</b><br />");
                doc.Append("Spørsmål 3: " + wbe.appleQuestionThree + "<br />");
                doc.Append("Svar: <b>" + wbe.appleAnswerThree + "</b>");
                doc.Append("</div>");
                doc.Append("</div>");

                AddField(doc, "Fødselsdato:", wbe.appleDay + " " + wbe.appleMonth + " " + wbe.appleYear, true);

                doc.Append("</div>"); // end
            }

            if (wbe.email)
            {
                height += 110 + 25;
                doc.Append("<div class=\"email service\">");
                doc.Append("<p><h2>E-post konto</h2>");
                doc.Append("E-post er ferdig oppsatt på ditt produkt.<br /></p>");

                AddField(doc, "Brukernavn / E-post:", wbe.emailUser, true);
                AddField(doc, "Passord:", wbe.emailPass, true);

                doc.Append("</div>"); // end
            }

            if (wbe.dropbox)
            {
                height += 160 + 25;
                doc.Append("<div class=\"dropbox service\">");
                doc.Append("<p><h2>Dropbox Konto</h2>");
                doc.Append("Din Dropbox er satt opp og aktivert på ditt produkt.<br /></p>");

                AddField(doc, "E-post adresse:", wbe.dropboxUser, true);
                AddField(doc, "Passord:", wbe.dropboxPass, true);
                if (wbe.dropboxDay != "Dag")
                    AddField(doc, "Fødselsdato:", wbe.dropboxDay + " " + wbe.dropboxMonth + " " + wbe.dropboxYear, true);

                doc.Append("</div>"); // end
            }

            if (wbe.samsung)
            {
                height += 110 + 25;
                doc.Append("<div class=\"samsung service\">");
                doc.Append("<p><h2>Samsung konto</h2>");
                doc.Append("Vi har opprettet en Samsung konto i forbindelse med klargjøringen av ditt produkt.</p>");

                AddField(doc, "Brukernavn / E-post:", wbe.samsungUser, true);
                AddField(doc, "Passord:", wbe.samsungPass, true);

                doc.Append("</div>"); // end
            }

            if (wbe.pin)
            {
                height += 88 + 25;
                doc.Append("<div class=\"pin service\">");
                doc.Append("<p><h2>Pin</h2>");
                doc.Append("En sikkerhets PIN er aktivert på ditt produkt.<br /></p>");

                AddField(doc, "PIN:", wbe.pinCode, true);

                doc.Append("</div>"); // end
            }

            if (wbe.comment)
            {
                height += 88 + 25;
                doc.Append("<div class=\"kommentar service\">");
                doc.Append("<p><h2>Kommentar</h2>");
                doc.Append("<p>" + wbe.commentString + "</p>");

                doc.Append("</div>"); // end
            }

            int pixelBudget = 1000;
            if (appConfig.pdfLandscape)
                pixelBudget = 620;
            if (appConfig.pdfStyle == 1)
                pixelBudget = Convert.ToInt32((double)pixelBudget * 0.95);
            height = pixelBudget - height;
            
            if (height > 0)
                doc.Append("<p><div style=\"height:" + height + "px;\">&nbsp;</div></p>");
            else
            {
                if (Alert("Det kan se ut som informasjonen ikke passer til bare en side. Vil du fortsette?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    return false;
            }

            doc.Append("<p>");
            doc.Append("&nbsp;<br/>");
            doc.Append("&nbsp;<br/>");
            doc.Append("<span class='Bottom'>" + appConfig.shopName + "<br/>Kundeservice: 815 32 000 &nbsp;&nbsp; Åpningstider: Man - fre: 09:00 - 21:00 (lørdag 10:00 - 15:00)</span>");
            doc.Append("</p>");
            doc.Append("</div>");
            doc.Append("</body>");
            doc.Append("</html>");

            content = doc;

            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (appConfig.WindowMax)
                this.WindowState = FormWindowState.Maximized;
            else
            {
                if (appConfig.WindowLocationX > 0 && appConfig.WindowLocationY > 0)
                    this.Location = new Point(appConfig.WindowLocationX, appConfig.WindowLocationY);
                this.Size = new Size(appConfig.WindowSizeX, appConfig.WindowSizeY);
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void CloseSettings(object sender, HtmlElementEventArgs e)
        {
            panelSettings.Visible = false;
            if (webBrowser.Document != null)
                webBrowser.Document.InvokeScript("doHideOverlay");
            tabControl1.SelectedTab = tabInnstillinger;
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Location = new Point(
                this.ClientSize.Width / 2 - panelSettings.Size.Width / 2,
                this.ClientSize.Height / 2 - panelSettings.Size.Height / 2);
            panelSettings.Anchor = AnchorStyles.None;

            tabControl1.SelectedTab = tabInnstillinger;

            if (panelSettings.Visible)
            {
                panelSettings.Visible = false;
                if (webBrowser.Document != null)
                    webBrowser.Document.InvokeScript("doHideOverlay");
            }
            else
            {
                panelSettings.Visible = true;
                if (webBrowser.Document != null)
                    webBrowser.Document.InvokeScript("doShowOverlay");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            if (webBrowser.Document != null)
                webBrowser.Document.InvokeScript("doHideOverlay");
            tabControl1.SelectedTab = tabInnstillinger;
        }

        private void textBoxSettingsShopName_TextChanged(object sender, EventArgs e)
        {
            appConfig.shopName = textBoxSettingsShopName.Text;
        }


        public static string GetRandomString()
        {
	        string path = Path.GetRandomFileName();
	        path = path.Replace(".", ""); // Remove period.
	        return path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FillQuick();
        }

        private void FillQuick()
        {
            var nameForm = new Quick();
            if (nameForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {


                string _fornavn = nameForm.textBoxFornavn.Text;
                string _etternavn = nameForm.textBoxEtternavn.Text;
                string _mobil = nameForm.textBoxMobile.Text;
                bool simplePasswords = nameForm.checkBox1.Checked;

                if (_fornavn == "" || _mobil == "")
                {
                    Alert("Mangelfull utfylling.");
                    return;
                }

                var rnd = new Random();
                string _password = simplePasswords ? "Elkjop123" : CreatePassword(rnd.Next(9999999));
                string _email = _fornavn.ToLower().Trim().Replace(" ", ".") + "." + _etternavn.ToLower().Trim().Replace(" ", ".") + "@outlook.com";

                webBrowser.Document.GetElementById("jottacheck").InvokeMember("CLICK");
                webBrowser.Document.GetElementById("jottauser").SetAttribute("value", _mobil);
                webBrowser.Document.GetElementById("jottapass").SetAttribute("value", _password);

                webBrowser.Document.GetElementById("mcafeecheck").InvokeMember("CLICK");
                webBrowser.Document.GetElementById("mcafeeuser").SetAttribute("value", _email);
                webBrowser.Document.GetElementById("mcafeepass").SetAttribute("value", _password);

                webBrowser.Document.GetElementById("mscheck").InvokeMember("CLICK");
                webBrowser.Document.GetElementById("microsoftuser").SetAttribute("value", _email);
                webBrowser.Document.GetElementById("microsoftpass").SetAttribute("value", _password);
                webBrowser.Document.GetElementById("ms-day").Children[1].SetAttribute("selected", "selected");
                webBrowser.Document.GetElementById("ms-month").Children[1].SetAttribute("selected", "selected");
                webBrowser.Document.GetElementById("ms-year").Children[35].SetAttribute("selected", "selected");
            }
        }

        public string CreatePassword(int seed = 029347576)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random(seed);
            int length = 5;
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }

            string password = res.ToString().Substring(0, 1).ToUpper() + res.ToString().Substring(1, res.ToString().Length - 1) + rnd.Next(100, 999);

            return password;
        }

        private void buttonEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (GenerateHtml())
                {
                    var emailForm = new Email();
                    emailForm.textBoxEmail.Text = FindFirstEmailAddress();

                    if (emailForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (emailForm.textBoxEmail.Text.Length > 0)
                        {
                            buttonEmail.Enabled = false;
                            buttonEmail.Text = "Sender..";
                            emailCustomer = emailForm.textBoxEmail.Text;
                            BackgroundWorker bwPdfSend = new BackgroundWorker();
                            bwPdfSend.DoWork += new DoWorkEventHandler(bwPdf_DoWork);
                            bwPdfSend.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwPdfSend_Completed);
                            bwPdfSend.RunWorkerAsync();
                        }
                        else
                            Alert("Mangler e-post adressen!", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new Error("Feil ved opprettelse av moduler.", ex);
                error.ShowDialog();
            }
        }

        private string FindFirstEmailAddress()
        {
            if (wbe != null)
            {
                if (wbe.jottaUser.Length > 0 && IsValidEmail(wbe.jottaUser))
                    return wbe.jottaUser;
                else if (wbe.microsoftUser.Length > 0 && IsValidEmail(wbe.microsoftUser))
                    return wbe.microsoftUser;
                else if (wbe.gmailUser.Length > 0 && IsValidEmail(wbe.gmailUser))
                    return wbe.gmailUser;
                else if (wbe.appleUser.Length > 0 && IsValidEmail(wbe.appleUser))
                    return wbe.appleUser;
                else if (wbe.mcafeeUser.Length > 0 && IsValidEmail(wbe.mcafeeUser))
                    return wbe.mcafeeUser;
                else if (wbe.emailUser.Length > 0 && IsValidEmail(wbe.emailUser))
                    return wbe.emailUser;
                else if (wbe.dropboxUser.Length > 0 && IsValidEmail(wbe.dropboxUser))
                    return wbe.dropboxUser;
                else if (wbe.samsungUser.Length > 0 && IsValidEmail(wbe.samsungUser))
                    return wbe.samsungUser;
                else
                    return "";
            }
            else
                return "";
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        private void bwPdfSend_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var error = new Error("Feil under generering av PDF", e.Error.InnerException);
                error.ShowDialog();
            }
            else if (e.Cancelled)
            {
                Alert("Avbrutt!");
            }
            else if (emailCustomer.Length > 0)
            {
                try
                {
                    string result = (string)e.Result;

                    var pdf = new Pdf(appConfig);
                    pdf.SendAsEmail(result, emailCustomer);
                    Alert("E-post sendt.\nTil: " + emailCustomer, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (FileNotFoundException ex)
                {
                    var error = new Error("Fant ikke PDF", ex);
                    error.ShowDialog();
                }
                catch(Exception ex)
                {
                    var error = new Error("Sendings feil", ex);
                    error.ShowDialog();
                }
            }
            else
            {
                Alert("E-post ikke sendt!\nUkjent feil oppstod.", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            buttonEmail.Enabled = true;
            buttonEmail.Text = "Send E-post";
            emailCustomer = "";
        }

        private void textBoxSettingsSmtpHost_TextChanged(object sender, EventArgs e)
        {
            appConfig.emailSmtpHost = textBoxSettingsSmtpHost.Text;
        }

        private void textBoxSettingsSmtpPort_TextChanged(object sender, EventArgs e)
        {
            int _port = 0;
            if (Int32.TryParse(textBoxSettingsSmtpPort.Text, out _port))
            {
                appConfig.emailSmtpPort = _port;
            }
        }

        private void checkBoxSettingsSmtpUseSsl_CheckedChanged(object sender, EventArgs e)
        {
            appConfig.emailSmtpUseSsl = checkBoxSettingsSmtpUseSsl.Checked;
        }

        private void textBoxSettingsFromAddress_TextChanged(object sender, EventArgs e)
        {
            appConfig.emailFromAddress = textBoxSettingsFromAddress.Text;
        }

        private void buttonSettingsReset_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(settingsFile);
                appConfig = new AppSettings();
                SaveSettings();
                FillSettings();
            }
            catch
            {

            }
        }

        private void radioButtonSettingsOrientV_CheckedChanged(object sender, EventArgs e)
        {
            appConfig.pdfLandscape = radioButtonSettingsOrientH.Checked;
        }

        private void numericSettingsPdfZoom_ValueChanged(object sender, EventArgs e)
        {
            appConfig.pdfZoom = numericSettingsPdfZoom.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Alert("KontoInfo v" + version + "  " + RetrieveLinkerTimestamp().ToShortDateString() + "\nProgrammet er laget av Trond Borgund med inspirasjon fra Tommy W. Major som laget Kontoinfo.htm\n", "Om..", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        private DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;
            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }
            int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }

        private void checkBoxSettingsWarnMissingOrderno_CheckedChanged(object sender, EventArgs e)
        {
            appConfig.warnMissingOrderno = checkBoxSettingsWarnMissingOrderno.Checked;
        }

        private void comboBoxSettingsPdfStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            appConfig.pdfStyle = comboBoxSettingsPdfStyle.SelectedIndex;
        }

        private void checkBoxSettingsWarnMissingBarcode_CheckedChanged(object sender, EventArgs e)
        {
            appConfig.pdfAddBarcode = checkBoxSettingsAddBarcode.Checked;
        }
    }

}
