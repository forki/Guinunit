using Guinunit.Models;
using Guinunit.Utilities;
using MyToolkit.Command;
using MyToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Guinunit.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        private readonly IList<TestModule> testAssemblies = new List<TestModule>();

        private bool isRunningTests;
        private TimeSpan duration;
        private int passedCount;
        private int failedCount;
        private int skippedCount;
        private int totalCount;

        public UserSettings UserSettings { get; } = UserSettings.Default;
        public ObservableCollection<TestCase> TestCases { get; } = new ObservableCollection<TestCase>();

        public bool HasTestCases => TestCases.Count > 0;

        public bool IsRunningTests
        {
            get { return isRunningTests; }
            set { Set(ref isRunningTests, value); }
        }

        public TimeSpan Duration
        {
            get { return duration; }
            set { Set(ref duration, value); }
        }

        public int PassedCount
        {
            get { return passedCount; }
            set { Set(ref passedCount, value); }
        }

        public int FailedCount
        {
            get { return failedCount; }
            set { Set(ref failedCount, value); }
        }

        public int SkippedCount
        {
            get { return skippedCount; }
            set { Set(ref skippedCount, value); }
        }

        public int TotalCount
        {
            get { return totalCount; }
            set { Set(ref totalCount, value); }
        }

        public RelayCommand AboutCommand { get; } = new RelayCommand(() => MessageBox.Show("A very basic GUI for NUnit 3", "About Guinunit", MessageBoxButton.OK, MessageBoxImage.Information));
        public RelayCommand ExitCommand { get; } = new RelayCommand(Application.Current.Shutdown);

        public MainWindowModel()
        {
            TestCases.CollectionChanged += OnTestCasesCollectionChanged;
        }

        private void OnTestCasesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged<MainWindowModel>(x => x.HasTestCases);
        }
    }
}