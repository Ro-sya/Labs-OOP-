using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        private string enteredText = "";
        private int scrollValue = -1;

        public Form1()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Лаба 1 - Варіант 24";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            //Головне меню
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem menuWork1 = new ToolStripMenuItem("Робота1");
            menuWork1.Click += new EventHandler(MenuWork1_Click);

            ToolStripMenuItem menuWork2 = new ToolStripMenuItem("Робота2");
            menuWork2.Click += new EventHandler(MenuWork2_Click);

            menuStrip.Items.Add(menuWork1);
            menuStrip.Items.Add(menuWork2);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            //Перемалювання вікна
            this.Paint += new PaintEventHandler(Form1_Paint);
        }

        private void MenuWork1_Click(object sender, EventArgs e)
        {
            FormWork1 dialog = new FormWork1();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                enteredText = dialog.InputText;
                this.Invalidate(); //оновнення гол. вікна 
            }
            dialog.Dispose();
        }

        private void MenuWork2_Click(object sender, EventArgs e)
        {
            FormWork2 dialog = new FormWork2();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scrollValue = dialog.ScrollValue;
                this.Invalidate(); //оновнення гол. вікна 
            }
            dialog.Dispose();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Arial", 12);

            if (string.IsNullOrEmpty(enteredText) == false)
            {
                graphics.DrawString("Результат з М1 (текст): " + enteredText, font, Brushes.Black, 30, 80);
            }

            if (scrollValue >= 1)
            {
                graphics.DrawString("Результат з М2 (повзунок): " + scrollValue.ToString(), font, Brushes.Blue, 30, 130);
            }
        }
    }
}