using LibraryManagement.Entities;
using LibraryManagement.Enums;
namespace LibraryManagement.DataSeeder;

internal static class Seeder
{
    public static LibraryBranch Seed()
    {
        var manager = new Librarian(
            id: "LIB-01",
            name: "Sara Ahmed",
            phone: "01012345678",
            salary: 8500m,
            hireDate: new DateOnly(2019, 3, 15));

        var branch = new LibraryBranch(
            id: "BR-01",
            name: "City Library — Nasr City Branch",
            address: "15 Nasr Road, Nasr City, Cairo",
            phone: "01012345678",
            openingHours: "Sat–Thu: 09:00 AM – 09:00 PM",
            manager: manager);

        // Members — seeded directly to bypass business-rule validation
        branch.SeedMember(new Member("Ahmed Kamal", "01098765432", "ahmed@email.com", new DateOnly(2023, 1, 20)));
        branch.SeedMember(new Member("Nour Hassan", "01155556677", "nour@email.com", new DateOnly(2024, 3, 5)));
        branch.SeedMember(new Member("Omar Fathy", "01234567890", null, new DateOnly(2024, 6, 1)));

        // Books
        var cleanCode = new Book("978-0-13-468599-1", "Clean Code", "Robert C. Martin", "Software Engineering", 2008);
        var pragmatic = new Book("978-0-13-235088-4", "The Pragmatic Programmer", "David Thomas", "Software Engineering", 1999);
        var designPatt = new Book("978-0-20-163361-5", "Design Patterns", "Gang of Four", "Software Engineering", 1994);
        var atomicHabits = new Book("978-0-73-521370-8", "Atomic Habits", "James Clear", "Self-Help", 2018);
        var sapiens = new Book("978-0-06-231609-7", "Sapiens", "Yuval Noah Harari", "History", 2011);

        // 8 Book Copies (matching spec)
        branch.AddBookCopy(new BookCopy("COPY-001", cleanCode, BookCondition.Good));
        branch.AddBookCopy(new BookCopy("COPY-002", cleanCode, BookCondition.New));
        branch.AddBookCopy(new BookCopy("COPY-003", pragmatic, BookCondition.Good));
        branch.AddBookCopy(new BookCopy("COPY-004", pragmatic, BookCondition.Good));
        branch.AddBookCopy(new BookCopy("COPY-005", designPatt, BookCondition.Worn, CopyStatus.Damaged));
        branch.AddBookCopy(new BookCopy("COPY-006", atomicHabits, BookCondition.New));
        branch.AddBookCopy(new BookCopy("COPY-007", atomicHabits, BookCondition.Good));
        branch.AddBookCopy(new BookCopy("COPY-008", sapiens, BookCondition.New));

        return branch;
    }
}