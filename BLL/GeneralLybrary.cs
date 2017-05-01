using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using trukkerUAE.XSD;
using BLL.Utilities;
using System.IO;
using System.Web.Script.Serialization;
using System.Data;
using trukkerUAE.Models;


namespace trukkerUAE.BLL
{
    public class GeneralLybrary : ServerBase
    {
        public bool SetAndModifyStatusinAnyTableNew(string tablename, string status, string shipperid, string inquiryno, string ownerid, string truckid, string driverid, ref IDbCommand DBCommand)
        {
            DBCommand.Parameters.Clear();
            string where = "";
            string query = " UPDATE " + tablename +
                     " SET status =   '" + status + "' " + " WHERE ";

            if (shipperid != string.Empty && shipperid != "" && shipperid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@shipperid", DbType.String, shipperid));
                where = " shipper_id= @shipperid ";
            }
            if (inquiryno != string.Empty && inquiryno != "" && inquiryno.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inquiryno", DbType.String, inquiryno));
                if (where != string.Empty)
                    where = where + "AND  load_inquiry_no= @inquiryno ";
                else
                    where = " load_inquiry_no = @inquiryno ";
            }
            if (ownerid != string.Empty && ownerid != "" && ownerid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@ownerid", DbType.String, ownerid));
                if (where != string.Empty)
                    where = where + "AND  owner_id= @ownerid ";
                else
                    where = " owner_id = @ownerid ";
            }
            if (truckid != string.Empty && truckid != "" && truckid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truckid", DbType.String, truckid));
                if (where != string.Empty)
                    where = where + "AND  truck_id= @truckid ";
                else
                    where = " truck_id = @truckid ";
            }
            if (driverid != string.Empty && driverid != "" && driverid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@driverid", DbType.String, driverid));
                if (where != string.Empty)
                    where = where + "AND  driver_id= @driverid ";
                else
                    where = " driver_id = @driverid ";
            }
            query = query + where;
            DBCommand.CommandText = query;
            int row_count = 0;
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            try
            {
                row_count = DBCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
            if (row_count <= 0)
            {
                ServerLog.Log("Status not Updated " + tablename + Environment.NewLine + query);
                return false;
            }
            else
                return true;

        }
        public DataTable GetTableDataCommonMethod(string tablename, string status, string shipperid, string inquiryno, string ownerid, string truckid, string driverid, ref IDbCommand DBCommand)
        {

            DBCommand.Parameters.Clear();
            string where = "";
            string query = "select * from " + tablename + " WHERE ";


            if (shipperid != string.Empty && shipperid != "" && shipperid.TrimEnd() != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@shipperid", DbType.String, shipperid.TrimEnd()));
                where = " shipper_id= @shipperid ";
            }
            if (inquiryno != string.Empty && inquiryno != "" && inquiryno.TrimEnd() != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inquiryno", DbType.String, inquiryno.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  load_inquiry_no= @inquiryno ";
                else
                    where = " load_inquiry_no = @inquiryno ";
            }


            if (ownerid != string.Empty && ownerid != "" && ownerid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@ownerid", DbType.String, ownerid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  owner_id= @ownerid ";
                else
                    where = " owner_id = @ownerid ";
            }

            if (truckid != string.Empty && truckid != "" && truckid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truckid", DbType.String, truckid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  truck_id= @truckid ";
                else
                    where = " truck_id = @truckid ";
            }
            if (driverid != string.Empty && driverid != "" && driverid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@driverid", DbType.String, driverid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  driver_id= @driverid ";
                else
                    where = " driver_id = @driverid ";
            }
            query = query + where;
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DataSet ds = new DataSet();
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.Fill(ds);
            if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return ds.Tables[0];

        }
    }
}