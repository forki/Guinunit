using System;
using Eto.Forms;

namespace Guinunit.Commands
{
    public class AboutCommand : Command
    {
        public AboutCommand()
        {
            MenuText = "&About";
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            MessageBox.Show("A very basic GUI for NUnit 3", "About Guinunit", MessageBoxButtons.OK, MessageBoxType.Information);
        }
    }
}
