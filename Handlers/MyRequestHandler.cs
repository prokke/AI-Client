using AI_Client.Models;
using CefSharp;
using System.Security.Cryptography.X509Certificates;


namespace WpfApp2
{
    public class MyRequestHandler : IRequestHandler
    {

        private readonly ProxySettings _proxySettings;

        public MyRequestHandler(ProxySettings proxySettings)
        {
            _proxySettings = proxySettings;
        }
        bool IRequestHandler.GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {

            if (isProxy == true)
            {
                callback.Continue(_proxySettings.ProxyUsername, _proxySettings.ProxyPassword);
                return true;
            }

            return false;

        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) => default;

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) => default;


        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) => default;


        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }


        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) => default;


        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {

        }


        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }


        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) => default;


    }
}
