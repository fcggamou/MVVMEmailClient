namespace DeveloperTest.Models;

/// <summary>
/// Holds the UID of an email.
/// </summary>
public struct EmailUID
{
    private long? uIDImap;

    private string uIDPop3;

    /// <summary>
    /// Creates a POP3 <see cref="EmailUID"/>.
    /// </summary>
    public EmailUID(string uID)
    {
        uIDPop3 = uID;
        uIDImap = null;
    }

    /// <summary>
    /// Creates an IMAP <see cref="EmailUID"/>.
    /// </summary>
    public EmailUID(long uID)
    {
        uIDImap = uID;
        uIDPop3 = null;
    }

    /// <summary>
    /// Gets the IMAP UID.
    /// </summary>
    public long UIDImap
    {
        get { return uIDImap.Value; }
    }

    /// <summary>
    /// Gets the POP3 UID.
    /// </summary>
    public string UIDPop3
    {
        get { return uIDPop3; }
    }

}
