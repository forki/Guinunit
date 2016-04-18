using Guinunit.Utilities;
using System.ComponentModel;
using System.Windows;

namespace Guinunit.Views
{
    public partial class MainWindow : Window
    {
        public UserSettings UserSettings => UserSettings.Default;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            UserSettings.Save();
        }
    }
}