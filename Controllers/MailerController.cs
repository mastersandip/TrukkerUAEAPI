using BLL.Master;
using BLL.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using trukkerUAE.BLL.Master;
using trukkerUAE.Classes;
using trukkerUAE.Controllers;
using trukkerUAE.Models;
using trukkerUAE.XSD;

namespace TrkrLite.Controllers
{
    public class MailerController : ServerBase
    {

        [HttpPost]
        public string SendEMail([FromBody]JObject jobj)
        {
            string imageURL = ConfigurationManager.AppSettings["ImageURL"].ToString();
            string strSubInter = ConfigurationManager.AppSettings["SUBINTER"].ToString();
            string strSubVender = ConfigurationManager.AppSettings["SUBVENDER"].ToString();
            string strSubpartner = ConfigurationManager.AppSettings["SUBPARTNER"].ToString();
            string strSubgoods = ConfigurationManager.AppSettings["SUBGOODS"].ToString();
            string strSubhome = ConfigurationManager.AppSettings["SUBHOME"].ToString();
            string strSubHoliday = ConfigurationManager.AppSettings["SUBHOLIDAY"].ToString();
            string strSubBHM = ConfigurationManager.AppSettings["SUBBETTERHOME"].ToString();
            string strSubOfficeMove = ConfigurationManager.AppSettings["SUBOFFICEMOVE"].ToString();
            //string mailto,string message
            //List<Mailer> tload = new List<Mailer>();
            //tload = jobj["Email"].ToObject<List<Mailer>>();
            DataSet dsemaillog = new DataSet();
            DS_InquiryEmailLog dsemail = new DS_InquiryEmailLog();
            List<InquiryEmailLog> emlog = new List<InquiryEmailLog>();
            Master master = new Master(); Document objDocument = new Document();
            string logid = ""; string msg = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();
            Boolean bl;

            if (jobj["Email"] != null)
            {
                emlog = jobj["Email"].ToObject<List<InquiryEmailLog>>();
                dsemaillog = master.CreateDataSet(emlog);
                //User usr = JsonConvert.DeserializeObject<User>(jobj["User"].ToString());
            }
            if (emlog[0].Message == null || emlog[0].Message.ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Message Not provided");
            }


            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                if (dsemaillog != null && dsemaillog.Tables.Count > 0 && dsemaillog.Tables[0].Rows.Count > 0)
                {
                    EMail objemail = new EMail();
                    msg = "";
                    StreamReader sr;

                    string strdata = "";
                    if (emlog[0].Message == "P")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_RealEstateAgentMail.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "I")
                    {
                        strdata = "<div style='width:100%;'><img src='http://trukker.ae/trukkerUAEAPI/Images/MailerImage/Intern_Mailer.jpg'></div>";
                    }
                    else if (emlog[0].Message == "V")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_VendorMail.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                        //strdata = "<div style='width:100%;'><img src='http://trukker.ae/trukkerUAEAPI/Images/MailerImage/Vendor_Mailer.jpg'></div><div style='width:100%;padding-top:1%'><a href='http://www.trukker.ae/become-partner.html' target='_blank' ><img src='http://52.35.101.81/trukkerUAEAPI/Images/MailerImage/button.jpg' ></a></div><div style='width:100%;padding-top:1%'><img src='http://52.35.101.81/trukkerUAEAPI/Images/MailerImage/orange_base_line.png'></div>";
                    }
                    else if (emlog[0].Message == "G")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_MovingGoodsMailer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "H")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_MovingHomeMailer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "HLD")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_Holidaymailer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "BHM")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_BHomeMailer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "OFM")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OfficeMoveFlyer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else if (emlog[0].Message == "GSM")
                    {
                        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_MovingGoodsShortMailer.html");
                        while (!sr.EndOfStream)
                        {
                            strdata = sr.ReadToEnd();
                            strdata = strdata.Replace("DOMAIN", ConfigurationManager.AppSettings["Domain"]);
                        }
                        sr.Close();
                    }
                    else
                        strdata = emlog[0].Message;

                    BLGeneralUtil blgn = new BLGeneralUtil();
                    //DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref msg); msg = "";
                    //if (dt_param == null)
                    //{
                    //    ServerLog.Log(msg);
                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //    return BLGeneralUtil.return_ajax_string("0", msg);
                    //}
                    //string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    //string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; msg = "";
                    if (emlog[0].Message != "")
                    {
                        if (emlog[0].EmailID == null || emlog[0].EmailID.ToString() == "")
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", "Email Id not provided");
                        }
                        sendto = emlog[0].EmailID.ToString();
                        if (emlog[0].Message == "I")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubInter, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Intern Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Intern Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "V")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubVender, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Vendor Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Vendor Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "P")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubpartner, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Re Agent Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Re Agent Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "G")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubgoods, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Goods Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Goods Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "H")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubhome, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Home Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Home Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "HLD")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubHoliday, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Holiday Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "BHM")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubBHM, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Better Home Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "OFM")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubOfficeMove, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Office Move flyer Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Email on " + sendto + Environment.NewLine + msg);
                            }
                        else if (emlog[0].Message == "GSM")
                            foreach (var address in emlog[0].EmailID.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                bl = objemail.SendMail(address.Trim(), strdata, strSubgoods, ref msg, "CONTACT", "SENDFROM");
                                if (bl)
                                    ServerLog.MailLog("Goods Move Short Mail Sent Sucessfully : " + sendto);
                                else
                                    ServerLog.MailLog("Error Sending Goods Move Short Email on " + sendto + Environment.NewLine + msg);
                            }
                        else
                            bl = false;
                    }
                    else
                    {
                        DataTable dt_param = new DataTable();
                        dt_param = null; msg = "";
                        string newmsg = "";
                        newmsg = emlog[0].Message;
                        dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                        if (dt_param == null)
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", msg);
                        }
                        sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                        bl = objemail.SendMail(sendto, newmsg, " Make money from Home Moves & Maintenance ", ref msg, "", "");
                    }
                    msg = "";
                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "IQ", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }
                    dsemail.EnforceConstraints = false;
                    dsemail.InquiryEmailLog.ImportRow(dsemaillog.Tables[0].Rows[0]);
                    dsemail.InquiryEmailLog[0].logid = logid;
                    dsemail.InquiryEmailLog[0].maildate = System.DateTime.UtcNow;
                    dsemail.InquiryEmailLog[0].Status = "Success";
                    //if (bl)
                    //    dsemail.InquiryEmailLog[0].Status = "Success";
                    //else
                    //{
                    //    dsemail.InquiryEmailLog[0].Status = "Error";
                    //    ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + msg);
                    //}
                    dsemail.InquiryEmailLog[0].AcceptChanges();
                    dsemail.InquiryEmailLog[0].SetAdded();
                    dsemail.EnforceConstraints = true;
                    if (dsemail != null)// && dsemail.Tables != null && dsemail.Tables[0] != null && dsemail.Tables[0].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsemail.InquiryEmailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

            DBCommand.Transaction.Commit();
            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            objBLReturnObject.ExecutionStatus = 1;
            ServerLog.SuccessLog("Mailer Email Saved: " + emlog[0].EmailID);
            return BLGeneralUtil.return_ajax_string("1", "Email Sent Successfully");
        }

        [HttpGet]
        public DataTable GetFlyerDetailReport(string fromdate, string todate, string UserType)
        {
            /// <summary>
            /// Nitu Bhavsar at 21-03-2016 
            /// Get flyer detail for report
            /// api/Mailer/GetFlyerDetailReport?fromdate=null&todate=null
            /// </summary>

            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            if (UserType == "Contact")
            {
                if (fromdate != "null" && todate != "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                            " from InquiryEmailLog" +
                            " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' and Source='Contact'" +
                            "order by maildate desc";
                }
                else if (fromdate != "null" && todate == "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and Source='Contact'" +
                           "order by maildate desc";
                }
                else if (fromdate == "null" && todate != "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' and Source='Contact'" +
                           "order by maildate desc";
                }
                else
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no " +
                            "from InquiryEmailLog" +
                            " where maildate>=dateadd(day,-30,getdate()) and maildate<=getdate() and Source='Contact'" +
                            "order by maildate desc";
                }
            }
            else if (UserType == "Mailer")
            {
                if (fromdate != "null" && todate != "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                            " from InquiryEmailLog" +
                            " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' and Source='Mailer'" +
                            "order by maildate desc";
                }
                else if (fromdate != "null" && todate == "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and Source='Mailer'" +
                           "order by maildate desc";
                }
                else if (fromdate == "null" && todate != "null")
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' and Source='Mailer'" +
                           "order by maildate desc";
                }
                else
                {
                    query1 = @"select CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no " +
                            "from InquiryEmailLog" +
                            " where maildate>=dateadd(day,-30,getdate()) and maildate<=getdate() and Source='Mailer'" +
                            "order by maildate desc";
                }
            }
            else if (UserType == "-1")
            {
                if (fromdate != "null" && todate != "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                            " from InquiryEmailLog" +
                            " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "'" +
                            " order by InquiryEmailLog.maildate desc ";
                    //"and Source in ('P','V','I','HLD') order by maildate desc";

                }
                else if (fromdate != "null" && todate == "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "'" +
                            " order by InquiryEmailLog.maildate desc ";
                    //" and Source in ('P','V','I','HLD') order by maildate desc";
                }
                else if (fromdate == "null" && todate != "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "'" +
                            " order by InquiryEmailLog.maildate desc ";
                    //" and Source in ('P','V','I','HLD') order by maildate desc";
                }
                else
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no " +
                            "from InquiryEmailLog" +
                            " where maildate>=dateadd(day,-30,getdate()) and maildate<=getdate() " +
                             " order by InquiryEmailLog.maildate desc ";
                }
            }
            else
            {
                if (fromdate != "null" && todate != "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                            " from InquiryEmailLog" +
                            " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "'" +
                            " and InquiryEmailLog.Source= '" + UserType + "' order by maildate desc";
                }
                else if (fromdate != "null" && todate == "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "'" +
                           " and InquiryEmailLog.Source= '" + UserType + "' order by maildate desc";
                }
                else if (fromdate == "null" && todate != "null")
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no" +
                           " from InquiryEmailLog" +
                           " where CAST(maildate AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "'" +
                           " and InquiryEmailLog.Source= '" + UserType + "' order by maildate desc";
                }
                else
                {
                    query1 = @"select  CONVERT(VARCHAR(10),maildate,126) as maildate,source,EmailID,Message,mobile_no " +
                            "from InquiryEmailLog" +
                            " where maildate>=dateadd(day,-30,getdate()) and maildate<=getdate() " +
                            " and InquiryEmailLog.Source= '" + UserType + "' order by maildate desc";
                }
            }
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;

        }

        [HttpGet]
        public string GenerateMail(string mailto, string title, string UserName, string message)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\Mastermail.html");
            string strdata = ""; string msg = "";
            string TITLE = title;


            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "logo.png");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "grillbg.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("IMAGE3", ConfigurationManager.AppSettings["Domain"] + "truck-bg.jpg");
                strdata = strdata.Replace("IMAGE4", ConfigurationManager.AppSettings["Domain"] + "icon.png");
                strdata = strdata.Replace("IMAGE5", ConfigurationManager.AppSettings["Domain"] + "vertical-line-lite1.png");
                strdata = strdata.Replace("IMAGE6", ConfigurationManager.AppSettings["Domain"] + "button.png");
                strdata = strdata.Replace("TEXT1", message);

            }
            sr.Close();
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            return BLGeneralUtil.return_ajax_string("1", "Registration Email Sent Successfully");
        }

        [HttpPost]
        public string SaveContactUsDetails([FromBody]JObject jobj)
        {
            DataSet dsemaillog = new DataSet();
            DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
            List<InquiryEmailLog> emlog = new List<InquiryEmailLog>();
            Master master = new Master(); Document objDocument = new Document();
            string logid = ""; string msg = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();

            if (jobj["Email"] != null)
            {
                emlog = jobj["Email"].ToObject<List<InquiryEmailLog>>();
                dsemaillog = master.CreateDataSet(emlog);
            }


            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                if (dsemaillog != null && dsemaillog.Tables.Count > 0 && dsemaillog.Tables[0].Rows.Count > 0)
                {
                    EMail objemail = new EMail();
                    msg = "";

                    BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                    DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref msg);
                    if (dt_param == null)
                    {
                        ServerLog.Log(msg);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; msg = "";

                    dt_param = null; msg = "";
                    string newmsg = "";
                    //newmsg = emlog[0].Message;
                    newmsg = " <br/> <b> Subject </b> : " + emlog[0].subject + "<br/> <b> Email </b>:" + emlog[0].EmailID + " <br/> <b> Name </b> : " + emlog[0].name + " <br/> <b> Mobile Number</b> : " + emlog[0].mobile_no + " <br/> <b> Message </b>:" + emlog[0].Message;
                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();

                    bl = objemail.SendMail(sendto, newmsg, "Contact us Request", ref msg, fromemail, frompword, "TruKKer Technologies");

                    msg = "";
                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "IQ", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    dsinqeml.EnforceConstraints = false;
                    dsinqeml.InquiryEmailLog.ImportRow(dsemaillog.Tables[0].Rows[0]);
                    dsinqeml.InquiryEmailLog[0].logid = logid;
                    dsinqeml.InquiryEmailLog[0].maildate = System.DateTime.UtcNow;
                    if (bl)
                        dsinqeml.InquiryEmailLog[0].Status = "Success";
                    else
                    {
                        dsinqeml.InquiryEmailLog[0].Status = "Error";
                        ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + msg);
                    }
                    dsinqeml.InquiryEmailLog[0].AcceptChanges();
                    dsinqeml.InquiryEmailLog[0].SetAdded();
                    dsinqeml.EnforceConstraints = true;
                    if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables[0] != null && dsinqeml.Tables[0].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsinqeml.InquiryEmailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
            DBCommand.Transaction.Commit();
            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            objBLReturnObject.ExecutionStatus = 1;
            ServerLog.SuccessLog("Inquiry/Mailer Email Saved: " + emlog[0].EmailID);
            return BLGeneralUtil.return_ajax_string("1", "Email Sent Successfully");
        }

        [HttpPost]
        public string SaveAddOnMailLogDetails([FromBody]JObject jobj)
        {
            DataSet dsemaillog = new DataSet();
            DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
            List<InquiryEmailLog> emlog = new List<InquiryEmailLog>();
            Master master = new Master(); Document objDocument = new Document();
            string logid = ""; string msg = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();

            if (jobj["Email"] != null)
            {
                emlog = jobj["Email"].ToObject<List<InquiryEmailLog>>();
                dsemaillog = master.CreateDataSet(emlog);
            }

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (dsemaillog != null && dsemaillog.Tables.Count > 0 && dsemaillog.Tables[0].Rows.Count > 0)
                {
                    EMail objemail = new EMail();
                    msg = "";

                    BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                    DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref msg);
                    if (dt_param == null)
                    {
                        ServerLog.Log(msg);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; msg = "";

                    dt_param = null; msg = "";
                    string newmsg = "";

                    if (emlog[0].load_inquiry_no != "" && emlog[0].load_inquiry_no != null)
                        newmsg = "<b> Services </b>: " + emlog[0].subject + " <br/> <b> UserName </b>: " + emlog[0].name + " <br/><b> Email </b>:" + emlog[0].EmailID + " <br/> <b>Mobile Number</b> : " + emlog[0].mobile_no + " <br/> <b>Order ID </b>:" + emlog[0].load_inquiry_no;
                    else
                        newmsg = "<b> Services </b>: " + emlog[0].subject + " <br/> <b> UserName </b>: " + emlog[0].name + " <br/><b> Email </b>:" + emlog[0].EmailID + " <br/> <b>Mobile Number</b> : " + emlog[0].mobile_no;
                    //"Name of Service/s, Order No. (If available), Userrname, Email, Mobile Number"
                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    bl = objemail.SendMail(sendto, newmsg, " Request for add on service " + emlog[0].subject + "", ref msg, fromemail, frompword, "TruKKer Technologies");

                    msg = "";



                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "AD", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    dsinqeml.EnforceConstraints = false;
                    dsinqeml.AddOnMailLog.ImportRow(dsemaillog.Tables[0].Rows[0]);
                    dsinqeml.AddOnMailLog[0].logid = logid;
                    dsinqeml.AddOnMailLog[0].Message = newmsg;
                    dsinqeml.AddOnMailLog[0].maildate = System.DateTime.UtcNow;
                    if (bl)
                        dsinqeml.AddOnMailLog[0].Status = "Success";
                    else
                    {
                        dsinqeml.AddOnMailLog[0].Status = "Error";
                        ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + msg);
                    }
                    dsinqeml.AddOnMailLog[0].AcceptChanges();
                    dsinqeml.AddOnMailLog[0].SetAdded();
                    dsinqeml.EnforceConstraints = true;
                    if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables["AddOnMailLog"] != null && dsinqeml.Tables["AddOnMailLog"].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsinqeml.AddOnMailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }

                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 1;
                ServerLog.SuccessLog("Inquiry/Mailer Email Saved: " + emlog[0].EmailID);
                return BLGeneralUtil.return_ajax_string("1", "Request Received");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        public DataTable GetUserdetails(string inquiryno)
        {
            try
            {
                DataTable dt_user = new DataTable();
                string qury = "select * from orders join user_mst on user_mst.unique_id = orders.shipper_id where orders.load_inquiry_no= '" + inquiryno + "'";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = qury;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_user = ds.Tables[0];
                }
                if (dt_user != null && dt_user.Rows.Count > 0)
                    return dt_user;
                else
                    return null;
            }
            catch
            {
                if (DBCommand.Connection.State == ConnectionState.Open) DBConnection.Close();
                return null;
            }
        }

        [HttpGet]
        public string SaveAddOnMailLogDetailsBymail(string loadinq)
        {
            // DataSet dsemaillog = new DataSet();
            DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
            //List<InquiryEmailLog> emlog = new List<InquiryEmailLog>();
            Master master = new Master(); Document objDocument = new Document();
            string logid = ""; string msg = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();

            string lnq = "";
            if (loadinq != "" && loadinq.ToString() != string.Empty)
            {
                lnq = BLGeneralUtil.Decrypt(loadinq);
            }

            DataTable dtdetails = GetUserdetails(lnq);
            if (dtdetails != null)
            {
                try
                {


                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    //if (dsemaillog != null && dsemaillog.Tables.Count > 0 && dsemaillog.Tables[0].Rows.Count > 0)
                    //{
                    EMail objemail = new EMail();
                    msg = "";

                    BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                    DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref msg);
                    if (dt_param == null)
                    {
                        ServerLog.Log(msg);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; msg = "";

                    dt_param = null; msg = "";
                    string newmsg = "";

                    newmsg = "<b> Services </b>: Requeste for addon services <br/> <b> UserName </b>: " + dtdetails.Rows[0]["first_name"].ToString() + " <br/><b> Email </b>:" + dtdetails.Rows[0]["email_id"].ToString() + " <br/> <b>Mobile Number</b> : " + dtdetails.Rows[0]["user_id"].ToString() + " <br/> <b>Order ID </b>:" + lnq;

                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    bl = objemail.SendMail(sendto, newmsg, " Request for add on service ", ref msg, fromemail, frompword, "TruKKer Technologies");

                    msg = "";



                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "AD", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    dsinqeml.EnforceConstraints = false;

                    DS_InquiryEmailLog.AddOnMailLogRow tr = dsinqeml.AddOnMailLog.NewAddOnMailLogRow();
                    tr.shipper_id = dtdetails.Rows[0]["first_name"].ToString();
                    tr.Source = "ADDON";
                    tr.EmailID = dtdetails.Rows[0]["email_id"].ToString();
                    tr.subject = " Requeste for addon services from mail ";
                    tr.logid = logid;
                    tr.Message = newmsg;
                    tr.maildate = System.DateTime.UtcNow;
                    if (bl)
                        tr.Status = "Success";
                    else
                    {
                        tr.Status = "Error";
                        ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + msg);
                    }
                    dsinqeml.AddOnMailLog.AddAddOnMailLogRow(tr);
                    dsinqeml.EnforceConstraints = true;

                    if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables["AddOnMailLog"] != null && dsinqeml.Tables["AddOnMailLog"].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsinqeml.AddOnMailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Inquiry/Mailer Email Saved: " + dtdetails.Rows[0]["email_id"].ToString());
                    return BLGeneralUtil.return_ajax_string("1", "Thank you.....Request Received");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Not send");
        }


        public DataTable GetRequestQuotationbyID(string inquiryno)
        {
            try
            {
                DataTable dt_user = new DataTable();
                string qury = "select * from RequestQuotationMailLog where load_inquiry_no= @loadinquiryno";
                if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinquiryno", DbType.String, inquiryno));
                DBDataAdpterObject.SelectCommand.CommandText = qury;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_user = ds.Tables[0];
                }
                DBConnection.Close();
                if (dt_user != null && dt_user.Rows.Count > 0)
                    return dt_user;
                else
                    return null;
            }
            catch
            {
                if (DBCommand.Connection.State == ConnectionState.Open) DBConnection.Close();
                return null;
            }
        }

        [HttpPost]
        public string SaveQuotationRequestMail([FromBody]JObject jobj)
        {
            DataSet dsemaillog = new DataSet();
            DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
            List<InquiryEmailLog> emlog = new List<InquiryEmailLog>();
            Master master = new Master(); Document objDocument = new Document();
            string logid = ""; string msg = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DataTable dtquot = new DataTable();

            if (jobj["Email"] != null)
            {
                emlog = jobj["Email"].ToObject<List<InquiryEmailLog>>();
                dsemaillog = master.CreateDataSet(emlog);
            }

            try
            {

                if (emlog[0].load_inquiry_no != null && emlog[0].load_inquiry_no != "")
                    dtquot = GetRequestQuotationbyID(emlog[0].load_inquiry_no);
                else
                    dtquot = null;

                if (dtquot == null)
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    if (dsemaillog != null && dsemaillog.Tables.Count > 0 && dsemaillog.Tables[0].Rows.Count > 0)
                    {
                        EMail objemail = new EMail();
                        msg = "";

                        BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                        DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref msg);
                        if (dt_param == null)
                        {
                            ServerLog.Log(msg);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", msg);
                        }

                        string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                        string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                        string sendto = ""; msg = "";

                        dt_param = null; msg = "";
                        StringBuilder newmsg = new StringBuilder();
                        newmsg.Clear(); string strdatatable = "";


                        TruckerMaster objTruckerMaster = new TruckerMaster();
                        DataTable dtSizeTypeMst = new DataTable(); string Message = "";
                        DataTable dtHireTruckSizeTypeMst = new DataTable();
                        string SizeTypeCode = ""; DateTime OrderShippingDatetime = Convert.ToDateTime(emlog[0].ShippingDatetime); string goods_type_flag = ""; decimal TotalDistance = 0; string TotalDistanceUOM = ""; decimal TimeToTravelInMinute = 0; string IncludePackingCharge = ""; int? NoOfTruck = null; int? NoOfDriver = null; int? NoOfLabour = null; int? NoOfHandiman = null; int? NoOfSupervisor = null;
                        string order_type_flag = ""; DateTime OrderTodate = new DateTime();

                        if (emlog[0].load_inquiry_no != null || emlog[0].load_inquiry_no != "")
                        {
                            DataTable dt = new PostOrderController().GetLoadInquiryBySizetypeId(emlog[0].load_inquiry_no);
                            SizeTypeCode = dt.Rows[0]["SizeTypeCode"].ToString();
                            goods_type_flag = dt.Rows[0]["goods_type_flag"].ToString();
                            TotalDistance = Convert.ToDecimal(emlog[0].TotalDistance);
                            TotalDistanceUOM = emlog[0].TotalDistanceUOM;
                            TimeSpan Tsshippingtime = TimeSpan.Parse(dt.Rows[0]["TimeToTravelInMinute"].ToString());
                            TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);
                            IncludePackingCharge = "Y";
                            order_type_flag = dt.Rows[0]["order_type_flag"].ToString();
                            if (dt.Rows[0]["Hiretruck_To_datetime"].ToString() != "")
                                OrderTodate = Convert.ToDateTime(dt.Rows[0]["Hiretruck_To_datetime"].ToString());

                            if (order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                            {
                                dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtquot, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                            }
                            else if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                            {
                                dtHireTruckSizeTypeMst = objTruckerMaster.CalculateRateHireTruck(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                                if (dtHireTruckSizeTypeMst == null || dtHireTruckSizeTypeMst.Rows.Count <= 0)
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                            }
                            else
                            {
                                dtSizeTypeMst = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "P", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                    return BLGeneralUtil.return_ajax_string("0", Message);

                                DataTable dtSizeTypeMstBudget = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                                if (dtSizeTypeMstBudget == null || dtSizeTypeMstBudget.Rows.Count <= 0)
                                    return BLGeneralUtil.return_ajax_string("0", Message);

                                DataTable dtSizeTypeMstSuperSaver = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "S", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                                if (dtSizeTypeMstSuperSaver == null || dtSizeTypeMstSuperSaver.Rows.Count <= 0)
                                    return BLGeneralUtil.return_ajax_string("0", Message);

                                DataRow dr_temp_sizetypeBudget = dtSizeTypeMst.NewRow();
                                dtSizeTypeMstBudget.Rows[0]["rate_type_flag"] = "Standard";
                                dr_temp_sizetypeBudget.ItemArray = dtSizeTypeMstBudget.Rows[0].ItemArray;
                                dtSizeTypeMst.Rows.Add(dr_temp_sizetypeBudget);

                                DataRow dr_temp_sizetypeSuperSaver = dtSizeTypeMst.NewRow();
                                dtSizeTypeMstSuperSaver.Rows[0]["rate_type_flag"] = "Super Saver";
                                dr_temp_sizetypeSuperSaver.ItemArray = dtSizeTypeMstSuperSaver.Rows[0].ItemArray;
                                dtSizeTypeMst.Rows.Add(dr_temp_sizetypeSuperSaver);

                                dtSizeTypeMst.Rows[0]["rate_type_flag"] = "Premium";
                            }

                            if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                            {
                                string[] selectedColumns = new[] { "NoOfTruck", "NoOfDriver", "NoOfLabour", "NoOfHandiman", "HireTruck_MinRate", "HireTruck_FuelRatePerDay", "HireTruck_AdditionalKMCharges", "Total_cost" };

                                DataTable dtprice = new DataView(dtHireTruckSizeTypeMst).ToTable(false, selectedColumns);
                                strdatatable = ExportDatatableToHtml(dtprice);
                            }
                            else
                            {
                                string[] selectedColumns = new[] { "rate_type_flag", "NoOfTruck", "NoOfDriver", "NoOfLabour", "NoOfHandiman", "NoOfSupervisor", "TotalPackingRate", "Total_cost" };

                                DataTable dtprice = new DataView(dtSizeTypeMst).ToTable(false, selectedColumns);
                                strdatatable = ExportDatatableToHtml(dtprice);

                            }

                        }

                        if (emlog[0].load_inquiry_no == null || emlog[0].load_inquiry_no == "")
                            newmsg.Append(" Hello, <br/> <b> Subject </b>: " + emlog[0].subject + " by <b>" + emlog[0].name + " <b> <br/><b> Email </b>:" + emlog[0].EmailID + " <br/> <b> Mobile Number </b> : " + emlog[0].mobile_no + " <br/> <b> Shipping DateTime </b> : " + emlog[0].ShippingDatetime + "");
                        else
                        {
                            if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                            {
                                newmsg.Append("<b> Subject </b>: Quotation Request <br/> <b> Truck Type </b>: " + emlog[0].Sizetypecode + " <br/>  <b> Start Point </b>: " + emlog[0].Sourceaddress + " <br/> <b> UserName </b>: " + emlog[0].name + " <br/><b> Email </b>:" + emlog[0].EmailID + " <br/> <b> Mobile Number </b> : " + emlog[0].mobile_no + " <br/> <b> Start  Date </b> : " + emlog[0].ShippingDatetime + "<br/> <b> To Date </b> : " + OrderTodate.ToString("dd MMM yyyy") + "<br/> <b> Order ID  </b> : " + emlog[0].load_inquiry_no + "");
                            }
                            else
                            {
                                string txttype = order_type_flag == Constant.ORDERTYPECODEFORHOME ? " Size  " : " Truck ";
                                newmsg.Append("<b> Subject </b>: Quotation Request <br/> <b> " + txttype + " Type </b>: " + emlog[0].Sizetypecode + " <br/>  <b> Source Address </b>: " + emlog[0].Sourceaddress + " <br/>  <b> Destination Address </b>: " + emlog[0].Destinationaddress + " <br/>  <b> Total Distance </b>: " + TotalDistance + " " + TotalDistanceUOM + " <br/> <b> UserName </b>: " + emlog[0].name + " <br/><b> Email </b>:" + emlog[0].EmailID + " <br/> <b> Mobile Number </b> : " + emlog[0].mobile_no + " <br/> <b> Shipping DateTime </b> : " + emlog[0].ShippingDatetime + "<br/> <b> Order ID  </b> : " + emlog[0].load_inquiry_no + "");
                            }
                        }
                        newmsg.Append("<br/> <br/> <b> Quotation details  </b>: <br/>");
                        newmsg.Append(strdatatable);

                        dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                        if (dt_param == null)
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", msg);
                        }

                        sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                        if (emlog[0].load_inquiry_no == null || emlog[0].load_inquiry_no == "")
                            bl = objemail.SendMail(sendto, newmsg.ToString(), " Request for Moving Home from  " + emlog[0].name + "", ref msg, fromemail, frompword, "TruKKer Technologies");
                        else
                        {
                            string ordertype = "";
                            if (order_type_flag == Constant.ORDERTYPECODEFORHOME)
                                ordertype = " Moving Home ";
                            else if (order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                                ordertype = " Moving Goods ";
                            else if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                                ordertype = " Hire Truck ";

                            bl = objemail.SendMail(sendto, newmsg.ToString(), " Request Quotation for " + ordertype + " (Order ID: " + emlog[0].load_inquiry_no + ") by " + emlog[0].name, ref msg, fromemail, frompword, "TruKKer Technologies");
                        }
                        msg = "";

                        if (!objDocument.W_GetNextDocumentNoNew(ref DBCommand, "RQM", "", "", ref logid, ref msg))
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", msg);
                        }

                        dsinqeml.EnforceConstraints = false;
                        dsinqeml.RequestQuotationMailLog.ImportRow(dsemaillog.Tables[0].Rows[0]);
                        dsinqeml.RequestQuotationMailLog[0].logid = logid;
                        dsinqeml.RequestQuotationMailLog[0].Message = newmsg.ToString();
                        dsinqeml.RequestQuotationMailLog[0].maildate = System.DateTime.UtcNow;
                        if (bl)
                            dsinqeml.RequestQuotationMailLog[0].Status = "Success";
                        else
                        {
                            dsinqeml.RequestQuotationMailLog[0].Status = "Error";
                            ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + msg);
                        }
                        dsinqeml.RequestQuotationMailLog[0].Sizetypecode = emlog[0].Sizetypecode;
                        dsinqeml.RequestQuotationMailLog[0].Sourceaddress = emlog[0].Sourceaddress;
                        dsinqeml.RequestQuotationMailLog[0].Destinationaddress = emlog[0].Destinationaddress;
                        dsinqeml.RequestQuotationMailLog[0].shippingdatetime = emlog[0].ShippingDatetime;
                        dsinqeml.RequestQuotationMailLog[0].load_inquiry_no = emlog[0].load_inquiry_no;

                        dsinqeml.RequestQuotationMailLog[0].AcceptChanges();
                        dsinqeml.RequestQuotationMailLog[0].SetAdded();
                        dsinqeml.EnforceConstraints = true;
                        if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables["RequestQuotationMailLog"] != null && dsinqeml.Tables["RequestQuotationMailLog"].Rows.Count > 0)
                        {
                            objBLReturnObject = master.UpdateTables(dsinqeml.RequestQuotationMailLog, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }
                        }

                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Inquiry/Mailer Email Saved: " + emlog[0].EmailID);
                    return BLGeneralUtil.return_ajax_string("1", "Request Received");
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", "Already send ");
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        protected string ExportDatatableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags.  
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;

        }

        [HttpGet]
        public bool GenerateInvoiceMail(string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            string PDfFilepath = ConfigurationManager.AppSettings["InvoicePdfPath"];
            StreamReader sr; string LINK = "";

            string strdata = ""; string msg = "";
            string TITLE = "Invoice details"; bool bl = true;
            string[] strBillingAdd = new string[] { };

            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            //DataTable dtDrivertruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(OrderID);
            string addonloadinquiry = BLGeneralUtil.Encrypt(OrderID);

            if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
            {
                if (dtorder.Rows[0]["order_by"].ToString().Trim() == Constant.USERTYPE)
                    sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\DubiOrderInvoiceDetailMailHome.html");
                else
                    sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderInvoiceDetailMailHome.html");
            }
            else if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderInvoiceDetailMailHireTrucks.html");
            else
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderInvoiceDetailMailGoods.html");

            string shippername = new PostOrderController().GetUserdetailsByID(dtorder.Rows[0]["shipper_id"].ToString());
            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("DOMAIN", domian);
                strdata = strdata.Replace("ADDONLINK", addonloadinquiry);
                double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                strdata = strdata.Replace("DRIVERLOGO", ConfigurationManager.AppSettings["Domain"] + "MailerImage/driver.png");
                strdata = strdata.Replace("TRUCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/truck.png");
                strdata = strdata.Replace("ClOCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/clock.png");
                strdata = strdata.Replace("USERNAME", dtorder.Rows[0]["billing_name"].ToString());
                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
                strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy h:mm tt"));
                strdata = strdata.Replace("SHIPPINGTIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("h:mm tt"));
                strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
                strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
                strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["cbmlink"].ToString());
                strdata = strdata.Replace("TOTALDISTANCE", dtorder.Rows[0]["TotalDistance"].ToString());
                strdata = strdata.Replace("DISTANCEUOM", dtorder.Rows[0]["TotalDistanceUOM"].ToString());

                strdata = strdata.Replace("INVOICEDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy"));
                //strdata = strdata.Replace("INVOICEDATE", new PostOrderController().DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow).ToString("dd MMM yyyy"));
                strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["BaseRate"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());

                if (dtorder.Rows[0]["rate_type_flag"].ToString() == "P")
                {
                    strdata = strdata.Replace("BASERATE", "AED  " + totalprice.ToString());// dtorder.Rows[0]["TotalTravelingRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["BaseRate"].ToString() : "");
                    strdata = strdata.Replace("TOTALTRAVELINGRATE", dtorder.Rows[0]["TotalTravelingRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalTravelingRate"].ToString() : "");
                    strdata = strdata.Replace("TOTALDRIVERRATE", dtorder.Rows[0]["TotalDriverRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalDriverRate"].ToString() : "");
                    strdata = strdata.Replace("TOTALLABOURRATE", dtorder.Rows[0]["TotalLabourRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalLabourRate"].ToString() : "");
                    strdata = strdata.Replace("TOTALHANDIMANRATE", dtorder.Rows[0]["TotalHandimanRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalHandimanRate"].ToString() : "");
                }
                else
                {
                    strdata = strdata.Replace("BASERATE", "AED  " + dtorder.Rows[0]["BaseRate"].ToString());
                    strdata = strdata.Replace("TOTALTRAVELINGRATE", "AED  " + dtorder.Rows[0]["TotalTravelingRate"].ToString());
                    strdata = strdata.Replace("TOTALDRIVERRATE", "AED  " + dtorder.Rows[0]["TotalDriverRate"].ToString());
                    strdata = strdata.Replace("TOTALLABOURRATE", "AED  " + dtorder.Rows[0]["TotalLabourRate"].ToString());
                    strdata = strdata.Replace("TOTALHANDIMANRATE", "AED  " + dtorder.Rows[0]["TotalHandimanRate"].ToString());
                }
                strBillingAdd = dtorder.Rows[0]["billing_add"].ToString().Split('^');
                if (strBillingAdd.Length > 0)
                {
                    if (strBillingAdd.Length > 1)
                    {
                        //strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0] + "," + strBillingAdd[1]);
                        strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0] + "," + strBillingAdd[1]);
                        if (strBillingAdd.Length == 3)
                            strdata = strdata.Replace("BILLINGADD2", strBillingAdd[2]);
                        else
                            strdata = strdata.Replace("BILLINGADD2", "");
                    }
                    else
                    {
                        strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0]);
                        strdata = strdata.Replace("BILLINGADD2", "");
                    }

                    ////strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0] + "," + strBillingAdd[1]);
                    //strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0] + "," + strBillingAdd[1]);
                    //if (strBillingAdd.Length == 3)
                    //    strdata = strdata.Replace("BILLINGADD2", strBillingAdd[2]);
                    //else
                    //    strdata = strdata.Replace("BILLINGADD2", "");
                }

                strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalSupervisorRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalSupervisorRate"].ToString() : "");
                strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalDriverRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalSupervisorRate"].ToString() : "");



                if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                {
                    int noofdays = dtorder.Rows[0]["Hiretruck_NoofDay"].ToString() != "" ? Convert.ToInt32(dtorder.Rows[0]["Hiretruck_NoofDay"].ToString()) : 1;

                    strdata = strdata.Replace("NOOFDAYS", noofdays.ToString());
                    strdata = strdata.Replace("MOVINGTYPE", "Hire Truck");

                    if (dtorder.Rows[0]["Hiretruck_IncludingFuel"].ToString() == Constant.Flag_Yes)
                        strdata = strdata.Replace("FUELCHARGES", "<tr><td style=\"text-align: left\">Fuel Charge</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["Hiretruck_TotalFuelRate"].ToString())).ToString() + "</td></tr>");
                    else
                        strdata = strdata.Replace("FUELCHARGES", "");

                    if (noofdays > 1)
                        strdata = strdata.Replace("TODATE", "<b> To </b> <span style=\"font-size: 15px\"><span style=\"color: #a9a9a9\">" + Convert.ToDateTime(dtorder.Rows[0]["Hiretruck_To_datetime"].ToString()).ToString("dd MMM yyyy") + "</span></span>");
                    else
                        strdata = strdata.Replace("TODATE", "");


                }

                else if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
                    strdata = strdata.Replace("MOVINGTYPE", "Moving Home");
                else
                    strdata = strdata.Replace("MOVINGTYPE", "Moving Goods");


                if (dtorder.Rows[0]["rate_type_flag"].ToString() != "P")
                {
                    if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                        strdata = strdata.Replace("PACKINGCHARGE", "AED  " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["TotalPackingCharge"].ToString())).ToString());
                    else
                        strdata = strdata.Replace("PACKINGCHARGE", "AED 00");
                }
                else
                {
                    strdata = strdata.Replace("PACKINGCHARGE", "");
                }



                if (dtorder != null)
                {
                    // string str = "";
                    decimal Discount = dtorder.Rows[0]["Discount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Discount"].ToString()) : 0;
                    if (Discount != 0)
                        strdata = strdata.Replace("DISCOUNT", Discount.ToString());

                }


                if (dtorder != null)
                {
                    string str = "";
                    decimal Remaningamt = Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString());
                    decimal Totalcostwithoutdiscount = dtorder.Rows[0]["Total_cost_without_discount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Total_cost_without_discount"].ToString()) : 0;
                    decimal Recievedamt = Totalcostwithoutdiscount - Remaningamt;
                    if (Remaningamt == 0)
                    {
                        if (dtorder.Rows[0]["payment_mode"].ToString() == "O")
                        {
                            str += "<td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\">Online Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td>";
                        }
                        else
                        {
                            str += "<td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td>";
                        }
                        strdata = strdata.Replace("PAYMENTDETAIL", str);
                    }
                    else
                    {
                        str += "<tr><td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\"></td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td><tr>";
                        str += "<tr><td style=\"text-align:left\">Paid amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\"> - AED " + Math.Round(Recievedamt).ToString() + "</td><tr>";
                        str += "<tr><td style=\"text-align:left\">Remaining Amount</td><td style=\"text-align: left\"></td><td><strong></strong></td><td style=\"text-align: right;border-top:1px solid #AAAAAA;\">AED " + Math.Round(Remaningamt).ToString() + "</td><tr>";

                        strdata = strdata.Replace("PAYMENTDETAIL", str);
                    }

                }

                //if (dtDrivertruckdetails != null)
                //{
                //    string str = "";
                //    for (int i = 0; i < dtDrivertruckdetails.Rows.Count; i++)
                //    {
                //        str += " <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width:100%;border-collapse:collapse\"><tbody><tr><td valign=\"top\"> " +
                //               " <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width:100%;border-collapse:collapse\"><tbody><tr> " +
                //               " <td style=\"padding-top:9px;padding-left:18px;padding-bottom:9px;padding-right:18px\"><table border=\"0\" cellpadding=\"18\" cellspacing=\"0\" width=\"100%\" style=\"min-width:100%!important;background-color:#eeeeee;border-collapse:collapse\">  " +
                //               " <tbody><tr><td valign=\"top\" style=\"color:#f2f2f2;font-family:Helvetica;font-size:14px;font-weight:normal;line-height:100%;text-align:left;word-break:break-word\">  " +
                //               " <table style=\"width:100%;font-size:16px;border-collapse:collapse\"><tbody><tr><td style=\"text-align:left\"><span style=\"font-size:13px\"><strong><span style=\"color:#000000\"><img align=\"none\" height=\"20\" src=\"DRIVERLOGO\" style=\"width:20px;min-height:20px;margin:0px 10px 0px 0px;border:0;outline:none;text-decoration:none\" valign=\"middle\" width=\"20\" class=\"CToWUd\">Driver &nbsp;: &nbsp;</span></strong><span style=\"color:#000000\">(" + dtDrivertruckdetails.Rows[i]["Name"].ToString() + ")</span></span></td>   " +
                //               " <td style=\"text-align:left\"><span style=\"font-size:13px\"><strong><span style=\"color:#000000\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<img align=\"none\" height=\"20\" src=\"TRUCKIMAGE\" style=\"width:20px;min-height:20px;margin:0px 10px 0px 0px;border:0;outline:none;text-decoration:none\" valign=\"middle\" width=\"20\" class=\"CToWUd\">&nbsp;Truck No &nbsp;: &nbsp;</span></strong><span style=\"color:#000000\"> " + dtDrivertruckdetails.Rows[i]["vehicle_reg_no"].ToString() + "</span></span></td> " +
                //               " </tr><tr><td style=\"text-align:left\"><span style=\"font-size:13px\"><strong><span style=\"color:#000000\"><img align=\"none\" height=\"20\" src=\"ClOCKIMAGE\" style=\"width:20px;min-height:20px;margin:0px;border:0;outline:none;text-decoration:none\" valign=\"middle\" width=\"20\" class=\"CToWUd\">&nbsp; Start Time &nbsp;: &nbsp;</span></strong><span style=\"color:#000000\"><span class=\"aBn\" data-term=\"goog_307291734\" tabindex=\"0\"><span class=\"aQJ\"> " + Convert.ToDateTime(dtDrivertruckdetails.Rows[0]["start_time"].ToString()).ToString("h:mm tt") + "</span></span></span></span></td> " +
                //               " <td style=\"text-align:left\"><span style=\"font-size:13px\"><strong><span style=\"color:#000000\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<img align=\"none\" height=\"20\" src=\"ClOCKIMAGE\" style=\"width:20px;min-height:20px;margin:0px;border:0;outline:none;text-decoration:none\" valign=\"middle\" width=\"20\" class=\"CToWUd\">&nbsp; &nbsp;End Time &nbsp;: &nbsp;</span></strong><span style=\"color:#000000\"> " + Convert.ToDateTime(dtDrivertruckdetails.Rows[0]["complete_time"].ToString()).ToString("h:mm tt") + "</span></span></td> " +
                //               " </tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table> ";
                //    }
                //    strdata = strdata.Replace("DRIVERDETAILS", str);
                //}

            }
            sr.Close();

            try
            {
                var htmlContent = String.Format(sr.ToString(), DateTime.UtcNow);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                //var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                var pdfBytes = htmlToPdf.GeneratePdf(strdata);

                FileStream fs = new FileStream(Path.Combine(PDfFilepath, "Invoicedetail_" + OrderID + ".pdf"), FileMode.Create);
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Dispose();
                fs.Close();


                return true;
            }
            catch (Exception Ex)
            {
                ServerLog.Log("Invoice PDF" + Ex.Message);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }

            //htmlToPdf.GeneratePdfFromFile("http://www.nrecosite.com/", null, "export.pdf");

            //msg = "";
            //EMail objemail = new EMail();
            //Boolean bl = objemail.SendMail("sandippr12@gmail.com", strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            //if (!bl)
            //{
            //    return BLGeneralUtil.return_ajax_string("0", msg);
            //}

            //return bl;
        }

        public Boolean OrderConfirmationMailToAdmin(DataTable dtorder)
        {
            string order_type_flag = "";
            string LoadInquiryno = "";
            string SizetypeCode = "";
            string ratetypeflag = "";
            string shippername = "";
            string UserMobileNo = "";
            DataTable dtaddonservices = null;

            if (dtorder != null)
            {
                order_type_flag = dtorder.Rows[0]["order_type_flag"].ToString();
                LoadInquiryno = dtorder.Rows[0]["load_inquiry_no"].ToString();
                ratetypeflag = dtorder.Rows[0]["rate_type_flag"].ToString();
                SizetypeCode = new PostOrderController().GetSizetypeDetails(dtorder.Rows[0]["SizeTypeCode"].ToString());
                shippername = new PostOrderController().GetUserdetailsByID(dtorder.Rows[0]["shipper_id"].ToString());
                UserMobileNo = new PostOrderController().GetMobileNoByID(dtorder.Rows[0]["shipper_id"].ToString());
                dtaddonservices = new PostOrderController().GetorderAddonServiceDetailsByid(LoadInquiryno);

            }

            try
            {


                EMail objemail = new EMail();
                string msg = "";

                BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl = true;
                DataTable dt_paramFrom = blgn.GetParameter("CONTACT", "SENDFROM", ref msg);
                if (dt_paramFrom == null)
                {
                    ServerLog.Log(msg);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return false;
                }

                string fromemail = dt_paramFrom.Rows[0]["param_value"].ToString().Trim();
                string frompword = dt_paramFrom.Rows[0]["Remark"].ToString().Trim();
                string sendto = ""; msg = "";

                msg = "";
                StringBuilder newmsg = new StringBuilder();

                newmsg.Clear();

                ParameterMst objParameterMstTruckRate = new ParameterMst();
                DataTable dt_paramTo = objParameterMstTruckRate.GetParameter("ORDERCONFIRM", null, ref msg);

                if (ratetypeflag == Constant.RATE_TYPE_FLAG_STANDERD)
                    ratetypeflag = " STANDERD ";
                else if (order_type_flag == Constant.RATE_TYPE_FLAG_PREMIUM)
                    ratetypeflag = " PREMIUM ";
                else if (order_type_flag == Constant.RATE_TYPE_FLAG_SUPERSAVER)
                    ratetypeflag = " SUPERSAVER ";

                string ordertype = "";
                if (order_type_flag == Constant.ORDERTYPECODEFORHOME)
                    ordertype = " Moving Home ";
                else if (order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    ordertype = " Moving Goods ";
                else if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                    ordertype = " Hire Truck ";

                if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                {
                    newmsg.Append("<b> Move Type </b>: " + ordertype + " <br/> <b> Truck Type </b>: " + SizetypeCode + "<br/>  <b> Installers/Helpers: </b>: " + dtorder.Rows[0]["NoOfHandiman"].ToString() + "/" + dtorder.Rows[0]["NoOfLabour"].ToString() + " <br/> <b> Source Address </b>: " + dtorder.Rows[0]["inquiry_source_addr"].ToString() + " <br/> <b> Destination Address </b>: " + dtorder.Rows[0]["inquiry_destination_addr"].ToString() + " <br/>  <b> Total Distance </b>: " + dtorder.Rows[0]["TotalDistance"].ToString() + " " + dtorder.Rows[0]["TotalDistanceUOM"].ToString() + " <br/> <b> UserName </b>: " + shippername + " <br/><b> Email </b>:" + new PostOrderController().GetEmailByID(dtorder.Rows[0]["shipper_id"].ToString()) + " <br/> <b> Mobile Number </b> : " + UserMobileNo + " <br/> <b> Shipping DateTime </b> : " + Convert.ToDateTime(dtorder.Rows[0]["ShippingDatetime"].ToString()).ToString("dd-MM-yyyy HH:mm:ss tt") + "<br/> <b> Order ID  </b> : " + dtorder.Rows[0]["load_inquiry_no"].ToString() + "");
                }
                else
                {
                    string txttype = order_type_flag == Constant.ORDERTYPECODEFORHOME ? " Size  " : " Truck ";
                    newmsg.Append(" <b> Move Type </b>: " + ordertype + " <br/> <b> " + txttype + " Type </b>: " + SizetypeCode + " <br/>  <b> Package </b>: " + ratetypeflag + " <br/> ");
                    newmsg.Append(" <b> Installers/Helpers: </b>: " + dtorder.Rows[0]["NoOfHandiman"].ToString() + "/" + dtorder.Rows[0]["NoOfLabour"].ToString() + " <br/> ");
                    newmsg.Append(" <b> Source Address </b>: " + dtorder.Rows[0]["inquiry_source_addr"].ToString() + " <br/> <b> Destination Address </b>: " + dtorder.Rows[0]["inquiry_destination_addr"].ToString() + " <br/> ");
                    newmsg.Append(" <b> Total Distance </b>: " + dtorder.Rows[0]["TotalDistance"].ToString() + " " + dtorder.Rows[0]["TotalDistanceUOM"].ToString() + " <br/> <b> UserName </b>: " + shippername + " <br/> ");
                    newmsg.Append(" <b> Email </b>:" + new PostOrderController().GetEmailByID(dtorder.Rows[0]["shipper_id"].ToString()) + " <br/> <b> Mobile Number </b> : " + UserMobileNo + " <br/> ");
                    newmsg.Append(" <b> Shipping DateTime </b> : " + Convert.ToDateTime(dtorder.Rows[0]["ShippingDatetime"].ToString()).ToString("dd-MM-yyyy HH:mm:ss tt") + "<br/> ");
                    newmsg.Append(" <b> Order ID  </b> : " + dtorder.Rows[0]["load_inquiry_no"].ToString() + "");


                    string stritem = "";
                    if (dtorder.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                    {
                        if (dtaddonservices != null)
                        {
                            for (int i = 0; i < dtaddonservices.Rows.Count; i++)
                            {
                                if (i == 0)
                                    stritem = "'" + dtaddonservices.Rows[i]["ServiceTypeCode"].ToString() + "'";
                                else
                                {
                                    stritem += ",";
                                    stritem += "'" + dtaddonservices.Rows[i]["ServiceTypeCode"].ToString() + "'";
                                }
                            }
                        }
                    }
                    else
                        stritem = "";

                    if (dtorder.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                    {
                        stritem = stritem.Replace("'PT'", "Painting");
                        stritem = stritem.Replace("'CL'", "Cleaning");
                        stritem = stritem.Replace("'PEST'", "Pest Control");
                        newmsg.Append("<br/> <b> Addon Service  </b> : " + stritem + "");
                    }

                }

                //else
                //{
                //    string txttype = order_type_flag == Constant.ORDERTYPECODEFORHOME ? " Size  " : " Truck ";
                //    newmsg.Append("<b> Move Type </b>: " + ordertype + " <br/> <b> " + txttype + " Type </b>: " + SizetypeCode + " <br/>  <b> Package </b>: " + ratetypeflag + " <br/>  <b> Installers/Helpers: </b>: " + dtorder.Rows[0]["NoOfHandiman"].ToString() + "/" + dtorder.Rows[0]["NoOfLabour"].ToString() + " <br/>  <b> Source Address </b>: " + dtorder.Rows[0]["inquiry_source_addr"].ToString() + " <br/> <b> Destination Address </b>: " + dtorder.Rows[0]["inquiry_destination_addr"].ToString() + " <br/>  <b> Total Distance </b>: " + dtorder.Rows[0]["TotalDistance"].ToString() + " " + dtorder.Rows[0]["TotalDistanceUOM"].ToString() + " <br/> <b> UserName </b>: " + shippername + " <br/><b> Email </b>:" + new PostOrderController().GetEmailByID(dtorder.Rows[0]["shipper_id"].ToString()) + " <br/> <b> Mobile Number </b> : " + UserMobileNo + " <br/> <b> Shipping DateTime </b> : " + Convert.ToDateTime(dtorder.Rows[0]["ShippingDatetime"].ToString()).ToString("dd-MM-yyyy HH:mm:ss tt") + "<br/> <b> Order ID  </b> : " + dtorder.Rows[0]["load_inquiry_no"].ToString() + "");
                //}

                if (dt_paramTo != null && dt_paramTo.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_paramTo.Rows.Count; i++)
                    {
                        sendto = dt_paramTo.Rows[i]["value"].ToString().Trim();
                        bl = objemail.SendMail(sendto, newmsg.ToString(), " Booked: (" + ordertype + ")(" + SizetypeCode + ") (" + LoadInquiryno + ") ( " + shippername + " )", ref msg, fromemail, frompword, "TruKKer Technologies");
                    }

                }
                return bl;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool SendQuotationRequestMailandSaveData(ref IDbCommand DBCommand, ref DataTable dtReqQuMail, ref DataTable dtParameters, ref string Message)
        {
            try
            {
                DataTable dtquot = null;
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
                Document objDocument = new Document();
                Master master = new Master();

                if (dtParameters != null && dtParameters.Rows.Count > 0)
                {
                    EMail objemail = new EMail();

                    BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                    DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref Message);
                    if (dt_param == null)
                    {
                        ServerLog.Log(Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; Message = "";

                    dt_param = null; Message = "";
                    StringBuilder newmsg = new StringBuilder();
                    newmsg.Clear(); string strdatatable = "";


                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    DataTable dtSizeTypeMst = new DataTable();
                    DataTable dtHireTruckSizeTypeMst = new DataTable();
                    string SizeTypeCode = "";
                    string goods_type_flag = ""; decimal TotalDistance = 0; string TotalDistanceUOM = ""; decimal TimeToTravelInMinute = 0; string IncludePackingCharge = ""; int? NoOfTruck = null; int? NoOfDriver = null; int? NoOfLabour = null; int? NoOfHandiman = null; int? NoOfSupervisor = null;
                    DateTime OrderShippingDatetime = new DateTime();
                    if (dtParameters.Rows[0]["ShippingDatetime"].ToString() != "")
                        OrderShippingDatetime = Convert.ToDateTime(dtParameters.Rows[0]["ShippingDatetime"].ToString());
                    else
                        OrderShippingDatetime = System.DateTime.UtcNow;

                    string order_type_flag = ""; DateTime OrderTodate = new DateTime();

                    if (dtParameters.Rows[0]["load_inquiry_no"].ToString() != null || dtParameters.Rows[0]["load_inquiry_no"].ToString() != "")
                    {
                        DataTable dt = new PostOrderController().GetLoadInquiryBySizetypeId(dtParameters.Rows[0]["load_inquiry_no"].ToString());
                        SizeTypeCode = dt.Rows[0]["SizeTypeCode"].ToString();
                        goods_type_flag = dt.Rows[0]["goods_type_flag"].ToString();
                        TotalDistance = Convert.ToDecimal(dtParameters.Rows[0]["TotalDistance"].ToString());
                        TotalDistanceUOM = dtParameters.Rows[0]["TotalDistanceUOM"].ToString();
                        TimeSpan Tsshippingtime = TimeSpan.Parse(dt.Rows[0]["TimeToTravelInMinute"].ToString());
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);
                        IncludePackingCharge = "Y";
                        order_type_flag = dt.Rows[0]["order_type_flag"].ToString();
                        if (dt.Rows[0]["Hiretruck_To_datetime"].ToString() != "")
                            OrderTodate = Convert.ToDateTime(dt.Rows[0]["Hiretruck_To_datetime"].ToString());

                        if (order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        {
                            dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtquot, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                return false;
                        }
                        else if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        {
                            dtHireTruckSizeTypeMst = objTruckerMaster.CalculateRateHireTruck(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            if (dtHireTruckSizeTypeMst == null || dtHireTruckSizeTypeMst.Rows.Count <= 0)
                                return false;
                        }
                        else
                        {
                            dtSizeTypeMst = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "P", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                return false;

                            DataTable dtSizeTypeMstBudget = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstBudget == null || dtSizeTypeMstBudget.Rows.Count <= 0)
                                return false;

                            DataTable dtSizeTypeMstSuperSaver = objTruckerMaster.CalculateRate(dt, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "S", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstSuperSaver == null || dtSizeTypeMstSuperSaver.Rows.Count <= 0)
                                return false;

                            DataRow dr_temp_sizetypeBudget = dtSizeTypeMst.NewRow();
                            dtSizeTypeMstBudget.Rows[0]["rate_type_flag"] = "Standard";
                            dr_temp_sizetypeBudget.ItemArray = dtSizeTypeMstBudget.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeBudget);

                            DataRow dr_temp_sizetypeSuperSaver = dtSizeTypeMst.NewRow();
                            dtSizeTypeMstSuperSaver.Rows[0]["rate_type_flag"] = "Super Saver";
                            dr_temp_sizetypeSuperSaver.ItemArray = dtSizeTypeMstSuperSaver.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeSuperSaver);

                            dtSizeTypeMst.Rows[0]["rate_type_flag"] = "Premium";
                        }

                        if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        {
                            string[] selectedColumns = new[] { "NoOfTruck", "NoOfDriver", "NoOfLabour", "NoOfHandiman", "HireTruck_MinRate", "HireTruck_FuelRatePerDay", "HireTruck_AdditionalKMCharges", "Total_cost" };

                            DataTable dtprice = new DataView(dtHireTruckSizeTypeMst).ToTable(false, selectedColumns);
                            strdatatable = ExportDatatableToHtml(dtprice);
                        }
                        else
                        {
                            string[] selectedColumns = new[] { "rate_type_flag", "NoOfTruck", "NoOfDriver", "NoOfLabour", "NoOfHandiman", "NoOfSupervisor", "TotalPackingRate", "Total_cost" };

                            DataTable dtprice = new DataView(dtSizeTypeMst).ToTable(false, selectedColumns);
                            strdatatable = ExportDatatableToHtml(dtprice);

                        }

                    }

                    if (dtParameters.Rows[0]["load_inquiry_no"].ToString() == null || dtParameters.Rows[0]["load_inquiry_no"].ToString() == "")
                        newmsg.Append(" Hello, <br/> <b> Subject </b>: " + dtParameters.Rows[0]["subject"].ToString() + " by <b>" + dtParameters.Rows[0]["name"].ToString() + " <b> <br/><b> Email </b>:" + dtParameters.Rows[0]["EmailID"].ToString() + " <br/> <b> Mobile Number </b> : " + dtParameters.Rows[0]["mobile_no"].ToString() + " <br/> <b> Shipping DateTime </b> : " + dtParameters.Rows[0]["ShippingDatetime"].ToString() + "");
                    else
                    {
                        if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        {
                            newmsg.Append("<b> Subject </b>: Quotation Request <br/> <b> Truck Type </b>: " + dtParameters.Rows[0]["Sizetypecode"].ToString() + " <br/>  <b> Start Point </b>: " + dtParameters.Rows[0]["Sourceaddress"].ToString() + " <br/> <b> UserName </b>: " + dtParameters.Rows[0]["name"].ToString() + " <br/><b> Email </b>:" + dtParameters.Rows[0]["EmailID"].ToString() + " <br/> <b> Mobile Number </b> : " + dtParameters.Rows[0]["mobile_no"].ToString() + " ");
                            newmsg.Append(" <br/> <b> Start  Date </b> : " + dtParameters.Rows[0]["ShippingDatetime"].ToString() + "<br/> <b> To Date </b> : " + OrderTodate.ToString("dd MMM yyyy") + "<br/> <b> Order ID  </b> : " + dtParameters.Rows[0]["load_inquiry_no"].ToString() + "");
                        }
                        else
                        {
                            string txttype = order_type_flag == Constant.ORDERTYPECODEFORHOME ? " Size  " : " Truck ";
                            newmsg.Append("<b> Subject </b>: Quotation Request <br/> <b> " + txttype + " Type </b>: " + dtParameters.Rows[0]["Sizetypecode"].ToString() + " <br/>  <b> Source Address </b>: " + dtParameters.Rows[0]["Sourceaddress"].ToString() + " <br/>  <b> Destination Address </b>: " + dtParameters.Rows[0]["Destinationaddress"].ToString() + " <br/>  <b> Total Distance </b>: " + TotalDistance + " " + TotalDistanceUOM + " <br/> <b> UserName </b>: " + dtParameters.Rows[0]["name"].ToString() + " <br/><b> Email </b>:" + dtParameters.Rows[0]["EmailID"].ToString() + " <br/> <b> Mobile Number </b> : " + dtParameters.Rows[0]["mobile_no"].ToString() + " <br/> <b> Order ID  </b> : " + dtParameters.Rows[0]["load_inquiry_no"].ToString() + "");
                        }
                    }
                    newmsg.Append("<br/> <br/> <b> Quotation details  </b>: <br/>");
                    newmsg.Append(strdatatable);

                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref Message);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    if (dtParameters.Rows[0]["load_inquiry_no"].ToString() == null || dtParameters.Rows[0]["load_inquiry_no"].ToString() == "")
                        bl = objemail.SendMail(sendto, newmsg.ToString(), " Request for Moving Home from  " + dtParameters.Rows[0]["name"].ToString() + "", ref Message, fromemail, frompword, "TruKKer Technologies");
                    else
                    {
                        string ordertype = "";
                        if (order_type_flag == Constant.ORDERTYPECODEFORHOME)
                            ordertype = " Moving Home ";
                        else if (order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                            ordertype = " Moving Goods ";
                        else if (order_type_flag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                            ordertype = " Hire Truck ";

                        bl = objemail.SendMail(sendto, newmsg.ToString(), " Request Quotation for " + ordertype + " (Order ID: " + dtParameters.Rows[0]["load_inquiry_no"].ToString() + ") by " + dtParameters.Rows[0]["name"].ToString() + " from Dubizzle  ", ref Message, fromemail, frompword, "TruKKer Technologies");
                    }
                    string logid = "";

                    if (!objDocument.W_GetNextDocumentNoNew(ref DBCommand, "RQM", "", "", ref logid, ref Message))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    dsinqeml.EnforceConstraints = false;
                    dsinqeml.RequestQuotationMailLog.ImportRow(dtParameters.Rows[0]);
                    dsinqeml.RequestQuotationMailLog[0].logid = logid;
                    dsinqeml.RequestQuotationMailLog[0].Message = newmsg.ToString();
                    dsinqeml.RequestQuotationMailLog[0].maildate = System.DateTime.UtcNow;
                    if (bl)
                        dsinqeml.RequestQuotationMailLog[0].Status = "Success";
                    else
                    {
                        dsinqeml.RequestQuotationMailLog[0].Status = "Error";
                        ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + Message);
                    }
                    dsinqeml.RequestQuotationMailLog[0].Sizetypecode = dtParameters.Rows[0]["Sizetypecode"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].Sourceaddress = dtParameters.Rows[0]["Sourceaddress"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].Destinationaddress = dtParameters.Rows[0]["Destinationaddress"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].shippingdatetime = dtParameters.Rows[0]["ShippingDatetime"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].load_inquiry_no = dtParameters.Rows[0]["load_inquiry_no"].ToString();

                    dsinqeml.RequestQuotationMailLog[0].AcceptChanges();
                    dsinqeml.RequestQuotationMailLog[0].SetAdded();
                    dsinqeml.EnforceConstraints = true;
                    if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables["RequestQuotationMailLog"] != null && dsinqeml.Tables["RequestQuotationMailLog"].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsinqeml.RequestQuotationMailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }
        }

        public bool SendDubi_Goods_QuotationRequestMailandSaveData(ref IDbCommand DBCommand, ref DataTable dtParameters, ref string Message)
        {
            try
            {
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_InquiryEmailLog dsinqeml = new DS_InquiryEmailLog();
                Document objDocument = new Document();
                Master master = new Master();

                if (dtParameters != null && dtParameters.Rows.Count > 0)
                {
                    EMail objemail = new EMail();

                    BLGeneralUtil blgn = new BLGeneralUtil(); Boolean bl;
                    DataTable dt_param = blgn.GetParameter("CONTACT", "SENDFROM", ref Message);
                    if (dt_param == null)
                    {
                        ServerLog.Log(Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                    string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();
                    string sendto = ""; Message = "";

                    dt_param = null; Message = "";
                    StringBuilder newmsg = new StringBuilder();
                    newmsg.Clear();

                    string order_type_flag = ""; DateTime OrderTodate = new DateTime();

                    newmsg.Append("<b> Subject </b>: Quotation Request from Dubizzle <br/> <b> Item Description </b>: " + dtParameters.Rows[0]["subject"].ToString() + " <br/>  <b> Source Address </b>: " + dtParameters.Rows[0]["Sourceaddress"].ToString() + " <br/>  <b> Destination Address </b>: " + dtParameters.Rows[0]["Destinationaddress"].ToString() + " <br/>  <b> Total Distance </b>: " + dtParameters.Rows[0]["TotalDistance"].ToString() + " " + dtParameters.Rows[0]["TotalDistanceUOM"].ToString() + " <br/> <b> UserName </b>: " + dtParameters.Rows[0]["name"].ToString() + " <br/><b> Email </b>:" + dtParameters.Rows[0]["EmailID"].ToString() + " <br/> <b> Mobile Number </b> : " + dtParameters.Rows[0]["mobile_no"].ToString() + " <br/> <b> Order ID  </b> : " + dtParameters.Rows[0]["load_inquiry_no"].ToString() + "");

                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref Message);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    if (dtParameters.Rows[0]["load_inquiry_no"].ToString() == null || dtParameters.Rows[0]["load_inquiry_no"].ToString() == "")
                        bl = objemail.SendMail(sendto, newmsg.ToString(), " Request for Moving Home from  " + dtParameters.Rows[0]["name"].ToString() + "", ref Message, fromemail, frompword, "TruKKer Technologies");
                    else
                    {
                            bl = objemail.SendMail(sendto, newmsg.ToString(), " Request Quotation for Moving Goods (Order ID: " + dtParameters.Rows[0]["load_inquiry_no"].ToString() + ") by " + dtParameters.Rows[0]["name"].ToString() + " from Dubizzle  ", ref Message, fromemail, frompword, "TruKKer Technologies");
                    }
                    string logid = "";

                    if (!objDocument.W_GetNextDocumentNoNew(ref DBCommand, "RQM", "", "", ref logid, ref Message))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    dsinqeml.EnforceConstraints = false;
                    dsinqeml.RequestQuotationMailLog.ImportRow(dtParameters.Rows[0]);
                    dsinqeml.RequestQuotationMailLog[0].logid = logid;
                    dsinqeml.RequestQuotationMailLog[0].Message = newmsg.ToString();
                    dsinqeml.RequestQuotationMailLog[0].maildate = System.DateTime.UtcNow;
                    if (bl)
                        dsinqeml.RequestQuotationMailLog[0].Status = "Success";
                    else
                    {
                        dsinqeml.RequestQuotationMailLog[0].Status = "Error";
                        ServerLog.Log("Error Sending Email on " + sendto + Environment.NewLine + Message);
                    }
                    dsinqeml.RequestQuotationMailLog[0].Sizetypecode = dtParameters.Rows[0]["Sizetypecode"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].Sourceaddress = dtParameters.Rows[0]["Sourceaddress"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].Destinationaddress = dtParameters.Rows[0]["Destinationaddress"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].shippingdatetime = dtParameters.Rows[0]["ShippingDatetime"].ToString();
                    dsinqeml.RequestQuotationMailLog[0].load_inquiry_no = dtParameters.Rows[0]["load_inquiry_no"].ToString();

                    dsinqeml.RequestQuotationMailLog[0].AcceptChanges();
                    dsinqeml.RequestQuotationMailLog[0].SetAdded();
                    dsinqeml.EnforceConstraints = true;
                    if (dsinqeml != null && dsinqeml.Tables != null && dsinqeml.Tables["RequestQuotationMailLog"] != null && dsinqeml.Tables["RequestQuotationMailLog"].Rows.Count > 0)
                    {
                        objBLReturnObject = master.UpdateTables(dsinqeml.RequestQuotationMailLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }
        }
    }
}


