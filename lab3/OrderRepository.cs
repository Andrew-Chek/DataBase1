using Npgsql;

namespace lab3
{
    public class OrderRepository
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc");
        postgresContext context;
        private bool addIndex;
        public OrderRepository(postgresContext context)
        {
            this.context = context;
        }
        public object Insert(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
            return order.OrdId;
        }
        public bool Delete(int id)
        {
            Order order = context.Orders.Find(id);
            if (order == null)
            {
                return false;
            }
            else
            {
                context.Orders.Remove(order);
                DeleteByOrIdOrdsIts(order.OrdId);
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
        public int DeleteByOrdersItems(int ord_id, int item_id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders_items WHERE ord_id = @id AND item_id = @it_id";
            command.Parameters.AddWithValue("id", ord_id);
            command.Parameters.AddWithValue("it_id", item_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        private void AddingIndexes()
        {
            this.addIndex = true;
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE INDEX if not exists orders_cost_idx ON orders (cost);
                CREATE INDEX if not exists orders_usId_idx ON orders (us_id);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
        public List<Order> GetAllSearch(int[] measures2, int[] measures1)
        {
            if(!addIndex)
            {
                AddingIndexes();
            }
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE us_id BETWEEN @c AND @d 
            AND cost BETWEEN @a AND @b";
            command.Parameters.AddWithValue("c", measures2[0]);
            command.Parameters.AddWithValue("d", measures2[1]);
            command.Parameters.AddWithValue("a", measures1[0]);
            command.Parameters.AddWithValue("b", measures1[1]);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Order> list = new List<Order>();
            while(reader.Read())
            {
                Order order = new Order();
                order.OrdId = reader.GetInt32(0);
                order.Cost = reader.GetDouble(1);
                order.UsId = reader.GetInt32(2);
                list.Add(order);
            }
            connection.Close();
            return list;
        }
    }
}
