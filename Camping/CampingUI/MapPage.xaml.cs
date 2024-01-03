using CampingCore;
using CampingUI.GenerateComponentsMap;
using CampingUI.Map;
using CampingUI.Map.AreaMap;
using CampingUI.Map.PlaceMap;
using CampingUI.Map.StreetMap;
using CampingUI.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Transform = CampingUI.NewFolder.Transform;

namespace CampingUI
{
    public partial class MapPage : Page
    {
        public Camping _camping;
        public List<Area> _areas;
        public List<Place> _places;
        public List<Street> _streets;
        public int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID, _placeAreaID, _yPressed, _xPressed;
        public Canvas SelectedPlace;
        public int _streetSurfaceArea, _streetPersons;
        public int _areaSurfaceArea, _areaPersons;
        public double _placePricePerNightPerPerson, _streetPricePerNightPerPerson, _areaPricePerNightPerPerson, _xCord1, _yCord1, _xCord2, _yCord2;
        public string _streetName, _areaName;
        public Canvas _previousSelectedCanvas;
        public bool _editPlaceBool, _editStreetBool, _wrongInput;
        public string _selectedMapButton = "View";
        public Point _streetPoint1 = new Point(-1, -1);
        public Point _areaStartPoint = new Point(-1, -1);
        public Border _newArea;
        public Area SelectedArea;
        public Street SelectedStreet;
        public PlaceOnMap _placeOnMap;
        public StreetOnMap _streetOnMap;
        public AreaOnMap _areaOnMap;
        public MapMethods _mapMethods;

        public MapPage(Camping camping)
        {
            InitializeComponent();
            _camping = camping;
            _placeOnMap = new PlaceOnMap(this, camping);
            _streetOnMap = new StreetOnMap(this, camping);
            _areaOnMap = new AreaOnMap(this, camping);
            _mapMethods = new MapMethods(this, camping);
            new Transform(field);
            _mapMethods.GenerateMap();

            Loaded += (sender, e) =>
            {
                Focusable = true;
                Keyboard.Focus(this);
            };
            KeyDown += Handle_KeyDown;     
        }
        private void field_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                Place place = new Place(1000000000, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                _placeOnMap.GeneratePreviewPlace(place, Brushes.Aquamarine);
            }
            else if (_selectedMapButton.Contains("Street"))
            {
                Line line1 = null;
                bool canMakeLine = true;
                foreach (var component in field.Children)
                {
                    if (component is Line line)
                    {
                        if (line.Name.Equals("firstPoint")) line1 = line;
                        if (line.Name.Equals("LineSet")) canMakeLine = false;
                    }
                }
                if (line1 == null && canMakeLine)
                    _streetOnMap.GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
            }
        }
        private void field_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                foreach (var component in field.Children)
                {
                    if (component is Border border && border.Name.Equals("Place_1000000000"))
                    {
                        Point p = Mouse.GetPosition(field);
                        if (border != null)
                        {
                            Canvas.SetLeft(border, p.X - 15);
                            Canvas.SetTop(border, p.Y - 15);
                        }
                        break;
                    }
                }
            }
            else if (_selectedMapButton.Contains("Street"))
            {
                Point p = Mouse.GetPosition(field);
                foreach (var component in field.Children)
                {
                    if (component is Line line && line.Name.Equals("MoveablePoint") && !line.Name.Equals("firstPoint"))
                    {
                        double centerX = line.X1 + (line.X2 - line.X1) / 2;
                        double centerY = line.Y1 + (line.Y2 - line.Y1) / 2;

                        double offsetX = p.X - centerX;
                        double offsetY = p.Y - centerY;

                        line.X1 += offsetX;
                        line.Y1 += offsetY;
                        line.X2 += offsetX;
                        line.Y2 += offsetY;
                    }
                    else if (component is Line firstPoint && firstPoint.Name.Equals("firstPoint"))
                    {
                        firstPoint.X2 = p.X + 7.5;
                        firstPoint.Y2 = p.Y - 3.75;
                    }
                }
            }
        }
        private void field_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                foreach (var component in field.Children)
                    if (component is Border border && border.Name.Equals("Place_1000000000"))
                    {
                        field.Children.Remove(border);
                        break;
                    }
            }
        }
        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                _placeOnMap.RemoveOldPreviewPlace();
                _placeOnMap.GenerateNewPlaceWithInfoBoxes();
            }
            else if (_selectedMapButton.Contains("Street"))
            {
                _streetOnMap.FieldMouseDownStreet();
            }
            else if (_selectedMapButton.Contains("Area"))
            {
                _areaOnMap.FieldMouseDownArea();
            }
        }
        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (PlaceInfoGrid.Visibility.Equals(Visibility.Hidden) && StreetInfoGrid.Visibility.Equals(Visibility.Hidden) && AreaInfoGrid.Visibility.Equals(Visibility.Hidden))
                {
                    _selectedMapButton = "View";
                    foreach (Button gridButton in MapGridButtons.Children)
                    {
                        Style selectionStyle = (Style)gridButton.FindResource("ButtonStyle1Selection");
                        gridButton.Style = selectionStyle;
                    }
                }
                else _mapMethods.HideInfoGrids();
                _streetPoint1 = new Point(-1, -1);
                _areaStartPoint = new Point(-1, -1);
                _mapMethods.GenerateMap();
                if (_selectedMapButton.Contains("Place"))
                {
                    Point p = Mouse.GetPosition(field);
                    _xPressed = (int)p.X;
                    _yPressed = (int)p.Y;

                    Place place = new Place(1000000000, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                    _placeOnMap.GeneratePreviewPlace(place, Brushes.Aquamarine);
                }
                if (_selectedMapButton.Contains("Street"))
                    _streetOnMap.GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
            }
        }
        private void MakeMapComponentButton_Click(object sender, RoutedEventArgs e)
        {
            _placeOnMap.HideInfoGrids();
            Button button = (Button)sender;
            Style applyStyle = (Style)button.FindResource("ButtonStyle1Apply");
            Style selectionStyle = (Style)button.FindResource("ButtonStyle1Selection");
            if (button.Style.Equals(selectionStyle))
            {
                foreach (Button gridButton in MapGridButtons.Children)
                    gridButton.Style = selectionStyle;

                button.Style = applyStyle;
                _selectedMapButton = button.Name;
            }
            else
            {
                button.Style = selectionStyle;
                _mapMethods.GenerateMap();
                _selectedMapButton = "View";
            }
        }
        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            _placeOnMap.GetAddPlaceValues();
            if (!_wrongInput)
            {
                bool hasPower = false;
                if (PlaceHasPower.IsChecked == true)
                    hasPower = true;
                bool hasDogs = false;
                if (PlaceHasDogs.IsChecked == true)
                    hasDogs = true;
                Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
                Area area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());

                if (_editPlaceBool)
                {
                    _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(Int32.Parse(PlacePlaceID.Text), street.StreetID, area.AreaID, hasPower, _placeSurfaceArea, _placePricePerNightPerPerson, _placePersons, hasDogs);
                    _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceDataExtending(Int32.Parse(PlacePlaceID.Text),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePersonsComboBox));
                    _placeOnMap.ResetAfterAddingMapComponent("Place");
                    return;
                }
                Place place = new Place(_placePlaceID, hasPower, street.StreetID, area.AreaID, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNightPerPerson, _xPressed, _yPressed);

                if (!_editPlaceBool)
                {
                    _camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    _camping.CampingRepository.CampingMapRepository.AddExtend(place.PlaceID,
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          _placeOnMap.GetValueFromExtendComboBox(PlacePersonsComboBox));
                    _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                }
                _placeOnMap.ResetInputFields("Place");
                _placeOnMap.ResetAfterAddingMapComponent("Place");
            }
        }

        private void HandleAddStreet_Click(Object sender, RoutedEventArgs e)
        {
            _streetOnMap.GetAddStreetValues();
            if (!_wrongInput)
            {
                bool hasPower = (bool)StreetHasPower.IsChecked;
                bool hasDogs = (bool)StreetHasDogs.IsChecked;
                if (_editStreetBool)
                {
                    _camping.CampingRepository.CampingMapRepository.UpdateStreetByStreetID(_streetName, hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, SelectedStreet.StreetID);
                    Street street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(SelectedStreet.StreetID);
                    List<Place> streetPlaces = _places.Where(p => p.StreetID == street.StreetID).ToList();
                    foreach (Place place in streetPlaces)
                    {
                        bool placeExtendPower = place.Power;
                        bool placeExtendDogs = place.Dogs;
                        int placeExtendSurfaceArea = place.SurfaceArea;
                        double placeExtendPricePerNightPerPerson = place.PricePerNightPerPerson;
                        int placeExtendAmountOfPeople = place.AmountOfPeople;

                        List<bool?> placeExtend = _camping.CampingRepository.CampingPlaceRepository.GetPlaceExtendingByPlaceID(place.PlaceID);
                        if (placeExtend[0] == true) placeExtendPower = true;
                        if (placeExtend[1] == true) placeExtendDogs = true;
                        if (placeExtend[2] == true) placeExtendSurfaceArea = street.SurfaceArea;
                        if (placeExtend[3] == true) placeExtendPricePerNightPerPerson = street.PricePerNightPerPerson;
                        if (placeExtend[4] == true) placeExtendAmountOfPeople = street.AmountOfPeople;
                        _camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place.PlaceID,
                                                                                          place.StreetID,
                                                                                          place.AreaID,
                                                                                          placeExtendPower,
                                                                                          placeExtendSurfaceArea,
                                                                                          placeExtendPricePerNightPerPerson,
                                                                                          placeExtendAmountOfPeople,
                                                                                          placeExtendDogs);
                    }
                }
                else
                {
                    _camping.CampingRepository.CampingMapRepository.AddNewStreet(_streetName, hasPower, hasDogs, _streetSurfaceArea, _streetPricePerNightPerPerson, _streetPersons, (int)_xCord1, (int)_yCord1, (int)_xCord2, (int)_yCord2);
                    _streetPoint1 = new Point(-1, -1);
                }
                _streetOnMap.ResetAfterAddingMapComponent("Street");
                _mapMethods.GenerateMap();
            }
        }
        private void SelectedArea_ColorChange(object sender, SelectionChangedEventArgs e)
        {
            if (_newArea != null)
            {
                Canvas usedCanvas = (Canvas)_newArea.Child;
                if (AreaColor.SelectedItem != null)
                {
                    int color = int.Parse(AreaColor.SelectedValue.ToString());
                    usedCanvas.Background = new SolidColorBrush(StaticUIMethods.GetColorFromInt(color));
                }
            }
        }
        public void HandleCancelAddComponent_Click(Object sender, RoutedEventArgs e)
        {
            _placeOnMap.ResetAfterAddingMapComponent("Place");
            _streetOnMap.ResetAfterAddingMapComponent("Street");
            _mapMethods.HideInfoGrids();
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
        private void HandleCancelAddArea_Click(object sender, RoutedEventArgs e)
        {
            _areaStartPoint.X = -1;
            _areaStartPoint.Y = -1;
            SelectedArea = null;
            _newArea = null;
            _mapMethods.GenerateMap();
            AreaInfoGrid.Visibility = Visibility.Hidden;
        }
        private void HandleAddArea_Click(object sender, RoutedEventArgs e)
        {
            _areaOnMap.ResetBorders(AreaInfoGrid);
            _areaOnMap.GetAddAreaValues();
            if(!_wrongInput)
            {
                _camping.CampingRepository.CampingMapRepository.AddNewArea(SelectedArea);
                AreaInfoGrid.Visibility = Visibility.Hidden;
                _areaStartPoint.X = -1;
                _areaStartPoint.Y = -1;
                _mapMethods.GenerateMap();
            }
        }
        private void PlaceStreetAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            Border border = PlaceAreaBorder;
            if (combobox.Name.Equals(PlaceStreetComboBox.Name)) border = PlaceStreetBorder;
            if (border.BorderBrush.Equals(Brushes.Red))
            {
                StaticUIMethods.ResetComboBoxBorder(border);
                _wrongInput = false;
            }
            _placeOnMap.EnableExtendComboBoxes(false);
            if (PlaceStreetComboBox.SelectedItem != null && PlaceAreaComboBox.SelectedItem != null)
            {
                _placeOnMap.EnableExtendComboBoxes(true);

                foreach (Grid grid in PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                        if (component is ComboBox com)
                            if (com.Name != PlaceStreetComboBox.Name && com.Name != PlaceAreaComboBox.Name)
                                _placeOnMap.HandleExtending(com);
                }
            }
        }
        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (_placeOnMap != null)
            {
                _placeOnMap.HandleExtending(comboBox);
            }
        }
    }
}
