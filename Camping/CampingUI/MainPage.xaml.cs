using CampingCore;
using CampingDataAccess;
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
        public Camping Camping { get; set; }
        public List<Area> Areas { get; set; }
        public List<Street> Streets { get; set; }
        public List<Place> Places { get; set; }
        public int XPressed {  get; set; }
        public int YPressed { get; set; }
        public bool WrongInput { get; set; }
        public int SelectedPlace {  get; set; }
        private int _placeSurfaceArea, _placePersons, _placePlaceID;
        private double _placePricePerNight;
        private Canvas previousSelectedCanvas;

        public MainPage(Camping camping)
        {
            InitializeComponent();
            // Calculate scale percentages based on screen size
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double desiredWidth = 1000;  // Replace with your desired canvas width
            double desiredHeight = 750;  // Replace with your desired canvas height

            // Calculate scaling factors while maintaining the aspect ratio
            double aspectRatio = desiredWidth / desiredHeight;
            double screenAspectRatio = screenWidth / screenHeight;

            double scaleX = 1.0;
            double scaleY = 1.0;

            if (screenAspectRatio > aspectRatio)
            {
                // Screen is wider, scale based on width
                scaleX = screenWidth / desiredWidth / 1.75;
                scaleY = scaleX;
            }
            else
            {
                // Screen is taller, scale based on height
                scaleY = screenHeight / desiredHeight / 1.75;
                scaleX = scaleY;
            }

            ApplyScaleTransform(scaleX, scaleY);

            Camping = camping;

            DataContext = this;

            // Generate areas with their streets (and later places)
            GenerateAreas();
        }

        private void ApplyScaleTransform(double scaleX, double scaleY)
        {
            if (field.FindName("plattegrond") is ScaleTransform plattegrond)
            {
                plattegrond.ScaleX = scaleX;
                plattegrond.ScaleY = scaleY;
            }
        }


        public void GenerateAreas()
        {
            Areas = Camping.CampingRepository.CampingMapRepository.GetAreas();
            List<Color> availableColors = new List<Color>
            {
            ChangeColorOpacity(Colors.Red, 0.5),
            ChangeColorOpacity(Colors.ForestGreen, 0.5),
            ChangeColorOpacity(Colors.CornflowerBlue, 0.5),
            ChangeColorOpacity(Colors.Yellow, 0.5)
            };

            if (Areas.Count() > 0)
            {
                foreach (var area in Areas)
                {
                    var coordinates = area.GetAreaPositions();


                    Canvas canvasArea = new Canvas
                    {
                        Width = coordinates[2],
                        Height = coordinates[3],
                        Background = GenerateRandomBrush(availableColors),
                        Name = "Canvas_" + area.AreaID.ToString(),
                    };

                    Border border = new Border
                    {
                        BorderBrush = Brushes.Black, // Set the color of the border
                        BorderThickness = new Thickness(1), // Set the thickness of the border
                        Child = canvasArea
                    };

                    Canvas.SetTop(border, coordinates[1]);  // Ycord1 to place from top.
                    Canvas.SetLeft(border, coordinates[0]); // XCord1 to place from left.
                    Canvas.SetZIndex(border, -1);
                    field.Children.Add(border);


                    // Generate streets that belong to the area.
                    GenerateStreetsPerArea(canvasArea, area);
                }
            }
        }
        private void GenerateStreetsPerArea(Canvas canvasArea, Area area)
        {
            Streets = Camping.CampingRepository.CampingMapRepository.GetStreets();

            if (Streets.Count() > 0) {
                var areaStreets = Streets.Where(street => street.AreaID == area.AreaID);

                if(areaStreets.Count() > 0)
                {
                    foreach (var street in areaStreets)
                    {
                        GenerateStreet(canvasArea, street);
                        GeneratePlacesPerStreet(street);
                    }
                }
            }
        }
        public void GenerateStreet(Canvas canvasArea, Street street)
        {
            if (street != null)
            {
                var coordinates = street.GetStreetPositions();

                // A street cannot exist outside the area in width.
                if ((coordinates[0] + coordinates[2]) > canvasArea.Width)
                {
                    coordinates[2] = ((int)canvasArea.Width - coordinates[0]);
                }

                // A street cannot exist outside the area in height.
                if ((coordinates[1] + coordinates[3]) > canvasArea.Height)
                {
                    coordinates[3] = ((int)canvasArea.Height - coordinates[1]);
                }
                // Cannot be set outside the area.
                if (coordinates[0] < 0)
                {
                    coordinates[0] = 0;
                }

                // Cannot be set outside the area.
                if (coordinates[1] < 0)
                {
                    coordinates[1] = 0;
                }

                Rectangle square = new Rectangle
                {
                    Width = coordinates[2],
                    Height = coordinates[3],
                    Fill = Brushes.Black,
                };

                Canvas.SetTop(square, coordinates[1]);  // Ycord1 to place from top.
                Canvas.SetLeft(square, coordinates[0]); // XCord1 to place from left.

                canvasArea.Children.Add(square);
            }
        }
        public void GeneratePlacesPerStreet(Street street)
        {
            Places = Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            if (street != null && Places.Count() > 0)
            {
                foreach (var place in Places)
                {
                    if (place.StreetID == street.StreetID)
                    {
                        GeneratePlace(place, Brushes.Black, true);
                    }
                }
            }
        }
        public void GeneratePlace(Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            // Create a Border to wrap the Canvas
            Border border = new Border
            {
                BorderBrush = Brushes.White, // Set the border color
                BorderThickness = new Thickness(1), // Set the border thickness
            };

            Canvas canvasPlace = new Canvas
            {
                Width = 30,
                Height = 30,
                Background = brush,
                Name = "Place_" + place.PlaceID.ToString(),
            };

            // Add the Canvas to the Border
            border.Child = canvasPlace;
            Canvas.SetZIndex(canvasPlace, 100);

            Canvas.SetTop(border, coordinates[1]);  // Ycord1 to place from top.
            Canvas.SetLeft(border, coordinates[0]); // XCord1 to place from left.

            // Add the Border to the main canvas
            field.Children.Add(border);

            // Add PlaceID as text in the center of the Canvas
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
                SelectedPlace = place.PlaceID;

                PlacePlaceID.Text = place.PlaceID.ToString();
                PlacePlaceID.IsEnabled = false;

                PlaceHasPower.IsChecked = place.Power;
                PlaceHasDogs.IsChecked = place.Dogs;

                PlaceSurfaceArea.Text = place.SurfaceArea.ToString();
                PlacePricePerNight.Text = place.PricePerNightPerPerson.ToString();
                PlacePersons.Text = place.AmountOfPeople.ToString();
            }
            else
            {
                PlacePlaceID.Text = null;
                PlaceSurfaceArea.Text = null;
                PlacePersons.Text = null;
                PlacePricePerNight.Text = null;
            }
                
        }

        static Color ChangeColorOpacity(Color color, double opacity)
        {
            return Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B);
        }

        private Random random = new Random();

        public Brush GenerateRandomBrush(List<Color> colors)
        {
            
            if (colors.Count == 0)
            {
                throw new InvalidOperationException("No more colors available.");
            }
            Color selectedColor = colors[0];
            colors.RemoveAt(0);

            SolidColorBrush brush = new SolidColorBrush(selectedColor);
            return brush;
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            field.Children.Clear();
            GenerateAreas();

            Point p = Mouse.GetPosition(field);
            List<Area> areas = Camping.CampingRepository.CampingMapRepository.GetAreas();
            List<Street> streets = Camping.CampingRepository.CampingMapRepository.GetStreets();
            List<Place> places = Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            foreach(Place place in places)
            {
                place.Xcord -= 15;
                place.Ycord -= 15;
            }
            foreach(Street street in streets)
            {
                Area area = Camping.CampingRepository.CampingMapRepository.GetAreaByAreaID(street);
                street.Xcord1 += area.Xcord1; ;  
                street.Ycord1 += area.Ycord1; ;
            }
            //MessageBox.Show($"{p.X}, {p.Y}");
            XPressed = (int)Math.Round(p.X) - 15;
            YPressed = (int)Math.Round(p.Y) - 15;
            List<Area> PlaceWithinAreas = areas.Where(i => i.Xcord1 <= (XPressed - 15))
                                               .Where(i => i.Xcord1 + i.Xcord2 >= (XPressed + 45))
                                               .Where(i => i.Ycord1 <= (YPressed - 15))
                                               .Where(i => i.Ycord1 + i.Ycord2 >= (YPressed + 45))
                                               .ToList();
            List<Place> placesNotInNewPlaceBorder = places.Where(i => i.Xcord >= (XPressed-45) && i.Xcord <= (XPressed+45))
                                                          .Where(i => i.Ycord >= (YPressed-45) && i.Ycord <= (YPressed+45))
                                                          .ToList();
            //List<Street> StreetsCloseEnough = streets.Where(i => i.Xcord1 > XPressed && i.Ycord1 > YPressed).ToList();
            List<Street> StreetsCloseEnough = streets.Where(i => ((i.Xcord1 - 40) >= XPressed && (i.Xcord1 - 70) <= XPressed)
                                                              || ((i.Xcord1 + i.Xcord2 + 40) <= XPressed && (i.Xcord1 + i.Xcord2 + 70) >= XPressed)
                                                              || ((i.Ycord1 - 40) >= YPressed && (i.Ycord1 - 70) <= YPressed)
                                                              || ((i.Ycord1 + i.Ycord2 + 40) <= YPressed && (i.Ycord1 + i.Ycord2 + 70) >= YPressed))
                                                              .ToList();
            MessageBox.Show($"{StreetsCloseEnough.Count()}");
            //MessageBox.Show($"{streets[6].Xcord1-30} - {XPressed} = {streets[6].Xcord1-30 - XPressed}");
            if (PlaceWithinAreas.Count == 1 && placesNotInNewPlaceBorder.Count == 0 && StreetsCloseEnough.Count() > 0)  
            {
                Area area = PlaceWithinAreas[0];
                Camping.Places = Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                int i = Camping.Places.Last().PlaceID + 1;

                Place place = new Place(0, false, 1, false, 0, 0, 0, XPressed, YPressed);
                GeneratePlace(place, Brushes.Gray, false);
                HandlePlaceClick(place, true);

                //Camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                //Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
            }
        }
        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            GetAddNumbers();
            if (!WrongInput)
            {
                bool hasPower = false;
                if(PlaceHasPower.IsChecked == true)
                    hasPower = true;
                bool hasDogs = false;
                if (PlaceHasDogs.IsChecked == true)
                    hasDogs = true;
                Place place = new Place(_placePlaceID, hasPower, 1, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNight, XPressed, YPressed);
                Camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                Camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                PlaceInfo.Visibility = Visibility.Collapsed;

                field.Children.Clear();
                GenerateAreas();
            }

        }
        public void HandleCancelAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            PlaceInfo.Visibility = Visibility.Collapsed;

            field.Children.Clear();
            GenerateAreas();
        }
        private void GetAddNumbers()
        {
            GetAddAmountOfPeople();
            GetAddPricePerNightPerPerson();
            GetAddSurfaceArea();
            GetAddPlaceID();
        }

        //Function (EventHandler) that resets the background of a textbox if the filters are reset
        private void TextBox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (textbox.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetTextboxBorder(textbox);
                WrongInput = false;
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
                WrongInput = true;
            }
            return editNumber;
        }

        private void GetAddAmountOfPeople()
        {
            _placePersons = GetAddTextBox(PlacePersons, _placePersons);
        }

        private void GetAddSurfaceArea()
        {
            _placeSurfaceArea = GetAddTextBox(PlaceSurfaceArea, _placePersons);
        }
        private void GetAddPlaceID()
        {

            
            _placePlaceID = GetAddTextBox(PlacePlaceID, _placePlaceID);
            List<Place> places = Camping.Places.Where(i => i.PlaceID == _placePlaceID).ToList();
            if(places.Count > 0)
            {
                StaticUIMethods.SetErrorTextboxBorder(PlacePlaceID);
                WrongInput = true;
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
                WrongInput = true;
            }
        }
    }
}
