using System.Collections.Generic;
using System.Windows.Data;
using MarkerTracking.Implementation;

namespace MarkerTracking
{
    public class MarkerTrackingViewModel
    {
        public MarkerTrackingViewModel()
        {
            //initialize marker tracking module
            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = (MarkerTrackingImpl) _markerTrackingModule.GetInstance();
            _markerData = (MarkerData) _markerTracking.GetData();

            //set marker display source
            _markerCollection = new CompositeCollection();
        }



        #region marker tracking variables

        private readonly MarkerTrackingModule _markerTrackingModule;
        private readonly MarkerTrackingImpl _markerTracking;
        private readonly MarkerData _markerData;
        private Dictionary<int, PXCMPoint3DF32> _markerLocations;

        private readonly CompositeCollection _markerCollection;

        #endregion
    }
}