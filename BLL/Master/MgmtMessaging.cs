using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Utilities;
using System.Configuration;
using System.Data;
using trukkerUAE.XSD;
using trukkerUAE.Classes;

namespace BLL.Master
{
    public class MgmtMessaging : ServerBase
    {
        public MgmtMessaging()
        {
        }

        public Byte SaveMessageQueue(DataSet dSUpload, ref String Message)
        {
            try
            {
                if (dSUpload == null || dSUpload.Tables.Count <= 0 || dSUpload.Tables[0].Rows.Count <= 0)
                {
                    Message = "No data available to Save. Operation Cancelled.";
                    return Constant.Status_Fail;
                }
                DS_MessageQueue dS_MessageQueue = (DS_MessageQueue)dSUpload;

                try
                {
                    dS_MessageQueue.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return Constant.Status_Fail;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return Constant.Status_Fail;
                }

                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DS_MessageQueue dS_MessageQueueServer = new DS_MessageQueue();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
                int MessageId, index;
                foreach (DS_MessageQueue.MessageQueueRow rows in dS_MessageQueue.MessageQueue.Rows)
                {
                    dS_MessageQueueServer.MessageQueue.Clear();
                    dS_MessageQueueServer.MessageQueue.ImportRow(rows);
                    //Insert in MessageQueue
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_MessageQueueServer.MessageQueue, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageQueue.Rows.Count)
                    {
                        DBCommand.Transaction.Rollback();
                        if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                        Message = objUpdateTableInfo.ErrorMessage;
                        return Constant.Status_Fail;
                    }

                    if (dS_MessageQueue.MessageAttachment.Rows.Count > 0)
                    {
                        dS_MessageQueueServer.MessageAttachment.Clear();

                        //Get MessageId.
                        DBCommand.Parameters.Clear();
                        DBCommand.CommandText = "SELECT @@IDENTITY AS 'Identity';";
                        Object objMessageId = DBCommand.ExecuteScalar();
                        if (objMessageId != null && objMessageId.ToString().Trim() != String.Empty)
                        {
                            //Set MessageId to Attachment.
                            MessageId = Convert.ToInt32(objMessageId);
                            foreach (DS_MessageQueue.MessageAttachmentRow attachmentRow in dS_MessageQueue.MessageAttachment.Rows)
                            {
                                dS_MessageQueueServer.MessageAttachment.ImportRow(attachmentRow);
                                index = dS_MessageQueueServer.MessageAttachment.Rows.Count - 1;
                                dS_MessageQueueServer.MessageAttachment[index].MessageId = MessageId;
                            }
                            //Insert in MessageAttachment
                            objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_MessageQueueServer.MessageAttachment, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                            if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageAttachment.Rows.Count)
                            {
                                DBCommand.Transaction.Rollback();
                                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                                Message = objUpdateTableInfo.ErrorMessage;
                                return Constant.Status_Fail;
                            }
                        }
                        else
                        {
                            DBCommand.Transaction.Rollback();
                            if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                            Message = "Fail to save Message Queue data.";
                            return Constant.Status_Success;
                        }
                    }
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "Message Queue data saved successfully.";
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        //public Byte SaveMessageQueue(ref IDbCommand command, String UniqueId, String Sender, String Subject, String MessageBody, String MessageType, ref String Message)
        //{
        //    try
        //    {
        //        if (UniqueId == null || UniqueId.Trim() == String.Empty)
        //        {
        //            Message = "Please supplied UniqueId.";
        //            return Constant.Status_Fail;
        //        }
        //        else if (Sender == null || Sender.Trim() == String.Empty)
        //        {
        //            Message = "Please supplied Sender.";
        //            return Constant.Status_Fail;
        //        }
        //        else if (Subject == null || Subject.Trim() == String.Empty)
        //        {
        //            Message = "Please supplied Subject.";
        //            return Constant.Status_Fail;
        //        }
        //        else if (MessageBody == null || MessageBody.Trim() == String.Empty)
        //        {
        //            Message = "Please supplied MessageBody.";
        //            return Constant.Status_Fail;
        //        }
        //        else if (MessageType == null || MessageType.Trim() == String.Empty)
        //        {
        //            Message = "Please supplied MessageType.";
        //            return Constant.Status_Fail;
        //        }

        //        IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
        //        DS_MessageQueue dS_MessageQueue = new DS_MessageQueue();
        //        dS_MessageQueue.EnforceConstraints = false;
        //        DS_MessageQueue.MessageQueueRow row = dS_MessageQueue.MessageQueue.NewMessageQueueRow();
        //        row.UniqueId = UniqueId;
        //        row.TerritoryNum = GetTerritoryNum(ref adapter, UniqueId, ref Message);
        //        row.Date = DateTime.Now;
        //        row.TargetApp = Constant.TargetApp;
        //        row.Sender = Sender;
        //        row.Subject = Subject;
        //        row.Message = MessageBody;
        //        row.MessageType = MessageType;
        //        row.Queue = Constant.FLAG_N;
        //        row.DeleteFlag = Constant.FLAG_N;
        //        row.ReminderSend = Constant.FLAG_N;
        //        row.ReminderStop = Constant.FLAG_N;
        //        dS_MessageQueue.MessageQueue.AddMessageQueueRow(row);

        //        try
        //        {
        //            dS_MessageQueue.EnforceConstraints = true;
        //        }
        //        catch (ConstraintException ce)
        //        {
        //            Message = ce.Message;
        //            ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
        //            return Constant.Status_Fail;
        //        }
        //        catch (Exception ex)
        //        {
        //            Message = ex.Message;
        //            ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //            return Constant.Status_Fail;
        //        }

        //        DS_MessageQueue dS_MessageQueueServer = new DS_MessageQueue();
        //        BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
        //        //int MessageId, index;
        //        foreach (DS_MessageQueue.MessageQueueRow rows in dS_MessageQueue.MessageQueue.Rows)
        //        {
        //            dS_MessageQueueServer.MessageQueue.Clear();
        //            dS_MessageQueueServer.MessageQueue.ImportRow(rows);
        //            //Insert in MessageQueue
        //            objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_MessageQueueServer.MessageQueue, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
        //            if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageQueue.Rows.Count)
        //            {
        //                Message = objUpdateTableInfo.ErrorMessage;
        //                return Constant.Status_Fail;
        //            }

        //            //if (dS_MessageQueue.MessageAttachment.Rows.Count > 0)
        //            //{
        //            //    dS_MessageQueueServer.MessageAttachment.Clear();

        //            //    //Get MessageId.
        //            //    command.Parameters.Clear();
        //            //    command.CommandText = "SELECT @@IDENTITY AS 'Identity';";
        //            //    Object objMessageId = command.ExecuteScalar();
        //            //    if (objMessageId != null && objMessageId.ToString().Trim() != String.Empty)
        //            //    {
        //            //        //Set MessageId to Attachment.
        //            //        MessageId = Convert.ToInt32(objMessageId);
        //            //        foreach (DS_MessageQueue.MessageAttachmentRow attachmentRow in dS_MessageQueue.MessageAttachment.Rows)
        //            //        {
        //            //            dS_MessageQueueServer.MessageAttachment.ImportRow(attachmentRow);
        //            //            index = dS_MessageQueueServer.MessageAttachment.Rows.Count - 1;
        //            //            dS_MessageQueueServer.MessageAttachment[index].MessageId = MessageId;
        //            //        }
        //            //        //Insert in MessageAttachment
        //            //        objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_MessageQueueServer.MessageAttachment, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
        //            //        if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageAttachment.Rows.Count)
        //            //        {
        //            //            Message = objUpdateTableInfo.ErrorMessage;
        //            //            return Constant.Status_Fail;
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        Message = "Fail to save Message Queue data.";
        //            //        return Constant.Status_Success;
        //            //    }
        //            //}
        //        }

        //        Message = "Message Queue data saved successfully.";
        //        return Constant.Status_Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = ex.Message;
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        return Constant.Status_Fail;
        //    }
        //}

        public Byte SaveMessageQueue(ref IDbCommand command, String UniqueId, String Sender, String Subject, String MessageBody, String MessageType, String order_id, String shipper_id, String load_inquiry_no, String truck_id, String driver_id, String owner_id, ref String Message)
        {
            try
            {
                if (UniqueId == null || UniqueId.Trim() == String.Empty)
                {
                    Message = "Please supplied UniqueId.";
                    return Constant.Status_Fail;
                }
                else if (Sender == null || Sender.Trim() == String.Empty)
                {
                    Message = "Please supplied Sender.";
                    return Constant.Status_Fail;
                }
                else if (Subject == null || Subject.Trim() == String.Empty)
                {
                    Message = "Please supplied Subject.";
                    return Constant.Status_Fail;
                }
                else if (MessageBody == null || MessageBody.Trim() == String.Empty)
                {
                    Message = "Please supplied MessageBody.";
                    return Constant.Status_Fail;
                }
                else if (MessageType == null || MessageType.Trim() == String.Empty)
                {
                    Message = "Please supplied MessageType.";
                    return Constant.Status_Fail;
                }

                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                DS_MessageQueue dS_MessageQueue = new DS_MessageQueue();
                dS_MessageQueue.EnforceConstraints = false;
                DS_MessageQueue.MessageQueueRow row = dS_MessageQueue.MessageQueue.NewMessageQueueRow();
                row.UniqueId = UniqueId;
                row.TerritoryNum = GetTerritoryNum(ref adapter, UniqueId, ref Message);
                row.Date = DateTime.UtcNow;
                row.TargetApp = Constant.TargetApp;
                row.Sender = Sender;
                row.Subject = Subject;
                row.Message = MessageBody;
                row.MessageType = MessageType;
                row.Queue = Constant.FLAG_N;
                row.DeleteFlag = Constant.FLAG_N;
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.order_id = order_id;
                else
                    row.Setorder_idNull();
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.shipper_id = shipper_id;
                else
                    row.Setshipper_idNull();
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.load_inquiry_no = load_inquiry_no;
                else
                    row.Setload_inquiry_noNull();
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.truck_id = truck_id;
                else
                    row.Settruck_idNull();
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.driver_id = driver_id;
                else
                    row.Setdriver_idNull();
                if (order_id != null && order_id.Trim() != String.Empty)
                    row.owner_id = order_id;
                else
                    row.Setowner_idNull();

                dS_MessageQueue.MessageQueue.AddMessageQueueRow(row);

                try
                {
                    dS_MessageQueue.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return Constant.Status_Fail;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return Constant.Status_Fail;
                }

                DS_MessageQueue dS_MessageQueueServer = new DS_MessageQueue();
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo;
                //int MessageId, index;
                foreach (DS_MessageQueue.MessageQueueRow rows in dS_MessageQueue.MessageQueue.Rows)
                {
                    dS_MessageQueueServer.MessageQueue.Clear();
                    dS_MessageQueueServer.MessageQueue.ImportRow(rows);
                    //Insert in MessageQueue
                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_MessageQueueServer.MessageQueue, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageQueue.Rows.Count)
                    {
                        Message = objUpdateTableInfo.ErrorMessage;
                        return Constant.Status_Fail;
                    }

                    //if (dS_MessageQueue.MessageAttachment.Rows.Count > 0)
                    //{
                    //    dS_MessageQueueServer.MessageAttachment.Clear();

                    //    //Get MessageId.
                    //    command.Parameters.Clear();
                    //    command.CommandText = "SELECT @@IDENTITY AS 'Identity';";
                    //    Object objMessageId = command.ExecuteScalar();
                    //    if (objMessageId != null && objMessageId.ToString().Trim() != String.Empty)
                    //    {
                    //        //Set MessageId to Attachment.
                    //        MessageId = Convert.ToInt32(objMessageId);
                    //        foreach (DS_MessageQueue.MessageAttachmentRow attachmentRow in dS_MessageQueue.MessageAttachment.Rows)
                    //        {
                    //            dS_MessageQueueServer.MessageAttachment.ImportRow(attachmentRow);
                    //            index = dS_MessageQueueServer.MessageAttachment.Rows.Count - 1;
                    //            dS_MessageQueueServer.MessageAttachment[index].MessageId = MessageId;
                    //        }
                    //        //Insert in MessageAttachment
                    //        objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_MessageQueueServer.MessageAttachment, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    //        if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageQueueServer.MessageAttachment.Rows.Count)
                    //        {
                    //            Message = objUpdateTableInfo.ErrorMessage;
                    //            return Constant.Status_Fail;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Message = "Fail to save Message Queue data.";
                    //        return Constant.Status_Success;
                    //    }
                    //}
                }

                Message = "Message Queue data saved successfully.";
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        private String GetTerritoryNum(ref IDbDataAdapter adapter, String UniqueId, ref String Message)
        {
            try
            {
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM  user_mst 
                                   WHERE  unique_id = @UniqueId ");
                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@UniqueId", DbType.String, UniqueId));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "user_mst");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "User detail data retrieved successfully for UniqueId : " + UniqueId;
                    if (ds.Tables[0].Rows[0]["user_id"].ToString().Trim() != String.Empty)
                        return ds.Tables[0].Rows[0]["user_id"].ToString();
                    else if (ds.Tables[0].Rows[0]["email_id"].ToString().Trim() != String.Empty)
                        return ds.Tables[0].Rows[0]["email_id"].ToString();
                    else
                        return "000000";
                }
                else
                {
                    Message = "User detail data is not found for UniqueId : " + UniqueId;
                    return "000000";
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return "000000";
            }
        }

        public Byte AddDeviceInfo(String AppName, String UniqueId, String DeviceId, String TokenId, String DeviceInfo, String OS, String IMEINo, ref String Message)
        {
            try
            {
                DS_DeviceInfo dS_DeviceInfo = new DS_DeviceInfo();
                dS_DeviceInfo.EnforceConstraints = false;
                DS_DeviceInfo.DeviceInfoRow row = dS_DeviceInfo.DeviceInfo.NewDeviceInfoRow();
                row.AppName = AppName;
                row.UniqueId = UniqueId;
                row.DeviceId = (DeviceId == null ? String.Empty : DeviceId);
                row.TokenId = TokenId;
                if (DeviceInfo != null && DeviceInfo.Trim() != String.Empty)
                    row.DeviceInfo = DeviceInfo;
                else
                    row.SetDeviceInfoNull();
                row.OS = OS;
                if (IMEINo != null && IMEINo.Trim() != String.Empty)
                    row.IMEINo = IMEINo;
                else
                    row.SetIMEINoNull();
                dS_DeviceInfo.DeviceInfo.AddDeviceInfoRow(row);

                try
                {
                    dS_DeviceInfo.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return Constant.Status_Fail;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return Constant.Status_Fail;
                }

                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                //Delete already existing DeviceInfo with same AppName and DeviceId
                if (OS.ToUpper() == Constant.OS_Android.ToUpper())
                {
                    DBCommand.Parameters.Clear();
                    StringBuilder SQLDelete = new StringBuilder();
                    SQLDelete.Append("DELETE FROM DeviceInfo WHERE AppName=@AppName AND (DeviceId=@DeviceId");
                    if (IMEINo != null && IMEINo.Trim() != String.Empty)
                    {
                        SQLDelete.Append(" OR IMEINo=@IMEINo");
                        DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IMEINo", DbType.String, IMEINo));
                    }
                    SQLDelete.Append(")");
                    DBCommand.CommandText = SQLDelete.ToString();
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, (DeviceId == null ? String.Empty : DeviceId)));
                    DBCommand.ExecuteNonQuery();

                    DBCommand.Parameters.Clear();
                    SQLDelete = new StringBuilder();
                    SQLDelete.Append("DELETE FROM MessageQueueFinal WHERE AppName=@AppName AND SentStatus='N' AND (DeviceId=@DeviceId");
                    if (IMEINo != null && IMEINo.Trim() != String.Empty)
                    {
                        SQLDelete.Append(" OR IMEINo=@IMEINo");
                        DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IMEINo", DbType.String, IMEINo));
                    }
                    SQLDelete.Append(")");
                    DBCommand.CommandText = SQLDelete.ToString();
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, (DeviceId == null ? String.Empty : DeviceId)));
                    DBCommand.ExecuteNonQuery();
                }
                else
                {
                    DBCommand.Parameters.Clear();
                    StringBuilder SQLDelete = new StringBuilder();
                    SQLDelete.Append("DELETE FROM DeviceInfo WHERE AppName=@AppName AND (DeviceId=@DeviceId OR TokenId=@TokenId)");
                    DBCommand.CommandText = SQLDelete.ToString();
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, (DeviceId == null ? String.Empty : DeviceId)));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TokenId", DbType.String, TokenId));
                    DBCommand.ExecuteNonQuery();

                    DBCommand.Parameters.Clear();
                    SQLDelete = new StringBuilder();
                    SQLDelete.Append("DELETE FROM MessageQueueFinal WHERE AppName=@AppName AND (DeviceId=@DeviceId OR TokenId=@TokenId) AND SentStatus='N'");
                    DBCommand.CommandText = SQLDelete.ToString();
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, (DeviceId == null ? String.Empty : DeviceId)));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TokenId", DbType.String, TokenId));
                    DBCommand.ExecuteNonQuery();
                }

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_DeviceInfo.DeviceInfo, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_DeviceInfo.DeviceInfo.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "Device registered fail." + Environment.NewLine + objUpdateTableInfo.ErrorMessage;
                    return Constant.Status_Fail;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "Device registered successfully.";
                    return Constant.Status_Success;
                }
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public Byte RemoveDeviceInfo(String AppName, String DeviceId, ref String Message)
        {
            try
            {
                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DBCommand.Parameters.Clear();
                StringBuilder SQLDelete = new StringBuilder();
                SQLDelete.Append("DELETE FROM DeviceInfo WHERE AppName=@AppName AND (DeviceId=@DeviceId OR TokenId=@TokenId)");
                DBCommand.CommandText = SQLDelete.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, DeviceId));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TokenId", DbType.String, DeviceId));
                int NoOfRowsDelete = DBCommand.ExecuteNonQuery();
                //if (NoOfRowsDelete <= 0)
                //{
                //    DBCommand.Transaction.Rollback();
                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //    Message = "Device unregistered fail.";
                //    return Constant.Status_Fail;
                //}
                //else
                {
                    DBCommand.Parameters.Clear();
                    SQLDelete = new StringBuilder();
                    SQLDelete.Append("DELETE FROM MessageQueueFinal WHERE AppName=@AppName AND (DeviceId=@DeviceId OR TokenId=@TokenId) AND SentStatus='N'");
                    DBCommand.CommandText = SQLDelete.ToString();
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, DeviceId));
                    DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TokenId", DbType.String, DeviceId));
                    NoOfRowsDelete = DBCommand.ExecuteNonQuery();

                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "Device unregistered successfully.";
                    return Constant.Status_Success;
                }
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public Byte UpdateUnreadCount(String AppName, String UniqueId, String DeviceId, int UnreadCount, ref String Message)
        {
            try
            {
                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DBCommand.Parameters.Clear();
                StringBuilder SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE DeviceInfo ");
                SQLUpdate.Append("SET    UnreadCount=@UnreadCount ");
                SQLUpdate.Append("WHERE  AppName=@AppName AND UniqueId=@UniqueId AND DeviceId=@DeviceId");
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UnreadCount", DbType.Int32, UnreadCount));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeviceId", DbType.String, DeviceId));
                int NoOfRowsDelete = DBCommand.ExecuteNonQuery();
                if (NoOfRowsDelete <= 0)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "UnreadCount update fail.";
                    return Constant.Status_Fail;
                }
                else
                {
                    DBCommand.Transaction.Commit();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "UnreadCount updated successfully.";
                    return Constant.Status_Success;
                }
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public DataTable GetMessages(String UniqueId, String AppName, int NoOfMessageInGetMessage, DateTime? LastMessageDateTime, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(" SELECT  *    FROM ");
                SQLSelect.Append(" (SELECT      MessageQueue.MessageId, Date, Sender, Subject, Message, UniqueId, TerritoryNum, ISNULL(IsAnyAttachment, 'N') AS IsAnyAttachment, CASE WHEN ISNULL(DeleteFlag, 'N') = 'Y' THEN 'D' WHEN ISNULL(ReadFlag, 'N') = 'Y' THEN 'R' ELSE 'U' END AS Status, LastUpdateDateTime, ROW_NUMBER() OVER(ORDER BY Date DESC) as row_num, ");
                SQLSelect.Append("              SrNo, AttachmentType, AttachmentName, AttachmentPath ");
                SQLSelect.Append("  FROM    MessageQueue ");
                SQLSelect.Append("  LEFT OUTER JOIN MessageAttachment ON MessageQueue.MessageId = MessageAttachment.MessageId ");
                SQLSelect.Append("  WHERE    (UniqueId = @UniqueId) AND (TargetApp = @TargetApp) ");//AND (DeleteFlag = @DeleteFlag) ");
                SQLSelect.Append("  AND      (DateTimeToSend IS NULL OR GETDATE() >= DateTimeToSend) ");
                if (LastMessageDateTime.HasValue)
                {
                    SQLSelect.Append("  AND    (Date >= @LastMessageDateTime OR LastUpdateDateTime >= @LastMessageDateTime) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastMessageDateTime", DbType.DateTime, LastMessageDateTime.Value.AddSeconds(1)));
                }
                SQLSelect.Append(" ) AS TempMessageQueue ");
                SQLSelect.Append(" WHERE   (row_num <= " + NoOfMessageInGetMessage + ") ");//Last 20 Message
                SQLSelect.Append(" ORDER BY row_num ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                //DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeleteFlag", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "MessageQueue");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "MessageQueue retrieved succesfully for UniqueId : " + UniqueId;
                    return ds.Tables[0];
                }
                else
                {
                    Message = " MessageQueue not found for UniqueId : " + UniqueId;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public DataTable GetPastMessages(String UniqueId, String AppName, int NoOfMessageInGetMessage, DateTime? LastMessageDateTime, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(" SELECT  *    FROM ");
                SQLSelect.Append(" (SELECT      MessageQueue.MessageId, Date, Sender, Subject, Message, UniqueId, TerritoryNum, ISNULL(IsAnyAttachment, 'N') AS IsAnyAttachment, CASE WHEN ISNULL(DeleteFlag, 'N') = 'Y' THEN 'D' WHEN ISNULL(ReadFlag, 'N') = 'Y' THEN 'R' ELSE 'U' END AS Status, LastUpdateDateTime, ROW_NUMBER() OVER(ORDER BY Date DESC) as row_num, ");
                SQLSelect.Append("              SrNo, AttachmentType, AttachmentName, AttachmentPath ");
                SQLSelect.Append("  FROM    MessageQueue ");
                SQLSelect.Append("  LEFT OUTER JOIN MessageAttachment ON MessageQueue.MessageId = MessageAttachment.MessageId ");
                SQLSelect.Append("  WHERE    (UniqueId = @UniqueId) AND (TargetApp = @TargetApp) ");//AND (DeleteFlag = @DeleteFlag) ");
                SQLSelect.Append("  AND      (DateTimeToSend IS NULL OR GETDATE() >= DateTimeToSend) ");
                if (LastMessageDateTime.HasValue)
                {
                    SQLSelect.Append("  AND    (Date < @LastMessageDateTime) ");// OR LastUpdateDateTime < @LastMessageDateTime) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastMessageDateTime", DbType.DateTime, LastMessageDateTime.Value.AddSeconds(-1)));
                }
                SQLSelect.Append(" ) AS TempMessageQueue ");
                SQLSelect.Append(" WHERE   (row_num <= " + NoOfMessageInGetMessage + ") ");//Last 20 Message
                SQLSelect.Append(" ORDER BY row_num ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                //DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeleteFlag", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "MessageQueue");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "MessageQueue retrieved succesfully for UniqueId : " + UniqueId;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "MessageQueue not found for UniqueId : " + UniqueId;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public DataTable GetMessageAttachment(String MessageIds, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(" SELECT  * ");
                SQLSelect.Append(" FROM    MessageAttachment ");
                if (MessageIds != null && MessageIds.Trim() != String.Empty)
                    SQLSelect.Append(" WHERE   (MessageId IN (" + MessageIds + ")) ");
                SQLSelect.Append(" ORDER BY MessageId, SrNo ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "MessageAttachment");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "MessageAttachment retrieved succesfully for MessageIds : " + MessageIds;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "MessageAttachment not found for MessageIds : " + MessageIds;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public Byte DeleteMessage(String MessageId, String AppName, ref String Message)
        {
            try
            {
                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                StringBuilder SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueue ");
                SQLUpdate.Append("SET    DeleteFlag=@DeleteFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (MessageId IN (" + MessageId + ")) AND (TargetApp=@TargetApp) ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeleteFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                int NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                if (NoOfRowsUpdate <= 0)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "Fail to delete Message for MessageId : " + MessageId;
                    return Constant.Status_Fail;
                }

                SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueueFinal ");
                SQLUpdate.Append("SET    DeleteFlag=@DeleteFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (MessageId IN (" + MessageId + ")) AND (AppName=@AppName) ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeleteFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                //if (NoOfRowsUpdate <= 0)
                //{
                //    DBCommand.Transaction.Rollback();
                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //    Message = "Fail to delete Message for MessageId : " + MessageId;
                //    return Constant.Status_Fail;
                //}

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "Successfully delete Message for MessageId : " + MessageId;
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public Byte MarkReadMessage(String MessageId, String AppName, ref String Message)
        {
            try
            {
                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                StringBuilder SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueue ");
                SQLUpdate.Append("SET    ReadFlag=@ReadFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (MessageId IN (" + MessageId + ")) AND (TargetApp=@TargetApp) ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ReadFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                int NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                if (NoOfRowsUpdate <= 0)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "Fail to Mark Read Message for MessageId : " + MessageId;
                    return Constant.Status_Fail;
                }

                SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueueFinal ");
                SQLUpdate.Append("SET    ReadFlag=@ReadFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (MessageId IN (" + MessageId + ")) AND (AppName=@AppName) ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ReadFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                //if (NoOfRowsUpdate <= 0)
                //{
                //    DBCommand.Transaction.Rollback();
                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //    Message = "Fail to Mark Read Message for MessageId : " + MessageId;
                //    return Constant.Status_Fail;
                //}

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "Successfully Mark Read Message for MessageId : " + MessageId;
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public int GetUnreadCount(String AppName, String UniqueId, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@" SELECT   COUNT(*) AS UnreadCount 
                                    FROM     MessageQueue 
                                    WHERE    (UniqueId = @UniqueId) AND (TargetApp = @TargetApp) 
                                    AND      (DeleteFlag = @DeleteFlag) AND (ReadFlag = @ReadFlag) ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@DeleteFlag", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ReadFlag", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "MessageQueue");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["UnreadCount"].ToString().Trim() != String.Empty)
                {
                    Message = "Unread Message count retrieved succesfully for UniqueId : " + UniqueId;
                    return Convert.ToInt32(ds.Tables[0].Rows[0]["UnreadCount"]);
                }
                else
                {
                    Message = "Unread Message count not found for UniqueId : " + UniqueId;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return -1;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public Byte SaveUserAttachmentOperationHistory(DataSet dSUpload, ref String Message)
        {
            try
            {
                if (dSUpload == null || dSUpload.Tables.Count <= 0 || dSUpload.Tables[0].Rows.Count <= 0)
                {
                    Message = "No data available to Save. Operation Cancelled.";
                    return Constant.Status_Fail;
                }
                DS_UserAttachmentOperationHistory dS_UserAttachmentOperationHistory = (DS_UserAttachmentOperationHistory)dSUpload;

                try
                {
                    dS_UserAttachmentOperationHistory.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return Constant.Status_Fail;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return Constant.Status_Fail;
                }

                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                //Insert in UserAttachmentOperationHistory
                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_UserAttachmentOperationHistory.UserAttachmentOperationHistory, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_UserAttachmentOperationHistory.UserAttachmentOperationHistory.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = objUpdateTableInfo.ErrorMessage;
                    return Constant.Status_Fail;
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "User Attachment Operation History data saved successfully.";
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }

        public DataTable GetRepAllowNotification(String UniqueId, String AppName, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(" SELECT    MessageTypeMst.MessageType, MessageTypeMst.MessageTypeDesc, ISNULL(MessageAllowToRep.AllowNotification, 'Y') AS AllowNotification ");
                SQLSelect.Append(" FROM      MessageTypeMst ");
                SQLSelect.Append(" LEFT OUTER JOIN MessageAllowToRep ON MessageTypeMst.MessageType = MessageAllowToRep.MessageType ");
                SQLSelect.Append(" WHERE     (MessageTypeMst.AppName = @AppName) AND (MessageAllowToRep.UniqueId = @UniqueId OR MessageAllowToRep.UniqueId IS NULL) ");

                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "MessageAllowToRep");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "MessageAllowToRep retrieved succesfully for UniqueId : " + UniqueId;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "MessageAllowToRep not found for UniqueId : " + UniqueId;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }

        public Byte SavesRepAllowNotification(String UniqueId, String AppName, DataSet dSUpload, ref String Message)
        {
            try
            {
                if (dSUpload == null || dSUpload.Tables.Count <= 0 || dSUpload.Tables[0].Rows.Count <= 0)
                {
                    Message = "No data available to Save. Operation Cancelled.";
                    return Constant.Status_Fail;
                }
                DS_MessageAllowToRep dS_MessageAllowToRep = (DS_MessageAllowToRep)dSUpload;

                try
                {
                    dS_MessageAllowToRep.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return Constant.Status_Fail;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return Constant.Status_Fail;
                }

                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                DBCommand.Parameters.Clear();
                StringBuilder SQLDelete = new StringBuilder();
                SQLDelete.Append(" DELETE  FROM  MessageAllowToRep WHERE  (MessageAllowToRep.AppName = @AppName) AND (MessageAllowToRep.UniqueId = @UniqueId) ");
                DBCommand.CommandText = SQLDelete.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                int NoOfRowsDelete = DBCommand.ExecuteNonQuery();

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref DBCommand, dS_MessageAllowToRep.MessageAllowToRep, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_MessageAllowToRep.MessageAllowToRep.Rows.Count)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = objUpdateTableInfo.ErrorMessage;
                    return Constant.Status_Fail;
                }

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "Message AllowToRep data saved successfully.";
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
            finally
            {
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
            }
        }
      
        public Byte MarkReadALLMessage(String UniqueId, String AppName, ref String Message)
        {
            try
            {
                //Establish DataBase Connection.
                if (DBConnection.State == ConnectionState.Closed)
                    DBConnection.Open();
                //Begin Transaction.
                DBCommand.Transaction = DBConnection.BeginTransaction();

                StringBuilder SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueue ");
                SQLUpdate.Append("SET    ReadFlag=@ReadFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (UniqueId=@UniqueId) AND (TargetApp=@TargetApp) AND (ReadFlag='N') ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TargetApp", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ReadFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                int NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                if (NoOfRowsUpdate <= 0)
                {
                    DBCommand.Transaction.Rollback();
                    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                    Message = "No Messages for UniqueId : " + UniqueId;
                    return Constant.Status_Success;
                }

                SQLUpdate = new StringBuilder();
                SQLUpdate.Append("UPDATE MessageQueueFinal ");
                SQLUpdate.Append("SET    ReadFlag=@ReadFlag, LastUpdateDateTime=@LastUpdateDateTime ");
                SQLUpdate.Append("WHERE  (UniqueId=@UniqueId) AND (AppName=@AppName) AND (ReadFlag='N') ");
                DBCommand.Parameters.Clear();
                DBCommand.CommandText = SQLUpdate.ToString();
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@UniqueId", DbType.String, UniqueId));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@AppName", DbType.String, AppName));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ReadFlag", DbType.String, Constant.FLAG_Y));
                DBCommand.Parameters.Add(DBObjectFactory.MakeParameter("@LastUpdateDateTime", DbType.DateTime, DateTime.UtcNow));
                NoOfRowsUpdate = DBCommand.ExecuteNonQuery();
                //if (NoOfRowsUpdate <= 0)
                //{
                //    DBCommand.Transaction.Rollback();
                //    if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                //    Message = "Fail to Mark Read ALL Messages for UniqueId : " + UniqueId;
                //    return Constant.Status_Fail;
                //}

                DBCommand.Transaction.Commit();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = "Successfully Mark Read ALL Messages for UniqueId : " + UniqueId;
                return Constant.Status_Success;
            }
            catch (Exception ex)
            {
                if (DBCommand.Transaction != null)
                    DBCommand.Transaction.Rollback();
                if (DBConnection.State == ConnectionState.Open) DBConnection.Close();
                Message = ex.Message;
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                return Constant.Status_Fail;
            }
        }


    }
}
