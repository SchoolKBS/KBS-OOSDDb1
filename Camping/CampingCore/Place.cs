﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Place
    {
        public int PlaceNumber { get; set; }
        public bool HasPower { get; set; }
        public int SurfaceArea { get; set; }
        public int NumberOfPeople { get; set; }
        public int PricePerNight { get; set; }
        public string Description { get; set; }

        public Place(int placeNumber, bool hasPower, int surfaceArea, int numberOfPeople, int pricePerNight)
        {
            this.PlaceNumber = placeNumber;
            this.HasPower = hasPower;
            this.SurfaceArea = surfaceArea;
            this.NumberOfPeople = numberOfPeople;
            this.PricePerNight = pricePerNight;
        }
        public override string ToString()
        {
            return "Plaats: " + PlaceNumber;
        }
    }
}
