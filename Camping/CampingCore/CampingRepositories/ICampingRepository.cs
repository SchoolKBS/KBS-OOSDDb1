using CampingCore.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.CampingRepositories
{
    public interface ICampingRepository
    {
        public ICampingRepository CampingRepository { get;  set; }
        public ICampingPlaceRepository CampingPlaceRepository { get;  set; }
        public ICampingReservationRepository CampingReservationRepository { get;  set; }
        public ICampingGuestRepository CampingGuestRepository { get; set; }
        public ICampingMapRepository CampingMapRepository { get; set; }
        public void CreateGuestTable();
        public void CreateAreaTable();
        public void CreateStreetTable();
        public void CreatePlaceTable();
        public void CreateReservationTable();
    }
}
