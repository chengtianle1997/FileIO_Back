using libMetroTunnelDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileIO_Back
{

    /// DeviceHandler 提供所选轨交线路和日期对应的有采集数据的设备列表
    /// success return: {"data":{"devices":[{"deviceId":"1","name":"WYNTELLI-GJA001"}]}}
    /// fail return: null

    public class DeviceHandler : IHttpHandler
    {
        public class RootJson
        {
            public DataJson data;
        }
        public class DataJson
        {
            public List<DevicesJson> devices;
        }
        public class DevicesJson
        {
            public string deviceId;
            public string name;
            public DevicesJson(string _deviceId, string _name)
            {
                deviceId = _deviceId;
                name = _name;
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
            catch(System.Exception)
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
            RootJson rootJson = new RootJson();
            rootJson.data = new DataJson();
            rootJson.data.devices = new List<DevicesJson>();
            if (detectRecords.Count < 1)
            {
                context.Response.Write(null);
                context.Response.End();
                return;
            }
            // 清除重复设备名
            Dictionary<string, string> device_dict = new Dictionary<string, string>();
            for (int i = 0; i < detectRecords.Count; i++)
            {
                if (!device_dict.ContainsKey(detectRecords[i].DeviceID))
                {
                    List<DetectDevice> detectDevices = new List<DetectDevice>();
                    try
                    {
                        Database.QueryDetectDevice(ref detectDevices, detectRecords[i].DeviceID);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    device_dict.Add(detectRecords[i].DeviceID, detectDevices[0].DetectDeviceName);
                }
            }
            // 遍历非重复设备
            foreach (string deviceId in device_dict.Keys)
            {
                rootJson.data.devices.Add(new DevicesJson(deviceId, device_dict[deviceId]));
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