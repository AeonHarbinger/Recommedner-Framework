using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    public interface IRecommenderService
    {
        /// <summary>
        /// Notifies the system about a user and gives him a recommendation provider.
        /// </summary>
        /// <param name="userId">Identifier of the user.</param>
        /// <returns>Recommendation provider for the user.</returns>
        SimpleProvider RegisterUser(int userId);
        /// <summary>
        /// Notifies the system that the user is ending their session.
        /// </summary>
        /// <param name="name">Name of the tracker users provider is calling.</param>
        /// <param name="userId">Identifier of the user.</param>
        void DeregisterUser(string name, int userId);
        /// <summary>
        /// Gets up to 100 movies whose name contains specific string. 
        /// </summary>
        /// <param name="containing">String the names of the movies must contain.</param>
        /// <returns>List of movies whose name contain the string.</returns>
        List<Movie> GetMovies(string containing);
        /// <summary>
        /// Gets list of movies rated by the user.
        /// </summary>
        /// <param name="userId">Identifier of the tracker.</param>
        /// <returns>List of movies rated by the user.</returns>
        List<Movie> RatedByUser(int userId);
        /// <summary>
        /// Gets rating a user has given to an item.
        /// </summary>
        /// <param name="userId">Identifier of the user.</param>
        /// <param name="itemId">Identifier of the item.</param>
        /// <returns>-1 if user hasn't rated an item.</returns>
        float GetRating(int userId, int itemId);

        /// <summary>
        /// Return all available recommenders.
        /// </summary>
        /// <returns>List of all recommenders available.</returns>
        List<IManagedRecommenderSystem> GetAvailableRecommenders();
        /// <summary>
        /// Returns recommender with given name.        
        /// </summary>
        /// <param name="name">Identifier of the recommender.</param>
        /// <returns>Recommender with given name.</returns>
        IManagedRecommenderSystem GetRecommenderByName(string name);
        /// <summary>
        /// Returns tracker with given name.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>Tracker with given name.</returns>
        RecommenderSystemTracker GetTrackerByName(string name);
        /// <summary>
        /// Returns the user coverage of given system.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>User coverage of the system.</returns>
        double UserCoverage(string name);
        /// <summary>
        /// Returns the item coverage of given system.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>Item coverage of the system.</returns>
        double ItemCoverage(string name);
        /// <summary>
        /// Returns the number of users a tracker is currently serving.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>How many users the tracker is serving.</returns>
        int TrackerUserCount(string name);
        /// <summary>
        /// Returs user with specified id.
        /// </summary>
        /// <param name="id">Identification of the user.</param>
        /// <returns>User with specified id.</returns>
        User GetUser(int id);
    }
}
