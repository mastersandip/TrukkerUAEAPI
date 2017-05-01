using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BLL.Utilities;
using System.Data.SqlClient;
using System.Text;


namespace BLL.Utilities
{
    public class Login : ServerBase
    {

        #region CONSTRUCTOR/DESTRUCTOR
        public Login()
        {

        }
        //~Login()
        //{
        //    if (DBConnection != null)
        //    {
        //        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
        //    }
        //}
        #endregion

        public void Dispose()
        {
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public LoginInfo oldCheckIn(String UserID, String Password, String countryCODE, String HostIP, String HostName, ClientType ClientType, Module ModuleType, String HelpList, String hdn_FBLoginFlag)
        {
            //DS_Masters objDS_Masters = new DS_Masters();
            LoginInfo obj_LoginInfo = new LoginInfo();
            obj_LoginInfo.ds_BusinessAreaInfo = new DataSet();

            String SessionID;
            try
            {
                #region BASIC VALIDATIONS
                //-------------------------------------------------------------
                //Basic Validations //MessageBox.Show("Basic Validation");
                //-------------------------------------------------------------
                UserID = UserID.Trim();
                if (hdn_FBLoginFlag == string.Empty || hdn_FBLoginFlag == null || hdn_FBLoginFlag == "")
                    Password = Password.Trim();

                //Store the information to be returned back to client in this object

                //Both userid and password are compulsory
                if (UserID == "")
                {

                    obj_LoginInfo.ServerMessage = "Please provide USERID";
                    obj_LoginInfo.LoginStatus = 2;
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return obj_LoginInfo;
                }
                if (hdn_FBLoginFlag == string.Empty || hdn_FBLoginFlag == null || hdn_FBLoginFlag == "")
                {
                    if (Password == "")
                    {
                        obj_LoginInfo.ServerMessage = "Please provide PASSWORD";
                        obj_LoginInfo.LoginStatus = 3;
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return obj_LoginInfo;
                    }
                }
                #endregion

                //-------------------------------------------------------------
                DBConnection.Open();
                //-------------------------------------------------------------
                //Major Validations
                //-------------------------------------------------------------
                //Authenticate the user	Get Existing Information of user Byte ret_val;

                #region GET USER INFORMANTION
                DataTable dt_usermst = new DataTable();
                String msg = "";

                //pooja vachhani on 2/7/15
                dt_usermst = this.GetUserTypeFromUserId(DBDataAdpterObject, UserID, ref msg);

                if (dt_usermst != null && dt_usermst.Rows.Count == 1)
                {
                    if (hdn_FBLoginFlag == null && hdn_FBLoginFlag == "")
                    {
                        if (dt_usermst.Rows[0]["password"].ToString() != Password)
                        {
                            //Validate Password
                            //ServerLog.InvalidLoginLog(dt_usermst.Rows[0]["user_id"].ToString() + " - Attemting to login With Invalid Password on " + DateTime.Now + " By IP " + HostIP.ToString());
                            obj_LoginInfo.ServerMessage = "Invalid Password.";
                            obj_LoginInfo.LoginStatus = 5;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }
                    String country_code = dt_usermst.Rows[0]["country_code"].ToString();
                    DataTable dtUserCountry = getUserCountry(DBDataAdpterObject, country_code, ref msg);
                    if (dtUserCountry == null || dtUserCountry.Rows.Count == 0)
                    {
                        //No rows found
                        //ServerLog.InvalidLoginLog(UserID.ToString() + " - Attemting to login. " + DateTime.Now + " By IP " + HostIP.ToString() + "User Details Not Found");
                        obj_LoginInfo.ServerMessage = "Country is Deactivated";
                        obj_LoginInfo.LoginStatus = 15;
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return obj_LoginInfo;
                    }

                    if (dt_usermst.Rows[0]["user_type"].ToString() != "RU" || dt_usermst.Rows[0]["user_type"].ToString() != "AG")
                    {
                        if (dt_usermst.Rows[0]["is_active"].ToString() == "N")
                        {
                            obj_LoginInfo.ServerMessage = "You Are Not Active User.";
                            obj_LoginInfo.LoginStatus = 10;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }
                    //pooja vachhani on 20/6/15
                    if (dt_usermst.Rows[0]["user_type"].ToString() != "SA")
                    {
                        DataTable dtUserGrpRights = this.GetUserGroupRights(DBDataAdpterObject, UserID, ref msg);
                        if (dtUserGrpRights != null)
                        {
                            if (dtUserGrpRights.Rows.Count > 0)
                            {
                                if (dtUserGrpRights.Rows[0]["is_active"].ToString() == "N")
                                {
                                    obj_LoginInfo.ServerMessage = "Your Role is not activated now.";
                                    obj_LoginInfo.LoginStatus = 8;
                                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                    return obj_LoginInfo;
                                }
                            }
                        }
                        else
                        {
                            obj_LoginInfo.ServerMessage = "Your Group is not defined";
                            obj_LoginInfo.LoginStatus = 9;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }



                    if (dt_usermst.Rows[0]["user_type"].ToString() != "SA")
                    {
                        //dt_usermst = null;
                        //dt_usermst = this.GetUserInformation(DBDataAdpterObject, UserID,ref msg);

                        HttpContext.Current.Session["country_code"] = dt_usermst.Rows[0]["country_code"].ToString();
                        if (dt_usermst == null)
                        {
                            //No rows found
                            //ServerLog.InvalidLoginLog(UserID.ToString() + " - Attemting to login. " + DateTime.Now + " By IP " + HostIP.ToString() + "User Details Not Found");
                            obj_LoginInfo.ServerMessage = "User Detail Not Found.";
                            obj_LoginInfo.LoginStatus = 4;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }




                    if (dt_usermst.Rows[0]["end_date"] == DBNull.Value)
                    {
                        if (Convert.ToDateTime(dt_usermst.Rows[0]["start_date"]) >= System.DateTime.UtcNow)
                        {
                            //ServerLog.InvalidLoginLog(dt_usermst.Rows[0]["user_id"].ToString() + " - Attemting to login With Expired Account on " + DateTime.Now + " By IP " + HostIP.ToString());
                            obj_LoginInfo.ServerMessage = "Your Account has been expire.";
                            obj_LoginInfo.LoginStatus = 6;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }
                    else if (!(Convert.ToDateTime(dt_usermst.Rows[0]["start_date"]) <= System.DateTime.UtcNow && Convert.ToDateTime(dt_usermst.Rows[0]["end_date"]) >= System.DateTime.UtcNow))
                    {
                        //ServerLog.InvalidLoginLog(dt_usermst.Rows[0]["user_id"].ToString() + " - Attemting to login With Account Which Not Start Yet on " + DateTime.Now + " By IP " + HostIP.ToString());
                        obj_LoginInfo.ServerMessage = "Your Account not Started Yet.";
                        obj_LoginInfo.LoginStatus = 11;
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return obj_LoginInfo;
                    }

                    //if (dt_usermst.Rows[0]["user_status_flag"].ToString() == "D")
                    //{

                    //    //User account is deactivated
                    //    //ServerLog.InvalidLoginLog(dt_usermst.Rows[0]["user_id"].ToString() + " - Attemting to login With Deactivated Account on " + DateTime.Now + " By IP " + HostIP.ToString());
                    //    obj_LoginInfo.ServerMessage = "Your account has been deactivated";
                    //    obj_LoginInfo.LoginStatus = 12;
                    //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    //    return obj_LoginInfo;
                    //}
                    if (dt_usermst.Rows[0]["user_type"].ToString() == "RU" || dt_usermst.Rows[0]["user_type"].ToString() == "AG")
                    {
                        if (dt_usermst.Rows[0]["is_active"].ToString() == "N")
                        {
                            obj_LoginInfo.ServerMessage = "Please active your membership from your Gmail Account.";
                            obj_LoginInfo.LoginStatus = 7;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }
                    //by pooja vachhani on 20/6/15


                #endregion

                    //String UserName = dt_usermst.Rows[0]["first_name"].ToString() + " " + dt_usermst.Rows[0]["middle_name"].ToString() + " " + dt_usermst.Rows[0]["last_name"].ToString();
                    String UserName = dt_usermst.Rows[0]["user_name"].ToString();
                    SessionID = SessionManager.GenerateNewSession(UserID, UserName, HostIP, HostName, ClientType, ModuleType);
                    if (SessionID == "-1")
                    {
                        obj_LoginInfo.ServerMessage = "Your license limit has exceeded.";
                        obj_LoginInfo.LoginStatus = 13;
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        return obj_LoginInfo;
                    }

                    if (dt_usermst != null)
                    {
                        if (dt_usermst.Rows.Count > 0)
                        {

                            dt_usermst.TableName = "dt_usermst";
                            obj_LoginInfo.ds_BusinessAreaInfo.Tables.Add(dt_usermst.Copy());
                            //obj_LoginInfo.ds_BusinessAreaInfo.Tables["dt_usermst"].Columns.Add("sa_id");
                            //obj_LoginInfo.ds_BusinessAreaInfo.Tables["dt_usermst"].Rows[0]["sa_id"] = "H01";
                        }
                    }

                    obj_LoginInfo.LoginStatus = 1;
                    obj_LoginInfo.ServerMessage = DateTime.UtcNow.ToString();
                    //SessionID for user
                    obj_LoginInfo.SessionId = SessionID;



                    dt_usermst.Rows[0]["last_login_date"] = DateTime.UtcNow;
                    DBCommand.Transaction = DBConnection.BeginTransaction();
                    DBCommand.Parameters.Clear();
                    DBCommand.CommandText = "";
                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = new BLGeneralUtil.UpdateTableInfo();
                    dt_usermst.TableName = "user_mst";
                    //Update User Master modified by sanket on 26-08-09
                    if (dt_usermst.Rows.Count > 0)
                    {
                        objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dt_usermst, BLGeneralUtil.UpdateWhereMode.KeyAndModifiedColumns);
                        if (objUpdateTableInfo.Status == false && objUpdateTableInfo.TotalRowsAffected != 1)
                        {
                            DBCommand.Transaction.Rollback();
                            obj_LoginInfo.ServerMessage = "An error occured while recording the login time.";
                            obj_LoginInfo.LoginStatus = 16;
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            return obj_LoginInfo;
                        }
                    }
                }
                else if (dt_usermst.Rows.Count > 1)
                {
                    obj_LoginInfo.ServerMessage = "More than one user registerd with same userId";
                    obj_LoginInfo.LoginStatus = 18;
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    return obj_LoginInfo;
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                return obj_LoginInfo;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //SessionInfo sess = new SessionInfo("Unknown", UserID, "Anonymous", HostIP, ClientType, ModuleType, DateTime.Now);
                //throw ExceptionManager.Help(ex, sess);
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                obj_LoginInfo.ServerMessage = ex.Message + " " + ex.StackTrace.Replace('\n', ' ').Replace('\r', ' ');
                obj_LoginInfo.LoginStatus = 17;
                return obj_LoginInfo;
            }
        }

        //pooja vachhani on 20/6/15
        private DataTable GetUserGroupRights(IDbDataAdapter adapter, String UserID, ref String msg)
        {
            try
            {
                 DataSet ds = new DataSet();

                 adapter.SelectCommand.CommandText = "SELECT * FROM user_group_rights WHERE user_id = '" + UserID + "'";
                 adapter.Fill(ds);

                 if (ds.Tables[0].Rows.Count >= 1)
                     return ds.Tables[0];
                 else
                     return null;

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }

        }

        //pooja vachhani on 2/7/15
        private DataTable GetUserTypeFromUserId(IDbDataAdapter adapter, String UserID,ref String msg)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT distinct user_mst.*,driver_mst.driver_photo FROM user_mst  ");
                sb.Append(" LEFT JOIN driver_mst on driver_mst.driver_id = user_mst.unique_id ");
                sb.Append(" WHERE (user_mst.user_id = @userid) and and user_mst.user_status_flag = 'A' and driver_mst.active_flag = 'Y' ");
                DataSet ds = new DataSet();

                adapter.SelectCommand.CommandText = sb.ToString();
                adapter.SelectCommand.Parameters.Clear();
                IDataParameter parma1 = DBObjectFactory.GetParameterObject();
                parma1.ParameterName = "@userid";
                parma1.DbType = DbType.String;
                parma1.Value = UserID;
                adapter.SelectCommand.Parameters.Add(parma1);

                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "user_mst");
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                    return ds.Tables[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }


        private DataTable GetUserInformation(IDbDataAdapter adapter, String UserID,ref String msg)
        {
            try
            {
                DataSet ds = new DataSet();

                adapter.SelectCommand.CommandText =

                //"SELECT user_mst.*, w_employee_master.full_name,  w_employee_master.first_name, w_employee_master.middle_name, w_employee_master.last_name, " +
                    //"w_employee_master.user_short_name FROM user_mst LEFT OUTER JOIN w_employee_master ON user_mst.user_id = w_employee_master.user_id " +
                    //"WHERE (user_mst.user_id = @userid)";

                "SELECT distinct user_mst.* FROM user_mst WHERE (user_mst.user_id = @userid )";

                adapter.SelectCommand.Parameters.Clear();
                IDataParameter parma1 = DBObjectFactory.GetParameterObject();
                parma1.ParameterName = "@userid";
                parma1.DbType = DbType.String;
                parma1.Value = UserID;
                adapter.SelectCommand.Parameters.Add(parma1);

                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "user_mst");
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                    return ds.Tables[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        private DataTable getUserCountry(IDbDataAdapter adapter, String country_code, ref String msg)
        {
            try
            {
                DataSet ds = new DataSet();

                adapter.SelectCommand.CommandText =

                //"SELECT user_mst.*, w_employee_master.full_name,  w_employee_master.first_name, w_employee_master.middle_name, w_employee_master.last_name, " +
                    //"w_employee_master.user_short_name FROM user_mst LEFT OUTER JOIN w_employee_master ON user_mst.user_id = w_employee_master.user_id " +
                    //"WHERE (user_mst.user_id = @userid)";

                "SELECT distinct country_master.* FROM country_master WHERE (country_master.country_code = @country_code) and (is_active = 'Y')";

                adapter.SelectCommand.Parameters.Clear();
                IDataParameter parma1 = DBObjectFactory.GetParameterObject();
                parma1.ParameterName = "@country_code";
                parma1.DbType = DbType.String;
                parma1.Value = country_code;
                adapter.SelectCommand.Parameters.Add(parma1);
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "country_master");
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                    return ds.Tables[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        private DataTable GetAppMenuList(IDbDataAdapter adapter, string UserID,string countryCODE, ClientType ClientType)
        {
            try
            {
                //String Org_Codes = "";
                //for (int i = 0; i < Org_ID.Count; i++)
                //{
                //    Org_Codes += "'" + Org_ID[i].ToString() + "'";
                //    if (i != Org_ID.Count - 1)
                //        Org_Codes += ",";
                //}
                String client_type = String.Empty;
                if (ClientType == ClientType.DeskTop)
                    client_type = "D";
                else if (ClientType == ClientType.Browser)
                    client_type = "B";
                DataSet ds = new DataSet();


//                adapter.SelectCommand.CommandText = @"SELECT menu_id, parent_menu_id, menu_display_name, menu_page_heading, menu_sort_id,menu_type,transaction_label,is_active,
//                            transaction_page_name,menu_color,layout_id,is_default, menu_link, href_id FROM menu_master WHERE (menu_id IN (SELECT DISTINCT menu_id
//                            FROM group_rights WHERE (group_id IN (SELECT group_id FROM user_group_rights WHERE (user_id =  @user_id) ) )) ) ";

                adapter.SelectCommand.CommandText = @"SELECT menu_id, parent_menu_id, menu_display_name, menu_page_heading, menu_sort_id,menu_type,transaction_label,is_active,
                            transaction_page_name,menu_color,layout_id,is_default, menu_link, href_id FROM menu_master WHERE (menu_id IN (SELECT DISTINCT menu_id
                            FROM group_rights WHERE (group_id IN (SELECT group_id FROM user_group_rights WHERE (user_id =  @user_id AND country_code = '" + countryCODE + "') ) )) ) ";


                //adapter.SelectCommand.CommandText = @"SELECT menu_id, pmenu_id, parameter, sort_id, transaction_type, menu_desc, window_name FROM menu_mst WHERE (menu_id IN (SELECT DISTINCT menu_id FROM group_rights WHERE (group_id IN (SELECT group_id FROM user_group_rights WHERE (user_id = @user_id) AND (country_code IN ()) AND (menu_mst.client_type = 'B') AND (start_date <= @today) AND (end_date > @today OR end_date IS NULL))) AND (start_date <= @today) AND (end_date > @today OR end_date IS NULL)))";
                adapter.SelectCommand.Parameters.Clear();
                IDataParameter parma1 = DBObjectFactory.GetParameterObject();
                parma1.ParameterName = "@today";
                parma1.DbType = DbType.DateTime;
                parma1.Value = System.DateTime.UtcNow;

                IDataParameter parma2 = DBObjectFactory.GetParameterObject();
                parma2.ParameterName = "@user_id";
                parma2.DbType = DbType.String;
                parma2.Value = UserID;

                adapter.SelectCommand.Parameters.Add(parma1);
                adapter.SelectCommand.Parameters.Add(parma2);
                adapter.Fill(ds);
                //ds.Tables[0].TableName = "dt_menu_info";
                //MessageBox.Show("fill",ds.Tables[0].Rows.Count.ToString());

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //MessageBox.Show("return");    
                    return ds.Tables[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + " " + ex.StackTrace);
                return null;
                //throw ExceptionManager.Help(ex, null);
            }
        }

    }
}
