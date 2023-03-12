using AngularEshop.DataLayer.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Utilities
{
   public interface IMailSender
    {
        void SendPlainTextGmail(string recipientEmail, string recipientName);
        void SendHtmlGmail(User user, string subject, string body);
        void SendHtmlwithAttachmentGmail(string recipientEmail, string recipientName);
        void SendPlainTextSendgrid(string recipientEmail, string recipientName);
        void SendHtmlSendgrid(string recipientEmail, string recipientName);
        void SendHtmlwithAttachmentSendgrid(string recipientEmail, string recipientName);
        void SendSendgridBulk(IEnumerable<string> recipientEmails);


    }
}
