using System.Configuration;
using Eto.Drawing;
using Eto.Forms;

namespace Guinunit
{
    public sealed class UserSettings : ApplicationSettingsBase
    {
        public static UserSettings Default { get; } = new UserSettings();

        private const string WINDOW_STATE = "WindowState";
        private const string WINDOW_LOCATION = "WindowLocation";
        private const string WINDOW_SIZE = "WindowSize";

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public WindowState WindowState
        {
            get { return (WindowState) this[WINDOW_STATE]; }
            set { this[WINDOW_STATE] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public Point WindowLocation
        {
            get { return (Point) this[WINDOW_LOCATION]; }
            set { this[WINDOW_LOCATION] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public Size WindowSize
        {
            get { return (Size) this[WINDOW_SIZE]; }
            set { this[WINDOW_SIZE] = value; }
        }
    }
}