using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BLL.Utilities;
using trukkerUAE.Classes;
using trukkerUAE.XSD;
using trukkerUAE.Controllers;
using BLL.Master;


namespace trukkerUAE.BLL.Master
{
    public class TruckerMaster : ServerBase
    {
        public DataTable GetSizeTypeMst(ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  * 
                                   FROM  SizeTypeMst    
                                   WHERE  IsActive = @IsActive 
                                   ORDER BY sr_no ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsActive", DbType.String, Constant.FLAG_Y));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "SizeTypeMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "SizeType master data retrieved successfully.";
                    return ds.Tables[0];
                }
                else
                {
                    Message = "SizeType master data is not found.";
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

        //        public DataTable GetSizeTypeDetail(String SizeTypeCode, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, ref String Message)
        //        {
        //            try
        //            {
        //                DBDataAdpterObject.SelectCommand.Parameters.Clear();
        //                StringBuilder SQLSelect = new StringBuilder();
        //                SQLSelect.Append(@"SELECT   SizeTypeMatrix.SizeTypeCode, SizeTypeMst.SizeTypeDesc, CAST(NULL AS varchar) AS goods_type_flag, SizeTypeMatrix.rate_type_flag, 0 AS NoOfDay, 
        //                                            TruckTypeMst.truck_type_code, truck_type_desc, NoOfTruck, 0 AS MinDistance, CAST(NULL AS varchar) AS MinDistanceUOM, 0 AS MinDistanceRate, 
        //                                            @TotalDistance AS TotalDistance, DistanceTypeMst.distance_type_code, distance_type_desc, 0 AS RatePerDistanceUOM, RatePerKM, 0 AS TotalTravelingRate, 
        //                                            DriverTypeMst.driver_type_code, driver_type_desc, NoOfDriver, 0 AS RatePerDriver, 0 AS TotalDriverRate, 
        //                                            LabourTypeMst.labour_type_code, labour_type_desc, NoOfLabour, 0 AS RatePerLabour, 0 AS TotalLabourRate,
        //                                            HandimanTypeMst.Handiman_type_code, Handiman_type_desc, NoOfHandiman, 0 AS RatePerHandiman, 0 AS TotalHandimanRate, 
        //                                            SupervisorTypeMst.Supervisor_type_code, Supervisor_type_desc, NoOfSupervisor, 0 AS RatePerSupervisor, 0 AS TotalSupervisorRate, 
        //                                            PackingTypeMst.Packing_type_code, Packing_type_desc, 0 AS TotalPackingRate,                                             
        //                                            TimeForLoadingInMinute, RateForLoading, 0 AS ActualTimeForLoadingInMinute, 0 AS TotalRateForLoading, 
        //                                            TimeForUnloadingInMinute, RateForUnloading, 0 AS ActualTimeForUnloadingInMinute, 0 AS TotalRateForUnLoading, 
        //                                            PerUnitAboveTimeForLoadUnloadMinute, RatePerUnitAboveTimeForLoadUnload, TrukkerMarginStandard, TrukkerMarginPremium, TrukkerMargin, RatePerMinute, 
        //                                            MinRateForMovingGoods,0 AS TimeToTravelInMinute, 0.0 AS TotalRateForTravelInMinute, 0 AS TotalTimeForPostOrder, 
        //                                            CBM_Min, CBM_Max, ManDaysByCBM_Min, ManDaysByCBM_Max, RatePerCBM, 
        //                                            0 AS BaseRate, 0 AS Total_cost, 0 AS Total_cost_IncludeMargin, 0 AS Discount, 0 AS Net_cost,DiffCityTruckAdditionalCharge ,
        //                                            HireTruck_MinRate,HireTruck_FuelRatePerDay,Hiretruck_MaxKM,HireTruck_AdditionalKMCharges,Discount as AddSerBaseDiscount,
        //                                            0 AS Total_PT_Charge,0 AS Total_CL_Charge,0 AS Total_PEST_Charge,0 AS TotalAddServiceDiscount,0 AS TotalAddServiceCharge,
        //                                            0 as AddSerBaseDiscount,0 as Total_PT_Discount,0 as Total_CL_Discount,0 as Total_PEST_Discount                                                 
        //                                           FROM SizeTypeMatrix 
        //                                           INNER JOIN SizeTypeMst ON SizeTypeMatrix.SizeTypeCode = SizeTypeMst.SizeTypeCode 
        //                                           LEFT OUTER JOIN TruckTypeMst ON SizeTypeMatrix.truck_type_code = TruckTypeMst.truck_type_code 
        //                                           LEFT OUTER JOIN DriverTypeMst ON SizeTypeMatrix.driver_type_code = DriverTypeMst.driver_type_code 
        //                                           LEFT OUTER JOIN LabourTypeMst ON SizeTypeMatrix.labour_type_code = LabourTypeMst.labour_type_code 
        //                                           LEFT OUTER JOIN HandimanTypeMst ON SizeTypeMatrix.Handiman_type_code = HandimanTypeMst.Handiman_type_code 
        //                                           LEFT OUTER JOIN SupervisorTypeMst ON SizeTypeMatrix.supervisor_type_code = SupervisorTypeMst.Supervisor_type_code 
        //                                           LEFT OUTER JOIN PackingTypeMst ON SizeTypeMatrix.Packing_type_code = PackingTypeMst.Packing_type_code 
        //                                           CROSS JOIN DistanceTypeMst 
        //                                           WHERE  SizeTypeMatrix.SizeTypeCode = @SizeTypeCode 
        //                                           AND   SizeTypeMatrix.rate_type_flag = @rate_type_flag 
        //                                           AND  DistanceTypeMst.distance_type_code = @distance_type_code ");
        //                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TotalDistance", DbType.Decimal, TotalDistance));
        //                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@distance_type_code", DbType.String, TotalDistanceUOM));
        //                DBDataAdpterObject.TableMappings.Clear();
        //                DBDataAdpterObject.TableMappings.Add("Table", "SizeTypeMst");
        //                DataSet ds = new DataSet();
        //                DBDataAdpterObject.Fill(ds);
        //                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //                {
        //                    Message = "SizeType detail data retrieved successfully for SizeTypeCode : " + SizeTypeCode;
        //                    return ds.Tables[0];
        //                }
        //                else
        //                {
        //                    Message = "SizeType detail data is not found for SizeTypeCode : " + SizeTypeCode;
        //                    return null;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //                Message = ex.Message;
        //                return null;
        //            }
        //        }

        public DataTable GetSizeTypeDetail(String SizeTypeCode, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT   SizeTypeMatrix.SizeTypeCode, SizeTypeMst.SizeTypeDesc, CAST(NULL AS varchar) AS goods_type_flag, SizeTypeMatrix.rate_type_flag, 0 AS NoOfDay, 
                                            TruckTypeMst.truck_type_code, truck_type_desc, NoOfTruck, 0 AS MinDistance, CAST(NULL AS varchar) AS MinDistanceUOM, 0 AS MinDistanceRate, 
                                            @TotalDistance AS TotalDistance, DistanceTypeMst.distance_type_code, distance_type_desc, 0 AS RatePerDistanceUOM, RatePerKM, 0 AS TotalTravelingRate, 
                                            DriverTypeMst.driver_type_code, driver_type_desc, NoOfDriver, 0 AS RatePerDriver, 0 AS TotalDriverRate, 
                                            LabourTypeMst.labour_type_code, labour_type_desc, NoOfLabour, 0 AS RatePerLabour, 0 AS TotalLabourRate,
                                            HandimanTypeMst.Handiman_type_code, Handiman_type_desc, NoOfHandiman, 0 AS RatePerHandiman, 0 AS TotalHandimanRate, 
                                            SupervisorTypeMst.Supervisor_type_code, Supervisor_type_desc, NoOfSupervisor, 0 AS RatePerSupervisor, 0 AS TotalSupervisorRate, 
                                            PackingTypeMst.Packing_type_code, Packing_type_desc, 0 AS TotalPackingRate,                                             
                                            TimeForLoadingInMinute, RateForLoading, 0 AS ActualTimeForLoadingInMinute, 0 AS TotalRateForLoading, 
                                            TimeForUnloadingInMinute, RateForUnloading, 0 AS ActualTimeForUnloadingInMinute, 0 AS TotalRateForUnLoading, 
                                            PerUnitAboveTimeForLoadUnloadMinute, RatePerUnitAboveTimeForLoadUnload, TrukkerMarginStandard, TrukkerMarginPremium, TrukkerMargin, RatePerMinute, 
                                            MinRateForMovingGoods,0 AS TimeToTravelInMinute, 0.0 AS TotalRateForTravelInMinute, 0 AS TotalTimeForPostOrder, 
                                            CBM_Min, CBM_Max, ManDaysByCBM_Min, ManDaysByCBM_Max, RatePerCBM, 
                                            0 AS BaseRate, 0 AS Total_cost, 0 AS Total_cost_IncludeMargin, 0 AS Discount, 0 AS Net_cost,DiffCityTruckAdditionalCharge ,
                                            HireTruck_MinRate,HireTruck_FuelRatePerDay,Hiretruck_MaxKM,HireTruck_AdditionalKMCharges,Discount as AddSerBaseDiscount,
                                            0 AS Total_PT_Charge,0 AS Total_CL_Charge,0 AS Total_PEST_Charge,0 AS TotalAddServiceDiscount,0 AS TotalAddServiceCharge,
                                            0 as AddSerBaseDiscount,0 as Total_PT_Discount,0 as Total_CL_Discount,0 as Total_PEST_Discount,0 as Total_cost_without_discount                                               
                                           FROM SizeTypeMatrix 
                                           INNER JOIN SizeTypeMst ON SizeTypeMatrix.SizeTypeCode = SizeTypeMst.SizeTypeCode 
                                           LEFT OUTER JOIN TruckTypeMst ON SizeTypeMatrix.truck_type_code = TruckTypeMst.truck_type_code 
                                           LEFT OUTER JOIN DriverTypeMst ON SizeTypeMatrix.driver_type_code = DriverTypeMst.driver_type_code 
                                           LEFT OUTER JOIN LabourTypeMst ON SizeTypeMatrix.labour_type_code = LabourTypeMst.labour_type_code 
                                           LEFT OUTER JOIN HandimanTypeMst ON SizeTypeMatrix.Handiman_type_code = HandimanTypeMst.Handiman_type_code 
                                           LEFT OUTER JOIN SupervisorTypeMst ON SizeTypeMatrix.supervisor_type_code = SupervisorTypeMst.Supervisor_type_code 
                                           LEFT OUTER JOIN PackingTypeMst ON SizeTypeMatrix.Packing_type_code = PackingTypeMst.Packing_type_code 
                                           CROSS JOIN DistanceTypeMst 
                                           WHERE  SizeTypeMatrix.SizeTypeCode = @SizeTypeCode 
                                           AND   SizeTypeMatrix.rate_type_flag = @rate_type_flag 
                                           AND  DistanceTypeMst.distance_type_code = @distance_type_code ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@TotalDistance", DbType.Decimal, TotalDistance));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@distance_type_code", DbType.String, TotalDistanceUOM));
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


        private DataTable GetTruckTypeDistanceMatrix(String SizeTypeCode, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM TruckTypeDistanceMatrix 
                                   WHERE  SizeTypeCode = @SizeTypeCode 
                                   ORDER BY SizeTypeCode, FromKM ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "TruckTypeDistanceMatrix");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Truck Type Distance Matrix detail data retrieved successfully for SizeTypeCode : " + SizeTypeCode;
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Truck Type Distance Matrix detail data is not found for SizeTypeCode : " + SizeTypeCode;
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

        public Byte GetTruckRate(String truck_type_code, String goods_type_flag, String rate_type_flag, int NoOfTruck, Decimal NoOfDay, DateTime OrderDate, ref Decimal MinDistanceInKM, ref Decimal MinDistanceRate, ref Decimal PerUnitAboveMinDistanceInKM, ref Decimal RatePerUnitAboveMinDistance, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM TruckRateDetail 
                                   WHERE  truck_type_code = @truck_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@truck_type_code", DbType.String, truck_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "TruckRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Truck Rate detail data retrieved successfully for truck_type_code : " + truck_type_code;
                    MinDistanceInKM = Convert.ToDecimal(ds.Tables[0].Rows[0]["MinDistanceInKM"]);
                    MinDistanceRate = Convert.ToDecimal(ds.Tables[0].Rows[0]["MinDistanceRate"]);
                    PerUnitAboveMinDistanceInKM = Convert.ToDecimal(ds.Tables[0].Rows[0]["PerUnitAboveMinDistanceInKM"]);
                    RatePerUnitAboveMinDistance = Convert.ToDecimal(ds.Tables[0].Rows[0]["RatePerUnitAboveMinDistance"]);
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
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@truck_type_code", DbType.String, truck_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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

        public Decimal GetDriverRate(String driver_type_code, String goods_type_flag, String rate_type_flag, int NoOfDriver, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerDriver, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DriverRateDetail 
                                   WHERE  driver_type_code = @driver_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@driver_type_code", DbType.String, driver_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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

        public Decimal GetLabourRate(String labour_type_code, String goods_type_flag, String rate_type_flag, int NoOfLabour, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerLabour, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM LabourRateDetail 
                                   WHERE  labour_type_code = @labour_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@labour_type_code", DbType.String, labour_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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

        public Decimal GetHandimanRate(String handiman_type_code, String goods_type_flag, String rate_type_flag, int NoOfHandiman, Decimal NoOfDay, DateTime OrderDate, ref Decimal RatePerHandiman, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM HandimanRateDetail 
                                   WHERE  handiman_type_code = @handiman_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@handiman_type_code", DbType.String, handiman_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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

        public Decimal GetSupervisorRate(String Supervisor_type_code, String goods_type_flag, String rate_type_flag, int NoOftruck, int NoOfDay, DateTime OrderDate, ref Decimal RatePerHelper, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM SupervisorRateDetail 
                                    WHERE  Supervisor_type_code = @Supervisor_type_code 
                                    AND    goods_type_flag = @goods_type_flag 
                                    AND    rate_type_flag = @rate_type_flag 
                                    AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Supervisor_type_code", DbType.String, Supervisor_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "SupervisorRateDetail");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Supervisor Rate detail data retrieved successfully for Supervisor_type_code : " + Supervisor_type_code;
                    RatePerHelper = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    return RatePerHelper * NoOftruck;
                }
                else
                {
                    Message = "Supervisor Rate detail data is not found for Supervisor_type_code : " + Supervisor_type_code;
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

        public Decimal GetDistanceRate(String distance_type_code, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, int NoOfTruck, DateTime OrderDate, ref Decimal RatePerDistanceUOM, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM DistanceRateDetail 
                                   WHERE  distance_type_code = @distance_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@distance_type_code", DbType.String, distance_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@FromUOM", DbType.String, FromUOM));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ToUOM", DbType.String, ToUOM));
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

        public Decimal GetPackingRate(String Packing_type_code, String goods_type_flag, String rate_type_flag, DateTime OrderDate, ref Decimal RatePerSizeTypeCode, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM PackingRateDetail 
                                   WHERE  Packing_type_code = @Packing_type_code 
                                   AND    goods_type_flag = @goods_type_flag 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    (start_date <= @OrderDate) AND (end_date IS NULL OR end_date >= @OrderDate)");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Packing_type_code", DbType.String, Packing_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
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

        public DataTable CalculateRate(DataTable dtPostOrderParameter, String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinute, String IncludePackingCharge, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, int? NoOfHandiman, int? NoOfSupervisor, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                //DateTime OrderDate = DateTime.Today;

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
                Decimal FullDayRule = 0.6M, WorkingHours = 12.0M;
                DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                drArray = dtParameter.Select("Code='WorkingHours'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                //if (TotalDistance > 25 && SizeTypeCode == "SZ0002" && rate_type_flag == Constant.RATE_TYPE_FLAG_STANDERD)
                //{
                //    rate_type_flag = "BV";
                //}


                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {
                    if (rate_type_flag == Constant.RateTypePremium)
                    {
                        Decimal TrukkerMarginPremium = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMarginPremium"]);
                        Decimal CBM_Min = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["CBM_Min"]);
                        Decimal CBM_Max = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["CBM_Max"]);
                        Decimal ManDaysByCBM_Min = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["ManDaysByCBM_Min"]);
                        Decimal ManDaysByCBM_Max = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["ManDaysByCBM_Max"]);
                        Decimal RatePerCBM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerCBM"]);
                        Decimal DiffCityTruckAdditionalCharge = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["DiffCityTruckAdditionalCharge"]);
                        bool isCity = true;

                        String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                        if (!NoOfTruck.HasValue)
                            NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);
                        String MinDistanceUOM = Constant.DistanceUOM_KM, PerUnitAboveMinDistanceUOM = Constant.DistanceUOM_KM; Decimal TotalTravelingRate = 0;
                        Decimal MinDistanceInKM = 0, MinDistanceRate = 0, PerUnitAboveMinDistanceInKM = 0, RatePerUnitAboveMinDistance = 0; Decimal TotalRateAboveMinDistance = 0;
                        Byte result = GetTruckRate(truck_type_code, goods_type_flag, rate_type_flag, NoOfTruck.Value, NoOfDay, OrderDate, ref MinDistanceInKM, ref MinDistanceRate, ref PerUnitAboveMinDistanceInKM, ref RatePerUnitAboveMinDistance, ref Message);
                        if (result != 1)
                            return null;
                        else
                        {

                            if (dtPostOrderParameter != null)
                            {
                                isCity = dtPostOrderParameter.Select("inquiry_source_city='DUBAI' or inquiry_source_city='SHARJAH' or inquiry_source_city='Mina Jebel Ali'   ").Any();
                            }

                            if (MinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                            {
                                Decimal ConversionFactor = GetConversionFactor(MinDistanceUOM, TotalDistanceUOM, ref Message);
                                if (ConversionFactor < 0)
                                    return null;
                                else
                                {
                                    MinDistanceInKM = MinDistanceInKM * ConversionFactor;
                                    MinDistanceUOM = TotalDistanceUOM;
                                }
                            }

                            if (PerUnitAboveMinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                            {
                                Decimal ConversionFactor = GetConversionFactor(PerUnitAboveMinDistanceUOM, TotalDistanceUOM, ref Message);
                                if (ConversionFactor < 0)
                                    return null;
                                else
                                {
                                    PerUnitAboveMinDistanceInKM = PerUnitAboveMinDistanceInKM * ConversionFactor;
                                    PerUnitAboveMinDistanceUOM = TotalDistanceUOM;
                                }
                            }


                            //if (TotalDistance > MinDistanceInKM)
                            //{
                            //    Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                            //    Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                            //    MinDistanceRate = CBM_Max * RatePerCBM;
                            //    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                            //}
                            //else
                            //    MinDistanceRate = CBM_Max * RatePerCBM;


                            if (TotalDistance > MinDistanceInKM)
                            {
                                Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                                Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                                MinDistanceRate = CBM_Max * RatePerCBM;
                                if (isCity == false)
                                {
                                    ParameterMst objParameterMstTruckRate = new ParameterMst();
                                    DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("RatePerUnitAboveMinDistance", null, ref Message);
                                    if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                        return null;

                                    DataRow[] drRateAboveMinDist = dtParametertrkRate.Select("Code='RatePerUnitAboveMinDistance'");
                                    if (drRateAboveMinDist != null && drRateAboveMinDist.Length > 0 && drRateAboveMinDist[0]["Value"].ToString().Trim() != String.Empty)
                                        RatePerUnitAboveMinDistance = Convert.ToDecimal(drRateAboveMinDist[0]["Value"]);

                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                }
                                else
                                {
                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                }
                            }
                            else
                                MinDistanceRate = CBM_Max * RatePerCBM;


                        }

                        Decimal TotalPaintingCharge = 0; Decimal TotalCleaningCharge = 0; Decimal TotalPestControlCharge = 0;
                        Decimal TotalPaintingDiscount = 0; Decimal TotalCleaningDiscount = 0; Decimal TotalPestControlDiscount = 0;
                        Decimal PaintingCharge = 0; Decimal CleaningCharge = 0; Decimal PestControlCharge = 0;
                        Decimal PaintingDiscount = 0; Decimal CleaningDiscount = 0; Decimal PestControlDiscount = 0;
                        Decimal AddSerBaseDiscount = 0;


                        if (dtPostOrderParameter != null)
                        {
                            if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                            {
                                if (dtPostOrderParameter.Columns.Contains("AddonServices"))
                                {
                                    string[] strservices = dtPostOrderParameter.Rows[0]["AddonServices"].ToString().Split(',');
                                    for (int i = 0; i < strservices.Length; i++)
                                    {
                                        string[] strserviceSizetype = strservices[i].Split('^');
                                        if (strserviceSizetype[0].Trim() == Constant.PAINTING_SERIVCES)
                                        {
                                            DataTable dtPaintingservice = GetPaitingServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                            if (dtPaintingservice == null)
                                                return null;

                                            AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                            PaintingCharge = TotalPaintingCharge;
                                            PaintingDiscount = TotalPaintingDiscount;
                                        }

                                        if (strserviceSizetype[0].Trim() == Constant.CLEANING_SERIVCES)
                                        {
                                            DataTable dtCleaningService = GetCleaningServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                            if (dtCleaningService == null)
                                                return null;

                                            AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                            CleaningCharge = TotalCleaningCharge;
                                            CleaningDiscount = TotalCleaningDiscount;
                                        }

                                        if (strserviceSizetype[0].Trim() == Constant.PESTCONTROL_SERIVCES)
                                        {
                                            DataTable dtPestControlservice = GetPestControlServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                            if (dtPestControlservice == null)
                                                return null;

                                            AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());

                                            PestControlCharge = TotalPestControlCharge;
                                            PestControlDiscount = TotalPestControlDiscount;

                                        }
                                    }

                                    if (TotalPaintingCharge == 0)
                                    {
                                        DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                        if (dtPaintingservice == null)
                                            return null;
                                    }

                                    if (TotalCleaningCharge == 0)
                                    {
                                        DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                        if (dtCleaningService == null)
                                            return null;
                                    }

                                    if (TotalPestControlCharge == 0)
                                    {
                                        DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                        if (dtPestControlservice == null)
                                            return null;
                                    }
                                }
                            }
                            else
                            {
                                DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                if (dtPaintingservice == null)
                                    return null;

                                DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                if (dtCleaningService == null)
                                    return null;

                                DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                if (dtPestControlservice == null)
                                    return null;
                            }
                        }


                        Decimal TotalAddServiceCharge = 0;
                        TotalAddServiceCharge = PaintingCharge + CleaningCharge + PestControlCharge;
                        Decimal TotalAddServiceDiscount = 0;
                        TotalAddServiceDiscount = AddSerBaseDiscount + PaintingDiscount + CleaningDiscount + PestControlDiscount;

                        if (TotalAddServiceDiscount > 0)
                        {
                            TotalAddServiceCharge = TotalAddServiceCharge - TotalAddServiceDiscount;
                        }
                        Decimal TotalRate = MinDistanceRate;
                        Decimal TotalRateIncludeMargin = TotalRate + (TotalRate * (TrukkerMarginPremium / 100));

                        Decimal Discount = 0;
                        Decimal NetRate = TotalRateIncludeMargin - Discount;

                        if (TotalDistance > MinDistanceInKM)
                            TotalRateIncludeMargin = TotalRateIncludeMargin + TotalRateAboveMinDistance;

                        if (isCity == false)
                        {
                            TotalRateIncludeMargin = TotalRateIncludeMargin + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                        }

                        dtSizeTypeMst.Rows[0]["Total_cost"] = TotalRateIncludeMargin + TotalAddServiceCharge;

                        dtSizeTypeMst.Rows[0]["Total_cost_IncludeMargin"] = TotalRateIncludeMargin;

                        dtSizeTypeMst.Rows[0]["Discount"] = Discount;

                        dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;

                        int totallaborWithDriver = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString()) + Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString());
                        dtSizeTypeMst.Rows[0]["NoOfLabour"] = totallaborWithDriver;

                        dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"] = AddSerBaseDiscount;
                        dtSizeTypeMst.Rows[0]["Total_PT_Charge"] = TotalPaintingCharge;
                        dtSizeTypeMst.Rows[0]["Total_PT_Discount"] = TotalPaintingDiscount;
                        dtSizeTypeMst.Rows[0]["Total_CL_Charge"] = TotalCleaningCharge;
                        dtSizeTypeMst.Rows[0]["Total_CL_Discount"] = TotalCleaningDiscount;
                        dtSizeTypeMst.Rows[0]["Total_PEST_Charge"] = TotalPestControlCharge;
                        dtSizeTypeMst.Rows[0]["Total_PEST_Discount"] = TotalPestControlDiscount;
                        dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"] = TotalAddServiceDiscount;
                        dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"] = TotalAddServiceCharge;

                        if (dtSizeTypeMst.Columns.Contains("Total_cost_without_addon"))
                            dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRateIncludeMargin;
                        else
                        {
                            dtSizeTypeMst.Columns.Add("Total_cost_without_addon");
                            dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRateIncludeMargin;
                        }


                        return dtSizeTypeMst;
                    }
                    else
                    {
                        Decimal TotalRateAboveMinDistance = 0;
                        Decimal TrukkerMarginStandard = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMarginStandard"]);
                        Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
                        Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
                        Decimal DiffCityTruckAdditionalCharge = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["DiffCityTruckAdditionalCharge"]);
                        Decimal TotalTimeForPostOrder = TimeForLoadingInMinute + TimeToTravelInMinute + TimeForUnloadingInMinute;
                        Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                        Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                        if (IncompleteDay > 0)
                        {
                            if (IncompleteDay < FullDayRule)
                                NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                            else
                                NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
                        }
                        else
                            NoOfDay = Math.Floor(NoCompleteDay);

                        //if (IncompleteDay < FullDayRule)
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                        //else
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                        //String Packing_type_code = dtSizeTypeMst.Rows[0]["Packing_type_code"].ToString();
                        bool isCity = true;
                        if (dtPostOrderParameter != null)
                        {
                            isCity = dtPostOrderParameter.Select("inquiry_source_city='DUBAI' or inquiry_source_city='SHARJAH' or inquiry_source_city='Mina Jebel Ali' ").Any();
                        }

                        //Truck Rate
                        String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                        if (!NoOfTruck.HasValue)
                            NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);
                        String MinDistanceUOM = Constant.DistanceUOM_KM, PerUnitAboveMinDistanceUOM = Constant.DistanceUOM_KM;
                        Decimal MinDistanceInKM = 0, MinDistanceRate = 0, PerUnitAboveMinDistanceInKM = 0, RatePerUnitAboveMinDistance = 0;
                        Byte result = GetTruckRate(truck_type_code, goods_type_flag, rate_type_flag, NoOfTruck.Value, NoOfDay, OrderDate, ref MinDistanceInKM, ref MinDistanceRate, ref PerUnitAboveMinDistanceInKM, ref RatePerUnitAboveMinDistance, ref Message);
                        if (result != 1)
                            return null;
                        else
                        {


                            //Driver Rate
                            String driver_type_code = dtSizeTypeMst.Rows[0]["driver_type_code"].ToString();
                            if (!NoOfDriver.HasValue)
                                NoOfDriver = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfDriver"]);
                            Decimal RatePerDriver = -1, TotalDriverRate = GetDriverRate(driver_type_code, goods_type_flag, rate_type_flag, NoOfDriver.Value, NoOfDay, OrderDate, ref RatePerDriver, ref Message);
                            if (TotalDriverRate < 0)
                                return null;
                            else
                            {
                                //Labour Rate
                                String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                                if (!NoOfLabour.HasValue)
                                    NoOfLabour = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"]);
                                Decimal RatePerLabour = -1, TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                                if (TotalLabourRate < 0)
                                    return null;
                                else
                                {
                                    //Handiman Rate
                                    String Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                                    if (!NoOfHandiman.HasValue)
                                        NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);
                                    Decimal RatePerHandiman = -1, TotalHandimanRate = GetHandimanRate(Handiman_type_code, goods_type_flag, rate_type_flag, NoOfHandiman.Value, NoOfDay, OrderDate, ref RatePerHandiman, ref Message);
                                    if (TotalHandimanRate < 0)
                                        return null;
                                    else
                                    {
                                        //Supervisor Rate
                                        String supervisor_type_code = dtSizeTypeMst.Rows[0]["supervisor_type_code"].ToString();
                                        if (!NoOfSupervisor.HasValue)
                                            NoOfSupervisor = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfSupervisor"]);
                                        Decimal RatePerSupervisor = -1, TotalSupervisorRate = GetSupervisorRate(supervisor_type_code, goods_type_flag, rate_type_flag, NoOfSupervisor.Value, NoOfTruck.Value, OrderDate, ref RatePerSupervisor, ref Message);
                                        if (TotalSupervisorRate < 0)
                                            return null;
                                        else
                                        {
                                            if (MinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                                            {
                                                Decimal ConversionFactor = GetConversionFactor(MinDistanceUOM, TotalDistanceUOM, ref Message);
                                                if (ConversionFactor < 0)
                                                    return null;
                                                else
                                                {
                                                    MinDistanceInKM = MinDistanceInKM * ConversionFactor;
                                                    MinDistanceUOM = TotalDistanceUOM;
                                                }
                                            }

                                            if (PerUnitAboveMinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                                            {
                                                Decimal ConversionFactor = GetConversionFactor(PerUnitAboveMinDistanceUOM, TotalDistanceUOM, ref Message);
                                                if (ConversionFactor < 0)
                                                    return null;
                                                else
                                                {
                                                    PerUnitAboveMinDistanceInKM = PerUnitAboveMinDistanceInKM * ConversionFactor;
                                                    PerUnitAboveMinDistanceUOM = TotalDistanceUOM;
                                                }
                                            }


                                            if (TotalDistance > MinDistanceInKM)
                                            {
                                                Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                                                Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                                                MinDistanceRate = MinDistanceRate * NoOfTruck.Value;
                                                if (isCity == false)
                                                {
                                                    ParameterMst objParameterMstTruckRate = new ParameterMst();
                                                    DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("RatePerUnitAboveMinDistance", null, ref Message);
                                                    if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                                        return null;

                                                    DataRow[] drRateAboveMinDist = dtParametertrkRate.Select("Code='RatePerUnitAboveMinDistance'");
                                                    if (drRateAboveMinDist != null && drRateAboveMinDist.Length > 0 && drRateAboveMinDist[0]["Value"].ToString().Trim() != String.Empty)
                                                        RatePerUnitAboveMinDistance = Convert.ToDecimal(drRateAboveMinDist[0]["Value"]);

                                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                                }
                                                else
                                                {
                                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                                }
                                            }
                                            else
                                                MinDistanceRate = MinDistanceRate * NoOfTruck.Value;

                                            String Packing_type_code = dtSizeTypeMst.Rows[0]["Packing_type_code"].ToString();
                                            Decimal TotalPackingRate = 0, RatePerSizeTypeCode = -1;

                                            TotalPackingRate = GetPackingRate(Packing_type_code, goods_type_flag, rate_type_flag, OrderDate, ref RatePerSizeTypeCode, ref Message);
                                            if (TotalPackingRate < 0)
                                                return null;


                                            Decimal TotalPaintingCharge = 0; Decimal TotalCleaningCharge = 0; Decimal TotalPestControlCharge = 0;
                                            Decimal TotalPaintingDiscount = 0; Decimal TotalCleaningDiscount = 0; Decimal TotalPestControlDiscount = 0;
                                            Decimal PaintingCharge = 0; Decimal CleaningCharge = 0; Decimal PestControlCharge = 0;
                                            Decimal PaintingDiscount = 0; Decimal CleaningDiscount = 0; Decimal PestControlDiscount = 0;
                                            Decimal AddSerBaseDiscount = 0;



                                            if (dtPostOrderParameter != null)
                                            {
                                                if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                                {
                                                    if (dtPostOrderParameter.Columns.Contains("AddonServices"))
                                                    {
                                                        string[] strservices = dtPostOrderParameter.Rows[0]["AddonServices"].ToString().Split(',');
                                                        for (int i = 0; i < strservices.Length; i++)
                                                        {
                                                            string[] strserviceSizetype = strservices[i].Split('^');
                                                            if (strserviceSizetype[0].Trim() == Constant.PAINTING_SERIVCES)
                                                            {
                                                                DataTable dtPaintingservice = GetPaitingServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                                                if (dtPaintingservice == null)
                                                                    return null;

                                                                AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                                                PaintingCharge = TotalPaintingCharge;
                                                                PaintingDiscount = TotalPaintingDiscount;
                                                            }

                                                            if (strserviceSizetype[0].Trim() == Constant.CLEANING_SERIVCES)
                                                            {
                                                                DataTable dtCleaningService = GetCleaningServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                                                if (dtCleaningService == null)
                                                                    return null;

                                                                AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                                                CleaningCharge = TotalCleaningCharge;
                                                                CleaningDiscount = TotalCleaningDiscount;
                                                            }

                                                            if (strserviceSizetype[0].Trim() == Constant.PESTCONTROL_SERIVCES)
                                                            {
                                                                DataTable dtPestControlservice = GetPestControlServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                                                if (dtPestControlservice == null)
                                                                    return null;

                                                                AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());

                                                                PestControlCharge = TotalPestControlCharge;
                                                                PestControlDiscount = TotalPestControlDiscount;

                                                            }
                                                        }

                                                        if (TotalPaintingCharge == 0)
                                                        {
                                                            DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                                            if (dtPaintingservice == null)
                                                                return null;
                                                        }

                                                        if (TotalCleaningCharge == 0)
                                                        {
                                                            DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                                            if (dtCleaningService == null)
                                                                return null;
                                                        }

                                                        if (TotalPestControlCharge == 0)
                                                        {
                                                            DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                                            if (dtPestControlservice == null)
                                                                return null;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                                    if (dtPaintingservice == null)
                                                        return null;

                                                    DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                                    if (dtCleaningService == null)
                                                        return null;

                                                    DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                                    if (dtPestControlservice == null)
                                                        return null;
                                                }
                                            }

                                            Decimal TotalAddServiceCharge = 0; Decimal TotalAddServiceChargeWithoudiscount = 0;
                                            TotalAddServiceCharge = PaintingCharge + CleaningCharge + PestControlCharge;
                                            Decimal TotalAddServiceDiscount = 0;
                                            TotalAddServiceDiscount = AddSerBaseDiscount + PaintingDiscount + CleaningDiscount + PestControlDiscount;

                                            if (TotalAddServiceDiscount > 0)
                                            {
                                                TotalAddServiceCharge = TotalAddServiceCharge - TotalAddServiceDiscount;
                                            }


                                            Decimal Discount = 0; Decimal TotalRateIncludeDiscount = 0; Decimal TotalRateIncludeMargin = 0; Decimal TotalRate = 0;
                                            Decimal BaseRate = MinDistanceRate + TotalDriverRate + TotalSupervisorRate;

                                            //// add discount in sizetype studio
                                            //Discount = 20; TotalRateIncludeDiscount = 0;
                                            //if (SizeTypeCode.Trim() == "SZ0000")
                                            //{
                                            //    TotalRateIncludeDiscount = BaseRate - (BaseRate * (Discount / 100));
                                            //    BaseRate = TotalRateIncludeDiscount;
                                            //}

                                            TotalRateIncludeMargin = BaseRate + (BaseRate * (TrukkerMarginStandard / 100));
                                            BaseRate = TotalRateIncludeMargin + TotalRateAboveMinDistance;



                                            TotalRate = 0;
                                            if (IncludePackingCharge == "N")
                                                TotalRate = BaseRate + TotalLabourRate + TotalHandimanRate;
                                            else
                                                TotalRate = BaseRate + TotalLabourRate + TotalHandimanRate + TotalPackingRate;

                                            if (dtPostOrderParameter != null)
                                            {
                                                if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                                    TotalRate = TotalRate + TotalAddServiceCharge;
                                            }

                                            #region Truck Rate For City Not In Dubai & Sharjah


                                            //ParameterMst objParameterMstTruckRate = new ParameterMst();
                                            //DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("CityTruckRate", null, ref Message);
                                            //if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                            //    return null;

                                            if (isCity == false)
                                            {

                                                TotalRate = TotalRate + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                                                BaseRate = BaseRate + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                                                //if (dtParametertrkRate != null)
                                                //{
                                                //    Decimal minRate = 0;
                                                //    DataRow[] drArraytrkRate = dtParametertrkRate.Select("Code='CityTruckRate'");
                                                //    if (drArraytrkRate != null && drArraytrkRate.Length > 0 && drArraytrkRate[0]["Value"].ToString().Trim() != String.Empty)
                                                //        minRate = Convert.ToDecimal(drArraytrkRate[0]["Value"]);

                                                //    TotalRate = TotalRate + (minRate * NoOfTruck.Value);
                                                //}
                                            }

                                            #endregion

                                            Decimal NetRate = TotalRateIncludeMargin - Discount;

                                            dtSizeTypeMst.Rows[0]["goods_type_flag"] = goods_type_flag;
                                            dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;
                                            dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;
                                            //Driver Rate                                        

                                            dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerDriver"] = RatePerDriver;
                                            dtSizeTypeMst.Rows[0]["TotalDriverRate"] = TotalDriverRate;
                                            //Labour Rate
                                            dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour + NoOfDriver;
                                            dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;
                                            dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;
                                            //Handiman Rate
                                            dtSizeTypeMst.Rows[0]["NoOfHandiman"] = NoOfHandiman.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerHandiman"] = RatePerHandiman;
                                            dtSizeTypeMst.Rows[0]["TotalHandimanRate"] = TotalHandimanRate;
                                            //Supervisor Rate
                                            dtSizeTypeMst.Rows[0]["NoOfSupervisor"] = NoOfSupervisor.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerSupervisor"] = RatePerSupervisor;
                                            dtSizeTypeMst.Rows[0]["TotalSupervisorRate"] = TotalSupervisorRate;

                                            //PackingRate
                                            //if (IncludePackingCharge == "Y")
                                            dtSizeTypeMst.Rows[0]["TotalPackingRate"] = TotalPackingRate;
                                            //else
                                            //    dtSizeTypeMst.Rows[0]["TotalPackingRate"] = 0;
                                            //Traveling Rate
                                            dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck.ToString();
                                            dtSizeTypeMst.Rows[0]["MinDistance"] = MinDistanceInKM;
                                            dtSizeTypeMst.Rows[0]["MinDistanceUOM"] = MinDistanceUOM;
                                            dtSizeTypeMst.Rows[0]["MinDistanceRate"] = MinDistanceRate;
                                            dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
                                            dtSizeTypeMst.Rows[0]["RatePerDistanceUOM"] = RatePerUnitAboveMinDistance;
                                            dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = MinDistanceRate;
                                            dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinute;
                                            dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;

                                            dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;
                                            dtSizeTypeMst.Rows[0]["Total_cost"] = Math.Round(TotalRate);
                                            dtSizeTypeMst.Rows[0]["Total_cost_IncludeMargin"] = TotalRateIncludeMargin;
                                            dtSizeTypeMst.Rows[0]["Discount"] = Discount;
                                            dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;

                                            dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"] = AddSerBaseDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_PT_Charge"] = TotalPaintingCharge;
                                            dtSizeTypeMst.Rows[0]["Total_PT_Discount"] = TotalPaintingDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_CL_Charge"] = TotalCleaningCharge;
                                            dtSizeTypeMst.Rows[0]["Total_CL_Discount"] = TotalCleaningDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_PEST_Charge"] = TotalPestControlCharge;
                                            dtSizeTypeMst.Rows[0]["Total_PEST_Discount"] = TotalPestControlDiscount;
                                            dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"] = TotalAddServiceDiscount;
                                            dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"] = TotalAddServiceCharge + TotalAddServiceDiscount;

                                            if (dtSizeTypeMst.Columns.Contains("Total_cost_without_addon"))
                                                dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRate - TotalAddServiceCharge;
                                            else
                                            {
                                                dtSizeTypeMst.Columns.Add("Total_cost_without_addon");
                                                dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRate - TotalAddServiceCharge; ;
                                            }

                                            return dtSizeTypeMst;
                                            //}
                                        }
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

        public DataTable CalculateRateHome_dubizzle(DataTable dtPostOrderParameter, String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinute, String IncludePackingCharge, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, int? NoOfHandiman, int? NoOfSupervisor, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                //DateTime OrderDate = DateTime.Today;

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
                Decimal FullDayRule = 0.6M, WorkingHours = 12.0M;
                DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                drArray = dtParameter.Select("Code='WorkingHours'");
                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                //if (TotalDistance > 25 && SizeTypeCode == "SZ0002" && rate_type_flag == Constant.RATE_TYPE_FLAG_STANDERD)
                //{
                //    rate_type_flag = "BV";
                //}


                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {
                    if (rate_type_flag == Constant.RateTypePremium)
                    {
                        Decimal TrukkerMarginPremium = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMarginPremium"]);
                        Decimal CBM_Min = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["CBM_Min"]);
                        Decimal CBM_Max = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["CBM_Max"]);
                        Decimal ManDaysByCBM_Min = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["ManDaysByCBM_Min"]);
                        Decimal ManDaysByCBM_Max = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["ManDaysByCBM_Max"]);
                        Decimal RatePerCBM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerCBM"]);
                        Decimal DiffCityTruckAdditionalCharge = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["DiffCityTruckAdditionalCharge"]);
                        bool isCity = true;

                        String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                        if (!NoOfTruck.HasValue)
                            NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);
                        String MinDistanceUOM = Constant.DistanceUOM_KM, PerUnitAboveMinDistanceUOM = Constant.DistanceUOM_KM; Decimal TotalTravelingRate = 0;
                        Decimal MinDistanceInKM = 0, MinDistanceRate = 0, PerUnitAboveMinDistanceInKM = 0, RatePerUnitAboveMinDistance = 0; Decimal TotalRateAboveMinDistance = 0;
                        Byte result = GetTruckRate(truck_type_code, goods_type_flag, rate_type_flag, NoOfTruck.Value, NoOfDay, OrderDate, ref MinDistanceInKM, ref MinDistanceRate, ref PerUnitAboveMinDistanceInKM, ref RatePerUnitAboveMinDistance, ref Message);
                        if (result != 1)
                            return null;
                        else
                        {

                            if (dtPostOrderParameter != null)
                            {
                                isCity = dtPostOrderParameter.Select("inquiry_source_city='DUBAI' or inquiry_source_city='SHARJAH' ").Any();
                            }

                            if (MinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                            {
                                Decimal ConversionFactor = GetConversionFactor(MinDistanceUOM, TotalDistanceUOM, ref Message);
                                if (ConversionFactor < 0)
                                    return null;
                                else
                                {
                                    MinDistanceInKM = MinDistanceInKM * ConversionFactor;
                                    MinDistanceUOM = TotalDistanceUOM;
                                }
                            }

                            if (PerUnitAboveMinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                            {
                                Decimal ConversionFactor = GetConversionFactor(PerUnitAboveMinDistanceUOM, TotalDistanceUOM, ref Message);
                                if (ConversionFactor < 0)
                                    return null;
                                else
                                {
                                    PerUnitAboveMinDistanceInKM = PerUnitAboveMinDistanceInKM * ConversionFactor;
                                    PerUnitAboveMinDistanceUOM = TotalDistanceUOM;
                                }
                            }


                            //if (TotalDistance > MinDistanceInKM)
                            //{
                            //    Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                            //    Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                            //    MinDistanceRate = CBM_Max * RatePerCBM;
                            //    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                            //}
                            //else
                            //    MinDistanceRate = CBM_Max * RatePerCBM;


                            if (TotalDistance > MinDistanceInKM)
                            {
                                Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                                Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                                MinDistanceRate = CBM_Max * RatePerCBM;
                                if (isCity == false)
                                {
                                    ParameterMst objParameterMstTruckRate = new ParameterMst();
                                    DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("RatePerUnitAboveMinDistance", null, ref Message);
                                    if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                        return null;

                                    DataRow[] drRateAboveMinDist = dtParametertrkRate.Select("Code='RatePerUnitAboveMinDistance'");
                                    if (drRateAboveMinDist != null && drRateAboveMinDist.Length > 0 && drRateAboveMinDist[0]["Value"].ToString().Trim() != String.Empty)
                                        RatePerUnitAboveMinDistance = Convert.ToDecimal(drRateAboveMinDist[0]["Value"]);

                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                }
                                else
                                {
                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                }
                            }
                            else
                                MinDistanceRate = CBM_Max * RatePerCBM;


                        }

                        //Decimal TotalPaintingCharge = 0; Decimal TotalCleaningCharge = 0; Decimal TotalPestControlCharge = 0;
                        //Decimal TotalPaintingDiscount = 0; Decimal TotalCleaningDiscount = 0; Decimal TotalPestControlDiscount = 0;
                        //Decimal PaintingCharge = 0; Decimal CleaningCharge = 0; Decimal PestControlCharge = 0;
                        //Decimal PaintingDiscount = 0; Decimal CleaningDiscount = 0; Decimal PestControlDiscount = 0;
                        //Decimal AddSerBaseDiscount = 0;


                        //if (dtPostOrderParameter != null)
                        //{
                        //    if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                        //    {
                        //        if (dtPostOrderParameter.Columns.Contains("AddonServices"))
                        //        {
                        //            string[] strservices = dtPostOrderParameter.Rows[0]["AddonServices"].ToString().Split(',');
                        //            for (int i = 0; i < strservices.Length; i++)
                        //            {
                        //                string[] strserviceSizetype = strservices[i].Trim().Split('^');
                        //                if (strserviceSizetype[0].Trim() == Constant.PAINTING_SERIVCES)
                        //                {
                        //                    DataTable dtPaintingservice = GetPaitingServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                        //                    if (dtPaintingservice == null)
                        //                        return null;

                        //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                        //                    PaintingCharge = TotalPaintingCharge;
                        //                    PaintingDiscount = TotalPaintingDiscount;
                        //                }
                        //                else if (strserviceSizetype[0].Trim() == Constant.CLEANING_SERIVCES)
                        //                {
                        //                    DataTable dtCleaningService = GetCleaningServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                        //                    if (dtCleaningService == null)
                        //                        return null;

                        //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                        //                    CleaningCharge = TotalCleaningCharge;
                        //                    CleaningDiscount = TotalCleaningDiscount;
                        //                }
                        //                else if (strserviceSizetype[0].Trim() == Constant.PESTCONTROL_SERIVCES)
                        //                {
                        //                    DataTable dtPestControlservice = GetPestControlServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                        //                    if (dtPestControlservice == null)
                        //                        return null;

                        //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());

                        //                    PestControlCharge = TotalPestControlCharge;
                        //                    PestControlDiscount = TotalPestControlDiscount;

                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                        //    if (dtPaintingservice == null)
                        //        return null;

                        //    DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                        //    if (dtCleaningService == null)
                        //        return null;

                        //    DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                        //    if (dtPestControlservice == null)
                        //        return null;
                        //}

                        //Decimal TotalAddServiceCharge = 0;
                        //TotalAddServiceCharge = PaintingCharge + CleaningCharge + PestControlCharge;
                        //Decimal TotalAddServiceDiscount = 0;
                        //TotalAddServiceDiscount = AddSerBaseDiscount + PaintingDiscount + CleaningDiscount + PestControlDiscount;

                        //if (TotalAddServiceDiscount > 0)
                        //{
                        //    TotalAddServiceCharge = TotalAddServiceCharge - TotalAddServiceDiscount;
                        //}

                        Decimal TotalRate = MinDistanceRate;
                        Decimal TotalRateIncludeMargin = TotalRate + (TotalRate * (TrukkerMarginPremium / 100));

                        Decimal Discount = 0;
                        Decimal NetRate = TotalRateIncludeMargin - Discount;

                        if (TotalDistance > MinDistanceInKM)
                            TotalRateIncludeMargin = TotalRateIncludeMargin + TotalRateAboveMinDistance;

                        if (isCity == false)
                        {
                            TotalRateIncludeMargin = TotalRateIncludeMargin + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                        }

                        dtSizeTypeMst.Rows[0]["Total_cost"] = TotalRateIncludeMargin; //+ TotalAddServiceCharge;

                        dtSizeTypeMst.Rows[0]["Total_cost_IncludeMargin"] = TotalRateIncludeMargin;

                        dtSizeTypeMst.Rows[0]["Discount"] = Discount;

                        dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;

                        int totallaborWithDriver = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfDriver"].ToString()) + Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"].ToString());
                        dtSizeTypeMst.Rows[0]["NoOfLabour"] = totallaborWithDriver;



                        dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"] = 0;// AddSerBaseDiscount;
                        dtSizeTypeMst.Rows[0]["Total_PT_Charge"] = 0;//TotalPaintingCharge;
                        dtSizeTypeMst.Rows[0]["Total_PT_Discount"] = 0;//TotalPaintingDiscount;
                        dtSizeTypeMst.Rows[0]["Total_CL_Charge"] = 0;// TotalCleaningCharge;
                        dtSizeTypeMst.Rows[0]["Total_CL_Discount"] = 0;//TotalCleaningDiscount;
                        dtSizeTypeMst.Rows[0]["Total_PEST_Charge"] = 0;//TotalPestControlCharge;
                        dtSizeTypeMst.Rows[0]["Total_PEST_Discount"] = 0;//TotalPestControlDiscount;
                        dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"] = 0;// TotalAddServiceDiscount;
                        dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"] = 0;//TotalAddServiceCharge;

                        if (dtSizeTypeMst.Columns.Contains("Total_cost_without_addon"))
                            dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRateIncludeMargin;
                        else
                        {
                            dtSizeTypeMst.Columns.Add("Total_cost_without_addon");
                            dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRateIncludeMargin;
                        }

                        return dtSizeTypeMst;
                    }
                    else
                    {
                        Decimal TotalRateAboveMinDistance = 0;
                        Decimal TrukkerMarginStandard = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMarginStandard"]);
                        Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
                        Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
                        Decimal DiffCityTruckAdditionalCharge = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["DiffCityTruckAdditionalCharge"]);
                        Decimal TotalTimeForPostOrder = TimeForLoadingInMinute + TimeToTravelInMinute + TimeForUnloadingInMinute;
                        Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                        Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                        if (IncompleteDay > 0)
                        {
                            if (IncompleteDay < FullDayRule)
                                NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                            else
                                NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
                        }
                        else
                            NoOfDay = Math.Floor(NoCompleteDay);

                        //if (IncompleteDay < FullDayRule)
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                        //else
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                        //String Packing_type_code = dtSizeTypeMst.Rows[0]["Packing_type_code"].ToString();
                        bool isCity = true;
                        if (dtPostOrderParameter != null)
                        {
                            isCity = dtPostOrderParameter.Select("inquiry_source_city='DUBAI' or inquiry_source_city='SHARJAH' ").Any();
                        }

                        //Truck Rate
                        String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                        if (!NoOfTruck.HasValue)
                            NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);
                        String MinDistanceUOM = Constant.DistanceUOM_KM, PerUnitAboveMinDistanceUOM = Constant.DistanceUOM_KM;
                        Decimal MinDistanceInKM = 0, MinDistanceRate = 0, PerUnitAboveMinDistanceInKM = 0, RatePerUnitAboveMinDistance = 0;
                        Byte result = GetTruckRate(truck_type_code, goods_type_flag, rate_type_flag, NoOfTruck.Value, NoOfDay, OrderDate, ref MinDistanceInKM, ref MinDistanceRate, ref PerUnitAboveMinDistanceInKM, ref RatePerUnitAboveMinDistance, ref Message);
                        if (result != 1)
                            return null;
                        else
                        {


                            //Driver Rate
                            String driver_type_code = dtSizeTypeMst.Rows[0]["driver_type_code"].ToString();
                            if (!NoOfDriver.HasValue)
                                NoOfDriver = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfDriver"]);
                            Decimal RatePerDriver = -1, TotalDriverRate = GetDriverRate(driver_type_code, goods_type_flag, rate_type_flag, NoOfDriver.Value, NoOfDay, OrderDate, ref RatePerDriver, ref Message);
                            if (TotalDriverRate < 0)
                                return null;
                            else
                            {
                                //Labour Rate
                                String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                                if (!NoOfLabour.HasValue)
                                    NoOfLabour = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"]);
                                Decimal RatePerLabour = -1, TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                                if (TotalLabourRate < 0)
                                    return null;
                                else
                                {
                                    //Handiman Rate
                                    String Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                                    if (!NoOfHandiman.HasValue)
                                        NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);
                                    Decimal RatePerHandiman = -1, TotalHandimanRate = GetHandimanRate(Handiman_type_code, goods_type_flag, rate_type_flag, NoOfHandiman.Value, NoOfDay, OrderDate, ref RatePerHandiman, ref Message);
                                    if (TotalHandimanRate < 0)
                                        return null;
                                    else
                                    {
                                        //Supervisor Rate
                                        String supervisor_type_code = dtSizeTypeMst.Rows[0]["supervisor_type_code"].ToString();
                                        if (!NoOfSupervisor.HasValue)
                                            NoOfSupervisor = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfSupervisor"]);
                                        Decimal RatePerSupervisor = -1, TotalSupervisorRate = GetSupervisorRate(supervisor_type_code, goods_type_flag, rate_type_flag, NoOfSupervisor.Value, NoOfTruck.Value, OrderDate, ref RatePerSupervisor, ref Message);
                                        if (TotalSupervisorRate < 0)
                                            return null;
                                        else
                                        {
                                            if (MinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                                            {
                                                Decimal ConversionFactor = GetConversionFactor(MinDistanceUOM, TotalDistanceUOM, ref Message);
                                                if (ConversionFactor < 0)
                                                    return null;
                                                else
                                                {
                                                    MinDistanceInKM = MinDistanceInKM * ConversionFactor;
                                                    MinDistanceUOM = TotalDistanceUOM;
                                                }
                                            }

                                            if (PerUnitAboveMinDistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                                            {
                                                Decimal ConversionFactor = GetConversionFactor(PerUnitAboveMinDistanceUOM, TotalDistanceUOM, ref Message);
                                                if (ConversionFactor < 0)
                                                    return null;
                                                else
                                                {
                                                    PerUnitAboveMinDistanceInKM = PerUnitAboveMinDistanceInKM * ConversionFactor;
                                                    PerUnitAboveMinDistanceUOM = TotalDistanceUOM;
                                                }
                                            }


                                            if (TotalDistance > MinDistanceInKM)
                                            {
                                                Decimal DistanceAboveMinDistance = TotalDistance - MinDistanceInKM;
                                                Decimal PerUnitInKM = DistanceAboveMinDistance / PerUnitAboveMinDistanceInKM;
                                                MinDistanceRate = MinDistanceRate * NoOfTruck.Value;
                                                if (isCity == false)
                                                {
                                                    ParameterMst objParameterMstTruckRate = new ParameterMst();
                                                    DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("RatePerUnitAboveMinDistance", null, ref Message);
                                                    if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                                        return null;

                                                    DataRow[] drRateAboveMinDist = dtParametertrkRate.Select("Code='RatePerUnitAboveMinDistance'");
                                                    if (drRateAboveMinDist != null && drRateAboveMinDist.Length > 0 && drRateAboveMinDist[0]["Value"].ToString().Trim() != String.Empty)
                                                        RatePerUnitAboveMinDistance = Convert.ToDecimal(drRateAboveMinDist[0]["Value"]);

                                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                                }
                                                else
                                                {
                                                    TotalRateAboveMinDistance = PerUnitInKM * RatePerUnitAboveMinDistance * NoOfTruck.Value;
                                                }
                                            }
                                            else
                                                MinDistanceRate = MinDistanceRate * NoOfTruck.Value;

                                            String Packing_type_code = dtSizeTypeMst.Rows[0]["Packing_type_code"].ToString();
                                            Decimal TotalPackingRate = 0, RatePerSizeTypeCode = -1;

                                            TotalPackingRate = GetPackingRate(Packing_type_code, goods_type_flag, rate_type_flag, OrderDate, ref RatePerSizeTypeCode, ref Message);
                                            if (TotalPackingRate < 0)
                                                return null;

                                            //Decimal TotalPaintingCharge = 0; Decimal TotalCleaningCharge = 0; Decimal TotalPestControlCharge = 0;
                                            //Decimal TotalPaintingDiscount = 0; Decimal TotalCleaningDiscount = 0; Decimal TotalPestControlDiscount = 0;
                                            //Decimal PaintingCharge = 0; Decimal CleaningCharge = 0; Decimal PestControlCharge = 0;
                                            //Decimal PaintingDiscount = 0; Decimal CleaningDiscount = 0; Decimal PestControlDiscount = 0;
                                            //Decimal AddSerBaseDiscount = 0;



                                            //if (dtPostOrderParameter != null)
                                            //{
                                            //    if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                            //    {
                                            //        if (dtPostOrderParameter.Columns.Contains("AddonServices"))
                                            //        {
                                            //            string[] strservices = dtPostOrderParameter.Rows[0]["AddonServices"].ToString().Split(',');
                                            //            for (int i = 0; i < strservices.Length; i++)
                                            //            {
                                            //                string[] strserviceSizetype = strservices[i].Trim().Split('^');
                                            //                if (strserviceSizetype[0].Trim() == Constant.PAINTING_SERIVCES)
                                            //                {
                                            //                    DataTable dtPaintingservice = GetPaitingServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                            //                    if (dtPaintingservice == null)
                                            //                        return null;

                                            //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                            //                    PaintingCharge = TotalPaintingCharge;
                                            //                    PaintingDiscount = TotalPaintingDiscount;
                                            //                }

                                            //                if (strserviceSizetype[0].Trim() == Constant.CLEANING_SERIVCES)
                                            //                {
                                            //                    DataTable dtCleaningService = GetCleaningServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                            //                    if (dtCleaningService == null)
                                            //                        return null;

                                            //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());
                                            //                    CleaningCharge = TotalCleaningCharge;
                                            //                    CleaningDiscount = TotalCleaningDiscount;
                                            //                }

                                            //                if (strserviceSizetype[0].Trim() == Constant.PESTCONTROL_SERIVCES)
                                            //                {
                                            //                    DataTable dtPestControlservice = GetPestControlServiceRate(strserviceSizetype[0].Trim(), strserviceSizetype[1].Trim(), goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                            //                    if (dtPestControlservice == null)
                                            //                        return null;

                                            //                    AddSerBaseDiscount = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"].ToString());

                                            //                    PestControlCharge = TotalPestControlCharge;
                                            //                    PestControlDiscount = TotalPestControlDiscount;

                                            //                }
                                            //            }

                                            //            if (TotalPaintingCharge == 0)
                                            //            {
                                            //                DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                            //                if (dtPaintingservice == null)
                                            //                    return null;
                                            //            }

                                            //            if (TotalCleaningCharge == 0)
                                            //            {
                                            //                DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                            //                if (dtCleaningService == null)
                                            //                    return null;
                                            //            }

                                            //            if (TotalPestControlCharge == 0)
                                            //            {
                                            //                DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                            //                if (dtPestControlservice == null)
                                            //                    return null;
                                            //            }
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        DataTable dtPaintingservice = GetPaitingServiceRate(Constant.PAINTING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPaintingCharge, ref TotalPaintingDiscount, ref Message);
                                            //        if (dtPaintingservice == null)
                                            //            return null;

                                            //        DataTable dtCleaningService = GetCleaningServiceRate(Constant.CLEANING_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalCleaningCharge, ref TotalCleaningDiscount, ref Message);
                                            //        if (dtCleaningService == null)
                                            //            return null;

                                            //        DataTable dtPestControlservice = GetPestControlServiceRate(Constant.PESTCONTROL_SERIVCES, SizeTypeCode, goods_type_flag, rate_type_flag, OrderDate, ref TotalPestControlCharge, ref TotalPestControlDiscount, ref Message);
                                            //        if (dtPestControlservice == null)
                                            //            return null;
                                            //    }
                                            //}

                                            Decimal TotalAddServiceCharge = 0; Decimal TotalAddServiceChargeWithoudiscount = 0;
                                            //TotalAddServiceCharge = PaintingCharge + CleaningCharge + PestControlCharge;
                                            //Decimal TotalAddServiceDiscount = 0;
                                            //TotalAddServiceDiscount = AddSerBaseDiscount + PaintingDiscount + CleaningDiscount + PestControlDiscount;

                                            //if (TotalAddServiceDiscount > 0)
                                            //{
                                            //    TotalAddServiceCharge = TotalAddServiceCharge - TotalAddServiceDiscount;
                                            //}


                                            Decimal Discount = 0; Decimal TotalRateIncludeDiscount = 0; Decimal TotalRateIncludeMargin = 0; Decimal TotalRate = 0;
                                            Decimal BaseRate = MinDistanceRate + TotalDriverRate + TotalSupervisorRate;

                                            //// add discount in sizetype studio
                                            //Discount = 20; TotalRateIncludeDiscount = 0;
                                            //if (SizeTypeCode.Trim() == "SZ0000")
                                            //{
                                            //    TotalRateIncludeDiscount = BaseRate - (BaseRate * (Discount / 100));
                                            //    BaseRate = TotalRateIncludeDiscount;
                                            //}

                                            TotalRateIncludeMargin = BaseRate + (BaseRate * (TrukkerMarginStandard / 100));
                                            BaseRate = TotalRateIncludeMargin + TotalRateAboveMinDistance;



                                            TotalRate = 0;
                                            if (IncludePackingCharge == "N")
                                                TotalRate = BaseRate + TotalLabourRate + TotalHandimanRate + TotalPackingRate;
                                            else
                                                TotalRate = BaseRate + TotalLabourRate + TotalHandimanRate + TotalPackingRate;

                                            //if (dtPostOrderParameter != null)
                                            //{
                                            //    if (dtPostOrderParameter.Rows[0]["IncludeAddonService"].ToString() == Constant.FLAG_Y)
                                            //        TotalRate = TotalRate;// +TotalAddServiceCharge;
                                            //}

                                            #region Truck Rate For City Not In Dubai & Sharjah


                                            //ParameterMst objParameterMstTruckRate = new ParameterMst();
                                            //DataTable dtParametertrkRate = objParameterMstTruckRate.GetParameter("CityTruckRate", null, ref Message);
                                            //if (dtParametertrkRate == null || dtParametertrkRate.Rows.Count <= 0)
                                            //    return null;

                                            if (isCity == false)
                                            {

                                                TotalRate = TotalRate + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                                                BaseRate = BaseRate + (DiffCityTruckAdditionalCharge * NoOfTruck.Value);
                                                //if (dtParametertrkRate != null)
                                                //{
                                                //    Decimal minRate = 0;
                                                //    DataRow[] drArraytrkRate = dtParametertrkRate.Select("Code='CityTruckRate'");
                                                //    if (drArraytrkRate != null && drArraytrkRate.Length > 0 && drArraytrkRate[0]["Value"].ToString().Trim() != String.Empty)
                                                //        minRate = Convert.ToDecimal(drArraytrkRate[0]["Value"]);

                                                //    TotalRate = TotalRate + (minRate * NoOfTruck.Value);
                                                //}
                                            }

                                            #endregion

                                            Decimal NetRate = TotalRateIncludeMargin - Discount;

                                            dtSizeTypeMst.Rows[0]["goods_type_flag"] = goods_type_flag;
                                            dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;
                                            dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;
                                            //Driver Rate                                        

                                            dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerDriver"] = RatePerDriver;
                                            dtSizeTypeMst.Rows[0]["TotalDriverRate"] = TotalDriverRate;
                                            //Labour Rate
                                            dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour + NoOfDriver;
                                            dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;
                                            dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;
                                            //Handiman Rate
                                            dtSizeTypeMst.Rows[0]["NoOfHandiman"] = NoOfHandiman.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerHandiman"] = RatePerHandiman;
                                            dtSizeTypeMst.Rows[0]["TotalHandimanRate"] = TotalHandimanRate;
                                            //Supervisor Rate
                                            dtSizeTypeMst.Rows[0]["NoOfSupervisor"] = NoOfSupervisor.ToString();
                                            dtSizeTypeMst.Rows[0]["RatePerSupervisor"] = RatePerSupervisor;
                                            dtSizeTypeMst.Rows[0]["TotalSupervisorRate"] = TotalSupervisorRate;

                                            //PackingRate
                                            //if (IncludePackingCharge == "Y")
                                            dtSizeTypeMst.Rows[0]["TotalPackingRate"] = TotalPackingRate;
                                            //else
                                            //    dtSizeTypeMst.Rows[0]["TotalPackingRate"] = 0;
                                            //Traveling Rate
                                            dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck.ToString();
                                            dtSizeTypeMst.Rows[0]["MinDistance"] = MinDistanceInKM;
                                            dtSizeTypeMst.Rows[0]["MinDistanceUOM"] = MinDistanceUOM;
                                            dtSizeTypeMst.Rows[0]["MinDistanceRate"] = MinDistanceRate;
                                            dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
                                            dtSizeTypeMst.Rows[0]["RatePerDistanceUOM"] = RatePerUnitAboveMinDistance;
                                            dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = MinDistanceRate;
                                            dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinute;
                                            dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;
                                            dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;
                                            dtSizeTypeMst.Rows[0]["Total_cost"] = Math.Round(TotalRate);
                                            dtSizeTypeMst.Rows[0]["Total_cost_IncludeMargin"] = TotalRateIncludeMargin;
                                            dtSizeTypeMst.Rows[0]["Total_cost_without_discount"] = Math.Round(TotalRate) + Discount;
                                            dtSizeTypeMst.Rows[0]["Discount"] = Discount;
                                            dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;

                                            dtSizeTypeMst.Rows[0]["AddSerBaseDiscount"] = 0;// AddSerBaseDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_PT_Charge"] = 0;//TotalPaintingCharge;
                                            dtSizeTypeMst.Rows[0]["Total_PT_Discount"] = 0;// TotalPaintingDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_CL_Charge"] = 0;//TotalCleaningCharge;
                                            dtSizeTypeMst.Rows[0]["Total_CL_Discount"] = 0;//TotalCleaningDiscount;
                                            dtSizeTypeMst.Rows[0]["Total_PEST_Charge"] = 0;// TotalPestControlCharge;
                                            dtSizeTypeMst.Rows[0]["Total_PEST_Discount"] = 0;//TotalPestControlDiscount;
                                            dtSizeTypeMst.Rows[0]["TotalAddServiceDiscount"] = 0;// TotalAddServiceDiscount;
                                            dtSizeTypeMst.Rows[0]["TotalAddServiceCharge"] = 0;// TotalAddServiceCharge + TotalAddServiceDiscount;

                                            if (dtSizeTypeMst.Columns.Contains("Total_cost_without_addon"))
                                                dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRate - TotalAddServiceCharge;
                                            else
                                            {
                                                dtSizeTypeMst.Columns.Add("Total_cost_without_addon");
                                                dtSizeTypeMst.Rows[0]["Total_cost_without_addon"] = TotalRate - TotalAddServiceCharge; ;
                                            }


                                            return dtSizeTypeMst;
                                            //}
                                        }
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


        public DataTable CalculateRateGoods_old(String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinuteOneTruck, Decimal ActualTimeForLoadingInMinuteOneTruck, Decimal ActualTimeForUnloadingInMinuteOneTruck, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                //DateTime OrderDate = DateTime.Today;
                // String goods_type_flag = "H";//Any Type of Goods (Heavy/Light)
                //String rate_type_flag = "B";//Budget(Nomral/Standard)
                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {
                    Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
                    Decimal RateForLoading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForLoading"]);
                    Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
                    Decimal RateForUnloading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForUnloading"]);
                    Decimal PerUnitAboveTimeForLoadUnloadMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["PerUnitAboveTimeForLoadUnloadMinute"]);
                    Decimal RatePerUnitAboveTimeForLoadUnload = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerUnitAboveTimeForLoadUnload"]);
                    Decimal TrukkerMargin = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMargin"]);
                    Decimal RatePerKM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerKM"]);
                    Decimal RatePerMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerMinute"]);
                    String DistanceUOM = Constant.DistanceUOM_KM;
                    Decimal UnitDistance = 1;

                    if (NoOfTruck.HasValue)
                        NoOfDriver = NoOfTruck;

                    if (ActualTimeForLoadingInMinuteOneTruck <= 0)
                        ActualTimeForLoadingInMinuteOneTruck = TimeForLoadingInMinute;

                    if (ActualTimeForUnloadingInMinuteOneTruck <= 0)
                        ActualTimeForUnloadingInMinuteOneTruck = TimeForUnloadingInMinute;

                    Decimal TotalRateForLoading = 0;
                    if (ActualTimeForLoadingInMinuteOneTruck > TimeForLoadingInMinute)
                    {
                        Decimal ExtraTimeForLoading = ActualTimeForLoadingInMinuteOneTruck - TimeForLoadingInMinute;
                        Decimal PerUnitInMinute = ExtraTimeForLoading / PerUnitAboveTimeForLoadUnloadMinute;
                        TotalRateForLoading = RateForLoading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
                    }
                    else
                        TotalRateForLoading = RateForLoading;

                    Decimal TotalRateForUnLoading = 0;
                    if (ActualTimeForUnloadingInMinuteOneTruck > TimeForUnloadingInMinute)
                    {
                        Decimal ExtraTimeForUnloading = ActualTimeForUnloadingInMinuteOneTruck - TimeForUnloadingInMinute;
                        Decimal PerUnitInMinute = ExtraTimeForUnloading / PerUnitAboveTimeForLoadUnloadMinute;
                        TotalRateForUnLoading = RateForUnloading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
                    }
                    else
                        TotalRateForUnLoading = RateForUnloading;

                    if (DistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                    {
                        Decimal ConversionFactor = GetConversionFactor(DistanceUOM, TotalDistanceUOM, ref Message);
                        if (ConversionFactor < 0)
                            return null;
                        else
                        {
                            UnitDistance = UnitDistance * ConversionFactor;
                            DistanceUOM = TotalDistanceUOM;
                        }
                    }

                    Decimal TotalTravelingRate = (TotalDistance / UnitDistance) * RatePerKM;
                    Decimal TotalRateForTravelInMinute = TimeToTravelInMinuteOneTruck * RatePerMinute;

                    Decimal TotalTimeForPostOrder = ActualTimeForLoadingInMinuteOneTruck + TimeToTravelInMinuteOneTruck + ActualTimeForUnloadingInMinuteOneTruck;

                    //Labour Rate
                    Decimal RatePerLabour = 0, TotalLabourRate = 0;
                    if (NoOfLabour.HasValue && NoOfLabour.Value > 0)
                    {
                        ParameterMst objParameterMst = new ParameterMst();
                        DataTable dtParameter = objParameterMst.GetParameter("FullDayRule", null, ref Message);
                        if (dtParameter == null || dtParameter.Rows.Count <= 0)
                            return null;

                        //FullDayRule
                        Decimal FullDayRule = 0.6M, WorkingHours = 10.0M;
                        DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                        if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                            FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                        drArray = dtParameter.Select("Code='WorkingHours'");
                        if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                            WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                        Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                        Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                        if (IncompleteDay > 0)
                        {
                            if (IncompleteDay < FullDayRule)
                                NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                            else
                                NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
                        }
                        else
                            NoOfDay = Math.Floor(NoCompleteDay);
                        //if (IncompleteDay < FullDayRule)
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                        //else
                        //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                        String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                        TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                        if (TotalLabourRate < 0)
                            return null;
                    }

                    Decimal BaseRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading;
                    Decimal TotalRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading + TotalTravelingRate + TotalRateForTravelInMinute + TotalLabourRate;
                    Decimal Discount = 0, NetRate = TotalRate - Discount;

                    dtSizeTypeMst.Rows[0]["goods_type_flag"] = rate_type_flag;

                    dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;

                    dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;

                    //Loading Rate
                    dtSizeTypeMst.Rows[0]["ActualTimeForLoadingInMinute"] = ActualTimeForLoadingInMinuteOneTruck;
                    dtSizeTypeMst.Rows[0]["TotalRateForLoading"] = TotalRateForLoading;
                    //TrukkerMargin
                    dtSizeTypeMst.Rows[0]["TrukkerMargin"] = TrukkerMargin;
                    //Unloading Rate
                    dtSizeTypeMst.Rows[0]["ActualTimeForUnloadingInMinute"] = ActualTimeForUnloadingInMinuteOneTruck;
                    dtSizeTypeMst.Rows[0]["TotalRateForUnLoading"] = TotalRateForUnLoading;
                    //Labour Rate
                    dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour.HasValue ? NoOfLabour.Value : 0;
                    dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;
                    dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;
                    //TotalDistance Rate
                    dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck;
                    dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver;
                    dtSizeTypeMst.Rows[0]["RatePerKM"] = RatePerKM;
                    dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
                    dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = TotalTravelingRate;
                    //TotalMinute Rate.
                    dtSizeTypeMst.Rows[0]["RatePerMinute"] = RatePerMinute;
                    dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinuteOneTruck;
                    dtSizeTypeMst.Rows[0]["TotalRateForTravelInMinute"] = TotalRateForTravelInMinute;

                    dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;

                    dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;

                    dtSizeTypeMst.Rows[0]["Total_cost"] = TotalRate;

                    dtSizeTypeMst.Rows[0]["Discount"] = Discount;

                    dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;


                    //by nitu 29-09-2016
                    dtSizeTypeMst.Columns.Add("Total_cost_margin", typeof(string));
                    dtSizeTypeMst.Rows[0]["Total_cost_margin"] = Convert.ToInt32(TotalRate + ((TotalRate * TrukkerMargin) / 100)).ToString();

                    return dtSizeTypeMst;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }

        public DataTable CalculateRateGoods(DataTable dtPostOrderParameter, String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinuteOneTruck, Decimal ActualTimeForLoadingInMinuteOneTruck, Decimal ActualTimeForUnloadingInMinuteOneTruck, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, int? NoOfHandiman, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                //TimeToTravelInMinuteOneTruck = TotalDistance * 2;
                //DateTime OrderDate = DateTime.Today;
                // String goods_type_flag = "H";//Any Type of Goods (Heavy/Light)
                //String rate_type_flag = "B";//Budget(Nomral/Standard)
                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {
                    DataTable dtTruckTypeDistanceMatrix = GetTruckTypeDistanceMatrix(SizeTypeCode, ref Message);
                    if (dtTruckTypeDistanceMatrix == null || dtTruckTypeDistanceMatrix.Rows.Count <= 0)
                        return null;
                    else
                    {
                        string objMinitMulti = "";
                        TimeToTravelInMinuteOneTruck = 0;
                        //DataRow[] dr_TruckTypeDistanceMatrix = dtTruckTypeDistanceMatrix.Select("FromKM <= " + TotalDistance + " and " + TotalDistance + " <= ToKM");
                        //if (dr_TruckTypeDistanceMatrix.Length > 0)
                        //{
                        //    objMinitMulti = dr_TruckTypeDistanceMatrix[0]["Minconversonfactor"].ToString();
                        //}
                        //TimeToTravelInMinuteOneTruck = TotalDistance * Convert.ToDecimal(objMinitMulti);
                        Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
                        //TimeForLoadingInMinute = TimeForLoadingInMinute + (TimeForLoadingInMinute * 40 / 100);
                        Decimal RateForLoading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForLoading"]);
                        Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
                        Decimal RateForUnloading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForUnloading"]);
                        Decimal PerUnitAboveTimeForLoadUnloadMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["PerUnitAboveTimeForLoadUnloadMinute"]);
                        Decimal RatePerUnitAboveTimeForLoadUnload = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerUnitAboveTimeForLoadUnload"]);
                        Decimal TrukkerMargin = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMargin"]);
                        Decimal RatePerKM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerKM"]);
                        Decimal RatePerMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerMinute"]);
                        String DistanceUOM = Constant.DistanceUOM_KM;
                        Decimal UnitDistance = 1;
                        Decimal MinRateForMovingGoods = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["MinRateForMovingGoods"]);


                        //Truck Rate
                        String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                        if (!NoOfTruck.HasValue)
                            NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);

                        Decimal FromKM = 0, ToKM = 0, Distance = 0, TotalTravelingRate = 0;
                        for (int i = 0; i <= dtTruckTypeDistanceMatrix.Rows.Count; i++)
                        {
                            FromKM = Convert.ToDecimal(dtTruckTypeDistanceMatrix.Rows[i]["FromKM"]);
                            ToKM = Convert.ToDecimal(dtTruckTypeDistanceMatrix.Rows[i]["ToKM"]);
                            RatePerKM = Convert.ToDecimal(dtTruckTypeDistanceMatrix.Rows[i]["RatePerKM"]);
                            objMinitMulti = dtTruckTypeDistanceMatrix.Rows[i]["Minconversonfactor"].ToString();

                            if (TotalDistance > ToKM)
                                Distance = (ToKM - FromKM) + 1;
                            else
                                Distance = (TotalDistance - FromKM) + 1;

                            TotalTravelingRate += Distance * RatePerKM;
                            TimeToTravelInMinuteOneTruck += Distance * Convert.ToDecimal(objMinitMulti);

                            if (TotalDistance <= ToKM)
                                break;
                        }


                        Decimal Totaltimefortransaction = TimeToTravelInMinuteOneTruck + TimeForLoadingInMinute + TimeForUnloadingInMinute;

                        // Decimal TotalRateForTravelInMinute = TimeToTravelInMinuteOneTruck * RatePerMinute + ((TimeToTravelInMinuteOneTruck * RatePerMinute) * TrukkerMargin / 100);
                        Decimal TotalRateForTravelInMinute = TimeToTravelInMinuteOneTruck * RatePerMinute;// +((TimeToTravelInMinuteOneTruck * RatePerMinute) * TrukkerMargin / 100);
                        Decimal BaseRate = MinRateForMovingGoods;
                        Decimal TotalRate = MinRateForMovingGoods + TotalTravelingRate + TotalRateForTravelInMinute;
                        Decimal Discount = 0, NetRate = TotalRate - Discount;


                        if (NoOfTruck.HasValue)
                            NoOfDriver = NoOfTruck;

                        //if (ActualTimeForLoadingInMinuteOneTruck <= 0)
                        //    ActualTimeForLoadingInMinuteOneTruck = TimeForLoadingInMinute;

                        //if (ActualTimeForUnloadingInMinuteOneTruck <= 0)
                        //    ActualTimeForUnloadingInMinuteOneTruck = TimeForUnloadingInMinute;

                        Decimal TotalRateForLoading = 0;
                        //if (ActualTimeForLoadingInMinuteOneTruck > TimeForLoadingInMinute)
                        //{
                        //    Decimal ExtraTimeForLoading = ActualTimeForLoadingInMinuteOneTruck - TimeForLoadingInMinute;
                        //    Decimal PerUnitInMinute = ExtraTimeForLoading / PerUnitAboveTimeForLoadUnloadMinute;
                        //    TotalRateForLoading = RateForLoading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
                        //}
                        //else
                        //    TotalRateForLoading = RateForLoading;

                        Decimal TotalRateForUnLoading = 0;
                        //if (ActualTimeForUnloadingInMinuteOneTruck > TimeForUnloadingInMinute)
                        //{
                        //    Decimal ExtraTimeForUnloading = ActualTimeForUnloadingInMinuteOneTruck - TimeForUnloadingInMinute;
                        //    Decimal PerUnitInMinute = ExtraTimeForUnloading / PerUnitAboveTimeForLoadUnloadMinute;
                        //    TotalRateForUnLoading = RateForUnloading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
                        //}
                        //else
                        //    TotalRateForUnLoading = RateForUnloading;

                        //if (DistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
                        //{
                        //    Decimal ConversionFactor = GetConversionFactor(DistanceUOM, TotalDistanceUOM, ref Message);
                        //    if (ConversionFactor < 0)
                        //        return null;
                        //    else
                        //    {
                        //        UnitDistance = UnitDistance * ConversionFactor;
                        //        DistanceUOM = TotalDistanceUOM;
                        //    }
                        //}

                        //Decimal TotalTravelingRate = (TotalDistance / UnitDistance) * RatePerKM;
                        //Decimal TotalRateForTravelInMinute = TimeToTravelInMinuteOneTruck * RatePerMinute;

                        //Decimal TotalTimeForPostOrder = ActualTimeForLoadingInMinuteOneTruck + TimeToTravelInMinuteOneTruck + ActualTimeForUnloadingInMinuteOneTruck;
                        Decimal TotalTimeForPostOrder = TimeToTravelInMinuteOneTruck;

                        // Labour Means Helper 
                        String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                        if (!NoOfLabour.HasValue)
                            NoOfLabour = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"]);

                        String Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                        if (!NoOfHandiman.HasValue)
                            NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);


                        ////Labour Rate
                        Decimal RatePerLabour = 0, TotalLabourRate = 0; Decimal TotalHandimanRate = 0; Decimal RatePerHandiman = 0;
                        if (NoOfLabour.HasValue && NoOfLabour.Value > 0)
                        {
                            ParameterMst objParameterMst = new ParameterMst();
                            DataTable dtParameter = objParameterMst.GetParameter("FullDayRule", null, ref Message);
                            if (dtParameter == null || dtParameter.Rows.Count <= 0)
                                return null;

                            //FullDayRule
                            Decimal FullDayRule = 0.6M, WorkingHours = 10.0M;
                            DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                            if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                                FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                            drArray = dtParameter.Select("Code='WorkingHours'");
                            if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                                WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                            Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                            Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                            if (IncompleteDay > 0)
                            {
                                if (IncompleteDay < FullDayRule)
                                    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                                else
                                    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
                            }
                            else
                                NoOfDay = Math.Floor(NoCompleteDay);
                            //if (IncompleteDay < FullDayRule)
                            //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                            //else
                            //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                            labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                            TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                            if (TotalLabourRate < 0)
                                return null;
                            else
                            {
                                //Handiman Rate
                                Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                                if (!NoOfHandiman.HasValue)
                                    NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);
                                TotalHandimanRate = GetHandimanRate(Handiman_type_code, goods_type_flag, rate_type_flag, NoOfHandiman.Value, NoOfDay, OrderDate, ref RatePerHandiman, ref Message);
                                if (TotalHandimanRate < 0)
                                    return null;
                            }
                        }

                        //Decimal BaseRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading;
                        //Decimal TotalRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading + TotalTravelingRate + TotalRateForTravelInMinute + TotalLabourRate;
                        //Decimal Discount = 0, NetRate = TotalRate - Discount;

                        dtSizeTypeMst.Rows[0]["goods_type_flag"] = goods_type_flag;

                        dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;

                        dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;

                        //Loading Rate
                        dtSizeTypeMst.Rows[0]["ActualTimeForLoadingInMinute"] = ActualTimeForLoadingInMinuteOneTruck;
                        dtSizeTypeMst.Rows[0]["TotalRateForLoading"] = TotalRateForLoading;
                        //TrukkerMargin
                        dtSizeTypeMst.Rows[0]["TrukkerMargin"] = TrukkerMargin;
                        //Unloading Rate
                        dtSizeTypeMst.Rows[0]["ActualTimeForUnloadingInMinute"] = ActualTimeForUnloadingInMinuteOneTruck;
                        dtSizeTypeMst.Rows[0]["TotalRateForUnLoading"] = TotalRateForUnLoading;
                        //Labour Rate
                        dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour.HasValue ? NoOfLabour.Value : 0;
                        dtSizeTypeMst.Rows[0]["NoOfHandiman"] = NoOfHandiman.HasValue ? NoOfHandiman.Value : 0;

                        if (Totaltimefortransaction < 240)
                            RatePerLabour = 80;//100;
                        else
                            RatePerLabour = 130;//150;


                        dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;

                        if (NoOfLabour > 0 && NoOfLabour < 1)
                            TotalLabourRate = NoOfLabour.Value * RatePerLabour;
                        else
                            TotalLabourRate = NoOfLabour.Value * RatePerLabour;

                        dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;

                        if (Totaltimefortransaction < 240)
                            RatePerHandiman = 80;//100;
                        else
                            RatePerHandiman = 130;//200;

                        dtSizeTypeMst.Rows[0]["RatePerHandiman"] = RatePerHandiman;

                        if (NoOfHandiman > 0 && NoOfHandiman < 1)
                            TotalHandimanRate = NoOfHandiman.Value * RatePerHandiman;
                        else
                            TotalHandimanRate = NoOfHandiman.Value * RatePerHandiman;

                        dtSizeTypeMst.Rows[0]["TotalHandimanRate"] = TotalHandimanRate;


                        //TotalDistance Rate
                        dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck;
                        dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver;
                        dtSizeTypeMst.Rows[0]["RatePerKM"] = RatePerKM;
                        dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
                        dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = TotalTravelingRate;
                        //TotalMinute Rate.
                        dtSizeTypeMst.Rows[0]["RatePerMinute"] = RatePerMinute;

                        //total hours
                        //int hours = (int)TimeToTravelInMinuteOneTruck / 60;
                        ////total minutes
                        //int minutes = (int)TimeToTravelInMinuteOneTruck % 60;
                        ////output is 1:10


                        dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinuteOneTruck;

                        dtSizeTypeMst.Rows[0]["TotalRateForTravelInMinute"] = TotalRateForTravelInMinute;

                        dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;

                        dtSizeTypeMst.Rows[0]["BaseRate"] = TotalRate;

                        TotalRate = TotalRate + TotalLabourRate + TotalHandimanRate;
                        //TotalRate = TotalRate + TotalLabourRate;

                        dtSizeTypeMst.Rows[0]["Total_cost"] = Math.Round(TotalRate);

                        dtSizeTypeMst.Rows[0]["Discount"] = Discount;

                        dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;


                        //by nitu 29-09-2016
                        dtSizeTypeMst.Columns.Add("Total_cost_margin", typeof(string));
                        dtSizeTypeMst.Rows[0]["Total_cost_margin"] = Convert.ToInt32(TotalRate + ((TotalRate * TrukkerMargin) / 100)).ToString();

                        return dtSizeTypeMst;
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

        public DataTable CalculateRateHireTruck(DataTable dtPostOrderParameter, String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinuteOneTruck, Decimal ActualTimeForLoadingInMinuteOneTruck, Decimal ActualTimeForUnloadingInMinuteOneTruck, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, int? NoOfHandiman, ref String Message)
        {
            try
            {
                Decimal NoOfDay = 1.0M;
                //TimeToTravelInMinuteOneTruck = TotalDistance * 2;
                //DateTime OrderDate = DateTime.Today;
                // String goods_type_flag = "H";//Any Type of Goods (Heavy/Light)
                //String rate_type_flag = "B";//Budget(Nomral/Standard)
                DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
                if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
                    return null;
                else
                {

                    TimeToTravelInMinuteOneTruck = 0;
                    Decimal MinRateForHireTruck = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["HireTruck_MinRate"]);
                    Decimal HireTruck_FuelRatePerDay = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["HireTruck_FuelRatePerDay"]);
                    Decimal Hiretruck_MaxKM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["Hiretruck_MaxKM"]);
                    Decimal HireTruck_AdditionalKMCharges = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["HireTruck_AdditionalKMCharges"]);
                    NoOfDay = dtPostOrderParameter.Rows[0]["Hiretruck_NoofDay"].ToString() != "" ? Convert.ToDecimal(dtPostOrderParameter.Rows[0]["Hiretruck_NoofDay"]) : 1;


                    String DistanceUOM = Constant.DistanceUOM_KM;

                    Decimal TotalRate = 0;

                    //Truck Rate
                    String truck_type_code = dtSizeTypeMst.Rows[0]["truck_type_code"].ToString();
                    if (!NoOfTruck.HasValue)
                        NoOfTruck = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfTruck"]);

                    Decimal Totaltimefortransaction = TimeToTravelInMinuteOneTruck;

                    Decimal BaseRate = MinRateForHireTruck * NoOfDay;
                    Decimal Hiretruck_TotalFuelRate = HireTruck_FuelRatePerDay * NoOfDay;

                    if (dtPostOrderParameter.Rows[0]["Hiretruck_IncludingFuel"].ToString() == Constant.Flag_Yes)
                        TotalRate = BaseRate + Hiretruck_TotalFuelRate;
                    else
                        TotalRate = BaseRate;

                    if (NoOfTruck.HasValue)
                        NoOfDriver = NoOfTruck;

                    Decimal TotalRateForLoading = 0;

                    Decimal TotalRateForUnLoading = 0;


                    // Labour Means Helper 
                    String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                    if (!NoOfLabour.HasValue)
                        NoOfLabour = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfLabour"]);

                    String Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                    if (!NoOfHandiman.HasValue)
                        NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);


                    ////Labour Rate
                    Decimal RatePerLabour = 0, TotalLabourRate = 0; Decimal TotalHandimanRate = 0; Decimal RatePerHandiman = 0;
                    //if (NoOfLabour.HasValue && NoOfLabour.Value > 0)
                    //{
                    //ParameterMst objParameterMst = new ParameterMst();
                    //DataTable dtParameter = objParameterMst.GetParameter("FullDayRule", null, ref Message);
                    //if (dtParameter == null || dtParameter.Rows.Count <= 0)
                    //    return null;

                    ////FullDayRule
                    //Decimal FullDayRule = 0.6M, WorkingHours = 10.0M;
                    //DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
                    //if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    //    FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
                    //drArray = dtParameter.Select("Code='WorkingHours'");
                    //if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
                    //    WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

                    //Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
                    //Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
                    //if (IncompleteDay > 0)
                    //{
                    //    if (IncompleteDay < FullDayRule)
                    //        NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                    //    else
                    //        NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
                    //}
                    //else
                    //    NoOfDay = Math.Floor(NoCompleteDay);

                    //if (IncompleteDay < FullDayRule)
                    //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
                    //else
                    //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

                    labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
                    TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
                    if (TotalLabourRate < 0)
                        return null;
                    else
                    {
                        //Handiman Rate
                        Handiman_type_code = dtSizeTypeMst.Rows[0]["Handiman_type_code"].ToString();
                        if (!NoOfHandiman.HasValue)
                            NoOfHandiman = Convert.ToInt32(dtSizeTypeMst.Rows[0]["NoOfHandiman"]);
                        TotalHandimanRate = GetHandimanRate(Handiman_type_code, goods_type_flag, rate_type_flag, NoOfHandiman.Value, NoOfDay, OrderDate, ref RatePerHandiman, ref Message);
                        if (TotalHandimanRate < 0)
                            return null;
                    }

                    dtSizeTypeMst.Columns.Add("Hiretruck_TotalFuelRate", typeof(string));
                    dtSizeTypeMst.Rows[0]["Hiretruck_TotalFuelRate"] = Hiretruck_TotalFuelRate;

                    if (dtSizeTypeMst.Columns.Contains("Hiretruck_IncludingFuel"))
                        dtSizeTypeMst.Rows[0]["Hiretruck_IncludingFuel"] = dtPostOrderParameter.Rows[0]["Hiretruck_IncludingFuel"].ToString();
                    else
                    {
                        dtSizeTypeMst.Columns.Add("Hiretruck_IncludingFuel", typeof(string));
                        dtSizeTypeMst.Rows[0]["Hiretruck_IncludingFuel"] = dtPostOrderParameter.Rows[0]["Hiretruck_IncludingFuel"].ToString();
                    }


                    if (dtPostOrderParameter.Columns.Contains("Hiretruck_TotalDayRate"))
                        dtPostOrderParameter.Rows[0]["Hiretruck_TotalDayRate"] = BaseRate;
                    else
                    {
                        dtPostOrderParameter.Columns.Add("Hiretruck_TotalDayRate", typeof(string));
                        dtPostOrderParameter.Rows[0]["Hiretruck_TotalDayRate"] = BaseRate;
                    }

                    dtSizeTypeMst.Rows[0]["goods_type_flag"] = goods_type_flag;

                    dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;

                    dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;

                    //Loading Rate
                    dtSizeTypeMst.Rows[0]["ActualTimeForLoadingInMinute"] = ActualTimeForLoadingInMinuteOneTruck;
                    dtSizeTypeMst.Rows[0]["TotalRateForLoading"] = TotalRateForLoading;
                    //TrukkerMargin
                    //dtSizeTypeMst.Rows[0]["TrukkerMargin"] = TrukkerMargin;
                    //Unloading Rate
                    dtSizeTypeMst.Rows[0]["ActualTimeForUnloadingInMinute"] = ActualTimeForUnloadingInMinuteOneTruck;
                    dtSizeTypeMst.Rows[0]["TotalRateForUnLoading"] = TotalRateForUnLoading;
                    //Labour Rate
                    dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour.HasValue ? NoOfLabour.Value : 0;
                    dtSizeTypeMst.Rows[0]["NoOfHandiman"] = NoOfHandiman.HasValue ? NoOfHandiman.Value : 0;

                    //if (Totaltimefortransaction < 240)
                    //    RatePerLabour = 100;
                    //else
                    //    RatePerLabour = 150;


                    dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;

                    //if (NoOfLabour > 0 && NoOfLabour < 1)
                    //    TotalLabourRate = NoOfLabour.Value * RatePerLabour;
                    //else
                    //    TotalLabourRate = NoOfLabour.Value * RatePerLabour;

                    dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;

                    //if (Totaltimefortransaction < 240)
                    //    RatePerHandiman = 100;
                    //else
                    //    RatePerHandiman = 200;

                    dtSizeTypeMst.Rows[0]["RatePerHandiman"] = RatePerHandiman;

                    //if (NoOfHandiman > 0 && NoOfHandiman < 1)
                    //    TotalHandimanRate = NoOfHandiman.Value * RatePerHandiman;
                    //else
                    //    TotalHandimanRate = NoOfHandiman.Value * RatePerHandiman;

                    dtSizeTypeMst.Rows[0]["TotalHandimanRate"] = TotalHandimanRate;


                    //TotalDistance Rate
                    dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck;
                    dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver;
                    // dtSizeTypeMst.Rows[0]["RatePerKM"] = RatePerKM;
                    dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
                    // dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = TotalTravelingRate;
                    //TotalMinute Rate.
                    //dtSizeTypeMst.Rows[0]["RatePerMinute"] = RatePerMinute;

                    //total hours
                    //int hours = (int)TimeToTravelInMinuteOneTruck / 60;
                    ////total minutes
                    //int minutes = (int)TimeToTravelInMinuteOneTruck % 60;
                    ////output is 1:10


                    dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinuteOneTruck;

                    //dtSizeTypeMst.Rows[0]["TotalRateForTravelInMinute"] = TotalRateForTravelInMinute;

                    //dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;

                    dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;

                    TotalRate = TotalRate + TotalLabourRate + TotalHandimanRate;
                    //TotalRate = TotalRate + TotalLabourRate;

                    dtSizeTypeMst.Rows[0]["Total_cost"] = Math.Round(TotalRate);

                    //by nitu 29-09-2016
                    dtSizeTypeMst.Columns.Add("Total_cost_margin", typeof(string));
                    //dtSizeTypeMst.Rows[0]["Total_cost_margin"] = Convert.ToInt32(TotalRate + ((TotalRate * TrukkerMargin) / 100)).ToString();

                    return dtSizeTypeMst;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }


        //public DataTable CalculateRateGoods(String SizeTypeCode, DateTime OrderDate, String goods_type_flag, String rate_type_flag, Decimal TotalDistance, String TotalDistanceUOM, Decimal TimeToTravelInMinuteOneTruck, Decimal ActualTimeForLoadingInMinuteOneTruck, Decimal ActualTimeForUnloadingInMinuteOneTruck, int? NoOfTruck, int? NoOfDriver, int? NoOfLabour, ref String Message)
        //{
        //    try
        //    {
        //        Decimal NoOfDay = 1.0M;
        //        //DateTime OrderDate = DateTime.Today;
        //        // String goods_type_flag = "H";//Any Type of Goods (Heavy/Light)
        //        //String rate_type_flag = "B";//Budget(Nomral/Standard)
        //        DataTable dtSizeTypeMst = GetSizeTypeDetail(SizeTypeCode, rate_type_flag, TotalDistance, TotalDistanceUOM, ref Message);
        //        if (dtSizeTypeMst == null || dtSizeTypeMst.Rows.Count <= 0)
        //            return null;
        //        else
        //        {
        //            Decimal TimeForLoadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForLoadingInMinute"]);
        //            Decimal RateForLoading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForLoading"]);
        //            Decimal TimeForUnloadingInMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TimeForUnloadingInMinute"]);
        //            Decimal RateForUnloading = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RateForUnloading"]);
        //            Decimal PerUnitAboveTimeForLoadUnloadMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["PerUnitAboveTimeForLoadUnloadMinute"]);
        //            Decimal RatePerUnitAboveTimeForLoadUnload = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerUnitAboveTimeForLoadUnload"]);
        //            Decimal TrukkerMargin = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["TrukkerMargin"]);
        //            Decimal RatePerKM = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerKM"]);
        //            Decimal RatePerMinute = Convert.ToDecimal(dtSizeTypeMst.Rows[0]["RatePerMinute"]);
        //            String DistanceUOM = Constant.DistanceUOM_KM;
        //            Decimal UnitDistance = 1;

        //            if (NoOfTruck.HasValue)
        //                NoOfDriver = NoOfTruck;

        //            if (ActualTimeForLoadingInMinuteOneTruck <= 0)
        //                ActualTimeForLoadingInMinuteOneTruck = TimeForLoadingInMinute;

        //            if (ActualTimeForUnloadingInMinuteOneTruck <= 0)
        //                ActualTimeForUnloadingInMinuteOneTruck = TimeForUnloadingInMinute;

        //            Decimal TotalRateForLoading = 0;
        //            if (ActualTimeForLoadingInMinuteOneTruck > TimeForLoadingInMinute)
        //            {
        //                Decimal ExtraTimeForLoading = ActualTimeForLoadingInMinuteOneTruck - TimeForLoadingInMinute;
        //                Decimal PerUnitInMinute = ExtraTimeForLoading / PerUnitAboveTimeForLoadUnloadMinute;
        //                TotalRateForLoading = RateForLoading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
        //            }
        //            else
        //                TotalRateForLoading = RateForLoading;

        //            Decimal TotalRateForUnLoading = 0;
        //            if (ActualTimeForUnloadingInMinuteOneTruck > TimeForUnloadingInMinute)
        //            {
        //                Decimal ExtraTimeForUnloading = ActualTimeForUnloadingInMinuteOneTruck - TimeForUnloadingInMinute;
        //                Decimal PerUnitInMinute = ExtraTimeForUnloading / PerUnitAboveTimeForLoadUnloadMinute;
        //                TotalRateForUnLoading = RateForUnloading + (PerUnitInMinute * RatePerUnitAboveTimeForLoadUnload);
        //            }
        //            else
        //                TotalRateForUnLoading = RateForUnloading;

        //            if (DistanceUOM.ToUpper() != TotalDistanceUOM.ToUpper())
        //            {
        //                Decimal ConversionFactor = GetConversionFactor(DistanceUOM, TotalDistanceUOM, ref Message);
        //                if (ConversionFactor < 0)
        //                    return null;
        //                else
        //                {
        //                    UnitDistance = UnitDistance * ConversionFactor;
        //                    DistanceUOM = TotalDistanceUOM;
        //                }
        //            }

        //            Decimal TotalTravelingRate = (TotalDistance / UnitDistance) * RatePerKM;
        //            Decimal TotalRateForTravelInMinute = TimeToTravelInMinuteOneTruck * RatePerMinute;

        //            Decimal TotalTimeForPostOrder = ActualTimeForLoadingInMinuteOneTruck + TimeToTravelInMinuteOneTruck + ActualTimeForUnloadingInMinuteOneTruck;

        //            //Labour Rate
        //            Decimal RatePerLabour = 0, TotalLabourRate = 0;
        //            if (NoOfLabour.HasValue && NoOfLabour.Value > 0)
        //            {
        //                ParameterMst objParameterMst = new ParameterMst();
        //                DataTable dtParameter = objParameterMst.GetParameter("FullDayRule", null, ref Message);
        //                if (dtParameter == null || dtParameter.Rows.Count <= 0)
        //                    return null;

        //                //FullDayRule
        //                Decimal FullDayRule = 0.6M, WorkingHours = 10.0M;
        //                DataRow[] drArray = dtParameter.Select("Code='FullDayRule'");
        //                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
        //                    FullDayRule = Convert.ToDecimal(drArray[0]["Value"]);
        //                drArray = dtParameter.Select("Code='WorkingHours'");
        //                if (drArray != null && drArray.Length > 0 && drArray[0]["Value"].ToString().Trim() != String.Empty)
        //                    WorkingHours = Convert.ToDecimal(drArray[0]["Value"]);

        //                Decimal NoCompleteDay = TotalTimeForPostOrder / (60 * WorkingHours);
        //                Decimal IncompleteDay = NoCompleteDay - Math.Floor(NoCompleteDay);
        //                if (IncompleteDay > 0)
        //                {
        //                    if (IncompleteDay < FullDayRule)
        //                        NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
        //                    else
        //                        NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;
        //                }
        //                else
        //                    NoOfDay = Math.Floor(NoCompleteDay);
        //                //if (IncompleteDay < FullDayRule)
        //                //    NoOfDay = Math.Floor(NoCompleteDay) + 0.5M;
        //                //else
        //                //    NoOfDay = Math.Floor(NoCompleteDay) + 1.0M;

        //                String labour_type_code = dtSizeTypeMst.Rows[0]["labour_type_code"].ToString();
        //                TotalLabourRate = GetLabourRate(labour_type_code, goods_type_flag, rate_type_flag, NoOfLabour.Value, NoOfDay, OrderDate, ref RatePerLabour, ref Message);
        //                if (TotalLabourRate < 0)
        //                    return null;
        //            }

        //            Decimal BaseRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading;
        //            Decimal TotalRate = TotalRateForLoading + TrukkerMargin + TotalRateForUnLoading + TotalTravelingRate + TotalRateForTravelInMinute + TotalLabourRate;
        //            Decimal Discount = 0, NetRate = TotalRate - Discount;

        //            dtSizeTypeMst.Rows[0]["goods_type_flag"] = rate_type_flag;

        //            dtSizeTypeMst.Rows[0]["rate_type_flag"] = rate_type_flag;

        //            dtSizeTypeMst.Rows[0]["NoOfDay"] = NoOfDay;

        //            //Loading Rate
        //            dtSizeTypeMst.Rows[0]["ActualTimeForLoadingInMinute"] = ActualTimeForLoadingInMinuteOneTruck;
        //            dtSizeTypeMst.Rows[0]["TotalRateForLoading"] = TotalRateForLoading;
        //            //TrukkerMargin
        //            dtSizeTypeMst.Rows[0]["TrukkerMargin"] = TrukkerMargin;
        //            //Unloading Rate
        //            dtSizeTypeMst.Rows[0]["ActualTimeForUnloadingInMinute"] = ActualTimeForUnloadingInMinuteOneTruck;
        //            dtSizeTypeMst.Rows[0]["TotalRateForUnLoading"] = TotalRateForUnLoading;
        //            //Labour Rate
        //            dtSizeTypeMst.Rows[0]["NoOfLabour"] = NoOfLabour.HasValue ? NoOfLabour.Value : 0;
        //            dtSizeTypeMst.Rows[0]["RatePerLabour"] = RatePerLabour;
        //            dtSizeTypeMst.Rows[0]["TotalLabourRate"] = TotalLabourRate;
        //            //TotalDistance Rate
        //            dtSizeTypeMst.Rows[0]["NoOfTruck"] = NoOfTruck;
        //            dtSizeTypeMst.Rows[0]["NoOfDriver"] = NoOfDriver;
        //            dtSizeTypeMst.Rows[0]["RatePerKM"] = RatePerKM;
        //            dtSizeTypeMst.Rows[0]["TotalDistance"] = TotalDistance;
        //            dtSizeTypeMst.Rows[0]["TotalTravelingRate"] = TotalTravelingRate;
        //            //TotalMinute Rate.
        //            dtSizeTypeMst.Rows[0]["RatePerMinute"] = RatePerMinute;
        //            dtSizeTypeMst.Rows[0]["TimeToTravelInMinute"] = TimeToTravelInMinuteOneTruck;
        //            dtSizeTypeMst.Rows[0]["TotalRateForTravelInMinute"] = TotalRateForTravelInMinute;

        //            dtSizeTypeMst.Rows[0]["TotalTimeForPostOrder"] = TotalTimeForPostOrder;

        //            dtSizeTypeMst.Rows[0]["BaseRate"] = BaseRate;

        //            dtSizeTypeMst.Rows[0]["Total_cost"] = TotalRate;

        //            dtSizeTypeMst.Rows[0]["Discount"] = Discount;

        //            dtSizeTypeMst.Rows[0]["Net_cost"] = NetRate;

        //            return dtSizeTypeMst;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
        //        Message = ex.Message;
        //        return null;
        //    }
        //}

        public Boolean IsCouponValid(String coupon_code, String shipper_id, String load_inquiry_no, String order_id, DateTime OrderDate, String order_type_flag, string ratetypeflag, string sizetypecode, ref Decimal FlatDiscount, ref Decimal PercentageDiscount, ref String Message)
        {
            try
            {
                FlatDiscount = 0;
                PercentageDiscount = 0;
                DataTable dtCouponMst = GetCouponDetail(coupon_code, order_type_flag, ratetypeflag, sizetypecode, OrderDate, ref Message);
                if (dtCouponMst == null || dtCouponMst.Rows.Count <= 0)
                    return false;
                else
                {

                    DBDataAdpterObject.SelectCommand.Parameters.Clear();
                    StringBuilder SQLSelect = new StringBuilder();
                    SQLSelect.Append(@" SELECT * FROM CouponUserHistory 
                                       WHERE  coupon_code = @coupon_code ");
                    if (dtCouponMst.Rows[0]["each_user_once_at_time"].ToString().Trim() == "Y")
                    {
                        SQLSelect.Append(" AND shipper_id = @shipper_id ");
                        DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                    }

                    DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@coupon_code", DbType.String, coupon_code));
                    DBDataAdpterObject.TableMappings.Clear();
                    DBDataAdpterObject.TableMappings.Add("Table", "CouponUserHistory");
                    DataSet ds = new DataSet();
                    DBDataAdpterObject.Fill(ds);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (dtCouponMst.Rows[0]["each_user_once_at_time"].ToString().Trim() == "N")
                        {
                            Message = "Valid coupon code : " + coupon_code;
                            FlatDiscount = Convert.ToDecimal(dtCouponMst.Rows[0]["flat_discount"]);
                            PercentageDiscount = Convert.ToDecimal(dtCouponMst.Rows[0]["percentage_discount"]);
                            return true;
                        }
                        else
                        {
                            Message = "Already used coupon code : " + coupon_code;
                            return false;
                        }
                    }
                    else
                    {
                        Message = "Valid coupon code : " + coupon_code;
                        FlatDiscount = Convert.ToDecimal(dtCouponMst.Rows[0]["flat_discount"]);
                        PercentageDiscount = Convert.ToDecimal(dtCouponMst.Rows[0]["percentage_discount"]);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return false;
            }
        }

        public DataTable GetCouponDetail(String coupon_code, String OrderTypeFlag, String ratetypeflag, String SizeTypeCode, DateTime OrderDate, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM CouponMst 
                                   WHERE  coupon_code = @coupon_code and order_type_flag=@OrderTypeFlag ");
                if (OrderTypeFlag == Constant.ORDERTYPECODEFORHOME)
                {
                    SQLSelect.Append(@" and SizeTypeCode=@SizeTypeCode and rate_type_flag=@ratetypeflag ");
                }



                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@coupon_code", DbType.String, coupon_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@OrderTypeFlag", DbType.String, OrderTypeFlag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ratetypeflag", DbType.String, ratetypeflag));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "CouponMst");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (!(Convert.ToDateTime(ds.Tables[0].Rows[0]["start_date"]) < OrderDate && (ds.Tables[0].Rows[0]["end_date"].ToString().Trim() == String.Empty || Convert.ToDateTime(ds.Tables[0].Rows[0]["end_date"]).AddDays(1) > OrderDate)))
                    {
                        Message = "Expired coupon code : " + coupon_code;
                        return null;
                    }
                    else if (OrderTypeFlag == Constant.ORDERTYPECODEFORHOME)
                    {
                        Message = "Coupon detail retrieved successfully coupon code : " + coupon_code;
                        return ds.Tables[0];
                    }
                    else if (OrderTypeFlag == Constant.ORDER_TYPE_CODE_HIRETRUCK)
                    {
                        Message = "Coupon detail retrieved successfully coupon code : " + coupon_code;
                        return ds.Tables[0];
                    }
                    else if (OrderTypeFlag == Constant.ORDER_TYPE_CODE_MOVING_GOODS_LATER)
                    {
                        Message = "Coupon detail retrieved successfully coupon code : " + coupon_code;
                        return ds.Tables[0];
                    }
                    else
                    {
                        Message = "Invalid coupon code : " + coupon_code;
                        return null;
                    }

                }
                else
                {
                    Message = "Invalid coupon code : " + coupon_code;
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

        //if (OrderTypeFlag == Constant.ORDERTYPECODEFORHOME)
        //{
        //    if (ds.Tables[0].Rows[0]["rate_type_flag"].ToString() != "A")
        //    {
        //        if (ratetypeflag == ds.Tables[0].Rows[0]["rate_type_flag"].ToString())
        //        {
        //            Message = "Coupon detail retrieved successfully coupon code : " + coupon_code;
        //            return ds.Tables[0];
        //        }
        //        else
        //        {
        //            string strratetype = "";
        //            if (ds.Tables[0].Rows[0]["rate_type_flag"].ToString() == Constant.RATE_TYPE_FLAG_SUPERSAVER)
        //                strratetype = "Super Saver";
        //            else if (ds.Tables[0].Rows[0]["rate_type_flag"].ToString() == Constant.RATE_TYPE_FLAG_PREMIUM)
        //                strratetype = "Premium";
        //            else if (ds.Tables[0].Rows[0]["rate_type_flag"].ToString() == Constant.RATE_TYPE_FLAG_STANDERD)
        //                strratetype = "Standard";

        //            Message = "Coupon code Valid For Only " + strratetype + " Quote Type  ";
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        Message = "Coupon detail retrieved successfully coupon code : " + coupon_code;
        //        return ds.Tables[0];
        //    }

        public Byte SaveCouponUserHistory(ref IDbCommand command, String coupon_code, String shipper_id, String load_inquiry_no, String order_id, DateTime OrderDate, Decimal FlatDiscount, Decimal PercentageDiscount, Decimal Total_cost, Decimal Discount, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                DS_CouponUserHistory dS_CouponUserHistory = new DS_CouponUserHistory();
                dS_CouponUserHistory.EnforceConstraints = false;

                DS_CouponUserHistory.CouponUserHistoryRow row = dS_CouponUserHistory.CouponUserHistory.NewCouponUserHistoryRow();
                row.coupon_code = coupon_code;
                row.shipper_id = shipper_id;
                row.load_inquiry_no = load_inquiry_no;
                row.order_id = order_id;
                row.order_date = OrderDate;
                int sr_no = GetMaxSrNoCouponUserHistory(ref command, coupon_code, shipper_id, load_inquiry_no, order_id, OrderDate, ref Message);
                if (sr_no <= 0)
                    return 2;
                row.sr_no = sr_no;
                row.flat_discount = FlatDiscount;
                row.percentage_discount = PercentageDiscount;
                row.Total_cost = Total_cost;
                row.Discount = Discount;
                row.created_by = created_by;
                row.created_date = DateTime.UtcNow;
                row.created_host = created_host;
                row.device_id = device_id;
                row.device_type = device_type;
                dS_CouponUserHistory.CouponUserHistory.AddCouponUserHistoryRow(row);

                try
                {
                    dS_CouponUserHistory.EnforceConstraints = true;
                }
                catch (ConstraintException ce)
                {
                    Message = ce.Message;
                    ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                    return 2;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return 2;
                }

                BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_CouponUserHistory.CouponUserHistory, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_CouponUserHistory.CouponUserHistory.Rows.Count)
                {
                    Message = "An error occurred while insert data in CouponUserHistory. " + objUpdateTableInfo.ErrorMessage;
                    return 2;
                }
                else
                {
                    Message = "CouponUserHistory data inserted successfully.";
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public int GetMaxSrNoCouponUserHistory(ref IDbCommand command, String coupon_code, String shipper_id, String load_inquiry_no, String order_id, DateTime OrderDate, ref String Message)
        {
            try
            {
                command.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT COUNT(*) AS MaxSrNo FROM CouponUserHistory 
                                   WHERE  coupon_code = @coupon_code 
                                   AND  shipper_id = @shipper_id
                                   AND  load_inquiry_no = @load_inquiry_no
                                   AND  order_id = @order_id
                                   AND  order_date = @OrderDate ");
                command.CommandText = SQLSelect.ToString();
                command.Parameters.Add(DBObjectFactory.MakeParameter("@coupon_code", DbType.String, coupon_code));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@OrderDate", DbType.DateTime, OrderDate));
                Object objMaxSrNo = command.ExecuteScalar();
                if (objMaxSrNo != null && objMaxSrNo.ToString().Trim() != String.Empty)
                {
                    Message = "Max SrNo retrieved successfully.";
                    return Convert.ToInt32(objMaxSrNo) + 1;
                }
                else
                {
                    Message = "Max SrNo retrieved successfully.";
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return -1;
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
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("load_inquiry_no", DbType.String, load_inquiry_no));
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

        public Byte SavePaymentReceipt(ref IDbCommand command, DateTime payment_date, String payment_mode, String bank_code, String cheque_no, DateTime? cheque_date, Decimal amount, String shipper_id, String remark, DataTable dt_payment_rcpt_dtl, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                if (payment_mode == null || payment_mode.Trim() == String.Empty)
                {
                    Message = "Payment Mode is not supplied.";
                    return 2;
                }
                else if (shipper_id == null || shipper_id.Trim() == String.Empty)
                {
                    Message = "Shipper ID is not supplied.";
                    return 2;
                }
                else if (dt_payment_rcpt_dtl == null || dt_payment_rcpt_dtl.Rows.Count <= 0)
                {
                    Message = "Payment Receipt detail is not supplied.";
                    return 2;
                }
                else
                {
                    DataTable dtOrderDrivertruckdetails = new driverController().GetOrderDriverTruckDetails(dt_payment_rcpt_dtl.Rows[0]["load_inquiry_no"].ToString());

                    DS_driver_order_notifications ds_dr_order = new DS_driver_order_notifications();
                    DS_Payment_Receipt dS_Payment_Receipt = new DS_Payment_Receipt();
                    dS_Payment_Receipt.EnforceConstraints = false;

                    Document objDocument = new Document();
                    String payment_rcpt_no = null;
                    DateTime payment_rcpt_date = payment_date;
                    if (!objDocument.W_GetNextDocumentNoNew(ref command, "PR", created_by, created_host, ref payment_rcpt_no, ref Message))
                        return 2;

                    DS_Payment_Receipt.payment_rcpt_hdrRow hdrRow = dS_Payment_Receipt.payment_rcpt_hdr.Newpayment_rcpt_hdrRow();
                    hdrRow.payment_rcpt_no = payment_rcpt_no;
                    hdrRow.payment_rcpt_date = payment_rcpt_date;
                    hdrRow.payment_mode = payment_mode;
                    if (bank_code != null && bank_code.Trim() != String.Empty)
                        hdrRow.bank_code = bank_code;
                    else
                        hdrRow.Setbank_codeNull();
                    if (cheque_no != null && cheque_no.Trim() != String.Empty)
                        hdrRow.cheque_no = cheque_no;
                    else
                        hdrRow.Setcheque_noNull();
                    if (cheque_date.HasValue)
                        hdrRow.cheque_date = cheque_date.Value;
                    else
                        hdrRow.Setcheque_dateNull();
                    hdrRow.amount = amount;
                    hdrRow.shipper_id = shipper_id;
                    hdrRow.currency_code = Constant.CurrencyCode;
                    hdrRow.conversion_rate = 1;
                    hdrRow.base_currency_amount = amount;
                    if (remark != null && remark.Trim() != String.Empty)
                        hdrRow.remark = remark;
                    else
                        hdrRow.SetremarkNull();
                    hdrRow.cancel_flag = Constant.FLAG_N;
                    hdrRow.created_by = created_by;
                    hdrRow.created_date = DateTime.UtcNow;
                    hdrRow.created_host = created_host;
                    hdrRow.device_id = device_id;
                    hdrRow.device_type = device_type;
                    dS_Payment_Receipt.payment_rcpt_hdr.Addpayment_rcpt_hdrRow(hdrRow);

                    DS_Payment_Receipt.payment_rcpt_dtlRow dtlRow = null;
                    int sr_no = 1;
                    Decimal RemainingAmountToReceive = 0;
                    foreach (DataRow dr in dt_payment_rcpt_dtl.Rows)
                    {
                        dtlRow = dS_Payment_Receipt.payment_rcpt_dtl.Newpayment_rcpt_dtlRow();

                        dtlRow.payment_rcpt_no = payment_rcpt_no;
                        dtlRow.payment_rcpt_date = payment_rcpt_date;
                        dtlRow.sr_no = sr_no++;
                        if (dt_payment_rcpt_dtl.Columns.Contains("order_id") && dr["order_id"].ToString().Trim() != String.Empty)
                            dtlRow.order_id = dr["order_id"].ToString();
                        else
                            dtlRow.Setorder_idNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty)
                            dtlRow.load_inquiry_no = dr["load_inquiry_no"].ToString();
                        else
                            dtlRow.Setload_inquiry_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("shippingdatetime") && dr["shippingdatetime"].ToString().Trim() != String.Empty)
                            dtlRow.shippingdatetime = Convert.ToDateTime(dr["shippingdatetime"]);
                        else
                            dtlRow.SetshippingdatetimeNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_no") && dr["ref_doc_no"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_no = dr["ref_doc_no"].ToString();
                        else
                            dtlRow.Setref_doc_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_date") && dr["ref_doc_date"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_date = Convert.ToDateTime(dr["ref_doc_date"]);
                        else
                            dtlRow.Setref_doc_dateNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("adjusted_amt") && dr["adjusted_amt"].ToString().Trim() != String.Empty)
                            dtlRow.adjusted_amt = Convert.ToDecimal(dr["adjusted_amt"]);
                        else
                        {
                            Message = "Adjusted Amount is not supplied.";
                            return 2;
                        }
                        //update orders.rem_amt_to_receive
                        if (dt_payment_rcpt_dtl.Columns.Contains("order_id") && dr["order_id"].ToString().Trim() != String.Empty &&
                            dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty &&
                            dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty && dr["match_unmatch"].ToString().Trim().ToUpper() == Constant.PaymentMatch)
                        {
                            if (UpdateRemainingAmountToReceive_orders(ref command, dtlRow.order_id, hdrRow.shipper_id, dtlRow.load_inquiry_no, dtlRow.adjusted_amt, ref RemainingAmountToReceive, created_by, created_host, device_id, device_type, ref Message) == 1)
                                dtlRow.remaining_amt = RemainingAmountToReceive;
                            else
                                return 2;

                            if (UpdateRemainingAmountToReceive_postloadInquiry(ref command, dtlRow.order_id, hdrRow.shipper_id, dtlRow.load_inquiry_no, dtlRow.adjusted_amt, ref RemainingAmountToReceive, created_by, created_host, device_id, device_type, ref Message) == 1)
                                dtlRow.remaining_amt = RemainingAmountToReceive;
                            else
                                return 2;

                            #region update status in order_driver_truck_details

                            if (dt_payment_rcpt_dtl.Rows[0]["rate_type_flag"].ToString() == "P")
                            {
                                Document objdoc = new Document();
                                BLReturnObject objBLobj = new BLReturnObject();

                                if (dtOrderDrivertruckdetails != null)
                                {
                                    for (int i = 0; i < dtOrderDrivertruckdetails.Rows.Count; i++)
                                    {

                                        ds_dr_order.EnforceConstraints = false;
                                        ds_dr_order.order_driver_truck_details.ImportRow(dtOrderDrivertruckdetails.Rows[i]);
                                        ds_dr_order.order_driver_truck_details[i].totalamount_shipper = RemainingAmountToReceive;
                                        ds_dr_order.EnforceConstraints = true;


                                    }
                                }

                                BLGeneralUtil.UpdateTableInfo objUpdateTable = BLGeneralUtil.UpdateTable(ref command, ds_dr_order.order_driver_truck_details, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                                if (!objUpdateTable.Status || objUpdateTable.TotalRowsAffected != ds_dr_order.order_driver_truck_details.Rows.Count)
                                {
                                    Message = "An error occurred while insert data in payment_rcpt_hdr. " + objUpdateTable.ErrorMessage;
                                    return 2;
                                }

                            }
                            #endregion

                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("db_cr") && dr["db_cr"].ToString().Trim() != String.Empty)
                            dtlRow.db_cr = dr["db_cr"].ToString();
                        else
                        {
                            Message = "db_cr is not supplied.";
                            return 2;
                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty)
                            dtlRow.match_unmatch = dr["match_unmatch"].ToString();
                        else
                        {
                            Message = "match_unmatch is not supplied.";
                            return 2;
                        }
                        if (payment_mode == "C")
                        {
                            if (dt_payment_rcpt_dtl.Columns.Contains("payment_rcv_by") && dr["payment_rcv_by"].ToString().Trim() != String.Empty)
                                dtlRow.payment_rcv_by = dr["payment_rcv_by"].ToString();
                            else
                            {
                                Message = "Payment receiver name not supplied.";
                                return 2;
                            }
                        }

                        dtlRow.currency_code = Constant.CurrencyCode;
                        dtlRow.conversion_rate = 1;
                        dtlRow.base_currency_amount = dtlRow.adjusted_amt;
                        if (dt_payment_rcpt_dtl.Columns.Contains("line_narration") && dr["line_narration"].ToString().Trim() != String.Empty)
                            dtlRow.line_narration = dr["line_narration"].ToString();
                        else
                            dtlRow.Setline_narrationNull();
                        dtlRow.cancel_flag = Constant.FLAG_N;
                        dtlRow.created_by = created_by;
                        dtlRow.created_date = DateTime.UtcNow;
                        dtlRow.created_host = created_host;
                        dtlRow.device_id = device_id;
                        dtlRow.device_type = device_type;
                        dS_Payment_Receipt.payment_rcpt_dtl.Addpayment_rcpt_dtlRow(dtlRow);

                    }

                    try
                    {
                        dS_Payment_Receipt.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                        return 2;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        return 2;
                    }


                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_hdr, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_hdr.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_hdr. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_dtl, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_dtl.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_dtl. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    Message = "Payment Receipt data saved successfully.";
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public Byte SaveAddonPaymentReceipt(ref IDbCommand command, DateTime payment_date, String payment_mode, String bank_code, String cheque_no, DateTime? cheque_date, Decimal amount, String shipper_id, String remark, DataTable dt_payment_rcpt_dtl, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                if (payment_mode == null || payment_mode.Trim() == String.Empty)
                {
                    Message = "Payment Mode is not supplied.";
                    return 2;
                }
                else if (dt_payment_rcpt_dtl == null || dt_payment_rcpt_dtl.Rows.Count <= 0)
                {
                    Message = "Payment Receipt detail is not supplied.";
                    return 2;
                }
                else
                {
                    DataTable dtOrderDrivertruckdetails = new driverController().GetOrderDriverTruckDetails(dt_payment_rcpt_dtl.Rows[0]["load_inquiry_no"].ToString());

                    DS_driver_order_notifications ds_dr_order = new DS_driver_order_notifications();
                    DS_Payment_Receipt dS_Payment_Receipt = new DS_Payment_Receipt();
                    dS_Payment_Receipt.EnforceConstraints = false;

                    Document objDocument = new Document();
                    String payment_rcpt_no = null;
                    DateTime payment_rcpt_date = payment_date;
                    if (!objDocument.W_GetNextDocumentNoNew(ref command, "PR", created_by, created_host, ref payment_rcpt_no, ref Message))
                        return 2;

                    DS_Payment_Receipt.payment_rcpt_hdrRow hdrRow = dS_Payment_Receipt.payment_rcpt_hdr.Newpayment_rcpt_hdrRow();
                    hdrRow.payment_rcpt_no = payment_rcpt_no;
                    hdrRow.payment_rcpt_date = payment_rcpt_date;
                    hdrRow.payment_mode = payment_mode;
                    if (bank_code != null && bank_code.Trim() != String.Empty)
                        hdrRow.bank_code = bank_code;
                    else
                        hdrRow.Setbank_codeNull();
                    if (cheque_no != null && cheque_no.Trim() != String.Empty)
                        hdrRow.cheque_no = cheque_no;
                    else
                        hdrRow.Setcheque_noNull();
                    if (cheque_date.HasValue)
                        hdrRow.cheque_date = cheque_date.Value;
                    else
                        hdrRow.Setcheque_dateNull();
                    hdrRow.amount = amount;
                    hdrRow.shipper_id = shipper_id;
                    hdrRow.currency_code = Constant.CurrencyCode;
                    hdrRow.conversion_rate = 1;
                    hdrRow.base_currency_amount = amount;
                    if (remark != null && remark.Trim() != String.Empty)
                        hdrRow.remark = remark;
                    else
                        hdrRow.SetremarkNull();
                    hdrRow.cancel_flag = Constant.FLAG_N;
                    hdrRow.created_by = created_by;
                    hdrRow.created_date = DateTime.UtcNow;
                    hdrRow.created_host = created_host;
                    hdrRow.device_id = device_id;
                    hdrRow.device_type = device_type;
                    dS_Payment_Receipt.payment_rcpt_hdr.Addpayment_rcpt_hdrRow(hdrRow);

                    DS_Payment_Receipt.payment_rcpt_dtlRow dtlRow = null;
                    int sr_no = 1;
                    Decimal RemainingAmountToReceive = 0;
                    foreach (DataRow dr in dt_payment_rcpt_dtl.Rows)
                    {
                        dtlRow = dS_Payment_Receipt.payment_rcpt_dtl.Newpayment_rcpt_dtlRow();

                        dtlRow.payment_rcpt_no = payment_rcpt_no;
                        dtlRow.payment_rcpt_date = payment_rcpt_date;
                        dtlRow.sr_no = sr_no++;
                        if (dt_payment_rcpt_dtl.Columns.Contains("order_id") && dr["order_id"].ToString().Trim() != String.Empty)
                            dtlRow.order_id = dr["order_id"].ToString();
                        else
                            dtlRow.Setorder_idNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty)
                            dtlRow.load_inquiry_no = dr["load_inquiry_no"].ToString();
                        else
                            dtlRow.Setload_inquiry_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("shippingdatetime") && dr["shippingdatetime"].ToString().Trim() != String.Empty)
                            dtlRow.shippingdatetime = Convert.ToDateTime(dr["shippingdatetime"]);
                        else
                            dtlRow.SetshippingdatetimeNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_no") && dr["ref_doc_no"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_no = dr["ref_doc_no"].ToString();
                        else
                            dtlRow.Setref_doc_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_date") && dr["ref_doc_date"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_date = Convert.ToDateTime(dr["ref_doc_date"]);
                        else
                            dtlRow.Setref_doc_dateNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("adjusted_amt") && dr["adjusted_amt"].ToString().Trim() != String.Empty)
                            dtlRow.adjusted_amt = Convert.ToDecimal(dr["adjusted_amt"]);
                        else
                        {
                            Message = "Adjusted Amount is not supplied.";
                            return 2;
                        }
                        //update orders.rem_amt_to_receive
                        if (dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty &&
                            dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty && dr["match_unmatch"].ToString().Trim().ToUpper() == Constant.PaymentMatch)
                        {
                            if (UpdateRemainingAmountToReceive_orderAddonService(ref command, dtlRow.load_inquiry_no, hdrRow.shipper_id, hdrRow.payment_mode, dtlRow.adjusted_amt, ref RemainingAmountToReceive, created_by, created_host, device_id, device_type, ref Message) == 1)
                                dtlRow.remaining_amt = RemainingAmountToReceive;
                            else
                                return 2;
                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("db_cr") && dr["db_cr"].ToString().Trim() != String.Empty)
                            dtlRow.db_cr = dr["db_cr"].ToString();
                        else
                        {
                            Message = "db_cr is not supplied.";
                            return 2;
                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty)
                            dtlRow.match_unmatch = dr["match_unmatch"].ToString();
                        else
                        {
                            Message = "match_unmatch is not supplied.";
                            return 2;
                        }
                        if (payment_mode == "C")
                        {
                            if (dt_payment_rcpt_dtl.Columns.Contains("payment_rcv_by") && dr["payment_rcv_by"].ToString().Trim() != String.Empty)
                                dtlRow.payment_rcv_by = dr["payment_rcv_by"].ToString();
                            else
                            {
                                Message = "Payment receiver name not supplied.";
                                return 2;
                            }
                        }

                        dtlRow.currency_code = Constant.CurrencyCode;
                        dtlRow.conversion_rate = 1;
                        dtlRow.base_currency_amount = dtlRow.adjusted_amt;
                        if (dt_payment_rcpt_dtl.Columns.Contains("line_narration") && dr["line_narration"].ToString().Trim() != String.Empty)
                            dtlRow.line_narration = dr["line_narration"].ToString();
                        else
                            dtlRow.Setline_narrationNull();
                        dtlRow.cancel_flag = Constant.FLAG_N;
                        dtlRow.created_by = created_by;
                        dtlRow.created_date = DateTime.UtcNow;
                        dtlRow.created_host = created_host;
                        dtlRow.device_id = device_id;
                        dtlRow.device_type = device_type;
                        dS_Payment_Receipt.payment_rcpt_dtl.Addpayment_rcpt_dtlRow(dtlRow);

                    }

                    try
                    {
                        dS_Payment_Receipt.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                        return 2;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        return 2;
                    }


                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_hdr, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_hdr.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_hdr. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_dtl, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_dtl.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_dtl. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    Message = "Payment Receipt data saved successfully.";
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public Byte SaveDubiGoodsPaymentReceipt(ref IDbCommand command, DateTime payment_date, String payment_mode, String bank_code, String cheque_no, DateTime? cheque_date, Decimal amount, String shipper_id, String remark, DataTable dt_payment_rcpt_dtl, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                if (payment_mode == null || payment_mode.Trim() == String.Empty)
                {
                    Message = "Payment Mode is not supplied.";
                    return 2;
                }
                else if (dt_payment_rcpt_dtl == null || dt_payment_rcpt_dtl.Rows.Count <= 0)
                {
                    Message = "Payment Receipt detail is not supplied.";
                    return 2;
                }
                else
                {

                    DS_driver_order_notifications ds_dr_order = new DS_driver_order_notifications();
                    DS_Payment_Receipt dS_Payment_Receipt = new DS_Payment_Receipt();
                    dS_Payment_Receipt.EnforceConstraints = false;

                    Document objDocument = new Document();
                    String payment_rcpt_no = null;
                    DateTime payment_rcpt_date = payment_date;
                    if (!objDocument.W_GetNextDocumentNoNew(ref command, "PR", created_by, created_host, ref payment_rcpt_no, ref Message))
                        return 2;

                    DS_Payment_Receipt.payment_rcpt_hdrRow hdrRow = dS_Payment_Receipt.payment_rcpt_hdr.Newpayment_rcpt_hdrRow();
                    hdrRow.payment_rcpt_no = payment_rcpt_no;
                    hdrRow.payment_rcpt_date = payment_rcpt_date;
                    hdrRow.payment_mode = payment_mode;
                    if (bank_code != null && bank_code.Trim() != String.Empty)
                        hdrRow.bank_code = bank_code;
                    else
                        hdrRow.Setbank_codeNull();
                    if (cheque_no != null && cheque_no.Trim() != String.Empty)
                        hdrRow.cheque_no = cheque_no;
                    else
                        hdrRow.Setcheque_noNull();
                    if (cheque_date.HasValue)
                        hdrRow.cheque_date = cheque_date.Value;
                    else
                        hdrRow.Setcheque_dateNull();
                    hdrRow.amount = amount;
                    hdrRow.shipper_id = shipper_id;
                    hdrRow.currency_code = Constant.CurrencyCode;
                    hdrRow.conversion_rate = 1;
                    hdrRow.base_currency_amount = amount;
                    if (remark != null && remark.Trim() != String.Empty)
                        hdrRow.remark = remark;
                    else
                        hdrRow.SetremarkNull();
                    hdrRow.cancel_flag = Constant.FLAG_N;
                    hdrRow.created_by = created_by;
                    hdrRow.created_date = DateTime.UtcNow;
                    hdrRow.created_host = created_host;
                    hdrRow.device_id = device_id;
                    hdrRow.device_type = device_type;
                    dS_Payment_Receipt.payment_rcpt_hdr.Addpayment_rcpt_hdrRow(hdrRow);

                    DS_Payment_Receipt.payment_rcpt_dtlRow dtlRow = null;
                    int sr_no = 1;
                    Decimal RemainingAmountToReceive = 0;
                    foreach (DataRow dr in dt_payment_rcpt_dtl.Rows)
                    {
                        dtlRow = dS_Payment_Receipt.payment_rcpt_dtl.Newpayment_rcpt_dtlRow();

                        dtlRow.payment_rcpt_no = payment_rcpt_no;
                        dtlRow.payment_rcpt_date = payment_rcpt_date;
                        dtlRow.sr_no = sr_no++;
                        if (dt_payment_rcpt_dtl.Columns.Contains("order_id") && dr["order_id"].ToString().Trim() != String.Empty)
                            dtlRow.order_id = dr["order_id"].ToString();
                        else
                            dtlRow.Setorder_idNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty)
                            dtlRow.load_inquiry_no = dr["load_inquiry_no"].ToString();
                        else
                            dtlRow.Setload_inquiry_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("shippingdatetime") && dr["shippingdatetime"].ToString().Trim() != String.Empty)
                            dtlRow.shippingdatetime = Convert.ToDateTime(dr["shippingdatetime"]);
                        else
                            dtlRow.SetshippingdatetimeNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_no") && dr["ref_doc_no"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_no = dr["ref_doc_no"].ToString();
                        else
                            dtlRow.Setref_doc_noNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("ref_doc_date") && dr["ref_doc_date"].ToString().Trim() != String.Empty)
                            dtlRow.ref_doc_date = Convert.ToDateTime(dr["ref_doc_date"]);
                        else
                            dtlRow.Setref_doc_dateNull();
                        if (dt_payment_rcpt_dtl.Columns.Contains("adjusted_amt") && dr["adjusted_amt"].ToString().Trim() != String.Empty)
                            dtlRow.adjusted_amt = Convert.ToDecimal(dr["adjusted_amt"]);
                        else
                        {
                            Message = "Adjusted Amount is not supplied.";
                            return 2;
                        }
                        //update orders.rem_amt_to_receive
                        if (dt_payment_rcpt_dtl.Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty &&
                            dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty && dr["match_unmatch"].ToString().Trim().ToUpper() == Constant.PaymentMatch)
                        {
                            if (UpdateRemainingAmountToReceive_Dubiordertbl(ref command, dtlRow.load_inquiry_no, hdrRow.shipper_id, hdrRow.payment_mode, dtlRow.adjusted_amt, ref RemainingAmountToReceive, created_by, created_host, device_id, device_type, ref Message) == 1)
                                dtlRow.remaining_amt = RemainingAmountToReceive;
                            else
                                return 2;
                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("db_cr") && dr["db_cr"].ToString().Trim() != String.Empty)
                            dtlRow.db_cr = dr["db_cr"].ToString();
                        else
                        {
                            Message = "db_cr is not supplied.";
                            return 2;
                        }
                        if (dt_payment_rcpt_dtl.Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty)
                            dtlRow.match_unmatch = dr["match_unmatch"].ToString();
                        else
                        {
                            Message = "match_unmatch is not supplied.";
                            return 2;
                        }
                        if (payment_mode == "C")
                        {
                            if (dt_payment_rcpt_dtl.Columns.Contains("payment_rcv_by") && dr["payment_rcv_by"].ToString().Trim() != String.Empty)
                                dtlRow.payment_rcv_by = dr["payment_rcv_by"].ToString();
                            else
                            {
                                Message = "Payment receiver name not supplied.";
                                return 2;
                            }
                        }

                        dtlRow.currency_code = Constant.CurrencyCode;
                        dtlRow.conversion_rate = 1;
                        dtlRow.base_currency_amount = dtlRow.adjusted_amt;
                        if (dt_payment_rcpt_dtl.Columns.Contains("line_narration") && dr["line_narration"].ToString().Trim() != String.Empty)
                            dtlRow.line_narration = dr["line_narration"].ToString();
                        else
                            dtlRow.Setline_narrationNull();
                        dtlRow.cancel_flag = Constant.FLAG_N;
                        dtlRow.created_by = created_by;
                        dtlRow.created_date = DateTime.UtcNow;
                        dtlRow.created_host = created_host;
                        dtlRow.device_id = device_id;
                        dtlRow.device_type = device_type;
                        dS_Payment_Receipt.payment_rcpt_dtl.Addpayment_rcpt_dtlRow(dtlRow);

                    }

                    try
                    {
                        dS_Payment_Receipt.EnforceConstraints = true;
                    }
                    catch (ConstraintException ce)
                    {
                        Message = ce.Message;
                        ServerLog.MgmtExceptionLog(ce.Message + Environment.NewLine + ce.StackTrace);
                        return 2;
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        return 2;
                    }


                    BLGeneralUtil.UpdateTableInfo objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_hdr, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_hdr.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_hdr. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    objUpdateTableInfo = BLGeneralUtil.UpdateTable(ref command, dS_Payment_Receipt.payment_rcpt_dtl, BLGeneralUtil.UpdateWhereMode.KeyColumnsOnly);
                    if (!objUpdateTableInfo.Status || objUpdateTableInfo.TotalRowsAffected != dS_Payment_Receipt.payment_rcpt_dtl.Rows.Count)
                    {
                        Message = "An error occurred while insert data in payment_rcpt_dtl. " + objUpdateTableInfo.ErrorMessage;
                        return 2;
                    }

                    Message = "Payment Receipt data saved successfully.";
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }


        private Byte UpdateRemainingAmountToReceive_orders(ref IDbCommand command, String order_id, String shipper_id, String load_inquiry_no, Decimal adjusted_amt, ref Decimal RemainingAmountToReceive, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  order_id, shipper_id, load_inquiry_no, Total_cost, rem_amt_to_receive, payment_status, payment_mode, 
                                           created_by, created_date, created_host, device_id, device_type, modified_by, modified_date, modified_host, modified_device_id, modified_device_type 
                                   FROM    orders
                                   WHERE   order_id = @order_id 
                                   AND     shipper_id = @shipper_id
                                   AND     load_inquiry_no = @load_inquiry_no ");

                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "orders");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["rem_amt_to_receive"].ToString().Trim() == String.Empty)
                    {
                        Message = "Unable to get Remaining Amount To Receive.";
                        return 2;
                    }
                    else
                    {
                        RemainingAmountToReceive = Convert.ToDecimal(ds.Tables[0].Rows[0]["rem_amt_to_receive"]);
                        if (RemainingAmountToReceive < adjusted_amt)
                        {
                            Message = "Remaining Amount To Receive must be greater than or equal to Adjusted Amount.  OR  May be payment taken against given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 2;
                        }
                        else
                        {
                            RemainingAmountToReceive = RemainingAmountToReceive - adjusted_amt;
                            command.Parameters.Clear();
                            StringBuilder SQLUpdate = new StringBuilder();
                            SQLUpdate.Append(@"UPDATE orders 
                                               SET    rem_amt_to_receive = @rem_amt_to_receive, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type 
                                               WHERE  order_id = @order_id 
                                               AND    shipper_id = @shipper_id
                                               AND    load_inquiry_no = @load_inquiry_no ");

                            command.CommandText = SQLUpdate.ToString();
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@rem_amt_to_receive", DbType.Decimal, RemainingAmountToReceive));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                            int NoOfRowsAffected = command.ExecuteNonQuery();
                            if (NoOfRowsAffected != 1)
                            {
                                Message = "Fail to update Remaining Amount To Receive for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                                return 2;
                            }
                            Message = "Remaining Amount To Receive updated successfully for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 1;
                        }
                    }
                }
                else
                {
                    Message = "Order detail is not found for Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
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

        private Byte UpdateRemainingAmountToReceive_orderAddonService(ref IDbCommand command, String Transaction_id, String shipper_id, String PaymentMode, Decimal adjusted_amt, ref Decimal RemainingAmountToReceive, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  *
                                   FROM    order_AddonService_details
                                   WHERE   Transaction_id = @Transaction_id ");

                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Transaction_id", DbType.String, Transaction_id));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "order_AddonService_details");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["rem_amt_to_receive"].ToString().Trim() == String.Empty)
                    {
                        Message = "Unable to get Remaining Amount To Receive.";
                        return 2;
                    }
                    else
                    {
                        RemainingAmountToReceive = Convert.ToDecimal(ds.Tables[0].Rows[0]["rem_amt_to_receive"]);
                        if (RemainingAmountToReceive < adjusted_amt)
                        {
                            Message = "Remaining Amount To Receive must be greater than or equal to Adjusted Amount.  OR  May be payment taken against given Order Id : " + Transaction_id;
                            return 2;
                        }
                        else
                        {
                            RemainingAmountToReceive = RemainingAmountToReceive - adjusted_amt;
                            command.Parameters.Clear();
                            StringBuilder SQLUpdate = new StringBuilder();
                            SQLUpdate.Append(@" UPDATE order_AddonService_details  SET    rem_amt_to_receive = @rem_amt_to_receive, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type ");
                            if (RemainingAmountToReceive == 0)
                                SQLUpdate.Append(",Payment_mode='" + PaymentMode + "',payment_status='" + Constant.FLAG_Y + "'");
                            SQLUpdate.Append(" WHERE Transaction_id = @Transaction_id ");

                            command.CommandText = SQLUpdate.ToString();
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@rem_amt_to_receive", DbType.Decimal, RemainingAmountToReceive));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@Transaction_id", DbType.String, Transaction_id));
                            int NoOfRowsAffected = command.ExecuteNonQuery();
                            if (NoOfRowsAffected != 1)
                            {
                                Message = "Fail to update Remaining Amount To Receive for given Transaction Id : " + Transaction_id;
                                return 2;
                            }
                            Message = "Remaining Amount To Receive updated successfully for given Transaction Id : " + Transaction_id;
                            return 1;
                        }
                    }
                }
                else
                {
                    Message = "Order detail is not found for Order Id : " + Transaction_id;
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

        private Byte UpdateRemainingAmountToReceive_Dubiordertbl(ref IDbCommand command, String Transaction_id, String shipper_id, String PaymentMode, Decimal adjusted_amt, ref Decimal RemainingAmountToReceive, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  *
                                   FROM    dubiz_goods_orders_details
                                   WHERE   Transaction_id = @Transaction_id ");

                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Transaction_id", DbType.String, Transaction_id));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "dubiz_goods_orders_details");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["rem_amt_to_receive"].ToString().Trim() == String.Empty)
                    {
                        Message = "Unable to get Remaining Amount To Receive.";
                        return 2;
                    }
                    else
                    {
                        RemainingAmountToReceive = Convert.ToDecimal(ds.Tables[0].Rows[0]["rem_amt_to_receive"]);
                        if (RemainingAmountToReceive < adjusted_amt)
                        {
                            Message = "Remaining Amount To Receive must be greater than or equal to Adjusted Amount.  OR  May be payment taken against given Order Id : " + Transaction_id;
                            return 2;
                        }
                        else
                        {
                            RemainingAmountToReceive = RemainingAmountToReceive - adjusted_amt;
                            command.Parameters.Clear();
                            StringBuilder SQLUpdate = new StringBuilder();
                            SQLUpdate.Append(@"UPDATE dubiz_goods_orders_details  
                                               SET    rem_amt_to_receive = @rem_amt_to_receive, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type ");
                            if (RemainingAmountToReceive == 0)
                                SQLUpdate.Append(",Payment_mode='" + PaymentMode + "',payment_status='" + Constant.FLAG_Y + "'");
                            SQLUpdate.Append(" WHERE Transaction_id = @Transaction_id ");

                            command.CommandText = SQLUpdate.ToString();
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@rem_amt_to_receive", DbType.Decimal, RemainingAmountToReceive));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@Transaction_id", DbType.String, Transaction_id));
                            int NoOfRowsAffected = command.ExecuteNonQuery();
                            if (NoOfRowsAffected != 1)
                            {
                                Message = "Fail to update Remaining Amount To Receive for given Transaction Id : " + Transaction_id;
                                return 2;
                            }
                            Message = "Remaining Amount To Receive updated successfully for given Transaction Id : " + Transaction_id;
                            return 1;
                        }
                    }
                }
                else
                {
                    Message = "Order detail is not found for Order Id : " + Transaction_id;
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


        private Byte UpdateRemainingAmountToReceive_postloadInquiry(ref IDbCommand command, String order_id, String shipper_id, String load_inquiry_no, Decimal adjusted_amt, ref Decimal RemainingAmountToReceive, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  shipper_id, load_inquiry_no, Total_cost, rem_amt_to_receive, payment_status, payment_mode, 
                                           created_by, created_date, created_host, device_id, device_type, modified_by, modified_date, modified_host, modified_device_id, modified_device_type 
                                   FROM    post_load_inquiry
                                   WHERE   shipper_id = @shipper_id
                                   AND     load_inquiry_no = @load_inquiry_no ");

                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "post_load_inquiry");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["rem_amt_to_receive"].ToString().Trim() == String.Empty)
                    {
                        Message = "Unable to get Remaining Amount To Receive.";
                        return 2;
                    }
                    else
                    {
                        RemainingAmountToReceive = Convert.ToDecimal(ds.Tables[0].Rows[0]["rem_amt_to_receive"]);
                        if (RemainingAmountToReceive < adjusted_amt)
                        {
                            Message = "Remaining Amount To Receive must be greater than or equal to Adjusted Amount.  OR  May be payment taken against given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 2;
                        }
                        else
                        {
                            RemainingAmountToReceive = RemainingAmountToReceive - adjusted_amt;
                            command.Parameters.Clear();
                            StringBuilder SQLUpdate = new StringBuilder();
                            SQLUpdate.Append(@"UPDATE post_load_inquiry 
                                               SET    rem_amt_to_receive = @rem_amt_to_receive, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type 
                                               WHERE  shipper_id = @shipper_id
                                               AND    load_inquiry_no = @load_inquiry_no ");

                            command.CommandText = SQLUpdate.ToString();
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@rem_amt_to_receive", DbType.Decimal, RemainingAmountToReceive));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                            int NoOfRowsAffected = command.ExecuteNonQuery();
                            if (NoOfRowsAffected != 1)
                            {
                                Message = "Fail to update Remaining Amount To Receive for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                                return 2;
                            }
                            Message = "Remaining Amount To Receive updated successfully for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 1;
                        }
                    }
                }
                else
                {
                    Message = "Order detail is not found for Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
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

        public Byte CancelPaymentReceiptOnOrderCancel(ref IDbCommand command, String order_id, String shipper_id, String load_inquiry_no, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                IDbDataAdapter adapter = DBObjectFactory.GetDataAdapterObject(command);
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  payment_rcpt_hdr.payment_rcpt_no, payment_rcpt_hdr.payment_rcpt_date, payment_rcpt_hdr.payment_mode, payment_rcpt_hdr.bank_code, 
                                           payment_rcpt_hdr.cheque_no, payment_rcpt_hdr.cheque_date, payment_rcpt_hdr.amount, payment_rcpt_hdr.shipper_id, payment_rcpt_dtl.order_id, 
                                           payment_rcpt_dtl.load_inquiry_no
                                   FROM    payment_rcpt_hdr 
                                   INNER JOIN payment_rcpt_dtl ON payment_rcpt_hdr.payment_rcpt_no = payment_rcpt_dtl.payment_rcpt_no 
                                   AND payment_rcpt_hdr.payment_rcpt_date = payment_rcpt_dtl.payment_rcpt_date 
                                   WHERE   (payment_rcpt_hdr.shipper_id = @shipper_id) 
                                   AND     (payment_rcpt_dtl.order_id = @order_id) 
                                   AND     (payment_rcpt_dtl.load_inquiry_no = @load_inquiry_no) 
                                   AND     (payment_rcpt_hdr.cancel_flag = 'N')");
                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Clear();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "payment_rcpt_hdr");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (CancelPaymentReceipt(ref command, ref adapter, dr["payment_rcpt_no"].ToString(), Convert.ToDateTime(dr["payment_rcpt_date"]), order_id, shipper_id, load_inquiry_no, created_by, created_host, device_id, device_type, ref Message) != 1)
                            return 2;
                    }

                    Message = "Payment Receipt Cancelled successfully for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                    return 1;
                }
                else
                {
                    Message = "There is no Payment Receipt against given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
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

        private Byte CancelPaymentReceipt(ref IDbCommand command, ref IDbDataAdapter adapter, String payment_rcpt_no, DateTime payment_rcpt_date, String order_id, String shipper_id, String load_inquiry_no, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  payment_rcpt_no, payment_rcpt_date, sr_no, order_id, load_inquiry_no, shippingdatetime, ref_doc_no, ref_doc_date, adjusted_amt, db_cr, match_unmatch, remaining_amt
                                   FROM    payment_rcpt_dtl 
                                   WHERE   (payment_rcpt_dtl.payment_rcpt_no = @payment_rcpt_no) 
                                   AND     (payment_rcpt_dtl.payment_rcpt_date = @payment_rcpt_date) 
                                   AND     (payment_rcpt_dtl.order_id = @order_id) 
                                   AND     (payment_rcpt_dtl.load_inquiry_no = @load_inquiry_no) ");
                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Clear();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@payment_rcpt_no", DbType.String, payment_rcpt_no));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@payment_rcpt_date", DbType.DateTime, payment_rcpt_date));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "payment_rcpt_dtl");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //update orders.rem_amt_to_receive
                        if (ds.Tables[0].Columns.Contains("order_id") && dr["order_id"].ToString().Trim() != String.Empty &&
                            ds.Tables[0].Columns.Contains("load_inquiry_no") && dr["load_inquiry_no"].ToString().Trim() != String.Empty &&
                            ds.Tables[0].Columns.Contains("match_unmatch") && dr["match_unmatch"].ToString().Trim() != String.Empty && dr["match_unmatch"].ToString().Trim().ToUpper() == Constant.PaymentMatch)
                        {
                            Decimal adjusted_amt = Convert.ToDecimal(dr["adjusted_amt"]);
                            if (UpdateRemainingAmountToReceiveOnCancel(ref command, ref adapter, order_id, shipper_id, load_inquiry_no, adjusted_amt, created_by, created_host, device_id, device_type, ref Message) != 1)
                                return 2;
                        }
                    }
                }
                else
                {
                    Message = "Payment Receipt Detail data not found for given Payment Receipt No : " + payment_rcpt_no + " and Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                    return 2;
                }

                return UpdatePaymentReceiptHeaderCancelFlag(ref command, payment_rcpt_no, payment_rcpt_date, shipper_id, created_by, created_host, device_id, device_type, ref Message);
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        private Byte UpdateRemainingAmountToReceiveOnCancel(ref IDbCommand command, ref IDbDataAdapter adapter, String order_id, String shipper_id, String load_inquiry_no, Decimal adjusted_amt, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                adapter.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  order_id, shipper_id, load_inquiry_no, Total_cost, rem_amt_to_receive, payment_status, payment_mode, 
                                           created_by, created_date, created_host, device_id, device_type, modified_by, modified_date, modified_host, modified_device_id, modified_device_type 
                                   FROM    orders
                                   WHERE   order_id = @order_id 
                                   AND     shipper_id = @shipper_id
                                   AND     load_inquiry_no = @load_inquiry_no ");

                adapter.SelectCommand.CommandText = SQLSelect.ToString();
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                adapter.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                adapter.TableMappings.Clear();
                adapter.TableMappings.Add("Table", "orders");
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["rem_amt_to_receive"].ToString().Trim() == String.Empty)
                    {
                        Message = "Unable to get Remaining Amount To Receive.";
                        return 2;
                    }
                    else
                    {
                        Decimal RemainingAmountToReceive = Convert.ToDecimal(ds.Tables[0].Rows[0]["rem_amt_to_receive"]);
                        Decimal Total_Cost = Convert.ToDecimal(ds.Tables[0].Rows[0]["Total_cost"]);
                        RemainingAmountToReceive = RemainingAmountToReceive + adjusted_amt;
                        if (RemainingAmountToReceive > Total_Cost)
                        {
                            Message = "Remaining Amount To Receive must be lest than or equal to Total Order Cost.  OR  May be payment receipt already cancelled against given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 2;
                        }
                        else
                        {
                            command.Parameters.Clear();
                            StringBuilder SQLUpdate = new StringBuilder();
                            SQLUpdate.Append(@"UPDATE orders 
                                            SET    rem_amt_to_receive = @rem_amt_to_receive, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type 
                                            WHERE  order_id = @order_id 
                                            AND    shipper_id = @shipper_id
                                            AND    load_inquiry_no = @load_inquiry_no ");

                            command.CommandText = SQLUpdate.ToString();
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@rem_amt_to_receive", DbType.Decimal, RemainingAmountToReceive));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@order_id", DbType.String, order_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                            command.Parameters.Add(DBObjectFactory.MakeParameter("@load_inquiry_no", DbType.String, load_inquiry_no));
                            int NoOfRowsAffected = command.ExecuteNonQuery();
                            if (NoOfRowsAffected != 1)
                            {
                                Message = "Fail to update Remaining Amount To Receive for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                                return 2;
                            }
                            Message = "Remaining Amount To Receive updated successfully for given Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
                            return 1;
                        }
                    }
                }
                else
                {
                    Message = "Order detail is not found for Order Id : " + order_id + " and Shipper Id : " + shipper_id + " and Load Inquiry No : " + load_inquiry_no;
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

        private Byte UpdatePaymentReceiptHeaderCancelFlag(ref IDbCommand command, String payment_rcpt_no, DateTime payment_rcpt_date, String shipper_id, String created_by, String created_host, String device_id, String device_type, ref String Message)
        {
            try
            {
                command.Parameters.Clear();
                StringBuilder SQLUpdate = new StringBuilder();
                SQLUpdate.Append(@"UPDATE payment_rcpt_hdr 
                                   SET    cancel_flag = @cancel_flag_new, modified_by = @modified_by, modified_date = @modified_date, modified_host = @modified_host, modified_device_id = @modified_device_id, modified_device_type = @modified_device_type 
                                   WHERE  payment_rcpt_no = @payment_rcpt_no 
                                   AND    payment_rcpt_date = @payment_rcpt_date 
                                   AND    shipper_id = @shipper_id 
                                   AND    cancel_flag = @cancel_flag_old ");

                command.CommandText = SQLUpdate.ToString();
                command.Parameters.Add(DBObjectFactory.MakeParameter("@cancel_flag_new", DbType.String, Constant.Flag_Yes));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_by", DbType.String, created_by));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_date", DbType.DateTime, DateTime.UtcNow));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_host", DbType.String, created_host));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_id", DbType.String, device_id));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@modified_device_type", DbType.String, device_type));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@payment_rcpt_no", DbType.String, payment_rcpt_no));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@payment_rcpt_date", DbType.DateTime, payment_rcpt_date));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                command.Parameters.Add(DBObjectFactory.MakeParameter("@cancel_flag_old", DbType.String, Constant.Flag_No));
                int NoOfRowsAffected = command.ExecuteNonQuery();
                if (NoOfRowsAffected != 1)
                {
                    Message = "Fail to update Payment Receipt Header cancel flag for given Payment Receipt No : " + payment_rcpt_no;
                    return 2;
                }
                Message = "Payment Receipt Header cancel flag updated successfully for given Payment Receipt No : " + payment_rcpt_no;
                return 1;
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return 2;
            }
        }

        public DataTable GetShipperOutStandingReport(String shipper_id, DateTime? from_date, DateTime? to_date, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  orders.shipper_id, user_mst.first_name + CASE WHEN user_mst.middle_name IS NOT NULL AND LTRIM(RTRIM(user_mst.middle_name)) <> '' THEN ' ' + user_mst.middle_name ELSE '' END 
                                                                                  + CASE WHEN user_mst.last_name IS NOT NULL AND LTRIM(RTRIM(user_mst.last_name)) <> '' THEN ' ' + user_mst.last_name ELSE '' END AS shipper_name, 
                                   user_mst.user_short_name AS shipper_short_name, orders.order_id, orders.load_inquiry_no, orders.shippingdatetime, orders.inquiry_source_addr, orders.order_completion_date, orders.Total_cost AS Amount, 
                                   ISNULL(orders.rem_amt_to_receive, 0) AS OutStandingDebit, 0 AS OutStandingCredit, 0 AS OutStanding, orders.payment_due_date AS DueDate, 
                                   CASE WHEN orders.payment_due_date IS NOT NULL AND orders.payment_due_date < GETDATE() THEN DATEDIFF(day, orders.payment_due_date, GETDATE()) ELSE 0 END AS NoOfDaysOverDue 
                                   FROM    orders 
                                   INNER JOIN user_mst ON orders.shipper_id = user_mst.unique_id
                                   WHERE   (ISNULL(orders.IsCancel, 'N') = @IsCancel) AND (ISNULL(orders.active_flag, 'Y') = @IsActive) 
                                   AND     (orders.status = @status) AND (orders.order_completion_date IS NOT NULL) AND (ISNULL(orders.rem_amt_to_receive, 0) > 0) ");
                if (shipper_id != null && shipper_id.Trim() != String.Empty)
                {
                    SQLSelect.Append(" AND  (orders.shipper_id = @shipper_id) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                }
                if (from_date.HasValue)
                {
                    SQLSelect.Append(" AND  (orders.shippingdatetime >= @from_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@from_date", DbType.DateTime, from_date.Value));
                }
                if (to_date.HasValue)
                {
                    SQLSelect.Append(" AND  (orders.shippingdatetime <= @to_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@to_date", DbType.DateTime, to_date.Value));
                }
                SQLSelect.Append(" ORDER BY orders.shipper_id, orders.shippingdatetime ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsCancel", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsActive", DbType.String, Constant.FLAG_Y));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@status", DbType.String, Constant.ORDER_COMPLETED));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "ShipperOutStanding");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Shipper OutStanding data retrieved successfully.";
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        dr["OutStanding"] = Convert.ToDecimal(dr["OutStandingDebit"]) - Convert.ToDecimal(dr["OutStandingCredit"]);
                    return ds.Tables[0];
                }
                else
                {
                    Message = "There is no OutStanding pending for Shipper.";
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

        public DataTable GetShipperLedgerReport(String shipper_id, DateTime? from_date, DateTime? to_date, ref String Message)
        {
            try
            {
                #region Get Shipper Post Order
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  orders.shipper_id, user_mst.first_name + CASE WHEN user_mst.middle_name IS NOT NULL AND LTRIM(RTRIM(user_mst.middle_name)) <> '' THEN ' ' + user_mst.middle_name ELSE '' END 
                                                                                  + CASE WHEN user_mst.last_name IS NOT NULL AND LTRIM(RTRIM(user_mst.last_name)) <> '' THEN ' ' + user_mst.last_name ELSE '' END AS shipper_name, 
                                           user_mst.user_short_name AS shipper_short_name, orders.shippingdatetime AS DocDate, orders.order_id AS DocNo, orders.load_inquiry_no AS RefDocNo, 'Post Order' AS DocumentName, 
                                           orders.Total_cost AS DebitAmount, 0 AS CreditAmount, CAST(NULL AS varchar) AS AdjustedDetail
                                   FROM    orders 
                                   INNER JOIN user_mst ON orders.shipper_id = user_mst.unique_id
                                   WHERE   (orders.IsCancel = @IsCancel) AND (orders.active_flag = @IsActive) ");
                //AND     (orders.status = @status) AND (orders.order_completion_date IS NOT NULL) ");
                if (shipper_id != null && shipper_id.Trim() != String.Empty)
                {
                    SQLSelect.Append(" AND  (orders.shipper_id = @shipper_id) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                }
                if (from_date.HasValue)
                {
                    SQLSelect.Append(" AND  (orders.shippingdatetime >= @from_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@from_date", DbType.DateTime, from_date.Value));
                }
                if (to_date.HasValue)
                {
                    SQLSelect.Append(" AND  (orders.shippingdatetime <= @to_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@to_date", DbType.DateTime, to_date.Value));
                }
                SQLSelect.Append(" ORDER BY orders.shipper_id, orders.shippingdatetime ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsCancel", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsActive", DbType.String, Constant.FLAG_Y));
                //DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@status", DbType.String, Constant.ORDER_COMPLETED));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "ShipperLedgerPostOrder");
                DataSet dsPostOrder = new DataSet();
                DBDataAdpterObject.Fill(dsPostOrder);
                #endregion

                #region Get Shipper Payment Receipt
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT  payment_rcpt_hdr.shipper_id, user_mst.first_name + CASE WHEN user_mst.middle_name IS NOT NULL AND LTRIM(RTRIM(user_mst.middle_name)) <> '' THEN ' ' + user_mst.middle_name ELSE '' END 
                                                                                            + CASE WHEN user_mst.last_name IS NOT NULL AND LTRIM(RTRIM(user_mst.last_name)) <> '' THEN ' ' + user_mst.last_name ELSE '' END AS shipper_name, 
                                           user_mst.user_short_name AS shipper_short_name, payment_rcpt_hdr.payment_rcpt_date AS DocDate, payment_rcpt_hdr.payment_rcpt_no AS DocNo, CAST(NULL AS varchar) AS RefDocNo, 'Payment Receipt' AS DocumentName, 
                                           0 AS DebitAmount, payment_rcpt_hdr.amount AS CreditAmount, 
                                          'Post Order - ' + payment_rcpt_dtl.order_id + ' - ' + payment_rcpt_dtl.load_inquiry_no + ' - ' + CAST(payment_rcpt_dtl.shippingdatetime AS varchar) + ' - ' + CAST(payment_rcpt_dtl.adjusted_amt AS varchar) + ' - ' + payment_rcpt_hdr.payment_mode AS AdjustedDetail 
                                   FROM    payment_rcpt_hdr 
                                   INNER JOIN payment_rcpt_dtl ON payment_rcpt_hdr.payment_rcpt_no = payment_rcpt_dtl.payment_rcpt_no AND payment_rcpt_hdr.payment_rcpt_date = payment_rcpt_dtl.payment_rcpt_date 
                                   INNER JOIN user_mst ON payment_rcpt_hdr.shipper_id = user_mst.unique_id
                                   WHERE   (payment_rcpt_hdr.cancel_flag = @IsCancel) AND (payment_rcpt_dtl.cancel_flag = @IsCancel) ");
                if (shipper_id != null && shipper_id.Trim() != String.Empty)
                {
                    SQLSelect.Append(" AND  (payment_rcpt_hdr.shipper_id = @shipper_id) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@shipper_id", DbType.String, shipper_id));
                }
                if (from_date.HasValue)
                {
                    SQLSelect.Append(" AND  (payment_rcpt_hdr.payment_rcpt_date >= @from_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@from_date", DbType.DateTime, from_date.Value));
                }
                if (to_date.HasValue)
                {
                    SQLSelect.Append(" AND  (payment_rcpt_hdr.payment_rcpt_date <= @to_date) ");
                    DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@to_date", DbType.DateTime, to_date.Value));
                }
                SQLSelect.Append(" ORDER BY payment_rcpt_hdr.shipper_id, payment_rcpt_hdr.payment_rcpt_date ");
                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@IsCancel", DbType.String, Constant.FLAG_N));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "ShipperLedgerPaymentReceipt");
                DataSet dsPaymentReceipt = new DataSet();
                DBDataAdpterObject.Fill(dsPaymentReceipt);
                #endregion

                #region Merge Post Order & Payment Receipt
                DataTable dtShipperLedger = null;
                if (dtShipperLedger == null && dsPostOrder.Tables.Count > 0)
                    dtShipperLedger = dsPostOrder.Tables[0].Copy();
                if (dtShipperLedger == null && dsPaymentReceipt.Tables.Count > 0)
                    dtShipperLedger = dsPaymentReceipt.Tables[0].Copy();
                else
                {
                    foreach (DataRow dr in dsPaymentReceipt.Tables[0].Rows)
                        dtShipperLedger.ImportRow(dr);
                }

                if (dtShipperLedger != null && dtShipperLedger.Rows.Count > 0)
                {
                    dtShipperLedger.DefaultView.Sort = "shipper_id, DocDate, DocNo, DocumentName";
                    dtShipperLedger = dtShipperLedger.DefaultView.ToTable();
                    Message = "Shipper Ledger data retrieved successfully.";
                    return dtShipperLedger;
                }
                else
                {
                    Message = "There is no Ledger data found for Shipper.";
                    return null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ServerLog.MgmtExceptionLog(ex.Message + Environment.NewLine + ex.StackTrace);
                Message = ex.Message;
                return null;
            }
        }

        public DataTable GetPestControlServiceRate(String PestControl_type_code, String SizeTypeCode, String goods_type_flag, String rate_type_flag, DateTime OrderDate, ref Decimal PestControlRate, ref Decimal PestControlDiscount, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM PestControlRateDetails 
                                   WHERE  1=1 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    SizeTypeCode = @SizeTypeCode ");

                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@PestControl_type_code", DbType.String, PestControl_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "PestControlRateDetails");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "PestControl Rate detail data retrieved successfully for PestControl_type_code : " + PestControl_type_code;
                    PestControlRate = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    PestControlDiscount = Convert.ToDecimal(ds.Tables[0].Rows[0]["Discount"]);
                    return ds.Tables[0];
                }
                else
                {
                    Message = "PestControl Rate detail data is not found for Packing_type_code : " + PestControl_type_code;
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

        public DataTable GetPaitingServiceRate(String Painting_type_code, String SizeTypeCode, String goods_type_flag, String rate_type_flag, DateTime OrderDate, ref Decimal PaintingRate, ref Decimal PaintingDiscount, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM PaitingRateDetails 
                                   WHERE  1=1 
                                   AND    rate_type_flag = @rate_type_flag  AND    SizeTypeCode = @SizeTypeCode ");

                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Painting_type_code", DbType.String, Painting_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));

                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "PaitingRateDetails");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Packing Rate detail data retrieved successfully for Packing_type_code : " + Painting_type_code;
                    PaintingRate = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    PaintingDiscount = Convert.ToDecimal(ds.Tables[0].Rows[0]["Discount"]);
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Packing Rate detail data is not found for Packing_type_code : " + Painting_type_code;
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

        public DataTable GetCleaningServiceRate(String Cleaning_type_code, String SizeTypeCode, String goods_type_flag, String rate_type_flag, DateTime OrderDate, ref Decimal CleaningRate, ref Decimal CleaningDiscount, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@"SELECT * FROM CleaningRateDetails 
                                   WHERE  1=1 
                                   AND    rate_type_flag = @rate_type_flag 
                                   AND    SizeTypeCode = @SizeTypeCode ");

                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@Cleaning_type_code", DbType.String, Cleaning_type_code));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@goods_type_flag", DbType.String, goods_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@rate_type_flag", DbType.String, rate_type_flag));
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@SizeTypeCode", DbType.String, SizeTypeCode));
                DBDataAdpterObject.TableMappings.Clear();
                DBDataAdpterObject.TableMappings.Add("Table", "CleaningRateDetails");
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = "Cleaning Rate detail data retrieved successfully for Cleaning_type_code : " + Cleaning_type_code;
                    CleaningRate = Convert.ToDecimal(ds.Tables[0].Rows[0]["rate"]);
                    CleaningDiscount = Convert.ToDecimal(ds.Tables[0].Rows[0]["Discount"]);
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Cleaning Rate detail data is not found for Cleaning_type_code : " + Cleaning_type_code;
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

        public DataTable GetDubiGoodsRate(String ItemCode, ref String Message)
        {
            try
            {
                DBDataAdpterObject.SelectCommand.Parameters.Clear();
                StringBuilder SQLSelect = new StringBuilder();
                SQLSelect.Append(@" select dubiz_goods_item_mst.Item_desc,* from dubiz_goods_itemrate_details
                                    join dubiz_goods_item_mst on dubiz_goods_item_mst.item_code=dubiz_goods_itemrate_details.item_code
                                      where dubiz_goods_itemrate_details.item_code=@ItemCode ");

                DBDataAdpterObject.SelectCommand.CommandText = SQLSelect.ToString();
                DBDataAdpterObject.SelectCommand.Parameters.Add(DBObjectFactory.MakeParameter("@ItemCode", DbType.String, ItemCode));
                DBDataAdpterObject.TableMappings.Clear();
                DataSet ds = new DataSet();
                DBDataAdpterObject.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Message = " Item data retrieved successfully.";
                    return ds.Tables[0];
                }
                else
                {
                    Message = "Data not found for items ";
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

