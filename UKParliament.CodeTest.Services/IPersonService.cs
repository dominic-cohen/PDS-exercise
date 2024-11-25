using UKParliament.CodeTest.Data;

namespace UKParliament.CodeTest.Services
{
    public interface IPersonService
    {
        Task<int> AddPersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<IEnumerable<Person>> GetAllPeopleAsync();
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Person?> GetPersonByIdAsync(int id);
        Task UpdatePersonAsync(Person person);
    }
}