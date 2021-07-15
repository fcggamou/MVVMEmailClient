using DeveloperTest.ViewModels;
using System.Windows;

namespace DeveloperTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new Services.Email.EmailService());
        }
    }
}
