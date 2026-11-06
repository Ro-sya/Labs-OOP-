using System;
using System.Drawing;

namespace Lab5
{
    public abstract class Shape
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        // Bonus 1
        public bool IsSelected { get; set; } = false;

        public abstract void Draw(Graphics g);
        public abstract string GetTypeName();
    }

    public class DotShape : Shape
    {
        public DotShape(int x1, int y1, int x2, int y2) { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public override string GetTypeName() => "Dot";
        public override void Draw(Graphics g)
        {
            Color col = IsSelected ? Color.Red : Color.Black; // Bonus 1
            using (Brush brush = new SolidBrush(col))
            {
                g.FillEllipse(brush, X1 - 1, Y1 - 1, IsSelected ? 4 : 2, IsSelected ? 4 : 2);
            }
        }
    }

    public class LineShape : Shape
    {
        public LineShape(int x1, int y1, int x2, int y2) { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public override string GetTypeName() => "Line";
        public override void Draw(Graphics g)
        {
            Color col = IsSelected ? Color.Red : Color.Black; // Bonus 1
            using (Pen pen = new Pen(col, IsSelected ? 2 : 1))
            {
                g.DrawLine(pen, X1, Y1, X2, Y2);
            }
        }
    }

    public class RectShape : Shape
    {
        public RectShape(int x1, int y1, int x2, int y2) { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public override string GetTypeName() => "Rectangle";
        public override void Draw(Graphics g)
        {
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);
            int x = X1 - dx;
            int y = Y1 - dy;
            int width = dx * 2;
            int height = dy * 2;
            Color col = IsSelected ? Color.Red : Color.Black; // Bonus 1
            using (Pen pen = new Pen(col, IsSelected ? 2 : 1))
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
        }
    }

    public class EllipseShape : Shape
    {
        public EllipseShape(int x1, int y1, int x2, int y2) { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        public override string GetTypeName() => "Ellipse";
        public override void Draw(Graphics g)
        {
            int x = Math.Min(X1, X2);
            int y = Math.Min(Y1, Y2);
            int width = Math.Abs(X2 - X1);
            int height = Math.Abs(Y2 - Y1);
            Color col = IsSelected ? Color.Red : Color.Black; // Bonus 1
            using (Pen pen = new Pen(col, IsSelected ? 2 : 1))
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

        public override string GetTypeName() => "Line with circles";
        public override void Draw(Graphics g)
        {
            // Bonus 1
            line.IsSelected = this.IsSelected;
            startCircle.IsSelected = this.IsSelected;
            endCircle.IsSelected = this.IsSelected;
            line.Draw(g);
            startCircle.Draw(g);
            endCircle.Draw(g);
        }
    }

    public class CubeShape : Shape
    {
        private RectShape frontFace, backFace;
        private LineShape edgeTopLeft, edgeTopRight, edgeBottomLeft, edgeBottomRight;

        public CubeShape(int x1, int y1, int x2, int y2)
        {
            X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
            int minX = Math.Min(x1, x2);
            int minY = Math.Min(y1, y2);
            int width = Math.Max(8, Math.Abs(x2 - x1));
            int height = Math.Max(8, Math.Abs(y2 - y1));
            int offset = Math.Max(4, Math.Min(width, height) / 4);

            int fLeft = minX, fTop = minY + offset, fRight = minX + (width - offset), fBottom = minY + height;
            int bLeft = minX + offset, bTop = minY, bRight = minX + width, bBottom = minY + (height - offset);

            frontFace = new RectShape((fLeft + fRight) / 2, (fTop + fBottom) / 2, fRight, fBottom);
            backFace = new RectShape((bLeft + bRight) / 2, (bTop + bBottom) / 2, bRight, bBottom);

            edgeTopLeft = new LineShape(fLeft, fTop, bLeft, bTop);
            edgeTopRight = new LineShape(fRight, fTop, bRight, bTop);
            edgeBottomLeft = new LineShape(fLeft, fBottom, bLeft, bBottom);
            edgeBottomRight = new LineShape(fRight, fBottom, bRight, bBottom);
        }

        public override string GetTypeName() => "Cube";
        public override void Draw(Graphics g)
        {
            // Bonus 1
            frontFace.IsSelected = this.IsSelected;
            backFace.IsSelected = this.IsSelected;
            edgeTopLeft.IsSelected = this.IsSelected;
            edgeTopRight.IsSelected = this.IsSelected;
            edgeBottomLeft.IsSelected = this.IsSelected;
            edgeBottomRight.IsSelected = this.IsSelected;

            frontFace.Draw(g);
            backFace.Draw(g);
            edgeTopLeft.Draw(g);
            edgeTopRight.Draw(g);
            edgeBottomLeft.Draw(g);
            edgeBottomRight.Draw(g);
        }
    }
}