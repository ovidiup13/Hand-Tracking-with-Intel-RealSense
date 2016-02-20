using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FirstFloor.ModernUI.Windows.Controls;
using MarkerTracking.Implementation;
using Image = System.Drawing.Image;

namespace UserInterface.Pages
{
    /// <summary>
    ///     Interaction logic for MarkerTrackingPage.xaml
    /// </summary>
    public partial class MarkerTrackingPage : UserControl
    {
        public MarkerTrackingPage()
        {
            InitializeComponent();

            //initialize marker tracking module
            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = (MarkerTrackingImpl) _markerTrackingModule.GetInstance();
            _markerData = (MarkerData) _markerTracking.GetData();

            //set marker display source
            markerCollection = new CompositeCollection();
            MarkerList.ItemsSource = markerCollection;
        }

        /// <summary>
        /// Method that starts the marker tracking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTrackingButton_OnClick(object sender, RoutedEventArgs e)
        {
            //if the button is not enabled, stop marker tracking
            if (!StartTrackingButton.IsEnabled)
            {
                return;
            }

            //disable next button
            NextButton.IsEnabled = false;

            //initialize camera modules
            try
            {
                _markerTracking.InitializeCameraModules();
            }
            catch (MarkerTrackingException markerTrackingException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(markerTrackingException.Message, "Error", messageBox);
                Console.WriteLine(markerTrackingException.StackTrace);
                return;
            }

            //clear marker locations
            markerCollection.Clear();

            //start marker tracking
            try
            {
                _markerTracking.StartProcessing();
            }
            catch (MarkerTrackingException markerTrackingException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(markerTrackingException.Message, "Error", messageBox);
                Console.WriteLine(markerTrackingException.StackTrace);
            }

            //register events with handlers
            _markerTracking.NewImageAvailable += imageStream_NewImageAvailable;
            _markerTracking.NewMarkerAvailable += markerData_NewMarkerAvailable;


            //change buttons
            StartTrackingButton.IsEnabled = false;
            StopTrackingButton.IsEnabled = true;
        }

        /// <summary>
        /// Method that updates the image on the view when a new frame is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void imageStream_NewImageAvailable(object sender, MarkerTrackingImpl.NewImageArgs args)
        {
            Dispatcher.Invoke(() => { ImgColorStream.Source = ImageUtils.ConvertBitmapToWpf(args.Bitmap); });
        }

        /// <summary>
        /// Add a new marker to the collection.
        /// TODO: remove marker when not in image frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void markerData_NewMarkerAvailable(object sender, MarkerTrackingImpl.NewMarkerArgs args)
        {
            Dispatcher.Invoke(() => { AddNewMarker(new Marker() { Id = args.Id, xPosition = args.Position.x, yPosition = args.Position.y, zPosition = args.Position.z}); }); 
        }

        /// <summary>
        /// Method that adds a new marker to the collection if it does not exist or updates a marker position otherwise.
        /// </summary>
        /// <param name="marker"></param>
        private void AddNewMarker(Marker marker)
        {
//            markerCollection.Clear();

            //go through all markers in the collection
            foreach(Marker m in markerCollection)
            {
                //if any of the markers match id, update its position
                if (m.Id == marker.Id)
                {
                    markerCollection.Remove(m);
                    break;
                }
            }

            //add the new marker to the collection
            markerCollection.Add(marker);

            /*Pen blackPen = new Pen(Color.Red, 3);
            using (var graphics = Graphics.FromImage()
            {
                graphics.DrawLine(blackPen, x1, y1, x2, y2);
            }*/
        }

        /// <summary>
        ///     Method that stops the marker tracking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTrackingButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!StopTrackingButton.IsEnabled)
            {
                return;
            }

            try
            {
                _markerTracking.StopProcessing();
            }
            catch (MarkerTrackingException markerTrackingException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(markerTrackingException.Message, "Error", messageBox);
                Console.WriteLine(markerTrackingException.StackTrace);
            }

            StopTrackingButton.IsEnabled = false;
            StartTrackingButton.IsEnabled = true;

            if (markerCollection.Count != 0)
            {
                //do nothing
                NextButton.IsEnabled = true;
            }
        }

        #region tracking vars

        private readonly MarkerTrackingModule _markerTrackingModule;
        private readonly MarkerTrackingImpl _markerTracking;
        private readonly MarkerData _markerData;
        private Dictionary<int, PXCMPoint3DF32> _markerLocations;

        private CompositeCollection markerCollection;

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Marker
    {
        public int Id { get; set; }
        public PXCMPoint3DF32 Position { get; set; }
        public float xPosition { get; set; }
        public float yPosition { get; set; }
        public float zPosition { get; set; }

    }
}