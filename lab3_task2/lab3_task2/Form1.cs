using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3_task2
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        Graphics graphics;
        Boolean isMouseDown = false;
        Boolean callPickOnClick = false;
        Point startPoint;
        Point lastPoint;
        Pen pen;

        public Form1()
        {
            InitializeComponent();
            SetupPictureBox();
        }

        public void SetupPictureBox()
        {
            bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = bitmap;
            graphics = Graphics.FromImage(pictureBox.Image);
            graphics.Clear(Color.White);

            pen = new Pen(Color.Black, 1);
        }

        private void PictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (!callPickOnClick)
            {
                isMouseDown = true;
                lastPoint = e.Location;
                startPoint = e.Location;
            } else
            {
                startPoint = e.Location;
                TurnBorderToRed();
                pictureBox.Refresh();
            }

        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown && !callPickOnClick)
            {
                graphics.DrawLine(pen, lastPoint, e.Location);
                lastPoint = e.Location;
                pictureBox.Refresh();
            }
        }

        private void PictureBoxMouseUp (object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            lastPoint = Point.Empty;
            pictureBox.Cursor = Cursors.Cross;
            callPickOnClick = true;
        }

        private Point FindBorderPoint()
        {
            int x = startPoint.X;
            int y = startPoint.Y;
            Color background = bitmap.GetPixel(x, y);
            Color current = background;
            while (x < bitmap.Width - 1 && current.ToArgb() == background.ToArgb())
            {
                x++;
                current = bitmap.GetPixel(x, y);
            }

            return new Point(x, y);
        }

        private void TurnBorderToRed()
        {
            LinkedList<Point> points = new LinkedList<Point>();
            Point current = FindBorderPoint();
            Point start = current;
            Point next = new Point();

            points.AddLast(start);
            Color borderColor = bitmap.GetPixel(current.X, current.Y);

            int currentDirection = 6;
            int nextDirection = 0;

            while (next != start)
            {
                nextDirection = (currentDirection - 2 + 8) % 8;
                int mt = nextDirection;
                do
                {
                    next = current;
                    switch (nextDirection)
                    {
                        case 0:
                            next.X++;
                            break;

                        case 1:
                            next.X++;
                            next.Y--;
                            break;

                        case 2:
                            next.Y--;
                            break;

                        case 3:
                            next.X--;
                            next.Y--;
                            break;

                        case 4:
                            next.X--;
                            break;

                        case 5:
                            next.X--;
                            next.Y++;
                            break;

                        case 6:
                            next.Y++;
                            break;

                        case 7:
                            next.X++;
                            next.Y++;
                            break;
                    }
                    
                    if (next == start)
                        break;

                    if (bitmap.GetPixel(next.X, next.Y) == borderColor)
                    {
                        points.AddLast(next);
                        current = next;
                        currentDirection = nextDirection;
                        break;
                    }
                    nextDirection = (nextDirection + 1) % 8;
                } while (nextDirection != mt && next.X < bitmap.Width - 1 && next.Y < bitmap.Height - 1);
            }

            foreach (var p in points)
                bitmap.SetPixel(p.X, p.Y, Color.Red);
        }

        private void OnBtnClearClick(object sender, EventArgs e)
        {
            SetupPictureBox();
            callPickOnClick = false;
            isMouseDown = false;
            pictureBox.Cursor = Cursors.Default;
        }
    }
}
