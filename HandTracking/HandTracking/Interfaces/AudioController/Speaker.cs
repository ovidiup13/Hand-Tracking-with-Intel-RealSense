using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandTracking.Interfaces.AudioController
{
    public abstract class Speaker
    {

        /// <summary>
        /// Method that plays a sound through the Speaker instance.
        /// </summary>
        /// <param name="soundPath">Path to the wav file</param>
        public abstract void Play(string soundPath);
    }
}
