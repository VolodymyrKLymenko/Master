namespace IdentityService.Domain
{
    public class User
    {
        public int Id { get; }
        public string Login { get; }
        public string Password { get; }

        private User(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }

        public static User Create(int id, string login, string password)
        {
            return new User(id, login, password);
        }
    }
}
