namespace VsCSharpWinForm_sample2.Views
{
    partial class FrmTicTacToe
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
            this.LblMode = new System.Windows.Forms.Label();
            this.TxtLog = new System.Windows.Forms.TextBox();
            this.CbxMode = new System.Windows.Forms.ComboBox();
            this.TlpGrid = new System.Windows.Forms.TableLayoutPanel();
            this.BtnRestartGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(499, 9);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(113, 13);
            this.LblVersion.TabIndex = 0;
            this.LblVersion.Text = "Version: XX.XX.XX.XX";
            // 
            // LblMode
            // 
            this.LblMode.AutoSize = true;
            this.LblMode.Location = new System.Drawing.Point(12, 94);
            this.LblMode.Name = "LblMode";
            this.LblMode.Size = new System.Drawing.Size(34, 13);
            this.LblMode.TabIndex = 3;
            this.LblMode.Text = "Mode";
            // 
            // TxtLog
            // 
            this.TxtLog.Location = new System.Drawing.Point(12, 25);
            this.TxtLog.Multiline = true;
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLog.Size = new System.Drawing.Size(600, 60);
            this.TxtLog.TabIndex = 2;
            this.TxtLog.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtLog_KeyUp);
            // 
            // CbxMode
            // 
            this.CbxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxMode.FormattingEnabled = true;
            this.CbxMode.Location = new System.Drawing.Point(52, 91);
            this.CbxMode.Name = "CbxMode";
            this.CbxMode.Size = new System.Drawing.Size(100, 21);
            this.CbxMode.TabIndex = 4;
            this.CbxMode.SelectedIndexChanged += new System.EventHandler(this.CbxMode_SelectedIndexChanged);
            this.CbxMode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbxMode_KeyUp);
            // 
            // TlpGrid
            // 
            this.TlpGrid.BackColor = System.Drawing.Color.Transparent;
            this.TlpGrid.ColumnCount = 3;
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.TlpGrid.Location = new System.Drawing.Point(12, 120);
            this.TlpGrid.Name = "TlpGrid";
            this.TlpGrid.RowCount = 3;
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpGrid.Size = new System.Drawing.Size(600, 311);
            this.TlpGrid.TabIndex = 6;
            this.TlpGrid.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.TlpGrid_CellPaint);
            this.TlpGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TlpGrid_MouseClick);
            // 
            // BtnRestartGame
            // 
            this.BtnRestartGame.Location = new System.Drawing.Point(187, 91);
            this.BtnRestartGame.Name = "BtnRestartGame";
            this.BtnRestartGame.Size = new System.Drawing.Size(90, 23);
            this.BtnRestartGame.TabIndex = 5;
            this.BtnRestartGame.Text = "Restart Game";
            this.BtnRestartGame.UseVisualStyleBackColor = true;
            this.BtnRestartGame.Click += new System.EventHandler(this.BtnRestartGame_Click);
            this.BtnRestartGame.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BtnRestartGame_KeyUp);
            // 
            // FrmTicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.BtnRestartGame);
            this.Controls.Add(this.TlpGrid);
            this.Controls.Add(this.CbxMode);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.LblMode);
            this.Controls.Add(this.LblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmTicTacToe";
            this.Text = "Tic Tac Toe";
            this.Load += new System.EventHandler(this.FrmTicTacToe_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.Label LblMode;
        private System.Windows.Forms.TextBox TxtLog;
        private System.Windows.Forms.ComboBox CbxMode;
        private System.Windows.Forms.TableLayoutPanel TlpGrid;
        private System.Windows.Forms.Button BtnRestartGame;
    }
}