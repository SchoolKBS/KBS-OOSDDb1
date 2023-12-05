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

        public MainPage(Camping camping)
        {
            InitializeComponent();
            Camping = camping;

            // Generate areas with there streets (and later places)
            GenerateAreas();
          
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
                    if (place.StreetID == street.StreetID) {
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

                        // Center the Canvas within the Border
                        Canvas.SetTop(canvasPlace, (border.ActualHeight - canvasPlace.Height) / 2);
                        Canvas.SetLeft(canvasPlace, (border.ActualWidth - canvasPlace.Width) / 2);

                        // Add hover effect
                        canvasPlace.MouseEnter += (sender, e) =>
                        {
                            canvasPlace.Background = Brushes.Gray; // Change the background color on hover
                        };

                        canvasPlace.MouseLeave += (sender, e) =>
                        {
                            canvasPlace.Background = Brushes.Black; // Change back to the original color on leave
                        };

                    }
                }
            }
        }




        private List<Color> availableColors = new List<Color>
        {
            ChangeColorOpacity(Colors.Red, 0.5),
            ChangeColorOpacity(Colors.ForestGreen, 0.5),
            ChangeColorOpacity(Colors.CornflowerBlue, 0.5),
            ChangeColorOpacity(Colors.Yellow, 0.5)
        };

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
