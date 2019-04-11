using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class RatingBasedTests
    {
        [TestMethod]
        public void MAE_Empty1()
        {
            var res = Evaluation.MAE(new List<Recommendation>(), new Dictionary<UserItemPair, float?>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void MAE_NoIntersect()
        {
            NoIntersect(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.MAE(rl, prefs);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void MAE_Valid1()
        {
            Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.MAE(rl, prefs);
            Assert.IsTrue(Math.Abs(res - 2) < 0.001);
        }
        [TestMethod]
        public void MAE_Valid2()
        {
            Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.MAE(rl, prefs);
            Assert.IsTrue(Math.Abs(res - 4) < 0.001);
        }


        
        [TestMethod]
        public void RMSE_Empty()
        {
            var res = Evaluation.RMSE(new List<Recommendation>(), new Dictionary<UserItemPair, float?>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void RMSE_NoIntersect()
        {
            NoIntersect(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.RMSE(rl, prefs);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void RMSE_Valid1()
        {
            Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.RMSE(rl, prefs);
            Assert.IsTrue(Math.Abs(res - 2.94392) < 0.001);
        }
        [TestMethod]
        public void RMSE_Valid2()
        {
            Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs);
            var res = Evaluation.RMSE(rl, prefs);
            Assert.IsTrue(Math.Abs(res - 4.94975) < 0.001);
        }



        /// <summary>
        /// Setup a scenario where we don't have any known preference for recommended feedback.
        /// </summary>
        /// <param name="rl">List of recommendations.</param>
        /// <param name="uip">List of user preferences.</param>
        void NoIntersect(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs)
        {
            rl = new List<Recommendation>();
            var rl1 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 1 },
                    ExpectedPreference = 3
                },
                new RecommendedItem {
                    Item = new DummyItem { Id = 2 },
                    ExpectedPreference = 7
                }
            };

            var rl2 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 15 },
                    ExpectedPreference = 8
                }
            };

            var rl3 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 5 },
                    ExpectedPreference = 2
                }
            };
            
            rl.Add(new Recommendation(1, rl1, DateTime.MinValue, 0));
            rl.Add(new Recommendation(1, rl2, DateTime.MinValue, 0));
            rl.Add(new Recommendation(2, rl3, DateTime.MinValue, 0));


            prefs = new Dictionary<UserItemPair, float?>{
                { new UserItemPair(1, 4), 6 },
                { new UserItemPair(5, 4), 1 },
                { new UserItemPair(5, 2), 3 }
            };
        }

        /// <summary>
        /// Setup a simple valid scenario.
        /// </summary>
        /// <param name="rl">List of recommendations.</param>
        /// <param name="uip">List of user preferences.</param>
        void Simple1(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs)
        {
            rl = new List<Recommendation>();
            var rl1 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 1 },
                    ExpectedPreference = 3
                },
                new RecommendedItem {
                    Item = new DummyItem { Id = 2 },
                    ExpectedPreference = 7
                }
            };

            var rl2 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 15 },
                    ExpectedPreference = 8
                }
            };

            var rl3 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 5 },
                    ExpectedPreference = 2
                }
            };


            rl.Add(new Recommendation(1, rl1, DateTime.MinValue, 0));
            rl.Add(new Recommendation(1, rl2, DateTime.MinValue, 0));
            rl.Add(new Recommendation(2, rl3, DateTime.MinValue, 0));


            prefs = new Dictionary<UserItemPair, float?>{
                { new UserItemPair(1,  2), 6 },
                { new UserItemPair(1, 15), 3 },
                { new UserItemPair(2,  5), 2 },
                { new UserItemPair(5,  2), 3 }
            };
        }
        
        /// <summary>
        /// Setup a simple valid scenario.
        /// </summary>
        /// <param name="rl">List of recommendations.</param>
        /// <param name="uip">List of user preferences.</param>
        void Simple2(out List<Recommendation> rl, out Dictionary<UserItemPair, float?> prefs)
        {
            rl = new List<Recommendation>();
            var rl1 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 1 },
                    ExpectedPreference = 3
                },
                new RecommendedItem {
                    Item = new DummyItem { Id = 2 },
                    ExpectedPreference = 7
                }
            };

            var rl2 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 15 },
                    ExpectedPreference = 8
                }
            };

            var rl3 = new List<RecommendedItem> {
                new RecommendedItem {
                    Item = new DummyItem { Id = 5 },
                    ExpectedPreference = 2
                }
            };


            rl.Add(new Recommendation(1, rl1, DateTime.MinValue, 0));
            rl.Add(new Recommendation(1, rl2, DateTime.MinValue, 0));
            rl.Add(new Recommendation(2, rl3, DateTime.MinValue, 0));



            prefs = new Dictionary<UserItemPair, float?>{
                { new UserItemPair(1,  1),  6 },
                { new UserItemPair(1,  2),  7 },
                { new UserItemPair(1, 15),  3 },
                { new UserItemPair(2,  5), 10 }
            };
        }

        UserItemPair NewUIP(int userId, int itemId) => new UserItemPair(userId, itemId);
    }
}
