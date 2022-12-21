using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Of_Bridge
{
    internal static class UxTheme
    {
        public const string User32 = "user32.dll";

        public const string Uxtheme = "uxtheme.dll";

        public enum PreferredAppMode
        {
            Default = 0,
            AllowDark = 1,
            ForceDark = 2,
            ForceLight = 3,
            Max = 4
        };

        /*
         * - Mark boolean P/Invoke arguments with MarshalAs
         *   The Boolean representation that is required by the unmanaged method should be determined and matched to the appropriate System.Runtime.InteropServices.UnmanagedType.
         *   UnmanagedType.Bool is the Win32 BOOL type, which is always 4 bytes. UnmanagedType.U1 should be used for C++ bool or other 1-byte types
         *   See https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1414?view=vs-2022#rule-description
         */

        /*
         * OS version 1809 ( OS build 17763 )
         */

        /// <summary>
        /// This undocumented API apparently triggers a refresh/repaint after changing the dark mode of a window
        /// </summary>
        [DllImport(Uxtheme, EntryPoint = "#104")]
        public static extern void RefreshImmersiveColorPolicyState();

        /// <summary>
        /// Returns the 'deafault app mode' on 'Custom' color settings
        /// </summary>
        [DllImport(Uxtheme, EntryPoint = "#132")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ShouldAppsUseDarkMode();

        [DllImport(Uxtheme, EntryPoint = "#133")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool AllowDarkModeForWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.U1)] bool allow);

        [DllImport(Uxtheme, EntryPoint = "#135")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool AllowDarkModeForApp([MarshalAs(UnmanagedType.U1)] bool allow); // 135 in both 1903 and 1809 

        [DllImport(Uxtheme, EntryPoint = "#137")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsDarkModeAllowedForWindow(IntPtr hWnd);

        /*
         * OS version 1903 ( OS build 18362 )
         */

        [DllImport(Uxtheme, EntryPoint = "#135")]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern PreferredAppMode SetPreferredAppMode(PreferredAppMode mode); // 135 in both 1903 and 1809 

        /// <summary>
        /// Returns the 'deafault Windows mode' on 'Custom' color settings
        /// </summary>
        [DllImport(Uxtheme, EntryPoint = "#138")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ShouldSystemUseDarkMode();

        [DllImport(Uxtheme, EntryPoint = "#139")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsDarkModeAllowedForApp(IntPtr hWnd);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        public enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20u,

        }

        public static int TrueValue = 0x01;
        public static int FalseValue = 0x00;
    }
}
