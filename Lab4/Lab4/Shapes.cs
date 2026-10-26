using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Lab4
{
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
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);
            int x = X1 - dx;
            int y = Y1 - dy;
            int width = dx * 2;
            int height = dy * 2;
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
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawEllipse(pen, x, y, width, height);
            }
        }
    }

    public class LineCirclesShape : Shape
    {
        private LineShape line;
        private EllipseShape startCircle;
        private EllipseShape endCircle;

        public LineCirclesShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
            line = new LineShape(x1, y1, x2, y2);
            startCircle = new EllipseShape(x1 - 4, y1 - 4, x1 + 4, y1 + 4);
            endCircle = new EllipseShape(x2 - 4, y2 - 4, x2 + 4, y2 + 4);
        }

        public override void Draw(Graphics g)
        {
            line.Draw(g);
            startCircle.Draw(g);
            endCircle.Draw(g);
        }
    }

    public class CubeShape : Shape
    {
        private RectShape frontFace;
        private RectShape backFace;
        private LineShape edgeTopLeft;
        private LineShape edgeTopRight;
        private LineShape edgeBottomLeft;
        private LineShape edgeBottomRight;

        public CubeShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;

            int minX = Math.Min(x1, x2);
            int minY = Math.Min(y1, y2);
            int width = Math.Abs(x2 - x1);
            int height = Math.Abs(y2 - y1);

            if (width < 8) width = 8;
            if (height < 8) height = 8;

            int offset = Math.Max(4, Math.Min(width, height) / 4);

            int fLeft = minX;
            int fTop = minY + offset;
            int fRight = minX + (width - offset);
            int fBottom = minY + height;

            int fCx = (fLeft + fRight) / 2;
            int fCy = (fTop + fBottom) / 2;

            int bLeft = minX + offset;
            int bTop = minY;
            int bRight = minX + width;
            int bBottom = minY + (height - offset);

            int bCx = (bLeft + bRight) / 2;
            int bCy = (bTop + bBottom) / 2;

            frontFace = new RectShape(fCx, fCy, fRight, fBottom);
            backFace = new RectShape(bCx, bCy, bRight, bBottom);

            edgeTopLeft = new LineShape(fLeft, fTop, bLeft, bTop);
            edgeTopRight = new LineShape(fRight, fTop, bRight, bTop);
            edgeBottomLeft = new LineShape(fLeft, fBottom, bLeft, bBottom);
            edgeBottomRight = new LineShape(fRight, fBottom, bRight, bBottom);
        }

        public override void Draw(Graphics g)
        {
            frontFace.Draw(g);
            backFace.Draw(g);

            edgeTopLeft.Draw(g);
            edgeTopRight.Draw(g);
            edgeBottomLeft.Draw(g);
            edgeBottomRight.Draw(g);
        }
    }
}