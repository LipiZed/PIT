using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Drawing_Lines_App
{
    public partial class Form1 : Form
    {
        private Bitmap drawingBitmap;
        private bool isDrawing;
        private Point previousPoint;
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
            this.DoubleClick += new EventHandler(MainForm_DoubleClick);

            // Создаем пустой холст для рисования
            drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Рисуем текущее изображение
            e.Graphics.DrawImage(drawingBitmap, 0, 0);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                previousPoint = e.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Рисуем линию на холсте
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.DrawLine(Pens.Black, previousPoint, e.Location);
                }
                previousPoint = e.Location;
                this.Invalidate(); // Перерисовываем форму
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;
            }
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            // Включаем заливку на двойной клик
            Point clickPoint = this.PointToClient(Cursor.Position);

            if (useImageFill && fillImage != null)
            {
                FloodFillWithImage(clickPoint, drawingBitmap.GetPixel(clickPoint.X, clickPoint.Y));
            }
            else
            {
                FloodFill(clickPoint, fillColor, drawingBitmap.GetPixel(clickPoint.X, clickPoint.Y));
            }

            this.Invalidate(); // Обновляем экран
        }

        private void FloodFill(Point point, Color fillColor, Color targetColor)
        {
            if (targetColor == fillColor) return; // Предотвращаем зацикливание

            Stack<Point> pixelsToFill = new Stack<Point>();
            pixelsToFill.Push(point);

            while (pixelsToFill.Count > 0)
            {
                Point currentPoint = pixelsToFill.Pop();

                if (currentPoint.X < 0 || currentPoint.X >= drawingBitmap.Width || currentPoint.Y < 0 || currentPoint.Y >= drawingBitmap.Height)
                {
                    continue;
                }

                if (drawingBitmap.GetPixel(currentPoint.X, currentPoint.Y) != targetColor)
                {
                    continue;
                }

                drawingBitmap.SetPixel(currentPoint.X, currentPoint.Y, fillColor);

                pixelsToFill.Push(new Point(currentPoint.X + 1, currentPoint.Y));
                pixelsToFill.Push(new Point(currentPoint.X - 1, currentPoint.Y));
                pixelsToFill.Push(new Point(currentPoint.X, currentPoint.Y + 1));
                pixelsToFill.Push(new Point(currentPoint.X, currentPoint.Y - 1));
            }
        }

        private void FloodFillWithImage(Point point, Color targetColor)
        {
            if (fillImage == null || targetColor == Color.Transparent) return;

            Stack<Point> pixelsToFill = new Stack<Point>();
            pixelsToFill.Push(point);

            while (pixelsToFill.Count > 0)
            {
                Point currentPoint = pixelsToFill.Pop();

                if (currentPoint.X < 0 || currentPoint.X >= drawingBitmap.Width || currentPoint.Y < 0 || currentPoint.Y >= drawingBitmap.Height)
                {
                    continue;
                }

                if (drawingBitmap.GetPixel(currentPoint.X, currentPoint.Y) != targetColor)
                {
                    continue;
                }

                // Получаем соответствующий пиксель из изображения заливки
                Color imageColor = fillImage.GetPixel(currentPoint.X % fillImage.Width, currentPoint.Y % fillImage.Height);
                drawingBitmap.SetPixel(currentPoint.X, currentPoint.Y, imageColor);

                pixelsToFill.Push(new Point(currentPoint.X + 1, currentPoint.Y));
                pixelsToFill.Push(new Point(currentPoint.X - 1, currentPoint.Y));
                pixelsToFill.Push(new Point(currentPoint.X, currentPoint.Y + 1));
                pixelsToFill.Push(new Point(currentPoint.X, currentPoint.Y - 1));
            }
        }

        // Метод для выбора изображения пользователем
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

        // Метод для выбора цвета заливки
        private void ChooseColorForFill()
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color;
                useImageFill = false;
            }
        }

        // Добавим кнопки для выбора заливки цветом или изображением
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


