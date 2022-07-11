using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AspNetCore6WebApp.WinForm.Forms
{
    public partial class FormLogin : Form
    {
        public string VersionString { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Alert { get; set; } = string.Empty;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VersionString)) LblVersion.Text = string.Empty;
            else
            {
                LblVersion.Text = VersionString;
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;
            }
            if (!string.IsNullOrWhiteSpace(Username)) TxtUsername.Text = Username;
            if (!string.IsNullOrWhiteSpace(Password)) TxtPassword.Text = Password;
            LblAlert.Text = string.IsNullOrWhiteSpace(Alert) ? string.Empty : Alert;

            TxtUsername.Select();
            TxtUsername.SelectionStart = 0;
            TxtUsername.SelectionLength = TxtUsername.TextLength;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Username = string.IsNullOrWhiteSpace(TxtUsername.Text) ? string.Empty : TxtUsername.Text.Trim();
            Password = string.IsNullOrWhiteSpace(TxtPassword.Text) ? string.Empty : TxtPassword.Text.Trim();
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TxtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) TxtPassword.Focus();
        }

        private void TxtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter) BtnOK_Click(null, null);
            if (e.KeyCode == Keys.Enter) BtnOK.PerformClick();
        }
    }
}
