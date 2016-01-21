using System.Collections.Generic;
using System.IO;
using HandTracking.Interfaces.AudioController;

namespace HandTracking.Implementation.AudioDesigns
{
    class GeigerAudioDesign : IAudioDesign
    {
        public AudioDesignType Type { get; set; }

        private string _file;

        public GeigerAudioDesign(string file)
        {
           SetFile(file);
        }

        public void SetFile(string file)
        {
            _file = file;
        }

        public void Play(double distance)
        {
            throw new System.NotImplementedException();
        }

        public void SetSpeaker(Speaker speaker)
        {
            throw new System.NotImplementedException();
        }
    }
}
