using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents functionality of a recommender system that allows outside control of it.
    /// </summary>
    public interface IManagedRecommenderSystem : IRecommenderSystem
    {
        /// <summary>
        /// Gets value of given parameter.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <returns>Object containing parameter value.</returns>
        object GetParamValue(string name);
        /// <summary>
        /// Sets value of given parameter.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Object containing parameter value.</param>
        void SetParamValue(string name, object value);
        /// <summary>
        /// Returns all parameters with their values.
        /// </summary>
        /// <returns>List of pairs containing parameter name and value.</returns>
        List<KeyValuePair<string, object>> GetParams();

        /// <summary>
        /// Performs training on the system.
        /// </summary>
        void Train();
        /// <summary>
        /// Saves the recommender system.
        /// </summary>
        /// <param name="fileName">Name of the file where the system should be saved.</param>
        void SaveModel(string fileName);
        /// <summary>
        /// Loads the recommender system.
        /// </summary>
        /// <param name="fileName">Name of the file from which system should be loaded.</param>
        void LoadModel(string fileName);
    }
}
