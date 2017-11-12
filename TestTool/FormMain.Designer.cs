namespace TestTool
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.userButton1 = new HslCommunication.Controls.UserButton();
            this.userButton2 = new HslCommunication.Controls.UserButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.userButton3 = new HslCommunication.Controls.UserButton();
            this.userButton4 = new HslCommunication.Controls.UserButton();
            this.userButton5 = new HslCommunication.Controls.UserButton();
            this.userButton6 = new HslCommunication.Controls.UserButton();
            this.userButton7 = new HslCommunication.Controls.UserButton();
            this.SuspendLayout();
            // 
            // userButton1
            // 
            this.userButton1.BackColor = System.Drawing.Color.Transparent;
            this.userButton1.CustomerInformation = "";
            this.userButton1.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton1.Location = new System.Drawing.Point(12, 13);
            this.userButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton1.Name = "userButton1";
            this.userButton1.Size = new System.Drawing.Size(143, 33);
            this.userButton1.TabIndex = 0;
            this.userButton1.UIText = "PLC网络测试";
            this.userButton1.Click += new System.EventHandler(this.userButton1_Click);
            // 
            // userButton2
            // 
            this.userButton2.BackColor = System.Drawing.Color.Transparent;
            this.userButton2.CustomerInformation = "";
            this.userButton2.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton2.Location = new System.Drawing.Point(12, 472);
            this.userButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton2.Name = "userButton2";
            this.userButton2.Size = new System.Drawing.Size(143, 33);
            this.userButton2.TabIndex = 1;
            this.userButton2.UIText = "命令头测试";
            this.userButton2.Click += new System.EventHandler(this.userButton2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(12, 512);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(767, 30);
            this.textBox1.TabIndex = 2;
            // 
            // userButton3
            // 
            this.userButton3.BackColor = System.Drawing.Color.Transparent;
            this.userButton3.CustomerInformation = "";
            this.userButton3.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton3.Location = new System.Drawing.Point(161, 13);
            this.userButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton3.Name = "userButton3";
            this.userButton3.Size = new System.Drawing.Size(143, 33);
            this.userButton3.TabIndex = 3;
            this.userButton3.UIText = "ModBus Tcp";
            this.userButton3.Click += new System.EventHandler(this.userButton3_Click);
            // 
            // userButton4
            // 
            this.userButton4.BackColor = System.Drawing.Color.Transparent;
            this.userButton4.CustomerInformation = "";
            this.userButton4.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton4.Location = new System.Drawing.Point(161, 472);
            this.userButton4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton4.Name = "userButton4";
            this.userButton4.Size = new System.Drawing.Size(143, 33);
            this.userButton4.TabIndex = 4;
            this.userButton4.UIText = "时间测试";
            this.userButton4.Click += new System.EventHandler(this.userButton4_Click);
            // 
            // userButton5
            // 
            this.userButton5.BackColor = System.Drawing.Color.Transparent;
            this.userButton5.CustomerInformation = "";
            this.userButton5.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton5.Location = new System.Drawing.Point(310, 13);
            this.userButton5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton5.Name = "userButton5";
            this.userButton5.Size = new System.Drawing.Size(143, 33);
            this.userButton5.TabIndex = 5;
            this.userButton5.UIText = "JSON测试";
            this.userButton5.Click += new System.EventHandler(this.userButton5_Click);
            // 
            // userButton6
            // 
            this.userButton6.BackColor = System.Drawing.Color.Transparent;
            this.userButton6.CustomerInformation = "";
            this.userButton6.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton6.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton6.Location = new System.Drawing.Point(12, 178);
            this.userButton6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton6.Name = "userButton6";
            this.userButton6.Size = new System.Drawing.Size(143, 33);
            this.userButton6.TabIndex = 6;
            this.userButton6.UIText = "流水号生成器";
            this.userButton6.Click += new System.EventHandler(this.userButton6_Click);
            // 
            // userButton7
            // 
            this.userButton7.BackColor = System.Drawing.Color.Transparent;
            this.userButton7.CustomerInformation = "";
            this.userButton7.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton7.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton7.Location = new System.Drawing.Point(161, 178);
            this.userButton7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton7.Name = "userButton7";
            this.userButton7.Size = new System.Drawing.Size(143, 33);
            this.userButton7.TabIndex = 7;
            this.userButton7.UIText = "CRC校验";
            this.userButton7.Click += new System.EventHandler(this.userButton7_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 554);
            this.Controls.Add(this.userButton7);
            this.Controls.Add(this.userButton6);
            this.Controls.Add(this.userButton5);
            this.Controls.Add(this.userButton4);
            this.Controls.Add(this.userButton3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.userButton2);
            this.Controls.Add(this.userButton1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HslCommunication.Controls.UserButton userButton1;
        private HslCommunication.Controls.UserButton userButton2;
        private System.Windows.Forms.TextBox textBox1;
        private HslCommunication.Controls.UserButton userButton3;
        private HslCommunication.Controls.UserButton userButton4;
        private HslCommunication.Controls.UserButton userButton5;
        private HslCommunication.Controls.UserButton userButton6;
        private HslCommunication.Controls.UserButton userButton7;
    }
}

