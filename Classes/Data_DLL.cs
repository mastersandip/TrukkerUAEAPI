using BLL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


public enum RequestTypeInfo
{
    Login,
    SubmitRetrieve
}

public class UserLoginInfo
{
    public UserLoginInfo()
    {

        UserId = "";
        Password = "";
        HostIp = "";
    }

    public string UserId { get; set; }
    public string Password { get; set; }
    public string HostIp { get; set; }
}

public class GeneralInfo
{
    public GeneralInfo()
    {
        SessionId = "";

        UserId = "";
        HostIp = "";

    }
    public string SessionId { get; set; }
    public string companyId { get; set; }
    public string UserId { get; set; }
    public string HostIp { get; set; }
}

public class ObjectProfileInfo
{
    public ObjectProfileInfo()
    {
        ClassName = "";
        MethodName = "";
        FactoryType = "";
        ModuleName = "";
        MenuId = "";
        TransactionType = "";
    }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public string FactoryType { get; set; } //M-Master, T-Transaction, R-Report
    public string ModuleName { get; set; } //S-Sales Module, F-Finance Module
    public string MenuId { get; set; }
    public string TransactionType { get; set; } //Constant to get object of Particular Class (from menu_mst.transactiontype)
}

public class RequestObjectInfo
{
    public RequestObjectInfo()
    {
        UserInfo = null;
        GeneralArgs = null;
        ObjectProfile = null;
        OpArgs = null;
        ds_UploadData = null;
        msg = String.Empty;
    }

    public RequestTypeInfo RequestType { get; set; }
    public UserLoginInfo UserInfo { get; set; }
    public GeneralInfo GeneralArgs { get; set; }
    public ObjectProfileInfo ObjectProfile { get; set; }
    public Dictionary<string, object> OpArgs { get; set; }
    public dynamic ds_UploadData { get; set; }
    public string msg { get; set; }
}

public class ResponseObjectInfo
{
    public ResponseObjectInfo()
    {
        Status = 0;
        Message = String.Empty;
        msg = String.Empty;
        dt_ReturnedTables = null;
        ObjRetArgs = null;
        transactionType = "";
    }

    public int Status { get; set; }
    public string transactionType { get; set; }
    public string Message { get; set; }
    public dynamic dt_ReturnedTables;
    public string msg { get; set; }
    public Object[] ObjRetArgs;
    public string smtpHost { get; set; }
    public string smtpPort { get; set; }
    public string smtpAuth { get; set; }
    public string compPassword { get; set; }
    public string fromEmail { get; set; }
    public string smtpSecure { get; set; }
    public string fromName { get; set; }
}

public class ResponseObjectInfoNew
{
    public ResponseObjectInfoNew()
    {
        Status = 0;
        Message = String.Empty;
        msg = String.Empty;
        dt_ReturnedTables = null;
        ObjRetArgs = null;
    }

    public int Status { get; set; }
    public string transactionType { get; set; }
    public string Message { get; set; }
    public Dictionary<string,object> dt_ReturnedTables;
    public string msg { get; set; }
    public Object[] ObjRetArgs;
    public string smtpHost { get; set; }
    public string smtpPort { get; set; }
    public string smtpAuth { get; set; }
    public string compPassword { get; set; }
    public string fromEmail { get; set; }
    public string smtpSecure { get; set; }
    public string fromName { get; set; }
}

public class SendReceiveJSon
{
    public SendReceiveJSon()
    {
        requestObjectInfo = new RequestObjectInfo();
        responseObjectInfo = new ResponseObjectInfo();
    }

    public RequestObjectInfo requestObjectInfo { get; set; }
    public ResponseObjectInfo responseObjectInfo { get; set; }

    #region GetJson
    /* Method: GetJson
         * feature: convert DataTable into JSON object
         * parameters: DataTable
         * return: JSON object
         * */
    public static List<Dictionary<string, string>> getDictionaryList(List<DataRow> rowList)
    {
        DataTable dt = new DataTable();
        //JavaScriptSerializer ser = new JavaScriptSerializer();
        List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

        //Convert DataTable to List<Dictionary<string, string>> data structure
        foreach (DataRow VDataRow in rowList)
        {
            var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
            foreach (DataColumn Column in dt.Columns)
            {
                Row.Add(Column.ColumnName, VDataRow[Column].ToString());
            }
            dataRows.Add(Row);
        }
        return dataRows; // convert list to JSON string 

    }

    public static string GetJson(DataTable dt)
    {
        JavaScriptSerializer ser = new JavaScriptSerializer();
        List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

        //Convert DataTable to List<Dictionary<string, string>> data structure
        foreach (DataRow VDataRow in dt.Rows)
        {
            var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
            foreach (DataColumn Column in dt.Columns)
            {
                Row.Add(Column.ColumnName, VDataRow[Column].ToString());
            }
            dataRows.Add(Row);
        }
        return ser.Serialize(dataRows); // convert list to JSON string 

    }

    public static string GetJsonNew(DataTable dt)
    {
        JavaScriptSerializer ser = new JavaScriptSerializer();
        List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects

        //Convert DataTable to List<Dictionary<string, string>> data structure
        foreach (DataRow VDataRow in dt.Rows)
        {
            var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
            foreach (DataColumn Column in dt.Columns)
            {
                Row.Add(Column.ColumnName, VDataRow[Column].ToString());
            }
            dataRows.Add(Row);
        }
        ser.MaxJsonLength = Int32.MaxValue;
        return ser.Serialize(dataRows); // convert list to JSON string 

    }

    public static string GetJsonFromDataSet(int status, string Message, DataSet ds)
    {
        ResponseObjectInfoNew responseObjectInfo = new ResponseObjectInfoNew();

        responseObjectInfo.Status = status;
        responseObjectInfo.Message = Message;

        responseObjectInfo.dt_ReturnedTables = new Dictionary<string, object>();
        
        //Convert DataTable to List<Dictionary<string, string>> data structure
        for (int i = 0; i < ds.Tables.Count; i++)
        {
            responseObjectInfo.dt_ReturnedTables[ds.Tables[i].TableName] = ds.Tables[i];
        }

        return JsonConvert.SerializeObject(responseObjectInfo); // convert list to JSON string 
    }

    /* feature:give DataTable whose 1st column of first row contains string **columan name="returnString"
     * parameters: str //input string
     * return: DataTable
     * */
    public static DataTable GetDataTableFromString(string Str)
    {
        DataTable returnStringTable = new DataTable();
        DataColumn ret = new DataColumn("returnString");
        ret.DataType = System.Type.GetType("System.String");
        returnStringTable.Columns.Add(ret);
        DataRow Row = returnStringTable.NewRow();
        Row[ret] = Str;
        returnStringTable.Rows.Add(Row);
        return returnStringTable;
    }
    #endregion
}


