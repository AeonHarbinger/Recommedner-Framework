using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    class RecommenderService : MarshalByRefObject, IRecommenderService
    {
        SystemManager manager;

        public RecommenderService()
        {
            manager = Program.Manager;
        }

        /// <inheritdoc />
        public SimpleProvider RegisterUser(int userId)
        {
            string name = manager.RegisterUser(userId);
            var tracker = manager.GetTrackerByName(name);
            var user = manager.Database.GetUser(userId);

            Logger.Message(0, $"User {userId} registered to {name}");
            return new SimpleProvider(user, manager.Database.Items.Values, tracker);
        }
        /// <inheritdoc />
        public void DeregisterUser(string name, int userId)
        {
            manager.DeregisterUser(name, userId);
            Logger.Message(0, $"User {userId} deregistered from {name}");
        }
        /// <inheritdoc />
        public List<Movie> GetMovies(string containing)
        {
            var movies = new List<Movie>();
            foreach (var item in manager.Database.Items.Values)
            {
                var movie = (Movie)item;
                if (movie.Name.Contains(containing))
                {
                    var clone = movie.Clone();
                    clone.LoadPoster();
                    movies.Add(clone);
                }
                if (movies.Count >= 100) break;
            }

            return movies;
        }
        /// <inheritdoc />
        public List<Movie> RatedByUser(int userId)
        {
            var movies = new List<Movie>();
            foreach (var itemId in manager.Database.KnownForUser(userId).Keys)
            {
                var clone = ((Movie)manager.Database.GetItem(itemId)).Clone();
                clone.LoadPoster();
                movies.Add(clone);

                if (movies.Count >= 100) break;
            }

            return movies;
        }
        /// <inheritdoc />
        public float GetRating(int userId, int itemId)
        {
            if (manager.Database.PreferenceIsKnown(userId, itemId)) return manager.Database[userId, itemId];
            else return -1;
        }



        /// <inheritdoc />
        public IManagedRecommenderSystem GetRecommenderByName(string name)
        {
            return manager.GetRecommenderByName(name);
        }
        /// <inheritdoc />
        public List<IManagedRecommenderSystem> GetAvailableRecommenders()
        {
            var recs = manager.GetAvailableRecommenders();
            var result = new List<IManagedRecommenderSystem>();
            foreach (var nameRecPair in recs)
            {
                result.Add(nameRecPair.Value);
            }
            return result;
        }
        /// <inheritdoc />
        public User GetUser(int id)
        {
            return manager.Database.Users[id];
        }
        /// <inheritdoc />
        public RecommenderSystemTracker GetTrackerByName(string name)
        {
            return manager.GetTrackerByName(name);
        }
        /// <inheritdoc />
        public double UserCoverage(string rst)
        {
            var allUsers = new List<User>();
            foreach (var user in manager.Database.Users.Values)
            {
                allUsers.Add(user);
            }
            return manager.GetTrackerByName(rst).UserCoverage(allUsers);
        }
        /// <inheritdoc />
        public double ItemCoverage(string rst)
        {
            var allItems = new List<Item>();
            foreach (var item in manager.Database.Items.Values)
            {
                allItems.Add(item);
            }
            return manager.GetTrackerByName(rst).ItemCoverage(allItems);
        }
        /// <inheritdoc />
        public int TrackerUserCount(string rst)
        {
            return manager.ServingUsers(rst);
        }
    }
}
