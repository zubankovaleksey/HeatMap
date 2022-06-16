using BrowserController.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WebsitesScreenshot.SupportClasses;

namespace BrowserController.view
{
    public partial class HeatMapWindow : Form
    {
        Bitmap bitmap;
        int sizeX;
        int sizeY;
        WebDB _db;
        int _sessionID;
        int _siteID;
        string _fileputh;
        float k;
        string url="";
        PictureBox pb = new PictureBox();


        List<Pen> pens = new List<Pen>() { Pens.Black, Pens.Brown, Pens.DarkBlue, Pens.Firebrick};

        List<Point> heatMapPoints;
        List<List<Point>> trs;
        private int lastX = 0;
        private int lastY = 0;

        public HeatMapWindow(int sizeX, int sizeY, WebDB db, int sessionID, int siteID, string fileputh)
        {
            _db = db;
            List<PageDto> lp = _db.getAllOnSite(siteID);
            fileputh = lp[0].name;
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            _sessionID = sessionID;
            _fileputh = fileputh;
            _siteID = siteID;
            comboBox1.DataSource = lp;
            comboBox1.DisplayMember="name";
            comboBox1.SelectedIndex = 0;
        }

        

        private async System.Threading.Tasks.Task getPage() {
            var pageId = ((PageDto)comboBox1.SelectedItem).id;
            
            var a = await _db.getAllBySessionIdAndPageId(_sessionID, pageId);
            heatMapPoints = new List<Point>();
            trs = new List<List<Point>>();

            foreach (var ass in a) {
                var b = await _db.getTraectories((int)ass.id);
                if (!radioButton1.Checked)
                {
                    trs.Add(b[0].points);
                    heatMapPoints.AddRange(b[0].points);
                }
                else {
                    trs.Add(b[1].points);
                    heatMapPoints.AddRange(b[1].points);
                }
            }
        }
        
        private void drawHeatMap2(float k)
        {//где-то тут надо поставить отсечки по размерам
         //где-то тут надо поставить отсечки по размерам
                List<List<int>> pointCounts = new List<List<int>>();
            int STEP = Settings.sqrSize;

            for (int i = 0; i < sizeY; i += STEP) {
                pointCounts.Add(new List<int>());
                for (int j = 0; j < sizeX; j += STEP) {
                    pointCounts.Last().Add(heatMapPoints.Where(c => c.X > j  && c.X < j + STEP &&
                    c.Y > i  && c.Y < i + STEP).Count());
                }
            }
            int hiBottom;
            if (Settings.isAbsolutCounting)
            {
                /*hiBottom = 0;
                List<PageDto> tmpPages = _db.getAllOnSite(_siteID);
                for (int i = 0; i < tmpPages.Count; i++)
                {
                    var tmp = _db.getAllBySessionAndPage(_sessionID, tmpPages[i].id);
                    if (tmp.Count!=0)
                    {
                        long frame = _db.getAllBySessionAndPage(_sessionID, tmpPages[i].id)[0].id;
                        hiBottom += _db.getTraectoriesFromFrame(frame)[1].points.Count;
                    }
                }*/
                hiBottom = heatMapPoints.Count;
            }
            else {
                hiBottom = 0;
                for (int i = 0; i < pointCounts.Count; i++) {
                    for (int j = 0; j < pointCounts[i].Count; j++)
                    {
                        if (pointCounts[i][j] > hiBottom) { hiBottom = pointCounts[i][j]; }
                    }
                }
            }
            List<List<string>>prs = ((PageDto)comboBox1.SelectedItem).priority;
            double firstCount = 0;
            double secondCount = 0;
            double thirdCount = 0;
            using (var g = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < pointCounts.Count; i++) {
                    for (int j = 0; j < pointCounts[i].Count; j++) {
                        Brush c = Brushes.Green;
                        if ((double)pointCounts[i][j] / (double)hiBottom > Settings.yellowBottom) { c = Brushes.Yellow; }

                        if ((double)pointCounts[i][j] / (double)hiBottom > Settings.orangeBottom) { c = Brushes.Orange; }

                        if ((double)pointCounts[i][j] / (double)hiBottom > Settings.redBottom) { c = Brushes.Red; }

                        g.FillRectangle(c, j * STEP, i * STEP, STEP, STEP);
                        if (prs != null && prs.Count != 0)
                        {
                            for (int f = 1; f < prs[0].Count; f += 4)
                            {
                                if (j * STEP > Convert.ToInt32(prs[0][f]) && j * STEP < Convert.ToInt32(prs[0][f + 2]) && i * STEP > Convert.ToInt32(prs[0][f + 1]) && i * STEP < Convert.ToInt32(prs[0][f + 3]) ||
                                    j * STEP > Convert.ToInt32(prs[0][f]) && j * STEP < Convert.ToInt32(prs[0][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[0][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[0][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[0][f]) && j * STEP + STEP < Convert.ToInt32(prs[0][f + 2]) && i * STEP > Convert.ToInt32(prs[0][f + 1]) && i * STEP < Convert.ToInt32(prs[0][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[0][f]) && j * STEP + STEP < Convert.ToInt32(prs[0][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[0][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[0][f + 3]))
                                {
                                    firstCount += (double)pointCounts[i][j] / (double)hiBottom;
                                }
                            }
                            for (int f = 1; f < prs[1].Count; f += 4)
                            {
                                if (j * STEP > Convert.ToInt32(prs[1][f]) && j * STEP < Convert.ToInt32(prs[1][f + 2]) && i * STEP > Convert.ToInt32(prs[1][f + 1]) && i * STEP < Convert.ToInt32(prs[1][f + 3]) ||
                                    j * STEP > Convert.ToInt32(prs[1][f]) && j * STEP < Convert.ToInt32(prs[1][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[1][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[1][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[1][f]) && j * STEP + STEP < Convert.ToInt32(prs[1][f + 2]) && i * STEP > Convert.ToInt32(prs[1][f + 1]) && i * STEP < Convert.ToInt32(prs[1][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[1][f]) && j * STEP + STEP < Convert.ToInt32(prs[1][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[1][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[1][f + 3]))
                                {
                                    secondCount += (double)pointCounts[i][j] / (double)hiBottom;
                                }
                            }
                            for (int f = 1; f < prs[2].Count; f += 4)
                            {
                                if (j * STEP > Convert.ToInt32(prs[2][f]) && j * STEP < Convert.ToInt32(prs[2][f + 2]) && i * STEP > Convert.ToInt32(prs[2][f + 1]) && i * STEP < Convert.ToInt32(prs[2][f + 3]) ||
                                    j * STEP > Convert.ToInt32(prs[2][f]) && j * STEP < Convert.ToInt32(prs[2][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[2][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[2][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[2][f]) && j * STEP + STEP < Convert.ToInt32(prs[2][f + 2]) && i * STEP > Convert.ToInt32(prs[2][f + 1]) && i * STEP < Convert.ToInt32(prs[2][f + 3]) ||
                                    j * STEP + STEP > Convert.ToInt32(prs[2][f]) && j * STEP + STEP < Convert.ToInt32(prs[2][f + 2]) && i * STEP + STEP > Convert.ToInt32(prs[2][f + 1]) && i * STEP + STEP < Convert.ToInt32(prs[2][f + 3]))
                                {
                                    thirdCount += (double)pointCounts[i][j] / (double)hiBottom;
                                }
                            }
                        }
                    }
                }
                int curPen = 0;
                foreach (var tr in trs) {
                    for (int i = 0; i < tr.Count - 1; i++)
                    {
                        g.DrawLine(pens[curPen], tr[i], tr[i + 1]);
                    }
                    curPen++;
                    if (curPen == pens.Count) {
                        curPen = 0;
                    }
                }                
            }
            pb = new PictureBox();
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Add(pb);
            pb.Image = bitmap;
            string firstRes = firstCount == 0 || Double.IsNaN(firstCount) ? "0" : Math.Round(firstCount * 100, 2).ToString();
            string firstWait = prs != null && prs[0] != null ? prs[0][0] : "0";
            firstP.Text = firstRes + "/"+ firstWait + "%";
            string secondRes = secondCount == 0 || Double.IsNaN(secondCount) ? "0" : Math.Round(secondCount * 100, 2).ToString();
            string secondWait = prs != null && prs[1] != null ? prs[1][0] : "0";
            secondP.Text = secondRes + "/" + secondWait + "%";
            string thirdRes = thirdCount == 0 || Double.IsNaN(thirdCount) ? "0" : Math.Round(thirdCount * 100, 2).ToString();
            string thirdWait = prs != null && prs[2] != null ? prs[2][0] : "0";
            thirdP.Text = thirdRes + "/" + thirdWait + "%";
        }

        private async void HeatMapWindow_LoadAsync(object sender, EventArgs e)
        {
            webBrowser1.Navigate(_fileputh);
            timer1.Start();
        }

        private async void webBrowser1_NavigatedAsync(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (webBrowser1.Url.ToString() != url)
            {
                url = webBrowser1.Url.ToString();
                await repaintMap();
            }
        }

        private async void radioButton2_CheckedChangedAsync(object sender, EventArgs e)
        {
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }
            await repaintMap();
        }

        public async System.Threading.Tasks.Task repaintMap() {
            await getPage();
            int max = 0;
            if(trs.Count!=0)
                foreach (Point m in trs[0])
                {
                   if (m.Y > max)
                      max = m.Y;
                }
            if (sizeY < max)
                sizeY = max;
            bitmap = new Bitmap(sizeX, sizeY);
            drawHeatMap2(k);
        }

        private async void timer1_TickAsync(object sender, EventArgs e)//таймер проверки скрола
        {
            if (webBrowser1.Document==null || webBrowser1.Document.Body==null)
                return;
            var rect = webBrowser1.Document.Body.ScrollRectangle;
            if (lastX != rect.X || lastY != rect.Y)
            {
                await repaintMap();
                lastX = rect.X;
                lastY = rect.Y;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }
            _fileputh = ((PageDto)comboBox1.SelectedItem).name;
            webBrowser1.Navigate(_fileputh);
            timer1.Start();
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
