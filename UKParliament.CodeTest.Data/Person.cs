namespace UKParliament.CodeTest.Data;

public class Person
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly DOB { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? AddressLine4 { get; set; }
    public string? PostCode { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}