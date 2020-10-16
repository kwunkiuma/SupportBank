using System;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Globalization;
using Newtonsoft.Json;
using System.IO;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private static void InitLog()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                FileName = @"C:\Work\Logs\SupportBank.log",
                Layout = @"${longdate} ${level} - ${logger}: ${message}"
            };
            var consoleTarget = new ConsoleTarget
            {
                Name = @"Console",
                Layout = @"${level} - ${message}"
            };
            config.AddTarget("File Logger", fileTarget);
            config.AddTarget("Console Logger", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, consoleTarget));
            LogManager.Configuration = config;

            logger.Info("Logger initialised");
        }

        static string ReadConsole(string prompt)
        {
            string answer = null;
            do
            {
                Console.Write(prompt);
                try
                {
                    answer = Console.ReadLine();
                }
                catch
                {
                }
            } while (string.IsNullOrEmpty(answer));

            return answer;
        }
     
        static void ExecuteCommand(Dictionary<string, Account> accounts)
        {
            string input = ReadConsole("Input a command: ");

            if (input.StartsWith("List "))
            {
                OutputHelper.ShowList(accounts, input, logger);
                return;
            }

            if (input.StartsWith("Import File "))
            {
                FileHelper.ImportFile(accounts, input, logger);
                return;
            }

            logger.Error($"Invalid command; {input}.");
        }

        static void Main(string[] args)
        {
            InitLog();

            var accounts = new Dictionary<string, Account>();

            while (true)
            {
                ExecuteCommand(accounts);
            }
        }
    }
}