using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace Student
{
    public partial class SplashForm : Form
    {
        LoadingCircle loadingCircle;
        private delegate void UpdateStatusHandle(string s);//更新状态label的委托
        private delegate void CloseSplashHandle();//关闭启动画面的委托
        public SplashForm()
        {
            InitializeComponent();
            //调整label的一些属性
            this.label_Name.Width = (int)((this.Size.Width - this.label_Name.Size.Width) / 2.0);
            lblHostname.Text = Machine.Hostname;
            //设置此窗体为启动窗体的一些参数
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            //全屏调整参数
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            this.label_Name.Location = new System.Drawing.Point(rect.Width / 2 - this.label_Name.Width / 2,
                rect.Height / 2);

            this.label_version.Top = this.label_Name.Bottom;
            this.label_version.Left = this.label_Name.Right - this.label_version.Width;

            this.label_status.Top = this.label_version.Bottom + this.label_Name.Height;

            //this.label_copyright.Top = this.label_status.Bottom + this.label_Name.Height;
            //this.label_copyright.Left = this.label_version.Left;
            //this.label_date.Left = this.label_copyright.Left;
            //this.label_date.Top = this.label_copyright.Bottom;

            //  this.TopMost = true;
            try
            {
                this.BackgroundImage = Image.FromFile("Resources/qidong.jpg");
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("加载启动画面失败！" + ex.Message);
                this.Dispose();
                System.Environment.Exit(System.Environment.ExitCode);
            }


            this.Load += SplashForm_Load;
            ///设置双缓冲，防止画面闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        void SplashForm_Load(object sender, EventArgs e)
        {
            loadingCircle = new LoadingCircle();
            this.loadingCircle.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircle.Color = SystemColors.ControlLightLight;
            this.loadingCircle.ForeColor = System.Drawing.Color.Red;
            this.loadingCircle.InnerCircleRadius = 4;
            this.loadingCircle.Location = new System.Drawing.Point(10, 290);
            this.loadingCircle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 9;
            this.loadingCircle.OuterCircleRadius = 5;
            this.loadingCircle.RotationSpeed = 30;
            this.loadingCircle.Size = new System.Drawing.Size(50, 90);
            this.loadingCircle.SpokeThickness = 4;
            this.loadingCircle.StylePreset = LoadingCircle.StylePresets.IE7;
            this.loadingCircle.TabIndex = 14;
            this.loadingCircle.Text = "loadingCircle";
            this.Controls.Add(this.loadingCircle);
            this.loadingCircle.Active = true;
            this.loadingCircle.Top = this.label_status.Top + this.label_status.Size.Height / 2 - this.loadingCircle.Size.Height / 2 + 1;//调整Circle与状态label的位置
            this.loadingCircle.Left = this.label_Name.Left;
            this.label_status.Left = this.loadingCircle.Left + this.loadingCircle.Size.Width;


        }

        /// <summary>
        /// 变更状态label
        /// </summary>
        /// <param name="s">显示的内容</param>
        public void UpdateStatus(string s)
        {
            if (this.InvokeRequired)
            {
                UpdateStatusHandle ush = new UpdateStatusHandle(UpdateStatus);
                this.Invoke(ush, new object[] { s });

            }
            else
            {
                this.label_status.Text = s + " . . .";
                this.Refresh();
            }
        }

        /// <summary>
        /// 关闭启动画面
        /// </summary>
        public void CloseSplash()
        {
            if (this.InvokeRequired)
            {
                CloseSplashHandle csh = new CloseSplashHandle(CloseSplash);
                this.Invoke(csh, null);

            }
            else
            {
                this.loadingCircle.Active = false;
                //this.loadingCircle.DisposeTimer();//释放这个控件中使用的定时器
                this.Close();
            }
        }
    }
}
