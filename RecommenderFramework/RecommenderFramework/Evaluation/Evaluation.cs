using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static double MAE(List<RecommendationList> recommendations, List<UserItemPreference> preferences)
        {
            List<UserItemPreference> expected = GetExpectedPreference(recommendations);

            var actual = new Dictionary<UserItemPair, float>();
            foreach (var item in preferences)
            {
                actual.Add(item.UserItem, item.Value);
            }

            double sumAbsDiff = 0;
            int canCompare = 0;
            foreach (var exp in expected)
            {
                if (actual.TryGetValue(exp.UserItem, out float actualValue))
                {
                    sumAbsDiff += Math.Abs(actualValue - exp.Value);
                    canCompare++;
                }
            }

            return sumAbsDiff / canCompare;
        }

        /// <summary>
        /// Calculates root mean square error of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <returns>Root mean square error of recommendations.</returns>
        public static double RMSE(List<RecommendationList> recommendations, List<UserItemPreference> preferences)
        {
            List<UserItemPreference> expected = GetExpectedPreference(recommendations);

            var actual = new Dictionary<UserItemPair, float>();
            foreach (var item in preferences)
            {
                actual.Add(item.UserItem, item.Value);
            }

            double sumDiffSquared = 0;
            int canCompare = 0;
            foreach (var exp in expected)
            {
                if (actual.TryGetValue(exp.UserItem, out float expectedValue))
                {
                    sumDiffSquared += Math.Pow(expectedValue - exp.Value, 2);
                    canCompare++;
                }
            }

            return Math.Sqrt(sumDiffSquared / canCompare);
        }
        #endregion

        #region Ranking based
        /// <summary>
        /// Calculates the mean average reciprocal hit rank of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Average reciprocal hit rank of recommendations.</returns>
        public static double ARHR(List<RecommendationList> recommendations, Func<int, int, bool> IsItemRelevant)
        {
            var allRanks = new List<int?>();
            foreach (var recList in recommendations)
            {
                int? rank = null;
                for (int i = 0; i < recList.Items.Count; i++) 
                {
                    int itemId = recList.Items[i].Item.Id;
                    if (IsItemRelevant(recList.UserId, itemId))
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
                if (rank != null) rankSum += 1.0 / (int)rank;
            }

            return 1.0 * rankSum / allRanks.Count;
        }

        /// <summary>
        /// Calculates the average discounted cumulative gain of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <param name="rankPosition">At which rank position will the DCG be calculated.</param>
        /// <returns>Average discounted cumulative gain of recommendations.</returns>
        public static double AverageDCG(List<RecommendationList> recommendations, List<UserItemPreference> preferences, int rankPosition)
        {
            var prefDict = new Dictionary<UserItemPair, float>();
            foreach (var pref in preferences)
                prefDict.Add(pref.UserItem, pref.Value);

            double sumRecomDCG = 0;
            foreach (var recList in recommendations)
            {
                for (int i = 0; i < rankPosition; i++)
                {
                    var uip = new UserItemPair(recList.UserId, recList.Items[i].Item.Id);
                    if (!prefDict.TryGetValue(uip, out float relevance))
                        relevance = 0;

                    sumRecomDCG += relevance / Math.Log(i + 2, 2);
                    // sumRecomDCG += (Math.Pow(2, relevance) - 1) / Math.Log(i + 2, 2);
                }
            }

            return sumRecomDCG / recommendations.Count;
        }

        /// <summary>
        /// Calculates the average normalized discounted cumulative gain of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="preferences">List of actual preferences by users.</param>
        /// <param name="rankPosition">At which rank position will the NDCG be calculated.</param>
        /// <returns>Average normalized discounted cumulative gain of recommendations.</returns>
        public static double AverageNDCG(List<RecommendationList> recommendations, List<UserItemPreference> preferences, int rankPosition)
        {
            var prefDict = new Dictionary<UserItemPair, float>();
            foreach (var pref in preferences)
                prefDict.Add(pref.UserItem, pref.Value);

            double sumRecomNDCG = 0;
            int listsWithFeedback = 0;
            foreach (var recList in recommendations)
            {
                double DCG = 0;
                for (int i = 0; i < rankPosition; i++)
                {
                    var uip = new UserItemPair(recList.UserId, recList.Items[i].Item.Id);
                    if (!prefDict.TryGetValue(uip, out float relevance))
                        relevance = 0;

                    DCG += relevance / Math.Log(i + 2, 2);
                }

                var sorted = new SortedList<float, UserItemPair>(new CompareFloat());
                for (int i = 0; i < recList.Items.Count; i++)
                {
                    var uip = new UserItemPair(recList.UserId, recList.Items[i].Item.Id);
                    if (!prefDict.TryGetValue(uip, out float relevance))
                        relevance = 0;
                    sorted.Add(relevance, uip);
                }

                double IDCG = 0;
                for (int i = 0; i < rankPosition; i++)
                {
                    IDCG += sorted.Keys[sorted.Count - i - 1] / Math.Log(i + 2, 2);
                    // IDCG += (Math.Pow(2, sorted.Keys[sorted.Count - i - 1]]) - 1) / Math.Log(i + 2, 2);
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
        public static double MeanResponseTime(List<RecommendationList> recommendations)
        {
            if (recommendations.Count == 0) return double.NaN;
            else return recommendations.Average(list => list.ResponseTimeMs);
        }
        /// <summary>
        /// Caclulates the median response time of recommendations.
        /// </summary>
        /// <param name="recommendations">Recommendations whose response time we are interested in.</param>
        /// <returns>Median response time in miliseconds.</returns>
        public static double MedianResponseTime(List<RecommendationList> recommendations)
        {
            if (recommendations.Count == 0) return double.NaN;
            else
            {
                List<int> times = new List<int>();
                foreach (var list in recommendations)
                    times.Add(list.ResponseTimeMs);
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
        public static double CTR(List<RecommendationList> recommendations, List<Feedback> feedback)
        {
            var filteredFeedback = new List<Feedback>();
            foreach (var feed in feedback)
                if (feed is ImplicitFeedback && ((ImplicitFeedback)feed).Type == "ClickOnRecommendation") filteredFeedback.Add(feed);

            long recCount = 0;
            foreach (var recList in recommendations)
                recCount += recList.Items.Count;

            return 1.0 * filteredFeedback.Count / recCount;
        }

        /// <summary>
        /// Calculates what portion of users can the system predict rating for.
        /// </summary>
        /// <param name="users">List of all users.</param>
        /// <param name="system">System to be tested.</param>
        /// <returns>Portion of users for which some rating can be predicted.</returns>
        public static double UserCoverage(List<User> users, IRecommenderSystem system)
        {
            int canProvide = 0;
            foreach (var user in users)
                if (system.CanPredictForUser(user)) canProvide++;
            return 1.0 * canProvide / users.Count;
        }
        /// <summary>
        /// Calculates what portion of items can the system predict rating for.
        /// </summary>
        /// <param name="items">List of all items.</param>
        /// <param name="system">System to be tested.</param>
        /// <returns>Portion of items for which some rating can be predicted.</returns>
        public static double ItemCoverage(List<Item> items, IRecommenderSystem system)
        {
            int canProvide = 0;
            foreach (var item in items)
                if (system.CanPredictForItem(item)) canProvide++;
            return 1.0 * canProvide / items.Count;
        }

        /// <summary>
        /// Calculates the average diversity of items of within one recommendation.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="difference">Function that tells us how much two items differ.</param>
        /// <returns>Average diversity of a recommendation list.</returns>
        public static double AverageListDiversity(List<RecommendationList> recommendations, Func<Item, Item, double> difference)
        {
            double sumAllListDiversity = 0;

            foreach (var list in recommendations)
            {
                double sumThisListDiversity = 0;

                for (int i = 0; i < list.Items.Count; i++)
                    for (int j = i + 1; j < list.Items.Count; j++)
                        sumThisListDiversity += difference(list.Items[i].Item, list.Items[j].Item);

                sumAllListDiversity += sumThisListDiversity / NumberOfPairs(list.Items.Count);
            }

            return sumAllListDiversity / recommendations.Count;
        }
        #endregion

        #region Recall, Accuracy, Precission, MAP
        /// <summary>
        /// Calculates the recall of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <returns>Recall of recommendations.</returns>
        public static double Recall(List<RecommendationList> recommendations, Func<int, List<int>> GetRelevantItems)
        {
            var recommendedItemsToUser = new Dictionary<int, HashSet<int>>();
            foreach (var recList in recommendations)
            {
                if (!recommendedItemsToUser.TryGetValue(recList.UserId, out HashSet<int> set))
                {
                    set = new HashSet<int>();
                    recommendedItemsToUser.Add(recList.UserId, set);
                }

                foreach (var recItem in recList.Items)
                {
                    set.Add(recItem.Item.Id);
                }
            }

            int relevant = 0;
            int relevantAndRecommended = 0;
            foreach (var userId in recommendedItemsToUser.Keys)
            {
                HashSet<int> recItems = recommendedItemsToUser[userId];
                List<int> relevItems = GetRelevantItems(userId);
                relevant += relevItems.Count;
                relevantAndRecommended += recItems.Intersect(relevItems).Count();
            }

            return 1.0 * relevantAndRecommended / relevant;
        }

        /// <summary>
        /// Calculates the accuracy of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="GetRelevantItems">Function that retrieves all items relevant to a user.</param>
        /// <param name="numberOfItems">Total number of items.</param>
        /// <returns>Accuracy of recommendations.</returns>
        public static double Accuracy(List<RecommendationList> recommendations, Func<int, List<int>> GetRelevantItems, int numberOfItems)
        {
            // TODO Make own method for some of this code, reuse.

            var recommendedItemsToUser = new Dictionary<int, HashSet<int>>();
            foreach (var recList in recommendations)
            {
                if (!recommendedItemsToUser.TryGetValue(recList.UserId, out HashSet<int> set))
                {
                    set = new HashSet<int>();
                    recommendedItemsToUser.Add(recList.UserId, set);
                }

                foreach (var recItem in recList.Items)
                {
                    set.Add(recItem.Item.Id);
                }
            }

            int nonRelevantNonRecommended = 0;
            int relevantAndRecommended = 0;
            foreach (var userId in recommendedItemsToUser.Keys)
            {
                HashSet<int> recItems = recommendedItemsToUser[userId];
                List<int> relevItems = GetRelevantItems(userId);
                nonRelevantNonRecommended += (numberOfItems - recItems.Union(relevItems).Count());
                relevantAndRecommended += recItems.Intersect(relevItems).Count();
            }

            return (1.0 * nonRelevantNonRecommended + relevantAndRecommended) / numberOfItems;
        }

        /// <summary>
        /// Calculates the precision of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Precision of recommendations.</returns>
        public static double Precision(List<RecommendationList> recommendations, Func<int, int, bool> IsItemRelevant) // TODO Create own delegate with explicit signature.
        {
            int countRecommended = 0;
            int countRelevant = 0;

            foreach (var recList in recommendations)
                foreach (var recItem in recList.Items)
                {
                    countRecommended++;
                    if (IsItemRelevant(recList.UserId, recItem.Item.Id)) countRelevant++;
                }

            return 1.0 * countRelevant / countRecommended;
        }

        /// <summary>
        /// Calculates the mean average precision of recommendations.
        /// </summary>
        /// <param name="recommendations">List of recommendations by recommender system.</param>
        /// <param name="IsItemRelevant">Function that tells us whether user finds certain item relevant.</param>
        /// <returns>Mean average precision of recommendations.</returns>
        public static double MAP(List<RecommendationList> recommendations, Func<int, int, bool> IsItemRelevant)
        {
            var recommendedItemsToUser = new Dictionary<int, List<RecommendationList>>();
            foreach (var recList in recommendations)
            {
                if (!recommendedItemsToUser.TryGetValue(recList.UserId, out List<RecommendationList> list))
                {
                    list = new List<RecommendationList>();
                    recommendedItemsToUser.Add(recList.UserId, list);
                }

                list.Add(recList);
            }

            int userCount = 0;
            double userPrecissionSum = 0;
            foreach (var userList in recommendedItemsToUser.Values)
            {
                userCount++;
                userPrecissionSum += Precision(userList, IsItemRelevant);
            }

            return userPrecissionSum / userCount;
        }
        #endregion
       


        #region Private functions and classes
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
        /// Given list of recommendations returns the preference of user toward item as expected by recommender.
        /// </summary>
        /// <param name="recommendations">Recommendations with expected preferences.</param>
        /// <returns>List of expected preferences only.</returns>
        static List<UserItemPreference> GetExpectedPreference(List<RecommendationList> recommendations)
        {
            var expectedRatings = new List<UserItemPreference>();
            foreach (var list in recommendations)
            {
                foreach (var rec in list.Items)
                {
                    UserItemPair uip = new UserItemPair(list.UserId, rec.Item.Id);
                    expectedRatings.Add(new UserItemPreference(uip, rec.ExpectedPreference));
                }
            }

            return expectedRatings;
        }

        /// <summary>
        /// Returns total number of pairs given number of items can make up.
        /// </summary>
        /// <param name="n">Number of items making up pairs.</param>
        /// <returns>Total number of pairs.</returns>
        static long NumberOfPairs(int n)
        {
            return Factorial(n) / (2 * Factorial(n - 2));
        }

        /// <summary>
        /// Returns factorial of given number.
        /// </summary>
        /// <param name="n">Number whose factorial will be returned.</param>
        /// <returns>Factorial of number.</returns>
        static long Factorial(int n)
        {
            if (n < 0) throw new Exception("Factorial cannot be of number smaller than zero.");

            long res = 1;
            for (int i = 2; i <= n; i++)
                res *= i;
            return res;
        }
        #endregion
    }
}
