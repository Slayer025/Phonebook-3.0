using PhonebookApp.Models;

namespace PhonebookApp.Repositories
{
    public interface IContactRepository
    {
        Task<PagedResult<Contact>> GetContactsPagedAsync(int pageNumber, int pageSize, string searchTerm);
        Task<Contact> GetContactByIdAsync(int id);
        Task<int> InsertContactAsync(Contact contact);
        Task<bool> UpdateContactAsync(Contact contact);
        Task<bool> DeleteContactAsync(int id);
    }
}
