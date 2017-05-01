using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using trukkerUAE.Models;
using BLL.Utilities;
using BLL.Master;
using trukkerUAE.XSD;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json.Linq;
using trukkerUAE.Classes;
using System.ComponentModel;
using System.Globalization;
using trukkerUAE.BLL.Master;
using TrkrLite.Controllers;
using System.Configuration;


namespace trukkerUAE.Controllers
{
    public class PostOrderController : ServerBase
    {
        StringBuilder sb = new StringBuilder();
        String message = "";

        // GET api/postorder
        #region Get Methods


        [HttpGet]
        public DataTable GetLoadInquiryQuotationById(string id, string ownid)
        {
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            if (ownid == "-1")
                query1 = "SELECT * FROM load_order_enquiry_quotation where load_inquiry_no = @inqid and active_flag= 'Y'";
            else
                query1 = "SELECT * FROM load_order_enquiry_quotation where load_inquiry_no = @inqid and active_flag= 'Y' and owner_id = @owid  ";
            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, id));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("owid", DbType.String, ownid));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;
        }

        [HttpGet]
        public DataTable GetLoadInquiryById(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM orders  where load_inquiry_no = '" + loadinqno + "' and active_flag='Y'";
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

        [HttpGet]
        public DataTable GetLoadInquiryTempById(string shipperid, string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            // This method will bring all temporary load inquiry which is not converted to actual inquiry 
            // '01'  = temporary inquiry 
            String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            if (loadinqno != "-1")
            {
                query1 = "SELECT * FROM post_load_inquiry_temp  where load_inquiry_no = @inqid and active_flag='Y' AND status = '01' AND shipper_id = @shiprid and ref_inq_no is null order by load_inquiry_date desc";
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("shiprid", DbType.String, shipperid));
            }
            else
            {
                query1 = "SELECT * FROM post_load_inquiry_temp Where active_flag='Y' and shipper_id = @shiprid AND status = '01' and ref_inq_no is null order by load_inquiry_date desc ";
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("shiprid", DbType.String, shipperid));
            }

            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;
        }

        public decimal GetDeliveryDays(decimal aprx_kms)
        {
            decimal Aproxdays = 0;
            decimal totalhours = 0;
            totalhours = aprx_kms / 30; // Considered 30 kms per hour 
            Aproxdays = totalhours / 12;  // Usaually driver works for 12 hours a day  - It will give actual days 
            Aproxdays = BLGeneralUtil.GetRoundedValue(Constant.ROUNDING_NEAREST_RUPEE, Aproxdays);
            return Aproxdays;
        }


        [HttpGet]
        public string GetOnGoingOrders(string drvid, string deviceid)
        {

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(drvid);
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != deviceid)
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }
            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = "";
                query1 = " Select distinct user_mst.first_name as shipper_name,user_mst.user_id as shipper_mobile,user_mst.email_id as shipper_email,order_driver_truck_details.*,orders.*,truck_mst.*,driver_license_detail.*,driver_contact_detail.*,getdate() as currentdate,    " +
                        " post_load_inquiry.payment_mode,case when order_driver_truck_details.status not in(02) then 'Y' else 'N' end as Isongoing " +
                        " from orders " +
                       " left join user_mst on user_mst.unique_id = orders.shipper_id " +
                       "  left join order_driver_truck_details on order_driver_truck_details.load_inquiry_no = orders.load_inquiry_no " +
                       " left join truck_mst on truck_mst.truck_id = order_driver_truck_details.truck_id " +
                       " left join driver_license_detail on driver_license_detail.driver_id = order_driver_truck_details.driver_id " +
                       " left join driver_contact_detail on driver_contact_detail.driver_id = order_driver_truck_details.driver_id " +
                       " left join truck_make_mst on truck_make_mst.make_id = truck_mst.truck_make_id " +
                       " left join truck_model_mst on truck_model_mst.model_id = truck_mst.truck_model " +
                       " left join post_load_inquiry on post_load_inquiry.load_inquiry_no = orders.load_inquiry_no " +
                     "   Where orders.active_flag  = 'Y' and order_driver_truck_details.driver_id=@drid   and CAST(order_driver_truck_details.status AS INT) not in (45,-01)    Order by orders.load_inquiry_no desc ";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("drid", DbType.String, drvid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtPostLoadOrders = ds.Tables[0];
                }
                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
                }
                else
                {
                    return (BLGeneralUtil.return_ajax_string("0", "No inquiry for driver id = " + drvid));
                }
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }
        public string GetOwnerMobileNoByOwnID(string ownerid)
        {
            try
            {

                String query1 = ""; string ownermobile = "";
                query1 = "select mobile_no from owner_master where owner_id=@ownerid";
                DBConnection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownerid", DbType.String, ownerid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        ownermobile = ds.Tables[0].Rows[0]["mobile_no"].ToString();
                    DBConnection.Close();
                    return ownermobile;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public string GetMobileNoByID(string shipperid)
        {
            try
            {

                String query1 = ""; string mobileno = "";
                query1 = "select user_id from user_mst where unique_id=@shipperid";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("shipperid", DbType.String, shipperid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        mobileno = ds.Tables[0].Rows[0]["user_id"].ToString();
                    return mobileno;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public string GetEmailByID(string userid)
        {
            try
            {

                String query1 = ""; string mobileno = "";
                query1 = "select email_id from user_mst where unique_id=@userid";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, userid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        mobileno = ds.Tables[0].Rows[0]["email_id"].ToString();
                    return mobileno;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public string GetUserdetailsByID(string userid)
        {
            try
            {

                String query1 = ""; string firstname = "";
                query1 = "select * from user_mst where unique_id=@userid";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, userid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        firstname = ds.Tables[0].Rows[0]["first_name"].ToString();
                    return firstname;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetDriverDetailsByID(string driverid)
        {
            try
            {

                String query1 = ""; string mobileno = "";
                query1 = " select first_name,truck_body_mst.truck_body_desc,truck_rto_registration_detail.vehicle_reg_no,user_id,driver_mst.driver_photo,driver_mst.driver_id,* from user_mst " +
                           " join driver_truck_details as dest on dest.driver_id= user_mst.unique_id " +
                           " join truck_mst on dest.truck_id =  truck_mst.truck_id  " +
                           " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id   " +
                           " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id  " +
                           " left join truck_rto_registration_detail on dest.truck_id = truck_rto_registration_detail.truck_id   " +
                           " left join truck_permit_details on dest.truck_id = truck_permit_details.truck_id    " +
                           " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id  " +
                           " join driver_mst on driver_mst.driver_id = dest.driver_id and driver_mst.isfree='Y' where driver_mst.driver_id=@driverid  ";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, driverid));
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                        return dt;
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetUserdetailsByuserID(string userid)
        {
            try
            {
                DataTable dtuserdtl = new DataTable();
                String query1 = ""; string firstname = "";
                query1 = "select * from user_mst where unique_id=@userid";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, userid));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtuserdtl = ds.Tables[0];

                    if (dtuserdtl != null && dtuserdtl.Rows.Count > 0)
                        return dtuserdtl;
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }


        #endregion

        #region Post Methods

        public BLReturnObject SaveHistory(DataTable dt_hist, ref IDbCommand DBCommand)
        {
            string histid = "";
            Document doc = new Document();
            DS_load_order_quotation ds_hist = new DS_load_order_quotation();
            BLReturnObject objBLReturnObject = new BLReturnObject(); Master master = new Master();
            ds_hist.EnforceConstraints = false;
            ds_hist.load_order_enquiry_history.ImportRow(dt_hist.Rows[0]);
            ds_hist.load_order_enquiry_history[0].hist_id = Guid.NewGuid();
            ds_hist.load_order_enquiry_history[0].hist_date = System.DateTime.UtcNow;
            ds_hist.load_order_enquiry_history[0].created_date = System.DateTime.UtcNow;
            if (dt_hist.Rows[0]["modified_by"] != null && dt_hist.Rows[0]["modified_by"].ToString() != "")
                ds_hist.load_order_enquiry_history[0].created_by = dt_hist.Rows[0]["modified_by"].ToString();
            else
                ds_hist.load_order_enquiry_history[0].created_by = dt_hist.Rows[0]["created_by"].ToString();

            if (dt_hist.Rows[0]["modified_device_id"] != null && dt_hist.Rows[0]["modified_device_id"].ToString() != "")
                ds_hist.load_order_enquiry_history[0].device_id = dt_hist.Rows[0]["modified_device_id"].ToString();
            else
                ds_hist.load_order_enquiry_history[0].device_id = dt_hist.Rows[0]["device_id"].ToString();

            if (dt_hist.Rows[0]["modified_device_type"] != null && dt_hist.Rows[0]["modified_device_type"].ToString() != "")
                ds_hist.load_order_enquiry_history[0].device_type = dt_hist.Rows[0]["modified_device_type"].ToString();
            else
                ds_hist.load_order_enquiry_history[0].device_type = dt_hist.Rows[0]["device_type"].ToString();
            objBLReturnObject = master.UpdateTables(ds_hist.load_order_enquiry_history, ref DBCommand);
            if (objBLReturnObject.ExecutionStatus == 2)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                objBLReturnObject.ExecutionStatus = 2;
            }
            return objBLReturnObject;
        }

        public DataSet Generate_load_enquiry_quotations(DataTable ds_Post_load_enquiry)
        {
            DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            dS_load_order_quotation.EnforceConstraints = false;
            decimal fixedcost = 0; decimal tollcost = 0; decimal da_cost = 0; decimal Aproxdays = 0; decimal totalhours = 0;

            if (ds_Post_load_enquiry.Rows[0]["aprox_kms"] != null && Convert.ToDecimal(ds_Post_load_enquiry.Rows[0]["aprox_kms"]) != 0)
            {
                fixedcost = Convert.ToDecimal(ds_Post_load_enquiry.Rows[0]["aprox_kms"]) * 35M; // total kms * Rate per km
            }

            totalhours = Convert.ToDecimal(ds_Post_load_enquiry.Rows[0]["aprox_kms"]) / 30; // Considered 30 kms per hour 
            Aproxdays = totalhours / 12;  // Usaually driver works for 12 hours a day  - It will give actual days 
            Aproxdays = BLGeneralUtil.GetRoundedValue(Constant.ROUNDING_NEAREST_RUPEE, Aproxdays);
            if (Aproxdays < 1M)
                Aproxdays = 1;

            da_cost = Aproxdays * 500;  // Considerd fixed 500 Rs per day DA to driver 

            tollcost = 2272; // Considered toll cost

            if (ds_Post_load_enquiry != null)
            {
                dS_load_order_quotation.load_order_enquiry_quotation.ImportRow(ds_Post_load_enquiry.Rows[0]);
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["aprox_days"] = Aproxdays;
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["quotation_cost"] = fixedcost;
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["driver_da"] = da_cost;
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["toll_cost"] = tollcost;
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["quotation_total_cost"] = fixedcost + da_cost + tollcost;
                //dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["quotation_cost"] = 30000;
                //dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["quotation_total_cost"] = 4000;
                //dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["quotation_others"] = 10000;
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["active_flag"] = "Y";
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["owner_id"] = "-9999"; // Default/Initial Quote
                //dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["status"] = "01"; // Generating Quote 
                dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["status"] = "-01"; // Default/Initial Quote
            }

            return dS_load_order_quotation;
        }

        [HttpPost]
        public string ValidatePromocode([FromBody]JObject jobj)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<PromoCode> tload = new List<PromoCode>();
            decimal flatdiscount = 0; decimal PercentageDiscount = 0; decimal DiscountPrice = 0; string Message = ""; string loadinquiryno = "";
            DataTable dtParameter = new DataTable(); DataTable dt_load = new DataTable();
            TruckerMaster objTruckerMaster = new TruckerMaster();
            Boolean B1 = false;

            if (jobj["Promocode"] != null)
            {
                tload = jobj["Promocode"].ToObject<List<PromoCode>>();
                dtParameter = BLGeneralUtil.ToDataTable(tload);
                dtParameter = BLGeneralUtil.CheckDateTime(dtParameter);
            }

            string order_type_flag = ""; string Sizetypecode = "";
            DataTable dtpostorder = GetPostloaddetailsByID(tload[0].load_inquiry_no);
            if (dtpostorder != null)
            {
                tload[0].rate_type_flag = dtpostorder.Rows[0]["rate_type_flag"].ToString();
                order_type_flag = dtpostorder.Rows[0]["order_type_flag"].ToString();
                Sizetypecode = dtpostorder.Rows[0]["SizeTypeCode"].ToString();
            }

            B1 = objTruckerMaster.IsCouponValid(tload[0].coupon_code, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, order_type_flag, tload[0].rate_type_flag, Sizetypecode, ref flatdiscount, ref PercentageDiscount, ref Message);
            if (B1)
            {
                if (flatdiscount != 0)
                    dtParameter.Rows[0]["Discount_value"] = Math.Round(flatdiscount);
                else if (PercentageDiscount != 0)
                {
                    dtParameter.Rows[0]["PercentageDiscount"] = Math.Round(PercentageDiscount);
                    DiscountPrice = Convert.ToDecimal(tload[0].Total_cost) * (PercentageDiscount / 100);
                    dtParameter.Rows[0]["Discount_value"] = Math.Round(DiscountPrice);
                }

                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtParameter));
            }
            else
                return BLGeneralUtil.return_ajax_string("0", Message);
        }


        #endregion

        [HttpGet]
        public string GetTruckType()
        {
            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = "select * from TruckTypeMst";
                DBConnection.Open();
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
                {
                    DBConnection.Close();
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
                }
                else
                {
                    DBConnection.Close();
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
                }
            }
            catch (Exception ex)
            {
                DBConnection.Close();
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        public string GetDistanceRateByCodeType(string distancetypecode)
        {
            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = "select rate from DistanceRateDetail where distance_type_code= '" + distancetypecode + "'";
                DBConnection.Open();
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
                {
                    DBConnection.Close();
                    return dtPostLoadOrders.Rows[0]["rate"].ToString();
                }
                else
                {
                    DBConnection.Close();
                    return "No Data Found";
                }
            }
            catch (Exception ex)
            {
                DBConnection.Close();
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        [HttpGet]
        public DataTable GetLoadInquiryBySizetypeId(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from post_load_inquiry  where load_inquiry_no = @inqid and active_flag=" + "'Y'";
            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else

                return null;
        }

        [HttpGet]
        public string GetOrderDetaislById(string shipperid)
        {

            String query1 = "";
            DataTable dtPostLoadOrders = new DataTable();
            if (shipperid == "-1")
            {
                query1 = " select orders.* from orders ORDER BY orders.created_date DESC";
            }
            else
            {
                query1 = "select orders.* from orders  where shipper_id = @userid and active_flag='Y'  and orders.status not in ('45')  ORDER BY orders.created_date DESC ";

            }
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid ", DbType.String, shipperid));
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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        [HttpGet]
        public string GetCompletedOrderDetaislById(string shipperid)
        {

            String query1 = "";
            DataTable dtPostLoadOrders = new DataTable();
            if (shipperid == "-1")
            {
                query1 = " select orders.*   from orders ORDER BY orders.created_date DESC";
            }
            else
            {
                query1 = "select orders.*    from orders  where shipper_id = @userid and active_flag='Y' and orders.status='45' ORDER BY orders.created_date DESC ";

            }
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid ", DbType.String, shipperid));
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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        [HttpGet]
        public string GetOrderDetaislByIdNew(string shipperid)
        {

            String query1 = "";
            DataTable dtPostLoadOrders = new DataTable();
            if (shipperid == "-1")
            {
                query1 = " select orders.*    from orders  ORDER BY orders.created_date DESC";
            }
            else
            {
                query1 = "select orders.*    from orders  where shipper_id = @userid and active_flag='Y' and orders.status='45' ORDER BY orders.created_date DESC ";

            }
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid ", DbType.String, shipperid));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            DataSet dsfinalorderdetails = new DataSet();

            DataTable dt = dtPostLoadOrders;
            dt.TableName = "dtorder";
            dsfinalorderdetails.Tables.Add(dt.Copy());

            DataTable dtdriver = new DataTable();
            dtdriver = GetDriverOrderDetails();
            dtdriver.TableName = "dtdriver";
            dsfinalorderdetails.Tables.Add(dtdriver.Copy());

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return ds2json(dsfinalorderdetails);
            else

                return null;
        }

        public static string ds2json(DataSet ds)
        {
            return JsonConvert.SerializeObject(ds, Formatting.Indented);
        }

        [HttpGet]
        public DataTable GetLoadInquiryquotation(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM load_order_enquiry_quotation  where load_inquiry_no = @inqid and active_flag=" + "'Y' ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
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

        //by sandip 20/4/2016 
        // get order details by load inquiry number for chage driver or truck
        [HttpGet]
        public DataTable GetOrders(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM  orders where load_inquiry_no=@inqid  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
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
        public DataTable GetCancelOrdersRequestByInq(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM  order_cancellation_details where   load_inquiry_no=@inqid  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
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
        public DataTable GetRescheduleRequestDetailsByInq(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM  order_reschedule_req_details where   load_inquiry_no=@inqid and active_flag='N' ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
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

        public DataTable GetDriverOrderDetails()
        {
            String query = "SELECT * FROM order_driver_truck_details";

            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
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
        public DataTable GetLoadOrdersByID(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT *,null as truck_lat,null as truck_lng FROM orders where load_inquiry_no = @inqid and active_flag=" + "'Y'";
            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;
        }

        [HttpGet]
        public DataTable GetPostloaddetailsByID(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM post_load_inquiry where load_inquiry_no = @inqid ";
            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;
        }

        [HttpGet]
        public string GetDraftOrderDetails(string inqno, string shipperid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            //            String query1 = @"select     SizeTypeMatrix.Hiretruck_MaxKM,SizeTypeMatrix.TimeForUnloadingInMinute,SizeTypeMatrix.TimeForloadingInMinute,convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],
            //                              SizeTypeMatrix.CBM_Max,SizeTypeMatrix.HireTruck_MinRate,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id1' OrderKey  
            //                              from orders  JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.

            //                              join user_mst on user_mst.unique_id =  orders.shipper_id 
            //                              join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag 
            //                              where  orders.active_flag='N'  and orders.IsDraft='Y' ";

            String query1 = @" select 
                                            SizeTypeMatrix.Hiretruck_MaxKM,SizeTypeMatrix.TimeForUnloadingInMinute,SizeTypeMatrix.TimeForloadingInMinute,
                                            convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],SizeTypeMatrix.CBM_Max,
                                            SizeTypeMatrix.HireTruck_MinRate,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, 
                                            SizeTypeMst.SizeTypeDesc, orders.*,res_inner.*,'id1' OrderKey  
                                            from orders  
                                            JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
                                            JOIN user_mst on user_mst.unique_id =  orders.shipper_id 
                                            JOIN SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode 
                                                and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag 
                                            left join(select * from(
                                                        select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge AS Varchar(50)) as ServiceCharge 
                                                        from order_AddonService_details
                                                    union
                                                        select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceCharge 
                                                        from order_AddonService_details
                                                   union
            			                                select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizetypeCode 
            			                                from order_AddonService_details
                                                )res
                                                PIVOT(
                                                    MAX(ServiceCharge) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                                                )pivot2
                                            )res_inner on res_inner.load_inquiry_no = orders.load_inquiry_no
                                            where  orders.active_flag='N'  and orders.IsDraft='Y' ";

            if (shipperid != null && shipperid.Trim() != "")
                query1 += "and orders.shipper_id = '" + shipperid + "' ";
            if (inqno != null && inqno.Trim() != "")
            {
                query1 += "and orders.load_inquiry_no = '" + inqno + "' ";
                query1 += " select ServiceTypeCode,SizetypeCode from order_AddonService_details where load_inquiry_no= '" + inqno + "' ";
            }

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];

                if (dtPostLoadOrders.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y && inqno != null)
                {
                    if (ds != null && ds.Tables[1].Rows.Count > 0)
                    {
                        string straddon = "";
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (i == 0)
                                straddon = ds.Tables[1].Rows[i]["ServiceTypeCode"].ToString() + "^" + ds.Tables[1].Rows[i]["SizetypeCode"].ToString();
                            else
                                straddon += "," + ds.Tables[1].Rows[i]["ServiceTypeCode"].ToString() + "^" + ds.Tables[1].Rows[i]["SizetypeCode"].ToString();
                        }

                        if (straddon != "")
                        {
                            dtPostLoadOrders.Columns.Add("AddonServices");
                            dtPostLoadOrders.Rows[0]["AddonServices"] = straddon;
                        }
                        String Message = String.Empty;
                        DataTable dtorder = new PostOrderController().GetCalculateRate(dtPostLoadOrders, ref Message);

                        if (dtorder != null)
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtorder));
                    }
                }
                else
                {
                    if (dtPostLoadOrders.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
                    {
                        String Message = String.Empty;
                        DataTable dtorder = new PostOrderController().GetCalculateRate(dtPostLoadOrders, ref Message);

                        if (dtorder != null)
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtorder));
                    }
                }

            }
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "No Detail found "));
        }

        [HttpGet]
        public string GetOrderDetaislByloadinquiryId(string loadinquiryno)
        {

            StringBuilder Sb = new StringBuilder();
            DataTable dtPostLoadOrders = new DataTable();

            //            Sb.Append(@" select status1 as status,(SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) 
            //               ORDER BY CAST(status AS int) desc ) as final_status,* from ( 
            //              select CASE  WHEN order_reschedule_req_details.RescheduleReq_id IS not NULL and order_reschedule_req_details.active_flag='N' THEN '26' else result.status
            //                                          END AS status1, order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck,  
            //               order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.* 
            //               from 
            //               ( 
            //               select user_mst.first_name as shipper_name,user_mst.email_id as shipper_email,user_mst.user_id as shipper_mobileno,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
            //               from orders    
            //               LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
            //               LEFT OUTER JOIN user_mst ON user_mst.unique_id = orders.shipper_id 
            //               Where 1=1 and orders.active_flag  = 'Y'   
            //               and orders.shippingdatetime>getdate()  
            //               UNION ALL 
            //               select user_mst.first_name as shipper_name,user_mst.email_id as shipper_email,user_mst.user_id as shipper_mobileno,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
            //               LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
            //               LEFT OUTER JOIN user_mst ON user_mst.unique_id = orders.shipper_id 
            //               Where 1=1 and orders.active_flag  = 'Y'  
            //               and orders.shippingdatetime<getdate() 
            //               ) result 	  
            //               LEFT OUTER JOIN 	
            //               ( 
            //                 select 	load_inquiry_no,COUNT(*) as NoOfAssignedDriver, COUNT(*) as NoOfAssignedTruck, SUM(CAST(NoOfHandiman AS int)) as NoOfAssignedHandiman, SUM(CAST(NoOfLabour AS int)) as NoOfAssignedLabour
            //                 from  order_driver_truck_details 
            //                 group by load_inquiry_no
            //               ) as order_driver_truck_details_summary	  
            //               ON result.load_inquiry_no = order_driver_truck_details_summary.load_inquiry_no
            //                LEFT OUTER JOIN order_reschedule_req_details ON result.load_inquiry_no = order_reschedule_req_details.load_inquiry_no 
            //               ) as lstorder where  1=1  and lstorder.load_inquiry_no=@loadinquiryno  ");


            Sb.Append(@" select status1 as status,(SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) 
               ORDER BY CAST(status AS int) desc ) as final_status,* from ( 
              select CASE WHEN order_cancellation_details.cancellation_id IS not NULL THEN '25' else result.status
                                          END AS status1,res_inner.Total_PT_Charge,res_inner.Total_PT_Discount,res_inner.PT_SizeTypeCode,res_inner.Total_CL_Charge,res_inner.Total_CL_Discount,res_inner.CL_SizeTypeCode,
                        	 res_inner.Total_PEST_Charge,res_inner.Total_PEST_Discount,res_inner.PEST_SizeTypeCode,                                          
                            order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck,  
               order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.* 
               from 
               ( 
               select user_mst.first_name as shipper_name,user_mst.email_id as shipper_email,user_mst.user_id as shipper_mobileno,SizeTypeMst.SizeTypeDesc,orders.*,'id1' OrderKey  
               from orders    
               LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
               LEFT OUTER JOIN user_mst ON user_mst.unique_id = orders.shipper_id 
               Where 1=1 and orders.active_flag  = 'Y'   
               and orders.shippingdatetime>getdate()  
               UNION ALL 
               select user_mst.first_name as shipper_name,user_mst.email_id as shipper_email,user_mst.user_id as shipper_mobileno,SizeTypeMst.SizeTypeDesc,orders.*,'id2' OrderKey from orders  
               LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
               LEFT OUTER JOIN user_mst ON user_mst.unique_id = orders.shipper_id 
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
                left join(
                            select * from(
                                select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge_without_discount AS Varchar(50)) as ServiceDiscount
                                from order_AddonService_details
                                union
                                select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceDiscount
                                from order_AddonService_details
                                union
                                select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizeTypeMst.SizeTypeDesc 
                                from order_AddonService_details join SizeTypeMst on SizeTypeMst.SizeTypeCode=order_AddonService_details.SizeTypeCode
                            )res
                            PIVOT(
                                MAX(ServiceDiscount) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                            )pivot2
                        )res_inner on res_inner.load_inquiry_no = result.load_inquiry_no
               ) as lstorder where  1=1   and lstorder.load_inquiry_no=@loadinquiryno  ");

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = Sb.ToString();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinquiryno ", DbType.String, loadinquiryno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    //-------------by nitu 06-10-2016 start-----------------------------------
                    dtPostLoadOrders = ds.Tables[0];
                    //for (int i = 0; i < dtPostLoadOrders.Rows.Count; i++)
                    //{
                    //    driverController dc = new driverController();
                    //    DataTable dt_tempDriverDtl = new DataTable();
                    //    string tempPostLoadInquiryNo = dtPostLoadOrders.Rows[i]["load_inquiry_no"].ToString();
                    //    dt_tempDriverDtl = dc.GetDriverTruckDetails(tempPostLoadInquiryNo);
                    //    if (dt_tempDriverDtl != null && dt_tempDriverDtl.Rows.Count > 0)
                    //        dtPostLoadOrders.Rows[i]["driverDetails"] = SendReceiveJSon.GetJson(dt_tempDriverDtl);
                    //    else
                    //        dtPostLoadOrders.Rows[i]["driverDetails"] = "";
                    //}
                }
                //-------------by nitu 06-10-2016 end-----------------------------------
            }
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        [HttpGet]
        public string GetQuotSpecificationsDetails()
        {
            try
            {
                DataTable dtQuotdetails = new DataTable();
                String query1 = "select * from quote_specifications_mst order by order_no asc ";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtQuotdetails = ds.Tables[0];
                }
                if (dtQuotdetails != null && dtQuotdetails.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtQuotdetails)));
                }
                else
                {
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
                }
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        [HttpGet]
        public string GetpostloadinquiryByID(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],* FROM  post_load_inquiry where load_inquiry_no=@inqid  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
            {
                if (dtPostLoadOrders.Rows[0]["rate_type_flag"].ToString() == "N")
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                else
                {
                    dtPostLoadOrders.Clear();
                    String query2 = @" SELECT  user_mst.user_id,user_mst.email_id,user_mst.first_name,SizeTypeMatrix.Discount as  AddSerBaseDiscount,convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],*,res_inner.*
                                FROM post_load_inquiry
                                left join user_mst on user_mst.unique_id=post_load_inquiry.shipper_id
                                left join SizeTypeMatrix on post_load_inquiry.SizeTypeCode=SizeTypeMatrix.SizeTypeCode and SizeTypeMatrix.rate_type_flag=post_load_inquiry.rate_type_flag
                                left join(
	                                select * from(
                                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Charge')as ServiceTypeCode,Cast(ServiceCharge AS Varchar(50)) as ServiceCharge
                                            from order_AddonService_details
                                            union
                                            select load_inquiry_no,('Total_'+ServiceTypeCode+'_Discount')as ServiceTypeCode,Cast(ServiceDiscount  AS Varchar(50)) as ServiceCharge
                                            from order_AddonService_details
                                            union
                                            select load_inquiry_no,(ServiceTypeCode+'_SizeTypeCode')as ServiceTypeCode,SizetypeCode 
                                            from order_AddonService_details
                                    )res
                                    PIVOT(
                                        MAX(ServiceCharge) For ServiceTypeCode in (Total_PT_Charge,Total_PT_Discount,PT_SizeTypeCode,Total_CL_Charge,Total_CL_Discount,CL_SizeTypeCode,Total_PEST_Charge,Total_PEST_Discount,PEST_SizeTypeCode)
                                    )pivot2
                                )res_inner on res_inner.load_inquiry_no = post_load_inquiry.load_inquiry_no
                                where post_load_inquiry.load_inquiry_no= '" + loadinqno + @"'
                                select ServiceTypeCode,SizetypeCode from order_AddonService_details where load_inquiry_no= '" + loadinqno + @"'";

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.CommandText = query2;
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
                    DataSet ds_addon = new DataSet();
                    DBDataAdpterObject.Fill(ds_addon);
                    if (ds_addon != null && ds_addon.Tables.Count > 0)
                    {
                        if (ds_addon.Tables[0] != null && ds_addon.Tables[0].Rows.Count > 0)
                            dtPostLoadOrders = ds_addon.Tables[0];

                        if (ds_addon.Tables[1] != null && ds_addon.Tables[1].Rows.Count > 0)
                        {
                            string straddon = "";
                            for (int i = 0; i < ds_addon.Tables[1].Rows.Count; i++)
                            {
                                if (i == 0)
                                    straddon = ds_addon.Tables[1].Rows[i]["ServiceTypeCode"].ToString() + "^" + ds_addon.Tables[1].Rows[i]["SizetypeCode"].ToString();
                                else
                                    straddon += "," + ds_addon.Tables[1].Rows[i]["ServiceTypeCode"].ToString() + "^" + ds_addon.Tables[1].Rows[i]["SizetypeCode"].ToString();
                            }

                            if (straddon != "")
                            {
                                dtPostLoadOrders.Columns.Add("AddonServices");
                                dtPostLoadOrders.Rows[0]["AddonServices"] = straddon;
                            }
                            String Message = String.Empty;
                            DataTable dtorder = new PostOrderController().GetCalculateRate(dtPostLoadOrders, ref Message);

                            if (dtorder != null)
                                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtorder));
                        }
                    }
                    if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                        return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                    else
                        return BLGeneralUtil.return_ajax_string("0", "No order found");
                }
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        public DataTable GetCalculateRate(DataTable dtPostOrderParameter, ref string Message)
        {

            if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
            {
                Message = " No data available to Calculate Rate. Operation cancelled.";
                return null;
            }
            else
            {
                DateTime OrderDate = DateTime.Today;
                String SizeTypeCode = String.Empty;
                String OrderTypeCode = String.Empty;
                String TruckTypeCode = String.Empty;
                TimeSpan Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());


                if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                    OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

                if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
                    TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                    if (dtPostOrderParameter.Columns.Contains("SizeTypeCode"))
                        SizeTypeCode = dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString();

                String rate_type_flag = "";
                if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                    rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

                String goods_type_flag = "";
                if (dtPostOrderParameter.Columns.Contains("goods_type_flag"))
                    goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();

                DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                Decimal TotalDistance = -1;
                if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                    TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                String TotalDistanceUOM = String.Empty;
                if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                    TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                Decimal TimeToTravelInMinute = -1;
                if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                    TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                //No Of Truck Edited by User
                int? NoOfTruck = null;
                if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                    if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                        NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

                //No Of Driver Edited by User
                int? NoOfDriver = null;
                if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                    if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                        NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

                //No Of Labour Edited by User
                int? NoOfLabour = null;
                if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                    if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                    {
                        NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);
                        NoOfLabour = NoOfLabour.Value - NoOfTruck.Value;
                    }

                //No Of Handiman Edited by User
                int? NoOfHandiman = null;
                if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                    if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                        NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


                int? NoOfSupervisor = null;
                if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                    if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                        NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


                String IncludePackingCharge = "N";
                if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                    IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

                if (NoOfTruck != NoOfDriver)
                    NoOfDriver = NoOfTruck;

                TruckerMaster objTruckerMaster = new TruckerMaster();
                DataTable dtSizeTypeMst = new DataTable();

                dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                {
                    ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(dtPostOrderParameter) + ")");
                    return null;
                }
                else
                {

                    dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                    dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();

                    dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"] = dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString();
                    dtPostOrderParameter.Rows[0]["Total_PT_Charge"] = dtSizeTypeMst.Rows[0]["Total_PT_Charge"].ToString();
                    dtPostOrderParameter.Rows[0]["Total_PT_Discount"] = dtSizeTypeMst.Rows[0]["Total_PT_Discount"].ToString();

                    dtPostOrderParameter.Rows[0]["Total_CL_Charge"] = dtSizeTypeMst.Rows[0]["Total_CL_Charge"].ToString();
                    dtPostOrderParameter.Rows[0]["Total_CL_Discount"] = dtSizeTypeMst.Rows[0]["Total_CL_Discount"].ToString();
                    dtPostOrderParameter.Rows[0]["Total_PEST_Charge"] = dtSizeTypeMst.Rows[0]["Total_PEST_Charge"].ToString();
                    dtPostOrderParameter.Rows[0]["Total_PEST_Discount"] = dtSizeTypeMst.Rows[0]["Total_PEST_Discount"].ToString();

                    dtPostOrderParameter.Rows[0]["TotalAddServiceDiscount"] = dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"].ToString();
                    dtPostOrderParameter.Rows[0]["TotalAddServiceCharge"] = dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"].ToString();
                    //dtSizeTypeMst.Columns.Add("IncludeAddonService", typeof(String));
                    //dtSizeTypeMst.Rows[0]["IncludeAddonService"] = dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString();
                    //dtSizeTypeMst.Columns.Add("AddonServices", typeof(String));
                    //dtSizeTypeMst.Rows[0]["AddonServices"] = dtPostOrderParameter.Rows[0]["AddonServices"].ToString();

                    //dtSizeTypeMst.Columns.Add("AddonServices");
                    //dtSizeTypeMst.Rows[0]["AddonServices"] = dtPostOrderParameter.Rows[0]["AddonServices"].ToString();
                    //dtSizeTypeMst.Columns.Add("IncludeAddonService", typeof(String));
                    //dtSizeTypeMst.Rows[0]["IncludeAddonService"] = dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString();

                    Message = "Data Retrieve sucessfully";
                    return dtPostOrderParameter;
                }
            }
        }



        public DateTime DatetimeUTC_ToDubaiConverter(string zone, DateTime strdate)// string date, string time)
        {
            zone = "UTC+04:00";
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime, FetchedDate;
            //Set the time zone information to US Mountain Standard Time 
            //timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(drpToTimeZone.SelectedValue);
            //Get date and time in US Mountain Standard Time 
            //dateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);

            FetchedDate = strdate;

            var tZone = TimeZoneInfo.GetSystemTimeZones();
            String temp = tZone.Where(e1 => e1.DisplayName.Contains(zone)).Select(t => t.Id).FirstOrDefault().ToString();

            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(temp);

            dateTime = TimeZoneInfo.ConvertTimeFromUtc(FetchedDate, timeZoneInfo);


            return dateTime;
        }

        [HttpPost]
        public string save_quote_accepted_by_Driver([FromBody]JObject jobj)
        {
            // Method to save quote accepted by with/without reverse quote from Transporter (Reverse Quote)
            try
            {
                string quotid = "";
                string inquiryId = ""; string driverid = "";
                PostOrderController obj = new PostOrderController();
                DataSet dsdrv = new DataSet();
                DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
                BLReturnObject objBLobj = new BLReturnObject(); Document objdoc = new Document();
                Master master = new Master();

                List<InquiryQuote> jobjdata = new List<InquiryQuote>();
                DataTable dt_DriverTruckdetails = new DataTable();
                if (jobj["quote"] != null)
                {
                    jobjdata = jobj["quote"].ToObject<List<InquiryQuote>>();
                    dsdrv = master.CreateDataSet(jobjdata);
                }

                inquiryId = jobjdata[0].load_inquiry_no;
                driverid = jobjdata[0].driver_id;

                DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(driverid);
                if (dtdeviceIds != null)
                {
                    DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                    if (dr.Length > 0)
                        if (dr[0].ItemArray[3].ToString() != jobjdata[0].modified_device_id)
                        {
                            return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                        }
                }
                DataTable dtdriverorder = new driverController().Getdriverordernotification(inquiryId);
                DataTable dtdriverdetails = new driverController().GetDriverDetailsTableById(driverid); // check driver is in database 
                // get orders details for update driverid/truckid in database
                DataTable dt_orders = new PostOrderController().GetOrders(inquiryId);

                if (jobjdata[0].Isaccepted == "N")
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    for (int i = 0; i < dtdriverorder.Rows.Count; i++)
                    {
                        if (dtdriverorder.Rows[i]["driver_id"].ToString() == driverid)
                        {
                            DS_driver_order_notifications ds_driverordernotification = new DS_driver_order_notifications();
                            int index;
                            string DocNtficID = ""; string Message = "";

                            if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "DRN", "", "", ref DocNtficID, ref Message)) // New Driver Notification ID
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            }
                            ds_driverordernotification.EnforceConstraints = false;
                            ds_driverordernotification.driver_order_notifications_history.ImportRow(dtdriverorder.Rows[i]);
                            index = ds_driverordernotification.driver_order_notifications_history.Rows.Count - 1;
                            ds_driverordernotification.driver_order_notifications_history[index].notification_id = DocNtficID;
                            ds_driverordernotification.driver_order_notifications_history[index].AcceptChanges();
                            ds_driverordernotification.driver_order_notifications_history[index].SetAdded();

                            //delete active_flag = 'Y' row from table driver_order_notifications
                            ds_driverordernotification.driver_order_notifications.ImportRow(dtdriverorder.Rows[i]);
                            ds_driverordernotification.driver_order_notifications[0].Delete();

                            //DS_driver_order_notifications ds_driver_order_notification = new DS_driver_order_notifications();
                            //ds_driverordernotification.EnforceConstraints = false;
                            //ds_driver_order_notification.driver_order_notifications.ImportRow(dtdriverorder.Rows[i]);
                            //ds_driver_order_notification.driver_order_notifications[0].active_flag = Constant.Flag_No;
                            //ds_driver_order_notification.driver_order_notifications[0].modified_by = jobjdata[0].modified_by;
                            //ds_driver_order_notification.driver_order_notifications[0].modified_date = System.DateTime.UtcNow;
                            //ds_driver_order_notification.driver_order_notifications[0].modified_host = jobjdata[0].modified_host;
                            //ds_driver_order_notification.driver_order_notifications[0].modified_device_id = jobjdata[0].modified_device_id;
                            //ds_driver_order_notification.driver_order_notifications[0].modified_device_type = jobjdata[0].modified_device_type;

                            try
                            {
                                ds_driverordernotification.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }


                            objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }

                            objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications_history, ref DBCommand);
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

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Quote Regected By driver :" + driverid);
                    return BLGeneralUtil.return_ajax_string("1", "Quote Rejected Successfully ");
                }
                else
                {
                    if (dt_orders != null)
                        if (dt_orders.Rows[0]["isassign_driver_truck"].ToString() == Constant.Flag_Yes)
                            return BLGeneralUtil.return_ajax_string("0", "Order request already accepted by another driver ");

                    DateTime Dr_PreviousOrdershippingdatetime = new DateTime();
                    DateTime TotalDr_PreviousOrderdatetime = new DateTime();
                    DateTime Dr_NextOrdershippingdatetime = new DateTime();
                    DateTime TotalDr_NextOrderdatetime = new DateTime();

                    //get driver ongoing ordres 
                    DataTable dt_driverorders = new driverController().GetOnGoingOrdersByDriver(driverid);

                    //if (dt_driverorders != null)
                    //{
                    //    int dtcount = dt_driverorders.Rows.Count;

                    //    DateTime NewOrdershippingdatetime = Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString());
                    //    TimeSpan Newordertime = new TimeSpan();
                    //    Newordertime = TimeSpan.Parse(dt_orders.Rows[0]["TimeToTravelInMinute"].ToString());
                    //    DateTime NewOrderDatetime = NewOrdershippingdatetime.AddMinutes(Newordertime.TotalMinutes);
                    //    NewOrderDatetime = NewOrderDatetime.AddHours(2);

                    //    //driver previous order details based on new order shipping date 
                    //    DataTable dtDriverPreviousorders = new driverController().GetDriverOrdersByID(driverid, NewOrdershippingdatetime.ToString(), "1");
                    //    DataTable dtDriverNextorders = new driverController().GetDriverOrdersByID(driverid, NewOrdershippingdatetime.ToString(), "2");

                    //    if (dtDriverPreviousorders != null)
                    //    {
                    //        Dr_PreviousOrdershippingdatetime = Convert.ToDateTime(dtDriverPreviousorders.Rows[0]["shippingdatetime"].ToString());
                    //        TimeSpan DriverPreviousOrdertime = new TimeSpan();
                    //        DriverPreviousOrdertime = TimeSpan.Parse(dtDriverPreviousorders.Rows[0]["TimeToTravelInMinute"].ToString());
                    //        TotalDr_PreviousOrderdatetime = Dr_PreviousOrdershippingdatetime.AddHours(DriverPreviousOrdertime.TotalHours);
                    //        TotalDr_PreviousOrderdatetime = TotalDr_PreviousOrderdatetime.AddHours(2);

                    //        if (TotalDr_PreviousOrderdatetime > NewOrdershippingdatetime)
                    //            sb.Append("You are busy in order free on '" + TotalDr_PreviousOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "' and Current Order shipping  date is '" + NewOrdershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt"));
                    //    }
                    //    else if (dtDriverNextorders != null)
                    //    {
                    //        Dr_NextOrdershippingdatetime = Convert.ToDateTime(dtDriverNextorders.Rows[0]["shippingdatetime"].ToString());
                    //        TotalDr_NextOrderdatetime = Dr_NextOrdershippingdatetime.AddHours(-2);

                    //        if (TotalDr_NextOrderdatetime < NewOrderDatetime)
                    //            sb.Append("You are busy in order. completion time  of received ordered will be '" + NewOrderDatetime.ToString("dd-MM-yyy HH:mm:ss tt") + "' and You already having order at " + TotalDr_NextOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + " ");
                    //    }
                    //}

                    //if (sb.Length > 0)
                    //    return BLGeneralUtil.return_ajax_string("0", sb.ToString());

                    DataTable dt_quotation = obj.GetLoadInquiryQuotationById(inquiryId, "-1");
                    if (dt_quotation != null && dt_quotation.Rows.Count > 0)
                    {
                        DBConnection.Open();
                        DBCommand.Transaction = DBConnection.BeginTransaction();
                        try
                        {
                            dS_load_order_quotation.EnforceConstraints = false;
                            dS_load_order_quotation.load_order_enquiry_quotation.ImportRow(dt_quotation.Rows[0]);
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["driver_id"] = driverid;
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["status"] = "02"; // Reverse Quote Entered
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["created_date"] = DateTime.UtcNow;
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["created_by"] = jobjdata[0].modified_by;
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["device_id"] = jobjdata[0].modified_device_id;
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0]["device_type"] = jobjdata[0].modified_device_type;
                            dS_load_order_quotation.load_order_enquiry_quotation.AcceptChanges();
                            dS_load_order_quotation.load_order_enquiry_quotation.Rows[0].SetAdded();

                            objBLobj = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                return BLGeneralUtil.return_ajax_string("0", "Error In Saving Data");
                            }


                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                        try
                        {
                            #region Save History
                            objBLobj = SaveHistory(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                        try
                        {
                            if (dtdriverorder != null && dtdriverorder.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtdriverorder.Rows.Count; i++)
                                {
                                    string truckid = "";
                                    DataTable dt = new driverController().GetTruckIdBy(driverid);
                                    if (dt != null)
                                        truckid = dt.Rows[0]["truck_id"].ToString();

                                    #region Update Table driver_order_notifications details

                                    DS_driver_order_notifications ds_driver_order_notification = new DS_driver_order_notifications();
                                    ds_driver_order_notification.EnforceConstraints = false;
                                    ds_driver_order_notification.driver_order_notifications.ImportRow(dtdriverorder.Rows[i]);
                                    if (dtdriverorder.Rows[i]["driver_id"].ToString() == driverid)
                                        ds_driver_order_notification.driver_order_notifications[0].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                                    ds_driver_order_notification.driver_order_notifications[0].active_flag = Constant.Flag_No;
                                    ds_driver_order_notification.driver_order_notifications[0].modified_by = jobjdata[0].modified_by;
                                    ds_driver_order_notification.driver_order_notifications[0].modified_date = System.DateTime.UtcNow;
                                    ds_driver_order_notification.driver_order_notifications[0].modified_host = jobjdata[0].modified_host;
                                    ds_driver_order_notification.driver_order_notifications[0].modified_device_id = jobjdata[0].modified_device_id;
                                    ds_driver_order_notification.driver_order_notifications[0].modified_device_type = jobjdata[0].modified_device_type;
                                    objBLobj = master.UpdateTables(ds_driver_order_notification.driver_order_notifications, ref DBCommand);
                                    if (objBLobj.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLobj.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                    }

                                    #endregion

                                    if (dtdriverorder.Rows[i]["driver_id"].ToString() == driverid)
                                    {
                                        DS_driver_order_notifications.order_driver_truck_detailsRow tr_order_driver = ds_driver_order_notification.order_driver_truck_details.Neworder_driver_truck_detailsRow();
                                        ds_driver_order_notification.EnforceConstraints = false;
                                        tr_order_driver.load_inquiry_no = jobjdata[0].load_inquiry_no;
                                        tr_order_driver.driver_id = driverid;
                                        tr_order_driver.truck_id = truckid;

                                        tr_order_driver.NoOfHandiman = dt_orders.Rows[0]["NoOfHandiman"].ToString();
                                        tr_order_driver.NoOfLabour = dt_orders.Rows[0]["NoOfLabour"].ToString();

                                        tr_order_driver.status = Constant.ALLOCATED_BUT_NOT_STARTE;
                                        tr_order_driver.active_flag = Constant.Flag_Yes;
                                        tr_order_driver.created_by = jobjdata[0].modified_by;
                                        tr_order_driver.created_date = System.DateTime.UtcNow;
                                        tr_order_driver.created_host = jobjdata[0].modified_host;
                                        tr_order_driver.device_id = jobjdata[0].modified_device_id;
                                        tr_order_driver.device_type = jobjdata[0].modified_device_type;
                                        ds_driver_order_notification.order_driver_truck_details.Addorder_driver_truck_detailsRow(tr_order_driver);
                                        ds_driver_order_notification.order_driver_truck_details.Rows[0].AcceptChanges();
                                        ds_driver_order_notification.order_driver_truck_details.Rows[0].SetAdded();
                                        ds_driver_order_notification.EnforceConstraints = false;
                                        objBLobj = master.UpdateTables(ds_driver_order_notification.order_driver_truck_details, ref DBCommand);
                                        if (objBLobj.ExecutionStatus == 2)
                                        {
                                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            objBLobj.ExecutionStatus = 2;
                                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                        }

                                        Byte status = 0; string Msg = "";
                                        string DriverName = GetUserdetailsByID(driverid);
                                        status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, dt_orders.Rows[0]["shipper_id"].ToString(), "ADMIN", Constant.MessageType_AssignDriverTruck, "Driver " + DriverName + " are assign your order # " + jobjdata[0].load_inquiry_no, Constant.MessageType_AssignDriverTruck, jobjdata[0].load_inquiry_no, dt_orders.Rows[0]["shipper_id"].ToString(), jobjdata[0].load_inquiry_no, truckid, driverid, dt_orders.Rows[0]["owner_id"].ToString(), ref Msg);
                                        if (status == Constant.Status_Fail)
                                        {
                                            Log.ExceptionLog("Error in save notification Data ");
                                            DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            //return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                                        }

                                    }
                                }

                            }


                            #region update status in driver_mst

                            //if (dtdriverdetails != null)
                            //{
                            //    DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                            //    if (dtdriverdetails != null && dtdriverdetails.Rows.Count > 0)
                            //    {
                            //        ds_owner.driver_mst.ImportRow(dtdriverdetails.Rows[0]);
                            //        ds_owner.driver_mst[0].isfree = Constant.Flag_No;

                            //        objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            //        if (objBLobj.ExecutionStatus != 1)
                            //        {
                            //            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            //            DBCommand.Transaction.Rollback();
                            //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            //        }
                            //    }
                            //}

                            #endregion

                            #region update status in Orders

                            DS_orders ds_order = new DS_orders();
                            ds_order.orders.ImportRow(dt_orders.Rows[0]);
                            string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(jobjdata[0].load_inquiry_no);
                            ds_order.orders[0].trackurl = trakurl;
                            ds_order.orders[0].isassign_driver_truck = Constant.Flag_Yes;

                            //ds_order.orders[0].IsAssignDriver = Constant.Flag_Yes;
                            //ds_order.orders[0].IsAssignTruck = Constant.Flag_Yes;

                            objBLobj = master.UpdateTables(ds_order.orders, ref DBCommand);
                            if (objBLobj.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }

                            #endregion

                            //Msg = "Thank you..Trukker to you driver " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + jobjdata[0].load_inquiry_no;
                            //new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(dt_orders.Rows[0]["owner_id"].ToString()));

                            //string MsgMailbody = "Thank you..Your order from  " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + jobjdata[0].load_inquiry_no;
                            //ServerLog.Log(GetEmailByID(dt_orders.Rows[0]["owner_id"].ToString()));
                            //var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(dt_orders.Rows[0]["owner_id"].ToString()), " Your Order is confirmed (Order ID: " + jobjdata[0].load_inquiry_no + ")", "", MsgMailbody, ""));
                            //if (result["status"].ToString() == "0")
                            //{
                            //    ServerLog.Log("Error Sending Activation Email");
                            //    // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                            //}

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Quote Accepted with Suggested Fixed Cost" + jobjdata[0].load_inquiry_no + " " + jobjdata[0].driver_id);
                            return BLGeneralUtil.return_ajax_string("1", "Data Saved Successfully ");


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
                        return BLGeneralUtil.return_ajax_string("0", "Load Inquiry Not found with = " + inquiryId);

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

        #region Moving Home services

        #region Additional service details

        [HttpGet]
        public string GetSizetypeDetails(string SizeTypeCode)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT SizeTypeDesc from SizeTypeMst where SizeTypeCode=@SizeTypeCode  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("SizeTypeCode", DbType.String, SizeTypeCode));
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }

            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders.Rows[0]["SizeTypeDesc"].ToString();
            else
                return "";

        }
        [HttpGet]
        public string GetAllServices()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT * from Services_mst order by SortNo ";

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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        public DataTable GetorderAddonServiceDetailsByid(string loadinquiryno)
        {
            DataTable dt = new DataTable();
            String query1 = " SELECT * from order_AddonService_details where load_inquiry_no=@loadinquiryno  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinquiryno", DbType.String, loadinquiryno));
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

        public Boolean SaveAddOnServiceDetails(ref IDbCommand command, ref DataTable dtPostOrderParameter, ref DataTable dtSizeTypeMst, ref String Message)
        {

            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DS_orders ds_orders = new DS_orders(); DataRow[] drdeleteaddon = new DataRow[] { };
            DataTable dtuserdtl = GetUserdetailsByuserID(dtPostOrderParameter.Rows[0]["shipper_id"].ToString());
            try
            {
                #region Create Addon Entry


                DataTable dtorderaddon = GetorderAddonServiceDetailsByid(dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                string[] strservices = dtPostOrderParameter.Rows[0]["AddonServices"].ToString().Split(',');

                string stritem = "";

                for (int i = 0; i < strservices.Length; i++)
                {
                    string[] strserSizetype = strservices[i].Split('^');
                    if (i == 0)
                        stritem = "'" + strserSizetype[0] + "'";
                    else
                    {
                        stritem += ",";
                        stritem += "'" + strserSizetype[0] + "'";
                    }
                }

                if (dtorderaddon != null)
                    drdeleteaddon = dtorderaddon.Select("ServiceTypeCode not in (" + stritem + ")");


                for (int i = 0; i < strservices.Length; i++)
                {
                    string[] strservicesSizetype = strservices[i].Split('^');
                    if (strservicesSizetype[0] == Constant.PAINTING_SERIVCES)
                    {
                        DataRow[] dr = new DataRow[] { };
                        if (dtorderaddon != null)
                            dr = dtorderaddon.Select("ServiceTypeCode='" + strservicesSizetype[0] + "'");

                        ds_orders.EnforceConstraints = false;
                        if (dr.Length > 0)
                            ds_orders.order_AddonService_details.ImportRow(dr[0]);
                        else
                        {
                            string AddonID = "";
                            if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "ADS", "", "", ref AddonID, ref message)) // New Driver Notification ID
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            }
                            ds_orders.order_AddonService_details.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_orders.order_AddonService_details[i].Transaction_id = AddonID;
                        }

                        ds_orders.order_AddonService_details[i].load_inquiry_no = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                        ds_orders.order_AddonService_details[i].AddSerBaseDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceTypeCode = strservicesSizetype[0];
                        ds_orders.order_AddonService_details[i].SizeTypeCode = strservicesSizetype[1];
                        ds_orders.order_AddonService_details[i].SubServiceTypeCode = Constant.SUBSERVICE_PAINTING;
                        ds_orders.order_AddonService_details[i].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceCharge = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceCharge_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PT_Charge"].ToString());
                        ds_orders.order_AddonService_details[i].payment_status = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        ds_orders.order_AddonService_details[i].CelingRequired = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].NoofCleaners = 0;
                        ds_orders.order_AddonService_details[i].NoofCleling = 0;
                        ds_orders.order_AddonService_details[i].addon_by = "U";
                        ds_orders.order_AddonService_details[i].active_flag = dtPostOrderParameter.Rows[0]["AddonFlag"].ToString();


                        if (dtuserdtl != null)
                        {
                            ds_orders.order_AddonService_details[i].user_name = dtuserdtl.Rows[0]["first_name"].ToString();
                            ds_orders.order_AddonService_details[i].user_email = dtuserdtl.Rows[0]["email_id"].ToString();
                            ds_orders.order_AddonService_details[i].user_mobileno = dtuserdtl.Rows[0]["user_id"].ToString();
                        }
                    }
                    else if (strservicesSizetype[0] == Constant.CLEANING_SERIVCES)
                    {
                        DataRow[] dr = new DataRow[] { };
                        if (dtorderaddon != null)
                            dr = dtorderaddon.Select("ServiceTypeCode='" + strservicesSizetype[0] + "'");

                        ds_orders.EnforceConstraints = false;

                        if (dr.Length > 0)
                            ds_orders.order_AddonService_details.ImportRow(dr[0]);
                        else
                        {
                            string AddonID = "";
                            if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "ADS", "", "", ref AddonID, ref message)) // New Driver Notification ID
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            }
                            ds_orders.order_AddonService_details.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_orders.order_AddonService_details[i].Transaction_id = AddonID;
                        }

                        ds_orders.order_AddonService_details[i].load_inquiry_no = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                        ds_orders.order_AddonService_details[i].AddSerBaseDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceTypeCode = strservicesSizetype[0];
                        ds_orders.order_AddonService_details[i].SizeTypeCode = strservicesSizetype[1];
                        ds_orders.order_AddonService_details[i].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceCharge = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceCharge_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_CL_Charge"].ToString());
                        ds_orders.order_AddonService_details[i].payment_status = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        ds_orders.order_AddonService_details[i].SubServiceTypeCode = Constant.SUBSERVICE_CLEANING;
                        ds_orders.order_AddonService_details[i].CelingRequired = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].NoofCleaners = 0;
                        ds_orders.order_AddonService_details[i].NoofCleling = 0;
                        ds_orders.order_AddonService_details[i].addon_by = "U";
                        ds_orders.order_AddonService_details[i].active_flag = dtPostOrderParameter.Rows[0]["AddonFlag"].ToString();

                        if (dtuserdtl != null)
                        {
                            ds_orders.order_AddonService_details[i].user_name = dtuserdtl.Rows[0]["first_name"].ToString();
                            ds_orders.order_AddonService_details[i].user_email = dtuserdtl.Rows[0]["email_id"].ToString();
                            ds_orders.order_AddonService_details[i].user_mobileno = dtuserdtl.Rows[0]["user_id"].ToString();
                        }
                    }
                    else if (strservicesSizetype[0] == Constant.PESTCONTROL_SERIVCES)
                    {
                        DataRow[] dr = new DataRow[] { };
                        if (dtorderaddon != null)
                            dr = dtorderaddon.Select("ServiceTypeCode='" + strservicesSizetype[0] + "'");


                        ds_orders.EnforceConstraints = false;

                        if (dr.Length > 0)
                            ds_orders.order_AddonService_details.ImportRow(dr[0]);
                        else
                        {
                            string AddonID = "";
                            if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "ADS", "", "", ref AddonID, ref message)) // New Driver Notification ID
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            }
                            ds_orders.order_AddonService_details.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_orders.order_AddonService_details[i].Transaction_id = AddonID;
                        }

                        ds_orders.order_AddonService_details[i].load_inquiry_no = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                        ds_orders.order_AddonService_details[i].AddSerBaseDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceTypeCode = strservicesSizetype[0];
                        ds_orders.order_AddonService_details[i].SizeTypeCode = strservicesSizetype[1];
                        ds_orders.order_AddonService_details[i].ServiceCharge = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceDiscount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Charge"].ToString()) - Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Discount"].ToString());
                        ds_orders.order_AddonService_details[i].ServiceCharge_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_PEST_Charge"].ToString());
                        ds_orders.order_AddonService_details[i].payment_status = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].status = Constant.ALLOCATED_BUT_NOT_STARTE;
                        ds_orders.order_AddonService_details[i].SubServiceTypeCode = Constant.SUBSERVICE_PEST;
                        ds_orders.order_AddonService_details[i].CelingRequired = Constant.FLAG_N;
                        ds_orders.order_AddonService_details[i].NoofCleaners = 0;
                        ds_orders.order_AddonService_details[i].NoofCleling = 0;
                        ds_orders.order_AddonService_details[i].addon_by = "U";
                        ds_orders.order_AddonService_details[i].active_flag = dtPostOrderParameter.Rows[0]["AddonFlag"].ToString();

                        if (dtuserdtl != null)
                        {
                            ds_orders.order_AddonService_details[i].user_name = dtuserdtl.Rows[0]["first_name"].ToString();
                            ds_orders.order_AddonService_details[i].user_email = dtuserdtl.Rows[0]["email_id"].ToString();
                            ds_orders.order_AddonService_details[i].user_mobileno = dtuserdtl.Rows[0]["user_id"].ToString();
                        }
                    }

                    ds_orders.order_AddonService_details[i].created_date = System.DateTime.UtcNow;
                    ds_orders.EnforceConstraints = true;

                }


                if (drdeleteaddon.Length > 0)
                {
                    foreach (DataRow deserv in drdeleteaddon)
                    {
                        ds_orders.order_AddonService_details.ImportRow(deserv);
                        ds_orders.order_AddonService_details[ds_orders.order_AddonService_details.Rows.Count - 1].Delete();
                    }
                }

                objBLReturnObject = master.UpdateTables(ds_orders.order_AddonService_details, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    Message = "An error occurred while insert data in order_AddonService_details. " + objBLReturnObject.ServerMessage;
                    return false;
                }
                else
                {
                    Message = "order_AddonService_details data inserted successfully.";
                    return true;
                }

                #endregion
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        #endregion

        [HttpGet]
        public DataTable GetSizeTypedesc(string Sizetypecode)
        {
            DataTable dt = new DataTable();
            String query1 = " SELECT * FROM sizetypemst where SizeTypeCode = @Sizetypecode ";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("Sizetypecode", DbType.String, Sizetypecode));
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
        public string GetHomeSizeTypeCode(string typecode)
        {
            try
            {
                DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
                if (typecode.ToUpper() == "S")
                    query1 = "select * from sizetypemst  WHERE (SizeTypeCode LIKE 'SZ%') and Isactive='Y' order by sr_no  ";
                else if (typecode.ToUpper() == "T")
                    query1 = "select * from sizetypemst  WHERE (SizeTypeCode NOT LIKE 'SZ%')  and Isactive='Y' order by sr_no ";
                else if (typecode.ToUpper() == "H")
                    query1 = " select * from sizetypemst  WHERE hiretruck_flag='Y'  and Isactive='Y' order by sr_no  ";

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
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
                }
                else
                {
                    return (BLGeneralUtil.return_ajax_string("0", "No Size type Found"));
                }
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        [HttpPost]
        public String SaveMovingHomeDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<load_post_enquiry> tload = new List<load_post_enquiry>();
            decimal ddays = 0; int fraction = 0; string OrdId = ""; string loadinquiryno = "";
            DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
            TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();

            try
            {

                if (objPostOrder["PostOrderParameter"] != null)
                {
                    tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);

                    if (tload[0].load_inquiry_no == "")
                        ServerLog.Log("SaveMovingHomeDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {
                    if (tload[0].order_type_flag == Constant.ORDERTYPECODEFORHOME)
                    {
                        DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

                        DateTime before24hr = dubaiTime.AddHours(24);

                        //if (shippingdatetime < before24hr)
                        //{
                        //    return BLGeneralUtil.return_ajax_string("3", "Please provide shipping date after 24 hour");
                        //}
                        if (shippingdatetime < before24hr)
                        {
                            return BLGeneralUtil.return_ajax_string("0", " Please choose another date ");
                        }
                    }

                    Decimal NoOfDay = 1.0M;
                    DateTime OrderDate = DateTime.Today;
                    String Message = String.Empty;
                    String SizeTypeCode = String.Empty;
                    String OrderTypeCode = String.Empty;
                    String TruckTypeCode = String.Empty;
                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                        OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

                    if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
                        TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    //if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                    //    SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    //if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    //    SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();
                    //else 

                    if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                        if (dtPostOrderParameter.Columns.Contains("SizeTypeCode"))
                            SizeTypeCode = dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString();

                    if (SizeTypeCode == "")
                        return BLGeneralUtil.return_ajax_string("0", " Please provide SizeTypeDetails ");

                    String rate_type_flag = "";
                    if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                        rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

                    String goods_type_flag = "";
                    if (tload[0].goods_type_flag != "")
                    {
                        goods_type_flag = tload[0].goods_type_flag;
                        if (goods_type_flag == Constant.Flag_Yes)
                            goods_type_flag = "H";
                        else
                            goods_type_flag = "L";
                    }
                    else
                        goods_type_flag = "H";

                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    //No Of Truck Edited by User
                    int? NoOfTruck = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                        if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                            NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

                    //No Of Driver Edited by User
                    int? NoOfDriver = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                        if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                            NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

                    //No Of Labour Edited by User
                    int? NoOfLabour = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                        if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                        {
                            NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);
                            NoOfLabour = NoOfLabour.Value - NoOfTruck.Value;
                        }

                    //No Of Handiman Edited by User
                    int? NoOfHandiman = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                        if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                            NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


                    int? NoOfSupervisor = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                        if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                            NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


                    String IncludePackingCharge = "N";
                    if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                        IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

                    if (NoOfTruck != NoOfDriver)
                        NoOfDriver = NoOfTruck;

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Truck Type");

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Truck Type");

                    if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                        if (SizeTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Size Type");


                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == String.Empty)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {

                        TruckerMaster objTruckerMaster = new TruckerMaster();
                        DataTable dtSizeTypeMst = new DataTable();

                        if (rate_type_flag.ToString().ToUpper() != "N")
                        {
                            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                            {
                                dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                {
                                    ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }
                            }
                            else
                            {
                                dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                {
                                    ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }
                            }
                        }
                        else
                        {

                            dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "P", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }


                            DataTable dtSizeTypeMstBudget = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstBudget == null || dtSizeTypeMstBudget.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }


                            DataTable dtSizeTypeMstSuperSaver = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "S", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstSuperSaver == null || dtSizeTypeMstSuperSaver.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }


                            DataRow dr_temp_sizetypeBudget = dtSizeTypeMst.NewRow();
                            dr_temp_sizetypeBudget.ItemArray = dtSizeTypeMstBudget.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeBudget);

                            DataRow dr_temp_sizetypeSuperSaver = dtSizeTypeMst.NewRow();
                            dr_temp_sizetypeSuperSaver.ItemArray = dtSizeTypeMstSuperSaver.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeSuperSaver);
                        }

                        //if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "" && tload[0].Isfinalorder == "N")
                        //{
                        //    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
                        //}


                        dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();

                        dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"] = dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_PT_Charge", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_PT_Charge"] = dtSizeTypeMst.Rows[0]["Total_PT_Charge"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_PT_Discount", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_PT_Discount"] = dtSizeTypeMst.Rows[0]["Total_PT_Discount"].ToString();

                        dtPostOrderParameter.Columns.Add("Total_CL_Charge", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_CL_Charge"] = dtSizeTypeMst.Rows[0]["Total_CL_Charge"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_CL_Discount", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_CL_Discount"] = dtSizeTypeMst.Rows[0]["Total_CL_Discount"].ToString();

                        dtPostOrderParameter.Columns.Add("Total_PEST_Charge", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_PEST_Charge"] = dtSizeTypeMst.Rows[0]["Total_PEST_Charge"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_PEST_Discount", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_PEST_Discount"] = dtSizeTypeMst.Rows[0]["Total_PEST_Discount"].ToString();

                        //dtPostOrderParameter.Columns.Add("TotalAddServiceDiscount", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalAddServiceDiscount"] = dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"].ToString();
                        //dtPostOrderParameter.Columns.Add("TotalAddServiceCharge", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalAddServiceCharge"] = dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"].ToString();
                        dtSizeTypeMst.Columns.Add("IncludeAddonService", typeof(String));
                        dtSizeTypeMst.Rows[0]["IncludeAddonService"] = dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString();
                        dtSizeTypeMst.Columns.Add("AddonServices", typeof(String));
                        dtSizeTypeMst.Rows[0]["AddonServices"] = dtPostOrderParameter.Rows[0]["AddonServices"].ToString();

                        dtPostOrderParameter.Rows[0]["Total_cost_without_addon"] = dtSizeTypeMst.Rows[0]["Total_cost_without_addon"].ToString();



                        if (tload[0].shipper_id != "")
                        {
                            dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
                            if (dtbillingadd != null)
                            {
                                DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
                                if (dr.Length != 0)
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = ""; //dr[0]["source_full_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = ""; //dr[0]["destination_full_add"].ToString();
                                }
                                else
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = "";
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = "";
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = "";
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
                                }
                            }
                            else
                            {
                                dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                                dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                                dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                                dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            }
                        }


                        if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry
                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = "-9999";
                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
                            ds_Post_load_enquiry.post_load_inquiry[0].order_by = "TRUKKR";

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                            {
                                ServerLog.Log(message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", message);
                            }
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            #region Create Addon Entry

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {

                                dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_N;
                                Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                if (!Result)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                            }

                            #endregion

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
                            return BLGeneralUtil.return_ajax_string("2", OrdId);
                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry

                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                            {
                                ServerLog.Log(message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", message);
                            }
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            #region Create Addon Entry

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {

                                dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_N;
                                Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                if (!Result)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                            }

                            #endregion

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

                            if (dtSizeTypeMst.Rows.Count > 1)
                            {
                                dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                                dtSizeTypeMst.Rows[1]["load_inquiry_no"] = OrdId;
                            }
                            //                            return BLGeneralUtil.return_ajax_string("2", OrdId);
                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
                        {
                            #region Create Post Load Inquiry
                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = "-9999";
                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }



                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            #region Create Addon Entry

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {

                                dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_N;
                                Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                if (!Result)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                            }

                            #endregion

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
                        {
                            #region Create Post Load Inquiry
                            DataTable dtPost_Load_Inquiry = new DataTable();
                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, tload[0].load_inquiry_no, ref Message);
                            if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                            {
                                ServerLog.Log(message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", message);
                            }
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
                            ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            //if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                            //{
                            //    ServerLog.Log(message);
                            //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //    return BLGeneralUtil.return_ajax_string("0", message);
                            //}
                            //ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            if (dtSizeTypeMst.Rows.Count > 1)
                            {
                                dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                                dtSizeTypeMst.Rows[1]["load_inquiry_no"] = OrdId;
                            }

                            #region Create Addon Entry

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {

                                dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_N;
                                Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                if (!Result)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                            }

                            #endregion

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
                            // return BLGeneralUtil.return_ajax_string("1", OrdId);


                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
                        {
                            //string strpromocode = "";
                            DataTable dt_ordersByinq = GetOrders(tload[0].load_inquiry_no);
                            //if (dt_ordersByinq != null)
                            //strpromocode = dt_ordersByinq.Rows[0]["coupon_code"].ToString();

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            decimal DiscountPrice = 0;
                            if (tload[0].promocode.ToString().Trim() != "")
                            {

                                if (tload[0].promocode != "")
                                {
                                    decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                                    Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, SizeTypeCode, ref flatdiscount, ref PercentageDiscount, ref Msg);
                                    if (B1)
                                    {
                                        decimal Total_cost = Convert.ToDecimal(tload[0].required_price);
                                        if (flatdiscount != 0)
                                            DiscountPrice = Math.Round(flatdiscount);
                                        else if (PercentageDiscount != 0)
                                            DiscountPrice = Total_cost * (PercentageDiscount / 100);

                                        if (DiscountPrice != 0)
                                        {
                                            dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                                            dtSizeTypeMst.Rows[0]["Discount"] = DiscountPrice;
                                            if (!dtSizeTypeMst.Columns.Contains("Total_cost_without_discount"))
                                                dtSizeTypeMst.Columns.Add("Total_cost_without_discount");
                                            dtSizeTypeMst.Rows[0]["Total_cost_without_discount"] = tload[0].required_price;
                                        }
                                    }
                                    else
                                    {
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", Msg);
                                    }
                                }

                            }


                            #region Post Load Inquiry

                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = false;

                                if (tload[0].load_inquiry_no.Trim() != "")
                                {
                                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                                    ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
                                    ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = tload[0].load_inquiry_no;
                                    ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = tload[0].rate_type_flag;
                                    ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
                                    if (rate_type_flag == "P")
                                    {
                                        ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
                                        ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
                                    }
                                    else
                                    {
                                        ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                                        ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                                    }

                                    //ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = tload[0].IncludePackingCharge;
                                    // ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                                    ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                                    ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;
                                    ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                                    //ds_Post_load_enquiry.post_load_inquiry[0].modified_by = tload[0].created_by;
                                    ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.FLAG_Y;
                                    //ds_Post_load_enquiry.post_load_inquiry[0].modified_host = tload[0].created_host;
                                    //ds_Post_load_enquiry.post_load_inquiry[0].modified_device_id = tload[0].device_id;
                                    //ds_Post_load_enquiry.post_load_inquiry[0].modified_device_type = tload[0].device_type;
                                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                                    ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
                                    ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                                    ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());

                                    if (tload[0].required_price != null)
                                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);

                                    ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
                                    ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
                                    ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());

                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalTravelingRate = dtPostOrderParameter.Rows[0]["TotalTravelingRate"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].BaseRate = dtPostOrderParameter.Rows[0]["BaseRate"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalDriverRate = dtPostOrderParameter.Rows[0]["TotalDriverRate"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalLabourRate = dtPostOrderParameter.Rows[0]["TotalLabourRate"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalHandimanRate = dtPostOrderParameter.Rows[0]["TotalHandimanRate"].ToString();
                                    ds_Post_load_enquiry.post_load_inquiry[0].TotalSupervisorRate = dtPostOrderParameter.Rows[0]["TotalSupervisorRate"].ToString();

                                    ds_Post_load_enquiry.EnforceConstraints = true;



                                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                                    if (objBLReturnObject.ExecutionStatus != 1)
                                    {
                                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                    }

                                    #region Create Addon Entry

                                    if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                    {
                                        dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_N;
                                        Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                        if (!Result)
                                        {
                                            ServerLog.Log(message);
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return BLGeneralUtil.return_ajax_string("0", message);
                                        }
                                    }

                                    #endregion

                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 1;
                                    ServerLog.SuccessLog("Load Inquiry update With  Inquire ID : " + tload[0].load_inquiry_no);

                                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no.Trim();
                                }
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion



                        }


                        DataTable dt_order = new DataTable();
                        if (tload[0].Isfinalorder == "Y")
                        {
                            ServerLog.Log("CalculateRate(" + Convert.ToString(objPostOrder) + ")");


                            // string strpromocode = "";
                            DataTable dt_ordersByinq = GetOrders(tload[0].load_inquiry_no);
                            //if (dt_ordersByinq != null)
                            //    strpromocode = dt_ordersByinq.Rows[0]["coupon_code"].ToString();

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();


                            #region Save Coupon Code details


                            decimal DiscountPrice = 0;
                            if (tload[0].promocode.ToString().Trim() != "")
                            {

                                decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                                Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, SizeTypeCode, ref flatdiscount, ref PercentageDiscount, ref Msg);
                                if (B1)
                                {
                                    decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                    if (flatdiscount != 0)
                                        DiscountPrice = Math.Round(flatdiscount);
                                    else if (PercentageDiscount != 0)
                                        DiscountPrice = Total_cost * (PercentageDiscount / 100);

                                    if (DiscountPrice != 0)
                                    {
                                        dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                                    }

                                    Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tload[0].promocode, tload[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tload[0].created_by, tload[0].created_host, tload[0].device_id, tload[0].device_type, ref Msg);
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


                            string trakurl = "";
                            string cbmlink = "";

                            #region Post Load Inquiry

                            DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            DataTable dtPost_Load_Inquiry = null;
                            if (tload[0].load_inquiry_no.Trim() != "")
                            {
                                loadinquiryno = tload[0].load_inquiry_no;

                                trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                                cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

                                dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, loadinquiryno, ref Message);
                                if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }

                                ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
                                ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
                                ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                                ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
                                ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                                ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                                ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                ds_Post_load_enquiry.post_load_inquiry[0].modified_by = tload[0].created_by;
                                ds_Post_load_enquiry.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
                                ds_Post_load_enquiry.post_load_inquiry[0].modified_host = tload[0].created_host;
                                ds_Post_load_enquiry.post_load_inquiry[0].modified_device_id = tload[0].device_id;
                                ds_Post_load_enquiry.post_load_inquiry[0].modified_device_type = tload[0].device_type;

                                if (tload[0].payment_mode == Constant.PaymentModeCash)
                                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                                else
                                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;

                                ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
                                ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                                ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
                                ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
                                ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
                                ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                                ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;

                            }
                            else
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref loadinquiryno, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }

                                trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                                cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

                                ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
                                ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = loadinquiryno;
                                ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.Today;
                                ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                                ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                                ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                                ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                                ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                ds_Post_load_enquiry.post_load_inquiry[0].created_by = tload[0].created_by;
                                ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                                ds_Post_load_enquiry.post_load_inquiry[0].created_host = tload[0].created_host;
                                ds_Post_load_enquiry.post_load_inquiry[0].device_id = tload[0].device_id;
                                ds_Post_load_enquiry.post_load_inquiry[0].device_type = tload[0].device_type;
                                if (tload[0].payment_mode == Constant.PaymentModeCash)
                                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                                else
                                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;

                                ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
                                ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                                ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
                                ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                                ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
                                ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
                                ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
                                ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                                ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;


                            }

                            try
                            {
                                ds_Post_load_enquiry.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus != 1)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }
                            #endregion

                            #region Create Order Entry

                            DS_orders ds_orders = new DS_orders();
                            trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                            cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);



                            if (dt_ordersByinq == null)
                            {
                                ds_orders.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_orders.orders[0].order_id = OrdId;
                            }
                            else
                                ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

                            ds_orders.orders[0].shipper_id = tload[0].shipper_id;
                            ds_orders.orders[0].load_inquiry_no = loadinquiryno;
                            ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
                            ds_orders.orders[0].created_date = System.DateTime.UtcNow;
                            ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
                            ds_orders.orders[0].isassign_mover = Constant.Flag_No;
                            ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                            ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                            ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                            ds_orders.orders[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;//dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString(); 
                            ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                            ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                            ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                            ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                            if (rate_type_flag == "P")
                                ds_orders.orders[0].IncludePackingCharge = "N";
                            else
                            {
                                ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                                ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                            }

                            ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                            ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                            ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                            ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].order_type_flag = OrderTypeCode;
                            ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                            ds_orders.orders[0].goods_type_flag = goods_type_flag;
                            ds_orders.orders[0].trackurl = trakurl;
                            ds_orders.orders[0].cbmlink = cbmlink;
                            ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                            ds_orders.orders[0].Discount = DiscountPrice;
                            ds_orders.orders[0].coupon_code = tload[0].promocode;
                            ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());

                            if (tload[0].payment_mode == Constant.PaymentModeONLINE)
                            {
                                ds_orders.orders[0].payment_mode = Constant.PaymentModeONLINE;
                                ds_orders.orders[0].payment_status = Constant.FLAG_N;
                                ds_orders.orders[0].active_flag = Constant.Flag_No;
                                ds_orders.orders[0].IsCancel = Constant.Flag_No;
                                ds_orders.orders[0].IsDraft = Constant.Flag_No;
                            }
                            else
                            {
                                ds_orders.orders[0].payment_mode = tload[0].payment_mode;
                                ds_orders.orders[0].payment_status = Constant.FLAG_N;
                                ds_orders.orders[0].active_flag = Constant.Flag_Yes;
                                ds_orders.orders[0].IsCancel = Constant.Flag_No;
                                ds_orders.orders[0].IsDraft = Constant.Flag_No;
                            }

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                ds_orders.orders[0].IncludeAddonService = Constant.Flag_Yes;
                            else
                                ds_orders.orders[0].IncludeAddonService = Constant.Flag_No;

                            dt_order = ds_orders.orders;

                            try
                            {
                                ds_orders.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }
                            #endregion

                            #region Create Addon Entry

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {
                                dtPostOrderParameter.Rows[0]["AddonFlag"] = Constant.FLAG_Y;
                                Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                                if (!Result)
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                            }

                            #endregion

                            #region Generate Initial Quotation
                            DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
                            dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(ds_Post_load_enquiry.post_load_inquiry);
                            string quotid = "";
                            if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    ServerLog.Log(message);
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
                                objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                            #endregion

                            #region Insertbilling address
                            if (tload[0].Isupdatebillingadd == "Y")
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
                                    row.billing_name = tload[0].billing_name;
                                    row.shipper_id = tload[0].shipper_id;
                                    row.billing_add = tload[0].billing_add;
                                    row.active_flag = Constant.FLAG_Y;
                                    row.source_full_add = tload[0].source_full_add;
                                    row.destination_full_add = tload[0].destination_full_add;
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
                                        return BLGeneralUtil.return_ajax_string("0", Message);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    Message = ex.Message;
                                    return BLGeneralUtil.return_ajax_string("0", Message);
                                }
                            }
                            #endregion

                            #region Create CBM Entry
                            DS_CBM objDsCbm = new DS_CBM();
                            Master objmaster = new Master(); string DocNtficID = "";

                            DataTable dtappid = new CBMController().GetAppIDbySizetype(dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString());

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
                            tr.customer_name = GetUserdetailsByID(tload[0].shipper_id);
                            tr.customer_mobile = GetMobileNoByID(tload[0].shipper_id);
                            tr.customer_email = GetEmailByID(tload[0].shipper_id);
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

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

                            if (tload[0].payment_mode == Constant.PaymentModeCash)
                            {
                                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                                string shippername = GetUserdetailsByID(tload[0].shipper_id);

                                try
                                {
                                    string Msg = " Thank you! Your order no. " + tload[0].load_inquiry_no + " from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " to " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been confirmed. Your booking is scheduled for " + OrderShippingDatetime.ToString("dd-MM-yyyy HH:mm:ss tt");

                                    //string Msg = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;
                                    new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    ServerLog.Log("Error in send OTP on Completation ");
                                }

                                string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

                                ServerLog.Log(GetEmailByID(tload[0].shipper_id));

                                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
                                if (result["status"].ToString() == "0")
                                {
                                    ServerLog.Log("Error Sending Activation Email");
                                    // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                                }

                                try
                                {
                                    Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt_order);
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    //eturn BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }

                                try
                                {

                                    DBConnection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();

                                    String msg = ""; Byte status = 0;
                                    status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
                                    if (status == Constant.Status_Fail)
                                    {
                                        ServerLog.Log("Error in save notification Data ");
                                        DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        //return BLGeneralUtil.return_ajax_string("0", msg);
                                    }

                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }
                            }


                            // return "{"status":"1","message":"Order Generated Successfully","data":" + SendReceiveJSon.GetJson(ds_orders.orders) + "}";
                            return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(ds_orders.orders));
                        }
                        else
                        {
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        //[HttpPost]
        //public String SaveMovingHomeDetailsNew([FromBody]JObject objPostOrder)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    List<load_post_enquiry> tload = new List<load_post_enquiry>();
        //    decimal ddays = 0; int fraction = 0; string OrdId = ""; string loadinquiryno = "";
        //    DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
        //    TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();

        //    try
        //    {

        //        if (objPostOrder["PostOrderParameter"] != null)
        //        {
        //            tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
        //            dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
        //            dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
        //        }

        //        if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
        //        {
        //            return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
        //        }
        //        else
        //        {
        //            if (tload[0].order_type_flag == Constant.ORDERTYPECODEFORHOME)
        //            {
        //                DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

        //                DateTime before24hr = dubaiTime.AddHours(24);

        //                //if (shippingdatetime < before24hr)
        //                //{
        //                //    return BLGeneralUtil.return_ajax_string("3", "Please provide shipping date after 24 hour");
        //                //}
        //                if (shippingdatetime < before24hr)
        //                {
        //                    return BLGeneralUtil.return_ajax_string("0", " Please choose another date ");
        //                }
        //            }

        //            Decimal NoOfDay = 1.0M;
        //            DateTime OrderDate = DateTime.Today;
        //            String Message = String.Empty;
        //            String SizeTypeCode = String.Empty;
        //            String OrderTypeCode = String.Empty;
        //            String TruckTypeCode = String.Empty;
        //            Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

        //            if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
        //                OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

        //            if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
        //                TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();
        //            else if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
        //                if (dtPostOrderParameter.Columns.Contains("SizeTypeCode"))
        //                    SizeTypeCode = dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString();


        //            String rate_type_flag = "";
        //            if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
        //                rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

        //            String goods_type_flag = "";
        //            if (tload[0].goods_type_flag != "")
        //            {
        //                goods_type_flag = tload[0].goods_type_flag;
        //                if (goods_type_flag == Constant.Flag_Yes)
        //                    goods_type_flag = "H";
        //                else
        //                    goods_type_flag = "L";
        //            }

        //            DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

        //            Decimal TotalDistance = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
        //                TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

        //            String TotalDistanceUOM = String.Empty;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
        //                TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

        //            Decimal TimeToTravelInMinute = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
        //                TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

        //            //No Of Truck Edited by User
        //            int? NoOfTruck = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
        //                    NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

        //            //No Of Driver Edited by User
        //            int? NoOfDriver = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
        //                    NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

        //            //No Of Labour Edited by User
        //            int? NoOfLabour = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
        //                    NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);

        //            //No Of Handiman Edited by User
        //            int? NoOfHandiman = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
        //                    NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


        //            int? NoOfSupervisor = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
        //                    NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


        //            String IncludePackingCharge = "N";
        //            if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
        //                IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

        //            if (NoOfTruck != NoOfDriver)
        //                NoOfDriver = NoOfTruck;

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
        //                if (SizeTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied SizeTypeCode.");

        //            if (TotalDistance < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
        //            }
        //            else if (TotalDistanceUOM.Trim() == String.Empty)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
        //            }
        //            else if (TimeToTravelInMinute < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
        //            }
        //            else
        //            {

        //                TruckerMaster objTruckerMaster = new TruckerMaster();
        //                DataTable dtSizeTypeMst = new DataTable();

        //                if (rate_type_flag.ToString().ToUpper() != "N")
        //                {
        //                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                    {
        //                        dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
        //                        if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //                            return BLGeneralUtil.return_ajax_string("0", Message);
        //                    }
        //                    else
        //                    {
        //                        dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
        //                        if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //                            return BLGeneralUtil.return_ajax_string("0", Message);
        //                    }
        //                }
        //                else
        //                {

        //                    dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "P", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
        //                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //                        return BLGeneralUtil.return_ajax_string("0", Message);

        //                    DataTable dtSizeTypeMstBudget = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
        //                    if (dtSizeTypeMstBudget == null || dtSizeTypeMstBudget.Rows.Count <= 0)
        //                        return BLGeneralUtil.return_ajax_string("0", Message);

        //                    DataTable dtSizeTypeMstSuperSaver = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "S", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
        //                    if (dtSizeTypeMstSuperSaver == null || dtSizeTypeMstSuperSaver.Rows.Count <= 0)
        //                        return BLGeneralUtil.return_ajax_string("0", Message);

        //                    DataRow dr_temp_sizetypeBudget = dtSizeTypeMst.NewRow();
        //                    dr_temp_sizetypeBudget.ItemArray = dtSizeTypeMstBudget.Rows[0].ItemArray;
        //                    dtSizeTypeMst.Rows.Add(dr_temp_sizetypeBudget);

        //                    DataRow dr_temp_sizetypeSuperSaver = dtSizeTypeMst.NewRow();
        //                    dr_temp_sizetypeSuperSaver.ItemArray = dtSizeTypeMstSuperSaver.Rows[0].ItemArray;
        //                    dtSizeTypeMst.Rows.Add(dr_temp_sizetypeSuperSaver);
        //                }


        //                dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();



        //                if (tload[0].shipper_id != "")
        //                {
        //                    dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
        //                    if (dtbillingadd != null)
        //                    {
        //                        DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
        //                        if (dr.Length != 0)
        //                        {
        //                            dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
        //                            dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
        //                            dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["source_full_add"] = dr[0]["source_full_add"].ToString();
        //                            dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["destination_full_add"] = dr[0]["destination_full_add"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_name"] = "";
        //                            dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_add"] = "";
        //                            dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["source_full_add"] = "";
        //                            dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                        dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                        dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                        dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                    }
        //                }


        //                if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry

        //                    string Msg = "";

        //                    try
        //                    {

        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        dtPostOrderParameter.Rows[0]["shipper_id"] = "-9999";
        //                        Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Msg);

        //                        if (ObjStatus)
        //                        {
        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("2", dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Msg);
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry

        //                    string Msg = "";

        //                    try
        //                    {

        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Msg);

        //                        if (ObjStatus)
        //                        {
        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

        //                            // add load inquiry number in all quotation rows
        //                            if (dtSizeTypeMst.Rows.Count > 1)
        //                            {
        //                                dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
        //                                dtSizeTypeMst.Rows[1]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Msg);
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
        //                {
        //                    #region Create Post Load Inquiry

        //                    string Msg = "";

        //                    try
        //                    {

        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        dtPostOrderParameter.Columns.Add("cbmlink");
        //                        dtPostOrderParameter.Rows[0]["shipper_id"] = "-9999";
        //                        Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Msg);

        //                        if (ObjStatus)
        //                        {
        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Msg);
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    //DataTable dtPost_Load_Inquiry = new DataTable();
        //                    //DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    //dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, tload[0].load_inquiry_no, ref Message);
        //                    //if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                    //{
        //                    //    ServerLog.Log(message);
        //                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //    return BLGeneralUtil.return_ajax_string("0", message);
        //                    //}

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    try
        //                    {
        //                        dtPostOrderParameter.Columns.Add("cbmlink");
        //                        Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
        //                        if (ObjStatus)
        //                        {
        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Message);
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }


        //                    if (dtSizeTypeMst.Rows.Count > 1)
        //                    {
        //                        dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
        //                        dtSizeTypeMst.Rows[1]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
        //                        dtSizeTypeMst.Rows[2]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
        //                    }


        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
        //                {


        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode.ToString().Trim() != "")
        //                    {
        //                        decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                        Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, SizeTypeCode, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                        if (B1)
        //                        {
        //                            decimal Total_cost = Convert.ToDecimal(tload[0].required_price);
        //                            if (flatdiscount != 0)
        //                                DiscountPrice = Math.Round(flatdiscount);
        //                            else if (PercentageDiscount != 0)
        //                                DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                            if (DiscountPrice != 0)
        //                            {
        //                                dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                                dtSizeTypeMst.Rows[0]["Discount"] = DiscountPrice;
        //                                dtSizeTypeMst.Columns.Add("Total_cost_without_discount");
        //                                dtSizeTypeMst.Rows[0]["Total_cost_without_discount"] = tload[0].required_price;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Msg);
        //                        }

        //                    }


        //                    #region Post Load Inquiry

        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    try
        //                    {
        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        try
        //                        {
        //                            dtPostOrderParameter.Columns.Add("cbmlink");
        //                            Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
        //                            if (ObjStatus)
        //                            {
        //                                DBCommand.Transaction.Commit();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            }
        //                            else
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                        }


        //                        dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion



        //                }


        //                DataTable dt_order = new DataTable();
        //                if (tload[0].Isfinalorder == "Y")
        //                {
        //                    ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");


        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();


        //                    #region Save Coupon Code details


        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode.ToString().Trim() != "")
        //                    {
        //                        decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                        Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                        if (B1)
        //                        {
        //                            decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                            if (flatdiscount != 0)
        //                                DiscountPrice = Math.Round(flatdiscount);
        //                            else if (PercentageDiscount != 0)
        //                                DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                            if (DiscountPrice != 0)
        //                            {
        //                                dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                            }

        //                            Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tload[0].promocode, tload[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tload[0].created_by, tload[0].created_host, tload[0].device_id, tload[0].device_type, ref Msg);
        //                            if (B2 != 1)
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Msg);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", Msg);
        //                        }
        //                    }

        //                    #endregion


        //                    string trakurl = "";
        //                    string cbmlink = "";

        //                    #region Post Load Inquiry

        //                    if (tload[0].load_inquiry_no.Trim() != "")
        //                    {

        //                        loadinquiryno = tload[0].load_inquiry_no;

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        dtPostOrderParameter.Columns.Add("cbmlink");
        //                        dtPostOrderParameter.Rows[0]["cbmlink"] = cbmlink;
        //                        try
        //                        {
        //                            Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
        //                            if (!ObjStatus)
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                        }
        //                    }
        //                    else
        //                    {

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        dtPostOrderParameter.Columns.Add("cbmlink");
        //                        dtPostOrderParameter.Rows[0]["cbmlink"] = cbmlink;
        //                        try
        //                        {
        //                            Boolean ObjStatus = SavePostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
        //                            if (!ObjStatus)
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                        }


        //                    }

        //                    #endregion

        //                    #region Create Order Entry

        //                    DS_orders ds_orders = new DS_orders();
        //                    trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                    cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                    DataTable dt_ordersByinq = GetOrders(loadinquiryno);

        //                    if (dt_ordersByinq == null)
        //                    {
        //                        ds_orders.EnforceConstraints = false;
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
        //                        ds_orders.orders[0].order_id = OrdId;
        //                    }
        //                    else
        //                        ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

        //                    ds_orders.orders[0].shipper_id = tload[0].shipper_id;
        //                    ds_orders.orders[0].load_inquiry_no = loadinquiryno;
        //                    ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_orders.orders[0].created_date = System.DateTime.UtcNow;
        //                    ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
        //                    ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
        //                    ds_orders.orders[0].isassign_mover = Constant.Flag_No;
        //                    ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                    ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                    ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                    ds_orders.orders[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;//dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString(); 
        //                    ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                    ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                    ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                    ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

        //                    if (rate_type_flag == "P")
        //                        ds_orders.orders[0].IncludePackingCharge = "N";
        //                    else
        //                    {
        //                        ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
        //                        ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                    }

        //                    ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                    ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                    ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                    ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                    ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                    ds_orders.orders[0].order_type_flag = OrderTypeCode;
        //                    ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                    ds_orders.orders[0].goods_type_flag = goods_type_flag;
        //                    ds_orders.orders[0].trackurl = trakurl;
        //                    ds_orders.orders[0].cbmlink = cbmlink;
        //                    ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                    ds_orders.orders[0].Discount = DiscountPrice;
        //                    ds_orders.orders[0].coupon_code = tload[0].promocode;
        //                    ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());

        //                    if (tload[0].payment_mode == Constant.PaymentModeONLINE)
        //                    {
        //                        ds_orders.orders[0].payment_status = Constant.FLAG_N;
        //                        ds_orders.orders[0].active_flag = Constant.Flag_No;
        //                        ds_orders.orders[0].IsCancel = Constant.Flag_No;
        //                    }
        //                    else
        //                    {
        //                        ds_orders.orders[0].payment_status = Constant.FLAG_N;
        //                        ds_orders.orders[0].active_flag = Constant.Flag_Yes;
        //                        ds_orders.orders[0].IsCancel = Constant.Flag_No;

        //                    }

        //                    dt_order = ds_orders.orders;

        //                    try
        //                    {
        //                        ds_orders.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }
        //                    #endregion

        //                    #region Generate Initial Quotation
        //                    DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
        //                    dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(dtPostOrderParameter);
        //                    string quotid = "";
        //                    if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
        //                    {
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            ServerLog.Log(message);
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
        //                        objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus != 1)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }
        //                    }
        //                    #endregion

        //                    #region Insertbilling address
        //                    if (tload[0].Isupdatebillingadd == "Y")
        //                    {
        //                        try
        //                        {
        //                            DS_Shipper dS_shipper = new DS_Shipper();
        //                            string Msg = ""; string sr_no = "";
        //                            DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

        //                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
        //                            {
        //                                ServerLog.Log(message);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", message);
        //                            }
        //                            row.billing_srno = sr_no;
        //                            row.billing_name = tload[0].billing_name;
        //                            row.shipper_id = tload[0].shipper_id;
        //                            row.billing_add = tload[0].billing_add;
        //                            row.active_flag = Constant.FLAG_Y;
        //                            row.source_full_add = tload[0].source_full_add;
        //                            row.destination_full_add = tload[0].destination_full_add;
        //                            row.order_type_flag = Constant.ORDERTYPECODEFORHOME;
        //                            row.created_by = tload[0].created_by;
        //                            row.created_date = System.DateTime.UtcNow;

        //                            dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

        //                            try
        //                            {
        //                                dS_shipper.EnforceConstraints = true;
        //                            }
        //                            catch (ConstraintException ce)
        //                            {
        //                                Message = ce.Message;
        //                                ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Message = ex.Message;
        //                                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }

        //                            objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
        //                            if (objBLReturnObject.ExecutionStatus != 1)
        //                            {
        //                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            Message = ex.Message;
        //                            return BLGeneralUtil.return_ajax_string("0", Message);
        //                        }
        //                    }
        //                    #endregion


        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

        //                    if (tload[0].payment_mode == Constant.PaymentModeCash)
        //                    {
        //                        try
        //                        {
        //                            string Msg = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;
        //                            new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                            ServerLog.Log("Error in send OTP on Completation ");
        //                        }

        //                        string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

        //                        ServerLog.Log(GetEmailByID(tload[0].shipper_id));
        //                        string shippername = GetUserdetailsByID(tload[0].shipper_id);
        //                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
        //                        if (result["status"].ToString() == "0")
        //                        {
        //                            ServerLog.Log("Error Sending Activation Email");
        //                            // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
        //                        }

        //                        try
        //                        {

        //                            DBConnection.Open();
        //                            DBCommand.Transaction = DBConnection.BeginTransaction();

        //                            String msg = ""; Byte status = 0;
        //                            status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
        //                            if (status == Constant.Status_Fail)
        //                            {
        //                                ServerLog.Log("Error in save notification Data ");
        //                                DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", msg);
        //                            }

        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                        }


        //                    }



        //                    // return "{"status":"1","message":"Order Generated Successfully","data":" + SendReceiveJSon.GetJson(ds_orders.orders) + "}";
        //                    return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(ds_orders.orders));
        //                }
        //                else
        //                {
        //                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}

        public Boolean SavePostLoadInquiryDetails(ref IDbCommand command, ref DataTable dtPostOrderParameter, ref DataTable dtSizeTypeMst, ref String Message)
        {
            try
            {
                if (dtPostOrderParameter != null)
                {
                    Document objdoc = new Document();
                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    string OrdId = "";
                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                    ds_Post_load_enquiry.EnforceConstraints = false;


                    if (dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString() == "")
                    {
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref Message)) // New Driver Notification ID
                        {
                            ServerLog.Log(Message);
                            return false;
                        }
                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);

                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
                        dtPostOrderParameter.Rows[0]["load_inquiry_no"] = OrdId;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 

                    }
                    else
                    {
                        DataTable dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), ref Message);
                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }


                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;


                        if (dtPostOrderParameter.Rows[0]["payment_mode"].ToString() == Constant.PaymentModeCash)
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                        else
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;

                        if (dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString() == "P")
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
                            ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
                        }

                        if (dtPostOrderParameter.Rows[0]["required_price"].ToString() != "")
                            ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["required_price"].ToString());

                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = dtSizeTypeMst.Rows[0]["Discount"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Discount"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = dtPostOrderParameter.Rows[0]["cbmlink"].ToString();
                        if (dtPost_Load_Inquiry != null)
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["created_date"].ToString());
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPost_Load_Inquiry.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPost_Load_Inquiry.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPost_Load_Inquiry.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPost_Load_Inquiry.Rows[0]["device_type"].ToString();
                        }
                        else
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPostOrderParameter.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPostOrderParameter.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPostOrderParameter.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPostOrderParameter.Rows[0]["device_type"].ToString();
                        }


                    }

                    try
                    {
                        ds_Post_load_enquiry.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        return false;
                    }

                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_Post_load_enquiry.post_load_inquiry, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_Post_load_enquiry.post_load_inquiry.Rows.Count)
                    {
                        Message = "An error occurred while insert data in post_load_inquiry. " + objUpdateTableInfo.ErrorMessage;
                        return false;
                    }
                    else
                    {
                        dtPostOrderParameter = ds_Post_load_enquiry.post_load_inquiry;
                        Message = "post_load_inquiry data inserted successfully.";
                        return true;
                    }
                }
                else
                {
                    Message = " No Data To Save ";
                    return false;

                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        public string SaveOrderDraftDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<load_post_enquiry> tload = new List<load_post_enquiry>();
            decimal ddays = 0; int fraction = 0; string OrdId = ""; string loadinquiryno = "";
            DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
            TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();
            DataTable dtSizeTypeMst = new DataTable(); string Message = "";
            TruckerMaster objTruckerMaster = new TruckerMaster();

            if (objPostOrder["PostOrderParameter"] != null)
            {
                tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
                dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
                dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
                ServerLog.Log("SaveOrderDraftDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
            }

            if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
            {
                return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
            }
            else
            {
                Boolean rtnStatus = false;

                rtnStatus = ValidatePostloadJson(ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                if (!rtnStatus)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {

                    if (dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString() != "")
                        loadinquiryno = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();

                    string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                    string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

                    if (dtPostOrderParameter.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                    {
                        dtPostOrderParameter.Columns.Add("Hiretruck_TotalFuelRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["Hiretruck_TotalFuelRate"] = dtSizeTypeMst.Rows[0]["Hiretruck_TotalFuelRate"].ToString();
                    }

                    dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                    dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                    dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                    dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                    dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                    dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                    dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
                    dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    if (tload[0].order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    {
                        dtPostOrderParameter.Rows[0]["NoOfLabour"] = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        dtPostOrderParameter.Rows[0]["NoOfHandiman"] = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                    }

                    dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"] = dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_PT_Charge", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_PT_Charge"] = dtSizeTypeMst.Rows[0]["Total_PT_Charge"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_PT_Discount", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_PT_Discount"] = dtSizeTypeMst.Rows[0]["Total_PT_Discount"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_CL_Charge", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_CL_Charge"] = dtSizeTypeMst.Rows[0]["Total_CL_Charge"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_CL_Discount", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_CL_Discount"] = dtSizeTypeMst.Rows[0]["Total_CL_Discount"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_PEST_Charge", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_PEST_Charge"] = dtSizeTypeMst.Rows[0]["Total_PEST_Charge"].ToString();
                    dtPostOrderParameter.Columns.Add("Total_PEST_Discount", typeof(String));
                    dtPostOrderParameter.Rows[0]["Total_PEST_Discount"] = dtSizeTypeMst.Rows[0]["Total_PEST_Discount"].ToString();
                    //dtPostOrderParameter.Columns.Add("TotalAddServiceDiscount", typeof(String));		
                    dtPostOrderParameter.Rows[0]["TotalAddServiceDiscount"] = dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"].ToString();
                    //dtPostOrderParameter.Columns.Add("TotalAddServiceCharge", typeof(String));		
                    dtPostOrderParameter.Rows[0]["TotalAddServiceCharge"] = dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"].ToString();


                    if (tload[0].shipper_id != "")
                    {
                        dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
                        if (dtbillingadd != null)
                        {
                            DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
                            if (dr.Length != 0)
                            {
                                dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
                                dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
                                dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["source_full_add"] = dr[0]["source_full_add"].ToString();
                                dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["destination_full_add"] = dr[0]["destination_full_add"].ToString();
                            }
                            else
                            {
                                dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_name"] = "";
                                dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_add"] = "";
                                dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["source_full_add"] = "";
                                dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
                            }
                        }
                        else
                        {
                            dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                            dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                            dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                            dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                            dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                            dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                            dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                            dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                        }
                    }

                    DataTable dt_ordersByinq = GetOrders(loadinquiryno);

                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    #region Save Coupon Code details


                    decimal DiscountPrice = 0;
                    if (tload[0].promocode.ToString().Trim() != "")
                    {
                        decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                        Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Msg);
                        if (B1)
                        {
                            decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            if (flatdiscount != 0)
                                DiscountPrice = Math.Round(flatdiscount);
                            else if (PercentageDiscount != 0)
                                DiscountPrice = Total_cost * (PercentageDiscount / 100);

                            if (DiscountPrice != 0)
                            {
                                dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                            }

                            //Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tload[0].promocode, tload[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tload[0].created_by, tload[0].created_host, tload[0].device_id, tload[0].device_type, ref Msg);
                            //if (B2 != 1)
                            //{
                            //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //    return BLGeneralUtil.return_ajax_string("0", Msg);
                            //}
                        }
                        else
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", Msg);
                        }
                    }

                    #endregion

                    #region Post Load Inquiry


                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();

                    DataTable dtPost_Load_Inquiry = null;
                    dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, loadinquiryno, ref Message);
                    if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                    {
                        ServerLog.Log(message);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }




                    try
                    {


                        if (dtPost_Load_Inquiry == null)
                        {
                            ds_Post_load_enquiry.EnforceConstraints = false;
                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref loadinquiryno, ref message)) // New Driver Notification ID
                            {
                                ServerLog.Log(message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", message);
                            }
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = loadinquiryno;
                        }
                        else
                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);

                        ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].created_by = tload[0].created_by;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_host = tload[0].created_host;
                        ds_Post_load_enquiry.post_load_inquiry[0].device_id = tload[0].device_id;
                        ds_Post_load_enquiry.post_load_inquiry[0].device_type = tload[0].device_type;

                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtSizeTypeMst.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) + DiscountPrice;
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;
                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.Flag_Yes;

                        ds_Post_load_enquiry.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }

                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                    else
                    {
                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    }


                    #endregion

                    #region Create Order Entry

                    DS_orders ds_orders = new DS_orders();
                    trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                    cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);



                    if (dt_ordersByinq != null)
                    {
                        dt_ordersByinq.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                        dt_ordersByinq.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                        dt_ordersByinq.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                        dt_ordersByinq.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                        dt_ordersByinq.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                        dt_ordersByinq.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                        dt_ordersByinq.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();

                    }
                    try
                    {
                        DBConnection.Open();
                        DBCommand.Transaction = DBConnection.BeginTransaction();




                        if (dt_ordersByinq == null)
                        {
                            ds_orders.EnforceConstraints = false;
                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                            {
                                ServerLog.Log(message);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", message);
                            }
                            ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                            ds_orders.orders[0].order_id = OrdId;
                        }
                        else
                        {
                            dtPostOrderParameter.Columns.Add("order_id");
                            dtPostOrderParameter.Rows[0]["order_id"] = dt_ordersByinq.Rows[0]["order_id"].ToString();
                            ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                        }

                        ds_orders.orders[0].shipper_id = tload[0].shipper_id;
                        ds_orders.orders[0].load_inquiry_no = loadinquiryno;
                        ds_orders.orders[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString()); ;
                        ds_orders.orders[0].created_date = System.DateTime.UtcNow;
                        ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                        ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
                        ds_orders.orders[0].isassign_mover = Constant.Flag_No;
                        ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                        ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                        if (dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString() == "P")
                            ds_orders.orders[0].IncludePackingCharge = "N";
                        else
                        {
                            ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                            ds_orders.orders[0].TotalPackingCharge = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                        }

                        ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                        ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                        ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                        ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                        ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_orders.orders[0].order_type_flag = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();
                        ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_orders.orders[0].goods_type_flag = dtSizeTypeMst.Rows[0]["goods_type_flag"].ToString();
                        ds_orders.orders[0].trackurl = trakurl;
                        ds_orders.orders[0].cbmlink = cbmlink;
                        ds_orders.orders[0].Discount = DiscountPrice;
                        ds_orders.orders[0].coupon_code = tload[0].promocode;
                        ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) + DiscountPrice;
                        ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_orders.orders[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_orders.orders[0].payment_mode = tload[0].payment_mode;
                        ds_orders.orders[0].payment_status = Constant.FLAG_N;
                        ds_orders.orders[0].active_flag = Constant.Flag_No;
                        ds_orders.orders[0].IsCancel = Constant.Flag_No;
                        ds_orders.orders[0].IsDraft = Constant.Flag_Yes;

                        if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            ds_orders.orders[0].IncludeAddonService = Constant.Flag_Yes;
                        else
                            ds_orders.orders[0].IncludeAddonService = Constant.Flag_No;

                        ds_orders.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }

                    objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
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

                    if (tload[0].Isupdatebillingadd == "Y")
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
                            row.billing_name = tload[0].billing_name;
                            row.shipper_id = tload[0].shipper_id;
                            row.billing_add = tload[0].billing_add;
                            row.active_flag = Constant.FLAG_Y;
                            row.source_full_add = tload[0].source_full_add;
                            row.destination_full_add = tload[0].destination_full_add;
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
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            Message = ex.Message;
                            return BLGeneralUtil.return_ajax_string("0", Message);
                        }
                    }
                    #endregion

                    #region Create Addon Entry

                    if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                    {
                        Boolean Result = SaveAddOnServiceDetails(ref DBCommand, ref dtPostOrderParameter, ref dtSizeTypeMst, ref Message);
                        if (!Result)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                    }

                    #endregion

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("1", " Order Saved as Draft ");
                }
            }
        }

        public Boolean ValidatePostloadJson(ref DataTable dtPostOrderParameter, ref DataTable dtSizeTypeMst, ref string Message)
        {
            Decimal NoOfDay = 1.0M;
            DateTime OrderDate = DateTime.Today;
            Message = String.Empty;
            String SizeTypeCode = String.Empty;
            String OrderTypeCode = String.Empty;
            String TruckTypeCode = String.Empty;
            TimeSpan Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

            if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

            if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
                TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
            {
                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();
                dtPostOrderParameter.Rows[0]["SizeTypeCode"] = SizeTypeCode;
            }
            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
            {
                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();
                dtPostOrderParameter.Rows[0]["SizeTypeCode"] = SizeTypeCode;
            }
            else if (OrderTypeCode == Constant.ORDER_TYPE_CODE_HIRETRUCK)
            {
                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();
                dtPostOrderParameter.Rows[0]["SizeTypeCode"] = SizeTypeCode;
            }
            else if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                if (dtPostOrderParameter.Columns.Contains("SizeTypeCode"))
                    SizeTypeCode = dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString();


            String rate_type_flag = "";
            if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

            String goods_type_flag = "";
            if (dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString() != "")
            {
                goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                if (goods_type_flag == Constant.Flag_Yes)
                    goods_type_flag = "H";
                else
                    goods_type_flag = "L";
            }


            DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());


            if (dtPostOrderParameter.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
            {
                DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

                DateTime before24hr = dubaiTime.AddHours(1);

                if (shippingdatetime < before24hr)
                {
                    Message = " Please choose Minimum 1 hour lead time ";
                    return false;
                }
            }


            Decimal TotalDistance = -1;
            if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

            String TotalDistanceUOM = String.Empty;
            if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

            Decimal TimeToTravelInMinute = -1;
            if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

            //No Of Truck Edited by User
            int? NoOfTruck = null;
            if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                    NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

            //No Of Driver Edited by User
            int? NoOfDriver = null;
            if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                    NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

            //No Of Labour Edited by User
            int? NoOfLabour = null;
            if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                {
                    NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);
                    if (NoOfLabour != 0)
                        if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                            NoOfLabour = NoOfLabour - NoOfTruck;
                }
            //No Of Handiman Edited by User
            int? NoOfHandiman = null;
            if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                    NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


            int? NoOfSupervisor = null;
            if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                    NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


            String IncludePackingCharge = "N";
            if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

            if (NoOfTruck != NoOfDriver)
                NoOfDriver = NoOfTruck;

            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                if (OrderTypeCode.Trim() == String.Empty)
                {
                    Message = " Please supplied TruckTypeCode.";
                    return false;
                }
            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                if (OrderTypeCode.Trim() == String.Empty)
                {
                    Message = " Please supplied TruckTypeCode.";
                    return false;
                }

            if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                if (OrderTypeCode.Trim() == String.Empty)
                {
                    Message = " Please supplied SizeTypeCode.";
                    return false;
                }

            if (TotalDistance < 0)
            {
                Message = " Please supplied TotalDistance. ";
                return false;
            }
            else if (TotalDistanceUOM.Trim() == String.Empty)
            {
                Message = "Please supplied TotalDistance Unit";
                return false;
            }
            else if (TimeToTravelInMinute < 0)
            {
                Message = "Please supplied Time To Travel In Minute.";
                return false;
            }
            else
            {

                TruckerMaster objTruckerMaster = new TruckerMaster();
                string Msg = "";

                if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                {
                    goods_type_flag = "H";
                    dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Msg);
                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    {
                        Message = Msg;
                        return false;
                    }
                    else
                    {
                        dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                        Message = Msg;
                        return true;
                    }
                }
                else if (OrderTypeCode == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                {
                    goods_type_flag = "H";
                    dtSizeTypeMst = objTruckerMaster.CalculateRateHireTruck(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Msg);
                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    {
                        Message = Msg;
                        return false;
                    }
                    else
                    {
                        dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                        Message = Msg;
                        return true;
                    }
                }
                else
                {
                    dtSizeTypeMst = objTruckerMaster.CalculateRate(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Msg);
                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    {
                        Message = Msg;
                        return false;
                    }
                    else
                    {
                        Message = Msg;
                        return true;
                    }
                }
            }
        }

        #endregion

        #region Moving Goods services

        // if someone cancel online payment then get order details
        [HttpGet]
        public string GetpostloadinquiryByIDGoods(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],post_load_inquiry.*,SizeTypeMatrix.* FROM  post_load_inquiry " +
                            " join SizeTypeMatrix on post_load_inquiry.SizeTypeCode=SizeTypeMatrix.SizeTypeCode and SizeTypeMatrix.rate_type_flag=post_load_inquiry.rate_type_flag " +
                            "  where load_inquiry_no=@inqid   ";
            //String query1 = " SELECT convert(char(5), load_inquiry_shipping_time, 108) [load_inquiry_shipping_time],* FROM  post_load_inquiry where load_inquiry_no=@inqid  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }


        //[HttpPost]
        //public String SaveMovingGoodsDetails_old([FromBody]JObject objPostOrder)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    List<load_post_enquiry> tload = new List<load_post_enquiry>();
        //    decimal ddays = 0; int fraction = 0; string OrdId = ""; string loadinquiryno = "";
        //    DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
        //    TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();

        //    try
        //    {

        //        if (objPostOrder["PostOrderParameter"] != null)
        //        {
        //            tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
        //            dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
        //            dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
        //        }

        //        if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
        //        {
        //            return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
        //        }
        //        else
        //        {
        //            Decimal NoOfDay = 1.0M;
        //            DateTime OrderDate = DateTime.Today;
        //            String Message = String.Empty;
        //            String SizeTypeCode = String.Empty;
        //            String OrderTypeCode = String.Empty;
        //            String TruckTypeCode = String.Empty;
        //            Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

        //            if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
        //                OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

        //            if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
        //                TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            String rate_type_flag = "";
        //            if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
        //                rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

        //            String goods_type_flag = "";
        //            if (tload[0].goods_type_flag != "")
        //            {
        //                goods_type_flag = tload[0].goods_type_flag;
        //                if (goods_type_flag == Constant.Flag_Yes)
        //                    goods_type_flag = "H";
        //                else
        //                    goods_type_flag = "L";
        //            }

        //            DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

        //            if (tload[0].order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //            {
        //                DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

        //                DateTime before24hr = dubaiTime.AddHours(1);

        //                if (shippingdatetime < before24hr)
        //                {
        //                    return BLGeneralUtil.return_ajax_string("0", " Please choose Minimum 1 hour lead time ");
        //                }
        //            }


        //            Decimal TotalDistance = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
        //                TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

        //            String TotalDistanceUOM = String.Empty;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
        //                TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

        //            Decimal TimeToTravelInMinute = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
        //                TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

        //            //No Of Truck Edited by User
        //            int? NoOfTruck = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
        //                    NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

        //            //No Of Driver Edited by User
        //            int? NoOfDriver = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
        //                    NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

        //            //No Of Labour Edited by User
        //            int? NoOfLabour = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
        //                    NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);

        //            //No Of Handiman Edited by User
        //            int? NoOfHandiman = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
        //                    NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


        //            int? NoOfSupervisor = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
        //                    NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


        //            String IncludePackingCharge = "N";
        //            if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
        //                IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

        //            if (NoOfTruck != NoOfDriver)
        //                NoOfDriver = NoOfTruck;

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (TotalDistance < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
        //            }
        //            else if (TotalDistanceUOM.Trim() == String.Empty)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
        //            }
        //            else if (TimeToTravelInMinute < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
        //            }
        //            else
        //            {

        //                TruckerMaster objTruckerMaster = new TruckerMaster();
        //                DataTable dtSizeTypeMst = new DataTable();

        //                if (rate_type_flag.ToString().ToUpper() != "N")
        //                {
        //                    dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
        //                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //                        return BLGeneralUtil.return_ajax_string("0", Message);

        //                }

        //                dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("Total_cost", typeof(String));
        //                dtPostOrderParameter.Rows[0]["Total_cost"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
        //                dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                dtPostOrderParameter.Rows[0]["NoOfLabour"] = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();


        //                //if (tload[0].shipper_id != "")
        //                //{
        //                //    dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
        //                //    DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
        //                //    if (dr.Length != 0)
        //                //    {
        //                //        dtPostOrderParameter.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
        //                //        dtPostOrderParameter.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
        //                //        dtPostOrderParameter.Rows[0]["source_full_add"] = dr[0]["source_full_add"].ToString();
        //                //        dtPostOrderParameter.Rows[0]["destination_full_add"] = dr[0]["destination_full_add"].ToString();
        //                //    }
        //                //}



        //                if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = "-9999";
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
        //                    tload[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
        //                    return BLGeneralUtil.return_ajax_string("2", OrdId);
        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.FLAG_Y;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

        //                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
        //                    loadinquiryno = OrdId;
        //                    //                            return BLGeneralUtil.return_ajax_string("2", OrdId);
        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DataTable dtPost_Load_Inquiry = new DataTable();
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, tload[0].load_inquiry_no, ref Message);
        //                    if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    //if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    //{
        //                    //    ServerLog.Log(message);
        //                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //    return BLGeneralUtil.return_ajax_string("0", message);
        //                    //}
        //                    //ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;
        //                    // tload[0].load_inquiry_no = OrdId;
        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
        //                    // return BLGeneralUtil.return_ajax_string("1", OrdId);


        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
        //                {
        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    #region apply coupancode


        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode != null)
        //                    {
        //                        if (tload[0].promocode.ToString().Trim() != "")
        //                        {
        //                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                            Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                            if (B1)
        //                            {
        //                                decimal Total_cost = Convert.ToDecimal(tload[0].required_price);
        //                                if (flatdiscount != 0)
        //                                    DiscountPrice = Math.Round(flatdiscount);
        //                                else if (PercentageDiscount != 0)
        //                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                                if (DiscountPrice != 0)
        //                                {
        //                                    dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Msg);
        //                            }
        //                        }
        //                    }

        //                    #endregion

        //                    #region Post Load Inquiry

        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = false;
        //                        // DataTable dtPost_Load_Inquiry = null;
        //                        if (tload[0].load_inquiry_no.Trim() != "")
        //                        {
        //                            //dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, tload[0].load_inquiry_no.Trim(), ref Message);
        //                            //if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                            //{
        //                            //    ServerLog.Log(message);
        //                            //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            //    return BLGeneralUtil.return_ajax_string("0", message);
        //                            //}

        //                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                            ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = tload[0].rate_type_flag;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                            if (rate_type_flag == "P")
        //                            {
        //                                ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
        //                                ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
        //                            }
        //                            else
        //                            {
        //                                ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
        //                                ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                            }

        //                            //ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = tload[0].IncludePackingCharge;
        //                            // ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                            ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                            //ds_Post_load_enquiry.post_load_inquiry[0].modified_by = tload[0].created_by;
        //                            //ds_Post_load_enquiry.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
        //                            //ds_Post_load_enquiry.post_load_inquiry[0].modified_host = tload[0].created_host;
        //                            //ds_Post_load_enquiry.post_load_inquiry[0].modified_device_id = tload[0].device_id;
        //                            //ds_Post_load_enquiry.post_load_inquiry[0].modified_device_type = tload[0].device_type;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].goods_details = tload[0].goods_details;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.FLAG_Y;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;

        //                            ds_Post_load_enquiry.EnforceConstraints = true;



        //                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                            if (objBLReturnObject.ExecutionStatus != 1)
        //                            {
        //                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                            }

        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 1;
        //                            ServerLog.SuccessLog("Load Inquiry update With  Inquire ID : " + tload[0].load_inquiry_no);

        //                            dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                            dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no.Trim();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion
        //                }

        //                DataTable dt_order = new DataTable();
        //                if (tload[0].Isfinalorder == "Y")
        //                {

        //                    ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode != null)
        //                    {
        //                        if (tload[0].promocode.ToString().Trim() != "")
        //                        {
        //                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                            Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                            if (B1)
        //                            {
        //                                decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                                if (flatdiscount != 0)
        //                                    DiscountPrice = Math.Round(flatdiscount);
        //                                else if (PercentageDiscount != 0)
        //                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                                if (DiscountPrice != 0)
        //                                {
        //                                    dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                                }

        //                                Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tload[0].promocode, tload[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tload[0].created_by, tload[0].created_host, tload[0].device_id, tload[0].device_type, ref Msg);
        //                                if (B2 != 1)
        //                                {
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", Msg);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Msg);
        //                            }
        //                        }
        //                    }

        //                    string trakurl = "";
        //                    string cbmlink = "";

        //                    #region Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    DataTable dtPost_Load_Inquiry = null;
        //                    if (tload[0].load_inquiry_no.Trim() != "")
        //                    {
        //                        loadinquiryno = tload[0].load_inquiry_no;

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                        dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, loadinquiryno, ref Message);
        //                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_by = tload[0].created_by;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_host = tload[0].created_host;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_device_id = tload[0].device_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_device_type = tload[0].device_type;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;

        //                    }
        //                    else
        //                    {
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref loadinquiryno, ref message)) // New Driver Notification ID
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = loadinquiryno;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.Today;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_by = tload[0].created_by;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_host = tload[0].created_host;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].device_id = tload[0].device_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].device_type = tload[0].device_type;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;


        //                    }

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus != 1)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }
        //                    #endregion

        //                    #region Create Order Entry

        //                    DS_orders ds_orders = new DS_orders();
        //                    trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                    cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                    DataTable dt_ordersByinq = GetOrders(loadinquiryno);

        //                    if (dt_ordersByinq == null)
        //                    {
        //                        ds_orders.EnforceConstraints = false;
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
        //                        ds_orders.orders[0].order_id = OrdId;
        //                    }
        //                    else
        //                        ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

        //                    ds_orders.orders[0].shipper_id = tload[0].shipper_id;
        //                    ds_orders.orders[0].load_inquiry_no = loadinquiryno;
        //                    ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_orders.orders[0].created_date = System.DateTime.UtcNow;
        //                    ds_orders.orders[0].active_flag = Constant.Flag_Yes;
        //                    ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
        //                    ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
        //                    ds_orders.orders[0].isassign_mover = Constant.Flag_No;
        //                    ds_orders.orders[0].IsCancel = Constant.Flag_No;
        //                    ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                    ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                    ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                    ds_orders.orders[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                    ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                    ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                    ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                    ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

        //                    if (rate_type_flag == "P")
        //                        ds_orders.orders[0].IncludePackingCharge = "N";
        //                    else
        //                    {
        //                        ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
        //                        ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                    }

        //                    ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                    ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                    ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                    ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                    ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                    ds_orders.orders[0].order_type_flag = OrderTypeCode;
        //                    ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                    ds_orders.orders[0].goods_type_flag = goods_type_flag;
        //                    ds_orders.orders[0].trackurl = trakurl;
        //                    ds_orders.orders[0].cbmlink = cbmlink;
        //                    ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                    ds_orders.orders[0].Discount = DiscountPrice;
        //                    ds_orders.orders[0].coupon_code = tload[0].promocode;
        //                    ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                    ds_orders.orders[0].payment_status = Constant.FLAG_N;


        //                    dt_order = ds_orders.orders;
        //                    try
        //                    {
        //                        ds_orders.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }
        //                    #endregion

        //                    #region Generate Initial Quotation
        //                    DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
        //                    dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(ds_Post_load_enquiry.post_load_inquiry);
        //                    string quotid = "";
        //                    if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
        //                    {
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            ServerLog.Log(message);
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
        //                        objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus != 1)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }
        //                    }
        //                    #endregion

        //                    #region Insertbilling address

        //                    //if (dtbillingadd == null)
        //                    //{
        //                    //    try
        //                    //    {
        //                    //        DS_Shipper dS_shipper = new DS_Shipper();
        //                    //        dS_shipper.EnforceConstraints = false;

        //                    //        DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();
        //                    //        row.billing_name = tload[0].billing_name;
        //                    //        row.billing_add = tload[0].billing_name;
        //                    //        row.active_flag = tload[0].billing_name;
        //                    //        row.source_full_add = tload[0].billing_name;
        //                    //        row.destination_full_add = tload[0].billing_name;
        //                    //        row.created_by = tload[0].billing_name;
        //                    //        row.created_date = System.DateTime.UtcNow;

        //                    //        dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

        //                    //        try
        //                    //        {
        //                    //            dS_shipper.EnforceConstraints = true;
        //                    //        }
        //                    //        catch (ConstraintException ce)
        //                    //        {
        //                    //            Message = ce.Message;
        //                    //            ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
        //                    //            return BLGeneralUtil.return_ajax_string("0", Message);
        //                    //        }
        //                    //        catch (Exception ex)
        //                    //        {
        //                    //            Message = ex.Message;
        //                    //            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                    //            return BLGeneralUtil.return_ajax_string("0", Message);
        //                    //        }

        //                    //        objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
        //                    //        if (objBLReturnObject.ExecutionStatus != 1)
        //                    //        {
        //                    //            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    //            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    //        }
        //                    //    }
        //                    //    catch (Exception ex)
        //                    //    {
        //                    //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                    //        Message = ex.Message;
        //                    //        return BLGeneralUtil.return_ajax_string("0", Message);
        //                    //    }
        //                    //}

        //                    #endregion

        //                    //try
        //                    //{

        //                    //    String msg = ""; Byte status = 0;
        //                    //    status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, truckid, driverid, ownerid, ref msg);
        //                    //    if (status == Constant.Status_Fail)
        //                    //    {
        //                    //        ServerLog.Log("Error in save notification Data ");
        //                    //        DBCommand.Transaction.Rollback();
        //                    //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //        return BLGeneralUtil.return_ajax_string("0", msg);
        //                    //    }
        //                    //}
        //                    //catch (Exception ex)
        //                    //{
        //                    //    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //    return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    //}


        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;

        //                    dt_order.Columns.Add("TimeForLoadingInMinute");
        //                    dt_order.Rows[0]["TimeForLoadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"].ToString();
        //                    dt_order.Columns.Add("TimeForUnloadingInMinute");
        //                    dt_order.Rows[0]["TimeForUnloadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"].ToString();
        //                    dt_order.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
        //                    dt_order.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();




        //                    if (tload[0].payment_mode == Constant.PaymentModeCash)
        //                    {
        //                        try
        //                        {
        //                            string Msg = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + tload[0].load_inquiry_no;
        //                            new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));

        //                            string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

        //                            ServerLog.Log(GetEmailByID(tload[0].shipper_id));
        //                            string shippername = GetUserdetailsByID(tload[0].shipper_id);
        //                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
        //                            if (result["status"].ToString() == "0")
        //                            {
        //                                ServerLog.Log("Error Sending Activation Email");
        //                                // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                            ServerLog.Log("Error in send OTP on Completation ");
        //                        }
        //                    }


        //                    if (tload[0].order_type_flag == "GN")
        //                        return BLGeneralUtil.return_ajax_statusdata("0", "No driver available please try again", SendReceiveJSon.GetJson(dt_order));
        //                    else
        //                        return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(dt_order));
        //                }
        //                else
        //                {
        //                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}

        //[HttpPost]
        //public String SaveMovingGoodsDetails_New([FromBody]JObject objPostOrder)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    List<load_post_enquiry> tload = new List<load_post_enquiry>();
        //    decimal ddays = 0; int fraction = 0; string OrdId = ""; string loadinquiryno = "";
        //    DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
        //    TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();

        //    try
        //    {

        //        if (objPostOrder["PostOrderParameter"] != null)
        //        {
        //            tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
        //            dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
        //            dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);


        //            if (tload[0].load_inquiry_no == "")
        //                ServerLog.Log("SaveMovingGoodsDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
        //        }

        //        if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
        //        {
        //            return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
        //        }
        //        else
        //        {
        //            Decimal NoOfDay = 1.0M;
        //            DateTime OrderDate = DateTime.Today;
        //            String Message = String.Empty;
        //            String SizeTypeCode = String.Empty;
        //            String OrderTypeCode = String.Empty;
        //            String TruckTypeCode = String.Empty;
        //            Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

        //            if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
        //                OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

        //            if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
        //                TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

        //            String rate_type_flag = "";
        //            if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
        //                rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

        //            String goods_type_flag = "";
        //            if (tload[0].goods_type_flag != "")
        //            {
        //                goods_type_flag = tload[0].goods_type_flag;
        //                if (goods_type_flag == Constant.Flag_Yes)
        //                    goods_type_flag = "H";
        //                else
        //                    goods_type_flag = "L";
        //            }

        //            DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

        //            if (tload[0].order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //            {
        //                DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

        //                DateTime before24hr = dubaiTime.AddHours(1);

        //                if (shippingdatetime < before24hr)
        //                {
        //                    return BLGeneralUtil.return_ajax_string("0", " Please choose Minimum 1 hour lead time ");
        //                }
        //            }


        //            Decimal TotalDistance = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
        //                TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

        //            String TotalDistanceUOM = String.Empty;
        //            if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
        //                TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

        //            Decimal TimeToTravelInMinute = -1;
        //            if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
        //                TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

        //            //No Of Truck Edited by User
        //            int? NoOfTruck = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
        //                    NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

        //            //No Of Driver Edited by User
        //            int? NoOfDriver = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
        //                    NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

        //            //No Of Labour Edited by User
        //            int? NoOfLabour = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
        //                    NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);

        //            //No Of Handiman Edited by User
        //            int? NoOfHandiman = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
        //                    NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


        //            int? NoOfSupervisor = null;
        //            if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
        //                if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
        //                    NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


        //            String IncludePackingCharge = "N";
        //            if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
        //                IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

        //            if (NoOfTruck != NoOfDriver)
        //                NoOfDriver = NoOfTruck;

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
        //                if (TruckTypeCode.Trim() == String.Empty)
        //                    return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

        //            if (TotalDistance < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
        //            }
        //            else if (TotalDistanceUOM.Trim() == String.Empty)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
        //            }
        //            else if (TimeToTravelInMinute < 0)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
        //            }
        //            else
        //            {

        //                TruckerMaster objTruckerMaster = new TruckerMaster();
        //                DataTable dtSizeTypeMst = new DataTable();

        //                if (rate_type_flag.ToString().ToUpper() != "N")
        //                {
        //                    dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
        //                    if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //                        return BLGeneralUtil.return_ajax_string("0", Message);

        //                }

        //                dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
        //                dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
        //                dtPostOrderParameter.Columns.Add("Total_cost", typeof(String));
        //                dtPostOrderParameter.Rows[0]["Total_cost"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
        //                dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                dtPostOrderParameter.Rows[0]["NoOfLabour"] = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                dtPostOrderParameter.Rows[0]["NoOfHandiman"] = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

        //                if (tload[0].shipper_id != "")
        //                {
        //                    dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
        //                    if (dtbillingadd != null)
        //                    {
        //                        DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
        //                        if (dr.Length != 0)
        //                        {
        //                            dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
        //                            dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
        //                            dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["source_full_add"] = dr[0]["source_full_add"].ToString();
        //                            dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["destination_full_add"] = dr[0]["destination_full_add"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_name"] = "";
        //                            dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["billing_add"] = "";
        //                            dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["source_full_add"] = "";
        //                            dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                            dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                        dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                        dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                        dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
        //                        dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                    }
        //                }



        //                if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = "-9999";
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
        //                    tload[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
        //                    return BLGeneralUtil.return_ajax_string("2", OrdId);
        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;



        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

        //                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
        //                    loadinquiryno = OrdId;
        //                    //                            return BLGeneralUtil.return_ajax_string("2", OrdId);
        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
        //                {
        //                    #region Create Post Load Inquiry
        //                    DataTable dtPost_Load_Inquiry = new DataTable();
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, tload[0].load_inquiry_no, ref Message);
        //                    if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                    {
        //                        ServerLog.Log(message);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", message);
        //                    }
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.QUOTE_NOT_SELECTED; // New Orders // Quote Selected and sent to shipper 
        //                    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                    ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    //if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                    //{
        //                    //    ServerLog.Log(message);
        //                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    //    return BLGeneralUtil.return_ajax_string("0", message);
        //                    //}
        //                    //ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }

        //                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;
        //                    // tload[0].load_inquiry_no = OrdId;
        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;
        //                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);
        //                    // return BLGeneralUtil.return_ajax_string("1", OrdId);


        //                    #endregion
        //                }

        //                if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
        //                {
        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    #region apply coupancode


        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode != null)
        //                    {
        //                        if (tload[0].promocode.ToString().Trim() != "")
        //                        {
        //                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                            Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                            if (B1)
        //                            {
        //                                decimal Total_cost = Convert.ToDecimal(tload[0].required_price);
        //                                if (flatdiscount != 0)
        //                                    DiscountPrice = Math.Round(flatdiscount);
        //                                else if (PercentageDiscount != 0)
        //                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                                if (DiscountPrice != 0)
        //                                {
        //                                    dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Msg);
        //                            }
        //                        }
        //                    }

        //                    #endregion

        //                    #region Post Load Inquiry

        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = false;
        //                        // DataTable dtPost_Load_Inquiry = null;
        //                        if (tload[0].load_inquiry_no.Trim() != "")
        //                        {

        //                            ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                            ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = tload[0].rate_type_flag;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                            if (rate_type_flag == "P")
        //                            {
        //                                ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
        //                                ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
        //                            }
        //                            else
        //                            {
        //                                ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
        //                                ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                            }

        //                            ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                            ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                            ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
        //                            ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                            ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].goods_details = tload[0].goods_details;
        //                            ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.FLAG_Y;

        //                            ds_Post_load_enquiry.EnforceConstraints = true;



        //                            objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                            if (objBLReturnObject.ExecutionStatus != 1)
        //                            {
        //                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                            }

        //                            DBCommand.Transaction.Commit();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 1;
        //                            ServerLog.SuccessLog("Load Inquiry update With  Inquire ID : " + tload[0].load_inquiry_no);

        //                            dtSizeTypeMst.Columns.Add("load_inquiry_no");
        //                            dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no.Trim();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    #endregion
        //                }

        //                DataTable dt_order = new DataTable();
        //                if (tload[0].Isfinalorder == "Y")
        //                {

        //                    ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");

        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    decimal DiscountPrice = 0;
        //                    if (tload[0].promocode != null)
        //                    {
        //                        if (tload[0].promocode.ToString().Trim() != "")
        //                        {
        //                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                            Boolean B1 = objTruckerMaster.IsCouponValid(tload[0].promocode, tload[0].shipper_id, tload[0].load_inquiry_no, tload[0].load_inquiry_no, System.DateTime.UtcNow, tload[0].order_type_flag, tload[0].rate_type_flag, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                            if (B1)
        //                            {
        //                                decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                                if (flatdiscount != 0)
        //                                    DiscountPrice = Math.Round(flatdiscount);
        //                                else if (PercentageDiscount != 0)
        //                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                                if (DiscountPrice != 0)
        //                                {
        //                                    dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
        //                                }

        //                                Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tload[0].promocode, tload[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tload[0].created_by, tload[0].created_host, tload[0].device_id, tload[0].device_type, ref Msg);
        //                                if (B2 != 1)
        //                                {
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", Msg);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", Msg);
        //                            }
        //                        }
        //                    }

        //                    string trakurl = "";
        //                    string cbmlink = "";

        //                    #region Post Load Inquiry
        //                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
        //                    ds_Post_load_enquiry.EnforceConstraints = false;
        //                    DataTable dtPost_Load_Inquiry = null;
        //                    if (tload[0].load_inquiry_no.Trim() != "")
        //                    {
        //                        loadinquiryno = tload[0].load_inquiry_no;

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                        dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, loadinquiryno, ref Message);
        //                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPost_Load_Inquiry.Rows[0]);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].AcceptChanges();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_by = tload[0].created_by;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_date = System.DateTime.UtcNow;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_host = tload[0].created_host;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_device_id = tload[0].device_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].modified_device_type = tload[0].device_type;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;

        //                    }
        //                    else
        //                    {
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref loadinquiryno, ref message)) // New Driver Notification ID
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }

        //                        trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                        cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = tload[0].shipper_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = loadinquiryno;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.Today;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
        //                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_by = tload[0].created_by;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].created_host = tload[0].created_host;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].device_id = tload[0].device_id;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].device_type = tload[0].device_type;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = OrderTypeCode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = goods_type_flag;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = tload[0].promocode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = tload[0].payment_mode;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
        //                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;


        //                    }

        //                    try
        //                    {
        //                        ds_Post_load_enquiry.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_Post_load_enquiry.post_load_inquiry, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus != 1)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }
        //                    #endregion

        //                    #region Create Order Entry

        //                    DS_orders ds_orders = new DS_orders();
        //                    trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
        //                    cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(loadinquiryno);

        //                    DataTable dt_ordersByinq = GetOrders(loadinquiryno);

        //                    if (dt_ordersByinq == null)
        //                    {
        //                        ds_orders.EnforceConstraints = false;
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                        {
        //                            ServerLog.Log(message);
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
        //                        ds_orders.orders[0].order_id = OrdId;
        //                    }
        //                    else
        //                        ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

        //                    ds_orders.orders[0].shipper_id = tload[0].shipper_id;
        //                    ds_orders.orders[0].load_inquiry_no = loadinquiryno;
        //                    ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
        //                    ds_orders.orders[0].created_date = System.DateTime.UtcNow;
        //                    ds_orders.orders[0].active_flag = Constant.Flag_Yes;
        //                    ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
        //                    ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
        //                    ds_orders.orders[0].isassign_mover = Constant.Flag_No;
        //                    ds_orders.orders[0].IsCancel = Constant.Flag_No;
        //                    ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
        //                    ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
        //                    ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
        //                    ds_orders.orders[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
        //                    ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
        //                    ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
        //                    ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
        //                    ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

        //                    if (rate_type_flag == "P")
        //                        ds_orders.orders[0].IncludePackingCharge = "N";
        //                    else
        //                    {
        //                        ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
        //                        ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
        //                    }

        //                    ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
        //                    ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
        //                    ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
        //                    ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
        //                    ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                    ds_orders.orders[0].order_type_flag = OrderTypeCode;
        //                    ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
        //                    ds_orders.orders[0].goods_type_flag = goods_type_flag;
        //                    ds_orders.orders[0].trackurl = trakurl;
        //                    ds_orders.orders[0].cbmlink = cbmlink;
        //                    ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
        //                    ds_orders.orders[0].Discount = DiscountPrice;
        //                    ds_orders.orders[0].coupon_code = tload[0].promocode;
        //                    ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
        //                    ds_orders.orders[0].payment_status = Constant.FLAG_N;


        //                    dt_order = ds_orders.orders;
        //                    try
        //                    {
        //                        ds_orders.EnforceConstraints = true;
        //                    }
        //                    catch (ConstraintException ce)
        //                    {
        //                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ce.Message);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                    }

        //                    objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
        //                    if (objBLReturnObject.ExecutionStatus == 2)
        //                    {
        //                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 2;
        //                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                    }
        //                    #endregion

        //                    #region Generate Initial Quotation
        //                    DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
        //                    dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(ds_Post_load_enquiry.post_load_inquiry);
        //                    string quotid = "";
        //                    if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
        //                    {
        //                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
        //                        {
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            ServerLog.Log(message);
        //                            return BLGeneralUtil.return_ajax_string("0", message);
        //                        }
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
        //                        dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
        //                        objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus != 1)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }
        //                    }
        //                    #endregion

        //                    #region Insertbilling address
        //                    if (tload[0].Isupdatebillingadd != null)
        //                    {
        //                        if (tload[0].Isupdatebillingadd == "Y")
        //                        {
        //                            try
        //                            {
        //                                DS_Shipper dS_shipper = new DS_Shipper();
        //                                string Msg = ""; string sr_no = "";
        //                                DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

        //                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
        //                                {
        //                                    ServerLog.Log(message);
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", message);
        //                                }
        //                                row.billing_srno = sr_no;
        //                                row.billing_name = tload[0].billing_name;
        //                                row.shipper_id = tload[0].shipper_id;
        //                                row.billing_add = tload[0].billing_add;
        //                                row.active_flag = Constant.FLAG_Y;
        //                                row.source_full_add = tload[0].source_full_add;
        //                                row.destination_full_add = tload[0].destination_full_add;
        //                                row.order_type_flag = Constant.ORDERTYPECODEFORHOME;
        //                                row.created_by = tload[0].created_by;
        //                                row.created_date = System.DateTime.UtcNow;

        //                                dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

        //                                try
        //                                {
        //                                    dS_shipper.EnforceConstraints = true;
        //                                }
        //                                catch (ConstraintException ce)
        //                                {
        //                                    Message = ce.Message;
        //                                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", Message);
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    Message = ex.Message;
        //                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", Message);
        //                                }

        //                                objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
        //                                if (objBLReturnObject.ExecutionStatus != 1)
        //                                {
        //                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", Message);
        //                                }

        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                Message = ex.Message;
        //                                return BLGeneralUtil.return_ajax_string("0", Message);
        //                            }
        //                        }

        //                    }
        //                    #endregion


        //                    DBCommand.Transaction.Commit();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 1;

        //                    dt_order.Columns.Add("TimeForLoadingInMinute");
        //                    dt_order.Rows[0]["TimeForLoadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"].ToString();
        //                    dt_order.Columns.Add("TimeForUnloadingInMinute");
        //                    dt_order.Rows[0]["TimeForUnloadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"].ToString();
        //                    dt_order.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
        //                    dt_order.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();




        //                    if (tload[0].payment_mode == Constant.PaymentModeCash)
        //                    {
        //                        try
        //                        {
        //                            string Msg = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + tload[0].load_inquiry_no;
        //                            new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));

        //                            string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

        //                            ServerLog.Log(GetEmailByID(tload[0].shipper_id));
        //                            string shippername = GetUserdetailsByID(tload[0].shipper_id);
        //                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
        //                            if (result["status"].ToString() == "0")
        //                            {
        //                                ServerLog.Log("Error Sending Activation Email");
        //                                // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
        //                            }

        //                            try
        //                            {

        //                                DBConnection.Open();
        //                                DBCommand.Transaction = DBConnection.BeginTransaction();

        //                                String msg = ""; Byte status = 0;
        //                                status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
        //                                if (status == Constant.Status_Fail)
        //                                {
        //                                    ServerLog.Log("Error in save notification Data ");
        //                                    DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    return BLGeneralUtil.return_ajax_string("0", msg);
        //                                }

        //                                DBCommand.Transaction.Commit();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //                            ServerLog.Log("Error in send OTP on Completation ");
        //                        }
        //                    }


        //                    if (tload[0].order_type_flag == "GN")
        //                        return BLGeneralUtil.return_ajax_statusdata("0", "No driver available please try again", SendReceiveJSon.GetJson(dt_order));
        //                    else
        //                        return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(dt_order));
        //                }
        //                else
        //                {
        //                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}


        [HttpPost]
        public String SaveMovingGoodsDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<load_post_enquiry> tload = new List<load_post_enquiry>();
            string OrdId = ""; string loadinquiryno = "";
            DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
            TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();
            DataTable dtMultiDrop = new DataTable();
            try
            {

                if (objPostOrder["PostOrderParameter"] != null)
                {
                    tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);

                    if (tload[0].load_inquiry_no == "")
                        ServerLog.Log("SaveMovinGoodsDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {

                    DateTime OrderDate = DateTime.Today;
                    String Message = String.Empty;
                    String SizeTypeCode = String.Empty;
                    String OrderTypeCode = String.Empty;
                    String TruckTypeCode = String.Empty;
                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    #region Validate data


                    if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                        OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

                    if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
                        TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                        SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    String rate_type_flag = "";
                    if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                        rate_type_flag = Constant.RATE_TYPE_FLAG_STANDERD;

                    String goods_type_flag = "";
                    if (tload[0].goods_type_flag != "")
                    {
                        goods_type_flag = tload[0].goods_type_flag;
                        if (goods_type_flag == Constant.Flag_Yes)
                        {
                            dtPostOrderParameter.Rows[0]["goods_type_flag"] = "H";
                            goods_type_flag = "H";
                        }
                        else
                        {
                            dtPostOrderParameter.Rows[0]["goods_type_flag"] = "L";
                            goods_type_flag = "L";
                        }
                    }
                    else
                    {
                        dtPostOrderParameter.Rows[0]["goods_type_flag"] = "H";
                        goods_type_flag = "H";
                    }

                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                    if (tload[0].order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    {
                        DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                        DateTime before24hr = dubaiTime.AddHours(1);


                        //TimeSpan startTime = TimeSpan.Parse("21:00");
                        //TimeSpan EndTime = TimeSpan.Parse("09:00");
                        //TimeSpan CurrentTime = TimeSpan.Parse(dubaiTime.ToString("HH:mm"));
                        //TimeSpan ShippingTime = TimeSpan.Parse(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToString("HH:mm"));

                        //if (CurrentTime >= startTime && CurrentTime <= TimeSpan.Parse("24:00:00"))
                        //{
                        //    if (ShippingTime <= startTime && ShippingTime >= EndTime)
                        //    {
                        //    }
                        //    else
                        //    {
                        //        return BLGeneralUtil.return_ajax_string("0", " Please choose time after 9:00 ");
                        //    }
                        //}
                        //else if (CurrentTime <= EndTime)
                        //{

                        //}

                        if (shippingdatetime < before24hr)
                        {
                            return BLGeneralUtil.return_ajax_string("0", " Please choose Minimum 1 hour lead time ");
                        }
                    }


                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    //No Of Truck Edited by User
                    int? NoOfTruck = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                        if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                            NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

                    //No Of Driver Edited by User
                    int? NoOfDriver = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                        if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                            NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

                    //No Of Labour Edited by User
                    int? NoOfLabour = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                        if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                            NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);

                    //No Of Handiman Edited by User
                    int? NoOfHandiman = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                        if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                            NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


                    int? NoOfSupervisor = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                        if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                            NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


                    String IncludePackingCharge = "N";
                    if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                        IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

                    if (NoOfTruck != NoOfDriver)
                        NoOfDriver = NoOfTruck;

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please supplied TruckTypeCode.");

                    #endregion

                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == "")
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {

                        TruckerMaster objTruckerMaster = new TruckerMaster();
                        DataTable dtSizeTypeMst = new DataTable();

                        if (rate_type_flag.ToString().ToUpper() != "N")
                        {
                            dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error SaveMovingGoodsDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }

                        }

                        dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_cost", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_cost"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
                        dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                        dtPostOrderParameter.Rows[0]["NoOfLabour"] = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        dtPostOrderParameter.Rows[0]["NoOfHandiman"] = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                        if (dtSizeTypeMst.Columns.Contains("goods_weight"))
                            dtSizeTypeMst.Rows[0]["goods_weight"] = dtPostOrderParameter.Rows[0]["goods_weight"].ToString();
                        else
                        {
                            dtSizeTypeMst.Columns.Add("goods_weight");
                            dtSizeTypeMst.Rows[0]["goods_weight"] = dtPostOrderParameter.Rows[0]["goods_weight"].ToString();
                        }

                        if (dtSizeTypeMst.Columns.Contains("goods_weightUOM"))
                            dtSizeTypeMst.Rows[0]["goods_weightUOM"] = dtPostOrderParameter.Rows[0]["goods_weightUOM"].ToString();
                        else
                        {
                            dtSizeTypeMst.Columns.Add("goods_weightUOM");
                            dtSizeTypeMst.Rows[0]["goods_weightUOM"] = dtPostOrderParameter.Rows[0]["goods_weightUOM"].ToString();
                        }
                        //if (dtSizeTypeMst.Columns.Contains("goods_weight"))goods_weightUOM

                        if (tload[0].shipper_id != "")
                        {
                            #region Billing Address


                            dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
                            if (dtbillingadd != null)
                            {
                                DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
                                if (dr.Length != 0)
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = ""; //dr[0]["source_full_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = ""; //dr[0]["destination_full_add"].ToString();
                                }
                                else
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = "";
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = "";
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = "";
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
                                }
                            }
                            else
                            {
                                dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                                dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                                dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                                dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Rows[0]["shipper_id"] = "-9999";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsGoods(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("2", dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }
                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                Boolean ObjStatus = SavePostLoadInquiryDetailsGoods(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Columns.Add("cbmlink");
                                dtPostOrderParameter.Rows[0]["cbmlink"] = "";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsGoods(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Columns.Add("cbmlink");
                                dtPostOrderParameter.Rows[0]["cbmlink"] = "";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsGoods(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        DataTable dt_order = new DataTable();
                        if (tload[0].Isfinalorder == "Y")
                        {
                            string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tload[0].load_inquiry_no);
                            string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(tload[0].load_inquiry_no);

                            ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");

                            loadinquiryno = tload[0].load_inquiry_no;

                            string Msg = "";

                            #region Create PostLoad Entry

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Columns.Add("cbmlink");
                                dtPostOrderParameter.Rows[0]["cbmlink"] = "";

                                dtPostOrderParameter.Columns.Add("trakurl");
                                dtPostOrderParameter.Rows[0]["trakurl"] = trakurl;


                                Boolean ObjStatus = SavePostLoadInquiryDetailsGoods(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion

                            #region Create Order Entry

                            DS_orders ds_orders = new DS_orders();
                            DataTable dt_ordersByinq = GetOrders(loadinquiryno);

                            if (dt_ordersByinq == null)
                            {
                                ds_orders.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_orders.orders[0].order_id = OrdId;
                            }
                            else
                                ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

                            ds_orders.orders[0].shipper_id = tload[0].shipper_id;
                            ds_orders.orders[0].load_inquiry_no = loadinquiryno;
                            ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
                            ds_orders.orders[0].created_date = System.DateTime.UtcNow;
                            ds_orders.orders[0].active_flag = Constant.Flag_Yes;
                            ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
                            ds_orders.orders[0].isassign_mover = Constant.Flag_No;
                            ds_orders.orders[0].IsCancel = Constant.Flag_No;
                            ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                            ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                            ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                            ds_orders.orders[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                            ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                            ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                            ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                            ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                            if (rate_type_flag == "P")
                                ds_orders.orders[0].IncludePackingCharge = "N";
                            else
                            {
                                ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                                ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                            }

                            ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                            ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                            ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                            ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].order_type_flag = OrderTypeCode;
                            ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                            ds_orders.orders[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                            ds_orders.orders[0].trackurl = trakurl;
                            ds_orders.orders[0].cbmlink = cbmlink;
                            ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                            // ds_orders.orders[0].Discount =  Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());;
                            ds_orders.orders[0].coupon_code = tload[0].promocode;
                            ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].payment_status = Constant.FLAG_N;
                            ds_orders.orders[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();


                            dt_order = ds_orders.orders;
                            try
                            {
                                ds_orders.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }
                            #endregion

                            #region Generate Initial Quotation
                            DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
                            dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(dt_order);
                            string quotid = "";
                            if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    ServerLog.Log(message);
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
                                objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                            #endregion

                            #region Insertbilling address
                            if (tload[0].Isupdatebillingadd != null)
                            {
                                if (tload[0].Isupdatebillingadd == "Y")
                                {
                                    try
                                    {
                                        DS_Shipper dS_shipper = new DS_Shipper();
                                        Msg = ""; string sr_no = "";
                                        DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

                                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
                                        {
                                            ServerLog.Log(message);
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return BLGeneralUtil.return_ajax_string("0", message);
                                        }
                                        row.billing_srno = sr_no;
                                        row.billing_name = tload[0].billing_name;
                                        row.shipper_id = tload[0].shipper_id;
                                        row.billing_add = tload[0].billing_add;
                                        row.active_flag = Constant.FLAG_Y;
                                        row.source_full_add = tload[0].source_full_add;
                                        row.destination_full_add = tload[0].destination_full_add;
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
                                            return BLGeneralUtil.return_ajax_string("0", Message);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        Message = ex.Message;
                                        return BLGeneralUtil.return_ajax_string("0", Message);
                                    }
                                }

                            }
                            #endregion


                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;

                            dt_order.Columns.Add("TimeForLoadingInMinute");
                            dt_order.Rows[0]["TimeForLoadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"].ToString();
                            dt_order.Columns.Add("TimeForUnloadingInMinute");
                            dt_order.Rows[0]["TimeForUnloadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"].ToString();
                            dt_order.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                            dt_order.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();




                            if (tload[0].payment_mode == Constant.PaymentModeCash)
                            {
                                #region mails and Send Notifications

                                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                                string shippername = GetUserdetailsByID(tload[0].shipper_id);

                                try
                                {
                                    Msg = " Thank you! Your order no. " + tload[0].load_inquiry_no + " from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " to " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been confirmed. Your booking is scheduled for " + OrderShippingDatetime.ToString("dd-MM-yyyy HH:mm:ss tt");

                                    //Msg = " Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + tload[0].load_inquiry_no;
                                    new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));

                                    string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

                                    ServerLog.Log(GetEmailByID(tload[0].shipper_id));
                                    //string shippername = GetUserdetailsByID(tload[0].shipper_id);
                                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
                                    if (result["status"].ToString() == "0")
                                    {
                                        ServerLog.Log("Error Sending Activation Email");
                                        // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                                    }


                                    try
                                    {
                                        Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt_order);
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                    }

                                    try
                                    {

                                        DBConnection.Open();
                                        DBCommand.Transaction = DBConnection.BeginTransaction();

                                        String msg = ""; Byte status = 0;
                                        status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
                                        if (status == Constant.Status_Fail)
                                        {
                                            ServerLog.Log("Error in save notification Data ");
                                            DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return BLGeneralUtil.return_ajax_string("0", msg);
                                        }

                                        DBCommand.Transaction.Commit();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    ServerLog.Log("Error in send OTP on Completation ");
                                }

                                #endregion
                            }


                            if (tload[0].order_type_flag == "GN")
                                return BLGeneralUtil.return_ajax_statusdata("0", "No driver available please try again", SendReceiveJSon.GetJson(dt_order));
                            else
                                return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(dt_order));
                        }
                        else
                        {
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public Boolean SavePostLoadInquiryDetailsGoods(ref IDbCommand command, ref DataTable dtPostOrderParameter, List<load_post_enquiry> JobjOrder, ref DataTable dtSizeTypeMst, ref String Message)
        {
            try
            {
                if (dtPostOrderParameter != null)
                {
                    Document objdoc = new Document();
                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    string OrdId = "";
                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                    DS_orders dsorder = new DS_orders();
                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                    ds_Post_load_enquiry.EnforceConstraints = false;

                    if (JobjOrder[0].load_inquiry_no == "")
                    {

                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref Message)) // New Driver Notification ID
                        {
                            ServerLog.Log(Message);
                            return false;
                        }

                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
                        dtPostOrderParameter.Rows[0]["load_inquiry_no"] = OrdId;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 


                        if (dtSizeTypeMst != null)
                        {
                            if (!dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                            {
                                dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                            }
                            else
                            {
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                            }

                        }
                    }
                    else
                    {

                        #region apply coupancode


                        decimal DiscountPrice = 0;
                        if (JobjOrder[0].promocode != null)
                        {
                            if (JobjOrder[0].promocode.ToString().Trim() != "")
                            {
                                decimal flatdiscount = 0; decimal PercentageDiscount = 0;
                                Boolean B1 = objTruckerMaster.IsCouponValid(JobjOrder[0].promocode, JobjOrder[0].shipper_id, JobjOrder[0].load_inquiry_no, JobjOrder[0].load_inquiry_no, System.DateTime.UtcNow, JobjOrder[0].order_type_flag, JobjOrder[0].rate_type_flag, dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Message);
                                if (B1)
                                {
                                    decimal Total_cost = Convert.ToDecimal(JobjOrder[0].required_price);
                                    if (flatdiscount != 0)
                                        DiscountPrice = Math.Round(flatdiscount);
                                    else if (PercentageDiscount != 0)
                                        DiscountPrice = Total_cost * (PercentageDiscount / 100);

                                    if (DiscountPrice != 0)
                                    {
                                        dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                                        dtSizeTypeMst.Rows[0]["Discount"] = DiscountPrice;
                                        if (!dtSizeTypeMst.Columns.Contains("Total_cost_without_discount"))
                                            dtSizeTypeMst.Columns.Add("Total_cost_without_discount");
                                        dtSizeTypeMst.Rows[0]["Total_cost_without_discount"] = JobjOrder[0].required_price;

                                    }

                                    if (JobjOrder[0].Isfinalorder == Constant.FLAG_Y)
                                    {
                                        Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, JobjOrder[0].promocode, JobjOrder[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, JobjOrder[0].created_by, JobjOrder[0].created_host, JobjOrder[0].device_id, JobjOrder[0].device_type, ref Message);
                                        if (B2 != 1)
                                        {
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return false;
                                }
                            }
                        }

                        #endregion

                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(JobjOrder[0].load_inquiry_no);
                        string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(JobjOrder[0].load_inquiry_no);

                        DataTable dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), ref Message);
                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }


                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;


                        //if (dtPostOrderParameter.Rows[0]["payment_mode"].ToString() == Constant.PaymentModeCash)
                        //    ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.FLAG_N;
                        //else
                        //    ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.FLAG_Y;

                        if (dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString() == "P")
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
                            ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
                        }

                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = dtPostOrderParameter.Rows[0]["required_price"].ToString() == "" ? 0 : Convert.ToDecimal(dtPostOrderParameter.Rows[0]["required_price"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = dtSizeTypeMst.Rows[0]["Discount"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Discount"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;
                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = dtPostOrderParameter.Rows[0]["promocode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;

                        if (dtPost_Load_Inquiry != null)
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = dtPost_Load_Inquiry.Rows[0]["active_flag"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = dtPost_Load_Inquiry.Rows[0]["IsDraft"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPost_Load_Inquiry.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPost_Load_Inquiry.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPost_Load_Inquiry.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPost_Load_Inquiry.Rows[0]["device_type"].ToString();
                        }
                        else
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPostOrderParameter.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPostOrderParameter.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPostOrderParameter.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPostOrderParameter.Rows[0]["device_type"].ToString();
                        }

                        //ds_Post_load_enquiry.post_load_inquiry[0].BaseRate = dtPostOrderParameter.Rows[0]["BaseRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalTravelingRate = dtPostOrderParameter.Rows[0]["TotalTravelingRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalDriverRate = dtPostOrderParameter.Rows[0]["TotalDriverRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalLabourRate = dtPostOrderParameter.Rows[0]["TotalLabourRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalHandimanRate = dtPostOrderParameter.Rows[0]["TotalHandimanRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalSupervisorRate = dtPostOrderParameter.Rows[0]["TotalSupervisorRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                        //ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString();

                        //ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].goods_details = dtPostOrderParameter.Rows[0]["goods_details"].ToString();

                    }

                    try
                    {
                        ds_Post_load_enquiry.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        return false;
                    }

                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_Post_load_enquiry.post_load_inquiry, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_Post_load_enquiry.post_load_inquiry.Rows.Count)
                    {
                        Message = "An error occurred while insert data in post_load_inquiry. " + objUpdateTableInfo.ErrorMessage;
                        return false;
                    }
                    else
                    {
                        dtPostOrderParameter = ds_Post_load_enquiry.post_load_inquiry;
                        Message = "post_load_inquiry data inserted successfully.";
                        return true;
                    }
                }
                else
                {
                    Message = " No Data To Save ";
                    return false;

                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        #endregion

        #region Hire Truck services

        [HttpPost]
        public String SaveHireTruckDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<load_post_enquiry> tload = new List<load_post_enquiry>();
            string OrdId = ""; string loadinquiryno = "";
            DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
            TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();
            DataTable dtMultiDrop = new DataTable();
            try
            {

                if (objPostOrder["PostOrderParameter"] != null)
                {
                    tload = objPostOrder["PostOrderParameter"].ToObject<List<load_post_enquiry>>();
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);

                    if (tload[0].load_inquiry_no == "")
                        ServerLog.Log("SaveHireTruckDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {

                    DateTime OrderDate = DateTime.Today;
                    String Message = String.Empty;
                    String SizeTypeCode = String.Empty;
                    String OrderTypeCode = String.Empty;
                    String TruckTypeCode = String.Empty;
                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    #region Validate data


                    if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                        OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

                    if (dtPostOrderParameter.Columns.Contains("TruckTypeCode"))
                        TruckTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        SizeTypeCode = dtPostOrderParameter.Rows[0]["TruckTypeCode"].ToString();

                    String rate_type_flag = "";
                    if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                        rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                    else
                        rate_type_flag = Constant.RATE_TYPE_FLAG_STANDERD;


                    String goods_type_flag = "";
                    if (tload[0].goods_type_flag != "")
                    {
                        goods_type_flag = tload[0].goods_type_flag;
                        if (goods_type_flag == Constant.Flag_Yes)
                        {
                            dtPostOrderParameter.Rows[0]["goods_type_flag"] = "H";
                            goods_type_flag = "H";
                        }
                        else
                        {
                            dtPostOrderParameter.Rows[0]["goods_type_flag"] = "L";
                            goods_type_flag = "L";
                        }
                    }
                    else
                    {
                        dtPostOrderParameter.Rows[0]["goods_type_flag"] = "H";
                        goods_type_flag = "H";
                    }

                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                    if (tload[0].order_type_flag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    {
                        DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

                        DateTime before24hr = dubaiTime.AddHours(1);

                        if (shippingdatetime < before24hr)
                        {
                            return BLGeneralUtil.return_ajax_string("0", " Please choose Minimum 1 hour lead time ");
                        }
                    }


                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    //No Of Truck Edited by User
                    int? NoOfTruck = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                        if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                            NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

                    //No Of Driver Edited by User
                    int? NoOfDriver = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                        if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                            NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

                    //No Of Labour Edited by User
                    int? NoOfLabour = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                        if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                            NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);

                    //No Of Handiman Edited by User
                    int? NoOfHandiman = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                        if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                            NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


                    int? NoOfSupervisor = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                        if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                            NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


                    String IncludePackingCharge = "N";
                    if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                        IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

                    if (NoOfTruck != NoOfDriver)
                        NoOfDriver = NoOfTruck;

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please Select Truck Type ");


                    #endregion

                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == "")
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {

                        TruckerMaster objTruckerMaster = new TruckerMaster();
                        DataTable dtSizeTypeMst = new DataTable();

                        if (rate_type_flag.ToString().ToUpper() != "N")
                        {
                            dtSizeTypeMst = objTruckerMaster.CalculateRateHireTruck(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                            {
                                ServerLog.Log("SaveHireTruckDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }

                        }

                        // return BLGeneralUtil.return_ajax_string("0", Message);

                        dtPostOrderParameter.Columns.Add("Hiretruck_TotalFuelRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["Hiretruck_TotalFuelRate"] = dtSizeTypeMst.Rows[0]["Hiretruck_TotalFuelRate"].ToString();

                        if (dtPostOrderParameter.Columns.Contains("Hiretruck_MaxKM"))
                            dtPostOrderParameter.Rows[0]["Hiretruck_MaxKM"] = dtSizeTypeMst.Rows[0]["Hiretruck_MaxKM"].ToString();
                        else
                        {
                            dtPostOrderParameter.Columns.Add("Hiretruck_MaxKM");
                            dtPostOrderParameter.Rows[0]["Hiretruck_MaxKM"] = dtSizeTypeMst.Rows[0]["Hiretruck_MaxKM"].ToString();
                        }

                        dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
                        dtPostOrderParameter.Columns.Add("Total_cost", typeof(String));
                        dtPostOrderParameter.Rows[0]["Total_cost"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
                        dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"] = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                        dtPostOrderParameter.Rows[0]["NoOfLabour"] = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        dtPostOrderParameter.Rows[0]["NoOfHandiman"] = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();


                        if (tload[0].shipper_id != "")
                        {
                            #region Billing Address


                            dtbillingadd = new ShipperController().GetuserbillingaddressbyId(tload[0].shipper_id);
                            if (dtbillingadd != null)
                            {
                                DataRow[] dr = dtbillingadd.Select("active_flag='Y'");
                                if (dr.Length != 0)
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = dr[0]["billing_name"].ToString();
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = dr[0]["billing_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = "";// dr[0]["source_full_add"].ToString();
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = "";// dr[0]["destination_full_add"].ToString();
                                }
                                else
                                {
                                    dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_name"] = "";
                                    dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["billing_add"] = "";
                                    dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["source_full_add"] = "";
                                    dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                    dtSizeTypeMst.Rows[0]["destination_full_add"] = "";
                                }
                            }
                            else
                            {
                                dtSizeTypeMst.Columns.Add("billing_name", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_name"] = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                                dtSizeTypeMst.Columns.Add("billing_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["billing_add"] = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                                dtSizeTypeMst.Columns.Add("source_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["source_full_add"] = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                                dtSizeTypeMst.Columns.Add("destination_full_add", typeof(String));
                                dtSizeTypeMst.Rows[0]["destination_full_add"] = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            }

                            #endregion
                        }

                        // if user not loged in and request for first time (click on get quote button)
                        if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Rows[0]["shipper_id"] = "-9999";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsHireTruck(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_statusdata("2", dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), SendReceiveJSon.GetJson(dtSizeTypeMst));
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }
                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                Boolean ObjStatus = SavePostLoadInquiryDetailsHireTruck(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                    if (dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                    else
                                    {
                                        dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                    }
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Columns.Add("cbmlink");
                                dtPostOrderParameter.Rows[0]["cbmlink"] = "";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsHireTruck(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                    if (dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;
                                    else
                                    {
                                        dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                        dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no; ;
                                    }


                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dtPostOrderParameter.Columns.Add("cbmlink");
                                dtPostOrderParameter.Rows[0]["cbmlink"] = "";
                                Boolean ObjStatus = SavePostLoadInquiryDetailsHireTruck(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        DataTable dt_order = new DataTable();
                        if (tload[0].Isfinalorder == "Y")
                        {

                            ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");

                            loadinquiryno = tload[0].load_inquiry_no;
                            string Msg = "";

                            try
                            {

                                DBConnection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                if (!dtPostOrderParameter.Columns.Contains("cbmlink"))
                                {
                                    dtPostOrderParameter.Columns.Add("cbmlink");
                                    dtPostOrderParameter.Rows[0]["cbmlink"] = "";
                                }
                                else
                                    dtPostOrderParameter.Rows[0]["cbmlink"] = "";

                                if (!dtPostOrderParameter.Columns.Contains("trakurl"))
                                {
                                    dtPostOrderParameter.Columns.Add("trakurl");
                                    dtPostOrderParameter.Rows[0]["trakurl"] = "";
                                }
                                else
                                    dtPostOrderParameter.Rows[0]["trakurl"] = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tload[0].load_inquiry_no);

                                Boolean ObjStatus = SavePostLoadInquiryDetailsHireTruck(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref Msg);

                                if (ObjStatus)
                                {

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #region Create Order Entry

                            string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(loadinquiryno);
                            string cbmlink = "";

                            DS_orders ds_orders = new DS_orders();
                            DataTable dt_ordersByinq = GetOrders(loadinquiryno);

                            if (dt_ordersByinq == null)
                            {
                                ds_orders.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_orders.orders[0].order_id = OrdId;
                            }
                            else
                            {
                                ds_orders.EnforceConstraints = false;
                                ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_orders.orders[0].order_id = dt_ordersByinq.Rows[0]["order_id"].ToString();
                            }

                            ds_orders.orders[0].shipper_id = tload[0].shipper_id;
                            ds_orders.orders[0].load_inquiry_no = loadinquiryno;
                            ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
                            ds_orders.orders[0].created_date = System.DateTime.UtcNow;
                            ds_orders.orders[0].active_flag = Constant.Flag_Yes;
                            ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
                            ds_orders.orders[0].isassign_mover = Constant.Flag_No;
                            ds_orders.orders[0].IsCancel = Constant.Flag_No;
                            ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                            ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                            ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                            ds_orders.orders[0].TimeToTravelInMinute = dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString();
                            ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                            ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                            ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                            ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                            if (rate_type_flag == "P")
                                ds_orders.orders[0].IncludePackingCharge = "N";
                            else
                            {
                                ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                                ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                            }

                            ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                            ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                            ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                            ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].order_type_flag = OrderTypeCode;
                            ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                            ds_orders.orders[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                            ds_orders.orders[0].trackurl = trakurl;
                            ds_orders.orders[0].cbmlink = cbmlink;
                            ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                            // ds_orders.orders[0].Discount =  Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());;
                            ds_orders.orders[0].coupon_code = tload[0].promocode;
                            ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].payment_status = Constant.FLAG_N;


                            dt_order = ds_orders.orders;
                            try
                            {
                                ds_orders.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }
                            #endregion

                            #region Generate Initial Quotation
                            DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
                            dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(dt_order);
                            string quotid = "";
                            if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    ServerLog.Log(message);
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
                                objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                            #endregion

                            #region Insertbilling address
                            if (tload[0].Isupdatebillingadd != null)
                            {
                                if (tload[0].Isupdatebillingadd == "Y")
                                {
                                    try
                                    {
                                        DS_Shipper dS_shipper = new DS_Shipper();
                                        Msg = ""; string sr_no = "";
                                        DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

                                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
                                        {
                                            ServerLog.Log(message);
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return BLGeneralUtil.return_ajax_string("0", message);
                                        }
                                        row.billing_srno = sr_no;
                                        row.billing_name = tload[0].billing_name;
                                        row.shipper_id = tload[0].shipper_id;
                                        row.billing_add = tload[0].billing_add;
                                        row.active_flag = Constant.FLAG_Y;
                                        row.source_full_add = tload[0].source_full_add;
                                        row.destination_full_add = tload[0].destination_full_add;
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
                                            return BLGeneralUtil.return_ajax_string("0", Message);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        Message = ex.Message;
                                        return BLGeneralUtil.return_ajax_string("0", Message);
                                    }
                                }

                            }
                            #endregion


                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;

                            dt_order.Columns.Add("TimeForLoadingInMinute");
                            dt_order.Rows[0]["TimeForLoadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"].ToString();
                            dt_order.Columns.Add("TimeForUnloadingInMinute");
                            dt_order.Rows[0]["TimeForUnloadingInMinute"] = dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"].ToString();
                            dt_order.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                            dt_order.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                            dt_order.Columns.Add("TruckTypeDesc");
                            dt_order.Rows[0]["TruckTypeDesc"] = dtSizeTypeMst.Rows[0]["SizeTypeDesc"].ToString();




                            if (tload[0].payment_mode == Constant.PaymentModeCash)
                            {
                                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                                string shippername = GetUserdetailsByID(tload[0].shipper_id);

                                try
                                {
                                    Msg = " Thank you! Your order no. " + tload[0].load_inquiry_no + " from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " to " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been confirmed. Your booking is scheduled for " + OrderShippingDatetime.ToString("dd-MM-yyyy HH:mm:ss tt");

                                    //Msg = " Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + tload[0].load_inquiry_no;
                                    string strmsg = new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));

                                    string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

                                    ServerLog.Log(GetEmailByID(tload[0].shipper_id));
                                    //string shippername = GetUserdetailsByID(tload[0].shipper_id);
                                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderConfirmationMailHireTruck(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["Hiretruck_TotalFuelRate"].ToString()));
                                    if (result["status"].ToString() == "0")
                                    {
                                        ServerLog.Log("Error Sending Activation Email");
                                        // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                                    }

                                    try
                                    {
                                        Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt_order);
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                    }

                                    try
                                    {

                                        //DBConnection.Open();
                                        //DBCommand.Transaction = DBConnection.BeginTransaction();

                                        //String msg = ""; Byte status = 0;
                                        //status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
                                        //if (status == Constant.Status_Fail)
                                        //{
                                        //    ServerLog.Log("Error in save notification Data ");
                                        //    DBCommand.Transaction.Rollback();
                                        //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        //    return BLGeneralUtil.return_ajax_string("0", msg);
                                        //}

                                        //DBCommand.Transaction.Commit();
                                        //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    ServerLog.Log("Error in send OTP on Completation ");
                                }
                            }

                            return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(dt_order));
                        }
                        else
                        {
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public Boolean SavePostLoadInquiryDetailsHireTruck(ref IDbCommand command, ref DataTable dtPostOrderParameter, List<load_post_enquiry> JobjOrder, ref DataTable dtSizeTypeMst, ref String Message)
        {
            try
            {
                if (dtPostOrderParameter != null)
                {
                    Document objdoc = new Document();
                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    string OrdId = "";
                    DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                    DS_orders dsorder = new DS_orders();
                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                    ds_Post_load_enquiry.EnforceConstraints = false;

                    if (JobjOrder[0].load_inquiry_no == "")
                    {

                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref Message)) // New Driver Notification ID
                        {
                            ServerLog.Log(Message);
                            return false;
                        }

                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
                        dtPostOrderParameter.Rows[0]["load_inquiry_no"] = OrdId;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                        ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = 0;


                        if (dtSizeTypeMst != null)
                        {
                            dtSizeTypeMst.Columns.Add("load_inquiry_no");
                            dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                        }
                    }
                    else
                    {

                        #region apply coupancode


                        decimal DiscountPrice = 0;
                        if (JobjOrder[0].promocode != null)
                        {
                            if (JobjOrder[0].promocode.ToString().Trim() != "")
                            {
                                decimal flatdiscount = 0; decimal PercentageDiscount = 0;
                                Boolean B1 = objTruckerMaster.IsCouponValid(JobjOrder[0].promocode, JobjOrder[0].shipper_id, JobjOrder[0].load_inquiry_no, JobjOrder[0].load_inquiry_no, System.DateTime.UtcNow, JobjOrder[0].order_type_flag, JobjOrder[0].rate_type_flag, dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Message);
                                if (B1)
                                {
                                    decimal Total_cost = Convert.ToDecimal(JobjOrder[0].required_price);
                                    if (flatdiscount != 0)
                                        DiscountPrice = Math.Round(flatdiscount);
                                    else if (PercentageDiscount != 0)
                                        DiscountPrice = Total_cost * (PercentageDiscount / 100);

                                    if (DiscountPrice != 0)
                                    {
                                        dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                                        dtSizeTypeMst.Rows[0]["Discount"] = DiscountPrice;
                                    }

                                    if (JobjOrder[0].Isfinalorder == Constant.FLAG_Y)
                                    {
                                        Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, JobjOrder[0].promocode, JobjOrder[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, JobjOrder[0].created_by, JobjOrder[0].created_host, JobjOrder[0].device_id, JobjOrder[0].device_type, ref Message);
                                        if (B2 != 1)
                                        {
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return false;
                                }
                            }
                        }

                        #endregion

                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(JobjOrder[0].load_inquiry_no);
                        string cbmlink = "";

                        DataTable dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), ref Message);
                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }


                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();

                        //if (dtPostOrderParameter.Rows[0]["payment_mode"].ToString() == Constant.PaymentModeCash)
                        //    ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.FLAG_N;
                        //else
                        //    ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.FLAG_Y;

                        //if (dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString() == "P")
                        //{
                        ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
                        //}

                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost_without_discount = dtPostOrderParameter.Rows[0]["required_price"].ToString() == "" ? 0 : Convert.ToDecimal(dtPostOrderParameter.Rows[0]["required_price"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;
                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = dtPostOrderParameter.Rows[0]["promocode"].ToString();

                        if (dtPost_Load_Inquiry != null)
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].active_flag = dtPost_Load_Inquiry.Rows[0]["active_flag"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = dtPost_Load_Inquiry.Rows[0]["IsDraft"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPost_Load_Inquiry.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPost_Load_Inquiry.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPost_Load_Inquiry.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPost_Load_Inquiry.Rows[0]["device_type"].ToString();
                        }
                        else
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPostOrderParameter.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPostOrderParameter.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPostOrderParameter.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPostOrderParameter.Rows[0]["device_type"].ToString();
                        }

                        if (JobjOrder[0].Isfinalorder == Constant.Flag_Yes)
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.Flag_No;
                        }

                        //ds_Post_load_enquiry.post_load_inquiry[0].BaseRate = dtPostOrderParameter.Rows[0]["BaseRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalTravelingRate = dtPostOrderParameter.Rows[0]["TotalTravelingRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalDriverRate = dtPostOrderParameter.Rows[0]["TotalDriverRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalLabourRate = dtPostOrderParameter.Rows[0]["TotalLabourRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalHandimanRate = dtPostOrderParameter.Rows[0]["TotalHandimanRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].TotalSupervisorRate = dtPostOrderParameter.Rows[0]["TotalSupervisorRate"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                        //ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString();

                        //ds_Post_load_enquiry.post_load_inquiry[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                        //ds_Post_load_enquiry.post_load_inquiry[0].goods_details = dtPostOrderParameter.Rows[0]["goods_details"].ToString();


                    }

                    try
                    {
                        ds_Post_load_enquiry.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        return false;
                    }

                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_Post_load_enquiry.post_load_inquiry, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_Post_load_enquiry.post_load_inquiry.Rows.Count)
                    {
                        Message = "An error occurred while insert data in post_load_inquiry. " + objUpdateTableInfo.ErrorMessage;
                        return false;
                    }
                    else
                    {
                        dtPostOrderParameter = ds_Post_load_enquiry.post_load_inquiry;
                        Message = "post_load_inquiry data inserted successfully.";
                        return true;
                    }
                }
                else
                {
                    Message = " No Data To Save ";
                    return false;

                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        #endregion

        #region Admin Pannel Services

        [HttpPost]
        public string GetOrdersDetailsForDeshboard([FromBody]JObject Jobj)
        {

            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = ""; string sourcecity = "";
            string destinationcity = ""; string owner_id = ""; string username = ""; string order_type_flag = "";
            string RowsPerPage = ""; string PageNo = ""; string Assigndriver = ""; string OrderBy = ""; string SortBy = "";
            objOrder = Jobj["order_deshboard"].ToObject<List<orders>>(); int dtrowcount = 0; DataTable dtfinal = null;
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
                RowsPerPage = objOrder[0].RowsPerPage;
                PageNo = objOrder[0].PageNo;
                Assigndriver = objOrder[0].Assigndriver;
                OrderBy = objOrder[0].OrderBy;
                SortBy = objOrder[0].SortBy;
            }

            int PageFrom = (Convert.ToInt16(RowsPerPage) * (Convert.ToInt16(PageNo) - 1)) + 1;
            int PageTo = Convert.ToInt16(RowsPerPage) * Convert.ToInt16(PageNo);

            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                StringBuilder SQLSelect = new StringBuilder();

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
			                                        select    orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover, orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                              orders.inquiry_source_lat,orders.inquiry_source_lng,orders.inquiry_destionation_lat,orders.inquiry_destionation_lng,
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,orders.order_by,
    		                                        SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc,'id1' OrderKey  
    		                                        from orders    
    		                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
    		                                        join user_mst on user_mst.unique_id =  orders.shipper_id
    		                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
    		                                        Where 1=1 and orders.active_flag  = 'Y' 
    		                                        and orders.shippingdatetime>getdate()  
    		                                        UNION ALL 
    		                                        select     orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover, orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                               orders.inquiry_source_lat,orders.inquiry_source_lng,orders.inquiry_destionation_lat,orders.inquiry_destionation_lng,
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,orders.order_by,
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
                        SQLSelect.Append(") as tblpg where 1=1 order by shippingdatetime " + SortBy);
                    else
                        SQLSelect.Append(") as tblpg where 1=1 order by created_date " + SortBy);
                }

                if (Assigndriver == "" && OrderBy == "" && SortBy != "")
                    SQLSelect.Append("  ) as tblpg order by shippingdatetime asc ");

                //if (OrderBy != "")
                //{
                //    if (OrderBy == "M")
                //        SQLSelect.Append(" ) as tblpg where 1=1 and  SrNo BETWEEN " + PageFrom + @" AND " + PageTo + @"  order by shippingdatetime " + SortBy);
                //    else
                //        SQLSelect.Append(" ) as tblpg where 1=1 and  SrNo BETWEEN " + PageFrom + @" AND " + PageTo + @"  order by created_date " + SortBy);
                //}
                //if (Assigndriver == "" && OrderBy == "" && SortBy != "")
                //    SQLSelect.Append(" ) as tblpg where 1=1 and  SrNo BETWEEN " + PageFrom + @" AND " + PageTo + @"  order by SrNo,OrderKey,shippingdatetime asc");

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dtPostLoadOrders = ds.Tables[0];
                        dtrowcount = dtPostLoadOrders.Rows.Count;

                        //DataTable dtfinalorders = dtPostLoadOrders.Select("SrNo >= " + PageFrom + " AND  SrNo <=" + PageTo).CopyToDataTable();
                        //if (dtfinalorders != null)
                        //{
                        //    dtfinalorders.Columns.Add("TotalCount");
                        //    for (int i = 0; i < dtfinalorders.Rows.Count; i++)
                        //    {
                        //        dtfinalorders.Rows[i]["TotalCount"] = dtrowcount;
                        //    }
                        //    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtfinalorders));
                        //}
                        //else
                        //{
                        //    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                        //}

                    }
                }

                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
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
                DataTable dt_order = GetLoadInquiryById(tord[0].load_inquiry_no);
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
                DataTable dt_loadinq = GetLoadInquiryById(tord[0].load_inquiry_no);
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

                DataTable dt_quote = GetLoadInquiryQuotationById(loadinqid, ownid);
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
                DataTable dt_order = GetLoadInquiryById(tord[0].load_inquiry_no);
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
                DataTable dt_loadinq = GetLoadInquiryById(tord[0].load_inquiry_no);
                if (dt_loadinq == null)
                    return BLGeneralUtil.return_ajax_string("0", " Load Inquiry details Not found");


                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["shipper_id"].ToString();
                    //  trukid = dt_order.Rows[0]["truck_id"].ToString();
                }

                loadinqid = tord[0].load_inquiry_no.ToString();
                driverController objDriver = new driverController();

                DataTable dt_quote = GetLoadInquiryQuotationById(loadinqid, ownid);
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
                DataTable dt_order = GetLoadInquiryById(tord[0].load_inquiry_no);
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
                DataTable dt_loadinq = GetLoadInquiryById(tord[0].load_inquiry_no);
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

                DataTable dt_quote = GetLoadInquiryQuotationById(loadinqid, ownid);
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

                DateTime dubaitime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
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
                DataTable dt_order = GetLoadInquiryById(tord[0].load_inquiry_no);
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
                DataTable dt_loadinq = GetLoadInquiryById(tord[0].load_inquiry_no);
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

                DataTable dt_quote = GetLoadInquiryQuotationById(loadinqid, ownid);
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
			                                        select    orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover,  orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,orders.order_by,
    		                                        SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc,'id1' OrderKey  
    		                                        from orders    
    		                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
    		                                        join user_mst on user_mst.unique_id =  orders.shipper_id
    		                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
    		                                        Where 1=1 and orders.active_flag  = 'Y' 
    		                                        and orders.shippingdatetime>getdate()  
    		                                        UNION ALL 
    		                                        select     orders.order_id, orders.shipper_id, orders.load_inquiry_no, orders.isassign_driver_truck, orders.isassign_mover, orders.inquiry_source_addr, orders.inquiry_source_city,  
                                                              orders.aprox_days, orders.load_inquiry_shipping_date, orders.load_inquiry_shipping_time, orders.required_price, orders.status, orders.active_flag, orders.LR_Issued, orders.trackurl, orders.Remark, orders.IsCancel, orders.receipt_imagepath, 
                                                              orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, 
                                                              orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, orders.Discount, Total_cost_without_discount, rem_amt_to_receive, shippingdatetime, Area, 
                                                              orders.rate_type_flag, orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, orders.destination_full_add, orders.billing_add, orders.cbmlink, 
                                                              orders.created_by, orders.created_date, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, TotalHandimanRate, orders.TotalSupervisorRate, orders.goods_details, orders.goods_weight, 
                                                              orders.goods_weightUOM, orders.payment_due_date, orders.last_outstanding_reminder_send_datetime, orders.invoice_pdf_link, orders.IsDraft, orders.Notes, orders.inquiry_destination_addr,orders.MultiDrop_flag,
                                                                orders.Hiretruck_To_datetime,orders.Hiretruck_NoofDay,orders.Hiretruck_TotalDayRate,orders.Hiretruck_IncludingFuel,orders.Hiretruck_TotalFuelRate,
                                                                orders.Hiretruck_MaxKM,orders.AddSerBaseDiscount,orders.TotalAddServiceDiscount,orders.TotalAddServiceCharge,orders.IncludeAddonService,orders.Total_cost_without_addon,orders.order_by,
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
            DataTable dtorderdetail = GetLoadInquiryById(objOrder[0].load_inquiry_no);
            DataTable dt_ReRqorders = new PostOrderController().GetRescheduleRequestDetailsByInq(objOrder[0].load_inquiry_no);

            if (dtorderdetail != null)
            {
                try
                {
                    DBConnection.Open();
                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();
                    message = "";

                    string shipperEmail = GetEmailByID(dtorderdetail.Rows[0]["shipper_id"].ToString());
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
                Document objdoc = new Document();

                // get order details by load inquiry number from order table
                DataTable dt_order = GetLoadInquiryById(Jobj["load_inquiry_no"].ToString());
                if (dt_order == null)
                    return BLGeneralUtil.return_ajax_string("0", " Order details Not found ");

                DBConnection.Open();
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";

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

        #region Driver Services

        [HttpGet]
        public string GetParameterDetails(string paramvalue)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT * from ParameterMst where Parameter=@paramvalue  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("paramvalue", DbType.String, paramvalue));
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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        [HttpGet]
        public string GetParameterDetails(string paramvalue, string driver_id)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @" SELECT     Parameter, Code, Value,CONVERT(VARCHAR,GETUTCDATE(),120) as Utcdatetime,
                          (SELECT     TOP (1) CONVERT(VARCHAR,log_date,120) FROM truck_current_position WHERE (driver_id = '" + driver_id + @"') ORDER BY created_date DESC) AS created_date
                            FROM         ParameterMst WHERE     (Parameter = @paramvalue ) ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("paramvalue", DbType.String, paramvalue));
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
                return BLGeneralUtil.return_ajax_string("0", "No order found");
        }

        #endregion

        #region Dubizzle Order Details

        #region Move My Home

        [HttpPost]
        public String Save_Dubi_MovingHomeDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<Dubi_load_post_enquiry> tload = new List<Dubi_load_post_enquiry>();
            string OrdId = ""; string loadinquiryno = "";
            DataTable dtPostOrderParameter = new DataTable(); DataTable dt_load = new DataTable();
            TimeSpan Tsshippingtime; DataTable dtbillingadd = new DataTable();

            try
            {

                if (objPostOrder["PostOrderParameter"] != null)
                {
                    tload = objPostOrder["PostOrderParameter"].ToObject<List<Dubi_load_post_enquiry>>();
                    //tload[0].load_inquiry_shipping_date = DateTime.Parse(tload[0].load_inquiry_shipping_date.Trim()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(tload);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);

                    if (tload[0].load_inquiry_no == "")
                        ServerLog.Log("Save_Dubi_MovingHomeDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {
                    if (tload[0].order_type_flag == Constant.ORDERTYPECODEFORHOME && tload[0].rate_type_flag != Constant.FLAG_N)
                    {
                        //DateTime shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).ToShortDateString() + " " + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        //DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);

                        //DateTime before24hr = dubaiTime.AddHours(24);

                        //if (shippingdatetime < before24hr)
                        //{
                        //    return BLGeneralUtil.return_ajax_string("0", " Please choose another date ");
                        //}
                    }

                    #region validate Json

                    DateTime OrderDate = DateTime.Today;
                    String Message = String.Empty;
                    String SizeTypeCode = String.Empty;
                    String OrderTypeCode = String.Empty;
                    String TruckTypeCode = String.Empty;

                    if (dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString() != "")
                        Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());
                    else
                        return BLGeneralUtil.return_ajax_string("0", " Time to travel not found");

                    if (dtPostOrderParameter.Columns.Contains("order_type_flag"))
                        OrderTypeCode = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();

                    if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                        if (dtPostOrderParameter.Columns.Contains("SizeTypeCode"))
                            SizeTypeCode = dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString();

                    if (SizeTypeCode == "")
                        return BLGeneralUtil.return_ajax_string("0", " Please provide SizeTypeDetails ");

                    String rate_type_flag = "";
                    if (dtPostOrderParameter.Columns.Contains("rate_type_flag"))
                        rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();

                    String goods_type_flag = "H";
                    DateTime OrderShippingDatetime = new DateTime();
                    if (tload[0].load_inquiry_shipping_date != "")
                        OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                    else
                    {
                        OrderShippingDatetime = System.DateTime.UtcNow.AddDays(2);
                        dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"] = OrderShippingDatetime;
                        dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
                    }
                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    //No Of Truck Edited by User
                    int? NoOfTruck = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfTruck"))
                        if (dtPostOrderParameter.Rows[0]["NoOfTruck"].ToString().Trim() != "")
                            NoOfTruck = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfTruck"]);

                    //No Of Driver Edited by User
                    int? NoOfDriver = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfDriver"))
                        if (dtPostOrderParameter.Rows[0]["NoOfDriver"].ToString().Trim() != "")
                            NoOfDriver = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfDriver"]);

                    //No Of Labour Edited by User
                    int? NoOfLabour = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfLabour"))
                        if (dtPostOrderParameter.Rows[0]["NoOfLabour"].ToString().Trim() != "")
                        {
                            NoOfLabour = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfLabour"]);
                            if (NoOfLabour.Value > NoOfTruck.Value)
                                NoOfLabour = NoOfLabour.Value - NoOfTruck.Value;
                        }

                    //No Of Handiman Edited by User
                    int? NoOfHandiman = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfHandiman"))
                        if (dtPostOrderParameter.Rows[0]["NoOfHandiman"].ToString().Trim() != "")
                            NoOfHandiman = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfHandiman"]);


                    int? NoOfSupervisor = null;
                    if (dtPostOrderParameter.Columns.Contains("NoOfSupervisor"))
                        if (dtPostOrderParameter.Rows[0]["NoOfSupervisor"].ToString().Trim() != "")
                            NoOfSupervisor = Convert.ToInt32(dtPostOrderParameter.Rows[0]["NoOfSupervisor"]);


                    String IncludePackingCharge = "N";
                    if (dtPostOrderParameter.Columns.Contains("IncludePackingCharge"))
                        IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();

                    if (NoOfTruck != NoOfDriver)
                        NoOfDriver = NoOfTruck;

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_NOW)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Truck Type");

                    if (OrderTypeCode == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                        if (TruckTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Truck Type");

                    if (OrderTypeCode == Constant.ORDERTYPECODEFORHOME)
                        if (SizeTypeCode.Trim() == "")
                            return BLGeneralUtil.return_ajax_string("0", " Please select Size Type");

                    #endregion

                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == String.Empty)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {

                        TruckerMaster objTruckerMaster = new TruckerMaster();
                        DataTable dtSizeTypeMst = new DataTable();

                        if (rate_type_flag.ToString().ToUpper() != "N")
                        {
                            dtSizeTypeMst = objTruckerMaster.CalculateRateHome_dubizzle(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }
                        }
                        else
                        {
                            #region Calculate Rate


                            DataTable dtSizeTypeMstBudget = objTruckerMaster.CalculateRateHome_dubizzle(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstBudget == null || dtSizeTypeMstBudget.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }

                            dtSizeTypeMst = objTruckerMaster.CalculateRateHome_dubizzle(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, "H", "P", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }

                            DataTable dtSizeTypeMstSuperSaver = objTruckerMaster.CalculateRateHome_dubizzle(dtPostOrderParameter, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "S", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstSuperSaver == null || dtSizeTypeMstSuperSaver.Rows.Count <= 0)
                            {
                                ServerLog.Log("Error in SaveMovingHoemDetails(" + JsonConvert.SerializeObject(objPostOrder) + ")");
                                return BLGeneralUtil.return_ajax_string("0", Message);
                            }


                            DataRow dr_temp_sizetypeBudget = dtSizeTypeMst.NewRow();
                            dr_temp_sizetypeBudget.ItemArray = dtSizeTypeMstBudget.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeBudget);

                            DataRow dr_temp_sizetypeSuperSaver = dtSizeTypeMst.NewRow();
                            dr_temp_sizetypeSuperSaver.ItemArray = dtSizeTypeMstSuperSaver.Rows[0].ItemArray;
                            dtSizeTypeMst.Rows.Add(dr_temp_sizetypeSuperSaver);

                            #endregion
                        }

                        dtPostOrderParameter.Columns.Add("TotalPackingCharge", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalPackingCharge"] = dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("BaseRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["BaseRate"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalTravelingRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalTravelingRate"] = dtSizeTypeMst.Rows[0]["TotalTravelingRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalDriverRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalDriverRate"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalLabourRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalLabourRate"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalHandimanRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalHandimanRate"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                        dtPostOrderParameter.Columns.Add("TotalSupervisorRate", typeof(String));
                        dtPostOrderParameter.Rows[0]["TotalSupervisorRate"] = dtSizeTypeMst.Rows[0]["TotalSupervisorRate"].ToString();
                        dtPostOrderParameter.Rows[0]["AddSerBaseDiscount"] = dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString();
                        dtPostOrderParameter.Rows[0]["Total_cost_without_discount"] = dtSizeTypeMst.Rows[0]["Total_cost_without_discount"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_PT_Charge", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_PT_Charge"] = dtSizeTypeMst.Rows[0]["Total_PT_Charge"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_PT_Discount", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_PT_Discount"] = dtSizeTypeMst.Rows[0]["Total_PT_Discount"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_CL_Charge", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_CL_Charge"] = dtSizeTypeMst.Rows[0]["Total_CL_Charge"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_CL_Discount", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_CL_Discount"] = dtSizeTypeMst.Rows[0]["Total_CL_Discount"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_PEST_Charge", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_PEST_Charge"] = dtSizeTypeMst.Rows[0]["Total_PEST_Charge"].ToString();
                        //dtPostOrderParameter.Columns.Add("Total_PEST_Discount", typeof(String));
                        //dtPostOrderParameter.Rows[0]["Total_PEST_Discount"] = dtSizeTypeMst.Rows[0]["Total_PEST_Discount"].ToString();
                        //dtPostOrderParameter.Rows[0]["TotalAddServiceDiscount"] = dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"].ToString();
                        //dtPostOrderParameter.Rows[0]["TotalAddServiceCharge"] = dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"].ToString();
                        //dtPostOrderParameter.Rows[0]["Total_cost_without_addon"] = dtSizeTypeMst.Rows[0]["Total_cost_without_addon"].ToString();
                        dtSizeTypeMst.Columns.Add("IncludeAddonService", typeof(String));
                        dtSizeTypeMst.Rows[0]["IncludeAddonService"] = dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString();
                        dtSizeTypeMst.Columns.Add("AddonServices", typeof(String));
                        dtSizeTypeMst.Rows[0]["AddonServices"] = dtPostOrderParameter.Rows[0]["AddonServices"].ToString();

                        DataTable dtpostload = new DataTable();
                        DataTable DtUserdtl = new DataTable();
                        // here requested for first time so is update flag is N
                        if (tload[0].shipper_id.Trim() == "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no == "" && tload[0].rate_type_flag == "N")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {
                                if (DBConnection.State == ConnectionState.Closed)
                                {
                                    DBConnection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();
                                }

                                if (tload[0].shipper_id == "")
                                {
                                    Boolean ObjUsrStatus = SaveUserDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref DtUserdtl, ref Msg);
                                    if (ObjUsrStatus)
                                    {
                                        tload[0].shipper_id = DtUserdtl.Rows[0]["unique_id"].ToString();
                                        dtPostOrderParameter.Rows[0]["shipper_id"] = DtUserdtl.Rows[0]["unique_id"].ToString();
                                    }
                                }

                                Boolean ObjStatus = Save_Dubi_PostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref dtpostload, ref Msg);
                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());

                                    DataTable dtParameters = new DataTable();
                                    dtParameters.Columns.AddRange(new DataColumn[]{ new DataColumn("EmailID"),
                                                                                    new DataColumn("Message"),
                                                                                    new DataColumn("Source"),
                                                                                    new DataColumn("mobile_no"),
                                                                                    new DataColumn("name"),
                                                                                    new DataColumn("subject"),
                                                                                    new DataColumn("Sizetypecode"),
                                                                                    new DataColumn("Sourceaddress"),
                                                                                    new DataColumn("Destinationaddress"),
                                                                                    new DataColumn("ShippingDatetime"),
                                                                                    new DataColumn("load_inquiry_no"),
                                                                                    new DataColumn("TotalDistance"),
                                                                                    new DataColumn("TotalDistanceUOM")

                                                                                    });

                                    DataRow drtoins = dtParameters.NewRow();
                                    drtoins["EmailID"] = dtPostOrderParameter.Rows[0]["email_id"].ToString();
                                    drtoins["Message"] = "";
                                    drtoins["Source"] = "QUOTES";
                                    drtoins["mobile_no"] = dtPostOrderParameter.Rows[0]["UserMobileNo"].ToString();
                                    drtoins["name"] = dtPostOrderParameter.Rows[0]["UserName"].ToString();
                                    drtoins["subject"] = dtSizeTypeMst.Rows[0]["SizeTypeDesc"].ToString();
                                    drtoins["Sizetypecode"] = dtSizeTypeMst.Rows[0]["SizeTypeDesc"].ToString();
                                    drtoins["Sourceaddress"] = dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString();
                                    drtoins["Destinationaddress"] = dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString();
                                    drtoins["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                    drtoins["TotalDistance"] = dtPostOrderParameter.Rows[0]["TotalDistance"].ToString();
                                    drtoins["TotalDistanceUOM"] = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                                    dtParameters.Rows.Add(drtoins);

                                    DataTable dtReqQuoteMail = new DataTable();
                                    if (dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString() != "")
                                        dtReqQuoteMail = new MailerController().GetRequestQuotationbyID(dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                                    else
                                        dtReqQuoteMail = null;

                                    if (dtParameters != null)
                                    {
                                        DBConnection.Open();
                                        DBCommand.Transaction = DBConnection.BeginTransaction();

                                        Boolean blStatus = new MailerController().SendQuotationRequestMailandSaveData(ref DBCommand, ref dtReqQuoteMail, ref dtParameters, ref Message);
                                        if (blStatus)
                                        {
                                            DBCommand.Transaction.Commit();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        }
                                        else
                                        {
                                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            return BLGeneralUtil.return_ajax_string("0", Message);
                                        }
                                    }
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                                if (dtSizeTypeMst.Rows.Count > 1)
                                {
                                    if (!dtSizeTypeMst.Columns.Contains("shipper_id"))
                                        dtSizeTypeMst.Columns.Add("shipper_id");

                                    for (int i = 0; i < dtSizeTypeMst.Rows.Count; i++)
                                    {
                                        dtSizeTypeMst.Rows[i]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                        dtSizeTypeMst.Rows[i]["shipper_id"] = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                                    }
                                }

                                ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID_1 : " + dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }


                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag == "N")
                        {
                            #region Create Post Load Inquiry

                            try
                            {
                                if (dtSizeTypeMst.Rows.Count > 1)
                                {
                                    if (!dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                                        dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                    if (!dtSizeTypeMst.Columns.Contains("shipper_id"))
                                        dtSizeTypeMst.Columns.Add("shipper_id");

                                    for (int i = 0; i < dtSizeTypeMst.Rows.Count; i++)
                                    {
                                        dtSizeTypeMst.Rows[i]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                        dtSizeTypeMst.Rows[i]["shipper_id"] = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion
                        }

                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag == "N")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {
                                if (DBConnection.State == ConnectionState.Closed)
                                {
                                    DBConnection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();
                                }

                                Boolean ObjUsrStatus = SaveUserDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref DtUserdtl, ref Msg);
                                if (ObjUsrStatus)
                                {
                                    tload[0].shipper_id = DtUserdtl.Rows[0]["unique_id"].ToString();
                                    dtPostOrderParameter.Rows[0]["shipper_id"] = DtUserdtl.Rows[0]["unique_id"].ToString();
                                }

                                Boolean ObjStatus = Save_Dubi_PostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref dtpostload, ref Msg);
                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                    if (dtSizeTypeMst.Rows.Count > 1)
                                    {
                                        if (!dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                                            dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                        if (!dtSizeTypeMst.Columns.Contains("shipper_id"))
                                            dtSizeTypeMst.Columns.Add("shipper_id");

                                        for (int i = 0; i < dtSizeTypeMst.Rows.Count; i++)
                                        {
                                            dtSizeTypeMst.Rows[i]["load_inquiry_no"] = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                            dtSizeTypeMst.Rows[i]["shipper_id"] = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                                        }
                                    }

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion

                            ServerLog.SuccessLog(" Load Inquiry Saved With Quotation Inquire ID_3 : " + dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                        }
                        if (tload[0].shipper_id.Trim() != "" && tload[0].Isupdate == "Y" && tload[0].Isfinalorder == "N" && tload[0].load_inquiry_no != "" && tload[0].rate_type_flag != "N")
                        {
                            #region Create Post Load Inquiry

                            string Msg = "";

                            try
                            {
                                if (DBConnection.State == ConnectionState.Closed)
                                {
                                    DBConnection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();
                                }

                                Boolean ObjStatus = Save_Dubi_PostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref dtpostload, ref Msg);
                                if (ObjStatus)
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                    if (!dtSizeTypeMst.Columns.Contains("shipper_id"))
                                        dtSizeTypeMst.Columns.Add("shipper_id");
                                    if (!dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                                        dtSizeTypeMst.Columns.Add("load_inquiry_no");

                                    dtSizeTypeMst.Rows[0]["load_inquiry_no"] = tload[0].load_inquiry_no;
                                    dtSizeTypeMst.Rows[0]["shipper_id"] = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();

                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion

                            ServerLog.SuccessLog(" Load Inquiry Saved With Quotation Inquire ID_4 : " + dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString());
                        }

                        DataTable dt_order = new DataTable();
                        if (tload[0].Isfinalorder == "Y")
                        {
                            string Msg = "";

                            string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tload[0].load_inquiry_no);
                            string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(tload[0].load_inquiry_no);

                            DBConnection.Open();
                            DBCommand.Transaction = DBConnection.BeginTransaction();

                            #region Create PostLoad Entry

                            try
                            {
                                Boolean ObjUsrStatus = SaveUserDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref DtUserdtl, ref Msg);
                                if (ObjUsrStatus)
                                {
                                    tload[0].shipper_id = DtUserdtl.Rows[0]["unique_id"].ToString();
                                    dtPostOrderParameter.Rows[0]["shipper_id"] = DtUserdtl.Rows[0]["unique_id"].ToString();
                                }

                                Boolean ObjStatus = Save_Dubi_PostLoadInquiryDetails(ref DBCommand, ref dtPostOrderParameter, tload, ref dtSizeTypeMst, ref dtpostload, ref Msg);

                                if (ObjStatus)
                                {
                                    loadinquiryno = dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString();
                                }
                                else
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", Msg);
                                }

                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            #endregion

                            #region used when take cash payment

                            #region Create Order Entry

                            DS_orders ds_orders = new DS_orders();

                            DataTable dt_ordersByinq = GetOrders(loadinquiryno);

                            if (dt_ordersByinq == null)
                            {
                                ds_orders.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                ds_orders.orders.ImportRow(dtPostOrderParameter.Rows[0]);
                                ds_orders.orders[0].order_id = OrdId;
                            }
                            else
                                ds_orders.orders.ImportRow(dt_ordersByinq.Rows[0]);

                            ds_orders.orders[0].shipper_id = tload[0].shipper_id;
                            ds_orders.orders[0].load_inquiry_no = loadinquiryno;
                            ds_orders.orders[0].shippingdatetime = OrderShippingDatetime;
                            ds_orders.orders[0].created_date = System.DateTime.UtcNow;
                            ds_orders.orders[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper 
                            ds_orders.orders[0].isassign_driver_truck = Constant.Flag_No;
                            ds_orders.orders[0].isassign_mover = Constant.Flag_No;
                            ds_orders.orders[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                            ds_orders.orders[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                            ds_orders.orders[0].TotalDistanceUOM = tload[0].TotalDistanceUOM;
                            ds_orders.orders[0].TimeToTravelInMinute = tload[0].TimeToTravelInMinute;//dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"].ToString(); 
                            ds_orders.orders[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                            ds_orders.orders[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                            ds_orders.orders[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                            ds_orders.orders[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();

                            if (rate_type_flag == "P")
                                ds_orders.orders[0].IncludePackingCharge = "N";
                            else
                            {
                                ds_orders.orders[0].IncludePackingCharge = dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString();
                                ds_orders.orders[0].TotalPackingCharge = dtPostOrderParameter.Rows[0]["TotalPackingCharge"].ToString();
                            }

                            ds_orders.orders[0].billing_add = dtPostOrderParameter.Rows[0]["billing_add"].ToString();
                            ds_orders.orders[0].billing_name = dtPostOrderParameter.Rows[0]["billing_name"].ToString();
                            ds_orders.orders[0].source_full_add = dtPostOrderParameter.Rows[0]["source_full_add"].ToString();
                            ds_orders.orders[0].destination_full_add = dtPostOrderParameter.Rows[0]["destination_full_add"].ToString();
                            ds_orders.orders[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].order_type_flag = OrderTypeCode;
                            ds_orders.orders[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                            ds_orders.orders[0].goods_type_flag = goods_type_flag;
                            ds_orders.orders[0].trackurl = trakurl;
                            ds_orders.orders[0].cbmlink = cbmlink;
                            ds_orders.orders[0].Total_cost_without_discount = Convert.ToDecimal(tload[0].required_price);
                            //ds_orders.orders[0].Discount = DiscountPrice;
                            ds_orders.orders[0].coupon_code = tload[0].promocode;
                            ds_orders.orders[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                            ds_orders.orders[0].order_by = "DUBI";

                            if (tload[0].payment_mode == Constant.PaymentModeONLINE)
                            {
                                ds_orders.orders[0].payment_mode = Constant.PaymentModeONLINE;
                                ds_orders.orders[0].payment_status = Constant.FLAG_N;
                                ds_orders.orders[0].active_flag = Constant.Flag_No;
                                ds_orders.orders[0].IsCancel = Constant.Flag_No;
                                ds_orders.orders[0].IsDraft = Constant.Flag_No;
                            }
                            else
                            {
                                ds_orders.orders[0].payment_mode = tload[0].payment_mode;
                                ds_orders.orders[0].payment_status = Constant.FLAG_N;
                                ds_orders.orders[0].active_flag = Constant.Flag_Yes;
                                ds_orders.orders[0].IsCancel = Constant.Flag_No;
                                ds_orders.orders[0].IsDraft = Constant.Flag_No;
                            }

                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                ds_orders.orders[0].IncludeAddonService = Constant.Flag_Yes;
                            else
                                ds_orders.orders[0].IncludeAddonService = Constant.Flag_No;

                            dt_order = ds_orders.orders;

                            try
                            {
                                ds_orders.EnforceConstraints = true;
                            }
                            catch (ConstraintException ce)
                            {
                                ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ce.Message);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }

                            objBLReturnObject = master.UpdateTables(ds_orders.orders, ref DBCommand);
                            if (objBLReturnObject.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLReturnObject.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                            }

                            #endregion

                            #region Generate Initial Quotation
                            DS_load_order_quotation dS_load_order_quotation = new DS_load_order_quotation();
                            dS_load_order_quotation = (DS_load_order_quotation)Generate_load_enquiry_quotations(dt_order);
                            string quotid = "";
                            if (dS_load_order_quotation.load_order_enquiry_quotation != null && dS_load_order_quotation.load_order_enquiry_quotation.Rows.Count > 0)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "QT", "", "", ref quotid, ref message))
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    ServerLog.Log(message);
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["quot_id"] = quotid;
                                dS_load_order_quotation.load_order_enquiry_quotation[0]["owner_id"] = tload[0].shipper_id;
                                objBLReturnObject = master.UpdateTables(dS_load_order_quotation.load_order_enquiry_quotation, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus != 1)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                            }
                            #endregion

                            #region Create CBM Entry

                            DS_CBM objDsCbm = new DS_CBM();
                            Master objmaster = new Master(); string DocNtficID = "";

                            DataTable dtappid = new CBMController().GetAppIDbySizetype(dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString());

                            if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QHD", "", "", ref DocNtficID, ref message)) // New Driver Notification ID
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            }

                            objDsCbm.EnforceConstraints = false;

                            DS_CBM.quote_hdrRow tr = objDsCbm.quote_hdr.Newquote_hdrRow();
                            tr.quote_id = loadinquiryno;
                            tr.quote_hdr_id = DocNtficID;
                            tr.appartment_id = dtappid.Rows[0]["appartment_id"].ToString();
                            tr.room_type = dtappid.Rows[0]["appartment_desc"].ToString();
                            tr.customer_name = GetUserdetailsByID(tload[0].shipper_id);
                            tr.customer_mobile = GetMobileNoByID(tload[0].shipper_id);
                            tr.customer_email = GetEmailByID(tload[0].shipper_id);
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

                            #endregion

                            DBCommand.Transaction.Commit();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 1;
                            ServerLog.SuccessLog("Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

                            #region Mail


                            if (tload[0].payment_mode == Constant.PaymentModeCash)
                            {
                                DateTime dubaiTime = DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                                string shippername = GetUserdetailsByID(tload[0].shipper_id);

                                try
                                {
                                    //Msg = " Thank you! Your order no. " + tload[0].load_inquiry_no + " from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " to " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been confirmed. Your booking is scheduled for " + OrderShippingDatetime.ToString("dd-MM-yyyy HH:mm:ss tt");

                                    ////string Msg = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;
                                    //new EMail().SendOtpToUserMobileNoUAE(Msg, GetMobileNoByID(tload[0].shipper_id));
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                    ServerLog.Log("Error in send OTP on Completation ");
                                }

                                string MsgMailbody = "Thank you..Your order from  " + dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString() + " To " + dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + loadinquiryno;

                                ServerLog.Log(GetEmailByID(tload[0].shipper_id));

                                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(GetEmailByID(tload[0].shipper_id), " Your Order is confirmed (Order ID: " + loadinquiryno + ")", shippername, MsgMailbody, loadinquiryno, dt_order, dtSizeTypeMst.Rows[0]["TotalPackingRate"].ToString()));
                                if (result["status"].ToString() == "0")
                                {
                                    ServerLog.Log("Error Sending Activation Email");
                                    // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                                }

                                try
                                {
                                    // Boolean Blresult = new MailerController().OrderConfirmationMailToAdmin(dt_order);
                                }
                                catch (Exception ex)
                                {
                                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    //eturn BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }

                                //try
                                //{

                                //    DBConnection.Open();
                                //    DBCommand.Transaction = DBConnection.BeginTransaction();

                                //    String msg = ""; Byte status = 0;
                                //    status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tload[0].shipper_id, "ADMIN", Constant.MessageType_PostOrder, "Your Order is confirmed (Order ID: " + loadinquiryno + " ) ", Constant.MessageType_AssignDriverTruck, loadinquiryno, tload[0].shipper_id, loadinquiryno, "", "", tload[0].shipper_id, ref msg);
                                //    if (status == Constant.Status_Fail)
                                //    {
                                //        ServerLog.Log("Error in save notification Data ");
                                //        DBCommand.Transaction.Rollback();
                                //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                //        //return BLGeneralUtil.return_ajax_string("0", msg);
                                //    }

                                //    DBCommand.Transaction.Commit();
                                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                //}
                                //catch (Exception ex)
                                //{
                                //    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                //    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                //}
                            }

                            #endregion

                            // return "{"status":"1","message":"Order Generated Successfully","data":" + SendReceiveJSon.GetJson(ds_orders.orders) + "}";
                            return BLGeneralUtil.return_ajax_statusdata("1", "Order Generated Successfully", SendReceiveJSon.GetJson(dtpostload));
                        }
                        else
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtSizeTypeMst));
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public Boolean Save_Dubi_PostLoadInquiryDetails(ref IDbCommand command, ref DataTable dtPostOrderParameter, List<Dubi_load_post_enquiry> JobjOrder, ref DataTable dtSizeTypeMst, ref DataTable dtpostload, ref String Message)
        {
            try
            {
                if (dtPostOrderParameter != null)
                {
                    Document objdoc = new Document();
                    TruckerMaster objTruckerMaster = new TruckerMaster();
                    string OrdId = ""; DateTime OrderShippingDatetime = new DateTime();
                    if (dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString() != "")
                        OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrderParameter.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                    DS_Post_load_enquiry ds_Post_load_enquiry = new DS_Post_load_enquiry();
                    ds_Post_load_enquiry.EnforceConstraints = false;


                    if (dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString() == "")
                    {
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref OrdId, ref Message)) // New Driver Notification ID
                        {
                            ServerLog.Log(Message);
                            return false;
                        }

                        if (dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"].ToString() == "")
                        {
                            dtPostOrderParameter.Rows[0]["load_inquiry_shipping_date"] = System.DateTime.UtcNow;
                            OrderShippingDatetime = System.DateTime.UtcNow;
                        }

                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_no = OrdId;
                        dtPostOrderParameter.Rows[0]["load_inquiry_no"] = OrdId;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].active_flag = Constant.Flag_Yes;
                        ds_Post_load_enquiry.post_load_inquiry[0].order_by = "DUBI";
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // New Orders // Quote Selected and sent to shipper

                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;

                        ds_Post_load_enquiry.post_load_inquiry[0].order_type_flag = dtPostOrderParameter.Rows[0]["order_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost_without_discount"].ToString());
                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = dtPostOrderParameter.Rows[0]["promocode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = dtPostOrderParameter.Rows[0]["payment_status"].ToString();

                        if (dtSizeTypeMst != null)
                        {
                            if (!dtSizeTypeMst.Columns.Contains("load_inquiry_no"))
                            {
                                dtSizeTypeMst.Columns.Add("load_inquiry_no");
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                            }
                            else
                            {
                                dtSizeTypeMst.Rows[0]["load_inquiry_no"] = OrdId;
                            }

                        }

                    }
                    else
                    {
                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(JobjOrder[0].load_inquiry_no);
                        string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(JobjOrder[0].load_inquiry_no);


                        DataTable dtPost_Load_Inquiry = objTruckerMaster.GetPost_Load_Inquiry(ref DBDataAdpterObject, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), ref Message);
                        if (dtPost_Load_Inquiry == null || dtPost_Load_Inquiry.Rows.Count <= 0)
                        {
                            ServerLog.Log(message);
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return false;
                        }

                        #region Save Coupon Code details


                        decimal DiscountPrice = 0;
                        if (JobjOrder[0].promocode.ToString().Trim() != "")
                        {

                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
                            Boolean B1 = objTruckerMaster.IsCouponValid(JobjOrder[0].promocode, JobjOrder[0].shipper_id, JobjOrder[0].load_inquiry_no, JobjOrder[0].load_inquiry_no, System.DateTime.UtcNow, JobjOrder[0].order_type_flag, JobjOrder[0].rate_type_flag, dtPostOrderParameter.Rows[0]["SizeTypeCode"].ToString(), ref flatdiscount, ref PercentageDiscount, ref Msg);
                            if (B1)
                            {
                                decimal Total_cost = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString());
                                if (flatdiscount != 0)
                                    DiscountPrice = Math.Round(flatdiscount);
                                else if (PercentageDiscount != 0)
                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

                                if (DiscountPrice != 0)
                                {
                                    dtSizeTypeMst.Rows[0]["Total_cost"] = Total_cost - DiscountPrice;
                                    dtSizeTypeMst.Rows[0]["Discount"] = DiscountPrice;
                                }

                                Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, JobjOrder[0].promocode, JobjOrder[0].shipper_id, dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), dtPostOrderParameter.Rows[0]["load_inquiry_no"].ToString(), System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, JobjOrder[0].created_by, JobjOrder[0].created_host, JobjOrder[0].device_id, JobjOrder[0].device_type, ref Msg);
                                if (B2 != 1)
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return false;
                                }
                            }
                            else
                            {
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return false;
                            }
                        }

                        #endregion

                        ds_Post_load_enquiry.post_load_inquiry.ImportRow(dtPostOrderParameter.Rows[0]);
                        ds_Post_load_enquiry.post_load_inquiry[0].load_inquiry_date = System.DateTime.UtcNow;
                        ds_Post_load_enquiry.post_load_inquiry[0].shippingdatetime = OrderShippingDatetime;
                        ds_Post_load_enquiry.post_load_inquiry[0].shipper_id = dtPostOrderParameter.Rows[0]["shipper_id"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].rate_type_flag = dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN; // Generating Quote 
                        ds_Post_load_enquiry.post_load_inquiry[0].SizeTypeCode = dtSizeTypeMst.Rows[0]["SizeTypeCode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistance = dtSizeTypeMst.Rows[0]["TotalDistance"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].TimeToTravelInMinute = dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfTruck = dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfDriver = dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfLabour = dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfHandiman = dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Total_cost = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].order_by = "DUBI";

                        //if (dtPostOrderParameter.Rows[0]["payment_mode"].ToString() == Constant.PaymentModeCash)
                        //    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_N;
                        //else
                        //    ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = Constant.FLAG_Y;

                        if (dtPostOrderParameter.Rows[0]["rate_type_flag"].ToString() == "P")
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].IncludePackingCharge = "N";
                            ds_Post_load_enquiry.post_load_inquiry[0].TotalPackingCharge = string.Empty;
                        }

                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = dtSizeTypeMst.Rows[0]["Discount"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Discount"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].rem_amt_to_receive = dtSizeTypeMst.Rows[0]["Total_cost"].ToString() != "" ? Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Total_cost"].ToString()) : 0;
                        ds_Post_load_enquiry.post_load_inquiry[0].NoOfSupervisor = dtSizeTypeMst.Rows[0]["NoOfSupervisor"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].goods_type_flag = dtPostOrderParameter.Rows[0]["goods_type_flag"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_status = Constant.Flag_No;
                        ds_Post_load_enquiry.post_load_inquiry[0].payment_mode = dtPostOrderParameter.Rows[0]["payment_mode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].cbmlink = cbmlink;
                        ds_Post_load_enquiry.post_load_inquiry[0].coupon_code = dtPostOrderParameter.Rows[0]["promocode"].ToString();
                        ds_Post_load_enquiry.post_load_inquiry[0].Discount = DiscountPrice;

                        if (dtPost_Load_Inquiry != null)
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].IsDraft = dtPost_Load_Inquiry.Rows[0]["IsDraft"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;// Convert.ToDateTime(dtPost_Load_Inquiry.Rows[0]["created_date"].ToString());
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPost_Load_Inquiry.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPost_Load_Inquiry.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPost_Load_Inquiry.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPost_Load_Inquiry.Rows[0]["device_type"].ToString();
                        }
                        else
                        {
                            ds_Post_load_enquiry.post_load_inquiry[0].created_date = System.DateTime.UtcNow;
                            ds_Post_load_enquiry.post_load_inquiry[0].created_by = dtPostOrderParameter.Rows[0]["created_by"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].created_host = dtPostOrderParameter.Rows[0]["created_host"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_id = dtPostOrderParameter.Rows[0]["device_id"].ToString();
                            ds_Post_load_enquiry.post_load_inquiry[0].device_type = dtPostOrderParameter.Rows[0]["device_type"].ToString();
                        }


                    }

                    try
                    {
                        ds_Post_load_enquiry.EnforceConstraints = true;
                        dtpostload = ds_Post_load_enquiry.post_load_inquiry;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.Log(ce.Message + Environment.NewLine + ce.StackTrace);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        return false;
                    }

                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_Post_load_enquiry.post_load_inquiry, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_Post_load_enquiry.post_load_inquiry.Rows.Count)
                    {
                        Message = "An error occurred while insert data in post_load_inquiry. " + objUpdateTableInfo.ErrorMessage;
                        return false;
                    }
                    else
                    {
                        //dtPostOrderParameter = ds_Post_load_enquiry.post_load_inquiry;
                        dtpostload = ds_Post_load_enquiry.post_load_inquiry;
                        Message = "post_load_inquiry data inserted successfully.";
                        return true;
                    }
                }
                else
                {
                    Message = " No Data To Save ";
                    return false;

                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        public Boolean SaveUserDetails(ref IDbCommand command, ref DataTable dtPostOrderParameter, List<Dubi_load_post_enquiry> JobjOrder, ref DataTable DtUserdtl, ref String Message)
        {
            Document objdoc = new Document();
            DS_User ds_user = new DS_User();
            Master master = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DtUserdtl = new ShipperController().GetUser("", JobjOrder[0].UserMobileNo);
            if (DtUserdtl == null)
            {
                string DocUserID = "";

                #region Create User

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "SH", "", "", ref DocUserID, ref message)) // New Driver Notification ID
                {
                    ServerLog.Log(message);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return false;
                }

                ds_user.EnforceConstraints = false;
                ds_user.user_mst.ImportRow(dtPostOrderParameter.Rows[0]);
                ds_user.user_mst.Rows[0]["client_type"] = "B";
                ds_user.user_mst.Rows[0]["user_id"] = JobjOrder[0].UserMobileNo;
                ds_user.user_mst.Rows[0]["role_id"] = "SH";
                ds_user.user_mst.Rows[0]["unique_id"] = DocUserID;
                ds_user.user_mst.Rows[0]["first_name"] = JobjOrder[0].UserName;
                ds_user.user_mst.Rows[0]["last_name"] = "";
                ds_user.user_mst.Rows[0]["email_id"] = JobjOrder[0].email_id;
                ds_user.user_mst.Rows[0]["password"] = Constant.USERDEFAULTPASSWORD;
                ds_user.user_mst.Rows[0]["start_date"] = System.DateTime.UtcNow;
                ds_user.user_mst.Rows[0]["user_loc_flag"] = "L";
                ds_user.user_mst.Rows[0]["pass_expiry_date"] = System.DateTime.UtcNow.AddYears(10);
                ds_user.user_mst.Rows[0]["user_status_flag"] = Constant.Active;
                ds_user.user_mst.Rows[0]["created_date"] = System.DateTime.UtcNow;
                ds_user.user_mst[0].user_type = Constant.USERTYPE;

                ds_user.EnforceConstraints = true;
                objBLReturnObject = master.UpdateTables(ds_user.user_mst, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return false;
                }
                else
                {
                    DtUserdtl = ds_user.user_mst;
                    message = "User details update successfully ";
                    return true;
                }

                #endregion
            }
            else
            {
                #region Update User details

                ds_user.EnforceConstraints = false;
                ds_user.user_mst.ImportRow(DtUserdtl.Rows[0]);
                ds_user.user_mst[0].first_name = JobjOrder[0].UserName;
                ds_user.user_mst[0].last_name = "";
                ds_user.user_mst[0].email_id = JobjOrder[0].email_id;
                ds_user.user_mst[0].user_type = Constant.USERTYPE;

                ds_user.EnforceConstraints = true;
                objBLReturnObject = master.UpdateTables(ds_user.user_mst, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return false;
                }
                else
                {
                    DtUserdtl = ds_user.user_mst;
                    message = "User Details Update successfully ";
                    return true;
                }

                #endregion
            }
        }

        #endregion

        #region Move My goods

        public DataTable GetDubiGoodsOrdersbyId(string TrnId)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM  dubiz_goods_orders_details where Transaction_id=@TrnId  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("TrnId", DbType.String, TrnId));
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
        public DataTable GetDubiItemByitemcode(string item_Code)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = @" select dubiz_goods_item_mst.Item_desc,* from dubiz_goods_itemrate_details
                               join dubiz_goods_item_mst on dubiz_goods_item_mst.item_code=dubiz_goods_itemrate_details.item_code where dubiz_goods_itemrate_details.item_code=@item_Code  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("item_Code", DbType.String, item_Code));
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
        public string GetAllDubizlOrders()
        {
            try
            {
                DataTable dtQuotdetails = new DataTable();
                String query1 = @" select * from dubiz_goods_orders_details ";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtQuotdetails = ds.Tables[0];
                }
                if (dtQuotdetails != null && dtQuotdetails.Rows.Count > 0)
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtQuotdetails)));
                else
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }
        [HttpGet]
        public string GetDubiItemDetails()
        {
            try
            {
                DataTable dtQuotdetails = new DataTable();
                String query1 = @" select distinct dubiz_goods_item_mst.Item_desc,* from dubiz_goods_itemrate_details
                                 join dubiz_goods_item_mst on dubiz_goods_item_mst.item_code=dubiz_goods_itemrate_details.item_code ";
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtQuotdetails = ds.Tables[0];
                }
                if (dtQuotdetails != null && dtQuotdetails.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtQuotdetails)));
                }
                else
                {
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
                }
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        [HttpGet]
        public string GetDubiGoodsOrdersbyTrnID(string TrnId)
        {
            try
            {
                DataTable dtQuotdetails = GetDubiGoodsOrdersbyId(TrnId);

                if (dtQuotdetails != null && dtQuotdetails.Rows.Count > 0)
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtQuotdetails)));
                else
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
            }
            catch (Exception ex)
            {
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        [HttpPost]
        public String Save_dubi_MovingGoodsDetailsOld([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<dubiz_goods_orders_details> torder = new List<dubiz_goods_orders_details>();
            DataTable dtPostOrderParameter = new DataTable();
            TimeSpan TsPickuptime; TimeSpan TsDeliverytime;
            TimeSpan Tsshippingtime;
            DS_Dubi_Orders dsdubiorder = new DS_Dubi_Orders(); string OrdId = "";

            try
            {

                if (objPostOrder["DubiGoodsOrder"] != null)
                {
                    torder = objPostOrder["DubiGoodsOrder"].ToObject<List<dubiz_goods_orders_details>>();
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(torder);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {

                    String Message = String.Empty;
                    String ItemCode = String.Empty;

                    #region Validate data


                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    if (dtPostOrderParameter.Rows[0]["pickup_date"].ToString() == "")
                        return BLGeneralUtil.return_ajax_string("0", " Please provide preferred pickup date ");

                    //if (dtPostOrderParameter.Rows[0]["pickup_time"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred pickup time ");

                    //if (dtPostOrderParameter.Rows[0]["delivery_date"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred delivery date ");

                    //if (dtPostOrderParameter.Rows[0]["delivery_time"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred delivery time ");

                    if (dtPostOrderParameter.Columns.Contains("item_Code"))
                        ItemCode = dtPostOrderParameter.Rows[0]["item_Code"].ToString();
                    else
                        return BLGeneralUtil.return_ajax_string("0", " Item Code not found ");

                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    #endregion

                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == "")
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {
                        TruckerMaster objtrukker = new TruckerMaster();
                        DataTable dtItemDetails = objtrukker.GetDubiGoodsRate(ItemCode, ref Message);
                        if (dtItemDetails != null)
                        {
                            dtPostOrderParameter.Rows[0]["item_code"] = dtItemDetails.Rows[0]["Item_code"].ToString();
                            //   dtPostOrderParameter.Rows[0]["item_desc"] = dtItemDetails.Rows[0]["Item_desc"].ToString();
                            dtPostOrderParameter.Rows[0]["Total_cost"] = dtItemDetails.Rows[0]["Item_rate"].ToString();

                            if (torder[0].Isfinalorder == Constant.FLAG_Y)
                            {
                                DBCommand.Connection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dsdubiorder.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "PDO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }

                                dsdubiorder.dubiz_goods_orders_details.ImportRow(dtPostOrderParameter.Rows[0]);
                                dsdubiorder.dubiz_goods_orders_details[0].Transaction_id = OrdId;
                                dsdubiorder.dubiz_goods_orders_details[0].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                dsdubiorder.dubiz_goods_orders_details[0].Total_cost_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                dsdubiorder.dubiz_goods_orders_details[0].active_flag = Constant.FLAG_Y;
                                dsdubiorder.dubiz_goods_orders_details[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN;
                                dsdubiorder.dubiz_goods_orders_details[0].created_date = System.DateTime.UtcNow;
                                dsdubiorder.dubiz_goods_orders_details[0].created_by = torder[0].created_by;
                                dsdubiorder.dubiz_goods_orders_details[0].device_id = torder[0].device_id;

                                dsdubiorder.EnforceConstraints = true;
                                objBLReturnObject = master.UpdateTables(dsdubiorder.dubiz_goods_orders_details, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                                else
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dsdubiorder.dubiz_goods_orders_details));
                                }
                            }
                            else
                                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostOrderParameter));
                        }
                        else
                            return BLGeneralUtil.return_ajax_string("0", " Date not found ");

                    }
                    return BLGeneralUtil.return_ajax_string("0", " Date not found ");
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpPost]
        public String Save_dubi_MovingGoodsDetails([FromBody]JObject objPostOrder)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            List<dubiz_goods_orders_details> torder = new List<dubiz_goods_orders_details>();
            DataTable dtPostOrderParameter = new DataTable();
            TimeSpan TsPickuptime; TimeSpan TsDeliverytime;
            TimeSpan Tsshippingtime;
            DS_Dubi_Orders dsdubiorder = new DS_Dubi_Orders(); string OrdId = "";

            try
            {

                if (objPostOrder["DubiGoodsOrder"] != null)
                {
                    torder = objPostOrder["DubiGoodsOrder"].ToObject<List<dubiz_goods_orders_details>>();
                    dtPostOrderParameter = BLGeneralUtil.ToDataTable(torder);
                    dtPostOrderParameter = BLGeneralUtil.CheckDateTime(dtPostOrderParameter);
                }

                if (dtPostOrderParameter == null || dtPostOrderParameter.Rows.Count <= 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", " No data available to Calculate Rate. Operation cancelled.");
                }
                else
                {

                    String Message = String.Empty;
                    String ItemCode = String.Empty;

                    #region Validate data


                    Tsshippingtime = TimeSpan.Parse(dtPostOrderParameter.Rows[0]["TimeToTravelInMinute"].ToString());

                    if (dtPostOrderParameter.Rows[0]["pickup_date"].ToString() == "")
                        return BLGeneralUtil.return_ajax_string("0", " Please provide preferred pickup date ");

                    //if (dtPostOrderParameter.Rows[0]["pickup_time"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred pickup time ");

                    //if (dtPostOrderParameter.Rows[0]["delivery_date"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred delivery date ");

                    //if (dtPostOrderParameter.Rows[0]["delivery_time"].ToString() == "")
                    //    return BLGeneralUtil.return_ajax_string("0", " Please provide preferred delivery time ");

                    if (dtPostOrderParameter.Columns.Contains("item_Code"))
                        ItemCode = dtPostOrderParameter.Rows[0]["item_Code"].ToString();
                    else
                        return BLGeneralUtil.return_ajax_string("0", " Item Code not found ");

                    Decimal TotalDistance = -1;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistance"))
                        TotalDistance = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["TotalDistance"]);

                    String TotalDistanceUOM = String.Empty;
                    if (dtPostOrderParameter.Columns.Contains("TotalDistanceUOM"))
                        TotalDistanceUOM = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();

                    Decimal TimeToTravelInMinute = -1;
                    if (dtPostOrderParameter.Columns.Contains("TimeToTravelInMinute"))
                        TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                    #endregion

                    if (TotalDistance < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance. ");
                    }
                    else if (TotalDistanceUOM.Trim() == "")
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied TotalDistance Unit. ");
                    }
                    else if (TimeToTravelInMinute < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Please supplied Time To Travel In Minute. ");
                    }
                    else
                    {
                        TruckerMaster objtrukker = new TruckerMaster();
                        DataTable dtItemDetails = objtrukker.GetDubiGoodsRate(ItemCode, ref Message);
                        if (dtItemDetails != null)
                        {
                            dtPostOrderParameter.Rows[0]["item_code"] = dtItemDetails.Rows[0]["Item_code"].ToString();
                            //   dtPostOrderParameter.Rows[0]["item_desc"] = dtItemDetails.Rows[0]["Item_desc"].ToString();
                            dtPostOrderParameter.Rows[0]["Total_cost"] = dtItemDetails.Rows[0]["Item_rate"].ToString();

                            // here requested for first time so is update flag is N
                            if (torder[0].Transaction_id.Trim() == "" && torder[0].Isfinalorder == "N")
                            {
                                #region Create Post Load Inquiry

                                string Msg = "";

                                try
                                {
                                    DBCommand.Connection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();

                                    dsdubiorder.EnforceConstraints = false;
                                    dsdubiorder.dubiz_goods_orders_details_temp.ImportRow(dtPostOrderParameter.Rows[0]);


                                    if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "PDO", "", "", ref OrdId, ref message)) // New dubizzle goods move Notification ID
                                    {
                                        ServerLog.Log(message);
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", message);
                                    }
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].Transaction_id = OrdId;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].Total_cost_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].active_flag = Constant.FLAG_Y;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].created_date = System.DateTime.UtcNow;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].created_by = torder[0].created_by;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].device_id = torder[0].device_id;

                                    dsdubiorder.EnforceConstraints = true;
                                    objBLReturnObject = master.UpdateTables(dsdubiorder.dubiz_goods_orders_details_temp, ref DBCommand);
                                    if (objBLReturnObject.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLReturnObject.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                    }
                                    else
                                    {
                                        DBCommand.Transaction.Commit();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                                        #region MyRegion

                                        DataTable dtParameters = new DataTable();
                                        dtParameters.Columns.AddRange(new DataColumn[]{ new DataColumn("EmailID"),
                                                                                    new DataColumn("Message"),
                                                                                    new DataColumn("Source"),
                                                                                    new DataColumn("mobile_no"),
                                                                                    new DataColumn("name"),
                                                                                    new DataColumn("subject"),
                                                                                    new DataColumn("Sizetypecode"),
                                                                                    new DataColumn("Sourceaddress"),
                                                                                    new DataColumn("Destinationaddress"),
                                                                                    new DataColumn("ShippingDatetime"),
                                                                                    new DataColumn("load_inquiry_no"),
                                                                                    new DataColumn("TotalDistance"),
                                                                                    new DataColumn("TotalDistanceUOM")

                                                                                    });
                                        DataTable dtItemdesc = GetDubiItemByitemcode(dtPostOrderParameter.Rows[0]["item_code"].ToString());

                                        DataRow drtoins = dtParameters.NewRow();
                                        drtoins["EmailID"] = dtPostOrderParameter.Rows[0]["user_email"].ToString();
                                        drtoins["Message"] = "";
                                        drtoins["Source"] = "QUOTES";
                                        drtoins["mobile_no"] = dtPostOrderParameter.Rows[0]["user_no"].ToString();
                                        drtoins["name"] = dtPostOrderParameter.Rows[0]["user_name"].ToString();
                                        drtoins["subject"] = dtItemdesc.Rows[0]["Item_desc"].ToString();
                                        drtoins["Sourceaddress"] = dtPostOrderParameter.Rows[0]["inquiry_source_addr"].ToString();
                                        drtoins["Destinationaddress"] = dtPostOrderParameter.Rows[0]["inquiry_destination_addr"].ToString();
                                        drtoins["load_inquiry_no"] = OrdId;
                                        drtoins["TotalDistance"] = dtPostOrderParameter.Rows[0]["TotalDistance"].ToString();
                                        drtoins["TotalDistanceUOM"] = dtPostOrderParameter.Rows[0]["TotalDistanceUOM"].ToString();
                                        dtParameters.Rows.Add(drtoins);

                                        DataTable dtReqQuoteMail = new DataTable();
                                        if (dtPostOrderParameter.Rows[0]["Transaction_id"].ToString() != "")
                                            dtReqQuoteMail = new MailerController().GetRequestQuotationbyID(OrdId);
                                        else
                                            dtReqQuoteMail = null;

                                        if (dtParameters != null)
                                        {
                                            DBConnection.Open();
                                            DBCommand.Transaction = DBConnection.BeginTransaction();

                                            Boolean blStatus = new MailerController().SendDubi_Goods_QuotationRequestMailandSaveData(ref DBCommand, ref dtParameters, ref Message);
                                            if (blStatus)
                                            {
                                                DBCommand.Transaction.Commit();
                                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                            }
                                            else
                                            {
                                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                                return BLGeneralUtil.return_ajax_string("0", Message);
                                            }
                                        }

                                        #endregion

                                        return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dsdubiorder.dubiz_goods_orders_details_temp));
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }


                                #endregion


                            }

                            if (torder[0].Transaction_id.Trim() == "" && torder[0].Isfinalorder == "N")
                            {
                                #region Create Post Load Inquiry

                                string Msg = "";

                                try
                                {
                                    DBCommand.Connection.Open();
                                    DBCommand.Transaction = DBConnection.BeginTransaction();

                                    dsdubiorder.EnforceConstraints = false;
                                    dsdubiorder.dubiz_goods_orders_details_temp.ImportRow(dtPostOrderParameter.Rows[0]);
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].Total_cost_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].active_flag = Constant.FLAG_Y;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].created_date = System.DateTime.UtcNow;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].created_by = torder[0].created_by;
                                    dsdubiorder.dubiz_goods_orders_details_temp[0].device_id = torder[0].device_id;

                                    dsdubiorder.EnforceConstraints = true;
                                    objBLReturnObject = master.UpdateTables(dsdubiorder.dubiz_goods_orders_details_temp, ref DBCommand);
                                    if (objBLReturnObject.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLReturnObject.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                    }
                                    else
                                    {
                                        DBCommand.Transaction.Commit();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dsdubiorder.dubiz_goods_orders_details_temp));
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                                }


                                #endregion
                            }

                            if (torder[0].Transaction_id != "" && torder[0].Isfinalorder == Constant.FLAG_Y)
                            {
                                DataTable dtDubiGoodsMoveTemp = GetDubiGoodsOrdersbyId(torder[0].Transaction_id);
                                DBCommand.Connection.Open();
                                DBCommand.Transaction = DBConnection.BeginTransaction();

                                dsdubiorder.EnforceConstraints = false;
                                if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "PDO", "", "", ref OrdId, ref message)) // New Driver Notification ID
                                {
                                    ServerLog.Log(message);
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", message);
                                }

                                dsdubiorder.dubiz_goods_orders_details.ImportRow(dtPostOrderParameter.Rows[0]);
                                dsdubiorder.dubiz_goods_orders_details[0].Transaction_id = OrdId;
                                dsdubiorder.dubiz_goods_orders_details[0].rem_amt_to_receive = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                dsdubiorder.dubiz_goods_orders_details[0].Total_cost_without_discount = Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Total_cost"].ToString());
                                dsdubiorder.dubiz_goods_orders_details[0].active_flag = Constant.FLAG_Y;
                                dsdubiorder.dubiz_goods_orders_details[0].status = Constant.BEST_QUOTE_SELECTED_BY_ADMIN;
                                dsdubiorder.dubiz_goods_orders_details[0].created_date = System.DateTime.UtcNow;
                                dsdubiorder.dubiz_goods_orders_details[0].created_by = torder[0].created_by;
                                dsdubiorder.dubiz_goods_orders_details[0].device_id = torder[0].device_id;

                                dsdubiorder.EnforceConstraints = true;
                                objBLReturnObject = master.UpdateTables(dsdubiorder.dubiz_goods_orders_details, ref DBCommand);
                                if (objBLReturnObject.ExecutionStatus == 2)
                                {
                                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    objBLReturnObject.ExecutionStatus = 2;
                                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                }
                                else
                                {
                                    DBCommand.Transaction.Commit();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dsdubiorder.dubiz_goods_orders_details));
                                }
                            }
                            else
                                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostOrderParameter));
                        }
                        else
                            return BLGeneralUtil.return_ajax_string("0", " Date not found ");

                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog("CalculateRate(" + Convert.ToString(objPostOrder) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }



        #endregion

        #endregion

    }
}
