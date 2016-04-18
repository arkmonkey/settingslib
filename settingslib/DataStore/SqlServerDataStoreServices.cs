using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace settingslib.DataStore
{
    public class SqlServerDataStoreServices : ISettingsDataStoreServices
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
            string sentinelString = "##SENTINEL##";
            string result = GetString(scope, settingName, instanceKey, sentinelString);

            if (sentinelString.Equals(result))
            {
                return defaultIfNotExists;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            var converted = typeConverter.ConvertFromString(result);
            if (converted != null)
            {
                return (T)converted;
            }
            else
            {
                return defaultIfNotExists;
            }
        }

        public string GetString(string scope, string settingName, string instanceKey, string defaultIfNotExists = "")
        {
            var query = QueryHelper.GetSettingQuery(scope, settingName, instanceKey);
            SqlCommand cmd = new SqlCommand(query, _conn);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                try
                {
                    reader.Read();
                    return reader["SettingValue"].ToString();
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
            string query = QueryHelper.CheckSettingExistenceQuery(scope, settingName);
            SqlCommand cmd = new SqlCommand(query, _conn);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                throw new Exception("Invalid Set() call: Setting (including setting instance) does not exist.");
            }

            query = QueryHelper.CheckSettingInstanceExistenceQuery(scope, settingName, instanceKey);
            cmd = new SqlCommand(query, _conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                query = QueryHelper.UpdateSettingInstanceQuery((int) reader["SettingInstanceId"], value);
                SqlCommand cmdUpdate = new SqlCommand(query, _conn);
                cmdUpdate.ExecuteNonQuery();
            }
            else
            {
                query = QueryHelper.InsertNewSettingInstanceQuery((int)reader["SettingId"], instanceKey, value);
                SqlCommand cmdInsert = new SqlCommand(query, _conn);
                cmdInsert.ExecuteNonQuery();
            }
        }

        public bool Exists(string scope, string settingName)
        {
            var query = QueryHelper.CheckSettingExistenceQuery(scope, settingName);
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
