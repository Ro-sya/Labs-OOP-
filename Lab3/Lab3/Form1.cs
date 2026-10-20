using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form1 : Form
    {
        private Shape[] pcshape;
        private int shapeCount = 0;
        private const int MaxShapes = 125;

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

        private ToolStrip toolStrip1;
        private ToolStripButton btnDot;
        private ToolStripButton btnLine;
        private ToolStripButton btnRect;
        private ToolStripButton btnEllipse;

        public Form1()
        {
            InitializeComponent();
            pcshape = new Shape[MaxShapes];
            this.DoubleBuffered = true;
            SetupMenu();
            SetupToolbar();
            UpdateTitle();
        }

        private void SetupMenu()
        {
            menuStrip1 = new MenuStrip();
            menuFile = new ToolStripMenuItem("File");

            menuObjects = new ToolStripMenuItem("Object");
            menuObjects.DropDownOpening += (s, e) =>
            {
                itemDot.Checked = (currentTool == 0);
                itemLine.Checked = (currentTool == 1);
                itemRect.Checked = (currentTool == 2);
                itemEllipse.Checked = (currentTool == 3);
            };

            itemDot = new ToolStripMenuItem("Dot", null, (s, e) => SetTool(0));
            itemLine = new ToolStripMenuItem("Line", null, (s, e) => SetTool(1));
            itemRect = new ToolStripMenuItem("Rectangle", null, (s, e) => SetTool(2));
            itemEllipse = new ToolStripMenuItem("Ellipse", null, (s, e) => SetTool(3));

            menuObjects.DropDownItems.Add(itemDot);
            menuObjects.DropDownItems.Add(itemLine);
            menuObjects.DropDownItems.Add(itemRect);
            menuObjects.DropDownItems.Add(itemEllipse);

            menuHelp = new ToolStripMenuItem("Information");

            menuStrip1.Items.Add(menuFile);
            menuStrip1.Items.Add(menuObjects);
            menuStrip1.Items.Add(menuHelp);

            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);
        }

        private void SetupToolbar()
        {
            toolStrip1 = new ToolStrip();

            btnDot = new ToolStripButton(null, CreateDotIcon(), (s, e) => SetTool(0)) { ToolTipText = "Dot" };
            btnLine = new ToolStripButton(null, CreateLineIcon(), (s, e) => SetTool(1)) { ToolTipText = "Line" };
            btnRect = new ToolStripButton(null, CreateRectIcon(), (s, e) => SetTool(2)) { ToolTipText = "Rectangle" };
            btnEllipse = new ToolStripButton(null, CreateEllipseIcon(), (s, e) => SetTool(3)) { ToolTipText = "Ellipse" };

            toolStrip1.Items.Add(btnDot);
            toolStrip1.Items.Add(btnLine);
            toolStrip1.Items.Add(btnRect);
            toolStrip1.Items.Add(btnEllipse);

            this.Controls.Add(toolStrip1);
            toolStrip1.SendToBack();
            menuStrip1.SendToBack();
        }

        private Image CreateDotIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Brush b = new SolidBrush(Color.Black))
                {
                    g.FillEllipse(b, 10, 10, 4, 4);
                }
            }
            return bmp;
        }

        private Image CreateLineIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 2))
                {
                    g.DrawLine(p, 4, 20, 20, 4);
                }
            }
            return bmp;
        }

        private Image CreateRectIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 2))
                {
                    g.DrawRectangle(p, 4, 6, 16, 12);
                }
            }
            return bmp;
        }

        private Image CreateEllipseIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 2))
                {
                    g.DrawEllipse(p, 4, 6, 16, 12);
                }
            }
            return bmp;
        }
        
        private void SetTool(int toolIndex)
        {
            currentTool = toolIndex;
            UpdateTitle();
            btnDot.Checked = (currentTool == 0);
            btnLine.Checked = (currentTool == 1);
            btnRect.Checked = (currentTool == 2);
            btnEllipse.Checked = (currentTool == 3);
        }
        // в заголовку вікна
        private void UpdateTitle()
        {
            string toolName = "";
            switch (currentTool)
            {
                case 0: toolName = "Dot"; break;
                case 1: toolName = "Line"; break;
                case 2: toolName = "Rectangle"; break;
                case 3: toolName = "Ellipse"; break;
            }
            this.Text = $"Current object: {toolName}";
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Y > menuStrip1.Height + toolStrip1.Height)
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
                        newShape = new DotShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y);
                        break;
                    case 1:
                        newShape = new LineShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
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
        // гумовий слід червоний
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
                using (Pen rubberPen = new Pen(Color.Red, 1))
                {
                    switch (currentTool)
                    {
                        case 0:
                            g.DrawEllipse(rubberPen, startPoint.X - 1, startPoint.Y - 1, 2, 2);
                            break;
                        case 1:
                            g.DrawLine(rubberPen, startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y);
                            break;
                        case 2:
                            int dxR = Math.Abs(currentPoint.X - startPoint.X);
                            int dyR = Math.Abs(currentPoint.Y - startPoint.Y);
                            g.DrawRectangle(rubberPen, startPoint.X - dxR, startPoint.Y - dyR, dxR * 2, dyR * 2);
                            break;
                        case 3:
                            int rx = Math.Min(startPoint.X, currentPoint.X);
                            int ry = Math.Min(startPoint.Y, currentPoint.Y);
                            int rw = Math.Abs(currentPoint.X - startPoint.X);
                            int rh = Math.Abs(currentPoint.Y - startPoint.Y);
                            g.DrawEllipse(rubberPen, rx, ry, rw, rh);
                            break;
                    }
                }
            }
        }
    }
}
