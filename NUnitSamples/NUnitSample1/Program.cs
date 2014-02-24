using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Program code
namespace NUnitSample1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("End of Program");
        }
    }

    public class Account
    {
        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
        }

        private decimal minimumBalance = 10m;
        public decimal MinimimumBalance
        {
            get { return minimumBalance; }
        }

        public void Deposit(decimal amount)
        {
            balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            balance -= amount;
        }

        public void TransferFunds(Account destination, decimal amount)
        {
            if (balance - amount < minimumBalance)
                throw new InsufficientFundsException();

            destination.Deposit(amount);
            Withdraw(amount);
        }
    }

    public class InsufficientFundsException : ApplicationException
    {
    }
}
#endregion

