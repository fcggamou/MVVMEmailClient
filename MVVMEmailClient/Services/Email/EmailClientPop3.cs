using DeveloperTest.Models;
using Limilabs.Client;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeveloperTest.Services.Email;

/// <summary>
/// Encapsulates POP3 email client functionality.
/// </summary>
public class EmailClientPop3 : EmailClientBase
{
    private Pop3 client;

    protected override ClientBase Client
    {
        get { return client; }
    }

    public override async Task ConnectAsync(EmailServerInfo serverInfo)
    {
        client = new Pop3();
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
    }

    public override async Task<IEnumerable<EmailUID>> GetAllEmailUIDsAsync()
    {
        List<string> uIDs = await client.GetAllAsync();
        return uIDs.Select(x => new EmailUID(x));
    }

    public override async Task<IEnumerable<EmailHeader>> GetEmailHeadersByUIDAsync(IEnumerable<EmailUID> uIDs)
    {
        List<EmailHeader> emailHeaders = new List<EmailHeader>();
        foreach (EmailUID uID in uIDs)
        {
            IMail iMail = mailBuilder.CreateFromEml(await client.GetHeadersByUIDAsync(uID.UIDPop3));
            emailHeaders.Add(new EmailHeader(uID, string.Join("; ", iMail.From), iMail.Date, iMail.Subject));
        }
        return emailHeaders;
    }

    public override async Task<string> GetEmailBodyAsync(EmailUID uID)
    {
        byte[] message = await client.GetMessageByUIDAsync(uID.UIDPop3);
        IMail email = mailBuilder.CreateFromEml(message);
        return email.GetBodyAsText();
    }
}

