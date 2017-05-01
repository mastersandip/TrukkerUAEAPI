using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading;
using System.IO;
using WkHtmlToXSharp;


public class ReportPrinter
{
//    private static readonly global::Common.Logging.ILog _Log = global::Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//    public string PageFile { get; set; }
//    public string HeaderFile { get; set; }
//    public string FooterFile { get; set; }
//    public static int count = 0;
//    public string pdfFile { get; set; }
//    private const int ConcurrentTimeout = 50000;
//    public string PathToSave { get; set; }

//    public string GetPdf()
//    {
//        CanConvertConcurrently();

//        return pdfFile;
//    }

//    private MultiplexingConverter _GetConverter()
//    {
//        var obj = new MultiplexingConverter();
//        obj.Begin += (s, e) => _Log.DebugFormat("Conversion begin, phase count: {0}", e.Value);
//        obj.Error += (s, e) => _Log.Error(e.Value);
//        obj.Warning += (s, e) => _Log.Warn(e.Value);
//        obj.PhaseChanged += (s, e) => _Log.InfoFormat("PhaseChanged: {0} - {1}", e.Value, e.Value2);
//        obj.ProgressChanged += (s, e) => _Log.InfoFormat("ProgressChanged: {0} - {1}", e.Value, e.Value2);
//        obj.Finished += (s, e) => _Log.InfoFormat("Finished: {0}", e.Value ? "success" : "failed!");
//        return obj;
//    }

//    private void _SimpleConversion()
//    {
//        using (var wk = _GetConverter())
//        {
//            _Log.DebugFormat("Performing conversion..");

//            wk.GlobalSettings.Margin.Top = "1.5cm";
//            wk.GlobalSettings.Margin.Bottom = "4cm";
//            wk.GlobalSettings.Margin.Left = "0cm";
//            wk.GlobalSettings.Margin.Right = "0cm";

//            //wk.GlobalSettings.Out = @"c:\temp\tmp.pdf";

//            //wk.ObjectSettings.footer.fontSize = 13;
//            //wk.ObjectSettings.footer.htmlUrl = FooterFile;
//            //wk.ObjectSettings.footer.line = true;
//            //wk.ObjectSettings.footer.fontName = "times";

//            // wk.ObjectSettings.header.center = "header";
//            //wk.ObjectSettings.header.line = true;
//            //wk.ObjectSettings.header.fontSize = 13;
//            //wk.ObjectSettings.header.fontName = "times";
//            //wk.ObjectSettings.header.htmlUrl = HeaderFile;


//            wk.ObjectSettings.Web.EnablePlugins = false;
//            wk.ObjectSettings.Web.EnableJavascript = true;
//            wk.ObjectSettings.Load.LoadErrorHandling = LoadErrorHandlingType.ignore;
//            //wk.ObjectSettings.Page = SimplePageFile;

//            wk.ObjectSettings.Page = PageFile;
//            // wk.ObjectSettings.Page = "http://www.google.com";
//            //wk.ObjectSettings.Page = @"C:\Users\user\Desktop\TempHtml.html";
//            wk.ObjectSettings.Load.Proxy = "none";

//            var tmp = wk.Convert();


//            var number = 0;
//            //lock (this) number = count++;
//            // string FileName = "PDFPrint"; //PageFile.Substring(PageFile.LastIndexOf('/') + 1, PageFile.IndexOf(".aspx") - (PageFile.LastIndexOf('/') + 1));
//            string FileName = Guid.NewGuid().ToString() + ".pdf";
//            pdfFile = PathToSave + FileName;///@"c:\temp\tmp" + (number) + @".pdf";

//            File.WriteAllBytes(pdfFile, tmp);
//            //Process.Start(@"c:\temp\tmp" + (number) + ".pdf");
//        }
//    }


//    class ThreadData
//    {
//        public Thread Thread;
//        public Exception Exception;
//        public ManualResetEvent WaitHandle;
//    }

//    void ThreadStart(object arg)
//    {
//        _Log.DebugFormat("New thread {0}", arg);

//        var tmp = arg as ThreadData;
//        try
//        {
//            _SimpleConversion();
//        }
//        catch (Exception ex)
//        {
//            tmp.Exception = ex;
//        }
//        finally
//        {
//            tmp.WaitHandle.Set();
//        }
//    }





//    public void CanConvertConcurrently()
//    {
//        var error = false;
//        var threads = new List<ThreadData>();

//        //for (int i = 0; i < 5; i++)
//        //{
//        var tmp = new ThreadData()
//        {
//            Thread = new Thread(ThreadStart),
//            WaitHandle = new ManualResetEvent(false)
//        };
//        threads.Add(tmp);
//        tmp.Thread.Start(tmp);
//        // }

//        var handles = threads.Select(x => x.WaitHandle).ToArray();
//        WaitHandle.WaitAll(handles, ConcurrentTimeout);
//        //WaitAll(handles);

//        threads.ForEach(x => x.Thread.Abort());

//        var exceptions = threads.Select(x => x.Exception).Where(x => x != null);

//        foreach (var tmp1 in threads)
//        {
//            if (tmp1.Exception != null)
//            {
//                error = true;
//                var tid = tmp1.Thread.ManagedThreadId;
//                _Log.Error("Thread-" + tid + " failed!", tmp1.Exception);
//            }
//        }

//        // Assert.IsFalse(error, "At least one thread failed!");
//    }

//    public static void SetSession(string qStringSession)
//    {
//        JavaScriptSerializer ser = new JavaScriptSerializer();

//        Dictionary<string, object> sessionDic = new Dictionary<string, object>();

//        if (qStringSession != null)
//        {
//            sessionDic = ser.Deserialize<Dictionary<string, object>>(qStringSession);
//        }

//        HttpContext.Current.Session.Clear();

//        if (sessionDic.Keys.Count > 0)
//        {
//            foreach (string key in sessionDic.Keys)
//            {
//                HttpContext.Current.Session[key] = sessionDic[key];
//            }
//        }
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Script.Serialization;
//using System.Threading;
//using WkHtmlToXSharp;

//namespace CEPT.BLL.ExtraUtilities
//{
//    public class ReportPrinter
//    {
        private static readonly global::Common.Logging.ILog _Log = global::Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string MarginTop { get; set; }
        public bool HeaderLine { get; set; }
        public bool FooterLine { get; set; }
        public string PageFile { get; set; }
        public string HeaderFile { get; set; }
        public string FooterFile { get; set; }
        public static int count = 0;
        public string pdfFile { get; set; }
        private const int ConcurrentTimeout = 9000000;
        public string PathToSave { get; set; }
        public string MarginBottom { get; set; }
        public byte[] FileContent { get; set; }
        public PdfOrientation Orientation { get; set; }

        public ReportPrinter()
        {
            HeaderLine = true;
            FooterLine = true;
            MarginTop = "1.5cm";
            MarginBottom = "1cm";
            Orientation = PdfOrientation.Portrait;
        }

        public void GetPdf()
        {
            CanConvertConcurrently();

            // return pdfFile;
        }

        private MultiplexingConverter _GetConverter()
        {
            var obj = new MultiplexingConverter();
            obj.Begin += (s, e) => _Log.DebugFormat("Conversion begin, phase count: {0}", e.Value);
            obj.Error += (s, e) => _Log.Error(e.Value);
            obj.Warning += (s, e) => _Log.Warn(e.Value);
            obj.PhaseChanged += (s, e) => _Log.InfoFormat("PhaseChanged: {0} - {1}", e.Value, e.Value2);
            obj.ProgressChanged += (s, e) => _Log.InfoFormat("ProgressChanged: {0} - {1}", e.Value, e.Value2);
            obj.Finished += (s, e) => _Log.InfoFormat("Finished: {0}", e.Value ? "success" : "failed!");
            return obj;
        }

        private void _SimpleConversion()
        {
            using (var wk = _GetConverter())
            {
                _Log.DebugFormat("Performing conversion..");

                wk.GlobalSettings.Margin.Top = MarginTop;
                wk.GlobalSettings.Margin.Bottom = MarginBottom;
                wk.GlobalSettings.Margin.Left = "0cm";
                wk.GlobalSettings.Margin.Right = "0cm";

                //wk.GlobalSettings.Out = @"c:\temp\tmp.pdf";

                wk.ObjectSettings.footer.fontSize = 13;
                wk.ObjectSettings.footer.htmlUrl = FooterFile;
                wk.ObjectSettings.footer.line = HeaderLine;
                wk.ObjectSettings.footer.fontName = "times";

                // wk.ObjectSettings.header.center = "header";
                //wk.ObjectSettings.header.line = HeaderLine;
                wk.ObjectSettings.header.fontSize = 13;
                wk.ObjectSettings.header.fontName = "times";
                wk.ObjectSettings.header.htmlUrl = HeaderFile;


                wk.ObjectSettings.Web.EnablePlugins = false;
                wk.ObjectSettings.Web.EnableJavascript = true;
                wk.ObjectSettings.Load.LoadErrorHandling = LoadErrorHandlingType.ignore;
                //wk.ObjectSettings.Page = SimplePageFile;
                wk.ObjectSettings.Page = PageFile;

                wk.GlobalSettings.Orientation = Orientation;

                //   wk.ObjectSettings.Page = "http://www.google.com";
                wk.ObjectSettings.Load.Proxy = "none";

                FileContent = wk.Convert();


                //var number = 0;
                //lock (this) number = count++;
                //string FileName = PageFile.Substring(PageFile.LastIndexOf('/') + 1, PageFile.IndexOf(".aspx") - (PageFile.LastIndexOf('/') + 1));
                //pdfFile = PathToSave + FileName + DateTime.Now.ToString("MM.dd.yyyy HH.mm.ss") + ".pdf";  //@"c:\temp\tmp" + (number) + @".pdf";

                //File.WriteAllBytes(pdfFile, FileContent);
                //Process.Start(@"c:\temp\tmp" + (number) + ".pdf");
            }
        }


        class ThreadData
        {
            public Thread Thread;
            public Exception Exception;
            public ManualResetEvent WaitHandle;
        }

        void ThreadStart(object arg)
        {

            _Log.DebugFormat("New thread {0}", arg);

            var tmp = arg as ThreadData;
            try
            {
                _SimpleConversion();
            }
            catch (Exception ex)
            {
                tmp.Exception = ex;



            }
            finally
            {
                tmp.WaitHandle.Set();
            }
        }

        public void CanConvertConcurrently()
        {
            var error = false;
            var threads = new List<ThreadData>();

            //for (int i = 0; i < 5; i++)
            //{
            var tmp = new ThreadData()
            {
                Thread = new Thread(ThreadStart),
                WaitHandle = new ManualResetEvent(false)
            };
            threads.Add(tmp);
            tmp.Thread.Start(tmp);
            // }

            var handles = threads.Select(x => x.WaitHandle).ToArray();
            WaitHandle.WaitAll(handles, ConcurrentTimeout);
            //WaitAll(handles);

            threads.ForEach(x => x.Thread.Abort());

            var exceptions = threads.Select(x => x.Exception).Where(x => x != null);

            foreach (var tmp1 in threads)
            {
                if (tmp1.Exception != null)
                {
                    error = true;
                    var tid = tmp1.Thread.ManagedThreadId;
                    _Log.Error("Thread-" + tid + " failed!", tmp1.Exception);
                }
            }

            // Assert.IsFalse(error, "At least one thread failed!");
        }

        public static void SetSession(string qStringSession)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            Dictionary<string, object> sessionDic = new Dictionary<string, object>();

            if (qStringSession != null)
            {
                sessionDic = ser.Deserialize<Dictionary<string, object>>(qStringSession);
            }

            HttpContext.Current.Session.Clear();

            if (sessionDic.Keys.Count > 0)
            {
                foreach (string key in sessionDic.Keys)
                {
                    HttpContext.Current.Session[key] = sessionDic[key];
                }
            }
        }


    }
//}