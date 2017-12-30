namespace TestTool.TestForm
{
    partial class FormControls
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
            this.userGaugeChart1 = new HslCommunication.Controls.UserGaugeChart();
            this.userPieChart1 = new HslCommunication.Controls.UserPieChart();
            this.SuspendLayout();
            // 
            // userGaugeChart1
            // 
            this.userGaugeChart1.BackColor = System.Drawing.Color.Transparent;
            this.userGaugeChart1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userGaugeChart1.Location = new System.Drawing.Point(35, 37);
            this.userGaugeChart1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userGaugeChart1.Name = "userGaugeChart1";
            this.userGaugeChart1.Size = new System.Drawing.Size(450, 263);
            this.userGaugeChart1.TabIndex = 0;
            // 
            // userPieChart1
            // 
            this.userPieChart1.BackColor = System.Drawing.Color.Transparent;
            this.userPieChart1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userPieChart1.Location = new System.Drawing.Point(448, 414);
            this.userPieChart1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.userPieChart1.Name = "userPieChart1";
            this.userPieChart1.Size = new System.Drawing.Size(200, 200);
            this.userPieChart1.TabIndex = 1;
            // 
            // FormControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 627);
            this.Controls.Add(this.userPieChart1);
            this.Controls.Add(this.userGaugeChart1);
            this.Name = "FormControls";
            this.Text = "FormControls";
            this.ResumeLayout(false);

        }

        #endregion

        private HslCommunication.Controls.UserGaugeChart userGaugeChart1;
        private HslCommunication.Controls.UserPieChart userPieChart1;
    }
}