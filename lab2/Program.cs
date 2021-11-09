using static System.Console;
using System;
namespace lab2
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc";
            ItemRepository items = new ItemRepository(connString);
            OrderRepository orders = new OrderRepository(connString);
            UserRepository users = new UserRepository(connString);
            Generation gen = new Generation(connString, items, orders, users);
            ConsoleLog log = new ConsoleLog(items, orders, users, gen);
            string begText = "This program offers you a set of commands to manipulate with my database, here all of them:\r\n";
            string commandText = "`gi12` - get item by 12 id; (the same for order(go12) and user(gu12));\r\n`di12` - delete item by 12 id(order- do12, user - du12);\r\n";
            string commandText1 = "`ii` - insert item with next logs for input (the same for order(io) and user(iu));\r\n`ui12` - updating item by 12 id (for order - uo, user - uu)";
            string commandText2 = "\r\nFor searching - `s` with next logs for search parameters;\r\nFor generating - `Gall` with next log for the number of generated values";
            WriteLine(begText + commandText + commandText1 + commandText2);
            log.ProcessCommands(connString);
        }
        static int[] GetMeasures(string value)
        {
            int[] arr = new int[2];
            string[] array = value.Split(',');
            if (array.Length != 2)
            {
                return arr;
            }
            if (Controller.CheckInteger(array[0]) && Controller.CheckInteger(array[1])
                && int.Parse(array[1]) > int.Parse(array[0]))
            {
                arr[0] = int.Parse(array[0]);
                arr[1] = int.Parse(array[1]);
            }
            return arr;
        }
        static string GetValue(string command, int index)
        {
            string value = "";
            for (int i = index; i < command.Length; i++)
            {
                value += command[i];
            }
            return value;
        }
        static Item FillItem()
        {
            Item item = new Item();
            string costVal = "";
            string avVal = "";
            while (true)
            {
                Write("Enter a name of inserting item: ");
                item.name = ReadLine();
                while (true)
                {
                    Write("Enter a cost of inserting item: ");
                    costVal = ReadLine();
                    if (Controller.CheckInteger(costVal))
                    {
                        item.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while (true)
                {
                    Write("Enter an availability of inserting item: ");
                    avVal = ReadLine();
                    if (Controller.CheckBoolean(avVal))
                    {
                        item.availability = bool.Parse(avVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Availibility is wrong, enter again!");
                    }
                }
                break;
            }
            return item;
        }
        static Item SetItem(Item item)
        {
            string costVal = "";
            string avVal = "";
            while (true)
            {
                Write("Enter a name of updating item: ");
                string name = ReadLine();
                if (name != "")
                {
                    item.name = name;
                }
                while (true)
                {
                    Write("Enter a cost of updating item: ");
                    costVal = ReadLine();
                    if (costVal == "")
                    {
                        break;
                    }
                    if (Controller.CheckInteger(costVal))
                    {
                        item.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while (true)
                {
                    Write("Enter an availability of inserting item: ");
                    avVal = ReadLine();
                    if (avVal == "")
                    {
                        break;
                    }
                    if (Controller.CheckBoolean(avVal))
                    {
                        item.availability = bool.Parse(avVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Availibility is wrong, enter again!");
                    }
                }
                break;
            }
            return item;
        }
        static Order FillOrder(UserRepository users)
        {
            Order order = new Order();
            string costVal = "";
            string us_idVal = "";
            while (true)
            {
                while (true)
                {
                    Write("Enter a cost of inserting order: ");
                    costVal = ReadLine();
                    if (Controller.CheckInteger(costVal))
                    {
                        order.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while (true)
                {
                    Write("Enter an id of user of inserting order: ");
                    us_idVal = ReadLine();
                    if (Controller.CheckInteger(us_idVal))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.user_id = us_id;
                            break;
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        WriteLine("Us_id is wrong, enter again!");
                    }
                }
                break;
            }
            return order;
        }
        static int[] SetOrdItem(OrderRepository orders, ItemRepository items)
        {
            int[] array = new int[2];
            string message = "";
            while (true)
            {
                Write("Enter order_id and item_id: ");
                message = ReadLine();
                string[] vArr = message.Split(',');
                if (Controller.CheckInteger(vArr[0]) && Controller.CheckInteger(vArr[1]))
                {
                    try
                    {
                        array[0] = int.Parse(vArr[0]);
                        array[1] = int.Parse(vArr[1]);
                        orders.GetById(array[0]);
                        items.GetById(array[1]);
                        break;
                    }
                    catch
                    {
                        WriteLine("There are no items and orders by these ids");
                    }
                }
                else
                {
                    WriteLine("Values are wrong, enter again!");
                }
            }
            return array;
        }
        static Order SetOrder(UserRepository users, Order order)
        {
            string costVal = "";
            string us_idVal = "";
            while (true)
            {
                while (true)
                {
                    Write("Enter a cost of updating order: ");
                    costVal = ReadLine();
                    if (costVal == "")
                    {
                        break;
                    }
                    if (Controller.CheckInteger(costVal))
                    {
                        order.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while (true)
                {
                    Write("Enter an id of user of updating order: ");
                    us_idVal = ReadLine();
                    if (us_idVal == "")
                    {
                        break;
                    }
                    if (Controller.CheckInteger(us_idVal))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.user_id = us_id;
                            break;
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        WriteLine("Us_id is wrong, enter again!");
                    }
                }
                break;
            }
            return order;
        }
        static User FillUser()
        {
            User user = new User();
            while (true)
            {
                Write("Enter a name of inserting user: ");
                user.name = ReadLine();
                Write("Enter a password for inserting user: ");
                user.password = ReadLine();
                break;
            }
            return user;
        }
        static User SetUser(User user)
        {
            while (true)
            {
                Write("Enter a name of updating user: ");
                string name = ReadLine();
                if (name != "")
                {
                    user.name = name;
                }
                Write("Enter a password for updating user: ");
                string password = ReadLine();
                if (password != "")
                {
                    user.password = password;
                }
                break;
            }
            return user;
        }
    }
}
