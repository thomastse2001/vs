namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormRenameFilenames
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
            this.LblNote = new System.Windows.Forms.Label();
            this.LblInputFilenameTemplate = new System.Windows.Forms.Label();
            this.TxtInputFilenameTemplate = new System.Windows.Forms.TextBox();
            this.LblOutputFilenameTemplate = new System.Windows.Forms.Label();
            this.TxtOutputFilenameTemplate = new System.Windows.Forms.TextBox();
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.BtnRun = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BWorkerRenameFilenames = new System.ComponentModel.BackgroundWorker();
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblTargetFolder = new System.Windows.Forms.Label();
            this.TxtTargetFolder = new System.Windows.Forms.TextBox();
            this.BtnBrowseFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblNote
            // 
            this.LblNote.AutoSize = true;
            this.LblNote.Location = new System.Drawing.Point(12, 130);
            this.LblNote.Name = "LblNote";
            this.LblNote.Size = new System.Drawing.Size(238, 15);
            this.LblNote.TabIndex = 2;
            this.LblNote.Text = "Enter the templates including file extension.";
            // 
            // LblInputFilenameTemplate
            // 
            this.LblInputFilenameTemplate.AutoSize = true;
            this.LblInputFilenameTemplate.Location = new System.Drawing.Point(12, 151);
            this.LblInputFilenameTemplate.Name = "LblInputFilenameTemplate";
            this.LblInputFilenameTemplate.Size = new System.Drawing.Size(137, 15);
            this.LblInputFilenameTemplate.TabIndex = 3;
            this.LblInputFilenameTemplate.Text = "Input Filename Template";
            // 
            // TxtInputFilenameTemplate
            // 
            this.TxtInputFilenameTemplate.Location = new System.Drawing.Point(170, 148);
            this.TxtInputFilenameTemplate.Name = "TxtInputFilenameTemplate";
            this.TxtInputFilenameTemplate.Size = new System.Drawing.Size(302, 23);
            this.TxtInputFilenameTemplate.TabIndex = 4;
            this.TxtInputFilenameTemplate.Text = "invoice_AAA_{0:yyyy-MM-dd}_BBB.pdf";
            // 
            // LblOutputFilenameTemplate
            // 
            this.LblOutputFilenameTemplate.AutoSize = true;
            this.LblOutputFilenameTemplate.Location = new System.Drawing.Point(12, 180);
            this.LblOutputFilenameTemplate.Name = "LblOutputFilenameTemplate";
            this.LblOutputFilenameTemplate.Size = new System.Drawing.Size(147, 15);
            this.LblOutputFilenameTemplate.TabIndex = 5;
            this.LblOutputFilenameTemplate.Text = "Output Filename Template";
            // 
            // TxtOutputFilenameTemplate
            // 
            this.TxtOutputFilenameTemplate.Location = new System.Drawing.Point(170, 177);
            this.TxtOutputFilenameTemplate.Name = "TxtOutputFilenameTemplate";
            this.TxtOutputFilenameTemplate.Size = new System.Drawing.Size(302, 23);
            this.TxtOutputFilenameTemplate.TabIndex = 6;
            this.TxtOutputFilenameTemplate.Text = "AAAAA_{0:yyyy-MM-dd}_BB.pdf";
            // 
            // TxtLog
            // 
            this.TxtLog.Location = new System.Drawing.Point(12, 27);
            this.TxtLog.Multiline = true;
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLog.Size = new System.Drawing.Size(460, 100);
            this.TxtLog.TabIndex = 1;
            // 
            // BtnRun
            // 
            this.BtnRun.Location = new System.Drawing.Point(235, 235);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(75, 23);
            this.BtnRun.TabIndex = 10;
            this.BtnRun.Text = "Run";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Enabled = false;
            this.BtnStop.Location = new System.Drawing.Point(397, 235);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(75, 23);
            this.BtnStop.TabIndex = 11;
            this.BtnStop.Text = "Stop";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BWorkerRenameFilenames
            // 
            this.BWorkerRenameFilenames.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerRenameFilenames_DoWork);
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(356, 9);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(116, 15);
            this.LblVersion.TabIndex = 0;
            this.LblVersion.Text = "Version: XX.XX.XX.XX";
            // 
            // LblTargetFolder
            // 
            this.LblTargetFolder.AutoSize = true;
            this.LblTargetFolder.Location = new System.Drawing.Point(12, 209);
            this.LblTargetFolder.Name = "LblTargetFolder";
            this.LblTargetFolder.Size = new System.Drawing.Size(75, 15);
            this.LblTargetFolder.TabIndex = 7;
            this.LblTargetFolder.Text = "Target Folder";
            // 
            // TxtTargetFolder
            // 
            this.TxtTargetFolder.Location = new System.Drawing.Point(107, 206);
            this.TxtTargetFolder.Name = "TxtTargetFolder";
            this.TxtTargetFolder.Size = new System.Drawing.Size(329, 23);
            this.TxtTargetFolder.TabIndex = 8;
            // 
            // BtnBrowseFolder
            // 
            this.BtnBrowseFolder.Location = new System.Drawing.Point(442, 206);
            this.BtnBrowseFolder.Name = "BtnBrowseFolder";
            this.BtnBrowseFolder.Size = new System.Drawing.Size(30, 23);
            this.BtnBrowseFolder.TabIndex = 9;
            this.BtnBrowseFolder.Text = "..";
            this.BtnBrowseFolder.UseVisualStyleBackColor = true;
            this.BtnBrowseFolder.Click += new System.EventHandler(this.BtnBrowseFolder_Click);
            // 
            // FormRenameFilenames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 281);
            this.Controls.Add(this.BtnBrowseFolder);
            this.Controls.Add(this.TxtTargetFolder);
            this.Controls.Add(this.LblTargetFolder);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnRun);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.TxtOutputFilenameTemplate);
            this.Controls.Add(this.LblOutputFilenameTemplate);
            this.Controls.Add(this.TxtInputFilenameTemplate);
            this.Controls.Add(this.LblInputFilenameTemplate);
            this.Controls.Add(this.LblNote);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRenameFilenames";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename Filenames";
            this.Load += new System.EventHandler(this.FormRenameFilenames_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label LblNote;
        private Label LblInputFilenameTemplate;
        private TextBox TxtInputFilenameTemplate;
        private Label LblOutputFilenameTemplate;
        private TextBox TxtOutputFilenameTemplate;
        private TextBox TxtLog;
        private Button BtnRun;
        private Button BtnStop;
        private System.ComponentModel.BackgroundWorker BWorkerRenameFilenames;
        private Label LblVersion;
        private Label LblTargetFolder;
        private TextBox TxtTargetFolder;
        private Button BtnBrowseFolder;
    }
}