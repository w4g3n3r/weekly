using Weekly.Serializers;

namespace Weekly.Infrastructure
{
    public class FileManager
    {
        public const string WorkLogExtension = ".wk";
        public const string TemplateExtension = ".template.wk";

        private readonly IEnumerable<IWorkLogSerializer> _serializers;

        public FileManager(IEnumerable<IWorkLogSerializer> serializers)
        {
            _serializers = serializers ?? throw new ArgumentNullException(nameof(serializers));
        }

        public async Task<WorkLog> OpenWorkLogAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var fileContents = await File.ReadAllTextAsync(filePath);

            var serializer = _serializers
                .Where(s => s.CanDeserialize(fileContents))
                .FirstOrDefault();

            if (serializer == null)
                throw new InvalidOperationException("Could not find a serializer for the file.");

            try
            {
                var workLog = serializer.Deserialize(fileContents);
                workLog.FilePath = filePath;

                return workLog;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not deserialize the file.", ex);
            }
        }

        public async Task SaveWorkLogAsync(WorkLog workLog)
        {
            if (workLog == null)
                throw new ArgumentNullException(nameof(workLog));

            if (string.IsNullOrEmpty(workLog.FilePath))
                throw new InvalidOperationException("Work log file path is not set.");

            var serializer = _serializers
                .Where(s => s.Version == workLog.Version)
                .FirstOrDefault();

            if (serializer == null)
                throw new InvalidOperationException("Could not find a serializer for the work log.");

            var serializedWorkLog = serializer.Serialize(workLog);

            await File.WriteAllTextAsync(workLog.FilePath, serializedWorkLog);
        }

        public async Task<string> GetCurrentWorkLogFilePathAsync(string workLogDirectory)
        {
            if (string.IsNullOrEmpty(workLogDirectory))
                throw new ArgumentNullException(nameof(workLogDirectory));

            if (!Directory.Exists(workLogDirectory))
                throw new DirectoryNotFoundException("Work log directory not found.");

            await foreach (var workLog in GetWorkLogsAsync(workLogDirectory))
            {
                if (workLog.Period.IsCurrent)
                {
                    return workLog.FilePath;
                }
            }

            return null;
        }

        public string GetWorkLogFilePath(string workLogDirectory, string workLogName)
        {
            if (string.IsNullOrEmpty(workLogDirectory))
                throw new ArgumentNullException(nameof(workLogDirectory));

            if (string.IsNullOrEmpty(workLogName))
                throw new ArgumentNullException(nameof(workLogName));

            if (!Directory.Exists(workLogDirectory))
                throw new DirectoryNotFoundException("Work log directory not found.");

            if (workLogName.EndsWith(WorkLogExtension))
            {
                return Path.Combine(workLogDirectory, workLogName);
            }
            else
            {
                return Path.Combine(workLogDirectory, $"{workLogName}{WorkLogExtension}");
            }
        }

        public string GetTemplateFilePath(string templateDirectory, string templateName)
        {
            if (string.IsNullOrEmpty(templateDirectory))
                throw new ArgumentNullException(nameof(templateDirectory));

            if (string.IsNullOrEmpty(templateName))
                throw new ArgumentNullException(nameof(templateName));

            if (!Directory.Exists(templateDirectory))
                throw new DirectoryNotFoundException("Template directory not found.");

            if (templateName.EndsWith(TemplateExtension))
            {
                return Path.Combine(templateDirectory, templateName);
            }
            else
            {
                return Path.Combine(templateDirectory, $"{templateName}{TemplateExtension}");
            }
        }

        public async IAsyncEnumerable<WorkLog> GetWorkLogsAsync(string workLogDirectory)
        {
            if (string.IsNullOrEmpty(workLogDirectory))
                throw new ArgumentNullException(nameof(workLogDirectory));

            if (!Directory.Exists(workLogDirectory))
                throw new DirectoryNotFoundException("Work log directory not found.");

            var workLogs = new List<WorkLog>();
            var files = Directory.GetFiles(workLogDirectory, $"*{WorkLogExtension}");

            foreach (var file in files)
            {
                var workLog = await OpenWorkLogAsync(file);

                if (workLog != null)
                    yield return workLog;
            }
        }
    }
}
