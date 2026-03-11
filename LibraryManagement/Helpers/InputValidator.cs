namespace LibraryManagement.Helpers;

public static class InputValidator
{
    public static bool IsValidName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string trimmed = value.Trim();

        if (trimmed.Length < 3 || trimmed.Length > 50) return false;

        foreach (char c in trimmed)
        {
            if (!char.IsLetter(c) && c != ' ')
                return false;
        }

        return true;
    }

    public static bool IsValidPhone(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string trimmed = value.Trim();

        if (trimmed.Length != 11) return false;

        foreach (char c in trimmed)
        {
            if (!char.IsDigit(c)) return false;
        }

        return trimmed.StartsWith("010")
            || trimmed.StartsWith("011")
            || trimmed.StartsWith("012")
            || trimmed.StartsWith("015");
    }

    public static bool IsValidEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string trimmed = value.Trim();

        int atIndex = trimmed.IndexOf('@');

        if (atIndex <= 0) return false;

        int dotIndex = trimmed.IndexOf('.', atIndex);

        if (dotIndex < 0 || dotIndex == trimmed.Length - 1) return false;

        return true;
    }

    public static bool IsValidMemberID(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string trimmed = value.Trim().ToUpperInvariant();

        if (trimmed.Length != 7) return false;

        if (!trimmed.StartsWith("MEM-")) return false;

        string digits = trimmed.Substring(4);

        foreach (char c in digits)
        {
            if (!char.IsDigit(c)) return false;
        }

        return true;
    }

    public static bool IsValidCopyID(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        string trimmed = value.Trim().ToUpperInvariant();

        return trimmed.Length >= 6 && trimmed.StartsWith("COPY-");
    }
}

