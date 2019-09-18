using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace WindowsFormsApp5
{
    class MyRect
    {
        private Point[] point = new Point[2];
        private Rectangle rect;
        private int thick;
        private bool isSolid;
        private Color pen;
        private Color brush;
        private bool fill;
        private int n;
        public MyRect()
        {
            point[0] = new Point();
            point[1] = new Point();
            rect = new Rectangle();
            thick = 1;
            isSolid = true;
        }
        public void setRect(Point start, Point finish, int thick, bool isSolid, Color pen, Color brush, bool fill, int n)
        {
            this.point[0] = start;
            this.point[1] = finish;
            rect.X = Math.Min(start.X, finish.X);
            rect.Y = Math.Min(start.Y, finish.Y);
            rect.Width = Math.Abs(start.X - finish.X);
            rect.Height = Math.Abs(start.Y - finish.Y);
            this.thick = thick;
            this.isSolid = isSolid;
            this.pen = pen;
            this.brush = brush;
            this.fill = fill;
            this.n = n;
        }
        public Rectangle getRect()
        {
            return rect;
        }
        public int getThick()
        {
            return thick;
        }
        public bool getSolid()
        {
            return isSolid;
        }
        public Color getPen()
        {
            return pen;
        }
        public Color getBrush()
        {
            return brush;
        }
        public bool getfill()
        {
            return fill;
        }
        public int getn()
        {
            return n;
        }
        public Point getPoint1()
        {
            return point[0];
        }
        public Point getPoint2()
        {
            return point[1];
        }
    }
}
