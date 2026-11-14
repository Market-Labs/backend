namespace marketgo_backend.Iam.Interfaces.Acl;

public interface IIamContextFacade
{
    Task<int> CreateUser(string fullName, string email, string password, string organizationName,
        string accountType, CancellationToken cancellationToken);
    Task<int> FetchUserIdByEmail(string email, CancellationToken cancellationToken);
    Task<string> FetchEmailByUserId(int userId, CancellationToken cancellationToken);
}
