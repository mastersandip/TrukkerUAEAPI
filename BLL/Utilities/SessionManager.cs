using System;
using System.Collections;
using System.Configuration;

namespace BLL.Utilities
{
    public class SessionManager
    {
        private static ArrayList SessionList = new ArrayList();

        ~SessionManager()
        {
            ServerLog.Log("SESSIONMANAGER OBJECT DESTROYED.THIS MUST NEVER HAPPEN.");
        }

        public static String GenerateNewSession(String UserID, String UserName, String HostIP, String HostName, ClientType Client_Type, Module ModuleType)
        {
            int rand_num;
            lock (typeof(SessionManager))
            {
                Random rand = new Random();
                int min = Convert.ToInt16(ConfigurationManager.AppSettings.Get("MinimumSession"));
                int max = Convert.ToInt16(ConfigurationManager.AppSettings.Get("MaximumSession"));
                rand_num = rand.Next(min, max);
                SessionInfo sinfo;

                //Check whether random number is unique or not
                for (int i = 0; i < SessionList.Count; i++)
                {
                    sinfo = (SessionInfo)SessionList[i];
                    if (sinfo.GetSessionID() == rand_num.ToString()) //session number is not unique
                    {
                        if (SessionList.Count < max - min) //check number of users
                        {
                            rand_num = rand.Next(min, max);
                            i = -1;
                        }
                        else
                        {
                          //  ServerLog.Log("Connection Refused:Maximum session limit exceeded. UserID-" + UserID + ",HostIP-" + HostIP);
                            return "-1";
                        }
                    }
                }
                //Session id is unique then  add session-id to arraylist
                SessionInfo objSessionInfo = new SessionInfo(rand_num.ToString(), UserID, UserName, HostIP, HostName, Client_Type, ModuleType, DateTime.UtcNow);
                SessionList.Add(objSessionInfo);
                //((SBS_SND_BL.frmSBSServer)Application.OpenForms[0]).SetTotalConnections(GetTotalSessions());
                //((SBS_SND_BL.frmSBSServer)Application.OpenForms[0]).AddNewSession(objSessionInfo);
            }
            //ServerLog.Log("USER : " + UserID + " HostIP - " + HostIP + " logged in...");
         //   ServerLog.Log(UserID + "   " + rand_num.ToString() + "  " + HostIP + "   " + DateTime.Now + "    " + " LogIn Successfully");
            return rand_num.ToString();
        }
        public static void TerminateSession(String SessionID)
        {
            SessionInfo sinfo;
            for (int i = 0; i < SessionList.Count; i++)
            {
                sinfo = (SessionInfo)SessionList[i];
                if (sinfo.GetSessionID() == SessionID)
                {
                    //ServerLog.Log("USER:" + SessionManager.GetUserID(SessionID) + " logged out successfully...");
                    //ServerLog.Log("USER : " + SessionID + " logged out successfully...");
                    ServerLog.Log(sinfo.GetUserID() + "   " + SessionID + "  " + sinfo.GetHostID() + "   " + DateTime.UtcNow + "   " + "Logout Successfully");
                    SessionList.RemoveAt(i);
                    return;
                }
            }
            ServerLog.Log("ERROR:Attempting to terminate an unavailable SESSION");
        }
        public static SessionInfo IsValidSession(String SessionID)
        {
            SessionInfo sinfo;
            for (int i = 0; i < SessionList.Count; i++)
            {
                sinfo = (SessionInfo)SessionList[i];
                if (sinfo.GetSessionID() == SessionID)
                {
                    DateTime dtLastActivity = DateTime.UtcNow;
                    sinfo.UpdateLastActivityTime(dtLastActivity);
                    //return sinfo.Copy(sinfo);
                    //((SBS_SND_BL.frmSBSServer)Application.OpenForms[0]).UpdateSession(SessionID, dtLastActivity);
                    return sinfo.Copy();
                }
            }
            return null;
        }
        public static String GetUserID(String SessionID)
        {
            SessionInfo sinfo;
            for (int i = 0; i < SessionList.Count; i++)
            {
                sinfo = (SessionInfo)SessionList[i];
                if (sinfo.GetSessionID() == SessionID)
                    return sinfo.GetUserID();
            }
            return "";
        }
        public static Boolean TerminateAllSessions()
        {
            SessionList.Clear();
            return true;
        }
        public static int GetTotalSessions()
        {
            return SessionList.Count;
        }
        public static ArrayList GetAllCurrentSessions()
        {
            return SessionList;
        }
        //		public static String GetHostID(String SessionID)
        //		{			
        //			SessionInfo sinfo;
        //			for(int i=0;i<SessionList.Count;i++)
        //			{
        //				sinfo = (SessionInfo)SessionList[i];				
        //				if(sinfo.GetSessionID() == SessionID) //session number is not unique
        //					return sinfo.GetHostID();
        //			}
        //			return "";
        //		}
        //		public static ClientType GetClientType(String SessionID)
        //		{			
        //			SessionInfo sinfo;
        //			for(int i=0;i<SessionList.Count;i++)
        //			{
        //				sinfo = (SessionInfo)SessionList[i];				
        //				if(sinfo.GetSessionID() == SessionID) 
        //					return sinfo.GetClientType();
        //			}
        //			return 0;
        //		}
        //		public static Module GetModule(String SessionID)
        //		{			
        //			SessionInfo sinfo;
        //			for(int i=0;i<SessionList.Count;i++)
        //			{
        //				sinfo = (SessionInfo)SessionList[i];				
        //				if(sinfo.GetSessionID() == SessionID)
        //					return sinfo.GetModule();
        //			}
        //			return 0;
        //		}
    }
}
