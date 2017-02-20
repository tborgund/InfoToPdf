using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;

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
                string sourceFile = @"Temp\" + GetTempFilename(".html");
                string destinationFile = @"Temp\" + GetTempFilename(".pdf");

                File.WriteAllText(Static.AppPath + @"\" + sourceFile, source.ToString());

                string options = "-B 0 -L 0 -R 0 -T 0 ";

                Console.WriteLine("PDF argument: " + options + " " + sourceFile + " " + destinationFile);

                var wkhtmltopdf = new ProcessStartInfo();
                wkhtmltopdf.WindowStyle = ProcessWindowStyle.Hidden;
                wkhtmltopdf.FileName = Static.ProgramWkhtmltopdf;
                wkhtmltopdf.Arguments = options + " " + sourceFile + " " + destinationFile;
                wkhtmltopdf.WorkingDirectory = Static.AppPath;
                wkhtmltopdf.RedirectStandardOutput = true;
                wkhtmltopdf.CreateNoWindow = true;
                wkhtmltopdf.UseShellExecute = false;

                Process D = Process.Start(wkhtmltopdf);

                D.WaitForExit(20000);

                if (!D.HasExited)
                    throw new TimeoutException("Obs! Det tok for lang tid å lage denne PDF'en. Prøv igjen!");

                int result = D.ExitCode;
                if (result != 0)
                    throw new Exception("wkhtmltopdf returncode: " + result + " wkhtmltopdf arg: " + wkhtmltopdf.Arguments);

                Console.WriteLine("wkhtmltopdf returncode: " + result);

                return Static.AppPath + @"\" + destinationFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Feil oppstod under generering av PDF", ex);
            }
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string GetTempFilename(string ext)
        {
            string name = Path.GetRandomFileName();
            name = Path.ChangeExtension(name, ext);
            return "infotopdf_" + name;
        }


        public bool SendAsEmail(string file, string email)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient smtpServer = new SmtpClient(appConfig.emailSmtpHost);
                    mail.From = new MailAddress(appConfig.emailFromAddress);

                    var addr = new MailAddress(email);
                    mail.To.Add(addr);
                    mail.Subject = "Konto Informasjon - " + appConfig.shopName;
                    mail.Body = "Hei" + Environment.NewLine + Environment.NewLine + "Vedlagt følger informasjon angående ditt klargjorte produkt." +
                        Environment.NewLine + "Har du spørsmål om klargjøringen eller trenger hjelp, kontakt vårt kundesenter på telefon 815 32 000 eller ta kontakt med butikken." +
                        Environment.NewLine + "E-poster sendt til denne avsenderen vil ikke bli besvart.";

                    Attachment attachment = new System.Net.Mail.Attachment(file);
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
