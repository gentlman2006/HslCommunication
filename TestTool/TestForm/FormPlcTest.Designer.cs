namespace TestTool.TestForm
{
    partial class FormPlcTest
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.userButton4 = new HslCommunication.Controls.UserButton();
            this.userButton3 = new HslCommunication.Controls.UserButton();
            this.userButton2 = new HslCommunication.Controls.UserButton();
            this.userButton1 = new HslCommunication.Controls.UserButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.userButton5 = new HslCommunication.Controls.UserButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.userButton14 = new HslCommunication.Controls.UserButton();
            this.userButton13 = new HslCommunication.Controls.UserButton();
            this.userButton12 = new HslCommunication.Controls.UserButton();
            this.userButton11 = new HslCommunication.Controls.UserButton();
            this.userButton10 = new HslCommunication.Controls.UserButton();
            this.userButton6 = new HslCommunication.Controls.UserButton();
            this.userButton7 = new HslCommunication.Controls.UserButton();
            this.userButton8 = new HslCommunication.Controls.UserButton();
            this.userButton9 = new HslCommunication.Controls.UserButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.userButton15 = new HslCommunication.Controls.UserButton();
            this.userButton16 = new HslCommunication.Controls.UserButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(792, 236);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.userButton4);
            this.tabPage1.Controls.Add(this.userButton3);
            this.tabPage1.Controls.Add(this.userButton2);
            this.tabPage1.Controls.Add(this.userButton1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(784, 210);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "三菱PLC测试";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // userButton4
            // 
            this.userButton4.BackColor = System.Drawing.Color.Transparent;
            this.userButton4.CustomerInformation = "";
            this.userButton4.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton4.Location = new System.Drawing.Point(152, 43);
            this.userButton4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton4.Name = "userButton4";
            this.userButton4.Size = new System.Drawing.Size(140, 28);
            this.userButton4.TabIndex = 4;
            this.userButton4.UIText = "D100-D104写入";
            this.userButton4.Click += new System.EventHandler(this.userButton4_Click);
            // 
            // userButton3
            // 
            this.userButton3.BackColor = System.Drawing.Color.Transparent;
            this.userButton3.CustomerInformation = "";
            this.userButton3.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton3.Location = new System.Drawing.Point(6, 43);
            this.userButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton3.Name = "userButton3";
            this.userButton3.Size = new System.Drawing.Size(140, 28);
            this.userButton3.TabIndex = 3;
            this.userButton3.UIText = "M100-M104写入";
            this.userButton3.Click += new System.EventHandler(this.userButton3_Click);
            // 
            // userButton2
            // 
            this.userButton2.BackColor = System.Drawing.Color.Transparent;
            this.userButton2.CustomerInformation = "";
            this.userButton2.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton2.Location = new System.Drawing.Point(152, 7);
            this.userButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton2.Name = "userButton2";
            this.userButton2.Size = new System.Drawing.Size(140, 28);
            this.userButton2.TabIndex = 2;
            this.userButton2.UIText = "D100-D104读取";
            this.userButton2.Click += new System.EventHandler(this.userButton2_Click);
            // 
            // userButton1
            // 
            this.userButton1.BackColor = System.Drawing.Color.Transparent;
            this.userButton1.CustomerInformation = "";
            this.userButton1.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton1.Location = new System.Drawing.Point(6, 7);
            this.userButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton1.Name = "userButton1";
            this.userButton1.Size = new System.Drawing.Size(140, 28);
            this.userButton1.TabIndex = 1;
            this.userButton1.UIText = "M100-M104读取";
            this.userButton1.Click += new System.EventHandler(this.userButton1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.userButton5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(784, 210);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "西门子PLC测试类一";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // userButton5
            // 
            this.userButton5.BackColor = System.Drawing.Color.Transparent;
            this.userButton5.CustomerInformation = "";
            this.userButton5.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton5.Location = new System.Drawing.Point(8, 7);
            this.userButton5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton5.Name = "userButton5";
            this.userButton5.Size = new System.Drawing.Size(121, 25);
            this.userButton5.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.userButton16);
            this.tabPage3.Controls.Add(this.userButton15);
            this.tabPage3.Controls.Add(this.userButton14);
            this.tabPage3.Controls.Add(this.userButton13);
            this.tabPage3.Controls.Add(this.userButton12);
            this.tabPage3.Controls.Add(this.userButton11);
            this.tabPage3.Controls.Add(this.userButton10);
            this.tabPage3.Controls.Add(this.userButton6);
            this.tabPage3.Controls.Add(this.userButton7);
            this.tabPage3.Controls.Add(this.userButton8);
            this.tabPage3.Controls.Add(this.userButton9);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(784, 210);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "西门子S7协议";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // userButton14
            // 
            this.userButton14.BackColor = System.Drawing.Color.Transparent;
            this.userButton14.CustomerInformation = "";
            this.userButton14.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton14.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton14.Location = new System.Drawing.Point(152, 43);
            this.userButton14.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton14.Name = "userButton14";
            this.userButton14.Size = new System.Drawing.Size(140, 28);
            this.userButton14.TabIndex = 13;
            this.userButton14.UIText = "批量位写入测试";
            this.userButton14.Click += new System.EventHandler(this.userButton14_Click);
            // 
            // userButton13
            // 
            this.userButton13.BackColor = System.Drawing.Color.Transparent;
            this.userButton13.CustomerInformation = "";
            this.userButton13.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton13.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton13.Location = new System.Drawing.Point(300, 121);
            this.userButton13.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton13.Name = "userButton13";
            this.userButton13.Size = new System.Drawing.Size(140, 28);
            this.userButton13.TabIndex = 12;
            this.userButton13.UIText = "M116-M125读取";
            this.userButton13.Click += new System.EventHandler(this.userButton13_Click);
            // 
            // userButton12
            // 
            this.userButton12.BackColor = System.Drawing.Color.Transparent;
            this.userButton12.CustomerInformation = "";
            this.userButton12.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton12.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton12.Location = new System.Drawing.Point(300, 85);
            this.userButton12.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton12.Name = "userButton12";
            this.userButton12.Size = new System.Drawing.Size(140, 28);
            this.userButton12.TabIndex = 11;
            this.userButton12.UIText = "条码写入测试";
            this.userButton12.Click += new System.EventHandler(this.userButton12_Click);
            // 
            // userButton11
            // 
            this.userButton11.BackColor = System.Drawing.Color.Transparent;
            this.userButton11.CustomerInformation = "";
            this.userButton11.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton11.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton11.Location = new System.Drawing.Point(451, 43);
            this.userButton11.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton11.Name = "userButton11";
            this.userButton11.Size = new System.Drawing.Size(140, 28);
            this.userButton11.TabIndex = 10;
            this.userButton11.UIText = "1500测试";
            this.userButton11.Click += new System.EventHandler(this.userButton11_Click);
            // 
            // userButton10
            // 
            this.userButton10.BackColor = System.Drawing.Color.Transparent;
            this.userButton10.CustomerInformation = "";
            this.userButton10.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton10.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton10.Location = new System.Drawing.Point(451, 7);
            this.userButton10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton10.Name = "userButton10";
            this.userButton10.Size = new System.Drawing.Size(140, 28);
            this.userButton10.TabIndex = 9;
            this.userButton10.UIText = "散乱数组读取";
            this.userButton10.Click += new System.EventHandler(this.userButton10_Click);
            // 
            // userButton6
            // 
            this.userButton6.BackColor = System.Drawing.Color.Transparent;
            this.userButton6.CustomerInformation = "";
            this.userButton6.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton6.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton6.Location = new System.Drawing.Point(152, 7);
            this.userButton6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton6.Name = "userButton6";
            this.userButton6.Size = new System.Drawing.Size(140, 28);
            this.userButton6.TabIndex = 8;
            this.userButton6.UIText = "位写入测试";
            this.userButton6.Click += new System.EventHandler(this.userButton6_Click);
            // 
            // userButton7
            // 
            this.userButton7.BackColor = System.Drawing.Color.Transparent;
            this.userButton7.CustomerInformation = "";
            this.userButton7.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton7.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton7.Location = new System.Drawing.Point(6, 43);
            this.userButton7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton7.Name = "userButton7";
            this.userButton7.Size = new System.Drawing.Size(140, 28);
            this.userButton7.TabIndex = 7;
            this.userButton7.UIText = "M100-M104写入";
            this.userButton7.Click += new System.EventHandler(this.userButton7_Click);
            // 
            // userButton8
            // 
            this.userButton8.BackColor = System.Drawing.Color.Transparent;
            this.userButton8.CustomerInformation = "";
            this.userButton8.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton8.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton8.Location = new System.Drawing.Point(85, 113);
            this.userButton8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton8.Name = "userButton8";
            this.userButton8.Size = new System.Drawing.Size(140, 28);
            this.userButton8.TabIndex = 6;
            this.userButton8.UIText = "D100-D104读取";
            // 
            // userButton9
            // 
            this.userButton9.BackColor = System.Drawing.Color.Transparent;
            this.userButton9.CustomerInformation = "";
            this.userButton9.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton9.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton9.Location = new System.Drawing.Point(6, 7);
            this.userButton9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton9.Name = "userButton9";
            this.userButton9.Size = new System.Drawing.Size(140, 28);
            this.userButton9.TabIndex = 5;
            this.userButton9.UIText = "M100-M104读取";
            this.userButton9.Click += new System.EventHandler(this.userButton9_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 242);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(792, 237);
            this.textBox1.TabIndex = 1;
            // 
            // userButton15
            // 
            this.userButton15.BackColor = System.Drawing.Color.Transparent;
            this.userButton15.CustomerInformation = "";
            this.userButton15.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton15.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton15.Location = new System.Drawing.Point(597, 7);
            this.userButton15.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton15.Name = "userButton15";
            this.userButton15.Size = new System.Drawing.Size(140, 28);
            this.userButton15.TabIndex = 14;
            this.userButton15.UIText = "批量散乱数组读取";
            this.userButton15.Click += new System.EventHandler(this.userButton15_Click);
            // 
            // userButton16
            // 
            this.userButton16.BackColor = System.Drawing.Color.Transparent;
            this.userButton16.CustomerInformation = "";
            this.userButton16.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton16.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton16.Location = new System.Drawing.Point(8, 175);
            this.userButton16.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton16.Name = "userButton16";
            this.userButton16.Size = new System.Drawing.Size(140, 28);
            this.userButton16.TabIndex = 15;
            this.userButton16.UIText = "开启共享模式";
            this.userButton16.Click += new System.EventHandler(this.userButton16_Click);
            // 
            // FormPlcTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 479);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormPlcTest";
            this.Text = "FormPlcTest";
            this.Load += new System.EventHandler(this.FormPlcTest_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private HslCommunication.Controls.UserButton userButton1;
        private HslCommunication.Controls.UserButton userButton4;
        private HslCommunication.Controls.UserButton userButton3;
        private HslCommunication.Controls.UserButton userButton2;
        private HslCommunication.Controls.UserButton userButton5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private HslCommunication.Controls.UserButton userButton6;
        private HslCommunication.Controls.UserButton userButton7;
        private HslCommunication.Controls.UserButton userButton8;
        private HslCommunication.Controls.UserButton userButton9;
        private HslCommunication.Controls.UserButton userButton10;
        private HslCommunication.Controls.UserButton userButton11;
        private HslCommunication.Controls.UserButton userButton12;
        private HslCommunication.Controls.UserButton userButton13;
        private HslCommunication.Controls.UserButton userButton14;
        private HslCommunication.Controls.UserButton userButton15;
        private HslCommunication.Controls.UserButton userButton16;
    }
}