using System;
using System.Collections.Generic;
using AudioModule.Interfaces;
using CameraModule.Interfaces.Settings;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioController
{
    public class AudioSettingsImpl : AudioSettings
    {

        public AudioSettingsImpl()
        {
            InitializeSoundCard(DefaultSoundCard);
            SetVolume(DefaultVolume);
        }

        #region designs and feedback

        /// <summary>
        /// Method that returns all the design types
        /// </summary>
        /// <returns>List of all audio designs types</returns>
        public List<DesignType> GetDesignTypes()
        {
            return GetEnumList<DesignType>();
        }

        /// <summary>
        /// Method that returns all the feedback types.
        /// </summary>
        /// <returns>List of audio feedback types</returns>
        public List<FeedbackType> GetFeedbackTypes()
        {
            return GetEnumList<FeedbackType>();
        } 

        /// <summary>
        /// Generic method that retrieves all the values of a enumeration type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }

        #endregion

        //get the wrist speaker from the audio design
        private SpeakerImpl _wristSpeaker = AudioDesign.WristSpeaker;
        public SpeakerImpl WristSpeaker
        {
            get { return _wristSpeaker; }
            set
            {
                if (value == null) return;
                _wristSpeaker = value;
                OnPropertyChanged(nameof(WristSpeaker));
            }
        }

        /// <summary>
        ///     Method that sets the current volume for all speakers.
        /// </summary>
        /// <param name="value">Floating point value between 0 and 1.</param>
        private void SetVolume(int value)
        {
            if (!Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, value))
            {
                throw new AudioException("An error occurred while setting the speaker volume.");
            }
        }

        /// <summary>
        ///     Method that increases the volume.
        /// </summary>
        public void IncreaseVolume()
        {
            //check if volume already maximum
            if (Math.Abs(_volume - 1) <= 0)
                return;

            //increase volume
            _volume += VolumeGap;
            SetVolume(_volume);
        }

        /// <summary>
        ///     Method that decreases the current volume.
        /// </summary>
        public void DecreaseVolume()
        {
            //if already 0, skip
            if (Math.Abs(_volume) <= 0)
                return;

            //decrease volume
            _volume -= VolumeGap;
            SetVolume(_volume);
        }

        /// <summary>
        ///     Method that initializes the default soundcard as the BASS.
        /// </summary>
        internal void InitializeSoundCard(int id)
        {
            //bass.net registration
            BassNet.Registration("ovidiu.popoviciu@hotmail.co.uk", "2X2417830312420");

            if (!Bass.BASS_Init(id, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                throw new AudioException("An error occurred while initializing the BASS library: " +
                                         Bass.BASS_ErrorGetCode());
            }
        }

        //todo: add all settings for speaker controller here

        #region volume variables

        private const int VolumeGap = 50;
        private const int DefaultVolume = 200;
        private int _volume = DefaultVolume;

        protected const int DefaultSoundCard = -1;

        #endregion

        #region speaker variables

        //BASSFlags
        public readonly List<BASSFlag> SpeakerFlags = new List<BASSFlag>
        {
            BASSFlag.BASS_SPEAKER_REAR2LEFT, //7
            BASSFlag.BASS_SPEAKER_REARRIGHT, //6
            BASSFlag.BASS_SPEAKER_REARLEFT, //5
            BASSFlag.BASS_SPEAKER_LFE, //4
            BASSFlag.BASS_SPEAKER_CENTER, //3
            BASSFlag.BASS_SPEAKER_FRONTLEFT, //1???
            BASSFlag.BASS_SPEAKER_FRONTRIGHT, //2???
            BASSFlag.BASS_SPEAKER_REAR2RIGHT //8

        };

        #endregion
    }
}