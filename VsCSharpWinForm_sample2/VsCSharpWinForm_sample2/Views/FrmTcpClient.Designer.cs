namespace VsCSharpWinForm_sample2.Views
{
    partial class FrmTcpClient
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
            this.LblServerHost = new System.Windows.Forms.Label();
            this.TxtServerHost = new System.Windows.Forms.TextBox();
            this.LblServerPort = new System.Windows.Forms.Label();
            this.NudServerPort = new System.Windows.Forms.NumericUpDown();
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.BtnDisconnect = new System.Windows.Forms.Button();
            this.TxtInput = new System.Windows.Forms.TextBox();
            this.BtnSendText = new System.Windows.Forms.Button();
            this.ChkContainLengthAsHeader = new System.Windows.Forms.CheckBox();
            this.ChkEncryptData = new System.Windows.Forms.CheckBox();
            this.BtnSendFile = new System.Windows.Forms.Button();
            this.ChkHeartbeatInterval = new System.Windows.Forms.CheckBox();
            this.NudHeartbeatInterval = new System.Windows.Forms.NumericUpDown();
            this.LblReceiveDataInterval = new System.Windows.Forms.Label();
            this.NudReceiveDataInterval = new System.Windows.Forms.NumericUpDown();
            this.NudMaxDataSize = new System.Windows.Forms.NumericUpDown();
            this.LblMaxDataSize = new System.Windows.Forms.Label();
            this.NudMaxIdleDuration = new System.Windows.Forms.NumericUpDown();
            this.ChkMaxIdleDuration = new System.Windows.Forms.CheckBox();
            this.NudMaxConnectionDuration = new System.Windows.Forms.NumericUpDown();
            this.ChkMaxConnectionDuration = new System.Windows.Forms.CheckBox();
            this.TControlParameters = new System.Windows.Forms.TabControl();
            this.TPageMainParameters = new System.Windows.Forms.TabPage();
            this.TPageOtherParameters = new System.Windows.Forms.TabPage();
            this.NudSleepingInterval = new System.Windows.Forms.NumericUpDown();
            this.ChkSleepingInterval = new System.Windows.Forms.CheckBox();
            this.NudReceiveTotalBufferSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.BWorkerCheckConnected = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudHeartbeatInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudReceiveDataInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxDataSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxIdleDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxConnectionDuration)).BeginInit();
            this.TControlParameters.SuspendLayout();
            this.TPageMainParameters.SuspendLayout();
            this.TPageOtherParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudSleepingInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudReceiveTotalBufferSize)).BeginInit();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(309, 9);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(113, 13);
            this.LblVersion.TabIndex = 0;
            this.LblVersion.Text = "Version: XX.XX.XX.XX";
            // 
            // LblServerHost
            // 
            this.LblServerHost.AutoSize = true;
            this.LblServerHost.Location = new System.Drawing.Point(6, 11);
            this.LblServerHost.Name = "LblServerHost";
            this.LblServerHost.Size = new System.Drawing.Size(63, 13);
            this.LblServerHost.TabIndex = 0;
            this.LblServerHost.Text = "Server Host";
            // 
            // TxtServerHost
            // 
            this.TxtServerHost.Location = new System.Drawing.Point(140, 7);
            this.TxtServerHost.Name = "TxtServerHost";
            this.TxtServerHost.Size = new System.Drawing.Size(150, 20);
            this.TxtServerHost.TabIndex = 1;
            // 
            // LblServerPort
            // 
            this.LblServerPort.AutoSize = true;
            this.LblServerPort.Location = new System.Drawing.Point(6, 40);
            this.LblServerPort.Name = "LblServerPort";
            this.LblServerPort.Size = new System.Drawing.Size(60, 13);
            this.LblServerPort.TabIndex = 2;
            this.LblServerPort.Text = "Server Port";
            // 
            // NudServerPort
            // 
            this.NudServerPort.Location = new System.Drawing.Point(140, 37);
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
            this.NudServerPort.Size = new System.Drawing.Size(150, 20);
            this.NudServerPort.TabIndex = 3;
            this.NudServerPort.Value = new decimal(new int[] {
            8888,
            0,
            0,
            0});
            // 
            // TxtLog
            // 
            this.TxtLog.Location = new System.Drawing.Point(12, 26);
            this.TxtLog.Multiline = true;
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLog.Size = new System.Drawing.Size(410, 65);
            this.TxtLog.TabIndex = 1;
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(347, 98);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(75, 23);
            this.BtnConnect.TabIndex = 3;
            this.BtnConnect.Text = "Connect";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // BtnDisconnect
            // 
            this.BtnDisconnect.Location = new System.Drawing.Point(347, 127);
            this.BtnDisconnect.Name = "BtnDisconnect";
            this.BtnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.BtnDisconnect.TabIndex = 4;
            this.BtnDisconnect.Text = "Disconnect";
            this.BtnDisconnect.UseVisualStyleBackColor = true;
            this.BtnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // TxtInput
            // 
            this.TxtInput.Location = new System.Drawing.Point(12, 246);
            this.TxtInput.Name = "TxtInput";
            this.TxtInput.Size = new System.Drawing.Size(320, 20);
            this.TxtInput.TabIndex = 6;
            this.TxtInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtInput_KeyUp);
            // 
            // BtnSendText
            // 
            this.BtnSendText.Location = new System.Drawing.Point(347, 247);
            this.BtnSendText.Name = "BtnSendText";
            this.BtnSendText.Size = new System.Drawing.Size(75, 23);
            this.BtnSendText.TabIndex = 7;
            this.BtnSendText.Text = "Send Text";
            this.BtnSendText.UseVisualStyleBackColor = true;
            this.BtnSendText.Click += new System.EventHandler(this.BtnSendText_Click);
            // 
            // ChkContainLengthAsHeader
            // 
            this.ChkContainLengthAsHeader.AutoSize = true;
            this.ChkContainLengthAsHeader.Location = new System.Drawing.Point(6, 67);
            this.ChkContainLengthAsHeader.Name = "ChkContainLengthAsHeader";
            this.ChkContainLengthAsHeader.Size = new System.Drawing.Size(151, 17);
            this.ChkContainLengthAsHeader.TabIndex = 4;
            this.ChkContainLengthAsHeader.Text = "Contain Length As Header";
            this.ChkContainLengthAsHeader.UseVisualStyleBackColor = true;
            // 
            // ChkEncryptData
            // 
            this.ChkEncryptData.AutoSize = true;
            this.ChkEncryptData.Location = new System.Drawing.Point(174, 67);
            this.ChkEncryptData.Name = "ChkEncryptData";
            this.ChkEncryptData.Size = new System.Drawing.Size(86, 17);
            this.ChkEncryptData.TabIndex = 5;
            this.ChkEncryptData.Text = "Encrypt data";
            this.ChkEncryptData.UseVisualStyleBackColor = true;
            // 
            // BtnSendFile
            // 
            this.BtnSendFile.Location = new System.Drawing.Point(347, 216);
            this.BtnSendFile.Name = "BtnSendFile";
            this.BtnSendFile.Size = new System.Drawing.Size(75, 25);
            this.BtnSendFile.TabIndex = 5;
            this.BtnSendFile.Text = "Send File";
            this.BtnSendFile.UseVisualStyleBackColor = true;
            this.BtnSendFile.Click += new System.EventHandler(this.BtnSendFile_Click);
            // 
            // ChkHeartbeatInterval
            // 
            this.ChkHeartbeatInterval.AutoSize = true;
            this.ChkHeartbeatInterval.Location = new System.Drawing.Point(6, 8);
            this.ChkHeartbeatInterval.Name = "ChkHeartbeatInterval";
            this.ChkHeartbeatInterval.Size = new System.Drawing.Size(125, 17);
            this.ChkHeartbeatInterval.TabIndex = 10;
            this.ChkHeartbeatInterval.Text = "Heartbeat Interval (s)";
            this.ChkHeartbeatInterval.UseVisualStyleBackColor = true;
            this.ChkHeartbeatInterval.CheckedChanged += new System.EventHandler(this.ChkHeartbeatInterval_CheckedChanged);
            // 
            // NudHeartbeatInterval
            // 
            this.NudHeartbeatInterval.Location = new System.Drawing.Point(208, 7);
            this.NudHeartbeatInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudHeartbeatInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.NudHeartbeatInterval.Name = "NudHeartbeatInterval";
            this.NudHeartbeatInterval.Size = new System.Drawing.Size(50, 20);
            this.NudHeartbeatInterval.TabIndex = 11;
            this.NudHeartbeatInterval.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // LblReceiveDataInterval
            // 
            this.LblReceiveDataInterval.AutoSize = true;
            this.LblReceiveDataInterval.Location = new System.Drawing.Point(6, 130);
            this.LblReceiveDataInterval.Name = "LblReceiveDataInterval";
            this.LblReceiveDataInterval.Size = new System.Drawing.Size(125, 13);
            this.LblReceiveDataInterval.TabIndex = 18;
            this.LblReceiveDataInterval.Text = "Receive Data Interval (s)";
            // 
            // NudReceiveDataInterval
            // 
            this.NudReceiveDataInterval.Location = new System.Drawing.Point(208, 128);
            this.NudReceiveDataInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudReceiveDataInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.NudReceiveDataInterval.Name = "NudReceiveDataInterval";
            this.NudReceiveDataInterval.Size = new System.Drawing.Size(50, 20);
            this.NudReceiveDataInterval.TabIndex = 19;
            // 
            // NudMaxDataSize
            // 
            this.NudMaxDataSize.Location = new System.Drawing.Point(168, 98);
            this.NudMaxDataSize.Maximum = new decimal(new int[] {
            104857600,
            0,
            0,
            0});
            this.NudMaxDataSize.Name = "NudMaxDataSize";
            this.NudMaxDataSize.Size = new System.Drawing.Size(90, 20);
            this.NudMaxDataSize.TabIndex = 17;
            this.NudMaxDataSize.Value = new decimal(new int[] {
            104857600,
            0,
            0,
            0});
            // 
            // LblMaxDataSize
            // 
            this.LblMaxDataSize.AutoSize = true;
            this.LblMaxDataSize.Location = new System.Drawing.Point(6, 100);
            this.LblMaxDataSize.Name = "LblMaxDataSize";
            this.LblMaxDataSize.Size = new System.Drawing.Size(110, 13);
            this.LblMaxDataSize.TabIndex = 16;
            this.LblMaxDataSize.Text = "Max Data Size (bytes)";
            // 
            // NudMaxIdleDuration
            // 
            this.NudMaxIdleDuration.Location = new System.Drawing.Point(208, 67);
            this.NudMaxIdleDuration.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudMaxIdleDuration.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.NudMaxIdleDuration.Name = "NudMaxIdleDuration";
            this.NudMaxIdleDuration.Size = new System.Drawing.Size(50, 20);
            this.NudMaxIdleDuration.TabIndex = 15;
            this.NudMaxIdleDuration.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // ChkMaxIdleDuration
            // 
            this.ChkMaxIdleDuration.AutoSize = true;
            this.ChkMaxIdleDuration.Location = new System.Drawing.Point(6, 68);
            this.ChkMaxIdleDuration.Name = "ChkMaxIdleDuration";
            this.ChkMaxIdleDuration.Size = new System.Drawing.Size(123, 17);
            this.ChkMaxIdleDuration.TabIndex = 14;
            this.ChkMaxIdleDuration.Text = "Max Idle Duration (s)";
            this.ChkMaxIdleDuration.UseVisualStyleBackColor = true;
            this.ChkMaxIdleDuration.CheckedChanged += new System.EventHandler(this.ChkMaxIdleDuration_CheckedChanged);
            // 
            // NudMaxConnectionDuration
            // 
            this.NudMaxConnectionDuration.Location = new System.Drawing.Point(208, 37);
            this.NudMaxConnectionDuration.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudMaxConnectionDuration.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.NudMaxConnectionDuration.Name = "NudMaxConnectionDuration";
            this.NudMaxConnectionDuration.Size = new System.Drawing.Size(50, 20);
            this.NudMaxConnectionDuration.TabIndex = 13;
            this.NudMaxConnectionDuration.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // ChkMaxConnectionDuration
            // 
            this.ChkMaxConnectionDuration.AutoSize = true;
            this.ChkMaxConnectionDuration.Location = new System.Drawing.Point(6, 38);
            this.ChkMaxConnectionDuration.Name = "ChkMaxConnectionDuration";
            this.ChkMaxConnectionDuration.Size = new System.Drawing.Size(160, 17);
            this.ChkMaxConnectionDuration.TabIndex = 12;
            this.ChkMaxConnectionDuration.Text = "Max Connection Duration (s)";
            this.ChkMaxConnectionDuration.UseVisualStyleBackColor = true;
            this.ChkMaxConnectionDuration.CheckedChanged += new System.EventHandler(this.ChkMaxConnectionDuration_CheckedChanged);
            // 
            // TControlParameters
            // 
            this.TControlParameters.Controls.Add(this.TPageMainParameters);
            this.TControlParameters.Controls.Add(this.TPageOtherParameters);
            this.TControlParameters.Location = new System.Drawing.Point(12, 98);
            this.TControlParameters.Name = "TControlParameters";
            this.TControlParameters.SelectedIndex = 0;
            this.TControlParameters.Size = new System.Drawing.Size(320, 141);
            this.TControlParameters.TabIndex = 2;
            // 
            // TPageMainParameters
            // 
            this.TPageMainParameters.AutoScroll = true;
            this.TPageMainParameters.Controls.Add(this.LblServerHost);
            this.TPageMainParameters.Controls.Add(this.TxtServerHost);
            this.TPageMainParameters.Controls.Add(this.LblServerPort);
            this.TPageMainParameters.Controls.Add(this.NudServerPort);
            this.TPageMainParameters.Controls.Add(this.ChkContainLengthAsHeader);
            this.TPageMainParameters.Controls.Add(this.ChkEncryptData);
            this.TPageMainParameters.Location = new System.Drawing.Point(4, 22);
            this.TPageMainParameters.Name = "TPageMainParameters";
            this.TPageMainParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TPageMainParameters.Size = new System.Drawing.Size(312, 115);
            this.TPageMainParameters.TabIndex = 0;
            this.TPageMainParameters.Text = "Main Parameters";
            this.TPageMainParameters.UseVisualStyleBackColor = true;
            // 
            // TPageOtherParameters
            // 
            this.TPageOtherParameters.AutoScroll = true;
            this.TPageOtherParameters.Controls.Add(this.NudSleepingInterval);
            this.TPageOtherParameters.Controls.Add(this.ChkSleepingInterval);
            this.TPageOtherParameters.Controls.Add(this.NudReceiveTotalBufferSize);
            this.TPageOtherParameters.Controls.Add(this.label1);
            this.TPageOtherParameters.Controls.Add(this.NudReceiveDataInterval);
            this.TPageOtherParameters.Controls.Add(this.LblReceiveDataInterval);
            this.TPageOtherParameters.Controls.Add(this.ChkMaxConnectionDuration);
            this.TPageOtherParameters.Controls.Add(this.NudMaxConnectionDuration);
            this.TPageOtherParameters.Controls.Add(this.NudMaxDataSize);
            this.TPageOtherParameters.Controls.Add(this.ChkMaxIdleDuration);
            this.TPageOtherParameters.Controls.Add(this.ChkHeartbeatInterval);
            this.TPageOtherParameters.Controls.Add(this.LblMaxDataSize);
            this.TPageOtherParameters.Controls.Add(this.NudHeartbeatInterval);
            this.TPageOtherParameters.Controls.Add(this.NudMaxIdleDuration);
            this.TPageOtherParameters.Location = new System.Drawing.Point(4, 22);
            this.TPageOtherParameters.Name = "TPageOtherParameters";
            this.TPageOtherParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TPageOtherParameters.Size = new System.Drawing.Size(312, 115);
            this.TPageOtherParameters.TabIndex = 1;
            this.TPageOtherParameters.Text = "Other Parameters";
            this.TPageOtherParameters.UseVisualStyleBackColor = true;
            // 
            // NudSleepingInterval
            // 
            this.NudSleepingInterval.Location = new System.Drawing.Point(208, 189);
            this.NudSleepingInterval.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSleepingInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.NudSleepingInterval.Name = "NudSleepingInterval";
            this.NudSleepingInterval.Size = new System.Drawing.Size(50, 20);
            this.NudSleepingInterval.TabIndex = 23;
            this.NudSleepingInterval.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // ChkSleepingInterval
            // 
            this.ChkSleepingInterval.AutoSize = true;
            this.ChkSleepingInterval.Checked = true;
            this.ChkSleepingInterval.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkSleepingInterval.Location = new System.Drawing.Point(6, 190);
            this.ChkSleepingInterval.Name = "ChkSleepingInterval";
            this.ChkSleepingInterval.Size = new System.Drawing.Size(127, 17);
            this.ChkSleepingInterval.TabIndex = 22;
            this.ChkSleepingInterval.Text = "Sleeping Interval (ms)";
            this.ChkSleepingInterval.UseVisualStyleBackColor = true;
            this.ChkSleepingInterval.CheckedChanged += new System.EventHandler(this.ChkSleepingInterval_CheckedChanged);
            // 
            // NudReceiveTotalBufferSize
            // 
            this.NudReceiveTotalBufferSize.Location = new System.Drawing.Point(188, 158);
            this.NudReceiveTotalBufferSize.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.NudReceiveTotalBufferSize.Name = "NudReceiveTotalBufferSize";
            this.NudReceiveTotalBufferSize.Size = new System.Drawing.Size(70, 20);
            this.NudReceiveTotalBufferSize.TabIndex = 21;
            this.NudReceiveTotalBufferSize.Value = new decimal(new int[] {
            10485760,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Receive Total Buffer Size (bytes)";
            // 
            // BWorkerCheckConnected
            // 
            this.BWorkerCheckConnected.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerCheckConnected_DoWork);
            // 
            // FrmTcpClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 283);
            this.Controls.Add(this.TControlParameters);
            this.Controls.Add(this.BtnSendFile);
            this.Controls.Add(this.BtnSendText);
            this.Controls.Add(this.TxtInput);
            this.Controls.Add(this.BtnDisconnect);
            this.Controls.Add(this.BtnConnect);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmTcpClient";
            this.Text = "FrmTcpClient";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTcpClient_FormClosed);
            this.Load += new System.EventHandler(this.FrmTcpClient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NudServerPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudHeartbeatInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudReceiveDataInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxDataSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxIdleDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMaxConnectionDuration)).EndInit();
            this.TControlParameters.ResumeLayout(false);
            this.TPageMainParameters.ResumeLayout(false);
            this.TPageMainParameters.PerformLayout();
            this.TPageOtherParameters.ResumeLayout(false);
            this.TPageOtherParameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudSleepingInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudReceiveTotalBufferSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.Label LblServerHost;
        private System.Windows.Forms.Label LblServerPort;
        private System.Windows.Forms.TextBox TxtLog;
        public System.Windows.Forms.NumericUpDown NudServerPort;
        public System.Windows.Forms.TextBox TxtServerHost;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.Button BtnDisconnect;
        private System.Windows.Forms.TextBox TxtInput;
        private System.Windows.Forms.Button BtnSendText;
        public System.Windows.Forms.CheckBox ChkContainLengthAsHeader;
        public System.Windows.Forms.CheckBox ChkEncryptData;
        private System.Windows.Forms.Button BtnSendFile;
        private System.Windows.Forms.CheckBox ChkHeartbeatInterval;
        private System.Windows.Forms.NumericUpDown NudHeartbeatInterval;
        private System.Windows.Forms.NumericUpDown NudMaxConnectionDuration;
        private System.Windows.Forms.CheckBox ChkMaxConnectionDuration;
        private System.Windows.Forms.CheckBox ChkMaxIdleDuration;
        private System.Windows.Forms.NumericUpDown NudMaxIdleDuration;
        private System.Windows.Forms.Label LblMaxDataSize;
        private System.Windows.Forms.NumericUpDown NudMaxDataSize;
        private System.Windows.Forms.NumericUpDown NudReceiveDataInterval;
        private System.Windows.Forms.Label LblReceiveDataInterval;
        private System.Windows.Forms.TabControl TControlParameters;
        private System.Windows.Forms.TabPage TPageMainParameters;
        private System.Windows.Forms.TabPage TPageOtherParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NudReceiveTotalBufferSize;
        private System.Windows.Forms.NumericUpDown NudSleepingInterval;
        private System.Windows.Forms.CheckBox ChkSleepingInterval;
        private System.ComponentModel.BackgroundWorker BWorkerCheckConnected;
    }
}