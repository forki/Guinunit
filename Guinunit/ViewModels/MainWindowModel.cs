using MyToolkit.Command;
using MyToolkit.Mvvm;
using System.Windows;

namespace Guinunit.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        public RelayCommand AboutCommand { get; } = new RelayCommand(() => MessageBox.Show("A very basic GUI for NUnit 3", "About Guinunit", MessageBoxButton.OK, MessageBoxImage.Information));
        public RelayCommand ExitCommand { get; } = new RelayCommand(() => Application.Current.Shutdown());
    }
}