using Npgsql;

namespace lab3
{
    public class ItemRepository
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc");
        postgresContext context;
        private bool addIndex;
        public ItemRepository(postgresContext context)
        {
            this.context = context;
        }
        public object Insert(Item item)
        {
            context.Items.Add(item);
            context.SaveChanges();
            return item.ItemId;
        }
        public bool Delete(int id)
        {
            Item item = context.Items.Find(id);
            if (item == null)
            {
                return false;
            }
            else
            {
                context.Items.Remove(item);
                DeleteByItIdOrdsIts(item.ItemId);
                context.SaveChanges();
                return true;
            }
        }
        public Item GetById(int id)
        {
            Item item = context.Items.Find(id);
            if (item == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return item;
        }
        public bool Update(Item item)
        {
            Item itemToUpdate = context.Items.Find(item.ItemId);
            if (itemToUpdate == null)
                return false;
            else
                itemToUpdate.Name = item.Name;
                itemToUpdate.Cost = item.Cost;
                itemToUpdate.Availability = item.Availability;
            context.SaveChanges();
            return true;
        }
        public int DeleteByItIdOrdsIts(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM orders_items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public List<Item> GetAllSearch(string value, bool av, int[] measures)
        {
            if(!addIndex)
            {
                AddingIndexes();
            }
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE name LIKE '%' || @value || '%' AND availability = @av AND cost BETWEEN @a AND @b";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("av", av);
            command.Parameters.AddWithValue("a", measures[0]);
            command.Parameters.AddWithValue("b", measures[1]);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Item> list = new List<Item>();
            while(reader.Read())
            {
                Item item = new Item();
                item.ItemId = reader.GetInt32(0);
                item.Cost = reader.GetDouble(1);
                item.Availability = reader.GetBoolean(2);
                item.Name = reader.GetString(3);
                list.Add(item);
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
                CREATE INDEX if not exists items_cost_idx ON items (cost);
                CREATE INDEX if not exists items_name_idx ON items using GIN (name);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
