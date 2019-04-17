using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class DiversityAndCTRTests
    {
        [TestMethod]
        public void CTR_Empty()
        {
            var res = Evaluation.CTR(new List<Recommendation>(), new List<Feedback>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void CTR_Valid1()
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
            var rl2 = new Recommendation(3,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 5 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 9 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1, rl2 };



            var feed = new List<Feedback> {
                new ExplicitFeedback { UserId = 1, ItemId = 3 },
                new ExplicitFeedback { UserId = 2, ItemId = 2 },
                new ExplicitFeedback { UserId = 1, ItemId = 2 },
                new ClickOnRecommendation { UserId = 3, ItemId = 9 },
                new ClickOnRecommendation { UserId = 1, ItemId = 3 },
                new ClickOnRecommendation { UserId = 3, ItemId = 1 }
            };

            var res = Evaluation.CTR(rl, feed);
            Assert.IsTrue(Math.Abs(res - 0.42857) < 0.001);
        }
        [TestMethod]
        public void CTR_Valid2()
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
            var rl2 = new Recommendation(3,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 5 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 9 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );
            var rl3 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 8 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 7 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 1 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1, rl2, rl3 };



            var feed = new List<Feedback> {
                new ExplicitFeedback { UserId = 1, ItemId = 3 },
                new ExplicitFeedback { UserId = 2, ItemId = 2 },
                new ExplicitFeedback { UserId = 1, ItemId = 2 },
                new DummyFeedback { UserId = 3, ItemId = 9, Type = "Test1" },
                new ClickOnRecommendation { UserId = 1, ItemId = 3 },
                new ClickOnRecommendation { UserId = 3, ItemId = 1 },
                new ClickOnRecommendation { UserId = 1, ItemId = 7 }
            };

            var res = Evaluation.CTR(rl, feed);
            Assert.IsTrue(Math.Abs(res - 0.3) < 0.001);
        }



        [TestMethod]
        public void Diversity_Empty()
        {
            var res = Evaluation.Diversity(new List<Recommendation>(), (a, b) => 0);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Diversity_Valid1()
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1, value = 0.2 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2, value = 0.6 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3, value = 0.7 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1 };
            var res = Evaluation.Diversity(rl, DummyFunctions.Diversity);
            Assert.IsTrue(Math.Abs(res - 0.33333) < 0.001);
        }
        [TestMethod]
        public void Diversity_Valid2()
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1, value = 0.2 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2, value = 0.6 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3, value = 0.7 }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );
            var rl2 = new Recommendation(2,
                new List<RecommendedItem> {
                    new RecommendedItem{ Item = new DummyItem{ Id = 1, value = 0.2 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 2, value = 0.6 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 3, value = 0.7 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 7, value = 0.1 }, ExpectedPreference = 0 },
                    new RecommendedItem{ Item = new DummyItem{ Id = 9, value = 1   }, ExpectedPreference = 0 }
                },
                DateTime.MinValue,
                0
            );

            var rl = new List<Recommendation> { rl1, rl2 };
            var res = Evaluation.Diversity(rl, DummyFunctions.Diversity);
            Assert.IsTrue(Math.Abs(res - 0.39667) < 0.001);
        }
    }
}
