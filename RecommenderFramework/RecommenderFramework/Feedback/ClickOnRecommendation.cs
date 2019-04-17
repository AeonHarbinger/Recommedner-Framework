using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents an event where the user clicked on recommended item.
    /// </summary>
    [Table]
    public sealed class ClickOnRecommendation : Feedback
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
    }
}
