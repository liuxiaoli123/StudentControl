namespace Student
{
    partial class TestInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestInfoForm));
            this.lblTestRemainder = new System.Windows.Forms.Label();
            this.webBrowTest = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // lblTestRemainder
            // 
            this.lblTestRemainder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblTestRemainder.AutoSize = true;
            this.lblTestRemainder.Font = new System.Drawing.Font("黑体", 15F);
            this.lblTestRemainder.Location = new System.Drawing.Point(201, 38);
            this.lblTestRemainder.Name = "lblTestRemainder";
            this.lblTestRemainder.Size = new System.Drawing.Size(89, 20);
            this.lblTestRemainder.TabIndex = 2;
            this.lblTestRemainder.Text = "考试需知";
            // 
            // webBrowTest
            // 
            this.webBrowTest.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.webBrowTest.Location = new System.Drawing.Point(46, 99);
            this.webBrowTest.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowTest.Name = "webBrowTest";
            this.webBrowTest.Size = new System.Drawing.Size(389, 167);
            this.webBrowTest.TabIndex = 3;
            // 
            // TestInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 336);
            this.ControlBox = false;
            this.Controls.Add(this.lblTestRemainder);
            this.Controls.Add(this.webBrowTest);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestInfoForm";
            this.Text = "TestInfoForm";
            this.Load += new System.EventHandler(this.TestInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTestRemainder;
        private System.Windows.Forms.WebBrowser webBrowTest;

    }
}