using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationTests
{
    class DummyDatabase : IDatabase
    {
        readonly Dictionary<UserItemPair, ExplicitFeedback> feed;

        public DummyDatabase(Dictionary<UserItemPair, ExplicitFeedback> f)
        {
            feed = f;
        }

        public User GetUser(int id) => throw new NotImplementedException();
        public Item GetItem(int id) => throw new NotImplementedException();

        public List<Feedback> GetFeedback(int userId, int itemId)
        {
            var uiFeedback = new List<Feedback>();
            if (feed.TryGetValue(new UserItemPair(userId, itemId), out ExplicitFeedback f))
                uiFeedback.Add(f);
            return uiFeedback;
        }
        public List<Feedback> GetUserFeedback(int userId) => throw new NotImplementedException();
        public List<Feedback> GetItemFeedback(int itemId) => throw new NotImplementedException();

        public List<Recommendation> GetSystemRecommendations(string rsName, string rsVersion) => throw new NotImplementedException();
        public void SaveSystemRecommendation(string rsName, string rsVersion, Recommendation rl)
        {
            throw new NotImplementedException();
        }
    }
}
