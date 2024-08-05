using System.Windows;
using League_Account_Manager.views.AccountManagement;
using NLog;

namespace League_Account_Manager.Windows;

/// <summary>
///     Interaction logic for MissingLogin.xaml
/// </summary>
public partial class MissingLogin : Window
{
    public MissingLogin()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (username.Text.Length > 0 && password.Text.Length > 0)
            {
                AccountManagement.SelectedUsername = username.Text;
                AccountManagement.SelectedPassword = password.Password;
                Close();
            }
        }
        catch (Exception exception)
        {
            LogManager.GetCurrentClassLogger().Error(exception, "Error adding logins");
        }
    }
}