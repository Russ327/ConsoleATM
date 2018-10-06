using ATMLibrary.Accounts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ATMLibrary
{
    public class UserManager
    {
        /// <summary>
        /// Checks if email currently exists in the user database. See CheckEmail. This
        /// was all originally one method but has been split into two so it is easier to read.
        /// </summary>
        /// <param name="email">Email to be checked.</param>
        /// <returns>true if </returns>
        public static bool EmailExists(string email)
        {
            try
            {
                return CheckEmail(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Internal method that checks if the passed email is registered within the database.
        /// This is only used internally. See UserManager.EmailExists for more info.
        /// </summary>
        /// <param name="email">The email to check if is registered.</param>
        /// <returns>True if found false if not.</returns>
        private static bool CheckEmail(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLData.connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `users` WHERE `email`=@EMAIL", connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@EMAIL", email);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else return false;
                    }
                }
            }
        }


        /// <summary>
        /// Registers users with given username and password. Passwords are hashed internally
        /// using MD5. 
        /// </summary>
        /// <param name="email">email to register</param>
        /// <param name="password">password of users account.</param>
        /// <returns>true if successful or false if failed.</returns>
        public static bool RegisterUser(string email, string password)
        {
            if (CheckEmail(email) == false)
            {
                using (MySqlConnection connection = new MySqlConnection(SQLData.connectionString))
                {
                    string hashedPass = Utility.HashPassword(password);
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `users` SET `email`=@EMAIL, `password`=@PASSWORD",connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@EMAIL", email);
                        cmd.Parameters.AddWithValue("@PASSWORD", hashedPass);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            else return false;
        }

        /// <summary>
        /// Updates the user pin. 
        /// </summary>
        /// <param name="userIndex">users primary unique key in database</param>
        /// <param name="pin">desired pin</param>
        public static int UpdatePin(long userIndex, ushort pin)
        {
            using (MySqlConnection connection = new MySqlConnection(SQLData.connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `users` SET `pin`=@PIN WHERE `idx`=@INDEX", connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@PIN", pin);
                    cmd.Parameters.AddWithValue("@INDEX", userIndex);
                    return cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Attempts to log user into the atm. Returns false if failing. Passwords are hashed interally
        /// before searching account.
        /// </summary>
        /// <param name="email">email of account</param>
        /// <param name="password">password of account</param>
        /// <returns>User instance if sucessful null if not.</returns>
        public static User LoginUser(string email, string password)
        {
            if (CheckEmail(email) == true)
            {
                using (MySqlConnection connection = new MySqlConnection(SQLData.connectionString))
                {
                    string hashedPass = Utility.HashPassword(password);
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `users` WHERE `email`=@EMAIL AND `password`=@PASSWORD LIMIT 1", connection))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@EMAIL", email);
                        cmd.Parameters.AddWithValue("@PASSWORD", hashedPass);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            long idx;
                            if (long.TryParse(reader["idx"].ToString(), out idx))
                            {
                                return new User(idx, ushort.MaxValue);

                            }
                            else
                            {
                                return null;
                            }
                        }
                        
                    }
                }
            }
            else return null;
        }
    }
}
