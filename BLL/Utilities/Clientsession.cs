using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Clientsession
/// </summary>
public class Clientsession
{
	public Clientsession()
	{
		//
		// TODO: Add constructor logic here
		//
	}
 
    public static String Session_ID;
    public static String HR_ID;
    public static String employee_ID;
    public static String User_ID;
   
    public static String GetSessionId(String sesion_id)
    {
        Session_ID = sesion_id;
        return Session_ID;
    }
    public static String GetHR_ID(String sesion_id)
    {
        HR_ID = sesion_id;
        return HR_ID;
    }
    public static String Getemployee_ID(String sesion_id)
    {
        employee_ID = sesion_id;
        return employee_ID;
    }
    public static String Getuser_ID(String user_id)
    {
        User_ID = user_id;
        return User_ID;
    }
}