using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Notifications
{
    public interface IEmailService
    {
        Task SendWelcomeMail(string toEmail);
    }
}
