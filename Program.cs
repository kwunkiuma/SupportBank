using System;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace SupportBank
{
    class Program
    {
        static Dictionary<string, Account> ReadFile() {

            var accounts = new Dictionary<string, Account>();

            var parser = new TextFieldParser(@"Transactions2014.csv")
            {
                TextFieldType = FieldType.Delimited
            };
            parser.SetDelimiters(",");

            // Skip first line
            if (!parser.EndOfData)
            {
                parser.ReadLine();
            }

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();

                var date = fields[0];
                var from = fields[1];
                var to = fields[2];
                var narrative = fields[3];
                var amount = decimal.Parse(fields[4]);

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
            foreach (Transaction trans in accounts[name].Transactions)
            {
                Console.WriteLine(trans.GetSummary());
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

            switch (operand)
            {
                case "All":
                    ListAll(accounts);
                    break;
                default:
                    if (!accounts.ContainsKey(operand))
                    {
                        Console.WriteLine("Account not found");
                    }
                    ListAccount(accounts, operand);
                    break;
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, Account> accounts = ReadFile();

            while (true)
            {
                ExecuteCommand(accounts);
            }
        }
    }
}