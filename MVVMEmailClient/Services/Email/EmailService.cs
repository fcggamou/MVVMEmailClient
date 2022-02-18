using DeveloperTest.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Services.Email;

/// <summary>
/// Implementation of the <see cref="IEmailService"></see> interface, supporting POP3 and IMAP protocols.
/// </summary>
public class EmailService : IEmailService
{
    private const int HeaderDownloadBatchSize = 10;

    private const int ConcurrentConnections = 5;

    private EmailClientBase onDemandClient;

    private readonly List<EmailClientBase> clients = new List<EmailClientBase>();

    private readonly ConcurrentStack<EmailUID> emailHeadersToFetch = new ConcurrentStack<EmailUID>();

    private readonly ConcurrentDictionary<EmailUID, string> emailBodies = new ConcurrentDictionary<EmailUID, string>();

    private readonly ConcurrentDictionary<EmailUID, byte> emailBodiesToFetch = new ConcurrentDictionary<EmailUID, byte>();

    private readonly SemaphoreSlim downloadEmailIDsLock = new SemaphoreSlim(1);

    private readonly SemaphoreSlim getEmailBodyLock = new SemaphoreSlim(1);

    private readonly SemaphoreSlim getEmailBodyOnDemandLock = new SemaphoreSlim(1);

    private ConcurrentDictionary<EmailUID, Task<string>> getEmailBodyTasks = new ConcurrentDictionary<EmailUID, Task<string>>();

    public event EventHandler<EmailHeaderDownloadedEventArgs> EmailHeaderDownloaded;

    public async Task ConnectAndDownloadAllEmailsAsync(EmailServerInfo emailServerInfo, CancellationToken cancellationToken)
    {
        emailBodies.Clear();
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < ConcurrentConnections; i++)
        {
            EmailClientBase client = GetEmailClient(emailServerInfo);
            clients.Add(client);
            tasks.Add(ConnectClientAndDownloadAllEmailsAsync(client, emailServerInfo, cancellationToken));
        }
        onDemandClient = GetEmailClient(emailServerInfo);
        await onDemandClient.ConnectAsync(emailServerInfo);
        await Task.WhenAll(tasks);
    }

    public async Task DisconnectAsync()
    {
        foreach (EmailClientBase client in clients)
            client.Dispose();

        await getEmailBodyOnDemandLock.WaitAsync();
        onDemandClient.Dispose();
        getEmailBodyOnDemandLock.Release();

        emailBodiesToFetch.Clear();
        emailHeadersToFetch.Clear();
        clients.Clear();
    }

    public async Task<string> GetEmailBodyAsync(EmailUID uID)
    {
        string emailBody;

        if (emailBodiesToFetch.TryRemove(uID, out byte _))
        {
            await getEmailBodyOnDemandLock.WaitAsync();
            emailBody = await onDemandClient.GetEmailBodyAsync(uID);
            emailBodies[uID] = emailBody;
            getEmailBodyOnDemandLock.Release();
        }
        else
        {
            await getEmailBodyLock.WaitAsync();

            if (getEmailBodyTasks.TryGetValue(uID, out Task<string> t))
                emailBody = await t;
            else
                emailBodies.TryGetValue(uID, out emailBody);

            getEmailBodyLock.Release();
        }

        return emailBody;
    }

    protected EmailClientBase GetEmailClient(EmailServerInfo emailServerInfo)
    {
        return EmailClientFactory.GetEmailClient(emailServerInfo.ServerType);
    }

    private async Task ConnectClientAndDownloadAllEmailsAsync(EmailClientBase client, EmailServerInfo emailServerInfo, CancellationToken cancellationToken)
    {
        await client.ConnectAsync(emailServerInfo);

        await DownloadAllEmailIDsAsync(client);
        await Task.Run(() => DownloadAllEmailHeadersAsync(client, cancellationToken));
        await Task.Run(() => DownloadEmailBodiesAsync(client, cancellationToken));
    }

    private async Task DownloadAllEmailIDsAsync(EmailClientBase client)
    {
        await downloadEmailIDsLock.WaitAsync();
        if (!emailHeadersToFetch.Any())
        {
            emailHeadersToFetch.PushRange((await client.GetAllEmailUIDsAsync()).ToArray());
            foreach (EmailUID id in emailHeadersToFetch)
                emailBodiesToFetch.TryAdd(id, default(byte));
        }

        downloadEmailIDsLock.Release();
    }

    private async Task DownloadAllEmailHeadersAsync(EmailClientBase client, CancellationToken cancellationToken)
    {
        EmailUID[] headerUIDsToFetch = new EmailUID[HeaderDownloadBatchSize];
        int count;
        while ((count = emailHeadersToFetch.TryPopRange(headerUIDsToFetch)) > 0)
        {
            foreach (EmailHeader emailHeader in await client.GetEmailHeadersByUIDAsync(headerUIDsToFetch.Take(count)))
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                EmailHeaderDownloaded(this, new EmailHeaderDownloadedEventArgs(emailHeader));
            }
        }
    }

    private async Task DownloadEmailBodiesAsync(EmailClientBase client, CancellationToken cancellationToken)
    {
        foreach (EmailUID uID in emailBodiesToFetch.Keys)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (emailBodiesToFetch.TryRemove(uID, out byte _))
            {
                await getEmailBodyLock.WaitAsync();
                Task<string> t = client.GetEmailBodyAsync(uID);
                getEmailBodyTasks[uID] = t;
                getEmailBodyLock.Release();

                string body = await t;

                await getEmailBodyLock.WaitAsync();
                emailBodies[uID] = body;
                getEmailBodyTasks.TryRemove(uID, out Task<string> _);
                getEmailBodyLock.Release();
            }
        }
    }
}

