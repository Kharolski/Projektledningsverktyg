using System;

public class MemberModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; }
    public DateTime BirthDate { get; set; }
    public string ProfileImagePath { get; set; }
}
