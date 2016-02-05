using System;
using System.Collections.Generic;
using System.Text;

namespace settingslib
{
    /// <summary>
    /// Helps out creating queries
    /// </summary>
    internal class DbQueryHelper
    {
        public static class TableNames
        {
            public const string SETTING = "Setting";
            public const string ENTITYSETTING = "EntitySetting";
        }
        private readonly string _prefix;
        private const int SettingKeyFieldSize = 100;
        private const int EntityIdFieldSize = 100;
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
        internal string GenerateQueryForTableExistence(string tableRootName)
        {
            return string.Format(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = '" + GetTableName(tableRootName) + "'");
        }


        internal string GenerateQueryToCreateTable(string tableRootName)
        {
            tableRootName = tableRootName.ToLowerInvariant();
            string query;

            if (tableRootName.Equals(TableNames.SETTING.ToLowerInvariant()))
            {
                query = string.Format(" " +
                    "CREATE TABLE {0} (" + 
                        "SettingId INT IDENTITY(1, 1) PRIMARY KEY, " +
                        "SettingKey VARCHAR({1}) NOT NULL, " +
                        "[Description] VARCHAR(200) NULL, " +
                        "isVendorLevel BIT NOT NULL DEFAULT(0), " +
                        "IsTenantLevel BIT NOT NULL DEFAULT(0), " +
                        "IsUserLevel BIT NOT NULL DEFAULT(0), " +
                        "IsActive BIt NOT NULL DEFAULT(1), " +
                        "DateCreated SMALLDATETIME NOT NULL DEFAULT(GETDATE()) " +
                        ")", GetTableName(TableNames.SETTING),
                        SettingKeyFieldSize);
            }
            else if (tableRootName.Equals(TableNames.ENTITYSETTING))
            {
                query = string.Format(" " +
                    "CREATE TABLE {0} (" +
                        "SettingId INT FOREIGN KEY REFERENCES Setting(SettingId)," +
                        "EntityId VARCHAR({1}) NOT NULL," +
                        "SettingValue VARCHAR({2}) NULL," +
                        "DateLastUpdated SMALLDATETIME NOT NULL DEFAULT(GETDATE())," +
                        "PRIMARY KEY(SettingId, EntityId)" +
                    ")", GetTableName(TableNames.ENTITYSETTING),
                    EntityIdFieldSize,
                    SettingValueFieldSize);
            }
            else
            {
                throw new Exception(string.Format("table name '{0}' not recognized", GetTableName(tableRootName)));
            }

            return query;
        }
        #endregion //DB tables-related

        #region Settings-Related

        internal string CreateNewSettingQuery(bool isVendorLevel, bool isTenantLevel, bool isUserLevel, string key)
        {
            key = key.Substring(0, Math.Min(key.Length, SettingKeyFieldSize));
            string query = string.Format("INSERT INTO {4}(SettingKey, IsVendorLevel, IsTenantLevel, IsUserLevel, IsActive) " +
                           " VALUES('{0}', {1}, {2}, {3}, 1) ",
                           key, 
                           isVendorLevel ? "1" : "0",
                           isTenantLevel ? "1" : "0",
                           isUserLevel ? "1" : "0",
                           GetTableName(TableNames.SETTING));
            return query;
        }

        internal string GetSettingQuery(bool isVendorLevel, bool isTenantLevel, bool isUserLevel, string key, string entityId)
        {
            key = key.Substring(0, Math.Min(key.Length, SettingKeyFieldSize));
            entityId = entityId.Substring(0, Math.Min(entityId.Length, EntityIdFieldSize));

            string query = string.Format("SELECT TOP 1 s.SettingId, s.SettingKey, es.EntityId, es.SettingValue " +
                           " FROM {0} s LEFT JOIN {1} es ON s.SettingId = es.SettingId " +
                           " WHERE s.IsActive = 1" +
                           " AND s.SettingKey='{2}' " +
                           " AND es.EntityId='{3}' " +
                           " AND s.IsVendorLevel={4} " +
                           " AND s.IsTenantLevel={5} " +
                           " AND s.IsUserLevel={6} ",
                           GetTableName(TableNames.SETTING),
                           GetTableName(TableNames.ENTITYSETTING),
                           key,
                           entityId,
                           isVendorLevel ? "1" : "0",
                           isTenantLevel ? "1" : "0",
                           isUserLevel ? "1" : "0");
            return query;
        }

        internal string SetSettingQuery(bool isVendorLevel, bool isTenantLevel, bool isUserLevel, string key, string entityId, string value)
        {
            key = key.Substring(0, Math.Min(key.Length, SettingKeyFieldSize));
            entityId = entityId.Substring(0, Math.Min(entityId.Length, EntityIdFieldSize));
            value = value.Substring(0, Math.Min(value.Length, SettingValueFieldSize));

            string whereClause = string.Format(" IsVendorLevel={0} AND IsTenantLevel={1} AND IsUserLevel={2} AND SettingKey='{3}' ",
                isVendorLevel ? "1" : "0",
                isTenantLevel ? "1" : "0",
                isUserLevel ? "1" : "0",
                key);
            string query = string.Format("IF(EXISTS(SELECT * FROM {0} WHERE SettingId=(SELECT SettingId FROM {1} WHERE {2}) AND EntityId='{3}'))" +
                          "UPDATE {0} SET Value = {4} WHERE SettingId=(SELECT SettingId FROM {1} WHERE {2}) AND EntityId='{3}' " +
                          "ELSE" +
                          "INSERT {0}(SettingId, EntityId, SettingValue) VALUES((SELECT SettingId FROM {1} WHERE {2}), {3}, {4}) ", 
                          GetTableName(TableNames.ENTITYSETTING),
                          GetTableName(TableNames.SETTING),
                          whereClause,
                          entityId,
                          value);
            return query;
        }



        /*
         * CreateNewSetting
         * GetSettingValue
         * SetSettingValue
         */
        #endregion //Settings-related

    }

}
