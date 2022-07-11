namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormTcpClient
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
            this.TabControlParameter = new System.Windows.Forms.TabControl();
            this.TabPageMainParameters = new System.Windows.Forms.TabPage();
            this.ChkIsEncryptData = new System.Windows.Forms.CheckBox();
            this.NudServerPort = new System.Windows.Forms.NumericUpDown();
            this.LblServerPort = new System.Windows.Forms.Label();
            this.TxtServerHost = new System.Windows.Forms.TextBox();
            this.LblServerHost = new System.Windows.Forms.Label();
            this.TabPageOtherParameters = new System.Windows.Forms.TabPage();
            this.DgvOtherParameters = new System.Windows.Forms.DataGridView();
            this.ColKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.BtnDisconnect = new System.Windows.Forms.Button();
            this.BtnSendFile = new System.Windows.Forms.Button();
            this.TxtInput = new System.Windows.Forms.TextBox();
            this.BtnSendText = new System.Windows.Forms.Button();
            this.BWorkerCheckConnected = new System.ComponentModel.BackgroundWorker();
            this.TabControlParameter.SuspendLayout();
            this.TabPageMainParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).BeginInit();
            this.TabPageOtherParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvOtherParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(306, 9);
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
            this.TxtLog.Size = new System.Drawing.Size(410, 90);
            this.TxtLog.TabIndex = 1;
            // 
            // TabControlParameter
            // 
            this.TabControlParameter.Controls.Add(this.TabPageMainParameters);
            this.TabControlParameter.Controls.Add(this.TabPageOtherParameters);
            this.TabControlParameter.Location = new System.Drawing.Point(12, 123);
            this.TabControlParameter.Name = "TabControlParameter";
            this.TabControlParameter.SelectedIndex = 0;
            this.TabControlParameter.Size = new System.Drawing.Size(329, 143);
            this.TabControlParameter.TabIndex = 2;
            // 
            // TabPageMainParameters
            // 
            this.TabPageMainParameters.Controls.Add(this.ChkIsEncryptData);
            this.TabPageMainParameters.Controls.Add(this.NudServerPort);
            this.TabPageMainParameters.Controls.Add(this.LblServerPort);
            this.TabPageMainParameters.Controls.Add(this.TxtServerHost);
            this.TabPageMainParameters.Controls.Add(this.LblServerHost);
            this.TabPageMainParameters.Location = new System.Drawing.Point(4, 24);
            this.TabPageMainParameters.Name = "TabPageMainParameters";
            this.TabPageMainParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageMainParameters.Size = new System.Drawing.Size(321, 115);
            this.TabPageMainParameters.TabIndex = 0;
            this.TabPageMainParameters.Text = "Main Parameters";
            this.TabPageMainParameters.UseVisualStyleBackColor = true;
            // 
            // ChkIsEncryptData
            // 
            this.ChkIsEncryptData.AutoSize = true;
            this.ChkIsEncryptData.Location = new System.Drawing.Point(6, 64);
            this.ChkIsEncryptData.Name = "ChkIsEncryptData";
            this.ChkIsEncryptData.Size = new System.Drawing.Size(93, 19);
            this.ChkIsEncryptData.TabIndex = 5;
            this.ChkIsEncryptData.Text = "Encrypt Data";
            this.ChkIsEncryptData.UseVisualStyleBackColor = true;
            // 
            // NudServerPort
            // 
            this.NudServerPort.Location = new System.Drawing.Point(79, 35);
            this.NudServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NudServerPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.NudServerPort.Name = "NudServerPort";
            this.NudServerPort.Size = new System.Drawing.Size(236, 23);
            this.NudServerPort.TabIndex = 3;
            this.NudServerPort.Value = new decimal(new int[] {
            8888,
            0,
            0,
            0});
            // 
            // LblServerPort
            // 
            this.LblServerPort.AutoSize = true;
            this.LblServerPort.Location = new System.Drawing.Point(6, 37);
            this.LblServerPort.Name = "LblServerPort";
            this.LblServerPort.Size = new System.Drawing.Size(64, 15);
            this.LblServerPort.TabIndex = 2;
            this.LblServerPort.Text = "Server Port";
            // 
            // TxtServerHost
            // 
            this.TxtServerHost.Location = new System.Drawing.Point(79, 6);
            this.TxtServerHost.Name = "TxtServerHost";
            this.TxtServerHost.Size = new System.Drawing.Size(236, 23);
            this.TxtServerHost.TabIndex = 1;
            // 
            // LblServerHost
            // 
            this.LblServerHost.AutoSize = true;
            this.LblServerHost.Location = new System.Drawing.Point(6, 9);
            this.LblServerHost.Name = "LblServerHost";
            this.LblServerHost.Size = new System.Drawing.Size(67, 15);
            this.LblServerHost.TabIndex = 0;
            this.LblServerHost.Text = "Server Host";
            // 
            // TabPageOtherParameters
            // 
            this.TabPageOtherParameters.Controls.Add(this.DgvOtherParameters);
            this.TabPageOtherParameters.Location = new System.Drawing.Point(4, 24);
            this.TabPageOtherParameters.Name = "TabPageOtherParameters";
            this.TabPageOtherParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageOtherParameters.Size = new System.Drawing.Size(321, 115);
            this.TabPageOtherParameters.TabIndex = 1;
            this.TabPageOtherParameters.Text = "Other Parameters";
            this.TabPageOtherParameters.UseVisualStyleBackColor = true;
            // 
            // DgvOtherParameters
            // 
            this.DgvOtherParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvOtherParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColKey,
            this.ColValue});
            this.DgvOtherParameters.Location = new System.Drawing.Point(6, 6);
            this.DgvOtherParameters.Name = "DgvOtherParameters";
            this.DgvOtherParameters.RowTemplate.Height = 25;
            this.DgvOtherParameters.Size = new System.Drawing.Size(309, 103);
            this.DgvOtherParameters.TabIndex = 0;
            // 
            // ColKey
            // 
            this.ColKey.HeaderText = "Key";
            this.ColKey.Name = "ColKey";
            this.ColKey.Width = 150;
            // 
            // ColValue
            // 
            this.ColValue.HeaderText = "Value";
            this.ColValue.Name = "ColValue";
            this.ColValue.Width = 80;
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(347, 123);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(75, 23);
            this.BtnConnect.TabIndex = 3;
            this.BtnConnect.Text = "Connect";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // BtnDisconnect
            // 
            this.BtnDisconnect.Location = new System.Drawing.Point(347, 153);
            this.BtnDisconnect.Name = "BtnDisconnect";
            this.BtnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.BtnDisconnect.TabIndex = 4;
            this.BtnDisconnect.Text = "Disconnect";
            this.BtnDisconnect.UseVisualStyleBackColor = true;
            this.BtnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // BtnSendFile
            // 
            this.BtnSendFile.Location = new System.Drawing.Point(347, 243);
            this.BtnSendFile.Name = "BtnSendFile";
            this.BtnSendFile.Size = new System.Drawing.Size(75, 23);
            this.BtnSendFile.TabIndex = 5;
            this.BtnSendFile.Text = "Send File";
            this.BtnSendFile.UseVisualStyleBackColor = true;
            this.BtnSendFile.Click += new System.EventHandler(this.BtnSendFile_Click);
            // 
            // TxtInput
            // 
            this.TxtInput.Location = new System.Drawing.Point(12, 272);
            this.TxtInput.Name = "TxtInput";
            this.TxtInput.Size = new System.Drawing.Size(320, 23);
            this.TxtInput.TabIndex = 6;
            this.TxtInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtInput_KeyUp);
            // 
            // BtnSendText
            // 
            this.BtnSendText.Location = new System.Drawing.Point(347, 272);
            this.BtnSendText.Name = "BtnSendText";
            this.BtnSendText.Size = new System.Drawing.Size(75, 23);
            this.BtnSendText.TabIndex = 7;
            this.BtnSendText.Text = "Send Text";
            this.BtnSendText.UseVisualStyleBackColor = true;
            this.BtnSendText.Click += new System.EventHandler(this.BtnSendText_Click);
            // 
            // BWorkerCheckConnected
            // 
            this.BWorkerCheckConnected.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerCheckConnected_DoWork);
            // 
            // FormTcpClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 311);
            this.Controls.Add(this.BtnSendText);
            this.Controls.Add(this.TxtInput);
            this.Controls.Add(this.BtnSendFile);
            this.Controls.Add(this.BtnDisconnect);
            this.Controls.Add(this.BtnConnect);
            this.Controls.Add(this.TabControlParameter);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormTcpClient";
            this.Text = "FormTcpClient";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTcpClient_FormClosed);
            this.Load += new System.EventHandler(this.FormTcpClient_Load);
            this.TabControlParameter.ResumeLayout(false);
            this.TabPageMainParameters.ResumeLayout(false);
            this.TabPageMainParameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).EndInit();
            this.TabPageOtherParameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvOtherParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LblVersion;
        private TextBox TxtLog;
        private TabControl TabControlParameter;
        private TabPage TabPageMainParameters;
        private TabPage TabPageOtherParameters;
        private Button BtnConnect;
        private Button BtnDisconnect;
        private Label LblServerPort;
        private TextBox TxtServerHost;
        private Label LblServerHost;
        private NumericUpDown NudServerPort;
        private CheckBox ChkIsEncryptData;
        private Button BtnSendFile;
        private TextBox TxtInput;
        private Button BtnSendText;
        private System.ComponentModel.BackgroundWorker BWorkerCheckConnected;
        private DataGridView DgvOtherParameters;
        private DataGridViewTextBoxColumn ColKey;
        private DataGridViewTextBoxColumn ColValue;
    }
}