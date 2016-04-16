using Eto.Forms;
using Eto.Drawing;

namespace Guinunit
{
    public class MainForm : Form
    {
        public MainForm()
        {
            ClientSize = new Size(600, 400);
            Title = "Guinunit";

            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem
                    {
                        Text = "&File",
                        Items =
                        {
                            new ExitCommand()
                        }
                    }
                }
            };
        }
    }
}