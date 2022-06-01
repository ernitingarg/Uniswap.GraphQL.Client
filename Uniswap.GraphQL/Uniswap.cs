using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using Uniswap.GraphQL.Entities;

namespace Uniswap.GraphQL
{
    public class Uniswap : IUniswap
    {
        private readonly IGraphQLClient _graphQlClient;

        public Uniswap(IGraphQLClient graphQlClient)
        {
            _graphQlClient = graphQlClient
                             ?? throw new ArgumentNullException(
                                 nameof(graphQlClient));
        }

        public async Task<TopTokens> GetTopTokens()
        {
            var query = new GraphQLRequest
            {
                Query = @"
                {
                  tokens (first: 5, orderDirection: desc){
                    symbol
                    name
                    volume
                    volumeUSD
                    totalSupply
                  }
                }
                "
            };

            var response = await _graphQlClient.SendQueryAsync<TopTokens>(query);
            return response.Data;
        }

        public async Task<OwnerPosition> GetOwnerPosition(int poolId)
        {
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

            var response = await _graphQlClient.SendQueryAsync<OwnerPosition>(query);
            return response.Data;
        }
    }
}