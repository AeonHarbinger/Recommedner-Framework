using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    public class SimpleProvider : MarshalByRefObject
    {
        public User user { get; private set; }
        IRecommenderSystem system;
        
        public SimpleProvider(User u, IRecommenderSystem sys)
        {
            user = u;
            system = sys;
        }

        public bool CanPredict() => system.CanPredictForUser(user);
        public bool CanPredictRating(Item item) => system.CanPredictRating(user, item);

        public float GetRating(Item item) => system.GetExpectedRating(user, item);
        public void GiveFeedback(Feedback feedback) => system.GiveFeedback(feedback);

        public List<Movie> GetRanking(List<Item> fromItems)
        {
            return JustTheItems(system.GetRanking(user, fromItems));
        }
        public List<Movie> GetRanking(Predicate<Item> satisfies)
        {
            // This cannot download the whole database
            throw new NotImplementedException();

            /*
            var filtered = new List<Item>();
            foreach (var item in database.Items)
            {
                if (satisfies(item.Value)) filtered.Add(item.Value);
            }
            return JustTheItems(system.GetRanking(user, filtered));
            */
        }

        public List<Movie> GetRecommendation(int count)
        {
            return JustTheItems(system.GetRecommendation(user, null, count));
        }
        public List<Movie> GetRecommendation(List<Item> fromItems, int count)
        {
            return JustTheItems(system.GetRecommendation(user, fromItems, count));
        }
        public List<Movie> GetRecommendation(Predicate<Item> satisfies, int count)
        {
            throw new NotImplementedException();

            /*
            var filtered = new List<Item>();
            foreach (var item in database.Items)
            {
                if (satisfies(item.Value)) filtered.Add(item.Value);
            }
            return JustTheItems(system.GetRecommendation(user, filtered, count));
            */
        }

        List<Movie> JustTheItems(List<RecommendedItem> recommendedItems)
        {
            var items = new List<Movie>();
            foreach (var recItem in recommendedItems)
            {
                var movie = (Movie)recItem.Item;

                items.Add(movie);
            }
            return items;
        }
    }
}
