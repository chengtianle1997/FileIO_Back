using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using libMetroTunnelDB;
using Newtonsoft.Json;

namespace FileIO_Back
{
    /// DetectTimeHandler 提供所选轨交线路所有有采集数据的日期列表
    /// success: {"data":{"dates":["2020-4-11","2020-10-16"]}}
    /// fail: null
    public class DetectTimeHandler : IHttpHandler
    {
        public class RootJson
        {
            public DataJson data;
        }
        public class DataJson
        {
            public List<string> dates;
        }

        public void ProcessRequest(HttpContext context)
        {
            int lineId = Convert.ToInt32(context.Request.Params["lineId"].ToLower());
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            List<DetectRecord> detectRecords = new List<DetectRecord>();
            try
            {
                MetroTunnelDB Database = new MetroTunnelDB();
                Database.QueryDetectRecord(ref detectRecords, lineId);
            }
            catch(System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.dates = new List<string>();
            if (detectRecords.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            for (int i = 0; i < detectRecords.Count; i++)
            {
                rootJson.data.dates.Add(detectRecords[i].DetectTime.ToString("yyyy-MM-dd"));
            }
            string rootJsonStr = JsonConvert.SerializeObject(rootJson);
            context.Response.Write(rootJsonStr);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}