using System;
using System.IO;
using System.Configuration;
using System.Text;

namespace BLL.Utilities
{
    public class ServerLog
    {
        private static String ServerLogFile = ConfigurationManager.AppSettings.Get("ServerLogFile");
        private static String InvalidLoginLogFile = ConfigurationManager.AppSettings.Get("InvalidLoginLogFile");
        private static String ExceptionLogFile = ConfigurationManager.AppSettings.Get("ExceptionLogFile");
        private static String ThemeLogFile = ConfigurationManager.AppSettings.Get("ThemeLogFile");
        private static String TrnsLogFile = ConfigurationManager.AppSettings.Get("TransLogFile");
        private static String OTPLogFile = ConfigurationManager.AppSettings.Get("OTPLogFile");
        private static String MailLogFile = ConfigurationManager.AppSettings.Get("MailLogFile");
        private static String RequestLogFile = ConfigurationManager.AppSettings.Get("RequestLogFile");

        //private static String DirectoryPath = System.Reflection.Assembly.LoadFrom(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)) + @"\Log";
        private static String DirectoryPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Log";
        private static String MgmtExceptionLogFile = ConfigurationManager.AppSettings.Get("MgmtExceptionLogFile");
        private static String DateTimeLogFormat = "dd-MMM-yyyy hh:mm:ss:fffffff tt";

        public static void Log(String message)
        {
            FileInfo fi = new FileInfo(ServerLogFile);
            if (!fi.Exists)
            {
                string dr = fi.Directory.FullName;
                ServerLogFile = dr + "\\log.txt";
            }
            FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
            sw.WriteLine(System.DateTime.UtcNow + "-" + message);
            sw.Close();
        }
        public static void SuccessLog(String message)
        {
            FileInfo fi = new FileInfo(TrnsLogFile);
            if (!fi.Exists)
            {
                string dr = fi.Directory.FullName;
                TrnsLogFile = dr + "\\Trnslog.txt";
            }
            FileStream fs = new FileStream(TrnsLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(TrnsLogFile, true, Encoding.ASCII);
            sw.WriteLine(System.DateTime.UtcNow + "-" + message);
            sw.Close();
        }

        public static void RequestLog(String message)
        {
            FileInfo fi = new FileInfo(TrnsLogFile);
            if (!fi.Exists)
            {
                string dr = fi.Directory.FullName;
                RequestLogFile = dr + "\\RequestLog.txt";
            }
            FileStream fs = new FileStream(RequestLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(RequestLogFile, true, Encoding.ASCII);
            sw.WriteLine(System.DateTime.UtcNow + "-" + message);
            sw.Close();
        }


        public static void OTPLog(String message)
        {
            FileInfo fi = new FileInfo(OTPLogFile);
            if (!fi.Exists)
            {
                string dr = fi.Directory.FullName;
                OTPLogFile = dr + "\\OTPlog.txt";
            }
            FileStream fs = new FileStream(OTPLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(OTPLogFile, true, Encoding.ASCII);
            sw.WriteLine(System.DateTime.UtcNow + "-" + message);
            sw.Close();
        }

        public static void MailLog(String message)
        {
            FileInfo fi = new FileInfo(MailLogFile);
            if (!fi.Exists)
            {
                string dr = fi.Directory.FullName;
                MailLogFile = dr + "\\MailLog.txt";
            }
            FileStream fs = new FileStream(MailLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(MailLogFile, true, Encoding.ASCII);
            sw.WriteLine(System.DateTime.UtcNow + "-" + message);
            sw.Close();
        }
        public static void InvalidLoginLog(String message)
        {
            FileStream fs = new FileStream(InvalidLoginLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(InvalidLoginLogFile, true, Encoding.ASCII);
            sw.WriteLine(message);
            sw.Close();

        }
        public static void ExceptionLog(String message)
        {
            FileStream fs = new FileStream(ExceptionLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(ExceptionLogFile, true, Encoding.ASCII);
            sw.WriteLine(message);
            sw.Close();

        }
        public static void ThemeLog(String message)
        {
            FileStream fs = new FileStream(ThemeLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(ThemeLogFile, true, Encoding.ASCII);
            sw.WriteLine(message);
            sw.Close();

        }

        public static void Flush()
        {
            File.Delete(ServerLogFile);
        }

        public static void MgmtExceptionLog(String message)
        {
            WriteLog(MgmtExceptionLogFile, message);

            //StreamWriter sw = null;
            //if (File.Exists(MgmtExceptionLogFile))
            //    sw = File.AppendText(MgmtExceptionLogFile);
            //else
            //    sw = File.CreateText(MgmtExceptionLogFile);
            //sw.WriteLine(DateTime.Now.ToString() + "  " + message);
            //sw.Close();
        }

        private static void WriteLog(String LogFile, String message)
        {
            //Application.StartupPath
            //System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath

            String AutoFlushServerLogFile = "Y";
            long MaxSizeToAutoFlushServerLogFile = 5000000; //in bytes.

            try
            {
                AutoFlushServerLogFile = ConfigurationManager.AppSettings.Get("AutoFlushServerLogFile");
                if (AutoFlushServerLogFile == null || AutoFlushServerLogFile.Trim() == String.Empty)
                    AutoFlushServerLogFile = "Y";

                String LogFileSize = ConfigurationManager.AppSettings.Get("MaxSizeToAutoFlushServerLogFile");
                if (LogFileSize == null || LogFileSize.Trim() == String.Empty)
                    LogFileSize = "5000000";
                MaxSizeToAutoFlushServerLogFile = Convert.ToInt64(LogFileSize);
            }
            catch
            {
                AutoFlushServerLogFile = "Y";
                MaxSizeToAutoFlushServerLogFile = 5000000;
            }

            try
            {
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                String LogFilePath = DirectoryPath + @"\" + LogFile;

                StreamWriter sw = null;
                if (AutoFlushServerLogFile == "Y")
                {
                    if (File.Exists(LogFilePath))
                    {
                        FileInfo objFileInfo = new FileInfo(LogFilePath);
                        if (objFileInfo.Length >= MaxSizeToAutoFlushServerLogFile)
                        {
                            File.Delete(LogFilePath);
                            sw = File.CreateText(LogFilePath);
                            sw.WriteLine("Auto Clear Log file on " + DateTime.UtcNow.ToString(DateTimeLogFormat));
                            sw.WriteLine(DateTime.UtcNow.ToString() + "  " + message);
                            sw.Close();
                            return;
                        }
                    }
                }

                if (!File.Exists(LogFilePath))
                    sw = File.CreateText(LogFilePath);
                else
                    sw = File.AppendText(LogFilePath);
                sw.WriteLine(DateTime.UtcNow.ToString(DateTimeLogFormat) + "  " + message);
                sw.Close();
            }
            catch { }
        }

        private static void Flush(String LogFile)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                String LogFilePath = DirectoryPath + @"\" + LogFile;

                if (File.Exists(LogFilePath))
                    File.Delete(LogFilePath);
                StreamWriter sw = File.CreateText(LogFilePath);
                sw.WriteLine("Clear Log file on " + DateTime.UtcNow.ToString(DateTimeLogFormat));
                sw.Close();
            }
            catch { }
        }

        public static void FlushExceptionLog()
        {
            Flush(ExceptionLogFile);
        }

        public static void FlushALL()
        {
            Flush(ServerLogFile);
            Flush(ExceptionLogFile);
        }
    }
}
