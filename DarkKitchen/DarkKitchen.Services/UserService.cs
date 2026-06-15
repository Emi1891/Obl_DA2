using System.Text.RegularExpressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Repository;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.UserDTOs;

namespace DarkKitchen.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public void CreateUser(UserDto newUser)
    {
        ValidateUserFields(newUser.name, newUser.surname, newUser.email, newUser.password, newUser.phone);

        var existing = _userRepository.GetByEmail(newUser.email!);

        if(existing != null)
        {
            if(!existing.IsDeleted)
            {
                throw new BadRequestException("Email already in use.");
            }

            existing.Name = newUser.name!;
            existing.Surname = newUser.surname!;
            existing.Phone = newUser.phone!;
            existing.Password = BCrypt.Net.BCrypt.HashPassword(newUser.password);
            existing.IsDeleted = false;
            _userRepository.Update(existing);
            return;
        }

        var role = _userRepository.HasAny() ? Role.Client : Role.Admin;

        var user = new User
        {
            Name = newUser.name,
            Surname = newUser.surname,
            Email = newUser.email,
            Phone = newUser.phone,
            Password = BCrypt.Net.BCrypt.HashPassword(newUser.password),
            Role = role,
        };

        _userRepository.Add(user);
    }

    public void UpdateUser(UserDto updatedUser)
    {
        if(string.IsNullOrWhiteSpace(updatedUser.email))
        {
            throw new EmailEmptyException();
        }

        User user = _userRepository.GetByEmail(updatedUser.email!)
            ?? throw new UserNotFoundException();

        if(updatedUser.name != null)
        {
            user.Name = updatedUser.name;
        }

        if(updatedUser.surname != null)
        {
            user.Surname = updatedUser.surname;
        }

        if(updatedUser.phone != null)
        {
            user.Phone = updatedUser.phone;
        }

        if(updatedUser.password != null)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.password);
        }

        _userRepository.Update(user);
    }

    public List<UserResponseDto> GetUsers(UserFiltersDto filter)
    {
        List<User> users = _userRepository.GetUsers(filter.Name, filter.Surname);

        var result = users.Select(user => new UserResponseDto(user.Id, user.Name, user.Surname, user.Email, user.Phone, user.Role.ToString())).ToList();

        return result;
    }

    public void DeleteUser(string email)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            throw new BadRequestException("Invalid email.");
        }

        User? user = _userRepository.GetByEmail(email);

        if(user == null)
        {
            throw new NotFoundException("User");
        }

        _userRepository.Delete(user);
    }

    public void EnsureNotSelf(string? targetEmail, string requesterEmail)
    {
        if(string.Equals(targetEmail, requesterEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("A user cannot modify or delete their own account.");
        }
    }

    public UserResponseDto? GetUserById(int id)
    {
        User? user = _userRepository.GetById(id);

        if(user == null)
        {
            return null;
        }

        return new UserResponseDto(user.Id, user.Name, user.Surname, user.Email, user.Phone, user.Role.ToString());
    }

    public void CreateUserWithRole(CreateUserDto newUser)
    {
        ValidateUserFields(newUser.name, newUser.surname, newUser.email, newUser.password);

        if(_userRepository.GetByEmail(newUser.email!) != null)
        {
            throw new BadRequestException("Email already in use.");
        }

        if(!Enum.TryParse<Role>(newUser.role, ignoreCase: true, out var role))
        {
            throw new BadRequestException("Invalid role.");
        }

        var user = new User
        {
            Name = newUser.name!,
            Surname = newUser.surname!,
            Email = newUser.email!,
            Phone = newUser.phone!,
            Password = BCrypt.Net.BCrypt.HashPassword(newUser.password),
            Role = role,
        };

        _userRepository.Add(user);
    }

    private static void ValidateUserFields(string? name, string? surname, string? email, string? password, string? phone = null)
    {
        var message = string.Empty;

        if(string.IsNullOrEmpty(name))
        {
            message = "Name can't be empty.";
        }

        if(string.IsNullOrEmpty(surname))
        {
            message = "Surname can't be empty.";
        }

        if(string.IsNullOrEmpty(email))
        {
            throw new BadRequestException("Invalid email format.");
        }

        if((!email!.Contains('@')) || email.Count(c => c == '@') > 2)
        {
            message = "Invalid email format.";
        }

        if(password!.Length > 25)
        {
            message = "Password can't have more than 25 characters.";
        }

        if(password.Length < 15)
        {
            message = "Password can't have less than 15 characters.";
        }

        if(!password.Any(char.IsUpper))
        {
            message = "Password must contain at least one upper case.";
        }

        if(!password.Any(char.IsLower))
        {
            message = "Password must contain at least one lower case.";
        }

        var specialChars = "!@#$%^&*()_+-=[]{};:,.<>?/~";
        if(!password.Any(specialChars.Contains))
        {
            message = "Password must contain at least one special character (!@#$%^&*()_+-=[]{};:,.<>?/~).";
        }

        if(!password.Any(char.IsDigit))
        {
            message = "Password must contain at least one number.";
        }

        var previousCharWasDigit = false;
        foreach(var letter in password)
        {
            if(char.IsDigit(letter) && previousCharWasDigit)
            {
                message = "Password can't contain a number sequence.";
                break;
            }

            previousCharWasDigit = char.IsDigit(letter);
        }

        if(!string.IsNullOrEmpty(phone))
        {
            var digitsOnly = Regex.Replace(phone, @"[\s\-().]", string.Empty);
            if(!Regex.IsMatch(digitsOnly, @"^\+[1-9]\d{6,14}$"))
            {
                message = "Phone number must include a country code and be in a valid format (e.g. +1 5551234567).";
            }
        }

        if(!string.IsNullOrEmpty(message))
        {
            throw new BadRequestException(message);
        }
    }
}
