using Newtonsoft.Json;
using System.Collections.Generic;

public class Transaction
{
    private string date;
    [JsonProperty("FromAccount")]
    private string from;
    [JsonProperty("ToAccount")]
    private string to;
    private string narrative;
    private decimal amount;

    public Transaction(string date, string from, string to, string narrative, decimal amount)
    {
        this.date = date;
        this.from = from;
        this.to = to;
        this.narrative = narrative;
        this.amount = amount;
    }

    public string GetSummary()
    {
        return $"{date} {narrative}";
    }

    public decimal GetChangeForAccount(string name)
    {
        if (from == name)
        {
            return -amount;
        }

        if (to == name)
        {
            return amount;
        }

        return 0;
    }

    public void UpdateAccounts(Dictionary<string, Account> accounts)
    {
        if (!accounts.ContainsKey(from))
        {
            accounts.Add(from, new Account(from));
        }

        if (!accounts.ContainsKey(to))
        {
            accounts.Add(to, new Account(to));
        }

        accounts[from].AddTransaction(this);
        accounts[to].AddTransaction(this);
    }
}
