using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    //public interface ILogging
    //{
    //    //void Log(LogLevel logLevel, string format, params object?[] args);
    //}

    public class Logging
    {
        /// Write log to a text file in a specific format with a specific file name and path.
        /// Updated date: 2021-03-31
        /// Usage.
        /// // 1. Create an instance for it.
        /// Logging Logger = new();
        /// // Better to set it to static.
        /// static Logging Logger = new();
        /// // 2. Set the parameters for it if necessary.
        /// Logger.FilePathFormat = @"log\{0:yyyy-MM-dd}.log";// {0} = Date time.
        /// Logger.ContentFormat = "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}";// {0} = Date time, {1} = Log level, {2} = Log message.
        /// ...
        /// // 3. Start process if operation mode is not 0.
        /// // Logger.StartProcess();
        /// // 4. Log a text.
        /// Logger.Info("ABC DEF");
        /// // 5. Before the program is closed, shutdown and dispose the logger.
        /// // Logger.ShutdownAndDispose();
        /// Logger = null;

        /// https://logging.apache.org/log4j/2.x/manual/customloglevels.html
        /// https://github.com/NLog/NLog/wiki/Tutorial
        public enum LogLevel : int
        {
            OFF = 0,
            FATAL = 100,
            ERROR = 200,
            WARN = 300,
            INFO = 400,
            DEBUG = 500,
            TRACE = 600,
            ALL = int.MaxValue
        };

        /// Variables.
        //private static readonly string ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name;
        private readonly object WriterLocker = new();
        private readonly string DefaultFolder;
        public string FilePathFormat { get; set; } = @"log\{0:yyyy-MM-dd}.log";/// {0} = Date time.
        public string ContentFormat { get; set; } = "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}";/// {0} = Date time, {1} = Log level, {2} = Log message.
        public LogLevel MinLogLevel { get; set; } = LogLevel.ALL;

        public Logging()
        {
            DefaultFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
        }

        ///// Get the absolte path if the input path is a relative path.
        ///// Return value = output.
        ///// path = input path
        //private string GetAbsolutePathIfRelative(string path)
        //{
        //    return System.IO.Path.IsPathRooted(path) ? path : System.IO.Path.Combine(DefaultFolder, path);
        //}

        /// Verify if the folder exists or not. If the folder does not exist, create it.
        /// Return Value = true if the folder exists. False if the folder does not exist and it fails to create it.
        /// folder = folder path
        private static bool FolderExistsOrCreateIt(string folder)
        {
            //folder = folder.TrimEnd((char)9, ' ', System.IO.Path.DirectorySeparatorChar).Trim();
            if (string.IsNullOrWhiteSpace(folder)) return false;
            if (System.IO.Directory.Exists(folder)) return true;
            System.IO.Directory.CreateDirectory(folder);
            return System.IO.Directory.Exists(folder);
        }

        /// Write line to a file.
        /// Return value = true if sccess. False otherwise.
        /// filepath = file path
        /// format = content format to be written to a file
        /// args = arguments of the content format
        /// bAppend = false if overwrite the file. True if append to the file if the file already exists.
        /// https://support.microsoft.com/en-us/help/816149/how-to-read-from-and-write-to-a-text-file-by-using-visual-c
        private static void WriteLineToFile(string filepath, string format, params object?[] args)
        {
            using System.IO.StreamWriter sw = new(filepath, true);/// true for append.
            if (args == null || args.Length < 1) sw.WriteLine(format);
            else sw.WriteLine(format, args);
            sw.Flush();
        }

        /// https://support.microsoft.com/en-us/help/816149/how-to-read-from-and-write-to-a-text-file-by-using-visual-c
        private static bool TryToWriteLineToFile(string filepath, string format, params object?[] args)
        {
            /// To save time, try to write log first. If it fails with DirectoryNotFoundException, then create the directory for the log file.
            /// It prevents to check whether the folder exists or not every time.
            try
            {
                WriteLineToFile(filepath, format, args);
                return true;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                try
                {
                    if (!FolderExistsOrCreateIt(System.IO.Path.GetDirectoryName(filepath) ?? string.Empty)) return false;
                    WriteLineToFile(filepath, format, args);
                    return true;
                }
                catch { return false; }
            }
            catch { return false; }
        }

        /// Write log to a file in a folder.
        private void WriteLogInFolder(string filepath, string format, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(filepath)) return;
            if (!System.IO.Path.IsPathRooted(filepath)) filepath = System.IO.Path.Combine(DefaultFolder, filepath);
            lock (WriterLocker)
            {
                if (!TryToWriteLineToFile(filepath, format, args))
                {
                    int i = 1;
                    int iMax = 7;
                    bool bLoop = true;
                    while (bLoop && i <= iMax)
                    {
                        if (TryToWriteLineToFile(filepath, string.Format("Re-Write log at {0} as the file is being used. {1}", i, format), args)) bLoop = false;
                        else i += 1;
                    }
                }
            }
        }

        /// https://github.com/NLog/NLog/wiki/Tutorial
        public void Log(LogLevel logLevel, string format, params object?[] args)
        {
            /// OFF = 0.
            if (logLevel == 0) return;
            /// Other levels.
            if (logLevel <= MinLogLevel)
            {
                DateTime tRef = DateTime.Now;/// single point of reference.
                WriteLogInFolder(string.Format(FilePathFormat, tRef),
                    string.Format(ContentFormat, tRef, logLevel.ToString("g"), format),
                    args);
            }
        }
        public void Log(LogLevel logLevel, Exception ex) { Log(logLevel, ex.ToString()); }

        public void Fatal(string format, params object?[] args) { Log(LogLevel.FATAL, format, args); }
        public void Fatal(Exception ex) { Log(LogLevel.FATAL, ex.ToString()); }
        public void Error(string format, params object?[] args) { Log(LogLevel.ERROR, format, args); }
        public void Error(Exception ex) { Log(LogLevel.ERROR, ex.ToString()); }
        public void Warn(string format, params object?[] args) { Log(LogLevel.WARN, format, args); }
        public void Warn(Exception ex) { Log(LogLevel.WARN, ex.ToString()); }
        public void Info(string format, params object?[] args) { Log(LogLevel.INFO, format, args); }
        public void Info(Exception ex) { Log(LogLevel.INFO, ex.ToString()); }
        public void Debug(string format, params object?[] args) { Log(LogLevel.DEBUG, format, args); }
        public void Debug(Exception ex) { Log(LogLevel.DEBUG, ex.ToString()); }
        public void Trace(string format, params object?[] args) { Log(LogLevel.TRACE, format, args); }
        public void Trace(Exception ex) { Log(LogLevel.TRACE, ex.ToString()); }
    }
}
