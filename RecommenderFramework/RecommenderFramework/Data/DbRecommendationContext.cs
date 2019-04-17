using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents a connection to database containing recommendation data.
    /// </summary>
    [Database]
    internal class DbRecommendationContext : DataContext
    {
        /// <summary>
        /// Creates new database context for given database. 
        /// Sets up tables for recommendation data (if they don't exist).
        /// </summary>
        /// <param name="conStr">Connection string for the database.</param>
        public DbRecommendationContext(string conStr) : base(conStr)
        {
            string createReccommendation = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + nameof(DbRecommendation) + "' and xtype='U')"
                                            + "CREATE TABLE [" + nameof(DbRecommendation) + "] ("
                                            + "[Id] INT NOT NULL,"
                                            + "[Name] VARCHAR(50) NULL,"
                                            + "[Version] VARCHAR(50) NULL,"
                                            + "[UserID] INT NULL,"
                                            + "[AtTime] DATETIME NULL,"
                                            + "[ResponseTime] INT NULL,"
                                            + "PRIMARY KEY([Id]));";

            using (SqlCommand cmd = new SqlCommand(createReccommendation, new SqlConnection(conStr)))
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }


            string createRecItem = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + nameof(DbRecommendedItem) + "' and xtype='U')"
                                    + "CREATE TABLE [" + nameof(DbRecommendedItem) + "] ("
                                        + "[RecommendationId] INT NOT NULL,"
                                        + "[ItemId] INT NOT NULL,"
                                        + "[ExpectedPreference] FLOAT NULL,"
                                        + "PRIMARY KEY([Id]));";

            using (SqlCommand cmd = new SqlCommand(createRecItem, new SqlConnection(conStr)))
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Table of recommendations.
        /// </summary>
        public Table<DbRecommendation>  Recommendations;
        /// <summary>
        /// Table of recommended items.
        /// </summary>
        public Table<DbRecommendedItem> RecommendedItems;
    }
}
