using BrowserController.DB;
using BrowserController.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserController.view
{
    public partial class CreateUser : Form
    {
        private WebDB _db;

        public CreateUser(WebDB db)
        {
            InitializeComponent();
            _db = db;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User u = new User();
            long day, month, year;
            if (!Int64.TryParse(maskedTextBox1.Text.Split('.')[0], out day) || !Int64.TryParse(maskedTextBox1.Text.Split('.')[1], out month) || !Int64.TryParse(maskedTextBox1.Text.Split('.')[2], out year) || textBox1.Text == "" || textBox2.Text == "")
            {
                string message = "Пожалуйста, введите данные: Фамилия, Имя и Дата рождения";
                string caption = "Неверные данные";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
            }
            else
            {
                u.dateOfBirth = maskedTextBox1.Text.Split('.')[2] + "-" + maskedTextBox1.Text.Split('.')[1] + "-" + maskedTextBox1.Text.Split('.')[0];

                u.gender = radioButton1.Checked;

                u.name = textBox2.Text;
                u.surname = textBox1.Text;
                u.patronymic = textBox3.Text;

                _db.CreateUser(u);

                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
