using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using settingslib.DataStore;

namespace settingslib.test
{
    [TestClass]
    public class SqlServerBuilderTests
    {
        private SqlConnection _sqlConn;

        [TestInitialize]
        public void Initialize()
        {
            const string CONNECTION_STRING = @"data source=.\sqlexpress2014;initial catalog=SettingsLibUnitTestDb;user id=sa;password=asdf";
            _sqlConn = new SqlConnection(CONNECTION_STRING);
            _sqlConn.Open();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _sqlConn.Close();
            _sqlConn.Dispose();
            _sqlConn = null;
        }

        [TestMethod]
        public void TestTablesExists()
        {
            //create random prefix
            string prefix = RandomString(10);
            
            SqlServerDataStoreBuilder builder = new SqlServerDataStoreBuilder(prefix, _sqlConn);
            //call Validate() method
            var isValid = builder.IsDataStoreValid();

            //see that it's false
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void TestBuildTables()
        {
            //create random prefix
            string prefix = RandomString(10);

            SqlServerDataStoreBuilder builder = new SqlServerDataStoreBuilder(prefix, _sqlConn);
            //call Validate() method
            var isValid = builder.IsDataStoreValid();

            //see that it's false
            Assert.IsFalse(isValid);

            //build tables afterwards
            builder.BuildDataStore();

            //re-validate second time
            isValid = builder.IsDataStoreValid();
            //see that it's now true
            Assert.IsTrue(isValid);
            
            //test setting value to the parameter
            SqlServerDataStoreServices dataStoreServices = new SqlServerDataStoreServices(prefix, _sqlConn);
            string randomScope = RandomString(50);
            string randomSettingName = RandomString(50);
            var exists = dataStoreServices.Exists(randomScope, randomSettingName);
            //check that the setting exists
            Assert.IsFalse(exists);

            dataStoreServices.Create(randomScope, randomSettingName, "1", "foo");
            //retrieve the value
            var settingValue = dataStoreServices.GetString(randomScope, randomSettingName, "1", "notfoo");
            Assert.AreEqual("foo", settingValue);

        }

        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
