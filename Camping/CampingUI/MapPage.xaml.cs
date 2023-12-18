﻿using CampingCore;
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
        private int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID;
        private double _placePricePerNight;
        private Canvas previousSelectedCanvas;
        private bool _editPlaceBool;

        public MainPage(Camping camping)
        {
            InitializeComponent();
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double desiredWidth = 1000;  
            double desiredHeight = 750;  

            double aspectRatio = desiredWidth / desiredHeight;
            double screenAspectRatio = screenWidth / screenHeight;

            double scaleX = 1.0;
            double scaleY = 1.0;

            if (screenAspectRatio > aspectRatio)
            {
                scaleX = screenWidth / desiredWidth / 1.75;
                scaleY = scaleX;
            }
            else
            {
                scaleY = screenHeight / desiredHeight / 1.75;
                scaleX = scaleY;
            }
            ApplyScaleTransform(scaleX, scaleY);
            _camping = camping;
            DataContext = this;

            // Generate areas with their streets (and later places)
            GenerateMap();
        }
        private void ApplyScaleTransform(double scaleX, double scaleY)
        {
            if (field.FindName("plattegrond") is ScaleTransform plattegrond)
            {
                plattegrond.ScaleX = scaleX;
                plattegrond.ScaleY = scaleY;
            }
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
                foreach (var area in _areas)
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

                // Ensure the TextBlock is centered within the CanvasPlace
                canvasPlace.Loaded += (sender, e) =>
                {
                    Canvas.SetTop(textBlock, (canvasPlace.ActualHeight - textBlock.ActualHeight) / 2);
                    Canvas.SetLeft(textBlock, (canvasPlace.ActualWidth - textBlock.ActualWidth) / 2);
                };
            }

            // Center the Canvas within the Border
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
                    // Change the background color of the previously selected canvas back to black.
                    previousSelectedCanvas.Background = Brushes.Black;
                }

                // Set the background color of the current canvas to red
                canvasPlace.Background = Brushes.DarkCyan;

                // Update the reference to the current canvas
                previousSelectedCanvas = canvasPlace;

                // Handle the click event
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
                PlacePlaceID.Text = null;
                PlaceSurfaceArea.Text = null;
                PlacePersons.Text = null;
                PlacePricePerNight.Text = null;
                PlaceStreetComboBox.SelectedItem = null;
                PlaceHasDogs.IsChecked = false;
                PlaceHasPower.IsChecked = false;
                PlaceStreetComboBox.Items.Clear();
                foreach(Street street in _camping.CampingRepository.CampingMapRepository.GetStreets())
                {
                    PlaceStreetComboBox.Items.Add(street.Name);
                } 
                AddPlaceButton.Content = "Toevoegen";
                _editPlaceBool = false;
                PlacePlaceID.IsEnabled = true;
                PlaceHasPower.IsEnabled = true;
                PlaceHasDogs.IsEnabled = true;
                PlaceSurfaceArea.IsEnabled = true;
                PlacePricePerNight.IsEnabled = true;
                PlacePersons.IsEnabled = true;
                PlaceStreetComboBox.IsEnabled = true;
                ExtendStreetPlaceButton.IsEnabled = false;
                AddPlaceButton.Visibility = Visibility.Visible;

            }
                
        }
        private void SetPlaceDataOnFields(Place place)
        {
            SelectedPlace = place.PlaceID;
            PlacePlaceID.Text = place.PlaceID.ToString();
            PlaceHasPower.IsChecked = place.Power;
            PlaceHasDogs.IsChecked = place.Dogs;
            PlaceSurfaceArea.Text = place.SurfaceArea.ToString();
            PlacePricePerNight.Text = place.PricePerNightPerPerson.ToString();
            PlacePersons.Text = place.AmountOfPeople.ToString();
            PlaceStreetComboBox.Text = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place).Name;
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

                Place place = new Place(0, false, 1, false, 0, 0, 0, _xPressed, _yPressed);
                GeneratePlace(place, Brushes.Gray, false);
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
                Street street = _camping.CampingRepository.CampingMapRepository.GetSteetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                Place place = new Place(_placePlaceID, hasPower, street.StreetID, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNight, _xPressed, _yPressed);
                if (_editPlaceBool)
                    _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place.PlaceID, street.StreetID, hasPower, _placeSurfaceArea, _placePricePerNight, _placePersons, hasDogs);
                else
                {
                    _camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    _camping.CampingRepository.CampingPlaceRepository.GetPlaces(); 
                }
                PlaceInfo.Visibility = Visibility.Collapsed;
                field.Children.Clear();
                GenerateMap();
            }

        }
        public void HandleCancelAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            PlaceInfo.Visibility = Visibility.Collapsed;
            StaticUIMethods.ResetTextboxBorder(PlaceSurfaceArea);
            StaticUIMethods.ResetTextboxBorder(PlacePersons);
            StaticUIMethods.ResetTextboxBorder(PlacePricePerNight);
            StaticUIMethods.ResetTextboxBorder(PlacePlaceID);
            ResetComboBoxBorder(PlaceStreetComboBox);
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
        }
        private void GetAddStreetID()
        {
            if(PlaceStreetComboBox.SelectedItem != null)
            {
                Street street = _camping.CampingRepository.CampingMapRepository.GetSteetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                _placeStreetID = street.StreetID;
            }
            else
            {
                SetErrorComboBoxBorder(PlaceStreetComboBox);
                _wrongInput = true;
            }
        }
        //Function (EventHandler) that resets the background of a textbox if the filters are reset
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
        private void GetAddAmountOfPeople()
        {
            _placePersons = GetAddTextBox(PlacePersons, _placePersons);
        }
        private void PlaceStreetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            ExtendStreetPlaceButton.IsEnabled = true;
            if (PlaceStreetBorder.BorderBrush.Equals(Brushes.Red))
            {
                ResetComboBoxBorder(combobox);
                _wrongInput = false;
            }
        }
        public void SetErrorComboBoxBorder(ComboBox comboBox)
        {
            PlaceStreetBorder.BorderBrush = Brushes.Red;
            PlaceStreetBorder.BorderThickness = new Thickness(3, 3, 3, 3);
        }
        public void ResetComboBoxBorder(ComboBox comboBox)
        {
            PlaceStreetBorder.BorderBrush = Brushes.White;
            PlaceStreetBorder.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        private void ExtendStreetPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Street street = _camping.CampingRepository.CampingMapRepository.GetSteetByStreetName(PlaceStreetComboBox.Text);
            PlaceHasDogs.IsChecked = street.Dogs;
            PlaceHasPower.IsChecked = street.Power;
            PlacePersons.Text = street.AmountOfPeople.ToString();
            PlaceSurfaceArea.Text = street.SurfaceArea.ToString();
            PlacePricePerNight.Text = street.PricePerNightPerPerson.ToString();
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
    }
}