using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents explicit preference a user expressed for an item.
    /// </summary>
    [Table]
    public sealed class ExplicitFeedback : Feedback
    {
        /// <inheritdoc />
        [Column(IsPrimaryKey = true)]        
        public override int UserId      { get; set; }
        /// <inheritdoc />
        [Column(IsPrimaryKey = true)]
        public override int ItemId      { get; set; }
        /// <inheritdoc />
        [Column(IsPrimaryKey = true)]
        public override DateTime AtTime { get; set; }

        /// <summary>
        /// To what extent does the user like the item.
        /// </summary>
        [Column]
        public float Preference         { get; set; }
    }
}
