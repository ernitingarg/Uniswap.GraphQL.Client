using System.Threading.Tasks;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    public interface IUniswap
    {
        Task<TopTokens> GetTopTokens();
        Task<OwnerPosition> GetOwnerPosition(int poolId);
    }
}
