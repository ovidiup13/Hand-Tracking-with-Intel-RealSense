using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.AudioController.Designs;

namespace HandTracking.Implementation.AudioDesigns.Geiger
{
    class GeigerIndividual : GeigerDesign, INdividualDesign
    {

        /// <summary>
        ///     Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        public GeigerIndividual() : base()
        {
        }

        /// <summary>
        ///     Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="file"></param>
        public GeigerIndividual(string file) : base(file)
        {
        }

        public void PlayIndividual()
        {
            throw new NotImplementedException();
        }
    }
}
