using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using trukkerUAE.Classes;
//using System.Data.OracleClient;
//using MySql.Data.MySqlClient;

namespace BLL.Utilities
{
    public class DBObjectFactory
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        private static string Provider = ConfigurationManager.ConnectionStrings[0].ProviderName;
        //public static string Provider = "System.Data.OracleClient"; //"ORAOLEDB.ORACLE";
        //private static string Provider = "MySql.Data.MySqlClient";

        public static IDbConnection GetConnectionObject()
        {
            switch (Provider)
            {
                case "System.Data.SqlClient":
                    return new SqlConnection();
                //case "System.Data.OracleClient":
                //    return new OracleConnection();
                //case "MySql.Data.MySqlClient":
                //    return new MySqlConnection();
            }
            return null;
        }

        public static IDbCommand GetCommandObject()
        {
            switch (Provider)
            {
                case "System.Data.SqlClient":
                    return new SqlCommand();
                //case "System.Data.OracleClient":
                //    return new OracleCommand();
                //case "MySql.Data.MySqlClient":
                //    return new MySqlCommand();
            }
            return null;
        }

        public static IDbDataAdapter GetDataAdapterObject(IDbCommand DBCommand)
        {
            switch (Provider)
            {
                case "System.Data.SqlClient":
                    return new SqlDataAdapter((SqlCommand)DBCommand);
                //case "System.Data.OracleClient":
                //    return new OracleDataAdapter((OracleCommand)DBCommand);
                //case "MySql.Data.MySqlClient":
                //    return new MySqlDataAdapter((MySqlCommand)DBCommand);
            }
            return null;
        }

        public static IDataParameter GetParameterObject()
        {
            switch (Provider)
            {
                case "System.Data.SqlClient":
                    return new SqlParameter();
                //case "System.Data.OracleClient":
                //    return new OracleParameter();
                //case "MySql.Data.MySqlClient":
                //    return new MySqlParameter();
            }
            return null;
        }

        public static IDataParameter MakeParameter(String ParameterName, DbType ParameterType, Object ParameterValue)
        {
            IDataParameter parameter = DBObjectFactory.GetParameterObject();
            parameter.ParameterName = ParameterName;
            parameter.DbType = ParameterType;
            parameter.Value = ParameterValue;
            return parameter;
        }

        public static String SetQuery(String Query)
        {
            if (DBObjectFactory.Provider == Constant.Provider_Oracle)
            {
                Query = Query.Replace("ISNULL", "NVL");
                Query = Query.Replace('@', ':');
                return Query;
            }
            else
                return Query;
        }
    }
}
