using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public static class ConsoleWriter
    {
        public static bool stopPrintLevel = false;

        public static void PrintVersion()
        {
            try
            {
                string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                version = version.Substring(0, version.Length - 2);
                ColoredConsoleWrite(Color.SpringGreen, "Fastest pokemon go bot.");
                ColoredConsoleWrite(Color.Green, "Your version is " + version);
                ColoredConsoleWrite(Color.Green, "You can find it at www.github.com/mfron/PokemonGo-Bot");
            }
            catch (Exception)
            {
                ColoredConsoleWrite(Color.Red, "Unable to check for updates now...");
            }
        }



        public static void Evolved(PokemonData pokemon, EvolvePokemonOut res)
        {
            ColoredConsoleWrite(Color.Cyan,
    $"Evolved {pokemon.PokemonId} successfully for {res.ExpAwarded} xp {pokemon.Cp} cp and IV {PokemonActions.Perfect(pokemon)}%");

        }

        public static void TransferedPokemon(PokemonData pokemon, TransferPokemonOut transferPokemonResponse)
        {
            string pokemonName;
            if (ReadSettings.language == "german")
                pokemonName = LanguageSetting.GermanName[(int)pokemon.PokemonId];
            else if (ReadSettings.language == "french")
                pokemonName = LanguageSetting.frenchPokemons[(int)pokemon.PokemonId];
            else
                pokemonName = Convert.ToString(pokemon.PokemonId);
            if (transferPokemonResponse.Status == 1)
                ColoredConsoleWrite(Color.Green, $"Pokemon {pokemon.PokemonId} with {pokemon.Cp} CP and IV at {PokemonActions.Perfect(pokemon)}% is transfered");
            else
                ColoredConsoleWrite(Color.Red, $"Somehow failed to transfer {pokemonName} with {pokemon.Cp} CP. " +
                                         $"ReleasePokemonOutProto.Status was  {transferPokemonResponse.Status}");
        }

        public static void CaughtPokemon(CatchPokemonResponse caughtRes, MapPokemon pokemon, int? pokemonCP, int pokemonIV)
        {
            string pokemonName;
            if (ReadSettings.language == "german")
                pokemonName = LanguageSetting.GermanName[(int)pokemon.PokemonId];
            else if (ReadSettings.language == "french")
                pokemonName = LanguageSetting.frenchPokemons[(int)pokemon.PokemonId];
            else
                pokemonName = Convert.ToString(pokemon.PokemonId);

            if (caughtRes.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                ColoredConsoleWrite(Color.Green, $"We caught a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
            else
                ColoredConsoleWrite(Color.Red, $"{pokemonName} with {pokemonCP} CP and {pokemonIV}% IV got away..");
        }

        public static void WriteProfile(GetPlayerResponse profile)
        {
            ColoredConsoleWrite(Color.Yellow, "----------------------------");
            ColoredConsoleWrite(Color.DarkGray, "Name: " + profile.Profile.Username);
            ColoredConsoleWrite(Color.DarkGray, "Team: " + profile.Profile.Team);
            if (profile.Profile.Currency.ToArray()[0].Amount > 0) // If player has any pokecoins it will show how many they have.
                ColoredConsoleWrite(Color.DarkGray, "Pokecoins: " + profile.Profile.Currency.ToArray()[0].Amount);
            ColoredConsoleWrite(Color.DarkGray, "Stardust: " + profile.Profile.Currency.ToArray()[1].Amount + "\n");
            ColoredConsoleWrite(Color.DarkGray, "Latitude: " + ReadSettings.defaultLatitude);
            ColoredConsoleWrite(Color.DarkGray, "Longitude: " + ReadSettings.defaultLongitude);
            ColoredConsoleWrite(Color.Yellow, "----------------------------");
            ColoredConsoleWrite(Color.White, "----------------------------");
            ColoredConsoleWrite(Color.Fuchsia, "Best Donator : Zayceur With 2$");
            ColoredConsoleWrite(Color.Fuchsia, "We take time for your bot, you can donate someting you too");
            ColoredConsoleWrite(Color.Fuchsia, "You can donate here : https://www.paypal.me/mfron");
            ColoredConsoleWrite(Color.White, "----------------------------");
        }


        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (MainForm.instance.InvokeRequired)
            {
                MainForm.instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }
            MainForm.instance.logTextBox.Select(MainForm.instance.logTextBox.Text.Length, 1); // Reset cursor to last
            string textToAppend = "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\r\n";
            MainForm.instance.logTextBox.SelectionColor = color;
            MainForm.instance.logTextBox.AppendText(textToAppend);
        }


        public static void ColoredConsoleAppendText(Color color, string text)
        {
            if (MainForm.instance.InvokeRequired)
            {
                MainForm.instance.Invoke(new Action<Color, string>(ColoredConsoleAppendText), color, text);
                return;
            }
            MainForm.instance.logTextBox.Select(MainForm.instance.logTextBox.Text.Length, 1); // Reset cursor to last
            MainForm.instance.logTextBox.SelectionColor = color;
            MainForm.instance.logTextBox.AppendText(text);
        }

        public static void PrintPokestopTakeInfos(FortDetailsResponse fortInfo, FortSearchResponse fortSearch)
        {
            StringWriter PokeStopOutput = new StringWriter();
            if (fortSearch.ExperienceAwarded == 0)
            {
                Movements.waitUnlock = true;                             
                return;
            }
            Movements.waitUnlock = false;
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
            MainForm.totalExperience += (fortSearch.ExperienceAwarded);
            PokeStopOutput.Dispose();
          
        }

        public static void StartPokestop(double traveledDistance)
        {
            ColoredConsoleWrite(Color.LightYellow, "Travaled distance : " + (int)(traveledDistance * 1000) + " meters | " + ReadSettings.travelSpeed + "Km/H | Wait : " +  ((ReadSettings.timePerKmMs * traveledDistance) / 1000).ToString("N1") );
        }


        private static string GetFriendlyItemsString(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return enumerable.GroupBy(i => i.ItemId)
                    .Select(kvp => new { ItemName = kvp.Key.ToString().Substring(4), Amount = kvp.Sum(x => x.ItemCount) })
                    .Select(y => $"{y.Amount}x {y.ItemName}")
                    .Aggregate((a, b) => $"{a}, {b}");
        }

        public static async Task PrintLevel(Client client, GetInventoryResponse inventory)
        {
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            foreach (var v in stats)
                if (v != null)
                {
                    int XpDiff = xpDiff[v.Level];
                    if (ReadSettings.levelOutput == "time")
                        ColoredConsoleWrite(Color.Yellow, $"Current Level: " + v.Level + " (" + (v.Experience - XpDiff) + "/" + (v.NextLevelXp - XpDiff) + ")");
                    else if (ReadSettings.levelOutput == "levelup")
                        if (MainForm.Currentlevel != v.Level)
                        {
                            MainForm.Currentlevel = v.Level;
                            ColoredConsoleWrite(Color.Magenta, $"Current Level: " + v.Level + ". XP needed for next Level: " + (v.NextLevelXp - v.Experience));
                        }
                }
        }


        public static bool PokestopFarmStart(List<FortData> pokestops)
        {
            if (pokestops.Count < 1)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"ERROR ->  Cannot farm without at least 1 pokestop near you.");
                return false;
            }
            else if (pokestops.Count < 30)
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Not a great place for farm, only {pokestops.Count} pokestops near you");
            else if (pokestops.Count < 100)
                ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"{pokestops.Count} pokestops near you");
            else if (pokestops.Count < 150)
                ConsoleWriter.ColoredConsoleWrite(Color.Yellow, $"Great place, {pokestops.Count} pokestops near you");
            else
                ConsoleWriter.ColoredConsoleWrite(Color.Pink, $"Wouw ! Excelente place, {pokestops.Count} pokestops near you");
            return true;  
        }



        public static async Task ConsoleLevelTitle(string Username, Client client, GetInventoryResponse inventory)
        {
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            var profile = await client.GetProfile();
            foreach (var v in stats)
                if (v != null)
                {

                        int XpDiff = xpDiff[v.Level];
                        //Calculating the exp needed to level up
                        Single expNextLvl = (v.NextLevelXp - v.Experience);
                        //Calculating the exp made per second
                        var xpSec = (Math.Round(MainForm.totalExperience / MainForm.GetRuntime()) / 60) / 60;
                        //Calculating the seconds left to level up
                        int secondsLeft = 0;
                        if (xpSec != 0)
                            secondsLeft = Convert.ToInt32((expNextLvl / xpSec));
                        //formatting data to make an output like DateFormat
                        var ts = TimeSpan.FromSeconds(secondsLeft);       
                    string waitUnlock = (Movements.waitUnlock) ? " | Wait unlock catching " : "";
                    MainForm.instance.SetStatusText(string.Format(
                        profile.Profile.Username + " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}",
                        v.Level, _getSessionRuntimeInTimeFormat(),
                        (v.Experience - v.PrevLevelXp - XpDiff), (v.NextLevelXp - v.PrevLevelXp - XpDiff), profile.Profile.Currency.ToArray()[1].Amount) +
                        " | XP/Hour: " + Math.Round(MainForm.totalExperience / MainForm.GetRuntime()) + " | Pokemon/Hour: " + Math.Round(PokemonActions.nbCatchs / MainForm.GetRuntime()) +
                        " | Next level in: " + ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds + waitUnlock);

                }
        }


        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - MainForm.InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }




        public static int[] xpDiff = new int[]
       {
            0,
            0,
            1000,
            2000,
            3000,
            4000,
            5000,
            6000,
            7000,
            8000,
            9000,
            10000,
            10000,
            10000,
            10000,
            15000,
            20000,
            20000,
            20000,
            25000,
            25000,
            50000,
            75000,
            100000,
            125000,
            150000,
            190000,
            200000,
            250000,
            300000,
            350000,
            500000,
            500000,
            750000,
            1000000,
            1250000,
            1500000,
            2000000,
            2500000,
            1000000,
            1000000
       };
    }
}
