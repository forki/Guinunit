using System.Configuration;
using Eto.Forms;

namespace Guinunit
{
    public sealed class UserSettings : ApplicationSettingsBase
    {
        public static UserSettings Default { get; } = new UserSettings();

        private const string MAIN_WINDOW_STATE = "MainWindowState";

        [UserScopedSetting]
        [DefaultSettingValue("Normal")]
        public WindowState MainWindowState
        {
            get { return (WindowState) this[MAIN_WINDOW_STATE]; }
            set { this[MAIN_WINDOW_STATE] = value; }
        }
    }
}