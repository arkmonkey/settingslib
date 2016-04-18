using System;
using System.Collections.Generic;
using System.Text;

namespace settingslib.DataStore
{
    /// <summary>
    /// Helps out creating queries
    /// </summary>
    internal class DbQueryHelper
    {
        public static class TableNames
        {
            public const string SETTING = "Setting";
            public const string SETTING_INSTANCE = "SettingInstance";
        }

        private readonly string _prefix;
        private const int SettingKeyFieldSize = 100;
        private const int InstanceIdFieldSize = 100;
        private const int SettingValueFieldSize = 500;

        public DbQueryHelper(string prefix)
        {
            _prefix = prefix;
        }

        internal string GetTableName(string tableName)
        {
            return string.Format("{0}{1}", _prefix, tableName);
        }

        #region DB tables-related
        /// <summary>
        /// generates the query to determine if a table exists
        /// </summary>
        /// <param name="tableRootName"></param>
        /// <returns></returns>
        internal string TableExistenceQuery(string tableRootName)
        {
            return string.Format(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = '" + GetTableName(tableRootName) + "'");
        }


        internal string CreateTableQuery(string tableRootName)
        {
            string effectiveTableName = GetTableName(tableRootName);
            string query;

            if (tableRootName.Equals(TableNames.SETTING, StringComparison.InvariantCultureIgnoreCase))
            {
                query = string.Format(" " +
                    "CREATE TABLE {0} (" + 
                        "SettingId INT IDENTITY(1, 1) PRIMARY KEY, " +
                        "SettingScope VARCHAR({1}) NOT NULL, " +
                        "SettingName VARCHAR({1}) NOT NULL, " +
                        "IsActive BIt NOT NULL DEFAULT(1), " +
                        "DateCreated SMALLDATETIME NOT NULL DEFAULT(GETDATE()) " +
                        ")", effectiveTableName,
                        SettingKeyFieldSize);
            }
            else if (tableRootName.Equals(TableNames.SETTING_INSTANCE, StringComparison.CurrentCultureIgnoreCase))
            {
                query = string.Format(" " +
                    "CREATE TABLE {0} (" + 
                        "SettingInstanceId INT IDENTITY(1,1) PRIMARY KEY, " +
                        "SettingId INT NOT NULL FOREIGN KEY REFERENCES {1}(SettingId)," +
                        "InstanceKey VARCHAR({2}) NOT NULL," +
                        "SettingValue VARCHAR({3}) NULL," +
                        "DateLastUpdated SMALLDATETIME NOT NULL DEFAULT(GETDATE()) " +
                    ")", effectiveTableName,
                    GetTableName(TableNames.SETTING),
                    InstanceIdFieldSize,
                    SettingValueFieldSize);
            }
            else
            {
                throw new Exception(string.Format("table name '{0}' not recognized", effectiveTableName));
            }

            return query;
        }
        #endregion //DB tables-related

        #region Settings-Related

        internal string CreateNewSettingQuery(string scope, string settingName)
        {
            string query = string.Format("INSERT INTO {0}(SettingScope, SettingName, IsActive) " +
                           " VALUES('{1}', '{2}', 1) ",
                           GetTableName(TableNames.SETTING),
                           scope,
                           settingName
                           );
            return query;
        }

        internal string GetSettingQuery(string scope, string settingName, string instanceKey)
        {
            string query = string.Format("SELECT TOP 1 s.SettingId, si.SettingInstanceId, s.IsActive, s.SettingScope, s.SettingName, si.InstanceKey, si.SettingValue " +
                           " FROM {0} s LEFT JOIN {1} si ON s.SettingId = si.SettingId " +
                           " WHERE " +
                           " s.SettingScope='{2}' " +
                           " AND s.SettingName='{3}' " +
                           " AND si.InstanceKey='{4}' ",
                           GetTableName(TableNames.SETTING),
                           GetTableName(TableNames.SETTING_INSTANCE),
                           scope,
                           settingName,
                           instanceKey);
            return query;
        }

        internal string CheckSettingExistenceQuery(string scope, string settingName)
        {
            string query = string.Format("SELECT SettingId " +
                                         "FROM {0} " +
                                         "WHERE SettingScope='{1}'" +
                                               "AND SettingName='{2}'",
                            GetTableName(TableNames.SETTING),
                            scope,
                            settingName);
            return query;
        }

        internal string CheckSettingInstanceExistenceQuery(string scope, string settingName, string instanceKey)
        {
            string query = string.Format("SELECT SettingId, SettingInstanceId, SettingValue " + 
                                         "FROM {0} " + 
                                         "WHERE SettingId=(SELECT SettingId " + 
                                                          "FROM {1} " +
                                                          "WHERE SettingScope='{2}' AND SettingName='{3}') " + 
                                               "AND InstanceKey='{4}'",
                            GetTableName(TableNames.SETTING_INSTANCE),
                            GetTableName(TableNames.SETTING),
                            scope,
                            settingName,
                            instanceKey);
            return query;
        }

        internal string InsertNewSettingInstanceQuery(int settingId, string instanceKey, string value)
        {
            string query = string.Format("INSERT INTO {0}(SettingId, InstanceKey, SettingValue) VALUES({1}, '{2}', '{3}') " +
                                         GetTableName(TableNames.SETTING_INSTANCE),
                                         settingId,
                                         instanceKey,
                                         value);
            return query;
        }

        internal string UpdateSettingInstanceQuery(int settingInstanceId, string value)
        {
            string query = string.Format("UPDATE {0} SET SettingValue = '{1}', DateLastUpdated=GETDATE() WHERE SettingInstanceId = {2}",
                                          GetTableName(TableNames.SETTING_INSTANCE),
                                          value,
                                          settingInstanceId);
            return query;
        }
        
        #endregion //Settings-related

    }

}
