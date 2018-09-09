using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    public interface IRecommenderService
    {
        SimpleProvider RegisterUser(int id);
        List<Movie> GetMovies(string containing);
        float GetRating(int userId, int itemId);
    }
}
