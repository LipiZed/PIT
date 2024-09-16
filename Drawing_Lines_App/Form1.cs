using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Drawing_Lines_App
{
    public partial class Form1 : Form
    {
        private Bitmap drawingBitmap;
        private bool isDrawing;
        private List<Point> drawnPoints = new List<Point>();
        private Color fillColor = Color.Red; // Цвет для заливки
        private Bitmap fillImage; // Изображение для заливки
        private bool useImageFill = false; // Флаг для заливки изображением

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(MainForm_Paint);
            this.MouseDown += new MouseEventHandler(MainForm_MouseDown);
            this.MouseMove += new MouseEventHandler(MainForm_MouseMove);
            this.MouseUp += new MouseEventHandler(MainForm_MouseUp);

            // Создаем холст
            drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(drawingBitmap, 0, 0);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                drawnPoints.Clear();
                drawnPoints.Add(e.Location);
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.DrawLine(Pens.Black, drawnPoints.Last(), e.Location);
                }
                drawnPoints.Add(e.Location);
                this.Invalidate();
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;

                if (drawnPoints.Count > 2)
                {
                    if (useImageFill && fillImage != null)
                    {
                        FillDrawnAreaWithImage();
                    }
                    else
                    {
                        FillDrawnAreaWithColor();
                    }
                    this.Invalidate(); // Обновляем экран
                }
            }
        }

        private void FillDrawnAreaWithImage()
        {
            PointF[] pointsArray = drawnPoints.Select(p => new PointF(p.X, p.Y)).ToArray();
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(pointsArray);
                    g.SetClip(path);

                    RectangleF boundingRect = path.GetBounds();

                    // Scale the image to fit the bounding rectangle
                    using (Bitmap scaledImage = new Bitmap(fillImage, (int)boundingRect.Width, (int)boundingRect.Height))
                    {
                        g.DrawImage(scaledImage, boundingRect);
                    }
                }
            }
        }

        private void FillDrawnAreaWithColor()
        {
            PointF[] pointsArray = drawnPoints.Select(p => new PointF(p.X, p.Y)).ToArray();
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(pointsArray);
                    g.SetClip(path);
                    g.Clear(fillColor); // Заливаем замкнутую область выбранным цветом
                }
            }
        }

        private void ChooseImageForFill()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fillImage = new Bitmap(openFileDialog.FileName);
                useImageFill = true;
            }
        }

        private void ChooseColorForFill()
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color;
                useImageFill = false;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Drawing Lines App";
            this.ClientSize = new System.Drawing.Size(800, 600);

            // Кнопка для выбора цвета заливки
            Button colorFillButton = new Button();
            colorFillButton.Text = "Выбрать цвет заливки";
            colorFillButton.Location = new Point(10, 10);
            colorFillButton.Click += (s, e) => ChooseColorForFill();
            this.Controls.Add(colorFillButton);

            // Кнопка для выбора изображения заливки
            Button imageFillButton = new Button();
            imageFillButton.Text = "Выбрать изображение для заливки";
            imageFillButton.Location = new Point(10, 50);
            imageFillButton.Click += (s, e) => ChooseImageForFill();
            this.Controls.Add(imageFillButton);
        }
    }
}
