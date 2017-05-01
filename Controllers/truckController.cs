using Newtonsoft.Json.Linq;
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
using trukkerUAE.XSD;
using System.Web;
using BLL.Utilities;
using BLL.Master;
using System.Text;
using trukkerUAE.Classes;
using Newtonsoft.Json;
using System.IO;


namespace trukkerUAE.Controllers
{
    public class truckController : ServerBase
    {
        Master trukmst = new Master();
        DS_Truck_Mst objtruck = new DS_Truck_Mst();
        BLReturnObject objBLReturnObject = new BLReturnObject();
        JavaScriptSerializer jser = new JavaScriptSerializer();
        StringBuilder sb = new StringBuilder();

        #region Get methods

        [HttpGet]
        public string GetTruckDetails(HttpClient ht)
        {
            DataTable dtTruck = new DataTable();
            String query = "SELECT * FROM truck_mst";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTruck = ds.Tables[0];
            }

            if (dtTruck != null && dtTruck.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTruck)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "No Truck Details Found "));
        }

        //GET truckModel Data by madhuri on 24/10/15
        [HttpGet]
        public string GetTruckModelData1()
        {

            String query = "SELECT model_id,model_desc FROM truck_model_mst where active_flag='Y'";
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
                return (BLGeneralUtil.return_ajax_string("0", "Truck model details not found "));

        }

        [HttpGet]
        public string GetTruckModelData()
        {
            String query = "SELECT model_id,model_desc FROM truck_model_mst where active_flag='Y'";
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
                return (BLGeneralUtil.return_ajax_string("0", "Truck model details not found "));

        }

        [HttpGet]
        public string GetTruckMakeData()
        {
            String query = " SELECT make_id,make_name FROM truck_make_mst where active_flag='Y' ";
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
                return (BLGeneralUtil.return_ajax_string("0", "Truck model details not found "));

        }

        //GET truckModels make_id wise by pooja vachhani on 26/10/15

        [HttpGet]
        public string GetTruckModelDataIdWise(string make_id)
        {
            String query = "";
            if (make_id != "-1")
                query = "SELECT model_id,model_desc FROM truck_model_mst where active_flag='Y' and make_id = '" + make_id + "'";
            else
                query = "SELECT model_id,make_id,model_desc FROM truck_model_mst where active_flag='Y'";

            DataTable dtTruck = new DataTable();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtTruck = ds.Tables[0];
            }
            if (dtTruck != null && dtTruck.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtTruck)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Truck model details not found "));
        }

        [HttpGet]
        public DataTable GetTruckCurrentLocation(string truckid, string loadinq)
        {
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            if (truckid == "-1" && loadinq != "-1")
            {
                query1 = "SELECT * FROM truck_current_position where load_inquiry_no = @loadno order by log_date  ";
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, loadinq));
            }
            else
            {
                query1 = "SELECT * FROM truck_current_position where truck_id = @trkid and load_inquiry_no = @loadno and active_flag= 'Y' ";
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("trkid", DbType.String, truckid));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, loadinq));
            }

            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            //DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                //return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                return dtPostLoadOrders;
            else
                return null;
            //return BLGeneralUtil.return_ajax_string("0", "No Order found ");
        }

        [HttpGet]
        public string GetAllTrucksByUser(string userid)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT     truck_mst.truck_id, truck_mst.reg_date, truck_mst.truck_make_id, truck_mst.truck_model, truck_mst.year_of_mfg, truck_mst.load_capacity, truck_mst.axle_detail,");
            sb.Append("truck_mst.body_type, truck_mst.current_millage, truck_mst.avg_millage_per_month, truck_mst.fuel_average, truck_mst.registration_details, truck_mst.active_flag,  ");
            sb.Append("truck_mst.created_by, truck_mst.created_date, truck_mst.created_host, truck_mst.device_id, truck_mst.device_type, truck_mst.modified_by, truck_mst.modified_date, ");
            sb.Append("truck_mst.modified_host, truck_mst.modified_device_id, truck_mst.modified_device_type, truck_make_mst.make_name, truck_model_mst.model_desc,  ");
            sb.Append("truck_body_mst.truck_body_desc FROM         truck_mst LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id  ");
            sb.Append("LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model LEFT OUTER JOIN truck_make_mst ON ");
            sb.Append("truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id LEFT OUTER JOIN truck_axel_mst ON ");
            sb.Append("truck_mst.axle_detail = truck_axel_mst.axel_id  WHERE  truck_mst.active_flag = 'Y' AND truck_mst.created_by = @userid ");

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
            {
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                //return "1";
            }
            else
                return "0";
        }

        [HttpGet]
        public string GetAllTruckByUserForRegistration(string userid)
        {

            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc FROM TRUCK_MST ");
            sb.Append("left join owner_truck_details on truck_mst.truck_id = owner_truck_details.truck_id ");
            sb.Append("left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id ");
            sb.Append("left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id ");
            sb.Append("left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ");
            sb.Append("where truck_mst.created_by  = @userid and owner_truck_details.truck_id is null ");

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

        [HttpGet]
        public DataSet GetTruckById(string TrukId)
        {
            sb.Clear();
            sb.Append(@"SELECT     truck_mst.truck_id, truck_mst.reg_date, truck_mst.truck_make_id, truck_mst.truck_model, truck_mst.year_of_mfg, truck_mst.load_capacity, truck_mst.axle_detail, 
                        truck_mst.body_type, truck_mst.current_millage, truck_mst.avg_millage_per_month, truck_mst.fuel_average, truck_mst.registration_details, truck_mst.active_flag,  
                        truck_mst.created_by, truck_mst.created_date, truck_mst.created_host, truck_mst.device_id, truck_mst.device_type, truck_mst.modified_by, truck_mst.modified_date, 
                        truck_mst.modified_host, truck_mst.modified_device_id, truck_mst.modified_device_type, truck_make_mst.make_name, truck_model_mst.model_desc,  
                        truck_body_mst.truck_body_desc,truck_rto_registration_detail.*,truck_permit_details.*  FROM         truck_mst 
                        LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id  
                        LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model 
                        LEFT OUTER JOIN truck_make_mst ON truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id
                        LEFT OUTER JOIN truck_axel_mst ON truck_mst.axle_detail = truck_axel_mst.axel_id  
                        LEFT OUTER JOIN truck_rto_registration_detail ON truck_mst.truck_id = truck_rto_registration_detail.truck_id  
                        LEFT OUTER JOIN truck_permit_details ON truck_mst.truck_id = truck_permit_details.truck_id
                        WHERE  truck_mst.active_flag = 'Y' AND truck_mst.truck_id = @truckid 
                        select * from truck_insurance_detail where truck_id= @truckid  and active_flag='Y'
                        select * from truck_maintenance_detail where truck_id= @truckid  and active_flag='Y'");

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, TrukId));
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet dsOwner = new DataSet();
            DBDataAdpterObject.Fill(dsOwner);
            DataTable dttruck = new DataTable();
            if (dsOwner != null && dsOwner.Tables.Count > 0)
                return dsOwner;
            else
                return null;

            //if (dttruck != null && dttruck.Rows.Count > 0)
            //    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dsOwner.Tables[0]) + SendReceiveJSon.GetJson(dsOwner.Tables[1]) + SendReceiveJSon.GetJson(dsOwner.Tables[2])));
            //else
            //    return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        public DataTable GetTruckTableById(string TrukId)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT     truck_mst.truck_id, truck_mst.reg_date, truck_mst.truck_make_id, truck_mst.truck_model, truck_mst.year_of_mfg, truck_mst.load_capacity, truck_mst.axle_detail,");
            sb.Append("truck_mst.body_type, truck_mst.current_millage, truck_mst.avg_millage_per_month, truck_mst.fuel_average, truck_mst.registration_details, truck_mst.active_flag,  ");
            sb.Append("truck_mst.created_by, truck_mst.created_date, truck_mst.created_host, truck_mst.device_id, truck_mst.device_type, truck_mst.modified_by, truck_mst.modified_date, ");
            sb.Append("truck_mst.modified_host, truck_mst.modified_device_id, truck_mst.modified_device_type, truck_make_mst.make_name, truck_model_mst.model_desc,  ");
            sb.Append("truck_body_mst.truck_body_desc FROM         truck_mst LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id  ");
            sb.Append("LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model LEFT OUTER JOIN truck_make_mst ON ");
            sb.Append("truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id LEFT OUTER JOIN truck_axel_mst ON ");
            sb.Append("truck_mst.axle_detail = truck_axel_mst.axel_id  WHERE  truck_mst.active_flag = 'Y' AND truck_mst.truck_id = @truckid ");

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@truckid";
            pr1.Value = TrukId;


            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            cmd.Parameters.Add(pr1);

            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtOwner);

            if (dtOwner != null && dtOwner.Rows.Count > 0)
            {
                return dtOwner;
            }
            else
                return null;
        }

        [HttpGet]
        public JObject GetTruckMakeDataNew()
        {
            //SqlConnection con = new SqlConnection("Data Source=192.168.1.3\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT make_id,make_name FROM truck_make_mst where active_flag='Y'";

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

            JObject jsn = JObject.Parse(strDriver);
            return jsn;
            //return ser.Serialize(dataRows);
            // return strDriver;
        }

        //Get truckAxel Data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetTruckAxelData()
        {
            String query = "SELECT axel_id,axel_desc FROM truck_axel_mst where active_flag='Y'";
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
                return (BLGeneralUtil.return_ajax_string("0", "Truck Axcel details not found "));
        }

        //Get truck_body_mst data by pooja vachhani on 26/10/15
        [HttpGet]
        public string GetTruckBodyData()
        {
            String query = "SELECT truck_body_id, truck_body_desc FROM truck_body_mst where active_flag='Y'";
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
                return (BLGeneralUtil.return_ajax_string("0", "Truck BODY details not found "));
        }

        [HttpGet]
        public string GetStateDetails()
        {
            //SqlConnection con = new SqlConnection("Data Source=192.168.1.3\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT state_code, state_name FROM state_mst where active_flag='Y'";

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
        public string GetCityDetailsFromState(string state_code)
        {
            //SqlConnection con = new SqlConnection("Data Source=192.168.1.3\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            String query = "";
            if (state_code != "-1")
                query = "SELECT city_code, city_name FROM city_mst where state_code = '" + state_code + "' and active_flag='Y'";
            else
                query = "SELECT city_code, state_code , city_name FROM city_mst where active_flag='Y'";

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

        //pooja vachhani on 24/11/15
        [HttpGet]
        public string GetRegisteredTruckDetail()
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "select * from truck_mst";

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
        public string GetTruckPosition()
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "select * from truck_mst";

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

        // GET api/truck/5
        public string Get(int id)
        {
            return "value";
        }

        #endregion

        #region  Post method

        // POST api/truck

        //[HttpPost
        //public string Post_SaveImages([FromBody] JObject Images)
        //{

        //}


        //pooja vachhani on 25/11/15

        [HttpPost]
        public string Post_SaveTruckMst([FromBody] JObject Parameter)
        {
            var tempkey = Parameter.Properties().Select(p => p.Name).ToList();
            int flag = 0;
            Random rand = new Random();
            Random rand1 = new Random();
            int re = rand.Next(1, 1000000);
            string owid = "";
            ResponseCls responsecls = new ResponseCls();

            truck truckmstdata = new truck();
            truck_rto_registration_detail truck_rto_registration_detail_data = new truck_rto_registration_detail();
            truck_permit_details truck_permit_details_data = new truck_permit_details();
            owner_truck_details truck_Owner_data = new owner_truck_details();
            driver_truck_detail drivertruck = new driver_truck_detail();
            truck_insurance_detail truckinsurance = new truck_insurance_detail();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            if (tempkey.Contains("truck"))
                truckmstdata = Parameter["truck"].ToObject<truck>();
            if (tempkey.Contains("truck_rto_registration_detail"))
                truck_rto_registration_detail_data = Parameter["truck_rto_registration_detail"].ToObject<truck_rto_registration_detail>();
            if (tempkey.Contains("truck_permit_details"))
                truck_permit_details_data = Parameter["truck_permit_details"].ToObject<truck_permit_details>();
            //if (tempkey.Contains("truck_owner"))
            //    truck_Owner_data = Parameter["truck_owner"].ToObject<owner_truck_details>();
            if (tempkey.Contains("truck_driver"))
                drivertruck = Parameter["truck_driver"].ToObject<driver_truck_detail>();
            if (tempkey.Contains("truck_insurance_detail"))
                truckinsurance = Parameter["truck_insurance_detail"].ToObject<truck_insurance_detail>();

            if (truck_rto_registration_detail_data.vehicle_reg_no == null || truck_rto_registration_detail_data.vehicle_reg_no.ToString() == "")
            {
                responsecls.status = "0";
                responsecls.Message = "RTO registration cannot be empty ";
                return jser.Serialize(responsecls);
            }
            if (GetRTORegID(truck_rto_registration_detail_data.vehicle_reg_no))
            {
                responsecls.status = "0";
                responsecls.Message = "Truck with registration no " + truck_rto_registration_detail_data.vehicle_reg_no + " is already entered ";
                return jser.Serialize(responsecls);
            }

            #region truck_master
            if (truckmstdata != null)
            {
                try
                {
                    DS_Truck_Mst.truck_mstRow trukrow = objtruck.truck_mst.Newtruck_mstRow();
                    trukrow.truck_id = re.ToString();
                    trukrow.reg_date = System.DateTime.UtcNow;
                    trukrow.truck_make_id = truckmstdata.truck_make_id;
                    trukrow.truck_make_id_other = truckmstdata.truck_make_id_other;
                    trukrow.truck_model = truckmstdata.truck_model;
                    trukrow.truck_model_other = truckmstdata.truck_model_other;
                    trukrow.year_of_mfg = truckmstdata.year_of_mfg;
                    trukrow.load_capacity = truckmstdata.load_capacity;
                    trukrow.axle_detail = truckmstdata.axle_detail;
                    trukrow.axle_detail_other = truckmstdata.axle_detail_other;
                    trukrow.body_type = truckmstdata.body_type;
                    trukrow.body_type_other = truckmstdata.body_type_other;
                    trukrow.current_millage = truckmstdata.current_millage;
                    trukrow.avg_millage_per_month = truckmstdata.avg_millage_per_month;
                    trukrow.fuel_average = Convert.ToDecimal(truckmstdata.fuel_average);
                    trukrow.finance_company = truckmstdata.finance_company;
                    trukrow.active_flag = truckmstdata.active_flag;
                    trukrow.created_by = truckmstdata.created_by;
                    trukrow.created_date = System.DateTime.UtcNow;
                    trukrow.created_host = "1111";
                    trukrow.device_id = truckmstdata.device_id;
                    trukrow.device_type = truckmstdata.device_type;
                    objtruck.truck_mst.Addtruck_mstRow(trukrow);

                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message.ToString());
                    return ex.Message.ToString();
                }
            }
            #endregion

            #region truck_rto_registration_detail
            if (truck_rto_registration_detail_data != null)
            {
                try
                {
                    DS_Truck_Mst.truck_rto_registration_detailRow truckRtoRegRow = objtruck.truck_rto_registration_detail.Newtruck_rto_registration_detailRow();
                    truckRtoRegRow.reg_id = re.ToString();
                    truckRtoRegRow.truck_id = re.ToString();
                    truckRtoRegRow.vehicle_reg_no = truck_rto_registration_detail_data.vehicle_reg_no;
                    truckRtoRegRow.vehicle_reg_date = System.DateTime.UtcNow;
                    truckRtoRegRow.reg_place = truck_rto_registration_detail_data.reg_place;
                    truckRtoRegRow.active_flag = "Y";
                    truckRtoRegRow.created_by = truck_rto_registration_detail_data.created_by;
                    truckRtoRegRow.created_date = System.DateTime.UtcNow;
                    truckRtoRegRow.created_host = "1111";
                    truckRtoRegRow.device_id = truck_rto_registration_detail_data.device_id;
                    truckRtoRegRow.device_type = truck_rto_registration_detail_data.device_type;

                    objtruck.truck_rto_registration_detail.Addtruck_rto_registration_detailRow(truckRtoRegRow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log("Rto Registration : " + ex.Message.ToString());
                    return ex.Message.ToString();
                }
            }

            #endregion

            #region truck_permit_details
            if (truck_permit_details_data != null)
            {
                if (truck_permit_details_data.permit_type == "National")
                {
                    try
                    {
                        DS_Truck_Mst.truck_permit_detailsRow truckPermitRow = objtruck.truck_permit_details.Newtruck_permit_detailsRow();
                        truckPermitRow.permit_reg_id = re.ToString();
                        truckPermitRow.truck_id = re.ToString();
                        truckPermitRow.permit_no = truck_permit_details_data.permit_no;
                        truckPermitRow.permit_type = truck_permit_details_data.permit_type;
                        truckPermitRow.state_code = "ALL";
                        truckPermitRow.valid_from = Convert.ToDateTime(truck_permit_details_data.valid_from);
                        truckPermitRow.valid_upto = Convert.ToDateTime(truck_permit_details_data.valid_upto);
                        //truckPermitRow.permit_photo = "";
                        truckPermitRow.active_flag = "Y";
                        truckPermitRow.created_by = truck_permit_details_data.created_by;
                        truckPermitRow.created_date = System.DateTime.UtcNow;
                        truckPermitRow.created_host = "1111";
                        truckPermitRow.device_id = truck_permit_details_data.device_id;
                        truckPermitRow.device_type = truck_permit_details_data.device_type;

                        objtruck.truck_permit_details.Addtruck_permit_detailsRow(truckPermitRow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Permit Detials(National): " + ex.Message.ToString());
                        return ex.Message.ToString();
                    }
                }
                else if (truck_permit_details_data.permit_type == "Local")
                {
                    try
                    {
                        for (int i = 0; i < truck_permit_details_data.state_code.Length; i++)
                        {
                            int re1 = rand1.Next(1, 1000000);
                            DS_Truck_Mst.truck_permit_detailsRow truckPermitRow = objtruck.truck_permit_details.Newtruck_permit_detailsRow();

                            truckPermitRow.permit_reg_id = re1.ToString();
                            truckPermitRow.truck_id = re1.ToString();
                            truckPermitRow.permit_no = truck_permit_details_data.permit_no;
                            truckPermitRow.permit_type = truck_permit_details_data.permit_type;

                            truckPermitRow.state_code = truck_permit_details_data.state_code[i];

                            truckPermitRow.valid_from = Convert.ToDateTime(truck_permit_details_data.valid_from);
                            truckPermitRow.valid_upto = Convert.ToDateTime(truck_permit_details_data.valid_upto);
                            //truckPermitRow.permit_photo = truck_permit_details_data.permit_photo;
                            truckPermitRow.active_flag = "Y";
                            truckPermitRow.created_by = truck_permit_details_data.created_by;
                            truckPermitRow.created_date = System.DateTime.UtcNow;
                            truckPermitRow.created_host = "1111";
                            truckPermitRow.device_id = truck_permit_details_data.device_id;
                            truckPermitRow.device_type = truck_permit_details_data.device_type;

                            objtruck.truck_permit_details.Addtruck_permit_detailsRow(truckPermitRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Permit Detials(Local): " + ex.Message.ToString());
                        return ex.Message.ToString();
                    }
                }
            }
            #endregion

            #region Truck Insurance Details
            if (truckinsurance != null)
            {
                try
                {
                    DS_Truck_Mst.truck_insurance_detailRow trkrow = objtruck.truck_insurance_detail.Newtruck_insurance_detailRow();
                    trkrow.truck_id = "111";
                    trkrow.insurance_policy_no = "";
                    trkrow.insurance_details = truckinsurance.insurance_details;
                    if (truckinsurance.policy_expiry_date != "")
                        trkrow.policy_expiry_date = Convert.ToDateTime(truckinsurance.policy_expiry_date);
                    trkrow.status = truckinsurance.status;
                    trkrow.active_flag = truckinsurance.active_flag;
                    trkrow.created_by = truckinsurance.created_by;
                    trkrow.created_date = System.DateTime.UtcNow;
                    trkrow.created_host = "1111";
                    trkrow.device_id = truckinsurance.device_id;
                    trkrow.device_type = truckinsurance.device_type;

                    objtruck.truck_insurance_detail.Addtruck_insurance_detailRow(trkrow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log("Truck Insurance: " + ex.Message.ToString());
                    return ex.Message.ToString();
                }
            }

            #endregion

            #region Truck Owner Data
            //if (truck_Owner_data != null)
            //{
            //    try
            //    {
            //        //owid = truck_Owner_data.owner_id;
            //        DS_Truck_Mst.owner_truck_detailsRow owrow = objtruck.owner_truck_details.Newowner_truck_detailsRow();
            //        owrow.owner_id = truck_Owner_data.owner_id;
            //        owrow.truck_id = "111";
            //        owrow.active_flag = truckmstdata.active_flag;
            //        owrow.created_by = truckmstdata.created_by;
            //        owrow.created_date = System.DateTime.UtcNow;
            //        owrow.created_host = "1111";
            //        owrow.device_id = truckmstdata.device_id;
            //        owrow.device_type = truckmstdata.device_type;
            //        objtruck.owner_truck_details.Addowner_truck_detailsRow(owrow);
            //    }
            //    catch (Exception ex)
            //    {
            //        ServerLog.Log("Truck Owner: " + ex.Message.ToString() + ex.StackTrace.ToString());
            //        return ex.Message.ToString();
            //    }
            //}
            #endregion

            #region Truck Driver Details
            if (drivertruck != null)
            {
                try
                {

                    DS_Truck_Mst.driver_truck_detailsRow trkdrvrow = objtruck.driver_truck_details.Newdriver_truck_detailsRow();
                    trkdrvrow.driver_id = drivertruck.driver_id;
                    trkdrvrow.truck_id = "111";
                    trkdrvrow.active_flag = truckmstdata.active_flag;
                    trkdrvrow.created_by = truckmstdata.created_by;
                    trkdrvrow.created_date = System.DateTime.UtcNow;
                    trkdrvrow.created_host = "1111";
                    trkdrvrow.device_id = truckmstdata.device_id;
                    trkdrvrow.device_type = truckmstdata.device_type;
                    objtruck.driver_truck_details.Adddriver_truck_detailsRow(trkdrvrow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log("Truck Driver Details : " + ex.Message.ToString());
                    return ex.Message.ToString();
                }
            }

            #endregion

            #region Save Truck Mst Data
            objBLReturnObject = trukmst.Save_Truck_Mst(objtruck);
            if (objBLReturnObject.ExecutionStatus == 1)
            {
                //flag = 1;
                //String str = objBLReturnObject.ServerMessage.ToString();
                //return str;
                return objBLReturnObject.ServerMessage;
                #region Commented Code
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
            }
            else
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                return "{\"status\":\"0\"}";
            }
            #endregion

            //return null;
        }

        //[HttpPost]
        //public string SaveTruckPosition([FromBody]truck_current_position objtruck)
        //{
        //    if (objtruck.load_inquiry_no == null || objtruck.load_inquiry_no.ToString() == "")
        //    {
        //        return BLGeneralUtil.return_ajax_string("0", "Load inquiry no not found to save truck current position");
        //    }

        //    DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(objtruck.driver_id);
        //    DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
        //    if (dr.Length > 0)
        //        if (dr[0].ItemArray[3].ToString() != objtruck.device_id)
        //        {
        //            return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
        //        }


        //    Master master = new Master();
        //    BLReturnObject objBLReturnObject = new BLReturnObject();
        //    Document objdoc = new Document();
        //    string temploaddoc = "";
        //    DS_Truck_current_location ds_location = new DS_Truck_current_location();

        //    try
        //    {
        //        if (objtruck != null)
        //        {
        //            DBConnection.Open();
        //            DBCommand.Transaction = DBConnection.BeginTransaction();
        //            try
        //            {
        //                temploaddoc = objtruck.load_inquiry_no;
        //                DataTable dt_truck = GetTruckCurrentLocation(objtruck.truck_id, objtruck.load_inquiry_no);
        //                if (dt_truck != null && dt_truck.Rows.Count > 0)
        //                {
        //                    dt_truck.Rows[0]["active_flag"] = Constant.Flag_No;
        //                    ds_location.EnforceConstraints = false;
        //                    ds_location.truck_current_position.ImportRow(dt_truck.Rows[0]);
        //                    ds_location.truck_current_position.Rows[0].AcceptChanges();
        //                    ds_location.truck_current_position.Rows[0].SetAdded();
        //                }
        //                DS_Truck_current_location.truck_current_positionRow tr = ds_location.truck_current_position.Newtruck_current_positionRow();
        //                ds_location.EnforceConstraints = false;
        //                tr.truck_id = objtruck.truck_id;
        //                tr.driver_id = objtruck.driver_id;
        //                tr.log_date = System.DateTime.UtcNow;
        //                tr.load_inquiry_no = objtruck.load_inquiry_no;
        //                tr.truck_lat = objtruck.truck_lat;
        //                tr.truck_lng = objtruck.truck_lng;
        //                tr.truck_location = objtruck.truck_location;
        //                tr.remaining_kms = objtruck.remaining_kms;
        //                tr.eta = objtruck.eta;
        //                tr.active_flag = Constant.Flag_Yes;
        //                tr.created_by = objtruck.created_by;
        //                tr.created_date = System.DateTime.UtcNow;
        //                tr.created_host = objtruck.created_host;
        //                tr.device_id = objtruck.device_id;
        //                tr.device_type = objtruck.device_type;
        //                ds_location.EnforceConstraints = false;
        //                ds_location.truck_current_position.Addtruck_current_positionRow(tr);
        //                ds_location.truck_current_position.Rows[0].AcceptChanges();
        //                ds_location.truck_current_position.Rows[0].SetAdded();
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.Log(ex.Message + ex.StackTrace);
        //                return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //            }
        //            objBLReturnObject = master.UpdateTables(ds_location.truck_current_position, ref DBCommand);
        //            if (objBLReturnObject.ExecutionStatus == 2)
        //            {
        //                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
        //                DBCommand.Transaction.Rollback();
        //                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //                objBLReturnObject.ExecutionStatus = 2;
        //                return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
        //            }

        //            DBCommand.Transaction.Commit();
        //            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //            objBLReturnObject.ExecutionStatus = 1;
        //            ServerLog.SuccessLog("Truck Current Position Saved : " + temploaddoc);
        //            return BLGeneralUtil.return_ajax_string("1", objBLReturnObject.ServerMessage);
        //        }
        //        else
        //        {
        //            return BLGeneralUtil.return_ajax_string("0", "No Data To Save ");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
        //        return BLGeneralUtil.return_ajax_string("0", ex.Message);
        //    }
        //}

        public string SaveTruckPosition([FromBody]truck_current_position objtruck)
        {


            Master master = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            Document objdoc = new Document(); string skms = "";
            string temploaddoc = ""; decimal totalkms = 0; decimal currentkms = 0;
            DS_Truck_current_location ds_location = new DS_Truck_current_location(); decimal rkms = 0; decimal rkms_old = 0; decimal tkms = 0;


            ServerLog.SuccessLog("SaveTruckPosition(" + JsonConvert.SerializeObject(objtruck) + ")");

            #region Check valid Driver device register


            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(objtruck.driver_id);
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != objtruck.device_id)
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }

            #endregion

            if (objtruck.truck_lat == null || objtruck.truck_lat == "")
                return BLGeneralUtil.return_ajax_string("0", " Latitude is not found");

            if (objtruck.truck_lng == null || objtruck.truck_lng == "")
                return BLGeneralUtil.return_ajax_string("0", "Inquiry source Longitude is not found");

            if (objtruck.truck_lat.Trim() != "")
                if (Math.Round(Convert.ToDecimal(objtruck.truck_lat)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "latlong is Zero");

            if (objtruck.truck_lng.Trim() != "")
                if (Math.Round(Convert.ToDecimal(objtruck.truck_lng)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "latlong is Zero");

            if (objtruck.created_by == null || objtruck.created_by == "")
                return BLGeneralUtil.return_ajax_string("0", "Created By value not found");

            if (objtruck.truck_id == null || objtruck.truck_id == "")
                return BLGeneralUtil.return_ajax_string("0", "truck Id not found");

            #region extract kms (decimal part) from post

            if (objtruck.remaining_kms != null && objtruck.remaining_kms.ToString() != "")
                skms = objtruck.remaining_kms;

            if (skms != "")
            {
                if (skms.IndexOf(" ") != 0)
                {
                    if (skms.Substring(skms.IndexOf(" "), (skms.Length - skms.IndexOf(" "))).Trim() == "m")
                    {
                        skms = skms.Substring(0, skms.IndexOf(" "));
                        rkms = Convert.ToDecimal(skms);
                        rkms = rkms / 1000;
                        rkms = Math.Round(rkms, 2);
                    }
                    else
                    {
                        skms = skms.Substring(0, skms.IndexOf(" "));
                        rkms = Convert.ToDecimal(skms);
                    }
                }
            }
            #endregion

            try
            {
                if (objtruck != null)
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();
                    try
                    {
                        temploaddoc = objtruck.load_inquiry_no;
                        //DataTable dt_truck = GetTruckByID(objtruck.truck_id, objtruck.load_inquiry_no);
                        DataTable dt_truck = GetTruckByID(objtruck.truck_id, "-1");
                        rkms_old = 0;
                        if (dt_truck != null && dt_truck.Rows.Count > 0)
                        {
                            ds_location.EnforceConstraints = false;
                            for (int i = 0; i < dt_truck.Rows.Count; i++)
                            {
                                dt_truck.Rows[i]["active_flag"] = Constant.Flag_No;
                                if (dt_truck.Rows[i]["remaining_kms"] == DBNull.Value || dt_truck.Rows[i]["remaining_kms"].ToString() == "")
                                    rkms_old = 0;
                                else
                                {
                                    skms = dt_truck.Rows[i]["remaining_kms"].ToString();
                                    if (skms.Substring(skms.IndexOf(" "), (skms.Length - skms.IndexOf(" "))).Trim() == "m")
                                    {
                                        skms = skms.Substring(0, skms.IndexOf(" "));
                                        rkms_old = Convert.ToDecimal(skms);
                                        rkms_old = rkms_old / 1000;
                                        rkms_old = Math.Round(rkms, 2);
                                    }
                                    else
                                    {
                                        skms = skms.Substring(0, skms.IndexOf(" "));
                                        rkms_old = Convert.ToDecimal(skms);
                                    }
                                }

                                if (dt_truck.Rows[i]["total_kms"] == DBNull.Value || dt_truck.Rows[i]["total_kms"].ToString() == "")
                                    tkms = 0;
                                else
                                    tkms = Convert.ToDecimal(dt_truck.Rows[i]["total_kms"]);

                                ds_location.truck_current_position.ImportRow(dt_truck.Rows[i]);
                                ds_location.truck_current_position.Rows[i].AcceptChanges();
                                if (objtruck.load_inquiry_no == null || objtruck.load_inquiry_no.ToString() == "")
                                    ds_location.truck_current_position.Rows[i].Delete();
                                else
                                    ds_location.truck_current_position.Rows[i].SetAdded();
                            }
                        }
                        if (rkms == 0)
                        {
                            if (rkms_old == 0)
                                rkms = 0;
                            else
                                rkms = rkms_old;
                        }

                        if (rkms_old == 0)
                            currentkms = 0;
                        else
                            currentkms = (rkms_old - rkms);

                        if (currentkms < 0) currentkms = currentkms * -1;

                        #region truck_position_Data
                        DS_Truck_current_location.truck_current_positionRow tr = ds_location.truck_current_position.Newtruck_current_positionRow();
                        ds_location.EnforceConstraints = false;
                        tr.truck_id = objtruck.truck_id;
                        tr.driver_id = objtruck.driver_id;
                        tr.log_date = System.DateTime.UtcNow;
                        tr.load_inquiry_no = objtruck.load_inquiry_no;
                        tr.truck_lat = objtruck.truck_lat;
                        tr.truck_lng = objtruck.truck_lng;
                        tr.truck_location = objtruck.truck_location;
                        tr.remaining_kms = objtruck.remaining_kms;
                        tr.eta = objtruck.eta;
                        tr.current_kms = currentkms;
                        tr.total_kms = tkms + currentkms;
                        tr.active_flag = Constant.Flag_Yes;
                        tr.created_by = objtruck.created_by;
                        tr.created_date = System.DateTime.UtcNow;
                        tr.created_host = objtruck.created_host;
                        tr.device_id = objtruck.device_id;
                        tr.device_type = objtruck.device_type;
                        ds_location.EnforceConstraints = false;
                        ds_location.truck_current_position.Addtruck_current_positionRow(tr);
                        int indx = ds_location.truck_current_position.Rows.Count - 1;
                        ds_location.truck_current_position.Rows[indx].AcceptChanges();
                        ds_location.truck_current_position.Rows[indx].SetAdded();
                        #endregion


                        objBLReturnObject = master.UpdateTables(ds_location.truck_current_position, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log("SaveTruckPosition(" + objBLReturnObject.ServerMessage.ToString() + ")");
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("SaveTruckPosition(" + ex.Message + Environment.NewLine + ex.StackTrace + ")");
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("SaveTruckPosition( Truck Current Position Saved : " + JsonConvert.SerializeObject(objtruck) + ")");
                    return BLGeneralUtil.return_ajax_string("1", objBLReturnObject.ServerMessage);
                }
                else
                {
                    ServerLog.SuccessLog("SaveTruckPosition( No Data To Save For: " + temploaddoc + ")");
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    return BLGeneralUtil.return_ajax_string("0", "No Data To Save ");
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("SaveTruckPosition(" + ex.Message + Environment.NewLine + ex.StackTrace + ")");
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }


        public bool GetRTORegID(string rtoid)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT truck_id FROM truck_rto_registration_detail Where vehicle_reg_no = @rtid ");

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@rtid";
            pr1.Value = rtoid;


            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            cmd.Parameters.Add(pr1);

            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtOwner);

            if (dtOwner != null && dtOwner.Rows.Count > 0)
                return true;
            else
                return false;
        }

        #endregion


        public DataTable GetTruckByID(string truckid, string loadinq)
        {
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            if (loadinq == "-1")
                query1 = "Select * FROM truck_current_position where truck_id = @trkid and active_flag= 'Y' ";
            else
                query1 = "Select * FROM truck_current_position where truck_id = @trkid and load_inquiry_no = @loadno and active_flag= 'Y' ";


            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("trkid", DbType.String, truckid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, loadinq));

            // if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtPostLoadOrders = ds.Tables[0];
            }
            //DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                //return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
                return dtPostLoadOrders;
            else
                return null;
            //return BLGeneralUtil.return_ajax_string("0", "No Order found ");
        }

        public DataTable GetTruckMasterById(string TrukId)
        {
            String query = "SELECT * FROM truck_mst Where truck_id = '" + TrukId + "' and active_flag='Y' ";
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

        [HttpPost]
        public string SaveTruck([FromBody] JObject Jobj)
        {

            DS_Truck_Mst ds_truck = new DS_Truck_Mst();
            Document objdoc = new Document(); Master master = new Master();
            List<truck> Truck = new List<truck>(); string msg = "";
            string TrukID = ""; string TrukRTOID = ""; string DocPerId = "";
            DataSet dstruck = new DataSet(); DataSet dsRto = new DataSet(); DataSet dsPermit = new DataSet();
            DataSet dsInsurance = new DataSet(); DataSet dsmaintenance = new DataSet(); DataSet dsOwner = new DataSet();
            DataTable dt_truck = new DataTable();
            DataTable dttrucktemp = new DataTable();
            DataTable dt_permit = new DataTable();
            //DataTable dt_insurance = new DataTable();
            //DataTable dt_maintenance = new DataTable();
            if (Jobj["Truck"] != null)
            {
                Truck = Jobj["Truck"].ToObject<List<truck>>();
                DataSet ds = master.CreateDataSet(Jobj["Truck"].ToObject<List<truck>>());
                dttrucktemp = BLGeneralUtil.CheckDateTime(ds.Tables[0]);
            }

            List<truck_rto_registration_detail> TRto = new List<truck_rto_registration_detail>();
            if (Jobj["RTO"] != null)
            {
                TRto = Jobj["RTO"].ToObject<List<truck_rto_registration_detail>>();
                dsRto = master.CreateDataSet(Jobj["RTO"].ToObject<List<truck_rto_registration_detail>>());
            }

            List<truck_permit_details> TPermit = new List<truck_permit_details>();
            if (Jobj["Permit"] != null)
            {
                TPermit = Jobj["Permit"].ToObject<List<truck_permit_details>>();
                dsPermit = master.CreateDataSet(Jobj["Permit"].ToObject<List<truck_permit_details>>());
                dt_permit = BLGeneralUtil.CheckDateTime(dsPermit.Tables[0]);
            }

            List<truck_insurance_detail> TIns = new List<truck_insurance_detail>();
            if (Jobj["Insurance"] != null)
            {
                TIns = Jobj["Insurance"].ToObject<List<truck_insurance_detail>>();
                dsInsurance = master.CreateDataSet(Jobj["Insurance"].ToObject<List<truck_insurance_detail>>());
                //if (dsInsurance != null)
                //    dt_insurance = BLGeneralUtil.CheckDateTime(dsInsurance.Tables[0]);
            }

            List<truck_maintenance_detail> TMain = new List<truck_maintenance_detail>();
            if (Jobj["Maintenance"] != null)
            {
                TMain = Jobj["Maintenance"].ToObject<List<truck_maintenance_detail>>();
                dsmaintenance = master.CreateDataSet(Jobj["Maintenance"].ToObject<List<truck_maintenance_detail>>());
                //if (dsmaintenance != null)
                //    dt_maintenance = BLGeneralUtil.CheckDateTime(dsmaintenance.Tables[0]);
            }


            if (TRto[0].vehicle_reg_no == null || TRto[0].vehicle_reg_no.ToString() == "")
            {
                return BLGeneralUtil.return_ajax_string("0", "RTO registration cannot be empty ");
            }

            if (Truck[0].truck_id != null && Truck[0].truck_id.Trim() != "")
            {
                TrukID = Truck[0].truck_id.ToString();
                dt_truck = GetTruckMasterById(TrukID);
            }
            else
            {
                TrukID = "";
                if (GetRTORegID(TRto[0].vehicle_reg_no.ToUpper()))
                {
                    return BLGeneralUtil.return_ajax_string("0", "Truck with registration no " + TRto[0].vehicle_reg_no + " is already entered ");
                }
            }


            // DataTable dtOwnerDriver = new DataTable();
            DataTable dtdrivertruck = new DataTable();
            DataTable dt_userdetail = new DataTable();

            DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();
            ds_truck.EnforceConstraints = false;

            #region truck_master
            if (dttrucktemp != null && Truck.Count > 0)
            {
                try
                {
                    ds_truck.EnforceConstraints = false;
                    if (TrukID == "")
                    {
                        if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "TK", "", "", ref TrukID, ref msg))
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            BLGeneralUtil.return_ajax_string("0", msg);
                        }
                    }
                    dttrucktemp.Rows[0]["reg_date"] = System.DateTime.UtcNow;
                    if (dttrucktemp.Rows[0]["fuel_average"].ToString() == "")
                        dttrucktemp.Rows[0]["fuel_average"] = 0;
                    //DS_Truck_Mst.truck_mstDataTable dstrk = Jobj["Truck"].ToObject<DS_Truck_Mst.truck_mstDataTable>();
                    ds_truck.truck_mst.ImportRow(dttrucktemp.Rows[0]);
                    if (dt_truck != null && dt_truck.Rows.Count > 0)
                    {
                        if (dt_truck.Rows[0]["reg_date"] == DBNull.Value && dt_truck.Rows[0]["reg_date"].ToString() == "")
                            ds_truck.truck_mst[0].reg_date = System.DateTime.UtcNow;
                        else
                            ds_truck.truck_mst[0].reg_date = Convert.ToDateTime(dt_truck.Rows[0]["reg_date"]);
                    }

                    ds_truck.truck_mst[0].truck_id = TrukID;
                    ds_truck.truck_mst[0].active_flag = Constant.Flag_Yes;
                    ds_truck.truck_mst[0].created_date = System.DateTime.UtcNow;
                    ds_truck.truck_mst[0].AcceptChanges();
                    ds_truck.truck_mst[0].SetAdded();
                    ds_truck.EnforceConstraints = true;
                }
                catch (Exception ex)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    ServerLog.Log(ex.Message.ToString());
                    return BLGeneralUtil.return_ajax_string("0", ex.Message.ToString());
                }
            }
            #endregion

            #region truck_rto_registration_detail
            if (Jobj["RTO"] != null)
            {
                if (TRto != null && TRto.Count > 0)
                {
                    try
                    {
                        DataTable dtrto = new DataTable();
                        dtrto = GetTruckRTOById(TrukID);
                        msg = "";
                        if (dtrto == null)
                        {
                            if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "TR", "", "", ref TrukRTOID, ref msg))
                            {
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                return BLGeneralUtil.return_ajax_string("0", msg);
                            }
                        }
                        else
                            TrukRTOID = dtrto.Rows[0]["reg_id"].ToString();
                        //DS_Truck_Mst.truck_rto_registration_detailDataTable dsrto = Jobj["RTO"].ToObject<DS_Truck_Mst.truck_rto_registration_detailDataTable>();
                        ds_truck.EnforceConstraints = false;
                        ds_truck.truck_rto_registration_detail.ImportRow(dsRto.Tables[0].Rows[0]);
                        ds_truck.truck_rto_registration_detail[0].vehicle_reg_date = System.DateTime.UtcNow;
                        ds_truck.truck_rto_registration_detail[0].reg_id = TrukRTOID;
                        ds_truck.truck_rto_registration_detail[0].truck_id = TrukID;
                        ds_truck.truck_rto_registration_detail[0].active_flag = Constant.Flag_Yes;
                        ds_truck.truck_rto_registration_detail[0].created_date = System.DateTime.UtcNow;
                        ds_truck.truck_rto_registration_detail[0].AcceptChanges();
                        ds_truck.truck_rto_registration_detail[0].SetAdded();
                        ds_truck.EnforceConstraints = true;
                    }
                    catch (Exception ex)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        ServerLog.Log("Rto Registration : " + ex.Message.ToString());
                        return BLGeneralUtil.return_ajax_string("0", "Rto Registration : " + ex.Message.ToString());
                    }
                }
            }
            #endregion

            #region truck_permit_details
            if (Jobj["Permit"] != null)
            {
                if (TPermit != null && TPermit.Count > 0)
                {

                    if (TPermit[0].permit_type == "National")
                    {
                        try
                        {
                            msg = "";
                            DataTable dtpermit = new DataTable();
                            dtpermit = GetTruckPermitById(TrukID);
                            if (dtpermit == null)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "TPD", "", "", ref DocPerId, ref msg))
                                {
                                    DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", msg);
                                }
                            }
                            else
                            {
                                DocPerId = dtpermit.Rows[0]["permit_reg_id"].ToString();
                            }
                            //DS_Truck_Mst.truck_permit_detailsDataTable dsprmt = Jobj["Permit"].ToObject<DS_Truck_Mst.truck_permit_detailsDataTable>();
                            ds_truck.EnforceConstraints = false;
                            ds_truck.truck_permit_details.ImportRow(dt_permit.Rows[0]);
                            ds_truck.truck_permit_details[0].permit_reg_id = DocPerId;
                            ds_truck.truck_permit_details[0].truck_id = TrukID;
                            ds_truck.truck_permit_details[0].state_code = "ALL";
                            ds_truck.truck_permit_details[0].valid_from = BLGeneralUtil.ConvertToDateTime(TPermit[0].valid_from.ToString(), "dd/mm/yyyy");
                            ds_truck.truck_permit_details[0].valid_upto = BLGeneralUtil.ConvertToDateTime(TPermit[0].valid_upto.ToString(), "dd/mm/yyyy");
                            ds_truck.truck_permit_details[0].active_flag = Constant.Flag_Yes;
                            ds_truck.truck_permit_details[0].created_date = System.DateTime.UtcNow;
                            ds_truck.EnforceConstraints = true;
                        }
                        catch (Exception ex)
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            ServerLog.Log("Permit Details(National): " + ex.Message.ToString());
                            return BLGeneralUtil.return_ajax_string("0", "Permit Details(National): " + ex.Message.ToString());
                        }
                    }
                    else if (TPermit[0].permit_type == "Local")
                    {
                        try
                        {
                            msg = "";
                            DataTable dtpermit = new DataTable();
                            dtpermit = GetTruckPermitById(TrukID);
                            if (dtpermit == null)
                            {
                                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "TPD", "", "", ref DocPerId, ref msg))
                                {
                                    DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", msg);
                                }
                            }
                            else
                            {
                                DocPerId = dtpermit.Rows[0]["permit_reg_id"].ToString();
                            }
                            //for (int i = 0; i < TPermit[0].state_code.Length; i++)
                            //{
                            //DS_Truck_Mst.truck_permit_detailsDataTable dsprmt = Jobj["Permit"].ToObject<DS_Truck_Mst.truck_permit_detailsDataTable>();
                            ds_truck.EnforceConstraints = false;
                            ds_truck.truck_permit_details.ImportRow(dt_permit.Rows[0]);
                            //  int indx = ds_truck.truck_permit_details.Rows.IndexOf(dt_permit.Rows[0]);
                            ds_truck.truck_permit_details[0].permit_reg_id = DocPerId;
                            ds_truck.truck_permit_details[0].truck_id = TrukID;
                            ds_truck.truck_permit_details[0].state_code = "ALL";
                            ds_truck.truck_permit_details[0].valid_from = BLGeneralUtil.ConvertToDateTime(TPermit[0].valid_from.ToString(), "dd/mm/yyyy");
                            ds_truck.truck_permit_details[0].valid_upto = BLGeneralUtil.ConvertToDateTime(TPermit[0].valid_upto.ToString(), "dd/mm/yyyy");
                            ds_truck.truck_permit_details[0].active_flag = Constant.Flag_Yes;
                            ds_truck.truck_permit_details[0].state_code = "";
                            ds_truck.truck_permit_details[0].created_date = System.DateTime.UtcNow;
                            ds_truck.EnforceConstraints = true;
                            //    }
                        }
                        catch (Exception ex)
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            ServerLog.Log("Permit Details(Local): " + ex.Message.ToString());
                            return BLGeneralUtil.return_ajax_string("0", "Permit Details(Local): " + ex.Message.ToString());
                        }
                    }
                }
            }
            #endregion

            #region Truck Insurance Details
            if (dsInsurance != null)
            {
                if (TIns[0].insurance_policy_no != "" && TIns.Count > 0)
                {
                    for (int i = 0; i < TIns.Count; i++)
                    {
                        DataTable dtins = new DataTable();
                        dtins = trukmst.GetInsuranceDataNew(TrukID, TIns[i].insurance_sr_id);


                        //for (int j = 0; j < dsInsurance.Tables[0].Rows.Count; j++)
                        //{
                        if (dsInsurance.Tables[0].Rows[i]["policy_issue_date"].ToString() == "")
                            dsInsurance.Tables[0].Rows[i]["policy_issue_date"] = DBNull.Value;
                        else if (dsInsurance.Tables[0].Rows[i]["policy_issue_date"].ToString() != "")
                            dsInsurance.Tables[0].Rows[i]["policy_issue_date"] = BLGeneralUtil.ConvertToDateTime(dsInsurance.Tables[0].Rows[i]["policy_issue_date"].ToString(), "dd/mm/yyyy");

                        if (dsInsurance.Tables[0].Rows[i]["policy_expiry_date"].ToString() == "")
                            dsInsurance.Tables[0].Rows[i]["policy_expiry_date"] = DBNull.Value;
                        else if (dsInsurance.Tables[0].Rows[i]["policy_expiry_date"].ToString() != "")
                            dsInsurance.Tables[0].Rows[i]["policy_expiry_date"] = BLGeneralUtil.ConvertToDateTime(dsInsurance.Tables[0].Rows[i]["policy_expiry_date"].ToString(), "dd/mm/yyyy");

                        //}

                        try
                        {
                            ds_truck.EnforceConstraints = false;
                            ds_truck.truck_insurance_detail.ImportRow(dsInsurance.Tables[0].Rows[i]);

                            if (TIns[i].insurance_sr_id == "")
                            {
                                string DocId = "";
                                if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "TID", "", "", ref DocId, ref msg))
                                {
                                    DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", msg);
                                }
                                ds_truck.truck_insurance_detail[i].insurance_sr_id = DocId;
                            }
                            else
                                ds_truck.truck_insurance_detail[i].insurance_sr_id = TIns[i].insurance_sr_id;

                            //DS_Truck_Mst.truck_insurance_detailDataTable Tins = Jobj["Insurance"].ToObject<DS_Truck_Mst.truck_insurance_detailDataTable>();
                            //if (TIns[i].policy_issue_date != "")
                            //    ds_truck.truck_insurance_detail[i].policy_issue_date = BLGeneralUtil.ConvertToDateTime(TIns[i].policy_issue_date, "dd/mm/yyyy");
                            //if (TIns[i].policy_expiry_date != "")
                            //    ds_truck.truck_insurance_detail[i].policy_expiry_date = BLGeneralUtil.ConvertToDateTime(TIns[i].policy_expiry_date, "dd/mm/yyyy");

                            ds_truck.truck_insurance_detail[i].truck_id = TrukID;
                            ds_truck.truck_insurance_detail[i].active_flag = Constant.Flag_Yes;
                            ds_truck.truck_insurance_detail[i].created_date = System.DateTime.UtcNow;
                            ds_truck.EnforceConstraints = false;
                        }
                        catch (Exception ex)
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            ServerLog.Log("Truck Insurance: " + ex.Message.ToString());
                            return ex.Message.ToString();
                        }
                    }
                }
            }
            #endregion


            #region Truck Maintenance Details
            if (dsmaintenance != null)
            {
                if (TMain.Count > 0)
                {
                    for (int i = 0; i < dsmaintenance.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            DataTable dtMaint = new DataTable();
                            dtMaint = trukmst.GetMaintenanceDataNew(TrukID, TMain[i].maintenance_id);

                            //for (int j = 0; j < dsmaintenance.Tables[0].Rows.Count; j++)
                            //{
                            if (dsmaintenance.Tables[0].Rows[i]["maintenance_date"].ToString() == "")
                                dsmaintenance.Tables[0].Rows[i]["maintenance_date"] = DBNull.Value;
                            else if (dsmaintenance.Tables[0].Rows[i]["maintenance_date"].ToString() != "")
                                dsmaintenance.Tables[0].Rows[i]["maintenance_date"] = BLGeneralUtil.ConvertToDateTime(dsmaintenance.Tables[0].Rows[i]["maintenance_date"].ToString(), "dd/mm/yyyy");
                            //}

                            ds_truck.EnforceConstraints = false;
                            ds_truck.truck_maintenance_detail.ImportRow(dsmaintenance.Tables[0].Rows[i]);

                            if (TMain[i].maintenance_id == "")
                            {

                                string DocId = "";
                                if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "TMD", "", "", ref DocId, ref msg))
                                {
                                    DBCommand.Transaction.Rollback();
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return BLGeneralUtil.return_ajax_string("0", msg);
                                }
                                ds_truck.truck_maintenance_detail[i].maintenance_id = DocId;
                            }
                            else
                                ds_truck.truck_maintenance_detail[i].maintenance_id = TMain[i].maintenance_id;

                            //DS_Truck_Mst.truck_insurance_detailDataTable Tins = Jobj["Insurance"].ToObject<DS_Truck_Mst.truck_insurance_detailDataTable>();


                            ds_truck.truck_maintenance_detail[i].truck_id = TrukID;
                            //if (TMain[i].maintenance_date != "")
                            //    ds_truck.truck_maintenance_detail[i].maintenance_date = BLGeneralUtil.ConvertToDateTime(TMain[i].maintenance_date, "dd/mm/yyyy");

                            ds_truck.truck_maintenance_detail[i].active_flag = Constant.Flag_Yes;
                            ds_truck.truck_maintenance_detail[i].created_date = System.DateTime.UtcNow;
                            ds_truck.EnforceConstraints = false;
                        }
                        catch (Exception ex)
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            ServerLog.Log("Truck Insurance: " + ex.Message.ToString());
                            return ex.Message.ToString();
                        }
                    }
                }
            }
            #endregion


            if (ds_truck.truck_mst != null && ds_truck.truck_mst.Rows.Count > 0)
            {
                objBLReturnObject = master.UpdateTables(ds_truck.truck_mst, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }

            if (ds_truck.truck_rto_registration_detail != null && ds_truck.truck_rto_registration_detail.Rows.Count > 0)
            {
                objBLReturnObject = master.UpdateTables(ds_truck.truck_rto_registration_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }

            if (ds_truck.truck_permit_details != null && ds_truck.truck_permit_details.Rows.Count > 0)
            {
                objBLReturnObject = master.UpdateTables(ds_truck.truck_permit_details, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }

            if (ds_truck.truck_insurance_detail != null && ds_truck.truck_insurance_detail.Rows.Count > 0)
            {
                objBLReturnObject = master.UpdateTables(ds_truck.truck_insurance_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }

            if (ds_truck.truck_maintenance_detail != null && ds_truck.truck_maintenance_detail.Rows.Count > 0)
            {
                objBLReturnObject = master.UpdateTables(ds_truck.truck_maintenance_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }



            DBCommand.Transaction.Commit();
            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            objBLReturnObject.ExecutionStatus = 1;
            ServerLog.SuccessLog("Truck Master Saved : " + TrukID);
            return BLGeneralUtil.return_ajax_statusdata("1", "Truck Details Added ", SendReceiveJSon.GetJson(ds_truck.truck_mst));

        }

        [HttpPost]
        public HttpResponseMessage PostFile()
        {
            HttpResponseMessage result = null;
            try
            {

                var httpRequest = HttpContext.Current.Request;
                Dictionary<string, string> array1 = new Dictionary<string, string>();

                int intcunt = httpRequest.Files.Count;
                ServerLog.Log(httpRequest["truck_id"].ToString() + "_" + intcunt.ToString());

                if (httpRequest.Files.Count > 0)
                {
                    int randNum = 0;
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        ServerLog.Log(postedFile.FileName);

                        string Fname = postedFile.FileName.Replace(postedFile.FileName, httpRequest["truck_id"].ToString() + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + randNum.ToString() + Path.GetExtension(postedFile.FileName));
                        ServerLog.Log(Fname);
                        var filePath = HttpContext.Current.Server.MapPath("~/Images/truckRegPhoto/" + Fname);
                        postedFile.SaveAs(filePath);

                        docfiles.Add(filePath);
                        string filename = httpRequest["truck_id"].ToString() + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + randNum.ToString() + Path.GetExtension(postedFile.FileName);
                        array1[file] = "/Images/truckRegPhoto/" + filename;
                        randNum++;
                    }
                    //ServerLog.Log(httpRequest["truck_id"].ToString() + "____" + array1["reg_doc_copy"] + "____" + array1["vehicle_regno_copy"] + "____" + array1["permit_photo"]);
                    //  String str = update_rto_detail_Table(httpRequest["truck_id"].ToString(), array1["reg_doc_copy"], array1["vehicle_regno_copy"], array1["permit_photo"]);

                    String str = update_rto_detail_Table(httpRequest["truck_id"], array1);
                    //result = Request.CreateResponse(HttpStatusCode.Created, httpRequest["truck_id"] + "_" + str);
                    result = Request.CreateResponse(HttpStatusCode.Created, str);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("Image Upload " + ex.Message.ToString());
                //ServerLog.Log(ex.InnerException.ToString());
                return null;

            }
            return result;
        }

        public string update_rto_detail_Table(string truck_id, Dictionary<string, string> data)
        {
            Dictionary<string, string> filePath = new Dictionary<string, string>();
            foreach (var pair in data)
            {
                //var key = pair.Key;
                //var value = pair.Value;
                filePath[pair.Key] = pair.Value;

                // ServerLog.Log(pair.Key + "___" + pair.Value);
            }



            string truck1 = "";
            if (truck_id != null && truck_id != string.Empty)
            {
                truck1 = truck_id.Replace('"', ' ').Trim();
                truck1 = truck1.TrimEnd().TrimStart();
            }



            int flag = 0;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            DataTable dt_truck_rto_detail = trukmst.GetRtoDetailTableData(truck1);

            try
            {

                #region truck_rto_registration_detail

                DS_Truck_Mst.truck_rto_registration_detailRow truckRtoRegRow = objtruck.truck_rto_registration_detail.Newtruck_rto_registration_detailRow();

                truckRtoRegRow.reg_id = dt_truck_rto_detail.Rows[0]["reg_id"].ToString();
                truckRtoRegRow.truck_id = dt_truck_rto_detail.Rows[0]["truck_id"].ToString();
                truckRtoRegRow.vehicle_reg_no = dt_truck_rto_detail.Rows[0]["vehicle_reg_no"].ToString();
                truckRtoRegRow.vehicle_reg_date = Convert.ToDateTime(dt_truck_rto_detail.Rows[0]["vehicle_reg_date"].ToString());
                truckRtoRegRow.reg_place = dt_truck_rto_detail.Rows[0]["reg_place"].ToString();
                truckRtoRegRow.reg_doc_copy = dt_truck_rto_detail.Rows[0]["reg_doc_copy"].ToString();
                truckRtoRegRow.vehicle_regno_copy = dt_truck_rto_detail.Rows[0]["vehicle_regno_copy"].ToString();
                truckRtoRegRow.vehice_photo = dt_truck_rto_detail.Rows[0]["vehice_photo"].ToString();

                if (filePath.Keys.Contains("reg_doc_copy"))
                    truckRtoRegRow.reg_doc_copy = filePath["reg_doc_copy"];

                if (filePath.Keys.Contains("vehicle_regno_copy"))
                    truckRtoRegRow.vehicle_regno_copy = filePath["vehicle_regno_copy"];

                if (filePath.Keys.Contains("vehice_photo"))
                    truckRtoRegRow.vehice_photo = filePath["vehice_photo"];

                truckRtoRegRow.active_flag = dt_truck_rto_detail.Rows[0]["active_flag"].ToString();
                truckRtoRegRow.created_by = dt_truck_rto_detail.Rows[0]["created_by"].ToString();
                truckRtoRegRow.created_date = System.DateTime.UtcNow;
                truckRtoRegRow.created_host = dt_truck_rto_detail.Rows[0]["created_host"].ToString();
                truckRtoRegRow.device_id = dt_truck_rto_detail.Rows[0]["device_id"].ToString();
                truckRtoRegRow.device_type = dt_truck_rto_detail.Rows[0]["device_type"].ToString();

                truckRtoRegRow.modified_by = dt_truck_rto_detail.Rows[0]["created_by"].ToString();
                truckRtoRegRow.modified_date = System.DateTime.UtcNow;
                truckRtoRegRow.modified_host = dt_truck_rto_detail.Rows[0]["created_host"].ToString();
                truckRtoRegRow.modified_device_id = dt_truck_rto_detail.Rows[0]["device_id"].ToString();
                truckRtoRegRow.modified_device_type = dt_truck_rto_detail.Rows[0]["device_type"].ToString();

                objtruck.truck_rto_registration_detail.Addtruck_rto_registration_detailRow(truckRtoRegRow);
                #endregion

                #region truck_permit_details
                DataTable dtPermitData = trukmst.GetPermitData(truck1);
                if (dtPermitData != null)
                {
                    for (int i = 0; i < dtPermitData.Rows.Count; i++)
                    {
                        DS_Truck_Mst.truck_permit_detailsRow truckPermitRow = objtruck.truck_permit_details.Newtruck_permit_detailsRow();

                        truckPermitRow.permit_reg_id = dtPermitData.Rows[i]["permit_reg_id"].ToString();
                        truckPermitRow.truck_id = dtPermitData.Rows[i]["truck_id"].ToString();
                        truckPermitRow.permit_no = dtPermitData.Rows[i]["permit_no"].ToString();
                        truckPermitRow.permit_type = dtPermitData.Rows[i]["permit_type"].ToString();
                        truckPermitRow.state_code = dtPermitData.Rows[i]["state_code"].ToString();
                        truckPermitRow.valid_from = Convert.ToDateTime(dtPermitData.Rows[i]["valid_from"].ToString());
                        truckPermitRow.valid_upto = Convert.ToDateTime(dtPermitData.Rows[i]["valid_upto"].ToString());
                        truckPermitRow.permit_photo = dtPermitData.Rows[i]["permit_photo"].ToString();

                        if (filePath.Keys.Contains("permit_photo"))
                            truckPermitRow.permit_photo = filePath["permit_photo"];
                        truckPermitRow.active_flag = dtPermitData.Rows[i]["active_flag"].ToString();
                        truckPermitRow.created_by = dtPermitData.Rows[i]["created_by"].ToString();
                        truckPermitRow.created_date = DateTime.UtcNow;
                        truckPermitRow.created_host = dtPermitData.Rows[i]["created_host"].ToString();
                        truckPermitRow.device_id = dtPermitData.Rows[i]["device_id"].ToString();
                        truckPermitRow.device_type = dtPermitData.Rows[i]["device_type"].ToString();

                        truckPermitRow.modified_by = dtPermitData.Rows[i]["created_by"].ToString();
                        truckPermitRow.modified_date = System.DateTime.UtcNow;
                        truckPermitRow.modified_host = dtPermitData.Rows[i]["created_host"].ToString();
                        truckPermitRow.modified_device_id = dtPermitData.Rows[i]["device_id"].ToString();
                        truckPermitRow.modified_device_type = dtPermitData.Rows[i]["device_type"].ToString();


                        objtruck.truck_permit_details.Addtruck_permit_detailsRow(truckPermitRow);
                    }
                }
                #endregion

                #region Truck Insurance Details

                int Inscnt = 1;

                DataTable dtinsurnace = trukmst.GetInsuranceData(truck1);
                if (dtinsurnace != null)
                {
                    for (int i = 0; i < dtinsurnace.Rows.Count; i++)
                    {
                        //if (filePath.ContainsKey("policy_photo_path" + Inscnt))
                        //{
                        DS_Truck_Mst.truck_insurance_detailRow truckInsRow = objtruck.truck_insurance_detail.Newtruck_insurance_detailRow();

                        truckInsRow.insurance_sr_id = dtinsurnace.Rows[i]["insurance_sr_id"].ToString();
                        truckInsRow.truck_id = dtinsurnace.Rows[i]["truck_id"].ToString();
                        truckInsRow.insurance_policy_no = dtinsurnace.Rows[i]["insurance_policy_no"].ToString();
                        truckInsRow.insurance_details = dtinsurnace.Rows[i]["insurance_details"].ToString();
                        truckInsRow.policy_photo_path = dtinsurnace.Rows[i]["policy_photo_path"].ToString();

                        if (filePath.Keys.Contains("policy_photo_path" + Inscnt))
                            truckInsRow.policy_photo_path = filePath["policy_photo_path" + Inscnt];

                        if (dtinsurnace.Rows[i]["policy_issue_date"].ToString() != "")
                            truckInsRow.policy_issue_date = Convert.ToDateTime(dtinsurnace.Rows[i]["policy_issue_date"].ToString());
                        if (dtinsurnace.Rows[i]["policy_expiry_date"].ToString() != "")
                            truckInsRow.policy_expiry_date = Convert.ToDateTime(dtinsurnace.Rows[i]["policy_expiry_date"].ToString());
                        truckInsRow.status = dtinsurnace.Rows[i]["status"].ToString();

                        truckInsRow.active_flag = dtinsurnace.Rows[i]["active_flag"].ToString();
                        truckInsRow.created_by = dtinsurnace.Rows[i]["created_by"].ToString();
                        truckInsRow.created_date = Convert.ToDateTime(dtinsurnace.Rows[i]["created_date"].ToString());
                        truckInsRow.created_host = dtinsurnace.Rows[i]["created_host"].ToString();
                        truckInsRow.device_id = dtinsurnace.Rows[i]["device_id"].ToString();
                        truckInsRow.device_type = dtinsurnace.Rows[i]["device_type"].ToString();

                        truckInsRow.modified_by = dtinsurnace.Rows[i]["created_by"].ToString();
                        truckInsRow.modified_date = System.DateTime.UtcNow;
                        truckInsRow.modified_host = dtinsurnace.Rows[i]["created_host"].ToString();
                        truckInsRow.modified_device_id = dtinsurnace.Rows[i]["device_id"].ToString();
                        truckInsRow.modified_device_type = dtinsurnace.Rows[i]["device_type"].ToString();


                        objtruck.truck_insurance_detail.Addtruck_insurance_detailRow(truckInsRow);

                        //}
                        Inscnt++;
                    }
                }
                #endregion

                #region Truck Maintenance Details

                int mainval = 1;

                DataTable dtmaintenance = trukmst.GetMaintenanceData(truck1);
                if (dtmaintenance != null)
                {
                    for (int i = 0; i < dtmaintenance.Rows.Count; i++)
                    {
                        //if (filePath.ContainsKey("maintenance_copy" + mainval))
                        //{
                        DS_Truck_Mst.truck_maintenance_detailRow truckMainRow = objtruck.truck_maintenance_detail.Newtruck_maintenance_detailRow();

                        truckMainRow.truck_id = dtmaintenance.Rows[i]["truck_id"].ToString();
                        truckMainRow.maintenance_id = dtmaintenance.Rows[i]["maintenance_id"].ToString();
                        truckMainRow.type_of_maintenance = dtmaintenance.Rows[i]["type_of_maintenance"].ToString();
                        if (dtmaintenance.Rows[i]["maintenance_date"].ToString() != "")
                            truckMainRow.maintenance_date = Convert.ToDateTime(dtmaintenance.Rows[i]["maintenance_date"].ToString());
                        truckMainRow.maintenance_copy = dtmaintenance.Rows[i]["maintenance_copy"].ToString();

                        if (filePath.Keys.Contains("maintenance_copy" + mainval))
                            truckMainRow.maintenance_copy = filePath["maintenance_copy" + mainval];


                        truckMainRow.remarks = dtmaintenance.Rows[i]["remarks"].ToString();
                        truckMainRow.active_flag = dtmaintenance.Rows[i]["active_flag"].ToString();
                        truckMainRow.modified_by = dtmaintenance.Rows[i]["created_by"].ToString();

                        truckMainRow.active_flag = dtmaintenance.Rows[i]["active_flag"].ToString();
                        truckMainRow.created_by = dtmaintenance.Rows[i]["created_by"].ToString();
                        truckMainRow.created_date = Convert.ToDateTime(dtmaintenance.Rows[i]["created_date"].ToString());
                        truckMainRow.created_host = dtmaintenance.Rows[i]["created_host"].ToString();
                        truckMainRow.device_id = dtmaintenance.Rows[i]["device_id"].ToString();
                        truckMainRow.device_type = dtmaintenance.Rows[i]["device_type"].ToString();

                        truckMainRow.modified_date = System.DateTime.UtcNow;
                        truckMainRow.modified_host = dtmaintenance.Rows[i]["created_host"].ToString();
                        truckMainRow.modified_device_id = dtmaintenance.Rows[i]["device_id"].ToString();
                        truckMainRow.modified_device_type = dtmaintenance.Rows[i]["device_type"].ToString();


                        objtruck.truck_maintenance_detail.Addtruck_maintenance_detailRow(truckMainRow);

                        mainval++;
                        //}
                    }
                }
                #endregion


                objBLReturnObject = trukmst.Update_rto_permit_table(objtruck);

                if (objBLReturnObject.ExecutionStatus == 1)
                {
                    if (filePath.Keys.Contains("reg_doc_copy"))
                    {
                        if (dt_truck_rto_detail != null)
                            new Master().DeleteFile(dt_truck_rto_detail.Rows[0]["reg_doc_copy"].ToString().Trim());
                    }
                    if (filePath.Keys.Contains("vehicle_regno_copy"))
                    {
                        if (dt_truck_rto_detail != null)
                            new Master().DeleteFile(dt_truck_rto_detail.Rows[0]["vehicle_regno_copy"].ToString().Trim());
                    }
                    if (filePath.Keys.Contains("vehice_photo"))
                    {
                        if (dt_truck_rto_detail != null)
                            new Master().DeleteFile(dt_truck_rto_detail.Rows[0]["vehice_photo"].ToString().Trim());
                    }
                    if (filePath.Keys.Contains("permit_photo"))
                    {
                        if (dtPermitData != null)
                            new Master().DeleteFile(dtPermitData.Rows[0]["permit_photo"].ToString().Trim());
                    }

                    Inscnt = 1;
                    if (dtinsurnace != null)
                    {
                        for (int i = 0; i < dtinsurnace.Rows.Count; i++)
                        {
                            if (filePath.Keys.Contains("policy_photo_path" + Inscnt))
                            {
                                new Master().DeleteFile(dtinsurnace.Rows[i]["policy_photo_path"].ToString().Trim());
                            }
                            Inscnt++;
                        }
                    }

                    mainval = 1;
                    if (dtmaintenance != null)
                    {
                        for (int i = 0; i < dtmaintenance.Rows.Count; i++)
                        {
                            if (filePath.Keys.Contains("maintenance_copy" + mainval))
                            {
                                new Master().DeleteFile(dtmaintenance.Rows[i]["maintenance_copy"].ToString().Trim());
                            }
                            mainval++;
                        }
                    }
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
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + "______" + truck_id);
                foreach (var pair in data)
                {
                    filePath[pair.Key] = pair.Value;

                    var filePazxth = HttpContext.Current.Server.MapPath(pair.Value);
                    if (File.Exists(filePazxth))
                        File.Delete(filePazxth);
                }
                return ex.Message.ToString();
            }

        }

        public DataTable GetTruckPermitById(string TrukId)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            try
            {
                sb.Clear();
                sb.Append("SELECT * FROM truck_permit_details Where truck_id = @truckid ");

                SqlParameter pr1 = new SqlParameter();
                pr1.DbType = DbType.String;
                pr1.ParameterName = "@truckid";
                pr1.Value = TrukId;


                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                cmd.Parameters.Add(pr1);

                con.Open();
                cmd.ExecuteNonQuery();
                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);
                if (con.State == ConnectionState.Open) con.Close();
                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return dtOwner;
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        public DataTable GetTruckRTOById(string TrukId)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            try
            {
                sb.Clear();
                sb.Append("SELECT * FROM truck_rto_registration_detail Where truck_id = @truckid ");

                SqlParameter pr1 = new SqlParameter();
                pr1.DbType = DbType.String;
                pr1.ParameterName = "@truckid";
                pr1.Value = TrukId;


                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                cmd.Parameters.Add(pr1);

                con.Open();
                cmd.ExecuteNonQuery();
                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);
                if (con.State == ConnectionState.Open) con.Close();
                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return dtOwner;
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        [HttpPost]
        public string GetAllTrucksForDeshboard([FromBody]JObject Jobjdriver)
        {
            List<orders> objOrder = new List<orders>();
            string status = ""; string fromdate = ""; string Todate = "";
            if (Jobjdriver["truck_deshboard"] != null)
            {
                objOrder = Jobjdriver["truck_deshboard"].ToObject<List<orders>>();
                status = objOrder[0].status;
                fromdate = objOrder[0].fromdate;
                Todate = objOrder[0].todate;
            }
            String query1 = "";
            DataTable dtPostLoadOrders = new DataTable();

            query1 = "  select * from( " +
                     "  SELECT distinct driver_mst.Name,truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, " +
                     "  truck_rto_registration_detail.vehice_photo,truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy,  " +
                     "  truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo,    " +
                     "  case  when tralocated.Isfree='N' then 'N' else 'Y' end as Isfree   " +
                     "  from truck_mst  " +
                     " Left Outer Join driver_truck_details on driver_truck_details.truck_id =  truck_mst.truck_id " +
                     " Left Outer Join driver_mst on driver_mst.driver_id =  driver_truck_details.driver_id " +
                     "  LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id and truck_mst.active_flag='Y'   " +
                     "  LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model " +
                     "  LEFT OUTER JOIN truck_make_mst ON truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id and truck_mst.active_flag='Y' " +
                     "  LEFT OUTER JOIN truck_axel_mst ON truck_mst.axle_detail = truck_axel_mst.axel_id and truck_mst.active_flag='Y' " +
                     "  left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id and truck_mst.active_flag='Y'   " +
                     "  LEFT JOIN truck_rto_registration_detail ON truck_rto_registration_detail.truck_id = truck_mst.truck_id   " +
                     "  left join       " +
                     "  (select distinct truck_id,'N' as Isfree  from load_order_enquiry_quotation where  " +
                     "  cast(load_order_enquiry_quotation.status as int) >= 1 and cast(load_order_enquiry_quotation.status as int)  " +
                     "  not in (45,98,99) and truck_id is not null and load_order_enquiry_quotation.active_flag = 'Y')     " +
                     "  tralocated on tralocated.truck_id=truck_mst.truck_id                     " +
                     "  ) as temp   where  1=1 and temp.active_flag='Y'   ";


            if (status != "")
            {
                query1 += " and temp.Isfree='" + status + "' ";
            }
            if (fromdate.Trim() != "" && Todate.Trim() != "")
            {
                query1 += " and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' and CAST(created_date AS DATE)<= '" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (fromdate.Trim() != "")
            {
                query1 += "and CAST(created_date AS DATE)>='" + BLGeneralUtil.ConvertToDateTime(fromdate.ToString(), "dd/mm/yyyy") + "' ";
            }
            else if (Todate.Trim() != "")
            {
                query1 += "and CAST(created_date AS DATE)<='" + BLGeneralUtil.ConvertToDateTime(Todate.ToString(), "dd/mm/yyyy") + "' ";
            }
            query1 += " order by created_date desc ";

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
            DBConnection.Close();
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtPostLoadOrders));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");
        }

        [HttpGet]
        public DataTable TrackTruckLocation(string loadinq)
        {
            // Tracking Data of particular shipement will be accessed using tracking link given to shipper 
            // and this method is to retrieve tracking data from encrypted load inquiry no and truck id.

            string trk = ""; string lnq = "";
            if (loadinq != "" && loadinq.ToString() != string.Empty)
            {
                lnq = BLGeneralUtil.Decrypt(loadinq);
            }

            //get orders details using load inquiry numbers
            DataTable dt_order = new PostOrderController().GetLoadOrdersByID(lnq);


            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();

            query1 = " Select " +
                " convert(varchar(50),order_driver_truck_details.pickup_time,103)+ ' '+convert(varchar(50),order_driver_truck_details.pickup_time,108) as pickup_time, " +
                "     convert(varchar(50),order_driver_truck_details.loadingstart_time,103)+ ' '+convert(varchar(50),order_driver_truck_details.loadingstart_time,108) as loadingstart_time, " +
                "     convert(varchar(50),order_driver_truck_details.start_time,103)+ ' '+convert(varchar(50),order_driver_truck_details.start_time,108) as start_time, " +
                "     convert(varchar(50),order_driver_truck_details.unloadingstart_time,103)+ ' '+convert(varchar(50),order_driver_truck_details.unloadingstart_time,108) as unloadingstart_time," +
                "     convert(varchar(50),order_driver_truck_details.complete_time,103)+ ' '+convert(varchar(50),order_driver_truck_details.complete_time,108) as complete_time," +
                  "    order_driver_truck_details.*,orders.*,tcp.truck_lat,tcp.truck_lng,tcp.truck_location,tcp.remaining_kms,tcp.eta, " +
                                         "    driver_mst.Name as driver_name,driver_mst.mobile_no,driver_license_detail.License_no,truck_rto_registration_detail.vehicle_reg_no " +
                             "   from order_driver_truck_details " +
                             "   left join driver_mst on order_driver_truck_details.driver_id = driver_mst.driver_id  " +
                             "    left join driver_license_detail on driver_mst.driver_id = driver_license_detail.driver_id " +
                             "    left join truck_rto_registration_detail on truck_rto_registration_detail.truck_id = order_driver_truck_details.truck_id " +
                             "  left join orders on orders.load_inquiry_no=order_driver_truck_details.load_inquiry_no " +
                             " left join truck_current_position as tcp on tcp.load_inquiry_no = order_driver_truck_details.load_inquiry_no   " +
                             " and tcp.truck_id = order_driver_truck_details.truck_id and tcp.active_flag= 'Y'  " +
                             "  where order_driver_truck_details.load_inquiry_no=@loadno ";

            //if status of orders is completed then we have to display recordes from history table for web 
            if (dt_order != null)
            {
                if (dt_order.Rows[0]["status"].ToString() == "45" || dt_order.Rows[0]["status"].ToString() == "98" || dt_order.Rows[0]["status"].ToString() == "99")
                { }
                else
                {
                    // below condition is to get active / ongoing orders only.
                    query1 += " and cast(orders.status as int) >= 1 and cast(orders.status as int) not in (45,98,99) ";
                }
            }
            else
            {
                // below condition is to get active / ongoing orders only.
                query1 += " and cast(orders.status as int) >= 1 and cast(orders.status as int) not in (45,98,99) ";
            }

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, lnq));
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (dt_order.Rows[0]["isassign_driver_truck"].ToString() == "N")
                    dtPostLoadOrders = dt_order;
                else
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        dtPostLoadOrders = ds.Tables[0];
                }
            }
            if (dtPostLoadOrders != null && dtPostLoadOrders.Rows.Count > 0)
            {
                //DateTime? pickup_time = dtPostLoadOrders.Rows[0]["pickup_time"].ToString() != "" ? new PostOrderController().DatetimeUTC_ToDubaiConverter("", Convert.ToDateTime(dtPostLoadOrders.Rows[0]["pickup_time"].ToString())) : (DateTime?)null;
                //DateTime? loadingstart_time = dtPostLoadOrders.Rows[0]["loadingstart_time"].ToString() != "" ? new PostOrderController().DatetimeUTC_ToDubaiConverter("", Convert.ToDateTime(dtPostLoadOrders.Rows[0]["loadingstart_time"].ToString())) : (DateTime?)null;
                //DateTime? start_time = dtPostLoadOrders.Rows[0]["start_time"].ToString() != "" ? new PostOrderController().DatetimeUTC_ToDubaiConverter("", Convert.ToDateTime(dtPostLoadOrders.Rows[0]["start_time"].ToString())) : (DateTime?)null;
                //DateTime? unloadingstart_time = dtPostLoadOrders.Rows[0]["unloadingstart_time"].ToString() != "" ? new PostOrderController().DatetimeUTC_ToDubaiConverter("", Convert.ToDateTime(dtPostLoadOrders.Rows[0]["unloadingstart_time"].ToString())) : (DateTime?)null;
                //DateTime? complete_time = dtPostLoadOrders.Rows[0]["complete_time"].ToString() != "" ? new PostOrderController().DatetimeUTC_ToDubaiConverter("", Convert.ToDateTime(dtPostLoadOrders.Rows[0]["complete_time"].ToString())) : (DateTime?)null;


                //dtPostLoadOrders.Rows[0]["pickup_time"] = pickup_time == null ? dtPostLoadOrders.Rows[0]["pickup_time"] : pickup_time.ToString();
                //dtPostLoadOrders.Rows[0]["loadingstart_time"] = loadingstart_time == null ? dtPostLoadOrders.Rows[0]["loadingstart_time"] : loadingstart_time.ToString();
                //dtPostLoadOrders.Rows[0]["start_time"] = start_time == null ? dtPostLoadOrders.Rows[0]["start_time"] : start_time.ToString();
                //dtPostLoadOrders.Rows[0]["unloadingstart_time"] = unloadingstart_time == null ? dtPostLoadOrders.Rows[0]["unloadingstart_time"] : unloadingstart_time.ToString();
                //dtPostLoadOrders.Rows[0]["complete_time"] = complete_time == null ? dtPostLoadOrders.Rows[0]["complete_time"] : complete_time.ToString();

                dtPostLoadOrders = BLGeneralUtil.CheckDateTime(dtPostLoadOrders);
                return dtPostLoadOrders;
            }
            else
                return null;
        }

        [HttpGet]
        public DataTable GetTruckCurrentLocation(string loadinq)
        {
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();

            query1 = " Select order_driver_truck_details.*,orders.*,tcp.truck_lat,tcp.truck_lng,tcp.truck_location,tcp.remaining_kms,tcp.eta, " +
                  "    driver_mst.Name as driver_name,driver_mst.mobile_no,driver_license_detail.License_no,truck_rto_registration_detail.vehicle_reg_no " +
                  "   from order_driver_truck_details " +
                  "   left join driver_mst on order_driver_truck_details.driver_id = driver_mst.driver_id  " +
                  "    left join driver_license_detail on driver_mst.driver_id = driver_license_detail.driver_id " +
                  "    left join truck_rto_registration_detail on truck_rto_registration_detail.truck_id = order_driver_truck_details.truck_id " +
                  "  left join orders on orders.load_inquiry_no=order_driver_truck_details.load_inquiry_no " +
                  " left join truck_current_position as tcp on tcp.load_inquiry_no = order_driver_truck_details.load_inquiry_no   " +
                  " and tcp.truck_id = order_driver_truck_details.truck_id and tcp.active_flag= 'Y'  " +
                  "  where order_driver_truck_details.load_inquiry_no=@loadno ";

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, loadinq));
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

        public string GetTruckHistorylocation(string truckid, string loadinqno)
        {
            StringBuilder sb = new StringBuilder();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();


            sb.Append(" SELECT  orders.load_inquiry_shipping_date,orders.load_inquiry_shipping_time,order_driver_truck_details.*,truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no,driver_mst.mobile_no,driver_mst.Name,truck_current_position_history.driver_id, ");
            sb.Append(" truck_current_position_history.truck_id,truck_current_position_history.log_date,truck_current_position_history.load_inquiry_no,  ");
            sb.Append("   truck_current_position_history.truck_lat,truck_current_position_history.truck_lng,truck_current_position_history.truck_location, truck_current_position_history.eta,  ");
            sb.Append("     truck_current_position_history.remaining_kms,truck_current_position_history.current_kms, truck_current_position_history.total_kms,  ");
            sb.Append("    orders.inquiry_source_lat as startlat,orders.inquiry_source_lng as startlng,orders.inquiry_destionation_lat as dest_lat,orders.inquiry_destionation_lng as dest_lng, ");
            sb.Append("   orders.inquiry_source_addr,orders.inquiry_destination_addr,orders.load_inquiry_load ");
            sb.Append("   FROM truck_current_position_history ");
            sb.Append("   LEFT OUTER JOIN driver_mst ON truck_current_position_history.driver_id = driver_mst.driver_id ");
            sb.Append("   LEFT OUTER JOIN driver_license_detail ON truck_current_position_history.driver_id = driver_license_detail.driver_id ");
            sb.Append("  LEFT OUTER JOIN truck_rto_registration_detail ON truck_current_position_history.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("  LEFT OUTER JOIN orders ON truck_current_position_history.load_inquiry_no = orders.load_inquiry_no ");
            sb.Append("  LEFT OUTER JOIN order_driver_truck_details ON truck_current_position_history.load_inquiry_no = order_driver_truck_details.load_inquiry_no  and order_driver_truck_details.status = '45' ");
            sb.Append("  WHERE (truck_current_position_history.truck_id =@truckid) AND (truck_current_position_history.load_inquiry_no = @loadinqno)   ");
            // sb.Append(" AND (truck_current_position_history.truck_location IS NOT NULL) AND (truck_current_position_history.truck_location <> '') ");

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinqno", DbType.String, loadinqno));
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);

            int no_of_rows = ds.Tables[0].Rows.Count;
            DataSet copydata = new DataSet();
            DataTable dt_temp = new DataTable();
            DataTable dt_temp_copydata = new DataTable();
            int no = 0; double distance = 0;

            if (no_of_rows > 10)
            {
                copydata = ds.Clone();
                copydata.Tables[0].ImportRow(ds.Tables[0].Rows[0]);
                for (int i = 1; i <= 8; i++)
                {
                    no = no_of_rows / 9;
                    no = no * i;
                    copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no]);
                }
                copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no_of_rows - 1]);
                int rcount = copydata.Tables[0].Rows.Count;

                double lat11, lon11, lat22, lon22;
                lat11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lat"].ToString());
                lon11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lng"].ToString());
                lat22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lat"].ToString());
                lon22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lng"].ToString());
                distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');

                dt_temp_copydata.Clear();
                if (distance < 0.1)
                {
                    dt_temp_copydata = copydata.Tables[0].Clone();
                    dt_temp_copydata.TableName = "Table2";

                    DataRow dr_temp_copydata1 = dt_temp_copydata.NewRow();
                    dr_temp_copydata1.ItemArray = copydata.Tables[0].Rows[0].ItemArray;

                    DataRow dr_temp_copydata2 = dt_temp_copydata.NewRow();
                    dr_temp_copydata2.ItemArray = copydata.Tables[0].Rows[copydata.Tables[0].Rows.Count - 1].ItemArray;

                    dt_temp_copydata.Rows.Add(dr_temp_copydata1);
                    dt_temp_copydata.Rows.Add(dr_temp_copydata2);
                }
                //copydata.Tables.Add(dt_temp_copydata);
                //copydata.Tables[0];
            }
            else
            {
                dt_temp_copydata.Clear();
                dt_temp_copydata = ds.Tables[0].Copy();
            }


            if (dt_temp_copydata != null && dt_temp_copydata.Rows.Count > 0)
            {
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_temp_copydata));
            }
            else if (copydata != null)
            {
                if (copydata.Tables.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(copydata.Tables[0]));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Record Found");
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "No Record Found");

        }

        #region MyRegion

        //[HttpGet]
        //public DataTable GetTruckTruckHistorylocation(string truckid, string loadinqno)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    DBDataAdpterObject.SelectCommand.Parameters.Clear();


        //    sb.Append(" SELECT truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no,driver_mst.mobile_no,driver_mst.Name,truck_current_position_history.driver_id, ");
        //    sb.Append(" truck_current_position_history.truck_id,truck_current_position_history.log_date,truck_current_position_history.load_inquiry_no,  ");
        //    sb.Append("   truck_current_position_history.truck_lat,truck_current_position_history.truck_lng,truck_current_position_history.truck_location, truck_current_position_history.eta,  ");
        //    sb.Append("     truck_current_position_history.remaining_kms,truck_current_position_history.current_kms, truck_current_position_history.total_kms,  ");
        //    sb.Append("    orders.inquiry_source_lat as startlat,orders.inquiry_source_lng as startlng,orders.inquiry_destionation_lat as dest_lat,orders.inquiry_destionation_lng as dest_lng, ");
        //    sb.Append("   orders.inquiry_source_addr,orders.inquiry_destination_addr,orders.load_inquiry_load ");
        //    sb.Append("   FROM truck_current_position_history ");
        //    sb.Append("   LEFT OUTER JOIN driver_mst ON truck_current_position_history.driver_id = driver_mst.driver_id ");
        //    sb.Append("   LEFT OUTER JOIN driver_license_detail ON truck_current_position_history.driver_id = driver_license_detail.driver_id ");
        //    sb.Append("  LEFT OUTER JOIN truck_rto_registration_detail ON truck_current_position_history.truck_id = truck_rto_registration_detail.truck_id ");
        //    sb.Append("  LEFT OUTER JOIN orders ON truck_current_position_history.load_inquiry_no = orders.load_inquiry_no ");
        //    sb.Append("  WHERE (truck_current_position_history.truck_id =@truckid) AND (truck_current_position_history.load_inquiry_no = @loadinqno) ");
        //    // sb.Append(" and (truck_current_position_history.truck_location IS NOT NULL) AND (truck_current_position_history.truck_location <> '') ");

        //    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));
        //    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinqno", DbType.String, loadinqno));
        //    if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
        //    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
        //    DataSet ds = new DataSet();
        //    DBDataAdpterObject.Fill(ds);

        //    int no_of_rows = ds.Tables[0].Rows.Count;
        //    DataSet copydata = new DataSet();
        //    DataTable dt_temp = new DataTable();
        //    DataTable dt_temp_copydata = new DataTable();
        //    int no = 0; double distance = 0;

        //    if (no_of_rows > 10)
        //    {
        //        copydata = ds.Clone();
        //        copydata.Tables[0].ImportRow(ds.Tables[0].Rows[0]);
        //        for (int i = 1; i <= 8; i++)
        //        {
        //            no = no_of_rows / 9;
        //            no = no * i;
        //            copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no]);
        //        }
        //        copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no_of_rows - 1]);
        //        int rcount = copydata.Tables[0].Rows.Count;

        //        double lat11, lon11, lat22, lon22;
        //        lat11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lat"].ToString());
        //        lon11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lng"].ToString());
        //        lat22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lat"].ToString());
        //        lon22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lng"].ToString());
        //        distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');

        //        dt_temp_copydata.Clear();
        //        if (distance < 0.1)
        //        {
        //            dt_temp_copydata = copydata.Tables[0].Clone();
        //            dt_temp_copydata.TableName = "Table2";

        //            DataRow dr_temp_copydata1 = dt_temp_copydata.NewRow();
        //            dr_temp_copydata1.ItemArray = copydata.Tables[0].Rows[0].ItemArray;

        //            DataRow dr_temp_copydata2 = dt_temp_copydata.NewRow();
        //            dr_temp_copydata2.ItemArray = copydata.Tables[0].Rows[copydata.Tables[0].Rows.Count - 1].ItemArray;

        //            dt_temp_copydata.Rows.Add(dr_temp_copydata1);
        //            dt_temp_copydata.Rows.Add(dr_temp_copydata2);
        //        }
        //        //copydata.Tables.Add(dt_temp_copydata);
        //        //copydata.Tables[0];
        //    }
        //    else
        //    {
        //        dt_temp_copydata.Clear();
        //        dt_temp_copydata = ds.Tables[0].Copy();
        //    }


        //    if (dt_temp_copydata != null && dt_temp_copydata.Rows.Count > 0)
        //    {
        //        return dt_temp_copydata;
        //    }
        //    else if (copydata != null)
        //    {
        //        if (copydata.Tables.Count > 0)
        //            return copydata.Tables[0];
        //        else
        //            return null;
        //    }
        //    else
        //        return null;

        //}

        #endregion

        [HttpGet]
        public DataTable GetTruckTruckHistorylocation(string truckid, string loadinqno)
        {
            StringBuilder sb = new StringBuilder();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();


            sb.Append(" SELECT truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no,driver_mst.mobile_no,driver_mst.Name,truck_current_position_history.driver_id, ");
            sb.Append(" truck_current_position_history.truck_id,truck_current_position_history.log_date,truck_current_position_history.load_inquiry_no,  ");
            sb.Append("   truck_current_position_history.truck_lat,truck_current_position_history.truck_lng,truck_current_position_history.truck_location, truck_current_position_history.eta,  ");
            sb.Append("     truck_current_position_history.remaining_kms,truck_current_position_history.current_kms, truck_current_position_history.total_kms,  ");
            sb.Append("    orders.inquiry_source_lat as startlat,orders.inquiry_source_lng as startlng,orders.inquiry_destionation_lat as dest_lat,orders.inquiry_destionation_lng as dest_lng, ");
            sb.Append("   orders.inquiry_source_addr,orders.inquiry_destination_addr,orders.load_inquiry_load, NTILE(10) OVER(ORDER By truck_current_position_history.log_date) As row_num ");
            sb.Append("   FROM truck_current_position_history ");
            sb.Append("   LEFT OUTER JOIN driver_mst ON truck_current_position_history.driver_id = driver_mst.driver_id ");
            sb.Append("   LEFT OUTER JOIN driver_license_detail ON truck_current_position_history.driver_id = driver_license_detail.driver_id ");
            sb.Append("  LEFT OUTER JOIN truck_rto_registration_detail ON truck_current_position_history.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("  LEFT OUTER JOIN orders ON truck_current_position_history.load_inquiry_no = orders.load_inquiry_no ");
            sb.Append("  WHERE (truck_current_position_history.truck_id =@truckid) AND (truck_current_position_history.load_inquiry_no = @loadinqno)  ");
            // sb.Append("  (truck_current_position_history.truck_location IS NOT NULL) AND (truck_current_position_history.truck_location <> '') ");
            sb.Append(" ORDER BY  truck_current_position_history.log_date ");

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinqno", DbType.String, loadinqno));
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);

            int no_of_rows = ds.Tables[0].Rows.Count;
            DataSet copydata = new DataSet();
            DataTable dt_temp = new DataTable();
            DataTable dt_temp_copydata = new DataTable();
            int no = 0; double distance = 0;

            //for (int i = 0; i < no_of_rows; i++)
            //{
            //    if (i == 0)
            //    {
            //        dt_temp = ds.Tables[0].Clone();
            //        DataRow dr_temp_copydata1 = dt_temp.NewRow();
            //        dr_temp_copydata1.ItemArray = ds.Tables[0].Rows[0].ItemArray;
            //        dt_temp.Rows.Add(dr_temp_copydata1);
            //    }
            //    else
            //    {
            //        double lat11, lon11, lat22, lon22;
            //        lat11 = Convert.ToDouble(ds.Tables[0].Rows[i - 1]["truck_lat"].ToString());
            //        lon11 = Convert.ToDouble(ds.Tables[0].Rows[i - 1]["truck_lng"].ToString());
            //        lat22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lat"].ToString());
            //        lon22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lng"].ToString());
            //        distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');
            //        if (distance > 0.1)
            //        {
            //            DataRow dr_temp_copydata2 = dt_temp.NewRow();
            //            dr_temp_copydata2.ItemArray = ds.Tables[0].Rows[i].ItemArray;
            //            dt_temp.Rows.Add(dr_temp_copydata2);
            //        }
            //    }
            //}

            //if (dt_temp.Rows.Count > 10)
            //{
            //    Decimal noTemp = dt_temp.Rows.Count / 9.0M;
            //    copydata = ds.Clone();
            //    copydata.Tables[0].ImportRow(dt_temp.Rows[0]);
            //    for (int i = 1; i <= 8; i++)
            //    {
            //        no = (int)(noTemp * (Decimal)i);
            //        copydata.Tables[0].ImportRow(dt_temp.Rows[no]);
            //    }

            //}

            //if (copydata != null && copydata.Tables.Count > 0)
            //    return copydata.Tables[0];
            //else
            //    return null;

            #region temp remove


            if (no_of_rows > 10)
            {
                Decimal noTemp = no_of_rows / 9.0M;
                copydata = ds.Clone();
                copydata.Tables[0].ImportRow(ds.Tables[0].Rows[0]);
                for (int i = 1; i <= 8; i++)
                {
                    //no = no_of_rows / 9;
                    //no = no * i;
                    no = (int)(noTemp * (Decimal)i);
                    copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no]);
                }
                //int index = 2;
                //for (int i = 1; i < ds.Tables[0].Rows.Count - 1; i++)
                //{
                //    if (Convert.ToInt32(ds.Tables[0].Rows[i]["row_num"]) == index)
                //    {
                //        copydata.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                //        if (index == 9)
                //            break;
                //        index++;
                //    }
                //}
                copydata.Tables[0].ImportRow(ds.Tables[0].Rows[no_of_rows - 1]);
                int rcount = copydata.Tables[0].Rows.Count;

                double lat11, lon11, lat22, lon22;
                lat11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lat"].ToString());
                lon11 = Convert.ToDouble(copydata.Tables[0].Rows[0]["truck_lng"].ToString());
                lat22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lat"].ToString());
                lon22 = Convert.ToDouble(copydata.Tables[0].Rows[rcount - 1]["truck_lng"].ToString());
                distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');

                dt_temp_copydata.Clear();
                if (distance < 0.1)
                {
                    dt_temp_copydata = copydata.Tables[0].Clone();
                    dt_temp_copydata.TableName = "Table2";

                    DataRow dr_temp_copydata1 = dt_temp_copydata.NewRow();
                    dr_temp_copydata1.ItemArray = copydata.Tables[0].Rows[0].ItemArray;

                    DataRow dr_temp_copydata2 = dt_temp_copydata.NewRow();
                    dr_temp_copydata2.ItemArray = copydata.Tables[0].Rows[copydata.Tables[0].Rows.Count - 1].ItemArray;

                    dt_temp_copydata.Rows.Add(dr_temp_copydata1);
                    dt_temp_copydata.Rows.Add(dr_temp_copydata2);
                }
            }
            else
            {
                dt_temp_copydata.Clear();
                dt_temp_copydata = ds.Tables[0].Copy();
            }


            if (dt_temp_copydata != null && dt_temp_copydata.Rows.Count > 0)
            {
                return dt_temp_copydata;
            }
            else if (copydata != null && copydata.Tables.Count > 0)
                return copydata.Tables[0];
            else
                return null;

            #endregion

            //if (dt_temp_copydata != null && dt_temp_copydata.Rows.Count > 0)
            //{
            //    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_temp_copydata));
            //}
            //else if (copydata != null && copydata.Tables.Count > 0)
            //    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(copydata.Tables[0]));
            //else
            //    return BLGeneralUtil.return_ajax_string("0", "No Record Found");

        }

        [HttpGet]
        public DataTable GetTruckHistorylocationNew(string truckid, string loadinqno)
        {
            StringBuilder sb = new StringBuilder();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();


            sb.Append(" SELECT truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no,driver_mst.mobile_no,driver_mst.Name,truck_current_position_history.driver_id, ");
            sb.Append(" truck_current_position_history.truck_id,truck_current_position_history.log_date,truck_current_position_history.load_inquiry_no,  ");
            sb.Append("   truck_current_position_history.truck_lat,truck_current_position_history.truck_lng,truck_current_position_history.truck_location, truck_current_position_history.eta,  ");
            sb.Append("     truck_current_position_history.remaining_kms,truck_current_position_history.current_kms, truck_current_position_history.total_kms,  ");
            sb.Append("    orders.inquiry_source_lat as startlat,orders.inquiry_source_lng as startlng,orders.inquiry_destionation_lat as dest_lat,orders.inquiry_destionation_lng as dest_lng, ");
            sb.Append("   orders.inquiry_source_addr,orders.inquiry_destination_addr,orders.load_inquiry_load, NTILE(10) OVER(ORDER By truck_current_position_history.log_date) As row_num ");
            sb.Append("   FROM truck_current_position_history ");
            sb.Append("   LEFT OUTER JOIN driver_mst ON truck_current_position_history.driver_id = driver_mst.driver_id ");
            sb.Append("   LEFT OUTER JOIN driver_license_detail ON truck_current_position_history.driver_id = driver_license_detail.driver_id ");
            sb.Append("  LEFT OUTER JOIN truck_rto_registration_detail ON truck_current_position_history.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("  LEFT OUTER JOIN orders ON truck_current_position_history.load_inquiry_no = orders.load_inquiry_no ");
            sb.Append("  WHERE (truck_current_position_history.truck_id =@truckid) AND (truck_current_position_history.load_inquiry_no = @loadinqno)  ");
            // sb.Append("  (truck_current_position_history.truck_location IS NOT NULL) AND (truck_current_position_history.truck_location <> '') ");
            sb.Append(" ORDER BY  truck_current_position_history.log_date ");

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinqno", DbType.String, loadinqno));
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);

            int no_of_rows = ds.Tables[0].Rows.Count;
            DataSet copydata = new DataSet();
            DataTable dt_temp = new DataTable();
            DataTable dt_temp_copydata = new DataTable();
            int no = 0; double distance = 0; int i = 0;

            for (int j = 0; j < no_of_rows; j++)
            {
                if (j == 0)
                {
                    dt_temp = ds.Tables[0].Clone();
                    DataRow dr_temp_copydata1 = dt_temp.NewRow();
                    dr_temp_copydata1.ItemArray = ds.Tables[0].Rows[0].ItemArray;
                    dt_temp.Rows.Add(dr_temp_copydata1);
                }
                else
                {
                    double lat11, lon11, lat22, lon22;
                    lat11 = Convert.ToDouble(ds.Tables[0].Rows[j]["truck_lat"].ToString());
                    lon11 = Convert.ToDouble(ds.Tables[0].Rows[j]["truck_lng"].ToString());
                    lat22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lat"].ToString());
                    lon22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lng"].ToString());
                    distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');
                    if (distance > 0.1)
                    {
                        DataRow dr_temp_copydata2 = dt_temp.NewRow();
                        dr_temp_copydata2.ItemArray = ds.Tables[0].Rows[j].ItemArray;
                        dt_temp.Rows.Add(dr_temp_copydata2);
                        i = j;
                    }
                }
            }

            if (dt_temp.Rows.Count > 10)
            {
                Decimal noTemp = dt_temp.Rows.Count / 9.0M;
                copydata = ds.Clone();
                copydata.Tables[0].ImportRow(dt_temp.Rows[0]);
                for (int k = 1; k <= 8; k++)
                {
                    no = (int)(noTemp * (Decimal)k);
                    copydata.Tables[0].ImportRow(dt_temp.Rows[no]);
                }

            }

            if (copydata != null && copydata.Tables.Count > 0)
                return copydata.Tables[0];
            else
                return null;

        }

        [HttpGet]
        public DataTable GetTruckOngoingLocation(string truckid, string loadinqno)
        {
            StringBuilder sb = new StringBuilder();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();


            sb.Append(" SELECT truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no,driver_mst.mobile_no,driver_mst.Name,truck_current_position.driver_id, ");
            sb.Append(" truck_current_position.truck_id,truck_current_position.log_date,truck_current_position.load_inquiry_no,  ");
            sb.Append("   truck_current_position.truck_lat,truck_current_position.truck_lng,truck_current_position.truck_location, truck_current_position.eta,  ");
            sb.Append("     truck_current_position.remaining_kms,truck_current_position.current_kms, truck_current_position.total_kms,  ");
            sb.Append("    orders.inquiry_source_lat as startlat,orders.inquiry_source_lng as startlng,orders.inquiry_destionation_lat as dest_lat,orders.inquiry_destionation_lng as dest_lng, ");
            sb.Append("   orders.inquiry_source_addr,orders.inquiry_destination_addr,orders.load_inquiry_load, NTILE(10) OVER(ORDER By truck_current_position.log_date) As row_num ");
            sb.Append("   FROM truck_current_position ");
            sb.Append("   LEFT OUTER JOIN driver_mst ON truck_current_position.driver_id = driver_mst.driver_id ");
            sb.Append("   LEFT OUTER JOIN driver_license_detail ON truck_current_position.driver_id = driver_license_detail.driver_id ");
            sb.Append("  LEFT OUTER JOIN truck_rto_registration_detail ON truck_current_position.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("  LEFT OUTER JOIN orders ON truck_current_position.load_inquiry_no = orders.load_inquiry_no ");
            sb.Append("  WHERE (truck_current_position.truck_id =@truckid) AND (truck_current_position.load_inquiry_no = @loadinqno)  ");
            sb.Append(" ORDER BY  truck_current_position.log_date ");

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, truckid));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadinqno", DbType.String, loadinqno));
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);

            int no_of_rows = ds.Tables[0].Rows.Count;
            DataTable dt_temp = new DataTable();
            double distance = 0; int i = 0;

            for (int j = 0; j < no_of_rows; j++)
            {
                if (j == 0)
                {
                    dt_temp = ds.Tables[0].Clone();
                    DataRow dr_temp_copydata1 = dt_temp.NewRow();
                    dr_temp_copydata1.ItemArray = ds.Tables[0].Rows[0].ItemArray;
                    dt_temp.Rows.Add(dr_temp_copydata1);
                }
                else
                {
                    double lat11, lon11, lat22, lon22;
                    lat11 = Convert.ToDouble(ds.Tables[0].Rows[j]["truck_lat"].ToString());
                    lon11 = Convert.ToDouble(ds.Tables[0].Rows[j]["truck_lng"].ToString());
                    lat22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lat"].ToString());
                    lon22 = Convert.ToDouble(ds.Tables[0].Rows[i]["truck_lng"].ToString());
                    distance = GetTotaldistance(lat11, lon11, lat22, lon22, 'M');
                    if (distance > 0.1)
                    {
                        DataRow dr_temp_copydata2 = dt_temp.NewRow();
                        dr_temp_copydata2.ItemArray = ds.Tables[0].Rows[j].ItemArray;
                        dt_temp.Rows.Add(dr_temp_copydata2);
                        i = j;
                    }
                }
            }

            if (dt_temp != null && dt_temp.Rows.Count > 0)
                return dt_temp;
            else
                return null;
        }

        [HttpPost]
        public string GetTrucksNearByLocation([FromBody]JObject Jobjdata)//string lat, string lng, string MinDistance, string TruckTypeCode)
        {
            List<orders> objOrder = new List<orders>();
            DataTable dt_trucks = new DataTable();
            string current_lat = ""; string current_lng = ""; string MinDistance = ""; string truckType = "";
            if (Jobjdata["order"] != null)
            {
                objOrder = Jobjdata["order"].ToObject<List<orders>>();
                current_lat = objOrder[0].current_lat;
                current_lng = objOrder[0].current_lng;
                MinDistance = objOrder[0].MinDistance;
                truckType = objOrder[0].truckType;
            }

            dt_trucks = GetTrucksNearByLocation(current_lat, current_lng, MinDistance, truckType);
            if (dt_trucks != null && dt_trucks.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_trucks)));
            else
                return (BLGeneralUtil.return_ajax_string("0", " Trucks not found "));
        }

        [HttpGet]
        public string GetTrucksBodyType(string typecode)
        {
            DataTable dttruckbodytype = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            if (typecode.ToUpper() == "T")
                query1 = "select SizeTypeDesc as truck_body_desc,sizetypecode as truck_body_id from sizetypemst  WHERE (SizeTypeCode NOT LIKE 'SZ%')  and Isactive='Y' order by sr_no ";
            else if (typecode.ToUpper() == "HT")
                query1 = " select SizeTypeDesc as truck_body_desc,sizetypecode as truck_body_id from sizetypemst  WHERE hiretruck_flag='Y'  and Isactive='Y' order by sr_no  ";

            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dttruckbodytype = ds.Tables[0];
            }
            if (dttruckbodytype != null && dttruckbodytype.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dttruckbodytype)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Trucks not found"));
        }



        public DataTable GetTrucksNearByLocation(string lat, string lng, string MinDistance, string TruckTypeCode)
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
                   " select top 10 * from ( " +
                   "  SELECT truck_body_mst.truck_body_id,dest.driver_id,dest.truck_id,dest.truck_lat,dest.truck_lng,dest.truck_location,dest.active_flag, " +
                   " 3956 * 2 * ASIN(SQRT( " +
                   " POWER(SIN((@orig_lat - abs(case when dest.truck_lat='' Then '0' else dest.truck_lat end)) * pi()/180 / 2), 2) +  COS(@orig_lat * pi()/180 ) * " +
                   " COS(abs(case when dest.truck_lat='' Then '0' else dest.truck_lat end) * pi()/180) *  " +
                   " POWER(SIN((@orig_lon - case when dest.truck_lng='' Then '0' else dest.truck_lng end) * pi()/180 / 2), 2) )) as  distance " +
                   " FROM truck_current_position dest " +
                   " join truck_mst on dest.truck_id =  truck_mst.truck_id " +
                   " left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id  " +
                   " left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id " +
                   " left join truck_rto_registration_detail on dest.truck_id = truck_rto_registration_detail.truck_id   " +
                   " left join truck_permit_details on dest.truck_id = truck_permit_details.truck_id   " +
                   " left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id " +
                   " join driver_mst on driver_mst.driver_id = dest.driver_id and driver_mst.isfree='Y' ) as temp " +
                   " where active_flag='Y' and  truck_body_id='" + truckType + "' and truck_lat <> '' and truck_lng <> '' order by distance ";

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

        // by sandip(09-08-2016)
        // create new service for track ongoing order 
        // in this service send data in bunch of 10 
        [HttpGet]
        public DataTable TrackTruckLocationNew(string truckid, string lid, string srno)
        {
            // Tracking Data of particular shipement will be accessed using tracking link given to shipper 
            // and this method is to retrieve tracking data from encrypted load inquiry no and truck id.

            string trk = ""; string lnq = ""; Int32 Sr_no = 0;
            if (truckid != "" && truckid.ToString() != string.Empty)
            {
                trk = BLGeneralUtil.Decrypt(truckid);
            }
            if (lid != "" && lid.ToString() != string.Empty)
            {
                lnq = BLGeneralUtil.Decrypt(lid);
            }
            if (srno != "" && srno.ToString() != string.Empty)
            {
                Sr_no = Convert.ToInt32(srno);
            }

            DataTable dt = new DataTable();
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            query1 = " select * from " +
                       "(select NTILE((select COUNT(*)/9 from truck_current_position where load_inquiry_no=@loadno)) OVER(ORDER By TC.log_date) As batch_no,* " +
                       "from " +
                       "(   select ROW_NUMBER() OVER (ORDER BY truck_current_position.log_date) AS [sr_no],* " +
                       " from truck_current_position where load_inquiry_no=@loadno ) as TC)  " +
                       " as temp where 1=1 ";
            if (Sr_no == 0)
            { }
            else
                query1 += "and temp.sr_no> " + Sr_no;

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, lnq));
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

        // by sandip(09-08-2016)
        // create new service for track ongoing order 
        // in this service send data in bunch of 10 
        [HttpGet]
        public string TrackTruckLocation_New(string truckid, string lid, string srno)
        {
            // Tracking Data of particular shipement will be accessed using tracking link given to shipper 
            // and this method is to retrieve tracking data from encrypted load inquiry no and truck id.

            string lnq = ""; Int32 Sr_no = 0;
            if (srno != "" && srno.ToString() != string.Empty)
            {
                Sr_no = Convert.ToInt32(srno);
            }

            DataTable dt = new DataTable();
            DataTable dtPostLoadOrders = new DataTable(); String query1 = "";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            query1 = " select * from " +
                       "(select NTILE((select COUNT(*)/9 from truck_current_position where load_inquiry_no=@loadno)) OVER(ORDER By TC.log_date) As batch_no,* " +
                       "from " +
                       "(   select ROW_NUMBER() OVER (ORDER BY truck_current_position.log_date) AS [sr_no],* " +
                       " from truck_current_position where load_inquiry_no=@loadno ) as TC)  " +
                       " as temp where 1=1 ";
            if (Sr_no == 0)
            { }
            else
                query1 += "and temp.sr_no> " + Sr_no;

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("loadno", DbType.String, lnq));
            DBDataAdpterObject.SelectCommand.CommandText = query1;
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
                return BLGeneralUtil.return_ajax_string("0", "No Data Found");

        }

        // get distance between two lat lons 
        public double GetTotaldistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        [HttpGet]
        public string GetAllTrucksForMap()
        {
            String query = @"SELECT case when truck_body_mst.truck_body_id='0001' and driver_mst.IsOnDuty='Y'  then 'G' 
                              when truck_body_mst.truck_body_id='0002' and driver_mst.IsOnDuty='Y' then 'O' 
                              when truck_body_mst.truck_body_id='0003' and driver_mst.IsOnDuty='Y'then 'Y'  
                              when truck_body_mst.truck_body_id='0004' and driver_mst.IsOnDuty='Y'then 'BL'  
                              when truck_body_mst.truck_body_id='0005' and driver_mst.IsOnDuty='Y'then 'BLK' 
                              when truck_body_mst.truck_body_id='0047' and driver_mst.IsOnDuty='Y' then 'B' 
                              when truck_body_mst.truck_body_id='0028' and driver_mst.IsOnDuty='Y'then 'R' 
                              when driver_mst.IsOnDuty='N' then 'P'  end as color_code, 
                             driver_mst.IsOnDuty,DATEDIFF(MINUTE, dest.created_date,GETUTCDATE()) as timeDiffence,dest.created_date,driver_mst.name,driver_mst.mobile_no,truck_rto_registration_detail.vehicle_reg_no,driver_license_detail.License_no, 
                             truck_body_mst.truck_body_desc,truck_body_mst.truck_body_id,dest.driver_id,dest.truck_id,dest.truck_lat,dest.truck_lng,dest.truck_location,dest.active_flag FROM truck_current_position dest  
                             join truck_mst on dest.truck_id =  truck_mst.truck_id 
                             JOIN driver_license_detail ON dest.driver_id = driver_license_detail.driver_id 
                             left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id  
                             left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id 
                             left join truck_rto_registration_detail on dest.truck_id = truck_rto_registration_detail.truck_id   
                             left join truck_permit_details on dest.truck_id = truck_permit_details.truck_id   
                             left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id 
                             join driver_mst on driver_mst.driver_id = dest.driver_id and dest.active_flag='Y' 
                             and truck_lat <> '' and truck_lng <> '' 
                             where  DATEDIFF(MINUTE, dest.created_date,GETUTCDATE()) < 120 and dest.driver_id in ( 
                             select driver_id from driver_login_detail where driver_login_detail.Is_logedin='Y') ";

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
                return (BLGeneralUtil.return_ajax_string("0", "Truck details not found "));
        }

        public string SaveTruckPositionNew([FromBody]truck_current_position objtruck)
        {


            Master master = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            Document objdoc = new Document(); string skms = "";
            string temploaddoc = ""; decimal totalkms = 0; decimal currentkms = 0;
            DS_Truck_current_location ds_location = new DS_Truck_current_location(); decimal rkms = 0; decimal rkms_old = 0; decimal tkms = 0;


            ServerLog.SuccessLog("SaveTruckPositionNew(" + JsonConvert.SerializeObject(objtruck) + ")");

            #region Check valid Driver device register

            DataTable dtDriverdetailsBydeviceid = new LoginController().GetDriverdetailsBydeviceid(objtruck.device_id);
            if (dtDriverdetailsBydeviceid != null)
            {
                objtruck.driver_id = dtDriverdetailsBydeviceid.Rows[0]["driver_id"].ToString();
                objtruck.truck_id = dtDriverdetailsBydeviceid.Rows[0]["truck_id"].ToString();

                if (dtDriverdetailsBydeviceid.Rows[0]["driver_id"].ToString() != "")
                    objtruck.driver_id = dtDriverdetailsBydeviceid.Rows[0]["driver_id"].ToString();
                if (dtDriverdetailsBydeviceid.Rows[0]["truck_id"].ToString() != "")
                    objtruck.truck_id = dtDriverdetailsBydeviceid.Rows[0]["truck_id"].ToString();
                if (dtDriverdetailsBydeviceid.Rows[0]["status"].ToString() != "02")
                {
                    if (dtDriverdetailsBydeviceid.Rows[0]["load_inquiry_no"].ToString() != "")
                        objtruck.load_inquiry_no = dtDriverdetailsBydeviceid.Rows[0]["load_inquiry_no"].ToString();
                }
            }
            else
            {
                return (BLGeneralUtil.return_ajax_string("0", " Driver details not found "));
            }

            DataTable dtdeviceIds = new LoginController().GetDriverdeviceIDByDriverID(objtruck.driver_id);
            if (dtdeviceIds != null)
            {
                DataRow[] dr = dtdeviceIds.Select("Is_logedin = 'Y'");
                if (dr.Length > 0)
                    if (dr[0].ItemArray[3].ToString() != objtruck.device_id)
                    {
                        return (BLGeneralUtil.return_ajax_string("3", " User login another Device Loged Out"));
                    }
            }
            #endregion

            if (objtruck.truck_lat == null || objtruck.truck_lat == "")
                return BLGeneralUtil.return_ajax_string("0", " Latitude is not found");

            if (objtruck.truck_lng == null || objtruck.truck_lng == "")
                return BLGeneralUtil.return_ajax_string("0", "Inquiry source Longitude is not found");

            if (objtruck.truck_lat.Trim() != "")
                if (Math.Round(Convert.ToDecimal(objtruck.truck_lat)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "latlong is Zero");

            if (objtruck.truck_lng.Trim() != "")
                if (Math.Round(Convert.ToDecimal(objtruck.truck_lng)) == 0)
                    return BLGeneralUtil.return_ajax_string("0", "latlong is Zero");

            if (objtruck.created_by == null || objtruck.created_by == "")
                return BLGeneralUtil.return_ajax_string("0", "Created By value not found");

            if (objtruck.truck_id == null || objtruck.truck_id == "")
                return BLGeneralUtil.return_ajax_string("0", "truck Id not found");

            #region extract kms (decimal part) from post

            if (objtruck.remaining_kms != null && objtruck.remaining_kms.ToString() != "")
                skms = objtruck.remaining_kms;

            if (skms != "")
            {
                if (skms.IndexOf(" ") != 0)
                {
                    if (skms.Substring(skms.IndexOf(" "), (skms.Length - skms.IndexOf(" "))).Trim() == "m")
                    {
                        skms = skms.Substring(0, skms.IndexOf(" "));
                        rkms = Convert.ToDecimal(skms);
                        rkms = rkms / 1000;
                        rkms = Math.Round(rkms, 2);
                    }
                    else
                    {
                        skms = skms.Substring(0, skms.IndexOf(" "));
                        rkms = Convert.ToDecimal(skms);
                    }
                }
            }
            #endregion

            try
            {
                if (objtruck != null)
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();
                    try
                    {
                        temploaddoc = objtruck.load_inquiry_no;
                        //DataTable dt_truck = GetTruckByID(objtruck.truck_id, objtruck.load_inquiry_no);
                        DataTable dt_truck = GetTruckByID(objtruck.truck_id, "-1");
                        rkms_old = 0;
                        if (dt_truck != null && dt_truck.Rows.Count > 0)
                        {
                            ds_location.EnforceConstraints = false;
                            for (int i = 0; i < dt_truck.Rows.Count; i++)
                            {
                                dt_truck.Rows[i]["active_flag"] = Constant.Flag_No;
                                if (dt_truck.Rows[i]["remaining_kms"] == DBNull.Value || dt_truck.Rows[i]["remaining_kms"].ToString() == "")
                                    rkms_old = 0;
                                else
                                {
                                    skms = dt_truck.Rows[i]["remaining_kms"].ToString();
                                    if (skms.Substring(skms.IndexOf(" "), (skms.Length - skms.IndexOf(" "))).Trim() == "m")
                                    {
                                        skms = skms.Substring(0, skms.IndexOf(" "));
                                        rkms_old = Convert.ToDecimal(skms);
                                        rkms_old = rkms_old / 1000;
                                        rkms_old = Math.Round(rkms, 2);
                                    }
                                    else
                                    {
                                        skms = skms.Substring(0, skms.IndexOf(" "));
                                        rkms_old = Convert.ToDecimal(skms);
                                    }
                                }

                                if (dt_truck.Rows[i]["total_kms"] == DBNull.Value || dt_truck.Rows[i]["total_kms"].ToString() == "")
                                    tkms = 0;
                                else
                                    tkms = Convert.ToDecimal(dt_truck.Rows[i]["total_kms"]);

                                ds_location.truck_current_position.ImportRow(dt_truck.Rows[i]);
                                ds_location.truck_current_position.Rows[i].AcceptChanges();
                                if (objtruck.load_inquiry_no == null || objtruck.load_inquiry_no.ToString() == "")
                                    ds_location.truck_current_position.Rows[i].Delete();
                                else
                                    ds_location.truck_current_position.Rows[i].SetAdded();
                            }
                        }
                        if (rkms == 0)
                        {
                            if (rkms_old == 0)
                                rkms = 0;
                            else
                                rkms = rkms_old;
                        }

                        if (rkms_old == 0)
                            currentkms = 0;
                        else
                            currentkms = (rkms_old - rkms);

                        if (currentkms < 0) currentkms = currentkms * -1;

                        #region truck_position_Data
                        DS_Truck_current_location.truck_current_positionRow tr = ds_location.truck_current_position.Newtruck_current_positionRow();
                        ds_location.EnforceConstraints = false;
                        tr.truck_id = objtruck.truck_id;
                        tr.driver_id = objtruck.driver_id;
                        tr.log_date = System.DateTime.UtcNow;
                        tr.load_inquiry_no = objtruck.load_inquiry_no;
                        tr.truck_lat = objtruck.truck_lat;
                        tr.truck_lng = objtruck.truck_lng;
                        tr.truck_location = objtruck.truck_location;
                        tr.remaining_kms = objtruck.remaining_kms;
                        tr.eta = objtruck.eta;
                        tr.current_kms = currentkms;
                        tr.total_kms = tkms + currentkms;
                        tr.active_flag = Constant.Flag_Yes;
                        tr.created_by = objtruck.created_by;
                        tr.created_date = System.DateTime.UtcNow;
                        tr.created_host = objtruck.created_host;
                        tr.device_id = objtruck.device_id;
                        tr.device_type = objtruck.device_type;
                        ds_location.EnforceConstraints = false;
                        ds_location.truck_current_position.Addtruck_current_positionRow(tr);
                        int indx = ds_location.truck_current_position.Rows.Count - 1;
                        ds_location.truck_current_position.Rows[indx].AcceptChanges();
                        ds_location.truck_current_position.Rows[indx].SetAdded();
                        #endregion


                        objBLReturnObject = master.UpdateTables(ds_location.truck_current_position, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus == 2)
                        {
                            ServerLog.Log("SaveTruckPositionNew(" + objBLReturnObject.ServerMessage.ToString() + ")");
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            objBLReturnObject.ExecutionStatus = 2;
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("SaveTruckPositionNew(" + ex.Message + Environment.NewLine + ex.StackTrace + ")");
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        return BLGeneralUtil.return_ajax_string("0", ex.Message);
                    }

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("SaveTruckPositionNew( Truck Current Position Saved : " + JsonConvert.SerializeObject(objtruck) + ")");
                    return BLGeneralUtil.return_ajax_string("1", objBLReturnObject.ServerMessage);
                }
                else
                {
                    ServerLog.SuccessLog("SaveTruckPosition( No Data To Save For: " + temploaddoc + ")");
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    return BLGeneralUtil.return_ajax_string("0", "No Data To Save ");
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("SaveTruckPosition(" + ex.Message + Environment.NewLine + ex.StackTrace + ")");
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpGet]
        public string GetNewTrucksDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT     truck_mst.truck_id, truck_mst.reg_date, truck_mst.truck_make_id, truck_mst.truck_model, truck_mst.year_of_mfg, truck_mst.load_capacity, truck_mst.axle_detail, 
                        truck_mst.body_type, truck_mst.current_millage, truck_mst.avg_millage_per_month, truck_mst.fuel_average, truck_mst.registration_details, truck_mst.active_flag,  
                        truck_mst.created_by, truck_mst.created_date, truck_mst.created_host, truck_mst.device_id, truck_mst.device_type, truck_mst.modified_by, truck_mst.modified_date, 
                        truck_mst.modified_host, truck_mst.modified_device_id, truck_mst.modified_device_type, truck_make_mst.make_name, truck_model_mst.model_desc,  
                        truck_body_mst.truck_body_desc,truck_rto_registration_detail.*,truck_permit_details.*  FROM         truck_mst 
                        LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id  
                        LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model 
                        LEFT OUTER JOIN truck_make_mst ON truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id
                        LEFT OUTER JOIN truck_axel_mst ON truck_mst.axle_detail = truck_axel_mst.axel_id  
                        LEFT OUTER JOIN truck_rto_registration_detail ON truck_mst.truck_id = truck_rto_registration_detail.truck_id  
                        LEFT OUTER JOIN truck_permit_details ON truck_mst.truck_id = truck_permit_details.truck_id
                        WHERE  truck_mst.active_flag = 'Y' and truck_mst.truck_id not in (select truck_id from driver_truck_details where active_flag='Y')");

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
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "Driver details not found");
        }

        [HttpGet]
        public DataSet GetDriverTruckAdditionalDetails(string TrukId, string driverID, string orderid)
        {
            sb.Clear();
            sb.Append(@"SELECT     truck_mst.truck_id, truck_mst.reg_date, truck_mst.truck_make_id, truck_mst.truck_model, truck_mst.year_of_mfg, truck_mst.load_capacity, truck_mst.axle_detail, 
                        truck_mst.body_type, truck_mst.current_millage, truck_mst.avg_millage_per_month, truck_mst.fuel_average, truck_mst.registration_details, truck_mst.active_flag,  
                        truck_mst.created_by, truck_mst.created_date, truck_mst.created_host, truck_mst.device_id, truck_mst.device_type, truck_mst.modified_by, truck_mst.modified_date, 
                        truck_mst.modified_host, truck_mst.modified_device_id, truck_mst.modified_device_type, truck_make_mst.make_name, truck_model_mst.model_desc,  
                        truck_body_mst.truck_body_desc,truck_rto_registration_detail.*,truck_permit_details.*  FROM   truck_mst 
                        LEFT OUTER JOIN truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id  
                        LEFT OUTER JOIN truck_model_mst ON truck_model_mst.model_id = truck_mst.truck_model 
                        LEFT OUTER JOIN truck_make_mst ON truck_make_mst.make_id = truck_mst.truck_make_id AND truck_model_mst.make_id = truck_make_mst.make_id
                        LEFT OUTER JOIN truck_axel_mst ON truck_mst.axle_detail = truck_axel_mst.axel_id  
                        LEFT OUTER JOIN truck_rto_registration_detail ON truck_mst.truck_id = truck_rto_registration_detail.truck_id  
                        LEFT OUTER JOIN truck_permit_details ON truck_mst.truck_id = truck_permit_details.truck_id
                        WHERE  truck_mst.active_flag = 'Y' AND truck_mst.truck_id = @truckid 
                        select * from truck_insurance_detail where truck_id= @truckid  and active_flag='Y'
                        select * from truck_maintenance_detail where truck_id= @truckid  and active_flag='Y'
                        select * from driver_mst 
                        left join  user_mst on user_mst.unique_id=driver_mst.driver_id 
                        left join  driver_contact_detail on driver_contact_detail.driver_id=driver_mst.driver_id 
                        left join  driver_license_detail on driver_license_detail.driver_id=driver_mst.driver_id 
                        left join  driver_truck_details on driver_truck_details.driver_id=driver_mst.driver_id 
                         where driver_mst.driver_id= @driverid");
            if (orderid != "")
            {
                sb.Append(" select * from orders where load_inquiry_no= @orderid ");
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("orderid", DbType.String, orderid));
            }

            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("truckid", DbType.String, TrukId));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("driverid", DbType.String, driverID));
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DataSet dsOwner = new DataSet();
            DBDataAdpterObject.Fill(dsOwner);
            DataTable dttruck = new DataTable();
            if (dsOwner != null && dsOwner.Tables.Count > 0)
                return dsOwner;
            else
                return null;
        }

        [HttpGet]
        public string GetDriverTruckAdditionalDetailsforApp(string TrukId, string driverID, string orderid)
        {
            DataSet ds = new DataSet();
            ds = GetDriverTruckAdditionalDetails(TrukId, driverID, orderid);

            if (ds != null)
            {

                if (ds.Tables.Count > 0)
                    return SendReceiveJSon.GetJsonFromDataSet(1, "data found", ds);
                else
                    return BLGeneralUtil.return_ajax_string("0", "Details not found");

                //if (ds.Tables.Count > 0)
                //    return "{\"status\":\"" + 1 + "\",\"Truckdetails\":" + SendReceiveJSon.GetJson(ds.Tables[0]) + ",\"TruckInsDtl\":" + SendReceiveJSon.GetJson(ds.Tables[1]) + ",\"TruckMaintDtl\":" + SendReceiveJSon.GetJson(ds.Tables[2]) + ",\"Driverdtl\":" + SendReceiveJSon.GetJson(ds.Tables[3]) + ",\"OrderDtl\":" + SendReceiveJSon.GetJson(ds.Tables[4]) + "}";
                //else
                //    return BLGeneralUtil.return_ajax_string("0", "Details not found");
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "Details not found");
        }

    }
}