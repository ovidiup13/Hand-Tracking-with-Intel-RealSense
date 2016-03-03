using System.Collections.Generic;
using CoreModule.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using HandModule.Implementation.HandTracking;

namespace UserInterface.ViewModels.HandViewModels
{
    public class HandSetupViewModel : ViewModelBase
    {

        public HandSetupViewModel()
        {
            //retrieve hand tracking module and settings
            var handTracking = SimpleIoc.Default.GetInstance<HandTrackingImpl>();
            HandTrackingSettings = handTracking.Settings as HandTrackingSettings;

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
