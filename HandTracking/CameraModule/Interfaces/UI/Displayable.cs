using System.ComponentModel;

namespace CameraModule.Interfaces.UI
{
    /// <summary>
    ///     Provides a base implementation for objects that are displayed in the UI.
    /// </summary>
    public abstract class Displayable
        : INotifyPropertyChanged
    {
        private string _displayName;

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}