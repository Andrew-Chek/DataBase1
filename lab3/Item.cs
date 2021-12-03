namespace lab3
{
    public class Item
    {
        public int item_id;
        public string name;
        public double cost;
        public bool availability;
        public Item(string name, double cost, bool availability)
        {
            this.name = name;
            this.cost = cost;
            this.availability = availability;
        }
        public Item(int goods_id, string name, double cost, bool availability)
        {
            this.name = name;
            this.cost = cost;
            this.item_id = goods_id;
            this.availability = availability;
        }
        public Item()
        {
            this.name = "";
            this.cost = 0;
        }
        public override string ToString()
        {
            return $"Id: {this.item_id}, Name: {this.name}";
        }
    }
}
