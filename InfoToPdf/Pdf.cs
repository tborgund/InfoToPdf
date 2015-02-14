using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace InfoToPdf
{
    class Pdf
    {
        AppSettings appConfig;

        public Pdf(AppSettings app)
        {
            this.appConfig = app;
        }

        public string CreatePDF(StringBuilder source, BackgroundWorker bw = null, string ArgOrderno = "", string ArgEmail = "")
        {
            try
            {
                string sourceFile = MainForm.appTemp + @"\KontoInfoHtml.html";

                string ordern = "";
                if (ArgOrderno != null)
                    if (ArgOrderno.Length > 4)
                        ordern = ArgOrderno;

                if (ordern.Length == 0 && ArgEmail != null)
                    if (IsValidEmail(ArgEmail))
                        ordern = ArgEmail;

                string destinationFile = MainForm.appTemp + @"\KontoInfo " + ordern + ".pdf";
                if (File.Exists(destinationFile))
                {
                    try
                    {
                        File.Delete(destinationFile);
                    }
                    catch
                    { }
                }
                File.WriteAllText(sourceFile, source.ToString());

                string options = "-B 0 -L 0 -R 0 -T 0 --zoom " + appConfig.pdfZoom + " ";
                if (appConfig.pdfLandscape)
                    options += "-O landscape ";

                Console.WriteLine("PDF argument: " + options + sourceFile + destinationFile);

                var wkhtmltopdf = new ProcessStartInfo();
                wkhtmltopdf.WindowStyle = ProcessWindowStyle.Hidden;
                wkhtmltopdf.FileName = MainForm.filePDFwkhtmltopdf;
                wkhtmltopdf.Arguments = options + "\"" + sourceFile + "\" \"" + destinationFile + "\"";
                wkhtmltopdf.WorkingDirectory = MainForm.settingsPath;
                wkhtmltopdf.CreateNoWindow = true;
                wkhtmltopdf.UseShellExecute = false;

                Process D = Process.Start(wkhtmltopdf);

                D.WaitForExit(20000);

                if (!D.HasExited)
                    throw new TimeoutException("Obs! Det tok for lang tid å lage denne PDF'en. Prøv igjen!");

                int result = D.ExitCode;
                if (result != 0)
                    throw new Exception("wkhtmltopdf feilkode: " + result);

                return destinationFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Ukjent feil oppstod under generering av PDF.", ex);
            }
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


        public bool SendAsEmail(string file, string email)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient smtpServer = new SmtpClient(appConfig.emailSmtpHost);
                    mail.From = new MailAddress(appConfig.emailFromAddress);

                    var addr = new System.Net.Mail.MailAddress(email);
                    mail.To.Add(addr);
                    mail.Subject = "Konto Informasjon - " + appConfig.shopName;
                    mail.Body = "Hei" + Environment.NewLine + Environment.NewLine + "Vedlagt følger informasjon angående ditt klargjorte produkt." +
                        Environment.NewLine + "Har du spørsmål om klargjøringen eller trenger hjelp, kontakt vårt kundesenter på telefon 815 32 000 eller ta kontakt med butikken." +
                        Environment.NewLine + "E-poster sendt til denne avsenderen vil ikke bli besvart.";

                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(file);
                    mail.Attachments.Add(attachment);

                    smtpServer.Port = appConfig.emailSmtpPort;
                    //smtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                    smtpServer.EnableSsl = appConfig.emailSmtpUseSsl;

                    smtpServer.Send(mail);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }


    }
}
