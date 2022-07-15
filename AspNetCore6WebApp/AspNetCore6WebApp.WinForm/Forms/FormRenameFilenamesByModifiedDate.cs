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
    public partial class FormRenameFilenamesByModifiedDate : Form
    {
        public string VersionString { get; set; } = string.Empty;
        public TT.Logging? Logger;

        private bool IsEnd = false;// Whether the process of renaming filenames is end. The default value is false;
        private readonly string StartText = "{0:";
        private readonly string EndText = "}";
        private string WorkDir = string.Empty;
        private string OutputFilenameTemplate = string.Empty;

        public FormRenameFilenamesByModifiedDate()
        {
            InitializeComponent();
        }

        private void LogToUi(string format, params object?[] args)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        int iMax = 100000;
                        TxtLog.AppendText((args == null || args.Length < 1) ? format : String.Format(format, args));
                        TxtLog.AppendText(Environment.NewLine);
                        if (TxtLog.TextLength > iMax) TxtLog.Text = TxtLog.Text[^iMax..];
                        TxtLog.SelectionStart = TxtLog.TextLength;
                        TxtLog.ScrollToCaret();
                    }
                    catch (Exception ex2)
                    {
                        try { Logger?.Error(ex2); }
                        catch (Exception ex3) { Console.WriteLine("[error] {0}.{1}. {2}", System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex3.Message); }
                    }
                }));
            }
            catch (Exception ex)
            {
                try { Logger?.Error(ex); }
                catch (Exception ex4) { Console.WriteLine("[error] {0}", ex4.ToString()); }
            }
        }

        private void LocalLogger(TT.Logging.LogLevel logLevel, string format, params object?[] args)
        {
            Logger?.Log(logLevel, format, args);
            LogToUi(format, args);
        }

        private void FormRenameFilenamesByModifiedDate_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VersionString)) LblVersion.Text = string.Empty;
            else
            {
                LblVersion.Text = VersionString;
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;
            }
        }

        private void BtnBrowseFolder_Click(object sender, EventArgs e)
        {
            using var oDialog = new FolderBrowserDialog() { ShowNewFolderButton = false };
            if (oDialog.ShowDialog() != DialogResult.OK) return;
            TxtTargetFolder.Text = oDialog.SelectedPath;
            LocalLogger(TT.Logging.LogLevel.DEBUG, "Selected Folder = {0}", TxtTargetFolder.Text);
        }

        private string CheckText(string text, string label)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Format("{0} cannot be EMPTY", label);
            int iStart = text.IndexOf(StartText);
            if (iStart < 0) return string.Format("{0} does NOT contain {1}", label, StartText);
            int iEnd = text.IndexOf(EndText);
            if (iEnd < 0) return string.Format("{0} does NOT contain {1}", label, EndText);
            if (iEnd < iStart) return string.Format("{0} contains wrong position of {1} and {2}", label, StartText, EndText);
            return string.Empty;
        }

        private void ButtonEnabled(Button button, bool enabled)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        button.Enabled = enabled;
                    }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex.ToString()); }
        }

        private void TextBoxReadOnly(TextBox textBox, bool readOnly)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try { textBox.ReadOnly = readOnly; }
                    catch (Exception ex2) { Logger?.Error(ex2); }
                }));
            }
            catch (Exception ex) { Logger?.Error(ex.ToString()); }
        }

        private void ControlsAvailable(bool isRunning)
        {
            ButtonEnabled(BtnRun, !isRunning);
            ButtonEnabled(BtnBrowseFolder, !isRunning);
            ButtonEnabled(BtnStop, isRunning);
            TextBoxReadOnly(TxtOutputFilenameTemplate, isRunning);
            TextBoxReadOnly(TxtTargetFolder, isRunning);
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            ControlsAvailable(true);
            try
            {
                string s = CheckText(TxtOutputFilenameTemplate.Text, LblOutputFilenameTemplate.Text);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    LocalLogger(TT.Logging.LogLevel.ERROR, s);
                    ControlsAvailable(false);
                    return;
                }
                OutputFilenameTemplate = TxtOutputFilenameTemplate.Text.Trim();

                WorkDir = TxtTargetFolder.Text.Trim();
                if (!System.IO.Directory.Exists(WorkDir))
                {
                    LocalLogger(TT.Logging.LogLevel.ERROR, "Target folder does not exist {0}", WorkDir);
                    ControlsAvailable(false);
                    return;
                }
                LocalLogger(TT.Logging.LogLevel.DEBUG, "Target folder = {0}", WorkDir);

                IsEnd = false;
                BWorkerRenameByModifiedDate.RunWorkerAsync();
            }
            catch (Exception ex) { LocalLogger(TT.Logging.LogLevel.ERROR, ex.ToString()); }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            LocalLogger(TT.Logging.LogLevel.DEBUG, "Stop to rename filenames by modified date");
            IsEnd = true;
        }

        private void RenameFilenamesByModifiedDate(string filepath)
        {
            if (!System.IO.File.Exists(filepath)) return;
            string fileExt = System.IO.Path.GetExtension(filepath);
            DateTime modifiedDate = System.IO.File.GetLastWriteTime(filepath);
            string outputFilepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filepath) ?? string.Empty, string.Format("{0}{1}", string.Format(OutputFilenameTemplate, modifiedDate), fileExt));
            LocalLogger(TT.Logging.LogLevel.DEBUG, "File = {0}; Date Time = {1:yyyy-MM-dd}; Output = {2}", filepath, modifiedDate, outputFilepath);
            System.IO.File.Move(filepath, outputFilepath);
        }

        private void BWorkerRenameByModifiedDate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LocalLogger(TT.Logging.LogLevel.DEBUG, "Start to rename filenames by modified date");
                string[] fileNameArray = System.IO.Directory.GetFiles(WorkDir);
                if (fileNameArray != null)
                {
                    int fileCount = fileNameArray.Length;
                    int i = 0;
                    while (IsEnd == false && i < fileCount)
                    {
                        RenameFilenamesByModifiedDate(fileNameArray[i]);
                        i++;
                    }
                    LocalLogger(TT.Logging.LogLevel.DEBUG, "Proceed {0} files", i);
                }
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                LocalLogger(TT.Logging.LogLevel.DEBUG, "End to rename filenames by modified date");
                ControlsAvailable(false);
            }
        }
    }
}
