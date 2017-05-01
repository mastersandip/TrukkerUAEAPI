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
using System.Data.SqlClient;
using System.Web.Configuration;

namespace BLL.Master
{
    public class Master : ServerBase
    {
        DS_Truck_Mst objtrukmst = new DS_Truck_Mst();
        DS_Owner_Mst objownermst = new DS_Owner_Mst();
        DS_Post_load_enquiry objPostOrder = new DS_Post_load_enquiry();
        Document objDocument = new Document();
        String message = "";
        String DocNo = "";
        JavaScriptSerializer jser = new JavaScriptSerializer();

        #region GET Methods

        //pooja vachhani on 26/11/15
        public DataTable GetRtoDetailTableData(string truck_id)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_rto_registration_detail where truck_id = '" + truck_id + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + ex.StackTrace);
                return null;
            }
        }

        //pooja vachhani on 26/11/15
        public DataTable GetPermitData(string truck_id)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_permit_details where truck_id = '" + truck_id + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetInsuranceDataNew(string truck_id, string insId)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_insurance_detail where truck_id = '" + truck_id + "' and insurance_sr_id='" + insId + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetInsuranceData(string truck_id)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_insurance_detail where truck_id = '" + truck_id + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetMaintenanceData(string truck_id)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_maintenance_detail where truck_id = '" + truck_id + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }
        public DataTable GetMaintenanceDataNew(string truck_id, string maintid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";

                SqlSelect = "select * from truck_maintenance_detail where truck_id = '" + truck_id + "' and maintenance_id='" + maintid + "' and active_flag = 'Y'";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetDriverMst(string driverid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from driver_mst where driver_id = '" + driverid + "' and active_flag = 'Y'";
                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetDriverContactdetails(string driverid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from driver_contact_detail where driver_id = '" + driverid + "' and active_flag = 'Y'";
                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetDriverIdentificationData(string driverid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from driver_identification_detail where driver_id = '" + driverid + "' and active_flag = 'Y'";
                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetDriverLicenseData(string driverid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from driver_license_detail where driver_id = '" + driverid + "' and active_flag = 'Y'";
                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable GetDriverNotification(string loadinqid)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from driver_order_notifications where load_inquiry_no = '" + loadinqid + "' and active_flag = 'Y'";
                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                        return null;
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message + " " + ex.StackTrace);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        public DataTable CalculateExpDate(decimal aprox_kms)
        {
            DataTable dt_res = new DataTable();
            dt_res.Columns.Add("Data");
            dt_res.Columns.Add("Value");
            string[,] temp = new string[3, 3];
            decimal Aproxdays = 0; decimal totalhours = 0;
            totalhours = aprox_kms / 30; // Considered 30 KMS per hour
            Aproxdays = totalhours / 12; // Usually Driver works 12 hour a day
            DataRow dr = dt_res.NewRow();
            dr["Data"] = "Hours";
            dr["Value"] = totalhours.ToString();

            dt_res.Rows.Add(dr);

            dr = null;
            dr["Data"] = "Days";
            dr["Value"] = Aproxdays.ToString();
            dt_res.Rows.Add(dr);

            return dt_res;
        }

        #endregion

        #region post methods
        //pooja vachhani on 25/11/15
        public BLReturnObject Save_Truck_Mst(DataSet ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            String DocRtoId = "";
            String DocPerId = "";


            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                objtrukmst = (DS_Truck_Mst)ds_UploadData;

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (objtrukmst.truck_mst != null && objtrukmst.truck_mst.Rows.Count > 0)
                    objDocument.W_GetNextDocumentNo(ref DBCommand, "TK", "", "", ref DocNo, ref message); // Truck Master - Truck_ID 
                if (objtrukmst.truck_rto_registration_detail != null && objtrukmst.truck_rto_registration_detail.Rows.Count > 0)
                    objDocument.W_GetNextDocumentNo(ref DBCommand, "TR", "", "", ref DocRtoId, ref message); // Truck RTO Registration Details - Reg_id

                DS_Truck_Mst server_trk = new DS_Truck_Mst();

                ResponseCls responsecls = new ResponseCls();

                if (objtrukmst.truck_mst != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_mst.Rows.Count; mprCnt++)
                    {
                        server_trk.truck_mst.ImportRow(objtrukmst.truck_mst.Rows[mprCnt]);
                        server_trk.truck_mst[mprCnt].truck_id = DocNo;

                    }
                }

                if (objtrukmst.truck_rto_registration_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_rto_registration_detail.Rows.Count; mprCnt++)
                    {
                        server_trk.truck_rto_registration_detail.ImportRow(objtrukmst.truck_rto_registration_detail.Rows[mprCnt]);
                        server_trk.truck_rto_registration_detail[mprCnt].reg_id = DocRtoId;
                        server_trk.truck_rto_registration_detail[mprCnt].truck_id = DocNo;
                    }
                }
                if (objtrukmst.truck_permit_details != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_permit_details.Rows.Count; mprCnt++)
                    {
                        objDocument.W_GetNextDocumentNo(ref DBCommand, "TPD", "", "", ref DocPerId, ref message);
                        server_trk.truck_permit_details.ImportRow(objtrukmst.truck_permit_details.Rows[mprCnt]);
                        server_trk.truck_permit_details[mprCnt].permit_reg_id = DocPerId;
                        server_trk.truck_permit_details[mprCnt].truck_id = DocNo;
                    }
                }


                if (objtrukmst.owner_truck_details != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.owner_truck_details.Rows.Count; mprCnt++)
                    {
                        server_trk.owner_truck_details.ImportRow(objtrukmst.owner_truck_details.Rows[mprCnt]);
                        server_trk.owner_truck_details[mprCnt].truck_id = DocNo;
                    }
                }

                if (objtrukmst.truck_insurance_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_insurance_detail.Rows.Count; mprCnt++)
                    {
                        server_trk.truck_insurance_detail.ImportRow(objtrukmst.truck_insurance_detail.Rows[mprCnt]);
                        server_trk.truck_insurance_detail[mprCnt].truck_id = DocNo;
                    }
                }

                if (objtrukmst.driver_truck_details != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.driver_truck_details.Rows.Count; mprCnt++)
                    {
                        server_trk.driver_truck_details.ImportRow(objtrukmst.driver_truck_details.Rows[mprCnt]);
                        server_trk.driver_truck_details[mprCnt].truck_id = DocNo;
                    }
                }

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                //   objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_pkg.package_master, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.truck_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.truck_mst.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.truck_rto_registration_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.truck_rto_registration_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                if (objtrukmst.owner_truck_details != null)
                {
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.owner_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.owner_truck_details.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.truck_insurance_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.truck_insurance_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.truck_permit_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.truck_permit_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.driver_truck_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    responsecls.status = "1";
                    responsecls.ownerid = "";
                    responsecls.driverid = "";
                    responsecls.truckid = DocNo;
                    responsecls.Message = "Data Saved Successfully ";
                    ServerLog.SuccessLog("Truck Master Saved : TrcukId = " + DocNo);
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString());
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = "Fail to Save Details.";
                return objBLReturnObject;
            }
        }

        //pooja vachhani on 25/11/15
        public BLReturnObject Save_Owner_Mst(DataSet[] ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            String DocNoDriverID = "";
            ResponseCls responsecls = new ResponseCls();
            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                objownermst = (DS_Owner_Mst)ds_UploadData[0];

                //pooja vachhani on 27/11/15
                DS_Owner_Mst objownermstDriver = new DS_Owner_Mst();
                objownermstDriver = (DS_Owner_Mst)ds_UploadData[1];

                //if (ds_UploadDriverData != null)
                //{

                //    objownermstDriver = (DS_Owner_Mst)ds_UploadDriverData;
                //}



                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                objDocument.W_GetNextDocumentNo(ref DBCommand, "OM", "", "", ref DocNo, ref message); // New Owner ID 
                //if (ds_UploadDriverData != null)
                if (objownermstDriver != null)
                    objDocument.W_GetNextDocumentNo(ref DBCommand, "DR", "", "", ref DocNoDriverID, ref message); // New Driver ID
                //else if (objownermst.owner_driver_details != null && objownermst.owner_driver_details.Rows.Count > 0)
                //    objDocument.W_GetNextDocumentNo(ref DBCommand, "DR", "", "", ref DocNoDriverID, ref message); // New Driver ID


                DS_Owner_Mst server_trk = new DS_Owner_Mst();
                DS_Owner_Mst server_OwnerDriver = new DS_Owner_Mst();

                for (int mprCnt = 0; mprCnt < objownermst.owner_master.Rows.Count; mprCnt++)
                {
                    server_trk.owner_master.ImportRow(objownermst.owner_master.Rows[mprCnt]);
                    server_trk.owner_master[mprCnt].owner_id = DocNo;
                    server_trk.owner_master[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.owner_contact_detail.Rows.Count; mprCnt++)
                {
                    server_trk.owner_contact_detail.ImportRow(objownermst.owner_contact_detail.Rows[mprCnt]);
                    server_trk.owner_contact_detail[mprCnt].owner_id = DocNo;
                }


                #region OwnerDriver (Save Driver Master for Owner)
                //if (ds_UploadDriverData != null)
                if (objownermstDriver != null)
                {
                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_mst.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_mst.ImportRow(objownermstDriver.driver_mst.Rows[mprCnt]);
                        server_OwnerDriver.driver_mst[mprCnt].driver_id = DocNoDriverID;
                        server_OwnerDriver.driver_mst[mprCnt].owner_id = DocNo;

                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_prefered_destination.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_prefered_destination.ImportRow(objownermstDriver.driver_prefered_destination.Rows[mprCnt]);
                        server_OwnerDriver.driver_prefered_destination[mprCnt].driver_id = DocNoDriverID;

                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_mobile_detail.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_mobile_detail.ImportRow(objownermstDriver.driver_mobile_detail.Rows[mprCnt]);
                        server_OwnerDriver.driver_mobile_detail[mprCnt].driver_id = DocNoDriverID;

                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_language_detail.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_language_detail.ImportRow(objownermstDriver.driver_language_detail.Rows[mprCnt]);
                        server_OwnerDriver.driver_language_detail[mprCnt].driver_id = DocNoDriverID;

                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_insurance_detail.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_insurance_detail.ImportRow(objownermstDriver.driver_insurance_detail.Rows[mprCnt]);
                        server_OwnerDriver.driver_insurance_detail[mprCnt].driver_id = DocNoDriverID;
                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_identification_detail.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_identification_detail.ImportRow(objownermstDriver.driver_identification_detail.Rows[mprCnt]);
                        server_OwnerDriver.driver_identification_detail[mprCnt].driver_id = DocNoDriverID;

                    }

                    for (int mprCnt = 0; mprCnt < objownermstDriver.driver_contact_detail.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_contact_detail.ImportRow(objownermstDriver.driver_contact_detail.Rows[mprCnt]);
                        server_OwnerDriver.driver_contact_detail[mprCnt].driver_id = DocNoDriverID;

                    }
                }
                #endregion

                #region Owner Truck Relation (Called from Truck Master)
                DS_Truck_Mst objOwnerTruck = new DS_Truck_Mst();
                objOwnerTruck = (DS_Truck_Mst)ds_UploadData[2];
                if (objOwnerTruck != null && objOwnerTruck.owner_truck_details.Rows.Count > 0)
                {
                    for (int mprCnt = 0; mprCnt < objOwnerTruck.owner_truck_details.Rows.Count; mprCnt++)
                    {
                        server_trk.owner_truck_details.ImportRow(objOwnerTruck.owner_truck_details.Rows[mprCnt]);
                        server_trk.owner_truck_details[mprCnt].owner_id = DocNo;
                    }
                }
                #endregion

                #region Owner Driver Relation (Called from Driver Master)
                if (objownermst != null && objownermst.owner_driver_details.Rows.Count > 0)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.owner_driver_details.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.owner_driver_details.ImportRow(objownermst.owner_driver_details.Rows[mprCnt]);
                        server_OwnerDriver.owner_driver_details[mprCnt].owner_id = DocNo;
                    }
                }
                #endregion

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;



                //if (ds_UploadDriverData != null)
                if (objownermstDriver != null)
                {
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_mst.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        //objBLReturnObject.ServerMessage = "Fail to Save Details.";
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_contact_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_contact_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_identification_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_identification_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_license_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_license_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_mobile_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_mobile_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_prefered_destination, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_prefered_destination.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_language_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_language_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_insurance_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_insurance_detail.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        responsecls.status = "0";
                        responsecls.Message = objBLReturnObject.ServerMessage;
                        objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                        return objBLReturnObject;
                    }

                }

                //   objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_pkg.package_master, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.owner_master, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.owner_master.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.owner_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.owner_truck_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.owner_driver_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.owner_driver_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.owner_contact_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.owner_contact_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    //objBLReturnObject.ServerMessage = "Data Saved Successfully";

                    responsecls.status = "1";
                    responsecls.ownerid = DocNo;
                    responsecls.driverid = DocNoDriverID;
                    responsecls.truckid = "";
                    responsecls.Message = "Data Saved Successfully ";
                    ServerLog.SuccessLog("Owner Master Saved : OwnerId = " + DocNo + ", DriverId = " + DocNoDriverID);
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);

                    //if(ds_UploadDriverData != null)
                    //    //return jser.Serialize(responsecls);
                    //    //objBLReturnObject.ServerMessage = "{\"Ownerid\":\"" + DocNo + ",driverid:" + DocNoDriverID;
                    //    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //else
                    //    objBLReturnObject.ServerMessage = "{\"Ownerid\":\"" + DocNo + "\"}";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log("Save_Owner_Mst - " + ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                //ServerLog.Log(ex.Message.ToString());
                responsecls.status = "0";
                responsecls.Message = objBLReturnObject.ServerMessage;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
        }

        public BLReturnObject SaveOwnerTruckDriverRelation(DataSet[] ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            ResponseCls responsecls = new ResponseCls();
            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                objtrukmst = (DS_Truck_Mst)ds_UploadData[0];
                objownermst = (DS_Owner_Mst)ds_UploadData[1];

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();


                DS_Owner_Mst server_trk = new DS_Owner_Mst();
                DS_Owner_Mst server_OwnerDriver = new DS_Owner_Mst();


                #region Truck Owner Relation
                if (objtrukmst != null && objtrukmst.owner_truck_details.Rows.Count > 0)
                {
                    responsecls.ownerid = objtrukmst.owner_truck_details.Rows[0]["owner_id"].ToString();
                    responsecls.truckid = objtrukmst.owner_truck_details.Rows[0]["truck_id"].ToString();
                    for (int mprCnt = 0; mprCnt < objtrukmst.owner_truck_details.Rows.Count; mprCnt++)
                        server_trk.owner_truck_details.ImportRow(objtrukmst.owner_truck_details.Rows[mprCnt]);
                }
                #endregion

                #region Truck Driver Relation
                if (objtrukmst != null && objtrukmst.driver_truck_details.Rows.Count > 0)
                {
                    responsecls.driverid = objtrukmst.driver_truck_details.Rows[0]["driver_id"].ToString();
                    responsecls.truckid = objtrukmst.driver_truck_details.Rows[0]["truck_id"].ToString();
                    for (int mprCnt = 0; mprCnt < objtrukmst.driver_truck_details.Rows.Count; mprCnt++)
                        server_trk.driver_truck_details.ImportRow(objtrukmst.driver_truck_details.Rows[mprCnt]);
                }
                #endregion

                #region Owner Driver Relation
                if (objownermst != null && objownermst.owner_driver_details.Rows.Count > 0)
                {
                    responsecls.ownerid = objownermst.owner_driver_details.Rows[0]["owner_id"].ToString();
                    responsecls.driverid = objownermst.owner_driver_details.Rows[0]["driver_id"].ToString();
                    for (int mprCnt = 0; mprCnt < objownermst.owner_driver_details.Rows.Count; mprCnt++)
                        server_OwnerDriver.owner_driver_details.ImportRow(objownermst.owner_driver_details.Rows[mprCnt]);
                }
                #endregion

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.owner_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.owner_truck_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.driver_truck_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.owner_driver_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.owner_driver_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    //objBLReturnObject.ServerMessage = "Data Saved Successfully";

                    responsecls.status = "1";
                    responsecls.Message = "Data Saved Successfully ";
                    ServerLog.SuccessLog("Owner, Truck, Driver Relation Saved ");
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);

                    //if(ds_UploadDriverData != null)
                    //    //return jser.Serialize(responsecls);
                    //    //objBLReturnObject.ServerMessage = "{\"Ownerid\":\"" + DocNo + ",driverid:" + DocNoDriverID;
                    //    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //else
                    //    objBLReturnObject.ServerMessage = "{\"Ownerid\":\"" + DocNo + "\"}";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log("Save Owner Truck Driver Relation - " + ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                //ServerLog.Log(ex.Message.ToString());
                responsecls.status = "0";
                responsecls.Message = objBLReturnObject.ServerMessage;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
        }

        public BLReturnObject SaveQuickRegistration(DataSet[] ds_uploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            ResponseCls responsecls = new ResponseCls();


            try
            {
                if (ds_uploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                objtrukmst = (DS_Truck_Mst)ds_uploadData[0];
                objownermst = (DS_Owner_Mst)ds_uploadData[1];

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DS_Owner_Mst server_trk = new DS_Owner_Mst();


            }
            catch (Exception ex)
            {
                ServerLog.Log("Quick Registration Failed - " + ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                responsecls.status = "0";
                responsecls.Message = objBLReturnObject.ServerMessage;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
            return objBLReturnObject;
        }

        //pooja vachhani on 23/11/15
        public BLReturnObject SaveLoadInq(DataSet ds_UploadData, ref IDbCommand DBCommand)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                objPostOrder = (DS_Post_load_enquiry)ds_UploadData;
                objDocument.W_GetNextDocumentNo(ref DBCommand, "PO", "", "", ref DocNo, ref message);

                if (DocNo == null || DocNo.ToString() == "")
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log("Error Generating Load Inquiry ID");
                    objBLReturnObject.ServerMessage = "Error Generating Load Inquiry ID";
                    return objBLReturnObject;
                }
                DS_Post_load_enquiry server_trk = new DS_Post_load_enquiry();
                server_trk.EnforceConstraints = false;
                for (int mprCnt = 0; mprCnt < objPostOrder.post_load_inquiry.Rows.Count; mprCnt++)
                {
                    server_trk.post_load_inquiry.ImportRow(objPostOrder.post_load_inquiry.Rows[mprCnt]);
                    server_trk.post_load_inquiry[mprCnt].load_inquiry_no = DocNo;
                }
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_trk.post_load_inquiry, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_trk.post_load_inquiry.Rows.Count)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = "Fail to Save Details.";
                    return objBLReturnObject;
                }
                else
                {
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Post Load Inquiry Saved : Inquiry Id = " + DocNo);
                    objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    objBLReturnObject.returnsomething = DocNo.ToString();
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = "Fail to Save Details.";
                return objBLReturnObject;
            }
        }

        //pooja vachhani on 27/11/15
        public BLReturnObject Save_Driver_Mst(DataSet ds_UploadDriverData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();
            String DocNoDriverID = "";
            ServerLog.Log("Save_Driver_Mst Called");
            ResponseCls responsecls = new ResponseCls();
            try
            {
                if (ds_UploadDriverData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                //pooja vachhani on 27/11/15
                DS_Owner_Mst objownermst = new DS_Owner_Mst();
                if (ds_UploadDriverData != null)
                {

                    objownermst = (DS_Owner_Mst)ds_UploadDriverData;
                }

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (ds_UploadDriverData.Tables["user_mst"] != null)
                {
                    if (ds_UploadDriverData.Tables["user_mst"].Rows[0]["unique_id"].ToString() == "")
                        objDocument.W_GetNextDocumentNo(ref DBCommand, "DR", "", "", ref DocNoDriverID, ref message);
                    else
                        DocNoDriverID = objownermst.user_mst[0].unique_id;
                }
                else
                {
                    objDocument.W_GetNextDocumentNo(ref DBCommand, "DR", "", "", ref DocNoDriverID, ref message);
                }


                DS_Owner_Mst server_OwnerDriver = new DS_Owner_Mst();

                #region Driver Master
                for (int mprCnt = 0; mprCnt < objownermst.driver_mst.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_mst.ImportRow(objownermst.driver_mst.Rows[mprCnt]);
                    server_OwnerDriver.driver_mst[mprCnt].driver_id = DocNoDriverID;
                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_prefered_destination.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_prefered_destination.ImportRow(objownermst.driver_prefered_destination.Rows[mprCnt]);
                    server_OwnerDriver.driver_prefered_destination[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_mobile_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_mobile_detail.ImportRow(objownermst.driver_mobile_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_mobile_detail[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_language_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_language_detail.ImportRow(objownermst.driver_language_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_language_detail[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_insurance_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_insurance_detail.ImportRow(objownermst.driver_insurance_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_insurance_detail[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_identification_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_identification_detail.ImportRow(objownermst.driver_identification_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_identification_detail[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_contact_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_contact_detail.ImportRow(objownermst.driver_contact_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_contact_detail[mprCnt].driver_id = DocNoDriverID;

                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_bank_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_bank_detail.ImportRow(objownermst.driver_bank_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_bank_detail[mprCnt].driver_id = DocNoDriverID;
                }

                for (int mprCnt = 0; mprCnt < objownermst.driver_license_detail.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.driver_license_detail.ImportRow(objownermst.driver_license_detail.Rows[mprCnt]);
                    server_OwnerDriver.driver_license_detail[mprCnt].driver_id = DocNoDriverID;
                }

                for (int mprCnt = 0; mprCnt < objownermst.user_mst.Rows.Count; mprCnt++)
                {
                    server_OwnerDriver.user_mst.ImportRow(objownermst.user_mst.Rows[mprCnt]);
                    server_OwnerDriver.user_mst[mprCnt].unique_id = DocNoDriverID;
                }

                #endregion

                #region Owner Driver Relation
                //if (objownermst.owner_driver_details != null && objownermst.owner_driver_details.Rows.Count > 0)
                //{
                //    for (int mprCnt = 0; mprCnt < objownermst.owner_driver_details.Rows.Count; mprCnt++)
                //    {
                //        server_OwnerDriver.owner_driver_details.ImportRow(objownermst.owner_driver_details.Rows[mprCnt]);
                //        server_OwnerDriver.owner_driver_details[mprCnt].driver_id = DocNoDriverID;
                //    }
                //}
                #endregion

                #region Truck & Driver Relation
                if (objownermst.driver_truck_details != null && objownermst.driver_truck_details.Rows.Count > 0)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.driver_truck_details.Rows.Count; mprCnt++)
                    {
                        server_OwnerDriver.driver_truck_details.ImportRow(objownermst.driver_truck_details.Rows[mprCnt]);
                        server_OwnerDriver.driver_truck_details[mprCnt].driver_id = DocNoDriverID;
                    }
                }
                #endregion

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_mst.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                //objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.owner_driver_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                //if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.owner_driver_details.Rows.Count)
                //{
                //    DBCommand.Transaction.Rollback();
                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //    objBLReturnObject.ExecutionStatus = 2;
                //    responsecls.status = "0";
                //    responsecls.Message = objBLReturnObject.ServerMessage;
                //    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                //    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                //    return objBLReturnObject;
                //}
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_truck_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_contact_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_contact_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_identification_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_identification_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = jser.Serialize(objBLReturnObject.ServerMessage);
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_mobile_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_mobile_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_prefered_destination, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_prefered_destination.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_language_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_language_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_bank_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_bank_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_insurance_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_insurance_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.user_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.user_mst.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_OwnerDriver.driver_license_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_OwnerDriver.driver_license_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    responsecls.status = "0";
                    responsecls.Message = objBLReturnObject.ServerMessage;
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    //objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    responsecls.status = "1";
                    responsecls.ownerid = DocNo;
                    responsecls.driverid = DocNoDriverID;
                    responsecls.truckid = "";
                    ServerLog.SuccessLog("Driver Master Saved : OwnerId = " + DocNo + ", DriverId = " + DocNoDriverID);
                    responsecls.Message = "Data Saved Successfully ";
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                responsecls.status = "0";
                responsecls.Message = objBLReturnObject.ServerMessage;
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
        }

        //pooja vachhani on 23/11/15
        public BLReturnObject Update_rto_permit_table(DataSet ds_UploadData)
        {
            ResponseCls responsecls = new ResponseCls();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                objtrukmst = (DS_Truck_Mst)ds_UploadData;

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                DS_Truck_Mst server_truck = new DS_Truck_Mst();
                Document Doc = new Document();
                Document objDocument = new Document();

                if (objtrukmst.truck_rto_registration_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_rto_registration_detail.Rows.Count; mprCnt++)
                    {
                        server_truck.truck_rto_registration_detail.ImportRow(objtrukmst.truck_rto_registration_detail.Rows[mprCnt]);
                        server_truck.truck_rto_registration_detail[mprCnt].AcceptChanges();
                        server_truck.truck_rto_registration_detail[mprCnt].SetModified();
                    }
                }

                if (objtrukmst.truck_permit_details != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_permit_details.Rows.Count; mprCnt++)
                    {
                        server_truck.truck_permit_details.ImportRow(objtrukmst.truck_permit_details.Rows[mprCnt]);
                        server_truck.truck_permit_details[mprCnt].AcceptChanges();
                        server_truck.truck_permit_details[mprCnt].SetModified();
                    }
                }

                if (objtrukmst.truck_insurance_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_insurance_detail.Rows.Count; mprCnt++)
                    {
                        server_truck.truck_insurance_detail.ImportRow(objtrukmst.truck_insurance_detail.Rows[mprCnt]);
                        server_truck.truck_insurance_detail[mprCnt].AcceptChanges();
                        server_truck.truck_insurance_detail[mprCnt].SetModified();
                    }
                }

                if (objtrukmst.truck_maintenance_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objtrukmst.truck_maintenance_detail.Rows.Count; mprCnt++)
                    {
                        server_truck.truck_maintenance_detail.ImportRow(objtrukmst.truck_maintenance_detail.Rows[mprCnt]);
                        server_truck.truck_maintenance_detail[mprCnt].AcceptChanges();
                        server_truck.truck_maintenance_detail[mprCnt].SetModified();
                    }
                }



                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_truck.truck_rto_registration_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);

                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_truck.truck_rto_registration_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Truck RTO Registration Details : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_truck.truck_permit_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_truck.truck_permit_details.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Truck Permit Details : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //objBLReturnObject.ServerMessage = "Fail to Save Details.";
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_truck.truck_insurance_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_truck.truck_insurance_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Truck Insurance Details : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_truck.truck_maintenance_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_truck.truck_maintenance_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Truck Maintenance Details : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    responsecls.status = "1";
                    responsecls.ownerid = "";
                    responsecls.driverid = "";
                    responsecls.truckid = "";
                    ServerLog.SuccessLog("RTO Permit Details, Insurance Details Updated For Image Upload");
                    responsecls.Message = "Data Saved Successfully";
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("RTO Permit Photo Update Details : " + ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                responsecls.status = "0";
                responsecls.Message = ex.Message;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
        }

        public BLReturnObject Update_driver_table(DataSet ds_UploadData)
        {
            string DrvPhtpath = "";
            ResponseCls responsecls = new ResponseCls();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 0;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                objownermst = (DS_Owner_Mst)ds_UploadData;

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                DS_Owner_Mst server_driver = new DS_Owner_Mst();
                Document Doc = new Document();
                Document objDocument = new Document();

                if (objownermst.driver_mst != null)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.driver_mst.Rows.Count; mprCnt++)
                    {
                        server_driver.driver_mst.ImportRow(objownermst.driver_mst.Rows[mprCnt]);
                        server_driver.driver_mst[mprCnt].AcceptChanges();
                        server_driver.driver_mst[mprCnt].SetModified();
                        DrvPhtpath = server_driver.driver_mst[0].driver_photo;
                    }

                }



                if (objownermst.driver_license_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.driver_license_detail.Rows.Count; mprCnt++)
                    {
                        server_driver.driver_license_detail.ImportRow(objownermst.driver_license_detail.Rows[mprCnt]);
                        server_driver.driver_license_detail[mprCnt].AcceptChanges();
                        server_driver.driver_license_detail[mprCnt].SetModified();
                    }
                }

                if (objownermst.driver_contact_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.driver_contact_detail.Rows.Count; mprCnt++)
                    {
                        server_driver.driver_contact_detail.ImportRow(objownermst.driver_contact_detail.Rows[mprCnt]);
                        server_driver.driver_contact_detail[mprCnt].AcceptChanges();
                        server_driver.driver_contact_detail[mprCnt].SetModified();
                    }
                }

                if (objownermst.driver_identification_detail != null)
                {
                    for (int mprCnt = 0; mprCnt < objownermst.driver_identification_detail.Rows.Count; mprCnt++)
                    {
                        server_driver.driver_identification_detail.ImportRow(objownermst.driver_identification_detail.Rows[mprCnt]);
                        server_driver.driver_identification_detail[mprCnt].AcceptChanges();
                        server_driver.driver_identification_detail[mprCnt].SetModified();
                    }
                }

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_driver.driver_mst, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);

                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_driver.driver_mst.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 0;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Driver Master in Update : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_driver.driver_license_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_driver.driver_license_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 0;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Driver License Details in Update : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //objBLReturnObject.ServerMessage = "Fail to Save Details.";
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_driver.driver_contact_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_driver.driver_contact_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 0;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Driver Contact Details in Update : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //objBLReturnObject.ServerMessage = "Fail to Save Details.";
                    return objBLReturnObject;
                }

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, server_driver.driver_identification_detail, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.Update);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != server_driver.driver_identification_detail.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 0;
                    responsecls.status = "0";
                    responsecls.Message = objUpdateTableInfo.ErrorMessage;
                    ServerLog.Log("Driver Identification Details in Update : " + objUpdateTableInfo.ErrorMessage.ToString());
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    responsecls.status = "1";
                    responsecls.ownerid = "";
                    responsecls.driverid = "";
                    responsecls.truckid = "";
                    responsecls.driver_img = DrvPhtpath;
                    ServerLog.SuccessLog("Driver Insurance, Identification Image Upload ");
                    responsecls.Message = "Data Saved Successfully";
                    objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                    //objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log("Driver Photo Update Details : " + ex.Message + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                responsecls.status = "0";
                responsecls.Message = ex.Message;
                objBLReturnObject.ServerMessage = jser.Serialize(responsecls);
                return objBLReturnObject;
            }
        }

        public BLReturnObject Save_Post_Order_quotation(DataSet ds_UploadData, ref IDbCommand DBCommand)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {

                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                DS_load_order_quotation ds_load_order_quotation = new DS_load_order_quotation();
                ds_load_order_quotation = (DS_load_order_quotation)ds_UploadData;
                //DBConnection.Open();
                //DBCommand.Transaction = DBConnection.BeginTransaction();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_load_order_quotation.load_order_enquiry_quotation, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != ds_load_order_quotation.load_order_enquiry_quotation.Rows.Count)
                {
                    //DBCommand.Transaction.Rollback();
                    //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ServerMessage = objUpdateTableInfo.ErrorMessage;
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    return objBLReturnObject;
                }
                else
                {
                    //DBCommand.Transaction.Commit();
                    //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    ServerLog.SuccessLog("Quotation Generated for Inquiry no = " + ds_load_order_quotation.load_order_enquiry_quotation.Rows[0]["Load_Inquiry_no"].ToString());
                    objBLReturnObject.ExecutionStatus = 1;
                    objBLReturnObject.ServerMessage = "Initial Quote Genereted Successfully";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = ex.Message;
                return objBLReturnObject;
            }
        }

        public BLReturnObject Save_For_Transporter_Notification(DataSet ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                DS_load_order_quotation ds_load_order = new DS_load_order_quotation();
                ds_load_order = (DS_load_order_quotation)ds_UploadData;

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_load_order.load_order_enquiry_quotation, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != ds_load_order.load_order_enquiry_quotation.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = "Failed to Save Details.";
                    return objBLReturnObject;
                }
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_load_order.load_enquiry_transporter_notification, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != ds_load_order.load_enquiry_transporter_notification.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    //objBLReturnObject.ServerMessage = "Failed to Save Details.";
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Saved For Transporter Notification : Inquiry ID = " + ds_load_order.load_enquiry_transporter_notification.Rows[0]["load_inquiry_no"].ToString());
                    objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = "Fail to Save Details.";
                return objBLReturnObject;
            }
        }

        public BLReturnObject Update_Accepted_Transporter_Notification(DataSet ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                DS_load_order_quotation ds_load_order = new DS_load_order_quotation();
                ds_load_order = (DS_load_order_quotation)ds_UploadData;

                string inqid = ds_load_order.load_enquiry_transporter_notification.Rows[0]["load_inquiry_no"].ToString();
                string trkid = ds_load_order.load_enquiry_transporter_notification.Rows[0]["truck_id"].ToString();
                string ownid = ds_load_order.load_enquiry_transporter_notification.Rows[0]["owner_id"].ToString();
                string drvid = ds_load_order.load_enquiry_transporter_notification.Rows[0]["driver_id"].ToString();

                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, ds_load_order.load_enquiry_transporter_notification, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != ds_load_order.load_enquiry_transporter_notification.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    //objBLReturnObject.ServerMessage = "Failed to Save Details.";
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    ServerLog.SuccessLog("Update Transporter Notification : Inquiry ID = " + inqid + ", Owner ID = " + ownid + ", Driver ID = " + drvid + ", Truck ID = " + trkid);
                    objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = "Fail to Save Details.";
                return objBLReturnObject;
            }
        }

        public BLReturnObject save_load_order_paid_and_confirmedByShipper(DataSet ds_UploadData)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {
                if (ds_UploadData == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }
                DS_orders dS_orders = new DS_orders();
                dS_orders = (DS_orders)ds_UploadData;
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_orders.orders, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != dS_orders.orders.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    objBLReturnObject.ServerMessage = "Failed to Save Details.";
                    return objBLReturnObject;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 1;
                    objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }
            }
            catch (Exception ex)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                ServerLog.Log(ex.Message.ToString());
                objBLReturnObject.ServerMessage = "Fail to Save Details.";
                return objBLReturnObject;
            }
        }

        public BLReturnObject UpdateTables(DataTable dt_update, ref IDbCommand DBCommand)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {
                if (dt_update == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "There is no data to save";
                    return objBLReturnObject;
                }

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;

                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dt_update, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly, BLGeneralUtil.UpdateMethod.DeleteAndInsert);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != dt_update.Rows.Count)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objUpdateTableInfo.ErrorMessage);
                    objBLReturnObject.ServerMessage = "Failed to Save Details.";
                    return objBLReturnObject;
                }
                else
                {
                    objBLReturnObject.ExecutionStatus = 1;
                    objBLReturnObject.ServerMessage = "Data Saved Successfully";
                    return objBLReturnObject;
                }

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = ex.Message.ToString() + Environment.NewLine + ex.StackTrace;
                return objBLReturnObject;
            }
        }

        public BLReturnObject UpdateTablesArray(DataTable[] dt_update)
        {
            BLReturnObject objrtn = new BLReturnObject();
            if (dt_update == null)
            {
                objrtn.ExecutionStatus = 2;
                objrtn.ServerMessage = " NO TABLES FOUND TO UPDATE";
                return objrtn;
            }
            try
            {
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();
                BLGeneralUtil.UpdateTableInfo utf;
                for (int i = 0; i < dt_update.Length; i++)
                {
                    if (dt_update[i] == null)
                        continue;
                    utf = BLGeneralUtil.UpdateTable(ref DBCommand, dt_update[i], BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!utf.Status || utf.TotalRowsAffected != dt_update[i].Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open)
                            DBConnection.Close();
                        ServerLog.Log(utf.ErrorMessage);
                        objrtn.ExecutionStatus = 2;
                        objrtn.ServerMessage = " Failed to Update: " + dt_update[i].TableName;
                        return objrtn;
                    }
                }
                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objrtn.ExecutionStatus = 1;
                objrtn.ServerMessage = " Data Saved Successfully";
                return objrtn;
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objrtn.ExecutionStatus = 2;
                objrtn.ServerMessage = "Failed to Save Data.";
                return objrtn;
            }
        }

        private DataTable GetEmailTemplate(string TemplateType)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            DataTable dtmailtemplate = new DataTable();
            String query1 = "SELECT * FROM emailTemplate Where name = '" + TemplateType + "' and active ='Y' ";
            SqlCommand cmd = new SqlCommand(query1, con);
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dtmailtemplate);
            if (dtmailtemplate != null && dtmailtemplate.Rows.Count > 0)
                return dtmailtemplate;
            else
                return null;
        }

        public BLReturnObject SaveEmail(EmailData objemail, ref IDbCommand DBCommand)
        {
            BLReturnObject objBLReturnObject = new BLReturnObject();

            try
            {
                if (objemail == null)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "Nothing to send as Email";
                    return objBLReturnObject;
                }
                DataTable dt_template = GetEmailTemplate(objemail.MailFor);
                if (dt_template == null || dt_template.Rows.Count == 0)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    objBLReturnObject.ServerMessage = "Email template not found";
                    return objBLReturnObject;
                }
                DS_Email dS_email = new DS_Email();
                string emailbody = "";
                string emailsubject = "";

                emailsubject = dt_template.Rows[0]["subject"].ToString();
                emailbody = dt_template.Rows[0]["content"].ToString();

                emailbody = emailbody.Replace("{NAME}", objemail.shipper_id);
                emailbody = emailbody.Replace("{InquiryID}", objemail.load_inquiry_no);
                emailbody = emailbody.Replace("{OwnerID}", objemail.owner_id);
                emailbody = emailbody.Replace("{DriverID}", objemail.driver_id);
                emailbody = emailbody.Replace("{TruckID}", objemail.truck_id);


                emailbody = emailbody.Replace("{LRID}", objemail.lr_id);
                emailbody = emailbody.Replace("{LRDT}", objemail.lr_date);
                emailbody = emailbody.Replace("{MATERIALDESC}", objemail.material_desc);
                emailbody = emailbody.Replace("{MATERIALQTY}", objemail.material_qty);
                emailbody = emailbody.Replace("{MATERIALVALUE}", objemail.material_value);
                emailbody = emailbody.Replace("{TRANSPORTER}", objemail.transporter);


                emailsubject = emailsubject.Replace("{ShipperID}", objemail.shipper_id);
                emailsubject = emailsubject.Replace("{InquiryID}", objemail.load_inquiry_no);
                emailsubject = emailsubject.Replace("{OwnerID}", objemail.owner_id);
                emailsubject = emailsubject.Replace("{DriverID}", objemail.driver_id);
                emailsubject = emailsubject.Replace("{TruckID}", objemail.truck_id);


                string maildocno = "";
                objDocument.W_GetNextDocumentNo(ref DBCommand, "EMAIL", "", "", ref maildocno, ref message);

                dS_email.EnforceConstraints = false;
                DS_Email.sentMailLogRow mailrow = dS_email.sentMailLog.NewsentMailLogRow();
                mailrow.doc_id = maildocno;
                mailrow.MailFor = objemail.MailFor;
                mailrow.to_email = objemail.to_email;
                mailrow.CC = objemail.CC;
                mailrow.BCC = objemail.BCC;
                mailrow.Subject = emailsubject;
                mailrow.AttachWIPOPdf = objemail.AttachWIPOPdf;
                mailrow.attachedDoc = objemail.attachedDoc;
                mailrow.emailbody = emailbody;
                mailrow.active = "Y";
                mailrow.IsEmailSent = "N";
                mailrow.created_date = System.DateTime.UtcNow;
                mailrow.created_by = objemail.created_by;
                mailrow.created_host = "admin";
                mailrow.created_host = "admin";
                mailrow.device_id = objemail.device_id;
                mailrow.device_type = objemail.device_type;
                dS_email.sentMailLog.AddsentMailLogRow(mailrow);


                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
                objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_email.sentMailLog, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (objUpdateTableInfo.Status == false || objUpdateTableInfo.TotalRowsAffected != dS_email.sentMailLog.Rows.Count)
                {
                    objBLReturnObject.ExecutionStatus = 2;
                    ServerLog.Log(objUpdateTableInfo.ErrorMessage);
                    objBLReturnObject.ServerMessage = objUpdateTableInfo.ErrorMessage;
                    return objBLReturnObject;
                }
                else
                {
                    objBLReturnObject.ExecutionStatus = 1;
                    objBLReturnObject.ServerMessage = "Email Data Saved Successfully";
                    return objBLReturnObject;
                }
            }
            catch (Exception ex)
            {
                objBLReturnObject.ExecutionStatus = 2;
                objBLReturnObject.ServerMessage = ex.Message;
                ServerLog.Log(ex.Message.ToString());
                return objBLReturnObject;
            }
        }

        public bool CreateDriverNotificationHistory(ref DS_driver_order_notifications ds_driver_notification, DataTable dtData, ref IDbCommand DBCommand, ref String msg)
        {
            try
            {
                ds_driver_notification.EnforceConstraints = false;
                Document objdoc = new Document(); String DocNtficID = "";
                if (!objdoc.W_GetNextDocumentNo(ref DBCommand, "DRN", "", "", ref DocNtficID, ref msg)) // New Driver Notification ID
                    return false;
                ds_driver_notification.driver_order_notifications_history.ImportRow(dtData.Rows[0]);
                ds_driver_notification.driver_order_notifications_history.Rows[0]["notification_id"] = DocNtficID;
                ds_driver_notification.driver_order_notifications_history.Rows[0]["notification_date"] = System.DateTime.UtcNow;
                ds_driver_notification.driver_order_notifications_history.Rows[0].AcceptChanges();
                ds_driver_notification.driver_order_notifications_history.Rows[0].SetAdded();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            return true;
        }

        public bool SetAndModifyStatusinAnyTable(string tablename, string status, string shipperid, string inquiryno, ref IDbCommand DBCommand, ref String message)
        {
            message = "";
            DBCommand.Parameters.Clear();
            string where = "";
            string query = " UPDATE " + tablename +
                     " SET status =   '" + status + "' " + " WHERE ";

            if (shipperid != string.Empty && shipperid != "" && shipperid != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@shipperid", DbType.String, shipperid));
                where = " shipper_id= @shipperid ";
            }
            if (inquiryno != string.Empty && inquiryno != "" && inquiryno != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inquiryno", DbType.String, inquiryno));
                if (where != string.Empty)
                    where = where + "AND  load_inquiry_no= @inquiryno ";
                else
                    where = " load_inquiry_no = @inquiryno ";
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
                message = ex.Message + ex.StackTrace;
                return false;
            }
            if (row_count <= 0)
            {
                ServerLog.Log("Status not Updated " + tablename + Environment.NewLine + query);
                message = "Status not Updated " + tablename + Environment.NewLine + query;
                return false;
            }
            else
                return true;

        }


        public DataSet CreateDataSet<T>(List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            var obj = list[0].GetType();

            //now grab all properties
            var properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            var dataSet = new DataSet();
            var dataTable = new DataTable();

            //now build the columns from the properties
            var columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            dataSet.Tables.Add(dataTable);

            //return dataset
            return dataSet;
        }


        public byte DeleteFile(string ImgPath)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ImgPath)))
            {
                System.IO.File.Delete(HttpContext.Current.Server.MapPath(ImgPath));
                return 0;
            }
            else
                return 1;
        }
        #endregion
    }
}