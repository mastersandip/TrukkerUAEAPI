using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace trukkerUAE.Models
{
    public class TruckkerProjectModels
    {

    }
    public class driver_bank_detail
    {
        public string driver_id { get; set; }
        public string bank_name { get; set; }
        public string atm { get; set; }
        public string ETransfer { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class driver_contact_detail
    {
        public string driver_id { get; set; }
        public string addr_id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string phone_no { get; set; }
        public string mobile_no { get; set; }

        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

        public string emirates_id { get; set; }
        public string emirates_id_exp_date { get; set; }
        public string emirates_id_copy { get; set; }
        public string nationality { get; set; }

    }
    public class driver_identification_detail
    {
        public string driver_id { get; set; }
        public string identification_id { get; set; }
        public string id_no { get; set; }
        public string id_path { get; set; }
        public string id_issued_from { get; set; }
        public string id_valid_from { get; set; }
        public string id_valid_upto { get; set; }
        public string status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_insurance_detail
    {
        public string driver_id { get; set; }
        public string insurance_policy_no { get; set; }
        public string insurance_details { get; set; }
        public string status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class driver_language_detail
    {
        public string driver_id { get; set; }
        public string language_id { get; set; }
        public string can_speak { get; set; }
        public string can_read { get; set; }
        public string can_write { get; set; }
        public string active_flag { get; set; }

        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_license_detail
    {
        public string driver_id { get; set; }
        public string License_no { get; set; }
        public string License_type { get; set; }
        public string Issued_place { get; set; }
        public string Valid_from { get; set; }
        public string Valid_upto { get; set; }
        public string id_path { get; set; }
        public string status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_mobile_detail
    {
        public string driver_id { get; set; }
        public string phone_model { get; set; }
        public string current_network { get; set; }
        public string consume_3G_Data { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_mst
    {
        public string driver_id { get; set; }
        public string reg_date { get; set; }
        public string Name { get; set; }
        public string age { get; set; }
        public string dob { get; set; }
        public string qualification { get; set; }
        public string driver_origin { get; set; }
        public string martial_status { get; set; }
        public string no_of_child { get; set; }
        public string health_issues { get; set; }
        public string smoking { get; set; }
        public string alcohol { get; set; }
        public string legal_history { get; set; }
        public string commercial_experience { get; set; }
        public string phone_no { get; set; }
        public string mobile_no { get; set; }
        public string driver_photo { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_prefered_destination
    {
        public string driver_id { get; set; }
        public string destination_id { get; set; }
        public string state { get; set; }

        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class driver_truck_detail
    {
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class load_post_enquiry
    {
        public string NoOfSupervisor { get; set; }
        public string SizeTypeCode { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDistanceUOM { get; set; }
        public string TimeToTravelInMinute { get; set; }
        public string NoOfTruck { get; set; }
        public string NoOfDriver { get; set; }
        public string NoOfLabour { get; set; }
        public string NoOfHandiman { get; set; }
        public string NoOfHelper { get; set; }
        public string Isfinalorder { get; set; }
        public string IncludePackingCharge { get; set; }
        public string rate_type_flag { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string AssignOrderToDR { get; set; }
        public string isfinal { get; set; }
        public string Area { get; set; }
        public string order_type_flag { get; set; }
        public string TruckTypeCode { get; set; }
        public string goods_type_flag { get; set; }
        public string isassign_driver_truck { get; set; }
        public string isassign_mover { get; set; }
        public string mover_id { get; set; }

        public string payment_mode { get; set; }
        public string payment_status { get; set; }
        public string billing_add { get; set; }
        public string billing_name { get; set; }
        public string destination_full_add { get; set; }
        public string source_full_add { get; set; }



        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string load_inquiry_date { get; set; }
        public string email_id { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_pincode { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_pincode { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }
        public decimal aprox_kms { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date_shipper { get; set; }
        public string load_inquiry_delivery_time_shipper { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_enquiry_expiry_hours { get; set; }

        public string consignor_name { get; set; }
        public string origin_address { get; set; }
        public string origin_pincode { get; set; }
        public string receiver_name { get; set; }
        public string receiver_mobile { get; set; }
        public string receiver_email { get; set; }
        public string owner_id { get; set; }

        public string consignee_name { get; set; }
        public string origin_lat { get; set; }
        public string origin_lng { get; set; }

        public string destination_address { get; set; }
        public string destination_pincode { get; set; }
        public string dest_lat { get; set; }
        public string dest_lng { get; set; }

        public string load_inquiry_aprx_days { get; set; }
        public string load_inquiry_estd_delivery_date { get; set; }
        public string Remarks { get; set; }

        public string flexible_days { get; set; }
        public string load_unload_extra_hours { get; set; }
        public string required_price { get; set; }

        public string status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string Isupdate { get; set; }
        public string promocode { get; set; }
        public string goods_details { get; set; }
        public string goods_weight { get; set; }
        public string goods_weightUOM { get; set; }


        public string Isupdatebillingadd { get; set; }
        public string IsDraft { get; set; }

        public string Hiretruck_NoofDay { get; set; }
        public string Hiretruck_To_datetime { get; set; }
        public string Hiretruck_IncludingFuel { get; set; }

        // add for additional services 		
        public string IncludeAddonService { get; set; }
        public string AddonServices { get; set; }
        public string AddonServicesrate { get; set; }
        public string AddSerBaseDiscount { get; set; }
        public string TotalPaintingCharge { get; set; }
        public string TotalPaintingDiscount { get; set; }
        public string TotalCleaningCharge { get; set; }
        public string TotalCleaningDiscount { get; set; }
        public string TotalPestControlCharge { get; set; }
        public string TotalPestControlDiscount { get; set; }
        public string TotalAddServiceDiscount { get; set; }
        public string TotalAddServiceCharge { get; set; }
        public string AddonFlag { get; set; }
        public string Total_cost_without_addon { get; set; }
        public string order_by { get; set; }
        

    }
    public class post_load_inquiry_temp
    {

        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string load_inquiry_date { get; set; }
        public string email_id { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_pincode { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_pincode { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }
        public decimal aprox_kms { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_delivery_date_shipper { get; set; }
        public string load_inquiry_delivery_time_shipper { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_enquiry_expiry_hours { get; set; }

        public string load_inquiry_aprx_days { get; set; }
        public string load_inquiry_estd_delivery_date { get; set; }
        public string Remarks { get; set; }


        public string payment_mode { get; set; }
        public string flexible_days { get; set; }
        public string load_unload_extra_hours { get; set; }
        public string status { get; set; }
        public string required_price { get; set; }
        public string ref_inq_no { get; set; }

        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class LoginCheck
    {
        public string user_id { get; set; }
        public string password { get; set; }
        public string user_status_flag { get; set; }
        public string last_login_date { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string load_inquiry_no { get; set; }
    }
    public class owner
    {
        public string owner_id { get; set; }
        public string reg_date { get; set; }
        public string owner_name { get; set; }
        public string owner_type { get; set; }
        public string no_of_trucks { get; set; }
        public string date_of_birth { get; set; }

        public string contact_person { get; set; }
        public string contact_number { get; set; }

        public string phone_no { get; set; }
        public string mobile_no { get; set; }
        public string city { get; set; }
        public string IsOwnerDriver { get; set; }
        public string mandi_id { get; set; }

        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class owner_contact_detail
    {
        public string owner_id { get; set; }
        public string addr_id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string phone_no { get; set; }
        public string mobile_no { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class owner_driver_detail
    {
        public string owner_id { get; set; }
        public string driver_id { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class owner_truck_details
    {
        public string owner_id { get; set; }
        public string truck_id { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class ResponseCls
    {
        public string status { get; set; }
        public string ownerid { get; set; }
        public string truckid { get; set; }
        public string driverid { get; set; }
        public string Message { get; set; }
        public string driver_img { get; set; }

    }
    public class truck
    {
        public string truck_id { get; set; }
        public string reg_date { get; set; }
        public string truck_make_id { get; set; }
        public string truck_make_id_other { get; set; }
        public string truck_model { get; set; }
        public string truck_model_other { get; set; }
        public string year_of_mfg { get; set; }
        public string load_capacity { get; set; }
        public string axle_detail { get; set; }
        public string axle_detail_other { get; set; }
        public string body_type { get; set; }
        public string body_type_other { get; set; }
        public string current_millage { get; set; }
        public string avg_millage_per_month { get; set; }
        public string fuel_average { get; set; }
        public string finance_company { get; set; }
        public string vehicle_reg_no { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class truck_insurance_detail
    {
        public string truck_id { get; set; }
        public string insurance_sr_id { get; set; }
        public string insurance_policy_no { get; set; }
        public string insurance_details { get; set; }
        public string policy_photo_path { get; set; }
        public string policy_issue_date { get; set; }
        public string policy_expiry_date { get; set; }
        public string active_flag { get; set; }
        public string status { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }

    public class truck_maintenance_detail
    {
        public string truck_id { get; set; }
        public string maintenance_id { get; set; }

        public string type_of_maintenance { get; set; }
        public string maintenance_copy { get; set; }
        public string maintenance_date { get; set; }
        public string active_flag { get; set; }
        public string remarks { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }

    public class truck_permit_details
    {
        public string permit_reg_id { get; set; }
        public string truck_id { get; set; }
        public string permit_no { get; set; }
        public string[] state_code { get; set; }
        public string permit_type { get; set; }
        public string valid_from { get; set; }
        public string valid_upto { get; set; }
        public string permit_photo { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class truck_permit_state_detail
    {
        public string permit_reg_id { get; set; }
        public string truck_id { get; set; }
        public string state { get; set; }
        public string valid_from { get; set; }
        public string valid_upto { get; set; }
        public string permit_photo { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class truck_rto_registration_detail
    {
        public string reg_id { get; set; }
        public string truck_id { get; set; }
        public string vehice_photo { get; set; }
        public string vehicle_reg_no { get; set; }
        public string vehicle_reg_date { get; set; }
        public string reg_place { get; set; }
        public string reg_doc_copy { get; set; }
        public string vehicle_regno_copy { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class load_order_enquiry_quotation
    {
        public string quot_id { get; set; }
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string owner_id { get; set; }
        public string load_inquiry_date { get; set; }
        public string email_id { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }
        public string aprox_kms { get; set; }
        public string aprox_days { get; set; }
        public decimal load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string required_price { get; set; }
        public string quotation_cost { get; set; }
        public string quotation_others { get; set; }
        public string asked_quot_cost { get; set; }
        public string driver_da { get; set; }
        public string toll_cost { get; set; }
        public string quotation_total_cost { get; set; }
        public string status { get; set; }
        public string reason_code { get; set; }
        public string active_flag { get; set; }
        public string Remark { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class load_enquiry_transporter_notification
    {
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string load_inquiry_date { get; set; }
        public string truck_id { get; set; }
        public string owner_id { get; set; }
        public string driver_id { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destination_lat { get; set; }
        public string inquiry_destination_lng { get; set; }
        public decimal aprox_kms { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_inquiry_status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class orders
    {
        public string order_id { get; set; }
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string owner_id { get; set; }
        public string shipper_email_id { get; set; }

        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_pincode { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }

        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_pincode { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }

        public string aprox_kms { get; set; }
        public string aprox_days { get; set; }

        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_inquiry_status { get; set; }

        public string required_price { get; set; }
        public string quotation_cost { get; set; }
        public string asked_quot_cost { get; set; }
        public string driver_da { get; set; }
        public string toll_cost { get; set; }

        public string quotation_total_cost { get; set; }
        public string quotation_estimation_transit_time { get; set; }
        public string quotation_estimated_distance { get; set; }
        public string quotation_estimated_travel_time { get; set; }
        public string quotation_estimated_delivery_time { get; set; }
        public string quotation_distance_based_cost { get; set; }
        public string quotation_advance_paid_amount { get; set; }
        public string quotation_payment_due { get; set; }
        public string quotation_final_paid_amount { get; set; }

        public string origin_address { get; set; }
        public string origin_pincode { get; set; }
        public string destination_address { get; set; }
        public string destination_pincode { get; set; }

        public string receiver_name { get; set; }
        public string receiver_mobile { get; set; }
        public string receiver_email { get; set; }
        public string distance_kms_to_origin { get; set; }
        public string approx_time_to_reach { get; set; }

        public string pickup_ready { get; set; }
        public string driver_reached { get; set; }
        public string shipper_confirmed { get; set; }
        public string document_issued { get; set; }
        public string document_received { get; set; }
        public string loading_end_time_actual { get; set; }

        public string active_flag { get; set; }
        public string Remark { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

        public string kmsdistance { get; set; }
        public string timetoreach { get; set; }
        public string status { get; set; }
        public string loading_start_time { get; set; }
        public string loading_end_time { get; set; }
        public string material_value { get; set; }
        public string material_description { get; set; }

        public string current_lat { get; set; }
        public string current_lng { get; set; }

        public string IsOnDuty { get; set; }
        public string isfree { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string IsCancel { get; set; }
        public string MinDistance { get; set; }
        public string truckType { get; set; }

        public string username { get; set; }
        public string order_type { get; set; }
        public string PageTo { get; set; }
        public string PageFrom { get; set; }

        public string Assigndriver { get; set; }
        public string OrderBy { get; set; }
        public string SortBy { get; set; }

        public string PageNo { get; set; }
        public string RowsPerPage { get; set; }
        public string AddOnTransactionIds { get; set; }
    }

    public class InquiryQuote
    {
        public string driver_id { get; set; }
        public string owner_id { get; set; }
        public string load_inquiry_no { get; set; }
        public decimal asked_quot_cost { get; set; }
        public string user_id { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_host { get; set; }
        public string modified_by { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string Isaccepted { get; set; }
    }
    public class driver_order_notifications
    {
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string truck_id { get; set; }
        public string owner_id { get; set; }
        public string driver_id { get; set; }
        public string load_inquiry_source { get; set; }
        public string load_inquiry_destination { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_inquiry_status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class EmailData
    {
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string truck_id { get; set; }
        public string owner_id { get; set; }
        public string driver_id { get; set; }
        public string MailFor { get; set; }
        public string to_email { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string AttachWIPOPdf { get; set; }
        public string attachedDoc { get; set; }
        public string emailbody { get; set; }
        public string active { get; set; }
        public string IsEmailSent { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

        public string lr_id { get; set; }
        public string lr_date { get; set; }
        public string material_desc { get; set; }
        public string material_qty { get; set; }
        public string material_value { get; set; }
        public string transporter { get; set; }

    }
    public class Shipper
    {
        public string reg_type { get; set; }
        public string shipper_id { get; set; }
        public string reg_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string mobile_no { get; set; }
        public string landline_no { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string city_other { get; set; }
        public string pincode { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string company_name { get; set; }
        public string company_type { get; set; }
        public string company_type_other { get; set; }
        public string website { get; set; }
        public string pan_no { get; set; }
        public string tin_no { get; set; }
        public string profile_pic { get; set; }
        public string alt_contact_name { get; set; }
        public string alt_mobile { get; set; }
        public string otp { get; set; }
        public string otp_verified { get; set; }
        public string email_verified { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string domainname { get; set; }


        public object middle_name { get; set; }

        public object OTP { get; set; }
    }
    public class Transporter
    {
        public string transporter_id { get; set; }
        public string reg_type { get; set; }
        public string reg_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string city_other { get; set; }
        public string pincode { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public string landline_no { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string company_name { get; set; }
        public string company_type { get; set; }
        public string company_type_other { get; set; }
        public string website { get; set; }
        public string pan_no { get; set; }
        public string tin_no { get; set; }
        public string profile_pic { get; set; }
        public string alt_contact_name { get; set; }
        public string alt_mobile { get; set; }
        public string otp { get; set; }
        public string otp_verified { get; set; }
        public string email_verified { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string domainname { get; set; }
    }
    public class Transporter_truck_type
    {
        public string transporter_id { get; set; }
        public string transporter_truck_type { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class Transporter_company_type
    {
        public string transporter_id { get; set; }
        public string company_type { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class truck_current_position
    {
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string log_date { get; set; }
        public string load_inquiry_no { get; set; }
        public string truck_lat { get; set; }
        public string truck_lng { get; set; }
        public string truck_location { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }

        public string remaining_kms { get; set; }
        public string eta { get; set; }
    }
    public class orders_lr
    {
        public string lr_type { get; set; }
        public string lr_id { get; set; }
        public DateTime lr_date { get; set; }
        public string order_id { get; set; }
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string owner_id { get; set; }
        public string shipper_email_id { get; set; }
        public string consignor_name { get; set; }
        public string consignor_address { get; set; }
        public string consignee_name { get; set; }
        public string consignee_address { get; set; }
        public string material_description { get; set; }
        public string material_qty { get; set; }
        public string material_weight { get; set; }
        public string material_value { get; set; }
        public decimal lr_rate { get; set; }
        public decimal lr_value { get; set; }
        public decimal lr_value_paid { get; set; }
        public decimal lr_value_pending { get; set; }
        public string status { get; set; }
        public string active_flag { get; set; }
        public string Remark { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
    }
    public class AppLog
    {
        public string user_id { get; set; }
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string owner_id { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string order_id { get; set; }
        public string log_detail { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }
    public class Activate
    {
        public string reg_type { get; set; }
        public string reg_id { get; set; }
    }

    public class User
    {

        public string client_type { get; set; }
        public string user_id { get; set; }
        public string role_id { get; set; }
        public string unique_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string user_short_name { get; set; }
        public string email_id { get; set; }
        public string user_dept { get; set; }
        public string password { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string login_levels { get; set; }
        public string pass_expiry_date { get; set; }
        public string user_status_flag { get; set; }
        public string user_loc_flag { get; set; }
        public string remark { get; set; }
        public string last_login_date { get; set; }
        public string OTP { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string last_modified_by { get; set; }
        public string last_modified_date { get; set; }
        public string last_modified_host { get; set; }
        public string device_type { get; set; }
        public string load_inquiry_no { get; set; }
        public string old_password { get; set; }

        public string fromdate { get; set; }
        public string todate { get; set; }
        public string reg_type { get; set; }
    }

    public class user_mst
    {

        public string client_type { get; set; }
        public string user_id { get; set; }
        public string role_id { get; set; }
        public string unique_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string user_short_name { get; set; }
        public string email_id { get; set; }
        public string user_dept { get; set; }
        public string password { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string login_levels { get; set; }
        public string pass_expiry_date { get; set; }
        public string user_status_flag { get; set; }
        public string user_loc_flag { get; set; }
        public string remark { get; set; }
        public string last_login_date { get; set; }
        public string OTP { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string last_modified_by { get; set; }
        public string last_modified_date { get; set; }
        public string last_modified_host { get; set; }
        public string device_type { get; set; }
        public string load_inquiry_no { get; set; }
        public string old_password { get; set; }

        public string fromdate { get; set; }
        public string todate { get; set; }
        public string reg_type { get; set; }
    }

    public class Partners_mst
    {
        public string partner_id { get; set; }
        public string company_name { get; set; }
        public string city_name { get; set; }
        public string service_offered { get; set; }
        public string website { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string name { get; set; }
        public string mobileno { get; set; }
        public string email { get; set; }
        public string address { get; set; }

    }
    public class InquiryEmailLog
    {
        public string maildate { get; set; }
        public string Source { get; set; }
        public string EmailID { get; set; }
        public string Message { get; set; }
        public string mobile_no { get; set; }
        public string Status { get; set; }
        public string name { get; set; }
        public string subject { get; set; }
        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }


        public string ShippingDatetime { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDistanceUOM { get; set; }
        public string Destinationaddress { get; set; }
        public string Sourceaddress { get; set; }
        public string Sizetypecode { get; set; }

        public string IsEncripted { get; set; }
    }

    public class PromoCode
    {
        public string coupon_code { get; set; }
        public string OrderDate { get; set; }
        public string rate_type_flag { get; set; }
        public string load_inquiry_no { get; set; }
        public string shipper_id { get; set; }
        public string order_id { get; set; }
        public string FlatDiscount { get; set; }
        public string PercentageDiscount { get; set; }
        public string Total_cost { get; set; }
        public string Discount_value { get; set; }
    }


    public class paymentdetails
    {

        public string Transaction_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string amount { get; set; }
        public string authorization_code { get; set; }
        public string card_number { get; set; }
        public string command { get; set; }
        public string currency { get; set; }
        public string customer_email { get; set; }
        public string customer_ip { get; set; }
        public string eci { get; set; }
        public string expiry_date { get; set; }
        public string fort_id { get; set; }
        public string language { get; set; }
        public string merchant_reference { get; set; }
        public string payment_option { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string sdk_token { get; set; }
        public string status { get; set; }
        public string token_name { get; set; }
        public string merchant_identifier { get; set; }
        public string signature { get; set; }
        public string device_type { get; set; }
        public string device_id { get; set; }
        public string created_host { get; set; }
        public string created_by { get; set; }

        public string Isupdatebillingadd { get; set; }
    }

    public class CouponMst
    {
        public string coupon_id { get; set; }
        public string coupon_code { get; set; }
        public string coupon_desc { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string coupon_type { get; set; }
        public string discount { get; set; }
        public string flat_discount { get; set; }
        public string percentage_discount { get; set; }
        public string first_user_only { get; set; }
        public string each_user_once_at_time { get; set; }
        public string user_once_only { get; set; }
        public string order_type_flag { get; set; }
        public string rate_type_flag { get; set; }
        public string SizeTypeCode { get; set; }
        public string IsActive { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }

    }

    public class CBMDetails
    {
        public List<CBM_QuoteDtl> QuoteDtl { get; set; }
        public List<CBM_Quotehdr> QuoteHdr { get; set; }
        public string quote_id { get; set; }
        public string created_by { get; set; }
    }

    public class CBM_QuoteDtl
    {
        public string quote_dtl_id { get; set; }
        public string quote_id { get; set; }
        public string qt_dtl_id { get; set; }

        public string item_code { get; set; }
        public string item_cbm { get; set; }
        public string item_qty { get; set; }
        public string location_id { get; set; }
        public string ItemFlg { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string last_modified_by { get; set; }
        public string last_modified_date { get; set; }
        public string last_modified_host { get; set; }
    }

    public class CBM_Quotehdr
    {
        public string quote_id { get; set; }
        public string quote_hdr_id { get; set; }
        public string customer_name { get; set; }
        public string customer_mobile { get; set; }
        public string customer_email { get; set; }
        public string total_cbm { get; set; }
        public string room_type { get; set; }
        public string appartment_id { get; set; }
        public string cbmlink { get; set; }
        public string otp { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string last_modified_by { get; set; }
        public string last_modified_date { get; set; }
        public string last_modified_host { get; set; }
        public string StatusFlag { get; set; }
        public string notes { get; set; }
        public string GeneratedPdfLink { get; set; }

    }

    public class order_AddonService_details
    {
        public string Transaction_id { get; set; }
        public string AddSerBaseDiscount { get; set; }
        public string ServiceTypeCode { get; set; }
        public string SizeTypeCode { get; set; }
        public string ServiceCharge { get; set; }
        public string ServiceDiscount { get; set; }
        public string load_inquiry_no { get; set; }
        public string user_mobileno { get; set; }
        public string user_email { get; set; }
        public string user_name { get; set; }
        public string address { get; set; }
        public string CelingRequired { get; set; }
        public string NoofCleling { get; set; }
        public string Service_date { get; set; }
        public string Service_time { get; set; }
        public string NoofRooms { get; set; }
        public string NoofCleaners { get; set; }
        public string Notes { get; set; }
        public string status { get; set; }
        public string AgencyId { get; set; }
        public string rem_amt_to_receive { get; set; }
        public string Payment_mode { get; set; }
        public string payment_status { get; set; }
        public string payment_link { get; set; }
        public string addon_by { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string SubServiceTypeCode { get; set; }

    }

    public class Dubi_load_post_enquiry
    {
        public string NoOfSupervisor { get; set; }
        public string SizeTypeCode { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDistanceUOM { get; set; }
        public string TimeToTravelInMinute { get; set; }
        public string NoOfTruck { get; set; }
        public string NoOfDriver { get; set; }
        public string NoOfLabour { get; set; }
        public string NoOfHandiman { get; set; }
        public string NoOfHelper { get; set; }
        public string Isfinalorder { get; set; }
        public string IncludePackingCharge { get; set; }
        public string rate_type_flag { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string AssignOrderToDR { get; set; }
        public string isfinal { get; set; }
        public string Area { get; set; }
        public string order_type_flag { get; set; }
        public string TruckTypeCode { get; set; }
        public string goods_type_flag { get; set; }
        public string isassign_driver_truck { get; set; }
        public string isassign_mover { get; set; }
        public string mover_id { get; set; }

        public string payment_mode { get; set; }
        public string payment_status { get; set; }
        public string billing_add { get; set; }
        public string billing_name { get; set; }
        public string destination_full_add { get; set; }
        public string source_full_add { get; set; }



        public string shipper_id { get; set; }
        public string load_inquiry_no { get; set; }
        public string load_inquiry_date { get; set; }
        public string email_id { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_pincode { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_pincode { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }
        public decimal aprox_kms { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date_shipper { get; set; }
        public string load_inquiry_delivery_time_shipper { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_delivery_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_enquiry_expiry_hours { get; set; }

        public string consignor_name { get; set; }
        public string origin_address { get; set; }
        public string origin_pincode { get; set; }
        public string receiver_name { get; set; }
        public string receiver_mobile { get; set; }
        public string receiver_email { get; set; }
        public string owner_id { get; set; }

        public string consignee_name { get; set; }
        public string origin_lat { get; set; }
        public string origin_lng { get; set; }

        public string destination_address { get; set; }
        public string destination_pincode { get; set; }
        public string dest_lat { get; set; }
        public string dest_lng { get; set; }

        public string load_inquiry_aprx_days { get; set; }
        public string load_inquiry_estd_delivery_date { get; set; }
        public string Remarks { get; set; }

        public string flexible_days { get; set; }
        public string load_unload_extra_hours { get; set; }
        public string required_price { get; set; }

        public string status { get; set; }
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string Isupdate { get; set; }
        public string promocode { get; set; }
        public string goods_details { get; set; }
        public string goods_weight { get; set; }
        public string goods_weightUOM { get; set; }


        public string Isupdatebillingadd { get; set; }
        public string IsDraft { get; set; }

        public string Hiretruck_NoofDay { get; set; }
        public string Hiretruck_To_datetime { get; set; }
        public string Hiretruck_IncludingFuel { get; set; }

        // add for additional services 
        public string IncludeAddonService { get; set; }
        public string AddonServices { get; set; }
        public string AddonServicesrate { get; set; }

        public string AddSerBaseDiscount { get; set; }
        public string TotalPaintingCharge { get; set; }
        public string TotalPaintingDiscount { get; set; }
        public string TotalCleaningCharge { get; set; }
        public string TotalCleaningDiscount { get; set; }
        public string TotalPestControlCharge { get; set; }
        public string TotalPestControlDiscount { get; set; }
        public string TotalAddServiceDiscount { get; set; }
        public string TotalAddServiceCharge { get; set; }
        public string AddonFlag { get; set; }
        public string Total_cost_without_addon { get; set; }
        public string UserName { get; set; }
        public string UserMobileNo { get; set; }
        public string Total_cost_without_discount { get; set; }
        public string cbmlink { get; set; }
        public string trakurl { get; set; }



    }

    public class dubiz_goods_orders_details
    {
        public string Transaction_id { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDistanceUOM { get; set; }
        public string TimeToTravelInMinute { get; set; }
        public string inquiry_source_addr { get; set; }
        public string inquiry_source_city { get; set; }
        public string inquiry_source_pincode { get; set; }
        public string inquiry_source_state { get; set; }
        public string inquiry_source_lat { get; set; }
        public string inquiry_source_lng { get; set; }
        public string inquiry_destination_addr { get; set; }
        public string inquiry_destination_city { get; set; }
        public string inquiry_destination_pincode { get; set; }
        public string inquiry_destination_state { get; set; }
        public string inquiry_destionation_lat { get; set; }
        public string inquiry_destionation_lng { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string status { get; set; }
        public string Total_cost { get; set; }
        public string Total_cost_without_discount { get; set; }
        public string rem_amt_to_receive { get; set; }
        public string active_flag { get; set; }
        public string item_code { get; set; }
        public string item_desc { get; set; }
        public string user_name { get; set; }
        public string user_no { get; set; }
        public string user_email { get; set; }
        public string pickup_contact_name { get; set; }
        public string pickup_contact_no { get; set; }
        public string pickup_date { get; set; }
        public string delivery_contact_name { get; set; }
        public string delivery_contact_no { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_by { get; set; }
        public string modified_date { get; set; }
        public string modified_host { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
        public string Isfinalorder { get; set; }

    }
}