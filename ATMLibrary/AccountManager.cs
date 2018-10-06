using ATMLibrary.Accounts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

using System.Text;

namespace ATMLibrary
{
    public class AccountManager
    {

        public static List<BankAccount> FetchAccounts(User user)
        {
            try
            {
                List<BankAccount> accounts = new List<BankAccount>();
                using (MySqlConnection connection = new MySqlConnection(SQLData.connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `accounts` WHERE `owneridx`=@OWNER", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BankAccount bankAcc = ReadBankAccountData(user, reader);
                                if (bankAcc != null)
                                {
                                    accounts.Add(bankAcc);
                                }
                            }
                            return accounts;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static BankAccount ReadBankAccountData(User user, MySqlDataReader reader)
        {
            BankAccount bankAcc = null;
            long accID;
            if (!long.TryParse(reader["idx"].ToString(), out accID))
            {
                return null;
            }

            long userID;
            if(!long.TryParse(reader["owneridx"].ToString(), out userID))
            {
                return null;
            }

            long balance;
            if(!long.TryParse(reader["balance"].ToString(),out balance))
            {
                return null;
            }

            switch ((AccountType)Convert.ToByte(reader["type"]))
            {
                case AccountType.Checking:
                    bankAcc = new CheckingAccount(accID, balance);
                    break;

                case AccountType.Savings:
                    bankAcc = new SavingsAccount(accID, balance);
                    break;

                case AccountType.Business:
                    bankAcc = new BusinessAccount(accID, balance);
                    break;
            }
            return bankAcc;
        }
    }
}
