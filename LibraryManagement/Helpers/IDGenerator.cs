namespace LibraryManagement.Helpers;

public static class IDGenerator
{
    private static int _memberCounter = 1;
    private static int _transactionCounter = 1000;

    public static string GenerateMemberID()
        => $"MEM-{_memberCounter++:D3}";

    public static int GenerateTransactionID()
        => ++_transactionCounter;

    public static void SetMemberCounter(int value)
        => _memberCounter = value;

    public static void SetTransactionCounter(int value)
        => _transactionCounter = value;
}
