using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using Common;
using System.Management;
using System.Windows.Forms;

namespace Student
{
    public static class ReadConfigXml
    {
        public static bool Load_config()
        {
            try
            {
                XmlReader rdr = XmlReader.Create("studentconfig.xml");
                while (!rdr.EOF)
                {
                    if (rdr.NodeType == XmlNodeType.Element && rdr.Name == "Info")
                    {
                        try
                        {
                            GloabalData.TeacherIP = rdr.GetAttribute("TEACHER_IP");
                            return true;
                        }
                        catch (ArgumentNullException ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message, "警告");
                            return false;
                        }
                    }
                    else
                    {
                        rdr.Read();
                    }
                }
                return false;
             }
            catch(System.ArgumentNullException ex) 
            {
                System.Windows.Forms.MessageBox.Show("读取xml错误:"+ex.Message, "提示");
                return false;
            }  
        }
    }
    public static class FormConnInfo
    {
        /// <summary>
        /// 与教师机通信连接成功标志,通信成功为true
        /// </summary>
        public static bool teacherConnSuecc;
        /// <summary>
        /// 教师机点击开始考试后连接成功标志
        /// </summary>
        public static bool begin_examFlag;
        /// <summary>
        /// 学生机点击交卷或则教师机点击收卷为true,教师机异常用
        /// </summary>
        public static bool end_examFlag;
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string web_ip;
        /// <summary>
        /// 分辨率宽度，默认1024
        /// </summary>
        public static int screen_width;
        /// <summary>
        /// 分辨率高度,默认768
        /// </summary>
        public static int screen_height;
        /// <summary>
        /// 学生系别
        /// </summary>
        public static string claasName;
        /// <summary>
        /// 学生班级ID,学生插入答案登陆使用
        /// </summary>
        public static string classId;
        /// <summary>
        /// 学生学号
        /// </summary>
        public static string StudentNum;
        /// <summary>
        /// 学生的姓名
        /// </summary>
        public static string StudentName;
        static FormConnInfo()
        {
            screen_width = 1024;
            screen_height = 768;
            web_ip = null;
            teacherConnSuecc = false;
            begin_examFlag = false;
            claasName = null;
            classId = null;
            StudentNum = null;
            StudentName = null;
            end_examFlag = false;
        }
    }

    /// <summary>
    /// 获取本机相关信息
    /// </summary>
    public static class Machine
    {
        /// <summary>
        /// 本机Mac地址
        /// </summary>
        public static string MacAddress
        {
            get { return GetNetCardMAC(); }
        }
        /// <summary>
        /// 本机主机名
        /// </summary>
        public static string Hostname
        {
            get { return System.Net.Dns.GetHostName(); }
        }
        /// <summary>
        /// 房间号
        /// </summary>
        public static string RoomNum
        {
            get
            {
                return  Hostname.Substring(3, 3);//Stu20101从第四位开始取3位
            }
        }

        /// <summary>
        /// 获取网卡的MAC地址
        /// </summary>
        /// <returns>返回一个string</returns>
        private static string GetNetCardMAC()
        {
            try
            {
                string stringMAC = "";
                ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection MOC = MC.GetInstances();
                foreach (ManagementObject MO in MOC)
                {
                    if ((bool)MO["IPEnabled"] == true)
                    {
                        stringMAC += MO["MACAddress"].ToString();
                    }
                }
                return stringMAC;
            }
            catch
            {
                return "";
            }
        }
    }

    public static class MyControl
    {
        /// <summary>
        /// 返回一个标签，记录机器名，其中名称和位置并没有设置
        /// </summary>
        public static Label MyLabel
        {
            get { return GetMyLabel(); }
        }
        /// <summary>
        /// 制造一个我用标签 
        /// </summary>
        /// <returns></returns>
       private static Label GetMyLabel()
        {
            Label lblHostName = new Label(); 
            lblHostName.AutoSize = true;
            lblHostName.BackColor = System.Drawing.Color.Transparent;
            lblHostName.Font = new System.Drawing.Font("楷体", 36F, System.Drawing.FontStyle.Bold);
            lblHostName.ForeColor = System.Drawing.Color.Red;
            lblHostName.Location = new System.Drawing.Point(0, 0);
            lblHostName.Size = new System.Drawing.Size(95, 48);
            lblHostName.Text = Machine.Hostname;
            return lblHostName;
        }
    }

}
