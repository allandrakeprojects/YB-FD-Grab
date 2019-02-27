using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
        private bool __is_send = true;
        private int __send = 0;
        private int __total_player = 0;
        private string __brand_code = "YB";
        private string __brand_color = "#EC6506";
        private string __app = "FD Grab";
        private string __app_type = "1";
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
                        
                        SendITSupport("The application have been logout, please re-login again.");
                        SendMyBot("The application have been logout, please re-login again.");
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
            try
            {
                if (Properties.Settings.Default.______last_bill_no == "")
                {
                    ___GetLastBillNo();
                }

                label_player_last_bill_no.Text = "Last Bill No.: " + Properties.Settings.Default.______last_bill_no;
            }
            catch (Exception err)
            {
                __send++;
                if (__send == 5)
                {
                    SendMyBot(err.ToString());
                    SendITSupport("There's a problem to the server, please re-open the application.");

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    ___WaitNSeconds(10);
                    ___PlayerLastBillNo();
                }
            }
        }

        private void ___GetLastBillNo()
        {
            try
            {
                string password = __brand_code.ToString() + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["token"] = token
                    };

                    var result = wb.UploadValues("http://zeus.ssitex.com:8080/API/lastFDRecord", "POST", data);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken lbn = jo.SelectToken("$.msg");

                    Properties.Settings.Default.______last_bill_no = lbn.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ___GetLastBillNo2();
                    }
                }
            }
        }

        private void ___GetLastBillNo2()
        {
            try
            {
                string password = __brand_code + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["token"] = token
                    };

                    var result = wb.UploadValues("http://zeus2.ssitex.com:8080/API/lastFDRecord", "POST", data);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken lbn = jo.SelectToken("$.msg");

                    Properties.Settings.Default.______last_bill_no = lbn.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ___GetLastBillNo();
                    }
                }
            }
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

                byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/payment/searchDeposit?transactionId=&referenceNo=&userId=&status=9999&type=2&toBankIdOrBranch=-1&createDateStart=" + start_time + "&createDateEnd=" + end_time + "&vipLevel=-1&approvedDateStart=&approvedDateEnd=&pageNumber=1&pageSize=1000000&sortCondition=4&sortName=createTime&sortOrder=1&searchText=");
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
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
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
                        JToken pg_bill_no = __jo.SelectToken("$.aaData[" + i + "].referenceNo").ToString();
                        if (status.ToString() == "2")
                        {
                            status = "1";
                        }
                        else
                        {
                            status = "0";
                        }

                        player_info.Add(username + "*|*" + name + "*|*" + date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss") + "*|*" + vip + "*|*" + amount + "*|*" + gateway + "*|*" + status + "*|*" + bill_no + "*|*" + __playerlist_cn + "*|*" + process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss") + "*|*" + method + "*|*" + pg_bill_no);
                    }
                    else
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
                        //DateTime process_datetime_replace = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToDouble(process_datetime.ToString()) / 1000d)).ToLocalTime();
                        JToken vip = __jo.SelectToken("$.aaData[" + i + "].vipLevel").ToString();
                        JToken gateway = __jo.SelectToken("$.aaData[" + i + "].toBankName").ToString();
                        JToken method = __jo.SelectToken("$.aaData[" + i + "].toPaymentType").ToString();
                        JToken amount = __jo.SelectToken("$.aaData[" + i + "].amount").ToString().Replace(",", "");
                        JToken pg_bill_no = __jo.SelectToken("$.aaData[" + i + "].referenceNo").ToString();
                        status = "2";

                        player_info.Add(username + "*|*" + name + "*|*" + date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss") + "*|*" + vip + "*|*" + amount + "*|*" + gateway + "*|*" + status + "*|*" + bill_no + "*|*" + __playerlist_cn + "*|*" + "" + "*|*" + method + "*|*" + pg_bill_no);

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
                            string _pg_bill_no = "";

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
                                // PG Bill No
                                else if (count == 12)
                                {
                                    _pg_bill_no = value_inner;
                                }
                            }

                            // ----- Insert Data
                            using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\fdgrab_yb.txt", true, Encoding.UTF8))
                            {
                                file.WriteLine(_username + "*|*" + _name + "*|*" + _contact_no + "*|*" + _date_deposit + "*|*" + _vip + "*|*" + _amount + "*|*" + _gateway + "*|*" + _status + "*|*" + _bill_no + "*|*" + _process_datetime + "*|*" + _method + "*|*" + _pg_bill_no);
                                file.Close();
                            }
                            if (__last_username == _username)
                            {
                                Thread.Sleep(Properties.Settings.Default.______thread_mill);
                                ___InsertData(_username, _name, _date_deposit, _vip, _amount, _gateway, _status, _bill_no, _contact_no, _process_datetime, _method, _pg_bill_no);
                            }
                            else
                            {
                                ___InsertData(_username, _name, _date_deposit, _vip, _amount, _gateway, _status, _bill_no, _contact_no, _process_datetime, _method, _pg_bill_no);
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
            if (__isLogin)
            {
                timer_pending.Stop();
                char[] split = "*|*".ToCharArray();
                string[] values = Properties.Settings.Default.______pending_bill_no.Split(split);
                foreach (var value in values)
                {
                    if (value != "")
                    {
                        await ___SearchPendingAsync(value);
                    }
                }
                timer_pending.Start();
            }
        }

        private async Task ___SearchPendingAsync(string bill_no)
        {
            try
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

                byte[] result = await wc.DownloadDataTaskAsync("http://103.4.104.8/manager/payment/searchDeposit?transactionId=" + bill_no + "&referenceNo=&userId=&status=9999&type=2&toBankIdOrBranch=-1&createDateStart=" + start_time + "&createDateEnd=" + end_time + "&vipLevel=-1&approvedDateStart=&approvedDateEnd=&pageNumber=1&pageSize=10&sortCondition=4&sortName=createTime&sortOrder=1&searchText=");
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
                    JToken pg_bill_no = jo.SelectToken("$.aaData[0].referenceNo").ToString();

                    if (__last_username_pending == username.ToString())
                    {
                        Thread.Sleep(Properties.Settings.Default.______thread_mill);
                        ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString(), pg_bill_no.ToString());
                    }
                    else
                    {
                        ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString(), pg_bill_no.ToString());
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
                    JToken pg_bill_no = jo.SelectToken("$.aaData[0].referenceNo").ToString();

                    if (__last_username_pending == username.ToString())
                    {
                        Thread.Sleep(Properties.Settings.Default.______thread_mill);
                        ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString(), pg_bill_no.ToString());
                    }
                    else
                    {
                        ___InsertData(username.ToString(), name.ToString(), date_deposit_replace.ToString("yyyy-MM-dd HH:mm:ss"), vip.ToString(), amount.ToString(), gateway.ToString(), status.ToString(), bill_no, __playerlist_cn_pending, process_datetime_replace.ToString("yyyy-MM-dd HH:mm:ss"), method.ToString(), pg_bill_no.ToString());
                    }
                    __last_username_pending = username.ToString();

                    __send = 0;
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __send++;
                    if (__send == 5)
                    {
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());
                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        await ___SearchPendingAsync(bill_no);
                    }
                }
            }
        }

        private void ___InsertData(string username, string name, string date_deposit, string vip, string amount, string gateway, string status, string bill_no, string contact_no, string process_datetime, string method, string pg_bill_no)
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
                        ["trans_id"] = bill_no,
                        ["pg_trans_id"] = pg_bill_no,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssitex.com:8080/API/sendFD", "POST", data);
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
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ____InsertData2(username, name, date_deposit, vip, amount, gateway, status, bill_no, contact_no, process_datetime, method, pg_bill_no);
                    }
                }
            }
        }

        private void ____InsertData2(string username, string name, string date_deposit, string vip, string amount, string gateway, string status, string bill_no, string contact_no, string process_datetime, string method, string pg_bill_no)
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
                        ["trans_id"] = bill_no,
                        ["pg_trans_id"] = pg_bill_no,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssitex.com:8080/API/sendFD", "POST", data);
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
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ___InsertData(username, name, date_deposit, vip, amount, gateway, status, bill_no, contact_no, process_datetime, method, pg_bill_no);
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
                    __send++;
                    if (__send == 5)
                    {
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        await ___PlayerListContactNumberAsync(username, type);
                    }
                }
            }
        }

        private async void timer_TickAsync(object sender, EventArgs e)
        {
            timer.Stop();
            await ___GetPlayerListsRequest();
        }

        private void SendMyBot(string message)
        {
            try
            {
                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                string apiToken = "772918363:AAHn2ufmP3ocLEilQ1V-IHcqYMcSuFJHx5g";
                string chatId = "@allandrake";
                string text = "-----" + __brand_code + " " + __app + "-----%0A%0AIP:%20" + Properties.Settings.Default.______server_ip + "%0ALocation:%20" + Properties.Settings.Default.______server_location + "%0ADate%20and%20Time:%20[" + datetime + "]%0AMessage:%20" + message + "";
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
                    SendITSupport("There's a problem to the server, please re-open the application.");
                    SendMyBot(err.ToString());

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    ___WaitNSeconds(10);
                    SendMyBot(message);
                }
            }
        }

        private void SendITSupport(string message)
        {
            if (__is_send)
            {
                try
                {
                    string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                    string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                    string apiToken = "612187347:AAE9doWWcStpWrDrfpOod89qGSxCJ5JwQO4";
                    string chatId = "@it_support_ssi";
                    string text = "-----" + __brand_code + " " + __app + "-----%0A%0AIP:%20" + Properties.Settings.Default.______server_ip + "%0ALocation:%20" + Properties.Settings.Default.______server_location + "%0ADate%20and%20Time:%20[" + datetime + "]%0AMessage:%20" + message + "";
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
                        {
                            sb.Append(line);
                        }
                    }
                }
                catch (Exception err)
                {
                    __send++;
                    if (__send == 5)
                    {
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        SendITSupport(message);
                    }
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

        private void timer_flush_memory_Tick(object sender, EventArgs e)
        {
            ___FlushMemory();
        }

        public static void ___FlushMemory()
        {
            Process prs = Process.GetCurrentProcess();
            try
            {
                prs.MinWorkingSet = (IntPtr)(300000);
            }
            catch (Exception err)
            {
                // leave blank
            }
        }

        private void timer_detect_running_Tick(object sender, EventArgs e)
        {
            ___DetectRunning();
        }

        private void ___DetectRunning()
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string password = __brand_code + datetime + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["app_type"] = __app_type,
                        ["last_update"] = datetime,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssitex.com:8080/API/updateAppStatus", "POST", data);
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
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ___DetectRunning2();
                    }
                }
            }
        }

        private void ___DetectRunning2()
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string password = __brand_code + datetime + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["app_type"] = __app_type,
                        ["last_update"] = datetime,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssitex.com:8080/API/updateAppStatus", "POST", data);
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
                        SendITSupport("There's a problem to the server, please re-open the application.");
                        SendMyBot(err.ToString());

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___WaitNSeconds(10);
                        ___DetectRunning();
                    }
                }
            }
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (__is_send)
            {
                __is_send = false;
                MessageBox.Show("Telegram Notification is Disabled.", __brand_code + " " + __app, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                __is_send = true;
                MessageBox.Show("Telegram Notification is Enabled.", __brand_code + " " + __app, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ___WaitNSeconds(int sec)
        {
            if (sec < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(sec);
            while (DateTime.Now < _desired)
            {
                Application.DoEvents();
            }
        }
    }
}