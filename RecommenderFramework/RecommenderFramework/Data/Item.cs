using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents an item in recommender system.
    /// </summary>
    [Serializable]
    public class Item
    {
        /// <summary>
        /// Unique identification of the item.
        /// </summary>
        public int Id;
    }
}
