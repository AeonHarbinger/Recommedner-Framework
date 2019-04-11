using MyMediaLite.IO;
using MyMediaLite.RatingPrediction;
using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    class UIBImplemented : MarshalByRefObject, IManagedRecommenderSystem
    {
        /// <inheritdoc />
        public string Name => "UIB";
        /// <inheritdoc />
        public string Version { get; private set; }

        /// <summary>
        /// Trained recommender.
        /// </summary>
        UserItemBaseline system;
        /// <summary>
        /// Database of the system.
        /// </summary>
        Database database;

        /// <summary>
        /// Parameters of the recommender.
        /// </summary>
        Dictionary<string, object> parameters;

        /// <summary>
        /// Creates instance with specified parameters.
        /// </summary>
        /// <param name="version">Version of the algorithm.</param>
        /// <param name="data">Database of the system.</param>
        public UIBImplemented(string version, Database data)
        {
            Version = version;
            database = data;
            parameters = new Dictionary<string, object>();
        }
        
        #region Recommendation
        /// <inheritdoc />
        public bool CanPredictRating(User user, Item item) => system.CanPredict(user.Id, item.Id);
        /// <inheritdoc />
        public bool CanPredictForUser(User user)
        {
            foreach (var item in database.Items)
            {
                if (system.CanPredict(user.Id, item.Value.Id)) return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool CanPredictForItem(Item item)
        {
            foreach (var user in database.Users)
            {
                if (system.CanPredict(user.Value.Id, item.Id)) return true;
            }
            return false;
        }
        /// <inheritdoc />
        public float GetExpectedRating(User user, Item item)
        {
            return system.Predict(user.Id, item.Id);
        }

        /// <inheritdoc />
        public List<RecommendedItem> GetRanking(User user, List<Item> fromItems)
        {
            var sortedList = new SortedList<float, int>();
            foreach (var item in fromItems)
            {
                sortedList.Add(system.Predict(user.Id, item.Id), item.Id);
            }

            var result = new List<RecommendedItem>();
            foreach (var tuple in sortedList)
            {
                Item i = database.GetItem(tuple.Value);
                result.Add(new RecommendedItem() { Item = i, ExpectedPreference = tuple.Key });
            }

            result.Reverse();
            return result;
        }
        /// <inheritdoc />
        public List<RecommendedItem> GetRecommendation(User user, List<Item> fromItems, int count)
        {
            List<int> candidateItems = null;
            if (fromItems != null)
            {
                candidateItems = new List<int>();
                foreach (Item item in fromItems)
                {
                    candidateItems.Add(item.Id);
                }
            }

            var received = system.Recommend(user.Id, count, null, candidateItems);
            return RecommendedItem.MakeList(database, received);
        }

        /// <inheritdoc />
        public void HandleFeedback(Feedback feedback)
        {
            if (feedback is ImplicitFeedback) return;
            var expl = (ExplicitFeedback)feedback;

            system.Ratings.Add(expl.UserId, expl.ItemId, expl.Preference);
        }
        #endregion

        #region Manipulation
        /// <inheritdoc />
        public List<KeyValuePair<string, object>> GetParams()
        {
            return parameters.ToList();
        }
        /// <inheritdoc />
        public object GetParamValue(string name)
        {
            return parameters[name];
        }
        /// <inheritdoc />
        public void SetParamValue(string name, object value)
        {
            switch (name)
            {
                case "NumIter":
                    system.NumIter = (uint)value;
                    break;
                case "RegI":
                    system.RegI = (float)value;
                    break;
                case "RegU":
                    system.RegU = (float)value;
                    break;
                default:
                    throw new Exception(string.Format("Unknown parameter \"{0}\"", name));
            }

            parameters[name] = value;
        }

        /// <inheritdoc />
        public void LoadModel(string file)
        {
            system = new UserItemBaseline
            {
                Ratings = RatingData.Read("C:/Users/Ondra/Disk Google/UK/Ročníkový projekt - zpracování/Data/reduced/training.csv")
            };
            system.LoadModel(file);

            parameters = new Dictionary<string, object>
            {
                { "RegI", system.RegI },
                { "RegU", system.RegU },
                { "NumIter", system.NumIter }
            };
        }
        /// <inheritdoc />
        public void SaveModel(string file)
        {
            system.SaveModel(file);
        }

        /// <inheritdoc />
        public void Train()
        {
            system.Train();
        }
        #endregion
    }
}
