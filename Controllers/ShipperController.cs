using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using trukkerUAE.Models;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using BLL.Utilities;
using BLL.Master;
using trukkerUAE.XSD;
using System.Text;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using trukkerUAE.Classes;
using trukkerUAE.Controllers;
using System.ComponentModel;
using System.IO;
using System.Configuration;
using System.Web;
using trukkerUAE.BLL.Master;


namespace trukkerUAE.Controllers
{
    public class ShipperController : ServerBase
    {
        Master mst = new Master();

        BLReturnObject objBLReturnObject = new BLReturnObject();
        JavaScriptSerializer jser = new JavaScriptSerializer();
        StringBuilder sb = new StringBuilder();

        [HttpGet]
        public DataTable GetCompanyType()
        {
            String query = "";
            query = "SELECT company_type_code,company_type_desc FROM company_type Where active_flag='Y'";

            DataTable dtcompanytype = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtcompanytype = ds.Tables[0];
            }
            if (dtcompanytype != null && dtcompanytype.Rows.Count > 0)
                return dtcompanytype;
            else
                return null;
        }

        public DataTable GetUserID(string userid, string email)
        {
            String query = "SELECT * FROM user_mst where user_id  = @userid or email_id = @email ";
            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, userid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("email", DbType.String, email));
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

        private DataTable ValidateTransporter(string email, string mobile)
        {
            String query = "SELECT * FROM transporter_registration where email = @email or mobile_no = @moble";
            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("email", DbType.String, email));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("moble", DbType.String, mobile));
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
        //Regstration Shipper

        public DataTable GetShipper(string shipid)
        {

            try
            {
                DataTable dt_shi_id = new DataTable();

                string querry = "select * from Shipper_master where Shipper_id= @shpr ";
                DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("shpr", DbType.String, shipid));
                DBDataAdpterObject.SelectCommand.CommandText = querry;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_shi_id = ds.Tables[0];
                }
                DBConnection.Close();

                if (dt_shi_id != null && dt_shi_id.Rows.Count > 0)
                    return dt_shi_id;
                else
                    return null;
            }//end try
            catch (Exception ex)
            {
                return null;


            }


        }

        [HttpPost]
        public string RegisterTransporter([FromBody]JObject jobj)
        {
            DataTable dt_transporter = new DataTable();
            string DocNtficID = "";
            string message = ""; bool stat = false; bool IsNewTransporter = false; string fullname = ""; string domainname = "";
            Document objdoc = new Document();
            try
            {
                List<Transporter> tord = new List<Transporter>();
                tord = jobj["Transporter"].ToObject<List<Transporter>>();

                if (!ValidateTransporter(tord, ref message))
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                BLReturnObject objBLobj = new BLReturnObject();

                if (CheckRegistration(tord[0].email, tord[0].mobile_no, "T"))
                {
                    return BLGeneralUtil.return_ajax_string("0", "This Transporter already registered with Trukker");
                }

                string regtype = tord[0].reg_type;
                domainname = tord[0].domainname;

                DBCommand.Connection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                try
                {
                    DS_Transporter.transporter_registrationDataTable dstransporter = new DS_Transporter.transporter_registrationDataTable();
                    DS_Transporter.transporter_registration_tempDataTable Dstemp = jobj["Transporter"].ToObject<DS_Transporter.transporter_registration_tempDataTable>();//object of dataset
                    DocNtficID = "";
                    stat = objdoc.W_GetNextDocumentNo(ref DBCommand, "TR", "", "", ref DocNtficID, ref message);
                    if (stat == false)
                    {
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }
                    IsNewTransporter = true;
                    Dstemp[0].transporter_id = DocNtficID;
                    Dstemp[0].full_name = Dstemp[0].first_name + ' ' + Dstemp[0].last_name;
                    Dstemp[0].reg_date = System.DateTime.UtcNow;
                    Dstemp[0].created_by = "NEW TRANSPORTER";
                    Dstemp[0].created_date = System.DateTime.UtcNow;
                    Dstemp[0].email_verified = Constant.Flag_No;
                    Dstemp[0].active_flag = Constant.Flag_No;
                    dstransporter.ImportRow(Dstemp[0]);

                    DS_Transporter.transporter_company_typeDataTable dsCmptype = new DS_Transporter.transporter_company_typeDataTable();//object of dataset
                    DS_Transporter.transporter_company_type_tempDataTable dsCmptemptype = jobj["CmpType"].ToObject<DS_Transporter.transporter_company_type_tempDataTable>();//object of dataset
                    if (dsCmptemptype != null && dsCmptemptype.Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCmptemptype.Rows.Count; i++)
                        {
                            dsCmptemptype[i].transporter_id = DocNtficID;
                            dsCmptemptype[i].created_by = "NEW TRANSPORTER";
                            dsCmptemptype[i].created_date = System.DateTime.UtcNow;
                            dsCmptemptype[i].active_flag = Constant.Flag_Yes;
                            dsCmptype.ImportRow(dsCmptemptype[i]);
                        }
                    }

                    DS_Transporter.transporter_truck_typeDataTable dstrktype = new DS_Transporter.transporter_truck_typeDataTable();//object of dataset
                    DS_Transporter.transporter_truck_type_tempDataTable dstrktemptype = jobj["TruckType"].ToObject<DS_Transporter.transporter_truck_type_tempDataTable>();//object of dataset
                    if (dstrktemptype != null && dstrktemptype.Rows.Count > 0)
                    {
                        for (int j = 0; j < dstrktemptype.Rows.Count; j++)
                        {
                            dstrktemptype[j].transporter_id = DocNtficID;
                            dstrktemptype[j].created_by = "NEW TRANSPORTER";
                            dstrktemptype[j].created_date = System.DateTime.UtcNow;
                            dstrktemptype[j].active_flag = Constant.Flag_Yes;
                            dstrktype.ImportRow(dstrktemptype[j]);
                        }
                    }

                    objBLobj = mst.UpdateTables(dstransporter, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    if (dsCmptype != null && dsCmptype.Rows.Count > 0)
                    {
                        objBLobj = mst.UpdateTables(dsCmptype, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }

                    if (dstrktype != null && dstrktype.Rows.Count > 0)
                    {
                        objBLobj = mst.UpdateTables(dstrktype, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(GenerateRegistrationEmail(tord[0].email.ToString(), DocNtficID, fullname, "T", domainname));
                    if (result["status"].ToString() == "0")
                    {
                        return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email ");

                    }
                    return BLGeneralUtil.return_ajax_string("1", "Data Saved Successfully");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }

        public DataTable GetTransporter(string trnsid)
        {
            try
            {
                DataTable dt_shi_id = new DataTable();

                string querry = "select * from Transporter_registration where transporter_id = @trns ";
                DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("trns", DbType.String, trnsid));
                DBDataAdpterObject.SelectCommand.CommandText = querry;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_shi_id = ds.Tables[0];
                }
                DBConnection.Close();

                if (dt_shi_id != null && dt_shi_id.Rows.Count > 0)
                    return dt_shi_id;
                else
                    return null;
            }//end try
            catch (Exception ex)
            {
                return null;
            }
        }

        private Boolean ValidateTransporter(List<Transporter> trns, ref string msg)
        {

            if (trns[0].first_name == null || trns[0].first_name.ToString() == "")
            {
                msg = "Please provide First Name";
                return false;
            }
            if (trns[0].last_name == null || trns[0].last_name.ToString() == "")
            {
                msg = "Please provide Last Name";
                return false;
            }
            if (trns[0].mobile_no == null || trns[0].mobile_no.ToString() == "")
            {
                msg = "Please provide Mobile no";
                return false;
            }
            if (trns[0].email == null || trns[0].email.ToString() == "")
            {
                msg = "Please provide email id";
                return false;
            }
            msg = "";
            return true;
        }

        [HttpPost]
        public string ActivateRegistration(string regid, string regtype)
        {
            DataTable dt_data = new DataTable();
            try
            {
                BLReturnObject objBLobj = new BLReturnObject();
                Document objdoc = new Document();
                if (regid == "" || String.IsNullOrEmpty(regid))
                {
                    return BLGeneralUtil.return_ajax_string("0", "Registration Id not provided");
                }

                if (regtype == "" || String.IsNullOrEmpty(regtype))
                {
                    return BLGeneralUtil.return_ajax_string("0", "Registration Type not provided");
                }


                if (regid != "" && regtype == "S")
                    dt_data = GetShipper(regid);
                if (regid != "" && regtype == "T")
                    dt_data = GetTransporter(regid);

                if (dt_data == null)
                {
                    ServerLog.Log("Registration Details not Found for Registration ID : " + regid + ", Registration Type : " + regtype);
                    return BLGeneralUtil.return_ajax_string("0", "Registration Details not found for activation");
                }
                if (dt_data != null && dt_data.Rows.Count > 0)
                {
                    if (regtype == "S")
                    {
                        if (!String.IsNullOrEmpty(dt_data.Rows[0]["email_verified"].ToString()))
                        {
                            if (dt_data.Rows[0]["email_verified"].ToString().Trim() == Constant.Flag_Yes)
                            {
                                return BLGeneralUtil.return_ajax_string("0", "Shipper already activated ");
                            }
                        }
                        if (!String.IsNullOrEmpty(dt_data.Rows[0]["active_flag"].ToString()))
                        {
                            if (dt_data.Rows[0]["active_flag"].ToString().Trim() == Constant.Flag_Yes)
                            {
                                return BLGeneralUtil.return_ajax_string("0", "Shipper already activated ");
                            }
                        }
                    }
                    else if (regtype == "T")
                    {
                        if (!String.IsNullOrEmpty(dt_data.Rows[0]["email_verified"].ToString()))
                        {
                            if (dt_data.Rows[0]["email_verified"].ToString().Trim() == Constant.Flag_Yes)
                            {
                                return BLGeneralUtil.return_ajax_string("0", "Transporter already activated ");
                            }
                        }
                        if (!String.IsNullOrEmpty(dt_data.Rows[0]["active_flag"].ToString()))
                        {
                            if (dt_data.Rows[0]["active_flag"].ToString().Trim() == Constant.Flag_Yes)
                            {
                                return BLGeneralUtil.return_ajax_string("0", "Transporter already activated ");
                            }
                        }
                    }
                }

                if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                try
                {
                    if (regtype == "S")
                    {
                        DS_Shipper.shipper_masterDataTable dsshipper = new DS_Shipper.shipper_masterDataTable();
                        if (dt_data != null)
                        {
                            dsshipper.ImportRow(dt_data.Rows[0]);
                            dsshipper[0].email_verified = Constant.Flag_Yes;
                            dsshipper[0].active_flag = Constant.Flag_Yes;
                            dsshipper[0].modified_by = "admin";
                            dsshipper[0].modified_date = System.DateTime.UtcNow;
                            dsshipper[0].modified_host = "admin";
                            dsshipper[0].modified_device_id = "admin";
                            dsshipper[0].modified_device_type = "admin";
                            dsshipper.AcceptChanges();
                            dsshipper[0].SetAdded();
                        }

                        objBLobj = mst.UpdateTables(dsshipper, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 1;
                        return BLGeneralUtil.return_ajax_string("1", "Activated Successfully");

                    }
                    else if (regtype == "T")
                    {
                        DS_Transporter.transporter_registrationDataTable dstransporter = new DS_Transporter.transporter_registrationDataTable();
                        if (dt_data != null)
                        {
                            dstransporter.ImportRow(dt_data.Rows[0]);
                            dstransporter[0].email_verified = Constant.Flag_Yes;
                            dstransporter[0].active_flag = Constant.Flag_Yes;
                            dstransporter[0].modified_by = "admin";
                            dstransporter[0].modified_date = System.DateTime.UtcNow;
                            dstransporter[0].modified_host = "admin";
                            dstransporter[0].modified_device_id = "admin";
                            dstransporter[0].modified_device_type = "admin";
                            dstransporter.AcceptChanges();
                            dstransporter[0].SetAdded();
                        }
                        objBLobj = mst.UpdateTables(dstransporter, ref DBCommand);
                        if (objBLobj.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLobj.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 1;
                        return BLGeneralUtil.return_ajax_string("1", "Activated Successfully");
                    }
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
                return BLGeneralUtil.return_ajax_string("1", "Activated Successfully");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpPost]
        public string RegisterShipper([FromBody]JObject jobj)
        {

            // generate OTP when driver postloads inquiry for receiver

            #region GenerateOTP

            // by sandip 7/4/2016 for generate otp rendom number
            Random rand = new Random();
            int Otp = rand.Next(1000, 9999);


            #endregion

            DataTable dt_shipper = new DataTable();
            try
            {
                List<User> tuser = new List<User>();
                tuser = jobj["Shipper_master"].ToObject<List<User>>();
                DataTable dtuser = BLGeneralUtil.ToDataTable(tuser);
                BLReturnObject objBLobj = new BLReturnObject();
                Master mst = new Master();

                DS_User ds_user = new DS_User();
                string DocNtficID = ""; string fullname = "";
                string message = ""; bool stat = false; string domainname = "";
                Document objdoc = new Document();

                if (!ValidateShipper(tuser, ref message))
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                DataTable dt_log = new DataTable();
                dt_log = GetUser(tuser[0].email_id, tuser[0].user_id);

                if (dt_log != null && dt_log.Rows.Count > 0)
                {
                    return BLGeneralUtil.return_ajax_string("0", "User already registered with Trukker");
                }

                DBCommand.Connection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();


                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "SH", "", "", ref DocNtficID, ref message)) // New Driver Notification ID
                {
                    ServerLog.Log(message);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                ds_user.EnforceConstraints = false;
                ds_user.user_mst.ImportRow(dtuser.Rows[0]);
                ds_user.user_mst.Rows[0]["client_type"] = dtuser.Rows[0]["device_type"];
                ds_user.user_mst.Rows[0]["user_id"] = dtuser.Rows[0]["user_id"];
                ds_user.user_mst.Rows[0]["role_id"] = "SH";
                ds_user.user_mst.Rows[0]["unique_id"] = DocNtficID;
                ds_user.user_mst.Rows[0]["first_name"] = dtuser.Rows[0]["first_name"];
                ds_user.user_mst.Rows[0]["middle_name"] = dtuser.Rows[0]["middle_name"];
                ds_user.user_mst.Rows[0]["last_name"] = dtuser.Rows[0]["last_name"];
                ds_user.user_mst.Rows[0]["email_id"] = dtuser.Rows[0]["email_id"];
                ds_user.user_mst.Rows[0]["password"] = dtuser.Rows[0]["password"];
                ds_user.user_mst.Rows[0]["start_date"] = System.DateTime.UtcNow;
                ds_user.user_mst.Rows[0]["user_loc_flag"] = "L";
                ds_user.user_mst.Rows[0]["pass_expiry_date"] = System.DateTime.UtcNow.AddYears(10);
                ds_user.user_mst.Rows[0]["user_status_flag"] = Constant.Flag_No;
                ds_user.user_mst.Rows[0]["created_by"] = dtuser.Rows[0]["created_by"];
                ds_user.user_mst.Rows[0]["created_date"] = System.DateTime.UtcNow;
                ds_user.user_mst.Rows[0]["created_host"] = dtuser.Rows[0]["created_host"];
                ds_user.user_mst.Rows[0]["OTP"] = Otp;



                ds_user.EnforceConstraints = true;
                objBLReturnObject = mst.UpdateTables(ds_user.user_mst, ref DBCommand);
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
                objBLobj.ExecutionStatus = 1;

                if (tuser[0].load_inquiry_no.Trim() != "")
                {
                    DataTable dtPostOrder = new PostOrderController().GetLoadInquiryBySizetypeId(tuser[0].load_inquiry_no.Trim());
                    DataTable dtuserTemp = new DataTable();

                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    try
                    {
                        DS_Post_load_enquiry ds_post_load = new DS_Post_load_enquiry();
                        ds_post_load.EnforceConstraints = false;
                        ds_post_load.post_load_inquiry.ImportRow(dtPostOrder.Rows[0]);
                        ds_post_load.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        ds_post_load.post_load_inquiry[0].shipper_id = DocNtficID;
                        ds_post_load.EnforceConstraints = true;
                        objBLReturnObject = mst.UpdateTables(ds_post_load.post_load_inquiry, ref DBCommand);
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
                        //ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

                        try
                        {


                            String SizeTypeCode = dtPostOrder.Rows[0]["SizeTypeCode"].ToString();
                            String goods_type_flag = dtPostOrder.Rows[0]["goods_type_flag"].ToString();
                            String rate_type_flag = "B";
                            String order_type_flag = dtPostOrder.Rows[0]["order_type_flag"].ToString();
                            Decimal TotalDistance = Convert.ToDecimal(dtPostOrder.Rows[0]["TotalDistance"].ToString());
                            String TotalDistanceUOM = dtPostOrder.Rows[0]["TotalDistanceUOM"].ToString();

                            TimeSpan Tsshippingtime = TimeSpan.Parse(dtPostOrder.Rows[0]["TimeToTravelInMinute"].ToString());
                            Decimal TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);
                            String IncludePackingCharge = dtPostOrder.Rows[0]["IncludePackingCharge"].ToString();
                            int? NoOfTruck = dtPostOrder.Rows[0]["NoOfTruck"].ToString() != "" ? Convert.ToInt16(dtPostOrder.Rows[0]["NoOfTruck"].ToString()) : 0;
                            int? NoOfDriver = dtPostOrder.Rows[0]["NoOfDriver"].ToString() != "" ? Convert.ToInt16(dtPostOrder.Rows[0]["NoOfDriver"].ToString()) : 0;
                            int? NoOfLabour = 0;
                            int? NoOfHandiman = 0;
                            int? NoOfSupervisor = dtPostOrder.Rows[0]["NoOfSupervisor"].ToString() != "" ? Convert.ToInt16(dtPostOrder.Rows[0]["NoOfSupervisor"].ToString()) : 0;
                            String Message = "";
                            DataTable dtSizeTypeMst = new DataTable();
                            DateTime OrderShippingDatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());

                            if (order_type_flag == Constant.ORDERTYPECODEFORHOME)
                                dtSizeTypeMst = new TruckerMaster().CalculateRate(null, SizeTypeCode, OrderShippingDatetime, goods_type_flag, "D", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            else
                                dtSizeTypeMst = new TruckerMaster().CalculateRateGoods(null, SizeTypeCode, OrderShippingDatetime, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            //ServerLog.SuccessLog("Temp Load Inquiry Saved With Quotation Inquire ID : " + OrdId);

                            dtuserTemp = dtSizeTypeMst;
                            dtuserTemp.Columns.Add("email_id");
                            dtuserTemp.Rows[0]["email_id"] = dtuser.Rows[0]["email_id"].ToString();
                            dtuserTemp.Columns.Add("role_id");
                            dtuserTemp.Rows[0]["role_id"] = dtuser.Rows[0]["role_id"].ToString();
                            dtuserTemp.Columns.Add("first_name");
                            dtuserTemp.Rows[0]["first_name"] = dtuser.Rows[0]["first_name"].ToString();
                            dtuserTemp.Columns.Add("password");
                            dtuserTemp.Rows[0]["password"] = dtuser.Rows[0]["password"].ToString();
                            dtuserTemp.Columns.Add("unique_id");
                            dtuserTemp.Rows[0]["unique_id"] = DocNtficID;
                            dtuserTemp.Columns.Add("user_id");
                            dtuserTemp.Rows[0]["user_id"] = dtuser.Rows[0]["user_id"].ToString();

                        }
                        catch (Exception ex)
                        {
                            ServerLog.OTPLog(ex.Message + ex.StackTrace);
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }


                        try
                        {
                            ServerLog.OTPLog("start Verification Code");
                            // send OTP to User Mobile number when user post load
                            new EMail().SendOtpToUserMobileNoUAE(" Thank you for Joining Trukker Your Verification Code to activate user id is : " + Otp.ToString(), tuser[0].user_id);
                            ServerLog.OTPLog("Verification Code send");
                        }
                        catch (Exception ex)
                        {
                            ServerLog.OTPLog(ex.Message + ex.StackTrace);
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", ex.Message);
                        }

                        ServerLog.Log("Mail start");
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateRegistrationEmailToshipper(tuser[0].email_id.ToString(), DocNtficID, dtuser.Rows[0]["first_name"].ToString(), tuser[0].user_id, ""));
                        if (result["status"].ToString() == "0")
                        {
                            return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                        }

                        return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtuserTemp)));
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + ex.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }

                    //    return (BLGeneralUtil.return_ajax_string("0", "Invalid User name or password "));
                }


                ServerLog.OTPLog("start in");
                try
                {
                    ServerLog.OTPLog("start OTP");
                    // send OTP to User Mobile number when user post load
                    new EMail().SendOtpToUserMobileNoUAE(" Thank you for Joining Trukker Your Verification Code to activate user id is : " + Otp.ToString(), tuser[0].user_id);
                    ServerLog.OTPLog("Verification Code send");
                }
                catch (Exception ex)
                {
                    ServerLog.OTPLog(ex.Message + ex.StackTrace);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }

                ServerLog.Log("Mail start");
                var resultMail = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateRegistrationEmailToshipper(tuser[0].email_id.ToString(), DocNtficID, dtuser.Rows[0]["first_name"].ToString(), tuser[0].user_id, ""));
                if (resultMail["status"].ToString() == "0")
                {
                    return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                }

                return BLGeneralUtil.return_ajax_string("1", "Registration successfully done enter Verification Code send to your mobile number ");
            }
            catch (Exception ex)
            {
                ServerLog.OTPLog(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }

        [HttpGet]
        public string GenerateRegistrationEmail(string mailto, string regid, string UserName, string RegType, string domainname)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            StreamReader sr; string LINK = "";
            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\newmail_trukkeruae.html");
            string strdata = ""; string msg = "";
            string TITLE = "TruKKer Technologies ";

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
                //strdata = strdata.Replace("AUTHLINK", ConfigurationManager.AppSettings["Domain"] + "check_activation.html?regid=" + regid + "&regtype=" + RegType);
                //strdata = strdata.Replace("AUTHLINK", domainname + "check_activation.html?regid=" + regid + "&regtype=" + RegType);
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

        private Boolean ValidateShipper(List<User> shpr, ref string msg)
        {

            if (shpr[0].first_name == null || shpr[0].first_name.ToString() == "")
            {
                msg = "Please provide First Name";
                return false;
            }
            if (shpr[0].user_id == null || shpr[0].user_id.ToString() == "")
            {
                msg = "Please provide Mobile no";
                return false;
            }

            msg = "";
            return true;
        }
        //get shipper id

        private bool CheckRegistration(string email, string mobile, string regtype)
        {
            String query = "";
            if (regtype == "S")
                query = "SELECT * FROM shipper_master where email = @email or mobile_no = @moble";
            else if (regtype == "T")
                query = "SELECT * FROM transporter_registration where email = @email or mobile_no = @moble";
            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("email", DbType.String, email));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("moble", DbType.String, mobile));
            DBDataAdpterObject.SelectCommand.CommandText = query;

            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dt = ds.Tables[0];
            }
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public DataTable GetUser(string emailid, string moblno)
        {
            try
            {
                DataTable dt_user = new DataTable();
                string qury = "select * from user_mst where user_id= @mobln"; // user_id= @emlid or email_id = @emlid or user_id=@mobln ";
                if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                //                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("emlid", DbType.String, emailid));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("mobln", DbType.String, moblno));
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

        //        [HttpPost]
        //        public string GetShipperOrdersDetails([FromBody]JObject Jobj)
        //        {

        //            List<orders> objOrder = new List<orders>();
        //            string status = ""; string fromdate = ""; string Todate = ""; string sourcecity = ""; string destinationcity = ""; string owner_id = "";
        //            objOrder = Jobj["shipper_order"].ToObject<List<orders>>();
        //            if (Jobj["shipper_order"] != null)
        //            {
        //                status = objOrder[0].status;
        //                fromdate = objOrder[0].fromdate;
        //                Todate = objOrder[0].todate;
        //                sourcecity = objOrder[0].inquiry_source_city;
        //                destinationcity = objOrder[0].inquiry_destination_city;
        //                owner_id = objOrder[0].owner_id;
        //            }

        //            try
        //            {
        //                DataTable dtPostLoadOrders = new DataTable();
        //                StringBuilder SQLSelect = new StringBuilder();

        //                SQLSelect.Append(@"select statusFinal as status,* from ( select driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,quote_hdr.GeneratedPdfLink as cbm_GeneratedPdfLink,truckdetails.*,quote_hdr.total_cbm,quote_hdr.StatusFlag,status1 as statusFinal,
        //                                                        feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,
        //                                                        (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,lstorder.* from (
        //					                                      select CASE WHEN order_cancellation_details.cancellation_id IS NULL THEN result.status ELSE '25' END AS status1, order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck,  
        //					                                      order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.*,
        //					                                      (SELECT   top(1)  truck_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_truck_id,
        //                                                         (SELECT   top(1)  driver_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_id
        //					                                      from 
        //					                                      ( 
        //					                                    select    SizeTypeMatrix.CBM_Max, user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.order_id, orders.shipper_id, orders.load_inquiry_no, 
        //					                                    orders.isassign_driver_truck, orders.isassign_mover, orders.owner_id, orders.shipper_email_id, orders.inquiry_source_addr, orders.inquiry_source_city, 
        //					                                    orders.inquiry_source_pincode, orders.inquiry_source_state, orders.inquiry_source_lat, orders.inquiry_source_lng, orders.inquiry_destination_addr, 
        //					                                    orders.inquiry_destination_city, orders.inquiry_destination_pincode, orders.inquiry_destination_state, orders.inquiry_destionation_lat, 
        //					                                    orders.inquiry_destionation_lng, orders.aprox_kms, orders.aprox_days, orders.load_inquiry_truck_type, orders.load_inquiry_shipping_date, 
        //					                                    orders.load_inquiry_shipping_time, orders.load_inquiry_delivery_date, orders.load_inquiry_delivery_time, orders.load_inquiry_material_type, 
        //					                                    orders.load_inquiry_load, orders.load_inquiry_packing, orders.required_price, orders.quotation_cost, orders.quotation_others, orders.asked_quot_cost, 
        //					                                    orders.driver_da, orders.toll_cost, orders.quotation_total_cost, orders.quotation_estimation_transit_time, orders.quotation_estimated_distance, 
        //					                                    orders.quotation_estimated_travel_time, orders.quotation_estimated_delivery_time, orders.quotation_distance_based_cost, orders.quotation_advance_paid_amount, 
        //					                                    orders.quotation_payment_due, orders.quotation_final_paid_amount, orders.destination_address, orders.destination_pincode, orders.receiver_name, 
        //					                                    orders.receiver_mobile, orders.receiver_email, orders.otp, orders.distance_kms_to_origin, orders.approx_time_to_reach, orders.status, orders.pickup_ready, 
        //					                                    orders.driver_reached, orders.document_issued, orders.document_received, orders.loading_lat, orders.loading_lng, orders.loading_start_time, 
        //					                                    orders.loading_end_time, orders.loading_end_time_actual, orders.start_commencement, orders.material_value, orders.material_description, orders.active_flag, 
        //					                                    orders.LR_Issued, orders.unloading_started, orders.unloading_complete_expected, orders.unloading_completed, orders.order_completion_date, orders.trackurl, 
        //					                                    orders.Remark, orders.geo_flag, orders.geo_kms, orders.IsCancel, orders.pickup_lat, orders.pickup_lng, orders.pickup_time, orders.start_lat, orders.start_lng, 
        //					                                    orders.start_time, orders.complete_lat, orders.complete_lng, orders.complete_time, orders.Is_receipt_upload, orders.receipt_imagepath, 
        //					                                    orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, 
        //					                                    orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, 
        //					                                    orders.TotalHandimanRate, orders.TotalSupervisorRate, orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, 
        //					                                    orders.Discount, orders.Total_cost_without_discount, orders.rem_amt_to_receive, orders.shippingdatetime, orders.Area, orders.rate_type_flag, 
        //					                                    orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, 
        //					                                    orders.destination_full_add, orders.billing_add, orders.cbmlink, orders.created_by, orders.created_date, orders.created_host, orders.device_id, orders.device_type, 
        //					                                    orders.modified_by, orders.modified_date, orders.modified_host, orders.modified_device_id, orders.modified_device_type, orders.goods_details,orders.invoice_pdf_link,'id1' OrderKey  
        //					                                    from orders    
        //					                                    JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
        //					                                    join user_mst on user_mst.unique_id =  orders.shipper_id
        //					                                    join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
        //					                                    Where 1=1 and orders.active_flag  = 'Y' and orders.IsCancel='N'  
        //					                                    and orders.shippingdatetime>getdate()  
        //					                                    UNION ALL 
        //					                                    select    SizeTypeMatrix.CBM_Max, user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.order_id, orders.shipper_id, orders.load_inquiry_no, 
        //					                                    orders.isassign_driver_truck, orders.isassign_mover, orders.owner_id, orders.shipper_email_id, orders.inquiry_source_addr, orders.inquiry_source_city, 
        //					                                    orders.inquiry_source_pincode, orders.inquiry_source_state, orders.inquiry_source_lat, orders.inquiry_source_lng, orders.inquiry_destination_addr, 
        //					                                    orders.inquiry_destination_city, orders.inquiry_destination_pincode, orders.inquiry_destination_state, orders.inquiry_destionation_lat, 
        //					                                    orders.inquiry_destionation_lng, orders.aprox_kms, orders.aprox_days, orders.load_inquiry_truck_type, orders.load_inquiry_shipping_date, 
        //					                                    orders.load_inquiry_shipping_time, orders.load_inquiry_delivery_date, orders.load_inquiry_delivery_time, orders.load_inquiry_material_type, 
        //					                                    orders.load_inquiry_load, orders.load_inquiry_packing, orders.required_price, orders.quotation_cost, orders.quotation_others, orders.asked_quot_cost, 
        //					                                    orders.driver_da, orders.toll_cost, orders.quotation_total_cost, orders.quotation_estimation_transit_time, orders.quotation_estimated_distance, 
        //					                                    orders.quotation_estimated_travel_time, orders.quotation_estimated_delivery_time, orders.quotation_distance_based_cost, orders.quotation_advance_paid_amount, 
        //					                                    orders.quotation_payment_due, orders.quotation_final_paid_amount, orders.destination_address, orders.destination_pincode, orders.receiver_name, 
        //					                                    orders.receiver_mobile, orders.receiver_email, orders.otp, orders.distance_kms_to_origin, orders.approx_time_to_reach, orders.status, orders.pickup_ready, 
        //					                                    orders.driver_reached, orders.document_issued, orders.document_received, orders.loading_lat, orders.loading_lng, orders.loading_start_time, 
        //					                                    orders.loading_end_time, orders.loading_end_time_actual, orders.start_commencement, orders.material_value, orders.material_description, orders.active_flag, 
        //					                                    orders.LR_Issued, orders.unloading_started, orders.unloading_complete_expected, orders.unloading_completed, orders.order_completion_date, orders.trackurl, 
        //					                                    orders.Remark, orders.geo_flag, orders.geo_kms, orders.IsCancel, orders.pickup_lat, orders.pickup_lng, orders.pickup_time, orders.start_lat, orders.start_lng, 
        //					                                    orders.start_time, orders.complete_lat, orders.complete_lng, orders.complete_time, orders.Is_receipt_upload, orders.receipt_imagepath, 
        //					                                    orders.SizeTypeCode, orders.TotalDistance, orders.TotalDistanceUOM, orders.TimeToTravelInMinute, orders.NoOfTruck, orders.NoOfDriver, 
        //					                                    orders.NoOfLabour, orders.NoOfHandiman, orders.NoOfSupervisor, orders.BaseRate, orders.TotalTravelingRate, orders.TotalDriverRate, orders.TotalLabourRate, 
        //					                                    orders.TotalHandimanRate, orders.TotalSupervisorRate, orders.IncludePackingCharge, orders.TotalPackingCharge, orders.Total_cost, orders.coupon_code, 
        //					                                    orders.Discount, orders.Total_cost_without_discount, orders.rem_amt_to_receive, orders.shippingdatetime, orders.Area, orders.rate_type_flag, 
        //					                                    orders.order_type_flag, orders.goods_type_flag, orders.payment_status, orders.payment_mode, orders.billing_name, orders.source_full_add, 
        //					                                    orders.destination_full_add, orders.billing_add, orders.cbmlink, orders.created_by, orders.created_date, orders.created_host, orders.device_id, orders.device_type, 
        //					                                    orders.modified_by, orders.modified_date, orders.modified_host, orders.modified_device_id, orders.modified_device_type, orders.goods_details,orders.invoice_pdf_link,'id2' OrderKey from orders  
        //					                                    JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
        //					                                    join user_mst on user_mst.unique_id =  orders.shipper_id
        //					                                    join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
        //					                                    Where 1=1 and orders.active_flag  = 'Y' and orders.IsCancel='N' 
        //					                                    and orders.shippingdatetime<getdate() 
        //					                                    ) result 	  
        //					                                    LEFT OUTER JOIN 	
        //					                                    ( 
        //						                                    select 	load_inquiry_no,COUNT(*) as NoOfAssignedDriver, COUNT(*) as NoOfAssignedTruck, SUM(CAST(NoOfHandiman AS int)) as NoOfAssignedHandiman, SUM(CAST(NoOfLabour AS int)) as NoOfAssignedLabour
        //						                                    from  order_driver_truck_details 
        //						                                    group by load_inquiry_no
        //					                                    ) as order_driver_truck_details_summary	  
        //					                                    ON result.load_inquiry_no = order_driver_truck_details_summary.load_inquiry_no
        //					                                    LEFT OUTER JOIN order_cancellation_details ON result.load_inquiry_no = order_cancellation_details.load_inquiry_no 
        //			                                    ) as lstorder  
        //			                                    left join (
        //					                                    SELECT distinct truck_mst.truck_id,truck_mst.body_type,truck_mst.truck_model,truck_mst.truck_make_id
        //					                                    ,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, 
        //					                                    truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, 
        //					                                    truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo 
        //					                                    from truck_mst 
        //					                                    left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id 
        //					                                    left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
        //					                                    left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id 
        //					                                    left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id 
        //					                                    left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id
        //			                                    ) as truckdetails on lstorder.driver_truck_id=truckdetails.truck_id
        //                                                left join driver_mst on lstorder.driver_id = driver_mst.driver_id
        //                                                left join feedback_mst on lstorder.load_inquiry_no =  Feedback_mst.load_inquiry_no
        //			                                    LEFT OUTER JOIN quote_hdr ON lstorder.load_inquiry_no = quote_hdr.quote_id 
        //			                                    ) as tblfinal
        //			                                    where  1=1");

        //                #region not in used


        //                //query1 = "    select    driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,driverdetails.*,quote_hdr.total_cbm,quote_hdr.StatusFlag,feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,(case ISNULL(oc.load_inquiry_no,'') when '' then result.status else '25' end) status, " +
        //                //   "    (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) " +
        //                //   "     ORDER BY CAST(status AS int) desc ) as final_status, " +
        //                //   "    orders.trackurl,orders.isassign_driver_truck,orders.isassign_mover,IsDraft,result.* " +
        //                //   "     from ( " +
        //                //   "            select post_load_inquiry.*,SizeTypeMst.SizeTypeDesc,order_driver_truck_details.driver_id,order_driver_truck_details.truck_id, 'id2' OrderKey from post_load_inquiry  " +
        //                //   "            LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = post_load_inquiry.sizetypecode " +
        //                //   "            left outer join  order_driver_truck_details on order_driver_truck_details.load_inquiry_no=post_load_inquiry.load_inquiry_no" +
        //                //   "            Where 1=1 and post_load_inquiry.active_flag  = 'Y' and shipper_id != '-9999' " +
        //                //   "            and post_load_inquiry.shippingdatetime>getdate() " +
        //                //   "            union all " +
        //                //   "            select post_load_inquiry.*,SizeTypeMst.SizeTypeDesc,order_driver_truck_details.driver_id,order_driver_truck_details.truck_id, 'id2' OrderKey from post_load_inquiry  " +
        //                //   "            LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = post_load_inquiry.sizetypecode " +
        //                //   "            left outer join  order_driver_truck_details on order_driver_truck_details.load_inquiry_no=post_load_inquiry.load_inquiry_no" +
        //                //   "            Where 1=1 and post_load_inquiry.active_flag  = 'Y' and shipper_id != '-9999' " +
        //                //   "            and post_load_inquiry.shippingdatetime<getdate() " +
        //                //   "    ) result  left join ( " +
        //                //   "                                      SELECT distinct truck_mst.truck_id,truck_mst.body_type,truck_body_mst.truck_body_desc, " +
        //                //   "                                    truck_rto_registration_detail.vehicle_reg_no " +
        //                //   "                                    from truck_mst " +
        //                //   "                                    left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id " +
        //                //   "                                    left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
        //                //   "                                    left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id " +
        //                //   "                                    left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id " +
        //                //   "                                    left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id" +
        //                //   "                                     ) as driverdetails on result.truck_id=driverdetails.truck_id" +
        //                //   "    left join driver_mst on result.driver_id = driver_mst.driver_id " +
        //                //   "    join orders on orders.load_inquiry_no = result.load_inquiry_no and orders.active_flag  = 'Y' " +
        //                //   "    left join feedback_mst on orders.load_inquiry_no =  Feedback_mst.load_inquiry_no " +
        //                //   "    left join  order_cancellation_details as OC on OC.load_inquiry_no= result.load_inquiry_no " +
        //                //   "    LEFT OUTER JOIN quote_hdr ON result.load_inquiry_no = quote_hdr.quote_id" +
        //                //   "    where  1=1  ";

        //                //query1 = " select   quote_hdr.total_cbm,quote_hdr.StatusFlag,feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,(case ISNULL(oc.load_inquiry_no,'') when '' then result.status else '25' end) status, " +
        //                //         " (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no) " +
        //                //         " ORDER BY CAST(status AS int) desc ) as final_status, " +
        //                //          "orders.trackurl,orders.isassign_driver_truck,orders.isassign_mover,IsDraft,result.* from ( " +
        //                //        " select post_load_inquiry.*,SizeTypeMst.SizeTypeDesc,'id2' OrderKey from post_load_inquiry  " +
        //                //        " LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = post_load_inquiry.sizetypecode " +
        //                //        " Where 1=1 and post_load_inquiry.active_flag  = 'Y' and shipper_id != '-9999' " +
        //                //        " and post_load_inquiry.shippingdatetime>getdate() " +
        //                //        " union all " +
        //                //        " select post_load_inquiry.*,SizeTypeMst.SizeTypeDesc,'id2' OrderKey from post_load_inquiry  " +
        //                //        " LEFT OUTER JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = post_load_inquiry.sizetypecode " +
        //                //        " Where 1=1 and post_load_inquiry.active_flag  = 'Y' and shipper_id != '-9999' " +
        //                //        " and post_load_inquiry.shippingdatetime<getdate() " +
        //                //        " ) result  " +
        //                //        " join orders on orders.load_inquiry_no = result.load_inquiry_no and orders.active_flag  = 'Y' " +
        //                //        " left join feedback_mst on orders.load_inquiry_no =  Feedback_mst.load_inquiry_no " +
        //                //        " left join  order_cancellation_details as OC on OC.load_inquiry_no= result.load_inquiry_no " +
        //                //        " LEFT OUTER JOIN quote_hdr ON result.load_inquiry_no = quote_hdr.quote_id" +
        //                //        " where  1=1 ";

        //                #endregion

        //                if (status != "")
        //                {
        //                    SQLSelect.Append("and tblfinal.statusFinal = '" + status + "'");
        //                    //if (status == "02" || status == "45")
        //                    //{
        //                    //    SQLSelect.Append(" and (case ISNULL(load_inquiry_no,'') when '' then statusfinal else '25' end)  = '" + status + "' ");
        //                    //}//   query1 += " and status = '" + status + "' ";
        //                    //else
        //                    //    SQLSelect.Append(" and (case ISNULL(load_inquiry_no,'') when '' then statusfinal else '25' end) not in (02,45) ");
        //                }
        //                if (owner_id != "")
        //                {
        //                    SQLSelect.Append(" and shipper_id = '" + owner_id + "'  ");
        //                }
        //                if (fromdate.Trim() != "" && Todate.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
        //                }
        //                else if (fromdate.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) >=  CONVERT(date,'" + fromdate.ToString() + "',103) ");
        //                }
        //                else if (Todate.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and CONVERT(date,tblfinal.load_inquiry_shipping_date,103) <= CONVERT(date,'" + Todate.ToString() + "',103)  ");
        //                }
        //                if (sourcecity.Trim() != "" && destinationcity.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%' and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
        //                }
        //                else if (sourcecity.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and tblfinal.inquiry_source_addr like'%" + sourcecity + "%'");
        //                }
        //                else if (destinationcity.Trim() != "")
        //                {
        //                    SQLSelect.Append(" and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
        //                }

        //                SQLSelect.Append("  order by OrderKey,shippingdatetime asc");

        //                //if (fromdate.Trim() != "" && Todate.Trim() != "")
        //                //{
        //                //    SQLSelect.Append("and CAST(load_inquiry_shipping_date AS DATE) >='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(load_inquiry_shipping_date AS DATE) <= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "'  ");
        //                //}
        //                //else if (fromdate.Trim() != "")
        //                //{
        //                //    SQLSelect.Append("and CAST(load_inquiry_shipping_date AS DATE) >='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "'  ");
        //                //}
        //                //else if (Todate.Trim() != "")
        //                //{
        //                //    SQLSelect.Append(" and CAST(load_inquiry_shipping_date AS DATE) <= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "'  ");
        //                //}
        //                //if (sourcecity.Trim() != "" && destinationcity.Trim() != "")
        //                //{
        //                //    SQLSelect.Append(" and inquiry_source_addr like'%" + sourcecity + "%' and inquiry_destination_addr like '%" + destinationcity + "%' ");
        //                //}
        //                //else if (sourcecity.Trim() != "")
        //                //{
        //                //    SQLSelect.Append(" and inquiry_source_addr like'%" + sourcecity + "%'");
        //                //}
        //                //else if (destinationcity.Trim() != "")
        //                //{
        //                //    SQLSelect.Append(" and result.inquiry_destination_addr like '%" + destinationcity + "%' ");
        //                //}

        //                //SQLSelect.Append(" order by OrderKey,shippingdatetime asc ");

        //                DBDataAdpterObject.SelectCommand.Parameters.Clear();
        //                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
        //                DataSet ds = new DataSet();
        //                DBDataAdpterObject.Fill(ds);
        //                if (ds != null && ds.Tables.Count > 0)
        //                {
        //                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        //                        dtPostLoadOrders = ds.Tables[0];
        //                }
        //                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
        //                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
        //                else
        //                    return BLGeneralUtil.return_ajax_string("0", "No Data Found");
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.Log(ex.Message + ex.StackTrace);
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //            }
        //        }

        public string SendCancelOrderRequestByShipper([FromBody]JObject Jobj)
        {
            try
            {
                if (Jobj["load_inquiry_no"].ToString() == "")
                    return BLGeneralUtil.return_ajax_string("0", "please povide load inquiry number");
                if (Jobj["shipper_id"].ToString() == "")
                    return BLGeneralUtil.return_ajax_string("0", "Please provide shipper id");

                Master master = new Master(); Document objDocument = new Document();
                string logid = ""; string msg = "";
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_orders dsOrder = new DS_orders();

                DataTable dt_orders = new PostOrderController().GetOrders(Jobj["load_inquiry_no"].ToString());
                DataTable dt_Cancelorders = new PostOrderController().GetCancelOrdersRequestByInq(Jobj["load_inquiry_no"].ToString());
                if (dt_Cancelorders != null)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Alredy Requested For Cancellation");
                }

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                try
                {



                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "CN", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    DS_orders.order_cancellation_detailsRow tr_order_cancel = dsOrder.order_cancellation_details.Neworder_cancellation_detailsRow();

                    dsOrder.EnforceConstraints = false;
                    tr_order_cancel.cancellation_id = logid;
                    tr_order_cancel.load_inquiry_no = Jobj["load_inquiry_no"].ToString();
                    tr_order_cancel.shipper_id = Jobj["shipper_id"].ToString();
                    tr_order_cancel.Reason = Jobj["Reason"].ToString();
                    tr_order_cancel.status = Constant.FLAG_N;

                    dsOrder.order_cancellation_details.Addorder_cancellation_detailsRow(tr_order_cancel);
                    dsOrder.order_cancellation_details[0].AcceptChanges();
                    dsOrder.order_cancellation_details[0].SetAdded();
                    dsOrder.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dsOrder.order_cancellation_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }



                    string MsgMailbody = "";
                    string shippername = new PostOrderController().GetUserdetailsByID(Jobj["shipper_id"].ToString());



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
                    newmsg = " Order ID: " + Jobj["load_inquiry_no"].ToString() + "  <br/>   <b> UserName : </b>" + shippername + " <b> Reason : </b>" + Jobj["Reason"].ToString();
                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    bl = objemail.SendMail(sendto, newmsg, "Request For cancellation Order by " + shippername + " Order ID :" + Jobj["load_inquiry_no"].ToString(), ref msg, fromemail, frompword, "TruKKer Technologies");
                    if (!bl)
                    {
                        ServerLog.Log("Error Sending cancellation Email To Admin");
                        // return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    if (dt_orders != null)
                        MsgMailbody = " Your order from  " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + " cancellation request made successfully On " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + Jobj["load_inquiry_no"].ToString();

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderCancellationEmail(new PostOrderController().GetEmailByID(Jobj["shipper_id"].ToString()), " Order cancellation Request for (Order ID: " + Jobj["load_inquiry_no"].ToString() + ")", shippername, MsgMailbody, Jobj["load_inquiry_no"].ToString()));
                    if (result["status"].ToString() == "0")
                    {
                        ServerLog.Log("Error Sending cancellation Email To User");
                    }

                    msg = "";

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;


                    return BLGeneralUtil.return_ajax_string("1", " Order Cancellation Request Sent ");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }

                //                return BLGeneralUtil.return_ajax_string("1", " Order cancelation request send Successfully for Order ID :" + Jobj["load_inquiry_no"].ToString());
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }


        }

        public string SendCancelOrderRequestByShipperNew([FromBody]JObject Jobj)
        {
            try
            {
                if (Jobj["load_inquiry_no"].ToString() == "")
                    return BLGeneralUtil.return_ajax_string("0", "please povide load inquiry number");
                if (Jobj["shipper_id"].ToString() == "")
                    return BLGeneralUtil.return_ajax_string("0", "Please provide shipper id");

                Master master = new Master(); Document objDocument = new Document();
                string logid = ""; string msg = "";
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_orders dsOrder = new DS_orders();

                DataTable dt_orders = new PostOrderController().GetOrders(Jobj["load_inquiry_no"].ToString());
                //DataTable dt_Cancelorders = new PostOrderController().GetCancelOrdersRequestByInq(Jobj["load_inquiry_no"].ToString());
                //if (dt_Cancelorders != null)
                //{
                //    return BLGeneralUtil.return_ajax_string("0", "Alredy Requested For Cancellation");
                //}

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                try
                {

                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "CN", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    DS_orders.order_cancellation_detailsRow tr_order_cancel = dsOrder.order_cancellation_details.Neworder_cancellation_detailsRow();

                    dsOrder.EnforceConstraints = false;
                    tr_order_cancel.cancellation_id = logid;
                    tr_order_cancel.load_inquiry_no = Jobj["load_inquiry_no"].ToString();
                    tr_order_cancel.shipper_id = Jobj["shipper_id"].ToString();
                    tr_order_cancel.Reason = Jobj["Reason"].ToString();
                    tr_order_cancel.status = Constant.FLAG_Y;

                    dsOrder.order_cancellation_details.Addorder_cancellation_detailsRow(tr_order_cancel);
                    dsOrder.order_cancellation_details[0].AcceptChanges();
                    dsOrder.order_cancellation_details[0].SetAdded();
                    dsOrder.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dsOrder.order_cancellation_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    #region Update Date In Orders Table

                    if (dt_orders != null)
                    {
                        dsOrder.orders.ImportRow(dt_orders.Rows[0]);
                        dsOrder.orders[0].status = Constant.ORDER_CANCEL_REQUESTED;
                        dsOrder.orders[0].IsCancel = Constant.FLAG_N;

                        objBLReturnObject = master.UpdateTables(dsOrder.orders, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", " Order details Not found");
                    }
                    #endregion

                    #region Update Table post_load_inquiry  details for Cancel Order

                    DS_Post_load_enquiry ds_postload = new DS_Post_load_enquiry();
                    if (dt_orders != null)
                    {
                        ds_postload.EnforceConstraints = false;
                        ds_postload.post_load_inquiry.ImportRow(dt_orders.Rows[0]);

                        ds_postload.post_load_inquiry[0].status = Constant.ORDER_CANCEL_REQUESTED;

                        ds_postload.EnforceConstraints = true;
                        objBLReturnObject = master.UpdateTables(ds_postload.post_load_inquiry, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                    }
                    #endregion

                    string MsgMailbody = "";
                    string shippername = new PostOrderController().GetUserdetailsByID(Jobj["shipper_id"].ToString());



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
                    newmsg = " Order ID: " + Jobj["load_inquiry_no"].ToString() + "  <br/>   <b> UserName : </b>" + shippername + " <b> Reason : </b>" + Jobj["Reason"].ToString();
                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    bl = objemail.SendMail(sendto, newmsg, "Request For cancellation Order by " + shippername + " Order ID :" + Jobj["load_inquiry_no"].ToString(), ref msg, fromemail, frompword, "TruKKer Technologies");
                    if (!bl)
                    {
                        ServerLog.Log("Error Sending cancellation Email To Admin");
                        // return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    if (dt_orders != null)
                        MsgMailbody = " Your order from  " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + " cancellation request made successfully On " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + Jobj["load_inquiry_no"].ToString();

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderCancellationEmail(new PostOrderController().GetEmailByID(Jobj["shipper_id"].ToString()), " Order cancellation Request for (Order ID: " + Jobj["load_inquiry_no"].ToString() + ")", shippername, MsgMailbody, Jobj["load_inquiry_no"].ToString()));
                    if (result["status"].ToString() == "0")
                    {
                        ServerLog.Log("Error Sending cancellation Email To User");
                    }

                    msg = "";

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;


                    return BLGeneralUtil.return_ajax_string("1", " Order Cancellation Request Sent ");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }

                //                return BLGeneralUtil.return_ajax_string("1", " Order cancelation request send Successfully for Order ID :" + Jobj["load_inquiry_no"].ToString());
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }


        }


        public string GetShippingTimeById(string sizetypecode)
        {
            string query = "";
            try
            {
                DataTable dt_shi_id = new DataTable();
                if (sizetypecode == "SZ0000")
                    query = "select * from Shippingtime_mst ";
                else
                    query = "select * from Shippingtime_mst where SizeType_code= '0'";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_shi_id = ds.Tables[0];
                }
                if (dt_shi_id != null && dt_shi_id.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_shi_id));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data Found");
            }
            catch (Exception ex)
            {
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public string GetPackingMaterialDetailsBySizeType(string sizetype, string ratetypeflag)
        {
            string query = "";
            try
            {
                DataTable dt_shi_id = new DataTable();
                if (sizetype != "")
                    query = "  SELECT * FROM PackingMaterialMst join  packingMaterialdetails on packingMaterialdetails.packingmaterial_no = PackingMaterialMst.PackingMaterial_no where rate_type_flag='" + ratetypeflag + "' and  SizeTypecode = '" + sizetype + "' and packingMaterialdetails.quantity > 0";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_shi_id = ds.Tables[0];
                }
                if (dt_shi_id != null && dt_shi_id.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_shi_id));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data Found");
            }
            catch (Exception ex)
            {
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public string GetCancellationReasonsDetails()
        {
            string query = "";
            try
            {
                DataTable dt_shi_id = new DataTable();
                query = " select * from cancellation_reasons_mst ";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dt_shi_id = ds.Tables[0];
                }
                if (dt_shi_id != null && dt_shi_id.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_shi_id));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data Found");
            }
            catch (Exception ex)
            {
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        public string Getuserbillingaddress(string shipperid)
        {
            DataTable dt = GetuserbillingaddressbyId(shipperid);
            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No details Found");

        }

        public DataTable GetuserbillingaddressbyId(string shipperid)
        {
            String query = "SELECT * FROM UserBillingAdd_mst where shipper_id  = @userid  order by created_date desc";
            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, shipperid));
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

        //public Byte SaveCouponUserHistory(ref IDbCommand command, ref  String Message,DataTable dt)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    DS_Shipper dS_shipper = new DS_Shipper();
        //    dS_shipper.EnforceConstraints = false;
        //    string sr_no = ""; string message = "";
        //    DS_Shipper.UserBillingAdd_mstRow row = dS_shipper.UserBillingAdd_mst.NewUserBillingAdd_mstRow();

        //    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();

        //    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "UB", "", "", ref sr_no, ref message)) // New Driver Notification ID
        //    {
        //        ServerLog.Log(message);
        //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return 2;
        //    }
        //    row.billing_srno = sr_no;
        //    row.billing_name = dt.Rows[0]["billing_name"].ToString();
        //    row.shipper_id = dt.Rows[0]["shipper_id"].ToString();
        //    row.billing_add = dt.Rows[0]["billing_add"].ToString();
        //    row.active_flag = Constant.FLAG_Y;
        //    row.source_full_add = dt.Rows[0]["source_full_add"].ToString();
        //    row.destination_full_add = dt.Rows[0]["destination_full_add"].ToString();
        //    row.created_by = dt.Rows[0]["created_by"].ToString();
        //    row.created_date = System.DateTime.UtcNow;

        //    dS_shipper.UserBillingAdd_mst.AddUserBillingAdd_mstRow(row);

        //    try
        //    {
        //        dS_shipper.EnforceConstraints = true;
        //    }
        //    catch (ConstraintException ce)
        //    {
        //        Message = ce.Message;
        //        ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
        //        return 2;
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = ex.Message;
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        return 2;
        //    }

        //    objBLReturnObject = master.UpdateTables(dS_shipper.UserBillingAdd_mst, ref DBCommand);
        //    if (objBLReturnObject.ExecutionStatus != 1)
        //    {
        //        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return 2;
        //    }
        //    else
        //    {
        //        Message = "CouponUserHistory data inserted successfully.";
        //        return 1;
        //    }
        //}

        public DataTable GetCouponCodeByid(string couponid)
        {
            String query = "SELECT * FROM CouponMst where coupon_id  = @couponid ";
            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("couponid", DbType.String, couponid));
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

        [HttpPost]
        public string SaveCoupanDetails([FromBody]JObject jobj)
        {
            Master master = new Master();
            DS_Shipper dS_shipper = new DS_Shipper();
            string Msg = ""; string DocNo = "";
            // DS_Shipper.CouponMstRow row = dS_shipper.CouponMst.NewCouponMstRow();
            string message = ""; DataTable dtcpndtl = new DataTable();
            Document objdoc = new Document();
            List<CouponMst> objcoupan = new List<CouponMst>();
            DataTable dtCoupon = new DataTable();

            if (jobj["Coupon"] != null)
            {
                objcoupan = jobj["Coupon"].ToObject<List<CouponMst>>();
                dtCoupon = BLGeneralUtil.ToDataTable(objcoupan);
                dtCoupon = BLGeneralUtil.CheckDateTime(dtCoupon);
            }

            try
            {

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (objcoupan[0].coupon_id == null || objcoupan[0].coupon_id == "")
                {
                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "CM", "", "", ref DocNo, ref message)) // New Driver Notification ID
                    {
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }

                    //  dtCoupon.Rows[0]["start_date"] = Convert.ToDateTime(dtCoupon.Rows[0]["start_date"].ToString());
                    if (dtCoupon.Rows[0]["end_date"].ToString() == "")
                        dtCoupon.Rows[0]["end_date"] = DBNull.Value;
                    //else
                    //    dtCoupon.Rows[0]["end_date"] = dtCoupon.Rows[0]["end_date"].ToString();


                    dS_shipper.EnforceConstraints = false;
                    dS_shipper.CouponMst.ImportRow(dtCoupon.Rows[0]);
                    dS_shipper.CouponMst[0].coupon_id = DocNo;

                    if (objcoupan[0].coupon_type == "F")
                    {
                        dS_shipper.CouponMst[0].flat_discount = Convert.ToDecimal(objcoupan[0].discount);
                        dS_shipper.CouponMst[0].percentage_discount = 0;
                    }
                    else
                    {
                        dS_shipper.CouponMst[0].flat_discount = 0;
                        dS_shipper.CouponMst[0].percentage_discount = Convert.ToDecimal(objcoupan[0].discount);
                    }

                    if (objcoupan[0].user_once_only == Constant.FLAG_Y)
                        dS_shipper.CouponMst[0].user_once_only = Constant.FLAG_Y;
                    else
                        dS_shipper.CouponMst[0].user_once_only = Constant.FLAG_N;

                    dS_shipper.CouponMst[0].each_user_once_at_time = objcoupan[0].each_user_once_at_time;


                    dS_shipper.CouponMst[0].IsActive = Constant.FLAG_Y;
                    dS_shipper.CouponMst[0].created_by = "ADMIN";
                    dS_shipper.CouponMst[0].created_date = System.DateTime.UtcNow;
                    dS_shipper.CouponMst[0].created_host = "ADMIN";
                    dS_shipper.CouponMst[0].device_id = objcoupan[0].device_id;
                    dS_shipper.CouponMst[0].device_type = objcoupan[0].device_type;

                }
                else
                {
                    dtcpndtl = GetCouponCodeByid(objcoupan[0].coupon_id);
                    if (dtCoupon.Rows[0]["end_date"].ToString() == "")
                    {
                        dtcpndtl.Rows[0]["end_date"] = DBNull.Value;
                    }
                    dS_shipper.EnforceConstraints = false;
                    dS_shipper.CouponMst.ImportRow(dtcpndtl.Rows[0]);


                    dS_shipper.CouponMst[0].order_type_flag = objcoupan[0].order_type_flag;
                    dS_shipper.CouponMst[0].rate_type_flag = objcoupan[0].rate_type_flag;
                    dS_shipper.CouponMst[0].SizeTypeCode = objcoupan[0].SizeTypeCode;
                    dS_shipper.CouponMst[0].coupon_desc = objcoupan[0].coupon_desc;
                    dS_shipper.CouponMst[0].start_date = Convert.ToDateTime(dtCoupon.Rows[0]["start_date"].ToString());
                    if (dtCoupon.Rows[0]["end_date"].ToString() == "")
                    {
                        dtCoupon.Rows[0]["end_date"] = DBNull.Value;
                    }
                    else
                        dS_shipper.CouponMst[0].end_date = Convert.ToDateTime(dtCoupon.Rows[0]["end_date"].ToString());

                    dS_shipper.CouponMst[0].coupon_type = objcoupan[0].coupon_type;

                    if (objcoupan[0].coupon_type == "F")
                    {
                        dS_shipper.CouponMst[0].flat_discount = Convert.ToDecimal(objcoupan[0].discount);
                        dS_shipper.CouponMst[0].percentage_discount = 0;
                    }
                    else
                    {
                        dS_shipper.CouponMst[0].flat_discount = 0;
                        dS_shipper.CouponMst[0].percentage_discount = Convert.ToDecimal(objcoupan[0].discount);
                    }

                    dS_shipper.CouponMst[0].first_user_only = Constant.FLAG_Y;
                    dS_shipper.CouponMst[0].each_user_once_at_time = objcoupan[0].each_user_once_at_time;
                    if (objcoupan[0].user_once_only == Constant.FLAG_Y)
                        dS_shipper.CouponMst[0].user_once_only = Constant.FLAG_Y;
                    else
                        dS_shipper.CouponMst[0].user_once_only = Constant.FLAG_N;

                    dS_shipper.CouponMst[0].modified_by = "ADMIN";
                    dS_shipper.CouponMst[0].modified_date = System.DateTime.UtcNow;
                    dS_shipper.CouponMst[0].modified_host = "ADMIN";
                    dS_shipper.CouponMst[0].modified_device_id = objcoupan[0].device_id;
                    dS_shipper.CouponMst[0].modified_device_type = objcoupan[0].device_type;
                }

                dS_shipper.CouponMst.Rows[0].AcceptChanges();
                dS_shipper.CouponMst.Rows[0].SetAdded();



                //row.coupon_code = objcoupan[0].coupon_code;
                //row.rate_type_flag = objcoupan[0].rate_type_flag;
                //row.coupon_desc = objcoupan[0].coupon_desc;
                //row.start_date = System.DateTime.UtcNow;
                //if (objcoupan[0].end_date == null || objcoupan[0].end_date == "")
                //    row.end_date = System.DateTime.UtcNow.AddDays(10);
                //else
                //    row.end_date = Convert.ToDateTime(objcoupan[0].end_date);

                //row.coupon_type = objcoupan[0].coupon_type;

                //if (objcoupan[0].coupon_type == "F")
                //{
                //    row.flat_discount = Convert.ToDecimal(objcoupan[0].discount);
                //    row.percentage_discount = 0;
                //}
                //else
                //{
                //    row.flat_discount = 0;
                //    row.percentage_discount = Convert.ToDecimal(objcoupan[0].discount);
                //}

                //row.first_user_only = Constant.FLAG_Y;
                //row.each_user_once_at_time = Constant.FLAG_Y;
                //if (objcoupan[0].user_once_only == Constant.FLAG_Y)
                //    row.user_once_only = Constant.FLAG_Y;
                //else
                //    row.user_once_only = Constant.FLAG_N;

                //row.IsActive = Constant.FLAG_Y;
                //row.created_by = "ADMIN";
                //row.created_date = System.DateTime.UtcNow;
                //row.created_host = "ADMIN";
                //row.device_id = objcoupan[0].device_id;
                //row.device_type = objcoupan[0].device_type;

                //dS_shipper.CouponMst.AddCouponMstRow(row);

                //DBConnection.Open();
                //DBCommand.Transaction = DBConnection.BeginTransaction();

                //try
                //{
                //    dS_shipper.EnforceConstraints = true;
            }
            catch (ConstraintException ce)
            {
                ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ce.Message);
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

            objBLReturnObject = master.UpdateTables(dS_shipper.CouponMst, ref DBCommand);
            if (objBLReturnObject.ExecutionStatus != 1)
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage.ToString());
            }


            DBCommand.Transaction.Commit();
            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

            return BLGeneralUtil.return_ajax_string("1", "Coupan Generated sucessfully");
        }

        [HttpGet]
        public string GetCouponDetails()
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT CouponMst.*,sizetypemst.SizeTypeDesc FROM CouponMst 
                                    left join sizetypemst on sizetypemst.SizeTypeCode=CouponMst.SizeTypeCode 
                                    ORDER BY created_date DESC ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "CouponMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(ds.Tables[0]));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data Found");
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpPost]
        public string GetShipperOrdersDetails([FromBody]JObject Jobj)
        {

            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = ""; string sourcecity = "";
            string destinationcity = ""; string owner_id = ""; string username = ""; string order_type_flag = "";
            objOrder = Jobj["shipper_order"].ToObject<List<orders>>();
            if (Jobj["shipper_order"] != null)
            {
                status = objOrder[0].status;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
                sourcecity = objOrder[0].inquiry_source_city;
                destinationcity = objOrder[0].inquiry_destination_city;
                owner_id = objOrder[0].owner_id;
                username = objOrder[0].username;
                order_type_flag = objOrder[0].order_type;
            }

            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                StringBuilder SQLSelect = new StringBuilder();

                SQLSelect.Append(@"select statusFinal as status,* from ( select res_inner.Total_PT_charge,res_inner.Total_PT_Discount,res_inner.PT_SizeTypeCode,res_inner.Total_CL_Charge,res_inner.Total_CL_Discount,
                                    res_inner.CL_SizeTypeCode,res_inner.Total_PEST_Charge,res_inner.Total_PEST_Discount,res_inner.PEST_SizeTypeCode,driver_mst.mobile_no,driver_mst.driver_photo,driver_mst.Name as drivername,
                                                                                 quote_hdr.GeneratedPdfLink as cbm_GeneratedPdfLink,truckdetails.*,quote_hdr.total_cbm,quote_hdr.StatusFlag,status1 as statusFinal,
                                                                                 feedback_mst.star_rating as feedback_rating,feedback_mst.feedback as feedback_msg,
                                (SELECT   top(1)  status  FROM  order_driver_truck_details  WHERE (load_inquiry_no =lstorder.load_inquiry_no) ORDER BY CAST(status AS int) desc ) as final_status,lstorder.* from (
                                          select distinct CASE WHEN order_cancellation_details.cancellation_id IS not NULL THEN '25' 
                                       --WHEN order_reschedule_req_details.RescheduleReq_id IS not NULL and order_reschedule_req_details.active_flag='N' THEN '26' 
                                        else result.status
                                          END AS status1,order_driver_truck_details_summary.NoOfAssignedDriver, order_driver_truck_details_summary.NoOfAssignedTruck,  
                                          order_driver_truck_details_summary.NoOfAssignedHandiman, order_driver_truck_details_summary.NoOfAssignedLabour,  result.*,
                                          (SELECT   top(1)  truck_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_truck_id,
                                          (SELECT   top(1)  driver_id  FROM  order_driver_truck_details  WHERE (load_inquiry_no =result.load_inquiry_no)) as driver_id
                                          from 
                                          ( 
                                        select    SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id1' OrderKey  
                                        from orders    
                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode  
                                        join user_mst on user_mst.unique_id =  orders.shipper_id
                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
                                        Where 1=1 and orders.active_flag  = 'Y' and orders.IsCancel='N'
                                        and orders.shippingdatetime>getdate()  
                                        UNION ALL 
                                        select    SizeTypeMatrix.CBM_Max,user_mst.user_id as shipper_mobileno,user_mst.first_name AS username, SizeTypeMst.SizeTypeDesc, orders.*,'id2' OrderKey from orders  
                                        JOIN SizeTypeMst ON SizeTypeMst.SizeTypeCode = orders.sizetypecode 
                                        join user_mst on user_mst.unique_id =  orders.shipper_id
                                        join SizeTypeMatrix on SizeTypeMatrix.sizetypecode=  orders.sizetypecode and SizeTypeMatrix.rate_type_flag=orders.rate_type_flag
                                        Where 1=1 and orders.active_flag  = 'Y' and orders.IsCancel='N'
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
                                       -- LEFT OUTER JOIN order_reschedule_req_details ON result.load_inquiry_no = order_reschedule_req_details.load_inquiry_no 
                                ) as lstorder  
                                left join (
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
                                where  1=1  and tblfinal.load_inquiry_no not in ( select load_inquiry_no from orders where order_id like '%LO%' ) ");

                if (status != "")
                {
                    SQLSelect.Append("and tblfinal.statusFinal = '" + status + "'");
                    //if (status == "02" || status == "45")
                    //{
                    //    SQLSelect.Append(" and (case ISNULL(load_inquiry_no,'') when '' then statusfinal else '25' end)  = '" + status + "' ");
                    //}//   query1 += " and status = '" + status + "' ";
                    //else
                    //    SQLSelect.Append(" and (case ISNULL(load_inquiry_no,'') when '' then statusfinal else '25' end) not in (02,45) ");
                }
                if (owner_id != "")
                {
                    SQLSelect.Append(" and shipper_id = '" + owner_id + "'  ");
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
                {
                    SQLSelect.Append(" and tblfinal.inquiry_destination_addr like '%" + destinationcity + "%' ");
                }

                SQLSelect.Append("  order by OrderKey,shippingdatetime asc");

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

        public string SendOrderRescheduleRequestByUser([FromBody]JObject Jobj)
        {
            try
            {
                List<orders> objOrder = new List<orders>();
                DS_orders dsord = new DS_orders();

                if (Jobj["order"] != null)
                {
                    objOrder = Jobj["order"].ToObject<List<orders>>();
                }

                Master master = new Master(); Document objDocument = new Document();
                string logid = ""; string msg = "";
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_orders dsOrder = new DS_orders();

                DataTable dt_orders = new PostOrderController().GetOrders(objOrder[0].load_inquiry_no);
                DataTable dt_ReRqorders = new PostOrderController().GetRescheduleRequestDetailsByInq(objOrder[0].load_inquiry_no);
                if (dt_ReRqorders != null)
                {
                    return BLGeneralUtil.return_ajax_string("0", " Alredy Requested For reschedule ");
                }

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                try
                {
                    if (!objDocument.W_GetNextDocumentNo(ref DBCommand, "RR", "", "", ref logid, ref msg))
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }


                    DateTime rescheduleShippingDateTime = Convert.ToDateTime(BLGeneralUtil.ConvertToDateTime(objOrder[0].load_inquiry_shipping_date, "dd/mm/yyyy").ToShortDateString() + " " + TimeSpan.Parse(objOrder[0].load_inquiry_shipping_time));

                    DS_orders.order_reschedule_req_detailsRow tr = dsOrder.order_reschedule_req_details.Neworder_reschedule_req_detailsRow();

                    dsOrder.EnforceConstraints = false;
                    tr.RescheduleReq_id = logid;
                    tr.load_inquiry_no = objOrder[0].load_inquiry_no;
                    tr.shipper_id = objOrder[0].shipper_id;
                    tr.active_flag = Constant.Flag_No;
                    tr.RescheduleShippingDateTime = rescheduleShippingDateTime;
                    tr.shippingDateTime = Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString());
                    tr.created_by = objOrder[0].created_by;
                    tr.created_host = objOrder[0].created_host;
                    tr.device_id = objOrder[0].device_id;
                    tr.device_type = objOrder[0].device_type;
                    tr.created_date = System.DateTime.UtcNow;

                    dsOrder.order_reschedule_req_details.Addorder_reschedule_req_detailsRow(tr);
                    dsOrder.order_reschedule_req_details[0].AcceptChanges();
                    dsOrder.order_reschedule_req_details[0].SetAdded();
                    dsOrder.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(dsOrder.order_reschedule_req_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    dsOrder.EnforceConstraints = false;
                    dsOrder.orders.ImportRow(dt_orders.Rows[0]);
                    dsOrder.orders[0].status = Constant.ORDER_RESCHEDULE_REQUESTED;
                    dsOrder.EnforceConstraints = true;
                    objBLReturnObject = master.UpdateTables(dsOrder.orders, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }


                  

                    string MsgMailbody = "";
                    string shippername = new PostOrderController().GetUserdetailsByID(objOrder[0].shipper_id);
                    string shipperEmail = new PostOrderController().GetEmailByID(objOrder[0].shipper_id);


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
                    newmsg = " Order ID: " + objOrder[0].load_inquiry_no + "  <br/>   <b> UserName : </b>" + shippername + " <br/>   <b> Shipping Date : </b>" + Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy h:mm tt ") + " <br/>   <b> reschedule Requested Date : </b>" + rescheduleShippingDateTime.ToString("dd MMM yyyy h:mm tt ");
                    dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", msg);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;

                    sendto = dt_param.Rows[0]["param_value"].ToString().Trim();
                    bl = objemail.SendMail(sendto, newmsg, " Order Rescheduling Request Order by " + shippername + " Order ID :" + objOrder[0].load_inquiry_no, ref msg, fromemail, frompword, "TruKKer Technologies");
                    if (!bl)
                    {
                        ServerLog.Log("Error Sending cancellation Email To Admin");
                    }

                    string strTitle = ConfigurationManager.AppSettings["SUBRESCH_REQ"];

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().OrderRescheduleEmail(shipperEmail, strTitle + " for (Order ID: " + objOrder[0].load_inquiry_no + ")", shippername, rescheduleShippingDateTime, objOrder[0].load_inquiry_no));
                    if (result["status"].ToString() == "0")
                    {
                        ServerLog.Log("Error Sending reschedule Email To User");
                    }

                    msg = "";
                    
                    return BLGeneralUtil.return_ajax_string("1", " Order reschedule Request Sent ");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
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
    }
}

