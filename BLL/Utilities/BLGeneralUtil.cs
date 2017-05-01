using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using BLL.Utilities;
using System.Collections;
using System.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Web.Configuration;
using trukkerUAE.Classes;
using System.ComponentModel;
using System.Web.Http;
using trukkerUAE.XSD;
using System.Reflection;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;

namespace BLL.Utilities
{
    public class BLGeneralUtil : ServerBase
    {
        
        #region FOR WEB USE
        private static StringBuilder sb = new StringBuilder();
        int KeyLength = 16;
        //Added by Ketan on 21/07/2011
        public struct UpdateTableInfo
        {
            public Boolean Status;
            public String ErrorMessage;
            public int TotalRowsAffected;
            public DataRow ErrorRow;
            public String ErrorSQLStatement;

        }

        public struct CreateTableInfo
        {
            public byte Exec_status;
            public string Message;
        }

        public enum UpdateWhereMode
        {
            KeyColumnsOnly,//PK columns only
            KeyAndModifiedColumns //PK and non-PK columns
        }

        public enum UpdateMethod
        {
            Deleted,
            Update, //Fires an Update Statement
            DeleteAndInsert //Fires a Delete statement and then Inserts a new row while updating
        }

        public enum PeriodType
        {
            Attendance,
            Leave,
            Loan,
            General
        }

        public static UpdateTableInfo UpdateTable(ref IDbCommand comm, DataTable dt, UpdateWhereMode updateWhereMode)
        {
            UpdateTableInfo structUpdateTableInfo;
            try
            {
                #region Initial Validations
                if (comm == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Command object is not initialized.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Connection == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Database connection object is not set.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Connection.State != ConnectionState.Open)
                {
                    structUpdateTableInfo.ErrorMessage = "Database connection is not open.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Transaction == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Transaction object is not initialized.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                #endregion //Initial Validations

                int TotalRowsSentForUpdate = 0;
                foreach (DataRow drow in dt.Rows)
                {
                    #region Logic for generating the query
                    String SQL = "";
                    Boolean IsChangeFound = false;
                    switch (drow.RowState)
                    {
                        case DataRowState.Added:
                            comm.Parameters.Clear();
                            IsChangeFound = true;
                            String ColumnNames = "";
                            String ParameterNames = "";
                            foreach (DataColumn dcol in dt.Columns)
                            {
                                if (dcol.Ordinal < dt.Columns.Count - 1)
                                {
                                    ColumnNames += dcol.ColumnName + ",";
                                    ParameterNames += "@" + dcol.ColumnName + ",";
                                }
                                else
                                {
                                    ColumnNames += dcol.ColumnName;
                                    ParameterNames += "@" + dcol.ColumnName;
                                }

                                IDataParameter param = DBObjectFactory.GetParameterObject();
                                param.ParameterName = "@" + dcol.ColumnName;
                                param.Value = drow[dcol, DataRowVersion.Current];
                                comm.Parameters.Add(param);
                            }
                            if (ColumnNames == "" || ParameterNames == "")
                                break;

                            SQL = "INSERT INTO " + dt.TableName +
                                    " ( " + ColumnNames + ") VALUES (" + ParameterNames + ")";

                            //MessageBox.Show(SQL, "INSERT");
                            break;
                        case DataRowState.Modified:
                            comm.Parameters.Clear();
                            IsChangeFound = true;
                            String SetColumnValues = "";
                            String WhereColumns = "";
                            if (updateWhereMode == UpdateWhereMode.KeyAndModifiedColumns)
                            {
                                #region KeyAndModifiedColumns
                                foreach (DataColumn dcol in dt.Columns)
                                {
                                    //Form the SET part for the columns whose Current Value != Original Value
                                    if (drow[dcol, DataRowVersion.Current].ToString() != drow[dcol, DataRowVersion.Original].ToString())
                                    {
                                        SetColumnValues += dcol.ColumnName + " = @" + dcol.ColumnName + ",";
                                        IDataParameter NewValueParam = DBObjectFactory.GetParameterObject();
                                        NewValueParam.ParameterName = "@" + dcol.ColumnName;
                                        NewValueParam.Value = drow[dcol, DataRowVersion.Current];
                                        comm.Parameters.Add(NewValueParam);

                                        if (drow[dcol, DataRowVersion.Original] != System.DBNull.Value)
                                        {
                                            WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                            IDataParameter OldValueParam = DBObjectFactory.GetParameterObject();
                                            OldValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                            OldValueParam.Value = drow[dcol, DataRowVersion.Original];
                                            comm.Parameters.Add(OldValueParam);
                                        }
                                        else
                                            WhereColumns += dcol.ColumnName + " IS NULL AND ";
                                    }
                                }
                                //Remove the last comma from SET string
                                if (SetColumnValues != "")
                                    SetColumnValues = SetColumnValues.Substring(0, SetColumnValues.Length - 1);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }

                                //Set the Primary Key Field
                                foreach (DataColumn dcol in dt.PrimaryKey)
                                {
                                    //Avoid including those PK fields in WHERE clause if they are modified as
                                    //this is already done in previous loop
                                    if (drow[dcol, DataRowVersion.Current].ToString() == drow[dcol, DataRowVersion.Original].ToString())
                                    {
                                        WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + " AND ";
                                        IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                        KeyValueParam.ParameterName = "@" + dcol.ColumnName;
                                        KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                        comm.Parameters.Add(KeyValueParam);
                                    }
                                }
                                //Remove the last AND from WHERE string
                                if (WhereColumns != "")
                                    WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }
                                #endregion //KeyAndModifiedColumns
                            }
                            else if (updateWhereMode == UpdateWhereMode.KeyColumnsOnly)
                            {
                                #region KeyColumnsOnly
                                foreach (DataColumn dcol in dt.Columns)
                                {
                                    //Form the SET part for the columns whose Current Value != Original Value
                                    SetColumnValues += dcol.ColumnName + " = @" + dcol.ColumnName + ",";
                                    IDataParameter NewValueParam = DBObjectFactory.GetParameterObject();
                                    NewValueParam.ParameterName = "@" + dcol.ColumnName;
                                    NewValueParam.Value = drow[dcol, DataRowVersion.Current];
                                    comm.Parameters.Add(NewValueParam);
                                }
                                //Remove the last comma from SET string
                                if (SetColumnValues != "")
                                    SetColumnValues = SetColumnValues.Substring(0, SetColumnValues.Length - 1);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }

                                //Set the Primary Key Field
                                foreach (DataColumn dcol in dt.PrimaryKey)
                                {
                                    WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                    IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                    KeyValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                    KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                    comm.Parameters.Add(KeyValueParam);
                                }
                                //Remove the last AND from WHERE string
                                if (WhereColumns != "")
                                    WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }
                                #endregion //KeyColumnsOnly
                            }
                            SQL = "UPDATE " + dt.TableName +
                                    " SET " + SetColumnValues + " WHERE " + WhereColumns;

                            //MessageBox.Show(SQL, "UPDATE");
                            break;
                        case DataRowState.Deleted:
                            comm.Parameters.Clear();
                            IsChangeFound = true;
                            String DeleteWhereColumns = "";

                            //Set the Primary Key Field
                            foreach (DataColumn dcol in dt.PrimaryKey)
                            {
                                DeleteWhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + " AND ";
                                IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                KeyValueParam.ParameterName = "@" + dcol.ColumnName;
                                KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                comm.Parameters.Add(KeyValueParam);
                            }
                            if (DeleteWhereColumns != "")
                                //Remove the last AND from WHERE string
                                DeleteWhereColumns = DeleteWhereColumns.Substring(0, DeleteWhereColumns.Length - 4);
                            else
                            {
                                IsChangeFound = false;
                                break;
                            }

                            SQL = "DELETE FROM " + dt.TableName + " WHERE " + DeleteWhereColumns;

                            //MessageBox.Show(SQL, "DELETE");
                            break;
                    }
                    #endregion

                    #region Fire the query
                    if (IsChangeFound)
                    {
                        comm.CommandType = CommandType.Text;
                        comm.CommandText = SQL;
                        if (comm.ExecuteNonQuery() != 1)
                        {
                            structUpdateTableInfo.ErrorMessage = "Database updation failed.";
                            structUpdateTableInfo.ErrorSQLStatement = SQL;
                            structUpdateTableInfo.ErrorRow = drow;
                            structUpdateTableInfo.Status = false;
                            structUpdateTableInfo.TotalRowsAffected = TotalRowsSentForUpdate;
                            return structUpdateTableInfo;
                        }
                        TotalRowsSentForUpdate++;
                        IsChangeFound = false;
                    }
                    #endregion
                }

                structUpdateTableInfo.ErrorMessage = "";
                structUpdateTableInfo.ErrorSQLStatement = "";
                structUpdateTableInfo.ErrorRow = null;
                structUpdateTableInfo.Status = true;
                structUpdateTableInfo.TotalRowsAffected = TotalRowsSentForUpdate;

                return structUpdateTableInfo;
            }
            catch (Exception e)
            {
                comm.Transaction.Rollback();
                throw (e);
            }
        }

        public static UpdateTableInfo UpdateTable(ref IDbCommand comm, DataTable dt, UpdateWhereMode updateWhereMode, UpdateMethod updateMethod)
        {
            UpdateTableInfo structUpdateTableInfo;
            try
            {
                #region Initial Validations
                if (comm == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Command object is not initialized.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Connection == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Database connection object is not set.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Connection.State != ConnectionState.Open)
                {
                    structUpdateTableInfo.ErrorMessage = "Database connection is not open.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (comm.Transaction == null)
                {
                    structUpdateTableInfo.ErrorMessage = "Transaction object is not initialized.Operation cancelled.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                if (dt == null)
                {
                    structUpdateTableInfo.ErrorMessage = "No Table to Update.";
                    structUpdateTableInfo.ErrorSQLStatement = "";
                    structUpdateTableInfo.ErrorRow = null;
                    structUpdateTableInfo.Status = false;
                    structUpdateTableInfo.TotalRowsAffected = 0;
                    return structUpdateTableInfo;
                }
                #endregion //Initial Validations

                int TotalRowsSentForUpdate = 0;


                foreach (DataRow drow in dt.Rows)
                {
                    String SQL = "";
                    String SQLDelete = "";
                    Boolean IsChangeFound = false;
                    String SetColumnValues = "";
                    String WhereColumns = "";

                    switch (drow.RowState)
                    {
                        #region Query Generation for Insert
                        case DataRowState.Added:
                            comm.Parameters.Clear();
                            IsChangeFound = true;
                            String ColumnNames = "";
                            String ParameterNames = "";


                            foreach (DataColumn dcol in dt.PrimaryKey)
                            {

                                WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                KeyValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                KeyValueParam.Value = drow[dcol];//, DataRowVersion.Original];
                                comm.Parameters.Add(KeyValueParam);

                            }
                            //Remove the last AND from WHERE string
                            if (WhereColumns != "")
                                WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                            else
                            {
                                IsChangeFound = false;
                                break;
                            }

                            SQLDelete = "DELETE " + dt.TableName +
                                        " WHERE " + WhereColumns;

                            foreach (DataColumn dcol in dt.Columns)
                            {
                                if (dcol.Ordinal < dt.Columns.Count - 1)
                                {
                                    ColumnNames += dcol.ColumnName + ",";
                                    ParameterNames += "@" + dcol.ColumnName + ",";
                                }
                                else
                                {
                                    ColumnNames += dcol.ColumnName;
                                    ParameterNames += "@" + dcol.ColumnName;
                                }

                                IDataParameter param = DBObjectFactory.GetParameterObject();
                                param.ParameterName = "@" + dcol.ColumnName;
                                param.Value = drow[dcol, DataRowVersion.Current];
                                comm.Parameters.Add(param);
                            }
                            if (ColumnNames == "" || ParameterNames == "")
                                break;

                            SQL = "INSERT INTO " + dt.TableName +
                                    " ( " + ColumnNames + ") VALUES (" + ParameterNames + ")";

                            //MessageBox.Show(SQL, "INSERT");
                            break;
                        #endregion

                        #region Query Generation for Delete
                        case DataRowState.Deleted:
                            comm.Parameters.Clear();
                            IsChangeFound = true;
                            String DeleteWhereColumns = "";

                            //Set the Primary Key Field
                            foreach (DataColumn dcol in dt.PrimaryKey)
                            {
                                DeleteWhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + " AND ";
                                IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                KeyValueParam.ParameterName = "@" + dcol.ColumnName;
                                KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                comm.Parameters.Add(KeyValueParam);
                            }
                            if (DeleteWhereColumns != "")
                                //Remove the last AND from WHERE string
                                DeleteWhereColumns = DeleteWhereColumns.Substring(0, DeleteWhereColumns.Length - 4);
                            else
                            {
                                IsChangeFound = false;
                                break;
                            }

                            SQL = "DELETE FROM " + dt.TableName + " WHERE " + DeleteWhereColumns;

                            //MessageBox.Show(SQL, "DELETE");
                            break;

                        #endregion

                        #region Query Generation for Update
                        case DataRowState.Modified:
                            comm.Parameters.Clear();
                            IsChangeFound = true;


                            if (updateMethod == UpdateMethod.Update)
                            {
                                ////When the Update method is Update

                                #region When Update Method is Update

                                if (updateWhereMode == UpdateWhereMode.KeyAndModifiedColumns)
                                {
                                    #region KeyAndModifiedColumns
                                    foreach (DataColumn dcol in dt.Columns)
                                    {
                                        //Form the SET part for the columns whose Current Value != Original Value
                                        if (drow[dcol, DataRowVersion.Current].ToString() != drow[dcol, DataRowVersion.Original].ToString())
                                        {
                                            SetColumnValues += dcol.ColumnName + " = @" + dcol.ColumnName + ",";
                                            IDataParameter NewValueParam = DBObjectFactory.GetParameterObject();
                                            NewValueParam.ParameterName = "@" + dcol.ColumnName;
                                            NewValueParam.Value = drow[dcol, DataRowVersion.Current];
                                            comm.Parameters.Add(NewValueParam);

                                            if (drow[dcol, DataRowVersion.Original] != System.DBNull.Value)
                                            {
                                                WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                                IDataParameter OldValueParam = DBObjectFactory.GetParameterObject();
                                                OldValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                                OldValueParam.Value = drow[dcol, DataRowVersion.Original];
                                                comm.Parameters.Add(OldValueParam);
                                            }
                                            else
                                                WhereColumns += dcol.ColumnName + " IS NULL AND ";
                                        }
                                    }
                                    //Remove the last comma from SET string
                                    if (SetColumnValues != "")
                                        SetColumnValues = SetColumnValues.Substring(0, SetColumnValues.Length - 1);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }

                                    //Set the Primary Key Field
                                    foreach (DataColumn dcol in dt.PrimaryKey)
                                    {
                                        //Avoid including those PK fields in WHERE clause if they are modified as
                                        //this is already done in previous loop
                                        if (drow[dcol, DataRowVersion.Current].ToString() == drow[dcol, DataRowVersion.Original].ToString())
                                        {
                                            WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + " AND ";
                                            IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                            KeyValueParam.ParameterName = "@" + dcol.ColumnName;
                                            KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                            comm.Parameters.Add(KeyValueParam);
                                        }
                                    }
                                    //Remove the last AND from WHERE string
                                    if (WhereColumns != "")
                                        WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }
                                    #endregion //KeyAndModifiedColumns
                                }
                                else if (updateWhereMode == UpdateWhereMode.KeyColumnsOnly)
                                {
                                    #region KeyColumnsOnly
                                    foreach (DataColumn dcol in dt.Columns)
                                    {
                                        //Form the SET part for the columns whose Current Value != Original Value
                                        SetColumnValues += dcol.ColumnName + " = @" + dcol.ColumnName + ",";
                                        IDataParameter NewValueParam = DBObjectFactory.GetParameterObject();
                                        NewValueParam.ParameterName = "@" + dcol.ColumnName;
                                        NewValueParam.Value = drow[dcol, DataRowVersion.Current];
                                        comm.Parameters.Add(NewValueParam);
                                    }
                                    //Remove the last comma from SET string
                                    if (SetColumnValues != "")
                                        SetColumnValues = SetColumnValues.Substring(0, SetColumnValues.Length - 1);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }

                                    //Set the Primary Key Field
                                    foreach (DataColumn dcol in dt.PrimaryKey)
                                    {
                                        WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                        IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                        KeyValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                        KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                        comm.Parameters.Add(KeyValueParam);
                                    }
                                    //Remove the last AND from WHERE string
                                    if (WhereColumns != "")
                                        WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }
                                    #endregion //KeyColumnsOnly
                                }
                                SQL = "UPDATE " + dt.TableName +
                                        " SET " + SetColumnValues + " WHERE " + WhereColumns;

                                #endregion
                            }
                            else if (updateMethod == UpdateMethod.DeleteAndInsert)
                            {
                                ////When the Update method is Delete and Insert

                                #region When the Update method is Delete and Insert
                                if (updateWhereMode == UpdateWhereMode.KeyAndModifiedColumns)
                                {
                                    #region KeyAndModifiedColumns
                                    foreach (DataColumn dcol in dt.Columns)
                                    {
                                        if (drow[dcol, DataRowVersion.Current].ToString() != drow[dcol, DataRowVersion.Original].ToString())
                                        {
                                            if (drow[dcol, DataRowVersion.Original] != System.DBNull.Value)
                                            {
                                                WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                                IDataParameter OldValueParam = DBObjectFactory.GetParameterObject();
                                                OldValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                                OldValueParam.Value = drow[dcol, DataRowVersion.Original];
                                                comm.Parameters.Add(OldValueParam);
                                            }
                                            else
                                                WhereColumns += dcol.ColumnName + " IS NULL AND ";

                                        }
                                    }
                                    //Set the Primary Key Field
                                    foreach (DataColumn dcol in dt.PrimaryKey)
                                    {
                                        //Avoid including those PK fields in WHERE clause if they are modified as
                                        //this is already done in previous loop
                                        if (drow[dcol, DataRowVersion.Current].ToString() == drow[dcol, DataRowVersion.Original].ToString())
                                        {
                                            WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + " AND ";
                                            IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                            KeyValueParam.ParameterName = "@" + dcol.ColumnName;
                                            KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                            comm.Parameters.Add(KeyValueParam);
                                        }
                                    }
                                    //Remove the last AND from WHERE string
                                    if (WhereColumns != "")
                                        WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }
                                    #endregion
                                }
                                else if (updateWhereMode == UpdateWhereMode.KeyColumnsOnly)
                                {
                                    #region KeyColumnsOnly
                                    //Set the Primary Key Field
                                    foreach (DataColumn dcol in dt.PrimaryKey)
                                    {

                                        WhereColumns += dcol.ColumnName + " = @" + dcol.ColumnName + "1 AND ";
                                        IDataParameter KeyValueParam = DBObjectFactory.GetParameterObject();
                                        KeyValueParam.ParameterName = "@" + dcol.ColumnName + "1";
                                        KeyValueParam.Value = drow[dcol, DataRowVersion.Original];
                                        comm.Parameters.Add(KeyValueParam);

                                    }
                                    //Remove the last AND from WHERE string
                                    if (WhereColumns != "")
                                        WhereColumns = WhereColumns.Substring(0, WhereColumns.Length - 4);
                                    else
                                    {
                                        IsChangeFound = false;
                                        break;
                                    }
                                    #endregion
                                }

                                SQLDelete = "DELETE " + dt.TableName +
                                        " WHERE " + WhereColumns;


                                //Build the Insert Query
                                String Columns = "";
                                foreach (DataColumn dcol in dt.Columns)
                                {
                                    //Form the SET part for the columns whose Current Value != Original Value
                                    Columns += dcol.ColumnName + ",";
                                    SetColumnValues += "@" + dcol.ColumnName + ",";
                                    IDataParameter NewValueParam = DBObjectFactory.GetParameterObject();
                                    NewValueParam.ParameterName = "@" + dcol.ColumnName;
                                    NewValueParam.Value = drow[dcol, DataRowVersion.Current];
                                    comm.Parameters.Add(NewValueParam);
                                }
                                ///To remove the last comma in the Set Statement
                                if (SetColumnValues != "")
                                    SetColumnValues = SetColumnValues.Substring(0, SetColumnValues.Length - 1);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }
                                if (Columns != "")
                                    Columns = Columns.Substring(0, Columns.Length - 1);
                                else
                                {
                                    IsChangeFound = false;
                                    break;
                                }

                                SQL = "INSERT INTO " + dt.TableName +
                                      " ( " + Columns + " ) VALUES ( " + SetColumnValues + " ) ";

                                #endregion
                            }
                            break;

                        #endregion
                    }

                    #region Fire the query
                    if (IsChangeFound)
                    {
                        if (updateMethod == UpdateMethod.DeleteAndInsert)
                        {
                            if (SQLDelete.Trim() != "")
                            {
                                comm.CommandType = CommandType.Text;
                                comm.CommandText = SQLDelete;
                                int SqlTest = comm.ExecuteNonQuery();
                                //if (comm.ExecuteNonQuery() != 1)
                                //System.Windows.Forms.MessageBox.Show(muck.ToString());
                                if (SqlTest != 1 && SqlTest != 0)
                                {

                                    structUpdateTableInfo.ErrorMessage = "Database updation failed.";
                                    structUpdateTableInfo.ErrorSQLStatement = SQL;
                                    structUpdateTableInfo.ErrorRow = drow;
                                    structUpdateTableInfo.Status = false;
                                    structUpdateTableInfo.TotalRowsAffected = TotalRowsSentForUpdate;
                                    return structUpdateTableInfo;
                                }
                            }

                        }

                        comm.CommandType = CommandType.Text;
                        comm.CommandText = SQL;
                        if (comm.ExecuteNonQuery() != 1)
                        {
                            structUpdateTableInfo.ErrorMessage = "Database updation failed.";
                            structUpdateTableInfo.ErrorSQLStatement = SQL;
                            structUpdateTableInfo.ErrorRow = drow;
                            structUpdateTableInfo.Status = false;
                            structUpdateTableInfo.TotalRowsAffected = TotalRowsSentForUpdate;
                            return structUpdateTableInfo;
                        }


                        TotalRowsSentForUpdate++;
                        IsChangeFound = false;
                    }

                    #endregion
                }

                structUpdateTableInfo.ErrorMessage = "";
                structUpdateTableInfo.ErrorSQLStatement = "";
                structUpdateTableInfo.ErrorRow = null;
                structUpdateTableInfo.Status = true;
                structUpdateTableInfo.TotalRowsAffected = TotalRowsSentForUpdate;


                return structUpdateTableInfo;
            }
            catch (Exception e)
            {
                comm.Transaction.Rollback();
                throw (e);
            }


        }

        public static IDataParameter MakeParameter(String ParameterName, DbType ParameterType, Object ParameterValue)
        {
            IDataParameter parameter = DBObjectFactory.GetParameterObject();
            parameter.ParameterName = ParameterName;
            parameter.DbType = ParameterType;
            parameter.Value = ParameterValue;
            return parameter;
        }

        public static ArrayList GetDistinctColumnValues(DataTable dt, string ColumnName)
        {
            ArrayList UniqueValues = new ArrayList();
            string sortorder = ColumnName + " DESC";
            dt.DefaultView.Sort = sortorder;
            DataTable sorted_table = dt.DefaultView.ToTable();
            UniqueValues.Add(sorted_table.Rows[0][ColumnName].ToString());
            for (int i = 0; i < sorted_table.Rows.Count; i++)
            {
                if (sorted_table.Rows[i][ColumnName].ToString() == UniqueValues[UniqueValues.Count - 1].ToString())
                    continue;
                else
                    UniqueValues.Add(sorted_table.Rows[i][ColumnName].ToString());
            }
            return UniqueValues;
        }

        //public static Boolean IsNumeric(Object obj)
        //{
        //    return Information.IsNumeric(obj);
        //}

        public static string return_ajax_string(string status, string message)
        {
            //return_ajax_string("false", "You have not completed the feedback, please fill the feedback to view your grades");
            return "{\"status\":\"" + status + "\",\"message\":\"" + message + "\"}";
        }

        public static string return_ajax_data(string status, string json_data)
        {
            return "{\"status\":\"" + status + "\",\"message\":" + json_data + "}";
        }

        public static string return_ajax_statusdata(string status, string message, string json_data)
        {
            //return_ajax_string("false", "You have not completed the feedback, please fill the feedback to view your grades");
            return "{\"status\":\"" + status + "\",\"message\":\"" + message + "\",\"data\":" + json_data + "}";
        }

        public static string GetTokenName(JObject jobj, string tkname)
        {
            string tknamefound = "";

            JObject jr = JObject.Parse(jobj.ToString());

            for (int i = 0; i < jobj.Count; i++)
            {
                if (true)
                {

                }
                //if (jobj.ChildrenTokens[i].ToString() == tkname)
                //    tknamefound = jobj.ChildrenTokens[i].ToString();
            }

            return tknamefound;
        }

        public static string GetCodeFromName(String WhereColumn, String SelectColumn, String tablename)
        {
            sb.Clear();
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            sb.Append("Select " + SelectColumn + " from " + tablename + " Where " + WhereColumn);

            SqlCommand cmd = new SqlCommand(sb.ToString(), con);
            con.Open();

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            //return dtPostLoadOrders;
            else
                return "0";
        }

        #endregion

        public static DateTime ConvertToDateTime(string value, string format)
        {
            value = value.Substring(0, 10);

            DateTime date = new DateTime();
            if (format == @"dd/mm/yyyy")
            {                                   //year                                 //month                                 //day   
                date = new DateTime(Convert.ToInt32(value.Substring(6)), Convert.ToInt32(value.Substring(3, 2)), Convert.ToInt32(value.Substring(0, 2)));
                return date;
            }
            else if (format == @"mm/dd/yyyy")
            {                                   //year                                 //month                                 //day   
                date = new DateTime(Convert.ToInt32(value.Substring(6)), Convert.ToInt32(value.Substring(0, 2)), Convert.ToInt32(value.Substring(3, 2)));
                return date;
            }

            return date;
        }
        public static DataTable CheckDateTime(DataTable dt_temp)
        {
            string[] format = new string[] { "dd-MM-yyyy", "dd/mm/yyyy" };
            DateTime date = new DateTime();
            for (int i = 0; i < dt_temp.Columns.Count; i++)
            {
                for (int j = 0; j < dt_temp.Rows.Count; j++)
                {
                    // if (DateTime.TryParse(dt_temp.Rows[j][i].ToString(), out date))
                    if (DateTime.TryParseExact(dt_temp.Rows[j][i].ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        //dt_temp.Rows[j][i] = ConvertToDateTime(Convert.ToString(Convert.ToDateTime(dt_temp.Rows[j][i].ToString()).Date).Substring(0,10), "dd/mm/yyyy");
                        dt_temp.Rows[j][i] = ConvertToDateTime(dt_temp.Rows[j][i].ToString(), "dd/mm/yyyy");
                    }
                }
            }
            return dt_temp;
        }
        public static decimal GetRoundedValue(string rounding_method, decimal value)
        {
            if (rounding_method == Constant.ROUNDING_NEAREST_RUPEE)
            {
                value += 0.50M;
                value = Math.Floor(value);
            }
            else if (rounding_method == Constant.ROUNDING_UPTO_2_DECIMAL_PTS)
            {
                //value += 0.50M;
                value = Math.Round(value, 2);
            }
            else if (rounding_method == Constant.ROUNDING_NEAREST_FIVE_RUPEES)
            {
                value = Math.Round(value / 5) * 5;
            }
            else if (rounding_method == Constant.ROUNDING_NEAREST_TEN_RUPEES)
            {
                value = Math.Round(value / 10) * 10;
            }
            else if (rounding_method == Constant.ROUNDING_NEAREST_HUNDRED_RUPEES)
            {
                value = Math.Round(value / 100) * 100;
            }
            else if (rounding_method == Constant.ROUNDING_NONE)
            {
                return value;
            }
            return value;
        }
        public bool SetAndModifyStatusinAnyTableNew(string tablename, string status, string shipperid, string inquiryno, string ownerid, string truckid, string driverid, ref IDbCommand DBCommand)
        {
            DBCommand.Parameters.Clear();
            string where = "";
            string query = " UPDATE " + tablename +
                     " SET status =   '" + status + "' " + " WHERE ";

            if (shipperid != string.Empty && shipperid != "" && shipperid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@shipperid", DbType.String, shipperid));
                where = " shipper_id= @shipperid ";
            }
            if (inquiryno != string.Empty && inquiryno != "" && inquiryno.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inquiryno", DbType.String, inquiryno));
                if (where != string.Empty)
                    where = where + "AND  load_inquiry_no= @inquiryno ";
                else
                    where = " load_inquiry_no = @inquiryno ";
            }
            if (ownerid != string.Empty && ownerid != "" && ownerid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@ownerid", DbType.String, ownerid));
                if (where != string.Empty)
                    where = where + "AND  owner_id= @ownerid ";
                else
                    where = " owner_id = @ownerid ";
            }
            if (truckid != string.Empty && truckid != "" && truckid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truckid", DbType.String, truckid));
                if (where != string.Empty)
                    where = where + "AND  truck_id= @truckid ";
                else
                    where = " truck_id = @truckid ";
            }
            if (driverid != string.Empty && driverid != "" && driverid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@driverid", DbType.String, driverid));
                if (where != string.Empty)
                    where = where + "AND  driver_id= @driverid ";
                else
                    where = " driver_id = @driverid ";
            }
            query = query + where;
            DBCommand.CommandText = query;
            int row_count = 0;
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            try
            {
                row_count = DBCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
            if (row_count <= 0)
            {
                ServerLog.Log("Status not Updated " + tablename + Environment.NewLine + query);
                return false;
            }
            else
                return true;

        }
        public DataTable GetTableDataCommonMethod(string tablename, string status, string shipperid, string inquiryno, string ownerid, string truckid, string driverid, ref IDbCommand DBCommand)
        {

            DBCommand.Parameters.Clear();
            string where = "";
            string query = "select * from " + tablename + " WHERE ";


            if (shipperid != string.Empty && shipperid != "" && shipperid.TrimEnd() != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@shipperid", DbType.String, shipperid.TrimEnd()));
                where = " shipper_id= @shipperid ";
            }
            if (inquiryno != string.Empty && inquiryno != "" && inquiryno.TrimEnd() != null)
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@inquiryno", DbType.String, inquiryno.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  load_inquiry_no= @inquiryno ";
                else
                    where = " load_inquiry_no = @inquiryno ";
            }


            if (ownerid != string.Empty && ownerid != "" && ownerid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@ownerid", DbType.String, ownerid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  owner_id= @ownerid ";
                else
                    where = " owner_id = @ownerid ";
            }

            if (truckid != string.Empty && truckid != "" && truckid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truckid", DbType.String, truckid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  truck_id= @truckid ";
                else
                    where = " truck_id = @truckid ";
            }
            if (driverid != string.Empty && driverid != "" && driverid.TrimEnd() != "")
            {
                DBCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@driverid", DbType.String, driverid.TrimEnd()));
                if (where != string.Empty)
                    where = where + "AND  driver_id= @driverid ";
                else
                    where = " driver_id = @driverid ";
            }
            query = query + where;
            DBDataAdpterObject.SelectCommand.CommandText = query;
            DataSet ds = new DataSet();
            if (DBConnection.State == ConnectionState.Closed) DBConnection.Open();
            DBDataAdpterObject.Fill(ds);
            if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return ds.Tables[0];

        }
        public string DirectIntoDatasetTableFromJsonStringArray([FromBody]JObject tc)
        {
            DS_Owner_Mst objownerDriver = new DS_Owner_Mst();

            ////////////// ACTUAL CODE ///////////////
            DS_Owner_Mst.driver_mstDataTable dt1 = tc["driver_mst"].ToObject<DS_Owner_Mst.driver_mstDataTable>();
            DS_Owner_Mst.driver_contact_detailDataTable dt2 = tc["driver_contact_detail"].ToObject<DS_Owner_Mst.driver_contact_detailDataTable>();

            //////////////////////////////////////////
            return null;

        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static string ToSHA256(string value)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            return returnValue.ToString();
        }

        public DataTable GetParameter(string paramcode, string paramvalue, ref string msg)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                String SqlSelect = "";
                SqlSelect = "select * from param_mst Where param_code = @paracode and param_key = @parvalue ";

                DBDataAdpterObject.SelectCommand.CommandText = SqlSelect;
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("paracode", DbType.String, paramcode));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("parvalue", DbType.String, paramvalue));
                DataSet ds = new DataSet();
                try
                {
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables[0].Rows.Count <= 0)
                    {
                        msg = "Data Not Found";
                        return null;
                    }
                    else
                        return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return null;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "trkr2016";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "trkr2016";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
