namespace LibraryManagement.Helpers
{
    internal static class ConsoleHelper
    {
        public static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("  " + new string('═', 47));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("       CITY LIBRARY SYSTEM — MENU             ");
            Console.ResetColor();
            Console.WriteLine("  " + new string('═', 47));
            Console.WriteLine("1. View Branch Information");
            Console.WriteLine("2. Register Member");
            Console.WriteLine("3. View Users");
            Console.WriteLine("4. View Books");
            Console.WriteLine("5. Borrow Book");
            Console.WriteLine("6. Return Book");
            Console.WriteLine("7. Borrow History");
            Console.WriteLine("8. Top Borrowers");
            Console.WriteLine("9. Overdue Alert");
            Console.WriteLine("10. Search");
            Console.WriteLine("11. Inventory");
            Console.WriteLine("12. Fines Report");
            Console.WriteLine("  " + new string('─', 47));
            Console.WriteLine("  0.  Exit");
            Console.WriteLine("  " + new string('═', 47));
            Console.Write("  Enter your choice: ");
        }
    }
}
