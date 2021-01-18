namespace PortalGestorias.Web.Security
{
    public interface ISecurityManagerService
    {
        bool ValidateCredentials(string username, string password);
        BasicUser GetIdentityInfoByUsername(string username);
        BasicUser GetIdentityInfoBySubject(string subject);
    }
}