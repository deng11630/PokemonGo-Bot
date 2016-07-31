﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public class LocationManager
    {
        private Client client;
        private double kilometersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this.client = client;
            this.kilometersPerMillisecond = speed / 3600000;
        }

        public double getDistance(double lat, double lng)
        {
            Coordinate currentLoc = new Coordinate(client.getCurrentLat(), client.getCurrentLong());
            return currentLoc.distanceFrom(new Coordinate(lat, lng));
        }

        public async Task update(double lat, double lng)
        {
            Map.UpdatePlayerLocation(lat, lng);       
            await client.UpdatePlayerLocation(lat, lng);
            return ;
        }


        public double BasicDist(double lat, double lng)
        {

            return (float)Math.Sqrt(Math.Pow(lat - lng, 2) + Math.Pow(client.getCurrentLat() - client.getCurrentLong(), 2));

        }
        public static double mpi180 = Math.PI / 180;
        public struct Coordinate
        {

            public Coordinate(double lat, double lng)
            {
                this.latitude = lat;
                this.longitude = lng;
            }
            public double latitude;
            public double longitude;



            //returns distance in kilometers 
            public double distanceFrom(Coordinate c2)
            {

                var lat2 = c2.latitude;
                var lon2 = c2.longitude;

                var lat1 = this.latitude;
                var lon1 = this.longitude;

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
        }
    }

}
