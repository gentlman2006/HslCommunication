namespace HslCommunication.BasicFramework
{
    partial class FormSupport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 392);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(608, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "如果这个组件真的帮到了你或你们公司，那么非常感谢您的支持，个人打赏，请视个人能力选择金额，感谢支持。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 484);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(591, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "如果您的公司使用了本产品，那么非常感谢对本产品的信任，企业赞助或是合作请专门联系VIP群，或是作者。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(340, 518);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(363, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "作者：Richard.Hu 上图为支付宝和微信账户的收钱码，金额自定义";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(32, 411);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(667, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "如果不小心点错了，需要退款，请通过支付宝或是本组件的交流群联系作者，群号：592132877";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::HslCommunication.Properties.Resources.mm_facetoface_collect_qrcode_1525331158525;
            this.pictureBox2.Location = new System.Drawing.Point(375, 10);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(283, 375);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HslCommunication.Properties.Resources.alipay;
            this.pictureBox1.Location = new System.Drawing.Point(35, 10);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(290, 375);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(32, 430);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(334, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "打赏超过100 Rmb的，可以申请VIP技术支持群：838185568";
            // 
            // FormSupport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(710, 542);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormSupport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "开源不易，如果您使用了本类库，感谢赞助";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label6;
    }
}