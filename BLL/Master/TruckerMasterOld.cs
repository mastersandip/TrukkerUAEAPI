using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BLL.Utilities;

namespace trukkerUAE.BLL.Master
{
    public class TruckerMaster : ServerBase
    {
        public DataTable GetSizeTypeDetail(String SizeTypeCode, Decimal TotalDistance, String TotalDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT   SizeTypeCode, SizeTypeDesc, 
                                            TruckTypeMst.truck_type_code, truck_type_desc, NoOfTruck, 0.0000 AS MinDistance, CAST(NULL AS varchar) AS MinDistanceUOM,
                                            DriverTypeMst.driver_type_code, driver_type_desc, NoOfDriver, 0.0000 AS RatePerDriver, 0.0000 AS TotalDriverRate, 
                                            LabourTypeMst.labour_type_code, labour_type_desc, NoOfLabour, 0.0000 AS RatePerLabour, 0.0000 AS TotalLabourRate,
                                            HandimanTypeMst.Handiman_type_code, Handiman_type_desc, NoOfHandiman, 0.0000 AS RatePerHandiman, 0.0000 AS TotalHandimanRate, 
                                            @TotalDistance AS TotalDistance, DistanceTypeMst.distance_type_code, distance_type_desc, 0.0000 AS RatePerDistanceUOM, 0.0000 AS TotalDistanceRate, 
                                            TimeForLoadingInMinute, TimeForUnloadingInMinute, 0.0000 AS TimeToTravelInMinute, 0.0000 AS TotalTimeForPostOrder, 0.0000 AS NoOfDay, 0.0000 AS Total_cost   
                                            ,SizeTypeMst.Packing_rate AS TotalPackingRate
                                           FROM SizeTypeMst 
                                           LEFT OUTER JOIN TruckTypeMst ON SizeTypeMst.truck_type_code = TruckTypeMst.truck_type_code 
                                           LEFT OUTER JOIN DriverTypeMst ON SizeTypeMst.driver_type_code = DriverTypeMst.driver_type_code 
                                           LEFT OUTER JOIN LabourTypeMst ON SizeTypeMst.labour_type_code = LabourTypeMst.labour_type_code 
                                           LEFT OUTER JOIN HandimanTypeMst ON SizeTypeMst.Handiman_type_code = HandimanTypeMst.Handiman_type_code 
                                           CROSS JOIN DistanceTypeMst 
                                           WHERE  SizeTypeCode = @SizeTypeCode 
                                           AND  DistanceTypeMst.distance_type_code = @distance_type_code ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@TotalDistance", DbType.Decimal, TotalDistance));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@distance_type_code", DbType.String, TotalDistanceUOM));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "SizeTypeMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "SizeType detail data retrieved successfully for SizeTypeCode : " + SizeTypeCode;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "SizeType detail data is not found for SizeTypeCode : " + SizeTypeCode;
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }

        public Byte GetTruckRate(String truck_type_code, String rate_type_flag, int NoOfTruck, Decimal NoOfDay, DateTime OrderDate, ref Decimal MinDistance, ref String MinDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM TruckRateDetail 
                                   WHERE  truck_type_code = @truck_type_code 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truck_type_code", DbType.String, truck_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "TruckRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Truck Rate detail data retrieved successfully for truck_type_code : " + truck_type_code;
                    MinDistance = Convert.ToDecimal(ds.Tables[0].Rows[0]["MinDistance"]);
                    MinDistanceUOM = ds.Tables[0].Rows[0]["MinDistanceUOM"].ToString();
                    return 1;
                }
                else
                {
                    Message = "Truck Rate detail data is not found for truck_type_code : " + truck_type_code;
                    return 2;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public Byte GetDriverTruckRate(String truck_type_code, int NoOfTruck, Decimal NoOfDay, DateTime OrderDate, ref Decimal MinDistance, ref String MinDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DriverTruckRateDetail 
                                   WHERE  truck_type_code = @truck_type_code 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@truck_type_code", DbType.String, truck_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "TruckRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Truck Rate detail data retrieved successfully for truck_type_code : " + truck_type_code;
                    MinDistance = Convert.ToDecimal(ds.Tables[0].Rows[0]["MinDistance"]);
                    MinDistanceUOM = ds.Tables[0].Rows[0]["MinDistanceUOM"].ToString();
                    return 1;
                }
                else
                {
                    Message = "Truck Rate detail data is not found for truck_type_code : " + truck_type_code;
                    return 2;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public Decimal GetDriverRate(String driver_type_code, String rate_type_flag, int NoOfDriver, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerDriver, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DriverRateDetail 
                                   WHERE  driver_type_code = @driver_type_code 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@driver_type_code", DbType.String, driver_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "DriverRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Driver Rate detail data retrieved successfully for driver_type_code : " + driver_type_code;
                    RatePerDriver = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerDriver * NoOfDriver * NoOfDay;
                }
                else
                {
                    Message = "Driver Rate detail data is not found for driver_type_code : " + driver_type_code;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        public Decimal GetLabourRate(String labour_type_code, String rate_type_flag, int NoOfLabour, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerLabour, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM LabourRateDetail 
                                   WHERE  labour_type_code = @labour_type_code 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@labour_type_code", DbType.String, labour_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "LabourRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Labour Rate detail data retrieved successfully for labour_type_code : " + labour_type_code;
                    RatePerLabour = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerLabour * NoOfLabour * NoOfDay;
                }
                else
                {
                    Message = "Labour Rate detail data is not found for labour_type_code : " + labour_type_code;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        public Decimal GetHandimanRate(String handiman_type_code, String rate_type_flag, int NoOfHandiman, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerHandiman, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM HandimanRateDetail 
                                   WHERE  handiman_type_code = @handiman_type_code 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@handiman_type_code", DbType.String, handiman_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "HandimanRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Handiman Rate detail data retrieved successfully for handiman_type_code : " + handiman_type_code;
                    RatePerHandiman = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerHandiman * NoOfHandiman * NoOfDay;
                }
                else
                {
                    Message = "Handiman Rate detail data is not found for handiman_type_code : " + handiman_type_code;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        //Method NotUsed
        //        public Decimal GetHelperRate(String Helper_type_code, String rate_type_flag, int NoOfHelper, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerHelper, ref String Message)
        //        {
        //            try
        //            {
        //                DBDataAdpterObject.SelectCommand.Parameters.Clear();
        //                StringBuilder SQLSelect = new StringBuilder();
        //                SQLSelect.Append(@"SELECT * FROM HelperRateDetail 
        //                                   WHERE  Helper_type_code = @Helper_type_code 
        //                                   AND    rate_type_flag = @rate_type_flag 
        //                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
        //                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@Helper_type_code", DbType.String, Helper_type_code));
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
        //                DBDataAdpterObject.TableMappings.Clear();
        //                DBDataAdpterObject.TableMappings.Add("Table", "HelperRateDetail");
        //                DataSet ds = new DataSet();
        //                DBDataAdpterObject.Fill(ds);
        //                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //                {
        //                    Message = "Helper Rate detail data retrieved successfully for Helper_type_code : " + Helper_type_code;
        //                    RatePerHelper = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
        //                    return RatePerHelper * NoOfHelper * NoOfDay;
        //                }
        //                else
        //                {
        //                    Message = "Helper Rate detail data is not found for Helper_type_code : " + Helper_type_code;
        //                    return -1;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                Message = ex.Message;
        //                return -1;
        //            }
        //        }

        public Decimal GetDistanceRate(String distance_type_code, String rate_type_flag, Decimal TotalDistance, int NoOfTruck, DateTime OrderDate, ref Decimal RatePerDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DistanceRateDetail 
                                   WHERE  distance_type_code = @distance_type_code 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@distance_type_code", DbType.String, distance_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "DistanceRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Distance Rate detail data retrieved successfully for distance_type_code : " + distance_type_code;
                    RatePerDistanceUOM = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerDistanceUOM * TotalDistance * NoOfTruck;
                }
                else
                {
                    Message = "Distance Rate detail data is not found for distance_type_code : " + distance_type_code;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        public Decimal GetConversionFactor(String FromUOM, String ToUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DistanceConversion 
                                   WHERE  (FromUOM = @FromUOM) 
                                   AND    (ToUOM <= @ToUOM) ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@FromUOM", DbType.String, FromUOM));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@ToUOM", DbType.String, ToUOM));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "DistanceConversion");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Conversion Factor retrieved successfully.";
                    return Convert.ToDecimal(ds.Tables[0].Rows[0]["ConversionFactor"]); ;
                }
                else
                {
                    Message = "Fail to get Conversion Factor.";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        public Decimal GetPackingRate(String Packing_type_code, DateTime OrderDate, ref Decimal RatePerSizeTypeCode, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM PackingRateDetail 
                                   WHERE  Packing_type_code = @Packing_type_code 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@Packing_type_code", DbType.String, Packing_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "PackingRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Packing Rate detail data retrieved successfully for Packing_type_code : " + Packing_type_code;
                    RatePerSizeTypeCode = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerSizeTypeCode;
                }
                else
                {
                    Message = "Packing Rate detail data is not found for Packing_type_code : " + Packing_type_code;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
            }
        }

        public DataTable GetDriverPriceRate(ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DriverPriceMst ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "DriverPriceMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Driver Price detail data retrieved successfully ";
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Driver Price detail data Not Found ";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }

        public DataTable CalculateRate(String SizeTypeCode, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinute, String IncludePackingCharge, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, int? NoOfHandiman, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                DateTime OrderDate = DateTime.Today;

                if (SizeTypeCode.Trim() == String.Empty)
                {
                    Message = "Please supplied SizeTypeCode.";
                    return null;
                }
                else if (TotalDistance < 0)
                {
                    Message = "Please supplied TotalDistance.";
                    return null;
                }
                else if (TotalDistanceUOM.Trim() == String.Empty)
                {
                    Message = "Please supplied TotalDistance Unit.";
                    return null;
                }
                else if (TimeToTravelInMinute < 0)
                {
                    Message = "Please supplied Time To Travel In Minute.";
                    return null;
                }

                ParameterMst objParameterMst = new ParameterMst();
                DataTable dtParameter = objParameterMst.GetParameter("FullDayRule", null, ref Message);
                if (dtParameter == null || dtParameter.Rows.Count <= 0)
                    return null;

                //FullDayRule
                Decimal FullDayRule = 0.6M, WorkingHours = 8.0M;
                DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                drArray = dtParameter.Select("Code='WorkingHours'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {
                    Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
                    Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
                    Decimal TotalTimeForPostOrder = TimeForLoadingInMinute + TimeToTravelInMinute + TimeForUnloadingInMinute;
                    Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                    Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                    if (IncompleteDay < FullDayRule)
                        NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                    else
                        NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                    //String Packing_type_code = dtSizeTypeMst.Rows[0]["Packing_type_code"].ToString();

                    //Truck Rate
                    String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                    if (!NoOfTruck.HasValue)
                        NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);
                    Decimal MinDistance = 0;
                    String MinDistanceUOM = String.Empty;
                    Byte result = GetTruckRate(truck_type_code, rate_type_flag, NoOfTruck.Value, NoOfDay, OrderDate, ref MinDistance, ref MinDistanceUOM, ref Message);
                    if (result != 1)
                        return null;
                    else
                    {
                        //Driver Rate
                        String driver_type_code = dtSizeTypeMst.Rows[0]["driver_type_code"].ToString();
                        if (!NoOfDriver.HasValue)
                            NoOfDriver = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfDriver"]);
                        Decimal RatePerDriver = -1, TotalDriverRate = GetDriverRate(driver_type_code, rate_type_flag, NoOfDriver.Value, NoOfDay, OrderDate, ref RatePerDriver, ref Message);
                        if (TotalDriverRate < 0)
                            return null;
                        else
                        {
                            //Labour Rate
                            String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                            if (!NoOfLabour.HasValue)
                                NoOfLabour = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"]);
                            Decimal RatePerLabour = -1, TotalLabourRate = GetLabourRate(labour_type_code, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                            if (TotalLabourRate < 0)
                                return null;
                            else
                            {
                                //Handiman Rate
                                String Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                                if (!NoOfHandiman.HasValue)
                                    NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);
                                Decimal RatePerHandiman = -1, TotalHandimanRate = GetHandimanRate(Handiman_type_code, rate_type_flag, NoOfHandiman.Value, NoOfDay, OrderDate, ref RatePerHandiman, ref Message);
                                if (TotalHandimanRate < 0)
                                    return null;
                                else
                                {
                                    ////Helper Rate
                                    //String helper_type_code = dtSizeTypeMst.Rows[0]["helper_type_code"].ToString();
                                    //if (!NoOfHelper.HasValue)
                                    //    NoOfHelper = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHelper"]);
                                    //Decimal RatePerHelper = -1, TotalHelperRate = GetHelperRate(helper_type_code, NoOfHelper.Value, NoOfDay, OrderDate, ref RatePerHelper, ref Message);
                                    //if (TotalHelperRate < 0)
                                    //    return null;
                                    //else
                                    //{
                                    if (MinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                                    {
                                        Decimal ConversionFactor = GetConversionFactor(MinDistanceUOM, TotalDistanceUOM, ref Message);
                                        if (ConversionFactor < 0)
                                            return null;
                                        else
                                        {
                                            MinDistance = MinDistance * ConversionFactor;
                                            MinDistanceUOM = TotalDistanceUOM;
                                        }
                                    }

                                    if (TotalDistance < MinDistance)
                                        TotalDistance = MinDistance;

                                    //Total Distance Rate
                                    Decimal RatePerDistanceUOM = -1, TotalDistanceRate = GetDistanceRate(TotalDistanceUOM, rate_type_flag, TotalDistance, NoOfTruck.Value, OrderDate, ref RatePerDistanceUOM, ref Message);
                                    if (TotalDistanceRate < 0)
                                        return null;
                                    else
                                    {
                                        Decimal TotalPackingRate = 0;
                                        if (IncludePackingCharge == "N")
                                            TotalPackingRate = 0;
                                        else
                                            TotalPackingRate = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TotalPackingRate"]);
                                        //Decimal TotalPackingRate = 0;
                                        //if (dtPostOrderParameter.Rows[0]["IncludePackingCharge"].ToString() == "Y")
                                        //{
                                        //    Decimal RatePerSizeTypeCode = -1;
                                        //    TotalPackingRate = objTruckerMaster.GetPackingRate(Packing_type_code, OrderDate, ref RatePerSizeTypeCode, ref Message);
                                        //    if (TotalPackingRate < 0)
                                        //        return null;
                                        //}

                                        Decimal BaseRate = TotalDistanceRate + TotalDriverRate ;
                                        Decimal TotalRate = TotalDriverRate + TotalLabourRate + TotalHandimanRate + TotalDistanceRate + TotalPackingRate;

                                        dtSizeTypeMst.Rows[0]["MinDistance"] = MinDistance;
                                        dtSizeTypeMst.Rows[0]["MinDistanceUOM"] = MinDistanceUOM;
                                        dtSizeTypeMst.Rows[0]["RatePerDriver"] = RatePerDriver;
                                        dtSizeTypeMst.Rows[0]["TotalDriverRate"] = TotalDriverRate;
                                        dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;
                                        dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;
                                        dtSizeTypeMst.Rows[0]["RatePerHandiman"] = RatePerHandiman;
                                        dtSizeTypeMst.Rows[0]["TotalHandimanRate"] = TotalHandimanRate;
                                        //dtSizeTypeMst.Rows[0]["RatePerHelper"] = RatePerHelper;
                                        //dtSizeTypeMst.Rows[0]["TotalHelperRate"] = TotalHelperRate;
                                        dtSizeTypeMst.Rows[0]["RatePerDistanceUOM"] = RatePerDistanceUOM;
                                        dtSizeTypeMst.Rows[0]["TotalDistanceRate"] = TotalDistanceRate;
                                        dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TotalTimeForPostOrder;
                                        dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;
                                        dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;
                                        dtSizeTypeMst.Rows[0]["Total_cost"] = TotalRate;
                                        dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck.ToString() == dtSizeTypeMst.Rows[0]["NoOfTruck"].ToString() ? dtSizeTypeMst.Rows[0]["NoOfTruck"] : NoOfTruck;
                                        dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver.ToString() == dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString() ? dtSizeTypeMst.Rows[0]["NoOfDriver"] : NoOfDriver;
                                        dtSizeTypeMst.Rows[0]["NoOfHandiman"] = NoOfHandiman.ToString() == dtSizeTypeMst.Rows[0]["NoOfHandiman"].ToString() ? dtSizeTypeMst.Rows[0]["NoOfHandiman"] : NoOfHandiman;
                                        //dtSizeTypeMst.Rows[0]["NoOfHelper"] = NoOfHelper.ToString() == dtSizeTypeMst.Rows[0]["NoOfHelper"].ToString() ? dtSizeTypeMst.Rows[0]["NoOfHelper"] : NoOfHelper;
                                        dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour.ToString() == dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString() ? dtSizeTypeMst.Rows[0]["NoOfLabour"] : NoOfLabour;

                                        dtSizeTypeMst.Columns.Add("BaseRate", typeof(String));
                                        dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;

                                        dtSizeTypeMst.Columns.Add("rate_type_flag", typeof(String));
                                        dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;

                                        return dtSizeTypeMst;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }

        public DataTable GetPost_Load_Inquiry(ref IDbDataAdapter adapter, String load_inquiry_no, ref String Message)
        {
            try
            {
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM post_load_inquiry WHERE load_inquiry_no=@load_inquiry_no ");
                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(BLGeneralUtil.MakeParameter("load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "post_load_inquiry");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Post Load Inquiry data retrieved successfully ";
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Post Load Inquiry data Not Found ";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }
    }
}