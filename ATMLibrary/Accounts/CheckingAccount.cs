using System;
using System.Collections.Generic;
using System.Text;

namespace ATMLibrary.Accounts
{
    public class CheckingAccount : BankAccount
    {
        internal CheckingAccount(long id, long balance) : base(id, balance)
        {
        }

        public override string ToString()
        {
            return "Checking";
        }

        new public AccountType GetType
        {
            get
            {
                return AccountType.Checking;
            }
        }
    }
}
