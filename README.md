Exercises 1: Log File Analyzer
Create a program that analyzes log files and generates a summary report.

Requirements:
Read from a log file with entries in this format:

[TIMESTAMP] [LEVEL] Message
[2024-01-15 10:30:15] [ERROR] Database connection failed
[2024-01-15 10:30:20] [INFO] Server started
[2024-01-15 10:31:00] [WARNING] High memory usage  
Program should:

Count occurrences of each log level (ERROR, INFO, WARNING)
Find and list all ERROR messages
Calculate time span between first and last log entry
Create a summary report in a new file
Implementation must:

Use proper file handling with using statements
Include error handling for file operations
Use appropriate stream readers
Format output clearly
Expected Output Example:

Log Analysis Report
------------------
Analysis Date: 2024-01-15
Log File: application.log

Summary:
- Total Entries: 150
- ERROR Count: 5
- WARNING Count: 12
- INFO Count: 133
- Time Span: 2 hours 15 minutes

Error Messages:
1. [10:30:15] Database connection failed
2. [11:15:20] Memory allocation error
...

Report generated successfully!  
