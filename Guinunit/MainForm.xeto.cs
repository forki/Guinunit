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
                Bounds = new Rectangle(new Point(100, 100), new Size(600, 400));
                WindowState = WindowState.Normal;
            }
            else
            {
                Bounds = new Rectangle(UserSettings.Default.WindowLocation, UserSettings.Default.WindowSize);
                WindowState = UserSettings.Default.WindowState;
            }
        }

        private void SaveSettings()
        {
            var bounds = WindowState != WindowState.Normal ? RestoreBounds : Bounds;

            UserSettings.Default.WindowState = WindowState;
            UserSettings.Default.WindowLocation = bounds.Location;
            UserSettings.Default.WindowSize = bounds.Size;

            UserSettings.Default.Save();
        }
    }
}