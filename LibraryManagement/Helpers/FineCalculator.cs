using LibraryManagement.Enums;

namespace LibraryManagement.Helpers;

public static class FineCalculator
{
    public const decimal FinePerDay = 10m;
    public const int LoanDays = 14;

    public static int DaysLate(DateOnly dueDate, DateOnly returnDate)
    {
        int days = returnDate.DayNumber - dueDate.DayNumber;
        return days > 0 ? days : 0;
    }

    public static decimal Calculate(DateOnly dueDate, DateOnly returnDate)
        => DaysLate(dueDate, returnDate) * FinePerDay;

    public static bool IsOverdue(DateOnly dueDate)
        => DateOnly.FromDateTime(DateTime.Today) > dueDate;

    public static decimal GetDiscount(decimal fine, LoyaltyTier tier)
        => tier switch
        {
            LoyaltyTier.Gold => Math.Round(fine * 0.20m, 2),
            LoyaltyTier.Silver => Math.Round(fine * 0.10m, 2),
            LoyaltyTier.Bronze => Math.Round(fine * 0.05m, 2),
            _ => 0m
        };

    public static decimal ApplyDiscount(decimal fine, LoyaltyTier tier)
        => fine - GetDiscount(fine, tier);

    public static string GetTierLabel(LoyaltyTier tier)
        => tier switch
        {
            LoyaltyTier.Gold => "Gold",
            LoyaltyTier.Silver => "Silver",
            LoyaltyTier.Bronze => "Bronze",
            _ => "None"
        };

    public static int GetDiscountPercent(LoyaltyTier tier)
        => tier switch
        {
            LoyaltyTier.Gold => 20,
            LoyaltyTier.Silver => 10,
            LoyaltyTier.Bronze => 5,
            _ => 0
        };
}