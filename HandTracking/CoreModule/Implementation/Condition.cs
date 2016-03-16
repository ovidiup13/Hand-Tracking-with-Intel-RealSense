using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;
using CameraModule.Annotations;
using CoreModule.Interfaces;

namespace CoreModule.Implementation
{
    public class Condition : INotifyPropertyChanged
    {
        private static long _id = 0;

        /// <summary>
        ///     Constructor that instantiates an object with a number of trials.
        /// </summary>
        /// <param name="trials"></param>
        public Condition()
        {
            ConditionId = _id++;
        }

        /// <summary>
        ///     Fields that sets an ID to the current condition.
        /// </summary>
        public long ConditionId { get; protected set; }

        /// <summary>
        ///     Field that sets and gets the type of AudioDesign associated with the Condition.
        /// </summary>
        private AudioDesign _audioDesign;
        public AudioDesign AudioDesign
        {
            get { return _audioDesign; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _audioDesign = value;
            }
        }

        /// <summary>
        /// Field that holds the design type of the condition.
        /// </summary>
        private DesignType _designType;
        public DesignType DesignType
        {
            get { return _designType; }
            set
            {
                _designType = value;
                OnPropertyChanged(nameof(DesignType));
            }
        }

        /// <summary>
        /// Field that holds the type of feedback of the condition.
        /// </summary>
        private FeedbackType _feedbackType;
        public FeedbackType FeedbackType
        {
            get { return _feedbackType; }
            set
            {
                _feedbackType = value;
                OnPropertyChanged(nameof(FeedbackType));
            }
        }

        /// <summary>
        ///     Field that sets and gets the number of trials associated with each condition
        /// </summary>
        private int _numberOfTrials = 1;
        public int NumberOfTrials
        {
            get { return _numberOfTrials; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                _numberOfTrials = value;
                OnPropertyChanged(nameof(NumberOfTrials));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}