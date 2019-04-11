using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// A database representation of a recommendation.
    /// </summary>
    [Table]
    internal class DbRecommendation
    {
        /// <summary>
        /// Identification of the recommendation.
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public int Id           { get; set; }

        /// <summary>
        /// Name of the system that provided this recommendation.
        /// </summary>
        [Column]
        public string Name      { get; set; }
        /// <summary>
        /// Version of the system that provided this recommendation.
        /// </summary>
        [Column]
        public string Version   { get; set; }

        /// <summary>
        /// Identification of the user.
        /// </summary>
        [Column]
        public int UserId       { get; set; }
        /// <summary>
        /// When the recommendation was created.
        /// </summary>
        [Column]
        public DateTime AtTime  { get; set; }
        /// <summary>
        /// How long did it take for recommender system to provide this recommendation (in ms).
        /// </summary>
        [Column]
        public int ResponseTime { get; set; }

        /// <summary>
        /// Creates new instance with for specified id for a given recommender.
        /// </summary>
        /// <param name="recId">Identification of the recommendation.</param>
        /// <param name="name">Name of the recommender system that provided the recommendation.</param>
        /// <param name="version">Version of the recommender system that provided the recommendation.</param>
        /// <param name="rec">Recomendation whose DB representation we want.</param>
        public DbRecommendation(int recId, string name, string version, Recommendation rec)
        {
            Id           = recId;
            Name         = name;
            Version      = version;
            UserId       = rec.UserId;
            AtTime       = rec.AtTime;
            ResponseTime = rec.ResponseTime;
        }
    }
}
