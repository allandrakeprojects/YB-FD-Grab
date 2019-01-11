﻿using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YB_FD_Grab
{
    public partial class Main_Form : Form
    {
        private ChromiumWebBrowser chromeBrowser;
        private JObject __jo;
        private bool __isLogin = false;
        private bool __isClose;
        private bool m_aeroEnabled;
        private int __send = 0;
        private int __total_player = 0;
        private string __brand_code = "YB";
        private string __brand_color = "#EC6506";
        private string __player_last_bill_no = "";
        private string __playerlist_cn = "";
        private string __playerlist_cn_pending = "";
        private string __last_username = "";
        private string __last_username_pending = "";
        private string __url = "";
        Form __mainFormHandler;

        // Drag Header to Move
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        // ----- Drag Header to Move

        // Form Shadow
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        };
                        DwmExtendFrameIntoClientArea(Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
                m.Result = (IntPtr)HTCAPTION;
        }
        // ----- Form Shadow

        public Main_Form()
        {
            InitializeComponent();

            timer_landing.Start();
        }

        // Drag to Move
        private void panel_header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

            //Properties.Settings.Default.______last_bill_no = "";
            //Properties.Settings.Default.Save();
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_loader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_brand_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel_landing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_landing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // ----- Drag to Move

        // Click Close
        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Exit the program?", "YB FD Grab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                __isClose = true;
                Environment.Exit(0);
            }
        }

        // Click Minimize
        private void pictureBox_minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // Form Closing
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!__isClose)
            {
                DialogResult dr = MessageBox.Show("Exit the program?", "YB FD Grab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            Environment.Exit(0);
        }
        
        // Form Load
        private void Main_Form_Load(object sender, EventArgs e)
        {
            InitializeChromium();
            
            label1.Text = Properties.Settings.Default.______pending_bill_no;
            if (Properties.Settings.Default.______last_bill_no == "")
            {
                textBox_bill_no.Visible = true;
                panel_cefsharp.Enabled = false;
            }
        }

        private void textBox_bill_no_KeyDown(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox_bill_no.Text.Trim()))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DialogResult dr = MessageBox.Show("Proceed?", "YB FD Grab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        Properties.Settings.Default.______last_bill_no = textBox_bill_no.Text.Trim();
                        Properties.Settings.Default.Save();
                        textBox_bill_no.Visible = false;
                        panel_cefsharp.Enabled = true;
                    }
                }
            }
        }

        // CefSharp Initialize
        private void InitializeChromium()
        {
            CefSettings settings = new CefSettings();

            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            Cef.Initialize(settings);
            chromeBrowser = new ChromiumWebBrowser("http://103.4.104.8/page/manager/login.jsp");
            panel_cefsharp.Controls.Add(chromeBrowser);
            chromeBrowser.AddressChanged += ChromiumBrowserAddressChanged;
        }

        static int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }

        // CefSharp Address Changed
        private void ChromiumBrowserAddressChanged(object sender, AddressChangedEventArgs e)
        {
            __url = e.Address.ToString();
            if (e.Address.ToString().Equals("http://103.4.104.8/page/manager/login.jsp"))
            {
                if (__isLogin)
                {
                    Invoke(new Action(() =>
                    {
                        label_brand.Visible = false;
                        pictureBox_loader.Visible = false;
                        label_player_last_bill_no.Visible = false;
                        label_page_count.Visible = false;
                        label_currentrecord.Visible = false;
                        __mainFormHandler = Application.OpenForms[0];
                        __mainFormHandler.Size = new Size(466, 468);

                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendITSupport("The application have been logout, please re-login again.");
                        SendEmail("<html><body>Brand: <font color='" + __brand_color + "'>-----" + __brand_code + "-----</font><br/>IP: 192.168.10.252<br/>Location: Robinsons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Line Number: " + LineNumber() + "<br/>Message: <b>The application have been logout, please re-login again.</b></body></html>");
                        __send = 0;
                        timer_pending.Stop();
                    }));
                }

                __isLogin = false;
                timer.Stop();

                Invoke(new Action(() =>
                {
                    chromeBrowser.FrameLoadEnd += (sender_, args) =>
                    {
                        if (args.Frame.IsMain)
                        {
                            Invoke(new Action(() =>
                            {
                                if (!__isLogin)
                                {
                                    args.Frame.ExecuteJavaScriptAsync("document.getElementById('username').value = 'ybrainfd';");
                                    args.Frame.ExecuteJavaScriptAsync("document.getElementById('password').value = 'rain12345';");
                                    __isLogin = false;
                                    panel_cefsharp.Visible = true;
                                    label_player_last_bill_no.Text = "-";
                                    label_brand.Visible = false;
                                    pictureBox_loader.Visible = false;
                                    label_player_last_bill_no.Visible = false;
                                }
                            }));
                        }
                    };
                }));
            }

            if (e.Address.ToString().Equals("http://103.4.104.8/page/manager/member/search.jsp") || e.Address.ToString().Equals("http://103.4.104.8/page/manager/dashboard.jsp"))
            {
                Invoke(new Action(async () =>
                {
                    label_brand.Visible = true;
                    pictureBox_loader.Visible = true;
                    label_player_last_bill_no.Visible = true;
                    label_page_count.Visible = true;
                    label_currentrecord.Visible = true;
                    __mainFormHandler = Application.OpenForms[0];
                    __mainFormHandler.Size = new Size(466, 168);

                    if (!__isLogin)
                    {
                        timer_pending.Start();
                        __isLogin = true;
                        panel_cefsharp.Visible = false;
                        label_brand.Visible = true;
                        pictureBox_loader.Visible = true;
                        label_player_last_bill_no.Visible = true;
                        ___PlayerLastBillNo();
                        await ___GetPlayerListsRequest();
                    }
                }));
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;

        void ___CloseMessageBox()
        {
            IntPtr windowPtr = FindWindowByCaption(IntPtr.Zero, "JavaScript Alert - http://103.4.104.8");

            if (windowPtr == IntPtr.Zero)
            {
                return;
            }

            SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void timer_landing_Tick(object sender, EventArgs e)
        {
            panel_landing.Visible = false;
            timer_landing.Stop();
        }

        private void timer_close_message_box_Tick(object sender, EventArgs e)
        {
            ___CloseMessageBox();
        }
        
        private void ___PlayerLastBillNo()
        {
            label_player_last_bill_no.Text = "Last Bill No.: " + Properties.Settings.Default.______last_bill_no;
        }
        
        private void ___SavePlayerLastBillNo(string bill_no)
        {
            Properties.Settings.Default.______last_bill_no = bill_no.Trim();
            Properties.Settings.Default.Save();
        }

        // ----- Functions
        private async Task ___GetPlayerListsRequest()
        {
            try
            {
                string start_time = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                string end_time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");

                start_time = start_time.Replace("-", "%2F");
                start_time = start_time.Replace(" ", "+");
                start_time = start_time.Replace(":", "%3A");

                end_time = end_time.Replace("-", "%2F");
                end_time = end_time.Replace(" ", "+");
                end_time = end_time.Replace(":", "%3A");

                var cookieManager = Cef.GetGlobalCookieManager();
                var visitor = new CookieCollector();
                cookieManager.VisitUrlCookies(__url, true, visitor);
                var cookies = await visitor.Task;
                var cookie = CookieCollector.GetCookieHeader(cookies);
                WebClient wc = new WebClient();
                wc.Headers.Add("Cookie", cookie);
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/payment/searchDeposit?transactionId=&referenceNo=&userId=&status=9999&type=0&toBankIdOrBranch=-1&createDateStart=" + start_time + "&createDateEnd=" + end_time + "&vipLevel=-1&approvedDateStart=&approvedDateEnd=&pageNumber=1&pageSize=1000000&sortCondition=4&sortName=createTime&sortOrder=1&searchText=");
                string responsebody = Encoding.UTF8.GetString(result);
                var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                __jo = JObject.Parse(deserializeObject.ToString());
                JToken count = __jo.SelectToken("$.aaData");
                __total_player = count.Count();
                await ___PlayerListAsync();
                __send = 0;
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendEmail("<html><body>Brand: <font color='" + __brand_color + "'>-----" + __brand_code + "-----</font><br/>IP: 192.168.10.252<br/>Location: Robinsons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Line Number: " + LineNumber() + "<br/>Message: <b>" + err.ToString() + "</b></body></html>");
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        await ___GetPlayerListsRequest();
                    }
                }
            }
        }

        private async Task ___PlayerListAsync()
        {
            List<string> player_info = new List<string>();

            for (int i = 0; i < __total_player; i++)
            {
                JToken bill_no = __jo.SelectToken("$.aaData[" + i + "].id").ToString();
                if (bill_no.ToString().Trim() != Properties.Settings.Default.______last_bill_no)
                {
                    JToken status = __jo.SelectToken("$.aaData[" + i + "].status").ToString();
                    if (status.ToString() != "0")
                    {
                        if (i == 0)
                        {
                            __player_last_bill_no = bill_no.ToString().Trim();
                        }

                        JToken username = __jo.SelectToken("$.aaData[" + i + "].userId").ToString();
                        await ___PlayerListContactNumberAsync(username.ToString(), "normal");
                        JToken name = __jo.SelectToken("$.aaData[" + i + "].userName").ToString();
                        JToken date_deposit = __jo.SelectToken("$.aaData[" + i + "].createTime").ToString();
                        DateTime date_deposit_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(date_deposit.ToString()) / 1000d)).ToLocalTime();
                        JToken process_datetime = __jo.SelectToken("$.aaData[" + i + "].approvedTime").ToString();
                        DateTime process_datetime_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(process_datetime.ToString()) / 1000d)).ToLocalTime();
                        JToken vip = __jo.SelectToken("$.aaData[" + i + "].vipLevel").ToString();
                        JToken gateway = __jo.SelectToken("$.aaData[" + i + "].toBankName").ToString();
                        JToken method = __jo.SelectToken("$.aaData[" + i + "].toPaymentType").ToString();
                        JToken amount = __jo.SelectToken("$.aaData[" + i + "].amount").ToString().Replace(",", "");
                        if (status.ToString() == "2")
                        {
                            status = "1";
                        }
                        else
                        {
                            status = "0";
                        }

                        player_info.Add(username + "*|*" + name + "*|*" + date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss") + "*|*" + vip + "*|*" + amount + "*|*" + gateway + "*|*" + status + "*|*" + bill_no + "*|*" + __playerlist_cn + "*|*" + process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss") + "*|*" + method);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            __player_last_bill_no = bill_no.ToString().Trim();
                        }
                        
                        bool isContains = false;
                        char[] split = "*|*".ToCharArray();
                        string[] values = Properties.Settings.Default.______pending_bill_no.Split(split);
                        foreach (var value in values)
                        {
                            if (value != "")
                            {
                                if (bill_no.ToString() == value)
                                {
                                    isContains = true;
                                    break;
                                }
                                else
                                {
                                    isContains = false;
                                }
                            }
                        }

                        if (!isContains)
                        {
                            Properties.Settings.Default.______pending_bill_no += bill_no + "*|*";
                            label1.Text = Properties.Settings.Default.______pending_bill_no;
                            Properties.Settings.Default.Save();
                        }
                        else if (Properties.Settings.Default.______pending_bill_no == "")
                        {
                            Properties.Settings.Default.______pending_bill_no += bill_no + "*|*";
                            label1.Text = Properties.Settings.Default.______pending_bill_no;
                            Properties.Settings.Default.Save();
                        }
                    }
                }
                else
                {
                    // send to api
                    if (player_info.Count != 0)
                    {
                        player_info.Reverse();
                        string player_info_get = String.Join(",", player_info);
                        char[] split = ",".ToCharArray();
                        string[] values = player_info_get.Split(split);
                        foreach (string value in values)
                        {
                            Application.DoEvents();
                            string[] values_inner = value.Split(new string[] { "*|*" }, StringSplitOptions.None);
                            int count = 0;
                            string _username = "";
                            string _name = "";
                            string _date_deposit = "";
                            string _vip = "";
                            string _amount = "";
                            string _gateway = "";
                            string _status = "";
                            string _bill_no = "";
                            string _contact_no = "";
                            string _process_datetime = "";
                            string _method = "";

                            foreach (string value_inner in values_inner)
                            {
                                count++;

                                // Username
                                if (count == 1)
                                {
                                    _username = value_inner;
                                }
                                // Name
                                else if (count == 2)
                                {
                                    _name = value_inner;
                                }
                                // Deposit Date
                                else if (count == 3)
                                {
                                    _date_deposit = value_inner;
                                }
                                // VIP
                                else if (count == 4)
                                {
                                    _vip = value_inner;
                                }
                                // Amount
                                else if (count == 5)
                                {
                                    _amount = value_inner;
                                }
                                // Gateway
                                else if (count == 6)
                                {
                                    _gateway = value_inner;
                                }
                                // Status
                                else if (count == 7)
                                {
                                    _status = value_inner;
                                }
                                // Bill No
                                else if (count == 8)
                                {
                                    _bill_no = value_inner;
                                }
                                // Contact No
                                else if (count == 9)
                                {
                                    _contact_no = value_inner;
                                }
                                // Process Time
                                else if (count == 10)
                                {
                                    _process_datetime = value_inner;
                                }
                                // Method
                                else if (count == 11)
                                {
                                    _method = value_inner;
                                }
                            }

                            // ----- Insert Data
                            using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\fdgrab_yb.txt", true, Encoding.UTF8))
                            {
                                file.WriteLine(_username + "*|*" + _name + "*|*" + _contact_no + "*|*" + _date_deposit + "*|*" + _vip + "*|*" + _amount + "*|*" + _gateway + "*|*" + _status + "*|*" + _bill_no + "*|*" + _process_datetime + "*|*" + _method);
                                file.Close();
                            }
                            if (__last_username == _username)
                            {
                                Thread.Sleep(1000);
                                ___InsertData(_username, _name, _date_deposit, _vip, _amount, _gateway, _status, _bill_no, _contact_no, _process_datetime, _method);
                            }
                            else
                            {
                                ___InsertData(_username, _name, _date_deposit, _vip, _amount, _gateway, _status, _bill_no, _contact_no, _process_datetime, _method);
                            }
                            __last_username = _username;

                            __send = 0;
                        }
                    }

                    if (!String.IsNullOrEmpty(__player_last_bill_no.Trim()))
                    {
                        ___SavePlayerLastBillNo(__player_last_bill_no);

                        Invoke(new Action(() =>
                        {
                            label_player_last_bill_no.Text = "Last Bill No.: " + Properties.Settings.Default.______last_bill_no;
                        }));
                    }

                    player_info.Clear();
                    timer.Start();
                    break;
                }
            }
        }

        private async void timer_pending_TickAsync(object sender, EventArgs e)
        {
            try
            {
                if (__isLogin)
                {
                    timer_pending.Stop();
                    char[] split = "*|*".ToCharArray();
                    string[] values = Properties.Settings.Default.______pending_bill_no.Split(split);
                    foreach (var value in values)
                    {
                        if (value != "")
                        {
                            await SearchPendingAsync(value);
                        }
                    }
                    timer_pending.Start();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private async Task SearchPendingAsync(string bill_no)
        {
            string start_time = "2016-01-01 00:00:00";
            string end_time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");

            start_time = start_time.Replace("-", "%2F");
            start_time = start_time.Replace(" ", "+");
            start_time = start_time.Replace(":", "%3A");

            end_time = end_time.Replace("-", "%2F");
            end_time = end_time.Replace(" ", "+");
            end_time = end_time.Replace(":", "%3A");

            var cookieManager = Cef.GetGlobalCookieManager();
            var visitor = new CookieCollector();
            cookieManager.VisitUrlCookies(__url, true, visitor);
            var cookies = await visitor.Task;
            var cookie = CookieCollector.GetCookieHeader(cookies);
            WebClient wc = new WebClient();
            wc.Headers.Add("Cookie", cookie);
            wc.Encoding = Encoding.UTF8;
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/payment/searchDeposit?transactionId=" + bill_no + "&referenceNo=&userId=&status=9999&type=0&toBankIdOrBranch=-1&createDateStart=" + start_time + "&createDateEnd=" + end_time + "&vipLevel=-1&approvedDateStart=&approvedDateEnd=&pageNumber=1&pageSize=10&sortCondition=4&sortName=createTime&sortOrder=1&searchText=");
            string responsebody = Encoding.UTF8.GetString(result);
            var deserializeObject = JsonConvert.DeserializeObject(responsebody);
            JToken jo = JObject.Parse(deserializeObject.ToString());
            JToken status = jo.SelectToken("$.aaData[0].status");
            
            string path = Path.GetTempPath() + @"\fdgrab_yb_pending.txt";
            if (status.ToString() == "2")
            {
                Properties.Settings.Default.______pending_bill_no = Properties.Settings.Default.______pending_bill_no.Replace(bill_no + "*|*", "");
                label1.Text = Properties.Settings.Default.______pending_bill_no;
                Properties.Settings.Default.Save();

                status = "1";
                JToken username = jo.SelectToken("$.aaData[0].userId").ToString();
                await ___PlayerListContactNumberAsync(username.ToString(), "pending");
                JToken name = jo.SelectToken("$.aaData[0].userName").ToString();
                JToken date_deposit = jo.SelectToken("$.aaData[0].createTime").ToString();
                DateTime date_deposit_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(date_deposit.ToString()) / 1000d)).ToLocalTime();
                JToken process_datetime = jo.SelectToken("$.aaData[0].approvedTime").ToString();
                DateTime process_datetime_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(process_datetime.ToString()) / 1000d)).ToLocalTime();
                JToken vip = jo.SelectToken("$.aaData[0].vipLevel").ToString();
                JToken gateway = jo.SelectToken("$.aaData[0].toBankName").ToString();
                JToken method = jo.SelectToken("$.aaData[0].toPaymentType").ToString();
                JToken amount = jo.SelectToken("$.aaData[0].amount").ToString().Replace(",", "");

                if (__last_username_pending == username.ToString())
                {
                    Thread.Sleep(1000);
                    ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString());
                }
                else
                {
                    ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString());
                }
                __last_username_pending = username.ToString();

                __send = 0;
            }
            else if (status.ToString() == "-2")
            {
                Properties.Settings.Default.______pending_bill_no = Properties.Settings.Default.______pending_bill_no.Replace(bill_no + "*|*", "");
                label1.Text = Properties.Settings.Default.______pending_bill_no;
                Properties.Settings.Default.Save();

                status = "0";
                JToken username = jo.SelectToken("$.aaData[0].userId").ToString();
                await ___PlayerListContactNumberAsync(username.ToString(), "pending");
                JToken name = jo.SelectToken("$.aaData[0].userName").ToString();
                JToken date_deposit = jo.SelectToken("$.aaData[0].createTime").ToString();
                DateTime date_deposit_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(date_deposit.ToString()) / 1000d)).ToLocalTime();
                JToken process_datetime = jo.SelectToken("$.aaData[0].approvedTime").ToString();
                DateTime process_datetime_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(process_datetime.ToString()) / 1000d)).ToLocalTime();
                JToken vip = jo.SelectToken("$.aaData[0].vipLevel").ToString();
                JToken gateway = jo.SelectToken("$.aaData[0].toBankName").ToString();
                JToken method = jo.SelectToken("$.aaData[0].toPaymentType").ToString();
                JToken amount = jo.SelectToken("$.aaData[0].amount").ToString().Replace(",", "");

                if (__last_username_pending == username.ToString())
                {
                    Thread.Sleep(1000);
                    ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString());
                }
                else
                {
                    ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString());
                }
                __last_username_pending = username.ToString();

                __send = 0;
            }
        }

        private void ___InsertData(string username, string name, string date_deposit, string vip, string amount, string gateway, string status, string bill_no, string contact_no, string process_datetime, string method)
        {
            try
            {
                double amount_replace = Convert.ToDouble(amount);
                string password = __brand_code + username.ToLower() + date_deposit + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["username"] = username,
                        ["name"] = name,
                        ["date_deposit"] = date_deposit,
                        ["contact"] = contact_no,
                        ["vip"] = vip,
                        ["gateway"] = gateway,
                        ["brand_code"] = __brand_code,
                        ["amount"] = amount_replace.ToString("N0"),
                        ["success"] = status,
                        ["action_date"] = process_datetime,
                        ["method"] = method,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssimakati.com:8080/API/sendFD", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendEmail("<html><body>Brand: <font color='" + __brand_color + "'>-----" + __brand_code + "-----</font><br/>IP: 192.168.10.252<br/>Location: Robinsons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Line Number: " + LineNumber() + "<br/>Message: <b>" + err.ToString() + "</b></body></html>");
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ____InsertData2(username, name, date_deposit, vip, amount, gateway, status, bill_no, contact_no, process_datetime, method);
                    }
                }
            }
        }

        private void ____InsertData2(string username, string name, string date_deposit, string vip, string amount, string gateway, string status, string bill_no, string contact_no, string process_datetime, string method)
        {
            try
            {
                double amount_replace = Convert.ToDouble(amount);
                string password = __brand_code + username.ToLower() + date_deposit + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["username"] = username,
                        ["name"] = name,
                        ["date_deposit"] = date_deposit,
                        ["contact"] = contact_no,
                        ["vip"] = vip,
                        ["gateway"] = gateway,
                        ["brand_code"] = __brand_code,
                        ["amount"] = amount_replace.ToString("N0"),
                        ["success"] = status,
                        ["action_date"] = process_datetime,
                        ["method"] = method,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssimakati.com:8080/API/sendFD", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendEmail("<html><body>Brand: <font color='" + __brand_color + "'>-----" + __brand_code + "-----</font><br/>IP: 192.168.10.252<br/>Location: Robinsons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Line Number: " + LineNumber() + "<br/>Message: <b>" + err.ToString() + "</b></body></html>");
                        __send = 0;

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___InsertData(username, name, date_deposit, vip, amount, gateway, status, bill_no, contact_no, process_datetime, method);
                    }
                }
            }
        }

        private async Task ___PlayerListContactNumberAsync(string username, string type)
        {
            try
            {
                if (type == "normal")
                {
                    var cookieManager = Cef.GetGlobalCookieManager();
                    var visitor = new CookieCollector();
                    cookieManager.VisitUrlCookies(__url, true, visitor);
                    var cookies = await visitor.Task;
                    var cookie = CookieCollector.GetCookieHeader(cookies);
                    WebClient wc = new WebClient();
                    wc.Headers.Add("Cookie", cookie);
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/member/getProfileOverview?userId=" + username);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo_deposit = JObject.Parse(deserializeObject.ToString());
                    JToken _phone_number = jo_deposit.SelectToken("$.phoneNumber").ToString();

                    __playerlist_cn = _phone_number.ToString();
                }
                else
                {
                    var cookieManager = Cef.GetGlobalCookieManager();
                    var visitor = new CookieCollector();
                    cookieManager.VisitUrlCookies(__url, true, visitor);
                    var cookies = await visitor.Task;
                    var cookie = CookieCollector.GetCookieHeader(cookies);
                    WebClient wc = new WebClient();
                    wc.Headers.Add("Cookie", cookie);
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/member/getProfileOverview?userId=" + username);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo_deposit = JObject.Parse(deserializeObject.ToString());
                    JToken _phone_number = jo_deposit.SelectToken("$.phoneNumber").ToString();

                    __playerlist_cn_pending = _phone_number.ToString();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    await ___PlayerListContactNumberAsync(username, type);
                }
            }
        }

        private async void timer_TickAsync(object sender, EventArgs e)
        {
            timer.Stop();
            await ___GetPlayerListsRequest();
        }

        private void SendITSupport(string message)
        {
            try
            {
                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                string apiToken = "798422517:AAGxMBvataWOid8SRDMid0nkTv0q0l64-Qs";
                string chatId = "@fd_grab_it_support";
                string text = "Brand:%20-----" + __brand_code + "-----%0AIP:%20192.168.10.252%0ALocation:%20Robinsons%20Summit%20Office%0ADate%20and%20Time:%20[" + datetime + "]%0AMessage:%20" + message + "";
                urlString = String.Format(urlString, apiToken, chatId, text);
                WebRequest request = WebRequest.Create(urlString);
                Stream rs = request.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(rs);
                string line = "";
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                        sb.Append(line);
                }
            }
            catch (Exception err)
            {
                __send++;
                if (__send == 5)
                {
                    SendITSupport(message);
                }
                else
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private void SendEmail(string get_message)
        {
            try
            {
                int port = 587;
                string host = "smtp.gmail.com";
                string username = "drake@18tech.com";
                string password = "@ccess123418tech";
                string mailFrom = "noreply@mail.com";
                string mailTo = "drake@18tech.com";
                string mailTitle = __brand_code + " FD Grab";
                string mailMessage = get_message;

                using (SmtpClient client = new SmtpClient())
                {
                    MailAddress from = new MailAddress(mailFrom);
                    MailMessage message = new MailMessage
                    {
                        From = from
                    };
                    message.To.Add(mailTo);
                    message.Subject = mailTitle;
                    message.Body = mailMessage;
                    message.IsBodyHtml = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = host;
                    client.Port = port;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential
                    {
                        UserName = username,
                        Password = password
                    };
                    client.Send(message);
                }
            }
            catch (Exception err)
            {
                __send++;
                if (__send == 5)
                {
                    SendEmail(get_message);
                }
                else
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private void label_player_last_bill_no_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label_player_last_bill_no_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(label_player_last_bill_no.Text.Replace("Last Bill No.: ", "").Trim());
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            label1.Visible = true;
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            label1.Visible = false;
        }
    }
}