using libMetroTunnelDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FileIO_Back
{
    /// DetectRecordHandler 根据线路号和日期提供检测记录号及检测记录信息
    /// success: {"data":{"records":[{"catchId":"1","deviceId":"1","totallength":"0","recordsCount":"0"}]}}
    /// fail: null
    public class DetectRecordHandler : IHttpHandler
    {
        public class RootJson
        {
            public DataJson data;
        }
        public class DataJson
        {
            public List<RecordsJson> records;
        }
        public class RecordsJson
        {
            public string catchId { set; get; }
            public string deviceId { set; get; }
            public string totallength { set; get; }
            public string recordsCount { set; get; }
            public RecordsJson(int _catchId, string _deviceId, float _totallength, int _recordsCount)
            {
                catchId = _catchId.ToString();
                deviceId = _deviceId;
                totallength = _totallength.ToString("0.##");
                recordsCount = _recordsCount.ToString();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            int lineId = 0;
            DateTime date, end_date;
            try
            {
                lineId = Convert.ToInt32(context.Request.Params["lineId"].ToLower());
                date = Convert.ToDateTime(context.Request.Params["date"].ToLower());
                end_date = date.AddDays(1);
            }
            catch (System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            List<DetectRecord> detectRecords = new List<DetectRecord>();
            MetroTunnelDB Database = null;
            try
            {
                Database = new MetroTunnelDB();
                Database.QueryDetectRecord(ref detectRecords, lineId, date, end_date);
            }
            catch (System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            if (detectRecords.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.records = new List<RecordsJson>();
            for (int i = 0; i < detectRecords.Count; i++)
            {
                List<DataOverview> dataOverviews = new List<DataOverview>();
                int recordsCount = 0;
                try
                {
                    Database.QueryDataOverview(ref dataOverviews, (int)(detectRecords[i].RecordID));
                    recordsCount = dataOverviews.Count();
                }
                catch(System.Exception)
                {
                    ;
                }
                rootJson.data.records.Add(new RecordsJson((int)detectRecords[i].RecordID, detectRecords[i].DeviceID, detectRecords[i].Length, recordsCount));
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