using Microsoft.VisualStudio.TestTools.UnitTesting;
using Echo.Model;
using Echo.Logic;
using System.Linq;

namespace Echo.test
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestUDCListModel()
        {
            UDCListModel udc = new UDCListModel("Data Source=isostore:/Test1.sdf", true);
            UserModel testUser = new UserModel("1", "foo", "bar", "");
            udc.addUser(testUser);

            Assert.Equals(testUser, udc.UsersByFirstName[0]);
            Assert.IsTrue(udc.SubmitChanges());

            udc.LoadListsFromDatabase();

            Assert.Equals(testUser, udc.UsersByFirstName[0]);

            GroupModel testGroup = new GroupModel("TestGroup");
            udc.addGroup(testGroup);
            udc.addToGroup(testUser, testGroup);

            Assert.IsTrue(udc.GroupList[0].Users.Contains(testUser));

            CallLogEntry testEntry = new CallLogEntry();
            CallLogModel testLog = new CallLogModel(1);
            testLog.addEntry(testEntry);
            testUser.CallLogs.Add(testLog);

            Assert.IsTrue(udc.SubmitChanges());
            Assert.IsTrue(testUser.CallLogs.Any());
            Assert.Equals(testUser.CallLogs.First(), testLog);

            udc.removeGroup("TestGroup");
            Assert.IsTrue(udc.GroupList.Count == 0);

        }

        [TestMethod]
        public void TestSettingsModel()
        {
            SettingsModel setModel = new SettingsModel(true);
            Assert.IsFalse(setModel.NetworkSettingsAreChanged);
            setModel.AddOrUpdateValue(setModel.EchoPortSettingKeyName, 666);
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
            setModel = new SettingsModel(true);
            setModel.AddOrUpdateValue(setModel.EchoServerSettingKeyName, "foo");
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
            setModel = new SettingsModel(true);
            setModel.AddOrUpdateValue(setModel.ServerSettingKeyName, "bar");
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
            setModel = new SettingsModel(true);
            setModel.AddOrUpdateValue(setModel.PortSettingKeyName, 666);
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
            setModel = new SettingsModel(true);
            setModel.AddOrUpdateValue(setModel.UsernameSettingKeyName, "foo");
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
            setModel = new SettingsModel(true);
            setModel.AddOrUpdateValue(setModel.PasswordSettingKeyName, "bar");
            Assert.IsTrue(setModel.NetworkSettingsAreChanged);
        }
    }
}