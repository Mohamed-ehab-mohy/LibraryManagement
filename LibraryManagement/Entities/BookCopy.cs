using LibraryManagement.Contracts;
using LibraryManagement.Enums;
namespace LibraryManagement.Entities;

public sealed class BookCopy : IDisplayable, IBorrowable, IValidatable
{
    public string CopyId { get; private set; } = null!;
    public Book Book { get; private set; } = null!;
    public CopyStatus Status { get; private set; }
    public BookCondition Condition { get; private set; }
    public BorrowTransaction? ActiveBorrowTransaction { get; private set; }
    public string Title => Book.Title;
    public string AuthorName => Book.AuthorName;
    public string Category => Book.Category;
    public string ISBN => Book.ISBN;
    public int AvailableCopies => IsAvailable() ? 1 : 0;
    bool IBorrowable.IsAvailable => IsAvailable();

    public BookCopy(string copyId, Book book, BookCondition condition,
        CopyStatus status = CopyStatus.Available)
    {
        CopyId = copyId;
        Book = book;
        Condition = condition;
        Status = status;
    }

    public bool IsAvailable() => Status == CopyStatus.Available;

    public void Borrow(Member member, int loanDays = 14)
    {
        if (!IsAvailable())
            throw new InvalidOperationException(
                $"Copy {CopyId} is not available (Status: {Status}).");

        if (!member.CanBorrowMore())
            throw new InvalidOperationException(
                $"{member.Name} has reached the maximum of 3 active borrows.");

        if (member.HasActiveBorrow(CopyId))
            throw new InvalidOperationException(
                $"{member.Name} already has Copy {CopyId} on an active loan.");

        Status = CopyStatus.Borrowed;
        ActiveBorrowTransaction = new BorrowTransaction(member, this, loanDays);
        member.AddBorrowTransaction(ActiveBorrowTransaction);
    }

    public decimal Return()
    {
        if (Status == CopyStatus.Available)
            throw new InvalidOperationException(
                $"Copy {CopyId} is already available — it is not currently borrowed.");

        if (ActiveBorrowTransaction is null)
            throw new InvalidOperationException(
                $"No active transaction found for Copy {CopyId}. Please contact support.");

        LoyaltyTier tier = ActiveBorrowTransaction.Member.GetTier();
        decimal fine = ActiveBorrowTransaction.CompleteReturn(tier);

        Status = CopyStatus.Available;
        ActiveBorrowTransaction = null;

        return fine;
    }

    public void MarkDamaged() => Status = CopyStatus.Damaged;

    public bool IsValid(out string errorMessage)
    {
        errorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(CopyId)) { errorMessage = "Copy ID cannot be empty."; return false; }
        if (Book is null) { errorMessage = "BookCopy must reference a Book."; return false; }
        return true;
    }

    public string DisplayInformations()
        => $"  {CopyId,-14}{Book.Title,-35}{Condition}";
}