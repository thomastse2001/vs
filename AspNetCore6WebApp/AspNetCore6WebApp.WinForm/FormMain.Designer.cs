namespace AspNetCore6WebApp.WinForm
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.LblVersion = new System.Windows.Forms.Label();
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.BWorkerLogin = new System.ComponentModel.BackgroundWorker();
            this.BtnHideMainDialog = new System.Windows.Forms.Button();
            this.ContextMenuStripHide = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.UnhideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotifyIconHide = new System.Windows.Forms.NotifyIcon(this.components);
            this.TabControlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.BtnIsVersionUpdated = new System.Windows.Forms.Button();
            this.BtnCreateExitFile = new System.Windows.Forms.Button();
            this.BtnRenameFilenamesByModifiedDate = new System.Windows.Forms.Button();
            this.BtnRenameFilenames = new System.Windows.Forms.Button();
            this.BtnTicTacToe = new System.Windows.Forms.Button();
            this.BtnPolygonShape2 = new System.Windows.Forms.Button();
            this.BtnTest1 = new System.Windows.Forms.Button();
            this.TxtFirstInput = new System.Windows.Forms.TextBox();
            this.BtnCryptoAES = new System.Windows.Forms.Button();
            this.TabPageTcpSocket = new System.Windows.Forms.TabPage();
            this.GroupBoxTcpClient = new System.Windows.Forms.GroupBox();
            this.BtnCloseAllTcpClients = new System.Windows.Forms.Button();
            this.BtnNewTcpClient = new System.Windows.Forms.Button();
            this.GroupBoxTcpServer = new System.Windows.Forms.GroupBox();
            this.BtnTcpServerSendText = new System.Windows.Forms.Button();
            this.TxtTcpServerInput = new System.Windows.Forms.TextBox();
            this.BtnTcpServerSendFile = new System.Windows.Forms.Button();
            this.ClbTcpServerClientList = new System.Windows.Forms.CheckedListBox();
            this.LblTcpServerClientList = new System.Windows.Forms.Label();
            this.BtnTcpServerDeselectAllClients = new System.Windows.Forms.Button();
            this.ChkTcpServerSelectAllClients = new System.Windows.Forms.CheckBox();
            this.LblTcpServerRestart = new System.Windows.Forms.Label();
            this.BtnTcpServerStopListening = new System.Windows.Forms.Button();
            this.BtnTcpServerStartListening = new System.Windows.Forms.Button();
            this.TabControlTcpServerParameters = new System.Windows.Forms.TabControl();
            this.TabPageTcpServerMainParameters = new System.Windows.Forms.TabPage();
            this.ChkTcpServerIsEncryptData = new System.Windows.Forms.CheckBox();
            this.NudTcpServerListeningPort = new System.Windows.Forms.NumericUpDown();
            this.LblTcpServerListeningPort = new System.Windows.Forms.Label();
            this.TabPageTcpServerOtherParameters = new System.Windows.Forms.TabPage();
            this.DgvTcpServerOtherParameters = new System.Windows.Forms.DataGridView();
            this.ColKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BWorkerTcpServerIncomingDataHandler = new System.ComponentModel.BackgroundWorker();
            this.BWorkerTcpServerUpdatingClientList = new System.ComponentModel.BackgroundWorker();
            this.BWorkerExitFile = new System.ComponentModel.BackgroundWorker();
            this.Dtp1 = new System.Windows.Forms.DateTimePicker();
            this.ContextMenuStripHide.SuspendLayout();
            this.TabControlMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.TabPageTcpSocket.SuspendLayout();
            this.GroupBoxTcpClient.SuspendLayout();
            this.GroupBoxTcpServer.SuspendLayout();
            this.TabControlTcpServerParameters.SuspendLayout();
            this.TabPageTcpServerMainParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudTcpServerListeningPort)).BeginInit();
            this.TabPageTcpServerOtherParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvTcpServerOtherParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(656, 9);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(116, 15);
            this.LblVersion.TabIndex = 0;
            this.LblVersion.Text = "Version: XX.XX.XX.XX";
            // 
            // TxtLog
            // 
            this.TxtLog.Location = new System.Drawing.Point(12, 28);
            this.TxtLog.Multiline = true;
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLog.Size = new System.Drawing.Size(761, 100);
            this.TxtLog.TabIndex = 1;
            // 
            // BWorkerLogin
            // 
            this.BWorkerLogin.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerLogin_DoWork);
            this.BWorkerLogin.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BWorkerLogin_RunWorkerCompleted);
            // 
            // BtnHideMainDialog
            // 
            this.BtnHideMainDialog.Location = new System.Drawing.Point(572, 61);
            this.BtnHideMainDialog.Name = "BtnHideMainDialog";
            this.BtnHideMainDialog.Size = new System.Drawing.Size(120, 23);
            this.BtnHideMainDialog.TabIndex = 2;
            this.BtnHideMainDialog.Text = "Hide Main Dialog";
            this.BtnHideMainDialog.UseVisualStyleBackColor = true;
            this.BtnHideMainDialog.Click += new System.EventHandler(this.BtnHideMainDialog_Click);
            // 
            // ContextMenuStripHide
            // 
            this.ContextMenuStripHide.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UnhideToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.ContextMenuStripHide.Name = "ContextMenuStripHide";
            this.ContextMenuStripHide.Size = new System.Drawing.Size(113, 48);
            // 
            // UnhideToolStripMenuItem
            // 
            this.UnhideToolStripMenuItem.Name = "UnhideToolStripMenuItem";
            this.UnhideToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.UnhideToolStripMenuItem.Text = "Unhide";
            this.UnhideToolStripMenuItem.Click += new System.EventHandler(this.UnhideToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // NotifyIconHide
            // 
            this.NotifyIconHide.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.NotifyIconHide.ContextMenuStrip = this.ContextMenuStripHide;
            this.NotifyIconHide.Text = "notifyIcon1";
            this.NotifyIconHide.Visible = true;
            // 
            // TabControlMain
            // 
            this.TabControlMain.Controls.Add(this.tabPage1);
            this.TabControlMain.Controls.Add(this.TabPageTcpSocket);
            this.TabControlMain.Location = new System.Drawing.Point(12, 134);
            this.TabControlMain.Name = "TabControlMain";
            this.TabControlMain.SelectedIndex = 0;
            this.TabControlMain.Size = new System.Drawing.Size(760, 300);
            this.TabControlMain.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Dtp1);
            this.tabPage1.Controls.Add(this.BtnIsVersionUpdated);
            this.tabPage1.Controls.Add(this.BtnCreateExitFile);
            this.tabPage1.Controls.Add(this.BtnRenameFilenamesByModifiedDate);
            this.tabPage1.Controls.Add(this.BtnRenameFilenames);
            this.tabPage1.Controls.Add(this.BtnTicTacToe);
            this.tabPage1.Controls.Add(this.BtnPolygonShape2);
            this.tabPage1.Controls.Add(this.BtnTest1);
            this.tabPage1.Controls.Add(this.TxtFirstInput);
            this.tabPage1.Controls.Add(this.BtnCryptoAES);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(752, 272);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // BtnIsVersionUpdated
            // 
            this.BtnIsVersionUpdated.Location = new System.Drawing.Point(113, 65);
            this.BtnIsVersionUpdated.Name = "BtnIsVersionUpdated";
            this.BtnIsVersionUpdated.Size = new System.Drawing.Size(120, 23);
            this.BtnIsVersionUpdated.TabIndex = 8;
            this.BtnIsVersionUpdated.Text = "Is Version Updated";
            this.BtnIsVersionUpdated.UseVisualStyleBackColor = true;
            this.BtnIsVersionUpdated.Click += new System.EventHandler(this.BtnIsVersionUpdated_Click);
            // 
            // BtnCreateExitFile
            // 
            this.BtnCreateExitFile.Location = new System.Drawing.Point(7, 65);
            this.BtnCreateExitFile.Name = "BtnCreateExitFile";
            this.BtnCreateExitFile.Size = new System.Drawing.Size(100, 23);
            this.BtnCreateExitFile.TabIndex = 7;
            this.BtnCreateExitFile.Text = "Create Exit File";
            this.BtnCreateExitFile.UseVisualStyleBackColor = true;
            this.BtnCreateExitFile.Click += new System.EventHandler(this.BtnCreateExitFile_Click);
            // 
            // BtnRenameFilenamesByModifiedDate
            // 
            this.BtnRenameFilenamesByModifiedDate.Location = new System.Drawing.Point(492, 35);
            this.BtnRenameFilenamesByModifiedDate.Name = "BtnRenameFilenamesByModifiedDate";
            this.BtnRenameFilenamesByModifiedDate.Size = new System.Drawing.Size(220, 23);
            this.BtnRenameFilenamesByModifiedDate.TabIndex = 6;
            this.BtnRenameFilenamesByModifiedDate.Text = "Rename Filenames by Modifed Date";
            this.BtnRenameFilenamesByModifiedDate.UseVisualStyleBackColor = true;
            this.BtnRenameFilenamesByModifiedDate.Click += new System.EventHandler(this.BtnRenameFilenamesByModifiedDate_Click);
            // 
            // BtnRenameFilenames
            // 
            this.BtnRenameFilenames.Location = new System.Drawing.Point(365, 36);
            this.BtnRenameFilenames.Name = "BtnRenameFilenames";
            this.BtnRenameFilenames.Size = new System.Drawing.Size(121, 23);
            this.BtnRenameFilenames.TabIndex = 5;
            this.BtnRenameFilenames.Text = "Rename Filenames";
            this.BtnRenameFilenames.UseVisualStyleBackColor = true;
            this.BtnRenameFilenames.Click += new System.EventHandler(this.BtnRenameFilenames_Click);
            // 
            // BtnTicTacToe
            // 
            this.BtnTicTacToe.Location = new System.Drawing.Point(284, 36);
            this.BtnTicTacToe.Name = "BtnTicTacToe";
            this.BtnTicTacToe.Size = new System.Drawing.Size(75, 23);
            this.BtnTicTacToe.TabIndex = 4;
            this.BtnTicTacToe.Text = "Tic Tac Toe";
            this.BtnTicTacToe.UseVisualStyleBackColor = true;
            this.BtnTicTacToe.Click += new System.EventHandler(this.BtnTicTacToe_Click);
            // 
            // BtnPolygonShape2
            // 
            this.BtnPolygonShape2.Location = new System.Drawing.Point(168, 36);
            this.BtnPolygonShape2.Name = "BtnPolygonShape2";
            this.BtnPolygonShape2.Size = new System.Drawing.Size(110, 23);
            this.BtnPolygonShape2.TabIndex = 3;
            this.BtnPolygonShape2.Text = "PolygonShape2";
            this.BtnPolygonShape2.UseVisualStyleBackColor = true;
            this.BtnPolygonShape2.Click += new System.EventHandler(this.BtnPolygonShape2_Click);
            // 
            // BtnTest1
            // 
            this.BtnTest1.Location = new System.Drawing.Point(87, 35);
            this.BtnTest1.Name = "BtnTest1";
            this.BtnTest1.Size = new System.Drawing.Size(75, 23);
            this.BtnTest1.TabIndex = 2;
            this.BtnTest1.Text = "Test1";
            this.BtnTest1.UseVisualStyleBackColor = true;
            this.BtnTest1.Click += new System.EventHandler(this.BtnTest1_Click);
            // 
            // TxtFirstInput
            // 
            this.TxtFirstInput.Location = new System.Drawing.Point(6, 6);
            this.TxtFirstInput.Name = "TxtFirstInput";
            this.TxtFirstInput.Size = new System.Drawing.Size(740, 23);
            this.TxtFirstInput.TabIndex = 0;
            this.TxtFirstInput.Text = "ABC 123 *$# def 456";
            // 
            // BtnCryptoAES
            // 
            this.BtnCryptoAES.Location = new System.Drawing.Point(6, 35);
            this.BtnCryptoAES.Name = "BtnCryptoAES";
            this.BtnCryptoAES.Size = new System.Drawing.Size(75, 23);
            this.BtnCryptoAES.TabIndex = 1;
            this.BtnCryptoAES.Text = "CryptoAES";
            this.BtnCryptoAES.UseVisualStyleBackColor = true;
            this.BtnCryptoAES.Click += new System.EventHandler(this.BtnCryptoAES_Click);
            // 
            // TabPageTcpSocket
            // 
            this.TabPageTcpSocket.Controls.Add(this.GroupBoxTcpClient);
            this.TabPageTcpSocket.Controls.Add(this.GroupBoxTcpServer);
            this.TabPageTcpSocket.Location = new System.Drawing.Point(4, 24);
            this.TabPageTcpSocket.Name = "TabPageTcpSocket";
            this.TabPageTcpSocket.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTcpSocket.Size = new System.Drawing.Size(752, 272);
            this.TabPageTcpSocket.TabIndex = 1;
            this.TabPageTcpSocket.Text = "TCP Socket";
            this.TabPageTcpSocket.UseVisualStyleBackColor = true;
            // 
            // GroupBoxTcpClient
            // 
            this.GroupBoxTcpClient.Controls.Add(this.BtnCloseAllTcpClients);
            this.GroupBoxTcpClient.Controls.Add(this.BtnNewTcpClient);
            this.GroupBoxTcpClient.Location = new System.Drawing.Point(572, 6);
            this.GroupBoxTcpClient.Name = "GroupBoxTcpClient";
            this.GroupBoxTcpClient.Size = new System.Drawing.Size(153, 179);
            this.GroupBoxTcpClient.TabIndex = 1;
            this.GroupBoxTcpClient.TabStop = false;
            this.GroupBoxTcpClient.Text = "TCP Client";
            // 
            // BtnCloseAllTcpClients
            // 
            this.BtnCloseAllTcpClients.Location = new System.Drawing.Point(6, 51);
            this.BtnCloseAllTcpClients.Name = "BtnCloseAllTcpClients";
            this.BtnCloseAllTcpClients.Size = new System.Drawing.Size(125, 23);
            this.BtnCloseAllTcpClients.TabIndex = 1;
            this.BtnCloseAllTcpClients.Text = "Close ALL TcpClients";
            this.BtnCloseAllTcpClients.UseVisualStyleBackColor = true;
            this.BtnCloseAllTcpClients.Click += new System.EventHandler(this.BtnCloseAllTcpClients_Click);
            // 
            // BtnNewTcpClient
            // 
            this.BtnNewTcpClient.Location = new System.Drawing.Point(6, 22);
            this.BtnNewTcpClient.Name = "BtnNewTcpClient";
            this.BtnNewTcpClient.Size = new System.Drawing.Size(98, 23);
            this.BtnNewTcpClient.TabIndex = 0;
            this.BtnNewTcpClient.Text = "New TcpClient";
            this.BtnNewTcpClient.UseVisualStyleBackColor = true;
            this.BtnNewTcpClient.Click += new System.EventHandler(this.BtnNewTcpClient_Click);
            // 
            // GroupBoxTcpServer
            // 
            this.GroupBoxTcpServer.Controls.Add(this.BtnTcpServerSendText);
            this.GroupBoxTcpServer.Controls.Add(this.TxtTcpServerInput);
            this.GroupBoxTcpServer.Controls.Add(this.BtnTcpServerSendFile);
            this.GroupBoxTcpServer.Controls.Add(this.ClbTcpServerClientList);
            this.GroupBoxTcpServer.Controls.Add(this.LblTcpServerClientList);
            this.GroupBoxTcpServer.Controls.Add(this.BtnTcpServerDeselectAllClients);
            this.GroupBoxTcpServer.Controls.Add(this.ChkTcpServerSelectAllClients);
            this.GroupBoxTcpServer.Controls.Add(this.LblTcpServerRestart);
            this.GroupBoxTcpServer.Controls.Add(this.BtnTcpServerStopListening);
            this.GroupBoxTcpServer.Controls.Add(this.BtnTcpServerStartListening);
            this.GroupBoxTcpServer.Controls.Add(this.TabControlTcpServerParameters);
            this.GroupBoxTcpServer.Location = new System.Drawing.Point(6, 6);
            this.GroupBoxTcpServer.Name = "GroupBoxTcpServer";
            this.GroupBoxTcpServer.Size = new System.Drawing.Size(560, 250);
            this.GroupBoxTcpServer.TabIndex = 0;
            this.GroupBoxTcpServer.TabStop = false;
            this.GroupBoxTcpServer.Text = "TCP Server";
            // 
            // BtnTcpServerSendText
            // 
            this.BtnTcpServerSendText.Location = new System.Drawing.Point(479, 207);
            this.BtnTcpServerSendText.Name = "BtnTcpServerSendText";
            this.BtnTcpServerSendText.Size = new System.Drawing.Size(75, 23);
            this.BtnTcpServerSendText.TabIndex = 10;
            this.BtnTcpServerSendText.Text = "Send Text";
            this.BtnTcpServerSendText.UseVisualStyleBackColor = true;
            this.BtnTcpServerSendText.Click += new System.EventHandler(this.BtnTcpServerSendText_Click);
            // 
            // TxtTcpServerInput
            // 
            this.TxtTcpServerInput.Location = new System.Drawing.Point(7, 208);
            this.TxtTcpServerInput.Name = "TxtTcpServerInput";
            this.TxtTcpServerInput.Size = new System.Drawing.Size(467, 23);
            this.TxtTcpServerInput.TabIndex = 9;
            this.TxtTcpServerInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtTcpServerInput_KeyUp);
            // 
            // BtnTcpServerSendFile
            // 
            this.BtnTcpServerSendFile.Location = new System.Drawing.Point(479, 174);
            this.BtnTcpServerSendFile.Name = "BtnTcpServerSendFile";
            this.BtnTcpServerSendFile.Size = new System.Drawing.Size(75, 23);
            this.BtnTcpServerSendFile.TabIndex = 8;
            this.BtnTcpServerSendFile.Text = "Send File";
            this.BtnTcpServerSendFile.UseVisualStyleBackColor = true;
            this.BtnTcpServerSendFile.Click += new System.EventHandler(this.BtnTcpServerSendFile_Click);
            // 
            // ClbTcpServerClientList
            // 
            this.ClbTcpServerClientList.FormattingEnabled = true;
            this.ClbTcpServerClientList.Location = new System.Drawing.Point(354, 91);
            this.ClbTcpServerClientList.Name = "ClbTcpServerClientList";
            this.ClbTcpServerClientList.Size = new System.Drawing.Size(200, 76);
            this.ClbTcpServerClientList.TabIndex = 7;
            // 
            // LblTcpServerClientList
            // 
            this.LblTcpServerClientList.AutoSize = true;
            this.LblTcpServerClientList.Location = new System.Drawing.Point(354, 73);
            this.LblTcpServerClientList.Name = "LblTcpServerClientList";
            this.LblTcpServerClientList.Size = new System.Drawing.Size(59, 15);
            this.LblTcpServerClientList.TabIndex = 6;
            this.LblTcpServerClientList.Text = "Client List";
            // 
            // BtnTcpServerDeselectAllClients
            // 
            this.BtnTcpServerDeselectAllClients.Location = new System.Drawing.Point(354, 47);
            this.BtnTcpServerDeselectAllClients.Name = "BtnTcpServerDeselectAllClients";
            this.BtnTcpServerDeselectAllClients.Size = new System.Drawing.Size(130, 23);
            this.BtnTcpServerDeselectAllClients.TabIndex = 5;
            this.BtnTcpServerDeselectAllClients.Text = "Deselect All Clients";
            this.BtnTcpServerDeselectAllClients.UseVisualStyleBackColor = true;
            this.BtnTcpServerDeselectAllClients.Click += new System.EventHandler(this.BtnTcpServerDeselectAllClients_Click);
            // 
            // ChkTcpServerSelectAllClients
            // 
            this.ChkTcpServerSelectAllClients.AutoSize = true;
            this.ChkTcpServerSelectAllClients.Checked = true;
            this.ChkTcpServerSelectAllClients.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkTcpServerSelectAllClients.Location = new System.Drawing.Point(354, 22);
            this.ChkTcpServerSelectAllClients.Name = "ChkTcpServerSelectAllClients";
            this.ChkTcpServerSelectAllClients.Size = new System.Drawing.Size(119, 19);
            this.ChkTcpServerSelectAllClients.TabIndex = 4;
            this.ChkTcpServerSelectAllClients.Text = "Select ALL Clients";
            this.ChkTcpServerSelectAllClients.UseVisualStyleBackColor = true;
            this.ChkTcpServerSelectAllClients.CheckedChanged += new System.EventHandler(this.ChkTcpServerSelectAllClients_CheckedChanged);
            // 
            // LblTcpServerRestart
            // 
            this.LblTcpServerRestart.AutoSize = true;
            this.LblTcpServerRestart.Location = new System.Drawing.Point(218, 178);
            this.LblTcpServerRestart.Name = "LblTcpServerRestart";
            this.LblTcpServerRestart.Size = new System.Drawing.Size(244, 15);
            this.LblTcpServerRestart.TabIndex = 3;
            this.LblTcpServerRestart.Text = "Please restart this app to start listening again.";
            this.LblTcpServerRestart.Visible = false;
            // 
            // BtnTcpServerStopListening
            // 
            this.BtnTcpServerStopListening.Location = new System.Drawing.Point(112, 178);
            this.BtnTcpServerStopListening.Name = "BtnTcpServerStopListening";
            this.BtnTcpServerStopListening.Size = new System.Drawing.Size(100, 23);
            this.BtnTcpServerStopListening.TabIndex = 2;
            this.BtnTcpServerStopListening.Text = "Stop Listening";
            this.BtnTcpServerStopListening.UseVisualStyleBackColor = true;
            this.BtnTcpServerStopListening.Click += new System.EventHandler(this.BtnTcpServerStopListening_Click);
            // 
            // BtnTcpServerStartListening
            // 
            this.BtnTcpServerStartListening.Location = new System.Drawing.Point(6, 178);
            this.BtnTcpServerStartListening.Name = "BtnTcpServerStartListening";
            this.BtnTcpServerStartListening.Size = new System.Drawing.Size(100, 23);
            this.BtnTcpServerStartListening.TabIndex = 1;
            this.BtnTcpServerStartListening.Text = "Start Listening";
            this.BtnTcpServerStartListening.UseVisualStyleBackColor = true;
            this.BtnTcpServerStartListening.Click += new System.EventHandler(this.BtnTcpServerStartListening_Click);
            // 
            // TabControlTcpServerParameters
            // 
            this.TabControlTcpServerParameters.Controls.Add(this.TabPageTcpServerMainParameters);
            this.TabControlTcpServerParameters.Controls.Add(this.TabPageTcpServerOtherParameters);
            this.TabControlTcpServerParameters.Location = new System.Drawing.Point(6, 22);
            this.TabControlTcpServerParameters.Name = "TabControlTcpServerParameters";
            this.TabControlTcpServerParameters.SelectedIndex = 0;
            this.TabControlTcpServerParameters.Size = new System.Drawing.Size(330, 150);
            this.TabControlTcpServerParameters.TabIndex = 0;
            // 
            // TabPageTcpServerMainParameters
            // 
            this.TabPageTcpServerMainParameters.Controls.Add(this.ChkTcpServerIsEncryptData);
            this.TabPageTcpServerMainParameters.Controls.Add(this.NudTcpServerListeningPort);
            this.TabPageTcpServerMainParameters.Controls.Add(this.LblTcpServerListeningPort);
            this.TabPageTcpServerMainParameters.Location = new System.Drawing.Point(4, 24);
            this.TabPageTcpServerMainParameters.Name = "TabPageTcpServerMainParameters";
            this.TabPageTcpServerMainParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTcpServerMainParameters.Size = new System.Drawing.Size(322, 122);
            this.TabPageTcpServerMainParameters.TabIndex = 0;
            this.TabPageTcpServerMainParameters.Text = "Main Parameters";
            this.TabPageTcpServerMainParameters.UseVisualStyleBackColor = true;
            // 
            // ChkTcpServerIsEncryptData
            // 
            this.ChkTcpServerIsEncryptData.AutoSize = true;
            this.ChkTcpServerIsEncryptData.Checked = true;
            this.ChkTcpServerIsEncryptData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkTcpServerIsEncryptData.Location = new System.Drawing.Point(6, 35);
            this.ChkTcpServerIsEncryptData.Name = "ChkTcpServerIsEncryptData";
            this.ChkTcpServerIsEncryptData.Size = new System.Drawing.Size(93, 19);
            this.ChkTcpServerIsEncryptData.TabIndex = 3;
            this.ChkTcpServerIsEncryptData.Text = "Encrypt Data";
            this.ChkTcpServerIsEncryptData.UseVisualStyleBackColor = true;
            // 
            // NudTcpServerListeningPort
            // 
            this.NudTcpServerListeningPort.Location = new System.Drawing.Point(92, 6);
            this.NudTcpServerListeningPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NudTcpServerListeningPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.NudTcpServerListeningPort.Name = "NudTcpServerListeningPort";
            this.NudTcpServerListeningPort.Size = new System.Drawing.Size(120, 23);
            this.NudTcpServerListeningPort.TabIndex = 1;
            this.NudTcpServerListeningPort.Value = new decimal(new int[] {
            8001,
            0,
            0,
            0});
            // 
            // LblTcpServerListeningPort
            // 
            this.LblTcpServerListeningPort.AutoSize = true;
            this.LblTcpServerListeningPort.Location = new System.Drawing.Point(6, 9);
            this.LblTcpServerListeningPort.Name = "LblTcpServerListeningPort";
            this.LblTcpServerListeningPort.Size = new System.Drawing.Size(80, 15);
            this.LblTcpServerListeningPort.TabIndex = 0;
            this.LblTcpServerListeningPort.Text = "Listening Port";
            // 
            // TabPageTcpServerOtherParameters
            // 
            this.TabPageTcpServerOtherParameters.Controls.Add(this.DgvTcpServerOtherParameters);
            this.TabPageTcpServerOtherParameters.Location = new System.Drawing.Point(4, 24);
            this.TabPageTcpServerOtherParameters.Name = "TabPageTcpServerOtherParameters";
            this.TabPageTcpServerOtherParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTcpServerOtherParameters.Size = new System.Drawing.Size(322, 122);
            this.TabPageTcpServerOtherParameters.TabIndex = 1;
            this.TabPageTcpServerOtherParameters.Text = "Other Parameters";
            this.TabPageTcpServerOtherParameters.UseVisualStyleBackColor = true;
            // 
            // DgvTcpServerOtherParameters
            // 
            this.DgvTcpServerOtherParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvTcpServerOtherParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColKey,
            this.ColValue});
            this.DgvTcpServerOtherParameters.Location = new System.Drawing.Point(6, 6);
            this.DgvTcpServerOtherParameters.Name = "DgvTcpServerOtherParameters";
            this.DgvTcpServerOtherParameters.RowTemplate.Height = 25;
            this.DgvTcpServerOtherParameters.Size = new System.Drawing.Size(310, 110);
            this.DgvTcpServerOtherParameters.TabIndex = 0;
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
            // BWorkerTcpServerIncomingDataHandler
            // 
            this.BWorkerTcpServerIncomingDataHandler.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerTcpServerIncomingDataHandler_DoWork);
            // 
            // BWorkerTcpServerUpdatingClientList
            // 
            this.BWorkerTcpServerUpdatingClientList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerTcpServerUpdatingClientList_DoWork);
            // 
            // BWorkerExitFile
            // 
            this.BWorkerExitFile.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorkerExitFile_DoWork);
            // 
            // Dtp1
            // 
            this.Dtp1.Location = new System.Drawing.Point(239, 65);
            this.Dtp1.Name = "Dtp1";
            this.Dtp1.Size = new System.Drawing.Size(200, 23);
            this.Dtp1.TabIndex = 9;
            this.Dtp1.Value = new System.DateTime(2022, 9, 1, 8, 45, 0, 0);
            this.Dtp1.ValueChanged += new System.EventHandler(this.Dtp1_ValueChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.TabControlMain);
            this.Controls.Add(this.BtnHideMainDialog);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ContextMenuStripHide.ResumeLayout(false);
            this.TabControlMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.TabPageTcpSocket.ResumeLayout(false);
            this.GroupBoxTcpClient.ResumeLayout(false);
            this.GroupBoxTcpServer.ResumeLayout(false);
            this.GroupBoxTcpServer.PerformLayout();
            this.TabControlTcpServerParameters.ResumeLayout(false);
            this.TabPageTcpServerMainParameters.ResumeLayout(false);
            this.TabPageTcpServerMainParameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudTcpServerListeningPort)).EndInit();
            this.TabPageTcpServerOtherParameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvTcpServerOtherParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LblVersion;
        private TextBox TxtLog;
        private System.ComponentModel.BackgroundWorker BWorkerLogin;
        private Button BtnHideMainDialog;
        private ContextMenuStrip ContextMenuStripHide;
        private ToolStripMenuItem UnhideToolStripMenuItem;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private NotifyIcon NotifyIconHide;
        private TabControl TabControlMain;
        private TabPage tabPage1;
        private TabPage TabPageTcpSocket;
        private GroupBox GroupBoxTcpServer;
        private GroupBox GroupBoxTcpClient;
        private Button BtnNewTcpClient;
        private Button BtnCloseAllTcpClients;
        private TabControl TabControlTcpServerParameters;
        private TabPage TabPageTcpServerMainParameters;
        private TabPage TabPageTcpServerOtherParameters;
        private NumericUpDown NudTcpServerListeningPort;
        private Label LblTcpServerListeningPort;
        private CheckBox ChkTcpServerIsEncryptData;
        private Label LblTcpServerRestart;
        private Button BtnTcpServerStopListening;
        private Button BtnTcpServerStartListening;
        private Button BtnTcpServerDeselectAllClients;
        private CheckBox ChkTcpServerSelectAllClients;
        private CheckedListBox ClbTcpServerClientList;
        private Label LblTcpServerClientList;
        private Button BtnTcpServerSendFile;
        private Button BtnTcpServerSendText;
        private TextBox TxtTcpServerInput;
        private System.ComponentModel.BackgroundWorker BWorkerTcpServerIncomingDataHandler;
        private System.ComponentModel.BackgroundWorker BWorkerTcpServerUpdatingClientList;
        private Button BtnCryptoAES;
        private TextBox TxtFirstInput;
        private Button BtnTest1;
        private DataGridView DgvTcpServerOtherParameters;
        private DataGridViewTextBoxColumn ColKey;
        private DataGridViewTextBoxColumn ColValue;
        private Button BtnPolygonShape2;
        private Button BtnTicTacToe;
        private Button BtnRenameFilenames;
        private Button BtnRenameFilenamesByModifiedDate;
        private System.ComponentModel.BackgroundWorker BWorkerExitFile;
        private Button BtnCreateExitFile;
        private Button BtnIsVersionUpdated;
        private DateTimePicker Dtp1;
    }
}