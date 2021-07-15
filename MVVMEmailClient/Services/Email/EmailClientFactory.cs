using DeveloperTest.Models;

namespace DeveloperTest.Services.Email
{
    /// <summary>
    /// Factory for creating instances of concrete implementations of <see cref="EmailClientBase"></see>.
    /// </summary>
    public static class EmailClientFactory
    {
        /// <summary>
        /// Returns a new instance of <see cref="EmailClientBase"></see> based on the provided <see cref="ServerType"></see>.
        /// </summary>
        public static EmailClientBase GetEmailClient(ServerType serverType)
        {
            if (serverType == ServerType.IMAP)
                return new EmailClientImap();
            else
                return new EmailClientPop3();
        }
    }
}
