using CampingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CampingUI.Map.StreetMap
{
    public class StreetOnMap : MapMethods
    {
        public StreetOnMap(MapPage mapPage, Camping camping) : base(mapPage, camping) { }

        public void GetAddStreetValues()
        {
            MapPage._streetName = GetAddComponentNameTextbox(MapPage.StreetName, MapPage._streetName);
            MapPage._streetSurfaceArea = GetAddSurfaceArea(MapPage.StreetSurfaceArea, MapPage._streetSurfaceArea);
            MapPage._streetPersons = GetAddAmountOfPeople(MapPage.StreetPersons, MapPage._streetPersons);
            MapPage._streetPricePerNightPerPerson = GetAddPricePerNightPerPerson(MapPage.StreetPricePerNight, MapPage._streetPricePerNightPerPerson);
        }
        public void CalculateStreetLineAngle(Line line)
        {
            MapPage._xCord2 = line.X2;
            MapPage._yCord2 = line.Y2;

            line.Stroke = Brushes.DarkGray;
            line.Name = "LineSet";

            double deltaY;
            double deltaX;
            if (line.X1 > line.X2) deltaX = line.X1 - line.X2;
            else deltaX = line.X2 - line.X1;

            if (line.Y1 > line.Y2) deltaY = line.Y1 - line.Y2;
            else deltaY = line.Y2 - line.Y1;

            double degrees = Math.Atan(deltaY / deltaX) * 180 / Math.PI;
            if (degrees < 7) line.Y2 = line.Y1;
            if (degrees > 83) line.X2 = line.X1;

            MapPage._xCord2 = line.X2;
            MapPage._yCord2 = line.Y2;
        }
        public void HandleStreetClick(Street street)
        {
            DeselectAllFields();
            MapPage._editStreetBool = true;
            MapPage.SelectedPlace = null;
            MapPage.SelectedArea = null;
            MapPage.SelectedStreet = street;
            SetInfoVisible("Street");
            MapPage.StreetHasDogs.IsChecked = street.Dogs;
            MapPage.StreetHasPower.IsChecked = street.Power;
            MapPage.StreetPersons.Text = street.AmountOfPeople.ToString();
            MapPage.StreetName.Text = street.Name;
            MapPage.StreetPricePerNight.Text = street.PricePerNightPerPerson.ToString();
            MapPage.StreetSurfaceArea.Text = street.SurfaceArea.ToString();
            foreach (Grid grid in MapPage.StreetInfoGrid.Children)
            {
                foreach (var comp in grid.Children)
                {
                    if (comp is TextBox textbox) textbox.IsEnabled = true;
                    if (comp is CheckBox checkbox) checkbox.IsEnabled = true;
                }
            }
            HighLightPlaces(street, Brushes.DarkCyan);
        }
        public void SetStreetEvents(Line line, Street street)
        {
            line.MouseLeftButtonDown += (sender, e) =>
            {
                if (MapPage._selectedMapButton.Equals("View"))
                {
                    MapPage._streetOnMap.HandleStreetClick(street);
                    line.Stroke = Brushes.DarkCyan;
                }
            };

            line.MouseEnter += (sender, e) => { if (MapPage.SelectedStreet == null || (MapPage.SelectedStreet != null && !MapPage.SelectedStreet.Equals(street))) line.Stroke = Brushes.DarkCyan; };
            line.MouseLeave += (sender, e) =>
            {
                if (MapPage.SelectedStreet == null || (MapPage.SelectedStreet != null && !MapPage.SelectedStreet.Equals(street))) line.Stroke = Brushes.Black;
            };
        }

        public void SetFirstLinePoint()
        {
            foreach (var component in MapPage.field.Children)
            {
                if (component is Line line && line.Name.Equals("MoveablePoint"))
                {
                    MapPage._xCord1 = line.X1;
                    MapPage._yCord1 = line.Y1;
                    line.Name = "firstPoint";
                }
            }
        }

        public void SetSecondLinePoint()
        {
            foreach (var component in MapPage.field.Children)
            {
                if (component is Line createdLine && createdLine.Name.Equals("LineSet"))
                    createdLine.Name = "firstPoint";
                if (component is Line line && line.Name.Equals("firstPoint"))
                    MapPage._streetOnMap.CalculateStreetLineAngle(line);
            }
        }
        public void FieldMouseDownStreet()
        {
            MapPage._editStreetBool = false;
            if (MapPage._streetPoint1.X == -1)
            {
                MapPage._streetPoint1.X = -2;
                MapPage._streetOnMap.SetFirstLinePoint();
            }
            else
            {
                MapPage._streetOnMap.SetSecondLinePoint();
                MapPage._streetOnMap.SetInfoVisible("Street");
                MapPage._streetOnMap.ResetInputs(MapPage.StreetInfoGrid);
            }
        }
        public void GeneratePreviewLine(string name, SolidColorBrush color)
        {
            Point p = Mouse.GetPosition(MapPage.field);
            Line line = new Line();
            line.X1 = p.X - 7.5;
            line.Y1 = p.Y - 3.75;
            line.X2 = p.X + 7.5;
            line.Y2 = p.Y - 3.75;
            line.StrokeThickness = 15;
            line.Stroke = color;
            line.Name = name;
            MapPage.field.Children.Add(line);
        }
    }
}
