using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;

namespace CameraModule.Interfaces.Settings
{
    public abstract class AudioSettings : ISettings
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
