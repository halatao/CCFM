using CiscoConfigurationFileManager;
using CiscoConfigurationFileManager.Models;
using CiscoConfigurationFileManager.Services.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CiscoConfigurationFileManagerTests.Services.Configurations
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void TestConfiguration()
        {
            // Arrange
            var configuration = new List<string>
            {
                "!",
                "hostname R1",
                "!",
                "interface GigabitEthernet0/0",
                " ip address"
            };

            // Act
            var configurationObject = new Configuration(configuration);

            // Assert
            Assert.IsNotNull(configurationObject);
            Assert.IsNotNull(configurationObject.ExampleModules);
            Assert.IsNotNull(configurationObject.RunningModules);
            Assert.IsNotNull(configurationObject.ExampleModulesNames);
        }

        [TestMethod]
        public void TestSplitConfiguration()
        {
            // Arrange
            var configuration = new List<string>
            {
                "!",
                "hostname R1",
                "!",
                "interface GigabitEthernet0/0",
                " ip address",
                "!",
            };

            // Act
            var configurationObject = new Configuration(configuration);
            var splitConfiguration = configurationObject.SplitConfiguration(configuration);

            // Assert
            Assert.IsNotNull(splitConfiguration);
            Assert.IsTrue(splitConfiguration.Count == 2);
        }

        [TestMethod]
        public void TestSplitConfigurationWithEmptyLines()
        {
            // Arrange
            var configuration = new List<string>
            {
                "!",
                "hostname R1",
                "!",
                "interface GigabitEthernet0/0",
                " ip address",
                "!",
                "!",
                "!",
                "!",
                "!",
            };

            var configurationObject = new Configuration(configuration);

            // Act
            var splitConfiguration = configurationObject.SplitConfiguration(configuration);

            // Assert
            Assert.IsNotNull(splitConfiguration);
            Assert.IsTrue(splitConfiguration.Count == 2);
        }

        [TestMethod]
        public void TestGetModules()
        {
            // Arrange
            var configuration = new List<string>
            {
                "!",
                "hostname R1",
                "!",
                "interface GigabitEthernet0/0",
                " ip address",
                "!",
                "!",
                "!",
                "!",
                "!",
            };
            var configurationObject = new Configuration(configuration);
            var split = configurationObject.SplitConfiguration(configuration);
            var dict = configurationObject.ConfigurationToDictionary(split);

            // Act
            var modules = configurationObject.GetModules(dict);

            // Assert
            Assert.IsNotNull(modules);
            Assert.IsTrue(modules.Count == 2);
        }

        [TestMethod]
        public void TestUpdateConfiguration()
        {
            // Arrange
            var configuration = new List<string>
            {
                "!",
                "hostname R1",
                "!",
                "interface GigabitEthernet0/0",
                " ip address",
                "!",
                "!",
                "!",
                "!",
                "!",
            };
            var configurationObject = new Configuration(configuration);
            var split = configurationObject.SplitConfiguration(configuration);
            var dict = configurationObject.ConfigurationToDictionary(split);
            var modules = configurationObject.GetModules(dict);

            // Act
            modules.First().Configuration.First().Operate(OperationEnum.Delete);
            configurationObject.UpdateConfiguration();

            // Assert
            Assert.IsNotNull(configurationObject.ConfigurationToUpload);
            Assert.IsTrue(modules.First().Configuration.Any(q => q.OperationEnum == OperationEnum.Delete));
        }

        [TestMethod]
        public void TestConvertInterfaceName()
        {
            // Arrange
            var configurationObject = new Configuration(new List<string>());
            var g1 = "GigabitEthernet0/1";
            var g2 = "GigabitEthernet0/12";
            var g3 = "GigabitEthernet1/2";

            // Act
            var conv1 = configurationObject.ConvertInterfaceName(g1);
            var conv2 = configurationObject.ConvertInterfaceName(g2);
            var conv3 = configurationObject.ConvertInterfaceName(g3);

            // Assert
            Assert.IsNotNull(conv1);
            Assert.IsNotNull(conv2);
            Assert.IsNotNull(conv3);
            Assert.AreEqual("G0/1", conv1);
            Assert.AreEqual("G0/12", conv2);
            Assert.AreEqual("G1/2", conv3);
        }

        [TestMethod]
        public void TestOpenConfiguration()
        {
            // Arrange
            var configurationObject = new Configuration(new List<string>());

            //Act
            configurationObject.OpenConfiguration(Paths.Clean);

            // Assert
            Assert.IsNotNull(configurationObject);
            Assert.IsNotNull(configurationObject.ExampleModules);
            Assert.IsNotNull(configurationObject.RunningModules);
            Assert.IsNotNull(configurationObject.ExampleModulesNames);
            Assert.IsTrue(configurationObject.RunningModules.Count > 0);
        }
    }
}