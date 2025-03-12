using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class LogFileAnalyzer
{
    static void Main()
    {
        string logFilePath = "application.log";
        string reportFilePath = "log_report.txt"; 

        if (!File.Exists(logFilePath))
        {
            Console.WriteLine("Error: Log file not found.");
            return;
        }

        try
        {
            var logLevelCounts = new Dictionary<string, int> { { "ERROR", 0 }, { "WARNING", 0 }, { "INFO", 0 } };
            var errorMessages = new List<string>();
            DateTime? firstTimestamp = null, lastTimestamp = null;
            int totalEntries = 0;

            string pattern = @"^\[(.*?)\] \[(ERROR|WARNING|INFO)\] (.*)$";

            using (StreamReader reader = new StreamReader(logFilePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    Match match = Regex.Match(line, pattern);
                    if (!match.Success) continue;

                    string timestampStr = match.Groups[1].Value;
                    string level = match.Groups[2].Value.ToUpper();
                    string message = match.Groups[3].Value;

                    if (DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                    {
                        firstTimestamp ??= timestamp;
                        lastTimestamp = timestamp;
                    }

                    logLevelCounts[level]++;
                    totalEntries++;

                    if (level == "ERROR")
                    {
                        errorMessages.Add($"[{timestamp:HH:mm:ss}] {message}");
                    }
                }
            }

            TimeSpan timeSpan = (firstTimestamp.HasValue && lastTimestamp.HasValue)
                ? lastTimestamp.Value - firstTimestamp.Value
                : TimeSpan.Zero;

            using (StreamWriter writer = new StreamWriter(reportFilePath))
            {
                writer.WriteLine("Log Analysis Report");
                writer.WriteLine("------------------");
                writer.WriteLine($"Analysis Date: {DateTime.Now:yyyy-MM-dd}");
                writer.WriteLine($"Log File: {logFilePath}");
                writer.WriteLine();
                writer.WriteLine("Summary:");
                writer.WriteLine($"- Total Entries: {totalEntries}");
                writer.WriteLine($"- ERROR Count: {logLevelCounts["ERROR"]}");
                writer.WriteLine($"- WARNING Count: {logLevelCounts["WARNING"]}");
                writer.WriteLine($"- INFO Count: {logLevelCounts["INFO"]}");
                writer.WriteLine($"- Time Span: {timeSpan.Hours} hours {timeSpan.Minutes} minutes");
                writer.WriteLine();
                writer.WriteLine("Error Messages:");


                for (int i = 0; i < errorMessages.Count; i++)
                {
                    writer.WriteLine($"{i + 1}. {errorMessages[i]}");
                }

                writer.WriteLine();
                writer.WriteLine("Report generated successfully!");
            }

            Console.WriteLine("Log analysis completed. Report saved to log_report.txt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" An error occurred while processing the log file: {ex.Message}");
        }
    }
}
