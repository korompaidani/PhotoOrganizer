using Microsoft.Web.WebView2.Wpf;
using PhotoOrganizer.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Engine
{
    public class ChromiumBrowserEngine
    {
        private static ChromiumBrowserEngine _instance = null;

        private string _initialCoordinates;

        public static ChromiumBrowserEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChromiumBrowserEngine();
                }
                return _instance;
            }
        }

        public static WebView2 ControlInstance
        {
            get { return _instance.Control; }
        }

        public WebView2 Control { get; private set; }

        public bool IsReady { get; private set; }
        
        public async Task RestoreMapDefaults()
        {
            if (IsReady)
            {
                var script = @"disposeMap()";
                await Control.ExecuteScriptAsync(script);
            }
        }

        public void PinInitialLocation(string coordinates)
        {
            _initialCoordinates = coordinates;
            Control.NavigationCompleted += Control_NavigationCompleted;
            if (IsReady)
            {
                Control_NavigationCompleted(null, null);
            }
        }

        private void Control_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(_initialCoordinates)) { return; }

            var longLat = _initialCoordinates.Split(',');
            if (longLat.Length == 2)
            {
                var longitude = longLat[0].Replace("\0", "");
                var latitude = longLat[1].Replace("\0", "");

                if (IsReady)
                {
                    var script = $@"placeInitialPinOnCoordinates({longitude}, {latitude})";
                    Control.ExecuteScriptAsync(script).Await();
                }
            }
        }

        public async Task<string> RequestCoordinates()
        {
            var script = @"getPosition()";
            var scriptResult = await Control.ExecuteScriptAsync(script);
            return scriptResult.Replace("\"", string.Empty).Trim();
        }

        private ChromiumBrowserEngine()
        {
        }

        public void Initialize()
        {
            Control = new WebView2();
            Control.Initialized += Control_Initialized;            
        }

        private async void Control_Initialized(object sender, EventArgs e)
        {
            
            await Control.EnsureCoreWebView2Async(null);
            using (StreamReader reader = new StreamReader(FilePaths.MapLocation))
            {
                Control.NavigateToString(reader.ReadToEnd());
            }
            
            IsReady = true;
        }
    }
}