using LibraryManagement.Contracts;
using LibraryManagement.Enums;
using TransactionStatus = LibraryManagement.Enums.TransactionStatus;
namespace LibraryManagement.Entities;

public sealed class BorrowTransaction : IDisplayable, IValidatable
{
    private static int _counter = 1000;
    private const decimal FinePerDay = 10m;
    public int TransactionId { get; private set; }
    public Member Member { get; private set; } = null!;
    public BookCopy BookCopy { get; private set; } = null!;
    public string BookTitle { get; private set; } = null!;
    public DateOnly BorrowDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }
    public decimal Fine { get; private set; }
    public decimal DiscountApplied { get; private set; }
    public TransactionStatus Status { get; private set; } = TransactionStatus.Active;

    public BorrowTransaction(Member member, BookCopy bookCopy, int loanDays = 14)
    {
        TransactionId = ++_counter;
        Member = member;
        BookCopy = bookCopy;
        BookTitle = bookCopy.Book.Title;
        BorrowDate = DateOnly.FromDateTime(DateTime.Today);
        DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(loanDays));
        Status = TransactionStatus.Active;
    }

    public bool IsActive() => Status == TransactionStatus.Active;
    public bool IsReturned() => ReturnDate.HasValue;

    public int DaysOverdue()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int days = today.DayNumber - DueDate.DayNumber;
        return days > 0 ? days : 0;
    }

    public decimal CompleteReturn(LoyaltyTier tier)
    {
        ReturnDate = DateOnly.FromDateTime(DateTime.Today);

        int overdueDays = ReturnDate.Value.DayNumber - DueDate.DayNumber;

        if (overdueDays > 0)
        {
            decimal rawFine = overdueDays * FinePerDay;
            decimal discount = tier switch
            {
                LoyaltyTier.Gold => rawFine * 0.20m,
                LoyaltyTier.Silver => rawFine * 0.10m,
                LoyaltyTier.Bronze => rawFine * 0.05m,
                _ => 0m
            };

            DiscountApplied = Math.Round(discount, 2);
            Fine = Math.Round(rawFine - DiscountApplied, 2);
            Status = TransactionStatus.Overdue;
        }
        else
        {
            Fine = 0m;
            DiscountApplied = 0m;
            Status = TransactionStatus.Returned;
        }

        return Fine;
    }

    public bool IsValid(out string errorMessage)
    {
        errorMessage = string.Empty;
        if (Member is null) { errorMessage = "Transaction must have a member."; return false; }
        if (BookCopy is null) { errorMessage = "Transaction must have a book copy."; return false; }
        return true;
    }

    public string DisplayInformations()
    {
        string returned = ReturnDate?.ToString("dd/MM/yyyy") ?? "Not returned yet";
        string statusText = Status.ToString();

        string fineText;
        if (Fine == 0)
            fineText = "None";
        else if (DiscountApplied > 0)
            fineText = $"{Fine:N2} EGP (after {Member.GetTier()} discount)";
        else
            fineText = $"{Fine:N2} EGP";

        return
            $"  ── Transaction #{TransactionId} ──────────────────────\n" +
            $"  Book      : {BookTitle}\n" +
            $"  Copy ID   : {BookCopy.CopyId}\n" +
            $"  Member    : {Member.Name}\n" +           
            $"  Borrowed  : {BorrowDate:dd/MM/yyyy}\n" +
            $"  Due       : {DueDate:dd/MM/yyyy}\n" +
            $"  Returned  : {returned}\n" +
            $"  Status    : {statusText}\n" +
            $"  Fine      : {fineText}\n";
    }
}