using Npgsql;

namespace lab3
{
    public class UserRepository
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc");
        private postgresContext context;
        private bool addIndex;
        public UserRepository(postgresContext context)
        {
            this.context = context;
        }
        public object Insert(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user.UsId;
        }
        public bool Delete(int id)
        {
            User user = context.Users.Find(id);
            if (user == null)
            {
                return false;
            }
            else
            {
                context.Users.Remove(user);
                DeleteAllOrdersByUser(user);
                context.SaveChanges();
                return true;
            }
        }
        public Order GetById(int id)
        {
            Order order = context.Orders.Find(id);
            if (order == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return order;
        }
        public bool Update(Order order)
        {
            Order orderToUpdate = context.Orders.Find(order.OrdId);
            if (orderToUpdate == null || context.Users.Find(order.UsId) == null)
                return false;
            else if(orderToUpdate != null && context.Users.Find(order.UsId) != null)
                orderToUpdate.UsId = order.UsId;
                orderToUpdate.Cost = order.Cost;
                context.SaveChanges();
                return true;
        }
        public List<Order> FindOrdersByUsId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM orders WHERE us_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Order> orders = new List<Order>();
            while(reader.Read())
            {
                Order order = new Order();
                order.OrdId = reader.GetInt32(0);
                order.Cost = reader.GetDouble(1);
                order.UsId = reader.GetInt32(2);
                orders.Add(order);
            }
            connection.Close();
            return orders;
        }
        public bool DeleteAllOrdersByUser(User user)
        {
            List<Order> orders = FindOrdersByUsId(user.UsId);
            DeleteAllConsByUser(orders);
            int nChanged = -1;
            foreach(Order ord in orders)
            {
                connection.Open();
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM orders WHERE ord_id = @id";
                command.Parameters.AddWithValue("id", ord.OrdId);
                nChanged = command.ExecuteNonQuery();
                connection.Close();
            }
            return nChanged == 1;
        }
        public bool DeleteAllConsByUser(List<Order> orders)
        {
            if(orders.Count != 0)
            {
                foreach(Order ord in orders)
                {
                    DeleteByOrIdOrdsIts(ord.OrdId);
                }
                return true;
            }
            else
                return false;
        }
        public int DeleteByOrIdOrdsIts(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders_items WHERE ord_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public List<User> GetAllSearch(string value)
        {
            if(!addIndex)
            {
                AddingIndexes();
            }
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE name LIKE '%' || @value || '%' 
                AND password LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<User> list = new List<User>();
            while(reader.Read())
            {
                User user = new User();
                user.UsId = reader.GetInt32(0);
                user.Name = reader.GetString(1);
                user.Password = reader.GetString(2);
                list.Add(user);
            }
            connection.Close();
            return list;
        }
        private void AddingIndexes()
        {
            this.addIndex = true;
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE INDEX if not exists users_psw_idx ON users using GIN (password);
                CREATE INDEX if not exists users_name_idx ON users using GIN (name);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
