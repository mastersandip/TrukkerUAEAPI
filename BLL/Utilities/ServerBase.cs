using System;
using System.Data;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Web;
using BLL.Utilities;
using BLL.Master;
 

namespace BLL.Utilities 
{
    public class ServerBase : ApiController
    {
        protected IDbConnection DBConnection;
        #region FOR WEB USE
        //added by ketan on 21/07/2011
        protected IDbTransaction DBTransactionObject = null;
        protected IDbCommand DBCommand;
        protected IDbDataAdapter DBDataAdpterObject;
        protected String Trackurl;
        protected String CbmUrl;
        protected String PaymentUrl;
        protected String AddonPaymentUrl;

        #endregion

        public ServerBase()
        {
            AddonPaymentUrl = ConfigurationManager.AppSettings["AddonPayment"].ToString();
            PaymentUrl = ConfigurationManager.AppSettings["Payment"].ToString();
            Trackurl = ConfigurationManager.AppSettings["Track"].ToString();
            CbmUrl = ConfigurationManager.AppSettings["cbm"].ToString();
            DBConnection = DBObjectFactory.GetConnectionObject();
            DBConnection.ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
            DBCommand = DBObjectFactory.GetCommandObject();
            DBCommand.Connection = DBConnection;
            DBDataAdpterObject = DBObjectFactory.GetDataAdapterObject(DBCommand);
            DBDataAdpterObject.SelectCommand.CommandTimeout = 60;
         
        }
        public void Dispose()
        {
            if (DBConnection != null)
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

       
    }
}
