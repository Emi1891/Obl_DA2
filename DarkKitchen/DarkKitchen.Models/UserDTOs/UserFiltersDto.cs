namespace DarkKitchen.Models.UserDTOs;

public sealed record UserFiltersDto
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
}
