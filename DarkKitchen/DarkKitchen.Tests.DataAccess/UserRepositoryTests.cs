using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Tests.DataAccess;

[TestClass]
public class UserRepositoryTests
{
    private readonly string validEmail = "valid@email.com";
    private readonly string validEmail2 = "valid@email2.com";
    private User? user;
    private User? user2;
    private AppDbContext? _context;
    private UserRepository? _userRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        user = new User
        {
            Name = "Name",
            Surname = "Surname",
            Email = validEmail,
            Phone = "+598099123456",
            Password = "Password1!",
            Role = Role.Client
        };
        user2 = new User
        {
            Name = "Name2",
            Surname = "Surname2",
            Email = validEmail2,
            Phone = "+598099123456",
            Password = "Password1!",
            Role = Role.Client
        };
        var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _context = new AppDbContext(options);
        _userRepository = new UserRepository(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void GetUserByEmail_WhenUserExists_ReturnsUser()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();

        var result = _userRepository.GetByEmail(validEmail);

        Assert.AreEqual(validEmail, result.Email);
    }

    [TestMethod]
    public void GetUserByEmail_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = _userRepository.GetByEmail(validEmail);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetById_WhenUserExists_ReturnsUser()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();

        var result = _userRepository.GetById(user!.Id);

        Assert.AreEqual(validEmail, result!.Email);
    }

    [TestMethod]
    public void GetById_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = _userRepository.GetById(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void AddUser_WhenUserIsValid_AddsUserToDatabase()
    {
        _userRepository.Add(user!);

        var result = _context.Users.FirstOrDefault(u => u.Email == validEmail);

        Assert.AreEqual(validEmail, result.Email);
    }

    [TestMethod]
    public void AddUser_WhenUserAlreadyExists_ThrowsException()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();

        Assert.ThrowsException<ArgumentException>(() =>
        {
            _userRepository.Add(user!);
        });
    }

    [TestMethod]
    public void DeleteUser_WhenUserExists_SoftDeletesUser()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();

        _userRepository.Delete(user!);
        var result = _context.Users.FirstOrDefault(u => u.Email == validEmail);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsDeleted);
    }

    [TestMethod]
    public void DeleteUser_WhenUserDoesNotExist_DoesNothing()
    {
        _userRepository.Delete(user!);

        Assert.AreEqual(0, _context!.Users.Count());
    }

    [TestMethod]
    public void GetUsers_WhenUsersExist_ReturnsListOfUsers()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();

        var result = _userRepository.GetUsers(user.Name, user.Surname);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(validEmail, result[0].Email);
    }

    [TestMethod]
    public void GetUsers_WhenNullParameters_ReturnsAllUsers()
    {
        _context.Users.Add(user!);
        _context.Users.Add(user2!);
        _context.SaveChanges();

        var result = _userRepository.GetUsers(null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetUsers_WhenNoUsersExist_ReturnsEmptyList()
    {
        var result = _userRepository.GetUsers(null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetUsers_WhenUserIsSoftDeleted_ExcludesUser()
    {
        user!.IsDeleted = true;
        _context.Users.Add(user!);
        _context.Users.Add(user2!);
        _context.SaveChanges();

        var result = _userRepository.GetUsers(null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(validEmail2, result[0].Email);
    }

    [TestMethod]
    public void UpdateUser_WhenUserExists_UpdatesUserInDatabase()
    {
        _context.Users.Add(user!);
        _context.SaveChanges();
        user.Name = "UpdatedName";

        _userRepository.Update(user!);
        var result = _context.Users.FirstOrDefault(u => u.Email == validEmail);

        Assert.AreEqual("UpdatedName", result.Name);
    }

    [TestMethod]
    public void UpdateUser_WhenUserDoesNotExist_DoesNothing()
    {
        _userRepository.Update(user!);

        Assert.AreEqual(0, _context!.Users.Count());
    }
}
