using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3_task1
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private bool     leftButtonDown;
        private Pen      pen;
        private Pen      fillPen;
        private Point    prevCords;
        private Bitmap   originalImage;
        private Color    chosenColor;

        private void drawPoint(int x, int y)
        {
            g.DrawEllipse(pen, x, y, 1, 1);
            pictureBox1.Invalidate();
        }

        private void drawPoint(Point p)
        {
            g.DrawEllipse(pen, p.X, p.Y, 1, 1);
            pictureBox1.Invalidate();
        }

        private void drawLineTo(Point p)
        {
            g.DrawLine(pen, prevCords, p);
            prevCords = p;
            pictureBox1.Invalidate();
        }

        private Point getLeftBound(Point p)
        {
            int y = p.Y;
            int x = p.X;
            while (x > 0 && chosenColor == originalImage.GetPixel(x - 1, y))
                --x;
            return new Point(x, y);
        }

        private Point getRightBound(Point p)
        {
            int y = p.Y;
            int x = p.X;
            int width = originalImage.Width;
            while (x < width - 1 && chosenColor == originalImage.GetPixel(x + 1, y))
                ++x;
            return new Point(x, y);
        }

        private void floodFillByPicture(Point p)
        {
            if (originalImage.GetPixel(p.X, p.Y) != chosenColor) return;

            Point leftBound = getLeftBound(p);
            Point rightBound = getRightBound(p);
            g.DrawLine(fillPen, leftBound, rightBound);
            pictureBox1.Invalidate();

            int leftX = leftBound.X;
            int rightX = rightBound.X;

            if (p.Y < originalImage.Height - 1)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByPicture(new Point(i, p.Y + 1));
                }
            }

            if (p.Y > 0)
            {
                for (int i = leftX; i <= rightX; ++i)
                {
                    floodFillByPicture(new Point(i, p.Y - 1));
                }
            }
            pictureBox1.Invalidate();
        }

        public Form1()
        {
            InitializeComponent();
            originalImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = originalImage;
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);

            leftButtonDown = false;
            pen = new Pen(Color.SlateBlue);
            fillPen = new Pen(Color.Pink);
            //pen.Dispose();
        }

        /*private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("image1.png");
        }*/

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftButtonDown)
            {
                drawLineTo(e.Location);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftButtonDown = true;
                prevCords = e.Location;
                drawPoint(prevCords);
            }
            else if (e.Button == MouseButtons.Right)
            {
                chosenColor = originalImage.GetPixel(e.X, e.Y);
                floodFillByPicture(e.Location);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftButtonDown = false;
            }
        }
    }
}
