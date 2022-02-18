using System;

namespace DeveloperTest.Models;

/// <summary>
/// Holds UID, author, date and subject data of an email header.
/// </summary>
public class EmailHeader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailHeader"/> class.
    /// </summary>
    public EmailHeader(EmailUID uID, string from, DateTime? date, string subject)
    {
        UID = uID;
        From = from;
        Date = date;
        Subject = subject;
    }

    /// <summary>
    /// The UID of them email.
    /// </summary>
    public EmailUID UID { get; set; }

    /// <summary>
    /// Name and address(es) of the author(s) of the email.
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// Date of the email.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// The subject of the email.
    /// </summary>
    public string Subject { get; set; }
}

