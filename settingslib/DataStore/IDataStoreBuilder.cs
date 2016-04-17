namespace settingslib.DataStore
{
    public interface IDataStoreBuilder
    {
        bool IsDataStoreValid();    //checks whether the data store is good to use or not
        void BuildDataStore();      //builds it out 
    }
}