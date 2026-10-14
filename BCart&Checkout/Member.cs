using Project_OOP;
using System;

namespace ProjectOOP_TokoOnline
{
    public class Member : Customer
    {
        public int Point { get; private set; }

        public Member(string customerName, string address)
            : base(customerName, address)
        {
            Point = 0;
        }

        // Member checkout = normal checkout + earned points.
        public override void Checkout(Cart cart)
        {
            if (cart.IsEmpty())
            {
                base.Checkout(cart);
                return;
            }

            // Capture the total before the base checkout empties the cart.
            decimal total = cart.GetTotal();

            base.Checkout(cart);

            int earned = CalculatePoint(total);
            Point += earned;

            Console.WriteLine($"Points earned: {earned}");
            Console.WriteLine($"Total points: {Point}");
        }

        // 1 point for every Rp10.000 spent.
        public int CalculatePoint(decimal total)
        {
            return (int)(total / 10000m);
        }
    }
}
