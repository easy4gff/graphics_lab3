using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace lab3_task1
{
    public partial class Form1 : Form
    {
        // Графический объект
        private Graphics g;
        // Индикатор нажатия левой кнопки мыши
        private bool     leftButtonDown;
        // Карандаш для рисования
        private Pen      pen;
        // Карандаш для заполнения
        private Pen      fillPen;
        // Предыдущие координаты (используется в рисовании линиями)
        private Point    prevCords;
        // Битмап, в котором рисуем и который заполняем
        private Bitmap   mainBitMap;
        // Изображение, которым заполняем
        private Bitmap   fillImage;

        // Изображение, которым заполняем (масштабированное)
        private Bitmap   fillImageScaled;
        // Кооринаты левого угла масштабированного изображения для заполнения
        private Point    scaledImageLocation;
        // Границы масштабированного изображения
        private int      leftBoundScaled;
        private int      topBoundScaled;
        private int      rightBoundScaled;
        private int      bottomBoundScaled;
        // Кэш уже проверенных координат
        private bool[][] сheckedCords;

        // Цвет фона (для заливки)
        private Color    chosenColor;
        // Кэш уже установленных пикселей
        private bool[][] filledPixels;

        // Очищает кэш залитых пикселей
        private void clearFilledPixels()
        {
            filledPixels = new bool[pictureBox1.Width][];
            for (int i = 0; i < filledPixels.Length; ++i)
                filledPixels[i] = new bool[pictureBox1.Height];
        }

        // Очищает кэш проверенных координат
        private void clearСheckedCords()
        {
            сheckedCords = new bool[pictureBox1.Width][];
            for (int i = 0; i < сheckedCords.Length; ++i)
                сheckedCords[i] = new bool[pictureBox1.Height];
        }

        // Рисует точку заданным цветом
        private void drawPoint(int x, int y)
        {
            g.DrawEllipse(pen, x, y, 1, 1);
            pictureBox1.Invalidate();
        }

        // Рисует точку заданным цветом
        private void drawPoint(Point p)
        {
            g.DrawEllipse(pen, p.X, p.Y, 1, 1);
            pictureBox1.Invalidate();
        }

        // Рисует линию заданным цветом
        private void drawLineTo(Point p)
        {
            g.DrawLine(pen, prevCords, p);
            prevCords = p;
            pictureBox1.Invalidate();
        }

        // Лисует линию из изображения
        private void drawLineFromImageCommon(Point p1, Point p2)
        {
            int y = p1.Y;
            for (int i = p1.X; i <= p2.X; ++i)
            {
                mainBitMap.SetPixel(i, y, fillImage.GetPixel(i, y));
                filledPixels[i][y] = true;
            }
        }

        // Лисует линию из изображения (масштабированного)
        private void drawLineFromImageScaled(Point p1, Point p2)
        {
            int y = p1.Y;
            for (int i = p1.X; i <= p2.X; ++i)
            {
                mainBitMap.SetPixel(i, y, getPixelFromScaledImage(i, y));
                filledPixels[i][y] = true;
            }
        }

        // Возвращает левую границу для пиклеся
        private Point getLeftBound(Point p)
        {
            int y = p.Y;
            int x = p.X;
            //while (x > 0 && chosenColor == originalImage.GetPixel(x - 1, y))
            while (x > 0 && colorsAreEqual(chosenColor, mainBitMap.GetPixel(x - 1, y)))
                --x;
            return new Point(x, y);
        }

        // Возвращает правую границу для пикселя
        private Point getRightBound(Point p)
        {
            int y = p.Y;
            int x = p.X;
            int width = mainBitMap.Width;
            //while (x < width - 1 && chosenColor == originalImage.GetPixel(x + 1, y))
            while (x < width - 1 && colorsAreEqual(chosenColor, mainBitMap.GetPixel(x + 1, y)))
                ++x;
            return new Point(x, y);
        }

        // Проверка цветов на равенство (какого-то черта, обычное сравнение работает некорректно)
        private bool colorsAreEqual(Color c1, Color c2) {
            return c1.A == c2.A && c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }
        
        // Заполнение области заданным цветом
        private void floodFillByLine(Point p)
        {
            Color c = mainBitMap.GetPixel(p.X, p.Y);

            if (c != chosenColor || colorsAreEqual(c, fillPen.Color)) return;

            Point leftBound = getLeftBound(p);
            Point rightBound = getRightBound(p);

            int leftX = leftBound.X;
            int rightX = rightBound.X;
            if (leftX != rightX)
                g.DrawLine(fillPen, leftBound, rightBound);
            else
                g.DrawEllipse(fillPen, p.X, p.Y, 1, 1);

            if (p.Y < mainBitMap.Height - 1)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByLine(new Point(i, p.Y + 1));
                }
            }

            if (p.Y > 0)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByLine(new Point(i, p.Y - 1));
                }
            }
            pictureBox1.Invalidate();
        }

        // Заполнение области заданным изображением
        private void floodFillByImageCommon(Point p)
        {
            Color c = mainBitMap.GetPixel(p.X, p.Y);

            if (c != chosenColor || filledPixels[p.X][p.Y]) return;

            Point leftBound = getLeftBound(p);
            Point rightBound = getRightBound(p);

            int leftX = leftBound.X;
            int rightX = rightBound.X;
            if (leftX != rightX)
                drawLineFromImageCommon(leftBound, rightBound);
            else
            {
                mainBitMap.SetPixel(p.X, p.Y, fillImage.GetPixel(p.X, p.Y));
                filledPixels[p.X][p.Y] = true;
            }

            if (p.Y < mainBitMap.Height - 1)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByImageCommon(new Point(i, p.Y + 1));
                }
            }

            if (p.Y > 0)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByImageCommon(new Point(i, p.Y - 1));
                }
            }
            pictureBox1.Invalidate();
        }

        // Вычисляет координаты масштабированного изображения по координатам основного битмапа
        private Color getPixelFromScaledImage(int x, int y)
        {
            int diffX = x - scaledImageLocation.X + 1;
            int diffY = y - scaledImageLocation.Y + 1;
            return fillImageScaled.GetPixel(diffX, diffY);
        }

        // Определяет максимальные границы для дальнейшего масштабирования изображения для заливки
        private void determineMinMaxBoundsOfScaledImage(Point p)
        {
            Color c = mainBitMap.GetPixel(p.X, p.Y);

            if (c != chosenColor || сheckedCords[p.X][p.Y]) return;

            Point leftBound = getLeftBound(p);
            Point rightBound = getRightBound(p);

            int leftX = leftBound.X;
            if (leftX < leftBoundScaled)
                leftBoundScaled = leftX;

            int rightX = rightBound.X;
            if (rightX > rightBoundScaled)
                rightBoundScaled = rightX;

            if (p.Y < bottomBoundScaled)
                bottomBoundScaled = p.Y;

            if (p.Y > topBoundScaled)
                topBoundScaled = p.Y;


            for (int i = leftX; i <= rightX; ++i)
                сheckedCords[i][p.Y] = true;

            if (p.Y < mainBitMap.Height - 1)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    determineMinMaxBoundsOfScaledImage(new Point(i, p.Y + 1));
                }
            }

            if (p.Y > 0)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    determineMinMaxBoundsOfScaledImage(new Point(i, p.Y - 1));
                }
            }
        }

        // Вычисляет координаты масштабированного изображения
        private Point determineScaledImageLocation()
        {
            return new Point(leftBoundScaled, bottomBoundScaled);
        }

        // Инициализирует данные для масштабируемого заполнения
        private void InitFillData(Point p)
        {
            clearСheckedCords();

            leftBoundScaled = fillImage.Width;
            topBoundScaled = 0;
            rightBoundScaled = 0;
            bottomBoundScaled = fillImage.Height;

            determineMinMaxBoundsOfScaledImage(p);
            scaledImageLocation = determineScaledImageLocation();
            fillImageScaled = new Bitmap(fillImage, rightBoundScaled - leftBoundScaled + 2,
                topBoundScaled - bottomBoundScaled + 2);
        }

        // Заполнение области заданным изображением (c масштабированием последнего)
        private void floodFillByImageScaled(Point p)
        {
            InitFillData(p);
            floodFillByImageScaled_Recursive(p);
            pictureBox1.Invalidate();
        }

        // Заполнение области заданным изображением (c масштабированием последнего)
        private void floodFillByImageScaled_Recursive(Point p)
        {
            Color c = mainBitMap.GetPixel(p.X, p.Y);

            if (c != chosenColor || filledPixels[p.X][p.Y]) return;

            Point leftBound = getLeftBound(p);
            Point rightBound = getRightBound(p);

            int leftX = leftBound.X;
            int rightX = rightBound.X;
            if (leftX != rightX)
                drawLineFromImageScaled(leftBound, rightBound);
            else
            {
                mainBitMap.SetPixel(p.X, p.Y, getPixelFromScaledImage(p.X, p.Y));
                filledPixels[p.X][p.Y] = true;
            }

            if (p.Y < mainBitMap.Height - 1)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByImageScaled_Recursive(new Point(i, p.Y + 1));
                }
            }

            if (p.Y > 0)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByImageScaled_Recursive(new Point(i, p.Y - 1));
                }
            }
        }

        // Инициализация выпадающего списка
        private void initComboBox()
        {
            comboBox1.Items.AddRange(new string[] {
               "Немасштабируемая заливка",
               "Масштабируемая заливка"
            });
            comboBox1.SelectedIndex = 0;
        }

        // Конструктор формы
        public Form1()
        {
            InitializeComponent();
            initComboBox();
            mainBitMap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = mainBitMap;
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);

            leftButtonDown = false;
            pen = new Pen(Color.SlateBlue);
            fillPen = new Pen(Color.Pink);

            clearFilledPixels();
        }

        // Рисование заданным цветом
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftButtonDown)
            {
                drawLineTo(e.Location);
            }
        }

        // Активация рисования или заливка (в зависимости от нажатой кнопки)
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftButtonDown = true;
                prevCords = e.Location;
                drawPoint(prevCords);
            }
            else if (e.Button == MouseButtons.Right && fillImage != null)
            {
                chosenColor = mainBitMap.GetPixel(e.X, e.Y);
                //floodFillByLine(e.Location);
                if (comboBox1.SelectedIndex == 0)
                {
                    floodFillByImageCommon(e.Location);
                }
                else {
                    floodFillByImageScaled(e.Location);
                }
            }
        }

        // Деактивация рисования
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftButtonDown = false;
            }
        }

        // Диалог открытия файла
        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            openFileDialog1 = new OpenFileDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            Size fillImageSize = new Size(pictureBox1.Width, pictureBox1.Height);
                            fillImage = new Bitmap(myStream);
                            pictureBox2.Image = new Bitmap(fillImage, pictureBox2.Width, pictureBox2.Height);
                            fillImage = new Bitmap(fillImage, fillImageSize);
                            button1.Enabled = true;
                            textBox1.Text = Path.GetFileName(openFileDialog1.FileName);
                            textBox1.BackColor = Color.Green;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        // Функция очищения битмапа
        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            clearFilledPixels();
            pictureBox1.Invalidate();
        }
    }
}
