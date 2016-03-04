using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;

namespace CameraModule.Interfaces.Module
{
    public abstract class Data : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
