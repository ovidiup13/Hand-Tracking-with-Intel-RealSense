using System;
using System.Threading;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Interfaces.Module
{
    public abstract class Tracking
    {

        #region private vars

        private IData _data;
        private ISettings _settings;

        #endregion

        /// <summary>
        /// Method that sets and gets the IData field for this Tracking instance.
        /// </summary>
        public IData Data
        {
            get { return _data; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _data = value;
            }
        }

        /// <summary>
        /// Field that sets and gets the ISettings field for this Tracking instance.
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

        /// <summary>
        /// Method that initializes the camera modules and sets up the module for processing.
        /// </summary>
        /// <returns>True if no erros occurred, false otherwise.</returns>
        public abstract bool InitializeCameraModules();

        /// <summary>
        /// Method that starts the processing thread within the derived types.
        /// </summary>
        public abstract void StartProcessing();

        /// <summary>
        /// Method that stops the processing thread within derived types.
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        /// Method that pauses the processig thread within derived types.
        /// </summary>
        public abstract void PauseProcessing();

        /// <summary>
        /// Returns the data instance.
        /// </summary>
        /// <returns></returns>
        public abstract IData GetData();
    }
}