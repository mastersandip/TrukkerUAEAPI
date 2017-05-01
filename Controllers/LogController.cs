using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using trukkerUAE.Models;
using trukkerUAE.XSD;
using BLL.Master;
using BLL.Utilities;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using trukkerUAE.Classes;

namespace trukkerUAE.Controllers
{
    public class LogController : ServerBase
    {
        // GET api/log
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/log/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/log
        public void Post([FromBody]string value)
        {
        }

        // PUT api/log/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/log/5
        public void Delete(int id)
        {
        }

         [HttpGet]
        public string Unlock()
        {
            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                return BLGeneralUtil.return_ajax_string("1", "Done");
            }
            catch (Exception ex)
            {
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
           
        }

        [HttpPost]
        public string SaveAppLog([FromBody]  JObject jobj)
        {
            AppLog jobjdata = new AppLog();
            Document doc = new Document();
            BLReturnObject objBLReturnObject = new BLReturnObject(); Master master = new Master();
            string message = "";
            DS_AppLog ds_app = new DS_AppLog();
            List<AppLog> tload = new List<AppLog>();
            tload = jobj["log"].ToObject<List<AppLog>>();
            DataTable dt_log = new DataTable(); string LogDocID = "";
            dt_log = BLGeneralUtil.ToDataTable(tload);
            try
            {
                if (jobj != null)
                {
                    if (dt_log != null && dt_log.Rows.Count > 0)
                    {
                        DBConnection.Open();
                        DBCommand.Transaction = DBConnection.BeginTransaction();
                        ds_app.EnforceConstraints = false;
                        if (!doc.W_GetNextDocumentNo(ref DBCommand, "APL", "", "", ref LogDocID, ref message)) // New Driver Notification ID
                        {
                            return BLGeneralUtil.return_ajax_string("0", message);
                        }
                        ds_app.AppLog.ImportRow(dt_log.Rows[0]);
                        ds_app.AppLog[0].log_id = LogDocID;
                        ds_app.AppLog[0].log_date = System.DateTime.UtcNow;
                        ds_app.AppLog[0].created_date = System.DateTime.UtcNow;
                        ds_app.AppLog[0].AcceptChanges();
                        ds_app.AppLog[0].SetAdded();
                        ds_app.EnforceConstraints = true;
                        objBLReturnObject = master.UpdateTables(ds_app.AppLog, ref DBCommand);
                        if (objBLReturnObject.ExecutionStatus != 1)
                        {
                            ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                        }
                        DBCommand.Transaction.Commit();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 1;
                        ServerLog.SuccessLog("App Log Entry Saved : " + LogDocID);
                    }
                }
            }
            catch(Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);
            }
            return BLGeneralUtil.return_ajax_string("1", "App Log Entry Saved : " + LogDocID);
        }

        [HttpGet]
        public string GetLogParameter()
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM param_mst where param_code = 'LOGEMAIL'  ";
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
                return BLGeneralUtil.return_ajax_string("0", "No Data Found");
        }

        [HttpGet]
        public string GetParameter(string paramcode, string paramvalue)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "SELECT * FROM param_mst where param_code = @parcode and param_value = @parvalue ";
            DBConnection.Open();
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            DBDataAdpterObject.SelectCommand.CommandText = query1;
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("parcode", DbType.String, paramcode));
            DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("parvalue", DbType.String, paramvalue));
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
                return BLGeneralUtil.return_ajax_string("0", "No Data Found");
        }

    }
}
