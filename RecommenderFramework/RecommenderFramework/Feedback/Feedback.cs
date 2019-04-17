using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents attitude a user expressed for an item.
    /// </summary>
    public abstract class Feedback
    {
        /// <summary>
        /// Identification of user.
        /// </summary>
        public abstract int UserId      { get; set; }
        /// <summary>
        /// Identification of item.
        /// </summary>
        public abstract int ItemId      { get; set; }
        /// <summary> 
        /// Timestamp at which feedback was collected.
        /// </summary>
        public abstract DateTime AtTime { get; set; }
    }
}
