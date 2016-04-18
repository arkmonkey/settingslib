
namespace settingslib.DataStore
{
    public interface ISettingsDataStoreServices
    {
        T Get<T>(string scope, string settingName, string instanceKey, T defaultIfNotExists) where T: struct;
        string GetString(string scope, string settingName, string instanceKey, string defaultIfNotExists = "");
        void Set(string scope, string settingName, string instanceKey, string value);
        bool Exists(string scope, string settingName);  //checks whether the setting (not setting instance) exists
        void Create(string scope, string settingName, string instanceKey, string initialValue);
    }
}