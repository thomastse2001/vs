namespace VsCSharpWinForm_sample2.Views
{
    partial class FrmRenameFilenames
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
            this.LblInputFilenameTemplate = new System.Windows.Forms.Label();
            this.TxtInputFilenameTemplate = new System.Windows.Forms.TextBox();
            this.LblOutputFilenameTemplate = new System.Windows.Forms.Label();
            this.TxtOutputFilenameTemplate = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.LblNote = new System.Windows.Forms.Label();
            this.LblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblInputFilenameTemplate
            // 
            this.LblInputFilenameTemplate.AutoSize = true;
            this.LblInputFilenameTemplate.Location = new System.Drawing.Point(12, 103);
            this.LblInputFilenameTemplate.Name = "LblInputFilenameTemplate";
            this.LblInputFilenameTemplate.Size = new System.Drawing.Size(123, 13);
            this.LblInputFilenameTemplate.TabIndex = 0;
            this.LblInputFilenameTemplate.Text = "Input Filename Template";
            // 
            // TxtInputFilenameTemplate
            // 
            this.TxtInputFilenameTemplate.Location = new System.Drawing.Point(149, 100);
            this.TxtInputFilenameTemplate.Name = "TxtInputFilenameTemplate";
            this.TxtInputFilenameTemplate.Size = new System.Drawing.Size(220, 20);
            this.TxtInputFilenameTemplate.TabIndex = 1;
            this.TxtInputFilenameTemplate.Text = "invoice_AAA_{0:yyyy_MM_dd}_BBB.pdf";
            // 
            // LblOutputFilenameTemplate
            // 
            this.LblOutputFilenameTemplate.AutoSize = true;
            this.LblOutputFilenameTemplate.Location = new System.Drawing.Point(12, 129);
            this.LblOutputFilenameTemplate.Name = "LblOutputFilenameTemplate";
            this.LblOutputFilenameTemplate.Size = new System.Drawing.Size(131, 13);
            this.LblOutputFilenameTemplate.TabIndex = 2;
            this.LblOutputFilenameTemplate.Text = "Output Filename Template";
            // 
            // TxtOutputFilenameTemplate
            // 
            this.TxtOutputFilenameTemplate.Location = new System.Drawing.Point(149, 126);
            this.TxtOutputFilenameTemplate.Name = "TxtOutputFilenameTemplate";
            this.TxtOutputFilenameTemplate.Size = new System.Drawing.Size(220, 20);
            this.TxtOutputFilenameTemplate.TabIndex = 3;
            this.TxtOutputFilenameTemplate.Text = "AAAAA_{0:yyyy-MM-dd}_BB.pdf";
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(149, 176);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 4;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(297, 176);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 5;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // LblNote
            // 
            this.LblNote.AutoSize = true;
            this.LblNote.Location = new System.Drawing.Point(12, 70);
            this.LblNote.Name = "LblNote";
            this.LblNote.Size = new System.Drawing.Size(210, 13);
            this.LblNote.TabIndex = 6;
            this.LblNote.Text = "Enter the templates including file extension.";
            // 
            // LblMessage
            // 
            this.LblMessage.AutoSize = true;
            this.LblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblMessage.ForeColor = System.Drawing.Color.Red;
            this.LblMessage.Location = new System.Drawing.Point(12, 9);
            this.LblMessage.Name = "LblMessage";
            this.LblMessage.Size = new System.Drawing.Size(57, 13);
            this.LblMessage.TabIndex = 7;
            this.LblMessage.Text = "Message";
            // 
            // FrmRenameFilenames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.LblMessage);
            this.Controls.Add(this.LblNote);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.TxtOutputFilenameTemplate);
            this.Controls.Add(this.LblOutputFilenameTemplate);
            this.Controls.Add(this.TxtInputFilenameTemplate);
            this.Controls.Add(this.LblInputFilenameTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmRenameFilenames";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rename Filenames";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblInputFilenameTemplate;
        private System.Windows.Forms.Label LblOutputFilenameTemplate;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label LblNote;
        public System.Windows.Forms.TextBox TxtInputFilenameTemplate;
        public System.Windows.Forms.TextBox TxtOutputFilenameTemplate;
        public System.Windows.Forms.Label LblMessage;
    }
}