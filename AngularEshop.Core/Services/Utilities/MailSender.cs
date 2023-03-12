using AngularEshop.DataLayer.Entities.Account;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AngularEshop.Core.Services.Utilities
{
    public class MailSender : IMailSender
    {
        private readonly IServiceProvider _serviceProvider ;
            public MailSender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async void SendHtmlGmail(User user,string subject,string body)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
               .To(user.Email, user.FirstName)
               .Subject(subject)
               .UsingTemplateFromFile(body, new
               {
                   FirstName = user.FirstName,
                   LastName= user.LastName,
                   EmailActiveCode=user.EmailActiveCode
               });
                await email.SendAsync();
            }
        }

        public void SendHtmlSendgrid(string recipientEmail, string recipientName)
        {
            throw new NotImplementedException();
        }

        public void SendHtmlwithAttachmentGmail(string recipientEmail, string recipientName)
        {
            throw new NotImplementedException();
        }

        public void SendHtmlwithAttachmentSendgrid(string recipientEmail, string recipientName)
        {
            throw new NotImplementedException();
        }

        public async void SendPlainTextGmail(string recipientEmail, string recipientName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
               .To("sina.saeedipoor4@gmail.com", "John")
               .Subject("سلام فلوینت")
               .Body("به نام خدا")
               ;
                await email.SendAsync();
            }
        }

        public void SendPlainTextSendgrid(string recipientEmail, string recipientName)
        {
            throw new NotImplementedException();
        }

        public void SendSendgridBulk(IEnumerable<string> recipientEmails)
        {
            throw new NotImplementedException();
        }
    }
}
