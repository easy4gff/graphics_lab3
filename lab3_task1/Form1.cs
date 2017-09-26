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
        private bool leftButtonDown;
        private Pen pen;
        private Point prevCords;

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

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);

            leftButtonDown = false;
            pen = new Pen(Color.SlateBlue);
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
            leftButtonDown = true;
            prevCords = e.Location;
            drawPoint(prevCords);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            leftButtonDown = false;
        }
    }
}
