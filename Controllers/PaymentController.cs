using BLL.Master;
using BLL.Utilities;
using Start.Net;
using Start.Net.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using trukkerUAE.Classes;
using trukkerUAE.XSD;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using trukkerUAE.BLL.Master;
using Newtonsoft.Json;
using trukkerUAE.Models;
using TrkrLite.Controllers;
using System.IO;
using System.Web;
using System.Configuration;

namespace trukkerUAE.Controllers
{
    public class PaymentController : ServerBase
    {

        [HttpGet]
        public string GetOrderPaymentsDetaislById(string inqno)
        {

            StringBuilder Sb = new StringBuilder();
            DataTable dtPostLoadOrders = new DataTable();

            Sb.Append(@" select * from payment_rcpt_dtl  left join payment_rcpt_hdr on payment_rcpt_hdr.payment_rcpt_no =  payment_rcpt_dtl.payment_rcpt_no 
                      where load_inquiry_no = @inqno order by  payment_rcpt_dtl.created_date DESC ");


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = Sb.ToString();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqno", DbType.String, inqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));// + ',' + SendReceiveJSon.GetJson(dtdriver) + ',' + SendReceiveJSon.GetJson(dttruck));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Details found");
        }

        [HttpPost]
        public String GenerateSignature([FromBody]JObject jobj)
        {
            //merchant id='UIxcLVrJ'
            //Access Code = 'gUErE32CkOPj2QGLqL97'
            //SHA Request Phrase = 'jgjsgcyfy6rfhkf'
            string access_code = jobj["access_code"].ToString();
            string amount = jobj["amount"].ToString();
            string command = jobj["command"].ToString();
            string currency = jobj["currency"].ToString();
            string customer_email = jobj["customer_email"].ToString();
            string language = jobj["language"].ToString();
            string merchant_identifier = jobj["merchant_identifier"].ToString();
            string merchant_reference = jobj["merchant_reference"].ToString();
            string PassPrefix = jobj["PassPrefix"].ToString();
            string ReturnURL = jobj["return_url"].ToString();

            string strSignature = sha256_hash(PassPrefix + "access_code=" + access_code + "amount=" + amount + "command=" + command + "currency=" + currency + "customer_email=" + customer_email + "language=" + language + "merchant_identifier=" + merchant_identifier + "merchant_reference=" + merchant_reference + "return_url=" + ReturnURL + PassPrefix);

            return BLGeneralUtil.return_ajax_string("1", strSignature);
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

        [HttpPost]
        public String GenerateCustomPaymentLink([FromBody]paymentdetails ObjPay)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = ""; string status = "";

            ServerLog.Log(ObjPay.ToString());
            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(ObjPay);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(ObjPay.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();

            DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

            DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();

            try
            {


                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                {
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                string Paymentlink = PaymentUrl + "?Pid=" + BLGeneralUtil.Encrypt(DocNo);

                dspayment.EnforceConstraints = false;
                dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                dspayment.order_paymentdetails[0].paymentLink = Paymentlink;
                dspayment.order_paymentdetails[0].status = "1111";
                dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;

                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }



                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 1;

                return BLGeneralUtil.return_ajax_string("1", "Link Generate Sucessfully");

            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }

        }

        [HttpGet]
        public string GetCustomPaymentDetails(string Trnid)
        {
            string trk = ""; string TrId = "";
            if (Trnid != "" && Trnid.ToString() != string.Empty)
            {
                TrId = BLGeneralUtil.Decrypt(Trnid);
            }

            String query1 = "";
            DataTable dtTrnDtl = new DataTable();

            query1 = " select user_mst.email_id,order_paymentdetails.status as linkStatus,* from order_paymentdetails left join orders on orders.load_inquiry_no=order_paymentdetails.load_inquiry_no left join user_mst on orders.shipper_id=user_mst.unique_id  where Transaction_id = @TrnID ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnID ", DbType.String, TrId));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTrnDtl = ds.Tables[0];
            }

            if (dtTrnDtl != null && dtTrnDtl.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTrnDtl));
            else
                return BLGeneralUtil.return_ajax_string("0", "Payment Details Not found ");

        }

        [HttpGet]
        public string GetPaymentLink(string inqno)
        {
            String query1 = "";
            DataTable dtTrnDtl = new DataTable();


            query1 = @"select user_mst.email_id,user_mst.first_name as user_name,order_paymentdetails.* from order_paymentdetails 
                       left join orders on orders.load_inquiry_no=order_paymentdetails.load_inquiry_no  
                       left join user_mst on orders.shipper_id = user_mst.unique_id where   order_paymentdetails.load_inquiry_no=@inqno and  order_paymentdetails.created_by='admin'  ";

            //query1 = " select *  FROM  order_paymentdetails where load_inquiry_no=@inqno  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inqno", DbType.String, inqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTrnDtl = ds.Tables[0];
            }

            if (dtTrnDtl != null && dtTrnDtl.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTrnDtl));
            else
                return BLGeneralUtil.return_ajax_string("0", "Payment Details Not found ");

        }

        public DataTable GetOrderPaymentDetails(string TrnId)
        {
            String query = " select order_paymentdetails.*,orders.* from order_paymentdetails left join orders on orders.load_inquiry_no=order_paymentdetails.load_inquiry_no  where Transaction_id= @TrnID ";

            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnID ", DbType.String, TrnId));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt = ds.Tables[0];
            }
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public DataTable Getorder_paymentdetailsbyTrnId(string TrnId)
        {
            String query = @"select * from order_paymentdetails where Transaction_id=@TrnId ";

            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnId ", DbType.String, TrnId));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt = ds.Tables[0];
            }
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }
        [HttpPost]
        public String SaveCustomPaymentDetails([FromBody]paymentdetails Objtrans)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = "";

            ServerLog.Log("SaveCustomPaymentDetails : " + Objtrans.ToString() + ")");
            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(Objtrans);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);
            DataTable dtorder_paymentdetails = new DataTable();
            decimal totalamount = Convert.ToDecimal(Objtrans.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();

            DataTable dtpostloadinq = new DataTable();
            if (Objtrans.Transaction_id != "")
            {
                dtorder_paymentdetails = Getorder_paymentdetailsbyTrnId(Objtrans.Transaction_id);
                if (dtorder_paymentdetails != null)
                {
                    if (dtorder_paymentdetails.Rows[0]["status"].ToString() == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                        return BLGeneralUtil.return_ajax_string("0", "Payment already done...");
                }
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Please provide order id");

            try
            {

                DataTable dttrndtl = new DataTable();
                DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (Objtrans.Transaction_id == "")
                {
                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                    {
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }
                    dspayment.EnforceConstraints = false;
                    dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                    dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                    dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                }
                else
                {
                    dttrndtl = GetOrderPaymentDetails(Objtrans.Transaction_id);
                    dspayment.EnforceConstraints = false;
                    dspayment.order_paymentdetails.ImportRow(dttrndtl.Rows[0]);
                    dspayment.order_paymentdetails[0].Transaction_id = Objtrans.Transaction_id;
                    dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                    dspayment.order_paymentdetails[0].status = Objtrans.status;
                    dspayment.order_paymentdetails[0].amount = totalamount.ToString();
                    dspayment.order_paymentdetails[0].authorization_code = Objtrans.authorization_code;
                    dspayment.order_paymentdetails[0].card_number = Objtrans.card_number;
                    dspayment.order_paymentdetails[0].command = Objtrans.command;
                    dspayment.order_paymentdetails[0].currency = Objtrans.currency;
                    dspayment.order_paymentdetails[0].customer_email = Objtrans.customer_email;
                    dspayment.order_paymentdetails[0].customer_ip = Objtrans.customer_ip;
                    dspayment.order_paymentdetails[0].eci = Objtrans.eci;
                    dspayment.order_paymentdetails[0].expiry_date = Objtrans.expiry_date;
                    dspayment.order_paymentdetails[0].fort_id = Objtrans.fort_id;
                    dspayment.order_paymentdetails[0].language = Objtrans.language;
                    dspayment.order_paymentdetails[0].merchant_reference = Objtrans.merchant_reference;
                    dspayment.order_paymentdetails[0].merchant_identifier = Objtrans.merchant_identifier;
                    dspayment.order_paymentdetails[0].payment_option = Objtrans.payment_option;
                    dspayment.order_paymentdetails[0].response_code = Objtrans.response_code;
                    dspayment.order_paymentdetails[0].response_message = Objtrans.response_message;
                    dspayment.order_paymentdetails[0].sdk_token = Objtrans.sdk_token;
                    dspayment.order_paymentdetails[0].status = Objtrans.status;
                    dspayment.order_paymentdetails[0].token_name = Objtrans.token_name;
                    dspayment.order_paymentdetails[0].signature = Objtrans.signature;
                    //dspayment.order_paymentdetails[0].paymentLink = dttrndtl.Rows[0]["paymentLink"].ToString();

                }

                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }


                String load_inquiry_no = "";

                if (Objtrans.load_inquiry_no != "")
                    load_inquiry_no = Objtrans.load_inquiry_no;
                else
                    load_inquiry_no = Objtrans.load_inquiry_no;

                if (Objtrans.status == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                {
                    decimal Amount = Convert.ToDecimal(Objtrans.amount);
                    Amount = Amount / 100;
                    String bank_code = "";
                    String cheque_no = "";
                    DateTime? cheque_date = (DateTime?)null;
                    String payment_mode = "O";
                    DateTime payment_date = System.DateTime.UtcNow;
                    String remark = "";
                    String created_by = "ADMIN";
                    String created_host = "ADMIN";
                    String device_id = "ADMIN";
                    String device_type = "Bowser";
                    String payment_rcv_by = "";
                    String shipperid = "";
                    string Message = "";
                    DS_orders dsorder = new DS_orders();
                    DataTable dtOrder = new PostOrderController().GetOrders(load_inquiry_no);


                    shipperid = dttrndtl.Rows[0]["shipper_id"].ToString();
                    dttrndtl.Columns.Add("adjusted_amt", typeof(String));
                    dttrndtl.Rows[0]["adjusted_amt"] = Amount.ToString();
                    dttrndtl.Columns.Add("db_cr", typeof(String));
                    dttrndtl.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                    dttrndtl.Columns.Add("match_unmatch", typeof(String));
                    dttrndtl.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                    dttrndtl.Columns.Add("payment_rcv_by", typeof(String));
                    dttrndtl.Rows[0]["payment_rcv_by"] = payment_rcv_by;

                    try
                    {
                        TruckerMaster objTrukker = new TruckerMaster();
                        if (dtOrder != null)
                        {
                            //decimal Order_Total_cost = Convert.ToDecimal(dtOrder.Rows[0]["Total_cost"].ToString());
                            //decimal Order_rem_amt_to_receive = Convert.ToDecimal(dtOrder.Rows[0]["rem_amt_to_receive"].ToString());
                            //decimal Order_TotalAddServiceCharge = Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceCharge"].ToString());
                            //decimal Order_TotalAddServiceDiscount = Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString());
                            //decimal Order_TotalCostWithoutAddon = Convert.ToDecimal(dtOrder.Rows[0]["Total_cost_without_addon"].ToString());

                            decimal Order_Total_cost = dtOrder.Rows[0]["Total_cost"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["Total_cost"].ToString());
                            decimal Order_rem_amt_to_receive = dtOrder.Rows[0]["rem_amt_to_receive"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["rem_amt_to_receive"].ToString());
                            decimal Order_TotalAddServiceCharge = dtOrder.Rows[0]["TotalAddServiceCharge"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceCharge"].ToString());
                            decimal Order_TotalAddServiceDiscount = dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString());
                            decimal Order_TotalCostWithoutAddon = dtOrder.Rows[0]["Total_cost_without_addon"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["Total_cost_without_addon"].ToString());


                            if (dtOrder.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {
                                DataTable dtaddonorder = new AdminController().SelectOrderAddonDetailsByInqID(load_inquiry_no, "");
                                if (dtaddonorder != null)
                                {
                                    dtaddonorder = dtaddonorder.Select("addon_by='U'").CopyToDataTable();
                                    if (dtaddonorder != null && dtaddonorder.Rows.Count > 0)
                                    {
                                        object sumObject; sumObject = dtaddonorder.Compute("Sum(rem_amt_to_receive)", "");
                                        decimal TotalAddonRemAmt = Convert.ToDecimal(sumObject);
                                        decimal OrderRemAmt = Order_rem_amt_to_receive - TotalAddonRemAmt;

                                        if (OrderRemAmt < Amount)
                                        {
                                            Decimal FinalRemAmt = Amount - OrderRemAmt;
                                            //if (FinalRemAmt < Amount)
                                            //{
                                            decimal New_rcv_amount = 0;
                                            decimal addon_rem_amt_to_receive = 0;

                                            for (int i = 0; i < dtaddonorder.Rows.Count; i++)
                                            {
                                                if (i == 0)
                                                    New_rcv_amount = FinalRemAmt;

                                                addon_rem_amt_to_receive = Convert.ToDecimal(dtaddonorder.Rows[i]["rem_amt_to_receive"].ToString());

                                                dsorder.EnforceConstraints = false;
                                                dsorder.order_AddonService_details.ImportRow(dtaddonorder.Rows[i]);

                                                if (New_rcv_amount < addon_rem_amt_to_receive)
                                                {
                                                    //dsorder.order_AddonService_details[i].rem_amt_to_receive = addon_rem_amt_to_receive - (addonChargewithdiscount - Order_Current_rem_amt);
                                                    dsorder.order_AddonService_details[i].rem_amt_to_receive = addon_rem_amt_to_receive - New_rcv_amount;
                                                    i = dtaddonorder.Rows.Count;
                                                }
                                                else
                                                {
                                                    dsorder.order_AddonService_details[i].rem_amt_to_receive = 0;
                                                    New_rcv_amount = New_rcv_amount - addon_rem_amt_to_receive;
                                                }

                                                dsorder.EnforceConstraints = true;
                                            }
                                            // }
                                        }
                                    }
                                }

                                objBLReturnObject = master.UpdateTables(dsorder.order_AddonService_details, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                        }


                        Byte SaveStatus = objTrukker.SavePaymentReceipt(ref DBCommand, payment_date, payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dttrndtl, created_by, created_host, device_id, device_type, ref Message);
                        if (SaveStatus != 1)
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", Message);
                        }


                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 1;
                        string shipperEmail = new PostOrderController().GetEmailByID(shipperid);
                        string shippername = new PostOrderController().GetUserdetailsByID(shipperid);
                        new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED" + Amount + " for Order Id (" + load_inquiry_no + ")", shippername, Amount.ToString(), load_inquiry_no);

                        return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);

                    }
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("1", Objtrans.response_message);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }
        }

        [HttpPost]
        public String SaveTransactionDetails([FromBody]paymentdetails Objtrans)
        {
            ServerLog.Log("SaveTransactionDetails(" + Convert.ToString(Objtrans) + ")");

            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = ""; string status = "";

            ServerLog.Log(Objtrans.ToString());
            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(Objtrans);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(Objtrans.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();
            DataTable dtaddonorder = new DataTable();
            DataTable dtpostloadinq = new DataTable();
            DataTable dtorder_paymentdetails = new DataTable();
            if (Objtrans.load_inquiry_no.Trim() != "")
            {
                dtpostloadinq = new LoginController().GetOrders(Objtrans.load_inquiry_no);
                if (dtpostloadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", "Order detail not found");

                dtaddonorder = new AdminController().SelectOrderAddonDetailsByInqID(Objtrans.load_inquiry_no, "");
                //dtorder_paymentdetails = Getorder_paymentdetailsbyTrnId(Objtrans.Transaction_id);
                //if (dtorder_paymentdetails != null)
                //{
                //    if (dtorder_paymentdetails.Rows[0]["status"].ToString() == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                //        return BLGeneralUtil.return_ajax_string("0", "Payment already done...");
                //}

            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Please provide order id");
            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                #region Create Transaction Entry

                DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }
                dspayment.EnforceConstraints = false;
                dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                #endregion

                DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();

                if (Objtrans.status == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                {
                    #region Update PostLoad Table

                    string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(Objtrans.load_inquiry_no);
                    string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(Objtrans.load_inquiry_no);
                    dspostload.EnforceConstraints = false;
                    dspostload.post_load_inquiry.ImportRow(dtpostloadinq.Rows[0]);
                    dspostload.post_load_inquiry[0].payment_status = Constant.FLAG_Y;
                    dspostload.post_load_inquiry[0].cbmlink = cbmlink;
                    dspostload.post_load_inquiry[0].IsDraft = Constant.Flag_No;
                    dspostload.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #endregion

                    #region Update Data in Order table

                    DS_orders dsorder = new DS_orders();
                    string OrdId = ""; //string strpromocode = "";
                    DataTable dt_ordersByinq = new PostOrderController().GetOrders(Objtrans.load_inquiry_no);
                    //if (dt_ordersByinq != null)
                    //    strpromocode = dt_ordersByinq.Rows[0]["coupon_code"].ToString();

                    if (dt_ordersByinq == null)
                    {
                        dsorder.EnforceConstraints = false;
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        dsorder.orders.ImportRow(dspostload.post_load_inquiry[0]);
                        dsorder.orders[0].order_id = OrdId;
                    }
                    else
                        dsorder.orders.ImportRow(dt_ordersByinq.Rows[0]);

                    dsorder.orders[0].trackurl = trakurl;
                    dsorder.orders[0].cbmlink = cbmlink;
                    dsorder.orders[0].active_flag = Constant.Flag_Yes;
                    dsorder.orders[0].payment_mode = Constant.PaymentModeONLINE;
                    //dsorder.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtpostloadinq.Rows[0]["rem_amt_to_receive"].ToString());
                    dsorder.orders[0].isassign_driver_truck = Constant.Flag_No;
                    dsorder.orders[0].isassign_mover = Constant.Flag_No;
                    dsorder.orders[0].IsCancel = Constant.Flag_No;
                    dsorder.orders[0].IsDraft = Constant.Flag_No;
                    dsorder.orders[0].created_date = System.DateTime.UtcNow;
                    dsorder.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #endregion

                    #region Insertbilling address

                    String Message = String.Empty;
                    if (Objtrans.Isupdatebillingadd != null)
                    {
                        if (Objtrans.Isupdatebillingadd == "Y")
                        {
                            try
                            {
                                DS_Shipper dS_shipper = new DS_Shipper();
                                string Msg = ""; string sr_no = "";
                                DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                row.billing_srno = sr_no;
                                row.billing_name = dtpostloadinq.Rows[0]["billing_name"].ToString();
                                row.shipper_id = dtpostloadinq.Rows[0]["shipper_id"].ToString();
                                row.billing_add = dtpostloadinq.Rows[0]["billing_add"].ToString();
                                row.active_flag = Constant.FLAG_Y;
                                row.source_full_add = dtpostloadinq.Rows[0]["source_full_add"].ToString();
                                row.destination_full_add = dtpostloadinq.Rows[0]["destination_full_add"].ToString();
                                row.order_type_flag = Constant.ORDERTYPECODEFORHOME;
                                row.created_by = tload[0].created_by;
                                row.created_date = System.DateTime.UtcNow;

                                dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

                                try
                                {
                                    dS_shipper.EnforceConstraints = true;
                                }
                                catch (ConstraintException ce)
                                {
                                    Message = ce.Message;
                                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }
                                catch (Exception ex)
                                {
                                    Message = ex.Message;
                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }

                                objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage.ToString());
                                }

                            }
                            catch (Exception ex)
                            {
                                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                Message = ex.Message;
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }
                        }
                    }
                    #endregion

                    #region Update Coupon Code

                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    decimal DiscountPrice = 0;
                    if (dtpostloadinq.Rows[0]["coupon_code"].ToString().Trim() != "")
                    {
                        decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                        Boolean B1 = objTruckerMaster.IsCouponValid(dtpostloadinq.Rows[0]["coupon_code"].ToString(), dtpostloadinq.Rows[0]["shipper_id"].ToString(), Objtrans.load_inquiry_no, Objtrans.load_inquiry_no, System.DateTime.UtcNow, dtpostloadinq.Rows[0]["order_type_flag"].ToString(), dtpostloadinq.Rows[0]["rate_type_flag"].ToString(), dtpostloadinq.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Msg);
                        if (B1)
                        {
                            decimal Total_cost = dtpostloadinq.Rows[0]["Total_cost_without_discount"].ToString() == "" ? 0 : Convert.ToDecimal(dtpostloadinq.Rows[0]["Total_cost_without_discount"].ToString());
                            if (flatdiscount != 0)
                                DiscountPrice = Math.Round(flatdiscount);
                            else if (PercentageDiscount != 0)
                                DiscountPrice = Total_cost * (PercentageDiscount / 100);

                            if (DiscountPrice != 0)
                            {
                                dtpostloadinq.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                            }

                            Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, dtpostloadinq.Rows[0]["coupon_code"].ToString(), dtpostloadinq.Rows[0]["shipper_id"].ToString(), dtpostloadinq.Rows[0]["load_inquiry_no"].ToString(), dtpostloadinq.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, Objtrans.created_by, Objtrans.created_host, Objtrans.device_id, Objtrans.device_type, ref Msg);
                            if (B2 != 1)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", Msg);
                            }
                        }
                        else
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", Msg);
                        }

                    }


                    #endregion

                    #region Create CBM Entry

                    DS_CBM objDsCbm = new DS_CBM();
                    Master objmaster = new Master(); string DocNtficID = "";

                    DataTable dtappid = new CBMController().GetAppIDbySizetype(dtpostloadinq.Rows[0]["SizeTypeCode"].ToString());

                    if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QHD", "", "", ref DocNtficID, ref message)) // New Driver Notification ID
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    }

                    objDsCbm.EnforceConstraints = false;

                    DS_CBM.quote_hdrRow tr = objDsCbm.quote_hdr.Newquote_hdrRow();
                    tr.quote_id = tload[0].load_inquiry_no;
                    tr.quote_hdr_id = DocNtficID;
                    tr.appartment_id = dtappid.Rows[0]["appartment_id"].ToString();
                    tr.room_type = dtappid.Rows[0]["appartment_desc"].ToString();
                    tr.customer_name = new PostOrderController().GetUserdetailsByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.customer_mobile = new PostOrderController().GetMobileNoByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.customer_email = new PostOrderController().GetEmailByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.total_cbm = 0;
                    tr.cbmlink = cbmlink;
                    tr.is_assign_to_order = Constant.FLAG_Y;
                    tr.is_create_on_order = Constant.FLAG_Y;
                    tr.StatusFlag = "D";
                    tr.created_date = System.DateTime.UtcNow;
                    tr.created_host = tload[0].created_host;
                    tr.created_by = tload[0].created_by;

                    objDsCbm.quote_hdr.Addquote_hdrRow(tr);
                    objDsCbm.quote_hdr.Rows[0].AcceptChanges();
                    objDsCbm.quote_hdr.Rows[0].SetAdded();

                    objDsCbm.EnforceConstraints = true;
                    objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_hdr, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }


                    #endregion

                    DataTable dt = dsorder.orders;

                    String load_inquiry_no = Objtrans.load_inquiry_no;
                    decimal Amount = Convert.ToDecimal(Objtrans.amount);
                    Amount = Amount / 100;
                    String bank_code = "";
                    String cheque_no = "";
                    DateTime? cheque_date = (DateTime?)null;
                    String payment_mode = "O";
                    DateTime payment_date = System.DateTime.UtcNow;
                    String remark = "";
                    String created_by = Objtrans.created_by;
                    String created_host = Objtrans.created_host;
                    String device_id = Objtrans.device_id;
                    String device_type = Objtrans.device_type;
                    String payment_rcv_by = "Admin";
                    String shipperid = "";


                    shipperid = dt.Rows[0]["shipper_id"].ToString();
                    dt.Columns.Add("adjusted_amt", typeof(String));
                    dt.Rows[0]["adjusted_amt"] = Amount.ToString();
                    dt.Columns.Add("db_cr", typeof(String));
                    dt.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                    dt.Columns.Add("match_unmatch", typeof(String));
                    dt.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                    dt.Columns.Add("payment_rcv_by", typeof(String));
                    dt.Rows[0]["payment_rcv_by"] = payment_rcv_by;


                    try
                    {
                        TruckerMaster objTrukker = new TruckerMaster();

                        Byte SaveStatus = objTrukker.SavePaymentReceipt(ref DBCommand, payment_date, payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dt, created_by, created_host, device_id, device_type, ref Message);
                        if (SaveStatus != 1)
                        {
                            ServerLog.Log(Message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", Message);

                        }
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);

                    }

                    dspostload.EnforceConstraints = false;
                    dspostload.post_load_inquiry[0].rem_amt_to_receive = 0;
                    dspostload.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;


                    if (dsorder.orders[0].order_type_flag == Constant.ORDERTYPECODEFORHOME)
                    {
                        if (dsorder.orders[0] != null)
                        {
                            ServerLog.Log("Update Addon In(" + SendReceiveJSon.GetJson(dsorder.orders) + ")");

                            if (dsorder.orders[0].IncludeAddonService == Constant.FLAG_Y)
                            {

                                if (dtaddonorder != null && dtaddonorder.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtaddonorder.Rows.Count; i++)
                                    {
                                        ServerLog.Log("Addoon In (" + dtaddonorder.Rows[i]["rem_amt_to_receive"].ToString() + ")");

                                        dsorder.EnforceConstraints = false;
                                        dsorder.order_AddonService_details.ImportRow(dtaddonorder.Rows[i]);
                                        dsorder.order_AddonService_details[i].rem_amt_to_receive = 0;
                                        dsorder.order_AddonService_details[i].payment_status = Constant.FLAG_Y;
                                        dsorder.order_AddonService_details[i].active_flag = Constant.FLAG_Y;
                                        dsorder.EnforceConstraints = true;

                                        ServerLog.Log("Addoon In (" + dsorder.order_AddonService_details[i].rem_amt_to_receive + ")");
                                    }
                                }
                            }
                        }

                        try
                        {

                            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            objBLReturnObject = master.UpdateTables(dsorder.order_AddonService_details, ref DBCommand);
                            ServerLog.Log("dsorder.order_AddonService_details(" + SendReceiveJSon.GetJson(dsorder.order_AddonService_details) + ")");
                            ServerLog.Log("dsorder.order_AddonService_details(" + objBLReturnObject.ServerMessage + ")");
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;


                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("order_AddonService_details(" + ex.Message + ")");
                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        }

                    }

                    try
                    {
                        PostOrderController objPostOrder = new PostOrderController();

                        DateTime dubaiTime = objPostOrder.DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                        string shipperEmail = objPostOrder.GetEmailByID(dt.Rows[0]["shipper_id"].ToString());
                        string shippername = objPostOrder.GetUserdetailsByID(dt.Rows[0]["shipper_id"].ToString());

                        string Msg = " Thank you..Your order from  " + dt.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt.Rows[0]["load_inquiry_no"].ToString();
                        new EMail().SendOtpToUserMobileNoUAE(Msg, objPostOrder.GetMobileNoByID(dtpostloadinq.Rows[0]["shipper_id"].ToString()));

                        string MsgMailbody = "Thank you..Your order from  " + dt.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt.Rows[0]["load_inquiry_no"].ToString();

                        ServerLog.Log(shipperEmail);

                        if (dt.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        {
                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderConfirmationMailHireTruck(shipperEmail, " Your Order is confirmed (Order ID: " + Objtrans.load_inquiry_no + ")", shippername, MsgMailbody, Objtrans.load_inquiry_no, dt, dt.Rows[0]["Hiretruck_TotalFuelRate"].ToString()));
                            if (result["status"].ToString() == "0")
                                ServerLog.Log("Error Sending Activation Email");
                        }
                        else
                        {
                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(shipperEmail, " Your Order is confirmed (Order ID: " + Objtrans.load_inquiry_no + ")", shippername, MsgMailbody, Objtrans.load_inquiry_no, dt, dt.Rows[0]["TotalPackingCharge"].ToString()));
                            if (result["status"].ToString() == "0")
                                ServerLog.Log("Error Sending Activation Email");
                        }


                        try
                        {
                            Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt);
                        }
                        catch (Exception ex)
                        {
                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        ServerLog.Log("Error in send OTP on Completation ");
                    }

                    return BLGeneralUtil.return_ajax_statusdata("1", "Thank you...Payment done sucessfully", SendReceiveJSon.GetJson(dt));

                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("0", Objtrans.response_message);

                }



            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }
        }

        [HttpPost]
        public String SaveOrderCashPaymentDetails([FromBody]JObject jobj)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DS_orders dsorder = new DS_orders();
            string DocNo = ""; string message = ""; string status = ""; string shipperid = "";

            ServerLog.MgmtExceptionLog("SaveOrderPaymentDetails(" + Convert.ToString(jobj) + ")");
            String load_inquiry_no = jobj["load_inquiry_no"].ToString();
            decimal Amount = Convert.ToDecimal(jobj["amount"].ToString());
            String bank_code = jobj["bank_code"].ToString();
            String cheque_no = jobj["cheque_no"].ToString();
            DateTime? cheque_date = jobj["cheque_date"].ToString().Trim() != "" ? Convert.ToDateTime(jobj["cheque_date"].ToString()) : (DateTime?)null;
            String payment_mode = jobj["payment_mode"].ToString();
            String payment_date = jobj["payment_date"].ToString();
            String remark = jobj["remark"].ToString();
            String created_by = jobj["created_by"].ToString();
            String created_host = jobj["created_host"].ToString();
            String device_id = jobj["device_id"].ToString();
            String device_type = jobj["device_type"].ToString();
            String payment_rcv_by = jobj["payment_rcv_by"].ToString();

            // DataTable dtOrder = new PostOrderController().Getorderdr(load_inquiry_no);
            DataTable dtOrder = new PostOrderController().GetOrders(load_inquiry_no);
            if (dtOrder == null)
                return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            else
            {
                shipperid = dtOrder.Rows[0]["shipper_id"].ToString();
                dtOrder.Columns.Add("adjusted_amt", typeof(String));
                dtOrder.Rows[0]["adjusted_amt"] = Amount.ToString();
                dtOrder.Columns.Add("db_cr", typeof(String));
                dtOrder.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                dtOrder.Columns.Add("match_unmatch", typeof(String));
                dtOrder.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                dtOrder.Columns.Add("payment_rcv_by", typeof(String));
                dtOrder.Rows[0]["payment_rcv_by"] = payment_rcv_by;
            }

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                String Message = String.Empty;
                TruckerMaster objTrukker = new TruckerMaster();

                if (dtOrder != null)
                {
                    //decimal Order_Total_cost = Convert.ToDecimal(dtOrder.Rows[0]["Total_cost"].ToString());
                    //decimal Order_rem_amt_to_receive = Convert.ToDecimal(dtOrder.Rows[0]["rem_amt_to_receive"].ToString());
                    //decimal Order_TotalAddServiceCharge = Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceCharge"].ToString());
                    //decimal Order_TotalAddServiceDiscount = Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString());
                    //decimal Order_TotalCostWithoutAddon = Convert.ToDecimal(dtOrder.Rows[0]["Total_cost_without_addon"].ToString());

                    decimal Order_Total_cost = dtOrder.Rows[0]["Total_cost"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["Total_cost"].ToString());
                    decimal Order_rem_amt_to_receive = dtOrder.Rows[0]["rem_amt_to_receive"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["rem_amt_to_receive"].ToString());
                    decimal Order_TotalAddServiceCharge = dtOrder.Rows[0]["TotalAddServiceCharge"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceCharge"].ToString());
                    decimal Order_TotalAddServiceDiscount = dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["TotalAddServiceDiscount"].ToString());
                    //   decimal Order_TotalCostWithoutAddon = dtOrder.Rows[0]["Total_cost_without_addon"].ToString() == "" ? 0 : Convert.ToDecimal(dtOrder.Rows[0]["Total_cost_without_addon"].ToString());


                    if (dtOrder.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
                    {
                        if (dtOrder.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                        {
                            DataTable dtaddonorder = new AdminController().SelectOrderAddonDetailsByInqID(load_inquiry_no, "");
                            if (dtaddonorder != null)
                            {
                                dtaddonorder = dtaddonorder.Select("addon_by='U'").CopyToDataTable();
                                if (dtaddonorder != null && dtaddonorder.Rows.Count > 0)
                                {
                                    object sumObject; sumObject = dtaddonorder.Compute("Sum(rem_amt_to_receive)", "");
                                    decimal TotalAddonRemAmt = Convert.ToDecimal(sumObject);
                                    decimal OrderRemAmt = Order_rem_amt_to_receive - TotalAddonRemAmt;



                                    if (OrderRemAmt < Amount)
                                    {
                                        Decimal FinalRemAmt = Amount - OrderRemAmt;

                                        decimal New_rcv_amount = 0;
                                        decimal addon_rem_amt_to_receive = 0;

                                        for (int i = 0; i < dtaddonorder.Rows.Count; i++)
                                        {
                                            if (i == 0)
                                                New_rcv_amount = FinalRemAmt;

                                            addon_rem_amt_to_receive = Convert.ToDecimal(dtaddonorder.Rows[i]["rem_amt_to_receive"].ToString());

                                            dsorder.EnforceConstraints = false;
                                            dsorder.order_AddonService_details.ImportRow(dtaddonorder.Rows[i]);

                                            if (New_rcv_amount < addon_rem_amt_to_receive)
                                            {
                                                dsorder.order_AddonService_details[i].rem_amt_to_receive = addon_rem_amt_to_receive - New_rcv_amount;
                                                i = dtaddonorder.Rows.Count;
                                            }
                                            else
                                            {
                                                dsorder.order_AddonService_details[i].rem_amt_to_receive = 0;
                                                New_rcv_amount = New_rcv_amount - addon_rem_amt_to_receive;
                                            }

                                            dsorder.EnforceConstraints = true;
                                        }
                                    }
                                }

                                objBLReturnObject = master.UpdateTables(dsorder.order_AddonService_details, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                        }
                    }
                }


                Byte SaveStatus = objTrukker.SavePaymentReceipt(ref DBCommand, Convert.ToDateTime(payment_date), payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dtOrder, created_by, created_host, device_id, device_type, ref Message);
                if (SaveStatus != 1)
                {
                    ServerLog.Log(Message);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", Message);

                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    string shipperEmail = new PostOrderController().GetEmailByID(shipperid);
                    string shippername = new PostOrderController().GetUserdetailsByID(shipperid);
                    new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED " + Amount + " for Order Id (" + load_inquiry_no + ")", shippername, jobj["amount"].ToString(), load_inquiry_no);
                    return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
                }
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }

        [HttpPost]
        public String SaveAdditionalServiceChargeDetails([FromBody]JObject jobj)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DS_orders dsorder = new DS_orders();
            string DocNo = ""; string message = ""; string status = ""; string shipperid = "";

            ServerLog.MgmtExceptionLog("SaveOrderPaymentDetails(" + Convert.ToString(jobj) + ")");
            String load_inquiry_no = jobj["load_inquiry_no"].ToString();
            decimal Amount = Convert.ToDecimal(jobj["amount"].ToString());
            String bank_code = jobj["bank_code"].ToString();
            String cheque_no = jobj["cheque_no"].ToString();
            DateTime? cheque_date = jobj["cheque_date"].ToString().Trim() != "" ? Convert.ToDateTime(jobj["cheque_date"].ToString()) : (DateTime?)null;
            String payment_mode = jobj["payment_mode"].ToString();
            String payment_date = jobj["payment_date"].ToString();
            String remark = jobj["remark"].ToString();
            String created_by = jobj["created_by"].ToString();
            String created_host = jobj["created_host"].ToString();
            String device_id = jobj["device_id"].ToString();
            String device_type = jobj["device_type"].ToString();
            String payment_rcv_by = jobj["payment_rcv_by"].ToString();

            // DataTable dtOrder = new PostOrderController().Getorderdr(load_inquiry_no);
            DataTable dtOrder = new PostOrderController().GetOrders(load_inquiry_no);
            if (dtOrder == null)
                return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            else
            {
                shipperid = dtOrder.Rows[0]["shipper_id"].ToString();
                dtOrder.Columns.Add("adjusted_amt", typeof(String));
                dtOrder.Rows[0]["adjusted_amt"] = Amount.ToString();
                dtOrder.Columns.Add("db_cr", typeof(String));
                dtOrder.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                dtOrder.Columns.Add("match_unmatch", typeof(String));
                dtOrder.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                dtOrder.Columns.Add("payment_rcv_by", typeof(String));
                dtOrder.Rows[0]["payment_rcv_by"] = payment_rcv_by;
            }

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                String Message = String.Empty;
                TruckerMaster objTrukker = new TruckerMaster();

                Byte SaveStatus = objTrukker.SavePaymentReceipt(ref DBCommand, Convert.ToDateTime(payment_date), payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dtOrder, created_by, created_host, device_id, device_type, ref Message);
                if (SaveStatus != 1)
                {
                    ServerLog.Log(Message);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", Message);

                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    string shipperEmail = new PostOrderController().GetEmailByID(shipperid);
                    string shippername = new PostOrderController().GetUserdetailsByID(shipperid);
                    // new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED " + Amount + " for Order Id (" + load_inquiry_no + ")", shippername, jobj["amount"].ToString(), load_inquiry_no);
                    return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
                }
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }


        public DataTable GetOrderPaymentLinkDetails(string TrnId)
        {
            String query = @" select user_mst.email_id,user_mst.first_name as user_name,order_paymentdetails.*,orders.* from order_paymentdetails 
                            left join orders on orders.load_inquiry_no=order_paymentdetails.load_inquiry_no  
                            left join user_mst on orders.shipper_id = user_mst.unique_id where Transaction_id= @TrnID ";

            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnID ", DbType.String, TrnId));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt = ds.Tables[0];
            }
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        [HttpGet]
        public string SendOrderPaymentLinkMail(string transactionid, string emailId)
        {
            try
            {
                StreamReader sr; string LINK = "";
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_SendADDONGeneratedLink.html");
                string strdata = ""; string msg = "";
                string SubjectOrderOnlinePayment = ConfigurationManager.AppSettings["SUBORDERCUSTOMPAYMENT"];

                DataTable dtOrderDtl = GetOrderPaymentLinkDetails(transactionid);

                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();

                    if (dtOrderDtl.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
                        strdata = strdata.Replace("SERVICES", " Moving Home ");

                    if (dtOrderDtl.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        strdata = strdata.Replace("SERVICES", " Moving Goods ");

                    if (dtOrderDtl.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        strdata = strdata.Replace("SERVICES", " Hire Truck ");

                    strdata = strdata.Replace("UserName", dtOrderDtl.Rows[0]["user_name"].ToString());
                    strdata = strdata.Replace("ADDONLINK", dtOrderDtl.Rows[0]["paymentlink"].ToString());
                    strdata = strdata.Replace("SERVICECHARGE", dtOrderDtl.Rows[0]["amount"].ToString());
                }
                sr.Close();


                msg = "";
                EMail objemail = new EMail();
                Boolean bl = objemail.SendMail(emailId, strdata, dtOrderDtl.Rows[0]["user_name"].ToString() + "," + SubjectOrderOnlinePayment + dtOrderDtl.Rows[0]["amount"].ToString(), ref msg, "CONTACT", "SENDFROM");
                if (!bl)
                    return BLGeneralUtil.return_ajax_string("0", msg);
                else
                    return BLGeneralUtil.return_ajax_string("1", "Mail Send Successfully");

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.StackTrace + "error in send CBM email" + ex.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }

        #region Addon Admin service

        [HttpPost]
        public String GenerateAddonPaymentLink([FromBody]paymentdetails ObjPay)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = ""; string status = "";

            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(ObjPay);
            ServerLog.Log("GenerateAddonPaymentLink(" + JsonConvert.SerializeObject(tload) + ")");
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(ObjPay.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();

            DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

            DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();

            try
            {

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                {
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                string Paymentlink = AddonPaymentUrl + "?Pid=" + BLGeneralUtil.Encrypt(DocNo);

                dspayment.EnforceConstraints = false;
                dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                dspayment.order_paymentdetails[0].paymentLink = Paymentlink;
                dspayment.order_paymentdetails[0].status = "1111";
                dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;

                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 1;

                return BLGeneralUtil.return_ajax_string("1", "Link Generate Sucessfully");

            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }

        }

        //[HttpPost]
        //public String GenerateAddonPaymentLink([FromBody]JObject jObj)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    DS_orders ds_order = new DS_orders();
        //    List<order_AddonService_details> lstaddon = new List<order_AddonService_details>();
        //    DataTable dtAddonParameter = new DataTable(); DataTable dt_load = new DataTable();
        //    string DocNo = ""; string message = "";
        //    DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

        //    if (jObj["Addon"] != null)
        //    {
        //        lstaddon = jObj["Addon"].ToObject<List<order_AddonService_details>>();
        //        dtAddonParameter = BLGeneralUtil.ToDataTable(lstaddon);
        //        dtAddonParameter = BLGeneralUtil.CheckDateTime(dtAddonParameter);

        //        ServerLog.Log("GenerateAddonPaymentLink(" + JsonConvert.SerializeObject(lstaddon) + ")");
        //    }

        //    DataTable dtaddondetails = new AdminController().SelectOrderAddonDetailsByTrnID(lstaddon[0].Transaction_id);
        //    if (dtaddondetails != null)
        //    {
        //        decimal totalamount = Convert.ToDecimal(lstaddon[0].ServiceCharge);
        //        totalamount = totalamount / 100;
        //        dtaddondetails.Rows[0]["ServiceCharge"] = totalamount.ToString();

        //        DBConnection.Open();
        //        DBCommand.Transaction = DBConnection.BeginTransaction();

        //        try
        //        {
        //            string Paymentlink = AddonPaymentUrl + "?Pid=" + BLGeneralUtil.Encrypt(lstaddon[0].Transaction_id);

        //            ds_order.EnforceConstraints = false;
        //            ds_order.order_AddonService_details.ImportRow(dtaddondetails.Rows[0]);
        //            ds_order.order_AddonService_details[0].payment_link = Paymentlink;
        //            ds_order.EnforceConstraints = true;

        //            objBLReturnObject = master.UpdateTables(ds_order.order_AddonService_details, ref DBCommand);
        //            if (objBLReturnObject.ExecutionStatus == 2)
        //            {
        //                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 2;
        //                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //            }

        //            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
        //            {
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                return BLGeneralUtil.return_ajax_string("0", message);
        //            }

        //            dspayment.EnforceConstraints = false;
        //            dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
        //            dspayment.order_paymentdetails[0].Transaction_id = DocNo;
        //            dspayment.order_paymentdetails[0].paymentLink = Paymentlink;
        //            dspayment.order_paymentdetails[0].status = "1111";
        //            dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;

        //            dspayment.order_paymentdetails.Rows[0].AcceptChanges();
        //            dspayment.order_paymentdetails.Rows[0].SetAdded();

        //            objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
        //            if (objBLReturnObject.ExecutionStatus == 2)
        //            {
        //                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 2;
        //                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //            }


        //            DBCommand.Transaction.Commit();
        //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //            objBLReturnObject.ExecutionStatus = 1;

        //            return BLGeneralUtil.return_ajax_string("1", "Link Generate Sucessfully");

        //        }
        //        catch (Exception ex)
        //        {
        //            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //            objBLReturnObject.ExecutionStatus = 2;
        //            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

        //        }



        //    }
        //    else
        //    {
        //        return BLGeneralUtil.return_ajax_string("0", "Details not found");
        //    }
        //}

        [HttpGet]
        public string GetAddonPaymentDetails(string Trnid)
        {
            string trk = ""; string TrId = "";
            if (Trnid != "" && Trnid.ToString() != string.Empty)
            {
                TrId = BLGeneralUtil.Decrypt(Trnid);
            }

            String query1 = "";
            DataTable dtTrnDtl = new DataTable();

            query1 = @"  select amount,order_paymentdetails.Transaction_id,order_paymentdetails.status as linkStatus,Agency_mst.agency_Name,SizeTypeMst.SizeTypeDesc,Subservice_maping_details.SubServiceTypeDesc,Services_mst.ServiceTypeDesc,
                                order_AddonService_details.Transaction_id as load_inquiry_no , order_AddonService_details.AddSerBaseDiscount, order_AddonService_details.ServiceTypeCode, order_AddonService_details.SizeTypeCode, order_AddonService_details.ServiceCharge,
                                order_AddonService_details.ServiceDiscount, order_AddonService_details.SubServiceTypeCode,user_mobileno, 
                                user_email, user_name, address, CelingRequired, NoofCleling, CONVERT(VARCHAR, order_AddonService_details.Service_date, 103) AS Service_date, CONVERT(VARCHAR, order_AddonService_details.Service_time, 108) 
                                AS Service_time, order_AddonService_details.NoofCleaners, order_AddonService_details.Notes, order_AddonService_details.status, order_AddonService_details.AgencyId, order_AddonService_details.rem_amt_to_receive,
                                order_AddonService_details.Payment_mode, order_AddonService_details.payment_status, order_AddonService_details.payment_link, order_AddonService_details.created_by, order_AddonService_details.created_date, 
                                order_AddonService_details.created_host, order_AddonService_details.device_id, order_AddonService_details.device_type, order_AddonService_details.modified_by, order_AddonService_details.modified_date, 
                                order_AddonService_details.modified_host, order_AddonService_details.modified_device_id, order_AddonService_details.modified_device_type
                                from order_paymentdetails
                                left join order_AddonService_details on order_AddonService_details.Transaction_id=order_paymentdetails.load_inquiry_no                                
                                left join Subservice_maping_details on Subservice_maping_details.SubServiceTypeCode=order_AddonService_details.SubServiceTypeCode
                                left join Services_mst on Services_mst.ServiceTypeCode=order_AddonService_details.ServiceTypeCode
                                left join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                                left join Agency_mst on Agency_mst.agency_id=order_AddonService_details.AgencyId 
                                where order_paymentdetails.Transaction_id=@TrnID ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnID ", DbType.String, TrId));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTrnDtl = ds.Tables[0];
            }

            if (dtTrnDtl != null && dtTrnDtl.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTrnDtl));
            else
                return BLGeneralUtil.return_ajax_string("0", "Payment Details Not found ");

        }

        [HttpGet]
        public string GetAddonPaymentsLink(string Trnid)
        {
            String query1 = "";
            DataTable dtTrnDtl = new DataTable();
            query1 = @"  select order_AddonService_details.user_email,*  FROM  order_paymentdetails 
                         left join order_AddonService_details on order_AddonService_details.Transaction_id=order_paymentdetails.load_inquiry_no
                        where order_paymentdetails.load_inquiry_no=@Trnid and  order_paymentdetails.created_by='admin'  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@Trnid", DbType.String, Trnid));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTrnDtl = ds.Tables[0];
            }

            if (dtTrnDtl != null && dtTrnDtl.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTrnDtl));
            else
                return BLGeneralUtil.return_ajax_string("0", "Payment Details Not found ");

        }

        [HttpPost]
        public String SaveAddonPaymentDetails([FromBody]paymentdetails Objtrans)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = "";
            DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();

            ServerLog.Log("SaveAddonPaymentDetails : " + Objtrans.ToString() + ")");

            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(Objtrans);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(Objtrans.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();

            //DataTable dtpostloadinq = new DataTable();
            //if (Objtrans.load_inquiry_no != "")
            //{
            //    dtpostloadinq = new LoginController().GetOrders(Objtrans.load_inquiry_no);
            //    if (dtpostloadinq == null)
            //        return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            //}
            //else
            //    return BLGeneralUtil.return_ajax_string("0", "Please provide order id");

            try
            {

                DataTable dttrndtl = new DataTable();
                DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (Objtrans.Transaction_id == "")
                {
                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                    {
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }
                    dspayment.EnforceConstraints = false;
                    dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                    dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                    dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                }
                else
                {
                    dttrndtl = GetOrderPaymentDetails(Objtrans.Transaction_id);
                    dspayment.EnforceConstraints = false;
                    dspayment.order_paymentdetails.ImportRow(dttrndtl.Rows[0]);
                    dspayment.order_paymentdetails[0].Transaction_id = Objtrans.Transaction_id;
                    dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                    dspayment.order_paymentdetails[0].status = Objtrans.status;
                    dspayment.order_paymentdetails[0].amount = totalamount.ToString();
                    dspayment.order_paymentdetails[0].authorization_code = Objtrans.authorization_code;
                    dspayment.order_paymentdetails[0].card_number = Objtrans.card_number;
                    dspayment.order_paymentdetails[0].command = Objtrans.command;
                    dspayment.order_paymentdetails[0].currency = Objtrans.currency;
                    dspayment.order_paymentdetails[0].customer_email = Objtrans.customer_email;
                    dspayment.order_paymentdetails[0].customer_ip = Objtrans.customer_ip;
                    dspayment.order_paymentdetails[0].eci = Objtrans.eci;
                    dspayment.order_paymentdetails[0].expiry_date = Objtrans.expiry_date;
                    dspayment.order_paymentdetails[0].fort_id = Objtrans.fort_id;
                    dspayment.order_paymentdetails[0].language = Objtrans.language;
                    dspayment.order_paymentdetails[0].merchant_reference = Objtrans.merchant_reference;
                    dspayment.order_paymentdetails[0].merchant_identifier = Objtrans.merchant_identifier;
                    dspayment.order_paymentdetails[0].payment_option = Objtrans.payment_option;
                    dspayment.order_paymentdetails[0].response_code = Objtrans.response_code;
                    dspayment.order_paymentdetails[0].response_message = Objtrans.response_message;
                    dspayment.order_paymentdetails[0].sdk_token = Objtrans.sdk_token;
                    dspayment.order_paymentdetails[0].status = Objtrans.status;
                    dspayment.order_paymentdetails[0].token_name = Objtrans.token_name;
                    dspayment.order_paymentdetails[0].signature = Objtrans.signature;

                }

                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                if (Objtrans.status == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                {
                    decimal Amount = Convert.ToDecimal(Objtrans.amount);
                    Amount = Amount / 100;
                    String bank_code = "";
                    String cheque_no = "";
                    DateTime? cheque_date = (DateTime?)null;
                    String payment_mode = "O";
                    DateTime payment_date = System.DateTime.UtcNow;
                    String remark = "";
                    String created_by = "ADMIN";
                    String created_host = "ADMIN";
                    String device_id = "ADMIN";
                    String device_type = "Bowser";
                    String payment_rcv_by = "";
                    String shipperid = "";
                    string Message = "";


                    shipperid = dttrndtl.Rows[0]["shipper_id"].ToString();
                    dttrndtl.Columns.Add("adjusted_amt", typeof(String));
                    dttrndtl.Rows[0]["adjusted_amt"] = Amount.ToString();
                    dttrndtl.Columns.Add("db_cr", typeof(String));
                    dttrndtl.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                    dttrndtl.Columns.Add("match_unmatch", typeof(String));
                    dttrndtl.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                    dttrndtl.Columns.Add("payment_rcv_by", typeof(String));
                    dttrndtl.Rows[0]["payment_rcv_by"] = payment_rcv_by;

                    try
                    {
                        DataTable dtAddondtl = new AdminController().SelectOrderAddonDetailsByTrnID(Objtrans.load_inquiry_no);
                        if (dtAddondtl != null)
                        {
                            DS_orders dsorder = new DS_orders();
                            if (dtAddondtl.Rows[0]["addon_by"].ToString() == "U")
                            {
                                DataTable dtorder = new PostOrderController().GetOrders(dtAddondtl.Rows[0]["load_inquiry_no"].ToString());
                                DataTable dtpostloadInq = new PostOrderController().GetPostloaddetailsByID(dtAddondtl.Rows[0]["load_inquiry_no"].ToString());

                                if (dtorder != null && dtorder.Rows.Count > 0)
                                {
                                    decimal prevCost = Convert.ToDecimal(dtAddondtl.Rows[0]["ServiceCharge"].ToString());
                                    decimal Total_cost = dtorder.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Total_cost"].ToString()) : 0;
                                    decimal rem_amt_to_receive = dtorder.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                    decimal TotalAddServiceDiscount = dtorder.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                    decimal TotalAddServiceCharge = dtorder.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                    dsorder.EnforceConstraints = false;
                                    dsorder.orders.ImportRow(dtorder.Rows[0]);
                                    //dsorder.orders[0].Total_cost = Total_cost - Amount;
                                    dsorder.orders[0].rem_amt_to_receive = rem_amt_to_receive - Amount;
                                    dsorder.EnforceConstraints = true;
                                }

                                if (dtpostloadInq != null && dtpostloadInq.Rows.Count > 0)
                                {
                                    decimal prevCost = Convert.ToDecimal(dtAddondtl.Rows[0]["ServiceCharge"].ToString());
                                    decimal Total_cost = dtpostloadInq.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["Total_cost"].ToString()) : 0;
                                    decimal rem_amt_to_receive = dtpostloadInq.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                    decimal TotalAddServiceDiscount = dtpostloadInq.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                    decimal TotalAddServiceCharge = dtpostloadInq.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                    dsorder.EnforceConstraints = false;
                                    dspostload.post_load_inquiry.ImportRow(dtpostloadInq.Rows[0]);
                                    dspostload.post_load_inquiry[0].rem_amt_to_receive = rem_amt_to_receive - Amount;
                                    dspostload.EnforceConstraints = true;
                                }

                                objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }

                            }
                        }

                        TruckerMaster objTrukker = new TruckerMaster();

                        Byte SaveStatus = objTrukker.SaveAddonPaymentReceipt(ref DBCommand, payment_date, payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dttrndtl, created_by, created_host, device_id, device_type, ref Message);
                        if (SaveStatus != 1)
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", Message);
                        }

                        objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }


                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 1;
                        //string shipperEmail = new PostOrderController().GetEmailByID(shipperid);
                        //string shippername = new PostOrderController().GetUserdetailsByID(shipperid);
                        // new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED" + Amount + " for Order Id (" + load_inquiry_no + ")", shippername, Amount.ToString(), load_inquiry_no);

                        return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);

                    }
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("1", Objtrans.response_message);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }
        }

        [HttpPost]
        public String SaveAddonCashPaymentDetails([FromBody]JObject jobj)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DS_orders dsorder = new DS_orders();
            DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();
            string DocNo = ""; string message = ""; string status = ""; string shipperid = "";

            ServerLog.MgmtExceptionLog("SaveOrderPaymentDetails(" + Convert.ToString(jobj) + ")");
            String load_inquiry_no = jobj["load_inquiry_no"].ToString();
            decimal Amount = Convert.ToDecimal(jobj["amount"].ToString());
            String bank_code = jobj["bank_code"].ToString();
            String cheque_no = jobj["cheque_no"].ToString();
            DateTime? cheque_date = jobj["cheque_date"].ToString().Trim() != "" ? Convert.ToDateTime(jobj["cheque_date"].ToString()) : (DateTime?)null;
            String payment_mode = jobj["payment_mode"].ToString();
            String payment_date = jobj["payment_date"].ToString();
            String remark = jobj["remark"].ToString();
            String created_by = jobj["created_by"].ToString();
            String created_host = jobj["created_host"].ToString();
            String device_id = jobj["device_id"].ToString();
            String device_type = jobj["device_type"].ToString();
            String payment_rcv_by = jobj["payment_rcv_by"].ToString();
            String load_inq_no = "";

            // DataTable dtOrder = new PostOrderController().Getorderdr(load_inquiry_no);
            DataTable dtOrderAddon = new AdminController().SelectOrderAddonDetailsByTrnID(load_inquiry_no);
            if (dtOrderAddon == null)
                return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            else
            {
                //shipperid = dtOrder.Rows[0]["shipper_id"].ToString();
                dtOrderAddon.Columns.Add("adjusted_amt", typeof(String));
                dtOrderAddon.Rows[0]["adjusted_amt"] = Amount.ToString();
                dtOrderAddon.Columns.Add("db_cr", typeof(String));
                dtOrderAddon.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                dtOrderAddon.Columns.Add("match_unmatch", typeof(String));
                dtOrderAddon.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                dtOrderAddon.Columns.Add("payment_rcv_by", typeof(String));
                dtOrderAddon.Rows[0]["payment_rcv_by"] = payment_rcv_by;
            }

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();


                if (dtOrderAddon != null)
                {
                    if (dtOrderAddon.Rows[0]["addon_by"].ToString() == "U")
                    {
                        #region Update Add by User Addons Details

                        DataTable dtorder = new PostOrderController().GetOrders(dtOrderAddon.Rows[0]["load_inquiry_no"].ToString());
                        DataTable dtpostloadInq = new PostOrderController().GetPostloaddetailsByID(dtOrderAddon.Rows[0]["load_inquiry_no"].ToString());

                        if (dtorder != null && dtorder.Rows.Count > 0)
                        {
                            decimal prevCost = Convert.ToDecimal(dtOrderAddon.Rows[0]["ServiceCharge"].ToString());
                            decimal Total_cost = dtorder.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Total_cost"].ToString()) : 0;
                            decimal rem_amt_to_receive = dtorder.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                            decimal TotalAddServiceDiscount = dtorder.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                            decimal TotalAddServiceCharge = dtorder.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                            dsorder.EnforceConstraints = false;
                            dsorder.orders.ImportRow(dtorder.Rows[0]);
                            //dsorder.orders[0].Total_cost = Total_cost - Amount;
                            dsorder.orders[0].rem_amt_to_receive = rem_amt_to_receive - Amount;
                            dsorder.EnforceConstraints = true;
                        }


                        if (dtpostloadInq != null && dtpostloadInq.Rows.Count > 0)
                        {
                            decimal prevCost = Convert.ToDecimal(dtOrderAddon.Rows[0]["ServiceCharge"].ToString());
                            decimal Total_cost = dtpostloadInq.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["Total_cost"].ToString()) : 0;
                            decimal rem_amt_to_receive = dtpostloadInq.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                            decimal TotalAddServiceDiscount = dtpostloadInq.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                            decimal TotalAddServiceCharge = dtpostloadInq.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtpostloadInq.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                            dsorder.EnforceConstraints = false;
                            dspostload.post_load_inquiry.ImportRow(dtpostloadInq.Rows[0]);
                            dspostload.post_load_inquiry[0].rem_amt_to_receive = rem_amt_to_receive - Amount;
                            dspostload.EnforceConstraints = true;
                        }


                        objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }

                        objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }

                        #endregion
                    }
                    else
                    {

                    }

                }

                String Message = String.Empty;
                TruckerMaster objTrukker = new TruckerMaster();

                dtOrderAddon.Rows[0]["load_inquiry_no"] = load_inquiry_no;

                Byte SaveStatus = objTrukker.SaveAddonPaymentReceipt(ref DBCommand, Convert.ToDateTime(payment_date), payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dtOrderAddon, created_by, created_host, device_id, device_type, ref Message);
                if (SaveStatus != 1)
                {
                    ServerLog.Log(Message);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", Message);
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    //string shipperEmail = new PostOrderController().GetEmailByID(shipperid);
                    //string shippername = new PostOrderController().GetUserdetailsByID(shipperid);
                    //new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED " + Amount + " for Order Id (" + load_inquiry_no + ")", shippername, jobj["amount"].ToString(), load_inquiry_no);
                    return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
                }


            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }

        #endregion

        public String Save_dubi_OrderOnlinePayment_Details([FromBody]paymentdetails Objtrans)
        {
            ServerLog.Log("Save_dubi_OrderOnlinePayment_Details(" + Objtrans + ")");

            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = ""; string status = "";

            ServerLog.Log(Objtrans.ToString());
            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(Objtrans);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(Objtrans.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();
            DataTable dtaddonorder = new DataTable();
            DataTable dtpostloadinq = new DataTable();
            if (Objtrans.load_inquiry_no.Trim() != "")
            {
                dtpostloadinq = new LoginController().GetOrders(Objtrans.load_inquiry_no);
                if (dtpostloadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Please provide order id");



            try
            {

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                #region Create Transaction Entry

                DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }
                dspayment.EnforceConstraints = false;
                dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                #endregion

                DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();

                if (Objtrans.status == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                {
                    #region Update PostLoad Table

                    string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(Objtrans.load_inquiry_no);
                    string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(Objtrans.load_inquiry_no);
                    dspostload.EnforceConstraints = false;
                    dspostload.post_load_inquiry.ImportRow(dtpostloadinq.Rows[0]);
                    dspostload.post_load_inquiry[0].payment_status = Constant.FLAG_Y;
                    dspostload.post_load_inquiry[0].cbmlink = cbmlink;
                    dspostload.post_load_inquiry[0].IsDraft = Constant.Flag_No;
                    dspostload.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #endregion

                    #region Update Data in Order table


                    DS_orders dsorder = new DS_orders();
                    string OrdId = ""; //string strpromocode = "";
                    DataTable dt_ordersByinq = new PostOrderController().GetOrders(Objtrans.load_inquiry_no);
                    //if (dt_ordersByinq != null)
                    //    strpromocode = dt_ordersByinq.Rows[0]["coupon_code"].ToString();

                    if (dt_ordersByinq == null)
                    {
                        dsorder.EnforceConstraints = false;
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        dsorder.orders.ImportRow(dspostload.post_load_inquiry[0]);
                        dsorder.orders[0].order_id = OrdId;
                        dsorder.orders[0].order_id = OrdId;
                    }
                    else
                        dsorder.orders.ImportRow(dt_ordersByinq.Rows[0]);

                    dsorder.orders[0].trackurl = trakurl;
                    dsorder.orders[0].cbmlink = cbmlink;
                    dsorder.orders[0].active_flag = Constant.Flag_Yes;
                    dsorder.orders[0].payment_mode = Constant.PaymentModeONLINE;
                    //dsorder.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtpostloadinq.Rows[0]["rem_amt_to_receive"].ToString());
                    dsorder.orders[0].isassign_driver_truck = Constant.Flag_No;
                    dsorder.orders[0].isassign_mover = Constant.Flag_No;
                    dsorder.orders[0].IsCancel = Constant.Flag_No;
                    dsorder.orders[0].IsDraft = Constant.Flag_No;
                    dsorder.orders[0].created_date = System.DateTime.UtcNow;
                    dsorder.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #endregion

                    #region Insertbilling address

                    String Message = String.Empty;
                    if (Objtrans.Isupdatebillingadd != null)
                    {
                        if (Objtrans.Isupdatebillingadd == "Y")
                        {
                            try
                            {
                                DS_Shipper dS_shipper = new DS_Shipper();
                                string Msg = ""; string sr_no = "";
                                DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                row.billing_srno = sr_no;
                                row.billing_name = dtpostloadinq.Rows[0]["billing_name"].ToString();
                                row.shipper_id = dtpostloadinq.Rows[0]["shipper_id"].ToString();
                                row.billing_add = dtpostloadinq.Rows[0]["billing_add"].ToString();
                                row.active_flag = Constant.FLAG_Y;
                                row.source_full_add = dtpostloadinq.Rows[0]["source_full_add"].ToString();
                                row.destination_full_add = dtpostloadinq.Rows[0]["destination_full_add"].ToString();
                                row.order_type_flag = Constant.ORDERTYPECODEFORHOME;
                                row.created_by = tload[0].created_by;
                                row.created_date = System.DateTime.UtcNow;

                                dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

                                try
                                {
                                    dS_shipper.EnforceConstraints = true;
                                }
                                catch (ConstraintException ce)
                                {
                                    Message = ce.Message;
                                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }
                                catch (Exception ex)
                                {
                                    Message = ex.Message;
                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }

                                objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage.ToString());
                                }

                            }
                            catch (Exception ex)
                            {
                                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                Message = ex.Message;
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }
                        }
                    }
                    #endregion

                    #region Update Coupon Code

                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    decimal DiscountPrice = 0;
                    if (dtpostloadinq.Rows[0]["coupon_code"].ToString().Trim() != "")
                    {
                        decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                        Boolean B1 = objTruckerMaster.IsCouponValid(dtpostloadinq.Rows[0]["coupon_code"].ToString(), dtpostloadinq.Rows[0]["shipper_id"].ToString(), Objtrans.load_inquiry_no, Objtrans.load_inquiry_no, System.DateTime.UtcNow, dtpostloadinq.Rows[0]["order_type_flag"].ToString(), dtpostloadinq.Rows[0]["rate_type_flag"].ToString(), dtpostloadinq.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Msg);
                        if (B1)
                        {
                            decimal Total_cost = Convert.ToDecimal(dtpostloadinq.Rows[0]["Total_cost_without_discount"].ToString());
                            if (flatdiscount != 0)
                                DiscountPrice = Math.Round(flatdiscount);
                            else if (PercentageDiscount != 0)
                                DiscountPrice = Total_cost * (PercentageDiscount / 100);

                            if (DiscountPrice != 0)
                            {
                                dtpostloadinq.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                            }

                            Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, dtpostloadinq.Rows[0]["coupon_code"].ToString(), dtpostloadinq.Rows[0]["shipper_id"].ToString(), dtpostloadinq.Rows[0]["load_inquiry_no"].ToString(), dtpostloadinq.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, Objtrans.created_by, Objtrans.created_host, Objtrans.device_id, Objtrans.device_type, ref Msg);
                            if (B2 != 1)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", Msg);
                            }
                        }
                        else
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", Msg);
                        }

                    }
                    #endregion

                    #region Create CBM Entry

                    DS_CBM objDsCbm = new DS_CBM();
                    Master objmaster = new Master(); string DocNtficID = "";

                    DataTable dtappid = new CBMController().GetAppIDbySizetype(dtpostloadinq.Rows[0]["SizeTypeCode"].ToString());

                    if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QHD", "", "", ref DocNtficID, ref message)) // New Driver Notification ID
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    }
                    PostOrderController objpostorder = new PostOrderController();

                    objDsCbm.EnforceConstraints = false;

                    DS_CBM.quote_hdrRow tr = objDsCbm.quote_hdr.Newquote_hdrRow();
                    tr.quote_id = tload[0].load_inquiry_no;
                    tr.quote_hdr_id = DocNtficID;
                    tr.appartment_id = dtappid.Rows[0]["appartment_id"].ToString();
                    tr.room_type = dtappid.Rows[0]["appartment_desc"].ToString();
                    tr.customer_name = objpostorder.GetUserdetailsByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.customer_mobile = objpostorder.GetMobileNoByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.customer_email = objpostorder.GetEmailByID(dtpostloadinq.Rows[0]["shipper_id"].ToString());
                    tr.total_cbm = 0;
                    tr.cbmlink = cbmlink;
                    tr.is_assign_to_order = Constant.FLAG_Y;
                    tr.is_create_on_order = Constant.FLAG_Y;
                    tr.StatusFlag = "D";
                    tr.created_date = System.DateTime.UtcNow;
                    tr.created_host = tload[0].created_host;
                    tr.created_by = tload[0].created_by;

                    objDsCbm.quote_hdr.Addquote_hdrRow(tr);
                    objDsCbm.quote_hdr.Rows[0].AcceptChanges();
                    objDsCbm.quote_hdr.Rows[0].SetAdded();

                    objDsCbm.EnforceConstraints = true;
                    objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_hdr, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #endregion

                    DataTable dt = dsorder.orders;

                    String load_inquiry_no = Objtrans.load_inquiry_no;
                    decimal Amount = Convert.ToDecimal(Objtrans.amount);
                    Amount = Amount / 100;
                    String bank_code = "";
                    String cheque_no = "";
                    DateTime? cheque_date = (DateTime?)null;
                    String payment_mode = "O";
                    DateTime payment_date = System.DateTime.UtcNow;
                    String remark = "";
                    String created_by = Objtrans.created_by;
                    String created_host = Objtrans.created_host;
                    String device_id = Objtrans.device_id;
                    String device_type = Objtrans.device_type;
                    String payment_rcv_by = "Admin";
                    String shipperid = "";


                    shipperid = dt.Rows[0]["shipper_id"].ToString();
                    dt.Columns.Add("adjusted_amt", typeof(String));
                    dt.Rows[0]["adjusted_amt"] = Amount.ToString();
                    dt.Columns.Add("db_cr", typeof(String));
                    dt.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                    dt.Columns.Add("match_unmatch", typeof(String));
                    dt.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                    dt.Columns.Add("payment_rcv_by", typeof(String));
                    dt.Rows[0]["payment_rcv_by"] = payment_rcv_by;

                    try
                    {
                        ServerLog.Log("SavePaymentReceipt_Entry(" + SendReceiveJSon.GetJson(dt) + ")");

                        TruckerMaster objTrukker = new TruckerMaster();

                        Byte SaveStatus = objTrukker.SavePaymentReceipt(ref DBCommand, payment_date, payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dt, created_by, created_host, device_id, device_type, ref Message);
                        if (SaveStatus != 1)
                        {
                            ServerLog.Log(Message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", Message);

                        }

                        ServerLog.Log("SavePaymentReceipt_Exit(" + SendReceiveJSon.GetJson(dsorder.orders) + ")");


                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);

                    }


                    dspostload.EnforceConstraints = false;
                    dspostload.post_load_inquiry[0].rem_amt_to_receive = 0;
                    dspostload.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;


                    try
                    {
                        PostOrderController objPostOrder = new PostOrderController();

                        DateTime dubaiTime = objPostOrder.DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                        string shipperEmail = objPostOrder.GetEmailByID(dt.Rows[0]["shipper_id"].ToString());
                        string shippername = objPostOrder.GetUserdetailsByID(dt.Rows[0]["shipper_id"].ToString());

                        string MsgMailbody = "";// "Thank you..Your order from  " + dt.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt.Rows[0]["load_inquiry_no"].ToString();

                        ServerLog.Log(shipperEmail);


                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(shipperEmail, " Your Order is confirmed (Order ID: " + Objtrans.load_inquiry_no + ")", shippername, MsgMailbody, Objtrans.load_inquiry_no, dt, dt.Rows[0]["TotalPackingCharge"].ToString()));
                        if (result["status"].ToString() == "0")
                            ServerLog.Log("Error Sending Activation Email");

                        //new EMail().GeneratePaymentAckMail(shipperEmail, " Payment of AED" + Amount + " for Order Id (" + Objtrans.load_inquiry_no + ")", shippername, Amount.ToString(), Objtrans.load_inquiry_no);


                        try
                        {
                            // Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt);
                        }
                        catch (Exception ex)
                        {
                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        ServerLog.Log("Error in send OTP on Completation ");
                    }

                    return BLGeneralUtil.return_ajax_statusdata("1", "Thank you...Payment done sucessfully", SendReceiveJSon.GetJson(dt));

                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("0", Objtrans.response_message);

                }



            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }
        }

        public String Save_dubi_GoodsOrderOnlinePayment([FromBody]paymentdetails Objtrans)
        {
            ServerLog.Log("Save_dubi_GoodsOrderOnlinePayment(" + Objtrans + ")");

            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            string DocNo = ""; string message = ""; string status = "";

            ServerLog.Log(Objtrans.ToString());
            List<paymentdetails> tload = new List<paymentdetails>();
            tload.Add(Objtrans);
            DataTable dtParameter = BLGeneralUtil.ToDataTable(tload);

            decimal totalamount = Convert.ToDecimal(Objtrans.amount);
            totalamount = totalamount / 100;
            dtParameter.Rows[0]["amount"] = totalamount.ToString();
            DataTable dt_DubiGoodsOrder = new DataTable();
            if (Objtrans.load_inquiry_no.Trim() != "")
            {
                dt_DubiGoodsOrder = new PostOrderController().GetDubiGoodsOrdersbyId(Objtrans.load_inquiry_no);
                if (dt_DubiGoodsOrder == null)
                    return BLGeneralUtil.return_ajax_string("0", "Order detail not found");
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Please provide order id");

            try
            {

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                #region Create Transaction Entry

                DS_Payment_Receipt dspayment = new DS_Payment_Receipt();

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }
                dspayment.EnforceConstraints = false;
                dspayment.order_paymentdetails.ImportRow(dtParameter.Rows[0]);
                dspayment.order_paymentdetails[0].Transaction_id = DocNo;
                dspayment.order_paymentdetails[0].created_date = System.DateTime.UtcNow;
                dspayment.order_paymentdetails.Rows[0].AcceptChanges();
                dspayment.order_paymentdetails.Rows[0].SetAdded();

                objBLReturnObject = master.UpdateTables(dspayment.order_paymentdetails, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                #endregion

                DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();

                if (Objtrans.status == Constant.PAYFORT_PURCHASE_SUCESS_STATUS)
                {
                    #region Update Data in Order table


                    DS_Dubi_Orders ds_dubiorder = new DS_Dubi_Orders();
                    string OrdId = "";


                    if (dt_DubiGoodsOrder == null)
                    {
                        ds_dubiorder.EnforceConstraints = false;
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        ds_dubiorder.dubiz_goods_orders_details.ImportRow(dspostload.post_load_inquiry[0]);
                        ds_dubiorder.dubiz_goods_orders_details[0].Transaction_id = OrdId;
                    }
                    else
                    {

                        //ds_dubiorder.dubiz_goods_orders_details.ImportRow(dt_DubiGoodsOrder.Rows[0]);
                        //ds_dubiorder.dubiz_goods_orders_details[0].active_flag = Constant.Flag_Yes;
                        //ds_dubiorder.dubiz_goods_orders_details[0].payment_mode = Constant.PaymentModeONLINE;
                        //ds_dubiorder.dubiz_goods_orders_details[0].created_date = System.DateTime.UtcNow;
                        //ds_dubiorder.EnforceConstraints = true;

                        //objBLReturnObject = master.UpdateTables(ds_dubiorder.dubiz_goods_orders_details, ref DBCommand);
                        //if (objBLReturnObject.ExecutionStatus == 2)
                        //{
                        //    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //    objBLReturnObject.ExecutionStatus = 2;
                        //    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        //}

                        String load_inquiry_no = Objtrans.load_inquiry_no;
                        decimal Amount = Convert.ToDecimal(Objtrans.amount);
                        Amount = Amount / 100;
                        String bank_code = "";
                        String cheque_no = "";
                        DateTime? cheque_date = (DateTime?)null;
                        String payment_mode = "O";
                        DateTime payment_date = System.DateTime.UtcNow;
                        String remark = "";
                        String created_by = Objtrans.created_by;
                        String created_host = Objtrans.created_host;
                        String device_id = Objtrans.device_id;
                        String device_type = Objtrans.device_type;
                        String payment_rcv_by = "Admin";
                        String shipperid = "";


                        shipperid = dt_DubiGoodsOrder.Rows[0]["user_no"].ToString();
                        dt_DubiGoodsOrder.Columns.Add("adjusted_amt", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["adjusted_amt"] = Amount.ToString();
                        dt_DubiGoodsOrder.Columns.Add("db_cr", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["db_cr"] = "C";  // "D" for debit amount and "C" 
                        dt_DubiGoodsOrder.Columns.Add("match_unmatch", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["match_unmatch"] = Constant.PaymentMatch;
                        dt_DubiGoodsOrder.Columns.Add("payment_rcv_by", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["payment_rcv_by"] = payment_rcv_by;
                        dt_DubiGoodsOrder.Columns.Add("load_inquiry_no", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["load_inquiry_no"] = load_inquiry_no;
                        dt_DubiGoodsOrder.Columns.Add("order_id", typeof(String));
                        dt_DubiGoodsOrder.Rows[0]["order_id"] = load_inquiry_no;
                        dt_DubiGoodsOrder.Columns.Add("shippingdatetime", typeof(DateTime));
                        dt_DubiGoodsOrder.Rows[0]["shippingdatetime"] = Convert.ToDateTime(dt_DubiGoodsOrder.Rows[0]["pickup_date"].ToString());

                        try
                        {

                            string Message = "";
                            TruckerMaster objTrukker = new TruckerMaster();
                            Byte SaveStatus = objTrukker.SaveDubiGoodsPaymentReceipt(ref DBCommand, payment_date, payment_mode, bank_code, cheque_no, cheque_date, Amount, shipperid, remark, dt_DubiGoodsOrder, created_by, created_host, device_id, device_type, ref Message);
                            if (SaveStatus != 1)
                            {
                                ServerLog.Log(Message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }
                            else
                            {
                                DBCommand.Transaction.Commit();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log(ex.Message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", ex.Message);

                        }

                    #endregion

                    }

                    try
                    {
                        PostOrderController objPostOrder = new PostOrderController();

                        DateTime dubaiTime = objPostOrder.DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                        string shipperEmail = dt_DubiGoodsOrder.Rows[0]["user_email"].ToString();
                        string shippername = dt_DubiGoodsOrder.Rows[0]["user_name"].ToString();

                        //string MsgMailbody = "Thank you..Your order from  " + dt_DubiGoodsOrder.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_DubiGoodsOrder.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_DubiGoodsOrder.Rows[0]["load_inquiry_no"].ToString();

                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateDubiGoodsOrderConfirmationMail(shipperEmail.Trim(), " Your Order is confirmed (Order ID: " + Objtrans.load_inquiry_no + ")", shippername, Objtrans.load_inquiry_no, dt_DubiGoodsOrder));
                        if (result["status"].ToString() == "0")
                            ServerLog.Log("Error Sending Activation Email");

                        try
                        {
                            // Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt);
                        }
                        catch (Exception ex)
                        {
                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        ServerLog.Log("Error in send OTP on Completation ");
                    }

                    return BLGeneralUtil.return_ajax_statusdata("1", "Thank you...Payment done sucessfully", SendReceiveJSon.GetJson(dt_DubiGoodsOrder));

                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("0", Objtrans.response_message);

                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

            }
        }

    }
}
