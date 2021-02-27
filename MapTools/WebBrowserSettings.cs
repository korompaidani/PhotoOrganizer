using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace PhotoOrganizer.MapTools
{
    public static class WebBrowserSettings
    {
        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {            
            using (var key = Registry.CurrentUser.CreateSubKey(
                string.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        public static void SetBrowserEmulationMode()
        {
            var app = System.AppDomain.CurrentDomain.FriendlyName;
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;
            UInt32 mode = 10000;
            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, mode);
        }
    }
}
