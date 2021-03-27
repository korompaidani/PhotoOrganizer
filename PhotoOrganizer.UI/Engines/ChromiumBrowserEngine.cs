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