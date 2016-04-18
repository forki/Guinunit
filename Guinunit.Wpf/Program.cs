using System;
using Eto;
using Eto.Forms;

namespace Guinunit.Wpf
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var application = new Application(Platforms.Wpf))
            using (var form = new MainForm())
                application.Run(form);
        }
    }
}