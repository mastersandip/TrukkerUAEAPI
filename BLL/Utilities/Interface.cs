using System;
using System.Collections;
using System.Data;

namespace BLL.Utilities
{
    public enum ClientType
    {
        DeskTop,
        Browser
    }

    public enum Module
    {
        SND,
        INVENTORY,
        BOND
    }
    [Serializable]
    public class LoginInfo
    {
        //***************************************************************************************
        //Information in this static class is set by the server if the user is  authenticated 
        //***************************************************************************************
        //Unique ID given to user by the server
        public String SessionId;
        //Any message returned by server
        public String ServerMessage;
        //This DataSet contains information about all those
        //business areas in which the user is allowed to operate
        public DataSet ds_BusinessAreaInfo;
        //External Name for Organization Levels
        public ArrayList LevelNames;
        //Total levels provided at the login time
        public Byte LoginLevel;
        //Status of checkin procedure
        //1:Login is successful
        //2:Invalid userid or password
        //3:Account deactivated
        //4:User is logging in for the first time.User must be enforced to change the password
        //5:Userid and Password are valid but password has expired.User must be enforced to change the password
        //6:Userid and Password are valid but user don't rights for any business area				
        //7:Database exception
        //8:Unknown exception on data layer
        public Byte LoginStatus;
        // When server call is made at Login time and there is only One Level of ha_id,syncHelp is retreived 
        // to update local files ,and the status of Helplist is send to client to update the files
        public String HelpStatus;
        public LoginInfo()
        {
            SessionId = null;
            ServerMessage = null;
            ds_BusinessAreaInfo = null;
            LevelNames = null;
            LoginLevel = 0;
            LoginStatus = 0;
            HelpStatus = null;
        }
    };

    /// <summary>
    /// Class BLReturnObject gets results.
    /// </summary>
    [Serializable]
    public class BLReturnObject
    {
        //Any message returned by server
        public String ServerMessage;
        //Status Code returned by server
        //0:Failure
        //1:Success
        public byte ExecutionStatus;
        //Blob
        public DataTable[] dt_ReturnedTables;
        //Blob
        public String returnsomething;
        public BLReturnObject()
        {
            ServerMessage = null;
            dt_ReturnedTables = null;
            ExecutionStatus = 0;
            returnsomething =  null;
        }
    }

    public interface IServer
    {
        /* GeneralArgs[0] = SessionID
         * GeneralArgs[1] = SA_ID*/
        //BLReturnObject SaveMaster(int ActionCode,String[] OpArgs,String[] GeneralArgs,DataWindowChanges[] dwc_UploadData);		
        //BLReturnObject Submit(String[] ObjectProfile,String[] OpArgs,String[] GeneralArgs,DataWindowChanges[] dwc_UploadData);
        BLReturnObject Submit(String[] ObjectProfile, String[] OpArgs, String[] GeneralArgs, DataSet ds_UploadData);
        BLReturnObject Retrieve(String[] ObjectProfile, String[] OpArgs, String[] GeneralArgs, DataSet ds_UploadData);
    }

    //Retrieve data as well as structure from server 
    [Serializable]
    public class QueryReturnObject
    {
        //Any message returned by server
        public String ServerMessage;
        //Status Code returned by server
        //0:Failure
        //1:Success
        public byte ExecutionStatus;
        //Blob
        //public DataWindowFullState[] dwf_ReturnedTables;
        public DataTable[] dt_ReturnedTables;

        public QueryReturnObject()
        {
            ServerMessage = null;
            dt_ReturnedTables = null;
            ExecutionStatus = 0;
        }
    }

    public interface IExecuteQuery
    {
        QueryReturnObject ExecuteQuery(String[] ObjectProfile, String[] OpArgs, String[] GeneralArgs, DataSet ds_UploadData);
    }

    [Serializable]
    public class SBSServerException : System.Runtime.Remoting.RemotingException, System.Runtime.Serialization.ISerializable
    {
        private int _code = 0;
        private String _exceptionMessage;
        public override String Message
        {
            get
            {
                return _exceptionMessage;
            }
        }
        public int Code
        {
            get
            {
                return _code;
            }
        }
        public SBSServerException(int Code, String Message)
        {
            this._code = Code;
            this._exceptionMessage = Message;
        }
        public SBSServerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _code = (int)info.GetValue("Code", typeof(int));
            _exceptionMessage = (string)info.GetValue("ExceptionMessage", typeof(string));
        }
        #region ISerializable Members

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // Add SBSServerException.GetObjectData implementation
            info.AddValue("Code", _code);
            info.AddValue("ExceptionMessage", _exceptionMessage);

        }

        #endregion
    }
}
