using IMSWebApi.Models;
using ReusableEmailComponent;
using System.Text;
using System.Web;
using System.Configuration;
using IMSWebApi.ViewModel;

namespace IMSWebApi.Common
{
    public class SendEmail
    {

        public static string _smtpAddress =ConfigurationManager.AppSettings["SMTPHost"];
        public static string _emailFrom = ConfigurationManager.AppSettings["SMTPUserName"];
        public static string _password = ConfigurationManager.AppSettings["SMTPPassword"];

        public void email(MstUser result,string originalPassword,string fileName,bool isResetPassword)
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
                sbEmailDetails = sbEmailDetails.Replace("@Password", originalPassword);
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
            objEmail.Subject = isResetPassword ? "Reset Your Password" : "Welcome to SoftZone!!!";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail); 
        }

        public void notificationForPO(VMTrnPurchaseOrder purchaseOrder, string fileName,MstUser loggedInUser,string adminEmail)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            sbEmailDetails = sbEmailDetails.Replace("@user",loggedInUser.userName);
            sbEmailDetails = sbEmailDetails.Replace("@courierMode", purchaseOrder.courierMode);
            sbEmailDetails = sbEmailDetails.Replace("@supplierName", purchaseOrder.supplierName);
            sbEmailDetails = sbEmailDetails.Replace("@shippingAddress", purchaseOrder.shippingAddress);
            sbEmailDetails = sbEmailDetails.Replace("@courierName", purchaseOrder.courierName);
            
            string rows = "";
         
            foreach (var poItem in purchaseOrder.TrnPurchaseOrderItems)
            {
                //string serialOrSize = poItem.shadeId != null ? poItem.serialno : poItem.fomSizeId != null ? poItem.size : poItem.matSizeId != null ? poItem.size : poItem.accessoryId;
                string serialOrSize = poItem.shadeId != null ? poItem.serialno : poItem.size;
                rows += "<tr><td>" + poItem.categoryName + "</td><td> " + poItem.collectionName + "</td><td> " + serialOrSize + "</td><td> " 
                    + poItem.orderQuantity + "</td><td> " + poItem.orderType+
                        "</td><td> " + poItem.rate + "</td><td> " + poItem.amountWithGST + "</td> </tr>";  
            }  
            sbEmailDetails = sbEmailDetails.Replace("@rows",rows );

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(adminEmail);
            objEmail.Subject = "Purchase Order Generated";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }
    }
}