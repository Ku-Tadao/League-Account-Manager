    using System.IO;
    using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CsvHelper;
using League_Account_Manager.views;
using static League_Account_Manager.views.AccountManagement.AccountManagement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using CsvHelper.Configuration;
using System.Globalization;
using League_Account_Manager.views.AccountManagement;
using League_Account_Manager.Utilities;


namespace League_Account_Manager;

/// <summary>
///     Interaction logic for Window4.xaml
/// </summary>
public partial class AccountNoteWindow : Window
{
    private AccountList dataholder;
    private readonly CsvConfiguration _config = new(CultureInfo.CurrentCulture) { Delimiter = ";" };

    public AccountNoteWindow(AccountList Data)
    {
        InitializeComponent();
        Datathing.Clear();
        dataholder = Data;
        Datathing.AppendText(Data.note);
    }


    private void Window_MouseDownDatadisplay(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private async void Window_Deactivated(object sender, EventArgs e)
    {
        if (Datathing.Text != dataholder.note)
        {
            dataholder.note = Datathing.Text;
            AccountManagement.ActualAccountlists.RemoveAll(r => r.username == dataholder.username && r.password == dataholder.password);
            AccountManagement.ActualAccountlists.Add(dataholder);
            AccountManagement.RemoveDoubleQuotesFromList(AccountManagement.ActualAccountlists);
            FileStream? fileStream = null;
            while (fileStream == null)
                try
                {
                    fileStream =
                        File.Open(Path.Combine(Directory.GetCurrentDirectory(), $"{Settings.settingsloaded.filename}.csv"),
                            FileMode.Open, FileAccess.Read, FileShare.None);
                    fileStream.Close();
                }
                catch (IOException)
                {
                    // The file is in use by another process. Wait and try again.
                    await Task.Delay(1000);
                }

            using var writer =
                new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), $"{Settings.settingsloaded.filename}.csv"));
            using var csvWriter = new CsvWriter(writer, _config);
            csvWriter.WriteRecords(AccountManagement.ActualAccountlists);

        }
        Close();
    }

}