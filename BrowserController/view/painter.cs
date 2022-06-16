using BrowserController.DB;
using BrowserController.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebsitesScreenshot.SupportClasses;

namespace BrowserController.view
{
    public partial class painter : Form
    {
        WebDB _db = new WebDB();
        int id;
        bool isEdit;
        bool isNew;
        bool isFind;
        bool isDoc;
        string url;
        public bool siteIsExist = true;
        List<PageDto> pageDtos;
        public object JsonSerializer { get; private set; }

        public painter(string url, int id, bool isNew, bool isEdit)
        {
            
            InitializeComponent();
            button2.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            this.id = _db.getIdSiteByName(url);
            id = this.id;
            this.isEdit = isEdit;
            this.isDoc = false;
            this.url = url;
            if (this.id != 0)
            {
                isNew = false;
            }
            this.isNew = isNew;

            pageDtos = new List<PageDto>();
            webBrowser1.ScriptErrorsSuppressed = true;

            if (isNew)
            {
                pages.Visible = false;
            }
            else if (!isEdit)
            {
                pages.Visible = true;
                pageDtos = _db.getAllOnSite(id);
                pages.DataSource = pageDtos;
                pages.DisplayMember = "name";
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                comboBox1.Visible = false;
                numericUpDown1.Visible = true;
                numericUpDown2.Visible = true;
                numericUpDown3.Visible = true; 
                numericUpDown1.Enabled = false; 
                numericUpDown2.Enabled = false; 
                numericUpDown3.Enabled = false; 
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
            }
            else if (!isNew)
            {
                pages.Visible = true;
                pageDtos = _db.getAllOnSite(id);
                pages.DataSource = pageDtos;
                pages.DisplayMember = "name";
            }

            webBrowser1.Navigate(url);
            webBrowser1.Navigating += new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(w_DocumentCompleted);

        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            isDoc = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void painter_Load(object sender, System.EventArgs e)
        {
            int k = 0;
        }

        void w_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.DocumentTitle == "Переход отменен")
            {
                siteIsExist = false;
                this.Close();
                return;
            }
            if (!isDoc)
            {
                HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0]; 
                HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                element.text = "var t=0; var first=''; var second=''; var third=''; var x=0; var colorRect=0; var y=0; function addCan(){ var scrollHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight,document.body.offsetHeight, document.documentElement.offsetHeight,document.body.clientHeight, document.documentElement.clientHeight); var b = document.body; var newDiv = document.createElement('canvas'); newDiv.height = scrollHeight; newDiv.width=window.innerWidth; newDiv.setAttribute('id','can');newDiv.style.cssText='position:absolute; z-index:5000;';b.insertBefore(newDiv,b.firstChild);newDiv.onclick = function(e){if(t===0){ x=window.pageXOffset+e.clientX; y=window.pageYOffset+e.clientY; t=1;} else{ t=0; var c=document.getElementById('can'); var ctx = c.getContext('2d'); var tmpCoord=x;tmpCoord+= ' ' + y;tmpCoord+= ' ' + (pageXOffset+e.clientX);tmpCoord+= ' ' + (window.pageYOffset+e.clientY); if(colorRect===0){first+= ' ' + tmpCoord; ctx.strokeStyle = '#FF0000';} if(colorRect===1){second+= ' ' + tmpCoord;ctx.strokeStyle = '#FFFF00'; }if(colorRect===2){third+= ' ' + tmpCoord;ctx.strokeStyle = '#008000';} ctx.strokeRect(x,y,window.pageXOffset+e.clientX-x,window.pageYOffset+e.clientY-y);}}}function changeColor(color){colorRect=color;} function saveLists(){return first + ':' + second + ':' + third;} function setRect(firstIn,secondIn,thirdIn){firstIn=firstIn.split(' '); secondIn=secondIn.split(' '); thirdIn=thirdIn.split(' '); var c=document.getElementById('can'); var ctx = c.getContext('2d'); for (var i = 1; i < firstIn.length ; i+=4){ctx.strokeStyle = '#FF0000';ctx.strokeRect(firstIn[i],firstIn[i+1],firstIn[i+2]-firstIn[i],firstIn[i+3]-firstIn[i+1]); first+=' ' + firstIn[i] + ' ' + firstIn[i+1] +' ' + firstIn[i+2] +' ' + firstIn[i+3];} for (var i = 1; i < secondIn.length ; i+=4){ctx.strokeStyle = '#FFFF00';ctx.strokeRect(secondIn[i],secondIn[i+1],secondIn[i+2]-secondIn[i],secondIn[i+3]-secondIn[i+1]); second+=' ' + secondIn[i] + ' ' + secondIn[i+1] +' ' + secondIn[i+2] +' ' + secondIn[i+3];}for (var i = 1; i < thirdIn.length ; i+=4){ctx.strokeStyle = '#008000';ctx.strokeRect(thirdIn[i],thirdIn[i+1],thirdIn[i+2]-thirdIn[i],thirdIn[i+3]-thirdIn[i+1]); third+=' ' + thirdIn[i] + ' ' + thirdIn[i+1] +' ' + thirdIn[i+2] +' ' + thirdIn[i+3];}} function clear(){ first=''; second=''; third=''; var c=document.getElementById('can'); var context = c.getContext('2d'); context.clearRect(0, 0, c.width, c.height);} function deleteTargetBlank(){ var links = document.links, i, length; for (i = 0, length = links.length; i < length; i++) {links[i].target == '_blank' && links[i].removeAttribute('target');}}";
                head.AppendChild(scriptEl);
                webBrowser1.Document.InvokeScript("deleteTargetBlank");
                PageDto currentPage = (PageDto)pages.SelectedItem;
                if (currentPage!=null && currentPage.priority != null && currentPage.priority.Count==3)
                {
                    if (currentPage.priority[0] != null && currentPage.priority[0].Count > 0)
                    {
                        numericUpDown1.Value = Decimal.Parse(currentPage.priority[0][0]);
                    }
                    if (currentPage.priority[1] != null && currentPage.priority[1].Count > 0)
                    {
                        numericUpDown2.Value = Decimal.Parse(currentPage.priority[1][0]);
                    }
                    if (currentPage.priority[2] != null && currentPage.priority[2].Count > 0)
                    {
                        numericUpDown3.Value = Decimal.Parse(currentPage.priority[2][0]);
                    }
                }
                button2.Enabled = true;
                button3.Enabled = false;
                if (isNew && this.id == 0)
                {
                    Site site = new Site();
                    site.fileLocation = this.url;
                    site.name = this.url;
                    _db.CreateSite(site);
                    this.id = _db.getIdSiteByName(this.url);
                }
                if (!isEdit)
                {
                    // добавляем только что созданый элемент в дерево DOM
                    webBrowser1.Document.InvokeScript("addCan");
                    PageDto pd = _db.getPage(((PageDto)pages.SelectedItem).id);
                    if (pd.priority != null)
                    {
                        object[] number = new object[3];
                        string s = "";
                        for (int i = 1; i < pd.priority[0].Count; i++)
                            s += ' ' + pd.priority[0][i].ToString();
                        number[0] = s;
                        s = "";
                        for (int i = 1; i < pd.priority[1].Count; i++)
                            s += ' ' + pd.priority[1][i].ToString();
                        number[1] = s;
                        s = "";
                        for (int i = 1; i < pd.priority[2].Count; i++)
                            s += ' ' + pd.priority[2][i].ToString();
                        number[2] = s;
                        webBrowser1.Document.InvokeScript("setRect", number);
                    }
                }
                isDoc = true;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // добавляем только что созданый элемент в дерево DOM
            object[] number = new object[1];
            number[0] = comboBox1.SelectedIndex;

            object w = webBrowser1.Document.InvokeScript("changeColor", number);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object w = webBrowser1.Document.InvokeScript("saveLists");
            string s = (string)w;
            List<string> priority = s.Split(':').ToList();
            List<string> first = priority[0].Split(' ').ToList();
            List<string> second = priority[1].Split(' ').ToList();
            List<string> third = priority[2].Split(' ').ToList();
            List<List<string>> all = new List<List<string>>();
            first[0] = numericUpDown1.Value.ToString();
            second[0] = numericUpDown2.Value.ToString();
            third[0] = numericUpDown3.Value.ToString();
            all.Add(first);
            all.Add(second);
            all.Add(third);
            PageDto page = new PageDto();
            page.name = webBrowser1.Url.ToString();
            page.siteId = this.id;
            page.isStartPage = true;//переделать
            page.url= webBrowser1.Url.ToString();
            page.priority = all;
            if (isNew)
            {
                _db.CreatePage(page);
                isNew = false;
                pages.Visible = true;
            }
            else
            {
                if (webBrowser1.Url.ToString() == ((PageDto)pages.SelectedItem).name)
                {
                    page.id = ((PageDto)pages.SelectedItem).id;
                    _db.updatePage(page);
                }
                else
                    _db.CreatePage(page);
            }
            pageDtos = _db.getAllOnSite(id);
            pages.DataSource = pageDtos;
            pages.DisplayMember = "name";
            pages.SelectedIndex = pageDtos.Count - 1;
            button1.Enabled = false;
            button3.Enabled = false;
            comboBox1.SelectedIndex = 0;
            isDoc = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                /*element.text = "var scrollHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight,document.body.offsetHeight, document.documentElement.offsetHeight,document.body.clientHeight, document.documentElement.clientHeight);" +
                "var t=0;" +
                "var first='';" +
                "var second='';" +
                "var third='';" +
                "var x=0;" +
                "var colorRect=0;" +
                "var y=0;" +
                "function addCan() {" +
                    "var b = document.body;" +
                    "var newDiv = document.createElement('canvas');" +
                    "newDiv.height = scrollHeight;" +
                    "newDiv.width=window.innerWidth;" +
                    "newDiv.setAttribute('id','can');" +
                    "newDiv.style.cssText='position:absolute; z-index:3000;';" +
                    "b.insertBefore(newDiv,b.firstChild);" +
                    "newDiv.onclick = function(e){" +
                        "if(t===0){ " +
                            "x=window.pageXOffset+e.clientX; " +
                            "y=window.pageYOffset+e.clientY; " +
                            "t=1;" +
                        "} " +
                        "else{ " +
                            "t=0; " +
                            "var c=document.getElementById('can');" +
                            "var ctx = c.getContext('2d'); " +
                            "var tmpCoord=x;" +
                            "tmpCoord+= ' ' + y;" +
                            "tmpCoord+= ' ' + (pageXOffset+e.clientX);" +
                            "tmpCoord+= ' ' + (window.pageYOffset+e.clientY); " +
                            "if(colorRect===0){" +
                                "first+= ' ' + tmpCoord; " +
                                "ctx.strokeStyle = '#FF0000';" +
                            "} " +
                            "if(colorRect===1){" +
                                "second+= ' ' + tmpCoord;" +
                                "ctx.strokeStyle = '#FFFF00'; " +
                            "}" +
                            "if(colorRect===2)" +
                            "{" +
                                "third+= ' ' + tmpCoord;" +
                                "ctx.strokeStyle = '#008000';" +
                            "} " +
                            "ctx.strokeRect(x,y,window.pageXOffset+e.clientX-x,window.pageYOffset+e.clientY-y);" +
                       "}" +
                   "}" +
               "} "+
               "function changeColor(color){" +
                    "colorRect=color;" +
               "} " +
               "function saveLists(){" +
                    "return first + ':' + second + ':' + third;" +
               "} " +
               "function setRect(firstIn,secondIn,thirdIn){" +
                    "firstIn=firstIn.split(' '); " +
                    "secondIn=secondIn.split(' '); " +
                    "thirdIn=thirdIn.split(' '); " +
                    "var c=document.getElementById('can'); " +
                    "var ctx = c.getContext('2d'); " +
                    "for (let i = 1; i < firstIn.length ; i+=4){" +
                        "ctx.strokeStyle = '#FF0000';" +
                        "ctx.strokeRect(firstIn[i],firstIn[i+1],firstIn[i+2]-firstIn[i],firstIn[i+3]-firstIn[i+1]); " +
                        "first+=' ' + firstIn[i] + ' ' + firstIn[i+1] +' ' + firstIn[i+2] +' ' + firstIn[i+3];" +
                    "} " +
                    "for (let i = 1; i < secondIn.length ; i+=4){" +
                        "ctx.strokeStyle = '#FFFF00';" +
                        "ctx.strokeRect(secondIn[i],secondIn[i+1],secondIn[i+2]-secondIn[i],secondIn[i+3]-secondIn[i+1]); " +
                        "second+=' ' + secondIn[i] + ' ' + secondIn[i+1] +' ' + secondIn[i+2] +' ' + secondIn[i+3];" +
                    "}" +
                    "for (let i = 1; i < thirdIn.length ; i+=4){" +
                        "ctx.strokeStyle = '#008000';" +
                        "ctx.strokeRect(thirdIn[i],thirdIn[i+1],thirdIn[i+2]-thirdIn[i],thirdIn[i+3]-thirdIn[i+1]); " +
                        "third+=' ' + thirdIn[i] + ' ' + thirdIn[i+1] +' ' + thirdIn[i+2] +' ' + thirdIn[i+3];" +
                    "}" +
                "} " +
                "function clear(){ " +
                    "first=''; " +
                    "second=''; " +
                    "third=''; " +
                    "var c=document.getElementById('can'); " +
                    "const context = c.getContext('2d'); " +
                    "context.clearRect(0, 0, c.width, c.height);" +
                "}"*/;
            webBrowser1.Document.InvokeScript("addCan");
            if (!this.isNew)
            {
                PageDto pd = new PageDto();
                pd.priority = new List<List<string>>();
                if (((PageDto)pages.SelectedItem).name == webBrowser1.Url.ToString())
                    pd = _db.getPage(((PageDto)pages.SelectedItem).id);
                else
                {
                    string url = webBrowser1.Url.ToString();
                    isFind = false;
                    for (int i = 0; i < pageDtos.Count && !isFind; i++)
                    {
                        if (pageDtos[i].name == url)
                        {
                            isFind = true;
                            pd = pageDtos[i];
                            pages.SelectedIndex = i;
                        }
                    }
                }
                object[] number = new object[3];
                string s = "";
                if (pd.priority.Count != 0)
                {
                    for (int i = 1; i < pd.priority[0].Count; i++)
                        s += ' ' + pd.priority[0][i].ToString();
                    number[0] = s;
                    s = "";
                    for (int i = 1; i < pd.priority[1].Count; i++)
                        s += ' ' + pd.priority[1][i].ToString();
                    number[1] = s;
                    s = "";
                    for (int i = 1; i < pd.priority[2].Count; i++)
                        s += ' ' + pd.priority[2][i].ToString();
                    number[2] = s;
                    webBrowser1.Document.InvokeScript("setRect", number);//var c=document.getElementById('can'); var ctx = c.getContext('2d'); firstIn=firstIn.split(' '); for (let i = 1; i < firstIn.length ; i+=4){ctx.strokeStyle = '#FF0000';ctx.strokeRect(firstIn[i],firstIn[i+1],firstIn[i+2],firstIn[i+3]); first+=' ' + firstIn[i] + ' ' + firstIn[i+1] +' ' + firstIn[i+2] +' ' + firstIn[i+3];}
                }
            }
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("clear");//var c=document.getElementById('can'); var ctx = c.getContext('2d'); firstIn=firstIn.split(' '); for (let i = 1; i < firstIn.length ; i+=4){ctx.strokeStyle = '#FF0000';ctx.strokeRect(firstIn[i],firstIn[i+1],firstIn[i+2],firstIn[i+3]); first+=' ' + firstIn[i] + ' ' + firstIn[i+1] +' ' + firstIn[i+2] +' ' + firstIn[i+3];}
        }

        private void pages_SelectedIndexChanged(object sender, EventArgs e)
        {
            isDoc = false;
            webBrowser1.Navigate(((PageDto)pages.SelectedItem).name);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 100 - numericUpDown2.Value - numericUpDown3.Value)
            {
                numericUpDown1.Value = 100 - numericUpDown2.Value - numericUpDown3.Value;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > 100 - numericUpDown1.Value - numericUpDown3.Value)
            {
                numericUpDown2.Value = 100 - numericUpDown1.Value - numericUpDown3.Value;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value > 100 - numericUpDown2.Value - numericUpDown1.Value)
            {
                numericUpDown3.Value = 100 - numericUpDown2.Value - numericUpDown1.Value;
            }
        }
    }
}
