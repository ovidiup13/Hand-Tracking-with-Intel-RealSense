using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;

namespace AudioModule.Implementation.AudioDesigns.Constant
{
    public class ControlDesign : ConstantIndividual
    {

        public ControlDesign()
        {
            AudioDesignDesignType = DesignType.Control;
            FeedbackType = FeedbackType.Individual;
        }

        public override void Play()
        {
        }

        public override string ToString()
        {
            return base.ToString() + "_CTRL";
        }


    }
}
