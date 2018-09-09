using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents a form of feedback where users preference is implicit.
    /// </summary>
    [Serializable]
    public class ImplicitFeedback : Feedback
    {
        /// <summary>
        /// Type of feedback.
        /// </summary>
        public string Type;
        /// <summary>
        /// How strongly or which way was users preference expressed.
        /// </summary>
        public object Value;
    }
}
