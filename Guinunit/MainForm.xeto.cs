using Eto.Forms;
using Eto.Serialization.Xaml;
using System;
using System.ComponentModel;
using Eto.Drawing;

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
            if (UserSettings.Default.WindowSize.IsEmpty)
            {
                WindowState = WindowState.Normal;
                Location = new Point(100, 100);
                Size = new Size(600, 400);
            }
            else
            {
                WindowState = UserSettings.Default.WindowState;

                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;

                Location = UserSettings.Default.WindowLocation;
                Size = UserSettings.Default.WindowSize;
            }
        }

        private void SaveSettings()
        {
            UserSettings.Default.WindowState = WindowState;

            if (WindowState == WindowState.Normal)
            {
                UserSettings.Default.WindowLocation = Location;
                UserSettings.Default.WindowSize = Size;
            }
            else
            {
                UserSettings.Default.WindowLocation = RestoreBounds.Location;
                UserSettings.Default.WindowSize = RestoreBounds.Size;
            }

            UserSettings.Default.Save();
        }
    }
}