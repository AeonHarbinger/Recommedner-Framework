using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    public class SystemManager
    {
        /// <summary>
        /// Function which assigns a user a tracker that will provide him with recommendations.
        /// </summary>
        /// <param name="feedback">Identification of the user.</param>
        /// <returns>Name of the system tracker.</returns>
        public delegate string SystemAssignment(int userId);
        public SystemAssignment Assigner;

        Dictionary<string, IManagedRecommenderSystem> allSystems = new Dictionary<string, IManagedRecommenderSystem>();
        Dictionary<string, RecommenderSystemTracker> allTrackers = new Dictionary<string, RecommenderSystemTracker>();
        public Database Database;
        Random r = new Random(42);

        Dictionary<string, HashSet<int>> trackerServingUsers = new Dictionary<string, HashSet<int>>();

        /// <summary>
        /// Creates a system manager with specified database.
        /// </summary>
        /// <param name="data">Database with which the manager works.</param>
        public SystemManager(Database data)
        {
            Database = data;
        }

        #region Recommenders
        /// <summary>
        /// Adds a new recommender system to the manager.
        /// </summary>
        /// <param name="name">Identifier of the system.</param>
        /// <param name="system">System to be added.</param>
        public void AddRecommender(string name, IManagedRecommenderSystem system)
        {
            lock (allSystems)
                allSystems.Add(name, system);
        }
        /// <summary>
        /// Removes recommender from the manager.
        /// </summary>
        /// <param name="name">Name of the recommender to be removed.</param>
        public void RemoveRecommender(string name)
        {
            lock (allSystems)
                allSystems.Remove(name);
        }
        /// <summary>
        /// Returns recommender system with given identifier.
        /// </summary>
        /// <param name="name">Identifier of the system.</param>
        /// <returns>System with given identifier.</returns>
        public IManagedRecommenderSystem GetRecommenderByName(string name)
        {
            return allSystems[name];
        }
        /// <summary>
        /// Returns all available recommender systems.
        /// </summary>
        /// <returns>List containing pairs of recommender system and its identifier.</returns>
        public List<KeyValuePair<string, IManagedRecommenderSystem>> GetAvailableRecommenders()
        {
            return allSystems.ToList();
        }
        #endregion

        #region Trackers
        /// <summary>
        /// Adds a new tracker to the manager.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <param name="tracker">Tracker to be added.</param>
        public void AddTracker(string name, RecommenderSystemTracker tracker)
        {
            lock (allTrackers)
                allTrackers.Add(name, tracker);
            trackerServingUsers.Add(name, new HashSet<int>());
        }
        /// <summary>
        /// Removes recommender from the manager.
        /// </summary>
        /// <param name="name">Name of the recommender to be removed.</param>
        public void RemoveTracker(string name)
        {
            lock (allTrackers)
                allTrackers.Remove(name);
        }
        /// <summary>
        /// Returns tracker with given identifier.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>Tracker with given identifier.</redturns>
        public RecommenderSystemTracker GetTrackerByName(string name)
        {
            return allTrackers[name];
        }
        /// <summary>
        /// Returns all available trackers.
        /// </summary>
        /// <returns>List containing pairs of tracker and its identifier.</returns>
        public List<KeyValuePair<string, RecommenderSystemTracker>> GetAvailableTrackers()
        {
            return allTrackers.ToList();
        }
        #endregion

        #region User
        /// <summary>
        /// Returns how many users are registered to a certain tracker.
        /// </summary>
        /// <param name="name">Identifier of the tracker.</param>
        /// <returns>Number of users tracker is serving.</returns>
        public int ServingUsers(string name)
        {
            return trackerServingUsers[name].Count;
        }

        /// <summary>
        /// Registers that a user is being served by a system.
        /// </summary>
        /// <param name="id">Identification of a user.</param>
        /// <returns>Identification of tracker that will be providing him recommendations.</returns>
        public string RegisterUser(int id)
        {
            if (Assigner == null) throw new Exception("Function that assigns trackers to users wasn't provided.");

            var trackerName = Assigner(id);
            var set = trackerServingUsers[trackerName];

            lock (set)
                set.Add(id);

            return trackerName;
        }
        /// <summary>
        /// Marks user as not being provided to by a system.
        /// </summary>
        /// <param name="trackerName">Name of the tracker serving the user.</param>
        /// <param name="id">Identification of the user.</param>
        public void DeregisterUser(string trackerName, int id)
        {
            var set = trackerServingUsers[trackerName];

            lock (set)
                set.Remove(id);
        }
        #endregion
    }
}
