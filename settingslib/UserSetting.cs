namespace settingslib
{
    public class UserSetting
    {
        internal UserSetting()
        {
            
        }

        public int SettingId { get; set; }
        public string UserId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
