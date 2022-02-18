using System;

namespace DeveloperTest.Models;

/// <summary>
/// Contains email header data.
/// </summary>
public class EmailHeaderDownloadedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailHeaderDownloadedEventArgs"/> class.
    /// </summary>
    public EmailHeaderDownloadedEventArgs(EmailHeader emailHeader)
    {
        EmailHeader = emailHeader;
    }

    /// <summary>
    /// The downloaded email header.
    /// </summary>
    public EmailHeader EmailHeader { get; set; }
}

