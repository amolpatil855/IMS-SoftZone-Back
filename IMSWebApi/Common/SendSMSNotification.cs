using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace IMSWebApi.Common
{
    public class SendSMSNotification
    {
        public static string _smsApiUrl = ConfigurationManager.AppSettings["SMSApiUrl"];
        public static string _textLocalApiKey = ConfigurationManager.AppSettings["TextLocalAPIKey"];
        public static string _senderCode = ConfigurationManager.AppSettings["SMSSender"];

        public string SendSMS(string messageContent, string receiver)
        {
            string result = string.Empty;
            String message = HttpUtility.UrlEncode(messageContent.Replace("\\n", System.Environment.NewLine));
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues(_smsApiUrl, new NameValueCollection()
                {
                {"apikey" , _textLocalApiKey},
                {"numbers" , "91" + receiver},
                {"message" , message},
                {"sender" , _senderCode}
                });
                result = System.Text.Encoding.UTF8.GetString(response);
            }
            return result;
        }
    }
}