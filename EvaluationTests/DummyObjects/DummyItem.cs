using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationTests
{
    class DummyItem : Item
    {
        public override int Id { get; set; }
        public double value;
    }
}
