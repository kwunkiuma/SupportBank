public class Transaction
{
    private string date;
    public string from;
    public string to;
    private string narrative;
    private decimal amount;

    public Transaction(string[] fields)
    {
        date = fields[0];
        from = fields[1];
        to = fields[2];
        narrative = fields[3];
        amount = decimal.Parse(fields[4]);
    }

    public string GetSummary()
    {
        //return string.Join(" | ", new string[] { date, from, to, narrative, amount.ToString() });
        return string.Join(" ", new string[] {date, narrative});
    }

    public decimal GetChange(string name)
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
