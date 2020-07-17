
using System;
using System.Windows.Forms;

namespace Chrome
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool kontrol = InternetKontrol();

            if (kontrol != true)
            {
                MessageBox.Show("İnternet bağlantınızı kontrol ediniz.");
            }
            Application.Run(new Form1());
            
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
