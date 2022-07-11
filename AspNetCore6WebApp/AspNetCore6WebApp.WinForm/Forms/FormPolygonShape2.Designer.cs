namespace AspNetCore6WebApp.WinForm.Forms
{
    partial class FormPolygonShape2
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
            this.SuspendLayout();
            // 
            // FormPolygonShape2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 790);
            this.Name = "FormPolygonShape2";
            this.Text = "FormPolygonShape2";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPolygonShape2_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormPolygonShape2_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormPolygonShape2_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}