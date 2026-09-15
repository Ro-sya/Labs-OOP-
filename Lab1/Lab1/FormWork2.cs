using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public class FormWork2 : Form
    {
        private TrackBar trackBar1;
        private Label lbValue;
        private Button btnOk;
        private Button btnCancel;

        public int ScrollValue
        {
            get { return trackBar1.Value; }
        }

        public FormWork2()
        {
            this.Text = "Вибір значення (М2)";
            this.Size = new Size(380, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            trackBar1 = new TrackBar();
            trackBar1.Maximum = 100;
            trackBar1.Minimum = 1;
            trackBar1.Value = 50;
            trackBar1.Location = new Point(30, 20);
            trackBar1.Size = new Size(300, 45);
            trackBar1.Scroll += new EventHandler(TrackBar1_Scroll);

            lbValue = new Label();
            lbValue.Text = "Поточне значення: " + trackBar1.Value.ToString();
            lbValue.Location = new Point(30, 75);
            lbValue.AutoSize = true;

            btnOk = new Button();
            btnOk.Text = "Так";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(145, 120);

            btnCancel = new Button();
            btnCancel.Text = "Скасувати";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(230, 120);

            this.Controls.Add(trackBar1);
            this.Controls.Add(lbValue);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            lbValue.Text = "Поточне значення: " + trackBar1.Value.ToString();
        }
    }
}
