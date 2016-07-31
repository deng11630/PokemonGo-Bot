using AllEnum;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{

    public static class Inventory
    {
        public static List<PokemonData> pokemons;
        public static GetInventoryResponse inventory;
        public static List<Item> items;
        public static int razzBerry = 0;
        public static int nbPokemons = 0;
        public static int pokeBallsCount = 0;
        public static int greatBallsCount = 0;
        public static int ultraBallsCount = 0;
        public static int masterBallsCount = 0;

        public static async Task UpdateInventory(Client client)
        {
            inventory = await client.GetInventory();
            pokemons = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p != null && p?.PokemonId > 0).ToList();
            nbPokemons = pokemons.Count;
            items = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item).Where(p => p != null).ToList();
            razzBerry = items.Where(i => (ItemId)i.Item_ == ItemId.ItemRazzBerry).Count();


            var pokeballsRequest = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null)
                .GroupBy(i => (MiscEnums.Item)i.Item_)
                .Select(kvp => new { ItemId = kvp.Key, Amount = kvp.Sum(x => x.Count) }).ToList();


            pokeBallsCount = pokeballsRequest.Where(p => p.ItemId == MiscEnums.Item.ITEM_POKE_BALL).
                DefaultIfEmpty(new { ItemId = MiscEnums.Item.ITEM_POKE_BALL, Amount = 0 }).FirstOrDefault().Amount;
            greatBallsCount = pokeballsRequest.Where(p => p.ItemId == MiscEnums.Item.ITEM_GREAT_BALL).
                DefaultIfEmpty(new { ItemId = MiscEnums.Item.ITEM_GREAT_BALL, Amount = 0 }).FirstOrDefault().Amount;
            ultraBallsCount = pokeballsRequest.Where(p => p.ItemId == MiscEnums.Item.ITEM_ULTRA_BALL).
                DefaultIfEmpty(new { ItemId = MiscEnums.Item.ITEM_ULTRA_BALL, Amount = 0 }).FirstOrDefault().Amount;
            masterBallsCount = pokeballsRequest.Where(p => p.ItemId == MiscEnums.Item.ITEM_MASTER_BALL).
                DefaultIfEmpty(new { ItemId = MiscEnums.Item.ITEM_MASTER_BALL, Amount = 0 }).FirstOrDefault().Amount;

        }


        public static MiscEnums.Item GetPokeballToUse(int? pokemonCP)
        {

            // Use better balls for high CP pokemon
            if (masterBallsCount > 0 && pokemonCP >= 1000)
            {
             //   ColoredConsoleWrite(ConsoleColor.Green, $"Master Ball is being used");
                return MiscEnums.Item.ITEM_MASTER_BALL;
            }

            if (ultraBallsCount > 0 && pokemonCP >= 600)
            {
              //  ColoredConsoleWrite(ConsoleColor.Green, $"Ultra Ball is being used");
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            }

            if (greatBallsCount > 0 && pokemonCP >= 350)
            {
             //   ColoredConsoleWrite(ConsoleColor.Green, $"Great Ball is being used");
                return MiscEnums.Item.ITEM_GREAT_BALL;
            }

            // If low CP pokemon, but no more pokeballs; only use better balls if pokemon are of semi-worthy quality
            if (pokeBallsCount > 0)
            {
                //   ColoredConsoleWrite(ConsoleColor.Green, $"Poke Ball is being used");
                return MiscEnums.Item.ITEM_POKE_BALL;
            }
            else if ((greatBallsCount < 40 && pokemonCP >= 200) || greatBallsCount >= 40)
            {
                //   ColoredConsoleWrite(ConsoleColor.Green, $"Great Ball is being used");
                return MiscEnums.Item.ITEM_GREAT_BALL;
            }
            else if (ultraBallsCount > 0 && pokemonCP >= 500)
            {
                //   ColoredConsoleWrite(ConsoleColor.Green, $"Ultra Ball is being used");
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            }
            else if (masterBallsCount > 0 && pokemonCP >= 700)
            {
                //    ColoredConsoleWrite(ConsoleColor.Green, $"Master Ball is being used");
                return MiscEnums.Item.ITEM_MASTER_BALL;
            }
            return MiscEnums.Item.ITEM_POKE_BALL;
        }

    }

}
