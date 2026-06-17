using System.Security.Claims;
using DarkKitchen.API.Controllers;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests.API.Controllers;

[TestClass]
public class UsersControllerTest
{
    private readonly string validEmail = "valid@email.com";
    private readonly string password = "ValidPassword123!";
    private readonly string validPhone = "+59899123123";
    private UserDto validUser;
    private UserDto updatedUser;
    private CreateUserDto validAdminUser;
    private List<UserResponseDto>? users;
    private Mock<IUserService>? userServiceMock;
    private UsersController? userController;

    [TestInitialize]
    public void Setup()
    {
        userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        userController = new UsersController(userServiceMock.Object);

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@email.com")]);
        userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        users =
        [
            new UserResponseDto(1, "Name", "Surname", validEmail, validPhone, nameof(Role.Client))
        ];
        validUser = new UserDto("Name", "Surname", validEmail, validPhone, password);
        updatedUser = new UserDto("UpdatedName", "UpdatedSurname", validEmail, validPhone, password);
        validAdminUser = new CreateUserDto("Name", "Surname", validEmail, validPhone, password, nameof(Role.Admin));
    }

    [TestMethod]
    public void UserSignUp_ValidUserWithoutRole_UserRegisteredAsClient()
    {
        userServiceMock.Setup(s => s.CreateUser(validUser));
        var result = userController.SignUp(validUser);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj.StatusCode);
    }

    [TestMethod]
    public void UserSignUp_UserAlreadyExists_ThrowsBadRequestException()
    {
        userServiceMock.Setup(s => s.CreateUser(validUser)).Throws(new BadRequestException("User already exists."));
        Assert.ThrowsException<BadRequestException>(() => userController.SignUp(validUser));
    }

    [TestMethod]
    public void UserSignUp_InvalidEmailFormat_ThrowsBadRequestException()
    {
        var invalidEmailUser = new UserDto("Name", "Surname", "invalid-email", validPhone, password);
        userServiceMock.Setup(s => s.CreateUser(invalidEmailUser)).Throws(new BadRequestException("Invalid email address."));
        Assert.ThrowsException<BadRequestException>(() => userController.SignUp(invalidEmailUser));
    }

    [TestMethod]
    public void UserSignUp_InvalidPhoneFormat_ThrowsBadRequestException()
    {
        var invalidPhoneUser = new UserDto("Name", "Surname", validEmail, "invalid-phone", password);
        userServiceMock.Setup(s => s.CreateUser(invalidPhoneUser)).Throws(new BadRequestException("Invalid phone number."));
        Assert.ThrowsException<BadRequestException>(() => userController.SignUp(invalidPhoneUser));
    }

    [TestMethod]
    public void UserSignUp_InvalidPassword_ThrowsBadRequestException()
    {
        var invalidPasswordUser = new UserDto("Name", "Surname", validEmail, validPhone, "short");
        userServiceMock.Setup(s => s.CreateUser(invalidPasswordUser)).Throws(new BadRequestException("Password does not meet complexity requirements."));
        Assert.ThrowsException<BadRequestException>(() => userController.SignUp(invalidPasswordUser));
    }

    [TestMethod]
    public void CreateUserWithRole_ValidData_ReturnsCreated()
    {
        userServiceMock.Setup(s => s.CreateUserWithRole(validAdminUser));
        var result = userController.CreateUserWithRole(validAdminUser);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(201, resultObj.StatusCode);
    }

    [TestMethod]
    public void CreateUserWithRole_UserAlreadyExists_ThrowsBadRequestException()
    {
        userServiceMock.Setup(s => s.CreateUserWithRole(validAdminUser)).Throws(new BadRequestException("User already exists."));
        Assert.ThrowsException<BadRequestException>(() => userController.CreateUserWithRole(validAdminUser));
    }

    [TestMethod]
    public void UpdateUser_ExistingUser_ReturnsOk()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(validUser.email, It.IsAny<string>()));
        userServiceMock.Setup(s => s.UpdateUser(validUser));
        var result = userController.UpdateUser(validUser);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_UserNotFound_ThrowsNotFoundException()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(validUser.email, It.IsAny<string>()));
        userServiceMock.Setup(s => s.UpdateUser(validUser)).Throws(new NotFoundException("User not found."));
        Assert.ThrowsException<NotFoundException>(() => userController.UpdateUser(validUser));
    }

    [TestMethod]
    public void GetUsers_ValidFilter_ReturnsOk()
    {
        userServiceMock.Setup(s => s.GetUsers(It.IsAny<UserFiltersDto>())).Returns(users!);
        var filters = new UserFiltersDto
        {
            Name = "Name",
            Surname = "Surname"
        };

        var result = userController.GetUsers(filters);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void GetUsers_NoUsersFound_ThrowsNotFoundException()
    {
        userServiceMock.Setup(s => s.GetUsers(It.IsAny<UserFiltersDto>())).Throws(new NotFoundException("No users found."));
        var filters = new UserFiltersDto
        {
            Name = "Name",
            Surname = "Surname"
        };

        Assert.ThrowsException<NotFoundException>(() => userController.GetUsers(filters));
    }

    [TestMethod]
    public void DeleteUser_ExistingUser_ReturnsOk()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(validEmail, It.IsAny<string>()));
        userServiceMock.Setup(s => s.DeleteUser(validEmail));
        var result = userController.DeleteUser(validEmail);
        var resultObj = result as ObjectResult;

        Assert.IsNotNull(resultObj);
        Assert.AreEqual(200, resultObj.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_UserNotFound_ThrowsNotFoundException()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(validEmail, It.IsAny<string>()));
        userServiceMock.Setup(s => s.DeleteUser(validEmail)).Throws(new NotFoundException("User not found."));
        Assert.ThrowsException<NotFoundException>(() => userController.DeleteUser(validEmail));
    }

    [TestMethod]
    public void DeleteUser_EmptyEmail_ThrowsBadRequestException()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(string.Empty, It.IsAny<string>()));
        userServiceMock.Setup(s => s.DeleteUser(string.Empty)).Throws(new BadRequestException("Email can't be empty."));
        Assert.ThrowsException<BadRequestException>(() => userController.DeleteUser(string.Empty));
    }

    [TestMethod]
    public void DeleteUser_WhenSelf_PropagatesBadRequestException()
    {
        userServiceMock.Setup(s => s.EnsureNotSelf(validEmail, It.IsAny<string>()))
            .Throws(new BadRequestException("A user cannot modify or delete their own account."));

        Assert.ThrowsException<BadRequestException>(() => userController.DeleteUser(validEmail));
        userServiceMock.Verify(s => s.DeleteUser(It.IsAny<string>()), Times.Never);
    }
}
