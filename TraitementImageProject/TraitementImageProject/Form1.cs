using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraitementImageProject
{
    public partial class Form1 : Form
    {

        Dictionary<int, int> _histogramme;

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
            _histogramme = calculHistogramme(bmapGris);

        }

        private Bitmap convertirEnNiveauDeGris(Bitmap imageBitmap)
        {
            Color c;
            var imageData = new int[imageBitmap.Width, imageBitmap.Height];
            Bitmap bitmapToreturn = new Bitmap(imageBitmap.Width, imageBitmap.Height);

            int minNG = 255, maxNG = 0;

            for (int i = 0; i < imageBitmap.Width; i++)
            {
                for (int j = 0; j < imageBitmap.Height; j++)
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

        private Dictionary<int, int> calculHistogramme(Bitmap imageBitmapEnGris)
        {
            Color c;
            Dictionary<int, int> histogramme = new Dictionary<int, int>();
            
            for (int w = 0; w < imageBitmapEnGris.Width; w++)
            {
                for (int h = 0; h < imageBitmapEnGris.Height; h++)
                {
                    c = imageBitmapEnGris.GetPixel(w, h);
                    if (histogramme.ContainsKey((byte)c.R))
                        histogramme[(byte)c.R]++;
                    else
                        histogramme.Add((byte)c.R, 1);
                }
            }
            
            return histogramme;
        }
        
        private void DrawHistogram(Graphics gr, Color back_color, int[] values, int width, int height)
        {
            int max_value = 100;

            Color[] Colors = new Color[] {
                Color.Red, Color.LightGreen, Color.Blue,
                Color.Pink, Color.Green, Color.LightBlue,
                Color.Orange, Color.Yellow, Color.Purple
            };

            gr.Clear(back_color);
            
            // Make a transformation to the PictureBox.
            RectangleF data_bounds = new RectangleF(0, 0, values.Length, max_value);
            PointF[] points = {
                new PointF(0, height),
                new PointF(width, height),
                new PointF(0, 0)
            };
                    Matrix transformation = new Matrix(data_bounds, points);
                    gr.Transform = transformation;

                    // Draw the histogram.
                    using (Pen thin_pen = new Pen(Color.Black, 0))
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            RectangleF rect = new RectangleF(i, 0, 1, values[i]);
                            using (Brush the_brush =
                                new SolidBrush(Colors[i % Colors.Length]))
                            {
                                gr.FillRectangle(the_brush, rect);
                                gr.DrawRectangle(thin_pen, rect.X, rect.Y,
                                    rect.Width, rect.Height);
                            }
                   
                        }
                    }

                    gr.ResetTransform();
                    gr.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1);
            
        }

        private void button_ShowHisto_Click(object sender, EventArgs e)
        {
            pictureBox3.Refresh();
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            if (_histogramme == null)
                return;


            int[] hist = new int[_histogramme.Values.Count]; // Exemple à essayer { 5, 8, 1, 9, 75, 0, 5 };

            _histogramme.Values.CopyTo(hist, 0); // Effacer ceci pour essayer l'exemple

            DrawHistogram(e.Graphics, pictureBox3.BackColor, hist,
         pictureBox3.ClientSize.Width, pictureBox3.ClientSize.Height);
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (_histogramme == null)
                return;

            // Determine which data value was clicked.
            float bar_wid = pictureBox3.ClientSize.Width /
                (int)_histogramme.Count;
            int i = (int)(e.X / bar_wid);
            try
            {
                MessageBox.Show("Item " + i + " has value " + _histogramme[i],
                  "Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                //A négliger
            }
        }
    }
}
