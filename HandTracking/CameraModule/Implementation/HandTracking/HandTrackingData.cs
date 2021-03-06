﻿using CameraModule.Interfaces.Module;

namespace CameraModule.Implementation.HandTracking
{
    /// <summary>
    /// Class that holds 2D and 3D location of the hand. The fields will be accessible anywhere in the system, 
    /// however, it can only be set by the Tracking implementation. 
    /// 
    /// As multiple threads will set and get the values, locking is in place in order to avoid race conditions.
    /// </summary>
    public class HandTrackingData : Data
    {

        #region locks

        private readonly object _lock2D = new object();
        private readonly object _lock3D = new object();

        #endregion

        #region private vars

        private PXCMPoint3DF32 _point3D;
        private PXCMPoint3DF32 _point2D;

        #endregion

        #region access vars

        //access 2D point
        public PXCMPoint3DF32 Location2D
        {
            get { return _point2D; }
            internal set
            {
                lock (_lock2D)
                {
                    _point2D = value;
                }
            }

        }

        //access 3D point
        public PXCMPoint3DF32 Location3D
        {
            get { return _point3D; }
            internal set
            {
//                lock (_lock3D)
//                {
                    _point3D = value;
//                }
            }
        }


        private bool _handDetected = false;
        public bool HandDetected
        {
            get { return _handDetected; }
            internal set
            {
                lock (_lock3D)
                {
                    _handDetected = value;
                    OnPropertyChanged(nameof(HandDetected));
                }
            }
        }

        #endregion

        /// <summary>
        /// Resets the hand module.
        /// </summary>
        public void ResetHand()
        {
            HandDetected = false;
            Location3D = new PXCMPoint3DF32(100, 100, 100);
        }
    }
}
