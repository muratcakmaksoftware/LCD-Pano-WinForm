using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Data.OleDb;

namespace Pano
{
    public partial class Ayarlar : Form
    {
        public Ayarlar()
        {
            InitializeComponent();
        }

        SQLiteConnection baglanti = new SQLiteConnection("Data Source=" + Application.StartupPath.ToString() + "\\database.s3db;");

        SQLiteCommand cmd;
        SQLiteCommand cekme;

        public bool guncelleme = false;

        public ArrayList list = new ArrayList();
        public int index = 0;
        public int index2 = 0;

        string gun = "";

        private void Ayarlar_Load(object sender, EventArgs e)
        {

            cmbYayinUrl.Items.Add("TRT");
            cmbYayinUrl.Items.Add("ATV");
            cmbYayinUrl.Items.Add("SHOW TV");
            cmbYayinUrl.Items.Add("Power Türk TV");
            cmbYayinUrl.Items.Add("KRAL TV");
            cmbYayinUrl.Items.Add("FOX");
            cmbYayinUrl.Items.Add("Haber Türk");
            cmbYayinUrl.Items.Add("NTV");
            cmbYayinUrl.Items.Add("NTV SPOR");
            cmbYayinUrl.Items.Add("SİNEMA");
            cmbYayinUrl.Items.Add("SİNEMA1");
            cmbYayinUrl.Items.Add("SİNEMA2");
            cmbYayinUrl.Items.Add("Kanal D");
            cmbYayinUrl.Items.Add("Star TV");

            panel1.Visible = true;
            try
            {
                string Okuladi = "";
                string KayanMetin = "";
                string KayanYaziHizi = "";
                string KayanYaziPiksel = "";
                string NobetciPersonelPiksel = "";
                string Kayma = "";
                string SlaytGecisSaniye = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT * FROM Pano where id=1 ", baglanti);
                SQLiteDataReader dr = cekme.ExecuteReader();
                while (dr.Read())
                {
                    Kayma = dr["Kayma"].ToString();
                    Okuladi = dr["OkulAdi"].ToString();
                    KayanMetin = dr["KayanMetin"].ToString();
                    KayanYaziHizi = dr["KazanYaziHizi"].ToString();
                    KayanYaziPiksel = dr["KayanYaziPiksel"].ToString();
                    SlaytGecisSaniye = dr["SlaytGecisSaniye"].ToString();
                    NobetciPersonelPiksel = dr["NobetciPersonelPiksel"].ToString();
                    if (dr["RadioDers"].ToString() == "TV") cmbDersteyken.SelectedIndex = 1; else cmbDersteyken.SelectedIndex = 0;
                    if (dr["RadioTenefus"].ToString() == "TV") cmbTenefusdeyken.SelectedIndex = 1; else cmbTenefusdeyken.SelectedIndex = 0;
                    break;
                }
                // Dönüştürme.
                int KayanMetinMiliSaniye = Convert.ToInt32(KayanYaziHizi);
                int KayanMetinPiksel = Convert.ToInt32(KayanYaziPiksel);
                int NobetciPersPiksel = Convert.ToInt32(NobetciPersonelPiksel);
                int Kaymax = Convert.ToInt32(Kayma);
                int SlaytGecisSaniyex = Convert.ToInt32(SlaytGecisSaniye);

                string okulAd = Convert.ToString(Okuladi);
                string KayanMet = Convert.ToString(KayanMetin);

                cekme.Dispose();
                dr.Close();
                baglanti.Close();

                txtKayanMetin.Text = KayanMet;
                txtOkulAdi.Text = okulAd;

                // max ve min değerlerini verdikten sonra value değerini alır.

                numKayanMiliSaniye.Minimum = 1;
                numKayanMiliSaniye.Maximum = 10000;

                numKayanPiksel.Minimum = 1;
                numKayanPiksel.Maximum = 100;

                numNobetciPerPiksel.Minimum = 1;
                numNobetciPerPiksel.Maximum = 90;

                numKayanMiliSaniye.Value = KayanMetinMiliSaniye;
                numKayanPiksel.Value = KayanMetinPiksel;
                numNobetciPerPiksel.Value = NobetciPersPiksel;

                numKayma.Minimum = 1;
                numKayma.Maximum = 100;
                numKayma.Value = Kaymax;

                numSlaytSure.Minimum = 1;
                numSlaytSure.Maximum = 600;
                numSlaytSure.Value = SlaytGecisSaniyex;


                gun = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.DayNames[(int)DateTime.Now.DayOfWeek];

                ArrayList kayanMetinList = new ArrayList();

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT KayanMetin, id FROM Pano WHERE 1", baglanti);
                SQLiteDataReader drkay = cekme.ExecuteReader();
                while (drkay.Read())
                {
                    string id = Convert.ToString(drkay["id"].ToString());
                    if (id == "1")
                    {
                        // atla
                    }
                    else
                    {
                        kayanMetinList.Add(Convert.ToString(drkay["KayanMetin"].ToString()));
                    }
                }

                cekme.Dispose();
                drkay.Close();
                baglanti.Close();

                foreach (string kelime in kayanMetinList)
                {
                    listBox1.Items.Add(kelime);
                }

                /////////////////////////////////////////////////////////////////////////

                string Radioa = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT RadioChecked FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drchk = cekme.ExecuteReader();
                while (drchk.Read())
                {
                    Radioa = Convert.ToString(drchk["RadioChecked"].ToString());

                }

                int Radiox = Convert.ToInt32(Radioa);

                cekme.Dispose();
                drchk.Close();
                baglanti.Close();

                if (Radiox == 0)
                {
                    radioButton1.Checked = true;
                }
                else if (Radiox == 1)
                {
                    radioButton2.Checked = true;
                }
                else if (Radiox == 2)
                {
                    radioButton3.Checked = true;

                }
                else if (Radiox == 3)
                {
                    radioButton4.Checked = true;
                }
                else
                {
                    // radio secilmemiş.
                }

                // radiobutton 1

                string MediaUrla = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT MediaUrl FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drurl = cekme.ExecuteReader();
                while (drurl.Read())
                {
                    MediaUrla = drurl["MediaUrl"].ToString();

                }

                cekme.Dispose();
                drurl.Close();
                baglanti.Close();

                lblMedia.Text = MediaUrla.ToString();


                // radiobutton 2

                string MediaYayin = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT MediaYayinUrl FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drYayin = cekme.ExecuteReader();
                while (drYayin.Read())
                {
                    MediaYayin = drYayin["MediaYayinUrl"].ToString();

                }

                cekme.Dispose();
                drYayin.Close();
                baglanti.Close();

                if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000'  id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=trt1&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=trt1&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true'></object>")
                {
                    cmbYayinUrl.Text = "TRT";
                }
                else if(MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='1000' height='600' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=atv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=atv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='1000' height='600'></object>")
                {
                    cmbYayinUrl.Text = "ATV";
                }
                else if(MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=showtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=showtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "SHOW TV";
                }
                else if(MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=powerturktvh.stream&amp;streamer=rtmp://cdn.powergroup.com.tr:80/powertv/&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=powerturktvh.stream&amp;streamer=rtmp://cdn.powergroup.com.tr:80/powertv/&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "Power Türk TV";
                }
                else if (MediaYayin == "<object '='' id='player' name='player' data='http://cdnapi.kaltura.com/index.php/kwidget/wid/_990652/uiconf_id/20952162/entry_id/1/cache_st/556655158345' type='application/x-shockwave-flash'><param name='movie' value='http://cdnapi.kaltura.com/index.php/kwidget/wid/_990652/uiconf_id/20952162/entry_id/1/cache_st/556655158345'><param name='flashvars' value='&amp;alias=kraltvcanli&amp;comscore=kraltvcanli&amp;tags=kraltvcanli&amp;entryId=rtmp%3A%2F%2Fmn-l.mncdn.com%2Fkraltv_live%2Fkraltv1%3Ftoken%3D58d3c1116deee9415ca4daec93c5e2705276c4c385a010f7&amp;sourceType=url&amp;EmbedPlayer.ReplaceSources=%5Bobject%20Object%5D'><param name='allowFullScreen' value='true'><param name='allowNetworking' value='all'><param name='allowScriptAccess' value='always'><param name='bgcolor' value='#000000'></object>")
                {
                    cmbYayinUrl.Text = "KRAL TV";
                }
                else if(MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=foxtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=foxtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "FOX";
                }
                else if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=haberturk.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=haberturk.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "Haber Türk";
                }
                else if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=ntv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=ntv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "NTV";
                }
                else if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=ntvspor.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=ntvspor.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "NTV SPOR";
                }
                else if (MediaYayin == "mms://yayin7.canliyayin.org/sinema")
                {
                    cmbYayinUrl.Text = "SİNEMA";
                }
                else if (MediaYayin == "mms://yayin7.canliyayin.org/sinema1")
                {
                    cmbYayinUrl.Text = "SİNEMA1";
                }
                else if (MediaYayin == "mms://yayin7.canliyayin.org/sinema2")
                {
                    cmbYayinUrl.Text = "SİNEMA2";
                }
                else if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=kanald.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=kanald.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "Kanal D";
                }
                else if (MediaYayin == "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=startv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=startv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>")
                {
                    cmbYayinUrl.Text = "Star TV";
                }
                else
                {
                    // hiçbiri değilse bu yeni bir url.
                    cmbYayinUrl.Text = MediaYayin;
                }
                // radiobutton 3

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT ResimYollari FROM Resim WHERE 1", baglanti);
                SQLiteDataReader dryol = cekme.ExecuteReader();
                while (dryol.Read())
                {
                    listBox2.Items.Add(dryol["ResimYollari"].ToString());

                }

                cekme.Dispose();
                dryol.Close();
                baglanti.Close();
            }
            catch (Exception exx)
            {
                //MessageBox.Show("Ayarlar Yüklemesinde bir hata oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                MessageBox.Show(exx.ToString());
            }


        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if (keyData == Keys.F12)
            {
                if (panel1.Visible == true)
                {
                    button3.PerformClick();
                }
                else
                {
                    // hata
                }
            }
            else
            {
                // hiçbirşey yapma
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            baglanti.Open();

            cmd = new SQLiteCommand("UPDATE Pano SET OkulAdi='" + txtOkulAdi.Text.Replace("'", "''") + "',  KazanYaziHizi=" + numKayanMiliSaniye.Value + ", KayanYaziPiksel=" + numKayanPiksel.Value + ", NobetciPersonelPiksel=" + numNobetciPerPiksel.Value + ", Kayma=" + numKayma.Value + " where id=1 ", baglanti);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            baglanti.Close();

            Form1.guncellemeKontrol = 1;


            if (kontrolListGuncelleme == true)
            {
                ArrayList VeriTabaniListeyiSil = new ArrayList();

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT id FROM Pano WHERE 1", baglanti);
                SQLiteDataReader drsile = cekme.ExecuteReader();
                while (drsile.Read())
                {
                    string aranan = Convert.ToString(drsile["id"].ToString());
                    if (drsile["id"].ToString() == "1")
                    {
                        // boş geç
                    }
                    else
                    {
                        VeriTabaniListeyiSil.Add(Convert.ToString(drsile["id"].ToString()));
                    }


                }

                cekme.Dispose();
                drsile.Close();
                baglanti.Close();

                int countlisteyiSil = VeriTabaniListeyiSil.Count;

                baglanti.Open();
                for (int i = 0; i < countlisteyiSil; i++)
                {
                    cmd = new SQLiteCommand("DELETE FROM Pano WHERE id=" + VeriTabaniListeyiSil[i] + "", baglanti);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                baglanti.Close();


                ArrayList liste2 = new ArrayList();

                foreach (string lis in listBox1.Items)
                {
                    liste2.Add(lis);
                }

                int count = liste2.Count;

                baglanti.Open();
                for (int i = 0; i < count; i++)
                {
                    cmd = new SQLiteCommand("INSERT INTO Pano (KayanMetin)  VALUES ('" + Convert.ToString(liste2[i]).Replace("'", "''") + "')", baglanti);
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
                baglanti.Close();

                kontrolListGuncelleme = false;
            }

            if (radio == 0)
            {
                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET MediaUrl='" + lblMedia.Text.Replace("'", "''") + "', RadioChecked='" + radio + "' where id=1", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();
            }
            else if (radio == 1)
            {
                string yayinlanacakKanal = "";

                if (cmbYayinUrl.Text == "TRT")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000'  id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=trt1&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=trt1&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true'></object>";
                }
                else if(cmbYayinUrl.Text == "ATV")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='1000' height='600' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=atv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=atv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='1000' height='600'></object>";
                }
                else if (cmbYayinUrl.Text == "SHOW TV")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=showtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=showtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "Power Türk TV")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=powerturktvh.stream&amp;streamer=rtmp://cdn.powergroup.com.tr:80/powertv/&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=powerturktvh.stream&amp;streamer=rtmp://cdn.powergroup.com.tr:80/powertv/&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "KRAL TV")
                {
                    yayinlanacakKanal = "<object '='' id='player' name='player' data='http://cdnapi.kaltura.com/index.php/kwidget/wid/_990652/uiconf_id/20952162/entry_id/1/cache_st/556655158345' type='application/x-shockwave-flash'><param name='movie' value='http://cdnapi.kaltura.com/index.php/kwidget/wid/_990652/uiconf_id/20952162/entry_id/1/cache_st/556655158345'><param name='flashvars' value='&amp;alias=kraltvcanli&amp;comscore=kraltvcanli&amp;tags=kraltvcanli&amp;entryId=rtmp%3A%2F%2Fmn-l.mncdn.com%2Fkraltv_live%2Fkraltv1%3Ftoken%3D58d3c1116deee9415ca4daec93c5e2705276c4c385a010f7&amp;sourceType=url&amp;EmbedPlayer.ReplaceSources=%5Bobject%20Object%5D'><param name='allowFullScreen' value='true'><param name='allowNetworking' value='all'><param name='allowScriptAccess' value='always'><param name='bgcolor' value='#000000'></object>";
                }
                else if (cmbYayinUrl.Text == "FOX")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=foxtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=foxtv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "Haber Türk")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=haberturk.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=haberturk.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "NTV")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=ntv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=ntv.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "NTV SPOR")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=ntvspor.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=ntvspor.flv&amp;streamer=rtmp://yayin1.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "SİNEMA")
                {
                    yayinlanacakKanal = "mms://yayin7.canliyayin.org/sinema";
                }
                else if (cmbYayinUrl.Text == "SİNEMA1")
                {
                    yayinlanacakKanal = "mms://yayin7.canliyayin.org/sinema1";
                }
                else if (cmbYayinUrl.Text == "SİNEMA2")
                {
                    yayinlanacakKanal = "mms://yayin7.canliyayin.org/sinema2";
                }
                else if (cmbYayinUrl.Text == "Kanal D")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=kanald.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=kanald.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else if (cmbYayinUrl.Text == "Star TV")
                {
                    yayinlanacakKanal = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' width='370' height='317' id='FlashPlayer'> <param name='movie' value='http://www.canlitv.com/flashplayer/player.swf?file=startv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com'> <param name='allowscriptaccess' value='always'> <param name='allowFullScreen' value='true'> <embed id='FlashPlayer' src='http://www.canlitv.com/flashplayer/player.swf?file=startv.flv&amp;streamer=rtmp://yayin5.canlitv.com/live&amp;provider=rtmp&amp;autostart=1&amp;controlbar.position=over&amp;bufferlength=8&amp;logo=http://www.canlitv.com/flashplayer/img/logo.png&amp;logo.hide=false&amp;logo.position=top-right&amp;logo.link=http://www.canlitv.com' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='370' height='317'></object>";
                }
                else
                {
                    // hiçbiri değilse bu yeni bir url.
                    yayinlanacakKanal = cmbYayinUrl.Text;
                }

                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET MediaYayinUrl='" + yayinlanacakKanal.Replace("'", "''") + "', RadioChecked='" + radio + "' where id=1", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();
            }
            else if (radio == 2)
            {
                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET RadioChecked='" + radio + "' where id=1", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();
            }
            else if (radio == 3)
            {
                if( cmbDersteyken.Text == "" || cmbTenefusdeyken.Text == "")
                {
                    MessageBox.Show("Ders ve Teneffüs Bölmünü Boş Geçemezsiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else if (cmbDersteyken.Text == cmbTenefusdeyken.Text)
                {
                    MessageBox.Show("Ders ve Teneffüs Aynı Giremezsiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    baglanti.Open();
                    cmd = new SQLiteCommand("UPDATE Pano SET RadioChecked='" + radio + "', RadioDers='" + cmbDersteyken.Text + "', RadioTenefus='" + cmbTenefusdeyken.Text + "' where id=1", baglanti);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    baglanti.Close();
                }
            }
            else
            {
                // radio seçilmemiş.
            }

            if (kontrolListSlayt == true)
            {
                // eski kayıtları sil.

                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET SlaytGecisSaniye='" + numSlaytSure.Value + "' where id=1", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();

                baglanti.Open();
                cmd = new SQLiteCommand("DELETE FROM Resim WHERE 1", baglanti); // //DELETE FROM `uye_tablo` WHERE 1
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();

                // Yeni kayıtları ekle.

                ArrayList resimYol = new ArrayList();

                foreach (string lis in listBox2.Items)
                {
                    resimYol.Add(lis);
                }

                int count = resimYol.Count;

                baglanti.Open();
                for (int i = 0; i < count; i++)
                {

                    cmd = new SQLiteCommand("INSERT INTO Resim (ResimYollari)  VALUES ('" + Convert.ToString(resimYol[i]).Replace("'", "''") + "')", baglanti);
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
                baglanti.Close();

                kontrolListSlayt = false;

            }


            MessageBox.Show("Kaydedildi.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }


        private void button9_Click(object sender, EventArgs e)
        {
            
            panel1.Visible = false;
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            /*
            if (txtKayanMetin.Text.IndexOf(" ", 0) == 0)
            {
                // zaten 0  index de boşluk var.
            }
            else
            {
                // 0 indexte boşluk yok.
                txtKayanMetin.Text = " " + txtKayanMetin.Text;
            }*/
            listBox1.Items.Add(txtKayanMetin.Text.ToString());
            kontrolListGuncelleme = true;
        }

        public string mediaUrl = "";

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                using (OpenFileDialog openFile = new OpenFileDialog())
                {
                    openFile.Filter = "All Files|*.*";
                    if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            mediaUrl = openFile.FileName;
                            lblMedia.Text = openFile.FileName;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Video Yüklenemedi.\n\n" + ex.ToString());
                        }
                    }
                }
            }
        }

        public bool kontrolListGuncelleme = false;

        private void button7_Click_1(object sender, EventArgs e)
        {
            //string secilen = listBox1.SelectedItem.ToString();
            //listBox1.Items.Remove(secilen.ToString());
            for (int i = 0; i < listBox1.SelectedItems.Count; )
            {
                listBox1.Items.Remove(listBox1.SelectedItems[i]);
            }

            //listBox1.Items.Remove(listBox1.SelectedItem);
            kontrolListGuncelleme = true;
        }

        public bool kontrolListSlayt = false;

        private void btnResimEkle_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFile2 = new OpenFileDialog();
            DialogResult dr = new DialogResult();
            openFile2.Filter = "Resimler (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" + "All files (*.*)|*.*";
            openFile2.Multiselect = true;
            openFile2.Title = "Resimleri Seçin";
            openFile2.CheckFileExists = true;
            openFile2.CheckPathExists = true;

            dr = openFile2.ShowDialog();
            //DialogResult sonuc = openFile2.ShowDialog();
            string[] fileNames = openFile2.FileNames;

            if (dr == DialogResult.OK)
            {
                try
                {
                    kontrolListSlayt = true;

                    // TEK TEK EKLEME
                    //ımageList1.Images.Add(Image.FromFile(openFile2.FileName));
                    /*
                    Image secilenResim = Image.FromFile(openFile2.FileName);
                    ımageList1.Images.Add(secilenResim);
                    listBox2.Items.Add(openFile2.FileNames.ToString());*/


                    // Birden Fazla Seçim olarak Ekleme
                    string[] dosya = fileNames;
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        //ListViewItem li = new ListViewItem(dosya[i]);
                        listBox2.Items.Add(dosya[i]);
                        //MessageBox.Show(li.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim Eklenemedi.\n\n" + ex.ToString());
                }
            }
        }

        private void btnListeKaldir_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.SelectedItems.Count; )
            {
                listBox2.Items.Remove(listBox2.SelectedItems[i]);
            }
            //listBox2.Items.Remove(listBox2.SelectedItem);
            kontrolListSlayt = true;
        }
       

        public int radio;

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            radio = 0;
            lblMedia.Enabled = true;
            button8.Enabled = true;

            listBox2.Enabled = false;
            btnResimEkle.Enabled = false;
            btnListeKaldir.Enabled = false;
            cmbYayinUrl.Enabled = false;
            numSlaytSure.Enabled = false;
            panel3.Visible = false;
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            radio = 1;
            lblMedia.Enabled = false;
            button8.Enabled = false;
            listBox2.Enabled = false;
            btnResimEkle.Enabled = false;
            btnListeKaldir.Enabled = false;
            numSlaytSure.Enabled = false;
            panel3.Visible = false;

            cmbYayinUrl.Enabled = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radio = 2;
            lblMedia.Enabled = false;
            button8.Enabled = false;
            cmbYayinUrl.Enabled = false;
            panel3.Visible = false;
            
            listBox2.Enabled = true;
            btnResimEkle.Enabled = true;
            btnListeKaldir.Enabled = true;
            numSlaytSure.Enabled = true;

        }



        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radio = 3;

            lblMedia.Enabled = false;
            button8.Enabled = false;
            cmbYayinUrl.Enabled = false;

            listBox2.Enabled = false;
            btnResimEkle.Enabled = false;
            btnListeKaldir.Enabled = false;
            numSlaytSure.Enabled = false;

            panel3.Visible = true;

        }

        private void numSlaytSure_ValueChanged(object sender, EventArgs e)
        {
            kontrolListSlayt = true;
        }

        private void cmbYayinUrl_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void Ayarlar_FormClosing(object sender, FormClosingEventArgs e)
        {
            baglanti.Open();
            cmd = new SQLiteCommand("UPDATE Pano SET ScreenEngel=1", baglanti);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            baglanti.Close();
        }

        Process process;

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                process = Process.Start(Application.StartupPath.ToString() + "\\bilgiler.xls");                
            }
            catch (Exception exx)
            {
                try
                {
                    process.Kill();
                }
                catch
                {

                }
                MessageBox.Show("Excel Dosyası Silinmiş Olabilir veya Başka Bir Program Tarafından Kullanılıyor olabilir. \n\n" + exx.ToString() + "", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            #region saatler
            DataTable dt4 = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [Saatler$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "Saatler");
                adp.Fill(dt4);
            }
            catch
            { MessageBox.Show("Saatlerin Bilgileri Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); }
            if (dt4 == null)
            {
                MessageBox.Show("Saatlerin Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            else
            {
                Form1.sekroSaatler = 1;
                MessageBox.Show("Saatlerin Bilgileri Senkronize Edildi!", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            }
            #endregion

            #region Mekan Sinif Ogretmen
            DataTable dt = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [MekanSinifOgretmen$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "MekanSinifOgretmenBilgi");                
                adp.Fill(dt);
            }
            catch
            { MessageBox.Show("Alan Yerleşim Planı Bilgileri Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); return;}
            if (dt == null)
            {
                MessageBox.Show("Lütfen Alan Yerleşim Planı Bilgileri Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                Form1.sekro = 1;
                MessageBox.Show("Alan Yerleşim Planı Bilgileri Senkronize Edildi!", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
               
            }
            #endregion

            #region Nobetci Ogrenci
                DataTable dt2 = new DataTable();
                try
                {
                    OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [NobetciOgrenci$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                    DataSet ds = new DataSet();
                    adp.Fill(ds, "NobetciOgrenciBilgi");                
                    adp.Fill(dt2);
                }
                catch
                { MessageBox.Show("Nöbetçi Öğrenci Bilgileri Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); return; }
                if (dt2 == null)
                {
                    MessageBox.Show("Lütfen Nöbetçi Öğrenci Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                else
                {
                    bool veriVarMi = false;
                    foreach (DataRow item in dt2.Rows) // veri boş ise girmicek.
                    {
                        veriVarMi = true;
                        DateTime dtx = Convert.ToDateTime(item[0].ToString());
                        string tarih = string.Format("{0:dd MM yyyy}", dtx);

                    }

                    if(veriVarMi) // veri var
                    {
                        Form1.sekroNobetciOgrenci = 1;
                        MessageBox.Show("Nöbetçi Öğrenci Senkronize Edildi!", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        MessageBox.Show("Nöbetçi Öğrenci En az 1 Adet giriniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }
                }
            #endregion

            #region Nöbetçi Öğretmen
                DataTable dt3 = new DataTable();
                try
                {
                    OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [NobetciOgretmen$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                    DataSet ds = new DataSet();
                    adp.Fill(ds, "NobetciOgretmenBilgi");
                    adp.Fill(dt3);
                }
                catch
                { MessageBox.Show("Nöbetçi Öğretmen Bilgileri Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); return; }
                if (dt == null)
                {
                    MessageBox.Show("Lütfen Nöbetçi Öğretmen Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                else
                {
                    int adet5 = 0;
                    foreach (DataRow item in dt3.Rows)
                    {
                        adet5++;
                    }

                    if (adet5 == 5) // adet adet bilgi girilmiş ekrana çıkışı verilebilir
                    {
                        Form1.sekroNobetciOgretmen = 1;
                        MessageBox.Show("Nöbetçi Öğretmen Bilgileri Senkronize Edildi!", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                    else // veri yok !
                        MessageBox.Show("Nöbetçi Öğretmen 5 adet Satır bulunmamakta lütfen gerekli yerleri doldurunuz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }

            #endregion
            

        }




    }
}
