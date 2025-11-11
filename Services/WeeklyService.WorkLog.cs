using System.Text;
using Weekly.Exceptions;

namespace Weekly.Services
{
    public partial class WeeklyService
    {
        public string WorkLogDirectory => _configuration.WorkLogDirectory ?? string.Empty;
        public string TemplateDirectory => Path.Combine(WorkLogDirectory, _configuration.TemplateDirectoryName ?? string.Empty);

        /// <summary>
        /// Gets the work log with the specified workLogName, or gets the work log for the current period.
        /// </summary>
        /// <param name="workLogName">The name of the work log to get, or the current one if null.</param>
        /// <returns></returns>
        /// <exception cref="WorkLogNotFoundException"></exception>
        /// <exception cref="TemplateNotFoundException"></exception>
        /// <exception cref="WorkLogParseException"></exception>
        /// <exception cref="MissingWorkLogDirectoryException"></exception>
        /// <exception cref="InternalException"></exception>
        public async Task<WorkLog> GetWorkLogAsync(string? workLogName)
        {
            try
            {
                if (!string.IsNullOrEmpty(workLogName))
                {
                    return await GetWorkLogByNameAsync(workLogName);
                }
                else
                {
                    return await GetWorkLogByCurrentPeriodAsync();
                }
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Looks for the current worklog, or worklog with the specified work log name. If
        /// not found, will create a new worklog for the current period using the template
        /// provided. If no template is provided, the default template will be used.
        /// </summary>
        /// <param name="workLogName">The name of the work log to get, or the current one if null.</param>
        /// <param name="templateName">The template to use when creating a new work log, or the default template if null.</param>
        /// <returns></returns>
        /// <exception cref="WorkLogNotFoundException"></exception>
        /// <exception cref="TemplateNotFoundException"></exception>
        /// <exception cref="WorkLogParseException"></exception>
        /// <exception cref="MissingWorkLogDirectoryException"></exception>
        /// <exception cref="MissingTemplateDirectoryException"></exception>"
        /// <exception cref="InternalException"></exception>
        public async Task<WorkLog> GetOrCreateWorkLogAsync(string workLogName, string templateName)
        {

            try
            {

                if (!string.IsNullOrEmpty(workLogName))
                {
                    return await GetWorkLogByNameAsync(workLogName);
                }
                else
                {
                    return await GetOrCreateWorkLogForCurrentPeriodAsync(templateName);
                }
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Create a new work log.
        /// </summary>
        /// <param name="workLogName"></param>
        /// <param name="period"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        /// <exception cref="InternalException"></exception>
        public async Task<WorkLog> CreateWorkLogAsync(string workLogName, WorkLogPeriod period, string templateName)
        {
            WorkLog newWorkLog = await GetTemplateByNameAsync(templateName);

            try
            {
                newWorkLog.FilePath = _fileManager.GetWorkLogFilePath(WorkLogDirectory, workLogName);
                newWorkLog.ApplyPeriod(period, _configuration.HoursPerDay);

                await _fileManager.SaveWorkLogAsync(newWorkLog);

                return await _fileManager.OpenWorkLogAsync(newWorkLog.FilePath);
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Saves the currnet work log to the work log directory.
        /// </summary>
        /// <param name="workLog">The work log to save.</param>
        /// <returns></returns>
        /// <exception cref="InternalException"></exception>
        public async Task SaveWorkLogAsync(WorkLog workLog)
        {
            try
            {
                await _fileManager.SaveWorkLogAsync(workLog);
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Checks if the work log exists in the work log directory.
        /// </summary>
        /// <param name="workLogName"></param>
        /// <returns></returns>
        /// <exception cref="MissingWorkLogDirectoryException"></exception>
        /// <exception cref="InternalException"></exception>
        public bool WorkLogExists(string workLogName)
        {
            try
            {
                if (string.IsNullOrEmpty(_configuration.WorkLogDirectory))
                    throw new MissingWorkLogDirectoryException();

                var workLogPath = _fileManager.GetWorkLogFilePath(_configuration.WorkLogDirectory, workLogName);

                return File.Exists(workLogPath);
            }
            catch (DirectoryNotFoundException dex)
            {
                throw new MissingWorkLogDirectoryException(dex);
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Deletes an existing work log file.
        /// </summary>
        /// <param name="workLogName"></param>
        /// <exception cref="MissingWorkLogDirectoryException"></exception>
        /// <exception cref="InternalException"></exception>
        public void DeleteWorkLog(string workLogName)
        {
            try
            {
                if (string.IsNullOrEmpty(_configuration.WorkLogDirectory))
                    throw new MissingWorkLogDirectoryException();

                var workLogPath = _fileManager.GetWorkLogFilePath(_configuration.WorkLogDirectory, workLogName);

                File.Delete(workLogPath);
            }
            catch (DirectoryNotFoundException dex)
            {
                throw new MissingWorkLogDirectoryException(dex);
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        /// <summary>
        /// Initializes the work log directory.
        /// </summary>
        /// <param name="directory">The directory to set as the work log directory.</param>
        /// <exception cref="InitializationException"></exception>
        /// <exception cref="InternalException"></exception>
        public void InitializeWorkLogDirectory(string directory)
        {

            if (string.IsNullOrEmpty(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            try
            {
                if (Directory.Exists(directory))
                {
                    _configuration.WorkLogDirectory = directory;
                    var templateDirectory = Path.Combine(directory, _configuration.TemplateDirectoryName);

                    if (!Directory.Exists(templateDirectory))
                    {
                        Directory.CreateDirectory(templateDirectory);
                    }

                    var defaultTemplatePath = _fileManager.GetTemplateFilePath(templateDirectory, "default");
                    if (!File.Exists(defaultTemplatePath))
                    {
                        using (var sw = new StreamWriter(defaultTemplatePath))
                        {
                            var fileContents = Encoding.UTF8.GetString(Properties.Resources.Default_Template);
                            sw.Write(fileContents);
                        }
                    }

                    if (!_configuration.TrySave())
                    {
                        throw new InitializationException(directory);
                    }
                }
                else
                {
                    throw new InitializationException(directory);
                }
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        private async Task<WorkLog> GetOrCreateWorkLogForCurrentPeriodAsync(string templateName)
        {
            WorkLog? workLog = null;
            WorkLog? template = null;
            try
            {
                workLog = await GetWorkLogByCurrentPeriodAsync();
                return workLog;
            }
            catch (WorkLogNotFoundException)
            {
                workLog = null;
            }

            var currentPeriod = WorkLogPeriod
                .GetCurrentPeriod(_configuration.WeekBeginsOn[0], _configuration.DaysPerFile);

            if (string.IsNullOrEmpty(templateName))
                templateName = _configuration.DefaultTemplateName;

            return await CreateWorkLogAsync(currentPeriod.StartDate.ToString("yyyyMMdd"), currentPeriod, templateName);

        }


        private async Task<WorkLog> GetTemplateByNameAsync(string templateName)
        {
            try
            {
                string templatePath = _fileManager
                    .GetTemplateFilePath(TemplateDirectory, templateName);

                var template = await _fileManager.OpenWorkLogAsync(templatePath);

                return template;
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new MissingTemplateDirectoryException(ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new WorkLogParseException(ex);
            }
            catch (Exception ex) when (ex is ArgumentNullException or FileNotFoundException)
            {
                throw new TemplateNotFoundException(ex);
            }
        }

        private async Task<WorkLog> GetWorkLogByNameAsync(string workLogName)
        {
            try
            {
                string workLogPath = _fileManager
                    .GetWorkLogFilePath(WorkLogDirectory, workLogName);

                var workLog = await _fileManager.OpenWorkLogAsync(workLogPath);

                return workLog;
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new MissingWorkLogDirectoryException(ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new WorkLogParseException(ex);
            }
            catch (Exception ex) when (ex is ArgumentNullException or FileNotFoundException)
            {
                throw new WorkLogNotFoundException(ex);
            }
        }

        private async Task<WorkLog> GetWorkLogByCurrentPeriodAsync()
        {
            try
            {
                var workLogPath = await _fileManager
                    .GetCurrentWorkLogFilePathAsync(WorkLogDirectory);

                var workLog = await _fileManager.OpenWorkLogAsync(workLogPath);

                return workLog;

            }
            catch (DirectoryNotFoundException ex)
            {
                throw new MissingWorkLogDirectoryException(ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new WorkLogParseException(ex);
            }
            catch (Exception ex) when (ex is ArgumentNullException or FileNotFoundException)
            {
                throw new WorkLogNotFoundException(ex);
            }
        }
    }
}
