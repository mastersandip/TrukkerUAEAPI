using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Utilities;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Xml;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using trukkerUAE.Controllers;
using System.Net;
using System.Web;
using trukkerUAE.Classes;

namespace BLL.Utilities
{
    public class EMail : ServerBase
    {
        public void Dispose()
        {
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public Boolean SendMail(string to, string msg, string subject, ref string errmsg, string fromemail, string frompword, string Title)
        {
            try
            {
                string smtphst = ConfigurationManager.AppSettings["EmailHost"].ToString();
                string smtpprt = ConfigurationManager.AppSettings["EmailPort"].ToString();

                if (smtphst == "")
                {
                    errmsg = "SMTP HOST settings not found . . . ";
                    return false;
                }
                if (smtpprt == "")
                {
                    errmsg = "SMTP Port settings not found . . . ";
                    return false;
                }

                SmtpClient sendmail = new SmtpClient();
                sendmail.Credentials = new System.Net.NetworkCredential(fromemail, frompword);
                sendmail.Port = Convert.ToInt16(smtpprt);
                sendmail.Host = smtphst;
                sendmail.EnableSsl = true;

                MailMessage mail = new MailMessage();
                if (Title != "")
                    mail.From = new MailAddress(fromemail, Title);
                else
                    mail.From = new MailAddress(fromemail, "Trukker");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                try
                {
                    sendmail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sendmail.Send(mail);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message);
                    errmsg = ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        public Boolean SendMail(string to, string msg, string subject, ref string errmsg, string param_code, string param_key)
        {
            try
            {
                string smtphst = ConfigurationManager.AppSettings["EmailHost"].ToString();
                string smtpprt = ConfigurationManager.AppSettings["EmailPort"].ToString();
                string fromemail = ""; string frompword = "";
                BLGeneralUtil blgn = new BLGeneralUtil();
                DataTable dt_param = blgn.GetParameter(param_code, param_key, ref msg);
                if (dt_param == null)
                {
                    errmsg = "Email Parameter Not found ";
                    return false;
                }
                fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                frompword = dt_param.Rows[0]["Remark"].ToString().Trim();

                if (smtphst == "")
                {
                    errmsg = "SMTP HOST settings not found . . . ";
                    return false;
                }
                if (smtpprt == "")
                {
                    errmsg = "SMTP Port settings not found . . . ";
                    return false;
                }

                SmtpClient sendmail = new SmtpClient();
                //sendmail.Credentials = new System.Net.NetworkCredential("testaarin5889@gmail.com", "aarin@123");
                sendmail.Credentials = new System.Net.NetworkCredential(fromemail, frompword);
                sendmail.Port = Convert.ToInt16(smtpprt);
                sendmail.Host = smtphst;
                sendmail.EnableSsl = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(to, "TruKKer Technologies ", System.Text.Encoding.UTF8);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                // mail.ReplyTo = new MailAddress("vachhanipoojad@gmail.com");
                // mail.ReplyTo = new MailAddress(user_id);
                try
                {
                    sendmail.Send(mail);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message);
                    errmsg = ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        public Boolean SendOrderCompletaionMail(string to, string msg, string subject, ref string errmsg, string param_code, string param_key, string AttachmentFilePath)
        {
            try
            {
                string smtphst = ConfigurationManager.AppSettings["EmailHost"].ToString();
                string smtpprt = ConfigurationManager.AppSettings["EmailPort"].ToString();
                string fromemail = ""; string frompword = "";
                BLGeneralUtil blgn = new BLGeneralUtil();
                DataTable dt_param = blgn.GetParameter(param_code, param_key, ref msg);
                if (dt_param == null)
                {
                    errmsg = "Email Parameter Not found ";
                    return false;
                }
                fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                frompword = dt_param.Rows[0]["Remark"].ToString().Trim();

                if (smtphst == "")
                {
                    errmsg = "SMTP HOST settings not found . . . ";
                    return false;
                }
                if (smtpprt == "")
                {
                    errmsg = "SMTP Port settings not found . . . ";
                    return false;
                }

                SmtpClient sendmail = new SmtpClient();
                //sendmail.Credentials = new System.Net.NetworkCredential("testaarin5889@gmail.com", "aarin@123");
                sendmail.Credentials = new System.Net.NetworkCredential(fromemail, frompword);
                sendmail.Port = Convert.ToInt16(smtpprt);
                sendmail.Host = smtphst;
                sendmail.EnableSsl = true;
                MailMessage mail = new MailMessage();

                if (AttachmentFilePath != null)
                    mail.Attachments.Add(new Attachment(AttachmentFilePath));

                mail.From = new MailAddress(to, "TruKKer Technologies ", System.Text.Encoding.UTF8);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                // mail.ReplyTo = new MailAddress("vachhanipoojad@gmail.com");
                // mail.ReplyTo = new MailAddress(user_id);
                try
                {
                    sendmail.Send(mail);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message);
                    errmsg = ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                errmsg = ex.Message;
                return false;
            }
            return true;
        }


        public string SendOtpToUserMobileNoUAE(string Msg, string phoneNo)
        {
            if (phoneNo != "" && phoneNo != null)
            {
                try
                {
                    using (System.Net.WebClient client = new WebClient())
                    {
                        string createdURL = "";
                        string domain = ConfigurationManager.AppSettings["OTPdomain"].ToString();
                        string username = ConfigurationManager.AppSettings["OTPusername"].ToString();
                        string password = ConfigurationManager.AppSettings["OTPpassword"].ToString();
                        string sender = ConfigurationManager.AppSettings["OTPsender"].ToString();

                        if (Msg.Length > 160)
                            createdURL = domain + "?username=" + username + "&password=" + password + "&senderid=" + sender + "&to=971" + phoneNo + "&text=" + Msg + "&type=text";
                        else
                            createdURL = domain + "?username=" + username + "&password=" + password + "&senderid=" + sender + "&to=971" + phoneNo + "&text=" + Msg + "&type=text";

                        string str = client.DownloadString(createdURL);
                        JObject o = JObject.Parse(str);

                        //This will be "Apple"
                        JObject o2 = JObject.Parse((string)o["data"].ToString());
                        string status = (string)o2["status"];
                        if (status == "SUCCESS")
                        {
                            ServerLog.OTPLog("OTP Message Send Sucessfully - " + phoneNo);
                            return BLGeneralUtil.return_ajax_string("1", "OTP Message Send Sucessfully");

                        }
                        else
                        {
                            ServerLog.OTPLog("OTP Message Not Send - " + phoneNo);
                            return BLGeneralUtil.return_ajax_string("0", "OTP Not Send");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ServerLog.OTPLog(ex.Message);
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Phone Number Required");
            }
        }
        ///by sandip 7/4/2016 send otp to mobile number
        public string SendOtpToUserMobileNo(string Msg, string phoneNo)
        {
            if (phoneNo != "" && phoneNo != null)
            {
                try
                {


                    using (System.Net.WebClient client = new WebClient())
                    {
                        string createdURL = "";
                        string domain = ConfigurationManager.AppSettings["OTPdomain"].ToString();
                        string username = ConfigurationManager.AppSettings["OTPusername"].ToString();
                        string password = ConfigurationManager.AppSettings["OTPpassword"].ToString();
                        string sender = ConfigurationManager.AppSettings["OTPsender"].ToString();

                        if (Msg.Length > 160)
                            createdURL = domain + "?user=" + username + "&password=" + password + "&sender=" + sender + "&SMSText=" + Msg + "&GSM=91" + phoneNo + "&type=longSMS";
                        else
                            createdURL = domain + "?user=" + username + "&password=" + password + "&sender=" + sender + "&SMSText=" + Msg + "&GSM=91" + phoneNo;

                        string str = client.DownloadString(createdURL);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(str);
                        XmlNodeList xnode = xmlDoc.SelectNodes("results/result/status");
                        if (xnode[0].InnerText == "0")
                        {
                            ServerLog.OTPLog("OTP Message Send Sucessfully - " + phoneNo);
                            return BLGeneralUtil.return_ajax_string("1", "OTP Message Send Sucessfully");

                        }
                        else
                        {
                            ServerLog.OTPLog("OTP Message Send Sucessfully - " + phoneNo);
                            return BLGeneralUtil.return_ajax_string("0", "OTP Not Send");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ServerLog.OTPLog(ex.Message);
                    return "";
                }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Phone Number Required");
            }
        }

        //<table style="min-width: 100%; border-collapse: collapse" width="100%" cellspacing="0" cellpadding="0" border="0">
        //<tbody><tr><td valign="top"><table style="min-width: 100%; border-collapse: collapse" width="100%" cellspacing="0" cellpadding="0" border="0" align="left">
        //<tbody><tr><td style="padding-top: 9px; padding-left: 18px; padding-bottom: 9px; padding-right: 18px">
        //<table style="min-width: 100%!important; background-color: #eeeeee; border-collapse: collapse" width="100%" cellspacing="0" cellpadding="18" border="0">
        //<tbody><tr><td style="color: #f2f2f2; font-family: Helvetica; font-size: 14px; font-weight: normal; line-height: 100%; text-align: left; word-break: break-word" valign="top">
        //<table style="width: 100%; font-size: 16px; border-collapse: collapse"><tbody>
        // <tr><td style="text-align: left"><span style="font-size: 13px"><strong><span style="color: #000000">
        // <img src="https://ci4.googleusercontent.com/proxy/vZpL9Qyk5nq2DXkztP_nJZCmV2Kj6ivIxyULI61-Cx2zW6oWvOJYMfimZMca3nSdh8KcIAGuL5jwZZa4g47R_yFRKx_tGofl9R-kL3uZKycxZX2K12FAZsSU_2epByFnS3HaNA07KgqwwK9jsLC9nMJ8tehjDusaXQCgeB0=s0-d-e1-ft#https://gallery.mailchimp.com/5a05ca263fac7c79a5efb199d/images/9f6e2238-c3db-410d-ae48-b75014974e57.png" style="width: 20px; min-height: 20px; margin: 0px 10px 0px 0px; border: 0; outline: none; text-decoration: none" valign="middle" class="CToWUd" width="20" align="none" height="20">Driver &nbsp;: &nbsp;</span></strong><span style="color: #000000">(Name)</span></span></td>
        // <td style="text-align: left"><span style="font-size: 13px"><strong><span style="color: #000000">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<img src="https://ci6.googleusercontent.com/proxy/X0s0obgUYuCFfEK8J1BquUw_j8jfb77WEv9Z7QeWnZkLNcJkqg8wDht3owv0tXbZhKktX3ZeZD7-8vPZpIF8V2Dc9njFesGjocxtGEivnOIUQoEADEkd4GtbKOzGH3f01laWJrmQ97hBMbs-WDqEd0bx03DwTiDkLIeAIwM=s0-d-e1-ft#https://gallery.mailchimp.com/5a05ca263fac7c79a5efb199d/images/e67ae0ad-4808-47bb-aa01-866487316e7a.png" style="width: 20px; min-height: 20px; margin: 0px 10px 0px 0px; border: 0; outline: none; text-decoration: none" valign="middle" class="CToWUd" width="20" align="none" height="20">&nbsp;Truck No &nbsp;: &nbsp;</span></strong><span style="color: #000000">GJ 10 9542</span></span></td>
        // </tr><tr><td style="text-align: left"><span style="font-size: 13px"><strong><span style="color: #000000">
        // <img src="https://ci4.googleusercontent.com/proxy/kjCXciKZdB8ljOeqxrCO60KuMMiTrPb46T_pt7v-FbxsyAoeapdBvqyc2ZkmYJF9v2SNFzMBeWp7yzCKEiCoaa15V8xVC1NMej6MIYyEOUUmMv3pKnMvgnNfzaydpiuAYJGf9dW092qADT48iW0Qb72uSeJBhwFZSGo6erY=s0-d-e1-ft#https://gallery.mailchimp.com/5a05ca263fac7c79a5efb199d/images/0801597e-3da9-49d1-bd4c-b236dc4b12b5.png" style="width: 20px; min-height: 20px; margin: 0px; border: 0; outline: none; text-decoration: none" valign="middle" class="CToWUd" width="20" align="none" height="20">&nbsp; Start Time &nbsp;: &nbsp;</span></strong><span style="color: #000000"><span class="aBn" data-term="goog_216788853" tabindex="0"><span class="aQJ">10:00 am</span></span></span></span></td>
        // <td style="text-align: left"><span style="font-size: 13px"><strong><span style="color: #000000">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<img src="https://ci4.googleusercontent.com/proxy/kjCXciKZdB8ljOeqxrCO60KuMMiTrPb46T_pt7v-FbxsyAoeapdBvqyc2ZkmYJF9v2SNFzMBeWp7yzCKEiCoaa15V8xVC1NMej6MIYyEOUUmMv3pKnMvgnNfzaydpiuAYJGf9dW092qADT48iW0Qb72uSeJBhwFZSGo6erY=s0-d-e1-ft#https://gallery.mailchimp.com/5a05ca263fac7c79a5efb199d/images/0801597e-3da9-49d1-bd4c-b236dc4b12b5.png" style="width: 20px; min-height: 20px; margin: 0px; border: 0; outline: none; text-decoration: none" valign="middle" class="CToWUd" width="20" align="none" height="20">&nbsp; &nbsp;End Time &nbsp;: &nbsp;</span></strong><span style="color: #000000">10::00 pm</span></span></td>
        // </tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table>

        //[HttpGet]
        //public string GenerateOrderGenerationMail(string mailto, string title, string UserName, string message, string OrderID, DataTable dtorder, string Packingrate)
        //{
        //    string domian = ConfigurationManager.AppSettings["Domain"];
        //    StreamReader sr; string LINK = "";
        //    if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
        //        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMail.html");
        //    else
        //        sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMailGoods.html");

        //    string strdata = ""; string msg = "";
        //    string TITLE = title;
        //    string addonloadinquiry = BLGeneralUtil.Encrypt(dtorder.Rows[0]["load_inquiry_no"].ToString());

        //    while (!sr.EndOfStream)
        //    {
        //        double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
        //        int totallaborWithDriver = Convert.ToInt32(dtorder.Rows[0]["NoOfLabour"].ToString());

        //        strdata = sr.ReadToEnd();
        //        strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
        //        strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
        //        strdata = strdata.Replace("USERNAME", UserName);
        //        strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
        //        strdata = strdata.Replace("SHIPPINGDATETIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy hh:mm tt"));
        //        strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
        //        strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
        //        strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
        //        strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
        //        strdata = strdata.Replace("NOOFLABOUR", totallaborWithDriver.ToString());
        //        strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
        //        strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
        //        strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["cbmlink"].ToString());
        //        strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
        //        strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
        //        strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
        //        strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
        //        // strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["inquiry_source_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
        //        if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
        //            strdata = strdata.Replace("MOVINGTYPE", "Home");
        //        else
        //            strdata = strdata.Replace("MOVINGTYPE", "Goods");

        //        if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
        //            strdata = strdata.Replace("PACKINGCHARGE", "(Include packaging charge AED " + Math.Round(Convert.ToDecimal(Packingrate)) + " )");
        //        else
        //            strdata = strdata.Replace("PACKINGCHARGE", "");
        //        strdata = strdata.Replace("ADDONLINK", addonloadinquiry);

        //        string SOURCELAT = dtorder.Rows[0]["inquiry_source_lat"].ToString();
        //        string SOURCELONGS = dtorder.Rows[0]["inquiry_source_lng"].ToString();
        //        string DESTLAT = dtorder.Rows[0]["inquiry_destionation_lat"].ToString();
        //        string DESTILONGS = dtorder.Rows[0]["inquiry_destionation_lng"].ToString();

        //        //string str = "";
        //        //str = "<script type=\"text/javascript\"> " +
        //        //      " initMap();" +
        //        //      "  function initMap() {" +
        //        //      "  var latlng = new google.maps.LatLng(23.0158, 72.5045);" +
        //        //      "  var directionsService = new google.maps.DirectionsService;" +
        //        //      "  //var directionsDisplay = new google.maps.DirectionsRenderer;" +
        //        //      "  var directionsDisplay = new google.maps.DirectionsRenderer({ suppressMarkers: true });" +
        //        //      "  // var bounds = new google.maps.LatLngBounds();" +
        //        //      "  var map = new google.maps.Map(document.getElementById('dvMap_current'), {" +
        //        //      "      zoom: 17," +
        //        //      "      center: latlng," +
        //        //      "      scrollwheel: false," +
        //        //      "      navigationControl: false," +
        //        //      "      mapTypeControl: false," +
        //        //      "      scaleControl: false," +
        //        //      "      draggable: false," +
        //        //      "      panControl: false," +
        //        //      "      zoomControl: false," +
        //        //      "      disableDoubleClickZoom: true," +
        //        //      "      streetViewControl: false," +
        //        //      "      mapTypeId: google.maps.MapTypeId.ROADMAP" +
        //        //      "  });" +
        //        //      "  directionsDisplay.setMap(map);" +
        //        //      "  calculateAndDisplayRoute(directionsService, directionsDisplay);" +
        //        //       " }" +
        //        //   "     function calculateAndDisplayRoute(directionsService, directionsDisplay) {" +
        //        //   "        directionsService.route({" +
        //        //   "            origin: new google.maps.LatLng(23.0158, 72.5045), " +
        //        //   "   destination: new google.maps.LatLng(23.002272,  72.502243), " +
        //        //    //"            origin: new google.maps.LatLng(" + SOURCELAT + "," + SOURCELONGS + ")," +
        //        //    //"            destination: new google.maps.LatLng(" + DESTLAT + "," + DESTILONGS + ")," +
        //        //   "            travelMode: 'DRIVING'" +
        //        //   "        }, function (response, status) {" +
        //        //   "            if (status === 'OK') {" +
        //        //   "                directionsDisplay.setDirections(response);" +
        //        //   "            } else {" +
        //        //   "                window.alert('Directions request failed due to ' + status);" +
        //        //   "            }" +
        //        //   "        });" +
        //        //   "    }" +
        //        //   " </script>";

        //        //strdata = strdata.Replace("JQUERY", str);
        //    }

        //    sr.Close();
        //    msg = "";
        //    EMail objemail = new EMail();
        //    Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
        //    if (!bl)
        //    {
        //        return BLGeneralUtil.return_ajax_string("0", msg);
        //    }
        //    return BLGeneralUtil.return_ajax_string("1", "Registration Email Sent Successfully");
        //}

        [HttpGet]
        public string GenerateOrderGenerationMail(string mailto, string title, string UserName, string message, string OrderID, DataTable dtorder, string Packingrate)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";

            if (dtorder.Rows[0]["order_type_flag"].ToString() == "H" && dtorder.Rows[0]["order_by"].ToString().Trim() == "DUBI")
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\DubiOrderConfirmationMailhome.html");
            else if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMail.html");
            else
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMailGoods.html");

            string strdata = ""; string msg = "";
            string TITLE = title;
            string addonloadinquiry = BLGeneralUtil.Encrypt(dtorder.Rows[0]["load_inquiry_no"].ToString());
            DataTable dtaddonservices = new PostOrderController().GetorderAddonServiceDetailsByid(dtorder.Rows[0]["load_inquiry_no"].ToString());

            while (!sr.EndOfStream)
            {
                double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
                int totallaborWithDriver = Convert.ToInt32(dtorder.Rows[0]["NoOfLabour"].ToString());

                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                strdata = strdata.Replace("USERNAME", UserName);

                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H" && dtorder.Rows[0]["order_by"].ToString().Trim() == "DUBI")
                {
                    if (dtorder.Rows[0]["payment_mode"].ToString() == Constant.PaymentModeONLINE)
                        strdata = strdata.Replace("TXTPAYMENT", "<span style=\"font-size: 16px\">We have received your payment of <b> AED " + totalprice.ToString() + " </b> </span><br> ");
                    else
                        strdata = strdata.Replace("TXTPAYMENT", " <span style=\"font-size: 16px\">Payment of <b> AED " + totalprice.ToString() + " </b> due in cash on your flawless move day. </span><br> ");
                }
                else
                    strdata = strdata.Replace("TXTPAYMENT", "");

                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
                strdata = strdata.Replace("SHIPPINGDATETIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy hh:mm tt"));
                strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                strdata = strdata.Replace("NOOFLABOUR", totallaborWithDriver.ToString());
                strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
                strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["cbmlink"].ToString());
                strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                // strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["inquiry_source_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                    strdata = strdata.Replace("MOVINGTYPE", "Home");
                else
                    strdata = strdata.Replace("MOVINGTYPE", "Goods");

                string stritem = "";
                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                {
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
                        stritem = "," + stritem;
                    }
                    else
                        stritem = "";
                }

                if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                    strdata = strdata.Replace("PACKINGCHARGE", "(Include Packaging " + stritem + " charges )");
                else if (dtorder.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                    strdata = strdata.Replace("PACKINGCHARGE", "(Include " + stritem + " charges )");
                else
                    strdata = strdata.Replace("PACKINGCHARGE", "");

                strdata = strdata.Replace("'PT'", "Painting");
                strdata = strdata.Replace("'CL'", "Cleaning");
                strdata = strdata.Replace("'PEST'", "Pest Control");

                //if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                //    strdata = strdata.Replace("PACKINGCHARGE", "(Include packaging charge AED " + Math.Round(Convert.ToDecimal(Packingrate)) + " )");
                //else
                //    strdata = strdata.Replace("PACKINGCHARGE", "");
                strdata = strdata.Replace("ADDONLINK", addonloadinquiry);

                string SOURCELAT = dtorder.Rows[0]["inquiry_source_lat"].ToString();
                string SOURCELONGS = dtorder.Rows[0]["inquiry_source_lng"].ToString();
                string DESTLAT = dtorder.Rows[0]["inquiry_destionation_lat"].ToString();
                string DESTILONGS = dtorder.Rows[0]["inquiry_destionation_lng"].ToString();

                //string str = "";
                //str = "<script type=\"text/javascript\"> " +
                //      " initMap();" +
                //      "  function initMap() {" +
                //      "  var latlng = new google.maps.LatLng(23.0158, 72.5045);" +
                //      "  var directionsService = new google.maps.DirectionsService;" +
                //      "  //var directionsDisplay = new google.maps.DirectionsRenderer;" +
                //      "  var directionsDisplay = new google.maps.DirectionsRenderer({ suppressMarkers: true });" +
                //      "  // var bounds = new google.maps.LatLngBounds();" +
                //      "  var map = new google.maps.Map(document.getElementById('dvMap_current'), {" +
                //      "      zoom: 17," +
                //      "      center: latlng," +
                //      "      scrollwheel: false," +
                //      "      navigationControl: false," +
                //      "      mapTypeControl: false," +
                //      "      scaleControl: false," +
                //      "      draggable: false," +
                //      "      panControl: false," +
                //      "      zoomControl: false," +
                //      "      disableDoubleClickZoom: true," +
                //      "      streetViewControl: false," +
                //      "      mapTypeId: google.maps.MapTypeId.ROADMAP" +
                //      "  });" +
                //      "  directionsDisplay.setMap(map);" +
                //      "  calculateAndDisplayRoute(directionsService, directionsDisplay);" +
                //       " }" +
                //   "     function calculateAndDisplayRoute(directionsService, directionsDisplay) {" +
                //   "        directionsService.route({" +
                //   "            origin: new google.maps.LatLng(23.0158, 72.5045), " +
                //   "   destination: new google.maps.LatLng(23.002272,  72.502243), " +
                //    //"            origin: new google.maps.LatLng(" + SOURCELAT + "," + SOURCELONGS + ")," +
                //    //"            destination: new google.maps.LatLng(" + DESTLAT + "," + DESTILONGS + ")," +
                //   "            travelMode: 'DRIVING'" +
                //   "        }, function (response, status) {" +
                //   "            if (status === 'OK') {" +
                //   "                directionsDisplay.setDirections(response);" +
                //   "            } else {" +
                //   "                window.alert('Directions request failed due to ' + status);" +
                //   "            }" +
                //   "        });" +
                //   "    }" +
                //   " </script>";

                //strdata = strdata.Replace("JQUERY", str);
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


        [HttpGet]
        public string GenerateOrderCompletationMail(string mailto, string title, string UserName, string message, string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            string PDFPAth = ConfigurationManager.AppSettings["InvoicePdfPath"];
            StreamReader sr; string LINK = "";
            //sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderCompletaionMail.html");
            string strdata = ""; string msg = ""; string StrAttachmentPath = "";
            string TITLE = title;



            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            if (dtorder != null)
            {
                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H" && dtorder.Rows[0]["order_by"].ToString().Trim() == "DUBI")
                    sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\DubiOrderComplitionMailhome.html");
                else
                    sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderCompletaionMail.html");
            }
            else
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderCompletaionMail.html");



            string strpdflink = "";
            if (dtorder.Rows[0]["invoice_pdf_link"].ToString() != null)
                strpdflink = dtorder.Rows[0]["invoice_pdf_link"].ToString() != "" ? dtorder.Rows[0]["invoice_pdf_link"].ToString().Split('/')[5].ToString() : "";

            if (dtorder != null && strpdflink != "")
                StrAttachmentPath = PDFPAth + strpdflink;

            //DataTable dtDrivertruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(OrderID);
            string addonloadinquiry = BLGeneralUtil.Encrypt(OrderID);
            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("DOMAIN", domian);
                strdata = strdata.Replace("ADDONLINK", addonloadinquiry);
                //double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
                //strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                //strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                //strdata = strdata.Replace("DRIVERLOGO", ConfigurationManager.AppSettings["Domain"] + "MailerImage/driver.png");
                //strdata = strdata.Replace("TRUCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/truck.png");
                //strdata = strdata.Replace("ClOCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/clock.png");
                strdata = strdata.Replace("UserName", UserName);
                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
                //strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy"));
                //strdata = strdata.Replace("SHIPPINGTIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("h:mm tt"));
                //strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                //strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                //strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                //strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                //strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
                //strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                //strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
                //strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["cbmlink"].ToString());
                //strdata = strdata.Replace("TOTALDISTANCE", dtorder.Rows[0]["TotalDistance"].ToString());
                //strdata = strdata.Replace("TOTALDISTANCEUOM", dtorder.Rows[0]["TotalDistanceUOM"].ToString());
                ////strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                ////strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                ////strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                ////strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                //strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["inquiry_source_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                {
                    strdata = strdata.Replace("MOVINGTYPE", "Hire Truck");
                    strdata = strdata.Replace("TXTHOME", "Thank you for choosing TruKKer! We hope you had a convenient and worry-free experience with TruKKer.");

                }
                else if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
                {
                    strdata = strdata.Replace("MOVINGTYPE", "Moving Home");
                    strdata = strdata.Replace("TXTHOME", " Thank you for choosing TruKKer! We hope you had an amazing and smooth move.");

                }
                else
                {
                    strdata = strdata.Replace("MOVINGTYPE", "Moving Goods");
                    strdata = strdata.Replace("TXTHOME", "Thank you for choosing TruKKer! We hope you had a convenient and worry-free experience with TruKKEr.");

                }
                //if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                //    strdata = strdata.Replace("PACKINGCHARGE", "(Include packaging charge AED " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["TotalPackingCharge"].ToString())) + " )");
                //else
                //    strdata = strdata.Replace("PACKINGCHARGE", "");

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
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendOrderCompletaionMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM", StrAttachmentPath);
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            return BLGeneralUtil.return_ajax_string("1", "Order completation mail send Successfully");
        }

        [HttpGet]
        public string GenerateRegistrationEmailToshipper(string mailto, string regid, string UserName, string UserID, string domainname)
        {
            ServerLog.Log("Mail IN");
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\RegistrationMail_trukkeruae.html");
            string strdata = ""; string msg = "";
            string TITLE = "Trukker Registration ";


            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/RegistrationTopHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/RegistrationBottom.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("USERID", UserID);
                strdata = strdata.Replace("IMAGE3", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Movemyhome.png");
                strdata = strdata.Replace("IMAGE4", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Movemygoods.png");
            }
            sr.Close();
            msg = "";
            Boolean bl = SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            ServerLog.Log("Mail Send");
            return BLGeneralUtil.return_ajax_string("1", "Registration Email Sent Successfully");
        }


        [HttpGet]
        public string GenerateOrderGenerationMail(string mailto, string title, string UserName, string message, string domainname)
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


        [HttpGet]
        public string OrderCancellationEmail(string mailto, string title, string UserName, string message, string inqno)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderCancellationMail.html");
            string strdata = ""; string msg = "";
            string TITLE = title;


            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "grillbg.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("ORDERID", inqno);
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

        [HttpGet]
        public string GenerateBecomePartnerEmail(string mailto, string regid, string UserName, string RegType, string domainname)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUae_BecomePartnerMail.html");
            string strdata = ""; string msg = "";
            string TITLE = "TruKKer Technologies ";

            while (!sr.EndOfStream)
            {
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("DOMAIN", domian);

            }
            sr.Close();
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            return BLGeneralUtil.return_ajax_string("1", "Email Sent Successfully");
        }

        [HttpGet]
        public string GenerateInvoiceMail(string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderInvoiceDetailMailHome.html");
            string strdata = ""; string msg = "";
            string TITLE = "Inoice details";


            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            //DataTable dtDrivertruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(OrderID);
            string addonloadinquiry = BLGeneralUtil.Encrypt(OrderID);
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
                strdata = strdata.Replace("UserName", "sandip");
                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
                strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy"));
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
                strdata = strdata.Replace("TOTALDISTANCEUOM", dtorder.Rows[0]["TotalDistanceUOM"].ToString());
                strdata = strdata.Replace("INVOICEDATE", new PostOrderController().DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow).ToString("dd MMM yyyy"));
                //strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                //strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                //strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                //strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["inquiry_source_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                strdata = strdata.Replace("BASERATE", dtorder.Rows[0]["BaseRate"].ToString());
                strdata = strdata.Replace("TOTALTRAVELINGRATE", dtorder.Rows[0]["TotalTravelingRate"].ToString());
                strdata = strdata.Replace("TOTALDRIVERRATE", dtorder.Rows[0]["TotalDriverRate"].ToString());
                strdata = strdata.Replace("TOTALLABOURRATE", dtorder.Rows[0]["TotalLabourRate"].ToString());
                strdata = strdata.Replace("TOTALHANDIMANRATE", dtorder.Rows[0]["TotalHandimanRate"].ToString());
                strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalSupervisorRate"].ToString());
                strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalSupervisorRate"].ToString());


                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                    strdata = strdata.Replace("MOVINGTYPE", "Home");
                else
                    strdata = strdata.Replace("MOVINGTYPE", "Goods");

                if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                    strdata = strdata.Replace("PACKINGCHARGE", Math.Round(Convert.ToDecimal(dtorder.Rows[0]["TotalPackingCharge"].ToString())).ToString());
                else
                    strdata = strdata.Replace("PACKINGCHARGE", "00");

                if (dtorder != null)
                {
                    // string str = "";
                    decimal Discount = Convert.ToDecimal(dtorder.Rows[0]["Discount"].ToString());
                    if (Discount != 0)
                    {
                        strdata = strdata.Replace("DISCOUNT", Discount.ToString());
                    }

                }

                if (dtorder != null)
                {
                    string str = "";
                    decimal Remaningamt = Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString());
                    decimal Totalcostwithoutdiscount = Convert.ToDecimal(dtorder.Rows[0]["Total_cost_without_discount"].ToString());
                    decimal Recievedamt = Totalcostwithoutdiscount - Remaningamt;
                    if (Remaningamt == 0)
                    {
                        if (dtorder.Rows[0]["rem_amt_to_receive"].ToString() == "P")
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
                        str += "<td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td>";
                        str += "<td style=\"text-align:left\">Paid amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Recievedamt).ToString() + "</td>";
                        str += "<td style=\"text-align:left\">Remaining Amount</td><td style=\"text-align: left\"></td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Remaningamt).ToString() + "</td>";

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

            //var htmlContent = String.Format(sr.ToString(), DateTime.Now);
            //var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            //var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);


            //htmlToPdf.GeneratePdfFromFile("http://www.nrecosite.com/", null, "export.pdf");

            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail("sandippr12@gmail.com", strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }

            return BLGeneralUtil.return_ajax_string("1", "Order completation mail send Successfully");
        }

        [HttpGet]
        public string GeneratePaymentAckMail(string mailto, string title, string UserName, string amount, string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderReciptMail.html");
            string strdata = ""; string msg = "";
            string TITLE = title;


            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            //DataTable dtDrivertruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(OrderID);
            string addonloadinquiry = BLGeneralUtil.Encrypt(OrderID);
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
                strdata = strdata.Replace("UserName", UserName);
                strdata = strdata.Replace("ORDERID", OrderID);
                //strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy"));
                //strdata = strdata.Replace("SHIPPINGTIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("h:mm tt"));
                //strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                //strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                //strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                //strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                //strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
                //strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                //strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
                //strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["cbmlink"].ToString());
                //strdata = strdata.Replace("TOTALDISTANCE", dtorder.Rows[0]["TotalDistance"].ToString());
                //strdata = strdata.Replace("TOTALDISTANCEUOM", dtorder.Rows[0]["TotalDistanceUOM"].ToString());
                //strdata = strdata.Replace("INVOICEDATE", DateTime.Now.ToString("dd MMM yyyy"));
                //strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                //strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                //strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                //strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                //strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["inquiry_source_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                    strdata = strdata.Replace("MOVINGTYPE", "Home");
                else
                    strdata = strdata.Replace("MOVINGTYPE", "Goods");
                strdata = strdata.Replace("AMOUNT", amount);
                //if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                //    strdata = strdata.Replace("PACKINGCHARGE", "( Include packaging charge AED " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["TotalPackingCharge"].ToString())) + " )");
                //else
                //    strdata = strdata.Replace("PACKINGCHARGE", "");

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

            //var htmlContent = String.Format(sr.ToString(), DateTime.Now);
            //var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            //var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);


            //htmlToPdf.GeneratePdfFromFile("http://www.nrecosite.com/", null, "export.pdf");

            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            return BLGeneralUtil.return_ajax_string("1", "Payment ACK mail send Successfully");
        }

        public void GeneratePdf_SendMail(string quote_id, string emailId)
        {
            try
            {
                ReportPrinter obReportPrinter = new ReportPrinter();
                //obReportPrinter.PageFile = Server.MapPath("/") + "Generate_Cbm_Pdf.aspx?quote_id=" + quote_id;
                obReportPrinter.PageFile = "http://localhost:2120/Generate_Cbm_Pdf.html?quote_id=" + quote_id;
                // obReportPrinter.PageFile = "http://182.70.119.213/CBMchamp/Generate_Cbm_Pdf.html?quote_id=" + quote_id;


                obReportPrinter.MarginBottom = "0";
                //string path = Path.Combine(Server.MapPath("/GeneratedPdf/"), quote_id + ".pdf");
                //obReportPrinter.PathToSave = path;

                obReportPrinter.GetPdf();

                Random rd = new Random();
                int n = rd.Next();

                FileStream fs = new FileStream(Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/GeneratedPdf/"), n + ".pdf"), FileMode.Create);
                // FileStream fs = new FileStream(Path.Combine(Server.MapPath("/GeneratedPdf/"), quote_id + "_" + n + ".pdf"), FileMode.Create);
                fs.Write(obReportPrinter.FileContent, 0, obReportPrinter.FileContent.Length);
                fs.Dispose();
                fs.Close();

                // sendMail();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public string OrderRescheduleEmail(string mailto, string title, string UserName, DateTime Reschedualdate, string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderRescheduleRequestMail.html");
            string strdata = ""; string msg = "";
            string TITLE = title;

            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            try
            {
                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();
                    strdata = strdata.Replace("DOMAIN", domian);
                    strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                    strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                    strdata = strdata.Replace("DRIVERLOGO", ConfigurationManager.AppSettings["Domain"] + "MailerImage/driver.png");
                    strdata = strdata.Replace("TRUCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/truck.png");
                    strdata = strdata.Replace("ClOCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/clock.png");
                    strdata = strdata.Replace("UserName", UserName);
                    strdata = strdata.Replace("ORDERID", OrderID);

                    if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                        strdata = strdata.Replace("MOVINGTYPE", "Home");
                    else
                        strdata = strdata.Replace("MOVINGTYPE", "Goods");

                    strdata = strdata.Replace("RESCHEDULDDATE", Reschedualdate.ToString("dd MMM yyyy h:mm tt"));
                    strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy h:mm tt"));
                }
                sr.Close();
            }
            catch (Exception ex)
            {
            }
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }

            return BLGeneralUtil.return_ajax_string("1", " Reschedual Mail send sucessfully  ");
        }

        [HttpGet]
        public string OrderRescheduleConfirmationEmail(string mailto, string title, string UserName, DateTime Shippingdate, string OrderID)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderRescheduleMail.html");
            string strdata = ""; string msg = "";
            string TITLE = title;

            DataTable dtorder = new PostOrderController().GetLoadInquiryById(OrderID);
            try
            {
                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();
                    strdata = strdata.Replace("DOMAIN", domian);
                    strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                    strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                    strdata = strdata.Replace("DRIVERLOGO", ConfigurationManager.AppSettings["Domain"] + "MailerImage/driver.png");
                    strdata = strdata.Replace("TRUCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/truck.png");
                    strdata = strdata.Replace("ClOCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/clock.png");
                    strdata = strdata.Replace("UserName", UserName);
                    strdata = strdata.Replace("ORDERID", OrderID);

                    if (dtorder.Rows[0]["order_type_flag"].ToString() == "H")
                        strdata = strdata.Replace("MOVINGTYPE", "Home");
                    else
                        strdata = strdata.Replace("MOVINGTYPE", "Goods");

                    strdata = strdata.Replace("RESCHEDULDDATE", Shippingdate.ToString("dd MMM yyyy h:mm tt"));
                    strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy h:mm tt"));
                }
                sr.Close();
            }
            catch (Exception ex)
            {
            }
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }

            return BLGeneralUtil.return_ajax_string("1", "Reschedual Mail send sucessfully ");
        }


        [HttpGet]
        public string OrderConfirmationMailHireTruck(string mailto, string title, string UserName, string message, string OrderID, DataTable dtorder, string FuelCharge)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";

            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMailHireTruck.html");

            string strdata = ""; string msg = "";
            string TITLE = title;

            while (!sr.EndOfStream)
            {
                double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
                strdata = strdata.Replace("SHIPPINGDATETIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy hh:mm tt"));

                if (dtorder.Rows[0]["Hiretruck_NoofDay"].ToString() != "1")
                    strdata = strdata.Replace("TODATE", Convert.ToDateTime(dtorder.Rows[0]["Hiretruck_To_datetime"].ToString()).ToString("dd MMM yyyy hh:mm tt"));

                strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
                strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());

                strdata = strdata.Replace("MOVINGTYPE", "Hire Truck");
                strdata = strdata.Replace("NOOFDAYS", dtorder.Rows[0]["Hiretruck_NoofDay"].ToString());
                strdata = strdata.Replace("TRUCKTYPEDESC", dtorder.Rows[0]["TruckTypeDesc"].ToString());


                if (dtorder.Rows[0]["Hiretruck_IncludingFuel"].ToString() == "Y")
                    strdata = strdata.Replace("TOTALFUELCHARGE", "(Include Fuel charge AED " + Math.Round(Convert.ToDecimal(FuelCharge)) + " )");
                else
                    strdata = strdata.Replace("TOTALFUELCHARGE", "");

                if (dtorder.Rows[0]["Hiretruck_NoofDay"].ToString() != "1")
                    strdata = strdata.Replace("COMPDT", " <strong> To <strong> <span style=\"color: #a9a9a9\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif\"><span style=\"font-size: 12px\">" + Convert.ToDateTime(dtorder.Rows[0]["Hiretruck_To_datetime"].ToString()).ToString("dd MMM yyyy hh:mm tt") + "</span></span></span>");
                else
                    strdata = strdata.Replace("COMPDT", "");


                //if (dtorder.Rows[0]["Hiretruck_IncludingFuel"].ToString() == "Y")
                //    strdata = strdata.Replace("TOTALFUELCHARGE", dtorder.Rows[0]["Hiretruck_TotalFuelRate"].ToString());
                //else
                //    strdata = strdata.Replace("TOTALFUELCHARGE", "");

            }

            sr.Close();
            msg = "";
            EMail objemail = new EMail();
            Boolean bl = objemail.SendMail(mailto, strdata, TITLE, ref msg, "CONTACT", "SENDFROM");
            if (!bl)
            {
                return BLGeneralUtil.return_ajax_string("0", msg);
            }
            else
                return BLGeneralUtil.return_ajax_string("1", "Hire Truck Order Confirmation mail Sent Successfully");
        }

        [HttpGet]
        public string GenerateAddonConfirmationMail(string mailto, string title, string UserName, string message, string OrderID, DataTable dtAddon)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr;

            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderConfirmationMailAddon.html");

            string strdata = ""; string msg = "";
            string TITLE = title;

            while (!sr.EndOfStream)
            {
                double totalprice = Convert.ToDouble(dtAddon.Rows[0]["ServiceCharge"].ToString());
                DateTime servicedatetime = Convert.ToDateTime(Convert.ToDateTime(dtAddon.Rows[0]["Service_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtAddon.Rows[0]["Service_time"].ToString()).ToShortTimeString());
                DataTable dtsizetype = new PostOrderController().GetSizeTypedesc(dtAddon.Rows[0]["SizeTypeCode"].ToString());

                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("ORDERID", dtAddon.Rows[0]["Transaction_id"].ToString());
                strdata = strdata.Replace("ADDON_DATETIME", servicedatetime.ToString("dd MMM yyyy hh:mm tt"));
                strdata = strdata.Replace("ORDERADDRESS", dtAddon.Rows[0]["address"].ToString());
                strdata = strdata.Replace("TOTALCOST", totalprice.ToString());

                if (dtsizetype != null)
                    strdata = strdata.Replace("SIZETYPECODE", dtsizetype.Rows[0]["SizeTypeDesc"].ToString());

                if (dtAddon.Rows[0]["ServiceTypeCode"].ToString() == Constant.PAINTING_SERIVCES)
                {
                    strdata = strdata.Replace("SERVICETYPECODE", "Move Out Painting");
                    TITLE = TITLE.Replace("SERVICETYPECODE", "Painting");
                }

                if (dtAddon.Rows[0]["ServiceTypeCode"].ToString() == Constant.CLEANING_SERIVCES)
                {
                    strdata = strdata.Replace("SERVICETYPECODE", "Cleaning");
                    TITLE = TITLE.Replace("SERVICETYPECODE", "Cleaning");
                }
                if (dtAddon.Rows[0]["ServiceTypeCode"].ToString() == Constant.PESTCONTROL_SERIVCES)
                {
                    strdata = strdata.Replace("SERVICETYPECODE", "Pest Control");
                    TITLE = TITLE.Replace("SERVICETYPECODE", "Pest Control");
                }
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

        [HttpGet]
        public string GenerateDubiGoodsOrderConfirmationMail(string mailto, string title, string UserName, string OrderID, DataTable dtorder)//, string Packingrate)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";

            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\DubiOrderConfirmationMailGoods.html");

            string strdata = ""; string msg = "";
            string TITLE = title;

            while (!sr.EndOfStream)
            {
                double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());

                strdata = sr.ReadToEnd();
                strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
                strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
                strdata = strdata.Replace("USERNAME", UserName);
                strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["Transaction_id"].ToString());
                strdata = strdata.Replace("PICKUPDATE", Convert.ToDateTime(dtorder.Rows[0]["pickup_date"].ToString()).ToString("dd MMM yyyy"));
                strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
                strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
                strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
                strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
                strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
                strdata = strdata.Replace("MOVINGTYPE", "Goods");
                strdata = strdata.Replace("ITEMDESC", dtorder.Rows[0]["item_desc"].ToString());


                string SOURCELAT = dtorder.Rows[0]["inquiry_source_lat"].ToString();
                string SOURCELONGS = dtorder.Rows[0]["inquiry_source_lng"].ToString();
                string DESTLAT = dtorder.Rows[0]["inquiry_destionation_lat"].ToString();
                string DESTILONGS = dtorder.Rows[0]["inquiry_destionation_lng"].ToString();
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
    }
}


