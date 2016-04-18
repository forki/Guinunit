using Eto.Forms;
using Eto.Serialization.Xaml;
using System;

namespace Guinunit
{
    public class MainForm : Form
    {
        public MainForm()
        {
            XamlReader.Load(this);
        }

        protected void HandleClickMe(object sender, EventArgs e)
        {
            MessageBox.Show("I was clicked!");
        }
    }
}