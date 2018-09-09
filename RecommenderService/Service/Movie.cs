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

        /// <summary>
        /// Creates new instance with specified parameters.
        /// </summary>
        /// <param name="id">Identifier of the movie / tv show.</param>
        /// <param name="name">Name of the movie / tv show.</param>
        /// <param name="year">Year the movie was release / tv show aired.</param>
        /// <param name="runtime">Length of the movie / tv show.</param>
        /// <param name="genres">Genres of the movie / tv show.</param>
        internal Movie(int id, string name, string year, string runtime, string[] genres)
        {
            Id = id;
            Name = name;
            Year = year;
            Runtime = runtime;
            Genres = genres;
        }

        /// <summary>
        /// Loads poster based on movie ID.
        /// </summary>
        public void LoadPoster()
        {
            var posterFile = Program.DataFolder + "/IMDB/posters/" + Id + ".jpeg";
            if (File.Exists(posterFile))
            {
                Poster = ImageToString(new Bitmap(posterFile));
            }
        }

        /// <summary>
        /// Creates a clone of this object with the same values except poster.
        /// </summary>
        /// <returns>Clone of this Movie.</returns>
        public Movie Clone()
        {
            return new Movie(Id, Name, Year, Runtime, Genres);
        }
        
        /// <summary>
        /// Converts image to a string representation.
        /// </summary>
        /// <param name="img">Image to be converted.</param>
        /// <returns>String form of the image.</returns>
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

        public override string ToString()
        {
            return $"{Id}:{Name}";
        }
    }
}
