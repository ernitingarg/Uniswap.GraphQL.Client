using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    public class Uniswap : IUniswap
    {
        readonly IGraphQLClient _graphQlClient;
        const int MaxRetryCount = 3;

        public Uniswap(IGraphQLClient graphQlClient)
        {
            _graphQlClient = graphQlClient
                             ?? throw new ArgumentNullException(
                                 nameof(graphQlClient));
        }


        public async Task<LiquidityPosition> GetLiquidityPosition(
            int poolId,
            int? precision0 = null,
            int? precision1 = null)
        {
            int retry = 0;

            var query = new GraphQLRequest
            {
                Query = @"
                {
                  position(id: " + poolId + @") {
                    id
                    owner
                    token0 {
                      symbol
                      decimals
                    }
                    token1 {
                      symbol
                      decimals
                    }
                    pool {
                      sqrtPrice
                    }
                    liquidity
                    tickLower {
                      price0
                    }
                    tickUpper {
                      price0
                    }
                  }
                }
                "
            };

        API:
            GraphQLResponse<LiquidityPosition> response;
            try
            {
                response = await _graphQlClient.SendQueryAsync<LiquidityPosition>(query);
            }
            catch (Exception)
            {
                retry++;
                if (retry <= MaxRetryCount)
                {
                    goto API;
                }

                throw;
            }

            var result = response.Data;

            var amounts = PriceHelper.GetPositionAmounts(
                response.Data,
                precision0,
                precision1);

            result.Position.Amount0 = amounts.Amount0;
            result.Position.Amount1 = amounts.Amount1;

            return result;
        }
    }
}