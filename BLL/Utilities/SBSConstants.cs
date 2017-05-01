using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Collections;
using System.IO;
using System.Configuration;

namespace SBSPayrollClient.Utilities
{
    public static class SBSConstants
    {
        //
        public const string STANDARD_VERSION = "SV";
        public const string ENTERPRISES_VERSION = "EV";

        //
        // data uploading
        public const string TT_EMPLOYEE_MASTER_MAIN_PROFILE_UPLOAD = "UPLOAD1";
        public const string TT_EMPLOYEE_PF_ESIC_GRATUITY_DETAILS = "UPLOAD2";
        public const string TT_EMPLOYEE_PAYMODE_BANK_DETAILS = "UPLOAD3";
        public const string TT_EMPLOYEE_CONTACT_DETAILS = "UPLOAD4";
        public const string TT_EMPLOYEE_FAMILY_DETAILS = "UPLOAD5";
        public const string TT_ATTENDANCE_SUMMARY_DETAILS = "UPLOAD09";
        public const string TT_EMPLOYEE_LEAVE_BALANCE_DETAILS = "UPLOAD10";
        public const string TT_EMPLOYEE_SALARY_MASTER_DETAILS = "UPLOAD20";
        public const string TT_REIMBURSEMENTS_OPENING_BALACNE = "UPLOAD21";
        public const string TT_REIMBURSEMENTS_CLOSING_BALACNE = "UPLOAD22";
        public const string TT_MONTHLY_VARIABLE_CTC_PERCENTAGE = "UPLOAD23";
        public const string TT_RAW_PUNCH_DATA_UPLOADING = "UPLOAD24";
        public const string TT_SALARY_TRANSACTION_UPLOADING = "UPLOAD25";
        public const string TT_EMPLOYEE_QUALIFICATION_DETAILS = "UPLOAD26";
        public const string TT_PRE_SERVICE_HISTORY_DATA = "UPLOAD27";
        public const string TT_DESIGNATION_MASTER = "UPLOAD28";
        public const string TT_QUALIFICATION_MASTER_UPLOAD = "UPLOAD29";
        public const string TT_GRADE_MASTER_UPLOAD = "UPLOAD30";
        public const string TT_COMPANY_CSID_MASTER = "UPLOAD31";
        public const string TT_INTERCOM_NOS = "UPLOAD32";
        public const string TT_MISS_PAYMENTS_TRANSACTION_UPLOADING = "UPLOAD33";
        public const string TT_EMPLOYEE_DEPT_CHANGE_UPLOAD = "UPLOAD34";
        public const string TT_SALARY_TRANSACTION_UPLOADING_TDS = "UPLOAD35";
        //
        #region    EMPLOYEE MASTER VALIDATION MODE  IN PARAMTER TABLE
        public const string EMPLOYEE_MASTER_PARAMETER_CODE = "EMPLOYEEMASTER";
        public const string EMPLOYEE_MASTER_VALIDATION_PARAMETER_KEY = "VALIDATIONMODE";
        public const string EMPLOYEE_CODE_AUTOGENERATION_PARAMETER_KEY = "EMPLOYEECODE_AUTO_INCREMENT"; //Y/N
        // VALIDATION MODES
        public const string FULL_VALIDATION = "FULL";
        public const string MINIMUM_VALIDATION = "MINIMUM";
        public const string LEAVE_GRADE = "LEAVE_GRADE";
        public const string LEAVEONLY = "LEAVEONLY";
        public const string GRADEONLY = "GRADEONLY";


        #endregion


        //public const String COMPANY_NAME = "HARSHA ENGINEERS LTD";
        public const String CS_TITLE = "Harsha Eng. Emp. Co. OP Credit Society Ltd";
        public const String DOC_TYPE_FOR_LOAN = "LOAN";
        public const String DOC_TYPE_FOR_CSLOAN = "CSLN";

        public const String TT_USER_SELECTION = "GUS";//User Selection

        //header and footer space
        public const String HEADER_FOTTER_SPACELENGTH = "3";

        // Punch data correction Type
        public const String CORRECTION_TYPE_ADD = "N";
        public const String CORRECTION_TYPE_DELETE = "D";
        public const String CORRECTION_TYPE_SHIFT = "S";
        public const String CORRECTION_TYPE_PROCESS = "P";
        public const String CORRECTION_TYPE_MinusMinute = "M";

        //Variable is Define for 50
        public const String FIFTY_PERCENTAGE = "50.00";

        // YES/NO
        public const String YES = "YES";
        public const String NO = "NO";
        public const String FLAG_Y = "Y";
        public const String FLAG_N = "N";

        #region Leave Codes
        public const string LEAVE_CODE_CL = "L01", LEAVE_CODE_PL = "L02";
        public const String LEAVE_CODE_SL = "L08"; // SICK LEAVE
        public const String LEAVE_CODE_SPL = "L03";  // SPECIAL LEAVE
        public const String LEAVE_CODE_ML = "L09";
        public const String LEAVE_CODE_ESI = "L04";
        public const string LEAVE_CODE_COFF = "L05", LEAVE_CODE_LWP = "L06";
        public const String CARRYFORWARDONLY = "CARRYFORWARD";
        public const String ENCASHABLEONLY = "ENCASH";
        public const String BOTH_CARRRYFORWARD_ENCASH = "CFANDENCASH";
        public const String NO_CF_AND_ENCASH = "NOCFANDENCASH";
        #endregion

        #region CONSTANTS USED IN SALARY CALCULATION

        public const String NONE = "X";

        public const String NEW_EMPLOYEE = "N";
        public const String EXISTING_EMPLOYEE = "E";
        public const String RELEIVE_EMPLOYEE = "R";
        public const String NEW_RELIEVE = "NR";//EMP JOINS AND RELIEVED ON SAME MONTH.


        //EMPLOYEE RETIREMENT YEARS
        public const int EMPLOYEE_RETIREMENT_AGE = 58;

        // esi

        public const string ESI_FIRST_PERIOD = "1";
        public const String ESI_SECOND_PERIOD = "2";


        //RETIREMENT AGE STATUS
        public const String RET_AGE_STATUS_LESSTHAN_RET_AGE = "1";
        public const String RET_AGE_STATUS_GREATER_THAN_OR_EQUALTO_RET_AGE = "2";
        public const String RET_AGE_STATUS_AGE_REACHES_TO_RETIREMENT_DURING_THE_MONTH = "3";


        //REGULAR OR SUPPLEMENTARY
        public const String SALARY_REGULAR = "R";
        public const String SALARY_SUPPLEMENTARY = "S";

        // EMPLOYEE_TYPE 
        public const String CONTRACT = "04";


        #region//ED CODES
        public const String PARAMETER_CODE_DOUBLESHIFT = "DOUBLESHIFT";   // LVS
        //public const String ED_CODE_BASIC = "E001";
        //public const String ED_CODE_PF = "D001";

        /*In before the Value of EPF is C001 and FPS is C002.It have changed changes made on 28-2-07*/
        //public const String ED_CODE_EPF = "C002";
        //public const String ED_CODE_FPS = "C001";
        //public const String ED_CODE_PRODUCTION_BONUS = "E009";



        public const string//earnings
            // ED_CODE_DOUBLE_SHIFT = "E011",      // DOUBLE SHIFT CODE   FOR SCHOOL   , DAY SCHOLORS
            ED_CODE_BASIC = "E001",
            ED_CODE_DA = "E002",
            ED_CODE_HRA = "E003",
            ED_CODE_TRANSPORT_ALLOW = "E004",
            ED_CODE_ADDITIONAL_ALLOW = "E004",
            ED_CODE_CAR_MAINTENANCE = "E007",         // earlier it was city allowance in harsha
            ED_CODE_EDUCATIONAL = "E005",
            ED_CODE_WASH_ALLW = "E006",
            ED_CODE_STAFF_ALLW = "E007",         // in bhagavathi
            ED_CODE_BASKET = "E008",
            ED_CODE_MANAGEMENTALL = "E008",
            ED_CODE_FLEXI_PAY = "E008",
            ED_CODE_HOSTEL_ALL = "E008",
            ED_CODE_PRODUCTION_BONUS = "E009",
            ED_CODE_SPECIAL_ALLOWANCE = "E010",
            ED_CODE_MEDICAL = "E011",
            ED_CODE_LTA = "E012",            // harsha
            ED_CODE_CONVEYANCE = "E013",
            ED_CODE_OVERTIME = "E020",


            ED_CODE_REIMBURSEMENT_CONVEYANCE = "E021",
            ED_CODE_ATTENDANCE_ALLOWANCE = "E021",
            ED_CODE_MISC_REIMBURSEMENT = "E014",    // used in harsha
            ED_CODE_OTHER_EARNINGS = "E014",
            ED_CODE_ARREARS = "E014",
            ED_CODE_LEAVE_ENCASHMENT = "E015",
            ED_CODE_FOOD_ALLOWANCE = "E016",
            ED_CODE_PERIODICAL_ALLOWANCE = "E017",
            ED_CODE_TELEPHONE_ALL = "E018",
            ED_CODE_HEAVY_DUTY_ALL = "E019",
            ED_CODE_TRAVELLING_FARE = "E023",
            ED_CODE_ANNUAL_BONUS = "E022",
            ED_CODE_TEA_ALLOWANCE = "E023", // BHAGAVATHI
            ED_CODE_VDA = "E024",
            ED_CODE_CITY_ALLOW = "E025", // BHAGAVATHI
            ED_CODE_CYBER_ALLOW = "E025",  // For Hidden Brains
            ED_CODE_TEA_COFEE_ALLOW = "E026", // 07-OCT-2013 
            ED_CODE_MOBILE_ALLOW = "E029", // 07-OCT-2013

            //
            ED_CODE_DRIVER_ALL = "E030", //  NEW CODES 3 SERIES  ON 12-12-2012
            ED_CODE_MAGAZINE_SUBSCRIPTION = "E031", //  NEW CODES 3 SERIES  ON 12-12-2012


            ED_CODE_OTHER_EARNING = "E032", // 07-OCT-2013
            ED_CODE_GE_BENEFIT = "E033", // 07-OCT-2013
            ED_CODE_ATTENDANCE_BONUS = "E034", // 07-OCT-2013
            ED_CODE_PERFORMANCE_BONUS = "E035", // 07-OCT-2013
            ED_CODE_INCENTIVE = "E036", // 07-OCT-2013

            //

            ED_CODE_VARIBALE_CTC = "E088",   // SPECIAL ED CODE

            // REIMBURSEMENTS 
            ED_CODE_MEDICAL_REIMBURSEMENT = "E016",
            ED_CODE_LTA_REIMBURSEMENT = "E017",
            ED_CODE_SODEX_REIMBURSEMENT = "E018",
            ED_CODE_KITTY_REIMBURSEMENT = "E019", 
            ED_CODE_MOBILE_REIMBURSEMENT = "E021",

            ///
            //deductions
            ED_CODE_PF = "D001",
            ED_CODE_PROFTAX = "D002",
            ED_CODE_E_S_I = "D003",
            ED_CODE_L_W_F = "D004",
            ED_CODE_INCOME_TAX = "D005",
            //ED_CODE_SECURITY_DEDUCTION = "D006",
            ED_CODE_ADVANCE = "D006",
            ED_CODE_BUSRENT = "D007",
            ED_CODE_OTHER_DED = "D007",
            ED_CODE_MISC_DED_1 = "D008",
            ED_CODE_MISC_DED_2 = "D009",
            ED_CODE_STAMP = "D009",
            ED_CODE_CANTEEN_DED = "D010",
            ED_CODE_CANTEEN_ALLOW = "D010",
            ED_CODE_DUP_PUNCH_CARD = "D011",
            ED_CODE_LOAN_DEDUCTION = "D011",
            ED_CODE_REV_STAMP = "D099",

            //credit society loans
            LOAN_1 = "L001",
            LOAN_2 = "L002",

            //cs others
            ED_CODE_SOC_MEM_FEE = "L004",
            ED_CODE_CS_SAVING = "L003",
            ED_CODE_SOCIETY_SHARE = "L005",
            ED_CODE_RE_CONVE = "E021",

            //company loans            
            ED_CODE_LIC = "L007",
            ED_CODE_LOAN_ADVANCE = "L009",
            ED_CODE_SALARY_ADVANCE = "L009",
            ED_CODE_SALARY_ADVANCE_BHAGAVATHI = "L011",
            ED_CODE_SECURITY_DEPOSIT_AS_LOAN = "L008",  // SALARY SECUTIRY DEPOSIT  FOR RAJASTHAN HOSPITAL
            LOAN_HOUSE_COMPANY = "L010",
            ED_CODE_MEDICAL_LOAN_VULCAN = "L010", // RAO* 
            LOAN_VEHICLE_COMPANY = "L011",
            PERSONAL_LOAN = "L012",//added by mahesh
            ED_CODE_SECURITY_DEDUCTION = "L014",// activate this and use this code for all clients 10-10-2012 rao
            //contributions
            ED_CODE_FPS = "C001",
            ED_CODE_EPF = "C002",
            ED_CODE_ESI_CONTRIB = "C003";

        #endregion

        //Insurance fund percentage.
        public const Decimal INSURANCE_FUND_PERCENTAGE = 0.005M;
        public const Decimal INSURANCE_FUND_MAX_LIMIT = 33.00M;
        public const Decimal INSURANCE_FUND_MAX_LIMIT_NEW = 75.00M;
        public const Decimal ADMINISTRATION_CHARGES_PERCENTAGE = 0.011m;
        public const Decimal ADMINISTRATION_CHARGES_PERCENTAGE_REVISED = 0.0085m; // this new charges revised from 01-Jan-2015 by PF Department
        public const Decimal INSPECTION_CHARGES_PERCENTAGE = 0.00005m;
        public const Decimal ANUAL_BONUS_PERCENTAGE = 0.2M;//20% OF BASIC WILL BE GIVEN AS ANUAL BONUS
        public const Decimal PF_PERCENTAGE_IN_COSTING = 0.131M;//13.10 % OF BASIC HAVE TO SHOW IN COSTING REPORT.
        public const Decimal GRAUTITY_CALCULATION_VALUE = 1.25M;//GRAUTITY WILL BE CALCULATED 1.25 TIMES OF 
        public const string GRATUITY_ELIGIBLE_YEARS = "5";

        public const String ALL = "99";
        public const String PERIOD_OPEN = "O";
        public const String PERIOD_CLOSED = "C";

        //calculation type
        public const String CALC_TYPE_AUTOMATIC = "1";
        public const String CALC_TYPE_MANUAL = "2";

        //eligibility method
        public const String ELIGIBILIY_METHOD_FIXED = "1";
        public const String ELIGIBILIY_METHOD_PER_OF_BASEFORMULA = "2";
        public const String ELIGIBILIY_METHOD_AS_PERFORMULA = "3";
        public const String ELIGIBILIY_METHOD_FIXED_CUM_FORMULA = "4";

        //base calculation depends on
        public const String BASE_CALC_MONTHLY_RATE = "1";
        public const String BASE_CALC_CURR_MON_EARNINGS = "2";
        public const String BASE_CALC_CURR_MON_EARNINGS_INCLUSIVE_OF_ARREARS = "3";

        //extra paiddays allowed
        public const String EXTRA_PAID_DAYS_ALLOWED = "1";
        public const String EXTRA_PAID_DAYS_NOT_ALLOWED = "2";

        //ed_perday calculation
        public const String ED_PERDAY_ONE = "1";
        public const String ED_PERDAY_TWO = "2";
        public const String ED_PERDAY_THREE = "3";
        public const String ED_PERDAY_FOUR = "4";  // new by 26 days


        public const String ELIGIBLE_DAYS_ONE = "1";//"Total Month Days"
        public const String ELIGIBLE_DAYS_TWO = "2";//"Total Month Days - weeklyoff";
        public const String ELIGIBLE_DAYS_THREE = "3";//"Total Month Days - (weeklyoff + Public holidays)"
        public const String ELIGIBLE_DAYS_FOUR = "4";//Salary Calculation Days
        public const String ELIGIBLE_DAYS_FIVE = "5";//"Present Days";
        public const String ELIGIBLE_DAYS_SIX = "6";//"Total Month Days - Absent days"
        public const String ELIGIBLE_DAYS_SEVEN = "7";//"Present Days + Paid leaves"
        public const String ELIGIBLE_DAYS_EIGHT = "8";//"Total Monthdays with six days limit";
        public const String ELIGIBLE_DAYS_NINE = "9";//"Present Days + Paid Leaves + Public Holidays"
        public const String ELIGIBLE_DAYS_TEN = "10";//"Present Days + Paid Leaves + weekly off +  Public Holidays"
        public const String ELIGIBLE_DAYS_ELEVEN = "11";// specific to  Bhagavathi //14-02-2012
        public const String ELIGIBLE_DAYS_TWELVE = "12";// specific to  Bhagavathi //14-02-2012
        public const String ELIGIBLE_DAYS_THIRTEEN = "13";// specific to  Bhagavathi //14-02-2012
        public const String ELIGIBLE_DAYS_FOURTEEN = "14";// specific to Texspin (Present Days calculated by actual hours worked) Jenish 26-May-2014
        public const String ELIGIBLE_DAYS_NINETY_EIGHT = "98";// specific to VULCAN 20-9-2013
        public const String NOT_DEPENDS_ONDAYS = "99";//"Not dependens on days";

        //periodicity of payment
        public const String PERIODICITY_MONTHLY = "M";
        public const String PERIODICITY_QUARTERLY = "Q";
        public const String PERIODICITY_HALFYEARLY = "H";
        public const String PERIODICITY_YEARLY = "Y";
        public const String PERIODICITY_MANUAL = "2";
        public const String PERIODICITY_SPECIFIC_MONHTS = "SM";

        //pay type
        public const String PAYTYPE_PAYSLIP = "1";
        public const String PAYTYPE_REIMBURSEMENT = "2";

        //random required
        public const String RANDOM_REQ_YES = "Y";
        public const String RANDOM_REQ_NO = "N";

        //percent/fixed/formula
        public const String FIXED_ = "1";
        public const String PERCENTAGE = "2";
        public const String FORMULA = "3";

        //incremental/absolute
        public const String INCREMENTAL = "1";
        public const String ABSOLUTE = "2";

        //daily/monthly rate
        public const String DAILY_RATE = "1";
        public const String MONTHLY_RATE = "2";
        public const string DAILY_RATE_D = "D";
        public const string MONTHLY_RATE_M = "M";

        //ed_type
        public const String ED_TYPE_EARNING = "E";
        public const String ED_TYPE_DEDUCTION = "D";
        public const String ED_TYPE_CONTRIBUTIONS = "C";
        public const String ED_TYPE_REIMBURSEMENTS = "R";
        public const String ED_TYPE_COMPANY_LOANS = "CL";
        public const String ED_TYPE_SOCIETY_LOANS = "SL";
        public const String ED_TYPE_SOCIETY_OTHERS = "SO";
        public const String ED_TYPE_OTHER_LOANS = "OL";
        public const String ED_TYPE_OPTIONAL_LOANS = "L";

        //ed eligible/not eligible
        public const String ED_ELIGIBLE = "1";
        public const String ED_NOT_ELIGIBLE = "2";

        //appear on payslip
        public const String APPEAR_ONPAYSLIP_YES = "Y";
        public const String APPEAR_ONPAYSLIP_NO = "N";

        //PF Applicable/not applicable
        public const String PF_APPLICABLE = "Y";
        public const String PF_NOT_APPLICABLE = "N";

        //ESI Applicble/ not applicable
        public const String ESI_APPLICABLE = "Y";
        public const String ESI_NOT_APPLICABLE = "N";

        //Parameter_key values for ED codes defined in parameter table.
        public const String CTC_PARAM_KEY = "CTC";
        public const String PF_PARAM_KEY = "PF";
        public const String ESI_PARAM_KEY = "ESI";
        public const String BUSRENT_PARAM_KEY = "BUS_RENT";
        public const String CS_SAVING_PARAM_KEY = "CS_SAVING";
        public const String OVERTIME_PARAM_KEY = "OT";
        public const String WORKINGDAYS_PARAM_KEY = "W_DAYS";
        public const String OT_HOURS_PARAM_KEY = "OT_HRS";
        public const String DAILY_MONTHLY_PARAM_KEY = "D_M";
        public const String AGE_PARAM_KEY = "AGE";
        public const String C_DAYS_PARAM_KEY = "C_DAYS";//The days to calculate F.P.S in case if the age becomes 58 in   
        //between month
        public const String MONTHDAYS_PARAM_KEY = "M_DAYS";//FOR TOTAL PERIOD DAYS.
        public const String BASE_VALUE_PARAM_KEY = "BASE";//TO STORE BASE VALUE AFTER CALCUALATION OF BASE VALUE.



        //Is applicable for add_deduct hours
        public const String ADD_DEDUCT_HOURS_APPLICABLE = "1";
        public const String ADD_DEDUCT_HOURS_NOTAPPLICABLE = "2";

        //Working Hours per shift
        public const Decimal NO_OF_WORKING_HOURS = 8;

        #region //Rounding Options

        //rounding for Netpay
        public const String ROUNDING_NEAREST_RUPEE = "1";
        public const String ROUNDING_UPTO_2_DECIMAL_PTS = "2";
        public const String ROUNDING_CARRY_FORWARD_PAISE = "3";
        public const string ROUNDING_NONE = "4";
        public const string ROUNDING_NEAREST_FIVE_RUPEES = "5";
        public const string ROUNDING_NEAREST_TEN_RUPEES = "6";
        public const string ROUNDING_NEAREST_HUNDRED_RUPEES = "7";


        #endregion


        #endregion

        #region constants used Employee Master

        public const String ADD = "A";
        public const String MODIFY = "M";

        //pf no prefix value
        //public const String PF_NO_PREFIX_VALUE = "GJ/7742/";
        public const string COMPANY_PF_CODE = "7742";

        //employee nationality
        public const String NATIONALITY_INDIAN = "INDIAN";

        //relations
        public const String RELATION_FATHER = "01";
        public const String RELATION_MOTHER = "02";
        public const String RELATION_WIFE = "03";
        public const String RELATION_HUSBAND = "04";
        public const String RELATION_BROTHER = "05";
        public const String RELATION_SISTER = "06";
        public const String RELATION_GRAND_MOTHER = "07";
        public const String RELATION_GRAND_FATHER = "08";
        public const String RELATION_SON = "09";
        public const String RELATION_DAUGHTER = "10";
        public const String RELATION_MOTHERS_FATHER = "11";

        //martail_status

        public const String BACHELOR = "01";
        public const String SPINSTER = "02";
        public const String MARRIED = "03";
        public const String DIVORCEE = "04";

        //gender
        public const String MALE = "M";
        public const String FEMALE = "F";
        //payment mode
        public const String PAY_MODE_BANK_TRANSFER = "01";
        public const String PAY_MODE_CASH = "02";
        public const String PAY_MODE_DEMAND_DRAFT = "03";
        public const String PAY_MODE_CHEQUE = "04";
        public const String PAY_MODE_NON_BANKING = "05";

        //Bank code
        public const String BANK_ICICI = "01";
        public const String BANK_BOB = "02";
        public const String BANK_CANARA = "03";

        #region EMPLOYEE PRESENT STATUS

        public const String PRESENT_STATUS_ACTIVE = "01";
        public const String PRESENT_STATUS_RETIRED = "02";
        public const String PRESENT_STATUS_RESIGNED = "03";
        public const String PRESENT_STATUS_SUSPENDED = "04";
        public const String PRESENT_STATUS_TERMINATED = "05";
        public const String PRESENT_STATUS_EXPIRED = "06";
        public const String PRESENT_STATUS_LEFT = "07";
        public const String PRESENT_STATUS_TRANSFER = "08";

        #endregion

        #region Employee Type
        public const String EMPLOYEE_TYPE_MANAGEMENT = "01";
        public const String EMPLOYEE_TYPE_STAFF = "02";
        public const String EMPLOYEE_TYPE_STAFF_TECHNICAL = "05";//ADDED BY MAHESH
        public const String EMPLOYEE_TYPE_WORKER = "03";
        public const String EMPLOYEE_TYPE_ONCONTRACT = "04";
        #endregion

        #region Employee Service Type

        public const string TRAINEES = "02";
        public const string APPRENTICE = "07";
        public const string REGULAR = "01";
        public const String PROBATION = "03";
        public const String ONCONTRACT = "04";
        public const string DAILY = "05";
        public const String TEMP = "06";
        public const String CONSULTANT = "99"; // SPECIAL CODE GIVEN , SALARY WILL NOT BE CALCULATED FOR THESE

        #endregion

        #endregion//Employee master

        #region Constants Used for Parameter tables

        public const String TABLE_NAME_PLACE_MASTER = "place_master";
        public const String TABLE_NAME_BANK_MASTER = "bank_master";
        public const String TABLE_NAME_DEPT_MASTER = "department_master";
        public const String TABLE_NAME_COST_CENTER_MASTER = "cost_center_master";
        public const String TABLE_NAME_COUNTRY_MASTER = "country_mst";
        public const String TABLE_NAME_PRESENT_STATUS_MASTER = "employee_present_status";
        public const String TABLE_NAME_PAYMODE_MASTER = "paymode_master";
        public const String TABLE_NAME_QUALIFICATION_MASTER = "qualification_mst";
        public const String TABLE_NAME_RELATION_MASTER = "relation_mst";
        public const String TABLE_NAME_MARTIAL_STATUS_MASTER = "marital_status_master";
        public const String TABLE_NAME_PUBLIC_HOLIDAY_REF_LIST = "public_holiday_reference_list";
        public const String TABLE_NAME_SHIFT_CATEGORY_DEFINITION = "shift_category_definition";
        public const String TABLE_NAME_SERVICE_TYPE_MASTER = "servicetype_master";
        public const String TABLE_NAME_EMPLOYEE_TYPE_MASTER = "employee_type_master";
        public const String TABLE_NAME_WEEKLYOFF_GROUP_MASTER = "weekly_off_reference_list";
        public const String TABLE_NAME_GRADE_MASTER = "grade_master";
        public const String TABLE_NAME_DESIGNATION_MASTER = "designation_master";
        public const String TABLE_NAME_LOAN_PROVIDERS_MASTER = "loan_providers_table";
        public const String TABLE_NAME_STATE_MASTER = "state_Definition_Table";
        public const String TABLE_NAME_CITY_MASTER = "city_Definition_Table";


        #region Transaction Types

        public const String TT_PLACE_MASTER = "115N";//place master.
        public const String TT_DEPT_MASTER = "099N";//DEPERATMENT MASTER
        public const String TT_BANK_MASTER = "117N";// "bank_master";
        public const String TT_COST_CENTER_MASTER = "118N";// "cost_center_master";
        public const String TT_COUNTRY_MASTER = "119N";// "country_mst";
        public const String TT_PRESENT_STATUS_MASTER = "120N";// "employee_present_status";
        public const String TT_PAYMODE_MASTER = "121N";// "paymode_master";
        public const String TT_QUALIFICATION_MASTER = "122N";// "qualification_mst";
        public const String TT_RELATION_MASTER = "123N";// "relation_mst";
        public const String TT_MARTIAL_STATUS_MASTER = "124N";// "marital_status_master";
        public const String TT_PUBLIC_HOLIDAY_REF_LIST = "125N";// "public_holiday_reference_list";
        public const String TT_SHIFT_CATEGORY_DEFINITION = "126N";// "shift_category_definition"
        public const String TT_DESIGANTION_MASTER = "110N";//DESIGNATION MASTER
        public const String TT_GRADE_MASTER = "112N";//GRADE MASTER
        public const String TT_SERVICE_TYPE_MASTER = "114N";//EMPLOYEE SERVICE TYPE MASTER
        public const String TT_LOAN_PROVIDERS_MASTER = "127N";//LOAN PROVIDERS TABLE
        public const String TT_EMPLOYEETYPE_MASTER = "113N"; //EMPLOYEE TYPE MASTER
        public const String TT_CITY_MASTER = "128C";//STATE MASTER
        public const String TT_STATE_MASTER = "129S";//CITY MASTER
        public const String TT_WEEKLYOFF_GROUP_CREATION = "105G";//CITY MASTER


        #endregion

        #endregion

        public const String ACTIVE = "A";
        public const String DEACTIVE = "D";
        public const String LOAN_CLOSED = "C";
        public const String INSURANCE_PREMIUM = "L007";
        public const String VEHILCE_LOAN = "L010";
        public const String EMI_OUTSIDE_HOU_LOAN = "L013";//ADDED BY MAHESH

        //TYPE OF LOAN
        public const String NEW_LOAN = "1";
        public const String PART_PAYMENT = "2";
        public const String CONSOLIDATION_WITH_PREV_LOANS = "3";
        public const String CONSOLIDATION_OF_MULTIPLE_DISBURSEMENTS = "4";

        //LOAN PROVIDERS
        public const String PROVIDER_ORG = "01";
        public const String PROVIDER_HDFC = "02";
        public const String PROVIDER_LIC = "03";

        //INTEREST CALCULATION MODE
        public const String BASED_ON_YEARDAYS = "1";
        public const String BASED_ON_ACTUAL_YEARDAYS = "2";
        public const String BASED_ON_MONTHDAYS = "3";
        public const String BASED_ON_ACTUAL_MONTHDAYS = "4";

        //RECOVERY MODE
        public const String INTEREST_ONLY = "I";
        public const String EMI = "EMI";
        public const String SIMPLE_NO_INTEREST_ON_INTEREST = "S1";
        public const String SIMPLE_WITH_INTEREST_ON_INTEREST = "S2";
        public const String VARIABLE_INSTALLMENT = "V";

        public const Decimal MEMBERSHIP_FEE = 10;
        public const Decimal INITIAL_SHARE_AMT = 100;

        //TRANSACTIONS  TYPE
        public const String FLAG_INSURANCE = "05";
        public const String FLAG_MEM_FEE = "10";
        public const String FLAG_SAVINGS = "20";
        public const String FLAG_SHARE = "30";
        public const String FLAG_CONSOLIDATED = "40";
        public const String FLAG_SUPPLEMENTARY = "50";
        public const String FLAG_INSTALLMENTS = "60";
        public const String FLAG_NEWLOAN = "03";

        // CS SETTLEMENT TRANSACTIONS TYPE 
        public const String FLAG_SAVINGS_PAID_CASH = "65";
        public const String FLAG_SAVINGS_PAID_CHEQUE = "67";
        public const String FLAG_SAVINGS_PAID_TRANSFER_TO_LOANS = "70";
        public const String FLAG_LOANS_REPAID__CASH = "75";
        public const String FLAG_LOANS_REPAID_CHEQUE = "77";
        public const String FLAG_LOANS_REPAID_FROM_SAVINGS = "80";
        public const String FLAG_LOANS_REPAID_BY_ORGANIZATION = "87";

        public const String TT_OVERTIME_DETAILS_ENTRY = "108N"; //Overtime Details Entry
        public const String TT_OVERTIME_DETAILS_MODIFICATION = "108M"; //Overtime Details Modification
        public const String TT_COFF_DETAILS_ENTRY = "127N"; //Coof Details Entry
    }
}

