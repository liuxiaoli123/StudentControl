using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Net;

namespace TeacherClient
{
    public partial class MainForm : Form
    {
        [DllImport("User32.dll")]
        public static extern void SetForegroundWindow(IntPtr hwnd);//前置窗体

        [DllImport("Kernel32.dll")]
        private extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);//设置本地系统时间

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {//系统时间结构体，API规定
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        private Thread showSp; //显示启动画面的线程
        private Image bgImage;//主窗体背景图像
        private MDIChild_Login childLogin;//login子窗体
        private MDIChild_ImportOP childImportOp;//导入操作题子窗口
        private MDIChild_Monitor childMonitor;//考场监视子窗口

        /// <summary>
        /// 此函数设置MDIClient区域为双缓冲，效果极佳
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is MdiClient)
            {
                System.Reflection.MethodInfo mi =
                e.Control.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                mi.Invoke(e.Control, new object[] { ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true });
            }
            base.OnControlAdded(e);
        }

        public MainForm()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);//设置双缓冲，提高屏幕响应速度
            this.UpdateStyles();

            //Control.CheckForIllegalCrossThreadCalls = false;
            showSp = new Thread(new ThreadStart(this.showSplash));
            showSp.Start();
            showSp.IsBackground = true;
            showSp.Join();
            showSp = null;
            InitializeFrame();//加载主窗体并处理子窗体
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
            this.WindowState = FormWindowState.Maximized;//窗口最大化
            //this.TopMost = true;//窗口置顶
            this.AutoSize = false;
            this.BackgroundImage = bgImage;
            this.BackgroundImageLayout = ImageLayout.Stretch;

        }

        private void InitializeFrame()
        {
            InitializeComponent();
            //工具栏按钮处理
            this.toolStripButton_importOP.Enabled = false;
            this.toolStripButton_monitor.Enabled = false;
            this.toolStripButton_shoujuan.Enabled = false;
            this.toolStripButton_exit.Enabled = false;

            bgImage = Image.FromFile("..//..//Resources/MDIMain.jpg");
            this.Load += MainForm_Load;//添加窗体加载事件
            //MDI处理
            this.IsMdiContainer = true;//指示MainForm为MDI主窗口
            childLogin = new MDIChild_Login(this);

            childLogin.Show();
            childLogin.bEnableEvent += new MDIChild_Login.ButtonEnablehandler(childForm_bEnable);//注册使导入操作题按钮可用的事件

            this.SizeChanged += MainForm_SizeChanged;
        }

        void MainForm_SizeChanged(object sender, EventArgs e)
        {
            this.Paint += MainForm_Paint;//暂时不需要重绘，刷新即可
            childLogin.Location = new Point((this.Width - childLogin.Width) / 2, this.Height - childLogin.Height-200);
            if(childImportOp != null)//确保此时导入操作题窗口已创建
                childImportOp.Location = new Point((this.Width - childImportOp.Width) / 2, (this.ClientSize.Height - childImportOp.Height)/2-150);
            this.Refresh();//防止窗体改变大小出现背景异常关键是这句，重绘函数甚至不需要
        }

        void MainForm_Paint(object sender, PaintEventArgs e)
        {
                     
            
        }
        /// <summary>
        /// 使导入操作题按钮可用的事件触发函数，激活按钮
        /// </summary>
        void childForm_bEnable()
        {
            this.toolStripButton_importOP.Enabled = true;
        }
        /// <summary>
        /// 启动画面线程函数
        /// </summary>
        public void showSplash()
        {
            //this.Refresh
            int sleepvalue;
            SplashForm splashF = new SplashForm();
            try
            {
                splashF.Show();
                splashF.Refresh();
                //如下这个形式的唯一作用就是让Circle转动
                sleepvalue = 900;
                for (int i = sleepvalue; i >= 0;i -= 30)
                {
                    splashF.Refresh();
                    Thread.Sleep(30);
                }

                CheckEnvironment(splashF);//检测本机环境
                sleepvalue = 2400;
                for (int i = sleepvalue; i >= 0; i -= 30)
                {
                    splashF.Refresh();
                    Thread.Sleep(30);
                }

                CheckWebSyncTime(splashF);//检测网络连接并同步时间
                sleepvalue = 2400;
                for (int i = sleepvalue; i >= 0; i -= 30)
                {
                    splashF.Refresh();
                    Thread.Sleep(30);
                }

                showSp.Abort();
                
            }
            catch (ThreadAbortException e)
            {
                // Thread was aborted normally 
                Debug.WriteLine("Splash window was aborted normally:" + e.Message);
            }
            finally
            {
                splashF.Close();
            }

        }

        #region 系统启动时做的Check
        /// <summary>
        /// 检测本机系统环境，有无装Excel，有无D盘，D盘空间是否足够
        /// </summary>
        public void CheckEnvironment(SplashForm sf)
        {
            sf.UpdateStatus("正在检测本机坏境");
            Type ty = Type.GetTypeFromProgID("Excel.Application");
            if (ty == null)
            {
                DialogResult r = MessageBox.Show("本系统需要安装Excel，否则程序不可用", "警告", MessageBoxButtons.OK);
                this.Dispose();
                this.Close();
                System.Environment.Exit(System.Environment.ExitCode);

            }
            else
            {//检测D盘是否存在
                bool dFlag = false;//指示D盘是否存在
                ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
                ManagementObjectCollection disks = diskClass.GetInstances();
                foreach (ManagementObject disk in disks)
                {
                    if (String.Equals(disk["Name"].ToString(), "D:"))
                        dFlag = true;
                    
                }
                if (dFlag == false)
                {
                    MessageBox.Show("本系统需要D驱动盘，否则程序不可用", "警告", MessageBoxButtons.OK);
                    this.Dispose();
                    this.Close();
                    System.Environment.Exit(System.Environment.ExitCode);
                }
                else
                {
                    DriveInfo di = new DriveInfo("E");
                    if(di.DriveType!=DriveType.Fixed)
                    {//判断D盘是否为固定磁盘，而非CDROM之类的磁盘
                        MessageBox.Show("检测到D盘非固定磁盘，程序不可用", "警告", MessageBoxButtons.OK);
                        this.Dispose();
                        this.Close();
                        System.Environment.Exit(System.Environment.ExitCode);
                    }
                    else
                    {
                        long capablity = di.TotalFreeSpace / (1024 * 1024);
                        if (capablity <= 10)
                        {
                            MessageBox.Show("D盘空间不足，程序不可用", "警告", MessageBoxButtons.OK);
                            this.Dispose();
                            this.Close();
                            System.Environment.Exit(System.Environment.ExitCode);
                        }
                    }
                }
                   
            }
        }
        /// <summary>
        /// 此函数功能为：与Web服务器的通信，如果连上则同步时间
        /// </summary>
        public void CheckWebSyncTime(SplashForm sf)
        {
            int sleepvalue;
            sf.UpdateStatus("正在尝试与服务器连接");
            try
            {
                UriBuilder ser = new UriBuilder("http", ConfigXML.GetWebIP(), 80);
                HttpWebRequest hwRequest = WebRequest.CreateHttp(ser.Uri);
                hwRequest.Timeout = 6000;
                HttpWebResponse hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                if (hwResponse.StatusCode == HttpStatusCode.OK)
                {//连上就同步时间
                    sleepvalue = 2400;
                    for (int i = sleepvalue; i >= 0; i -= 30)
                    {
                        sf.Refresh();
                        Thread.Sleep(30);
                    }
                    sf.UpdateStatus("与服务器连接成功");
                    sleepvalue = 720;
                    for (int i = sleepvalue; i >= 0; i -= 30)
                    {
                        sf.Refresh();
                        Thread.Sleep(30);
                    }
                    sf.UpdateStatus("正在同步时间");
                    //hwResponse.Headers[3]就是http头中表示时间的字段
                    DateTime serverTime = GMT2Local(hwResponse.Headers[3]);

                    SYSTEMTIME st = new SYSTEMTIME();
                    st.wYear = Convert.ToUInt16(serverTime.Year);
                    st.wMonth = Convert.ToUInt16(serverTime.Month);
                    st.wDay = Convert.ToUInt16(serverTime.Day);
                    st.wHour = Convert.ToUInt16(serverTime.Hour);
                    st.wMilliseconds = Convert.ToUInt16(serverTime.Millisecond);
                    st.wMinute = Convert.ToUInt16(serverTime.Minute);
                    st.wSecond = Convert.ToUInt16(serverTime.Second);
                    //MessageBox.Show(serverTime.ToString());
                    SetLocalTime(ref st);//需要以管理员身份运行才能改变系统时间
                }
                hwResponse.Close();//应该是连接之后马上关掉，再Request，而不是一直维持一个Response
                //hwRequest.
            }
            catch (WebException wException)
            {
                sf.UpdateStatus("与服务器连接失败");
                sleepvalue = 810;
                for (int i = sleepvalue; i >= 0; i -= 30)
                {
                    sf.Refresh();
                    Thread.Sleep(30);
                }
                MessageBox.Show("与服务器连接错误，原因为" + wException.Status.ToString()+",程序将退出", "警告", MessageBoxButtons.OK);
                this.Dispose();
                this.Close();
                System.Environment.Exit(System.Environment.ExitCode);
            }

        }

        /// <summary>  
        /// GMT时间转成本地时间  
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns></returns>  
        public  DateTime GMT2Local(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;  
        }
        #endregion
        /// <summary>
        /// 鼠标点击导入操作题按钮事件触发函数
        /// </summary>
        private void toolStripButton_importOP_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this.toolStrip1.Height.ToString());
            childImportOp = new MDIChild_ImportOP(this);
            childImportOp.MdiParent = this;
            childImportOp.StartPosition = FormStartPosition.CenterScreen;
            childImportOp.Show();          
        }
        /// <summary>
        /// 激活考场监视按钮
        /// </summary>
        public void monitor_bEnable()
        {
            this.toolStripButton_monitor.Enabled = true;
        }

        private void toolStripButton_monitor_Click(object sender, EventArgs e)
        {
            childMonitor = new MDIChild_Monitor(this);
            childMonitor.MdiParent = this;
            childMonitor.Show();
        }

        public int ToolStripHeight()
        {
            return this.toolStrip1.Height;
        }
    }
}
