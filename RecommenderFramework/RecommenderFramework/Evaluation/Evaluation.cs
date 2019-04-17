using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents users preference towards an item.
    /// </summary>
    public struct UserItemPreference
    {
        /// <summary>
        /// Which users preference toward what item are we interested in.
        /// </summary>
        public UserItemPair UserItem;
        /// <summary>
        /// How large is the preference.
        /// </summary>
        public float Value;

        /// <summary>
        /// Creates new instance with given parameters.
        /// </summary>
        /// <param name="uip">Which user and item the preference refers to.</param>
        /// <param name="val">Value of the preference.</param>
        public UserItemPreference(UserItemPair uip, float val)
        {
            UserItem = uip;
            Value = val;
        }
    }

    /// <summary>
    /// Represents a pair of user and item.
    /// </summary>
    public struct UserItemPair
    {
        public int UserId;
        public int ItemId;

        /// <summary>
        /// Creates new instance with specified parameters.
        /// </summary>
        /// <param name="userId">Identification of the user.</param>
        /// <param name="itemId">Identification of the item.</param>
        public UserItemPair(int userId, int itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
    }

    /// <summary>
    /// Class used for evaluation of recommender systems.
    /// </summary>
    public class Evaluation
    {
        #region Rating based
        /// <summary>
        /// Calculates mean absolute error of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <returns>Mean absolute error of recommendations.</returns>
        public static double MAE(IEnumerable<Recommendation> recommendations, Dictionary<UserItemPair, float?> userPref)
        {
            List<UserItemPreference> expected = GetExpectedPreference(recommendations);
            
            double sumAbsDiff = 0;
            int canCompareCount = 0;
            foreach (var exp in expected)
            {
                userPref.TryGetValue(new UserItemPair(exp.UserItem.UserId, exp.UserItem.ItemId), out float? actualValue);
                if (actualValue != null)
                {
                    sumAbsDiff += Math.Abs((float)actualValue - exp.Value);
                    canCompareCount++;
                }
            }

            return sumAbsDiff / canCompareCount;
        }

        /// <summary>
        /// Calculates root mean square error of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <returns>Root mean square error of recommendations.</returns>
        public static double RMSE(IEnumerable<Recommendation> recommendations, Dictionary<UserItemPair, float?> userPref)
        {
            List<UserItemPreference> expected = GetExpectedPreference(recommendations);
            
            double sumDiffSquared = 0;
            int canCompare = 0;
            foreach (var exp in expected)
            {
                userPref.TryGetValue(new UserItemPair(exp.UserItem.UserId, exp.UserItem.ItemId), out float? actualValue);
                if (actualValue != null)
                {
                    sumDiffSquared += Math.Pow((float)actualValue - exp.Value, 2);
                    canCompare++;
                }
            }

            return Math.Sqrt(sumDiffSquared / canCompare);
        }
        #endregion

        #region Ranking based
        /// <summary>
        /// Calculates the mean reciprocal rank of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="isItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Mean reciprocal rank of recommendations.</returns>
        public static double MRR(IEnumerable<Recommendation> recommendations, Func<int, int, bool> isItemRelevant)
        {
            var allRanks = new List<int?>();
            foreach (var rec in recommendations)
            {
                int? rank = null;
                for (int i = 0; i < rec.Items.Count; i++) 
                {
                    int itemId = rec.Items[i].Item.Id;
                    if (isItemRelevant(rec.UserId, itemId))
                    {
                        rank = i + 1;
                        break;
                    }
                }

                allRanks.Add(rank);
            }

            double rankSum = 0;
            foreach (var rank in allRanks)
            {
                if (rank != null)
                    rankSum += 1.0 / (int)rank;
            }

            return rankSum / allRanks.Count;
        }

        /// <summary>
        /// Calculates the average discounted cumulative gain of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <param name="rankPosition">At which rank position will the DCG be calculated.</param>
        /// <returns>Average discounted cumulative gain of recommendations.</returns>
        public static double AverageDCG (IEnumerable<Recommendation> recommendations, Dictionary<UserItemPair, float?> userPref, int rankPosition)
        {
            double sumRecomDCG = 0;
            foreach (var rec in recommendations)
            {
                for (int i = 0; i < rankPosition; i++)
                {
                    userPref.TryGetValue(new UserItemPair(rec.UserId, rec.Items[i].Item.Id), out float? actualValue);
                    if (actualValue == null)
                        actualValue = 0;

                    sumRecomDCG += (float)actualValue / Math.Log(i + 2, 2);
                }
            }

            return sumRecomDCG / recommendations.Count();
        }

        /// <summary>
        /// Calculates the average normalized discounted cumulative gain of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <param name="rankPosition">At which rank position will the NDCG be calculated.</param>
        /// <returns>Average normalized discounted cumulative gain of recommendations.</returns>
        public static double AverageNDCG(IEnumerable<Recommendation> recommendations, Dictionary<UserItemPair, float?> userPref, int rankPosition)
        {
            double sumRecomNDCG = 0;
            int listsWithFeedback = 0;
            foreach (var rec in recommendations)
            {
                double DCG = 0;
                for (int i = 0; i < rankPosition; i++)
                {
                    userPref.TryGetValue(new UserItemPair(rec.UserId, rec.Items[i].Item.Id), out float? actualValue);
                    if (actualValue == null)
                        actualValue = 0;

                    DCG += (float)actualValue / Math.Log(i + 2, 2);
                }

                var sorted = new SortedList<float, UserItemPair>(new CompareFloat());
                for (int i = 0; i < rec.Items.Count; i++)
                {
                    userPref.TryGetValue(new UserItemPair(rec.UserId, rec.Items[i].Item.Id), out float? actualValue);
                    if (actualValue == null)
                        actualValue = 0;

                    sorted.Add((float)actualValue, new UserItemPair(rec.UserId, rec.Items[i].Item.Id));
                }

                double IDCG = 0;
                for (int i = 0; i < rankPosition; i++)
                {
                    IDCG += sorted.Keys[sorted.Count - i - 1] / Math.Log(i + 2, 2);
                }

                if (IDCG != 0)
                {
                    sumRecomNDCG += DCG / IDCG;
                    listsWithFeedback++;
                }
            }

            return sumRecomNDCG / listsWithFeedback;
        }
        #endregion

        #region Speed of response
        /// <summary>
        /// Caclulates the mean response time of recommendations.
        /// </summary>
        /// <param name="recommendations">Recommendations whose response time we are interested in.</param>
        /// <returns>Mean response time in miliseconds.</returns>
        public static double MeanResponseTime(IEnumerable<Recommendation> recommendations)
        {
            if (recommendations.Count() == 0) return double.NaN;
            else return recommendations.Average(list => list.ResponseTime);
        }

        /// <summary>
        /// Caclulates the median response time of recommendations.
        /// </summary>
        /// <param name="recommendations">Recommendations whose response time we are interested in.</param>
        /// <returns>Median response time in miliseconds.</returns>
        public static double MedianResponseTime(IEnumerable<Recommendation> recommendations)
        {
            if (recommendations.Count() == 0) return double.NaN;
            else
            {
                List<int> times = new List<int>();
                foreach (var list in recommendations)
                    times.Add(list.ResponseTime);
                times.Sort();

                int count = times.Count;
                if (count % 2 != 0) return times[count / 2];
                else return 1.0 * (times[count / 2] + times[count / 2 - 1]) / 2;
            }
        }
        #endregion

        #region Click-through, Coverage, Diversity
        /// <summary>
        /// Calculates the click-through rate of recommendations (aka. the (# ClickOnRecommendations / # RecommendedItemsDisplayed))
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="feedback">List of feedback given to recommender system.</param>
        /// <returns>Click through rate of recommendations.</returns>
        public static double CTR(IEnumerable<Recommendation> recommendations, List<Feedback> feedback)
        {
            var filteredFeedback = new HashSet<UserItemPair>();
            foreach (var feed in feedback)
                if (feed is ClickOnRecommendation)
                    filteredFeedback.Add(new UserItemPair(feed.UserId, feed.ItemId));

            var recCount = new HashSet<UserItemPair>();
            foreach (var rec in recommendations)
                foreach (var item in rec.Items)
                    recCount.Add(new UserItemPair(rec.UserId, item.Item.Id));

            return 1.0 * filteredFeedback.Count / recCount.Count;
        }

        /// <summary>
        /// Calculates what portion of users can the system predict rating for.
        /// </summary>
        /// <param name="users">List of all users.</param>
        /// <param name="system">System to be tested.</param>
        /// <returns>Portion of users for which some rating can be predicted.</returns>
        public static double UserCoverage(IEnumerable<User> users, IRecommenderSystem system)
        {
            int canPredict = 0;
            foreach (var user in users)
                if (system.CanRecommendToUser(user)) canPredict++;
            return 1.0 * canPredict / users.Count();
        }
        /// <summary>
        /// Calculates what portion of items can the system predict rating for.
        /// </summary>
        /// <param name="items">List of all items.</param>
        /// <param name="system">System to be tested.</param>
        /// <returns>Portion of items for which some rating can be predicted.</returns>
        public static double ItemCoverage(IEnumerable<Item> items, IRecommenderSystem system)
        {
            int canPredict = 0;
            foreach (var item in items)
                if (system.CanRecommendItem(item)) canPredict++;
            return 1.0 * canPredict / items.Count();
        }

        /// <summary>
        /// Calculates the average diversity of items of within one recommendation.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="differenceOfItems">Function that tells us how much two items differ.</param>
        /// <returns>Average diversity of a recommendation list.</returns>
        public static double Diversity(IEnumerable<Recommendation> recommendations, Func<Item, Item, double> differenceOfItems)
        {
            double sumAllListDiversity = 0;

            foreach (var list in recommendations)
            {
                double sumThisListDiversity = 0;

                for (int i = 0; i < list.Items.Count; i++)
                    for (int j = i + 1; j < list.Items.Count; j++)
                        sumThisListDiversity += differenceOfItems(list.Items[i].Item, list.Items[j].Item);

                int n = list.Items.Count;
                sumAllListDiversity += sumThisListDiversity / (1.0 * n * (n - 1) / 2);
            }

            return sumAllListDiversity / recommendations.Count();
        }
        #endregion

        #region Recall, Accuracy, Precission, MAP
        /// <summary>
        /// Calculates the recall of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="usersRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <returns>Recall of recommendations.</returns>
        public static double Recall   (IEnumerable<Recommendation> recommendations, Func<int, List<int>> usersRelevantItems)
        {
            Dictionary<int, HashSet<int>> recommendedToUser = GetRecommendedToUser(recommendations);

            double relevanceSum = 0;
            foreach (var userId in recommendedToUser.Keys)
            {
                HashSet<int> recItems = recommendedToUser[userId];
                List<int> relevItems = usersRelevantItems(userId);

                int relevant = relevItems.Count;
                int relevantAndRecommended = recItems.Intersect(relevItems).Count();

                relevanceSum += 1.0 * relevantAndRecommended / relevant;
            }

            return relevanceSum / recommendedToUser.Count;
        }

        /// <summary>
        /// Calculates the accuracy of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="usersRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <param name="numberOfItems">Total number of items.</param>
        /// <returns>Accuracy of recommendations.</returns>
        public static double Accuracy (IEnumerable<Recommendation> recommendations, Func<int, List<int>> usersRelevantItems, int numberOfItems)
        {
            Dictionary<int, HashSet<int>> recommendedToUser = GetRecommendedToUser(recommendations);

            if (recommendedToUser.Count == 0)
                return double.NaN;

            int nonRelevantNotRecommended = 0;
            int relevantRecommended = 0;
            foreach (var userId in recommendedToUser.Keys)
            {
                HashSet<int> recItems = recommendedToUser[userId];
                List<int> relevItems = usersRelevantItems(userId);

                nonRelevantNotRecommended += (numberOfItems - recItems.Union(relevItems).Count());
                relevantRecommended       += recItems.Intersect(relevItems).Count();
            }

            return (1.0 * nonRelevantNotRecommended + relevantRecommended) / (numberOfItems * recommendedToUser.Count);
        }

        /// <summary>
        /// Calculates the precision of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="isItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Precision of recommendations.</returns>
        public static double Precision(IEnumerable<Recommendation> recommendations, Func<int, int, bool> isItemRelevant)
        {
            int recommendedCount = 0;
            int relevantCount    = 0;

            foreach (var rec in recommendations)
                foreach (var recItem in rec.Items)
                {
                    recommendedCount++;
                    if (isItemRelevant(rec.UserId, recItem.Item.Id))
                        relevantCount++;
                }

            return 1.0 * relevantCount / recommendedCount;
        }

        /// <summary>
        /// Calculates the mean average precision of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="isItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Mean average precision of recommendations.</returns>
        public static double MAP      (IEnumerable<Recommendation> recommendations, Func<int, int, bool> isItemRelevant)
        {
            var recommendedItemsToUser = new Dictionary<int, List<Recommendation>>();
            foreach (var rec in recommendations)
            {
                if (!recommendedItemsToUser.TryGetValue(rec.UserId, out List<Recommendation> list))
                {
                    list = new List<Recommendation>();
                    recommendedItemsToUser.Add(rec.UserId, list);
                }

                list.Add(rec);
            }

            int userCount = 0;
            double userPrecissionSum = 0;
            foreach (var userList in recommendedItemsToUser.Values)
            {
                userCount++;
                userPrecissionSum += Precision(userList, isItemRelevant);
            }

            return userPrecissionSum / userCount;
        }
        #endregion
       


        #region Private helper functions and classes
        /// <summary>
        /// Comparer which allows us to store multiple values in sortedlist with the same float key.
        /// </summary>
        class CompareFloat : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                var result = x.CompareTo(y);
                if (result == 0) return 1;
                else return result;
            }
        }

        /// <summary>
        /// Returns the items recommended for each user. 
        /// </summary>
        /// <param name="recommendations">List of recommendations provided by the system.</param>
        /// <returns>Set of items recommended to each user.</returns>
        static Dictionary<int, HashSet<int>> GetRecommendedToUser(IEnumerable<Recommendation> recommendations)
        {
            var recommendedToUser = new Dictionary<int, HashSet<int>>();
            foreach (var rec in recommendations)
            {
                if (!recommendedToUser.TryGetValue(rec.UserId, out HashSet<int> set))
                {
                    set = new HashSet<int>();
                    recommendedToUser.Add(rec.UserId, set);
                }

                foreach (var recItem in rec.Items)
                    set.Add(recItem.Item.Id);
            }

            return recommendedToUser;
        }

        /// <summary>
        /// Given list of recommendations returns the preference of user toward item as expected by recommender.
        /// </summary>
        /// <param name="recommendations">Recommendations with expected preferences.</param>
        /// <returns>List of expected preferences only.</returns>
        static List<UserItemPreference> GetExpectedPreference(IEnumerable<Recommendation> recommendations)
        {
            var expectedPreferences = new List<UserItemPreference>();
            foreach (var list in recommendations)
            {
                foreach (var rec in list.Items)
                {
                    if (rec.ExpectedPreference == null)
                        continue;
                    UserItemPair uip = new UserItemPair(list.UserId, rec.Item.Id);
                    expectedPreferences.Add(new UserItemPreference(uip, (float)rec.ExpectedPreference));
                }
            }

            return expectedPreferences;
        }
        #endregion
    }
}
