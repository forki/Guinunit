using Guinunit.Utilities;
using System.ComponentModel;
using System.Windows;

namespace Guinunit.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            UserSettings.Default.Save();
        }
    }
}