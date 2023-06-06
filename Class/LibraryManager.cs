using System;
using System.Data.SqlClient;

namespace Book_Rental.Class
{
    class LibraryManager
    {
        private string connectionString = "Data Source=DKOMPUTER\\WINCC;Initial Catalog=MyLibrary;Integrated Security=True";
        private BookManager bookManager;
        private ReaderManager readerManager;

        public LibraryManager()
        {
            bookManager = new BookManager(connectionString);
            readerManager = new ReaderManager(connectionString);
        }

        public void DisplayBooks()
        {
            bookManager.DisplayBooks();
        }

        public void BorrowBook()
        {
            Console.WriteLine("Readers available:");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, FirstName, LastName FROM Customers";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("{0,-5} | {1,-15} | {2,-15}", "ID", "First name", "Last name");
                Console.WriteLine("---------------------------------");

                while (reader.Read())
                {
                    int customerId = (int)reader["Id"];
                    string firstName = (string)reader["FirstName"];
                    string lastName = (string)reader["LastName"];

                    Console.WriteLine("{0,-5} | {1,-15} | {2,-15}", customerId, firstName, lastName);
                }

                reader.Close();
            }

            Console.WriteLine();

            Console.WriteLine("Enter the ID of the reader:");
            int selectedCustomerId = int.Parse(Console.ReadLine());

            Console.WriteLine();

            DisplayBooks();

            Console.WriteLine();
            Console.WriteLine("Enter the ID of the book:");
            int selectedBookId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkAvailabilityQuery = "SELECT Quantity, BorrowedQuantity FROM Books WHERE Id = @BookId";

                SqlCommand checkAvailabilityCommand = new SqlCommand(checkAvailabilityQuery, connection);
                checkAvailabilityCommand.Parameters.AddWithValue("@BookId", selectedBookId);

                connection.Open();

                SqlDataReader availabilityReader = checkAvailabilityCommand.ExecuteReader();

                if (availabilityReader.Read())
                {
                    int quantity = (int)availabilityReader["Quantity"];
                    int borrowedQuantity = (int)availabilityReader["BorrowedQuantity"];

                    if (quantity - borrowedQuantity > 0)
                    {
                        string checkBorrowedBooksQuery = "SELECT COUNT(*) FROM Books WHERE CurrentCustomerId = @CustomerId";

                        SqlCommand checkBorrowedBooksCommand = new SqlCommand(checkBorrowedBooksQuery, connection);
                        checkBorrowedBooksCommand.Parameters.AddWithValue("@CustomerId", selectedCustomerId);

                        availabilityReader.Close();

                        int borrowedBooksCount = (int)checkBorrowedBooksCommand.ExecuteScalar();

                        if (borrowedBooksCount < 5)
                        {
                            string borrowQuery = "UPDATE Books SET BorrowedQuantity = BorrowedQuantity + 1, CurrentCustomerId = @CustomerId WHERE Id = @BookId";

                            SqlCommand borrowCommand = new SqlCommand(borrowQuery, connection);
                            borrowCommand.Parameters.AddWithValue("@CustomerId", selectedCustomerId);
                            borrowCommand.Parameters.AddWithValue("@BookId", selectedBookId);

                            int rowsAffected = borrowCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("The book has been borrowed.");

                                // Update the number of available and borrowed books in the database
                                string updateQuantityQuery = "UPDATE Books SET Quantity = Quantity - 1 WHERE Id = @BookId";
                                SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection);
                                updateQuantityCommand.Parameters.AddWithValue("@BookId", selectedBookId);
                                updateQuantityCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                Console.WriteLine("The book could not be borrowed. Verify the book ID and reader ID are correct.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("You cannot borrow more than 5 books at a time");
                        }
                    }
                    else
                    {
                        Console.WriteLine("This book cannot be borrowed. No copies available");
                    }
                }
                else
                {
                    Console.WriteLine("The book with the given ID could not be found");
                }

                availabilityReader.Close();
            }
        }

        public void ReturnBook()
        {
            Console.WriteLine("Enter the ID of the reader:");
            int customerId = int.Parse(Console.ReadLine());

            Console.WriteLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkBorrowedBooksQuery = "SELECT B.Id, B.Title FROM Books B WHERE B.CurrentCustomerId = @CustomerId";

                SqlCommand checkBorrowedBooksCommand = new SqlCommand(checkBorrowedBooksQuery, connection);
                checkBorrowedBooksCommand.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();

                SqlDataReader borrowedBooksReader = checkBorrowedBooksCommand.ExecuteReader();

                if (borrowedBooksReader.HasRows)
                {
                    Console.WriteLine("Borrowed books:");
                    Console.WriteLine("{0,-5} | {1,-30}", "ID", "Tytuł");
                    Console.WriteLine("---------------------------------------");

                    while (borrowedBooksReader.Read())
                    {
                        int bookId = (int)borrowedBooksReader["Id"];
                        string title = (string)borrowedBooksReader["Title"];

                        Console.WriteLine("{0,-5} | {1,-30}", bookId, title);
                    }

                    borrowedBooksReader.Close();

                    Console.WriteLine();
                    Console.WriteLine("Enter the ID of the book you want to return:");
                    int bookIdToReturn = int.Parse(Console.ReadLine());

                    string returnQuery = "UPDATE Books SET BorrowedQuantity = BorrowedQuantity - 1, CurrentCustomerId = NULL WHERE Id = @BookId AND CurrentCustomerId = @CustomerId";

                    SqlCommand returnCommand = new SqlCommand(returnQuery, connection);
                    returnCommand.Parameters.AddWithValue("@BookId", bookIdToReturn);
                    returnCommand.Parameters.AddWithValue("@CustomerId", customerId);

                    int rowsAffected = returnCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("The book has been returned");


                        // Update the number of available and borrowed books in the database
                        string updateQuantityQuery = "UPDATE Books SET Quantity = Quantity + 1 WHERE Id = @BookId";
                        SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection);
                        updateQuantityCommand.Parameters.AddWithValue("@BookId", bookIdToReturn);
                        updateQuantityCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("The book could not be returned. Verify the book ID is correct");
                    }
                }
                else
                {
                    Console.WriteLine("The reader has no borrowed books");
                }

                borrowedBooksReader.Close();
            }
        }

        public void DisplayReadersWithBorrowedBooks()
        {
            readerManager.DisplayReadersWithBorrowedBooks();
        }

        public void AddReader()
        {
            Console.WriteLine("Enter the reader first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter the reader last name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Enter the reader's email address:");
            string email = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string insertReaderQuery = "INSERT INTO Customers (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email)";

                SqlCommand insertReaderCommand = new SqlCommand(insertReaderQuery, connection);
                insertReaderCommand.Parameters.AddWithValue("@FirstName", firstName);
                insertReaderCommand.Parameters.AddWithValue("@LastName", lastName);
                insertReaderCommand.Parameters.AddWithValue("@Email", email);

                connection.Open();

                int rowsAffected = insertReaderCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("The reader has been added to the database");
                }
                else
                {
                    Console.WriteLine("Failed to add reader to database");
                }
            }
        }
    }
}