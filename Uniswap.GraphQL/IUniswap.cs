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
        /// <param name="maxRetryCount">Maximum number of retries in case of failure from server</param>
        /// <param name="precisionForPosition0">decimal precision for contract0 position</param>
        /// <param name="precisionForPosition1">decimal precision for contract1 position</param>
        /// /// <param name="precisionForCurrentPrice">decimal precision for current price of contract0:contract1</param>
        /// <returns></returns>
        Task<LiquidityPosition> GetLiquidityPosition(
            int poolId,
            int maxRetryCount,
            int? precisionForPosition0 = null,
            int? precisionForPosition1 = null,
            int? precisionForCurrentPrice = null);
    }
}
