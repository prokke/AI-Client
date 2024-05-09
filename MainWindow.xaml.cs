using AI_Client.Models;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfApp2;
using MaterialDesignThemes;


namespace AI_Client
{

    public partial class MainWindow : Window
    {
        private ChromiumWebBrowser chromeBrowser;
        private ProxySettings proxySettings;
        private AddNewProxy addNewProxy;
        private FileIOService fileIOService;

        private readonly string PATH = $"{Environment.CurrentDirectory}\\ProxyList.json";
        private readonly string PATHURLS = $"{Environment.CurrentDirectory}\\Urls.json";
        //readonly Urls urls = new Urls();

        public MainWindow()
        {
            InitializeComponent();
            InitializeChromium();
            addNewProxy = new AddNewProxy(this);
            proxySettings = LoadFile();
            var NewUrlList = fileIOService.LoadUrlList();
            LoadUrlListElements();
            if (NewUrlList.Count > 0)
            {
                LoadUrl(NewUrlList[0].NewUrl.ToString()); // Получение первого элемента по индексу 0
                                        // ... используйте firstUrl
            }
            else
            {
                // Список пуст, обработайте ситуацию
            }
            //LoadUrl(urls._ipUrl);
        }

        private ProxySettings LoadFile()
        {
            fileIOService = new FileIOService(PATH, PATHURLS);
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
            fileIOService = new FileIOService(PATH, PATHURLS);
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
                chromeBrowser.Address = "about:blank";
                chromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации Chromium: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (chromeBrowser.IsBrowserInitialized)
            {
                chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
                ProxyConnect();
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
            string caseValue = selectedItem.DataContext.ToString();

            LoadUrl(caseValue);
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
                Direct_Connect();

                MessageBox.Show("No proxy settings loaded, using a direct connection");
            }
        }
        //private void Testproxy_Click(object sender, RoutedEventArgs e)
        //{
        //    ProxyConnect();
        //}
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!addNewProxy.IsLoaded)
            {
                addNewProxy = new AddNewProxy(this)
                {
                    Owner = this
                };
                addNewProxy.ShowDialog();
            }
            else if(addNewProxy.IsLoaded)
            {
                addNewProxy.Close();
            }


        }


        private void TittleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximaizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaxIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }

            else
            {
                WindowState = WindowState.Maximized;
                MaxIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
                
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(8);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }
        public void LoadUrlListElements()
        {
            var urls = fileIOService.LoadUrlList();
            if (urls != null)
            {
                ListBoxUrls.Items.Clear();

                try
                {

                    foreach (var url in urls)
                    {
                        ListBoxItem listBoxItem = new ListBoxItem
                        {
                            //Name = url.NewUrlName,
                            DataContext = url.NewUrl,



                            Content = new TextBlock
                            {
                                Margin = new Thickness(7, 0, 7, 0),
                                Padding = new Thickness(0),
                                VerticalAlignment = VerticalAlignment.Center,
                                
                                Text = url.NewUrlName,
                                Style = FindResource("MaterialDesignTextBlock") as Style,
                                FontSize = 16,
                                FontWeight = FontWeights.Medium,
                            }
                           
                        };
                        
                        listBoxItem.Selected += AI_Selected;
                        listBoxItem.Style = FindResource("MaterialDesignListBoxItem") as Style;
                        ListBoxUrls.Items.Add(listBoxItem);

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Direct_Click(object sender, RoutedEventArgs e)
        {
            Direct_Connect();
        }
        private void Direct_Connect()
        {
            CurentProxyName.Text = proxySettings.ProxyName.ToString();
            Cef.UIThreadTaskFactory.StartNew(delegate
            {

                var rc = chromeBrowser.GetBrowser().GetHost().RequestContext;
                var dict = new Dictionary<string, object>
                    {
                        { "mode", "direct" },
                    };
                bool success = rc.SetPreference("proxy", dict, out string error);
            });
            chromeBrowser?.Reload();
            CurentProxyName.Text = "NONE";
        }

        private void ProxyToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Direct_Connect();

        }

        private void ProxyToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ProxyConnect();

        }
    }
}
