using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private Shape[] pcshape;
        private int shapeCount = 0;
        private const int MaxShapes = 124;

        private int currentTool = 2;

        private bool isDrawing = false;
        private Point startPoint;
        private Point currentPoint;

        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuObjects;
        private ToolStripMenuItem itemDot;
        private ToolStripMenuItem itemLine;
        private ToolStripMenuItem itemRect;
        private ToolStripMenuItem itemEllipse;
        private ToolStripMenuItem menuHelp;

        public Form1()
        { 
            InitializeComponent();
            pcshape = new Shape[MaxShapes];
            this.DoubleBuffered = true;
            SetupMenu();
        }

        private void SetupMenu()
        {
            menuStrip1 = new MenuStrip();
            menuFile = new ToolStripMenuItem("Файл");

            menuObjects = new ToolStripMenuItem("Об'єкти");
            menuObjects.DropDownOpening += (s, e) =>
            {
                itemDot.Checked = (currentTool == 0);
                itemLine.Checked = (currentTool == 1);
                itemRect.Checked = (currentTool == 2);
                itemEllipse.Checked = (currentTool == 3);
            };

            itemDot = new ToolStripMenuItem("Крапка", null, (s, e) => SetTool(0));
            itemLine = new ToolStripMenuItem("Лінія", null, (s, e) => SetTool(1));
            itemRect = new ToolStripMenuItem("Прямокутник", null, (s, e) => SetTool(2));
            itemEllipse = new ToolStripMenuItem("Еліпс", null, (s, e) => SetTool(3));

            menuObjects.DropDownItems.Add(itemDot);
            menuObjects.DropDownItems.Add(itemLine);
            menuObjects.DropDownItems.Add(itemRect);
            menuObjects.DropDownItems.Add(itemEllipse);

            menuHelp = new ToolStripMenuItem("Довідка");

            menuStrip1.Items.Add(menuFile);
            menuStrip1.Items.Add(menuObjects);
            menuStrip1.Items.Add(menuHelp);

            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);
        }

        private void SetTool(int toolIndex)
        {
            currentTool = toolIndex;
        }

        protected override void OnMenuStart(EventArgs e)
        {
            base.OnMenuStart(e);
            itemDot.Checked = (currentTool == 0);
            itemLine.Checked = (currentTool == 1);
            itemRect.Checked = (currentTool == 2);
            itemEllipse.Checked = (currentTool == 3);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = new Point(e.X, e.Y);
                currentPoint = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDrawing)
            {
                currentPoint = new Point(e.X, e.Y);
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isDrawing && shapeCount < MaxShapes)
            {
                isDrawing = false;
                Point endPoint = new Point(e.X, e.Y);
                Shape newShape = null;

                switch (currentTool)
                {
                    case 0:
                        newShape = new DotLineShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y, true);
                        break;
                    case 1:
                        newShape = new DotLineShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, false);
                        break;
                    case 2:
                        newShape = new RectShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                        break;
                    case 3:
                        newShape = new EllipseShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                        break;
                }
                if (newShape != null) 
                {
                    pcshape[shapeCount++] = newShape;
                }

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int i = 0; i < shapeCount; i++)
            { 
                if (pcshape[i] != null)
                {
                    pcshape[i].Draw(g);
                }
            }

            if (isDrawing)
            {
                using (Pen rubberPen = new Pen(Color.Black, 1))
                {
                    switch (currentTool)
                    {
                        case 0:
                            g.DrawEllipse(rubberPen, startPoint.X - 1, startPoint.Y - 1, 2, 3);
                            break;
                        case 1:
                            g.DrawLine(rubberPen, startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y);
                            break;
                        case 2:
                            int rx = Math.Min(startPoint.X, currentPoint.X);
                            int ry = Math.Min(startPoint.Y, currentPoint.Y);
                            int rw = Math.Abs(currentPoint.X - startPoint.X);
                            int rh = Math.Abs(currentPoint.Y - startPoint.Y);
                            g.DrawRectangle(rubberPen, rx, ry, rw, rh);
                            break;
                        case 3:
                            int dx = Math.Abs(currentPoint.X - startPoint.X);
                            int dy = Math.Abs(currentPoint.Y - startPoint.Y);
                            int ex = startPoint.X - dx;
                            int ey = startPoint.Y - dy;
                            g.DrawEllipse(rubberPen, ex, ey, dx * 2, dy * 2);
                            break;
                    }
                }
            }
        }
    }
}
