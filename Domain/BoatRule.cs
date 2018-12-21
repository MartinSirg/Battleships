namespace Domain
{
    public class BoatRule
    {
        public int BoatRuleId { get; set; }
        
        public int Size { get; set; }
        public int Quantity { get; set; }

        public BoatRule(int size, int quantity)
        {
            Size = size;
            Quantity = quantity;
        }
    }
}