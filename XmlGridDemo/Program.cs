using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XmlGridDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var mainForm = new Form1();
            Application.Run(mainForm);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(unhandledExceptionEventArgs.ExceptionObject);
        }
    }
}
