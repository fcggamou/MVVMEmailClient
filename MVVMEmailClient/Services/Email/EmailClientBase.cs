using DeveloperTest.Models;
using Limilabs.Client;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperTest.Services.Email
{
    /// <summary>
    /// Exposes email client functionalities.
    /// </summary>
    public abstract class EmailClientBase : IDisposable
    {
        protected MailBuilder mailBuilder = new MailBuilder();

        protected abstract ClientBase Client { get; }

        /// <summary>
        /// Connects the client using the specified email server information.
        /// </summary>
        public abstract Task ConnectAsync(EmailServerInfo serverInfo);

        /// <summary>
        /// Gets the email headers for the specified uIDs.
        /// </summary>
        /// <returns>A dictionary where each key/value pair holds the UID and the corresponding header of each fetched email.</returns>
        public abstract Task<IEnumerable<EmailHeader>> GetEmailHeadersByUIDAsync(IEnumerable<EmailUID> uIDs);

        /// <summary>
        /// Gets the UIDs of all the emails in the mailbox.
        /// </summary>
        public abstract Task<IEnumerable<EmailUID>> GetAllEmailUIDsAsync();

        /// <summary>
        /// Gets the body for the email with the specified UID. 
        /// </summary>
        public abstract Task<string> GetEmailBodyAsync(EmailUID uID);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Client.Dispose();
            }
        }
    }
}
