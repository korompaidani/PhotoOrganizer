﻿using Microsoft.Web.WebView2.Wpf;
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

        public async Task<string> RequestCoordinates()
        {
            var script = @"getPosition()";
            return await Control.ExecuteScriptAsync(script);
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
            using (StreamReader reader = new StreamReader(@".\..\..\Resources\Web\OpenLayersMap.html"))
            {
                Control.NavigateToString(reader.ReadToEnd());
            }

        }
    }
}