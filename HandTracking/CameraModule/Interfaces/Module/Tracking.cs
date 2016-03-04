using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using CameraModule.Annotations;
using CameraModule.Interfaces.Settings;

namespace CameraModule.Interfaces.Module
{
    public abstract class Tracking : INotifyPropertyChanged
    {
        /// <summary>
        ///     Method that sets and gets the IData field for this Tracking instance.
        /// </summary>
        public Data Data
        {
            get { return _data; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _data = value;
            }
        }

        /// <summary>
        ///     Field that sets and gets the ISettings field for this Tracking instance.
        /// </summary>
        public ISettings Settings
        {
            get { return _settings; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _settings = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method that initializes the camera modules and sets up the module for processing.
        /// </summary>
        /// <returns>True if no erros occurred, false otherwise.</returns>
        public abstract void InitializeCameraModules();

        /// <summary>
        ///     Method that starts the processing thread within the derived types.
        /// </summary>
        public abstract void StartProcessing();

        /// <summary>
        ///     Method that runs in a separate thread and provides tracking calculations/processing.
        /// </summary>
        protected abstract void TrackingThread();

        /// <summary>
        ///     Method that stops the processing thread within derived types.
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        ///     Method that pauses the processig thread within derived types.
        /// </summary>
        public abstract void PauseProcessing();

        /// <summary>
        ///     Returns the data instance.
        /// </summary>
        /// <returns></returns>
        public abstract Data GetData();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region new image events

        public delegate void NewImageEventHandler(object sender, NewImageArgs args);

        /// <summary>
        ///     Event arguments class for passing the image bitmap to the main view.
        /// </summary>
        public class NewImageArgs : EventArgs
        {
            public NewImageArgs(PXCMCapture.StreamType streamType, Bitmap bitmap)
            {
                Bitmap = bitmap;
                StreamType = streamType;
            }

            public Bitmap Bitmap { get; private set; }
            private PXCMCapture.StreamType StreamType { get; set; }
        }

        #endregion

        #region private vars

        private Data _data;
        private ISettings _settings;

        #endregion

        #region RealSense vars

        protected PXCMSession Session;
        protected PXCMSenseManager SenseManager;

        #endregion

        #region threading vars

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                _isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        protected bool IsInitialized;
        protected Thread ProcessingThread;
        protected bool ProcessingFlag;

        #endregion
    }
}