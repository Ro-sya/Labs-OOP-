using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public class FormWork1 : Form
    {
        private TextBox textBox1;
        private Button btnOk;
        private Button btnCancel;

        public string InputText
        { 
            get { return textBox1.Text; }
        }

        public FormWork1()
        {
            this.Text = "Вивід тексту (М1)";
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label label = new Label();
            label.Text = "Введіть текст";
            label.Location = new Point(30, 20);
            label.AutoSize = true;

            textBox1 = new TextBox();
            textBox1.Location = new Point(30, 45);
            textBox1.Size = new Size(270, 25);

            btnOk = new Button();
            btnOk.Text = "Так";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(115, 100);

            btnCancel = new Button();
            btnCancel.Text = "Ні";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(200, 100);

            this.Controls.Add(label);
            this.Controls.Add(textBox1);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
