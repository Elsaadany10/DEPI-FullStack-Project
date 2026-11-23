namespace HotelManagement.Api.Security;

public sealed record CurrentUser(int Id, string Email, string Role)
{
    public bool IsStaff => Role is Roles.Receptionist or Roles.Admin;
    public bool IsAdmin => Role == Roles.Admin;
}