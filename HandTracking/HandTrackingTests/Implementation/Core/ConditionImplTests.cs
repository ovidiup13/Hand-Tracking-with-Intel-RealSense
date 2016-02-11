﻿using HandTracking.Implementation.AudioDesigns;
using HandTracking.Implementation.AudioDesigns.Constant;
using HandTracking.Implementation.Core;
using HandTracking.Interfaces.AudioController.Designs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandTrackingTests.Implementation.Core
{
    [TestClass()]
    public class ConditionImplTests
    {
        [TestMethod()]
        public void ConditionImplTestTrials()
        {
            int trials = 3;
            var impl = new ConditionImpl(trials);

            Assert.Equals(impl.NumberOfTrials, trials);
        }

        [TestMethod()]
        public void ConditionImplTestAudioDesign()
        {
            int trials = 3;
            ConditionImpl impl = new ConditionImpl(trials);

            impl.AudioDesign = new ConstantIndividual();

            Assert.IsNotNull(impl.AudioDesign);
        }

        [TestMethod()]
        public void ConditionImplTestId()
        {
            int trials = 3;
            ConditionImpl impl = new ConditionImpl(trials);

            Assert.AreEqual(impl.ConditionId, 2);
        }
    }
}