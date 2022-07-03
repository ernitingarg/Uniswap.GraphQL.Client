using System;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    internal static class PositionExtensions
    {
        internal static Price GetPrice(this Position position)
        {
            // Prices (not decimal adjusted)

            double priceCurrentSqrt = double.Parse(position.Pool.SqrtPrice) / Math.Pow(2, 96);
            double priceCurrent = Math.Pow(priceCurrentSqrt, 2);
            double priceUpper = double.Parse(position.TickUpper.Price0);
            double priceLower = double.Parse(position.TickLower.Price0);

            return new Price(priceCurrent, priceLower, priceUpper);
        }

        internal static Price GetPriceSqrt(this Position position)
        {
            // Square roots of the prices (not decimal adjusted)

            double priceCurrentSqrt = double.Parse(position.Pool.SqrtPrice) / Math.Pow(2, 96);
            double priceUpperSqrt = Math.Sqrt(double.Parse(position.TickUpper.Price0));
            double priceLowerSqrt = Math.Sqrt(double.Parse(position.TickLower.Price0));

            return new Price(priceCurrentSqrt, priceLowerSqrt, priceUpperSqrt);
        }
    }

    internal class Price
    {
        public Price(
            double currentPrice,
            double lowerPrice,
            double upperPrice)
        {
            CurrentPrice = currentPrice;
            LowerPrice = lowerPrice;
            UpperPrice = upperPrice;
        }

        public double CurrentPrice { get; }
        public double LowerPrice { get; }
        public double UpperPrice { get; }
    }
}
