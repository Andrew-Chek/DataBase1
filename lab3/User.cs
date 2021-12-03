namespace lab3
{
    public class User
    {
        public string name;
        public Order[] orders;
        public int user_id;
        public string password;
        public User(string name)
        {
            this.name = name;
        }
        public User(int id, string name)
        {
            this.user_id = id;
            this.name = name;
        }
        public User(string name, string password)
        {
            this.password = password;
            this.name = name;
        }
        public User()
        {
            this.name = "newUser";
        }
        public override string ToString()
        {
            return $"Id: {this.user_id}, Name: {this.name}";
        }
    }
}
