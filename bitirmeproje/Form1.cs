using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using NAudio.Wave;
using NAudio;

namespace bitirmeproje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int[] sayiuret(int uzunluk)
        {
            int[] dizi = new int[uzunluk];
            Random rndm = new Random();
            string key = "";
            for (int i = 0; i < uzunluk; i++)
            {
                dizi[i] = rndm.Next(1, 9);
            }
            return dizi;

        }
        private void refresh_btn_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            dosyayolu.Text = "";
            textBox2.Text = "";
            dosyayolu.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Gray;
            pictureBox2.BackColor = Color.Gray;
            textBox4.Visible = false;
            label8.Visible = false;
            textBox3.Visible = false;
            label5.Visible = false;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.png,*.jpg) | *.png;*.jpg";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dosyayolu.Text = dialog.FileName.ToString();
                Bitmap image = new Bitmap(dosyayolu.Text);
                image = resizeImage(image, new Size(387, 244));
                pictureBox1.Image = (Image)image;
                dosyayolu.Enabled = false;
            }
        }
        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return (new Bitmap(imgToResize, size));
        }
  


        private void button2_Click(object sender, EventArgs e)//şifrele butonu
        {

            int[] sifre = sayiuret(textBox2.TextLength);
            try
            {
                int seviye = comboBox1.SelectedIndex;
                Bitmap image = new Bitmap(dosyayolu.Text);
                image = resizeImage(image, new Size(387, 244));
                pictureBox1.Image = (Image)image;

                switch (seviye)
                {

                    case 1: //sezar + sol kolon algoritmam
                        for (int i = 0; i < image.Width; i++)
                        {

                            for (int j = 0; j < image.Height; j++)
                            {

                                Color piksel = image.GetPixel(i, j);
                                if (i < 1 && j < textBox2.TextLength)
                                {

                                    char mesaj = Convert.ToChar(textBox2.Text.Substring(j, 1));
                                    int AsciDeger = Convert.ToInt32(mesaj) + sifre[j];
                                    image.SetPixel(i, j, Color.FromArgb(piksel.R, piksel.G, AsciDeger));


                                }
                                if (i == image.Width - 1 && j == image.Height - 1)
                                {
                                    image.SetPixel(i, j, Color.FromArgb(piksel.R, piksel.G, textBox2.TextLength));
                                }
                            }

                        }

                        for (int j = 0; j < textBox2.TextLength; j++)
                        {
                            Color piksel = image.GetPixel(image.Width - 1, j);
                            image.SetPixel(image.Width - 1, j, Color.FromArgb(piksel.R, piksel.G, sifre[j]));
                        }
                        break;

                    case 2:
                        Random random = new Random();
                        for (int i = 0; i < textBox2.TextLength; i++)
                        {
                            int satir = random.Next(1, 256);
                            int sutun = random.Next(1, 256);
                            Color piksel = image.GetPixel(satir, sutun);
                            char mesaj = Convert.ToChar(textBox2.Text.Substring(i, 1));
                            int AsciDeger = Convert.ToInt32(mesaj);
                            image.SetPixel(satir, sutun, Color.FromArgb(piksel.R,AsciDeger, piksel.B));

                            textBox3.Text += Convert.ToChar(satir);
                            textBox3.Text += Convert.ToChar(sutun);
                        }
                        MessageBox.Show("Fotoğrafı Kaydettikten sonra Anahtar Değerini mutlaka bir yere Kaydediniz");
                        break;

                    default:
                        MessageBox.Show("Lütfen şifreleme Seviyesi giriniz");
                        break;
                }

                if(comboBox1.SelectedIndex != 0)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Image Files(*.png,*.jpg) | *.png;*.jpg";

                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        dosyayolu.Text = saveFile.FileName.ToString();
                        pictureBox1.ImageLocation = dosyayolu.Text;

                        image.Save(dosyayolu.Text);
                    }

                    textBox3.Visible = true;
                    label5.Visible = true;
                }
            

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) //şifre_çöz butonu
        {
           
        }

        public byte[] getBytes(string text)
        {

            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return bytes;
        }
        public void button4_Click(object sender, EventArgs e)//audio şifrele butonu
        {
            if (textBox1.Text != "")
            {


                try
                {
                    int subChunk1Size = 16;
                    short audioFormat = 1;
                    short bitsPerSample = 16; // her sample 2 bayt
                    short numChannels = 2;
                    int sampleRate = 22050;
                    int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
                    int numSamples = 19000;
                    short blockAlign = (short)(numChannels * (bitsPerSample / 8));
                    int subChunk2Size = numSamples * numChannels * (bitsPerSample / 8);
                    int chunkSize = 4 + (8 + subChunk1Size) + (8 + subChunk2Size);

                    File.Delete("test.wav");
                    FileStream f = new FileStream("test.wav", FileMode.Create);
                    BinaryWriter wr = new BinaryWriter(f);
                    wr.Write(getBytes("RIFF"));
                    wr.Write(chunkSize);
                    wr.Write(getBytes("WAVE"));
                    wr.Write(getBytes("fmt"));
                    wr.Write((byte)32);
                    wr.Write(subChunk1Size);
                    wr.Write(audioFormat);
                    wr.Write(numChannels);
                    wr.Write(sampleRate);
                    wr.Write(byteRate);
                    wr.Write(blockAlign);
                    wr.Write(bitsPerSample);
                    wr.Write(getBytes("data"));
                    wr.Write(subChunk2Size);
                    Random RandomByte = new Random();
                    byte[] rbs = { 0, 0, 0, 0 };
                    wr.Write(rbs);
                    textBox1.Text += "/";
                    for (int i = 0; i < numSamples; i++)
                    {
                        wr.Write(getBytes(textBox1.Text));
                    }

                    for (int i = 0; i < numSamples; i++)
                    {
                        wr.Write((byte)RandomByte.Next(255));
                        wr.Write((byte)RandomByte.Next(255));
                    }
                    MessageBox.Show("Şifrelenmiş Halde Ses Doyanız Oluşturuldu");
                    wr.Close();
                    wr.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Mesaj Yeri Boş Geçilemez");
            }
        }



        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                SoundPlayer sp = new SoundPlayer("test.wav");
                sp.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        struct WavHeader
        {
            public byte[] riffID;
            public uint size;  
            public byte[] wavID;  
            public byte[] fmtID;  
            public uint fmtSize; 
            public ushort format; 
            public ushort channels;
            public uint sampleRate; 
            public uint bytePerSec;
            public ushort blockSize; 
            public ushort bit;  
            public byte[] dataID; 
            public uint dataSize;
            public ulong data;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            label3.Text = "";
            string dosya_yolu = string.Empty;
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                dosya_yolu = fd.FileName.ToString();
            }
            WavHeader Header = new WavHeader();
            List<short> lDataList = new List<short>();
            List<short> rDataList = new List<short>();
            char[] abs = new char[400];
            string mesaj = "";
            using (FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {

                    Header.riffID = br.ReadBytes(4);
                    Header.size = br.ReadUInt32();
                    Header.wavID = br.ReadBytes(4);
                    Header.fmtID = br.ReadBytes(4);
                    Header.fmtSize = br.ReadUInt32();
                    Header.format = br.ReadUInt16();
                    Header.channels = br.ReadUInt16();
                    Header.sampleRate = br.ReadUInt32();
                    Header.bytePerSec = br.ReadUInt32();
                    Header.blockSize = br.ReadUInt16();
                    Header.bit = br.ReadUInt16();
                    Header.dataID = br.ReadBytes(4);
                    Header.dataSize = br.ReadUInt32();


                    for (int i = 0; i < 400; i++)
                    {

                        char character = (char)(br.ReadByte());
                        if (character == '/')
                        {
                            break;
                        }
                        abs[i] = character;

                    }
                    for (int i = 0; i < 400; i++)
                    {
                        listBox1.Items.Add(abs[i]);
                        label3.Text += abs[i];
                    }
                }
                finally
                {
                    listBox1.Refresh();
                    if (br != null)
                    {
                        br.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

            }
        
    
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.png,*.jpg) | *.png;*.jpg";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dosyayolu.Text = dialog.FileName.ToString();
                Bitmap image = new Bitmap(dosyayolu.Text);
                image = resizeImage(image, new Size(387, 244));
                pictureBox2.Image = (Image)image;
                dosyayolu.Enabled = false;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            label7.Text = "";
            switch (comboBox2.SelectedIndex)
            {
                case 1:
                    try
                    {
                        Bitmap image = new Bitmap(dosyayolu.Text);
                        string anamesaj = "";
                        Color sonpiksel = image.GetPixel(image.Width - 1, image.Height - 1);
                        int mesajuzunlugu = sonpiksel.B;
                        int[] sifre = new int[mesajuzunlugu];
                        for (int j = 0; j < mesajuzunlugu; j++)
                        {
                            Color piksel = image.GetPixel(image.Width - 1, j);
                            sifre[j] = piksel.B;
                        }
                        for (int i = 0; i < image.Width; i++)
                        {
                            for (int j = 0; j < image.Height; j++)
                            {
                                Color piksel = image.GetPixel(i, j);
                                if (i < 1 && j < mesajuzunlugu)
                                {
                                    int deger = piksel.B;
                                    char c = Convert.ToChar(deger - sifre[j]);
                                    string harf = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });
                                    anamesaj = anamesaj + harf;
                                }
                            }
                        }


                        label7.Text = anamesaj;


                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                    break;
                case 2:
                    if(textBox4.Text != "")
                    {
                        try
                        {
                            Bitmap image = new Bitmap(dosyayolu.Text);
                            string anamesaj = "";
                            for (int i = 0; i < textBox4.TextLength; i+=2)
                            {
                                char satir = Convert.ToChar(textBox4.Text.Substring(i, 1));
                                char sutun = Convert.ToChar(textBox4.Text.Substring(i+1,1));
                                Color piksel = image.GetPixel(Convert.ToInt32(satir),Convert.ToInt32(sutun));
                                char c = Convert.ToChar(piksel.G);
                                string harf = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });
                                label7.Text += harf;

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        
                        }
                      

                    }
                    else
                    {
                        MessageBox.Show("Anahtar Değerini giriniz");
                    }

                    break;
                default:
                    MessageBox.Show("Şifrelenme Seviyesini Seçiniz Bilmiyorsanız resmi gönderen kişi ile temasa geçiniz");
                    break;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex == 2)
            {
                textBox4.Visible = true;
                label8.Visible = true;
            }
            else
            {
                textBox4.Visible = false;
                label8.Visible = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
   
                textBox3.Visible = false;
                label5.Visible = false;
        }
    }
}

