using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoToPdf
{
    public static class Static
    {
        public static string AppVersion { get; } = "1.1";
        public static string AppPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf";
            }
        }

        public static string AppSettingsFile
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Settings.xml";
            }
        }

        public static string AppStringsFile
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Strings.xml";
            }
        }

        public static string AppTemp
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Temp";
            }
        }

        public static string ProgramWkhtmltopdf
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\wkhtmltopdf.exe";
            }
        }

        public static string FileHtml
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\InfoPdf\Temp\form.html";
            }
        }
    }
}
