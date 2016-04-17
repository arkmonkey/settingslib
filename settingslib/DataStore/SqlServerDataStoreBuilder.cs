using System;
using System.Data;
using System.Data.SqlClient;

namespace settingslib.DataStore
{
    public class SqlServerDataStoreBuilder : IDataStoreBuilder
    {
        private readonly string _prefix;
        private DbQueryHelper _queryHelper;
        private readonly SqlConnection _conn;

        public SqlServerDataStoreBuilder(string prefix, SqlConnection conn)
        {
            _prefix = prefix;
            _conn = conn;
        }

        public bool IsDataStoreValid()
        {
            return TableExists(DbQueryHelper.TableNames.SETTING, _conn)
                   && TableExists(DbQueryHelper.TableNames.SETTING_INSTANCE, _conn);
        }

        public void BuildDataStore()
        {
            BuildTable(DbQueryHelper.TableNames.SETTING, _conn);
            BuildTable(DbQueryHelper.TableNames.SETTING_INSTANCE, _conn);
        }

        #region Helpers
        private DbQueryHelper QueryHelper
        {
            get
            {
                if (_queryHelper == null)
                {
                    _queryHelper = new DbQueryHelper(_prefix);
                }
                return _queryHelper;
            }
        }

        private bool TableExists(string tableRootName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableRootName) || conn == null)
            {
                throw new ArgumentNullException();
            }
            if (conn.State == ConnectionState.Closed)
            {
                throw new ArgumentException("Connection is closed");
            }
            var query = QueryHelper.TableExistenceQuery(tableRootName);
            SqlCommand cmd = new SqlCommand(query, conn);
            var result = (int) cmd.ExecuteScalar();
            return result == 1;
        }

        private void BuildTable(string tableRootName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableRootName) || conn == null)
            {
                throw new ArgumentNullException();
            }
            if (conn.State == ConnectionState.Closed)
            {
                throw new ArgumentException("Connection is closed");
            }
            var query = QueryHelper.CreateTableQuery(tableRootName);
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
        #endregion

    }
}
