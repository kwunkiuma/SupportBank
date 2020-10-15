public class Transaction
{
    private string date;
    private string from;
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
}
