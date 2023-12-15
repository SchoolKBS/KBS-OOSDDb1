using CampingCore;
using CampingDataAccess;
using CampingUI.GenerateComponentsMap;
using CampingUI.NewFolder;
using Org.BouncyCastle.Crypto.Operators;
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
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Camping _camping;
        private List<Area> _areas;
        private List<Place> _places;
        private List<Street> _streets;
        private int _xPressed;
        private int _yPressed;
        private bool _wrongInput;
        private int SelectedPlace;
        private int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID, _placeAreaID;
        private double _placePricePerNight;
        private Canvas previousSelectedCanvas;
        private bool _editPlaceBool;

        public MainPage(Camping camping)
        {
            InitializeComponent();

            _camping = camping;
            new Transform(field); // Transform scale of the map.
            GenerateMap();

            // For the keyboard handler
            Loaded += (sender, e) =>
            {
                Focusable = true;
                Keyboard.Focus(this);
            };

            KeyDown += Handle_KeyDown;      // Handle keyboard buttons.
        }

        public void GenerateMap()
        {
            GenerateAreas();
            GenerateStreets();
            GeneratePlaces();
        }

        public void GenerateAreas()
        {
            _areas = _camping.CampingRepository.CampingMapRepository.GetAreas();

            if (_areas.Count() > 0)
            {
                foreach (Area area in _areas)
                { 
                    field.Children.Add(MapPageArea.GenerateArea(area));
                }
            }
        }

        public void GenerateStreets()
        {
            _streets = _camping.CampingRepository.CampingMapRepository.GetStreets();
            if (_streets.Count() > 0)
            {
                foreach (Street street in _streets)
                {
                    field.Children.Add(MapPageStreet.GenerateStreet(street));
                }
            }
        }

        public void GeneratePlaces()
        {
            _places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            if (_places.Count() > 0)
            {
                foreach(Place place  in _places)
                {
                    GeneratePlace(place, Brushes.Black, true);
                }     
            }

        }

        public void GeneratePlace(Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            Border border = new Border
            {
                BorderBrush = Brushes.White, 
                BorderThickness = new Thickness(1), 
            };

            Canvas canvasPlace = new Canvas
            {
                Width = 30,
                Height = 30,
                Background = brush,
                Name = "Place_" + place.PlaceID.ToString(),
            };

            border.Child = canvasPlace;
            Canvas.SetZIndex(canvasPlace, 100);

            Canvas.SetTop(border, coordinates[1]); 
            Canvas.SetLeft(border, coordinates[0]); 

            field.Children.Add(border);

            if (AddPlaceBool)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = place.PlaceID.ToString(),
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 20,
                };

                canvasPlace.Children.Add(textBlock);
                canvasPlace.Loaded += (sender, e) =>
                {
                    Canvas.SetTop(textBlock, (canvasPlace.ActualHeight - textBlock.ActualHeight) / 2);
                    Canvas.SetLeft(textBlock, (canvasPlace.ActualWidth - textBlock.ActualWidth) / 2);
                };
            }

            Canvas.SetTop(canvasPlace, (border.ActualHeight - canvasPlace.Height) / 2);
            Canvas.SetLeft(canvasPlace, (border.ActualWidth - canvasPlace.Width) / 2);

            if(brush == Brushes.Black)
            {
                canvasPlace.MouseEnter += (sender, e) =>
                {
                    canvasPlace.Background = Brushes.DarkCyan; // Change the background color on hover
                };

                canvasPlace.MouseLeave += (sender, e) =>
                {
                    if (place.PlaceID != SelectedPlace)
                    {
                        canvasPlace.Background = Brushes.Black;
                    }
                };
            }

            canvasPlace.MouseLeftButtonDown += (sender, e) =>
            {
                if (previousSelectedCanvas != null)
                {
                    previousSelectedCanvas.Background = Brushes.Black;
                }

                canvasPlace.Background = Brushes.DarkCyan;
                previousSelectedCanvas = canvasPlace;
                HandlePlaceClick(place, false);
            };
        }

        public void HandlePlaceClick(Place place, bool AddPlaceBool)
        {
            PlaceInfo.Visibility = Visibility.Visible;
            if (!AddPlaceBool)
            {
                /*                SetPlaceDataOnFields(place);
                AddPlaceButton.Content = "Aanpassen";
                PlaceHasPower.IsEnabled = false;
                PlaceHasDogs.IsEnabled = false;
                PlaceSurfaceArea.IsEnabled = false;
                PlacePricePerNight.IsEnabled = false;
                PlacePersons.IsEnabled = false;
                PlaceStreetComboBox.IsEnabled = false;
                PlacePlaceID.IsEnabled = false;
                AddPlaceButton.Visibility = Visibility.Collapsed;
                _editPlaceBool = true;*/
                PlaceInfo.Visibility = Visibility.Collapsed;
                field.Children.Clear();
                GenerateMap();
            }
            else
            {
                ResetInputFields();
                AddPlaceButton.Content = "Toevoegen";
                _editPlaceBool = false;
                AddPlaceButton.Visibility = Visibility.Visible;
            }
                
        }

        private void ResetInputFields()
        {
            PlaceStreetComboBox.Items.Clear();
            PlaceAreaComboBox.Items.Clear();
            foreach (Street street in _camping.CampingRepository.CampingMapRepository.GetStreets())
            {
                PlaceStreetComboBox.Items.Add(street.Name);
            }
            foreach (Area area in _camping.CampingRepository.CampingMapRepository.GetAreas())
            {
                PlaceAreaComboBox.Items.Add(area.Name);
            }
            foreach (Grid grid in PlaceInfo.Children)
            {

                foreach (var component in grid.Children)
                {
                    if (component is TextBox textbox)
                    {
                        textbox.Text = null;
                        textbox.IsEnabled = true;
                    }
                    if (component is CheckBox checkbox)
                    {
                        checkbox.IsChecked = false;
                        checkbox.IsEnabled = true;
                    }
                    if (component is ComboBox combobox)
                    {
                        if (!combobox.Name.Equals(PlaceAreaComboBox.Name) && !combobox.Name.Equals(PlaceStreetComboBox.Name))
                        {
                            combobox.SelectedIndex = 2;
                            combobox.IsEnabled = false;
                        }

                    }
                }
            }
        }


        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            field.Children.Clear();
            GenerateMap();

            Point p = Mouse.GetPosition(field);
            List<Area> areas = _camping.CampingRepository.CampingMapRepository.GetAreas();
            List<Street> streets = _camping.CampingRepository.CampingMapRepository.GetStreets();
            List<Place> places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            foreach(Place place in places)
            {
                place.XCord -= 15;
                place.YCord -= 15;
            }
            _xPressed = (int)Math.Round(p.X) - 15;
            _yPressed = (int)Math.Round(p.Y) - 15;
            List<Area> PlaceWithinAreas = areas.Where(i => i.XCord1 <= (_xPressed - 15))
                                               .Where(i => i.XCord1 + i.Width >= (_xPressed + 45))
                                               .Where(i => i.YCord1 <= (_yPressed - 15))
                                               .Where(i => i.YCord1 + i.Height >= (_yPressed + 45))
                                               .ToList();
            List<Place> placesNotInNewPlaceBorder = places.Where(i => i.XCord >= (_xPressed-45) && i.XCord <= (_xPressed+45))
                                                          .Where(i => i.YCord >= (_yPressed-45) && i.YCord <= (_yPressed+45))
                                                          .ToList();
            if (PlaceWithinAreas.Count == 1 && placesNotInNewPlaceBorder.Count == 0)  
            {
                _camping.Places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                int i = _camping.Places.Last().PlaceID + 1;

                Place place = new Place(0, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                GeneratePlace(place, Brushes.Gray, false);
                EnableExtendComboBoxes(false);
                HandlePlaceClick(place, true);
            }
        }

        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            GetAddValues();
            if (!_wrongInput)
            {
                bool hasPower = false;
                if(PlaceHasPower.IsChecked == true)
                    hasPower = true;
                bool hasDogs = false;
                if (PlaceHasDogs.IsChecked == true)
                    hasDogs = true;
                Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                Area area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                Place place = new Place(_placePlaceID, hasPower, street.StreetID, area.AreaID, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNight, _xPressed, _yPressed);
                if (_editPlaceBool)
                    _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place.PlaceID, street.StreetID, area.AreaID, hasPower, _placeSurfaceArea, _placePricePerNight, _placePersons, hasDogs);
                else
                {
                    _camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    _camping.CampingRepository.CampingPlaceRepository.GetPlaces(); 
                }
                HandleCancelAddPlace();
            }
        }

        public void HandleCancelAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            HandleCancelAddPlace();
        }

        private void HandleCancelAddPlace()
        {
            foreach (Grid grid in PlaceInfo.Children)
            {
                foreach (var component in grid.Children)
                {
                    if (component is TextBox textbox)
                    {
                        StaticUIMethods.ResetTextboxBorder(textbox);
                    }
                    if (component is Border combobox)
                    {
                        ResetComboBoxBorder(combobox);
                    }
                }
            }
            PlaceInfo.Visibility = Visibility.Collapsed;
            field.Children.Clear();
            GenerateMap();
        }

        private void GetAddValues()
        {
            GetAddAmountOfPeople();
            GetAddPricePerNightPerPerson();
            GetAddSurfaceArea();
            GetAddPlaceID();
            GetAddStreetID();
            GetAddAreaID();
        }

        private void GetAddAmountOfPeople()
        {
            _placePersons = GetAddTextBox(PlacePersons, _placePersons);
        }

        private void GetAddPricePerNightPerPerson()
        {
            double number;
            if (double.TryParse(PlacePricePerNight.Text, out number) && number >= 0 && !string.IsNullOrEmpty(PlacePricePerNight.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                _placePricePerNight = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(PlacePricePerNight);
                _wrongInput = true;
            }
        }

        private void GetAddSurfaceArea()
        {
            _placeSurfaceArea = GetAddTextBox(PlaceSurfaceArea, _placePersons);
        }

        private void GetAddPlaceID()
        {
            _placePlaceID = GetAddTextBox(PlacePlaceID, _placePlaceID);
            List<Place> places = _camping.Places.Where(i => i.PlaceID == _placePlaceID).ToList();
            if (places.Count > 0 && !_editPlaceBool)
            {
                StaticUIMethods.SetErrorTextboxBorder(PlacePlaceID);
                _wrongInput = true;
                _placePlaceID = -1;
            }
        }

        private void GetAddStreetID()
        {
            if(PlaceStreetComboBox.SelectedItem != null)
            {
                Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                _placeStreetID = street.StreetID;
            }
            else
            {
                SetErrorComboBoxBorder(PlaceStreetBorder);
                _wrongInput = true;
            }
        }

        private void GetAddAreaID()
        {
            if (PlaceAreaComboBox.SelectedItem != null)
            {
                Area area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                _placeAreaID = area.AreaID;
            }
            else
            {
                SetErrorComboBoxBorder(PlaceAreaBorder);
                _wrongInput = true;
            }
        }

        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetTextboxBorder(textbox);
                _wrongInput = false;
            }
        }

        private int GetAddTextBox(TextBox textbox, int editNumber)
        {
            int number;
            if (int.TryParse(textbox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(textbox.Text))// Checks if int can be parsed and if number is bigger or equal to 0
                editNumber = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return editNumber;
        }

        private void PlaceStreetAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            Border border = PlaceAreaBorder;
            if (combobox.Name.Equals(PlaceStreetComboBox.Name)) border = PlaceStreetBorder;
            if (combobox.BorderBrush.Equals(Brushes.Red))
            {
                ResetComboBoxBorder(border);
                _wrongInput = false;
            }
            if (PlaceStreetComboBox.SelectedItem != null && PlaceAreaComboBox.SelectedItem != null)
            {
                EnableExtendComboBoxes(true);

                foreach (Grid grid in PlaceInfo.Children)
                {

                    foreach (var component in grid.Children)
                    {
                        if (component is ComboBox com)
                        {
                            if(com.Name != PlaceStreetComboBox.Name && com.Name != PlaceAreaComboBox.Name) {
                                HandleExtending(com);
                            }
                        }
                    }
                }
            }
            else
            {
                EnableExtendComboBoxes(false);
            }
        }

        private void EnableExtendComboBoxes(bool extend)
        {
            foreach(Grid grid in PlaceInfo.Children)
            {
                foreach(var component in grid.Children)
                {
                    if(component is ComboBox comboBox)
                    {
                        if (extend == false) comboBox.Opacity = 0.5;
                        else comboBox.Opacity = 1;
                        comboBox.IsEnabled = extend;
                    }
                }
            }
        }

        public void SetErrorComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.Red;
            border.BorderThickness = new Thickness(3, 3, 3, 3);
        }

        public void ResetComboBoxBorder(Border border)
        {
            border.BorderBrush = Brushes.White;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            HandleExtending(comboBox);
        }

        public void HandleExtending(ComboBox comboBox)
        {
            Area area = null;
            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceAreaComboBox.SelectedItem != null)
                {
                    area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                }
            }

            Street street = null;
            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceStreetComboBox.SelectedItem != null)
                {
                    street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                }
            }

            if (street != null || area != null)
            {
                foreach (Grid grid in PlaceInfo.Children)
                {

                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                        {
                            if (comboBox.Name.Contains("SurfaceArea") && textbox.Name.Contains("SurfaceArea"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.SurfaceArea.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.SurfaceArea.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("PricePerNight") && textbox.Name.Contains("PricePerNight"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.PricePerNightPerPerson.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.PricePerNightPerPerson.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("Persons") && textbox.Name.Contains("Persons"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    textbox.Text = street.AmountOfPeople.ToString();
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    textbox.Text = area.AmountOfPeople.ToString();
                                }
                                textbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    textbox.IsEnabled = true;
                                }
                            }
                        }

                        if (component is CheckBox checkbox)
                        {
                            if (comboBox.Name.Contains("Power") && checkbox.Name.Contains("Power"))
                            {

                                if (comboBox.SelectedIndex == 0)
                                {
                                    checkbox.IsChecked = street.Power;
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    checkbox.IsChecked = area.Power;
                                }
                                checkbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    checkbox.IsEnabled = true;
                                }
                            }
                            if (comboBox.Name.Contains("Dogs") && checkbox.Name.Contains("Dogs"))
                            {
                                if (comboBox.SelectedIndex == 0)
                                {
                                    checkbox.IsChecked = street.Dogs;
                                }
                                if (comboBox.SelectedIndex == 1)
                                {
                                    checkbox.IsChecked = area.Dogs;
                                }
                                checkbox.IsEnabled = false;
                                if (comboBox.SelectedIndex == 2)
                                {
                                    checkbox.IsEnabled = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is the Escape key
            if (e.Key == Key.Escape)
            {
                if(PlaceInfo.Visibility == Visibility.Visible)
                {
                    HandleCancelAddPlace();
                }
            }
        }
    }
}
