using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// A database representation of a recommended item.
    /// </summary>
    [Table]
    internal class DbRecommendedItem
    {
        /// <summary>
        /// Identification of the recommendation this item is part of.
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public int RecommendationId      { get; set; }
        /// <summary>
        /// Identification of the item.
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public int ItemId         { get; set; }
        /// <summary>
        /// Preference of the user for this item as expected by the recommender system. 
        /// </summary>
        [Column]
        public float? ExpectedPreference { get; set; }

        /// <summary>
        /// Creates new instance with for specified item with the listid in which it is contained.
        /// </summary>
        /// <param name="recId">Identification of the recommendation this item is part of.</param>
        /// <param name="ri">Item whose DB representation we want.</param>
        public DbRecommendedItem(int recId, RecommendedItem ri)
        {
            RecommendationId = recId;
            ItemId = ri.Item.Id;
            ExpectedPreference = ri.ExpectedPreference;
        }
    }
}
