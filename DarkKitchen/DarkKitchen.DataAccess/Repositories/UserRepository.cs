using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Repository;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public bool HasAny() => context.Users.Any();

    public void Add(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public void Delete(User user)
    {
        var existingUser = context.Users.Find(user.Id);
        if(existingUser == null)
        {
            return;
        }

        existingUser.IsDeleted = true;
        context.SaveChanges();
    }

    public User? GetByEmail(string email)
    {
        return context.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(int id)
    {
        return context.Users.Find(id);
    }

    public List<User> GetUsers(string? name, string? surname)
    {
        return context.Users.Where(u =>
            !u.IsDeleted && (string.IsNullOrEmpty(name) || u.Name == name) && (string.IsNullOrEmpty(surname) || u.Surname == surname)).ToList();
    }

    public void Update(User user)
    {
        var existingUser = context.Users.Find(user.Id);
        if(existingUser == null)
        {
            return;
        }

        context.Entry(existingUser).CurrentValues.SetValues(user);
        context.SaveChanges();
    }
}
