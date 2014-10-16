namespace Student
{
    partial class EndForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EndForm));
            this.picEndExam = new System.Windows.Forms.PictureBox();
            this.picReminder = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picEndExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReminder)).BeginInit();
            this.SuspendLayout();
            // 
            // picEndExam
            // 
            this.picEndExam.Image = ((System.Drawing.Image)(resources.GetObject("picEndExam.Image")));
            this.picEndExam.InitialImage = null;
            this.picEndExam.Location = new System.Drawing.Point(4, 12);
            this.picEndExam.Name = "picEndExam";
            this.picEndExam.Size = new System.Drawing.Size(532, 102);
            this.picEndExam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picEndExam.TabIndex = 1;
            this.picEndExam.TabStop = false;
            // 
            // picReminder
            // 
            this.picReminder.Image = ((System.Drawing.Image)(resources.GetObject("picReminder.Image")));
            this.picReminder.Location = new System.Drawing.Point(12, 138);
            this.picReminder.Name = "picReminder";
            this.picReminder.Size = new System.Drawing.Size(492, 236);
            this.picReminder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picReminder.TabIndex = 2;
            this.picReminder.TabStop = false;
            // 
            // EndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 386);
            this.Controls.Add(this.picReminder);
            this.Controls.Add(this.picEndExam);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EndForm";
            this.Text = "EndForm";
            this.Load += new System.EventHandler(this.EndForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picEndExam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReminder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picEndExam;
        private System.Windows.Forms.PictureBox picReminder;
    }
}