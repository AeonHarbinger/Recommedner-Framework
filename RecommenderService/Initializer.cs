using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    static class Initializer
    {
        /// <summary>
        /// Loads additional data about the movies into the database.
        /// </summary>
        /// <param name="items">Items whose info should be loaded.</param>
        static void LoadAdditionalItemInfo(Dictionary<int, Item> items)
        {
            using (StreamReader movies = new StreamReader(Program.DataFolder + "IMDB/MovieData.txt"))
            {
                movies.ReadLine();

                string line;
                while ((line = movies.ReadLine()) != null)
                {
                    var split = line.Split('|');
                    int id = int.Parse(split[0]);

                    // Remove Item object.
                    if (items.ContainsKey(id))
                        items.Remove(id);
                    
                    // Replace it with Movie object.
                    var movie = new Movie(id, split[1], split[2], split[3], split[4].Split(','));
                    items.Add(id, movie);
                }
            }
        }

        /// <summary>
        /// Loads user and item info, aswell as known user to item preference.
        /// </summary>
        /// <returns>Database containing user and item info.</returns>
        static Database LoadDatabase()
        {
            var users = new Dictionary<int, User>();
            var items = new Dictionary<int, Item>();
            var pref = new SparseMatrix<float>();

            // Load known preference.
            long ratingCount = 0;
            using (StreamReader ratings = new StreamReader(Program.DataFolder + "training.csv"))
            {
                string line;
                while ((line = ratings.ReadLine()) != null)
                {
                    ratingCount++;
                    string[] split = line.Split(',');
                    int uid = int.Parse(split[0]);
                    int iid = int.Parse(split[1]);
                    float rating = float.Parse(split[2]);

                    if (!users.ContainsKey(uid)) users.Add(uid, new User() { Id = uid });
                    if (!items.ContainsKey(iid)) items.Add(iid, new Item() { Id = iid });
                    pref[uid, iid] = rating;
                }
            }

            Logger.Message(2, "Loading additional item data.");
            LoadAdditionalItemInfo(items);
            Logger.Message(2, "Additional item data loaded.");

            Logger.Message(1, "Database loaded.");
            Logger.Message(1, "- contains:");
            Logger.Message(1, string.Format("{0,15}", Format(users.Count)) + " users");
            Logger.Message(1, string.Format("{0,15}", Format(items.Count)) + " items");
            Logger.Message(1, string.Format("{0,15}", Format(ratingCount)) + " ratings");

            return new Database(users, items, pref);
        }

        static List<Feedback> LoadFeedback(string fileName)
        {
            var allFeedback = new List<Feedback>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var type = line;
                    var lineSplit = reader.ReadLine().Split('|');
                    Feedback feedback;
                    if (type == "e")
                    {
                        feedback = new ExplicitFeedback();
                        ((ExplicitFeedback)feedback).Preference = float.Parse(reader.ReadLine());
                    }
                    else if (type == "i")
                    {
                        feedback = new ImplicitFeedback();
                        ((ImplicitFeedback)feedback).Type  = reader.ReadLine();
                        ((ImplicitFeedback)feedback).Value = reader.ReadLine();
                    }
                    else throw new Exception("Unknown feedback type \"" + type + "\"");

                    feedback.AtTime = DateTime.Parse(lineSplit[0]);
                    feedback.UserId = int.Parse(lineSplit[1]);
                    feedback.ItemId = int.Parse(lineSplit[2]);

                    allFeedback.Add(feedback);
                }
            }

            return allFeedback;
        }

        /// <summary>
        /// Loads recommender algorithms and passes them to 
        /// </summary>
        /// <param name="manager">Manager that will receive the recommender systems.</param>
        /// <param name="database">Database that recommender systems work with.</param>
        static void LoadRecommenders(SystemManager manager, Database database, List<Feedback> feedback)
        {
            string recName1 = "SVD";
            string rstName1 = recName1 + "Tracker";
            Logger.Message(2, $"Loading \"{recName1}\".");
            var rec1 = new SVDImplemented("1.0", database);
            rec1.LoadModel(Program.DataFolder + $"Trained{recName1}.txt");
            manager.AddRecommender(recName1, rec1);
            var track1 = new RecommenderSystemTracker(rstName1, rec1)
            {
                Aggregator = MyAggregator
            };
            track1.AddFeedback(feedback);
            track1.LoadRecommendations(Program.DataFolder + recName1 + "Recom.txt", database);
            manager.AddTracker(rstName1, track1);
            Logger.Message(2, $"Added recommender \"{recName1}\" to manager.");

            
            
            string recName2 = "UIB";
            string rstName2 = recName2 + "Tracker";
            Logger.Message(2, $"Loading \"{recName2}\".");
            var rec2 = new UIBImplemented("1.0", database);
            rec2.LoadModel(Program.DataFolder + $"Trained{recName2}.txt");
            manager.AddRecommender(recName2, rec2);
            var track2 = new RecommenderSystemTracker(rstName2, rec2)
            {
                Aggregator = MyAggregator
            };
            track2.AddFeedback(feedback);
            track2.LoadRecommendations(Program.DataFolder + recName2 + "Recom.txt", database);
            manager.AddTracker(rstName2, track2);
            Logger.Message(2, $"Added recommender \"{recName2}\" to manager.");
        }
               
        /// <summary>
        /// Initializes server side of recommender service.
        /// </summary>
        /// <returns>System manager of the service.</returns>
        public static SystemManager Initialize()
        {
            Logger.Message(1, "Loading database.");
            var database = LoadDatabase();

            var manager = new SystemManager(database);

            Logger.Message(1, "Loading feedback.");
            var feedback = LoadFeedback(Program.DataFolder + "feedback.txt");
            Logger.Message(1, "Feedback loaded.");

            Logger.Message(1, "Loading recommenders.");
            LoadRecommenders(manager, database, feedback);
            Logger.Message(1, "Recommenders loaded.");

            manager.Assigner = (id) => 
            {
                if (id % 2 == 0) return "SVDTracker";
                else return "UIBTracker";
            };

            return manager;
        }

        /// <summary>
        /// Aggregates users feedback for specific item. Takes the latest explicit rating the user gave the item.
        /// </summary>
        /// <param name="feedback">List of feedback user gave to an item.</param>
        /// <returns>Null, if preference cannot be extrapolated.</returns>
        static float? MyAggregator(List<Feedback> feedback)
        {
            var expl = new List<ExplicitFeedback>();
            foreach (var feed in feedback)
            {
                if (feed is ExplicitFeedback) expl.Add((ExplicitFeedback)feed);
            }
            if (expl.Count == 0) return null;
            
            DateTime latestTime = expl[0].AtTime;
            float latestPref = expl[0].Preference;
            foreach (var feed in expl)
            {
                if (DateTime.Compare(latestTime, feed.AtTime) < 0)
                {
                    latestTime = feed.AtTime;
                    latestPref = feed.Preference;
                }
            }
            return latestPref;
        }
        
        /// <summary>
        /// Takes a number and converts it to string with thousands separated.
        /// </summary>
        /// <param name="a">Number to be formated.</param>
        /// <returns>Space separated string representation.</returns>
        static string Format(long a)
        {
            string s = a.ToString();
            string r = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (i != 0 && i % 3 == 0) r = " " + r;
                r = s[s.Length - i - 1] + r;
            }

            return r;
        }
    }
}
