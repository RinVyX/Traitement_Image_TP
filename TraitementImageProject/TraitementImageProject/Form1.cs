using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraitementImageProject
{
    public partial class Form1 : Form
    {
        public string F
        {
            get { return "HHH"; }
        }

        public static Form1 Instance
        {
            get
            {
                if (Instance == null)
                    Instance = new Form1();
                return Instance;
            }
            set
            {
                
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Charger_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            Bitmap bmap = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = bmap;

            Bitmap bmapGris = convertirEnNiveauDeGris(bmap);
            pictureBox2.Image = bmapGris;
            calculHistogramme(bmapGris);
        }

        private Bitmap convertirEnNiveauDeGris(Bitmap imageBitmap)
        {
            Color c;
            var imageData = new int[imageBitmap.Width, imageBitmap.Height];
            Bitmap bitmapToreturn = new Bitmap(imageBitmap.Width, imageBitmap.Height);

            int minNG = 255, maxNG = 0;

            for(int i = 0; i < imageBitmap.Width; i++)
            {
                for(int j = 0; j < imageBitmap.Height; j++)
                {
                    c = imageBitmap.GetPixel(i, j);
                    byte gray = (byte)(Math.Min(.299 * c.R + .587 * c.G + .114 * c.B, 255));

                    bitmapToreturn.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                    imageData[i, j] = gray;

                    if (gray < minNG) minNG = gray;
                    if (gray > maxNG) maxNG = gray;

                }
            }

            return bitmapToreturn;
        }

        private void calculHistogramme(Bitmap imageBitmapEnGris)
        {
            Color c;
            int[] histogramme = new int[256];

            for(int i = 0; i < 256; i++)
            {
                histogramme[i] = 0;
            }

            for (int w = 0; w < imageBitmapEnGris.Width; w++)
            {
                for (int h = 0; h < imageBitmapEnGris.Height; h++)
                {
                    c = imageBitmapEnGris.GetPixel(w, h);
                    histogramme[(byte)c.R]++;
                }
            }

            foreach(int g in histogramme)
            {
                textBox_DisplayValues.Text += '-' + g;
            }
            //return histogramme;
        }
    }
}
