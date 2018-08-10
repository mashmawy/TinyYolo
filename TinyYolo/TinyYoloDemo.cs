using AlbiruniML;
using alb = AlbiruniML.Ops;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TinyYolo
{
    public partial class TinyYoloDemo : Form
    {
        TinyYoloPredictor yoloTiny;
        public TinyYoloDemo()
        {
            InitializeComponent();
        }


        Tensor imageTensor;
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private Tensor LoadImage(OpenFileDialog ofd)
        {
            Image image = ResizeImage(Bitmap.FromFile(ofd.FileName), 416, 416);
            SrcPictureBox.Image = image;
            var x = alb.buffer(new int[] { 1, image.Height, image.Width, 3 });
            Bitmap bmp = new Bitmap(image);
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color clr = bmp.GetPixel(j, i);
                    float red = clr.R / 255.0f;
                    float green = clr.G / 255.0f;
                    float blue = clr.B / 255.0f;

                    x.Set(red, 0, i, j, 0);
                    x.Set(green, 0, i, j, 1);
                    x.Set(blue, 0, i, j, 2);
                }
            }
            yoloTiny = new TinyYoloPredictor();
            yoloTiny.ReportProgress += yoloTiny_ReportProgress;
            return x.toTensor();
        }
        void UpdateProgress(object value)
        {
            this.progressBar1.Value = Convert.ToInt32(value);
        }
        void yoloTiny_ReportProgress(int progress)
        {
            this.Invoke(new FormInvokWithParam(UpdateProgress), progress);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var resp = ofd.ShowDialog();
            if (resp == System.Windows.Forms.DialogResult.OK)
            {
                this.imageTensor = LoadImage(ofd);
                this.imageTensor.keep();
                DetectButton.Enabled = true;
            }
        }

        List<YoloBox> YoloBoxs = new List<YoloBox>();
        Thread t;
        private void DetectButton_Click(object sender, EventArgs e)
        {
            this.DetectButton.Enabled = false;
            this.label2.Visible = true;

            this.progressBar1.Visible = true;
            this.progressBar1.Value = 0;
            this.progressBar1.Maximum = 100;

            this.BrowseButton.Enabled = false;
            t = new Thread(new ThreadStart(Detect));
            t.Start();
             
        }
        delegate void FormInvok();
        delegate void FormInvokWithParam(object param);
        private void Detect()
        {
           

            alb.tidy(() =>
            {
                YoloBoxs = yoloTiny.Detect(this.imageTensor).OrderByDescending(p => p.classProb).ToList();//.FirstOrDefault();
            });
            this.Invoke(new FormInvok(Display));
           
        }

        private void Display( )
        {
            Bitmap src = new Bitmap(this.SrcPictureBox.Image);
            ResultList.Items.Clear();
            foreach (var item in YoloBoxs)
            {
                ResultList.Items.Add(item.className + "  " + item.classProb.ToString("0.00%"));
                int w = ((item.right - item.left));
                int h = (item.bottom - item.top);
                int x = (int)(item.left);
                int y = (int)(item.top);
                using (Graphics graphics = Graphics.FromImage(src))
                {

                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        var rect = new Rectangle(x, y, w, h);

                        graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(x, y, 50, 20));
                        graphics.DrawString(item.className + " " + item.classProb.ToString("0.00"),
                            SystemFonts.StatusFont, new SolidBrush(Color.Black),
                            new RectangleF(x, y, 50, 20)

                            );
                        graphics.DrawRectangle(pen, rect);

                    }




                }
            }

            this.SrcPictureBox.Image = src;
            this.SrcPictureBox.Refresh();
            this.label2.Visible = false;

            this.progressBar1.Visible = false;
            this.progressBar1.Value = 0;
            this.BrowseButton.Enabled = true;
        }

        private void TinyYoloDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t!=null)
            {
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
        }
    }
}
