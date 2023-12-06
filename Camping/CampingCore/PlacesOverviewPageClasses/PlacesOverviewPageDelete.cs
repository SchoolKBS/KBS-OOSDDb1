﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.PlacesOverviewPageClasses
{
    public  class PlacesOverviewPageDelete
    {
        public static void DeletePlace(Camping camping, Place place, DateTime departureDate)
        {
            camping.CampingRepository.CampingReservationRepository.RemoveAllPreviousReservationsByPlace(place, departureDate);
            camping.CampingRepository.CampingPlaceRepository.RemovePlace(place);
        }
    }
}
