# Async WPF Email Client following the MVVM design pattern.

# Application Requirements

- Connect, using Mail.dll, to a mail server of the type specified in the server type combo box. The options should be 'IMAP' and 'POP3'. The encryption options should be 'Unencrypted', 'SSL/TLS' and 'STARTTLS'.
- Once connected, the app should begin downloading the message envelopes/headers part for all emails in the inbox automatically, and display the message data in the data grid on the left as they download. The columns should at least include 'From', 'Subject' and 'Date', but you're welcome to add more.
- At the same time as the envelopes are being downloaded, the app should also be downloading the message bodies (the actual HTML/Text) of those envelopes, in separate threads, so they're ready to be viewed when a message is selected in the view.
- To perform all these automatic download tasks, 5 connections to the server should be created when the start button is pressed, and these 5 connections should be used and reused (without disconnecting) during the course of the run of downloading everything in the inbox. When all envelopes and bodies have been downloaded, the connections should be disconnected.
- Clicking on a message in the data grid should select it, and show the message body HTML/Text in the text box on the right side. The body should be downloaded from the server on demand, if not already downloaded, or if already downloaded just shown immediately. Downloading on demand should use a new, separate, connection that exists outside of the 5 other connections. This has the intentional effect of introducing a race condition between the automatic download of message bodies and the on demand download, so make sure to handle this.
- The application should be completely thread safe, and no header or message body should ever (even in theory) be able to download more than once. To accomplish this, some locking will likely be required.

