using System.Text.Json;
using System.Text.Json.Serialization;

namespace Weekly
{
    [JsonSerializable(typeof(Configuration))]
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNameCaseInsensitive = true)]
    internal partial class ConfigurationJsonContext : JsonSerializerContext
    {
    }

    public sealed class Configuration
    {
        private const string AppName = "Weekly";
        private const string ConfigFileName = "config.json";

        /// <summary>
        /// The day of the week on which the week begins. The value should be one of the following:
        /// u - Sunday
        /// m - Monday
        /// t - Tuesday
        /// w - Wednesday
        /// r - Thursday
        /// f - Friday
        /// s - Saturday 
        /// </summary>
        public string WeekBeginsOn { get; set; }

        /// <summary>
        /// The number of hours in a work day. The default is 8.
        /// </summary>
        public int HoursPerDay { get; set; }

        /// <summary>
        /// The number of days to include in each file. The default is 5.
        /// </summary>
        public int DaysPerFile { get; set; }

        /// <summary>
        /// The minimum dynamic value. The default is .25.
        /// </summary>
        public double MinimumHourValue { get; set; }

        /// <summary>
        /// The directory where the user will store thier work logs
        /// </summary>
        public string? WorkLogDirectory { get; set; }

        /// <summary>
        /// The naem of the directory in the WorkLogDirectory where the
        /// templates will be stored.
        /// </summary>
        public string TemplateDirectoryName { get; set; }

        /// <summary>
        /// The name of the default template.
        /// </summary>
        public string DefaultTemplateName { get; set; }

        /// <summary>
        /// If true, the inner exception of the thrown ProgramException will be written
        /// to the console.
        /// </summary>
        public bool? LogExceptionsToConsole { get; set; }

        /// <summary>
        /// If true the help message on the ProgramException will be written to the
        /// console.
        /// </summary>
        public bool? ShowHelpMessages { get; set; }

        public static Configuration Default
        {
            get
            {
                return new Configuration
                {
                    WeekBeginsOn = "m",
                    HoursPerDay = 8,
                    DaysPerFile = 5,
                    MinimumHourValue = .25,
                    TemplateDirectoryName = "Templates",
                    DefaultTemplateName = "default",
                    LogExceptionsToConsole = false,
                    ShowHelpMessages = true
                };
            }
        }

        public static string Path
        {
            get
            {
                string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                string configDirectoryPath = System.IO.Path.Combine(appDataPath, AppName);
                string configFilePath = System.IO.Path.Combine(configDirectoryPath, ConfigFileName);

                return configFilePath;
            }
        }

        public bool TrySave()
        {
            try
            {
                var directory = System.IO.Path.GetDirectoryName(Configuration.Path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(this, ConfigurationJsonContext.Default.Configuration);
                File.WriteAllText(Configuration.Path, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsInitialized()
        {
            return !string.IsNullOrEmpty(WorkLogDirectory) && Directory.Exists(WorkLogDirectory);
        }

        public static Configuration Load()
        {
            if (!File.Exists(Path))
            {
                return Default;
            }

            var json = File.ReadAllText(Path);
            var deserializedConfig = JsonSerializer
                .Deserialize(json, ConfigurationJsonContext.Default.Configuration);

            return MergeWithDefault(deserializedConfig);
        }

        public static Configuration Create()
        {
            var config = Configuration.Default;
            _ = config.TrySave();
            return config;
        }

        public static bool FileExists()
        {
            return File.Exists(Path);
        }

        private static Configuration MergeWithDefault(Configuration deserializedConfig)
        {
            var defaultConfig = Default;

            return new Configuration
            {
                WeekBeginsOn = deserializedConfig.WeekBeginsOn ?? defaultConfig.WeekBeginsOn,
                HoursPerDay = deserializedConfig.HoursPerDay != 0 ? deserializedConfig.HoursPerDay : defaultConfig.HoursPerDay,
                DaysPerFile = deserializedConfig.DaysPerFile != 0 ? deserializedConfig.DaysPerFile : defaultConfig.DaysPerFile,
                MinimumHourValue = deserializedConfig.MinimumHourValue != 0 ? deserializedConfig.MinimumHourValue : defaultConfig.MinimumHourValue,
                WorkLogDirectory = deserializedConfig.WorkLogDirectory ?? defaultConfig.WorkLogDirectory,
                TemplateDirectoryName = deserializedConfig.TemplateDirectoryName ?? defaultConfig.TemplateDirectoryName,
                DefaultTemplateName = deserializedConfig.DefaultTemplateName ?? defaultConfig.DefaultTemplateName,
                LogExceptionsToConsole = deserializedConfig.LogExceptionsToConsole ?? defaultConfig.LogExceptionsToConsole,
                ShowHelpMessages = deserializedConfig.ShowHelpMessages ?? defaultConfig.ShowHelpMessages
            };
        }
    }
}
