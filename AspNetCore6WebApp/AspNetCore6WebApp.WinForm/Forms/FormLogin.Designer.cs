namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormLogin
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
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblAlert = new System.Windows.Forms.Label();
            this.LblUsername = new System.Windows.Forms.Label();
            this.TxtUsername = new System.Windows.Forms.TextBox();
            this.LblPassword = new System.Windows.Forms.Label();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.LblHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(256, 9);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(116, 15);
            this.LblVersion.TabIndex = 0;
            this.LblVersion.Text = "Version: XX.XX.XX.XX";
            // 
            // LblAlert
            // 
            this.LblAlert.AutoSize = true;
            this.LblAlert.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LblAlert.ForeColor = System.Drawing.Color.Red;
            this.LblAlert.Location = new System.Drawing.Point(30, 35);
            this.LblAlert.Name = "LblAlert";
            this.LblAlert.Size = new System.Drawing.Size(38, 17);
            this.LblAlert.TabIndex = 1;
            this.LblAlert.Text = "Alert";
            // 
            // LblUsername
            // 
            this.LblUsername.AutoSize = true;
            this.LblUsername.Location = new System.Drawing.Point(12, 83);
            this.LblUsername.Name = "LblUsername";
            this.LblUsername.Size = new System.Drawing.Size(60, 15);
            this.LblUsername.TabIndex = 2;
            this.LblUsername.Text = "Username";
            // 
            // TxtUsername
            // 
            this.TxtUsername.Location = new System.Drawing.Point(100, 80);
            this.TxtUsername.Name = "TxtUsername";
            this.TxtUsername.Size = new System.Drawing.Size(272, 23);
            this.TxtUsername.TabIndex = 3;
            this.TxtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtUsername_KeyUp);
            // 
            // LblPassword
            // 
            this.LblPassword.AutoSize = true;
            this.LblPassword.Location = new System.Drawing.Point(12, 112);
            this.LblPassword.Name = "LblPassword";
            this.LblPassword.Size = new System.Drawing.Size(57, 15);
            this.LblPassword.TabIndex = 4;
            this.LblPassword.Text = "Password";
            // 
            // TxtPassword
            // 
            this.TxtPassword.Location = new System.Drawing.Point(100, 109);
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.PasswordChar = '*';
            this.TxtPassword.Size = new System.Drawing.Size(272, 23);
            this.TxtPassword.TabIndex = 5;
            this.TxtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtPassword_KeyUp);
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(154, 176);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 6;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(297, 176);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 7;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // LblHint
            // 
            this.LblHint.AutoSize = true;
            this.LblHint.Location = new System.Drawing.Point(12, 169);
            this.LblHint.Name = "LblHint";
            this.LblHint.Size = new System.Drawing.Size(72, 30);
            this.LblHint.TabIndex = 8;
            this.LblHint.Text = "Username: a\r\nPassword: a";
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.LblHint);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.LblPassword);
            this.Controls.Add(this.TxtUsername);
            this.Controls.Add(this.LblUsername);
            this.Controls.Add(this.LblAlert);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LblVersion;
        private Label LblAlert;
        private Label LblUsername;
        private TextBox TxtUsername;
        private Label LblPassword;
        private TextBox TxtPassword;
        private Button BtnOK;
        private Button BtnCancel;
        private Label LblHint;
    }
}