using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace IMSWebApi.Common
{
    public class SendEmailHelper
    {
        private SmtpConnection smtpConn = new SmtpConnection();
        public void sendMail(EmailProperties emailProperties)
        {
            try
            {
                SmtpConfig Configuration = new SmtpConfig();
                Configuration.SmtpAddress = emailProperties.SmtpAddress;
                Configuration.EmailFrom = emailProperties.EmailFrom;
                Configuration.Password = emailProperties.Password;
                Configuration.EnableSSL = emailProperties.EnableSSL;
                Configuration.PortNumber = emailProperties.PortNumber > 0 ? emailProperties.PortNumber : Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPortNo"]);
                MailMessage smail = new MailMessage();
                if (IsValidEmail(emailProperties.EmailFrom))
                {
                    List<Uri> uriList = this.FetchLinksFromSource(emailProperties.Body);
                    if (uriList.Count != 0)
                    {
                        int url = 0;
                        AlternateView alternateViewFromString = AlternateView.CreateAlternateViewFromString(Regex.Replace(emailProperties.Body, "<img[^>]*?src\\s*=\\s*[\"']?([^'\" >]+?)[ '\"][^>]*?>", (MatchEvaluator)(m => "<img src=\"cid:Pic" + url++.ToString() + "\"/>"), RegexOptions.IgnoreCase), (Encoding)null, "text/html");
                        for (int index = 0; index < uriList.Count; ++index)
                        {
                            LinkedResource linkedResource = new LinkedResource(uriList[index].AbsolutePath, "image/jpeg");
                            linkedResource.ContentId = "Pic" + index.ToString();
                            alternateViewFromString.LinkedResources.Add(linkedResource);
                        }
                        smail.AlternateViews.Add(alternateViewFromString);
                    }
                    else
                        smail.Body = emailProperties.Body;
                    smail.From = new MailAddress(emailProperties.EmailFrom);
                    foreach (string str in emailProperties.EmailTo)
                    {
                        if (IsValidEmail(str))
                            smail.To.Add(str);
                        //else
                        //    this.log.CreateLog("Invalid email Id(To):" + str, (Exception)null);
                    }
                    foreach (string str in emailProperties.EmailCC)
                    {
                        if (IsValidEmail(str))
                            smail.CC.Add(str);
                        //else
                        //    this.log.CreateLog("Invalid email Id(CC):" + str, (Exception)null);
                    }
                    foreach (string str in emailProperties.EmailBCC)
                    {
                        if (IsValidEmail(str))
                            smail.Bcc.Add(str);
                        //else
                        //    this.log.CreateLog("Invalid email Id(BCC):" + str, (Exception)null);
                    }
                    smail.Subject = emailProperties.Subject;
                    smail.IsBodyHtml = emailProperties.isBodyHtml;
                    foreach (string fileName in emailProperties.Attachment)
                        smail.Attachments.Add(new Attachment(fileName));
                    this.smtpConn.SetConfiguration(Configuration);
                    this.smtpConn.SendMail(smail);
                }
                //else
                //    this.log.CreateLog("Sender's email id is invalid(EmailFrom):" + emailProperties.EmailFrom, (Exception)null);
            }
            catch (Exception ex)
            {
                //this.log.CreateLog("Exception:", ex);
            }
        }

        private List<Uri> FetchLinksFromSource(string htmlSource)
        {
            List<Uri> uriList = new List<Uri>();
            string pattern = "<img[^>]*?src\\s*=\\s*[\"']?([^'\" >]+?)[ '\"][^>]*?>";
            foreach (Match match in Regex.Matches(htmlSource, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                string uriString = match.Groups[1].Value;
                uriList.Add(new Uri(uriString));
            }
            return uriList;
        }

        private static bool IsValidEmail(string email)
        {
            Regex regex = new Regex("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$");
            if (!string.IsNullOrEmpty(email))
                return regex.IsMatch(email);
            return false;
        }
    }

    public class EmailProperties
    {
        private List<string> _EmailTo = new List<string>();
        private List<string> _EmailCC = new List<string>();
        private List<string> _EmailBCC = new List<string>();
        private List<string> _Attachment = new List<string>();

        public string SmtpAddress { get; set; }

        public int PortNumber { get; set; }

        public bool EnableSSL { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Password { get; set; }

        public bool isBodyHtml { get; set; }

        public string EmailFrom { get; set; }

        public List<string> EmailTo
        {
            get
            {
                return this._EmailTo;
            }
        }

        public List<string> EmailCC
        {
            get
            {
                return this._EmailCC;
            }
        }

        public List<string> EmailBCC
        {
            get
            {
                return this._EmailBCC;
            }
        }

        public List<string> Attachment
        {
            get
            {
                return this._Attachment;
            }
        }

    }

    public class SmtpConnection
    {
        private SmtpClient smtpClient = new SmtpClient();

        public void SetConfiguration(SmtpConfig Configuration)
        {
            this.smtpClient.Host = Configuration.SmtpAddress;
            this.smtpClient.Port = Configuration.PortNumber;
            this.smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential(Configuration.EmailFrom, Configuration.Password);
            this.smtpClient.EnableSsl = Configuration.EnableSSL;
        }

        public void SendMail(MailMessage smail)
        {
            this.smtpClient.Send(smail);
        }
    }

    public class SmtpConfig
    {
        public string SmtpAddress { get; set; }

        public int PortNumber { get; set; }

        public bool EnableSSL { get; set; }

        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }

        public string Password { get; set; }
    }
}