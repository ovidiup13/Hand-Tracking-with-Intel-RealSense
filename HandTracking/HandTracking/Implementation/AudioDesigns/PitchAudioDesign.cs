using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandTracking.Interfaces.AudioController;

namespace HandTracking.Implementation.AudioDesigns
{
    class PitchAudioDesign : AudioDesign
    {

        public override void Play(float volume)
        {
            throw new NotImplementedException();
        }

        public override void StopPlayback()
        {
            throw new NotImplementedException();
        }

        public override void SetDistance(double distance)
        {
            throw new NotImplementedException();
        }

        #region vars

        private float _volume;
        private Timer _timer;
        private readonly List<string> _files;
        private string _currentFile;
        private int _stream;
        private double _distance = -1;

        #endregion

    }
}
