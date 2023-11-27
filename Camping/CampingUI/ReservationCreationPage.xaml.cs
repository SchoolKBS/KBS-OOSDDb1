using CampingCore;
using Microsoft.IdentityModel.Tokens;
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
            place = (Place) _page.PlacesListView.SelectedItem;
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
            ArrivalDatePicker.Background = null;
            DepartureDatePicker.SelectedDate = null;
            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
            ShowAvailableDatesDeparture();

        }

        private void DepartureDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartureDatePicker.Background = null;
            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
        }
        private double CalcPrice()
        {
            int.TryParse(PeopleCountText.Text, out var TextToInt);
            if (TextToInt.GetType() == typeof(int) && TextToInt > 0 && TextToInt <= place.PersonCount && ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue)
            {
                int dayscount = (int) DepartureDatePicker.SelectedDate.Value.Subtract(ArrivalDatePicker.SelectedDate.Value).TotalDays;
                PeopleCountText.Background = null;
                return place.PricePerNight * TextToInt * dayscount;
            }
            else
            {
                PeopleCountText.Background = Brushes.Red;
                return price;
            }
        }
        public bool CheckValues()
        {
            bool result = true;
            if(!CheckDates() && result) result = false;
            if(!CheckPeopleCount() && result) result = false;
            if(!CheckGuestInputFields() && result) result = false;
            return result;
        }
        public bool CheckDates()
        {
            if (!ArrivalDatePicker.SelectedDate.HasValue)
            {
                ArrivalDatePicker.Background = Brushes.Red;
            }
            if (!DepartureDatePicker.SelectedDate.HasValue)
            {
                DepartureDatePicker.Background = Brushes.Red;
            }
            return ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue;
        }
        public bool CheckPeopleCount()
        {
            bool result = int.TryParse(PeopleCountText.Text, out int number);
            if (!result) PeopleCountText.Background = Brushes.Red;
            else if(number > place.PersonCount)
            {
                result = false;
                PeopleCountText.Background = Brushes.Red;
            }
            return result;
        }
        public bool CheckGuestInputFields()
        {
            if (FirstNameTB.Text.IsNullOrEmpty()) FirstNameTB.Background = Brushes.Red;
            if (LastnameTB.Text.IsNullOrEmpty()) LastnameTB.Background = Brushes.Red;
            if (PhoneNumberTB.Text.IsNullOrEmpty()) PhoneNumberTB.Background = Brushes.Red;
            return !FirstNameTB.Text.IsNullOrEmpty() && !LastnameTB.Text.IsNullOrEmpty() && !PhoneNumberTB.Text.IsNullOrEmpty();
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
            var reservations = _camping.Reservations.Where(r => r.PlaceID == place.PlaceNumber && r.DepartureDate > DateTime.Now).ToList();
            foreach( var reservation in reservations )
            {
                ArrivalDatePicker.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
            }
        }
        private void ShowAvailableDatesDeparture()
        {
            if (ArrivalDatePicker.SelectedDate != null)
            {
                DepartureDatePicker.BlackoutDates.Clear();
                DateTime arrivalDate = (DateTime) ArrivalDatePicker.SelectedDate;
                DepartureDatePicker.IsEnabled = true;
                DepartureDatePicker.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);
                DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, arrivalDate));
                var reservations = _camping.Reservations.Where(r => r.PlaceID == place.PlaceNumber && r.ArrivalDate > DateTime.Today).ToList();
                if (reservations.Count > 0  && reservations.Min(r => r.ArrivalDate) > ArrivalDatePicker.SelectedDate)
                {
                    var SoonestReservationStart = reservations.Min(r => r.ArrivalDate);
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
            if (CheckValues())
            {
                Guest guest = new Guest(FirstNameTB.Text, InfixTB.Text, LastnameTB.Text, AddressTB.Text, CityTB.Text, EmailTB.Text, PhoneNumberTB.Text, PostalCodeTB.Text);
                _camping.CampingRepository.AddGuest(guest);
                _camping.CampingRepository.AddReservation(new Reservation(0, (DateTime)ArrivalDatePicker.SelectedDate, (DateTime)DepartureDatePicker.SelectedDate, place.PlaceNumber, 2, _camping.CampingRepository.GetLastGuestID(), int.Parse(PeopleCountText.Text), IsPaidCB.IsChecked.Value, price));
                NavigationService.GoBack();
            }
        }

        private void Input_Changed(object sender, TextChangedEventArgs e)
        {
            if(sender.GetType() == typeof(TextBox)) {
                TextBox textbox = (TextBox)sender;
                textbox.Background = null;
            }
        }
    }
}
