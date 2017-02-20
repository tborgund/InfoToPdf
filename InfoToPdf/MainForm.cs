using InfoToPdf.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using BarcodeLib;
using System.Text.RegularExpressions;

namespace InfoToPdf
{
    public partial class MainForm : Form 
    {
        AppSettings appConfig = new AppSettings();
        AppStrings appStrings = new AppStrings();
        BackgroundWorker bwPdf = new BackgroundWorker();
        public StringBuilder content = new StringBuilder();
        
        public static string emailCustomer = "";
        public WebBrowserExtract wbe;
        private Notification notify = new Notification();

        public MainForm()
        {
            StartupCheck();
            InitializeComponent();
            LoadSettings();
            LoadStrings();
            UpdateWindowTitle();
            LoadPage();
            appConfig.statsCountStarted++;
        }

        private void LoadPage()
        {
            if (appConfig.chainSelected.Equals("Lefdal"))
                File.WriteAllText(Static.FileHtml, Resources.KontoinfoLefdal.Replace("[tips]", RandomTips()));
            else
                File.WriteAllText(Static.FileHtml, Resources.kontoinfo.Replace("[tips]", RandomTips()));
            Go(Static.FileHtml);
        }

        private void UpdateWindowTitle()
        {
            if (appConfig.chainSelected.Equals("Lefdal"))
                this.Text = "Konto informasjon " + Static.AppVersion + " - " + appStrings.ChainNameLefdal;
            else
                this.Text = "Konto informasjon " + Static.AppVersion + " - " + appStrings.ChainNameElkjop;
        }

        private string RandomTips()
        {
            try
            {
                Random rnd = new Random();

                if (appConfig.chainSelected.Equals("Lefdal"))
                    return "<p>" + appStrings.StartupTipsLefdal[rnd.Next(0, appStrings.StartupTipsLefdal.Length)] + "</p>";

                return "<p>" + appStrings.StartupTips[rnd.Next(0, appStrings.StartupTips.Length)] + "</p>";
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
                    if (webBrowser.Document != null)
                        webBrowser.Document.InvokeScript("doShowOverlayPrinted");

                    string result = (string)e.Result;
                    buttonConvert.Text = "Åpner PDF..";
                    Process.Start(result);
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
            if (!File.Exists(Static.AppSettingsFile))
                return;

            XmlSerializer mySerializer = new XmlSerializer(typeof(AppSettings));
            using (StreamReader myXmlReader = new StreamReader(Static.AppSettingsFile))
            {
                try
                {
                    appConfig = (AppSettings)mySerializer.Deserialize(myXmlReader);

                    FillSettings();
                }
                catch(Exception ex)
                {
                    var error = new Error("Error loading XML " + Static.AppSettingsFile, ex);
                    error.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Load strings from String.xml
        /// </summary>
        public void LoadStrings()
        {
            if (!File.Exists(Static.AppStringsFile))
                SaveStrings();

            XmlSerializer mySerializer = new XmlSerializer(typeof(AppStrings));
            using (StreamReader myXmlReader = new StreamReader(Static.AppStringsFile))
            {
                try
                {
                    appStrings = (AppStrings)mySerializer.Deserialize(myXmlReader);
                }
                catch (Exception ex)
                {
                    var error = new Error("Error loading XML " + Static.AppStringsFile, ex);
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
            if (appConfig.chainSelected.Equals("Lefdal"))
                radioButtonChainLefdal.Checked = true;
            else
                radioButtonChainElkjop.Checked = true;

            // Stats
            labelStatsCountDocuments.Text = appConfig.statsCountDocuments.ToString();
        }

        /// <summary>
        /// Save current settings to settings.xml
        /// </summary>
        public void SaveSettings()
        {
            var serializerObj = new XmlSerializer(typeof(AppSettings));
            TextWriter writeFileStream = new StreamWriter(Static.AppSettingsFile);
            using (writeFileStream)
            {
                serializerObj.Serialize(writeFileStream, appConfig);
            }
        }

        /// <summary>
        /// Save strings to strings.xml
        /// </summary>
        public void SaveStrings()
        {
            var serializerObj = new XmlSerializer(typeof(AppStrings));
            TextWriter writeFileStream = new StreamWriter(Static.AppStringsFile);
            using (writeFileStream)
            {
                serializerObj.Serialize(writeFileStream, appStrings);
            }
        }

        /// <summary>
        /// Check for missing directories and files
        /// </summary>
        public static void StartupCheck()
        {
            try
            {
                if (!Directory.Exists(Static.AppPath))
                    Directory.CreateDirectory(Static.AppPath);

                if (!Directory.Exists(Static.AppTemp))
                    Directory.CreateDirectory(Static.AppTemp);

                if (!File.Exists(Static.ProgramWkhtmltopdf))
                    File.WriteAllBytes(Static.ProgramWkhtmltopdf, Resources.wkhtmltopdf);
                else if (new FileInfo(Static.ProgramWkhtmltopdf).Length != 23063552)
                    File.WriteAllBytes(Static.ProgramWkhtmltopdf, Resources.wkhtmltopdf);
            }
            catch (Exception ex)
            {
                var error = new Error("Error while loading modules", ex);
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
                DirectoryInfo downloadedMessageInfo = new DirectoryInfo(Static.AppTemp);

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
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Diagnostics.Process.Start(start);
            }
            else
            {
                if (BrowserContainsData() && appConfig.warnDataLoss)
                    if (Alert("Er du sikker på at du vil slette gjeldene data?", "Nytt dokument", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;

                object y = webBrowser.Document.InvokeScript("resetForm");
            }
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
                !wbe.email && !wbe.dropbox && !wbe.samsung && !wbe.tomtom && !wbe.other && !wbe.pin && !wbe.comment)
            {
                return false;
            }

            return true;
        }

        private bool GenerateHtml()
        {
            string nameOfChain = string.Empty;
            if (appConfig.chainSelected.Equals("Lefdal"))
                nameOfChain = appStrings.ChainNameLefdal;
            else
                nameOfChain = appStrings.ChainNameElkjop;

            try
            {
                wbe = new WebBrowserExtract();
                wbe.Extract(webBrowser);

                if (!wbe.jotta && !wbe.mcafee && !wbe.fsecure && !wbe.microsoft && !wbe.office && !wbe.gmail && !wbe.apple &&
                    !wbe.email && !wbe.dropbox && !wbe.samsung && !wbe.tomtom && !wbe.other && !wbe.pin && !wbe.comment)
                {
                    Alert("Ingen bokser valgt!", "Mangler valg - Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (wbe.orderno.Length == 0 && appConfig.warnMissingOrderno)
                {
                    if (Alert("Ordrenummer er ikke utfylt. Fortsette?", "Mangler valg", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                        return false;
                }

                StringBuilder doc = new StringBuilder();

                if (appConfig.pdfStyle <= 0)
                {
                    if (appConfig.chainSelected.Equals("Lefdal"))
                        doc.AppendLine(Resources.htmlStartLefdal);
                    else
                        doc.AppendLine(Resources.htmlStart);
                }
                else
                {
                    if (appConfig.chainSelected.Equals("Lefdal"))
                        doc.AppendLine(Resources.htmlStartPlainLefdal);
                    else
                        doc.AppendLine(Resources.htmlStartPlain);
                }
                    
                doc.AppendLine("<div id=\"container\">");
                if (wbe.orderno.Length > 0)
                {
                    if (wbe.orderno.Length >= 7 && appConfig.pdfAddBarcode)
                    {
                        try {
                            File.Delete(Static.AppTemp + @"\barcode.png");
                        }
                        catch (IOException) { }
                        using (Barcode barcode = new Barcode(wbe.orderno))
                        {
                            barcode.EncodedType = TYPE.CODE128;
                            barcode.Height = 75;
                            barcode.Width = 300;
                            barcode.Encode();
                            barcode.SaveImage(Static.AppTemp + @"\barcode.png", SaveTypes.PNG);
                        }
                        if (File.Exists(Static.AppTemp + @"\barcode.png"))
                            doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"><img src='barcode.png' style='width:190px;height:20px;vertical-align:middle;'> Ordre-id: " + wbe.orderno + "</span>");
                        else
                            doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
                    }
                    else
                        doc.AppendLine("<span style=\"float:right;margin-top: 0px;padding:10px 0px;\"> Ordre-id: " + wbe.orderno + "</span>");
                }
                doc.AppendLine("<h1>Konto informasjon</h1>");
                doc.AppendLine("<p>");
                
                if (wbe.kundenavn.Equals(""))
                    doc.AppendLine("Kjære kunde!<br />");
                else
                    doc.AppendLine("Hei <b>" + wbe.kundenavn + "!</b><br />");
                doc.AppendLine(appStrings.DocGreetCustomerStart);
                doc.AppendLine(appStrings.DocGreetCustomerEnd);
                doc.AppendLine("</p>");

                if (wbe.jotta)
                {
                    doc.AppendLine("<div class=\"jotta service\">");
                    doc.AppendLine("<p><h2>" + nameOfChain + " Cloud konto</h2>");
                    if (wbe.jottaUnlimited)
                        doc.AppendLine(nameOfChain + " Cloud med ubegrenset lagring er installert og klargjort.</p>");
                    else
                        doc.AppendLine(nameOfChain + " Cloud med 15 GB gratis lagring er installert og klargjort.</p>");

                    AddField(doc, "Brukernavn:", wbe.jottaUser, true);
                    AddField(doc, "Passord:", wbe.jottaPass, true);

                    doc.AppendLine("<p>For administrasjon og endring av passord, gå til<br />");

                    if (appConfig.chainSelected.Equals("Lefdal"))
                        doc.AppendLine("<a href=\"https://sikkerlagring.lefdal.com/login\">sikkerlagring.lefdal.com/login</a></p>");
                    else
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

                if (wbe.other)
                {
                    doc.AppendLine("<div class=\"other service\">");
                    doc.AppendLine("<p><h2>" + wbe.otherName + "</h2>");

                    AddField(doc, "Brukernavn / E-post:", wbe.otherUser, true);
                    AddField(doc, "Passord:", wbe.otherPass, true);

                    if (!wbe.otherText.Equals("")) {
                        doc.AppendLine("<p><b>Informasjon:</b><br />");
                        doc.AppendLine(wbe.otherText + "</p>");
                    }

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
                    if (Alert("Det kan se ut som informasjonen ikke passer på en side. Vil du fortsette?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.No)
                        return false;
                }

                doc.AppendLine("<div class=\"footertext\"><span class='Bottom'>" + appConfig.shopName + "<br/>");
                if (appConfig.chainSelected.Equals("Lefdal"))
                    doc.AppendLine(appStrings.DocFooterCallcenterLefdal);
                else
                    doc.AppendLine(appStrings.DocFooterCallcenterElkjop);
                doc.AppendLine("<br/>" + appStrings.DocFooterSupportcenter);
                doc.AppendLine("</span></div>");

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
            try
            {
                var nameForm = new Quick(this);
                if (nameForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string _fornavn = nameForm.textBoxFornavn.Text;
                    string _etternavn = nameForm.textBoxEtternavn.Text;
                    string _mobil = nameForm.textBoxMobil.Text;
                    string _passord = nameForm.textBoxPassord.Text;

                    var document = new WebBrowserExtract();
                    document.Extract(webBrowser);

                    if (document.overlayPrintedActive)
                    {
                        Alert("Dokumentet er ferdig!\nÅpne for redigering eller opprett nytt");
                        return;
                    }

                    if (_fornavn.Trim().Length <= 1)
                    {
                        Alert("Fornavn er obligatorisk");
                        return;
                    }

                    string _email_part1 = _fornavn.ToLower().Trim().Replace(" ", ".");
                    string _email_part2 = _etternavn.ToLower().Trim().Replace(" ", ".");
                    string _email = "";
                    if (_etternavn.Equals(""))
                        _email = _email_part1 + "@outlook.com";
                    else
                        _email = _email_part1 + "." + _email_part2 + "@outlook.com";

                    _email = Regex.Replace(_email, @"[^\w\.@-]", "", RegexOptions.None);

                    if (!IsValidEmail(_email)) {
                        Alert("Kunne ikke generere en gyldig e-post adresse!\n" + _email + "\nPrøv igjen");
                        return;
                    }

                    if (_mobil.Equals(""))
                        _mobil = _email_part1.Replace(".", "");

                    webBrowser.Document.GetElementById("kundenavn").SetAttribute("value", _fornavn + " " + _etternavn);

                    if (!document.jotta)
                        webBrowser.Document.GetElementById("jottacheck").InvokeMember("CLICK");
                    webBrowser.Document.GetElementById("jottauser").SetAttribute("value", _mobil + " \\ " + _email);
                    webBrowser.Document.GetElementById("jottapass").SetAttribute("value", _passord);

                    if (!document.mcafee)
                        webBrowser.Document.GetElementById("mcafeecheck").InvokeMember("CLICK");
                    webBrowser.Document.GetElementById("mcafeeuser").SetAttribute("value", _email);
                    webBrowser.Document.GetElementById("mcafeepass").SetAttribute("value", _passord);

                    if (!document.microsoft)
                        webBrowser.Document.GetElementById("mscheck").InvokeMember("CLICK");
                    webBrowser.Document.GetElementById("microsoftuser").SetAttribute("value", _email);
                    webBrowser.Document.GetElementById("microsoftpass").SetAttribute("value", _passord);
                    webBrowser.Document.GetElementById("ms-day").Children[1].SetAttribute("selected", "selected");
                    webBrowser.Document.GetElementById("ms-month").Children[1].SetAttribute("selected", "selected");
                    webBrowser.Document.GetElementById("ms-year").Children[35].SetAttribute("selected", "selected");
                }
            }
            catch (Exception ex)
            {
                var error = new Error("Kunne ikke fullføre hurtigutfylling", ex);
                error.ShowDialog();
            }
        }

        static string CleanString(string str)
        {
            try
            {
                return Regex.Replace(str, @"[^\w\.@-]", "", RegexOptions.None);
            }
            catch
            {
                return String.Empty;
            }
        }

        public string CreatePassword(int length = 8, string characters = "abcdefghijklmnopqrstuvwxyz")
        {
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            length = length - 3;
            while (0 < length--)
            {
                res.Append(characters[rnd.Next(characters.Length)]);
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
                    if (webBrowser.Document != null)
                        webBrowser.Document.InvokeScript("doShowOverlayPrinted");

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
                    Directory.Delete(Static.AppPath, true);
                }
                catch
                {
                    throw new IOException("En eller flere filer var i bruk under " + Static.AppPath + " og kunne ikke bli slettet.\n For en fullstendig tilbakestilling av programmet til standard anbefales det å slette mappen manuelt.");
                }

                StartupCheck();
                appConfig = new AppSettings();
                appConfig.statsCountStarted = s;
                appConfig.statsCountDocuments = d;
                SaveSettings();
                FillSettings();

                appStrings = new AppStrings();
                SaveStrings();
            }
            catch (Exception ex)
            {
                var error = new Error("Noe skjedde ved reset av innstillingene. Lukk alle programmer og prøv igjen.", ex);
                error.ShowDialog();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Alert("KontoInfo v" + Static.AppVersion + "  " + RetrieveLinkerTimestamp().ToShortDateString()
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
            Stream s = null;
            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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

        private void radioButtonChainElkjop_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChainElkjop.Checked)
                appConfig.chainSelected = "Elkjop";
            else
                appConfig.chainSelected = "Lefdal";

            UpdateWindowTitle();
        }

        private void radioButtonChainLefdal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChainLefdal.Checked)
                appConfig.chainSelected = "Lefdal";
            else
                appConfig.chainSelected = "Elkjop";

            UpdateWindowTitle();
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenPassGen();
        }

        public void OpenPassGen()
        {
            var form = new PassordGenerator(this);
            if (form.ShowDialog() == DialogResult.Yes)
            {
                string _pass = form.textBoxPassword.Text;

                if (_pass.Equals(""))
                {
                    Alert("Passordet var tomt!");
                    return;
                }

                var document = new WebBrowserExtract();
                document.Extract(webBrowser);

                if (document.overlayPrintedActive)
                {
                    Alert("Dokumentet er ferdig!\nÅpne for redigering eller opprett nytt");
                    return;
                }

                if (document.jotta && document.jottaPass.Equals(""))
                    webBrowser.Document.GetElementById("jottapass").SetAttribute("value", _pass);

                if (document.mcafee && document.mcafeePass.Equals(""))
                    webBrowser.Document.GetElementById("mcafeepass").SetAttribute("value", _pass);

                if (document.microsoft && document.microsoftPass.Equals(""))
                    webBrowser.Document.GetElementById("microsoftpass").SetAttribute("value", _pass);

                if (document.apple && document.applePass.Equals(""))
                    webBrowser.Document.GetElementById("applepass").SetAttribute("value", _pass);

                if (document.dropbox && document.dropboxPass.Equals(""))
                    webBrowser.Document.GetElementById("dropboxpass").SetAttribute("value", _pass);

                if (document.email && document.emailPass.Equals(""))
                    webBrowser.Document.GetElementById("emailpass").SetAttribute("value", _pass);

                if (document.gmail && document.gmailPass.Equals(""))
                    webBrowser.Document.GetElementById("gmailpass").SetAttribute("value", _pass);

                if (document.other && document.otherPass.Equals(""))
                    webBrowser.Document.GetElementById("otherpass").SetAttribute("value", _pass);

                if (document.samsung && document.samsungPass.Equals(""))
                    webBrowser.Document.GetElementById("samsungpass").SetAttribute("value", _pass);

                if (document.tomtom && document.tomtomPass.Equals(""))
                    webBrowser.Document.GetElementById("tomtompass").SetAttribute("value", _pass);
            }
        }

        private void linkLabelStringsXml_Click(object sender, EventArgs e)
        {
            try
            {
                string argument = "/select, \"" + Static.AppStringsFile + "\"";
                Process.Start("explorer.exe", argument);
            }
            catch { }
        }

        private void pictureElkjop_Click(object sender, EventArgs e)
        {
            radioButtonChainElkjop.Checked = true;
            appConfig.chainSelected = "Elkjop";

            UpdateWindowTitle();
        }

        private void pictureLefdal_Click(object sender, EventArgs e)
        {
            radioButtonChainLefdal.Checked = true;
            appConfig.chainSelected = "Lefdal";

            UpdateWindowTitle();
        }
    }
}
