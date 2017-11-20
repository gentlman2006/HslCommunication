namespace TestTool.TestForm
{
    partial class FormFileTest
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
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("文件列表");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.userButton1 = new HslCommunication.Controls.UserButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.userButton3 = new HslCommunication.Controls.UserButton();
            this.userButton2 = new HslCommunication.Controls.UserButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.userButton5 = new HslCommunication.Controls.UserButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.userButton4 = new HslCommunication.Controls.UserButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.userButton1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 533);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器端";
            // 
            // userButton1
            // 
            this.userButton1.BackColor = System.Drawing.Color.Transparent;
            this.userButton1.CustomerInformation = "";
            this.userButton1.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton1.Location = new System.Drawing.Point(6, 21);
            this.userButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton1.Name = "userButton1";
            this.userButton1.Size = new System.Drawing.Size(183, 36);
            this.userButton1.TabIndex = 1;
            this.userButton1.UIText = "启动文件引擎";
            this.userButton1.Click += new System.EventHandler(this.userButton1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.userButton3);
            this.groupBox2.Controls.Add(this.userButton2);
            this.groupBox2.Location = new System.Drawing.Point(216, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(678, 109);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "上传文件块";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 79);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(661, 17);
            this.progressBar1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "上传进度";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(11, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(489, 26);
            this.textBox1.TabIndex = 4;
            // 
            // userButton3
            // 
            this.userButton3.BackColor = System.Drawing.Color.Transparent;
            this.userButton3.CustomerInformation = "";
            this.userButton3.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton3.Location = new System.Drawing.Point(506, 21);
            this.userButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton3.Name = "userButton3";
            this.userButton3.Size = new System.Drawing.Size(80, 28);
            this.userButton3.TabIndex = 3;
            this.userButton3.UIText = "选择";
            this.userButton3.Click += new System.EventHandler(this.userButton3_Click);
            // 
            // userButton2
            // 
            this.userButton2.BackColor = System.Drawing.Color.Transparent;
            this.userButton2.CustomerInformation = "";
            this.userButton2.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton2.Location = new System.Drawing.Point(592, 21);
            this.userButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton2.Name = "userButton2";
            this.userButton2.Size = new System.Drawing.Size(80, 28);
            this.userButton2.TabIndex = 2;
            this.userButton2.UIText = "上传";
            this.userButton2.Click += new System.EventHandler(this.userButton2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.progressBar2);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.userButton5);
            this.groupBox3.Location = new System.Drawing.Point(216, 127);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(678, 106);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "下载文件块";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(11, 77);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(661, 17);
            this.progressBar2.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "下载进度";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(11, 20);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(575, 26);
            this.textBox2.TabIndex = 9;
            // 
            // userButton5
            // 
            this.userButton5.BackColor = System.Drawing.Color.Transparent;
            this.userButton5.CustomerInformation = "";
            this.userButton5.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton5.Location = new System.Drawing.Point(592, 19);
            this.userButton5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton5.Name = "userButton5";
            this.userButton5.Size = new System.Drawing.Size(80, 28);
            this.userButton5.TabIndex = 7;
            this.userButton5.UIText = "下载";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.textBox4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.userButton4);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.treeView1);
            this.groupBox4.Location = new System.Drawing.Point(213, 239);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(681, 306);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "获取文件信息块";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(259, 52);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(416, 248);
            this.textBox3.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 52);
            this.treeView1.Name = "treeView1";
            treeNode4.Name = "节点0";
            treeNode4.Text = "文件列表";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.treeView1.Size = new System.Drawing.Size(247, 248);
            this.treeView1.TabIndex = 0;
            // 
            // userButton4
            // 
            this.userButton4.BackColor = System.Drawing.Color.Transparent;
            this.userButton4.CustomerInformation = "";
            this.userButton4.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton4.Location = new System.Drawing.Point(571, 17);
            this.userButton4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton4.Name = "userButton4";
            this.userButton4.Size = new System.Drawing.Size(104, 28);
            this.userButton4.TabIndex = 8;
            this.userButton4.UIText = "获取文件列表";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "第一级分类：";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(78, 19);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(103, 21);
            this.textBox4.TabIndex = 10;
            this.textBox4.Text = "Files";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(261, 19);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(103, 21);
            this.textBox5.TabIndex = 12;
            this.textBox5.Text = "Personal";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(186, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "第一级分类：";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(440, 19);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(103, 21);
            this.textBox6.TabIndex = 14;
            this.textBox6.Text = "Admin";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(370, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "第一级分类：";
            // 
            // FormFileTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 557);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormFileTest";
            this.Text = "FormFileTest";
            this.Load += new System.EventHandler(this.FormFileTest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private HslCommunication.Controls.UserButton userButton1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private HslCommunication.Controls.UserButton userButton3;
        private HslCommunication.Controls.UserButton userButton2;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private HslCommunication.Controls.UserButton userButton5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        private HslCommunication.Controls.UserButton userButton4;
    }
}