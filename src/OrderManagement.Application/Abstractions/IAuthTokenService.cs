namespace OrderManagement.Application.Abstractions;

public interface IAuthTokenService
{
    string CreateToken(string email);
}
