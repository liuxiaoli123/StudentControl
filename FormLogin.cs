using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Configuration;

namespace Student
{
    public partial class frmLogin : Form
    {
        /// <summary>
        /// 课程名
        /// </summary>
        private string _coursename;
        /// <summary>
        /// 课程号
        /// </summary>
        private string _courseid;
        /// <summary>
        /// 服务器地址
        /// </summary>
        private string _web_Ip;
        /// <summary>
        /// 查看是否有选择填空题
        /// </summary>
        private bool _havechoiceblank;
        private OperaForm opfrm;
        private ClientSocket _studentSocket;
        private Sql_Operate stulogin;//用于打开数据库的对象
        /// <summary>
        /// 主机名
        /// </summary>
        private Label lblHostName;

        /// <summary>
        /// 带参数的构造函数frmLogin
        /// </summary>
        /// <param name="classname">班级名</param>
        /// <param name="classid">班级号</param>
        /// <param name="web_Ip">服务器地址</param>
        /// <param name="op">传入的的主要界面</param>
        /// <param name="studentSocket">传入的通信程序</param>
        public frmLogin(OperaForm op, ClientSocket studentSocket,bool havechoiceblank)
        {
            opfrm = op;
            this._coursename = GloabalData.GBL_COURSENAME;//GloabalData.GBL_COURSENAME课程名
            this._courseid = GloabalData.GBL_COURSEID;//课程号
            this._studentSocket = studentSocket;
            this._web_Ip = GloabalData.GBL_WEB_IP;//服务器地址
            this._havechoiceblank = havechoiceblank;
            InitializeComponent();
            ///设置双缓冲，防止画面闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            
            opfrm.Hide();
            opfrm.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized; 
            //全屏调整参数，调节各个控件位置
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            this.centralPanel.Location = new System.Drawing.Point(rect.Width / 2 - this.centralPanel.Width / 2,
                rect.Height / 2 - this.centralPanel.Height / 2);
            this.webBrowPicture.Left = this.centralPanel.Left - this.webBrowPicture.Width;
            this.webBrowPicture.Top = this.centralPanel.Top + 10;
            // 动态开辟显示主机名称的标签，显示在窗体左上角
            lblHostName = MyControl.MyLabel;
            this.lblHostName.Left = this.Left;
            this.lblHostName.Top = this.Top;
            this.lblHostName.Visible = true;
            this.Controls.Add(this.lblHostName);
            //连接上数据库
            stulogin = new Sql_Operate(GloabalData.GBL_DATABASE_SERVER, GloabalData.GBL_DATABASE_USER,
               GloabalData.GBL_DATABASE_PWD);
            lblCourID.Text = this._coursename;
            txtNumber.Select();
            this.ActiveControl = txtNumber;
            txtNumber.Focus();
        }

        private void txt_name_TextChanged(object sender, EventArgs e)
        {
            checkLoginBTN();
        }

        /// <summary>
        /// 登陆检验函数
        /// </summary>
        /// <returns></returns>
        private bool checkLoginBTN()
        {
            if (String.IsNullOrEmpty(txtNumber.Text) || String.IsNullOrWhiteSpace(txtNumber.Text))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 登陆检验
        /// </summary>
        /// <returns>如果成功返回true,否则返回false</returns>
        private bool LoginCheck()//尝试连接数据库，并对学号姓名进行检验
        {
           stulogin.Open();
            //查找学生姓名，班级名称，是否登陆的sql
            string selectsql =
                String.Format("use platform select S.student_name, C.classname,C.classid, E.flag from exam_students as E, Students as S, Class as C "+
            "where E.stu_number= {0} and E.stu_number = S.student_number and E.classid=C.classid and C.classid IN(select classid from class where course_id={1})",
                this.txtNumber.Text,this._courseid);
            try
            {
                SqlDataReader dr = stulogin.ExceRead(selectsql);
                if (dr.HasRows)
                {
                    dr.Read();//流式读取
                    int isRepeatNum = (int)dr["flag"];
                    if (isRepeatNum != 0)//如果重复，退出
                    {
                        MessageBox.Show("您的学号已经登陆,请检查");
                        dr.Close();
                        return false;
                    }
                    FormConnInfo.StudentName = dr["student_name"].ToString();
                    FormConnInfo.StudentNum = txtNumber.Text;
                    lblName.Text = FormConnInfo.StudentName;
                    lblClass.Text = dr["classname"].ToString();
                    FormConnInfo.claasName = lblClass.Text;
                    FormConnInfo.classId = dr["classid"].ToString();
                    dr.Close();
                    stulogin.Close();
                    Uri web_uri = new Uri(string.Format("http://{0}/ksweb/Pic.aspx?userid={1}", _web_Ip, FormConnInfo.StudentNum));
                    webBrowPicture.Url = web_uri;
                    txtNumber.Enabled = false;
                    return true;
                }
                else
                {
                    MessageBox.Show("请输入正确学号");
                    dr.Close();
                    return false;
                }
            }
            catch (SqlException)
            {
                ;
            }
            stulogin.Close();
            return false;
        }


        private void stulogin_Click(object sender, EventArgs e)
        {
            if (checkLoginBTN())
            {
                if (LoginCheck())
                {
                    _studentSocket.SendStrMsg(EclassCommand.LOGININFO + GloabalData.SeperateChar.ToString() + FormConnInfo.StudentNum +
                        "&" + FormConnInfo.StudentName);
                    //如果不重复，更新Stu_Exam的sql
                    //string updateStu_Examsql = String.Format("update exam_students set flag=1,mac_address='{0}',computer_name='{1}' where stu_number='{2}' " +
                    //    "and classid={3}", Machine.MacAddress, Machine.Hostname, FormConnInfo.StudentNum, FormConnInfo.classId);
                    //stulogin.ExceSql(updateStu_Examsql);
                    //stulogin.Close();
                    if (_havechoiceblank)
                    {
                        opfrm.ReadTestLibrary();
                        opfrm.DynamicShowTimuButton();
                    }
                    FormConnInfo.StudentNum = txtNumber.Text;
                    opfrm.AddInfomation(FormConnInfo.StudentName, FormConnInfo.StudentNum, lblCourID.Text);
                    TestInfoForm mytestInfoForm = new TestInfoForm(this, opfrm);
                    mytestInfoForm.Show();
                }
                else
                {
                    MessageBox.Show("学号有误，请查看班级，并输入正确的学号");
                }
            }
            else
                return;
        }

        private void txtNumber_MouseLeave(object sender, EventArgs e)
        {
            if (checkLoginBTN())
            {
                if (!LoginCheck())
                {
                    lblName.Text = "XXXXX";
                    lblClass.Text = "XXXXX";
                }
            }
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认退出", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
                opfrm.Close();
                Environment.Exit(0);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtNumber.Clear();
            txtNumber.Enabled = true;
            lblName.Text = "XXXXX";
            lblClass.Text = "XXXXX";
        }
        
    }
}
