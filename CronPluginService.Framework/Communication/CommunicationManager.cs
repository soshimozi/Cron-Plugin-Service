using System;
using System.Net.Mail;
using log4net;
using System.Reflection;

namespace CronPluginService.Framework.Communication
{
    /// <summary>
    /// A manager component implementing the singleton pattern which provides 
    /// simple email capability.
    /// </summary>
    public class CommunicationManager
    {
        private static CommunicationManager _instance;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static CommunicationManager Instance
        {
            get
            {
                return _instance ?? (_instance = new CommunicationManager());
            }
        }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public MailMessage BuildMessage(string[] recipients, string subject, string body)
        {
            var message = new MailMessage();
            foreach (string recipient in recipients)
            {
                message.To.Add(new MailAddress(recipient));
            }

            message.Subject = subject;
            message.Body = body;

            return message;
        }

        /// <summary>
        /// Sends the attachment.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool SendAttachment(string path, MailMessage message)
        {
            message.Attachments.Add(
                new Attachment(path)
            );

            return SendMail(message);
        }

    
        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns></returns>
        public bool SendMail(MailMessage mail)
        {
            //string smtpEnableSSL = ConfigurationManager.AppSettings.Get("SMTP Enable SSL");

            // use default instantiation to read from web.config
            var smtp = new SmtpClient();
            
            //bool enableSSL;
            //if( !bool.TryParse(smtpEnableSSL, out enableSSL) )
            //{
            //    enableSSL = false;
            //}

            //smtp.EnableSsl = enableSSL;

            try
            {
                smtp.Send(mail);
            }
            catch (SmtpException ex1)
            {
                Log.Error(ex1.ToString());
                //     The connection to the SMTP server failed.  -or- Authentication failed.  -or-
                //     The operation timed out.
                //     This indicates connectivity problems.
            }
            catch (InvalidOperationException ex2)
            {
                Log.Error(ex2.ToString());
                //     This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
                //     call in progress.  -or- System.Net.Mail.SmtpClient.Host is null.  -or- System.Net.Mail.SmtpClient.Host
                //     is equal to the empty string ("").  -or- System.Net.Mail.SmtpClient.Port
                //     is zero.
                //     This is most likely due to invalid configuration.
            }

            return true;
        }
    }
}
