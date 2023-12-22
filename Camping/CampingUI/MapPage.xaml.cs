using CampingCore;
using CampingDataAccess;
using CampingUI.GenerateComponentsMap;
using CampingUI.NewFolder;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Windows.Themes;
using Org.BouncyCastle.Asn1.BC;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
    /// </summary>
    public partial class MapPage : Page
    {
        private Camping _camping;
        private List<Area> _areas;
        private List<Place> _places;
        private List<Street> _streets;
        private int _placeSurfaceArea, _placePersons, _placePlaceID, _placeStreetID, _placeAreaID, _yPressed, _xPressed;
        private Canvas SelectedPlace;
        private int _streetSurfaceArea, _streetPersons;
        private int _areaSurfaceArea, _areaPersons;
        private double _placePricePerNightPerPerson, _streetPricePerNightPerPerson, _areaPricePerNightPerPerson, _xCord1, _yCord1, _xCord2, _yCord2;
        private string _streetName, _areaName;
        private Canvas _previousSelectedCanvas;
        private bool _editPlaceBool, _editStreetBool, _wrongInput;
        private string _selectedMapButton = "View";
        private Point _streetPoint1 = new Point(-1, -1);
        private Point _areaStartPoint = new Point(-1, -1);
        private Border _newArea;
        public Area SelectedArea { get; private set; }
        public Street SelectedStreet;
        public MapPage(Camping camping)
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
            KeyDown += Handle_KeyDown;     
        }

        public void GenerateMap()
        {
            field.Children.Clear();
            _areas = _camping.CampingRepository.CampingMapRepository.GetAreas().ToList();
            _streets = _camping.CampingRepository.CampingMapRepository.GetStreets().ToList();
            _places = _camping.CampingRepository.CampingPlaceRepository.GetPlaces().ToList();
            GenerateComponentsMap(_areas);
            GenerateComponentsMap(_streets);
            GenerateComponentsMap(_places);
        }
        public void GenerateComponentsMap<T>(List<T> list)
        {
            if(list != null && list.Count() > 0)
            {
                foreach (var comp in list)
                {
                   if(comp is Area)
                    {
                        Border border = MapPageArea.GenerateArea((Area)(object)(comp));                      
                        field.Children.Add(border);
                        SetAreaEvents(border, (Area)(object)comp);
                    }
                    if (comp is Street street )
                    {
                        MapPageStreet.GenerateStreet(street, Brushes.Black);
                        Line line = MapPageStreet.GetLine();
                        field.Children.Add(line);
                        field.Children.Add(MapPageStreet.GetTextBlock());
                        SetStreetEvents(line, street);
                    }                      
                    if (comp is Place)
                        GeneratePlace((Place)(object)comp, Brushes.Black, true);
                }
            }
        }

        private void SetAreaEvents(Border border, Area area)
        {
            border.MouseLeftButtonDown += (sender, e) =>
            {
                DeselectAllFields();
                if (_selectedMapButton.Contains("View"))
                {
                    SelectedArea = area;
                    SelectedStreet = null;
                    SelectedPlace = null;
                    ToggleAreaInput(false);
                    HandleAreaClick();
                    border.BorderBrush = Brushes.LightCyan;
                    border.BorderThickness = new Thickness(4);
                    HighLightPlaces(area, Brushes.DarkCyan);
                }
            };
        }
        private void SetStreetEvents(Line line, Street street )
        {
            line.MouseLeftButtonDown += (sender, e) =>
            {
                if (_selectedMapButton.Equals("View"))
                {
                    HandleStreetClick(street);
                    line.Stroke = Brushes.DarkCyan;
                }
            };

            line.MouseEnter += (sender, e) => { if (SelectedStreet == null || (SelectedStreet != null && !SelectedStreet.Equals(street))) line.Stroke = Brushes.DarkCyan; };
            line.MouseLeave += (sender, e) =>
            {
                if (SelectedStreet == null || (SelectedStreet != null && !SelectedStreet.Equals(street))) line.Stroke = Brushes.Black;
            };
        }
       
        public Border GeneratePlace(Place place, SolidColorBrush brush, bool AddPlaceBool)
        {
            var coordinates = place.GetPlacePositions();

            Border border = new Border
            {
                BorderBrush = Brushes.White,
                Width = 30,
                Height = 30,
                BorderThickness = new Thickness(1),
                Name = "Place_" + place.PlaceID.ToString(),
            };

            Canvas canvasPlace = new Canvas
            {
                Background = brush,
                Name = "Place" + place.PlaceID.ToString(),
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

            if (brush == Brushes.Black)
            {
                canvasPlace.MouseEnter += (sender, e) =>
                {
                    if (_selectedMapButton.Contains("View")) canvasPlace.Background = Brushes.DarkCyan; 
                };

                canvasPlace.MouseLeave += (sender, e) =>
                {
                    if (!((SelectedPlace != null && SelectedPlace.Name.Equals(canvasPlace.Name)) 
                       || (SelectedArea != null && SelectedArea.AreaID == place.AreaID)
                       || (SelectedStreet != null && SelectedStreet.StreetID == place.StreetID)))
                        canvasPlace.Background = Brushes.Black;
                };
            }

            canvasPlace.MouseLeftButtonDown += (sender, e) =>
            {

                if (_selectedMapButton.Contains("View"))
                {
                    DeselectAllFields();
                    GenerateMap();
                    canvasPlace.Background = Brushes.DarkCyan;
                    _previousSelectedCanvas = canvasPlace;
                    SelectedStreet = null;
                    SelectedArea = null;
                    SelectedPlace = canvasPlace;
                    HandlePlaceClick(place, false);
                }
            };
            return border;
        }
        public void HandlePlaceClick(Place place, bool AddPlaceBool)
        {
            PlaceInfo.Visibility = Visibility.Visible;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Hidden;
            PlaceInfoGrid.Visibility = Visibility.Visible;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Hidden;
            ResetInputFields("Place");
            _editPlaceBool = false;
            if (!AddPlaceBool)
            {
                _editPlaceBool = true;
                SetPlaceDataOnFields(place);
                SetPlaceDataExtending(place);
            }
        }

        private void SetPlaceDataExtending(Place place)
        {
            List<bool?> placeExtend = _camping.CampingRepository.CampingPlaceRepository.GetPlaceExtendingByPlaceID(place.PlaceID);
            int counter = 0;
            foreach (Grid grid in PlaceInfoGrid.Children)
            {
                foreach (var  comp in grid.Children)
                {
                    if (comp is ComboBox comboBox && !comboBox.Name.Equals("PlaceStreetComboBox") && !comboBox.Name.Equals("PlaceAreaComboBox"))
                    {
                        if (placeExtend[counter].HasValue)
                        {
                            if (placeExtend[counter] == true) comboBox.SelectedIndex = 0;
                            else comboBox.SelectedIndex = 1;
                        }
                        else comboBox.SelectedIndex = 2;
                        counter++;
                       
                    }
                }
                
            }

        }

        private void SetPlaceDataOnFields(Place place)
        {
            PlaceStreetComboBox.Text = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetID(place).Name;
            PlaceAreaComboBox.Text = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaID(place).Name;
            PlacePlaceID.IsEnabled = false;
            PlacePlaceID.Text = place.PlaceID.ToString();
            PlaceHasPower.IsChecked = place.Power;
            PlaceHasDogs.IsChecked = place.Dogs;
            PlaceSurfaceArea.Text = place.SurfaceArea.ToString();
            PlacePricePerNight.Text = place.PricePerNightPerPerson.ToString();
            PlacePersons.Text = place.AmountOfPeople.ToString();
        }
        public void HandleStreetClick(Street street)
        {
            DeselectAllFields();
            _editStreetBool = true;
            SelectedPlace = null;
            SelectedArea = null;
            SelectedStreet = street;
            PlaceInfo.Visibility = Visibility.Hidden;
            StreetInfo.Visibility = Visibility.Visible;
            StreetInfoGrid.Visibility = Visibility.Visible;
            AreaInfo.Visibility = Visibility.Hidden;
            StreetHasDogs.IsChecked = street.Dogs;
            StreetHasPower.IsChecked = street.Power;
            StreetPersons.Text = street.AmountOfPeople.ToString();
            StreetName.Text = street.Name;
            StreetPricePerNight.Text = street.PricePerNightPerPerson.ToString();
            StreetSurfaceArea.Text = street.SurfaceArea.ToString();
            foreach(Grid grid in StreetInfoGrid.Children)
            {
                foreach (var comp in grid.Children)
                {
                    if (comp is TextBox textbox) textbox.IsEnabled = true;
                    if (comp is CheckBox checkbox) checkbox.IsEnabled = true;
                }
            }
            HighLightPlaces(street, Brushes.DarkCyan);
        }

        private void HighLightPlaces(Object type, SolidColorBrush color)
        {
            List<Place> places = new List<Place>() ;

            if (type is Street street)
                places = _places.Where(p => p.StreetID == street.StreetID).ToList();            
            if (type is Area area)
                places = _places.Where(p => p.AreaID == area.AreaID).ToList();

            if (places.Count > 0)
            {
                foreach (var comp in field.Children)
                {
                    if (comp is Border placeBlock && placeBlock.Child is Canvas canvas && canvas.Name.Contains("Place"))
                    {
                        foreach (Place placeData in places)
                            if (canvas.Name.Equals("Place" + placeData.PlaceID.ToString())) canvas.Background = color;
                    }
                }
            }
        }
        private void DeselectAllFields()
        {
            foreach(var comp in field.Children)
            {
                if(comp is Line line)
                    line.Stroke = Brushes.Black;
                if (comp is Border borderArea && borderArea.Name.Contains("Canvas"))
                {
                    borderArea.BorderBrush = Brushes.Black;
                    borderArea.BorderThickness = new Thickness(1);
                }
                if (comp is Border borderPlace && borderPlace.Name.Contains("Place") && borderPlace.Child is Canvas canvasPlace)
                    canvasPlace.Background = Brushes.Black;
            }
        }

        private void ResetInputFields(string componentString)
        {
            if (componentString.Equals("Place"))
            {
                PlaceStreetComboBox.Items.Clear();
                PlaceAreaComboBox.Items.Clear();
                foreach (Street street in _streets)
                {
                    PlaceStreetComboBox.Items.Add(street.Name);
                }
                foreach (Area area in _areas)
                {
                    PlaceAreaComboBox.Items.Add(area.Name);
                }
            }
            foreach (Grid grid in PlaceInfoGrid.Children)
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
            ResetBorders();
        }

        private void ResetBorders()
        {
            StaticUIMethods.ResetTextboxBorder(PlacePlaceID);
            StaticUIMethods.ResetTextboxBorder(PlaceSurfaceArea);
            StaticUIMethods.ResetTextboxBorder(PlacePersons);
            StaticUIMethods.ResetTextboxBorder(PlacePricePerNight);
            ResetComboBoxBorder(PlaceAreaBorder);
            ResetComboBoxBorder(PlaceStreetBorder);
        }

        private void field_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                Place place = new Place(1000000000, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                GeneratePreviewPlace(place, Brushes.Aquamarine);
            }
            if(_selectedMapButton.Contains("Street"))
            {
                GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
            }
        }

        private void GeneratePreviewPlace(Place place, SolidColorBrush color)
        {
            Point p = Mouse.GetPosition(field);
            _xPressed = (int)Math.Round(p.X) - 15;
            _yPressed = (int)Math.Round(p.Y) - 15;
            GeneratePlace(place, color, false);
        }

        private void GeneratePreviewLine(string name, SolidColorBrush color)
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
            Border borderPlace = new Border();
            foreach (var component in field.Children)
            {
                if (component is Border border && border.Name.Equals("Place_1000000000"))
                {
                    borderPlace = border;
                }
            }
            field.Children.Remove(borderPlace);
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectedMapButton.Contains("Place"))
            {
                foreach (var component in field.Children)
                {
                    if (component is Border border && border.Name.Equals("Place_0"))
                    {
                        border.Name = "Place_1000000000";
                        Canvas place = (Canvas)border.Child;
                        place.Background = Brushes.Aquamarine;
                    }
                }
                Point p = Mouse.GetPosition(field);
                _xPressed = (int)Math.Round(p.X) - 15;
                _yPressed = (int)Math.Round(p.Y) - 15;
                Place place1 = new Place(0, false, 1, 1, false, 0, 0, 0, _xPressed, _yPressed);
                GeneratePreviewPlace(place1, Brushes.DarkGray);
                EnableExtendComboBoxes(false);
                HandlePlaceClick(place1, true);
            }
            else if (_selectedMapButton.Contains("Street"))
            {
                _editStreetBool = false;
                if (_streetPoint1.X == -1 && _streetPoint1.Y == -1) 
                {
                    _streetPoint1.X = -2;
                    _streetPoint1.Y = -2;
                    foreach (var component in field.Children)
                        if (component is Line line && line.Name.Equals("MoveablePoint")) {
                            _xCord1 = line.X1;
                            _yCord1 = line.Y1;
                            line.Name = "firstPoint";
                        }
                }
                else
                {
                    StreetInfo.Visibility = Visibility.Visible;
                    StreetInfoGrid.Visibility = Visibility.Visible;

                    foreach (var component in field.Children)
                    {
                        if (component is Line createdLine && createdLine.Name.Equals("LineSet"))
                        {
                            createdLine.Name = "firstPoint";
                        }
                        if (component is Line line && line.Name.Equals("firstPoint"))
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
                    }

                    PlaceInfo.Visibility = Visibility.Visible;
                    PlaceInfoGrid.Visibility = Visibility.Visible;
                    AreaInfo.Visibility = Visibility.Visible;
                    AreaInfoGrid.Visibility = Visibility.Visible;
                    foreach (Grid grid1 in StreetInfoGrid.Children)
                    {
                        foreach (var comp in grid1.Children)
                        {
                            if (comp is TextBox textbox) textbox.Text = null;
                            if (comp is CheckBox checkbox) checkbox.IsChecked = false;
                        }
                    }
                }
            }
            else if (_selectedMapButton.Contains("Area"))
            {
                GenerateMap();
                Point point = Mouse.GetPosition(field);
                double xCord = Math.Round(point.X);
                double yCord = Math.Round(point.Y);
                if (_areaStartPoint.X == -1 && _areaStartPoint.Y == -1)
                {
                    _areaStartPoint = new Point(xCord, yCord);
                    Ellipse ellipse = new Ellipse();
                    Canvas.SetLeft(ellipse, xCord - 7.5);
                    Canvas.SetTop(ellipse, yCord - 7.5);
                    ellipse.Width = 15;
                    ellipse.Height = 15;
                    ellipse.Fill = Brushes.DarkGray;
                    field.Children.Add(ellipse);
                    ResertAreaBorders();
                }
                else
                {
                    AreaInfoGrid.Visibility = Visibility.Visible;
                    AreaInfo.Visibility = Visibility.Visible;
                    SelectedArea = new Area();
                    HandleAreaClick();
                    if (xCord < _areaStartPoint.X) (xCord, _areaStartPoint.X) = (_areaStartPoint.X, xCord);
                    if(yCord < _areaStartPoint.Y) (yCord, _areaStartPoint.Y) = (_areaStartPoint.Y, yCord);
                    if (_areaStartPoint.X < 5) _areaStartPoint.X = 0;
                    if (_areaStartPoint.Y < 5) _areaStartPoint.Y = 0;

                    SelectedArea.XCord1 = (int)_areaStartPoint.X;
                    SelectedArea.YCord1 = (int)_areaStartPoint.Y;
                    SelectedArea.Width = (int)xCord - SelectedArea.XCord1;
                    SelectedArea.Height = (int)yCord - SelectedArea.YCord1;
                    if (_areaStartPoint.X + SelectedArea.Width > (int)field.Width - 5) SelectedArea.Width = (int)field.Width - SelectedArea.Width;
                    if (_areaStartPoint.Y + SelectedArea.Height > (int)field.Height - 5) SelectedArea.Height = (int)field.Height - SelectedArea.Height;
                    Border border = MapPageArea.GenerateArea(SelectedArea);
                    field.Children.Add(border);
                    _newArea = border;
                    SetAreaEvents(border, SelectedArea);

                    ToggleAreaInput(true);
                    AreaInfoVisible();
                }
            }
        }
        private void HandleAddStreet_Click(Object sender, RoutedEventArgs e)
        {
            GetAddStreetValues();
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
                ResetAfterAddingMapComponent("Street");
                GenerateMap();
            }
            
        }

        public void GetAddStreetValues()
        {
            _streetName = GetAddComponentNameTextbox(StreetName, _streetName);
            _streetSurfaceArea = GetAddSurfaceArea(StreetSurfaceArea, _streetSurfaceArea);
            _streetPersons = GetAddAmountOfPeople(StreetPersons, _streetPersons);
            _streetPricePerNightPerPerson = GetAddPricePerNightPerPerson(StreetPricePerNight, _streetPricePerNightPerPerson);
        }

        public void HandleAddPlace_Click(Object sender, RoutedEventArgs e)
        {
            GetAddPlaceValues();
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
                                                          GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          GetValueFromExtendComboBox(PlacePersonsComboBox));
                    ResetAfterAddingMapComponent("Place");
                    return;
                }
                Place place = new Place(_placePlaceID, hasPower, street.StreetID, area.AreaID, hasDogs, _placeSurfaceArea, _placePersons, _placePricePerNightPerPerson, _xPressed, _yPressed);

                if (!_editPlaceBool)
                {
                    _camping.CampingRepository.CampingPlaceRepository.AddPlace(place);
                    _camping.CampingRepository.CampingMapRepository.AddExtend(place.PlaceID,
                                                          GetValueFromExtendComboBox(PlacePowerComboBox),
                                                          GetValueFromExtendComboBox(PlaceDogsComboBox),
                                                          GetValueFromExtendComboBox(PlaceSurfaceAreaComboBox),
                                                          GetValueFromExtendComboBox(PlacePricePerNightPerPersonComboBox),
                                                          GetValueFromExtendComboBox(PlacePersonsComboBox));
                    _camping.CampingRepository.CampingPlaceRepository.GetPlaces();
                }
                ResetInputFields("Place");
                ResetAfterAddingMapComponent("Place");
            }
        }
        private bool? GetValueFromExtendComboBox(ComboBox combobox)
        {
            bool? extendBool = null;
            if(combobox.SelectedIndex == 0) extendBool = true;
            else if(combobox.SelectedIndex == 1) extendBool = false;
            return extendBool;
        }

        public void HandleCancelAddComponent_Click(Object sender, RoutedEventArgs e)
        {
            ResetAfterAddingMapComponent("Place");
            ResetAfterAddingMapComponent("Street");
            HideInfoGrids();
        }

        private void ResetAfterAddingMapComponent(string mapComponent)
        {
            UIElementCollection collection = null;
            if (mapComponent.Contains("Place"))
            {
                PlaceInfoGrid.Visibility = Visibility.Hidden;
                collection = PlaceInfoGrid.Children;
            }
            if (mapComponent.Contains("Street"))
            {
                collection = StreetInfoGrid.Children;
                StreetInfoGrid.Visibility = Visibility.Hidden;
            }

            if(collection != null && collection.Count > 0)
            {
                foreach (Grid grid in collection)
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
                GenerateMap();
            }
        }

        private void GetAddPlaceValues()
        {
            GetAddPlaceID();
            GetAddStreetID();
            GetAddAreaID();
            _placePersons = GetAddAmountOfPeople(PlacePersons, _placePersons);
            _placeSurfaceArea = GetAddSurfaceArea(PlaceSurfaceArea, _placeSurfaceArea);
            _placePricePerNightPerPerson = GetAddPricePerNightPerPerson(PlacePricePerNight, _placePricePerNightPerPerson);
    }

        private int GetAddAmountOfPeople(TextBox textbox, int amountOfPeople)
        {
            return GetAddTextBox(textbox, amountOfPeople);
        }

        private double GetAddPricePerNightPerPerson(TextBox textbox, double pricePerNightPerPerson)
        {
            double number;
            if (double.TryParse(textbox.Text, out number) && number >= 0 && !string.IsNullOrEmpty(textbox.Text))
                pricePerNightPerPerson = number;
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return pricePerNightPerPerson;
        }

        private int GetAddSurfaceArea(TextBox textbox, int surfaceArea)
        {
            return GetAddTextBox(textbox, surfaceArea);
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
            if (PlaceStreetComboBox.SelectedItem != null)
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
        private string GetAddComponentNameTextbox(TextBox textbox, string name)
        {
            if (!string.IsNullOrEmpty(textbox.Text))
            {
                name = textbox.Text;
            }
            else
            {
                StaticUIMethods.SetErrorTextboxBorder(textbox);
                _wrongInput = true;
            }
            return name;
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
        private void HandleCancelAddArea_Click(object sender, RoutedEventArgs e)
        {
            _areaStartPoint.X = -1;
            _areaStartPoint.Y = -1;
            SelectedArea = null;
            _newArea = null;
            GenerateMap();
            AreaInfoGrid.Visibility = Visibility.Hidden;
        }

        private void HandleAddArea_Click(object sender, RoutedEventArgs e)
        {
            ResertAreaBorders();
            GetAddAreaValues();
            if(!_wrongInput)
            {
                _camping.CampingRepository.CampingMapRepository.AddNewArea(SelectedArea);
                AreaInfoGrid.Visibility = Visibility.Hidden;
                _areaStartPoint.X = -1;
                _areaStartPoint.Y = -1;
                GenerateMap();
            }
        }
        private void GetAddAreaValues()
        {
            _wrongInput = false;
            SelectedArea.Name = GetAddComponentNameTextbox(AreaName, _areaName);
            GetColorID();
            SelectedArea.AmountOfPeople = GetAddAmountOfPeople(AreaAmountOfPeople, _areaPersons);
            SelectedArea.PricePerNightPerPerson = GetAddPricePerNightPerPerson(AreaPrice, _areaPricePerNightPerPerson);
            SelectedArea.SurfaceArea = GetAddSurfaceArea(AreaPlaceSurfaceArea, _areaSurfaceArea);
            SelectedArea.Power = (bool)AreaPower.IsChecked;
            SelectedArea.Dogs = (bool)AreaDogs.IsChecked;
            
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

        private void PlaceStreetAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            Border border = PlaceAreaBorder;
            if (combobox.Name.Equals(PlaceStreetComboBox.Name)) border = PlaceStreetBorder;
            if (border.BorderBrush.Equals(Brushes.Red))
            {
                ResetComboBoxBorder(border);
                _wrongInput = false;
            }
            EnableExtendComboBoxes(false);
            if (PlaceStreetComboBox.SelectedItem != null && PlaceAreaComboBox.SelectedItem != null)
            {
                EnableExtendComboBoxes(true);

                foreach (Grid grid in PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                    {
                        if (component is ComboBox com)
                        {
                            if (com.Name != PlaceStreetComboBox.Name && com.Name != PlaceAreaComboBox.Name) {
                                HandleExtending(com);
                            }
                        }
                    }
                }
            }
        }

        private void EnableExtendComboBoxes(bool extend)
        {
            foreach (Grid grid in PlaceInfoGrid.Children)
            {
                foreach (var component in grid.Children)
                {
                    if (component is ComboBox comboBox)
                    {
                        if (extend == false) comboBox.Opacity = 0.5;
                        else comboBox.Opacity = 1;
                        comboBox.IsEnabled = extend;
                    }
                }
            }
        }

        private void HandleExtendChange_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            HandleExtending(comboBox);
        }

        public void HandleExtending(ComboBox comboBox)
        {
            Area area = null;
            Street street = null;

            if (!string.IsNullOrEmpty(comboBox.Text))
            {
                if (PlaceAreaComboBox.SelectedItem != null)
                    area = _camping.CampingRepository.CampingMapRepository.GetAreaByAreaName(PlaceAreaComboBox.SelectedItem.ToString());
                if (PlaceStreetComboBox.SelectedItem != null)
                    street = _camping.CampingRepository.CampingMapRepository.GetStreetByStreetName(PlaceStreetComboBox.SelectedItem.ToString());
            }

            if (street != null || area != null)
            {
                foreach (Grid grid in PlaceInfoGrid.Children)
                {
                    foreach (var component in grid.Children)
                    {
                        if (component is TextBox textbox)
                            SetTextboxValues(comboBox, textbox, street, area);
                        else if (component is CheckBox checkbox)
                            SetCheckboxValues(comboBox, checkbox, street, area);
                    }
                }
            }
        }

        private void SetTextboxValues(ComboBox comboBox, TextBox textbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("SurfaceArea") && textbox.Name.Contains("SurfaceArea"))
            {
                textbox.Text = selectedIndex == 0 ? street.SurfaceArea.ToString() : selectedIndex == 1 ? area.SurfaceArea.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("PricePerNight") && textbox.Name.Contains("PricePerNight"))
            {
                textbox.Text = selectedIndex == 0 ? street.PricePerNightPerPerson.ToString() : selectedIndex == 1 ? area.PricePerNightPerPerson.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("Persons") && textbox.Name.Contains("Persons"))
            {
                textbox.Text = selectedIndex == 0 ? street.AmountOfPeople.ToString() : selectedIndex == 1 ? area.AmountOfPeople.ToString() : null;
                textbox.IsEnabled = selectedIndex == 2;
            }
        }

        private void SetCheckboxValues(ComboBox comboBox, CheckBox checkbox, Street street, Area area)
        {
            int selectedIndex = comboBox.SelectedIndex;
            if (comboBox.Name.Contains("Power") && checkbox.Name.Contains("Power"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Power : selectedIndex == 1 ? area.Power : false;
                checkbox.IsEnabled = selectedIndex == 2;
            }
            else if (comboBox.Name.Contains("Dogs") && checkbox.Name.Contains("Dogs"))
            {
                checkbox.IsChecked = selectedIndex == 0 ? street.Dogs : selectedIndex == 1 ? area.Dogs : false;
                checkbox.IsEnabled = selectedIndex == 2;
            }
        }

        private void Handle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if(PlaceInfoGrid.Visibility.Equals(Visibility.Hidden) && StreetInfoGrid.Visibility.Equals(Visibility.Hidden) && AreaInfoGrid.Visibility.Equals(Visibility.Hidden))
                {
                    _selectedMapButton = "View";
                    foreach (Button gridButton in MapGridButtons.Children)
                    {
                        Style selectionStyle = (Style)gridButton.FindResource("ButtonStyle1Selection");
                        gridButton.Style = selectionStyle;
                    }
                }
                else HideInfoGrids();
                _streetPoint1 = new Point(-1, -1);
                _areaStartPoint = new Point(-1, -1);
                GenerateMap();
                if(_selectedMapButton == "Place")
                {
                    GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
                }
                if (_selectedMapButton == "Street")
                {
                    GeneratePreviewLine("MoveablePoint", Brushes.Aquamarine);
                }
            }

        }

        private void HideInfoGrids()
        {
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Hidden;
            PlaceInfoGrid.Visibility = Visibility.Hidden;
            PlaceInfo.Visibility = Visibility.Visible;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Hidden;

            SelectedArea = null;
            SelectedPlace = null;
            SelectedStreet = null;
            _streetPoint1 = new Point(-1, -1);
            GenerateMap();
        }

        private void MakeMapComponentButton_Click(object sender, RoutedEventArgs e)
        {
            HideInfoGrids();
            Button button = (Button)sender;
            Style applyStyle = (Style)button.FindResource("ButtonStyle1Apply");
            Style selectionStyle = (Style)button.FindResource("ButtonStyle1Selection");
            if (button.Style.Equals(selectionStyle))
            {
                foreach (Button gridButton in MapGridButtons.Children)
                {
                    gridButton.Style = selectionStyle;
                }
                button.Style = applyStyle;
                _selectedMapButton = button.Name;
            }
            else
            {
                button.Style = selectionStyle;
                GenerateMap();
                _selectedMapButton = "View";
            }
        }
        private void AreaInfoVisible()
        {
            PlaceInfo.Visibility = Visibility.Hidden;
            StreetInfo.Visibility = Visibility.Hidden;
            AreaInfo.Visibility = Visibility.Visible;
            PlaceInfoGrid.Visibility = Visibility.Hidden;
            StreetInfoGrid.Visibility = Visibility.Hidden;
            AreaInfoGrid.Visibility = Visibility.Visible;
        }
        private void HandleAreaClick()
        {
            AreaInfoVisible();
            AreaName.Text = SelectedArea.Name;
            SetAreaComboBox();
            AreaPower.IsChecked = SelectedArea.Power;
            AreaDogs.IsChecked = SelectedArea.Dogs;
            if (SelectedArea.SurfaceArea > 0) AreaPlaceSurfaceArea.Text = SelectedArea.SurfaceArea.ToString();
            else AreaPlaceSurfaceArea.Text = "";
            if (SelectedArea.PricePerNightPerPerson > 0) AreaPrice.Text = SelectedArea.PricePerNightPerPerson.ToString();
            else AreaPrice.Text = "";
            if (SelectedArea.AmountOfPeople > 0) AreaAmountOfPeople.Text = SelectedArea.AmountOfPeople.ToString();
            else AreaAmountOfPeople.Text = "";
        }
        private void SetAreaComboBox()
        {
            AreaColor.Items.Clear();
            List<int> activeColors =_areas.Where(a => SelectedArea != null ? a.Color != SelectedArea.Color : true).Select(a => a.Color).ToList();
            List<int> possibelColors = Enumerable.Range(0, StaticUIMethods.ColorCount).Except(activeColors).ToList();
            for (int i = 0; i < possibelColors.Count; i++)
            {
                AreaColor.Items.Add(new {Text = StaticUIMethods.GetColorNameFromInt(possibelColors[i]), Value = possibelColors[i]});
            }
            if( SelectedArea != null ) AreaColor.SelectedValue = SelectedArea.Color;
        }
        private void ToggleAreaInput(bool enabled)
        {
            AreaName.IsEnabled = enabled;
            AreaColor.IsEnabled = enabled;
            AreaPower.IsEnabled = enabled;
            AreaDogs.IsEnabled = enabled;
            AreaPlaceSurfaceArea.IsEnabled = enabled;
            AreaPrice.IsEnabled = enabled;
            AreaAmountOfPeople.IsEnabled = enabled;
            if (enabled) AddAreaButton.Visibility = Visibility.Visible;
            else AddAreaButton.Visibility = Visibility.Hidden;
        }
        private void GetColorID()
        {
            if (AreaColor.SelectedItem != null)
            {
                SelectedArea.Color = (int)AreaColor.SelectedValue;
            }
            else
            {
                SetErrorComboBoxBorder(AreaColorBorder);
                _wrongInput = true;
            }
        }
        private void ResertAreaBorders()
        {
            StaticUIMethods.ResetTextboxBorder(AreaName);
            StaticUIMethods.ResetTextboxBorder(AreaPrice);
            StaticUIMethods.ResetTextboxBorder(AreaAmountOfPeople);
            StaticUIMethods.ResetTextboxBorder(AreaPlaceSurfaceArea);
            ResetComboBoxBorder(AreaColorBorder);
        }
    }
}
