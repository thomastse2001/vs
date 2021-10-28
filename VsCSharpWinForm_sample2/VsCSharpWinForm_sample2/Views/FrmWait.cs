using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsCSharpWinForm_sample2.Views
{
    public partial class FrmWait : Form
    {
        //public static Helpers.TLog Logger { get; set; }
        public bool EndWaiting = false;/// Flag to indicate whether end waiting.
        public bool ExitNow { get; private set; } = false;/// Flag to indicate whether exit the application. Read only for public.
        public FrmWait()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit this application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                ExitNow = true;
                TimerEndWaiting.Enabled = false;
                this.Close();
            }
        }

        private void TimerEndWaiting_Tick(object sender, EventArgs e)
        {
            if (EndWaiting)
            {
                TimerEndWaiting.Enabled = false;
                this.Close();
            }
        }

        private void FrmWait_Load(object sender, EventArgs e)
        {
            TimerEndWaiting.Start();
        }
    }
}
