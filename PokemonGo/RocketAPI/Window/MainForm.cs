using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Window;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {

        public static int totalExperience = 0;
        public static bool forceUnbanning = false;
        public static bool stopping = false;
        public static MainForm instance;
        public static ISettings ClientSettings;
        public static int Currentlevel = -1;
        private static int TotalExperience = 0;
        public static GetPlayerResponse profile;
        private static DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;
        private static bool stopPrintLevel = false;
        public static bool connected = false;
        private Task recycle = null;
        private Task printLevel = null;
        public static Client client;
        LocationManager locationManager;

        private async void Execute()
        {
            client = new Client(ClientSettings);
            PokemonActions.client = client;
            Movements.locationManager = new LocationManager(client, ClientSettings.TravelSpeed);
            try
            {
                await Connect();
                await client.SetServer();
                profile = await client.GetProfile();
                var settings = await client.GetSettings();
                ConsoleWriter.WriteProfile(profile);
                InventoryActions(2000, 10);
                connected = true;
                client.stopRecycle = false;
                stopPrintLevel = false;
                await Movements.GoWhereYouWantButFarmStock(client);
                await UnbannedCheck();
            }
            catch (TaskCanceledException)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting"); if (!stopping) await Restart(client);
            }
            catch (UriFormatException)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting"); if (!stopping) await Restart(client);
            }
            catch (ArgumentOutOfRangeException) { ConsoleWriter.ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting"); if (!stopping) await Restart(client); }
            catch (ArgumentNullException) { ConsoleWriter.ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting"); if (!stopping) await Restart(client); }
            catch (NullReferenceException e) { e = e; ConsoleWriter.ColoredConsoleWrite(Color.Red, "Null Refference - Restarting"); if (!stopping) await Restart(client); }
            catch (Exception ex) { ConsoleWriter.ColoredConsoleWrite(Color.Red, ex.ToString()); if (!stopping) await Restart(client); }
        }


        public MainForm()
        {
            instance = this;
            ReadSettings.Load(Settings.Instance);
            ClientSettings = Settings.Instance;
            InitializeComponent();
        }

        public static double GetRuntime()
        {
            return ((DateTime.Now - TimeStarted).TotalSeconds) / 3600;
        }


        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/DetectiveSquirrel/Pokemon-Go-Rocket-API/master/PokemonGo/RocketAPI/Window/Properties/AssemblyInfo.cs");
        }


        public void ConsoleClear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ConsoleClear));
                return;
            }

            logTextBox.Clear();
        }

        public void SetStatusText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatusText), text);
                return;
            }

            statusLabel.Text = text;
        }


        private async Task UnbannedCheck()
        {
            while (forceUnbanning)
                await Task.Delay(25);
            // await ForceUnban(client);
            if (!stopping)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"No nearby useful locations found. Please wait 10 seconds.");
                await Task.Delay(10000);
                Restart(client);
            }
            else
            {
                ConsoleClear();
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
                startStopBotToolStripMenuItem.Text = "Start";
                stopping = false;
                bot_started = false;
            }
        }


        private async Task Restart(Client client)
        {
            client.stopRecycle = true;
            stopPrintLevel = true;
            if (recycle != null)
                await recycle;
            if (printLevel != null)
                await printLevel;
            Execute();
        }


        private async Task Connect()
        {
            switch (ReadSettings.authType)
            {
                case AuthType.Ptc:
                    ConsoleWriter.ColoredConsoleWrite(Color.Green, "Login Type: Pokemon Trainers Club");
                    await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                    break;
                case AuthType.Google:
                    ConsoleWriter.ColoredConsoleWrite(Color.Green, "Login Type: Google");
                    await client.DoGoogleLogin(ClientSettings.Email, ClientSettings.Password);
                    break;
            }
        }

       

        public static async Task InventoryActions(int delay, int nb)
        {
            var inventory = await client.GetInventory();
            if (nb++ > 30)
            {
                await client.RecycleInventory(inventory);
                nb = 0;
            }
            ConsoleWriter.ConsoleLevelTitle(profile.Profile.Username, client, inventory);
            ConsoleWriter.PrintLevel(client, inventory);
            PokemonActions.EvolveAndTransfert(client, inventory);
            await Task.Delay(delay);
            InventoryActions(delay, nb);
        }

        private static string CallAPI(string elem, double lat, double lon)
        {
            using (XmlReader reader = XmlReader.Create(@"http://api.geonames.org/findNearby?lat=" + lat + "&lng=" + lon + "&username=demo"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (elem)
                        {
                            case "country":
                                if (reader.Name == "countryName")
                                {
                                    return reader.ReadString();
                                }
                                break;

                            case "place":
                                if (reader.Name == "toponymName")
                                {
                                    return reader.ReadString();
                                }
                                break;
                            default:
                                return "N/A";
                                break;
                        }
                    }
                }
            }
            return "Error";
        }










        private async Task ForceUnban(Client client)
        {
            if (!forceUnbanning && !stopping)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Waiting for last farming action to be complete...");
                forceUnbanning = true;
                while (Movements.farmingStops || PokemonActions.farmingPokemons)
                    await Task.Delay(1);

                ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Starting force unban...");
                var mapObjects = await client.GetMapObjects();
                var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
                await Task.Delay(10000);
                bool done = false;
                foreach (var pokeStop in pokeStops)
                {

                    double pokeStopDistance = locationManager.getDistance(pokeStop.Latitude, pokeStop.Longitude);
                    await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                    var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                    if (fortInfo.Name != string.Empty)
                    {
                        ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Chosen PokeStop " + fortInfo.Name + " for force unban");
                        for (int i = 1; i <= 50; i++)
                        {
                            var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                            if (fortSearch.ExperienceAwarded == 0)
                                ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Attempt: " + i);
                            else
                            {
                                ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Fuck yes, you are now unbanned! Total attempts: " + i);
                                done = true;
                                break;
                            }
                        }
                    }

                    if (!done)
                        ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Force unban failed, please try again.");
                    forceUnbanning = false;
                    break;
                }
                return;
            }
            ConsoleWriter.ColoredConsoleWrite(Color.Red, "A action is in play... Please wait.");
        }










        // Pulled from NecronomiconCoding
        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }



        private void logTextBox_TextChanged(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private static bool bot_started = false;
        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!bot_started)
            {
                bot_started = true;
                startStopBotToolStripMenuItem.Text = "Stop Bot";
                Task.Run(() =>
                {
                    try
                    {
                        ConsoleWriter.PrintVersion();
                        Execute();
                    }
                    catch (PtcOfflineException)
                    {
                        ConsoleWriter.ColoredConsoleWrite(Color.Red, "PTC Servers are probably down OR your credentials are wrong. Try google");
                    }
                    catch (Exception ex)
                    {
                        ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Unhandled exception: {ex}");
                    }
                });
            }
            else
            {
                if (!forceUnbanning)
                {
                    stopping = true;
                    ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Stopping the bot.. Waiting for the last action to be complete.");
                }
                else
                    ConsoleWriter.ColoredConsoleWrite(Color.Red, $"An action is in play, please wait until it's done.");
            }
        }

        private void showAllToolStripMenuItem3_Click(object sender, EventArgs e)
        {
        }

        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo: add player stats later
        }

        private async void useLuckyEggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                try
                {
                    IEnumerable<Item> myItems = await client.GetItems(client);
                    IEnumerable<Item> LuckyEggs = myItems.Where(i => (ItemId)i.Item_ == ItemId.ItemLuckyEgg);
                    Item LuckyEgg = LuckyEggs.FirstOrDefault();
                    if (LuckyEgg != null)
                    {
                        var useItemXpBoostRequest = await client.UseItemXpBoost(ItemId.ItemLuckyEgg);
                        ConsoleWriter.ColoredConsoleWrite(Color.Green, $"Using a Lucky Egg, we have {LuckyEgg.Count} left.");
                        ConsoleWriter.ColoredConsoleWrite(Color.Yellow, $"Lucky Egg Valid until: {DateTime.Now.AddMinutes(30).ToString()}");

                        var stripItem = sender as ToolStripMenuItem;
                        stripItem.Enabled = false;
                        await Task.Delay(30000);
                        stripItem.Enabled = true;
                    }
                    else
                    {
                        ConsoleWriter.ColoredConsoleWrite(Color.Red, $"You don't have any Lucky Egg to use.");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Unhandled exception in using lucky egg: {ex}");
                }
            }
            else
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "Please start the bot before trying to use a lucky egg.");
            }
        }

        private async void forceUnbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                if (forceUnbanning)
                    ConsoleWriter.ColoredConsoleWrite(Color.Red, "A force unban attempt is in action... Please wait.");
                else
                    await ForceUnban(client);
            }
            else
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "Please start the bot before trying to force unban");
        }

        private void showAllToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void pokemonToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var pForm = new PokeUi();
            pForm.Show();
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mForm = new MapForm(ref client);
            mForm.Show();
        }


    }
}
