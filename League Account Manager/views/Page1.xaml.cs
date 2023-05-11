﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace League_Account_Manager.views;

/// <summary>
///     Interaction logic for Page1.xaml
/// </summary>
public partial class Page1 : Page
{
    public static string SelectedUsername, SelectedPassword;
    private readonly CsvConfiguration config = new(CultureInfo.CurrentCulture) { Delimiter = ";" };
    public DataTable dt = new();
    private double running;


    public Page1()
    {
        InitializeComponent();
        loaddata();
    }

    public List<Page2.Champs> jotain { get; private set; }

    public void loaddata()
    {
        if (File.Exists(Directory.GetCurrentDirectory() + "/List.csv"))
        {
            using (var reader = new StreamReader(Directory.GetCurrentDirectory() + "/List.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt.Clear();
                    dt.Load(dr);
                    Championlist.ItemsSource = null;
                    Championlist.ItemsSource = dt.DefaultView;
                }
            }
        }
        else
        {
            var records = new List<champs>
            {
                new()
                {
                    username = "username", password = "Password", level = "10", server = "EUW", be = "1000",
                    rp = "1000", rank = "bronze", champions = "10", skins = "10"
                }
            };
            using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + "/List.csv"))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
            }

            loaddata();
        }
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = Championlist.SelectedItem;
        if (selectedItem != null)
        {
            dt.Rows.Remove((Championlist.SelectedItem as DataRowView).Row);
            var serializeddt = JsonConvert.SerializeObject(dt, Formatting.Indented);
            var clslist = JsonConvert.DeserializeObject<List<champs>>(serializeddt,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + "/List.csv"))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(clslist);
            }
        }
    }

    public async void ring()
    {
        running += 7;
        edistyy.Progress = running;
    }

    private async void PullData_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedUsername == null || SelectedPassword == null) new Window1().ShowDialog();
        Progressgrid.Visibility = Visibility.Visible;
        ring();
        var resp = await lcu.Connector("league", "get", "/lol-service-status/v1/lcu-status", "");
        var responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (resp.StatusCode.ToString() == "OK" || 200)
        {
            ring();
            resp = await lcu.Connector("league", "get", "/lol-summoner/v1/current-summoner", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var summonerinfo = JObject.Parse(responseBody2);
            ring();
            resp = await lcu.Connector("league", "get", "/lol-catalog/v1/items/CHAMPION_SKIN", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var Skininfo = JArray.Parse(responseBody2);
            ring();
            dynamic Champinfo;
            while (true)
                try
                {
                    resp = await lcu.Connector("league", "get",
                        "/lol-champions/v1/inventories/" + summonerinfo["summonerId"] + "/champions-minimal", "");
                    responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Champinfo = JArray.Parse(responseBody2);
                    break;
                }
                catch (Exception ex)
                {
                    var jotain = JToken.Parse(responseBody2);
                    if (jotain["errorCode"] != "RPC_ERROR")
                    {
                        MessageBox.Show("Fatal error! program will now quit");
                        Environment.Exit(1);
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }

            ring();
            resp = await lcu.Connector("league", "get", "/lol-loot/v1/player-loot-map", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var LootInfo = JToken.Parse(responseBody2);
            ring();
            resp = await lcu.Connector("league", "get", "/lol-ranked/v1/current-ranked-stats", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var Rankedinfo = JToken.Parse(responseBody2);
            ring();
            resp = await lcu.Connector("league", "get", "/lol-store/v1/wallet", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var wallet = JToken.Parse(responseBody2);
            ring();
            resp = await lcu.Connector("league", "get", "/riotclient/get_region_locale", "");
            responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var region = JToken.Parse(responseBody2);
            ring();
            var skinlist = "";
            var skincount = 0;
            var champlist = "";
            var champcount = 0;
            var Lootlist = "";
            string Rank = " Rank: " + Rankedinfo["queueMap"]["RANKED_SOLO_5x5"]["tier"] + " With: " +
                          Rankedinfo["queueMap"]["RANKED_SOLO_5x5"]["wins"] + "Wins and " +
                          Rankedinfo["queueMap"]["RANKED_SOLO_5x5"]["losses"] + " Losses";
            foreach (var item in Skininfo)
                if (item["owned"] != "false")
                {
                    skinlist = skinlist + " : " + item["name"];
                    skincount++;
                }

            ring();
            foreach (var item in Champinfo)
                if (item["ownership"]["owned"] != "false")
                {
                    champlist = champlist + " : " + item["name"];
                    champcount++;
                }

            ring();
            foreach (var item in LootInfo)
            foreach (var thing in item)
                if (thing["count"] > 0)
                {
                    resp = await lcu.Connector("league", "get", "/lol-loot/v1/player-loot/" + thing["lootId"], "");
                    responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        var Loot = JObject.Parse(responseBody2);
                        if (Loot["itemDesc"] != "")
                            Lootlist = Lootlist + " : " + Loot["itemDesc"] + "x" + Loot["count"];
                        else if (Loot["localizedName"] != "")
                            Lootlist = Lootlist + " : " + Loot["localizedName"] + "x" + Loot["count"];
                        else
                            Lootlist = Lootlist + " : " + Loot["asset"] + "x" + Loot["count"];
                    }
                    catch (Exception ex)
                    {
                    }
                }

            ring();
            skinlist = skincount + " " + skinlist;
            champlist = champcount + " " + champlist;
            using (var reader = new StreamReader(Directory.GetCurrentDirectory() + "/List.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Page2.Champs>();
                jotain = records.ToList();
            }

            ring();
            jotain.RemoveAll(x => x.username == SelectedUsername);
            jotain.Add(new Page2.Champs
            {
                username = SelectedUsername, password = SelectedPassword, level = summonerinfo["summonerLevel"],
                server = region["region"], be = wallet["ip"], rp = wallet["rp"], rank = Rank, champions = champlist,
                skins = skinlist, Loot = Lootlist
            });
            ring();
            using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + "/List.csv"))
            using (var csv2 = new CsvWriter(writer, config))
            {
                csv2.WriteRecords(jotain);
            }
        }
        else
        {
            MessageBox.Show("Error", "lcu is still loading, please try again in a bit!");
        }

        running = 0;
        Progressgrid.Visibility = Visibility.Hidden;
        loaddata();
        NavigationService.Refresh();
        Championlist.Items.Refresh();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var i = 0;
        DataGridCellInfo cellinfo;
        foreach (var row in Championlist.SelectedCells)
        {
            if (i == 0)
                SelectedUsername = (row.Column.GetCellContent(row.Item) as TextBlock).Text;
            else if (i == 1) SelectedPassword = (row.Column.GetCellContent(row.Item) as TextBlock).Text;
            i++;
        }
        var processesByName = Process.GetProcessesByName("RiotClientUx");
        var processesByName2 = Process.GetProcessesByName("LeagueClientUx");
        killleaguefunc(processesByName, processesByName2);

        var num = 0;
        Process.Start("C:\\Riot Games\\Riot Client\\RiotClientServices.exe",
            "--launch-product=league_of_legends --launch-patchline=live");
        while (processesByName.Length == 0 || processesByName2.Length == 0)
        {
            processesByName2 = Process.GetProcessesByName("RiotClientUxRender");
            processesByName = Process.GetProcessesByName("RiotClientUx");
            Thread.Sleep(2000);
            num++;
            if (num == 5) return;
        }

        var resp = await lcu.Connector("riot", "post", "/rso-auth/v2/authorizations",
            "{\"clientId\":\"riot-client\",\"trustLevels\":[\"always_trusted\"]}");
        var responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        resp = await lcu.Connector("riot", "put", "/rso-auth/v1/session/credentials",
            "{\"username\":\"" + SelectedUsername + "\",\"password\":\"" + SelectedPassword +
            "\", \"persistLogin\":\"false\"}");
        var responseBody1 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        Console.WriteLine(SelectedPassword);
        Console.WriteLine(SelectedUsername);

    }

    private void Championlist_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            var selectedItem = Championlist.SelectedItem;
            if (selectedItem != null)
            {
                dt.Rows.Remove((Championlist.SelectedItem as DataRowView).Row);
                var serializeddt = JsonConvert.SerializeObject(dt, Formatting.Indented);
                var clslist = JsonConvert.DeserializeObject<List<champs>>(serializeddt,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + "/List.csv"))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(clslist);
                }
            }
        }
    }

    private void killleaguefunc(dynamic processesByName, dynamic processesByName2)
    {
        while(processesByName.Length != 0 || processesByName2.Length != 0){
        var source = new List<string>
        {
            "RiotClientUxRender", "RiotClientUx", "RiotClientServices", "RiotClientCrashHandler", "LeagueCrashHandler",
            "LeagueClientUxRender", "LeagueClientUx", "LeagueClient"
        };
        try
        {
            foreach (var item in source.SelectMany(name => Process.GetProcessesByName(name))) item.Kill();
        }
        catch (Exception)
        {
        }
        processesByName = Process.GetProcessesByName("RiotClientUx");
        processesByName2 = Process.GetProcessesByName("LeagueClientUx");
        Thread.Sleep(1000);
        }
    }

    private void killleague_Click(object sender, RoutedEventArgs e)
    {
        var processesByName = Process.GetProcessesByName("RiotClientUx");
        var processesByName2 = Process.GetProcessesByName("LeagueClientUx");
        killleaguefunc(processesByName, processesByName2);
    }

    private void openleague1_Click(object sender, RoutedEventArgs e)
    {
        var processesByName = Process.GetProcessesByName("RiotClientUx");
        var processesByName2 = Process.GetProcessesByName("LeagueClientUx");
        killleaguefunc(processesByName, processesByName2);
        openleague();
    }

    private void openleague()
    {
        Process.Start("C:\\Riot Games\\Riot Client\\RiotClientServices.exe",
            "--launch-product=league_of_legends --launch-patchline=live");
    }

    public class champs
    {
        public string username { get; set; }
        public string password { get; set; }
        public string level { get; set; }
        public string server { get; set; }
        public string be { get; set; }
        public string rp { get; set; }
        public string rank { get; set; }
        public string champions { get; set; }
        public string skins { get; set; }
        public string Loot { get; set; }
    }
}