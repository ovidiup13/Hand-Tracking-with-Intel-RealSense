using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.AudioController;

namespace HandTracking.Interfaces.Core
{
    abstract class Condition
    {

        #region private vars

        private int _numberOfTrials;
        private string _id;
        private IAudioDesign _audioDesign;

        #endregion

        /// <summary>
        /// Fields that sets an ID to the current condition.
        /// </summary>
        public string ConditionId
        {
            get { return _id; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _id = value;
            }
        }

        /// <summary>
        /// Field that sets and gets the type of AudioDesign associated with the Condition.
        /// </summary>
        public IAudioDesign AudioDesign
        {
            get { return _audioDesign; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _audioDesign = value;
            }
        }

        /// <summary>
        /// Field that sets and gets the number of trials associated with each condition
        /// </summary>
        public int NumberOfTrials
        {
            get { return _numberOfTrials; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                _numberOfTrials = value;
            }
        }
    }
}
