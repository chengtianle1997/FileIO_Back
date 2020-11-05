using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web;
using libMetroTunnelDB;
using Newtonsoft.Json;

namespace FileIO_Back
{
    /// <summary>
    /// DisplayDataHandler 的摘要说明
    /// {"data":{"records":["{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},{"dpNo":"39820092","value":["39820092","0","-0"],"itemStyle":{"color":"blue"}},
    /// </summary>
    public class DisplayDataHandler : IHttpHandler
    {
        public class RootJson
        {
            public DataJson data;
        }
        public class DataJson
        {
            public List<string> records;
        }

        public void ProcessRequest(HttpContext context)
        {
            int catchId = 0, queryStart = 0, queryNum = 0;
            try
            {
                catchId = Convert.ToInt32(context.Request.Params["catchId"].ToLower());
                queryStart = Convert.ToInt32(context.Request.Params["queryStart"].ToLower());
                queryNum = Convert.ToInt32(context.Request.Params["queryNum"].ToLower());
            }
            catch (System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            List<DataDisp> dataDisps = new List<DataDisp>();
            MetroTunnelDB Database = null;
            try
            {
                Database = new MetroTunnelDB();
                Database.QueryDataDisp(ref dataDisps, catchId, queryStart, queryNum);
            }
            catch (System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            if (dataDisps.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.records = new List<string>();
            for (int i = 0; i < dataDisps.Count; i++)
            {
                rootJson.data.records.Add(dataDisps[i].JsonString.Substring(1, dataDisps[i].JsonString.Length - 2));
            }
            string rootJsonStr = JsonConvert.SerializeObject(rootJson).Replace("\\", "");
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