using System;
using Npgsql;
using System.Collections.Generic;
namespace lab2
{
    public class OrderRepository
    {
        private NpgsqlConnection connection;
        public OrderRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Order GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM orders WHERE ord_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Order order = new Order();
                order.order_id = reader.GetInt32(0);
                order.cost = reader.GetDouble(1);
                order.user_id = reader.GetInt32(2);
                connection.Close();
                return order;
            }
            else 
            {
                connection.Close();
                throw new Exception("there is no orders with such id");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders WHERE ord_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public int DeleteByUserId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders WHERE us_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public int DeleteByIdOrdersItems(int id)
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
        public object Insert(Order order)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO orders (cost, us_id) 
                VALUES (@cost, @us_id) RETURNING ord_id;
            ";
            command.Parameters.AddWithValue("cost", order.cost);
            command.Parameters.AddWithValue("us_id", order.user_id);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Order order)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            "Update orders SET cost = @cost, us_id = @us_id WHERE ord_id = @id";
            command.Parameters.AddWithValue("cost", order.cost);
            command.Parameters.AddWithValue("us_id", order.user_id);
            command.Parameters.AddWithValue("id", order.order_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public int InsertOrderItems(int order_id, int item_id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO orders_items (ord_id, item_id) 
                VALUES (@order_id, @item_id) RETURNING ord_id;
            ";
            command.Parameters.AddWithValue("order_id", order_id);
            command.Parameters.AddWithValue("item_id", item_id);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return (int)newId;
        }
        public long GetCount()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM orders";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public long GetItemsInOrdersCount(int item_id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM orders_items WHERE item_id = @item_id";
            command.Parameters.AddWithValue("item_id", item_id);
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public long GetSearchCount(int[] measures2, int[] measures1)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM orders WHERE us_id BETWEEN @c AND @d 
            AND cost BETWEEN @a AND @b";
            command.Parameters.AddWithValue("c", measures2[0]);
            command.Parameters.AddWithValue("d", measures2[1]);
            command.Parameters.AddWithValue("a", measures1[0]);
            command.Parameters.AddWithValue("b", measures1[1]);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public List<Order> GetAllSearch(int[] measures2, int[] measures1)
        {
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
                order.order_id = reader.GetInt32(0);
                order.cost = reader.GetDouble(1);
                order.user_id = reader.GetInt32(2);
                list.Add(order);
            }
            connection.Close();
            return list;
        }
    }
}
