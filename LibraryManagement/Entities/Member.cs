using LibraryManagement.Contracts;
using LibraryManagement.Enums;
namespace LibraryManagement.Entities;

public class Member : User, IValidatable
{
    private static int _counter = 1;
    private readonly List<BorrowTransaction> _borrowTransactions = [];

    public string Id { get; private set; } = null!;
    public string? Email { get; private set; }
    public DateOnly MembershipDate { get; private set; }

    public int BorrowCount => _borrowTransactions.Count;
    public int ActiveBorrowCount => _borrowTransactions.Count(t => t.IsActive());
    public IReadOnlyList<BorrowTransaction> BorrowedBooks => _borrowTransactions.Where(t => t.IsActive()).ToList();

    public IReadOnlyList<BorrowTransaction> BorrowTransactions => _borrowTransactions;

    public Member(string name, string phone, string? email, DateOnly membershipDate)
        : base(name, phone)
    {
        Id = $"MEM-{_counter++:D3}";
        Email = string.IsNullOrWhiteSpace(email) ? null : email;
        MembershipDate = membershipDate;
    }

    public Member(string name, string phone, string? email)
        : this(name, phone, email, DateOnly.FromDateTime(DateTime.Today)) { }

    public Member(string name, string phone)
        : this(name, phone, null, DateOnly.FromDateTime(DateTime.Today)) { }

    public LoyaltyTier GetTier() => BorrowCount switch
    {
        >= 10 => LoyaltyTier.Gold,
        >= 5 => LoyaltyTier.Silver,
        >= 3 => LoyaltyTier.Bronze,
        _ => LoyaltyTier.None
    };

    public decimal GetDiscount() => GetTier() switch
    {
        LoyaltyTier.Gold => 0.20m,
        LoyaltyTier.Silver => 0.10m,
        LoyaltyTier.Bronze => 0.05m,
        _ => 0m
    };

    public bool CanBorrowMore() => ActiveBorrowCount < 3;

    public bool HasActiveBorrow(string copyId)
        => _borrowTransactions.Any(t => t.IsActive() &&
               t.BookCopy.CopyId.Equals(copyId, StringComparison.OrdinalIgnoreCase));

    public void AddBorrowTransaction(BorrowTransaction transaction)
        => _borrowTransactions.Add(transaction);

    public IReadOnlyList<BorrowTransaction> GetBorrowHistory()
        => _borrowTransactions.OrderBy(t => t.BorrowDate).ToList();

    public bool IsValid(out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!Name.IsValidName(out string nameErr)) { errorMessage = nameErr; return false; }
        if (!Phone.IsValidEgyptianPhone(out string phoneErr)) { errorMessage = phoneErr; return false; }
        if (Email != null && !Email.IsValidEmail(out string emailErr)) { errorMessage = emailErr; return false; }
        return true;
    }

    public override string DisplayInformations()
    {
        string tier = GetTier() switch
        {
            LoyaltyTier.Gold => "🥇 Gold",
            LoyaltyTier.Silver => "🥈 Silver",
            LoyaltyTier.Bronze => "🥉 Bronze",
            _ => "None"
        };

        return
            $"  ID      : {Id}\n" +
            $"  Name    : {Name}\n" +
            $"  Phone   : {Phone.MaskPhone()}\n" +
            $"  Email   : {Email ?? "N/A"}\n" +
            $"  Joined  : {MembershipDate:dd/MM/yyyy}\n" +
            $"  Borrows : {BorrowCount}\n" +
            $"  Tier    : {tier}";
    }
}