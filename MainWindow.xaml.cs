using AI_Client.Models;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfApp2;


namespace AI_Client
{

    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser chromeBrowser;
        private ProxySettings proxySettings;
        private AddNewProxy addNewProxy;
        private FileIOService fileIOService;

        private readonly string PATH = $"{Environment.CurrentDirectory}\\ProxyList.json";
        readonly Urls urls = new Urls();

        public MainWindow()
        {
            InitializeComponent();
            InitializeChromium();
            addNewProxy = new AddNewProxy(this);
            proxySettings = LoadFile();
            LoadUrl(urls._ipUrl);
        }

        private ProxySettings LoadFile()
        {
            fileIOService = new FileIOService(PATH);
            var proxies = fileIOService.LoadProxyList();
            if (proxies != null)
            {
                try
                {
                    foreach (var proxy in proxies)
                    {
                        if (proxy.LastUsingProxy == true)
                        {
                            return proxy;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при чтении файла конфигурации" + ex.Message);
                    return null;
                }
            }
            return null;
        }

        public void LoadSettingsByName(string name)
        {
            fileIOService = new FileIOService(PATH);
            var proxies = fileIOService.LoadProxyList();
            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    if (proxy.ProxyName == name)
                    {
                        proxySettings = proxy;
                        ProxyConnect();
                        break;
                    }
                }
            }
        }

        private void InitializeChromium()
        {
            try
            {
                CefSettings settings = new CefSettings
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 /CefSharp Browser" + Cef.CefSharpVersion,
                    CachePath = Directory.GetCurrentDirectory() + "\\cache"

                };


                Cef.Initialize(settings);

                chromeBrowser = new ChromiumWebBrowser();
                browsergrid.Children.Add(chromeBrowser);
                Canvas.SetLeft(chromeBrowser, 0);
                Canvas.SetTop(chromeBrowser, 0);
                chromeBrowser.Address = "about:blank";
                chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации Chromium: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.Invoke(() => ProggresBar.Visibility = Visibility.Hidden);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            chromeBrowser?.Reload();
        }

        private void LoadUrl(string url)
        {

            if (chromeBrowser != null)
            {
                ProggresBar.Visibility = Visibility.Visible;
                chromeBrowser.Address = url;
            }

        }
        private void AI_Selected(object sender, RoutedEventArgs e)
        {

            ListBoxItem selectedItem = (ListBoxItem)sender;
            string caseValue = selectedItem.Name.ToString();

            switch (caseValue)
            {
                case "gpt":
                    LoadUrl(urls._gptUrl);
                    break;
                case "claude":
                    LoadUrl(urls._claudeUrl);
                    break;
                case "gemini":
                    LoadUrl(urls._geminiUrl);
                    break;
                case "ip":
                    LoadUrl(urls._ipUrl);
                    break;
                default:
                    break;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Cef.Shutdown();
            foreach (Window w in App.Current.Windows)
                w.Close();

        }
        private void ProxyConnect()
        {
            if (proxySettings != null)
            {
                CurentProxyName.Text = proxySettings.ProxyName.ToString();
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    chromeBrowser.RequestHandler = new MyRequestHandler(proxySettings);

                    string ip = proxySettings.ProxyIP;
                    string port = proxySettings.ProxyPort;
                    var rc = chromeBrowser.GetBrowser().GetHost().RequestContext;
                    var dict = new Dictionary<string, object>
                {
                    { "mode", "fixed_servers" },
                    { "server", "" + ip + ":" + port + "" }
                };
                    bool success = rc.SetPreference("proxy", dict, out string error);
                });
                chromeBrowser?.Reload();
            }
            else
            {
                MessageBox.Show("No proxy settings loaded.");
            }
        }
        private void Testproxy_Click(object sender, RoutedEventArgs e)
        {
            ProxyConnect();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!addNewProxy.IsLoaded)
            {
                addNewProxy = new AddNewProxy(this);
            }

            addNewProxy.Show();
        }
    }
}
