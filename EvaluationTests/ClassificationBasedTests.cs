using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class ClassificationBasedTests
    {
        [TestMethod]
        public void Recall_Empty()
        {
            var res = Evaluation.Recall(new List<Recommendation>(), (_) => new List<int>());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Recall_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.Recall(rl, DummyGetRelevant);
            Assert.IsTrue(Math.Abs(res - 0.625) < 0.001);
        }
        [TestMethod]
        public void Recall_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.Recall(rl, DummyGetRelevant);
            Assert.IsTrue(Math.Abs(res - 0.6125) < 0.001);
        }



        [TestMethod]
        public void Accuracy_Empty()
        {
            var res = Evaluation.Accuracy(new List<Recommendation>(), (_) => new List<int>(), 5);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Accuracy_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.Accuracy(rl, DummyGetRelevant, 15);
            Assert.IsTrue(Math.Abs(res - 0.6) < 0.001);
        }
        [TestMethod]
        public void Accuracy_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.Accuracy(rl, DummyGetRelevant, 15);
            Assert.IsTrue(Math.Abs(res - 0.66667) < 0.001);
        }



        [TestMethod]
        public void Precision_Empty()
        {
            var res = Evaluation.Precision(new List<Recommendation>(), (a, b) => true);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Precision_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.Precision(rl, (a, b) => (a + b) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.625) < 0.001);
        }
        [TestMethod]
        public void Precision_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.Precision(rl, (a, b) => (a + b) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.53846) < 0.001);
        }


        [TestMethod]
        public void MAP_Empty()
        {
            var res = Evaluation.MAP(new List<Recommendation>(), (a, b) => true);
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void MAP_Valid1()
        {
            Simple1(out List<Recommendation> rl);
            var res = Evaluation.MAP(rl, (a, b) => (a + b) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.625) < 0.001);
        }
        [TestMethod]
        public void MAP_Valid2()
        {
            Simple2(out List<Recommendation> rl);
            var res = Evaluation.MAP(rl, (a, b) => (a + b) % 2 == 0);
            Assert.IsTrue(Math.Abs(res - 0.5125) < 0.001);
        }



        List<int> DummyGetRelevant(int userId)
        {
            if (userId == 1)
                return new List<int> { 1, 4, 5, 6, 8, 9, 10, 11 };
            else if (userId == 2)
                return new List<int> { 2, 3, 5, 11, 13 };
            else throw new NotImplementedException();
        }
               
        void Simple1(out List<Recommendation> rl)
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem { Item = new DummyItem { Id = 1  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 6  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 5  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 2  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 11 }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 4  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 7  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 13 }, ExpectedPreference = 0},
                },
                DateTime.MinValue,
                0
            );
            
            rl = new List<Recommendation> { rl1 };
        }
        void Simple2(out List<Recommendation> rl)
        {
            var rl1 = new Recommendation(1,
                new List<RecommendedItem> {
                    new RecommendedItem { Item = new DummyItem { Id = 1  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 6  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 5  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 2  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 11 }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 4  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 7  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 13 }, ExpectedPreference = 0}
                },
                DateTime.MinValue,
                0
            );
            var rl2 = new Recommendation(2,
                new List<RecommendedItem> {
                    new RecommendedItem { Item = new DummyItem { Id = 1  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 6  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 5  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 2  }, ExpectedPreference = 0},
                    new RecommendedItem { Item = new DummyItem { Id = 11 }, ExpectedPreference = 0}
                },
                DateTime.MinValue,
                0
            );

            rl = new List<Recommendation> { rl1, rl2 };
        }
    }
}
