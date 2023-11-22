using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public interface ICampingRepository
    {
        public List<Place> GetPlaces();
        public List<Reservation> GetReservations();
    }
}
