using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace trukkerProject.Models
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
        public string load_inquiry_deliverty_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_enquiry_expiry_hours { get; set; }
        
        public string origin_address { get; set; }
        public string origin_pincode { get; set; }
        public string receiver_name { get; set; }
        public string receiver_mobile { get; set; }
        public string receiver_email { get; set; }
        public string owner_id { get; set; }

        public string destination_address { get; set; }
        public string destination_pincode { get; set; }

        public string load_inquiry_aprx_days { get; set; }
        public string load_inquiry_estd_delivery_date { get; set; }
        public string Remarks { get; set; }
        public string payment_mode { get; set; }
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
        public string inquiry_destination_lat { get; set; }
        public string inquiry_destination_lng { get; set; }
        public decimal aprox_kms { get; set; }
        public string load_inquiry_truck_type { get; set; }
        public string load_inquiry_shipping_date { get; set; }
        public string load_inquiry_shipping_time { get; set; }
        public string load_inquiry_delivery_date { get; set; }
        public string load_inquiry_deliverty_time { get; set; }
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
        public string insurance_policy_no { get; set; }
        public string insurance_details { get; set; }
        public string policy_photo_path { get; set; }
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
        public string load_inquiry_deliverty_time { get; set; }
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
        public string load_inquiry_deliverty_time { get; set; }
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
        public string load_inquiry_deliverty_time { get; set; }
        public string load_inquiry_material_type { get; set; }
        public string load_inquiry_load { get; set; }
        public string load_inquiry_packing { get; set; }
        public string load_inquiry_status { get; set; }

        public decimal kmsdistance { get; set; }
        public string timetoreach { get; set; }
        public string status { get; set; }
        public string loading_start_time { get; set; }
        public string loading_end_time { get; set; }
        public string material_value { get; set; }
        public string material_description { get; set; }

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
    public class InquiryQuote
    {
        public string owner_id { get; set; }
        public string load_inquiry_no {get;set;}
        public decimal asked_quot_cost {get;set;}
        public string user_id { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string modified_host { get; set; }
        public string modified_by { get; set; }
        public string modified_device_id { get; set; }
        public string modified_device_type { get; set; }
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
        public string load_inquiry_deliverty_time { get; set; }
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
    }
    public class Shipper
    {
        public string shipper_id { get; set; }
        public string reg_date { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string company_name { get; set; }
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
        public string active_flag { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string created_host { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
    }

}