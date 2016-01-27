using System.Collections.Generic;
using System.IO;
using HandTracking.Interfaces.AudioController;

namespace HandTracking.Implementation.AudioDesigns
{
    class GeigerAudioDesign : AudioDesign
    {
        public override AudioDesignType Type { get; set; }
        public override string File { get; set; }

        private string _file;

        public GeigerAudioDesign(string file)
        {
           SetFile(file);
        }

        public void SetFile(string file)
        {
            _file = file;
        }

        public override void Play(double distance, float volume)
        {
            throw new System.NotImplementedException();
        }

        public override void SetSpeaker(Speaker speaker)
        {
            throw new System.NotImplementedException();
        }
    }
}
