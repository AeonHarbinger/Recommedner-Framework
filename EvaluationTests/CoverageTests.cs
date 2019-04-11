using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderFramework;

namespace EvaluationTests
{
    [TestClass]
    public class CoverageTests
    {
        [TestMethod]
        public void User_Empty()
        {
            var res = Evaluation.UserCoverage(new List<User>(), new DummyRecommender());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void User_Valid1()
        {
            var users = new List<User> {
                new DummyUser{ Id = 2 }
            };
            var res = Evaluation.UserCoverage(users, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 1) < 0.001);
        }
        [TestMethod]
        public void User_Valid2()
        {
            var users = new List<User> {
                new DummyUser{ Id = 2 },
                new DummyUser{ Id = 5 },
                new DummyUser{ Id = 4 },
                new DummyUser{ Id = 6 }
            };
            var res = Evaluation.UserCoverage(users, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 0.75) < 0.001);
        }
        [TestMethod]
        public void User_Valid3()
        {
            var users = new List<User> {
                new DummyUser{ Id = 2  },
                new DummyUser{ Id = 5  },
                new DummyUser{ Id = 4  },
                new DummyUser{ Id = 6  },
                new DummyUser{ Id = 11 },
                new DummyUser{ Id = 13 },
                new DummyUser{ Id = 7  }
            };
            var res = Evaluation.UserCoverage(users, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 0.42857) < 0.001);
        }



        [TestMethod]
        public void Item_Empty()
        {
            var res = Evaluation.ItemCoverage(new List<Item>(), new DummyRecommender());
            Assert.IsTrue(double.IsNaN(res));
        }
        [TestMethod]
        public void Item_Valid1()
        {
            var items = new List<Item> {
                new DummyItem { Id = 1 },
            };
            var res = Evaluation.ItemCoverage(items, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 0) < 0.001);
        }
        [TestMethod]
        public void Item_Valid2()
        {
            var items = new List<Item> {
                new DummyItem { Id = 1 },
                new DummyItem { Id = 7 },
                new DummyItem { Id = 2 },
                new DummyItem { Id = 4 }
            };
            var res = Evaluation.ItemCoverage(items, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 0.5) < 0.001);
        }
        [TestMethod]
        public void Item_Valid3()
        {
            var items = new List<Item> {
                new DummyItem { Id = 1  },
                new DummyItem { Id = 7  },
                new DummyItem { Id = 2  },
                new DummyItem { Id = 4  },
                new DummyItem { Id = 6  },
                new DummyItem { Id = 12 },
                new DummyItem { Id = 8  }
            };
            var res = Evaluation.ItemCoverage(items, new DummyRecommender());
            Assert.IsTrue(Math.Abs(res - 0.71429) < 0.001);
        }


        class DummyRecommender : IRecommenderSystem
        {
            public string Name    => throw new NotImplementedException();
            public string Version => throw new NotImplementedException();

            public bool CanRecommendItem  (Item item) => (item.Id % 2) == 0;
            public bool CanRecommendToUser(User user) => (user.Id % 2) == 0;

            public bool CanPredictPreference  (User user, Item item) => throw new NotImplementedException();
            public float GetExpectedPreference(User user, Item item) => throw new NotImplementedException();

            public List<RecommendedItem> GetRanking(User user, List<Item> fromItems) => throw new NotImplementedException();
            public List<RecommendedItem> GetRecommendation(User user, List<Item> fromItems, int count) => throw new NotImplementedException();
            public void HandleFeedback(Feedback feedback) => throw new NotImplementedException();
        }

    }
}
