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
        public static string version = "0.2";
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
            appConfig.statsCountStarted++;
        }

        private void LoadPage()
        {
            File.WriteAllText(fileHtml, Resources.kontoinfo.Replace("[tips]", RandomTips()));
            Go(fileHtml);
        }

        private string RandomTips()
        {
            try
            {
                string[] tips = new string[] {
                "Be kunden om å velge et passord som er minst 8 tegn langt med 1 stor bokstav og minst 1 tall. Max 16 tegn."
                , "E-post adressen kan også brukes til å logge inn på Elkjøp Cloud."
                , "Skjema kan fylles ut for hånd også. Bare merk av boksene du trenger og skriv ut."
                , "Dokumentet har to forskjellige utseender som kan endres under Innstillinger."
                , "Hvis informasjonen tar mer plass enn èn side, huk av noen av kontoene og skriv de ut seperat etterpå."
                , "Hvis ordrenummer legges til, lages det en strekkode som kan leses inn i Elguide."
                , "Hele navnet på butikken kan fylles ut under innstilligene og vil bli satt inn i bunnteksten."
                };

                Random rnd = new Random();
                return "<p>" + tips[rnd.Next(0, tips.Length)] + "</p>";
            }
            catch { }
            return "";
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
                    appConfig.statsCountDocuments++;
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
            if (appConfig.pdfStyle > -1 && appConfig.pdfStyle <= (comboBoxSettingsPdfStyle.Items.Count - 1))
                comboBoxSettingsPdfStyle.SelectedIndex = appConfig.pdfStyle;
            else
                comboBoxSettingsPdfStyle.SelectedIndex = 0;
            textBoxSettingsShopName.Text = appConfig.shopName;
            // E-mail settings
            textBoxSettingsSmtpHost.Text = appConfig.emailSmtpHost;
            textBoxSettingsSmtpPort.Text = appConfig.emailSmtpPort.ToString();
            textBoxSettingsFromAddress.Text = appConfig.emailFromAddress;
            // PDF settings
            checkBoxSettingsAddBarcode.Checked = appConfig.pdfAddBarcode;
            
            // Other settings
            checkBoxSettingsWarnMissingOrderno.Checked = appConfig.warnMissingOrderno;
            checkBoxSettingsWarnExit.Checked = appConfig.warnDataLoss;

            // Stats
            labelStatsCountDocuments.Text = appConfig.statsCountDocuments.ToString();
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
                else if (new System.IO.FileInfo(filePDFwkhtmltopdf).Length != 23063552)
                {
                    Console.WriteLine("filePDFwkhtmltopdf har ikke riktig lengde! 23063552");
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

            if (BrowserContainsData() && appConfig.warnDataLoss)
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
            if (BrowserContainsData() && appConfig.warnDataLoss)
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
                doc.Append("<b>" + field + "</b>");
            else
                doc.Append(field);
            doc.Append("</div>");
            doc.Append("</div>");
        }

        public DialogResult Alert(string txt, string title = "Info", MessageBoxButtons msgButton = MessageBoxButtons.OK, MessageBoxIcon msgIcon = MessageBoxIcon.Error, MessageBoxDefaultButton msgDefaultButton = MessageBoxDefaultButton.Button1, bool extraHeight = false)
        {
            Notification n = new Notification();
            n.Init(txt, title, msgButton, msgIcon, msgDefaultButton, extraHeight);
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
            try
            {
                wbe = new WebBrowserExtract();
                wbe.Extract(webBrowser);

                if (!wbe.jotta && !wbe.mcafee && !wbe.fsecure && !wbe.microsoft && !wbe.office && !wbe.gmail && !wbe.apple &&
                    !wbe.email && !wbe.dropbox && !wbe.samsung && !wbe.tomtom && !wbe.pin && !wbe.comment)
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

                if (appConfig.pdfStyle <= 0)
                    doc.AppendLine(Resources.htmlStart);
                else
                    doc.AppendLine(Resources.htmlStartPlain);

                doc.AppendLine("<div id=\"container\">");
                if (wbe.orderno.Length > 0)
                {
                    if (wbe.orderno.Length >= 7 && appConfig.pdfAddBarcode)
                    {
                        try {
                            File.Delete(MainForm.appTemp + @"\barcode.png");
                        }
                        catch (IOException) { }
                        using (Barcode barcode = new Barcode(wbe.orderno))
                        {
                            barcode.EncodedType = TYPE.CODE128;
                            barcode.Height = 75;
                            barcode.Width = 300;
                            barcode.Encode();
                            barcode.SaveImage(MainForm.appTemp + @"\barcode.png", SaveTypes.PNG);
                        }
                        if (File.Exists(MainForm.appTemp + @"\barcode.png"))
                            doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"><img src='barcode.png' style='width:190px;height:20px;vertical-align:middle;'> Ordre-id: " + wbe.orderno + "</span>");
                        else
                            doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
                    }
                    else
                        doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
                }
                doc.AppendLine("<h1>Konto informasjon</h1>");
                doc.AppendLine("<p>");
                doc.AppendLine("Kjære kunde!<br />");
                doc.AppendLine("Takk for at du har benyttet deg av våre teknikere til å få satt opp ditt produkt. Her har du en oversikt over brukernavn og passord knyttet til dine kontoer.<br />");
                doc.AppendLine("<b>Ta godt vare på denne informasjonen!</b>");
                doc.AppendLine("</p>");

                if (wbe.jotta)
                {
                    doc.AppendLine("<div class=\"jotta service\">");
                    doc.AppendLine("<p><h2>Elkjøp Cloud konto</h2>");
                    if (wbe.jottaUnlimited)
                        doc.AppendLine("Elkjøp Cloud med ubegrenset lagring er installert og klargjort.</p>");
                    else
                        doc.AppendLine("Elkjøp Cloud med 15 GB gratis lagring er installert og klargjort.</p>");

                    AddField(doc, "Brukernavn:", wbe.jottaUser, true);
                    AddField(doc, "Passord:", wbe.jottaPass, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://sikkerlagring.elkjop.no/login\">sikkerlagring.elkjop.no/login</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.mcafee)
                {
                    doc.AppendLine("<div class=\"mcafee service\">");
                    doc.AppendLine("<p><h2>McAfee konto</h2>");
                    doc.AppendLine("Din sikkerhetsprogramvare fra McAfee er installert og klargjort.<br /></p>");

                    AddField(doc, "Brukernavn / E-post:", wbe.mcafeeUser, true);
                    AddField(doc, "Passord:", wbe.mcafeePass, true);
                    if (wbe.mcafeeLicense.Length > 0)
                        AddField(doc, "Lisens:", wbe.mcafeeLicense, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"http://home.mcafee.com/Default.aspx?culture=NB-NO\" target=\"_blank\">home.mcafee.com</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.microsoft)
                {
                    doc.AppendLine("<div class=\"microsoft service\">");
                    doc.AppendLine("<p><h2>Microsoft konto</h2>");
                    doc.AppendLine("Vi har opprettet en Microsoft konto i forbindelse med klargjøringen<br/> av ditt produkt.<br /></p>");

                    AddField(doc, "E-post adresse:", wbe.microsoftUser, true);
                    AddField(doc, "Passord:", wbe.microsoftPass, true);
                    if (!wbe.microsoftDay.Equals("Dag") && wbe.microsoftUser.Length != 0)
                        AddField(doc, "Fødselsdato:", wbe.microsoftDay + " " + wbe.microsoftMonth + " " + wbe.microsoftYear, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://login.live.com\" target=\"_blank\">login.live.com</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.office)
                {
                    doc.AppendLine("<div class=\"office service\">");
                    doc.AppendLine("<p><h2>Office Lisens</h2>");

                    AddField(doc, "Lisens:", wbe.officeKey, true);

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.gmail)
                {
                    doc.AppendLine("<div class=\"gmail service\">");
                    doc.AppendLine("<p><h2>Google Konto</h2>");
                    doc.AppendLine("Vi har opprettet en Google konto i forbindelse med klargjøringen av ditt produkt.<br /></p>");

                    AddField(doc, "E-post adresse:", wbe.gmailUser, true);
                    AddField(doc, "Passord:", wbe.gmailPass, true);
                    if (wbe.gmailAnswer.Length > 0)
                    {
                        AddField(doc, "Sikkerhetsspørsmål:", wbe.gmailQuestion, true);
                        AddField(doc, "Hemmelig svar:", wbe.gmailAnswer, true);
                    }
                    if (!wbe.gmailDay.Equals("Dag") && wbe.gmailUser.Length != 0)
                        AddField(doc, "Fødselsdato:", wbe.gmailDay + " " + wbe.gmailMonth + " " + wbe.gmailYear, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://accounts.google.com\" target=\"_blank\">accounts.google.com</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.apple)
                {
                    doc.AppendLine("<div class=\"apple service\">");
                    doc.AppendLine("<p><h2>Apple-ID</h2>");
                    doc.AppendLine("Vi har opprettet en Apple-ID i forbindelse med klargjøringen av ditt produkt.<br /></p>");

                    AddField(doc, "Brukernavn / E-post adresse:", wbe.appleUser, true);
                    AddField(doc, "Passord:", wbe.applePass, true);

                    if (!(wbe.appleAnswerOne.Equals("") && wbe.appleAnswerTwo.Equals("") && wbe.appleAnswerThree.Equals("")) && wbe.appleUser.Length != 0)
                    {
                        doc.AppendLine("<div id=\"table\">");
                        doc.AppendLine("<div id=\"left\">");
                        doc.AppendLine("Sikkerhetsspørsmål:");
                        doc.AppendLine("</div>");
                        doc.AppendLine("<div id=\"right\"  style=\"font-size:12px;line-height: 135%;\">");
                        doc.AppendLine("Spørsmål 1: " + wbe.appleQuestionOne + "<br />");
                        doc.AppendLine("Svar: <b>" + wbe.appleAnswerOne + "</b><br />");
                        doc.AppendLine("Spørsmål 2: " + wbe.appleQuestionTwo + "<br />");
                        doc.AppendLine("Svar: <b>" + wbe.appleAnswerTwo + "</b><br />");
                        doc.AppendLine("Spørsmål 3: " + wbe.appleQuestionThree + "<br />");
                        doc.AppendLine("Svar: <b>" + wbe.appleAnswerThree + "</b>");
                        doc.AppendLine("</div>");
                        doc.AppendLine("</div>");
                    }
                    if (!wbe.appleDay.Equals("Dag") && wbe.appleUser.Length != 0)
                        AddField(doc, "Fødselsdato:", wbe.appleDay + " " + wbe.appleMonth + " " + wbe.appleYear, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://appleid.apple.com\" target=\"_blank\">appleid.apple.com</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.email)
                {
                    doc.AppendLine("<div class=\"email service\">");
                    doc.AppendLine("<p><h2>E-post konto</h2>");
                    doc.AppendLine("E-post er ferdig oppsatt på ditt produkt.<br /></p>");

                    AddField(doc, "Brukernavn / E-post:", wbe.emailUser, true);
                    AddField(doc, "Passord:", wbe.emailPass, true);

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.dropbox)
                {
                    doc.AppendLine("<div class=\"dropbox service\">");
                    doc.AppendLine("<p><h2>Dropbox Konto</h2>");
                    doc.AppendLine("Din Dropbox er satt opp og aktivert på ditt produkt.<br /></p>");

                    AddField(doc, "E-post adresse:", wbe.dropboxUser, true);
                    AddField(doc, "Passord:", wbe.dropboxPass, true);
                    if (!wbe.dropboxDay.Equals("Dag") && wbe.dropboxUser.Length != 0)
                        AddField(doc, "Fødselsdato:", wbe.dropboxDay + " " + wbe.dropboxMonth + " " + wbe.dropboxYear, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://www.dropbox.com\" target=\"_blank\">dropbox.com/</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.samsung)
                {
                    doc.AppendLine("<div class=\"samsung service\">");
                    doc.AppendLine("<p><h2>Samsung konto</h2>");
                    doc.AppendLine("Vi har opprettet en Samsung konto i forbindelse med klargjøringen av ditt produkt.</p>");

                    AddField(doc, "Brukernavn / E-post:", wbe.samsungUser, true);
                    AddField(doc, "Passord:", wbe.samsungPass, true);
                    if (!wbe.samsungDay.Equals("Dag") && wbe.samsungUser.Length != 0)
                        AddField(doc, "Fødselsdato:", wbe.samsungDay + " " + wbe.samsungMonth + " " + wbe.samsungYear, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://account.samsung.com/account/check.do\">account.samsung.com/account/check.do</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.tomtom)
                {
                    doc.AppendLine("<div class=\"tomtom service\">");
                    doc.AppendLine("<p><h2>TomTom konto</h2>");
                    doc.AppendLine("Vi har opprettet en TomTom konto i forbindelse med klargjøringen av ditt produkt.</p>");

                    AddField(doc, "Brukernavn / E-post:", wbe.tomtomUser, true);
                    AddField(doc, "Passord:", wbe.tomtomPass, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");
                    doc.AppendLine("<a href=\"https://no.support.tomtom.com/app/utils/login_form\">no.support.tomtom.com/app/utils/login_form</a></p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.fsecure)
                {
                    doc.AppendLine("<div class=\"fsecure service\">");
                    doc.AppendLine("<p><h2>F-Secure Internet Security</h2>");
                    doc.AppendLine("Din sikkerhetsprogramvare F-Secure Internet Security er installert og klargjort.<br /></p>");

                    AddField(doc, "Lisens:", wbe.fsecureKey, true);

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.pin)
                {
                    doc.AppendLine("<div class=\"pin service\">");
                    doc.AppendLine("<p><h2>Pin</h2>");
                    doc.AppendLine("En sikkerhets PIN er aktivert på ditt produkt.<br /></p>");

                    AddField(doc, "PIN:", wbe.pinCode, true);

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.comment)
                {
                    doc.AppendLine("<div class=\"kommentar service\">");
                    doc.AppendLine("<p><h2>Kommentar</h2>");
                    doc.AppendLine("<p>" + wbe.commentString + "</p>");

                    doc.AppendLine("</div>"); // end
                }

                if (wbe.GetNumberOfItems() > 6)
                {
                    if (Alert("Det kan se ut som informasjonen ikke passer på en side. Vil du fortsette?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                        return false;
                }

                doc.AppendLine("<div class=\"footertext\"><span class='Bottom'>" + appConfig.shopName + "<br/>Kundeservice: 815 32 000 &nbsp;&nbsp; Åpningstider: Man - fre: 09:00 - 21:00 (lørdag 10:00 - 15:00)</span></div>");
                doc.AppendLine("</div>");
                doc.AppendLine("</body>");
                doc.AppendLine("</html>");

                content = doc;

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error("Feil ved genereing av PDF", ex);
                error.ShowDialog();
            }
            return false;
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

                var document = new WebBrowserExtract();
                document.Extract(webBrowser);

                if (!document.jotta)
                    webBrowser.Document.GetElementById("jottacheck").InvokeMember("CLICK");
                webBrowser.Document.GetElementById("jottauser").SetAttribute("value", _mobil);
                webBrowser.Document.GetElementById("jottapass").SetAttribute("value", _password);

                if (!document.mcafee)
                    webBrowser.Document.GetElementById("mcafeecheck").InvokeMember("CLICK");
                webBrowser.Document.GetElementById("mcafeeuser").SetAttribute("value", _email);
                webBrowser.Document.GetElementById("mcafeepass").SetAttribute("value", _password);

                if (!document.microsoft)
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
                if (Alert("Sikker?", "Tilbakestill alt", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                    return;

                int s = 0, d = 0;
                if (appConfig != null)
                {
                    s = appConfig.statsCountStarted;
                    d = appConfig.statsCountStarted;
                }

                try
                {
                    Directory.Delete(settingsPath, true);
                }
                catch
                {
                    throw new IOException("En eller flere filer var i bruk under " + settingsPath + " og kunne ikke bli slettet.\n For en fullstendig tilbakestilling av programmet til standard anbefales det å slette mappen manuelt.");
                }

                StartupCheck();
                appConfig = new AppSettings();
                appConfig.statsCountStarted = s;
                appConfig.statsCountDocuments = d;
                SaveSettings();
                FillSettings();
            }
            catch (Exception ex)
            {
                var error = new Error("Noe skjedde ved reset av innstillingene. Lukk alle programmer og prøv igjen.", ex);
                error.ShowDialog();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Alert("KontoInfo v" + version + "  " + RetrieveLinkerTimestamp().ToShortDateString()
                + "\nProgrammet er laget av Trond Borgund med inspirasjon fra Tommy W. Major som laget Kontoinfo.htm\n\n"
                + "Takk til Brad Barnhill for Barcode Library 11-02-2013\n"
                + "og folket bak github.com/wkhtmltopdf/wkhtmltopdf!"
                ,"Om..", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, true);
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

        private void panelSettings_VisibleChanged(object sender, EventArgs e)
        {
            if (appConfig != null)
                labelStatsCountDocuments.Text = appConfig.statsCountDocuments.ToString();
        }

        private void checkBoxSettingsWarnExit_CheckedChanged(object sender, EventArgs e)
        {
            appConfig.warnDataLoss = checkBoxSettingsWarnExit.Checked;
        }
    }

}
