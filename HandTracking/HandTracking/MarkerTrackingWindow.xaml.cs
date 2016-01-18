using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using HandTracking.Implementation.MarkerTracking;
using HandTracking.Interfaces.Module;

namespace HandTracking
{
    //TODO: have the image color stream display on the image plane, currently there is a conflict between marker tracking and the other
    //TODO: allow user to update marker location during experiment 
    /// <summary>
    ///     Interaction logic for MarkerTrackingWindow.xaml
    /// </summary>
    public partial class MarkerTrackingWindow : Window
    {
        /// <summary>
        ///     Method that initializes the marker tracking module and starts marker tracking process.
        /// </summary>
        private void StartTrackingClick(object sender, RoutedEventArgs e)
        {
            if (_markerTracking != null)
            {
                var messageBox = MessageBox.Show("Marker Tracking is currently processing.");
                return;
            }

            // Initializing the image stream
//            _imageStream = new RealSenseImageStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR); // Change here with you want to see other stream types
//            _imageStream.InitializeStream();
//            _imageStream.StartStream();
//            _imageStream.NewImageAvailable += imageStream_NewImageAvailable;

            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = _markerTrackingModule.GetInstance();
            _markerTracking.InitializeCameraModules();
            _markerData = _markerTracking.GetData() as MarkerData;

            _markerTracking.StartProcessing();
        }

        private void imageStream_NewImageAvailable(object sender, RealSenseImageStream.NewImageArgs args)
        {
            Dispatcher.Invoke(() => { imgColorStream.Source = ImageUtils.ConvertBitmapToWpf(args.Bitmap); });
        }

        /// <summary>
        ///     Method that stops the image stream.
        /// </summary>
        private void CloseImageStream()
        {
            // Avoiding problems with Dispatcher
            _imageStream.NewImageAvailable -= imageStream_NewImageAvailable;
            _imageStream.StopStream();
        }

        /// <summary>
        ///     Method that stops Marker tracking process,
        /// </summary>
        private void StopTrackingClick(object sender, RoutedEventArgs e)
        {
            if (_markerTracking != null && _markerTracking.IsProcessing)
            {
                //stop processing
                _markerTracking.StopProcessing();
                //get marker data
                _markerLocations = _markerData.Markers;
                //set to null
                _markerTracking = null;
            }
        }

        /// <summary>
        ///     Method that returns to the Main Menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (_markerTracking != null && !_markerTracking.IsProcessing)
            {
                _markerTracking.StopProcessing();
//                CloseImageStream();
            }

            //hide the current window
            Hide();
            //go back to previous window
            _previousWindow.Show();
        }

        private void MarkerTrackingWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //if marker tracking has not been initialized or is null, exit
            if (_markerTracking == null || !_markerTracking.IsProcessing)
            {
                var result =
                    MessageBox.Show(
                        "Are you sure you want to quit?",
                        "Exit Application", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
//                    CloseImageStream();
                    Application.Current.Shutdown();
                }
            }
            //marker tracking is not null and running
            else
            {
                var result =
                    MessageBox.Show(
                        "Marker Tracking process is currently running. Do you want to close it and exit the application?",
                        "Marker Tracking is Running", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _markerTracking.StopProcessing();
//                    CloseImageStream();
                    Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        ///     //TODO: test this method
        ///     Method called when the user presses the Next button on the window. If marker locations
        ///     have been registered, the user may proceed with the main experiment.
        ///     Otherwise, an error message will be displayed. The user may not continue if locations have not been registered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_button_OnClick(object sender, RoutedEventArgs e)
        {
            if (_markerLocations == null || _markerLocations.Count == 0)
            {
                MessageBox.Show(
                    "Marker Locations have not been registered. Please Start Marker Tracking and place the markers in front of the camera.",
                    "Marker Locations not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (_markerTracking.IsProcessing)
            {
                MessageBox.Show(
                    "Please stop marker tracking process before continuing. Make sure marker locations have been registered.",
                    "Marker Tracking still Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                var result = MessageBox.Show(
                    "Proceed to the experiment?",
                    "Marker Locations updated", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _nextWindow = new MainWindow(_previousWindow, _markerLocations);
                    Close();
                    _nextWindow.Show();
                }
            }
        }

        #region constructors

        public MarkerTrackingWindow(Window previous)
        {
            InitializeComponent();
            _previousWindow = previous;
        }

        public MarkerTrackingWindow()
        {
        }

        #endregion

        #region window vars

        private readonly Window _previousWindow;
        private Window _nextWindow;

        #endregion

        #region tracking vars

        private MarkerTrackingModule _markerTrackingModule;
        private RealSenseImageStream _imageStream;

        private Tracking _markerTracking;
        private MarkerData _markerData;
        private Dictionary<int, PXCMPoint3DF32> _markerLocations;

        #endregion
    }
}