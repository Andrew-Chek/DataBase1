namespace lab2
{
    public class Order
    {
        public int order_id;
        public int user_id;
        public User author;
        public double cost;
        public Order(int order_id, double cost, int user_id)
        {
            this.cost = cost;
            this.order_id = order_id;
            this.user_id = user_id;
        }
        public Order(double cost, int user_id)
        {
            this.cost = cost;
            this.user_id = user_id;
        }
        public Order()
        {
            this.cost = 0;
        }
        public override string ToString()
        {
            return $"Id: {this.order_id}, Cost: {this.cost}";
        }
    }
}
