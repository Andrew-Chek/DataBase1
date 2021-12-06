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
            ItemRepository items = new ItemRepository(context);
            OrderRepository orders = new OrderRepository(context);
            UserRepository users = new UserRepository(context);
            ConsoleLog log = new ConsoleLog(items, orders, users);
            log.ProcessCommands();
        }
        public class ConsoleLog
        {
            private ItemRepository items;
            private OrderRepository orders;
            private UserRepository users;
            public ConsoleLog(ItemRepository items, OrderRepository orders, UserRepository users)
            {
                this.items = items;
                this.orders = orders;
                this.users = users;
            }
            public void ProcessCommands()
            {
                while (true)
                {
                    Write("Enter a command: ");
                    string command = ReadLine();
                    if (command.StartsWith('g'))
                    {
                        string numVal = GetValue(command, 2);
                        int id;
                        if (int.TryParse(numVal, out id))
                        {
                            id = int.Parse(numVal);
                            if (command[1] == 'i')
                            {
                                ProcessGetItem(id);
                            }
                            else if (command[1] == 'o')
                            {
                                ProcessGetOrder(id);
                            }
                            else if (command[1] == 'u')
                            {
                                ProcessGetUser(id);
                            }
                            else
                            {
                                WriteLine("Unknown command.");
                            }
                        }
                        else
                        {
                            WriteLine("Id should be a number");
                        }
                    }
                    else if (command == "dio")
                    {
                        int[] array = SetOrdItem(orders, items);
                        ProcessDelOrderItem(array[0], array[1]);
                    }
                    else if (command.StartsWith('d'))
                    {
                        string numVal = GetValue(command, 2);
                        if (int.TryParse(numVal, out int num))
                        {
                            int id = int.Parse(numVal);
                            if (command[1] == 'i')
                            {
                                ProcessDelItem(id);
                            }
                            else if (command[1] == 'o')
                            {
                                ProcessDelOrder(id);
                            }
                            else if (command[1] == 'u')
                            {
                                ProcessDelUser(id);
                            }
                            else
                            {
                                WriteLine("Unknown command.");
                            }
                        }
                        else
                        {
                            WriteLine("Id should be a number");
                        }
                    }
                    else if (command.StartsWith('i'))
                    {
                        if (command[1] == 'i')
                        {
                            if (command.Length == 2)
                            {
                                Item item = FillItem();
                                WriteLine($"Id of new item is: {items.Insert(item)}");
                            }
                            else if (command[2] == 'o')
                            {
                                int[] values = SetOrdItem(orders, items);
                                WriteLine($"New item to order {orders.InsertOrderItems(values[0], values[1])} was succesfully added");
                            }
                            else
                            {
                                WriteLine("Unknown command.");
                            }
                        }
                        else if (command[1] == 'o')
                        {
                            Order order = FillOrder(users);
                            WriteLine($"Id of new order is: {orders.Insert(order)}");
                        }
                        else if (command[1] == 'u')
                        {
                            User user = FillUser();
                            WriteLine($"Id of new user is: {users.Insert(user)}");
                        }
                        else
                        {
                            WriteLine("Unknown command.");
                        }
                    }
                    else if (command.StartsWith('u'))
                    {
                        string numVal = GetValue(command, 2);
                        if (int.TryParse(numVal, out int num))
                        {
                            int id = int.Parse(numVal);
                            if (command[1] == 'i')
                            {
                                Item item = items.GetById(id);
                                Item newItem = SetItem(item);
                                WriteLine($"Was item updated successfully? - {items.Update(newItem)}");
                            }
                            else if (command[1] == 'o')
                            {
                                Order order = orders.GetById(id);
                                Order newOrder = SetOrder(users, order);
                                WriteLine($"Was order updated successfully? - {orders.Update(newOrder)}");
                            }
                            else if (command[1] == 'u')
                            {
                                User user = users.GetById(id);
                                User newUser = SetUser(user);
                                WriteLine($"Was user updated successfully? - {users.Update(newUser)}");
                            }
                            else
                            {
                                WriteLine("Unknown command.");
                            }
                        }
                        else
                        {
                            WriteLine("Id should be a number");
                        }
                    }
                    else if (command.Contains("search"))
                    {
                        Write("Enter a search subline: ");
                        string value = ReadLine();
                        int[] measures1;
                        while (true)
                        {
                            Write("Enter measures for cost for order and item: ");
                            string measure1 = ReadLine();
                            measures1 = GetMeasures(measure1);
                            if (measures1[0] != measures1[1])
                            {
                                break;
                            }
                            else
                            {
                                WriteLine("Wrong measures, please enter again!");
                            }
                        }
                        int[] measures2;
                        while (true)
                        {
                            Write("Enter measures for us_id for order and item: ");
                            string measure2 = ReadLine();
                            measures2 = GetMeasures(measure2);
                            if (measures2[0] != measures2[1])
                            {
                                break;
                            }
                            else
                            {
                                WriteLine("Wrong measures, please enter again!");
                            }
                        }
                        if(command.Contains("items"))
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            ProcessSearchItems(value, measures1);
                            sw.Stop();
                        }
                        else if(command.Contains("orders"))
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            ProcessSearchOrders(measures1, measures2);
                            sw.Stop();
                        }
                        else if(command.Contains("users"))
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            ProcessSearchUsers(value);
                            sw.Stop();
                        }
                        else
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            ProcessSearchItems(value, measures1);
                            ProcessSearchOrders(measures1, measures2);
                            ProcessSearchUsers(value);
                            sw.Stop();
                            WriteLine($"Elapsed time for all search is: {sw.Elapsed}");
                        }
                    }
                    else if (command == "exit" || command == "")
                    {
                        WriteLine("Bye.");
                        break;
                    }
                    else
                    {
                        WriteLine("Unknown command.");
                    }
                }
            }
            public void ProcessSearchUsers(string value)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Write("Serch Users: ");
                List<User> searchUsrs = users.GetAllSearch(value);
                User[] userArr = new User[searchUsrs.Count];
                searchUsrs.CopyTo(userArr);
                if (userArr.Length == 0)
                {
                    Write("no in this request");
                }
                WriteLine();
                for (int i = 0; i < userArr.Length; i++)
                {
                    WriteLine(userArr[i].ToString());
                }
                sw.Stop();
                WriteLine($"Elapsed time for user search is: {sw.Elapsed}");
            }
            public void ProcessSearchItems(string value, int[] measures1)
            {
                bool av;
                while (true)
                {
                    Write("Enter bool value for item: ");
                    string boolean = ReadLine();
                    if (bool.TryParse(boolean, out bool b))
                    {
                        av = bool.Parse(boolean);
                        break;
                    }
                    else
                    {
                        WriteLine("Wrong value, please enter again!");
                    }
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Write("Serch Items: ");
                List<Item> searchIts = items.GetAllSearch(value, av, measures1);
                Item[] itArr = new Item[searchIts.Count];
                searchIts.CopyTo(itArr);
                if (itArr.Length == 0)
                {
                    Write("no in this request");
                }
                WriteLine();
                for (int i = 0; i < itArr.Length; i++)
                {
                    WriteLine(itArr[i].ToString());
                }
                sw.Stop();
                WriteLine($"Elapsed time for item search is: {sw.Elapsed}");
            }
            public void ProcessSearchOrders(int[] measures1, int[] measures2)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                List<Order> searchOrds = orders.GetAllSearch(measures2, measures1);
                Order[] ordArr = new Order[searchOrds.Count];
                searchOrds.CopyTo(ordArr);
                Write("Serch Orders: ");
                if (ordArr.Length == 0)
                {
                    Write("no in this request");
                }
                WriteLine();
                for (int i = 0; i < ordArr.Length; i++)
                {
                    WriteLine(ordArr[i].ToString());
                }
                sw.Stop();
                WriteLine($"Elapsed time for order search is: {sw.Elapsed}");
            }
            public void ProcessGetItem(int id)
            {
                try
                {
                    Item item = items.GetById(id);
                    WriteLine(item.ToString());
                }
                catch
                {
                    WriteLine("item id isn`t correct");
                }
            }
            public void ProcessGetOrder(int id)
            {
                try
                {
                    Order order = orders.GetById(id);
                    WriteLine(order.ToString());
                }
                catch
                {
                    WriteLine("order id isn`t correct");
                }
            }
            public void ProcessGetUser(int id)
            {
                try
                {
                    try
                    {
                        User user = users.GetById(id);
                        WriteLine(user.ToString());
                    }
                    catch (Exception ex)
                    {
                        WriteLine(ex.Message);
                    }
                }
                catch
                {
                    WriteLine("user id isn`t correct");
                }
            }
            public void ProcessDelItem(int id)
            {
                try
                {
                    WriteLine($"Item was deleted? - {items.Delete(id)}");
                }
                catch
                {
                    WriteLine("item id isn`t correct");
                }
            }
            public void ProcessDelOrder(int id)
            {
                try
                {
                    WriteLine($"Order was deleted? - {orders.Delete(id)}");
                }
                catch
                {
                    WriteLine("order id isn`t correct");
                }
            }
            public void ProcessDelOrderItem(int ord_id, int item_id)
            {
                try
                {
                    int nChanged = orders.DeleteByOrdersItems(ord_id, item_id);
                    WriteLine($"Item from order was deleted? - {nChanged == 1}");
                }
                catch
                {
                    WriteLine("One of ids isn`t correct");
                }
            }
            public void ProcessDelUser(int id)
            {
                try
                {
                    WriteLine($"User was deleted? - {users.Delete(id)}");
                }
                catch
                {
                    WriteLine("user id isn`t correct");
                }
            }
        }
        static int[] GetMeasures(string value)
        {
            int[] arr = new int[2];
            string[] array = value.Split(',');
            if (array.Length != 2)
            {
                return arr;
            }
            if (int.TryParse(array[0], out arr[0]) && int.TryParse(array[1], out arr[1])
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
                item.Name = ReadLine();
                while (true)
                {
                    Write("Enter a cost of inserting item: ");
                    costVal = ReadLine();
                    if (double.TryParse(costVal, out double num))
                    {
                        item.Cost = double.Parse(costVal);
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
                    if (bool.TryParse(avVal, out bool value))
                    {
                        item.Availability = bool.Parse(avVal);
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
                    item.Name = name;
                }
                while (true)
                {
                    Write("Enter a cost of updating item: ");
                    costVal = ReadLine();
                    if (costVal == "")
                    {
                        break;
                    }
                    if (int.TryParse(costVal, out int num))
                    {
                        item.Cost = int.Parse(costVal);
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
                    if (bool.TryParse(avVal, out bool value))
                    {
                        item.Availability = bool.Parse(avVal);
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
                    if (int.TryParse(costVal, out int num))
                    {
                        order.Cost = int.Parse(costVal);
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
                    if (int.TryParse(us_idVal, out int num))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.UsId = us_id;
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
                if (int.TryParse(vArr[0], out int num1) && int.TryParse(vArr[1], out int num2))
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
                    if (int.TryParse(costVal, out int num))
                    {
                        order.Cost = int.Parse(costVal);
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
                    if (int.TryParse(us_idVal, out int num))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.UsId = us_id;
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
            Write("Enter a name of inserting user: ");
            user.Name = ReadLine();
            Write("Enter a password for inserting user: ");
            user.Password = ReadLine();
            return user;
        }
        static User SetUser(User user)
        {
            Write("Enter a name of updating user: ");
            string name = ReadLine();
            if (name != "")
            {
                user.Name = name;
            }
            Write("Enter a password for updating user: ");
            string password = ReadLine();
            if (password != "")
            {
                user.Password = password;
            }
            return user;
        }
    }
}