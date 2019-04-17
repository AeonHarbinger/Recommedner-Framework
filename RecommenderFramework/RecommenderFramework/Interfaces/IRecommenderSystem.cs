using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents basic functionality expected of a recommender system.
    /// </summary>
    public interface IRecommenderSystem
    {
        /// <summary>
        /// Name of the system.
        /// </summary>
        string Name    { get; }
        /// <summary>
        /// Version of the system.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Returns whether the system can predict user's rating for any item.
        /// </summary>
        /// <param name="user">User whose recommendation we are interested in.</param>
        /// <returns>True, if the system can recommend an item.</returns>
        bool CanRecommendToUser(User user);
        /// <summary>
        /// Returns whether system can recommend this item to any user.
        /// </summary>
        /// <param name="item">Item whose recommend-ability we are interested in.</param>
        /// <returns>True, if the system can recommend this item.</returns>
        bool CanRecommendItem(Item item);
        /// <summary>
        /// Returns whether the system can predict user's rating for an item.
        /// </summary>
        /// <param name="user">User whose rating we are interested in.</param>
        /// <param name="item">Item for which we want user's rating.</param>
        /// <returns>True, if system can predict rating.</returns>
        bool CanPredictPreference(User user, Item item);

        /// <summary>
        /// Passes user's feedback back to recommender system.
        /// </summary>
        /// <param name="feedback">Feedback received from a user.</param>
        void HandleFeedback(Feedback feedback);

        /// <summary>
        /// Returns user's preference for item as predicted by the recommender system. 
        /// </summary>
        /// <param name="user">User, whose preference interest us.</param>
        /// <param name="item">Item, in which user might be interested.</param>
        /// <returns>Expected preference expressed in a numerical value.</returns>
        float GetExpectedPreference(User user, Item item);
        /// <summary>
        /// Returns items ranked according to user's preference for them as predicted by the recommender system.
        /// </summary>
        /// <param name="user">User, whose preference should be consdiered.</param>
        /// <param name="fromItems">Items to be ranked.</param>
        /// <returns>Returns list of items and user's exptected preference for them.</returns>
        List<RecommendedItem> GetRanking(User user, List<Item> fromItems);
        /// <summary>
        /// Returns items which might be of interest to user as predicted by the recommender system.
        /// </summary>
        /// <param name="user">User, whose preference should be consdiered.</param>
        /// <param name="fromItems">Items to be considered for choice.</param>
        /// <param name="count">How many items should be recommended.</param>
        /// <returns>List of items and user's exptected preference for them.</returns>
        List<RecommendedItem> GetRecommendation(User user, List<Item> fromItems, int count);
    }
}
