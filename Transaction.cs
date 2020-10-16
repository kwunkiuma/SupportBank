using Newtonsoft.Json;
using System.Collections.Generic;

public class Transaction
{
    [JsonProperty("FromAccount")]
    public readonly string from;
    [JsonProperty("ToAccount")]
    public readonly string to;

    [JsonProperty("Date")]
    private string date;
    [JsonProperty("Narrative")]
    private string narrative;
    [JsonProperty("Amount")]
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
    }
}
