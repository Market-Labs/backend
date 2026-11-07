namespace marketgo_backend.Iam.Application.Services;

public interface ISessionTokenService
{
    string CreateToken();
    string HashToken(string token);
}
