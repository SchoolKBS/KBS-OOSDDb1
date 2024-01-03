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
            _streetName = GetAddComponentNameTextbox(StreetName, _streetName);
            _streetSurfaceArea = GetAddSurfaceArea(StreetSurfaceArea, _streetSurfaceArea);
            _streetPersons = GetAddAmountOfPeople(StreetPersons, _streetPersons);
            _streetPricePerNightPerPerson = GetAddPricePerNightPerPerson(StreetPricePerNight, _streetPricePerNightPerPerson);
        }
        public void CalculateStreetLineAngle(Line line)
        {
            _xCord2 = line.X2;
            _yCord2 = line.Y2;

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

            _xCord2 = line.X2;
            _yCord2 = line.Y2;
        }
        public void HandleStreetClick(Street street)
        {
            _streetOnMap.DeselectAllFields();
            _editStreetBool = true;
            SelectedPlace = null;
            SelectedArea = null;
            SelectedStreet = street;
            _streetOnMap.SetInfoVisible("Street");
            StreetHasDogs.IsChecked = street.Dogs;
            StreetHasPower.IsChecked = street.Power;
            StreetPersons.Text = street.AmountOfPeople.ToString();
            StreetName.Text = street.Name;
            StreetPricePerNight.Text = street.PricePerNightPerPerson.ToString();
            StreetSurfaceArea.Text = street.SurfaceArea.ToString();
            foreach (Grid grid in StreetInfoGrid.Children)
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
                if (_selectedMapButton.Equals("View"))
                {
                    _streetOnMap.HandleStreetClick(street);
                    line.Stroke = Brushes.DarkCyan;
                }
            };

            line.MouseEnter += (sender, e) => { if (SelectedStreet == null || (SelectedStreet != null && !SelectedStreet.Equals(street))) line.Stroke = Brushes.DarkCyan; };
            line.MouseLeave += (sender, e) =>
            {
                if (SelectedStreet == null || (SelectedStreet != null && !SelectedStreet.Equals(street))) line.Stroke = Brushes.Black;
            };
        }

        public void SetFirstLinePoint()
        {
            foreach (var component in field.Children)
            {
                if (component is Line line && line.Name.Equals("MoveablePoint"))
                {
                    _xCord1 = line.X1;
                    _yCord1 = line.Y1;
                    line.Name = "firstPoint";
                }
            }
        }

        public void SetSecondLinePoint()
        {
            foreach (var component in field.Children)
            {
                if (component is Line createdLine && createdLine.Name.Equals("LineSet"))
                    createdLine.Name = "firstPoint";
                if (component is Line line && line.Name.Equals("firstPoint"))
                    _streetOnMap.CalculateStreetLineAngle(line);
            }
        }
        public void FieldMouseDownStreet()
        {
            _editStreetBool = false;
            if (_streetPoint1.X == -1)
            {
                _streetPoint1.X = -2;
                _streetOnMap.SetFirstLinePoint();
            }
            else
            {
                _streetOnMap.SetSecondLinePoint();
                _streetOnMap.SetInfoVisible("Street");
                _streetOnMap.ResetInputs(StreetInfoGrid);
            }
        }
        public void GeneratePreviewLine(string name, SolidColorBrush color)
        {
            Point p = Mouse.GetPosition(field);
            Line line = new Line();
            line.X1 = p.X - 7.5;
            line.Y1 = p.Y - 3.75;
            line.X2 = p.X + 7.5;
            line.Y2 = p.Y - 3.75;
            line.StrokeThickness = 15;
            line.Stroke = color;
            line.Name = name;
            field.Children.Add(line);
        }
    }
}
