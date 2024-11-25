namespace UKParliament.CodeTest.Data;

public class Department
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Person> People { get; set; } = [];
}
