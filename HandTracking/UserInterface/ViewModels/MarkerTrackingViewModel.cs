using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CameraModule.Implementation.MarkerTracking;
using CameraModule.Interfaces;
using CameraModule.Interfaces.Module;
using CameraModule.Interfaces.UI;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;

namespace UserInterface.ViewModels
{
    public class MarkerTrackingViewModel : ViewModelBase
    {
        public MarkerTrackingViewModel(MarkerTrackingImpl markerTracking)
        {
            _markerTracking = markerTracking;
            _markersDetected = new ObservableCollection<Marker>();

            StartTrackingCommand = new RelayCommand(StartTracking, CanStartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking, CanStopTracking);

            Dispatcher.CurrentDispatcher.ShutdownStarted += ShutDownViewModel;
        }

        private void ShutDownViewModel(object sender, EventArgs e)
        {
            if (_markerTracking.Settings.TrackingStatus != TrackingStatus.Running) return;
            var messageBox = MessageBoxButton.OK;
            ModernDialog.ShowMessage("Application will exit.", "Bye", messageBox);
            _markerTracking.StopProcessing();
        }

        /// <summary>
        ///     Method that returns a boolean whether tracking can be stopped.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanStopTracking(object arg)
        {
            return _markerTracking.Settings.TrackingStatus == TrackingStatus.Running;
        }

        /// <summary>
        ///     Method that returns a boolean whether tracking can be started.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanStartTracking(object arg)
        {
            return _markerTracking.Settings.TrackingStatus != TrackingStatus.Running;
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

            //update subscribers
            if (MarkersDetected.Count > 0)
            {
                NewMarkersAvailableEvent?.Invoke(this, new NewMarkersArgs(MarkersDetected.ToList()));
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
        ///     Event raised when no markers are detected on screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void markerData_NoMarkerAvailable(object sender, MarkerTrackingImpl.NewMarkerArgs args)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => _markersDetected.Clear()));
        }

        /// <summary>
        ///     Method that updates the markers detected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void markerData_NewMarkerAvailable(object sender, MarkerTrackingImpl.NewMarkerArgs args)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render,
                new Action(() => UpdateMarkers(args.Markers)));
        }

        /// <summary>
        ///     Updates list of markers.
        /// </summary>
        /// <param name="markers"></param>
        private void UpdateMarkers(List<Marker> markers)
        {
            _markersDetected.Clear();
            foreach (var marker in markers)
            {
                UpdateMarker(marker);
            }
        }

        /// <summary>
        ///     Method that updates the list of markers detected.
        /// </summary>
        /// <param name="m"></param>
        private void UpdateMarker(Marker m)
        {
            foreach (var marker in MarkersDetected)
            {
                if (marker.Id == m.Id)
                {
                    marker.Position3D = m.Position3D;
                    return;
                }
            }

            MarkersDetected.Add(m);
        }

        /// <summary>
        ///     Method that updates the image on the view when a new frame is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void imageStream_NewImageAvailable(object sender, Tracking.NewImageArgs args)
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
                {
                    _markersDetected = value;
                    RaisePropertyChanged(nameof(MarkersDetected));
                }
            }
        }

        #endregion

        #region commands

        public ICommand StartTrackingCommand { get; set; }
        public ICommand StopTrackingCommand { get; set; }

        #endregion

        #region vars

        private MarkerTrackingModule _markerTrackingModule;
        private MarkerTrackingImpl _markerTracking;

        #endregion

        #region marker delegate

        public event NewMarkersAvailable NewMarkersAvailableEvent;
        public delegate void NewMarkersAvailable(object sender, NewMarkersArgs markersArgs);

        /// <summary>
        ///     Event arguments class for passing the image bitmap to the main view.
        /// </summary>
        public class NewMarkersArgs : EventArgs
        {
            public NewMarkersArgs(List<Marker> markers)
            {
                Markers = markers;
            }

            public List<Marker> Markers { get; private set; }
        }

        #endregion
    }
}