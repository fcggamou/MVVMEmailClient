using DeveloperTest.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Services.Email;

/// <summary>
/// Encapsulates access to a pool of concurrent email client connections.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Connects the email clients using the provided email server information and triggers the concurrent download of email headers and bodies.
    /// </summary>
    /// <remarks>
    /// Each time an email header is fetched, an <see cref="EmailHeaderDownloaded"/> event containing the header's data is raised.
    /// </remarks>
    Task ConnectAndDownloadAllEmailsAsync(EmailServerInfo serverInfo, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches the body for the email with the specified uID.
    /// </summary>        
    /// <remarks>
    /// The service must be connected using <see cref="ConnectAndDownloadAllEmailsAsync"/> before calling this method.
    /// </remarks>
    Task<string> GetEmailBodyAsync(EmailUID uID);

    /// <summary>
    /// Disconnects the email clients.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Raised whenever a new email header is downloaded.
    /// </summary>
    event EventHandler<EmailHeaderDownloadedEventArgs> EmailHeaderDownloaded;
}

