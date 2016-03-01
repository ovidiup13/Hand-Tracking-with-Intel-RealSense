using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CameraModule.Annotations;
using MarkerTracking.Implementation;
using Un4seen.Bass;

namespace AudioModule.Interfaces
{
    public abstract class Speaker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method that plays a sound through the Speaker instance.
        /// </summary>
        /// <param name="stream">Bass Stream to play</param>
        public static void Play(int stream)
        {
            if (stream == 0)
                throw new AudioException("Speaker: Stream error. Stream cannot be zero.");
            
            //play the stream
            Bass.BASS_ChannelPlay(stream, true);
        }

        /// <summary>
        ///     Method that returns a boolean indicating whether the current speaker is playing a file.
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying(int stream)
        {
            return Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        /// <summary>
        ///     Method that stops the current playback.
        /// </summary>
        public static void StopPlayback(int stream)
        {
            //if the stream is not defined, then ignore
            if (stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. Error: " + Bass.BASS_ErrorGetCode());

            //wait until finished
//            while (IsPlaying(stream)) { }

            if (!Bass.BASS_ChannelStop(stream))
                throw new AudioException("Cannot stop playback. Error: " + Bass.BASS_ErrorGetCode());

            //free stream
            Bass.BASS_StreamFree(stream);
        }

        /// <summary>
        ///     Plays the confirmation sound.
        /// </summary>
        public void PlayConfirm()
        {
            var confirmStream = Bass.BASS_StreamCreateFile(ConfirmFile, 0L, 0L,
                SpeakerFlag | BASSFlag.BASS_STREAM_AUTOFREE);

            if (confirmStream == 0)
            {
                throw new AudioException("Confirm stream encountered an error. Error: " + Bass.BASS_ErrorGetCode());
            }

            //play the confirm sound
            Bass.BASS_ChannelPlay(confirmStream, true);
        }

        /// <summary>
        ///     Plays the confirmation sound.
        /// </summary>
        public void Test()
        {
            var testStream = Bass.BASS_StreamCreateFile(TestFile, 0L, 0L,
                SpeakerFlag | BASSFlag.BASS_STREAM_AUTOFREE);

            if (testStream == 0)
            {
                throw new AudioException("Confirm stream encountered an error. Error: " + Bass.BASS_ErrorGetCode());
            }

            //play the confirm sound
            Bass.BASS_ChannelPlay(testStream, true);
        }

        /// <summary>
        ///     Method that returns the position of the speaker.
        /// </summary>
        /// <returns></returns>
        public PXCMPoint3DF32 GetPosition()
        {
            return Marker.Position3D;
        }

        public BASSFlag GetFlag()
        {
            return SpeakerFlag;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region vars

//        protected AudioSettingsImpl Settings;
        private BASSFlag _speakerFlag;
        public BASSFlag SpeakerFlag
        {
            get
            {
                return _speakerFlag;
            }
            set
            {
                _speakerFlag = value;
                OnPropertyChanged(nameof(SpeakerFlag));
                Console.WriteLine("Speaker flag changed to: " + _speakerFlag);
            }
        }

        private Marker _marker;
        public Marker Marker
        {
            get { return _marker; }
            set
            {
                if (value == null) return;
                _marker = value;
                OnPropertyChanged(nameof(Marker));
            }
        }

        #endregion

        #region files

        private const string ConfirmFile = "Sounds\\confirm.wav";
        private const string TestFile = "Sounds\\peakGeiger.wav";

        #endregion
    }
}