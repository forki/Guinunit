using Eto.Forms;
using System;

namespace Guinunit
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) => MessageBox.Show(e.ExceptionObject.ToString(), "Unexpected error", MessageBoxButtons.OK, MessageBoxType.Error);

            using (var application = new Application())
            using (var form = new MainForm())
                application.Run(form);
        }
    }
}