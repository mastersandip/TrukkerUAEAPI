using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

public class Log
{
    private static String ClientRequestLogFile = ConfigurationManager.AppSettings.Get("ClientRequestLogFile");
    private static String InvalidLoginLogFile = ConfigurationManager.AppSettings.Get("InvalidLoginLogFile");
    private static String ExceptionLogFile = ConfigurationManager.AppSettings.Get("ExceptionLogFile");
    private static String DateTimeLogFormat = "dd-MMM-yyyy hh:mm:ss:fffffff tt";

    private static void WriteLog(String LogFileName, String message)
    {
        //System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath

        String AutoFlushServerLogFile = "Y";
        long MaxSizeToAutoFlushServerLogFile = 5000000; //in bytes.

        try
        {
            AutoFlushServerLogFile = ConfigurationManager.AppSettings.Get("AutoFlushServerLogFile");
            if (AutoFlushServerLogFile == null)
                AutoFlushServerLogFile = "Y";

            String LogFileSize = ConfigurationManager.AppSettings.Get("MaxSizeToAutoFlushServerLogFile");
            if (LogFileSize == null)
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
            //String DirectoryPath = System.Reflection.Assembly.LoadFrom(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)) + @"\Log";
            String DirectoryPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Log";
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            String LogFilePath = DirectoryPath + @"\" + LogFileName;

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

    public static void ClientRequestLog(String message)
    {
        WriteLog(ClientRequestLogFile, message);
    }

    public static void InvalidLoginLog(String message)
    {
        WriteLog(InvalidLoginLogFile, message);
    }

    public static void ExceptionLog(String message)
    {
        WriteLog(ExceptionLogFile, message);
    }

    private static void Flush(String LogFile)
    {
        try
        {
            File.Delete(LogFile);
            StreamWriter sw = File.CreateText(LogFile);
            sw.WriteLine("Clear Log file on " + DateTime.UtcNow.ToString(DateTimeLogFormat));
            sw.Close();
        }
        catch { }
    }

    public static void FlushClientRequestLog()
    {
        Flush(ClientRequestLogFile);
    }

    public static void FlushInvalidLoginLog()
    {
        Flush(InvalidLoginLogFile);
    }

    public static void FlushExceptionLog()
    {
        Flush(ExceptionLogFile);
    }

    public static void FlushALL()
    {
        Flush(ClientRequestLogFile);
        Flush(InvalidLoginLogFile);
        Flush(ExceptionLogFile);
    }
}