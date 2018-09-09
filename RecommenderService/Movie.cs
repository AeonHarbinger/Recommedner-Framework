using RecommenderFramework;
using System;
using System.Drawing;
using System.IO;

namespace RecommenderService
{
    [Serializable]
    public class Movie : Item
    {
        public string Name;
        public string Year;
        public string Runtime;
        public string[] Genres;
        public string Poster;

        internal Movie(int id, string name, string year, string runtime, string[] genres)
        {
            Id = id;
            Name = name;
            Year = year;
            Runtime = runtime;
            Genres = genres;
            
            var posterFile = Program.DataFolder + "/IMDB/posters/" + Id + ".jpeg";
            if (File.Exists(posterFile))
            {
                Poster = ImageToString(new Bitmap(posterFile));
            }
        }

        public override string ToString()
        {
            return $"{Id}:{Name}";
        }
        
        string ImageToString(Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return Convert.ToBase64String(byteArray);
        }
    }
}
