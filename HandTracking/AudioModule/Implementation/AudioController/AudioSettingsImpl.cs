using System;
using System.Collections.Generic;
using AudioModule.Interfaces;
using CameraModule.Interfaces.Settings;

namespace AudioModule.Implementation.AudioController
{
    public class AudioSettingsImpl : AudioSettings
    {
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

        //todo: create methods to initialize speaker flags here
        //todo: add all settings for speaker controller here

    }
}