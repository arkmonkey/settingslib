using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace settingslib.DataStore
{
    internal class SqlServerDataStoreServices : ISettingsDataStoreServices
    {
        private readonly string _prefix;
        private readonly SqlConnection _conn;
        private DbQueryHelper _queryHelper;

        public SqlServerDataStoreServices(string prefix, SqlConnection conn)
        {
            _prefix = prefix;
            _conn = conn;
        }

        public T Get<T>(string scope, string settingName, string instanceKey, T defaultIfNotExists) where T : struct
        {
            var query = _queryHelper.GetSettingQuery(scope, settingName, instanceKey);
            SqlCommand cmd = new SqlCommand(query, _conn);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof (T));
                try
                {
                    var converted = typeConverter.ConvertFromString(reader["SettingValue"].ToString());
                    if (converted != null)
                    {
                        return (T) converted;
                    }
                    else
                    {
                        return defaultIfNotExists;
                    }
                }
                catch (Exception)
                {
                    return defaultIfNotExists;
                }
            }
            else
            {
                return defaultIfNotExists;
            }
        }

        public void Set(string scope, string settingName, string instanceKey, string value)
        {
            string query = _queryHelper.CheckSettingExistenceQuery(scope, settingName);
            SqlCommand cmd = new SqlCommand(query, _conn);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                throw new Exception("Invalid Set() call: Setting (including setting instance) does not exist.");
            }

            query = _queryHelper.CheckSettingInstanceExistenceQuery(scope, settingName, instanceKey);
            cmd = new SqlCommand(query, _conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                query = _queryHelper.UpdateSettingInstanceQuery((int) reader["SettingInstanceId"], value);
                SqlCommand cmdUpdate = new SqlCommand(query, _conn);
                cmdUpdate.ExecuteNonQuery();
            }
            else
            {
                query = _queryHelper.InsertNewSettingInstanceQuery((int)reader["SettingId"], instanceKey, value);
                SqlCommand cmdInsert = new SqlCommand(query, _conn);
                cmdInsert.ExecuteNonQuery();
            }
        }

        public bool Exists(string scope, string settingName)
        {
            var query = _queryHelper.CheckSettingExistenceQuery(scope, settingName);
            SqlCommand cmd = new SqlCommand(query, _conn);
            var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }

        public void Create(string scope, string settingName, string instanceKey, string initialValue)
        {
            throw new NotImplementedException();
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
        
        #endregion

    }
}
