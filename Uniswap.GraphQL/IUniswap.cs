using System.Threading.Tasks;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    public interface IUniswap
    {
        /// <summary>
        /// Get liquidity position for a given pool id
        /// </summary>
        /// <param name="poolId">pool id</param>
        /// <param name="precision0">decimal precision for amount0</param>
        /// <param name="precision1">decimal precision for amount1</param>
        /// <returns></returns>
        Task<LiquidityPosition> GetLiquidityPosition(
            int poolId,
            int? precision0 = null,
            int? precision1 = null);
    }
}
