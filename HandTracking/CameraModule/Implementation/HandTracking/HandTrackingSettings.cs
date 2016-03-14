﻿using System;
using System.Collections.Generic;
using CameraModule.Interfaces.Settings;

namespace CameraModule.Implementation.HandTracking
{
    public class HandTrackingSettings : CameraSettings
    {
        #region constructors

        /// <summary>
        ///     Creates a new instance of HandTrackingSettings based on its super class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        protected internal HandTrackingSettings(int width, int height, int fps) : base(width, height, fps)
        {
        }

        /// <summary>
        ///     Creates a new instance of HandTrackingSettings with default values.
        /// </summary>
        protected internal HandTrackingSettings()
        {
        }

        #endregion

        #region methods retrieving enum types

        /// <summary>
        ///     Method that returns a list of all extremity types.
        /// </summary>
        /// <returns></returns>
        public List<PXCMHandData.ExtremityType> GetExtremeTypes()
        {
            return GetEnumList<PXCMHandData.ExtremityType>();
        }

        /// <summary>
        ///     Method that retrieves all enumeration values for JointType.
        /// </summary>
        /// <returns></returns>
        public List<PXCMHandData.JointType> GetJointTypes()
        {
            return GetEnumList<PXCMHandData.JointType>();
        }

        /// <summary>
        ///     Method that retrieves all enumeration values for Tracking Mode Type.
        /// </summary>
        /// <returns></returns>
        public List<PXCMHandData.TrackingModeType> GetTrackingTypes()
        {
            return GetEnumList<PXCMHandData.TrackingModeType>();
        }

        /// <summary>
        ///     Method that retrieves all enumeration values for Access Order Type.
        /// </summary>
        /// <returns></returns>
        public List<PXCMHandData.AccessOrderType> GetAccessOrderTypes()
        {
            return GetEnumList<PXCMHandData.AccessOrderType>();
        }

        /// <summary>
        ///     Generic method that retrieves all the values of a enumeration type.
        /// </summary>
        /// <typeparam name="T">Type of enumeration</typeparam>
        /// <returns>a list of T values</returns>
        private static List<T> GetEnumList<T>()
        {
            var array = (T[]) Enum.GetValues(typeof (T));
            var list = new List<T>(array);
            return list;
        }

        #endregion

        #region Hand settings vars

        /// <summary>
        ///     description of extremity type to follow
        /// </summary>
        private PXCMHandData.ExtremityType _extremityType = PXCMHandData.ExtremityType.EXTREMITY_CLOSEST;
        public PXCMHandData.ExtremityType ExtremityType
        {
            get { return _extremityType; }
            set
            {
                _extremityType = value;
                OnPropertyChanged(nameof(ExtremityType));
            }
        }

        /// <summary>
        ///     description of stabilizer does
        /// </summary>
        private bool _enableStabilizer = true;
        public bool EnableStabilizer
        {
            get { return _enableStabilizer; }
            set
            {
                _enableStabilizer = value;
                OnPropertyChanged(nameof(EnableStabilizer));
            }
        }

        /// <summary>
        ///     description of smoothing value
        /// </summary>
        private float _smoothingValue = 1;
        public float SmoothingValue
        {
            get { return _smoothingValue; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value));
                _smoothingValue = value;
                OnPropertyChanged(nameof(SmoothingValue));
            }
        }

        /// <summary>
        ///     Joint type to tracked. Can be the center of palm, extremities, etc.
        /// </summary>
        private PXCMHandData.JointType _jointType = PXCMHandData.JointType.JOINT_MIDDLE_TIP;
        public PXCMHandData.JointType JointType
        {
            get { return _jointType; }
            set
            {
                _jointType = value;
                OnPropertyChanged(nameof(JointType));
            }
        }

        /// <summary>
        ///     Field that sets the current tracking mode.
        /// </summary>
        private PXCMHandData.TrackingModeType _trackingModeType = PXCMHandData.TrackingModeType.TRACKING_MODE_EXTREMITIES;

        public PXCMHandData.TrackingModeType TrackingModeType
        {
            get { return _trackingModeType; }
            set
            {
                _trackingModeType = value;
                OnPropertyChanged(nameof(TrackingModeType));
            }
        }

        /// <summary>
        ///     Field that holds the order in which hands are assigned IDs.
        /// </summary>
        private PXCMHandData.AccessOrderType _accessOrderType = PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_ID;

        public PXCMHandData.AccessOrderType AccessOrderType
        {
            get { return _accessOrderType; }
            set
            {
                _accessOrderType = value;
                OnPropertyChanged(nameof(AccessOrderType));
            }
        }

        #endregion
    }
}