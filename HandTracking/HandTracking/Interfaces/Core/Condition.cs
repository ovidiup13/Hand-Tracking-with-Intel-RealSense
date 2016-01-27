using System;
using HandTracking.Interfaces.AudioController;

namespace HandTracking.Interfaces.Core
{
    public abstract class Condition
    {
        /// <summary>
        ///     Fields that sets an ID to the current condition.
        /// </summary>
        public long ConditionId { get; protected set; }

        /// <summary>
        ///     Field that sets and gets the type of AudioDesign associated with the Condition.
        /// </summary>
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
        ///     Field that sets and gets the number of trials associated with each condition
        /// </summary>
        public int NumberOfTrials
        {
            get { return _numberOfTrials; }
            protected set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                _numberOfTrials = value;
            }
        }

        #region private vars

        private int _numberOfTrials;
        private AudioDesign _audioDesign;

        #endregion
    }
}