using LibraryManagement.Entities;

namespace LibraryManagement.Contracts;

public interface IBorrowable
{
    bool IsAvailable { get; }
    
        void Borrow(Member member,int loandDays = 14 );
    
        decimal Return();
}
