﻿using System.Collections.Generic;

public class Account
{
    public List<Transaction> Transactions { get; }

    private string name;
    private decimal balance;

    public Account(string name)
    {
        this.name = name;
        Transactions = new List<Transaction>();
    }

    public void AddTransaction(Transaction newTrans)
    {
        Transactions.Add(newTrans);

        balance += newTrans.GetChangeForAccount(name);
    }

    public string GetSummary()
    {
        return string.Concat(name, " ", balance.ToString());
    }
}
