

namespace settingslib
{
    /// <summary>
    /// This represents the scope of a settings.
    /// Scope can be anything defined by the dev
    /// that uses this library.  But commonly, it can 
    /// be "MyApp" or "MyApp.Tenant1", or "MyApp.User", etc
    /// </summary>
    public class SettingsScope
    {
        public SettingsScope(string scopeName)
        {
            ScopeName = scopeName;
        }

        public SettingsScope()
        {
            
        }

        public string ScopeName { get; set; }
    }
}
