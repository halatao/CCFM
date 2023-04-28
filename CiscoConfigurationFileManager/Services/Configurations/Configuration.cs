using CiscoConfigurationFileManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CiscoConfigurationFileManager.Services.Configurations;

public class Configuration
{
    public List<string>? ConfigurationToUpload { get; set; }
    public List<RunningModule> RunningModules;
    public List<ExampleModule> ExampleModules;
    public List<StringWrapper> ExampleModulesNames;

    /// <summary>
    /// Constructor for Configuration class.
    /// </summary>
    /// <param name="configuration">List of configuration strings.</param>
    public Configuration(List<string> configuration)
    {
        ConfigurationToUpload = new List<string>();

        var exampleConfiguration = new List<string>(File.ReadAllLines(Paths.Example));
        var confRun = SplitConfiguration(configuration);
        var confExp = SplitConfiguration(exampleConfiguration);

        var runningConfigurationByCategory = ConfigurationToDictionary(confRun);
        var exampleConfigurationByCategory = ConfigurationToDictionary(confExp);

        ExampleModules = GetModules(exampleConfigurationByCategory)
            .Select(q => new ExampleModule
            {
                Name = q.Name,
                OperationEnum = q.OperationEnum,
                Configuration = (q.Configuration ?? new List<LineWrapper>())
                    .Select(q => new StringWrapperRead(q.Line.Value)).ToList(),
                Tag = q.Tag
            })
            .ToList();
        RunningModules = GetModules(runningConfigurationByCategory);

        ExampleModulesNames = ExampleModules.Select(q => new StringWrapper {String = q.Tag}).DistinctBy(q => q.String)
            .OrderBy(q => q.String)
            .ToList();
    }

    /// <summary>
    /// Splits a list of configuration strings into a list of lists of configuration strings, where each sublist represents a separate category.
    /// </summary>
    /// <param name="configuration">List of configuration strings.</param>
    /// <returns>List of lists of configuration strings.</returns>
    public List<List<string>> SplitConfiguration(List<string> configuration)
    {
        var configurationList = new List<List<string>>();
        var tempList = new List<string>();
        foreach (var line in configuration)
        {
            if (line == "!")
            {
                configurationList.Add(tempList);
                tempList = new List<string>();
            }
            else
            {
                tempList.Add(line);
            }
        }

        return configurationList.Where(q => q.Count > 0).ToList();
    }

    /// <summary>
    /// Converts a list of lists of configuration strings into a dictionary, where the keys are the category names and the values are the lists of configuration strings for each category.
    /// </summary>
    /// <param name="configuration">List of lists of configuration strings.</param>
    /// <returns>Dictionary of category names and lists of configuration strings.</returns>
    public Dictionary<string, List<string>> ConfigurationToDictionary(List<List<string>> configuration)
    {
        var commandDictionary = new Dictionary<string, List<string>>();
        foreach (var list in configuration)
        {
            var trimmedLine = list[0].Trim();
            var category = Categories.GetCommandCategory(trimmedLine);

            if (trimmedLine.Contains("interface"))
            {
                commandDictionary.Add(trimmedLine, list);
            }
            else if (commandDictionary.ContainsKey(category))
            {
                commandDictionary[category].AddRange(list);
            }
            else
            {
                commandDictionary.Add(category, list);
            }
        }

        return commandDictionary;
    }

    /// <summary>
    /// Gets a list of RunningModules based on a given configuration dictionary.
    /// </summary>
    /// <param name="config">Dictionary containing configuration data.</param>
    /// <returns>A list of RunningModules.</returns>
    public List<RunningModule> GetModules(Dictionary<string, List<string>> config)
    {
        var modules = new List<RunningModule>();

        foreach (var (key, value) in config)
        {
            var module = new RunningModule()
            {
                Name = key,
                Tag = key,
                Configuration =
                    value.Select(q => new LineWrapper {Line = {Value = q}})
                        .ToList()
            };

            if (key.Contains("interface") && !key.ToLower().Contains("vlan"))
            {
                module.Tag = "Port interface";
                module.Name = "Port interface " + ConvertInterfaceName(key);
            }
            else if (key.ToLower().Contains("vlan"))
            {
                module.Name = "Vlan interface";
                module.Tag = "Vlan interface";
            }
            else if (key.ToLower().Contains("ignore"))
                continue;

            modules.Add(module);
        }

        return modules;
    }

    /// <summary>
    /// Updates the configuration to be uploaded based on the current RunningModules.
    /// </summary>
    public void UpdateConfiguration()
    {
        var configurationToUpload = new List<string>();

        foreach (var mod in RunningModules)
        {
            if (mod.Configuration == null) continue;

            configurationToUpload.Add("!");
            foreach (var line in mod.Configuration)
            {
                if (line.OperationEnum == OperationEnum.Delete)
                {
                    if (!line.Original)
                        continue;
                }

                configurationToUpload.Add(line.Line.Value);
            }
        }

        configurationToUpload.Add("!");

        ConfigurationToUpload = configurationToUpload;
    }

    /// <summary>
    /// Updates the list of RunningModules and updates the configuration to be uploaded based on the updated list.
    /// </summary>
    /// <param name="modules">The updated list of RunningModules.</param>
    public void UpdateModules(List<RunningModule> modules)
    {
        RunningModules = modules;
        UpdateConfiguration();
    }

    /// <summary>
    /// Converts an input string into an interface name.
    /// </summary>
    /// <param name="inputString">The input string to be converted.</param>
    /// <returns>The converted interface name.</returns>
    public string ConvertInterfaceName(string inputString)
    {
        if (inputString.ToLower().Contains("example"))
            return "";
        var interfaceName = inputString.Split(' ').Last().First().ToString();
        var numbers = new List<int>();
        const string pattern = @"\d+";
        var matches = Regex.Matches(inputString, pattern);

        foreach (Match match in matches)
        {
            if (int.TryParse(match.Value, out var number))
            {
                interfaceName += match + "/";
            }
        }

        interfaceName = interfaceName.Remove(interfaceName.Length - 1);

        return interfaceName;
    }

    /// <summary>
    /// Reads a configuration file and creates a list of RunningModules based on the configuration data.
    /// </summary>
    /// <param name="path">The path to the configuration file to be read.</param>
    public void OpenConfiguration(string path)
    {
        var config = new List<string>(File.ReadAllLines(path));
        var confRun = SplitConfiguration(config);
        var confDict = ConfigurationToDictionary(confRun);
        RunningModules = GetModules(confDict);
    }

    /// <summary>
    /// Reads the default configuration file and creates a list of RunningModules based on the configuration data.
    /// </summary>
    public void NewConfiguration()
    {
        OpenConfiguration(Paths.Clean);
    }
}