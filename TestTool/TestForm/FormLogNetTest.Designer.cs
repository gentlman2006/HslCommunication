namespace TestTool.TestForm
{
    partial class FormLogNetTest
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
            this.logNetAnalysisControl1 = new HslCommunication.LogNet.LogNetAnalysisControl();
            this.userButton1 = new HslCommunication.Controls.UserButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // logNetAnalysisControl1
            // 
            this.logNetAnalysisControl1.Location = new System.Drawing.Point(3, 12);
            this.logNetAnalysisControl1.Name = "logNetAnalysisControl1";
            this.logNetAnalysisControl1.Size = new System.Drawing.Size(778, 283);
            this.logNetAnalysisControl1.TabIndex = 0;
            // 
            // userButton1
            // 
            this.userButton1.BackColor = System.Drawing.Color.Transparent;
            this.userButton1.CustomerInformation = "";
            this.userButton1.EnableColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.userButton1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.userButton1.Location = new System.Drawing.Point(787, 13);
            this.userButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userButton1.Name = "userButton1";
            this.userButton1.Size = new System.Drawing.Size(110, 25);
            this.userButton1.TabIndex = 1;
            this.userButton1.UIText = "测试";
            this.userButton1.Click += new System.EventHandler(this.userButton1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 309);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(772, 243);
            this.textBox1.TabIndex = 2;
            // 
            // FormLogNetTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 564);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.userButton1);
            this.Controls.Add(this.logNetAnalysisControl1);
            this.Name = "FormLogNetTest";
            this.Text = "FormLogNetTest";
            this.Load += new System.EventHandler(this.FormLogNetTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HslCommunication.LogNet.LogNetAnalysisControl logNetAnalysisControl1;
        private HslCommunication.Controls.UserButton userButton1;
        private System.Windows.Forms.TextBox textBox1;
    }
}