using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pano
{
    public partial class LocalLisansSahte : Form
    {
        public LocalLisansSahte()
        {
            InitializeComponent();
        }

        private void LocalLisansSahte_Load(object sender, EventArgs e)
        {

        }

        int i = 20;
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.TopMost = true;
            i--;
            label1.Text = "Local Lisans Sahtekarlığı ! Orjinal Setup üzerinden \nyüklemelisiniz. \nProgram kapatılıyor. Kalan Saniye :" + i.ToString();
            if(i == 0)
            {
                Application.Exit();
            }
        }

        private void LocalLisansSahte_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
