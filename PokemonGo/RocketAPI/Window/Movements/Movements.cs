using AllEnum;
using MoreLinq;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class Movements
    {
        public static bool waitUnlock = false;
        public static double mpi180 = Math.PI / 180;
        public static bool farmingStops = false;
        public static LocationManager locationManager;
        public static List<FortData> pokeStops = new List<FortData>();

        public static FortData GetNearestPokestop(List<FortData> pokeStops)
        {
            var PossiblePokestop = pokeStops.
                   Where(i => i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime()).MinBy(i => locationManager.getDistance(i.Latitude, i.Longitude));
            if (PossiblePokestop == null)
            {
                PossiblePokestop = pokeStops.MinBy(i => i.CooldownCompleteTimestampMs);
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"All pokestops in cooldown, wait.");
            }
            if (PossiblePokestop == null)
            {
                ConsoleWriter.ColoredConsoleWrite(Color.Red, $"Please choice a spot with at least 1 pokestop.");
                return null;
            }
            return PossiblePokestop;
        }


        public static double distanceFrom(double lat1, double lon1, double lat2, double lon2)
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

        public static double PokestopDefaultPosDist(FortData i)
        {
            return distanceFrom(i.Latitude, i.Longitude, ReadSettings.defaultLatitude, ReadSettings.defaultLongitude);
        }

        public static long CooldownTimeLeft(FortData pokestop)
        {
            return pokestop.CooldownCompleteTimestampMs - DateTime.UtcNow.ToUnixTime();
        }
      


        public static async Task FarmAllInRange(Client client)
        {

            farmingStops = true;
            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            if (pokeStops.Count < 1)
                pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && PokestopDefaultPosDist(i) < ReadSettings.deplacementsMaxDist).ToList();
            if (!ConsoleWriter.PokestopFarmStart(pokeStops))
                return;
            pokeStops = RouteOptimizer.Optimize(pokeStops, client.getCurrentLat(), client.getCurrentLong(), Map.pokestopsOverlay);
            PokemonActions.wildPokemons = mapObjects.MapCells.SelectMany(i => i.WildPokemons).ToList();
            ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"Travel mode : Farm all in range of " + ReadSettings.deplacementsMaxDist.ToString("N3") + "Km");
            double traveledDistance = 0;
            var nextPokestopsTravel = new List<FortData>(pokeStops);
            foreach (var pokeStop in pokeStops)
            {
                if (CooldownTimeLeft(pokeStop) > 30) continue;
                traveledDistance = distanceFrom(pokeStop.Latitude, pokeStop.Longitude, client.getCurrentLat(), client.getCurrentLong());
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Map.UpdateMap();
                int wait = ReadSettings.waitForUnlock;
                if (!ReadSettings.instantMoove)
                {
                    wait = (int)(ReadSettings.timePerKmMs * traveledDistance); //speed * millisecondes
                    ConsoleWriter.StartPokestop(traveledDistance);
                }
                do
                {
                    await Task.Delay(wait);
                    wait += ReadSettings.waitForUnlock;
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    ConsoleWriter.PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0 && !MainForm.unbanning && !MainForm.stop);
                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddMinutes(5).ToUnixTime();
                var pokeStopMapObjects = await client.GetMapObjects();

                nextPokestopsTravel.AddRange(
                    pokeStopMapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Checkpoint && nextPokestopsTravel.Where(j => j.Id == i.Id).Count() < 1
                     && PokestopDefaultPosDist(i) < ReadSettings.deplacementsMaxDist)
                    .ToList());
                if (ReadSettings.catchPokemon)
                    await PokemonActions.ExecuteCatchAllNearbyPokemons(pokeStopMapObjects);
                if (MainForm.unbanning || MainForm.stop)
                {
                    farmingStops = false;
                    return;
                }
            }
            pokeStops = nextPokestopsTravel;  
        }

        public static async Task FarmXPokestops(Client client)
        {

            farmingStops = true;
            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            if (pokeStops.Count < 1)
                pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();
            if (pokeStops.Count > ReadSettings.maxPokestop)
                pokeStops = pokeStops.OrderByDescending(i => PokestopDefaultPosDist(i)).Skip(ReadSettings.maxPokestop - pokeStops.Count).ToList();
            if (!ConsoleWriter.PokestopFarmStart(pokeStops))
                return;
            pokeStops = RouteOptimizer.Optimize(pokeStops, client.getCurrentLat(), client.getCurrentLong(), Map.pokestopsOverlay);
            PokemonActions.wildPokemons = mapObjects.MapCells.SelectMany(i => i.WildPokemons).ToList();
            ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"Travel mode : Farm and save " + ReadSettings.maxPokestop + " pokestops");
            double traveledDistance = 0;
            var nextPokestopsTravel = new List<FortData>(pokeStops);
            foreach (var pokeStop in pokeStops)
            {               
                if (CooldownTimeLeft(pokeStop) > 30) continue;
                traveledDistance = distanceFrom(pokeStop.Latitude, pokeStop.Longitude, client.getCurrentLat(), client.getCurrentLong());
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Map.UpdateMap();
                int wait = ReadSettings.waitForUnlock;
                if (!ReadSettings.instantMoove)
                {
                    wait = (int)(ReadSettings.timePerKmMs * traveledDistance); //speed * millisecondes
                    ConsoleWriter.StartPokestop(traveledDistance);
                }
                do
                {
                    await Task.Delay(wait);
                    wait += ReadSettings.waitForUnlock;
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    ConsoleWriter.PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0 && !MainForm.unbanning && !MainForm.stop);
                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddMinutes(5).ToUnixTime();
                var pokeStopMapObjects = await client.GetMapObjects();

                if (pokeStops.Count < ReadSettings.maxPokestop)
                {
                    nextPokestopsTravel.AddRange(
                    pokeStopMapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Checkpoint && nextPokestopsTravel.Where(j => j.Id == i.Id).Count() < 1).ToList());
                    if (nextPokestopsTravel.Count > ReadSettings.maxPokestop)
                        nextPokestopsTravel = nextPokestopsTravel.OrderByDescending(i => PokestopDefaultPosDist(i)).Skip(ReadSettings.maxPokestop - pokeStops.Count).ToList();
                }

                
                if (ReadSettings.catchPokemon)
                    await PokemonActions.ExecuteCatchAllNearbyPokemons(pokeStopMapObjects);
                if (MainForm.unbanning || MainForm.stop)
                {
                    farmingStops = false;
                    return;
                }
            }
            pokeStops = nextPokestopsTravel; 
        }


        public static async Task FarmAllWithoutSaving(Client client)
        {

            farmingStops = true;
            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            if (pokeStops.Count < 1)
                pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();
            if (!ConsoleWriter.PokestopFarmStart(pokeStops))
                return;
            pokeStops = RouteOptimizer.Optimize(pokeStops, client.getCurrentLat(), client.getCurrentLong(), Map.pokestopsOverlay);
            PokemonActions.wildPokemons = mapObjects.MapCells.SelectMany(i => i.WildPokemons).ToList();
            ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"Travel mode : Travel the world step by step");
            double traveledDistance = 0;             
            foreach (var pokeStop in pokeStops)
            {
                if (CooldownTimeLeft(pokeStop) > 30) continue;
                traveledDistance = distanceFrom(pokeStop.Latitude, pokeStop.Longitude, client.getCurrentLat(), client.getCurrentLong());
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Map.UpdateMap();
                int wait = ReadSettings.waitForUnlock;
                if (!ReadSettings.instantMoove)
                {
                    wait = (int)(ReadSettings.timePerKmMs * traveledDistance); //speed * millisecondes
                    ConsoleWriter.StartPokestop(traveledDistance);
                }
                do
                {
                    await Task.Delay(wait);
                    wait += ReadSettings.waitForUnlock;
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    ConsoleWriter.PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0 && !MainForm.unbanning && !MainForm.stop);
                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddMinutes(5).ToUnixTime();
                mapObjects = await client.GetMapObjects();
                if (ReadSettings.catchPokemon)
                    await PokemonActions.ExecuteCatchAllNearbyPokemons(mapObjects);
                if (MainForm.unbanning || MainForm.stop)
                {
                    farmingStops = false;
                    return;
                }
            }
            pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();  
        }



        public static async Task FarmAllWithSaving(Client client)
        {

            farmingStops = true;
            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            if (pokeStops.Count < 1)
                pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();
            if (!ConsoleWriter.PokestopFarmStart(pokeStops))
                return;
            pokeStops = RouteOptimizer.Optimize(pokeStops, client.getCurrentLat(), client.getCurrentLong(), Map.pokestopsOverlay);
            PokemonActions.wildPokemons = mapObjects.MapCells.SelectMany(i => i.WildPokemons).ToList();
            ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"Travel mode : Travel the world and save every pokestops !!");
            double traveledDistance = 0;
            var nextPokestopsTravel = new List<FortData>(pokeStops);
            foreach (var pokeStop in pokeStops)
            {
                if (CooldownTimeLeft(pokeStop) > 30) continue;
                traveledDistance = distanceFrom(pokeStop.Latitude, pokeStop.Longitude, client.getCurrentLat(), client.getCurrentLong());
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Map.UpdateMap();
                int wait = ReadSettings.waitForUnlock;
                if (!ReadSettings.instantMoove)
                {
                    wait = (int)(ReadSettings.timePerKmMs * traveledDistance); //speed * millisecondes
                    ConsoleWriter.StartPokestop(traveledDistance);
                }
                do
                {
                    await Task.Delay(wait);
                    wait += ReadSettings.waitForUnlock;
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    ConsoleWriter.PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0 && !MainForm.unbanning && !MainForm.stop);
                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddMinutes(5).ToUnixTime();
                var pokeStopMapObjects = await client.GetMapObjects();

                nextPokestopsTravel.AddRange(
                    pokeStopMapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Checkpoint && nextPokestopsTravel.Where(j => j.Id == i.Id).Count() < 1)
                    .ToList());
                if (ReadSettings.catchPokemon)
                    await PokemonActions.ExecuteCatchAllNearbyPokemons(pokeStopMapObjects);
                if (MainForm.unbanning || MainForm.stop)
                {
                    farmingStops = false;
                    return;
                }
            }
            pokeStops = nextPokestopsTravel;      
        }

        public static async Task GoWhereYouWantButFarm(Client client)
        {

            farmingStops = true;
            FortSearchResponse fortSearch;
            var mapObjects = await client.GetMapObjects();
            var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();
            ConsoleWriter.ColoredConsoleWrite(Color.Cyan, $"Travel the world and discover a new pokestops !!");  
            double traveledDistance = 0;
            if (!ConsoleWriter.PokestopFarmStart(pokeStops))
                return;
            do
            {
                var pokeStop = GetNearestPokestop(pokeStops);
                traveledDistance = distanceFrom(pokeStop.Latitude, pokeStop.Longitude, client.getCurrentLat(), client.getCurrentLong());
                if (pokeStop == null)
                    return;
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Map.UpdateMap();
                int wait = ReadSettings.waitForUnlock;
                if (!ReadSettings.instantMoove)
                {
                    wait = (int)(ReadSettings.timePerKmMs * traveledDistance); //speed * millisecondes
                    ConsoleWriter.StartPokestop(traveledDistance);
                }
                do
                {
                    await Task.Delay(wait);
                    fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                    ConsoleWriter.PrintPokestopTakeInfos(fortInfo, fortSearch);
                }
                while (fortSearch.ExperienceAwarded == 0 && !MainForm.unbanning && !MainForm.stop);
                var pokeStopMapObjects = await client.GetMapObjects();
                pokeStops =  pokeStopMapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint).ToList();
                if (ReadSettings.catchPokemon)
                    await PokemonActions.ExecuteCatchAllNearbyPokemons(pokeStopMapObjects);


            } while (!MainForm.unbanning && !MainForm.stop);
            farmingStops = false;
        }


    }
}
