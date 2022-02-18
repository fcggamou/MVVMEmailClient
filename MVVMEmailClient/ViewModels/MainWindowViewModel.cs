using AsyncAwaitBestPractices.MVVM;
using DeveloperTest.Models;
using DeveloperTest.Services.Email;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeveloperTest.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private const string StartButtonText = "Start";

    private const string StopButtonText = "Stop";

    private readonly IEmailService emailService;

    private readonly EmailServerInfo serverInfo;

    private readonly AsyncCommand startStopDownloadEmailsCommand;

    private readonly AsyncCommand getEmailBodyCommand;

    private bool downloadingEmails;

    private string startButtonText = StartButtonText;

    private readonly ObservableCollection<EmailHeader> emailHeaders = new ObservableCollection<EmailHeader>();

    private EmailHeader selectedEmailHeader;

    private string selectedEmailBody;

    private CancellationTokenSource cancellationToken = new CancellationTokenSource();

    public MainWindowViewModel(IEmailService mailService)
    {
        emailService = mailService;
        emailService.EmailHeaderDownloaded += EmailsServiceEmailHeaderDownloaded;
        startStopDownloadEmailsCommand = new AsyncCommand(StartStopDownloadingAllEmails);
        getEmailBodyCommand = new AsyncCommand(GetEmailBody);
        serverInfo = new EmailServerInfo();
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> responsible for fetching the selected email's body.
    /// </summary>
    public ICommand GetEmailBodyCommand
    {
        get { return getEmailBodyCommand; }
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> responsible for starting/stopping the email downloading task.
    /// </summary>
    /// <remarks>
    /// If the process is not already running, this command will start it, otherwise it will stop it.
    /// </remarks>
    public ICommand StartStopDownloadingEmailsCommand
    {
        get { return startStopDownloadEmailsCommand; }
    }

    /// <summary>
    /// Gets the <see cref="EmailServerInfo"/> holding the information used to connect to the email server.
    /// </summary>
    public EmailServerInfo ServerInfo
    {
        get { return serverInfo; }
    }

    /// <summary>
    /// Gets the downloaded <see cref="EmailHeader"/>s.
    /// </summary>
    public IEnumerable<EmailHeader> EmailHeaders
    {
        get { return emailHeaders; }
    }

    /// <summary>
    /// Gets or sets the currently selected <see cref="EmailHeader"/>.
    /// </summary>
    public EmailHeader SelectedEmailHeader
    {
        get
        {
            return selectedEmailHeader;
        }
        set
        {
            selectedEmailHeader = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the body corresponding to the <see cref="SelectedEmailHeader"/>.
    /// </summary>
    public string SelectedEmailBody
    {
        get
        {
            return selectedEmailBody;
        }
        set
        {
            selectedEmailBody = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the values and display texts of the supported <see cref="ServerType"/>s.
    /// </summary>
    public IEnumerable<KeyValuePair<ServerType, string>> ServerTypes
    {
        get { return Helpers.EnumHelper.GetEnumValuesAndDescriptions<ServerType>(); }
    }

    /// <summary>
    /// Gets the values and display texts of the supported <see cref="Encryptions"/>s.
    /// </summary>
    public IEnumerable<KeyValuePair<Encryption, string>> Encryptions
    {
        get { return Helpers.EnumHelper.GetEnumValuesAndDescriptions<Encryption>(); }
    }

    /// <summary>
    /// Gets or sets the display text of the start/stop button.
    /// </summary>
    public string StartStopButtonText
    {
        get
        {
            return startButtonText;
        }
        set
        {
            startButtonText = value;
            OnPropertyChanged();
        }
    }

    private bool IsDownloadingEmails
    {
        get
        {
            return downloadingEmails;
        }
        set
        {
            StartStopButtonText = value ? StopButtonText : StartButtonText;
            downloadingEmails = value;
        }
    }

    private async Task StartStopDownloadingAllEmails()
    {
        if (IsDownloadingEmails)
            StopDownloadingEmails();
        else
            await ConnectAndDownloadAllEmails();
    }

    private void StopDownloadingEmails()
    {
        cancellationToken?.Cancel();
        return;
    }

    private async Task ConnectAndDownloadAllEmails()
    {
        IsDownloadingEmails = true;
        try
        {
            ClearEmailsData();
            cancellationToken = new CancellationTokenSource();
            await emailService.ConnectAndDownloadAllEmailsAsync(serverInfo, cancellationToken.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error downloading emails");
        }
        finally
        {
            await emailService.DisconnectAsync();
            cancellationToken.Dispose();
            IsDownloadingEmails = false;
        }
    }

    private async Task GetEmailBody()
    {
        if (SelectedEmailHeader == null)
            return;
        try
        {
            SelectedEmailBody = string.Empty;
            SelectedEmailBody = await emailService.GetEmailBodyAsync(SelectedEmailHeader.UID);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error downloading email body");
        }
    }

    private void ClearEmailsData()
    {
        SelectedEmailBody = string.Empty;
        emailHeaders.Clear();
    }

    private void EmailsServiceEmailHeaderDownloaded(object sender, EmailHeaderDownloadedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => emailHeaders.Add(e.EmailHeader));
    }
}

