using System.Windows;
using League_Account_Manager.views.AddAccount;

namespace League_Account_Manager.Windows;

/// <summary>
///     Interaction logic for AddAccountBulk.xaml
/// </summary>
public partial class AddAccountBulk : Window
{
    public AddAccountBulk()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var lines = accountlogins.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        AddAccount.BulkAdd.Clear();

        if (lines.Length < 1 || string.IsNullOrWhiteSpace(lines[0]))
        {
            Close();
            return;
        }

        foreach (var line in lines)
        {
            var credentials = line.Split(":");
            if (credentials.Length >= 2)
                AddAccount.BulkAdd.Add(new AddAccount.UserNameList
                {
                    Username = credentials[0],
                    Password = credentials[1]
                });
        }

        Close();
    }
}