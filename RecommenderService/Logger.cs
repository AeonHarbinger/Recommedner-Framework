using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderService
{
    static class Logger
    {
        static Stopwatch start;

        /// <summary>
        /// Starts timer.
        /// </summary>
        public static void Start()
        {
            start = Stopwatch.StartNew();
        }

        /// <summary>
        /// Gets string containing 4*n spaces.
        /// </summary>
        /// <param name="n">How large should the space be.</param>
        /// <returns>4*n spaces.</returns>
        static string Spaces(int n) => new string(' ', 4 * n);
        /// <summary>
        /// Outputs time since start and indented message. 
        /// </summary>
        /// <param name="n">How large should the indentation be.</param>
        /// <param name="msg">Message to be written.</param>
        public static void Message(int n, string msg)
        {
            Console.WriteLine(start.Elapsed.ToString().Split('.')[0] + ": " + Spaces(n) + msg);
        }
    }
}
