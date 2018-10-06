using System;
using System.Collections.Generic;
using System.Text;

namespace ATMLibrary.Accounts
{
    public class SavingsAccount : BankAccount
    {
        internal SavingsAccount(long id, long balance) : base(id, balance)
        {
        }

        public override string ToString()
        {
            return "Savings";
        }

        new public AccountType GetType
        {
            get
            {
                return AccountType.Savings;
            }
        }
    }
}
