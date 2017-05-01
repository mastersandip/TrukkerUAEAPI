using BLL.Master;
using BLL.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TrkrLite.Controllers;
using trukkerUAE.Classes;
using trukkerUAE.Models;
using trukkerUAE.XSD;

namespace trukkerUAE.Controllers
{
    public class AdminController : ServerBase
    {

        PostOrderController objcntrlpostorder = new PostOrderController();

        #region Addon Services


        [HttpGet]
        public string GetAllServices(string opt)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from Services_mst ";
            if (opt.ToUpper() == "P")
                query1 += " where type in ('" + opt + "') order by sortno  ";
            else
                query1 += " order by sortno ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetAllAgency()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from Agency_mst ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetSubServicesById(string serviceID)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from Subservice_maping_details where ServiceTypeCode='" + serviceID + "'";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Details found");
        }
        [HttpGet]
        public string GetUserDetailsByLoadInqNo(string inqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from user_mst join orders on orders.shipper_id=user_mst.unique_id where orders.load_inquiry_no='" + inqno + "'";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Details found");
        }
        [HttpGet]
        public string GetAllOrderAddonServiceDetails()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @"select Agency_mst.agency_Name,SizeTypeMst.SizeTypeDesc,Subservice_maping_details.SubServiceTypeDesc,Services_mst.ServiceTypeDesc,order_AddonService_details.* from order_AddonService_details 
                             left join Subservice_maping_details on Subservice_maping_details.SubServiceTypeCode=order_AddonService_details.SubServiceTypeCode
                             left join Services_mst on Services_mst.ServiceTypeCode=order_AddonService_details.ServiceTypeCode
                             left join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode   
                             left join Agency_mst on Agency_mst.agency_id=order_AddonService_details.AgencyId
                             where order_AddonService_details.active_flag='Y'
                             order by order_AddonService_details.created_date desc ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetOrderAddonDetailsByTrnID(string transactionid)
        {
            DataTable dtAddondetails = new DataTable();
            String query1 = @" select Agency_mst.agency_Name,SizeTypeMst.SizeTypeDesc,Subservice_maping_details.SubServiceTypeDesc,Services_mst.ServiceTypeDesc,
                                order_AddonService_details.Transaction_id, order_AddonService_details.AddSerBaseDiscount, order_AddonService_details.ServiceTypeCode, order_AddonService_details.SizeTypeCode, order_AddonService_details.ServiceCharge,
                                order_AddonService_details.ServiceDiscount, order_AddonService_details.SubServiceTypeCode, order_AddonService_details.load_inquiry_no, user_mobileno, 
                                user_email, user_name, address, CelingRequired, NoofCleling, CONVERT(VARCHAR, order_AddonService_details.Service_date, 103) AS Service_date, CONVERT(VARCHAR, order_AddonService_details.Service_time, 108) 
                                AS Service_time, order_AddonService_details.NoofCleaners, order_AddonService_details.Notes, order_AddonService_details.status, order_AddonService_details.AgencyId, order_AddonService_details.rem_amt_to_receive,
                                order_AddonService_details.Payment_mode, order_AddonService_details.payment_status, order_AddonService_details.payment_link, order_AddonService_details.created_by, order_AddonService_details.created_date, 
                                order_AddonService_details.created_host, order_AddonService_details.device_id, order_AddonService_details.device_type, order_AddonService_details.modified_by, order_AddonService_details.modified_date, 
                                order_AddonService_details.modified_host, order_AddonService_details.modified_device_id, order_AddonService_details.modified_device_type,
                                order_AddonService_details.addon_by,order_AddonService_details.rem_amt_to_receive,order_AddonService_details.Payment_mode,order_AddonService_details.payment_status,order_AddonService_details.payment_link,order_AddonService_details.active_flag
                                from order_AddonService_details
                                left join Subservice_maping_details on Subservice_maping_details.SubServiceTypeCode=order_AddonService_details.SubServiceTypeCode
                                left join Services_mst on Services_mst.ServiceTypeCode=order_AddonService_details.ServiceTypeCode
                                left join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                                left join Agency_mst on Agency_mst.agency_id=order_AddonService_details.AgencyId
                                where order_AddonService_details.Transaction_id='" + transactionid + "' order by order_AddonService_details.created_date desc";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtAddondetails = ds.Tables[0];
            }

            if (dtAddondetails != null && dtAddondetails.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtAddondetails));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Details found");
        }
        [HttpGet]
        public string GetOrderAddonDetailsByInqID(string inqId)
        {
            DataTable dtAddondetails = new DataTable();
            String query1 = @" select Agency_mst.agency_Name,SizeTypeMst.SizeTypeDesc,Subservice_maping_details.SubServiceTypeDesc,Services_mst.ServiceTypeDesc,
                                order_AddonService_details.Transaction_id, order_AddonService_details.AddSerBaseDiscount, order_AddonService_details.ServiceTypeCode, order_AddonService_details.SizeTypeCode, order_AddonService_details.ServiceCharge,
                                order_AddonService_details.ServiceDiscount, order_AddonService_details.SubServiceTypeCode, order_AddonService_details.load_inquiry_no, user_mobileno, 
                                user_email, user_name, address, CelingRequired, NoofCleling, CONVERT(VARCHAR, order_AddonService_details.Service_date, 103) AS Service_date, CONVERT(VARCHAR, order_AddonService_details.Service_time, 108) 
                                AS Service_time, order_AddonService_details.NoofCleaners, order_AddonService_details.Notes, order_AddonService_details.status, order_AddonService_details.AgencyId, order_AddonService_details.rem_amt_to_receive,
                                order_AddonService_details.Payment_mode, order_AddonService_details.payment_status, order_AddonService_details.payment_link, order_AddonService_details.created_by, order_AddonService_details.created_date, 
                                order_AddonService_details.created_host, order_AddonService_details.device_id, order_AddonService_details.device_type, order_AddonService_details.modified_by, order_AddonService_details.modified_date, 
                                order_AddonService_details.modified_host, order_AddonService_details.modified_device_id, order_AddonService_details.modified_device_type,
                                order_AddonService_details.addon_by,order_AddonService_details.rem_amt_to_receive,order_AddonService_details.Payment_mode,order_AddonService_details.payment_status,order_AddonService_details.payment_link,order_AddonService_details.active_flag
                                from order_AddonService_details
                                left join Subservice_maping_details on Subservice_maping_details.SubServiceTypeCode=order_AddonService_details.SubServiceTypeCode
                                left join Services_mst on Services_mst.ServiceTypeCode=order_AddonService_details.ServiceTypeCode
                                left join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                                left join Agency_mst on Agency_mst.agency_id=order_AddonService_details.AgencyId
                                where order_AddonService_details.load_inquiry_no='" + inqId + "' order by order_AddonService_details.created_date desc";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtAddondetails = ds.Tables[0];
            }

            if (dtAddondetails != null && dtAddondetails.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtAddondetails));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Details found");
        }
        public DataTable SelectOrderAddonDetailsByTrnID(String trnsactionid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @" select Agency_mst.agency_Name,SizeTypeMst.SizeTypeDesc,Subservice_maping_details.SubServiceTypeDesc,Services_mst.ServiceTypeDesc,
                                order_AddonService_details.Transaction_id, order_AddonService_details.AddSerBaseDiscount, order_AddonService_details.ServiceTypeCode, order_AddonService_details.SizeTypeCode, order_AddonService_details.ServiceCharge,
                                order_AddonService_details.ServiceDiscount, order_AddonService_details.SubServiceTypeCode, order_AddonService_details.load_inquiry_no, user_mobileno, 
                                user_email, user_name, address, CelingRequired, NoofCleling,order_AddonService_details.Service_date,order_AddonService_details.Service_time, order_AddonService_details.NoofCleaners, order_AddonService_details.Notes, order_AddonService_details.status, order_AddonService_details.AgencyId, order_AddonService_details.rem_amt_to_receive,
                                order_AddonService_details.Payment_mode, order_AddonService_details.payment_status, order_AddonService_details.payment_link, order_AddonService_details.created_by, order_AddonService_details.created_date, 
                                order_AddonService_details.created_host, order_AddonService_details.device_id, order_AddonService_details.device_type, order_AddonService_details.modified_by, order_AddonService_details.modified_date, 
                                order_AddonService_details.modified_host, order_AddonService_details.modified_device_id, order_AddonService_details.modified_device_type,
                                order_AddonService_details.addon_by,order_AddonService_details.rem_amt_to_receive,order_AddonService_details.Payment_mode,order_AddonService_details.payment_status,order_AddonService_details.payment_link,order_AddonService_details.active_flag
                                from order_AddonService_details
                                left join Subservice_maping_details on Subservice_maping_details.SubServiceTypeCode=order_AddonService_details.SubServiceTypeCode
                                left join Services_mst on Services_mst.ServiceTypeCode=order_AddonService_details.ServiceTypeCode
                                left join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                                left join Agency_mst on Agency_mst.agency_id=order_AddonService_details.AgencyId
                                where order_AddonService_details.Transaction_id='" + trnsactionid + "' order by order_AddonService_details.created_date desc";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
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
        public DataTable SelectAddonPaymentDetailsByTrnId(String trnsactionid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @" select * from order_paymentdetails 
                            left join order_AddonService_details on order_AddonService_details.transaction_id=order_paymentdetails.load_inquiry_no 
                            where order_paymentdetails.transaction_id ='" + trnsactionid + "' ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
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
        public DataTable SelectOrderAddonDetailsByInqID(string loadinqno, string transactionid)
        {
            DataTable dt = new DataTable();
            String query1 = @" select     * from order_AddonService_details
                               where order_AddonService_details.load_inquiry_no='" + loadinqno + "' ";

            if (transactionid != "")
                query1 += " or Transaction_id = '" + transactionid + "'   ";

            query1 += "order by order_AddonService_details.created_date desc";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
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
        public string DeleteAddonService(string Transaction_id)
        {
            Document objdoc = new Document();
            BLReturnObject objBLobj = new BLReturnObject();
            Master master = new Master();
            DS_orders dsord = new DS_orders();
            Document objDocument = new Document();

            DataTable dtserDetails = SelectOrderAddonDetailsByTrnID(Transaction_id);
            if (dtserDetails != null)
            {
                dtserDetails = BLGeneralUtil.CheckDateTime(dtserDetails);
                DataTable dtorderPrv = new PostOrderController().GetLoadOrdersByID(dtserDetails.Rows[0]["load_inquiry_no"].ToString());

                //string strdate = Convert.ToDateTime(dtserDetails.Rows[0]["Service_date"].ToString()).ToString("yyyy-MM-dd hh:mm:ss tt");
                //dtserDetails.Rows[0]["Service_date"] = Convert.ToDateTime(dtserDetails.Rows[0]["Service_date"].ToString()).ToString("MM-dd-yyyy"); 
                //dtserDetails.Rows[0]["created_date"] = Convert.ToDateTime(dtserDetails.Rows[0]["created_date"].ToString()).ToString("yyyy-MM-dd hh:mm:ss tt");

                try
                {


                    DBConnection.Open();
                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();


                    if (dtserDetails.Rows[0]["addon_by"].ToString() == "U")
                    {
                        if (dtorderPrv != null)
                        {
                            decimal CurrentServCharge = dtserDetails.Rows[0]["ServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtserDetails.Rows[0]["ServiceCharge"].ToString()) : 0;
                            decimal CurrentServrRem_amt = dtserDetails.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtserDetails.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                            decimal CurrentServDisc = dtserDetails.Rows[0]["ServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtserDetails.Rows[0]["ServiceDiscount"].ToString()) : 0;


                            decimal Total_cost = dtorderPrv.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["Total_cost"].ToString()) : 0;
                            decimal Total_cost_without_discount = dtorderPrv.Rows[0]["Total_cost_without_discount"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["Total_cost_without_discount"].ToString()) : 0;
                            decimal rem_amt_to_receive = dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                            decimal TotalAddServiceDiscount = dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                            decimal TotalAddServiceCharge = dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;


                            dsord.EnforceConstraints = false;

                            dsord.orders.ImportRow(dtorderPrv.Rows[0]);
                            dsord.orders[0].Total_cost = Total_cost - CurrentServCharge;
                            if (rem_amt_to_receive == 0)
                                dsord.orders[0].rem_amt_to_receive = rem_amt_to_receive;
                            else
                                dsord.orders[0].rem_amt_to_receive = rem_amt_to_receive - CurrentServrRem_amt;

                            dsord.orders[0].Total_cost_without_discount = Total_cost_without_discount - CurrentServCharge;
                            dsord.orders[0].TotalAddServiceCharge = TotalAddServiceCharge - CurrentServCharge;
                            dsord.orders[0].TotalAddServiceDiscount = TotalAddServiceDiscount - CurrentServDisc;

                            dsord.EnforceConstraints = true;

                            objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }


                    dsord.EnforceConstraints = false;

                    //DS_orders.order_AddonService_detailsRow dr = dsord.order_AddonService_details.Neworder_AddonService_detailsRow();
                    //dr.ItemArray = dtserDetails.Rows[0].ItemArray;
                    //dsord.order_AddonService_details.Addorder_AddonService_detailsRow(dr);
                    dsord.order_AddonService_details.ImportRow(dtserDetails.Rows[0]);
                    dsord.order_AddonService_details.AcceptChanges();
                    dsord.order_AddonService_details[0].Delete();

                    dsord.EnforceConstraints = true;

                    objBLobj = master.UpdateTables(dsord.order_AddonService_details, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;

                    return BLGeneralUtil.return_ajax_string("1", " Addon Service Delete sucessfully ");

                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", " Details Not Found ");
            }


        }

        [HttpGet]
        public string SendAddonPaymentLinkMail(string transactionid, string emailId)
        {
            try
            {

                StreamReader sr; string LINK = "";
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_SendADDONGeneratedLink.html");
                string strdata = ""; string msg = "";
                string SubAddonPayment = ConfigurationManager.AppSettings["SUBADDONPAYMENT"];

                DataTable dtaddondetails = SelectAddonPaymentDetailsByTrnId(transactionid);

                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();

                    if (dtaddondetails.Rows[0]["ServiceTypeCode"].ToString() == Constant.PAINTING_SERIVCES)
                        strdata = strdata.Replace("SERVICES", "Move Out Painting");
                    if (dtaddondetails.Rows[0]["ServiceTypeCode"].ToString() == Constant.CLEANING_SERIVCES)
                        strdata = strdata.Replace("SERVICES", "Cleaning");
                    if (dtaddondetails.Rows[0]["ServiceTypeCode"].ToString() == Constant.PESTCONTROL_SERIVCES)
                        strdata = strdata.Replace("SERVICES", "Pest Control");

                    strdata = strdata.Replace("UserName", dtaddondetails.Rows[0]["user_name"].ToString());
                    strdata = strdata.Replace("ADDONLINK", dtaddondetails.Rows[0]["paymentlink"].ToString());
                    strdata = strdata.Replace("SERVICECHARGE", dtaddondetails.Rows[0]["amount"].ToString());
                }
                sr.Close();


                msg = "";
                EMail objemail = new EMail();
                Boolean bl = objemail.SendMail(emailId, strdata, dtaddondetails.Rows[0]["user_name"].ToString() + "," + SubAddonPayment + dtaddondetails.Rows[0]["amount"].ToString(), ref msg, "CONTACT", "SENDFROM");
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

        [HttpPost]
        public string SaveOrderAddonDetails([FromBody]JObject Jobj)
        {
            List<order_AddonService_details> objOrderAddon = new List<order_AddonService_details>();
            DS_orders dsord = new DS_orders();
            DS_Post_load_enquiry dspostorder = new DS_Post_load_enquiry();
            Document objdoc = new Document(); String DocNtficID = ""; string message = "";
            BLReturnObject objBLobj = new BLReturnObject();
            Master master = new Master();
            DataSet dsorderaddon = new DataSet(); DataTable dtparameter = new DataTable();
            Document objDocument = new Document();
            DataTable dtaddondetails = null;
            DataTable dtOrderaddOnloadInq = null;

            if (Jobj["Addons"] != null)
            {
                objOrderAddon = Jobj["Addons"].ToObject<List<order_AddonService_details>>();
                dsorderaddon = master.CreateDataSet(objOrderAddon);
                dtparameter = BLGeneralUtil.CheckDateTime(dsorderaddon.Tables[0]);
                ServerLog.Log("SaveOrderAddonDetails : " + Jobj.ToString() + ")");
            }

            if (objOrderAddon[0].Transaction_id != "")
            {
                DataTable dttmp = new DataTable();
                dttmp = SelectOrderAddonDetailsByTrnID(objOrderAddon[0].Transaction_id);
                dtaddondetails = BLGeneralUtil.CheckDateTime(dttmp);
                // string strdate = Convert.ToDateTime(dtaddondetails.Rows[0]["Service_date"].ToString()).ToString("yyyy-MM-dd");
                // dtaddondetails.Rows[0]["Service_date"] = strdate;
            }

            if (objOrderAddon[0].load_inquiry_no != "")
            {
                dtOrderaddOnloadInq = SelectOrderAddonDetailsByInqID(objOrderAddon[0].load_inquiry_no, objOrderAddon[0].Transaction_id);
            }

            try
            {
                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = ""; TimeSpan servicetime = TimeSpan.Parse(dtparameter.Rows[0]["Service_time"].ToString());

                if (dtaddondetails == null)
                {
                    if (!objDocument.W_GetNextDocumentNoNew(ref DBCommand, "ADS", "", "", ref DocNtficID, ref message))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }

                    if (dtOrderaddOnloadInq != null)
                    {
                        DataRow[] dr = dtOrderaddOnloadInq.Select("ServiceTypeCode = '" + objOrderAddon[0].ServiceTypeCode + "'");
                        if (dr.Length > 0)
                        {
                            DataRow[] drsub = dr.CopyToDataTable().Select("SubServiceTypeCode='" + objOrderAddon[0].SubServiceTypeCode + "'");
                            if (drsub.Length > 0)
                            {
                                ServerLog.Log("Already created service for load_inquiry_no : " + objOrderAddon[0].load_inquiry_no);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", "Already service created for load_inquiry_no : " + objOrderAddon[0].load_inquiry_no);
                            }
                        }
                    }

                    dsord.EnforceConstraints = false;
                    dsord.order_AddonService_details.ImportRow(dtparameter.Rows[0]);
                    dsord.order_AddonService_details[0].Transaction_id = DocNtficID;
                    dsord.order_AddonService_details[0].addon_by = "AD";
                    dsord.order_AddonService_details[0].active_flag = Constant.FLAG_Y;
                }
                else
                {
                    if (dtOrderaddOnloadInq != null)
                    {
                        DataRow[] dr = dtOrderaddOnloadInq.Select("ServiceTypeCode = '" + objOrderAddon[0].ServiceTypeCode + "'");
                        if (dr.Length > 0)
                        {
                            DataRow[] drsub = dr.CopyToDataTable().Select("SubServiceTypeCode='" + objOrderAddon[0].SubServiceTypeCode + "'");
                            if (drsub.Length > 0 && drsub[0]["Transaction_id"].ToString() != objOrderAddon[0].Transaction_id)
                            {
                                if (drsub[0]["ServiceTypeCode"].ToString() != "OTH")
                                {
                                    ServerLog.Log("Already created service for load_inquiry_no :" + objOrderAddon[0].load_inquiry_no);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", "Already service created for load_inquiry_no : " + objOrderAddon[0].load_inquiry_no);
                                }
                            }
                        }
                    }

                    dsord.EnforceConstraints = false;
                    dsord.order_AddonService_details.ImportRow(dtaddondetails.Rows[0]);
                }

                decimal ServiceCharge = objOrderAddon[0].ServiceCharge != "" ? Convert.ToDecimal(objOrderAddon[0].ServiceCharge) : 0;
                decimal ServiceDiscount = objOrderAddon[0].ServiceDiscount != "" ? Convert.ToDecimal(objOrderAddon[0].ServiceDiscount) : 0;
                decimal prevRemamt = 0;
                decimal prevCost = 0;

                if (objOrderAddon[0].load_inquiry_no != "")
                {

                    DataRow[] drPerviousOrderdtlbyInqId = null;
                    DataRow[] drPerviousOrderdtlByTrnId = null;
                    DataRow[] drCurrrentAddon = null;
                    //decimal SerPrvAmtByInqId = 0; decimal SerPrvDisByInqId = 0;
                    decimal SerPrvAmtByTrnId = 0; decimal SerPrvDisByTrnId = 0; int ordcnt = 0;

                    if (dtOrderaddOnloadInq != null)
                    {
                        drPerviousOrderdtlbyInqId = dtOrderaddOnloadInq.Select(" load_inquiry_no = '" + objOrderAddon[0].load_inquiry_no + "' and Transaction_id not in ('" + objOrderAddon[0].Transaction_id + "') ");
                        drPerviousOrderdtlByTrnId = dtOrderaddOnloadInq.Select(" Transaction_id = '" + objOrderAddon[0].Transaction_id + "' and load_inquiry_no not in ( '" + objOrderAddon[0].load_inquiry_no + "' ) ");
                        drCurrrentAddon = dtOrderaddOnloadInq.Select(" Transaction_id = '" + objOrderAddon[0].Transaction_id + "' and load_inquiry_no = '" + objOrderAddon[0].load_inquiry_no + "' ");
                        //dr= dtOrderaddOnloadInq.Select("ServiceTypeCode = '" + objOrderAddon[0].ServiceTypeCode + "' or Transaction_id not in ( '" + objOrderAddon[0].Transaction_id + "' )");

                        if (drCurrrentAddon.Length > 0 && drCurrrentAddon[0]["addon_by"].ToString() == "U")
                        {
                            if (drPerviousOrderdtlByTrnId != null)
                            {
                                if (drCurrrentAddon != null && drCurrrentAddon.Length > 0)
                                {
                                    DataTable dtorder = new PostOrderController().GetLoadOrdersByID(objOrderAddon[0].load_inquiry_no);
                                    prevCost = Convert.ToDecimal(drCurrrentAddon[0]["ServiceCharge"].ToString());
                                    prevRemamt = Convert.ToDecimal(drCurrrentAddon[0]["rem_amt_to_receive"].ToString());
                                    decimal Total_cost = dtorder.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Total_cost"].ToString()) : 0;
                                    decimal rem_amt_to_receive = dtorder.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                    decimal TotalAddServiceDiscount = dtorder.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                    decimal TotalAddServiceCharge = dtorder.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                    ordcnt = dsord.orders.Rows.Count;

                                    dsord.EnforceConstraints = false;
                                    dsord.orders.ImportRow(dtorder.Rows[0]);
                                    dsord.orders[ordcnt].Total_cost = (Total_cost - prevCost) + ServiceCharge;
                                    dsord.orders[ordcnt].rem_amt_to_receive = (rem_amt_to_receive - prevCost) + ServiceCharge;
                                    dsord.orders[ordcnt].TotalAddServiceCharge = (TotalAddServiceCharge - prevCost) + ServiceCharge;
                                    dsord.EnforceConstraints = true;



                                }
                                else if (drPerviousOrderdtlByTrnId[0]["ServiceTypeCode"].ToString() == objOrderAddon[0].ServiceTypeCode)
                                {
                                    // Update Total cost in previous order

                                    SerPrvAmtByTrnId = drPerviousOrderdtlByTrnId[0]["ServiceCharge"].ToString() != "" ? Convert.ToDecimal(drPerviousOrderdtlByTrnId[0]["ServiceCharge"].ToString()) : 0;
                                    SerPrvDisByTrnId = drPerviousOrderdtlByTrnId[0]["ServiceDiscount"].ToString() != "" ? Convert.ToDecimal(drPerviousOrderdtlByTrnId[0]["ServiceDiscount"].ToString()) : 0;

                                    DataTable dtorderPrv = new PostOrderController().GetLoadOrdersByID(drPerviousOrderdtlByTrnId[0]["load_inquiry_no"].ToString());
                                    if (dtorderPrv != null)
                                    {
                                        decimal Total_cost = dtorderPrv.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["Total_cost"].ToString()) : 0;
                                        decimal rem_amt_to_receive = dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                        decimal TotalAddServiceDiscount = dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                        decimal TotalAddServiceCharge = dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                        ordcnt = dsord.orders.Rows.Count;

                                        dsord.EnforceConstraints = false;
                                        dsord.orders.ImportRow(dtorderPrv.Rows[0]);
                                        dsord.orders[ordcnt].Total_cost = Total_cost - SerPrvAmtByTrnId;
                                        dsord.orders[ordcnt].rem_amt_to_receive = rem_amt_to_receive - SerPrvAmtByTrnId;
                                        dsord.orders[ordcnt].TotalAddServiceCharge = TotalAddServiceCharge - SerPrvAmtByTrnId;
                                        dsord.EnforceConstraints = true;
                                    }

                                    //update new total cost in new order 
                                    DataTable dtorder_new = new PostOrderController().GetLoadOrdersByID(objOrderAddon[0].load_inquiry_no);
                                    if (dtorder_new != null)
                                    {
                                        decimal Total_cost_new = dtorder_new.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["Total_cost"].ToString()) : 0;
                                        decimal rem_amt_to_receive_new = dtorder_new.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                        decimal TotalAddServiceDiscount_new = dtorder_new.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                        decimal TotalAddServiceCharge_new = dtorder_new.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                        ordcnt = dsord.orders.Rows.Count;

                                        dsord.EnforceConstraints = false;
                                        dsord.orders.ImportRow(dtorder_new.Rows[0]);
                                        dsord.orders[ordcnt].Total_cost = Total_cost_new + ServiceCharge;
                                        dsord.orders[ordcnt].rem_amt_to_receive = rem_amt_to_receive_new + ServiceCharge;
                                        dsord.orders[ordcnt].TotalAddServiceCharge = TotalAddServiceCharge_new + ServiceCharge;
                                        //dsord.orders[ordcnt].TotalAddServiceDiscount = TotalAddServiceDiscount_new + ServiceDiscount;
                                        dsord.EnforceConstraints = true;
                                    }
                                }
                                else
                                {
                                    DataRow[] dr = dtOrderaddOnloadInq.Select("ServiceTypeCode = '" + objOrderAddon[0].ServiceTypeCode + "' ");//or Transaction_id not in ( '" + objOrderAddon[0].Transaction_id + "' )");
                                    DataTable dtorder = new PostOrderController().GetLoadOrdersByID(objOrderAddon[0].load_inquiry_no);
                                    decimal serChargePrev = 0;

                                    decimal Total_cost = dtorder.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["Total_cost"].ToString()) : 0;
                                    decimal rem_amt_to_receive = dtorder.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                    decimal TotalAddServiceDiscount = dtorder.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                    decimal TotalAddServiceCharge = dtorder.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                    DataRow[] drPrevious = dtOrderaddOnloadInq.Select("Transaction_id = '" + objOrderAddon[0].Transaction_id + "'");

                                    if (drPrevious.Length > 0)
                                    {
                                        serChargePrev = drPrevious[0]["ServiceCharge"].ToString() != "" ? Convert.ToDecimal(drPrevious[0]["ServiceCharge"].ToString()) : 0;
                                        Total_cost = Total_cost - serChargePrev;
                                        rem_amt_to_receive = rem_amt_to_receive - serChargePrev;
                                    }

                                    if (dr.Length == 0 || serChargePrev != ServiceCharge)
                                    {
                                        ordcnt = dsord.orders.Rows.Count;

                                        dsord.EnforceConstraints = false;
                                        dsord.orders.ImportRow(dtorder.Rows[0]);
                                        dsord.orders[ordcnt].Total_cost = Total_cost + ServiceCharge;
                                        dsord.orders[ordcnt].rem_amt_to_receive = rem_amt_to_receive + ServiceCharge;
                                        dsord.orders[ordcnt].TotalAddServiceCharge = TotalAddServiceCharge + ServiceCharge;
                                        //dsord.orders[ordcnt].TotalAddServiceDiscount = TotalAddServiceDiscount + ServiceDiscount;
                                        dsord.EnforceConstraints = true;
                                    }
                                }

                            }
                            else
                            {
                                // Update Total cost in previous order

                                SerPrvAmtByTrnId = drPerviousOrderdtlByTrnId[0]["ServiceCharge"].ToString() != "" ? Convert.ToDecimal(drPerviousOrderdtlByTrnId[0]["ServiceCharge"].ToString()) : 0;
                                SerPrvDisByTrnId = drPerviousOrderdtlByTrnId[0]["ServiceDiscount"].ToString() != "" ? Convert.ToDecimal(drPerviousOrderdtlByTrnId[0]["ServiceDiscount"].ToString()) : 0;

                                DataTable dtorderPrv = new PostOrderController().GetLoadOrdersByID(drPerviousOrderdtlByTrnId[0]["load_inquiry_no"].ToString());

                                decimal Total_cost = dtorderPrv.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["Total_cost"].ToString()) : 0;
                                decimal rem_amt_to_receive = dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                decimal TotalAddServiceDiscount = dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                decimal TotalAddServiceCharge = dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorderPrv.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;


                                ordcnt = dsord.orders.Rows.Count;

                                dsord.EnforceConstraints = false;
                                dsord.orders.ImportRow(dtorderPrv.Rows[0]);
                                dsord.orders[ordcnt].Total_cost = Total_cost - SerPrvAmtByTrnId;
                                dsord.orders[ordcnt].rem_amt_to_receive = rem_amt_to_receive - SerPrvAmtByTrnId;
                                dsord.orders[ordcnt].TotalAddServiceCharge = TotalAddServiceCharge - SerPrvAmtByTrnId;
                                //dsord.orders[ordcnt].TotalAddServiceDiscount = TotalAddServiceDiscount - SerPrvDisByTrnId;
                                dsord.EnforceConstraints = true;

                                //update new total cost in new order 
                                DataTable dtorder_new = new PostOrderController().GetLoadOrdersByID(objOrderAddon[0].load_inquiry_no);

                                decimal Total_cost_new = dtorder_new.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["Total_cost"].ToString()) : 0;
                                decimal rem_amt_to_receive_new = dtorder_new.Rows[0]["rem_amt_to_receive"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["rem_amt_to_receive"].ToString()) : 0;
                                decimal TotalAddServiceDiscount_new = dtorder_new.Rows[0]["TotalAddServiceDiscount"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["TotalAddServiceDiscount"].ToString()) : 0;
                                decimal TotalAddServiceCharge_new = dtorder_new.Rows[0]["TotalAddServiceCharge"].ToString() != "" ? Convert.ToDecimal(dtorder_new.Rows[0]["TotalAddServiceCharge"].ToString()) : 0;

                                ordcnt = dsord.orders.Rows.Count;

                                dsord.EnforceConstraints = false;
                                dsord.orders.ImportRow(dtorder_new.Rows[0]);
                                dsord.orders[ordcnt].Total_cost = Total_cost_new + ServiceCharge;
                                dsord.orders[ordcnt].rem_amt_to_receive = rem_amt_to_receive_new + ServiceCharge;
                                dsord.orders[ordcnt].TotalAddServiceCharge = TotalAddServiceCharge_new + ServiceCharge;
                                //dsord.orders[ordcnt].TotalAddServiceDiscount = TotalAddServiceDiscount_new + ServiceDiscount;
                                dsord.EnforceConstraints = true;
                            }
                        }
                    }

                    if (dsord.orders != null && dsord.orders.Rows.Count > 0)
                    {
                        dspostorder.EnforceConstraints = false;
                        dspostorder.post_load_inquiry.ImportRow(dsord.orders[0]);
                        dspostorder.EnforceConstraints = true;

                        objBLobj = master.UpdateTables(dspostorder.post_load_inquiry, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                }


                dsord.order_AddonService_details[0].ServiceTypeCode = objOrderAddon[0].ServiceTypeCode;
                dsord.order_AddonService_details[0].SizeTypeCode = objOrderAddon[0].SizeTypeCode;
                dsord.order_AddonService_details[0].ServiceCharge = ServiceCharge;
                dsord.order_AddonService_details[0].ServiceCharge_without_discount = ServiceCharge;

                if (prevCost == Convert.ToDecimal(objOrderAddon[0].ServiceCharge))
                {
                    if (prevRemamt < prevCost)
                        dsord.order_AddonService_details[0].rem_amt_to_receive = prevRemamt;
                    else
                        dsord.order_AddonService_details[0].rem_amt_to_receive = ServiceCharge;
                }
                else
                {
                    if (prevCost < Convert.ToDecimal(objOrderAddon[0].ServiceCharge))
                        dsord.order_AddonService_details[0].rem_amt_to_receive = prevRemamt + (Convert.ToDecimal(objOrderAddon[0].ServiceCharge) - prevCost);
                    else if (prevCost > Convert.ToDecimal(objOrderAddon[0].ServiceCharge))
                        dsord.order_AddonService_details[0].rem_amt_to_receive = prevRemamt - (prevCost - Convert.ToDecimal(objOrderAddon[0].ServiceCharge));
                }

                dsord.order_AddonService_details[0].ServiceDiscount = ServiceDiscount;
                dsord.order_AddonService_details[0].SubServiceTypeCode = objOrderAddon[0].SubServiceTypeCode;
                dsord.order_AddonService_details[0].load_inquiry_no = objOrderAddon[0].load_inquiry_no;
                dsord.order_AddonService_details[0].user_mobileno = objOrderAddon[0].user_mobileno;
                dsord.order_AddonService_details[0].user_email = objOrderAddon[0].user_email;
                dsord.order_AddonService_details[0].user_name = objOrderAddon[0].user_name;
                dsord.order_AddonService_details[0].address = objOrderAddon[0].address;
                dsord.order_AddonService_details[0].CelingRequired = objOrderAddon[0].CelingRequired;
                dsord.order_AddonService_details[0].Service_date = Convert.ToDateTime(dtparameter.Rows[0]["Service_date"].ToString());
                dsord.order_AddonService_details[0].Service_time = servicetime;
                dsord.order_AddonService_details[0].NoofCleling = objOrderAddon[0].NoofCleling != "" ? Convert.ToDecimal(objOrderAddon[0].NoofCleling) : 0;
                dsord.order_AddonService_details[0].NoofCleaners = objOrderAddon[0].NoofCleaners != "" ? Convert.ToDecimal(objOrderAddon[0].NoofCleaners) : 0;
                dsord.order_AddonService_details[0].Notes = objOrderAddon[0].Notes;
                dsord.order_AddonService_details[0].status = objOrderAddon[0].status;
                dsord.order_AddonService_details[0].payment_status = Constant.FLAG_N;
                dsord.order_AddonService_details[0].AgencyId = objOrderAddon[0].AgencyId;
                dsord.order_AddonService_details[0].created_by = objOrderAddon[0].created_by;
                dsord.order_AddonService_details[0].created_date = System.DateTime.UtcNow;
                dsord.order_AddonService_details[0].created_host = objOrderAddon[0].created_host;
                dsord.order_AddonService_details[0].device_id = objOrderAddon[0].device_id;
                dsord.order_AddonService_details[0].device_type = objOrderAddon[0].device_type;


                dsord.EnforceConstraints = true;

                objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }

                objBLobj = master.UpdateTables(dsord.order_AddonService_details, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                if (DocNtficID != "")
                    new EMail().GenerateAddonConfirmationMail(objOrderAddon[0].user_email, " Your SERVICETYPECODE service has been confirmed (Order ID: " + DocNtficID + ")", objOrderAddon[0].user_name, "", DocNtficID, dsord.order_AddonService_details);

                return BLGeneralUtil.return_ajax_string("1", " Addon Service Add sucessfully ");

                //#endregion
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }


        #endregion

        #region Admin Pannel Services

        [HttpPost]
        public string GetOrdersDetailsForDeshboardold([FromBody]JObject Jobj)
        {

            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = ""; string sourcecity = "";
            string destinationcity = ""; string owner_id = ""; string username = ""; string order_type_flag = "";
            string RowsPerPage = ""; string PageNo = ""; string Assigndriver = ""; string OrderBy = ""; string SortBy = "";
            objOrder = Jobj["order_deshboard"].ToObject<List<orders>>();
            if (Jobj["order_deshboard"] != null)
            {
                status = objOrder[0].status;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
                sourcecity = objOrder[0].inquiry_source_city;
                destinationcity = objOrder[0].inquiry_destination_city;
                owner_id = objOrder[0].owner_id;
                username = objOrder[0].username;
                order_type_flag = objOrder[0].order_type;
                Assigndriver = objOrder[0].Assigndriver;
                OrderBy = objOrder[0].OrderBy;
                SortBy = objOrder[0].SortBy;
            }

            //int PageFrom = (Convert.ToInt16(RowsPerPage) * (Convert.ToInt16(PageNo) - 1)) + 1;
            //int PageTo = Convert.ToInt16(RowsPerPage) * Convert.ToInt16(PageNo);


            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                StringBuilder SQLSelect = new StringBuilder();

                SQLSelect.Append(@"  select statusFinal as status,* from ( select ROW_NUMBER() OVER(ORDER BY load_inquiry_no) AS SrNo,* from ( 
                    select  res_inner.Total_PT_charge,res_inner.Total_PT_Discount,res_inner.PT_SizeTypeCode,res_inner.Total_CL_Charge,res_inner.Total_CL_Discount,
                    res_inner.CL_SizeTypeCode,res_inner.Total_PEST_Charge,res_inner.Total_PEST_Discount,res_inner.PEST_SizeTypeCode,(
                    select top 1 mover_Name from order_driver_truck_details join mover_mst on mover_mst.mover_id=order_driver_truck_details.mover_id where load_inquiry_no=lstorder.load_inquiry_no) as mover_name,driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,quote_hdr.GeneratedPdfLink as cbm_GeneratedPdfLink,truckdetails.*,quote_hdr.total_cbm,quote_hdr.StatusFlag,status1 as statusFinal,
                    feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,(SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,lstorder.* 
                    from 
                    (
                    select distinct CASE WHEN order_cancellation_details.cancellation_id IS not NULL THEN '25'
                                    --WHEN order_reschedule_req_details.RescheduleReq_id IS not NULL and order_reschedule_req_details.active_flag='N' THEN '26' 
                                    else result.status
                    END AS status1,order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck, order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.*,
                    (SELECT   top(1)  truck_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_truck_id,(SELECT   top(1)  driver_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_id
                        from 
                        ( 
                        		select    SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id1' OrderKey  
                        		from orders    
                        		JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
                        		join user_mst on user_mst.unique_id =  orders.shipper_id
                        		join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
                        		Where 1=1 and orders.active_flag  = 'Y' 
                        		and orders.shippingdatetime>getdate()  
                        		UNION ALL 
                        		select    SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id2' OrderKey from orders  
                        		JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
                        		join user_mst on user_mst.unique_id =  orders.shipper_id
                        		join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
                        		Where 1=1 and orders.active_flag  = 'Y'
                        		and orders.shippingdatetime<getdate() 
                        ) result 	  
                        LEFT OUTER JOIN 	
                        ( 
                        	select 	load_inquiry_no,COUNT(*) as NoOfAssignedDriver, COUNT(*) as NoOfAssignedTruck, SUM(CAST(NoOfHandiman AS int)) as NoOfAssignedHandiman, SUM(CAST(NoOfLabour AS int)) as NoOfAssignedLabour
                        	from  order_driver_truck_details 
                        	group by load_inquiry_no
                        ) as order_driver_truck_details_summary	  
                        ON result.load_inquiry_no = order_driver_truck_details_summary.load_inquiry_no
                        LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no 
                        --  LEFT OUTER JOIN order_reschedule_req_details ON result.load_inquiry_no = order_reschedule_req_details.load_inquiry_no 
                    ) as lstorder  
                    left join 
                    (
                        SELECT distinct truck_mst.truck_id,truck_mst.body_type,truck_mst.truck_model,truck_mst.truck_make_id
                        ,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, 
                        truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, 
                        truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo 
                        from truck_mst 
                        left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id 
                        left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
                        left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id 
                        left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id 
                        left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id
                    ) as truckdetails on lstorder.driver_truck_id=truckdetails.truck_id
                    left join driver_mst on lstorder.driver_id = driver_mst.driver_id 
                    left join feedback_mst on lstorder.load_inquiry_no =  Feedback_mst.load_inquiry_no
                    LEFT OUTER JOIN quote_hdr ON lstorder.load_inquiry_no = quote_hdr.quote_id 
                    left join(
        	            select * from(
                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge AS Varchar(50)) as ServiceCharge 
                            from order_AddonService_details
                        union
                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceCharge 
                            from order_AddonService_details
                        union
                            select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizeTypeMst.SizeTypeDesc 
                            from order_AddonService_details join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                        )res
                        PIVOT(
                            MAX(ServiceCharge) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                        )pivot2
                    )res_inner on res_inner.load_inquiry_no = lstorder.load_inquiry_no
                    ) as tblfinal
                    ) as SOD  where  1=1 ");


                if (status != "")
                {
                    if (status == "45")
                        SQLSelect.Append(" and statusfinal in ('45') OR (IsCancel='Y') ");
                    else if (status == "10")
                        SQLSelect.Append(" and statusfinal not in ('02','25','45') ");
                    else
                        SQLSelect.Append("and statusfinal = " + status + " and  IsCancel='N' ");
                    //SQLSelect.Append(" and status1 = " + status + " ");
                }
                else
                {
                    SQLSelect.Append(" and statusfinal not in ('45') and  IsCancel='N' ");
                }

                if (username != "")
                {
                    SQLSelect.Append(" and tblfinal.username like'%" + username + "%'");
                }
                if (order_type_flag != "")
                {
                    SQLSelect.Append(" and tblfinal.order_type_flag= '" + order_type_flag + "'");
                }

                if (OrderBy == "M")
                {
                    if (fromdate.Trim() != "" && Todate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                    }
                    else if (fromdate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) ");
                    }
                    else if (Todate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                    }
                }
                else
                {
                    if (fromdate.Trim() != "" && Todate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.created_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) and CONVERT(date,tblfinal.created_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                    }
                    else if (fromdate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.created_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) ");
                    }
                    else if (Todate.Trim() != "")
                    {
                        SQLSelect.Append(" and CONVERT(date,tblfinal.created_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                    }
                }

                if (sourcecity.Trim() != "" && destinationcity.Trim() != "")
                {
                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%' and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
                }
                else if (sourcecity.Trim() != "")
                {
                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%'");
                }
                else if (destinationcity.Trim() != "")
                    SQLSelect.Append(" and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
                if (Assigndriver != "")
                    SQLSelect.Append(" and  tblfinal.isassign_driver_truck = '" + Assigndriver + "' ");
                else if (Assigndriver != "" && OrderBy == "")
                    SQLSelect.Append(" and  tblfinal.isassign_driver_truck = '" + Assigndriver + "' order by shippingdatetime " + SortBy);



                if (OrderBy != "")
                {
                    if (OrderBy == "M")
                        SQLSelect.Append(" order by shippingdatetime " + SortBy);
                    else
                        SQLSelect.Append(" order by created_date " + SortBy);
                }

                if (Assigndriver == "" && OrderBy == "" && SortBy != "")
                    SQLSelect.Append("  order by shippingdatetime asc ");

                //SQLSelect.Append(" and SrNo BETWEEN " + PageFrom + @" AND " + PageTo + @"  order by SrNo,OrderKey,shippingdatetime asc");

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtPostLoadOrders = ds.Tables[0];
                }
                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                else
                    return BLGeneralUtil.return_ajax_string("0", " Order Details Not found "); ;
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpPost]
        public string CompleteOrderByadmin([FromBody]JObject ord)
        {
            #region Validation

            if (ord["complete_orders"][0]["load_inquiry_no"] == null || ord["complete_orders"][0]["load_inquiry_no"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Provide Inquiry ID");
            }


            #endregion

            string loadinqid = ""; //string trukid = "";
            DataTable dt_notification = new DataTable();
            Master master = new Master(); string ownid = "";
            DataSet dsdrv = new DataSet();
            string Invoicelink = ConfigurationManager.AppSettings["InvoicePdfLink"].ToString();

            try
            {

                BLReturnObject objBLobj = new BLReturnObject();
                //DS_driver_order_notifications ds_notification = new DS_driver_order_notifications();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                DS_orders dsord = new DS_orders();
                Document objdoc = new Document(); String DocNtficID = ""; string message = "";
                int extrahrs = 0;
                List<orders> tord = new List<orders>();
                DataTable dt_DriverTruckdetails = new DataTable();
                if (ord["complete_orders"] != null)
                {
                    tord = ord["complete_orders"].ToObject<List<orders>>();
                    dsdrv = master.CreateDataSet(tord);
                }
                // get order details by load inquiry number from order table
                DataTable dt_order =objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");

                if (dt_order != null)
                {
                    string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    if (isassigndriver_truck == "N")
                    {
                    }
                    else
                    {
                        // get order details by load inquiry number from order table
                        dt_DriverTruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(tord[0].load_inquiry_no);
                        if (dt_DriverTruckdetails == null)
                            return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }
                }




                // get order details by load inquiry number from post_load_inquiry table
                DataTable dt_loadinq = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_loadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", " Load Inquiry details Not found");


                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["shipper_id"].ToString();
                    //  trukid = dt_order.Rows[0]["truck_id"].ToString();
                }

                //if (dt_loadinq != null && dt_loadinq.Rows.Count > 0)
                //{
                //    if (dt_loadinq.Rows[0]["load_unload_extra_hours"] != null && dt_loadinq.Rows[0]["load_unload_extra_hours"].ToString() != "")
                //        extrahrs = Convert.ToInt16(dt_loadinq.Rows[0]["load_unload_extra_hours"]);
                //}


                loadinqid = tord[0].load_inquiry_no.ToString();
                driverController objDriver = new driverController();

                DataTable dt_quote = objcntrlpostorder.GetLoadInquiryQuotationById(loadinqid, ownid);
                DataTable dt_truck_currentPO = objDriver.GetTruckCurrentPositionById(loadinqid);

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";


                if (dt_order != null)
                {
                    //string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    //if (isassigndriver_truck == "N")
                    //{
                    //}
                    //else
                    //{
                    #region Update Table post_load_inquiry  details for Complete Order

                    DS_Post_load_enquiry ds_postload = new DS_Post_load_enquiry();
                    if (dt_loadinq != null)
                    {
                        ds_postload.EnforceConstraints = false;
                        ds_postload.post_load_inquiry.ImportRow(dt_loadinq.Rows[0]);

                        if (tord[0].status.Trim() != "")
                            ds_postload.post_load_inquiry[0].status = tord[0].status;

                        if (tord[0].IsCancel == "Y")
                        {
                            ds_postload.post_load_inquiry[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        }

                        ds_postload.post_load_inquiry[0].modified_by = tord[0].modified_by;
                        ds_postload.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                        ds_postload.post_load_inquiry[0].modified_host = tord[0].modified_host;
                        ds_postload.post_load_inquiry[0].modified_device_id = tord[0].modified_device_id;
                        ds_postload.post_load_inquiry[0].modified_device_type = tord[0].modified_device_type;
                        ds_postload.EnforceConstraints = true;
                        objBLobj = master.UpdateTables(ds_postload.post_load_inquiry, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Table load_order_enquiry_quotation details for Complete Order

                    if (dt_quote != null && dt_quote.Rows.Count > 0)
                    {
                        #region Save Load Order Quote History

                        //objBLobj = SaveHistory(dt_quote, ref DBCommand);
                        //if (objBLobj.ExecutionStatus != 1)
                        //{
                        //    ServerLog.Log(objBLobj.ServerMessage.ToString());
                        //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        //}
                        #endregion

                        DS_load_order_quotation ds_quote = new DS_load_order_quotation();
                        ds_quote.EnforceConstraints = false;
                        ds_quote.load_order_enquiry_quotation.ImportRow(dt_quote.Rows[0]);
                        ds_quote.load_order_enquiry_quotation[0].status = tord[0].status;
                        ds_quote.load_order_enquiry_quotation[0].modified_by = tord[0].created_by;
                        ds_quote.load_order_enquiry_quotation[0].modified_date = System.DateTime.UtcNow;
                        ds_quote.load_order_enquiry_quotation[0].modified_host = tord[0].created_host;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_id = tord[0].modified_device_id;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_type = tord[0].modified_device_type;
                        objBLobj = master.UpdateTables(ds_quote.load_order_enquiry_quotation, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Data In Orders_driver_truck_details Table

                    DS_driver_order_notifications objds_driver_order_notifiation = new DS_driver_order_notifications();


                    if (dt_DriverTruckdetails != null)
                    {
                        for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                        {
                            objds_driver_order_notifiation.order_driver_truck_details.ImportRow(dt_DriverTruckdetails.Rows[i]);

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.ALLOCATED_BUT_NOT_STARTE)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[0].pickup_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.TRUCK_READY_FOR_PICKUP)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.lOADING_STARTED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }
                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.START)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.UNLOADING_COMPLETED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (tord[0].IsCancel == "Y")
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].active_flag = Constant.Flag_No;
                                objds_driver_order_notifiation.order_driver_truck_details[i].status = Constant.UNLOADING_COMPLETED;
                            }
                            if (tord[0].status.Trim() != "")
                            {
                                if (tord[0].status.Trim() == Constant.ALLOCATED_BUT_NOT_STARTE)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].pickup_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.lOADING_STARTED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].start_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_COMPLETED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].complete_by = "ADMIN";

                                objds_driver_order_notifiation.order_driver_truck_details[i]["status"] = tord[0].status;
                            }

                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_by"] = tord[0].created_by;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_date"] = System.DateTime.UtcNow;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_host"] = tord[0].created_host;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_id"] = tord[0].device_id;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_type"] = tord[0].device_type;

                            objBLobj = master.UpdateTables(objds_driver_order_notifiation.order_driver_truck_details, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }

                    #endregion

                    #region Save Truck Position to History Table on Order Completion

                    DS_Truck_current_location ds_truck = new DS_Truck_current_location();
                    ds_truck.EnforceConstraints = false;
                    if (tord[0].status == Constant.ORDER_COMPLETED)
                    {
                        #region Save Truck History


                        if (dt_truck_currentPO != null && dt_truck_currentPO.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_truck_currentPO.Rows.Count; i++)
                            {
                                ds_truck.truck_current_position_history.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position_history[i].AcceptChanges();
                                ds_truck.truck_current_position_history[i].SetAdded();
                                ds_truck.truck_current_position.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position[i].AcceptChanges();
                                ds_truck.truck_current_position[i].Delete();
                            }
                            if (ds_truck != null && ds_truck.truck_current_position_history != null && ds_truck.truck_current_position_history.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position_history, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", "Error while saving position history");
                                }
                            }

                            if (ds_truck != null && ds_truck.truck_current_position != null && ds_truck.truck_current_position.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                }

                #region Update Date In Orders Table

                if (dt_order != null)
                {
                    dsord.orders.ImportRow(dt_order.Rows[0]);

                    if (tord[0].IsCancel == "Y")
                    {
                        dsord.orders[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        dsord.orders[0].active_flag = Constant.FLAG_Y;
                        dsord.orders[0].IsCancel = Constant.FLAG_Y;
                    }
                    if (tord[0].status.Trim() != "")
                        dsord.orders.Rows[0]["status"] = tord[0].status;

                    dsord.orders.Rows[0]["Remark"] = tord[0].Remark;
                    dsord.orders.Rows[0]["modified_by"] = tord[0].created_by;
                    dsord.orders.Rows[0]["modified_date"] = System.DateTime.UtcNow;
                    dsord.orders.Rows[0]["modified_host"] = tord[0].created_host;
                    dsord.orders.Rows[0]["modified_device_id"] = tord[0].device_id;
                    dsord.orders.Rows[0]["modified_device_type"] = tord[0].device_type;
                    dsord.orders.Rows[0]["order_completion_date"] = System.DateTime.UtcNow;
                    if (tord[0].status != null && tord[0].status.Trim() != String.Empty && tord[0].status == Constant.ORDER_COMPLETED)
                    {
                        driverController objdriverController = new driverController();
                        int CreditDays = objdriverController.GetShipperCreditDays(ref DBCommand, dsord.orders[0].shipper_id, ref message);
                        if (CreditDays < 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        dsord.orders[0].order_completion_date = DateTime.Today;
                        DateTime payment_due_date = dsord.orders[0].order_completion_date.AddDays(CreditDays);
                        dsord.orders[0]["payment_due_date"] = payment_due_date;

                        bool bl = new MailerController().GenerateInvoiceMail(tord[0].load_inquiry_no);
                        string filepath = Invoicelink + "Invoicedetail_" + tord[0].load_inquiry_no + ".pdf";
                        dsord.orders.Rows[0]["invoice_pdf_link"] = filepath;
                    }

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                }
                #endregion

                #region update status in driver_mst

                if (dt_DriverTruckdetails != null)
                {
                    for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                    {
                        DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                        if (dt_DriverTruckdetails != null && dt_DriverTruckdetails.Rows.Count > 0)
                        {
                            ds_owner.driver_mst.ImportRow(dt_DriverTruckdetails.Rows[i]);
                            ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;

                            objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                }

                #endregion

                #region Delete Addon details
                if (tord[0].IsCancel == "Y")
                {
                    if (tord[0].AddOnTransactionIds.Trim() != "")
                    {
                        String[] strAddonTrnIds = tord[0].AddOnTransactionIds.Trim().Split(',');
                        if (strAddonTrnIds.Length > 0)
                        {
                            for (int i = 0; i < strAddonTrnIds.Length; i++)
                            {
                                DataTable dtaddondetails = new AdminController().SelectOrderAddonDetailsByTrnID(strAddonTrnIds[i]);
                                dtaddondetails = BLGeneralUtil.CheckDateTime(dtaddondetails);
                                try
                                {
                                    dsord.EnforceConstraints = false;
                                    dsord.order_AddonService_details.ImportRow(dtaddondetails.Rows[0]);
                                    dsord.order_AddonService_details.AcceptChanges();
                                    dsord.order_AddonService_details[0].Delete();

                                    dsord.EnforceConstraints = true;

                                    objBLobj = master.UpdateTables(dsord.order_AddonService_details, ref DBCommand);
                                    if (objBLobj.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLobj.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }
                            }

                        }
                    }
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                string strtitle = " Your Order No. " + tord[0].load_inquiry_no + " : Successfully Completed + Your Invoice ";
                string Msg = "Thank you..<br>Your order from  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been delivered successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_order.Rows[0]["load_inquiry_no"].ToString();
                string shippername = new PostOrderController().GetUserdetailsByID(dt_order.Rows[0]["shipper_id"].ToString());
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderCompletationMail(new PostOrderController().GetEmailByID(dt_order.Rows[0]["shipper_id"].ToString()), strtitle, shippername, Msg, tord[0].load_inquiry_no));
                if (result["status"].ToString() == "0")
                    ServerLog.Log("Error Sending Activation Email");

                //  ServerLog.SuccessLog("Driver notification Updated Inq Id = " + loadinqid);
                return BLGeneralUtil.return_ajax_string("1", "Order Completed Successfully ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        [HttpPost]
        public string CancelOrderByadminNew([FromBody]JObject ord)
        {
            #region Validation

            if (ord["cancel_orders"][0]["load_inquiry_no"] == null || ord["cancel_orders"][0]["load_inquiry_no"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Provide Inquiry ID");
            }


            #endregion

            string loadinqid = ""; //string trukid = "";
            DataTable dt_notification = new DataTable();
            Master master = new Master(); string ownid = "";
            DataSet dsdrv = new DataSet();

            try
            {

                BLReturnObject objBLobj = new BLReturnObject();
                //DS_driver_order_notifications ds_notification = new DS_driver_order_notifications();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                DS_orders dsord = new DS_orders();
                Document objdoc = new Document(); String DocNtficID = ""; string message = "";
                int extrahrs = 0;
                List<orders> tord = new List<orders>();
                DataTable dt_DriverTruckdetails = new DataTable();
                if (ord["cancel_orders"] != null)
                {
                    tord = ord["cancel_orders"].ToObject<List<orders>>();
                    dsdrv = master.CreateDataSet(tord);
                }
                // get order details by load inquiry number from order table
                DataTable dt_order = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");

                if (dt_order != null)
                {
                    string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    if (isassigndriver_truck == "N")
                    {
                    }
                    else
                    {
                        // get order details by load inquiry number from order table
                        //dt_DriverTruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(tord[0].load_inquiry_no);
                        //if (dt_DriverTruckdetails == null)
                        //    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }
                }




                // get order details by load inquiry number from post_load_inquiry table
                DataTable dt_loadinq = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_loadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", " Load Inquiry details Not found");


                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["shipper_id"].ToString();
                    //  trukid = dt_order.Rows[0]["truck_id"].ToString();
                }

                loadinqid = tord[0].load_inquiry_no.ToString();
                driverController objDriver = new driverController();

                DataTable dt_quote = objcntrlpostorder.GetLoadInquiryQuotationById(loadinqid, ownid);
                DataTable dt_truck_currentPO = objDriver.GetTruckCurrentPositionById(loadinqid);

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";


                if (dt_order != null)
                {

                    #region Update Table post_load_inquiry  details for Complete Order

                    DS_Post_load_enquiry ds_postload = new DS_Post_load_enquiry();
                    if (dt_loadinq != null)
                    {
                        ds_postload.EnforceConstraints = false;
                        ds_postload.post_load_inquiry.ImportRow(dt_loadinq.Rows[0]);

                        if (tord[0].IsCancel == Constant.FLAG_N)
                        {
                            ds_postload.post_load_inquiry[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        }

                        ds_postload.post_load_inquiry[0].modified_by = tord[0].modified_by;
                        ds_postload.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                        ds_postload.post_load_inquiry[0].modified_host = tord[0].modified_host;
                        ds_postload.post_load_inquiry[0].modified_device_id = tord[0].modified_device_id;
                        ds_postload.post_load_inquiry[0].modified_device_type = tord[0].modified_device_type;
                        ds_postload.EnforceConstraints = true;
                        objBLobj = master.UpdateTables(ds_postload.post_load_inquiry, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Data In Orders_driver_truck_details Table

                    DS_driver_order_notifications objds_driver_order_notifiation = new DS_driver_order_notifications();


                    if (dt_DriverTruckdetails != null)
                    {
                        for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                        {
                            objds_driver_order_notifiation.order_driver_truck_details.ImportRow(dt_DriverTruckdetails.Rows[i]);

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.ALLOCATED_BUT_NOT_STARTE)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[0].pickup_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.TRUCK_READY_FOR_PICKUP)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.lOADING_STARTED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }
                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.START)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.UNLOADING_COMPLETED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (tord[0].IsCancel == "Y")
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].active_flag = Constant.Flag_No;
                                objds_driver_order_notifiation.order_driver_truck_details[i].status = Constant.UNLOADING_COMPLETED;
                            }
                            if (tord[0].status.Trim() != "")
                            {
                                if (tord[0].status.Trim() == Constant.ALLOCATED_BUT_NOT_STARTE)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].pickup_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.lOADING_STARTED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].start_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_COMPLETED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].complete_by = "ADMIN";

                                objds_driver_order_notifiation.order_driver_truck_details[i]["status"] = tord[0].status;
                            }

                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_by"] = tord[0].created_by;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_date"] = System.DateTime.UtcNow;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_host"] = tord[0].created_host;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_id"] = tord[0].device_id;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_type"] = tord[0].device_type;

                            objBLobj = master.UpdateTables(objds_driver_order_notifiation.order_driver_truck_details, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }

                    #endregion

                    #region Save Truck Position to History Table on Order Completion

                    DS_Truck_current_location ds_truck = new DS_Truck_current_location();
                    ds_truck.EnforceConstraints = false;
                    if (tord[0].IsCancel == Constant.FLAG_Y)
                    {
                        #region Save Truck History


                        if (dt_truck_currentPO != null && dt_truck_currentPO.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_truck_currentPO.Rows.Count; i++)
                            {
                                ds_truck.truck_current_position_history.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position_history[i].AcceptChanges();
                                ds_truck.truck_current_position_history[i].SetAdded();
                                ds_truck.truck_current_position.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position[i].AcceptChanges();
                                ds_truck.truck_current_position[i].Delete();
                            }
                            if (ds_truck != null && ds_truck.truck_current_position_history != null && ds_truck.truck_current_position_history.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position_history, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", "Error while saving position history");
                                }
                            }

                            if (ds_truck != null && ds_truck.truck_current_position != null && ds_truck.truck_current_position.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                }

                #region Update Date In Orders Table

                if (dt_order != null)
                {
                    dsord.orders.ImportRow(dt_order.Rows[0]);

                    if (tord[0].IsCancel == "Y")
                    {
                        dsord.orders[0].active_flag = Constant.FLAG_Y;
                        dsord.orders[0].IsCancel = Constant.FLAG_Y;
                        dsord.orders[0].status = Constant.UNLOADING_COMPLETED;
                    }
                    else
                    {
                        dsord.orders[0].IsCancel = Constant.FLAG_N;
                        dsord.orders[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                    }

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                }
                #endregion

                #region update status in driver_mst


                if (dt_DriverTruckdetails != null)
                {
                    if (tord[0].IsCancel == Constant.FLAG_Y)
                    {
                        for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                        {
                            DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                            if (dt_DriverTruckdetails != null && dt_DriverTruckdetails.Rows.Count > 0)
                            {
                                ds_owner.driver_mst.ImportRow(dt_DriverTruckdetails.Rows[i]);
                                ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;

                                objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                                if (objBLobj.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                }
                            }
                        }
                    }
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                //string Msg = "Thank you..<br>Your order from  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been delivered successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_order.Rows[0]["load_inquiry_no"].ToString();
                //string shippername = new PostOrderController().GetUserdetailsByID(dt_order.Rows[0]["shipper_id"].ToString());
                //var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderCompletationMail(new PostOrderController().GetEmailByID(dt_order.Rows[0]["shipper_id"].ToString()), " Your Order has been delivered successfully (Order ID: " + tord[0].load_inquiry_no + ")", shippername, Msg, tord[0].load_inquiry_no));
                //if (result["status"].ToString() == "0")
                //    ServerLog.Log("Error Sending Activation Email");

                //  ServerLog.SuccessLog("Driver notification Updated Inq Id = " + loadinqid);
                string strstatus = "";

                if (tord[0].IsCancel == Constant.FLAG_Y)
                    strstatus = "Order Cancellation Request Accepted ";
                else
                    strstatus = "Order Cancellation Request Rejected  ";

                return BLGeneralUtil.return_ajax_string("1", strstatus);
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        [HttpPost]
        public string CompleteOrderByadminold([FromBody]JObject ord)
        {
            #region Validation

            if (ord["complete_orders"][0]["load_inquiry_no"] == null || ord["complete_orders"][0]["load_inquiry_no"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Provide Inquiry ID");
            }


            #endregion

            string loadinqid = ""; //string trukid = "";
            DataTable dt_notification = new DataTable();
            Master master = new Master(); string ownid = "";
            DataSet dsdrv = new DataSet();

            try
            {

                BLReturnObject objBLobj = new BLReturnObject();
                //DS_driver_order_notifications ds_notification = new DS_driver_order_notifications();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                DS_orders dsord = new DS_orders();
                Document objdoc = new Document(); String DocNtficID = ""; string message = "";
                int extrahrs = 0;
                List<orders> tord = new List<orders>();
                DataTable dt_DriverTruckdetails = new DataTable();
                if (ord["complete_orders"] != null)
                {
                    tord = ord["complete_orders"].ToObject<List<orders>>();
                    dsdrv = master.CreateDataSet(tord);
                }
                // get order details by load inquiry number from order table
                DataTable dt_order = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");

                if (dt_order != null)
                {
                    string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    if (isassigndriver_truck == "N")
                    {
                    }
                    else
                    {
                        // get order details by load inquiry number from order table
                        dt_DriverTruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(tord[0].load_inquiry_no);
                        if (dt_DriverTruckdetails == null)
                            return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }
                }




                // get order details by load inquiry number from post_load_inquiry table
                DataTable dt_loadinq = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_loadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", " Load Inquiry details Not found");


                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["shipper_id"].ToString();
                    //  trukid = dt_order.Rows[0]["truck_id"].ToString();
                }

                //if (dt_loadinq != null && dt_loadinq.Rows.Count > 0)
                //{
                //    if (dt_loadinq.Rows[0]["load_unload_extra_hours"] != null && dt_loadinq.Rows[0]["load_unload_extra_hours"].ToString() != "")
                //        extrahrs = Convert.ToInt16(dt_loadinq.Rows[0]["load_unload_extra_hours"]);
                //}


                loadinqid = tord[0].load_inquiry_no.ToString();
                driverController objDriver = new driverController();

                DataTable dt_quote = objcntrlpostorder.GetLoadInquiryQuotationById(loadinqid, ownid);
                DataTable dt_truck_currentPO = objDriver.GetTruckCurrentPositionById(loadinqid);

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";


                if (dt_order != null)
                {
                    //string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    //if (isassigndriver_truck == "N")
                    //{
                    //}
                    //else
                    //{
                    #region Update Table post_load_inquiry  details for Complete Order

                    DS_Post_load_enquiry ds_postload = new DS_Post_load_enquiry();
                    if (dt_loadinq != null)
                    {
                        ds_postload.EnforceConstraints = false;
                        ds_postload.post_load_inquiry.ImportRow(dt_loadinq.Rows[0]);

                        if (tord[0].status.Trim() != "")
                            ds_postload.post_load_inquiry[0].status = tord[0].status;

                        ds_postload.post_load_inquiry[0].modified_by = tord[0].modified_by;
                        ds_postload.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                        ds_postload.post_load_inquiry[0].modified_host = tord[0].modified_host;
                        ds_postload.post_load_inquiry[0].modified_device_id = tord[0].modified_device_id;
                        ds_postload.post_load_inquiry[0].modified_device_type = tord[0].modified_device_type;
                        ds_postload.EnforceConstraints = true;
                        objBLobj = master.UpdateTables(ds_postload.post_load_inquiry, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Table load_order_enquiry_quotation details for Complete Order

                    if (dt_quote != null && dt_quote.Rows.Count > 0)
                    {
                        #region Save Load Order Quote History

                        //objBLobj = SaveHistory(dt_quote, ref DBCommand);
                        //if (objBLobj.ExecutionStatus != 1)
                        //{
                        //    ServerLog.Log(objBLobj.ServerMessage.ToString());
                        //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        //}
                        #endregion

                        DS_load_order_quotation ds_quote = new DS_load_order_quotation();
                        ds_quote.EnforceConstraints = false;
                        ds_quote.load_order_enquiry_quotation.ImportRow(dt_quote.Rows[0]);
                        ds_quote.load_order_enquiry_quotation[0].status = tord[0].status;
                        ds_quote.load_order_enquiry_quotation[0].modified_by = tord[0].created_by;
                        ds_quote.load_order_enquiry_quotation[0].modified_date = System.DateTime.UtcNow;
                        ds_quote.load_order_enquiry_quotation[0].modified_host = tord[0].created_host;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_id = tord[0].modified_device_id;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_type = tord[0].modified_device_type;
                        objBLobj = master.UpdateTables(ds_quote.load_order_enquiry_quotation, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Data In Orders_driver_truck_details Table

                    DS_driver_order_notifications objds_driver_order_notifiation = new DS_driver_order_notifications();


                    if (dt_DriverTruckdetails != null)
                    {
                        for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                        {
                            objds_driver_order_notifiation.order_driver_truck_details.ImportRow(dt_DriverTruckdetails.Rows[i]);

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.ALLOCATED_BUT_NOT_STARTE)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[0].pickup_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.TRUCK_READY_FOR_PICKUP)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.lOADING_STARTED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }
                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.START)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.UNLOADING_COMPLETED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (tord[0].IsCancel == "Y")
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].active_flag = Constant.Flag_No;
                                objds_driver_order_notifiation.order_driver_truck_details[i].status = Constant.UNLOADING_COMPLETED;
                            }
                            if (tord[0].status.Trim() != "")
                            {
                                if (tord[0].status.Trim() == Constant.ALLOCATED_BUT_NOT_STARTE)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].pickup_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.lOADING_STARTED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].start_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_COMPLETED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].complete_by = "ADMIN";

                                objds_driver_order_notifiation.order_driver_truck_details[i]["status"] = tord[0].status;
                            }

                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_by"] = tord[0].created_by;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_date"] = System.DateTime.UtcNow;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_host"] = tord[0].created_host;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_id"] = tord[0].device_id;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_type"] = tord[0].device_type;

                            objBLobj = master.UpdateTables(objds_driver_order_notifiation.order_driver_truck_details, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }

                    #endregion

                    #region Save Truck Position to History Table on Order Completion

                    DS_Truck_current_location ds_truck = new DS_Truck_current_location();
                    ds_truck.EnforceConstraints = false;
                    if (tord[0].status == Constant.ORDER_COMPLETED)
                    {
                        #region Save Truck History


                        if (dt_truck_currentPO != null && dt_truck_currentPO.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_truck_currentPO.Rows.Count; i++)
                            {
                                ds_truck.truck_current_position_history.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position_history[i].AcceptChanges();
                                ds_truck.truck_current_position_history[i].SetAdded();
                                ds_truck.truck_current_position.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position[i].AcceptChanges();
                                ds_truck.truck_current_position[i].Delete();
                            }
                            if (ds_truck != null && ds_truck.truck_current_position_history != null && ds_truck.truck_current_position_history.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position_history, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", "Error while saving position history");
                                }
                            }

                            if (ds_truck != null && ds_truck.truck_current_position != null && ds_truck.truck_current_position.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                }

                #region Update Date In Orders Table
                string Invoicelink = ConfigurationManager.AppSettings["InvoicePdfLink"].ToString();

                if (dt_order != null)
                {
                    dsord.orders.ImportRow(dt_order.Rows[0]);

                    if (tord[0].IsCancel == "Y")
                    {
                        dsord.orders[0].active_flag = Constant.FLAG_Y;
                        dsord.orders[0].IsCancel = Constant.FLAG_Y;
                    }
                    if (tord[0].status.Trim() != "")
                        dsord.orders.Rows[0]["status"] = tord[0].status;
                    dsord.orders.Rows[0]["Remark"] = tord[0].Remark;
                    dsord.orders.Rows[0]["modified_by"] = tord[0].created_by;
                    dsord.orders.Rows[0]["modified_date"] = System.DateTime.UtcNow;
                    dsord.orders.Rows[0]["modified_host"] = tord[0].created_host;
                    dsord.orders.Rows[0]["modified_device_id"] = tord[0].device_id;
                    dsord.orders.Rows[0]["modified_device_type"] = tord[0].device_type;
                    dsord.orders.Rows[0]["order_completion_date"] = System.DateTime.UtcNow;
                    if (tord[0].status != null && tord[0].status.Trim() != String.Empty && tord[0].status == Constant.ORDER_COMPLETED)
                    {
                        driverController objdriverController = new driverController();
                        int CreditDays = objdriverController.GetShipperCreditDays(ref DBCommand, dsord.orders[0].shipper_id, ref message);
                        if (CreditDays < 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        dsord.orders[0].order_completion_date = DateTime.Today;
                        DateTime payment_due_date = dsord.orders[0].order_completion_date.AddDays(CreditDays);
                        dsord.orders[0]["payment_due_date"] = payment_due_date;

                        bool bl = new MailerController().GenerateInvoiceMail(tord[0].load_inquiry_no);
                        string filepath = Invoicelink + "Invoicedetail_" + tord[0].load_inquiry_no + ".pdf";
                        dsord.orders.Rows[0]["invoice_pdf_link"] = filepath;
                    }

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                }

                #endregion

                #region update status in driver_mst

                if (dt_DriverTruckdetails != null)
                {
                    for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                    {
                        DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                        if (dt_DriverTruckdetails != null && dt_DriverTruckdetails.Rows.Count > 0)
                        {
                            ds_owner.driver_mst.ImportRow(dt_DriverTruckdetails.Rows[i]);
                            ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;

                            objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                DateTime dubaitime = objcntrlpostorder.DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                string strtitle = " Your Order No. " + tord[0].load_inquiry_no + " : Successfully Completed + Your Invoice ";


                string Msg = "Thank you..<br>Your order from  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been delivered successfully " + dubaitime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_order.Rows[0]["load_inquiry_no"].ToString();
                string shippername = new PostOrderController().GetUserdetailsByID(dt_order.Rows[0]["shipper_id"].ToString());
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderCompletationMail(new PostOrderController().GetEmailByID(dt_order.Rows[0]["shipper_id"].ToString()), strtitle, shippername, Msg, tord[0].load_inquiry_no));
                if (result["status"].ToString() == "0")
                    ServerLog.Log("Error Sending Activation Email");

                //  ServerLog.SuccessLog("Driver notification Updated Inq Id = " + loadinqid);
                return BLGeneralUtil.return_ajax_string("1", "Order Completed Successfully ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        [HttpPost]
        public string CancelOrderByadmin([FromBody]JObject ord)
        {
            #region Validation

            if (ord["complete_orders"][0]["load_inquiry_no"] == null || ord["complete_orders"][0]["load_inquiry_no"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Provide Inquiry ID");
            }

            #endregion

            string loadinqid = ""; //string trukid = "";
            DataTable dt_notification = new DataTable();
            Master master = new Master(); string ownid = "";
            DataSet dsdrv = new DataSet();

            try
            {

                BLReturnObject objBLobj = new BLReturnObject();
                //DS_driver_order_notifications ds_notification = new DS_driver_order_notifications();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                DS_orders dsord = new DS_orders();
                Document objdoc = new Document(); String DocNtficID = ""; string message = "";
                int extrahrs = 0;
                List<orders> tord = new List<orders>();
                DataTable dt_DriverTruckdetails = new DataTable();
                if (ord["cancel_orders"] != null)
                {
                    tord = ord["cancel_orders"].ToObject<List<orders>>();
                    dsdrv = master.CreateDataSet(tord);
                }
                // get order details by load inquiry number from order table
                DataTable dt_order = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");

                if (dt_order != null)
                {
                    string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    if (isassigndriver_truck == "N")
                    {
                    }
                    else
                    {
                        // get order details by load inquiry number from order table
                        dt_DriverTruckdetails = new driverController().GetDriverTruckDetailsByinquiryId(tord[0].load_inquiry_no);
                        if (dt_DriverTruckdetails == null)
                            return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }
                }




                // get order details by load inquiry number from post_load_inquiry table
                DataTable dt_loadinq = objcntrlpostorder.GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_loadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", " Load Inquiry details Not found");


                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["shipper_id"].ToString();
                    //  trukid = dt_order.Rows[0]["truck_id"].ToString();
                }

                //if (dt_loadinq != null && dt_loadinq.Rows.Count > 0)
                //{
                //    if (dt_loadinq.Rows[0]["load_unload_extra_hours"] != null && dt_loadinq.Rows[0]["load_unload_extra_hours"].ToString() != "")
                //        extrahrs = Convert.ToInt16(dt_loadinq.Rows[0]["load_unload_extra_hours"]);
                //}


                loadinqid = tord[0].load_inquiry_no.ToString();
                driverController objDriver = new driverController();

                DataTable dt_quote = objcntrlpostorder.GetLoadInquiryQuotationById(loadinqid, ownid);
                DataTable dt_truck_currentPO = objDriver.GetTruckCurrentPositionById(loadinqid);

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";


                if (dt_order != null)
                {
                    //string isassigndriver_truck = dt_order.Rows[0]["isassign_driver_truck"].ToString();
                    //if (isassigndriver_truck == "N")
                    //{
                    //}
                    //else
                    //{
                    #region Update Table post_load_inquiry  details for Complete Order

                    DS_Post_load_enquiry ds_postload = new DS_Post_load_enquiry();
                    if (dt_loadinq != null)
                    {
                        ds_postload.EnforceConstraints = false;
                        ds_postload.post_load_inquiry.ImportRow(dt_loadinq.Rows[0]);

                        if (tord[0].status.Trim() != "")
                            ds_postload.post_load_inquiry[0].status = tord[0].status;

                        ds_postload.post_load_inquiry[0].modified_by = tord[0].modified_by;
                        ds_postload.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                        ds_postload.post_load_inquiry[0].modified_host = tord[0].modified_host;
                        ds_postload.post_load_inquiry[0].modified_device_id = tord[0].modified_device_id;
                        ds_postload.post_load_inquiry[0].modified_device_type = tord[0].modified_device_type;
                        ds_postload.EnforceConstraints = true;
                        objBLobj = master.UpdateTables(ds_postload.post_load_inquiry, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Table load_order_enquiry_quotation details for Complete Order

                    if (dt_quote != null && dt_quote.Rows.Count > 0)
                    {
                        #region Save Load Order Quote History

                        //objBLobj = SaveHistory(dt_quote, ref DBCommand);
                        //if (objBLobj.ExecutionStatus != 1)
                        //{
                        //    ServerLog.Log(objBLobj.ServerMessage.ToString());
                        //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        //}
                        #endregion

                        DS_load_order_quotation ds_quote = new DS_load_order_quotation();
                        ds_quote.EnforceConstraints = false;
                        ds_quote.load_order_enquiry_quotation.ImportRow(dt_quote.Rows[0]);
                        ds_quote.load_order_enquiry_quotation[0].status = tord[0].status;
                        ds_quote.load_order_enquiry_quotation[0].modified_by = tord[0].created_by;
                        ds_quote.load_order_enquiry_quotation[0].modified_date = System.DateTime.UtcNow;
                        ds_quote.load_order_enquiry_quotation[0].modified_host = tord[0].created_host;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_id = tord[0].modified_device_id;
                        ds_quote.load_order_enquiry_quotation[0].modified_device_type = tord[0].modified_device_type;
                        objBLobj = master.UpdateTables(ds_quote.load_order_enquiry_quotation, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    #endregion

                    #region Update Data In Orders_driver_truck_details Table

                    DS_driver_order_notifications objds_driver_order_notifiation = new DS_driver_order_notifications();


                    if (dt_DriverTruckdetails != null)
                    {
                        for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                        {
                            objds_driver_order_notifiation.order_driver_truck_details.ImportRow(dt_DriverTruckdetails.Rows[i]);

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.ALLOCATED_BUT_NOT_STARTE)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[0].pickup_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].pickup_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.TRUCK_READY_FOR_PICKUP)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.lOADING_STARTED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].start_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }
                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.START)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_time = System.DateTime.UtcNow;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (dt_DriverTruckdetails.Rows[i]["status"].ToString() == Constant.UNLOADING_COMPLETED)
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lat = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_lng = null;
                                objds_driver_order_notifiation.order_driver_truck_details[i].complete_time = System.DateTime.UtcNow;
                            }

                            if (tord[0].IsCancel == "Y")
                            {
                                objds_driver_order_notifiation.order_driver_truck_details[i].active_flag = Constant.Flag_No;
                                objds_driver_order_notifiation.order_driver_truck_details[i].status = Constant.UNLOADING_COMPLETED;
                            }
                            if (tord[0].status.Trim() != "")
                            {
                                if (tord[0].status.Trim() == Constant.ALLOCATED_BUT_NOT_STARTE)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].pickup_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.lOADING_STARTED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].loadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].start_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_START)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].unloadingstart_by = "ADMIN";
                                else if (tord[0].status.Trim() == Constant.UNLOADING_COMPLETED)
                                    objds_driver_order_notifiation.order_driver_truck_details[i].complete_by = "ADMIN";

                                objds_driver_order_notifiation.order_driver_truck_details[i]["status"] = tord[0].status;
                            }

                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_by"] = tord[0].created_by;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_date"] = System.DateTime.UtcNow;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_host"] = tord[0].created_host;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_id"] = tord[0].device_id;
                            objds_driver_order_notifiation.order_driver_truck_details[i]["modified_device_type"] = tord[0].device_type;

                            objBLobj = master.UpdateTables(objds_driver_order_notifiation.order_driver_truck_details, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }

                    #endregion

                    #region Save Truck Position to History Table on Order Completion

                    DS_Truck_current_location ds_truck = new DS_Truck_current_location();
                    ds_truck.EnforceConstraints = false;
                    if (tord[0].status == Constant.ORDER_COMPLETED)
                    {
                        #region Save Truck History


                        if (dt_truck_currentPO != null && dt_truck_currentPO.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_truck_currentPO.Rows.Count; i++)
                            {
                                ds_truck.truck_current_position_history.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position_history[i].AcceptChanges();
                                ds_truck.truck_current_position_history[i].SetAdded();
                                ds_truck.truck_current_position.ImportRow(dt_truck_currentPO.Rows[i]);
                                ds_truck.truck_current_position[i].AcceptChanges();
                                ds_truck.truck_current_position[i].Delete();
                            }
                            if (ds_truck != null && ds_truck.truck_current_position_history != null && ds_truck.truck_current_position_history.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position_history, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", "Error while saving position history");
                                }
                            }

                            if (ds_truck != null && ds_truck.truck_current_position != null && ds_truck.truck_current_position.Rows.Count > 0)
                            {
                                objBLobj = master.UpdateTables(ds_truck.truck_current_position, ref DBCommand);
                                if (objBLobj.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLobj.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                }

                #region Update Date In Orders Table

                if (dt_order != null)
                {
                    dsord.orders.ImportRow(dt_order.Rows[0]);

                    if (tord[0].IsCancel == "Y")
                    {
                        dsord.orders[0].active_flag = Constant.FLAG_Y;
                        dsord.orders[0].IsCancel = Constant.FLAG_Y;
                    }
                    if (tord[0].status.Trim() != "")
                        dsord.orders.Rows[0]["status"] = tord[0].status;

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                }

                #endregion

                #region update status in driver_mst

                if (dt_DriverTruckdetails != null)
                {
                    for (int i = 0; i < dt_DriverTruckdetails.Rows.Count; i++)
                    {
                        DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                        if (dt_DriverTruckdetails != null && dt_DriverTruckdetails.Rows.Count > 0)
                        {
                            ds_owner.driver_mst.ImportRow(dt_DriverTruckdetails.Rows[i]);
                            ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;

                            objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                }

                #endregion

                #region Delete Addon details
                if (tord[0].IsCancel == "Y")
                {
                    if (tord[0].AddOnTransactionIds.Trim() != "")
                    {
                        String[] strAddonTrnIds = tord[0].AddOnTransactionIds.Trim().Split(',');
                        if (strAddonTrnIds.Length > 0)
                        {
                            for (int i = 0; i < strAddonTrnIds.Length; i++)
                            {
                                DataTable dtaddondetails = new AdminController().SelectOrderAddonDetailsByInqID("", strAddonTrnIds[i]);
                                dtaddondetails = BLGeneralUtil.CheckDateTime(dtaddondetails);
                                try
                                {
                                    dsord.EnforceConstraints = false;
                                    dsord.order_AddonService_details.ImportRow(dtaddondetails.Rows[0]);
                                    dsord.order_AddonService_details.AcceptChanges();
                                    dsord.order_AddonService_details[0].Delete();

                                    dsord.EnforceConstraints = true;

                                    objBLobj = master.UpdateTables(dsord.order_AddonService_details, ref DBCommand);
                                    if (objBLobj.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLobj.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }
                            }

                        }
                    }
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                string Msg = "Thank you..<br>Your order from  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been delivered successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_order.Rows[0]["load_inquiry_no"].ToString();
                string shippername = new PostOrderController().GetUserdetailsByID(dt_order.Rows[0]["shipper_id"].ToString());
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderCompletationMail(new PostOrderController().GetEmailByID(dt_order.Rows[0]["shipper_id"].ToString()), " Your Order has been delivered successfully (Order ID: " + tord[0].load_inquiry_no + ")", shippername, Msg, tord[0].load_inquiry_no));
                if (result["status"].ToString() == "0")
                    ServerLog.Log("Error Sending Activation Email");


                //  ServerLog.SuccessLog("Driver notification Updated Inq Id = " + loadinqid);
                return BLGeneralUtil.return_ajax_string("1", " Order Completed Successfully ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }

        [HttpPost]
        public string GetHistoryOrderDetails([FromBody]JObject Jobj)
        {

            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = ""; string sourcecity = "";
            string destinationcity = ""; string owner_id = ""; string username = ""; string order_type_flag = "";
            string RecordNo = ""; string PageNo = ""; string Assigndriver = ""; string OrderBy = ""; string SortBy = "";
            objOrder = Jobj["order_deshboard"].ToObject<List<orders>>();
            if (Jobj["order_deshboard"] != null)
            {
                status = objOrder[0].status;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
                sourcecity = objOrder[0].inquiry_source_city;
                destinationcity = objOrder[0].inquiry_destination_city;
                owner_id = objOrder[0].owner_id;
                username = objOrder[0].username;
                order_type_flag = objOrder[0].order_type;
                Assigndriver = objOrder[0].Assigndriver;
                OrderBy = objOrder[0].OrderBy;
                SortBy = objOrder[0].SortBy;
            }


            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                StringBuilder SQLSelect = new StringBuilder();

                #region MyRegion


                //                SQLSelect.Append(@"select statusFinal as status,* from ( select  res_inner.Total_PT_charge,res_inner.Total_PT_Discount,res_inner.PT_SizeTypeCode,res_inner.Total_CL_Charge,res_inner.Total_CL_Discount,
                //                                            res_inner.CL_SizeTypeCode,res_inner.Total_PEST_Charge,res_inner.Total_PEST_Discount,res_inner.PEST_SizeTypeCode,(select top 1 mover_Name from order_driver_truck_details join mover_mst on mover_mst.mover_id=order_driver_truck_details.mover_id where load_inquiry_no=lstorder.load_inquiry_no) as mover_name,driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,quote_hdr.GeneratedPdfLink as cbm_GeneratedPdfLink,truckdetails.*,quote_hdr.total_cbm,quote_hdr.StatusFlag,status1 as statusFinal,
                //                                                        feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,
                //                                                        (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,lstorder.* from (
                //                                                                  select result.status AS status1,order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck,  
                //                                                                  order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.*,
                //                                                                  (SELECT   top(1)  truck_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_truck_id,
                //                                                                  (SELECT   top(1)  driver_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_id
                //                                                                  from 
                //                                                                  ( 
                //                                                                select    SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id1' OrderKey  
                //                                                                from orders    
                //                                                                JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
                //                                                                join user_mst on user_mst.unique_id =  orders.shipper_id
                //                                                                join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
                //                                                                Where 1=1 and orders.active_flag  = 'Y' 
                //                                                                ) result 	  
                //                                                                LEFT OUTER JOIN 	
                //                                                                ( 
                //                                                                    select 	load_inquiry_no,COUNT(*) as NoOfAssignedDriver, COUNT(*) as NoOfAssignedTruck, SUM(CAST(NoOfHandiman AS int)) as NoOfAssignedHandiman, SUM(CAST(NoOfLabour AS int)) as NoOfAssignedLabour
                //                                                                    from  order_driver_truck_details 
                //                                                                    group by load_inquiry_no
                //                                                                ) as order_driver_truck_details_summary	  
                //                                                                ON result.load_inquiry_no = order_driver_truck_details_summary.load_inquiry_no
                //                                                        ) as lstorder  
                //                                                        left join (
                //                                                                SELECT distinct truck_mst.truck_id,truck_mst.body_type,truck_mst.truck_model,truck_mst.truck_make_id
                //                                                                ,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, 
                //                                                                truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, 
                //                                                                truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo 
                //                                                                from truck_mst 
                //                                                                left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id 
                //                                                                left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
                //                                                                left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id 
                //                                                                left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id 
                //                                                                left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id
                //                                                        ) as truckdetails on lstorder.driver_truck_id=truckdetails.truck_id
                //                                                        left join driver_mst on lstorder.driver_id = driver_mst.driver_id 
                //                                                        left join feedback_mst on lstorder.load_inquiry_no =  Feedback_mst.load_inquiry_no
                //                                                        LEFT OUTER JOIN quote_hdr ON lstorder.load_inquiry_no = quote_hdr.quote_id 
                //                                                        left join(
                //        	                                                                select * from(
                //                                                                                      select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge AS Varchar(50)) as ServiceCharge 
                //                                                                                    from order_AddonService_details
                //                                                                                union
                //                                                                                    select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceCharge 
                //                                                                                    from order_AddonService_details
                //                                                                                union
                //                                                                                    select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizeTypeMst.SizeTypeDesc 
                //                                                                                    from order_AddonService_details join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                //                                                                            )res
                //                                                                            PIVOT(
                //                                                                                MAX(ServiceCharge) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                //                                                                            )pivot2
                //                                                                        )res_inner on res_inner.load_inquiry_no = lstorder.load_inquiry_no
                //                                                        ) as tblfinal
                //                                                        where  1=1");

                #endregion

                SQLSelect.Append(@" select statusFinal as status,* from ( select ROW_NUMBER() OVER(ORDER BY load_inquiry_no) AS SrNo,* from ( 
                                        select  res_inner.Total_PT_charge,res_inner.Total_PT_Discount,res_inner.PT_SizeTypeCode,res_inner.Total_CL_Charge,res_inner.Total_CL_Discount,
                                        res_inner.CL_SizeTypeCode,res_inner.Total_PEST_Charge,res_inner.Total_PEST_Discount,res_inner.PEST_SizeTypeCode,(
                                        select top 1 mover_Name from order_driver_truck_details join mover_mst on mover_mst.mover_id=order_driver_truck_details.mover_id where load_inquiry_no=lstorder.load_inquiry_no) as mover_name,
                                        driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,quote_hdr.GeneratedPdfLink as cbm_GeneratedPdfLink,quote_hdr.total_cbm,quote_hdr.StatusFlag,status1 as statusFinal,
                                        feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,(SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,lstorder.* 
                                        from 
                                        (
                                        select distinct CASE WHEN order_cancellation_details.cancellation_id IS not NULL THEN '25'
                                                        --WHEN order_reschedule_req_details.RescheduleReq_id IS not NULL and order_reschedule_req_details.active_flag='N' THEN '26' 
                                                        else result.status
                                        END AS status1,order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck, order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.*,
                                        (SELECT   top(1)  truck_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_truck_id,(SELECT   top(1)  driver_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_id
                                            from 
                                            ( 
			                                        select    orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover, orders.shipper_email_id, orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes,orders.owner_id, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,
    		                                        SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc,'id1' OrderKey  
    		                                        from orders    
    		                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
    		                                        join user_mst on user_mst.unique_id =  orders.shipper_id
    		                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
    		                                        Where 1=1 and orders.active_flag  = 'Y' 
    		                                        and orders.shippingdatetime>getdate()  
    		                                        UNION ALL 
    		                                        select     orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover, orders.shipper_email_id, orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes,orders.owner_id, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,
    		                                        SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc,'id2' OrderKey from orders  
    		                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
    		                                        join user_mst on user_mst.unique_id =  orders.shipper_id
    		                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
    		                                        Where 1=1 and orders.active_flag  = 'Y'
    		                                        and orders.shippingdatetime<getdate() 
                                            ) result 	  
                                            LEFT OUTER JOIN 	
                                            ( 
    	                                        select 	load_inquiry_no,COUNT(*) as NoOfAssignedDriver, COUNT(*) as NoOfAssignedTruck, SUM(CAST(NoOfHandiman AS int)) as NoOfAssignedHandiman, SUM(CAST(NoOfLabour AS int)) as NoOfAssignedLabour
    	                                        from  order_driver_truck_details 
    	                                        group by load_inquiry_no
                                            ) as order_driver_truck_details_summary	  
                                            ON result.load_inquiry_no = order_driver_truck_details_summary.load_inquiry_no
                                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no 
                                            --  LEFT OUTER JOIN order_reschedule_req_details ON result.load_inquiry_no = order_reschedule_req_details.load_inquiry_no 
                                        ) as lstorder  
                                        left join 
                                        (
                                            SELECT distinct truck_mst.truck_id,truck_mst.body_type,truck_mst.truck_model,truck_mst.truck_make_id
                                            ,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, 
                                            truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, 
                                            truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo 
                                            from truck_mst 
                                            left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id 
                                            left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
                                            left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id 
                                            left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id 
                                            left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id
                                        ) as truckdetails on lstorder.driver_truck_id=truckdetails.truck_id
                                        left join driver_mst on lstorder.driver_id = driver_mst.driver_id 
                                        left join feedback_mst on lstorder.load_inquiry_no =  Feedback_mst.load_inquiry_no
                                        LEFT OUTER JOIN quote_hdr ON lstorder.load_inquiry_no = quote_hdr.quote_id 
                                            left join(
                                                        select * from(
                                                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge AS Varchar(50)) as ServiceCharge 
                                                            from order_AddonService_details
                                                        union
                                                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceCharge 
                                                            from order_AddonService_details
                                                        union
                                                            select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizeTypeMst.SizeTypeDesc 
                                                            from order_AddonService_details join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                                                        )res
                                                        PIVOT(
                                                            MAX(ServiceCharge) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                                                        )pivot2
                                                    )res_inner on res_inner.load_inquiry_no = lstorder.load_inquiry_no
                                        ) as tblfinal
                                        where  1=1 ");

                if (status != "")
                {
                    if (status == "45")
                        SQLSelect.Append(" and (statusfinal in ('45') and (IsCancel='N')) ");
                    else
                        SQLSelect.Append("and IsCancel='Y' ");
                }
                else
                {
                    SQLSelect.Append(" and (statusfinal in ('45') OR (IsCancel='Y')) ");
                }

                if (username != null && username != "")
                {
                    SQLSelect.Append(" and tblfinal.username like'%" + username + "%'");
                }
                if (order_type_flag != "")
                {
                    SQLSelect.Append(" and tblfinal.order_type_flag= '" + order_type_flag + "'");
                }
                if (fromdate.Trim() != "" && Todate.Trim() != "")
                {
                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                }
                else if (fromdate.Trim() != "")
                {
                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) ");
                }
                else if (Todate.Trim() != "")
                {
                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
                }
                if (sourcecity.Trim() != "" && destinationcity.Trim() != "")
                {
                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%' and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
                }
                else if (sourcecity.Trim() != "")
                {
                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%'");
                }
                else if (destinationcity.Trim() != "")
                    SQLSelect.Append(" and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
                if (Assigndriver != "" && OrderBy != "" && SortBy != "")
                    SQLSelect.Append(" and  tblfinal.isassign_driver_truck = '" + Assigndriver + "' ");
                else if (Assigndriver != "" && OrderBy == "")
                    SQLSelect.Append(" and  tblfinal.isassign_driver_truck = '" + Assigndriver + "' order by shippingdatetime " + SortBy);
                else if (Assigndriver != "")
                    SQLSelect.Append(" and  tblfinal.isassign_driver_truck = '" + Assigndriver + "' order by shippingdatetime asc ");



                if (OrderBy != "")
                {
                    if (OrderBy == "M")
                        SQLSelect.Append(" ) as tblpg where 1=1 order by shippingdatetime " + SortBy);
                    else
                        SQLSelect.Append(" ) as tblpg where 1=1 order by created_date " + SortBy);
                }

                if (Assigndriver == "" && OrderBy == "" && SortBy != "")
                    SQLSelect.Append(" ) as tblpg where 1=1 order by shippingdatetime desc ");

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtPostLoadOrders = ds.Tables[0];
                }
                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJsonNew(dtPostLoadOrders));
                else
                    return BLGeneralUtil.return_ajax_string("0", "You have no orders yet "); ;
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpPost]
        public string SaveOrderRescheduleDetails([FromBody]JObject Jobj)
        {
            List<orders> objOrder = new List<orders>();
            DS_orders dsord = new DS_orders();
            DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();
            DS_driver_order_notifications DsdriverOrderNotifications = new DS_driver_order_notifications();
            Document objdoc = new Document(); String DocNtficID = ""; string message = "";
            BLReturnObject objBLobj = new BLReturnObject();
            Master master = new Master();
            DataSet dsorder = new DataSet(); DataTable dtparameter = new DataTable();
            TimeSpan Tsshippingtime; Document objDocument = new Document();

            if (Jobj["order"] != null)
            {
                objOrder = Jobj["order"].ToObject<List<orders>>();
                dsorder = master.CreateDataSet(objOrder);
                dtparameter = BLGeneralUtil.CheckDateTime(dsorder.Tables[0]);
            }

            DataTable dtdriverTruckDtl = new driverController().GetOrderDriverTruckDetails(objOrder[0].load_inquiry_no);
            DataTable dtorderdetail = objcntrlpostorder.GetLoadInquiryById(objOrder[0].load_inquiry_no);
            DataTable dt_ReRqorders = new PostOrderController().GetRescheduleRequestDetailsByInq(objOrder[0].load_inquiry_no);

            if (dtorderdetail != null)
            {
                try
                {
                    DBConnection.Open();
                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();
                    message = "";

                    string shipperEmail = objcntrlpostorder.GetEmailByID(dtorderdetail.Rows[0]["shipper_id"].ToString());
                    string shippername = new PostOrderController().GetUserdetailsByID(dtorderdetail.Rows[0]["shipper_id"].ToString());
                    DateTime rescheduleShippingDateTime = Convert.ToDateTime(Convert.ToDateTime(dtparameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + TimeSpan.Parse(dtparameter.Rows[0]["load_inquiry_shipping_time"].ToString()));

                    #region Update Date In Orders Table

                    dsord.orders.ImportRow(dtorderdetail.Rows[0]);
                    dsord.EnforceConstraints = false;
                    dsord.orders[0].load_inquiry_shipping_date = DateTime.Parse(dtparameter.Rows[0]["load_inquiry_shipping_date"].ToString());
                    dsord.orders[0].load_inquiry_shipping_time = TimeSpan.Parse(dtparameter.Rows[0]["load_inquiry_shipping_time"].ToString());
                    dsord.orders[0].shippingdatetime = rescheduleShippingDateTime;
                    dsord.orders[0].isassign_driver_truck = Constant.FLAG_N;
                    dsord.orders[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                    dsord.orders[0].isassign_mover = Constant.FLAG_N;
                    dsord.orders[0].modified_by = objOrder[0].created_by;
                    dsord.orders[0].modified_date = System.DateTime.UtcNow;
                    dsord.orders[0].modified_host = objOrder[0].created_host;
                    dsord.orders[0].modified_device_id = objOrder[0].device_id;
                    dsord.orders[0].modified_device_type = objOrder[0].device_type;
                    dsord.EnforceConstraints = true;

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }


                    dspostload.post_load_inquiry.ImportRow(dsord.orders.Rows[0]);
                    dspostload.EnforceConstraints = false;
                    dspostload.post_load_inquiry[0].modified_by = objOrder[0].created_by;
                    dspostload.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                    dspostload.post_load_inquiry[0].modified_host = objOrder[0].created_host;
                    dspostload.post_load_inquiry[0].modified_device_id = objOrder[0].device_id;
                    dspostload.post_load_inquiry[0].modified_device_type = objOrder[0].device_type;
                    dspostload.EnforceConstraints = true;

                    objBLobj = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    int index;

                    if (dtdriverTruckDtl != null)
                    {
                        for (int i = 0; i < dtdriverTruckDtl.Rows.Count; i++)
                        {
                            DsdriverOrderNotifications.order_driver_truck_details.ImportRow(dtdriverTruckDtl.Rows[i]);
                            DsdriverOrderNotifications.order_driver_truck_details[i].Delete();
                        }

                        objBLobj = master.UpdateTables(DsdriverOrderNotifications.order_driver_truck_details, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }


                    if (dt_ReRqorders != null)
                    {
                        dsord.EnforceConstraints = false;
                        dsord.order_reschedule_req_details.ImportRow(dt_ReRqorders.Rows[0]);
                        dsord.order_reschedule_req_details[0].active_flag = Constant.Flag_Yes;
                    }
                    else
                    {
                        if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "RR", "", "", ref DocNtficID, ref message))
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        dsord.EnforceConstraints = false;
                        dsord.order_reschedule_req_details.ImportRow(dtparameter.Rows[0]);
                        dsord.order_reschedule_req_details[0].RescheduleReq_id = DocNtficID;
                        dsord.order_reschedule_req_details[0].shipper_id = dtorderdetail.Rows[0]["shipper_id"].ToString();
                        dsord.order_reschedule_req_details[0].shippingDateTime = Convert.ToDateTime(dtorderdetail.Rows[0]["shippingdatetime"].ToString());
                        dsord.order_reschedule_req_details[0].RescheduleShippingDateTime = rescheduleShippingDateTime;
                        dsord.order_reschedule_req_details[0].active_flag = Constant.FLAG_Y;
                        dsord.order_reschedule_req_details[0].created_date = System.DateTime.UtcNow;
                        dsord.order_reschedule_req_details[0].created_by = "ADMIN";
                    }

                    dsord.EnforceConstraints = true;

                    objBLobj = master.UpdateTables(dsord.order_reschedule_req_details, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;

                    string strTitle = ConfigurationManager.AppSettings["SUBRESCH_CONF"];
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderRescheduleConfirmationEmail(shipperEmail, strTitle + " for (Order ID: " + objOrder[0].load_inquiry_no + ")", shippername, rescheduleShippingDateTime, objOrder[0].load_inquiry_no));
                    if (result["status"].ToString() == "0")
                    {
                        ServerLog.Log("Error Sending reschedule Email To User");
                    }

                    return BLGeneralUtil.return_ajax_string("1", " Order reschedule sucessfully ");

                    #endregion
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Please Provide order details");
        }

        [HttpPost]
        public string AddNotes([FromBody]JObject Jobj)
        {
            try
            {
                Master master = new Master();
                BLReturnObject objBLobj = new BLReturnObject();
                DS_orders dsord = new DS_orders();
                Document objdoc = new Document();string message = "";


                // get order details by load inquiry number from order table
                DataTable dt_order = objcntrlpostorder.GetLoadInquiryById(Jobj["load_inquiry_no"].ToString());
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found ");

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                
                #region Update Date In Orders Table

                if (dt_order != null)
                {
                    dsord.orders.ImportRow(dt_order.Rows[0]);

                    dsord.orders[0].Notes = Jobj["Note"].ToString();

                    objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                return BLGeneralUtil.return_ajax_string("1", " Note Added Successfully ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

        }


        [HttpGet]
        public string GetOrdersCountsOld(string fromdate, string todate)
        {
            DataTable dt_ordercount = new DataTable();
            StringBuilder SQLSelect = new StringBuilder();


            if (fromdate == null && todate == null)
            {
                SQLSelect.Append(@"select   GL_02+GL_10+GL_25+ GL_45+ GN_02+GN_10+ GN_25+ GN_45+ H_02+H_10+ H_25+ H_45 as TotalOrders,
                                                      H_02+H_10+ H_25+ H_45  as Moving_home,
                                                     GN_02+GN_10+ GN_25+ GN_45 as Goods_Now,
                                                     GL_02+GL_10+GL_25+ GL_45 as Goods_later,
                                                    GL_10+GN_10+H_10 as Totalongoing,GL_02+GN_02+H_02 as totalupcoming,
                                                    GL_45+GN_45+H_45 as totalcompleted,GL_25+GN_25+H_25 as totalcancel,
                                                    GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,
        	                            (select Count(*) from user_mst where role_id not in ('AD')) as Total_Users,
        	                            (select Count(*) from user_mst where role_id='SH')   as Shipper,
                                        (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' )   as totaldrivers,
                                        (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' )   as freedrivers, 
                                        (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='N' )   as busydrivers, 
        	                            (select Count(*) from truck_mst where active_flag='Y')   as totaltrucks 
        	                            from (
                                    select order_type_flag+'_'+status_final as order_type_flag_status,load_inquiry_no from (
                                    select   case 
        		                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        		                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,*
                                     from (
        		                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        		                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,result.created_date
        		                            from 
        		                            ( 
        			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        			                            from orders    
        			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        			                            join user_mst on user_mst.unique_id =  orders.shipper_id
        			                            Where 1=1 and orders.active_flag  = 'Y' 
        			                            and orders.shippingdatetime>getdate()  
        			                            UNION ALL 
        			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
        			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
        			                            join user_mst on user_mst.unique_id =  orders.shipper_id
        			                            Where 1=1 and orders.active_flag  = 'Y' 
        			                            and orders.shippingdatetime<getdate() 
        		                            ) result 	  
        		                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no
                                    ) tblfinal
                                    ) tbltemp 
                                    )TableToBePivot
                                    PIVOT
        	                            (
        	                            COUNT(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45) 
        	                            ) AS PivotedTable");
            }
            else
            {
                DateTime dt;
                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                if (!DateTime.TryParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid ToDate");


                SQLSelect.Append("select   GL_02+GL_10+GL_25+ GL_45+ GN_02+GN_10+ GN_25+ GN_45+ H_02+H_10+ H_25+ H_45 as TotalOrders,H_02+H_10+ H_25+ H_45  as Moving_home, ");
                SQLSelect.Append(" GN_02+GN_10+ GN_25+ GN_45 as Goods_Now, ");
                SQLSelect.Append("  GL_02+GL_10+GL_25+ GL_45 as Goods_later,GL_10+GN_10+H_10 as Totalongoing,GL_02+GN_02+H_02 as totalupcoming,GL_45+GN_45+H_45 as totalcompleted,GL_25+GN_25+H_25 as totalcancel, ");
                SQLSelect.Append(" GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,");
                SQLSelect.Append("(select Count(*) from user_mst where role_id not in ('AD') and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' ) as Total_Users, ");
                SQLSelect.Append("(select Count(*) from user_mst where role_id='SH' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as Shipper,");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaldrivers,");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as freedrivers, ");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as busydrivers, ");
                SQLSelect.Append("(select Count(*) from truck_mst where active_flag='Y' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaltrucks ");
                //SQLSelect.Append("(select count(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y' and CAST(orders.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as TotalOrders,");
                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='H' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as Moving_home,");
                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='GN' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as Goods_Now,");
                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='GL' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')  as Goods_later ");
                SQLSelect.Append("from ( ");
                SQLSelect.Append("select order_type_flag+'_'+status_final as order_type_flag_status,load_inquiry_no from (");
                SQLSelect.Append(@"select   case 
        		                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        		                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,*
                                        from (
        		                            select result.shippingdatetime,CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        		                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,result.created_date
        		                            from 
        		                            ( 
        			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        			                            from orders    
        			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        			                            join user_mst on user_mst.unique_id =  orders.shipper_id
        			                            Where 1=1 and orders.active_flag  = 'Y' 
        			                            and orders.shippingdatetime>getdate()  
        			                            UNION ALL 
        			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
        			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
        			                            join user_mst on user_mst.unique_id =  orders.shipper_id
        			                            Where 1=1 and orders.active_flag  = 'Y' 
        			                            and orders.shippingdatetime<getdate() 
        		                            ) result 	  
        		                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no
                                        ) tblfinal where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103)");
                //where CONVERT(VARCHAR(10),shippingdatetime,103)>=CONVERT(VARCHAR(10)," + fromdate + ",103) and   CONVERT(VARCHAR(10),shippingdatetime,103)<=CONVERT(VARCHAR(10)," + todate + ",103)");
                SQLSelect.Append(@") tbltemp 
                                        )TableToBePivot
                                        PIVOT
        	                                (
        	                                COUNT(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45) 
        	                                ) AS PivotedTable");
            }

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt_ordercount = ds.Tables[0];
            }
            if (dt_ordercount != null && dt_ordercount.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_ordercount));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        [HttpGet]
        public string GetOrdersCounts(string fromdate, string todate, string opt)
        {
            DataTable dt_ordercount = new DataTable();
            StringBuilder SQLSelect = new StringBuilder();

            #region Comment Code old



            //if (fromdate == null && todate == null)
            //{
            //                SQLSelect.Append(@"select   GL_02+GL_10+GL_25+ GL_45+ GN_02+GN_10+ GN_25+ GN_45+ H_02+H_10+ H_25+ H_45 as TotalOrders,
            //                                              H_02+H_10+ H_25+ H_45  as Moving_home,
            //                                             GN_02+GN_10+ GN_25+ GN_45 as Goods_Now,
            //                                             GL_02+GL_10+GL_25+ GL_45 as Goods_later,
            //                                            GL_10+GN_10+H_10 as Totalongoing,GL_02+GN_02+H_02 as totalupcoming,
            //                                            GL_45+GN_45+H_45 as totalcompleted,GL_25+GN_25+H_25 as totalcancel,
            //                                            GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,
            //	                            (select Count(*) from user_mst where role_id not in ('AD')) as Total_Users,
            //	                            (select Count(*) from user_mst where role_id='SH')   as Shipper,
            //                                (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' )   as totaldrivers,
            //                                (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' )   as freedrivers, 
            //                                (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='N' )   as busydrivers, 
            //	                            (select Count(*) from truck_mst where active_flag='Y')   as totaltrucks 
            //	                            from (
            //                            select order_type_flag+'_'+status_final as order_type_flag_status,load_inquiry_no from (
            //                            select   case 
            //		                            when final_status Is not null and final_status not in ('02','45','25') then '10'
            //		                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,*
            //                             from (
            //		                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
            //		                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,result.created_date
            //		                            from 
            //		                            ( 
            //			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
            //			                            from orders    
            //			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
            //			                            join user_mst on user_mst.unique_id =  orders.shipper_id
            //			                            Where 1=1 and orders.active_flag  = 'Y' 
            //			                            and orders.shippingdatetime>getdate()  
            //			                            UNION ALL 
            //			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
            //			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
            //			                            join user_mst on user_mst.unique_id =  orders.shipper_id
            //			                            Where 1=1 and orders.active_flag  = 'Y' 
            //			                            and orders.shippingdatetime<getdate() 
            //		                            ) result 	  
            //		                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no
            //                            ) tblfinal
            //                            ) tbltemp 
            //                            )TableToBePivot
            //                            PIVOT
            //	                            (
            //	                            COUNT(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45) 
            //	                            ) AS PivotedTable");
            //}
            //             else
            //            {
            //                DateTime dt;
            //                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            //                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

            //                if (!DateTime.TryParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            //                    return BLGeneralUtil.return_ajax_string("0", "Invalid ToDate");


            //                SQLSelect.Append("select   GL_02+GL_10+GL_25+ GL_45+ GN_02+GN_10+ GN_25+ GN_45+ H_02+H_10+ H_25+ H_45 as TotalOrders,H_02+H_10+ H_25+ H_45  as Moving_home, ");
            //                SQLSelect.Append(" GN_02+GN_10+ GN_25+ GN_45 as Goods_Now, ");
            //                SQLSelect.Append("  GL_02+GL_10+GL_25+ GL_45 as Goods_later,GL_10+GN_10+H_10 as Totalongoing,GL_02+GN_02+H_02 as totalupcoming,GL_45+GN_45+H_45 as totalcompleted,GL_25+GN_25+H_25 as totalcancel, ");
            //                SQLSelect.Append(" GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,");
            //                SQLSelect.Append("(select Count(*) from user_mst where role_id not in ('AD') and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' ) as Total_Users, ");
            //                SQLSelect.Append("(select Count(*) from user_mst where role_id='SH' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as Shipper,");
            //                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaldrivers,");
            //                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as freedrivers, ");
            //                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as busydrivers, ");
            //                SQLSelect.Append("(select Count(*) from truck_mst where active_flag='Y' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaltrucks ");
            //                //SQLSelect.Append("(select count(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y' and CAST(orders.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as TotalOrders,");
            //                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='H' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as Moving_home,");
            //                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='GN' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "') as Goods_Now,");
            //                //SQLSelect.Append("(select COUNT(*) from orders join user_mst on unique_id= orders.shipper_id and orders.active_flag  = 'Y'  and order_type_flag='GL' and CAST(orders.shippingdatetime AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(orders.shippingdatetime AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')  as Goods_later ");
            //                SQLSelect.Append("from ( ");
            //                SQLSelect.Append("select order_type_flag+'_'+status_final as order_type_flag_status,load_inquiry_no from (");
            //                SQLSelect.Append(@"select   case 
            //		                            when final_status Is not null and final_status not in ('02','45','25') then '10'
            //		                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,*
            //                                from (
            //		                            select result.shippingdatetime,CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
            //		                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,result.created_date
            //		                            from 
            //		                            ( 
            //			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
            //			                            from orders    
            //			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
            //			                            join user_mst on user_mst.unique_id =  orders.shipper_id
            //			                            Where 1=1 and orders.active_flag  = 'Y' 
            //			                            and orders.shippingdatetime>getdate()  
            //			                            UNION ALL 
            //			                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
            //			                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
            //			                            join user_mst on user_mst.unique_id =  orders.shipper_id
            //			                            Where 1=1 and orders.active_flag  = 'Y' 
            //			                            and orders.shippingdatetime<getdate() 
            //		                            ) result 	  
            //		                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no
            //                                ) tblfinal where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103)");
            //                //where CONVERT(VARCHAR(10),shippingdatetime,103)>=CONVERT(VARCHAR(10)," + fromdate + ",103) and   CONVERT(VARCHAR(10),shippingdatetime,103)<=CONVERT(VARCHAR(10)," + todate + ",103)");
            //                SQLSelect.Append(@") tbltemp 
            //                                )TableToBePivot
            //                                PIVOT
            //	                                (
            //	                                COUNT(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45) 
            //	                                ) AS PivotedTable");
            //            }

            #endregion

            if (fromdate == null && todate == null)
            {
                SQLSelect.Append(@"select *,
                                    (select Count(*) from user_mst where role_id not in ('AD')) as Total_Users,
                                    (select Count(*) from user_mst where role_id='SH')   as Shipper,
                                    (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' )   as totaldrivers,
                                    (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' )   as freedrivers, 
                                    (select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='N' )   as busydrivers, 
                                    (select Count(*) from truck_mst where active_flag='Y')   as totaltrucks 
                                     from (
                                    select GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total
                                    from
                                    --(select order_type_flag+'_'+status_final as order_type_flag_status,CAST(COUNT(load_inquiry_no) as varchar)+','+CAST(SUM(Total_cost) as varchar) as load_inquiry_no
                                    (
                                    select order_type_flag+'_'+status_final as order_type_flag_status, --order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    COUNT(load_inquiry_no) as load_inquiry_no--,SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result.created_date,result.total_cost
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result 	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no
        		                            ) tblfinal 
        	                            ) tbltemp 
                                    group by order_type_flag+'_'+status_final
                                    union all
                                    select order_type_flag+'_'+status_final+'_total'  as order_type_flag_status_total, --, order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result1.status ELSE '25' END AS status_can,
        			                            order_cancellation_details.cancellation_id,result1.load_inquiry_no, result1.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result1.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result1.created_date,result1.total_cost
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result1	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result1.load_inquiry_no = order_cancellation_details.load_inquiry_no
        		                            ) tblfinal1 
        	                            ) tbltemp1 
                                    group by order_type_flag+'_'+status_final
        
                                    )result2
                                    PIVOT
                                    (
        	                            MAX(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total) 
                                    ) AS PivotedTable1
                                    ) as finalResult ");
            }
            else if (fromdate != null && todate == null)
            {
                DateTime dt;
                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                SQLSelect.Append("select   * ");
                SQLSelect.Append("from ( ");
                SQLSelect.Append(@"select GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total
                                    from
                                    --(select order_type_flag+'_'+status_final as order_type_flag_status,CAST(COUNT(load_inquiry_no) as varchar)+','+CAST(SUM(Total_cost) as varchar) as load_inquiry_no
                                    (
                                    select order_type_flag+'_'+status_final as order_type_flag_status, --order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    COUNT(load_inquiry_no) as load_inquiry_no--,SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result.created_date,result.total_cost,result.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result 	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no ");

                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + fromdate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)<=CONVERT(date,'" + fromdate + "',103)  ");

                SQLSelect.Append(@"    ) tbltemp 
                                    group by order_type_flag+'_'+status_final
                                    union all
                                    select order_type_flag+'_'+status_final+'_total'  as order_type_flag_status_total, --, order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result1.status ELSE '25' END AS status_can,
        			                            order_cancellation_details.cancellation_id,result1.load_inquiry_no, result1.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result1.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result1.created_date,result1.total_cost,result1.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result1	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result1.load_inquiry_no = order_cancellation_details.load_inquiry_no ");


                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + fromdate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)<=CONVERT(date,'" + fromdate + "',103)  ");


                //SQLSelect.Append(" ) tblfinal1   where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103) ");
                SQLSelect.Append(@"    ) tbltemp1 
                                    group by order_type_flag+'_'+status_final
        
                                    )result2
                                    PIVOT
                                    (
        	                            MAX(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total) 
                                    ) AS PivotedTable1
                                    ) as finalResult ");
            }
            else if (fromdate == null && todate != null)
            {
                DateTime dt;
                if (!DateTime.TryParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                SQLSelect.Append("select   * ");
                SQLSelect.Append("from ( ");
                SQLSelect.Append(@"select GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total
                                    from
                                    --(select order_type_flag+'_'+status_final as order_type_flag_status,CAST(COUNT(load_inquiry_no) as varchar)+','+CAST(SUM(Total_cost) as varchar) as load_inquiry_no
                                    (
                                    select order_type_flag+'_'+status_final as order_type_flag_status, --order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    COUNT(load_inquiry_no) as load_inquiry_no--,SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result.created_date,result.total_cost,result.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result 	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no ");

                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + todate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)>=CONVERT(date,'" + todate + "',103)  ");

                SQLSelect.Append(@"    ) tbltemp 
                                    group by order_type_flag+'_'+status_final
                                    union all
                                    select order_type_flag+'_'+status_final+'_total'  as order_type_flag_status_total, --, order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result1.status ELSE '25' END AS status_can,
        			                            order_cancellation_details.cancellation_id,result1.load_inquiry_no, result1.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result1.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result1.created_date,result1.total_cost,result1.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result1	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result1.load_inquiry_no = order_cancellation_details.load_inquiry_no ");


                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + todate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)>=CONVERT(date,'" + todate + "',103)  ");


                //SQLSelect.Append(" ) tblfinal1   where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103) ");
                SQLSelect.Append(@"    ) tbltemp1 
                                    group by order_type_flag+'_'+status_final
        
                                    )result2
                                    PIVOT
                                    (
        	                            MAX(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total) 
                                    ) AS PivotedTable1
                                    ) as finalResult ");
            }
            else
            {
                DateTime dt;
                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                if (!DateTime.TryParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid ToDate");


                SQLSelect.Append("select   *,  ");
                //SQLSelect.Append(" GN_02+GN_10+ GN_25+ GN_45 as Goods_Now, ");
                //SQLSelect.Append("  GL_02+GL_10+GL_25+ GL_45 as Goods_later,GL_10+GN_10+H_10 as Totalongoing,GL_02+GN_02+H_02 as totalupcoming,GL_45+GN_45+H_45 as totalcompleted,GL_25+GN_25+H_25 as totalcancel, ");
                //SQLSelect.Append(" GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,");
                SQLSelect.Append("(select Count(*) from user_mst where role_id not in ('AD') and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "' ) as Total_Users, ");
                SQLSelect.Append("(select Count(*) from user_mst where role_id='SH' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as Shipper,");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaldrivers,");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as freedrivers, ");
                SQLSelect.Append("(select COUNT(*)  from driver_mst join user_mst on driver_mst.driver_id=user_mst.unique_id where user_mst.user_status_flag='A' and driver_mst.isfree='Y' and CAST(user_mst.created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(user_mst.created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as busydrivers, ");
                SQLSelect.Append("(select Count(*) from truck_mst where active_flag='Y' and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and  CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(todate.ToString(), "dd/mm/yyyy") + "')   as totaltrucks ");
                SQLSelect.Append("from ( ");
                SQLSelect.Append(@"select GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total
                                    from
                                    --(select order_type_flag+'_'+status_final as order_type_flag_status,CAST(COUNT(load_inquiry_no) as varchar)+','+CAST(SUM(Total_cost) as varchar) as load_inquiry_no
                                    (
                                    select order_type_flag+'_'+status_final as order_type_flag_status, --order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    COUNT(load_inquiry_no) as load_inquiry_no--,SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status_can,order_cancellation_details.cancellation_id,result.load_inquiry_no, result.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result.created_date,result.total_cost,result.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result 	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no ");

                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,created_date,103)<=CONVERT(date,'" + todate + "',103) ");

                SQLSelect.Append(@"    ) tbltemp 
                                    group by order_type_flag+'_'+status_final
                                    union all
                                    select order_type_flag+'_'+status_final+'_total'  as order_type_flag_status_total, --, order_type_flag+'_'+status_final+'_total' as order_type_flag_status_total,
                                    SUM(Total_cost) as total_cost
                                    from (
        	                            select   case 
        	                            when final_status Is not null and final_status not in ('02','45','25') then '10'
        	                            when final_status Is not null and status_can !='25' then final_status else status_can end as status_final,* 
        	                            from (
        			                            select  CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result1.status ELSE '25' END AS status_can,
        			                            order_cancellation_details.cancellation_id,result1.load_inquiry_no, result1.order_type_flag,
        			                            (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result1.load_inquiry_no) 
        			                            ORDER BY CAST(status AS int) desc ) as final_status,result1.created_date,result1.total_cost,result1.shippingdatetime
        			                            from 
        			                            ( 
        				                            select  user_mst.first_name as  username,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
        				                            from orders    
        				                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        				                            join user_mst on user_mst.unique_id =  orders.shipper_id
        				                            Where 1=1 and orders.active_flag  = 'Y' and IsCancel='N'
        			                            ) result1	  
        			                            LEFT OUTER JOIN order_cancellation_details ON result1.load_inquiry_no = order_cancellation_details.load_inquiry_no ");


                if (opt == "SH")
                    SQLSelect.Append(") tblfinal  where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103) ");
                else
                    SQLSelect.Append(") tblfinal  where CONVERT(date,created_date,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,created_date,103)<=CONVERT(date,'" + todate + "',103) ");


                //SQLSelect.Append(" ) tblfinal1   where CONVERT(date,shippingdatetime,103)>=CONVERT(date,'" + fromdate + "',103) and   CONVERT(date,shippingdatetime,103)<=CONVERT(date,'" + todate + "',103) ");
                SQLSelect.Append(@"    ) tbltemp1 
                                    group by order_type_flag+'_'+status_final
        
                                    )result2
                                    PIVOT
                                    (
        	                            MAX(load_inquiry_no) FOR order_type_flag_status IN (GL_02,GL_10, GL_25, GL_45, GN_02,GN_10, GN_25, GN_45, H_02,H_10, H_25, H_45,HT_02,HT_10, HT_25, HT_45,HT_45_total, HT_02_total,HT_10_total, HT_25_total,GL_02_total,GL_10_total, GL_25_total, GL_45_total, GN_02_total,GN_10_total, GN_25_total, GN_45_total, H_02_total,H_10_total, H_25_total, H_45_total) 
                                    ) AS PivotedTable1
                                    ) as finalResult ");

            }

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt_ordercount = ds.Tables[0];
            }
            if (dt_ordercount != null && dt_ordercount.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_ordercount));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        [HttpGet]
        public string GetQuoteRequestCounts(string fromdate, string todate)
        {
            DataTable dt_ordercount = new DataTable();
            StringBuilder SQLSelect = new StringBuilder();

            if (fromdate == null && todate == null)
            {
                SQLSelect.Append(@" select Order_type_flag,Count(load_inquiry_no) as ReqCount
                                        from (
        	                                select user_mst.first_name,user_mst.user_id,post_load_inquiry.load_inquiry_no,post_load_inquiry.inquiry_source_addr,post_load_inquiry.Total_cost,
        	                                post_load_inquiry.Order_type_flag,post_load_inquiry.created_date,post_load_inquiry.shippingdatetime,post_load_inquiry.payment_mode,post_load_inquiry.status,
        	                                post_load_inquiry.active_flag,MONTH(post_load_inquiry.shippingdatetime)as shipping_month,YEAR(post_load_inquiry.shippingdatetime)as shipping_year 
        	                                from post_load_inquiry 
        	                                join user_mst on user_mst.unique_id = post_load_inquiry.shipper_id
        	                                where shipper_id !='-9999'  and user_id not in
        	                                ( 'admin' ,'524546471','1112345555','222222222','234678457','506517322','524693690','555429527','555791228','555791229','558643191',
                                        '559776001','566163740','566960082','7226006454','7383607357','7567258257','8320599411','9624008877','9879006454','3333333333'	)
                                        -- and post_load_inquiry.created_date between '2016/10/01' and '2016/11/01' 
                                        )
                                        as temp 
                                        group by Order_type_flag");
            }
            else if (fromdate != null && todate == null)
            {
                DateTime dt;
                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                SQLSelect.Append(@" select Order_type_flag,Count(load_inquiry_no) as ReqCount
                                        from (
        	                                select user_mst.first_name,user_mst.user_id,post_load_inquiry.load_inquiry_no,post_load_inquiry.inquiry_source_addr,post_load_inquiry.Total_cost,
        	                                post_load_inquiry.Order_type_flag,post_load_inquiry.created_date,post_load_inquiry.shippingdatetime,post_load_inquiry.payment_mode,post_load_inquiry.status,
        	                                post_load_inquiry.active_flag,MONTH(post_load_inquiry.shippingdatetime)as shipping_month,YEAR(post_load_inquiry.shippingdatetime)as shipping_year 
        	                                from post_load_inquiry 
        	                                join user_mst on user_mst.unique_id = post_load_inquiry.shipper_id
        	                                where shipper_id !='-9999'  and user_id not in
        	                                ( 'admin' ,'524546471','1112345555','222222222','234678457','506517322','524693690','555429527','555791228','555791229','558643191',
                                        '559776001','566163740','566960082','7226006454','7383607357','7567258257','8320599411','9624008877','9879006454','3333333333'	) ");
                SQLSelect.Append(" and CONVERT(date,post_load_inquiry.created_date,103)<=CONVERT(date,'" + fromdate + "',103) ");
                SQLSelect.Append(" ) as temp group by Order_type_flag");

            }
            else
            {
                DateTime dt;
                if (!DateTime.TryParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid FromDate");

                if (!DateTime.TryParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    return BLGeneralUtil.return_ajax_string("0", "Invalid ToDate");


                SQLSelect.Append(@" select Order_type_flag,Count(load_inquiry_no) as ReqCount
                                        from (
        	                                select user_mst.first_name,user_mst.user_id,post_load_inquiry.load_inquiry_no,post_load_inquiry.inquiry_source_addr,post_load_inquiry.Total_cost,
        	                                post_load_inquiry.Order_type_flag,post_load_inquiry.created_date,post_load_inquiry.shippingdatetime,post_load_inquiry.payment_mode,post_load_inquiry.status,
        	                                post_load_inquiry.active_flag,MONTH(post_load_inquiry.shippingdatetime)as shipping_month,YEAR(post_load_inquiry.shippingdatetime)as shipping_year 
        	                                from post_load_inquiry 
        	                                join user_mst on user_mst.unique_id = post_load_inquiry.shipper_id
        	                                where shipper_id !='-9999'  and user_id not in
        	                                ( 'admin' ,'524546471','1112345555','222222222','234678457','506517322','524693690','555429527','555791228','555791229','558643191',
                                        '559776001','566163740','566960082','7226006454','7383607357','7567258257','8320599411','9624008877','9879006454','3333333333'	) ");
                SQLSelect.Append(" and post_load_inquiry.created_date between CONVERT(date,'" + fromdate + "',103) and CONVERT(date,'" + todate + "',103) ");
                SQLSelect.Append(" ) as temp group by Order_type_flag");

            }

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt_ordercount = ds.Tables[0];
            }
            if (dt_ordercount != null && dt_ordercount.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_ordercount));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        #endregion

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


        #region Driver Services

        [HttpGet]
        public string GetDeviceInfo()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @"   select user_mst.first_name,deviceinfo.* from deviceinfo
                                   left join user_mst on  user_mst.unique_id= deviceinfo.uniqueid 
                                   where uniqueid like '%D%'  order by registerdatetime desc ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }

        // GET api/admin/5
        public string GetDriverTruckDetails(string loadinqid, string driverid, string opt)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @"SELECT * FROM ";

            if (opt == "H")
                query1 += " truck_current_position_history Where 1=1 ";
            else
                query1 += " truck_current_position Where 1=1 ";

            if (loadinqid != "")
                query1 += "and load_inquiry_no = @inqid ";
            if (driverid != "")
                query1 += "and driver_id=@driverid";

            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, driverid));

            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Driver Language details not found "));
        }

        public string GetQueryDetails(string strquery)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = strquery;

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        #endregion
    }
}

