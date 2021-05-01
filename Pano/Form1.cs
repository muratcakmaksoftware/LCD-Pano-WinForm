using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using WMPLib;
using System.Data.SQLite;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Management;

namespace Pano
{
    public partial class Form1 : Form
    {

        public Form1()
        {
    
            InitializeComponent();

        }


        SQLiteConnection baglanti = new SQLiteConnection("Data Source=" + Application.StartupPath.ToString() + "\\database.s3db;");

        public DataTable dt = null;

        public DataTable dt2 = null;

        public static int guncellemeRaporluOgretmen = 0;

        SQLiteCommand cekme;
        SQLiteCommand cmd;

        public int KayanDeger;

        ArrayList ResimYollari;

        public static int sekro = -1; // Mekan sınıf öğretmen
        public static int sekroNobetciOgrenci = -1;
        public static int sekroNobetciOgretmen = -1;
        public static int sekroSaatler = -1;
        

        string gun = "";
        string programBaslangicGun = "";
        ArrayList mekanNameLst;
        ArrayList mekanSinif;
        ArrayList mekanOgretmen;

        #region Alan Yerleşim Planı, Nobetçi Öğrenci, Nöbetçi Öğretmen, Saatler
        void kategoriler()
        {            
            gun = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.DayNames[(int)DateTime.Now.DayOfWeek];
            programBaslangicGun = gun;
            dataGridView1.RowHeadersVisible = false; // alt '*' kaldırma
            dataGridView1.ColumnHeadersVisible = false; // columns başlıklarını kaldırma
            DataTable dt = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [MekanSinifOgretmen$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "MekanSinifOgretmenBilgi");
                adp.Fill(dt);
            }
            catch
            { //MessageBox.Show("Alan Yerleşim Planı Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = 1;
                dataGridView1.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Alan Yerleşim Planı Çekerken Bir Hata Oluştu." };
                dataGridView1.Rows.Add(row);
                dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            if (dt == null)
            {
               // MessageBox.Show("Lütfen Alan Yerleşim Planı Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = 1;
                dataGridView1.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Lütfen Alan Yerleşim Planı Bilgileri Boş Geçmeyiniz." };
                dataGridView1.Rows.Add(row);
                dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            else
            {
                dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].Name = "Colums1";
                dataGridView1.Columns[1].Name = "Colums2";
                string[] row = new string[6]; // 6 rowluk dizi :D
                int satirSayisi = 0;
                int sabitSatirSayi = 0;
                string mekanName = "";
                int altiRows = 0;
                mekanNameLst = new ArrayList();
                mekanSinif = new ArrayList();
                mekanOgretmen = new ArrayList();
                bool dahaFazla = false;
                foreach (DataRow item in dt.Rows) // satır satır çekiyoruz.
                {
                    
                    if (altiRows >= 6)
                    {
                        if (item[1].ToString().Trim() == "SatirSayi") // hayla bir satirsayi değişkeni var ise excel demektirki daha fazla bilgi var bunları 6 rows sınır oldugu için dk lı döngü olucak
                        { 
                            dahaFazla = true;
                        }
                    }

                    if (item[1].ToString().Trim() == "SatirSayi")
                    {
                        mekanName = item[0].ToString().Trim();
                        sabitSatirSayi = Convert.ToInt32(item[2].ToString());
                    }

                    if (satirSayisi < sabitSatirSayi) // 0 ile 10 arası B201
                    {
                        if (gun == "Pazartesi")
                        {
                            if (item[0].ToString().Trim() == protokol) // Protokol ile eşleşen o saatteki öğretmeni çağırıyor.
                            {
                                mekanNameLst.Add(mekanName);
                                mekanSinif.Add(item[1].ToString());
                                mekanOgretmen.Add(item[2].ToString());
                                if (altiRows < 6)
                                {
                                    row = new string[] { "" + mekanName + "", "" + item[1].ToString() + "\n" + item[2].ToString() + "" };
                                    dataGridView1.Rows.Add(row);// aktarıldı.
                                }
                                altiRows++;

                            }
                        }
                        else if (gun == "Salı")
                        {
                            if (item[3].ToString().Trim() == protokol)
                            {
                                mekanNameLst.Add(mekanName);
                                mekanSinif.Add(item[4].ToString());
                                mekanOgretmen.Add(item[5].ToString());
                                if (altiRows >= 6)
                                {
                                    row = new string[] { "" + mekanName + "", "" + item[4].ToString() + "\n" + item[5].ToString() + "" };
                                    dataGridView1.Rows.Add(row);
                                }
                                altiRows++;
                            }
                        }
                        else if (gun == "Çarşamba")
                        {
                            if (item[6].ToString().Trim() == protokol)
                            {
                                mekanNameLst.Add(mekanName);
                                mekanSinif.Add(item[7].ToString());
                                mekanOgretmen.Add(item[8].ToString());
                                if (altiRows < 6)
                                {
                                    row = new string[] { "" + mekanName + "", "" + item[7].ToString() + "\n" + item[8].ToString() + "" };
                                    dataGridView1.Rows.Add(row);
                                }
                                altiRows++;
                            }
                        }
                        else if (gun == "Perşembe")
                        {
                            if (item[9].ToString().Trim() == protokol)
                            {
                                mekanNameLst.Add(mekanName);
                                mekanSinif.Add(item[10].ToString());
                                mekanOgretmen.Add(item[11].ToString());
                                if (altiRows < 6)
                                {
                                    row = new string[] { "" + mekanName + "", "" + item[10].ToString() + "\n" + item[11].ToString() + "" };
                                    dataGridView1.Rows.Add(row);
                                }
                                altiRows++;
                            }
                        }
                        else if (gun == "Cuma")
                        {
                            if (item[12].ToString().Trim() == protokol)
                            {
                                mekanNameLst.Add(mekanName);
                                mekanSinif.Add(item[13].ToString());
                                mekanOgretmen.Add(item[14].ToString());
                                if (altiRows < 6)
                                {
                                    row = new string[] { "" + mekanName + "", "" + item[13].ToString() + "\n" + item[14].ToString() + "" };
                                    dataGridView1.Rows.Add(row);
                                }
                                altiRows++;
                            }
                        }
                        else
                        {
                            row = new string[] { "Cumartesi", "Pazar" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }

                    }
                    satirSayisi++;
                }
                    
                try
                {
                    if (dahaFazla == true)
                    {
                        tmrAlanYerlesimGuncelleme.Start();
                    }
                    else
                    {
                        tmrAlanYerlesimGuncelleme.Stop();
                    }
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Width = 140; // kolum 0 genişlik

                    DataGridViewColumn column2 = dataGridView1.Columns[1];
                    column2.Width = 170; // kolum 1 genişlik

                    dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold); // yazı fontu
                    dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Arial", 12);

                    int yukseklikg1 =  dataGridView1.Size.Height / dataGridView1.Rows.Count;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++) // Yükseklik
                    {
                        dataGridView1.Rows[i].Height = yukseklikg1;
                    }
                    bool renkDegis = false;
                    for (int c = 0; c < dataGridView1.Rows.Count; c++) // arka plan rengi
                    {
                        if (!renkDegis)
                        {
                            dataGridView1.Rows[c].DefaultCellStyle.BackColor = Color.LightPink;
                            renkDegis = true;
                        }
                        else
                        {
                            dataGridView1.Rows[c].DefaultCellStyle.BackColor = Color.LightGray;
                            renkDegis = false;
                        }
                    }

                    dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.Black; // yazı rengi
                    dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

                    dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // yazıları ortalama
                    dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dataGridView1.MultiSelect = false;

                    dataGridView1.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView1_DataBindingComplete);// Yazıyı alta geçirme.
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    
                }
                catch
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.ColumnCount = 1;
                    dataGridView1.Columns[0].Name = "Colums1";
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    string[] row2 = new string[] { "Lütfen Alan Yerleşim Planını Senkronize Ediniz." };
                    dataGridView1.Rows.Add(row2);
                    string[] row3 = new string[] { "Oluşabilecek Hatalar: Satır eksikliği,\n Boş Bırakılması, Protokol hatası" };
                    dataGridView1.Rows.Add(row3);
                    dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                    dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                    dataGridView1.Rows[1].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                    dataGridView1.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView1_DataBindingComplete);// Yazıyı alta geçirme.
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView1.Rows[0].Height = 57;
                    dataGridView1.Rows[1].Height = 57;
                }
            }
        }
        // Alan Yerleşim Güncelleme 6 row dan fazlaysa belirlenen dk göre güncellenecek. ve başa sarıcak.
        int kalinanYer = 0;
        private void tmrAlanYerlesimGuncelleme_Tick(object sender, EventArgs e)
        {
            if (mekanNameLst.Count == -1)
            {
            }
            else
            {
                dataGridView1.Rows.Clear();
                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].Name = "Colums1";
                dataGridView1.Columns[1].Name = "Colums2";
                string[] row = new string[6];
                int altiRows = 0;
                for (int i = kalinanYer; i < mekanNameLst.Count; i++)
			    {		 			    
                    if (altiRows == 6)
                    {
                        kalinanYer = i;
                        if(mekanNameLst.Count <= kalinanYer)
                        {
                            kalinanYer = 0;
                        }
                        break; // döngüden çık çünkü 6 adet rows eklendi 2 ci döngü için kalınan yerden devam edicek.
                    }
                    else
                    {
                        if (mekanNameLst.Count == kalinanYer)
                        {
                            kalinanYer = 0;
                        }
                        if (gun == "Pazartesi")
                        {
                            row = new string[] { "" + mekanNameLst[i].ToString() + "", "" + mekanSinif[i].ToString() + "\n" + mekanOgretmen[i].ToString() + "" };
                            dataGridView1.Rows.Add(row);// aktarıldı.
                            altiRows++;
                        }
                        else if (gun == "Salı")
                        {
                            row = new string[] { "" + mekanNameLst[i].ToString() + "", "" + mekanSinif[i].ToString() + "\n" + mekanOgretmen[i].ToString() + "" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }
                        else if (gun == "Çarşamba")
                        {
                            row = new string[] { "" + mekanNameLst[i].ToString() + "", "" + mekanSinif[i].ToString() + "\n" + mekanOgretmen[i].ToString() + "" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }
                        else if (gun == "Perşembe")
                        {
                            row = new string[] { "" + mekanNameLst[i].ToString() + "", "" + mekanSinif[i].ToString() + "\n" + mekanOgretmen[i].ToString() + "" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }
                        else if (gun == "Cuma")
                        {
                            row = new string[] { "" + mekanNameLst[i].ToString() + "", "" + mekanSinif[i].ToString() + "\n" + mekanOgretmen[i].ToString() + "" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }
                        else
                        {
                            row = new string[] { "Cumartesi", "Pazar" };
                            dataGridView1.Rows.Add(row);
                            altiRows++;
                        }
                    }
                    
                }
                
                if (altiRows != 6)
                {
                    kalinanYer = 0;
                }

                // Belirtilen güncellemeler Yapıldı ve Tasarımını yapılıyor
                try
                {
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Width = 140; // kolum 0 genişlik

                    DataGridViewColumn column2 = dataGridView1.Columns[1];
                    column2.Width = 170; // kolum 1 genişlik

                    dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold); // yazı fontu
                    dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Arial", 12);
                    int d2Yukseklik = dataGridView1.Size.Height / dataGridView1.Rows.Count;
                    bool renkDegis = false;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++) // Yükseklik
                    {
                        dataGridView1.Rows[i].Height = d2Yukseklik;
                        if (!renkDegis)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                            renkDegis = true;
                        }
                        else
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                            renkDegis = false;
                        }
                    }

                    dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.Black; // yazı rengi
                    dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

                    dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // yazıları ortalama
                    dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dataGridView1.MultiSelect = false;

                    dataGridView1.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView1_DataBindingComplete);// Yazıyı alta geçirme.
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                catch
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.ColumnCount = 1;
                    dataGridView1.Columns[0].Name = "Colums1";
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    string[] row2 = new string[] { "Lütfen Alan Yerleşim Planını Senkronize Ediniz." };
                    dataGridView1.Rows.Add(row2);
                    string[] row3 = new string[] { "Oluşabilecek Hatalar: Satır eksikliği,\n Boş Bırakılması, Protokol hatası" };
                    dataGridView1.Rows.Add(row3);
                    dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                    dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                    dataGridView1.Rows[1].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                    dataGridView1.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView1_DataBindingComplete);// Yazıyı alta geçirme.
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView1.Rows[0].Height = 57;
                    dataGridView1.Rows[1].Height = 57;
                }
            }
        }
        
        void nobetciOgretmen()
        {
            
            dataGridView3.RowHeadersVisible = false; // alt '*' kaldırma
            dataGridView3.ColumnHeadersVisible = false; // columns başlıklarını kaldırma
            DataTable dt = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [NobetciOgretmen$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "NobetciOgretmenBilgi");
                adp.Fill(dt);
            }
            catch
            {// MessageBox.Show("Nöbetçi Öğretmen Bilgileri Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); 
                dataGridView3.Rows.Clear();
                dataGridView3.ColumnCount = 1;
                dataGridView3.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Nöbetçi Öğretmen Bilgileri Çekerken Bir Hata Oluştu." };
                dataGridView3.Rows.Add(row);
                dataGridView3.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView3.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            if (dt == null)
            {
                //MessageBox.Show("Lütfen Bilgileri Boş Geçmeyiniz.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                dataGridView3.Rows.Clear();
                dataGridView3.ColumnCount = 1;
                dataGridView3.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Nöbetçi Öğretmen Bilgileri Lütfen Bilgileri Boş Geçmeyiniz." };
                dataGridView3.Rows.Add(row);
                dataGridView3.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView3.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            else
            {
                int adet6 = 0;
                foreach (DataRow item in dt.Rows) // veri boş ise girmicek.
                {
                    adet6++;
                }
                if (adet6 == 5) // adet adet bilgi girilmiş ekrana çıkışı verilebilir
                {
                    dataGridView3.Rows.Clear();
                    dataGridView3.ColumnCount = 2;
                    dataGridView3.Columns[0].Name = "Colums1";
                    dataGridView3.Columns[1].Name = "Colums2";
                    if (gun == "Cumartesi" || gun == "Pazar")
                    {
                        string[] row = new string[] { "Cumartesi", "Pazar" };
                        dataGridView3.Rows.Add(row);
                        string[] row2 = new string[] { "Cumartesi", "Pazar" };
                        dataGridView3.Rows.Add(row2);
                    }
                    else
                    {
                        int satir = 0;
                        foreach (DataRow item in dt.Rows) // veri boş ise girmicek.
                        {

                            if (gun == "Pazartesi" && satir == 0)
                            {
                                string[] row = new string[] { "2.KAT", "" + item[1].ToString() + "" };
                                dataGridView3.Rows.Add(row);// aktarıldı.
                                string[] row2 = new string[] { "3.KAT", "" + item[2].ToString() + "" };
                                dataGridView3.Rows.Add(row2);
                            }
                            else if (gun == "Salı" && satir == 1)
                            {
                                string[] row = new string[] { "2.KAT", "" + item[1].ToString() + "" };
                                dataGridView3.Rows.Add(row);// aktarıldı.
                                string[] row2 = new string[] { "3.KAT", "" + item[2].ToString() + "" };
                                dataGridView3.Rows.Add(row2);
                            }
                            else if (gun == "Çarşamba" && satir == 2)
                            {
                                string[] row = new string[] { "2.KAT", "" + item[1].ToString() + "" };
                                dataGridView3.Rows.Add(row);// aktarıldı.
                                string[] row2 = new string[] { "3.KAT", "" + item[2].ToString() + "" };
                                dataGridView3.Rows.Add(row2);
                            }
                            else if (gun == "Perşembe" && satir == 3)
                            {
                                string[] row = new string[] { "2.KAT", "" + item[1].ToString() + "" };
                                dataGridView3.Rows.Add(row);// aktarıldı.
                                string[] row2 = new string[] { "3.KAT", "" + item[2].ToString() + "" };
                                dataGridView3.Rows.Add(row2);
                            }
                            else if (gun == "Cuma" && satir == 4)
                            {
                                string[] row = new string[] { "2.KAT", "" + item[1].ToString() + "" };
                                dataGridView3.Rows.Add(row);// aktarıldı.
                                string[] row2 = new string[] { "3.KAT", "" + item[2].ToString() + "" };
                                dataGridView3.Rows.Add(row2);
                            }
                            satir++;
                        }
                        DataGridViewColumn column = dataGridView3.Columns[0];
                        column.Width = 140; // kolum 0 genişlik

                        DataGridViewColumn column2 = dataGridView3.Columns[1];
                        column2.Width = 170; // kolum 1 genişlik

                        dataGridView3.Columns[0].DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold); // yazı fontu
                        dataGridView3.Columns[1].DefaultCellStyle.Font = new Font("Arial", 12);

                        int d3Yukseklik = dataGridView3.Size.Height / dataGridView3.Rows.Count;
                        for (int i = 0; i < dataGridView3.Rows.Count; i++)
                        {
                            dataGridView3.Rows[i].Height = d3Yukseklik;
                        }

                        dataGridView3.Rows[0].DefaultCellStyle.BackColor = Color.LightPink; // arka plan rengi
                        dataGridView3.Rows[1].DefaultCellStyle.BackColor = Color.LightGray;

                        dataGridView3.Columns[0].DefaultCellStyle.ForeColor = Color.Black; // yazı rengi
                        dataGridView3.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

                        dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // yazıları ortalama
                        dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        dataGridView3.MultiSelect = false;

                        dataGridView3.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView3_DataBindingComplete);// Yazıyı alta geçirme.
                        dataGridView3.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    }

                    
                }
                else
                {
                    dataGridView3.Rows.Clear();
                    dataGridView3.ColumnCount = 1;
                    dataGridView3.Columns[0].Name = "Colums1";
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    string[] row = new string[] { "Nöbetçi Öğretmen 5 Adet Bilgi Bulunmamaktadır." };
                    dataGridView3.Rows.Add(row);
                    string[] row2 = new string[] { "Lütfen Pano Ayarlardan Senkronize Yapın." };
                    dataGridView3.Rows.Add(row2);
                    dataGridView3.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                    dataGridView3.Rows[1].DefaultCellStyle.BackColor = Color.Red;
                    dataGridView3.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                }
            }
        }

        ArrayList saatlerBas;
        ArrayList saatlerTen;
        ArrayList saatlerBit;
        ArrayList saatlerName;
        ArrayList saatlerProtokol;
        int olusanRows = 0;
        void saatler() // Saatler
        {
            dataGridView2.RowHeadersVisible = false; // alt '*' kaldırma
            dataGridView2.ColumnHeadersVisible = false; // kolom başlıklarını kaldırma

            // 2 colums oluşturuldu.
            dataGridView2.ColumnCount = 3;
            dataGridView2.Columns[0].Name = "Colums1";
            dataGridView2.Columns[1].Name = "Colums2";
            dataGridView2.Columns[2].Name = "Colums3";

            DataTable dt = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [Saatler$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "Saatler");
                adp.Fill(dt);
            }
            catch
            { //MessageBox.Show("Saatler Çekerken Bir Hata Oluştu.", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); 
                dataGridView2.Rows.Clear();
                dataGridView2.ColumnCount = 1;
                dataGridView2.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Saatleri Çekerken Bir Hata Oluştu." };
                dataGridView2.Rows.Add(row);
                dataGridView2.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView2.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            if (dt == null)
            {
                //MessageBox.Show("", "Pano Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                dataGridView2.Rows.Clear();
                dataGridView2.ColumnCount = 1;
                dataGridView2.Columns[0].Name = "Colums1";
                DataGridViewColumn column = dataGridView1.Columns[0];
                string[] row = new string[] { "Lütfen Saatlerin Bilgileri Boş Geçmeyiniz." };
                dataGridView2.Rows.Add(row);
                dataGridView2.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                dataGridView2.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                return;
            }
            else
            {
                olusanRows = 0;
                string[] row;
                saatlerBas = new ArrayList();
                saatlerBit = new ArrayList();
                saatlerName = new ArrayList();
                saatlerProtokol = new ArrayList();
                saatlerTen = new ArrayList();
                foreach (DataRow item in dt.Rows) // satır satır çekiyoruz.
                {
                    if (item[0].ToString() == "Teneffüs" || item[0].ToString() == "Öğle Teneffüsü")
                    {
                        saatlerName.Add(item[0].ToString());
                        saatlerBas.Add(""); // Teneffüste ders başlangıç eklenmez.       
                        saatlerBit.Add(item[2].ToString());                        
                        saatlerTen.Add(item[3].ToString());
                        saatlerProtokol.Add(item[4].ToString());
                        row = new string[] { "" + item[0].ToString() + "", "" + item[2].ToString() + "", "" + item[3].ToString() + "" }; // dizi açıldı. bir satır açıldı // 1 colums değeri verildi.
                        dataGridView2.Rows.Add(row);// aktarıldı.
                        olusanRows++;
                    }
                    else
                    {
                        saatlerName.Add(item[0].ToString());
                        saatlerBas.Add(item[1].ToString());
                        saatlerBit.Add(item[2].ToString());
                        saatlerTen.Add(""); //derse teneffüs eklenmez.
                        saatlerProtokol.Add(item[4].ToString());
                        row = new string[] { "" + item[0].ToString() + "", "" + item[1].ToString() + "", "" + item[2].ToString() + "" }; // dizi açıldı. bir satır açıldı // 1 colums değeri verildi.
                        dataGridView2.Rows.Add(row);// aktarıldı.
                        olusanRows++;
                    }
                    
                }

                bool renkCevir = false;

                
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    dataGridView2.Rows[i].Height = dgw2;
                    if(!renkCevir) // false
                    {
                        dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        renkCevir = true;
                    }
                    else // true
                    {
                        dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                        renkCevir = false;
                    }
                    
                    
                }



                DataGridViewColumn column = dataGridView2.Columns[0];
                column.Width = 40; // kolum 0 genişlik ve yükseklik

                DataGridViewColumn column2 = dataGridView2.Columns[1];
                column2.Width = 30; // kolum 1 genişlik ve yükseklik

                DataGridViewColumn column3 = dataGridView2.Columns[2];
                column3.Width = 95;


                dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Arial", 12);
                dataGridView2.Columns[2].DefaultCellStyle.Font = new Font("Arial", 12);


                dataGridView2.Columns[0].DefaultCellStyle.ForeColor = Color.Black; // yazı rengi
                dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.Black;
                dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.Black;

                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // yazıları ortalama
                dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView2.MultiSelect = false;

                dataGridView2.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView2_DataBindingComplete);// Yazıyı alta geçirme.
                dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                timer4.Start();
            }
        }
        #endregion


        void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView2.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        void dataGridView3_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView3.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        ArrayList kayanYazilar = new ArrayList();
        public int KCount;
        void kayanMetin()
        {
            try
            {
                kayanYazilar.Clear();
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT id, KayanMetin FROM Pano WHERE 1", baglanti);
                SQLiteDataReader drekle = cekme.ExecuteReader();
                while (drekle.Read())
                {
                    string aranan = Convert.ToString(drekle["id"].ToString());
                    if (drekle["id"].ToString() == "1")
                    {
                        // boş geç
                    }
                    else
                    {
                        kayanYazilar.Add(Convert.ToString(drekle["KayanMetin"].ToString()));
                    }


                }
                cekme.Dispose();
                drekle.Close();
                baglanti.Close();

                KCount = kayanYazilar.Count;

                if (KCount == 0)
                {
                    lblKayanYazi.Text = "";
                }
                else
                {
                    lblKayanYazi.Text = kayanYazilar[0].ToString();
                }


                kayanSayi = 0;
            }
            catch //(Exception exx)
            {
                //MessageBox.Show(exx.ToString());
                lblKayanYazi.Text = "Kayan Yazi Hatası";
            }
        }

        public int kayanYazilarCount;

        public int ScreenX = 0;
        public int ScreenY = 0;

        string webEditbas = "<html><head><style type='text/css'>*{margin:0;padding:0;width:100%;height:100%;}</style></head><body>";
        string webEditbit = "</body></html>";

        
        private void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                string locallisans = (string)Registry.CurrentUser.OpenSubKey("popnear").GetValue("near");
                if (locallisans != null)
                {
                    if(locallisans == "xixmxtxexm")
                    {
                        // Local lisans onaylandı.
                    }
                }
                else
                {
                    LocalLisansSahte lls = new LocalLisansSahte();
                    lls.Show();
                }

            }
            catch
            {
                LocalLisansSahte lls = new LocalLisansSahte();
                lls.Show();
            }
            lblKayanYazi.Text = "";
            baglanti.Open();
            cmd = new SQLiteCommand("UPDATE Pano SET ScreenEngel=1", baglanti);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            baglanti.Close();
            this.TopMost = true;

            // Açılış başlatma
            try
            {
                
                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                runKey.SetValue("LCD Pano", "\"" + Application.ExecutablePath.ToString() + "\"");
                runKey.Close();
            }
            catch
            { } //Eğer eklenmediyse yönetici olarak çalıştırmamıştır.


            kayanYaziX = lblKayanYazi.Location.X;            
            
            saatler();
            kategoriler();
            kayanMetin();
            nobetciOgrenciSekro();
            nobetciOgretmen();

            MediaPlayer.settings.setMode("loop", true);

            string radioChecked = "";

            baglanti.Open();
            cekme = new SQLiteCommand("SELECT RadioChecked FROM Pano WHERE id=1", baglanti);
            SQLiteDataReader drradio = cekme.ExecuteReader();
            while (drradio.Read())
            {
                radioChecked = drradio["RadioChecked"].ToString();
                break;
            }
            cekme.Dispose();
            drradio.Close();
            baglanti.Close();


            int radioCheckedx = 0;

            if (radioChecked == "")
            {
                radioCheckedx = 1;
            }
            else
            {
                radioCheckedx = Convert.ToInt32(radioChecked);
            }


            

            if (radioCheckedx == 0) // Video
            {
                MediaPlayer.Visible = true;
                picSlayt.Visible = false;
                webBrowser1.Visible = false;
                webBrowser1.DocumentText = "";

                string url = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT MediaUrl FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    url = drUrl["MediaUrl"].ToString();
                    break;
                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();

                MediaPlayer.URL = "" + url.ToString() + "";

                timer3.Enabled = false;

                slayt = "";
            }
            else if (radioCheckedx == 1) // TV 
            {
                MediaPlayer.Visible = false;
                picSlayt.Visible = false;
                webBrowser1.Visible = true;
                MediaPlayer.Ctlcontrols.stop();


                string url = "";
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT MediaYayinUrl FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    url = drUrl["MediaYayinUrl"].ToString();
                    break;
                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();
                try
                {
                    if (url == "mms://yayin7.canliyayin.org/sinema" || url == "mms://yayin7.canliyayin.org/sinema1" || url == "mms://yayin7.canliyayin.org/sinema2")
                    {
                        webBrowser1.DocumentText = "";
                        webBrowser1.Visible = false;
                        MediaPlayer.Visible = true;
                        MediaPlayer.URL = url;
                    }
                    else
                    {
                        MediaPlayer.Ctlcontrols.stop();
                        MediaPlayer.Visible = false;
                        MediaPlayer.URL = "";
                        webBrowser1.Visible = true;
                        webBrowser1.DocumentText = webEditbas + url.ToString() + webEditbit;
                    }
                }
                catch
                { }

                timer3.Enabled = false;

                slayt = "";
            }
            else if (radioCheckedx == 2) // Slayt
            {
                MediaPlayer.Visible = false;
                picSlayt.Visible = true;
                webBrowser1.Visible = false;
                webBrowser1.DocumentText = "";

                ResimYollari = new ArrayList();
                ResimYollari.Clear();
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT ResimYollari FROM Resim WHERE 1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    ResimYollari.Add(drUrl["ResimYollari"].ToString());
                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();

                string saniye = "";
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT SlaytGecisSaniye FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drSaniye = cekme.ExecuteReader();
                while (drSaniye.Read())
                {
                    saniye = drSaniye["SlaytGecisSaniye"].ToString();
                    break;
                }
                cekme.Dispose();
                drSaniye.Close();
                baglanti.Close();

                int saniyex = Convert.ToInt32(saniye);

                timer3.Interval = saniyex * 1000;

                timer3.Enabled = true;

                // bug düzeltme
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.FormBorderStyle = FormBorderStyle.None;
                slayt = "";
                
            }
            else if (radioCheckedx == 3) // Otomatik tv & slayt
            {
                MediaPlayer.Visible = false;
                MediaPlayer.Ctlcontrols.stop();
                webBrowser1.DocumentText = "";

                string slayt = "";
                string tv = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT RadioDers, RadioTenefus FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    slayt = Convert.ToString(drUrl["RadioDers"].ToString());
                    tv = Convert.ToString(drUrl["RadioTenefus"].ToString());

                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();
                /*
                if (slayt == "Slayt" && tv == "TV")
                {
                    slaytVeTv = true;
                    tvVeSlayt = false;
                }
                else if (slayt == "TV" && tv == "Slayt")
                {
                    tvVeSlayt = true;
                    slaytVeTv = false;
                }*/

            }
            else
            {
                // radio secim yapılmamış.
            }



           ScreenX = Screen.PrimaryScreen.Bounds.Width; // Ekran Genişliği
           ScreenY = Screen.PrimaryScreen.Bounds.Height; // Ekran Yüksekliği

           this.MinimumSize = new Size(ScreenX, ScreenY); // Form Max Genişlikleri.
           this.WindowState = FormWindowState.Maximized;// FullScreen

            string Kayma = "";
            string Okuladi = "";
            //string KayanMetin = "";
            string KayanYaziHizi = "";
            string KayanYaziPiksel = "";
            string NobetciPersonelPiksel = "";

            baglanti.Open();
            cekme = new SQLiteCommand("SELECT * FROM Pano where id=1 ", baglanti);
            SQLiteDataReader dr = cekme.ExecuteReader();
            while (dr.Read())
            {
                Kayma = dr["Kayma"].ToString();
                Okuladi = dr["OkulAdi"].ToString();
                KayanYaziHizi = dr["KazanYaziHizi"].ToString();
                KayanYaziPiksel = dr["KayanYaziPiksel"].ToString();
                NobetciPersonelPiksel = dr["NobetciPersonelPiksel"].ToString();
                break;
            }
            // Dönüştürme.
            int KayanMetinMiliSaniye = Convert.ToInt32(KayanYaziHizi);
            int KayanMetinPiksel = Convert.ToInt32(KayanYaziPiksel);
            int NobetciPersPiksel = Convert.ToInt32(NobetciPersonelPiksel);
            int Kaymax = Convert.ToInt32(Kayma);

            string okulAd = Convert.ToString(Okuladi);

            cekme.Dispose();
            dr.Close();
            baglanti.Close();

            KayanDeger = Kaymax;
            lblokul.Text = okulAd;
            lblKayanYazi.Font = new Font("Arial", KayanMetinPiksel, FontStyle.Bold);
            timer1.Interval = KayanMetinMiliSaniye;
            lblNobetciyazi.Font = new Font("Arial", NobetciPersPiksel, FontStyle.Bold);
        }

        private void axShockwaveFlash1_Enter(object sender, EventArgs e)
        {

        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.F1)
            {
                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET ScreenEngel=0", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();
                this.TopMost = false;


                Ayarlar ayar = new Ayarlar();
                ayar.ShowDialog();

                return true;
            }
            else if (keyData == Keys.F2)
            {
                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET ScreenEngel=0", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                baglanti.Close();
                this.TopMost = false;
                DialogResult buton = MessageBox.Show("Programı Kapatmak İstediğinizden Emin Misiniz ?", "LCD Pano", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (buton == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    baglanti.Open();
                    cmd = new SQLiteCommand("UPDATE Pano SET ScreenEngel=1", baglanti);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    baglanti.Close();
                    this.TopMost = true;
                }
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                DialogResult buton = MessageBox.Show("Programı Kapatmak İstediğinizden Emin Misiniz ?", "LCD Pano", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (buton == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    // kapatmak istemiyor ;)
                }
                return true;
            }
            else
            {
                //  hiçbiri değilse işlem yapma.
            }
            return base.ProcessCmdKey(ref msg, keyData);

        }

        public int kayanSayi = 0;

        public int kayanYaziX;

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (kayanYazilar.Count == 0)
            {
                // boş geç
            }
            else
            {
                int lblLocation = lblKayanYazi.Location.X;
                int lblLckCikis = 0 - lblKayanYazi.Size.Width;
                if (lblLckCikis > lblLocation)
                {
                    //başa dön yeni text ile.
                    kayanSayi++;
                    if (kayanYazilar.Count == kayanSayi)
                    {
                        kayanSayi = 0;
                    }
                    lblKayanYazi.Text = kayanYazilar[kayanSayi].ToString();

                    lblKayanYazi.Location = new Point(pictureBox7.Location.X, lblKayanYazi.Location.Y);
                }
                else
                {
                    lblKayanYazi.Left -= KayanDeger;
                }
            }
        }

        public static int guncellemeKontrol = 0;


        public int dakika;

        public int dbguncelleme = 0;
        public int dbROgretmen = 0;

        public int ScreenEngel = 1;
        string slayt = "";
        string tv = "";

        private void timer2_Tick_1(object sender, EventArgs e)
        {
            // Seçiliyi kaldırma.            
            
            baglanti.Open();
            cekme = new SQLiteCommand("SELECT ScreenEngel FROM Pano WHERE id=1", baglanti);
            SQLiteDataReader dreng = cekme.ExecuteReader();
            while (dreng.Read())
            {
                ScreenEngel = Convert.ToInt32(dreng["ScreenEngel"]);
                break;
            }
            cekme.Dispose();
            dreng.Close();
            baglanti.Close();

            if (ScreenEngel == 1)
            {
                this.TopMost = true;
            }
            else
            {
                //this.TopMost = false;
            }


            try
            {
                DataGridViewRow selectedRow2 = dataGridView2.CurrentRow;
                selectedRow2.Selected = false;
                DataGridViewRow selectedRow3 = dataGridView3.CurrentRow;
                selectedRow3.Selected = false;
            }
            catch
            { }

            DateTime dt = DateTime.Now;
            string saatZm = string.Format("{0:HH:mm}", dt);
            lblsaat.Text = saatZm.ToString();

            string tarih = string.Format("{0:dd MMM yyyy}", dt);
            lblTarih.Text = tarih + ", " + CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.DayNames[(int)DateTime.Now.DayOfWeek];

            /////////////////////////
            //////// Database kaydet kontrol
            ////////////////////////////

            string dbguncelstr = "";
            string dbguncelRogretmen = "";

            baglanti.Open();
            cekme = new SQLiteCommand("SELECT guncelleme1, guncellemeRogretmen FROM Pano WHERE id=1", baglanti);
            SQLiteDataReader drdb = cekme.ExecuteReader();
            while (drdb.Read())
            {
                dbguncelstr = Convert.ToString(drdb["guncelleme1"].ToString());
                dbguncelRogretmen = Convert.ToString(drdb["guncellemeRogretmen"].ToString());
                break;
            }
            cekme.Dispose();
            drdb.Close();
            baglanti.Close();

            dbguncelleme = Convert.ToInt32(dbguncelstr);
            dbROgretmen = Convert.ToInt32(dbguncelRogretmen);

            gun = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.DayNames[(int)DateTime.Now.DayOfWeek]; // gün güncellemesi

            if (gun != programBaslangicGun) // gün değişmiş
            {
                saatler();
                lblNobetciOgrenci.Text = "";               
                kategoriler();
                nobetciOgrenciSekro();
                nobetciOgretmen();
            }

            

            if (guncellemeKontrol == 1 || dbguncelleme == 1)
            {
                guncellemeKontrol = 0;


                baglanti.Open();
                cmd = new SQLiteCommand("UPDATE Pano SET guncelleme1='0' WHERE id=1", baglanti);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd.Clone();
                baglanti.Close();


                string Kayan = "";
                string Okuladi = "";
                string KayanYaziHizi = "";
                string KayanYaziPiksel = "";
                string NobetciPersonelPiksel = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT * FROM Pano where id=1 ", baglanti);
                SQLiteDataReader dr = cekme.ExecuteReader();
                while (dr.Read())
                {
                    Kayan = dr["Kayma"].ToString();
                    Okuladi = dr["OkulAdi"].ToString();
                    KayanYaziHizi = dr["KazanYaziHizi"].ToString();
                    KayanYaziPiksel = dr["KayanYaziPiksel"].ToString();
                    NobetciPersonelPiksel = dr["NobetciPersonelPiksel"].ToString();
                    break;
                }
                // Dönüştürme.
                int KayanMetinMiliSaniye = Convert.ToInt32(KayanYaziHizi);
                int KayanMetinPiksel = Convert.ToInt32(KayanYaziPiksel);
                int NobetciPersPiksel = Convert.ToInt32(NobetciPersonelPiksel);
                int Kayanx = Convert.ToInt32(Kayan);
                string okulAd = Convert.ToString(Okuladi);

                cekme.Dispose();
                dr.Close();
                baglanti.Close();

                KayanDeger = Kayanx;
                lblokul.Text = okulAd;
                //lblKayanYazi.Text = KayanMet;
                lblKayanYazi.Font = new Font("Arial", KayanMetinPiksel, FontStyle.Bold);
                timer1.Interval = KayanMetinMiliSaniye;
                lblNobetciyazi.Font = new Font("Arial", NobetciPersPiksel, FontStyle.Bold);

                kayanMetin();

                string radioChecked = "";

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT RadioChecked FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drradio = cekme.ExecuteReader();
                while (drradio.Read())
                {
                    radioChecked = drradio["RadioChecked"].ToString();
                    break;
                }
                cekme.Dispose();
                drradio.Close();
                baglanti.Close();

                // convert
                int radioCheckedx = Convert.ToInt32(radioChecked);

                if (radioCheckedx == 0) // Video
                {
                    MediaPlayer.Visible = true;
                    picSlayt.Visible = false;
                    webBrowser1.Visible = false;
                    webBrowser1.DocumentText = "";


                    string url = "";

                    baglanti.Open();
                    cekme = new SQLiteCommand("SELECT MediaUrl FROM Pano WHERE id=1", baglanti);
                    SQLiteDataReader drUrl = cekme.ExecuteReader();
                    while (drUrl.Read())
                    {
                        url = drUrl["MediaUrl"].ToString();
                        break;
                    }
                    cekme.Dispose();
                    drUrl.Close();
                    baglanti.Close();

                    MediaPlayer.URL = "" + url.ToString() + "";

                    timer3.Enabled = false;

                    slayt = "";
                }
                else if (radioCheckedx == 1) // TV
                {
                    MediaPlayer.Ctlcontrols.stop();
                    MediaPlayer.Visible = false;
                    picSlayt.Visible = false;
                    webBrowser1.Visible = true;
                    MediaPlayer.URL = "";

                    string url = "";
                    baglanti.Open();
                    cekme = new SQLiteCommand("SELECT MediaYayinUrl FROM Pano WHERE id=1", baglanti);
                    SQLiteDataReader drUrl = cekme.ExecuteReader();
                    while (drUrl.Read())
                    {
                        url = drUrl["MediaYayinUrl"].ToString();
                        break;
                    }
                    cekme.Dispose();
                    drUrl.Close();
                    baglanti.Close();
                    try
                    {
                        if (url == "mms://yayin7.canliyayin.org/sinema" || url == "mms://yayin7.canliyayin.org/sinema1" || url == "mms://yayin7.canliyayin.org/sinema2")
                        {
                            webBrowser1.DocumentText = "";
                            webBrowser1.Visible = false;
                            MediaPlayer.Visible = true;
                            MediaPlayer.URL = url;                            
                        }
                        else
                        {
                            MediaPlayer.Ctlcontrols.stop();
                            MediaPlayer.Visible = false;
                            MediaPlayer.URL = "";
                            webBrowser1.Visible = true;
                            webBrowser1.DocumentText = webEditbas + url.ToString() + webEditbit;
                        }
                    }
                    catch
                    { }

                    timer3.Enabled = false;

                    slayt = "";
                }
                else if (radioCheckedx == 2) // slayt
                {
                    MediaPlayer.URL = "";
                    MediaPlayer.Ctlcontrols.stop();
                    MediaPlayer.Visible = false;
                    webBrowser1.Visible = false;
                    picSlayt.Visible = true;
                    ResimYollari = new ArrayList();
                    webBrowser1.DocumentText = "";

                    baglanti.Open();
                    cekme = new SQLiteCommand("SELECT ResimYollari FROM Resim WHERE 1", baglanti);
                    SQLiteDataReader drUrl = cekme.ExecuteReader();
                    while (drUrl.Read())
                    {
                        ResimYollari.Add(drUrl["ResimYollari"].ToString());
                    }
                    cekme.Dispose();
                    drUrl.Close();
                    baglanti.Close();

                    string saniye = "";
                    baglanti.Open();
                    cekme = new SQLiteCommand("SELECT SlaytGecisSaniye FROM Pano WHERE id=1", baglanti);
                    SQLiteDataReader drSaniye = cekme.ExecuteReader();
                    while (drSaniye.Read())
                    {
                        saniye = drSaniye["SlaytGecisSaniye"].ToString();
                        break;
                    }
                    cekme.Dispose();
                    drSaniye.Close();
                    baglanti.Close();

                    int saniyex = Convert.ToInt32(saniye);


                    timer3.Interval = saniyex * 1000;

                    timer3.Enabled = true;

                    slayt = "";
                }
                else if (radioCheckedx == 3) // otomatik
                {
                    MediaPlayer.Visible = false;
                    MediaPlayer.Ctlcontrols.stop();
                    webBrowser1.DocumentText = "";
                    MediaPlayer.URL = "";
                    

                    baglanti.Open();
                    cekme = new SQLiteCommand("SELECT RadioDers, RadioTenefus FROM Pano WHERE id=1", baglanti);
                    SQLiteDataReader drUrl = cekme.ExecuteReader();
                    while (drUrl.Read())
                    {
                        slayt = Convert.ToString(drUrl["RadioDers"].ToString());
                        tv = Convert.ToString(drUrl["RadioTenefus"].ToString());

                    }
                    cekme.Dispose();
                    drUrl.Close();
                    baglanti.Close();
                    
                    //Saatleri Güncelle Eğer otomatikteyse
                    protokol = "";
                    protokolDegistimi = "";
                    saatler();
                }
                else
                {
                    // radio secim yapılmamış.
                }
            }
            else
            {
                // Güncelleme Yok 
            }
            // dgw2 yükseklik ayarlarını sureklı guncel tut.
            
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Height = dgw2;
            }

            if(sekroSaatler == 1)
            {
                sekroSaatler = -1;
                protokol = "";
                protokolDegistimi = "";
                saatler();
            }

            if(sekro == 1)
            {
                sekro = -1;
                dataGridView1.Rows.Clear(); //mekan sinif öğretmenler
                kategoriler();
            }

            if(sekroNobetciOgrenci == 1)
            {
                sekroNobetciOgrenci = -1;
                lblNobetciOgrenci.Text = "";
                nobetciOgrenciSekro();
            }

            if (sekroNobetciOgretmen == 1)
            {
                dataGridView3.Rows.Clear();
                sekroNobetciOgretmen = -1;
                nobetciOgretmen();
            }

            
        }

        void nobetciOgrenciSekro()
        {
            DateTime dt = DateTime.Now;
            string tarih = string.Format("{0:dd MM yyyy}", dt);

            DataTable dt2 = new DataTable();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [NobetciOgrenci$]", "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath.ToString() + "\\bilgiler.xls; Extended Properties=Excel 8.0");

                DataSet ds = new DataSet();
                adp.Fill(ds, "NobetciOgrenciBilgi");
                adp.Fill(dt2);
            }
            catch
            { lblNobetciOgrenci.Text = "Lütfen Pano Ayarlardan Senkronize yapın !"; }//Nöbetçi Öğrenci Bilgileri Çekerken Bir Hata Oluştu.
            if (dt2 == null)
            {
                lblNobetciOgrenci.Text = "Lütfen Pano Ayarlardan Senkronize yapın !";
                lblNobetciOgrenci.Location = new Point(((pictureBox5.Location.X + pictureBox4.Location.X + 15) / 2) - (lblNobetciOgrenci.Size.Width / 2), lblNobetciOgrenci.Location.Y);
               
            }
            else
            {
                if (gun == "Cumartesi" || gun == "Pazar")
                {
                    lblNobetciOgrenci.Text = "Cumartesi Pazar Nöbetçi Öğrenci Yok!";
                }
                else
                {
                    bool veriVarMi = false;
                    foreach (DataRow item in dt2.Rows) // veri boş ise girmicek.
                    {
                        DateTime dtx = Convert.ToDateTime(item[0].ToString()); // herhangi bir formattaki tarihi alıp kendi formatıma çevirip kontrol etme
                        string tarihx = string.Format("{0:dd MM yyyy}", dtx);
                        if (tarih == tarihx) // eğer tarihle eşleşiyorsa; o günün nöbeticisi o dur.
                        {
                            veriVarMi = true;
                            lblNobetciOgrenci.Text = item[3].ToString() + "  " + item[1].ToString() + "  " + item[2].ToString();
                            lblNobetciOgrenci.Location = new Point(((pictureBox5.Location.X + pictureBox4.Location.X + 15) / 2) - (lblNobetciOgrenci.Size.Width / 2), lblNobetciOgrenci.Location.Y);
                            break;// döngüden çık
                        }
                    }
                    
                    if (veriVarMi) // veri var
                    {
                        // veri yazıldı.
                    }
                    else
                    {
                        lblNobetciOgrenci.Text = "Lütfen Pano Ayarlardan Senkronize yapın !";//Nöbetçi Öğrenci En az 1 Adet giriniz
                    }
                }
            }
        }

        //public int resimindex = 0;
        public int resimcount;
        public int dongu = -1;

        private void timer3_Tick_1(object sender, EventArgs e)
        {
            resimcount = ResimYollari.Count;
            if (resimcount == 0)
            {
                picSlayt.Image = Resource1.resimYok;
            }
            else
            {
                dongu++;
                if (resimcount == dongu)
                {
                    dongu = -1;
                    picSlayt.Image = Image.FromFile(ResimYollari[0].ToString());

                }
                else
                {
                    try
                    {
                        picSlayt.Image = Image.FromFile(ResimYollari[dongu].ToString());
                    }
                    catch
                    {
                        try
                        {
                            //picSlayt.Image = Image.FromFile(ResimYollari[0].ToString());
                            dongu = -1;
                        }
                        catch
                        {
                            picSlayt.Image = Resource1.resimYok;
                        }
                    }

                }
            }
        }

        bool dersOk = false;
        bool tenOk = false;
        void otomatikSaatDersVeTeneffusDegisim()
        {
            if (dersOk == true) 
            {
                ///////////////////////////////////
                webBrowser1.DocumentText = "";
                webBrowser1.Visible = false;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.Visible = false;
                picSlayt.Visible = true;
                ResimYollari = new ArrayList();

                baglanti.Open();
                cekme = new SQLiteCommand("SELECT ResimYollari FROM Resim WHERE 1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    ResimYollari.Add(drUrl["ResimYollari"].ToString());
                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();

                string saniye = "";
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT SlaytGecisSaniye FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drSaniye = cekme.ExecuteReader();
                while (drSaniye.Read())
                {
                    saniye = drSaniye["SlaytGecisSaniye"].ToString();
                    break;
                }
                cekme.Dispose();
                drSaniye.Close();
                baglanti.Close();

                int saniyex = Convert.ToInt32(saniye);

                timer3.Interval = saniyex * 1000;

                timer3.Enabled = true;
            }

            if (tenOk == true)
            {

                ///////////////////

                webBrowser1.Visible = true;
                picSlayt.Visible = false;

                string url = "";
                baglanti.Open();
                cekme = new SQLiteCommand("SELECT MediaYayinUrl FROM Pano WHERE id=1", baglanti);
                SQLiteDataReader drUrl = cekme.ExecuteReader();
                while (drUrl.Read())
                {
                    url = drUrl["MediaYayinUrl"].ToString();
                    break;
                }
                cekme.Dispose();
                drUrl.Close();
                baglanti.Close();
                if (url == "mms://yayin7.canliyayin.org/sinema" || url == "mms://yayin7.canliyayin.org/sinema1" || url == "mms://yayin7.canliyayin.org/sinema2")
                {
                    webBrowser1.DocumentText = "";
                    webBrowser1.Visible = false;
                    MediaPlayer.Visible = true;
                    MediaPlayer.URL = url;
                }
                else
                {
                    MediaPlayer.Ctlcontrols.stop();
                    MediaPlayer.Visible = false;
                    MediaPlayer.URL = "";
                    webBrowser1.Visible = true;
                    webBrowser1.DocumentText = webEditbas + url.ToString() + webEditbit;
                }

                timer3.Enabled = false;
            }
        }

        string protokolDegistimi = "";
        string protokol = "";
        int dersBittiMi = 1;
        private void timer4_Tick_1(object sender, EventArgs e)
        {

            string gun = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.DayNames[(int)DateTime.Now.DayOfWeek];
            DateTime suankiSaat = Convert.ToDateTime(DateTime.Now.ToShortTimeString());


            if (gun == "Cumartesi" || gun == "Pazar")
            {
                //tmr.Stop();
                //tmr2.Stop();
                lblTenefus.Text = "";
                lblDers.Text = "Hafta Sonu";
                lblDers.Location = new Point((panel4.Size.Width / 2) - (lblDers.Size.Width / 2), 5);
                lblTenefus.Location = new Point((panel4.Size.Width / 2) - (lblTenefus.Size.Width / 2), 39);
                

            }
            else
            {
                
                try
                {
                    dersBittiMi = 1;
                    for (int i = 0; i < saatlerName.Count; i++)
                    {
                        if (saatlerBas[i].ToString() != "" && suankiSaat >= Convert.ToDateTime(saatlerBas[i]) && suankiSaat < Convert.ToDateTime(saatlerBit[i]))
                        {
                                TimeSpan fark = Convert.ToDateTime(saatlerBit[i]) - Convert.ToDateTime(suankiSaat);
                                lblTenefus.Text = "Teneffüse " + fark.Hours + " : " + fark.Minutes + " dk";
                                protokol = saatlerProtokol[i].ToString();

                                if (protokol == protokolDegistimi)
                                {
                                    break;// Aynı protokol döngüden çık
                                }
                                else if (protokol != protokolDegistimi || protokolDegistimi == "")
                                {
                                    
                                    lblDers.Text = saatlerName[i].ToString();
                                    //Ders
                                    lblDers.Location = new Point((panel4.Size.Width / 2) - (lblDers.Size.Width / 2), 5);
                                    lblTenefus.Location = new Point((panel4.Size.Width / 2) - (lblTenefus.Size.Width / 2), 39);
                                    fark = Convert.ToDateTime(saatlerBit[i]) - Convert.ToDateTime(suankiSaat);
                                    lblTenefus.Text = "Teneffüse " + fark.Hours + " : " + fark.Minutes + " dk";

                                    bool renkDegis = false;
                                    for (int c = 0; c < dataGridView2.Rows.Count; c++)
                                    {
                                        if (c == i)// eğer şuan gösterilen saat birimi i değişkeni ile eşit ise kırmızı ile boyanıcak çünkü o saatteler
                                        {
                                            dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Red;

                                        }
                                        else // eğer o saatte olmayan bütün rowslar ise white ve lightgray renk karışımı ile boyanıcaklar.
                                        {
                                            if (!renkDegis)
                                            {
                                                dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.White;
                                                renkDegis = true;
                                            }
                                            else
                                            {
                                                dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.LightGray;
                                                renkDegis = false;
                                            }

                                        }

                                    }
                                    protokolDegistimi = protokol;
                                    // Bilgileri Güncelleme !
                                    sekro = 1;
                                    //DERSTEYKEN
                                    if (slayt == "Slayt") // slayt = derste iken slayt demek tenefüsteyken TV demek
                                    {
                                        dersOk = true; // slayt açıldı.
                                        tenOk = false; // tv kapatıldı. 
                                    }
                                    else if (slayt == "TV") // slayt == TV dersteyken TV tenefüsteyken Slayt
                                    {
                                        tenOk = true; // tv Açıldı
                                        dersOk = false; // slayt kapatıldı
                                    }
                                    otomatikSaatDersVeTeneffusDegisim();
                                }
                                   
                            }
                            else if (saatlerTen[i].ToString() != "" && Convert.ToDateTime(suankiSaat) >= Convert.ToDateTime(saatlerBit[i]) && Convert.ToDateTime(suankiSaat) < Convert.ToDateTime(saatlerTen[i]))
                            {
                                TimeSpan fark = Convert.ToDateTime(saatlerTen[i]) - Convert.ToDateTime(suankiSaat);
                                lblTenefus.Text = "Derse Girmeye " + fark.Hours + " : " + fark.Minutes + " dk";
                                protokol = saatlerProtokol[i].ToString();

                                if (protokol == protokolDegistimi)
                                {
                                    break;// Aynı protokol döngüden çık
                                }
                                else if (protokol != protokolDegistimi || protokolDegistimi == "")
                                {
                                    protokolDegistimi = protokol;
                                    lblDers.Text = saatlerName[i].ToString();
                                    //Teneffüs
                                    lblDers.Location = new Point((panel4.Size.Width / 2) - (lblDers.Size.Width / 2), 5);
                                    lblTenefus.Location = new Point((panel4.Size.Width / 2) - (lblTenefus.Size.Width / 2), 39);
                                    fark = Convert.ToDateTime(saatlerTen[i]) - Convert.ToDateTime(suankiSaat);
                                    lblTenefus.Text = "Derse Girmeye " + fark.Hours + " : " + fark.Minutes + " dk";
                                    bool renkDegis = false;
                                    for (int c = 0; c < dataGridView2.Rows.Count; c++)
                                    {
                                        if (c == i)// eğer şuan gösterilen saat birimi i değişkeni ile eşit ise kırmızı ile boyanıcak çünkü o saatteler
                                        {
                                            dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                        }
                                        else // eğer o saatte olmayan bütün rowslar ise white ve lightgray renk karışımı ile boyanıcaklar.
                                        {
                                            if (!renkDegis)
                                            {
                                                dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.White;
                                                renkDegis = true;
                                            }
                                            else
                                            {
                                                dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.LightGray;
                                                renkDegis = false;
                                            }

                                        }
                                    }
                                    // Bilgileri Güncelleme !
                                    //sekro = 1;  // teneffüste sekro yok !
                                    /*while (drUrl.Read())
                                    {
                                        slayt = Convert.ToString(drUrl["RadioDers"].ToString());
                                        tv = Convert.ToString(drUrl["RadioTenefus"].ToString());

                                    }
                                    cekme.Dispose();
                                    drUrl.Close();
                                    baglanti.Close();

                                    if (slayt == "Slayt" && tv == "TV") // slayt == tv || 
                                    {
                                        slaytVeTv = true;
                                        tvVeSlayt = false;
                                    }
                                    else if (slayt == "TV" && tv == "Slayt") // slayt == TV VE tv == Slayt  ||
                                    {
                                        tvVeSlayt = true;
                                        slaytVeTv = false;
                                    }*/

                                    /*
                                     slayt = ders
                                     * tv = teneffüs
                                     
                                     */
                                    //TENEFFÜS
                                    if (slayt == "Slayt") // slayt = derste iken slayt demek tenefüsteyken TV demek
                                    {
                                        dersOk = false; // slayt kapatıldı.
                                        tenOk = true; // tv acıldı 
                                    }
                                    else if (slayt == "TV") // slayt == TV dersteyken TV tenefüsteyken Slayt
                                    {
                                        tenOk = false; // tv kapatıldı
                                        dersOk = true; // slayt açıldı
                                    }
                                    otomatikSaatDersVeTeneffusDegisim();
                                    
                                }
                            
                            }
                            else
                            {
                                if(saatlerName.Count <= dersBittiMi)
                                {
                                    lblDers.Text = "Ders Bitti.";
                                    lblTenefus.Text = "İyi Akşamlar";
                                    lblDers.Location = new Point((panel4.Size.Width / 2) - (lblDers.Size.Width / 2), 5);
                                    lblTenefus.Location = new Point((panel4.Size.Width / 2) - (lblTenefus.Size.Width / 2), 39);
                                    bool renkDegis = false;
                                    for (int c = 0; c < dataGridView2.Rows.Count; c++)
                                    {
                                        if (!renkDegis)
                                        {
                                            dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.White;
                                            renkDegis = true;
                                        }
                                        else
                                        {
                                            dataGridView2.Rows[c].DefaultCellStyle.BackColor = Color.LightGray;
                                            renkDegis = false;
                                        }
                                    }
                                }
                                else
                                {
                                    dersBittiMi++;
                                }
                            }
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("Saat eklemede hata oluştu \n\n" + exx.ToString(), "LCD Pano", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        dataGridView2.Rows.Clear();
                        dataGridView2.ColumnCount = 1;
                        dataGridView2.Columns[0].Name = "Colums1";
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        string[] row = new string[] { "Saat eklemede hata oluştu. Lütfen Güncelleyin." };
                        dataGridView2.Rows.Add(row);
                        dataGridView2.Rows[0].DefaultCellStyle.BackColor = Color.Red; // arka plan rengi
                        dataGridView2.Columns[0].DefaultCellStyle.ForeColor = Color.White; // yazı rengi
                    }
            
            }
            
        }

        private void dataGridView1_SelectionChanged(Object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell.Selected = false;
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
            dataGridView2.CurrentCell.Selected = false;
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow selectedRow = dataGridView1.CurrentRow;
                selectedRow.Selected = false;

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell.Selected = false;

                dataGridView2.ClearSelection();
                dataGridView2.CurrentCell.Selected = false;

                dataGridView3.ClearSelection();
                dataGridView3.CurrentCell.Selected = false;
            }
            catch
            { }
        }
        /*
        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView3.ClearSelection();
            dataGridView3.CurrentCell.Selected = false;
        }*/



        public const int sabitForm = 1024; // 1262
        public const int sabitFormH = 768; // 793

        public const int sabitPlayer = 320; // 320; 527
        public const int sabitPlayerH = 527;

        public const int sabitPic6 = 640;
        public const int sabitPic7 = 1009;
        public const int sabitPanel7 = 551;
        public const int sabitPic1 = 1075;


        public int dgw2 = 24;


        private void Form1_SizeChanged(object sender, EventArgs e)
        {

            int getForm = this.Size.Width;
            int getFormH = this.Size.Height;
            int formSize = getForm - sabitForm; // artan genişlik
            int formSizeH = getFormH - sabitFormH; // artan yükseklik

            int gns = sabitPlayer + formSize; // ^^ + player genilik
            int yks = sabitPlayerH + formSizeH; // ^^ + player yükselkil
            int titleWidth = sabitPanel7 + formSize;

            MediaPlayer.Size = new Size(gns, yks); // height ayarlanacak
            picSlayt.Size = new Size(gns, yks);
            webBrowser1.Size = new Size (gns, yks);

            int picture6Loc = 15 + 289 + 15 + gns;
            pictureBox6.Location = new Point(picture6Loc, pictureBox6.Location.Y);

            int picture7Loc = 15 + 289 + 15 + gns + 15 + 355;
            pictureBox7.Location = new Point(picture7Loc, pictureBox7.Location.Y);

            int dersLocation = 15 + 289 + 15 + gns + 15;
            dataGridView2.Location = new Point(dersLocation, dataGridView2.Location.Y);
            panel4.Location = new Point(dersLocation, panel4.Location.Y);

            panel7.Size = new Size(titleWidth, panel7.Size.Height);
            panel1.Location = new Point(titleWidth, panel1.Location.Y);

            pictureBox2.Size = new Size(getForm, pictureBox2.Size.Height);

            pictureBox3.Size = new Size(getForm, pictureBox3.Size.Height);
            int picture3LocH = 74 + 15 + yks;
            pictureBox3.Location = new Point(pictureBox3.Location.X, picture3LocH);

            pictureBox8.Size = new Size(getForm, pictureBox8.Size.Height);
            //int picture8LocH = 74 + 15 + yks + 15 + 112;
            pictureBox8.Location = new Point(pictureBox8.Location.X, this.Size.Height - 15);

            pictureBox4.Size = new Size(pictureBox4.Size.Width, getFormH - 89);
            pictureBox7.Size = new Size(pictureBox7.Size.Width, getFormH - 89);

            pictureBox5.Size = new Size(pictureBox5.Size.Width, 527 + formSizeH);
            pictureBox6.Size = new Size(pictureBox6.Size.Width, 527 + formSizeH);

            
            // Saatler
            dataGridView2.Size = new Size(dataGridView2.Size.Width, 458 + formSizeH);

            try
            {
                dgw2 = (formSizeH + 458) / olusanRows;
            }
            catch { }// saatle

            dataGridView1.Size = new Size(dataGridView1.Size.Width, 235 + formSizeH);

            int picBox1 = formSize + sabitPic1;
            lblKayanYazi.Location = new Point(pictureBox7.Location.X + lblKayanYazi.Size.Width, 639 + formSizeH);
            panel5.Location = new Point(dataGridView1.Location.X, dataGridView1.Location.Y + dataGridView1.Size.Height - 1);
            lblNobetciOgrenci.Location = new Point(panel5.Location.X + 13, panel5.Location.Y + panel5.Size.Height + 15);
            lblNobetciOgrenci.Location = new Point(((pictureBox5.Location.X + pictureBox4.Location.X + 15) / 2) - (lblNobetciOgrenci.Size.Width / 2), lblNobetciOgrenci.Location.Y);
            panel2.Location = new Point(panel5.Location.X, lblNobetciOgrenci.Location.Y + 35);
            dataGridView3.Location = new Point(panel2.Location.X, panel2.Location.Y + panel2.Size.Height);
            dataGridView3.Size = new Size(dataGridView3.Size.Width, pictureBox3.Location.Y - (panel2.Location.Y + panel2.Size.Height));

            

        }

        private void zmnAsimiDuzenleyici_Tick(object sender, EventArgs e)
        {
            guncellemeKontrol = 1;
            guncellemeRaporluOgretmen = 1;
            sekroSaatler = -1;
            sekro = -1; // Mekan sınıf öğretmem            
            sekroNobetciOgrenci = -1;
            sekroNobetciOgretmen = -1;
            zmnAsimiDuzenleyici.Stop();
        }

        
    }
}
