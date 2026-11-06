using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab5
{
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

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColName", HeaderText = "Name", Width = 120, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColX1", HeaderText = "x1", Width = 90, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColY1", HeaderText = "y1", Width = 90, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColX2", HeaderText = "x2", Width = 90, Resizable = DataGridViewTriState.False });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColY2", HeaderText = "y2", Width = 90, Resizable = DataGridViewTriState.False });

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
}
