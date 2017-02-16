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
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"].ToString();
                } //end if
                else
                {
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"] +
                                                       " - Invalid Path or Replays not enabled! - Click here to update!";
                } //end else
            } //end try
            catch (Exception)
            {
                //ignore
                LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"] +
                                                   " - Invalid Path or Replays not enabled! - Click here to update!";
            } //end catch
        }

        private void InstallDirectoryClick(object sender, MouseButtonEventArgs e)
        {
            //the directory does not exist in the default location, ask the user to tell us where it is.
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowser.SelectedPath))
            {
                if (Directory.Exists(folderBrowser.SelectedPath + "/replays"))
                {
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.LawnGreen);
                    LblInstallDirectoryValue.Content = folderBrowser.SelectedPath;

                    Properties.Settings.Default["InstallDirectory"] = folderBrowser.SelectedPath;
                    Properties.Settings.Default.Save();
                } //end if
                else
                {
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"] +
                                                       " - Invalid Path or Replays not enabled! - Click here to update!";
                } //end else
            } //end if
            else if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                if (Directory.Exists(Properties.Settings.Default["InstallDirectory"] + "/replays"))
                {
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.LawnGreen);
                    LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"].ToString();
                } //end if
                else
                {
                    LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    LblInstallDirectoryValue.Content = Properties.Settings.Default["InstallDirectory"] +
                                                       " - Invalid Path or Replays not enabled! - Click here to update!";
                } //end else
            } //end else if
            else
            {
                LblInstallDirectoryValue.Foreground = new SolidColorBrush(Colors.OrangeRed);
                LblInstallDirectoryValue.Content = "Please Choose a Valid Path!";
            } //end else
        } //end InstallDirectoryClick

        private void ComRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update the players choice
            Properties.Settings.Default["Region"] = ComRegion.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        } //end ComRegion_SelectionChanged

        private async Task<List<PlayerShipStats>> FetchResults(ReplayModel replay, string region)
        {
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

                        switch (region)
                        {
                            case "EU":
                                client = new RestClient("https://api.worldofwarships.eu");
                                break;
                            case "SEA":
                                client = new RestClient("https://api.worldofwarships.asia");
                                break;
                            case "RU":
                                client = new RestClient("https://api.worldofwarships.ru");
                                break;
                            default:
                                client = new RestClient("https://api.worldofwarships.com");
                                break;
                        } //end switch

                        //start by getting the players accountId so we can grab their stats
                        var request = new RestRequest("/wows/account/list/?application_id={appId}&search={username}",
                            Method.GET);
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
                                    Battles = 0
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
                                        ShipName = _ships.FirstOrDefault(x => x.ShipId == s.ship_id).ShipName,
                                        ShipType = _ships.FirstOrDefault(x => x.ShipId == s.ship_id).ShipType,
                                        ShipTier = _ships.FirstOrDefault(x => x.ShipId == s.ship_id).Tier,
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
                            } //end else
                        } //end if
                        else
                        {
                            //no stats
                            //we didn't find this player set stats to 0
                            var stat = new PlayerShipStats
                            {
                                ShipId = long.Parse(p.shipId.ToString()),
                                Battles = 0
                            };

                            retList.Add(stat);
                        } //end else
                    } //end try
                    catch (Exception)
                    {
                        //ignore
                    } //end catch
                });

                return retList;
            });
        } //end FetchResults

        private void ClearResults()
        {
            LblStatus.Foreground = new SolidColorBrush(Colors.Black);
            LblStatus.Content = "Not Currently in Battle - Stats are from Last Battle";
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

        private static bool IsNewReplay(ReplayModel newReplay)
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

                        return true;
                    } //end if
                } //end if
                else
                {
                    //this is a new replay, update our settings
                    Properties.Settings.Default["LastReplay"] = newReplay.dateTime;
                    Properties.Settings.Default.Save();

                    //this is a new replay since the LastReplay is empty
                    return true;
                } //end else
            } //end try
            catch (Exception)
            {
                //ignore
            } //end catch

            return false;
        } //end IsNewReplay

        private static ReplayModel GetReplayDetails()
        {
            using (
                var sr =
                    new StreamReader(Properties.Settings.Default["InstallDirectory"] + "/replays/tempArenaInfo.json"))
            {
                return JsonConvert.DeserializeObject<ReplayModel>(sr.ReadToEnd());
            } //end using
        } //end GetReplayDetails

        private static bool CurrentlyPlaying()
        {
            return File.Exists(Properties.Settings.Default["InstallDirectory"] + "/replays/tempArenaInfo.json");
        } //end CurrentlyPlaying

        private async void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentlyPlaying())
            {
                //sleep for a few seconds while the game writes the file we need.
                System.Threading.Thread.Sleep(5000);

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
                } //end catch

                //we didn't fall into the catch so we have a valid json string here
                //is this the same replay as before or did the player move on to another battle?
                if (IsNewReplay(replayData) && replayData != null)
                {
                    //this is a new replay, let's go get the details
                    LblStatus.Foreground = new SolidColorBrush(Colors.Goldenrod);
                    LblStatus.Content = "Fetching Player Stats for Current Battle";

                    var playerStats = await FetchResults(replayData, ComRegion.SelectedItem.ToString());

                    if (playerStats.Any())
                    {
                        //start by removing all previous controls
                        FriendlyGroup.Children.Clear();
                        EnemyGroup.Children.Clear();

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

                        FriendlyGroup.Children.Add(new Separator());
                        EnemyGroup.Children.Add(new Separator());

                        //start with the friendly team.
                        foreach (var p in playerStats.Where(x => x.Relationship == 0 || x.Relationship == 1).OrderBy(x => x.ShipType).ThenByDescending(x => x.ShipTier))
                        {
                            try
                            {
                                var playerGrid = new Grid
                                {
                                    Height = 44
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
                                    Height = new GridLength(22, GridUnitType.Pixel)
                                };

                                var gridRow2 = new RowDefinition
                                {
                                    Height = new GridLength(22, GridUnitType.Pixel)
                                };

                                playerGrid.RowDefinitions.Add(gridRow1);
                                playerGrid.RowDefinitions.Add(gridRow2);

                                //add our custom controls to the player group box
                                var tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = p.Nickname,
                                    FontWeight = FontWeights.Bold,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 0);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Battles:  " + p.Battles,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 1);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Wins:  " + p.Wins,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 2);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Frags:  " + p.Frags,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 3);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = p.ShipName,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 0);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Avg XP:  " + Math.Round((float) p.XpEarned/(float) p.Battles, 0),
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 1);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content =
                                        @"Win Rate:  " + Math.Round((float) p.Wins/(float) p.Battles*100f, 2) + @"%",
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 2);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Avg Frags:  " + Math.Round((float) p.Frags/(float) p.Battles, 2),
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 3);
                                playerGrid.Children.Add(tempLabel);

                                FriendlyGroup.Children.Add(playerGrid);
                                FriendlyGroup.Visibility = Visibility.Visible;

                                //add a separator
                                FriendlyGroup.Children.Add(new Separator());
                            } //end try
                            catch (Exception)
                            {
                                //ignore
                            } //end catch
                        } //end foreach

                        //now the enemy team
                        foreach (var p in playerStats.Where(x => x.Relationship == 2).OrderBy(x => x.ShipType).ThenByDescending(x => x.ShipTier))
                        {
                            try
                            {
                                var playerGrid = new Grid
                                {
                                    Height = 44
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
                                    Height = new GridLength(22, GridUnitType.Pixel)
                                };

                                var gridRow2 = new RowDefinition
                                {
                                    Height = new GridLength(22, GridUnitType.Pixel)
                                };

                                playerGrid.RowDefinitions.Add(gridRow1);
                                playerGrid.RowDefinitions.Add(gridRow2);

                                //add our custom controls to the player group box
                                var tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = p.Nickname,
                                    FontWeight = FontWeights.Bold,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 0);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Battles:  " + p.Battles,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 1);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Wins:  " + p.Wins,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 2);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Frags:  " + p.Frags,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 0);
                                Grid.SetColumn(tempLabel, 3);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = p.ShipName,
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 0);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Avg XP:  " + Math.Round((float) p.XpEarned/(float) p.Battles, 0),
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 1);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content =
                                        @"Win Rate:  " + Math.Round((float) p.Wins/(float) p.Battles*100f, 2) + @"%",
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 2);
                                playerGrid.Children.Add(tempLabel);

                                tempLabel = new System.Windows.Controls.Label
                                {
                                    Content = @"Avg Frags:  " + Math.Round((float) p.Frags/(float) p.Battles, 2),
                                    FontSize = 9.0
                                };

                                Grid.SetRow(tempLabel, 1);
                                Grid.SetColumn(tempLabel, 3);
                                playerGrid.Children.Add(tempLabel);

                                EnemyGroup.Children.Add(playerGrid);
                                EnemyGroup.Visibility = Visibility.Visible;

                                //add a separator
                                EnemyGroup.Children.Add(new Separator());
                            } //end try
                            catch (Exception)
                            {
                                //ignore
                            } //end catch
                        } //end foreach
                    } //end if

                    LblStatus.Foreground = new SolidColorBrush(Color.FromRgb(17, 143, 19));
                    LblStatus.Content = @"Player Stats Succesfully Updated";
                } //end if
            } //end if
            else
            {
                //player is not playing, clear the results
                ClearResults();
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
                return new List<ShipModel>();
            } //end catch
        } //end GetShips
    } //end class
} //end namespace
