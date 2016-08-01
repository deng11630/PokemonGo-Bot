using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class ReadSettings
    {
        public static bool evolveAllGivenPokemons;
        public static string transferType;
        public static int maxPokemonsOnInventory;
        public static string language;
        public static int transferCPThreshold;
        public static int transferIVThreshold;
        public static string razzBerryMode;
        public static double razzBerrySetting;
        public static double defaultLatitude;
        public static double defaultLongitude;
        public static bool catchPokemon;
        public static double deplacementsMaxDist;
        public static string levelOutput;
        public static int levelTimeInterval;
        public static AuthType authType;
        public static uint timePerKmMs;
        public static int travelSpeed;
        public static int waitForUnlock;
        public static bool instantMoove;
        public static int transfertWait;
        public static int imageSize;
        public static string poke;

        public static string pokeballMode;
        public static int cpToUseSuperBall;
        public static int cpToUseHyperBall;
        public static int IVToUseSuperBall;
        public static int IVToUseHyperBall;
        public static AllEnum.TravelMode travelMode;
        public static Func<Client, Task> TravelAction;
        public static int maxPokestop;


        public static void Load(Settings settings)
        {
            transfertWait = 300;
            instantMoove = true;
            waitForUnlock = 300;
            travelSpeed = settings.TravelSpeed;
            timePerKmMs = (60 * 60 * 1000) / (uint)settings.TravelSpeed;
            authType = settings.AuthType;
            levelTimeInterval = settings.LevelTimeInterval;
            levelOutput = settings.LevelOutput;
            deplacementsMaxDist = 1;//km
            catchPokemon = settings.CatchPokemon;
            defaultLatitude = settings.DefaultLatitude;
            defaultLongitude = settings.DefaultLongitude;
            razzBerrySetting = settings.RazzBerrySetting;
            razzBerryMode = settings.RazzBerryMode;
            transferIVThreshold = settings.TransferIVThreshold;
            transferCPThreshold = settings.TransferCPThreshold;
            evolveAllGivenPokemons = settings.EvolveAllGivenPokemons;
            language = settings.Language;
            transferType = settings.TransferType;
            maxPokemonsOnInventory = 220;
            imageSize = settings.ImageSize;
            poke = settings.Poke;
            pokeballMode = settings.pokeballMode;
            cpToUseSuperBall = settings.cpToUseSuperBall;
            cpToUseHyperBall = settings.cpToUseHyperBall;
            IVToUseSuperBall = settings.IVToUseSuperBall;
            IVToUseHyperBall = settings.IVToUseHyperBall;

            switch (travelMode){
                case AllEnum.TravelMode.FarmAllInRange :
                    TravelAction = Movements.FarmAllInRange;
                    break;
                case AllEnum.TravelMode.FarmAllWithoutSaving:
                    TravelAction = Movements.FarmAllWithoutSaving;
                    break;
                case AllEnum.TravelMode.GoWhereYouWantButFarm:
                    TravelAction = Movements.GoWhereYouWantButFarm;
                    break;
                case AllEnum.TravelMode.FarmXPokestop:
                    TravelAction = Movements.FarmXPokestops;   
                    break;
                default :
                    TravelAction = Movements.FarmAllWithSaving;
                    break;
            }
        }

    }
}
