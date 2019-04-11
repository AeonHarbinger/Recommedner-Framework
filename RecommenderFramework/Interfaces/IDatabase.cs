using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents basic functionality of recommendation database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Gets user with given id.
        /// </summary>
        /// <param name="id">Identification of the user.</param>
        /// <returns>User with given id.</returns>
        User GetUser(int id);
        /// <summary>
        /// Gets item with given id.
        /// </summary>
        /// <param name="id">Identification of the item.</param>
        /// <returns>Item with given id.</returns>
        Item GetItem(int id);

        /// <summary>
        /// Gets all feedback the user provided for the item.
        /// </summary>
        /// <param name="userId">Identification of the user.</param>
        /// <param name="itemId">Identification of the item.</param>
        /// <returns>Feedback the user provided for item.</returns>
        List<Feedback> GetFeedback(int userId, int itemId);
        /// <summary>
        /// Get all feedback the user provided.
        /// </summary>
        /// <param name="userId">Identification of the user.</param>
        /// <returns>Feedback the user provided.</returns>
        List<Feedback> GetUserFeedback(int userId);
        /// <summary>
        /// Get all feedback provided for an item.
        /// </summary>
        /// <param name="itemId">Identification of the item.</param>
        /// <returns>Feedback provided for an item.</returns>
        List<Feedback> GetItemFeedback(int itemId);

        /// <summary>
        /// Save a recommendation to the database. 
        /// </summary>
        /// <param name="name">Name of the recommender system.</param>
        /// <param name="version">Version of the recommender system.</param>
        /// <param name="rec">Recommendation to be saved.</param>
        void SaveSystemRecommendation(string name, string version, Recommendation rec);
        /// <summary>
        /// Retrieves recommendations from the database. 
        /// </summary>
        /// <param name="name">Name of the recommender system.</param>
        /// <param name="version">Version of the recommender system.</param>
        /// <returns>Recommendations to be retrieved.</returns>
        List<Recommendation> GetSystemRecommendations(string name, string version);
    }
}
