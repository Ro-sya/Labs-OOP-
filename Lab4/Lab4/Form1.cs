using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab4
{
    public class MyEditor
    {
        public Shape[] Shapes { get; private set; }
        public int ShapeCount { get; set; }
        public int MaxShapes { get; private set; }

        public MyEditor(int maxShapes)
        {
            MaxShapes = maxShapes;
            Shapes = new Shape[MaxShapes];
            ShapeCount = 0;
        }

        public void AddShape(Shape shape)
        {
            if (ShapeCount < MaxShapes)
            {
                Shapes[ShapeCount++] = shape;
            }
        }
    }

    public partial class Form1 : Form
    {
        private MyEditor editor;
        private int currentTool = 2;

        private bool isDrawing = false;
        private Point startPoint;
        private Point currentPoint;

        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile, menuObjects, menuHelp;
        private ToolStripMenuItem itemDot, itemLine, itemRect, itemEllipse, itemLineCircles, itemCube;

        private ToolStrip toolStrip1;
        private ToolStripButton btnDot, btnLine, btnRect, btnEllipse, btnLineCircles, btnCube;

        public Form1()
        {
            InitializeComponent();
            editor = new MyEditor(125);
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
                itemLineCircles.Checked = (currentTool == 4);
                itemCube.Checked = (currentTool == 5);
            };

            itemDot = new ToolStripMenuItem("Dot", null, (s, e) => SetTool(0));
            itemLine = new ToolStripMenuItem("Line", null, (s, e) => SetTool(1));
            itemRect = new ToolStripMenuItem("Rectangle", null, (s, e) => SetTool(2));
            itemEllipse = new ToolStripMenuItem("Ellipse", null, (s, e) => SetTool(3));
            itemLineCircles = new ToolStripMenuItem("Line with circles", null, (s, e) => SetTool(4));
            itemCube = new ToolStripMenuItem("Cube", null, (s, e) => SetTool(5));

            menuObjects.DropDownItems.AddRange(new ToolStripItem[] { itemDot, itemLine, itemRect, itemEllipse, itemLineCircles, itemCube });
            menuHelp = new ToolStripMenuItem("Information");

            menuStrip1.Items.AddRange(new ToolStripItem[] { menuFile, menuObjects, menuHelp });
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
            btnLineCircles = new ToolStripButton(null, CreateLineCirclesIcon(), (s, e) => SetTool(4)) { ToolTipText = "Line with circles" };
            btnCube = new ToolStripButton(null, CreateCubeIcon(), (s, e) => SetTool(5)) { ToolTipText = "Cube" };

            toolStrip1.Items.AddRange(new ToolStripItem[] { btnDot, btnLine, btnRect, btnEllipse, btnLineCircles, btnCube });
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
                g.FillEllipse(Brushes.Black, 10, 10, 4, 4);
            }
            return bmp;
        }

        private Image CreateLineIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 1.5f))
                {
                    g.DrawLine(p, 3, 21, 21, 3);
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
                using (Pen p = new Pen(Color.Black, 1.5f))
                {
                    g.DrawRectangle(p, 3, 5, 18, 14);
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
                using (Pen p = new Pen(Color.Black, 1.5f))
                {
                    g.DrawEllipse(p, 3, 5, 18, 14);
                }
            }
            return bmp;
        }

        private Image CreateLineCirclesIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 1.5f))
                {
                    g.DrawLine(p, 4, 20, 20, 4);
                    g.FillEllipse(Brushes.Black, 2, 17, 5, 5);
                    g.FillEllipse(Brushes.Black, 17, 2, 5, 5);
                }
            }
            return bmp;
        }

        private Image CreateCubeIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using (Pen p = new Pen(Color.Black, 1.5f))
                {
                    g.DrawRectangle(p, 3, 8, 12, 12);
                    g.DrawRectangle(p, 9, 2, 12, 12);
                    g.DrawLine(p, 3, 8, 9, 2);
                    g.DrawLine(p, 15, 8, 21, 2);
                    g.DrawLine(p, 3, 20, 9, 14);
                    g.DrawLine(p, 15, 20, 21, 14);
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
            btnLineCircles.Checked = (currentTool == 4);
            btnCube.Checked = (currentTool == 5);
        }

        private void UpdateTitle()
        {
            string toolName = currentTool switch
            {
                0 => "Dot",
                1 => "Line",
                2 => "Rectangle",
                3 => "Ellipse",
                4 => "Line with circles",
                5 => "Cube"
            };
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
            if (isDrawing)
            {
                isDrawing = false;
                Point endPoint = new Point(e.X, e.Y);
                Shape newShape = null;

                switch (currentTool)
                {
                    case 0: newShape = new DotShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y); break;
                    case 1: newShape = new LineShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); break;
                    case 2: newShape = new RectShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); break;
                    case 3: newShape = new EllipseShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); break;
                    case 4: newShape = new LineCirclesShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); break;
                    case 5: newShape = new CubeShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); break;
                }

                if (newShape != null)
                {
                    editor.AddShape(newShape);
                }
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int i = 0; i < editor.ShapeCount; i++)
            {
                if (editor.Shapes[i] != null)
                {
                    editor.Shapes[i].Draw(g);
                }
            }

            if (isDrawing)
            {
                using (Pen rubberPen = new Pen(Color.Black, 1))
                {
                    rubberPen.DashStyle = DashStyle.Dash;

                    switch (currentTool)
                    {
                        case 0:
                            g.DrawEllipse(rubberPen, startPoint.X - 1, startPoint.Y - 1, 2, 2);
                            break;
                        case 1:
                            g.DrawLine(rubberPen, startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y);
                            break;
                        case 2:
                            {
                                int dx = Math.Abs(currentPoint.X - startPoint.X);
                                int dy = Math.Abs(currentPoint.Y - startPoint.Y);
                                g.DrawRectangle(rubberPen, startPoint.X - dx, startPoint.Y - dy, dx * 2, dy * 2);
                            }
                            break;
                        case 3:
                            {
                                int rx = Math.Min(startPoint.X, currentPoint.X);
                                int ry = Math.Min(startPoint.Y, currentPoint.Y);
                                int rw = Math.Abs(currentPoint.X - startPoint.X);
                                int rh = Math.Abs(currentPoint.Y - startPoint.Y);
                                g.DrawEllipse(rubberPen, rx, ry, rw, rh);
                            }
                            break;
                        case 4:
                            {
                                g.DrawLine(rubberPen, startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y);
                                using (Brush b = new SolidBrush(Color.Black))
                                {
                                    g.FillEllipse(b, startPoint.X - 4, startPoint.Y - 4, 8, 8);
                                    g.FillEllipse(b, currentPoint.X - 4, currentPoint.Y - 4, 8, 8);
                                }
                            }
                            break;
                        case 5:
                            {
                                int minX = Math.Min(startPoint.X, currentPoint.X);
                                int minY = Math.Min(startPoint.Y, currentPoint.Y);
                                int width = Math.Abs(currentPoint.X - startPoint.X);
                                int height = Math.Abs(currentPoint.Y - startPoint.Y);

                                if (width < 4) width = 4;
                                if (height < 4) height = 4;

                                int offset = Math.Max(2, Math.Min(width, height) / 4);

                                int frontX = minX;
                                int frontY = minY + offset;
                                int frontW = Math.Max(2, width - offset);
                                int frontH = Math.Max(2, height - offset);

                                int backX = minX + offset;
                                int backY = minY;
                                int backW = Math.Max(2, width - offset);
                                int backH = Math.Max(2, height - offset);

                                g.DrawRectangle(rubberPen, frontX, frontY, frontW, frontH);
                                g.DrawRectangle(rubberPen, backX, backY, backW, backH);
                                g.DrawLine(rubberPen, frontX, frontY, backX, backY);
                                g.DrawLine(rubberPen, frontX + frontW, frontY, backX + backW, backY);
                                g.DrawLine(rubberPen, frontX, frontY + frontH, backX, backY + backH);
                                g.DrawLine(rubberPen, frontX + frontW, frontY + frontH, backX + backW, backY + backH);
                            }
                            break;
                    }
                }
            }
        }
    }
}