using AudioModule.Implementation.AudioController;
using AudioModule.Implementation.AudioDesigns.Constant;
using AudioModule.Implementation.AudioDesigns.Geiger;
using AudioModule.Implementation.AudioDesigns.Pitch;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AudioModuleTests.Implementation.AudioController
{
    [TestClass()]
    public class AudioDesignFactoryTests
    {
        [TestMethod()]
        public void GetAudioDesignTestConstantIndividual()
        {
            //arrange
            var feedback = FeedbackType.Individual;
            var design = DesignType.Constant;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(ConstantIndividual));
        }

        [TestMethod()]
        public void GetAudioDesignTestConstantCoalescent()
        {
            //arrange
            var feedback = FeedbackType.Coalescent;
            var design = DesignType.Constant;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(ConstantCoalescent));
        }

        [TestMethod()]
        public void GetAudioDesignTestControl()
        {
            //arrange
            var feedback = FeedbackType.Individual;
            var design = DesignType.Control;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(ControlDesign));
        }

        [TestMethod()]
        public void GetAudioDesignTestGeigerIndividual()
        {
            //arrange
            var feedback = FeedbackType.Individual;
            var design = DesignType.Geiger;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(GeigerIndividual));
        }

        [TestMethod()]
        public void GetAudioDesignTestGeigerCoalescent()
        {
            //arrange
            var feedback = FeedbackType.Coalescent;
            var design = DesignType.Geiger;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(GeigerCoalescent));
        }

        [TestMethod()]
        public void GetAudioDesignTestGeigerWrist()
        {
            //arrange
            var feedback = FeedbackType.Wrist;
            var design = DesignType.Geiger;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(GeigerWrist));
        }

        [TestMethod()]
        public void GetAudioDesignTestPitchIndividual()
        {
            //arrange
            var feedback = FeedbackType.Individual;
            var design = DesignType.Pitch;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(PitchIndividual));
        }

        [TestMethod()]
        public void GetAudioDesignTestPitchCoalescent()
        {
            //arrange
            var feedback = FeedbackType.Coalescent;
            var design = DesignType.Pitch;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(PitchCoalescent));
        }

        [TestMethod()]
        public void GetAudioDesignTestPitchWrist()
        {
            //arrange
            var feedback = FeedbackType.Wrist;
            var design = DesignType.Pitch;

            //act
            var audioDesign = AudioDesignFactory.GetAudioDesign(design, feedback);

            //assert
            Assert.IsInstanceOfType(audioDesign, typeof(PitchWrist));
        }
    }
}