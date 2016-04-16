using Eto.Forms;

namespace Guinunit.Commands
{
    public class ExitCommand : Command
    {
        public ExitCommand()
        {
            MenuText = "E&xit";
        }

        protected override void OnExecuted(System.EventArgs e)
        {
            base.OnExecuted(e);
            Application.Instance.Quit();
        }
    }
}