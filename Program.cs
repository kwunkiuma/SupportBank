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

        static Dictionary<string, Account> ReadCSV(string filename)
        {
            logger.Info($"Initialising parser for: {filename}.");
            var parser = new TextFieldParser(filename)
            {
                TextFieldType = FieldType.Delimited
            };
            parser.SetDelimiters(",");
            logger.Info("Parser initialized, parsing file.");

            // Skip first line
            if (!parser.EndOfData)
            {
                parser.ReadLine();
            }

            var accounts = new Dictionary<string, Account>();

            while (!parser.EndOfData)
            {
                var line = parser.LineNumber;
                var fields = parser.ReadFields();

                var date = fields[0];
                var from = fields[1];
                var to = fields[2];
                var narrative = fields[3];
                if (!DateTime.TryParseExact(fields[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    logger.Warn($"Skipped line {line} due to invalid date: {fields[0]}.");
                }

                if (!decimal.TryParse(fields[4], out decimal amount))
                {
                    logger.Warn($"Skipped line {line} due to invalid amount: {fields[4]}.");
                }

                var newTransaction = new Transaction(date, from, to, narrative, amount);

                newTransaction.UpdateAccounts(accounts);
            }

            logger.Info("Parsing successful.");

            return accounts;
        }

        static Dictionary<string, Account> ReadJSON(string filename)
        {
            logger.Info($"Initialising parser for: {filename}.");
            List<Transaction> jsonItems;
            using (StreamReader streamReader = new StreamReader(filename))
            {
                string serialized = streamReader.ReadToEnd();
                jsonItems = JsonConvert.DeserializeObject<List<Transaction>>(serialized);
            }

            logger.Info("Parser initialized, parsing file.");

            var accounts = new Dictionary<string, Account>();
            foreach (var transaction in jsonItems)
            {
                transaction.UpdateAccounts(accounts);
            }

            logger.Info("Parsing successful.");

            return accounts;
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

        static void ListAll(Dictionary<string, Account> accounts)
        {
            foreach (string name in accounts.Keys)
            {
                Console.WriteLine(accounts[name].GetSummary());
            }
        }

        static void ListAccount(Dictionary<string, Account> accounts, string name)
        {
            foreach (Transaction transaction in accounts[name].Transactions)
            {
                Console.WriteLine(transaction.GetSummary());
            }
        }
        static void ShowList(Dictionary<string, Account> accounts, string input)
        {
            string operand = input.Substring(5);
            logger.Info($"User requested list for {operand}.");

            switch (operand)
            {
                case "All":
                    ListAll(accounts);
                    break;
                default:
                    if (!accounts.ContainsKey(operand))
                    {
                        Console.WriteLine("Account not found");
                        logger.Error($"{operand} does not exist.");
                        break;
                    }
                    ListAccount(accounts, operand);
                    break;
            }
        }

        static void ImportFile(Dictionary<String, Account> accounts, string input)
        {
            string operand = input.Substring(12);
            logger.Info($"User requested file import for {operand}.");

            try
            {
                string extension = input.Substring(input.LastIndexOf("."));
                switch (extension)
                {
                    case ".csv":
                        accounts = ReadCSV(operand);
                        break;
                    case ".json":
                        accounts = ReadJSON(operand);
                        break;
                    default:
                        logger.Error($"Invalid file extension: {extension}.");
                        break;
                }
            }
            catch
            {
                logger.Error($"Invalid filename: {operand}.");
            }
        }
     
        static void ExecuteCommand(Dictionary<string, Account> accounts)
        {
            string input = ReadConsole("Input a command: ");

            if (input.StartsWith("List "))
            {
                ShowList(accounts, input);
                return;
            }

            if (input.StartsWith("Import File "))
            {
                ImportFile(accounts, input);
                return;
            }

            logger.Error($"Invalid command; {input}.");
        }

        static void Main(string[] args)
        {
            InitLog();
            var accounts = ReadJSON(@"Transactions2013.json");

            while (true)
            {
                ExecuteCommand(accounts);
            }
        }
    }
}