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
    public partial class report : Form
    {
        WebDB _db = new WebDB();
        List<PageDto> pageDtos = new List<PageDto>();
        List<Session> sessions = new List<Session>();
        List<User> users = new List<User>();
        List<int> groupUsers = new List<int>();
        int idx = 0;
        int groupId = 0;
        int id = 0;
        public report(int id)
        {
            InitializeComponent();
            pageDtos = _db.getAllOnSite(id);
            sessions = _db.allSessionsOnSite(id);
            List<int> usersIds = new List<int>();
            for (int i = 0; i < sessions.Count; i++) 
            {
                if (!usersIds.Contains(Convert.ToInt32(sessions[i].userId)))
                {
                    usersIds.Add(Convert.ToInt32(sessions[i].userId));
                }
            }
            for (int i = 0; i < usersIds.Count; i++) {
                var us = _db.getUserById(usersIds[i]);
                users.Add(us);
            }
            for (int i = 0; i < users.Count; i++)
            {
                groupUsers.Add(Convert.ToInt32(users[i].id));
            }
            comboBox1.DataSource = pageDtos;
            comboBox1.DisplayMember = "name";
            this.id = id;
            comboBox2.SelectedIndex = 0;
            update();
        }
        private void update()
        {
            
            List<SessionFrame> sfs = _db.getAllByPage(pageDtos[idx].id);
            
            List<User> users = new List<User>();

            List<List<String>> prs = pageDtos[idx].priority;
            double first = 0;
            double second = 0;
            double third = 0;
            bool entered = false;
            int countInGroup = 0;
            for (int i = 0; i < sfs.Count; i++)
            {
                var us = sessions.First(s => s.id == sfs[i].sessionId).userId;
                if (groupUsers.Contains(Convert.ToInt32(us)))
                {
                    entered = true;
                    if (eye.Checked)
                    {
                        first += sfs[i].first == null ? 0 : (double)sfs[i].first;
                        second += sfs[i].second == null ? 0 : (double)sfs[i].second;
                        third += sfs[i].third == null ? 0 : (double)sfs[i].third;
                    }
                    else
                    {
                        first += sfs[i].firstCursor == null ? 0 : (double)sfs[i].firstCursor;
                        second += sfs[i].secondCursor == null ? 0 : (double)sfs[i].secondCursor;
                        third += sfs[i].thirdCursor == null ? 0 : (double)sfs[i].thirdCursor;
                    }
                    countInGroup++;
                }
            }
            if (!entered)
            {
                string message = "Нет Испытуемых этого сайта выбранной персональной группы";
                string caption = "Недостаточно данных";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
            }
            
            textBox1.Text = (Math.Round(countInGroup != 0?first / countInGroup:0, 2)).ToString();
            textBox2.Text = (Math.Round(countInGroup != 0 ? second / countInGroup : 0)).ToString();
            textBox3.Text = (Math.Round(countInGroup != 0 ? third / countInGroup : 0, 2)).ToString();
            textBox4.Text = prs[0][0].ToString();
            textBox5.Text = prs[1][0].ToString();
            textBox6.Text = prs[2][0].ToString();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Курсор_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void eye_CheckedChanged(object sender, EventArgs e)
        {
            update();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            idx = comboBox1.SelectedIndex;
            update();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            idx = comboBox1.SelectedIndex;
            update();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            groupId = comboBox2.SelectedIndex;
            groupUsers.Clear();
            if (groupId == 1) {
                for (int i = 0; i < users.Count; i++) {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 1963 && date.Year <= 1981 && users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 2)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 1963 && date.Year <= 1981 && !users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 3)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 1982 && date.Year <= 2000 && users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 4)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 1982 && date.Year <= 2000 && !users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 5)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 2001 && date.Year <= 2004 && users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 6)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var date = Convert.ToDateTime(users[i].dateOfBirth);
                    if (date.Year >= 2001 && date.Year <= 2004 && !users[i].gender)
                    {
                        groupUsers.Add(Convert.ToInt32(users[i].id));
                    }
                }
            }
            if (groupId == 0) {
                for (int i = 0; i < users.Count; i++) {
                    groupUsers.Add(Convert.ToInt32(users[i].id));
                }
            }
            update();
        }
    }
}
