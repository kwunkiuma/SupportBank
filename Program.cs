using System;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Globalization;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static Dictionary<string, Account> ReadFile(string filename)
        {
            logger.Info($"Initialising parser for: { filename }.");
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
            var line = 0;

            while (!parser.EndOfData)
            {
                line++;
                var fields = parser.ReadFields();

                var date = fields[0];
                var from = fields[1];
                var to = fields[2];
                var narrative = fields[3];

                if (!DateTime.TryParseExact(fields[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                {
                    string message = $"Skipped line { line } due to invalid date: { fields[0] }.";
                    Console.WriteLine(message);
                    logger.Warn(message);
                }

                if (!decimal.TryParse(fields[4], out decimal amount))
                {
                    string message = $"Skipped line { line } due to invalid amount: { fields[4] }.";
                    Console.WriteLine(message);
                    logger.Warn(message);
                }

                var newTransaction = new Transaction(date, from, to, narrative, amount);

                if (!accounts.ContainsKey(from))
                {
                    accounts.Add(from, new Account(from));
                }

                if (!accounts.ContainsKey(to))
                {
                    accounts.Add(to, new Account(to));
                }

                accounts[from].AddTransaction(newTransaction);
                accounts[to].AddTransaction(newTransaction);
            }

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

        static void ExecuteCommand(Dictionary<string, Account> accounts)
        {
            string input;

            do
            {
                input = ReadConsole("Input a command: ");
            } while (!input.StartsWith("List "));

            string operand = input.Substring(5);
            logger.Info($"User requested list for { operand }.");

            switch (operand)
            {
                case "All":
                    ListAll(accounts);
                    break;
                default:
                    if (!accounts.ContainsKey(operand))
                    {
                        Console.WriteLine("Account not found");
                        logger.Error($"{ operand } does not exist.");
                        break;
                    }
                    ListAccount(accounts, operand);
                    break;
            }
        }

        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            logger.Info("Logger initialised");

            var accounts = ReadFile(@"DodgyTransactions2015.csv");

            while (true)
            {
                ExecuteCommand(accounts);
            }
        }
    }
}