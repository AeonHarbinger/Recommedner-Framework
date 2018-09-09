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
        /// <summary>
        /// User this object provides for.
        /// </summary>
        public User User   { get; private set; }
        /// <summary>
        /// Name of the system.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// List of all items in database.
        /// </summary>
        readonly ICollection<Item> allItems;
        /// <summary>
        /// System from which we get recommendations.
        /// </summary>
        IRecommenderSystem system;
        
        /// <summary>
        /// Creates new instance with specified parameters.
        /// </summary>
        /// <param name="u">User to which this Provider belongs to.</param>
        /// <param name="items">Items in the database.</param>
        /// <param name="sys">System that provides user with recommendations.</param>
        public SimpleProvider(User u, ICollection<Item> items, RecommenderSystemTracker sys)
        {
            User = u;
            system = sys;
            allItems = items;
            Name = sys.Name;
        }

        /// <summary>
        /// All of these functions just call their RecommenderSystem equivalents.
        /// When necessary they filter all items for only those that user is interested in (based on given predicate).
        /// </summary>
        #region Calling recommender functions
        public bool CanPredict() => system.CanPredictForUser(User);
        public bool CanPredictRating(Item item) => system.CanPredictRating(User, item);

        public float GetRating(Item item) => system.GetExpectedRating(User, item);
        public void HandleFeedback(Feedback feedback) => system.HandleFeedback(feedback);

        public List<Movie> GetRanking(List<Item> fromItems)
        {
            return JustTheItems(system.GetRanking(User, fromItems));
        }
        public List<Movie> GetRanking(Predicate<Item> satisfies)
        {
            var itemsThatSatisfy = new List<Item>();
            foreach (var item in allItems)
            {
                if (satisfies(item)) itemsThatSatisfy.Add(item);
            }
            return JustTheItems(system.GetRanking(User, itemsThatSatisfy));
        }

        public List<Movie> GetRecommendation(int count)
        {
            return JustTheItems(system.GetRecommendation(User, null, count));
        }
        public List<Movie> GetRecommendation(List<Item> fromItems, int count)
        {
            return JustTheItems(system.GetRecommendation(User, fromItems, count));
        }
        public List<Movie> GetRecommendation(Predicate<Item> satisfies, int count)
        {
            var itemsThatSatisfy = new List<Item>();
            foreach (var item in allItems)
            {
                if (satisfies(item)) itemsThatSatisfy.Add(item);
            }
            return JustTheItems(system.GetRecommendation(User, itemsThatSatisfy, count));
        }
        #endregion

        /// <summary>
        /// Takes the output of the recommender and filters out the expected preference. Loads posters for movies. 
        /// </summary>
        /// <param name="recommendedItems">List of items and expected preferences returned by system.</param>
        /// <returns>List of movies with posters (where available).</returns>
        List<Movie> JustTheItems(List<RecommendedItem> recommendedItems)
        {
            var items = new List<Movie>();
            foreach (var recItem in recommendedItems)
            {
                var movie = (Movie)recItem.Item;
                var clone = movie.Clone();
                clone.LoadPoster();
                items.Add(clone);
            }
            return items;
        }
    }
}
