using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using Common;


namespace Student
{
    public partial class TestInfoForm : Form
    {
        private frmLogin closfs;
        private OperaForm opFrm;
        private Thread testThread;//检测是否开始考试
        private Label lblHostName;
        private Sql_Operate stulogin;//用于打开数据库的对象
        private FileStream fs;
        /// <summary>
        /// 定义一个委托，可以在副线程中调用主线程的东西，而不造成资源竞争
        /// </summary>
        private delegate void TestDele();
        public TestInfoForm(frmLogin fs,OperaForm op)
        {
            //连接上数据库
            stulogin = new Sql_Operate(GloabalData.GBL_DATABASE_SERVER, GloabalData.GBL_DATABASE_USER,
               GloabalData.GBL_DATABASE_PWD);
            opFrm = op;
            opFrm.Hide();
            fs.Hide();
            fs.Dispose();
            fs.Close();
            closfs = fs;
            InitializeComponent();
            webBrowTest.DocumentText = "<strong><font style=\"font-family:verdana\" size=\"6\" color=\"red\">" + "考试版本: " +
                SqlServerData.Exam_Ver + "<br>" + "</font></strong>" + "<font size=\"4\">" +
                SqlServerData.Exam_Role + "</font>";
            Show();
        }

        /// <summary>
        /// 教师机是否允许考试
        /// </summary>
        private void IsTestAllow()//如果收到可以考试通知
        {
            while (!FormConnInfo.begin_examFlag) ;//调试用的
            //更新Stu_Exam的sql
            string updateStu_Examsql = String.Format("update exam_students set flag=1,mac_address='{0}',computer_name='{1}' where stu_number='{2}' " +
                "and classid={3}", Machine.MacAddress, Machine.Hostname, FormConnInfo.StudentNum, FormConnInfo.classId);
            stulogin.ExceSql(updateStu_Examsql);
            stulogin.Close();
            OutThread();
            //生成备份文件
            fs = new FileStream("C:\\ECNU_KS\\ROOT" + "\\" + FormConnInfo.StudentNum + FormConnInfo.StudentName
                   + ".dat", FileMode.OpenOrCreate);
        }

        /// <summary>
        /// 跨线程调用
        /// </summary>
        private void OutThread()
        {
            if (opFrm.InvokeRequired || this.InvokeRequired || closfs.InvokeRequired) //等待异步
            {
                TestDele ansysT = new TestDele(OutThread);
                this.Invoke(ansysT);
            }
            else
            {
                opFrm.Show();
                //opFrm.ShowInTaskbar = true;
                //opFrm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                //opFrm.WindowState = FormWindowState.Maximized;
                opFrm.Refresh();
                opFrm.TopMost = true;
                testThread.Abort();
                this.Close();
                closfs.Close();
            }
        }

        /// <summary>
        /// 加载窗体触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestInfoForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // 设置边框为 None
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized; // 最大化
            lblHostName = MyControl.MyLabel;
            this.lblHostName.Left = this.Left;
            this.lblHostName.Top = this.Top;
            this.lblHostName.Visible = true;
            this.Controls.Add(this.lblHostName);
            //调整考试须知位置
            this.lblTestRemainder.Left = this.Width / 2 - this.lblTestRemainder.Width / 2;
            this.lblTestRemainder.Top = this.lblHostName.Bottom + 10;
            //调整考试须知内容位置
            this.webBrowTest.Top = this.lblTestRemainder.Bottom + 40;
            this.webBrowTest.Left = this.Left + 40;
            this.webBrowTest.Size = new System.Drawing.Size(this.Width - 2 * 40, this.Height - this.lblTestRemainder.Bottom - 2 * 40);
            //开启线程，检测是否允许考试
            testThread = new Thread(IsTestAllow);
            testThread.IsBackground = true;
            testThread.Start();
        }
    }
}
