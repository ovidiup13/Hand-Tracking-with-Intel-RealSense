using System.Collections.Generic;
using HandTracking.Interfaces.Module;

namespace HandTracking.Implementation.MarkerTracking
{
    internal class MarkerData : IData
    {
        /// <summary>
        ///     Constructor creates a new instance of MarkerData.
        /// </summary>
        internal MarkerData()
        {
            Markers = new Dictionary<int, PXCMPoint3DF32>();
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
                //if it contains the marker 
                if (Markers.ContainsKey(id))
                {
                    //update its position
                    Markers[id] = position;
                }
                //otherwise, add it to the dictionary
                else
                {
                    Markers.Add(id, position);
                }
            }
        }

        #region vars

        private readonly object _lock1 = new object();

        public Dictionary<int, PXCMPoint3DF32> Markers { get; }

        #endregion
    }
}