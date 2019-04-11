using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class RankingBasedTest
    {
        [TestMethod]
        public void MRR_Empty()
        {
            var res = Evaluation.MRR(new List<Recommendation>(), (uid, iid) => (uid + iid) % 2 == 0);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void MRR_Valid1()
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 4 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl2 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 8 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl3 = new Recommendation(2,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 7 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 8 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1, rl2, rl3 };
            var res = Evaluation.MRR(rl, (uid, iid) => (uid + iid) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.61111) < 0.001);
        }
        [TestMethod]
        public void MRR_Valid2()
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 4 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl2 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 8 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl3 = new Recommendation(2,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 7 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 8 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                },
                DateTime.MinValue,
                0
            );

            var rl4 = new Recommendation(3,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 4  }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 8  }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2  }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 12 }, ExpectedPreference = 0 },
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1, rl2, rl3, rl4 };
            var res = Evaluation.MRR(rl, (uid, iid) => (uid + iid) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.45833) < 0.001);
        }



        [TestMethod]
        public void DCG_Empty()
        {
            var emptyFeedback = new Dictionary<UserItemPair, ExplicitFeedback>();
            var res = Evaluation.AverageDCG(new List<Recommendation>(), new Dictionary<UserItemPair, float?>(), 6);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void DCG_Valid1()
        {
            Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.AverageDCG(rl, prefs, 6);
            Assert.IsTrue(Math.Abs(res - 6.86113) < 0.001);
        }
        [TestMethod]
        public void DCG_Valid2()
        {
            Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.AverageDCG(rl, prefs, 6);
            Assert.IsTrue(Math.Abs(res - 6.65316) < 0.001);
        }






        [TestMethod]
        public void NDCG_Empty()
        {
            var res = Evaluation.AverageNDCG(new List<Recommendation>(), new Dictionary<UserItemPair, float?>(), 6);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void NDCG_Valid1()
        {
            Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.AverageNDCG(rl, prefs, 6);
            Assert.IsTrue(Math.Abs(res - 0.96081) < 0.001);
        }
        [TestMethod]
        public void NDCG_Valid2()
        {
            Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.AverageNDCG(rl, prefs, 6);
            Assert.IsTrue(Math.Abs(res - 0.76121) < 0.001);
        }





        void Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> dict)
        {
            var rl1 = new List<RecommendedItem> {
                new RecommendedItem{ Item = new DummyItem { Id = 0 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 1 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 2 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 3 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 4 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 5 }, ExpectedPreference = 0 }
            };

            rl = new List<Recommendation> {
                new Recommendation(1, rl1, DateTime.MinValue, 0)
            };

            dict = new Dictionary<UserItemPair, float?>{
                { new UserItemPair(1, 0), 3 },
                { new UserItemPair(1, 1), 2 },
                { new UserItemPair(1, 2), 3 },
                { new UserItemPair(1, 3), 0 },
                { new UserItemPair(1, 4), 1 },
                { new UserItemPair(1, 5), 2 }
            };
        }

        void Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> dict)
        {
            var rl1 = new List<RecommendedItem> {
                new RecommendedItem{ Item = new DummyItem { Id = 0 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 1 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 2 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 3 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 4 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 5 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 6 }, ExpectedPreference = 0 },
                new RecommendedItem{ Item = new DummyItem { Id = 7 }, ExpectedPreference = 0 }
            };

            rl = new List<Recommendation> {
                new Recommendation(1, rl1, DateTime.MinValue, 0)
            };

            dict = new Dictionary<UserItemPair, float?>{
                { new UserItemPair(1, 0), 3 },
                { new UserItemPair(1, 1), 2 },
                { new UserItemPair(1, 2), 0 },
                { new UserItemPair(1, 3), 3 },
                { new UserItemPair(1, 4), 1 },
                { new UserItemPair(1, 5), 2 },
                { new UserItemPair(1, 6), 2 },
                { new UserItemPair(1, 7), 3 }
            };
        }
    }
}
