using System;
using static System.Console;
using System.Collections.Generic;
using Npgsql;
namespace lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc";

            var conn = new NpgsqlConnection(connString);
            conn.Open();
            ItemRepository repo = new ItemRepository(connString);
            Item item = repo.GetById(3);
            WriteLine(item.ToString());
            WriteLine(repo.GetCount());
        }
    }
    public class Item
    {
        public int item_id;
        public string name;
        public double cost;
        public bool availability;
        public Item(string name, double cost, bool availability)
        {
            this.name = name;
            this.cost = cost;
            this.availability = availability;
        }
        public Item(int goods_id, string name, double cost, bool availability)
        {
            this.name = name;
            this.cost = cost;
            this.item_id = goods_id;
            this.availability = availability;
        }
        public Item()
        {
            this.name = "";
            this.cost = 0;
        }
        public override string ToString()
        {
            return $"Id: {this.item_id}, Name: {this.name}";
        }
    }
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
                throw new Exception("there is no items with such id");
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
        public long Insert(Item item)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"INSERT INTO items (cost, availability, name) 
            VALUES (@cost, @availability, @name) RETURNING item_id;";
            command.Parameters.AddWithValue("name", $"{item.name}");
            command.Parameters.AddWithValue("cost", $"{item.cost}");
            command.Parameters.AddWithValue("availability", $"{item.availability}");
            long newId = (long)command.ExecuteScalar();
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
            command.Parameters.AddWithValue("$order_id", $"{order_id}");
            command.Parameters.AddWithValue("$item_id", $"{item_id}");
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
        public List<Item> GetPage(int pageNumber, int pageLength)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            int num = pageNumber * pageLength - pageLength;
            command.CommandText = "SELECT * FROM items LIMIT @numOfEl OFFSET @passEls";
            command.Parameters.AddWithValue("numOfEl", pageLength);
            command.Parameters.AddWithValue("passEls", num);
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
        public long GetSearchCount(string value, bool bVal, int a, int b)
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
            command.Parameters.AddWithValue("a", a);
            command.Parameters.AddWithValue("b", b);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public double GetPagesCount(int pageLength)
        {
            if(pageLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageLength));
            }
            return (double)GetCount() / (double) pageLength;
        }
        public List<Item> GetSearchPage(string value, bool bVal, int a, int b, int pageNumber, int pageLength)
        {
            if(string.IsNullOrEmpty(value))
            {
                return this.GetPage(pageNumber, pageLength);
            }
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            if(pageLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageLength));
            }
            int num = pageNumber * pageLength - pageLength;
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM items WHERE name LIKE '%' || @value || '%' 
            AND cost BETWEEN @a AND @b AND availability = @bVal LIMIT @numOfEl OFFSET @passEls";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("bVal", bVal);
            command.Parameters.AddWithValue("a", a);
            command.Parameters.AddWithValue("b", b);
            command.Parameters.AddWithValue("numOfEl", pageLength);
            command.Parameters.AddWithValue("passEls", num);
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
        public double GetSearchPagesCount(string value, bool bVal, int a, int b, int pagelength)
        {
            return (double)GetSearchCount(value, bVal, a, b) / (double) pagelength;
        }
    }
}
