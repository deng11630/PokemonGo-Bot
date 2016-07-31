using MoreLinq;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PokemonGo.RocketAPI.GeneratedCode.CatchPokemonResponse.Types;
using static PokemonGo.RocketAPI.GeneratedCode.EncounterResponse.Types;

namespace PokemonGo.RocketAPI.Window
{
    public static class PokemonActions
    {
        public static int nbCatchs = 0;
        public static bool farmingPokemons = false;
        public static  Client client;
        public static bool OntransfertEvolve = false;
        public static List<WildPokemon> wildPokemons;




        public static async Task<bool> EvolvePokemon(PokemonData pokemon)
        {
            EvolvePokemonOut res = await client.EvolvePokemon(pokemon.Id);
            if (res.Result == 1)
            {
                ConsoleWriter.Evolved(pokemon, res);
                MainForm.totalExperience += (res.ExpAwarded);
                return TransfertAndEvolveSetting.toEvolve.Contains(res.EvolvedPokemon.PokemonType);
            }
            return false;
        }





        public static async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve, GetInventoryResponse inventory)
        {
            var families = inventory.InventoryDelta.InventoryItems
                   .Select(i => i.InventoryItemData?.PokemonFamily)
                   .Where(p => p != null && (int)p?.FamilyId > 0)
                   .OrderByDescending(p => (int)p.FamilyId).ToList();
            foreach (var pokemon in pokemonToEvolve)
            {
                var candy = families.Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId).First();
                if (candy.Candy < 50) continue;
                candy.Candy -= 50;
                bool continueEvolve;
                do
                {
                    continueEvolve = await EvolvePokemon(pokemon);
                } while (continueEvolve);
             }
        }

        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }



        public static async Task<List<PokemonData>> UpdatePokemons()
        {
            await Inventory.UpdateInventory(client);
            return Inventory.pokemons;                
        }

        public static List<PokemonData> GetPokemonsToEvolve(List<PokemonData> pokemons)
        {
            return pokemons.Where(p => TransfertAndEvolveSetting.toEvolve.Contains(p.PokemonId)).OrderByDescending(p => Perfect(p)).ToList();
        }

        public static List<PokemonData> GetAllButStrongestPokemon(List<PokemonData> pokemons)
        {
            List<PokemonData> result = new List<PokemonData>();
            foreach (var unwantedPokemonType in TransfertAndEvolveSetting.toTransfert)
            {
                var partOfRes = pokemons.Where(p => p.PokemonId == unwantedPokemonType)
                    .OrderByDescending(p => p.Cp).Skip(1).ToList();
                result.AddRange(partOfRes);
            }
            return result;
        }

        public static List<PokemonData> GetAllButHighestIVPokemon(List<PokemonData> pokemons)
        {
            List<PokemonData> result = new List<PokemonData>();
            foreach (var unwantedPokemonType in TransfertAndEvolveSetting.toTransfert)
            {
                var partOfRes = pokemons.Where(p => p.PokemonId == unwantedPokemonType)
                    .OrderByDescending(p => Perfect(p)).Skip(1).ToList();
                result.AddRange(partOfRes);
            }
            return result;
        }

        public static List<PokemonData> GetAllUnderCPLimitsPokemon(List<PokemonData> pokemons)
        {
            return pokemons.Where(p => p.Cp < ReadSettings.transferCPThreshold).ToList();
        }

        public static List<PokemonData> GetAllUnderIVLimitsPokemon(List<PokemonData> pokemons)
        {
            return pokemons.Where(p => Perfect(p) < ReadSettings.transferIVThreshold).ToList();
        }

        private static async Task TransfertPokemon(PokemonData pokemon)
        {
            if (pokemon.Favorite == 0)
            {
                var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);
                ConsoleWriter.TransferedPokemon(pokemon, transferPokemonResponse);
                if (transferPokemonResponse.Status == 1)
                    Inventory.nbPokemons--;
                await Task.Delay(ReadSettings.transfertWait);
            }
        }


        private static async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons)
        {
            foreach (var pokemon in unwantedPokemons)
                await TransfertPokemon(pokemon);
        }


        public static async Task EvolveAndTransfert(Client client, GetInventoryResponse inventory)
        {
            var pokemons = await UpdatePokemons();
            if (OntransfertEvolve)
                return;
            if (Inventory.nbPokemons > ReadSettings.maxPokemonsOnInventory)
            {
                OntransfertEvolve = true;
                if (ReadSettings.evolveAllGivenPokemons)
                {
                    await EvolveAllGivenPokemons(client, GetPokemonsToEvolve(pokemons), inventory);
                    pokemons = UpdatePokemons().Result;
                }
                switch (ReadSettings.transferType)
                {
                    case "Leave Strongest":
                        await TransferAllGivenPokemons(client, GetAllButStrongestPokemon(pokemons));
                        break;
                    case "All":
                        await TransferAllGivenPokemons(client, pokemons);
                        break;
                    case "Duplicate":
                        await TransferAllGivenPokemons(client, GetAllButStrongestPokemon(pokemons));
                        break;
                    case "IV Duplicate":
                        await TransferAllGivenPokemons(client, GetAllButHighestIVPokemon(pokemons));
                        break;
                    case "CP":
                        await TransferAllGivenPokemons(client, GetAllUnderCPLimitsPokemon(pokemons));
                        break;
                    case "IV":
                        await TransferAllGivenPokemons(client, GetAllUnderIVLimitsPokemon(pokemons));
                        break;
                    default:
                        break;
                }
                OntransfertEvolve = false;
            }
        }


        private static async Task<CatchPokemonResponse> CatchPokemon(EncounterResponse encounterPokemonResponse, MapPokemon pokemon, int? pokemonCP)
        {
            if (Inventory.razzBerry > 0)
            {

                if (ReadSettings.razzBerryMode == "cp")
                    if (pokemonCP > ReadSettings.razzBerrySetting)
                    {
                        await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                        Inventory.razzBerry--;
                    }
                else if (ReadSettings.razzBerryMode == "probability")
                    if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() < ReadSettings.razzBerrySetting)
                    {
                        await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                        Inventory.razzBerry--;
                    }
            }
            return await client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, (int)Inventory.GetPokeballToUse(pokemonCP)); ; //note: reverted from settings because this should not be part of settings but part of logic

        }

        private static bool IsCaughtOrEscape(CatchPokemonResponse caughtRes)
        {
            return !(caughtRes.Status == CatchStatus.CatchMissed || caughtRes.Status == CatchStatus.CatchEscape);
        }

        public static async Task ExecuteCatchAllNearbyPokemons(GetMapObjectsResponse mapObjects)
        {
            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons).ToList();
            farmingPokemons = true;
            foreach (var pokemon in pokemons)
            {
                if (MainForm.unbanning || MainForm.stop)
                    break;
                var encounterPokemonResponse = await client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonCP = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                if (encounterPokemonResponse.Status != Status.EncounterSuccess)
                {
                    ConsoleWriter.ColoredConsoleWrite(Color.Red, "Error " + Enum.GetName(typeof(Status), encounterPokemonResponse.Status));
                    return;
                }
                var pokemonIV = Perfect(encounterPokemonResponse?.WildPokemon?.PokemonData);
                CatchPokemonResponse caughtRes;
                do
                {
                    caughtRes = await CatchPokemon(encounterPokemonResponse, pokemon, pokemonCP);
                } while (!IsCaughtOrEscape(caughtRes));
                if (caughtRes.Status == CatchStatus.CatchSuccess)
                {
                    foreach (int xp in caughtRes.Scores.Xp)
                        MainForm.totalExperience += xp;
                    Inventory.nbPokemons++;
                    nbCatchs++;
                }
                ConsoleWriter.CaughtPokemon(caughtRes, pokemon, pokemonCP, (int)pokemonIV);
            }
            farmingPokemons = false;
        }

    }
}
