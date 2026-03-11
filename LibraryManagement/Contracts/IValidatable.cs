namespace LibraryManagement.Contracts
{
    public interface IValidatable
    {
        bool IsValid(out string errorMessage);
    }
}
