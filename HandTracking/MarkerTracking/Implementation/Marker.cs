namespace MarkerTracking.Implementation
{
    public class Marker
    {
        protected internal Marker(int id, PXCMPoint3DF32 position3D, PXCMPointF32 position2D)
        {
            Id = id;
            Position3D = position3D;
            Position2D = position2D;

            //set xyz for UI
        }

        private void SetXyz()
        {
            XPosition = Position3D.x;
            YPosition = Position3D.y;
            ZPosition = Position3D.z;
        }

        #region vars

        public int Id { get; set; }

        private PXCMPoint3DF32 _position3D;
        public PXCMPoint3DF32 Position3D
        {
            get { return _position3D; }
            set
            {
                _position3D = value;
                SetXyz();
            }
        }
        public PXCMPointF32 Position2D { get; set; }

        //for UI access
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public float ZPosition { get; set; }

        #endregion
    }
}