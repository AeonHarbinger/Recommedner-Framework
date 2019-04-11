using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{ 
    /// <summary>
    /// Represents a list of items provided to the user as recommendation with additional info for evaluation.
    /// </summary>
    public class Recommendation
    {
        /// <summary>
        /// Identification of user who received this recommendation.
        /// </summary>
        public int UserId;
        /// <summary>
        /// List of items recommended to the user.
        /// </summary>
        public List<RecommendedItem> Items;

        /// <summary>
        /// When the recommendation was created.
        /// </summary>
        public DateTime AtTime;
        /// <summary>
        /// How long did it take for recommender system to provide this recommendation (in ms).
        /// </summary>
        public int ResponseTime;

        /// <summary>
        /// Creates new instance of recommendation with given properties.
        /// </summary>
        /// <param name="userId">Identification of the user.</param>
        /// <param name="items">List of recoommended items.</param>
        /// <param name="atTime">Time at which recommendation was created.</param>
        /// <param name="responseTime">How long did it take for recommender to provide list (in ms).</param>
        public Recommendation(int userId, List<RecommendedItem> items, DateTime atTime, int responseTime)
        {
            UserId = userId;
            Items = items;
            AtTime = atTime;
            ResponseTime = responseTime;
        }
    }
}
