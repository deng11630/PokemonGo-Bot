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
        public static List<PokemonFamily> families;
        public static List<Item> items;
        public static int razzBerry = 0;
        public static int nbPokemons = 0;
        public static int pokeBallsCount = 0;
        public static int greatBallsCount = 0;
        public static int ultraBallsCount = 0;
        public static int masterBallsCount = 0;



        public static List<PokemonFamily> GetFamilies()
        {
            return inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && (int)p?.FamilyId > 0)
                    .OrderByDescending(p => (int)p.FamilyId).ToList();
        }

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


        public static MiscEnums.Item GetPokeballToUse(int? pokemonCP, int pokemonIV)
        {
            if (ReadSettings.pokeballMode == "CP")
            {
                if (pokemonCP > ReadSettings.cpToUseHyperBall)
                    return MiscEnums.Item.ITEM_ULTRA_BALL;
                if (pokemonCP > ReadSettings.cpToUseSuperBall)
                    return MiscEnums.Item.ITEM_GREAT_BALL;
                return MiscEnums.Item.ITEM_POKE_BALL;
            }
            if (pokemonIV > ReadSettings.IVToUseHyperBall)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            if (pokemonIV > ReadSettings.IVToUseSuperBall)
                return MiscEnums.Item.ITEM_GREAT_BALL;
            return MiscEnums.Item.ITEM_POKE_BALL;
        }
    }

}
