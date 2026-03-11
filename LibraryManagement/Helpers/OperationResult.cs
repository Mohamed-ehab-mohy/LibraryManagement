namespace LibraryManagement.Helpers;

public class OperationResult<T>
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = null!;
    public T? Data { get; private set; }

    private OperationResult() { }

    public static OperationResult<T> Success(T data, string message = "")
        => new() { IsSuccess = true, Data = data, Message = message };

    public static OperationResult<T> Failure(string message)
        => new() { IsSuccess = false, Data = default, Message = message };

    public override string ToString() => Message;
}
