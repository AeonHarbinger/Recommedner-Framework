using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Wrapper for recommenders system. Tracks recommendations and feedback.
    /// </summary>
    public class RecommenderSystemTracker : IRecommenderSystem
    {
        /// <summary>
        /// Function used to guess users preference for an item during evaluation.
        /// </summary>
        public Func<List<Feedback>, float?> Aggregator { private get; set; } = null;
        public Action<Feedback> AddFeedbackToDatabase  { private get; set; } = null;

        /// <summary>
        /// Name of the tracker.
        /// </summary>
        public string Name    => system.Name;
        /// <summary>
        /// Version of the tracker.
        /// </summary>
        public string Version => system.Version;

        /// <summary>
        /// Database containing all recommendation service data.
        /// </summary>
        readonly Database database;
        /// <summary>
        /// Recommender system which is being tracked.
        /// </summary>
        IRecommenderSystem system;


        /// <summary>
        /// Return all recommendations provided by this system.
        /// </summary>
        /// <returns>All recommendations provided by this system.</returns>
        List<Recommendation> GetAllRecommendations() => database.GetSystemRecommendations(Name, Version);
        /// <summary>
        /// Retrun all feedback relevant to provided recommendations.
        /// </summary>
        /// <param name="recommendations">Only return feedback of users for items that were recommended to them.</param>
        /// <returns>All feedback relevant to provided recommendations.</returns>
        List<Feedback> GetAllFeedback(List<Recommendation> recommendations)
        {
            var userItemMapping = new Dictionary<int, HashSet<int>>();
            foreach (var rec in recommendations)
                foreach (var recItem in rec.Items)
                {
                    if (!userItemMapping.ContainsKey(rec.UserId))
                        userItemMapping.Add(rec.UserId, new HashSet<int>());

                    userItemMapping[rec.UserId].Add(recItem.Item.Id);
                }

            var feedback = new List<Feedback>();
            foreach (var userId in userItemMapping.Keys)
                foreach (var itemId in userItemMapping[userId])
                    feedback.AddRange(database.GetFeedback(userId, itemId));

            return feedback;
        }


        Dictionary<UserItemPair, float?> GetPreferenceFromRecommendation(List<Recommendation> recoms)
        {
            if (Aggregator == null)
                throw new Exception("Aggregator function not set.");

            var prefs = new Dictionary<UserItemPair, float?>();

            foreach (var rec in recoms)
                foreach (var item in rec.Items)
                {
                    var uip = new UserItemPair(rec.UserId, item.Item.Id);
                    if (!prefs.ContainsKey(uip))
                    {
                        float? preference = Aggregator(database.GetFeedback(rec.UserId, item.Item.Id));
                        prefs.Add(uip, preference);
                    }
                }

            return prefs;
        }



        
        /// <summary>
        /// Creates new instance of tracker with specified parameters.
        /// </summary>
        /// <param name="sys">System which is being tracked.</param>
        /// <param name="db">Recommendation database.</param>
        /// <param name="feedbackAdder">Method for adding feedback to database.</param>
        public RecommenderSystemTracker(IRecommenderSystem sys, Database db, Action<Feedback> feedbackAdder)
        {
            system = sys;
            database = db;
            AddFeedbackToDatabase = feedbackAdder;
        }

        /// <summary>
        /// Adds feedback to tracker.
        /// </summary>
        /// <param name="feed">List of feedback to be added.1</param>
        public void AddFeedback(List<Feedback> feed)
        {
            foreach (var f in feed)
                AddFeedbackToDatabase(f);
        }



        #region Recommendation
        /// <inheritdoc />
        public bool CanRecommendToUser(User user) => system.CanRecommendToUser(user);
        /// <inheritdoc />
        public bool CanRecommendItem  (Item item) => system.CanRecommendItem(item);
        /// <inheritdoc />
        public bool CanPredictPreference(User user, Item item) => system.CanPredictPreference(user, item);
        
        /// <summary>
        /// Saves feedback provided by user and passes it to the system.
        /// </summary>
        /// <param name="feedback">Feedback from user.</param>
        public void HandleFeedback(Feedback feedback)
        {
            AddFeedbackToDatabase(feedback);
            system.HandleFeedback(feedback);
        }

        /// <inheritdoc />
        public float GetExpectedPreference(User user, Item item) => system.GetExpectedPreference(user, item);
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

            var recom = new Recommendation(user.Id, recommendation, DateTime.Now, (int)timer.ElapsedMilliseconds);
            database.SaveSystemRecommendation(Name, Version, recom);
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

            var recom = new Recommendation(user.Id, recommendation, DateTime.Now, (int)timer.ElapsedMilliseconds);
            database.SaveSystemRecommendation(Name, Version, recom);
            return recommendation;
        }
        #endregion


                     
        #region Evaluation
        /// <summary>
        /// Calculates mean absolute error of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Mean absolute error of recommendations by tracked recommender.</returns>
        public double MAE()
        {
            var recs = GetAllRecommendations();
            var pref = GetPreferenceFromRecommendation(recs);
            return Evaluation.MAE(recs, pref);
        }
        /// <summary>
        /// Calculates root mean square error of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Root mean square error of recommendations by tracked recommender.</returns>
        public double RMSE()
        {
            var recs = GetAllRecommendations();
            var pref = GetPreferenceFromRecommendation(recs);
            return Evaluation.RMSE(recs, pref);
        }

        /// <summary>
        /// Calculates the mean average reciprocal hit rank of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Average reciprocal hit rank of recommendations by tracked recommender.</returns>
        public double ARHR(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.MRR(GetAllRecommendations(), IsItemRelevant);
        }

        /// <summary>
        /// Calculates the average discounted cumulative gain of recommendations by tracked recommender.
        /// </summary>
        /// <param name="rankPosition">At which rank position will the DCG be calculated.</param>
        /// <returns>Average discounted cumulative gain of recommendations by tracked recommender.</returns>
        public double AverageDCG(int rankPosition)
        {
            var recs = GetAllRecommendations();
            var pref = GetPreferenceFromRecommendation(recs);
            return Evaluation.AverageDCG(GetAllRecommendations(), pref, rankPosition);
        }
        /// <summary>
        /// Calculates the average normalized discounted cumulative gain of recommendations by tracked recommender.
        /// </summary>
        /// <param name="rankPosition">At which rank position will the NDCG be calculated.</param>
        /// <returns>Average normalized discounted cumulative gain of recommendations by tracked recommender.</returns>
        public double AverageNDCG(int rankPosition)
        {
            var recs = GetAllRecommendations();
            var pref = GetPreferenceFromRecommendation(recs);
            return Evaluation.AverageNDCG(GetAllRecommendations(), pref, rankPosition);
        }

        /// <summary>
        /// Calculates the click-through rate of recommendations by tracked recommender (aka. the (# ClickOnRecommendations / # RecommendedItemsDisplayed))
        /// </summary>
        /// <returns>Click through rate of recommendations by tracked recommender.</returns>
        public double CTR()
        {
            var recs = GetAllRecommendations();
            var feedback = GetAllFeedback(recs);
            return Evaluation.CTR(recs, feedback);
        }

        /// <summary>
        /// Calculates what portion of users can the tracked recommender predict rating for.
        /// </summary>
        /// <param name="users">List of all users.</param>
        /// <returns>Portion of users for which some rating can be predicted.</returns>
        public double UserCoverage()
        {
            return Evaluation.UserCoverage(database.Users.ToList(), system);
        }
        /// <summary>
        /// Calculates what portion of items can the tracked recommender predict rating for.
        /// </summary>
        /// <param name="items">List of all items.</param>
        /// <returns>Portion of items for which some rating can be predicted.</returns>
        public double ItemCoverage()
        {
            return Evaluation.ItemCoverage(database.Items.ToList(), system);
        }

        /// <summary>
        /// Calculates the average diversity of items of within one recommendation by tracked recommender.
        /// </summary>
        /// <param name="difference">Function that tells us how much two items differ.</param>
        /// <returns>Average diversity of a recommendation list by tracked recommender.</returns>
        public double AverageListDiversity(Func<Item, Item, double> difference)
        {
            return Evaluation.Diversity(GetAllRecommendations(), difference);
        }

        /// <summary>
        /// Calculates the recall of recommendations by tracked recommender.
        /// </summary>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <returns>Recall of recommendations by tracked recommender.</returns>
        public double Recall(Func<int, List<int>> GetRelevantItems)
        {
            return Evaluation.Recall(GetAllRecommendations(), GetRelevantItems);
        }
        /// <summary>
        /// Calculates the accuracy of recommendations by tracked recommender.
        /// </summary>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <param name="numberOfItems">Total number of items.</param>
        /// <returns>Accuracy of recommendations by tracked recommender.</returns>
        public double Accuracy(Func<int, List<int>> GetRelevantItems, int numberOfItems)
        {
            return Evaluation.Accuracy(GetAllRecommendations(), GetRelevantItems, numberOfItems);
        }
        /// <summary>
        /// Calculates the precision of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Precision of recommendations by tracked recommender.</returns>
        public double Precision(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.Precision(GetAllRecommendations(), IsItemRelevant);
        }
        /// <summary>
        /// Calculates the mean average precision of recommendations by tracked recommender.
        /// </summary>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Mean average precision of recommendations by tracked recommender.</returns>
        public double MAP(Func<int, int, bool> IsItemRelevant)
        {
            return Evaluation.MAP(GetAllRecommendations(), IsItemRelevant);
        }

        /// <summary>
        /// Caclulates the mean response time of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Mean response time in miliseconds.</returns>
        public double MeanResponseTime()
        {
            return Evaluation.MeanResponseTime(GetAllRecommendations());
        }
        /// <summary>
        /// Caclulates the median response time of recommendations by tracked recommender.
        /// </summary>
        /// <returns>Median response time in miliseconds.</returns>
        public double MedianResponseTime()
        {
            return Evaluation.MedianResponseTime(GetAllRecommendations());
        }
        #endregion
    }
}
