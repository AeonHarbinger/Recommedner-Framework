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
        
        public SimpleProvider RegisterUser(int id)
        {
            string name = manager.RegisterUser(id);
            var tracker = manager.GetTrackerByName(name);
            var user = manager.Database.GetUser(id);

            return new SimpleProvider(user, tracker);
        }

        public List<Movie> GetMovies(string containing)
        {
            var movies = new List<Movie>();
            foreach (var item in manager.Database.Items.Values)
            {
                var movie = (Movie)item;
                if (movie.Name.Contains(containing)) movies.Add(movie);
                if (movies.Count >= 100) break;
            }

            return movies;
        }

        public float GetRating(int userId, int itemId)
        {
            if (manager.Database.PreferenceIsKnown(userId, itemId)) return manager.Database.KnownPreference[userId, itemId];
            else return -1;
        }
    }
}
