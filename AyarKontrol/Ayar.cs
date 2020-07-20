using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AyarKontrol
{
    public partial class Ayar : Form
    {
        string url, dosya = Application.StartupPath + "\\ayar";
        string[] satir;
        public Ayar()
        {
            InitializeComponent();
            if (File.Exists(dosya) == true)
            {
                satir = File.ReadAllLines(dosya);
                if (satir.Length > 0)
                {
                    textBox1.Text = satir[0];
                    textBox2.Text = satir[1];
                    textBox3.Text = satir[2]; 
                } 
            }
        }
    }
}
