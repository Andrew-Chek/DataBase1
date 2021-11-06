using static System.Console;
namespace lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc";
            ItemRepository repo = new ItemRepository(connString);
            Generation gen = new Generation(connString);
        }
    }
}
