namespace Student
{
    partial class OperaForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OperaForm
            // 
            this.ClientSize = new System.Drawing.Size(600, 374);
            this.Name = "OperaForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip OperaFormMenu;
        private System.Windows.Forms.ToolStripMenuItem XuanZeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CaozuoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem JiaoJuanToolStripMenuItem;
        private System.Windows.Forms.Panel PersonalInfoPanel;
        private System.Windows.Forms.Label lblStuType;
        private System.Windows.Forms.Label lblStuNum;
        private System.Windows.Forms.Label lblStulName;
        private System.Windows.Forms.ToolStripMenuItem TianKongToolStripMenuItem;


        /// <summary>
        /// 添加个人信息的方法
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="stuNum">学号</param>
        /// <param name="courseName">课程名</param>
        public void AddInfomation(string name, string stuNum, string courseName)
        {
            System.Uri web_uri = new System.Uri(string.Format("http://{0}/pic.aspx?userid={1}",FormConnInfo.web_ip, FormConnInfo.StudentNum));
            webBrowPictrue.Url = web_uri;
            this.lblStulName.Text += name;
            this.lblStuNum.Text += stuNum;
            this.lblStuType.Text += courseName;
            //调整信息panel位置
            this.lblStulName.Top = this.webBrowPictrue.Bottom + 20;
            this.lblStuNum.Top = this.lblStulName.Bottom + 20; ;
            this.lblStuType.Top = this.lblStuNum.Bottom + 20; ;
            this.lblUpan.Top = this.lblStuType.Bottom + 20;
        }
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Timer showTimer;
        private System.Windows.Forms.Label lblUpan;
        private System.Windows.Forms.Panel toppanel;
        private System.Windows.Forms.WebBrowser webBrowPictrue;
        private System.Windows.Forms.Panel CaozuoPanel;
        private System.Windows.Forms.Panel toumingPanel;
        private System.Windows.Forms.Button btnNarrowSize;
        private System.Windows.Forms.Label lbltouming;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.WebBrowser CaozuoBrowser;
        private System.Windows.Forms.Panel XuanTianPanel;
        private System.Windows.Forms.Label lblReminder;
        private System.Windows.Forms.Panel choiceDyPanel;
        private System.Windows.Forms.Panel RadioPanel;
        private System.Windows.Forms.RadioButton radioButtonD;
        private System.Windows.Forms.RadioButton radioButtonC;
        private System.Windows.Forms.RadioButton radioButtonB;
        private System.Windows.Forms.RadioButton radioButtonA;
        private System.Windows.Forms.Panel TianKongPanel;
        private System.Windows.Forms.TextBox txtStuBlank;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtTimu;
        private System.Windows.Forms.Panel panelSizeChange;
        private System.Windows.Forms.Button formMaxed;
    }
}
