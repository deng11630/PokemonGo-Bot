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

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ClientSettings = Settings.Instance;
        }

        public static int nbPokemons;

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        public static ISettings ClientSettings;
        private static int Currentlevel = -1;
        private static int TotalExperience = 0;
        private static int TotalPokemon = 0;
        private static bool Stopping = false;
        private static bool ForceUnbanning = false;
        private static bool FarmingStops = false;
        private static bool FarmingPokemons = false;
        private static DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;
        private static bool stopPrintLevel = false;
        public static bool connected = false;
        private Task recycle = null;
        private Task printLevel = null;


        Client client;
        LocationManager locationManager;
        public static double GetRuntime()
        {
            return ((DateTime.Now - TimeStarted).TotalSeconds) / 3600;
        }

        public void CheckVersion()
        {
            try
            {  
                // makes sense to display your version and say what the current one is on github
                ColoredConsoleWrite(Color.Green, "Your version is 2.0.1" );
                ColoredConsoleWrite(Color.Green, "You can find it at www.github.com/mfron/Pokemon-Go-Bot");
            }
            catch (Exception)
            {
                ColoredConsoleWrite(Color.Red, "Unable to check for updates now...");
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/DetectiveSquirrel/Pokemon-Go-Rocket-API/master/PokemonGo/RocketAPI/Window/Properties/AssemblyInfo.cs");
        }

        public void ColoredConsoleWrite(Color color, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }

            logTextBox.Select(logTextBox.Text.Length, 1); // Reset cursor to last

            string textToAppend = "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\r\n";
            logTextBox.SelectionColor = color;
            logTextBox.AppendText(textToAppend);

            //object syncRoot = new object();
            //lock (syncRoot) // Added locking to prevent text file trying to be accessed by two things at the same time
            //{
            //    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Logs.txt", "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\n");
            //}
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



        private async Task EvolveAndTransfert(Client client)
        {
            var inventory = await client.GetInventory();
            var pokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);
            nbPokemons = pokemons.Count();
            if (pokemons.Count() < 230)
                return;

            if (ClientSettings.EvolveAllGivenPokemons)
                await EvolveAllGivenPokemons(client, pokemons.
                    Where(p => toEvolve.Contains(p.PokemonId)).OrderByDescending(p => Perfect(p)).ToList());
            switch (ClientSettings.TransferType)
            {
                case "Leave Strongest":
                    await TransferAllButStrongestUnwantedPokemon(client);
                    break;
                case "All":
                    await TransferAllGivenPokemons(client, pokemons);
                    break;
                case "Duplicate":
                    await TransferDuplicatePokemon(client);
                    break;
                case "IV Duplicate":
                    await TransfertAllButBestIV(client);
                    break;
                case "CP":
                    await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);
                    break;
                case "IV"   :
                    await TransfertAllBellowIV(client);
                    break;
                default:
                    ColoredConsoleWrite(Color.DarkGray, "Transfering pokemon disabled");
                    break;
            }
        }

        private async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve)
        {
            foreach (var pokemon in pokemonToEvolve)
            {
                /*
                enum Holoholo.Rpc.Types.EvolvePokemonOutProto.Result {
	                UNSET = 0;
	                SUCCESS = 1;
	                FAILED_POKEMON_MISSING = 2;
	                FAILED_INSUFFICIENT_RESOURCES = 3;
	                FAILED_POKEMON_CANNOT_EVOLVE = 4;
	                FAILED_POKEMON_IS_DEPLOYED = 5;
                }
                }*/

                var countOfEvolvedUnits = 0;
                var xpCount = 0;

                EvolvePokemonOut evolvePokemonOutProto;
                do
                {
                    evolvePokemonOutProto = await client.EvolvePokemon(pokemon.Id);
                    //todo: someone check whether this still works

                    if (evolvePokemonOutProto.Result == 1)
                    {
                        ColoredConsoleWrite(Color.Cyan,
                            $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded} xp {pokemon.Cp} cp and IV {Perfect(pokemon)}%");
                        TotalExperience += (evolvePokemonOutProto.ExpAwarded);
                        countOfEvolvedUnits++;
                        xpCount += evolvePokemonOutProto.ExpAwarded;
                        await Task.Delay(0);  //3000
                    }
                    else
                    {
                        var result = evolvePokemonOutProto.Result;
                        /*
                        ColoredConsoleWrite(ConsoleColor.White, $"Failed to evolve {pokemon.PokemonId}. " +
                                                 $"EvolvePokemonOutProto.Result was {result}");

                        ColoredConsoleWrite(ConsoleColor.White, $"Due to above error, stopping evolving {pokemon.PokemonId}");
                        */
                    }
                } while (evolvePokemonOutProto.Result == 1 && toEvolve.Contains(evolvePokemonOutProto.EvolvedPokemon.PokemonType));
                if (countOfEvolvedUnits > 0)
                    ColoredConsoleWrite(Color.Cyan,
                        $"Evolved {countOfEvolvedUnits} pieces of {pokemon.PokemonId} for {xpCount}xp");

            }
        }

        private void WriteConnectLogs(GetPlayerResponse profile)
        {
            ColoredConsoleWrite(Color.Yellow, "----------------------------");
            /*// dont actually want to display info but keeping here incase people want to \O_O/
             * if (ClientSettings.AuthType == AuthType.Ptc)
            {
                ColoredConsoleWrite(Color.Cyan, "Account: " + ClientSettings.PtcUsername);
                ColoredConsoleWrite(Color.Cyan, "Password: " + ClientSettings.PtcPassword + "\n");
            }
            else
            {
                ColoredConsoleWrite(Color.Cyan, "Email: " + ClientSettings.Email);
                ColoredConsoleWrite(Color.Cyan, "Password: " + ClientSettings.Password + "\n");
            }*/
            ColoredConsoleWrite(Color.DarkGray, "Name: " + profile.Profile.Username);
            ColoredConsoleWrite(Color.DarkGray, "Team: " + profile.Profile.Team);
            if (profile.Profile.Currency.ToArray()[0].Amount > 0) // If player has any pokecoins it will show how many they have.
                ColoredConsoleWrite(Color.DarkGray, "Pokecoins: " + profile.Profile.Currency.ToArray()[0].Amount);
            ColoredConsoleWrite(Color.DarkGray, "Stardust: " + profile.Profile.Currency.ToArray()[1].Amount + "\n");
            ColoredConsoleWrite(Color.DarkGray, "Latitude: " + ClientSettings.DefaultLatitude);
            ColoredConsoleWrite(Color.DarkGray, "Longitude: " + ClientSettings.DefaultLongitude);
            try
            {
                ColoredConsoleWrite(Color.DarkGray, "Country: " + CallAPI("country", ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude));
                ColoredConsoleWrite(Color.DarkGray, "Area: " + CallAPI("place", ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude));
            }
            catch (Exception)
            {
                ColoredConsoleWrite(Color.DarkGray, "Unable to get Country/Place");
            }

            ColoredConsoleWrite(Color.Yellow, "----------------------------");
        }

        private async Task UnbannedCheck()
        {
            while (ForceUnbanning)
                await Task.Delay(25);
            // await ForceUnban(client);
            if (!Stopping)
            {
                ColoredConsoleWrite(Color.Red, $"No nearby useful locations found. Please wait 10 seconds.");
                await Task.Delay(10000);
                CheckVersion();
                await Restart(client);
            }
            else
            {
                ConsoleClear();
                ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
                startStopBotToolStripMenuItem.Text = "Start";
                Stopping = false;
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

        private async void Execute()
        {
            client = new Client(ClientSettings);
            this.locationManager = new LocationManager(client, ClientSettings.TravelSpeed);

            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        ColoredConsoleWrite(Color.Green, "Login Type: Pokemon Trainers Club");
                        await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                        break;
                    case AuthType.Google:
                        ColoredConsoleWrite(Color.Green, "Login Type: Google");
                        await client.DoGoogleLogin(ClientSettings.Email, ClientSettings.Password);

                        break;
                }
                
                await client.SetServer();
                var profile = await client.GetProfile();
                var settings = await client.GetSettings();
                connected = true;


                ConsoleLevelTitle(profile.Profile.Username, client);

                // Write the players ingame details
                WriteConnectLogs(profile);

                // I believe a switch is more efficient and easier to read.

                client.stopRecycle = false;
                stopPrintLevel = false;

                //if (ClientSettings.Recycler)
                //    recycle = client.RecycleItems(client);
                printLevel = PrintLevel(client);
                await EvolveAndTransfert(client);
                await ExecuteFarmingPokestopsAndPokemons(client);
                await UnbannedCheck();
            }
            catch (TaskCanceledException) { ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting"); if (!Stopping) await Restart(client); }
            catch (UriFormatException) { ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting"); if (!Stopping) await Restart(client); }
            catch (ArgumentOutOfRangeException) { ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting"); if (!Stopping) await Restart(client); }
            catch (ArgumentNullException) { ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting"); if (!Stopping) await Restart(client); }
            catch (NullReferenceException) { ColoredConsoleWrite(Color.Red, "Null Refference - Restarting"); if (!Stopping) await Restart(client); }
            catch (Exception ex) { ColoredConsoleWrite(Color.Red, ex.ToString()); if (!Stopping) await Restart(client); }
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

     


        private async Task ExecuteCatchAllNearbyPokemons(Client clien, GetMapObjectsResponse mapObjects)
        {
            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons).ToList();
            if (!(nbPokemons < 230))
                await EvolveAndTransfert(clien);                
            foreach (var pokemon in pokemons)
            {
                if (ForceUnbanning || Stopping)
                    break;

                FarmingPokemons = true;

               // await locationManager.update(pokemon.Latitude, pokemon.Longitude);
                var encounterPokemonResponse = await client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonCP = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIV = Math.Round(Perfect(encounterPokemonResponse?.WildPokemon?.PokemonData));
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (ClientSettings.RazzBerryMode == "cp")
                        if (pokemonCP > ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    if (ClientSettings.RazzBerryMode == "probability")
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() < ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    caughtPokemonResponse = await client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, MiscEnums.Item.ITEM_POKE_BALL, pokemonCP); ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                string pokemonName;
                if (ClientSettings.Language == "german")
                {
                    //string name_english = Convert.ToString(pokemon.PokemonId);
                    //var request = (HttpWebRequest)WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + name_english);
                    //var response = (HttpWebResponse)request.GetResponse();
                    //pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    pokemonName = "no support german names for optimise time";
                }
                else
                    pokemonName = Convert.ToString(pokemon.PokemonId);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    ColoredConsoleWrite(Color.Green, $"We caught a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
                    foreach (int xp in caughtPokemonResponse.Scores.Xp)
                        TotalExperience += xp;
                    TotalPokemon += 1;
                    nbPokemons++;
                }
                else
                    ColoredConsoleWrite(Color.Red, $"{pokemonName} with {pokemonCP} CP and {pokemonIV}% IV got away..");
                FarmingPokemons = false;
                await Task.Delay(0);     //3000
            }                        
        }


        private void PrintPokestopTakeInfos(FortDetailsResponse fortInfo, FortSearchResponse fortSearch)
        {
            StringWriter PokeStopOutput = new StringWriter();
            if (fortSearch.ExperienceAwarded == 0)
            {
                ColoredConsoleWrite(Color.Cyan, "Wait for unlock catchs ...");
                return;
            }     
            PokeStopOutput.Write($"");
            if (fortInfo.Name != string.Empty)
                PokeStopOutput.Write("PokeStop: " + fortInfo.Name);
            if (fortSearch.ExperienceAwarded != 0)
                PokeStopOutput.Write($", XP: {fortSearch.ExperienceAwarded}");
            if (fortSearch.GemsAwarded != 0)
                PokeStopOutput.Write($", Gems: {fortSearch.GemsAwarded}");
            if (fortSearch.PokemonDataEgg != null)
                PokeStopOutput.Write($", Eggs: {fortSearch.PokemonDataEgg}");
            if (GetFriendlyItemsString(fortSearch.ItemsAwarded) != string.Empty)
                PokeStopOutput.Write($", Items: {GetFriendlyItemsString(fortSearch.ItemsAwarded)} ");
            ColoredConsoleWrite(Color.Cyan, PokeStopOutput.ToString());
            if (fortSearch.ExperienceAwarded != 0)
                TotalExperience += (fortSearch.ExperienceAwarded);

            PokeStopOutput.Dispose();
        }

        public double mpi180 = Math.PI / 180;

        public double distanceFrom(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // km
            var dLat = (lat2 - lat1) * mpi180;
            var dLon = (lon2 - lon1) * mpi180;
            lat1 = lat1 * mpi180;
            lat2 = lat2 * mpi180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }


        private async Task ExecuteFarmingPokestopsAndPokemons(Client client, IEnumerable<FortData> pokeStops = null)
        {
            double maxDist = 1;    //max dist in km

            double defLatitude = ClientSettings.DefaultLatitude;
            double defLongitude = ClientSettings.DefaultLongitude;
            bool catchPokemons = ClientSettings.CatchPokemon;


            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            if (pokeStops == null)
            {
                pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(
                    i => i.Type == FortType.Checkpoint && distanceFrom(i.Latitude, i.Longitude, defLatitude, defLongitude) < maxDist).ToList();
            }
           // pokeStops.ToList().ForEach(i => i.CooldownCompleteTimestampMs = 0);
            if (!ForceUnbanning && !Stopping)
                ColoredConsoleWrite(Color.Cyan, $"Visiting {pokeStops.Count()} PokeStops");

            do
            {
                
                var PossiblePokestops = pokeStops.
                    Where(i => i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime()).OrderBy(i => locationManager.getDistance(i.Latitude, i.Longitude)).ToList();
                //ColoredConsoleWrite(Color.Yellow, "Pokestop near " + pokeStops.Where(i => distanceFrom(i.Latitude, i.Longitude, ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude) < maxDist).Count());
                if (PossiblePokestops.Count() < 1)
                    PossiblePokestops = pokeStops.
                        Where(i => distanceFrom(i.Latitude, i.Longitude, defLatitude, defLongitude) < maxDist)
                        .OrderBy(i => i.CooldownCompleteTimestampMs).ToList();
                if (ForceUnbanning || Stopping)
                    break;
                var pokeStop = PossiblePokestops.First();

                FarmingStops = true;
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                int wait = 300;
                do
                {
                    await Task.Delay(wait);
                    wait += wait;
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0);
                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddMinutes(5).ToUnixTime();
                var pokeStopMapObjects = await client.GetMapObjects();


                ///* Gets all pokeStops near this pokeStop which are not in the set of pokeStops being currently
                // * traversed and which are ready to be farmed again.  */
                var pokeStopsNearPokeStop = pokeStopMapObjects.MapCells.SelectMany(i => i.Forts).Where(i =>
                    i.Type == FortType.Checkpoint).ToList();
                var ids = pokeStops.Select(p => p.Id).ToList();


                var items = pokeStopsNearPokeStop.Where(i => !ids.Contains(i.Id) && distanceFrom(i.Latitude, i.Longitude, defLatitude, defLongitude) < maxDist ).ToList();
                pokeStops = pokeStops.Concat(items);

            //    ColoredConsoleWrite(Color.Yellow, "Pokestop count " + pokeStops.Count());

                /* We choose the longest list of farmable PokeStops to traverse next, though we could use a different
                 * criterion, such as the number of PokeStops with lures in the list.*/
                //if (pokeStopsNearPokeStop.Count() > (nextPokeStopList == null ? 0 : nextPokeStopList.Count()))
                //{
                //    nextPokeStopList = pokeStopsNearPokeStop;
                //}
                if (catchPokemons)
                    await ExecuteCatchAllNearbyPokemons(client, pokeStopMapObjects);


            } while (true);
        }

        private async Task ForceUnban(Client client)
        {
            if (!ForceUnbanning && !Stopping)
            {
                ColoredConsoleWrite(Color.LightGreen, "Waiting for last farming action to be complete...");
                ForceUnbanning = true;

                while (FarmingStops || FarmingPokemons)
                {
                    await Task.Delay(25);
                }

                ColoredConsoleWrite(Color.LightGreen, "Starting force unban...");

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
                        ColoredConsoleWrite(Color.LightGreen, "Chosen PokeStop " + fortInfo.Name + " for force unban");
                        for (int i = 1; i <= 50; i++)
                        {
                            var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                            if (fortSearch.ExperienceAwarded == 0)
                            {
                                ColoredConsoleWrite(Color.LightGreen, "Attempt: " + i);
                            }
                            else
                            {
                                ColoredConsoleWrite(Color.LightGreen, "Fuck yes, you are now unbanned! Total attempts: " + i);
                                done = true;
                                break;
                            }
                        }
                    }

                    if (!done)
                        ColoredConsoleWrite(Color.LightGreen, "Force unban failed, please try again.");

                    ForceUnbanning = false;
                    break;
                }
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "A action is in play... Please wait.");
            }


        }

        private string GetFriendlyItemsString(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return enumerable.GroupBy(i => i.ItemId)
                    .Select(kvp => new { ItemName = kvp.Key.ToString().Substring(4), Amount = kvp.Sum(x => x.ItemCount) })
                    .Select(y => $"{y.Amount}x {y.ItemName}")
                    .Aggregate((a, b) => $"{a}, {b}");
        }


        private async Task TransfertAllButBestIV(Client client)
        {

            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();


            foreach (var unwantedPokemonType in toTransfert)
            {
                var unwantedPokemons = pokemons.Where(p => p.PokemonId == unwantedPokemonType)
                    .OrderByDescending(p => Perfect(p)).ThenBy(p => p.Cp).Skip(1).ToList();
                //ColoredConsoleWrite(ConsoleColor.White, $"Grinding {unwantedPokemon.Count} pokemons of type {unwantedPokemonType}");
                await TransferAllGivenPokemons(client, unwantedPokemons);
            }

            //ColoredConsoleWrite(ConsoleColor.White, $"Finished grinding all the meat");
        }


        private async Task TransfertAllBellowIV(Client client)
        {

            int iv = ClientSettings.TransferIVThreshold;
            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();


            foreach (var unwantedPokemonType in toTransfert)
            {
                var unwantedPokemons = pokemons.Where(p => p.PokemonId == unwantedPokemonType && Perfect(p) > iv)
                    .OrderByDescending(p => Perfect(p)).ThenBy(p => p.Cp).Skip(1).ToList();
                //ColoredConsoleWrite(ConsoleColor.White, $"Grinding {unwantedPokemon.Count} pokemons of type {unwantedPokemonType}");
                await TransferAllGivenPokemons(client, unwantedPokemons);
            }

            //ColoredConsoleWrite(ConsoleColor.White, $"Finished grinding all the meat");
        }



        private async Task TransferAllButStrongestUnwantedPokemon(Client client)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"Firing up the meat grinder");

            var unwantedPokemonTypes = new[]
            {
                PokemonId.Pidgey,
                PokemonId.Rattata,
                PokemonId.Weedle,
                PokemonId.Zubat,
                PokemonId.Caterpie,
                PokemonId.Pidgeotto,
                PokemonId.NidoranFemale,
                PokemonId.Paras,
                PokemonId.Venonat,
                PokemonId.Psyduck,
                PokemonId.Poliwag,
                PokemonId.Slowpoke,
                PokemonId.Drowzee,
                PokemonId.Gastly,
                PokemonId.Goldeen,
                PokemonId.Staryu,
                PokemonId.Magikarp,
                PokemonId.Clefairy,
                PokemonId.Eevee,
                PokemonId.Tentacool,
                PokemonId.Dratini,
                PokemonId.Ekans,
                PokemonId.Jynx,
                PokemonId.Lickitung,
                PokemonId.Spearow,
                PokemonId.NidoranFemale,
                PokemonId.NidoranMale
            };

            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            foreach (var unwantedPokemonType in unwantedPokemonTypes)
            {
                var pokemonOfDesiredType = pokemons.Where(p => p.PokemonId == unwantedPokemonType)
                    .OrderByDescending(p => p.Cp)
                    .ToList();

                var unwantedPokemon =
                    pokemonOfDesiredType.Skip(1);

                //ColoredConsoleWrite(ConsoleColor.White, $"Grinding {unwantedPokemon.Count} pokemons of type {unwantedPokemonType}");
                await TransferAllGivenPokemons(client, unwantedPokemon);
            }

            //ColoredConsoleWrite(ConsoleColor.White, $"Finished grinding all the meat");
        }

        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }

        private async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons, float keepPerfectPokemonLimit = 80.0f)
        {

            string language = ClientSettings.Language;
            foreach (var pokemon in unwantedPokemons)
            {
                //if (Perfect(pokemon) >= keepPerfectPokemonLimit) continue;
                

                if (pokemon.Favorite == 0)
                {
                    var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);

                    /*
                    ReleasePokemonOutProto.Status {
                        UNSET = 0;
                        SUCCESS = 1;
                        POKEMON_DEPLOYED = 2;
                        FAILED = 3;
                        ERROR_POKEMON_IS_EGG = 4;
                    }*/
                    string pokemonName;
                    if (language == "german")
                    {
                        // Dont really need to print this do we? youll know if its German or not
                        //ColoredConsoleWrite(Color.DarkCyan, "german");
                        string name_english = Convert.ToString(pokemon.PokemonId);
                        var request = (HttpWebRequest)WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + name_english);
                        var response = (HttpWebResponse)request.GetResponse();
                        pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    else
                        pokemonName = Convert.ToString(pokemon.PokemonId);
                    if (transferPokemonResponse.Status == 1)
                    {
                        ColoredConsoleWrite(Color.Green, $"Pokemon {pokemon.PokemonId} with {pokemon.Cp} CP and IV at {Perfect(pokemon)}% is transfered");
                        nbPokemons--;
                    }
                    else
                    {
                        var status = transferPokemonResponse.Status;

                        ColoredConsoleWrite(Color.Red, $"Somehow failed to transfer {pokemonName} with {pokemon.Cp} CP. " +
                                                 $"ReleasePokemonOutProto.Status was {status}");
                    }

                    await Task.Delay(0);  //3000
                }
            }
        }

        private async Task TransferDuplicatePokemon(Client client)
        {

            //ColoredConsoleWrite(ConsoleColor.White, $"Check for duplicates");
            var inventory = await client.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => x.Cp).Select((x, i) => new { index = i, value = x })
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;
                    if (dubpokemon.Favorite == 0)
                    {
                        var transfer = await client.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        //if (ClientSettings.Language == "german")
                        //{
                        //    string name_english = Convert.ToString(dubpokemon.PokemonId);
                        //    var request = (HttpWebRequest)WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + name_english);
                        //    var response = (HttpWebResponse)request.GetResponse();
                        //    pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        //}
                        //else
                            pokemonName = Convert.ToString(dubpokemon.PokemonId);
                        ColoredConsoleWrite(Color.DarkGreen,
                            $"Transferred {pokemonName} with {dubpokemon.Cp} CP (Highest is {dupes.ElementAt(i).Last().value.Cp})");

                    }
                }
            }
        }



        private async Task TransferAllWeakPokemon(Client client, int cpThreshold)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"Firing up the meat grinder");

            PokemonId[] doNotTransfer = new[] //these will not be transferred even when below the CP threshold
            { // DO NOT EMPTY THIS ARRAY
                //PokemonId.Pidgey,
                //PokemonId.Rattata,
                //PokemonId.Weedle,
                //PokemonId.Zubat,
                //PokemonId.Caterpie,
                //PokemonId.Pidgeotto,
                //PokemonId.NidoranFemale,
                //PokemonId.Paras,
                //PokemonId.Venonat,
                //PokemonId.Psyduck,
                //PokemonId.Poliwag,
                //PokemonId.Slowpoke,
                //PokemonId.Drowzee,
                //PokemonId.Gastly,
                //PokemonId.Goldeen,
                //PokemonId.Staryu,
                PokemonId.Magikarp,
                PokemonId.Eevee//,
                //PokemonId.Dratini
            };

            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                                .Select(i => i.InventoryItemData?.Pokemon)
                                .Where(p => p != null && p?.PokemonId > 0)
                                .ToArray();

            //foreach (var unwantedPokemonType in unwantedPokemonTypes)
            {
                List<PokemonData> pokemonToDiscard;
                if (doNotTransfer.Count() != 0)
                    pokemonToDiscard = pokemons.Where(p => !doNotTransfer.Contains(p.PokemonId) && p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();
                else
                    pokemonToDiscard = pokemons.Where(p => p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();


                //var unwantedPokemon = pokemonOfDesiredType.Skip(1) // keep the strongest one for potential battle-evolving
                //                                          .ToList();
                ColoredConsoleWrite(Color.Gray, $"Grinding {pokemonToDiscard.Count} pokemon below {cpThreshold} CP.");
                await TransferAllGivenPokemons(client, pokemonToDiscard);

            }

            ColoredConsoleWrite(Color.Gray, $"Finished grinding all the meat");
        }

        public async Task PrintLevel(Client client)
        {
            var inventory = await client.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            foreach (var v in stats)
                if (v != null)
                {
                    int XpDiff = GetXpDiff(client, v.Level);
                    if (ClientSettings.LevelOutput == "time")
                        ColoredConsoleWrite(Color.Yellow, $"Current Level: " + v.Level + " (" + (v.Experience - XpDiff) + "/" + (v.NextLevelXp - XpDiff) + ")");
                    else if (ClientSettings.LevelOutput == "levelup")
                        if (Currentlevel != v.Level)
                        {
                            Currentlevel = v.Level;
                            ColoredConsoleWrite(Color.Magenta, $"Current Level: " + v.Level + ". XP needed for next Level: " + (v.NextLevelXp - v.Experience));
                        }
                }
            if (ClientSettings.LevelOutput == "levelup")
                await Task.Delay(1000);
            else
                await Task.Delay(ClientSettings.LevelTimeInterval * 1000);
            if (!stopPrintLevel)
             PrintLevel(client);
        }

        // Pulled from NecronomiconCoding
        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }

        public async Task ConsoleLevelTitle(string Username, Client client)
        {
            var inventory = await client.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            var profile = await client.GetProfile();
            foreach (var v in stats)
                if (v != null)
                {
                    int XpDiff = GetXpDiff(client, v.Level);
                    SetStatusText(string.Format(Username + " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}", v.Level, _getSessionRuntimeInTimeFormat(), (v.Experience - v.PrevLevelXp - XpDiff), (v.NextLevelXp - v.PrevLevelXp - XpDiff), profile.Profile.Currency.ToArray()[1].Amount) + " | XP/Hour: " + Math.Round(TotalExperience / GetRuntime()) + " | Pokemon/Hour: " + Math.Round(TotalPokemon / GetRuntime()));
                }
            await Task.Delay(1000);
            ConsoleLevelTitle(Username, client);
        }

        public static int GetXpDiff(Client client, int Level)
        {
            switch (Level)
            {
                case 1:
                    return 0;
                case 2:
                    return 1000;
                case 3:
                    return 2000;
                case 4:
                    return 3000;
                case 5:
                    return 4000;
                case 6:
                    return 5000;
                case 7:
                    return 6000;
                case 8:
                    return 7000;
                case 9:
                    return 8000;
                case 10:
                    return 9000;
                case 11:
                    return 10000;
                case 12:
                    return 10000;
                case 13:
                    return 10000;
                case 14:
                    return 10000;
                case 15:
                    return 15000;
                case 16:
                    return 20000;
                case 17:
                    return 20000;
                case 18:
                    return 20000;
                case 19:
                    return 25000;
                case 20:
                    return 25000;
                case 21:
                    return 50000;
                case 22:
                    return 75000;
                case 23:
                    return 100000;
                case 24:
                    return 125000;
                case 25:
                    return 150000;
                case 26:
                    return 190000;
                case 27:
                    return 200000;
                case 28:
                    return 250000;
                case 29:
                    return 300000;
                case 30:
                    return 350000;
                case 31:
                    return 500000;
                case 32:
                    return 500000;
                case 33:
                    return 750000;
                case 34:
                    return 1000000;
                case 35:
                    return 1250000;
                case 36:
                    return 1500000;
                case 37:
                    return 2000000;
                case 38:
                    return 2500000;
                case 39:
                    return 1000000;
                case 40:
                    return 1000000;
            }
            return 0;
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
                        //ColoredConsoleWrite(ConsoleColor.White, "Coded by Ferox - edited by NecronomiconCoding");
                        CheckVersion();
                        Execute();
                    }
                    catch (PtcOfflineException)
                    {
                        ColoredConsoleWrite(Color.Red, "PTC Servers are probably down OR your credentials are wrong. Try google");
                    }
                    catch (Exception ex)
                    {
                        ColoredConsoleWrite(Color.Red, $"Unhandled exception: {ex}");
                    }
                });
            }
            else
            {
                if (!ForceUnbanning)
                {
                    Stopping = true;
                    ColoredConsoleWrite(Color.Red, $"Stopping the bot.. Waiting for the last action to be complete.");
                }
                else
                {
                    ColoredConsoleWrite(Color.Red, $"An action is in play, please wait until it's done.");
                }
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
                        ColoredConsoleWrite(Color.Green, $"Using a Lucky Egg, we have {LuckyEgg.Count} left.");
                        ColoredConsoleWrite(Color.Yellow, $"Lucky Egg Valid until: {DateTime.Now.AddMinutes(30).ToString()}");

                        var stripItem = sender as ToolStripMenuItem;
                        stripItem.Enabled = false;
                        await Task.Delay(30000);
                        stripItem.Enabled = true;
                    }
                    else
                    {
                        ColoredConsoleWrite(Color.Red, $"You don't have any Lucky Egg to use.");
                    }
                }
                catch (Exception ex)
                {
                    ColoredConsoleWrite(Color.Red, $"Unhandled exception in using lucky egg: {ex}");
                }
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "Please start the bot before trying to use a lucky egg.");
            }
        }

        private async void forceUnbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                if (ForceUnbanning)
                {
                    ColoredConsoleWrite(Color.Red, "A force unban attempt is in action... Please wait.");
                }
                else
                {
                    await ForceUnban(client);
                }
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "Please start the bot before trying to force unban");
            }
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

        #region Transfert And Evolve Setting
        public static PokemonId[] toTransfert = new[]
        {
        PokemonId.Bulbasaur,
        PokemonId.Ivysaur,
        PokemonId.Charmander,
        PokemonId.Charmeleon,
        PokemonId.Squirtle,
        PokemonId.Wartortle,
        PokemonId.Caterpie,
        PokemonId.Metapod,
        PokemonId.Butterfree,
        PokemonId.Weedle,
        PokemonId.Kakuna,
        PokemonId.Beedrill,
        PokemonId.Pidgey,
        PokemonId.Pidgeotto,
        PokemonId.Pidgeot,
        PokemonId.Rattata,
        PokemonId.Raticate,
        PokemonId.Spearow,
        PokemonId.Fearow,
        PokemonId.Ekans,
        PokemonId.Arbok,
        PokemonId.Pikachu,
        PokemonId.Raichu,
        PokemonId.Sandshrew,
        PokemonId.Sandlash,
        PokemonId.NidoranFemale,
        PokemonId.Nidorina,
        PokemonId.Nidoqueen,
        PokemonId.NidoranMale,
        PokemonId.Nidorino,
        PokemonId.Nidoking,
        PokemonId.Clefairy,
        PokemonId.Clefable,
        PokemonId.Vulpix,
        PokemonId.Ninetales,
        PokemonId.Jigglypuff,
        PokemonId.Wigglytuff,
        PokemonId.Zubat,
        PokemonId.Golbat,
        PokemonId.Oddish,
        PokemonId.Gloom,
        PokemonId.Vileplume,
        PokemonId.Paras,
        PokemonId.Parasect,
        PokemonId.Venonat,
        PokemonId.Venomoth,
        PokemonId.Diglett,
        PokemonId.Dugtrio,
        PokemonId.Meowth,
        PokemonId.Persian,
        PokemonId.Psyduck,
        PokemonId.Golduck,
        PokemonId.Mankey,
        PokemonId.Primeape,
        PokemonId.Growlithe,
        PokemonId.Poliwag,
        PokemonId.Poliwhirl,
        PokemonId.Poliwrath,
        PokemonId.Abra,
        PokemonId.Kadabra,
        PokemonId.Alakhazam,
        PokemonId.Machop,
        PokemonId.Machoke,
        PokemonId.Machamp,
        PokemonId.Bellsprout,
        PokemonId.Weepinbell,
        PokemonId.Victreebell,
        PokemonId.Tentacool,
        PokemonId.Tentacruel,
        PokemonId.Geodude,
        PokemonId.Graveler,
        PokemonId.Golem,
        PokemonId.Ponyta,
        PokemonId.Rapidash,
        PokemonId.Slowpoke,
        PokemonId.Slowbro,
        PokemonId.Magnemite,
        PokemonId.Magneton,
        PokemonId.Farfetchd,
        PokemonId.Doduo,
        PokemonId.Dodrio,
        PokemonId.Seel,
        PokemonId.Dewgong,
        PokemonId.Grimer,
        PokemonId.Muk,
        PokemonId.Shellder,
        PokemonId.Cloyster,
        PokemonId.Gastly,
        PokemonId.Haunter,
        PokemonId.Gengar,
        PokemonId.Onix,
        PokemonId.Drowzee,
        PokemonId.Hypno,
        PokemonId.Krabby,
        PokemonId.Kingler,
        PokemonId.Voltorb,
        PokemonId.Electrode,
        PokemonId.Exeggcute,
        PokemonId.Cubone,
        PokemonId.Marowak,
        PokemonId.Hitmonlee,
        PokemonId.Hitmonchan,
        PokemonId.Lickitung,
        PokemonId.Koffing,
        PokemonId.Weezing,
        PokemonId.Rhyhorn,
        PokemonId.Rhydon,
        PokemonId.Chansey,
        PokemonId.Tangela,
        PokemonId.Kangaskhan,
        PokemonId.Horsea,
        PokemonId.Seadra,
        PokemonId.Goldeen,
        PokemonId.Seaking,
        PokemonId.Staryu,
        PokemonId.Starmie,
        PokemonId.MrMime,
        PokemonId.Scyther,
        PokemonId.Jynx,
        PokemonId.Electabuzz,
        PokemonId.Magmar,
        PokemonId.Pinsir,
        PokemonId.Tauros,
        PokemonId.Magikarp,
        PokemonId.Ditto,
        PokemonId.Porygon,
        PokemonId.Omanyte,
        PokemonId.Omastar,
        PokemonId.Kabuto,
        PokemonId.Kabutops,
        PokemonId.Aerodactyl,
        PokemonId.Eevee,
        PokemonId.Dratini
        };
        public static PokemonId[] toEvolve = new[]
        {
         PokemonId.Caterpie,
        PokemonId.Weedle,
        PokemonId.Pidgey,
        PokemonId.Rattata,
        PokemonId.Spearow,
        PokemonId.Ekans,
        PokemonId.Pikachu,
        PokemonId.Sandshrew,
        PokemonId.Clefairy,
        PokemonId.Vulpix,
        PokemonId.Jigglypuff,
        PokemonId.Zubat,
        PokemonId.Oddish,
        PokemonId.Paras,
        PokemonId.Venonat,
        PokemonId.Diglett,
        PokemonId.Meowth,
        PokemonId.Psyduck,
        PokemonId.Mankey,
        PokemonId.Poliwag,
        PokemonId.Abra,
        PokemonId.Machop,
        PokemonId.Bellsprout,
        PokemonId.Tentacool,
        PokemonId.Geodude,
        PokemonId.Ponyta,
        PokemonId.Slowpoke,
        PokemonId.Magnemite,
        PokemonId.Farfetchd,
        PokemonId.Doduo,
        PokemonId.Seel,
        PokemonId.Grimer,
        PokemonId.Shellder,
        PokemonId.Cloyster,
        PokemonId.Gastly,
        PokemonId.Drowzee,
        PokemonId.Krabby,
        PokemonId.Voltorb,
        PokemonId.Cubone,
        PokemonId.Hitmonlee,
        PokemonId.Hitmonchan,
        PokemonId.Lickitung,
        PokemonId.Koffing,
        PokemonId.Rhyhorn,
        PokemonId.Horsea,
        PokemonId.Goldeen,
        PokemonId.Staryu,
        PokemonId.Ditto,
        PokemonId.Omanyte,
        PokemonId.Kabuto,
       };
        #endregion
    }
}
