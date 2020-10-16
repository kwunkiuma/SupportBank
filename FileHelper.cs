using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class FileHelper
{
    public static void ImportFile(Dictionary<String, Account> accounts, string input, ILogger logger)
    {
        var operand = input.Substring(12);
        logger.Info($"User requested file import for {operand}.");

        try
        {
            var extension = input.Substring(input.LastIndexOf("."));
            switch (extension)
            {
                case ".csv":
                    ReadCSV(accounts, operand, logger);
                    break;
                case ".json":
                    ReadJSON(accounts, operand, logger);
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

    static void AddTransaction(Dictionary<string, Account> accounts, Transaction transaction)
    {
        if (!accounts.ContainsKey(transaction.from))
        {
            accounts.Add(transaction.from, new Account(transaction.from));
        }

        if (!accounts.ContainsKey(transaction.to))
        {
            accounts.Add(transaction.to, new Account(transaction.to));
        }

        accounts[transaction.from].AddTransaction(transaction);
        accounts[transaction.to].AddTransaction(transaction);
    }

    static void ReadCSV(Dictionary<string, Account> accounts, string filename, ILogger logger)
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

            var transaction = new Transaction(date, from, to, narrative, amount);

            AddTransaction(accounts, transaction);

        }

        logger.Info("Parsing successful.");
    }

    static void ReadJSON(Dictionary<string, Account> accounts, string filename, ILogger logger)
    {
        logger.Info($"Initialising parser for: {filename}.");
        List<Transaction> jsonItems;
        using StreamReader streamReader = new StreamReader(filename);
        string serialized = streamReader.ReadToEnd();
        jsonItems = JsonConvert.DeserializeObject<List<Transaction>>(serialized);

        logger.Info("Parser initialized, parsing file.");

        foreach (var transaction in jsonItems)
        {
            AddTransaction(accounts, transaction);
        }

        logger.Info("Parsing successful.");
    }
}
