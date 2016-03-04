using AudioModule.Implementation.AudioDesigns.Constant;
using AudioModule.Implementation.AudioDesigns.Geiger;
using AudioModule.Implementation.AudioDesigns.Pitch;
using AudioModule.Interfaces;

namespace AudioModule.Implementation.AudioController
{
    public static class AudioDesignFactory
    {
        public static AudioDesign GetAudioDesign(DesignType designType, FeedbackType feedbackType)
        {
            switch (designType)
            {
                case DesignType.Constant:
                {
                    switch (feedbackType)
                    {
                        case FeedbackType.Individual:
                            return new ConstantIndividual();
                        case FeedbackType.Coalescent:
                            return new ConstantCoalescent();
                        case FeedbackType.Wrist:
                            return new ConstantWrist(); 
                    }
                    break;
                }

                case DesignType.Geiger:
                {
                    switch (feedbackType)
                    {
                        case FeedbackType.Individual:
                            return new GeigerIndividual();
                        case FeedbackType.Coalescent:
                            return new GeigerCoalescent();
                        case FeedbackType.Wrist:
                            return new GeigerWrist();
                    }
                    break;
                }

                case DesignType.Pitch:
                {
                    switch (feedbackType)
                    {
                        case FeedbackType.Individual:
                            return new PitchIndividual();
                        case FeedbackType.Coalescent:
                            return new PitchCoalescent();
                        case FeedbackType.Wrist:
                            return new PitchWrist();
                    }
                    break;
                }

                case DesignType.Control:
                {
                    return new ControlDesign();
                }
            }

            return null;
        }
    }
}