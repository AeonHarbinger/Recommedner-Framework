using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents attitude a user expressed towards item.
    /// </summary>
    [Serializable]
    public abstract class Feedback
    {
        /// <summary>
        /// Identification of user.
        /// </summary>
        public int UserId;
        /// <summary>
        /// Identification of item.
        /// </summary>
        public int ItemId;
        /// <summary>
        /// Timestamp at which feedback was collected.
        /// </summary>
        public DateTime AtTime;
    }
}
