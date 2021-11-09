using System;
using Npgsql;
using System.Collections.Generic;
namespace lab2
{
    public class UserRepository
    {
        private NpgsqlConnection connection;
        public UserRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public User GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE us_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                User user = new User();
                user.user_id = reader.GetInt32(0);
                user.name = reader.GetString(1);
                user.password = reader.GetString(2);
                connection.Close();
                return user;
            }
            else 
            {
                connection.Close();
                throw new Exception("there is no user with such id");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE us_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(User user)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (name, password) 
                VALUES (@name, @password) RETURNING us_id;
            ";
            command.Parameters.AddWithValue("name", user.name);
            command.Parameters.AddWithValue("password", user.password);
            object newId = command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(User user)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET name = @name, password = @password WHERE us_id = @user_id";
            command.Parameters.AddWithValue("name", user.name);
            command.Parameters.AddWithValue("password", user.password);
            command.Parameters.AddWithValue("user_id", user.user_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public long GetCount()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public long GetSearchCount(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return this.GetCount();
            }
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users WHERE name LIKE '%' || @value || '%' 
                AND password LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public List<User> GetAllSearch(string value)
        {
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
                user.user_id = reader.GetInt32(0);
                user.name = reader.GetString(1);
                user.password = reader.GetString(2);
                list.Add(user);
            }
            connection.Close();
            return list;
        }
    }
}
