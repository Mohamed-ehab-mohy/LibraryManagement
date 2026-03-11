using LibraryManagement.DataSeeder;
using LibraryManagement.Helpers;
using LibraryManagement.Services;

namespace LibraryManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var branch = Seeder.Seed();
            var display = new DisplayService();
            var service = new LibraryService(branch, display);

            bool running = true;

            while (running)
            {
                Console.Clear();


                display.ShowOverdueAlert(branch);
                ConsoleHelper.ShowMenu();

                string choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1": display.ShowBranchInfo(branch); break;
                        case "2": service.HandleRegisterMember(); break;
                        case "3": display.ShowAllUsers(branch); break;
                        case "4": display.ShowAvailableCopies(branch); break;
                        case "5": service.HandleBorrow(); break;
                        case "6": service.HandleReturn(); break;
                        case "7": service.HandleHistory(); break;
                        case "8": service.HandleTopBorrowers(); break;
                        case "9": display.ShowOverdueAlert(branch); break;
                        case "10": service.HandleSearch(); break;
                        case "11": service.HandleInventory(); break;
                        case "12": service.HandleFineReport(); break;

                        case "0":
                            Console.WriteLine("  Goodbye!");
                            running = false;
                            break;

                        default:
                            DisplayService.ShowError("Invalid option. Please enter a number from 0 to 12.");
                            break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    DisplayService.ShowError(ex.Message);
                }

                if (running)
                    DisplayService.Pause();
            }
        }
    }
}
