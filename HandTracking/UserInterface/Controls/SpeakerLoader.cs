using System;
using GalaSoft.MvvmLight;

namespace UserInterface.Controls
{
    public class SpeakerLoader : DefaultContentLoader
    {
        /// <summary>
        ///     Loads the content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri</param>
        /// <returns>The loaded content.</returns>
        protected override object LoadContent(Uri uri)
        {
            // return a new LoremIpsum user control instance no matter the uri
            return new Views.Content.SpeakerSettings();
        }

        public ViewModelBase ViewModel { get; set; }
    }
}