using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;
using trukkerUAE.Models;

namespace trukkerUAE.Controllers
{
    public class GetTruckLocationByGPSController : ApiController
    {
        // GET api/gettrucklocationbygps
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/gettrucklocationbygps/5
        public string Get(int id)
        {
            return "value";
        }

        public string TraceTruckLocationAndCalculateDistanceByRao([FromBody]owner ownerDetails)
        {

            int distance = 0;
            string tostring = string.Empty;
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);

            String query = "SELECT * FROM gps_current";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            DataTable dtGPS = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dtGPS);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Dictionary<string, string>> dataRows = new List<Dictionary<string, string>>(); // will contain datarows as dictionary objects
            decimal long2 = 0;
            decimal lati2 = 0;
            foreach (DataRow VDataRow in dtGPS.Rows)
            {
                var Row = new Dictionary<string, string>(); // DataRow as key-value pairs where key=columnName and value=fieldValue 
                foreach (DataColumn Column in dtGPS.Columns)
                {

                    Row.Add(Column.ColumnName, VDataRow[Column].ToString());

                }
                dataRows.Add(Row);
            }

            //System.Xml.XmlDocument doc = new XmlDocument();

            //doc.Load("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + "23.0257900  ,72.5872600 " + "&sensor=false");

            for (int j = 0; j <= dtGPS.Rows.Count - 1; j++)
            {
                // string url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + "ahmedabad" + "&destination=" + "delhi" + "&sensor=false";
                //string requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + from + "&alternatives=false&units=imperial&destination=" + to + "&sensor=false";
                long2 = Convert.ToDecimal(dtGPS.Rows[j]["GPScurrent_latitude"]);
                lati2 = Convert.ToDecimal(dtGPS.Rows[j]["GPScurrent_longitude"]);
                tostring = long2.ToString() + ',' + lati2.ToString() + ' ';
                string requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + "23.0257900  ,72.5872600 " + "&alternatives=false&metric=kilometers&destination=" + tostring + "&sensor=false";
                string requestur2 = @"http://maps.googleapis.com/maps/api/geocode/json?latlng= " + tostring + "&sensor=true";

                //  requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + "23.0257900  ,72.5872600 " + "&alternatives=false&metric=kilometers&destination=" + "22.170424,71.668427 " + "&sensor=false";

                string content = fileGetContents(requesturl);
                string content2 = fileGetContents(requestur2);
                JObject o = JObject.Parse(content);
                JObject n = JObject.Parse(content2);

                distance = (int)o.SelectToken("routes[0].legs[0].distance.value");

                string city1 = "";
                string city2 = "";


                var temp_locality = n.SelectTokens("$.results[0].address_components[?(@.types[0] == 'locality' && @.types[1] == 'political')]");
                city1 = temp_locality.ToList()[0]["long_name"].ToString();


                for (int i = 0; i < n.SelectTokens("results[0].address_components").ToList()[0].Count(); i++)
                {
                    var temp1 = n.SelectTokens("results[0].address_components[" + i + "].types[0]").ToList()[0].ToString();

                    if (temp1 == "locality")
                    {
                        var temp2 = n.SelectTokens("results[0].address_components[" + i + "].types[1]").ToList()[0].ToString();

                        if (temp2 == "political")
                        {
                            city2 = n.SelectTokens("results[0].address_components[" + i + "].long_name").ToList()[0].ToString();
                            break;
                        }
                    }
                }

                string name = n.SelectTokens("results[0].address_components[2].long_name").ToList()[0].ToString();

                (dtGPS.Rows[j]["GPScurrent_distance"]) = Convert.ToDecimal(distance);
                (dtGPS.Rows[j]["GPScurrent_city"]) = (city2).ToString();

            }

            return null;

        }

        protected string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }

    }
}
