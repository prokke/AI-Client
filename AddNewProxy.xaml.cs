using AI_Client.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using WpfApp2;


namespace AI_Client
{

    public partial class AddNewProxy : Window
    {
        private readonly MainWindow mainWindow;
        private readonly FileIOService fileIOService;

        private readonly string PATH = $"{Environment.CurrentDirectory}\\ProxyList.json";
        private readonly string PATHURLS = $"{Environment.CurrentDirectory}\\Urls.json";


        public AddNewProxy(MainWindow mainWindow)
        {
            InitializeComponent();
            fileIOService = new FileIOService(PATH, PATHURLS);
            LoadProxyListElements();
            LoadUrlListChips();
            this.mainWindow = mainWindow;
        }

        private void LoadProxyListElements()
        {
            var proxies = fileIOService.LoadProxyList();
            if (proxies != null)
            {
                StackPanel_ProxyList.Children.Clear();

                try
                {

                    foreach (var proxy in proxies)
                    {
                        Chip chip = new Chip
                        {
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            IsDeletable = true,
                            Style = (Style)FindResource("MyChip"),
                            Content = proxy.ProxyName.ToString(),
                        };

                        chip.DeleteClick += HandleChipDelete;
                        chip.Click += HandleChipClick;
                        StackPanel_ProxyList.Children.Add(chip);

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LoadUrlListChips()
        {
            var urls = fileIOService.LoadUrlList();
            if (urls != null)
            {
                StackPanel_UrlList.Children.Clear();

                try
                {

                    foreach (var url in urls)
                    {
                        Chip chip = new Chip
                        {
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            IsDeletable = true,
                            Style = (Style)FindResource("MyChip"),
                            Content = url.NewUrlName.ToString(),
                        };

                        chip.DeleteClick += HandleChipDeleteUrl;
                        StackPanel_UrlList.Children.Add(chip);

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void HandleChipDeleteUrl(object sender, RoutedEventArgs e)
        {
            Chip chipToDelete = (Chip)sender;

            // Logic to remove the chip from the StackPanel
            StackPanel_UrlList.Children.Remove(chipToDelete);
            fileIOService.DeleteUrl(chipToDelete.Content.ToString());
            LoadUrlListChips();
            mainWindow.LoadUrlListElements();
        }
        private void HandleChipClick(object sender, RoutedEventArgs e)
        {
            Chip chipToClick = (Chip)sender;
            mainWindow.LoadSettingsByName(chipToClick.Content.ToString());
            fileIOService.SetLastUsingStatus(chipToClick.Content.ToString());
        }

        private void HandleChipDelete(object sender, RoutedEventArgs e)
        {
            Chip chipToDelete = (Chip)sender;

            // Logic to remove the chip from the StackPanel
            StackPanel_ProxyList.Children.Remove(chipToDelete);
            fileIOService.DeleteProxy(chipToDelete.Content.ToString());
        }
        private void Add_url_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var urlList = new Urls
                {
                    NewUrl = IUrl.Text,
                    NewUrlName = IUrlName.Text
                };
                var urls = fileIOService.LoadUrlList();
                if (urls != null)
                {
                    urls.Add(urlList);
                    fileIOService.SaveUrlList(urls);
                }
                else
                {
                    var newUrlList = new List<Urls>
                    {
                        urlList
                    };
                    fileIOService.SaveUrlList(newUrlList);
                }
                //LoadUrlListElements();
                mainWindow.LoadUrlListElements();
                LoadUrlListChips();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding Url: " + ex.Message);
            }
        }
        
        private void Add_proxy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var proxySettings = new ProxySettings
                {
                    ProxyName = IProxyName.Text,
                    ProxyIP = IProxyIP.Text,
                    ProxyPort = IProxyPort.Text,
                    ProxyUsername = IProxyUsername.Text,
                    ProxyPassword = IProxyPassword.Text,
                    LastUsingProxy = false
                };

                var proxies = fileIOService.LoadProxyList();
                if (proxies != null)
                {
                    proxies.Add(proxySettings);
                    fileIOService.SaveProxyList(proxies);
                }
                else
                {
                    var newProxyList = new List<ProxySettings>
                    {
                        proxySettings
                    };
                    fileIOService.SaveProxyList(newProxyList);
                }
                LoadProxyListElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding proxy: " + ex.Message);
            }
        }

        private void CloseSettingButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsTittleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }


    }
}
