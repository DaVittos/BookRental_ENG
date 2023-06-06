using System;
using System.Data.SqlClient;

namespace Book_Rental.Class
{
    class ReaderManager
    {
        private string connectionString;

        public ReaderManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void DisplayReadersWithBorrowedBooks()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT C.Id, C.FirstName, C.LastName, C.Email, B.Title FROM Customers C LEFT JOIN Books B ON C.Id = B.CurrentCustomerId";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("List of readers with borrowed books:");
                Console.WriteLine("{0,-5} | {1,-15} | {2,-15} | {3,-30} | {4,-30}", "ID", "First name", "Last name", "E-mail", "Borrowed books");
                Console.WriteLine("--------------------------------------------------------------------------");

                while (reader.Read())
                {
                    int customerId = (int)reader["Id"];
                    string firstName = (string)reader["FirstName"];
                    string lastName = (string)reader["LastName"];
                    string email = (string)reader["Email"];
                    string title = reader.IsDBNull(4) ? "" : (string)reader["Title"];

                    Console.WriteLine("{0,-5} | {1,-15} | {2,-15} | {3,-30} | {4,-30}",
                        customerId, firstName, lastName, email, title);
                }

                reader.Close();
            }
        }
    }
}