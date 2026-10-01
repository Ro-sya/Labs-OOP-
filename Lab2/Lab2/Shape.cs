using System.Drawing;
using System.Drawing.Drawing2D;

namespace Lab2
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        { 
            X = x;
            Y = y;
        }
    }

    public abstract class Shape
    { 
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public abstract void Draw(Graphics g);
    }

    public class DotShape : Shape
    {
        public DotShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(brush, X1 - 1, Y1 - 1, 2, 2);
            }
        }
    }

    public class LineShape : Shape
    {

        public LineShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawLine(pen, X1, Y1, X2, Y2);
                }
            
        }
    }

    public class RectShape : Shape
    {
        public RectShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override void Draw(Graphics g)
        {
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);

            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawRectangle(pen, x, y, width, height);  
            }
        }

    }

    public class EllipseShape : Shape
    {
        public EllipseShape(int x1, int y1, int x2, int y2)
        { 
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override void Draw(Graphics g)
        {
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);
            int x = X1 - dx;
            int y = Y1 - dy;
            int width = dx * 2;
            int height = dy * 2;

            using (Brush brush = new SolidBrush(Color.Gray))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.FillEllipse(brush, x, y, width, height);
                g.DrawEllipse(pen, x, y, width, height);
            }
        }
    }
}
