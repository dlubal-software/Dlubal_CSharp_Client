using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Dlubal.WS.Common.Tools
{
    public class DataLogger : IDisposable
    {
        public enum LogResultType
        {
            FAILED = 0,
            CANCELED = 1,
            DONE = 2,
        }

        /// <summary>
        /// Returns empty DataLogger to avoid NullReferenceException.
        /// </summary>
        public static readonly DataLogger DummyDataLogger = new DataLogger(string.Empty, "Dummy DataLogger", true, null, DateTime.Now);

        #region Static Methods
        public static DataLogger Current { get; set; } = DummyDataLogger;

        /// <summary>
        /// This method initializes a new instance of DataLogger and starts logging.
        /// </summary>
        /// <param name="projectName">Name of the project/application.</param>
        /// <param name="rootDirectory">Root directory for log files.</param>
        /// <param name="isWebApplication">Value indicating that DataLogger is used with web based application.</param>
        /// <param name="dataLogger">Output reference to DataLogger instance.</param>
        /// <param name="logBoxReference">Reference to TextBoxBase component that can show log content.</param>
        /// <returns>Operation status.</returns>
        public static OperationStatus InitializeNewDataLogger(string projectName, string rootDirectory, bool isWebApplication, out DataLogger dataLogger, TextBox logBoxReference = null)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                dataLogger = DummyDataLogger;
                return new OperationStatus(OperationStatusType.ERROR, "InitializeNewDataLogger() - Project name is empty!");
            }

            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                rootDirectory = Path.Combine(rootDirectory, "Logs");
            }

            string filePath = string.Empty;

            try
            {
                // Check or create root directory.
                DirectoryInfo root = new DirectoryInfo(rootDirectory);
                if (!Directory.Exists(rootDirectory))
                {
                    root = Directory.CreateDirectory(rootDirectory);
                }

                // Check or create date subdirectory.
                DateTime currentDateTime = DateTime.Now;
                string dateDirectoryPath = Path.Combine(root.FullName, currentDateTime.ToString("yyyy-MM-dd"));
                DirectoryInfo dateDirectory = new DirectoryInfo(dateDirectoryPath);

                if (!Directory.Exists(dateDirectoryPath))
                {
                    dateDirectory = Directory.CreateDirectory(dateDirectoryPath);
                }

                filePath = Path.Combine(dateDirectory.FullName, projectName + "_" + currentDateTime.ToString("HH-mm-ss-ffff") + ".log");
                dataLogger = new DataLogger(filePath, projectName, isWebApplication, logBoxReference, currentDateTime);

                DataLogger.Current = dataLogger;
            }
            catch (Exception exception)
            {
                dataLogger = DummyDataLogger;
                return new OperationStatus(OperationStatusType.ERROR, "InitializeNewDataLogger() - " + exception.Message);
            }
            finally
            {
                CleanUpLogs(rootDirectory);
            }

            return new OperationStatus(OperationStatusType.OK, "Log file path: " + filePath);
        }

        /// <summary>
        /// This methods stops logging and closes selected log file.
        /// </summary>
        /// <param name="dataLogger">Reference to DataLogger instance that should be closed.</param>
        /// <returns>Operation status.</returns>
        public static OperationStatus CloseLog(ref DataLogger dataLogger)
        {
            if ((dataLogger != null) && (dataLogger != DummyDataLogger))
            {
                dataLogger.CloseLog();
            }

            if (Current == dataLogger)
            {
                Current = DummyDataLogger;
            }

            dataLogger = DummyDataLogger;

            return new OperationStatus(OperationStatusType.OK, "Log file was closed.");
        }

        /// <summary>
        /// This method cleans all directories with logs that are older than one week.
        /// </summary>
        /// <param name="rootDirectoryPath">Root directory path.</param>
        /// <returns>Operation status.</returns>
        public static OperationStatus CleanUpLogs(string rootDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(rootDirectoryPath))
            {
                return new OperationStatus(OperationStatusType.ERROR, "CleanUpLogs() - Root directory was not set!");
            }

            try
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(rootDirectoryPath);

                if (!Directory.Exists(rootDirectoryPath))
                {
                    return new OperationStatus(OperationStatusType.ERROR, "CleanUpLogs() - Root directory doesn't exist!");
                }

                DateTime currentDate = DateTime.Now;
                DateTime minDate = currentDate.Date.AddDays(-7);

                foreach (DirectoryInfo directory in rootDirectory.GetDirectories())
                {
                    if (!DateTime.TryParseExact(directory.Name, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime directoryDate))
                    {
                        continue;
                    }

                    if (directoryDate <= minDate)
                    {
                        Directory.Delete(directory.FullName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationStatus(OperationStatusType.ERROR, "CleanUpLogs() - " + ex.Message);
            }

            return new OperationStatus(OperationStatusType.OK, "Old logs have been cleaned.");
        }
        #endregion // Static Methods

        #region DataLogger fields and properties
        private FileStream fileStream;
        private StreamWriter log;
        private readonly DateTime loggingStartDateTime = DateTime.Now;
        private DateTime progressBarInitTime = DateTime.Now;

        // Contains current message offset from the beginning of the line.
        private string offset = string.Empty;

        public string FullPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the reference to TextBox based control which can be used to show log.
        /// </summary>
        public TextBox LogBox { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the LogBox is scrolled automatically to the end when new text is inserted.
        /// </summary>
        public bool AutoScrollLogBox { get; set; } = true;

        /// <summary>
        /// Gets or sets the reference to ProgressBar control which can be used to signalize progress of operation.
        /// </summary>
        public ProgressBar ProgressBar { get; set; } = null;

        /// <summary>
        /// Gets a value indicating whether this DataLogger is used with web based application (MessageBoxes should not be thrown).
        /// </summary>
        public bool IsWebApplication { get; private set; } = false;

        public bool WriteCurrentTime { get; set; } = true;

        public bool WriteDifferentialTime
        {
            get
            {
                return !WriteCurrentTime;
            }

            set
            {
                WriteCurrentTime = !value;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return !string.IsNullOrEmpty(FullPath) && (log != null);
            }
        }
        #endregion // DataLogger fields and properties

        #region DataLogger initialization and closing
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLogger"/> class and adds header to log.
        /// </summary>
        /// <param name="filePath">Full path to log file.</param>
        /// <param name="projectName">Name of the project/application.</param>
        /// <param name="isWebApplication">Value indicating that DataLogger instance is used with web based application.</param>
        /// <param name="logBoxReference">Reference to TextBoxBase instance that will show log content.</param>
        /// <param name="currentDateTime">Start date and time of the log.</param>
        private DataLogger(string filePath, string projectName, bool isWebApplication, TextBox logBoxReference, DateTime currentDateTime)
        {
            IsWebApplication = isWebApplication;

            try
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                    log = new StreamWriter(fileStream)
                    {
                        AutoFlush = true
                    };

                    FullPath = filePath;
                }
                else
                {
                    FullPath = string.Empty;
                }

                LogBox = logBoxReference;
                loggingStartDateTime = currentDateTime;
                AddHeader(projectName);
                AddBasicInformation();
            }
            catch (Exception exception)
            {
                log = null;
                FullPath = string.Empty;
                throw new InvalidOperationException("Initialization of a new DataLogger failed: " + Environment.NewLine + exception.Message);
            }
        }

        /// <summary>
        /// This methods stops logging and closes the log file.
        /// </summary>
        private void CloseLog()
        {
            if (log != null)
            {
                AddText("Closing log file...");
                log.Close();
                log.Dispose();
                log = null;

                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                FullPath = string.Empty;
            }

            if (ProgressBar != null)
            {
                InitializeProgressBar();
            }
        }

        /// <summary>
        /// This method adds header to the log.
        /// </summary>
        /// <param name="projectName">Name of the project/application.</param>
        private void AddHeader(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                return;
            }

            if (log != null)
            {
                log.WriteLine("========================================================================================");
                log.WriteLine("  " + projectName);
                log.WriteLine("========================================================================================");
            }

            if (LogBox != null)
            {
                AddTextToLogBox("========================================================================================" + Environment.NewLine);
                AddTextToLogBox("  " + projectName + Environment.NewLine);
                AddTextToLogBox("========================================================================================" + Environment.NewLine);
            }
        }

        /// <summary>
        /// This method adds basic information about OS to log.
        /// </summary>
        private void AddBasicInformation()
        {
            string platform;

            if (IntPtr.Size == 4)
            {
                platform = "x86";
            }
            else if (IntPtr.Size == 8)
            {
                platform = "x64";
            }
            else
            {
                platform = "Unknown";
            }

            AddText("Operation System : {0}", Environment.OSVersion.VersionString);
            AddText("Platform         : {0}", platform);
            AddText("Time             : {0} {1}", loggingStartDateTime.ToShortDateString(), loggingStartDateTime.ToLongTimeString());
#if DEBUG
            AddText("Configuration    : DEBUG");
#else
            AddText("Configuration    : RELEASE");
#endif
            AddText("Log path         : {0}", FullPath);
            AddSeparator();
        }

        /// <summary>
        /// This method shows content of log file in notepad.
        /// It should be used just for desktop applications or debugging of web applications.
        /// </summary>
        /// <returns>Operation status.</returns>
        public OperationStatus ShowLogFile()
        {
            if (IsInitialized)
            {
                try
                {
                    System.Diagnostics.Process.Start(FullPath);

                    return new OperationStatus(OperationStatusType.OK, string.Empty);
                }
                catch (System.ComponentModel.Win32Exception exception)
                {
                    return new OperationStatus(OperationStatusType.ERROR, exception.Message);
                }
            }

            return new OperationStatus(OperationStatusType.WARNING, "DataLogger is not initialized!");
        }

        public void InitializeProgressBar(double min = 0, double max = 100, double value = 0)
        {
            if (ProgressBar == null)
            {
                return;
            }

            if (!ProgressBar.Dispatcher.CheckAccess())
            {
                ProgressBar.Dispatcher.Invoke(new Action<double, double, double>(InitializeProgressBar), new object[] { min, max, value });
            }
            else
            {
                ProgressBar.Minimum = min;
                ProgressBar.Maximum = max;
                ProgressBar.Value = value;

                progressBarInitTime = DateTime.Now;

                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(delegate { }));
                }
            }
        }
        #endregion // DataLogger initialization and closing

        #region ProgressBar
        public void SetProgressBarValue(double value)
        {
            if (ProgressBar == null)
            {
                return;
            }

            if (!ProgressBar.Dispatcher.CheckAccess())
            {
                ProgressBar.Dispatcher.Invoke(new Action<double>(SetProgressBarValue), new object[] { value });
            }
            else
            {
                ProgressBar.Value = value;

                // Omezeni poctu prekreslovani ProgressBaru.
                if (((DateTime.Now - progressBarInitTime).Milliseconds > 250) || (value >= ProgressBar.Maximum))
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(delegate { }));
                    }

                    progressBarInitTime = DateTime.Now;
                }
            }
        }

        public void ResetProgressBar()
        {
            if (ProgressBar == null)
            {
                return;
            }

            InitializeProgressBar(0, 100, 0);
        }
        #endregion // ProgressBar

        #region Logging methods
        /// <summary>
        /// This method increments the offset of a messages.
        /// </summary>
        public void IncrementOffset()
        {
            offset += "  ";
        }

        /// <summary>
        /// This method decrements the offset of a messages.
        /// </summary>
        public void DecrementOffset()
        {
            if (offset.Length < 2)
            {
                offset = string.Empty;
            }
            else
            {
                offset = offset.Substring(0, offset.Length - 2);
            }
        }

        /// <summary>
        /// Adds text to TextBox based component - Invoke is called if required.
        /// </summary>
        /// <param name="text">Text, that should be added to TextBox based component.</param>
        private void AddTextToLogBox(string text)
        {
            if (LogBox == null)
            {
                return;
            }

            if (!LogBox.Dispatcher.CheckAccess())
            {
                LogBox.Dispatcher.Invoke(new Action<string>(AddTextToLogBox), new object[] { text });
            }
            else
            {
                LogBox.AppendText(text);

                if (AutoScrollLogBox)
                {
                    LogBox.ScrollToEnd();
                }
            }
        }

        /// <summary>
        /// This method appends message to the log.
        /// </summary>
        /// <param name="message">Message text.</param>
        public void AddText(string message)
        {
            string timeStamp = WriteCurrentTime ? DateTime.Now.ToString("HH:mm:ss.fff") + " | " :
                (DateTime.Now - loggingStartDateTime).ToString("HH:mm:ss.fff") + " | ";

            StringBuilder stringBuilder = new StringBuilder();

            using (StringReader stringReader = new StringReader(message))
            {
                string line = string.Empty;

                while ((line = stringReader.ReadLine()) != null)
                {
                    stringBuilder.AppendLine(timeStamp + offset + line);
                }
            }

            string finalMessage = stringBuilder.ToString();

            if (log != null)
            {
                log.Write(finalMessage);
            }

            if (LogBox != null)
            {
                AddTextToLogBox(finalMessage);

                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(delegate { }));
                }
            }
        }

        /// <summary>
        /// This method appends formatted message to the log.
        /// </summary>
        /// <param name="message">Message formatting string.</param>
        /// <param name="args">Message parameters.</param>
        public void AddText(string message, params object[] args)
        {
            AddText(string.Format(message, args));
        }

        /// <summary>
        /// This method appends specified action result string to the log.
        /// </summary>
        /// <param name="result">Result which will be written to the log.</param>
        public void AddResult(LogResultType result)
        {
            AddText("..." + result.ToString());
        }

        /// <summary>
        /// This method writes an error message to the log.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void AddError(string message)
        {
            if (message.Contains(Environment.NewLine))
            {
                AddLogStart("ERROR:");
                AddText(message);
                DecrementOffset();
                AddText("!!!");
            }
            else
            {
                AddText("ERROR: " + message);
            }
        }

        /// <summary>
        /// This method writes an assert message to the log.
        /// </summary>
        /// <param name="message">Assert message.</param>
        public void AddAssert(string message)
        {
            if (message.Contains(Environment.NewLine))
            {
                AddLogStart("ASSERTION FAILED:");
                AddText(message);
                DecrementOffset();
                AddText("!!!");
            }
            else
            {
                AddText("ASSERTION FAILED: " + message);
            }
        }

        /// <summary>
        /// This method writes details of an exception to the log.
        /// </summary>
        /// <param name="message">Exception details.</param>
        public void AddException(string message)
        {
            if (message.Contains(Environment.NewLine))
            {
                AddLogStart("EXCEPTION:");
                AddText(message);
                DecrementOffset();
                AddText("!!!");
            }
            else
            {
                AddText("EXCEPTION: " + message);
            }
        }

        /// <summary>
        /// This method writes a warning message to the log.
        /// </summary>
        /// <param name="message">Warning message.</param>
        public void AddWarning(string message)
        {
            if (message.Contains(Environment.NewLine))
            {
                AddLogStart("WARNING:");
                AddText(message);
                DecrementOffset();
                AddText("!!!");
            }
            else
            {
                AddText("WARNING: " + message);
            }
        }

        /// <summary>
        /// This method appends separator line to the log.
        /// </summary>
        public void AddSeparator()
        {
            if (log != null)
            {
                log.WriteLine("----------------------------------------------------------------------------------------");
            }

            if (LogBox != null)
            {
                AddTextToLogBox("----------------------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        /// <summary>
        /// This method shows an error message to the user. The message is also written to the log.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowError(string message)
        {
            AddError(message);

            if (!IsWebApplication)
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This method shows an error message to the user. The message is also written to the log.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowAssert(string message)
        {
            AddAssert(message);

            if (!IsWebApplication)
            {
                MessageBox.Show(message, "Assertion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This method shows an details of an exception to the user. The message is also written to the log.
        /// </summary>
        /// <param name="message">Message with details of an exception.</param>
        public void ShowException(string message)
        {
            AddException(message);

            if (!IsWebApplication)
            {
                MessageBox.Show(message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This method shows a warning message to the user. The message is also written to the log.
        /// </summary>
        /// <param name="message">Warning message.</param>
        public void ShowWarning(string message)
        {
            AddWarning(message);

            if (!IsWebApplication)
            {
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// This method shows an information message to the user. The message is also written to the log.
        /// </summary>
        /// <param name="message">Information message.</param>
        public void ShowInformation(string message)
        {
            AddText(message);

            if (!IsWebApplication)
            {
                MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// This method appends header for nested block of messages to the log.
        /// </summary>
        /// <param name="message">Header of message block.</param>
        public void AddLogStart(string message)
        {
            AddText(message);
            IncrementOffset();
        }

        /// <summary>
        /// This method appends header for nested block of messages to the log.
        /// </summary>
        /// <param name="message">Parametric header of message block.</param>
        /// <param name="args">Parametric header parameters.</param>
        public void AddLogStart(string message, params object[] args)
        {
            AddLogStart(string.Format(message, args));
        }

        /// <summary>
        /// This method appends end of nested block of messages to the log.
        /// </summary>
        /// <param name="succeeded">True if operations in the block succeeded, otherwise false.</param>
        public void AddLogEnd(bool succeeded)
        {
            AddLogEnd(succeeded ? LogResultType.DONE : LogResultType.FAILED);
        }

        /// <summary>
        /// This method appends end of nested block of messages to the log.
        /// </summary>
        /// <param name="result">Result of an action.</param>
        public void AddLogEnd(LogResultType result)
        {
            DecrementOffset();
            AddResult(result);
        }

        /// <summary>
        /// This method reports details of an exception to the user and closes the block of nested messages
        /// (opened by method AddLogStart()) with negative result.
        /// </summary>
        /// <param name="exception">Reference to an exception.</param>
        /// <param name="beSilent">
        /// If True then the message is only written to the log otherwise the MessageBox is also showed.
        /// </param>
        /// <returns>Message containing information about exception.</returns>
        public string ReportError(Exception exception, bool beSilent = false)
        {
            string message = Utilities.ParseException(exception);

            if (beSilent)
            {
                AddException(message);
            }
            else
            {
                ShowException(message);
            }

            AddLogEnd(LogResultType.FAILED);
            return message;
        }

        /// <summary>
        /// This method reports error message to the user and closes the block of nested messages
        /// (opened by method AddLogStart()) with negative result.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="beSilent">
        /// If True then the message is only written to the log otherwise the MessageBox is also showed.
        /// </param>
        public void ReportError(string message, bool beSilent = false)
        {
            if (beSilent)
            {
                AddError(message);
            }
            else
            {
                ShowError(message);
            }

            AddLogEnd(LogResultType.FAILED);
        }

        public void Assert(bool condition, string message)
        {
            if (!condition)
            {
                string stackTrace = Utilities.GetStackTrace();
#if DEBUG
                ShowAssert(string.Format($"{message}{Environment.NewLine}{Environment.NewLine}{stackTrace}"));
#else
                AddAssert(string.Format($"{message}{Environment.NewLine}{Environment.NewLine}{stackTrace}"));
#endif
            }
        }
        #endregion // Logging methods

        #region Console logging methods

        public void WriteToLogAndConsole(string message, bool error = false)
        {
            Console.WriteLine(message);

            if (error)
            {
                AddError(message);
            }
            else
            {
                AddText(message);
            }
        }

        #endregion Console logging methods

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseLog();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion // IDisposable Support
    }
}
