using System;
using Eto;
using Eto.Forms;

namespace Guinunit.Mac
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var application = new Application(Platforms.Mac))
            using (var form = new MainForm())
                application.Run(form);
        }
    }
}