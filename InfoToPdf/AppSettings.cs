using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace InfoToPdf
{
    [Serializable]
    public class AppSettings
    {
        public int WindowLocationX { get; set; }
        public int WindowLocationY { get; set; }
        public int WindowSizeX { get; set; }
        public int WindowSizeY { get; set; }
        public bool WindowMax { get; set; }

        public decimal pdfZoom { get; set; }
        public bool pdfLandscape { get; set; }
        public int pdfStyle { get; set; }

        public string shopName { get; set; }
        public string emailSmtpHost { get; set; }

        public int emailSmtpPort {get; set;}
        public bool emailSmtpUseSsl {get; set;}
        public string emailFromAddress { get; set; }
        public bool pdfAddBarcode { get; set; }
        public bool warnMissingOrderno { get; set; }
        
        public AppSettings()
        {
            // default settings
            if (WindowSizeX < 835) { WindowSizeX = 835; }
            if (WindowSizeY < 588) { WindowSizeY = 588; }
            if (WindowLocationX <= 0) { WindowLocationX = 50; }
            if (WindowLocationY <= 0) { WindowLocationY = 50; }
            this.pdfZoom = 1;
            this.pdfLandscape = false;
            this.shopName = "Elkjøp";
            this.emailFromAddress = "noreply@elkjop.no";
            this.emailSmtpHost = "smtp.elkjop.no";
            this.emailSmtpPort = 25;
            this.emailSmtpUseSsl = false;
            this.warnMissingOrderno = true;
            this.pdfStyle = 2;
            this.pdfAddBarcode = true;
        }

    }

    public class XmlColor
    {
        private Color color_ = Color.Black;

        public XmlColor() { }
        public XmlColor(Color c) { color_ = c; }


        public Color ToColor()
        {
            return color_;
        }

        public void FromColor(Color c)
        {
            color_ = c;
        }

        public static implicit operator Color(XmlColor x)
        {
            return x.ToColor();
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        [XmlAttribute]
        public string Web
        {
            get { return ColorTranslator.ToHtml(color_); }
            set
            {
                try
                {
                    if (Alpha == 0xFF) // preserve named color value if possible
                        color_ = ColorTranslator.FromHtml(value);
                    else
                        color_ = Color.FromArgb(Alpha, ColorTranslator.FromHtml(value));
                }
                catch (Exception)
                {
                    color_ = Color.Black;
                }
            }
        }

        [XmlAttribute]
        public byte Alpha
        {
            get { return color_.A; }
            set
            {
                if (value != color_.A) // avoid hammering named color if no alpha change
                    color_ = Color.FromArgb(value, color_);
            }
        }

        public bool ShouldSerializeAlpha() { return Alpha < 0xFF; }
    }
}
