using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Utilities
{
    public class SessionInfo
    {
        private String SessionID;
        private String UserID;
        private String UserName;
        private String HostID;
        private String HostName;
        private ClientType Client_Type;
        private Module ModuleName;
        private DateTime LastActivityOn;
        public SessionInfo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public SessionInfo(String Session_ID, String User_ID, String User_Name, String Host_ID, String Host_Name, ClientType Client_Type, Module Module_Name, DateTime LastActivityOn)
        {
            this.SessionID = Session_ID;
            this.UserID = User_ID;
            this.UserName = User_Name;
            this.HostID = Host_ID;
            this.HostName = Host_Name;
            this.Client_Type = Client_Type;
            this.ModuleName = Module_Name;
            this.LastActivityOn = LastActivityOn;
        }
        public String GetSessionID()
        { return SessionID; }
        public String GetUserID()
        { return UserID; }
        public String GetUserName()
        { return UserName; }
        public String GetHostID()
        { return HostID; }
        public String GetHostName()
        { return HostName; }
        public ClientType GetClientType()
        { return Client_Type; }
        public Module GetModule()
        { return ModuleName; }
        public DateTime GetLastActivityTime()
        { return LastActivityOn; }
        public void UpdateLastActivityTime(DateTime current_datetime)
        { LastActivityOn = current_datetime; }
        //		public SessionInfo Copy(SessionInfo sessInfo)
        //		{
        //			SessionInfo clone = new SessionInfo(sessInfo.GetSessionID(),sessInfo.GetUserID(),sessInfo.GetHostID(),sessInfo.GetClientType(),sessInfo.GetModule(),sessInfo.GetLastActivityTime());
        //			return clone;
        //		}
        public SessionInfo Copy()
        {
            SessionInfo clone = new SessionInfo(this.GetSessionID(), this.GetUserID(), this.GetUserName(), this.GetHostID(), this.GetHostName(), this.GetClientType(), this.GetModule(), this.GetLastActivityTime());
            return clone;
        }
    }
}
