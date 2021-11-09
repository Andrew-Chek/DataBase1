using System;
using System.Collections.Generic;
using Npgsql;
namespace lab2
{
    public class Generation
    {
        private NpgsqlConnection connection;
        private ItemRepository items;
        private OrderRepository orders;
        private UserRepository users;
        public Generation(string connString, ItemRepository items, OrderRepository orders, UserRepository users)
        {
            this.connection = new NpgsqlConnection(connString);
            this.items = items;
            this.orders = orders;
            this.users = users;
        }
        public void GenerateItems(int num)
        {
            string[] names = GenerateStrings(num);
            double[] costs = GenerateDoubles(num);
            Random rand = new Random();
            bool[] availRepo = new bool[num];
            for(int i = 0; i < num; i++)
            {
                availRepo[i] = rand.Next(1, 100) > 50;
            }
            for(int i = 0; i < num; i ++)
            {
                Item item = new Item(names[i], costs[i], availRepo[i]);
                items.Insert(item);
            }
        }
        public void GenerateOrders(int num)
        {
            int[] us_ids = GenerateUsers(num);
            double[] costs = GenerateDoubles(num);
            for(int i = 0; i < num; i ++)
            {
                Order order = new Order(costs[i], us_ids[i]);
                orders.Insert(order);
            }
        }
        public string[] GenerateStrings(int num)
        {
            List<string> strings = new List<string>();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"select chr(trunc(65+random()*25)::int) 
                || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) 
                    || chr(trunc(65+random()*25)::int) from generate_series(1,@num)";
            command.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                strings.Add(reader.GetString(0));
            }
            connection.Close();
            string[] stringRepo = new string[strings.Count];
            strings.CopyTo(stringRepo);
            return stringRepo;
        }
        public double[] GenerateDoubles(int num)
        {
            List<double> doubles = new List<double>();
            connection.Open();
            NpgsqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "select trunc(random()*100000)::int from generate_series(1,@num)";
            command1.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader1 = command1.ExecuteReader();
            while(reader1.Read())
            {
                doubles.Add(reader1.GetInt32(0));
            }
            connection.Close();
            double[] doubRepo = new double[doubles.Count];
            doubles.CopyTo(doubRepo);
            return doubRepo;
        }
        public int[] GenerateUsers(int num)
        {
            int[] array = new int[num];
            string[] names = GenerateStrings(num);
            string[] passwords = GenerateStrings(num);
            for(int i = 0; i < num; i ++)
            {
                User user = new User(names[i], passwords[i]);
                array[i] = (int)users.Insert(user);
            }
            return array;
        }
    }
}
