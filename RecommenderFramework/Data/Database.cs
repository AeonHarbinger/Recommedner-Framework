using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Implementation of a database for recommender service.
    /// </summary>
    public class Database : IDatabase
    {
        /// <summary>
        /// Table of users.
        /// </summary>
        public Table<User> Users;
        /// <summary>
        /// Table of items.
        /// </summary>
        public Table<Item> Items;
        /// <summary>
        /// Tables of Feedback.
        /// </summary>
        public List<Table<Feedback>> Feedback;

        /// <summary>
        /// Database containing recommendation data.
        /// </summary>
        internal DbRecommendationContext RecContext;

        /// <summary>
        /// Creates new with given table of users, items and feedback. 
        /// Sets up recommendation tables in specified database (if not available already).
        /// </summary>
        /// <param name="conStr">Connection string for the database.</param>
        /// <param name="users">Table of users.</param>
        /// <param name="items">Table of items.</param>
        /// <param name="feedback">Tables of Feedback.</param>
        public Database(string conStr, Table<User> users, Table<Item> items, List<Table<Feedback>> feedback)
        {
            Users    = users;
            Items    = items;
            Feedback = feedback;

            RecContext = new DbRecommendationContext(conStr);
        }

        /// <inheritdoc />
        public User GetUser(int id) => (from u in Users where u.Id == id select u).FirstOrDefault();
        /// <inheritdoc />
        public Item GetItem(int id) => (from i in Items where i.Id == id select i).FirstOrDefault();

        /// <inheritdoc />
        public List<Feedback> GetFeedback(int userId, int itemId)
        {
            var userFeedback = new List<Feedback>();
            foreach (var feedbackTable in Feedback)
            {
                var thisf = from f in feedbackTable where f.UserId == userId && f.ItemId == itemId select f;
                userFeedback.AddRange(thisf);
            }
            return userFeedback;
        }
        /// <inheritdoc />
        public List<Feedback> GetUserFeedback(int userId)
        {
            var userFeedback = new List<Feedback>();
            foreach (var feedbackTable in Feedback)
            {
                var thisf = from f in feedbackTable where f.UserId == userId select f;
                userFeedback.AddRange(thisf);
            }
            return userFeedback;
        }
        /// <inheritdoc />
        public List<Feedback> GetItemFeedback(int itemId)
        {
            var userFeedback = new List<Feedback>();
            foreach (var feedbackTable in Feedback)
            {
                var thisf = from f in feedbackTable where f.ItemId == itemId select f;
                userFeedback.AddRange(thisf);
            }
            return userFeedback;
        }

        /// <inheritdoc />
        public void SaveSystemRecommendation(string rsName, string rsVersion, Recommendation rl)
        {
            // TODO make thread safe get maxid, increment
            int maxId = (from r in RecContext.Recommendations select r.Id).Max();

            var dbrl = new DbRecommendation(maxId + 1, rsName, rsVersion, rl);
            RecContext.Recommendations.InsertOnSubmit(dbrl);

            foreach (var item in rl.Items)
                RecContext.RecommendedItems.InsertOnSubmit(new DbRecommendedItem(dbrl.Id, item));
            
            RecContext.SubmitChanges();
        }
        /// <inheritdoc />
        public List<Recommendation> GetSystemRecommendations(string rsName, string rsVersion)
        {
            var listIds = from rl in RecContext.Recommendations where rl.Name == rsName && rl.Version == rsVersion select rl.Id;

            var recommendations = new List<Recommendation>();
            foreach (var listId in listIds)
            {
                DbRecommendation dbList = (from rl in RecContext.Recommendations where rl.Id == listId select rl).First();

                var recItems = new List<RecommendedItem>();
                var dbItems = from ri in RecContext.RecommendedItems where ri.RecommendationId == listId select ri;
                foreach (var item in dbItems)
                {
                    recItems.Add(new RecommendedItem() {
                        Item = GetItem(item.ItemId),
                        ExpectedPreference = item.ExpectedPreference
                    });
                }

                recommendations.Add(new Recommendation(dbList.UserId, recItems, dbList.AtTime, dbList.ResponseTime));
            }

            return recommendations;
        }
    }
}

