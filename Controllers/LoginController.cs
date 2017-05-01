using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Configuration;
using System.Data;
using Newtonsoft.Json.Linq;
using trukkerUAE.Models;
using BLL.Utilities;
using System.Text;
using trukkerUAE.XSD;
using BLL.Master;
using trukkerUAE.Classes;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Mail;
using trukkerUAE.BLL.Master;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Start.Net.Constants;
using Start.Net;

namespace trukkerUAE.Controllers
{
    //[EnableCors(origins:"http://trukker.co",headers:"*",methods:"*")]
    public class LoginController : ServerBase
    {
        //"Login": { "user_id": "9409460478","password": "asdf","load_inquiry_no": "","created_host":"","device_id": "352359072005519" }
        [HttpPost]
        public string CheckIn([FromBody]JObject Parameter)
        {
            Master master = new Master();
            Document objdoc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DataTable dtuserTemp = new DataTable();
            DataTable dtuser = new DataTable();
            try
            {
                var data = Parameter["Login"].ToObject<LoginCheck>();
                if (data.user_id == "")
                    return (BLGeneralUtil.return_ajax_string("0", "Please Enter User ID"));
                if (data.password == "")
                    return (BLGeneralUtil.return_ajax_string("0", "Please Enter Password"));

                string userid = data.user_id;
                string password = data.password;
                string uniqueid = "";

                //string password = BLGeneralUtil.ToSHA256(data.password);
                String query1 = " SELECT u.*,first_name as FullName,d.driver_photo FROM user_mst as u left join dbo.driver_mst as d on d.driver_id=u.unique_id Where  user_id = '" + userid + "' and password = CAST('" + password + "' AS VARCHAR(100))  COLLATE SQL_Latin1_General_CP1_CS_AS " +
                                " and unique_id not in (select unique_id from user_mst where Isblocked='Y') and email_id not in (select email_id from user_mst where Isblocked='Y') ";
                //String query1 = "SELECT user_mst.* FROM user_mst  Where  user_id = '" + userid + "' and password = '" + password + "' ";
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtuser = ds.Tables[0];
                }

                if (dtuser != null && dtuser.Rows.Count > 0)
                {
                    DataTable dtdeviceIds = new DataTable();
                    if (dtuser.Rows[0]["role_id"].ToString() == "DR")
                    {
                        dtdeviceIds = GetDriverdeviceIDByDriverID(dtuser.Rows[0]["unique_id"].ToString());
                    }

                    uniqueid = dtuser.Rows[0]["unique_id"].ToString();
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    if (dtuser.Rows[0]["user_status_flag"].ToString() == "N")
                    {
                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 1;

                        return (BLGeneralUtil.return_ajax_string("2", "Please Active User "));
                    }

                    if (dtuser.Rows[0]["role_id"].ToString() == "DR")
                    {
                        string driverid = dtuser.Rows[0]["unique_id"].ToString();
                        if (dtdeviceIds != null)
                        {
                            DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                            if (dr.Length > 0)
                            {
                                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();

                                if (dr[0].ItemArray[3].ToString() != data.device_id)
                                {
                                    //   return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                                    ds_driver.EnforceConstraints = false;
                                    ds_driver.driver_login_detail.ImportRow(dr[0]);
                                    ds_driver.driver_login_detail.Rows[0]["Is_logedin"] = "N";
                                    ds_driver.EnforceConstraints = true;

                                    objBLReturnObject = master.UpdateTables(ds_driver.driver_login_detail, ref DBCommand);
                                    if (objBLReturnObject.ExecutionStatus == 2)
                                    {
                                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        objBLReturnObject.ExecutionStatus = 2;
                                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                                    }
                                    SaveDriverLogindetails(driverid, data);
                                }
                            }
                            else
                            {
                                SaveDriverLogindetails(driverid, data);
                            }
                        }
                        else
                        {
                            SaveDriverLogindetails(driverid, data);
                        }
                        string truckid = ""; string loadinquiry = string.Empty;
                        DataTable dt = new driverController().GetTruckIdBy(driverid);
                        if (dt != null)
                            truckid = dt.Rows[0]["truck_id"].ToString();

                        DataTable dt_ordernumber = new driverController().GetOngoingOrderNumber(driverid);
                        if (dt_ordernumber != null)
                            loadinquiry = dt_ordernumber.Rows[0]["load_inquiry_no"].ToString();

                        dtuser.Columns.Add("truck_id");
                        dtuser.Rows[0]["truck_id"] = truckid;
                        dtuser.Columns.Add("load_inquiry_no");
                        dtuser.Rows[0]["load_inquiry_no"] = loadinquiry;

                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                }
                else
                {
                    return (BLGeneralUtil.return_ajax_string("0", "Invalid User name or password "));
                }




                if (data.load_inquiry_no.Trim() != "")
                {
                    string Message = "";
                    DataTable dtPostOrder = new PostOrderController().GetLoadInquiryBySizetypeId(data.load_inquiry_no);
                    // DataTable dtPostOrder = new TruckerMaster().GetPost_Load_Inquiry(ref DBDataAdpterObject, data.load_inquiry_no, ref Message);
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    try
                    {
                        DS_Post_load_enquiry ds_post_load = new DS_Post_load_enquiry();
                        ds_post_load.EnforceConstraints = false;
                        ds_post_load.post_load_inquiry.ImportRow(dtPostOrder.Rows[0]);
                        ds_post_load.post_load_inquiry[0].shippingdatetime = Convert.ToDateTime(Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_date"].ToString()).Date.ToShortDateString() + ' ' + Convert.ToDateTime(dtPostOrder.Rows[0]["load_inquiry_shipping_time"].ToString()).ToShortTimeString());
                        ds_post_load.post_load_inquiry[0].shipper_id = uniqueid;
                        ds_post_load.EnforceConstraints = true;
                        objBLReturnObject = master.UpdateTables(ds_post_load.post_load_inquiry, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }

                        String SizeTypeCode = dtPostOrder.Rows[0]["SizeTypeCode"].ToString();
                        String goods_type_flag = dtPostOrder.Rows[0]["goods_type_flag"].ToString();
                        String rate_type_flag = "B";
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


                        DataTable dtSizeTypeMst = new TruckerMaster().CalculateRate(null, SizeTypeCode, System.DateTime.UtcNow, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
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
                        dtuserTemp.Rows[0]["unique_id"] = dtuser.Rows[0]["unique_id"].ToString();
                        dtuserTemp.Columns.Add("user_id");
                        dtuserTemp.Rows[0]["user_id"] = dtuser.Rows[0]["user_id"].ToString();
                        dtuserTemp.Columns.Add("shipper_id");
                        dtuserTemp.Rows[0]["shipper_id"] = uniqueid;


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

                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtuserTemp)));
                    //return (BLGeneralUtil.return_ajax_string("0", "Invalid User name or password "));
                }
                else
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtuser)));
                        //                        return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(ds.Tables[0])));
                        //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                        //return "1";
                    }
                    else
                        return (BLGeneralUtil.return_ajax_string("0", "Invalid User name or password "));
                }


            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                return ex.Message.ToString();
            }
        }

        public DataTable Getdevicedetails(string deviceid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from driver_login_detail where device_id = '" + deviceid + "' and Is_logedin='Y' ";
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

        private string SaveDriverLogindetails(string driver_id, LoginCheck data)
        {
            DataTable dtdriverbydevice = Getdevicedetails(data.device_id);

            try
            {

                Master master = new Master();
                Document objdoc = new Document();
                BLReturnObject objBLReturnObject = new BLReturnObject();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                string tempDriverid = ""; string message = "";
                DS_driver_order_notifications.driver_login_detailRow tr = ds_driver.driver_login_detail.Newdriver_login_detailRow();


                if (dtdriverbydevice != null)
                {
                    for (int i = 0; i < dtdriverbydevice.Rows.Count; i++)
                    {
                        ds_driver.EnforceConstraints = false;
                        ds_driver.driver_login_detail.ImportRow(dtdriverbydevice.Rows[i]);
                        ds_driver.driver_login_detail[i].Is_logedin = "N";
                        ds_driver.EnforceConstraints = true;
                    }

                    objBLReturnObject = master.UpdateTables(ds_driver.driver_login_detail, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                }

                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "BL", "", "", ref tempDriverid, ref message)) // New Driver Notification ID
                {
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                ds_driver.EnforceConstraints = false;
                tr.loginid = tempDriverid;
                tr.driver_id = driver_id;
                tr.Is_logedin = "Y";
                tr.device_id = data.device_id;
                tr.created_host = data.device_id;
                tr.created_date = System.DateTime.UtcNow;
                ds_driver.EnforceConstraints = true;

                ds_driver.driver_login_detail.Adddriver_login_detailRow(tr);
                ds_driver.driver_login_detail.Rows[0].AcceptChanges();
                ds_driver.driver_login_detail.Rows[0].SetAdded();


                objBLReturnObject = master.UpdateTables(ds_driver.driver_login_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                return (BLGeneralUtil.return_ajax_string("1", "valid "));
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.ToString() + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return (BLGeneralUtil.return_ajax_string("0", "Invalid  "));
            }

        }

        //by sandip 11/5
        // Service call when user request for forgot passwod
        // first check that user id is valid OR not and Then after send OTP to mobile Number
        //   "User": [{ "user_id": "9979870937" }]
        [HttpPost]
        public String ResendOTP([FromBody]JObject jobj)
        {
            List<User> tuser = new List<User>();

            if (jobj["User"] != null)
                tuser = jobj["User"].ToObject<List<User>>();

            if (tuser[0].user_id != "" && tuser[0].user_id != null)
            {
                //if (tuser[0].user_id.Length > 10 || tuser[0].user_id.Length < 10)
                //{
                //    return BLGeneralUtil.return_ajax_string("0", "Please Enter Valid Mobile Number");
                //}
                //else
                //{
                Random rand = new Random();
                int Otp = rand.Next(1111, 9999);
                try
                {
                    Master mst = new Master();
                    BLReturnObject objBLobj = new BLReturnObject();
                    DataTable dt_user = new DataTable();
                    DS_User ds_user = new DS_User();
                    string qury = "select * from user_mst where user_id= '" + tuser[0].user_id + "'";

                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.CommandText = qury;
                    DataSet ds = new DataSet();
                    DBDataAdpterObject.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {

                            ds_user.EnforceConstraints = false;
                            ds_user.user_mst.ImportRow(ds.Tables[0].Rows[0]);
                            ds_user.user_mst.Rows[0]["otp"] = Otp;
                            ds_user.user_mst.Rows[0].AcceptChanges();
                            ds_user.user_mst.Rows[0].SetModified();

                            ds_user.EnforceConstraints = true;

                            objBLobj = mst.UpdateTables(ds_user.user_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                            try
                            {
                                new EMail().SendOtpToUserMobileNoUAE("Verification Code is : " + Otp.ToString(), ds_user.user_mst.Rows[0]["user_id"].ToString());
                            }
                            catch (Exception ex)
                            {
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }
                        }
                        else
                        {
                            return BLGeneralUtil.return_ajax_string("0", "Invalid UserID");
                        }
                    }
                    else
                    {

                        return BLGeneralUtil.return_ajax_string("0", "This Mobile no. is not registrated");
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("1", "Verification Code Send Your Register Number Successfully ");

                }
                catch (Exception ex)
                {
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
                //}
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Send Mobile Number");
            }
        }

        [HttpPost]
        public string ValidateOTP([FromBody]JObject Parameter)
        {
            string roleid = ""; string userstatus = "";
            var objLogin = Parameter["User"].ToObject<User>();
            DataSet ds = new DataSet();
            try
            {
                string userid = objLogin.user_id;
                // string password = objLogin.password;
                if (objLogin.OTP.Trim() != null)
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Select * from user_mst Where user_id = @userid ");

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("userid", DbType.String, userid));
                    //     DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("pasword", DbType.String, password));
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();

                    DBDataAdpterObject.Fill(ds);
                    sb.Clear();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            string otp = "";
                            roleid = ds.Tables[0].Rows[0]["role_id"].ToString();
                            userstatus = ds.Tables[0].Rows[0]["user_status_flag"].ToString();
                            otp = ds.Tables[0].Rows[0]["OTP"].ToString();
                            if (otp == objLogin.OTP)
                            {
                                if (UpdateUserDetails(objLogin.user_id, ds.Tables[0].Rows[0]["password"].ToString()))
                                {
                                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(ds.Tables[0])));
                                }
                                else
                                {
                                    return (BLGeneralUtil.return_ajax_string("0", "Error while updating user status"));
                                }
                            }
                            else
                            {
                                return (BLGeneralUtil.return_ajax_string("0", "Invalid Verification Code "));
                            }
                        }
                        else
                        {
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return (BLGeneralUtil.return_ajax_string("0", "User details not found / userid / password is invalid"));
                        }
                    }
                    else
                    {
                        return (BLGeneralUtil.return_ajax_string("0", "User details not found / userid / password is invalid"));
                    }


                }
                else
                    return (BLGeneralUtil.return_ajax_string("0", "Please Enter Verification Code "));
            }
            catch (Exception ex)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                return ex.Message.ToString();
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        ///by sandip 7/4/2016 
        /// After Enter OTP by Deactive User Active using this Method 
        /// here Change Status of "user_status_flag"=A
        private bool UpdateUserDetails(string userid, string password)
        {
            DataTable dtuserdetails = new DataTable();
            DS_User ds_user = new DS_User();
            DS_User.user_mstRow dr = ds_user.user_mst.Newuser_mstRow();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            Master master = new Master();

            //get user details  for update using given userid and password 
            dtuserdetails = GetUserPassword("", userid, password);

            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();

            try
            {
                if (dtuserdetails != null && dtuserdetails.Rows.Count > 0)
                {
                    ds_user.EnforceConstraints = false;
                    ds_user.user_mst.ImportRow(dtuserdetails.Rows[0]);
                    ds_user.user_mst.Rows[0]["user_status_flag"] = "A";
                    ds_user.user_mst.Rows[0].AcceptChanges();
                    ds_user.user_mst.Rows[0].SetModified();

                    ds_user.EnforceConstraints = true;

                    objBLReturnObject = master.UpdateTables(ds_user.user_mst, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        return false;
                    }
                    else
                    {
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        //return BLGeneralUtil.return_ajax_string("1", SendReceiveJSon.GetJson(ds_user.user_mst));
                        return true;
                    }

                }
                else
                {
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //return (BLGeneralUtil.return_ajax_string("0", "Invalid User name or password or user not found "));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.ToString() + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
                // return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
            }

        }

        public DataTable GetUserPassword(string emailid, string moblno, string pwd)
        {
            try
            {
                DataTable dt_user = new DataTable();
                string qury = "select * from user_mst where (user_id= @emlid or email_id = @emlid or user_id=@mobln) and password = @pwdn ";
                if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("emlid", DbType.String, emailid));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("mobln", DbType.String, moblno));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("pwdn", DbType.String, pwd));
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

        //by sandip 11/5
        // Service call when user request for forgot passwod
        // first check that user id is valid OR not and Then after send OTP to mobile Number
        //   "User": [{ "user_id": "9979870937" }]
        [HttpPost]
        public String ValidateuserAndsendOTP([FromBody]JObject jobj)
        {
            List<User> tuser = new List<User>();

            if (jobj["User"] != null)
                tuser = jobj["User"].ToObject<List<User>>();

            if (tuser[0].user_id != "" && tuser[0].user_id != null)
            {
                //if (tuser[0].user_id.Length > 10 || tuser[0].user_id.Length < 10)
                //{
                //    return BLGeneralUtil.return_ajax_string("0", "Please Enter Valid Mobile Number");
                //}
                //else
                //{

                Random rand = new Random();
                int Otp = rand.Next(1111, 9999);
                try
                {
                    Master mst = new Master();
                    BLReturnObject objBLobj = new BLReturnObject();
                    DataTable dt_user = new DataTable();
                    DS_User ds_user = new DS_User();
                    string qury = "select * from user_mst where user_id= '" + tuser[0].user_id + "'";

                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.CommandText = qury;
                    DataSet ds = new DataSet();
                    DBDataAdpterObject.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            ds_user.EnforceConstraints = false;
                            ds_user.user_mst.ImportRow(ds.Tables[0].Rows[0]);
                            ds_user.user_mst.Rows[0]["otp"] = Otp;
                            ds_user.user_mst.Rows[0].AcceptChanges();
                            ds_user.user_mst.Rows[0].SetModified();
                            ds_user.EnforceConstraints = true;

                            objBLobj = mst.UpdateTables(ds_user.user_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                            try
                            {
                                new EMail().SendOtpToUserMobileNoUAE("Your Verification Code is: " + Otp.ToString(), ds_user.user_mst.Rows[0]["user_id"].ToString());
                            }
                            catch (Exception ex)
                            {
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                                return BLGeneralUtil.return_ajax_string("0", ex.Message);
                            }
                        }
                        else
                        {
                            return BLGeneralUtil.return_ajax_string("0", "Invalid UserID");
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", "This Mobile no. is not registrated");
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("1", "Verification Code Sent Your Register Number Successfully ");

                }
                catch (Exception ex)
                {
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    DBCommand.Transaction.Rollback();
                    ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }
                // }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Send Mobile Number");
            }
        }

        //by sandip 11/5
        // Update Password in user_mst table after enter
        //   "User": [{ "user_id": "9979870937" , "password": "asdf"}]
        [HttpPost]
        public String UpdateForgotpwd([FromBody]JObject jobj)
        {
            List<User> tuser = new List<User>();
            if (jobj["User"] != null)
                tuser = jobj["User"].ToObject<List<User>>();

            if (tuser[0].user_id == null || tuser[0].user_id.Trim() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Enter Valid Mobile Number");
            }
            else if (tuser[0].password == null || tuser[0].password.Trim() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please Provide Password");
            }
            //else if (tuser[0].user_id.Length < 10)
            //{
            //    return BLGeneralUtil.return_ajax_string("0", "Please Provide Valid Mobile Number");
            //}
            else
            {
                try
                {
                    Master mst = new Master();
                    BLReturnObject objBLobj = new BLReturnObject();
                    DataTable dt_user = new DataTable();
                    DS_User ds_user = new DS_User();
                    string qury = "select * from user_mst where  user_id= @moblno ";
                    if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("moblno", DbType.String, tuser[0].user_id));

                    DBDataAdpterObject.SelectCommand.CommandText = qury;
                    DataSet ds = new DataSet();
                    DBDataAdpterObject.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            ds_user.EnforceConstraints = false;
                            ds_user.user_mst.ImportRow(ds.Tables[0].Rows[0]);
                            ds_user.user_mst.Rows[0]["password"] = tuser[0].password;
                            ds_user.user_mst.Rows[0].AcceptChanges();
                            ds_user.user_mst.Rows[0].SetModified();

                            ds_user.EnforceConstraints = true;

                            objBLobj = mst.UpdateTables(ds_user.user_mst, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            }
                        }
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("0", "Invalid UserID");
                    }


                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;
                    return BLGeneralUtil.return_ajax_string("1", "Password Change Successfully ");


                }
                catch (Exception ex)
                {
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);

                }
            }

        }

        public DataTable GetDriverLoginDetails(string deviceid)
        {
            try
            {
                DataTable dtdriver = new DataTable();
                String query1 = "";
                query1 = " select * from driver_login_detail where device_id ='" + deviceid + "'";

                DBConnection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtdriver = ds.Tables[0];
                }
                DBConnection.Close();
                if (dtdriver != null && dtdriver.Rows.Count > 0)
                    return dtdriver;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable GetDriverdeviceIDByDriverID(string driverid)
        {
            try
            {
                DataTable dtdriver = new DataTable();
                String query1 = "";
                query1 = " select * from driver_login_detail where driver_id ='" + driverid + "'";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query1;
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtdriver = ds.Tables[0];
                }
                if (dtdriver != null && dtdriver.Rows.Count > 0)
                    return dtdriver;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Push Notification services

        [HttpPost]
        //public String RegisterDevice(String AppName, String RepId, String DeviceId, String TokenId, String DeviceInfo, String OS, String IMEINo)
        public String RegisterDevice([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("RegisterDevice(" + Convert.ToString(AppName) + ", " + Convert.ToString(UniqueId) + ", " + Convert.ToString(DeviceId) + ", " + Convert.ToString(TokenId) + ", " + Convert.ToString(DeviceInfo) + ", " + Convert.ToString(OS) + ", " + Convert.ToString(IMEINo) + ")");
                ServerLog.MgmtExceptionLog("RegisterDevice(" + Convert.ToString(jobj) + ")");
                String AppName = jobj["AppName"].ToString();
                String UniqueId = jobj["UniqueId"].ToString();
                String DeviceId = jobj["DeviceId"].ToString();
                String TokenId = jobj["TokenId"].ToString();
                String DeviceInfo = jobj["DeviceInfo"].ToString();
                String OS = jobj["OS"].ToString();
                String IMEINo = jobj["IMEINo"].ToString();

                if (DeviceId == null || DeviceId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied DeviceId.");
                }
                if (TokenId == null || TokenId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied TokenId.");
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.AddDeviceInfo(AppName, UniqueId, DeviceId, TokenId, DeviceInfo, OS, IMEINo, ref Message);
                    if (result == 1)
                        return BLGeneralUtil.return_ajax_string("1", Message);
                    else
                        return BLGeneralUtil.return_ajax_string("0", Message);
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog("RegisterDevice(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
            // return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String UnRegisterDevice(String AppName, String DeviceId)
        public String UnRegisterDevice([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("UnRegisterDevice(" + Convert.ToString(AppName) + ", " + Convert.ToString(DeviceId) + ")");
                ServerLog.MgmtExceptionLog("UnRegisterDevice(" + Convert.ToString(jobj) + ")");
                String AppName = jobj["AppName"].ToString();
                String DeviceId = jobj["DeviceId"].ToString();
                if (DeviceId == null || DeviceId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied DeviceId.");
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.RemoveDeviceInfo(AppName, DeviceId, ref Message);
                    if (result == 1)
                        return BLGeneralUtil.return_ajax_string("1", Message);
                    else
                        return BLGeneralUtil.return_ajax_string("0", Message);
                }
            }
            catch (Exception ex)
            {
                //ServerLog.MgmtExceptionLog("UnRegisterDevice(" + Convert.ToString(AppName) + ", " + Convert.ToString(DeviceId) + ")");
                ServerLog.MgmtExceptionLog("UnRegisterDevice(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
            // return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String UpdateUnreadCount(String AppName, String UniqueId, String DeviceId, int UnreadCount)
        public String UpdateUnreadCount([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("UpdateUnreadCount(" + Convert.ToString(AppName) + ", " + Convert.ToString(UniqueId) + ", " + Convert.ToString(DeviceId) + ", " + Convert.ToString(UnreadCount) + ")");
                ServerLog.MgmtExceptionLog("UpdateUnreadCount(" + Convert.ToString(jobj) + ")");
                String AppName = jobj["AppName"].ToString();
                String UniqueId = jobj["UniqueId"].ToString();
                String DeviceId = jobj["DeviceId"].ToString();
                int UnreadCount = Convert.ToInt32(jobj["UnreadCount"]);
                if (AppName == null || AppName.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied AppName.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied UniqueId.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (DeviceId == null || DeviceId.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied DeviceId.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (UnreadCount < 0)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied Valid UnreadCount.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.UpdateUnreadCount(AppName, UniqueId, DeviceId, UnreadCount, ref Message);
                    objResponseObjectInfo.Status = result;
                    objResponseObjectInfo.Message = Message;
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                //ServerLog.MgmtExceptionLog("UpdateUnreadCount(" + Convert.ToString(AppName) + ", " + Convert.ToString(UniqueId) + ", " + Convert.ToString(DeviceId) + ", " + Convert.ToString(UnreadCount) + ")");
                ServerLog.MgmtExceptionLog("UpdateUnreadCount(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String GetUnreadCount(String AppName, String UniqueId)
        public String GetUnreadCount([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("GetUnreadCount(" + Convert.ToString(AppName) + ", " + Convert.ToString(UniqueId) + ")");
                ServerLog.MgmtExceptionLog("GetUnreadCount(" + Convert.ToString(jobj) + ")");
                String AppName = jobj["AppName"].ToString();
                String UniqueId = jobj["UniqueId"].ToString();
                if (AppName == null || AppName.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied AppName.");
                    //objResponseObjectInfo.Status = Constant.Status_Fail;
                    //objResponseObjectInfo.Message = "Please supplied AppName.";
                    //objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied UniqueId ");
                    //objResponseObjectInfo.Status = Constant.Status_Fail;
                    //objResponseObjectInfo.Message = "Please supplied UniqueId.";
                    //objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    int UnreadCount = objMgmtMessaging.GetUnreadCount(AppName, UniqueId, ref Message);
                    objResponseObjectInfo.Message = Message;
                    if (UnreadCount < 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", "No data found");
                    }
                    else
                    {
                        return BLGeneralUtil.return_ajax_string("1", UnreadCount.ToString());
                        //objResponseObjectInfo.Status = Constant.Status_Success;
                        //objResponseObjectInfo.dt_ReturnedTables = null;
                        //objResponseObjectInfo.ObjRetArgs = new Object[1];
                        //objResponseObjectInfo.ObjRetArgs[0] = UnreadCount;
                    }
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                objResponseObjectInfo.ObjRetArgs = null;
                //ServerLog.MgmtExceptionLog("GetUnreadCount(" + Convert.ToString(AppName) + ", " + Convert.ToString(UniqueId) + ")");
                ServerLog.MgmtExceptionLog("GetUnreadCount(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String GetMessages(String UniqueId, String AppName, String LastMessageDateTime)
        public String GetMessages([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            int NoOfMessageInGetMessage = 20;
            try
            {
                if (ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage") != null && ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage").ToString().Trim() != String.Empty)
                {
                    NoOfMessageInGetMessage = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage"));
                    if (NoOfMessageInGetMessage <= 0)
                        NoOfMessageInGetMessage = 20;
                    if (NoOfMessageInGetMessage > 9999)
                        NoOfMessageInGetMessage = 9999;
                }
                else
                    NoOfMessageInGetMessage = 20;
            }
            catch
            {
                NoOfMessageInGetMessage = 20;
            }

            try
            {
                //ServerLog.MgmtExceptionLog("GetMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + "," + Convert.ToString(LastMessageDateTime) + ")");
                ServerLog.MgmtExceptionLog("GetMessages(" + Convert.ToString(jobj) + ")");
                String UniqueId = jobj["UniqueId"].ToString();
                String AppName = jobj["AppName"].ToString();
                String LastMessageDateTime = jobj["LastMessageDateTime"].ToString();
                if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Please supplied UniqueId.");
                    //objResponseObjectInfo.Status = Constant.Status_Fail;
                    //objResponseObjectInfo.Message = "Please supplied UniqueId.";
                    //objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    DateTime? _LastMessageDateTime = null;
                    if (LastMessageDateTime != null && LastMessageDateTime.Trim() != String.Empty)
                        _LastMessageDateTime = Convert.ToDateTime(LastMessageDateTime);

                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    DataTable dtMessages = objMgmtMessaging.GetMessages(UniqueId, AppName, NoOfMessageInGetMessage, _LastMessageDateTime, ref Message);
                    if (dtMessages == null || dtMessages.Rows.Count <= 0)
                    {
                        return BLGeneralUtil.return_ajax_string("0", " You have no notifications ");
                        //objResponseObjectInfo.Status = Constant.Status_Fail;
                        //objResponseObjectInfo.Message = Message;
                        //objResponseObjectInfo.dt_ReturnedTables = null;
                    }
                    else
                    {
                        DataTable dtMessageAttachment = null;
                        String MessageIds = String.Empty;
                        foreach (DataRow dr in dtMessages.Rows)
                        {
                            if (dr["IsAnyAttachment"].ToString() == Constant.FLAG_Y)
                                MessageIds += dr["MessageId"].ToString() + ",";
                        }
                        if (MessageIds.Length > 0)
                        {
                            MessageIds = MessageIds.Substring(0, MessageIds.Length - 1);
                            String ErrorMessage = String.Empty;
                            dtMessageAttachment = objMgmtMessaging.GetMessageAttachment(MessageIds, ref ErrorMessage);
                        }

                        if (dtMessages != null && dtMessages.Rows.Count > 0)
                            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtMessages));
                        else
                            return BLGeneralUtil.return_ajax_string("2", " You have no orders yet ");

                        //objResponseObjectInfo.Status = 1;
                        //objResponseObjectInfo.Message = Message;
                        //objResponseObjectInfo.ObjRetArgs = new Object[1];
                        //objResponseObjectInfo.ObjRetArgs[0] = dtMessages.Rows[0]["Date"].ToString();
                        //objResponseObjectInfo.dt_ReturnedTables = new dynamic[2];
                        //objResponseObjectInfo.dt_ReturnedTables[0] = dtMessages;
                        //objResponseObjectInfo.dt_ReturnedTables[1] = dtMessageAttachment;
                    }
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                //ServerLog.MgmtExceptionLog("GetMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + "," + Convert.ToString(LastMessageDateTime) + ")");
                ServerLog.MgmtExceptionLog("GetMessages(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String GetPastMessages(String UniqueId, String AppName, String LastMessageDateTime)
        public String GetPastMessages([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            int NoOfMessageInGetMessage = 20;
            try
            {
                if (ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage") != null && ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage").ToString().Trim() != String.Empty)
                {
                    NoOfMessageInGetMessage = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NoOfMessageInGetMessage"));
                    if (NoOfMessageInGetMessage <= 0)
                        NoOfMessageInGetMessage = 20;
                    if (NoOfMessageInGetMessage > 9999)
                        NoOfMessageInGetMessage = 9999;
                }
                else
                    NoOfMessageInGetMessage = 20;
            }
            catch
            {
                NoOfMessageInGetMessage = 20;
            }

            try
            {
                //ServerLog.MgmtExceptionLog("GetPastMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + "," + Convert.ToString(LastMessageDateTime) + ")");
                ServerLog.MgmtExceptionLog("GetPastMessages(" + Convert.ToString(jobj) + ")");
                String UniqueId = jobj["UniqueId"].ToString();
                String AppName = jobj["AppName"].ToString();
                String LastMessageDateTime = jobj["LastMessageDateTime"].ToString();
                if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied UniqueId.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    DateTime? _LastMessageDateTime = null;
                    if (LastMessageDateTime != null && LastMessageDateTime.Trim() != String.Empty)
                        _LastMessageDateTime = Convert.ToDateTime(LastMessageDateTime);

                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    DataTable dtMessages = objMgmtMessaging.GetPastMessages(UniqueId, AppName, NoOfMessageInGetMessage, _LastMessageDateTime, ref Message);
                    if (dtMessages == null || dtMessages.Rows.Count <= 0)
                    {
                        objResponseObjectInfo.Status = Constant.Status_Fail;
                        objResponseObjectInfo.Message = Message;
                        objResponseObjectInfo.dt_ReturnedTables = null;
                    }
                    else
                    {
                        DataTable dtMessageAttachment = null;
                        String MessageIds = String.Empty;
                        foreach (DataRow dr in dtMessages.Rows)
                        {
                            if (dr["IsAnyAttachment"].ToString() == Constant.FLAG_Y)
                                MessageIds += dr["MessageId"].ToString() + ",";
                        }
                        if (MessageIds.Length > 0)
                        {
                            MessageIds = MessageIds.Substring(0, MessageIds.Length - 1);
                            String ErrorMessage = String.Empty;
                            dtMessageAttachment = objMgmtMessaging.GetMessageAttachment(MessageIds, ref ErrorMessage);
                        }
                        objResponseObjectInfo.Status = 1;
                        objResponseObjectInfo.Message = Message;
                        objResponseObjectInfo.ObjRetArgs = new Object[1];
                        objResponseObjectInfo.ObjRetArgs[0] = dtMessages.Rows[dtMessages.Rows.Count - 1]["Date"].ToString();
                        objResponseObjectInfo.dt_ReturnedTables = new dynamic[2];
                        objResponseObjectInfo.dt_ReturnedTables[0] = dtMessages;
                        objResponseObjectInfo.dt_ReturnedTables[1] = dtMessageAttachment;
                    }
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                //ServerLog.MgmtExceptionLog("GetPastMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + "," + Convert.ToString(LastMessageDateTime) + ")");
                ServerLog.MgmtExceptionLog("GetPastMessages(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String DeleteMessages(String MessageId, String AppName)
        public String DeleteMessages([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("DeleteMessages(" + Convert.ToString(MessageId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("DeleteMessages(" + Convert.ToString(jobj) + ")");
                String MessageId = jobj["MessageId"].ToString();
                String AppName = jobj["AppName"].ToString();
                if (MessageId == null || MessageId.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied MessageId.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (AppName == null || AppName.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied AppName.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.DeleteMessage(MessageId, AppName, ref Message);
                    objResponseObjectInfo.Status = result;
                    objResponseObjectInfo.Message = Message;
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                //ServerLog.MgmtExceptionLog("DeleteMessages(" + Convert.ToString(MessageId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("DeleteMessages(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        //public String MarkReadMessages(String MessageId, String AppName)
        public String MarkReadMessages([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("MarkReadMessages(" + Convert.ToString(MessageId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("MarkReadMessages(" + Convert.ToString(jobj) + ")");
                String MessageId = jobj["MessageId"].ToString();
                String AppName = jobj["AppName"].ToString();
                if (MessageId == null || MessageId.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied MessageId.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else if (AppName == null || AppName.Trim() == String.Empty)
                {
                    objResponseObjectInfo.Status = Constant.Status_Fail;
                    objResponseObjectInfo.Message = "Please supplied AppName.";
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.MarkReadMessage(MessageId, AppName, ref Message);
                    objResponseObjectInfo.Status = result;
                    objResponseObjectInfo.Message = Message;
                    objResponseObjectInfo.dt_ReturnedTables = null;
                }
            }
            catch (Exception ex)
            {
                objResponseObjectInfo.Status = Constant.Status_Fail;
                objResponseObjectInfo.Message = ex.Message;
                objResponseObjectInfo.dt_ReturnedTables = null;
                //ServerLog.MgmtExceptionLog("MarkReadMessages(" + Convert.ToString(MessageId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("MarkReadMessages(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        [HttpPost]
        public String MarkReadALLMessages([FromBody]JObject jobj)
        {
            ResponseObjectInfo objResponseObjectInfo = new ResponseObjectInfo();
            try
            {
                //ServerLog.MgmtExceptionLog("MarkReadALLMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("MarkReadALLMessages(" + Convert.ToString(jobj) + ")");
                String UniqueId = jobj["UniqueId"].ToString();
                String AppName = jobj["AppName"].ToString();
                if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", " Please supplied UniqueId.");
                }
                else if (AppName == null || AppName.Trim() == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", " Please supplied AppName.");
                }
                else
                {
                    String Message = String.Empty;
                    MgmtMessaging objMgmtMessaging = new MgmtMessaging();
                    Byte result = objMgmtMessaging.MarkReadALLMessage(UniqueId, AppName, ref Message);
                    if (result == 1)
                        return BLGeneralUtil.return_ajax_string("1", Message);
                    else
                        return BLGeneralUtil.return_ajax_string("0", Message);
                    // return BLGeneralUtil.return_ajax_string("1", Message);
                }
            }
            catch (Exception ex)
            {
                //ServerLog.MgmtExceptionLog("MarkReadALLMessages(" + Convert.ToString(UniqueId) + "," + Convert.ToString(AppName) + ")");
                ServerLog.MgmtExceptionLog("MarkReadALLMessages(" + Convert.ToString(jobj) + ")");
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
            //return JsonConvert.SerializeObject(objResponseObjectInfo);
        }

        #endregion

        [HttpPost]
        public string SavePartnerDetails([FromBody] JObject Jobj)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            Document objdoc = new Document();
            Master master = new Master();
            string msg = "";
            string DocPerId = "";
            DataTable dt_partnerdetails = new DataTable();
            List<Partners_mst> lstpartner = new List<Partners_mst>();
            DS_User ds_user = new DS_User();
            BLGeneralUtil blgn = new BLGeneralUtil();

            if (Jobj["partners_details"] != null)
            {
                lstpartner = Jobj["partners_details"].ToObject<List<Partners_mst>>();
                DataSet ds = master.CreateDataSet(Jobj["partners_details"].ToObject<List<Partners_mst>>());
                dt_partnerdetails = BLGeneralUtil.CheckDateTime(ds.Tables[0]);
            }


            DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();

            try
            {

                #region truck_rto_registration_detail

                if (lstpartner != null && lstpartner.Count > 0)
                {
                    try
                    {
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "PRD", "", "", ref DocPerId, ref msg))
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", msg);
                        }

                        ds_user.EnforceConstraints = false;
                        ds_user.partners_mst.ImportRow(dt_partnerdetails.Rows[0]);
                        ds_user.partners_mst[0].partner_id = DocPerId;
                        ds_user.partners_mst[0].active_flag = Constant.Flag_Yes;
                        ds_user.partners_mst[0].created_date = System.DateTime.UtcNow;
                        ds_user.partners_mst[0].AcceptChanges();
                        ds_user.partners_mst[0].SetAdded();
                        ds_user.EnforceConstraints = true;
                    }
                    catch (Exception ex)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        ServerLog.Log("Rto Registration : " + ex.Message.ToString());
                        return BLGeneralUtil.return_ajax_string("0", "Partner details saved : " + ex.Message.ToString());
                    }
                }

                #endregion

                if (ds_user.partners_mst != null && ds_user.partners_mst.Rows.Count > 0)
                {
                    objBLReturnObject = master.UpdateTables(ds_user.partners_mst, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }

                string message = " Hello <br /> <br /> <b>" + lstpartner[0].name + "</b>  request for become a partner <br/><br/><b> NAME</b> : " + lstpartner[0].name + " <BR/> <b> CONTECT NUMBER </b>:" + lstpartner[0].mobileno + " <BR/> <b> COMPANY NAME </b> : " + lstpartner[0].company_name + " <br /> <b>CITY </b> : " + lstpartner[0].city_name + " <br/><b> SERVICE OFFERED </b>  : " + lstpartner[0].service_offered + " <BR/> <b> Address </b>:" + lstpartner[0].address + " <br/> <br/> Thanks you";

                EMail objmail = new EMail();
                DataTable dt_param = null; msg = "";
                string sendto = ""; msg = "";
                dt_param = blgn.GetParameter("CONTACT", "SENDTO", ref msg);
                if (dt_param == null)
                {
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", msg);
                }

                sendto = dt_param.Rows[0]["param_value"].ToString().Trim();

                Boolean bl = objmail.SendMail(sendto, message, "New Partner request", ref msg, "PARTNERDETAILS", "SENDFROM");
                new EMail().GenerateBecomePartnerEmail(lstpartner[0].email, "", lstpartner[0].name, "", "");
                //  Boolean bl2 = objmail.SendMail("contact@trukker.in", message, "Thanks for being/requesting part of  Trukker UAe", ref msg, "PARTNERDETAILS", "SENDFROM");

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 1;
                ServerLog.SuccessLog("Partner details save : " + DocPerId);
                return BLGeneralUtil.return_ajax_string("1", "Thanks for registering as a partner of TruKKer. We will get in touch with you shortly.");
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
        public DataTable GetOrders(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM   post_load_inquiry where load_inquiry_no=@inqid  ";

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

        //[HttpPost]
        //public String CreatePayment([FromBody]JObject jobj)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    DS_orders dsorder = new DS_orders();
        //    string DocNo = ""; string message = ""; string status = "";

        //    List<Card> tcard = new List<Card>();
        //    if (jobj["CardDetails"] != null)
        //        tcard = jobj["CardDetails"].ToObject<List<Card>>();

        //    if (tcard[0].Number == null || tcard[0].Number.Trim() == "")
        //    {
        //        return BLGeneralUtil.return_ajax_string("0", "Please provide Card Number");
        //    }
        //    else
        //    {
        //        DataTable dtpostloadinq = GetOrders(tcard[0].load_inquiry_no);
        //        if (dtpostloadinq == null)
        //            return BLGeneralUtil.return_ajax_string("0", "Order detail not found");

        //        try
        //        {
        //            #region Payment Start

        //            _Tokenservice = new StartChargeService("test_sec_k_16dc38ad730d6ba806a92");
        //            _workingCard = new Card()
        //            {
        //                Name = tcard[0].Name,
        //                Cvc = Convert.ToInt32(tcard[0].Cvc),
        //                ExpireMonth = tcard[0].ExpireMonth,
        //                ExpireYear = tcard[0].ExpireYear,
        //                Number = tcard[0].Number,

        //            };

        //            _createChargeRequest = new CreateChargeRequest()
        //            {
        //                Amount = Convert.ToInt32(tcard[0].Amount),
        //                Currency = Currency.AED,
        //                Email = tcard[0].Email
        //            };

        //            try
        //            {
        //                #region Create Transaction Entry


        //                DS_orders.order_paymentdetailsRow tr = dsorder.order_paymentdetails.Neworder_paymentdetailsRow();
        //                DBConnection.Open();
        //                DBCommand.Transaction = DBConnection.BeginTransaction();

        //                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
        //                {
        //                    return BLGeneralUtil.return_ajax_string("0", message);
        //                }
        //                dsorder.EnforceConstraints = false;
        //                tr.Transaction_id = DocNo;
        //                tr.load_inquiry_no = tcard[0].load_inquiry_no;
        //                dsorder.EnforceConstraints = true;

        //                dsorder.order_paymentdetails.Addorder_paymentdetailsRow(tr);
        //                dsorder.order_paymentdetails.Rows[0].AcceptChanges();
        //                dsorder.order_paymentdetails.Rows[0].SetAdded();

        //                objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                if (objBLReturnObject.ExecutionStatus == 2)
        //                {
        //                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 2;
        //                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                }

        //                DBCommand.Transaction.Commit();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 1;
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.Log(ex.ToString() + Environment.NewLine + ex.StackTrace);
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                return (BLGeneralUtil.return_ajax_string("0", "Invalid  "));
        //            }

        //                #endregion



        //            _createChargeRequest.CardDetails = _workingCard;

        //            ApiResponse<Charge> Response = _Tokenservice.CreateCharge(_createChargeRequest);
        //            if (Response.IsError)
        //            {
        //                return BLGeneralUtil.return_ajax_string("0", Response.Error.Extras.ToString());
        //            }
        //            else
        //            {
        //                status = Response.Content.State;
        //                if (status == ChargeState.Authorized)
        //                {
        //                    _createChargeRequest.Capture = true;
        //                    ApiResponse<Charge> ResponseCaptured = _Tokenservice.CreateCharge(_createChargeRequest);
        //                    status = ResponseCaptured.Content.State;

        //                    if (status == ChargeState.Captured)
        //                    {

        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        TruckerMaster objTruckerMaster = new TruckerMaster();
        //                        decimal DiscountPrice = 0;
        //                        if (tcard[0].promocode.ToString().Trim() != "")
        //                        {
        //                            decimal flatdiscount = 0; decimal PercentageDiscount = 0; string Msg = "";
        //                            Boolean B1 = objTruckerMaster.IsCouponValid(tcard[0].promocode, tcard[0].shipper_id, tcard[0].load_inquiry_no, tcard[0].load_inquiry_no, System.DateTime.UtcNow, ref flatdiscount, ref PercentageDiscount, ref Msg);
        //                            if (B1)
        //                            {
        //                                decimal Total_cost = Convert.ToDecimal(tcard[0].Total_cost);
        //                                if (flatdiscount != 0)
        //                                    DiscountPrice = Math.Round(flatdiscount);
        //                                else if (PercentageDiscount != 0)
        //                                    DiscountPrice = Total_cost * (PercentageDiscount / 100);

        //                                if (DiscountPrice != 0)
        //                                {
        //                                    Total_cost = Total_cost - DiscountPrice;
        //                                }

        //                                Byte B2 = objTruckerMaster.SaveCouponUserHistory(ref DBCommand, tcard[0].promocode, tcard[0].shipper_id, tcard[0].load_inquiry_no, tcard[0].load_inquiry_no, System.DateTime.UtcNow, flatdiscount, PercentageDiscount, Total_cost, DiscountPrice, tcard[0].created_by, tcard[0].created_host, tcard[0].device_id, tcard[0].device_type, ref Msg);
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




        //                        dsorder.EnforceConstraints = false;
        //                        dsorder.order_paymentdetails[0].Transaction_id = DocNo;
        //                        dsorder.order_paymentdetails[0].load_inquiry_no = tcard[0].load_inquiry_no;
        //                        dsorder.order_paymentdetails[0].charge_id = Response.Content.Id;
        //                        dsorder.order_paymentdetails[0].amount = Response.Content.Amount;
        //                        dsorder.order_paymentdetails[0].captured_amount = Response.Content.CapturedAmount;
        //                        dsorder.order_paymentdetails[0].currency = Response.Content.Currency;
        //                        dsorder.order_paymentdetails[0].description = Response.Content.Description;
        //                        dsorder.order_paymentdetails[0].email = Response.Content.Email;
        //                        dsorder.order_paymentdetails[0].status = Response.Content.State;
        //                        dsorder.order_paymentdetails[0].refunded_amount = Response.Content.RefundedAmount;
        //                        dsorder.order_paymentdetails[0].failure_code = Response.Content.FailureCode;
        //                        dsorder.order_paymentdetails[0].failure_reason = Response.Content.FailureReason;
        //                        dsorder.order_paymentdetails[0].created_at = Response.Content.CreatedAt;
        //                        dsorder.order_paymentdetails[0].updated_at = Response.Content.UpdatedAt;
        //                        dsorder.order_paymentdetails[0].card_id = Response.Content.Card.Id;
        //                        dsorder.order_paymentdetails[0].created_date = DateTime.UtcNow;
        //                        dsorder.order_paymentdetails[0].created_by = "ADMIN";
        //                        dsorder.EnforceConstraints = true;

        //                        objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus == 2)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 2;
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }


        //                        DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();
        //                        dspostload.EnforceConstraints = false;
        //                        dspostload.post_load_inquiry.ImportRow(dtpostloadinq.Rows[0]);
        //                        dspostload.post_load_inquiry[0].payment_status = Constant.FLAG_Y;
        //                        dspostload.post_load_inquiry[0].Total_cost_without_discount = Convert.ToDecimal(tcard[0].Total_cost);
        //                        dspostload.post_load_inquiry[0].Discount = DiscountPrice;
        //                        dspostload.post_load_inquiry[0].coupon_code = tcard[0].promocode;
        //                        dspostload.EnforceConstraints = true;

        //                        objBLReturnObject = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus == 2)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 2;
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }


        //                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tcard[0].load_inquiry_no);
        //                        string cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(tcard[0].load_inquiry_no);

        //                        string OrdId = "";
        //                        DataTable dt_ordersByinq = new PostOrderController().GetOrders(tcard[0].load_inquiry_no);

        //                        if (dt_ordersByinq == null)
        //                        {
        //                            dsorder.EnforceConstraints = false;
        //                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "LO", "", "", ref OrdId, ref message)) // New Driver Notification ID
        //                            {
        //                                ServerLog.Log(message);
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                return BLGeneralUtil.return_ajax_string("0", message);
        //                            }
        //                            dsorder.orders.ImportRow(dspostload.post_load_inquiry[0]);
        //                            dsorder.orders[0].order_id = OrdId;
        //                        }
        //                        else
        //                            dsorder.orders.ImportRow(dt_ordersByinq.Rows[0]);

        //                        dsorder.orders[0].trackurl = trakurl;
        //                        dsorder.orders[0].cbmlink = cbmlink;
        //                        dsorder.EnforceConstraints = true;

        //                        objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus == 2)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 2;
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }




        //                        DBCommand.Transaction.Commit();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 1;

        //                        DataTable dt = dspostload.post_load_inquiry;
        //                        try
        //                        {
        //                            PostOrderController objPostOrder = new PostOrderController();
        //                            string shipperEmail = objPostOrder.GetEmailByID(dt.Rows[0]["shipper_id"].ToString());
        //                            string shippername = objPostOrder.GetUserdetailsByID(dt.Rows[0]["shipper_id"].ToString());

        //                            string Msg = "Thank you..Your order from  " + dt.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt.Rows[0]["load_inquiry_no"].ToString();
        //                            new EMail().SendOtpToUserMobileNoUAE(Msg, objPostOrder.GetMobileNoByID(dtpostloadinq.Rows[0]["shipper_id"].ToString()));

        //                            string MsgMailbody = "Thank you..Your order from  " + dt.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt.Rows[0]["inquiry_destination_addr"].ToString() + " has been made successfully " + System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt.Rows[0]["load_inquiry_no"].ToString();

        //                            ServerLog.Log(shipperEmail);

        //                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderGenerationMail(shipperEmail, " Your Order is confirmed (Order ID: " + tcard[0].load_inquiry_no + ")", shippername, MsgMailbody, tcard[0].load_inquiry_no, dt, dt.Rows[0]["TotalPackingCharge"].ToString()));
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

        //                        return BLGeneralUtil.return_ajax_statusdata("1", "Thank you...Payment done sucessfully", SendReceiveJSon.GetJson(dt));
        //                    }
        //                    else
        //                    {
        //                        DBConnection.Open();
        //                        DBCommand.Transaction = DBConnection.BeginTransaction();

        //                        dsorder.EnforceConstraints = false;
        //                        dsorder.order_paymentdetails[0].Transaction_id = DocNo;
        //                        dsorder.order_paymentdetails[0].load_inquiry_no = tcard[0].load_inquiry_no;
        //                        dsorder.order_paymentdetails[0].charge_id = Response.Content.Id;
        //                        dsorder.order_paymentdetails[0].amount = Response.Content.Amount;
        //                        dsorder.order_paymentdetails[0].captured_amount = Response.Content.CapturedAmount;
        //                        dsorder.order_paymentdetails[0].currency = Response.Content.Currency;
        //                        dsorder.order_paymentdetails[0].description = Response.Content.Description;
        //                        dsorder.order_paymentdetails[0].email = Response.Content.Email;
        //                        dsorder.order_paymentdetails[0].status = Response.Content.State;
        //                        dsorder.order_paymentdetails[0].refunded_amount = Response.Content.RefundedAmount;
        //                        dsorder.order_paymentdetails[0].failure_code = Response.Content.FailureCode;
        //                        dsorder.order_paymentdetails[0].failure_reason = Response.Content.FailureReason;
        //                        dsorder.order_paymentdetails[0].created_at = Response.Content.CreatedAt;
        //                        dsorder.order_paymentdetails[0].updated_at = Response.Content.UpdatedAt;
        //                        dsorder.order_paymentdetails[0].card_id = Response.Content.Card.Id;
        //                        dsorder.order_paymentdetails[0].created_by = "ADMIN";
        //                        dsorder.EnforceConstraints = true;

        //                        objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                        if (objBLReturnObject.ExecutionStatus == 2)
        //                        {
        //                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                            objBLReturnObject.ExecutionStatus = 2;
        //                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                        }


        //                        DBCommand.Transaction.Commit();
        //                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                        objBLReturnObject.ExecutionStatus = 1;

        //                        return BLGeneralUtil.return_ajax_string("0", "Something went Wrong! Please try again.");
        //                    }
        //                }
        //                else
        //                {
        //                    DBConnection.Open();
        //                    DBCommand.Transaction = DBConnection.BeginTransaction();

        //                    dsorder.EnforceConstraints = false;
        //                    dsorder.order_paymentdetails[0].Transaction_id = DocNo;
        //                    dsorder.order_paymentdetails[0].load_inquiry_no = tcard[0].load_inquiry_no;
        //                    dsorder.order_paymentdetails[0].charge_id = Response.Content.Id;
        //                    dsorder.order_paymentdetails[0].amount = Response.Content.Amount;
        //                    dsorder.order_paymentdetails[0].captured_amount = Response.Content.CapturedAmount;
        //                    dsorder.order_paymentdetails[0].currency = Response.Content.Currency;
        //                    dsorder.order_paymentdetails[0].description = Response.Content.Description;
        //                    dsorder.order_paymentdetails[0].email = Response.Content.Email;
        //                    dsorder.order_paymentdetails[0].status = Response.Content.State;
        //                    dsorder.order_paymentdetails[0].refunded_amount = Response.Content.RefundedAmount;
        //                    dsorder.order_paymentdetails[0].failure_code = Response.Content.FailureCode;
        //                    dsorder.order_paymentdetails[0].failure_reason = Response.Content.FailureReason;
        //                    dsorder.order_paymentdetails[0].created_at = Response.Content.CreatedAt;
        //                    dsorder.order_paymentdetails[0].updated_at = Response.Content.UpdatedAt;
        //                    dsorder.order_paymentdetails[0].card_id = Response.Content.Card.Id;
        //                    dsorder.order_paymentdetails[0].created_by = "ADMIN";
        //                    dsorder.EnforceConstraints = true;

        //                    objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
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
        //                }

        //            #endregion

        //                if (status == ChargeState.Captured)
        //                    return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
        //                else
        //                    return BLGeneralUtil.return_ajax_string("0", "Something went Wrong! Please try again.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //            objBLReturnObject.ExecutionStatus = 2;
        //            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);

        //        }

        //        #region MyRegion

        //        //IStartChargeService _service = new StartChargeService("test_sec_k_47696b34550b73a0b40e9");

        //        //Card _workingCard = new Card()
        //        //{
        //        //    Name = "Abdullah Ahmed",
        //        //    Cvc = 123,
        //        //    ExpireMonth = 12,
        //        //    ExpireYear = 2020,
        //        //    Number = "4242424242424242"
        //        //};

        //        //Address _billingAddress = new Address()
        //        //{
        //        //    FirstName = "Abdullah",
        //        //    LastName = "Ahmed",
        //        //    Country = "UAE",
        //        //    City = "Dubai",
        //        //    Address1 = "201, BT Building",
        //        //    Address2 = "Knowledge Village",
        //        //    Phone = "+97144252690"
        //        //};

        //        //Address _shippingAddress = new Address()
        //        //{
        //        //    FirstName = "Abdullah",
        //        //    LastName = "Ahmed",
        //        //    Country = "UAE",
        //        //    City = "Dubai",
        //        //    Address1 = "201, BT Building",
        //        //    Address2 = "Knowledge Village",
        //        //    Phone = "+97144252690"
        //        //};

        //        //Item _item = new Item()
        //        //{
        //        //    Title = "iPhone 5s",
        //        //    Amount = 150000,
        //        //    Quantity = 1
        //        //};
        //        //List<Item> _items = new List<Item>();
        //        //_items.Add(_item);

        //        //ShoppingCart _shoppingCart = new ShoppingCart()
        //        //{
        //        //    UserName = "abdulla_ahmed",
        //        //    RegisteredAt = Convert.ToDateTime("2015-11-17T11:07:59.257Z"),
        //        //    Items = _items,
        //        //    BillingAddress = _billingAddress,
        //        //    ShippingAddress = _shippingAddress
        //        //};

        //        //CreateChargeRequest _createChargeRequest = new CreateChargeRequest()
        //        //{
        //        //    Amount = 10000,
        //        //    Currency = Currency.AED,
        //        //    Email = "john.doe@gmail.com",
        //        //    ShoppingCart = _shoppingCart,
        //        //    CardDetails = _workingCard
        //        //};

        //        //_Tokenservice = new StartChargeService("test_sec_k_47696b34550b73a0b40e9");
        //        //ApiResponse<Charge> responseToken = _Tokenservice.CreateToken(_createChargeRequest);

        //        //ApiResponse<Charge> response = _service.CreateCharge(_createChargeRequest);



        //        #endregion
        //    }

        //}

        [HttpPost]
        public string ChangePassword([FromBody]  JObject jobj)
        {
            DS_User ds_user = new DS_User();
            DataSet dsuser = new DataSet();
            Master master = new Master();
            List<User> tload = new List<User>();
            if (jobj["User"] != null)
            {
                tload = jobj["User"].ToObject<List<User>>();
                dsuser = master.CreateDataSet(tload);
            }
            DataTable dtusr = GetUserPassword("", tload[0].user_id, tload[0].old_password);
            if (dtusr == null)
            {
                return BLGeneralUtil.return_ajax_string("0", "User details not found for :" + tload[0].user_id + " or old password does not match ");
            }

            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();
            try
            {
                if (dsuser != null && dsuser.Tables.Count > 0 && dsuser.Tables[0].Rows.Count > 0)
                {
                    ds_user.EnforceConstraints = false;
                    ds_user.user_mst.ImportRow(dtusr.Rows[0]);
                    ds_user.user_mst[0].password = tload[0].password.ToString();
                    ds_user.user_mst[0].AcceptChanges();
                    ds_user.user_mst[0].SetModified();
                    ds_user.EnforceConstraints = true;
                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_user.user_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != ds_user.user_mst.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        ServerLog.Log(objUpdateTableInfo.ErrorMessage.ToString());
                        return BLGeneralUtil.return_ajax_string("0", "Error occured while updating user details ");
                    }
                }
                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("1", "Password Modified Successfully");
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        //[HttpPost]
        //public String CreatePaymentNew([FromBody]JObject jobj)
        //{
        //    Master master = new Master();
        //    Document objdoc = new Document();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    DS_orders dsorder = new DS_orders();
        //    string DocNo = ""; string message = "";

        //    if (jobj["token"]["id"] == null)
        //    {
        //        return BLGeneralUtil.return_ajax_string("0", "Please provide Card Number");
        //    }
        //    else
        //    {
        //        DataTable dtorder = new PostOrderController().GetOrders(jobj["load_inquiry_no"].ToString());
        //        try
        //        {

        //            CreateCustomer_UsingCardDetails_Success();
        //            #region Payment Start

        //            try
        //            {
        //                #region Create Transaction Entry


        //                DS_orders.order_paymentdetailsRow tr = dsorder.order_paymentdetails.Neworder_paymentdetailsRow();
        //                DBConnection.Open();
        //                DBCommand.Transaction = DBConnection.BeginTransaction();

        //                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "OP", "", "", ref DocNo, ref message)) // New Driver Notification ID
        //                {
        //                    return BLGeneralUtil.return_ajax_string("0", message);
        //                }
        //                dsorder.EnforceConstraints = false;
        //                tr.Transaction_id = DocNo;
        //                tr.load_inquiry_no = jobj["load_inquiry_no"].ToString();
        //                dsorder.EnforceConstraints = true;

        //                dsorder.order_paymentdetails.Addorder_paymentdetailsRow(tr);
        //                dsorder.order_paymentdetails.Rows[0].AcceptChanges();
        //                dsorder.order_paymentdetails.Rows[0].SetAdded();

        //                objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                if (objBLReturnObject.ExecutionStatus == 2)
        //                {
        //                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 2;
        //                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                }

        //                DBCommand.Transaction.Commit();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 1;
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.Log(ex.ToString() + Environment.NewLine + ex.StackTrace);
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                return (BLGeneralUtil.return_ajax_string("0", "Invalid  "));
        //            }

        //                #endregion

        //            _workingCard = new Card()
        //            {
        //                Name = "Abdullah Ahmed",
        //                Cvc = 123,
        //                ExpireMonth = 12,
        //                ExpireYear = 2020,
        //                Number = "4242424242424242"
        //            };

        //            _createChargeRequest = new CreateChargeRequest()
        //            {
        //                Amount = 10000,
        //                Currency = Currency.AED,
        //                Email = "john.doe@gmail.com"
        //            };

        //            _createChargeRequest.CardDetails = _workingCard;
        //            _Tokenservice = new StartChargeService("test_sec_k_8512e94e69d6a46c67ab2");

        //            ApiResponse<Charge> Response = _Tokenservice.CreateCharge(_createChargeRequest);

        //            if (Response.Content.State == ChargeState.Captured)
        //            {
        //                DBConnection.Open();
        //                DBCommand.Transaction = DBConnection.BeginTransaction();

        //                dsorder.EnforceConstraints = false;
        //                dsorder.order_paymentdetails[0].Transaction_id = DocNo;
        //                dsorder.order_paymentdetails[0].load_inquiry_no = jobj["load_inquiry_no"].ToString();
        //                dsorder.order_paymentdetails[0].charge_id = Response.Content.Id;
        //                dsorder.order_paymentdetails[0].amount = Response.Content.Amount;
        //                dsorder.order_paymentdetails[0].captured_amount = Response.Content.CapturedAmount;
        //                dsorder.order_paymentdetails[0].currency = Response.Content.Currency;
        //                dsorder.order_paymentdetails[0].description = Response.Content.Description;
        //                dsorder.order_paymentdetails[0].email = Response.Content.Email;
        //                dsorder.order_paymentdetails[0].status = Response.Content.State;
        //                dsorder.order_paymentdetails[0].refunded_amount = Response.Content.RefundedAmount;
        //                dsorder.order_paymentdetails[0].failure_code = Response.Content.FailureCode;
        //                dsorder.order_paymentdetails[0].failure_reason = Response.Content.FailureReason;
        //                dsorder.order_paymentdetails[0].created_at = Response.Content.CreatedAt;
        //                dsorder.order_paymentdetails[0].updated_at = Response.Content.UpdatedAt;
        //                dsorder.order_paymentdetails[0].card_id = Response.Content.Card.Id;
        //                dsorder.order_paymentdetails[0].created_date = DateTime.UtcNow;
        //                dsorder.order_paymentdetails[0].created_by = "ADMIN";
        //                dsorder.EnforceConstraints = true;

        //                objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                if (objBLReturnObject.ExecutionStatus == 2)
        //                {
        //                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 2;
        //                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                }

        //                dsorder.EnforceConstraints = false;
        //                dsorder.orders.ImportRow(dtorder.Rows[0]);
        //                dsorder.orders[0].payment_status = Constant.FLAG_Y;

        //                objBLReturnObject = master.UpdateTables(dsorder.orders, ref DBCommand);
        //                if (objBLReturnObject.ExecutionStatus == 2)
        //                {
        //                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 2;
        //                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                }

        //                DBCommand.Transaction.Commit();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 1;
        //            }
        //            else
        //            {
        //                DBConnection.Open();
        //                DBCommand.Transaction = DBConnection.BeginTransaction();

        //                dsorder.EnforceConstraints = false;
        //                dsorder.order_paymentdetails[0].Transaction_id = DocNo;
        //                dsorder.order_paymentdetails[0].load_inquiry_no = jobj["load_inquiry_no"].ToString();
        //                dsorder.order_paymentdetails[0].charge_id = Response.Content.Id;
        //                dsorder.order_paymentdetails[0].amount = Response.Content.Amount;
        //                dsorder.order_paymentdetails[0].captured_amount = Response.Content.CapturedAmount;
        //                dsorder.order_paymentdetails[0].currency = Response.Content.Currency;
        //                dsorder.order_paymentdetails[0].description = Response.Content.Description;
        //                dsorder.order_paymentdetails[0].email = Response.Content.Email;
        //                dsorder.order_paymentdetails[0].status = Response.Content.State;
        //                dsorder.order_paymentdetails[0].refunded_amount = Response.Content.RefundedAmount;
        //                dsorder.order_paymentdetails[0].failure_code = Response.Content.FailureCode;
        //                dsorder.order_paymentdetails[0].failure_reason = Response.Content.FailureReason;
        //                dsorder.order_paymentdetails[0].created_at = Response.Content.CreatedAt;
        //                dsorder.order_paymentdetails[0].updated_at = Response.Content.UpdatedAt;
        //                if (Response.Content.Card != null)
        //                    dsorder.order_paymentdetails[0].card_id = Response.Content.Card.Id;
        //                dsorder.order_paymentdetails[0].created_by = "ADMIN";
        //                dsorder.EnforceConstraints = true;

        //                objBLReturnObject = master.UpdateTables(dsorder.order_paymentdetails, ref DBCommand);
        //                if (objBLReturnObject.ExecutionStatus == 2)
        //                {
        //                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                    objBLReturnObject.ExecutionStatus = 2;
        //                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //                }

        //                DBCommand.Transaction.Commit();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 1;
        //            }

        //            #endregion

        //            if (Response.Content.State == ChargeState.Captured)
        //                return BLGeneralUtil.return_ajax_string("1", "Thank you...Payment done sucessfully");
        //            else
        //                return BLGeneralUtil.return_ajax_string("0", "Something went Wrong! Please try again.");
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
        //}


        //{"feedback":{"load_inquiry_no":"111222333","star_rating":"asdfg","feedback":"asdf","created_by":"","created_host":""}}
        [HttpPost]
        public String SaveFeedback([FromBody]JObject jobj)
        {
            Master master = new Master();
            Document objdoc = new Document(); string DocNo = ""; string message = "";
            BLReturnObject objBLReturnObject = new BLReturnObject();
            DS_InquiryEmailLog dsFeedback = new DS_InquiryEmailLog();

            if (jobj["feedback"]["load_inquiry_no"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Please provide load_inquiry_no");
            }
            else
            {
                try
                {
                    DS_InquiryEmailLog.Feedback_mstRow tr = dsFeedback.Feedback_mst.NewFeedback_mstRow();
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();


                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "FB", "", "", ref DocNo, ref message)) // New Driver Notification ID
                    {
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }

                    dsFeedback.EnforceConstraints = false;
                    tr.sr_no = DocNo;
                    tr.load_inquiry_no = jobj["feedback"]["load_inquiry_no"].ToString();
                    tr.star_rating = jobj["feedback"]["star_rating"].ToString();
                    tr.Feedback = jobj["feedback"]["feedback"].ToString();
                    tr.created_by = jobj["feedback"]["created_by"].ToString();
                    tr.created_host = jobj["feedback"]["created_host"].ToString();
                    tr.created_date = DateTime.UtcNow;
                    dsFeedback.EnforceConstraints = true;
                    dsFeedback.Feedback_mst.AddFeedback_mstRow(tr);
                    dsFeedback.Feedback_mst.Rows[0].AcceptChanges();
                    dsFeedback.Feedback_mst.Rows[0].SetAdded();

                    objBLReturnObject = master.UpdateTables(dsFeedback.Feedback_mst, ref DBCommand);
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
                    return BLGeneralUtil.return_ajax_string("1", "Thank you for your valuable feedback...");
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.ToString() + Environment.NewLine + ex.StackTrace);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return (BLGeneralUtil.return_ajax_string("0", "Invalid  "));
                }
            }
        }

        [HttpPost]
        public string GetAllUserForDeshboard([FromBody]JObject Jobj)
        {
            List<User> objOrder = new List<User>();
            string role = ""; string fromdate = ""; string Todate = ""; string reg_type = "";
            if (Jobj["user_deshboard"] != null)
            {
                objOrder = Jobj["user_deshboard"].ToObject<List<User>>();
                role = objOrder[0].role_id;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
                reg_type = objOrder[0].reg_type;
            }

            try
            {
                string qury = "";
                DataTable dt_user = new DataTable();

                qury = " select * from  user_mst join role_mst on user_mst.role_id=role_mst.RoleType  where  1=1 ";

                if (role != null && role.Trim() != "")
                {
                    qury += " and  roletype= '" + role + "' ";
                }
                if (reg_type != null && reg_type.Trim() != "")
                {
                    qury += " and  reg_type= '" + reg_type + "' ";
                }
                if (fromdate.Trim() != "" && Todate.Trim() != "")
                {
                    qury += " and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
                }
                else if (fromdate.Trim() != "")
                {
                    qury += "and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' ";
                }
                else if (Todate.Trim() != "")
                {
                    qury += "and CAST(created_date AS DATE)<='" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
                }

                qury += " order by created_date desc ";

                if (DBCommand.Connection.State == ConnectionState.Closed) DBCommand.Connection.Open();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
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
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_user)));
                else
                    return (BLGeneralUtil.return_ajax_string("0", "No Data Found"));
            }
            catch (Exception ex)
            {
                if (DBCommand.Connection.State == ConnectionState.Open) DBConnection.Close();
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                return (BLGeneralUtil.return_ajax_string("0", "Error in getting user details"));
            }
        }

        [HttpGet]
        public string GetAllRoles()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select * from role_mst ";
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
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        [HttpGet]
        public DataTable GetDriverdetailsBydeviceid(string deviceid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select (select top(1) load_inquiry_no from order_driver_truck_details  where driver_id=driver_login_detail.driver_id and status not in ('45') order by status desc) as load_inquiry_no, " +
                            " (select top(1) status from order_driver_truck_details  where driver_id=driver_login_detail.driver_id and status not in ('45') order by status desc) as status, " +
                             " * from driver_login_detail left outer join driver_truck_details  on driver_truck_details.driver_id =  driver_login_detail.driver_id " +
                            " where driver_login_detail.device_id = '" + deviceid + "' and driver_login_detail.Is_logedin= 'Y' order by driver_login_detail.created_date desc ";
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
    }
}
