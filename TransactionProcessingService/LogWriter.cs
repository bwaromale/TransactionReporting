using TransactionProcessingService.Models;


namespace TransactionProcessingService
{
    public class LogWriter
    {
        private static string _logFileLocation = "C:\\Logs\\Transactioneer\\TransactionProcessingService\\";
        private static string _logFileName = DateTime.Now.Year.ToString()
                                        + DateTime.Now.Month.ToString()
                                        + DateTime.Now.Day.ToString()
                                        + "TransactionProcessingServiceLog.txt";
        private static string _logFilePath = Path.Combine(_logFileLocation, _logFileName);
        private static string _exceptionLogFileName = DateTime.Now.Year.ToString()
                                        + DateTime.Now.Month.ToString()
                                        + DateTime.Now.Day.ToString()
                                        + "TransactionProcessingServiceExceptionLog.txt";
        private static string _exceptionLogFilePath = Path.Combine(_logFileLocation, _exceptionLogFileName);

        
        public static void WriteToLog(string logText)
        {
            try
            {
                if (!Directory.Exists(_logFileLocation))
                {
                    Directory.CreateDirectory(_logFileLocation);
                }
                
                if(!File.Exists(_logFilePath))
                {
                    File.Create(_logFilePath).Close();
                }
                using(StreamWriter sw = new StreamWriter(_logFilePath, true))
                {
                    sw.WriteLine(logText);
                }
            }
            catch(Exception ex) { }
        }
        public static void WriteExceptionToLog(string exception)
        {
            try
            {
                if (!Directory.Exists(_logFileLocation))
                {
                    Directory.CreateDirectory(_logFileLocation);
                }

                if (!File.Exists(_exceptionLogFilePath))
                {
                    File.Create(_exceptionLogFilePath).Close();
                }
                using (StreamWriter sw = new StreamWriter(_exceptionLogFilePath, true))
                {
                    sw.WriteLine(exception);
                }
            }
            catch (Exception ex) { }
        }
    }
}
