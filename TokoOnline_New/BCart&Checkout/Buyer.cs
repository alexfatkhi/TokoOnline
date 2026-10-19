
namespace TokoOnline_New.BCart_Checkout
{
    public class Buyer
    {
        public string BuyerId { get; private set; }
        public string Name { get; set; }
        public bool Member { get; set; }
        public int Points { get; private set; }
        

        public Buyer(string buyerId, string name, bool member)
        {
            BuyerId = buyerId;
            Name = name;
            Member = member;
            Points = 0;
        }

        public void AddPoints(int totalSpent)
        {
            if (Member == false)
            {
                return;
            }

            if (totalSpent <= 0)
            {
                return;
            }

            int earnedPoints = totalSpent / 10000;

            Points = Points + earnedPoints;
        }
    }
}
