using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents item that has been recommended to the user accompanied by expected preference for evaluation.
    /// </summary>
    [Serializable]
    public class RecommendedItem
    {
        /// <summary>
        /// Item which was recommended.
        /// </summary>
        public Item Item;
        /// <summary>
        /// Users preference for item as expected by the recommender.
        /// </summary>
        public float ExpectedPreference;

        /// <summary>
        /// Given a list of items and their expected preferences, creates a list of RecommendedItem.
        /// </summary>
        /// <param name="database">Database containing items.</param>
        /// <param name="list">List of pairs containing item ID and expected preference.</param>
        /// <returns>List of RecommendedItems.</returns>
        public static List<RecommendedItem> MakeList(Database database, IList<Tuple<int, float>> list)
        {
            var result = new List<RecommendedItem>();
            foreach (var tuple in list)
            {
                Item item = database.GetItem(tuple.Item1);
                result.Add(new RecommendedItem() { Item = item, ExpectedPreference = tuple.Item2 });
            }

            return result;
        }
    }
}
