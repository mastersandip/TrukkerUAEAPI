using BLL.Master;
using BLL.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using trukkerUAE.Classes;
using trukkerUAE.Models;
using trukkerUAE.XSD;

namespace trukkerUAE.Controllers
{
    public class CBMController : ServerBase
    {



        [HttpGet]
        public string getApp()
        {
            DataTable dt = new DataTable();
            dt = GetAppartment();

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");


        }
        [HttpGet]
        public string Get_Room_Name(string app_id)
        {
            DataTable dt = new DataTable();
            dt = GetLocation(app_id);


            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetAllItem(string room)
        {
            DataTable dt = new DataTable();
            dt = GetAllItemdt(room);

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetItem(string room, string quote_id)
        {
            DataTable dt = new DataTable();
            dt = GetItemdt(room, quote_id);

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string GetOrderDetails(string quote_id)
        {
            DataTable dt = new DataTable();
            dt = dtGetOrderDetails(quote_id);

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string Get_item_dropdown_Details(string location_id)
        {
            DataTable dt = new DataTable();
            //dt = objmaster.Get_item_dropdown_Details();
            dt = Get_item_dropdown(location_id);

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    return JsonConvert.SerializeObject(dt);
            //}
            //else
            //{
            //    return null;
            //}

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }
        [HttpGet]
        public string quote_data()
        {
            DataTable dt = new DataTable();
            dt = quote_data_dt();

            if (dt == null)
            {
                return null;
            }
            else
            {
                return JsonConvert.SerializeObject(dt);
                //  return objmaster.GetJson1(dt);
            }
        }


        [HttpGet]
        public string Get_check_quote_id_is_available(string quote_id)
        {
            DataTable dt = new DataTable();
            dt = dtcheck_quote_id_is_available(quote_id);

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }

        public DataTable quote_data_dt()
        {


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            SqlSelect = @"SELECT     quote_id, customer_name, customer_mobile, customer_email, item_cbm, room_type, otp, StatusFlag,appartment_id
                            FROM         quote_hdr 
                            inner join appartment_type_mst on appartment_type_mst.appartment_desc=quote_hdr.room_type";

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
                return null;
            }
        }
        public DataTable quote_details_dt(string quote_id)
        {


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            //            SqlSelect = @"SELECT     quote_dtl.quote_dtl_id, quote_dtl.quote_id, quote_dtl.item_code, case when item_mst.item_name is null then quote_dtl.item_code
            //else item_mst.item_name end  as item_name, quote_dtl.item_item_cbm, quote_dtl.item_qty
            //                        FROM quote_dtl left JOIN item_mst ON quote_dtl.item_code = item_mst.item_code where
            //                        quote_id = '" + quote_id + "'";

            SqlSelect = @"SELECT     quote_dtl.quote_dtl_id, quote_dtl.quote_id, quote_dtl.item_code, case when item_mst.item_name is null then quote_dtl.item_code
else item_mst.item_name end  as item_name, quote_dtl.item_cbm, quote_dtl.item_qty
                        FROM quote_dtl left JOIN item_mst ON quote_dtl.item_code = item_mst.item_code left join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code where
                        quote_id = '" + quote_id + "'";

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
                return null;
            }
        }
        public DataTable GetItemdt(string room, string quote_id)
        {
            quote_id = Decrypt(quote_id);

            DataTable dtquoteid = GetQtHdrDetailsByQuote_Hdr_id(quote_id);
            if (dtquoteid != null)
            {
                if (dtquoteid.Rows[0]["quote_id"].ToString() != quote_id)
                    quote_id = dtquoteid.Rows[0]["quote_id"].ToString();
            }

            var Flag = "Y";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master


            SqlSelect = @"if not exists(select Top(1) quote_id from quote_dtl where quote_id='" + quote_id + @"')
                            BEGIN                         
                            select srNo,item_code,company_code, owner,location_id, item_name,item_cbm, active_flag,item_qty,ItemFlg, RoomSrNo , bedroomSize
                            from(SELECT ROW_NUMBER() over (order by item_mst.item_code) as srNo,cast(item_mst.item_code as nvarchar(50)) as item_code,
                            company_code, owner, item_mst.location_id, item_name, isnull(cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
                            item_mst.location_id as RoomSrNo ,
                            case When [location_display] like 'Bedroom%' then SUBSTRING([location_display], 9,Len([location_display]) )
                            else 0 end as bedroomSize ,parent_location_id,Default_flag
                            FROM   item_mst
                            right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
                            right join location_mst on item_mst.location_id=location_mst.location_id   
                            right join location_app_mapping  on location_app_mapping.location_id=location_mst.location_id  and appartment_id=" + room + @"                          
                            )as y                              
                            where  active_flag= 'Y' and  parent_location_id=0   and  Default_flag ='Y'                 
                            END                            
                            ELSE                            
                            BEGIN    
                            select srNo,item_code,company_code,owner,location_id,
                            item_name,item_cbm,active_flag,item_qty,ItemFlg,RoomSrNo,bedroomSize                            
                            from (                            
                            select y1.srNo as srNo,
                            cast(y1.item_code as nvarchar(10)) as item_code,1 as company_code,                            
                            'abc' as owner,quote_dtl.location_id,item_name,quote_dtl.item_cbm,active_flag,quote_dtl.item_qty,quote_dtl.ItemFlg,
                            y1.location_id as RoomSrNo, (select room_type from quote_hdr where quote_id='" + quote_id + @"') as bedroomSize,
                            quote_id                            
                            from quote_dtl                            
                            inner join 
                            (select *                            
                            from(SELECT ROW_NUMBER() over (order by item_mst.item_code, item_mst.location_id) as srNo,cast(item_mst.item_code as nvarchar(50)) as item_code,                             
                            company_code, owner, item_mst.location_id, item_name, isnull(cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
                            item_mst.location_id as RoomSrNo ,
                            case When [location_display] like 'Bedroom%' then SUBSTRING([location_display], 9,Len([location_display]) ) 
                            else 0 end as bedroomSize ,Default_flag  
                            FROM   item_mst
                            right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
                            right join location_mst on item_mst.location_id=location_mst.location_id    
                            right join location_app_mapping  on location_app_mapping.location_id=location_mst.location_id  and appartment_id=" + room + @"                         
                            )as y                           
                            where  active_flag= 'Y'                   
                            )as y1 on quote_dtl.quote_id='" + quote_id + @"' and y1.item_code=quote_dtl.item_code and y1.location_id=quote_dtl.location_id                           
                            Union All                            
                            select (select COUNT(*) from item_mst right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
                            right join location_mst on item_mst.location_id=location_mst.location_id )+ ROW_NUMBER() OVER (order by item_code) AS srNo,item_code,1 as company_code,'abc' as owner,location_id,item_code,item_cbm,'Y' as active_flag,
                            item_qty,ItemFlg,location_id as RoomSrNo, (select room_type from quote_hdr where quote_id='" + quote_id + @"') as bedroomSize,
                            quote_id
                            from quote_dtl where ItemFlg='N'  and quote_id='" + quote_id + @"'                          
                            ) as y                            
                             where active_flag= 'Y'  and y.item_cbm is not null                             
                            order by location_id                            
                            END";

            //            SqlSelect = @"if not exists(select Top(1) quote_id from quote_dtl where quote_id='" + quote_id + @"')
            //                            BEGIN                         
            //                            select srNo,item_code,company_code, owner,location_id, item_name,item_cbm, active_flag,item_qty,ItemFlg, RoomSrNo , bedroomSize
            //                            from(SELECT ROW_NUMBER() over (order by item_mst.item_code) as srNo,cast(item_mst.item_code as nvarchar(50)) as item_code,
            //                            company_code, owner, item_mst.location_id, item_name, isnull(item_cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
            //                            item_mst.location_id as RoomSrNo ,
            //                            case When [location_display] like 'Bedroom%' then SUBSTRING([location_display], 9,Len([location_display]) )
            //                            else 0 end as bedroomSize ,parent_location_id,Default_flag
            //                            FROM   item_mst
            //                            right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
            //                            right join location_mst on item_mst.location_id=location_mst.location_id   
            //                            right join location_app_mapping  on location_app_mapping.location_id=location_mst.location_id  and appartment_id=" + room + @"                           
            //                            )as y                              
            //                            where  active_flag= '" + Flag + @"' and  parent_location_id=0   and  Default_flag ='Y'                 
            //                            END                            
            //                            ELSE                            
            //                            BEGIN    
            //                            select srNo,item_code,company_code,owner,location_id,
            //                            item_name,item_item_cbm AS item_cbm,active_flag,item_qty  as item_qty,ItemFlg  as ItemFlg,RoomSrNo,bedroomSize                            
            //                            from (                            
            //                            select y1.srNo as srNo,
            //                            cast(y1.item_code as nvarchar(10)) as item_code,1 as company_code,                            
            //                            'abc' as owner,quote_dtl.location_id,item_name,item_item_cbm,active_flag,item_qty,ItemFlg,
            //                            y1.location_id as RoomSrNo, (select bedroom from quote_hdr where quote_id='" + quote_id + @"') as bedroomSize,
            //                            quote_id                            
            //                            from quote_dtl                            
            //                            inner join 
            //                            (select *                            
            //                            from(SELECT ROW_NUMBER() over (order by item_mst.item_code, item_mst.location_id) as srNo,cast(item_mst.item_code as nvarchar(50)) as item_code,                             
            //                            company_code, owner, item_mst.location_id, item_name, isnull(item_cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
            //                            item_mst.location_id as RoomSrNo ,
            //                            case When [location_display] like 'Bedroom%' then SUBSTRING([location_display], 9,Len([location_display]) ) 
            //                            else 0 end as bedroomSize ,Default_flag  
            //                            FROM   item_mst
            //                            right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
            //                            right join location_mst on item_mst.location_id=location_mst.location_id    
            //                            right join location_app_mapping  on location_app_mapping.location_id=location_mst.location_id  and appartment_id=" + room + @"                        
            //                            )as y                           
            //                            where  active_flag= 'Y'                   
            //                            )as y1 on quote_dtl.quote_id='" + quote_id + @"' and y1.item_code=quote_dtl.item_code and y1.location_id=quote_dtl.location_id                           
            //                            Union All                            
            //                            select (select COUNT(*) from item_mst right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
            //                            right join location_mst on item_mst.location_id=location_mst.location_id )+ ROW_NUMBER() OVER (order by item_code) AS srNo,item_code,1 as company_code,'abc' as owner,location_id,item_code,item_item_cbm,'Y' as active_flag,
            //                            item_qty,ItemFlg,location_id as RoomSrNo, (select bedroom from quote_hdr where quote_id='" + quote_id + @"') as bedroomSize,
            //                            quote_id
            //                            from quote_dtl where ItemFlg='N'  and quote_id='" + quote_id + @"'                          
            //                            ) as y                            
            //                             where active_flag= '" + Flag + @"'  and y.item_item_cbm is not null                             
            //                            order by location_id                            
            //                            END";
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
                return null;
            }
        }
        public DataTable dtGetOrderDetails(string quote_id)
        {
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";

            quote_id = Decrypt(quote_id);

            DataTable dtquoteid = GetQtHdrDetailsByQuote_Hdr_id(quote_id);
            if (dtquoteid != null)
            {
                if (dtquoteid.Rows[0]["quote_id"].ToString() != quote_id)
                    quote_id = dtquoteid.Rows[0]["quote_id"].ToString();
            }

            DataTable dtorderdtl = new PostOrderController().GetOrders(quote_id);

            //            if (dtorderdtl != null)
            //            {
            //                SqlSelect = @"select [shipper_id]
            //                                  ,[load_inquiry_no]
            //                                  ,[inquiry_source_addr]
            //                                  ,[inquiry_destination_addr]
            //                                  , CONVERT(VARCHAR(10), CAST(load_inquiry_shipping_date AS DATE), 101) as load_inquiry_shipping_date
            //                                  , load_inquiry_shipping_time
            //                                  ,u.[first_name]+' '+u.[last_name] as customer_name
            //                                  ,RTRIM(LTRIM(u.email_id)) as customer_email
            //                                  ,u.user_id as customer_mobile
            //                                  ,[NoOfLabour]
            //                                  ,[NoOfHandiman]
            //                                  ,[NoOfTruck]
            //                                  ,[TotalDistance]
            //                                  ,[Total_cost]
            //                                  ,[appartment_id]
            //                                  ,a.appartment_desc AS room_type
            //                            from orders as o
            //                            inner join user_mst as u on u.unique_id=o.shipper_id  
            //                            inner join appartment_type_mst as a on a.SizeTypeCode=o.[SizeTypeCode] 
            //                            where [load_inquiry_no]='" + quote_id + @"'";

            //            }

            if (dtorderdtl != null)
            {
                SqlSelect = @"select [shipper_id]
                                  ,[load_inquiry_no]
                                  ,[inquiry_source_addr]
                                  ,[inquiry_destination_addr]
                                  , CONVERT(VARCHAR(10), CAST(load_inquiry_shipping_date AS DATE), 101) as load_inquiry_shipping_date
                                  , load_inquiry_shipping_time
                                  ,u.[first_name]+' '+u.[last_name] as customer_name
                                  ,RTRIM(LTRIM(u.email_id)) as customer_email
                                  ,u.user_id as customer_mobile
                                  ,[NoOfLabour]
                                  ,[NoOfHandiman]
                                  ,[NoOfTruck]
                                  ,[TotalDistance]
                                  ,[Total_cost]
                                  ,qh.appartment_id
                                  ,a.appartment_desc AS room_type
                            from orders as o
                            inner join user_mst as u on u.unique_id=o.shipper_id  
                            inner join quote_hdr as qh on qh.quote_id=o.load_inquiry_no 
                            inner join appartment_type_mst as a on a.appartment_id=qh.appartment_id 
                            where [load_inquiry_no]='" + quote_id + @"'";
            }
            else
            {
                SqlSelect = @"select  a.appartment_id,a.appartment_desc,* from quote_hdr 
                              inner join appartment_type_mst as a on a.appartment_id=quote_hdr.appartment_id where quote_hdr.quote_hdr_id ='" + quote_id + @"' ";
            }

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
                return null;
            }
        }
        public DataTable GetAllItemdt(string room)
        {

            var Flag = "Y";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            //            SqlSelect = @"select * from(SELECT ROW_NUMBER() over (order by item_code) as srNo,cast(item_code as nvarchar(50)) as item_code, company_code, owner, item_mst.location_id, item_name, isnull(item_cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
            //                        item_mst.location_id as RoomSrNo ,case When [location_desc] like 'Bedroom%' then SUBSTRING([location_desc], 9,Len([location_desc]) ) 
            //                        else 0 end as bedroomSize   FROM   item_mst inner join location_mst on item_mst.location_id=location_mst.location_id
            //                        )as y where  active_flag= '" + Flag + "' and bedroomSize <=" + room;

            SqlSelect = @"select *                            
                            from(SELECT ROW_NUMBER() over (order by item_mst.item_code) as srNo,cast(item_mst.item_code as nvarchar(50)) as item_code,                             
                            company_code, owner, item_mst.location_id, item_name, isnull(cbm,0) AS item_cbm, active_flag, 0 as item_qty,'Y' as ItemFlg,
                            item_mst.location_id as RoomSrNo ,
                            case When [location_display] like 'Bedroom%' then SUBSTRING([location_display], 9,Len([location_display]) ) 
                            else 0 end as bedroomSize ,Default_flag  
                            FROM   item_mst
                            right join item_mst_for_dropdown on  item_mst.item_code=item_mst_for_dropdown.item_code
                            right join location_mst on item_mst.location_id=location_mst.location_id    
                            right join location_app_mapping  on location_app_mapping.location_id=location_mst.location_id  and appartment_id=" + room + @"                        
                            )as y                           
                            where  active_flag= '" + Flag + "' ";
            //and bedroomSize <=" + room;



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
                return null;
            }
        }
        public DataTable dtcheck_quote_id_is_available(string quote_id)
        {
            quote_id = Decrypt(quote_id);

            DataTable dtquoteid = GetQtHdrDetailsByQuote_Hdr_id(quote_id);
            if (dtquoteid != null)
            {
                if (dtquoteid.Rows[0]["quote_id"].ToString() != quote_id)
                    quote_id = dtquoteid.Rows[0]["quote_id"].ToString();
            }

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            //            SqlSelect = @"SELECT     quote_id, customer_name, customer_mobile, customer_email, total_cbm, bedroom, otp, StatusFlag,appartment_id,appartment_desc
            //                            FROM         quote_hdr 
            //                            inner join appartment_type_mst on appartment_type_mst.appartment_desc=quote_hdr.bedroom
            //                        WHERE     --(StatusFlag = 'f') AND 
            //                    quote_id = '" + quote_id + "'";

            SqlSelect = @"SELECT  appartment_type_mst.appartment_desc,quote_hdr.*
                        FROM    quote_hdr  inner join appartment_type_mst on
                        appartment_type_mst.appartment_id=quote_hdr.appartment_id   WHERE      
                        quote_id = '" + quote_id + "'";

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
                return null;
            }
        }

        public DataTable GetQuote_hdr_details(string opt, string quote_id)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "";
            if (opt == "0")
                query1 = @"SELECT  *  FROM  quote_hdr where is_assign_to_order='N' order by created_date desc ";
            else if (opt == "2")
                query1 = @"SELECT  *  FROM  quote_hdr  where quote_hdr_id = '" + quote_id + "'";
            else
                query1 = @"SELECT  *  FROM  quote_hdr  where quote_id = '" + quote_id + "'";

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


            //DBDataAdpterObject.SelectCommand.Parameters.Clear();
            //String SqlSelect = "";
            ////appartment type master
            //SqlSelect = @"SELECT  *  FROM  quote_hdr  where quote_id='" + quote_id + "'";

            //DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;

            //DataSet ds = new DataSet();
            //try
            //{
            //    DBDataAdpterObject.Fill(ds);
            //    if (ds.Tables[0].Rows.Count <= 0)
            //        return null;
            //    else
            //        return ds.Tables[0];
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        [HttpGet]
        public DataTable GetQuote_dtl_details(string opt, string quote_id)
        {
            DataTable dtPostLoadOrders = new DataTable();
            //String query1 = @"SELECT  *  FROM  quote_dtl  where (quote_id='" + quote_id + "' or quote_hdr_id='" + quote_id + "')";

            String query1 = "";
            if (opt == "0")
                query1 = @"SELECT  *  FROM  quote_dtl where quote_id = '" + quote_id + "' ";
            else
                query1 = @"select * from (select tbl1.*,'a' orderkey from (
                            select location_mst.displayseq,case ISNULL(location_mst.location_display,'') when '' then quote_dtl.location_id else location_mst.location_display end as location_display,
                            case ISNULL(item_mst_for_dropdown.item_name,'') when '' then quote_dtl.item_code else item_mst_for_dropdown.item_name end as item_name,
                            quote_dtl.quote_dtl_id,quote_dtl.quote_id,quote_dtl.item_code,quote_dtl.item_cbm,quote_dtl.item_qty,quote_dtl.location_id,quote_dtl.ItemFlg 
                            from  quote_dtl 
                            left join item_mst_for_dropdown on item_mst_for_dropdown.item_code=quote_dtl.item_code
                            left join location_mst on location_mst.location_id=quote_dtl.location_id  
                            where quote_id = '" + quote_id + @"' and location_mst.DisplaySeq is Not null  
                            ) as tbl1
                            UNION ALL
                            select tbl2.*,'b' orderkey from (
                            select location_mst.displayseq,case ISNULL(location_mst.location_display,'') when '' then quote_dtl.location_id else location_mst.location_display end as location_display,
                            case ISNULL(item_mst_for_dropdown.item_name,'') when '' then quote_dtl.item_code else item_mst_for_dropdown.item_name end as item_name,
                            quote_dtl.quote_dtl_id,quote_dtl.quote_id,quote_dtl.item_code,quote_dtl.item_cbm,quote_dtl.item_qty,quote_dtl.location_id,quote_dtl.ItemFlg from  quote_dtl 
                            left join item_mst_for_dropdown on item_mst_for_dropdown.item_code=quote_dtl.item_code
                            left join location_mst on location_mst.location_id=quote_dtl.location_id  
                            where quote_id = '" + quote_id + @"' and location_mst.DisplaySeq is null 
                            ) as tbl2) as Temp order by OrderKey,Temp.displayseq asc	";



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

        public DataTable GetQtHdrDetailsByQuote_Hdr_id(string quote_hdr_id)
        {
            DataTable dtPostLoadOrders = new DataTable();
            String query1 = "";
            query1 = @"SELECT  *  FROM  quote_hdr  where quote_hdr_id = '" + quote_hdr_id + "'";

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

        public DataTable GetLocationDetails(string quote_id)
        {
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            SqlSelect = @"select location_mst.location_desc,item_mst_for_dropdown.item_name,* from  quote_dtl 
                join item_mst_for_dropdown on item_mst_for_dropdown.item_code=quote_dtl.item_code
                left join location_mst on location_mst.location_id=quote_dtl.location_id   where quote_id='" + quote_id + "'";

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
                return null;
            }
        }

        [HttpGet]
        public string quote_details(string quote_id)
        {
            DataTable dt = new DataTable();
            dt = quote_details_dt(quote_id);

            if (dt != null && dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            else
            {
                return null;
            }
        }

        public void SendMail(string email_id)
        {
            try
            {
                string errorMsg = "";
                //string MailBodyTrukker = "<html><head></head><body><b>Hello,</b><br/>"+ email_id + " is connected to FlexiBuilder</body></html>";
                string MailBodyTrukker = "<html><head></head><body>" + email_id + " has shown intrest in Flexibuilder</body></html>";
                SendMail_to_flexi(email_id.ToString().Trim(), MailBodyTrukker, "Flexibuilder", ref errorMsg, "Flexibuilder");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.StackTrace + "error in send email" + ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        public void SendMail_with_detail(string email_id, string detail)
        {
            try
            {
                string errorMsg = "";

                //string MailBodyTrukker = "<html><head></head><body><b>Hello,</b><br/>"+ email_id + " is connected to FlexiBuilder</body></html>";
                string MailBodyTrukker = "<html><head></head><body>" + email_id + " has shown intrest in Flexibuilder<br>Message :- " + detail + "</body></html>";
                SendMail_to_flexi(email_id.ToString().Trim(), MailBodyTrukker, "Flexibuilder", ref errorMsg, "Flexibuilder");
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.StackTrace + "error in send email" + ex.ToString());
                throw ex;
            }
        }

        public Boolean SendMail(string to, string msg, string subject, ref string errmsg, string Title, string AttachfilePath)
        {
            try
            {

                string smtphst = ConfigurationManager.AppSettings["EmailHost"].ToString();
                string smtpprt = ConfigurationManager.AppSettings["EmailPort"].ToString();

                if (smtphst == "")
                {
                    errmsg = "SMTP HOST settings not found . . . ";
                    return false;
                }
                if (smtpprt == "")
                {
                    errmsg = "SMTP Port settings not found . . . ";
                    return false;
                }


                DataTable dt_param = GetParameter("CONTACT", "SENDFROM", ref msg);
                if (dt_param == null)
                {
                    ServerLog.Log(msg);
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();

                }

                string fromemail = dt_param.Rows[0]["param_value"].ToString().Trim();
                string frompword = dt_param.Rows[0]["Remark"].ToString().Trim();

                SmtpClient sendmail = new SmtpClient();
                sendmail.Credentials = new System.Net.NetworkCredential(fromemail, frompword);
                sendmail.Port = Convert.ToInt16(smtpprt);
                sendmail.Host = smtphst;
                sendmail.EnableSsl = true;

                MailMessage mail = new MailMessage();
                if (Title != "")
                    mail.From = new MailAddress(fromemail, Title);
                else
                    mail.From = new MailAddress(fromemail, "Trukker Technologies");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                if (AttachfilePath != null)
                    mail.Attachments.Add(new Attachment(AttachfilePath));

                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(AttachfilePath);
                //mail.Attachments.Add(attachment);

                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                try
                {
                    sendmail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sendmail.Send(mail);
                    ServerLog.Log(to + "email send successfully");

                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message);
                    errmsg = ex.Message;
                    return false;
                    ServerLog.Log("error in email sending to " + to);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        //[HttpGet]
        //public string delete_quoteHdr(string quote)
        //{
        //    //quote = Decrypt(quote);
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
        //    con.Open();
        //    // DELETE FROM quote_hdr WHERE     (quote_id = '1') AND (StatusFlag <> 'f')
        //    String query = " DELETE  FROM quote_hdr where quote_id = '" + quote + "' and StatusFlag != 'F' ";
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand(query, con);
        //        int temp = cmd.ExecuteNonQuery();
        //        con.Close();
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "allready submited";
        //    }
        //}

        //delete all detail of quote
        [HttpGet]
        public DataTable delete_dtl(string quote)
        {
            //quote = Decrypt(quote);

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            con.Open();
            String query = " DELETE FROM quote_dtl WHERE quote_id = '" + quote + "'";
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
                int temp = cmd.ExecuteNonQuery();
                con.Close();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public bool GenerateCBMPdf(string quoteid)
        {
            string domian = ConfigurationManager.AppSettings["Domain"];
            string CBMPDfFilepath = ConfigurationManager.AppSettings["CBM_PDF_Path"];
            StreamReader sr; string LINK = "";

            ServerLog.Log("dtorder not found");

            string strdata = ""; string msg = "";
            string TITLE = "CBM details"; bool bl = true;
            string[] strBillingAdd = new string[] { };

            DataTable dtorder = new PostOrderController().GetLoadInquiryById(quoteid);
            DataTable dtquotehdr = GetQuote_hdr_details("1", quoteid);
            DataTable dtQuoteDtl = GetQuote_dtl_details("1", quoteid);

            //  DataTable dtLocationIDs = GetLocationDetails(quoteid);

            sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_GenerateCBMPDF.html");
            while (!sr.EndOfStream)
            {
                ServerLog.Log("Strdata In :-" + quoteid);

                string str = "";
                strdata = sr.ReadToEnd();
                if (dtorder != null)
                {
                    ServerLog.Log("dtorder found :-" + quoteid);

                    StringBuilder sborder = new StringBuilder(); ;
                    sborder.Append(" <tr><td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;  color: #646464; font-family: Helvetica; font-size: 16px; text-align: left; width: 200px;\"> ");
                    sborder.Append(" <strong>Moving From</strong><br><span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif\"> ");
                    sborder.Append("   <span style=\"font-size: 14px\">SOURCE_ADDRESS</span></span></span></td> ");
                    sborder.Append(" <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px; text-align: left; width: 200px;\">                              ");
                    sborder.Append("     <strong>Moving To</strong><br><span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif\"><span style=\"font-size: 14px\">DESTINATION_ADDRESS</span></span></span>   ");
                    sborder.Append("  </td>                                                                                                                                                                                                               ");
                    sborder.Append("  <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                             ");
                    sborder.Append("  <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/calendar.png\" width=\"20\" class=\"verticaltop\">                                                                                              ");
                    sborder.Append("    <span style=\"color: #a9a9a9\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif; padding: 10px;\"><span style=\"font-size: 14px; color: #646464;\">SHIPPINGDATE</span></span></span>          ");
                    sborder.Append("   </td>                                                                                                                                                                                                               ");
                    sborder.Append("  <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                             ");
                    sborder.Append("      <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/stopwatch.png\"width=\"20\" class=\"verticaltop\">                                                                                          ");
                    sborder.Append("            <span style=\"color: #a9a9a9\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif;padding: 10px;\"><span style=\"font-size: 14px; color: #646464;\">SHIPPINGTIME</span></span></span>    ");
                    sborder.Append("        </td>                                                                                                                                                                                                          ");
                    sborder.Append("    </tr>                                                                                                                                                                                                              ");
                    sborder.Append("    <tr>                                                                                                                                                                                                               ");
                    sborder.Append("        <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                        ");
                    sborder.Append("            <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/truck.png\" width=\"20\"class=\"verticaltop\">                                                                                         ");
                    sborder.Append("            <span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif;padding: 10px;\"><span style=\"font-size: 14px\">NOOFTRUCK</span></span></span>                       ");
                    sborder.Append("        </td>                                                                                                                                                                                                          ");
                    sborder.Append("        <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                        ");
                    sborder.Append("            <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/builder.png\" width=\"20\"class=\"verticaltop\">                                                                                       ");
                    sborder.Append("            <span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif;padding: 10px;\"><span style=\"font-size: 14px\">NOOFTRUCK</span></span></span>                      ");
                    sborder.Append("        </td>                                                                                                                                                                                                          ");
                    sborder.Append("        <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                        ");
                    sborder.Append("            <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/hammer.png\" width=\"20\"class=\"verticaltop\">                                                                                        ");
                    sborder.Append("            <span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif;padding: 10px;\"><span style=\"font-size: 14px\">NOOFLABOUR</span></span></span>                      ");
                    sborder.Append("        </td>                                                                                                                                                                                                          ");
                    sborder.Append("        <td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word;color: #646464; font-family: Helvetica; font-size: 16px;\">                                                        ");
                    sborder.Append("            <img src=\"http://trukker.ae/trukkeruaetest/movers/assets/img/icons/box.png\" width=\"20\"class=\"verticaltop\">                                                                                           ");
                    sborder.Append("            <span style=\"color: #646464;\"><span style=\"font-family: arial,helvetica neue,helvetica,sans-serif;padding: 10px;\"><span style=\"font-size: 14px\">AED TOTALCOST</span></span></span>                   ");
                    sborder.Append("        </td>                                                                                                                                                                                                          ");
                    sborder.Append("    </tr> ");


                    strdata = strdata.Replace("ORDERDETAILS", sborder.ToString());
                    ServerLog.Log("dtorder In");
                    strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy"));
                    strdata = strdata.Replace("SHIPPINGTIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("h:mm tt"));
                    strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
                    strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
                    strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
                    strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
                    strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
                    strdata = strdata.Replace("TOTALCOST", dtorder.Rows[0]["Total_cost"].ToString());
                }
                else
                {
                    ServerLog.Log("dtorder not found :-" + quoteid);
                    strdata = strdata.Replace("ORDERDETAILS", "");
                }

                if (dtquotehdr != null)
                {
                    ServerLog.Log("dtquotehdr found");
                    ServerLog.Log(dtquotehdr.Rows[0]["customer_name"].ToString());
                    ServerLog.Log(dtquotehdr.Rows[0]["customer_email"].ToString());
                    ServerLog.Log(dtquotehdr.Rows[0]["customer_mobile"].ToString());

                    strdata = strdata.Replace("USERNAME", dtquotehdr.Rows[0]["customer_name"].ToString());
                    strdata = strdata.Replace("USEREMAIL", dtquotehdr.Rows[0]["customer_email"].ToString());
                    strdata = strdata.Replace("USERMOBILE", dtquotehdr.Rows[0]["customer_mobile"].ToString());
                    strdata = strdata.Replace("ROOMTYPE", dtquotehdr.Rows[0]["room_type"].ToString());
                    strdata = strdata.Replace("TOTALCBM", dtquotehdr.Rows[0]["total_cbm"].ToString());
                }
                else
                {
                    ServerLog.Log("dtquotehdr not found :-" + quoteid);
                }



                if (dtQuoteDtl != null)
                {
                    ServerLog.Log("dtQuoteDtl found :-" + quoteid);

                    DataView view = new DataView(dtQuoteDtl);
                    DataTable distinctValues = view.ToTable(true, "location_display", "location_id");
                    for (int i = 0; i < distinctValues.Rows.Count; i++)
                    {
                        if (dtQuoteDtl != null)
                        {
                            DataRow[] drQTitem = dtQuoteDtl.Select("location_id='" + distinctValues.Rows[i]["location_id"].ToString() + "'");
                            for (int intj = 0; intj < drQTitem.Length; intj++)
                            {
                                if (drQTitem[intj]["item_qty"].ToString() != "0")
                                {
                                    //str += " <tr><td valign=\"top\" style=\"color: #000000; font-family: Helvetica; font-size: 14px; font-weight: normal; line-height: 200%; text-align: center; word-break: break-word\"> " +
                                    //       " <table style=\"width: 100%; font-size: 16px; border-collapse: collapse\"><tbody><tr><td><h3  style=\"margin: 3px; font-weight: bolder; padding-left: 12px\"><i class=\"fa fa-building-o\"></i>" + distinctValues.Rows[i]["location_desc"].ToString() + "<a></a></h3></td></tr> ";

                                    str += " <tr><td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word; color: #66464; font-family: Helvetica; font-size: 16px; text-align: left\"> " +
                                    " <div class=\"panel panel-primary\"><div class=\"panel-heading\">" + distinctValues.Rows[i]["location_display"].ToString().Replace("__", " ") + "</div>   <div class=\"panel-body\" id=\"div_1\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%; border-collapse: collapse\"> " +
                                                   " <tr><td><table id=\"tbl_1\" style=\"width: 100%;\"><tbody>";


                                    if (dtQuoteDtl != null)
                                    {
                                        DataRow[] dritem = dtQuoteDtl.Select("location_id='" + distinctValues.Rows[i]["location_id"].ToString() + "'");
                                        for (int j = 0; j < dritem.Length; j++)
                                        {
                                            if (dritem[j]["item_qty"].ToString() != "0")
                                            {
                                                //str += " <tr><td><table style=\"width: 100%;\"><tbody><tr class=\"col-xs-2 col-xs-6\"><td style=\"color: rgb(155, 155, 155); width: 100%;\">" + dritem[j]["item_name"].ToString() + "</td> " +
                                                //       " <td style=\"text-align: center; margin-top: 10px; color: rgb(155, 155, 155); width: 100%;\">" + dritem[j]["item_qty"].ToString() + "</td></tr></tbody></table></td></tr></tbody></table></td></tr> ";

                                                //str += " <tr class=\"col-xs-3 col-xs-6\"><td style=\"color: rgb(155, 155, 155); width: 90%; padding-right:5px\">" + dritem[j]["item_name"].ToString() + "</td><td style=\"text-align: center; margin-top: 10px; width: 15%;\">" + dritem[j]["item_qty"].ToString() + "</td></tr> ";
                                                //if (IsOdd(j))
                                                str += " <tr class=\"col-xs-3 col-xs-6\" style=\"Padding-top:0px\"><td style=\"color: rgb(155, 155, 155); padding-right:10px ;\" >" + dritem[j]["item_name"].ToString() + "</td><td style=\"text-align: center;\">" + dritem[j]["item_qty"].ToString() + "</td></tr> ";
                                                //else
                                                //    str += " <tr class=\"col-xs-4 col-xs-6\" style=\"PaddingMode-top:0px\"><td style=\"color: rgb(155, 155, 155); padding-right:10px;\" >" + dritem[j]["item_name"].ToString() + "</td><td style=\"text-align: center;\">" + dritem[j]["item_qty"].ToString() + "</td></tr> ";
                                            }
                                        }
                                        str += " </tbody></table></td></tr></table></div> ";
                                    }
                                    intj = drQTitem.Length;
                                }

                            }
                        }
                    }

                    str += " </div></td></tr> ";
                    if (dtquotehdr != null)
                    {
                        if (dtquotehdr.Rows[0]["notes"].ToString().Trim() != "")
                        {
                            str += "  <tr><td valign=\"top\" style=\"padding: 0px 18px 9px; line-height: 150%; word-break: break-word; color: #646464; font-family: Helvetica; font-size: 16px; text-align: left\"> " +
                                     " <div class=\"panel panel-primary\"><div class=\"panel-heading\">Notes</div><div class=\"panel-body\" id=\"div_1\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%; border-collapse: collapse\"> " +
                                     " <tbody><tr><td><table id=\"tbl_1\" style=\"width: 100%;\"><tbody><tr class=\"col-xs-12 col-xs-12\"><td style=\"color: rgb(155, 155, 155); width: 100%;\">" + dtquotehdr.Rows[0]["notes"].ToString().Trim() + "</td></tr> " +
                                     " </tbody></table></td></tr></tbody></table></div></div></td></tr> ";
                        }
                    }
                }
                else
                {
                    ServerLog.Log("dtQuoteDtl not found :-" + quoteid);
                }

                strdata = strdata.Replace("QUOTE_DETAIL_MASTER", str);
            }
            sr.Close();

            try
            {
                var htmlContent = String.Format(sr.ToString(), DateTime.UtcNow);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                //var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                var pdfBytes = htmlToPdf.GeneratePdf(strdata);
                Random rd = new Random();
                int n = rd.Next();


                // quote_id = Decrypt(quoteid);
                FileStream fs = new FileStream(Path.Combine(CBMPDfFilepath, "CBM_" + quoteid + ".pdf"), FileMode.Create);
                //FileStream fs = new FileStream(Path.Combine(PDfFilepath, "Invoicedetail_" + OrderID + ".pdf"), FileMode.Create);
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Dispose();
                fs.Close();

                ServerLog.Log("CBM PDF Save " + Path.Combine(CBMPDfFilepath, "CBM_" + quoteid + ".pdf"));
                return true;
            }
            catch (Exception Ex)
            {
                ServerLog.Log("CBM PDF" + Ex.Message);
                DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;
            }
        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        [HttpGet]
        public bool SendCBMfinalizeMail(string quote_id, string emailId)
        {
            try
            {
                //string CBMPDFPath = ConfigurationManager.AppSettings["CBM_PDF_URL"].ToString();
                //ServerLog.Log(""); ServerLog.Log(System.DateTime.Now.ToString());
                //ServerLog.Log("item_cbm email genrate start for " + emailId);
                //ReportPrinter obReportPrinter = new ReportPrinter();
                ////obReportPrinter.PageFile = Server.MapPath("/") + "Generate_Cbm_Pdf.html?quote_id=" + quote_id;
                //obReportPrinter.PageFile = CBMPDFPath + "Generate_Cbm_Pdf.html?quote_id=" + quote_id;
                //ServerLog.Log("CBM PDF page Link =" + CBMPDFPath + "Generate_Cbm_Pdf.html?quote_id=" + quote_id);
                ////obReportPrinter.PageFile = "http://trukker.ae/movers/Generate_Cbm_Pdf.html?quote_id=" + quote_id;
                //obReportPrinter.MarginBottom = "0";

                //ServerLog.Log("pdf link:-" + obReportPrinter.PageFile);
                //obReportPrinter.GetPdf();

                //Random rd = new Random();
                //int n = rd.Next();

                // quote_id = Decrypt(quote_id);


                StreamReader sr; string LINK = "";
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_SendCBMCompletationMail.html");
                string strdata = ""; string msg = "";


                DataTable dtquotehdr = GetQuote_hdr_details("1", quote_id);

                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();
                    strdata = strdata.Replace("UserName", dtquotehdr.Rows[0]["customer_name"].ToString());
                }
                sr.Close();

                //ServerLog.Log(quote_id);
                ////string filePath = Path.Combine(Server.MapPath("/GeneratedPdf/"), quote_id + "_" + n.ToString() + ".pdf");
                ////string filePath = "C:/inetpub/wwwroot/movers/GeneratedPdf/quote_id" + "_" + n.ToString() + ".pdf";
                string filePath = Path.Combine(ConfigurationManager.AppSettings["CBM_PDF_Path"].ToString(), "CBM_" + quote_id + ".pdf");

                ServerLog.Log("CBM PDF File Path=" + filePath);

                //FileStream fs = new FileStream(filePath, FileMode.Create);
                //fs.Write(obReportPrinter.FileContent, 0, obReportPrinter.FileContent.Length);
                //fs.Dispose();

                string errorMsg = "";

                //send mail

                if (dtquotehdr != null)
                {
                    DataTable dt_param = GetParameter("CONTACT", "SENDTO", ref msg);
                    if (dt_param == null)
                    {
                        ServerLog.Log(msg);
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return false;
                    }

                    //string MailBodyTrukker = "<html><head></head><body><b>Hello,</b><br/>CBM link finalized for " + quote_id + " <br/>Please find attachment.</br><b>Regards,</b></br>Trukker</body></html>";
                    //string MailBodyClient = "<html><head></head><body><b>Hello,</b><br/>Thank you for valuable time in giving details for CBM <br/>Please find attachment for final details.<br/>We will get back to you soon.</br><b>Regards,</b></br>Trukker</body></html>";

                    SendMail(dt_param.Rows[0]["param_value"].ToString().Trim(), strdata, "CBM for " + quote_id, ref errorMsg, "Trukker Technologies", filePath);
                    SendMail(dtquotehdr.Rows[0]["customer_email"].ToString().Trim(), strdata, " Hello " + dtquotehdr.Rows[0]["customer_name"].ToString() + ", Thanks for your Home Contents details to TruKKer ", ref errorMsg, "Trukker Technologies", filePath);

                    ServerLog.Log(dtquotehdr.Rows[0]["customer_email"].ToString().Trim() + "  email");
                    //Update in Quote header table
                    string filePath1 = Path.Combine(ConfigurationManager.AppSettings["CBM_PDF_URL"].ToString(), "CBM_" + quote_id + ".pdf");
                    int updateSts = Update_Quote(quote_id, filePath1);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.StackTrace + "error in send email" + ex.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return false;

            }
        }

        //[HttpGet]
        //public void GeneratePdf_SendMailNew(string quote_id, string emailId)
        //{
        //    quote_id = Decrypt(quote_id);

        //    string domian = ConfigurationManager.AppSettings["Domain"];
        //    string PDfFilepath = ConfigurationManager.AppSettings["InvoicePdfPath"];
        //    StreamReader sr; string LINK = "";

        //    string strdata = ""; string msg = "";
        //    string TITLE = "Invoice details"; bool bl = true;
        //    string[] strBillingAdd = new string[] { };

        //    DataTable dtQuoteHdr = GetQuote_hdr_details(quote_id);
        //    DataTable dtQuotedtl = GetQuote_hdr_details(quote_id);

        //    sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_OrderInvoiceDetailMailGoods.html");

        //    string shippername = new PostOrderController().GetUserdetailsByID(dtorder.Rows[0]["shipper_id"].ToString());
        //    while (!sr.EndOfStream)
        //    {
        //        strdata = sr.ReadToEnd();
        //        strdata = strdata.Replace("DOMAIN", domian);
        //        strdata = strdata.Replace("ADDONLINK", addonloadinquiry);
        //        double totalprice = Convert.ToDouble(dtorder.Rows[0]["Total_cost"].ToString());
        //        strdata = strdata.Replace("IMAGE1", ConfigurationManager.AppSettings["Domain"] + "MailerImage/OrderConfirmationHeader.jpg");
        //        strdata = strdata.Replace("IMAGE2", ConfigurationManager.AppSettings["Domain"] + "MailerImage/Bottom.jpg");
        //        strdata = strdata.Replace("DRIVERLOGO", ConfigurationManager.AppSettings["Domain"] + "MailerImage/driver.png");
        //        strdata = strdata.Replace("TRUCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/truck.png");
        //        strdata = strdata.Replace("ClOCKIMAGE", ConfigurationManager.AppSettings["Domain"] + "MailerImage/clock.png");
        //        strdata = strdata.Replace("USERNAME", dtorder.Rows[0]["billing_name"].ToString());
        //        strdata = strdata.Replace("ORDERID", dtorder.Rows[0]["load_inquiry_no"].ToString());
        //        strdata = strdata.Replace("SHIPPINGDATE", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("dd MMM yyyy h:mm tt"));
        //        strdata = strdata.Replace("SHIPPINGTIME", Convert.ToDateTime(dtorder.Rows[0]["shippingdatetime"].ToString()).ToString("h:mm tt"));
        //        strdata = strdata.Replace("SOURCE_ADDRESS", dtorder.Rows[0]["inquiry_source_addr"].ToString());
        //        strdata = strdata.Replace("DESTINATION_ADDRESS", dtorder.Rows[0]["inquiry_destination_addr"].ToString());
        //        strdata = strdata.Replace("NOOFTRUCK", dtorder.Rows[0]["NoOfTruck"].ToString());
        //        strdata = strdata.Replace("NOOFHANDIMAN", dtorder.Rows[0]["NoOfHandiman"].ToString());
        //        strdata = strdata.Replace("NOOFLABOUR", dtorder.Rows[0]["NoOfLabour"].ToString());
        //        strdata = strdata.Replace("TOTALCOST", totalprice.ToString());
        //        strdata = strdata.Replace("TRACKURL", dtorder.Rows[0]["trackurl"].ToString());
        //        strdata = strdata.Replace("CBMLINK", dtorder.Rows[0]["item_cbmlink"].ToString());
        //        strdata = strdata.Replace("TOTALDISTANCE", dtorder.Rows[0]["TotalDistance"].ToString());
        //        strdata = strdata.Replace("DISTANCEUOM", dtorder.Rows[0]["TotalDistanceUOM"].ToString());
        //        strdata = strdata.Replace("INVOICEDATE", new PostOrderController().DatetimeUTC_ToDubaiConverter("", DateTime.UtcNow).ToString("dd MMM yyyy"));
        //        strdata = strdata.Replace("SOURCELAT", dtorder.Rows[0]["inquiry_source_lat"].ToString());
        //        strdata = strdata.Replace("SOURCELONGS", dtorder.Rows[0]["inquiry_source_lng"].ToString());
        //        strdata = strdata.Replace("DESTLAT", dtorder.Rows[0]["inquiry_destionation_lat"].ToString());
        //        strdata = strdata.Replace("DESTILONGS", dtorder.Rows[0]["inquiry_destionation_lng"].ToString());
        //        strdata = strdata.Replace("LATLONGS", dtorder.Rows[0]["BaseRate"].ToString() + "," + dtorder.Rows[0]["inquiry_source_lng"].ToString() + "|" + dtorder.Rows[0]["inquiry_destionation_lat"].ToString() + "," + dtorder.Rows[0]["inquiry_destionation_lng"].ToString());

        //        if (dtorder.Rows[0]["rate_type_flag"].ToString() == "P")
        //        {
        //            strdata = strdata.Replace("BASERATE", dtorder.Rows[0]["TotalTravelingRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["BaseRate"].ToString() : "");
        //            strdata = strdata.Replace("TOTALTRAVELINGRATE", dtorder.Rows[0]["TotalTravelingRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalTravelingRate"].ToString() : "");
        //            strdata = strdata.Replace("TOTALDRIVERRATE", dtorder.Rows[0]["TotalDriverRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalDriverRate"].ToString() : "");
        //            strdata = strdata.Replace("TOTALLABOURRATE", dtorder.Rows[0]["TotalLabourRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalLabourRate"].ToString() : "");
        //            strdata = strdata.Replace("TOTALHANDIMANRATE", dtorder.Rows[0]["TotalHandimanRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalHandimanRate"].ToString() : "");
        //        }
        //        else
        //        {
        //            strdata = strdata.Replace("BASERATE", "AED  " + dtorder.Rows[0]["BaseRate"].ToString());
        //            strdata = strdata.Replace("TOTALTRAVELINGRATE", "AED  " + dtorder.Rows[0]["TotalTravelingRate"].ToString());
        //            strdata = strdata.Replace("TOTALDRIVERRATE", "AED  " + dtorder.Rows[0]["TotalDriverRate"].ToString());
        //            strdata = strdata.Replace("TOTALLABOURRATE", "AED  " + dtorder.Rows[0]["TotalLabourRate"].ToString());
        //            strdata = strdata.Replace("TOTALHANDIMANRATE", "AED  " + dtorder.Rows[0]["TotalHandimanRate"].ToString());
        //        }
        //        strBillingAdd = dtorder.Rows[0]["billing_add"].ToString().Split('^');
        //        if (strBillingAdd.Length > 0)
        //        {
        //            strdata = strdata.Replace("BILLINGADD1", strBillingAdd[0] + "," + strBillingAdd[1]);
        //            if (strBillingAdd.Length == 3)
        //                strdata = strdata.Replace("BILLINGADD2", strBillingAdd[2]);
        //            else
        //                strdata = strdata.Replace("BILLINGADD2", "");
        //        }

        //        strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalSupervisorRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalSupervisorRate"].ToString() : "");
        //        strdata = strdata.Replace("TOTALSUPERVISORRATE", dtorder.Rows[0]["TotalDriverRate"].ToString() != "0" ? "AED  " + dtorder.Rows[0]["TotalSupervisorRate"].ToString() : "");



        //        if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDER_TYPE_CODE_HIRETRUCK)
        //        {
        //            int noofdays = dtorder.Rows[0]["Hiretruck_NoofDay"].ToString() != "" ? Convert.ToInt32(dtorder.Rows[0]["Hiretruck_NoofDay"].ToString()) : 1;

        //            strdata = strdata.Replace("NOOFDAYS", noofdays.ToString());
        //            strdata = strdata.Replace("MOVINGTYPE", "Hire Truck");

        //            if (dtorder.Rows[0]["Hiretruck_IncludingFuel"].ToString() == Constant.Flag_Yes)
        //                strdata = strdata.Replace("FUELCHARGES", "<tr><td style=\"text-align: left\">Fuel Charge</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["Hiretruck_TotalFuelRate"].ToString())).ToString() + "</td></tr>");
        //            else
        //                strdata = strdata.Replace("FUELCHARGES", "");

        //            if (noofdays > 1)
        //                strdata = strdata.Replace("TODATE", "<b> To </b> <span style=\"font-size: 15px\"><span style=\"color: #a9a9a9\">" + Convert.ToDateTime(dtorder.Rows[0]["Hiretruck_To_datetime"].ToString()).ToString("dd MMM yyyy") + "</span></span>");
        //            else
        //                strdata = strdata.Replace("TODATE", "");


        //        }

        //        else if (dtorder.Rows[0]["order_type_flag"].ToString() == Constant.ORDERTYPECODEFORHOME)
        //            strdata = strdata.Replace("MOVINGTYPE", "Moving Home");
        //        else
        //            strdata = strdata.Replace("MOVINGTYPE", "Moving Goods");


        //        if (dtorder.Rows[0]["rate_type_flag"].ToString() != "P")
        //        {
        //            if (dtorder.Rows[0]["IncludePackingCharge"].ToString() == "Y")
        //                strdata = strdata.Replace("PACKINGCHARGE", "AED  " + Math.Round(Convert.ToDecimal(dtorder.Rows[0]["TotalPackingCharge"].ToString())).ToString());
        //            else
        //                strdata = strdata.Replace("PACKINGCHARGE", "AED 00");
        //        }
        //        else
        //        {
        //            strdata = strdata.Replace("PACKINGCHARGE", "");
        //        }



        //        if (dtorder != null)
        //        {
        //            // string str = "";
        //            decimal Discount = Convert.ToDecimal(dtorder.Rows[0]["Discount"].ToString());
        //            if (Discount != 0)
        //            {
        //                strdata = strdata.Replace("DISCOUNT", Discount.ToString());
        //            }

        //        }


        //        if (dtorder != null)
        //        {
        //            string str = "";
        //            decimal Remaningamt = Convert.ToDecimal(dtorder.Rows[0]["rem_amt_to_receive"].ToString());
        //            decimal Totalcostwithoutdiscount = Convert.ToDecimal(dtorder.Rows[0]["Total_cost_without_discount"].ToString());
        //            decimal Recievedamt = Totalcostwithoutdiscount - Remaningamt;
        //            if (Remaningamt == 0)
        //            {
        //                if (dtorder.Rows[0]["payment_mode"].ToString() == "O")
        //                {
        //                    str += "<td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\">Online Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td>";
        //                }
        //                else
        //                {
        //                    str += "<td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td>";
        //                }
        //                strdata = strdata.Replace("PAYMENTDETAIL", str);
        //            }
        //            else
        //            {
        //                str += "<tr><td style=\"text-align:left\">Total Amount</td><td style=\"text-align: left\"></td><td><strong></strong></td><td style=\"text-align: right\">AED " + Math.Round(Totalcostwithoutdiscount).ToString() + "</td><tr>";
        //                str += "<tr><td style=\"text-align:left\">Paid amount</td><td style=\"text-align: left\">Cash Payment</td><td><strong></strong></td><td style=\"text-align: right\"> - AED " + Math.Round(Recievedamt).ToString() + "</td><tr>";
        //                str += "<tr><td style=\"text-align:left\">Remaining Amount</td><td style=\"text-align: left\"></td><td><strong></strong></td><td style=\"text-align: right;border-top:1px solid #AAAAAA;\">AED " + Math.Round(Remaningamt).ToString() + "</td><tr>";

        //                strdata = strdata.Replace("PAYMENTDETAIL", str);
        //            }

        //        }
        //    }
        //    sr.Close();

        //    try
        //    {
        //        var htmlContent = String.Format(sr.ToString(), DateTime.UtcNow);
        //        var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
        //        //var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
        //        var pdfBytes = htmlToPdf.GeneratePdf(strdata);

        //        FileStream fs = new FileStream(Path.Combine(PDfFilepath, "Invoicedetail_" + OrderID + ".pdf"), FileMode.Create);
        //        fs.Write(pdfBytes, 0, pdfBytes.Length);
        //        fs.Dispose();
        //        fs.Close();


        //        return true;
        //    }
        //    catch (Exception Ex)
        //    {
        //        ServerLog.Log("Invoice PDF" + Ex.Message);
        //        DBCommand.Transaction.Rollback();
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //        return false;
        //    }
        //}


        public int Update_Quote(string quote, string Link)
        {
            //quote = Decrypt(quote);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            con.Open();
            // DELETE FROM quote_hdr WHERE     (quote_id = '1') AND (StatusFlag <> 'f')
            String query = " Update quote_hdr SET GeneratedPdfLink='" + Link + "' where quote_id = '" + quote + "' and StatusFlag = 'F' ";
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
                int temp = cmd.ExecuteNonQuery();
                return temp;

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                con.Close();
            }
        }

        #region MyRegion

        //5 august by nirav
        public DataTable GetAppartment()
        {

            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            SqlSelect = "SELECT company_code, appartment_id, appartment_desc,NoOfRooms, created_by, created_date, created_host, last_modified_by, last_modified_date, last_modified_host FROM appartment_type_mst where cancel_flag ='N' ";

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
                return null;
            }
        }

        //5 august by nirav
        public DataTable GetLocation(string app_id)
        {
            var Flag = "N";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            //            SqlSelect = @"select ROW_NUMBER() over (order by cast(location_id as int)) as srNo,* 
            //                        from (SELECT location_mst.location_id,location_display as location_desc,parent_location_id,cancel_flag ,appartment_id,
            //                        case When [location_desc] like 'Bedroom%' then SUBSTRING([location_desc], 9,Len([location_desc]) ) else 0 end as bedroomSize 
            //                        FROM   location_mst inner join dbo.location_app_mapping as m on m.location_id=location_mst.location_id) as t where cancel_flag= '" + Flag + "' and appartment_id=" + app_id + " order by location_desc";

            SqlSelect = @"select t.location_id as srNo ,* 
                        from (SELECT location_mst.location_id,location_display as location_desc,parent_location_id,cancel_flag ,appartment_id,
                        case When [location_desc] like 'Bedroom%' then SUBSTRING([location_desc], 9,Len([location_desc]) ) else 0 end as bedroomSize ,DisplaySeq
                        FROM   location_mst inner join dbo.location_app_mapping as m on m.location_id=location_mst.location_id) as t where cancel_flag= '" + Flag + "' and appartment_id=" + app_id + " order by DisplaySeq";


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
                return null;
            }
        }

        //5 august by nirav
        public DataTable save_data(string upload)
        {
            var Flag = "Y";
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            SqlSelect = "SELECT item_code, company_code, owner, location_id, item_name, isnull(item_cbm,0) AS item_cbm, active_flag, created_by, created_date, created_host, last_modified_by, last_modified_date,last_modified_host FROM   item_mst where  active_flag= '" + Flag + "'  ";

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
                return null;
            }
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "trkr2016";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "trkr2016";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        public DataTable Get_item_dropdown(string location_id)
        {
            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";

            if (location_id == null)
            {
                SqlSelect = @"SELECT     item_mst_for_dropdown.item_code, item_mst_for_dropdown.company_code, item_mst_for_dropdown.owner, item_mst_for_dropdown.item_name, 
                          item_mst_for_dropdown.cbm as item_cbm, item_mst_for_dropdown.active_flag, item_mst_for_dropdown.created_by, item_mst_for_dropdown.created_date, 
                          item_mst_for_dropdown.created_host, item_mst_for_dropdown.last_modified_by, item_mst_for_dropdown.last_modified_date, 
                          item_mst_for_dropdown.last_modified_host, item_mst_for_dropdown.seq_no, ISNULL(item_mst.Default_flag, '') AS default_flag
                        FROM         (SELECT     item_code, location_id, Default_flag, created_by, created_date, created_host, last_modified_by, last_modified_date, last_modified_host
                                                FROM          item_mst AS item_mst_1
                                                WHERE    (location_id = NULL) ) AS item_mst RIGHT OUTER JOIN
                                                item_mst_for_dropdown ON item_mst.item_code = item_mst_for_dropdown.item_code
                        WHERE     (item_mst.Default_flag = 'N') OR
                                                (item_mst.Default_flag IS NULL)
                        ORDER BY item_mst_for_dropdown.item_name";
            }
            else
            {
                //SqlSelect = @"SELECT     seq_no, item_code, company_code, owner, location_id, item_name, item_cbm, active_flag, created_by, created_date, created_host, last_modified_by, last_modified_date,  last_modified_host, seq_no AS Expr1 FROM         item_mst_for_dropdown WHERE     (location_id <> '')";

                SqlSelect = @"SELECT     item_mst_for_dropdown.item_code, item_mst_for_dropdown.company_code, item_mst_for_dropdown.owner, item_mst_for_dropdown.item_name, 
                          item_mst_for_dropdown.cbm as item_cbm, item_mst_for_dropdown.active_flag, item_mst_for_dropdown.created_by, item_mst_for_dropdown.created_date, 
                          item_mst_for_dropdown.created_host, item_mst_for_dropdown.last_modified_by, item_mst_for_dropdown.last_modified_date, 
                          item_mst_for_dropdown.last_modified_host, item_mst_for_dropdown.seq_no, ISNULL(item_mst.Default_flag, '') AS default_flag
                        FROM         (SELECT     item_code, location_id, Default_flag, created_by, created_date, created_host, last_modified_by, last_modified_date, last_modified_host
                                                FROM          item_mst AS item_mst_1
                                                WHERE      (location_id = '" + location_id + @"') ) AS item_mst RIGHT OUTER JOIN
                                                item_mst_for_dropdown ON item_mst.item_code = item_mst_for_dropdown.item_code
                        WHERE     (item_mst.Default_flag = 'N') OR
                                                (item_mst.Default_flag IS NULL)
                        ORDER BY item_mst.Default_flag DESC,item_mst_for_dropdown.item_name";
            }
            DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
            //DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@location_id", DbType.String, location_id));

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
                return null;
            }
        }

        public DataTable quote_data_new(string quote_id)
        {


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master

            //SqlSelect = @"SELECT     quote_id, customer_name, customer_mobile, customer_email, item_cbm, bedroom, otp, StatusFlag,appartment_id
            //                FROM         quote_hdr 
            //                inner join appartment_type_mst on appartment_type_mst.appartment_desc=quote_hdr.bedroom we";


            SqlSelect = @"   SELECT quote_hdr.quote_id, quote_hdr.customer_name, quote_hdr.customer_mobile, quote_hdr.customer_email, quote_hdr.total_cbm, quote_hdr.room_type, quote_hdr.otp, 
                      quote_hdr.StatusFlag, appartment_type_mst.appartment_id
FROM         quote_hdr INNER JOIN
                      appartment_type_mst ON appartment_type_mst.appartment_id = quote_hdr.appartment_id WHERE    quote_id = '" + quote_id + "'";

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
                return null;
            }
        }

        public DataTable GetParameter(string paramcode, string paramvalue, ref string msg)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from param_mst Where param_code = @paracode and param_key = @parvalue ";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("paracode", DbType.String, paramcode));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("parvalue", DbType.String, paramvalue));
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                    {
                        msg = "Data Not Found";
                        return null;
                    }
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return null;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public Boolean SendMail_to_flexi(string to, string msg, string subject, ref string errmsg, string Title)
        {
            try
            {
                string smtphst = ConfigurationManager.AppSettings["EmailHost"].ToString();
                string smtpprt = ConfigurationManager.AppSettings["EmailPort"].ToString();

                if (smtphst == "")
                {
                    errmsg = "SMTP HOST settings not found . . . ";
                    return false;
                }
                if (smtpprt == "")
                {
                    errmsg = "SMTP Port settings not found . . . ";
                    return false;
                }



                string fromemail = ConfigurationManager.AppSettings["Flexi_Email_from"].ToString();
                string frompword = ConfigurationManager.AppSettings["flexi_pass"].ToString();

                string tomail = ConfigurationManager.AppSettings["Flexi_Email_to"].ToString();




                SmtpClient sendmail = new SmtpClient();
                sendmail.Credentials = new System.Net.NetworkCredential(fromemail, frompword);
                sendmail.Port = Convert.ToInt16(smtpprt);
                sendmail.Host = smtphst;
                sendmail.EnableSsl = true;

                MailMessage mail = new MailMessage();
                if (Title != "")
                    mail.From = new MailAddress(fromemail, Title);
                else
                    mail.From = new MailAddress(fromemail, "FlexiBuilder");
                mail.To.Add(tomail);
                //mail.CC.Add("abc@gmail.com");
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;


                try
                {
                    sendmail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sendmail.Send(mail);
                }
                catch (Exception ex)
                {
                    ServerLog.Log(ex.Message);
                    errmsg = ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message);
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        #endregion


        [HttpPost]
        public string SaveCBMDetails([FromBody]JObject objPostOrder)
        {

            DS_CBM objDsCbm = new DS_CBM();
            Master objmaster = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            CBMDetails lstCbmDetails = new CBMDetails();
            Document objdoc = new Document();
            List<CBM_QuoteDtl> objlstQuotedtl = new List<CBM_QuoteDtl>();
            List<CBM_Quotehdr> objlstQuotehdr = new List<CBM_Quotehdr>();

            ServerLog.Log("SaveCBMDetails(" + Convert.ToString(objPostOrder) + ")");

            string quote = ""; string DocNtficID = ""; string QuoteDTLDocNtficID = ""; string Message = "";
            DataTable dtQuoteDtl = new DataTable(); DataTable dtQuotehdr = new DataTable();

            if (objPostOrder != null)
            {
                lstCbmDetails = objPostOrder.ToObject<CBMDetails>();
                if (lstCbmDetails.QuoteDtl.Count != 0)
                {
                    dtQuoteDtl = BLGeneralUtil.ToDataTable(lstCbmDetails.QuoteDtl);
                    dtQuoteDtl = BLGeneralUtil.CheckDateTime(dtQuoteDtl);
                }

                if (lstCbmDetails.QuoteHdr.Count != 0)
                {
                    dtQuotehdr = BLGeneralUtil.ToDataTable(lstCbmDetails.QuoteHdr);
                    dtQuotehdr = BLGeneralUtil.CheckDateTime(dtQuotehdr);
                }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Data Not Found ");
            }

            if (objPostOrder["quote_id"].ToString() != "")
                quote = Decrypt(objPostOrder["quote_id"].ToString());

            DataTable dtquoteid = GetQtHdrDetailsByQuote_Hdr_id(quote);
            if (dtquoteid != null)
            {
                if (dtquoteid.Rows[0]["quote_id"].ToString() != quote)
                    quote = dtquoteid.Rows[0]["quote_id"].ToString();
            }

            DataTable dtqtHdrDetails = GetQuote_hdr_details("1", quote);
            //if (dtqtHdrDetails != null)
            //{
            //if (dtqtHdrDetails.Rows[0]["StatusFlag"].ToString() == "F")
            //    return BLGeneralUtil.return_ajax_string("0", " CBM Already Submited !!");
            //else
            //{
            try
            {
                if (dtQuotehdr != null)
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    objDsCbm.EnforceConstraints = false;

                    if (dtqtHdrDetails == null)
                    {
                        if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QHD", "", "", ref DocNtficID, ref Message)) // New Driver Notification ID
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        }

                        objDsCbm.quote_hdr.ImportRow(dtQuotehdr.Rows[0]);
                        objDsCbm.quote_hdr[0].quote_hdr_id = DocNtficID;
                        objDsCbm.quote_hdr[0].quote_id = quote;
                        objDsCbm.quote_hdr[0].cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(DocNtficID);
                        objDsCbm.quote_hdr[0].created_date = DateTime.UtcNow;
                    }
                    else
                    {

                        objDsCbm.quote_hdr.ImportRow(dtqtHdrDetails.Rows[0]);
                        objDsCbm.quote_hdr[0].quote_id = quote;
                        objDsCbm.quote_hdr[0].customer_name = dtQuotehdr.Rows[0]["customer_name"].ToString();
                        objDsCbm.quote_hdr[0].customer_email = dtQuotehdr.Rows[0]["customer_email"].ToString();
                        objDsCbm.quote_hdr[0].customer_mobile = dtQuotehdr.Rows[0]["customer_mobile"].ToString();
                        objDsCbm.quote_hdr[0].room_type = dtQuotehdr.Rows[0]["room_type"].ToString();
                        objDsCbm.quote_hdr[0].total_cbm = Convert.ToDecimal(dtQuotehdr.Rows[0]["total_cbm"].ToString());
                        objDsCbm.quote_hdr[0].appartment_id = dtQuotehdr.Rows[0]["appartment_id"].ToString();
                        objDsCbm.quote_hdr[0].StatusFlag = dtQuotehdr.Rows[0]["StatusFlag"].ToString();
                        objDsCbm.quote_hdr[0].notes = dtQuotehdr.Rows[0]["notes"].ToString();


                    }

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
                }


                if (dtQuoteDtl != null)
                {
                    delete_dtl(quote);

                    objDsCbm.EnforceConstraints = false;
                    for (int i = 0; i < dtQuoteDtl.Rows.Count; i++)
                    {
                        if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QD", "", "", ref QuoteDTLDocNtficID, ref Message)) // New Driver Notification ID
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        }
                        objDsCbm.quote_dtl.ImportRow(dtQuoteDtl.Rows[i]);
                        objDsCbm.quote_dtl[i].quote_dtl_id = QuoteDTLDocNtficID;
                        objDsCbm.quote_dtl[i].created_date = DateTime.UtcNow;
                        objDsCbm.quote_dtl[i].quote_id = quote;
                    }
                    objDsCbm.EnforceConstraints = true;


                    objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_dtl, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }

                    //DBCommand.Transaction.Commit();
                    //if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //return BLGeneralUtil.return_ajax_string("1", " Data Save Sucessfully ");
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //return BLGeneralUtil.return_ajax_string("1", " Data Save Sucessfully ");

                if (lstCbmDetails.QuoteHdr[0].StatusFlag == "F")
                {
                    ServerLog.Log("Generate CBM PDF Start" + quote);
                    GenerateCBMPdf(quote);
                    ServerLog.Log("Generate CBM PDF Done " + quote);
                    ServerLog.Log("Send CBM Mail start " + quote);
                    SendCBMfinalizeMail(quote, lstCbmDetails.QuoteHdr[0].customer_email);
                    ServerLog.Log("Send CBM Mail start " + quote);
                    return BLGeneralUtil.return_ajax_string("1", "Hey! Thanks for taking the time to send us your CBM details. Can't wait to give you the best move!!");
                }
                else
                {
                    ServerLog.Log("saved as draft successfully");
                    return BLGeneralUtil.return_ajax_string("1", "Saved Successfully As Draft !!");
                }

            }
            catch (Exception Ex)
            {
                ServerLog.Log("CBM :-" + Ex.Message);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", Ex.Message);
            }
            // }
            //}
            //else
            //{
            //    return BLGeneralUtil.return_ajax_string("0", "Data Not Found");
            //}

        }

        [HttpPost]
        public string GenerateCBMlinkforUnregisterUsers([FromBody]JObject objPostOrder)
        {
            DS_CBM objDsCbm = new DS_CBM();
            Master objmaster = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            CBMDetails lstCbmDetails = new CBMDetails();
            List<CBM_QuoteDtl> objlstQuotedtl = new List<CBM_QuoteDtl>();
            List<CBM_Quotehdr> objlstQuotehdr = new List<CBM_Quotehdr>();

            Document objdoc = new Document();

            string quote = ""; string DocNtficID = ""; string Message = "";
            DataTable dtQuoteDtl = new DataTable(); DataTable dtQuotehdr = new DataTable();

            if (objPostOrder != null)
            {
                lstCbmDetails = objPostOrder.ToObject<CBMDetails>();
                if (lstCbmDetails.QuoteDtl.Count != 0)
                {
                    dtQuoteDtl = BLGeneralUtil.ToDataTable(lstCbmDetails.QuoteDtl);
                    dtQuoteDtl = BLGeneralUtil.CheckDateTime(dtQuoteDtl);
                }

                if (lstCbmDetails.QuoteHdr.Count != 0)
                {
                    dtQuotehdr = BLGeneralUtil.ToDataTable(lstCbmDetails.QuoteHdr);
                    dtQuotehdr = BLGeneralUtil.CheckDateTime(dtQuotehdr);
                }
            }
            else
            {
                return BLGeneralUtil.return_ajax_string("0", "Data Not Found ");
            }

            if (lstCbmDetails.quote_id == "")
                quote = "9999";

            DataTable dtqtHdrDetails = GetQtHdrDetailsByQuote_Hdr_id(lstCbmDetails.QuoteHdr[0].quote_hdr_id);

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                if (dtQuotehdr != null)
                {
                    objDsCbm.EnforceConstraints = false;

                    if (dtqtHdrDetails == null)
                    {
                        if (!objdoc.W_GetNextDocumentNoNew(ref DBCommand, "QHD", "", "", ref DocNtficID, ref Message)) // New Driver Notification ID
                        {
                            if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        }

                        objDsCbm.quote_hdr.ImportRow(dtQuotehdr.Rows[0]);
                        objDsCbm.quote_hdr[0].quote_hdr_id = DocNtficID;
                        objDsCbm.quote_hdr[0].quote_id = DocNtficID;
                        objDsCbm.quote_hdr[0].is_assign_to_order = "N";
                        objDsCbm.quote_hdr[0].cbmlink = CbmUrl + "?quote_id=" + BLGeneralUtil.Encrypt(DocNtficID);
                        objDsCbm.quote_hdr[0].created_date = DateTime.UtcNow;
                        objDsCbm.quote_hdr[0].created_by = lstCbmDetails.created_by;

                    }
                    else
                    {
                        objDsCbm.quote_hdr.ImportRow(dtqtHdrDetails.Rows[0]);
                        objDsCbm.quote_hdr[0].quote_id = lstCbmDetails.QuoteHdr[0].quote_hdr_id;
                        objDsCbm.quote_hdr[0].is_assign_to_order = "N";
                        objDsCbm.quote_hdr[0].customer_name = dtQuotehdr.Rows[0]["customer_name"].ToString();
                        objDsCbm.quote_hdr[0].customer_email = dtQuotehdr.Rows[0]["customer_email"].ToString();
                        objDsCbm.quote_hdr[0].customer_mobile = dtQuotehdr.Rows[0]["customer_mobile"].ToString();
                        objDsCbm.quote_hdr[0].room_type = dtQuotehdr.Rows[0]["room_type"].ToString();
                        objDsCbm.quote_hdr[0].total_cbm = Convert.ToDecimal(dtQuotehdr.Rows[0]["total_cbm"].ToString());
                        objDsCbm.quote_hdr[0].appartment_id = dtQuotehdr.Rows[0]["appartment_id"].ToString();
                        objDsCbm.quote_hdr[0].StatusFlag = dtQuotehdr.Rows[0]["StatusFlag"].ToString();
                        objDsCbm.quote_hdr[0].created_by = lstCbmDetails.created_by;


                    }

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

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return BLGeneralUtil.return_ajax_string("1", " Data Save Sucessfully ");

                }
                else
                {
                    return BLGeneralUtil.return_ajax_string("0", "Data Not Found");
                }
            }
            catch (Exception Ex)
            {
                ServerLog.Log(Ex.Message);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", Ex.Message);
            }
        }

        [HttpGet]
        public string Get_QuoteHdr_Details()
        {
            DataTable dt = new DataTable();
            dt = GetQuote_hdr_details("0", "");

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }

        [HttpGet]
        public string GetQtHdrDetails(string quote_hdr_id)
        {
            DataTable dt = new DataTable();
            dt = GetQtHdrDetailsByQuote_Hdr_id(quote_hdr_id);

            if (dt != null && dt.Rows.Count > 0)
                return BLGeneralUtil.return_ajax_data("1", SendReceiveJSon.GetJson(dt));
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }

        public DataTable GetAppIDbySizetype(string sizetypecode)
        {


            DBDataAdpterObject.SelectCommand.Parameters.Clear();
            String SqlSelect = "";
            //appartment type master
            SqlSelect = @"SELECT * from  appartment_type_mst where SizeTypeCode='" + sizetypecode + "'";

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
                return null;
            }
        }

        [HttpGet]
        public string GetCBMLinkByAppartmentID(string apptmtid)
        {
            DataTable dtgetappid = GetAppIDbySizetype(apptmtid);
            if (dtgetappid != null)
            {
                DataTable dtPostLoadOrders = new DataTable();
                String query1 = @"select SizeTypeCode,quote_hdr.* from quote_hdr  
                            join appartment_type_mst on appartment_type_mst.appartment_id = quote_hdr.appartment_id
                             where is_assign_to_order='N' order by created_date desc ";
                //where quote_hdr.appartment_id in ('" + dtgetappid.Rows[0]["appartment_id"].ToString() + "') and is_assign_to_order='N'";

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
                    return BLGeneralUtil.return_ajax_string("0", "No Data found");
            }
            else
                return BLGeneralUtil.return_ajax_string("0", "No Data found");
        }

        [HttpGet]
        public string AssignCBMToOrder(string inqno, string cbmlinkid)
        {
            DS_CBM objDsCbm = new DS_CBM();
            DS_orders dsorder = new DS_orders();
            Master objmaster = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            CBMDetails lstCbmDetails = new CBMDetails();
            Document objdoc = new Document();
            DataTable dtQuoteDtl = new DataTable(); DataTable dtQuotehdr = new DataTable();

            DataTable dtorderDetails = new PostOrderController().GetOrders(inqno);
            dtQuoteDtl = GetQuote_dtl_details("0", cbmlinkid);
            dtQuotehdr = GetQuote_hdr_details("1", cbmlinkid);

            DataTable dtQuoteDtlbyInq = GetQuote_dtl_details("0", inqno);
            DataTable dtQuotehdrbyInq = GetQuote_hdr_details("1", inqno);

            try
            {
                if (dtQuotehdr != null)
                {
                    DBConnection.Open();
                    DBCommand.Transaction = DBConnection.BeginTransaction();

                    objDsCbm.EnforceConstraints = false;
                    objDsCbm.quote_hdr.ImportRow(dtQuotehdr.Rows[0]);
                    objDsCbm.quote_hdr[0].quote_id = inqno;
                    objDsCbm.quote_hdr[0].is_assign_to_order = Constant.Flag_Yes;
                    objDsCbm.quote_dtl.AcceptChanges();
                    objDsCbm.EnforceConstraints = true;

                    //objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_hdr, ref DBCommand);
                    //if (objBLReturnObject.ExecutionStatus == 2)
                    //{
                    //    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //    objBLReturnObject.ExecutionStatus = 2;
                    //    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    //}
                }


                if (dtQuoteDtl != null)
                {
                    objDsCbm.EnforceConstraints = false;
                    for (int i = 0; i < dtQuoteDtl.Rows.Count; i++)
                    {
                        objDsCbm.quote_dtl.ImportRow(dtQuoteDtl.Rows[i]);
                        objDsCbm.quote_dtl[i].quote_id = inqno;
                    }
                    objDsCbm.EnforceConstraints = true;


                    //objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_dtl, ref DBCommand);
                    //if (objBLReturnObject.ExecutionStatus == 2)
                    //{
                    //    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    //    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //    objBLReturnObject.ExecutionStatus = 2;
                    //    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    //}

                }

                if (dtorderDetails != null)
                {
                    dsorder.EnforceConstraints = false;
                    dsorder.orders.ImportRow(dtorderDetails.Rows[0]);
                    dsorder.orders[0].cbmlink = dtQuotehdr.Rows[0]["cbmlink"].ToString();
                    dsorder.EnforceConstraints = true;

                    objBLReturnObject = objmaster.UpdateTables(dsorder.orders, ref DBCommand);
                    if (objBLReturnObject.ExecutionStatus == 2)
                    {
                        ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                        if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        objBLReturnObject.ExecutionStatus = 2;
                        return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                    }
                }


                if (dtQuotehdrbyInq != null)
                {
                    if (dtQuotehdrbyInq.Rows[0]["is_create_on_order"].ToString() == "Y")
                    {
                        objDsCbm.EnforceConstraints = false;
                        objDsCbm.quote_hdr.ImportRow(dtQuotehdrbyInq.Rows[0]);
                        objDsCbm.quote_hdr[1].Delete();
                        objDsCbm.EnforceConstraints = true;
                    }
                    else
                    {
                        dtQuotehdrbyInq.Rows[0]["quote_id"] = dtQuotehdrbyInq.Rows[0]["quote_hdr_id"].ToString();
                        dtQuotehdrbyInq.Rows[0]["is_assign_to_order"] = Constant.FLAG_N;
                        objDsCbm.EnforceConstraints = false;
                        objDsCbm.quote_hdr.ImportRow(dtQuotehdrbyInq.Rows[0]);
                        objDsCbm.EnforceConstraints = true;

                    }

                }


                if (dtQuoteDtlbyInq != null)
                {
                    for (int i = 0; i < (dtQuoteDtlbyInq.Rows.Count); i++)
                        dtQuoteDtlbyInq.Rows[i]["quote_id"] = dtQuotehdrbyInq.Rows[0]["quote_hdr_id"].ToString();

                    objDsCbm.EnforceConstraints = false;
                    for (int i = 0; i < (dtQuoteDtlbyInq.Rows.Count); i++)
                        objDsCbm.quote_dtl.ImportRow(dtQuoteDtlbyInq.Rows[i]);
                    objDsCbm.EnforceConstraints = true;
                }


                objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_hdr, ref DBCommand);
                if (objBLReturnObject.ExecutionStatus == 2)
                {
                    ServerLog.Log(objBLReturnObject.ServerMessage.ToString());
                    if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    objBLReturnObject.ExecutionStatus = 2;
                    return BLGeneralUtil.return_ajax_string("0", objBLReturnObject.ServerMessage);
                }


                objBLReturnObject = objmaster.UpdateTables(objDsCbm.quote_dtl, ref DBCommand);
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

                ServerLog.Log("saved AssignCBMToOrder ");
                return BLGeneralUtil.return_ajax_string("1", "Data Save Sucessfully !!");

            }
            catch (Exception Ex)
            {
                ServerLog.Log(Ex.Message);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                objBLReturnObject.ExecutionStatus = 2;
                return BLGeneralUtil.return_ajax_string("0", Ex.Message);
            }

        }

        [HttpGet]
        public string delete_QuoteHdrdtl(string quoteid)
        {
            DS_CBM objDsCbm = new DS_CBM();
            Master objmaster = new Master();
            BLReturnObject objBLReturnObject = new BLReturnObject();
            CBMDetails lstCbmDetails = new CBMDetails();
            Document objdoc = new Document();

            DataTable dtqtHdrDetails = GetQtHdrDetailsByQuote_Hdr_id(quoteid);

            try
            {
                DBConnection.Open();
                DBCommand.Transaction = DBConnection.BeginTransaction();

                objDsCbm.EnforceConstraints = false;

                if (dtqtHdrDetails != null)
                {
                    objDsCbm.quote_hdr.ImportRow(dtqtHdrDetails.Rows[0]);
                    objDsCbm.quote_hdr[0].Delete();
                }

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

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("1", "Delete successfully !!");
            }
            catch (Exception Ex)
            {
                ServerLog.Log(Ex.Message);
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", Ex.Message);
            }

        }

        [HttpGet]
        public string SendCBMLinkMail(string quote_id, string emailId)
        {
            try
            {

                StreamReader sr; string LINK = "";
                sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "\\Mailers\\TrukkerUAE_SendCBMGeneratedLink.html");
                string strdata = ""; string msg = "";


                DataTable dtquotehdr = GetQuote_hdr_details("2", quote_id);

                while (!sr.EndOfStream)
                {
                    strdata = sr.ReadToEnd();
                    strdata = strdata.Replace("UserName", dtquotehdr.Rows[0]["customer_name"].ToString());
                    strdata = strdata.Replace("CBMLINK", dtquotehdr.Rows[0]["cbmlink"].ToString());
                }
                sr.Close();


                msg = "";
                EMail objemail = new EMail();
                Boolean bl = objemail.SendMail(emailId, strdata, " Hello " + dtquotehdr.Rows[0]["customer_name"].ToString() + ", Tell us about your Home Move Contents ", ref msg, "CONTACT", "SENDFROM");
                if (!bl)
                    return BLGeneralUtil.return_ajax_string("0", msg);
                else
                    return BLGeneralUtil.return_ajax_string("1", "Mail Send Successfully");

            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.StackTrace + "error in send CBM email" + ex.ToString());
                if (DBCommand.Transaction != null) DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return BLGeneralUtil.return_ajax_string("0", ex.Message);

            }
        }
    }
}



