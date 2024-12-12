using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace League_Account_Manager.views.Settings;

/// <summary>
///     Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
        settingssaveinfobox.Text = Utilities.Settings.settingsloaded.filename;
        savesettingsupdates.IsChecked = Utilities.Settings.settingsloaded.updates;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Utilities.Settings.settingsloaded.filename = settingssaveinfobox.Text;
        if (savesettingsupdates.IsChecked != false)
            Utilities.Settings.settingsloaded.updates = true;
        else
            Utilities.Settings.settingsloaded.updates = false;
        var json = JsonSerializer.Serialize(Utilities.Settings.settingsloaded);
        File.WriteAllText(Directory.GetCurrentDirectory() + "/Settings.json", json);
        Process.Start(Process.GetCurrentProcess().MainModule.FileName);
        Application.Current.Shutdown();
    }
}