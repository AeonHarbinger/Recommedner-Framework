using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents a position within matrix.
    /// </summary>
    struct RowColumnPair
    {
        readonly int row;
        readonly int col;

        public RowColumnPair(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    /// <summary>
    /// Represents a 2D matrix, which is sparsely populated by values.
    /// </summary>
    /// <typeparam name="T">Type of value stored in matrix.</typeparam>
    public class SparseMatrix<T>
    {
        /// <summary>
        /// Contains values stored in matrix.
        /// </summary>
        Dictionary<RowColumnPair, T> matrix = new Dictionary<RowColumnPair, T>();

        /// <summary>
        /// Returns whether matrix contains value at given position.
        /// </summary>
        /// <param name="userId">Row number.</param>
        /// <param name="itemId">Column number.</param>
        /// <returns>True, if given position contains value.</returns>
        public bool Contains(int row, int col)
        {
            return matrix.ContainsKey(new RowColumnPair(row, col));
        }

        /// <summary>
        /// Accesses given position in a matrix.
        /// </summary>
        /// <param name="userId">Row number.</param>
        /// <param name="itemId">Column number.</param>
        /// <returns>Value stored at given position.</returns>
        public T this[int row, int col]
        {
            get { return matrix[new RowColumnPair(row, col)]; }
            set { matrix[new RowColumnPair(row, col)] = value; }
        }

        /// <summary>
        /// Removes all elements from the matrix.
        /// </summary>
        public void Clear()
        {
            var keys = new List<RowColumnPair>();
            foreach (var item in matrix)
            {
                keys.Add(item.Key);
            }

            foreach (var key in keys)
            {
                matrix.Remove(key);
            }
        }
    }
}
