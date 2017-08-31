using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Caching;

namespace Nido.Common.Utilities.Cache
{
    public class NidoCachePolicy : CachePolicy
    {
        /// <summary>
        /// User defined constructor for NidoCachePolicy
        /// </summary>
        /// <param name="_duration">in seconds</param>
        public NidoCachePolicy(int _durationSec)
        {
            this.Duration = new TimeSpan(0, 0, _durationSec);
        }

        /// <summary>
        /// User defined constructor for NidoCachePolicy
        /// </summary>
        /// <param name="_min">duration in minutes</param>
        /// <param name="_sec">duration in seconds</param>
        public NidoCachePolicy(int _min, int _sec)
        {
            this.Duration = new TimeSpan(0, _min, _sec);
        }

        /// <summary>
        /// User defined constructor for NidoCachePolicy
        /// </summary>
        /// <param name="_hr">duration in hours</param>
        /// <param name="_min">duration in minutes</param>
        /// <param name="_sec">duration in seconds</param>
        public NidoCachePolicy(int _hr, int _min, int _sec)
        {
            this.Duration = new TimeSpan(_hr, _min, _sec);
        }
    }
}
