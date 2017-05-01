using Newtonsoft.Json.Linq;
using System;
using System.Text;
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
using Newtonsoft.Json;

namespace trukkerUAE.Controllers
{
    public class ownerController : ServerBase
    {

        Master drvmst = new Master();
        DS_Owner_Mst objdriver = new DS_Owner_Mst();
        BLReturnObject objBLReturnObject = new BLReturnObject();
        JavaScriptSerializer jser = new JavaScriptSerializer();
        StringBuilder sb = new StringBuilder();

        #region Get methods
        // GET api/owner
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        public string GetOwnerDetails()
        {
            //SqlConnection con = new SqlConnection("Data Source=FWSERVER2\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT * FROM owner_master";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtOwner);
            string strOwner = "";

            //CarStocks ST = new CarStocks();
            //CarStocks ST1 = new CarStocks();
            //List<CarStocks> li = new List<CarStocks>();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            if (dtOwner != null && dtOwner.Rows.Count > 0)
            {
                //str = GetJson1(dt);



                //Convert DataTable to List<Dictionary<string, string>> data structure
                foreach (DataRow VDataRow in dtOwner.Rows)
                {
                    var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
                    foreach (DataColumn Column in dtOwner.Columns)
                    {

                        Row.Add(Column.ColumnName, VDataRow[Column].ToString());

                    }
                    dataRows.Add(Row);
                }

                strOwner = ser.Serialize(dataRows); // convert list to JSON string 


                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    //ST.CarName = dt.Rows[i]["make_name"].ToString();
                //    //ST.CarModel = dt.Rows[i]["language_id"].ToString();
                //    //ST.CarPrice = dt.Rows[i]["active_flag"].ToString();

                //    //li[i].Add(ST);
                //}
            }

            return strOwner;
            //   return li;
        }

        [HttpGet]
        public string GetOwnersByUser(String userid)
        {
            //SqlConnection con = new SqlConnection("Data Source=FWSERVER2\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT * FROM owner_master Where created_by = @userid ";

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@userid";
            pr1.Value = userid;

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


            //   return li;
        }

        [HttpGet]
        public DataTable GetOwnersByTruckID(String truckid)
        {
            //SqlConnection con = new SqlConnection("Data Source=FWSERVER2\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT * FROM owner_truck_details Where truck_id = @trkid and active_flag = 'Y' ";

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@trkid";
            pr1.Value = truckid;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(pr1);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtOwner = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtOwner);

            if (dtOwner != null && dtOwner.Rows.Count > 0)
            {
                return dtOwner;
                //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                //return "1";
            }
            else
                return null;


            //   return li;
        }

        public string GetTrucksOwnerUserwise(string userid, string ownerid)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT owner_master.owner_id, owner_master.owner_name, truck_mst.truck_id, truck_make_mst.make_name, truck_model_mst.model_desc, ");
            sb.Append("truck_body_mst.truck_body_desc FROM truck_model_mst INNER JOIN truck_make_mst ");
            sb.Append("ON truck_model_mst.make_id = truck_make_mst.make_id INNER JOIN owner_truck_details INNER JOIN ");
            sb.Append("truck_mst ON truck_mst.truck_id = owner_truck_details.truck_id INNER JOIN owner_master ON owner_truck_details.owner_id = owner_master.owner_id ");
            sb.Append("ON truck_make_mst.make_id = truck_mst.truck_make_id AND  truck_model_mst.model_id = truck_mst.truck_model INNER JOIN ");
            sb.Append("truck_body_mst ON truck_mst.body_type = truck_body_mst.truck_body_id Where truck_mst.active_flag = 'Y'");
            sb.Append(" and owner_truck_details.created_by = @userid and owner_truck_details.owner_id = @ownerid ");

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@userid";
            pr1.Value = userid;

            SqlParameter pr2 = new SqlParameter();
            pr2.DbType = DbType.String;
            pr2.ParameterName = "@ownerid";
            pr2.Value = ownerid;

            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            cmd.Parameters.Add(pr1);
            cmd.Parameters.Add(pr2);

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
        public string GetAllTruckByOwner(string ownerid)
        {

            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, ");
            sb.Append("truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, ");
            sb.Append("truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo ");
            sb.Append("from truck_mst ");
            sb.Append("left join owner_truck_details on truck_mst.truck_id = owner_truck_details.truck_id ");
            sb.Append("left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id ");
            sb.Append("left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id ");
            sb.Append("left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id ");
            sb.Append("left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id ");
            sb.Append("Where owner_truck_details.owner_id=@ownid and owner_truck_details.active_flag= 'Y' and truck_permit_details.active_flag= 'Y' ");

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@ownid";
            pr1.Value = ownerid;


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
        public string GetAllTruckByOwnerByBodyType(string ownerid, String bodytype)
        {
            // Status 45 = Order Status Completed - By Driver , Status 98 = Quote Not Selected , Status 99 = Quote Rejected By Shipper 
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            sb.Clear();
            sb.Append("SELECT distinct truck_mst.*,truck_make_mst.make_name,truck_model_mst.model_desc,truck_body_mst.truck_body_desc, ");
            sb.Append("truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, ");
            sb.Append("truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo ");
            sb.Append("from truck_mst ");
            sb.Append("left join owner_truck_details on truck_mst.truck_id = owner_truck_details.truck_id ");
            sb.Append("left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id ");
            sb.Append("left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id ");
            sb.Append("left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id  ");
            sb.Append("left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id  ");
            sb.Append("left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id  ");
            sb.Append("Where owner_truck_details.owner_id= @ownid and owner_truck_details.active_flag= 'Y' ");
            sb.Append("and owner_truck_details.truck_id not in (select distinct truck_id from load_order_enquiry_quotation where load_order_enquiry_quotation.owner_id = @ownid ");
            sb.Append("and cast(load_order_enquiry_quotation.status as int) >= 1 and cast(load_order_enquiry_quotation.status as int) not in (45,98,99) and truck_id is not null) ");
            sb.Append("and truck_mst.body_type = @bodytp ");

            SqlParameter pr1 = new SqlParameter();
            pr1.DbType = DbType.String;
            pr1.ParameterName = "@ownid";
            pr1.Value = ownerid;

            SqlParameter pr2 = new SqlParameter();
            pr2.DbType = DbType.String;
            pr2.ParameterName = "@bodytp";
            pr2.Value = bodytype;


            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            cmd.Parameters.Add(pr1);
            cmd.Parameters.Add(pr2);

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
        public string GetAllDriversByOwner(string ownerid)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            try
            {
                sb.Clear();
                //sb.Append("select driver_mst.*,driver_license_detail.License_no from owner_driver_details inner join driver_mst on owner_driver_details.driver_id = driver_mst.driver_id ");
                //sb.Append("left join driver_license_detail on driver_mst.driver_id  = driver_license_detail.driver_id ");
                //sb.Append("where owner_driver_details.owner_id = @ownid and owner_driver_details.active_flag = 'Y' ");

                sb.Append("Select driver_mst.*,driver_license_detail.License_no from owner_driver_details inner join driver_mst on owner_driver_details.driver_id = driver_mst.driver_id ");
                sb.Append("left join driver_license_detail on driver_mst.driver_id  = driver_license_detail.driver_id ");
                sb.Append("Where owner_driver_details.owner_id = @ownid and owner_driver_details.active_flag = 'Y' ");
                sb.Append("and owner_driver_details.driver_id not in (select distinct driver_id from load_order_enquiry_quotation where load_order_enquiry_quotation.owner_id = @ownid ");
                sb.Append("and cast(load_order_enquiry_quotation.status as int) >= 1 and cast(load_order_enquiry_quotation.status as int) not in (45,98,99) and driver_id is not null) ");

                SqlParameter pr1 = new SqlParameter();
                pr1.DbType = DbType.String;
                pr1.ParameterName = "@ownid";
                pr1.Value = ownerid;


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
                    return BLGeneralUtil.return_ajax_string("0", "No Drivers found for selected owner /  All drivers are busy ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + " = " + ex.StackTrace);
                return "0";
            }

        }

        [HttpGet]
        public string GetPendingInquiryForTransporterById(String LoadInquiryNo)
        {
            try
            {
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sb.Clear();
                sb.Append("SELECT load_enquiry_transporter_notification.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                sb.Append("(CASE WHEN [status] = '01' THEN 'Pending For Acceptance' ELSE (CASE WHEN [status] = '02' THEN 'Accepted By Transporter' ELSE 'NO STATUS' END)END) AS [status] ");
                sb.Append("FROM load_enquiry_transporter_notification ");
                sb.Append("left join truck_body_mst  on load_enquiry_transporter_notification.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                sb.Append("left join material_types on load_enquiry_transporter_notification.load_inquiry_material_type = material_types.material_type_code ");
                sb.Append("left join pack_types on load_enquiry_transporter_notification.load_inquiry_packing = pack_types.pack_code ");
                //sb.Append("Where (truck_id is null or truck_id = '') and (owner_id is null or owner_id = '') and load_enquiry_transporter_notification.active_flag = 'Y' ");
                sb.Append("Where (truck_id is null or truck_id = '') and load_enquiry_transporter_notification.active_flag = 'Y' ");
                sb.Append(" AND load_inquiry_no = @InqId ");

                SqlParameter pr1 = new SqlParameter();
                pr1.DbType = DbType.String;
                pr1.ParameterName = "@InqId";
                pr1.Value = LoadInquiryNo;

                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                con.Open();
                cmd.Parameters.Add(pr1);
                cmd.ExecuteNonQuery();


                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);

                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return "0";
            }
        }

        [HttpGet]
        public string GetAllInquiryForTransporter(string ownerid, string statid)
        {
            try
            {
                DataSet ds = new DataSet(); DataTable dt_fresh_quote = new DataTable();
                DataTable dt_inquiries = new DataTable();
                if (statid == "-01")
                {
                    #region query to get count of fresh inquires for Transporter app

                    sb.Clear();

                    #region commented Query not working
                    //sb.Append("SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '-01' THEN 'Generating Quote' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Pending for Shipper Acceptance' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '98' THEN 'Quote Not Selected' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '99' THEN 'Quote Rejected'");
                    //sb.Append("ELSE 'NO STATUS' END)END)END)END) AS [status] ");
                    //sb.Append("FROM load_order_enquiry_quotation ");
                    //sb.Append("left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                    //sb.Append("inner join truck_mst on truck_mst.body_type = truck_body_mst.truck_body_id ");
                    //sb.Append("inner join owner_truck_details on owner_truck_details.truck_id = truck_mst.truck_id ");
                    //sb.Append("left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                    //sb.Append("left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                    //sb.Append(" where (load_order_enquiry_quotation.owner_id = '-9999' and status = '-01') and load_inquiry_no not in ");
                    //sb.Append(" (select distinct load_inquiry_no from load_order_enquiry_quotation where owner_id = @ownerid and load_order_enquiry_quotation.status <> '-01') ");
                    //sb.Append(" Order by cast(load_order_enquiry_quotation.status as int), load_order_enquiry_quotation.load_inquiry_date ");
                    #endregion

                    sb.Append(" SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '-01' THEN 'Generating Quote' ELSE  ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Pending for Shipper Acceptance' ELSE ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '98' THEN 'Quote Not Selected' ELSE ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '99' THEN 'Quote Rejected' ");
                    sb.Append(" ELSE 'NO STATUS' END)END)END)END) AS [status]  FROM load_order_enquiry_quotation ");
                    sb.Append(" left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                    sb.Append(" left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                    sb.Append(" left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                    sb.Append(" Where (load_order_enquiry_quotation.owner_id = '-9999' and status = '-01') and load_inquiry_no not in ");
                    sb.Append(" (select distinct load_inquiry_no from load_order_enquiry_quotation where owner_id = @ownerid and load_order_enquiry_quotation.status <> '-01') ");
                    sb.Append(" and load_order_enquiry_quotation.load_inquiry_truck_type in (Select distinct body_type from truck_mst inner join owner_truck_details ");
                    sb.Append(" on owner_truck_details.truck_id  = truck_mst.truck_id and owner_truck_details.active_flag = 'Y' and owner_id = @ownerid )");
                    sb.Append(" Order by load_order_enquiry_quotation.load_inquiry_no desc, cast(load_order_enquiry_quotation.status as int)  ");

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownerid", DbType.String, ownerid));

                    DBDataAdpterObject.TableMappings.Clear();
                    DBDataAdpterObject.TableMappings.Add("Table", "dt_fresh");
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                    DBDataAdpterObject.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            dt_fresh_quote = ds.Tables[0];
                            ValidateNewQuotes(ref dt_fresh_quote, ownerid);
                        }
                    }
                    #endregion
                }
                else
                {
                    sb.Clear();

                    #region commented query not working
                    //sb.Append("SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '-01' THEN 'Generating Quote' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Pending for Shipper Acceptance' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '98' THEN 'Quote Not Selected' ELSE ");
                    //sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '99' THEN 'Quote Rejected'");
                    //sb.Append("ELSE 'NO STATUS' END)END)END)END) AS [status] ");
                    //sb.Append("FROM load_order_enquiry_quotation ");
                    //sb.Append("left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                    //sb.Append("inner join truck_mst on truck_mst.body_type = truck_body_mst.truck_body_id ");
                    //sb.Append("inner join owner_truck_details on owner_truck_details.truck_id = truck_mst.truck_id ");
                    //sb.Append("left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                    //sb.Append("left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                    //sb.Append(" Where (load_order_enquiry_quotation.owner_id = '-9999' and status = '-01') and load_inquiry_no not in ");
                    //sb.Append(" (select distinct load_inquiry_no from load_order_enquiry_quotation where owner_id = @ownerid and load_order_enquiry_quotation.status <> '-01') ");
                    //sb.Append(" Order by cast(load_order_enquiry_quotation.status as int), load_order_enquiry_quotation.load_inquiry_date ");
                    #endregion

                    sb.Append(" SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '-01' THEN 'Generating Quote' ELSE  ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Pending for Shipper Acceptance' ELSE ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '98' THEN 'Quote Not Selected' ELSE ");
                    sb.Append(" (CASE WHEN load_order_enquiry_quotation.[status] = '99' THEN 'Quote Rejected' ");
                    sb.Append(" ELSE 'NO STATUS' END)END)END)END) AS [status]  FROM load_order_enquiry_quotation ");
                    sb.Append(" left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                    sb.Append(" left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                    sb.Append(" left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                    sb.Append(" Where (load_order_enquiry_quotation.owner_id = '-9999' and status = '-01') and load_inquiry_no not in ");
                    sb.Append(" (select distinct load_inquiry_no from load_order_enquiry_quotation where owner_id = @ownerid and load_order_enquiry_quotation.status <> '-01') ");
                    sb.Append(" and load_order_enquiry_quotation.load_inquiry_truck_type in (Select distinct body_type from truck_mst inner join owner_truck_details ");
                    sb.Append(" on owner_truck_details.truck_id  = truck_mst.truck_id and owner_truck_details.active_flag = 'Y' and owner_id = @ownerid )");
                    sb.Append(" Order by load_order_enquiry_quotation.load_inquiry_no desc, cast(load_order_enquiry_quotation.status as int)  ");

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownerid", DbType.String, ownerid));

                    DBDataAdpterObject.TableMappings.Clear();
                    DBDataAdpterObject.TableMappings.Add("Table", "dt_fresh");
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                    DBDataAdpterObject.Fill(ds);

                    #region comment if no quote shown
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            dt_fresh_quote = ds.Tables[0];
                            ValidateNewQuotes(ref dt_fresh_quote, ownerid);
                        }
                    }
                    #endregion

                    // 01 = quote submitted , 02 = quote accepted by admin, 03 = shipper accepted , 98 = quote note selected , 99 = quote rejected , 35 = UN
                    sb.Clear();
                    sb.Append("SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '-01' THEN 'Generating Quote' ELSE ");
                    // sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Pending for Shipper Acceptance' ELSE ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '01' THEN 'Quote Submitted' ELSE ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '02' THEN 'Forwarded to Shipper' ELSE ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '03' THEN 'Quote Accepted - Awaiting Driver Allocation' ELSE ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '98' THEN 'Quote Not Selected' ELSE ");
                    sb.Append("(CASE WHEN load_order_enquiry_quotation.[status] = '99' THEN 'Quote Rejected'");
                    sb.Append("ELSE 'NO STATUS' END)END)END)END)END)END) AS [status] ");
                    sb.Append("FROM load_order_enquiry_quotation ");
                    sb.Append("left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                    sb.Append("left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                    sb.Append("left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                    sb.Append("left join reason_master on load_order_enquiry_quotation.reason_code = reason_master.reason_code ");
                    sb.Append(" where owner_id = @ownerid and status in ('01','02','03','98','99') ");
                    sb.Append(" and load_inquiry_no not in (Select distinct load_inquiry_no from load_enquiry_transporter_notification where owner_id = @ownerid ) ");
                    sb.Append(" Order by cast(load_order_enquiry_quotation.status as int), load_order_enquiry_quotation.load_inquiry_date ");

                    DBDataAdpterObject.TableMappings.Clear();
                    DBDataAdpterObject.TableMappings.Add("Table", "dt_quote");
                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownerid", DbType.String, ownerid));
                    DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                    DBDataAdpterObject.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (dt_fresh_quote != null && dt_fresh_quote.Rows.Count > 0)
                        {
                            dt_inquiries = dt_fresh_quote;
                        }
                        if (ds.Tables.Count > 1)
                        {
                            if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                            {
                                if (dt_inquiries != null && dt_inquiries.Rows.Count > 0)
                                    dt_inquiries.Merge(ds.Tables[1]);
                                else
                                    dt_inquiries = ds.Tables[1];
                            }
                        }
                    }
                }

                DBConnection.Close();
                if (dt_inquiries != null && dt_inquiries.Rows.Count > 0)
                    return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_inquiries));
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data found . . . ");
            }
            catch (Exception ex)
            {
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        private void ValidateNewQuotes(ref DataTable dt_new, string ownerid)
        {

            // This method get all trucks of selected owner and also gets status of trucks whether it is occupied/in transit 

            DataSet ds = new DataSet();
            DataTable dt_owner_truck = new DataTable();
            string available_truck_type = "";

            sb.Clear();

            //sb.Append(" Select COUNT(truck_mst.truck_id) as Total_trucks,truck_mst.body_type,isnull(Occupied_trucks.occupied_trucks,0) as occupied_trucks,Occupied_trucks.load_inquiry_truck_type, ");
            //sb.Append(" (CASE WHEN (COUNT(truck_mst.truck_id) - Occupied_trucks.occupied_trucks)<=0 THEN 0 ELSE (COUNT(truck_mst.truck_id) - isnull(Occupied_trucks.occupied_trucks,0)) END) as available_trucks  ");
            //sb.Append(" from owner_truck_details inner join truck_mst on owner_truck_details.truck_id = truck_mst.truck_id   ");
            //sb.Append(" left join (Select isnull(COUNT(*),0) as occupied_trucks,load_inquiry_truck_type from load_order_enquiry_quotation where owner_id= @ownid ");
            //sb.Append(" and cast(status as int) >= 1 and cast(status as int) not in (25,98,99) group by load_inquiry_truck_type) as Occupied_trucks on ");
            //sb.Append(" Occupied_trucks.load_inquiry_truck_type = truck_mst.body_type ");
            //sb.Append(" where owner_id = @ownid and owner_truck_details.active_flag = 'Y' group by body_type,Occupied_trucks.occupied_trucks,Occupied_trucks.occupied_trucks,Occupied_trucks.load_inquiry_truck_type ");

            sb.Append(" Select COUNT(truck_mst.truck_id) as Total_trucks,truck_mst.body_type,isnull(Occupied_trucks.occupied_trucks,0) as occupied_trucks,Occupied_trucks.load_inquiry_truck_type,inquiries.Total_inquiries, ");
            sb.Append(" (CASE WHEN (COUNT(truck_mst.truck_id) - Occupied_trucks.occupied_trucks)<=0 THEN 0 ELSE (COUNT(truck_mst.truck_id) - isnull(Occupied_trucks.occupied_trucks,0)) END) as available_trucks   ");
            sb.Append(" from owner_truck_details inner join truck_mst on owner_truck_details.truck_id = truck_mst.truck_id  ");
            sb.Append(" left join (Select isnull(COUNT(*),0) as occupied_trucks,load_inquiry_truck_type from load_order_enquiry_quotation where owner_id= @ownid  ");
            sb.Append(" and cast(status as int) >= 1 and cast(status as int) not in (45,98,99) group by load_inquiry_truck_type) as Occupied_trucks on  ");
            sb.Append(" Occupied_trucks.load_inquiry_truck_type = truck_mst.body_type  ");
            sb.Append(" left join (Select load_inquiry_truck_type,COUNT(*) as Total_inquiries from load_order_enquiry_quotation where status='-01' and owner_id = '-9999' group by load_inquiry_truck_type)   as inquiries");
            sb.Append(" on inquiries.load_inquiry_truck_type = truck_mst.body_type ");
            sb.Append(" where owner_id = @ownid and owner_truck_details.active_flag = 'Y' ");
            sb.Append(" group by body_type,Occupied_trucks.occupied_trucks,Occupied_trucks.occupied_trucks,Occupied_trucks.load_inquiry_truck_type,inquiries.Total_inquiries ");

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownid", DbType.String, ownerid));
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
            DBDataAdpterObject.Fill(ds);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    dt_owner_truck = ds.Tables[0];
                }
            }

            int available_trucks = 0;
            int inquiries = 0;
            DataRow[] dr;
            if (dt_owner_truck != null && dt_owner_truck.Rows.Count > 0)
            {
                for (int i = 0; i < dt_owner_truck.Rows.Count; i++)
                {
                    available_truck_type = dt_owner_truck.Rows[i]["body_type"].ToString();
                    dr = dt_new.Select("load_inquiry_truck_type = '" + available_truck_type + "'");
                    if (dr != null)
                    {
                        available_trucks = Convert.ToInt16(dt_owner_truck.Rows[i]["available_trucks"].ToString());
                        inquiries = Convert.ToInt16(dr.Length);
                        //excess_inquiries = inquiries - available_trucks;
                        //if (excess_inquiries > 0)
                        //{
                        //    for (int j = 0; j < excess_inquiries; j++)
                        //    {
                        //        if (available_truck_type == dt_new.Rows[j]["load_inquiry_truck_type"].ToString())
                        //        {
                        //            dt_new.Rows.RemoveAt(j);
                        //        }
                        //    }
                        //}
                        if (available_trucks == 0)
                        {
                            for (int j = 0; j < inquiries; j++)
                            {
                                if (available_truck_type == dt_new.Rows[j]["load_inquiry_truck_type"].ToString())
                                {
                                    dt_new.Rows.RemoveAt(j);
                                }
                            }
                        }

                    }
                }
            }
        }

        [HttpGet]
        public string GetPendingInquiryForTransporter(string ownerid)
        {
            try
            {
                //SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                DataSet ds = new DataSet(); DataTable dt_inquiries = new DataTable();
                sb.Clear();
                sb.Append("SELECT load_enquiry_transporter_notification.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                sb.Append("(CASE WHEN [status] = '01' THEN 'Awaiting Driver Allocation' ELSE (CASE WHEN [status] = '02' THEN 'Upcoming' ELSE 'NO STATUS' END)END) AS [status] ");
                sb.Append("FROM load_enquiry_transporter_notification ");
                sb.Append("left join truck_body_mst  on load_enquiry_transporter_notification.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                sb.Append("left join material_types on load_enquiry_transporter_notification.load_inquiry_material_type = material_types.material_type_code ");
                sb.Append("left join pack_types on load_enquiry_transporter_notification.load_inquiry_packing = pack_types.pack_code ");
                //sb.Append("Where (truck_id is null or truck_id = '') and (owner_id is null or owner_id = '') and load_enquiry_transporter_notification.active_flag = 'Y'  ");
                sb.Append("Where (truck_id is null or truck_id = '') and load_enquiry_transporter_notification.active_flag = 'Y'  ");
                sb.Append(" and load_enquiry_transporter_notification.owner_id = @ownid ");
                sb.Append("Order by load_enquiry_transporter_notification.load_inquiry_no desc");

                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("ownid", DbType.String, ownerid));
                DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();
                DBDataAdpterObject.Fill(ds);

                if (ds != null && ds.Tables.Count > 0)
                    dt_inquiries = ds.Tables[0];

                if (dt_inquiries != null && dt_inquiries.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt_inquiries)));
                    //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return BLGeneralUtil.return_ajax_string("0", "No Data Found ");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
        }

        [HttpGet]
        public string GetAllPendingQuoteForTransporter(string statid)
        {
            // This method will get all pending quote for which intital quote is generated and awaiting transporters acceptance 
            // Status = 02, Quote Generated, Awaiting Transporters Acceptance
            try
            {
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sb.Clear();

                #region Below query
                sb.Append("SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                sb.Append("(CASE WHEN [status] = '01' THEN 'Generating Quote' ELSE ");
                sb.Append("(CASE WHEN [status] = '02' THEN 'Awaiting Driver Allocation' ELSE ");
                sb.Append("(CASE WHEN [status] = '99' THEN 'Quote Rejected'");
                sb.Append("ELSE 'NO STATUS' END)END)END) AS [status] ");
                sb.Append("FROM load_order_enquiry_quotation ");
                sb.Append("left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                sb.Append("left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                sb.Append("left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                sb.Append("Where load_order_enquiry_quotation.active_flag = 'Y' and status = '" + statid + "' "); // Status = 01 (Generating Quote)
                if (statid == "01") // Pending load inquiry to submit quote
                {

                }
                sb.Append("Order by load_order_enquiry_quotation.load_inquiry_no desc ");
                #endregion

                //SqlParameter pr1 = new SqlParameter();
                //pr1.DbType = DbType.String;
                //pr1.ParameterName = "@ownid";
                //pr1.Value = ownrid;

                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                //                cmd.Parameters.Add(pr1);

                con.Open();
                cmd.ExecuteNonQuery();

                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);

                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return "0";
            }
        }

        [HttpGet]
        public string GetPendingQuoteForTransporterById(String LoadInquiryNo)
        {
            /* This method retrieves load inquiry with calculated quote */
            try
            {
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sb.Clear();
                sb.Append("SELECT load_order_enquiry_quotation.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                sb.Append("(CASE WHEN [status] = '-01' THEN 'Initial Quote Generated' ELSE (CASE WHEN [status] = '01' THEN 'Pending Acceptance' ELSE 'NO STATUS' END)END) AS [status] ");
                sb.Append("FROM load_order_enquiry_quotation ");
                sb.Append("left join truck_body_mst  on load_order_enquiry_quotation.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                sb.Append("left join material_types on load_order_enquiry_quotation.load_inquiry_material_type = material_types.material_type_code ");
                sb.Append("left join pack_types on load_order_enquiry_quotation.load_inquiry_packing = pack_types.pack_code ");
                sb.Append("Where load_order_enquiry_quotation.active_flag = 'Y' AND load_inquiry_no = @InqId and load_order_enquiry_quotation.status = '-01' and load_order_enquiry_quotation.owner_id = '-9999' ");
                //sb.Append("Where load_order_enquiry_quotation.active_flag = 'Y' AND load_inquiry_no = @InqId and status = '01' ");
                // Status=02 (Generating Quote)

                SqlParameter pr1 = new SqlParameter();
                pr1.DbType = DbType.String;
                pr1.ParameterName = "@InqId";
                pr1.Value = LoadInquiryNo;



                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                con.Open();
                cmd.Parameters.Add(pr1);
                cmd.ExecuteNonQuery();


                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);

                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return "0";
            }
        }

        [HttpGet]
        public string GetAcceptedOrdersByShipperForOwners(string ownerid)
        {

            try
            {
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sb.Clear();
                sb.Append("SELECT load_enquiry_transporter_notification.*,truck_body_mst.truck_body_desc,material_types.material_type_description,pack_types.pack_type, ");
                sb.Append("(CASE WHEN [status] = '10' THEN 'Accepted By Shipper, Start Moving' ELSE 'NO STATUS' END) AS [status] ");
                sb.Append("FROM load_enquiry_transporter_notification ");
                sb.Append("left join truck_body_mst  on load_enquiry_transporter_notification.load_inquiry_truck_type = truck_body_mst.truck_body_id ");
                sb.Append("left join material_types on load_enquiry_transporter_notification.load_inquiry_material_type = material_types.material_type_code ");
                sb.Append("left join pack_types on load_enquiry_transporter_notification.load_inquiry_packing = pack_types.pack_code ");
                sb.Append("left join pack_types on load_enquiry_transporter_notification.load_inquiry_packing = pack_types.pack_code ");
                sb.Append("Where owner_id = " + ownerid + " and load_enquiry_transporter_notification.active_flag = 'Y'  ");

                SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                con.Open();
                cmd.ExecuteNonQuery();

                DataTable dtOwner = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dtOwner);

                if (dtOwner != null && dtOwner.Rows.Count > 0)
                {
                    return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
                    //return Json(new { Status = 1, Message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    //return "1";
                }
                else
                    return "0";
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return "0";
            }
        }

        #endregion

        // #region Post methods

        // POST api/owner

        public string Post([FromBody]owner ownerDetails)
        {
            //SqlConnection con = new SqlConnection("Data Source=FWSERVER2\\SQL2008R2;Initial Catalog=Trukker;Persist Security Info=True;User ID=sa;Password=m_pradeep2411");
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            string query = "INSERT INTO owner_master(owner_id, reg_date, owner_name, owner_type, no_of_trucks,created_by,created_date,created_host)VALUES ('" + ownerDetails.owner_id + "','" + ownerDetails.reg_date + "','" + ownerDetails.owner_name + "','" + ownerDetails.owner_type + "','" + ownerDetails.no_of_trucks + "','abc','" + ownerDetails.reg_date + "','127.0.0.1')";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            try
            {
                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    return "Pass";
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        //pooja vachhani on 27/11/15
        public string Post_SaveDriverDetails([FromBody]JObject Parameter)
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

        [HttpPost]
        public HttpResponseMessage PostFile()
        {
            HttpResponseMessage result = null;
            try
            {

                var httpRequest = HttpContext.Current.Request;
                Dictionary<string, string> array1 = new Dictionary<string, string>();

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        if (postedFile.FileName.Trim() != "")
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/imageFolder/DrvPhoto/" + postedFile.FileName);
                            postedFile.SaveAs(filePath);

                            docfiles.Add(filePath);
                            array1[file] = "~/imageFolder/DrvPhoto/" + postedFile.FileName;
                        }
                    }
                    ServerLog.Log(httpRequest["truck_id"].ToString() + "____" + array1["reg_doc_copy"] + "____" + array1["vehicle_regno_copy"] + "____" + array1["permit_photo"]);
                    //String str = update_rto_detail_Table(httpRequest["truck_id"].ToString(), array1["reg_doc_copy"], array1["vehicle_regno_copy"], array1["permit_photo"]);

                    String str = update_drv_detail_Table(httpRequest["driver_id"], array1);
                    //result = Request.CreateResponse(HttpStatusCode.Created, httpRequest["driver_id"] + "_" + str);
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
                //ServerLog.Log(ex.InnerException.ToString());
                return null;
            }
            return result;
        }

        public string update_drv_detail_Table(string driverid, Dictionary<string, string> data)
        //public string update_rto_detail_Table(string truck_id, string permit_photo)
        {
            Dictionary<string, string> filePath = new Dictionary<string, string>();
            foreach (var pair in data)
            {
                //var key = pair.Key;
                //var value = pair.Value;
                filePath[pair.Key] = pair.Value;

                // ServerLog.Log(pair.Key + "___" + pair.Value);
            }



            string drvid = "";
            if (driverid != null && driverid != string.Empty)
            {
                drvid = driverid.Replace('"', ' ').Trim();
                drvid = driverid.TrimEnd().TrimStart();
            }

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            DataTable dtdrivermst = drvmst.GetDriverMst(drvid);

            #region Driver Mst Details
            try
            {
                objdriver.driver_mst.Clear();
                DS_Owner_Mst.driver_mstRow drivermstrow = objdriver.driver_mst.Newdriver_mstRow();
                drivermstrow.driver_id = dtdrivermst.Rows[0]["driver_id"].ToString();
                drivermstrow.reg_date = Convert.ToDateTime(dtdrivermst.Rows[0]["reg_date"]);
                drivermstrow.Name = dtdrivermst.Rows[0]["Name"].ToString();
                drivermstrow.age = dtdrivermst.Rows[0]["age"].ToString(); ;
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
                drivermstrow.driver_photo = filePath["driver_photo"];
                drivermstrow.active_flag = dtdrivermst.Rows[0]["active_flag"].ToString();
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
            #endregion

            #region driver identification table
            try
            {
                objdriver.driver_identification_detail.Clear();
                DataTable dtdriverid = drvmst.GetDriverIdentificationData(drvid);
                for (int i = 0; i < dtdriverid.Rows.Count; i++)
                {

                    DS_Owner_Mst.driver_identification_detailRow drvidRow = objdriver.driver_identification_detail.Newdriver_identification_detailRow();
                    drvidRow.driver_id = dtdriverid.Rows[i]["driver_id"].ToString();
                    drvidRow.identification_id = dtdriverid.Rows[i]["identification_id"].ToString();
                    drvidRow.id_no = dtdriverid.Rows[i]["id_no"].ToString();
                    if (drvidRow.identification_id.Trim() == "I001")
                    {
                        if (filePath.ContainsKey("license_photo"))
                            drvidRow.id_path = filePath["license_photo"];       // Driving Licsense Photo Path
                    }
                    else if (drvidRow.identification_id.Trim() == "I002")
                    {
                        if (filePath.ContainsKey("voter_photo"))
                            drvidRow.id_path = filePath["voter_photo"];         // Voter ID Photo Path   
                    }
                    else if (drvidRow.identification_id.Trim() == "I003")
                    {
                        if (filePath.ContainsKey("pan_photo"))
                            drvidRow.id_path = filePath["pan_photo"];           // PAN Card Photo Path
                    }
                    else if (drvidRow.identification_id.Trim() == "I004")
                    {
                        if (filePath.ContainsKey("ration_photo"))
                            drvidRow.id_path = filePath["ration_photo"];        // Ration Card Photo Path
                    }
                    else if (drvidRow.identification_id.Trim() == "I005")
                    {
                        if (filePath.ContainsKey("aadhar_photo"))
                            drvidRow.id_path = filePath["aadhar_photo"];        // Aadhar Photo Path
                    }
                    else if (drvidRow.identification_id.Trim() == "I006")
                    {
                        if (filePath.ContainsKey("npr_photo"))
                            drvidRow.id_path = filePath["npr_photo"];           // Nation population Register Photo path
                    }
                    else if (drvidRow.identification_id.Trim() == "I007")
                    {
                        if (filePath.ContainsKey("passport_photo"))
                            drvidRow.id_path = filePath["passport_photo"];      // Passport Photo path
                    }
                    drvidRow.id_issued_from = dtdriverid.Rows[i]["id_issued_from"].ToString();
                    if (dtdriverid.Rows[i]["id_valid_from"] != null && dtdriverid.Rows[i]["id_valid_from"].ToString() != "")
                        drvidRow.id_valid_from = Convert.ToDateTime(dtdriverid.Rows[i]["id_valid_from"]);
                    if (dtdriverid.Rows[i]["id_valid_upto"] != null && dtdriverid.Rows[i]["id_valid_upto"].ToString() != "")
                        drvidRow.id_valid_upto = Convert.ToDateTime(dtdriverid.Rows[i]["id_valid_upto"]);
                    drvidRow.status = dtdriverid.Rows[i]["status"].ToString();
                    drvidRow.active_flag = dtdriverid.Rows[i]["active_flag"].ToString();
                    drvidRow.created_by = dtdriverid.Rows[i]["created_by"].ToString();
                    drvidRow.created_date = Convert.ToDateTime(dtdriverid.Rows[i]["created_date"].ToString());
                    drvidRow.created_host = dtdriverid.Rows[i]["created_host"].ToString();
                    drvidRow.device_id = dtdriverid.Rows[i]["device_id"].ToString();
                    drvidRow.device_type = dtdriverid.Rows[i]["device_type"].ToString();
                    drvidRow.modified_by = dtdriverid.Rows[i]["created_by"].ToString();
                    drvidRow.modified_date = System.DateTime.UtcNow;
                    drvidRow.modified_host = dtdriverid.Rows[i]["created_host"].ToString();
                    drvidRow.modified_device_id = dtdriverid.Rows[i]["device_id"].ToString();
                    drvidRow.modified_device_type = dtdriverid.Rows[i]["device_type"].ToString();

                    objdriver.driver_identification_detail.Adddriver_identification_detailRow(drvidRow);
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log("Driver identification udpate : = " + ex.Message.ToString() + "-" + driverid + Environment.NewLine + ex.StackTrace);
                return ex.Message.ToString();
            }
            #endregion

            #region Driver License Details
            try
            {
                objdriver.driver_license_detail.Clear();
                if (filePath.ContainsKey("license_photo"))
                {
                    DataTable dtlicense = drvmst.GetDriverLicenseData(drvid);
                    if (dtlicense != null && dtlicense.Rows.Count > 0)
                    {
                        DS_Owner_Mst.driver_license_detailRow DrvLisnRow = objdriver.driver_license_detail.Newdriver_license_detailRow();
                        DrvLisnRow.driver_id = dtlicense.Rows[0]["driver_id"].ToString();
                        DrvLisnRow.License_no = dtlicense.Rows[0]["License_no"].ToString();
                        DrvLisnRow.License_type = dtlicense.Rows[0]["License_type"].ToString();
                        DrvLisnRow.Issued_place = dtlicense.Rows[0]["Issued_place"].ToString();
                        if (dtlicense.Rows[0]["Valid_from"] != null)
                            DrvLisnRow.Valid_from = Convert.ToDateTime(dtlicense.Rows[0]["Valid_from"].ToString());
                        if (dtlicense.Rows[0]["Valid_upto"] != null)
                            DrvLisnRow.Valid_upto = Convert.ToDateTime(dtlicense.Rows[0]["Valid_upto"].ToString());
                        DrvLisnRow.id_path = filePath["license_photo"];
                        DrvLisnRow.status = dtlicense.Rows[0]["status"].ToString();
                        DrvLisnRow.active_flag = dtlicense.Rows[0]["active_flag"].ToString();
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

        public string Post_SaveOwnerDetails([FromBody]JObject Parameter)
        {
            try
            {
                ResponseCls responsecls = new ResponseCls();
                Random rand = new Random();
                Random rand1 = new Random();
                int re = rand.Next(1, 1000000);
                int flag = 0;

                Master ownermst = new Master();
                DS_Owner_Mst objowner = new DS_Owner_Mst();
                DS_Truck_Mst objownertruck = new DS_Truck_Mst();
                BLReturnObject objBLReturnObject = new BLReturnObject();
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

                //List<Table> list = JsonConvert.DeserializeObject<List<Table>>(Parameter);

                var ownermstData = Parameter["owner"].ToObject<owner>();
                var ownerContactDetailData = Parameter["owner_contact_detail"].ToObject<owner_contact_detail>();
                var ownerTruckDetailData = Parameter["owner_truck_detail"].ToObject<owner_truck_details>();
                var ownerDriverData = Parameter["owner_driver"].ToObject<owner_driver_detail>();

                #region owner_mst
                try
                {
                    DS_Owner_Mst.owner_masterRow ownermstRow = objowner.owner_master.Newowner_masterRow();
                    ownermstRow.owner_id = re.ToString();
                    ownermstRow.reg_date = System.DateTime.UtcNow;
                    ownermstRow.owner_name = ownermstData.owner_name;
                    ownermstRow.owner_type = ownermstData.owner_type;
                    ownermstRow.no_of_trucks = ownermstData.no_of_trucks;

                    if (ownermstData.owner_type != "Company")
                        ownermstRow.date_of_birth = Convert.ToDateTime(ownermstData.date_of_birth);

                    ownermstRow.active_flag = ownermstData.active_flag;
                    ownermstRow.created_by = ownermstData.created_by;
                    ownermstRow.created_date = System.DateTime.UtcNow;
                    ownermstRow.created_host = "1111";
                    ownermstRow.device_id = ownermstData.device_id;
                    ownermstRow.device_type = ownermstData.device_type;

                    if (ownermstData.owner_type == "Company")
                    {
                        ownermstRow.contact_person = ownermstData.contact_person;
                        ownermstRow.contact_number = ownermstData.contact_number;
                    }

                    objowner.owner_master.Addowner_masterRow(ownermstRow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
                #endregion

                #region owner_contact_detail
                try
                {
                    DS_Owner_Mst.owner_contact_detailRow ownerContactDetailRow = objowner.owner_contact_detail.Newowner_contact_detailRow();
                    ownerContactDetailRow.owner_id = re.ToString();
                    ownerContactDetailRow.addr_id = 1;
                    ownerContactDetailRow.address = ownerContactDetailData.address;
                    ownerContactDetailRow.city = ownerContactDetailData.city;
                    ownerContactDetailRow.state = ownerContactDetailData.state;
                    ownerContactDetailRow.pincode = ownerContactDetailData.pincode;
                    ownerContactDetailRow.phone_no = ownerContactDetailData.phone_no;
                    ownerContactDetailRow.mobile_no = ownerContactDetailData.mobile_no;
                    ownerContactDetailRow.active_flag = ownerContactDetailData.active_flag;
                    ownerContactDetailRow.created_by = ownerContactDetailData.created_by;
                    ownerContactDetailRow.created_date = System.DateTime.UtcNow;
                    ownerContactDetailRow.created_host = "1111";
                    ownerContactDetailRow.device_id = ownerContactDetailData.device_id;
                    ownerContactDetailRow.device_type = ownerContactDetailData.device_type;
                    objowner.owner_contact_detail.Addowner_contact_detailRow(ownerContactDetailRow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
                #endregion

                #region Owner Truck Details
                try
                {
                    var tempkey = Parameter.Properties().Select(p => p.Name).ToList();
                    if (tempkey.Contains("owner_truck_detail"))
                    {
                        ownerTruckDetailData = Parameter["owner_truck_detail"].ToObject<owner_truck_details>();
                        if (ownerTruckDetailData != null)
                        {
                            try
                            {
                                DS_Truck_Mst.owner_truck_detailsRow owtruckrow = objownertruck.owner_truck_details.Newowner_truck_detailsRow();
                                owtruckrow.owner_id = "111";
                                owtruckrow.truck_id = ownerTruckDetailData.truck_id;
                                owtruckrow.active_flag = ownermstData.active_flag;
                                owtruckrow.created_by = ownermstData.created_by;
                                owtruckrow.created_date = System.DateTime.UtcNow;
                                owtruckrow.created_host = "1111";
                                owtruckrow.device_id = ownermstData.device_id;
                                owtruckrow.device_type = ownermstData.device_type;
                                objownertruck.owner_truck_details.Addowner_truck_detailsRow(owtruckrow);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log("Truck Owner: " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                                responsecls.status = "0";
                                responsecls.Message = ex.Message;
                                return jser.Serialize(responsecls);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
                #endregion

                #region Owner Driver Details
                try
                {
                    var tempkey = Parameter.Properties().Select(p => p.Name).ToList();
                    if (tempkey.Contains("owner_driver"))
                    {
                        ownerDriverData = Parameter["owner_driver"].ToObject<owner_driver_detail>();
                        if (ownerDriverData != null)
                        {
                            try
                            {
                                DS_Owner_Mst.owner_driver_detailsRow owndrvrow = objowner.owner_driver_details.Newowner_driver_detailsRow();
                                owndrvrow.owner_id = "222";
                                owndrvrow.driver_id = ownerDriverData.driver_id;
                                owndrvrow.active_flag = ownermstData.active_flag;
                                owndrvrow.created_by = ownermstData.created_by;
                                owndrvrow.created_date = System.DateTime.UtcNow;
                                owndrvrow.created_host = "1111";
                                owndrvrow.device_id = ownermstData.device_id;
                                owndrvrow.device_type = ownermstData.device_type;
                                objowner.owner_driver_details.Addowner_driver_detailsRow(owndrvrow);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log("Owner Driver: " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                                responsecls.status = "0";
                                responsecls.Message = ex.Message;
                                return jser.Serialize(responsecls);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
                #endregion

                DS_Owner_Mst objOwnerDriver = new DS_Owner_Mst();
                if (ownermstData.owner_type == "OwnerDriver")
                {
                    //saveDriverData(drivermstData, driverMobilesData, driverlanguageData, driverInsuranceData, driverIdentificationData, driverContactDetails, driverPreferedDestinationData);
                    objOwnerDriver = saveDriverData(Parameter);
                    //objOwnerDriver = savedriverlanguage(Parameter);
                }
                else
                    objOwnerDriver = null;

                // Here we will add code to save owner trucks 

                DataSet[] dsall = new DataSet[3];
                dsall[0] = objowner;
                dsall[1] = objOwnerDriver;
                dsall[2] = objownertruck;

                // objBLReturnObject = ownermst.Save_Owner_Mst(objowner, objOwnerDriver);
                objBLReturnObject = ownermst.Save_Owner_Mst(dsall);
                if (objBLReturnObject.ExecutionStatus == 1)
                {
                    flag = 1;

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

                    //return BLGeneralUtil.return_ajax_string("1", objBLReturnObject.ServerMessage);
                    return objBLReturnObject.ServerMessage;
                }
                else
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    return "{\"status\":\"0\"}";
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + " = " + ex.StackTrace);
                //ServerLog.Log("Post_SaveOwnerDetails - " + ex.InnerException.Message.ToString());
                //ServerLog.Log(ex.InnerException.Message.ToString());
                return ex.Message.ToString();
            }

        }

        public string SaveQuickReg([FromBody]JObject Parameter)
        {
            ResponseCls responsecls = new ResponseCls();
            Random rand = new Random();
            Random rand1 = new Random();
            int re = rand.Next(1, 1000000);
            string DocNoOwner = ""; string DocNoDriverID = ""; string TruckDocNo = ""; string DocRtoId = "";
            Document objDocument = new Document(); string message = "";
            bool status;
            Master master = new Master();
            DS_Owner_Mst objowner = new DS_Owner_Mst();
            DS_Truck_Mst objtruck = new DS_Truck_Mst();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

            var ownermst = Parameter["owner"].ToObject<owner>();
            var drivermst = Parameter["driver"].ToObject<driver_mst>();
            var truckmst = Parameter["truck"].ToObject<truck>();
            truckController trc = new truckController();

            if (truckmst != null)
            {
                if (truckmst.vehicle_reg_no == "" || truckmst.vehicle_reg_no == String.Empty)
                {
                    return BLGeneralUtil.return_ajax_string("0", "Vehicle Registration ID is not provided ");
                }
                if (trc.GetRTORegID(truckmst.vehicle_reg_no.Trim()))
                {
                    return BLGeneralUtil.return_ajax_string("0", "Vehicle Registration ID " + truckmst.vehicle_reg_no.Trim() + " is already registered");
                }
            }


            #region owner_mst
            if (ownermst != null)
            {
                try
                {
                    DS_Owner_Mst.owner_masterRow ownermstRow = objowner.owner_master.Newowner_masterRow();
                    ownermstRow.owner_id = re.ToString();
                    ownermstRow.reg_date = System.DateTime.UtcNow;
                    ownermstRow.owner_name = ownermst.owner_name;
                    ownermstRow.active_flag = ownermst.active_flag;
                    ownermstRow.created_by = ownermst.created_by;
                    ownermstRow.created_date = System.DateTime.UtcNow;
                    ownermstRow.created_host = "1111";
                    ownermstRow.device_id = ownermst.device_id;
                    ownermstRow.device_type = ownermst.device_type;
                    objowner.owner_master.Addowner_masterRow(ownermstRow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
            }
            #endregion

            #region owner_contact_detail
            if (ownermst != null)
            {
                try
                {
                    DS_Owner_Mst.owner_contact_detailRow ownerContactDetailRow = objowner.owner_contact_detail.Newowner_contact_detailRow();
                    ownerContactDetailRow.owner_id = re.ToString();
                    ownerContactDetailRow.addr_id = 1;
                    ownerContactDetailRow.city = ownermst.city; // Owner HUB 
                    ownerContactDetailRow.phone_no = ownermst.phone_no;
                    ownerContactDetailRow.mobile_no = ownermst.mobile_no;
                    ownerContactDetailRow.active_flag = ownermst.active_flag;
                    ownerContactDetailRow.created_by = ownermst.created_by;
                    ownerContactDetailRow.created_date = System.DateTime.UtcNow;
                    ownerContactDetailRow.created_host = "1111";
                    ownerContactDetailRow.device_id = ownermst.device_id;
                    ownerContactDetailRow.device_type = ownermst.device_type;
                    objowner.owner_contact_detail.Addowner_contact_detailRow(ownerContactDetailRow);

                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                    responsecls.status = "0";
                    responsecls.Message = ex.Message;
                    return jser.Serialize(responsecls);
                }
            }
            #endregion

            #region driver_mst
            if (drivermst != null)
            {
                try
                {
                    DS_Owner_Mst.driver_mstRow objdrvr = objowner.driver_mst.Newdriver_mstRow();
                    objdrvr.driver_id = re.ToString();
                    objdrvr.reg_date = System.DateTime.UtcNow;
                    objdrvr.Name = drivermst.Name;
                    objdrvr.active_flag = ownermst.active_flag;
                    objdrvr.created_by = ownermst.created_by;
                    objdrvr.created_date = System.DateTime.UtcNow;
                    objdrvr.created_host = "1111";
                    objdrvr.device_id = ownermst.device_id;
                    objdrvr.device_type = ownermst.device_type;
                    objowner.driver_mst.Adddriver_mstRow(objdrvr);

                }
                catch (Exception ex)
                {
                    ServerLog.Log("Driver Master - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                    return null;
                }
            }
            #endregion

            #region driver_contact_detail
            if (drivermst != null)
            {
                try
                {
                    DS_Owner_Mst.driver_contact_detailRow objOwnerDriverContact = objowner.driver_contact_detail.Newdriver_contact_detailRow();
                    objOwnerDriverContact.driver_id = re.ToString();
                    objOwnerDriverContact.addr_id = 1;
                    objOwnerDriverContact.phone_no = drivermst.phone_no;
                    objOwnerDriverContact.mobile_no = drivermst.mobile_no;
                    objOwnerDriverContact.active_flag = ownermst.active_flag;
                    objOwnerDriverContact.created_by = ownermst.created_by;
                    objOwnerDriverContact.created_date = System.DateTime.UtcNow;
                    objOwnerDriverContact.created_host = "1111";
                    objOwnerDriverContact.device_id = ownermst.device_id;
                    objOwnerDriverContact.device_type = ownermst.device_type;
                    objowner.driver_contact_detail.Adddriver_contact_detailRow(objOwnerDriverContact);
                }
                catch (Exception ex)
                {
                    ServerLog.Log("Driver Contact Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                    return null;
                }
            }
            #endregion

            #region truck_master
            if (truckmst != null)
            {
                try
                {
                    DS_Truck_Mst.truck_mstRow trukrow = objtruck.truck_mst.Newtruck_mstRow();
                    trukrow.truck_id = re.ToString();
                    trukrow.reg_date = System.DateTime.UtcNow;
                    trukrow.active_flag = ownermst.active_flag;
                    trukrow.created_by = ownermst.created_by;
                    trukrow.created_date = System.DateTime.UtcNow;
                    trukrow.created_host = "1111";
                    trukrow.device_id = ownermst.device_id;
                    trukrow.device_type = ownermst.device_type;
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
            if (truckmst != null)
            {
                try
                {
                    DS_Truck_Mst.truck_rto_registration_detailRow truckRtoRegRow = objtruck.truck_rto_registration_detail.Newtruck_rto_registration_detailRow();
                    truckRtoRegRow.reg_id = re.ToString();
                    truckRtoRegRow.truck_id = re.ToString();
                    truckRtoRegRow.vehicle_reg_no = truckmst.vehicle_reg_no;
                    truckRtoRegRow.active_flag = "Y";
                    truckRtoRegRow.created_by = ownermst.created_by;
                    truckRtoRegRow.created_date = System.DateTime.UtcNow;
                    truckRtoRegRow.created_host = "1111";
                    truckRtoRegRow.device_id = ownermst.device_id;
                    truckRtoRegRow.device_type = ownermst.device_type;
                    objtruck.truck_rto_registration_detail.Addtruck_rto_registration_detailRow(truckRtoRegRow);
                }
                catch (Exception ex)
                {
                    ServerLog.Log("Rto Registration : " + ex.Message.ToString());
                    return ex.Message.ToString();
                }
            }
            #endregion



            DBConnection.Open();
            DBCommand.Transaction = DBConnection.BeginTransaction();

            #region save Owner Master
            objowner.EnforceConstraints = false;
            if (objowner.owner_master != null && objowner.owner_master.Rows.Count > 0)
            {
                message = "";
                status = objDocument.W_GetNextDocumentNo(ref DBCommand, "OM", "", "", ref DocNoOwner, ref message); // New Owner ID 
                if (status == false)
                {
                    ServerLog.Log(message);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                objowner.owner_master[0]["owner_id"] = DocNoOwner;
                objBLReturnObject = master.UpdateTables(objowner.owner_master, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                if (objowner.owner_contact_detail != null && objowner.owner_contact_detail.Rows.Count > 0)
                {
                    objowner.owner_contact_detail[0]["owner_id"] = DocNoOwner;
                    objBLReturnObject = master.UpdateTables(objowner.owner_contact_detail, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }


            }
            #endregion

            #region save Driver Master
            objdriver.EnforceConstraints = false;
            if (objowner.driver_mst != null && objowner.driver_mst.Rows.Count > 0)
            {
                message = "";
                status = objDocument.W_GetNextDocumentNo(ref DBCommand, "DR", "", "", ref DocNoDriverID, ref message); // New Driver ID
                if (status == false)
                {
                    ServerLog.Log(message);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }

                objowner.driver_mst[0]["driver_id"] = DocNoDriverID;
                objBLReturnObject = master.UpdateTables(objowner.driver_mst, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                objowner.driver_contact_detail[0]["driver_id"] = DocNoDriverID;
                objBLReturnObject = master.UpdateTables(objowner.driver_contact_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

            }
            #endregion

            #region Save Truck Mst
            if (objtruck.truck_mst != null && objtruck.truck_mst.Rows.Count > 0)
            {
                message = "";
                status = objDocument.W_GetNextDocumentNo(ref DBCommand, "TK", "", "", ref TruckDocNo, ref message); // Truck Master - Truck_ID 
                if (status == false)
                {
                    ServerLog.Log(message);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }
                objtruck.truck_mst[0]["truck_id"] = TruckDocNo;
                objBLReturnObject = master.UpdateTables(objtruck.truck_mst, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }

                message = "";
                objDocument.W_GetNextDocumentNo(ref DBCommand, "TR", "", "", ref DocRtoId, ref message); // Truck RTO Registration Details - Reg_id
                if (status == false)
                {
                    ServerLog.Log(message);
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", message);
                }
                objtruck.truck_rto_registration_detail[0]["truck_id"] = TruckDocNo;
                objtruck.truck_rto_registration_detail[0]["reg_id"] = DocRtoId;
                objBLReturnObject = master.UpdateTables(objtruck.truck_rto_registration_detail, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus != 1)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }
            }
            #endregion

            #region Save Owner Driver Relation
            if (objowner.driver_mst != null && objowner.driver_mst.Rows.Count > 0)
            {
                if (ownermst.IsOwnerDriver == "Y")
                {
                    try
                    {
                        DS_Owner_Mst.owner_driver_detailsRow owndrvrow = objowner.owner_driver_details.Newowner_driver_detailsRow();
                        owndrvrow.owner_id = DocNoOwner;
                        owndrvrow.driver_id = DocNoDriverID;
                        owndrvrow.active_flag = "Y";
                        owndrvrow.created_by = ownermst.created_by;
                        owndrvrow.created_date = System.DateTime.UtcNow;
                        owndrvrow.created_host = "1111";
                        owndrvrow.device_id = ownermst.device_id;
                        owndrvrow.device_type = ownermst.device_type;
                        objowner.owner_driver_details.Addowner_driver_detailsRow(owndrvrow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Owner Driver Relation: " + ex.Message.ToString());
                        responsecls.status = "0";
                        responsecls.Message = ex.Message;
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return jser.Serialize(responsecls);
                    }
                    objBLReturnObject = master.UpdateTables(objowner.owner_driver_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }
            }
            #endregion

            #region Save Owner Truck Relation
            if (objtruck.truck_mst != null && objtruck.truck_mst.Rows.Count > 0)
            {
                if (objowner.owner_master != null && objowner.owner_master.Rows.Count > 0)
                {
                    try
                    {
                        DS_Truck_Mst.owner_truck_detailsRow owrow = objtruck.owner_truck_details.Newowner_truck_detailsRow();
                        owrow.owner_id = DocNoOwner;
                        owrow.truck_id = TruckDocNo;
                        owrow.active_flag = "Y";
                        owrow.created_by = ownermst.created_by;
                        owrow.created_date = System.DateTime.UtcNow;
                        owrow.created_host = "1111";
                        owrow.device_id = ownermst.device_id;
                        owrow.device_type = ownermst.device_type;
                        objtruck.owner_truck_details.Addowner_truck_detailsRow(owrow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Truck Owner Relation: " + ex.Message.ToString() + ex.StackTrace.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return ex.Message.ToString();
                    }
                    objBLReturnObject = master.UpdateTables(objtruck.owner_truck_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }
            }
            #endregion

            #region Driver Truck Relation
            if (objowner.driver_mst != null && objowner.driver_mst.Rows.Count > 0)
            {
                if (objtruck.truck_mst != null && objtruck.truck_mst.Rows.Count > 0)
                {
                    try
                    {
                        DS_Truck_Mst.driver_truck_detailsRow trkdrvrow = objtruck.driver_truck_details.Newdriver_truck_detailsRow();
                        trkdrvrow.driver_id = DocNoDriverID;
                        trkdrvrow.truck_id = TruckDocNo;
                        trkdrvrow.active_flag = "Y";
                        trkdrvrow.created_by = ownermst.created_by;
                        trkdrvrow.created_date = System.DateTime.UtcNow;
                        trkdrvrow.created_host = "1111";
                        trkdrvrow.device_id = ownermst.device_id;
                        trkdrvrow.device_type = ownermst.device_type;
                        objtruck.driver_truck_details.Adddriver_truck_detailsRow(trkdrvrow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Truck Driver Relation : " + ex.Message.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return ex.Message.ToString();
                    }
                    objBLReturnObject = master.UpdateTables(objtruck.driver_truck_details, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                }

            }
            #endregion

            #region Owner Mandi Data
            if (ownermst != null)
            {
                if (ownermst.mandi_id != "")
                {
                    try
                    {
                        DS_Owner_Mst.owner_hub_mandiRow owndrvrow = objowner.owner_hub_mandi.Newowner_hub_mandiRow();
                        owndrvrow.owner_id = DocNoOwner;
                        owndrvrow.city = ownermst.city;
                        owndrvrow.mandi_id = ownermst.mandi_id;
                        owndrvrow.active_flag = "Y";
                        owndrvrow.created_by = ownermst.created_by;
                        owndrvrow.created_date = System.DateTime.UtcNow;
                        owndrvrow.created_host = "1111";
                        owndrvrow.device_id = ownermst.device_id;
                        owndrvrow.device_type = ownermst.device_type;
                        objowner.owner_hub_mandi.Addowner_hub_mandiRow(owndrvrow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Owner Mandi Hub Error: " + ex.Message.ToString());
                        responsecls.status = "0";
                        responsecls.Message = ex.Message;
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return jser.Serialize(responsecls);
                    }
                    objBLReturnObject = master.UpdateTables(objowner.owner_hub_mandi, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus != 1)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }
            }
            #endregion

            DBCommand.Transaction.Commit();
            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            objBLReturnObject.ExecutionStatus = 1;
            responsecls.status = "1";
            responsecls.ownerid = DocNoOwner;
            responsecls.driverid = DocNoDriverID;
            responsecls.truckid = TruckDocNo;
            responsecls.Message = "Quick Registartion Saved ";
            ServerLog.SuccessLog("Quick Registartion Saved ");
            return jser.Serialize(responsecls);
        }
        //pooja vachhani on 27/11/15
        public DS_Owner_Mst saveDriverData(JObject Parameter)
        {
            var tempkey = Parameter.Properties().Select(p => p.Name).ToList();
            driver_mst drivermstData = new driver_mst();
            driver_mobile_detail driverMobilesData = new driver_mobile_detail();
            driver_language_detail driverlanguagedetail = new driver_language_detail();
            driver_insurance_detail driverInsuranceData = new driver_insurance_detail();
            driver_identification_detail driveridentificationdetail = new driver_identification_detail();
            driver_contact_detail driverContactDetails = new driver_contact_detail();
            driver_prefered_destination driverPreferedDestinationData = new driver_prefered_destination();
            driver_license_detail driverlicensedetail = new driver_license_detail();
            driver_bank_detail bankdetail = new driver_bank_detail();
            owner_driver_detail driverowner = new owner_driver_detail();
            driver_truck_detail drivertruck = new driver_truck_detail();


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
                    drivermstData = Parameter["driver_mst"].ToObject<driver_mst>();
                if (tempkey.Contains("driver_mobile_detail"))
                    driverMobilesData = Parameter["driver_mobile_detail"].ToObject<driver_mobile_detail>();
                if (tempkey.Contains("driver_language_detail"))
                    driverlanguagedetail = Parameter["driver_language_detail"].ToObject<driver_language_detail>();
                if (tempkey.Contains("driver_insurance_detail"))
                    driverInsuranceData = Parameter["driver_insurance_detail"].ToObject<driver_insurance_detail>();
                if (tempkey.Contains("driver_contact_detail"))
                    driverContactDetails = Parameter["driver_contact_detail"].ToObject<driver_contact_detail>();
                //if (tempkey.Contains("driver_identification_detail"))
                //    driveridentificationdetail = Parameter["driver_identification_detail"].ToObject<driver_identification_detail>();
                if (tempkey.Contains("driver_prefered_destination"))
                    driverPreferedDestinationData = Parameter["driver_prefered_destination"].ToObject<driver_prefered_destination>();
                if (tempkey.Contains("driver_license_detail"))
                    driverlicensedetail = Parameter["driver_license_detail"].ToObject<driver_license_detail>();
                if (tempkey.Contains("driver_bank_detail"))
                    bankdetail = Parameter["driver_bank_detail"].ToObject<driver_bank_detail>();
                if (tempkey.Contains("driver_owner"))
                    driverowner = Parameter["driver_owner"].ToObject<owner_driver_detail>();
                if (tempkey.Contains("truck_driver"))
                    drivertruck = Parameter["truck_driver"].ToObject<driver_truck_detail>();



                #region driver_mst
                if (drivermstData != null)
                {
                    try
                    {

                        DS_Owner_Mst.driver_mstRow objOwnerDriver = objownerDriver.driver_mst.Newdriver_mstRow();

                        //objownerDriver.driver_mst = Parameter["driver_mst"].ToObject<DS_Owner_Mst.driver_mstDataTable>();

                        objOwnerDriver.driver_id = re.ToString();
                        objOwnerDriver.reg_date = System.DateTime.UtcNow;
                        objOwnerDriver.Name = drivermstData.Name;
                        objOwnerDriver.age = drivermstData.age;
                        objOwnerDriver.dob = Convert.ToDateTime(drivermstData.dob);
                        objOwnerDriver.qualification = drivermstData.qualification;
                        objOwnerDriver.driver_origin = drivermstData.driver_origin;
                        objOwnerDriver.martial_status = drivermstData.martial_status;
                        objOwnerDriver.no_of_child = drivermstData.no_of_child;
                        objOwnerDriver.health_issues = drivermstData.health_issues;
                        objOwnerDriver.smoking = drivermstData.smoking;
                        objOwnerDriver.alcohol = drivermstData.alcohol;
                        objOwnerDriver.legal_history = drivermstData.legal_history;
                        objOwnerDriver.commercial_experience = drivermstData.commercial_experience;
                        objOwnerDriver.active_flag = drivermstData.active_flag;
                        objOwnerDriver.created_by = drivermstData.created_by;
                        objOwnerDriver.created_date = System.DateTime.UtcNow;
                        objOwnerDriver.created_host = "1111";
                        objOwnerDriver.device_id = drivermstData.device_id;
                        objOwnerDriver.device_type = drivermstData.device_type;
                        objownerDriver.driver_mst.Adddriver_mstRow(objOwnerDriver);

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Master - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver_mobile_detail
                if (driverMobilesData != null)
                {
                    if (Parameter["driver_mobile_detail"] != null)
                    {
                        try
                        {
                            DS_Owner_Mst.driver_mobile_detailRow objOwnerDriverMobile = objownerDriver.driver_mobile_detail.Newdriver_mobile_detailRow();
                            objOwnerDriverMobile.driver_id = re.ToString();
                            objOwnerDriverMobile.phone_model = driverMobilesData.phone_model;
                            objOwnerDriverMobile.current_network = driverMobilesData.current_network;
                            objOwnerDriverMobile.consume_3G_Data = driverMobilesData.consume_3G_Data;
                            objOwnerDriverMobile.active_flag = "Y";
                            objOwnerDriverMobile.created_by = driverMobilesData.created_by;
                            objOwnerDriverMobile.created_date = System.DateTime.UtcNow;
                            objOwnerDriverMobile.created_host = "1111";
                            objOwnerDriverMobile.device_id = driverMobilesData.device_id;
                            objOwnerDriverMobile.device_type = driverMobilesData.device_type;

                            objownerDriver.driver_mobile_detail.Adddriver_mobile_detailRow(objOwnerDriverMobile);

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
                if (driverlanguagedetail != null)
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
                if (driverInsuranceData != null)
                {
                    try
                    {
                        DS_Owner_Mst.driver_insurance_detailRow objOwnerDriverInsurance = objownerDriver.driver_insurance_detail.Newdriver_insurance_detailRow();
                        objOwnerDriverInsurance.driver_id = re.ToString();
                        objOwnerDriverInsurance.insurance_policy_no = driverInsuranceData.insurance_policy_no;
                        objOwnerDriverInsurance.insurance_details = driverInsuranceData.insurance_details;
                        objOwnerDriverInsurance.active_flag = driverInsuranceData.active_flag;
                        objOwnerDriverInsurance.status = driverInsuranceData.status;
                        objOwnerDriverInsurance.created_by = driverInsuranceData.created_by;
                        objOwnerDriverInsurance.created_date = System.DateTime.UtcNow;
                        objOwnerDriverInsurance.created_host = "1111";
                        objOwnerDriverInsurance.device_id = driverInsuranceData.device_id;
                        objOwnerDriverInsurance.device_type = driverInsuranceData.device_type;
                        objownerDriver.driver_insurance_detail.Adddriver_insurance_detailRow(objOwnerDriverInsurance);
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
                    try
                    {
                        var param1 = Parameter["driver_identification_detail"];
                        for (int k = 0; k < param1.Count(); k++)
                        {
                            DS_Owner_Mst.driver_identification_detailRow objidrow = objownerDriver.driver_identification_detail.Newdriver_identification_detailRow();
                            objidrow.driver_id = "1111";
                            objidrow.identification_id = param1[k].SelectToken("identification_id").ToString();
                            objidrow.id_no = param1[k].SelectToken("id_no").ToString();
                            if (param1[k].SelectToken("id_valid_from").ToString() != "")
                                objidrow.id_valid_from = Convert.ToDateTime(param1[k].SelectToken("id_valid_from"));
                            if (param1[k].SelectToken("id_valid_upto").ToString() != "")
                                objidrow.id_valid_upto = Convert.ToDateTime(param1[k].SelectToken("id_valid_upto"));
                            objidrow.active_flag = param1[k].SelectToken("active_flag").ToString();
                            objidrow.status = param1[k].SelectToken("status").ToString();
                            objidrow.created_by = param1[k].SelectToken("created_by").ToString();
                            objidrow.created_date = System.DateTime.UtcNow;
                            objidrow.created_host = "111";
                            objidrow.device_id = param1[k].SelectToken("device_id").ToString();
                            objidrow.device_type = param1[k].SelectToken("device_type").ToString();

                            objownerDriver.driver_identification_detail.Adddriver_identification_detailRow(objidrow);
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Identification Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver_contact_detail
                if (driverContactDetails != null)
                {
                    try
                    {
                        DS_Owner_Mst.driver_contact_detailRow objOwnerDriverContact = objownerDriver.driver_contact_detail.Newdriver_contact_detailRow();
                        objOwnerDriverContact.driver_id = re.ToString();
                        objOwnerDriverContact.addr_id = 1;
                        objOwnerDriverContact.address = driverContactDetails.address;
                        objOwnerDriverContact.city = driverContactDetails.city;
                        objOwnerDriverContact.state = driverContactDetails.state;
                        objOwnerDriverContact.pincode = driverContactDetails.pincode;
                        objOwnerDriverContact.phone_no = driverContactDetails.phone_no;
                        objOwnerDriverContact.mobile_no = driverContactDetails.mobile_no;

                        objOwnerDriverContact.active_flag = driverContactDetails.active_flag;
                        objOwnerDriverContact.created_by = driverContactDetails.created_by;
                        objOwnerDriverContact.created_date = System.DateTime.UtcNow;
                        objOwnerDriverContact.created_host = "1111";
                        objOwnerDriverContact.device_id = driverContactDetails.device_id;
                        objOwnerDriverContact.device_type = driverContactDetails.device_type;

                        objownerDriver.driver_contact_detail.Adddriver_contact_detailRow(objOwnerDriverContact);

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
                    try
                    {
                        DS_Owner_Mst.driver_prefered_destinationRow objOwnerDriverPreDestination = objownerDriver.driver_prefered_destination.Newdriver_prefered_destinationRow();

                        objOwnerDriverPreDestination.driver_id = re.ToString();
                        objOwnerDriverPreDestination.destination_id = driverPreferedDestinationData.destination_id;
                        objOwnerDriverPreDestination.state = driverPreferedDestinationData.state;

                        objOwnerDriverPreDestination.active_flag = driverPreferedDestinationData.active_flag;
                        objOwnerDriverPreDestination.created_by = driverPreferedDestinationData.created_by;
                        objOwnerDriverPreDestination.created_date = System.DateTime.UtcNow;
                        objOwnerDriverPreDestination.created_host = "1111";
                        objOwnerDriverPreDestination.device_id = driverPreferedDestinationData.device_id;
                        objOwnerDriverPreDestination.device_type = driverPreferedDestinationData.device_type;

                        objownerDriver.driver_prefered_destination.Adddriver_prefered_destinationRow(objOwnerDriverPreDestination);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Driver Prefered Destination - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                        return null;
                    }
                }
                #endregion

                #region driver License details
                if (driverlicensedetail != null)
                {
                    if (Parameter["driver_license_detail"] != null)
                    {
                        try
                        {
                            DS_Owner_Mst.driver_license_detailRow objlicenserow = objownerDriver.driver_license_detail.Newdriver_license_detailRow();
                            objlicenserow.driver_id = "1111";
                            objlicenserow.License_no = driverlicensedetail.License_no;
                            if (driverlicensedetail.Valid_from != null)
                                objlicenserow.Valid_from = Convert.ToDateTime(driverlicensedetail.Valid_from);
                            if (driverlicensedetail.Valid_upto != null)
                                objlicenserow.Valid_upto = Convert.ToDateTime(driverlicensedetail.Valid_upto);
                            objlicenserow.active_flag = driverlicensedetail.active_flag;
                            objlicenserow.status = driverlicensedetail.status;
                            objlicenserow.created_by = driverlicensedetail.created_by;
                            objlicenserow.created_date = System.DateTime.UtcNow;
                            objlicenserow.created_host = "111";
                            objlicenserow.device_id = driverlicensedetail.device_id;
                            objlicenserow.device_type = driverlicensedetail.device_type;

                            objownerDriver.driver_license_detail.Adddriver_license_detailRow(objlicenserow);
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
                if (bankdetail != null)
                {
                    if (Parameter["driver_bank_detail"] != null)
                    {
                        try
                        {
                            DS_Owner_Mst.driver_bank_detailRow bankrow = objownerDriver.driver_bank_detail.Newdriver_bank_detailRow();
                            bankrow.driver_id = "1111";
                            bankrow.bank_name = bankdetail.bank_name;
                            bankrow.atm = bankdetail.atm;
                            bankrow.ETransfer = bankdetail.ETransfer;
                            bankrow.active_flag = bankdetail.active_flag;
                            bankrow.created_by = bankdetail.created_by;
                            bankrow.created_date = System.DateTime.UtcNow;
                            bankrow.created_host = "111";
                            bankrow.device_id = bankdetail.device_id;
                            bankrow.device_type = bankdetail.device_type;

                            objownerDriver.driver_bank_detail.Adddriver_bank_detailRow(bankrow);
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
                if (driverowner != null)
                {
                    if (Parameter["driver_owner"] != null)
                    {
                        try
                        {
                            DS_Owner_Mst.owner_driver_detailsRow ownrow = objownerDriver.owner_driver_details.Newowner_driver_detailsRow();
                            ownrow.driver_id = "111";
                            ownrow.owner_id = driverowner.owner_id;
                            ownrow.active_flag = drivermstData.active_flag;
                            ownrow.created_by = drivermstData.created_by;
                            ownrow.created_date = System.DateTime.UtcNow;
                            ownrow.created_host = "111";
                            ownrow.device_id = drivermstData.device_id;
                            ownrow.device_type = drivermstData.device_type;
                            objownerDriver.owner_driver_details.Addowner_driver_detailsRow(ownrow);
                        }
                        catch (Exception ex)
                        {
                            ServerLog.Log("Driver Owner Details - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                            return null;
                        }
                    }
                }
                #endregion

                #region Truck Driver Relation
                if (drivertruck != null)
                {
                    if (Parameter["truck_driver"] != null)
                    {
                        try
                        {
                            DS_Owner_Mst.driver_truck_detailsRow drtrkrow = objownerDriver.driver_truck_details.Newdriver_truck_detailsRow();
                            drtrkrow.driver_id = "111";
                            drtrkrow.truck_id = drivertruck.truck_id;
                            drtrkrow.active_flag = drivermstData.active_flag;
                            drtrkrow.created_by = drivermstData.created_by;
                            drtrkrow.created_date = System.DateTime.UtcNow;
                            drtrkrow.created_host = "111";
                            drtrkrow.device_id = drivermstData.device_id;
                            drtrkrow.device_type = drivermstData.device_type;
                            objownerDriver.driver_truck_details.Adddriver_truck_detailsRow(drtrkrow);
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

        public DS_Owner_Mst savedriverlanguage(JObject parameter)
        {
            Random rand = new Random();
            Random rand1 = new Random();
            int re = rand.Next(1, 1000000);

            Master ownermst = new Master();
            DS_Owner_Mst objownerDriver = new DS_Owner_Mst();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            try
            {
                //var userName = (string)parameter["driver_language_detail"].SelectToken("language_data[0].language_id");
                var param = parameter["driver_language_detail"].SelectToken("language_data");
                bool already = false; string langid = "";
                #region driver_language_detail
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
                        //objOwnerDriverLang.active_flag = parameter["driver_language_detail"].SelectToken("active_flag").ToString();
                        objOwnerDriverLang.created_by = parameter["driver_language_detail"].SelectToken("created_by").ToString();
                        objOwnerDriverLang.created_date = System.DateTime.UtcNow;
                        objOwnerDriverLang.created_host = "1111";
                        objOwnerDriverLang.device_id = parameter["driver_language_detail"].SelectToken("device_id").ToString();
                        objOwnerDriverLang.device_type = parameter["driver_language_detail"].SelectToken("device_type").ToString();

                        objownerDriver.driver_language_detail.Adddriver_language_detailRow(objOwnerDriverLang);
                    }
                }

                #endregion

                var param1 = parameter["driver_identification_detail"];
                for (int k = 0; k < param1.Count(); k++)
                {
                    DS_Owner_Mst.driver_identification_detailRow objidrow = objownerDriver.driver_identification_detail.Newdriver_identification_detailRow();
                    objidrow.driver_id = "1111";
                    objidrow.identification_id = param1[k].SelectToken("identification_id").ToString();
                    objidrow.id_no = param1[k].SelectToken("id_no").ToString();
                    objidrow.id_valid_from = Convert.ToDateTime(param1[k].SelectToken("id_valid_from"));
                    objidrow.id_valid_upto = Convert.ToDateTime(param1[k].SelectToken("id_valid_upto"));
                    objidrow.active_flag = param1[k].SelectToken("active_flag").ToString();
                    objidrow.created_by = param1[k].SelectToken("created_by").ToString();
                    objidrow.created_date = System.DateTime.UtcNow;
                    objidrow.created_host = "111";
                    objidrow.device_id = param1[k].SelectToken("device_id").ToString();
                    objidrow.device_type = param1[k].SelectToken("device_type").ToString();

                    objownerDriver.driver_identification_detail.Adddriver_identification_detailRow(objidrow);
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log("Driver Langauage Detail - " + ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                return null;
            }


            return objownerDriver;
        }
    
        //get all free 
        [HttpGet]
        public string GetAllFreeTruck(string opt)
        {
            // Status 45 = Order Status Completed - By Driver , Status 98 = Quote Not Selected , Status 99 = Quote Rejected By Shipper 
            DataTable dtOwner = new DataTable();
            sb.Clear();
            sb.Append("SELECT distinct truck_mst.truck_id,truck_body_mst.truck_body_desc, ");
            sb.Append("truck_rto_registration_detail.vehicle_reg_no,truck_rto_registration_detail.reg_doc_copy,truck_rto_registration_detail.vehicle_regno_copy, ");
            sb.Append("truck_permit_details.permit_type,truck_permit_details.permit_no,truck_permit_details.valid_from,truck_permit_details.valid_upto,truck_permit_details.permit_photo, ");
            sb.Append("truck_current_position.truck_lat,truck_current_position.truck_lng,truck_mst.load_capacity ");
            sb.Append("from truck_mst ");
            sb.Append("left join truck_make_mst on truck_mst.truck_make_id = truck_make_mst.make_id ");
            sb.Append("left join truck_model_mst on truck_mst.truck_model = truck_model_mst.model_id ");
            sb.Append("left join truck_rto_registration_detail on truck_mst.truck_id = truck_rto_registration_detail.truck_id ");
            sb.Append("left join truck_permit_details on truck_mst.truck_id = truck_permit_details.truck_id  ");
            sb.Append("left join truck_body_mst on truck_mst.body_type = truck_body_mst.truck_body_id  ");
            sb.Append("left join truck_current_position on truck_mst.truck_id = truck_current_position.truck_id  and truck_current_position.active_flag = 'Y' ");


            sb.Append(" where 1=1  ");
            if (opt == "1") // all Free Trucks
            {
                sb.Append(" and truck_mst.truck_id not in ( select distinct truck_id from load_order_enquiry_quotation where cast(load_order_enquiry_quotation.status as int) >= 1 ");
                sb.Append(" and cast(load_order_enquiry_quotation.status as int) not in (02,05,06) and truck_id is not null and load_order_enquiry_quotation.active_flag = 'Y' ) ");
            }
            else if (opt == "2") // all Occupid Trucks
            {
                sb.Append(" and truck_mst.truck_id in ( select distinct truck_id from load_order_enquiry_quotation where cast(load_order_enquiry_quotation.status as int) >= 1 ");
                sb.Append(" and cast(load_order_enquiry_quotation.status as int) not in (02,05,06) and truck_id is not null and load_order_enquiry_quotation.active_flag = 'Y' ) ");
            }
            //sb.Append("and truck_mst.body_type = @bodytp ");

            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = sb.ToString();

            DataSet ds = new DataSet();
            DBDataAdpterObject.Fill(ds);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    dtOwner = ds.Tables[0];
            }

            if (dtOwner != null && dtOwner.Rows.Count > 0)
                return (BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dtOwner)));
            else
                return (BLGeneralUtil.return_ajax_string("0", "Truck details not found "));

        }
    }
}

