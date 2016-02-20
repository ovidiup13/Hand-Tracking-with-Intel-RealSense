using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Core;

namespace HandTracking.Implementation.Core
{
    class Trial : ITrial
    {
        private readonly int _id;

        public Trial(int id)
        {
            this._id = id;
        }

        public override string ToString()
        {
            return "Trial: " + _id;
        }
    }
}
