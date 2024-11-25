using Microsoft.EntityFrameworkCore;

namespace UKParliament.CodeTest.Data;

public class PersonManagerContext : DbContext
{

    private readonly bool _shouldSeedData;

    public PersonManagerContext(DbContextOptions<PersonManagerContext> options, bool shouldSeedData = true) : base(options)
    {
        // for unit testing
        People ??= Set<Person>(); 
        Departments ??= Set<Department>();
        _shouldSeedData = shouldSeedData; 
       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired();
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).IsRequired();
            entity.Property(p => p.LastName).IsRequired();

            entity.HasOne(p => p.Department)
                  .WithMany(d => d.People)
                  .HasForeignKey(p => p.DepartmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Sales" },
            new Department { Id = 2, Name = "Marketing" },
            new Department { Id = 3, Name = "Finance" },
            new Department { Id = 4, Name = "HR" });

        if (_shouldSeedData)
        {
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, FirstName = "Fred", LastName = "Bloggs", DOB = DateOnly.Parse("20/01/2000"), Title = "Mr", DepartmentId = 1, AddressLine1 = "1 Acacia Avenue", PostCode = "AC1 111" },
                new Person { Id = 2, FirstName = "John", LastName = "Smith", DOB = DateOnly.Parse("25/02/2000"), Title = "Mr", DepartmentId = 2, AddressLine1 = "2 Acacia Avenue", PostCode = "AC1 222" },
                new Person { Id = 3, FirstName = "Sophie", LastName = "Smith", DOB = DateOnly.Parse("10/03/2000"), Title = "Ms", DepartmentId = 3, AddressLine1 = "3 Acacia Avenue", PostCode = "AC1 333" },
                new Person { Id = 4, FirstName = "Matthew", LastName = "Bloggs", DOB = DateOnly.Parse("07/04/2000"), Title = "Mr", DepartmentId = 4, AddressLine1 = "4 Acacia Avenue", PostCode = "AC1 444" },
                new Person { Id = 5, FirstName = "Jenny", LastName = "Smith", DOB = DateOnly.Parse("28/05/2000"), Title = "Mrs", DepartmentId = 1, AddressLine1 = "5 Acacia Avenue", PostCode = "AC1 555" },
                new Person { Id = 6, FirstName = "Peter", LastName = "Bloggs", DOB = DateOnly.Parse("15/06/2000"), Title = "Mr", DepartmentId = 2, AddressLine1 = "6 Acacia Avenue", PostCode = "AC1 666" },
                new Person { Id = 7, FirstName = "Ben", LastName = "Smith", DOB = DateOnly.Parse("14/07/2000"), Title = "Mr", DepartmentId = 3, AddressLine1 = "7 Acacia Avenue", PostCode = "AC1 777" },
                new Person { Id = 8, FirstName = "Robert", LastName = "Bloggs", DOB = DateOnly.Parse("13/08/2000"), Title = "Mr", DepartmentId = 4, AddressLine1 = "8 Acacia Avenue", PostCode = "AC1 888" },
                new Person { Id = 9, FirstName = "Max", LastName = "Smith", DOB = DateOnly.Parse("12/09/2000"), Title = "Mr", DepartmentId = 1, AddressLine1 = "9 Acacia Avenue", PostCode = "AC1 999" },
                new Person { Id = 10, FirstName = "Mark", LastName = "Bloggs", DOB = DateOnly.Parse("02/10/2000"), Title = "Mr", DepartmentId = 2, AddressLine1 = "10 Acacia Avenue", PostCode = "AC1 101" },
                new Person { Id = 11, FirstName = "George", LastName = "Smith", DOB = DateOnly.Parse("04/11/2000"), Title = "Mr", DepartmentId = 3, AddressLine1 = "11 Acacia Avenue", PostCode = "AC1 101" },
                new Person { Id = 12, FirstName = "Emma", LastName = "Bloggs", DOB = DateOnly.Parse("30/12/2000"), Title = "Miss", DepartmentId = 4, AddressLine1 = "12 Acacia Avenue", PostCode = "AC1 202" }
                );
        }
    }

    public virtual  DbSet<Person> People { get; set; }

    public virtual  DbSet<Department> Departments { get; set; }
}