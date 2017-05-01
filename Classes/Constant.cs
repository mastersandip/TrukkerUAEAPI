using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace trukkerUAE.Classes
{
    public static class Constant
    {
        #region Yes/No
        public const String FLAG_Y = "Y";
        public const String FLAG_N = "N";
        #endregion

        public const String PaymentModeCash = "C";
        public const String PaymentModeONLINE = "O";
        public const String CurrencyCode = "AED";
        public const String PaymentMatch = "M";
        public const String PaymentUnMatch = "U";

        public const String MAILTYPE_ADDON = "ADDON";
        public const String RateTypeBudget = "B"; //Normal/Standard
        public const String RateTypePremium = "P"; //Luxury

        public const int Status_Success = 1;
        public const int Status_Fail = 2;

        public const String TargetApp = "TrukkerUAE";

        public const String OS_iOS = "iOS";
        public const String OS_Android = "Android";

        public const String MessageType_PostOrder = "PostOrder";
        public const String MessageType_AssignOrder = "AssignOrder";
        public const String MessageType_OrderComplete = "OrderComplete";
        public const String MessageType_Manual = "Manual";
        public const String MessageType_Offers = "Offers";
        public const String MessageType_AssignDriverTruck = "AssignDriverTruck";
        public const String MessageType_DriverStatus = "DriverStatus";

        public const String Provider_MSSQL = "System.Data.SqlClient";
        public const String Provider_Oracle = "System.Data.OracleClient"; //"ORAOLEDB.ORACLE";
        public const String Provider_MYSQL = "MySql.Data.MySqlClient";

        public const String DateFormat_SQL = "'%m-%d-%Y'";
        public const String DateFormat_CSharp = "MM-dd-yyyy";
        public const String Active = "A";
        public const String DeActive = "D";
        public const String Flag_Yes = "Y";
        public const String Flag_No = "N";

        public const String PaymentType_Cash = "C";
        public const String PaymentType_Cheque = "B";
        public const String PaymentType_Card = "D";

        public const String PaymentStatus_Pending = "P";
        public const String PaymentStatus_Received = "R";

        public const String Match_Payment = "M";
        public const String UnMatch_Payment = "U";

        public const String Credit = "C";
        public const String Debit = "D";

        public const Int16 LoginAttempt = 4;

        public const String ROUNDING_NEAREST_RUPEE = "1";
        public const String ROUNDING_UPTO_2_DECIMAL_PTS = "2";
        public const String ROUNDING_CARRY_FORWARD_PAISE = "3";
        public const String ROUNDING_NONE = "4";
        public const String ROUNDING_NEAREST_FIVE_RUPEES = "5";
        public const String ROUNDING_NEAREST_TEN_RUPEES = "6";
        public const String ROUNDING_NEAREST_HUNDRED_RUPEES = "7";

        public const String BEST_QUOTE_SELECTED_BY_ADMIN = "02"; // Quote Selected by Admin and Sent to shipper for acceptance
        public const String QUOTE_ACCEPTED = "03";  // Status By Shipper / Quote accepted and awaiting driver allocation 
        public const String TRUCK_AND_DRIVER_ALLOCATED = "04";  // Status By Transporter / Truck and driver allocated 
        public const String TRUCK_REACHED_ORIGIN_LOADING_TIME_STARTS = "06";  // Status by Driver - Driver Reached  - Loading time starts
        public const String DOCUMENT_ISSUED_BY_ADMIN = "08";  // Status by Admin - Document Issued
        public const String DOCUMENT_RECEIVED_BY_DRIVER = "09";  // Status by Driver - Document Received 
        public const String TRUCK_ORIGIN_LOADING_TIME_COMPLETED = "10";  // Status by Driver - Actual Loading time Completed // Commencement
        public const String TRUCK_REACHED_AT_DESTINATION_UNLOADING_STARTED = "25";      // Status by Driver - Reached At Destination // Unloading Started
        public const String TRUCK_REACHED_AT_DESTINATION_UNLOADING_COMPLETED = "35";    // Status by Driver - Reached At Destination // Unloading Completed
        public const String ORDER_COMPLETED = "45";    // Status by Driver - Reached At Destination // Unloading Completed
        public const String QUOTE_NOT_SELECTED = "98";  // Status by Shipper to mark other quote as Quote not Selected
        public const String QUOTE_REJECTED = "99";  // Status by Shipper to mark other quote as Quote not Selected
        public const String COMMISSION_AGENT = "1";
        public const String FLEET_OWNER = "2";
        public const String LOGISTIC_SERVICES = "3";


        public const String ALLOCATED_BUT_NOT_STARTE = "02"; // Quote Selected by Admin and Sent to shipper for acceptance
        public const String TRUCK_READY_FOR_PICKUP = "05";  // Status By Shipper / Quote accepted and awaiting driver allocation 
        public const String lOADING_STARTED = "06";  // Status By Transporter / Truck and driver allocated 
        public const String START = "07";  // Status by Driver - Ready for Pickup 
        public const String UNLOADING_START = "08";  // Status by Driver - Driver Reached  - Loading time starts
        public const String UNLOADING_COMPLETED = "45";  // Status by Admin - Document Issued

        public const String ORDER_TYPE_CODE_MOVING_GOODS_NOW = "GN";  // order type code for goods
        public const String ORDER_TYPE_CODE_HIRETRUCK = "HT";  // order type code for goods
        public const String ORDER_TYPE_CODE_MOVING_GOODS_LATER = "GL";
        public const String ORDERTYPECODEFORHOME = "H";  // order type code for HOME
        public static string DistanceUOM_KM = "KM";

        public const String RATE_TYPE_FLAG_SUPERSAVER = "S";
        public const String RATE_TYPE_FLAG_STANDERD = "B";
        public const String RATE_TYPE_FLAG_PREMIUM = "P";

        public static string PAYFORT_PURCHASE_SUCESS_STATUS = "14";

        public static string PAINTING_SERIVCES = "PT";
        public static string CLEANING_SERIVCES = "CL";
        public static string PESTCONTROL_SERIVCES = "PEST";

        public static string SUBSERVICE_PAINTING = "SS0002";
        public static string SUBSERVICE_CLEANING = "SS0003";
        public static string SUBSERVICE_PEST = "SS0006";

        public static string ORDER_CANCEL_REQUESTED = "25";
        public static string ORDER_RESCHEDULE_REQUESTED = "26";

        public const string USERTYPE = "DUBI";
        public const string USERDEFAULTPASSWORD = "TruKKer2015";
    }
}