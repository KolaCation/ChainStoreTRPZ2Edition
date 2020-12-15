using System.Threading.Tasks;
using ChainStore.Domain.Identity;

namespace ChainStore.DataAccessLayer.Identity
{
    public interface IAuthenticationService
    {
        Task<User> Login(string email, string password);
        Task<RegistrationResult> Register(string name, string email, string password, string confirmPassword);
    }
}