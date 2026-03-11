using LibraryManagement.Entities;
using LibraryManagement.Enums;
namespace LibraryManagement.Services;


public class DisplayService
{
    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine("  " + new string('═', 47));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {title.PadLeft((47 + title.Length) / 2).PadRight(47)}");
        Console.ResetColor();
        Console.WriteLine("  " + new string('═', 47));
    }

    private static void PrintDivider()
        => Console.WriteLine("  " + new string('─', 37));

    private static void PrintSectionTitle(string title)
    {
        PrintDivider();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {title}");
        Console.ResetColor();
        PrintDivider();
    }

    public static void ShowSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✔  {msg}");
        Console.ResetColor();
    }

    public static void ShowError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✘  {msg}");
        Console.ResetColor();
    }

    public static void ShowWarning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ⚠  {msg}");
        Console.ResetColor();
    }

    public static void ShowInfo(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  ℹ  {msg}");
        Console.ResetColor();
    }

    public static string PromptString(string label)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"  {label}: ");
        Console.ResetColor();
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public static void Pause()
    {
        Console.WriteLine("\n  Press Enter to continue...");
        Console.ReadLine();
    }

    public void ShowOverdueAlert(LibraryBranch branch)
    {
        var overdue = branch.GetOverdueTransactions();
        if (overdue.Count == 0) return;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ⚠  OVERDUE ALERT — {overdue.Count} copy/copies past due date:");
        foreach (var t in overdue)
            Console.WriteLine(
                $"     • {t.BookCopy.CopyId,-10} \"{t.BookTitle,-20}\"  →  " +
                $"{t.Member.Name,-18} ({t.DaysOverdue()} days overdue)");
        Console.ResetColor();
        Console.WriteLine();
    }

    public void ShowBranchInfo(LibraryBranch branch)
    {
        PrintHeader("BRANCH INFORMATION");
        Console.WriteLine(branch.DisplayInformations());
        Console.WriteLine("  " + new string('═', 47));
    }

    public void ShowAllUsers(LibraryBranch branch)
    {
        PrintHeader("ALL REGISTERED USERS");
        PrintSectionTitle("LIBRARIAN PROFILE");
        Console.WriteLine(branch.Manager.DisplayInformations());

        if (branch.Members.Count == 0)
        {
            ShowInfo("No members registered yet.");
            return;
        }

        foreach (var member in branch.Members)
        {
            PrintSectionTitle("MEMBER PROFILE");
            Console.WriteLine(member.DisplayInformations());
        }
    }

    public void ShowAvailableCopies(LibraryBranch branch)
    {
        PrintHeader("AVAILABLE BOOK COPIES");
        var available = branch.GetAvailableBookCopies();

        if (available.Count == 0)
        {
            ShowInfo("No copies are currently available for borrowing.");
            return;
        }

        Console.WriteLine($"  {"Copy ID",-14}{"Title",-35}Condition");
        Console.WriteLine("  " + new string('─', 55));
        foreach (var c in available)
            Console.WriteLine(c.DisplayInformations());
        Console.WriteLine("  " + new string('─', 55));
        Console.WriteLine($"  Total Available: {available.Count}");
    }
    public void ShowMemberBorrowInfo(Member member)
    {
        Console.WriteLine($"\n  Member found  : {member.Name}");
        Console.WriteLine($"  Active borrows: {member.ActiveBorrowCount} / 3");

        LoyaltyTier tier = member.GetTier();
        if (tier == LoyaltyTier.None) return;

        Console.ForegroundColor = tier switch
        {
            LoyaltyTier.Gold => ConsoleColor.Yellow,
            LoyaltyTier.Silver => ConsoleColor.Gray,
            _ => ConsoleColor.DarkYellow
        };

        string tierLabel = tier switch
        {
            LoyaltyTier.Gold => "🥇 Gold",
            LoyaltyTier.Silver => "🥈 Silver",
            _ => "🥉 Bronze"
        };

        Console.WriteLine($"  Loyalty Tier  : {tierLabel}  — {member.GetDiscount() * 100:0}% fine discount applied on late returns");
        Console.ResetColor();
    }

    public void ShowBorrowSuccess(BorrowTransaction tx)
    {
        ShowSuccess($"'{tx.BookTitle}' borrowed by {tx.Member.Name}. Due: {tx.DueDate:dd/MM/yyyy}");
        Console.WriteLine();
        Console.WriteLine(tx.DisplayInformations());
    }


    public void ShowReturnSuccess(BorrowTransaction tx, decimal rawFine, decimal discount, Member member)
    {
        ShowSuccess($"Copy [{tx.BookCopy.CopyId}] \"{tx.BookTitle}\" returned by {member.Name}.");

        if (tx.Fine > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠  Late return: {tx.DaysOverdue()} days overdue.");
            Console.WriteLine($"     Original fine : {rawFine:N2} EGP");

            if (discount > 0)
                Console.WriteLine(
                    $"     {member.GetTier()} discount  : -{discount:N2} EGP ({member.GetDiscount() * 100:0}%)");

            Console.WriteLine($"     Final fine     : {tx.Fine:N2} EGP");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine(tx.DisplayInformations());
    }

    public void ShowMemberHistory(Member member)
    {
        var history = member.GetBorrowHistory();

        if (history.Count == 0)
        {
            ShowInfo($"{member.Name} has no borrowing history yet.");
            return;
        }

        Console.WriteLine($"\n  Borrowing History for {member.Name} — {history.Count} record(s)");
        Console.WriteLine();

        foreach (var tx in history)
        {
            foreach (string ln in tx.DisplayInformations().Split('\n'))
            {
                if (ln.Contains("Active"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ln);
                    Console.ResetColor();
                }
                else
                    Console.WriteLine(ln);
            }
        }
    }

    public void ShowRegistrationSuccess(Member member)
    {
        ShowSuccess($"Member '{member.Name}' registered successfully. ID: {member.Id}");
        Console.WriteLine();
        PrintSectionTitle("MEMBER PROFILE");
        Console.WriteLine(member.DisplayInformations());
        PrintDivider();
    }


    public void ShowTopBorrowers(LibraryBranch branch)
    {
        PrintHeader("TOP BORROWERS — LOYALTY REPORT");
        var members = branch.GetTopBorrowers();

        if (members.Count == 0)
        {
            ShowInfo("No members registered yet.");
            return;
        }

        Console.WriteLine($"  {"Rank",-6}{"Name",-20}{"Borrows",-10}Tier");
        Console.WriteLine("  " + new string('─', 42));

        for (int i = 0; i < members.Count; i++)
        {
            var m = members[i];
            var tier = m.GetTier();

            string tierLabel = tier switch
            {
                LoyaltyTier.Gold => "🥇 Gold",
                LoyaltyTier.Silver => "🥈 Silver",
                LoyaltyTier.Bronze => "🥉 Bronze",
                _ => "None"
            };

            Console.ForegroundColor = tier switch
            {
                LoyaltyTier.Gold => ConsoleColor.Yellow,
                LoyaltyTier.Silver => ConsoleColor.Gray,
                LoyaltyTier.Bronze => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White
            };

            Console.WriteLine($"  #{i + 1,-5}{m.Name,-20}{m.BorrowCount,-10}{tierLabel}");
            Console.ResetColor();
        }

        Console.WriteLine("  " + new string('─', 42));
        Console.WriteLine($"  Total Members  : {members.Count}");
        Console.WriteLine($"  Gold Members   : {members.Count(m => m.GetTier() == LoyaltyTier.Gold)}");
        Console.WriteLine($"  Silver Members : {members.Count(m => m.GetTier() == LoyaltyTier.Silver)}");
        Console.WriteLine($"  Bronze Members : {members.Count(m => m.GetTier() == LoyaltyTier.Bronze)}");
    }


    public void ShowSearchResults(List<Member> results, string term)
    {
        if (results.Count == 0)
        {
            ShowInfo($"No members found matching \"{term}\".");
            return;
        }

        Console.WriteLine($"\n  {results.Count} result(s) found for \"{term}\":");
        foreach (var m in results)
        {
            PrintSectionTitle("MEMBER PROFILE");
            Console.WriteLine(m.DisplayInformations());
        }
        PrintDivider();
    }


    public void ShowInventoryReport(LibraryBranch branch)
    {
        PrintHeader("INVENTORY — COPY STATUS REPORT");
        var rows = branch.GetInventoryReport();

        Console.WriteLine($"  {"Title",-32}{"Available",-11}{"Borrowed",-10}{"Damaged",-9}Total");
        Console.WriteLine("  " + new string('─', 63));

        int totA = 0, totB = 0, totD = 0;
        foreach (var (title, avail, borrowed, damaged, total) in rows)
        {
            Console.WriteLine($"  {title,-32}{avail,-11}{borrowed,-10}{damaged,-9}{total}");
            totA += avail; totB += borrowed; totD += damaged;
        }

        Console.WriteLine("  " + new string('─', 63));
        Console.Write($"  {"TOTALS",-32}");
        Console.ForegroundColor = ConsoleColor.Green; Console.Write($"{totA,-11}");
        Console.ForegroundColor = ConsoleColor.Yellow; Console.Write($"{totB,-10}");
        Console.ForegroundColor = ConsoleColor.Red; Console.Write($"{totD,-9}");
        Console.ResetColor();
        Console.WriteLine(totA + totB + totD);
        Console.WriteLine("  " + new string('═', 63));
    }


    public void ShowFineReport(LibraryBranch branch)
    {
        PrintHeader("FINE SUMMARY — OVERDUE REPORT");
        var fines = branch.GetFineReport();

        if (fines.Count == 0)
        {
            ShowInfo("No outstanding fines at this time.");
            return;
        }

        Console.WriteLine($"  {"Rank",-6}{"Member",-16}{"TX #",-8}{"Book",-18}Fine");
        Console.WriteLine("  " + new string('─', 55));

        decimal total = 0m;
        for (int i = 0; i < fines.Count; i++)
        {
            var t = fines[i];
            Console.ForegroundColor = ConsoleColor.Red;   
            Console.WriteLine(
                $"  #{i + 1,-5}{t.Member.Name,-16}#{t.TransactionId,-7}{t.BookTitle,-18}{t.Fine:N2} EGP");
            Console.ResetColor();
            total += t.Fine;
        }

        Console.WriteLine("  " + new string('─', 55));

        Console.Write("  Total outstanding fines: ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{total:N2} EGP");
        Console.ResetColor();

        Console.WriteLine($"  Members with fines     : {fines.Select(t => t.Member.Id).Distinct().Count()}");
    }
}