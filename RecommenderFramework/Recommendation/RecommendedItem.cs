using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents item that has been recommended to the user accompanied by expected preference for evaluation.
    /// </summary>
    public class RecommendedItem
    {
        /// <summary>
        /// Item which was recommended.
        /// </summary>
        public Item Item;
        /// <summary>
        /// Users preference for item as expected by the recommender.
        /// </summary>
        public float? ExpectedPreference;
    }
}
