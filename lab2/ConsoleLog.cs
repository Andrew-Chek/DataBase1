using static System.Console;
using System.Collections.Generic;
using System.Diagnostics;
namespace lab2
{
    partial class Program
    {
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
                        if (Controller.CheckInteger(numVal))
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
                            if(command.Length == 2)
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
                        if (Controller.CheckInteger(numVal))
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
                    else if (command.StartsWith('s'))
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
                        bool av;
                        while (true)
                        {
                            Write("Enter bool value for item: ");
                            string boolean = ReadLine();
                            if (Controller.CheckBoolean(boolean))
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
                        WriteLine($"Elapsed time for search is: {sw.Elapsed}");
                    }
                    else if (command == "Gall")
                    {
                        int num;
                        while (true)
                        {
                            Write("Enter a num of generated values: ");
                            string value = ReadLine();
                            if (Controller.CheckInteger(value) && int.Parse(value) > 0)
                            {
                                num = int.Parse(value);
                                break;
                            }
                            else
                            {
                                WriteLine("Number wasn`t correct, please enter again!");
                            }
                        }
                        generation.GenerateItems(num);
                        generation.GenerateOrders(num);
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
                    orders.DeleteByIdOrdersItems(id);
                    WriteLine($"Order was deleted? - {nChanged == 1}");
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
    }
}
