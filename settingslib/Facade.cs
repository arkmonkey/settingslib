using System;
using System.Data.SqlClient;

namespace settingslib
{
    public class Facade
    {
        private readonly DataSourceInterfacer _dbInterfacer;
        private readonly DbQueryHelper _queryHelper;

        public Facade(DataSourceInterfacer dbInterfacer)
        {
            _dbInterfacer = dbInterfacer;
            _queryHelper = new DbQueryHelper(_dbInterfacer.Prefix);
        }

        #region Vendor
        public bool CreateNewVendorSetting(string key)
        {
            string query = _queryHelper.CreateNewSettingQuery(true, false, false, key);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }   
        }

        public VendorSetting GetVendorSetting(string key)
        {
            string query = _queryHelper.GetSettingQuery(true, false, false, key, string.Empty);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);
            var reader = cmd.ExecuteReader();

            VendorSetting vs = new VendorSetting();
            if (reader.Read())
            {
                vs.SettingId = reader.GetInt32(0);
                vs.Key = key;
                vs.Value = reader.GetString(3);
                return vs;
            }

            //if code got here most likely cause: 
            //  no such setting
            return vs;
        }

        public bool SetVendorSetting(string key, string value)
        {
            string query = _queryHelper.SetSettingQuery(true, false, false, key, string.Empty, value);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        #endregion //Vendor

        #region Tenant
        public bool CreateNewTenantSetting(string key)
        {
            string query = _queryHelper.CreateNewSettingQuery(false, true, false, key);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TenantSetting GetTenantSetting(string tenantId, string key)
        {
            string query = _queryHelper.GetSettingQuery(false, true, false, key, tenantId);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);
            var reader = cmd.ExecuteReader();

            TenantSetting ts = new TenantSetting();
            if (reader.Read())
            {
                ts.SettingId = reader.GetInt32(0);
                ts.Key = key;
                ts.Value = reader.GetString(3);
                ts.TenantId = tenantId;
                return ts;
            }

            //if code got here most likely cause: 
            //  no such setting
            return ts;
        }

        public bool SetTenantSetting(string tenantId, string key, string value)
        {
            string query = _queryHelper.SetSettingQuery(false, true, false, key, tenantId, value);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion //Tenant

        #region User
        public bool CreateNewUserSetting(string key)
        {
            string query = _queryHelper.CreateNewSettingQuery(false, false, true, key);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public UserSetting GetUserSetting(string userId, string key)
        {
            string query = _queryHelper.GetSettingQuery(false, false, true, key, userId);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);
            var reader = cmd.ExecuteReader();

            UserSetting us = new UserSetting();
            if (reader.Read())
            {
                us.SettingId = reader.GetInt32(0);
                us.Key = key;
                us.Value = reader.GetString(3);
                us.UserId = userId;
                return us;
            }

            //if code got here most likely cause: 
            //  no such setting
            return us;
        }

        public bool SetUserSetting(string userId, string key, string value)
        {
            string query = _queryHelper.SetSettingQuery(false, false, true, key, userId, value);
            SqlCommand cmd = new SqlCommand(query, _dbInterfacer.Connection);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion //User

    }
}
