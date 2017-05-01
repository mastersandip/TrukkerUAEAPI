using BLL.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using trukkerUAE.Models;
using trukkerUAE.XSD;
using Newtonsoft.Json;
using System.Data;
using BLL.Master;

namespace trukkerUAE.Controllers
{
    public class RelationController : ApiController
    {
        JavaScriptSerializer jser = new JavaScriptSerializer();
        DS_Truck_Mst objtruck = new DS_Truck_Mst();
        DS_Owner_Mst objowner = new DS_Owner_Mst();
        ResponseCls responsecls = new ResponseCls();

      
        [HttpPost]
        public string SaveRelation([FromBody] JObject Parameter)
        {
            var tempkey = Parameter.Properties().Select(p => p.Name).ToList();

            BLReturnObject objBLReturnObject = new BLReturnObject();
            Master SvRelation = new Master();

            owner_truck_details truck_Owner_data = new owner_truck_details();
            driver_truck_detail drivertruck = new driver_truck_detail();
            owner_driver_detail ownerDriverData = new owner_driver_detail();
            LoginCheck login_data = new LoginCheck();
            owner_truck_details ownerTruckDetailData = new owner_truck_details();

            if (tempkey.Contains("login_data"))
                login_data = Parameter["login_data"].ToObject<LoginCheck>();
            if (tempkey.Contains("truck_owner"))
                truck_Owner_data = Parameter["truck_owner"].ToObject<owner_truck_details>();
            if (tempkey.Contains("truck_driver"))
                drivertruck = Parameter["truck_driver"].ToObject<driver_truck_detail>();
            if (tempkey.Contains("owner_driver"))
                ownerDriverData = Parameter["owner_driver"].ToObject<owner_driver_detail>();


            #region Truck Owner Data
            if (truck_Owner_data != null)
            {
                if (truck_Owner_data.owner_id != null && truck_Owner_data.truck_id != null)
                {
                    try
                    {
                        //owid = truck_Owner_data.owner_id;
                        DS_Truck_Mst.owner_truck_detailsRow owrow = objtruck.owner_truck_details.Newowner_truck_detailsRow();
                        owrow.owner_id = truck_Owner_data.owner_id;
                        owrow.truck_id = truck_Owner_data.truck_id;
                        owrow.active_flag = "Y";
                        owrow.created_by = login_data.user_id;
                        owrow.created_date = System.DateTime.UtcNow;
                        owrow.created_host = "1111";
                        owrow.device_id = login_data.device_id;
                        owrow.device_type = login_data.device_type;
                        objtruck.owner_truck_details.Addowner_truck_detailsRow(owrow);
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Truck Owner Relation: " + ex.Message.ToString() + ex.StackTrace.ToString());
                        return ex.Message.ToString();
                    }
                }
            }
            #endregion

            #region Truck Driver Details
            if (drivertruck != null)
            {
                if (drivertruck.driver_id != null && drivertruck.truck_id != null)
                {
                    try
                    {
                        DS_Truck_Mst.driver_truck_detailsRow trkdrvrow = objtruck.driver_truck_details.Newdriver_truck_detailsRow();
                        trkdrvrow.driver_id = drivertruck.driver_id;
                        trkdrvrow.truck_id = drivertruck.truck_id;
                        trkdrvrow.active_flag = "Y";
                        trkdrvrow.created_by = login_data.user_id;
                        trkdrvrow.created_date = System.DateTime.UtcNow;
                        trkdrvrow.created_host = "1111";
                        trkdrvrow.device_id = login_data.device_id;
                        trkdrvrow.device_type = login_data.device_type;
                        objtruck.driver_truck_details.Adddriver_truck_detailsRow(trkdrvrow);

                    }
                    catch (Exception ex)
                    {
                        ServerLog.Log("Truck Driver Relation : " + ex.Message.ToString());
                        return ex.Message.ToString();
                    }
                }
            }

            #endregion

            #region Owner Driver Details
            try
            {
                if (tempkey.Contains("owner_driver"))
                {
                    ownerDriverData = Parameter["owner_driver"].ToObject<owner_driver_detail>();
                    if (ownerDriverData != null)
                    {
                        if (ownerDriverData.owner_id != null && ownerDriverData.driver_id != null)
                        {
                            try
                            {
                                DS_Owner_Mst.owner_driver_detailsRow owndrvrow = objowner.owner_driver_details.Newowner_driver_detailsRow();
                                owndrvrow.owner_id = ownerDriverData.owner_id;
                                owndrvrow.driver_id = ownerDriverData.driver_id;
                                owndrvrow.active_flag = "Y";
                                owndrvrow.created_by = login_data.user_id;
                                owndrvrow.created_date = System.DateTime.UtcNow;
                                owndrvrow.created_host = "1111";
                                owndrvrow.device_id = login_data.device_id;
                                owndrvrow.device_type = login_data.device_type;
                                objowner.owner_driver_details.Addowner_driver_detailsRow(owndrvrow);
                            }
                            catch (Exception ex)
                            {
                                ServerLog.Log("Owner Driver Relation: " + ex.Message.ToString());
                                responsecls.status = "0";
                                responsecls.Message = ex.Message;
                                return jser.Serialize(responsecls);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                responsecls.status = "0";
                responsecls.Message = ex.Message;
                return jser.Serialize(responsecls);
            }
            #endregion


            DataSet[] dsall = new DataSet[2];
            dsall[0] = objtruck;
            dsall[1] = objowner;

            // objBLReturnObject = ownermst.Save_Owner_Mst(objowner, objOwnerDriver);
            objBLReturnObject = SvRelation.SaveOwnerTruckDriverRelation(dsall);
            if (objBLReturnObject.ExecutionStatus == 1)
            {
                return objBLReturnObject.ServerMessage;
            }
            else
            {
                ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                return objBLReturnObject.ServerMessage;
            }
        }
    }
}
