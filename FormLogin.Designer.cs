namespace Student
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.centralPanel = new System.Windows.Forms.Panel();
            this.lblStuNum = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblStuName = new System.Windows.Forms.Label();
            this.lblCourseID = new System.Windows.Forms.Label();
            this.lblCourID = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.btnexit = new System.Windows.Forms.Button();
            this.webBrowPicture = new System.Windows.Forms.WebBrowser();
            this.centralPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // centralPanel
            // 
            this.centralPanel.BackColor = System.Drawing.Color.Transparent;
            this.centralPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.centralPanel.Controls.Add(this.lblStuNum);
            this.centralPanel.Controls.Add(this.btnReset);
            this.centralPanel.Controls.Add(this.lblClass);
            this.centralPanel.Controls.Add(this.lblName);
            this.centralPanel.Controls.Add(this.label4);
            this.centralPanel.Controls.Add(this.lblStuName);
            this.centralPanel.Controls.Add(this.lblCourseID);
            this.centralPanel.Controls.Add(this.lblCourID);
            this.centralPanel.Controls.Add(this.btnLogin);
            this.centralPanel.Controls.Add(this.txtNumber);
            this.centralPanel.Location = new System.Drawing.Point(228, 184);
            this.centralPanel.Name = "centralPanel";
            this.centralPanel.Size = new System.Drawing.Size(624, 296);
            this.centralPanel.TabIndex = 0;
            // 
            // lblStuNum
            // 
            this.lblStuNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStuNum.AutoSize = true;
            this.lblStuNum.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStuNum.ForeColor = System.Drawing.Color.White;
            this.lblStuNum.Location = new System.Drawing.Point(6, 67);
            this.lblStuNum.Name = "lblStuNum";
            this.lblStuNum.Size = new System.Drawing.Size(162, 38);
            this.lblStuNum.TabIndex = 4;
            this.lblStuNum.Text = "考生学号：";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.BackColor = System.Drawing.Color.Transparent;
            this.btnReset.BackgroundImage = global::StudentControll.Properties.Resources.登录背景;
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnReset.ForeColor = System.Drawing.Color.Tomato;
            this.btnReset.Location = new System.Drawing.Point(238, 247);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(118, 46);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblClass
            // 
            this.lblClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblClass.AutoSize = true;
            this.lblClass.BackColor = System.Drawing.Color.Transparent;
            this.lblClass.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.lblClass.ForeColor = System.Drawing.Color.White;
            this.lblClass.Location = new System.Drawing.Point(238, 167);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(112, 38);
            this.lblClass.TabIndex = 2;
            this.lblClass.Text = "XXXXX";
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(238, 117);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(112, 38);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "XXXXX";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(162, 38);
            this.label4.TabIndex = 4;
            this.label4.Text = "所在院系：";
            // 
            // lblStuName
            // 
            this.lblStuName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStuName.AutoSize = true;
            this.lblStuName.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.lblStuName.ForeColor = System.Drawing.Color.White;
            this.lblStuName.Location = new System.Drawing.Point(6, 117);
            this.lblStuName.Name = "lblStuName";
            this.lblStuName.Size = new System.Drawing.Size(162, 38);
            this.lblStuName.TabIndex = 5;
            this.lblStuName.Text = "考生姓名：";
            // 
            // lblCourseID
            // 
            this.lblCourseID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCourseID.AutoSize = true;
            this.lblCourseID.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCourseID.ForeColor = System.Drawing.Color.White;
            this.lblCourseID.Location = new System.Drawing.Point(6, 17);
            this.lblCourseID.Name = "lblCourseID";
            this.lblCourseID.Size = new System.Drawing.Size(162, 38);
            this.lblCourseID.TabIndex = 6;
            this.lblCourseID.Text = "考试科目：";
            // 
            // lblCourID
            // 
            this.lblCourID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCourID.AutoSize = true;
            this.lblCourID.BackColor = System.Drawing.Color.Transparent;
            this.lblCourID.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.lblCourID.ForeColor = System.Drawing.Color.White;
            this.lblCourID.Location = new System.Drawing.Point(238, 17);
            this.lblCourID.Name = "lblCourID";
            this.lblCourID.Size = new System.Drawing.Size(112, 38);
            this.lblCourID.TabIndex = 7;
            this.lblCourID.Text = "XXXXX";
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.BackgroundImage = global::StudentControll.Properties.Resources.登录背景;
            this.btnLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnLogin.ForeColor = System.Drawing.Color.Tomato;
            this.btnLogin.Location = new System.Drawing.Point(98, 247);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(118, 46);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "登陆";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.stulogin_Click);
            // 
            // txtNumber
            // 
            this.txtNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtNumber.BackColor = System.Drawing.Color.White;
            this.txtNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNumber.Font = new System.Drawing.Font("微软雅黑", 21.75F);
            this.txtNumber.ForeColor = System.Drawing.Color.Black;
            this.txtNumber.Location = new System.Drawing.Point(238, 67);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(234, 39);
            this.txtNumber.TabIndex = 0;
            this.txtNumber.TextChanged += new System.EventHandler(this.txt_name_TextChanged);
            this.txtNumber.MouseLeave += new System.EventHandler(this.txtNumber_MouseLeave);
            // 
            // btnexit
            // 
            this.btnexit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnexit.BackColor = System.Drawing.Color.Transparent;
            this.btnexit.BackgroundImage = global::StudentControll.Properties.Resources.登录背景;
            this.btnexit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnexit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnexit.FlatAppearance.BorderSize = 0;
            this.btnexit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnexit.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.btnexit.ForeColor = System.Drawing.Color.Red;
            this.btnexit.Location = new System.Drawing.Point(851, 559);
            this.btnexit.Name = "btnexit";
            this.btnexit.Size = new System.Drawing.Size(118, 46);
            this.btnexit.TabIndex = 1;
            this.btnexit.Text = "退出";
            this.btnexit.UseVisualStyleBackColor = false;
            this.btnexit.Click += new System.EventHandler(this.btnexit_Click);
            // 
            // webBrowPicture
            // 
            this.webBrowPicture.Location = new System.Drawing.Point(52, 184);
            this.webBrowPicture.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowPicture.Name = "webBrowPicture";
            this.webBrowPicture.ScrollBarsEnabled = false;
            this.webBrowPicture.Size = new System.Drawing.Size(176, 219);
            this.webBrowPicture.TabIndex = 2;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(969, 606);
            this.ControlBox = false;
            this.Controls.Add(this.btnexit);
            this.Controls.Add(this.webBrowPicture);
            this.Controls.Add(this.centralPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.centralPanel.ResumeLayout(false);
            this.centralPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel centralPanel;
        private System.Windows.Forms.Label lblStuNum;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblStuName;
        private System.Windows.Forms.Label lblCourseID;
        private System.Windows.Forms.Label lblCourID;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Button btnexit;
        private System.Windows.Forms.WebBrowser webBrowPicture;

    }
}