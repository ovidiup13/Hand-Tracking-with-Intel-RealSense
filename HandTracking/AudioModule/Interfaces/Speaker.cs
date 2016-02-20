using AudioModule.Implementation.AudioController;
using Un4seen.Bass;

namespace AudioModule.Interfaces
{
    public abstract class Speaker
    {
        /// <summary>
        ///     Method that plays a sound through the Speaker instance.
        /// </summary>
        /// <param name="stream">Bass Stream to play</param>
        public void Play(int stream)
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
        public void StopPlayback(int stream)
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
        /// Plays the confirmation sound.
        /// </summary>
        public void PlayConfirm()
        {
            int confirmStream = Bass.BASS_StreamCreateFile(ConfirmFile, 0L, 0L,
                SpeakerFlag | BASSFlag.BASS_STREAM_AUTOFREE);

            if (confirmStream == 0)
            {
                throw new AudioException("Confirm stream encountered an error. Error: " + Bass.BASS_ErrorGetCode());
            }

            //play the confirm sound
            Bass.BASS_ChannelPlay(confirmStream, true);
        }

        /// <summary>
        ///     Method that returns the position of the speaker.
        /// </summary>
        /// <returns></returns>
        public PXCMPoint3DF32 GetPosition()
        {
            return Position;
        }

        /// <summary>
        ///     Method that sets the current position of the speaker.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(PXCMPoint3DF32 position)
        {
            Position = position;
        }

        /// <summary>
        ///     Method that returns the id.
        /// </summary>
        /// <returns>id as int</returns>
        public int GetSpeakerId()
        {
            return Id;
        }

        public BASSFlag GetFlag()
        {
            return SpeakerFlag;
        }

        #region vars

        protected SpeakerSettingsImpl Settings;

        protected BASSFlag SpeakerFlag;

        protected PXCMPoint3DF32 Position;

        protected int Id;

        protected static readonly string ConfirmFile = "Sounds\\confirm.wav";

        #endregion
    }
}