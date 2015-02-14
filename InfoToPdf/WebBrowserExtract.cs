using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InfoToPdf
{
    public class WebBrowserExtract
    {
        public WebBrowser web;
        public string orderno { get; set; }
        public bool jotta { get; set; }
        public string jottaUser { get; set; }
        public string jottaPass { get; set; }
        public bool jottaUnlimited { get; set; }
        public bool mcafee { get; set; }
        public string mcafeeUser { get; set; }
        public string mcafeePass { get; set; }
        public string mcafeeLicense { get; set; }
        public bool fsecure { get; set; }
        public string fsecureKey { get; set; }
        public bool microsoft { get; set; }
        public string microsoftUser { get; set; }
        public string microsoftPass { get; set; }
        public string microsoftDay { get; set; }
        public string microsoftMonth { get; set; }
        public string microsoftYear { get; set; }
        public bool office { get; set; }
        public string officeKey { get; set; }
        public bool gmail { get; set; }
        public string gmailUser { get; set; }
        public string gmailPass { get; set; }
        public string gmailQuestion { get; set; }
        public string gmailAnswer { get; set; }
        public string gmailDay { get; set; }
        public string gmailMonth { get; set; }
        public string gmailYear { get; set; }
        public bool apple { get; set; }
        public string appleUser { get; set; }
        public string applePass { get; set; }
        public string appleQuestionOne { get; set; }
        public string appleAnswerOne { get; set; }
        public string appleQuestionTwo { get; set; }
        public string appleAnswerTwo { get; set; }
        public string appleQuestionThree { get; set; }
        public string appleAnswerThree { get; set; }
        public string appleDay { get; set; }
        public string appleMonth { get; set; }
        public string appleYear { get; set; }
        public bool email { get; set; }
        public string emailUser { get; set; }
        public string emailPass { get; set; }
        public bool dropbox { get; set; }
        public string dropboxUser { get; set; }
        public string dropboxPass { get; set; }
        public string dropboxDay { get; set; }
        public string dropboxMonth { get; set; }
        public string dropboxYear { get; set; }
        public bool samsung { get; set; }
        public string samsungUser { get; set; }
        public string samsungPass { get; set; }
        public bool pin { get; set; }
        public string pinCode { get; set; }
        public bool comment { get; set; }
        public string commentString { get; set; }

        public void Extract(WebBrowser wb)
        {
            try
            {
                this.web = wb;

                HtmlElementCollection colInput = web.Document.GetElementsByTagName("input");
                foreach (HtmlElement item in colInput)
                {
                    if (item.GetAttribute("id") == "ordrenr")
                        orderno = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "jottacheck")
                        jotta = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "jottauser")
                        jottaUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "jottapass")
                        jottaPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "mcafeecheck")
                        mcafee = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "mcafeeuser")
                        mcafeeUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "mcafeepass")
                        mcafeePass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "mcafeelicense")
                        mcafeeLicense = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "f-seccheck")
                        fsecure = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "fsecure")
                        fsecureKey = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "mscheck")
                        microsoft = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "microsoftuser")
                        microsoftUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "microsoftpass")
                        microsoftPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "msofficecheck")
                        office = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "office")
                        officeKey = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "gmailcheck")
                        gmail = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "gmailuser")
                        gmailUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "gmailpass")
                        gmailPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "gmailspm")
                        gmailAnswer = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "applecheck")
                        apple = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "appleuser")
                        appleUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "applepass")
                        applePass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "applespm1")
                        appleAnswerOne = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "applespm2")
                        appleAnswerTwo = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "applespm3")
                        appleAnswerThree = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "emailcheck")
                        email = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "emailuser")
                        emailUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "emailpass")
                        emailPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "dropboxcheck")
                        dropbox = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "dropboxuser")
                        dropboxUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "dropboxpass")
                        dropboxPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "samsungcheck")
                        samsung = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "samsunguser")
                        samsungUser = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "samsungpass")
                        samsungPass = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "pincheck")
                        pin = Convert.ToBoolean(item.GetAttribute("checked"));
                    else if (item.GetAttribute("id") == "pin")
                        pinCode = item.GetAttribute("value");
                    else if (item.GetAttribute("id") == "commentcheck")
                        comment = Convert.ToBoolean(item.GetAttribute("checked"));
                }

                if (jotta)
                {
                    if (GetOption("jotta-type") == "Ubegrenset")
                        jottaUnlimited = true;
                    else
                        jottaUnlimited = false;
                }
                if (microsoft)
                {
                    microsoftDay = GetOption("ms-day");
                    microsoftMonth = GetOption("ms-month");
                    microsoftYear = GetOption("ms-year");
                }
                if (gmail)
                {
                    gmailQuestion = GetOption("gmailquestion");
                    gmailDay = GetOption("gmail-day");
                    gmailMonth = GetOption("gmail-month");
                    gmailYear = GetOption("gmail-year");
                }
                if (apple)
                {
                    appleQuestionOne = GetOption("appleq1");
                    appleQuestionTwo = GetOption("appleq2");
                    appleQuestionThree = GetOption("appleq3");
                    appleDay = GetOption("apple-day");
                    appleMonth = GetOption("apple-month");
                    appleYear = GetOption("apple-year");
                }
                if (dropbox)
                {
                    dropboxDay = GetOption("dropbox-day");
                    dropboxMonth = GetOption("dropbox-month");
                    dropboxYear = GetOption("dropbox-year");
                }
                if (comment)
                {
                    HtmlElementCollection col = web.Document.GetElementsByTagName("textarea");
                    foreach (HtmlElement item in col)
                    {
                        if (item.GetAttribute("id") == "commentstring")
                            commentString = item.InnerText;
                    }

                }


            }
            catch (Exception ex)
            {
                var error = new Error("Feil ved lesing av variabler.", ex);
                error.ShowDialog();
            }
        }

        private string GetOption(string id)
        {
            try
            {
                var element = web.Document.GetElementById(id);
                dynamic dom = element.DomElement;
                int index = (int)dom.selectedIndex();
                if (index != -1)
                    return element.Children[index].InnerText;
                else
                    return "";
            }
            catch(Exception ex)
            {
                var error = new Error("Feil ved lesing av variabler (option).", ex);
                error.ShowDialog();
                return "";
            }
        }


    }
}
