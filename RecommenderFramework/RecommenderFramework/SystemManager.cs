using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    public struct NameVersionPair
    {
        public string Name;
        public string Version;

        public NameVersionPair(string name, string version)
        {
            Name    = name;
            Version = version;
        }
    }

    /// <summary>
    /// Class for keeping track of recommenders and their trackers.
    /// </summary>
    public class SystemManager
    {
        /// <summary>
        /// Method which assigns a tracker to a user.
        /// </summary>
        /// <param name="userId">Identification of the user.</param>
        /// <returns>Identification of the system tracker.</returns>
        public delegate NameVersionPair SystemAssignment(int userId);
        /// <summary>
        /// Method used for assigning systems to users.
        /// </summary>
        private SystemAssignment assignerPrivate;
        public  SystemAssignment Assigner
        {
            get
            {
                return assignerPrivate;
            }
            set
            {
                assignerPrivate = value ?? throw new Exception("Function that assigns trackers to users wasn't provided.");
            }
        }
               
        /// <summary>
        /// All available recommender systems.
        /// </summary>
        Dictionary<NameVersionPair, IManagedRecommenderSystem> allSystems = new Dictionary<NameVersionPair, IManagedRecommenderSystem>();
        /// <summary>
        /// All available system trackers.
        /// </summary>
        Dictionary<NameVersionPair, RecommenderSystemTracker> allTrackers = new Dictionary<NameVersionPair, RecommenderSystemTracker >();
        /// <summary>
        /// Database containing all recommendation service data.
        /// </summary>
        public Database Database;

        /// <summary>
        /// Stores all users that are using recommender system. 
        /// </summary>
        readonly Dictionary<NameVersionPair, HashSet<int>> TrackerServingUsers = new Dictionary<NameVersionPair, HashSet<int>>();
        
        /// <summary>
        /// Creates a system manager.
        /// </summary>
        /// <param name="data">Database containing users, items, and recommendation data.</param>
        /// <param name="assigner">Function that assigns recommenders to users.</param>
        public SystemManager(Database data, SystemAssignment assigner)
        {
            Database = data;
            Assigner = assigner;
        }

        #region Recommenders
        /// <summary>
        /// Adds a new recommender system to the manager.
        /// </summary>
        /// <param name="name">Identifier of the system.</param>
        /// <param name="system">System to be added.</param>
        public void AddRecommender(IManagedRecommenderSystem system)
        {
            lock (allSystems)
                allSystems.Add(new NameVersionPair(system.Name, system.Version), system);
        }
        /// <summary>
        /// Removes recommender from the manager.
        /// </summary>
        /// <param name="id">Name of the recommender to be removed.</param>
        public void RemoveRecommender(NameVersionPair id)
        {
            lock (allSystems)
                allSystems.Remove(id);
        }
        /// <summary>
        /// Returns recommender system with given identifier.
        /// </summary>
        /// <param name="id">Identifier of the system.</param>
        /// <returns>System with given identifier.</returns>
        public IManagedRecommenderSystem GetRecommender(NameVersionPair id)
        {
            return allSystems[id];
        }
        /// <summary>
        /// Returns all available recommender systems.
        /// </summary>
        /// <returns>List containing pairs of recommender system and its identifier.</returns>
        public List<IManagedRecommenderSystem> GetAvailableRecommenders()
        {
            return allSystems.Values.ToList();
        }
        #endregion

        #region Trackers
                
        /*
        // MAKE THIS AUTOMATIC WHEN ADDING RS ??
        
        /// <summary>
        /// Adds a new tracker to the manager.
        /// </summary>
        /// <param name="id">Identifier of the tracker.</param>
        /// <param name="tracker">Tracker to be added.</param>
        public void AddTracker(NameVersionPair id, RecommenderSystemTracker tracker)
        {
            lock (allTrackers)
                allTrackers.Add(id, tracker);
            trackerServingUsers.Add(id, new HashSet<int>());
        }
        /// <summary>
        /// Removes recommender from the manager.
        /// </summary>
        /// <param name="id">Name of the recommender to be removed.</param>
        public void RemoveTracker(NameVersionPair id)
        {
            lock (allTrackers)
                allTrackers.Remove(id);
        }
        */


        /// <summary>
        /// Returns tracker with given identifier.
        /// </summary>
        /// <param name="id">Identifier of the tracker.</param>
        /// <returns>Tracker with given identifier.</redturns>        
        public RecommenderSystemTracker GetTracker(NameVersionPair id)
        {
            return allTrackers[id];
        }
        /// <summary>
        /// Returns all available trackers.
        /// </summary>
        /// <returns>List containing pairs of tracker and its identifier.</returns>
        public List<RecommenderSystemTracker> GetAvailableTrackers()
        {
            return allTrackers.Values.ToList();
        }
        #endregion

        #region User
        /// <summary>
        /// Returns how many users are registered to a certain tracker.
        /// </summary>
        /// <param name="id">Identification of the tracker.</param>
        /// <returns>Number of users tracker is serving.</returns>
        public int ServingUsersCount(NameVersionPair id)
        {
            return TrackerServingUsers[id].Count;
        }

        /// <summary>
        /// Registers that a user is being served by a system.
        /// </summary>
        /// <param name="userId">Identification of a user.</param>
        /// <returns>Identification of tracker that will be providing him recommendations.</returns>
        public NameVersionPair RegisterUser(int userId)
        {
            var trackerId = Assigner(userId);
            var set = TrackerServingUsers[trackerId];

            lock (set)
                set.Add(userId);

            return trackerId;
        }
        /// <summary>
        /// Marks user as not being provided to by a system.
        /// </summary>
        /// <param name="trackerId">Name of the tracker serving the user.</param>
        /// <param name="userId">Identification of the user.</param>
        public void DeregisterUser(NameVersionPair trackerId, int userId)
        {
            var set = TrackerServingUsers[trackerId];

            lock (set)
                set.Remove(userId);
        }
        #endregion
    }
}
