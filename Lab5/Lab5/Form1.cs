using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Lab5
{
    public class MyEditor
    {
        private const int MaxShapeCount = 124;
        private static readonly MyEditor instance = new MyEditor();

        public Shape[] Shapes { get; private set; }
        public int ShapeCount { get; private set; }
        public int MaxShapes { get; private set; }

        private MyEditor()
        {
            MaxShapes = MaxShapeCount;
            Shapes = new Shape[MaxShapes];
            ShapeCount = 0;
        }

        public static MyEditor GetInstance()
        {
            return instance;
        }

        public bool AddShape(Shape shape)
        {
            if (shape == null || ShapeCount >= MaxShapes)
            {
                return false;
            }

            Shapes[ShapeCount++] = shape;
            return true;
        }
        // Bonus 2
        public void RemoveShapeAt(int index)
        {
            if (index < 0 || index >= ShapeCount)
            {
                return;
            }

            for (int i = index; i < ShapeCount - 1; i++)
            {
                Shapes[i] = Shapes[i + 1];
            }

            Shapes[--ShapeCount] = null;
        }

        public void Clear()
        {
            Array.Clear(Shapes, 0, ShapeCount);
            ShapeCount = 0;
        }
        // Bonus 3
        public void SaveToFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < ShapeCount; i++)
                {
                    Shape shape = Shapes[i];

                    if (shape != null)
                    {
                        writer.WriteLine(
                            $"{shape.GetTypeName()},{shape.X1},{shape.Y1},{shape.X2},{shape.Y2}");
                    }
                }
            }
        }
        // Bonus 3
        public void LoadFromFile(
        string path,
        Action<string, int, int, int, int> creator)
        {
            if (!File.Exists(path))
            {
                return;
            }

            Clear();

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(',');

                if (parts.Length != 5)
                {
                    continue;
                }

                if (int.TryParse(parts[1], out int x1) &&
                    int.TryParse(parts[2], out int y1) &&
                    int.TryParse(parts[3], out int x2) &&
                    int.TryParse(parts[4], out int y2))
                {
                    creator(parts[0].Trim(), x1, y1, x2, y2);
                }
            }
        }
    }

    public partial class MyTable : Form
    {
        private DataGridView dataGridView1;
        private Button btnDelete;
        public event Action<int> OnRowSelected;
        public event Action<int> OnRowDeleted;
        public event Action OnTableClosed;

        public MyTable()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Table of graphic objects";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 240,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false
            };

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColName", HeaderText = "Name", Width = 130, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColX1", HeaderText = "x1", Width = 80, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColY1", HeaderText = "y1", Width = 80, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColX2", HeaderText = "x2", Width = 80, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColY2", HeaderText = "y2", Width = 80, Resizable = DataGridViewTriState.False });

            // Bonus 1
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 144, 238, 144);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Blue;

            dataGridView1.SelectionChanged += (s, e) =>
            {
                if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.Focused)
                {
                    int index = dataGridView1.SelectedRows[0].Index;
                    OnRowSelected?.Invoke(index);
                }
            };
            // Bonus 2
            btnDelete = new Button
            {
                Text = "Delete selected object",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            btnDelete.Click += (s, e) =>
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;
                    OnRowDeleted?.Invoke(index);
                    //RefreshTable();
                }
            };

            this.Controls.Add(btnDelete);
            this.Controls.Add(dataGridView1);
        }

        public void RefreshTable(string[][] rows)
        {
            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Clear();
            foreach (string[] row in rows)
            {
                dataGridView1.Rows.Add(row);
            }

            dataGridView1.ClearSelection();
            dataGridView1.ResumeLayout();
            dataGridView1.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                OnTableClosed?.Invoke();
                this.Hide();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }

    // Main window form
    public partial class Form1 : Form
    {
        private MyEditor editor;
        private MyTable tableForm;
        private int currentTool = 2;

        private bool isDrawing = false;
        private Point startPoint;
        private Point currentPoint;

        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile, menuObjects, menuHelp, menuTable;
        private ToolStripMenuItem itemSave, itemLoad;
        private ToolStripMenuItem itemDot, itemLine, itemRect, itemEllipse, itemLineCircles, itemCube;

        private ToolStrip toolStrip1;
        private ToolStripButton btnDot, btnLine, btnRect, btnEllipse, btnLineCircles, btnCube;

        public Form1()
        {
            InitializeComponent();
            editor = MyEditor.GetInstance();
            this.DoubleBuffered = true;
            SetupMenu();
            SetupToolbar();
            UpdateTitle();

            tableForm = new MyTable();
            // Bonus 1
            tableForm.OnRowSelected += (index) =>
            {
                for (int i = 0; i < editor.ShapeCount; i++)
                {
                    if (editor.Shapes[i] != null)
                    {
                        editor.Shapes[i].IsSelected = (i == index);
                    }
                }
                this.Invalidate();
            };
            // Bonus 2
            tableForm.OnRowDeleted += (index) =>
            {
                editor.RemoveShapeAt(index);
                for (int i = 0; i < editor.ShapeCount; i++)
                {
                    if (editor.Shapes[i] != null)
                    {
                        editor.Shapes[i].IsSelected = false;
                    }
                }
                this.RefreshTableIfVisible();
                this.Invalidate();
            };
            tableForm.OnTableClosed += () =>
            {
                this.ClearSelection();
                this.Invalidate();
            };
        }

        private void ClearSelection()
        {
            for (int i = 0; i < editor.ShapeCount; i++)
            {
                if (editor.Shapes[i] != null)
                {
                    editor.Shapes[i].IsSelected = false;
                }
            }
        }

        private string[][] GetTableRows()
        {
            string[][] rows = new string[editor.ShapeCount][];

            for (int i = 0; i < editor.ShapeCount; i++)
            {
                Shape shape = editor.Shapes[i];

                rows[i] = new string[]
                {
            shape.GetTypeName(),
            shape.X1.ToString(),
            shape.Y1.ToString(),
            shape.X2.ToString(),
            shape.Y2.ToString()
                };
            }

            return rows;
        }

        private void RefreshTableIfVisible()
        {
            if (tableForm != null && tableForm.Visible)
            {
                tableForm.RefreshTable(GetTableRows());
            }
        }

        private void SetupMenu()
        {
            menuStrip1 = new MenuStrip();
            // Bonus 3
            menuFile = new ToolStripMenuItem("File");
            itemSave = new ToolStripMenuItem("Save to file...", null, (s, e) => this.SaveShapesFile());
            itemLoad = new ToolStripMenuItem("Load from file...", null, (s, e) => this.LoadShapesFile());
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { itemSave, itemLoad });

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

            menuTable = new ToolStripMenuItem("Table", null, (s, e) => ToggleTable());
            menuHelp = new ToolStripMenuItem("Information");

            menuStrip1.Items.AddRange(new ToolStripItem[] { menuFile, menuObjects, menuTable, menuHelp });
            this.MainMenuStrip = menuStrip1;
            this.Controls.Add(menuStrip1);
        }

        private void ToggleTable()
        {
            if (tableForm.Visible)
            {
                tableForm.Hide();
                ClearSelection();
                Invalidate();
            }
            else
            {
                tableForm.RefreshTable(GetTableRows());
                tableForm.Show(this);
            }
        }
        // Bonus 3
        private void SaveShapesFile()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    editor.SaveToFile(sfd.FileName);
                }
            }
        }
        // Bonus 3
        private void LoadShapesFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    editor.LoadFromFile(ofd.FileName, (type, x1, y1, x2, y2) =>
                    {
                        Shape newShape = type switch
                        {
                            "Dot" => new DotShape(x1, y1, x2, y2),
                            "Line" => new LineShape(x1, y1, x2, y2),
                            "Rectangle" => new RectShape(x1, y1, x2, y2),
                            "Ellipse" => new EllipseShape(x1, y1, x2, y2),
                            "Line with circles" => new LineCirclesShape(x1, y1, x2, y2),
                            "Cube" => new CubeShape(x1, y1, x2, y2),
                            _ => null
                        };

                        if (newShape != null)
                        {
                            editor.AddShape(newShape);
                        }
                    });

                    RefreshTableIfVisible();
                    Invalidate();
                }
            }
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
                g.FillEllipse(Brushes.Black, 10, 10, 4, 4); 
            return bmp; 
        }
        private Image CreateLineIcon() 
        { 
            Bitmap bmp = new Bitmap(24, 24); 
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen p = new Pen(Color.Black, 1.5f)) 
                g.DrawLine(p, 3, 21, 21, 3); 
            return bmp; 
        }
        private Image CreateRectIcon() 
        { 
            Bitmap bmp = new Bitmap(24, 24); 
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen p = new Pen(Color.Black, 1.5f)) 
                g.DrawRectangle(p, 3, 5, 18, 14); 
            return bmp; 
        }
        private Image CreateEllipseIcon() 
        { 
            Bitmap bmp = new Bitmap(24, 24); 
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen p = new Pen(Color.Black, 1.5f)) 
                g.DrawEllipse(p, 3, 5, 18, 14); 
            return bmp; 
        }
        private Image CreateLineCirclesIcon() 
        { 
            Bitmap bmp = new Bitmap(24, 24); 
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen p = new Pen(Color.Black, 1.5f)) 
            { 
                g.DrawLine(p, 4, 20, 20, 4); 
                g.FillEllipse(Brushes.Black, 2, 17, 5, 5); 
                g.FillEllipse(Brushes.Black, 17, 2, 5, 5); 
            } 
            return bmp; 
        }
        private Image CreateCubeIcon() 
        { 
            Bitmap bmp = new Bitmap(24, 24); 
            using (Graphics g = Graphics.FromImage(bmp)) 
            using (Pen p = new Pen(Color.Black, 1.5f)) 
            { 
                g.DrawRectangle(p, 3, 8, 12, 12); 
                g.DrawRectangle(p, 9, 2, 12, 12); 
                g.DrawLine(p, 3, 8, 9, 2); 
                g.DrawLine(p, 15, 8, 21, 2); 
                g.DrawLine(p, 3, 20, 9, 14); 
                g.DrawLine(p, 15, 20, 21, 14); 
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
                5 => "Cube",
                _ => ""
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
                Shape newShape = currentTool switch
                {
                    0 => new DotShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y),
                    1 => new LineShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y),
                    2 => new RectShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y),
                    3 => new EllipseShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y),
                    4 => new LineCirclesShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y),
                    5 => new CubeShape(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y),
                    _ => null
                };

                if (newShape != null)
                {
                    if (editor.AddShape(newShape))
                    {
                        RefreshTableIfVisible();
                    }
                    else
                    {
                        MessageBox.Show("The maximum number of objects has been reached.");
                    }
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
                                g.DrawEllipse(rubberPen, startPoint.X - 4, startPoint.Y - 4, 8, 8);
                                g.DrawEllipse(rubberPen, currentPoint.X - 4, currentPoint.Y - 4, 8, 8);
                            }
                            break;
                        case 5:
                            {
                                int minX = Math.Min(startPoint.X, currentPoint.X);
                                int minY = Math.Min(startPoint.Y, currentPoint.Y);
                                int width = Math.Abs(currentPoint.X - startPoint.X);
                                int height = Math.Abs(currentPoint.Y - startPoint.Y);

                                if (width < 8) width = 8;
                                if (height < 8) height = 8;

                                int offset = Math.Max(4, Math.Min(width, height) / 4);

                                int fLeft = minX;
                                int fTop = minY + offset;
                                int fRight = minX + (width - offset);
                                int fBottom = minY + height;

                                int bLeft = minX + offset;
                                int bTop = minY;
                                int bRight = minX + width;
                                int bBottom = minY + (height - offset);

                                int fCx = (fLeft + fRight) / 2;
                                int fCy = (fTop + fBottom) / 2;
                                int bCx = (bLeft + bRight) / 2;
                                int bCy = (bTop + bBottom) / 2;

                                int fWidth = fRight - fLeft;
                                int fHeight = fBottom - fTop;
                                int bWidth = bRight - bLeft;
                                int bHeight = bBottom - bTop;

                                g.DrawRectangle(rubberPen, fLeft, fTop, fWidth, fHeight);
                                g.DrawRectangle(rubberPen, bLeft, bTop, bWidth, bHeight);
                                g.DrawLine(rubberPen, fLeft, fTop, bLeft, bTop);
                                g.DrawLine(rubberPen, fRight, fTop, bRight, bTop);
                                g.DrawLine(rubberPen, fLeft, fBottom, bLeft, bBottom);
                                g.DrawLine(rubberPen, fRight, fBottom, bRight, bBottom);
                            }
                            break;
                    }
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            tableForm?.Close();
            base.OnFormClosed(e);
        }
    }
}