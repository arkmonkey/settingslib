using System;
using System.Data.SqlClient;

namespace settingslib
{
    public class DataSourceInterfacer
    {
        private string _connString;
        private string _prefix;

        internal enum DbTables
        {
            Setting,
            EntitySetting
        }

        public DataSourceInterfacer(string connectionString, string dbTablePrefix = "")
        {
            _connString = connectionString;
            _prefix = dbTablePrefix;
        }

        public string Prefix { get { return _prefix; } }

        public bool TablesExist()
        {
            return DoesIndividualDbTableExists("right") && DoesIndividualDbTableExists("userright");
        }

        public void BuildTables()
        {
            CreateTable("right");
            CreateTable("userright");
        }

        private SqlConnection _connection;
        internal SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_connString);
                }
                return _connection;
            }
        }

        /// <summary>
        /// Get the actual table name based on the root name of the table
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private string GetTableName(string root)
        {
            return string.Format("{0}{1}", _prefix, root);
        }

        /// <summary>
        /// generates the query to determine if a table exists
        /// </summary>
        /// <param name="tableRootName"></param>
        /// <returns></returns>
        private string GenerateQueryForTableExistence(string tableRootName)
        {
            return string.Format(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = '" + GetTableName(tableRootName) + "'");
        }

        /// <summary>
        /// return true if the table exists, false if otherwise
        /// </summary>
        /// <param name="tableRootName"></param>
        /// <returns></returns>
        private bool DoesIndividualDbTableExists(string tableRootName)
        {
            string query = GenerateQueryForTableExistence(tableRootName);
            SqlCommand cmd = new SqlCommand(query, this.Connection);
            return ((int)cmd.ExecuteScalar()) > 0;
        }

        private string GenerateQueryToCreateTable(string tableRootName)
        {
            tableRootName = tableRootName.ToLowerInvariant();
            string query;

            if (tableRootName.Equals("right"))
            {
                query = string.Format(" " +
                    "CREATE TABLE [{0}](" +
                        "RightId			INT	NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                        "RightCategory		VARCHAR(200)," +
                        "RightName			VARCHAR(200) NOT NULL" +
                    ")", GetTableName("Right"));
            }
            else if (tableRootName.Equals("userright"))
            {
                query = string.Format(" " +
                    "CREATE TABLE {0}(" +
                        "UserRightId		INT NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                        "RightCategory	    VARCHAR(200) NULL," +
                        "RightName          VARCHAR(200) NOT NULL," +
                        "ContextId		    INT NOT NULL," +
                        "[Value]			BIT NULL" +
                    ")", GetTableName("UserRight"));
            }
            else
            {
                throw new Exception(string.Format("table name '{0}' not recognized", GetTableName(tableRootName)));
            }

            return query;
        }

        private void CreateTable(string tableRootName)
        {
            string commandText = GenerateQueryToCreateTable(tableRootName);
            SqlCommand cmd = new SqlCommand(commandText, this.Connection);
            cmd.ExecuteNonQuery();
        }
    }
}
