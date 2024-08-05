using League_Account_Manager.views.Miscellaneous;
using System.Windows;

namespace League_Account_Manager.Windows;

/// <summary>
///     Interaction logic for RemoveAllFriends.xaml
/// </summary>
public partial class RemoveAllFriends : Window
{
    public RemoveAllFriends()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Miscellaneous.yayornay = 1;
        Close();
    }

    private void Button_Click1(object sender, RoutedEventArgs e)
    {
        Miscellaneous.yayornay = 2;
        Close();
    }
}