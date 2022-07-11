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
    public partial class FormWait : Form
    {
        public bool IsEndWaiting { get; set; } = false;// Flag to indicate whether end waiting.
        public bool IsExit { get; private set; } = false;// Flag to indicate whether exit the application. Read only for public.

        public FormWait()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit this application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                IsExit = true;
                this.Close();
            }
        }

        private void TimerEndWaiting_Tick(object sender, EventArgs e)
        {
            if (IsEndWaiting)
            {
                TimerEndWaiting.Enabled = false;
                this.Close();
            }
        }

        private void FormWait_Load(object sender, EventArgs e)
        {
            TimerEndWaiting.Start();
        }
    }
}
