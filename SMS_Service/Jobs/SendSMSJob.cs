using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Quartz;

namespace SMS_Service.Jobs
{
    public class SendSMSJob : IJob
    {
        private static readonly HttpClient client = new HttpClient();

        private static Encoding encoding33 = Encoding.UTF8;

        public void Execute(IJobExecutionContext context)
        {
            Log4netHelper.Loger.Info($"Listening for messages IJobExecutionContext." + DateTime.Now.ToShortDateString());

            //JobDataMap dataMap = context.JobDetail.JobDataMap;
            //string content = dataMap.GetString("jobSays");

            //Console.WriteLine("作业执行，jobSays:" + content);
            var url = $@"http://localhost:65298/api/pc/Transport/TransportSendSMSJob";

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("EventId", "627871354");
            dic.Add("ThreshholdMinutes", "30");
            dic.Add("MinuteSpan", "100000");

            var respStr = Post(url, new { EventId = 627871354, ThreshholdMinutes = 30, MinuteSpan = 100000 });

            //var respStr = Post(url, dic);

            Log4netHelper.Loger.Info($"respStr = {respStr} \n");
        }


        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json;";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// json 格式 Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj">对象可以的，会进行 JSON.SerializeAllField()</param>
        /// <param name="timeoutSeconds">TimeOut 秒数</param>
        /// <returns></returns>
        public static string Post(string url, object obj = null, int timeoutSeconds = 15)
        {


            if (string.IsNullOrWhiteSpace(url))
                return "";

            Encoding encode = Encoding.UTF8;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.ContentType = "application/json;charset=utf-8";
            myRequest.Method = "POST";// HttpUtil.UrlMethod.POST.ToString();
            if (timeoutSeconds < 3)
                timeoutSeconds = 3;

            byte[] bs = null;
            myRequest.Timeout = timeoutSeconds * 1000;
            if (obj != null)
            {
                bs = encoding33.GetBytes(JsonConvert.SerializeObject(obj, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local }));

                myRequest.ContentLength = bs.Length;
            }
            else
                myRequest.ContentLength = 0;


            using (Stream reqStream = myRequest.GetRequestStream())
            {
                if (obj != null)
                    reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding33))
                    {
                        var responseData = reader.ReadToEnd().ToString();
                        return responseData;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}
