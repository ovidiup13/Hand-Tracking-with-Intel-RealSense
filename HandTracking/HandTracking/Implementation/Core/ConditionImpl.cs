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
    class ConditionImpl : Condition
    {
        
        /// <summary>
        /// Constructor that instantiates an object with a number of trials.
        /// </summary>
        /// <param name="trials"></param>
        public ConditionImpl(int trials)
        {
            NumberOfTrials = trials;
        }
    }
}
