namespace Uniswap.GraphQL.Entities
{
    public class Position
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public Token Token0 { get; set; }
        public Token Token1 { get; set; }
        public Pool Pool { get; set; }
        public string Liquidity { get; set; }
        public Tick TickLower { get; set; }
        public Tick TickUpper { get; set; }
        public double Position0Amount { get; set; }
        public double Position1Amount { get; set; }
        public double Price0Amount { get; set; }
    }
}
