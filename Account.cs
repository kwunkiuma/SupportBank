using System.Collections.Generic;

public class Account
{
	private string name;
	private decimal balance;

	public List<Transaction> transactions { get; }

	public Account(string name)
	{
		this.name = name;
		transactions = new List<Transaction>();
	}

	public void AddTransaction(Transaction newTrans)
	{
		transactions.Add(newTrans);
		balance += newTrans.GetChange(name);
	}

	public string GetSummary()
	{
		return string.Concat(name, " ", balance.ToString());
	}
}
