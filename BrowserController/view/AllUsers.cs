using BrowserController.DB;
using BrowserController.model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace BrowserController.view
{
    public partial class AllUsers : Form
    {
        private WebDB _db;
        public delegate void SetUsersDeligate(List<User> users);
        public delegate void SetUserPropertysDeligate(User user);

        public AllUsers(WebDB db)
        {
            InitializeComponent();
            _db = db;
        }

        private void AllUsers_Load(object sender, EventArgs e)
        {
            (new Thread(setUsers)).Start();
        }

        private async void setUsers()
        {
            BeginInvoke(new SetUsersDeligate(setUsers), (await _db.getAllUsers()));
        }

        private void setUsers(List<User> users)
        {
            usersDataGridView.DataSource = users;
            usersDataGridView.Columns["id"].Visible = false;
            usersDataGridView.Columns["name"].Visible = false;
            usersDataGridView.Columns["surname"].Visible = false;
            usersDataGridView.Columns["patronymic"].Visible = false;
            usersDataGridView.Columns["gender"].Visible = false;
            usersDataGridView.Columns["fio"].HeaderText = "ФИО испытуемого";
            usersDataGridView.Columns["dateOfBirth"].HeaderText = "Дата рождения";
            usersDataGridView.Columns["sex"].HeaderText = "Пол";
        }
        private async void setUser()
        {
            if (usersDataGridView.SelectedRows.Count > 0)
            {
                BeginInvoke(new SetUserPropertysDeligate(setUser), (await _db.getUser((long)usersDataGridView.SelectedRows[0].Cells[usersDataGridView.Columns.IndexOf(usersDataGridView.Columns["id"])].Value)));
            }
        }

        private void setUser(User user)
        {
            textBox2.Text = user.name;
            textBox3.Text = user.patronymic;
            textBox1.Text = user.surname;

            maskedTextBox1.Text = user.dateOfBirth.Split('-')[2]+ user.dateOfBirth.Split('-')[1]+ user.dateOfBirth.Split('-')[0];
            radioButton1.Checked = user.gender;
            radioButton2.Checked = !user.gender;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateUser cu = new CreateUser(_db);
            cu.ShowDialog();
            AllUsers_Load(this, new EventArgs());
        }

        private void usersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            (new Thread(setUser)).Start();
        }
    }
}
