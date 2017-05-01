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
using System.Configuration;
using trukkerUAE.BLL.Master;
using TrkrLite.Controllers;
using System.Web;
using System.IO;

namespace trukkerUAE.Controllers
{
    public class driverController : ServerBase
    {
        Master drvmst = new Master();
        DS_Owner_Mst objdriver = new DS_Owner_Mst();
        BLReturnObject objBLReturnObject = new BLReturnObject();
        JavaScriptSerializer jser = new JavaScriptSerializer();
        StringBuilder sb = new StringBuilder();

        #region

        //get DriverQualification Data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetDriverQualificationData()
        {
            String query = "SELECT qualification_id, qualification_description FROM qualification_mst where active_flag='Y'";

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
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Driver Qualification details not found "));
        }

        //get DriverDestination Data and DriverOrigin Data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetDriverDestinationData()
        {
            String query = "SELECT city_code, city_name FROM city_mst where active_flag='Y'";

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
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Driver Destination details not found "));
        }

        //get DriverLanguage Data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetDriverLanguageData()
        {
            String query = "SELECT language_id, language_description FROM language_mst where active_flag='Y'";

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
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Driver Language details not found "));
        }

        [HttpGet]
        public DataTable GetTruckPositionHistory(string loadinqid, string truckid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM truck_current_position Where load_inquiry_no = @inqid and truck_id=@truckid ";
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));

            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            // DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return dtPostLoadOrders;
            else
                return null;
        }
        //get DriverIdentification Data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetDriverIdentificationData()
        {
            //SqlConnection con = new SqlConnection("Data Source=192.168.1.3\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT Identification_id, Identification_description FROM identification_type_mst where active_flag='Y'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dt);
            string strDriver = "";

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow VDataRow in dt.Rows)
                {
                    var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
                    foreach (DataColumn Column in dt.Columns)
                    {

                        Row.Add(Column.ColumnName, VDataRow[Column].ToString());

                    }
                    dataRows.Add(Row);
                }

                strDriver = ser.Serialize(dataRows); // convert list to JSON string 

            }

            return strDriver;
        }

        [HttpGet]
        public string GetEtransferMaster()
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT etrans_id, etrans_desc FROM etransfer_mst where active_flag='Y' ";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
                return SendReceiveJSon.GetJson(dt).ToString();
            //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt)));
            else
                return "0";


        }

        [HttpGet]
        public string GetDriverDetails()
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            BLReturnObject objrtn = new BLReturnObject();
            String query = "SELECT * FROM driver_mst";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dtOwner);
            if (dtOwner != null && dtOwner.Rows.Count > 0)
            {
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                //return "1";
            }
            else
                return "0";
        }

        [HttpGet]
        public string GetDriverDetailsById(String driverid)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            BLReturnObject objrtn = new BLReturnObject();

            String query = "SELECT * FROM driver_mst Where driver_id = @drvid ";

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@drvid";
            pr1.Value = driverid;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(pr1);

            con.Open();
            cmd.ExecuteNonQuery();

            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dtOwner);
            if (dtOwner != null && dtOwner.Rows.Count > 0)
            {
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                //return "1";
            }
            else
                return "0";
        }

        public DataTable GetDriverDetailsTableById(String driverid)
        {
            String query = "SELECT * FROM driver_mst Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDrivermobiledetails(String driverid)
        {
            String query = "SELECT * FROM driver_mobile_detail Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDriverinsurancedetails(String driverid)
        {
            String query = "SELECT * FROM driver_insurance_detail Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDriverContectdetails(String driverid)
        {
            String query = "SELECT * FROM driver_contact_detail Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDriverTruckDetailsByDrvID(String driverid)
        {
            String query = "SELECT * FROM driver_truck_details Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDriverPreferedDestination(String driverid)
        {
            String query = "SELECT * FROM driver_prefered_destination Where driver_id = '" + driverid + "' ";

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

        public DataTable GetDriverlicenseDetail(String driverid)
        {
            String query = "SELECT * FROM driver_license_detail Where driver_id = '" + driverid + "' ";

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
        public string GetAllDriverByUserForRegistration(string userid)
        {

            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            sb.Clear();
            sb.Append("SELECT distinct driver_mst.* FROM driver_mst left join owner_driver_details on driver_mst.driver_id= owner_driver_details.driver_id ");
            sb.Append("where driver_mst.created_by  = 'ADMIN' and owner_driver_details.driver_id is null order by driver_mst.driver_id");
            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@userid";
            pr1.Value = userid;


            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            cmd.Parameters.Add(pr1);

            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtOwner);

            if (dtOwner != null && dtOwner.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
            else
                return "0";

        }

        public DataTable GetDriverOrderDetails(string loadinqno, string driverid)
        {
            String query = "SELECT * FROM order_driver_details where load_inquiry_no='" + loadinqno + "' and driver_id='" + driverid + "'";

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

        public DataTable GetTruckIdBy(string driverid)
        {
            String query = "SELECT * FROM driver_truck_details where driver_id='" + driverid + "' ";

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
        public string GetDriverTruckDetails(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select orders.order_type_flag ,driver_license_detail.License_no,mover_mst.mover_name,* from order_driver_truck_details " +
                   "left join orders on orders.load_inquiry_no=order_driver_truck_details.load_inquiry_no " +
                    " join driver_mst on driver_mst.driver_id = order_driver_truck_details.driver_id " +
                    " join driver_license_detail on driver_license_detail.driver_id = order_driver_truck_details.driver_id " +
                    " join driver_contact_detail on driver_contact_detail.driver_id = order_driver_truck_details.driver_id " +
                   " join ( SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, " +
                   " truck_rto_registration_detail.vehice_photo,truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, " +
                   " truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo " +
                   " from truck_mst " +
                   " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id " +
                   " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                   " left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id " +
                   " left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id " +
                   " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ) as temp on temp.truck_id =  order_driver_truck_details.truck_id " +
                   " left join mover_mst on mover_mst.mover_id= order_driver_truck_details.mover_id " +
                   " where order_driver_truck_details.load_inquiry_no=@inqid";

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
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
            else
                return (BLGeneralUtil.return_ajax_string("0", " Driver not assigned "));

            //return (BLGeneralUtil.return_ajax_string("0", "Driver not assigned "));
        }
        private DataTable GetDriverTruckDetailsByloadinquiry(string loadinqno, string driverId)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select * from order_driver_truck_details " +
                           " join driver_mst on driver_mst.driver_id = order_driver_truck_details.driver_id " +
                           " join ( SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, " +
                           " truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, " +
                           " truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo " +
                           " from truck_mst " +
                           " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id " +
                           " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                           " left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id " +
                           " left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id " +
                           " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ) as temp on temp.truck_id =  order_driver_truck_details.truck_id " +
                           " where load_inquiry_no=@inqid and  order_driver_truck_details.driver_id='" + driverId + "'";
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
        public DataTable GetOrderstatusDetailsByinquiry(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " SELECT     TOP (1) load_inquiry_no, driver_id, truck_id, status " +
                           " FROM         order_driver_truck_details " +
                           " WHERE     (load_inquiry_no =@inqid) " +
                           " ORDER BY CAST(status AS int)";
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
        public DataTable GetOrderDriverTruckDetails(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            //String query1 = " select * from order_driver_truck_details  where load_inquiry_no=@inqid";
            String query1 = " select driver_mst.name as Name,mover_mst.mover_id,mover_mst.mover_Name,order_driver_truck_details.* from order_driver_truck_details " +
                           " left join driver_mst on order_driver_truck_details.driver_id=driver_mst.driver_id " +
                           " left join mover_mst on order_driver_truck_details.mover_id = mover_mst.mover_id " +
                           " where order_driver_truck_details.load_inquiry_no=@inqid";
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
        public DataTable GetDriverTruckDetailsByinquiryId(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select * from order_driver_truck_details " +
                           " join driver_mst on driver_mst.driver_id = order_driver_truck_details.driver_id " +
                           " join ( SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, " +
                           " truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, " +
                           " truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo " +
                           " from truck_mst " +
                           " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id " +
                           " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                           " left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id " +
                           " left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id " +
                           " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ) as temp on temp.truck_id =  order_driver_truck_details.truck_id " +
                           " where load_inquiry_no=@inqid ";
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
        public DataTable GetDriverOrdersByID(string drvid, string shippingdatetime, string opt)
        {
            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query = "";
                query = " Select distinct user_mst.first_name as shipper_name,user_mst.user_id as shipper_mobile,user_mst.email_id as shipper_email,driver_mst.name as drivername,order_driver_truck_details.*,orders.*,truck_mst.*,driver_license_detail.*,driver_contact_detail.*,GETUTCDATE() as currentdate,    " +
                        " post_load_inquiry.payment_mode  " +
                        " from orders " +
                        " left join user_mst on user_mst.unique_id = orders.shipper_id " +
                        "   join order_driver_truck_details on order_driver_truck_details.load_inquiry_no = orders.load_inquiry_no " +
                        "   left join truck_mst on truck_mst.truck_id = order_driver_truck_details.truck_id " +
                         "   left join driver_license_detail on driver_license_detail.driver_id = order_driver_truck_details.driver_id " +
                         "   left join driver_contact_detail on driver_contact_detail.driver_id = order_driver_truck_details.driver_id " +
                         "   left join truck_make_mst on truck_make_mst.make_id = truck_mst.truck_make_id " +
                         "   left join truck_model_mst on truck_model_mst.model_id = truck_mst.truck_model " +
                         "   left join post_load_inquiry on post_load_inquiry.load_inquiry_no = orders.load_inquiry_no " +
                         "  join driver_mst on order_driver_truck_details.driver_id = driver_mst.driver_id  " +
                         "   where 1=1 and orders.active_flag  = 'Y' ";
                if (opt == "1") // driver previous orders
                    query += "   and orders.shippingdatetime  < cast(@shippingdatetime  as datetime)";
                else if (opt == "2") // driver next orders
                    query += "  and orders.shippingdatetime > cast(@shippingdatetime  as datetime) ";

                query += "   and order_driver_truck_details.driver_id=@drid   and CAST(order_driver_truck_details.status AS INT) not in (45,-01)  order by  orders.shippingdatetime desc ";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("drid", DbType.String, drvid));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("shippingdatetime", DbType.String, shippingdatetime));
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtPostLoadOrders = ds.Tables[0];
                }
                if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                {
                    return dtPostLoadOrders;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DBConnection.Close();
                return null;
            }
        }
        public int GetShipperCreditDays(ref IDbCommand command, String shipper_id, ref String Message)
        {
            try
            {
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  credit_days 
                                   FROM  user_mst    
                                   WHERE  unique_id = @shipper_id ");
                command.Parameters.Clear();
                command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                command.CommandText = SQLSelect.ToString();
                Object objCreditDays = command.ExecuteScalar();
                if (objCreditDays != null && objCreditDays.ToString().Trim() != String.Empty)
                {
                    Message = "Credit Days retrieved successfully for shipper_id : " + shipper_id;
                    return Convert.ToInt32(objCreditDays);
                }
                else
                {
                    Message = "Credit Days is not defined for shipper_id : " + shipper_id;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }
        #endregion

        #region Post method

        /// <summary>
        /// save driver notifiaction 
        /// using this servie we can save the order details that driver pickup,start or complete the order  
        /// if driver Pickup then update status=05 in tables 
        /// if driver start then update status=06 in tables 
        /// if driver complete then update status=45 in tables and on complete then
        [HttpPost]
        public string SaveDriverNotification([FromBody]JObject ord)
        {

            ServerLog.Log(ord.ToString());

            #region Validation

            //if (ord["orders"][0]["owner_id"] == null || ord["orders"][0]["owner_id"].ToString() == "")
            //{
            //    return BLGeneralUtil.return_ajax_string("0", "Owner ID is not found");
            //}
            if (ord["orders"][0]["driver_id"] == null || ord["orders"][0]["driver_id"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Driver ID is not found");
            }
            if (ord["orders"][0]["truck_id"] == null || ord["orders"][0]["truck_id"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Truck ID is not found");
            }
            string currentlat = ord["orders"][0]["current_lat"].ToString().Trim();
            if (currentlat != "")
            {
                if (Math.Round(Convert.ToDecimal(currentlat)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "Latitude is Zero");
            }
            else if (currentlat == "")
                return BLGeneralUtil.return_ajax_string("0", "Latitude not found");

            string currentlng = ord["orders"][0]["current_lng"].ToString().Trim();
            if (currentlng != "")
            {
                if (Math.Round(Convert.ToDecimal(currentlng)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "Longitude is Zero");
            }
            else if (currentlng == "")
                return BLGeneralUtil.return_ajax_string("0", "Longitude not found");


            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(ord["orders"][0]["driver_id"].ToString());
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != ord["orders"][0]["device_id"].ToString())
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }

            if (ord["orders"][0]["current_lat"] == null || ord["orders"][0]["current_lat"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Latitude is not found");
            }
            if (ord["orders"][0]["current_lng"] == null || ord["orders"][0]["current_lng"].ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "Longitude is not found");
            }

            #endregion



            string loadinqid = ""; //string trukid = "";
            DataTable dt_notification = new DataTable();
            Master master = new Master(); string ownid = "";
            DataSet dsdrv = new DataSet();

            DataTable dtuser = new DataTable();
            string pointsperkm = ConfigurationManager.AppSettings["KmsPerCoin"].ToString();
            string Invoicelink = ConfigurationManager.AppSettings["InvoicePdfLink"].ToString();

            try
            {

                truck_current_position objtruck_po = new truck_current_position();
                PostOrderController objpostorder = new PostOrderController();
                BLReturnObject objBLobj = new BLReturnObject();
                DS_driver_order_notifications ds_notification = new DS_driver_order_notifications();
                Document objdoc = new Document(); String DocNtficID = ""; string message = ""; string status = "";
                PostOrderController obj = new PostOrderController(); int extrahrs = 0;
                List<orders> tord = new List<orders>();
                //tord = ord["orders"].ToObject<List<orders>>();
                if (ord["orders"] != null)
                {
                    tord = ord["orders"].ToObject<List<orders>>();
                    dsdrv = master.CreateDataSet(tord);
                }

                if (tord[0].status == Constant.TRUCK_READY_FOR_PICKUP)
                {
                    DataTable dtdriverongoingorder = GetdriverOngoingOrder(tord[0].driver_id);
                    if (dtdriverongoingorder != null)
                    {
                        if (dtdriverongoingorder.Rows.Count > 0)
                            if (dtdriverongoingorder.Rows[0]["load_inquiry_no"].ToString() != tord[0].load_inquiry_no)
                                return BLGeneralUtil.return_ajax_string("0", " You already busy in order cannot pickup for another one ");
                    }
                }

                // DataTable dt_quote = new DataTable();
                loadinqid = tord[0].load_inquiry_no.ToString();
                DataTable dt_post_load_inquiry = obj.GetLoadInquiryById(loadinqid);
                DataTable dt_order = obj.GetLoadInquiryById(tord[0].load_inquiry_no);

                if (dt_order != null && dt_order.Rows.Count > 0)
                {
                    ownid = dt_order.Rows[0]["owner_id"].ToString();
                    //trukid = dt_order.Rows[0]["truck_id"].ToString();
                }
                PostOrderController pord = new PostOrderController();
                DataTable dt_loadinq = pord.GetLoadInquiryById(loadinqid);
                //if (dt_loadinq != null && dt_loadinq.Rows.Count > 0)
                //{
                //    if (dt_loadinq.Rows[0]["load_unload_extra_hours"] != null && dt_loadinq.Rows[0]["load_unload_extra_hours"].ToString() != "")
                //        extrahrs = Convert.ToInt16(dt_loadinq.Rows[0]["load_unload_extra_hours"]);
                //}

                DataTable dt_truck_currentPO = GetTruckCurrentPositionById(loadinqid);
                // DataTable dt_truck = new truckController().GetTruckByID(trukid, loadinqid);
                DataTable driver_truckdetails = GetDriverTruckDetailsByloadinquiry(loadinqid, tord[0].driver_id);
                DataTable dtdriverdetails = GetDriverDetailsTableById(tord[0].driver_id.ToString()); // check driver is in database 




                DBConnection.Open();
                //  dt_quote = objpostorder.GetLoadInquiryQuotationById(loadinqid, "-1");

                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                message = "";

                DS_driver_order_notifications.driver_order_notifications_tempDataTable dt1 = ord["orders"].ToObject<DS_driver_order_notifications.driver_order_notifications_tempDataTable>();
                DS_driver_order_notifications ds_driver = new DS_driver_order_notifications();
                ds_driver.EnforceConstraints = false;

                try
                {
                    if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "DRN", "", "", ref DocNtficID, ref message)) // New Driver Notification ID
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", message);
                    }
                    ds_driver.driver_order_notifications_history.ImportRow(dt1.Rows[0]);
                    ds_driver.driver_order_notifications_history.Rows[0]["notification_id"] = DocNtficID;
                    ds_driver.driver_order_notifications_history.Rows[0]["notification_date"] = System.DateTime.UtcNow;
                    ds_driver.driver_order_notifications_history.Rows[0]["created_date"] = System.DateTime.UtcNow;
                    ds_driver.driver_order_notifications_history.Rows[0]["active_flag"] = Constant.Flag_Yes;
                    ds_driver.driver_order_notifications_history.Rows[0].AcceptChanges();
                    ds_driver.driver_order_notifications_history.Rows[0].SetAdded();
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }

                objBLobj = master.UpdateTables(ds_driver.driver_order_notifications_history, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }

                #region update status in order_driver_truck_details


                DS_driver_order_notifications ds_dr_order = new DS_driver_order_notifications();
                ds_dr_order.EnforceConstraints = false;
                ds_dr_order.order_driver_truck_details.ImportRow(driver_truckdetails.Rows[0]);
                ds_dr_order.order_driver_truck_details[0].status = tord[0].status;

                if (tord[0].status.Trim() == Constant.TRUCK_READY_FOR_PICKUP)   //  "05"  Status by Driver Order Completed
                {
                    //by sandip to add start lat/long in database
                    ds_dr_order.order_driver_truck_details[0].pickup_lat = tord[0].current_lat;
                    ds_dr_order.order_driver_truck_details[0].pickup_lng = tord[0].current_lng;
                    ds_dr_order.order_driver_truck_details[0].pickup_time = System.DateTime.UtcNow; //.ToString("dd-MM-yyyy HH:mm:ss tt");
                    ds_dr_order.order_driver_truck_details[0].pickup_by = tord[0].created_by;


                }
                if (tord[0].status.Trim() == Constant.lOADING_STARTED)   //  "06"  Status by Driver Order Completed
                {
                    //by sandip to add start lat/long in database
                    ds_dr_order.order_driver_truck_details[0].loadingstart_lat = tord[0].current_lat;
                    ds_dr_order.order_driver_truck_details[0].loadingstart_lng = tord[0].current_lng;
                    ds_dr_order.order_driver_truck_details[0].loadingstart_time = System.DateTime.UtcNow;//.ToString("dd-MM-yyyy HH:mm:ss tt");
                    ds_dr_order.order_driver_truck_details[0].loadingstart_by = tord[0].created_by;
                }
                if (tord[0].status.Trim() == Constant.START)   //  "07"  Status by Driver Order Completed
                {
                    //by sandip to add start lat/long in database
                    ds_dr_order.order_driver_truck_details[0].start_lat = tord[0].current_lat;
                    ds_dr_order.order_driver_truck_details[0].start_lng = tord[0].current_lng;
                    ds_dr_order.order_driver_truck_details[0].start_time = System.DateTime.UtcNow;//.ToString("dd-MM-yyyy HH:mm:ss tt");
                    ds_dr_order.order_driver_truck_details[0].start_by = tord[0].created_by;
                }
                if (tord[0].status.Trim() == Constant.UNLOADING_START)   //  "08"  Status by Driver Order Completed
                {
                    //by sandip to add start lat/long in database
                    ds_dr_order.order_driver_truck_details[0].unloadingstart_lat = tord[0].current_lat;
                    ds_dr_order.order_driver_truck_details[0].unloadingstart_lng = tord[0].current_lng;
                    ds_dr_order.order_driver_truck_details[0].unloadingstart_time = System.DateTime.UtcNow;//.ToString("dd-MM-yyyy HH:mm:ss tt");
                    ds_dr_order.order_driver_truck_details[0].unloadingstart_by = tord[0].created_by;
                }
                if (tord[0].status.Trim() == Constant.UNLOADING_COMPLETED)   //  "45"  Status by Driver Order Completed
                {
                    //by sandip to add start lat/long in database
                    ds_dr_order.order_driver_truck_details[0].complete_lat = tord[0].current_lat;
                    ds_dr_order.order_driver_truck_details[0].complete_lng = tord[0].current_lng;
                    ds_dr_order.order_driver_truck_details[0].complete_time = System.DateTime.UtcNow;//.ToString("dd-MM-yyyy HH:mm:ss tt");
                    ds_dr_order.order_driver_truck_details[0].complete_by = tord[0].created_by;
                }

                ds_dr_order.order_driver_truck_details[0].modified_by = tord[0].created_by;
                ds_dr_order.order_driver_truck_details[0].modified_date = System.DateTime.UtcNow;
                ds_dr_order.order_driver_truck_details[0].modified_host = tord[0].created_host;
                ds_dr_order.order_driver_truck_details[0].modified_device_id = tord[0].modified_device_id;
                ds_dr_order.order_driver_truck_details[0].modified_device_type = tord[0].modified_device_type;
                objBLobj = master.UpdateTables(ds_dr_order.order_driver_truck_details, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }


                #endregion


                #region update status in driver_mst

                if (tord[0].status.Trim() == Constant.TRUCK_READY_FOR_PICKUP)   //  "05"  Status by Driver Order Completed
                {
                    DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                    if (dtdriverdetails != null && dtdriverdetails.Rows.Count > 0)
                    {
                        ds_owner.driver_mst.ImportRow(dtdriverdetails.Rows[0]);
                        ds_owner.driver_mst[0].isfree = Constant.Flag_No;

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

                #endregion

                if (tord[0].status == Constant.ORDER_COMPLETED)
                {

                    DS_Truck_current_location ds_truck_po = new DS_Truck_current_location();
                    DS_Truck_current_location.truck_current_positionRow dr_t = ds_truck_po.truck_current_position.Newtruck_current_positionRow();
                    ds_truck_po.EnforceConstraints = false;
                    dr_t.driver_id = tord[0].driver_id;
                    dr_t.truck_id = tord[0].truck_id;
                    dr_t.log_date = System.DateTime.UtcNow;
                    dr_t.load_inquiry_no = tord[0].load_inquiry_no;
                    dr_t.truck_lat = tord[0].current_lat;
                    dr_t.truck_lng = tord[0].current_lng;
                    dr_t.truck_location = null;
                    dr_t.eta = null;
                    dr_t.remaining_kms = null;
                    dr_t.current_kms = Convert.ToDecimal(0.0);
                    dr_t.total_kms = Convert.ToDecimal(0.0);
                    dr_t.active_flag = Constant.Flag_Yes;
                    dr_t.created_by = tord[0].created_by;
                    dr_t.created_date = System.DateTime.UtcNow;
                    dr_t.created_host = tord[0].created_host;
                    dr_t.device_id = tord[0].device_id;
                    dr_t.device_type = tord[0].device_type;
                    ds_truck_po.EnforceConstraints = false;
                    ds_truck_po.truck_current_position.Addtruck_current_positionRow(dr_t);
                    ds_truck_po.truck_current_position.Rows[0].AcceptChanges();
                    ds_truck_po.truck_current_position.Rows[0].SetAdded();


                    objBLobj = master.UpdateTables(ds_truck_po.truck_current_position, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    #region update status in driver_mst

                    if (dtdriverdetails != null)
                    {
                        DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                        if (dtdriverdetails != null && dtdriverdetails.Rows.Count > 0)
                        {
                            ds_owner.driver_mst.ImportRow(dtdriverdetails.Rows[0]);
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

                    #endregion
                }

                #region Save Truck Position to History Table on Order Completion

                DS_Truck_current_location ds_truck = new DS_Truck_current_location();
                ds_truck.EnforceConstraints = false;

                // check OTP Required For complete transaction if record 
                // DataTable dt = GetotpforRecieverByID(loadinqid);

                if (tord[0].status == Constant.ORDER_COMPLETED)
                {
                    //if (dt != null && dt.Rows.Count > 0)
                    //{     // receiver mobile number not provide and if OTP require then its give error 
                    //    if (dt.Rows[0]["receiver_mobile"].ToString() != "" && dt.Rows[0]["receiver_mobile"] != null)
                    //    {
                    //        if (tord[0].otp == "" && tord[0].otp == null)
                    //        {
                    //            return BLGeneralUtil.return_ajax_string("0", "OTP Required For this transaction");
                    //        }
                    //        else if (dt.Rows[0]["otp"].ToString() != tord[0].otp) // check OTP With database and enter by receiver
                    //            return BLGeneralUtil.return_ajax_string("0", "Invalid OTP");
                    //    }
                    //    else
                    //    {
                    //        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //        return BLGeneralUtil.return_ajax_string("0", "Please Provide Receiver Mobile Number");
                    //    }
                    //}



                    DataTable dt_truck_hist = GetTruckPositionHistory(loadinqid, tord[0].truck_id);

                    #region Save Truck History

                    truckController trkc = new truckController();

                    if (dt_truck_hist != null && dt_truck_hist.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_truck_hist.Rows.Count; i++)
                        {
                            ds_truck.truck_current_position_history.ImportRow(dt_truck_hist.Rows[i]);
                            ds_truck.truck_current_position_history[i].AcceptChanges();
                            ds_truck.truck_current_position_history[i].SetAdded();
                            ds_truck.truck_current_position.ImportRow(dt_truck_hist.Rows[i]);
                            ds_truck.truck_current_position[i].AcceptChanges();
                            ds_truck.truck_current_position[i].Delete();
                        }
                        // if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                        if (ds_truck != null && ds_truck.truck_current_position_history != null && ds_truck.truck_current_position_history.Rows.Count > 0)
                        {
                            // DBCommand.Transaction = DBConnection.BeginTransaction();
                            objBLobj = master.UpdateTables(ds_truck.truck_current_position_history, ref DBCommand);
                            if (objBLobj.ExecutionStatus == 2)
                            {
                                ServerLog.Log(objBLobj.ServerMessage.ToString());
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                objBLobj.ExecutionStatus = 2;
                                return BLGeneralUtil.return_ajax_string("0", "Error while saving position history");
                            }
                            // DBCommand.Transaction.Commit();
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
                            //  DBCommand.Transaction.Commit();
                        }
                    }
                    #endregion

                    try
                    {
                        string Msg = "Your order form  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been sucessfully deliverd on " + System.DateTime.UtcNow;

                        new EMail().SendOtpToUserMobileNoUAE(Msg, new PostOrderController().GetOwnerMobileNoByOwnID(ownid));

                        if (dt_order.Rows[0]["receiver_mobile"].ToString() != null || dt_order.Rows[0]["receiver_mobile"].ToString() != "")
                            new EMail().SendOtpToUserMobileNoUAE(Msg, dt_order.Rows[0]["receiver_mobile"].ToString());


                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        ServerLog.Log("Error in send OTP on Completation ");
                    }
                    ServerLog.SuccessLog("Order Completed for Inq Id = " + loadinqid);
                }
                #endregion

                DataTable dtdrviertruckdetails = GetOrderstatusDetailsByinquiry(tord[0].load_inquiry_no);
                if (dtdrviertruckdetails != null && dtdrviertruckdetails.Rows.Count > 0)
                {
                    //if (dtdrviertruckdetails.Rows.Count == 1)
                    //    status = tord[0].status;
                    //else
                    status = dtdrviertruckdetails.Rows[0]["status"].ToString();
                }

                #region Update Order table status

                DS_orders dsord = new DS_orders();
                dsord.orders.ImportRow(dt_order.Rows[0]);
                dsord.orders.Rows[0]["status"] = status;
                if (status != null && status.Trim() != String.Empty && status == Constant.ORDER_COMPLETED)
                {
                    int CreditDays = GetShipperCreditDays(ref DBCommand, dsord.orders[0].shipper_id, ref message);
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

                    ServerLog.SuccessLog("Invoice Start = " + tord[0].load_inquiry_no);
                    bool bl = new MailerController().GenerateInvoiceMail(tord[0].load_inquiry_no);
                    if (bl)
                        ServerLog.SuccessLog("Invoice Generated = " + tord[0].load_inquiry_no);
                    else
                        ServerLog.SuccessLog("Invoice Not Generate = " + tord[0].load_inquiry_no);

                    string filepath = Invoicelink + "Invoicedetail_" + tord[0].load_inquiry_no + ".pdf";
                    dsord.orders.Rows[0]["invoice_pdf_link"] = filepath;
                }

                dsord.orders.Rows[0]["Remark"] = tord[0].Remark;
                dsord.orders.Rows[0]["modified_by"] = tord[0].created_by;
                dsord.orders.Rows[0]["modified_date"] = System.DateTime.UtcNow;
                dsord.orders.Rows[0]["modified_host"] = tord[0].created_host;
                dsord.orders.Rows[0]["modified_device_id"] = tord[0].device_id;
                dsord.orders.Rows[0]["modified_device_type"] = tord[0].device_type;

                objBLobj = master.UpdateTables(dsord.orders, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }


                #endregion

                #region Update post_load_inquiry table status

                DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();
                dspostload.post_load_inquiry.ImportRow(dt_post_load_inquiry.Rows[0]);
                dspostload.post_load_inquiry.Rows[0]["status"] = status;
                dspostload.post_load_inquiry.Rows[0]["modified_by"] = tord[0].created_by;
                dspostload.post_load_inquiry.Rows[0]["modified_date"] = System.DateTime.UtcNow;
                dspostload.post_load_inquiry.Rows[0]["modified_host"] = tord[0].created_host;
                dspostload.post_load_inquiry.Rows[0]["modified_device_id"] = tord[0].device_id;
                dspostload.post_load_inquiry.Rows[0]["modified_device_type"] = tord[0].device_type;

                objBLobj = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }

                #endregion

                string strtext = "";
                if (status == Constant.TRUCK_READY_FOR_PICKUP)
                    strtext = "your order is ready for Pickup";
                if (status == Constant.lOADING_STARTED)
                    strtext = "your luggage loading start ";
                if (status == Constant.START)
                    strtext = "your luggage is start to reach to your new home.";
                if (status == Constant.UNLOADING_START)
                    strtext = "your luggage unloading start";
                if (status == Constant.UNLOADING_COMPLETED)
                    strtext = "Thank you.!Your luggage successfully loaded to your new home visit again.  ";



                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

                if (status == Constant.UNLOADING_COMPLETED)
                {
                    DateTime dubaiTime = new PostOrderController().DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow);
                    string shippername = new PostOrderController().GetUserdetailsByID(dt_order.Rows[0]["shipper_id"].ToString());
                    string strtitle = " Your Order No. " + tord[0].load_inquiry_no + " : Successfully Completed + Your Invoice ";

                    string Msg = "Thank you..<br>Your order from  " + dt_order.Rows[0]["inquiry_source_addr"].ToString() + " To  " + dt_order.Rows[0]["inquiry_destination_addr"].ToString() + " has been delivered successfully " + dubaiTime.ToString("dd-MM-yyyy HH:mm:ss tt") + ".Your reference order number is " + dt_order.Rows[0]["load_inquiry_no"].ToString();

                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(new EMail().GenerateOrderCompletationMail(new PostOrderController().GetEmailByID(dt_order.Rows[0]["shipper_id"].ToString()), strtitle, shippername, Msg, tord[0].load_inquiry_no));
                    if (result["status"].ToString() == "0")
                    {
                        ServerLog.Log("Error Sending Activation Email");
                        // return BLGeneralUtil.return_ajax_string("0", "Error Sending Activation Email");
                    }

                    //Returnstatus = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tord[0].shipper_id, "ADMIN", Constant.MessageType_DriverStatus, strtext, Constant.MessageType_DriverStatus, tord[0].load_inquiry_no, tord[0].shipper_id, tord[0].load_inquiry_no, tord[0].truck_id, tord[0].driver_id, tord[0].owner_id, ref strMessage);
                    //if (Returnstatus == Constant.Status_Fail)
                    //{
                    //    ServerLog.Log(" Error in save notification Data ");
                    //    DBCommand.Transaction.Rollback();
                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //    return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                    //}

                }


                //  ServerLog.SuccessLog("Driver notification Updated Inq Id = " + loadinqid);
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

        //pooja vachhani on 27/11/15
        public string SaveDriverDetails([FromBody]JObject Parameter)
        {


            Master ownermst = new Master();
            DS_Owner_Mst objDriverOwner = new DS_Owner_Mst();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            JavaScriptSerializer ser = new JavaScriptSerializer();

            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            objDriverOwner = saveDriverData(Parameter);




            objBLReturnObject = ownermst.Save_Driver_Mst(objDriverOwner);

            if (objBLReturnObject.ExecutionStatus == 1)
            {
                #region commented code
                //DataTable dt = new DataTable();
                //dt.Columns.Add("status", typeof(string));
                //dt.Columns.Add("Message", typeof(string));

                //dt.Rows.Add("1", "Data Save Successfully");

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    foreach (DataRow VDataRow in dt.Rows)
                //    {
                //        var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
                //        foreach (DataColumn Column in dt.Columns)
                //        {

                //            Row.Add(Column.ColumnName, VDataRow[Column].ToString());

                //        }
                //        dataRows.Add(Row);
                //    }

                //    string strDriver = ser.Serialize(dataRows); // convert list to JSON string 
                //    return strDriver;

                //}
                //else
                //{
                //    return "Data Not Save";
                //}
                #endregion

                return objBLReturnObject.ServerMessage;
            }
            else
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                //return objBLReturnObject.ServerMessage.ToString();
                return objBLReturnObject.ServerMessage;
            }

        }
        //pooja vachhani on 27/11/15

        public DS_Owner_Mst saveDriverData(JObject Parameter)
        {
            var tempkey = Parameter.Properties().Select(p => p.Name).ToList();
            List<driver_mst> drivermstData = new List<driver_mst>();
            List<user_mst> usermstData = new List<user_mst>();
            List<driver_mobile_detail> driverMobilesData = new List<driver_mobile_detail>();
            List<driver_language_detail> driverlanguagedetail = new List<driver_language_detail>();
            List<driver_insurance_detail> driverInsuranceData = new List<driver_insurance_detail>();
            List<driver_identification_detail> driveridentificationdetail = new List<driver_identification_detail>();
            List<driver_contact_detail> driverContactDetails = new List<driver_contact_detail>();
            List<driver_prefered_destination> driverPreferedDestinationData = new List<driver_prefered_destination>();
            List<driver_license_detail> driverlicensedetail = new List<driver_license_detail>();
            List<driver_bank_detail> bankdetail = new List<driver_bank_detail>();
            List<owner_driver_detail> driverowner = new List<owner_driver_detail>();
            List<driver_truck_detail> drivertruck = new List<driver_truck_detail>();

            DataSet dsdrvmst = new DataSet(); DataSet dsusermst = new DataSet();
            DataSet dsdrvmobdtl = new DataSet(); DataSet dsdrvlangdtl = new DataSet(); DataSet dsdrvinsdtl = new DataSet();
            DataSet dsdrvcntcdtl = new DataSet(); DataSet dsdrvprefDest = new DataSet(); DataSet dsdrvbnkdtl = new DataSet();
            DataSet dsdrvlicedtl = new DataSet(); DataSet dsdrvownDtl = new DataSet(); DataSet dsdrvtruckdtl = new DataSet();
            DataTable dtdriverMst = new DataTable(); DataTable dtdrvcntcdtl = new DataTable(); DataTable dtdrvLicdtl = new DataTable();
            Random rand = new Random();
            Random rand1 = new Random();
            int re = rand.Next(1, 1000000);
            int flag = 0;
            JObject jr = JObject.Parse(Parameter.ToString());
            Master ownermst = new Master();
            DS_Owner_Mst objownerDriver = new DS_Owner_Mst();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            try
            {
                if (tempkey.Contains("driver_mst"))
                {
                    drivermstData = Parameter["driver_mst"].ToObject<List<driver_mst>>();
                    dsdrvmst = drvmst.CreateDataSet(Parameter["driver_mst"].ToObject<List<driver_mst>>());
                    dtdriverMst = BLGeneralUtil.CheckDateTime(dsdrvmst.Tables[0]);
                }
                if (tempkey.Contains("user_mst"))
                {
                    usermstData = Parameter["user_mst"].ToObject<List<user_mst>>();
                    dsusermst = drvmst.CreateDataSet(Parameter["user_mst"].ToObject<List<user_mst>>());
                }
                if (tempkey.Contains("driver_mobile_detail"))
                {
                    driverMobilesData = Parameter["driver_mobile_detail"].ToObject<List<driver_mobile_detail>>();
                    dsdrvmobdtl = drvmst.CreateDataSet(Parameter["driver_mobile_detail"].ToObject<List<driver_mobile_detail>>());
                }
                if (tempkey.Contains("driver_language_detail"))
                {
                    driverlanguagedetail = Parameter["driver_language_detail"].ToObject<List<driver_language_detail>>();
                    dsdrvlangdtl = drvmst.CreateDataSet(Parameter["driver_language_detail"].ToObject<List<driver_language_detail>>());
                }
                if (tempkey.Contains("driver_insurance_detail"))
                {
                    driverInsuranceData = Parameter["driver_insurance_detail"].ToObject<List<driver_insurance_detail>>();
                    dsdrvinsdtl = drvmst.CreateDataSet(Parameter["driver_insurance_detail"].ToObject<List<driver_insurance_detail>>());
                }
                if (tempkey.Contains("driver_contact_detail"))
                {
                    driverContactDetails = Parameter["driver_contact_detail"].ToObject<List<driver_contact_detail>>();
                    dsdrvcntcdtl = drvmst.CreateDataSet(Parameter["driver_contact_detail"].ToObject<List<driver_contact_detail>>());
                    dtdrvcntcdtl = BLGeneralUtil.CheckDateTime(dsdrvcntcdtl.Tables[0]);
                }
                //if (tempkey.Contains("driver_identification_detail"))
                //    driveridentificationdetail = Parameter["driver_identification_detail"].ToObject<driver_identification_detail>();
                if (tempkey.Contains("driver_prefered_destination"))
                {
                    driverPreferedDestinationData = Parameter["driver_prefered_destination"].ToObject<List<driver_prefered_destination>>();
                    dsdrvprefDest = drvmst.CreateDataSet(Parameter["driver_prefered_destination"].ToObject<List<driver_prefered_destination>>());
                }
                if (tempkey.Contains("driver_license_detail"))
                {
                    driverlicensedetail = Parameter["driver_license_detail"].ToObject<List<driver_license_detail>>();
                    dsdrvlicedtl = drvmst.CreateDataSet(Parameter["driver_license_detail"].ToObject<List<driver_license_detail>>());
                    dtdrvLicdtl = BLGeneralUtil.CheckDateTime(dsdrvlicedtl.Tables[0]);
                }
                if (tempkey.Contains("driver_bank_detail"))
                {
                    bankdetail = Parameter["driver_bank_detail"].ToObject<List<driver_bank_detail>>();
                    dsdrvbnkdtl = drvmst.CreateDataSet(Parameter["driver_bank_detail"].ToObject<List<driver_bank_detail>>());
                }
                if (tempkey.Contains("driver_owner"))
                {
                    driverowner = Parameter["driver_owner"].ToObject<List<owner_driver_detail>>();
                    dsdrvownDtl = drvmst.CreateDataSet(Parameter["owner_driver_detail"].ToObject<List<owner_driver_detail>>());
                }
                if (tempkey.Contains("driver_truck_detail"))
                {
                    drivertruck = Parameter["driver_truck_detail"].ToObject<List<driver_truck_detail>>();
                    dsdrvtruckdtl = drvmst.CreateDataSet(Parameter["driver_truck_detail"].ToObject<List<driver_truck_detail>>());
                }
                string driverID = ""; DataTable dtuser = new DataTable();
                if (usermstData != null)
                {
                    if (usermstData[0].unique_id != null)
                    {
                        driverID = usermstData[0].unique_id;
                        String query1 = "SELECT * FROM user_mst where  unique_id = '" + usermstData[0].unique_id + "' ";
                        DBDataAdpterObject.SelectCommand.CommandText = query1;
                        DataSet ds = new DataSet();
                        DBDataAdpterObject.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                                dtuser = ds.Tables[0];
                        }
                    }
                    else
                        driverID = "";
                }
                else
                {
                    driverID = "";
                }

                #region driver_mst
                if (drivermstData != null)
                {
                    try
                    {

                        DataTable dtdrivermst = new DataTable();
                        dtdrivermst = GetDriverDetailsTableById(driverID);

                        if (dtdrivermst == null)
                        {
                            driverID = re.ToString();
                        }
                        else
                            driverID = dtdrivermst.Rows[0]["driver_id"].ToString();

                        // DS_Owner_Mst.driver_mstRow objOwnerDriver = objownerDriver.driver_mst.Newdriver_mstRow();

                        //objownerDriver.driver_mst = Parameter["driver_mst"].ToObject<DS_Owner_Mst.driver_mstDataTable>();

                        if (dtdriverMst.Rows[0]["dob"].ToString() == "")
                            dtdriverMst.Rows[0]["dob"] = DBNull.Value;
                        else if (dtdriverMst.Rows[0]["dob"].ToString() != "")
                            dtdriverMst.Rows[0]["dob"] = dtdriverMst.Rows[0]["dob"].ToString();


                        objownerDriver.EnforceConstraints = false;
                        objownerDriver.driver_mst.ImportRow(dtdriverMst.Rows[0]);
                        objownerDriver.driver_mst[0].driver_id = driverID;
                        objownerDriver.driver_mst[0].reg_date = System.DateTime.UtcNow;
                        objownerDriver.driver_mst[0].Name = drivermstData[0].Name;
                        objownerDriver.driver_mst[0].mobile_no = drivermstData[0].mobile_no;
                        objownerDriver.driver_mst[0].age = drivermstData[0].age;
                        objownerDriver.driver_mst[0].qualification = drivermstData[0].qualification;
                        objownerDriver.driver_mst[0].driver_origin = drivermstData[0].driver_origin;
                        objownerDriver.driver_mst[0].martial_status = drivermstData[0].martial_status;
                        objownerDriver.driver_mst[0].no_of_child = drivermstData[0].no_of_child;
                        objownerDriver.driver_mst[0].health_issues = drivermstData[0].health_issues;
                        objownerDriver.driver_mst[0].smoking = drivermstData[0].smoking;
                        objownerDriver.driver_mst[0].alcohol = drivermstData[0].alcohol;
                        objownerDriver.driver_mst[0].legal_history = drivermstData[0].legal_history;
                        objownerDriver.driver_mst[0].commercial_experience = drivermstData[0].commercial_experience;
                        objownerDriver.driver_mst[0].active_flag = drivermstData[0].active_flag;
                        objownerDriver.driver_mst[0].created_by = drivermstData[0].created_by;
                        objownerDriver.driver_mst[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.driver_mst[0].created_host = "1111";
                        objownerDriver.driver_mst[0].device_id = drivermstData[0].device_id;
                        objownerDriver.driver_mst[0].device_type = drivermstData[0].device_type;
                        objownerDriver.driver_mst[0].isfree = Constant.FLAG_Y;
                        objownerDriver.driver_mst[0].IsOnDuty = Constant.FLAG_Y;

                        //objownerDriver.driver_mst[0].SetAdded();
                        //objownerDriver.driver_mst[0].AcceptChanges();

                        objownerDriver.EnforceConstraints = true;
                        //objownerDriver.driver_mst.Adddriver_mstRow(objownerDriver.driver_mst[0]);

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Master - " + ex.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region User_mst

                if (usermstData != null)
                {
                    try
                    {
                        if (dtuser.Rows.Count == 0)
                            driverID = re.ToString();
                        else
                            driverID = dtuser.Rows[0]["unique_id"].ToString();

                        objownerDriver.EnforceConstraints = false;
                        //DS_Owner_Mst.user_mstRow objusermst = objownerDriver.user_mst.Newuser_mstRow();
                        objownerDriver.user_mst.ImportRow(dsusermst.Tables[0].Rows[0]);
                        objownerDriver.user_mst[0].user_id = usermstData[0].user_id;
                        objownerDriver.user_mst[0].role_id = "DR";
                        objownerDriver.user_mst[0].client_type = "M";
                        objownerDriver.user_mst[0].first_name = usermstData[0].first_name;
                        objownerDriver.user_mst[0].email_id = usermstData[0].email_id;
                        objownerDriver.user_mst[0].password = usermstData[0].password;
                        objownerDriver.user_mst[0].start_date = System.DateTime.UtcNow.Date;
                        objownerDriver.user_mst[0].end_date = System.DateTime.UtcNow.AddYears(10);
                        objownerDriver.user_mst[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.user_mst[0].pass_expiry_date = System.DateTime.UtcNow.AddYears(10);
                        objownerDriver.user_mst[0].login_levels = 1;
                        objownerDriver.user_mst[0].user_status_flag = "A";
                        objownerDriver.user_mst[0].user_loc_flag = "L";
                        objownerDriver.user_mst[0].OTP = "123";
                        objownerDriver.user_mst[0].created_by = usermstData[0].created_by;
                        objownerDriver.user_mst[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.user_mst[0].created_host = "ADMIN";

                        //objownerDriver.user_mst[0].SetAdded();
                        //objownerDriver.user_mst[0].AcceptChanges();
                        objownerDriver.EnforceConstraints = true;
                        // objownerDriver.user_mst.Adduser_mstRow(objownerDriver.user_mst[0]);

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("User Master - " + ex.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver_mobile_detail
                if (driverMobilesData != null)
                {
                    if (Parameter["driver_mobile_detail"] != null)
                    {

                        DataTable dtdrivermobiledetail = new DataTable();
                        dtdrivermobiledetail = GetDrivermobiledetails(driverID);

                        if (dtdrivermobiledetail == null)
                            driverID = re.ToString();
                        else
                            driverID = dtdrivermobiledetail.Rows[0]["driver_id"].ToString();

                        try
                        {
                            objownerDriver.EnforceConstraints = false;
                            //DS_Owner_Mst.driver_mobile_detailRow objOwnerDriverMobile = objownerDriver.driver_mobile_detail.Newdriver_mobile_detailRow();
                            objownerDriver.driver_mobile_detail.ImportRow(dsdrvmobdtl.Tables[0].Rows[0]);
                            objownerDriver.driver_mobile_detail[0].driver_id = driverID;
                            objownerDriver.driver_mobile_detail[0].phone_model = driverMobilesData[0].phone_model;
                            objownerDriver.driver_mobile_detail[0].current_network = driverMobilesData[0].current_network;
                            objownerDriver.driver_mobile_detail[0].consume_3G_Data = driverMobilesData[0].consume_3G_Data;
                            objownerDriver.driver_mobile_detail[0].active_flag = "Y";
                            objownerDriver.driver_mobile_detail[0].created_by = driverMobilesData[0].created_by;
                            objownerDriver.driver_mobile_detail[0].created_date = System.DateTime.UtcNow;
                            objownerDriver.driver_mobile_detail[0].created_host = "1111";
                            objownerDriver.driver_mobile_detail[0].device_id = driverMobilesData[0].device_id;
                            objownerDriver.driver_mobile_detail[0].device_type = driverMobilesData[0].device_type;


                            //objownerDriver.driver_mobile_detail.Adddriver_mobile_detailRow(objownerDriver.driver_mobile_detail[0]);
                            //objownerDriver.driver_mobile_detail[0].SetAdded();
                            //objownerDriver.driver_mobile_detail[0].AcceptChanges();
                            objownerDriver.EnforceConstraints = true;
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("Driver Mobile Detail - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                            return null;
                        }
                    }
                }
                #endregion

                #region driver_language_detail
                if (driverlanguagedetail.Count != 0)
                {
                    try
                    {
                        if (Parameter["driver_language_detail"].SelectToken("language_data") != null)
                        {
                            var param = Parameter["driver_language_detail"].SelectToken("language_data");
                            if (param.HasValues)
                            {
                                bool already = false; string langid = "";
                                for (int j = 0; j < param.Count(); j++)
                                {
                                    already = false;
                                    langid = param[j].SelectToken("language_id").ToString();
                                    for (int i = 0; i < objownerDriver.driver_language_detail.Rows.Count; i++)
                                    {
                                        if (langid == objownerDriver.driver_language_detail[i]["language_id"].ToString())
                                        {
                                            already = true;
                                            break;
                                        }
                                    }
                                    if (already == false)
                                    {
                                        DS_Owner_Mst.driver_language_detailRow objOwnerDriverLang = objownerDriver.driver_language_detail.Newdriver_language_detailRow();
                                        objOwnerDriverLang.driver_id = "111";
                                        objOwnerDriverLang.language_id = param[j].SelectToken("language_id").ToString();
                                        objOwnerDriverLang.can_speak = param[j].SelectToken("Speak").ToString();
                                        objOwnerDriverLang.can_read = param[j].SelectToken("Read").ToString();
                                        objOwnerDriverLang.can_write = param[j].SelectToken("Write").ToString();
                                        objOwnerDriverLang.active_flag = Parameter["driver_language_detail"].SelectToken("active_flag").ToString();
                                        objOwnerDriverLang.created_by = Parameter["driver_language_detail"].SelectToken("created_by").ToString();
                                        objOwnerDriverLang.created_date = System.DateTime.UtcNow;
                                        objOwnerDriverLang.created_host = "1111";
                                        objOwnerDriverLang.device_id = Parameter["driver_language_detail"].SelectToken("device_id").ToString();
                                        objOwnerDriverLang.device_type = Parameter["driver_language_detail"].SelectToken("device_type").ToString();

                                        objownerDriver.driver_language_detail.Adddriver_language_detailRow(objOwnerDriverLang);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Language Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }

                #endregion

                #region driver_insurance_detail
                if (driverInsuranceData.Count != 0)
                {

                    DataTable dtdriverinsurancedetail = new DataTable();
                    dtdriverinsurancedetail = GetDriverinsurancedetails(driverID);

                    if (dtdriverinsurancedetail == null)
                        driverID = re.ToString();
                    else
                        driverID = dtdriverinsurancedetail.Rows[0]["driver_id"].ToString();


                    try
                    {
                        objownerDriver.EnforceConstraints = false;
                        //DS_Owner_Mst.driver_insurance_detailRow objOwnerDriverInsurance = objownerDriver.driver_insurance_detail.Newdriver_insurance_detailRow();
                        objownerDriver.driver_insurance_detail.ImportRow(dsdrvinsdtl.Tables[0].Rows[0]);
                        objownerDriver.driver_insurance_detail[0].driver_id = driverID;
                        objownerDriver.driver_insurance_detail[0].insurance_policy_no = driverInsuranceData[0].insurance_policy_no;
                        objownerDriver.driver_insurance_detail[0].insurance_details = driverInsuranceData[0].insurance_details;
                        objownerDriver.driver_insurance_detail[0].active_flag = driverInsuranceData[0].active_flag;
                        objownerDriver.driver_insurance_detail[0].status = driverInsuranceData[0].status;
                        objownerDriver.driver_insurance_detail[0].created_by = driverInsuranceData[0].created_by;
                        objownerDriver.driver_insurance_detail[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.driver_insurance_detail[0].created_host = "1111";
                        objownerDriver.driver_insurance_detail[0].device_id = driverInsuranceData[0].device_id;
                        objownerDriver.driver_insurance_detail[0].device_type = driverInsuranceData[0].device_type;


                        //objownerDriver.driver_insurance_detail.Adddriver_insurance_detailRow(objownerDriver.driver_insurance_detail[0]);
                        //objownerDriver.driver_insurance_detail[0].SetAdded();
                        //objownerDriver.driver_insurance_detail[0].AcceptChanges();

                        objownerDriver.EnforceConstraints = true;
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Insurance Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver identification details
                if (Parameter["driver_identification_detail"] != null)
                {
                    //DataTable dtdrvdentificationdetail = new DataTable();
                    //dtdrvdentificationdetail = GetDriverinsurancedetails(driverID);

                    //if (dtdrvdentificationdetail == null)
                    //    driverID = re.ToString();
                    //else
                    //    driverID = dtdrvdentificationdetail.Rows[0]["driver_id"].ToString();


                    try
                    {
                        //var param1 = Parameter["driver_identification_detail"];
                        //for (int k = 0; k < param1.Count(); k++)
                        //{
                        //DS_Owner_Mst.driver_identification_detailRow objidrow = objownerDriver.driver_identification_detail.Newdriver_identification_detailRow();

                        //objownerDriver.driver_identification_detail.ImportRow(dsdrvinsdtl.Tables[0].Rows[0]);
                        //objownerDriver.driver_identification_detail[0].driver_id = driverID;
                        //objownerDriver.driver_identification_detail[0].identification_id = param1[k].SelectToken("identification_id").ToString();
                        //objownerDriver.driver_identification_detail[0].id_no = param1[k].SelectToken("id_no").ToString();
                        //if (param1[k].SelectToken("id_valid_from").ToString() != "")
                        //    objownerDriver.driver_identification_detail[0].id_valid_from = Convert.ToDateTime(param1[k].SelectToken("id_valid_from"));
                        //if (param1[k].SelectToken("id_valid_upto").ToString() != "")
                        //    objownerDriver.driver_identification_detail[0].id_valid_upto = Convert.ToDateTime(param1[k].SelectToken("id_valid_upto"));
                        //objownerDriver.driver_identification_detail[0].active_flag = param1[k].SelectToken("active_flag").ToString();
                        //objownerDriver.driver_identification_detail[0].status = param1[k].SelectToken("status").ToString();
                        //objownerDriver.driver_identification_detail[0].created_by = param1[k].SelectToken("created_by").ToString();
                        //objownerDriver.driver_identification_detail[0].created_date = System.DateTime.UtcNow;
                        //objownerDriver.driver_identification_detail[0].created_host = "111";
                        //objownerDriver.driver_identification_detail[0].device_id = param1[k].SelectToken("device_id").ToString();
                        //objownerDriver.driver_identification_detail[0].device_type = param1[k].SelectToken("device_type").ToString();

                        //objownerDriver.driver_identification_detail.Adddriver_identification_detailRow(objidrow);
                        //}
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Identification Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver_contact_detail
                if (driverContactDetails.Count != 0)
                {
                    DataTable dtdriverContactdetail = new DataTable();
                    dtdriverContactdetail = GetDriverContectdetails(driverID);

                    if (dtdriverContactdetail == null)
                        driverID = re.ToString();
                    else
                        driverID = dtdriverContactdetail.Rows[0]["driver_id"].ToString();

                    if (dtdrvcntcdtl.Rows[0]["emirates_id_exp_date"].ToString() == "")
                        dtdrvcntcdtl.Rows[0]["emirates_id_exp_date"] = DBNull.Value;
                    else if (dtdrvcntcdtl.Rows[0]["emirates_id_exp_date"].ToString() != "")
                        dtdrvcntcdtl.Rows[0]["emirates_id_exp_date"] = dtdrvcntcdtl.Rows[0]["emirates_id_exp_date"].ToString();

                    try
                    {
                        // DS_Owner_Mst.driver_contact_detailRow objOwnerDriverContact = objownerDriver.driver_contact_detail.Newdriver_contact_detailRow();
                        objownerDriver.EnforceConstraints = false;
                        objownerDriver.driver_contact_detail.ImportRow(dtdrvcntcdtl.Rows[0]);
                        objownerDriver.driver_contact_detail[0].driver_id = driverID;
                        objownerDriver.driver_contact_detail[0].addr_id = 1;
                        objownerDriver.driver_contact_detail[0].address = driverContactDetails[0].address;
                        objownerDriver.driver_contact_detail[0].state = driverContactDetails[0].state;
                        objownerDriver.driver_contact_detail[0].pincode = driverContactDetails[0].pincode;
                        objownerDriver.driver_contact_detail[0].phone_no = driverContactDetails[0].phone_no;
                        objownerDriver.driver_contact_detail[0].mobile_no = driverContactDetails[0].mobile_no;
                        objownerDriver.driver_contact_detail[0].nationality = driverContactDetails[0].nationality;
                        objownerDriver.driver_contact_detail[0].emirates_id = driverContactDetails[0].emirates_id;
                        objownerDriver.driver_contact_detail[0].active_flag = driverContactDetails[0].active_flag;
                        objownerDriver.driver_contact_detail[0].created_by = driverContactDetails[0].created_by;
                        objownerDriver.driver_contact_detail[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.driver_contact_detail[0].created_host = "1111";
                        objownerDriver.driver_contact_detail[0].device_id = driverContactDetails[0].device_id;
                        objownerDriver.driver_contact_detail[0].device_type = driverContactDetails[0].device_type;


                        //objownerDriver.driver_contact_detail.Adddriver_contact_detailRow(objownerDriver.driver_contact_detail[0]);
                        //objownerDriver.driver_contact_detail[0].SetAdded();
                        //objownerDriver.driver_contact_detail[0].AcceptChanges();
                        objownerDriver.EnforceConstraints = true;

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Contact Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver_prefered_destination
                if (driverPreferedDestinationData != null)
                {
                    DataTable dtdriverprefDest = new DataTable();
                    dtdriverprefDest = GetDriverPreferedDestination(driverID);

                    if (dtdriverprefDest == null)
                        driverID = re.ToString();
                    else
                        driverID = dtdriverprefDest.Rows[0]["driver_id"].ToString();



                    try
                    {
                        objownerDriver.EnforceConstraints = false;
                        // DS_Owner_Mst.driver_prefered_destinationRow objOwnerDriverPreDestination = objownerDriver.driver_prefered_destination.Newdriver_prefered_destinationRow();

                        objownerDriver.driver_prefered_destination.ImportRow(dsdrvcntcdtl.Tables[0].Rows[0]);
                        objownerDriver.driver_prefered_destination[0].driver_id = driverID;
                        objownerDriver.driver_prefered_destination[0].destination_id = driverPreferedDestinationData[0].destination_id;
                        objownerDriver.driver_prefered_destination[0].state = driverPreferedDestinationData[0].state;
                        objownerDriver.driver_prefered_destination[0].active_flag = driverPreferedDestinationData[0].active_flag;
                        objownerDriver.driver_prefered_destination[0].created_by = driverPreferedDestinationData[0].created_by;
                        objownerDriver.driver_prefered_destination[0].created_date = System.DateTime.UtcNow;
                        objownerDriver.driver_prefered_destination[0].created_host = "1111";
                        objownerDriver.driver_prefered_destination[0].device_id = driverPreferedDestinationData[0].device_id;
                        objownerDriver.driver_prefered_destination[0].device_type = driverPreferedDestinationData[0].device_type;

                        //objownerDriver.driver_prefered_destination.Adddriver_prefered_destinationRow(objownerDriver.driver_prefered_destination[0]);
                        //objownerDriver.driver_prefered_destination[0].SetAdded();
                        //objownerDriver.driver_prefered_destination[0].AcceptChanges();
                        objownerDriver.EnforceConstraints = true;
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Prefered Destination - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver License details
                if (driverlicensedetail.Count != 0)
                {
                    if (Parameter["driver_license_detail"] != null)
                    {

                        DataTable dtdriverlicdtl = new DataTable();
                        dtdriverlicdtl = GetDriverlicenseDetail(driverID);

                        if (dtdriverlicdtl == null)
                            driverID = re.ToString();
                        else
                            driverID = dtdriverlicdtl.Rows[0]["driver_id"].ToString();

                        if (dtdrvLicdtl.Rows[0]["Valid_from"].ToString() == "")
                            dtdrvLicdtl.Rows[0]["Valid_from"] = DBNull.Value;
                        else if (dtdrvLicdtl.Rows[0]["Valid_from"].ToString() != "")
                            dtdrvLicdtl.Rows[0]["Valid_from"] = dtdrvLicdtl.Rows[0]["Valid_from"].ToString();

                        if (dtdrvLicdtl.Rows[0]["Valid_upto"].ToString() == "")
                            dtdrvLicdtl.Rows[0]["Valid_upto"] = DBNull.Value;
                        else if (dtdrvLicdtl.Rows[0]["Valid_upto"].ToString() != "")
                            dtdrvLicdtl.Rows[0]["Valid_upto"] = dtdrvLicdtl.Rows[0]["Valid_upto"].ToString();

                        try
                        {
                            objownerDriver.EnforceConstraints = false;
                            // DS_Owner_Mst.driver_license_detailRow objlicenserow = objownerDriver.driver_license_detail.Newdriver_license_detailRow();
                            objownerDriver.driver_license_detail.ImportRow(dtdrvLicdtl.Rows[0]);
                            objownerDriver.driver_license_detail[0].driver_id = driverID;
                            objownerDriver.driver_license_detail[0].License_no = driverlicensedetail[0].License_no;

                            objownerDriver.driver_license_detail[0].Valid_from = System.DateTime.UtcNow;
                            //if (driverlicensedetail[0].Valid_upto != null)
                            //    objownerDriver.driver_license_detail[0].Valid_upto = Convert.ToDateTime(dsdrvlicedtl.Tables[0].Rows[0]["Valid_upto"].ToString());
                            objownerDriver.driver_license_detail[0].active_flag = Constant.FLAG_Y;
                            objownerDriver.driver_license_detail[0].status = driverlicensedetail[0].status;
                            objownerDriver.driver_license_detail[0].created_by = driverlicensedetail[0].created_by;
                            objownerDriver.driver_license_detail[0].created_date = System.DateTime.UtcNow;
                            objownerDriver.driver_license_detail[0].created_host = "111";
                            objownerDriver.driver_license_detail[0].device_id = driverlicensedetail[0].device_id;
                            objownerDriver.driver_license_detail[0].device_type = driverlicensedetail[0].device_type;

                            //objownerDriver.driver_license_detail.Adddriver_license_detailRow(objownerDriver.driver_license_detail[0]);
                            //objownerDriver.driver_license_detail[0].SetAdded();
                            //objownerDriver.driver_license_detail[0].AcceptChanges();
                            objownerDriver.EnforceConstraints = true;
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("Driver License Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                            return null;
                        }
                    }
                }
                #endregion

                #region driver Bank details
                if (bankdetail.Count != 0)
                {
                    if (Parameter["driver_bank_detail"] != null)
                    {
                        DataTable dtdriverbnkdtl = new DataTable();
                        dtdriverbnkdtl = GetDriverlicenseDetail(driverID);

                        if (dtdriverbnkdtl == null)
                            driverID = re.ToString();
                        else
                            driverID = dtdriverbnkdtl.Rows[0]["driver_id"].ToString();


                        try
                        {
                            objownerDriver.EnforceConstraints = false;
                            // DS_Owner_Mst.driver_bank_detailRow bankrow = objownerDriver.driver_bank_detail.Newdriver_bank_detailRow();
                            objownerDriver.driver_bank_detail.ImportRow(dsdrvbnkdtl.Tables[0].Rows[0]);
                            objownerDriver.driver_bank_detail[0].driver_id = driverID;
                            objownerDriver.driver_bank_detail[0].bank_name = bankdetail[0].bank_name;
                            objownerDriver.driver_bank_detail[0].atm = bankdetail[0].atm;
                            objownerDriver.driver_bank_detail[0].ETransfer = bankdetail[0].ETransfer;
                            objownerDriver.driver_bank_detail[0].active_flag = bankdetail[0].active_flag;
                            objownerDriver.driver_bank_detail[0].created_by = bankdetail[0].created_by;
                            objownerDriver.driver_bank_detail[0].created_date = System.DateTime.UtcNow;
                            objownerDriver.driver_bank_detail[0].created_host = "111";
                            objownerDriver.driver_bank_detail[0].device_id = bankdetail[0].device_id;
                            objownerDriver.driver_bank_detail[0].device_type = bankdetail[0].device_type;

                            //objownerDriver.driver_bank_detail.Adddriver_bank_detailRow(objownerDriver.driver_bank_detail[0]);

                            //objownerDriver.driver_bank_detail[0].SetAdded();
                            //objownerDriver.driver_bank_detail[0].AcceptChanges();
                            objownerDriver.EnforceConstraints = true;
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("Driver Bank Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                            return null;
                        }
                    }
                }
                #endregion

                #region Owner Driver Detail
                //if (driverowner != null)
                //{
                //    if (Parameter["driver_owner"] != null)
                //    {
                //        try
                //        {
                //            DS_Owner_Mst.owner_driver_detailsRow ownrow = objownerDriver.owner_driver_details.Newowner_driver_detailsRow();
                //            ownrow.driver_id = "111";
                //            ownrow.owner_id = driverowner.owner_id;
                //            ownrow.active_flag = drivermstData.active_flag;
                //            ownrow.created_by = drivermstData.created_by;
                //            ownrow.created_date = System.DateTime.UtcNow;
                //            ownrow.created_host = "111";
                //            ownrow.device_id = drivermstData.device_id;
                //            ownrow.device_type = drivermstData.device_type;
                //            objownerDriver.owner_driver_details.Addowner_driver_detailsRow(ownrow);
                //        }
                //        catch (Exception ex)
                //        {
                //            ServerLog.Log("Driver Owner Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                //            return null;
                //        }
                //    }
                //}
                #endregion

                #region Truck Driver Relation
                if (drivertruck.Count != 0)
                {
                    if (Parameter["driver_truck_detail"] != null)
                    {

                        DataTable dtdriverTruckdetail = new DataTable();
                        dtdriverTruckdetail = GetDriverTruckDetailsByDrvID(driverID);

                        if (dtdriverTruckdetail == null)
                        {
                            driverID = re.ToString();
                            objownerDriver.EnforceConstraints = false;
                            //DS_Owner_Mst.driver_truck_detailsRow drtrkrow = objownerDriver.driver_truck_details.Newdriver_truck_detailsRow();
                            objownerDriver.driver_truck_details.ImportRow(dsdrvtruckdtl.Tables[0].Rows[0]);
                        }
                        else
                        {
                            driverID = dtdriverTruckdetail.Rows[0]["driver_id"].ToString();
                            objownerDriver.EnforceConstraints = false;
                            //DS_Owner_Mst.driver_truck_detailsRow drtrkrow = objownerDriver.driver_truck_details.Newdriver_truck_detailsRow();
                            objownerDriver.driver_truck_details.ImportRow(dtdriverTruckdetail.Rows[0]);
                            objownerDriver.driver_truck_details[0].truck_id = drivertruck[0].truck_id;
                        }
                        try
                        {
                            //objownerDriver.EnforceConstraints = false;
                            ////DS_Owner_Mst.driver_truck_detailsRow drtrkrow = objownerDriver.driver_truck_details.Newdriver_truck_detailsRow();
                            //objownerDriver.driver_truck_details.ImportRow(dsdrvtruckdtl.Tables[0].Rows[0]);
                            objownerDriver.driver_truck_details[0].driver_id = driverID;
                            objownerDriver.driver_truck_details[0].active_flag = drivertruck[0].active_flag;
                            objownerDriver.driver_truck_details[0].created_by = drivertruck[0].created_by;
                            objownerDriver.driver_truck_details[0].created_date = System.DateTime.UtcNow;
                            objownerDriver.driver_truck_details[0].created_host = "111";
                            objownerDriver.driver_truck_details[0].device_id = drivertruck[0].device_id;
                            objownerDriver.driver_truck_details[0].device_type = drivertruck[0].device_type;
                            objownerDriver.EnforceConstraints = true;
                            //objownerDriver.driver_truck_details.Adddriver_truck_detailsRow(objownerDriver.driver_truck_details[0]);
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("Driver Truck Relation - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                            return null;
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + " = " + ex.StackTrace);
                //ServerLog.Log("Post_SaveOwnerDetails - " + ex.InnerException.Message.ToString());
                //ServerLog.Log(ex.InnerException.Message.ToString());
                return null;
            }
            return objownerDriver;
        }


        #region Update driver Status by Admin

        [HttpGet]
        public String UpdateDriverStatusByAdmin(String load_inquiry_no, String driver_id, String truck_id, String status)
        {

            try
            {
                Master master = new Master();
                BLReturnObject objBLobj = new BLReturnObject();
                string Invoicelink = ConfigurationManager.AppSettings["InvoicePdfLink"].ToString();

                DataTable dtdriverongoingorder = GetdriverOngoingOrder(driver_id);
                if (dtdriverongoingorder != null)
                {
                    if (dtdriverongoingorder.Rows.Count > 0)
                        if (dtdriverongoingorder.Rows[0]["load_inquiry_no"].ToString() != load_inquiry_no)
                            return BLGeneralUtil.return_ajax_string("0", " Driver already busy in order cannot pickup for another one ");
                }

                DataTable dt_post_load_inquiry = new PostOrderController().GetLoadInquiryById(load_inquiry_no);
                DataTable dtdriverdetails = GetDriverDetailsTableById(driver_id); // check driver is in database 

                int index; string TruckLat = ""; string TruckLng = "";
                String Message = String.Empty, user_id = "ADMIN", host_id = String.Empty, device_id = String.Empty, device_type = String.Empty;
                //Get order driver truck details.
                DataTable dtOrder_driver_truck_details = GetOrder_driver_truck_details(load_inquiry_no, driver_id, truck_id, ref Message);
                if (dtOrder_driver_truck_details == null || dtOrder_driver_truck_details.Rows.Count <= 0)
                    return (BLGeneralUtil.return_ajax_string("0", Message));

                DataTable dtTruck_current_position = new DataTable();
                DataTable dtTruckCurrentPosition = new DataTable();
                //Get truck/driver current latitude and longitude
                dtTruck_current_position = GetTruck_current_position(load_inquiry_no, driver_id, truck_id, Constant.Flag_Yes, ref Message);
                if (dtTruck_current_position == null || dtTruck_current_position.Rows.Count <= 0)
                {
                    dtTruckCurrentPosition = GetTruckCurrentPosition(driver_id, truck_id, Constant.Flag_Yes, ref Message);
                    if (dtTruckCurrentPosition != null && dtTruckCurrentPosition.Rows.Count >= 0)
                    {
                        DateTime dtlogdate = Convert.ToDateTime(dtTruckCurrentPosition.Rows[0]["log_date"].ToString());
                        if (dtlogdate.Date == System.DateTime.UtcNow.Date)
                        {
                            dtTruck_current_position = dtTruckCurrentPosition;
                            TruckLat = dtTruck_current_position.Rows[0]["truck_lat"].ToString();
                            TruckLng = dtTruck_current_position.Rows[0]["truck_lng"].ToString();
                        }
                        else
                        {
                            dtTruckCurrentPosition.Rows[0]["truck_lat"] = dt_post_load_inquiry.Rows[0]["inquiry_source_lat"].ToString();
                            dtTruckCurrentPosition.Rows[0]["truck_lng"] = dt_post_load_inquiry.Rows[0]["inquiry_source_lng"].ToString();

                            TruckLat = dt_post_load_inquiry.Rows[0]["inquiry_source_lat"].ToString();
                            TruckLng = dt_post_load_inquiry.Rows[0]["inquiry_source_lng"].ToString();

                            dtTruck_current_position = dtTruckCurrentPosition;
                        }
                    }
                    else
                    {
                        TruckLat = dt_post_load_inquiry.Rows[0]["inquiry_source_lat"].ToString();
                        TruckLng = dt_post_load_inquiry.Rows[0]["inquiry_source_lng"].ToString();

                        // return (BLGeneralUtil.return_ajax_string("0", Message));
                    }
                }
                else
                {
                    TruckLat = dtTruck_current_position.Rows[0]["truck_lat"].ToString();
                    TruckLng = dtTruck_current_position.Rows[0]["truck_lng"].ToString();
                }

                #region Set driver Status in table : order_driver_truck_details

                DS_driver_order_notifications dS_driver_order_notifications = new DS_driver_order_notifications();
                dS_driver_order_notifications.EnforceConstraints = false;
                dS_driver_order_notifications.order_driver_truck_details.ImportRow(dtOrder_driver_truck_details.Rows[0]);
                index = dS_driver_order_notifications.order_driver_truck_details.Rows.Count - 1;
                dS_driver_order_notifications.order_driver_truck_details[index].status = status;

                if (status.Trim() == Constant.TRUCK_READY_FOR_PICKUP) //"05" - Status by Driver Order Completed
                {
                    dS_driver_order_notifications.order_driver_truck_details[index].pickup_lat = TruckLat;
                    dS_driver_order_notifications.order_driver_truck_details[index].pickup_lng = TruckLng;
                    dS_driver_order_notifications.order_driver_truck_details[index].pickup_time = DateTime.UtcNow;
                    dS_driver_order_notifications.order_driver_truck_details[index].pickup_by = user_id;
                }
                else if (status.Trim() == Constant.lOADING_STARTED) //"06" - Status by Driver Order Completed
                {
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_by = user_id;

                    dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lat = TruckLat;
                    dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lng = TruckLng;
                    dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_time = DateTime.UtcNow;
                    dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_by = user_id;
                }
                else if (status.Trim() == Constant.START)//"07" - Status by Driver Order Completed
                {
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_by = user_id;

                    dS_driver_order_notifications.order_driver_truck_details[index].start_lat = TruckLat;
                    dS_driver_order_notifications.order_driver_truck_details[index].start_lng = TruckLng;
                    dS_driver_order_notifications.order_driver_truck_details[index].start_time = DateTime.UtcNow;
                    dS_driver_order_notifications.order_driver_truck_details[index].start_by = user_id;
                }
                else if (status.Trim() == Constant.UNLOADING_START)//"08" - Status by Driver Order Completed
                {
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_by = user_id;

                    dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_lat = TruckLat;
                    dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_lng = TruckLng;
                    dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_time = DateTime.UtcNow;
                    dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_by = user_id;
                }
                else if (status.Trim() == Constant.UNLOADING_COMPLETED)//"45" - Status by Driver Order Completed
                {
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Ispickup_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].pickup_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isloadingstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].loadingstart_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].start_by = user_id;

                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isunloadingstart_latNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_lat = TruckLat;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isunloadingstart_lngNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_lng = TruckLng;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isunloadingstart_timeNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_time = DateTime.UtcNow;
                    if (dS_driver_order_notifications.order_driver_truck_details[index].Isunloadingstart_byNull())
                        dS_driver_order_notifications.order_driver_truck_details[index].unloadingstart_by = user_id;

                    dS_driver_order_notifications.order_driver_truck_details[index].complete_lat = TruckLat;
                    dS_driver_order_notifications.order_driver_truck_details[index].complete_lng = TruckLng;
                    dS_driver_order_notifications.order_driver_truck_details[index].complete_time = DateTime.UtcNow;
                    dS_driver_order_notifications.order_driver_truck_details[index].complete_by = user_id;
                }

                dS_driver_order_notifications.order_driver_truck_details[index].modified_by = user_id;
                dS_driver_order_notifications.order_driver_truck_details[index].modified_date = DateTime.UtcNow;
                dS_driver_order_notifications.order_driver_truck_details[index].modified_host = host_id;
                dS_driver_order_notifications.order_driver_truck_details[index].modified_device_id = device_id;
                dS_driver_order_notifications.order_driver_truck_details[index].modified_device_type = device_type;
                #endregion

                #region Move data truck_current_position => truck_current_position_history
                DS_Truck_current_location dS_Truck_current_location = null;
                if (status.Trim() == Constant.UNLOADING_COMPLETED)//"45" - Status by Driver Order Completed
                {
                    DataTable dtTruck_current_position_history = GetTruck_current_position(load_inquiry_no, driver_id, truck_id, null, ref Message);
                    if (dtTruck_current_position_history != null)
                    {                        //return (BLGeneralUtil.return_ajax_string("0", Message));
                        if (dtTruck_current_position_history.Rows.Count >= 0)
                        {
                            dS_Truck_current_location = new DS_Truck_current_location();
                            dS_Truck_current_location.EnforceConstraints = false;
                            foreach (DataRow dr in dtTruck_current_position_history.Rows)
                            {
                                dS_Truck_current_location.truck_current_position_history.ImportRow(dr);
                                index = dS_Truck_current_location.truck_current_position_history.Rows.Count - 1;
                                dS_Truck_current_location.truck_current_position_history[index].AcceptChanges();
                                dS_Truck_current_location.truck_current_position_history[index].SetAdded();

                                dS_Truck_current_location.truck_current_position.ImportRow(dr);
                                index = dS_Truck_current_location.truck_current_position.Rows.Count - 1;
                                dS_Truck_current_location.truck_current_position[index].AcceptChanges();
                                dS_Truck_current_location.truck_current_position[index].Delete();
                            }
                        }
                    }
                }
                #endregion

                try
                {
                    dS_driver_order_notifications.EnforceConstraints = true;
                    if (status.Trim() == Constant.UNLOADING_COMPLETED)//"45" - Status by Driver Order Completed
                        if (dS_Truck_current_location != null)
                            dS_Truck_current_location.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return (BLGeneralUtil.return_ajax_string("0", Message));
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return (BLGeneralUtil.return_ajax_string("0", Message));
                }

                if (DBConnection.State == ConnectionState.Open)
                    DBConnection.Close();
                //Establish DataBase Connection.
                DBConnection.Open();
                //BeginTransaction
                DBCommand.Transaction = DBConnection.BeginTransaction();

                BLGeneralUtil.UpdateTableInfo utf = BLGeneralUtil.UpdateTable(ref DBCommand, dS_driver_order_notifications.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (!utf.Status || utf.TotalRowsAffected != dS_driver_order_notifications.order_driver_truck_details.Rows.Count)
                {
                    ServerLog.Log(utf.ErrorMessage + Environment.NewLine + utf.ErrorRow + Environment.NewLine + utf.ErrorSQLStatement);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", "Error while update rows in table : order_driver_truck_details for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id);
                }

                if (status.Trim() == Constant.UNLOADING_COMPLETED)//"45" - Status by Driver Order Completed
                {
                    if (dS_Truck_current_location != null)
                    {
                        if (dS_Truck_current_location.truck_current_position_history.Rows.Count > 0)
                        {
                            utf = BLGeneralUtil.UpdateTable(ref DBCommand, dS_Truck_current_location.truck_current_position_history, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                            if (!utf.Status || utf.TotalRowsAffected != dS_Truck_current_location.truck_current_position_history.Rows.Count)
                            {
                                ServerLog.Log(utf.ErrorMessage + Environment.NewLine + utf.ErrorRow + Environment.NewLine + utf.ErrorSQLStatement);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", "Error while update rows in table : truck_current_position_history for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id);
                            }
                        }
                    }

                    if (dS_Truck_current_location != null)
                    {
                        if (dS_Truck_current_location.truck_current_position.Rows.Count > 0)
                        {
                            utf = BLGeneralUtil.UpdateTable(ref DBCommand, dS_Truck_current_location.truck_current_position, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                            if (!utf.Status || utf.TotalRowsAffected != dS_Truck_current_location.truck_current_position.Rows.Count)
                            {
                                ServerLog.Log(utf.ErrorMessage + Environment.NewLine + utf.ErrorRow + Environment.NewLine + utf.ErrorSQLStatement);
                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", "Error while delete rows from table : truck_current_position for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id);
                            }
                        }
                    }
                }



                #region update status in driver_mst

                //if (status == Constant.ALLOCATED_BUT_NOT_STARTE || status == Constant.TRUCK_READY_FOR_PICKUP || status == Constant.UNLOADING_COMPLETED)
                //{
                if (dtdriverdetails != null)
                {
                    DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                    if (dtdriverdetails != null && dtdriverdetails.Rows.Count > 0)
                    {
                        ds_owner.driver_mst.ImportRow(dtdriverdetails.Rows[0]);
                        //if (status == Constant.TRUCK_READY_FOR_PICKUP)
                        //    ds_owner.driver_mst[0].isfree = Constant.Flag_No;
                        //else if (status == Constant.UNLOADING_COMPLETED)
                        //    ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;
                        //else if (status == Constant.ALLOCATED_BUT_NOT_STARTE)
                        //    ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;


                        if (status == Constant.UNLOADING_COMPLETED)
                            ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;
                        else if (status == Constant.ALLOCATED_BUT_NOT_STARTE)
                            ds_owner.driver_mst[0].isfree = Constant.Flag_Yes;
                        else
                            ds_owner.driver_mst[0].isfree = Constant.Flag_No;
                        ds_owner.driver_mst[0].modified_by = "ADMIN";
                        ds_owner.driver_mst[0].modified_date = System.DateTime.UtcNow;

                        objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                        if (objBLobj.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLobj.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                        }
                    }
                    //}
                }
                #endregion


                string orderFinalstatus = null;


                try
                {

                    DataTable dtdrviertruckdetails = GetOrderstatusDetailsByinquiry(load_inquiry_no);
                    if (dtdrviertruckdetails != null && dtdrviertruckdetails.Rows.Count > 0)
                    {
                        orderFinalstatus = dtdrviertruckdetails.Rows[0]["status"].ToString();
                    }

                    DataTable dt_order = new PostOrderController().GetLoadInquiryById(load_inquiry_no);

                    #region Update Order table status

                    string message = "";
                    DS_orders dsord = new DS_orders();
                    dsord.orders.ImportRow(dt_order.Rows[0]);
                    dsord.orders.Rows[0]["status"] = orderFinalstatus;
                    if (orderFinalstatus != null && orderFinalstatus.Trim() != String.Empty && orderFinalstatus == Constant.ORDER_COMPLETED)
                    {
                        int CreditDays = GetShipperCreditDays(ref DBCommand, dt_order.Rows[0]["shipper_id"].ToString(), ref message);
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

                        ServerLog.SuccessLog("Invoice Start = " + load_inquiry_no);
                        bool bl = new MailerController().GenerateInvoiceMail(load_inquiry_no);
                        if (bl)
                            ServerLog.SuccessLog("Invoice Generated = " + load_inquiry_no);
                        else
                            ServerLog.SuccessLog("Invoice Not Generate = " + load_inquiry_no);

                        string filepath = Invoicelink + "Invoicedetail_" + load_inquiry_no + ".pdf";
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


                    #endregion

                    #region Update post_load_inquiry table status

                    DS_Post_load_enquiry dspostload = new DS_Post_load_enquiry();
                    dspostload.post_load_inquiry.ImportRow(dt_post_load_inquiry.Rows[0]);
                    dspostload.post_load_inquiry.Rows[0]["status"] = status;
                    dspostload.post_load_inquiry.Rows[0]["modified_by"] = "ADMIN";
                    dspostload.post_load_inquiry.Rows[0]["modified_date"] = System.DateTime.UtcNow;
                    dspostload.post_load_inquiry.Rows[0]["modified_host"] = "ADMIN";
                    dspostload.post_load_inquiry.Rows[0]["modified_device_id"] = "ADMIN";
                    dspostload.post_load_inquiry.Rows[0]["modified_device_type"] = "Browser";

                    objBLobj = master.UpdateTables(dspostload.post_load_inquiry, ref DBCommand);
                    if (objBLobj.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLobj.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }


                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return "{\"status\":\"1\",\"message\":\"Driver status updated sucessfully.\",\"FinalStatus\":\"" + orderFinalstatus + "\"}";
                //return BLGeneralUtil.return_ajax_string("1", "Driver status updated sucessfully.");
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        private DataTable GetOrder_driver_truck_details(String load_inquiry_no, String driver_id, String truck_id, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * 
                                   FROM order_driver_truck_details 
                                   WHERE load_inquiry_no=@load_inquiry_no 
                                   AND driver_id=@driver_id 
                                   AND truck_id=@truck_id");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@driver_id", DbType.String, driver_id));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@truck_id", DbType.String, truck_id));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "order_driver_truck_details");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "order_driver_truck_details detail retreived successfully for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "order_driver_truck_details detail not found for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        private DataTable GetTruck_current_position(String load_inquiry_no, String driver_id, String truck_id, String active_flag, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * 
                                   FROM truck_current_position 
                                   WHERE load_inquiry_no=@load_inquiry_no 
                                   AND driver_id=@driver_id 
                                   AND truck_id=@truck_id ");
                if (active_flag != null && active_flag.Trim() != String.Empty)
                {
                    SQLSelect.Append(" AND active_flag=@active_flag ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@active_flag", DbType.String, active_flag));
                }
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@driver_id", DbType.String, driver_id));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@truck_id", DbType.String, truck_id));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "truck_current_position");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "truck_current_position detail retreived successfully for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "truck_current_position detail not found for load_inquiry_no : " + load_inquiry_no + " and driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        private DataTable GetTruckCurrentPosition(String driver_id, String truck_id, String active_flag, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * 
                                   FROM truck_current_position 
                                   WHERE driver_id=@driver_id 
                                   AND truck_id=@truck_id ");
                if (active_flag != null && active_flag.Trim() != String.Empty)
                {
                    SQLSelect.Append(" AND active_flag=@active_flag order by log_date desc ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@active_flag", DbType.String, active_flag));
                }
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@driver_id", DbType.String, driver_id));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@truck_id", DbType.String, truck_id));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "truck_current_position");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "truck_current_position detail retreived successfully for load_inquiry_no :  driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "truck_current_position detail not found for load_inquiry_no :  driver_id : " + driver_id + " and truck_id : " + truck_id;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        #endregion

        #endregion

        public DataTable GetTruckCurrentPositionById(string loadinqid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM truck_current_position Where load_inquiry_no = @inqid ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqid));
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
        // get details by load inquiry number for change driver or truck after order generated
        [HttpGet]
        public DataTable Getdriverordernotification(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM driver_order_notifications where load_inquiry_no = @inqid and active_flag='Y'";
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
        public string GetCompletedOrdersBydriverID(string drvid, string deviceid)
        {

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(drvid);
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != deviceid)
                    {
                        ServerLog.Log(" User login another Device Loged Out " + drvid + ",DeviceID=" + deviceid);
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }

            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = "";

                query1 = " Select distinct user_mst.first_name as shipper_name,user_mst.user_id as shipper_mobile,user_mst.email_id as shipper_email,order_driver_truck_details.*,orders.*,truck_mst.*,driver_license_detail.*,driver_contact_detail.*,GETUTCDATE() as currentdate,    " +
                        " post_load_inquiry.payment_mode " +
                        " from orders " +
                        " left join user_mst on user_mst.unique_id = orders.shipper_id " +
                         "   join order_driver_truck_details on order_driver_truck_details.load_inquiry_no = orders.load_inquiry_no " +
                       "   left join truck_mst on truck_mst.truck_id = order_driver_truck_details.truck_id " +
                       "   left join driver_license_detail on driver_license_detail.driver_id = order_driver_truck_details.driver_id " +
                       "   left join driver_contact_detail on driver_contact_detail.driver_id = order_driver_truck_details.driver_id " +
                       "   left join truck_make_mst on truck_make_mst.make_id = truck_mst.truck_make_id " +
                       "   left join truck_model_mst on truck_model_mst.model_id = truck_mst.truck_model " +
                       "   left join post_load_inquiry on post_load_inquiry.load_inquiry_no = orders.load_inquiry_no " +
                       "   Where orders.active_flag  = 'Y' " +
                       "   and orders.load_inquiry_no not in ( select load_inquiry_no from orders where order_id like '%LO%' ) " + // remove test orders 
                       "   and order_driver_truck_details.driver_id=@drid   and  order_driver_truck_details.status='45' Order by orders.load_inquiry_no desc ";

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
                DBConnection.Close();
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        public DataTable GetOnGoingOrdersByDriver(string drvid)
        {
            try
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = "";
                query1 = " Select distinct user_mst.first_name as shipper_name,user_mst.user_id as shipper_mobile,user_mst.email_id as shipper_email,driver_mst.name as drivername,order_driver_truck_details.*,orders.*,truck_mst.*,driver_license_detail.*,driver_contact_detail.*,getdate() as currentdate,    " +
                        " post_load_inquiry.payment_mode  " +
                        " from orders " +
                        " left join user_mst on user_mst.unique_id = orders.shipper_id " +
                         "   join order_driver_truck_details on order_driver_truck_details.load_inquiry_no = orders.load_inquiry_no " +
                     "   left join truck_mst on truck_mst.truck_id = order_driver_truck_details.truck_id " +
                     "   left join driver_license_detail on driver_license_detail.driver_id = order_driver_truck_details.driver_id " +
                     "   left join driver_contact_detail on driver_contact_detail.driver_id = order_driver_truck_details.driver_id " +
                     "   left join truck_make_mst on truck_make_mst.make_id = truck_mst.truck_make_id " +
                     "   left join truck_model_mst on truck_model_mst.model_id = truck_mst.truck_model " +
                     "   left join post_load_inquiry on post_load_inquiry.load_inquiry_no = orders.load_inquiry_no " +
                      " join driver_mst on order_driver_truck_details.driver_id = driver_mst.driver_id " +
                     "   Where orders.active_flag  = 'Y' and order_driver_truck_details.driver_id=@drid   and CAST(order_driver_truck_details.status AS INT) not in (45,-01)  order by shippingdatetime desc ";

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
                    return dtPostLoadOrders;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DBConnection.Close();
                return null;
            }
        }

        [HttpPost]
        public string UpdateOnduty([FromBody]JObject Parameters_driver)
        {
            DataTable dt_driver_mast = new DataTable();
            DataTable dt_owner_driver_details = new DataTable();
            DataTable dt_owner_driver_tag_detail = new DataTable();
            Master master = new Master();
            BLReturnObject objBLobj = new BLReturnObject();
            List<orders> tord = new List<orders>();
            if (Parameters_driver["UpdateDriver"] != null)
            {
                tord = Parameters_driver["UpdateDriver"].ToObject<List<orders>>();
            }

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(tord[0].driver_id);
            dt_driver_mast = GetDriverDetailsTableById(tord[0].driver_id);
            if (dt_driver_mast == null)
            {
                return (BLGeneralUtil.return_ajax_string("0", " Driver details Not found "));
            }
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                {
                    if (dr[0].ItemArray[3].ToString() != tord[0].modified_device_id)
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
                }
            }

            if (tord[0].IsOnDuty == "Y")
            { }
            else
            {
                if (dt_driver_mast != null)
                {
                    if (dt_driver_mast.Rows[0]["isfree"].ToString() == "N")
                        return (BLGeneralUtil.return_ajax_string("0", "You are busy in order You can’t go for off Duty"));
                }

            }
            try
            {



                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DS_Owner_Mst ds_drivermst = new DS_Owner_Mst();

                #region Update Table owner_driver_details details for

                if (dt_owner_driver_details != null)
                {
                    ds_drivermst.EnforceConstraints = false;
                    ds_drivermst.driver_mst.ImportRow(dt_driver_mast.Rows[0]);
                    ds_drivermst.driver_mst[0].IsOnDuty = tord[0].IsOnDuty;
                    ds_drivermst.driver_mst[0].modified_by = tord[0].modified_by;
                    ds_drivermst.driver_mst[0].modified_date = System.DateTime.UtcNow;
                    ds_drivermst.driver_mst[0].modified_host = tord[0].modified_host;
                    ds_drivermst.driver_mst[0].modified_device_id = tord[0].modified_device_id;
                    ds_drivermst.driver_mst[0].modified_device_type = tord[0].modified_device_type;
                    ds_drivermst.EnforceConstraints = true;
                    objBLobj = master.UpdateTables(ds_drivermst.driver_mst, ref DBCommand);
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

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;
                ServerLog.SuccessLog(" Driver : " + tord[0].driver_id + " METHOD : " + Request.RequestUri.AbsolutePath + " HOST :" + Request.RequestUri.Host);
                return BLGeneralUtil.return_ajax_string("1", "Driver update Sucessfully");

            }
            catch (Exception ex)
            {

                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                ServerLog.Log("Driver Details : " + ex.Message.ToString());
                return ex.Message.ToString();

            }




        }

        [HttpPost]
        public string GetAllDriverForDeshboard([FromBody]JObject Jobjdriver)//string status)
        {

            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = "";
            if (Jobjdriver["driver_deshboard"] != null)
            {
                objOrder = Jobjdriver["driver_deshboard"].ToObject<List<orders>>();
                status = objOrder[0].status;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
            }

            String qury = "";
            DataTable dtdrivers = new DataTable();

            qury = " Select truck_body_mst.truck_body_desc,driver_contact_detail.emirates_id,truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.vehicle_reg_date,driver_license_detail.License_no,driver_mst.* " +
                   "  from driver_mst " +
                   " left join driver_truck_details on driver_truck_details.driver_id =  driver_mst.driver_id " +
                   " left join driver_license_detail on driver_mst.driver_id  = driver_license_detail.driver_id  " +
                   " left join driver_contact_detail on driver_mst.driver_id  = driver_contact_detail.driver_id " +
                   " left join truck_mst on driver_truck_details.truck_id =  truck_mst.truck_id " +
                   " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id  " +
                   " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                   " left join truck_rto_registration_detail on driver_truck_details.truck_id = truck_rto_registration_detail.truck_id   " +
                   " left join truck_permit_details on driver_truck_details.truck_id = truck_permit_details.truck_id   " +
                   " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id " +
                   " where 1=1 ";

            if (status == "free")
            {
                qury += " and  driver_mst.isfree='Y'  ";
            }
            if (status == "busy")
            {
                qury += " and  driver_mst.isfree='N'  ";
            }
            if (status == "" && fromdate.Trim() != "" && Todate.Trim() != "")
            {
                qury += " CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if ((status != "" && fromdate.Trim() != "" && Todate.Trim() != ""))
            {
                qury += " and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (status != "" && fromdate.Trim() != "")
            {
                qury += "and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (status == "" && fromdate.Trim() != "")
            {
                qury += "CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (status != "" && Todate.Trim() != "")
            {
                qury += "and CAST(created_date AS DATE)<='" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (status == "" && Todate.Trim() != "")
            {
                qury += " CAST(created_date AS DATE)<='" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }

            qury += " order by created_date desc ";

            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = qury;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtdrivers = ds.Tables[0];
            }
            DBConnection.Close();
            if (dtdrivers != null && dtdrivers.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrivers));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }


        public DataTable GetdriverOngoingOrder(string driverID)
        {
            String query = " select * from order_driver_truck_details where driver_id = '" + driverID + "' and status not in (02,45) ";

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
        public string GetOrderDriverTruckDetailsById(string loadinqno)
        {
            DataTable dtdrivers = GetOrderDriverTruckDetails(loadinqno);
            if (dtdrivers != null && dtdrivers.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrivers));
            else
                return BLGeneralUtil.return_ajax_string("0", "Drivers Details Not found");
        }

        [HttpGet]
        public string GetAllDriverOrders(string drvid, string deviceid)
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
                //StringBuilder Sb= new StringBuilder();
                string query = "";
                query = "select " +
                         "  case " +
                         "     when result.status not in ('02', '45')  then 'O'  " +
                         "     when result.shippingdatetime<GETDATE() and result.status not in (45) then 'R' " +
                         "     when result.status = '02' and TempFlg='T' then 'G'  " +
                         "     when result.status=45 then 'R' " +
                         "     Else 'W'  " +
                         "  end as color_code,driver_mst.driver_photo,*  " +
                         "  from (  " +
                         "  select temp.*,ROW_NUMBER() OVER (PARTITION BY driver_id ORDER BY OrderKey,driver_id, shippingdatetime) as row_num " +
                         "  ,case when status='02' and shippingdatetime=(select Top(1) OD.shippingdatetime from  orders as OD " +
                         " left outer join order_driver_truck_details as ODT on ODT.load_inquiry_no=OD.load_inquiry_no  " +
                         " where 1=1 and OD.active_flag  = 'Y' and ODT.status=2    and OD.shippingdatetime>getdate()  and ODT.driver_id =@drid order by OD.shippingdatetime) Then 'T' " +
                         " Else 'F' END as TempFlg " +
                         "  from " +
                         "  (select ODT.NoOfHandiman,ODT.NoOfLabour,OD.isassign_driver_truck,OD.TotalDistance,OD.TotalDistanceUOM, " +
                         "  ODT.baseCharge as driver_baseCharge,ODT.handimanCharge as driver_handimanCharge,ODT.labourcharge as driver_labourcharge,ODT.driverCharge as driver_driverCharge," +
                         "  ODT.shipper_baseCharge,ODT.shipper_handimanCharge,ODT.shipper_labourcharge,ODT.shipper_driverCharge,ODT.totalamount_shipper, " +
                         "  ODT.driver_id,ODT.truck_id,ODT.status,ODT.totaldriver_quot,OD.load_inquiry_no,OD.shippingdatetime,OD.shipper_id,(select top(1) user_id from user_mst where unique_id=OD.shipper_id) as shipper_mobile,(select top(1) first_name from user_mst where unique_id=OD.shipper_id) as shipper_name,(select top(1) email_id from user_mst where unique_id=OD.shipper_id) as shipper_email,OD.source_full_add,OD.destination_full_add,'id1' OrderKey  from  orders as OD  " +
                         "   left outer join order_driver_truck_details as ODT on ODT.load_inquiry_no=OD.load_inquiry_no " +
                         "   where 1=1 and OD.active_flag  = 'Y' and ODT.status not in ('45') and iscancel='N'" +
                         "   and OD.shippingdatetime>getdate() " +
                         "   union all " +
                         "  select ODT.NoOfHandiman,ODT.NoOfLabour,OD.isassign_driver_truck,OD.TotalDistance,OD.TotalDistanceUOM," +
                         "  ODT.baseCharge as driver_baseCharge,ODT.handimanCharge as driver_handimanCharge,ODT.labourcharge as driver_labourcharge,ODT.driverCharge as driver_driverCharge, " +
                         "  ODT.shipper_baseCharge,ODT.shipper_handimanCharge,ODT.shipper_labourcharge,ODT.shipper_driverCharge,ODT.totalamount_shipper, " +
                         "  ODT.driver_id,ODT.truck_id,ODT.status,ODT.totaldriver_quot,OD.load_inquiry_no,OD.shippingdatetime,OD.shipper_id,(select top(1) user_id from user_mst where unique_id=OD.shipper_id) as shipper_mobile,(select top(1) first_name from user_mst where unique_id=OD.shipper_id) as shipper_name,(select top(1) email_id from user_mst where unique_id=OD.shipper_id) as shipper_email,OD.source_full_add,OD.destination_full_add,'id2' OrderKey   from  orders as OD  " +
                         "   left outer join order_driver_truck_details as ODT on ODT.load_inquiry_no=OD.load_inquiry_no " +
                         "   where 1=1 and OD.active_flag  = 'Y'  and ODT.status not in ('45') and iscancel='N' " +
                         "    and OD.shippingdatetime<getdate()) as temp " +
                         "    ) result  " +
                         "  left join user_mst on user_mst.unique_id = result.shipper_id  " +
                         "  left join truck_mst on truck_mst.truck_id = result.truck_id  " +
                         "  left join driver_license_detail on driver_license_detail.driver_id = result.driver_id  " +
                         "  left join driver_contact_detail on driver_contact_detail.driver_id = result.driver_id  " +
                         "  left join truck_make_mst on truck_make_mst.make_id = truck_mst.truck_make_id  " +
                         "  left join truck_model_mst on truck_model_mst.model_id = truck_mst.truck_model " +
                         "  left join post_load_inquiry on post_load_inquiry.load_inquiry_no = result.load_inquiry_no  " +
                         "  left join driver_mst on result.driver_id  = driver_mst.driver_id  " +
                         "  where  1=1 and result.driver_id = @drid order by OrderKey, result.driver_id ,result.shippingdatetime asc ";

                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.CommandText = query;
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
                    return (BLGeneralUtil.return_ajax_string("0", " No Orders for driver id = " + drvid));
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                return (BLGeneralUtil.return_ajax_string("0", ex.Message));
            }
        }

        public DataTable GetdriverOrderNotificationByDrID(string DriverID)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT *,orders.* FROM driver_order_notifications " +
                            " join orders on orders.load_inquiry_no=driver_order_notifications.load_inquiry_no " +
                             " where driver_id = @Driverid and driver_order_notifications.active_flag='Y' order by driver_order_notifications.load_inquiry_date+driver_order_notifications.load_inquiry_delivery_time desc ";

            //String query1 = "SELECT * FROM driver_order_notifications where driver_id = @Driverid and active_flag=" + "'Y' order by load_inquiry_date+load_inquiry_delivery_time desc ";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("Driverid", DbType.String, DriverID));
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
        public string GetdriverOrderNotificationByDriverID(string DriverID, string deviceid)
        {

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(DriverID);
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != deviceid)
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }

            DataTable dtdrivers = GetdriverOrderNotificationByDrID(DriverID);
            if (dtdrivers != null && dtdrivers.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrivers));
            else
                return BLGeneralUtil.return_ajax_string("0", "Drivers Details Not found");
        }

        public DataTable GetOngoingOrderNumber(string Driverid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select * from order_driver_truck_details  where driver_id=@driverid and status not in ('02','45') ";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, Driverid));
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

        //this service change the driver after assign or assign new drivers to order
        [HttpPost]
        public string AssignTruckOrDriver([FromBody]JObject ord)
        {
            List<load_post_enquiry> tord = new List<load_post_enquiry>();
            DS_load_order_quotation ds_loadorderquotation = new DS_load_order_quotation();
            Master master = new Master();
            DataSet dsorder = new DataSet();
            Document objdoc = new Document();
            BLReturnObject objBLobj = new BLReturnObject();
            BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

            string str = "";

            if (ord["AddTruckOrDriver"] != null)
            {
                tord = ord["AddTruckOrDriver"].ToObject<List<load_post_enquiry>>();
                dsorder = master.CreateDataSet(tord);
            }

            // get load inquiry quatation details for update driverid/truckid  in database
            DataTable dt_loadinqquatation = new PostOrderController().GetLoadInquiryquotation(tord[0].load_inquiry_no);
            // get orders details for update driverid/truckid in database
            DataTable dt_orders = new PostOrderController().GetOrders(tord[0].load_inquiry_no);
            if (dt_orders != null)
                dt_orders = BLGeneralUtil.CheckDateTime(dt_orders);
            else
                return BLGeneralUtil.return_ajax_string("0", "Order details Not found");

            DataTable dtSizeTypeMst = new DataTable();
            DataTable dtSizeTypeMstFromShipper = new DataTable();

            DataTable dtfinal = new DataTable();
            DateTime Ordershippingdatetime = Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString());

            DataTable dtorderDrivertruckdetails = GetOrderDriverTruckDetails(tord[0].load_inquiry_no);

            try
            {
                try
                {
                    if (Ordershippingdatetime.Date < DateTime.UtcNow.Date)
                    {
                        sb.Append("Today date is " + DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss tt") + " and shipping date " + Ordershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "!!!.Are you sure want to assign order? <br>");
                    }

                    // for check driver is free or busy and display details prompt 
                    for (int i = 0; i < tord.Count; i++)
                    {
                        DateTime Dr_PreviousOrdershippingdatetime = new DateTime();
                        DateTime TotalDr_PreviousOrderdatetime = new DateTime();
                        DateTime Dr_NextOrdershippingdatetime = new DateTime();
                        DateTime TotalDr_NextOrderdatetime = new DateTime();

                        //get driver ongoing ordres 
                        DataTable dt_driverorders = GetOnGoingOrdersByDriver(tord[i].driver_id);

                        if (dt_driverorders != null)
                        {
                            // int dtcount = dt_driverorders.Rows.Count;

                            DateTime NewOrdershippingdatetime = Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString());
                            TimeSpan Newordertime = new TimeSpan();
                            Newordertime = TimeSpan.Parse(dt_orders.Rows[0]["TimeToTravelInMinute"].ToString());
                            DateTime NewOrderDatetime = NewOrdershippingdatetime.AddMinutes(Newordertime.TotalMinutes);
                            NewOrderDatetime = NewOrderDatetime.AddHours(2);

                            //driver previous order details based on new order shipping date 
                            DataTable dtDriverPreviousorders = GetDriverOrdersByID(tord[i].driver_id, NewOrdershippingdatetime.ToString(), "1");
                            DataTable dtDriverNextorders = GetDriverOrdersByID(tord[i].driver_id, NewOrdershippingdatetime.ToString(), "2");

                            if (dtDriverPreviousorders != null)
                            {
                                Dr_PreviousOrdershippingdatetime = Convert.ToDateTime(dtDriverPreviousorders.Rows[0]["shippingdatetime"].ToString());
                                TimeSpan DriverPreviousOrdertime = new TimeSpan();
                                DriverPreviousOrdertime = TimeSpan.Parse(dtDriverPreviousorders.Rows[0]["TimeToTravelInMinute"].ToString());
                                TotalDr_PreviousOrderdatetime = Dr_PreviousOrdershippingdatetime.AddHours(DriverPreviousOrdertime.TotalHours);
                                TotalDr_PreviousOrderdatetime = TotalDr_PreviousOrderdatetime.AddHours(2);

                                if (TotalDr_PreviousOrderdatetime > NewOrdershippingdatetime)
                                    sb.Append("'" + dt_driverorders.Rows[0]["drivername"].ToString() + "' is busy in order <br> '" + dt_driverorders.Rows[0]["drivername"].ToString() + "' free on '" + TotalDr_PreviousOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "' and Current Order shipping  date is '" + NewOrdershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt"));
                            }
                            else if (dtDriverNextorders != null)
                            {
                                Dr_NextOrdershippingdatetime = Convert.ToDateTime(dtDriverNextorders.Rows[0]["shippingdatetime"].ToString());
                                TotalDr_NextOrderdatetime = Dr_NextOrdershippingdatetime.AddHours(-2);

                                if (TotalDr_NextOrderdatetime < NewOrderDatetime)
                                    sb.Append("<br>'" + dt_driverorders.Rows[0]["drivername"].ToString() + "' is busy in order. <br> competition time  of received ordered will be '" + NewOrderDatetime.ToString("dd-MM-yyy HH:mm:ss tt") + "' and '" + dt_driverorders.Rows[0]["drivername"].ToString() + "'already having order at " + TotalDr_NextOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + " ");
                            }
                        }
                    }
                    if (tord[0].AssignOrderToDR == "N" && tord[0].isfinal == "Y")
                        if (sb.Length > 0)
                            return BLGeneralUtil.return_ajax_string("2", sb.ToString());

                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", ex.Message);
                }


                DataTable dt_driver = new DataTable();
                DataTable dt_truck = new DataTable();
                for (int i = 0; i < tord.Count; i++)
                {
                    str = ""; string truckid = "";
                    if (tord[i].driver_id != null && tord[i].driver_id != "")
                    {
                        dt_driver = GetDriverDetailsTableById(tord[i].driver_id.ToString()); // check driver is in database 
                        if (dt_driver == null)
                        {
                            return BLGeneralUtil.return_ajax_string("0", "Invalid Driver details");
                        }
                    }

                    try
                    {
                        DataTable dt = GetTruckIdBy(tord[i].driver_id);
                        if (dt != null)
                            truckid = dt.Rows[0]["truck_id"].ToString();
                        else
                            return BLGeneralUtil.return_ajax_string("0", "Driver Truck Not Found");

                        #region Calculate Price

                        TimeSpan Tsshippingtime;
                        DateTime OrderDate = DateTime.Today;
                        String Message = String.Empty;
                        String SizeTypeCode = String.Empty;
                        Tsshippingtime = TimeSpan.Parse(dt_orders.Rows[0]["TimeToTravelInMinute"].ToString());
                        if (dt_orders.Columns.Contains("SizeTypeCode"))
                            SizeTypeCode = dt_orders.Rows[0]["SizeTypeCode"].ToString();

                        String rate_type_flag = tord[0].rate_type_flag;

                        Decimal TotalDistance = -1;
                        if (dt_orders.Columns.Contains("TotalDistance"))
                            TotalDistance = Convert.ToDecimal(dt_orders.Rows[0]["TotalDistance"]);

                        String TotalDistanceUOM = String.Empty;
                        if (dt_orders.Columns.Contains("TotalDistanceUOM"))
                            TotalDistanceUOM = dt_orders.Rows[0]["TotalDistanceUOM"].ToString();

                        Decimal TimeToTravelInMinute = -1;
                        if (dt_orders.Columns.Contains("TimeToTravelInMinute"))
                            TimeToTravelInMinute = Convert.ToDecimal(Tsshippingtime.TotalMinutes);

                        String goods_type_flag = String.Empty;
                        if (dt_orders.Columns.Contains("goods_type_flag"))
                            goods_type_flag = dt_orders.Rows[0]["goods_type_flag"].ToString();


                        //No Of Truck Edited by User
                        int? NoOfTruck = null;
                        NoOfTruck = 1;

                        //No Of Driver Edited by User
                        int? NoOfDriver = null;
                        NoOfDriver = 1;

                        //No Of Labour Edited by User
                        int? NoOfLabour = null;
                        NoOfLabour = Convert.ToInt32(tord[i].NoOfLabour);

                        //No Of Handiman Edited by User
                        int? NoOfHandiman = null;
                        NoOfHandiman = Convert.ToInt32(tord[i].NoOfHandiman);

                        int? NoOfSupervisor = null;
                        NoOfSupervisor = NoOfTruck;


                        String IncludePackingCharge = "N";


                        if (NoOfTruck != NoOfDriver)
                            NoOfDriver = NoOfTruck;

                        if (dt_orders.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER || dt_orders.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                        {
                            TruckerMaster objTruckerMaster = new TruckerMaster();
                            dtSizeTypeMst = objTruckerMaster.CalculateRateGoods(dt_orders, SizeTypeCode, System.DateTime.UtcNow, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, 0, 0, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                return BLGeneralUtil.return_ajax_string("0", Message);

                            dtSizeTypeMst.Columns.Add("totalamount_shipper", typeof(String));
                            dtSizeTypeMst.Rows[0]["totalamount_shipper"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_driverCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_driverCharge"] = dtSizeTypeMst.Rows[0]["TotalDriverRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_truckCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_truckCharge"] = dtSizeTypeMst.Rows[0]["Total_cost"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_labourcharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_labourcharge"] = dtSizeTypeMst.Rows[0]["TotalLabourRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_handimanCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_handimanCharge"] = dtSizeTypeMst.Rows[0]["TotalHandimanRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_baseCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_baseCharge"] = dtSizeTypeMst.Rows[0]["BaseRate"].ToString();

                        }
                        else
                        {
                            TruckerMaster objTruckerMaster = new TruckerMaster();
                            dtSizeTypeMst = objTruckerMaster.CalculateRate(dt_orders, SizeTypeCode, System.DateTime.UtcNow, goods_type_flag, "B", TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                                return BLGeneralUtil.return_ajax_string("0", Message);

                            if (i == 0)
                                IncludePackingCharge = dt_orders.Rows[0]["IncludePackingCharge"].ToString();


                            dtSizeTypeMstFromShipper = objTruckerMaster.CalculateRate(dt_orders, SizeTypeCode, System.DateTime.UtcNow, goods_type_flag, rate_type_flag, TotalDistance, TotalDistanceUOM, TimeToTravelInMinute, IncludePackingCharge, NoOfTruck, NoOfDriver, NoOfLabour, NoOfHandiman, NoOfSupervisor, ref Message);
                            if (dtSizeTypeMstFromShipper == null || dtSizeTypeMstFromShipper.Rows.Count <= 0)
                                return BLGeneralUtil.return_ajax_string("0", Message);

                            dtSizeTypeMst.Columns.Add("totalamount_shipper", typeof(String));
                            dtSizeTypeMst.Rows[0]["totalamount_shipper"] = dtSizeTypeMstFromShipper.Rows[0]["Total_cost"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_driverCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_driverCharge"] = dtSizeTypeMstFromShipper.Rows[0]["TotalDriverRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_truckCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_truckCharge"] = dtSizeTypeMstFromShipper.Rows[0]["Total_cost"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_labourcharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_labourcharge"] = dtSizeTypeMstFromShipper.Rows[0]["TotalLabourRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_handimanCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_handimanCharge"] = dtSizeTypeMstFromShipper.Rows[0]["TotalHandimanRate"].ToString();
                            dtSizeTypeMst.Columns.Add("shipper_baseCharge", typeof(String));
                            dtSizeTypeMst.Rows[0]["shipper_baseCharge"] = dtSizeTypeMstFromShipper.Rows[0]["BaseRate"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log(ex.Message + ex.StackTrace);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }
                        #endregion

                    #region calculate multiple driver price based on labours and handiman  and retrn json data

                    // if multiple drivers are then send all details of all drivers using this 
                    if (dtfinal != null && dtfinal.Rows.Count > 0)
                    {
                        DataRow dr = dtfinal.NewRow();
                        dr.ItemArray = dtSizeTypeMst.Rows[0].ItemArray;
                        dtfinal.Rows.Add(dr);
                    }
                    else
                        dtfinal = dtSizeTypeMst.Copy();

                    #endregion



                    if (tord[i].AssignOrderToDR == "Y" && tord[i].isfinal == "Y" && dtfinal.Rows.Count == tord.Count)
                    {
                        if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                        DBCommand.Transaction = DBConnection.BeginTransaction();

                        for (int j = 0; j < tord.Count; j++)
                        {
                            DS_driver_order_notifications ds_driver_order_notifications = new DS_driver_order_notifications();
                            DataRow[] drdrvorderdtl = new DataRow[] { };
                            if (tord[j].Isupdate == "Y")
                            {
                                drdrvorderdtl = dtorderDrivertruckdetails.Select("driver_id='" + tord[j].driver_id + "'");
                                if (drdrvorderdtl.Length > 0)
                                {
                                    ds_driver_order_notifications.order_driver_truck_details.ImportRow(drdrvorderdtl[0]);
                                    ds_driver_order_notifications.order_driver_truck_details.Rows[0].Delete();

                                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_driver_order_notifications.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Deleted);
                                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_driver_order_notifications.order_driver_truck_details.Rows.Count)
                                    {
                                        DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                                    }
                                }
                            }

                            DataTable dt = GetTruckIdBy(tord[j].driver_id);
                            if (dt != null)
                                truckid = dt.Rows[0]["truck_id"].ToString();

                            #region Create Entry in order_driver_details



                            if (tord[j].driver_id.ToString().Trim() != "")
                            {
                                DS_driver_order_notifications.order_driver_truck_detailsRow tr_order_driver = ds_driver_order_notifications.order_driver_truck_details.Neworder_driver_truck_detailsRow();

                                ds_driver_order_notifications.EnforceConstraints = false;
                                tr_order_driver.load_inquiry_no = tord[j].load_inquiry_no;
                                tr_order_driver.mover_id = tord[j].mover_id;
                                tr_order_driver.driver_id = tord[j].driver_id;
                                tr_order_driver.truck_id = truckid;
                                tr_order_driver.status = Constant.ALLOCATED_BUT_NOT_STARTE;
                                tr_order_driver.active_flag = Constant.Flag_Yes;
                                tr_order_driver.created_by = tord[j].created_by;
                                tr_order_driver.created_date = System.DateTime.UtcNow;
                                tr_order_driver.created_host = tord[j].created_host;
                                tr_order_driver.device_id = tord[j].device_id;
                                tr_order_driver.device_type = tord[j].device_type;

                                tr_order_driver.NoOfHandiman = tord[j].NoOfHandiman;
                                tr_order_driver.NoOfLabour = tord[j].NoOfLabour;
                                if (tord[0].rate_type_flag == "P")
                                {
                                    tr_order_driver.totalamount_shipper = Convert.ToDecimal(dt_orders.Rows[0]["rem_amt_to_receive"].ToString());
                                    tr_order_driver.totaldriver_quot = 0;
                                    tr_order_driver.driverCharge = 0;
                                    tr_order_driver.labourcharge = 0;
                                    tr_order_driver.handimanCharge = 0;
                                    tr_order_driver.baseCharge = 0;

                                    tr_order_driver.shipper_driverCharge = 0;
                                    tr_order_driver.shipper_labourcharge = 0;
                                    tr_order_driver.shipper_handimanCharge = 0;
                                    tr_order_driver.shipper_baseCharge = 0;
                                }
                                else
                                {
                                    tr_order_driver.totalamount_shipper = Convert.ToDecimal(dtfinal.Rows[j]["totalamount_shipper"].ToString());
                                    tr_order_driver.totaldriver_quot = Convert.ToDecimal(dtfinal.Rows[j]["Total_cost"].ToString());
                                    tr_order_driver.driverCharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalDriverRate"].ToString());
                                    tr_order_driver.labourcharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalLabourRate"].ToString());
                                    tr_order_driver.handimanCharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalHandimanRate"].ToString());
                                    tr_order_driver.baseCharge = Convert.ToDecimal(dtfinal.Rows[j]["BaseRate"].ToString());

                                    tr_order_driver.shipper_driverCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_driverCharge"].ToString());
                                    tr_order_driver.shipper_labourcharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_labourcharge"].ToString());
                                    tr_order_driver.shipper_handimanCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_handimanCharge"].ToString());
                                    tr_order_driver.shipper_baseCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_baseCharge"].ToString());
                                }

                                ds_driver_order_notifications.order_driver_truck_details.Addorder_driver_truck_detailsRow(tr_order_driver);
                                ds_driver_order_notifications.order_driver_truck_details.Rows[0].AcceptChanges();
                                if (drdrvorderdtl.Length == 0)
                                    ds_driver_order_notifications.order_driver_truck_details.Rows[0].SetAdded();

                                ds_driver_order_notifications.EnforceConstraints = false;
                                objBLobj = master.UpdateTables(ds_driver_order_notifications.order_driver_truck_details, ref DBCommand);
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

                            #region update status in driver_mst

                            //DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                            //if (dt_driver != null && dt_driver.Rows.Count > 0)
                            //{
                            //    ds_owner.driver_mst.ImportRow(dt_driver.Rows[0]);
                            //    ds_owner.driver_mst[0].isfree = Constant.Flag_No;

                            //    objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            //    if (objBLobj.ExecutionStatus != 1)
                            //    {
                            //        ServerLog.Log(objBLobj.ServerMessage.ToString());
                            //        DBCommand.Transaction.Rollback();
                            //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            //    }
                            //}

                            #endregion

                            String Msg = ""; Byte status = 0;
                            status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tord[j].driver_id, "ADMIN", Constant.MessageType_AssignOrder, " You have Assign Order on '" + Ordershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + " ' from ' " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + " ' To ' " + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + " ' ", Constant.MessageType_AssignOrder, dt_orders.Rows[0]["load_inquiry_no"].ToString(), dt_orders.Rows[0]["shipper_id"].ToString(), dt_orders.Rows[0]["load_inquiry_no"].ToString(), tord[0].truck_id, tord[0].driver_id, dt_orders.Rows[0]["shipper_id"].ToString(), ref Msg);
                            if (status == Constant.Status_Fail)
                            {
                                ServerLog.Log("Error in save notification Data ");
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                            }

                            status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, dt_orders.Rows[0]["shipper_id"].ToString(), "ADMIN", Constant.MessageType_AssignDriverTruck, "Driver and truck are assign your order # " + tord[0].load_inquiry_no, Constant.MessageType_AssignDriverTruck, dt_orders.Rows[0]["load_inquiry_no"].ToString(), dt_orders.Rows[0]["shipper_id"].ToString(), dt_orders.Rows[0]["load_inquiry_no"].ToString(), truckid, tord[j].driver_id, dt_orders.Rows[0]["shipper_id"].ToString(), ref Msg);
                            if (status == Constant.Status_Fail)
                            {
                                ServerLog.Log("Error in save notification Data ");
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                            }

                            try
                            {
                                Msg = "You have Assign Order on ' " + Ordershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + " '  from ' " + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + "' To '" + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + "'.Your reference order number is " + dt_orders.Rows[0]["load_inquiry_no"].ToString();
                                string strno = new PostOrderController().GetMobileNoByID(tord[j].driver_id);
                                new EMail().SendOtpToUserMobileNoUAE(Msg, strno);
                                ServerLog.Log("OTP send TO " + strno);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                ServerLog.Log("Error in send OTP on Completation ");
                            }

                        }

                        #region update status in Orders

                        DS_orders ds_order = new DS_orders();
                        ds_order.orders.ImportRow(dt_orders.Rows[0]);
                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tord[i].load_inquiry_no);
                        ds_order.orders[0].trackurl = trakurl;
                        ds_order.orders[0].isassign_driver_truck = Constant.Flag_Yes;
                        if (tord[0].mover_id.Trim() != "")
                            ds_order.orders[0].isassign_mover = Constant.Flag_Yes;

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

                        if (tord[i].isfinal == "Y")
                        {
                            DS_driver_order_notifications ds_driver_order_notifications = new DS_driver_order_notifications();
                            DataRow[] drdrvorderdtl = new DataRow[] { };
                            if (tord[i].Isupdate == "Y")
                            {
                                var drivids = dsorder.Tables[0].AsEnumerable().Select(r => r.Field<string>("driver_id")).ToList();
                                string strdrvid = "";
                                for (int drvidcnt = 0; drvidcnt < drivids.Count; drvidcnt++)
                                {
                                    if (drvidcnt == 0)
                                        strdrvid = "'" + drivids[drvidcnt] + "',";
                                    else
                                        strdrvid = strdrvid + "'" + drivids[drvidcnt] + "',";
                                }
                                strdrvid = strdrvid.Remove(strdrvid.Length - 1);


                                drdrvorderdtl = dtorderDrivertruckdetails.Select("driver_id not in (" + strdrvid + ")");
                                if (drdrvorderdtl.Length > 0)
                                {
                                    for (int a = 0; a < drdrvorderdtl.Length; a++)
                                    {
                                        ds_driver_order_notifications.order_driver_truck_details.ImportRow(drdrvorderdtl[a]);
                                        ds_driver_order_notifications.order_driver_truck_details.Rows[a].Delete();
                                    }

                                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_driver_order_notifications.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Deleted);
                                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_driver_order_notifications.order_driver_truck_details.Rows.Count)
                                    {
                                        DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    }

                                }
                            }
                        }


                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 1;


                        return BLGeneralUtil.return_ajax_string("1", "Driver Add Successfully");
                    }

                    if (tord[i].AssignOrderToDR == "N" && tord[i].isfinal == "Y" && dtfinal.Rows.Count == tord.Count && sb.Length == 0)
                    {
                        if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                        DBCommand.Transaction = DBConnection.BeginTransaction();



                        for (int j = 0; j < tord.Count; j++)
                        {
                            DS_driver_order_notifications ds_driver_order_notifications = new DS_driver_order_notifications();

                            DataRow[] drdrvorderdtl = new DataRow[] { };
                            if (tord[j].Isupdate == "Y")
                            {
                                drdrvorderdtl = dtorderDrivertruckdetails.Select("driver_id='" + tord[j].driver_id + "'");
                                if (drdrvorderdtl.Length > 0)
                                {
                                    ds_driver_order_notifications.order_driver_truck_details.ImportRow(drdrvorderdtl[0]);
                                    ds_driver_order_notifications.order_driver_truck_details.Rows[0].Delete();

                                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_driver_order_notifications.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Deleted);
                                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_driver_order_notifications.order_driver_truck_details.Rows.Count)
                                    {
                                        DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    }
                                }
                            }

                            DataTable dt = GetTruckIdBy(tord[j].driver_id);
                            if (dt != null)
                                truckid = dt.Rows[0]["truck_id"].ToString();

                            #region Create Entry in order_driver_details



                            if (tord[j].driver_id.ToString().Trim() != "")
                            {
                                DS_driver_order_notifications.order_driver_truck_detailsRow tr_order_driver = ds_driver_order_notifications.order_driver_truck_details.Neworder_driver_truck_detailsRow();

                                ds_driver_order_notifications.EnforceConstraints = false;
                                tr_order_driver.load_inquiry_no = tord[j].load_inquiry_no;
                                tr_order_driver.mover_id = tord[j].mover_id;
                                tr_order_driver.driver_id = tord[j].driver_id;
                                tr_order_driver.truck_id = truckid;
                                tr_order_driver.status = Constant.ALLOCATED_BUT_NOT_STARTE;
                                tr_order_driver.active_flag = Constant.Flag_Yes;
                                tr_order_driver.created_by = tord[j].created_by;
                                tr_order_driver.created_date = System.DateTime.UtcNow;
                                tr_order_driver.created_host = tord[j].created_host;
                                tr_order_driver.device_id = tord[j].device_id;
                                tr_order_driver.device_type = tord[j].device_type;

                                tr_order_driver.NoOfHandiman = tord[j].NoOfHandiman;
                                tr_order_driver.NoOfLabour = tord[j].NoOfLabour;
                                if (tord[j].rate_type_flag == "P")
                                {
                                    tr_order_driver.totalamount_shipper = Convert.ToDecimal(dtfinal.Rows[j]["totalamount_shipper"].ToString());
                                    tr_order_driver.totaldriver_quot = 0;
                                    tr_order_driver.driverCharge = 0;
                                    tr_order_driver.labourcharge = 0;
                                    tr_order_driver.handimanCharge = 0;
                                    tr_order_driver.baseCharge = 0;

                                    tr_order_driver.shipper_driverCharge = 0;
                                    tr_order_driver.shipper_labourcharge = 0;
                                    tr_order_driver.shipper_handimanCharge = 0;
                                    tr_order_driver.shipper_baseCharge = 0;
                                }
                                else
                                {
                                    tr_order_driver.totalamount_shipper = Convert.ToDecimal(dtfinal.Rows[j]["totalamount_shipper"].ToString());
                                    tr_order_driver.totaldriver_quot = Convert.ToDecimal(dtfinal.Rows[j]["Total_cost"].ToString());
                                    tr_order_driver.driverCharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalDriverRate"].ToString());
                                    tr_order_driver.labourcharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalLabourRate"].ToString());
                                    tr_order_driver.handimanCharge = Convert.ToDecimal(dtfinal.Rows[j]["TotalHandimanRate"].ToString());
                                    tr_order_driver.baseCharge = Convert.ToDecimal(dtfinal.Rows[j]["BaseRate"].ToString());

                                    tr_order_driver.shipper_driverCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_driverCharge"].ToString());
                                    tr_order_driver.shipper_labourcharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_labourcharge"].ToString());
                                    tr_order_driver.shipper_handimanCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_handimanCharge"].ToString());
                                    tr_order_driver.shipper_baseCharge = Convert.ToDecimal(dtfinal.Rows[j]["shipper_baseCharge"].ToString());
                                }
                                ds_driver_order_notifications.order_driver_truck_details.Addorder_driver_truck_detailsRow(tr_order_driver);
                                ds_driver_order_notifications.order_driver_truck_details.Rows[0].AcceptChanges();
                                if (drdrvorderdtl.Length == 0)
                                    ds_driver_order_notifications.order_driver_truck_details.Rows[0].SetAdded();

                                ds_driver_order_notifications.EnforceConstraints = false;
                                objBLobj = master.UpdateTables(ds_driver_order_notifications.order_driver_truck_details, ref DBCommand);
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

                            #region update status in driver_mst

                            //DS_Owner_Mst ds_owner = new DS_Owner_Mst();
                            //if (dt_driver != null && dt_driver.Rows.Count > 0)
                            //{
                            //    ds_owner.driver_mst.ImportRow(dt_driver.Rows[0]);
                            //    ds_owner.driver_mst[0].isfree = Constant.Flag_No;

                            //    objBLobj = master.UpdateTables(ds_owner.driver_mst, ref DBCommand);
                            //    if (objBLobj.ExecutionStatus != 1)
                            //    {
                            //        ServerLog.Log(objBLobj.ServerMessage.ToString());
                            //        DBCommand.Transaction.Rollback();
                            //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            //        return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                            //    }
                            //}

                            #endregion

                            String Msg = ""; Byte status = 0;
                            status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, tord[j].driver_id, "ADMIN", Constant.MessageType_AssignOrder, "You have Assign Order on '" + Ordershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "' from '" + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + "' To '" + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + "'", Constant.MessageType_AssignOrder, dt_orders.Rows[0]["load_inquiry_no"].ToString(), dt_orders.Rows[0]["shipper_id"].ToString(), dt_orders.Rows[0]["load_inquiry_no"].ToString(), tord[0].truck_id, tord[0].driver_id, dt_orders.Rows[0]["shipper_id"].ToString(), ref Msg);
                            if (status == Constant.Status_Fail)
                            {
                                ServerLog.Log("Error in save notification Data ");
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                            }

                            //     DataTable dtdrivertruck = 
                            status = new MgmtMessaging().SaveMessageQueue(ref DBCommand, dt_orders.Rows[0]["shipper_id"].ToString(), "ADMIN", Constant.MessageType_AssignDriverTruck, " Driver & Truck are assigned to your order # " + tord[0].load_inquiry_no, Constant.MessageType_AssignDriverTruck, dt_orders.Rows[0]["load_inquiry_no"].ToString(), dt_orders.Rows[0]["shipper_id"].ToString(), dt_orders.Rows[0]["load_inquiry_no"].ToString(), truckid, tord[j].driver_id, dt_orders.Rows[0]["shipper_id"].ToString(), ref Msg);
                            if (status == Constant.Status_Fail)
                            {
                                ServerLog.Log("Error in save notification Data ");
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", " Error in save notification Data ");
                            }

                            try
                            {
                                Msg = "You have Assign Order on '" + Ordershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "' from '" + dt_orders.Rows[0]["inquiry_source_addr"].ToString() + "' To '" + dt_orders.Rows[0]["inquiry_destination_addr"].ToString() + "'.Your reference order number is " + dt_orders.Rows[0]["load_inquiry_no"].ToString();
                                string strno = new PostOrderController().GetMobileNoByID(tord[j].driver_id);
                                new EMail().SendOtpToUserMobileNoUAE(Msg, strno);
                                ServerLog.Log("OTP send TO " + strno);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                                ServerLog.Log("Error in send OTP on Completation ");
                            }

                        }

                        #region update status in Orders

                        DS_orders ds_order = new DS_orders();
                        ds_order.orders.ImportRow(dt_orders.Rows[0]);
                        string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tord[0].load_inquiry_no);
                        ds_order.orders[0].trackurl = trakurl;
                        ds_order.orders[0].isassign_driver_truck = Constant.Flag_Yes;
                        if (tord[0].mover_id != "")
                            ds_order.orders[0].isassign_mover = Constant.Flag_Yes;
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

                        if (tord[i].isfinal == "Y")
                        {
                            DS_driver_order_notifications ds_driver_order_notifications = new DS_driver_order_notifications();
                            DataRow[] drdrvorderdtl = new DataRow[] { };
                            if (tord[i].Isupdate == "Y")
                            {
                                var drivids = dsorder.Tables[0].AsEnumerable().Select(r => r.Field<string>("driver_id")).ToList();
                                string strdrvid = "";
                                for (int drvidcnt = 0; drvidcnt < drivids.Count; drvidcnt++)
                                {
                                    if (drvidcnt == 0)
                                        strdrvid = "'" + drivids[drvidcnt] + "',";
                                    else
                                        strdrvid = strdrvid + "'" + drivids[drvidcnt] + "',";
                                }
                                strdrvid = strdrvid.Remove(strdrvid.Length - 1);


                                drdrvorderdtl = dtorderDrivertruckdetails.Select("driver_id not in (" + strdrvid + ")");
                                if (drdrvorderdtl.Length > 0)
                                {
                                    for (int a = 0; a < drdrvorderdtl.Length; a++)
                                    {
                                        ds_driver_order_notifications.order_driver_truck_details.ImportRow(drdrvorderdtl[a]);
                                        ds_driver_order_notifications.order_driver_truck_details.Rows[a].Delete();
                                    }

                                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_driver_order_notifications.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Deleted);
                                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != ds_driver_order_notifications.order_driver_truck_details.Rows.Count)
                                    {
                                        DBCommand.Transaction.Rollback();
                                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    }

                                }
                            }
                        }

                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLobj.ExecutionStatus = 1;


                        return BLGeneralUtil.return_ajax_string("1", "Driver Add Successfully");

                    }
                }

                //flags for calculate price for selected drivers and return result
                if (tord[0].AssignOrderToDR == "N" && tord[0].isfinal == "N")
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtfinal));

                //flags for driver check that driver is busy or free if free then calculate price and return result
                if (tord[0].AssignOrderToDR == "N" && tord[0].isfinal == "Y")
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtfinal));


                return BLGeneralUtil.return_ajax_string("1", "Driver Add Successfully");


            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        //this service change the driver after assign or assign new drivers to order
        [HttpPost]
        public string AssignMoverToOrder([FromBody]JObject ord)
        {
            List<load_post_enquiry> tord = new List<load_post_enquiry>();
            DS_load_order_quotation ds_loadorderquotation = new DS_load_order_quotation();
            Master master = new Master();
            DataSet dsorder = new DataSet();
            Document objdoc = new Document();
            BLReturnObject objBLobj = new BLReturnObject();

            string str = "";

            if (ord["order"] != null)
            {
                tord = ord["order"].ToObject<List<load_post_enquiry>>();
                dsorder = master.CreateDataSet(tord);
            }

            // get orders details for update driverid/truckid in database
            DataTable dt_orders = new PostOrderController().GetOrders(tord[0].load_inquiry_no);
            if (dt_orders != null)
                dt_orders = BLGeneralUtil.CheckDateTime(dt_orders);
            else
                return BLGeneralUtil.return_ajax_string("0", "Order details Not found");

            DateTime Ordershippingdatetime = Convert.ToDateTime(dt_orders.Rows[0]["shippingdatetime"].ToString());

            DS_orders ds_order = new DS_orders();

            try
            {
                if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                #region Create Entry in order_driver_details

                DS_orders.movers_order_detailsRow tr_Mover_order = ds_order.movers_order_details.Newmovers_order_detailsRow();

                ds_order.EnforceConstraints = false;
                tr_Mover_order.load_inquiry_no = tord[0].load_inquiry_no;
                tr_Mover_order.mover_id = tord[0].mover_id;
                tr_Mover_order.status = Constant.ALLOCATED_BUT_NOT_STARTE;
                tr_Mover_order.active_flag = Constant.Flag_Yes;
                tr_Mover_order.created_by = tord[0].created_by;
                tr_Mover_order.created_date = System.DateTime.UtcNow;
                tr_Mover_order.created_host = tord[0].created_host;
                tr_Mover_order.device_id = tord[0].device_id;
                tr_Mover_order.device_type = tord[0].device_type;

                ds_order.movers_order_details.Addmovers_order_detailsRow(tr_Mover_order);
                ds_order.movers_order_details.Rows[0].AcceptChanges();
                ds_order.movers_order_details.Rows[0].SetAdded();

                ds_order.EnforceConstraints = false;
                objBLobj = master.UpdateTables(ds_order.movers_order_details, ref DBCommand);
                if (objBLobj.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }


                #endregion

                #region update status in Orders


                ds_order.orders.ImportRow(dt_orders.Rows[0]);
                string trakurl = Trackurl + "?lid=" + BLGeneralUtil.Encrypt(tord[0].load_inquiry_no);
                ds_order.orders[0].trackurl = trakurl;
                ds_order.orders[0].isassign_mover = Constant.Flag_Yes;

                objBLobj = master.UpdateTables(ds_order.orders, ref DBCommand);
                if (objBLobj.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLobj.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
                }

                #endregion

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLobj.ExecutionStatus = 1;

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }

            return BLGeneralUtil.return_ajax_string("1", "Driver Add Successfully");

        }

        [HttpGet]
        public string GetMoverDetails()
        {

            String query1 = "";
            DataTable dtmover_mst = new DataTable();
            query1 = " select * from mover_mst where active_flag='Y'  ";

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtmover_mst = ds.Tables[0];
            }

            if (dtmover_mst != null && dtmover_mst.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtmover_mst));
            else
                return BLGeneralUtil.return_ajax_string("0", "Movers details not found");
        }

        [HttpGet]
        public string GetMoverDetails(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = " select movers_order_details.*,mover_Name,company_name,mobile_no from movers_order_details " +
                          " join mover_mst on mover_mst.mover_id=movers_order_details.mover_id " +
                          " where load_inquiry_no=@inqid";
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
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Driver not assigned "));
        }

        // nitu mem 07-10-2016 not in use
        public DataTable GetDriverTruckDetailsByLoadInquiryNo(string loadinqno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select temp.truck_id,temp.body_type,temp.truck_body_desc,temp.vehicle_reg_no," +
                            "[Name] as driverName,[driver_photo],[IsOnDuty],[isfree],[mobile_no] as DriverMobileNo from order_driver_truck_details " +
                           " join driver_mst on driver_mst.driver_id = order_driver_truck_details.driver_id " +
                           " join ( SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, " +
                           " truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, " +
                           " truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo " +
                           " from truck_mst " +
                           " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id " +
                           " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                           " left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id " +
                           " left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id " +
                           " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ) as temp on temp.truck_id =  order_driver_truck_details.truck_id " +
                           " where load_inquiry_no=@inqid ";
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

        [HttpPost]
        public HttpResponseMessage PostFileDriver()
        {
            HttpResponseMessage result = null;
            try
            {

                var httpRequest = HttpContext.Current.Request;
                Dictionary<string, string> array1 = new Dictionary<string, string>();
                ServerLog.Log(httpRequest["driver_id"].ToString() + "_" + httpRequest.Files.Count);

                if (httpRequest.Files.Count > 0)
                {
                    int randNum = 0; string filenm = "";
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        ServerLog.Log(httpRequest["driver_id"].ToString() + "_File_" + postedFile.FileName);
                        if (postedFile.FileName.Trim() != "")
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/Images/DrvPhoto/" + httpRequest["driver_id"].ToString() + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + randNum.ToString() + Path.GetExtension(postedFile.FileName));
                            postedFile.SaveAs(filePath);

                            docfiles.Add(filePath);
                            string filename = httpRequest["driver_id"].ToString() + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + randNum.ToString() + Path.GetExtension(postedFile.FileName);
                            array1[file] = "/Images/DrvPhoto/" + filename;
                            randNum++;

                            //filenm = postedFile.FileName;
                            //filenm = filenm.ToString().Substring(0, filenm.IndexOf(".")) + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + filenm.ToString().Substring(filenm.IndexOf("."), (filenm.ToString().Length - filenm.IndexOf(".")));
                            //filePath = HttpContext.Current.Server.MapPath("~/Images/DrvPhoto/"+ filenm);
                            //postedFile.SaveAs(filePath);
                            //docfiles.Add(filePath);
                            //array1[file] = "/Images/DrvPhoto/" + filenm;

                        }
                    }

                    String str = update_drv_detail_Table(httpRequest["driver_id"], array1);
                    result = Request.CreateResponse(HttpStatusCode.Created, str);

                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                return null;
            }
            return result;
        }

        public string update_drv_detail_Table(string driverid, Dictionary<string, string> data)
        {
            Dictionary<string, string> filePath = new Dictionary<string, string>();
            foreach (var pair in data)
            {
                filePath[pair.Key] = pair.Value;
            }


            Master drvmst = new Master(); string drvid = "";
            if (driverid != null && driverid != string.Empty)
            {
                drvid = driverid.Replace('"', ' ').Trim();
                drvid = driverid.TrimEnd().TrimStart();
            }

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            DataTable dtdrivermst = drvmst.GetDriverMst(drvid);


            #region Driver Mst Details
            if (filePath.Keys.Contains("driver_photo"))
            {
                try
                {
                    objdriver.driver_mst.Clear();
                    DS_Owner_Mst.driver_mstRow drivermstrow = objdriver.driver_mst.Newdriver_mstRow();
                    drivermstrow.driver_id = dtdrivermst.Rows[0]["driver_id"].ToString();
                    drivermstrow.reg_date = Convert.ToDateTime(dtdrivermst.Rows[0]["reg_date"]);
                    drivermstrow.Name = dtdrivermst.Rows[0]["Name"].ToString();
                    drivermstrow.age = dtdrivermst.Rows[0]["age"].ToString();
                    if (dtdrivermst.Rows[0]["dob"].ToString() != "")
                        drivermstrow.dob = Convert.ToDateTime(dtdrivermst.Rows[0]["dob"]);

                    drivermstrow.qualification = dtdrivermst.Rows[0]["qualification"].ToString();
                    drivermstrow.driver_origin = dtdrivermst.Rows[0]["driver_origin"].ToString();
                    drivermstrow.martial_status = dtdrivermst.Rows[0]["martial_status"].ToString();
                    drivermstrow.no_of_child = dtdrivermst.Rows[0]["no_of_child"].ToString();
                    drivermstrow.health_issues = dtdrivermst.Rows[0]["health_issues"].ToString();
                    drivermstrow.smoking = dtdrivermst.Rows[0]["smoking"].ToString();
                    drivermstrow.alcohol = dtdrivermst.Rows[0]["alcohol"].ToString();
                    drivermstrow.legal_history = dtdrivermst.Rows[0]["legal_history"].ToString();
                    drivermstrow.commercial_experience = dtdrivermst.Rows[0]["commercial_experience"].ToString();
                    drivermstrow.owner_id = dtdrivermst.Rows[0]["owner_id"].ToString();
                    drivermstrow.driver_photo = dtdrivermst.Rows[0]["driver_photo"].ToString();
                    drivermstrow.mobile_no = dtdrivermst.Rows[0]["mobile_no"].ToString();
                    drivermstrow.isfree = dtdrivermst.Rows[0]["isfree"].ToString();
                    drivermstrow.IsOnDuty = dtdrivermst.Rows[0]["IsOnDuty"].ToString();


                    if (filePath.Keys.Contains("driver_photo"))
                        drivermstrow.driver_photo = filePath["driver_photo"];

                    drivermstrow.active_flag = Constant.Flag_Yes;
                    drivermstrow.created_by = dtdrivermst.Rows[0]["created_by"].ToString();
                    drivermstrow.created_date = Convert.ToDateTime(dtdrivermst.Rows[0]["created_date"].ToString());
                    drivermstrow.created_host = dtdrivermst.Rows[0]["created_host"].ToString();
                    drivermstrow.device_id = dtdrivermst.Rows[0]["device_id"].ToString();
                    drivermstrow.device_type = dtdrivermst.Rows[0]["device_type"].ToString();
                    drivermstrow.modified_by = dtdrivermst.Rows[0]["created_by"].ToString();
                    drivermstrow.modified_date = System.DateTime.UtcNow;
                    drivermstrow.modified_host = dtdrivermst.Rows[0]["created_host"].ToString();
                    drivermstrow.modified_device_id = dtdrivermst.Rows[0]["device_id"].ToString();
                    drivermstrow.modified_device_type = dtdrivermst.Rows[0]["device_type"].ToString();

                    objdriver.driver_mst.Adddriver_mstRow(drivermstrow);



                }
                catch (Exception ex)
                {
                    ServerLog.Log("Driver Master udpate : = " + ex.Message.ToString() + "-" + driverid + Environment.NewLine + ex.StackTrace);
                    return ex.Message.ToString();
                }
            }
            #endregion

            #region driver_contact_detail

            //DataTable dtdrivercontactdetails = drvmst.GetDriverContactdetails(drvid);

            //try
            //{
            //    objdriver.driver_contact_detail.Clear();
            //    DS_Owner_Mst.driver_contact_detailRow objOwnerDriverContact = objdriver.driver_contact_detail.Newdriver_contact_detailRow();
            //    objOwnerDriverContact.driver_id = dtdrivermst.Rows[0]["driver_id"].ToString();
            //    objOwnerDriverContact.addr_id = 1;
            //    objOwnerDriverContact.address = dtdrivercontactdetails.Rows[0]["address"].ToString();
            //    objOwnerDriverContact.city = dtdrivercontactdetails.Rows[0]["city"].ToString();
            //    objOwnerDriverContact.state = dtdrivercontactdetails.Rows[0]["state"].ToString();
            //    objOwnerDriverContact.pincode = dtdrivercontactdetails.Rows[0]["pincode"].ToString();
            //    objOwnerDriverContact.phone_no = dtdrivercontactdetails.Rows[0]["phone_no"].ToString();
            //    objOwnerDriverContact.mobile_no = dtdrivercontactdetails.Rows[0]["mobile_no"].ToString();
            //    objOwnerDriverContact.nationality = dtdrivercontactdetails.Rows[0]["nationality"].ToString();
            //    objOwnerDriverContact.emirates_id = dtdrivercontactdetails.Rows[0]["emirates_id"].ToString();
            //    objOwnerDriverContact.emirates_id_exp_date = dtdrivercontactdetails.Rows[0]["emirates_id_exp_date"].ToString() != "" ? Convert.ToDateTime(dtdrivercontactdetails.Rows[0]["emirates_id_exp_date"].ToString()) : DateTime.UtcNow.AddYears(10);
            //    objOwnerDriverContact.emirates_id_copy = dtdrivercontactdetails.Rows[0]["emirates_id_copy"].ToString();

            //    if (filePath.Keys.Contains("emirates_id_copy"))
            //        objOwnerDriverContact.emirates_id_copy = filePath["emirates_id_copy"];
            //    objOwnerDriverContact.active_flag = dtdrivercontactdetails.Rows[0]["active_flag"].ToString();
            //    objOwnerDriverContact.created_by = dtdrivercontactdetails.Rows[0]["created_by"].ToString();
            //    objOwnerDriverContact.created_date = Convert.ToDateTime(dtdrivercontactdetails.Rows[0]["created_date"].ToString());
            //    objOwnerDriverContact.created_host = dtdrivercontactdetails.Rows[0]["created_host"].ToString();
            //    objOwnerDriverContact.device_id = dtdrivercontactdetails.Rows[0]["device_id"].ToString();
            //    objOwnerDriverContact.device_type = dtdrivercontactdetails.Rows[0]["device_type"].ToString();

            //    objdriver.driver_contact_detail.Adddriver_contact_detailRow(objOwnerDriverContact);

            //}
            //catch (Exception ex)
            //{
            //    ServerLog.Log("Driver Contact Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
            //    return null;
            //}

            #endregion

            #region driver identification table
            try
            {
                //objdriver.driver_identification_detail.Clear();
                //DataTable dtdriverid = drvmst.GetDriverIdentificationData(drvid);
                //for (int i = 0; i < dtdriverid.Rows.Count; i++)
                //{

                //    DS_Owner_Mst.driver_identification_detailRow drvidRow = objdriver.driver_identification_detail.Newdriver_identification_detailRow();
                //    drvidRow.driver_id = dtdriverid.Rows[i]["driver_id"].ToString();
                //    drvidRow.identification_id = dtdriverid.Rows[i]["identification_id"].ToString();
                //    drvidRow.id_no = dtdriverid.Rows[i]["id_no"].ToString();
                //    if (drvidRow.identification_id.Trim() == "I001")
                //    {
                //        if (filePath.ContainsKey("license_photo"))
                //            drvidRow.id_path = filePath["license_photo"];       // Driving Licsense Photo Path
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I002")
                //    {
                //        if (filePath.ContainsKey("voter_photo"))
                //            drvidRow.id_path = filePath["voter_photo"];         // Voter ID Photo Path   
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I003")
                //    {
                //        if (filePath.ContainsKey("pan_photo"))
                //            drvidRow.id_path = filePath["pan_photo"];           // PAN Card Photo Path
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I004")
                //    {
                //        if (filePath.ContainsKey("ration_photo"))
                //            drvidRow.id_path = filePath["ration_photo"];        // Ration Card Photo Path
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I005")
                //    {
                //        if (filePath.ContainsKey("aadhar_photo"))
                //            drvidRow.id_path = filePath["aadhar_photo"];        // Aadhar Photo Path
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I006")
                //    {
                //        if (filePath.ContainsKey("npr_photo"))
                //            drvidRow.id_path = filePath["npr_photo"];           // Nation population Register Photo path
                //    }
                //    else if (drvidRow.identification_id.Trim() == "I007")
                //    {
                //        if (filePath.ContainsKey("passport_photo"))
                //            drvidRow.id_path = filePath["passport_photo"];      // Passport Photo path
                //    }
                //    drvidRow.id_issued_from = dtdriverid.Rows[i]["id_issued_from"].ToString();
                //    if (dtdriverid.Rows[i]["id_valid_from"] != null && dtdriverid.Rows[i]["id_valid_from"].ToString() != "")
                //        drvidRow.id_valid_from = Convert.ToDateTime(dtdriverid.Rows[i]["id_valid_from"]);
                //    if (dtdriverid.Rows[i]["id_valid_upto"] != null && dtdriverid.Rows[i]["id_valid_upto"].ToString() != "")
                //        drvidRow.id_valid_upto = Convert.ToDateTime(dtdriverid.Rows[i]["id_valid_upto"]);
                //    drvidRow.status = dtdriverid.Rows[i]["status"].ToString();
                //    drvidRow.active_flag = Constant.Flag_Yes;
                //    drvidRow.created_by = dtdriverid.Rows[i]["created_by"].ToString();
                //    drvidRow.created_date = Convert.ToDateTime(dtdriverid.Rows[i]["created_date"].ToString());
                //    drvidRow.created_host = dtdriverid.Rows[i]["created_host"].ToString();
                //    drvidRow.device_id = dtdriverid.Rows[i]["device_id"].ToString();
                //    drvidRow.device_type = dtdriverid.Rows[i]["device_type"].ToString();
                //    drvidRow.modified_by = dtdriverid.Rows[i]["created_by"].ToString();
                //    drvidRow.modified_date = System.DateTime.UtcNow;
                //    drvidRow.modified_host = dtdriverid.Rows[i]["created_host"].ToString();
                //    drvidRow.modified_device_id = dtdriverid.Rows[i]["device_id"].ToString();
                //    drvidRow.modified_device_type = dtdriverid.Rows[i]["device_type"].ToString();

                //    objdriver.driver_identification_detail.Adddriver_identification_detailRow(drvidRow);
                //}

            }
            catch (Exception ex)
            {
                ServerLog.Log("Driver identification udpate : = " + ex.Message.ToString() + "-" + driverid + Environment.NewLine + ex.StackTrace);
                return ex.Message.ToString();
            }
            #endregion

            #region Driver License Details
            DataTable dtlicense = new DataTable();
            try
            {
                objdriver.driver_license_detail.Clear();
                if (filePath.ContainsKey("license_photo"))
                {
                    dtlicense = drvmst.GetDriverLicenseData(drvid);
                    if (dtlicense != null && dtlicense.Rows.Count > 0)
                    {
                        DS_Owner_Mst.driver_license_detailRow DrvLisnRow = objdriver.driver_license_detail.Newdriver_license_detailRow();
                        DrvLisnRow.driver_id = dtlicense.Rows[0]["driver_id"].ToString();
                        DrvLisnRow.License_no = dtlicense.Rows[0]["License_no"].ToString();
                        DrvLisnRow.License_type = dtlicense.Rows[0]["License_type"].ToString();
                        DrvLisnRow.Issued_place = dtlicense.Rows[0]["Issued_place"].ToString();
                        if (dtlicense.Rows[0]["Valid_from"].ToString() != "")
                            DrvLisnRow.Valid_from = Convert.ToDateTime(dtlicense.Rows[0]["Valid_from"].ToString());
                        if (dtlicense.Rows[0]["Valid_upto"].ToString() != "")
                            DrvLisnRow.Valid_upto = Convert.ToDateTime(dtlicense.Rows[0]["Valid_upto"].ToString());

                        DrvLisnRow.id_path = dtlicense.Rows[0]["id_path"].ToString();
                        if (filePath.Keys.Contains("license_photo"))
                            DrvLisnRow.id_path = filePath["license_photo"];

                        DrvLisnRow.status = dtlicense.Rows[0]["status"].ToString();
                        DrvLisnRow.active_flag = Constant.Flag_Yes;
                        DrvLisnRow.created_by = dtlicense.Rows[0]["created_by"].ToString();
                        DrvLisnRow.created_date = Convert.ToDateTime(dtlicense.Rows[0]["created_date"].ToString());
                        DrvLisnRow.created_host = dtlicense.Rows[0]["created_host"].ToString();
                        DrvLisnRow.device_id = dtlicense.Rows[0]["device_id"].ToString();
                        DrvLisnRow.device_type = dtlicense.Rows[0]["device_type"].ToString();
                        DrvLisnRow.modified_by = dtlicense.Rows[0]["created_by"].ToString();
                        DrvLisnRow.modified_date = System.DateTime.UtcNow;
                        DrvLisnRow.modified_host = dtlicense.Rows[0]["created_host"].ToString();
                        DrvLisnRow.modified_device_id = dtlicense.Rows[0]["device_id"].ToString();
                        DrvLisnRow.modified_device_type = dtlicense.Rows[0]["device_type"].ToString();
                        objdriver.driver_license_detail.Adddriver_license_detailRow(DrvLisnRow);
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("Driver License udpate : = " + ex.Message.ToString() + "-" + driverid + Environment.NewLine + ex.StackTrace);
                return ex.Message.ToString();
            }
            #endregion

            objBLReturnObject = drvmst.Update_driver_table(objdriver);

            if (objBLReturnObject.ExecutionStatus == 1)
            {
                #region Delete Previous File


                if (filePath.Keys.Contains("license_photo"))
                {
                    if (dtlicense != null)
                        new Master().DeleteFile(dtlicense.Rows[0]["id_path"].ToString().Trim());
                }
                if (filePath.Keys.Contains("driver_photo"))
                {
                    if (dtdrivermst != null)
                        new Master().DeleteFile(dtdrivermst.Rows[0]["driver_photo"].ToString().Trim());
                }

                #endregion

                return objBLReturnObject.ServerMessage;
            }
            else
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                foreach (var pair in data)
                {
                    filePath[pair.Key] = pair.Value;
                    var filePazxth = HttpContext.Current.Server.MapPath(pair.Value);
                    if (File.Exists(filePazxth))
                        File.Delete(filePazxth);
                }
                return objBLReturnObject.ServerMessage;
            }


        }

        [HttpGet]
        public string GetDriverdetailsBydriverid(string driverID)
        {
            String query = " select * from driver_mst " +
                           " left join  user_mst on user_mst.unique_id=driver_mst.driver_id " +
                           " left join  driver_contact_detail on driver_contact_detail.driver_id=driver_mst.driver_id " +
                           " left join  driver_license_detail on driver_license_detail.driver_id=driver_mst.driver_id " +
                           " left join  driver_prefered_destination on driver_prefered_destination.driver_id=driver_mst.driver_id " +
                           " left join  driver_truck_details on driver_truck_details.driver_id=driver_mst.driver_id " +
                           "  where driver_mst.driver_id='" + driverID + "'  ";

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
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "Driver details not found");
        }

        public DataTable GetDriverTruckandLatlongDetails(String driverid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select distinct driver_truck_details.driver_id,truck_current_position.truck_lat,truck_current_position.truck_lng,driver_license_detail.License_no,truck_dtl.* 
                         from driver_truck_details 
                         join driver_mst on driver_mst.driver_id = driver_truck_details.driver_id 
                         join driver_license_detail on driver_license_detail.driver_id = driver_truck_details.driver_id 
                         join driver_contact_detail on driver_contact_detail.driver_id = driver_truck_details.driver_id 
                         join ( SELECT distinct truck_mst.truck_id,truck_body_mst.truck_body_id,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, 
                         truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, 
                         truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo 
                         from truck_mst 
                         left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id 
                         left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
                         left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id 
                         left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id 
                         left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ) as truck_dtl on truck_dtl.truck_id =  driver_truck_details.truck_id 
                         left join truck_current_position on driver_truck_details.driver_id =  truck_current_position.driver_id and truck_current_position.active_flag='Y'
                         where driver_truck_details.driver_id='" + driverid + "'");

            DataTable dt = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
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
        public DataTable Getdriverordernotification(string loadinqno, string driverid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select active_flag,* from  driver_order_notifications where DATEADD(minute,1,notification_senddate)>GETUTCDATE() and  load_inquiry_no = @inqid  and active_flag='Y' ";
            //String query1 = "SELECT * FROM driver_order_notifications where load_inquiry_no = @inqid and driver_id='" + driverid + "' and active_flag='Y'";
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

        public DataTable GetdriverordernotificationNew(string loadinqno, string driverid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "select active_flag,* from  driver_order_notifications where DATEADD(minute,1,notification_senddate)>GETUTCDATE()  and load_inquiry_no = @inqid  and active_flag='Y' ";
            //String query1 = "SELECT * FROM driver_order_notifications where load_inquiry_no = @inqid and driver_id='" + driverid + "' and active_flag='Y'";
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
        public DataTable GetdriverordernotificationHistoryNew(string loadinqno, string driverid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM driver_order_notifications_history where load_inquiry_no = @inqid and driver_id=@driverid and active_flag='Y' ";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, driverid));
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
        public DataTable GetOrderDetailsNearByLocationNew(string lat, string lng, string MinDistance, string TruckTypeCode, string driverID)
        {
            string current_lat = ""; string current_lng = ""; string MinDist = ""; string truckType = "";

            current_lat = lat;
            current_lng = lng;
            MinDist = MinDistance;
            truckType = TruckTypeCode;


            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();

            query1 = " declare @orig_lat decimal " +
                   " declare @orig_lon decimal " +
                   " declare @dist decimal " +
                   " set @orig_lat=" + current_lat + "; set @orig_lon=" + current_lng + "; " +
                   " set @dist=" + MinDist + "; " +
                   " select  * from " +
                   " ( SELECT orders.*,  3956 * 2 * ASIN(SQRT( " +
                   " POWER(SIN((@orig_lat - abs(case when orders.inquiry_source_lat='' Then '0' else orders.inquiry_source_lat end)) * pi()/180 / 2), 2) +  COS(@orig_lat * pi()/180 ) *  " +
                   " COS(abs(case when orders.inquiry_source_lat='' Then '0' else orders.inquiry_source_lat end) * pi()/180) *   " +
                   " POWER(SIN((@orig_lon - case when orders.inquiry_source_lng='' Then '0' else orders.inquiry_source_lng end) * pi()/180 / 2), 2) )) as  distance   " +
                   " FROM orders) as temp where active_flag='Y' and temp.order_type_flag in ('GL','GN') and temp.isassign_driver_truck='N'  " +
                   "  and temp.SizeTypeCode='" + TruckTypeCode + "' and temp.load_inquiry_no not in ( select load_inquiry_no from driver_order_notifications_history where driver_id='" + driverID + "')  order by distance asc , temp.created_date desc ";
            //" and   DATEDIFF(hour, created_date,GETDATE()) < 10 " +
            //" and distance < 2 order by distance  ";

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
        public Boolean SaveDriverOrderNotificationDetailsNew(ref IDbCommand DBCommand, ref DS_driver_order_notifications ds_driverordernotification, DataRow drorder, DataTable dtdrivertruckandLatlngDtl, ref string Msg)
        {
            try
            {

                Document objdoc = new Document();

                string DocNtficID = ""; string Message = "";

                if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "DRN", "", "", ref DocNtficID, ref Message)) // New Driver Notification ID
                {
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                }


                if (drorder != null)
                {
                    ds_driverordernotification.EnforceConstraints = false;
                    ds_driverordernotification.driver_order_notifications.ImportRow(drorder);
                    if (ds_driverordernotification.driver_order_notifications.Rows.Count == 1)
                    {
                        ds_driverordernotification.driver_order_notifications[0].status = "02";
                        ds_driverordernotification.driver_order_notifications[0].driver_id = dtdrivertruckandLatlngDtl.Rows[0]["driver_id"].ToString();
                        ds_driverordernotification.driver_order_notifications[0].truck_id = dtdrivertruckandLatlngDtl.Rows[0]["truck_id"].ToString();
                        ds_driverordernotification.driver_order_notifications[0].active_flag = Constant.FLAG_Y;
                        ds_driverordernotification.driver_order_notifications[0].created_date = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications[0].Notification_SendDate = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications[0].AcceptChanges();
                        ds_driverordernotification.driver_order_notifications[0].SetAdded();
                    }
                    else
                    {
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].status = "02";
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].driver_id = dtdrivertruckandLatlngDtl.Rows[0]["driver_id"].ToString();
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].truck_id = dtdrivertruckandLatlngDtl.Rows[0]["truck_id"].ToString();
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].active_flag = Constant.FLAG_Y;
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].created_date = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].Notification_SendDate = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].AcceptChanges();
                        ds_driverordernotification.driver_order_notifications[ds_driverordernotification.driver_order_notifications.Rows.Count - 1].SetAdded();
                    }




                    ds_driverordernotification.driver_order_notifications_history.ImportRow(drorder);
                    if (ds_driverordernotification.driver_order_notifications_history.Rows.Count == 1)
                    {
                        ds_driverordernotification.driver_order_notifications_history[0].notification_id = DocNtficID;
                        ds_driverordernotification.driver_order_notifications_history[0].status = "02";
                        ds_driverordernotification.driver_order_notifications_history[0].driver_id = dtdrivertruckandLatlngDtl.Rows[0]["driver_id"].ToString();
                        ds_driverordernotification.driver_order_notifications_history[0].truck_id = dtdrivertruckandLatlngDtl.Rows[0]["truck_id"].ToString();
                        ds_driverordernotification.driver_order_notifications_history[0].active_flag = Constant.FLAG_Y;
                        ds_driverordernotification.driver_order_notifications_history[0].created_date = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications_history[0].AcceptChanges();
                        ds_driverordernotification.driver_order_notifications_history[0].SetAdded();
                    }
                    else
                    {
                        int drvcount = ds_driverordernotification.driver_order_notifications_history.Rows.Count - 1;
                        ds_driverordernotification.driver_order_notifications_history[drvcount].notification_id = DocNtficID;
                        ds_driverordernotification.driver_order_notifications_history[drvcount].status = "02";
                        ds_driverordernotification.driver_order_notifications_history[drvcount].driver_id = dtdrivertruckandLatlngDtl.Rows[0]["driver_id"].ToString();
                        ds_driverordernotification.driver_order_notifications_history[drvcount].truck_id = dtdrivertruckandLatlngDtl.Rows[0]["truck_id"].ToString();
                        ds_driverordernotification.driver_order_notifications_history[drvcount].active_flag = Constant.FLAG_Y;
                        ds_driverordernotification.driver_order_notifications_history[drvcount].created_date = DateTime.UtcNow;
                        ds_driverordernotification.driver_order_notifications_history[drvcount].AcceptChanges();
                        ds_driverordernotification.driver_order_notifications_history[drvcount].SetAdded();
                    }


                }

                ds_driverordernotification.EnforceConstraints = true;

                return true;
            }
            catch (ConstraintException ce)
            {
                ServerLog.Log(ce.Message + ce.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }
        }
        public StringBuilder ValidateDriverOrderNew(DataRow drOrder, string driverid)
        {

            StringBuilder sb = new StringBuilder();
            DateTime Dr_PreviousOrdershippingdatetime = new DateTime();
            DateTime TotalDr_PreviousOrderdatetime = new DateTime();
            DateTime Dr_NextOrdershippingdatetime = new DateTime();
            DateTime TotalDr_NextOrderdatetime = new DateTime();

            DateTime NewOrdershippingdatetime = Convert.ToDateTime(drOrder["shippingdatetime"].ToString());
            TimeSpan Newordertime = new TimeSpan();
            Newordertime = TimeSpan.Parse(drOrder["TimeToTravelInMinute"].ToString());
            DateTime NewOrderDatetime = NewOrdershippingdatetime.AddMinutes(Newordertime.TotalMinutes);
            NewOrderDatetime = NewOrderDatetime.AddHours(2);

            //driver previous order details based on new order shipping date 
            DataTable dtDriverPreviousorders = GetDriverOrdersByID(driverid, NewOrdershippingdatetime.ToString(), "1");
            DataTable dtDriverNextorders = GetDriverOrdersByID(driverid, NewOrdershippingdatetime.ToString(), "2");

            if (dtDriverPreviousorders != null)
            {
                Dr_PreviousOrdershippingdatetime = Convert.ToDateTime(dtDriverPreviousorders.Rows[0]["shippingdatetime"].ToString());
                TimeSpan DriverPreviousOrdertime = new TimeSpan();
                DriverPreviousOrdertime = TimeSpan.Parse(dtDriverPreviousorders.Rows[0]["TimeToTravelInMinute"].ToString());
                TotalDr_PreviousOrderdatetime = Dr_PreviousOrdershippingdatetime.AddHours(DriverPreviousOrdertime.TotalHours);
                TotalDr_PreviousOrderdatetime = TotalDr_PreviousOrderdatetime.AddHours(2);

                if (TotalDr_PreviousOrderdatetime > NewOrdershippingdatetime)
                    sb.Append("You are busy in order free on '" + TotalDr_PreviousOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + "' and Current Order shipping  date is '" + NewOrdershippingdatetime.ToString("dd-MM-yyyy HH:mm:ss tt"));
            }
            else if (dtDriverNextorders != null)
            {
                Dr_NextOrdershippingdatetime = Convert.ToDateTime(dtDriverNextorders.Rows[0]["shippingdatetime"].ToString());
                TotalDr_NextOrderdatetime = Dr_NextOrdershippingdatetime.AddHours(-2);

                if (TotalDr_NextOrderdatetime < NewOrderDatetime)
                    sb.Append("You are busy in order. completion time  of received ordered will be '" + NewOrderDatetime.ToString("dd-MM-yyy HH:mm:ss tt") + "' and You already having order at " + TotalDr_NextOrderdatetime.ToString("dd-MM-yyyy HH:mm:ss tt") + " ");
            }

            return sb;
        }

        [HttpGet]
        public DataTable GetdriverordernotificationHistory(string loadinqno, string driverid)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM driver_order_notifications_history where load_inquiry_no = @inqid and driver_id=@driverid and active_flag='Y' ";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqno));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, driverid));
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

        public DataTable GetOrderDetailsNearByLocation(string lat, string lng, string MinDistance, string TruckTypeCode, string driverID)
        {
            string current_lat = ""; string current_lng = ""; string MinDist = ""; string truckType = "";

            current_lat = lat;
            current_lng = lng;
            MinDist = MinDistance;
            truckType = TruckTypeCode;


            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();

            query1 = " declare @orig_lat decimal " +
                   " declare @orig_lon decimal " +
                   " declare @dist decimal " +
                   " set @orig_lat=" + current_lat + "; set @orig_lon=" + current_lng + "; " +
                   " set @dist=" + MinDist + "; " +
                   " select  * from " +
                   " ( SELECT orders.*,  3956 * 2 * ASIN(SQRT( " +
                   " POWER(SIN((@orig_lat - abs(case when orders.inquiry_source_lat='' Then '0' else orders.inquiry_source_lat end)) * pi()/180 / 2), 2) +  COS(@orig_lat * pi()/180 ) *  " +
                   " COS(abs(case when orders.inquiry_source_lat='' Then '0' else orders.inquiry_source_lat end) * pi()/180) *   " +
                   " POWER(SIN((@orig_lon - case when orders.inquiry_source_lng='' Then '0' else orders.inquiry_source_lng end) * pi()/180 / 2), 2) )) as  distance   " +
                   " FROM orders) as temp where active_flag='Y' and temp.order_type_flag in ('GL','GN') and temp.isassign_driver_truck='N'  " +
                   "  and temp.SizeTypeCode='" + TruckTypeCode + "' and temp.load_inquiry_no not in ( select load_inquiry_no from driver_order_notifications where driver_id='" + driverID + "')  order by distance asc , temp.created_date desc ";
            //" and   DATEDIFF(hour, created_date,GETDATE()) < 10 " +
            //" and distance < 2 order by distance  ";

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

        //[HttpGet]
        //public string GetDriver_order_RequestDetailsNew(string DriverID, string deviceid)
        //{

        //    //DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(DriverID);
        //    //if (dtdeviceIds != null)
        //    //{
        //    //    DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
        //    //    if (dr.Length > 0)
        //    //        if (dr[0].ItemArray[3].ToString() != deviceid)
        //    //        {
        //    //            return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
        //    //        }
        //    //}


        //    try
        //    {
        //        PostOrderController obj = new PostOrderController();
        //        DataSet dsdrvdtl = new DataSet();
        //        BLReturnObject objBLobj = new BLReturnObject(); Document objdoc = new Document();
        //        Master master = new Master();
        //        DateTime Ordershippingdatetime = new DateTime();
        //        DataTable dt_DriverTruckdetails = new DataTable();
        //        DS_driver_order_notifications ds_driverordernotification = new DS_driver_order_notifications();
        //        DataTable dtdrvordernotifidtl = new DataTable();
        //        DataTable dtdrvnotdtl = new DataTable(); DataTable dtdrvnotdtlhistory = new DataTable();
        //        DataTable dtdrivertruckandLatlngDtl = GetDriverTruckandLatlongDetails(DriverID);


        //        if (dtdrivertruckandLatlngDtl != null)
        //        {
        //            DataTable dt_orders = GetOrderDetailsNearByLocation(dtdrivertruckandLatlngDtl.Rows[0]["truck_lat"].ToString(), dtdrivertruckandLatlngDtl.Rows[0]["truck_lng"].ToString(), "2", dtdrivertruckandLatlngDtl.Rows[0]["truck_body_id"].ToString(), DriverID);
        //            if (dt_orders != null)
        //            {
        //                for (int i = 0; i < dt_orders.Rows.Count; i++)
        //                {

        //                    dtdrvordernotifidtl = GetdriverordernotificationNew(dt_orders.Rows[i]["load_inquiry_no"].ToString(), DriverID);
        //                    if (dtdrvordernotifidtl == null)
        //                    {

        //                        dtdrvnotdtlhistory = GetdriverordernotificationHistory(dt_orders.Rows[i]["load_inquiry_no"].ToString(), DriverID);
        //                        if (dtdrvnotdtlhistory == null)
        //                        {
        //                            #region Save Driver Allocation Details


        //                            DBConnection.Open();
        //                            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
        //                            DBCommand.Transaction = DBConnection.BeginTransaction();

        //                            string Msg = "";
        //                            Boolean Result = SaveDriverOrderNotificationDetails(ref DBCommand, ref ds_driverordernotification, dt_orders, dtdrivertruckandLatlngDtl, ref Msg);
        //                            dtdrvnotdtl = ds_driverordernotification.driver_order_notifications;

        //                            if (Result)
        //                            {
        //                                objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications, ref DBCommand);
        //                                if (objBLobj.ExecutionStatus == 2)
        //                                {
        //                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    objBLobj.ExecutionStatus = 2;
        //                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
        //                                }

        //                                objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications_history, ref DBCommand);
        //                                if (objBLobj.ExecutionStatus == 2)
        //                                {
        //                                    ServerLog.Log(objBLobj.ServerMessage.ToString());
        //                                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                    objBLobj.ExecutionStatus = 2;
        //                                    return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
        //                                }

        //                                DBCommand.Transaction.Commit();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                objBLobj.ExecutionStatus = 1;
        //                                ServerLog.SuccessLog("Quote Request available ");

        //                                dtdrvnotdtl.Columns.Add("NoOfHandiman", typeof(string));
        //                                dtdrvnotdtl.Rows[0]["NoOfHandiman"] = dt_orders.Rows[0]["NoOfHandiman"].ToString();

        //                                dtdrvnotdtl.Columns.Add("NoOfLabour", typeof(string));
        //                                dtdrvnotdtl.Rows[0]["NoOfLabour"] = dt_orders.Rows[0]["NoOfLabour"].ToString();

        //                                dtdrvnotdtl.Columns.Add("Total_cost", typeof(string));
        //                                dtdrvnotdtl.Rows[0]["Total_cost"] = dt_orders.Rows[0]["Total_cost"].ToString();

        //                                dtdrvnotdtl.Columns.Add("shippingdatetime", typeof(string));
        //                                dtdrvnotdtl.Rows[0]["shippingdatetime"] = dt_orders.Rows[0]["shippingdatetime"].ToString();

        //                            }
        //                            else
        //                            {
        //                                ServerLog.Log(objBLobj.ServerMessage.ToString());
        //                                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                                objBLobj.ExecutionStatus = 2;
        //                                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
        //                            }

        //                            #endregion
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //                return BLGeneralUtil.return_ajax_string("0", "Data Not found ");

        //            return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrvnotdtl));
        //        }
        //        else
        //            return BLGeneralUtil.return_ajax_string("0", "Data Not found ");
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.Log(ex.Message + ex.StackTrace);
        //        DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}

        //[HttpGet]
        //public string GetDriver_order_RequestDetailsNew1(string DriverID, string deviceid)
        //{

        //    //DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(DriverID);
        //    //if (dtdeviceIds != null)
        //    //{
        //    //    DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
        //    //    if (dr.Length > 0)
        //    //        if (dr[0].ItemArray[3].ToString() != deviceid)
        //    //        {
        //    //            return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
        //    //        }
        //    //}


        //    try
        //    {
        //        PostOrderController obj = new PostOrderController();
        //        DataSet dsdrvdtl = new DataSet();
        //        BLReturnObject objBLobj = new BLReturnObject(); Document objdoc = new Document();
        //        Master master = new Master();
        //        DateTime Ordershippingdatetime = new DateTime();
        //        DataTable dt_DriverTruckdetails = new DataTable();
        //        DS_driver_order_notifications ds_driverordernotification = new DS_driver_order_notifications();
        //        DataTable dtdrvordernotifidtl = new DataTable();
        //        DataTable dtdrvnotdtl = new DataTable(); DataTable dtdrvnotdtlhistory = new DataTable();
        //        DataTable dtdrivertruckandLatlngDtl = GetDriverTruckandLatlongDetails(DriverID);


        //        if (dtdrivertruckandLatlngDtl != null)
        //        {
        //            DBConnection.Open();
        //            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
        //            DBCommand.Transaction = DBConnection.BeginTransaction();

        //            DataTable dt_orders = GetOrderDetailsNearByLocation(dtdrivertruckandLatlngDtl.Rows[0]["truck_lat"].ToString(), dtdrivertruckandLatlngDtl.Rows[0]["truck_lng"].ToString(), "2", dtdrivertruckandLatlngDtl.Rows[0]["truck_body_id"].ToString(), DriverID);
        //            if (dt_orders != null)
        //            {
        //                //Convert.ToDateTime(drOrder["shippingdatetime"].ToString());
        //                dtdrvordernotifidtl = Getdriverordernotification(dt_orders.Rows[0]["load_inquiry_no"].ToString(), DriverID);
        //                if (dtdrvordernotifidtl != null)
        //                {
        //                    if (dtdrvordernotifidtl.Rows[0]["driver_id"].ToString() == DriverID)
        //                    {
        //                        ds_driverordernotification.EnforceConstraints = false;
        //                        ds_driverordernotification.driver_order_notifications.ImportRow(dt_orders.Rows[0]);
        //                        ds_driverordernotification.driver_order_notifications[0].status = "02";
        //                        ds_driverordernotification.driver_order_notifications[0].driver_id = DriverID;
        //                        ds_driverordernotification.driver_order_notifications[0].truck_id = dtdrivertruckandLatlngDtl.Rows[0]["truck_id"].ToString(); ;
        //                        ds_driverordernotification.driver_order_notifications[0].active_flag = Constant.FLAG_N;
        //                        ds_driverordernotification.driver_order_notifications[0].AcceptChanges();
        //                        ds_driverordernotification.driver_order_notifications[0].SetAdded();
        //                    }
        //                    else
        //                    {
        //                        dtdrvordernotifidtl = Getdriverordernotification(dt_orders.Rows[0]["load_inquiry_no"].ToString(), DriverID);
        //                        if (dtdrvordernotifidtl == null)
        //                        {
        //                            dtdrvnotdtlhistory = GetdriverordernotificationHistory(dt_orders.Rows[0]["load_inquiry_no"].ToString(), DriverID);
        //                            if (dtdrvnotdtlhistory == null)
        //                            {
        //                                string Msg = "";
        //                                Boolean Result = SaveDriverOrderNotificationDetails(ref DBCommand, ref ds_driverordernotification, dt_orders, dtdrivertruckandLatlngDtl, ref Msg);
        //                                dtdrvnotdtl = ds_driverordernotification.driver_order_notifications;

        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    dtdrvordernotifidtl = Getdriverordernotification(dt_orders.Rows[0]["load_inquiry_no"].ToString(), DriverID);
        //                    if (dtdrvordernotifidtl == null)
        //                    {

        //                        dtdrvnotdtlhistory = GetdriverordernotificationHistory(dt_orders.Rows[0]["load_inquiry_no"].ToString(), DriverID);
        //                        if (dtdrvnotdtlhistory == null)
        //                        {
        //                            string Msg = "";
        //                            Boolean Result = SaveDriverOrderNotificationDetails(ref DBCommand, ref ds_driverordernotification, dt_orders, dtdrivertruckandLatlngDtl, ref Msg);
        //                            dtdrvnotdtl = ds_driverordernotification.driver_order_notifications;
        //                        }
        //                    }
        //                }
        //            }


        //            objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications, ref DBCommand);
        //            if (objBLobj.ExecutionStatus == 2)
        //            {
        //                ServerLog.Log(objBLobj.ServerMessage.ToString());
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLobj.ExecutionStatus = 2;
        //                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
        //            }

        //            objBLobj = master.UpdateTables(ds_driverordernotification.driver_order_notifications_history, ref DBCommand);
        //            if (objBLobj.ExecutionStatus == 2)
        //            {
        //                ServerLog.Log(objBLobj.ServerMessage.ToString());
        //                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLobj.ExecutionStatus = 2;
        //                return BLGeneralUtil.return_ajax_string("0", objBLobj.ServerMessage);
        //            }

        //            DBCommand.Transaction.Commit();
        //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //            objBLobj.ExecutionStatus = 1;
        //            ServerLog.SuccessLog("Quote Request available ");

        //            if (dtdrvnotdtl != null && dtdrvnotdtl.Rows.Count > 0)
        //            {
        //                dtdrvnotdtl.Columns.Add("NoOfHandiman", typeof(string));
        //                dtdrvnotdtl.Rows[0]["NoOfHandiman"] = dt_orders.Rows[0]["NoOfHandiman"].ToString();

        //                dtdrvnotdtl.Columns.Add("NoOfLabour", typeof(string));
        //                dtdrvnotdtl.Rows[0]["NoOfLabour"] = dt_orders.Rows[0]["NoOfLabour"].ToString();

        //                dtdrvnotdtl.Columns.Add("Total_cost", typeof(string));
        //                dtdrvnotdtl.Rows[0]["Total_cost"] = dt_orders.Rows[0]["Total_cost"].ToString();

        //                dtdrvnotdtl.Columns.Add("shippingdatetime", typeof(string));
        //                dtdrvnotdtl.Rows[0]["shippingdatetime"] = dt_orders.Rows[0]["shippingdatetime"].ToString();
        //                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrvnotdtl));
        //            }
        //            else
        //                return BLGeneralUtil.return_ajax_string("0", "Data Not found ");
        //        }
        //        else
        //        {
        //            return BLGeneralUtil.return_ajax_string("0", "Data Not found ");
        //        }




        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.Log(ex.Message + ex.StackTrace);
        //        DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}

        [HttpGet]
        public string GetDriver_order_RequestDetails(string DriverID, string deviceid)
        {

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(DriverID);
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
                PostOrderController obj = new PostOrderController();
                DataSet dsdrvdtl = new DataSet();
                BLReturnObject objBLobj = new BLReturnObject(); Document objdoc = new Document();
                Master master = new Master();
                DateTime Ordershippingdatetime = new DateTime();
                DataTable dt_DriverTruckdetails = new DataTable();
                DS_driver_order_notifications ds_driverordernotification = new DS_driver_order_notifications();
                DataTable dtdrvordernotifidtl = new DataTable();
                DataTable dtdrvnotdtl = new DataTable(); DataTable dtdrvnotdtlhistory = new DataTable();
                DataTable dtdrivertruckandLatlngDtl = GetDriverTruckandLatlongDetails(DriverID);


                if (dtdrivertruckandLatlngDtl != null)
                {
                    DBConnection.Open();
                    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    DataTable dt_orders = GetOrderDetailsNearByLocationNew(dtdrivertruckandLatlngDtl.Rows[0]["truck_lat"].ToString(), dtdrivertruckandLatlngDtl.Rows[0]["truck_lng"].ToString(), "2", dtdrivertruckandLatlngDtl.Rows[0]["truck_body_id"].ToString(), DriverID);
                    if (dt_orders != null)
                    {
                        for (int i = 0; i < dt_orders.Rows.Count; i++)
                        {
                            StringBuilder sb_busyin_order = ValidateDriverOrderNew(dt_orders.Rows[i], DriverID);
                            if (sb_busyin_order.Length == 0)
                            {
                                dtdrvordernotifidtl = GetdriverordernotificationNew(dt_orders.Rows[i]["load_inquiry_no"].ToString(), DriverID);
                                if (dtdrvordernotifidtl == null)
                                {

                                    dtdrvnotdtlhistory = GetdriverordernotificationHistoryNew(dt_orders.Rows[i]["load_inquiry_no"].ToString(), DriverID);
                                    if (dtdrvnotdtlhistory == null)
                                    {
                                        string Msg = "";
                                        Boolean Result = SaveDriverOrderNotificationDetailsNew(ref DBCommand, ref ds_driverordernotification, dt_orders.Rows[i], dtdrivertruckandLatlngDtl, ref Msg);
                                        dtdrvnotdtl = ds_driverordernotification.driver_order_notifications;

                                        if (dtdrvnotdtl != null && dtdrvnotdtl.Rows.Count > 0)
                                            i = dtdrvnotdtl.Rows.Count;
                                    }
                                }
                            }
                        }
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

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLobj.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Quote Request available ");

                    if (dtdrvnotdtl != null && dtdrvnotdtl.Rows.Count > 0)
                    {
                        dtdrvnotdtl.Columns.Add("NoOfHandiman", typeof(string));
                        dtdrvnotdtl.Rows[0]["NoOfHandiman"] = dt_orders.Rows[0]["NoOfHandiman"].ToString();

                        dtdrvnotdtl.Columns.Add("NoOfLabour", typeof(string));
                        dtdrvnotdtl.Rows[0]["NoOfLabour"] = dt_orders.Rows[0]["NoOfLabour"].ToString();

                        dtdrvnotdtl.Columns.Add("Total_cost", typeof(string));
                        dtdrvnotdtl.Rows[0]["Total_cost"] = dt_orders.Rows[0]["Total_cost"].ToString();

                        dtdrvnotdtl.Columns.Add("shippingdatetime", typeof(string));
                        dtdrvnotdtl.Rows[0]["shippingdatetime"] = dt_orders.Rows[0]["shippingdatetime"].ToString();
                        return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtdrvnotdtl));
                    }
                    else
                        return BLGeneralUtil.return_ajax_string("0", "Data Not found ");
                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", "Data Not found ");
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }


        // GET api/admin/5
        public string GetDriverTruckDetails(string opt, string loadinqid, string driverno)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "";

            if (opt == "C")
            {
                query1 += " select * from truck_current_position join user_mst on  user_mst.unique_id= truck_current_position.device_id Where 1=1  ";

                if (loadinqid != null)
                    query1 += " and truck_current_position.load_inquiry_no=@inqid ";
            }
            else
            {
                query1 += " select * from truck_current_position_history join user_mst on  user_mst.unique_id= truck_current_position_history.device_id Where 1=1 ";
                if (loadinqid != null)
                    query1 += " and truck_current_position_history.load_inquiry_no=@inqid ";
            }

            if (driverno != null)
                query1 += "and user_mst.user_id=@driverno";

            query1 += " order by log_date desc ";


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            if (loadinqid != null)
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("inqid", DbType.String, loadinqid));

            if (driverno != null)
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverno", DbType.String, driverno));

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
                return (BLGeneralUtil.return_ajax_string("0", "Driver Truck details not found "));
        }


        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            string result = "";
            string josndata = "";
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                int count = 0;
                var docfiles = new List<string>();

                DateTime dt = DateTime.Now;
                string str = dt.ToString("yyyyMMddHHmmssfff") + "_";
                try
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath1 = HttpContext.Current.Server.MapPath("~/Images/" + str + postedFile.FileName);

                        var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~//DriverFiles//" + str + postedFile.FileName);
                        string imagepathe = "//ProfileImage//" + str + postedFile.FileName;
                        postedFile.SaveAs(filePath);

                        docfiles.Add(filePath);
                        // josndata = "{\"image\":\"" + imagepathe + "\"}";
                    }
                    // httpRequest["email"].ToString();


                    //  result = BLGeneralUtil.return_ajax_data("1", josndata);
                    response = Request.CreateResponse(HttpStatusCode.Accepted, "1");





                }
                catch (Exception ex)
                {
                    response.StatusCode = 0;
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);

                }
            }
            else
            {
                response.StatusCode = 0;
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "no File Fount");
            }
            return response;
        }

    }
}

