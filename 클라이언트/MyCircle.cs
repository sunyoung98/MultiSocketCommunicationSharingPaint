using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace WindowsFormsApp5
{
    class MyCircle
    {
        private Rectangle rectC;
        private Point[] point = new Point[2];
        private int thick;
        private bool isSolid;
        private Color pen;
        private Color brush;
        private bool fill;
        private int n;

        public MyCircle()
        {
            point[0] = new Point();
            point[1] = new Point();
            rectC = new Rectangle();
            thick = 1;
            isSolid = true;
        }
        public void setRectc(Point start, Point finish, int thick, bool isSolid, Color pen, Color brush, bool fill,int n)
        {
            this.point[0] = start;
            this.point[1] = finish;
            rectC.X = Math.Min(start.X, finish.X);
            rectC.Y = Math.Min(start.Y, finish.Y);
            rectC.Width = Math.Abs(start.X - finish.X);
            rectC.Height = Math.Abs(start.Y - finish.Y);
            this.thick = thick;
            this.isSolid = isSolid;
            this.pen = pen;
            this.brush = brush;
            this.fill = fill;
            this.n = n;
        }
        public Rectangle getRectC()
        {
            return rectC;
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
