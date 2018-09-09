using MyMediaLite.Data;
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
    class SVDImplemented : MarshalByRefObject, IManagedRecommenderSystem
    {
        /// <inheritdoc />
        public string Name => "SVD";
        /// <inheritdoc />
        public string Version { get; private set; }

        /// <summary>
        /// Trained recommender.
        /// </summary>
        SVDPlusPlus system;
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
        public SVDImplemented(string version, Database data)
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
                case "Regularization":
                    system.Regularization = (float)value;
                    break;
                case "LearnRate":
                    system.LearnRate = (float)value;
                    break;
                case "Decay":
                    system.Decay = (float)value;
                    break;
                case "NumIter":
                    system.NumIter = (uint)value;
                    break;
                case "InitStdDev":
                    system.InitStdDev = (float)value;
                    break;
                case "NumFactors":
                    system.NumFactors = (uint)value;
                    break;
                case "BiasLearnRate":
                    system.BiasLearnRate = (float)value;
                    break;
                case "BiasReg":
                    system.BiasReg = (float)value;
                    break;
                case "FrequencyRegularization":
                    system.FrequencyRegularization = (bool)value;
                    break;
                case "InitMean":
                    system.InitMean = (float)value;
                    break;
                default:
                    throw new Exception(string.Format("Unknown parameter \"{0}\"", name));
            }

            parameters[name] = value;
        }

        /// <inheritdoc />
        public void LoadModel(string file)
        {
            system = new SVDPlusPlus
            {
                Ratings = RatingData.Read("C:/Users/Ondra/Disk Google/UK/Ročníkový projekt - zpracování/Data/reduced/training.csv")
            };
            system.LoadModel(file);

            parameters = new Dictionary<string, object>
            {
                { "Decay", system.Decay },
                { "NumIter", system.NumIter },
                { "BiasReg", system.BiasReg },
                { "InitMean", system.InitMean },
                { "LearnRate", system.LearnRate },
                { "InitStdDev", system.InitStdDev },
                { "NumFactors", system.NumFactors },
                { "BiasLearnRate", system.BiasLearnRate },
                { "Regularization", system.Regularization },
                { "FrequencyRegularization", system.FrequencyRegularization }
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
