using System;
using System.Collections.Generic;
using Npgsql;
namespace lab2
{
    public class Generation
    {
        private NpgsqlConnection connection;
        private string connString;
        public Generation(string connString)
        {
            this.connString = connString;
            this.connection = new NpgsqlConnection(connString);
        }
        public void Generate(int num)
        {
            ItemRepository repo = new ItemRepository(this.connString);
            List<string> names = new List<string>();
            List<double> costs = new List<double>();
            List<bool> availabilities = new List<bool>();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"select chr(trunc(65+random()*25)::int) 
                || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) 
                    || chr(trunc(65+random()*25)::int) from generate_series(1,@num)";
            command.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                names.Add(reader.GetString(0));
            }
            connection.Close();
            connection.Open();
            NpgsqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "select trunc(random()*10000)::int from generate_series(1,@num)";
            command1.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader1 = command1.ExecuteReader();
            while(reader1.Read())
            {
                costs.Add(reader1.GetInt32(0));
            }
            Random rand = new Random();
            for(int i = 0; i < num; i++)
            {
                availabilities.Add(rand.Next(1, 100) > 50);
            }
            string[] nameRepo = new string[names.Count];
            double[] costRepo = new double[costs.Count];
            bool[] availRepo = new bool[availabilities.Count];
            names.CopyTo(nameRepo);
            costs.CopyTo(costRepo);
            availabilities.CopyTo(availRepo);
            for(int i = 0; i < num; i ++)
            {
                Item item = new Item(nameRepo[i], costRepo[i], availRepo[i]);
                repo.Insert(item);
            }
            connection.Close();
        }
    }
}
