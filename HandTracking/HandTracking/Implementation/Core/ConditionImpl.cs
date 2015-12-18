using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.Core;

namespace HandTracking.Implementation.Core
{
    class ConditionImpl : ICondition
    {
        public IAudioDesign AudioDesign { get; set; }

        private int _numberOfTrials;

        /// <summary>
        /// Constructor that instantiates an object with a number of trials.
        /// </summary>
        /// <param name="trials"></param>
        public ConditionImpl(int trials)
        {
            SetNumberOfTrials(trials);
        }

        public void SetAudioDesign(IAudioDesign audioDesign)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that sets the number of trials for the condition.
        /// </summary>
        /// <param name="value"></param>
        public void SetNumberOfTrials(int value)
        {
            _numberOfTrials = value;
        }

        /// <summary>
        /// Implementation of method that returns the number of trials for the condition.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfTrials()
        {
            return _numberOfTrials;
        }
    }
}
