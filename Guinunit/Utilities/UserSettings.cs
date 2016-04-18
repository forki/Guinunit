using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Guinunit.Utilities
{
    public sealed class UserSettings : ApplicationSettingsBase
    {
        public static UserSettings Default { get; } = new UserSettings();

        public double WindowLeft
        {
            get { return WindowLocation.X; }
            set { WindowLocation = new Point(value, WindowLocation.Y); }
        }

        public double WindowTop
        {
            get { return WindowLocation.Y; }
            set { WindowLocation = new Point(WindowLocation.X, value); }
        }

        public double WindowHeight
        {
            get { return WindowSize.Height; }
            set { WindowSize = new Size(WindowSize.Width, value); }
        }

        public double WindowWidth
        {
            get { return WindowSize.Width; }
            set { WindowSize = new Size(value, WindowSize.Height); }
        }

        [UserScopedSetting]
        [DefaultSettingValue("Normal")]
        public WindowState WindowState
        {
            get { return GetValue<WindowState>(); }
            set { SetValue(value); }
        }

        [UserScopedSetting]
        [DefaultSettingValue("100,100")]
        public Point WindowLocation
        {
            get { return GetValue<Point>(); }
            set { SetValue(value); }
        }

        [UserScopedSetting]
        [DefaultSettingValue("600,400")]
        public Size WindowSize
        {
            get { return GetValue<Size>(); }
            set { SetValue(value); }
        }

        [UserScopedSetting]
        [DefaultSettingValue("250")]
        public GridLength TestCasesTreeWidth
        {
            get { return GetValue<GridLength>(); }
            set { SetValue(value); }
        }

        private T GetValue<T>([CallerMemberName] string key = "")
        {
            return (T)this[key];
        }

        private void SetValue(object value, [CallerMemberName] string key = "")
        {
            this[key] = value;
        }
    }
}