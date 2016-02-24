using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CameraModule.Interfaces.UI;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using MarkerTracking.Implementation;

namespace UserInterface.ViewModel
{
    public class MarkerTrackingViewModel : ViewModelBase
    {
        public MarkerTrackingViewModel()
        {
            //load markertracking modules
            LoadModules();

            StartTrackingCommand = new RelayCommand(StartTracking, CanStartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking, CanStopTracking);
        }

        /// <summary>
        ///     Method that returns a boolean whether tracking can be stopped.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanStopTracking(object arg)
        {
            return _markerTracking.IsProcessing;
        }

        /// <summary>
        ///     Method that returns a boolean whether tracking can be started.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanStartTracking(object arg)
        {
            return !_markerTracking.IsProcessing;
        }

        private void LoadModules()
        {
            //initialize marker tracking module
            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = (MarkerTrackingImpl) _markerTrackingModule.GetInstance();
            _markerData = (MarkerData) _markerTracking.GetData();

            //create marker collection
            _markersDetected = new ObservableCollection<Marker>();
        }

        /// <summary>
        ///     Method that stops marker tracking.
        /// </summary>
        /// <param name="obj"></param>
        private void StopTracking(object obj)
        {
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
        }

        /// <summary>
        ///     Method that starts marker tracking.
        /// </summary>
        /// <param name="obj"></param>
        private void StartTracking(object obj)
        {
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
            _markerTracking.NoMarkerAvailable += markerData_NoMarkerAvailable;
        }

        /// <summary>
        /// Event raised when no markers are detected on screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void markerData_NoMarkerAvailable(object sender, MarkerTrackingImpl.NewMarkerArgs args)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => _markersDetected.Clear()));
        }

        /// <summary>
        /// Method that updates the markers detected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void markerData_NewMarkerAvailable(object sender, MarkerTrackingImpl.NewMarkerArgs args)
        {
            /*_markersDetected = args.Markers;
            foreach (var marker in _markersDetected)
            {
                Console.WriteLine(marker.Id);
            }*/

            //TODO: draw marker location on image
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>  UpdateMarker(args.Marker)));
//            UpdateMarker(args.Marker);
        }

        private void UpdateMarker(Marker m)
        {

            foreach (var marker in _markersDetected)
            {
                if (marker.Id == m.Id)
                {
                    _markersDetected.Clear();
                    break;
                }
            }

            _markersDetected.Add(m);
        }

        /// <summary>
        ///     Method that updates the image on the view when a new frame is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void imageStream_NewImageAvailable(object sender, MarkerTrackingImpl.NewImageArgs args)
        {
            MarkerTrackingImage = ImageUtils.ConvertBitmapToWpf(args.Bitmap);
        }

        #region data

        /// <summary>
        ///     Marker Tracking Image
        /// </summary>
        private ImageSource _markerTrackingImage;

        public ImageSource MarkerTrackingImage
        {
            get { return _markerTrackingImage; }
            set
            {
                _markerTrackingImage = value;
                RaisePropertyChanged(nameof(MarkerTrackingImage));
            }
        }

        /// <summary>
        ///     List of markers detected
        /// </summary>
        private ObservableCollection<Marker> _markersDetected;

        public ObservableCollection<Marker> MarkersDetected
        {
            get { return _markersDetected; }
            set
            {
                if (value != null)
                    _markersDetected = value;
                RaisePropertyChanged(nameof(MarkersDetected));
            }
        }

        #endregion

        #region commands

        public ICommand StartTrackingCommand { get; private set; }

        public ICommand StopTrackingCommand { get; private set; }

        #endregion

        #region vars

        private MarkerTrackingModule _markerTrackingModule;
        private MarkerTrackingImpl _markerTracking;
        private MarkerData _markerData;

        #endregion
    }
}