using System;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    internal class PriceHelper
    {
        internal static (double Position0, double Position1) GetPositionAmounts(
            LiquidityPosition liquidityPosition,
            int? precisionForPosition0 = null,
            int? precisionForPosition1 = null)
        {
            var position = liquidityPosition.Position;
            double positionLiquidity = double.Parse(position.Liquidity);
            var price = position.GetPrice();
            var priceSqrt = position.GetPriceSqrt();

            var positionAmounts = CalculatePositionAmounts(positionLiquidity, price, priceSqrt);

            var position0AdjustedAmount = DecimalTruncateAdjustment(
                positionAmounts.amount0,
                position.Token0.Decimals,
                precisionForPosition0);

            var position1AdjustedAmount = DecimalTruncateAdjustment(
                positionAmounts.amount1,
                position.Token1.Decimals,
                precisionForPosition1);

            return (position0AdjustedAmount, position1AdjustedAmount);
        }

        internal static (double Price0, double Price1) GetPriceAmounts(
            LiquidityPosition liquidityPosition,
            int? precisionForPrice0 = null)
        {
            var position = liquidityPosition.Position;
            var price0 = position.GetTokenPrice();

            var price0AdjustedAmount = RoundAdjustment(
                price0,
                precisionForPrice0);

            return (price0AdjustedAmount, double.NaN);
        }

        static (double amount0, double amount1) CalculatePositionAmounts(
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

        static double DecimalTruncateAdjustment(
            double amount,
            string decimals,
            int? precisions)
        {
            double adjustedAmount = amount / Math.Pow(10, int.Parse(decimals));

            if (precisions.HasValue)
            {
                return Math.Truncate(
                    adjustedAmount * Math.Pow(10, precisions.Value))
                       / Math.Pow(10, precisions.Value);
            }

            return adjustedAmount;
        }

        static double RoundAdjustment(
            double amount,
            int? precisions)
        {
            if (precisions.HasValue)
            {
                return Math.Round(
                           amount * Math.Pow(10, precisions.Value))
                       / Math.Pow(10, precisions.Value);
            }

            return amount;
        }
    }
}
