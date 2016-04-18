using Eto.Forms;
using Eto.Serialization.Xaml;
using System;
using System.ComponentModel;

namespace Guinunit
{
    public class MainForm : Form
    {
        public MainForm()
        {
            XamlReader.Load(this, "Guinunit.MainForm");
        }

        protected void HandleClickMe(object sender, EventArgs e)
        {
            MessageBox.Show("I was clicked!");
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadSettings();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            WindowState = UserSettings.Default.MainWindowState;
        }

        private void SaveSettings()
        {
            var windowState = WindowState == WindowState.Minimized ? WindowState.Normal : WindowState;

            UserSettings.Default.MainWindowState = windowState;

            UserSettings.Default.Save();
        }
    }
}