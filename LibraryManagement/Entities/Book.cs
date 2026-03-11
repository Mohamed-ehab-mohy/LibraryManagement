using LibraryManagement.Contracts;
namespace LibraryManagement.Entities
{
    public class Book : IDisplayable
    {
        public string ISBN { get; private set; } = null!;

        public string Title { get; private set; } = null!;

        public string AuthorName { get; private set; } = null!;

        public string Category { get; private set; } = null!;

        public int PublicationYear { get; private set; }

        public Book(string ISBN, string title, string authorName, string category,
            int publicationYear)
        {
            this.ISBN = ISBN;
            Title = title;
            AuthorName = authorName;
            Category = category;
            PublicationYear = publicationYear;
        }
        public Book(string iSBN, string title)
            : this(iSBN, title, "unkown", "general", 0)
        {

        }
        public string DisplayInformations()
        => $"""
           ISBN : {ISBN}
           Title : {Title}
           Author : {AuthorName}
           Category : {Category}
           Publication Year : {PublicationYear}
       """;
    }
}
