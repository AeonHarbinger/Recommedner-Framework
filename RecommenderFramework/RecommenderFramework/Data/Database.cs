using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Class containing available data.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// All users.
        /// </summary>
        public IDictionary<int, User> Users;
        /// <summary>
        /// All items.
        /// </summary>
        public IDictionary<int, Item> Items;
        /// <summary>
        /// Matrix containing float values of known preference for given user towards given item. 
        /// </summary>
        SparseMatrix<float> KnownPreference;

        /// <summary>
        /// Creates new instance of database.
        /// </summary>
        /// <param name="users">All users.</param>
        /// <param name="items">All items.</param>
        /// <param name="pref">Known user-to-item preference.</param>
        public Database(Dictionary<int, User> users, Dictionary<int, Item> items, SparseMatrix<float> pref)
        { 
            Users = users;
            Items = items;
            KnownPreference = pref;
        }

        /// <summary>
        /// Gets a user with given ID.
        /// </summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>User with given ID.</returns>
        public User GetUser(int id) => Users[id];
        /// <summary>
        /// Gets an item with given ID.
        /// </summary>
        /// <param name="id">ID of the item.</param>
        /// <returns>Item with given ID.</returns>
        public Item GetItem(int id) => Items[id];

        /// <summary>
        /// Returns whether users preference for an item is known.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="itemId">ID of the item.</param>
        /// <returns>True, if users preference for an item is known.</returns>
        public bool PreferenceIsKnown(int userId, int itemId)
        {
            return KnownPreference.Contains(userId, itemId);
        }

        /// <summary>
        /// Acesses users preference for an item.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="itemId">ID of the item.</param>
        /// <returns>Preference of a user toward given item.</returns>
        public float this[int userId, int itemId] => KnownPreference[userId, itemId];

        /// <summary>
        /// Removes all elements from the database.
        /// </summary>
        public void Clear()
        {
            Users.Clear();
            Items.Clear();
            KnownPreference.Clear();
        }

        /// <summary>
        /// Returns all items, for which a certain users preference is known.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>All items, where preference is known and the value of preference.</returns>
        public Dictionary<int, float> KnownForUser(int userId)
        {
            var list = new Dictionary<int, float>();
            foreach (var item in Items.Values)
            {
                if (KnownPreference.Contains(userId, item.Id))
                    list.Add(item.Id, this[userId, item.Id]);
            }

            return list;
        }
    }
}

