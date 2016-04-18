using System;
using Eto;
using Eto.Forms;

namespace Guinunit.Gtk3
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var application = new Application(Platforms.Gtk3))
            using (var form = new MainForm())
                application.Run(form);
        }
    }
}