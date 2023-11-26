using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for ReservationCreationPage.xaml
    /// </summary>
    public partial class ReservationCreationPage : Page
    {
        private PlacesOverviewPage _page;
        private Camping _camping;
        private Place place;
        public double price {  get; set; }
        public ReservationCreationPage(PlacesOverviewPage placesOverviewPage)
        {
            InitializeComponent();
            _page = placesOverviewPage;
            _camping = _page.Camping;
            ShowAvailableDatesArrival();
            DepartureDatePicker.IsEnabled = false;
            SetKnownInformation();
        }
        public void SetKnownInformation()
        {
            place = (Place)_page.PlacesListView.SelectedItem;
            if (_page.FilterAplied)
            {
                if(_page.ArrivalDatePicker.SelectedDate != null) ArrivalDatePicker.SelectedDate = _page.ArrivalDatePicker.SelectedDate.Value;
                if (_page.DepartureDatePicker.SelectedDate != null)
                {
                    DepartureDatePicker.IsEnabled = true;
                    DepartureDatePicker.SelectedDate = _page.DepartureDatePicker.SelectedDate.Value;
                }
                if(_page.PersonCount > 0)PeopleCountText.Text = _page.PersonCount.ToString();
            }

        }

        private void ArrivalDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
            ShowAvailableDatesDeparture();

        }

        private void DepartureDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
        }
        private double CalcPrice()
        {
            int.TryParse(PeopleCountText.Text, out var TextToString);
            if (TextToString.GetType() == typeof(int) && TextToString > 0)
            {
                return place.PricePerNight * TextToString;
            }
            else return price;
        }
        public bool CheckValues()
        {
            return true;
        }
        public bool CheckDates()
        {
            return true;
        }
        public bool CheckPeopleCount()
        {
            return true;
        }
        private void PeopleCountText_Changed(object sender, TextChangedEventArgs e)
        {
            price = CalcPrice();
            PriceTB.Text = price.ToString() + "$";
        }
        private void ShowAvailableDatesArrival()
        {
            ArrivalDatePicker.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ArrivalDatePicker.BlackoutDates.AddDatesInPast();
            var reservations = _camping.Reservations.Where(r => r.place.PlaceNumber == place.PlaceNumber && r.EindDatum > DateTime.Now).ToList();
            foreach( var reservation in reservations )
            {
                ArrivalDatePicker.BlackoutDates.Add(new CalendarDateRange(reservation.StartDatum, reservation.EindDatum));
            }
        }
        private void ShowAvailableDatesDeparture()
        {
            if (ArrivalDatePicker.SelectedDate != null)
            {
                DateTime arrivalDate = (DateTime) ArrivalDatePicker.SelectedDate;
                DepartureDatePicker.IsEnabled = true;
                DepartureDatePicker.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);
                DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, arrivalDate));
                var reservations = _camping.Reservations.Where(r => r.place.PlaceNumber == place.PlaceNumber && r.StartDatum > DateTime.Today).ToList();
                if (reservations.Count > 0)
                {
                    var SoonestReservationStart = reservations.Min(r => r.StartDatum);
                    DepartureDatePicker.DisplayDateEnd = new DateTime(SoonestReservationStart.Year, SoonestReservationStart.Month, DateTime.DaysInMonth(SoonestReservationStart.Year, SoonestReservationStart.Month));
                    DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(SoonestReservationStart, DepartureDatePicker.DisplayDateEnd.Value));
                }

            }
        }

        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AcceptReservation_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
