namespace lab2
{
    public class User
    {
        public string name;
        public bool moderator;
        public Order[] orders;
        public int user_id;
        public string password;
        public User(string name, bool moderator)
        {
            this.name = name;
            this.moderator = moderator;
        }
        public User(int id, string name, bool moderator)
        {
            this.user_id = id;
            this.name = name;
            this.moderator = moderator;
        }
        public User()
        {
            this.name = "newUser";
            this.moderator = false;
        }
        public override string ToString()
        {
            return $"Id: {this.user_id}, Name: {this.name}";
        }
    }
}
