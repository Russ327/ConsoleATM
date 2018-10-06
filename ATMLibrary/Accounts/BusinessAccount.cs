using System;
using System.Collections.Generic;
using System.Text;

namespace ATMLibrary.Accounts
{
    public class BusinessAccount : BankAccount
    {
        internal BusinessAccount(long id, long balance) : base(id, balance)
        {

        }

        public override string ToString()
        {
            return "Business";
        }

        new public AccountType GetType
        {
            get
            {
                return AccountType.Business;
            }
        }
    }
}
