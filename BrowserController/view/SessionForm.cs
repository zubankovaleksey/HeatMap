using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tobii.Interaction;
using WebsitesScreenshot.SupportClasses;

namespace BrowserController.view
{
    public partial class SessionForm : Form
    {
        public delegate void SetEyePointDeligate(int x, int y);

        private Host host;
        private GazePointDataStream gazePointDataStream;
        public bool isConnected = false;

        public List<List<Point>> cursorTraectory = new List<List<Point>>();
        public List<List<Point>> eyeTraectory = new List<List<Point>>();
        public List<List<Point>> clickTraectory = new List<List<Point>>();
        public List<String> urls = new List<String>();
        public List<Int64> showingTime = new List<long>();
        public string url;
        public bool isClose = false;
        public bool isNavigated = false;
        System.Windows.Forms.HtmlElement htmlDoc;
        List<int> Ys = new List<int>();
        List<int> Ysc = new List<int>();
        int idx = -1;


        Point? getPointInSite(Point pointInBrowser) {
            if (webBrowser1.Document.Body == null)
            {
                return null;
            }
            var res = new Point(pointInBrowser.X + webBrowser1.Document.Body.ScrollRectangle.X, pointInBrowser.Y + htmlDoc.ScrollTop);
            Ys.Add(pointInBrowser.Y);
            Ysc.Add(htmlDoc.ScrollTop);
            return res;
        }
        
        public SessionForm(int timeToDrop, string url)
        {

            host = new Host();
            this.url = url;
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form_Closing);
            webBrowser1.ScriptErrorsSuppressed = true;
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer3.Interval = timeToDrop;
        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            isClose = true;
        }

        void w_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            htmlDoc = this.webBrowser1.Document.GetElementsByTagName("HTML")[0];
            System.Windows.Forms.HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            System.Windows.Forms.HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = "function deleteTargetBlank(){ var links = document.links, i, length; for (i = 0, length = links.length; i < length; i++) {links[i].target == '_blank' && links[i].removeAttribute('target');}}";
            head.AppendChild(scriptEl);
            webBrowser1.Document.InvokeScript("deleteTargetBlank");
            if (isNavigated == false)
            {
                timer1.Start();
                timer2.Start();
                timer3.Start();
                gazePointDataStream = host.Streams.CreateGazePointDataStream();
                host.EnableConnection();
                gazePointDataStream.GazePoint(RecordGazePointToList);
            }
            isNavigated = true;
            initNewFrame();
        }

        private void SessionForm_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(w_DocumentCompleted);
        }

        private void setPoint(int x, int y) {
            if (!isNavigated)
                return;
            
            Point point = new Point((int)x, (int)y);
            if (isPositionInBrowser(point))
            {
                Point? tmp = getPointInSite(webBrowser1.PointToClient(point));
                if (tmp != null)
                {
                    if(idx==-1)
                        eyeTraectory.Last().Add((Point)tmp);
                    else
                        eyeTraectory[idx].Add((Point)tmp);

                }
            }
        }

        private void RecordGazePointToList(double x, double y, double ts)
        {
            isConnected = true;
            /*if (label1.Visible)
                label1.Visible = false;*/
            BeginInvoke(new SetEyePointDeligate(setPoint), (int)x, (int)y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isPositionInBrowser(Cursor.Position) && isPositionChanged(Cursor.Position))
            {
                Point? tmp = getPointInSite(webBrowser1.PointToClient(Cursor.Position));
                if (tmp != null)
                {
                    if(idx==-1)
                        cursorTraectory.Last().Add((Point)tmp);
                    else
                        cursorTraectory[idx].Add((Point)tmp);

                }
            }
        }

        private bool isPositionInBrowser(Point currentPos) {
            var a = webBrowser1.PointToClient(currentPos);

            if (a.X < 0 || a.Y < 0 || a.X > webBrowser1.Width || a.Y > webBrowser1.Height) {
                return false;
            }

            return true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!isNavigated)
                return;
            showingTime[showingTime.Count - 1] += timer2.Interval;

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            host.DisableConnection();
            this.Close();
            isClose = false;

        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            
        }
        
        private bool isPositionChanged(Point currentPos)
        {
            if (cursorTraectory.Last().Count == 0)
            {
                return true;
            }

            var a = cursorTraectory.Last().Last();

            return Math.Abs(a.X - currentPos.X) > 3 && Math.Abs(a.Y - currentPos.Y) > 3;
        }

        private void initNewFrame() {
            idx = urls.IndexOf(webBrowser1.Url.ToString());
            if ((urls.Count == 0 || webBrowser1.Url.ToString() != urls.Last()) && idx == -1)
            {
                showingTime.Add(0);

                cursorTraectory.Add(new List<Point>());
                eyeTraectory.Add(new List<Point>());
                clickTraectory.Add(new List<Point>());
                urls.Add(/*webBrowser1.StatusText*/webBrowser1.Url.ToString());
            }
        }
    }
}
