using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioModule.Implementation.AudioController;
using CameraModule.Annotations;
using CameraModule.Implementation.MarkerTracking;
using Un4seen.Bass;

namespace AudioModule.Interfaces
{
    public abstract class AudioDesign : INotifyPropertyChanged
    {
        /// <summary>
        ///     Method that plays audio feedback from referenced speakers.
        /// </summary>
        public abstract void Play();

        /// <summary>
        ///     Sets the speaker for the AudioDesign
        /// </summary>
        /// <param name="speaker"></param>
        public void SetSpeaker(Speaker speaker)
        {
            if (speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");
            Speaker = speaker;
        }

        /// <summary>
        ///     Method that stops playback for current speakers.
        /// </summary>
        public abstract void StopPlayback();

        /// <summary>
        ///     Method that returns the speaker position.
        /// </summary>
        /// <returns>position of speaker in 3D</returns>
        public PXCMPoint3DF32 GetSpeakerPosition()
        {
            return Speaker.GetPosition();
        }

        /// <summary>
        ///     Method that returns the id of the target speaker.
        /// </summary>
        /// <returns></returns>
        public string GetSpeakerId()
        {
            return Speaker.Marker.Id.ToString();
        }

        /// <summary>
        ///     Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        ///     Method that sets the distance between hand and target speaker.
        /// </summary>
        public abstract void SetDistance(double distance);

        /*/// <summary>
        /// Method that sets the wrist speaker.
        /// </summary>
        /// <param name="flag"></param>
        public static void SetWristSpeaker(BASSFlag flag)
        {
            WristSpeaker = new SpeakerImpl(flag);
        }

        /// <summary>
        /// Method that sets the wrist speaker.
        /// </summary>
        /// <returns></returns>
        public static BASSFlag GetWristSpeaker()
        {
            return WristSpeaker.GetFlag();
        }*/

        /// <summary>
        ///     Method that sets the individual designs to use wrist instead of the target speaker.
        /// </summary>
        public void UseWrist()
        {
            FeedbackType = FeedbackType.Wrist;
        }

        #region vars

        protected Speaker Speaker;

        private static readonly BASSFlag _defaultWristSpeaker = BASSFlag.BASS_SPEAKER_REAR2RIGHT; //8

        private static SpeakerImpl _wristSpeakerImpl = new SpeakerImpl(new Marker(0), _defaultWristSpeaker);
        public static SpeakerImpl WristSpeaker
        {
            get
            {
                return _wristSpeakerImpl;
            }
            set
            {
                if (value == null) return;
                _wristSpeakerImpl = value;
            }
        }

        /// <summary>
        ///     Variables describing the type of audio design
        /// </summary>
        private DesignType _audioDesignDesignType;
        public DesignType AudioDesignDesignType
        {
            get { return _audioDesignDesignType; }
            set
            {
                _audioDesignDesignType = value;
                OnPropertyChanged(nameof(AudioDesignDesignType));
            }
        }

        private FeedbackType _feedbackType;
        public FeedbackType FeedbackType
        {
            get { return _feedbackType; }
            set
            {
                _feedbackType = value;
                OnPropertyChanged(nameof(FeedbackType));
            }
        }
        

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}