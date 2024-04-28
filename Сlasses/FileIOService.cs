using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using AI_Client.Models;

namespace WpfApp2
{
    class FileIOService
    {
        private readonly string PATH;
        private readonly string PATHURLS;

        public FileIOService(string path, string pathurls)
        {
            PATH = path;
            PATHURLS = pathurls;
        }

        public List<ProxySettings> LoadProxyList()
        {
            var fileExists = File.Exists(PATH);
            if (!fileExists)
            {
                File.CreateText(PATH).Dispose();
                return new List<ProxySettings>(); 
            }

            using (var reader = File.OpenText(PATH))
            {
                var fileText = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<List<ProxySettings>>(fileText);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    return new List<ProxySettings>();
                }
            }
        }

        public void SaveProxyList(List<ProxySettings> proxySettings)
        {
            using (StreamWriter writer = File.CreateText(PATH))
            {
                string output = JsonConvert.SerializeObject(proxySettings, Formatting.Indented);
                writer.WriteLine(output);
            }
        }

        public void DeleteProxy(string proxyName)
        {
            var proxies = LoadProxyList();
            var proxyToDelete = proxies.FirstOrDefault(p => p.ProxyName == proxyName);
            if (proxyToDelete == null)
            {
                MessageBox.Show("Proxy not found for deletion.");
                return;
            }

            proxies.Remove(proxyToDelete);
            SaveProxyList(proxies);
        }
        public void SetLastUsingStatus(string proxyName)
        {
            var proxySettings = LoadProxyList();
            foreach (var proxySetting in proxySettings)
            {
                if(proxySetting.ProxyName != proxyName)
                { 
                    proxySetting.LastUsingProxy = false;
                }
                else
                {
                    proxySetting.LastUsingProxy = true;
                }
            }
            SaveProxyList(proxySettings);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<Urls> LoadUrlList()
        {
            var fileExists = File.Exists(PATHURLS);
            if (!fileExists)
            {
                File.CreateText(PATHURLS).Dispose();
                return new List<Urls>();
            }

            using (var reader = File.OpenText(PATHURLS))
            {
                var fileText = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<List<Urls>>(fileText);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    return new List<Urls>();
                }
            }
        }
        public void SaveUrlList(List<Urls> urlList)
        {
            using (StreamWriter writer = File.CreateText(PATHURLS))
            {
                string output = JsonConvert.SerializeObject(urlList, Formatting.Indented);
                writer.WriteLine(output);
            }
        }

        public void DeleteUrl(string UrlName)
        {
            var urls = LoadUrlList();
            var UrlToDelete = urls.FirstOrDefault(p => p.NewUrlName == UrlName);
            if (UrlToDelete == null)
            {
                MessageBox.Show("Url not found for deletion.");
                return;
            }

            urls.Remove(UrlToDelete);
            SaveUrlList(urls);
        }


    }
}
