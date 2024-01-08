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
            MapPage.StreetName = GetAddComponentNameTextbox(MapPage.StreetNameTextbox, MapPage.StreetName);
            MapPage.StreetSurfaceArea = GetAddSurfaceArea(MapPage.StreetSurfaceAreaTextbox, MapPage.StreetSurfaceArea);
            MapPage.StreetAmountOfPeople = GetAddAmountOfPeople(MapPage.StreetAmountOfPeopleTextbox, MapPage.StreetAmountOfPeople);
            MapPage.StreetPricePerNightPerPerson = GetAddPricePerNightPerPerson(MapPage.StreetPricePerNightPerPersonTextbox, MapPage.StreetPricePerNightPerPerson);
        }
        public void CalculateStreetLineAngle(Line line)
        {
            MapPage.XCord2 = line.X2;
            MapPage.YCord2 = line.Y2;

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

            MapPage.XCord2 = line.X2;
            MapPage.YCord2 = line.Y2;
        }
        public void HandleStreetClick(Street street)
        {
            DeselectAllFields();
            MapPage.EditStreetBool = true;
            MapPage.SelectedPlace = null;
            MapPage.SelectedArea = null;
            MapPage.SelectedStreet = street;
            SetInfoVisible("Street");
            MapPage.StreetHasDogsCheckbox.IsChecked = street.Dogs;
            MapPage.StreetHasPowerCheckbox.IsChecked = street.Power;
            MapPage.StreetAmountOfPeopleTextbox.Text = street.AmountOfPeople.ToString();
            MapPage.StreetNameTextbox.Text = street.Name;
            MapPage.StreetPricePerNightPerPersonTextbox.Text = street.PricePerNightPerPerson.ToString();
            MapPage.StreetSurfaceAreaTextbox.Text = street.SurfaceArea.ToString();
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
                if (MapPage.SelectedMapButton.Equals("View"))
                {
                    MapPage.StreetOnMap.HandleStreetClick(street);
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
                    MapPage.XCord1 = line.X1;
                    MapPage.YCord1 = line.Y1;
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
                    MapPage.StreetOnMap.CalculateStreetLineAngle(line);
            }
        }
        public void FieldMouseDownStreet()
        {
            MapPage.EditStreetBool = false;
            if (MapPage.StreetPoint1.X == -1)
            {
                MapPage.StreetPoint1.X = -2;
                MapPage.StreetOnMap.SetFirstLinePoint();
            }
            else
            {
                MapPage.StreetOnMap.SetSecondLinePoint();
                MapPage.StreetOnMap.SetInfoVisible("Street");
                MapPage.StreetOnMap.ResetInputs(MapPage.StreetInfoGrid);
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
