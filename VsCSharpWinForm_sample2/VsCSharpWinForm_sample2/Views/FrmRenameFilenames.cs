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
    public partial class FrmRenameFilenames : Form
    {
        public string StartText = "{0:";
        public string EndText = "}";

        public FrmRenameFilenames()
        {
            InitializeComponent();
        }

        private string CheckText(string text, string label)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Format("{0} cannot be EMPTY.", label);
            int iStart = text.IndexOf(StartText);
            if (iStart < 0) return string.Format("{0} does NOT contain {1}", label, StartText);
            int iEnd = text.IndexOf(EndText);
            if (iEnd < 0) return string.Format("{0} does NOT contain {1}", label, EndText);
             if (iEnd < iStart) return string.Format("{0} contains wrong position of {1} and {2}", label, StartText, EndText);
            return null;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            LblMessage.Text = CheckText(TxtInputFilenameTemplate.Text, LblInputFilenameTemplate.Text) + Environment.NewLine;
            LblMessage.Text += CheckText(TxtOutputFilenameTemplate.Text, LblOutputFilenameTemplate.Text);
            if (!string.IsNullOrWhiteSpace(LblMessage.Text)) return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
