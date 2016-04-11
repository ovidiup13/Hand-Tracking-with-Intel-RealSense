using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;
using CameraModule.Annotations;

namespace CoreModule.Implementation
{
    public class ConditionDesign : INotifyPropertyChanged
    {
        private static long _id;

        /// <summary>
        ///     Field that sets and gets the type of AudioDesign associated with the ConditionDesign.
        /// </summary>
        private AudioDesign _audioDesign;

        /// <summary>
        ///     Field that holds the design type of the condition.
        /// </summary>
        private DesignType _designType;

        /// <summary>
        ///     Field that holds the type of feedback of the condition.
        /// </summary>
        private FeedbackType _feedbackType;

        /// <summary>
        ///     Field that sets and gets the number of trials associated with each condition
        /// </summary>
        private int _numberOfTrials = 1;

        /// <summary>
        ///     Constructor that instantiates an object with a number of trials.
        /// </summary>
        /// <param name="trials"></param>
        public ConditionDesign()
        {
            ConditionDesignId = _id++;
        }

        /// <summary>
        ///     Fields that sets an ID to the current condition.
        /// </summary>
        public long ConditionDesignId { get; protected set; }

        public AudioDesign AudioDesign
        {
            get { return _audioDesign; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _audioDesign = value;
            }
        }

        public DesignType DesignType
        {
            get { return _designType; }
            set
            {
                _designType = value;
                OnPropertyChanged(nameof(DesignType));
            }
        }

        public FeedbackType FeedbackType
        {
            get { return _feedbackType; }
            set
            {
                _feedbackType = value;
                OnPropertyChanged(nameof(FeedbackType));
            }
        }

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