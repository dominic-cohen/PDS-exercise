using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Data.Repositories;

namespace UKParliament.CodeTest.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddPersonAsync(Person person)
    {
        if (person == null)
        {
            throw new ArgumentNullException(nameof(person));
        }

        await _repository.AddPersonAsync(person);
        await SaveChangesAsync();

        return person.Id;
    }

    public async Task UpdatePersonAsync(Person person)
    {
        var existingPerson = await _repository.GetPersonByIdAsync(person.Id);
        if (existingPerson == null)
        {
            throw new InvalidOperationException($"Person with ID {person.Id} does not exist.");
        }

        existingPerson.Title = person.Title;
        existingPerson.FirstName = person.FirstName;
        existingPerson.LastName = person.LastName;
        existingPerson.DOB = person.DOB;
        existingPerson.AddressLine1 = person.AddressLine1;
        existingPerson.AddressLine2 = person.AddressLine2;
        existingPerson.AddressLine3 = person.AddressLine3;
        existingPerson.AddressLine4 = person.AddressLine4;
        existingPerson.PostCode = person.PostCode;
        existingPerson.DepartmentId = person.DepartmentId;

        await SaveChangesAsync();
    }

    public async Task<Person?> GetPersonByIdAsync(int id)
    {
        return await _repository.GetPersonByIdAsync(id);
    }

    public async Task DeletePersonAsync(int id)
    {
        var person = await _repository.GetPersonByIdAsync(id);
        if (person == null)
        {
            throw new InvalidOperationException($"Person with ID {id} does not exist.");
        }

        await _repository.DeletePersonAsync(id);
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<Person>> GetAllPeopleAsync()
    {
        return await _repository.GetAllPeopleAsync();
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _repository.GetAllDepartmentsAsync();
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }
}
