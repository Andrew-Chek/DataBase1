using static System.Console;
using System.Collections.Generic;
using System;
namespace lab2
{
    class Program
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
        public class ConsoleLog
        {
            private ItemRepository items;
            private OrderRepository orders;
            private UserRepository users;
            private Generation generation;
            public ConsoleLog(ItemRepository items, OrderRepository orders, UserRepository users, Generation generation)
            {
                this.items = items;
                this.orders = orders;
                this.users = users;
                this.generation = generation;
            }
            public void ProcessCommands(string connString)
            {
                while (true)
                {
                    Write("Enter a command: ");
                    string command = ReadLine();
                    if (command.StartsWith('g'))
                    {
                        string numVal = GetValue(command, 2);
                        int id;
                        if(int.TryParse(numVal, out id))
                        {
                            id = int.Parse(numVal);
                            if(command[1] == 'i')
                            {
                                ProcessGetItem(id);
                            }
                            else if(command[1] == 'o')
                            {
                                ProcessGetOrder(id);
                            }
                            else if(command[1] == 'u')
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
                    else if(command.StartsWith('d'))
                    {
                        string numVal = GetValue(command, 2);
                        if(Controller.CheckInteger(numVal))
                        {
                            int id = int.Parse(numVal);
                            if(command[1] == 'i')
                            {
                                ProcessDelItem(id);
                            }
                            else if(command[1] == 'o')
                            {
                                ProcessDelOrder(id);
                            }
                            else if(command[1] == 'u')
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
                    else if(command.StartsWith('i'))
                    {
                        if(command[1] == 'i')
                        {
                            Item item = FillItem();
                            WriteLine($"Id of new item is: {items.Insert(item)}");
                        }
                        else if(command[1] == 'o')
                        {
                            Order order = FillOrder(users);
                            WriteLine($"Id of new order is: {orders.Insert(order)}");
                        }
                        else if(command[1] == 'u')
                        {
                            User user = FillUser();
                            WriteLine($"Id of new user is: {users.Insert(user)}");
                        }
                        else
                        {
                            WriteLine("Unknown command.");
                        }
                    }
                    else if(command.StartsWith('u'))
                    {
                        string numVal = GetValue(command, 2);
                        if(Controller.CheckInteger(numVal))
                        {
                            int id = int.Parse(numVal);
                            if(command[1] == 'i')
                            {
                                Item item = items.GetById(id);
                                Item newItem = SetItem(item);
                                WriteLine($"Was item updated successfully? - {items.Update(newItem)}");
                            }
                            else if(command[1] == 'o')
                            {
                                Order order = orders.GetById(id);
                                Order newOrder = SetOrder(users, order);
                                WriteLine($"Was order updated successfully? - {orders.Update(newOrder)}");
                            }
                            else if(command[1] == 'u')
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
                    else if(command.StartsWith('s'))
                    {
                        Write("Enter a search subline: ");
                        string value = ReadLine();
                        int[] measures1;
                        while(true)
                        {
                            Write("Enter measures for cost for order and item: ");
                            string measure1 = ReadLine();
                            measures1 = GetMeasures(measure1);
                            if(measures1[0] != measures1[1])
                            {
                                break;
                            }
                            else
                            {
                                WriteLine("Wrong measures, please enter again!");
                            }
                        }
                        int[] measures2;
                        while(true)
                        {
                            Write("Enter measures for us_id for order and item: ");
                            string measure2 = ReadLine();
                            measures2 = GetMeasures(measure2);
                            if(measures2[0] != measures2[1])
                            {
                                break;
                            }
                            else
                            {
                                WriteLine("Wrong measures, please enter again!");
                            }
                        }
                        bool av;
                        while(true)
                        {
                            Write("Enter bool value for item: ");
                            string boolean = ReadLine();
                            if(Controller.CheckBoolean(boolean))
                            {
                                av = bool.Parse(boolean);
                                break;
                            }
                            else
                            {
                                WriteLine("Wrong value, please enter again!");
                            }
                        }
                        Write("Serch Items: ");
                        List<Item> searchIts = items.GetAllSearch(value, av, measures1);
                        Item[] itArr = new Item[searchIts.Count];
                        searchIts.CopyTo(itArr);
                        if(itArr.Length == 0)
                        {
                            Write("no in this request");
                        }
                        WriteLine();
                        for(int i = 0; i < itArr.Length; i++)
                        {
                            WriteLine(itArr[i].ToString());
                        }
                        List<Order> searchOrds = orders.GetAllSearch(measures2, measures1);
                        Order[] ordArr = new Order[searchOrds.Count];
                        searchOrds.CopyTo(ordArr);
                        Write("Serch Orders: ");
                        if(ordArr.Length == 0)
                        {
                            Write("no in this request");
                        }
                        WriteLine();
                        for(int i = 0; i < ordArr.Length; i++)
                        {
                            WriteLine(ordArr[i].ToString());
                        }
                        Write("Serch Users: ");
                        List<User> searchUsrs = users.GetAllSearch(value);
                        User[] userArr = new User[searchUsrs.Count];
                        searchUsrs.CopyTo(userArr);
                        if(userArr.Length == 0)
                        {
                            Write("no in this request");
                        }
                        WriteLine();
                        for(int i = 0; i < userArr.Length; i++)
                        {
                            WriteLine(userArr[i].ToString());
                        }
                    }
                    else if(command == "exit" || command == "")
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
                    User user = users.GetById(id);
                    WriteLine(user.ToString());
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
                    orders.DeleteByIdOrdersItems(id);
                    int nChanged = items.DeleteById(id);
                    WriteLine($"Item was deleted? - {nChanged == 1}");
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
                    int nChanged = orders.DeleteById(id);
                    WriteLine($"Order was deleted? - {nChanged == 1}");
                }
                catch
                {
                    WriteLine("order id isn`t correct");
                }
            }
            public void ProcessDelUser(int id)
            {
                try
                {
                    orders.DeleteByUserId(id);
                    int nChanged = users.DeleteById(id);
                    WriteLine($"User was deleted? - {nChanged == 1}");
                }
                catch
                {
                    WriteLine("user id isn`t correct");
                }
            }
        }
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2003Lipovetc";
            ItemRepository items = new ItemRepository(connString);
            OrderRepository orders = new OrderRepository(connString);
            UserRepository users = new UserRepository(connString);
            Generation gen = new Generation(connString);
            ConsoleLog log = new ConsoleLog(items, orders, users, gen);
            log.ProcessCommands(connString);
        }
        static int[] GetMeasures(string value)
        {
            int[] arr = new int[2];
            string[] array = value.Split(',');
            if(array.Length != 2)
            {
                return arr;
            }
            if(Controller.CheckInteger(array[0]) && Controller.CheckInteger(array[1]) 
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
            for(int i = index; i < command.Length; i++)
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
            while(true)
            {
                Write("Enter a name of inserting item: ");
                item.name = ReadLine();
                while(true)
                {
                    Write("Enter a cost of inserting item: ");
                    costVal = ReadLine();
                    if(Controller.CheckInteger(costVal))
                    {
                        item.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while(true)
                {
                    Write("Enter an availability of inserting item: ");
                    avVal = ReadLine();
                    if(Controller.CheckBoolean(avVal))
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
            while(true)
            {
                Write("Enter a name of updating item: ");
                string name = ReadLine();
                if(name != "")
                {
                    item.name = name;
                }
                while(true)
                {
                    Write("Enter a cost of updating item: ");
                    costVal = ReadLine();
                    if(costVal == "")
                    {
                        break;
                    }
                    if(Controller.CheckInteger(costVal))
                    {
                        item.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while(true)
                {
                    Write("Enter an availability of inserting item: ");
                    avVal = ReadLine();
                    if(avVal == "")
                    {
                        break;
                    }
                    if(Controller.CheckBoolean(avVal))
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
            while(true)
            {
                while(true)
                {
                    Write("Enter a cost of inserting order: ");
                    costVal = ReadLine();
                    if(Controller.CheckInteger(costVal))
                    {
                        order.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while(true)
                {
                    Write("Enter an id of user of inserting order: ");
                    us_idVal = ReadLine();
                    if(Controller.CheckInteger(us_idVal))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.user_id = us_id;
                            break;
                        }
                        catch(Exception ex)
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
        static Order SetOrder(UserRepository users, Order order)
        {
            string costVal = "";
            string us_idVal = "";
            while(true)
            {
                while(true)
                {
                    Write("Enter a cost of updating order: ");
                    costVal = ReadLine();
                    if(costVal == "")
                    {
                        break;
                    }
                    if(Controller.CheckInteger(costVal))
                    {
                        order.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while(true)
                {
                    Write("Enter an id of user of updating order: ");
                    us_idVal = ReadLine();
                    if(us_idVal == "")
                    {
                        break;
                    }
                    if(Controller.CheckInteger(us_idVal))
                    {
                        int us_id = int.Parse(us_idVal);
                        try
                        {
                            users.GetById(us_id);
                            order.user_id = us_id;
                            break;
                        }
                        catch(Exception ex)
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
            while(true)
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
            while(true)
            {
                Write("Enter a name of updating user: ");
                string name = ReadLine();
                if(name != "")
                {
                    user.name = name;
                }
                Write("Enter a password for updating user: ");
                string password = ReadLine();
                if(password != "")
                {
                    user.password = password;
                }
                break;
            }
            return user;
        }
    }
}
