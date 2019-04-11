using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{   
    /// <summary>
    /// Represents a user in recommender system.
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// Unique identification of the user.
        /// </summary>
        public abstract int Id { get; set; }
    }
}
