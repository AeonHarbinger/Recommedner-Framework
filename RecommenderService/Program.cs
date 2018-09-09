using RecommenderFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    public class Program
    {
        public static readonly string DataFolder = "C:/Users/Ondra/Desktop/Data/reduced/";
        public static SystemManager Manager;

        static void Main(string[] args)
        {
            Logger.Start();

            Logger.Message(0, "Starting Initialization.");
            Manager = Initializer.Initialize();
            Logger.Message(0, "System initialized.");

            RecommenderService host = new RecommenderService();
            var channel = new TcpChannel(42123);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType
                (typeof(RecommenderService), "RecService", WellKnownObjectMode.Singleton);

            Logger.Message(0, "Hosting started.");
            Console.ReadLine();
            
            Logger.Message(0, "Saving feedback.");
            SaveFeedback(DataFolder + "feedback.txt");
            Logger.Message(0, "Feedback saved.");

            var trackers = Manager.GetAvailableTrackers();
            trackers[0].Value.SaveRecommendations(DataFolder + "SVDRecom.txt");
            trackers[1].Value.SaveRecommendations(DataFolder + "UIBRecom.txt");
        }

        static void SaveFeedback(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                var saved = new HashSet<Feedback>();
                foreach (var nameTrackerPair in Manager.GetAvailableTrackers())
                {
                    List<Feedback> feedbackList = nameTrackerPair.Value.AllFeedback.Values();
                    foreach (var feedback in feedbackList)
                    {
                        if (saved.Contains(feedback)) continue;
                        
                        char type;
                        if (feedback is ExplicitFeedback) type = 'e';
                        else if (feedback is ImplicitFeedback) type = 'i';
                        else throw new Exception("Unknown feedback type \"" + feedback.GetType().Name + "\"");

                        writer.WriteLine(type);
                        writer.WriteLine(feedback.AtTime.ToString() + '|' + feedback.UserId + '|' + feedback.ItemId);
                        if (type == 'e') writer.WriteLine(((ExplicitFeedback)feedback).Preference);
                        else
                        {
                            writer.WriteLine(((ImplicitFeedback)feedback).Type);
                            writer.WriteLine(((ImplicitFeedback)feedback).Value);
                        }

                        saved.Add(feedback);
                    }
                }
            }
        }
    }
}
