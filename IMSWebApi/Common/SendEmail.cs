using IMSWebApi.Models;
using ReusableEmailComponent;
using System.Text;
using System.Web;

namespace IMSWebApi.Common
{
    public class SendEmail
    {

        public static string _smtpAddress = System.Configuration.ConfigurationSettings.AppSettings["SMTPHost"];
        public static string _emailFrom = System.Configuration.ConfigurationSettings.AppSettings["SMTPUserName"];
        public static string _password = System.Configuration.ConfigurationSettings.AppSettings["SMTPPassword"];

        public void email(MstUser result,string fileName)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            if (result.userName != null)
            {
                sbEmailDetails = sbEmailDetails.Replace("@UserName", result.userName);
            }
            else
            {
                sbEmailDetails = sbEmailDetails.Replace("@UserName", "");
            }
            if (result.password != null)
            {
                sbEmailDetails = sbEmailDetails.Replace("@Password", result.password);
            }
            else
            {
                sbEmailDetails = sbEmailDetails.Replace("@Password", "");
            }

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(result.email);
            objEmail.Subject = "Welcome to SoftZone!!!";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail); 
        }
    }
}