
using Microsoft.EntityFrameworkCore;
using Moq;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Data.Repositories;
using Xunit;

namespace UKParliament.CodeTest.Tests;
public class PersonRepositoryTests
{
    private readonly DbContextOptions<PersonManagerContext> _options;
    private readonly PersonManagerContext _context;
    private readonly PersonRepository _repository;

    public PersonRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<PersonManagerContext>()
            .UseInMemoryDatabase(databaseName: "PersonManagerTestDb")
            .Options;

        _context = new PersonManagerContext(_options, false);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _repository = new PersonRepository(_context);
    }


    [Fact]
    public async Task AddPersonAsync_ShouldAddPersonToContext()
    {
        var person = new Person { FirstName = "Dom", LastName = "Cohen", DOB = new DateOnly(1930, 7, 26), DepartmentId = 1 };

        var key = await _repository.AddPersonAsync(person);
        await _context.SaveChangesAsync();

        var addedPerson = await _context.People.FindAsync(key);

        Assert.NotNull(addedPerson);
        Assert.Equal("Dom", addedPerson.FirstName);
        Assert.Equal("Cohen", addedPerson.LastName);
    }


    [Fact]
    public async Task GetPersonByIdAsync_ShouldReturnCorrectPerson()
    {
        var person = new Person
        {
            FirstName = "FirstName",
            LastName = "LastName",
            DOB = new DateOnly(1990, 1, 1),
            DepartmentId = 1
        };

        var key = await _repository.AddPersonAsync(person);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPersonByIdAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
        Assert.Equal(person.FirstName, result.FirstName);
        Assert.Equal(person.LastName, result.LastName);
        Assert.Equal(person.DOB, result.DOB);
        Assert.Equal(person.DepartmentId, result.DepartmentId);
    }


    [Fact]
    public async Task GetAllPeopleAsync_ShouldReturnAllPeople()
    {
        var people = new List<Person>
        {
            new Person { FirstName = "Amber", LastName = "Bloggs", DOB = new DateOnly(1990, 1, 1), DepartmentId = 1 },
            new Person { FirstName = "Jane", LastName = "Smith", DOB = new DateOnly(1985, 2, 2), DepartmentId = 2 }
        }.AsQueryable();

        // Act
        foreach (var person in people)
        {
            await _repository.AddPersonAsync(person);
        }
        await _context.SaveChangesAsync();
        var result = await _repository.GetAllPeopleAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }


    [Fact]
    public async Task DeletePersonAsync_ShouldRemovePersonFromContext()
    {
        var people = new List<Person>
        {
            new Person { FirstName = "Amber", LastName = "Bloggs", DOB = new DateOnly(1990, 1, 1), DepartmentId = 1 },
            new Person { FirstName = "Jane", LastName = "Smith", DOB = new DateOnly(1985, 2, 2), DepartmentId = 2 }
        }.AsQueryable();

        // Act
        foreach (var person in people)
        {
            await _repository.AddPersonAsync(person);
        }
        await _repository.SaveChangesAsync();

        // remove a person
        var result = await _repository.GetAllPeopleAsync();
        var firstPersonKey = result.First().Id;
        await _repository.DeletePersonAsync(firstPersonKey);
        await _repository.SaveChangesAsync();
        var resultAfterDelete = await _repository.GetAllPeopleAsync();

        // Assert - should be 1 less person
        Assert.Equal(people.Count() - 1, resultAfterDelete.Count());
    }
}
