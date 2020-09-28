using Common.Domain.Entities;

namespace Common.Persistence.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
