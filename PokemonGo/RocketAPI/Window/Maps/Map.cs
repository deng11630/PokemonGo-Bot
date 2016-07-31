using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public static class Map
    {

        public static GMapOverlay searchAreaOverlay = new GMapOverlay("areas");
        public static GMapOverlay pokestopsOverlay = new GMapOverlay("pokestops");
        public static GMapOverlay pokemonsOverlay = new GMapOverlay("pokemons");
        public static GMapOverlay playerOverlay = new GMapOverlay("players");
        public static GMarkerGoogle playerMarker;




        public static  void Load(GMapControl gMapControl1)
        {
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            GMapProvider.WebProxy = null;
            gMapControl1.Position = new PointLatLng(ReadSettings.defaultLatitude, ReadSettings.defaultLongitude);
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 15;

            gMapControl1.Overlays.Add(searchAreaOverlay);
            gMapControl1.Overlays.Add(pokestopsOverlay);
            gMapControl1.Overlays.Add(pokemonsOverlay);
            gMapControl1.Overlays.Add(playerOverlay);

            playerMarker = new GMarkerGoogle(new PointLatLng(ReadSettings.defaultLatitude, ReadSettings.defaultLongitude),
            GMarkerGoogleType.orange_small);
            playerOverlay.Markers.Add(playerMarker);

            InitializeMap();

        }

        public static void InitializeMap()
        {
            playerMarker.Position = new PointLatLng(ReadSettings.defaultLatitude, ReadSettings.defaultLongitude);

            searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(S2Helper.GetNearbyCellIds(ReadSettings.defaultLatitude, ReadSettings.defaultLongitude), searchAreaOverlay);
        }

        public static void UpdateMap()
        {
            MainForm.synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                pokestopsOverlay.Markers.Clear();
                List<PointLatLng> routePoint = new List<PointLatLng>();
                foreach (var pokeStop in Movements.pokeStops)
                {
                    GMarkerGoogleType type = GMarkerGoogleType.blue_small;
                    if (pokeStop.CooldownCompleteTimestampMs > DateTime.UtcNow.ToUnixTime())
                    {
                        type = GMarkerGoogleType.gray_small;
                    }
                    var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                    var pokestopMarker = new GMarkerGoogle(pokeStopLoc, type);
                        //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                        pokestopsOverlay.Markers.Add(pokestopMarker);

                    routePoint.Add(pokeStopLoc);
                }
                pokestopsOverlay.Routes.Clear();
                pokestopsOverlay.Routes.Add(new GMapRoute(routePoint, "Walking Path"));


                pokemonsOverlay.Markers.Clear();
                if (PokemonActions.wildPokemons != null)
                {
                    foreach (var pokemon in PokemonActions.wildPokemons)
                    {
                        var pokemonMarker = new GMarkerGoogle(new PointLatLng(pokemon.Latitude, pokemon.Longitude),
                            GMarkerGoogleType.red_small);
                        pokemonsOverlay.Markers.Add(pokemonMarker);
                    }
                }

                S2GMapDrawer.DrawS2Cells(S2Helper.GetNearbyCellIds(ReadSettings.defaultLongitude, ReadSettings.defaultLatitude), searchAreaOverlay);
            }), null);
        }

        public static void UpdatePlayerLocation(double latitude, double longitude)
        {
            MainForm.synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                playerMarker.Position = (PointLatLng)o;
                searchAreaOverlay.Polygons.Clear();
            }), new PointLatLng(latitude, longitude));       
        }

    }
}
