using ClassLibrary1.Interfaces.Core;

namespace HandTracking.Implementation.Core
{
    public class ConditionImpl : Condition
    {
        private static long _id = 0;

        /// <summary>
        ///     Constructor that instantiates an object with a number of trials.
        /// </summary>
        /// <param name="trials"></param>
        public ConditionImpl(int trials)
        {
            NumberOfTrials = trials;
            ConditionId = _id++;
        }
    }
}