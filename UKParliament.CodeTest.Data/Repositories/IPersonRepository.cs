
namespace UKParliament.CodeTest.Data.Repositories
{
    public interface IPersonRepository
    {
        Task<int> AddPersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<IEnumerable<Person>> GetAllPeopleAsync();
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Person?> GetPersonByIdAsync(int id);
        Task SaveChangesAsync();
    }
}