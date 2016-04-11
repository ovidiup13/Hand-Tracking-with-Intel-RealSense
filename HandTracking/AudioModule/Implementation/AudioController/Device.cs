using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CameraModule.Annotations;

namespace AudioModule.Implementation.AudioController
{
    public class Device : INotifyPropertyChanged
    {

        /// <summary>
        /// Constructor for creating a new device.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Device(int id, string name)
        {
            Id = id;
            Name = name;
        }

        #region vars

        //id of device
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

        

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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
