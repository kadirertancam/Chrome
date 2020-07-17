using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;

namespace Baglanti
{
    class Program
    {
        private const string url = "https://www.google.com/";
        static void Main(string[] args)
        {
            Cef.EnableWaitForBrowsersToClose();
            CefSettings s = new CefSettings();
            Cef.WaitForBrowsersToClose();
            Cef.Initialize(s, performDependencyCheck: true, browserProcessHandler: null);
            bool kontrol = InternetKontrol();  
                                              
            if (kontrol != true)
            {
                Console.WriteLine("İnternet bağlantınızı kontrol ediniz.");
            }
            ChromiumWebBrowser br = new ChromiumWebBrowser();
            br.LoadHtml(url);
        don:
            if (br.IsLoading)
            {
                Console.WriteLine("Yüklendi!");
            }
            else
            {
                goto don;
            }
            Console.ReadKey();
            
        }
        public static bool InternetKontrol()
        {
            try
            {
                System.Net.Sockets.TcpClient kontrol_client = new System.Net.Sockets.TcpClient("www.google.com.tr", 80);
                kontrol_client.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
