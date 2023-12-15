using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CampingUI.GenerateComponentsMap
{
    public class MapPagePlace
    {
        /*private static Border _border;
        private Place _selectedPlace;

        public Border GeneratePlace(Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            Border border = CreateNewBorder();
            Canvas canvasPlace = CreateCanvasPlace(place, brush);
            border.Child = canvasPlace;
            Canvas.SetZIndex(canvasPlace, 100);
            Canvas.SetTop(border, coordinates[1]); 
            Canvas.SetLeft(border, coordinates[0]);

            _border = border;
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

            if (brush == Brushes.Black)
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
            return border;
        }

        private static Canvas CreateCanvasPlace(Place place, SolidColorBrush brush)
        {
            return new Canvas
            {
                Width = 30,
                Height = 30,
                Background = brush,
                Name = "Place_" + place.PlaceID.ToString(),
            };
        }

        private static Border CreateNewBorder()
        {
            return new Border
            {
                BorderBrush = Brushes.White, // Set the border color
                BorderThickness = new Thickness(1), // Set the border thickness
            };
        }
        public static Border getBorder()
        {
            return _border;
        }*/
    }
}
