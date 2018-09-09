using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    public class RecommenderSystemTracker : MarshalByRefObject, IRecommenderSystem
    {
        /// <summary>
        /// Function which based on feedback that user gave an item guesses users preference toward item.
        /// </summary>
        /// <param name="feedback">List of feedback that user gave an item.</param>
        /// <returns>Preference guess based on feedback. Null if no meaningful preference can be guessed.</returns>
        public delegate float? AggregationFunction(List<Feedback> feedback);
        /// <summary>
        /// Function used to guess users preference for an item during evaluation.
        /// </summary>
        public AggregationFunction Aggregator { private get; set; } = null;

        /// <summary>
        /// Name of the tracker.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Recommender system which is being tracked.
        /// </summary>
        IRecommenderSystem system;
        /// <summary>
        /// All recommendations the system provided.
        /// </summary>
        public SafeLinkedList<RecommendationList> AllRecommendations;
        /// <summary>
        /// All feedback given to the system.
        /// </summary>
        public SafeLinkedList<Feedback> AllFeedback;
        
        /// <summary>
        /// Creates new instance of tracker with specified parameters.
        /// </summary>
        /// <param name="name">Name of the tracker.</param>
        /// <param name="sys">System which is being tracked.</param>
        public RecommenderSystemTracker(string name, IRecommenderSystem sys)
        {
            Name = name;
            system = sys;
            AllRecommendations = new SafeLinkedList<RecommendationList>();
            AllFeedback        = new SafeLinkedList<Feedback>();
        }

        /// <summary>
        /// Adds feedback to tracker.
        /// </summary>
        /// <param name="feed">List of feedback to be added.1</param>
        public void AddFeedback(List<Feedback> feed)
        {
            foreach (var f in feed)
            {
                AllFeedback.Add(f);
            }
        }



        #region Recommendation
        
        /// <inheritdoc />
        public bool CanPredictForUser(User user) => system.CanPredictForUser(user);
        /// <inheritdoc />
        public bool CanPredictForItem(Item item) => system.CanPredictForItem(item);
        /// <inheritdoc />
        public bool CanPredictRating(User user, Item item) => system.CanPredictRating(user, item);
        
        /// <summary>
        /// Saves feedback provided by user and passes it to the system.
        /// </summary>
        /// <param name="feedback">Feedback from user.</param>
        public void HandleFeedback(Feedback feedback)
        {
            AllFeedback.Add(feedback);
            system.HandleFeedback(feedback);
        }

        /// <inheritdoc />
        public float GetExpectedRating(User user, Item item) => system.GetExpectedRating(user, item);
        /// <summary>
        /// Gets ranking of items from system. Saves ranking with additional data for evaluation.
        /// </summary>
        /// <param name="user">User to be recommended to.</param>
        /// <param name="fromItems">Items to be ranked.</param>
        /// <returns>Items ranked according to their relevance.</returns>
        public List<RecommendedItem> GetRanking(User user, List<Item> fromItems)
        {
            var timer = Stopwatch.StartNew();
            var recommendation = system.GetRanking(user, fromItems);
            timer.Stop();

            var newList = new RecommendationList(user.Id, recommendation, DateTime.Now, (int)timer.ElapsedMilliseconds);
            AllRecommendations.Add(newList);
            return recommendation;
        }
        /// <summary>
        /// Gets recommendation of items from system. Saves recommendation with additional data for evaluation.
        /// </summary>
        /// <param name="user">User to be recommended to.</param>
        /// <param name="fromItems">Items to be considered for recommendation.</param>
        /// <param name="count">How many items should be recommended. -1 if as many as possible</param>
        /// <returns>Items that might be relevant to user.</returns>
        public List<RecommendedItem> GetRecommendation(User user, List<Item> fromItems, int count)
        {
            var timer = Stopwatch.StartNew();
            var recommendation = system.GetRecommendation(user, fromItems, count);
            timer.Stop();

            var newList = new RecommendationList(user.Id, recommendation, DateTime.Now, (int)timer.ElapsedMilliseconds);
            AllRecommendations.Add(newList);
            return recommendation;
        }
        #endregion



        #region Manipulation
        /// <summary>
        /// Saves recommendations into a specified file.
        /// </summary>
        /// <param name="fileName">Name of the file to which recommendations should be saved.</param>
        public void SaveRecommendations(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(Name);

                foreach (var rec in AllRecommendations.Values())
                {
                    Console.WriteLine(rec.UserId + "|" + rec.AtTime + "|" + rec.ResponseTimeMs);
                    Console.WriteLine(rec.Items.Count);

                    foreach (var item in rec.Items)
                    {
                        Console.WriteLine(item.Item.Id + ":" + item.ExpectedPreference);
                    }
                }
            }
        }
        /// <summary>
        /// Loads recommendations from a specified file.
        /// </summary>
        /// <param name="fileName">Name of the file from which recommendations should be loaded.</param>
        public void LoadRecommendations(string fileName, Database data)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                if (Name != reader.ReadLine()) throw new Exception("Recommender name doesn't match first line of file.");

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split('|');
                    var userId = int.Parse(split[0]);
                    var atTime = DateTime.Parse(split[1]);
                    var response = int.Parse(split[2]);
                    int count = int.Parse(reader.ReadLine());

                    var list = new List<RecommendedItem>();
                    for (int i = 0; i < count; i++)
                    {
                        var item = reader.ReadLine().Split(':');
                        var itemId = int.Parse(item[0]);
                        var pref = float.Parse(item[1]);
                        list.Add(new RecommendedItem() { Item = data.GetItem(itemId), ExpectedPreference = pref });
                    }

                    var recList = new RecommendationList(userId, list, atTime, response);
                    AllRecommendations.Add(recList);
                }
            }
        }
        #endregion



        #region Evaluation
        /// <summary>
        /// Calculates mean absolute error of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Mean absolute error of recommendations by tracked recommender.</returns>
        public double MAE()
        {
            var pref = GetAllPreferences();
            return Evaluation.MAE(AllRecommendations.Values(), pref);
        }
        /// <summary>
        /// Calculates root mean square error of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Root mean square error of recommendations by tracked recommender.</returns>
        public double RMSE()
        {
            var pref = GetAllPreferences();
            return Evaluation.RMSE(AllRecommendations.Values(), pref);
        }

        /// <summary>
        /// Calculates the mean average reciprocal hit rank of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Average reciprocal hit rank of recommendations by tracked recommender.</returns>
        public double ARHR(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.ARHR(AllRecommendations.Values(), IsItemRelevant);
        }

        /// <summary>
        /// Calculates the average discounted cumulative gain of recommendations by tracked recommender.
        /// </summary>
        /// <param name="rankPosition">At which rank position will the DCG be calculated.</param>
        /// <returns>Average discounted cumulative gain of recommendations by tracked recommender.</returns>
        public double AverageDCG(int rankPosition)
        {
            var pref = GetAllPreferences();
            return Evaluation.AverageDCG(AllRecommendations.Values(), pref, rankPosition);
        }
        /// <summary>
        /// Calculates the average normalized discounted cumulative gain of recommendations by tracked recommender.
        /// </summary>
        /// <param name="rankPosition">At which rank position will the NDCG be calculated.</param>
        /// <returns>Average normalized discounted cumulative gain of recommendations by tracked recommender.</returns>
        public double AverageNDCG(int rankPosition)
        {
            var pref = GetAllPreferences();
            return Evaluation.AverageNDCG(AllRecommendations.Values(), pref, rankPosition);
        }

        /// <summary>
        /// Calculates the click-through rate of recommendations by tracked recommender (aka. the (# ClickOnRecommendations / # RecommendedItemsDisplayed))
        /// </summary>
        /// <returns>Click through rate of recommendations by tracked recommender.</returns>
        public double CTR()
        {
            return Evaluation.CTR(AllRecommendations.Values(), AllFeedback.Values());
        }

        /// <summary>
        /// Calculates what portion of users can the tracked recommender predict rating for.
        /// </summary>
        /// <param name="users">List of all users.</param>
        /// <returns>Portion of users for which some rating can be predicted.</returns>
        public double UserCoverage(List<User> users)
        {
            return Evaluation.UserCoverage(users, system);
        }
        /// <summary>
        /// Calculates what portion of items can the tracked recommender predict rating for.
        /// </summary>
        /// <param name="items">List of all items.</param>
        /// <returns>Portion of items for which some rating can be predicted.</returns>
        public double ItemCoverage(List<Item> items)
        {
            return Evaluation.ItemCoverage(items, system);
        }

        /// <summary>
        /// Calculates the average diversity of items of within one recommendation by tracked recommender.
        /// </summary>
        /// <param name="difference">Function that tells us how much two items differ.</param>
        /// <returns>Average diversity of a recommendation list by tracked recommender.</returns>
        public double AverageListDiversity(Func<Item, Item, double> difference)
        {
            return Evaluation.AverageListDiversity(AllRecommendations.Values(), difference);
        }

        /// <summary>
        /// Calculates the recall of recommendations by tracked recommender.
        /// </summary>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <returns>Recall of recommendations by tracked recommender.</returns>
        public double Recall(Func<int, List<int>> GetRelevantItems)
        {
            return Evaluation.Recall(AllRecommendations.Values(), GetRelevantItems);
        }
        /// <summary>
        /// Calculates the accuracy of recommendations by tracked recommender.
        /// </summary>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <param name="numberOfItems">Total number of items.</param>
        /// <returns>Accuracy of recommendations by tracked recommender.</returns>
        public double Accuracy(Func<int, List<int>> GetRelevantItems, int numberOfItems)
        {
            return Evaluation.Accuracy(AllRecommendations.Values(), GetRelevantItems, numberOfItems);
        }
        /// <summary>
        /// Calculates the precision of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Precision of recommendations by tracked recommender.</returns>
        public double Precision(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.Precision(AllRecommendations.Values(), IsItemRelevant);
        }
        /// <summary>
        /// Calculates the mean average precision of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Mean average precision of recommendations by tracked recommender.</returns>
        public double MAP(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.MAP(AllRecommendations.Values(), IsItemRelevant);
        }

        /// <summary>
        /// Caclulates the mean response time of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Mean response time in miliseconds.</returns>
        public double MeanResponseTime()
        {
            return Evaluation.MeanResponseTime(AllRecommendations.Values());
        }
        /// <summary>
        /// Caclulates the median response time of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Median response time in miliseconds.</returns>
        public double MedianResponseTime()
        {
            return Evaluation.MedianResponseTime(AllRecommendations.Values());
        }



        /// <summary>
        /// Returns preferences from feedback saved in tracker.
        /// </summary>
        /// <param name="agg">Function used for aggregating when implicit feedback is present.</param>
        /// <returns>List containing users preference for given item.</returns>
        List<UserItemPreference> GetAllPreferences()
        {
            bool explicitOnly = true;
            var feed = AllFeedback.Values();
            foreach (var item in feed)
            {
                if (item is ImplicitFeedback)
                {
                    explicitOnly = false;
                    break;
                }
            }

            if (explicitOnly) return PreferenceFromExplicit(feed);
            else
            {
                if (Aggregator == null) throw new Exception("Tracker contains implicit feedback but no aggregator was provided.");
                return PreferenceFromImplicit(feed, Aggregator);
            }
        }
        /// <summary>
        /// Returns preferences from feedback when it only contains explicit feedback.
        /// </summary>
        /// <param name="feed">Feedback from which preference is extracted.</param>
        /// <returns>List containing users preference for given item.</returns>
        List<UserItemPreference> PreferenceFromExplicit(List<Feedback> feed)
        {
            var pref = new Dictionary<UserItemPair, float>();
            foreach (var item in feed)
            {
                var expl = (ExplicitFeedback)item;
                var ui = new UserItemPair(expl.UserId, expl.ItemId);
                if (pref.ContainsKey(ui)) pref[ui] = expl.Preference;
                else pref.Add(ui, expl.Preference);
            }

            var preference = new List<UserItemPreference>();
            foreach (var item in pref)
            {
                preference.Add(new UserItemPreference(item.Key, item.Value));
            }

            return preference;
        }
        /// <summary>
        /// Returns preferences from feedback when it also contains implicit feedback.
        /// </summary>
        /// <param name="feed">Feedback from which preference is extracted.</param>
        /// <param name="aggregate">Function used for aggregating users feedback.</param>
        /// <returns>List containing users preference for given item.</returns>
        List<UserItemPreference> PreferenceFromImplicit(List<Feedback> feed, AggregationFunction aggregate)
        {
            var forUserAndItem = new Dictionary<UserItemPair, List<Feedback>>();
            foreach (var item in feed)
            {
                var uip = new UserItemPair(item.UserId, item.ItemId);
                if (!forUserAndItem.TryGetValue(uip, out List<Feedback> userItemFeed))
                {
                    userItemFeed = new List<Feedback>();
                    forUserAndItem.Add(uip, userItemFeed);
                }

                userItemFeed.Add(item);
            }

            var preference = new List<UserItemPreference>();
            foreach (var uip in forUserAndItem)
            {
                var prefForThisUserItemPair = aggregate(uip.Value);
                if (prefForThisUserItemPair != null) preference.Add(new UserItemPreference(uip.Key, (float)prefForThisUserItemPair));
            }
            return preference;
        }
        #endregion
    }
}
