using Npgsql;

namespace lab3
{
    public class ItemRepository
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc");
        postgresContext context;
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
    }
}
