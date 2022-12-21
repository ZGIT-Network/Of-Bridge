using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Of_Bridge
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private bool isSupportDarkMode = Environment.OSVersion.Version.Major == 10 &&
            Environment.OSVersion.Version.Build >= 17763;

        public App()
        {
            if (isSupportDarkMode)
            {
                UxTheme.AllowDarkModeForApp(true);
                UxTheme.ShouldSystemUseDarkMode();
            }
           
        }


    }
}
