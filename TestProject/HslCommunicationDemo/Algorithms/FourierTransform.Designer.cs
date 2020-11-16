﻿namespace HslCommunicationDemo.Algorithms
{
    partial class FourierTransform
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
            this.label20 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.userButton1 = new HslCommunication.Controls.UserButton();
            this.userCurve5 = new HslCommunication.Controls.UserCurve();
            this.userCurve6 = new HslCommunication.Controls.UserCurve();
            this.userCurve3 = new HslCommunication.Controls.UserCurve();
            this.userCurve4 = new HslCommunication.Controls.UserCurve();
            this.userCurve2 = new HslCommunication.Controls.UserCurve();
            this.userCurve1 = new HslCommunication.Controls.UserCurve();
            this.userButton2 = new HslCommunication.Controls.UserButton();
            this.userButton3 = new HslCommunication.Controls.UserButton();
            this.SuspendLayout();
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.Green;
            this.label20.Location = new System.Drawing.Point(888, 3);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(108, 17);
            this.label20.TabIndex = 11;
            this.label20.Text = "作者：Richard Hu";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(541, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "FFT 快速离散傅立叶变换";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(467, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "使用协议：";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(75, 3);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(287, 17);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.cnblogs.com/dathlin/p/7885368.html";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "博客地址：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "方波及变换后的波形";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "正弦波及变换后的波形";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 445);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 17);
            this.label6.TabIndex = 20;
            this.label6.Text = "混合波及变换后的波形";
            // 
            // userButton1
            // 
            this.userButton1.BackColor = System.Drawing.Color.Transparent;
            this.userButton1.CustomerInformation = "";
            this.userButton1.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton1.Location = new System.Drawing.Point(918, 25);
            this.userButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton1.Name = "userButton1";
            this.userButton1.Size = new System.Drawing.Size(78, 25);
            this.userButton1.TabIndex = 21;
            this.userButton1.UIText = "专用图形";
            this.userButton1.Click += new System.EventHandler(this.userButton1_Click);
            // 
            // userCurve5
            // 
            this.userCurve5.BackColor = System.Drawing.Color.Transparent;
            this.userCurve5.IsAbscissaStrech = true;
            this.userCurve5.Location = new System.Drawing.Point(510, 459);
            this.userCurve5.Margin = new System.Windows.Forms.Padding(3, 11, 3, 11);
            this.userCurve5.Name = "userCurve5";
            this.userCurve5.Size = new System.Drawing.Size(463, 190);
            this.userCurve5.StrechDataCountMax = 256;
            this.userCurve5.TabIndex = 19;
            // 
            // userCurve6
            // 
            this.userCurve6.BackColor = System.Drawing.Color.Transparent;
            this.userCurve6.IsAbscissaStrech = true;
            this.userCurve6.Location = new System.Drawing.Point(24, 459);
            this.userCurve6.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.userCurve6.Name = "userCurve6";
            this.userCurve6.Size = new System.Drawing.Size(463, 190);
            this.userCurve6.StrechDataCountMax = 256;
            this.userCurve6.TabIndex = 18;
            this.userCurve6.ValueMaxLeft = 10F;
            this.userCurve6.ValueMaxRight = 10F;
            // 
            // userCurve3
            // 
            this.userCurve3.BackColor = System.Drawing.Color.Transparent;
            this.userCurve3.IsAbscissaStrech = true;
            this.userCurve3.Location = new System.Drawing.Point(510, 255);
            this.userCurve3.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.userCurve3.Name = "userCurve3";
            this.userCurve3.Size = new System.Drawing.Size(463, 190);
            this.userCurve3.StrechDataCountMax = 256;
            this.userCurve3.TabIndex = 16;
            // 
            // userCurve4
            // 
            this.userCurve4.BackColor = System.Drawing.Color.Transparent;
            this.userCurve4.IsAbscissaStrech = true;
            this.userCurve4.Location = new System.Drawing.Point(24, 255);
            this.userCurve4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.userCurve4.Name = "userCurve4";
            this.userCurve4.Size = new System.Drawing.Size(463, 190);
            this.userCurve4.StrechDataCountMax = 256;
            this.userCurve4.TabIndex = 15;
            this.userCurve4.ValueMaxLeft = 10F;
            this.userCurve4.ValueMaxRight = 10F;
            // 
            // userCurve2
            // 
            this.userCurve2.BackColor = System.Drawing.Color.Transparent;
            this.userCurve2.IsAbscissaStrech = true;
            this.userCurve2.Location = new System.Drawing.Point(510, 43);
            this.userCurve2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.userCurve2.Name = "userCurve2";
            this.userCurve2.Size = new System.Drawing.Size(463, 190);
            this.userCurve2.StrechDataCountMax = 256;
            this.userCurve2.TabIndex = 13;
            // 
            // userCurve1
            // 
            this.userCurve1.BackColor = System.Drawing.Color.Transparent;
            this.userCurve1.IsAbscissaStrech = true;
            this.userCurve1.Location = new System.Drawing.Point(24, 43);
            this.userCurve1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userCurve1.Name = "userCurve1";
            this.userCurve1.Size = new System.Drawing.Size(463, 190);
            this.userCurve1.StrechDataCountMax = 256;
            this.userCurve1.TabIndex = 12;
            this.userCurve1.ValueMaxLeft = 10F;
            this.userCurve1.ValueMaxRight = 10F;
            // 
            // userButton2
            // 
            this.userButton2.BackColor = System.Drawing.Color.Transparent;
            this.userButton2.CustomerInformation = "";
            this.userButton2.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton2.Location = new System.Drawing.Point(918, 236);
            this.userButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton2.Name = "userButton2";
            this.userButton2.Size = new System.Drawing.Size(78, 25);
            this.userButton2.TabIndex = 22;
            this.userButton2.UIText = "专用图形";
            this.userButton2.Click += new System.EventHandler(this.userButton2_Click);
            // 
            // userButton3
            // 
            this.userButton3.BackColor = System.Drawing.Color.Transparent;
            this.userButton3.CustomerInformation = "";
            this.userButton3.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton3.Location = new System.Drawing.Point(918, 437);
            this.userButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton3.Name = "userButton3";
            this.userButton3.Size = new System.Drawing.Size(78, 25);
            this.userButton3.TabIndex = 23;
            this.userButton3.UIText = "专用图形";
            this.userButton3.Click += new System.EventHandler(this.userButton3_Click);
            // 
            // FourierTransform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 645);
            this.Controls.Add(this.userButton3);
            this.Controls.Add(this.userButton2);
            this.Controls.Add(this.userButton1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.userCurve5);
            this.Controls.Add(this.userCurve6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.userCurve3);
            this.Controls.Add(this.userCurve4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.userCurve2);
            this.Controls.Add(this.userCurve1);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FourierTransform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "傅立叶变换";
            this.Load += new System.EventHandler(this.傅立叶变换_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private HslCommunication.Controls.UserCurve userCurve1;
        private HslCommunication.Controls.UserCurve userCurve2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private HslCommunication.Controls.UserCurve userCurve3;
        private HslCommunication.Controls.UserCurve userCurve4;
        private System.Windows.Forms.Label label6;
        private HslCommunication.Controls.UserCurve userCurve5;
        private HslCommunication.Controls.UserCurve userCurve6;
        private HslCommunication.Controls.UserButton userButton1;
        private HslCommunication.Controls.UserButton userButton2;
        private HslCommunication.Controls.UserButton userButton3;
    }
}