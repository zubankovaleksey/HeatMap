using BrowserController.DB;
using BrowserController.model;
using BrowserController.view;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using AngleSharp.Html.Dom;
using OpenQA.Selenium;
using WebsitesScreenshot.SupportClasses;

namespace BrowserController
{
    public partial class Form1 : Form
    {
        public delegate void SetUsersDeligate(List<User> users);
        public delegate void SetSessionsDeligate(List<Session> sessions);
        public delegate void SetSitesDeligate(List<Site> sites);
        public delegate void SetSiteSessionsDeligate(List<Session> sessions);
        System.Windows.Forms.HtmlElement htmlDoc;

        WebDB _db = new WebDB();
        public enum BrowserEmulationVersion
        {
            Default = 0,
            Version7 = 7000,
            Version8 = 8000,
            Version8Standards = 8888,
            Version9 = 9000,
            Version9Standards = 9999,
            Version10 = 10000,
            Version10Standards = 10001,
            Version11 = 11000,
            Version11Edge = 11001
        }
        public static class WBEmulator
        {
            private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";

            public static int GetInternetExplorerMajorVersion()
            {
                int result;

                result = 0;

                try
                {
                    RegistryKey key;

                    key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);

                    if (key != null)
                    {
                        object value;

                        value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

                        if (value != null)
                        {
                            string version;
                            int separator;

                            version = value.ToString();
                            separator = version.IndexOf('.');
                            if (separator != -1)
                            {
                                int.TryParse(version.Substring(0, separator), out result);
                            }
                        }
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }
            private const string BrowserEmulationKey = InternetExplorerRootKey + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

            public static BrowserEmulationVersion GetBrowserEmulationVersion()
            {
                BrowserEmulationVersion result;

                result = BrowserEmulationVersion.Default;

                try
                {
                    RegistryKey key;

                    key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                    if (key != null)
                    {
                        string programName;
                        object value;

                        programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                        value = key.GetValue(programName, null);

                        if (value != null)
                        {
                            result = (BrowserEmulationVersion)Convert.ToInt32(value);
                        }
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }
            public static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
            {
                bool result;

                result = false;

                try
                {
                    RegistryKey key;

                    key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);

                    if (key != null)
                    {
                        string programName;

                        programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

                        if (browserEmulationVersion != BrowserEmulationVersion.Default)
                        {
                            // if it's a valid value, update or create the value
                            key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
                        }
                        else
                        {
                            // otherwise, remove the existing value
                            key.DeleteValue(programName, false);
                        }

                        result = true;
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }

            public static bool SetBrowserEmulationVersion()
            {
                int ieVersion;
                BrowserEmulationVersion emulationCode;

                ieVersion = GetInternetExplorerMajorVersion();
                emulationCode = BrowserEmulationVersion.Version9;
                return SetBrowserEmulationVersion(emulationCode);
            }
            public static bool IsBrowserEmulationSet()
            {
                return GetBrowserEmulationVersion() != BrowserEmulationVersion.Default;
            }
        }
        public Form1()
        {
            WBEmulator.SetBrowserEmulationVersion();
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private async void setUsers()
        {
            BeginInvoke(new SetUsersDeligate(setUsers), (await _db.getAllUsers()));
        }

        private void setUsers(List<User> users)
        {
            User last = new User();
            last.name = "Добавить";
            users.Add(last);
            usersNewSessionComboBox.DataSource = users;
            usersNewSessionComboBox.DisplayMember = "fio";
        }

        private async void setSessions()
        {
           BeginInvoke(new SetSessionsDeligate(setSessions), (await _db.getAllSessions()));
        }

        private void setSessions(List<Session> sessions)
        {
            
        }

        private void setSites(List<Site> sites)
        {
            if (sites.Count == 0)
            {
                button10.Enabled = false;
                button1.Enabled = false;
                button6.Enabled = false;
                button3.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button6.Enabled = true;
                button11.Enabled = true;
                button3.Enabled = true;
                button5.Enabled = true;
                button10.Enabled = true;
            }
            comboBox1.DataSource = sites;
            comboBox1.DisplayMember = "name";
            comboBox2.DataSource = new List<Site>(sites);
            comboBox2.DisplayMember = "name";
        }

        private void setSiteSessions(List<Session> sessions)
        {
            if (sessions.Count != 0)
            {
                button3.Enabled = true;
                comboBox3.DataSource = sessions;
                comboBox3.DisplayMember = "name";
            }
            else
            {
                button3.Enabled = false;
                comboBox3.DataSource = null;
            }

        }

        private async void setSitesAsync()
        {
            BeginInvoke(new SetSitesDeligate(setSites), (await _db.getAllSites()));
        }

        private async void setSiteSessionsAsync()
        {
            BeginInvoke(new SetSiteSessionsDeligate(setSiteSessions), (await _db.allOnSite(((Site)comboBox2.SelectedItem).id)));
        }

        //////////////////////////////////////////////////////////////////////////////////Обработчики
        private void usersToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AllUsers au = new AllUsers(_db);
            au.ShowDialog();
            Form1_Load(this, new System.EventArgs());
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
             var t = _db.getSites();
            if (t == null)
            {
                string message = "Связь с сервером потеряна";
                string caption = "Связь потеряна";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                if (MessageBox.Show(message, caption, buttons) == DialogResult.OK)
                {
                    Close();
                    return;
                }
            }
            (new Thread(setUsers)).Start();
            (new Thread(setSessions)).Start();
            (new Thread(setSitesAsync)).Start();
        }


        private async void button1_ClickAsync(object sender, System.EventArgs e)
        {
            long minutes = 0;
            long seconds = 0;
            if (!Int64.TryParse(maskedTextBox1.Text.Split(':')[0], out minutes) || !Int64.TryParse(maskedTextBox1.Text.Split(':')[1], out seconds) || textBox1.Text == "")
            {
                string message = "Пожалуйста, введите данные: Наименование эксперимента и Длительность эксперимента";
                string caption = "Неверные данные";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
            }
            else
            {
                //Создана новая сессия
                Session session = new Session();
                session.name = textBox1.Text;
                session.userId = ((User)usersNewSessionComboBox.SelectedItem).id;
                session.pageShowTime = minutes * 60000 + seconds * 1000;
                session.siteId = ((Site)comboBox1.SelectedItem).id;

                //Скачать и распаковать сайт
                /*await WebDB.DownloadFile(((Site)comboBox1.SelectedItem).fileLocation);
                string siteLocation = @"tmp\index.html";//////////////////////////////////////////////////////////////////??????*/

                //Начать сессию со стартивой страницы сайта
                SessionForm sf = new SessionForm((int)session.pageShowTime, ((Site)comboBox1.SelectedItem).fileLocation);
                sf.ShowDialog();
                bool isOk = sf.isConnected;
                if (!isOk) {
                    string message = "Tobii Eye Tracker не подключен или Испытуемый не смотрел в экран";
                    string caption = "Tobii не подключен";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;
                    result = MessageBox.Show(message, caption, buttons);
                    return;
                }
                if (!sf.isClose)
                {
                    //Если всё ок, создаь сессию в бд
                    session = _db.CreateSession(session);
                    List<FrameTraectoryes> trs = new List<FrameTraectoryes>();
                    List<PageDto> pds = _db.getAllOnSite((int)session.siteId);
                    bool isExist;
                    PageDto page = new PageDto();
                    //Для каждого фрейма ассоциировать с сессией
                    for (int i = 0; i < sf.urls.Count; i++)
                    {
                        isExist = false;
                        for (int j = 0; j < pds.Count; j++)
                        {
                            if (pds[j].name == sf.urls[i])
                            {
                                isExist = true;
                                page = _db.getPageId(new urlDto() { siteId = (int)session.siteId, url = sf.urls[i] });
                                break;
                            }
                        }
                        if (isExist)
                        {
                            //Создать связь траектории и фрейма
                            FrameTraectoryes ft = new FrameTraectoryes();
                            ft.trajectoryList = new List<Traectory>();

                            //Добавить к фрейму траектории курсора
                            Traectory t = new Traectory();
                            t.points = sf.cursorTraectory[i];
                            ft.trajectoryList.Add(t);

                            //Добавить к фрейму траекторию кликов
                            Traectory t2 = new Traectory();
                            t2.points = sf.eyeTraectory[i];
                            ft.trajectoryList.Add(t2);
                            ft.time = sf.showingTime[i];
                            ft.sessionFrame = new SessionFrame();
                            ft.sessionFrame.sessionId = session.id;
                            ft.sessionFrame.pageId = page.id;
                            double first = 0;
                            double second = 0;
                            double third = 0;
                            double firstCursor = 0;
                            double secondCursor = 0;
                            double thirdCursor = 0;
                            List<List<String>> prs = page.priority;
                            if (prs != null && prs.Count != 0)
                            {
                                for (int j = 0; j < sf.eyeTraectory[i].Count; j++)
                                {
                                    for (int k = 1; k < prs[0].Count; k += 4)
                                    {
                                        if (sf.eyeTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[0][k]), Convert.ToInt32(prs[0][k + 2])) && sf.eyeTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[0][k]), Convert.ToInt32(prs[0][k + 2]))
                                            && sf.eyeTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[0][k + 1]), Convert.ToInt32(prs[0][k + 3])) && sf.eyeTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[0][k + 1]), Convert.ToInt32(prs[0][k + 3])))
                                        {
                                            first += (double)(1.0 / sf.eyeTraectory[i].Count) * 100;
                                        }
                                    }
                                    for (int k = 1; k < prs[1].Count; k += 4)
                                    {
                                        if (sf.eyeTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[1][k]), Convert.ToInt32(prs[1][k + 2])) && sf.eyeTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[1][k]), Convert.ToInt32(prs[1][k + 2]))
                                        && sf.eyeTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[1][k + 1]), Convert.ToInt32(prs[1][k + 3])) && sf.eyeTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[1][k + 1]), Convert.ToInt32(prs[1][k + 3])))
                                        {
                                            second += (double)(1.0 / sf.eyeTraectory[i].Count) * 100;
                                        }
                                    }
                                    for (int k = 1; k < prs[2].Count; k += 4)
                                    {
                                        if (sf.eyeTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[2][k]), Convert.ToInt32(prs[2][k + 2])) && sf.eyeTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[2][k]), Convert.ToInt32(prs[2][k + 2]))
                                        && sf.eyeTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[2][k + 1]), Convert.ToInt32(prs[2][k + 3])) && sf.eyeTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[2][k + 1]), Convert.ToInt32(prs[2][k + 3])))
                                        {
                                            third += (double)(1.0 / sf.eyeTraectory[i].Count) * 100;
                                        }
                                    }
                                }
                                for (int j = 0; j < sf.cursorTraectory[i].Count; j++)
                                {
                                    for (int k = 1; k < prs[0].Count; k += 4)
                                    {
                                        if (sf.cursorTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[0][k]), Convert.ToInt32(prs[0][k + 2])) && sf.cursorTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[0][k]), Convert.ToInt32(prs[0][k + 2]))
                                            && sf.cursorTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[0][k + 1]), Convert.ToInt32(prs[0][k + 3])) && sf.cursorTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[0][k + 1]), Convert.ToInt32(prs[0][k + 3])))
                                        {
                                            firstCursor += (double)(1.0 / sf.cursorTraectory[i].Count) * 100;
                                        }
                                    }
                                    for (int k = 1; k < prs[1].Count; k += 4)
                                    {
                                        if (sf.cursorTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[1][k]), Convert.ToInt32(prs[1][k + 2])) && sf.cursorTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[1][k]), Convert.ToInt32(prs[1][k + 2]))
                                        && sf.cursorTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[1][k + 1]), Convert.ToInt32(prs[1][k + 3])) && sf.cursorTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[1][k + 1]), Convert.ToInt32(prs[1][k + 3])))
                                        {
                                            secondCursor += (double)(1.0 / sf.cursorTraectory[i].Count) * 100;
                                        }
                                    }
                                    for (int k = 1; k < prs[2].Count; k += 4)
                                    {
                                        if (sf.cursorTraectory[i][j].X > Math.Min(Convert.ToInt32(prs[2][k]), Convert.ToInt32(prs[2][k + 2])) && sf.cursorTraectory[i][j].X < Math.Max(Convert.ToInt32(prs[2][k]), Convert.ToInt32(prs[2][k + 2]))
                                        && sf.cursorTraectory[i][j].Y > Math.Min(Convert.ToInt32(prs[2][k + 1]), Convert.ToInt32(prs[2][k + 3])) && sf.cursorTraectory[i][j].Y < Math.Max(Convert.ToInt32(prs[2][k + 1]), Convert.ToInt32(prs[2][k + 3])))
                                        {
                                            thirdCursor += (double)(1.0 / sf.cursorTraectory[i].Count) * 100;
                                        }
                                    }
                                }
                            }
                            ft.sessionFrame.first = first;
                            ft.sessionFrame.second = second;
                            ft.sessionFrame.third = third;
                            ft.sessionFrame.firstCursor = firstCursor;
                            ft.sessionFrame.secondCursor = secondCursor;
                            ft.sessionFrame.thirdCursor = thirdCursor;
                            trs.Add(ft);
                        }
                    }
                    _db.CreateFrames(trs);
                    BeginInvoke(new SetSiteSessionsDeligate(setSiteSessions), (await _db.allOnSite(((Site)comboBox2.SelectedItem).id)));
                }
                else
                {

                    string message = "Эксперимент не был сохранен, так как был прерван";
                    string caption = "Прерывание эксперимента";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.
                    result = MessageBox.Show(message, caption, buttons);
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //По сессии для каждого фрейма, содержащего эту страницу
        }

        private async void button3_ClickAsync(object sender, EventArgs e)
        {
            //if (webBrowser1.Url==null || webBrowser1.Url.ToString()== "about:blank") {
            //    MessageBox.Show("Для этого нажмите кнопку \"Показать сайт\"", "Снчала следует открыть сайт");
            //    return;
            //}
            //var pageId = _db.getPageId(new urlDto() { siteId = (int)((Session)comboBox3.SelectedItem).siteId,
            //    url = webBrowser1.Url.LocalPath.Split('\\')[webBrowser1.Url.LocalPath.Split('\\').Length - 1] }).id;
            //var a = await _db.getAllBySessionIdAndPageId((int)((Session)comboBox3.SelectedItem).id, pageId);
            //var b = await _db.getTraectories((int)a[0].id);

            HeatMapWindow hmw = new HeatMapWindow(1338, 1395, _db, (int)((Session)comboBox3.SelectedItem).id, ((Site)comboBox2.SelectedItem).id, ((Site)comboBox2.SelectedItem).fileLocation);//new HeatMapWindow(1338, 701, b[0].points);
            hmw.Show();
        }

        void w_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                return;
            htmlDoc = this.webBrowser1.Document.GetElementsByTagName("HTML")[0];
            System.Windows.Forms.HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            System.Windows.Forms.HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = "function deleteTargetBlank(){ var links = document.links, i, length; for (i = 0, length = links.length; i < length; i++) {links[i].target == '_blank' && links[i].removeAttribute('target');}}";
            head.AppendChild(scriptEl);
            webBrowser1.Document.InvokeScript("deleteTargetBlank");
        }
        private async void button6_ClickAsync(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                webBrowser1.Navigate(((Site)comboBox1.SelectedItem).fileLocation);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(w_DocumentCompleted);
            }
        }

        private async void comboBox2_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                BeginInvoke(new SetSiteSessionsDeligate(setSiteSessions), (await _db.allOnSite(((Site)comboBox2.SelectedItem).id)));
                //(new Thread(setSiteSessionsAsync)).Start();

            }
        }

        private void button3_TabIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                button3.Visible = true;
                button5.Visible = true;
                webBrowser1.Visible = false;
                button6.Visible = false;
                textBox3.Visible = false;
                button9.Visible = false;
                button11.Visible = true;
            }
            else
            {
                button11.Visible = false;
                button6.Visible = true;
                webBrowser1.Visible = true;
                textBox3.Visible = true;
                button9.Visible = true;
                button3.Visible = false;
                button5.Visible = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void heatMapSettings_Click(object sender, EventArgs e)
        {
            Settings s = new Settings();
            s.ShowDialog();
        }
        void Doc2Comp(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            
        }
        private void button9_Click(object sender, EventArgs e)
        {
            
            setSitesAsync();
            string site = textBox3.Text;
            if (string.IsNullOrEmpty(site)) {
                string message = "Пустая строка сайта недопустима";
                string caption = "Пустая строка";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                return;
            }
            if (site.StartsWith("http://"))
            {
                site = site.Substring(7);
            }
            else if (site.StartsWith("https://"))
            {
                site = site.Substring(8);
            }
            if (site.StartsWith("www."))
            {
                site = site.Substring(4);
            }
            site = site.Split('/')[0];
            var sites = _db.getSites();
            bool sitesIsExist = false;
            for (int i = 0; i < sites.Count; i++)
            {
                if (sites[i].fileLocation == site)
                {
                    sitesIsExist = true;
                    break;
                }
            }
            if (sitesIsExist)
            {
                string message = "Указанный Вами сайт уже есть в системе";
                string caption = "Сайта существует";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                return;
            }
            painter painter = new painter(site, 0, true,true);
            painter.ShowDialog();
            if (!painter.siteIsExist)
            {
                string message = "Указанный Вами сайт не существует";
                string caption = "Сайта не существует";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
            }
            else
            {
                if (sites.Count == 0)
                {
                    button10.Enabled = false;
                    button1.Enabled = false;
                    button6.Enabled = false;
                    button11.Enabled = false;
                    button3.Enabled = false;
                    button5.Enabled = false;
                }
                else
                {
                    button1.Enabled = true;
                    button6.Enabled = true;
                    button11.Enabled = true;
                    button3.Enabled = true;
                    button5.Enabled = true;
                    button10.Enabled = true;
                }
                comboBox1.DataSource = sites;
                comboBox2.DataSource = sites;

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            painter painter = new painter(((Site)comboBox1.SelectedItem).fileLocation, ((Site)comboBox1.SelectedItem).id, false,true);
            painter.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            painter painter = new painter(((Site)comboBox2.SelectedItem).fileLocation, ((Site)comboBox2.SelectedItem).id, false, false);
            painter.ShowDialog();
        }

        private void usersNewSessionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usersNewSessionComboBox.SelectedIndex == usersNewSessionComboBox.Items.Count - 1)
            {
                CreateUser cu = new CreateUser(_db);
                cu.ShowDialog();
                setUsers();

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            report rep = new report(((Site)comboBox2.SelectedItem).id);
            rep.ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var l = comboBox3.DataSource;
        }
    }
}
