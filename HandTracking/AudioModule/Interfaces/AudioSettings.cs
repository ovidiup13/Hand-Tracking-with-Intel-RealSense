using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;

namespace AudioModule.Interfaces
{
    public abstract class AudioSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
