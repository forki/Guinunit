using Eto.Forms;
using System;

namespace Guinunit
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var application = new Application())
            using (var form = new MainForm())
                application.Run(form);
        }
    }
}
