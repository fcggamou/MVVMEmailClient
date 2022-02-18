using System.ComponentModel;

namespace DeveloperTest.Models;
/// <summary>
/// The email protocol.
/// </summary>
public enum ServerType
{
    [Description("IMAP")]
    IMAP,
    [Description("POP3")]
    POP3
}

/// <summary>
/// The email encryption protocol.
/// </summary>
public enum Encryption
{
    [Description("Unencrypted")]
    UNENCRPTED,
    [Description("SSL/TLS")]
    SSL_TLS,
    [Description("STARTTLS")]
    STARTTLS
}

/// <summary>
/// Holds the necessary information to connect to an email server.
/// </summary>
public class EmailServerInfo
{
    /// <summary>
    /// The server's port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// The server's address.
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// The encryption protocol to use.
    /// </summary>
    public Encryption Encryption { get; set; }

    /// <summary>
    /// The user name.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The server's email protocol.
    /// </summary>
    public ServerType ServerType { get; set; }
}

