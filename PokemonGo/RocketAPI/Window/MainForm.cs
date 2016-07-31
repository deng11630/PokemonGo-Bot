using System;
using System.Collections.Generic; 
using System.Data;
using System.Drawing;       
using System.IO;
using System.Linq;
using System.Net;                   
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode; 
using System.Threading;     
using BrightIdeasSoftware;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public static SynchronizationContext synchronizationContext;
        public static int totalExperience = 0;
        public static bool unbanning = false;
        public static bool stop = false;
        public static bool stopInventoryActions = false;
        public static MainForm instance;
        public static ISettings ClientSettings;
        public static int Currentlevel = -1;     
        public static GetPlayerResponse profile;
        private static DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;  
        public static bool connected = false;
        public static Client client;
        public static Task inventoryActions;

        private void MainForm_Load(object sender, EventArgs e)
        {
            Map.Load(gMapControl1);
            InitializePokemonForm();
        }



        private void InitializePokemonForm()
        {
            objectListView1.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate (object rowObject)
            {
                PokemonData pokemon = (PokemonData)rowObject;

                String key = pokemon.PokemonId.ToString();
                if (!objectListView1.SmallImageList.Images.ContainsKey(key))
                {
                    Image img = GetPokemonImage((int)pokemon.PokemonId);
                    objectListView1.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            objectListView1.CellToolTipShowing += delegate (object sender, ToolTipShowingEventArgs args)
            {
                PokemonData pokemon = (PokemonData)args.Model;
                if (args.ColumnIndex == 8)
                {
                    int candyOwned = Inventory.GetFamilies()
                            .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                            .Select(f => f.Candy)
                            .First();
                    args.Text = candyOwned + " Candy";
                }
            };
        }


        private Image GetPokemonImage(int pokemonId)
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";
            string location = Sprites + pokemonId + ".png";
            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);
            if (!File.Exists(location))
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png", @location);
            }
            return Image.FromFile(location);
        }

        private void PokemonListButton_Click(object sender, CellClickEventArgs e)
        {
            try
            {
                PokemonData pokemon = (PokemonData)e.Model;
                if (e.ColumnIndex == 6)
                {
                    TransferPokemon(pokemon);
                }
                else if (e.ColumnIndex == 7)
                {
                    PowerUpPokemon(pokemon);
                }
                else if (e.ColumnIndex == 8)
                {
                    EvolvePokemon(pokemon);
                }
            }
            catch (Exception ex) { ConsoleWriter.ColoredConsoleWrite(Color.Red, ex.ToString()); client = null; ReloadPokemonList(); }
        }


        private async void TransferPokemon(PokemonData pokemon)
        {
            if (MessageBox.Show($"Are you sure you want to transfer {pokemon.PokemonId.ToString()} with {pokemon.Cp} CP?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);
                string message = "";
                string caption = "";

                if (transferPokemonResponse.Status == 1)
                {
                    message = $"{pokemon.PokemonId} was transferred\n{transferPokemonResponse.CandyAwarded} candy awarded";
                    caption = $"{pokemon.PokemonId} transferred";
                    ReloadPokemonList();
                }
                else
                {
                    message = $"{pokemon.PokemonId} could not be transferred";
                    caption = $"Transfer {pokemon.PokemonId} failed";
                }

                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void PowerUpPokemon(PokemonData pokemon)
        {
            var evolvePokemonResponse = await client.PowerUp(pokemon.Id);
            string message = "";
            string caption = "";

            if (evolvePokemonResponse.Result == 1)
            {
                message = $"{pokemon.PokemonId} successfully upgraded.";
                caption = $"{pokemon.PokemonId} upgraded";
                ReloadPokemonList();
            }
            else
            {
                message = $"{pokemon.PokemonId} could not be upgraded";
                caption = $"Upgrade {pokemon.PokemonId} failed";
            }

            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void EvolvePokemon(PokemonData pokemon)
        {
            var evolvePokemonResponse = await client.EvolvePokemon(pokemon.Id);
            string message = "";
            string caption = "";

            if (evolvePokemonResponse.Result == 1)
            {
                message = $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemon.PokemonType}\n{evolvePokemonResponse.ExpAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded";
                caption = $"{pokemon.PokemonId} evolved into {evolvePokemonResponse.EvolvedPokemon.PokemonType}";
                ReloadPokemonList();
            }
            else
            {
                message = $"{pokemon.PokemonId} could not be evolved";
                caption = $"Evolve {pokemon.PokemonId} failed";
            }

            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private async void Execute()
        {
            unbanning = false;
            stop = false;
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
                inventoryActions = InventoryActions(2000, 10);
                connected = true;       
                await Movements.FarmAllWithSaving(client);
                await ForceUnban(client);
            }
            catch (TaskCanceledException)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting");  Restart(client);
            }
            catch (UriFormatException)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting");   Restart(client);
            }
            catch (ArgumentOutOfRangeException e) { e = e; ConsoleWriter.ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting"); await Restart(client); }
            catch (ArgumentNullException e) { e = e; ConsoleWriter.ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting");  Restart(client); }
            catch (NullReferenceException e) { e = e; ConsoleWriter.ColoredConsoleWrite(Color.Red, "Null Refference - Restarting");  Restart(client); }
            catch (Exception e) { e = e; ConsoleWriter.ColoredConsoleWrite(Color.Red, e.ToString()); Restart(client); }
        }


        public MainForm()
        {
            instance = this;
            synchronizationContext = SynchronizationContext.Current;
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


        private async Task Restart(Client client)
        {

            stopInventoryActions = true;
            await inventoryActions;
            stopInventoryActions = false;
            ConsoleWriter.ColoredConsoleWrite(Color.Red, "Bot stopped.");
            if (!stop)
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
            Inventory.UpdateInventory(client);
            if (nb++ > 30)
            {
                await client.RecycleItems(Inventory.items);
                nb = 0;
            }
            ConsoleWriter.ConsoleLevelTitle(profile.Profile.Username, client, Inventory.inventory);
            ConsoleWriter.PrintLevel(client, Inventory.inventory);
            PokemonActions.EvolveAndTransfert(client, Inventory.inventory);
            await Task.Delay(delay);
            if (!stopInventoryActions)
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
            if (!unbanning)
                return;
            await Task.Delay(5000);
            unbanning = false;
            ConsoleWriter.ColoredConsoleWrite(Color.LightGreen, "Starting force unban...");


            var mapObjects = await client.GetMapObjects();
            var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime()).ToList();
            bool done = false;
            foreach (var pokeStop in pokeStops)
            {

                double pokeStopDistance = Movements.locationManager.getDistance(pokeStop.Latitude, pokeStop.Longitude);
                await Movements.locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
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
                    ConsoleWriter.ColoredConsoleWrite(Color.White, "Force unban failed, please try again.");
                else
                    break;
            }
            await Restart(client);
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
                stop = true;
                stopInventoryActions = true;
                if (!unbanning)
                {
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
              
                    IEnumerable<Item> LuckyEggs = Inventory.items.Where(i => (ItemId)i.Item_ == ItemId.ItemLuckyEgg);
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
                unbanning = true;
                ConsoleWriter.ColoredConsoleWrite(Color.Red, "Start unban. Please wait while we stop the bot.");
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

        private void button1_Click(object sender, EventArgs e)
        {
            ReloadPokemonList();
        }

        private async void ReloadPokemonList()
        {
            button1.Enabled = false;
            objectListView1.Enabled = false;
            var currentScrollPosition = objectListView1.LowLevelScrollPosition;
            objectListView1.SetObjects(Inventory.pokemons);
            objectListView1.LowLevelScroll(currentScrollPosition.X, currentScrollPosition.Y);
            button1.Enabled = true;
            objectListView1.Enabled = true;
        }



        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mForm = new MapForm(ref client);
            mForm.Show();
        }

        private void pokémonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemonList = new PokemonList();
            pokemonList.Show();
        }
    }
}
