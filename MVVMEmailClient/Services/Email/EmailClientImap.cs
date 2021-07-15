using DeveloperTest.Models;
using Limilabs.Client;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeveloperTest.Services.Email
{
    /// <summary>
    /// Encapsulates IMAP email client functionality.
    /// </summary>
    public class EmailClientImap : EmailClientBase
    {
        private Imap client;

        protected override ClientBase Client
        {
            get { return client; }
        }

        public override async Task ConnectAsync(EmailServerInfo serverInfo)
        {
            client = new Imap();
            switch (serverInfo.Encryption)
            {
                case Encryption.STARTTLS:
                    await client.StartTLSAsync();
                    break;
                case Encryption.SSL_TLS:
                    await client.ConnectSSLAsync(serverInfo.Server, serverInfo.Port);
                    break;
                case Encryption.UNENCRPTED:
                    await client.ConnectAsync(serverInfo.Server, serverInfo.Port);
                    break;
            }

            await client.UseBestLoginAsync(serverInfo.Username, serverInfo.Password);
            await client.SelectInboxAsync();
        }

        public override async Task<IEnumerable<EmailUID>> GetAllEmailUIDsAsync()
        {
            List<long> uIDs = await client.GetAllAsync();
            return uIDs.Select(x => new EmailUID(x));
        }

        public override async Task<IEnumerable<EmailHeader>> GetEmailHeadersByUIDAsync(IEnumerable<EmailUID> uIDs)
        {
            List<MessageData> messagesData = await client.GetHeadersByUIDAsync(uIDs.Select(x => x.UIDImap).ToList());
            List<EmailHeader> emailHeaders = new List<EmailHeader>();
            foreach (MessageData messagedata in messagesData)
            {
                IMail iMail = mailBuilder.CreateFromEml(messagedata.EmlData);
                emailHeaders.Add(new EmailHeader(new EmailUID(messagedata.UID.Value), string.Join("; ", iMail.From), iMail.Date, iMail.Subject));
            }

            return emailHeaders;
        }

        public override async Task<string> GetEmailBodyAsync(EmailUID uID)
        {
            BodyStructure bodyStructure = await client.GetBodyStructureByUIDAsync(uID.UIDImap);
            return await client.GetTextByUIDAsync(bodyStructure.Text ?? bodyStructure.Html);
        }
    }
}
