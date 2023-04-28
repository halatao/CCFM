using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CiscoConfigurationFileManager.Services.Configurations
{
    /// <summary>
    /// Provides functionality for retrieving the category of a command based on predefined keywords and categories stored in a JSON file.
    /// </summary>
    public static class Categories
    {
        /// <summary>
        /// A lazy-loaded dictionary that contains the category keywords for commands. The dictionary is loaded from a JSON file located at <see cref="Paths.Categories"/>. If the file fails to load, an exception message is displayed using <see cref="CreateException.ShowExceptionMessage"/>.
        /// </summary>
        private static readonly Lazy<Dictionary<string, List<string>>?> CategoryKeywords = new(() =>
        {
            var categoryKeywords = new Dictionary<string, List<string>>();

            try
            {
                var json = File.ReadAllText(Paths.Categories);
                categoryKeywords = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            }
            catch (Exception ex)
            {
                CreateException.ShowExceptionMessage(ex, "Failed to load categories configuration: ", true);
            }

            return categoryKeywords;
        });

        /// <summary>
        /// Gets the category of a command based on predefined keywords and categories.
        /// </summary>
        /// <param name="command">The command to retrieve the category for.</param>
        /// <returns>The category of the command.</returns>
        public static string GetCommandCategory(string command)
        {
            if (CategoryKeywords.Value == null || string.IsNullOrEmpty(command) || command == " ") return "Ignore";
            foreach (var (key, value) in CategoryKeywords.Value)
            {
                if (value.Any(command.Contains))
                {
                    return key;
                }
            }

            return "Other";
        }
    }
}