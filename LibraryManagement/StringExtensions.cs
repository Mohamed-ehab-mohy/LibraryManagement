namespace LibraryManagement;

public static class StringExtensions
{
    public static string NormalizeId(this string id)
        => id?.Trim().ToUpperInvariant()!;

    public static string MaskPhone(this string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 8)
            return phone;
        return phone[..4] + "****" + phone[8..];
    }

    public static bool IsValidEgyptianPhone(this string phone, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(phone))
        {
            error = "Input cannot be empty.";
            return false;
        }

        if (phone.Length != 11 || !phone.All(char.IsDigit))
        {
            error = "Invalid phone number. Must be 11 digits starting with 010, 011, 012, or 015.";
            return false;
        }

        string prefix = phone[..3];
        if (prefix != "010" && prefix != "011" && prefix != "012" && prefix != "015")
        {
            error = "Invalid phone number. Must be 11 digits starting with 010, 011, 012, or 015.";
            return false;
        }

        return true;
    }

    public static bool IsValidName(this string name, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Input cannot be empty.";
            return false;
        }

        string trimmed = name.Trim();

        if (trimmed.Length < 3 || trimmed.Length > 50)
        {
            error = "Invalid name. Name must contain letters and spaces only, between 3 and 50 characters.";
            return false;
        }

        foreach (char c in trimmed)
        {
            if (!char.IsLetter(c) && c != ' ')
            {
                error = "Invalid name. Name must contain letters and spaces only, between 3 and 50 characters.";
                return false;
            }
        }

        return true;
    }
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;

        var words = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
            words[i] = char.ToUpper(words[i][0]) + words[i][1..].ToLower();

        return string.Join(' ', words);
    }

    public static bool IsValidEmail(this string value, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrEmpty(value))
            return true;

        int atIndex = value.IndexOf('@');

        if (atIndex <= 0)
        {
            error = "Invalid email format. Example: user@example.com";
            return false;
        }

        string afterAt = value[(atIndex + 1)..];
        int dotIndex = afterAt.LastIndexOf('.');

        if (dotIndex < 0 || dotIndex == afterAt.Length - 1)
        {
            error = "Invalid email format. Example: user@example.com";
            return false;
        }

        return true;
    }

    public static bool IsValidMemberIdFormat(this string id, out string error)
    {
        error = string.Empty;
        string norm = id?.Trim().ToUpperInvariant() ?? "";

        if (norm.Length != 7 || !norm.StartsWith("MEM-") || !norm[4..].All(char.IsDigit))
        {
            error = "Invalid Member ID format. Expected format: MEM-001";
            return false;
        }

        return true;
    }

    public static bool IsValidCopyIdFormat(this string id, out string error)
    {
        error = string.Empty;
        string norm = id?.Trim().ToUpperInvariant() ?? "";

        if (norm.Length < 6 || !norm.StartsWith("COPY-"))
        {
            error = "Invalid Copy ID format. Expected format: COPY-001";
            return false;
        }

        return true;
    }

    public static bool ContainDigit(this string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        foreach (char c in value) if (char.IsDigit(c)) return true;
        return false;
    }
}