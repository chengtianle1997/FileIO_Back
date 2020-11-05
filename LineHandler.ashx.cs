using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using libMetroTunnelDB;
using Newtonsoft.Json;

namespace FileIO_Back
{
    public class RootJson
    {
        public DataJson data;
    }
    public class DataJson
    {
        public List<LinesJson> lines;
    }

    public class LinesJson
    {
        public string lineId { set; get; }
        public string name { set; get; }

        public LinesJson(string _LineNum, string _LineName)
        {
            lineId = _LineNum;
            name = _LineName;
        }
    }

    /// LineHandler 提供轨交线路的数据
    /// success: {"data":{"lines":[{"lineId":"1","name":"苏州地铁一号线"}]}}
    /// fail: null
    public class LineHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Clear();
            List<Line> lines = new List<Line>();
            try
            {
                MetroTunnelDB Database = new MetroTunnelDB();
                Database.QueryLine(ref lines);
            }
            catch(System.Exception)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.lines = new List<LinesJson>();
            if (lines.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            for (int i = 0; i < lines.Count; i++)
            {
                rootJson.data.lines.Add(new LinesJson(lines[i].LineNumber, lines[i].LineName));
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