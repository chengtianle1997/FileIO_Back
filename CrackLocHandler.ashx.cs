using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using libMetroTunnelDB;
using Newtonsoft.Json;

namespace FileIO_Back
{

    /// CrackLocHandler 给距离控件提供裂缝数据
    /// success: {"data":{"recordsCount":"1","records":[{"dpNo":"39820092","distance":"39820092"}]}}
    /// fail: null

    public class CrackLocHandler : IHttpHandler
    {
        public class RootJson
        {
            public DataJson data;
        }
        public class DataJson
        {
            public string recordsCount;
            public List<RecordsJson> records;
        }
        public class RecordsJson
        {
            public string dpNo { set; get; }
            public string distance { set; get; }
            public RecordsJson(double _dpNo, double _distance)
            {
                dpNo = _dpNo.ToString();
                distance = _distance.ToString();
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            int catchId = 0;
            int queryStart = 0, queryNum = 500;
            try
            {
                catchId = Convert.ToInt32(context.Request.Params["catchId"].ToLower());
            }
            catch
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            List<DataOverview> dataOverviews = new List<DataOverview>();
            MetroTunnelDB Database = null;
            try
            {
                Database = new MetroTunnelDB();
                Database.QueryDataOverview(ref dataOverviews, catchId, queryStart, queryNum);
            }
            catch (System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            if (dataOverviews.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.records = new List<RecordsJson>();
            int records_count = 0;
            for (int i = 0; i < dataOverviews.Count; i++)
            {
                if (dataOverviews[i].Crack)
                {
                    rootJson.data.records.Add(new RecordsJson(dataOverviews[i].Distance, dataOverviews[i].Distance));
                    records_count++;
                }
            }
            rootJson.data.recordsCount = records_count.ToString();
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