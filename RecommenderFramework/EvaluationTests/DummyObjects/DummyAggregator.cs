using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationTests
{
    class DummyFunctions
    {
        public static float? Aggregate(List<Feedback> feedback)
        {
            if (feedback.Count != 1)
                return null;

            if (feedback[0].GetType() == typeof(ExplicitFeedback))
                return ((ExplicitFeedback)feedback[0]).Preference;

            return null;
        }

        public static double Diversity(Item a, Item b)
        {
            return Math.Abs(((DummyItem)a).value - ((DummyItem)b).value);
        }
    }
}

