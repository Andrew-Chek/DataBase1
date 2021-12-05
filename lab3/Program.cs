using System;
using static System.Console;
using System.Diagnostics;

namespace lab3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            postgresContext context = new postgresContext();
            ItemRepository repo = new ItemRepository(context);
            //Item item = new Item(12000, true, "Inserted item");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            repo.GetAllSearch("B", new int[]{100, 10000});
            sw.Stop();
            WriteLine(sw.Elapsed);
            //WriteLine(repo.Insert(item));
            //1.609
        }
    }
}
