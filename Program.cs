using System;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace SupportBank
{
    class Program
    {
        static Dictionary<string, Account> ReadFile() {

            var accounts = new Dictionary<string, Account>();

            var parser = new TextFieldParser(@"Transactions2014.csv");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            if (!parser.EndOfData)
                parser.ReadLine();

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                var newTrans = new Transaction(fields);

                foreach (string name in fields[1..3])
                {
                    if (!accounts.ContainsKey(name))
                    {
                        accounts.Add(name, new Account(name));
                    }
                    accounts[name].AddTransaction(newTrans);
                }
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
            foreach (Transaction trans in accounts[name].transactions)
            {
                Console.WriteLine(trans.GetSummary());
            }
        }

        static bool ExecuteCommand(Dictionary<string, Account> accounts)
        {
            string input;

            do
            {
                input = ReadConsole("Input a command: ");
            } while (!input.StartsWith("List "));

            string param = input.Substring(5);

            switch (param)
            {
                case "All":
                    ListAll(accounts);
                    break;
                default:
                    if (!accounts.ContainsKey(param))
                        Console.WriteLine("Account not found");
                    ListAccount(accounts, param);
                    break;
            }

            return true;
        }

        static void Main(string[] args)
        {
            Dictionary<string, Account> accounts = ReadFile();

            while (ExecuteCommand(accounts)) { }
        }
    }
}