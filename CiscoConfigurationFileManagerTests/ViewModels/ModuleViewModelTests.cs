using CiscoConfigurationFileManager.Models;
using CiscoConfigurationFileManager.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CiscoConfigurationFileManagerTests.ViewModels;

[TestClass]
public class ModuleViewModelTests
{
    [TestMethod]
    public void TestInitialize()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();

        // Assert
        Assert.IsNotNull(viewModel.ExampleModules);
        Assert.IsNotNull(viewModel.GoToConnections);
        Assert.IsNotNull(viewModel.SaveChangesModule);
        Assert.IsNotNull(viewModel.SaveChangesModules);
        Assert.IsNotNull(viewModel.MergeConfig);
        Assert.IsNotNull(viewModel.RemoveModule);
        Assert.IsNotNull(viewModel.RemoveLine);
        Assert.IsNotNull(viewModel.AddLine);
        Assert.IsNotNull(viewModel.ExampleToRunning);
        Assert.IsNotNull(viewModel.ShowExample);
        Assert.IsNotNull(viewModel.ShowSelected);
        Assert.IsNotNull(viewModel.FetchInteractiveConfig);
        Assert.IsNotNull(viewModel.AddExampleModule);
        Assert.IsNotNull(viewModel.InteractiveModules);
        Assert.IsNotNull(viewModel.SelectedModule);
        Assert.IsNotNull(viewModel.SelectedExampleModule);
        Assert.IsNotNull(viewModel.SelectedExampleModuleToAdd);
        Assert.IsNotNull(viewModel.ExampleShowed);
        Assert.IsNotNull(viewModel.SelectedShowed);
        Assert.IsNotNull(viewModel.InteractiveConfig);
    }

    [TestMethod]
    public void TestDownloadConfig()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.DownloadConfig().Wait();

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.IsTrue(viewModel.Modules.Any());
    }

    [TestMethod]
    public void TestMergeConfiguration()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.DownloadConfig().Wait();
        var originalModules = viewModel.Modules.ToList();
        viewModel.Modules.Add(new RunningModule
        { Name = "NewModule", Configuration = new List<LineWrapper>(), Tag = "Other" });
        viewModel.UpdateModules();
        viewModel.MergeConfiguration();

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(originalModules.Count + 1, viewModel.Modules.Count);
        Assert.IsNotNull(viewModel.Modules.FirstOrDefault(m => m.Name == "NewModule"));
    }

    [TestMethod]
    public void TestDeleteSelectedModule()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.DownloadConfig().Wait();
        var name = viewModel.Modules.First().Name;
        viewModel.SelectedModule.Value = viewModel.Modules.First();
        viewModel.DeleteSelectedModule();

        var module = viewModel.Modules.FirstOrDefault(m => m.Name == name);

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.IsNotNull(viewModel.SelectedModule.Value);
        Assert.AreEqual(OperationEnum.Delete, module.OperationEnum);
    }

    [TestMethod]
    public void TestAddLine()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.DownloadConfig().Wait();
        viewModel.SelectedModule.Value = viewModel.Modules.First();
        viewModel.AddEmptyLine();
        viewModel.UpdateModules();

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(viewModel.SelectedModule.Value.Configuration.Count,
            viewModel.Modules.First().Configuration.Count);
    }

    [TestMethod]
    public void TestRemoveLine()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");
        var sut1 = 0;
        var sut2 = 1;

        // Act
        viewModel.Init();
        viewModel.InitializeAsync().Wait();
        var module = viewModel.Modules.First();
        sut1 = module.Configuration.Count - 1;
        viewModel.SelectedModule.Value = module;
        viewModel.FetchInteractiveConfiguration();
        viewModel.DeleteLineCommand(viewModel.SelectedModule.Value.Configuration.First());
        viewModel.DeleteLineCommand(viewModel.SelectedModule.Value.Configuration.First());
        viewModel.UpdateModule();
        sut2 = viewModel.SelectedModule.Value.Configuration.Count;

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(sut1, sut2);
    }

    [TestMethod]
    public void TestRemoveModule()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.InitializeAsync().Wait();
        var module = viewModel.Modules.First();
        viewModel.SelectedModule.Value = module;
        viewModel.DeleteSelectedModule();
        viewModel.UpdateModules();

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(1,
            viewModel.Modules.Count(q => q.OperationEnum == OperationEnum.Delete));
    }

    [TestMethod]
    public void TestAddExampleModule()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.InitializeAsync().Wait();
        var module = viewModel.Modules.First(q => q.Name == "Line");
        var count = module.Configuration.Count;
        viewModel.SelectedModule.Value = module;
        viewModel.SelectedExampleModuleToAdd.Value = new StringWrapper { String = "Line" };
        viewModel.AddExampleModuleToModules();

        // Assert
        var sut1 = viewModel.Modules.Find(q => q.Name == "Line").Configuration.Count();
        var sut2 = count * 2;

        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(sut2, sut1);
    }

    [TestMethod]
    public void TestUpdateModules()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");

        // Act
        viewModel.Init();
        viewModel.InitializeAsync().Wait();
        var module = viewModel.Modules.First();
        var count = module.Configuration.Count();
        viewModel.SelectedModule.Value = module;
        viewModel.FetchInteractiveConfiguration();
        viewModel.AddEmptyLine();
        viewModel.UpdateModule();
        viewModel.UpdateModules();

        // Assert
        var sut1 = count + 1;
        var sut2 = viewModel.Modules.Find(q => q.Name == module.Name).Configuration.Count();

        Assert.IsNotNull(viewModel.Modules);
        Assert.AreEqual(sut1, sut2);
    }

    [TestMethod]
    public void TestCreateNewModules()
    {
        // Arrange
        var viewModel = new ModuleViewModel("");
        viewModel.Init();
        viewModel.InitializeAsync().Wait();

        //Act
        viewModel.Modules?.Clear();
        viewModel.CreateNewModules();

        // Assert
        Assert.IsNotNull(viewModel.Modules);
        Assert.IsTrue(viewModel.Modules.Count > 0);
    }
}