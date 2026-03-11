namespace LibraryManagement.Entities
{
    public class Librarian : User
    {
        public string Id { get; private set; }
        public decimal Salary { get; private set; }
        public DateOnly HireDate { get; private set; }

        public Librarian(string id, string name, string phone, decimal salary, DateOnly hireDate)
            : base(name, phone)
        {
            Id = id;
            Salary = salary;
            HireDate = hireDate;
        }

        public Librarian(string id, string name, string phone, decimal salary)
            : this(id, name, phone, salary, DateOnly.FromDateTime(DateTime.Today)) { }

        public override string DisplayInformations()
            =>
                $"  ID      : {Id}\n" +
                $"  Name    : {Name}\n" +
                $"  Phone   : {Phone.MaskPhone()}\n" +
                $"  Salary  : {Salary:N2} EGP\n" +
                $"  Hired   : {HireDate:dd/MM/yyyy}";
    }
}
