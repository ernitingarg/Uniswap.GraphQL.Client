using System;
using Uniswap.GraphQL;
using Uniswap.GraphQL.Entities;

namespace Uniswap.Client
{
    class PriceHelper
    {
        public static (double amount0, double amount1) GetPositionAmounts(
            OwnerPosition owenrPosition)
        {
            var position = owenrPosition.Position;
            double positionLiquidity = double.Parse(position.Liquidity);
            var price = position.GetPrice();
            var priceSqrt = position.GetPriceSqrt();

            var amounts = CalculateAmounts(positionLiquidity, price, priceSqrt);

            var amount0Adjusted = DecimalAdjustment(
                amounts.amount0,
                position.Token0.Decimals,
                5);

            var amount1Adjusted = DecimalAdjustment(
                amounts.amount1,
                position.Token1.Decimals,
                2);

            return (amount0Adjusted, amount1Adjusted);
        }

        static (double amount0, double amount1) CalculateAmounts(
           double positionLiquidity,
           Price price,
           Price priceSqrt)
        {
            // The amount calculations using positionLiquidity & current, upper and lower priceSqrt
            double amount0;
            double amount1;
            if (price.CurrentPrice <= price.LowerPrice)
            {
                amount0 = positionLiquidity * (1 / priceSqrt.LowerPrice - 1 / priceSqrt.UpperPrice);
                amount1 = 0;
            }
            else if (price.CurrentPrice < price.UpperPrice)
            {
                amount0 = positionLiquidity * (1 / priceSqrt.CurrentPrice - 1 / priceSqrt.UpperPrice);
                amount1 = positionLiquidity * (priceSqrt.CurrentPrice - priceSqrt.LowerPrice);
            }
            else
            {
                amount1 = positionLiquidity * (priceSqrt.UpperPrice - priceSqrt.LowerPrice);
                amount0 = 0;
            }

            return (amount0, amount1);
        }

        static double DecimalAdjustment(
            double amount,
            string decimals,
            int precisions)
        {
            double adjustedAmount = amount / Math.Pow(10, int.Parse(decimals));
            return Math.Truncate(adjustedAmount * Math.Pow(10, precisions)) / Math.Pow(10, precisions);
        }
    }
}
