using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;

namespace CoreModule.Interfaces
{
    /// <summary>
    ///     Class that represents the participant. Currently only contains the ID but other data can be filled in and output
    ///     to files for inspection.
    /// </summary>
    public class Participant : INotifyPropertyChanged
    {
        public Participant()
        {
        }

        public Participant(int id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return "P" + _id;
        }

        #region vars

        /// <summary>
        /// Id
        /// </summary>
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Age
        /// </summary>
        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        /// <summary>
        /// Gender
        /// </summary>
        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        #endregion

        #region property changed methods

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}