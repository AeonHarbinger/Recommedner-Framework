using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents basic functionality of a recommender system.
    /// </summary>
    public interface IRecommenderSystem
    {
        /// <summary>
        /// Returns whether system can predict users rating for any item.
        /// </summary>
        /// <param name="user">User whose rating we are interested in.</param>
        /// <returns>True, if there is item for which system can predict users rating.</returns>
        bool CanPredictForUser(User user);
        /// <summary>
        /// Returns whether system can predict any users rating for an item.
        /// </summary>
        /// <param name="item">Item for which we want users rating.</param>
        /// <returns>True, if system can predict rating of any user towards item.</returns>
        bool CanPredictForItem(Item item);
        /// <summary>
        /// Returns whether system can predict users rating for an item.
        /// </summary>
        /// <param name="user">User whose rating we are interested in.</param>
        /// <param name="item">Item for which we want users rating.</param>
        /// <returns>True, if system can predict rating.</returns>
        bool CanPredictRating(User user, Item item);

        /// <summary>
        /// Passes users feedback back to recommender system.
        /// </summary>
        /// <param name="feedback">Value of feedback.</param>
        void HandleFeedback(Feedback feedback);

        /// <summary>
        /// Returns users preference towards item as predicted by recommender system. 
        /// </summary>
        /// <param name="user">User, whose preference interest us.</param>
        /// <param name="item">Item, in which user might be interested.</param>
        /// <returns>Expected preference in the form of a float value.</returns>
        float GetExpectedRating(User user, Item item);
        /// <summary>
        /// Returns items ranked according to users preference towards them as predicted by recommender system.
        /// </summary>
        /// <param name="user">User, whose preference should be consdiered.</param>
        /// <param name="fromItems">Items to be ranked.</param>
        /// <returns>Returns list of items and users exptected preference towards them.</returns>
        List<RecommendedItem> GetRanking(User user, List<Item> fromItems);
        /// <summary>
        /// Returns items which might be of interest to user as predicted by recommender system.
        /// </summary>
        /// <param name="user">User, whose preference should be consdiered.</param>
        /// <param name="fromItems">Items to be considered for choice.</param>
        /// <param name="count">How many items are to be recommended.</param>
        /// <returns>Returns list of items and users exptected preference towards them.</returns>
        List<RecommendedItem> GetRecommendation(User user, List<Item> fromItems, int count);
    }
}
