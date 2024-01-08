using CampingCore;
using CampingDataAccess;
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
        private Place _place;
        public double Price {  get; set; }
        public ReservationCreationPage(PlacesOverviewPage placesOverviewPage)
        {
            InitializeComponent();
            _page = placesOverviewPage;

            _camping = _page.GetCamping();
            _camping.SetReservations(_camping.CampingRepository.CampingReservationRepository.GetReservations());

            _place = (Place) _page.PlacesListView.SelectedItem;
            ShowAvailableDatesArrival();
            DepartureDatePicker.IsEnabled = false;
            SetKnownInformation();
        }
        public void SetKnownInformation()
        {
            _place = (Place)_page.PlacesListView.SelectedItem;
            if (_page.GetFilterApplied())
            {
                if(_page.ArrivalDatePicker.SelectedDate != null) ArrivalDatePicker.SelectedDate = _page.ArrivalDatePicker.SelectedDate.Value;
                if (_page.DepartureDatePicker.SelectedDate != null)
                {
                    DepartureDatePicker.IsEnabled = true;
                    DepartureDatePicker.SelectedDate = _page.DepartureDatePicker.SelectedDate.Value;
                }
                if(_page.GetCampingAmountOfPeople() > 0)PeopleCountText.Text = _page.GetCampingAmountOfPeople().ToString();
            }
        }
        private void ArrivalDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartureDatePicker.SelectedDate = null;
            ArrivalDatePicker.BorderBrush = Brushes.White;
            DepartureDatePicker.BorderBrush = Brushes.White;
            ArrivalDatePicker.BorderThickness = new Thickness(1,1,1,1);
            DepartureDatePicker.BorderThickness = new Thickness(1,1,1,1);


            this.ArrivalDatePicker.Text = ArrivalDatePicker.SelectedDate.ToString();
            ShowAvailableDatesDeparture();

        }
        private void DepartureDatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            DepartureDatePicker.BorderBrush = Brushes.White;
            DepartureDatePicker.BorderThickness = new Thickness(1, 1, 1, 1);
            this.DepartureDatePicker.Text = DepartureDatePicker.SelectedDate.ToString();
            Price = CalcPrice();
            PriceLabel.Content = Price.ToString() + "€";
        }
        private double CalcPrice()
        {
            int.TryParse(PeopleCountText.Text, out var TextToInt);
            if (TextToInt.GetType() == typeof(int) && TextToInt > 0 && TextToInt <= _place.AmountOfPeople && ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue)
            {
                int dayscount = (int) DepartureDatePicker.SelectedDate.Value.Subtract(ArrivalDatePicker.SelectedDate.Value).TotalDays;
                PeopleCountText.BorderBrush = Brushes.White;
                PeopleCountText.BorderThickness = new Thickness(1,1,1,1);
                
                return _place.PricePerNightPerPerson * TextToInt * dayscount;
            }
            else
            {
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3, 3);
                return 0;
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
                ArrivalDatePicker.BorderBrush = Brushes.Red;
                ArrivalDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (!DepartureDatePicker.SelectedDate.HasValue)
            {
                DepartureDatePicker.BorderBrush = Brushes.Red;
                DepartureDatePicker.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            return ArrivalDatePicker.SelectedDate.HasValue && DepartureDatePicker.SelectedDate.HasValue;
        }
        public bool CheckPeopleCount()
        {
            bool result = int.TryParse(PeopleCountText.Text, out int number);
            if (!result)
            {
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            else if (number > _place.AmountOfPeople)
            {
                result = false;;
                PeopleCountText.BorderBrush = Brushes.Red;
                PeopleCountText.BorderThickness = new Thickness(3, 3, 3,3);
            }
            return result;
        }
        public bool CheckGuestInputFields()
        {
            if (FirstNameTB.Text.IsNullOrEmpty())
            {
                FirstNameTB.BorderBrush = Brushes.Red;
                FirstNameTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (LastnameTB.Text.IsNullOrEmpty())
            {
                LastnameTB.BorderBrush = Brushes.Red;
                LastnameTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
            if (PhoneNumberTB.Text.IsNullOrEmpty())
            {
                PhoneNumberTB.BorderBrush = Brushes.Red;
                PhoneNumberTB.BorderThickness = new Thickness(3, 3, 3, 3);
            }
                return !FirstNameTB.Text.IsNullOrEmpty() && !LastnameTB.Text.IsNullOrEmpty() && !PhoneNumberTB.Text.IsNullOrEmpty();
        }
        private void PeopleCountText_Changed(object sender, TextChangedEventArgs e)
        {
            Price = CalcPrice();
            PriceLabel.Content = Price.ToString() + "$";

        }
        private void ShowAvailableDatesArrival()
        {
            ArrivalDatePicker.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ArrivalDatePicker.BlackoutDates.AddDatesInPast();
            var reservations = _camping.GetReservations().Where(r => r.PlaceID == _place.PlaceID && r.DepartureDate > DateTime.Now).ToList();
            foreach( var reservation in reservations )
            {
                ArrivalDatePicker.BlackoutDates.Add(new CalendarDateRange(reservation.ArrivalDate, reservation.DepartureDate));
            }
        }
        private void ShowAvailableDatesDeparture()
        {
            if (ArrivalDatePicker.SelectedDate != null)
            {
                DepartureDatePicker.DisplayDateEnd = null;
                DepartureDatePicker.BlackoutDates.Clear();
                DateTime arrivalDate = (DateTime) ArrivalDatePicker.SelectedDate;
                DepartureDatePicker.IsEnabled = true;
                DepartureDatePicker.DisplayDateStart = new DateTime(arrivalDate.Year, arrivalDate.Month, 1);
                DepartureDatePicker.BlackoutDates.Add(new CalendarDateRange(DepartureDatePicker.DisplayDateStart.Value, arrivalDate));
                var reservations = _camping.GetReservations().Where(r => r.PlaceID == _place.PlaceID && r.ArrivalDate > DateTime.Today).ToList();
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
                _camping.CampingRepository.CampingGuestRepository.AddGuest(guest);
                //Database db = new Database();
                //db.AddGuestToDatabase(guest);
                _camping.CampingRepository.CampingReservationRepository.AddReservation(new Reservation(0, (DateTime)ArrivalDatePicker.SelectedDate, (DateTime)DepartureDatePicker.SelectedDate, _place.PlaceID, _camping.CampingRepository.CampingGuestRepository.GetLastGuestID(), int.Parse(PeopleCountText.Text), IsPaidCB.IsChecked.Value, Price));
                NavigationService.Navigate(new PlacesOverviewPage(_camping, (CampingRepository)_camping.CampingRepository));
            }
        }
        private void Input_Changed(object sender, TextChangedEventArgs e)
        {
            if(sender.GetType() == typeof(TextBox)) {
                TextBox textbox = (TextBox)sender;
                if (textbox.BorderBrush.Equals(Brushes.Red))
                {
                    textbox.BorderThickness = new Thickness(1, 1, 1, 1);
                    textbox.BorderBrush = Brushes.White;
                }

            }
        }
    }
}
