using LibraryManagement.Contracts;
using LibraryManagement.Enums;
namespace LibraryManagement.Entities;

public class LibraryBranch : IDisplayable
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Phone { get; private set; }
    public string OpeningHours { get; private set; }
    public Librarian Manager { get; private set; }

    private readonly List<BookCopy> _copies = [];
    private readonly List<Member> _members = [];

    public IReadOnlyList<Member> Members => _members;
    public IReadOnlyList<BookCopy> BookCopies => _copies;
    public int TotalMembers => _members.Count;
    public int TotalBookCopies => _copies.Count;

    public IReadOnlyList<User> Users
    {
        get { List<User> u = [Manager]; u.AddRange(_members); return u; }
    }

    public LibraryBranch(string id, string name, string address,
        string phone, string openingHours, Librarian manager)
    {
        Id = id;
        Name = name;
        Address = address;
        Phone = phone;
        OpeningHours = openingHours;
        Manager = manager;
    }

    public Member RegisterMember(string name, string phone, string? email = null)
    {
        if (!name.IsValidName(out string nameErr))
            throw new InvalidOperationException(nameErr);

        if (!phone.IsValidEgyptianPhone(out string phoneErr))
            throw new InvalidOperationException(phoneErr);

        foreach (var m in _members)
            if (m.Phone.Equals(phone, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"A member with phone {phone} is already registered.");

        if (!string.IsNullOrEmpty(email) && !email.IsValidEmail(out string emailErr))
            throw new InvalidOperationException(emailErr);

        var member = new Member(name.ToTitleCase(), phone, email);
        _members.Add(member);
        return member;
    }

    public void SeedMember(Member member) => _members.Add(member);

    public Member FindMemberById(string id)
    {
        if (!id.IsValidMemberIdFormat(out string fmtErr))
            throw new InvalidOperationException(fmtErr);

        string norm = id.NormalizeId();
        foreach (var m in _members)
            if (m.Id.Equals(norm, StringComparison.OrdinalIgnoreCase))
                return m;

        throw new InvalidOperationException($"Member '{norm}' not found.");
    }

    public BookCopy FindCopyById(string id)
    {
        if (!id.IsValidCopyIdFormat(out string fmtErr))
            throw new InvalidOperationException(fmtErr);

        string norm = id.NormalizeId();
        foreach (var c in _copies)
            if (c.CopyId.Equals(norm, StringComparison.OrdinalIgnoreCase))
                return c;

        throw new InvalidOperationException($"Copy '{norm}' not found.");
    }

    public void AddBookCopy(BookCopy copy) => _copies.Add(copy);

    public List<BookCopy> GetAvailableBookCopies()
        => _copies.Where(c => c.IsAvailable()).ToList();

    public List<Member> GetTopBorrowers()
        => _members.OrderByDescending(m => m.BorrowCount).ThenBy(m => m.Name).ToList();

    public List<BorrowTransaction> GetOverdueTransactions()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        return _members
            .SelectMany(m => m.BorrowTransactions)
            .Where(t => t.IsActive() && t.DueDate < today)
            .ToList();
    }

    public List<Member> SearchMembersByName(string term)
        => _members
            .Where(m => m.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public List<(string Title, int Available, int Borrowed, int Damaged, int Total)> GetInventoryReport()
        => _copies
            .GroupBy(c => c.Book.Title)
            .Select(g => (
                Title: g.Key,
                Available: g.Count(c => c.Status == CopyStatus.Available),
                Borrowed: g.Count(c => c.Status == CopyStatus.Borrowed),
                Damaged: g.Count(c => c.Status == CopyStatus.Damaged),
                Total: g.Count()
            ))
            .ToList();

    public List<BorrowTransaction> GetFineReport()
        => _members
            .SelectMany(m => m.BorrowTransactions)
            .Where(t => t.Fine > 0)
            .OrderByDescending(t => t.Fine)
            .ThenBy(t => t.Member.Name)  
            .ToList();

    public string DisplayInformations()
        =>
            $"  Branch ID     : {Id}\n" +
            $"  Name          : {Name}\n" +
            $"  Address       : {Address}\n" +
            $"  Phone         : {Phone}\n" +
            $"  Opening Hours : {OpeningHours}\n" +
            $"  Manager       : {Manager.Name}\n" +
            $"  ───────────────────────────────────────────────\n" +
            $"  Total Members     : {TotalMembers}\n" +
            $"  Total Book Copies : {TotalBookCopies}";
}