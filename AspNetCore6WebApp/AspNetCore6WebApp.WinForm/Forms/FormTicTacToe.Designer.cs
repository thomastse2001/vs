namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormTicTacToe
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
            this.LblMode = new System.Windows.Forms.Label();
            this.CbxMode = new System.Windows.Forms.ComboBox();
            this.BtnRestartGame = new System.Windows.Forms.Button();
            this.TlpGrid = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(496, 9);
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
            this.TxtLog.Size = new System.Drawing.Size(600, 60);
            this.TxtLog.TabIndex = 1;
            this.TxtLog.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtLog_KeyUp);
            // 
            // LblMode
            // 
            this.LblMode.AutoSize = true;
            this.LblMode.Location = new System.Drawing.Point(12, 98);
            this.LblMode.Name = "LblMode";
            this.LblMode.Size = new System.Drawing.Size(38, 15);
            this.LblMode.TabIndex = 2;
            this.LblMode.Text = "Mode";
            // 
            // CbxMode
            // 
            this.CbxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxMode.FormattingEnabled = true;
            this.CbxMode.Location = new System.Drawing.Point(56, 94);
            this.CbxMode.Name = "CbxMode";
            this.CbxMode.Size = new System.Drawing.Size(100, 23);
            this.CbxMode.TabIndex = 3;
            this.CbxMode.SelectedIndexChanged += new System.EventHandler(this.CbxMode_SelectedIndexChanged);
            this.CbxMode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbxMode_KeyUp);
            // 
            // BtnRestartGame
            // 
            this.BtnRestartGame.Location = new System.Drawing.Point(200, 94);
            this.BtnRestartGame.Name = "BtnRestartGame";
            this.BtnRestartGame.Size = new System.Drawing.Size(90, 23);
            this.BtnRestartGame.TabIndex = 4;
            this.BtnRestartGame.Text = "Restart Game";
            this.BtnRestartGame.UseVisualStyleBackColor = true;
            this.BtnRestartGame.Click += new System.EventHandler(this.BtnRestartGame_Click);
            this.BtnRestartGame.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BtnRestartGame_KeyUp);
            // 
            // TlpGrid
            // 
            this.TlpGrid.ColumnCount = 3;
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.TlpGrid.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.TlpGrid.Location = new System.Drawing.Point(12, 123);
            this.TlpGrid.Name = "TlpGrid";
            this.TlpGrid.RowCount = 3;
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.Size = new System.Drawing.Size(600, 306);
            this.TlpGrid.TabIndex = 5;
            this.TlpGrid.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.TlpGrid_CellPaint);
            this.TlpGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TlpGrid_MouseClick);
            // 
            // FormTicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.TlpGrid);
            this.Controls.Add(this.BtnRestartGame);
            this.Controls.Add(this.CbxMode);
            this.Controls.Add(this.LblMode);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormTicTacToe";
            this.Text = "Tic Tac Toe";
            this.Load += new System.EventHandler(this.FormTicTacToe_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label LblVersion;
        private TextBox TxtLog;
        private Label LblMode;
        private ComboBox CbxMode;
        private Button BtnRestartGame;
        private TableLayoutPanel TlpGrid;
    }
}