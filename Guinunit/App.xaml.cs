using Guinunit.Messages;
using Microsoft.Win32;
using MyToolkit.Messaging;
using System;
using System.Windows;

namespace Guinunit
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Messenger.Default.Register<OpenTestModuleMessage>(OpenTestModule);

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OpenTestModule(OpenTestModuleMessage message)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Add test module",
                Filter = "Assembly files (*.dll)|*.dll|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
                message.CallSuccessCallback(dialog.FileName);
            else
                message.CallFailCallback(null);
        }
    }
}
