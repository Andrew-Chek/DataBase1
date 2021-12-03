using System;
using static System.Console;

namespace lab3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            postgresContext context = new postgresContext();
            ItemRepository repo = new ItemRepository(context);
            Item item = new Item(12000, true, "Inserted item");
            WriteLine(repo.Insert(item));
        }
    }
}
