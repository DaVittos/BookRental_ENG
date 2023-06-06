using System;
using System.Data.SqlClient;

namespace Book_Rental.Class
{
    class BookManager
    {
        private string connectionString;

        public BookManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void DisplayBooks()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT B.Id, B.Title, A.Name AS Author, B.PublicationYear, B.Quantity, B.BorrowedQuantity FROM Books B INNER JOIN Authors A ON B.AuthorId = A.Id";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("Book list:");
                Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,-15} | {4,-10} | {5,-12}", "ID", "Title", "Author", "Publication Year", "Quantity", "Borrowed");
                Console.WriteLine("-------------------------------------------------------");

                while (reader.Read())
                {
                    int bookId = (int)reader["Id"];
                    string title = (string)reader["Title"];
                    string author = (string)reader["Author"];
                    int publicationYear = (int)reader["PublicationYear"];
                    int quantity = (int)reader["Quantity"];
                    int borrowedQuantity = (int)reader["BorrowedQuantity"];

                    Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,-15} | {4,-10} | {5,-12}",
                        bookId, title.PadRight(30), author.PadRight(20), publicationYear, quantity, borrowedQuantity);
                }

                reader.Close();
            }
        }
    }
}