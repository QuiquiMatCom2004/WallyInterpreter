namespace WallyInterpreter.Components.Services
{
    public class ConfigurationLog
    {
        private Logger<ConfigurationLog> logger;
        private readonly string PathArchive = "Logs/errors.log";
        public ConfigurationLog()
        {
            logger = new Logger<ConfigurationLog>( new LoggerFactory());
        }

        public void LogInformation(string message)
        {
            logger.LogInformation(message);
            WriteToFile("INFO", message);
        }

        public void LogWarning(string message)
        {
            logger.LogWarning(message);
            WriteToFile("WARNING", message);
        }

        public void LogError(Exception ex)
        {
            string errorMessage = $"Error: {ex.Message}\n{ex.StackTrace}";
            logger.LogError(ex, errorMessage);
            WriteToFile("ERROR", errorMessage);
        }
        private void WriteToFile(string level, string message)
        {
            string logEntry = $"{DateTime.Now} [{level}: {message}]\n";
            Directory.CreateDirectory("Logs");
            File.AppendAllText(PathArchive, logEntry);
        }
    }
}
