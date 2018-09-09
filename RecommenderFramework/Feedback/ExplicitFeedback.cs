using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents a form of feedback where users preference is explicitly known.
    /// </summary>
    [Serializable]
    public sealed class ExplicitFeedback : Feedback
    {
        /// <summary>
        /// What is the preference of a user towards item.
        /// </summary>
        public float Preference;
    }
}
