using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Models.UserDTOs;
using DarkKitchen.Services;
using Moq;

namespace DarkKitchen.Tests.Services;

[TestClass]
public class UserServiceTest
{
    private Mock<IUserRepository>? _userRepositoryMock;
    private UserService? _userService;
    private UserDto _validUser;
    private User? _user;

    [TestInitialize]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _userService = new UserService(_userRepositoryMock.Object);

        _validUser = new UserDto("validName", "validSurname", "validEmail@gmail.com", "+59899123456", "validPassword1!2!3!");

        _user = new User
        {
            Name = _validUser.name!,
            Surname = _validUser.surname!,
            Email = _validUser.email!,
            Phone = _validUser.phone!,
            Password = _validUser.password!,
            Role = Role.Client,
        };
    }

    [TestMethod]
    public void CreateUser_WhenValidUser_ShouldCreateUser()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(_validUser.email!))
            .Returns((User?)null);
        _userRepositoryMock!
            .Setup(repository => repository.HasAny())
            .Returns(true);
        _userRepositoryMock!
            .Setup(repository => repository.Add(It.IsAny<User>()));

        _userService!.CreateUser(_validUser);

        _userRepositoryMock!
            .Verify(repository => repository.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateUser_WhenNoUsersExist_ShouldCreateAdmin()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(_validUser.email!))
            .Returns((User?)null);
        _userRepositoryMock!
            .Setup(repository => repository.HasAny())
            .Returns(false);
        _userRepositoryMock!
            .Setup(repository => repository.Add(It.IsAny<User>()));

        _userService!.CreateUser(_validUser);

        _userRepositoryMock!
            .Verify(repository => repository.Add(It.Is<User>(u => u.Role == Role.Admin)), Times.Once);
    }

    [TestMethod]
    public void CreateUser_WhenUsersExist_ShouldCreateClient()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(_validUser.email!))
            .Returns((User?)null);
        _userRepositoryMock!
            .Setup(repository => repository.HasAny())
            .Returns(true);
        _userRepositoryMock!
            .Setup(repository => repository.Add(It.IsAny<User>()));

        _userService!.CreateUser(_validUser);

        _userRepositoryMock!
            .Verify(repository => repository.Add(It.Is<User>(u => u.Role == Role.Client)), Times.Once);
    }

    [TestMethod]
    public void CreateUser_WhenInvalidName_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { name = string.Empty };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenInvalidSurname_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { surname = null };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenInvalidEmail_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { email = "invalidEmail" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenEmailIsEmpty_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { email = string.Empty };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenEmailHasTooManyAtSigns_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { email = "a@b@c@.com" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordHasConsecutiveDigits_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "ValidPass12!aaaaaa" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenLongPassword_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "longPassworddddddddddddddddddd" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenShortPassword_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "shortPassword" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_PasswordDoesNotContainsUpperLetter_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "lowerpasswordddd" };
        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_PasswordDoesNotContainsLowerLetter_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "UPPERPASSWORDDDD" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordDoesNotContainSpecialCharacter_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "PassWithoutSpecialChar123" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenPasswordDoesNotContainANumber_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { password = "PasswordWithoutNumber!" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void UpdateUser_WhenUserNotFound_ShouldThrowExceptionAndNotUpdateUser()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .Returns((User)null);
        UserNotFoundException ex = Assert.ThrowsException<UserNotFoundException>(() =>
            _userService!.UpdateUser(new UserDto(null, null, "fakeUser@gmail.com", null, null)));
        Assert.AreEqual("User not found.", ex.Message);
    }

    [TestMethod]
    public void UpdateUser_WhenEmailIsNull_ShouldThrowExceptionAndNotUpdateUser()
    {
        EmailEmptyException ex = Assert.ThrowsException<EmailEmptyException>(() =>
            _userService!.UpdateUser(new UserDto(null, null, null, null, null)));
        Assert.AreEqual("Email cannot be empty or whitespace.", ex.Message);
    }

    [TestMethod]
    public void UpdateUser_WhenValidUser_ShouldUpdateUser()
    {
        var dto = new UserDto("UpdatedName", "UpdatedSurname", _user!.Email, "099999999", "UpdatedPassword1!");

        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(_user.Email))
            .Returns(_user);

        _userRepositoryMock!
            .Setup(repository => repository.Update(It.IsAny<User>()));

        _userService!.UpdateUser(dto);

        User result = _userRepositoryMock.Object.GetByEmail(_user.Email)!;
        Assert.AreEqual("UpdatedName", result.Name);
        Assert.AreEqual("UpdatedSurname", result.Surname);
    }

    [TestMethod]
    public void GetUsers_WhenThereAreNoUsers_ShouldReturnEmptyList()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetUsers("validName", "validSurname"))
            .Returns([]);
        var filters = new UserFiltersDto
        {
            Name = "validName",
            Surname = "validSurname"
        };

        List<UserResponseDto> result = _userService!.GetUsers(filters);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetUsers_WhenRepositoryReturnsUsers_ShouldReturnMappedDtos()
    {
        _user = new User
        {
            Name = "validName",
            Surname = "validSurname",
            Email = "validEmail@gmail.com",
            Phone = "099123456",
            Password = "PasswordSegura123!",
            Role = Role.Client,
        };

        var users = new List<User> { _user };

        _userRepositoryMock!
            .Setup(repository => repository.GetUsers("validName", "validSurname"))
            .Returns(users);
        var filters = new UserFiltersDto
        {
            Name = "validName",
            Surname = "validSurname"
        };

        List<UserResponseDto> result = _userService!.GetUsers(filters);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("validName", result[0].name);
        Assert.AreEqual("validSurname", result[0].surname);
        Assert.AreEqual("validEmail@gmail.com", result[0].email);
    }

    [TestMethod]
    public void GetUsers_WhenUsersExist_ShouldReturnUsers()
    {
        _user = new User
        {
            Name = "validName",
            Surname = "validSurname",
            Email = "validEmail@gmail.com",
            Phone = "099123456",
            Password = "PasswordSegura123!",
            Role = Role.Client,
        };

        var users = new List<User> { _user };
        _userRepositoryMock!
            .Setup(repository => repository.GetUsers("validName", "validSurname"))
            .Returns(users);
        var filters = new UserFiltersDto
        {
            Name = "validName",
            Surname = "validSurname"
        };

        var result = _userService!.GetUsers(filters);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("validName", result[0].name);
    }

    [TestMethod]
    public void DeleteUser_WhenUserExists_ShouldDeleteUser()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail("validEmail@gmail.com"))
            .Returns(_user!);
        _userRepositoryMock!
            .Setup(repository => repository.Delete(It.IsAny<User>()));

        _userService!.DeleteUser("validEmail@gmail.com");

        _userRepositoryMock!
            .Verify(repository => repository.Delete(_user!), Times.Once);
    }

    [TestMethod]
    public void DeleteUser_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail("fakeUser@gmail.com"))
            .Returns((User?)null);

        Assert.ThrowsException<NotFoundException>(() => _userService!.DeleteUser("fakeUser@gmail.com"));
    }

    [TestMethod]
    public void DeleteUser_WhenEmailIsEmpty_ShouldThrowBadRequestException()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(It.IsAny<string>()))
            .Returns((User?)null);

        Assert.ThrowsException<BadRequestException>(() => _userService!.DeleteUser(string.Empty));
    }

    [TestMethod]
    public void EnsureNotSelf_WhenDifferentUser_DoesNotThrow()
    {
        _userService!.EnsureNotSelf("target@gmail.com", "requester@gmail.com");
    }

    [TestMethod]
    public void EnsureNotSelf_WhenSameUser_ShouldThrowBadRequestException()
    {
        Assert.ThrowsException<BadRequestException>(() =>
            _userService!.EnsureNotSelf("requester@gmail.com", "requester@gmail.com"));
    }

    [TestMethod]
    public void CreateUserWithRole_WhenValidUser_ShouldCreateUser()
    {
        _userRepositoryMock!.Setup(repository => repository.GetByEmail("validEmail@gmail.com")).Returns((User?)null);
        _userRepositoryMock!.Setup(repository => repository.Add(It.IsAny<User>()));

        _userService!.CreateUserWithRole(new CreateUserDto("validName", "validSurname", "validEmail@gmail.com", "099123456", "validPassword1!2!3!", "Admin"));

        _userRepositoryMock!.Verify(repository => repository.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateUserWithRole_WhenInvalidRole_ShouldThrowBadRequestException()
    {
        _userRepositoryMock!.Setup(repository => repository.GetByEmail("validEmail@gmail.com")).Returns((User?)null);

        Assert.ThrowsException<BadRequestException>(() =>
            _userService!.CreateUserWithRole(new CreateUserDto("validName", "validSurname", "validEmail@gmail.com", "099123456", "validPassword1!2!3!", "InvalidRole")));
    }

    [TestMethod]
    public void CreateUser_WhenDuplicateEmail_ShouldThrowBadRequestException()
    {
        _userRepositoryMock!.Setup(repository => repository.GetByEmail(_validUser.email!)).Returns(_user!);

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenEmailExistsButDeleted_ShouldReactivateUser()
    {
        var deletedUser = new User
        {
            Name = "OldName",
            Surname = "OldSurname",
            Email = _validUser.email!,
            Phone = "+59899000000",
            Password = "oldHashedPassword",
            Role = Role.Admin,
            IsDeleted = true,
        };

        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(_validUser.email!))
            .Returns(deletedUser);
        _userRepositoryMock!
            .Setup(repository => repository.Update(It.IsAny<User>()));

        _userService!.CreateUser(_validUser);

        _userRepositoryMock!.Verify(repository => repository.Update(
            It.Is<User>(u => !u.IsDeleted && u.Name == _validUser.name && u.Role == Role.Admin)), Times.Once);
        _userRepositoryMock!.Verify(repository => repository.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateUser_WhenPhoneHasInvalidFormat_ShouldThrowBadRequestException()
    {
        _validUser = _validUser with { phone = "099123456" };

        Assert.ThrowsException<BadRequestException>(() => _userService!.CreateUser(_validUser));
    }

    [TestMethod]
    public void CreateUser_WhenPhoneIsNull_ShouldCreateUser()
    {
        var userWithNullPhone = _validUser with { phone = null };

        _userRepositoryMock!
            .Setup(repository => repository.GetByEmail(userWithNullPhone.email!))
            .Returns((User?)null);
        _userRepositoryMock!
            .Setup(repository => repository.HasAny())
            .Returns(true);
        _userRepositoryMock!
            .Setup(repository => repository.Add(It.IsAny<User>()));

        _userService!.CreateUser(userWithNullPhone);

        _userRepositoryMock!
            .Verify(repository => repository.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateUserWithRole_WhenDuplicateEmail_ShouldThrowBadRequestException()
    {
        _userRepositoryMock!.Setup(repository => repository.GetByEmail("validEmail@gmail.com")).Returns(_user!);

        Assert.ThrowsException<BadRequestException>(() =>
            _userService!.CreateUserWithRole(new CreateUserDto("validName", "validSurname", "validEmail@gmail.com", "099123456", "validPassword1!2!3!", "Admin")));
    }

    [TestMethod]
    public void CreateUserWithRole_WhenInvalidName_ShouldThrowBadRequestException()
    {
        Assert.ThrowsException<BadRequestException>(() =>
            _userService!.CreateUserWithRole(new CreateUserDto(string.Empty, "validSurname", "validEmail@gmail.com", "099123456", "validPassword123!", "Admin")));
    }

    [TestMethod]
    public void GetUserById_WhenUserExists_ShouldReturnUser()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetById(1))
            .Returns(_user!);

        var result = _userService!.GetUserById(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(_user!.Name, result.Value.name);
    }

    [TestMethod]
    public void GetUserById_WhenUserDoesNotExist_ShouldReturnNull()
    {
        _userRepositoryMock!
            .Setup(repository => repository.GetById(999))
            .Returns((User?)null);

        var result = _userService!.GetUserById(999);

        Assert.IsNull(result);
    }
}
