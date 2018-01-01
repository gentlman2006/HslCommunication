namespace TestTool.TestForm
{
    partial class FormLantern
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
            this.userLantern3 = new HslCommunication.Controls.UserLantern();
            this.userLantern2 = new HslCommunication.Controls.UserLantern();
            this.userLantern1 = new HslCommunication.Controls.UserLantern();
            this.SuspendLayout();
            // 
            // userLantern3
            // 
            this.userLantern3.BackColor = System.Drawing.Color.Transparent;
            this.userLantern3.LanternBackground = System.Drawing.Color.Tomato;
            this.userLantern3.Location = new System.Drawing.Point(214, 171);
            this.userLantern3.Name = "userLantern3";
            this.userLantern3.Size = new System.Drawing.Size(130, 139);
            this.userLantern3.TabIndex = 2;
            // 
            // userLantern2
            // 
            this.userLantern2.BackColor = System.Drawing.Color.Transparent;
            this.userLantern2.Location = new System.Drawing.Point(258, 52);
            this.userLantern2.Name = "userLantern2";
            this.userLantern2.Size = new System.Drawing.Size(86, 86);
            this.userLantern2.TabIndex = 1;
            // 
            // userLantern1
            // 
            this.userLantern1.BackColor = System.Drawing.Color.Transparent;
            this.userLantern1.Location = new System.Drawing.Point(30, 26);
            this.userLantern1.Name = "userLantern1";
            this.userLantern1.Size = new System.Drawing.Size(187, 181);
            this.userLantern1.TabIndex = 0;
            // 
            // FormLantern
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 400);
            this.Controls.Add(this.userLantern3);
            this.Controls.Add(this.userLantern2);
            this.Controls.Add(this.userLantern1);
            this.Name = "FormLantern";
            this.Text = "FormLantern";
            this.ResumeLayout(false);

        }

        #endregion

        private HslCommunication.Controls.UserLantern userLantern1;
        private HslCommunication.Controls.UserLantern userLantern2;
        private HslCommunication.Controls.UserLantern userLantern3;
    }
}