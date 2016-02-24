using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CameraModule.Interfaces.Module;

namespace MarkerTracking.Implementation
{
    public class MarkerData : IData
    {
        /// <summary>
        ///     Constructor creates a new instance of MarkerData.
        /// </summary>
        internal MarkerData()
        {
//            Markers = new Dictionary<int, PXCMPoint3DF32>();
//              Markers = new ObservableCollection<Marker> {new Marker(1, new PXCMPoint3DF32(1, 2, 3))};
        }

        /// <summary>
        ///     Clear the list of markers
        /// </summary>
        internal void ClearMarkers()
        {
            lock (_lock1)
            {
                Markers.Clear();
            }
        }

        /// <summary>
        ///     Method that adds or updates the marker list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        internal void AddMarker(int id, PXCMPoint3DF32 position)
        {
            //check whether id is within bounds
            if (id < 0) throw new MarkerTrackingException("ID cannot be less than zero."); 

            lock (_lock1)
            {
                /*//if it contains the marker 
                if (Markers.ContainsKey(id))
                {
                    //update its position
                    Markers[id] = position;
                }
                //otherwise, add it to the dictionary
                else
                {*/
//                    Markers.Add(new Marker(id, position));
               // }
            }
        }



        #region vars

        private readonly object _lock1 = new object();

        public ObservableCollection<Marker> Markers { get; }

        #endregion
    }
}