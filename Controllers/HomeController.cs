using BLL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace trukkerUAE.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //string[] arrdsf = new string[] { "524693690", "555791229" };
            //foreach (var item in arrdsf)
            //{
            //    new EMail().SendOtpToUserMobileNoUAE(" hellooo...Message from sandip for test You have Assign Order on '19-09-2016 09:00:00 AM' from 'AstroLabs Dubai - Jumeirah Lakes Towers - Dubai - United Arab Emirates' To 'Hotel ibis Styles Dragon Mart Dubai - Dubai - United Arab Emirates' ", item);
            //}


            string access_code = "";
            string status = "";
            string eci = "";
            string response_code = "";
            string customer_email = "";
            string customer_ip = "";
            string currency = "";
            string merchant_reference = "";
            string amount = "";
            string response_message = "";
            string command = "";
            string language = "";
            string signature = "";
            string merchant_identifier = "";
            string PassPrefix = "kjopohgfts868h";
            string service_command = "";

            if (Request["access_code"] != null) access_code = Request["access_code"].ToString();
            if (Request["amount"] != null) amount += Request["amount"].ToString();
            if (Request["command"] != null) command += Request["command"].ToString();
            if (Request["currency"] != null) currency += Request["currency"].ToString();
            if (Request["customer_email"] != null) customer_email += Request["customer_email"].ToString();
            if (Request["customer_ip"] != null) customer_ip += Request["customer_ip"].ToString();
            if (Request["eci"] != null) eci += Request["eci"].ToString();
            if (Request["language"] != null) language += Request["language"].ToString();
            if (Request["merchant_identifier"] != null) merchant_identifier += Request["merchant_identifier"].ToString();
            if (Request["merchant_reference"] != null) merchant_reference += Request["merchant_reference"].ToString();
            if (Request["response_code"] != null) response_code += Request["response_code"].ToString();
            if (Request["response_message"] != null) response_message += Request["response_message"].ToString();
            if (Request["status"] != null) status += Request["status"].ToString();
            if (Request["service_command"] != null) service_command += Request["service_command"].ToString();
            if (Request["signature"] != null) signature += Request["signature"].ToString();

            string strSignature = sha256_hash(PassPrefix + "access_code=" + access_code +
                                                            "amount=" + amount +
                                                            "command=" + command +
                                                            "currency=" + currency +
                                                            "customer_email=" + customer_email +
                                                            "customer_ip=" + customer_ip +
                                                            "eci=" + eci +
                                                            "language=" + language +
                                                            "merchant_identifier=" + merchant_identifier +
                                                            "merchant_reference=" + merchant_reference +
                                                            "response_code=" + response_code +
                                                            "response_message=" + response_message +
                                                            "status=" + status + PassPrefix);

            string strSignature2 = sha256_hash(PassPrefix + "access_code=" + access_code +
                                                           "service_command=" + service_command +
                                                           "language=" + language +
                                                           "merchant_identifier=" + merchant_identifier +
                                                           "merchant_reference=" + merchant_reference +
                                                           "response_code=" + response_code +
                                                           "response_message=" + response_message +
                                                           "status=" + status + PassPrefix);

            return View();
        }

        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
