namespace lab2
{
    partial class Program
    {
        static class Controller
        {
            public static bool CheckInteger(string value)
            {
                return int.TryParse(value, out int num);
            }
            public static bool CheckBoolean(string value)
            {
                return bool.TryParse(value, out bool boolean);
            }
        }
    }
}
