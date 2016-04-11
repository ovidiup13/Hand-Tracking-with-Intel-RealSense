using System;
using System.Collections.Generic;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;
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
                AudioDesign.WristSpeaker.SpeakerFlag = _wristSpeaker.SpeakerFlag;
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
        private void InitializeSoundCard(int id)
        {
            //bass.net registration
            BassNet.Registration("email", "api-key");

            //get devices
            Devices = GetDeviceList();

            //initialize default sound device
            if (!Bass.BASS_Init(id, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                throw new AudioException("An error occurred while initializing the BASS library: " +
                                         Bass.BASS_ErrorGetCode());
            }
        }

        /// <summary>
        /// Returns the list of currently connected devices. 
        /// </summary>
        /// <returns></returns>
        public List<Device> GetDeviceList()
        {
            //set device list
            List<Device> devices = new List<Device>();
            var deviceInfos = Bass.BASS_GetDeviceInfos();
            for (var i = 0; i < deviceInfos.Length; i++)
            {
                devices.Add(new Device(i, deviceInfos[i].name));
            }

            return devices;
        }

        /// <summary>
        /// Method that initializes a new sound card device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq"></param>
        /// <param name="flag"></param>
        public void InitializeSoundDevice(int device, int freq)
        {
            //free previous device
            Bass.BASS_Free();

            //init selected device
            //todo : the init flag can be selected in the UI by the client
            if (!Bass.BASS_Init(device, freq, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                throw new AudioException("An error occurred while initializing the BASS library: " +
                                         Bass.BASS_ErrorGetCode());
            }
        }   

        #region volume variables

        private const int VolumeGap = 50;
        private const int DefaultVolume = 200;
        private int _volume = DefaultVolume;

        protected const int DefaultSoundCard = -1;

        #endregion

        //list of devices
        private List<Device> _devices;
        public List<Device> Devices
        {
            get { return _devices; }
            set
            {
                if (value == null) return;
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }  

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