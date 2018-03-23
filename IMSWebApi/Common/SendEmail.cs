using IMSWebApi.Models;
using ReusableEmailComponent;
using System.Text;
using System.Web;
using System.Configuration;
using IMSWebApi.ViewModel;
using System.Collections.Generic;
using IMSWebApi.ViewModel.SalesInvoice;

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
                string accessoryCode = poItem.accessoryId != null ? poItem.accessoryName : "";
                string collectionName = poItem.collectionId != null ? poItem.collectionName : "";
                rows += "<tr><td>" + poItem.categoryName 
                    + "</td><td> " + collectionName 
                    + "</td><td> " + serialOrSize 
                    + "</td><td>" + accessoryCode 
                    + "</td><td>" + poItem.orderQuantity 
                    + "</td><td> " + poItem.orderType
                    + "</td><td> " + poItem.rate 
                    + "</td><td> " + poItem.amountWithGST 
                    + "</td> </tr>";  
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

        public void notificationForSO(VMTrnSaleOrder saleOrder, string fileName, MstUser loggedInUser, string adminEmail)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            string shippingAddress = saleOrder.shippingAddress.addressLine1 + ", " + (saleOrder.shippingAddress.addressLine2 != null ? saleOrder.shippingAddress.addressLine2 + ", " : "" )+
                    saleOrder.shippingAddress.city + ", " + saleOrder.shippingAddress.state + "Pincode : " + saleOrder.shippingAddress.pin;

            sbEmailDetails = sbEmailDetails.Replace("@user", loggedInUser.userName);
            sbEmailDetails = sbEmailDetails.Replace("@courierMode", saleOrder.courierMode);
            sbEmailDetails = sbEmailDetails.Replace("@customerName", saleOrder.customerName);
            sbEmailDetails = sbEmailDetails.Replace("@shippingAddress", shippingAddress);
            sbEmailDetails = sbEmailDetails.Replace("@courierName", saleOrder.courierName);

            string rows = "";

            foreach (var soItem in saleOrder.TrnSaleOrderItems)
            {
                string serialOrSize = soItem.shadeId != null ? soItem.serialno : soItem.size;
                string accessoryCode = soItem.accessoryId != null ? soItem.accessoryName : "";
                rows += "<tr><td>" + soItem.categoryName 
                    + "</td><td> " + soItem.collectionName 
                    + "</td><td> " + serialOrSize 
                    + "</td><td> " + accessoryCode
                    + "</td><td> " + soItem.orderQuantity 
                    + "</td><td> " + soItem.orderType
                    + "</td><td> " + soItem.rate 
                    + "</td><td> " + soItem.amountWithGST 
                    + "</td> </tr>";
            }
            sbEmailDetails = sbEmailDetails.Replace("@rows", rows);

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(adminEmail);
            objEmail.Subject = "Sale Order Generated";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }

        public void notifySupplierForPO(VMTrnPurchaseOrder purchaseOrder, string fileName,string supplierEmail)
        {
            string shippingAddress = purchaseOrder.shippingAddress!=null ? purchaseOrder.shippingAddress : purchaseOrder.MstCompanyLocation.addressLine1 + " " + purchaseOrder.MstCompanyLocation.addressLine2 +
                                                   "," + purchaseOrder.MstCompanyLocation.city + ", " + purchaseOrder.MstCompanyLocation.state + " PINCODE - "
                                                   + purchaseOrder.MstCompanyLocation.pin;

            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            sbEmailDetails = sbEmailDetails.Replace("@courierMode", purchaseOrder.courierMode);
            sbEmailDetails = sbEmailDetails.Replace("@supplierName", purchaseOrder.supplierName!=null ? purchaseOrder.supplierName : purchaseOrder.MstSupplier.name);
            sbEmailDetails = sbEmailDetails.Replace("@shippingAddress", shippingAddress);
            sbEmailDetails = sbEmailDetails.Replace("@courierName", purchaseOrder.courierName!=null ? purchaseOrder.courierName : purchaseOrder.MstCourier.name);

            string rows = "";

            foreach (var poItem in purchaseOrder.TrnPurchaseOrderItems)
            {
                string serialOrSize = poItem.serialno != null && poItem.serialno != ""? poItem.serialno :
                    poItem.size != null && poItem.size != "" ? poItem.size :
                    poItem.shadeId != null ? poItem.MstFWRShade.serialNumber + "(" + poItem.MstFWRShade.shadeCode + ")" :
                    poItem.matSizeId != null ? poItem.MstMatSize.sizeCode + " (" + poItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + poItem.MstMatSize.MstQuality.qualityCode + ")" :
                    poItem.fomSizeId != null ? poItem.MstFomSize.itemCode : poItem.matSizeCode;
                string accessoryCode = poItem.accessoryName != null && poItem.accessoryName != ""? poItem.accessoryName :
                    poItem.accessoryId != null ? poItem.MstAccessory.itemCode : "";
                string collectionName = poItem.collectionName != null && poItem.collectionName != ""? poItem.collectionName :
                    poItem.collectionId != null ? poItem.MstCollection.collectionName + " (" + poItem.MstCollection.MstSupplier.code + ")": "";
                string categoryName = poItem.categoryName != null && poItem.categoryName != ""? poItem.categoryName : poItem.MstCategory.name;
                rows += "<tr><td>" + categoryName
                    + "</td><td> " + collectionName
                    + "</td><td> " + serialOrSize
                    + "</td><td> " + accessoryCode
                    + "</td><td> " + poItem.orderQuantity
                    + "</td><td> " + poItem.orderType
                    + "</td><td> " + poItem.rate
                    + "</td><td> " + poItem.amountWithGST
                    + "</td></tr>";
            }
            sbEmailDetails = sbEmailDetails.Replace("@rows", rows);

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(supplierEmail);
            objEmail.Subject = "Purchase Order Generated";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }

        public void approvedSONotificationForCustomer(VMTrnSaleOrder saleOrder, string fileName,string customerEmail)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            string shippingAddress = saleOrder.shippingAddress != null ? (saleOrder.shippingAddress.addressLine1 + ", " + (saleOrder.shippingAddress.addressLine2 != null ? saleOrder.shippingAddress.addressLine2 + ", " : "") +
                saleOrder.shippingAddress.city + ", " + saleOrder.shippingAddress.state + "Pincode : " + saleOrder.shippingAddress.pin) :
                saleOrder.MstCustomerAddress != null ? (saleOrder.MstCustomerAddress.addressLine1 + ", " + (saleOrder.MstCustomerAddress.addressLine2 != null ? saleOrder.MstCustomerAddress.addressLine2 + ", " : "") +
                    saleOrder.MstCustomerAddress.city + ", " + saleOrder.MstCustomerAddress.state + "Pincode : " + saleOrder.MstCustomerAddress.pin) : null;

            sbEmailDetails = sbEmailDetails.Replace("@courierMode", saleOrder.courierMode);
            sbEmailDetails = sbEmailDetails.Replace("@customerName", saleOrder.customerName!=null ? saleOrder.customerName : saleOrder.MstCustomer.name);
            sbEmailDetails = sbEmailDetails.Replace("@shippingAddress", shippingAddress);
            sbEmailDetails = sbEmailDetails.Replace("@courierName", saleOrder.courierName!=null ? saleOrder.courierName : saleOrder.MstCourier.name);

            string rows = "";

            foreach (var soItem in saleOrder.TrnSaleOrderItems)
            {
                string serialOrSize = soItem.serialno != null && soItem.serialno != "" ? soItem.serialno :
                    soItem.size != null && soItem.size != "" ? soItem.size :
                   soItem.shadeId != null ? soItem.MstFWRShade.serialNumber + "(" + soItem.MstFWRShade.shadeCode + ")" :
                   soItem.matSizeId != null ? soItem.MstMatSize.sizeCode + " (" + soItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + soItem.MstMatSize.MstQuality.qualityCode + ")" :
                   soItem.fomSizeId != null ? soItem.MstFomSize.itemCode : soItem.sizeCode;
                string accessoryCode = soItem.accessoryName != null && soItem.accessoryName != ""? soItem.accessoryName : 
                    soItem.accessoryId != null ? soItem.MstAccessory.itemCode : "";
                string categoryName = soItem.categoryName != null && soItem.categoryName != "" ? soItem.categoryName : 
                    soItem.categoryId !=null ? soItem.MstCategory.code : "";
                string collectionName = soItem.collectionName != null && soItem.collectionName != "" ? soItem.collectionName :
                    soItem.collectionId != null ? soItem.MstCollection.collectionName : "";
                rows += "<tr><td>" + categoryName 
                    + "</td><td> " + collectionName 
                    + "</td><td> " + serialOrSize 
                    + "</td><td> " + accessoryCode 
                    + "</td><td> " + soItem.orderQuantity 
                    + "</td><td> " + soItem.orderType
                    + "</td><td> " + soItem.rate 
                    + "</td><td> " + soItem.amountWithGST 
                    + "</td> </tr>";
            }
            sbEmailDetails = sbEmailDetails.Replace("@rows", rows);

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(customerEmail);
            objEmail.Subject = "Sale Order Approved";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }

        public void cancelledSONotificationForCustomer(VMTrnSaleOrder saleOrder, string fileName)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            string shippingAddress = saleOrder.shippingAddress != null ?  saleOrder.shippingAddress.addressLine1 + ", " + (saleOrder.shippingAddress.addressLine2 != null ? saleOrder.shippingAddress.addressLine2 + ", " : "" )+
                saleOrder.shippingAddress.city + ", " + saleOrder.shippingAddress.state + "Pincode : " + saleOrder.shippingAddress.pin :
                    saleOrder.MstCustomerAddress != null ? (saleOrder.MstCustomerAddress.addressLine1 + ", " + (saleOrder.MstCustomerAddress.addressLine2 != null ? saleOrder.MstCustomerAddress.addressLine2 + ", " : "") +
                    saleOrder.MstCustomerAddress.city + ", " + saleOrder.MstCustomerAddress.state + "Pincode : " + saleOrder.MstCustomerAddress.pin) : null;

            sbEmailDetails = sbEmailDetails.Replace("@courierMode", saleOrder.courierMode);
            sbEmailDetails = sbEmailDetails.Replace("@customerName", saleOrder.MstCustomer.name);
            sbEmailDetails = sbEmailDetails.Replace("@shippingAddress", shippingAddress);
            sbEmailDetails = sbEmailDetails.Replace("@courierName", saleOrder.MstCourier.name);

            string rows = "";

            foreach (var soItem in saleOrder.TrnSaleOrderItems)
            {
                string serialOrSize = soItem.shadeId != null ? soItem.MstFWRShade.serialNumber + "(" + soItem.MstFWRShade.shadeCode + ")" :
                   soItem.matSizeId != null ? soItem.MstMatSize.sizeCode + " (" + soItem.MstMatSize.MstMatThickNess.thicknessCode + "-" + soItem.MstMatSize.MstQuality.qualityCode + ")" :
                   soItem.fomSizeId != null ? soItem.MstFomSize.itemCode : "";
                string accessoryCode = soItem.accessoryId != null ? soItem.MstAccessory.itemCode : "";
                string collectionName = soItem.accessoryId != null ? "" : soItem.MstCollection.collectionName;
                rows += "<tr><td>" + soItem.MstCategory.code
                    + "</td><td> " + collectionName
                    + "</td><td> " + serialOrSize
                    + "</td><td> " + accessoryCode
                    + "</td><td> " + soItem.orderQuantity
                    + "</td><td> " + soItem.orderType
                    + "</td><td> " + soItem.rate
                    + "</td><td> " + soItem.amountWithGST
                    + "</td> </tr>";
            }
            sbEmailDetails = sbEmailDetails.Replace("@rows", rows);

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(saleOrder.MstCustomer.email);
            objEmail.Subject = "Sale Order Cancelled";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }

        public void notificationForPendingGIN(string grnNumber, string fileName, List<string> ginNumbers, string adminEmail)
        {
            StringBuilder sbEmailDetails = new StringBuilder();
            sbEmailDetails.AppendLine(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\EmailTemplate\" + fileName + ".html")));

            sbEmailDetails = sbEmailDetails.Replace("@grnNumber", grnNumber);
            
            string rows = "";

            foreach (string ginNumber in ginNumbers)
            {
                rows += "<tr><td>" + ginNumber + "</td> </tr>";
            }
            sbEmailDetails = sbEmailDetails.Replace("@rows", rows);

            EmailProperties objEmail = new EmailProperties();
            objEmail.SmtpAddress = _smtpAddress;
            objEmail.EmailFrom = _emailFrom;
            objEmail.Password = _password;
            objEmail.EmailTo.Add(adminEmail);
            objEmail.Subject = "GRN Created";
            objEmail.EnableSSL = true;
            objEmail.Body = sbEmailDetails.ToString();
            objEmail.isBodyHtml = true;
            objEmail.EnableSSL = true;
            ReusableEmailComponent.DAOFactoryProvider.GetEmailDao().sendMail(objEmail);
        }
    }
}