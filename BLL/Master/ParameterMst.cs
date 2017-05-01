using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BLL.Utilities;

namespace trukkerUAE.BLL.Master
{
    public class ParameterMst : ServerBase
    {
        public ParameterMst()
        {
        }

        public DataTable GetParameter(String Parameter, String Code, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append("SELECT * FROM ParameterMst ");
                SQLSelect.Append("WHERE  1=1 ");
                if (Parameter != null && Parameter.Trim() != String.Empty)
                {
                    SQLSelect.Append("AND (Parameter=@Parameter) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@Parameter", DbType.String, Parameter));
                }
                if (Code != null && Code.Trim() != String.Empty)
                {
                    SQLSelect.Append("AND (Code=@Code) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@Code", DbType.String, Code));
                }
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "ParameterMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "ParameterMst retrieved successfully.";
                    return ds.Tables[0];
                }
                else
                {
                    Message = "ParameterMst not found.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }        
    }
}