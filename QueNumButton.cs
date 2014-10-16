using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Drawing;
using System.ComponentModel;

namespace Student
{
    class QueNumButton:Button
    {
        public enum
            StatusType
        {
            NoSelected,
            NotSure,
            SuccessedSelected
        };

        private StatusType status;
        private int questionNo;
        /// <summary>
        /// 纪录每道题目号
        /// </summary>
        public int QuestionNo
        {
            get { return questionNo; }
            set { questionNo = value; }
        }
        /// <summary>
        /// 题目号状态标志
        /// </summary>
        public StatusType Status
        {
            get { return status; }
            set 
            {
                status = value;
                if (value == StatusType.NoSelected)
                    this.BackColor = System.Drawing.SystemColors.Control;
                else if (value == StatusType.NotSure)
                    this.BackColor = System.Drawing.SystemColors.ActiveBorder;
                else if (value == StatusType.SuccessedSelected)
                    this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            }
        }

        public QueNumButton()
        {
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.Visible = true;
            this.Status = StatusType.NoSelected;
            
        }
    }
}
