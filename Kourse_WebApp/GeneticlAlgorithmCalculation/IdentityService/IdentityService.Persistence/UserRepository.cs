using IdentityService.Persistence.Entities;

namespace IdentityService.Persistence
{
    public interface IUserRepository : IGenericUserRepository<UserDTO>
    {
    }

    public class UserRepository : GenericUserRepository<UserDTO>, IUserRepository
    {
        public UserRepository(UserContext context) : base(context)
        {
        }
    }
}
