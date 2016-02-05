namespace settingslib
{
    public class TenantSetting
    {
        internal TenantSetting()
        {
            
        }

        public int SettingId { get; set; }
        public string TenantId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
