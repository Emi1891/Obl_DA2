using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces.Repository;

public interface IUserRepository
{
    public bool HasAny();
    public User? GetByEmail(string email);
    public User? GetById(int id);
    public void Add(User user);
    public void Update(User user);
    public List<User> GetUsers(string? name, string? surname);
    public void Delete(User user);
}
