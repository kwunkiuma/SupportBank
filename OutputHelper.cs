using NLog;
using System;
using System.Collections.Generic;

public class OutputHelper
{
    public static void ShowList(Dictionary<string, Account> accounts, string input, ILogger logger)
    {
        var operand = input.Substring(5);
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

    static void ListAll(Dictionary<string, Account> accounts)
    {
        foreach (var name in accounts.Keys)
        {
            Console.WriteLine(accounts[name].GetSummary());
        }
    }

    static void ListAccount(Dictionary<string, Account> accounts, string name)
    {
        foreach (var transaction in accounts[name].Transactions)
        {
            Console.WriteLine(transaction.GetSummary());
        }
    }
}
