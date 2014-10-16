namespace Student
{
    partial class SplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            this.label_Name = new System.Windows.Forms.Label();
            this.label_version = new System.Windows.Forms.Label();
            this.label_copyright = new System.Windows.Forms.Label();
            this.label_date = new System.Windows.Forms.Label();
            this.label_status = new System.Windows.Forms.Label();
            this.lblHostname = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.BackColor = System.Drawing.Color.Transparent;
            this.label_Name.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Name.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_Name.Location = new System.Drawing.Point(69, 44);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(363, 62);
            this.label_Name.TabIndex = 1;
            this.label_Name.Text = "计算机考试系统";
            this.label_Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_version
            // 
            this.label_version.AutoSize = true;
            this.label_version.BackColor = System.Drawing.Color.Transparent;
            this.label_version.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_version.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_version.Location = new System.Drawing.Point(315, 106);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(82, 21);
            this.label_version.TabIndex = 2;
            this.label_version.Text = "版本 1.0.0";
            // 
            // label_copyright
            // 
            this.label_copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_copyright.AutoSize = true;
            this.label_copyright.BackColor = System.Drawing.Color.Transparent;
            this.label_copyright.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_copyright.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_copyright.Location = new System.Drawing.Point(397, 342);
            this.label_copyright.Name = "label_copyright";
            this.label_copyright.Size = new System.Drawing.Size(104, 17);
            this.label_copyright.TabIndex = 3;
            this.label_copyright.Text = "华东师大计算中心";
            this.label_copyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_date
            // 
            this.label_date.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_date.AutoSize = true;
            this.label_date.BackColor = System.Drawing.Color.Transparent;
            this.label_date.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_date.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_date.Location = new System.Drawing.Point(397, 359);
            this.label_date.Name = "label_date";
            this.label_date.Size = new System.Drawing.Size(74, 17);
            this.label_date.TabIndex = 4;
            this.label_date.Text = "2014-07-08";
            this.label_date.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.BackColor = System.Drawing.Color.Transparent;
            this.label_status.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.label_status.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_status.Location = new System.Drawing.Point(74, 222);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(225, 35);
            this.label_status.TabIndex = 5;
            this.label_status.Text = "正在初始化系统...";
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.BackColor = System.Drawing.Color.Transparent;
            this.lblHostname.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHostname.Font = new System.Drawing.Font("楷体", 36F, System.Drawing.FontStyle.Bold);
            this.lblHostname.ForeColor = System.Drawing.Color.Red;
            this.lblHostname.Location = new System.Drawing.Point(0, 0);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(95, 48);
            this.lblHostname.TabIndex = 0;
            this.lblHostname.Text = "STU";
            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 385);
            this.Controls.Add(this.lblHostname);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.label_date);
            this.Controls.Add(this.label_copyright);
            this.Controls.Add(this.label_version);
            this.Controls.Add(this.label_Name);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashForm";
            this.Text = "SplashForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.Label label_copyright;
        private System.Windows.Forms.Label label_date;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label lblHostname;
    }
}