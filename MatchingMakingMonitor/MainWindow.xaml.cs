using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using MatchingMakingMonitor.Models;
using Newtonsoft.Json;
using RestSharp;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace MatchingMakingMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<ShipModel> _ships;

        public MainWindow()
        {
            InitializeComponent();

            Log("Initializing Started");

            try
            {
                //get our list of ships for later use
                _ships = GetShips();

                //start our timer
                var checkTimer = new System.Windows.Threading.DispatcherTimer();
                checkTimer.Tick += CheckTimer_Tick;
                checkTimer.Interval = new TimeSpan(0, 0, 10);
                checkTimer.Start();

                //set the players region
                UpdatePlayerRegion();

                if (Directory.Exists(Properties.Settings.Default["InstallDirectory"] + "/replays"))
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"].ToString();
                } //end if
                else
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"] +
                                                    " - Invalid Path or Replays not enabled! - Click here to update!";
                } //end else
            } //end try
            catch (Exception ex)
            {
                //ignore
                TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"] +
                                                " - Invalid Path or Replays not enabled! - Click here to update!";

                Log("Exception Occurred During Initialization:  " + ex.Message);
            } //end catch

            Log("Initialization Complete");
        } //end MainWindow

        private void InstallDirectoryClick(object sender, MouseButtonEventArgs e)
        {
            Log("Install Directory Clicked");

            //the directory does not exist in the default location, ask the user to tell us where it is.
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            Log("Showing Dialog");

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowser.SelectedPath))
            {
                if (Directory.Exists(folderBrowser.SelectedPath + "/replays"))
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    TxtInstallDirectoryValue.Text = folderBrowser.SelectedPath;

                    Properties.Settings.Default["InstallDirectory"] = folderBrowser.SelectedPath;
                    Properties.Settings.Default.Save();

                    Log("Valid Install Directory Chosen " + folderBrowser.SelectedPath);
                } //end if
                else
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"] +
                                                    " - Invalid Path or Replays not enabled! - Click here to update!";

                    Log("Invalid Install Directory Chosen " + folderBrowser.SelectedPath);
                } //end else
            } //end if
            else if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                Log("Dialog was Cancelled by the User");

                if (Directory.Exists(Properties.Settings.Default["InstallDirectory"] + "/replays"))
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"].ToString();

                    Log("Dialog was Cancelled by the User by path is valid");
                } //end if
                else
                {
                    TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    TxtInstallDirectoryValue.Text = Properties.Settings.Default["InstallDirectory"] +
                                                    " - Invalid Path or Replays not enabled! - Click here to update!";

                    Log("Dialog was Cancelled by the User and the path is still invalid");
                } //end else
            } //end else if
            else
            {
                TxtInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                TxtInstallDirectoryValue.Text = "Please Choose a Valid Path!";

                Log("Dialog was closed by an unknown source");
            } //end else
        } //end InstallDirectoryClick

        private void ComRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update the players choice
            Properties.Settings.Default["Region"] = ((ComboBoxItem) ComRegion.SelectedItem).Content.ToString();
            Properties.Settings.Default.Save();

            Log("Region was changed to " + Properties.Settings.Default["Region"]);
        } //end ComRegion_SelectionChanged

        private async Task<List<PlayerShipStats>> FetchResults(ReplayModel replay, string region)
        {
            Log("Fetching Results");

            return await Task.Run(() =>
            {
                var retList = new List<PlayerShipStats>();

                Parallel.ForEach(replay.vehicles, new ParallelOptions {MaxDegreeOfParallelism = 5}, p =>
                {
                    try
                    {
                        //setup our API calls
                        RestClient client;
                        var appId = Properties.Settings.Default["AppId"].ToString();

                        switch (region.ToString())
                        {
                            case "EU":
                                client = new RestClient("https://api.worldofwarships.eu");
                                appId = Properties.Settings.Default["AppIdEU"].ToString();
                                break;
                            case "SEA":
                                client = new RestClient("https://api.worldofwarships.asia");
                                appId = Properties.Settings.Default["AppIdSEA"].ToString();
                                break;
                            case "RU":
                                client = new RestClient("https://api.worldofwarships.ru");
                                appId = Properties.Settings.Default["AppIdRU"].ToString();
                                break;
                            default:
                                client = new RestClient("https://api.worldofwarships.com");
                                appId = Properties.Settings.Default["AppId"].ToString();
                                break;
                        } //end switch

                        //start by getting the players accountId so we can grab their stats
                        var request = new RestRequest("/wows/account/list/?application_id={appId}&search={username}", Method.GET);
                        request.AddUrlSegment("username", p.name);
                        request.AddUrlSegment("appId", appId);

                        var response = client.Execute<WargamingSearch>(request);

                        if (response.Data != null)
                        {
                            //we need just the player we want and this is a fuzzy search result
                            var thisPlayer = response.Data.data.FirstOrDefault(x => x.nickname == p.name);

                            if (thisPlayer == null)
                            {
                                //we didn't find this player set stats to 0
                                var stat = new PlayerShipStats
                                {
                                    ShipId = long.Parse(p.shipId.ToString()),
                                    ShipName = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipName,
                                    ShipType = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipType,
                                    ShipTier = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.Tier,
                                    Battles = 0,
                                    Nickname = p.name,
                                    Relationship = p.relation
                                };

                                retList.Add(stat);
                            } //end if
                            else
                            {
                                //we have a valid player to search, otherwise set this player to no stats
                                request =
                                    new RestRequest(
                                        "/wows/ships/stats/?application_id={appId}&account_id={accountId}&ship_id={shipId}",
                                        Method.GET);
                                request.AddUrlSegment("appId", appId);
                                request.AddUrlSegment("accountId", thisPlayer.account_id.ToString());
                                request.AddUrlSegment("shipId", p.shipId.ToString());

                                var statResponse = client.Execute(request);
                                var jsonString = statResponse.Content.Replace("\"" + thisPlayer.account_id + "\":",
                                    "\"Ships\":");

                                //now that we have their actual stats we can process them into a model
                                var ship = JsonConvert.DeserializeObject<PlayerStatsByShip>(jsonString);

                                if (ship?.data.Ships != null && ship.data.Ships.Any())
                                {
                                    var s = ship.data.Ships.FirstOrDefault();

                                    //we have their stats, pack it into a model
                                    var stat = new PlayerShipStats
                                    {
                                        ShipId = long.Parse(s.ship_id),
                                        ShipName = _ships.FirstOrDefault(x => x.ShipId == s.ship_id)?.ShipName,
                                        ShipType = _ships.FirstOrDefault(x => x.ShipId == s.ship_id)?.ShipType,
                                        ShipTier = _ships.FirstOrDefault(x => x.ShipId == s.ship_id)?.Tier,
                                        AccountId = thisPlayer.account_id,
                                        Nickname = thisPlayer.nickname,
                                        Distance = s.distance,
                                        Relationship = p.relation,
                                        LastBattle = s.last_battle_time.ToString(),
                                        MaxXp = s.pvp.max_xp,
                                        DamageToBuildings = s.pvp.damage_to_buildings,
                                        MainBatteryMaxFrags = s.pvp.main_battery.max_frags_battle,
                                        MainBatteryFrags = s.pvp.main_battery.frags,
                                        MainBatteryShots = s.pvp.main_battery.shots,
                                        MainBatteryHits = s.pvp.main_battery.hits,
                                        SuppressionCount = s.pvp.suppressions_count,
                                        MaxDamageScouting = s.pvp.max_damage_scouting,
                                        ArtAgro = s.pvp.art_agro,
                                        ShipSpotted = s.pvp.ships_spotted,
                                        SecondaryMaxFrags = s.pvp.second_battery.max_frags_battle,
                                        SecondaryFrags = s.pvp.second_battery.frags,
                                        SecondaryHits = s.pvp.second_battery.hits,
                                        SecondaryShots = s.pvp.second_battery.shots,
                                        XpEarned = s.pvp.xp,
                                        SurvivedBattles = s.pvp.survived_battles,
                                        DroppedCapPoints = s.pvp.dropped_capture_points,
                                        MaxDamageToBuildings = s.pvp.max_damage_dealt_to_buildings,
                                        TorpedoAgro = s.pvp.torpedo_agro,
                                        Draws = s.pvp.draws,
                                        PlanesKilled = s.pvp.planes_killed,
                                        Battles = s.pvp.battles,
                                        MaxShipsSpotted = s.pvp.max_ships_spotted,
                                        TeamCapturePoints = s.pvp.team_capture_points,
                                        Frags = s.pvp.frags,
                                        DamageScouting = s.pvp.damage_scouting,
                                        MaxTotalAgro = s.pvp.max_total_agro,
                                        MaxFrags = s.pvp.max_frags_battle,
                                        CapturePoints = s.pvp.capture_points,
                                        RamMaxFrags = s.pvp.ramming.max_frags_battle,
                                        RamFrags = s.pvp.ramming.frags,
                                        TorpMaxFrags = s.pvp.torpedoes.max_frags_battle,
                                        TorpFrags = s.pvp.torpedoes.frags,
                                        TorpHits = s.pvp.torpedoes.hits,
                                        TorpShots = s.pvp.torpedoes.shots,
                                        AircraftMaxFrags = s.pvp.aircraft.max_frags_battle,
                                        AircraftFrags = s.pvp.aircraft.frags,
                                        SurvivedWins = s.pvp.survived_wins,
                                        MaxDamage = s.pvp.max_damage_dealt,
                                        Wins = s.pvp.wins,
                                        Losses = s.pvp.losses,
                                        DamageDealt = s.pvp.damage_dealt,
                                        MaxPlanesKilled = s.pvp.max_planes_killed,
                                        MaxSuppressionCount = s.pvp.max_suppressions_count,
                                        TeamDroppedCapPoints = s.pvp.team_dropped_capture_points,
                                        BattlesSince512 = s.pvp.battles_since_512,
                                        LastUpdatedWG = s.updated_at.ToString()
                                    };

                                    retList.Add(stat);
                                } //end if
                                else
                                {
                                    //no stats, we need to show them anyways
                                    var stat = new PlayerShipStats
                                    {
                                        ShipId = long.Parse(p.shipId.ToString()),
                                        ShipName = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipName,
                                        ShipType = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipType,
                                        ShipTier = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.Tier,
                                        Battles = 0,
                                        Nickname = p.name,
                                        Relationship = p.relation
                                    };

                                    retList.Add(stat);
                                } //end else
                            } //end else
                        } //end if
                        else
                        {
                            //no stats
                            //we didn't find this player set stats to 0
                            var stat = new PlayerShipStats
                            {
                                ShipId = long.Parse(p.shipId.ToString()),
                                ShipName = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipName,
                                ShipType = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipType,
                                ShipTier = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.Tier,
                                Battles = 0,
                                Nickname = p.name,
                                Relationship = p.relation
                            };

                            retList.Add(stat);
                        } //end else
                    } //end try
                    catch (Exception ex)
                    {
                        //ignore
                        Log("An Error Occurred While Fetching Players" + ex.Message);

                        var stat = new PlayerShipStats
                        {
                            ShipId = long.Parse(p.shipId.ToString()),
                            ShipName = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipName,
                            ShipType = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.ShipType,
                            ShipTier = _ships.FirstOrDefault(x => x.ShipId == p.shipId.ToString())?.Tier,
                            Battles = 0,
                            Nickname = p.name,
                            Relationship = p.relation
                        };

                        retList.Add(stat);
                    } //end catch
                });

                Log("Fetched Players from Wargaming.  " + retList.Count() + " Players Successfully Found");

                return retList;
            });
        } //end FetchResults

        private void ClearResults()
        {
            LblStatus.Foreground = new SolidColorBrush(Colors.Black);
            LblStatus.Content = "Not Currently in Battle - Stats are from Last Battle";

            Log("Clearing Results after Battle");
        } //end ClearResults

        private void UpdatePlayerRegion()
        {
            switch (Properties.Settings.Default["Region"].ToString())
            {
                case "NA":
                    ComRegion.SelectedIndex = 0;

                    break;
                case "EU":
                    ComRegion.SelectedIndex = 1;

                    break;
                case "RU":
                    ComRegion.SelectedIndex = 2;

                    break;
                case "SEA":
                    ComRegion.SelectedIndex = 3;

                    break;
                default:
                    ComRegion.SelectedIndex = 0;

                    break;
            } //end switch
        } //edn UpdatePlayerRegion

        private bool IsNewReplay(ReplayModel newReplay)
        {
            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default["LastReplay"].ToString()))
                {
                    var oldReplay = Properties.Settings.Default["LastReplay"].ToString();

                    //it's not empty, so we can compare them now.
                    if (oldReplay != newReplay.dateTime)
                    {
                        //this is a new replay, update our settings
                        Properties.Settings.Default["LastReplay"] = newReplay.dateTime;
                        Properties.Settings.Default.Save();

                        Log("Found a new replay");

                        return true;
                    } //end if
                } //end if
                else
                {
                    //this is a new replay, update our settings
                    Properties.Settings.Default["LastReplay"] = newReplay.dateTime;
                    Properties.Settings.Default.Save();

                    Log("Found a new replay");

                    //this is a new replay since the LastReplay is empty
                    return true;
                } //end else
            } //end try
            catch (Exception ex)
            {
                //ignore
                Log("Exception Occurred while checking for new Replay: " + ex.Message);
            } //end catch

            return false;
        } //end IsNewReplay

        private ReplayModel GetReplayDetails()
        {
            using (
                var sr =
                    new StreamReader(Properties.Settings.Default["InstallDirectory"] + "/replays/tempArenaInfo.json"))
            {
                Log("Getting Replay Details");

                return JsonConvert.DeserializeObject<ReplayModel>(sr.ReadToEnd());
            } //end using
        } //end GetReplayDetails

        private bool CurrentlyPlaying()
        {
            Log("Checking if the player is currently playing");

            return File.Exists(Properties.Settings.Default["InstallDirectory"] + "/replays/tempArenaInfo.json");
        } //end CurrentlyPlaying

        private async void CheckTimer_Tick(object sender, EventArgs e)
        {
            Log("Timer Ticked");

            if (CurrentlyPlaying())
            {
                Log("Player is currently playing");

                ReplayModel replayData = null;

                try
                {
                    replayData = GetReplayDetails();
                } //end try
                catch (Exception ex)
                {
                    //this failed so we need to unset our property and try again in 10 seconds
                    Properties.Settings.Default["LastReplay"] = "";
                    Properties.Settings.Default.Save();

                    Log("Exception Occurred While Retrieving Replay Data: " + ex.Message);
                } //end catch

                //we didn't fall into the catch so we have a valid json string here
                //is this the same replay as before or did the player move on to another battle?
                if (IsNewReplay(replayData) && replayData != null)
                {
                    Log("Retrieved Valid Replay Data: " + JsonConvert.SerializeObject(replayData));
                    Log("New Replay and Valid Data Obtained");

                    //this is a new replay, let's go get the details
                    LblStatus.Foreground = new SolidColorBrush(Colors.Goldenrod);
                    LblStatus.Content = "Fetching Player Stats for Current Battle";

                    var playerStats = await FetchResults(replayData, ((ComboBoxItem) ComRegion.SelectedItem).Content.ToString());

                    if (playerStats.Any())
                    {
                        Log("Processing " + playerStats.Count + " Players");

                        //start by removing all previous controls
                        FriendlyGroup.Children.Clear();
                        EnemyGroup.Children.Clear();

                        Log("Cleared Groups");

                        //re-add our labels
                        FriendlyGroup.Children.Add(new System.Windows.Controls.Label
                        {
                            Content = "Friendly Team",
                            Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19))
                        });

                        EnemyGroup.Children.Add(new System.Windows.Controls.Label
                        {
                            Content = "Enemy Team",
                            Foreground = new SolidColorBrush(Color.FromRgb(170, 7, 7))
                        });

                        Log("Added Team Labels");

                        FriendlyGroup.Children.Add(new Separator());
                        EnemyGroup.Children.Add(new Separator());

                        Log("Added Separators");

                        //start with the friendly team.
                        foreach (var p in playerStats.Where(x => x.Relationship == 0 || x.Relationship == 1).OrderBy(x => x.ShipType).ThenByDescending(x => x.ShipTier))
                        {
                            Log("Processing Player: " + p.Nickname);

                            try
                            {
                                if (p.AccountId == 0)
                                {
                                    var background = new SolidColorBrush(GetPlayerBackground(p));

                                    var playerGrid = new Grid
                                    {
                                        Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
                                    };

                                    //create our columns
                                    var gridCol1 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol2 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol3 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol4 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.ColumnDefinitions.Add(gridCol1);
                                    playerGrid.ColumnDefinitions.Add(gridCol2);
                                    playerGrid.ColumnDefinitions.Add(gridCol3);
                                    playerGrid.ColumnDefinitions.Add(gridCol4);

                                    //create our rows
                                    var gridRow1 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridRow2 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.RowDefinitions.Add(gridRow1);
                                    playerGrid.RowDefinitions.Add(gridRow2);

                                    //add our custom controls to the player group box
                                    var tempTextblock = new TextBlock
                                    {
                                        Text = p.Nickname + " | " + p.AccountId,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 12.0,
                                        Foreground = background,
                                        Padding = new Thickness(5)
                                    };

                                    tempTextblock.MouseDown += PlayerDetailsMouseDown;
                                    tempTextblock.MouseEnter += PlayerDetailsMouseEnter;
                                    tempTextblock.MouseLeave += PlayerDetailsMouseLeave;

                                    Grid.SetRow(tempTextblock, 0);
                                    Grid.SetColumn(tempTextblock, 0);
                                    playerGrid.Children.Add(tempTextblock);

                                    var tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"No Stats or Hidden",
                                        FontSize = 12.0,
                                        Foreground = p.Battles > 100 ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.Black)
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgDamageColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = p.ShipName,
                                        FontSize = 11.0
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 0);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgXpColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerWinRateColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgFragsColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    FriendlyGroup.Children.Add(playerGrid);

                                    //add a separator
                                    FriendlyGroup.Children.Add(new Separator());
                                } //end if
                                else
                                {
                                    var background = new SolidColorBrush(GetPlayerBackground(p));

                                    var playerGrid = new Grid
                                    {
                                        Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
                                    };

                                    //create our columns
                                    var gridCol1 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol2 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol3 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol4 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.ColumnDefinitions.Add(gridCol1);
                                    playerGrid.ColumnDefinitions.Add(gridCol2);
                                    playerGrid.ColumnDefinitions.Add(gridCol3);
                                    playerGrid.ColumnDefinitions.Add(gridCol4);

                                    //create our rows
                                    var gridRow1 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridRow2 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.RowDefinitions.Add(gridRow1);
                                    playerGrid.RowDefinitions.Add(gridRow2);

                                    //add our custom controls to the player group box
                                    var tempTextblock = new TextBlock
                                    {
                                        Text = p.Nickname + " | " + p.AccountId,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 12.0,
                                        Foreground = background,
                                        Padding = new Thickness(5)
                                    };

                                    tempTextblock.MouseDown += PlayerDetailsMouseDown;
                                    tempTextblock.MouseEnter += PlayerDetailsMouseEnter;
                                    tempTextblock.MouseLeave += PlayerDetailsMouseLeave;

                                    Grid.SetRow(tempTextblock, 0);
                                    Grid.SetColumn(tempTextblock, 0);
                                    playerGrid.Children.Add(tempTextblock);

                                    var tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Battles:  " + p.Battles,
                                        FontSize = 12.0,
                                        Foreground = p.Battles > 100 ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.Black)
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Wins:  " + p.Wins,
                                        FontSize = 12.0
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg Dmg:  " + Math.Round((float)p.DamageDealt / (float)p.Battles, 0),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgDamageColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = p.ShipName,
                                        FontSize = 11.0
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 0);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg XP:  " + Math.Round((float)p.XpEarned / (float)p.Battles, 0),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgXpColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Win Rate:  " + Math.Round((float)p.Wins / (float)p.Battles * 100f, 2) + @"%",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerWinRateColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg Frags:  " + Math.Round((float)p.Frags / (float)p.Battles, 2),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgFragsColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    FriendlyGroup.Children.Add(playerGrid);

                                    //add a separator
                                    FriendlyGroup.Children.Add(new Separator());
                                } //end else
                            } //end try
                            catch (Exception ex)
                            {
                                //ignore
                                Log("Exception Occurred Processing Player: " + ex.Message);
                            } //end catch
                        } //end foreach

                        FriendlyGroup.Visibility = Visibility.Visible;
                        Log("Friendly Group is Visible");
                        Log("Entering Enemy Team Foreach");

                        //now the enemy team
                        foreach (
                            var p in
                            playerStats.Where(x => x.Relationship == 2)
                                .OrderBy(x => x.ShipType)
                                .ThenByDescending(x => x.ShipTier))
                        {
                            Log("Processing Player: " + p.Nickname);

                            try
                            {
                                if (p.AccountId == 0)
                                {
                                    var background = new SolidColorBrush(GetPlayerBackground(p));

                                    var playerGrid = new Grid
                                    {
                                        Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
                                    };

                                    //create our columns
                                    var gridCol1 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol2 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol3 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol4 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.ColumnDefinitions.Add(gridCol1);
                                    playerGrid.ColumnDefinitions.Add(gridCol2);
                                    playerGrid.ColumnDefinitions.Add(gridCol3);
                                    playerGrid.ColumnDefinitions.Add(gridCol4);

                                    //create our rows
                                    var gridRow1 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridRow2 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.RowDefinitions.Add(gridRow1);
                                    playerGrid.RowDefinitions.Add(gridRow2);

                                    //add our custom controls to the player group box
                                    var tempTextblock = new TextBlock
                                    {
                                        Text = p.Nickname + " | " + p.AccountId,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 12.0,
                                        Foreground = background,
                                        Padding = new Thickness(5)
                                    };

                                    tempTextblock.MouseDown += PlayerDetailsMouseDown;
                                    tempTextblock.MouseEnter += PlayerDetailsMouseEnter;
                                    tempTextblock.MouseLeave += PlayerDetailsMouseLeave;

                                    Grid.SetRow(tempTextblock, 0);
                                    Grid.SetColumn(tempTextblock, 0);
                                    playerGrid.Children.Add(tempTextblock);

                                    var tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"No Stats or Hidden",
                                        FontSize = 12.0,
                                        Foreground = p.Battles > 100 ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.Black)
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgDamageColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = p.ShipName,
                                        FontSize = 11.0
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 0);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgXpColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerWinRateColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgFragsColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    EnemyGroup.Children.Add(playerGrid);

                                    //add a separator
                                    EnemyGroup.Children.Add(new Separator());
                                } //end if
                                else
                                {
                                    var background = new SolidColorBrush(GetPlayerBackground(p));

                                    var playerGrid = new Grid
                                    {
                                        Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
                                    };

                                    //create our columns
                                    var gridCol1 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol2 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol3 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridCol4 = new ColumnDefinition
                                    {
                                        Width = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.ColumnDefinitions.Add(gridCol1);
                                    playerGrid.ColumnDefinitions.Add(gridCol2);
                                    playerGrid.ColumnDefinitions.Add(gridCol3);
                                    playerGrid.ColumnDefinitions.Add(gridCol4);

                                    //create our rows
                                    var gridRow1 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    var gridRow2 = new RowDefinition
                                    {
                                        Height = new GridLength(1.0, GridUnitType.Star)
                                    };

                                    playerGrid.RowDefinitions.Add(gridRow1);
                                    playerGrid.RowDefinitions.Add(gridRow2);

                                    //add our custom controls to the player group box
                                    var tempTextblock = new TextBlock
                                    {
                                        Text = p.Nickname + " | " + p.AccountId,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 12.0,
                                        Foreground = background,
                                        Padding = new Thickness(5)
                                    };

                                    tempTextblock.MouseDown += PlayerDetailsMouseDown;
                                    tempTextblock.MouseEnter += PlayerDetailsMouseEnter;
                                    tempTextblock.MouseLeave += PlayerDetailsMouseLeave;

                                    Grid.SetRow(tempTextblock, 0);
                                    Grid.SetColumn(tempTextblock, 0);
                                    playerGrid.Children.Add(tempTextblock);

                                    var tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Battles:  " + p.Battles,
                                        FontSize = 12.0,
                                        Foreground = p.Battles > 100 ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.Black)
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Wins:  " + p.Wins,
                                        FontSize = 12.0
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg Dmg:  " + Math.Round((float)p.DamageDealt / (float)p.Battles, 0),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgDamageColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 0);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = p.ShipName,
                                        FontSize = 11.0
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 0);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg XP:  " + Math.Round((float)p.XpEarned / (float)p.Battles, 0),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgXpColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 1);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Win Rate:  " + Math.Round((float)p.Wins / (float)p.Battles * 100f, 2) + @"%",
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerWinRateColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 2);
                                    playerGrid.Children.Add(tempLabel);

                                    tempLabel = new System.Windows.Controls.Label
                                    {
                                        Content = @"Avg Frags:  " + Math.Round((float)p.Frags / (float)p.Battles, 2),
                                        FontSize = 12.0,
                                        Foreground = new SolidColorBrush(GetPlayerAvgFragsColor(p))
                                    };

                                    Grid.SetRow(tempLabel, 1);
                                    Grid.SetColumn(tempLabel, 3);
                                    playerGrid.Children.Add(tempLabel);

                                    EnemyGroup.Children.Add(playerGrid);

                                    //add a separator
                                    EnemyGroup.Children.Add(new Separator());
                                } //end else
                            } //end try
                            catch (Exception ex)
                            {
                                //ignore
                                Log("Exception Occurred Processing Player: " + ex.Message);
                            } //end catch
                        } //end foreach
                    } //end if

                    EnemyGroup.Visibility = Visibility.Visible;
                    Log("Enemy Group is Visible");

                    LblStatus.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    LblStatus.Content = @"Player Stats Succesfully Updated";

                    Log("Updated Status Label");
                } //end if
            } //end if
            else
            {
                //player is not playing, clear the results
                ClearResults();

                Log("Cleared the Results");
            } //end else
        } //end Checktimer_Tick

        private List<ShipModel> GetShips()
        {
            try
            {
                var client = new RestClient("https://wowreplays.com");

                //start by getting the players accountId so we can grab their stats
                var request = new RestRequest("/Home/GetShipsForMatchmakingMonitor", Method.POST);

                var results = client.Execute<List<ShipModel>>(request);

                return results.Data;
            } //end try
            catch (Exception ex)
            {
                Log("Exception Occurred While Retrieving Ships:  " + ex.Message);

                return new List<ShipModel>();
            } //end catch
        } //end GetShips

        private Color GetPlayerBackground(PlayerShipStats player)
        {
            //do we have battles before we begin?
            if (player.Battles == 0)
            {
                //no battles so we assume they are bad
                return Color.FromRgb(255, 0, 0);
            } //end if

            //we know there are battles so we can do our math now.
            //what do their stats look like?
            var playerTotal = 0;

            var winRate = Math.Round((float) player.Wins/(float) player.Battles*100f, 2);
            var avgFrags = Math.Round((float) player.Frags/(float) player.Battles, 2);
            var avgXp = Math.Round((float) player.XpEarned/(float) player.Battles, 0);
            var avgDamage = Math.Round((float) player.DamageDealt/(float) player.Battles, 0);

            //how many battles do they have in this ship?
            if (player.Battles > 100)
            {
                playerTotal += 20;
            } //end fi
            else if (player.Battles > 50)
            {
                playerTotal += 10;
            } //end else if
            else if (player.Battles > 20)
            {
                playerTotal += 5;
            } //end else if

            //win rate
            if (winRate > 70.0f)
            {
                playerTotal += 20;
            } //end if
            else if (winRate > 50.0f)
            {
                playerTotal += 15;
            } //end else if
            else if (winRate > 40.0f)
            {
                playerTotal += 10;
            } //end if
            else if (winRate > 30.0f)
            {
                playerTotal += 5;
            } //end else if

            //avg frags?
            if (avgFrags > 1.5f)
            {
                playerTotal += 20;
            } //end if
            else if (avgFrags > 1.0f)
            {
                playerTotal += 15;
            } //end else if
            else if (avgFrags > 0.75f)
            {
                playerTotal += 10;
            } //end else if
            else if (avgFrags > 0.5f)
            {
                playerTotal += 5;
            } //end else if

            //how about average xp
            if (avgXp > 1000.0f)
            {
                playerTotal += 20;
            } //end if
            else if (avgXp > 800.0f)
            {
                playerTotal += 15;
            } //end else if
            else if (avgXp > 600.0f)
            {
                playerTotal += 10;
            } //end else if
            else if (avgXp > 300.0f)
            {
                playerTotal += 5;
            } //end else if

            //average damage
            if (avgDamage > 75000.0f)
            {
                playerTotal += 20;
            } //end if
            else if (avgDamage > 60000.0f)
            {
                playerTotal += 15;
            } //end else if
            else if (avgDamage > 45000.0f)
            {
                playerTotal += 10;
            } //end else if
            else if (avgDamage > 25000.0f)
            {
                playerTotal += 5;
            } //end else if

            if (playerTotal > 90)
            {
                return Color.FromRgb(184, 0, 169);
            } //end if

            if (playerTotal > 80)
            {
                return Color.FromRgb(0, 148, 17);
            } //end if

            if (playerTotal > 70)
            {
                return Color.FromRgb(106, 188, 0);
            } //en dif

            if (playerTotal > 60)
            {
                return Color.FromRgb(155, 188, 0);
            } //end if

            if (playerTotal > 50)
            {
                return Color.FromRgb(188, 181, 0);
            } //end if

            if (playerTotal > 40)
            {
                return Color.FromRgb(255, 174, 0);
            } //end if

            if (playerTotal > 30)
            {
                return Color.FromRgb(255, 114, 0);
            } //end if

            if (playerTotal > 20)
            {
                return Color.FromRgb(255, 66, 0);
            } //end if

            if (playerTotal > 10)
            {
                return Color.FromRgb(255, 0, 0);
            } //end if

            return Color.FromRgb(255, 0, 0);
        } //end GetPlayerBackground

        private Color GetPlayerWinRateColor(PlayerShipStats player)
        {
            //do we have battles before we begin?
            if (player.Battles == 0)
            {
                //no battles so we assume they are bad
                return Color.FromRgb(255, 0, 0);
            } //end if

            var winRate = Math.Round((float)player.Wins / (float)player.Battles * 100f, 2);

            if (winRate > 90)
            {
                return Color.FromRgb(184, 0, 169);
            } //end if

            if (winRate > 80)
            {
                return Color.FromRgb(0, 148, 17);
            } //end if

            if (winRate > 70)
            {
                return Color.FromRgb(106, 188, 0);
            } //en dif

            if (winRate > 60)
            {
                return Color.FromRgb(155, 188, 0);
            } //end if

            if (winRate > 50)
            {
                return Color.FromRgb(188, 181, 0);
            } //end if

            if (winRate > 40)
            {
                return Color.FromRgb(255, 174, 0);
            } //end if

            if (winRate > 30)
            {
                return Color.FromRgb(255, 114, 0);
            } //end if

            if (winRate > 20)
            {
                return Color.FromRgb(255, 66, 0);
            } //end if

            if (winRate > 10)
            {
                return Color.FromRgb(255, 0, 0);
            } //end if

            return Color.FromRgb(255, 0, 0);
        } //end GetPlayerBackground

        private Color GetPlayerAvgXpColor(PlayerShipStats player)
        {
            //do we have battles before we begin?
            if (player.Battles == 0)
            {
                //no battles so we assume they are bad
                return Color.FromRgb(255, 0, 0);
            } //end if

            var avgXp = Math.Round((float)player.XpEarned / (float)player.Battles, 0);

            if (avgXp > 1500)
            {
                return Color.FromRgb(184, 0, 169);
            } //end if

            if (avgXp > 1200)
            {
                return Color.FromRgb(0, 148, 17);
            } //end if

            if (avgXp > 1000)
            {
                return Color.FromRgb(106, 188, 0);
            } //en dif

            if (avgXp > 900)
            {
                return Color.FromRgb(155, 188, 0);
            } //end if

            if (avgXp > 800)
            {
                return Color.FromRgb(188, 181, 0);
            } //end if

            if (avgXp > 600)
            {
                return Color.FromRgb(255, 174, 0);
            } //end if

            if (avgXp > 500)
            {
                return Color.FromRgb(255, 114, 0);
            } //end if

            if (avgXp > 400)
            {
                return Color.FromRgb(255, 66, 0);
            } //end if

            return Color.FromRgb(255, 0, 0);
        } //end GetPlayerBackground

        private Color GetPlayerAvgFragsColor(PlayerShipStats player)
        {
            //do we have battles before we begin?
            if (player.Battles == 0)
            {
                //no battles so we assume they are bad
                return Color.FromRgb(255, 0, 0);
            } //end if

            var avgFrags = Math.Round((float)player.Frags / (float)player.Battles, 2);

            if (avgFrags > 1.5)
            {
                return Color.FromRgb(184, 0, 169);
            } //end if

            if (avgFrags > 1.3)
            {
                return Color.FromRgb(0, 148, 17);
            } //end if

            if (avgFrags > 1.1)
            {
                return Color.FromRgb(106, 188, 0);
            } //en dif

            if (avgFrags > 1.0)
            {
                return Color.FromRgb(155, 188, 0);
            } //end if

            if (avgFrags > 0.8)
            {
                return Color.FromRgb(188, 181, 0);
            } //end if

            if (avgFrags > 0.6)
            {
                return Color.FromRgb(255, 174, 0);
            } //end if

            if (avgFrags > .4)
            {
                return Color.FromRgb(255, 114, 0);
            } //end if

            if (avgFrags > .2)
            {
                return Color.FromRgb(255, 66, 0);
            } //end if

            return Color.FromRgb(255, 0, 0);
        } //end GetPlayerBackground

        private Color GetPlayerAvgDamageColor(PlayerShipStats player)
        {
            //do we have battles before we begin?
            if (player.Battles == 0)
            {
                //no battles so we assume they are bad
                return Color.FromRgb(255, 0, 0);
            } //end if

            var avgDamage = Math.Round((float)player.DamageDealt / (float)player.Battles, 0);

            if (avgDamage > 75000)
            {
                return Color.FromRgb(184, 0, 169);
            } //end if

            if (avgDamage > 65000)
            {
                return Color.FromRgb(0, 148, 17);
            } //end if

            if (avgDamage > 55000)
            {
                return Color.FromRgb(106, 188, 0);
            } //en dif

            if (avgDamage > 45000)
            {
                return Color.FromRgb(155, 188, 0);
            } //end if

            if (avgDamage > 35000)
            {
                return Color.FromRgb(188, 181, 0);
            } //end if

            if (avgDamage > 25000)
            {
                return Color.FromRgb(255, 174, 0);
            } //end if

            if (avgDamage > 15000)
            {
                return Color.FromRgb(255, 114, 0);
            } //end if

            if (avgDamage > 10000)
            {
                return Color.FromRgb(255, 66, 0);
            } //end if

            return Color.FromRgb(255, 0, 0);
        } //end GetPlayerBackground

        private void Log(string message)
        {
            try
            {
                using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Log.txt", true))
                {
                    sw.WriteLine(DateTime.Now + " - " + message);
                } //end using
            } //end try
            catch (Exception ex)
            {
                //ignore
            } //end catch
        } //end Log

        private void PlayerDetailsMouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var label = (TextBlock) sender;
                var playerName = label.Text.Split('|')[0].Replace(" ", "");
                var accountId = label.Text.Split('|')[1].Replace(" ", "");

                System.Diagnostics.Process.Start("https://" + Properties.Settings.Default.Region +
                                                 ".warships.today/player/" + accountId + "/" + playerName);
            } //end try
            catch (Exception ex)
            {
                Log("Exception Thrown on Click Event for Player: " + ex.Message);
            } //end catch
        } //end PlayerDetailsMouseDown

        private void PlayerDetailsMouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
            } //end try
            catch (Exception ex)
            {
                Log("Exception Thrown Changing Mouse Cursor: " + ex.Message);

                Mouse.OverrideCursor = null;
            } //end catch
        } //end PlayerDetailsMouseEnter

        private void PlayerDetailsMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.OverrideCursor = null;
        } //end PlayerDetailsMouseLeave

        private void LogoClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://wowreplays.com");
            } //end try
            catch (Exception ex)
            {
                Log("Exception Throw on Logo Click: " + ex.Message);
            } //end catch
        } //end LogoClick

        private void LogoMouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
            } //end try
            catch (Exception ex)
            {
                Log("Exception Thrown Changing Mouse Cursor: " + ex.Message);

                Mouse.OverrideCursor = null;
            } //end catch
        } //end LogoMouseEnter

        private void LogoMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        } //end LogoMouseLeave

        private void InstallDirectoryMouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
            } //end try
            catch (Exception ex)
            {
                Log("Exception Thrown Changing Mouse Cursor: " + ex.Message);

                Mouse.OverrideCursor = null;
            } //end catch
        } //end InstallDirectoryMouseEnter

        private void InstallDirectoryMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        } //end InstallDirectoryMouseEnter
    } //end class
} //end namespace