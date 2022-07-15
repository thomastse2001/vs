namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormRenameFilenamesByModifiedDate
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
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.LblOutputFilenameTemplate = new System.Windows.Forms.Label();
            this.TxtOutputFilenameTemplate = new System.Windows.Forms.TextBox();
            this.LblTargetFolder = new System.Windows.Forms.Label();
            this.TxtTargetFolder = new System.Windows.Forms.TextBox();
            this.BtnBrowseFolder = new System.Windows.Forms.Button();
            this.BtnRun = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BWorkerRenameByModifiedDate = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
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
            // LblOutputFilenameTemplate
            // 
            this.LblOutputFilenameTemplate.AutoSize = true;
            this.LblOutputFilenameTemplate.Location = new System.Drawing.Point(12, 136);
            this.LblOutputFilenameTemplate.Name = "LblOutputFilenameTemplate";
            this.LblOutputFilenameTemplate.Size = new System.Drawing.Size(221, 15);
            this.LblOutputFilenameTemplate.TabIndex = 2;
            this.LblOutputFilenameTemplate.Text = "Output Filename Template (without Ext.)";
            // 
            // TxtOutputFilenameTemplate
            // 
            this.TxtOutputFilenameTemplate.Location = new System.Drawing.Point(252, 133);
            this.TxtOutputFilenameTemplate.Name = "TxtOutputFilenameTemplate";
            this.TxtOutputFilenameTemplate.Size = new System.Drawing.Size(220, 23);
            this.TxtOutputFilenameTemplate.TabIndex = 3;
            this.TxtOutputFilenameTemplate.Text = "AAA_{0:yyyyMMdd_HHmmss}_BBB";
            // 
            // LblTargetFolder
            // 
            this.LblTargetFolder.AutoSize = true;
            this.LblTargetFolder.Location = new System.Drawing.Point(12, 166);
            this.LblTargetFolder.Name = "LblTargetFolder";
            this.LblTargetFolder.Size = new System.Drawing.Size(75, 15);
            this.LblTargetFolder.TabIndex = 4;
            this.LblTargetFolder.Text = "Target Folder";
            // 
            // TxtTargetFolder
            // 
            this.TxtTargetFolder.Location = new System.Drawing.Point(117, 162);
            this.TxtTargetFolder.Name = "TxtTargetFolder";
            this.TxtTargetFolder.Size = new System.Drawing.Size(319, 23);
            this.TxtTargetFolder.TabIndex = 5;
            // 
            // BtnBrowseFolder
            // 
            this.BtnBrowseFolder.Location = new System.Drawing.Point(442, 162);
            this.BtnBrowseFolder.Name = "BtnBrowseFolder";
            this.BtnBrowseFolder.Size = new System.Drawing.Size(30, 23);
            this.BtnBrowseFolder.TabIndex = 6;
            this.BtnBrowseFolder.Text = "..";
            this.BtnBrowseFolder.UseVisualStyleBackColor = true;
            this.BtnBrowseFolder.Click += new System.EventHandler(this.BtnBrowseFolder_Click);
            // 
            // BtnRun
            // 
            this.BtnRun.Location = new System.Drawing.Point(252, 191);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(75, 23);
            this.BtnRun.TabIndex = 7;
            this.BtnRun.Text = "Run";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Enabled = false;
            this.BtnStop.Location = new System.Drawing.Point(397, 191);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(75, 23);
            this.BtnStop.TabIndex = 8;
            this.BtnStop.Text = "Stop";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BWorkerRenameByModifiedDate
            // 
            this.BWorkerRenameByModifiedDate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerRenameByModifiedDate_DoWork);
            // 
            // FormRenameFilenamesByModifiedDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnRun);
            this.Controls.Add(this.BtnBrowseFolder);
            this.Controls.Add(this.TxtTargetFolder);
            this.Controls.Add(this.LblTargetFolder);
            this.Controls.Add(this.TxtOutputFilenameTemplate);
            this.Controls.Add(this.LblOutputFilenameTemplate);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRenameFilenamesByModifiedDate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormRenameFilenamesByModifiedDate";
            this.Load += new System.EventHandler(this.FormRenameFilenamesByModifiedDate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LblVersion;
        private TextBox TxtLog;
        private Label LblOutputFilenameTemplate;
        private TextBox TxtOutputFilenameTemplate;
        private Label LblTargetFolder;
        private TextBox TxtTargetFolder;
        private Button BtnBrowseFolder;
        private Button BtnRun;
        private Button BtnStop;
        private System.ComponentModel.BackgroundWorker BWorkerRenameByModifiedDate;
    }
}