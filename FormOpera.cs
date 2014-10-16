using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using CommonControl;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Timers;


namespace Student
{
    //定义一个枚举，表示拖动方向，具体作用看后面 
    public enum MouseDirection
    {
        Herizontal,//水平方向拖动，只改变窗体的宽度       
        Vertical,//垂直方向拖动，只改变窗体的高度 
        Declining,//倾斜方向，同时改变窗体的宽度和高度       
        None//不做标志，即不拖动窗体改变大小 
    }
    public partial class OperaForm : Form
    {
        /// <summary>
        /// 教师机是否是异常退出后的重启
        /// </summary>
        private bool IsTeacherRestart = false;
        /// <summary>
        /// 备份选择题填空题答案对象
        /// </summary>
        private FileStream fs;
        /// <summary>
        /// 读文件流对象
        /// </summary>
        private StreamReader sr;
        /// <summary>
        /// 写文件流对象
        /// </summary>
        private StreamWriter sw;
        /// <summary>
        /// 确保是第一次打开监控
        /// </summary>
        private bool IsfirstWatcher = true;
        /// <summary>
        /// 是否已经点击交卷
        /// </summary>
        private bool IsHandIn = false;
        /// <summary>
        /// 记录学生联系方式
        /// </summary>
        private string stuphoneNum;
        /// <summary>
        /// 参与考试的房间号
        /// </summary>
        private string roomNum;

        private bool isMouseDown = false; //表示鼠标当前是否处于按下状态，初始值为否 
        private MouseDirection direction = MouseDirection.None;//表示拖动的方向，起始为None，表示不拖动 

        /// <summary>
        /// root文件夹容量最大值，先定为10M
        /// </summary>
        private long rootfilemaxcapacity = 10 * 1024 * 1024;

        /// <summary>
        /// 判断是否有选择填空题，默认为true,即有。当收到了没有指令，在通信进程中置为fasle
        /// </summary>
        private bool HaveChoiceandBlank = true;

        /// <summary>
        /// 显示主机名标签
        /// </summary>
        private Label hostnameLabel;

        #region 网上邻居上传下载文件夹使用，并且增加监控
        /// <summary>
        /// 网上邻居地址，用来打开ECNU_KS文件夹
        /// </summary>
        private string net_neighbor_path = "C:\\ECNU_KS";
        /// <summary>
        /// 操作题考试题目地址1
        /// </summary>
        private string net_neighbor_htmpath = "C:\\ECNU_KS\\TEST\\tm.htm";
        /// <summary>
        /// 操作题考试题目地址2
        /// </summary>
        private string net_neighbor_htmlpath = "C:\\ECNU_KS\\Test\\tm.html";
        /// <summary>
        /// ROOT文件夹监控地址
        /// </summary>
        private string net_neighbor_rootfilePath = "C:\\ECNU_KS\\ROOT";
        /// <summary>
        /// 文件大小改变将其置为true
        /// </summary>
        private bool file_changed = false;
        /// <summary>
        /// 文件大小创建将其置为true
        /// </summary>
        private bool file_created = false;
        /// <summary>
        /// 文件删除将其置为true
        /// </summary>
        private bool file_deleted = false;
        /// <summary>
        /// 文件改变定时器
        /// </summary>
        private System.Timers.Timer fileBackuptimer = new System.Timers.Timer();
        /// <summary>
        /// 文件创建定时器
        /// </summary>
        private System.Timers.Timer fileCreatetimer = new System.Timers.Timer();
        /// <summary>
        /// 文件被删除计时器
        /// </summary>
        private System.Timers.Timer fileDeletetimer = new System.Timers.Timer();
        /// <summary>
        /// 监控文件对象
        /// </summary>
        FileSystemWatcher filewatcher = new FileSystemWatcher();
        #endregion

        private bool formMove = false;//判定窗体是否移动
        private Point formPoint;//记录窗体的位置

        private frmLogin stuLogin;
        /// <summary>
        /// 本机名
        /// </summary>
        string hostname;  
        /// <summary>
        /// 本机mac地址
        /// </summary>
        string hostmacAddress;
        /// <summary>
        /// 本机名后两位
        /// </summary>
        int hostnameNum;

        private int choice_num, blank_num;//纪录题目量 
        private int choice_score, blank_score;//纪录分值

        private string[] blank_timu;//填空题题干
        private string[] blank_timu_keys;//填空题答案
        private int tkLnum;//填空题序号
        
        /// <summary>
        /// 选择题题干
        /// </summary>
        private string[] radiochioce_timu;
        private string[] radio_optionA;//选择题A选项
        private string[] radio_optionB;//选择题B选项
        private string[] radio_optionC;//选择题C选项
        private string[] radio_optionD;//选择题D选项
        private string[] radio_keys;//选择题答案
        private int xzLnum;//选择题序号
        /// <summary>
        /// 学生的选择题答案
        /// </summary>
        private string[] stu_radio_keys = new string[255];
        /// <summary>
        /// 学生的填空题答案
        /// </summary>
        private string[] stu_blank_keys = new string[255];

        /// <summary>
        /// 学生的选择题答案是否标注
        /// </summary>
        private bool[] stu_radio_keys_remark = new bool[255];
        /// <summary>
        /// 学生的填空题答案,备注使用
        /// </summary>
        private bool[] stu_blank_keys_remark=new bool[255];
        /// <summary>
        /// 学生选择题答案总和，要传送给教师端
        /// </summary>
        private StringBuilder strChoiceAnswer;
        /// <summary>
        /// 学生填空题答案总和，要传送给教师端
        /// </summary>
        private StringBuilder strBlankAnswer;
        
        /// <summary>
        /// 纪录选择题的序号，作全局变量用，非常关键
        /// </summary>
        private static int radio_numCount = 0;
        /// <summary>
        /// 纪录填空题的序号
        /// </summary>
        private static int blank_numCount = 0;

        private QueNumButton[] dynaChoicebtn;//动态开辟选择题按钮使用
        private QueNumButton[] dynaBlankbtn;//动态开辟填空题按钮使用
        private Panel blankDyPanle = new Panel();//动态开辟填空题动态添加填空题按钮的面板
        /// <summary>
        /// 监听教师机程序线程
        /// </summary>
        private Thread thread;
        /// <summary>
        /// 圆环转动进程
        /// </summary>
        private Thread showSp;
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

        private const int SW_HIDE = 0;  //隐藏任务栏
        private const int SW_RESTORE = 9;//显示任务栏
        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        #region 老师原有字段
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public Log log;
        private Byte[] MsgSend;
        public ClientSocket studentSocket; //控制信息的通信
        private Socket CaptureSocket; //远程屏幕监控的通信
        public delegate void MsgServer(string msg);
        private Thread t;//连接capture server的线程
#endregion
        #region 设置屏幕分辨率
        // 平台调用的申明,设置分辨率
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        };
        // 控制改变屏幕分辨率的常量
        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
        // 控制改变方向的常量定义
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;
        /// <summary>
        /// 设置分辨率
        /// </summary>
        private void SetResolution()
        {
            DEVMODE devmode = new DEVMODE();
            EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devmode);
            devmode.dmPelsWidth = System.Int32.Parse(GloabalData.GBL_SCREEN_WIDTH);
            devmode.dmPelsHeight = System.Int32.Parse(GloabalData.GBL_SCREEN_HEIGHT);
            int a = ChangeDisplaySettings(ref devmode, CDS_UPDATEREGISTRY);
            if (a != 0)
                MessageBox.Show("设置分辨率不成功");
            
        }
        #endregion http://www.atcto.net/Tech/DOTNET/2013-08-20/DOTNET,13082013343300000001.html

        /// <summary>
        /// 构造函数
        /// </summary>
        public OperaForm()
        {
            //首先读取配置文件
            if (!ReadConfigXml.Load_config())
            {
                MessageBox.Show("配置信息读取出错","警告");
                Environment.Exit(0);
            }
            else
            {
                BanKey();
                hostname = Machine.Hostname;
                roomNum = Machine.RoomNum;
                hostmacAddress = Machine.MacAddress;
                try
                {
                    hostnameNum = Convert.ToInt32(hostname.Substring(6));
                }
                catch(Exception) 
                {
                    MessageBox.Show("机器名不正确，程序将退出");
                    this.Close();
                    Environment.Exit(0);
                }
                hostnameNum = Convert.ToInt32(hostname.Substring(6));
                for (int i = 0; i < 255; i++)//初始化备选项，刚开始都是未备注
                {
                    stu_radio_keys_remark[i] = false;
                    stu_blank_keys_remark[i] = false;
                }
                    MsgSend = new Byte[GloabalData.BytesBuffer];
               // start*****************************************
                studentSocket = new ClientSocket();
                studentSocket.OnServerMsg += new ClientSocket.EventHandler(ServerMsg);
              //  end*****************************************
                log = new Log();
                try
                {
                    thread = new Thread(ConnectTeacher);
                    thread.IsBackground = true;
                    thread.Start();
                }
                catch (SocketException se)
                {
                    log.Write(se.Message, MsgType.Error);
                }
                System.Timers.Timer banie = new System.Timers.Timer();//一个禁止ie的计数器
                banie.Interval = 1000 * 10;//每隔10s监测是否打开ie浏览器
                banie.Elapsed += new System.Timers.ElapsedEventHandler(BanieqqFunc);
                banie.Start();//开启屏蔽ie定时器
                showSp = new Thread(new ThreadStart(this.ShowSplash));
                //showSp.IsBackground = true;
                showSp.Start();
                showSp.Join();
                showSp = null;
                InitializeComponent();
                if (HaveChoiceandBlank)
                {
                    this.XuanTianPanel.Visible = true;
                    this.XuanTianPanel.Enabled = true;
                    this.TianKongPanel.Visible = false;
                    this.TianKongPanel.Enabled = false;
                    this.RadioPanel.Visible = true;
                    this.RadioPanel.Enabled = true;
                    this.choiceDyPanel.Visible = true;
                    this.blankDyPanle.Visible = false;
                }
                else 
                {
                    this.XuanZeToolStripMenuItem.Enabled = false;
                    this.TianKongToolStripMenuItem.Enabled = false;
                    this.XuanTianPanel.Visible = false;
                    this.XuanTianPanel.Enabled = false;
                    this.CaozuoPanel.Visible = true;
                    this.btnNarrowSize.Enabled = true;
                    this.btnNarrowSize.Visible = true;
                }
            }   
        }

        #region ShowSplash
        /// <summary>
        /// 让圆环转动的时间
        /// </summary>
        /// <param name="sleeptime"></param>
        /// <param name="splashF"></param>
        private void Circle_Sleep(int sleeptime, SplashForm splashF)
        {
            for (int i = sleeptime; i >= 0; i -= 30)
            {
                splashF.Refresh();
                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// 启动画面线程函数
        /// </summary>
        public void ShowSplash()
        {
            SplashForm splashF = new SplashForm();
            try
            {
                splashF.Show();
                splashF.Refresh();
                //如下这个形式的唯一作用就是让Circle转动
                Circle_Sleep(600, splashF);
                //检测本机环境
                CheckEnvironment(splashF);
                Circle_Sleep(600, splashF);
                //检测与教师机通信情况
                CheckTeacherConn(splashF);
                Circle_Sleep(600, splashF);
                //检测网络连接并同步时间
                CheckWebSyncTime(splashF);
                Circle_Sleep(600, splashF);
                //检测是否是本教室同学参加考试
                CheckRoom(splashF);
                Circle_Sleep(600, splashF);
                //设置分辨率
                splashF.UpdateStatus("正在设置分辨率");
                SetResolution();
                Circle_Sleep(600, splashF);
                //连接数据库
                CheckSqlConn(splashF);
                Circle_Sleep(600, splashF);
                //如果通信成功，转入通信界面
                splashF.Close();
            }
            catch (ThreadAbortException e)
            {
                Debug.WriteLine("Splash window was aborted normally:" + e.Message);
            }
            finally
            {
                splashF.Close();
            }

        }
        #endregion

        /// <summary>
        /// 锁定屏幕操作
        /// </summary>
        private void BanKey()
        {

            //启动钩子，处理钩子事件
            keyieShield.Hook_Start();
            //屏蔽任务管理器
            keyieShield.IEQQLocking(true);
        }
        /// <summary>
        /// 屏蔽ie浏览器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BanieqqFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcesses();
            foreach (Process pr in process)
            {
                if (pr.ProcessName == "iexplore" || pr.ProcessName == "QQ")
                    pr.Kill();
            }
        }
        /// <summary>
        /// 每隔一定时间刷新时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showTimer_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel3.Text = "当前系统时间为：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region 函数加载，关闭
        /// <summary>
        /// 函数加载触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperaForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // 设置边框为 None
            //隐藏任务栏
            //ShowWindow(FindWindow("Shell_TrayWnd", null), SW_HIDE);
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized; // 最大化
            //首先是隐藏主界面
            this.Hide();
            //然后显示机器名
            hostnameLabel = MyControl.MyLabel;
            this.hostnameLabel.Left = this.Width - this.hostnameLabel.Width - 200;
            this.hostnameLabel.Top = this.Top;
            this.hostnameLabel.Visible = true;
            this.Controls.Add(this.hostnameLabel);
            this.hostnameLabel.BringToFront();
            this.hostnameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            #region 调整界面
            //底部的bottompanel
            this.bottompanel.Left = this.Left + 10;
            this.bottompanel.Top = this.Height - this.bottompanel.Height - 10;
            this.bottompanel.Width = this.Width - 20;
            this.bottompanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            ///包含operaformMenu的toppanel
            this.toppanel.Top = this.Top;
            this.toppanel.Left = 10;
            ///下面左一的PersonnalInfoPanel
            this.PersonalInfoPanel.Top = this.toppanel.Bottom + 10;
            this.PersonalInfoPanel.Left = 10;

            this.lblstu_phone.Top = this.lblStuType.Bottom + 150;
            this.txtstu_num.Top = this.lblstu_phone.Bottom + 10;
            ///右边第三个，缩小窗口
            this.panelSizeChange.Top = this.toppanel.Bottom + 30;
            this.panelSizeChange.Left = this.Width - this.panelSizeChange.Width - 20;
            this.panelSizeChange.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnReduction.Left = this.panelSizeChange.Left;
            this.btnReduction.Top = this.btnReduction.Bottom + 10;
            this.btnReduction.Anchor = this.panelSizeChange.Anchor;

            if (HaveChoiceandBlank)//如果有选择填空题
            {
                ///右边的XuantianPanel
                this.XuanTianPanel.Top = this.PersonalInfoPanel.Top;
                this.XuanTianPanel.Left = this.PersonalInfoPanel.Right + 20;
                this.XuanTianPanel.Width = this.Width - this.PersonalInfoPanel.Width - this.panelSizeChange.Width - 100;
                this.XuanTianPanel.Height = this.Height - toppanel.Height - bottompanel.Height - 60;
                this.lblReminder.Top = 20;
                this.lblReminder.Left = 20;

                this.checkbox_remark.Left = this.lblReminder.Left;
                this.checkbox_remark.Top = this.lblReminder.Bottom + 20;
                this.webBrowerTimu.Left = this.checkbox_remark.Right + 8;
                this.webBrowerTimu.Top = this.lblReminder.Bottom + 15;
                this.webBrowerTimu.Width = this.XuanTianPanel.Width - 80;
                this.webBrowerTimu.Height = this.XuanTianPanel.Height * 1 / 7;//约为容器的1/5
                this.lblResult.Top = this.webBrowerTimu.Bottom + 10;
                this.lblResult.Left = this.webBrowerTimu.Left;

                this.RadioPanel.Left = this.lblResult.Left;
                this.RadioPanel.Top = this.lblResult.Bottom + 10;
                this.TianKongPanel.Left = this.lblResult.Left;
                this.TianKongPanel.Top = this.RadioPanel.Top + 50;
                this.TianKongPanel.Width = this.webBrowerTimu.Width * 4 / 5;

                this.choiceDyPanel.Top = this.RadioPanel.Bottom + 10;
                this.choiceDyPanel.Left = this.RadioPanel.Left;
                this.choiceDyPanel.Width = this.XuanTianPanel.Width - 60;
                this.choiceDyPanel.Height = 190;//可以容纳30到题目
                //初始化动态组件，新添加一个加入填空题的面板
                this.XuanTianPanel.Controls.Add(this.blankDyPanle);
                this.blankDyPanle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
                this.blankDyPanle.Location = this.choiceDyPanel.Location;
                this.blankDyPanle.Name = "blankDyPanle";
                this.blankDyPanle.Size = this.choiceDyPanel.Size;
                this.blankDyPanle.Visible = false;
                this.blankDyPanle.AutoScroll = true;
            }
            else//如果没选择填空题
            {
                //调节位置和调节大小
                CaozuoPanel.Top = this.PersonalInfoPanel.Top;
                CaozuoPanel.Left = this.PersonalInfoPanel.Right + 10;
                this.CaozuoPanel.Width = this.Width - this.PersonalInfoPanel.Width - (this.Width - this.panelSizeChange.Left) - 40;
                CaozuoPanel.Height = this.Height - toppanel.Height - bottompanel.Height - 60;
                CaozuoPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            }
            #endregion

            //更新时间进度条
            this.toolStripStatusLabel3.Text = "当前系统时间为：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//更新状态显示器
            this.showTimer.Interval = 1000;//每隔1s进行刷新
            this.showTimer.Start();//开启状态线程

            //进入登陆界面
            stuLogin = new frmLogin(this, studentSocket, HaveChoiceandBlank);
            stuLogin.Owner = this;
            stuLogin.Show();
        }

        /// <summary>
        /// 动态处理题目序号
        /// </summary>
        public void DynamicShowTimuButton()
        {
            dynaChoicebtn = new QueNumButton[choice_num];
            dynaBlankbtn = new QueNumButton[blank_num];
            //int tmp_height = quenumbtn[0].Height;
            //动态处理选择题按钮
            int everyLineNum = 9;
            for (int i = 0; i <= choice_num / everyLineNum; i++)
            {
                if (i < choice_num / everyLineNum)
                {
                    for (int j = 0; j < everyLineNum; j++)
                    {
                        dynaChoicebtn[i * everyLineNum + j] = new QueNumButton();
                        //增加效果
                        dynaChoicebtn[i * everyLineNum + j].Font = new System.Drawing.Font("微软雅黑", 15F);
                        dynaChoicebtn[i * everyLineNum + j].Click += dynaChoicebtn_Click;
                        dynaChoicebtn[i * everyLineNum + j].BackColor = Color.Red;
                        this.choiceDyPanel.Controls.Add(dynaChoicebtn[i * everyLineNum + j]);
                        dynaChoicebtn[i * everyLineNum + j].Location = new System.Drawing.Point(Convert.ToInt32((this.choiceDyPanel.Width - 5) * j / everyLineNum), 50 * i + 5);
                        dynaChoicebtn[i * everyLineNum + j].Text = String.Format("第{0}题", i * everyLineNum + j + 1);
                        dynaChoicebtn[i * everyLineNum + j].Name = (i * everyLineNum + j + 1).ToString();
                        dynaChoicebtn[i * everyLineNum + j].Size = new System.Drawing.Size(Convert.ToInt32((this.choiceDyPanel.Width - 100) / everyLineNum), 50);
                    }
                }
                else
                {
                    for (int j = 0; j < choice_num % everyLineNum; j++)
                    {
                        //QueNumButton quenumbtn = new QueNumButton();
                        dynaChoicebtn[i * everyLineNum + j] = new QueNumButton();
                        //增加效果
                        dynaChoicebtn[i * everyLineNum + j].Font = new System.Drawing.Font("微软雅黑", 15F);
                        dynaChoicebtn[i * everyLineNum + j].Click += dynaChoicebtn_Click;
                        dynaChoicebtn[i * everyLineNum + j].BackColor = Color.Red;
                        this.choiceDyPanel.Controls.Add(dynaChoicebtn[i * everyLineNum + j]);
                        dynaChoicebtn[i * everyLineNum + j].Location = new System.Drawing.Point(Convert.ToInt32((this.choiceDyPanel.Width - 5) * j / everyLineNum), 50 * i + 5);
                        dynaChoicebtn[i * everyLineNum + j].Text = String.Format("第{0}题", i * everyLineNum + j + 1);
                        dynaChoicebtn[i * everyLineNum + j].Name = (i * everyLineNum + j + 1).ToString();
                        dynaChoicebtn[i * everyLineNum + j].Size = new System.Drawing.Size(Convert.ToInt32((this.choiceDyPanel.Width - 100) / everyLineNum), 50);
                    }
                }
            }
            //动态处理填空题按钮
            for (int i = 0; i <= blank_num / everyLineNum; i++)
            {
                if (i < blank_num / everyLineNum)
                {
                    for (int j = 0; j < everyLineNum; j++)
                    {
                        dynaBlankbtn[i * everyLineNum + j] = new QueNumButton();
                        //增加效果
                        dynaBlankbtn[i * everyLineNum + j].Font = new System.Drawing.Font("微软雅黑", 15F);
                        dynaBlankbtn[i * everyLineNum + j].Click += dynaBlankbte_Click;
                        dynaBlankbtn[i * everyLineNum + j].BackColor = Color.Red;
                        this.blankDyPanle.Controls.Add(dynaBlankbtn[i * everyLineNum + j]);
                        dynaBlankbtn[i * everyLineNum + j].Location = new System.Drawing.Point(Convert.ToInt32((this.blankDyPanle.Width - 5) * j / everyLineNum), 50 * i + 5);
                        dynaBlankbtn[i * everyLineNum + j].Text = String.Format("第{0}题", i * everyLineNum + j + 1);
                        dynaBlankbtn[i * everyLineNum + j].Name = (i * everyLineNum + j + 1).ToString();
                        dynaBlankbtn[i * everyLineNum + j].Size = new System.Drawing.Size(Convert.ToInt32((this.blankDyPanle.Width - 100) / everyLineNum), 50);
                    }
                }
                else
                {
                    for (int j = 0; j < blank_num % everyLineNum; j++)
                    {
                        //QueNumButton quenumbtn = new QueNumButton();
                        dynaBlankbtn[i * everyLineNum + j] = new QueNumButton();
                        //增加效果
                        dynaBlankbtn[i * everyLineNum + j].Font = new System.Drawing.Font("微软雅黑", 15F);
                        dynaBlankbtn[i * everyLineNum + j].Click += dynaBlankbte_Click;
                        dynaBlankbtn[i * everyLineNum + j].BackColor = Color.Red;
                        this.blankDyPanle.Controls.Add(dynaBlankbtn[i * everyLineNum + j]);
                        dynaBlankbtn[i * everyLineNum + j].Location = new System.Drawing.Point(Convert.ToInt32((this.blankDyPanle.Width - 5) * j / everyLineNum), 50 * i + 5);
                        dynaBlankbtn[i * everyLineNum + j].Text = String.Format("第{0}题", i * everyLineNum + j + 1);
                        dynaBlankbtn[i * everyLineNum + j].Name = (i * everyLineNum + j + 1).ToString();
                        dynaBlankbtn[i * everyLineNum + j].Size = new System.Drawing.Size(Convert.ToInt32((this.blankDyPanle.Width - 100) / everyLineNum), 50);
                    }
                }
            }
        }

        /// <summary>
        /// 动态处理选择题按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dynaChoicebtn_Click(object sender, EventArgs e)
        {
            Button quotebtn = (Button)sender;//根据sender获得引用
            radio_numCount = int.Parse(quotebtn.Name) - 1;
            this.webBrowerTimu.DocumentText = (radio_numCount + 1).ToString() + ": " + radiochioce_timu[radio_numCount];
            this.webBrowAnsA.DocumentText = radio_optionA[radio_numCount];
            this.webBrowAnsB.DocumentText = radio_optionB[radio_numCount];
            this.webBrowAnsC.DocumentText = radio_optionC[radio_numCount];
            this.webBrowAnsD.DocumentText = radio_optionD[radio_numCount];
            SetRadioStatus();
        }

        /// <summary>
        /// 动态处理填空题按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dynaBlankbte_Click(object sender, EventArgs e)
        {
            Button quotebtn = (Button)sender;//根据sender获得引用
            blank_numCount = int.Parse(quotebtn.Name) - 1;
            this.webBrowerTimu.DocumentText = (blank_numCount + 1).ToString() + ": " + blank_timu[blank_numCount];
            txtStuBlank.Text = stu_blank_keys[blank_numCount];
            if (stu_blank_keys_remark[blank_numCount] == true)
                checkbox_remark.Checked = true;
            else
                checkbox_remark.Checked = false;

        }

        private void OperaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //解除钩子
            keyieShield.Hook_Clear();
            keyieShield.IEQQLocking(false);
            //显示任务栏
            ShowWindow(FindWindow("Shell_TrayWnd", null), SW_RESTORE);
        }

        #endregion

        #region Upan
        /// <summary>
        /// 检测U盘函数
        /// </summary>
        /// <param name="m">windows截获的消息</param>
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x0219)//WM_DEVICECHANGE
            {
                switch (m.WParam.ToInt32())
                {
                    case 0x8000://DBT_DEVICEARRIVAL
                        {
                            if (!FormConnInfo.end_examFlag)
                            {
                                lblUpan.Visible = true;
                                lblUpan.Text = "U盘插入!!!";
                                //发送信息给教师机
                                studentSocket.SendStrMsg(EclassCommand.HAVEUSB);
                            }
                            break;
                        }
                    case 0x8004://DBT_DEVICEREMOVECOMPLETE
                        {
                            //U盘.Text = "已经拔出";
                            lblUpan.Visible = true;
                            lblUpan.Text = "U盘拔出!!!";
                            break;
                        }
                    default:

                        break;
                }
            }
            base.DefWndProc(ref m);
        }
        #endregion 

        #region 系统启动时做的Check

        /// <summary>
        /// 检测是否是本教室同学参加考试
        /// </summary>
        /// <param name="sf"></param>
        private void CheckRoom(SplashForm sf)
        {
            sf.UpdateStatus("正在检测考场房间号");
            if (!roomNum.Equals(GloabalData.GBL_ROOMNUM))//如果不是本教室同学参与考试，退出
            {
                MessageBox.Show("本教室是" + roomNum + "教室,暂时不参加考试");
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// 检测与Sql的连接
        /// </summary>
        /// <param name="sf"></param>
        private void CheckSqlConn(SplashForm sf)
        {
            sf.UpdateStatus("正在检测数据库");
            Circle_Sleep(300, sf);
            //GloabalData.GBL_COURSEID = "24";//调试用的
            SqlServerData.ReadTestKnown();//读取考试须知
            sf.UpdateStatus("数据库连接正常");
        }

        /// <summary>
        /// 检测与教师机的连接
        /// </summary>
        /// <param name="sf"></param>
        private void CheckTeacherConn(SplashForm sf)
        {
            sf.UpdateStatus("正在检测与教师机的通信");
            while (!FormConnInfo.teacherConnSuecc)
            {
                Circle_Sleep(900, sf);
                sf.Refresh();
                for (int j = 0; j < 30; j++)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }
            }  
        }

        /// <summary>
        /// 检测本机系统环境，有无装Excel，有无D盘，D盘空间是否足够
        /// </summary>
        private void CheckEnvironment(SplashForm sf)
        {
            sf.UpdateStatus("正在检测本机环境");
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
                DriveInfo uPanCheck;
                foreach (ManagementObject disk in disks)
                {
                    if (String.Equals(disk["Name"].ToString(), "D:"))
                        dFlag = true;
                    //if(String.Equals(disk["Name"].ToString(),"")
                    uPanCheck = new DriveInfo(disk["Name"].ToString());
                    if (uPanCheck.DriveType == DriveType.Removable)
                    {
                        MessageBox.Show("U盘插入，非法操作！", "警告");
                        Environment.Exit(0);
                    }      
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
                    DriveInfo di = new DriveInfo("D");
                    if (di.DriveType != DriveType.Fixed)
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
        private void CheckWebSyncTime(SplashForm sf)
        {
            sf.UpdateStatus("正在尝试与服务器连接");
            try
            {
                //UriBuilder ser = new UriBuilder("http", GloabalData.GBL_WEB_IP);
                HttpWebRequest hwRequest = (HttpWebRequest)WebRequest.Create("http://" + GloabalData.GBL_WEB_IP + "/getDate.asp");
                hwRequest.Timeout = 6000;
                HttpWebResponse hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                if (hwResponse.StatusCode == HttpStatusCode.OK)
                {//连上就同步时间
                    sf.UpdateStatus("连接服务器并同步时间");
                    DateTime serverTime = GMT2Local(hwResponse.Headers["Date"]);
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
            }
            catch (WebException wException)
            {
                sf.UpdateStatus("与服务器连接失败");
                Circle_Sleep(600, sf);
                MessageBox.Show("与服务器连接错误，原因为" + wException.Status.ToString() + ",程序将退出", "警告", MessageBoxButtons.OK);
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
        private DateTime GMT2Local(string gmt)
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

        #region 原有的代码,与教师机通信

        private void ConnectTeacher() //控制信息的通信
        {
            while (true)
            {
                studentSocket.ConnectStart(GloabalData.TeacherIP, GloabalData.Port_teacher_stu, GloabalData.TFKeyWord);
                Thread.Sleep(1000); 
            }
        }
        /// <summary>
        /// 和教师机通信的回调信息
        /// </summary>
        /// <param name="msg"></param>
        private void ServerMsg(string msg)
        {
            if (InvokeRequired)
            {
                MsgServer c = new MsgServer(AddMsg);
                this.Invoke(c, new object[] { msg });
            }
            else
            {
                AddMsg(msg);
            }
        }
        /// <summary>
        /// 显示信息到窗口
        /// </summary>
        /// <param name="s">教师机反馈信息序列</param>
        private void AddMsg(string s)
        {
            try
            {
                if (s.IndexOf(EclassCommand.LOGINMETHOD, 0, s.Length) == 0)//与教师集通信成功
                {
                    log.Write("成功登录教师机!\r\n");
                    string[] tmp = s.Split(GloabalData.SeperateChar);//学生是否匿名登录
                    if (tmp.Length != 11)
                    {
                        return;
                    }
                    else
                    {
                        GloabalData.GBL_COURSEID = tmp[2];//课程名
                        GloabalData.GBL_COURSENAME = tmp[3];
                        GloabalData.GBL_SCREEN_HEIGHT = tmp[4];
                        GloabalData.GBL_SCREEN_WIDTH = tmp[5];
                        GloabalData.GBL_WEB_IP = tmp[6];
                        GloabalData.GBL_DATABASE_SERVER = tmp[7];
                        GloabalData.GBL_DATABASE_USER = tmp[8];
                        GloabalData.GBL_DATABASE_PWD = tmp[9];
                        GloabalData.GBL_ROOMNUM = tmp[10];
                        //设置全局变量，用来在所有窗体中共享
                        FormConnInfo.web_ip = GloabalData.GBL_WEB_IP;
                        FormConnInfo.teacherConnSuecc = true;
                        FormConnInfo.screen_width = int.Parse(GloabalData.GBL_SCREEN_WIDTH);
                        FormConnInfo.screen_height = int.Parse(GloabalData.GBL_SCREEN_HEIGHT);
                        //EclassCommand.
                        studentSocket.SendStrMsg(EclassCommand.IAMSTU + GloabalData.SeperateChar.ToString() + hostname);
                        if (IsTeacherRestart)
                        {
                            Thread.Sleep(1000);
                            studentSocket.SendStrMsg(EclassCommand.LOGININFO + GloabalData.SeperateChar.ToString() + FormConnInfo.StudentNum +
                             "&" + FormConnInfo.StudentName);
                            if (FormConnInfo.end_examFlag)
                            {
                                Thread.Sleep(1000);
                                studentSocket.SendStrMsg(EclassCommand.CHOICEBLANKRESULT + GloabalData.SeperateChar.ToString() + hostname
                + GloabalData.SeperateChar.ToString() + FormConnInfo.StudentNum + GloabalData.SeperateChar.ToString() +
                FormConnInfo.StudentName + GloabalData.SeperateChar.ToString() + FormConnInfo.claasName + GloabalData.SeperateChar.ToString() +
                strChoiceAnswer + GloabalData.SeperateChar.ToString() + strBlankAnswer + GloabalData.SeperateChar.ToString() +
                FormConnInfo.classId + GloabalData.SeperateChar.ToString() + stuphoneNum);//传给教师机选择填空题答案
                            }
                        }
                    }
                }
                else if (s.IndexOf(EclassCommand.GotoDoExam, 0, s.Length) == 0)//收到开始考试命令
                {
                    log.Write("收到开始考试命令!\r\n");
                    FormConnInfo.begin_examFlag = true;
                }
                else if (s.IndexOf(EclassCommand.GotoDoExamEnd, 0, s.Length) == 0)//收到交卷命令
                {
                    if(!IsHandIn)
                        EndExam();//学生结束考试
                    FormConnInfo.end_examFlag = true;
                    log.Write("收到教师机传来的交卷命令!\r\n");

                }
                else if(s.IndexOf(EclassCommand.GotoDoExamEndOK,0,s.Length)==0)//收到考试结束命令
                {
                    this.Close();
                    log.Write("考试结束!!!");
                }
                else if (s.IndexOf(EclassCommand.HAVENOTCHOICEBLANK, 0, s.Length) == 0)//收到没有选择填空题命令
                {
                    HaveChoiceandBlank = false;
                    log.Write("考试没有选择填空题");
                }
                else if (s.IndexOf(EclassCommand.REMINGDERTIME, 0, s.Length) == 0)//收到考试时间提醒
                {
                    string[] tmp = s.Split(GloabalData.SeperateChar);//学生是否匿名登录
                    if (tmp.Length != 2)
                    {
                        return;
                    }
                    string reminderTime = tmp[1];
                    if (!FormConnInfo.end_examFlag)
                    {
                        MessageBox.Show("距离考试结束还有" + reminderTime + "分钟", "考试提示");
                    }
                }
                else if (s.IndexOf(EclassCommand.RESTARTCOMPUTER, 0, s.Length) == 0)//收到了关机指令
                {
                    ShutDown();
                }
                else if (s.IndexOf(EclassCommand.NEEDRESTORE, 0, s.Length) == 0)//有还原指令
                {
                    if (!GetBackupResult())//自动还原，如果还原不成功，显示还原按钮
                    {
                        btnReduction.Enabled = false;
                        btnReduction.Visible = true;
                    }
                    log.Write("收到还原指令");
                }

                else if (s.IndexOf(EclassCommand.SameStudentNumber, 0, s.Length) == 0)
                {
                    MessageBox.Show("你输入的学号已经在系统中登录过，请退出先前的登录，或者重新输入一个新的学号");
                }
                else if (s.IndexOf(EclassCommand.RELOADCHOICEBLANK, 0, s.Length) == 0)//重新载入选择题
                {
                    ReadTestLibrary();//重新读入题目
                    if (RadioPanel.Visible == true)
                    {
                        this.webBrowerTimu.Refresh();
                        this.webBrowAnsA.Refresh();
                        this.webBrowAnsB.Refresh();
                        this.webBrowAnsC.Refresh();
                        this.webBrowAnsD.Refresh();
                        this.webBrowerTimu.DocumentText = (radio_numCount + 1).ToString() + ":" + radiochioce_timu[radio_numCount];
                        this.webBrowAnsA.DocumentText = radio_optionA[radio_numCount];
                        this.webBrowAnsB.DocumentText = radio_optionB[radio_numCount];
                        this.webBrowAnsC.DocumentText = radio_optionC[radio_numCount];
                        this.webBrowAnsD.DocumentText = radio_optionD[radio_numCount];
                        SetRadioStatus();//设置单选选中状态 
                    }
                    if (TianKongPanel.Visible == true)
                    {
                        this.webBrowerTimu.Refresh();
                        this.webBrowerTimu.DocumentText = (blank_numCount + 1).ToString() + blank_timu[blank_numCount];
                    }
                    //Refresh();
                }
                else if (s.IndexOf(EclassCommand.RELOADOP, 0, s.Length) == 0)
                {
                    try
                    {
                        Uri fileuri = new Uri(net_neighbor_htmpath);
                        CaozuoBrowser.Url = fileuri;
                    }
                    catch (ArgumentNullException)//如果没有htmpath
                    {
                        try
                        {
                            Uri filuri = new Uri(net_neighbor_htmlpath);
                            CaozuoBrowser.Url = filuri;
                        }
                        catch (ArgumentNullException)
                        {
                            MessageBox.Show("未找到html考试内容");
                        }
                    }
                    Refresh();
                }
                else
                {
                    if (s.IndexOf("-ConnError_0", 0, s.Length) == 0)//教师机主动关闭是可以捕获的
                    {
                        log.Write("教师机停止服务!\r\n");
                        if (FormConnInfo.begin_examFlag)
                            IsTeacherRestart = true;
                    }
                    if (s.IndexOf("-ConnError_1", 0, s.Length) == 0)
                    {
                        log.Write("Socket已经关闭!\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(ex.Message);
            }
        }
        protected void CloseCaptureSocket()
        {
            try
            {
                // if (t.IsAlive) 如何结束一个主线程开启的子线程
                t.Abort();
                if (CaptureSocket.Connected)
                {
                    //禁用发送和接受
                    CaptureSocket.Shutdown(SocketShutdown.Both);
                    //关闭套接字，不允许重用
                    CaptureSocket.Disconnect(false);
                    CaptureSocket.Close();
                }
            }
            catch
            { }
        }
        #endregion

        #region 点击时候发生
        /// <summary>
        /// 设置单选题答案状态
        /// </summary>
        private void SetRadioStatus()
        {
            if (stu_radio_keys[radio_numCount] == "A")
            {
                radioButtonA.Checked = true;
                radioButtonB.Checked = false;
                radioButtonC.Checked = false;
                radioButtonD.Checked = false;
            }
            else if (stu_radio_keys[radio_numCount] == "B")
            {
                radioButtonA.Checked = false;
                radioButtonB.Checked = true;
                radioButtonC.Checked = false;
                radioButtonD.Checked = false;
            }
            else if (stu_radio_keys[radio_numCount] == "C")
            {
                radioButtonA.Checked = false;
                radioButtonB.Checked = false;
                radioButtonC.Checked = true;
                radioButtonD.Checked = false;
            }
            else if (stu_radio_keys[radio_numCount] == "D")
            {
                radioButtonA.Checked = false;
                radioButtonB.Checked = false;
                radioButtonC.Checked = false;
                radioButtonD.Checked = true;
            }
            else//说明没有选
            {
                radioButtonA.Checked = false;
                radioButtonB.Checked = false;
                radioButtonC.Checked = false;
                radioButtonD.Checked = false;
            }
            if (stu_radio_keys_remark[radio_numCount])//如果已经标注，设置标注按钮
                checkbox_remark.Checked = true;
            else
                checkbox_remark.Checked = false;
           
        }

        private void RadioChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelSizeChange.Visible = false;
            XuanTianPanel.Enabled = true;
            TianKongPanel.Visible = false;
            TianKongPanel.Enabled = false;
            RadioPanel.Visible = true;
            RadioPanel.Enabled = true;
            XuanTianPanel.Visible = true;
            choiceDyPanel.Visible = true;
            blankDyPanle.Visible = false;
            lblReminder.Text = String.Format("本大题共有{0}小题，每题{1}分，共{2}分", choice_num, choice_score, choice_score * choice_num);
            CaozuoPanel.Visible = false;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.webBrowerTimu.DocumentText = (radio_numCount + 1).ToString() + ": " + radiochioce_timu[radio_numCount];
            this.webBrowAnsA.DocumentText = radio_optionA[radio_numCount];
            this.webBrowAnsB.DocumentText = radio_optionB[radio_numCount];
            this.webBrowAnsC.DocumentText = radio_optionC[radio_numCount];
            this.webBrowAnsD.DocumentText = radio_optionD[radio_numCount];
            SetRadioStatus();//设置单选选中状态 
        }

        private void BlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //这两句话非常重要，如果不是选择题这个不能操作
            studentSocket.SendStrMsg(EclassCommand.NEEDBACKUP + GloabalData.SeperateChar.ToString() +
                    FormConnInfo.StudentNum + GloabalData.SeperateChar.ToString() + hostname);
            XuanTianPanel.Enabled = true;
            XuanTianPanel.Visible = true;
            RadioPanel.Enabled = false;
            RadioPanel.Visible = false;
            TianKongPanel.Visible = true;
            TianKongPanel.Enabled = true;
            panelSizeChange.Visible = false;
            choiceDyPanel.Visible = false;
            blankDyPanle.Visible = true;
            lblReminder.Text = String.Format("本大题共有{0}小题，每题{1}分，共{2}分", blank_num, blank_score, blank_score * blank_num);
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            CaozuoPanel.Visible = false;
            this.webBrowerTimu.DocumentText = (blank_numCount + 1).ToString() + ": " + blank_timu[blank_numCount];
            txtStuBlank.Text = stu_blank_keys[blank_numCount];   
        }
       
        private void HandInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定交卷", "考试提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                studentSocket.SendStrMsg(EclassCommand.HANDINSINGLE);
                IsHandIn = true;
                FormConnInfo.end_examFlag = true;
                EndExam();
            }
            
        }
        
        /// <summary>
        /// 考试结束
        /// </summary>
        private void EndExam()
        {
            //关闭文件监控
            filewatcher.Dispose();
            GetStuResult();//获取答案
            studentSocket.SendStrMsg(EclassCommand.CHOICEBLANKRESULT + GloabalData.SeperateChar.ToString() + hostname
                + GloabalData.SeperateChar.ToString() + FormConnInfo.StudentNum + GloabalData.SeperateChar.ToString() +
                FormConnInfo.StudentName + GloabalData.SeperateChar.ToString() + FormConnInfo.claasName + GloabalData.SeperateChar.ToString() +
                strChoiceAnswer + GloabalData.SeperateChar.ToString() + strBlankAnswer + GloabalData.SeperateChar.ToString() +
                FormConnInfo.classId + GloabalData.SeperateChar.ToString() + stuphoneNum);//传给教师机选择填空题答案
            InsertStuResultToDB();//插入学生结果到数据库
            EndForm endform = new EndForm(this);
            this.Hide();
            endform.Show();
        }
        /// <summary>
        /// 将学生成绩插入数据库
        /// </summary>
        private void InsertStuResultToDB()
        {
            GetStuResult();
            ///答案要sql过滤
            string updateScore = String.Format("update exam_students set choice_answer='{0}',blank_answer = '{1}'," +
                "lastlogintime='{2}' where stu_number = {3} and classid={4}",
               strChoiceAnswer, strBlankAnswer.Replace("'","''"), DateTime.Now.ToString("yyyy-MM-dd HH:mm"), FormConnInfo.StudentNum, FormConnInfo.classId);
            Sql_Operate stulogin = new Sql_Operate(GloabalData.GBL_DATABASE_SERVER, GloabalData.GBL_DATABASE_USER,
            GloabalData.GBL_DATABASE_PWD);
            if (!stulogin.ExceSql(updateScore))
                MessageBox.Show("成绩插入数据库不成功");
            stulogin.Close();
        }

        private void radioButtonA_Click(object sender, EventArgs e)
        {
            if (!stu_radio_keys_remark[radio_numCount])
                dynaChoicebtn[radio_numCount].BackColor = Color.Green;
            radioButtonA.Checked = true;
            radioButtonB.Checked = false;
            radioButtonC.Checked = false;
            radioButtonD.Checked = false;
            stu_radio_keys[radio_numCount] = "A";
            BackupResult();
        }

        private void radioButtonB_Click(object sender, EventArgs e)
        {
            if (!stu_radio_keys_remark[radio_numCount])
                dynaChoicebtn[radio_numCount].BackColor = Color.Green;//说明已经选过
            radioButtonA.Checked = false;
            radioButtonB.Checked = true;
            radioButtonC.Checked = false;
            radioButtonD.Checked = false;
            stu_radio_keys[radio_numCount] = "B";
            BackupResult();
        }

        private void radioButtonC_Click(object sender, EventArgs e)
        {
            if (!stu_radio_keys_remark[radio_numCount])
                dynaChoicebtn[radio_numCount].BackColor = Color.Green;
            radioButtonA.Checked = false;
            radioButtonB.Checked = false;
            radioButtonC.Checked = true;
            radioButtonD.Checked = false;
            stu_radio_keys[radio_numCount] = "C";
            BackupResult();
        }

        private void radioButtonD_Click(object sender, EventArgs e)
        {
            if (!stu_radio_keys_remark[radio_numCount])
                dynaChoicebtn[radio_numCount].BackColor = Color.Green;
            radioButtonA.Checked = false;
            radioButtonB.Checked = false;
            radioButtonC.Checked = false;
            radioButtonD.Checked = true;
            stu_radio_keys[radio_numCount] = "D";
            BackupResult();
        }

        /// <summary>
        /// 添加备选，事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remark_Click(object sender, EventArgs e)
        {
            if (this.checkbox_remark.Checked == true)
            {
                if (this.RadioPanel.Visible == true)
                {
                    dynaChoicebtn[radio_numCount].BackColor = Color.Yellow;
                    
                    stu_radio_keys_remark[radio_numCount] = true;//设置为备注
                }
                else if (this.TianKongPanel.Visible == true)
                {
                    dynaBlankbtn[blank_numCount].BackColor = Color.Yellow;
                    stu_blank_keys_remark[blank_numCount] = true;//设置为备注
                }
            }
            else
            {
                if (this.RadioPanel.Visible == true)
                {
                    stu_radio_keys_remark[radio_numCount] = false;//设置不为备注
                    if (stu_radio_keys[radio_numCount] != null && stu_radio_keys[radio_numCount]!="X")
                        dynaChoicebtn[radio_numCount].BackColor = Color.Green;
                    else
                    dynaChoicebtn[radio_numCount].BackColor = Color.Red;

                }
                else if (this.TianKongPanel.Visible == true)
                {
                    stu_blank_keys_remark[blank_numCount] = false;
                    if (stu_blank_keys[blank_numCount] != null && stu_blank_keys[blank_numCount]!="")
                        dynaBlankbtn[blank_numCount].BackColor = Color.Green;
                    else
                        dynaBlankbtn[blank_numCount].BackColor = Color.Red;

                }
            }
        }

        private void txtStuBlank_TextChanged(object sender, EventArgs e)
        {
            
            if (txtStuBlank.Text.Contains(GloabalData.SeperateString))
            {
                MessageBox.Show("答案不能含有ECNU");
                txtStuBlank.Clear();
                return;
            }
            stu_blank_keys[blank_numCount] = txtStuBlank.Text;//保存答案
            if (stu_blank_keys_remark[blank_numCount] == true)
            {
                dynaBlankbtn[blank_numCount].BackColor = Color.Yellow;
            }
            else
            {
                if (stu_blank_keys[blank_numCount] == "")
                    dynaBlankbtn[blank_numCount].BackColor = Color.Red;
                else
                {
                    if (!stu_blank_keys_remark[blank_numCount] == true)
                        dynaBlankbtn[blank_numCount].BackColor = Color.Green;
                }
            }
            BackupResult();
        }

        #endregion

        #region 操作题操作实现
        private void OperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            studentSocket.SendStrMsg(EclassCommand.NEEDBACKUP + GloabalData.SeperateChar.ToString() +
                    FormConnInfo.StudentNum + GloabalData.SeperateChar.ToString() + hostname);
            this.TopMost = false;
            panelSizeChange.Visible = true;
            XuanTianPanel.Visible = false;
            XuanTianPanel.Enabled = false;
            CaozuoPanel.Visible = true;
            //调节位置和调节大小
            CaozuoPanel.Top = this.PersonalInfoPanel.Top;
            CaozuoPanel.Left = this.PersonalInfoPanel.Right + 20;
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            CaozuoPanel.Width = rect.Width - this.PersonalInfoPanel.Width - (this.Width - this.panelSizeChange.Left) - 40;
            CaozuoPanel.Height = rect.Height - toppanel.Height - bottompanel.Height - 60;
            CaozuoPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnNarrowSize.Enabled = true;
            btnNarrowSize.Visible = true;
            //ftp下载文件夹
            //ftpdownload.DownFtpDir(ftpAddr, localAddr);
            //this.TopMost = true;
            if (!FileOp.FolderExist(net_neighbor_path))
            {
                MessageBox.Show("无操作题文件夹", "警告");
                FileOp.FolderCreate(net_neighbor_path);
            }
            System.Diagnostics.Process.Start(net_neighbor_path);
            //首先监控C:\\ECNU_KS\\ROOT文件夹是否为空
            if (!FileOp.FolderExist(net_neighbor_rootfilePath))//如果不存在文件夹
            {
                MessageBox.Show("不存在ROOT文件夹");
                FileOp.FolderCreate(net_neighbor_rootfilePath);//创建新的文件
            }
            else
            {
                if (Directory.GetDirectories(net_neighbor_rootfilePath).Length + Directory.GetFiles(net_neighbor_rootfilePath).Length == 0)//如果文件夹为空
                {
                    studentSocket.SendStrMsg(EclassCommand.FILESIZE + GloabalData.SeperateChar.ToString() + "null");//发送文件为空消息
                    MessageBox.Show("文件夹为空");
                }
                if (GetDictSize(net_neighbor_rootfilePath) > rootfilemaxcapacity)
                {
                    studentSocket.SendStrMsg(EclassCommand.FILESIZE + GloabalData.SeperateChar.ToString() + "over");//发送文件超标消息
                    MessageBox.Show("文件夹过大，超过10M");
                }
            }
            try
            {
                Uri fileuri = new Uri(net_neighbor_htmpath);
                CaozuoBrowser.Url = fileuri;
            }
            catch (ArgumentNullException)//如果没有htmpath
            {
                try
                {
                    Uri filuri = new Uri(net_neighbor_htmlpath);
                    CaozuoBrowser.Url = filuri;
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("未找到html考试内容");
                }
            }
            if (IsfirstWatcher)
            {
                //创建备份,使用定时器每30s进行检测，并且进行传送
                filewatcher.Path = net_neighbor_rootfilePath;
                filewatcher.Filter = "*.*";
                filewatcher.IncludeSubdirectories = true;
                filewatcher.Changed += new FileSystemEventHandler(OnProcess);
                filewatcher.Deleted += new FileSystemEventHandler(OnProcess);
                filewatcher.Created += new FileSystemEventHandler(OnProcess);
                filewatcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                filewatcher.EnableRaisingEvents = true;//开启监控
                fileBackuptimer.Interval = 1000 * 30;
                fileBackuptimer.Elapsed += new System.Timers.ElapsedEventHandler(FileChanged);
                fileBackuptimer.Start();
                fileCreatetimer.Interval = 1000 * 10;
                fileCreatetimer.Elapsed += new System.Timers.ElapsedEventHandler(FileCreated);
                fileCreatetimer.Start();
                fileDeletetimer.Interval = 1000 * 10;
                fileDeletetimer.Elapsed += new System.Timers.ElapsedEventHandler(FileDeleted);
                fileDeletetimer.Start();
                IsfirstWatcher = false;
            }
        }

        private void OnProcess(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                file_changed = true;
            }
            else if (e.ChangeType == WatcherChangeTypes.Created)
            {
                file_created = true;
                file_changed = true;
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                file_deleted = true;
                file_changed = true;
            }
        }
        private void FileChanged(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (file_changed)
            {
                //发送备份命令
                studentSocket.SendStrMsg(EclassCommand.NEEDBACKUP + GloabalData.SeperateChar.ToString() +
                    FormConnInfo.StudentNum + GloabalData.SeperateChar.ToString() + hostname);
                file_changed = false;
            }
        }
        private void FileCreated(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (file_created)
            {
                //studentSocket.SendStrMsg(EclassCommand.FILECHANGE + GloabalData.SeperateChar.ToString() + "create");//发送创建文件消息
                file_created = false;
                if (GetDictSize(net_neighbor_rootfilePath) > rootfilemaxcapacity)//ROOT文件夹如果超过10M
                {
                    studentSocket.SendStrMsg(EclassCommand.FILESIZE + GloabalData.SeperateChar.ToString() + "over");//发送文件超标消息
                    MessageBox.Show("文件夹过大，超过10M");
                }
                
            }
        }
        private void FileDeleted(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (file_deleted)
            {
                //studentSocket.SendStrMsg(EclassCommand.FILECHANGE + GloabalData.SeperateChar.ToString() + "delete");//发送文件被删除消息
                file_deleted = false;
                if (!FileOp.FolderExist(net_neighbor_rootfilePath))//如果不存在文件夹
                {
                    MessageBox.Show("不存在ROOT文件夹");
                    FileOp.FolderCreate(net_neighbor_rootfilePath);//创建新的文件
                }
                if (Directory.GetDirectories(net_neighbor_rootfilePath).Length + Directory.GetFiles(net_neighbor_rootfilePath).Length == 0)//如果文件夹为空
                {
                    studentSocket.SendStrMsg(EclassCommand.FILESIZE + GloabalData.SeperateChar.ToString() + "null");//发送文件为空消息
                    MessageBox.Show("文件夹为空");
                }
            }
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="path">文件夹的地址</param>
        /// <returns></returns>
        private long GetDictSize(string path)
        {
            if (!System.IO.Directory.Exists(path))
                return 0;
            string[] fs = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            //获取该文件夹中所有的文件名
            long ll = 0;
            foreach (string f in fs)
            {
                dynamic fa = System.IO.File.GetAttributes(f);
                System.IO.FileInfo fi = new System.IO.FileInfo(f);
                ll += fi.Length;
            }
            return ll;
        }
        #endregion
  
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lbltouming.Text = "透明度:\r\n" + (100.0 - toumingtrackBar.Value).ToString() + "%";
            Opacity = toumingtrackBar.Value / 100.0;
        }
        private void formMaxed_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            this.toppanel.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseDown);
            this.toppanel.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseMove);
            this.toppanel.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseUp);
            //this.toolStripStatusLabel2.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseDown);
            //this.toolStripStatusLabel2.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseMove);
            //this.toolStripStatusLabel2.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseUp);
            btnNarrowSize.Enabled = true;
        }
        /// <summary>
        /// 缩小窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSizechange_Click(object sender, EventArgs e)
        {
            
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                this.toppanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseDown);
                this.toppanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseMove);
                this.toppanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toppanel_MouseUp);
                if (this.Width <= 200)
                {
                    this.Width = 200;
                }
                if (this.Height <= 200)
                {
                    this.Height = 200;
                }
                if (this.Top <= 0)
                {
                    this.Top = 0;
                }
                btnNarrowSize.Enabled = false;
                return;
            }           
        }
        private void toppanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (formMove == true)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(formPoint.X, formPoint.Y);
                Location = mousePos;
            } 
        }
        private void toppanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//按下的是鼠标左键
            {
                formMove = false;//停止移动
            }
            if (this.Top <= 0)
            {
                this.Top = 0;
            }
            if (this.Left <= 0 && this.Right <= 200)
            {
                this.Left = 0;
            }
            if (this.Top >= 800)
            {
                this.Top = 800;
            }
            if (this.Left >= 800)
            {
                this.Left = 800;
            }
        }
        private void toppanel_MouseDown(object sender, MouseEventArgs e)
        {
            formPoint = new Point();
            int xOffset;
            int yOffset;
            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height;
                formPoint = new Point(xOffset, yOffset);
                formMove = true;//开始移动
            }
        }

        #region 读取题库
        /// <summary>
        /// 读取题库
        /// </summary>
        public void ReadTestLibrary()
        {

            int test_name;
            //Sql_Operate test_library_num = new Sql_Operate("202.120.84.197", "sa",
            //   "ccwebmaster");
            Sql_Operate test_library_num = new Sql_Operate(GloabalData.GBL_DATABASE_SERVER, GloabalData.GBL_DATABASE_USER,
                GloabalData.GBL_DATABASE_PWD);
            test_library_num.Open();
            //GloabalData.GBL_COURSEID = "24";//调试用
            string testLibrasql = String.Format("select choice_num ,blank_num,choice_score,blank_score from Course where Courseid={0}", GloabalData.GBL_COURSEID); ;
            try
            {

                SqlDataReader dr = test_library_num.ExceRead(testLibrasql);
                if (dr.Read())
                {
                    choice_num = Int32.Parse(dr["choice_num"].ToString());
                    blank_num = Int32.Parse(dr["blank_num"].ToString());
                    choice_score = Int32.Parse(dr["choice_score"].ToString());
                    blank_score = Int32.Parse(dr["blank_score"].ToString());
                }
                else
                {
                    MessageBox.Show("读取题库总量错误");
                }
                dr.Close();
                test_library_num.Close();
            }
            catch (SqlException e1)
            {
                test_library_num.Close();
                MessageBox.Show("连接题库表不成功:" + e1.Message);
                return;
            }

            test_name = GetDayofWeek();
            //test_name = 13;//调试用
            string xztimuSql = String.Format("SELECT  Lnum,Tgan,Option1,Option2,Option3,Option4,keys FROM Xztjk where course_id = {0} and test_name={1} order by Lnum", GloabalData.GBL_COURSEID, test_name);
            string tktimuSql = String.Format("SELECT Lnum,Tgan,keys FROM Tctjk where course_id = {0} and test_name={1} order by Lnum", GloabalData.GBL_COURSEID, test_name);
            test_library_num.Open();
            DataSet ds = test_library_num.ReadToDataset(xztimuSql, "xzt");
            ds = test_library_num.ReadToDataset(tktimuSql, "tkt");
            //读取选择题数据集合
            GetTimu(ds);

        }

        /// <summary>
        /// 读取选择题题目
        /// </summary>
        /// <param name="ds"></param>
        private void GetTimu(DataSet ds)
        {
            blank_timu = new string[255];
            blank_timu_keys = new string[255];
            radiochioce_timu = new string[255];
            radio_optionA = new string[255];
            radio_optionB = new string[255];
            radio_optionC = new string[255];
            radio_optionD = new string[255];
            radio_keys = new string[255];
            DataRow rowTmp;

            //读取选择题
            for (int i = 0; i < ds.Tables["xzt"].Rows.Count; i++)
            {
                //Lnum,Tgan,Option1,Option2,Option3,Option4,keys
                rowTmp = ds.Tables["xzt"].Rows[i];
                xzLnum = Int32.Parse(rowTmp["Lnum"].ToString());
                radiochioce_timu[i] = rowTmp["Tgan"].ToString();
                radio_optionA[i] = rowTmp["Option1"].ToString();
                radio_optionB[i] = rowTmp["Option2"].ToString();
                radio_optionC[i] = rowTmp["Option3"].ToString();
                radio_optionD[i] = rowTmp["Option4"].ToString();
                radio_keys[i] = rowTmp["keys"].ToString();
            }

            //读取填空题
            for (int i = 0; i < ds.Tables["tkt"].Rows.Count; i++)
            {
                rowTmp = ds.Tables["tkt"].Rows[i];
                tkLnum = Int32.Parse(rowTmp["Lnum"].ToString());
                blank_timu[i] = rowTmp["Tgan"].ToString();
                blank_timu_keys[i] = rowTmp["keys"].ToString();
            }
        }

        /// <summary>
        /// 得到当前星期，并转换为题目库卷号返回
        /// </summary>
        /// <returns>题目库卷号</returns>
        private int GetDayofWeek()
        {
            DateTime dt = DateTime.Now;
            int tmp = Convert.ToInt32(dt.DayOfWeek);
            if (tmp == 0)
            {
                tmp = 7;
            }
            if (int.Parse(FormConnInfo.StudentNum.Substring(FormConnInfo.StudentNum.Length - 1)) % 2 == 1)
            {
                
                return 2 * tmp - 1;
            }
            else
                return 2 * tmp;
        }

        #endregion

        /// <summary>
        /// 获取学生答案
        /// </summary>
        private void GetStuResult()
        {
            //首先求出所有选择题答案
            strChoiceAnswer = new StringBuilder("");
            strChoiceAnswer.Capacity = 100;//声明最大容量为100
            for (int i = 1; i <= choice_num; i++)
            {
                strChoiceAnswer.Append(i.ToString());//首先是题号
                if (stu_radio_keys[i - 1] == null)
                {
                    strChoiceAnswer.Append("X");
                    continue;
                }
                strChoiceAnswer.Append(stu_radio_keys[i - 1]);
            }
            //然后求出所有填空题答案
            strBlankAnswer = new StringBuilder("");
            strBlankAnswer.Capacity = 1024;//声明最大容量为100
            string tmpstublankkeys = stu_blank_keys[0];
            if(stu_blank_keys[0]==null || stu_blank_keys[0]=="")
            {
                tmpstublankkeys = " ";
            }
            ///---答案中要去掉回车换行符
            strBlankAnswer.Append(tmpstublankkeys.Replace("\r\n",""));
            for (int i = 2; i <= blank_num; i++)
            {
                strBlankAnswer.Append(GloabalData.SeperateString);
                if (stu_blank_keys[i - 1] == null || stu_blank_keys[i - 1] == "")
                {
                    strBlankAnswer.Append(" ");
                    continue;
                }
                strBlankAnswer.Append(stu_blank_keys[i - 1].Replace("\r\n",""));
            }
        }
        #region  窗体拖动
        private void OperaForm_MouseDown(object sender, MouseEventArgs e)
        {    
            //鼠标按下 
            isMouseDown = true;
        }
        private void OperaForm_MouseUp(object sender, MouseEventArgs e)
        {     
            // 鼠标弹起， 
            isMouseDown = false;       //既然鼠标弹起了，那么就不能再改变窗体尺寸，拖拽方向置None             
            direction = MouseDirection.None;
        }
        private void OperaForm_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标移动过程中，坐标时刻在改变 
            //当鼠标移动时横坐标距离窗体右边缘5像素以内且纵坐标距离下边缘也在5像素以内时，要将光标变为倾斜的箭头形状，
            //同时拖拽方向direction置为MouseDirection.Declining 
            if (e.Location.X >= this.Width - 10 && e.Location.Y > this.Height - 10)
            {
                this.Cursor = Cursors.SizeNWSE; direction = MouseDirection.Declining;
            }
            //当鼠标移动时横坐标距离窗体右边缘5像素以内时，要将光标变为倾斜的箭头形状，同时拖拽方向direction置为MouseDirection.Herizontal 

            else if (e.Location.X >= this.Width - 10)
            {
                this.Cursor = Cursors.SizeWE;
                direction = MouseDirection.Herizontal;
            }
            //同理当鼠标移动时纵坐标距离窗体下边缘5像素以内时，要将光标变为倾斜的箭头形状，同时拖拽方向direction置为MouseDirection.Vertical  
            else if (e.Location.Y >= this.Height - 10)
            {
                this.Cursor = Cursors.SizeNS;
                direction = MouseDirection.Vertical;
            }
            //否则，以外的窗体区域，鼠标星座均为单向箭头（默认）             
            else
                this.Cursor = Cursors.Arrow;
            //设定好方向后，调用下面方法，改变窗体大小  
            ResizeWindow();
        }
        private void ResizeWindow()
        {    //这个判断很重要，只有在鼠标按下时才能拖拽改变窗体大小，如果不作判断，那么鼠标弹起和按下时，窗体都可以改变 
            if (!isMouseDown)
                return;
            //MousePosition的参考点是屏幕的左上角，表示鼠标当前相对于屏幕左上角的坐标this.left和this.top的参考点也是屏幕，属性MousePosition是该程序的重点 
            if (direction == MouseDirection.Declining)
            {
                //此行代码在mousemove事件中已经写过，在此再写一遍，并不多余，一定要写                 
                this.Cursor = Cursors.SizeNWSE;
                //下面是改变窗体宽和高的代码，不明白的可以仔细思考一下 
                this.Width = MousePosition.X - this.Left;
                if (this.Width <= 200)
                {
                    this.Width = 200;
                }
                this.Height = MousePosition.Y - this.Top;
                if (this.Height <= 200)
                {
                    this.Height = 200;
                }
                if (this.Left + 100 < this.Right)
                {
                    this.Left = 20;
                    this.Width = 200;
                }
                if (this.Top + 100 < this.Bottom)
                {
                    this.Top = 20;
                    this.Height = 200;
                }
            }
            //以下同理 
            if (direction == MouseDirection.Herizontal)
            {
                this.Cursor = Cursors.SizeWE;
                this.Width = MousePosition.X - this.Left;
                if (this.Width <= 200)
                {
                    this.Width = 200;
                }
            }
            else if (direction == MouseDirection.Vertical)
            {
                this.Cursor = Cursors.SizeNS;
                this.Height = MousePosition.Y - this.Top;
                if (this.Height <= 200)
                {
                    this.Height = 200;
                }
            }
            //即使鼠标按下，但是不在窗口右和下边缘，那么也不能改变窗口大小 
            else
                this.Cursor = Cursors.Arrow;
        }
        #endregion
        /// <summary>
        /// 记录学生联系方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtstu_num_TextChanged(object sender, EventArgs e)
        {
            stuphoneNum = txtstu_num.Text;//记录学生的电话号码
        }
        /// <summary>
        /// 备份选择填空题答案
        /// </summary>
        private void BackupResult()
        {
            GetStuResult();
            try
            {
                fs = new FileStream(net_neighbor_rootfilePath + "\\" + FormConnInfo.StudentNum + FormConnInfo.StudentName
                    + ".dat", FileMode.OpenOrCreate);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(strChoiceAnswer);//写入选择题答案
                sw.WriteLine(strBlankAnswer);//写入填空题答案
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// 得到备份答案，并且更新界面
        /// </summary>
        private bool GetBackupResult()
        {
            string bakksPath = "C:\\ECNU_KS_ANS\\" + FormConnInfo.StudentNum + FormConnInfo.StudentName + ".dat";
            Thread.Sleep(2000);
            string backupResultPath = net_neighbor_rootfilePath + "\\" + FormConnInfo.StudentNum + FormConnInfo.StudentName + ".dat";
            string backupChoiceResult;
            string backupBlankResult;
            try
            {
                if (!File.Exists(bakksPath))
                {
                    MessageBox.Show("不存在备份文件");
                    return false;
                }
                File.Copy(bakksPath, backupResultPath, true);//得到备份文件
                sr = new StreamReader(backupResultPath, Encoding.UTF8);
                backupChoiceResult = sr.ReadLine();
                backupBlankResult = sr.ReadLine();
                sr.Close();
                char[] tmpresult = backupChoiceResult.ToCharArray();
                int choicecount = 0;
                for (int i = 0; i < tmpresult.Length; i++)
                {
                    if (tmpresult[i] >= 'A' && tmpresult[i] <= 'D' || tmpresult[i] == 'X')
                    {
                        stu_radio_keys[choicecount] = tmpresult[i].ToString();
                        if (stu_radio_keys[choicecount] != "X")//如果有答案,更新界面
                        {
                            dynaChoicebtn[choicecount].BackColor = Color.Green;
                        }
                        choicecount++;
                    }
                }
                if (choicecount != choice_num)
                {
                    MessageBox.Show("读入题目数量错误!");
                }
                string[] separator_tct = { GloabalData.SeperateString };//分割标志
                string[] tmp = backupBlankResult.Split(separator_tct, blank_num, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < tmp.Length; i++)
                {
                    stu_blank_keys[i] = tmp[i];
                    if (stu_blank_keys[i] == " ")
                    {
                        stu_blank_keys[i] = "";
                    }
                    if (stu_blank_keys[i] != "" && stu_blank_keys[i] != null)
                    {
                        dynaBlankbtn[i].BackColor = Color.Green;
                    }
                }
                SetRadioStatus();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "还原未成功");
                return false;
            }
        }
        /// <summary>
        /// 关机程序
        /// </summary>
        private void ShutDown()
        {
            this.Close();
            try
            {
                //启动本地程序并执行命令
                System.Diagnostics.Process.Start("shutdown.exe", " /f /r /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 备份逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReduction_Click(object sender, EventArgs e)
        {
            if (GetBackupResult())
            {
                btnReduction.Enabled = false;
                btnReduction.Visible = false;
            }
        }

        
    }
}
