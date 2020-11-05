using libMetroTunnelDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FileIO_Back
{

    /// DetectDataHandler 根据数据组编号提供采集的数据
    /// success:
    /// {"data":{"records":[
    /// {"id":"0","distance":"39820092","longAxis":"0","horizontalAxis":"0","shortAxis":"0","shortAxisAngle":"0","hasConstr":"False","hasCrack":"False"},
    /// {"id":"1","distance":"39820144","longAxis":"0","horizontalAxis":"0","shortAxis":"0","shortAxisAngle":"0","hasConstr":"False","hasCrack":"False"},
    /// {"id":"2","distance":"39820196","longAxis":"0","horizontalAxis":"0","shortAxis":"0","shortAxisAngle":"0","hasConstr":"False","hasCrack":"False"}]}}
    /// fail: null
    
    public class DetectDataHandler : IHttpHandler
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
            public string id { set; get; }
            public string distance { set; get; }
            public string longAxis { set; get; }
            public string horizontalAxis { set; get; }
            public string shortAxis { set; get; }
            public string shortAxisAngle { set; get; }
            public string hasConstr { set; get; }
            public string hasCrack { set; get; }

            public RecordsJson(int _id, double _distance, float _longAxis, float _horizontalAxis, 
                float _shortAxis, float _shortAxisAngle, bool _hasConstr, bool _hasCrack)
            {
                id = _id.ToString();
                distance = _distance.ToString();
                longAxis = _longAxis.ToString();
                horizontalAxis = _horizontalAxis.ToString();
                shortAxis = _shortAxis.ToString();
                shortAxisAngle = _shortAxisAngle.ToString();
                hasConstr = _hasConstr.ToString();
                hasCrack = _hasCrack.ToString();
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            int catchId = 0;           
            int queryStart = 0, queryNum = 500;
            try
            {
                catchId = Convert.ToInt32(context.Request.Params["catchId"].ToLower());               
                queryStart = Convert.ToInt32(context.Request.Params["queryStart"].ToLower());
                queryNum = Convert.ToInt32(context.Request.Params["queryNum"].ToLower());
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
            for(int i = 0; i < dataOverviews.Count; i++)
            {
                rootJson.data.records.Add(new RecordsJson(queryStart + i, dataOverviews[i].Distance,dataOverviews[i].LongAxis, dataOverviews[i].HorizontalAxis,
                    dataOverviews[i].ShortAxis, dataOverviews[i].Rotation, dataOverviews[i].Constriction, dataOverviews[i].Crack));
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