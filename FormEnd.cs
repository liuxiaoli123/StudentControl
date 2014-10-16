using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Student
{
    public partial class EndForm : Form
    {
        private OperaForm opf;
        private Label hostnameLabel;
        public EndForm(OperaForm mainop)
        {
            opf = mainop;
            InitializeComponent();
            hostnameLabel = MyControl.MyLabel;
        }

        private void EndForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // 设置边框为 None
            this.WindowState = FormWindowState.Maximized; // 最大化
            this.hostnameLabel.Left = this.Left;
            this.hostnameLabel.Top = this.Top;
            this.hostnameLabel.Visible = true;
            this.Controls.Add(this.hostnameLabel);
            this.hostnameLabel.BringToFront();
            this.picEndExam.Left = this.hostnameLabel.Left + 50;
            this.picEndExam.Top = this.hostnameLabel.Bottom + 20;
            this.picEndExam.Width = this.Width - 100;
            this.picEndExam.Height = 220;
            this.picReminder.Left = this.hostnameLabel.Left + 200;
            this.picReminder.Top = this.picEndExam.Bottom + 100;
            this.picReminder.Width = this.Width - 400;
            this.picReminder.Height = 400;
        }
    }
}
