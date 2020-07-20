using AutoUpdaterDotNET;
using AyarKontrol;
using CefSharp;
using CefSharp.WinForms;
using Chrome.Handlers;
using Indieteur.GlobalHooks;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Chrome
{
    internal partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            req = new RequestHandler(this);
            Icon = Properties.Resources.Ico1; 
        }
       
        #region degisken
        public ChromiumWebBrowser br;
        RequestHandler req;
        string[] satir;
        string url, dosya = Application.StartupPath + "\\ayar", siteisim;
        bool ctrl = false;
        double i = 0;
        GlobalMouseHook globalMouseHook;
        #endregion 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult Cikis;
                Cikis = MessageBox.Show("Program Kapatılacak Emin siniz?", "Kapatma Uyarısı!", MessageBoxButtons.YesNo);
                File.WriteAllText(dosya, url + Environment.NewLine + siteisim + Environment.NewLine + (100 - (br.GetZoomLevelAsync().Result * 10)) + Environment.NewLine);
                if (Cikis == DialogResult.Yes)
                {
                    ghook.unhook();
                    br.Load(br.Address + @"/cikis.php");
                }
                else if (Cikis == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            catch
            {
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CefSettings f = new CefSettings();
            f.EnablePrintPreview();
            Cef.Initialize(f);
            AppDomain.CurrentDomain.AssemblyResolve += Assemblyresolver;
        don:
            if (File.Exists(dosya) == true)
            {
                satir = File.ReadAllLines(dosya);
                if (satir.Length > 0)
                {
                    YeniTabEkle(satir[0]);
                    url = satir[0];
                    siteisim = satir[1];
                    i = Convert.ToDouble(satir[2]);
                }
                else
                {
                    YeniTabEkle("www.google.com");
                    url = ("www.google.com");
                    siteisim = "google";
                    i = 100;
                }
            }
            else
            {
                url = "sipsan.raporzen.com";
                siteisim = "raporzen";
                i = 100;
                File.WriteAllText(dosya, url + Environment.NewLine + siteisim + Environment.NewLine + i);
                goto don;
            }
            i = ((100 - i) / 10) * 2;
            ghook = new GlobalKeyboardHook();
            ghook.KeyDown += new KeyEventHandler(ghook_KeyDown);
            ghook.KeyUp += new KeyEventHandler(ghook_KeyUp);

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                ghook.HookedKeys.Add(key);
            }
            timer1.Interval = 1000;
            timer1.Start();

        }
        AboutBox1 pr;
        public GlobalKeyboardHook ghook;
        private Assembly Assemblyresolver(object sender, ResolveEventArgs args)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var requiredDllName = $"{(new AssemblyName(args.Name).Name)}.dll";
            var resource = currentAssembly.GetManifestResourceNames().Where(s => s.EndsWith(requiredDllName)).FirstOrDefault();

            if (resource != null)
            {
                using (var stream = currentAssembly.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                    {
                        return null;
                    }
                    var block = new byte[stream.Length];
                    stream.Read(block, 0, block.Length);
                    return Assembly.Load(block);
                }
            }
            else
            {
                return null;
            }
        }

        private void Browser_BaslikDegis(object sender, TitleChangedEventArgs e)
        {
            try
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    tabControl1.SelectedTab.Text = e.Title;
                    this.Text = e.Title;
                }));
            }
            catch (Exception)
            {

            }

        }
        #region Eventlar

        private void ghook_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.ContainsFocus)
            {
                if (e.KeyData.ToString().ToUpper().IndexOf("LControlKey".ToUpper()) >= 0)
                {
                    if (globalMouseHook != null)
                    {
                        this.Invoke(new Action(delegate
                        {
                            globalMouseHook.OnMouseWheelScroll -= GlobalMouseHook_OnMouseWheelScroll;
                            globalMouseHook.Dispose();
                            globalMouseHook = null;
                        }));
                    }
                }
            }

        }

        private void GlobalMouseHook_OnMouseWheelScroll(object sender, GlobalMouseEventArgs e)
        {
            if (this.ContainsFocus)
            {
                if (e.wheelRotation.ToString() == "Forwards")
                {

                    this.Invoke(new Action(delegate
                    {

                        if (i < 6)
                        {
                            i++;
                            br.SetZoomLevel(i * 0.5);
                        }

                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        if (i > 0)
                        {
                            i--; br.SetZoomLevel(i);
                        }
                    }));
                }
                ctrl = false;
                e.Handled = true;
            }
        }

        private void tabControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var tabControl = sender as TabControl;
            var tabs = tabControl.TabPages;

            if (e.Button == MouseButtons.Middle)
            {
                tabs.Remove(tabs.Cast<TabPage>()
                        .Where((t, i) => tabControl.GetTabRect(i).Contains(e.Location))
                        .First());
            }
        }

        public void ghook_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ContainsFocus)
            {
                if (e.KeyData == (Keys.Control | Keys.P))
                {
                    this.Invoke(new Action(delegate
                    {
                        br.Print();
                    }));
                }
                if (e.KeyData == (Keys.LControlKey))
                {
                    this.Invoke(new Action(delegate
                    {
                        if (globalMouseHook == null)
                        {
                            globalMouseHook = new GlobalMouseHook();
                            globalMouseHook.OnMouseWheelScroll += GlobalMouseHook_OnMouseWheelScroll;
                        }
                    }));

                }

            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {

            ghook.hook();
        }

        public void YeniTabEkle(string url2)
        {
            TabPage tpage = new TabPage();
            tpage.Text = "Yeni Tab";
            Invoke((MethodInvoker)(() =>
            {
                {
                    tabControl1.Controls.Add(tpage);
                    tabControl1.SelectTab(tabControl1.TabCount - 1);
                }
                brOlustur(url2, tpage);
            }));
        }

        void brOlustur(string d, TabPage t)
        {
            br = new ChromiumWebBrowser(d);
            br.Dock = DockStyle.Fill;
            br.DownloadHandler = new DownloadHandler();
            br.RequestHandler = req;
            br.TitleChanged += Browser_BaslikDegis;
            br.MenuHandler = new CustomMenuHandler();
            t.Controls.Add(br);

        }
    }

}

