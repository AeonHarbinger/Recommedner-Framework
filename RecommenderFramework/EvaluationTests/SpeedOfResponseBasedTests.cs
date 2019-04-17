using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class SpeedOfResponseBasedTests
    {
        [TestMethod]
        public void Mean_Empty()
        {
            var res = Evaluation.MeanResponseTime(new List<Recommendation>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Mean_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.MeanResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 61) < 0.001);
        }
        [TestMethod]
        public void Mean_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.MeanResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 40.75) < 0.001);
        }
        [TestMethod]
        public void Mean_Valid3()
        {
            Simple3(out List<Recommendation> rl);
            var res = Evaluation.MeanResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 54.857) < 0.001);
        }



        [TestMethod]
        public void Median_Empty()
        {
            var res = Evaluation.MedianResponseTime(new List<Recommendation>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Median_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.MedianResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 61) < 0.001);
        }
        [TestMethod]
        public void Median_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.MedianResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 43.5) < 0.001);
        }
        [TestMethod]
        public void Median_Valid3()
        {
            Simple3(out List<Recommendation> rl);
            var res = Evaluation.MedianResponseTime(rl);
            Assert.IsTrue(Math.Abs(res - 55) < 0.001);
        }

        void Simple1(out List<Recommendation> rl)
        {
            rl = new List<Recommendation> {
                new Recommendation(0, null, DateTime.MinValue, 61)
            };
        }
        void Simple2(out List<Recommendation> rl)
        {
            rl = new List<Recommendation> {
                new Recommendation(0, null, DateTime.MinValue, 61),
                new Recommendation(0, null, DateTime.MinValue, 32),
                new Recommendation(0, null, DateTime.MinValue, 55),
                new Recommendation(0, null, DateTime.MinValue, 15)
            };
        }
        void Simple3(out List<Recommendation> rl)
        {
            rl = new List<Recommendation> {
                new Recommendation(0, null, DateTime.MinValue, 61),
                new Recommendation(0, null, DateTime.MinValue, 32),
                new Recommendation(0, null, DateTime.MinValue, 55),
                new Recommendation(0, null, DateTime.MinValue, 15),
                new Recommendation(0, null, DateTime.MinValue, 132),
                new Recommendation(0, null, DateTime.MinValue, 66),
                new Recommendation(0, null, DateTime.MinValue, 23)
            };
        }
    }
}
