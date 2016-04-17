
using settingslib.DataStore;

namespace settingslib
{
    public class SettingsHelper
    {
        private readonly ISettingsDataStoreServices _dataStoreServices;

        public SettingsHelper(ISettingsDataStoreServices dataStoreServices, 
            string scope, 
            string instanceKey) : this(dataStoreServices)
        {
            ScopeName = scope;
            InstanceKey = instanceKey;
        }

        public SettingsHelper(ISettingsDataStoreServices dataStoreServices)
        {
            _dataStoreServices = dataStoreServices;
        }

        public string ScopeName { get; set; }
        public string InstanceKey { get; set; }

        public T Get<T>(string settingName, T defaultValue) where T : struct 
        {
            return _dataStoreServices.Get<T>(ScopeName, settingName, InstanceKey, defaultValue);
        }

        public void Set(string settingName, string value)
        {
            if (!_dataStoreServices.Exists(ScopeName, settingName))
            {
                _dataStoreServices.Create(ScopeName, settingName, InstanceKey, value);
            }
            else
            {
                _dataStoreServices.Set(ScopeName, settingName, InstanceKey, value);
            }
        }
    }
}
