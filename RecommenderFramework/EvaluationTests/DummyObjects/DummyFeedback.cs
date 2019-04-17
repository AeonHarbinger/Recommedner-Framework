using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationTests
{
    class DummyFeedback : Feedback
    {
        public override int UserId      { get; set; }
        public override int ItemId      { get; set; }
        public override DateTime AtTime { get; set; }

        public string Type { get; set; } 
    }
}
