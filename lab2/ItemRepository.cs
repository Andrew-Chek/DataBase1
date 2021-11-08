using System;
using System.Collections.Generic;
using Npgsql;
namespace lab2
{
    public class ItemRepository
    {
        private NpgsqlConnection connection;
        public ItemRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Item GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.cost = reader.GetDouble(1);
                item.availability = reader.GetBoolean(2);
                item.name = reader.GetString(3);
                connection.Close();
                return item;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no items with such id");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public int DeleteByIdOrdersItems(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders_items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(Item item)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO items (cost, availability, name) 
                VALUES (@cost, @availability, @name) RETURNING item_id;
            ";
            command.Parameters.AddWithValue("name", item.name);
            command.Parameters.AddWithValue("cost", item.cost);
            command.Parameters.AddWithValue("availability", item.availability);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Item item)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            "Update items SET name = @name, cost = @cost, availability = @availability WHERE item_id = @id";
            command.Parameters.AddWithValue("cost", item.cost);
            command.Parameters.AddWithValue("name", item.name);
            command.Parameters.AddWithValue("availability", item.availability);
            command.Parameters.AddWithValue("id", item.item_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public long InsertOrderItems(int order_id, int item_id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO orders_items (ord_id, item_id) 
                VALUES (@order_id, @item_id);
            ";
            command.Parameters.AddWithValue("order_id", order_id);
            command.Parameters.AddWithValue("item_id", item_id);
            return order_id;
        }
        public long GetCount()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM items";
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
        public long GetSearchCount(string value, bool bVal, int[] measures)
        {
            if(string.IsNullOrEmpty(value))
            {
                return this.GetCount();
            }
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM items WHERE name LIKE '%' || @value || '%' 
                AND cost BETWEEN @a AND @b AND availability = @bVal";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("bVal", bVal);
            command.Parameters.AddWithValue("a", measures[0]);
            command.Parameters.AddWithValue("b", measures[1]);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public List<Item> GetAllSearch(string value, bool bVal, int[] measures)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE name LIKE '%' || @value || '%' 
                AND cost BETWEEN @a AND @b AND availability = @bVal";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("bVal", bVal);
            command.Parameters.AddWithValue("a", measures[0]);
            command.Parameters.AddWithValue("b", measures[1]);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Item> list = new List<Item>();
            while(reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.cost = reader.GetDouble(1);
                item.availability = reader.GetBoolean(2);
                item.name = reader.GetString(3);
                list.Add(item);
            }
            connection.Close();
            return list;
        }
    }
}
