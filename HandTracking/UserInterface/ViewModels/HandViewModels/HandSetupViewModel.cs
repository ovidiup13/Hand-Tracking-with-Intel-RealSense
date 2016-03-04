using System.Collections.Generic;
using CameraModule.Implementation.HandTracking;
using CoreModule.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

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
        }

        #region vars

        public HandTrackingSettings HandTrackingSettings { get; set; }

        public List<PXCMHandData.TrackingModeType> TrackingModeTypes { get; set; }
        public List<PXCMHandData.JointType> JointTypes { get; set; }
        public List<PXCMHandData.AccessOrderType> AccessOrderTypes { get; set; }

        public Participant Participant { get; set; }

        #endregion

    }
}
