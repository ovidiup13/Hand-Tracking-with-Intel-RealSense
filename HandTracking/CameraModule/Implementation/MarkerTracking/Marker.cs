using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;

namespace CameraModule.Implementation.MarkerTracking
{
    public class Marker : INotifyPropertyChanged
    {
        //TODO: remove this contructor
        public Marker(int id)
        {
            Id = id;
        }

        protected internal Marker(int id, PXCMPoint3DF32 position3D, PXCMPointF32 position2D)
        {
            Id = id;
            Position3D = position3D;
            Position2D = position2D;

            //set xyz for UI
        }

        private void SetXyz()
        {
            XPosition = _position3D.x;
            YPosition = _position3D.y;
            ZPosition = _position3D.z;
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
                OnPropertyChanged(nameof(Position3D));
            }
        }
        public PXCMPointF32 Position2D { get; set; }

        //for UI access
        private float _x;
        public float XPosition
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged(nameof(XPosition));
            }
        }

        private float _y;
        public float YPosition
        {
            get { return _y; }
            set
            {
                _y = value;
                OnPropertyChanged(nameof(YPosition));
            }
        }

        private float _z;
        public float ZPosition
        {
            get { return _z; }
            set
            {
                _z = value;
                OnPropertyChanged(nameof(ZPosition));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}