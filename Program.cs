using System;
using Book_Rental.Class;

namespace Book_Rental
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to our Book Rental! How can we help?");

            LibraryManager libraryManager = new LibraryManager();

            while (true)
            {
                Console.WriteLine("MAIN MENU\n\r\nChoose an option:");
                Console.WriteLine("1. View book list");
                Console.WriteLine("2. Borrow a book");
                Console.WriteLine("3. Return the book");
                Console.WriteLine("4. List of readers with books they have borrowed");
                Console.WriteLine("5. Add new reader");
                Console.WriteLine("0. Close");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        libraryManager.DisplayBooks();
                        Console.WriteLine("\r\nPress the key to return to the main menu");
                        Console.ReadLine();
                        break;
                    case "2":
                        libraryManager.BorrowBook();
                        Console.WriteLine("\r\nPress the key to return to the main menu");
                        Console.ReadLine();
                        break;
                    case "3":
                        libraryManager.ReturnBook();
                        Console.WriteLine("\r\nPress the key to return to the main menu");
                        Console.ReadLine();
                        break;
                    case "4":
                        libraryManager.DisplayReadersWithBorrowedBooks();
                        Console.WriteLine("\r\nPress the key to return to the main menu");
                        Console.ReadLine();
                        break;
                    case "5":
                        libraryManager.AddReader();
                        Console.WriteLine("\r\nPress the key to return to the main menu");
                        Console.ReadLine();
                        break;
                    case "0":
                        Console.WriteLine("Thank you for using the app! Have a nice day!");
                        return;
                    default:
                        Console.WriteLine("Invalid operation. Try again");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}