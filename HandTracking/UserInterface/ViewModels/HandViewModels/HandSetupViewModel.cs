using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using CameraModule.Implementation.HandTracking;
using CoreModule.Interfaces;
using GalaSoft.MvvmLight;

namespace UserInterface.ViewModels.HandViewModels
{
    public class HandSetupViewModel : ViewModelBase
    {

        public HandSetupViewModel(HandTrackingImpl handTracking)
        {
            //retrieve hand tracking module and settings
            HandTrackingSettings = handTracking.Settings;

            //create new participant
            Participant = new Participant();

            //get various types for UI
            InitializeEnumTypes();
        }

        private void InitializeEnumTypes()
        {
            TrackingModeTypes = HandTrackingSettings.GetTrackingTypes();
            JointTypes = HandTrackingSettings.GetJointTypes();
            AccessOrderTypes = HandTrackingSettings.GetAccessOrderTypes();
            ExtremityTypes = HandTrackingSettings.GetExtremeTypes();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            if (strValue.Length >= 4)
                return false;
            return true;
        }

        #region vars

        public HandTrackingSettings HandTrackingSettings { get; set; }

        public List<PXCMHandData.TrackingModeType> TrackingModeTypes { get; set; }
        public List<PXCMHandData.JointType> JointTypes { get; set; }
        public List<PXCMHandData.AccessOrderType> AccessOrderTypes { get; set; }
        public List<PXCMHandData.ExtremityType> ExtremityTypes { get; set; }


        public Participant Participant { get; set; }

        #endregion

    }

    public class TrackingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(PXCMHandData.TrackingModeType.TRACKING_MODE_FULL_HAND);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
