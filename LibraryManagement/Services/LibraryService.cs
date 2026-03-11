using LibraryManagement.Entities;
namespace LibraryManagement.Services;

public class LibraryService
{
    private readonly LibraryBranch _branch;
    private readonly DisplayService _display;

    public LibraryService(LibraryBranch branch, DisplayService display)
    {
        _branch = branch;
        _display = display;
    }

    public void HandleRegisterMember()
    {
        Console.WriteLine("\n  " + new string('═', 47));
        Console.WriteLine("             REGISTER NEW MEMBER               ");
        Console.WriteLine("  " + new string('═', 47));

        string name = DisplayService.PromptString("Full Name");
        string phone = DisplayService.PromptString("Phone Number");
        string email = DisplayService.PromptString("Email (Enter to skip)");

        string? emailArg = string.IsNullOrWhiteSpace(email) ? null : email;

        Member member = _branch.RegisterMember(name, phone, emailArg);
        _display.ShowRegistrationSuccess(member);
    }

    public void HandleBorrow()
    {
        Console.WriteLine("\n  " + new string('═', 47));
        Console.WriteLine("               BORROW A BOOK                   ");
        Console.WriteLine("  " + new string('═', 47));

        string memberId = DisplayService.PromptString("Enter Member ID");
        if (string.IsNullOrWhiteSpace(memberId))
            throw new InvalidOperationException("Input cannot be empty.");

        Member member = _branch.FindMemberById(memberId);
        _display.ShowMemberBorrowInfo(member);
        _display.ShowAvailableCopies(_branch);

        string copyId = DisplayService.PromptString("Enter Copy ID to borrow");
        if (string.IsNullOrWhiteSpace(copyId))
            throw new InvalidOperationException("Input cannot be empty.");

        BookCopy copy = _branch.FindCopyById(copyId);
        copy.Borrow(member);

        _display.ShowBorrowSuccess(copy.ActiveBorrowTransaction!);
    }

    public void HandleReturn()
    {
        string copyId = DisplayService.PromptString("Enter Copy ID to return");
        if (string.IsNullOrWhiteSpace(copyId))
            throw new InvalidOperationException("Input cannot be empty.");

        BookCopy copy = _branch.FindCopyById(copyId);

        Member member = copy.ActiveBorrowTransaction?.Member
            ?? throw new InvalidOperationException(
                $"No active transaction found for Copy {copyId}. Please contact support.");

        int overdueDays = copy.ActiveBorrowTransaction!.DaysOverdue();
        decimal rawFine = overdueDays > 0 ? overdueDays * 10m : 0m;
        decimal discount = member.GetDiscount() * rawFine;

        copy.Return();

        var tx = member.GetBorrowHistory().Last();
        _display.ShowReturnSuccess(tx, rawFine, discount, member);
    }

    public void HandleHistory()
    {
        string memberId = DisplayService.PromptString("Enter Member ID");
        if (string.IsNullOrWhiteSpace(memberId))
            throw new InvalidOperationException("Input cannot be empty.");

        Member member = _branch.FindMemberById(memberId);
        _display.ShowMemberHistory(member);
    }

    public void HandleTopBorrowers()
        => _display.ShowTopBorrowers(_branch);

    public void HandleSearchMember()
    {
        Console.WriteLine("\n  " + new string('═', 47));
        Console.WriteLine("          SEARCH MEMBER BY NAME                ");
        Console.WriteLine("  " + new string('═', 47));

        string term = DisplayService.PromptString("Enter name to search");
        if (string.IsNullOrWhiteSpace(term))
            throw new InvalidOperationException("Input cannot be empty.");

        _display.ShowSearchResults(_branch.SearchMembersByName(term), term);
    }

    public void HandleInventory()
        => _display.ShowInventoryReport(_branch);

    public void HandleFineReport()
        => _display.ShowFineReport(_branch);

    public void HandleSearch()
    {
        Console.WriteLine("\n  " + new string('═', 47));
        Console.WriteLine("               SEARCH BOOKS & MEMBERS             ");
        Console.WriteLine("  " + new string('═', 47));
        Console.WriteLine("  1. Book Title");
        Console.WriteLine("  2. ISBN");
        Console.WriteLine("  3. Member Name");
        Console.WriteLine("  4. Author");
        Console.WriteLine("  5. Category");
        Console.WriteLine("  6. Back");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                SearchByTitle();
                break;

            case "2":
                SearchByISBN();
                break;

            case "3":
                SearchMember();
                break;

            case "4":
                SearchByAuthor();
                break;

            case "5":
                SearchByCategory();
                break;

            default:
                return;
        }
    }

    private void SearchByCategory()
    {
        string category = DisplayService.PromptString("Enter category");
        if (string.IsNullOrWhiteSpace(category))
            throw new InvalidOperationException("Input cannot be empty.");

        var books = _branch.BookCopies
            .Where(b => b.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!books.Any())
        {
            DisplayService.ShowInfo("No books found in this category.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine($"  Title: {book.Title}");
            Console.WriteLine($"  Author: {book.AuthorName}");
            Console.WriteLine($"  Category: {book.Category}");
            Console.WriteLine($"  Available Copies: {book.AvailableCopies}");
            Console.WriteLine("  ---------------------------");
        }
    }

    private void SearchByAuthor()
    {
        string author = DisplayService.PromptString("Enter author name");
        if (string.IsNullOrWhiteSpace(author))
            throw new InvalidOperationException("Input cannot be empty.");

        var books = _branch.BookCopies
            .Where(b => b.AuthorName.Contains(author, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!books.Any())
        {
            DisplayService.ShowInfo("No books found for this author.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine($"  Title: {book.Title}");
            Console.WriteLine($"  Author: {book.AuthorName}");
            Console.WriteLine($"  Category: {book.Category}");
            Console.WriteLine($"  Available Copies: {book.AvailableCopies}");
            Console.WriteLine("  ---------------------------");
        }
    }

    private void SearchMember()
    {
        string name = DisplayService.PromptString("Enter member name");
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Input cannot be empty.");

        var members = _branch.Members
            .Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _display.ShowSearchResults(members, name);
    }

    private void SearchByISBN()
    {
        string isbn = DisplayService.PromptString("Enter ISBN");
        if (string.IsNullOrWhiteSpace(isbn))
            throw new InvalidOperationException("Input cannot be empty.");

        var book = _branch.BookCopies.FirstOrDefault(b => b.ISBN == isbn.Trim());

        if (book == null)
        {
            DisplayService.ShowInfo("No book found with this ISBN.");
            return;
        }

        Console.WriteLine($"  {book.Title} - {book.AuthorName} - Available: {book.AvailableCopies}");
    }
    private void SearchByTitle()
    {
        string title = DisplayService.PromptString("Enter book title");
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Input cannot be empty.");

        var books = _branch.BookCopies
            .Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!books.Any())
        {
            DisplayService.ShowInfo("No books found.");
            return;
        }

        foreach (var book in books)
            Console.WriteLine($"  {book.Title} - {book.AuthorName} - Available: {book.AvailableCopies}");
    }
}