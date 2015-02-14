using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InfoToPdf
{
    static class Program
    {
        /// <summary>
        /// Author: Trond Borgund
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
