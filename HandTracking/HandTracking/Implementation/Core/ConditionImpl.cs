using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces;
using HandTracking.Interfaces.Core;

namespace HandTracking.Implementation.Core
{
    class ConditionImpl : ICondition
    {
        public IAudioDesign AudioDesign { get; set; }
        public ITrial[] Trials { get; set; }

        public ConditionImpl(ITrial[] trials)
        {
            Trials = trials;
        }
    }
}
