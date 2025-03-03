using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

class LogFileAnalyzer
{
    static void Main()
    {
        string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "application.log");
        string reportFilePath = "log_report.txt";
        string jsonReportPath = "log_report.json";

        Console.WriteLine($"Looking for log file at: {logFilePath}");

        if (!File.Exists(logFilePath))
        {
            Console.WriteLine("Error: Log file not found.");
            return;
        }

        try
        {
            Dictionary<string, int> logLevelCounts = new Dictionary<string, int>
            {
                { "ERROR", 0 },
                { "WARNING", 0 },
                { "INFO", 0 }
            };
            List<string> errorMessages = new List<string>();
            DateTime? firstTimestamp = null, lastTimestamp = null;

            string pattern = @"^\[(.*?)\] \[(.*?)\] (.*)$"; // Regex for log parsing

            using (StreamReader reader = new StreamReader(logFilePath))
            {
                Console.WriteLine("Reading log file...");
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Console.WriteLine($"Read line: {line}");

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    Match match = Regex.Match(line, pattern);
                    if (!match.Success) continue;

                    string timestampStr = match.Groups[1].Value;
                    string level = match.Groups[2].Value.ToUpper();
                    string message = match.Groups[3].Value;

                    Console.WriteLine($"Parsed - Timestamp: {timestampStr}, Level: {level}, Message: {message}");

                    if (DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                    {
                        firstTimestamp ??= timestamp;
                        lastTimestamp = timestamp;
                    }

                    if (logLevelCounts.ContainsKey(level))
                    {
                        logLevelCounts[level]++;
                    }

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
                writer.WriteLine($"- Total Entries: {logLevelCounts.Values.Sum()}");
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

            Console.WriteLine("✅ Log analysis completed. Report saved to log_report.txt");

            var summary = new
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                TotalEntries = logLevelCounts.Values.Sum(),
                Errors = logLevelCounts["ERROR"],
                Warnings = logLevelCounts["WARNING"],
                Info = logLevelCounts["INFO"],
                TimeSpan = $"{timeSpan.Hours} hours {timeSpan.Minutes} minutes",
                ErrorMessages = errorMessages
            };

            string jsonReport = JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonReportPath, jsonReport);
            Console.WriteLine("✅ JSON report saved to log_report.json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred while processing the log file: {ex.Message}");
        }
    }
}
