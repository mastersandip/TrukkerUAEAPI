using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using BLL.Utilities;
using System.Configuration;

namespace trukkerUAE.Controllers
{
    public class FileUploadController : ServerBase
    {
        // GET api/fileupload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/fileupload/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/fileupload
        public void Post([FromBody]string value)
        {
        }

        // PUT api/fileupload/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/fileupload/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        public HttpResponseMessage UploadFile()
        {
            HttpResponseMessage result = null;
            try
            {
                string path = "";
                var filePath = ""; int upldcount = 0; string filenm = "";
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest["requesttype"] == "profile")
                {
                    path =  ConfigurationManager.AppSettings["ProfilePic"].ToString();
                }
                Dictionary<string, string> array1 = new Dictionary<string, string>();

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        if (postedFile.FileName.Trim() != "")
                        {
                            //var filePath = HttpContext.Current.Server.MapPath("~/imageFolder/DrvPhoto/" + postedFile.FileName);
                            filenm = postedFile.FileName;
                            filenm = filenm.ToString().Substring(0, filenm.IndexOf(".")) + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + filenm.ToString().Substring(filenm.IndexOf("."), (filenm.ToString().Length - filenm.IndexOf(".")));
                            filePath = HttpContext.Current.Server.MapPath(path + filenm);
                            postedFile.SaveAs(filePath);
                            docfiles.Add(filePath);
                            array1[file] = path + filenm;
                            upldcount++;
                        }
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, filenm);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Log(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
                //ServerLog.Log(ex.InnerException.ToString());
                result = Request.CreateResponse(HttpStatusCode.BadRequest,ex.Message);
                return result;
            }
            return result;
        }
    }
}
