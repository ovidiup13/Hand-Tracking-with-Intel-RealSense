using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    class SpeakerController : ISpeakerController
    {

        #region vars

        private Dictionary<int, PXCMPoint3DF32> _speakers;

        #endregion

        public SpeakerController(Dictionary<int, PXCMPoint3DF32> speakers)
        {
            _speakers = speakers;
        }

        public void SetSpeakers(List<ISpeaker> speakers)
        {
            throw new NotImplementedException();
        }

        public List<ISpeaker> GetAllSpeakers()
        {
            throw new NotImplementedException();
        }

        public BASSFlag? GetNextSpeaker()
        {
            throw new NotImplementedException();
        }

        public void SetAudioDesign(IAudioDesign audioDesign)
        {
            throw new NotImplementedException();
        }

        public void PlaySounds()
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfSpeakers()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void StopStreaming()
        {
            throw new NotImplementedException();
        }
    }
}
