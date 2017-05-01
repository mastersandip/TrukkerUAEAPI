using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace trukkerUAE.RegistrationForms
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string str2 = sha256_hash("PASSaccess_code=s31bpM1ebfNnwqoamount=1000command=AUTHORIZATIONcurrency=USDcustomer_email=test@payfort.comlanguage=enmerchant_identifier=FD1Ptqmerchant_reference=s2b3rj1vrjrhc1xPASS");

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