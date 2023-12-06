using CampingCore;
using CampingDataAccess;
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

        public int SelectedPlace {  get; set; }
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
            Areas = Camping.CampingRepository.GetAreas();

            foreach (var area in Areas)
            {
                var coordinates = area.GetAreaPositions();


                Canvas canvasArea = new Canvas
                {
                    Width = coordinates[2],
                    Height = coordinates[3],
                    Background = GenerateRandomBrush(),
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


        private void GenerateStreetsPerArea(Canvas canvasArea, Area area)
        {
            Streets = Camping.CampingRepository.GetStreets();

            var areaStreets = Streets.Where(street => street.AreaID == area.AreaID);

            foreach (var street in areaStreets)
            {
                GenerateStreet(canvasArea, street);
                GeneratePlacesPerStreet(street);
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
            Places = Camping.CampingRepository.GetPlaces();
            if (street != null)
            {
                foreach (var place in Places)
                {
                    if (place.StreetID == street.StreetID)
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
                            Background = Brushes.Black,
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

                        // Center the Canvas within the Border
                        Canvas.SetTop(canvasPlace, (border.ActualHeight - canvasPlace.Height) / 2);
                        Canvas.SetLeft(canvasPlace, (border.ActualWidth - canvasPlace.Width) / 2);

                        canvasPlace.MouseEnter += (sender, e) =>
                        {
                            canvasPlace.Background = Brushes.DarkCyan; // Change the background color on hover
                        };

                        canvasPlace.MouseLeave += (sender, e) =>
                        {
                            if(place.PlaceID != SelectedPlace)
                            {
                                canvasPlace.Background = Brushes.Black;
                            }
                        };

                        canvasPlace.MouseLeftButtonDown += (sender, e) =>
                        {
                            if (previousSelectedCanvas != null)
                            {
                                // Change the background color of the previously selected canvas back to black
                                previousSelectedCanvas.Background = Brushes.Black;
                            }

                            // Set the background color of the current canvas to red
                            canvasPlace.Background = Brushes.DarkCyan;

                            // Update the reference to the current canvas
                            previousSelectedCanvas = canvasPlace;

                            // Handle the click event
                            HandlePlaceClick(place);
                        };
                    }
                }
            }
        }

        public void HandlePlaceClick(Place place)
        {
            SelectedPlace = place.PlaceID;
            PlaceIDTextBlock.Text = $"Plaats {place.PlaceID.ToString()}";
            PlaceHasPowerLabel.Visibility = Visibility.Visible;

            PlaceInfo.Visibility = Visibility.Visible;

            PlaceHasPower.IsChecked = place.Power;
            PlaceHasDogs.IsChecked = place.Dogs;
              
            PlaceSurfaceArea.Text = place.SurfaceArea.ToString();
            PlacePricePerNight.Text = place.PricePerNightPerPerson.ToString();
            PlacePersons.Text = place.AmountOfPeople.ToString();
                
        }

        private List<Color> availableColors = new List<Color>
        {
            ChangeColorOpacity(Colors.Red, 0.5),
            ChangeColorOpacity(Colors.ForestGreen, 0.5),
            ChangeColorOpacity(Colors.CornflowerBlue, 0.5),
            ChangeColorOpacity(Colors.Yellow, 0.5)
        };


        public void HandleEditPlace_Click(Object sender, RoutedEventArgs e)
        {
            if(PlaceRow1.IsEnabled == false)
            {
                PlaceRow1.IsEnabled = true;
                PlaceRow2.IsEnabled = true;
                PlaceRow3.IsEnabled = true;
                PlaceRow4.IsEnabled = true;
                PlaceRow5.IsEnabled = true;

                EditButtonPlace.Content = "Opslaan";
            } else
            {
                PlaceRow1.IsEnabled = false;
                PlaceRow2.IsEnabled = false;
                PlaceRow3.IsEnabled = false;
                PlaceRow4.IsEnabled = false;
                PlaceRow5.IsEnabled = false;

                EditButtonPlace.Content = "Aanpassen";
            }
        }

        static Color ChangeColorOpacity(Color color, double opacity)
        {
            return Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B);
        }

        private Random random = new Random();

        public Brush GenerateRandomBrush()
        {
            if (availableColors.Count == 0)
            {
                throw new InvalidOperationException("No more colors available.");
            }

            int colorIndex = random.Next(availableColors.Count);
            Color selectedColor = availableColors[colorIndex];
            availableColors.RemoveAt(colorIndex);

            SolidColorBrush brush = new SolidColorBrush(selectedColor);
            return brush;
        }
    }
}
